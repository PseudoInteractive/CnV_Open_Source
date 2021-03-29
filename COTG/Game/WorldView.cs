using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Game
{
	class WorldView
	{
		public static WorldView instance= new WorldView(); 
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
		public Setting ownCities;
		public Setting ownAlliance;
		public Setting alliedAlliance;
		public Setting napAlliance;
		public Setting enemyAlliance;
		public Setting otherPlayers;
		public Setting lawless;
		public Setting friends;
		public bool citiesWithoutCastles;
		public bool citiesWithoutWater;
		public bool citiesWithoutTemples;
		public Setting caverns;
		public Setting bosses;

		public int cavernMinLevel, cavernMaxLevel;
		public int bossMinLevel, bossMaxLevel;

		public Setting shrines;
		public Setting inactivePortals;
		public Setting activePortals;
		public Dictionary<int,PlayerSetting> playerSettings = new Dictionary<int, PlayerSetting>();
		public Dictionary<int, AllianceSetting> allianceSettings = new Dictionary<int, AllianceSetting>();
	}

}
