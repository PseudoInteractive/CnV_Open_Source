using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using FluentAssertions;
using FluentAssertions.Common;
using static COTG.Debug;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using COTG.Helpers;
using System.Text.Json;

namespace COTG.Game
{
	public class City
	{
        public string name { get; set; }
        public int cid; // x,y combined into 1 number
        public string xy => $"{cid / 65536}:{cid % 65536}";
        public string owner { get; set; } // todo: this shoule be an int playerId
        public string alliance { get; set; }// todo:  this should be an into alliance id
        public string notes { get; set; }
        public DateTime lastUpdated { get; set; }
        public DateTime lastAccessed { get; set; } // lass user access
        public bool isCastle { get; set; }
        public bool isOnWater { get; set; }
        public bool isTemple { get; set; }
        public int points { get; set; }
        public BitmapImage icon => ImageHelper.FromImages(isCastle?"castle4.png" : "city4.png");
        public JsonElement jsE; // only for my own cities, and only if gC or similar has been called

        // Abusing invalid jsE by returning it when we want to return null
        public JsonElement troopsHome => !jsE.IsValid()?jsE : jsE.GetProperty("th");
        public JsonElement troopsTotal => !jsE.IsValid() ? jsE : jsE.GetProperty("tc");
        public string tsInfo => "Detailed info";

        public int ts
        {
            get
            {
                var tt = troopsTotal;
                if (!tt.IsValid())
                    return -1;
                return tt.EnumerateArray().Sum<JsonElement>((a) => a.GetInt32() );
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
        public static ConcurrentDictionary<int,City> all = new ConcurrentDictionary<int, City>(); // keyed by cid
        public void LoadFromJson(JsonElement jse)
        {
            jsE = jse;
            Debug.Assert(cid == jse.GetInt("cid"));
            name = jse.GetAsString("citn");
            owner = jse.GetAsString("pn");
        }
        const int bidCastle = 467;
        public (int commandSlotsInUse,int totalCommandSlots,int freeCommandSlots) GetCommandSlots()
        {
            if (!jsE.IsValid())
            {
                Log("Missing City data");
                return (0, 5, 0);
            }
            int total = 5;
            var bd = jsE.GetProperty("bd");
            foreach(var b in bd.EnumerateArray())
            {
                if(b.GetInt("bid") == bidCastle )
                {
                    total = b.GetInt("bl") + 5;
                }
            }
            var comm = jsE.GetInt("comm");
            return (comm, total, total - comm);
        }
	}
    public class BuildingCount
    {
        public BitmapImage image { get; set; }
        public int count { get; set; }

    }

}
