using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COTG.Helpers;
using static COTG.Debug;
using static COTG.Game.Enum;
using System.Text.Json;

namespace COTG.Game
{
    public class Dungeon 
    {
		public static DumbCollection<Dungeon> raidDungeons = new DumbCollection<Dungeon>(); // for row details, global as there is only 1 row detail open at a time

		public City city; // city to raid this, where distance is calculated from 
        public int cid; // dungeon id
        public string xy => cid.CidToString();
        //          [JsonProperty("t")]
        public int type { get; set; }
        public string kind => type switch
        {
            0 => "forest",
            1 => "hill",
            2 => "mountain",
            _ => "water"
        };

        public bool isWater => type == (int)DungeonType.water;
        

        //        [JsonProperty("l")]
        public int level { get; set; }

        //       [JsonProperty("x")]
        public int x => cid % 65536;

        //      [JsonProperty("y")]
        public int y => cid / 65536;

        //     [JsonProperty("p")]
        public float completion { get; set; }


        //    [JsonProperty("d")]
        public float dist { get; set; }

        public float loot => (type == (int)DungeonType.mountain ? mountainLoot[level - 1] : otherLoot[level - 1]) * (2 - completion * 0.01f);
        public string plan { get
            {
                var r = Raiding.ComputeIdealReps(this,city);
                return $"{r.reps}x {r.averageCarry:P1} carry";
            }
        }
        public float GetScore( byte bestDungeonType)
        {
            // lower is better
            var rv = dist;
            if (bestDungeonType != type)
                rv += 6; // penalty of 4 spaces for wrong type
            return rv;
        }
		public static async Task ShowDungeonList(City city, JsonElement jse, bool autoRaid)
		{
			var rv = autoRaid ? new DumbCollection<Dungeon>(): raidDungeons;
			rv.Clear();
			var idealType = city.GetIdealDungeonType();
			foreach (var dung in jse.EnumerateArray())
			{
				var type = dung.GetAsByte("t");
				if (Views.SettingsPage.raidOffDungeons || (type == idealType))
				{
					rv.Add(new Dungeon()
					{
						city = city,
						cid = dung.GetAsInt("c"),
						type = type,
						level = dung.GetAsByte("l"),
						completion = dung.GetAsFloat("p"),
						dist = dung.GetAsFloat("d")

					}
					);
				}
			}

			rv.Sort((a, b) => a.GetScore(idealType).CompareTo(b.GetScore(idealType)));
			if (autoRaid)
			{
				if(rv.Count>0)
				{
					await Raiding.SendRaids(rv[0],false);
				}
			}
			else
			{
				// dont wait on this 
				COTG.Views.MainPage.UpdateDungeonList(rv);
			}
        }
	}
}
