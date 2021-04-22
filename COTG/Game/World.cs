using COTG.Helpers;
using COTG.Services;
using COTG.Views;

using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.Xna.Framework;

using System;
using System.Buffers;
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
		public static int WorldToCid(this (int x, int y) a)
		{
			return a.x + (a.y << 16);
		}

		public static int Translate(this int cid, (int dx, int dy) d)
		{
			return cid + d.dx + d.dy * 65536;
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
		//public static void SetColor(this byte[] pixels,int index, uint r, uint g, uint b)
		//{
		//	SetColor(pixels, index, RGB16(r, g, b), 2);
			
		//}

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
		public const int idAll = count - 1;

		public static int GetPackedIdFromC((int x, int y) c) => GetPackedIdFromCont((c.x / 100, c.y / 100));
		public static int GetPackedIdFromCont((int x, int y) c) => c.x.Clamp(0, 5) + c.y.Clamp(0, 5) * spanX;
		public static (int x, int y) GetContIdFromPacked(int id) {
			if (id == idAll)
				return (5,6); // special aliase for "all"

			var y = id / spanX;
			var x = id- y * spanX;
			return (x, y);
		}
		public static int GetPackedIdFromCont(int cont)
		{
			var y = cont / 10;
			return GetPackedIdFromCont((cont - y*10,y));
		}
		public bool isOpen => cities > 0;
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
		public const int span = 600;
		public const int spanSquared = span* span;
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

		public static uint[] raw = new uint[span * span]; // reference with CID, stores playerID
		public static uint[] rawPrior0; // set if heatmaps are enabled
		public static uint[] rawPrior1; // set if heatmaps are enabled
		public static int ClampCoord(int x)
		{
			return x.Clamp(0, span);
		}
		public static (uint type, int player, bool isCastle, bool isBig, bool isWater, bool isTemple, uint data) GetInfoFromPackedId(int packedId)
		{
			return GetInfoFromPackedId(raw, packedId);
		}
		public static (uint type, int player, bool isCastle, bool isBig, bool isWater, bool isTemple, uint data) GetInfo((int x, int y) c)
		{
			return GetInfoFromPackedId(raw, packedId: GetPackedId(c));
		}
		public static (uint type, int player, bool isCastle, bool isBig, bool isWater, bool isTemple, uint data) GetInfoFromCid(int cid)
		{
			return GetInfo(cid.CidToWorld());
		}
		public static (uint type, int player, bool isCastle, bool isBig, bool isWater, bool isTemple, uint data) GetInfoPrior(int packedId)
		{
			return GetInfoFromPackedId(rawPrior0, packedId);
		}
		public static (uint type, int player, bool isCastle, bool isBig, bool isWater, bool isTemple, uint data) GetInfoFromPackedId(uint[] _raw, int packedId)
		{
			uint rv = _raw[packedId];
			return (rv & typeMask, (int)(rv & playerMask),
				(rv & typeCityFlagCastle) != 0,
				(rv & typeCityFlagBig) != 0,
				(rv & typeCityFlagWater) != 0,
				(rv & typeCityFlagTemple) != 0,
				rv);

		}
		public static MemoryOwner<uint> RentWorldBuffer()
		{
			return MemoryOwner<uint>.Allocate(World.spanSquared);
		}
		public static void ReturnWorldBuffer(MemoryOwner<uint> data)
		{
			data.Dispose();
		}

		public static MemoryOwner<uint> SwizzleForCompression(Span<uint> src)
		{
			var buffer = RentWorldBuffer();
			var rv = buffer.Span;
			for(int i =0;i<World.spanSquared;++i)
			{
				uint dat = src[i];
				switch(dat & typeMask )
				{
					case typeCity:
					case typeShrine:
					case typePortal:
						rv[i] = dat.RotateLeft12();
						break;
					default:
						rv[i] = 0;
						break;
				}
			}
			return buffer;
		}
		
		public static int CidToPlayerOrMe(int _cid) 
		{
			var rv = World.GetInfo(_cid.CidToWorld()).player;
			if (rv == 0)
				rv = Player.myId;
			return rv;
		}
		public static int GetPackedId((int x, int y) c)
		{
			var x = c.x;
			var y = c.y;
			return (x >= 0 & x < span & y >= 0 & y < span) ? x + y * span : 0;
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
			uint rv = (x >= 0 & x < span & y >= 0 & y < span) ? raw[x + y * span] : 0u;
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


		public static void CreateChangePixels(uint[] prior, uint[] prior1)
		{
			var pixels = new byte[outSize / 4 * outSize / 4 * 8];
			//for (int i = 0; i < outSize / 4 * outSize / 4; i++)
			//{
			//	pixels[i * 8 + 0] = WorldHelper.RGB16B0(0x60, 0, 0);
			//	pixels[i * 8 + 1] = WorldHelper.RGB16B1(0x60, 0, 0);
			//	pixels[i * 8 + 2] = 0;
			//	pixels[i * 8 + 3] = 0;
			//	pixels[i * 8 + 4] = 0x55;
			//	pixels[i * 8 + 5] = 0x55;
			//	pixels[i * 8 + 6] = 0x55;
			//	pixels[i * 8 + 7] = 0x55;
			//}
			for (int y = 0; y < span; ++y)
			{
				for (int x = 0; x < span; ++x)
				{
					var i = y * span + x;
					var d0 = prior[i];
					var d1 = prior1[i];
					//if (d0 == 0)
					//{
					//	// no change
					//}
					//else
					{
						var dtype0 = d0 & typeMask;
						var dtype1 = d1 & typeMask;
						if (d0 == d1 || dtype0 == typeDungeon || dtype0 == typeBoss || dtype1 == typeDungeon || dtype1 == typeBoss)
						{
							pixels[i * 8 + 0] = WorldHelper.RGB16B0(0x60, 0, 0);
							pixels[i * 8 + 1] = WorldHelper.RGB16B1(0x60, 0, 0);
							pixels[i * 8 + 2] = 0;
							pixels[i * 8 + 3] = 0;
							pixels[i * 8 + 4] = 0x55;
							pixels[i * 8 + 5] = 0x55;
							pixels[i * 8 + 6] = 0x55;
							pixels[i * 8 + 7] = 0x55;
							continue;
						}
						uint color = WorldHelper.RGB16(0x80, 0x80, 0x80);
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
								// castle change or size change or handover  Todo:  differentiate this two
								color = WorldHelper.RGB16(0x0, 0x0, isCastle0 != isCastle1 ? 0xC0u : 0x90u);
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
							else
							{
								color = WorldHelper.RGB16(0x90, 0x90, 0);  // no change
							}
							// Todo: handle more cases
						}

						pixels.SetColor(i, color, 0);
						// change:  Todo, analysis
						pixels[i * 8 + 4] = 2 | (3 << 2) | (3 << 4) | (2 << 6);
						pixels[i * 8 + 5] = 3 | (0 << 2) | (0 << 4) | (3 << 6); // color index 0
						pixels[i * 8 + 6] = 3 | (0 << 2) | (0 << 4) | (3 << 6); // color index 0
						pixels[i * 8 + 7] = 2 | (3 << 2) | (3 << 4) | (2 << 6);
					}
				}
			}
			changePixels = pixels;
			rawPrior0 = prior;
			rawPrior1 = prior1;
			DrawPixels(prior1);
		}
		public static void ClearHeatmap()
		{
			AGame.ClearHeatmapImage();
			rawPrior0 = null;
			rawPrior1 = null;
			DrawPixels(raw);
		}
		static bool isDrawingHeatMap => rawPrior0 != null;

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


		public static void DrawPixels( uint [] data)
		{
			var pixels = new byte[outSize / 4 * outSize / 4 * 8];
			for (int y = 0; y < span; ++y)
			{
				for (int x = 0; x < span; ++x)
				{
					var i = y * span + x;
					var d0 = data[i];

					var type0 = d0 & typeMask;
					var isVisible = true;
					switch (type0)
					{
						case typeCity:
							{
								var isBig = (d0 & typeCityFlagBig) != 0;
								var isCastle = (d0 & typeCityFlagCastle) != 0;
								var isTemple = (d0 & typeCityFlagTemple) != 0;
								var isWater = (d0 & typeCityFlagWater) != 0;
								var pid = d0 & playerMask;
								var hasPlayer = Player.all.TryGetValue((int)pid, out var p);
								var alliance = hasPlayer ? p.alliance : 0;
								if (isTemple)
									pixels[i * 8 + 0] = 31;  // temple.  Neutral color is blue
								

								if (!isTemple && !citiesWithoutTemples)
									isVisible = false;
								if (!isCastle && !citiesWithoutCastles)
									isVisible = false;
								if (!isWater && !citiesWithoutWater)
									isVisible = false;
								if (pid == 0)
								{
									// lawless
									pixels.SetColor(i, lawless.color.R, lawless.color.G, lawless.color.B);
									if (!lawless.isOn)
										isVisible = false;
								}
								else if (pid == Player.myId)
								{
									pixels.SetColor(i, ownCities.color.R, ownCities.color.G, ownCities.color.B);
									if (!ownCities.isOn)
										isVisible = false;

								}
								else if (alliance != 0 && alliance == Alliance.my.id)
								{
									if (!ownAlliance.isOn)
										isVisible = false;

									pixels.SetColor(i, ownAlliance.color.R, ownAlliance.color.G, ownAlliance.color.B);
								}
								else
								{
									switch (Alliance.GetDiplomacy((int)alliance))
									{
										default:
											if (!otherPlayers.isOn)
												isVisible = false;
											pixels.SetColor(i, otherPlayers.color.R, otherPlayers.color.G, otherPlayers.color.B);
											break;
										case Diplomacy.allied:
											if (!alliedAlliance.isOn)
												isVisible = false;
											pixels.SetColor(i, alliedAlliance.color.R, alliedAlliance.color.G, alliedAlliance.color.B);
											break;
										case Diplomacy.nap:
											if (!napAlliance.isOn)
												isVisible = false;

											pixels.SetColor(i, napAlliance.color.R, napAlliance.color.G, napAlliance.color.B);
											break;
										case Diplomacy.enemy:
											if (!enemyAlliance.isOn)
												isVisible = false;

											pixels.SetColor(i, enemyAlliance.color.R, enemyAlliance.color.G, enemyAlliance.color.B);
											break;


									}
								}

								if (allianceSettings.TryGetValue((int)alliance, out var allianceSetting) && allianceSetting.isOn)
								{
									pixels.SetColor(i, allianceSetting.color.R, allianceSetting.color.G, allianceSetting.color.B);
								}
								if (playerSettings.TryGetValue((int)pid, out var playerSetting) && playerSetting.isOn)
								{
									pixels.SetColor(i, playerSetting.color.R, playerSetting.color.G, playerSetting.color.B);
								}

								if (isVisible)
								{


									if (isCastle) // 3,4 is on/off water
									{

										if (!isBig)
										{
											pixels[i * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
											pixels[i * 8 + 5] = (byte)(1 | ((isTemple ? 0 : 3) << 2) | (1 << 4) | (3 << 6)); // color index 0
											pixels[i * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
											pixels[i * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (2 << 6);
										}
										else
										{
											pixels[i * 8 + 4] = (byte)(1 | ((isTemple ? 0 : 3) << 2) | (1 << 4) | (3 << 6));
											pixels[i * 8 + 5] = 1 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
											pixels[i * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
											pixels[i * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (2 << 6);
										}
									}
									else
									{
										if (!isBig)
										{
											// City
											pixels[i * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
											pixels[i * 8 + 5] = 3 | (3 << 2) | (3 << 4) | (3 << 6); // color index 0
											pixels[i * 8 + 6] = 3 | (1 << 2) | (1 << 4) | (3 << 6); // color index 0
											pixels[i * 8 + 7] = 3 | (3 << 2) | (2 << 4) | (2 << 6);
										}
										else // if (type == 5 || type == 6)
										{
											// City
											pixels[i * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
											pixels[i * 8 + 5] = 3 | (1 << 2) | (1 << 4) | (3 << 6); // color index 0
											pixels[i * 8 + 6] = 3 | (1 << 2) | (1 << 4) | (2 << 6); // color index 0
											pixels[i * 8 + 7] = 3 | (3 << 2) | (2 << 4) | (2 << 6);
										}
									}

								}
								break;
							}
						case typeBoss:
							{
								var level = d0 & 15;
								if (bosses.isOn && level >= bossMinLevel && level <= bossMaxLevel)
								{
									pixels.SetColor(i, bosses.color.R, bosses.color.G, bosses.color.B);
									pixels[i * 8 + 4] = 1 | (3 << 2) | (1 << 4) | (3 << 6);
									pixels[i * 8 + 5] = 3 | (2 << 2) | (3 << 4) | (2 << 6); // color index 0
									pixels[i * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (3 << 6); // color index 0
									pixels[i * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (2 << 6);
									//     Trace($"Boss: {b}");
								}
								else
								{
									isVisible = false;
								}
								break;
							}
						case typeDungeon:
							{

								var level = d0 & 15;
								if (caverns.isOn && level >= cavernMinLevel && level <= cavernMaxLevel)
								{
									pixels.SetColor(i, 0x90, (byte)(0xD0 - level * 8), (byte)(0x40 + level * 7));
									float t = (level - 1) / 9.0f;
									switch (level)
									{
										case 1:
										case 2:
											{
												pixels[i * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
												pixels[i * 8 + 5] = 3 | (1 << 2) | (3 << 4) | (3 << 6); // color i 0
												pixels[i * 8 + 6] = 3 | (3 << 2) | (2 << 4) | (3 << 6); // color i 0
												pixels[i * 8 + 7] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
											}
											break;
										case 3:
										case 4:
											{
												pixels[i * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
												pixels[i * 8 + 5] = 3 | (3 << 2) | (1 << 4) | (3 << 6); // color i 0
												pixels[i * 8 + 6] = 3 | (1 << 2) | (3 << 4) | (2 << 6); // color i 0
												pixels[i * 8 + 7] = 3 | (3 << 2) | (2 << 4) | (3 << 6);
											}
											break;
										case 5:
											{
												pixels[i * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
												pixels[i * 8 + 5] = 1 | (3 << 2) | (1 << 4) | (3 << 6); // color i 0
												pixels[i * 8 + 6] = 3 | (1 << 2) | (3 << 4) | (2 << 6); // color i 0
												pixels[i * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (3 << 6);
											}
											break;
										case 6:
											{
												pixels[i * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
												pixels[i * 8 + 5] = 3 | (1 << 2) | (3 << 4) | (3 << 6); // color i 0
												pixels[i * 8 + 6] = 1 | (3 << 2) | (1 << 4) | (3 << 6); // color i 0
												pixels[i * 8 + 7] = 3 | (2 << 2) | (3 << 4) | (2 << 6);
											}
											break;
										case 7:
											{
												pixels[i * 8 + 4] = 3 | (3 << 2) | (1 << 4) | (3 << 6);
												pixels[i * 8 + 5] = 1 | (3 << 2) | (3 << 4) | (2 << 6); // color i 0
												pixels[i * 8 + 6] = 3 | (1 << 2) | (3 << 4) | (3 << 6); // color i 0
												pixels[i * 8 + 7] = 3 | (3 << 2) | (2 << 4) | (3 << 6);
											}
											break;
										case 8:
											{
												pixels[i * 8 + 4] = 3 | (1 << 2) | (3 << 4) | (3 << 6);
												pixels[i * 8 + 5] = 1 | (3 << 2) | (2 << 4) | (3 << 6); // color i 0
												pixels[i * 8 + 6] = 3 | (2 << 2) | (1 << 4) | (3 << 6); // color i 0
												pixels[i * 8 + 7] = 3 | (3 << 2) | (3 << 4) | (2 << 6);
											}
											break;
										case 9:
											{
												pixels[i * 8 + 4] = 3 | (3 << 2) | (1 << 4) | (3 << 6);
												pixels[i * 8 + 5] = 3 | (1 << 2) | (3 << 4) | (2 << 6); // color i 0
												pixels[i * 8 + 6] = 1 | (3 << 2) | (1 << 4) | (3 << 6); // color i 0
												pixels[i * 8 + 7] = 3 | (2 << 2) | (3 << 4) | (2 << 6);
											}
											break;

										default:
											{
												pixels[i * 8 + 4] = 1 | (1 << 2) | (3 << 4) | (3 << 6);
												pixels[i * 8 + 5] = 3 | (2 << 2) | (1 << 4) | (3 << 6); // color i 0
												pixels[i * 8 + 6] = 1 | (3 << 2) | (1 << 4) | (2 << 6); // color i 0
												pixels[i * 8 + 7] = 3 | (2 << 2) | (3 << 4) | (2 << 6);
											}
											break;
									}
								}
								else
								{
									isVisible = false;
								}

								break;
							}

						case typeShrine:
							{
								isVisible = shrines.isOn;
								if (shrines.isOn)
								{
									var type = d0 & 255;
									if (type == 255)
									{
										pixels.SetColor(i, shrines.color.R, shrines.color.G, shrines.color.B);
									}
									else
									{
										pixels.SetColor(i, WorldHelper.FaithColor16((int)type), 2);

									}
									//ownerPixels[i * 8 + 4] = 0 | (0 << 2) | (1 << 4) | (1 << 6);
									//ownerPixels[i * 8 + 5] = 0 | (3 << 2) | (1 << 4) | (1 << 6); // color i 0
									//ownerPixels[i * 8 + 6] = 0 | (2 << 2) | (3 << 4) | (0 << 6); // color i 0
									//ownerPixels[i * 8 + 7] = 0 | (0 << 2) | (0 << 4) | (0 << 6);

									pixels[i * 8 + 4] = 1 | (3 << 2) | (1 << 4) | (3 << 6);
									pixels[i * 8 + 5] = 1 | (2 << 2) | (1 << 4) | (2 << 6); // color i 0
									pixels[i * 8 + 6] = 1 | (2 << 2) | (1 << 4) | (2 << 6); // color i 0
									pixels[i * 8 + 7] = 3 | (2 << 2) | (3 << 4) | (2 << 6);
								}
								break;
							}
						case typePortal:
							{
								
								var active = (d0 & 1) != 0;
								if (active)
								{
									isVisible = activePortals.isOn;
									pixels.SetColor(i, activePortals.color.R, activePortals.color.G, activePortals.color.B);
									//				ownerPixels.SetColor(i, 0xaA, 0xFA, 0xFF);
									pixels[i * 8 + 0] = 31;
								}
								else
								{
									isVisible = inactivePortals.isOn;
									pixels.SetColor(i, inactivePortals.color.R, inactivePortals.color.G, inactivePortals.color.B);
									//				ownerPixels.SetColor(i, 0xBA, 0xBA, 0xA0);
								}
								if (isVisible)
								{
									pixels[i * 8 + 4] = 3 | (3 << 2) | (1 << 4) | (3 << 6);
									pixels[i * 8 + 5] = 3 | (1 << 2) | (1 << 4) | (2 << 6); // color i 0
									pixels[i * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (2 << 6); // color i 0
									pixels[i * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (2 << 6);

									//ownerPixels[i * 8 + 4] = 0 | (1 << 2) | (1 << 4) | (1 << 6);
									//ownerPixels[i * 8 + 5] = 0 | (1 << 2) | (2 << 4) | (1 << 6); // color i 0
									//ownerPixels[i * 8 + 6] = 0 | (1 << 2) | (1 << 4) | (1 << 6); // color i 0
									//ownerPixels[i * 8 + 7] = 0 | (0 << 2) | (0 << 4) | (0 << 6);
								}
								break;
							}
						default:
							{
								isVisible = false;
								break;
							}

					}
					if(!isVisible)
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

				}
					
					

				
			}
			bitmapPixels = pixels;
		}
		
		public static async void Decode(JsonDocument jsd)
		{
			Assert(state == State.started);
		

		

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

				var index = (x + y * span);
				raw[index] = b.level | (uint)(b.type << 4) | typeBoss;

				
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
					var index = (int)(x + y * span);
					
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
				var index = (int)(b.x + b.y * span);
				
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

				var index = (int)(b.x + b.y * span);
				
				raw[index] = (b.active ? 1u : 0) | typePortal;
			}
			while (Player.all.Count == 0)
			{
			//	Assert(false);
				await Task.Delay(500);
			}

			
			foreach (var p in Player.all)
			{
				p.Value.cities.Clear();
			}

			var mustAdd = new List<Player>();
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
						var p = Player.Get((int)pid);
						if(p==null)
						{
							p = new Player();
							mustAdd.Add(p);
							p.name = $"*New Player {pid}";
						}
						
						p.alliance = (ushort)alliance;
						p.cities.Add(((int)x, (int)y).WorldToCid());		
					}

					//if (pid == Player.myId)
					//{
					//    LogJS(c);
					//    Log(_t);
					//}
					// cities.Add(c);
					var index = (int)(x + y * span);


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

			if (mustAdd.Any())
			{
				var _all = new Dictionary<int,Player>(Player.all);
				var _ids = new Dictionary<string, int>(Player.nameToId);
				foreach(var i in mustAdd)
				{
					_all.Add(i.id, i);
					_ids.Add(i.name, i.id);

				}

				mustAdd.Clear();
				Player.all = _all;
				Player.nameToId = _ids;
			}


			int counter = 0;
			// Wait for alliance diplomacy for colors
			while (!Alliance.diplomacyFetched && counter++ < 16)
			{
				await Task.Delay(1000);
			}

			
			//            rv.cities = cities.ToArray();
			rv.portals = portals.ToArray();
			rv.shrineList = shrineList.ToArray();
			Boss.all = bossList.ToArray();

			
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
			
			Task.Run( async () =>
			{
				await WorldStorage.SaveWorldData(SwizzleForCompression(raw));
			});
			current = rv;

			// delay this part
			

			///		Assert(state == State.none || state == State.completed);
			//		state = State.started;
			// reset players	
			

			while (!Alliance.alliancesFetched)
			{
				await Task.Delay(500);
			}
			if(!isDrawingHeatMap)
				DrawPixels(raw);

			state = State.completed;
			// Queue up another one
			App.QueueIdleTask(RefreshWorldDataIdleTask, 30 * 60 * 1000);  // 5 minutes - todo: change this to 30 minutes
			
			SpotTab.LoadFromPriorSession();
		}

		static void RefreshWorldDataIdleTask()
		{
			Trace("World refresh");
			if (World.completed)
				GetWorldInfo.Send();
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
	public class WorldBufferScope : IDisposable
	{
		public MemoryOwner<uint> b;
		// passing null results in a Scope with no effect
		public WorldBufferScope()
		{
			b = World.RentWorldBuffer();
		}
		public static implicit operator MemoryOwner<uint> (WorldBufferScope w) => w.b;
		public void Dispose()
		{
			var _b = b;
			b = null;
			if (_b != null )
			{
				World.ReturnWorldBuffer(_b);
			}
		}
	}

}
