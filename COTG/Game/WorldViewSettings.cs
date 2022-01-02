using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV.Game
{
	using static World;

	public class WorldViewSettings
	{

		public struct Setting
		{
			public bool isOn;
			public Color color;

		}
		public struct PlayerSetting
		{
			public bool isOn;
			public Color color;
			public int pid;
		}
		public struct AllianceSetting
		{
			public bool isOn;
			public Color color;
			public int pid;
		}
		public static Setting ownCities;
		public static Setting ownAlliance;
		public static Setting alliedAlliance;
		public static Setting napAlliance;
		public static Setting enemyAlliance;
		public static Setting otherPlayers;
		public static Setting lawless;
		public static Setting friends;
		public static bool citiesWithoutCastles;
		public static bool citiesWithoutWater;
		public static bool citiesWithoutTemples;
		public static Setting caverns;
		public static Setting bosses;

		public static int cavernMinLevel, cavernMaxLevel;
		public static int bossMinLevel, bossMaxLevel;

		public static Setting shrines;
		public static Setting inactivePortals;
		public static Setting activePortals;
		public static Dictionary<int, PlayerSetting> playerSettings = new Dictionary<int, PlayerSetting>();
		public static Dictionary<int, AllianceSetting> allianceSettings = new Dictionary<int, AllianceSetting>();


		//public static bool IsVisible(uint d0)
		//{
		//	var type0 = d0 & typeMask;
		//	var isVisible = true;



		//	switch (type0)
		//	{
		//		case typeCity:
		//			{
		//				var isBig = (d0 & typeCityFlagBig) != 0;
		//				var isCastle = (d0 & typeCityFlagCastle) != 0;
		//				var isTemple = (d0 & typeCityFlagTemple) != 0;
		//				var isWater = (d0 & typeCityFlagWater) != 0;
		//				if (!isTemple && !citiesWithoutTemples)
		//					return false;
		//				if (!isCastle && !citiesWithoutCastles)
		//					return false;
		//				if (!isWater && !citiesWithoutWater)
		//					return false;

		//				var pid = World.GetPlayer(d0);
		//				var hasPlayer = Player.all.TryGetValue((int)pid, out var p);
		//				var alliance = hasPlayer ? p.allianceId : 0;

		//				if (pid == 0)
		//				{
		//					// lawless
		//					if (!lawless.isOn)
		//						return false;
		//				}
		//				else if (pid == Player.myId)
		//				{
		//					if (!ownCities.isOn)
		//						return false;

		//				}
		//				else if (alliance != 0 && alliance == Alliance.my.id)
		//				{
		//					if (!ownAlliance.isOn)
		//						return false;

		//				}
		//				else
		//				{
		//					switch (Alliance.GetDiplomacy((int)alliance))
		//					{
		//						default:
		//							if (!otherPlayers.isOn)
		//								return false;
		//							break;
		//						case Diplomacy.allied:
		//							if (!alliedAlliance.isOn)
		//								return false; 
		//							break;
		//						case Diplomacy.nap:
		//							if (!napAlliance.isOn)
		//								return false;
		//							break;
		//						case Diplomacy.enemy:
		//							if (!enemyAlliance.isOn)
		//								return false; 

		//							break;


		//					}
		//				}


		//				break;
		//			}
		//		case typeBoss:
		//			{
		//				var level = GetData(d0);
		//				if (bosses.isOn && level >= bossMinLevel && level <= bossMaxLevel)
		//				{
		//					pixels.SetColor(i, bosses.color.R, bosses.color.G, bosses.color.B);
		//					pixels[i * 8 + 4] = 1 | (3 << 2) | (1 << 4) | (3 << 6);
		//					pixels[i * 8 + 5] = 3 | (2 << 2) | (3 << 4) | (2 << 6); // color index 0
		//					pixels[i * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (3 << 6); // color index 0
		//					pixels[i * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (2 << 6);
		//					//     Trace($"Boss: {b}");
		//				}
		//				else
		//				{
		//					isVisible = false;
		//				}
		//				break;
		//			}
		//		case typeDungeon:
		//			{

		//				var level = GetData(d0);
		//				if (caverns.isOn && level >= cavernMinLevel && level <= cavernMaxLevel)
		//				{
		//					pixels.SetColor(i, 0x90, (byte)(0xD0 - level * 8), (byte)(0x40 + level * 7));
		//					float t = (level - 1) / 9.0f;
		//					switch (level)
		//					{
		//						case 1:
		//						case 2:
		//							{
		//								pixels[i * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
		//								pixels[i * 8 + 5] = 3 | (1 << 2) | (3 << 4) | (3 << 6); // color i 0
		//								pixels[i * 8 + 6] = 3 | (3 << 2) | (2 << 4) | (3 << 6); // color i 0
		//								pixels[i * 8 + 7] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
		//							}
		//							break;
		//						case 3:
		//						case 4:
		//							{
		//								pixels[i * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
		//								pixels[i * 8 + 5] = 3 | (3 << 2) | (1 << 4) | (3 << 6); // color i 0
		//								pixels[i * 8 + 6] = 3 | (1 << 2) | (3 << 4) | (2 << 6); // color i 0
		//								pixels[i * 8 + 7] = 3 | (3 << 2) | (2 << 4) | (3 << 6);
		//							}
		//							break;
		//						case 5:
		//							{
		//								pixels[i * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
		//								pixels[i * 8 + 5] = 1 | (3 << 2) | (1 << 4) | (3 << 6); // color i 0
		//								pixels[i * 8 + 6] = 3 | (1 << 2) | (3 << 4) | (2 << 6); // color i 0
		//								pixels[i * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (3 << 6);
		//							}
		//							break;
		//						case 6:
		//							{
		//								pixels[i * 8 + 4] = 3 | (3 << 2) | (3 << 4) | (3 << 6);
		//								pixels[i * 8 + 5] = 3 | (1 << 2) | (3 << 4) | (3 << 6); // color i 0
		//								pixels[i * 8 + 6] = 1 | (3 << 2) | (1 << 4) | (3 << 6); // color i 0
		//								pixels[i * 8 + 7] = 3 | (2 << 2) | (3 << 4) | (2 << 6);
		//							}
		//							break;
		//						case 7:
		//							{
		//								pixels[i * 8 + 4] = 3 | (3 << 2) | (1 << 4) | (3 << 6);
		//								pixels[i * 8 + 5] = 1 | (3 << 2) | (3 << 4) | (2 << 6); // color i 0
		//								pixels[i * 8 + 6] = 3 | (1 << 2) | (3 << 4) | (3 << 6); // color i 0
		//								pixels[i * 8 + 7] = 3 | (3 << 2) | (2 << 4) | (3 << 6);
		//							}
		//							break;
		//						case 8:
		//							{
		//								pixels[i * 8 + 4] = 3 | (1 << 2) | (3 << 4) | (3 << 6);
		//								pixels[i * 8 + 5] = 1 | (3 << 2) | (2 << 4) | (3 << 6); // color i 0
		//								pixels[i * 8 + 6] = 3 | (2 << 2) | (1 << 4) | (3 << 6); // color i 0
		//								pixels[i * 8 + 7] = 3 | (3 << 2) | (3 << 4) | (2 << 6);
		//							}
		//							break;
		//						case 9:
		//							{
		//								pixels[i * 8 + 4] = 3 | (3 << 2) | (1 << 4) | (3 << 6);
		//								pixels[i * 8 + 5] = 3 | (1 << 2) | (3 << 4) | (2 << 6); // color i 0
		//								pixels[i * 8 + 6] = 1 | (3 << 2) | (1 << 4) | (3 << 6); // color i 0
		//								pixels[i * 8 + 7] = 3 | (2 << 2) | (3 << 4) | (2 << 6);
		//							}
		//							break;

		//						default:
		//							{
		//								pixels[i * 8 + 4] = 1 | (1 << 2) | (3 << 4) | (3 << 6);
		//								pixels[i * 8 + 5] = 3 | (2 << 2) | (1 << 4) | (3 << 6); // color i 0
		//								pixels[i * 8 + 6] = 1 | (3 << 2) | (1 << 4) | (2 << 6); // color i 0
		//								pixels[i * 8 + 7] = 3 | (2 << 2) | (3 << 4) | (2 << 6);
		//							}
		//							break;
		//					}
		//				}
		//				else
		//				{
		//					isVisible = false;
		//				}

		//				break;
		//			}

		//		case typeShrine:
		//			{
		//				isVisible = shrines.isOn;
		//				if (shrines.isOn)
		//				{
		//					var type = GetData(d0);
		//					if (type == 255)
		//					{
		//						pixels.SetColor(i, shrines.color.R, shrines.color.G, shrines.color.B);
		//					}
		//					else
		//					{
		//						pixels.SetColor(i, WorldHelper.FaithColor16((int)type), 2);

		//					}
		//					//ownerPixels[i * 8 + 4] = 0 | (0 << 2) | (1 << 4) | (1 << 6);
		//					//ownerPixels[i * 8 + 5] = 0 | (3 << 2) | (1 << 4) | (1 << 6); // color i 0
		//					//ownerPixels[i * 8 + 6] = 0 | (2 << 2) | (3 << 4) | (0 << 6); // color i 0
		//					//ownerPixels[i * 8 + 7] = 0 | (0 << 2) | (0 << 4) | (0 << 6);

		//					pixels[i * 8 + 4] = 1 | (3 << 2) | (1 << 4) | (3 << 6);
		//					pixels[i * 8 + 5] = 1 | (2 << 2) | (1 << 4) | (2 << 6); // color i 0
		//					pixels[i * 8 + 6] = 1 | (2 << 2) | (1 << 4) | (2 << 6); // color i 0
		//					pixels[i * 8 + 7] = 3 | (2 << 2) | (3 << 4) | (2 << 6);
		//				}
		//				break;
		//			}
		//		case typePortal:
		//			{

		//				var active = GetData(d0) != 0;
		//				if (active)
		//				{
		//					isVisible = activePortals.isOn;
		//					pixels.SetColor(i, activePortals.color.R, activePortals.color.G, activePortals.color.B);
		//					//				ownerPixels.SetColor(i, 0xaA, 0xFA, 0xFF);
		//					pixels[i * 8 + 0] = 31;
		//				}
		//				else
		//				{
		//					isVisible = inactivePortals.isOn;
		//					pixels.SetColor(i, inactivePortals.color.R, inactivePortals.color.G, inactivePortals.color.B);
		//					//				ownerPixels.SetColor(i, 0xBA, 0xBA, 0xA0);
		//				}
		//				if (isVisible)
		//				{
		//					pixels[i * 8 + 4] = 3 | (3 << 2) | (1 << 4) | (3 << 6);
		//					pixels[i * 8 + 5] = 3 | (1 << 2) | (1 << 4) | (2 << 6); // color i 0
		//					pixels[i * 8 + 6] = 1 | (1 << 2) | (1 << 4) | (2 << 6); // color i 0
		//					pixels[i * 8 + 7] = 3 | (2 << 2) | (2 << 4) | (2 << 6);

		//					//ownerPixels[i * 8 + 4] = 0 | (1 << 2) | (1 << 4) | (1 << 6);
		//					//ownerPixels[i * 8 + 5] = 0 | (1 << 2) | (2 << 4) | (1 << 6); // color i 0
		//					//ownerPixels[i * 8 + 6] = 0 | (1 << 2) | (1 << 4) | (1 << 6); // color i 0
		//					//ownerPixels[i * 8 + 7] = 0 | (0 << 2) | (0 << 4) | (0 << 6);
		//				}
		//				break;
		//			}
		//		default:
		//			{
		//				isVisible = false;
		//				break;
		//			}

		//	}
		//	return isVisible;

		//}
	}

}
