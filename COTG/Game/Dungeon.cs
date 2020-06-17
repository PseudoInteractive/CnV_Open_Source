using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COTG.Helpers;
using static COTG.Debug;
using System.Collections.Generic;

namespace COTG.Game
{
    public class Dungeon
    {
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

    }
}
