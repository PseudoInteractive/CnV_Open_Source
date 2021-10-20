using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using static COTG.Debug;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using COTG.Helpers;
using System.Text.Json;
using static COTG.Game.Troops;
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
using EnumsNET;
using static COTG.Game.TroopTypeCountHelper;
using static Cysharp.Text.ZString;
using Nito.AsyncEx;
using static COTG.BuildingDef;
using System.Buffers;

namespace COTG.Game
{
	//struct Building
	//{
	//	public byte type;
	//	public byte level;
	//	// bu:  time to upgrade
	//	// bd:  time to demo?
	//}

	public sealed partial class City : Spot, IANotifyPropertyChanged
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

		public const int bidMin = bidBarracks;
		public const int bidMax = bidTemple + 1;
		public AttackPlanCity plan => AttackPlan.GetForRead(cid);
		public AttackPlanCity planWritable => AttackPlan.Get(cid);
		public int attackCluster => plan.attackCluster;
		public AttackType attackType
		{
			get => plan.attackType;
			set => planWritable.attackType = value; // Todo:  Throw exception if not present
		}
		//public void CallPropertyChanged(string members = null)
		//{
		//	PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(members));
		//}

		public byte TroopType
		{
			get
			{
				TouchClassification();
				// Does not wait
				var rv = classificationTTs[(int)classification];
				//	Assert(planWritable == null || planWritable.troopType == rv || planWritable.troopType == 0 || (rv == ttPending));
				return rv;
			}
			set
			{
				var troopType = value;
				TryConvertTroopTypeToClassification(troopType, out classification);
				if (!isMine)
				{
					tags = TagHelper.FromTroopType(troopType, tags);

				}
				var plan = planWritable;
				if (plan != null)
					plan.troopType = troopType;

			}
		}
		public async Task<byte> TroopTypeAsync() => classificationTTs[(int)(await Classify())];

		/// <summary>
		/// Evaluates based on troops in cities if known
		/// Otherwise evaluates based on classification 
		/// </summary>
		/// <param name="onlyHomeTroops"></param>
		/// <param name="includeWater"></param>
		/// <returns></returns>
		//public byte GetPrimaryTroopType(bool onlyHomeTroops = false, bool includeWater = true)
		//{
		//	Assert(isMine);
		//	var troops = (onlyHomeTroops ? troopsHome : troopsTotal);
		//	if (!troops.Any())
		//		return (byte)TroopType;

		//	byte best = 0; // based on clasification or guards
		//	var bestTS = 0;
		//	foreach (var ttc in troops.Enumerate())
		//	{
		//		var type = ttc.type;
		//		if (IsTTNaval(type) && !includeWater)
		//			continue;
		//		var ts = ttc.ts;
		//		if (ts > bestTS)
		//		{
		//			bestTS = ts;
		//			best = (byte)type;
		//		}

		//	}
		//	if (best == 0)
		//		return (byte)TroopType;
		//	else
		//		return best;
		//}
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
		//	private const int buildingCount = citySpan * citySpan;
		public const int citySpotCount = citySpan * citySpan;
		public const int cityScratchSpot = citySpotCount - 1;
		public static Building[] Emptybuildings = new Building[citySpotCount];

		public bool buildingsLoaded => !Object.ReferenceEquals(buildings, Emptybuildings);
		public Building[] buildings = Emptybuildings;
		//public static Building[] buildingsCache;
		//	public static Building[] postQueuebuildingsCache = new JSON.Building[citySpotCount];

		//	public string buildStage => GetBuildStageNoFetch().ToString();

		//public static ArrayPool<Building> buildingCache = ArrayPool<Building>.Create(citySpotCount, citySpotCount);

		//Building[] BuildingsFromLayout()
		//{
		//	var rv = buildingCache.Rent(citySpotCount);
		//	for (int i = 0; i < citySpotCount; i++)
		//		rv[i] = buildings[i];
		/////	buildings = rv;
		//	return rv;


		//}


		//public Building GetBuiding((int x, int y) xy) => buildings[XYToId(xy)];
		//	public Building GetBuiding( int bspot) => buildings[bspot];
		public Building[] postQueuebuildingsCache = Emptybuildings;
		public DArray<BuildQueueItem> buildQueue = DArray<BuildQueueItem>.Rent(18);// fixed size to improve threading behaviour and performance
																				 //public static ManualResetEventSlim buildQUpdated = new();
		public const int buildQMax = 16; // this should depend on ministers
										 //	public static bool buildQueueFull => buildQueue.count >= buildQMax && !SettingsPage.extendedBuild;
										 //	public static bool wantBuildCommands => buildQueue.count < safeBuildQueueLength;
		public const int safeBuildQueueLength = 15; // leave space for autobuild

		public IEnumerable<BuildQueueItem> IterateQueue()
		{
			foreach (var i in buildQueue) // COTG Queue
				yield return i;

			if (ExtendedQueue.all.TryGetValue(cid, out var q))
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
		public int GetBuildQueueLength()
		{
			var rv = buildQueue.count;

			if (ExtendedQueue.all.TryGetValue(cid, out var q))
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
		//public static Span<BuildQueueItem> GetExtQueue(int cid)
		//{

		//	if (CityBuildQueue.all.TryGetValue(cid, out var q))
		//	{
		//		return new Span<BuildQueueItem>(q.queue.ToArray());
		//	}
		//	return new Span<BuildQueueItem>();


		//}
		//public static Span<BuildQueueItem> GetQueue()
		//{

		//	return GetQueue(City.build, City.buildQueue.span, GetExtQueue(build));


		//}

		public void IterateQueue(Action<BuildQueueItem> action)
		{
			foreach (var i in buildQueue)
				action(i);

			if (ExtendedQueue.all.TryGetValue(cid, out var q))
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
		public void SetAsBuildCity()
		{
			build = cid;
			AUtil.UnsafeCopy(CityView.baseAnimationOffsets, CityView.animationOffsets);
			//buildQueue.ClearKeepBuffer();
			//buildQInSync = false;
			Draw.CityView.ClearSelectedBuilding();
			//	CityBuild.ClearAction();
			//BuildingsOrQueueChanged();
			OnPropertyChanged();
			//if (CityBuild.menuOpen)
			//{
			//	App.DispatchOnUIThreadSneakyLow(() =>
			//   ShellPage.instance.buildMenu.IsOpen = false);
			//}
		}



		//		public long lastUpdateTick;

		public Raid[] raids = Array.Empty<Raid>();

		public float constructionSpeed { get; set; }

		public static City GetOrAddCity(int cid)
		{
			//Assert(cid > 65536);
			return Spot.GetOrAdd(cid);
		}

		public bool MightRaidsRepeat()
		{
			foreach (var r in raids)
			{
				if (r.isRepeatingOrScheduledToReturn)
					return true;
			}
			return false;
		}



		public DateTimeOffset GetRaidReturnTime()
		{
			return raids.Any() ? raids.Max(a => a.GetReturnTime(this)) : JSClient.ServerTime();
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
				return troopsHome.Sum(tc => tc.count * ttCarry[tc.type]);

			}
		}
		public float homeTroopsAttack
		{
			get
			{
				// Todo: water
				return troopsHome.Sum(tc => tc.attack);
			}
		}
		public static float CarryCapacity(in TroopTypeCounts tt, bool forWater)
		{
			return Raiding.GetTroops(tt, forWater, !forWater).Sum(tc => tc.count * ttCarry[tc.type]);

		}

		public float CarryCapacityHome(bool forWater)
		{
			return CarryCapacity(troopsHome, forWater);
		}
		public float CarryCapacityIncludeAway(bool forWater)
		{
			return CarryCapacity(troopsTotal, forWater);
		}
		internal static City Get(int cid) => GetOrAddCity(cid);

		public string raidReturn
		{
			get
			{
				if (raids.IsNullOrEmpty())
					return "---"; // no raids
				var postFix = raids[0].r4 switch { Raid.scheduled => "@", Raid.repeating => "+", _ => "~" };
				var t = raids.Max(a => a.GetReturnTime(this)); // should we check more than one
				return t.Format() + (raids.Any(a => !a.isReturning) ? " >" : " <") + postFix;
			}
		}

		public async void SelectInWorldView(bool lazyMove)
		{
			if (!isBuild)
			{
				if (!await JSClient.CitySwitch(cid, lazyMove)) // keep current view, switch to city 
					return;
			}
			if (!ShellPage.IsWorldView())
				JSClient.ChangeView(ShellPage.ViewMode.world);// toggle between city/region view

			NavStack.Push(cid);

		}

		// Abusing invalid jsE by returning it when we want to return null
		//  public JsonElement troopsHome => !jsE.IsValid() ? jsE : jsE.GetProperty("th");
		//  public JsonElement troopsTotal => !jsE.IsValid() ? jsE : jsE.GetProperty("tc");
		public static byte[] emptyLayout = NewLayout(); // new byte[citySpotCount];
		public static int emptyLayoutHashCode = emptyLayout.GetHashCode();

		public static string emptyLayoutString = ShareStringWithoutJson(NewLayout(),false); 
		public byte[] layout = emptyLayout;
		public static byte[] NewLayout()
		{
			var rv = new byte[citySpotCount];
			for(int i = 0;i<citySpotCount;++i)
			{
				rv[i] = i==bspotTownHall ? (byte)'T' : (byte)'-';

			}
			return rv;
		}
		internal bool isLayoutCustom => !object.ReferenceEquals(layout,emptyLayout);
		internal bool isLayoutEmpty => object.ReferenceEquals(layout,emptyLayout);

		// can have side effects
		public byte[] TouchLayoutForWrite()
		{
			if(isLayoutEmpty)
			{
				layout = NewLayout(); // we need a writable blank one
				shareStringLoaded=true;
			}
			return layout;
		}

		// can have side effects
		public byte[] layoutWritable => TouchLayoutForWrite();
		//public static string[] NewString()
		//{
		//	return ShareStringWithoutJson(ShareStringWithoutJson,isOnWater);
		//	var rv = new byte[citySpotCount];
		//	rv[bspotTownHall] = (bidTownHall - sharestringBuildingOffset).AsByte();
		//	return rv;
		//}
		public string shareStringJson = string.Empty;
		public string shareStringWithoutJson => ShareStringWithoutJson(layout,isOnWater);
		public static string ShareStringWithoutJson(byte [] layout, bool isOnWater) => $"[ShareString.1.3]{(isOnWater ? ";" : ":")}{Encoding.ASCII.GetString(layout)}";
		public string shareString
		{
			get => shareStringWithoutJson + shareStringJson;
		//	set => SetShareString(value);
		}

		//public byte[] plannerOverlay =null; // for building
		//public string shareString
		//{
		//	get
		//	{
		//		if (shareString == null)
		//			return string.Empty;
		//		return Encoding.ASCII.GetString(plannerOverlay);

		//	}
		//}

		public int GetLayoutBid(int id)
		{
			if (id == bspotTownHall)
				return bidTownHall;
			return BuildingDef.LayoutToBid(layout[id]);
		}
		public Building GetLayoutBuilding(int id)
		{
			var bid = GetLayoutBid(id);
			return new Building(BidToId(bid), IsBidRes(bid)||(bid==0) ? (byte)0 : (byte)10 );
		}
		//public int LayoutToBid(byte v) => BuildingDef.LayoutToBid(v);
		//public void SetBuildingInOverlay( (int x,int y) xy, int bid  )
		//{
		//	if (!isLayoutValid)
		//		return;
		//	shareString[ XYToId(xy) + shareStringStartOffset].bid
		//	var t = shareString[id + shareStringStartOffset];
		//	if (BuildingDef.sharestringToBuldings.TryGetValue((byte)t, out var c) && c != 0)
		//	{

		//		return (int)c + BuildingDef.sharestringOffset;
		//	}
		//	else
		//	{
		//		return 0;
		//	}

		//}
		//public void SetBuildingInOverlay(int id)
		//{
		//	var t = layout[id + shareStringStartOffset];

		//}

		public int FirstBuildingInOverlay(int bid)
		{
			if (!isLayoutCustom)
				return 0;
			for (int i = 0; i < City.citySpotCount; ++i)
			{
				var bO = GetLayoutBid(i);
				if (bid == bO)
					return i;
			}
			return 0;
		}
		public int CountBuildingsInOverlay(int bid)
		{
			if (layout.IsNullOrEmpty())
				return 0;
			int rv = 0;
			for (int i = 0; i < City.citySpotCount; ++i)
			{
				var bO = GetLayoutBid(i);
				if (bid == bO)
					++rv;
			}
			return rv;
		}
		public int GetLayoutBid((int x, int y) c) => GetLayoutBid(XYToId(c));

		//public (int bid, BuildingDef bd) BFromOverlay((int x, int y) c)
		//{
		//	var bid = GetLayoutBid(c);
		//	return (bid, BuildingDef.all[bid]);
		//}

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

		public int GetAutobuildCabinLevelNoFetch() => autobuildCabinLevel;

		public void LoadCityData(JsonElement jse)
		{

			try
			{
				if(cid != jse.GetInt("cid"))
				{
					Note.Show($"City bad? {nameMarkdown}");
					return;
				}
				if(jse.TryGetProperty("citn",out var citn))
					_cityName = citn.GetString();

				type = typeCity;
				// In rare cases, this changes
				pid = jse.GetAsInt("pid");
				Assert(pid != 0);


				//if (jse.TryGetProperty("ble", out var ble))
				//{
				//	Log(ble.ToString());
				//}
				if(jse.TryGetProperty("w",out var isOnWataer))
				{
					var i = isOnWataer.GetAsInt();
					if(i == 1)
						isOnWater = true;
					else if(i == 0)
						isOnWater = false;
				}

				if(jse.TryGetProperty("r",out var r))
				{
					if(r.TryGetProperty("1",out var w))
					{
						wood = w.GetAsFloat("r").RoundToInt();
					}
					if(r.TryGetProperty("2",out var s))
					{
						stone = s.GetAsFloat("r").RoundToInt();
					}
					if(r.TryGetProperty("3",out var _i))
					{
						iron = _i.GetAsFloat("r").RoundToInt();
					}
					if(r.TryGetProperty("4",out var f))
					{
						food = f.GetAsFloat("r").RoundToInt();
					}
				}
				// carts?
				if(jse.TryGetProperty("crth",out var crth))
				{
					cartsHome = crth.GetAsUShort();
				}
				if(jse.TryGetProperty("crt",out var crt))
				{
					carts = crt.GetAsUShort();
				}
				if(jse.TryGetProperty("shph",out var shph))
				{
					shipsHome = crth.GetAsUShort();
				}
				if(jse.TryGetProperty("shp",out var shp))
				{
					ships = shp.GetAsUShort();
				}
				if(jse.TryGetProperty("coof",out var coof))
				{
					ministersOn = coof.GetAsInt() != 0;
				}
				if(jse.TryGetProperty("mo",out var p))
				{
					SetMinisterOptions(p.ToString());
				}

				if(jse.TryGetProperty("cn",out var cn))
				{
					remarks = cn[0].GetAsString();
					notes = cn[1].GetAsString();
					this.UpdateTags();

				}
				if(jse.TryGetProperty("OGA",out var oga))
				{
					try
					{
						foreach(var attack in oga.EnumerateArray())
						{
							int id = attack[0].GetAsInt();
							if(id == 5) // Raid
								continue;
							if(id == 4) // reinforce
								continue;
							Trace($"{id} {attack[1].GetAsString()} {attack[5].GetAsString()} {attack[6].GetAsString()}");
						}

					}
					catch(Exception ex)
					{
						LogEx(ex,eventName: "OGA");
					}

				}
				if(jse.TryGetProperty("cs",out var cs))
				{
					constructionSpeed = cs.GetAsFloat();
				}
				if(!shareStringLoaded)
				{
					if(jse.TryGetProperty("sts",out var sts))
					{

						var s = sts.GetString();
						//	Log(s);
						s = s.Replace("&#34;","\"");
						//	Log(s);

						SetShareString(s,false);
					}

				}

				if(tradeInfo != CityTradeInfo.invalid)
				{
					if(jse.TryGetProperty("incRes",out var ir))
					{
						tradeInfo.inc[0] = ir[0].GetAsInt();
						tradeInfo.inc[1] = ir[1].GetAsInt();
						tradeInfo.inc[2] = ir[2].GetAsInt();
						tradeInfo.inc[3] = ir[3].GetAsInt();
						//if (NearRes.IsVisible())
						//{
						//	App.DispatchOnUIThreadLow( ()=> NearRes.instance.Refresh() );
						//}
					}
				}
				//	if ((this != ExtendedQueue.cityQueueInUse))
				{
					if(jse.TryGetProperty("bq",out var bq))
					{

						//	Log($"BQ: {(cid == City.build)} {nameAndRemarks} {bq.ToString()}");
						//if (cid == cityQueueInUse)
						//{
						if(bq.ValueKind == JsonValueKind.Array)
						{
							var count = bq.GetArrayLength();
							var prior = buildQueue.v;
							var priorLength = buildQueue.Length;
							var changedSize = count!=priorLength;
							if(changedSize)
							{
								Assert(count < buildQueue.v.Length);
								buildQueue.Resize(count);
							}
							int put = 0;
							var anyChanged = false;
							foreach(var js in bq.EnumerateArray())
							{
								var n = new BuildQueueItem(
									js.GetAsByte("slvl"),
									js.GetAsByte("elvl"),
									js.GetAsShort("brep"),
									js.GetAsShort("bspot"),
									buildTime: ((long)(js.GetAsInt("btime")+500) / 1000).ClampToU16(),
									pa: js.GetAsInt64("pa") == 1);
								if(changedSize || buildQueue[put] != n)
								{
									anyChanged = true;
									buildQueue[put] = n;
								}
								++put;
							}
							if(anyChanged)
							{
								BuildingsOrQueueChanged();

							}
						}
						else
						{
							Assert(false);
						}
						//}
					}
					else
					{
						var sum = buildQueue.Sum(a => a.buildTime);
						//Assert(sum <= 32 );
						Trace($"No Queue {nameMarkdown} was {buildQueue.Length} dt: {sum}s");
						//	buildQueue.Clear();
					}
				}
				//else
				//{
				//	Trace($"Skipped fetch bq {nameMarkdown}");
				//}


				TryGetBuildings(jse);
				if(jse.TryGetProperty("comm",out var comm))
				{
					activeCommands = comm.GetByte();
				}

				//  troopsHome = TroopTypeCount.empty;
				//  troopsTotal = TroopTypeCount.empty;



				if(jse.TryGetProperty("tc",out var tc))
				{
					Set(ref troopsTotal,tc);
					_tsTotal = troopsTotal.TS();
				}

				if(jse.TryGetProperty("th",out var th))
				{
					Set(ref troopsHome,th);
					_tsHome = troopsHome.TS();



				}
				if(jse.TryGetProperty("trintr",out var trintr))
				{
					if(trintr.ValueKind == JsonValueKind.Array && trintr.GetArrayLength() > 0)
					{
						var l = new List<Reinforcement>();

						foreach(var rein in trintr.EnumerateArray())
						{
							if(rein.ValueKind == JsonValueKind.Object)
							{
								var re = new Reinforcement();
								re.targetCid = cid;
								re.sourceCid = rein.GetAsInt("c");
								re.order = rein.GetAsInt64("o");
								Set2(ref re.troops,rein.GetProperty("tr"));
								l.Add(re);
							}
							else
							{
								Assert(rein.ValueKind == JsonValueKind.Array && rein.GetArrayLength() == 0);
							}
						}
						reinforcementsIn = l.ToArray();
					}

				}


				//lastUpdateTick = Environment.TickCount;

				//            if(COTG.Views.MainPage.cache.cities.Count!=0)
				// one off change
				NotifyChange();
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}


			//   OnPropertyChangedUI(String.Empty);// COTG.Views.MainPage.CityChange(this);
			//            COTG.Views.MainPage.CityListUpdateAll();
		}

		public bool TryGetBuildings(JsonElement jse)
		{
			if (jse.TryGetProperty("bd", out var eBd))
			{

				{
					commandSlots = 5;
					isCastle = false;
					var anyChanged = false;
					if (eBd.ValueKind == JsonValueKind.Array && eBd.GetArrayLength() == citySpotCount)
					{
						if(object.ReferenceEquals(buildings , Emptybuildings) 	)
							buildings = new Building[citySpotCount];
						int put = 0;
						foreach (var bdi in eBd.EnumerateArray())
						{
							var bid = bdi.GetAsInt("bid");
							var bl = bdi.GetAsByte("bl");
							var bi = BuildingDef.BidToId(bid);

							if ((put == 0) && (bid == 0))
							{
								buildings[put] = new Building(BuildingDef.BidToId(bidWall), (byte)bl);
							}
							else
							{
								if(buildings[put].id != bi || buildings[put].bl != bl)
								{
									anyChanged=true;
									buildings[put] = new Building(bi, bl);
								}
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
					
					BuildingsOrQueueChanged();
					
					if(anyChanged)
					{
						UpdateBuildStage();
						OnPropertyChanged();
					}
				}
				return true;
			}
			return false;
		}
		static int GetShareStringHash(string _layout, string _shareStringJson)
		{

			return (_layout.Length > shareStringStartOffset+1 ?  _layout.Substring(shareStringStartOffset).GetHashCode() - shareStringBaseHash : 0) +
					(_shareStringJson.Length > 3 ?  _shareStringJson.GetHashCode() : 0);
		}

		static int shareStringBaseHash = emptyLayoutString.Substring(shareStringStartOffset).GetHashCode();

		int shareStringSavedHash;
		bool shareStringLoaded; // this is only allowed to load once, or 0 times if switched to

		public Task<bool> SetShareString(string s, bool save=true)
		{
			(var _layout, var _shareStringJson) = ShareString.SplitShareString(s);
			return SetShareString(_layout,_shareStringJson,save);
		}
		
		public Task<bool> SetShareString(string _layout,string _shareStringJson,bool save = true)
		{
			shareStringLoaded=true;
			Assert(emptyLayout.GetHashCode() == emptyLayoutHashCode);

			//	(var _layout,var _shareStringJson) = ShareString.SplitShareString(s);
			Assert(_shareStringJson!=null);
		//	Assert(_layout.Length == City.citySpotCount+shareStringStartOffset);

			try
			{

				var hash = GetShareStringHash(_layout,_shareStringJson);

				if(hash == shareStringSavedHash)
					return AUtil.completedTaskTrue;

				shareStringJson = _shareStringJson;

				if(_layout.Length >= citySpotCount + shareStringStartOffset)
				{
				//	if(_layout.Length >= shareStringStartOffset + citySpotCount)
					//Assert(_layout.Length == City.citySpotCount);
					layout = Encoding.ASCII.GetBytes(_layout,shareStringStartOffset,citySpotCount );
				}
				else
					layout = emptyLayout;
				shareStringSavedHash = hash;
				if(save)
					return SaveLayout();
				//	if (CityBuild.isPlanner)
				//		ShareStringToBuildingsCache();
				
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
			return AUtil.completedTaskTrue;
		}
		//public void SetShareStringJson(string s,bool save)
		//{

		//	(var _layout, shareStringJson) = ShareString.SplitShareString(s);
		//	if(_layout.Length >= City.citySpotCount+shareStringStartOffset)
		//	{
		//		//Assert(_layout.Length == City.citySpotCount);
		//		layout = Encoding.ASCII.GetBytes(_layout,shareStringStartOffset,citySpotCount);
		//	}
		//	else
		//		layout = emptyLayout;

		//	if(save)
		//	{
		//		shareStrigSavedHash = hash;
		//		SaveLayout();
		//	}
		//	//	if (CityBuild.isPlanner)
		//	//		ShareStringToBuildingsCache();
		//}

		public Task SetMinistersOnAsync(bool on)
		{
			ministersOn = on;
			return Post.Get("includes/coOO.php", $"a={cid}&b={(on ? 1 : 0)}");
		}
		public void SetMinistersOn(bool on) => SetMinistersOnAsync(on);

		internal static City GetBuild()
		{
			if (build != 0 && Spot.TryGet(build, out var city))
				return city;
			return null;
		}
		public const string shareStringStart = "[ShareString.1.3]";

	


		public static string BuildingsToShareString(Building[] _layout, bool _isOnWater)
		{
			//	if (!City.GetBuild().isLayoutValid)
			//			return string.Empty;

			using var sb = CreateUtf8StringBuilder();
			sb.Append(shareStringStart);
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
					var x = BuildingDef.BidToLayout(c.bid);
					anyValid |= x.valid;
					o = x.c;


				}
				sb.Append((char)o);

			}
			//sb.Append("[/ShareString]");
			return anyValid ? sb.ToString() : string.Empty;
		}

		public void SetLayoutFromBuildings(Building[] bds, bool allowCabins)
		{
			// copy buildings to buildingCache
			var _layout = NewLayout();
			//var bds = b.buildings;
			//var rv = new Building[citySpotCount];
			for(int i = 0;i < citySpotCount;++i)
			{
				var bd = bds[i];
				if(bd.requiresBuildingSlot && (allowCabins||!bd.isCabin) )
				{
					_layout[i] = BidToLayout(bd.bid).c;
				}

			}
			this.layout = _layout;
			PlannerTab.BuildingsChanged(this);

		}

		public static string LayoutToShareString(byte[] _layout,bool _isOnWater)
		{
			//	if (!City.GetBuild().isLayoutValid)
			//			return string.Empty;

			using var sb = CreateUtf8StringBuilder();
			sb.Append(shareStringStart);
			sb.Append(_isOnWater ? ';' : ':');
			var anyValid = false;
			//int id = 0;
			for(var i =0;i<citySpotCount;++i)
			{
				byte o;
				if(i == bspotTownHall)
				{
					o = (byte)'T';
				}
				else
				{
					var bid = BuildingDef.LayoutToBid( _layout[i] ); 
					var x = BuildingDef.BidToLayout(bid);
					anyValid |= x.valid;
					o = x.c;


				}
				sb.Append((char)o);

			}
			//sb.Append("[/ShareString]");
			return anyValid ? sb.ToString() : string.Empty;
		}
		public Task<bool> SaveShareStringFromLayout()
		{
			return SetShareString( LayoutToShareString(layout,isOnWater), shareStringJson );
		}


		public const int shareStringStartOffset = 17 + 1;
		public const int minShareStringLength = shareStringStartOffset + City.citySpotCount;

		//	public (string ss, string json) splitShareString => ShareString.SplitShareString(layout);

		public  Task<bool> SaveLayout()
		{
		//	Note.Show("Saved layout");
			var post = $"cid={cid}&a=" + System.Web.HttpUtility.UrlEncode(shareString, Encoding.UTF8);
			return Post.SendForOkay("/includes/pSs.php", post, World.CidToPlayerOrMe(cid));
		//	Assert(rv == true);
		}

		


	

		

		public	 Building[] GetLayoutBuildings()
		{
			var rv = new Building[citySpotCount];
			for (int s = 0; s < citySpotCount; ++s)
			{
					rv[s]=GetLayoutBuilding(s);
			
			}
			return rv;
		}

		//public void ShareStringToBuildingsCache()
		//{
		//	buildingsCache = ShareStringToBuildingsCacheI();
		//	PlannerTab.BuildingsChanged();

		//}

		public void FlipLayoutH(bool notifyChange=false,bool ignoreWater=false)
		{
			//	if (layout == null)
			//		return;
			var water = isOnWater && !ignoreWater;
			//Assert(CityBuild.isPlanner);
			TouchLayoutForWrite();
			Assert(isLayoutCustom);
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

					AUtil.Swap(ref layout[XYToId((x, y))], ref layout[XYToId((x1, y))]);
				}
			}
			if(notifyChange)
				PlannerTab.BuildingsChanged(this);
			// SaveLayout();
		}

		public void FlipLayoutV( bool notifyChange=false,bool ignoreWater=false)
		{
			//if (layout == null)
			//	return;
		//	Assert(CityBuild.isPlanner);
			TouchLayoutForWrite(); 
			var water = isOnWater && !ignoreWater;
			for (int y = span0; y < 0; ++y)
			{
				for (int x = span0; x <= span1; ++x)
				{
					if (water)
					{
						if (x >= 0 && !(x, y).IsXYInCenter())
							continue;
					}
					var y1 = -y;

					AUtil.Swap(ref layout[XYToId((x, y))], ref layout[XYToId((x, y1))]);
				}
			}
			if(notifyChange)
				PlannerTab.BuildingsChanged(this);
			///SaveLayout();
		}



		//  static List<City> dummies = new List<City>();


		public void NotifyChange(string member = null)
		{
		   OnPropertyChanged(member);
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
		public string senForAttack {
			get
			{
				if (!isMine)
					return "?";
				var t = troopsTotal.FirstOrDefault((a) => a.type == ttSenator);
				if (t.count !=0 )
					return t.ts.ToString();
				foreach (var s in senatorInfo)
				{
					if (s.type == SenatorInfo.Type.recruit)
						return "+";
				}
				return "0";

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
		//public void OnPropertyChangedUI(string propertyName) => App.(()=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));

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
					if (a.RootElement.TryGetProperty("b", out var b) && b.ValueKind == JsonValueKind.Array)
					{
						foreach (var cit in b.EnumerateArray())
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
							foreach (var siege in cit[9].EnumerateArray())
							{
								sens.Add(new SenatorInfo()
								{
									type = SenatorInfo.Type.siege,
									count = siege[3].GetAsByte(),
									target = siege[0].GetInt32(),
									time = siege[1].GetString().ParseDateTime(false)
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
					}

					changed.NotifyChange(nameof(City.senny));
				}
			}
			catch (Exception ex)
			{
				LogEx(ex);
			}



		}

	
		

		//public static bool operator ==(City left, City right)
		//{
		//	return EqualityComparer<City>.Default.Equals(left, right);
		//}

		//public static bool operator !=(City left, City right)
		//{
		//	return !(left == right);
		//}


		private string GetDebuggerDisplay()
		{
			return ToString();
		}

		//public byte GetRaidTroopType()
		//{
		//	byte best = 0; // if no raiding troops we return guards 
		//	var bestTS = 0;
		//	foreach (var ttc in troopsHome)
		//	{
		//		var type = ttc.type;
		//		if (!IsRaider(type) || !SettingsPage.includeRaiders[type])
		//			continue;
		//		var ts = ttc.ts;
		//		if (ts > bestTS)
		//		{
		//			bestTS = ts;
		//			best = (byte)type;
		//		}

		//	}
		//	return best;
		//}
		public byte GetAttackTroopType()
		{
			byte best = 0; // if no raiding troops we return guards 
			var bestAttack = 0.0f;
			foreach (var ttc in troopsHome.Enumerate() )
			{
				var type = ttc.type;

				var ts = ttc.attack;
				if (ts > bestAttack)
				{
					bestAttack = ts;
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
			foreach (var ttc in troopsHome.Enumerate())
			{
				var type = ttBestDungeonType[ttc.type];
				if (type >= (byte)DungeonType.water)
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
		public bool sorcTower { get; set; }
		public NullableBool ministersOn;
		public bool? MinistersOn { get => ministersOn; set=>ministersOn=value; }

		public Resources res => new Resources(wood, stone, iron, food);
		public static NotifyCollection<City> gridCitySource = new NotifyCollection<City>();
		public static AsyncLock cityGridLock = new();
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
				var dist = cid.DistanceToCidD(target);
				t = TimeSpan.FromMinutes(dist * Troops.cartTravel);
				return true;

			}
		}
		public bool ComputeShipTravelTime(int target, out TimeSpan t)
		{
			t = TimeSpan.Zero;

			var targetC = Spot.GetOrAdd(target);
			if (!targetC.isOnWater || !isOnWater)
				return false;


			var dist = cid.DistanceToCidD(target);
			t = TimeSpan.FromMinutes(dist * Troops.shipTravel + 60);

			return true;

		}
		public bool ComputeTravelTime(int target, bool onlyHome,bool includeOffense,bool incudeSenators,bool includeSE, bool viaWater,float maxHours, ref float hours)
		{
			var onDifferentContinent = cont != target.CidToContinent();
			var targetCity = City.Get(target);
			var targetOnWater = targetCity.isOnWater;
			if (!(targetOnWater && isOnWater) && viaWater)
				return false;
			if (onDifferentContinent && !viaWater)
				return false;

			var dist = cid.DistanceToCidD(target);
			bool anyValid = false; 

			foreach ( var tt in (onlyHome ? troopsHome.Enumerate() : troopsTotal.Enumerate() ) )
			{
				if (tt.isNaval != viaWater)
					continue;
				if (!includeOffense && !tt.isDef)
					continue;
				if (!incudeSenators && tt.isSenator )
					continue;
				if (!includeSE && tt.isArt)
					continue;
				var t = (float)(tt.TravelTimeMinutes(dist)/ (60.0));

				if (t <= maxHours)
				{
					anyValid = true;
					hours = hours.Max(t);
				}
			}
			return anyValid;

		}
		//static City lastDugeonScanCity;
		
		public string dungeonsToggle =>  "+";

		public bool? isBuilding { get; internal set; }
		public bool autoWalls;
		public bool autoTowers;
		public byte autobuildCabinLevel = (byte)SettingsPage.cottageLevel;
		public sbyte buildingCountCache = -1;
		public bool wantCastle;
		public bool wantSorcTower;
		public int Bldgs => buildingCountCache; // for DataGrid

		public BuildStage buildStage;
		void SetBuildStage(BuildStage _stage)
		{
			if(	buildStage != _stage )
			{
				buildStage = _stage;
				if(BuildTab.IsVisible())
					OnPropertyChanged();
			}
		}
		void SetBuildingCount(int _count)
		{
			var count = (sbyte)_count;
			if(buildingCountCache != count)
			{
				buildingCountCache = count;
				if(BuildTab.IsVisible())
					OnPropertyChanged();
			}
		}

		void SetBuildStage(BuildInfo info) => SetBuildStage(info.stage);
		
		public BuildingCount UpdateBuildStage()
		{
			var bc = GetBuildingCounts();
			SetBuildingCount(bc.buildingCount);
			SetBuildStage(GetBuildStage(bc));
			return bc;
		}
		public bool bcBuildings { get; set; }
		public bool bcTowers { get; set; }
		public bool bcBlocked { get; set; }
		public void SetMinisterOptions( string ministerOptions)
		{
			try
			{
				if (ministerOptions.Length > 4)
				{
					var rv = ministerOptions.Split(',', StringSplitOptions.RemoveEmptyEntries);
					if (rv[CitySettings.ministerOptionAutobuildCabins - 1] == "[1")
					{
						byte.TryParse(rv[CitySettings.ministerOptionAutobuildCabins].TrimEnd(']'), out autobuildCabinLevel);

					}
					else
					{
						autobuildCabinLevel = 0;
					}
					autoTowers = rv[CitySettings.ministerOptionAutobuildTowers] == "1";
					autoWalls = rv[CitySettings.ministerOptionAutobuildWalls] == "1";
				}
			}
			catch (Exception ex)
			{
				Log(ex);
			}

		}
		internal static void CitiesChanged()
		{
			friendCitiesCache = null;
			myCitiesCache = null;
		}

		//public (int max, int count) CountBuildings(int cid, Span<BuildQueueItem> buildQueue)
		//{
		//	var max = -1;
		//	var count = 0;

		//	foreach (var bi in buildings)
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
		//	foreach (var r in buildQueue)
		//	{
		//		if (r.bid == 0)
		//			continue;

		//		if (r.isDemo)
		//		{
		//			var bd = BuildingDef.all[r.bid];
		//			if (!(bd.isWall || bd.isTower || r.isRes))
		//			{
		//				--count;
		//			}

		//		}
		//		else if (r.slvl == 0)
		//		{
		//			var bd = BuildingDef.all[r.bid];
		//			if (!(bd.isWall || bd.isTower))
		//			{
		//				++count;
		//			}
		//		}
		//		else if( r.bspot == bspotTownHall)
		//		{
		//			max = r.elvl * 10;
		//		}

		//	}
		//	return (max, count);

		//}



		public Building GetBuildingPostQueue(int bspot)
		{
			return GetBuildingPostQueue(bspot,buildQueue.span,this.GetExtendedBuildQueue());
		}

		// this is the actual building, ignores planner buildings
		public Building GetBuildingPostQueue(int bspot, in Span<BuildQueueItem> q0, in Span<BuildQueueItem> q1)
		{
			var rv = buildings[bspot];
			foreach(var i in q0)
			{

				if (i.bspot == bspot)
				{
				 rv= i.Apply(rv);
				}
			}
			foreach(var i in q1)
			{

				if(i.bspot == bspot)
				{
					rv= i.Apply(rv);
				}
			}

			return rv;
		}
		public Building GetBuildingPostQueue(int bspot,in Span<BuildQueueItem> q0)
		{
			var rv = buildings[bspot];
			foreach(var i in q0)
			{

				if(i.bspot == bspot)
				{
					rv= i.Apply(rv);
				}
			}
			
			return rv;
		}
		public static Building ApplyQueue(Building rv,int bspot,in Span<BuildQueueItem> q0)
		{
			foreach(var i in q0)
			{
				rv= i.Apply(rv);
			}
			return rv;
		}

		public (int max, int count) CountBuildingsWithoutQueue()
		{
			var max = -1;
			var count = 0;
			var bds = buildings;
			foreach(var bspot in CityBuild.buildingSpots)
			{
				var bi = bds[bspot];
				if (bi.id == 0 || bi.bl == 0)
					continue;
#if DEBUG
				var bd = bi.def;
				if (bd.isTower || bd.isWall)
				{
					Assert(false);
				}
				if (bd.isTownHall)
				{
					Assert(false);
				}
#endif
				++count;
			}
			return (bds[bspotTownHall].bl*10, count);
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
			App.QueueOnUIThread(() =>
			{
				var _build = City.GetBuild();
				if (_build != ShellPage.instance.cityBox.SelectedItem)
				{
					ShellPage.instance.cityBox.SelectedItem = _build;
				}
			});
		}

		public enum BuildStage : byte
		{
			_new,
			leave,
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
			count,
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
			public int buildingCount;
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
			public int ports;
			public int unfinishedBuildings;
			public int unfinishedCabins;
			public bool hasCastle;
			public int wallLevel;
			public readonly bool hasWall =>wallLevel > 0;
			public int scoutpostCount;
			public int unfinishedTowerCount;
			internal int towerCount;
			public readonly int buildingLimit => townHallLevel * 10;

			public int GetMainMilitaryBid()
			{
				var max = training.Max(academies).Max(sorcTowers).Max(stables.Max(blacksmiths).Max(shipyards) );
				if (max == training)
					return bidTrainingGround;
				else if (max == academies)
					return bidAcademy;
				else if (max == sorcTowers)
					return bidSorcTower;
				else if (max == stables)
					return bidStable;
				else if (max == blacksmiths)
					return bidBlacksmith;
				else
					return bidShipyard;
			}
		}
	//	public static BuildingCount GetBuildingCountPostQueue(int cabinLevel) => GetBuildingCounts(CityBuild.postQueueBuildings, cabinLevel);

	
		public int GetBuildingLimit(BuildingCount bc)
		{
			(bool hasCastleInLayout, bool hasSorcTowerInLayout) = hasCastleOrSorcTowerInLayout;
			var rv = 100;
			if (hasCastleInLayout && !bc.hasCastle)
				rv -= 1;
			if (bc.sorcTowers == 0 && hasSorcTowerInLayout)
				rv -= 1;
			return rv;

		}
		public BuildingCount GetLayoutBuildingCounts() => GetBuildingCounts(GetLayoutBuildings());

		public BuildingCount GetBuildingCounts() => GetBuildingCounts(postQueueBuildings );

		public BuildingCount GetBuildingCounts(Building [] buildings, bool recurse=true )
		{
			BuildingCount rv = new();
			rv.wallLevel = buildings[0].bl;
			for(int bspot = 2;bspot<citySpotCount-2;++bspot)
			{
				var bd = buildings[bspot];
				// not res or empty
				if (!bd.isBuilding)
					continue;
				var bdef = bd.def;
				var bid = bd.bid;
				if (bid == bidCastle)
					rv.hasCastle = true;
				if (bspot == bspotTownHall)
				{
					rv.townHallLevel = bd.bl;
				}
				else if (bspot == bspotWall)
				{
					Assert(false);
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
					Assert(bd.requiresBuildingSlot);
					Assert(bd.id != 0);
					++rv.buildingCount;
					if (bdef.isCabin)
					{
						++rv.cabins;
						if (bd.bl < autobuildCabinLevel)
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
				else if (bid == bidPort)
					++rv.ports;
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
			//if(rv.buildingCount == 42)
			//{
			//	Trace("wtf");
			//}
			//if(rv.buildingCount > 100)
			//{
			//	Trace("wtf2");
			//	if(recurse)
			//	{
			//		var rv2 = GetBuildingCounts(buildings,false);
			//	}
			//}
			//Log($"{rv.cabins} cabins, {rv.buildings} {rv.townHallLevel}");
			return rv;
		}
	//	public BuildingCount GetBuildingCounts(int cabinLevel) => GetBuildingCounts(buildings, cabinLevel);
		//public BuildingCount GetBuildingCountsNoFetch() => GetBuildingCounts(buildings, GetAutobuildCabinLevelNoFetch() );

		//public string buildingStr
		//{
		//	get
		//	{
		//		var bc = GetBuildingCountsNoFetch();
		//		return $"{bc.buildings}/{bc.maxBuildings} c:{bc.cabins} a:{bc.academies!=0} s:{bc.sorcTowers!=0} u:{bc.unfinishedBuildings}";
		//	}
		//}
		public float PlanPriority
		{
			get => plan.priority;
			set => planWritable.priority = value;
		}	
		//public static void AllCityDataDirty()
		//{
		//	App.(async () =>
		//   {
		//   using (var _ = await cityGridLock.LockAsync())
		//	{ 
			   
		//		foreach (var i in City.gridCitySource)
			   
		//		   i.OnPropertyChanged(string.Empty);
		//	  }
			
		//		City.gridCitySource.NotifyReset();
		//   });
		//}

		public BuildInfo GetBuildStage(BuildingCount bc)
		{
			var leave = leaveMe;
			var buildingLimit = GetBuildingLimit(bc);
			wantCastle = !leave && NeedsCastle(bc);
			wantSorcTower = !leave && NeedsSorcTower(bc);
			if(is7Point && bc.hasCastle && bc.townHallLevel==1)
				return new BuildInfo(BuildStage.complete, buildingLimit);
			if (leave)
				return new BuildInfo(BuildStage.leave, buildingLimit);
			if (IsNew())
				return new BuildInfo(BuildStage._new,buildingLimit);
			
			if (!isLayoutCustom)
				return new BuildInfo(BuildStage.noLayout, buildingLimit);


			if (bc.buildingCount < 8)
				return new BuildInfo(BuildStage.setup, buildingLimit);

			if ( bc.unfinishedCabins > 0)
				return new BuildInfo(BuildStage.cabins, buildingLimit);
			else if(bc.townHallLevel < 10)
			{
				return new BuildInfo(BuildStage.townHall, buildingLimit);
			}
			if (bc.cabins +4 >= bc.buildingCount )
			{
				return new BuildInfo(BuildStage.cabinsDone,buildingLimit);
			}


			var underLimit = bc.buildingCount < buildingLimit;
			if (bc.cabins+2 >= SettingsPage.startCabinCount || underLimit )
			{
				if (bc.unfinishedBuildings > 0 || underLimit)
					return new BuildInfo(BuildStage.mainBuildings, buildingLimit);
				else
					return new BuildInfo(BuildStage.preTeardown, buildingLimit);
			}

			if (bc.cabins > 0 || bc.buildingCount < 100 || !IsLayoutComplete(this) )
				return new BuildInfo(BuildStage.teardown,buildingLimit);

			return new BuildInfo(BuildStage.complete,buildingLimit);
		}
		public bool IsNew()
		{
			return ShareString.IsNew(this) && points <= 60;
		}
		public struct TownHallAndBuildingCount
		{
			public int townHallLevel;
			public int buildingCount;
			public int max => townHallLevel * 10;
			public int count => buildingCount;
		}
		public TownHallAndBuildingCount GetTownHallAndBuildingCount(bool forceUpdate)
		{
			if ( (postQueueBuildingCount < 0 )||(cachedCity!=cid) )
			{
				if(!forceUpdate && (cachedCity!=cid) )
				{
					return new TownHallAndBuildingCount() { townHallLevel = -1,buildingCount = -1 };
				}
				var bd = postQueueBuildings;
				if(CityBuild.isPlanner)
				{
					// we need the above statement to refresh the cache
					// we will use them later
					bd = GetLayoutBuildings();
				}
				var count = 0;

				for(var i = 1;i < citySpotCount-1;++i)
				{
					var b = bd[i];
					if(i == bspotTownHall)
					{
					 // nothing
					}
					else if(b.bl > 0 && !b.isTower)
						++count;
				}
				postQueueBuildingCount= count;
				SetBuildingCount(count);

				postQueueTownHallLevel  = bd[bspotTownHall].bl;
			}
			return new TownHallAndBuildingCount() { townHallLevel = postQueueTownHallLevel, buildingCount = postQueueBuildingCount };
		}

		public bool AutoWalls
		{
			get => autoWalls;
			set => CitySettings.SetAutoTowersOrWalls(cid,autoWalls:value);
		}
		public bool AutoTowers
		{
			get => autoTowers;
			set => CitySettings.SetAutoTowersOrWalls(cid, autoTowers: value);
		}



	}
	public static class CityHelpers
	{
		public static int CidOr0(this City c) => c != null ? c.cid : 0;
		public static City AsCity(this int cid) => City.Get(cid);
		public static bool TestContinentFilter(this int cid) => Spot.TestContinentFilter(cid);

		public static bool IsXYInCenter(this (int x, int y) xy) => (xy.x.Abs() + xy.y.Abs() < 7);

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
		public static List<int> CityListToCids(this IList selList)
		{
			var temp = new List<int>();
			foreach (City sel in selList)
			{
				temp.Add(sel.cid);
			}

			return temp;
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
		public Microsoft.UI.Xaml.Media.ImageBrush brush { get; set; }
		public int count { get; set; }

	}
	public class CityList
	{
		public string name { get; set; }
		public int id { get; set; } // 0 is unassigned, others are pids
		public bool isUnassigned => id == 0;
		public HashSet<int> cities = new HashSet<int>(); // list of cities
	//	public static bool IsNew(City city) => CityRename.IsNew(city._cityName) && city.points <= 60;
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
				if (xHasRemarks)
				{
					if (!yHasRemarks)
						return -1;
					rv = string.Compare(x.remarks, y.remarks, true);
					if (rv != 0)
						return rv;

				}
				else if (yHasRemarks)
				{
					return 1;
				}
				return x.spatialIndex.CompareTo(y.spatialIndex);

			}
		}

		public static ComboBox box => ShellPage.instance.cityListBox;
		public static void NotifyChange()
		{

			App.QueueOnUIThread(async () =>
		   {
				//               Log("CityListChange");

				var selectedCityList = CityList.box.SelectedItem as CityList;
			   IList<City> l;
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
			   l = l.Where(a => a.testContinentAndTagFilter).OrderBy((a) => a, new CityComparer()).ToArray();
			   ShellPage.instance.cityBox.ItemsSource = l;
			   SyncCityBox();
			   var reserveCartsFilter = DonationTab.reserveCarts;
			   if (DonationTab.IsVisible())
				   DonationTab.instance.donationGrid.ItemsSource = l.Where((city) => city.cartsHome >= reserveCartsFilter)
					   .OrderBy(a=>a.cont).ToArray();
			   //   if (MainPage.IsVisible())
			   {
				   using var _ = await cityGridLock.LockAsync();
				   City.gridCitySource.Set(l,true);
				   foreach(var i in UserTab.spotGrids)
					   i.DeselectAll();
			   }
			   //if (IncomingTab.instance.isVisible)
				//   IncomingTab.instance.refresh.Go();



		   });
		}


	}

}
