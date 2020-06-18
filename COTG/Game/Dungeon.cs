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
        public const int typeMountain = 2;
        public int cid;
        public string xy => $"{cid / 65536}:{cid % 65536}";
        //          [JsonProperty("t")]
        public int type { get; set; }

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
                var r = Raiding.ComputeIdealReps(this,City.current);
                return $"{r.reps} reps, carry={(int)r.averageCarry}%";
            }
        }

        public override string ToString()
        {
            return $"{{{nameof(xy)}={xy}, {nameof(type)}={type.ToString()}, {nameof(level)}={level.ToString()}, {nameof(x)}={x.ToString()}, {nameof(y)}={y.ToString()}, {nameof(completion)}={completion.ToString()}, {nameof(dist)}={dist.ToString()}, {nameof(loot)}={loot.ToString()}, {nameof(plan)}={plan}}}";
        }
    }
}
