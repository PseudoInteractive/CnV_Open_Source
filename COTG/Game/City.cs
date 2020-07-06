using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using static COTG.Debug;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using COTG.Helpers;
using System.Text.Json;
using static COTG.Game.Enum;

namespace COTG.Game
{
    public class City : Spot
    {
      

        public string notes { get; set; }

        public JsonElement jsE; // only for my own cities, and only if gC or similar has been called

        public Raid[] raids = Array.Empty<Raid>();

        public int commandSlots
        {
            get {
                var jse = jsE;
                if (!jse.IsValid())
                {
                    return isCastle ? 15 : 5;
                }
                else
                {
                    var bd = jse.GetProperty("bd");
                    foreach (var b in bd.EnumerateArray())
                    {
                        if (b.GetAsInt("bid") == bidCastle)
                        {
                            return (b.GetInt("bl") + 5);
                        }
                    }
                    return 5;
                }
            }
        }

        public int activeCommands => jsE.IsValid() ? jsE.GetInt("comm") : 0;

        public int freeCommandSlots => commandSlots - activeCommands;

        public int carryCapacity
        {
            get {
                // Todo: water
                var _carryCapacity = 0;
                {
                    var jse = jsE;
                    if (jse.IsValid())
                    {
                        var jst = troopsHome;
                        if (jst.IsValid())
                        {
                            foreach (var troopType in ttLandRaiders)
                            {
                                _carryCapacity += jst[troopType].GetInt32() * ttCarry[troopType];
                            }
                        }
                    }
                }
                return _carryCapacity;
            }
        }

        internal void TroopsChanged()
        {
            if (current == this)
                ;// throw new NotImplementedException();
        }


        // Abusing invalid jsE by returning it when we want to return null
        public JsonElement troopsHome => !jsE.IsValid() ? jsE : jsE.GetProperty("th");
        public JsonElement troopsTotal => !jsE.IsValid() ? jsE : jsE.GetProperty("tc");

        public int ts
        {
            get
            {
                var tt = troopsTotal;
                if (!tt.IsValid())
                    return -1;
                return tt.EnumerateArray().Sum<JsonElement>((a) => a.GetInt32());
            }
        }
        public int tsHome
        {
            get
            {
                var tt = troopsHome;
                if (!tt.IsValid())
                    return -1;
                return tt.EnumerateArray().Sum<JsonElement>((a) => a.GetInt32());
            }
        }
        public static City current => all.TryGetValue(JSClient.cid, out var c) ? c : null;
        public static ConcurrentDictionary<int, City> all = new ConcurrentDictionary<int, City>(); // keyed by cid
        public void LoadFromJson(JsonElement jse)
        {
            jsE = jse;
            Debug.Assert(cid == jse.GetInt("cid"));
            name = jse.GetAsString("citn");
            Note.L($"{name} {jse.GetInt("cid")}");
            pid = jse.GetAsInt("pid");

//            if(COTG.Views.MainPage.cache.cities.Count!=0)
              COTG.Views.MainPage.CityChange(this);
//            COTG.Views.MainPage.CityListUpdateAll();
        }

        const int bidCastle = 467;
        public (int commandSlotsInUse, int totalCommandSlots, int freeCommandSlots) GetCommandSlots()
        {
            if (!jsE.IsValid())
            {
                Log("Missing City data");
                return (0, 5, 0);
            }
            int total = 5;
            var bd = jsE.GetProperty("bd");
            foreach (var b in bd.EnumerateArray())
            {
                if (b.GetInt("bid") == bidCastle)
                {
                    total = b.GetInt("bl") + 5;
                }
            }
            var comm = jsE.GetInt("comm");
            return (comm, total, total - comm);
        }
   
        public byte raidCarry { get; set; }
        public static City Factory(int _id) => new City() { cid=_id };
        public override string ToString()
        {
            return $"{{{nameof(name)}={name}, {nameof(xy)}={xy}, {nameof(pid)}={pid}, {nameof(alliance)}={alliance}, {nameof(notes)}={notes}, {nameof(lastUpdated)}={lastUpdated.ToString()}, {nameof(lastAccessed)}={lastAccessed.ToString()}, {nameof(isCastle)}={isCastle.ToString()}, {nameof(isOnWater)}={isOnWater.ToString()}, {nameof(isTemple)}={isTemple.ToString()}, {nameof(points)}={points.ToString()}, {nameof(icon)}={icon}, {nameof(ts)}={ts.ToString()}, {nameof(tsHome)}={tsHome.ToString()}}}";
        }
    }
    public class BuildingCount
    {
        public BitmapImage image { get; set; }
        public int count { get; set; }

    }

}
