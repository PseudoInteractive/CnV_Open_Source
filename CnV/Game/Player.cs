﻿
// We use the ids from Playfab
global using PlayerId = System.Int32;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using static CnV.Debug;

namespace CnV.Game
{
	using Microsoft.UI.Xaml.Media.Imaging;

	public class Player
    {
		public int id;
        public string name;
        public string discordUserName;
        public ushort alliance;
        public byte title;
        public static int myId;
		public static int activeId;
		public int points;
		public string avatarUrl;
		public ulong discordId;
		public BitmapImage avatarBrush;

		public static HashSet<int> myIds = new HashSet<int>();
		public static string myName;
		public static string subOwner;
		public static int subOwnerId => NameToId(subOwner);

		public float MoralePenalty(Player b)
		{
			if (points <= b.points * 5)
				return 0;
			return (float)(points*10) / b.points;
		}
		public static float MoralePenalty(int attackPid, int defendPid)
		{
			return Player.Get(attackPid).MoralePenalty(Player.Get(defendPid));
		}

		public static Player me => all[myIds.First()];
		public static int myTitle;
        public static int viewHover; // in the view menu
		public static int moveSlots=100;
		public HashSet<int> cities = new();
		public static bool isSpecial => subOwnerId == 2375 ;


		public string allianceName => Alliance.IdToName(alliance);

//		public static object activePlayerName => IdToName(activeId);

		public static Dictionary<string, Player> playerByDiscordUserName = new(); // case sensitive
		public static Dictionary<ulong, Player> playerByDiscordId = new();
		public static Dictionary<int, Player> all = new Dictionary<int, Player>();
        public static Dictionary<string, int> nameToId = new Dictionary<string, int>();
        public static string IdToName(int id)
        {
            return Get(id).name;
         }
        public static int NameToId(string name)
        {
			
            return name!=null ? nameToId.GetValueOrDefault(name) : -1;
        }
        public static bool IsMe(int playerId)
        {
            return Player.myId == playerId;
        }
		public static bool IsFriend(int playerId)
		{
			return Player.myIds.Contains(playerId);
		}
		public static Player _default = new Player() { name = "!Zut!" };
		public byte allianceRole;
		internal ulong playfabId;

		public static ulong myPlayfabId; // the current players playfabId

		internal bool isMe => id == myId;

		public static Player Get(int id) => all.GetValueOrDefault(id, _default);
		public static Player GetOrAdd(int id)
		{
			Assert(id > 0);
			if (all.TryGetValue(id, out var rv))
				return rv;
			rv = new();
			all.Add(id, rv);
			return rv;
		}
		public static void Ctor(JsonElement json)
        {
				var _nameToId = new Dictionary<string, int>(1000);
				var _all = new Dictionary<int,Player>( 1000 );
				foreach (var entry in json.EnumerateObject())
				{
					var id = int.Parse(entry.Name);
					var str = entry.Value.GetString();
					_all.Add(id, new Player() { id = id, name = str });
					_nameToId.Add(str, id);
				}
				// bonus!
				var bonus = new string[] { "lawless" };
				{
					int counter = 0;
					foreach (var i in bonus)
					{
						_all.Add(counter, new Player() { id = counter, name = i });
						_nameToId.Add(i, counter);
						--counter;

					}
				}
			
            all = _all;
            nameToId = _nameToId;


        }

		internal static Player FromName(string player)
		{
			return all[nameToId.GetValueOrDefault(player)];
		}
		internal static Player FromNameOrNull(string player)
		{
			if(player is null)
				return null;
			if(nameToId.TryGetValue(player, out var p))
				return all[p];
			return null;
		}
	}
}
