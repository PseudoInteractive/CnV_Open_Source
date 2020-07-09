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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using COTG.Services;

namespace COTG.Game
{
    public class City : Spot, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

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
        public int tsHome   {  get;set;  }
        public static City current => all.TryGetValue(JSClient.cid, out var c) ? c : null;
        public static ConcurrentDictionary<int, City> all = new ConcurrentDictionary<int, City>(); // keyed by cid
        public void LoadFromJson(JsonElement jse)
        {
            jsE = jse;
            Debug.Assert(cid == jse.GetInt("cid"));
            name = jse.GetAsString("citn");
            Note.L($"{name} {jse.GetInt("cid")}");
            pid = jse.GetAsInt("pid");

            {
                var tt = troopsHome;
                if (tt.IsValid())
                {
                    tsHome = tt.EnumerateArray().Sum<JsonElement>((a) => a.GetInt32());
                }
            }
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

        public SenatorInfo[] senatorInfo = Array.Empty<SenatorInfo>();
        public string senny
        { get {
                if (senatorInfo.Length == 0)
                    return string.Empty;
                string rv = string.Empty;
                bool wantSpace = false;
                //var summary = new SortedList<byte,byte>();
                //foreach (var si in senatorInfo)
                //{
                //    var sv = summary.GetOrAdd((byte)si.type);
                //}
                foreach (var s in senatorInfo)
                {
                    if (wantSpace)
                        rv += ",";
                    wantSpace = true;
                    var type = s.type switch
                    {
                        SenatorInfo.Type.idle => "zzz",
                        SenatorInfo.Type.recruit=>"recr",
                        SenatorInfo.Type.settle => "setl",
                        SenatorInfo.Type.siege  => "seig",
                        _ => "Error"
                    };
                    rv += $"{type}:{s.count}";

                }
                return rv;

            }
        }

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }
            storage = value;
            OnPropertyChanged(propertyName);
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public override string ToString()
        {
            return $"{{{nameof(name)}={name}, {nameof(xy)}={xy}, {nameof(pid)}={pid}, {nameof(alliance)}={alliance}, {nameof(notes)}={notes}, {nameof(lastUpdated)}={lastUpdated.ToString()}, {nameof(lastAccessed)}={lastAccessed.ToString()}, {nameof(isCastle)}={isCastle.ToString()}, {nameof(isOnWater)}={isOnWater.ToString()}, {nameof(isTemple)}={isTemple.ToString()}, {nameof(points)}={points.ToString()}, {nameof(icon)}={icon}, {nameof(ts)}={ts.ToString()}, {nameof(tsHome)}={tsHome.ToString()}}}";
        }
        public async static void UpdateSenatorInfo()
        {
            var a = await Post.SendForJson("overview/senfind.php", "a=0");
            var empty = Array.Empty<SenatorInfo>();
            foreach (var city in City.all.Values)
            {
                if (city.senatorInfo != empty)
                {
                    city.senatorInfo = empty;
                    city.OnPropertyChanged(nameof(City.senny));
                }

            }

            foreach (var cit in a.RootElement.GetProperty("b").EnumerateArray())
            {
                var cid = cit[0].GetInt32();
                Log(cid.ToString());
                var city = City.all[cid];
                List<SenatorInfo> sens = new List<SenatorInfo>();
                foreach (var target in cit[7].EnumerateArray())
                {
                    sens.Add(new SenatorInfo()
                    {
                        type = SenatorInfo.Type.recruit,
                        count = (byte)target[0].GetInt32(),
                        time = target[1].GetString().ParseDateTime()
                    });
                }
                var idle = cit[4].GetByte();
                if(idle != 0)
                {
                    sens.Add(new SenatorInfo()
                    {
                        type = SenatorInfo.Type.idle,
                        count = idle
                    });
                }

                foreach (var target in cit[8].EnumerateArray())
                {
                    sens.Add(new SenatorInfo()
                    {
                        type = SenatorInfo.Type.settle,
                        count = 1,
                        target=target[0].GetInt32(),
                        time=target[1].GetString().ParseDateTime()
                    });
                }
                city.senatorInfo = sens.ToArray();
                city.OnPropertyChanged(nameof(City.senny));

            }

        }

    }
    public class SenatorInfo
    {
        public enum Type : byte
        {
            recruit,
            settle,
            siege,
            idle,
        }
        public Type type;
        public byte count;
        public int target; // only for seige and settle
        public DateTimeOffset time;  // not for idle
    }
    public class BuildingCount
    {
        public BitmapImage image { get; set; }
        public int count { get; set; }

    }

}
