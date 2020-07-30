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
using COTG.Views;

namespace COTG.Game
{
    // This is used for drawing only, we don't keep track of repeats, troops etc.
    public struct Raid : IEquatable<Raid>
    {
        public int target;// cid
        public DateTimeOffset time;
        public bool isReturning;
        public bool isRepeating; // neighter timed return nor raid once

        public override bool Equals(object obj)
        {
            return obj is Raid raid && Equals(raid);
        }

        public bool Equals(Raid other)
        {
            return target == other.target &&
                   time == other.time &&
                   isReturning == other.isReturning &&
                   isRepeating == other.isRepeating;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(target, time, isReturning, isRepeating);
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
            return $"{{target:{target},arrival:{time},isRepeating:{isRepeating},isReturning:{isReturning}}}";
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
        public struct sndRaidtr
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
        public static async Task SendRaids(Dungeon d)
        {
            var city = d.city;
            if (city == null)
                return;
            var r = ComputeIdealReps(d,city);
            if (r.reps <= 0 || r.averageCarry < 0.5f)
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
            MainPage.ClearDungeonList();


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
                    var v = City.allCities[cid];
                    if ((v.tsHome - ts).Abs() > 8)
                    {
                        v.tsHome = ts;
                        changed.Add(v);
                    }
                }
                changed.NotifyChange();
                
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
        public static async void ReturnSlow(int cid, bool updateUI )
        {
            Note.Show($"{cid.CidToStringMD()} Home Whenever");
            var json = "{\"a\":" + cid + ",\"c\":0,\"b\":1}";
            if (cid != 0)
            {
                await Post.SendEncrypted("includes/UrOA.php", json, "Rx3x5DdAxxerx3");
                if (updateUI)
                {
                    // await JSClient.PollCity(cid);
                    JSClient.ChangeCity(cid);
//                    await Task.Delay(300); // this might not be useful.
 //                   ScanDungeons.Post(cid, true);
                }
            }
        }
        public static async void ReturnFast(int cid, bool updateUI)
        {
            Note.Show($"{cid.CidToStringMD()} Home Please");
            if (cid != 0)
            {
                await Post.Send("overview/rcallall.php", "a=" + cid);
                if (updateUI)
                {
                    JSClient.ChangeCity(cid);
                    // await JSClient.PollCity(cid);
                    //  await Task.Delay(300); // this might not be useful.
                    //  ScanDungeons.Post(cid, true);
                }
            }
        }
        public static async void ReturnSlowBatch(IEnumerable<int>  cids)
        {
            int counter = 0;
            foreach (var cid in cids)
            {

                if (cid != 0)
                {
                    var json = "{\"a\":" + cid + ",\"c\":0,\"b\":1}";
                    await Post.SendEncrypted("includes/UrOA.php", json, "Rx3x5DdAxxerx3");
                    ++counter;
                }
            }
            Note.Show($"Issued Home Whenever on {counter} cities");
            ShellPage.ShowTipRefresh();
            UpdateTSHome();
        }
        public static async void ReturnFastBatch(IEnumerable<int> cids)
        {
            int counter = 0;
            foreach (var cid in cids)
            {
                if (cid != 0)
                {
                    ++counter;
                    await Post.Send("overview/rcallall.php", "a=" + cid);
                }
            }
            Note.Show($"Issued Home Please on {counter} cities");
            ShellPage.ShowTipRefresh();
            await Task.Delay(500);
            UpdateTSHome();
        }
    }
}
