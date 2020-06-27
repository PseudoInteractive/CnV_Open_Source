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
    public class City
    {
        public JsonElement jsE; // only for my own cities, and only if gC or similar has been called

        public Raid[] raids = Array.Empty<Raid>();

        readonly static int[] pointSizes = { 500, 1000, 2500, 4000, 5500, 7000, 8000 };
        const int pointSizeCount = 7;
        int GetSize() {
            for (int i = 0; i < pointSizeCount; ++i)
                if (points <= pointSizes[i])
                    return i;
            return pointSizeCount;
        }
        public string name { get; set; }
        public int cid; // x,y combined into 1 number
        public string xy => $"{cid % 65536}:{cid / 65536}";
        public string owner { get; set; } // todo: this shoule be an int playerId
        public string alliance { get; set; }// todo:  this should be an into alliance id
        public string notes { get; set; }
        public DateTime lastUpdated { get; set; }
        public DateTime lastAccessed { get; set; } // lass user access
        public bool isCastle { get; set; }
        public bool isOnWater { get; set; }
        public bool isTemple { get; set; }
        public ushort points { get; set; }
        public BitmapImage icon => ImageHelper.FromImages($"{(isCastle ? "castle" : "city")}{GetSize()}.png");
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
            owner = jse.GetAsString("pn");
            COTG.Views.MainPage.CityChange(this);

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
            return $"{{{nameof(name)}={name}, {nameof(xy)}={xy}, {nameof(owner)}={owner}, {nameof(alliance)}={alliance}, {nameof(notes)}={notes}, {nameof(lastUpdated)}={lastUpdated.ToString()}, {nameof(lastAccessed)}={lastAccessed.ToString()}, {nameof(isCastle)}={isCastle.ToString()}, {nameof(isOnWater)}={isOnWater.ToString()}, {nameof(isTemple)}={isTemple.ToString()}, {nameof(points)}={points.ToString()}, {nameof(icon)}={icon}, {nameof(ts)}={ts.ToString()}, {nameof(tsHome)}={tsHome.ToString()}}}";
        }
    }
    public class BuildingCount
    {
        public BitmapImage image { get; set; }
        public int count { get; set; }

    }

}
