using COTG.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using static COTG.Debug;

namespace COTG.Game
{
    static class WorldHelper
    {
        static internal uint SubStrAsInt(this string s, int start, int count)
        {
            uint rv = default;
            for (int i = start; i < start + count; ++i)
            {
                rv *= 10;
                var c = i < s.Length ? s[i] : '0';
                Assert(c >= '0');
                Assert(c <= '9');
                rv += (uint)(c - '0');
            }
            return rv;
        }
        static public ushort SubStrAsShort(this string s, int start, int count)
        {
            return (ushort)SubStrAsInt(s, start, count);
        }
        static public byte SubStrAsByte(this string s, int start, int count)
        {
            return (byte)SubStrAsInt(s, start, count);
        }
        public static uint RGB16(uint r, uint g, uint b) => ((r >> 3) << 11) | ((g >> 2) << 5) | (b >> 3);
        public static byte RGB16B0(uint r, uint g, uint b) => (byte)(RGB16(r, g, b) & 0xff);
        public static byte RGB16B1(uint r, uint g, uint b) => (byte)(RGB16(r, g, b) >> 8);
        public static void SetColor(this byte[] pixels, int index, uint r, uint g, uint b)
        {
            SetColor(pixels, index, RGB16(r, g, b));
        }
        public static void SetColor(this byte[] pixels, int index, uint color16)
        {
            pixels[index * 8 + 2] = (byte)(color16);
            pixels[index * 8 + 3] = (byte)(color16 >> 8);
        }
        static public uint FaithColor16(int type)
        {
            if (type == 7)
                return RGB16(0xff,0xB0,0x00);
            return RGB16((type & 1) != 0 ? 0xFFu : 0x3Fu, (type & 2) != 0 ? 0xFFu : 0x3Fu, (type & 4) != 0 ? 0xFFu : 0x3Fu);

        }

        public static int WorldToCid( this (int x, int y) a )
        {
            return a.x + a.y*65536;
        }
        public static (int x,int y) CidToWorld(this int c)
        {
            return (c%65536,c/65536);
        }
        public static Vector2 CidToWorldV(this int c)
        {
            var c2 = CidToWorld(c);
            return new Vector2((float)c2.x, (float)c2.y);
        }
        public static int WorldToContinent(this (int x, int y) c) =>   (c.y/100)* 10 + (c.x/100);
        public static int CidToContinent(this int cid) => WorldToContinent( CidToWorld(cid) );

    }

    public class World
    {
        public static World current;
        public const int worldDim = 600;
        public const int outSize = 2400;
        public const uint typeMask = 0xf0000000;
        public const uint dataMask = 0x0fffffff;
        public const uint typeCity = 0x10000000;
        public const uint typeShrine = 0x20000000;
        public const uint typePortal = 0x30000000;
        public const uint typeBoss = 0x40000000;
        public const uint typeDungeon = 0x50000000;
        public const uint typeNone = 0x00000000;
        public const int playerMask = 0x00ffffff;
        public const int typeCityFlagCastle = 0x1000000;
        public const int typeCityFlagTemple = 0x2000000;
        public const int typeCityFlagWater = 0x4000000;
        public const int typeCityFlagBig = 0x8000000;

        public static byte[] bitmapPixels;// = new byte[outSize / 4 * outSize / 4 * 8];

        public static uint[] raw = new uint[worldDim*worldDim]; // reference with CID, stores playerID
        public static int ClampCoord(int x)
        {
            return x.Clamp(0, worldDim);
        }
        public static (uint type, int player, bool isCastle, bool isBig, bool isWater, bool isTemple) CityLookup((int x, int y) c)
        {
            var x = c.x;
            var y = c.y;
            uint rv = (x >= 0 && x < worldDim && y >= 0 && y < worldDim) ? raw[x+y*worldDim] : 0u;
            return (rv & typeMask, (int)(rv & playerMask),
                (rv & typeCityFlagCastle) != 0,
                (rv & typeCityFlagBig) != 0,
                (rv & typeCityFlagWater) != 0,
                (rv & typeCityFlagTemple) != 0);

        }
        public static (uint type, uint data) RawLookup((int x, int y) c)
        {
            var x = c.x;
            var y = c.y;
            uint rv = (x >= 0 && x < worldDim && y >= 0 && y < worldDim) ? raw[x + y * worldDim] : 0u;
            return (rv & typeMask, (rv & dataMask));

        }

        public struct Boss
        {
            public byte level;
            public byte type;
            public ushort x;
            public ushort y;
        }
        public Boss[] bosses;

        public struct Shrine
        {
            public byte type;
            public ushort x;
            public ushort y;

        }
        public Shrine[] shrines;

        public struct Portal
        {
            public bool active;
            public ushort x;
            public ushort y;

        }
        public Portal[] portals;

        //        public struct City
        //        {
        //            public int playerId;
        //            public int allianceId;
        //            public ushort x;
        //            public ushort y;
        //            public byte type;
        //            public bool isCastle => type switch
        //            {
        //                3 => true,
        //                4 => true,
        //                7 => true,
        //                8 => true,
        //                _ => false

        //            };
        //        }
        ////        public City[] cities;

        static ulong AsNumber(string s)
        {
            if (ulong.TryParse(s, out var rv) == false)
                rv = 0;
            return rv;
        }
        public static void UpdateCurrent(JsonDocument jsd)
        {
            current = Decode(jsd);
        }

        //public static (int size, byte[] pixels) CreateBitmap()
        //{
        //    // fill with Alpha=clear
        //    //for (int i = 0; i < outSize / 4 * outSize / 4 ; i ++)
        //    //{
        //    //    pixels[i * 8 + 0] = 0;
        //    //    pixels[i * 8 + 1] = 0;
        //    //    pixels[i * 8 + 2] = 0;
        //    //    pixels[i * 8 + 3] = 0;
        //    //    pixels[i * 8 + 4] = 0xff;
        //    //    pixels[i * 8 + 5] = 0xff;
        //    //    pixels[i * 8 + 6] = 0xff;
        //    //    pixels[i * 8 + 7] = 0xff;
        //    //}
        //    //for (int i = 0; i < outSize / 4 * outSize / 4 - 8; i += 9)
        //    //{
        //    //    pixels[i * 8 + 0] = 0;
        //    //    pixels[i * 8 + 1] = 0;
        //    //    pixels[i * 8 + 2] = 0;
        //    //    pixels[i * 8 + 3] = 0;
        //    //    pixels[i * 8 + 4] = 0xf;
        //    //    pixels[i * 8 + 5] = 0xff;
        //    //    pixels[i * 8 + 6] = 0x0;
        //    //    pixels[i * 8 + 7] = 0xf0;
        //    //}

        //    return (outSize, pixels);

        //}

        public static World Decode(JsonDocument jsd)
        {
            var pixels = new byte[outSize / 4 * outSize / 4 * 8];
            Array.Clear(raw, 0, raw.Length); // Todo:  remove decaying cities?

            // fill with Alpha=clear
            for (int i = 0; i < outSize / 4 * outSize / 4; i++)
            {
                pixels[i * 8 + 0] = 0;
                pixels[i * 8 + 1] = 0;
                pixels[i * 8 + 2] = 0;
                pixels[i * 8 + 3] = 0;
                pixels[i * 8 + 4] = 0xff;
                pixels[i * 8 + 5] = 0xff;
                pixels[i * 8 + 6] = 0xff;
                pixels[i * 8 + 7] = 0xff;
            }

            var data = jsd.RootElement.GetProperty("a").GetString(); // we do at least one utf16 <-> utf8 round trip here
                                                                     //          List<City> cities = new List<City>(1024);
            List<Boss> bosses = new List<Boss>(32);
            List<Shrine> shrines = new List<Shrine>(128);
            List<Portal> portals = new List<Portal>(128);

            var temp = data.Split("|");
            var keys = temp[1].Split("l");
            var ckey = AsNumber(keys[0]);
            var skey_ = AsNumber(keys[1]);
            var bkey_ = AsNumber(keys[2]);
            var lkey_ = AsNumber(keys[3]);
            var cavkey_ = AsNumber(keys[4]);
            var pkey_ = AsNumber(keys[5]);
            var cities_ = temp[0].Split("l");
            var shrines_ = temp[2].Split("l");
            var bosses_1 = temp[3].Split("l");
            var lawless_ = temp[4].Split("l");
            var caverns_ = temp[5].Split("l");
            var portals_ = temp[6].Split("l");
            var rv = new World();
            /** @type {string} */
            foreach (var id in bosses_1)
            {
                if (id == "")
                    continue;
                /** @type {string} */
                var dat_ = AsNumber(id) + (bkey_);
                /** @type {string} */
                bkey_ = dat_;
                var _t = dat_.ToString();
                var b = new Boss()
                {
                    x = (ushort)(_t.SubStrAsInt(6, 3) - 100),
                    y = (ushort)(_t.SubStrAsInt(3, 3) - 100),
                    level = (byte)(_t.SubStrAsByte(0, 2) - 10),
                    type = _t.SubStrAsByte(2, 1)
                };
                //  LogJS(b);
                bosses.Add(b);
                var index = (int)(b.x + b.y * worldDim);
                pixels.SetColor(index, 0xF0, 0xF0, 0x40);
                pixels[index * 8 + 4] = 1 | (3 << 2) | (1 << 4) | (3 << 6);
                pixels[index * 8 + 5] = 3 | (2 << 2) | (3 << 4) | (2 << 6); // color index 0
                pixels[index * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (3 << 6); // color index 0
                pixels[index * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (2 << 6);

                raw[index] = b.level | (uint)(b.type << 4) | typeBoss;

            }
            foreach (var id in caverns_)
            {
                if (id == "")
                    continue;
                /** @type {string} */
                var dat_ = AsNumber(id) + (cavkey_);
                /** @type {string} */
                cavkey_ = dat_;
                var _t = dat_.ToString();
                var x = (ushort)(_t.SubStrAsInt(5, 3) - 100);
                var y = (ushort)(_t.SubStrAsInt(2, 3) - 100);
                var level = _t.SubStrAsByte(0, 2) - 10;

                //  LogJS(b);
                var index = (int)(x + y * worldDim);
                pixels.SetColor(index, 0x90, (byte)(0xD0 - level * 8), (byte)(0x40 + level * 7));
                float t = (level - 1) / 9.0f;
                switch (level)
                {
                    case 1:
                    case 2:
                        {
                            pixels[index * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
                            pixels[index * 8 + 5] = 3 | (1 << 2) | (3 << 4) | (3 << 6); // color index 0
                            pixels[index * 8 + 6] = 3 | (3 << 2) | (2 << 4) | (3 << 6); // color index 0
                            pixels[index * 8 + 7] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
                        }
                        break;
                    case 3:
                    case 4:
                        {
                            pixels[index * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
                            pixels[index * 8 + 5] = 3 | (3 << 2) | (1 << 4) | (3 << 6); // color index 0
                            pixels[index * 8 + 6] = 3 | (1 << 2) | (3 << 4) | (2 << 6); // color index 0
                            pixels[index * 8 + 7] = 3 | (3 << 2) | (2 << 4) | (3 << 6);
                        }
                        break;
                    case 5:
                        {
                            pixels[index * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
                            pixels[index * 8 + 5] = 1 | (3 << 2) | (1 << 4) | (3 << 6); // color index 0
                            pixels[index * 8 + 6] = 3 | (1 << 2) | (3 << 4) | (2 << 6); // color index 0
                            pixels[index * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (3 << 6);
                        }
                        break;
                    case 6:
                        {
                            pixels[index * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
                            pixels[index * 8 + 5] = 3 | (1 << 2) | (3 << 4) | (3 << 6); // color index 0
                            pixels[index * 8 + 6] = 1 | (3 << 2) | (1 << 4) | (3 << 6); // color index 0
                            pixels[index * 8 + 7] = 3 | (2 << 2) | (3 << 4) | (2 << 6);
                        }
                        break;
                    case 7:
                        {
                            pixels[index * 8 + 4] = 3 | (3 << 2) | (1 << 4) | (3 << 6);
                            pixels[index * 8 + 5] = 1 | (3 << 2) | (3 << 4) | (2 << 6); // color index 0
                            pixels[index * 8 + 6] = 3 | (1 << 2) | (3 << 4) | (3 << 6); // color index 0
                            pixels[index * 8 + 7] = 3 | (3 << 2) | (2 << 4) | (3 << 6);
                        }
                        break;
                    case 8:
                        {
                            pixels[index * 8 + 4] = 3 | (1 << 2) | (3 << 4) | (3 << 6);
                            pixels[index * 8 + 5] = 1 | (3 << 2) | (2 << 4) | (3 << 6); // color index 0
                            pixels[index * 8 + 6] = 3 | (2 << 2) | (1 << 4) | (3 << 6); // color index 0
                            pixels[index * 8 + 7] = 3 | (3 << 2) | (3 << 4) | (2 << 6);
                        }
                        break;
                    case 9:
                        {
                            pixels[index * 8 + 4] = 3 | (3 << 2) | (1 << 4) | (3 << 6);
                            pixels[index * 8 + 5] = 3 | (1 << 2) | (3 << 4) | (2 << 6); // color index 0
                            pixels[index * 8 + 6] = 1 | (3 << 2) | (1 << 4) | (3 << 6); // color index 0
                            pixels[index * 8 + 7] = 3 | (2 << 2) | (3 << 4) | (2 << 6);
                        }
                        break;

                    default:
                        {
                            pixels[index * 8 + 4] = 1 | (1 << 2) | (3 << 4) | (3 << 6);
                            pixels[index * 8 + 5] = 3 | (2 << 2) | (1 << 4) | (3 << 6); // color index 0
                            pixels[index * 8 + 6] = 1 | (3 << 2) | (1 << 4) | (2 << 6); // color index 0
                            pixels[index * 8 + 7] = 3 | (2 << 2) | (3 << 4) | (2 << 6);
                        }
                        break;
                }
                raw[index] = (uint)(level | typeDungeon);

            }
            foreach (var id in shrines_)
            {
                if (id == "")
                    continue;
                /** @type {string} */
                var dat_ = AsNumber(id) + (skey_);
                /** @type {string} */
                skey_ = dat_;
                var _t = dat_.ToString();
                var b = new Shrine()
                {
                    x = (ushort)(_t.SubStrAsInt(5, 3) - 100),
                    y = (ushort)(_t.SubStrAsInt(2, 3) - 100),
                    type = (byte)(_t.SubStrAsInt(0, 1) == 1 ? 255 : _t.SubStrAsByte(1, 1))
                };
                //  LogJS(b);
                shrines.Add(b);
                var index = (int)(b.x + b.y * worldDim);
                if (b.type == 255)
                {
                    pixels.SetColor(index, 0xC0, 0xD0, 0xC0);
                }
                else
                {
                    pixels.SetColor(index, WorldHelper.FaithColor16(b.type));

                }
                pixels[index * 8 + 4] = 1 | (3 << 2) | (1 << 4) | (3 << 6);
                pixels[index * 8 + 5] = 1 | (2 << 2) | (1 << 4) | (2 << 6); // color index 0
                pixels[index * 8 + 6] = 1 | (2 << 2) | (1 << 4) | (2 << 6); // color index 0
                pixels[index * 8 + 7] = 3 | (2 << 2) | (3 << 4) | (2 << 6);
                raw[index] = b.type | typeShrine;
            }

            foreach (var id in portals_)
            {
                if (id == "")
                    continue;
                /** @type {string} */
                var dat_ = AsNumber(id) + (pkey_);
                /** @type {string} */
                pkey_ = dat_;
                var _t = dat_.ToString();
                var b = new Portal()
                {
                    x = (ushort)(_t.SubStrAsInt(4, 3) - 100),
                    y = (ushort)(_t.SubStrAsInt(1, 3) - 100),
                    active = (_t.SubStrAsInt(0, 1) == 2)
                };
                portals.Add(b);
                var index = (int)(b.x + b.y * worldDim);
                if (b.active)
                {
                    pixels.SetColor(index, 0xFA, 0xFA, 0xA1);
                }
                else
                {
                    pixels.SetColor(index, 0xBA, 0xBA, 0xA0);
                }
                pixels[index * 8 + 4] = 1 | (1 << 2) | (1 << 4) | (3 << 6);
                pixels[index * 8 + 5] = 1 | (2 << 2) | (1 << 4) | (2 << 6); // color index 0
                pixels[index * 8 + 6] = 1 | (2 << 2) | (1 << 4) | (2 << 6); // color index 0
                pixels[index * 8 + 7] = 3 | (2 << 2) | (3 << 4) | (2 << 6);
                raw[index] = (b.active ? 1u : 0) | typePortal;
            }

            for (int isLL = 0; isLL < 2; ++isLL)
            {
                var ll = isLL == 1;
                var key = ll ? lkey_ : ckey;
                foreach (var id in ll ? lawless_ : cities_)
                {
                    if (id == "")
                        continue;
                    /** @type {string} */
                    var dat_ = AsNumber(id) + (key);
                    /** @type {string} */
                    key = dat_;
                    var _t = dat_.ToString();
                    uint x, y, pid, alliance, type;
                    var isTemple = false;
                    if (ll)
                    {
                        x = (_t.SubStrAsInt(4, 3) - 100);
                        y = (_t.SubStrAsInt(1, 3) - 100);
                        Assert(x < 600);
                        Assert(y < 600);
                        pid = 0;
                        alliance = 0;
                        // TODO:
                        type = _t.SubStrAsInt(0, 1) == 1 ? 1u : 7u;
                    }
                    else
                    {
                        var digitCount = _t.SubStrAsInt(10, 1);
                        pid = _t.SubStrAsInt(11, (int)digitCount);
                        int aliStart = 11 + (int)digitCount;
                        alliance = (_t.SubStrAsInt(aliStart, _t.Length - aliStart));
                        x = (_t.SubStrAsInt(7, 3) - 100);
                        y = (_t.SubStrAsInt(4, 3) - 100);
                        type = _t.SubStrAsByte(3, 1);
                        if ((int)_t.SubStrAsInt(0, 2) > 10)
                        {
                            isTemple = true;
                        }
                    }
                    //if (pid == JSClient.jsVars.pid)
                    //{
                    //    LogJS(c);
                    //    Log(_t);
                    //}
                    // cities.Add(c);
                    var index = (int)(x + y * worldDim);
                    if (isTemple)
                        pixels[index * 8 + 0] = 31;  // temple.  Neutral color is blue


                    if (pid == 0)
                    {
                        pixels.SetColor(index, 0xA0, 0x00, 0xB0);
                    }
                    else if (pid == JSClient.jsVars.pid)
                        pixels.SetColor(index, 0x60, 0xd0, 0x40);
                    else if (alliance == Alliance.my.id)
                        pixels.SetColor(index, 0x30, 0xa0, 0x30);
                    else
                    {
                        switch (Alliance.GetDiplomacy((int)alliance))
                        {
                            case Diplomacy.none:
                                pixels.SetColor(index, 0x80, 0x80, 0x80);
                                break;
                            case Diplomacy.allied:
                                pixels.SetColor(index, 0x20, 0xA0, 0x00);
                                break;
                            case Diplomacy.nap:
                                pixels.SetColor(index, 0x40, 0x80, 0x40);
                                break;
                            case Diplomacy.enemy:
                                pixels.SetColor(index, 0xB0, 0x30, 0x20);
                                break;
                            default:
                                break;
                        }
                    }
                    var isBig = type >= 5 ? 1 : 0;
                    var isCastle = 0;
                    var isWater = (type & 1);

                    if (type == 3 || type == 4) // 3,4 is on/off water
                    {
                        isCastle = 1;
                        pixels[index * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
                        pixels[index * 8 + 5] = (byte)(1 | ((isTemple ? 0 : 3) << 2) | (1 << 4) | (3 << 6)); // color index 0
                        pixels[index * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
                        pixels[index * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (2 << 6);
                    }
                    else if (type == 7 || type == 8) // 7 is on water
                    {
                        isCastle = 1;
                        pixels[index * 8 + 4] = (byte)(1 | ((isTemple ? 0 : 3) << 2) | (1 << 4) | (3 << 6));
                        pixels[index * 8 + 5] = 1 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
                        pixels[index * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
                        pixels[index * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (2 << 6);
                    }
                    else if (type == 1 || type == 2)
                    {
                        // City
                        pixels[index * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
                        pixels[index * 8 + 5] = 3 | (3 << 2) | (3 << 4) | (3 << 6); // color index 0
                        pixels[index * 8 + 6] = 3 | (1 << 2) | (1 << 4) | (3 << 6); // color index 0
                        pixels[index * 8 + 7] = 3 | (3 << 2) | (2 << 4) | (2 << 6);
                    }
                    else // if (type == 5 || type == 6)
                    {
                        // City
                        pixels[index * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
                        pixels[index * 8 + 5] = 3 | (1 << 2) | (1 << 4) | (3 << 6); // color index 0
                        pixels[index * 8 + 6] = 3 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
                        pixels[index * 8 + 7] = 3 | (3 << 2) | (2 << 4) | (2 << 6);
                    }
                    raw[index] = (uint)(pid | typeCity | isBig * typeCityFlagBig | isCastle * typeCityFlagCastle | (isTemple ? typeCityFlagTemple : 0) | isWater * typeCityFlagWater);



                }
            }
            //foreach (var id in lawless_) {
            //              /** @type {string} */
            //              var dat_ = AsNumber(lawless_[i_3]) + (lkey_);
            //	/** @type {string} */
            //	lkey_ = dat_;
            //	WorldData.ll.push(`3${dat_);
            //}
            //foreach (var id in caverns_) {
            //              /** @type {string} */
            //              var dat_ = AsNumber(caverns_[i_3]) + (cavkey_);
            //	/** @type {string} */
            //	cavkey_ = dat_;
            //	WorldData.cavern.push(`7${dat_);
            //}
            //foreach (var id in portals_) {
            //              /** @type {string} */
            //              var dat_ = AsNumber(portals_[i_3]) + (pkey_);
            //	/** @type {string} */
            //	pkey_ = dat_;
            //	WorldData.portals.push(`8${dat_);
            //}
            //foreach (var id in shrines_) {
            //              /** @type {string} */
            //              var dat_ = AsNumber(shrines_[i_3]) + (skey_);
            //	/** @type {string} */
            //	skey_ = dat_;
            //	WorldData.shrines.push(`9${dat_);
            //}
            // var ckey = __a6.ccazzx.encrypt(currentTime(), '1QA64sa23511sJx1e2', 256);
            // console.log(ckey);
            // var cdat = {
            // 	a: ckey
            // };
            //  console.log(JSON.stringify(cdat));
            //  console.log(cdat);
            // jQuery.ajax({
            // 	url: 'includes/pD.php',
            // 	type: 'POST',
            // 	async: true,
            // 	data: cdat,
            // 	success: function (data) {
            //     console.log(data);
            // 		pdata = (data);
            // 	}
            // });

            //            rv.cities = cities.ToArray();
            rv.portals = portals.ToArray();
            rv.shrines = shrines.ToArray();
            rv.bosses = bosses.ToArray();
            bitmapPixels = pixels;
            Task.Run(() => WorldStorage.SaveWorldData(raw) );
            return rv;
        }
        public static (string label,bool isMine) GetLabel((int x, int y) c)
        {
            var data = CityLookup(c);
            switch (data.type)
            {
                case World.typeCity:
                    {

                        if (data.player == 0)
                        {
                            return (null,false); // lawless
                        }
                        else
                        {
                            var isMine = false;
                            var player = Player.all.GetValueOrDefault(data.player, Player._default);
                            if (Player.IsMe(data.player))
                            {
                                isMine = true;
                                if (City.allCities.TryGetValue(c.WorldToCid(), out var city))
                                {
                                    return (city.cityName,true);
                                }

                            }
                            return (player.name,isMine);
                        }
                    }
                default:
                    return (null,false);
            }
        }
    }
}
