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
using COTG.Services;

namespace COTG.Game
{
    // This is used for drawing only, we don't keep track of repeats, troops etc.
    public struct Raid : IEquatable<Raid>
    {
        public int target;// cid
        public DateTimeOffset arrival;
        public bool isReturning;
        public bool isRepeating; // neighter timed return nor raid once

        public override bool Equals(object obj)
        {
            return obj is Raid raid && Equals(raid);
        }

        public bool Equals(Raid other)
        {
            return target == other.target &&
                   arrival == other.arrival &&
                   isReturning == other.isReturning &&
                   isRepeating == other.isRepeating;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(target, arrival, isReturning, isRepeating);
        }

        public static bool operator ==(Raid left, Raid right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Raid left, Raid right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{{target:{target},arrival:{arrival},isRepeating:{isRepeating},isReturning:{isReturning}}}";
        }
    }

    public static class Raiding
    {
        public static float desiredCarry = 1.03f;
        public static bool raidOnce;
        public static (int reps,float averageCarry) ComputeIdealReps(Dungeon d, City city)
        {
            var loot = d.loot;
            var carry = city.carryCapacity; // this should be on demand
            if (carry <= 0)
                return (0, 0);
         //   Log($"{desiredCarry} {carry / (loot * desiredCarry)}");
            int ideal = (int)( carry/(loot * desiredCarry) + 0.5f);
            ideal = Math.Min(ideal, city.freeCommandSlots ).Max(1);
            return (ideal, carry /(ideal*loot) );
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
            var city = d.city;
            if (city == null)
                return;
            var r = ComputeIdealReps(d,city);
            if (r.reps <= 0)
                return;
            var tr = new List<sndRaidtr>();
            foreach (var ttc in city.troopsHome)
            {
                if (!ttLandRaiders.Contains((byte)ttc.type))
                    continue;
                tr.Add(new sndRaidtr() { tt = ttc.type.ToString(), tv = (ttc.count/r.reps).ToString() });

            }
            var trs = JsonSerializer.Serialize(tr);
            var args = new sndRaidArgs() { rcid = d.cid, type = raidOnce?1:2, co = r.reps, rt = "1", snd = 1, rut = 0, ts = "", tr = trs };
            var snd = new COTG.Services.sndRaid(JsonSerializer.Serialize(args), city.cid);
            Note.Show($"{city.cid.CidToStringMD()} raid {d.cid.CidToStringMD()}");
            await snd.Post();
 //           await Task.Delay(500);
//            UpdateTS(true);
            city.tsHome = 0;

             city.NotifyChange(nameof(city.tsHome));
       
        }
        public static DateTimeOffset nextAllowedTsHomeUpdate;
        public static DateTimeOffset nextAllowedTsUpdate;
        public static async void UpdateTSHome(bool force = false)
        {
            var n = DateTimeOffset.UtcNow;
            if (n > nextAllowedTsHomeUpdate || force)
            {
                var changed = new HashSet<City>();
                nextAllowedTsHomeUpdate = n + TimeSpan.FromSeconds(24);
                var jso = await Post.SendForJson("includes/gIDl.php", "");
                foreach (var ci in jso.RootElement.EnumerateArray())
                {
                    var cid = ci.GetAsInt("i");
                    var ts = ci.GetAsInt("ts");
                    var v = City.all[cid];
                    if ((v.tsHome - ts).Abs() > 8)
                    {
                        v.tsHome = ts;
                        changed.Add(v);
                    }
                }
                
            }
        }
        // should this be waitable?
        public static void UpdateTS(bool force = false)
        {
            var n = DateTimeOffset.UtcNow;
            if (n > nextAllowedTsUpdate || force)
            {
                nextAllowedTsUpdate = n + TimeSpan.FromSeconds(24);
                nextAllowedTsHomeUpdate = nextAllowedTsUpdate; // stall this one too
                RestAPI.troopsOverview.Post();
            }
        }
        public static async Task UpdateTSSync(bool force = false)
        {
            var n = DateTimeOffset.UtcNow;
            if (n > nextAllowedTsUpdate || force)
            {
                nextAllowedTsUpdate = n + TimeSpan.FromSeconds(24);
                nextAllowedTsHomeUpdate = nextAllowedTsUpdate; // stall this one too
                await RestAPI.troopsOverview.Post();
            }
        }

    }
}
