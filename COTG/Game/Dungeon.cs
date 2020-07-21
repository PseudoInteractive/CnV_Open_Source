using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COTG.Helpers;
using static COTG.Debug;
using static COTG.Game.Enum;
using System.Collections.Generic;

namespace COTG.Game
{
    public class Dungeon 
    {
        public City city; // city to raid this, where distance is calculated from 
        public const int typeMountain = 2;
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

        public float loot => (type == typeMountain ? mountainLoot[level - 1] : otherLoot[level - 1]) * (2 - completion * 0.01f);
        public string plan { get
            {
                var r = Raiding.ComputeIdealReps(this,city);
                return $"{r.reps}x {r.averageCarry:P1} carry";
            }
        }

       
    }
}
