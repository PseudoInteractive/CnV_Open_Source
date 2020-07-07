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
        static internal ushort SubStrAsShort(this string s, int start, int count)
        {
            return (ushort)SubStrAsInt(s, start, count);
        }
        static internal byte SubStrAsByte(this string s, int start, int count)
        {
            return (byte)SubStrAsInt(s, start, count);
        }
        public static uint RGB16(uint r, uint g, uint b) => ((r >> 3) << 11) | ((g >> 2) << 5) | (b >> 3);
        public static byte RGB16B0(uint r, uint g, uint b) => (byte)(RGB16(r, g, b) & 0xff);
        public static byte RGB16B1(uint r, uint g, uint b) => (byte)(RGB16(r, g, b) >> 8);
        public static void SetColor(this byte[] pixels, int index, uint r, uint g, uint b)
        {
            var c = RGB16(r, g, b);
            pixels[index * 8 + 2] = (byte)(c);
            pixels[index * 8 + 3] = (byte)(c >> 8);
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
        public const uint typeCity = 0x10000000;
        public const uint typeNone = 0x00000000;
        public const  int dataMask = 0x0fffffff;

        public static byte[] bitmapPixels;// = new byte[outSize / 4 * outSize / 4 * 8];

        public static uint[,] cityLookup = new uint[worldDim, worldDim]; // reference with CID, stores playerID
        public static int ClampCoord(int x)
        {
            return x.Clamp(0, worldDim);
        }
        public static (uint type, int data) CityLookup( (int x, int y) c)
        {
            var x = c.x;
            var y = c.y;
            uint rv = (x >= 0 && x < worldDim && y >= 0 && y < worldDim) ? cityLookup[x, y] : 0u;
            return (rv & typeMask, (int)(rv & dataMask));

        }
        
        public struct Boss
        {
            public byte level;
            public ushort x;
            public ushort y;

            public override string ToString()
            {
                return $"{{{x}:{y},level:{level}}}";
            }
        }
        public Boss[] bosses;
        public struct City
        {
            public int playerId;
            public int allianceId;
            public ushort x;
            public ushort y;
            public byte type;
            public bool isCastle => type switch
            {
                3 => true,
                4 => true,
                7 => true,
                8 => true,
                _ => false

            };

            public override string ToString()
            {
                return $"{{{x}:{y},pid:{playerId},type:{type},c:{isCastle}}}";
            }
        }
        public City[] cities;

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
            Array.Clear(cityLookup,0,cityLookup.Length); // Todo:  remove decaying cities?

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
            List<City> cities = new List<City>(1024);
            List<Boss> bosses = new List<Boss>(128);

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

                try
                {
                    if (id == "")
                        continue;
                    /** @type {string} */
                    var dat_ = AsNumber(id) + (bkey_);
                    /** @type {string} */
                    bkey_ = dat_;
                    var _t = dat_.ToString();
                    var b = new Boss() { x = (ushort)(_t.SubStrAsInt(6, 3)-100), y = (ushort)(_t.SubStrAsInt(3, 3)-100), level = _t.SubStrAsByte(0, 2) };
                  //  LogJS(b);
                    bosses.Add(b);
                }
                catch (Exception e)
                {
                    Log(e);
                }


            }
            var rand = new Random();
            foreach (var id in cities_)
            {
                try
                {
                    if (id == "")
                        continue;
                    /** @type {string} */
                    var dat_ = AsNumber(id) + (ckey);
                    /** @type {string} */
                    ckey = dat_;
                    var _t = dat_.ToString();

                    var digitCount = _t.SubStrAsInt(10, 1);
                    var pid = (int) _t.SubStrAsInt(11, (int)digitCount);
                    int aliStart = 11 + (int)digitCount;
                    var alid = (int)_t.SubStrAsInt(aliStart,_t.Length-aliStart);
                    var c = (new City() { x = (ushort)(_t.SubStrAsInt(7, 3)-100), y = (ushort)(_t.SubStrAsInt(4, 3)-100), playerId = pid, allianceId=alid, type = _t.SubStrAsByte(3, 1) });
                    //if (pid == JSClient.jsVars.pid)
                    //{
                    //    LogJS(c);
                    //    Log(_t);
                    //}
                    cities.Add(c);
                    var index = c.x + c.y * worldDim;
                    pixels[index * 8 + 0] = 0;
                    pixels[index * 8 + 1] = 0;
                    if (c.playerId == JSClient.jsVars.pid)
                        pixels.SetColor(index, 0x60, 0xd0, 0x40);
                    else if (c.allianceId == Alliance.my.id)
                        pixels.SetColor(index, 0x30, 0xa0,0x30);
                    else
                    {
                        switch (Alliance.GetDiplomacy(c.allianceId))
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
                    if (c.type ==  3|| c.type==4)
                    {

                        pixels[index * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
                        pixels[index * 8 + 5] = 1 | (3 << 2) | (1 << 4) | (3 << 6); // color index 0
                        pixels[index * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
                        pixels[index * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (2 << 6);
                    }
                    else if (c.type == 7 || c.type == 8)
                    {

                        pixels[index * 8 + 4] = 1 | (3 << 2) | (1 << 4) | (3 << 6);
                        pixels[index * 8 + 5] = 1 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
                        pixels[index * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
                        pixels[index * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (2 << 6);
                    }
                    else
                    {
                        pixels[index * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
                        pixels[index * 8 + 5] = 3 | (1 << 2) | (1 << 4) | (3 << 6); // color index 0
                        pixels[index * 8 + 6] = 3 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
                        pixels[index * 8 + 7] = 3 | (3 << 2) | (2 << 4) | (2 << 6);
                    }
                    cityLookup[c.x, c.y] = (uint)pid | typeCity ;
                    
                }
                catch (Exception e)
                {
                    Log(e);
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

            rv.cities = cities.ToArray();
            rv.bosses = bosses.ToArray();
            bitmapPixels = pixels;
            return rv;
        }

    }
}
