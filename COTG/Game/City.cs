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
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Enumerables;
using COTG.Draw;
using System.Threading;

namespace COTG.Game
{
	//struct Building
	//{
	//	public byte type;
	//	public byte level;
	//	// bu:  time to upgrade
	//	// bd:  time to demo?
	//}

	public sealed partial class City : Spot
	{

		public const short bidTownHall = 455;
		public const short bidWall = 809;
		public const short bidTemple = 890;
		public const short bidForester = 448;
		public const short bidCottage = 446;
		public const short bidStorehouse = 464;
		public const short bidQuarry = 461;
		public const short bidHideaway = 479;
		public const short bidFarmhouse = 447;
		public const short bidCityguardhouse = 504;
		public const short bidBarracks = 445;
		public const short bidMine = 465;
		public const short bidTrainingGround = 483;
		public const short bidMarketplace = 449;
		public const short bidTownhouse = 481;
		public const short bidSawmill = 460;
		public const short bidStable = 466;
		public const short bidStonemason = 462;
		public const short bidSorcTower = 500;
		public const short bidWindmill = 463;



		public const short bidAcademy = 482;
		public const short bidSmelter = 477;
		public const short bidBlacksmith = 502;
		public const short bidCastle = 467;
		public const short bidPort = 488;
		public const short bidShipyard = 491;
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
		public const short bidResStart = 451;
		public const short bidResEnd = 454;
		public const int bspotWall = 0;
		public const int bspotTownHall = span1 * citySpan + span1;

		public static int XYToId((int x, int y) xy) => (xy.x.Clamp(span0, span1) - span0) + (xy.y.Clamp(span0, span1) - span0) * citySpan;
	
		public static (int x, int y) IdToXY(int id)
		{
			var y = id / citySpan;
			return (id - y * citySpan + span0, y + span0);
		}

		public const int span1 = (citySpan - 1) / 2; // inclusive
		public const int span0 = -span1; // inclusive

		//static string[] buildingNames = { "forester", "cottage", "storehouse", "quarry", "hideaway", "farmhouse", "cityguardhouse", "barracks", "mine", "trainingground", "marketplace", "townhouse", "sawmill", "stable", "stonemason", "mage_tower", "windmill", "temple", "smelter", "blacksmith", "castle", "port", "port", "port", "shipyard", "shipyard", "shipyard", "townhall", "castle" };
		//const short bidTownHall = 455;
		//static short[] bidMap = new short[] { 448, 446, 464, 461, 479, 447, 504, 445, 465, 483, 449, 481, 460, 466, 462, 500, 463, 482, 477, 502, 467, 488, 489, 490, 491, 496, 498, bidTownHall, 467 };
		public const int maxBuildings = 100;
		public const int citySpan = 21;
		public const int citySpotCount = citySpan * citySpan;
		public const int cityScratchSpot = citySpotCount - 1;
		public static Building[] Emptybuildings = new Building[citySpotCount];

		public bool buildingsLoaded => buildings != Emptybuildings;
		public Building[] buildings = Emptybuildings;
		public string ministerOptions;
		public static Building[] buildingsCache;
		public static Building[] postQueuebuildingsCache = new JSON.Building[citySpotCount];

	//	public string buildStage => GetBuildStageNoFetch().ToString();

		//public Building GetBuiding((int x, int y) xy) => buildings[XYToId(xy)];
		//	public Building GetBuiding( int bspot) => buildings[bspot];

		public static DArray<BuildQueueItem> buildQueue = new DArray<BuildQueueItem>(128);// fixed size to improve threading behaviour and performance
		public static bool buildQInSync;
		public static ManualResetEventSlim buildQUpdated = new();
		public const int buildQMax = 16; // this should depend on ministers
		public static bool buildQueueFull => buildQueue.count >= buildQMax && !SettingsPage.extendedBuild;
		public static bool wantBuildCommands => buildQueue.count < safeBuildQueueLength;
		public const int safeBuildQueueLength = 15; // leave space for autobuild

		public static IEnumerable<BuildQueueItem> IterateQueue()
		{
			foreach (var i in buildQueue)
				yield return i;

			if (CityBuildQueue.all.TryGetValue(City.build, out var q))
			{
				var count = q.queue.count;
				var data = q.queue.v;

				for (int i = 0; i < count; ++i)
				{
					//semi safe
					yield return data[i];
				}
			}
		}
		public static int GetBuildQueueLength()
		{
			var rv = buildQueue.count;

			if (CityBuildQueue.all.TryGetValue(City.build, out var q))
			{
				rv += q.queue.count;
			}
			return rv;
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

		public static void IterateQueue(Action<BuildQueueItem> action)
		{
			foreach (var i in buildQueue)
				action(i);

			if (CityBuildQueue.all.TryGetValue(City.build, out var q))
			{
				var count = q.queue.count;
				var data = q.queue.v;

				for (int i = 0; i < count; ++i)
				{
					//semi safe
					action(data[i]);
				}
			}
		}
		public static void CitySwitched()
		{
			AUtil.UnsafeCopy(CityView.baseAnimationOffsets, CityView.animationOffsets);
			buildQueue.Clear();
			buildQInSync = false;
			Draw.CityView.ClearSelectedBuilding();
			CityBuild.ClearAction();
			CityView.BuildingsOrQueueChanged();

			//if (CityBuild.menuOpen)
			//{
			//	App.DispatchOnUIThreadSneakyLow(() =>
			//   ShellPage.instance.buildMenu.IsOpen = false);
			//}
		}

		public City() { type = typeCity; }

		public long lastUpdateTick;

		public Raid[] raids = Array.Empty<Raid>();


		public static City GetOrAddCity(int cid)
		{
			//Assert(cid > 65536);
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
			foreach (var r in raids)
			{
				if (r.isReturning)
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
			get
			{
				// Todo: water
				var _carryCapacity = 0;
				foreach (var tc in troopsHome)
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
					atk += tc.count * ttAttack[tc.type] * ttCombatBonus[tc.type];
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
				if (!IsRaider(tc.type) || !SettingsPage.includeRaiders[tc.type])
					continue;
				if (IsWaterRaider(tc.type) == forWater)
				{
					_carryCapacity += tc.count * ttCarry[tc.type] * Raiding.troopFraction;
				}


			}
			return _carryCapacity;
		}

		internal static City Get(int cid) => GetOrAddCity(cid);

		public string raidReturn
		{
			get
			{
				if (raids.IsNullOrEmpty())
					return "---"; // no raids
				var dt = (float)(raids[0].time - JSClient.ServerTime()).TotalMinutes; // should we check more than one
				if (raids[0].isReturning)
					return dt.ToString("0.00");
				else
					return $"({(1.0f).Max(raids[0].GetOneWayTripTimeMinutes(this) - dt):N2})";
			}
		}

		public async void SelectInWorldView(bool lazyMove)
		{
			if (!isBuild)
			{
				await JSClient.ChangeCity(cid, lazyMove); // keep current view, switch to city

			}
			if (!ShellPage.IsWorldView())
				JSClient.ChangeView(ShellPage.ViewMode.world);// toggle between city/region view

			NavStack.Push(cid);

		}

		// Abusing invalid jsE by returning it when we want to return null
		//  public JsonElement troopsHome => !jsE.IsValid() ? jsE : jsE.GetProperty("th");
		//  public JsonElement troopsTotal => !jsE.IsValid() ? jsE : jsE.GetProperty("tc");

		public string shareString; // for building

		public int BidFromOverlay(int id)
		{
			if (shareString==null || shareString.Length <= shareStringStartOffset)
				return 0;
			if (id == bspotTownHall)
				return bidTownHall;
			var t = shareString[id + shareStringStartOffset];
			if (BuildingDef.sharestringToBuldings.TryGetValue((byte)t, out var c) && c != 0)
			{

				return (int)c + BuildingDef.sharestringOffset;
			}
			else
			{
				return 0;
			}

		}

		public int FirstBuildingInOverlay(int bid)
		{
			if (shareString.IsNullOrEmpty())
				return 0;
			for (int i = 0; i < City.citySpotCount; ++i)
			{
				var bO = BidFromOverlay(i);
				if (bid == bO)
					return i;
			}
			return 0;
		}
		public int CountBuildingsInOverlay(int bid)
		{
			if (shareString.IsNullOrEmpty())
				return 0;
			int rv = 0;
			for (int i = 0; i < City.citySpotCount; ++i)
			{
				var bO = BidFromOverlay(i);
				if (bid == bO)
					++rv;
			}
			return rv;
		}
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
				if (myCitiesCache == null)
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
					friendCitiesCache = Spot.allSpots.Values.Where(s => Player.myIds.Contains(s.pid)).ToArray();
				}
				return friendCitiesCache;
			}
		}
		public async Task<int> GetAutobuildCabinLevel()
		{
			var split = await CitySettings.GetMinisterOptions(this,false);
			if (int.TryParse(split[52].Trim(']'), out var rv))
				return rv;
			return SettingsPage.cottageLevel;
		}

		public int GetAutobuildCabinLevelNoFetch()
		{
			var split = CitySettings.GetMinisterOptionsNoFetch(this);
			if (int.TryParse(split[52].Trim(']'), out var rv))
				return rv;
			return SettingsPage.cottageLevel;
		}

		public void LoadCityData(JsonElement jse)
		{
			if(cid != jse.GetInt("cid"))
			{
				Note.Show($"City bad? {nameMarkdown}");
				return;
			}
			if (jse.TryGetProperty("citn", out var citn))
				_cityName = citn.GetString();

			type = typeCity;
			// In rare cases, this changes
			pid = jse.GetAsInt("pid");
			Assert(pid != 0);


			//if (jse.TryGetProperty("ble", out var ble))
			//{
			//	Log(ble.ToString());
			//}
			if (jse.TryGetProperty("w", out var isOnWataer))
			{
				var i = isOnWataer.GetAsInt();
				if (i == 1)
					isOnWater = true;
				else if (i == 0)
					isOnWater = false;
			}
			if(jse.TryGetProperty("coof", out var coof))
			{
				ministersOn = coof.GetAsInt() != 0;
			}
			if (jse.TryGetProperty("mo", out var p))
			{
				ministerOptions = p.ToString();
			}

			if (jse.TryGetProperty("cn", out var cn))
			{
				remarks = cn[0].GetAsString();
				notes = cn[1].GetAsString();

			}
			if (jse.TryGetProperty("sts", out var sts))
			{
				var s = sts.GetString();
				Log(s);
				s = s.Replace("&#34;", "\"");
				//	Log(s);
				if (!CityBuild.isPlanner)
				{

					SetShareString(s);
				}

			}
			if (tradeInfo != CityTradeInfo.invalid)
			{
				if (jse.TryGetProperty("incRes", out var ir))
				{
					tradeInfo.inc[0] = ir[0].GetAsInt();
					tradeInfo.inc[1] = ir[1].GetAsInt();
					tradeInfo.inc[2] = ir[2].GetAsInt();
					tradeInfo.inc[3] = ir[3].GetAsInt();
					//if (NearRes.IsVisible())
					//{
					//	App.DispatchOnUIThreadSneaky( ()=> NearRes.instance.Refresh() );
					//}
				}
			}
			if (cid == City.build && jse.TryGetProperty("bq", out var bq))
			{
				int count = bq.GetArrayLength();
				buildQueue.Clear();
				for (int i = 0; i < count; ++i)
				{
					var js = bq[i];
					buildQueue.Add(new BuildQueueItem(
						js.GetAsByte("slvl"),
						js.GetAsByte("elvl"),
						js.GetAsUShort("brep"),
						js.GetAsUShort("bspot")
						));
				}
				buildQInSync = true;
				CityView.BuildingsOrQueueChanged();
			}

			if (jse.TryGetProperty("bd", out var eBd))
			{

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
								buildings[put] = new Building() { id = BuildingDef.BidToId(bidWall), bl = (byte)bl };
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
				troopsHome = th.GetTroopTypeCount().ToArray();
				_tsHome = troopsHome.TS();



			}
			if (jse.TryGetProperty("trintr", out var trintr))
			{
				var l = new List<Reinforcement>();
				foreach (var rein in trintr.EnumerateArray())
				{
					if (rein.ValueKind == JsonValueKind.Object)
					{
						var re = new Reinforcement();
						re.targetCid = cid;
						re.sourceCid = rein.GetAsInt("c");
						re.order = rein.GetAsInt64("o");
						re.troops = rein.GetProperty("tr").GetTroopTypeCount2().ToArray();
						l.Add(re);
					}
				}
				reinforcementsIn = l.ToArray();
			}
			//if (jse.TryGetProperty("trin", out var trin))
			//{
			//    Log(trin);

			//}
			//if (jse.TryGetProperty("triin", out var triin))
			//{
			////    Log(triin);

			//}


			lastUpdateTick = Environment.TickCount;

			//            if(COTG.Views.MainPage.cache.cities.Count!=0)
			// one off change
			NotifyChange();


			//   OnPropertyChangedUI(String.Empty);// COTG.Views.MainPage.CityChange(this);
			//            COTG.Views.MainPage.CityListUpdateAll();
		}

		public void SetShareString(string s)
		{
			;
			if (s != null && s.Length >= citySpotCount)
			{
				// TODO:  What if we are in planner mode?
				shareString = s;

			}
			else
			{
				shareString = null;
			}
			if (CityBuild.isPlanner)
				ShareStringToBuildingsCache();
		}

		public void SetMinistersOn(bool on)
		{
			ministersOn = on;
			Post.Send("includes/coOO.php", $"a={cid}&b={(on ? 1 : 0)}");
		}
		internal static City GetBuild()
		{
			if (build != 0 && Spot.TryGet(build, out var city))
				return city;
			return null;
		}
		public const string shareStringStart = "[ShareString.1.3]";

		public static string BuildingsToShareString(Building[] _layout, bool _isOnWater)
		{
			if (_layout == null)
				return string.Empty;

			var sb = new StringBuilder(shareStringStart);
			sb.Append(_isOnWater ? ';' : ':');
			var anyValid = false;
			int id = 0;
			foreach (var c in _layout)
			{
				byte o;
				if (id++ == bspotTownHall)
				{
					o = (byte)'T';
				}
				else
				{
					var bid = c.bid;
					if (bid != 0)
					{
						bid -= BuildingDef.sharestringOffset;
						anyValid = true;
					}
					if (!BuildingDef.buildingsToSharestring.TryGetValue((byte)bid, out  o))
					{
						o = (byte)'-';
					}
				}
				sb.Append((char)o);
				
			}
			sb.Append("[/ShareString]");
			return anyValid ? sb.ToString() : string.Empty;
		}
		public const int shareStringStartOffset = 17 + 1;
		public const int minShareStringLength = shareStringStartOffset + City.citySpotCount;

		public (string ss, string json) splitShareString => ShareString.SplitShareString(shareString);
		public async Task SaveLayout()
		{
			Note.Show("Saved layout");
			var post = $"cid={cid}&a=" + System.Web.HttpUtility.UrlEncode(shareString ?? string.Empty, Encoding.UTF8);
			var rv = await Post.SendForOkay("/includes/pSs.php", post, World.CidToPlayerOrMe(cid));
			Assert(rv == true);
		}

		public void BuildingsCacheToShareString()
		{

			shareString = BuildingsToShareString(buildingsCache, isOnWater) + splitShareString.json;
			//			await SaveLayout();
		}


		public void ShareStringToBuildingsCache()
		{
			buildingsCache = new Building[citySpotCount];
			if (!isLayoutValid)
			{
				for (int s = 0; s < citySpotCount; ++s)
				{
					buildingsCache[s].id = 0;
					buildingsCache[s].bl = 0;
				}
			}
			else
			{
				for (int s = 1; s < citySpotCount; ++s)
				{
					var bid = BidFromOverlay(s);
					if (bid != 0)
					{
						buildingsCache[s].id = BuildingDef.BidToId(bid);
						if (buildingsCache[s].isRes)
							buildingsCache[s].bl = 0;
						else
							buildingsCache[s].bl = 10;
					}
					else
					{
						buildingsCache[s].bl = 0;
						buildingsCache[s].id = 0;
					}
				}
			}
			PlannerTab.BuildingsChanged();

		}

		public void FlipLayoutH(bool ignoreWater=false)
		{
			//	if (layout == null)
			//		return;
			var water = isOnWater && !ignoreWater;
			Assert(CityBuild.isPlanner);
			for (int y = span0; y <= span1; ++y)
			{
				for (int x = span0; x < 0; ++x)
				{
					if (water)
					{
						if (y >= 0 && !(x, y).IsXYInCenter())
							continue;
					}
					var x1 = -x;

					AUtil.Swap(ref buildingsCache[XYToId((x, y))], ref buildingsCache[XYToId((x1, y))]);
				}
			}
			PlannerTab.BuildingsChanged();
			// SaveLayout();
		}

		public void FlipLayoutV(bool ignoreWater=false)
		{
			//if (layout == null)
			//	return;
			Assert(CityBuild.isPlanner);
			var water = isOnWater && !ignoreWater;
			for (int y = span0; y <0; ++y)
			{
				for (int x = span0; x <= span1; ++x)
				{
					if (water)
					{
						if (x >= 0 && !(x, y).IsXYInCenter())
							continue;
					}
					var y1 = -y;

					AUtil.Swap(ref buildingsCache[XYToId((x, y))], ref buildingsCache[XYToId((x, y1))]);
				}
			}
			PlannerTab.BuildingsChanged();
			///SaveLayout();
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
		{
			get
			{
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
						SenatorInfo.Type.recruit => "recr",
						SenatorInfo.Type.settle => "setl",
						SenatorInfo.Type.siege => "seig",
						_ => "Error"
					};
					rv += $"{type}:{s.count}";

				}
				return rv;

			}
		}


		public string GetSenatorInfo()
		{
			StringBuilder sb = new StringBuilder();
			var idle = 0;
			var settle = 0;
			var siege = 0;
			var recruiting = 0;

			foreach (var sen in senatorInfo)
			{
				if (sen.type == SenatorInfo.Type.idle)
					idle += sen.count;
				else if (sen.type == SenatorInfo.Type.recruit)
					recruiting += sen.count;
				else if (sen.type == SenatorInfo.Type.settle)
					settle += sen.count;
				else if (sen.type == SenatorInfo.Type.siege)
					siege += sen.count;
				else
					Assert(false);
			}
			if (idle > 0)
				sb.Append($"\n{idle} idle senators");
			if (settle > 0)
				sb.Append($"\n{settle} senators settling");
			if (siege > 0)
				sb.Append($"\n{siege} senators sieging");
			if (recruiting > 0)
				sb.Append($"\n{recruiting} senators recruiting");
			return sb.ToString();

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
			return nameAndRemarks;// $"{{{nameof(cityName)}={cityName}, {nameof(xy)}={xy}, {nameof(cid)}={cid},{nameof(tsTotal)}={tsTotal.ToString()}, {nameof(tsHome)}={tsHome.ToString()}}}";
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

				if (a.RootElement.ValueKind == JsonValueKind.Object)
				{

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
				if (!IsRaider(type) || !SettingsPage.includeRaiders[type])
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
				if (type > (byte)DungeonType.water)
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
		public ushort carts;
		public ushort Carts => carts;
		public ushort cartsHome;
		public ushort CartsHome => cartsHome;
		public int wood;
		public int Wood => wood;
		public int woodStorage { get; set; }
		public int stone;
		public int Stone => stone;
		public int stoneStorage { get; set; }
		public int iron;
		public int ironStorage { get; set; }
		public int food;
		public int foodStorage { get; set; }
		public ushort ships;
		public ushort Ships => ships;
		public ushort shipsHome;
		public ushort ShipsHome => shipsHome;
		public bool academy { get; set; }
		public bool sorcTower { get; set; }
		public bool ministersOn { get; set; }

		public Resources res => new Resources(wood, stone, iron, food);
		public static DumbCollection<City> gridCitySource = new DumbCollection<City>();
		public static City[] emptyCitySource = Array.Empty<City>();
		internal CityTradeInfo tradeInfo = CityTradeInfo.invalid;
		public CityTradeInfo GetTradeInfo()
		{
			if (tradeInfo == CityTradeInfo.invalid)
			{
				tradeInfo = new CityTradeInfo();
			}
			return tradeInfo;
		}
		internal bool isLayoutValid => shareString != null && shareString.Length >= minShareStringLength;
		public bool ComputeCartTravelTime(int target, out TimeSpan t)
		{
			var onDifferentContinent = cont != target.CidToContinent();
			t = TimeSpan.Zero;

			var targetC = Spot.GetOrAdd(target);
			if (onDifferentContinent)
			{
				return false;
			}
			else
			{
				var dist = cid.DistanceToCid(target);
				t = TimeSpan.FromMinutes(dist * Enum.cartTravel);
				return true;

			}
		}
		public bool ComputeShipTravelTime(int target, out TimeSpan t)
		{
			t = TimeSpan.Zero;

			var targetC = Spot.GetOrAdd(target);
			if (!targetC.isOnWater || !isOnWater)
				return false;


			var dist = cid.DistanceToCid(target);
			t = TimeSpan.FromMinutes(dist * Enum.shipTravel + 60);

			return true;

		}
		public bool ComputeTravelTime(int target, bool onlyHome, out float hours, out bool onDifferentContinent)
		{
			hours = 0;
			onDifferentContinent = cont != target.CidToContinent();
			if (onDifferentContinent)
			{
				if (Spot.GetOrAdd(target).isOnWater && (onlyHome ? troopsHome : troopsTotal).Any((t) => t.type == ttGalley || t.type == ttStinger))
				{
					var tt = ttGalley;
					var dist = cid.DistanceToCid(target);
					hours = dist * ttTravel[tt] / (60f * ttSpeedBonus[tt]) + 1;
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
		
		public string dungeonsToggle =>  "+";

		public bool isBuilding { get; internal set; }

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


		public static (int max, int count, int townHallLevel) GetBuildingCountAndTownHallLevel()
		{

			return (CityBuild.postQueueTownHallLevel * 10, CityBuild.postQueueBuildingCount, CityBuild.postQueueTownHallLevel);
		}


		// this is the actual building, ignores planner buildings
		public Building GetBuildingPostQueue(int bspot, in DArray<BuildQueueItem> q)
		{
			var rv = buildings[bspot];
			if (q != null)
			{
				foreach (var i in q)
				{
					if (i.bspot == bspot)
					{
						rv.bl = i.elvl;
						if (i.elvl == 0)
							rv.id = 0;
						else
							rv.id = BuildingDef.BidToId(i.bid);
					}
				}
			}
			return rv;
		}
		public static void GetPostQueue(ref Building rv, int bspot, in BuildQueueItem[] q, int qSize)
		{
			for (int i = 0; i < qSize; ++i)
			{
				if (q[i].bspot == bspot)
				{
					rv.bl = q[i].elvl;
					if (q[i].elvl == 0)
						rv.id = 0;
					else
						rv.id = BuildingDef.BidToId(q[i].bid);
				}
			}
		}

		public (int max, int count) CountBuildingsWithoutQueue()
		{
			var max = -1;
			var count = 0;
			foreach (var bi in buildings)
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

		public enum BuildStage
		{
			_new,
			noLayout,
			setup,
			cabins,
			townHall,
			cabinsDone,
			mainBuildings,
			preTeardown,
			teardown,
			complete,
			pending,
		}
		public struct BuildInfo
		{
			public BuildStage stage;
			public int buildingLimit;

			public BuildInfo(BuildStage stage, int buildingLimit)
			{
				this.stage = stage;
				this.buildingLimit = buildingLimit;
			}
		}

		public struct BuildingCount
		{
			public int storeHouses;
			public int cabins;
			public int townHallLevel;
			public int buildings;
			public int sorcTowers;
			public int sorcTowerLevel;
			public int academies;
			public int training;
			public int stables;
			public int blacksmiths;
			public int shipyards;
			public int sawMills;
			public int stoneMasons;
			public int windMills;
			public int smelters;
			public int barracks;
			public int forums;
			public int unfinishedBuildings;
			public int unfinishedCabins;
			public bool hasCastle;
			public int wallLevel;
			public bool hasWall=>wallLevel > 0;
			public int scoutpostCount;
			public int unfinishedTowerCount;
			internal int towerCount;
			public int maxBuildings => townHallLevel * 10;

			public int GetMainMilitaryBid()
			{
				var max = training.Max(academies).Max(sorcTowers).Max(stables.Max(blacksmiths) );
				if (max == training)
					return  bidTrainingGround;
				else if (max == academies)
					return  bidAcademy;
				else if (max == sorcTowers)
					return  bidSorcTower;
				else if (max == stables)
					return  bidStable;
				else // if(max == bc.blacksmiths)
					return bidBlacksmith;

			}
		}
		public static BuildingCount GetBuildingCountPostQueue(int cabinLevel) => GetBuildingCounts(CityBuild.postQueueBuildings, cabinLevel);

	
		public  int GetBuildingLimit(BuildingCount bc)
		{
			(bool hasCastleInLayout, bool hasSorcTowerInLayout) = hasCastleOrSorcTowerInLayout;
			var rv = 100;
			if (hasCastleInLayout && !bc.hasCastle)
				rv -= 1;
			if (bc.sorcTowers == 0 && hasSorcTowerInLayout)
				rv -= 1;
			return rv;

		}

		public static BuildingCount GetBuildingCounts(Building[] buildings, int cabinLevel)
		{
			BuildingCount rv = new();

			foreach (var bd in buildings)
			{
				// not res or empty
				if (!bd.isBuilding)
					continue;
				var bdef = bd.def;
				var bid = bd.bid;
				if (bid == bidCastle)
					rv.hasCastle = true;
				if (bdef.isTownHall)
				{
					rv.townHallLevel = bd.bl;
				}

				else if (bdef.isWall)
				{
					rv.wallLevel = bd.bl;
				}
				else if (bdef.isTower )
				{
					++rv.towerCount;
					if (bdef.isScoutpost)
					{
						++rv.scoutpostCount;

					}
					if (bd.bl < SettingsPage.autoTowerLevel && SettingsPage.autoTowerLevel != 10)
					{
						++rv.unfinishedTowerCount;
					}
				}
				else
				{
					++rv.buildings;
					if (bdef.isCabin)
					{
						++rv.cabins;
						if (bd.bl < cabinLevel )
							++rv.unfinishedCabins;
					}
					else
					{
						if (bd.bl < 10)
							++rv.unfinishedBuildings;
					}
				}
				if (bd.bid == bidStorehouse)
					++rv.storeHouses;


				if (bid == bidSorcTower)
				{
					++rv.sorcTowers;
					if( bd.bl > rv.sorcTowerLevel)
						rv.sorcTowerLevel = bd.bl; /// any is fine
				}
				else if (bid == bidAcademy)
					++rv.academies;
				else if (bid == bidSawmill)
					++rv.sawMills;
				else if (bid == bidWindmill)
					++rv.windMills;
				else if (bid == bidStonemason)
					++rv.stoneMasons;
				else if (bid == bidSmelter)
					++rv.smelters;
				else if (bid == bidBarracks)
					++rv.barracks;
				else if (bid == bidMarketplace)
					++rv.forums;
				else if (bid == bidTrainingGround)
					++rv.training;
				else if (bid == bidStable)
					++rv.stables;
				else if (bid == bidShipyard)
					++rv.shipyards;
				else if (bid == bidBlacksmith)
					++rv.blacksmiths;
			}

			//Log($"{rv.cabins} cabins, {rv.buildings} {rv.townHallLevel}");
			return rv;
		}
		public static BuildingCount GetBuildingCountsPostQueue(int cabinLevel) => GetBuildingCounts(CityBuild.postQueueBuildings, cabinLevel);
		public BuildingCount GetBuildingCounts(int cabinLevel) => GetBuildingCounts(buildings, cabinLevel);
		public BuildingCount GetBuildingCountsNoFetch() => GetBuildingCounts(buildings, GetAutobuildCabinLevelNoFetch() );

		public string buildingStr
		{
			get
			{
				var bc = GetBuildingCountsNoFetch();
				return $"{bc.buildings}/{bc.maxBuildings} c:{bc.cabins} a:{bc.academies!=0} s:{bc.sorcTowers!=0} u:{bc.unfinishedBuildings}";
			}
		}

		public static void AllCityDataDirty()
		{
			App.DispatchOnUIThreadLow(() =>
		   {
			   foreach(var i in City.gridCitySource)
			   {
				   i.OnPropertyChanged(string.Empty);
			   }
			   City.gridCitySource.NotifyReset();
		   });
		}

		public BuildInfo GetBuildStage(BuildingCount bc)
		{
			var buildingLimit = GetBuildingLimit(bc);
			if (CityRename.IsNew(this))
				return new BuildInfo(BuildStage._new,buildingLimit);
			if (!isLayoutValid)
				return new BuildInfo(BuildStage.noLayout, buildingLimit);


			if (bc.buildings < 8)
				return new BuildInfo(BuildStage.setup, buildingLimit);

			if ( bc.unfinishedCabins > 0)
				return new BuildInfo(BuildStage.cabins, buildingLimit);
			else if(bc.townHallLevel < 10)
			{
				return new BuildInfo(BuildStage.townHall, buildingLimit);
			}
			if (bc.cabins +4 >= bc.buildings )
			{
				return new BuildInfo(BuildStage.cabinsDone,buildingLimit);
			}


			var underLimit = bc.buildings < buildingLimit;
			if (bc.cabins >= SettingsPage.startCabinCount || underLimit )
			{
				if (bc.unfinishedBuildings > 0 || underLimit)
					return new BuildInfo(BuildStage.mainBuildings, buildingLimit);
				else
					return new BuildInfo(BuildStage.preTeardown, buildingLimit);
			}

			if (bc.cabins > 0 || bc.buildings < 100 || !IsLayoutComplete() )
				return new BuildInfo(BuildStage.teardown,buildingLimit);

			return new BuildInfo(BuildStage.complete,buildingLimit);



		}
	

	}
	public static class CityHelpers
	{
		public static bool IsXYInCenter(this (int x, int y) xy) => (xy.x.Abs() <= 4 && xy.y.Abs() <= 4 && (xy.x.Abs() + xy.y.Abs() < 7));

		public static bool IsInCity(this (int x, int y) xy) => xy.x >= span0 && xy.x <= span1 && xy.y >= span0 && xy.y <= span1;

		public static ShellPage.ViewMode GetNext(this ShellPage.ViewMode mode)
		{
			return mode switch
			{
				ShellPage.ViewMode.city => ShellPage.ViewMode.region,
				_ => ShellPage.ViewMode.city
			};
		}
		public static ShellPage.ViewMode GetNextUnowned(this ShellPage.ViewMode mode)
		{
			return mode switch
			{
				ShellPage.ViewMode.region => ShellPage.ViewMode.world,
				_ => ShellPage.ViewMode.region
			};
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
		public bool isUnassigned => id == 0;
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
			{
				if (string.Equals(c.name, name, StringComparison.OrdinalIgnoreCase))
					return c;
			}
			//foreach (var c in all)
			//	if (name.Contains(c.name,StringComparison.OrdinalIgnoreCase)|| c.name.Contains(name, StringComparison.OrdinalIgnoreCase))
			//		return c;
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

		public static GroupDef gdLeaveMe = new GroupDef("LeaveME", new[] { "leaveme" });
		public static GroupDef gdHubs = new GroupDef("Hubs", new[] { "hub" });
		public static GroupDef gdStorage = new GroupDef("Storage", new[] { "storage" });
		public static GroupDef gdShipper = new GroupDef("Shipper", new[] { "shipping", "shipper" });
		public static GroupDef gdWarship = new GroupDef("Warships", new[] { "warship" });
		public static GroupDef gdGalley = new GroupDef("Galleys", new[] { "galley" });
		public static GroupDef gdStinger = new GroupDef("Stingers", new[] { "stinger" });
		public static GroupDef gdOffense = new GroupDef("Offense", new[] { "vanq", "sorc", "horse", "druid", "scorp", "warship" });
		public static GroupDef gdDefense = new GroupDef("Defense", new[] { "rt", "r/t", "vt", "v/t", "vrt", "v/r/t", "ranger", "triari", "priest", "prae", "arb", "ballista", "stinger" });

		//		public static string[] perContinentTags = { "rt", "vanq", "priest", "prae","sorc","horse","druid","arb","scorp" };
		//        public static string[] globalTags = { "navy","warship", "shipp", "stinger","galley" };
		public static CityList GetForContinent(int id, HashSet<CityList> processed) => GetOrAdd(id.ToString(), processed);
		public static CityList GetForContinentAndTag(int id, string tag, HashSet<CityList> processed) => GetOrAdd($"{id.ToString()} {tag}", processed);

		public static CityList GetOrAdd(string name, HashSet<CityList> processed)
		{
			var cl = CityList.Find(name);
			if (cl != null && processed.Add(cl))
			{
				cl.cities.Clear();
			}
			if (cl == null)
			{

				cl = new CityList(name);
				var id = CityList.all.Length;
				CityList.all = CityList.all.ArrayAppend(cl);
				while (id > 1)
				{
					if (CityList.all[id].name.CompareTo(CityList.all[id - 1].name) >= 0)
						break;
					AUtil.Swap(ref CityList.all[id], ref CityList.all[id - 1]);
					--id;
				}
				processed.Add(cl); ;
			}
			return cl;
		}


		public static CityList FindNewCities() => Find(sNewCities);

		public static CityList allCities = new CityList() { id = -1, name = "All" }; // special item for ui selection
		public static CityList[] all = Array.Empty<CityList>();
		public static CityList[] selections = new[] { allCities }; // Similar to the above array, but a dummy "All" entry (id=-1) at the start for Combo Boxes
		internal const string sNewCities = "NewCities";
		struct CityComparer : IComparer<City>
		{
			public int Compare(City x, City y)
			{
				var rv = string.Compare(x._cityName, y._cityName, true);
				if (rv != 0)
					return rv;
				var xHasRemarks = (x.remarks != null);
				var yHasRemarks = (y.remarks != null);
				if (!xHasRemarks && !yHasRemarks)
					return 0;
				else if (!xHasRemarks)
					return 1;
				else if (!yHasRemarks)
					return -1;
				else
					return string.Compare(x.remarks, y.remarks, true); ;
			}
		}

		public static ComboBox box => ShellPage.instance.cityListBox;
		public static void SelectedChange()
		{

			App.DispatchOnUIThreadLow(() =>
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
			   l = l.OrderBy((a) => a, new CityComparer()).ToArray();
			   ShellPage.instance.cityBox.ItemsSource = l;
			   SyncCityBox();
			   var reserveCartsFilter = DonationTab.reserveCarts;
			   if (DonationTab.IsVisible())
				   DonationTab.instance.donationGrid.ItemsSource = l.Where((city) => city.cartsHome >= reserveCartsFilter)
					   .OrderByDescending(a => a.cartsHome).ToArray();
				//   if (MainPage.IsVisible())
				City.gridCitySource.Set(l);
			   City.GetBuild().ScrollMeIntoView();
		   });
		}


	}

}
