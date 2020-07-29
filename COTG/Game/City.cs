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
using COTG.Views;

namespace COTG.Game
{
    [System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class City : Spot 
    {

        public string remarks { get; set; }

        public JsonElement jsE; // only for my own cities, and only if gC or similar has been called

        public Raid[] raids = Array.Empty<Raid>();

        public static City focus; // city that has focus (selected, but not necessarily building.  IF you click a city once, it goes to this state
        public static City build; // city that has Build selection.  I.e. in city view, the city you are in

        public static City GetOrAddCity(int cid)
        {
            Assert(cid > 65536);
            return allCities.GetOrAdd(cid, (cid) => new City() { cid = cid });
        }

        public static bool IsMine(int cid)
        {
            return allCities.ContainsKey(cid);
        }
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
                    if (jse.TryGetProperty("bd", out var bd))
                    {
                        foreach (var b in bd.EnumerateArray())
                        {
                            if (b.GetAsInt("bid") == bidCastle)
                            {
                                return (b.GetInt("bl") + 5);
                            }
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
                foreach(var tc in troopsHome)
                {
                    if(ttLandRaiders.Contains((byte)tc.type) )
                            {
                                _carryCapacity += tc.count * ttCarry[tc.type];
                            }
                        
                    
                }
                return _carryCapacity;
            }
        }

        public float raidReturn
        {
            get
            {
                if (raids.IsNullOrEmpty())
                    return 999; // no troops
                var dt = (float)(raids[0].arrival- JSClient.ServerTime()).TotalMinutes; // should we check more than one
                if (raids[0].isReturning)
                    return dt;
                else
                    return (-1.0f).Min(dt-75f);
            }
        }


        // Abusing invalid jsE by returning it when we want to return null
        //  public JsonElement troopsHome => !jsE.IsValid() ? jsE : jsE.GetProperty("th");
        //  public JsonElement troopsTotal => !jsE.IsValid() ? jsE : jsE.GetProperty("tc");

        public int tsTotal { get; set; } // ts total including those on commands like raids, def etc.

        public TroopTypeCount[] troopsHome = Array.Empty<TroopTypeCount>();
        public TroopTypeCount[] troopsTotal = Array.Empty<TroopTypeCount>();
        public static ConcurrentDictionary<int, City> allCities = new ConcurrentDictionary<int, City>(); // keyed by cid
        public void LoadFromJson(JsonElement jse)
        {
            jsE = jse;
            Debug.Assert(cid == jse.GetInt("cid"));
            cityName = jse.GetAsString("citn");
            Note.L($"{cityName} {jse.GetInt("cid")}");
            pid = jse.GetAsInt("pid");

            troopsHome = Array.Empty<TroopTypeCount>();
            troopsTotal = Array.Empty<TroopTypeCount>();

            for (int hc = 0; hc < 2; ++hc)
            {


                if (jse.TryGetProperty((hc == 0) ? "th" : "tc", out var tt))
                {
                    var tc = new List<TroopTypeCount>();
                    var tType = -1;
                    // stores one count per troop type, mostly 0s
                    foreach (var a in tt.EnumerateArray())
                    {
                        ++tType;
                        var count = a.GetInt32();
                        if (count > 0)
                        {
                            tc.Add(new TroopTypeCount(tType, count));
                        }

                    }
                    if (tc.Count > 0)
                    {
                        if (hc == 0)
                            troopsHome = tc.ToArray();
                        else
                            troopsTotal = tc.ToArray();
                    }

                }
            }
            tsHome = TroopTypeCount.TS(troopsHome);
            tsTotal = TroopTypeCount.TS(troopsTotal);
            CheckTipRaiding();

            //            if(COTG.Views.MainPage.cache.cities.Count!=0)
            // one off change
            NotifyChange();

            //   OnPropertyChangedUI(String.Empty);// COTG.Views.MainPage.CityChange(this);
            //            COTG.Views.MainPage.CityListUpdateAll();
        }

        public void CheckTipRaiding()
        {
            if (TipsSeen.instance.raiding1 == false)
            {
                if (this.tsHome == this.tsTotal && this.tsHome > 4000)
                {
                    MainPage.ShowTipRaiding1();
                }
            }
        }

        static List<City> dummies = new List<City>();


        public void NotifyChange(string member ="")
        {
            App.DispatchOnUIThreadSneaky(() =>
       {
           OnPropertyChanged(member);
       });
            //spots

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
            if (jsE.TryGetProperty("bd", out var bd))
            {
                foreach (var b in bd.EnumerateArray())
                {
                    if (b.GetInt("bid") == bidCastle)
                    {
                        total = b.GetInt("bl") + 5;
                    }
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

        //private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        //{
        //    if (Equals(storage, value))
        //    {
        //        return;
        //    }
        //    storage = value;
        //    OnPropertyChanged(propertyName);
        //}

        //public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //public void OnPropertyChangedUI(string propertyName) => App.DispatchOnUIThreadLow(()=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));

        public override string ToString()
        {
            return $"{{{nameof(cityName)}={cityName}, {nameof(xy)}={xy}, {nameof(cid)}={cid},{nameof(tsTotal)}={tsTotal.ToString()}, {nameof(tsHome)}={tsHome.ToString()}}}";
        }
        public async static void UpdateSenatorInfo()
        {
            var a = await Post.SendForJson("overview/senfind.php", "a=0");
            var empty = Array.Empty<SenatorInfo>();
            var changed = new HashSet<City>();
            foreach (var city in City.allCities.Values)
            {
                if (city.senatorInfo != empty)
                {
                    city.senatorInfo = empty;
                    changed.Add(city);
                }

            }



            foreach (var cit in a.RootElement.GetProperty("b").EnumerateArray())
            {
                var cid = cit[0].GetInt32();
              //  Log(cid.ToString());
                var city = City.allCities[cid];
                List<SenatorInfo> sens = new List<SenatorInfo>();
                foreach (var target in cit[7].EnumerateArray())
                {
                    sens.Add(new SenatorInfo()
                    {
                        type = SenatorInfo.Type.recruit,
                        count = (byte)target[0].GetInt32(),
                        time = target[1].GetString().ParseDateTime(false)
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
                        time=target[1].GetString().ParseDateTime(false)
                    });
                }
                city.senatorInfo = sens.ToArray();
                changed.Add(city);

            }
            
            changed.NotifyChange();//.OnPropertyChangedUI(nameof(City.senny));

        }

		public override bool Equals(object obj)
		{
			return obj is City city &&
				   base.Equals(obj) &&
				   cid == city.cid;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(base.GetHashCode(), cid);
		}

		public static bool operator ==(City left, City right)
		{
			return EqualityComparer<City>.Default.Equals(left, right);
		}

		public static bool operator !=(City left, City right)
		{
			return !(left == right);
		}
        public static void SetFocus(int cid, bool fromUI, bool noRaidScan, bool getCityData)
        {
            if (City.allCities.TryGetValue(cid, out var city))
            {
                city.SetFocus( fromUI, noRaidScan, getCityData);
            }
        }

        public void SetFocus(bool fromUI, bool noRaidScan, bool getCityData)
        {
            var changed = this != City.focus;
            City.focus = this;
 //           if (!fromUI && changed && MainPage.IsVisible())
 //               MainPage.CityGrid.SelectItem(this);
            if (!noRaidScan)
            {
                if (changed)
                    ScanDungeons.Post(cid, getCityData);
            }
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
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
    public class CityList
    {
        public string name { get; set; }
        public int id { get; set; } // 0 is unassigned, others are pids
        public HashSet<int> cities = new HashSet<int>(); // list of cities

        public static CityList Find(int id)
        {
            foreach (var c in all)
                if (c.id == id)
                    return c;
            return null;
        }
        public static CityList Find(string name)
        {
            foreach (var c in all)
                if (c.name == name)
                    return c;
            return null;
        }
        public static CityList FindForContinent(int id) => Find(id.ToString());


        public static CityList allCities = new CityList() { id = -1, name = "All" }; // special item for ui selection
        public static CityList[] all = Array.Empty<CityList>();
        public static DumbCollection<CityList> selections = new DumbCollection<CityList>( new[] { allCities }); // Similar to the above array, but a dummy "All" entry (id=-1) at the start for Combo Boxes

     
    }

}
