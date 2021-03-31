using COTG.Helpers;
using COTG.Services;
using COTG.Views;

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

using static COTG.Debug;
using static COTG.Game.WorldViewSettings;
using Vector2 = System.Numerics.Vector2;

namespace COTG.Game
{
	static class WorldHelper
	{
		public static int CidToPid(this int cid)
		{
			return World.GetInfo(CidToWorld(cid)).player;
		}
		static internal uint SubStrAsInt(this string s, int start, int count)
		{
			uint rv = default;
			for (int i = start; i < start + count; ++i)
			{
				var c = i < s.Length ? s[i] : '0';
				if (c == ',')
					continue;
				rv *= 10;
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
			SetColor(pixels, index, RGB16(r, g, b), 2);
		}
		public static void SetColor0(this byte[] pixels, int index, uint r, uint g, uint b)
		{
			SetColor(pixels, index, RGB16(r, g, b), 0);
		}
		public static void SetColor(this byte[] pixels, int index, uint color16, int offset)
		{
			pixels[index * 8 + offset] = (byte)(color16);
			pixels[index * 8 + offset + 1] = (byte)(color16 >> 8);
		}
		public static void SetColor(this byte[] pixels,byte[] ownerPixels, int index, uint r, uint g, uint b)
		{
			SetColor(pixels, index, RGB16(r, g, b), 2);
			SetColor(ownerPixels, index, RGB16(r, g, b), 2);
		}

		static public uint FaithColor16(int type)
		{
			if (type == 7)// ibria is orange not white
				return RGB16(0xff, 0xB0, 0x00);
			if (type == 2)//vex is red, not green
				return RGB16(0xff, 0x3F, 0x6F);
			if (type == 1)//evara is green not ref
				return RGB16(0x3f, 0xFF, 0x3F);

			return RGB16((type & 1) != 0 ? 0xFFu : 0x3Fu, (type & 2) != 0 ? 0xFFu : 0x3Fu, (type & 4) != 0 ? 0xFFu : 0x3Fu);

		}

		public static int Translate(this int cid, (int dx, int dy) d)
		{
			return cid + d.dx + d.dy * 65536;
		}
		public static int WorldToCid(this (int x, int y) a)
		{
			return a.x + (a.y << 16);
		}

		public static Vector2 ToVector(this (int x, int y) a)
		{
			return new Vector2(a.x, a.y);
		}
		public static (int x, int y) CidToWorld(this int c)
		{
			return (c & 65535, c >> 16);
		}
		public static System.Numerics.Vector2 CidToWorldV(this int c)
		{
			var c2 = CidToWorld(c);
			return new System.Numerics.Vector2((float)c2.x, (float)c2.y);
		}
		public static int WorldToContinent(this (int x, int y) c) => (c.y / 100) * 10 + (c.x / 100);
		//        public static int CidToContinent(this int cid) => ((cid/65536)/100)*10 | (cid % 65536) / 100;
		public static int CidToContinent(this int cid) => WorldToContinent(CidToWorld(cid));

	}
	public struct Continent
	{
		public byte id;
		public ushort bosses;   // [6]
		public int unsettled; // [0]
		public int settled; // [1]
		public int cities; // [2]
		public int castles; // [3]
		public ushort dungeons; // [4]
		public ushort temples; // [5]
		public const int spanX = 6;
		public const int spanY = 6;
		public const int count = spanX * spanY + 1; // 56 is summary of the workd
		public static int GetPackedIdFromC((int x, int y) c) => GetPackedIdFromCont((c.x / 100, c.y / 100));
		public static int GetPackedIdFromCont((int x, int y) c) => c.x.Clamp(0, 5) + c.y.Clamp(0, 5) * spanX;
		public static int GetPackedIdFromCont(int cont)
		{
			var y = cont / 10;
			return GetPackedIdFromCont((cont - y*10,y));
		}
		public static int GetPackedIdFromContUnpacked(int cont) => cont == 56 ? count - 1 : (GetPackedIdFromCont(cont));
		public static Continent[] all = new Continent[count]; // 56 is a summary for world

	};

	public struct ContinentsSnapshot
	{
		public DateTime time { get; set; }
		public struct ContinentSnapshot
		{
			public int settled { get; set; } // includes castles
			public int castles { get; set; } // [3]
		};

		public ContinentSnapshot[] continents { get; set; }

		static public ContinentsSnapshot[] all = Array.Empty<ContinentsSnapshot>(); // ordered by date and time
	};

	public class Boss
	{
		public byte level { get; set; }
		public byte type { get; set; }
		public int cid;
		public string xy => cid.CidToString();
		public int cont => cid.CidToContinent();
		public float dist
		{
			get
			{

				if (distanceReference == null)
					return 0;
				var d = cid.CidToWorld().DistanceToCid(distanceReference.cid);
				var tt = distanceReference.GetRaidTroopType();
				var rv = d * Enum.ttTravel[tt] / (60f * Enum.ttSpeedBonus[tt]);
				if (Enum.IsWaterRaider(tt))
					rv += 1.0f;
				return rv;
			}
		}

		public override string ToString()
		{
			return $"Boss {xy} {cont} {level} {type}";
		}
		public static Boss[] all = Array.Empty<Boss>();
		public static City distanceReference; // Set temporarily for boss hunting.  Null usually
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
		public const int typeCityFlagMask = 0x00f000000;
		public const int typeCityFlagCastle = 0x001000000;
		public const int typeCityFlagTemple = 0x2000000;
		public const int typeCityFlagWater = 0x4000000;
		public const int typeCityFlagBig = 0x8000000;

		public static byte[] worldOwnerPixels;// = new byte[outSize / 4 * outSize / 4 * 8];
		public static byte[] bitmapPixels;// = new byte[outSize / 4 * outSize / 4 * 8];
		public static byte[] changePixels;// = new byte[outSize / 4 * outSize / 4 * 8];

		public static uint[] raw = new uint[worldDim * worldDim]; // reference with CID, stores playerID
		public static uint[] rawPrior; // set if heatmaps are enabled
		public static int ClampCoord(int x)
		{
			return x.Clamp(0, worldDim);
		}
		public static (uint type, int player, bool isCastle, bool isBig, bool isWater, bool isTemple, uint data) GetInfoFromPackedId(int packedId)
		{
			return GetInfo(raw, packedId);
		}
		public static (uint type, int player, bool isCastle, bool isBig, bool isWater, bool isTemple, uint data) GetInfo((int x, int y) c)
		{
			return GetInfo(raw, packedId: GetPackedId(c));
		}
		public static (uint type, int player, bool isCastle, bool isBig, bool isWater, bool isTemple, uint data) GetInfoFromCid(int cid)
		{
			return GetInfo(cid.CidToWorld());
		}
		public static (uint type, int player, bool isCastle, bool isBig, bool isWater, bool isTemple, uint data) GetInfoPrior(int packedId)
		{
			return GetInfo(rawPrior, packedId);
		}
		public static (uint type, int player, bool isCastle, bool isBig, bool isWater, bool isTemple, uint data) GetInfo(uint[] _raw, int packedId)
		{
			uint rv = _raw[packedId];
			return (rv & typeMask, (int)(rv & playerMask),
				(rv & typeCityFlagCastle) != 0,
				(rv & typeCityFlagBig) != 0,
				(rv & typeCityFlagWater) != 0,
				(rv & typeCityFlagTemple) != 0,
				rv);

		}
		public static int CidToPlayer( int _cid) => World.GetInfo(_cid.CidToWorld()).player;
		public static int GetPackedId((int x, int y) c)
		{
			var x = c.x;
			var y = c.y;
			return (x >= 0 & x < worldDim & y >= 0 & y < worldDim) ? x + y * worldDim : 0;
		}
		public static Microsoft.Xna.Framework.Color GetTint(int packedId)
		{
			if(!SettingsPage.tintCities)
				return new Color(255, 255, 255, 255);
			//	var packedId = GetPackedId(xy);
			uint rv = raw[packedId];
			switch (rv & typeMask)
			{
				case typeCity:
					{
						var pid = rv & playerMask;
						// lawless
						if (pid == 0)
							return lawless.color.HalfSaturation();
						if (pid == Player.myId)
							return ownCities.color.HalfSaturation();
						var alliance = Player.Get((int)pid).alliance;
						Color tint;
						if (alliance == Alliance.my.id)
						{
							tint = ownAlliance.color.HalfSaturation();
						}
						else
						{
							switch (Alliance.GetDiplomacy((int)alliance))
							{
								default:
									tint = otherPlayers.color.HalfSaturation();
									break;
								case Diplomacy.allied:
									tint = alliedAlliance.color.HalfSaturation();
									break;
								case Diplomacy.nap:
									tint = napAlliance.color.HalfSaturation();
									break;
								case Diplomacy.enemy:
									tint = enemyAlliance.color.HalfSaturation();
									break;


							}
						}

						if (allianceSettings.TryGetValue((int)alliance, out var allianceSetting) && allianceSetting.isOn)
						{
							tint = allianceSetting.color.HalfSaturation();
						}
						if (playerSettings.TryGetValue((int)pid, out var playerSetting) && playerSetting.isOn)
						{
							tint = playerSetting.color.HalfSaturation();
						}

						return tint;
					}
				default:
					return new Color(255, 255, 255, 255);
			}

		}
		public static (uint type, uint data) RawLookup((int x, int y) c)
		{
			var x = c.x;
			var y = c.y;
			uint rv = (x >= 0 & x < worldDim & y >= 0 & y < worldDim) ? raw[x + y * worldDim] : 0u;
			return (rv & typeMask, (rv & dataMask));

		}


		public struct Shrine
		{
			public byte type;
			public ushort x;
			public ushort y;

		}
		public Shrine[] shrineList;

		public struct Portal
		{
			public bool active;
			public ushort x;
			public ushort y;

		}
		public Portal[] portals;
		internal static bool changeMapInProgress;

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
			Decode(jsd);
		}


		public static void CreateChangePixels(uint[] prior)
		{
			var pixels = new byte[outSize / 4 * outSize / 4 * 8];
			for (int i = 0; i < outSize / 4 * outSize / 4; i++)
			{
				pixels[i * 8 + 0] = WorldHelper.RGB16B0(0x60, 0, 0);
				pixels[i * 8 + 1] = WorldHelper.RGB16B1(0x60, 0, 0);
				pixels[i * 8 + 2] = 0;
				pixels[i * 8 + 3] = 0;
				pixels[i * 8 + 4] = 0x55;
				pixels[i * 8 + 5] = 0x55;
				pixels[i * 8 + 6] = 0x55;
				pixels[i * 8 + 7] = 0x55;
			}
			for (int y = 0; y < worldDim; ++y)
			{
				for (int x = 0; x < worldDim; ++x)
				{
					var i = y * worldDim + x;
					var d0 = prior[i];
					var d1 = raw[i];
					if (d0 == 0)
					{
						// no change
					}
					else
					{
						var dtype0 = d0 & typeMask;
						var dtype1 = d1 & typeMask;
						if (d0 == d1 || dtype0 == typeDungeon || dtype0 == typeBoss || dtype1 == typeDungeon || dtype1 == typeBoss)
							continue;
						uint color = WorldHelper.RGB16(0x40, 0x40, 0x40);
						if (dtype0 == typeCity || dtype1 == typeCity)
						{
							var owner0 = d0 & playerMask;
							var owner1 = d1 & playerMask;
							var alliance0 = owner0 == 0 ? -1 : Player.Get((int)owner0).alliance;
							var alliance1 = owner1 == 0 ? -1 : Player.Get((int)owner1).alliance;

							if (alliance0 == alliance1)
							{
								var isCastle0 = d0 & typeCityFlagCastle;
								var isCastle1 = d1 & typeCityFlagCastle;
								// castle change or size change or handover  Dodo:  differentiate this two
								color = WorldHelper.RGB16(0x0, 0x0, isCastle0 != isCastle1 ? 0xA0u : 0x60u);
							}
							else if (alliance0 == Alliance.myId)
							{
								// lost one
								color = WorldHelper.RGB16(0xA0, 0, 0);
							}
							else if (alliance1 == Alliance.myId)
							{
								// gained one
								color = WorldHelper.RGB16(0, 0xA0, 0);
							}
							// Todo: handle more cases
						}

						pixels.SetColor(i, color, 0);
						// change:  Todo, analysis
						pixels[i * 8 + 4] = 1 | (3 << 2) | (2 << 4) | (1 << 6);
						pixels[i * 8 + 5] = 2 | (0 << 2) | (0 << 4) | (3 << 6); // color index 0
						pixels[i * 8 + 6] = 3 | (0 << 2) | (0 << 4) | (2 << 6); // color index 0
						pixels[i * 8 + 7] = 1 | (2 << 2) | (3 << 4) | (1 << 6);
					}
				}
			}
			changePixels = pixels;
			rawPrior = prior;
		}

		public enum State
		{
			none,
			started,
			partWay,
			completed,
		}
		public static State state = State.none;
		public static bool initialized => state >= State.partWay;
		public static bool completed => state >= State.completed;
		public static async void Decode(JsonDocument jsd)
		{
			Assert(state == State.started);
	///		Assert(state == State.none || state == State.completed);
	//		state = State.started;

			var pixels = new byte[outSize / 4 * outSize / 4 * 8];
			var ownerPixels = new byte[worldDim * worldDim * 8];
			for (int i = 0; i < worldDim * worldDim; i++)
			{
				//ownerPixels[i * 8 + 0] = WorldHelper.RGB16B0(0xcf, 0xcf, 0xcf);
				//ownerPixels[i * 8 + 1] = WorldHelper.RGB16B1(0xcf, 0xcf, 0xcf); // w
				//ownerPixels[i * 8 + 2] = (byte)(WorldHelper.RGB16B0(0xc0, 0xc0, 0xc0)); ; // white, identity for multiply
				//ownerPixels[i * 8 + 3] = (byte)(WorldHelper.RGB16B1(0xc0, 0xc0, 0xc0)); ;
				//ownerPixels[i * 8 + 4] = 0x0; // indexes can be either
				//ownerPixels[i * 8 + 5] = 0x0;
				//ownerPixels[i * 8 + 6] = 0x0;
				//ownerPixels[i * 8 + 7] = 0x0;
				ownerPixels[i * 8 + 0] = 0;
				ownerPixels[i * 8 + 1] = 0; // w
				ownerPixels[i * 8 + 2] = 0; // black, identity for alpha
				ownerPixels[i * 8 + 3] = 0;
				ownerPixels[i * 8 + 4] = 0xff;
				ownerPixels[i * 8 + 5] = 0xff;
				ownerPixels[i * 8 + 6] = 0xff;
				ownerPixels[i * 8 + 7] = 0xff;
			}

			// fill with Alpha=clear
			for (int i = 0; i < outSize / 4 * outSize / 4; i++)
			{
				pixels[i * 8 + 0] = 0;
				pixels[i * 8 + 1] = 0; // w
				pixels[i * 8 + 2] = 0; // black, identity for alpha
				pixels[i * 8 + 3] = 0;
				pixels[i * 8 + 4] = 0xff;
				pixels[i * 8 + 5] = 0xff;
				pixels[i * 8 + 6] = 0xff;
				pixels[i * 8 + 7] = 0xff;
			}

			var data = jsd.RootElement.GetProperty("a").GetString(); // we do at least one utf16 <-> utf8 round trip here
																	 //          List<City> cities = new List<City>(1024);
			List<Boss> bossList = new List<Boss>(32);
			List<Shrine> shrineList = new List<Shrine>(128);
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

			Array.Clear(raw, 0, raw.Length); // Todo:  remove decaying cities?

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
				var x = (int)(_t.SubStrAsInt(6, 3) - 100);
				var y = (int)(_t.SubStrAsInt(3, 3) - 100);

				var b = new Boss()
				{
					cid = (x, y).WorldToCid(),
					level = (byte)(_t.SubStrAsByte(0, 2) - 10),
					type = _t.SubStrAsByte(2, 1)
				};
				//  LogJS(b);
				bossList.Add(b);

				var index = (x + y * worldDim);
				raw[index] = b.level | (uint)(b.type << 4) | typeBoss;

				if (bosses.isOn && b.level >= bossMinLevel && b.level <= bossMaxLevel)
				{
					pixels.SetColor(index, bosses.color.R, bosses.color.G, bosses.color.B);
					pixels[index * 8 + 4] = 1 | (3 << 2) | (1 << 4) | (3 << 6);
					pixels[index * 8 + 5] = 3 | (2 << 2) | (3 << 4) | (2 << 6); // color index 0
					pixels[index * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (3 << 6); // color index 0
					pixels[index * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (2 << 6);
					//     Trace($"Boss: {b}");
				}
			}
			{
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
					if (caverns.isOn  && level >= cavernMinLevel && level <= cavernMaxLevel)
					{
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
					}
					raw[index] = (uint)(level | typeDungeon);

				}
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
				shrineList.Add(b);
				var index = (int)(b.x + b.y * worldDim);
				if (shrines.isOn)
				{
					
					if (b.type == 255)
					{
						pixels.SetColor( index, shrines.color.R, shrines.color.G, shrines.color.B);
					}
					else
					{
						pixels.SetColor(index, WorldHelper.FaithColor16(b.type), 2);

					}
					//ownerPixels[index * 8 + 4] = 0 | (0 << 2) | (1 << 4) | (1 << 6);
					//ownerPixels[index * 8 + 5] = 0 | (3 << 2) | (1 << 4) | (1 << 6); // color index 0
					//ownerPixels[index * 8 + 6] = 0 | (2 << 2) | (3 << 4) | (0 << 6); // color index 0
					//ownerPixels[index * 8 + 7] = 0 | (0 << 2) | (0 << 4) | (0 << 6);

					pixels[index * 8 + 4] = 1 | (3 << 2) | (1 << 4) | (3 << 6);
					pixels[index * 8 + 5] = 1 | (2 << 2) | (1 << 4) | (2 << 6); // color index 0
					pixels[index * 8 + 6] = 1 | (2 << 2) | (1 << 4) | (2 << 6); // color index 0
					pixels[index * 8 + 7] = 3 | (2 << 2) | (3 << 4) | (2 << 6);
				}
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
				bool isOn;
				if (b.active)
				{
					isOn = activePortals.isOn;
					pixels.SetColor(index, activePortals.color.R,activePortals.color.G,activePortals.color.B);
	//				ownerPixels.SetColor(index, 0xaA, 0xFA, 0xFF);
					pixels[index * 8 + 0] = 31;
				}
				else
				{
					isOn = inactivePortals.isOn;
					pixels.SetColor(index, inactivePortals.color.R, inactivePortals.color.G, inactivePortals.color.B);
	//				ownerPixels.SetColor(index, 0xBA, 0xBA, 0xA0);
				}
				if (isOn)
				{
					pixels[index * 8 + 4] = 3 | (3 << 2) | (1 << 4) | (3 << 6);
					pixels[index * 8 + 5] = 3 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
					pixels[index * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
					pixels[index * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (2 << 6);

					//ownerPixels[index * 8 + 4] = 0 | (1 << 2) | (1 << 4) | (1 << 6);
					//ownerPixels[index * 8 + 5] = 0 | (1 << 2) | (2 << 4) | (1 << 6); // color index 0
					//ownerPixels[index * 8 + 6] = 0 | (1 << 2) | (1 << 4) | (1 << 6); // color index 0
					//ownerPixels[index * 8 + 7] = 0 | (0 << 2) | (0 << 4) | (0 << 6);
				}

				raw[index] = (b.active ? 1u : 0) | typePortal;
			}

			for (int isLL = 0; isLL < 2; ++isLL)
			{
				var ll = isLL == 1;
				var key = ll ? lkey_ : ckey;
				/*
                 1 :small city
                 2: small city on water
                 3: small castle on water
                 4: small castle on land
                 5: big city, land
                 6: big city water
                 7: big castle on water
                 8: big castle on land
                 */
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
					//if (pid == Player.myId)
					//{
					//    LogJS(c);
					//    Log(_t);
					//}
					// cities.Add(c);
					var index = (int)(x + y * worldDim);

					var isBig = type >= 5 ? 1 : 0;
					var isCastle = (type == 3 | type == 4 | type == 7 | type == 8) ? 1 : 0;
					var isWater = (type == 2 | type == 3 | type == 6 | type == 7) ? 1 : 0;
					raw[index] = (uint)(pid
										| typeCity
										| (uint)isBig * typeCityFlagBig
										| (uint)isCastle * typeCityFlagCastle
										| (uint)(isTemple ? typeCityFlagTemple : 0)
										| (uint)isWater * typeCityFlagWater);

				}
			}
			state = State.partWay;

			int counter = 0;
			// Wait for alliance diplomacy for colors
			while (!Alliance.diplomacyFetched && counter++ < 16)
			{
				await Task.Delay(1000);
			}

			for (int isLL = 0; isLL < 2; ++isLL)
			{
				var ll = isLL == 1;
				var key = ll ? lkey_ : ckey;
				/*
                 1 :small city
                 2: small city on water
                 3: small castle on water
                 4: small castle on land
                 5: big city, land
                 6: big city water
                 7: big castle on water
                 8: big castle on land
                 */
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
					//if (pid == Player.myId)
					//{
					//    LogJS(c);
					//    Log(_t);
					//}
					// cities.Add(c);
					var index = (int)(x + y * worldDim);

					var isBig = type >= 5 ? 1 : 0;
					var isCastle = (type == 3 | type == 4 | type == 7 | type == 8) ? 1 : 0;
					var isWater = (type == 2 | type == 3 | type == 6 | type == 7) ? 1 : 0;

					if (isTemple)
						pixels[index * 8 + 0] = 31;  // temple.  Neutral color is blue
					var isVisible = true;
					
					if (!isTemple && !citiesWithoutTemples)
						isVisible = false;
					if( isCastle==0 && !citiesWithoutCastles)
						isVisible = false;
					if (isWater==0 && !citiesWithoutWater)
						isVisible = false;



					if (pid == 0)
					{
						// lawless
						pixels.SetColor(ownerPixels,index, lawless.color.R,lawless.color.G, lawless.color.B);
						if (!lawless.isOn)
							isVisible = false;
					}
					else if (pid == Player.myId)
					{
						pixels.SetColor(ownerPixels,index, ownCities.color.R,ownCities.color.G, ownCities.color.B);
						if (!ownCities.isOn)
							isVisible = false;

					}
					else if (alliance != 0 && alliance == Alliance.my.id)
					{
						if (!ownAlliance.isOn)
							isVisible = false;

						pixels.SetColor(ownerPixels,index, ownAlliance.color.R,ownAlliance.color.G, ownAlliance.color.B);
					}
					else
					{
						switch (Alliance.GetDiplomacy((int)alliance))
						{
							default:
								if (!otherPlayers.isOn)
									isVisible = false;
								pixels.SetColor(ownerPixels,index, otherPlayers.color.R, otherPlayers.color.G, otherPlayers.color.B);
								break;
							case Diplomacy.allied:
								if (!alliedAlliance.isOn)
									isVisible = false;
								pixels.SetColor(ownerPixels,index, alliedAlliance.color.R,alliedAlliance.color.G, alliedAlliance.color.B);
								break;
							case Diplomacy.nap:
								if (!napAlliance.isOn)
									isVisible = false;

								pixels.SetColor(ownerPixels,index, napAlliance.color.R, napAlliance.color.G, napAlliance.color.B);
								break;
							case Diplomacy.enemy:
								if (!enemyAlliance.isOn)
									isVisible = false;

								pixels.SetColor(ownerPixels,index, enemyAlliance.color.R,enemyAlliance.color.G,enemyAlliance.color.B);
								break;


						}
					}

					if (allianceSettings.TryGetValue((int)alliance, out var allianceSetting) && allianceSetting.isOn)
					{
						pixels.SetColor(ownerPixels,index, allianceSetting.color.R, allianceSetting.color.G, allianceSetting.color.B);
					}
					if (playerSettings.TryGetValue((int)pid, out var playerSetting) && playerSetting.isOn)
					{
						pixels.SetColor(ownerPixels, index, playerSetting.color.R, playerSetting.color.G, playerSetting.color.B);
					}

					if (isVisible)
					{
						ownerPixels[index * 8 + 4] = 1 | (1 << 2) | (1 << 4) | (1 << 6);
						ownerPixels[index * 8 + 5] = 1 | (1 << 2) | (1 << 4) | (1 << 6); // color index 0
						ownerPixels[index * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (1 << 6); // color index 0
						ownerPixels[index * 8 + 7] = 1 | (1 << 2) | (1 << 4) | (1 << 6);

						if (type == 3 || type == 4) // 3,4 is on/off water
						{
							pixels[index * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
							pixels[index * 8 + 5] = (byte)(1 | ((isTemple ? 0 : 3) << 2) | (1 << 4) | (3 << 6)); // color index 0
							pixels[index * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
							pixels[index * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (2 << 6);
						}
						else if (type == 7 || type == 8) // 7 is on water
						{
							pixels[index * 8 + 4] = (byte)(1 | ((isTemple ? 0 : 3) << 2) | (1 << 4) | (3 << 6));
							pixels[index * 8 + 5] = 1 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
							pixels[index * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
							pixels[index * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (2 << 6);
						}
						else if (type == 1 || type == 2) // 1 is lawless I 
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
					}
					else
					{
						int q = 0;
					}


				}
			}
			//            rv.cities = cities.ToArray();
			rv.portals = portals.ToArray();
			rv.shrineList = shrineList.ToArray();
			Boss.all = bossList.ToArray();
			bitmapPixels = pixels;
			//worldOwnerPixels = ownerPixels;

			{
				var contData = jsd.RootElement.GetProperty("b");//.ToString();
				var shot = new ContinentsSnapshot();
				shot.continents = new ContinentsSnapshot.ContinentSnapshot[Continent.count];

				foreach (var cnt in contData.EnumerateObject())
				{
					var cntV = cnt.Value;
					var key = int.Parse(cnt.Name);
					var contId = Continent.GetPackedIdFromContUnpacked(key);
					Continent.all[contId].id = (byte)key;
					Continent.all[contId].unsettled = cntV[0].GetInt32();
					Continent.all[contId].settled = cntV[1].GetInt32();
					Continent.all[contId].cities = cntV[2].GetInt32();
					Continent.all[contId].castles = cntV[3].GetInt32();
					Continent.all[contId].dungeons = cntV[4].GetUInt16();
					Continent.all[contId].temples = cntV[5].GetUInt16();
					Continent.all[contId].bosses = cntV[6].GetUInt16();
					var index = Continent.GetPackedIdFromContUnpacked(contId);
					shot.continents[contId].settled = Continent.all[contId].settled;
					shot.continents[contId].castles = Continent.all[contId].castles;
				}
				shot.time = JSClient.ServerTime().UtcDateTime;
				if (ContinentsSnapshot.all.Length > 0 && shot.time - ContinentsSnapshot.all.Last().time < TimeSpan.FromHours(1.5f))
				{
					// don't update, max one snapshot per 1.5 hours
				}
				else
				{
					ContinentsSnapshot.all = ContinentsSnapshot.all.ArrayAppend(shot);
					ApplicationData.Current.LocalFolder.SaveAsync("continentHistory", ContinentsSnapshot.all);
				}

			}
			SettingsPage.pinned = SettingsPage.pinned.ArrayRemoveDuplicates();
			SpotTab.LoadFromPriorSession(SettingsPage.pinned);
			Task.Run(() => WorldStorage.SaveWorldData(raw));
			current = rv;
			state = State.completed;
		}
		public static async void LoadContinentHistory()
		{
			ContinentsSnapshot.all = await ApplicationData.Current.LocalFolder.ReadAsync("continentHistory", ContinentsSnapshot.all);

		}
		public static (string label, bool isMine, bool hasIncoming, bool hovered, Spot spot) GetLabel((int x, int y) c)
		{
			var data = GetInfo(c);
			switch (data.type)
			{
				case World.typeCity:
					{

						if (data.player == 0)
						{
							return (null, false, false, false, null); // lawless
						}
						else
						{
							var spot = Spot.GetOrAdd(c.WorldToCid());
							{
								var isMine = spot.isMine;
								var player = Player.all.GetValueOrDefault(data.player, Player._default);
								return (isMine ? spot.cityName : player.name, isMine, spot.incoming.Any(), !isMine && (data.player == Player.viewHover), spot);
							}
						}
						
					}
				default:
					return (null, false, false, false, null);
			}
		}
		public static async void DumpCities(int x0, int y0, int x1, int y1, string allianceName, bool onlyCastles, bool onlyOnWater)
		{
			using var scope = new ShellPage.WorkScope("Export Castles...");
			var sb = new StringBuilder();
			allianceName = allianceName.ToLower();
			sb.Append("Coords\tplayer\tclassification\tWater\tBig\tCastle\tTemple\nalliance\n");
			int counter = 0;
			for (int x = x0; x < x1; ++x)
				for (int y = y0; y < y1; ++y)
				{
					var dat = GetInfo((x, y));
					if (dat.type != World.typeCity)
						continue;
					if (onlyCastles && !dat.isCastle)
						continue;
					if (onlyOnWater && !dat.isWater)
						continue;
					if (dat.player == 0) // lawless
						continue;
					var player = Player.Get(dat.player);
					if (player.allianceName.ToLower().Contains(allianceName))
						continue;
					var cid = (x, y).WorldToCid();
					var spot = Spot.GetOrAdd(cid);
					await spot.ClassifyIfNeeded();
					sb.Append($"{spot.cid.CidToCoords()}\t{player.name}\t{spot.classificationString}\t{dat.isWater}\t{dat.isBig}\t{dat.isCastle}\t{dat.isTemple}\t{spot.alliance}\n");
					++counter;
					if ((counter & 63) == 0)
						Note.Show($"Progress: {counter}");
				}
			var str = sb.ToString();
			App.CopyTextToClipboard(str);
			Note.Show($"Complete: {counter}");



		}
		public static int[] continentOpeningOrder = { 22, 23, 32, 33, 12, 43, 13, 42, 21, 34, 24, 31, 11, 44, 14, 41, 02, 53, 20, 35, 25, 30, 52, 03, 01, 54, 04, 51, 40, 15, 45, 10, 05, 50, 00, 55 };
	}
}
