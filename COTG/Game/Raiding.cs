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
using System.Text.Encodings.Web;

namespace COTG.Game
{
    // This is used for drawing only, we don't keep track of repeats, troops etc.
    public struct Raid : IEquatable<Raid>
    {
        public int target;// cid
        public bool isReturning;
        public bool isRepeating; // neighter timed return nor raid once
        public byte repeatCount;
        public byte troopType; // todo:  We should store3 of these, or specials value for RT, VRT, VT
        public DateTimeOffset time;
		public static bool test;
        //  0 "guard",1 "ballista",2 "ranger",3 "triari", 
        //  4  "priestess",5 "vanquisher",6 "sorcerers",7 "scout", 
        //  8  "arbalist",9 "praetor",10 "horseman",11 "druid",
        //  12 "ram",13 "scorpion",14 "galley",15 "stinger",
        //  16 "warship",17 "senator"

        

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

        // restul int minutes
        public float GetOneWayTripTimeMinutes(City city)
        {
            var dist = target.DistanceToCid(city.cid);
            // based on slowest troop
            var rv = 0f;
            foreach (var tt in city.troopsTotal)
            {
                var type = tt.type;
                var travel = dist * ttTravel[type] / (ttSpeedBonus[type]);
                // if (IsWaterRaider(type))
                // 1 hour extra for all raids
                travel += 60.0f;
                if (travel > rv)
                    rv = travel;
            }
            return rv > 0 ? rv : 90; // if troops are not updated, cannot compute raid income
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
        public static float troopFraction = 1;
        public static bool FindAndIncrement(this Raid[] me, int target, DateTimeOffset dt)
        {
            int count = me.Length;
            for(int i=0;i<count;++i)
            {
                ref var raid = ref me[i];
                if (raid.target == target && raid.time == dt)
                {
                    ++raid.repeatCount;
                    return true;
                }
            }
            return false;
        }
        
        public static (int reps,float averageCarry, float fractionalReps) ComputeIdealReps(Dungeon d, City city)
        {
            var loot = d.loot;
            var carry = city.CarryCapacity(d.isWater); // this should be on demand
            if (carry <= 0)
                return (0, 0,0);
			//   Log($"{desiredCarry} {carry / (loot * desiredCarry)}");
			var  idealf = (carry / (loot * SettingsPage.raidCarryMin) );
			int ideal = (int)(idealf);  // carry at least the ideal amount
		    ideal = Math.Min(ideal, city.freeCommandSlots ).Max(1);
			if (idealf < ideal )//|| !SettingsPage.raidSendExact)
				idealf = ideal;
            return (ideal, carry /(ideal * loot),idealf );
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
            public int rt { get; set; }
            public int snd { get; set; }
            public int rut { get; set; }
            public string ts { get; set; }
			public int iv { get; set;  }
        }
        public static async Task<bool> SendRaids(Dungeon d, bool clearDungeonList)
        {
            var city = d.city;
            if (city == null)
                return true;
            var r = ComputeIdealReps(d,city);
			if(!clearDungeonList)
			{
				if (!d.isValid)
					return true;
			}

			var wantDelays = false;// intervals != 0 && clearDungeonList;
			for (int iter = 0; iter < (wantDelays ? r.reps : 1); ++iter)
			{
				var tr = new List<sndRaidtr>();
				foreach (var ttc in city.troopsHome)
				{
					if (!IsRaider(ttc.type) || !SettingsPage.includeRaiders[ttc.type])
						continue;
					if (IsWaterRaider(ttc.type) == d.isWater)
					{
						var count = (int)(ttc.count * troopFraction / (SettingsPage.raidSendExact? r.fractionalReps:r.reps) );
						tr.Add(new sndRaidtr() { tt = ttc.type.ToString(), tv = count.ToString() });
						ttc.count -= r.reps * count;
					}

				}
				var trs = JsonSerializer.Serialize(tr);
				var args = new sndRaidArgs() { rcid = d.cid, type = SettingsPage.wantRaidRepeat ? 1 : 2, co = wantDelays ? 1 : r.reps, rt = 1, snd = 1, rut = 0, tr = trs, iv = SettingsPage.raidIntervals + 1 };
				var snd = new COTG.Services.sndRaid(JsonSerializer.Serialize(args), city.cid);
				Note.Show($"{city.nameMarkdown} raid {r.reps}x{(r.averageCarry * 100).RoundToInt()}% carry, cavern: {d.cid.CidToStringMD()}");
				if (!await snd.Post())
					return false;
				//           await Task.Delay(500);
				//            UpdateTS(true);

			}

             city.NotifyChange(nameof(city.tsRaid));
			
            if(Raid.test)
            {
                await Task.Delay(1000);
                await city.SuperRaid();
            }
			return true;

        }
        public static DateTimeOffset nextAllowedTsHomeUpdate;
        public static DateTimeOffset nextAllowedTsUpdate;
        //public static async void UpdateTSHome(bool force = false, bool updateRaids=false)
        //{
        //    var n = DateTimeOffset.UtcNow;
        //    if (n > nextAllowedTsHomeUpdate || force)
        //    {
        //        var changed = new HashSet<City>();
        //        nextAllowedTsHomeUpdate = n + TimeSpan.FromSeconds(24);
        //        try
        //        {
        //            var jso = await Post.SendForJson("includes/gIDl.php", "");
        //            foreach (var ci in jso.RootElement.EnumerateArray())
        //            {
        //                var cid = ci.GetAsInt("i");
        //                var ts = ci.GetAsInt("ts");
        //                if (City.allCities.TryGetValue(cid, out var v))
        //                {

        //                    if ((v.tsHome - ts).Abs() > 8)
        //                    {
        //                        v.tsHome = ts;
        //                        changed.Add(v);
        //                    }
        //                }
        //            }
        //            changed.NotifyChange(nameof(City.tsHome));
        //            if (updateRaids && MainPage.IsVisible() && City.IsMine(Spot.focus))
        //                ScanDungeons.Post(Spot.focus, true);
        //        }
        //        catch (Exception e)
        //        {
        //            Log(e);
        //        }


        //    }
        //}
        // should this be waitable?
        public static async Task UpdateTS(bool force = false, bool updateRaids=false)
        {
            var n = DateTimeOffset.UtcNow;
            if (n > nextAllowedTsUpdate || force)
            {
                nextAllowedTsUpdate = n + TimeSpan.FromSeconds(24);
                nextAllowedTsHomeUpdate = nextAllowedTsUpdate; // stall this one too
                await RestAPI.troopsOverview.Post();

                if(updateRaids && DungeonView.IsVisible() )
                   await ScanDungeons.Post(DungeonView.openCity, true, false);

            }
        }
        //public static async Task UpdateTSSync(bool force = false)
        //{
        //    var n = DateTimeOffset.UtcNow;
        //    if (n > nextAllowedTsUpdate || force)
        //    {
        //        nextAllowedTsUpdate = n + TimeSpan.FromSeconds(24);
        //        nextAllowedTsHomeUpdate = nextAllowedTsUpdate; // stall this one too
        //        await RestAPI.troopsOverview.Post();
        //    }
        //}
        public static async void ReturnSlow(int cid, bool updateUI )
        {
            Note.Show($"{City.Get(cid).nameMarkdown} End Raids");
     
            if (cid != 0)
            {
                await Post.SendEncrypted("includes/UrOA.php", "{\"a\":" + cid + ",\"c\":0,\"b\":1}", "Rx3x5DdAxxerx3", World.CidToPlayerOrMe(cid));
                if (updateUI)
                {
                    // await JSClient.PollCity(cid);
                    JSClient.ChangeCity(cid,false);
                    NavStack.Push(cid);
                    //                    await Task.Delay(300); // this might not be useful.
                    //                   ScanDungeons.Post(cid, true);
                }
            }
        }
        
        public static async Task ReturnAt(int cid, DateTimeOffset at)
        {
            var json = $"{{\"a\":{cid},\"c\":\"{at.ToString(AUtil.raidDateTimeFormat)}\",\"b\":\"3\"}}";
            if (cid != 0)
            {
                await Post.SendEncrypted("includes/UrOA.php", json, "Rx3x5DdAxxerx3", World.CidToPlayerOrMe(cid));
            }
        }
        public static async Task ReturnFast(int cid, bool updateUI)
        {
            Note.Show($"{City.Get(cid).nameMarkdown} Home Please");
            if (cid != 0)
            {
                await Post.Send("overview/rcallall.php", "a=" + cid, World.CidToPlayerOrMe(cid));
                if (updateUI)
                {
                   // await JSClient.PollCity(cid);
                    await JSClient.ChangeCity(cid,false);
                    NavStack.Push(cid);
                    //// await JSClient.PollCity(cid);
                    ////  await Task.Delay(300); // this might not be useful.
                    ////  ScanDungeons.Post(cid, true);
                }
                if(NearDefenseTab.IsVisible())
                {

                //    await JSClient.PollCity(cid);
                    await RaidOverview.Send(); 
                }
            }
        }
        public static async Task ReturnSlowBatch(IEnumerable<int>  cids)
        {
			using var work = new ShellPage.WorkScope("Home Slow Please..");
			int counter = 0;
            foreach (var cid in cids)
            {

                if (cid != 0)
                {
                    var json = "{\"a\":" + cid + ",\"c\":0,\"b\":1}";
                    await Post.SendEncrypted("includes/UrOA.php", json, "Rx3x5DdAxxerx3", World.CidToPlayerOrMe(cid));
                    ++counter;
                }
            }
            Note.Show($"Issued End Raids on {counter} cities");
            ShellPage.ShowTipRefresh();
            await UpdateTS(true, true);
        }
        public static async Task ReturnFastBatch(IEnumerable<int> cids)
        {
			using var work = new ShellPage.WorkScope("Home Please..");

			int counter = 0;
            foreach (var cid in cids)
            {
                if (cid != 0)
                {
                    ++counter;
                    await Post.Send("overview/rcallall.php", "a=" + cid, World.CidToPlayerOrMe(cid));
                //    await JSClient.PollCity(cid);
                }
            }
            Note.Show($"Issued Home Please on {counter} cities");
            ShellPage.ShowTipRefresh();
            await RaidOverview.Send();
            await UpdateTS(true,true);
        }
    }
}
