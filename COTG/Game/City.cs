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
using COTG.JSON;
using static COTG.Game.City;
using Microsoft.Toolkit.HighPerformance.Extensions;
using Microsoft.Toolkit.HighPerformance.Enumerables;
using COTG.Draw;

namespace COTG.Game
{
	//struct Building
	//{
	//	public byte type;
	//	public byte level;
	//	// bu:  time to upgrade
	//	// bd:  time to demo?
	//}

    public sealed class City : Spot 
    {
		
		public const short bidTownHall = 455;
		public const short bidWall = 809;
		public const short bidTemple = 890;
		 public const short bidForester= 448;
		 public const short bidCottage= 446;
		 public const short bidStorehouse= 464;
		 public const short bidQuarry= 461;
		 public const short bidHideaway= 479;
		 public const short bidFarmhouse= 447;
		 public const short bidCityguardhouse= 504;
		 public const short bidBarracks= 445;
		 public const short bidMine= 465;
		 public const short bidTrainingground= 483;
		 public const short bidMarketplace= 449;
		 public const short bidTownhouse= 481;
		 public const short bidSawmill= 460;
		 public const short bidStable= 466;
		 public const short bidStonemason= 462;
		 public const short bidMage_tower= 500;
		 public const short bidWindmill= 463;
		 public const short bidAcademy= 482;
		 public const short bidSmelter= 477;
		 public const short bidBlacksmith= 502;
		 public const short bidCastle= 467;
		 public const short bidPort= 488;
		 public const short bidShipyard= 491;
		public const short bidTriariPost = 539;
		public const short bidRangerPost = 543;
		public const short bidSentinelPost = 547;
		public const short bidPriestessPost = 551;
		public const short bidBallistaPost = 555;
		public const short bidEquineBarricade = 559;
		public const short bidRuneBarricade = 563;
		public const short bidSnagBarricade = 567;
		public const short bidVeiledBarricade = 571;


		public const short bidStone = 451;
		public const short bidIron = 452;
		public const short bidLake = 453;
		public const short bidForest = 454;
		public const int bspotWall = 0;
		public const int bspotTownHall = span1 * citySpan + span1;

		public static int XYToId((int x, int y) xy) => (xy.x.Clamp(span0, span1) - span0) + (xy.y.Clamp(span0, span1) - span0) * citySpan;
		
		public static (int x, int y) IdToXY(int id)
		{
			var y = id / citySpan;
			return (id - y * citySpan+ span0, y+ span0);
		}

		public const int span1 = (citySpan - 1) / 2; // inclusive
		public const int span0 = -span1; // inclusive

		//static string[] buildingNames = { "forester", "cottage", "storehouse", "quarry", "hideaway", "farmhouse", "cityguardhouse", "barracks", "mine", "trainingground", "marketplace", "townhouse", "sawmill", "stable", "stonemason", "mage_tower", "windmill", "temple", "smelter", "blacksmith", "castle", "port", "port", "port", "shipyard", "shipyard", "shipyard", "townhall", "castle" };
		//const short bidTownHall = 455;
		//static short[] bidMap = new short[] { 448, 446, 464, 461, 479, 447, 504, 445, 465, 483, 449, 481, 460, 466, 462, 500, 463, 482, 477, 502, 467, 488, 489, 490, 491, 496, 498, bidTownHall, 467 };

		public const int citySpan = 21;
		public const int citySpotCount = citySpan* citySpan;
		public const int cityScratchSpot = citySpotCount - 1;
		public static Building[] Emptybuildings = new Building[citySpotCount];

		public Building[] buildings = Emptybuildings;
		
		

		public Building GetBuiding((int x, int y) xy) => buildings[XYToId(xy)];
		public Building GetBuiding( int bspot) => buildings[bspot];
		
		public static DArray<BuildQueueItem> buildQueue = new DArray<BuildQueueItem>(128);// fixed size to improve threading behaviour and performance
		
		public const int buildQMax = 16; // this should depend on ministers
		public static bool buildQueueFull => buildQueue.count >= buildQMax;
		public static bool wantBuildCommands => buildQueue.count < safeBuildQueueLength;
		public const int safeBuildQueueLength = 15; // leave space for autobuild

		public static IEnumerable<BuildQueueItem> IterateQueue()
		{
			foreach (var i in buildQueue)
				yield return i;

			if (BuildQueue.TryGetQueue(out var q))
			{
				while (q.MoveNext())
				{
					yield return q.Current;
				}
			}
		}
		//public static Span<BuildQueueItem> Combine( Span<BuildQueueItem> q0,Span<BuildQueueItem> q)
		//{
		//	var rv = BuildQueueItem.pool.Rent(q0.Length + (q.Length));
		//	var sp= rv.Memory.Span;			

		//	var put = 0;
		//	q0.CopyTo(REfrv,)
		//	foreach (var i in q0)
		//	{
		//		sp[put++] = i;
		//	}
		//	foreach(var i in q)
		//	{
		//		sp[put++] = i;
		//	}	
		//	return sp;

		//}
		//public static Span<BuildQueueItem> GetQueue(int cid,  Span<BuildQueueItem> q0)
		//{
		//	var city = City.GetOrAdd(cid);
		//	return GetQueue(cid,q0,);

		//	var rv = BuildQueueItem.pool.Rent(q0.Length + (q.Length));
		//	var s = rv.Memory;
		//	var sp = s.Span;
		//	var put = 0;
		//	foreach (var i in q0)
		//	{
		//		sp[put++] = i;
		//	}


		//	foreach (var i in q)
		//	{
		//		sp[put++] = i;
		//	}

		//	return sp;

		//}
		public static Span<BuildQueueItem> GetExtQueue(int cid)
		{

			if (CityBuildQueue.all.TryGetValue(cid, out var q))
			{
				return new Span<BuildQueueItem>(q.queue.ToArray());
			}
			return new Span<BuildQueueItem>();
	

		}

		//public static Span<BuildQueueItem> GetQueue()
		//{

		//	return GetQueue(City.build, City.buildQueue.span, GetExtQueue(build));
	

		//}

		public static void IterateQueue(  Action<BuildQueueItem> action )
		{
			foreach (var i in buildQueue)
				action(i);
			
			if (BuildQueue.TryGetQueue(out var q))
			{
				while (q.MoveNext())
				{
					action(q.Current);
				}
			}
		}
		public static void CitySwitched()
		{
			AUtil.UnsafeCopy(CityView.baseAnimationOffsets, CityView.animationOffsets);
			buildQueue.Clear(); 
			Draw.CityView.ClearSelectedBuilding();
			CityBuild.ClearAction();
			CityView.BuildingsOrQueueChanged();

			if (CityBuild.menuOpen)
			{
				App.DispatchOnUIThreadSneakyLow(() =>
			   ShellPage.instance.buildMenu.IsOpen = false);
			}
		}

		public City() { type = typeCity; }

		public long lastUpdateTick;
			
        public Raid[] raids = Array.Empty<Raid>();


        public static City GetOrAddCity(int cid)
        {
            Assert(cid > 65536);
			return Spot.GetOrAdd(cid);
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
   
		public static bool CanVisit(int cid)
		{
			return Spot.TryGet(cid, out var s) ? s.isFriend : false;
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
                if(!ShellPage.IsWorldView())
                    JSClient.ChangeView(false);// toggle between city/region view

                NavStack.Push(cid);

        }

		// Abusing invalid jsE by returning it when we want to return null
		//  public JsonElement troopsHome => !jsE.IsValid() ? jsE : jsE.GetProperty("th");
		//  public JsonElement troopsTotal => !jsE.IsValid() ? jsE : jsE.GetProperty("tc");
		public byte[] layout; // for building
		
		public int BidFromOverlay( int id) => layout[id]!=0? layout[id] + BuildingDef.sharestringOffset : 0;
		
		public int BidFromOverlay((int x, int y) c) => BidFromOverlay(XYToId(c));
	
		public (int bid, BuildingDef bd) BFromOverlay((int x, int y) c) 
		{
			var bid = BidFromOverlay(c);
			return (bid, BuildingDef.all[bid]);
		}


		public static City[] myCitiesCache;
		public static City[] friendCitiesCache;


		public static City[] myCities
		{
			get
			{
				if(myCitiesCache == null)
				{
					// Todo: should this be Any of my cities?  Determine case by case
			
					myCitiesCache = Spot.allSpots.Values.Where(s => s.pid == Player.activeId).ToArray();
				}
				return myCitiesCache;
			}
		}
		public static City[] friendCities
		{
			get
			{
				if (friendCitiesCache == null)
				{
					// Todo: should this be Any of my cities?  Determine case by case
					friendCitiesCache = Spot.allSpots.Values.Where(s => Player.myIds.Contains(s.pid) ).ToArray();
				}
				return friendCitiesCache;
			}
		}

		public void LoadCityData(JsonElement jse)
        {
            Debug.Assert(cid == jse.GetInt("cid"));
            if (jse.TryGetProperty("citn", out var citn))
                _cityName = citn.GetString();

			type = typeCity;
			// In rare cases, this changes
			pid = jse.GetAsInt("pid");
			Assert(pid != 0);


			if (jse.TryGetProperty("ble", out var ble))
			{
				Log(ble.ToString());
			}
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
			if (jse.TryGetProperty("sts", out var sts))
			{
				var s = sts.GetString();
				if (!s.IsNullOrEmpty())
				{

					layout = new byte[citySpotCount];

					const int offset = 18;
					const int count = citySpan * citySpan;
					// translate sharestring to building ids
					// anything that is not a building gets 0
					for (int i = 0; i < count; ++i)
					{
						if(!BuildingDef.sharestringToBuldings.TryGetValue( (byte)s[i + offset], out var b))
						{
							b = 0;
						}
						layout[i] = b;
					}
				}
				else
				{
					layout = null;
				}

			}
			if (cid == City.build && jse.TryGetProperty("bq", out var bq))
			{
				int count = bq.GetArrayLength();
				buildQueue.Clear();
				for(int i=0;i<count;++i)
				{
					var js = bq[i];
					buildQueue.Add(new BuildQueueItem(
						js.GetAsByte("slvl"),
						js.GetAsByte("elvl"), 
						js.GetAsUShort("brep"), 
						js.GetAsUShort("bspot")
						));
				}
				CityView.BuildingsOrQueueChanged();
			}

			if (jse.TryGetProperty("bd", out var eBd))
			{
				if (!CityBuild.isLayout)
				{
					commandSlots = 5;
					isCastle = false;
					if (eBd.GetArrayLength() == citySpan * citySpan)
					{
						if (buildings == Emptybuildings)
							buildings = new JSON.Building[citySpan * citySpan];
						int put = 0;
						foreach (var bdi in eBd.EnumerateArray())
						{
							var bid = bdi.GetAsInt("bid");
							var bl = bdi.GetAsInt("bl");
							var bi = BuildingDef.BidToId(bid);

							if ((put == 0) && (bid == 0))
							{
								buildings[put] = new Building() { id = BuildingDef.BidToId(bidWall), bl = 0 };
							}
							else
							{
								buildings[put] = new Building() { id = bi, bl = (byte)bl };
							}
							if (bid == bidCastle)
							{
								commandSlots = (byte)((bl + 5));
								isCastle = true;
							}
							++put;
						}
					}
					else
					{
						Log("error BD bad");
					}

					if (cid == City.build)
						CityView.BuildingsOrQueueChanged();

				}
			}
			if (jse.TryGetProperty("comm", out var comm))
			{
				activeCommands = comm.GetByte();
			}

          //  troopsHome = TroopTypeCount.empty;
          //  troopsTotal = TroopTypeCount.empty;



			if (jse.TryGetProperty("tc", out var tc))
			{
				troopsTotal = tc.GetTroopTypeCount().ToArray(); ;
				_tsTotal = troopsTotal.TS();
			}

            if (jse.TryGetProperty("th", out var th))
            {
                troopsHome = th.GetTroopTypeCount().ToArray(); ;
	
				_tsHome = troopsHome.TS();

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


			lastUpdateTick = Environment.TickCount;
          
            //            if(COTG.Views.MainPage.cache.cities.Count!=0)
            // one off change
            NotifyChange();
            

            //   OnPropertyChangedUI(String.Empty);// COTG.Views.MainPage.CityChange(this);
            //            COTG.Views.MainPage.CityListUpdateAll();
        }

        

        internal static City GetBuild()
        {
            if (build!=0 && Spot.TryGet(build, out var city))
                return city;
            return null;
        }

		public async Task SaveLayout()
		{
			if (layout == null)
				return;
			
				var sb = new StringBuilder("[ShareString.1.3]");
				sb.Append(isOnWater? ';' : ':');
				foreach(var c in layout)
				{
					if(!BuildingDef.buildingsToSharestring.TryGetValue(c, out var o) )
					{
						o = (byte)'-';
					}
					sb.Append((char)o);
				}
				sb.Append("[/ShareString]");
				var post = $"cid={cid}&a=" + System.Web.HttpUtility.UrlEncode(sb.ToString(), Encoding.UTF8);
				var rv = await Post.SendForOkay("/includes/pSs.php",post, World.CidToPlayer(cid));
			Assert(rv == true);
		}
		public async void SaveBuildingsToLayout()
		{
			layout = new byte[citySpotCount];
			int put = 0;
			foreach (var b in buildings)
			{
				 var bid =  b.def.bid;
				if (bid != 0)
					bid -= BuildingDef.sharestringOffset;
				layout[put++] = (byte)bid;
			}
			await SaveLayout();
		}

		public void LayoutToBuildings()
		{
			for (int s = 1; s < citySpotCount; ++s)
			{
				var bid = BidFromOverlay(s);
				if (bid != 0)
				{
					buildings[s].id = BuildingDef.BidToId(bid);
					if (buildings[s].isRes)
						buildings[s].bl = 0;
					else
						buildings[s].bl = 10;
				}
				else
				{
					buildings[s].bl = 0;
					buildings[s].id = 0;
				}
			}
		}
		public void FlipLayoutH()
		{
			if (layout == null)
				return;
			for(int y=0;y< citySpan;++y)
				for(int x=0;x< citySpan/2;++x)
				{
					if(isOnWater)
					{
						if (y > citySpan / 2)
							continue;
					}
					var x1 = citySpan - x - 1;

					AUtil.Swap(ref layout[x + y * citySpan], ref layout[x1 + y * citySpan]);
				}

			SaveLayout();
		}

		public void FlipLayoutV()
		{
			if (layout == null)
				return;
			for (int x = 0; x < citySpan; ++x)
				for (int y = 0; y < citySpan / 2; ++y)
				{
					if (isOnWater)
					{
						if (x > citySpan / 2)
							continue;
					}
					var y1 = citySpan - y - 1;

					AUtil.Swap(ref layout[x + y * citySpan], ref layout[x + y1 * citySpan]);
				}

			SaveLayout();
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
            App.DispatchOnUIThreadSneakyLow(() =>
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
        //public static City Factory(int cid)
        //{
        //    var rv = new City() { cid = cid, pid=Player.myId };
        //    allSpots[cid]= rv;
        //    return rv;
        //}

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
			// Todo:  Do overviews use secsessionid?

            try
            {
                var a = await Post.SendForJson("overview/senfind.php", "a=0");
                var empty = Array.Empty<SenatorInfo>();
                var changed = new HashSet<City>();
                foreach (var city in City.myCities)
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
                    if (!City.TryGet(cid, out var city))
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
		public ResetableCollection<Dungeon> dungeons
		{
			get
			{
				return Dungeon.raidDungeons;
			}
		}
		public string dungeonsToggle => MainPage.expandedCity==this ? "-" : "+";


		internal static void CitiesChanged()
		{
			friendCitiesCache = null;
			myCitiesCache = null;
		}

		public static (int max, int count) CountBuildings(int cid, Span<BuildQueueItem> buildQueue)
		{
			var max = -1;
			var count = 0;

			foreach (var bi in GetOrAddCity(cid).buildings)
			{
				if (bi.id == 0 || bi.bl == 0)
					continue;
				var bd = bi.def;
				if (bd.isTower || bd.isWall)
				{
					continue;
				}
				if (bd.isTownHall)
				{
					max = bi.bl * 10;
					continue;
				}
				++count;
			}
			// process queue for new and deleted buildings
			foreach (var r in buildQueue)
			{
				if (r.bid == 0)
					continue;

				if (r.isDemo)
				{
					var bd = BuildingDef.all[r.bid];
					if (!(bd.isWall || bd.isTower || r.isRes))
					{
						--count;
					}

				}
				else if (r.slvl == 0)
				{
					var bd = BuildingDef.all[r.bid];
					if (!(bd.isWall || bd.isTower))
					{
						++count;
					}
				}

			}
			return (max, count);

		}


		public static (int max, int count) CountBuildings()
		{
				var max = -1;
			var count = 0;
			foreach (var bi in CityBuild.postQueueBuildings)
				{
					if (bi.id == 0 || bi.bl == 0)
						continue;
					var bd = bi.def;
					if (bd.isTower || bd.isWall)
					{
						continue;
					}
					if (bd.isTownHall)
					{
						max = bi.bl * 10;
						continue;
					}
					++count;
				}
			return (max, count);
		}
		//	Span<BuildQueueItem> queue = stackalloc  BuildQueueItem[256];

		//	return CountBuildings( City.build, queue)
		//	var max = -1;
		//	var count = 0;

		//	foreach (var bi in GetBuild().buildings)
		//	{
		//		if (bi.id == 0 || bi.bl == 0)
		//			continue;
		//		var bd = bi.def;
		//		if (bd.isTower || bd.isWall)
		//		{
		//			continue;
		//		}
		//		if (bd.isTownHall)
		//		{
		//			max = bi.bl * 10;
		//			continue;
		//		}
		//		++count;
		//	}
		//	// process queue for new and deleted buildings
		//	foreach(var r in IterateQueue())
		//	{
		//		if (r.bid == 0)
		//			continue;

		//		if( r.isDemo )
		//		{
		//			var bd = BuildingDef.all[r.bid];
		//			if (!(bd.isWall || bd.isTower || r.isRes))
		//			{
		//				--count;
		//			}

		//		}
		//		else  if (r.slvl==0)
		//		{
		//			var bd = BuildingDef.all[r.bid];
		//			if (!(bd.isWall || bd.isTower))
		//			{
		//				++count;
		//			}
		//		}

		//	}
		//	return (max, count);

		//}
		public static void SyncCityBox()
		{
			App.DispatchOnUIThreadLow(() =>
			{
				var _build = City.GetBuild();
				if (_build != ShellPage.instance.cityBox.SelectedItem)
				{
					ShellPage.instance.cityBox.SelectedItem = _build;
				}
			});
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
        public Windows.UI.Xaml.Media.ImageBrush brush { get; set; }
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

			foreach (var c in all)
				if (name.Contains(c.name,StringComparison.OrdinalIgnoreCase)|| c.name.Contains(name, StringComparison.OrdinalIgnoreCase))
					return c;
			return null;
		}
		public class GroupDef
		{
		
			public string name;
			public string[] tags; // OR tags

			public GroupDef(string name, string[] tags)
			{
				this.name = name;
				this.tags = tags;
			}
		}

		public static GroupDef gdHubs = new GroupDef("Hubs", new[] { "hub" });
		public static GroupDef gdShipper = new GroupDef("Shipper", new[]{"shipping","shipper"} );
		public static GroupDef gdWarship = new GroupDef("Warships", new[] { "warship" });
		public static GroupDef gdGalley = new GroupDef("Galleys", new[] { "galley" });
		public static GroupDef gdOffense = new GroupDef("Offense", new[] { "vanq", "sorc", "horse","druid", "scorp","warship" });
		public static GroupDef gdDefense = new GroupDef("Defense", new[] { "rt","r/t", "vt","v/t", "vrt","v/r/t", "ranger","triari","priest","prae","arb","ballista" });

//		public static string[] perContinentTags = { "rt", "vanq", "priest", "prae","sorc","horse","druid","arb","scorp" };
//        public static string[] globalTags = { "navy","warship", "shipp", "stinger","galley" };
        public static CityList GetForContinent(int id) => GetOrAdd(id.ToString());
        public static CityList GetForContinentAndTag(int id,string tag) => GetOrAdd($"{id.ToString()} {tag}");

        public static CityList GetOrAdd(string name)
        {
            var cl = CityList.Find(name);
            if (cl == null)
            {
              
                cl = new CityList(name);
				var id = CityList.all.Length;
				CityList.all = CityList.all.ArrayAppend(cl);
				while(id > 1)
				{
					if (CityList.all[id].name.CompareTo(CityList.all[id - 1].name) >= 0)
						break;
					AUtil.Swap(ref CityList.all[id], ref CityList.all[id - 1]);
					--id;
				}
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
					l = City.myCities;
				}
				else
				{
					var cityList = selectedCityList;// CityList.Find(selectedCityList);
					var filtered = new List<City>();
					foreach (var cid in cityList.cities)
					{
						if (City.TryGet(cid, out var c))
						{
							filtered.Add(c);
						}
					}
					l = filtered;
				}
				l = l.OrderBy((a) => a._cityName).ToArray();
				ShellPage.instance.cityBox.ItemsSource = l;
				SyncCityBox();
				var reserveCartsFilter = DonationTab.reserveCarts;
				if (DonationTab.IsVisible())
					DonationTab.instance.donationGrid.ItemsSource = l.Where((city) => city.cartsHome >= reserveCartsFilter)
						.OrderByDescending(a => a.cartsHome).ToArray();
				//   if (MainPage.IsVisible())
				City.gridCitySource.Set(l);
				City.GetBuild().SelectInUI(true);
			});
        }

		
	}

}
