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

        public City() { type = typeCity; }
       

        public Raid[] raids = Array.Empty<Raid>();

        public static int build; // city that has Build selection.  I.e. in city view, the city you are in

        public bool isBuild => cid == build;
        public static bool IsBuild( int cid )
        {
            return build == cid;
        }
        public static City GetOrAddCity(int cid)
        {
            Assert(cid > 65536);
           var rv = allCities.GetOrAdd(cid, City.Factory );
			Assert(rv.cid == cid);
			return rv;
        }
        public bool AreRaidsRepeating()
        {
            foreach (var r in raids)
            {
                if (r.isRepeating)
                    return true;
            }
            return false;
        }
		public override string troopsString
		{
			get
			{
				string rv = string.Empty;
				string sep = string.Empty;
				foreach (var ttc in troopsTotal)
				{
					rv += $"{sep}{troopsHome.Count(ttc.type):N0}/{ttc.count:N0} {Enum.ttNameWithCaps[type]}";
					sep = ", ";
				}
				return rv;
			}
		}

		public DateTimeOffset GetRaidReturnTime()
        {
            var rv = AUtil.dateTimeZero;
            foreach(var r in raids)
            {
                if(r.isReturning)
                {
                    if (rv < r.time)
                        rv = r.time;
                }
                else
                {
                    
                    var travel = r.GetOneWayTripTimeMinutes(this);
                    var _t = r.time + TimeSpan.FromMinutes(travel);
                        if (_t > rv)
                            rv = _t;
                    
                }
            }
            return rv;
        }
        public static bool IsMine(int cid)
        {
            return allCities.ContainsKey(cid);
        }
        public byte commandSlots { get; set; }

        public byte activeCommands { get; set; }

        public int freeCommandSlots => commandSlots - activeCommands;

        public int carryCapacity
        {
            get {
                // Todo: water
                var _carryCapacity = 0;
                foreach(var tc in troopsHome)
                {
                  _carryCapacity += tc.count * ttCarry[tc.type];
                }
                return _carryCapacity;
            }
        }
        public float homeTroopsAttack
        {
            get
            {
                // Todo: water
                var atk = 0.0f;
                foreach (var tc in troopsHome)
                {
                    // todo: research?
                    atk += tc.count * ttAttack[tc.type] *ttCombatBonus[tc.type];
                }
                return atk;
            }
        }
        public float CarryCapacity(bool forWater)
        {
                // Todo: water
                var _carryCapacity = 0.0f;
                foreach (var tc in troopsHome)
                {
                if (!IsRaider(tc.type) || !Raid.includeRaiders[tc.type])
                    continue;
                    if(IsWaterRaider(tc.type) == forWater)
                    {
                        _carryCapacity += tc.count * ttCarry[tc.type] * Raiding.troopFraction;
                    }


                }
                return _carryCapacity;
        }

        public float raidReturn
        {
            get
            {
                if (raids.IsNullOrEmpty())
                    return 999; // no raids
                var dt = (float)(raids[0].time- JSClient.ServerTime()).TotalMinutes; // should we check more than one
                if (raids[0].isReturning)
                    return dt;
                else
                    return (-1.0f).Min(dt- raids[0].GetOneWayTripTimeMinutes(this) );
            }
        }

        public void SelectInWorldView( bool lazyMove)
        {
                if (!isBuild)
                {
                    JSClient.ChangeCity(cid, lazyMove); // keep current view, switch to city
    
                }
                if(!JSClient.IsWorldView())
                    JSClient.ChangeView(false);// toggle between city/region view

                NavStack.Push(cid);

        }

        // Abusing invalid jsE by returning it when we want to return null
        //  public JsonElement troopsHome => !jsE.IsValid() ? jsE : jsE.GetProperty("th");
        //  public JsonElement troopsTotal => !jsE.IsValid() ? jsE : jsE.GetProperty("tc");


        public TroopTypeCount[] troopsHome = TroopTypeCount.empty;
        public TroopTypeCount[] troopsTotal = TroopTypeCount.empty;
        public static ConcurrentDictionary<int, City> allCities = new ConcurrentDictionary<int, City>(); // keyed by cid

      

        public void LoadFromJson(JsonElement jse)
        {
            Debug.Assert(cid == jse.GetInt("cid"));
            if (jse.TryGetProperty("citn", out var citn))
                _cityName = citn.GetString();
            if (jse.TryGetProperty("w", out var isOnWataer))
            {
                var i = isOnWataer.GetAsInt();
                if (i==1)
                    isOnWater=true;
                else if(i==0)
                    isOnWater=false;
            }
			if (jse.TryGetProperty("cn", out var cn))
			{
				remarks = cn[0].GetAsString();
				notes = cn[1].GetAsString();
				
			}

			activeCommands = jse.GetAsByte("comm");
            {
                const int bidCastle = 467;
                if (jse.TryGetProperty("bd", out var bd))
                {
                        commandSlots = 5;
                        isCastle = false;
                        foreach (var b in bd.EnumerateArray())
                        {
                            if (b.GetAsInt("bid") == bidCastle)
                            {
                                commandSlots= (byte)( (b.GetInt("bl") + 5));
                                isCastle = true;
                            }
                        }
                    }
                }

            troopsHome = TroopTypeCount.empty;
            troopsTotal = TroopTypeCount.empty;



                if (jse.TryGetProperty("tc", out var tc))
                {
                    troopsTotal = tc.GetTroopTypeCount().ToArray(); ;
                }
            if (jse.TryGetProperty("th", out var th))
            {
                troopsHome = th.GetTroopTypeCount().ToArray(); ;
            }
            if (jse.TryGetProperty("trintr", out var trintr))
            {
                var l = new List<Reinforcement>();
                foreach (var rein in trintr.EnumerateArray())
                {
                    if (rein.ValueKind==JsonValueKind.Object)
                    {
                        var re = new Reinforcement();
                        re.targetCid=cid;
                        re.sourceCid =rein.GetAsInt("c");
                        re.order = rein.GetAsInt64("o");
                        re.troops = rein.GetProperty("tr").GetTroopTypeCount2().ToArray();
                        l.Add(re);
                    }
                }
                reinforcementsIn = l.ToArray();
            }
            if (jse.TryGetProperty("trin", out var trin))
            {
                Log(trin);

            }
            if (jse.TryGetProperty("triin", out var triin))
            {
                Log(triin);

            }


            tsRaid = troopsHome.TSRaid();
            tsHome = troopsHome.TS();
            tsTotal = troopsTotal.TS();
          
            //            if(COTG.Views.MainPage.cache.cities.Count!=0)
            // one off change
            NotifyChange();
            

            //   OnPropertyChangedUI(String.Empty);// COTG.Views.MainPage.CityChange(this);
            //            COTG.Views.MainPage.CityListUpdateAll();
        }

        

        internal static City GetBuild()
        {
            if (build!=0 && allCities.TryGetValue(build, out var city))
                return city;
            return null;
        }

        public static void CheckTipRaiding()
        {
            //if (TipsSeen.instance.raiding1 && TipsSeen.instance.raiding3 )
            //    return;
            //int homeCount = 0;
            //foreach (var city in City.allCities)
            //{
            //    if (city.Value.tsRaid >= city.Value.tsTotal && city.Value.tsRaid > 4000)
            //    {
            //        ++homeCount;
            //    }
            //}
            //if (homeCount == 0)
            //    return;
            //if (MainPage.instance.TipRaiding101.Show())
            //{
            //    return;
            //}
            //if (homeCount < 2)
            //    return;
            //if (!TipsSeen.instance.raiding2)
            //{
            //    MainPage.ShowTipRaiding2();
            //    return;
            //}
            //if (homeCount < 4)
            //    return;
            //if (!TipsSeen.instance.raiding3)
            //{
            //    MainPage.ShowTipRaiding3();
            //    return;
            //}

        }

        //  static List<City> dummies = new List<City>();


        public void NotifyChange(string member = "")
        {
            App.DispatchOnUIThreadSneaky(() =>
       {
           OnPropertyChanged(member);
       });
            if (NearDefenseTab.IsVisible())
            {
                foreach (var i in NearDefenseTab.supporters)
                {
                    if (i.city == this)
                    {
                        i.NotifyChange(member);
                         break;
                    }
                }

            }
        }
      

        
       
   
        public byte raidCarry { get; set; }
        public static City Factory(int cid)
        {
            var rv = new City() { cid = cid, pid=Player.myId };
            allSpots[cid]= rv;
            return rv;
        }

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

            try
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
                    if (!City.allCities.TryGetValue(cid, out var city))
                        continue;
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
                    if (idle != 0)
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
                            target = target[0].GetInt32(),
                            time = target[1].GetString().ParseDateTime(false)
                        });
                    }
                    city.senatorInfo = sens.ToArray();
                    changed.Add(city);

                }

                changed.NotifyChange(nameof(City.senny));
            }
            catch (Exception ex)
            {
                Log(ex);
            }



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
       
       
        public static (City city,bool changed) StBuild(int cid,bool scrollIntoView, bool select=true)
        {
            var city = City.GetOrAddCity(cid);
            return (city, city.SetBuild(scrollIntoView,select));
        }
        public bool SetBuild(bool scrollIntoView, bool select=true)
        {
            var changed = cid != build;
            City.build = cid;

            SetFocus(scrollIntoView, select);
            return changed;
            //if (!noRaidScan)
           // {
          //      if (changed)
          //          ScanDungeons.Post(cid, getCityData);
          //  }
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
        public byte GetRaidTroopType()
        {
            byte best = 0; // if no raiding troops we return guards 
            var bestTS = 0;
            foreach (var ttc in troopsHome)
            {
                var type = ttc.type;
                if (!IsRaider(type) || !Raid.includeRaiders[type])
                    continue;
                var ts = ttc.ts;
                if (ts > bestTS)
                {
                    bestTS = ts;
                    best = (byte)type;
                }

            }
            return best;
        }
        public override byte GetPrimaryTroopType(bool onlyHomeTroops) // troop type with most TS
        {
            byte best = 0; // if no raiding troops we return guards 
            var bestTS = 0;
            foreach (var ttc in (onlyHomeTroops?troopsHome:troopsTotal))
            {
                var type = ttc.type;
                var ts = ttc.ts;
                if (ts > bestTS)
                {
                    bestTS = ts;
                    best = (byte)type;
                }

            }
            if (best==0)
                return base.GetPrimaryTroopType(onlyHomeTroops);
            else
                return best;
        }

        public byte GetIdealDungeonType()
        {
            // todo:  handle water
            byte best = 0;
            var bestTS = 0;
            foreach (var ttc in troopsHome)
            {
                var type = ttBestDungeonType[ttc.type];
                if (type >= (byte)DungeonType.water )
                    continue;// todo: handle water
                var ts = ttc.ts;
                if (ts > bestTS)
                {
                    bestTS = ts;
                    best = type;
                }

            }
            return best;
        }
        /*
         * Resource and other details
        */
        public ushort carts { get; set; }
        public ushort cartsHome { get; set; }
        public int wood { get; set; }
        public int woodStorage { get; set; }
        public int stone { get; set; }
        public int stoneStorage { get; set; }
        public int iron { get; set; }
        public int ironStorage { get; set; }
        public int food { get; set; }
        public int foodStorage { get; set; }
        public ushort ships { get; set; }
        public ushort shipsHome { get; set; }
        public bool academy { get; set; }
        public bool sorcTower { get; set; }

        public static DumbCollection<City> gridCitySource = new DumbCollection<City>();
        public static City[] emptyCitySource = Array.Empty<City>();

       
        public bool ComputeTravelTime(int target, bool onlyHome, out float hours, out bool onDifferentContinent)
        {
            hours = 0;
            onDifferentContinent = cont != target.CidToContinent();
            if (onDifferentContinent)
            {
                if(Spot.GetOrAdd(target).isOnWater && (onlyHome?troopsHome:troopsTotal).Any((t)=> t.type==ttGalley || t.type==ttStinger))
                {
                    var tt = ttGalley;
                    var dist = cid.DistanceToCid(target);
                    hours = dist * ttTravel[tt] / (60f * ttSpeedBonus[tt])+1;
                    return true;

                }
                return false;

            }
            else
            {
                var tt = GetPrimaryTroopType(onlyHome);
                if (tt == 0)
                    return false;
                var dist = cid.DistanceToCid(target);
                hours = dist * ttTravel[tt] / (60f * ttSpeedBonus[tt]);
                return true;
            }
        }
		//static City lastDugeonScanCity;
		public DumbCollection<Dungeon> dungeons
		{
			get
			{
				return Dungeon.raidDungeons;
			}
		}
		public string dungeonsToggle => MainPage.expandedCity==this ? "-" : "+";
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
        public static bool IsNew(City city) => city._cityName == "*New City" && city.points <= 60;
        public CityList(string _name) { name = _name; id = AMath.random.Next(65536) + 10000; }
        public CityList() { }
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
        public static string[] perContinentTags = { "rt", "vanq", "priest", "prae","sorc","horse","druid","arb","scorp" };
        public static string[] globalTags = { "navy","warship", "shipp", "stinger","galley" };
        public static CityList GetForContinent(int id) => GetOrAdd(id.ToString());
        public static CityList GetForContinentAndTag(int id,string tag) => GetOrAdd($"{id.ToString()} {tag}");

        public static CityList GetOrAdd(string name)
        {
            var cl = CityList.Find(name);
            if (cl == null)
            {
                var id = AMath.random.Next(65536) + 10000;
                cl = new CityList(name);
                CityList.all = CityList.all.ArrayAppend(cl);
            }
            return cl;
        }
        public static CityList FindNewCities() => Find(sNewCities); 

        public static CityList allCities = new CityList() { id = -1, name = "All" }; // special item for ui selection
        public static CityList[] all = Array.Empty<CityList>();
        public static CityList [] selections = new [] { allCities }; // Similar to the above array, but a dummy "All" entry (id=-1) at the start for Combo Boxes
        internal const string sNewCities = "NewCities";

        public static ComboBox box => ShellPage.instance.cityListBox;
        public static void SelectedChange()
        {
 
            App.DispatchOnUIThreadLow( () =>
            {
 //               Log("CityListChange");

                var selectedCityList = CityList.box.SelectedItem as CityList;
                IEnumerable<City> l;
                if (selectedCityList == null || selectedCityList.id == -1) // "all"
                {
                    l = City.allCities.Values;
                }
                else
                {
                    var cityList = selectedCityList;// CityList.Find(selectedCityList);
                    var filtered = new List<City>();
                    foreach (var cid in cityList.cities)
                    {
                        if (City.allCities.TryGetValue(cid, out var c))
                        {
                            filtered.Add(c);
                        }
                    }
                    l = filtered;
                }
                l = l.OrderBy((a) => a._cityName).ToArray();
                ShellPage.instance.cityBox.ItemsSource = l;
                var reserveCartsFilter = DonationTab.reserveCarts;
                if (DonationTab.IsVisible())
                    DonationTab.instance.donationGrid.ItemsSource = l.Where((city) => city.cartsHome >= reserveCartsFilter)
                        .OrderByDescending(a=>a.cartsHome).ToArray();
             //   if (MainPage.IsVisible())
                    City.gridCitySource.Set(l);
                   City.GetBuild().SelectInUI(true);
            });
        }
        
    }

}
