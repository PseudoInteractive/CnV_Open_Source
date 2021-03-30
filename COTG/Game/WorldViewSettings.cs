using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Game
{
	class WorldViewSettings
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
		public static Dictionary<int,PlayerSetting> playerSettings = new Dictionary<int, PlayerSetting>();
		public static Dictionary<int, AllianceSetting> allianceSettings = new Dictionary<int, AllianceSetting>();
	}

}
