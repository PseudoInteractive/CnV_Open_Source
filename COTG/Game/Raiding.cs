using COTG.Helpers;
using System;
using COTG;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using static COTG.Debug;
using static COTG.Game.Enum;

namespace COTG.Game
{
    public static class Raiding
    {
        public static float desiredCarry = 0.9f;
        public static bool raidOnce;
        public static (int reps,float averageCarry) ComputeIdealReps(Dungeon d, City city)
        {
            var loot = d.loot;
            var carry = city.carryCapacity; // this should be on demand
            if (carry <= 0)
                return (0, 0);
            int ideal = (int)( carry/(loot * desiredCarry) + 0.5f);
            if (ideal < 1)
                ideal = 1;
            ideal = Math.Min(ideal, city.freeCommandSlots );
            return (ideal, 100.0f * carry /(ideal*loot) );
        }


        // for json
        internal struct sndRaidtr
        {
            public string tt { get; set; }
            public string tv { get; set; }
        }
        struct sndRaidArgs
        {
            
            public int rcid { get; set; }

            public string tr { get; set; }
            public int type { get; set; }
            public int co { get; set; }
            public string rt { get; set; }
            public int snd { get; set; }
            public int rut { get; set; }
            public string ts { get; set; }
        }
        internal static async Task SendRaids(Dungeon d)
        {
            var city = City.current;
            if (city == null)
                return;
            var r = ComputeIdealReps(d,city);
            if (r.reps <= 0)
                return;
            var jst = city.troopsHome;
            if (!jst.IsValid())
                return;
            var tr = new List<sndRaidtr>();
            foreach (var troopType in ttLandRaiders)
            {
                int count = jst[troopType].GetInt32()/r.reps;
                if (count <= 0)
                    continue;
                tr.Add(new sndRaidtr() { tt = troopType.ToString(), tv = count.ToString() });

            }
            var trs = JsonSerializer.Serialize(tr);
            var args = new sndRaidArgs() { rcid = d.cid, type = raidOnce?1:2, co = r.reps, rt = "1", snd = 1, rut = 0, ts = "", tr = trs };
            var snd = new COTG.Services.sndRaid(JsonSerializer.Serialize(args), city.cid);
            Note.Show($"{city.cid.ToCoordinateMD()} raid {d.cid.ToCoordinateMD()}");
            await snd.Post();
            Task.Run(city.TroopsChanged);

        }
    }
}
