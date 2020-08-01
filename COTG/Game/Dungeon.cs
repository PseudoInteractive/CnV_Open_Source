using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COTG.Helpers;
using static COTG.Debug;
using static COTG.Game.Enum;
using System.Collections.Generic;
using System.Text.Json;

namespace COTG.Game
{
    public class Dungeon 
    {
        public City city; // city to raid this, where distance is calculated from 
        public int cid; // dungeon id
        public string xy => $"{cid % 65536}:{cid / 65536}";
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
                rv += 4; // penalty of 4 spaces for wrong type
            return rv;
        }
        public static void ShowDungeonList(City city, JsonElement jse)
        {
            var rv = new List<Dungeon>();
            foreach (var dung in jse.EnumerateArray())
            {
                rv.Add(new Dungeon()
                {
                    city = city,
                    cid = dung.GetAsInt("c"),
                    type = dung.GetAsByte("t"),
                    level = dung.GetAsByte("l"),
                    completion = dung.GetAsFloat("p"),
                    dist = dung.GetAsFloat("d")

                });
            }
            var idealType = city.GetIdealDungeonType();
            rv.Sort((a, b) => a.GetScore(idealType).CompareTo(b.GetScore(idealType) ));
            // dont wait on this 
            COTG.Views.MainPage.UpdateDungeonList(rv);
        }

    }
}
