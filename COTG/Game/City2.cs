using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static COTG.Views.CityBuild;
using COTG.Views;
using static COTG.BuildingDef;
using COTG;
using COTG.JSON;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.UI.Xaml.Controls;
using static COTG.Debug;
using static COTG.Draw.CityView;
using static COTG.Game.City;
using static COTG.Views.CityBuild;
using static COTG.BuildingDef;
using COTG.Views;
using COTG.Services;

namespace COTG.Game
{


	public partial class City
	{

		private Building[] postQueueBuildingCache = null;// = new Building[citySpotCount];


	//	public object postQueueBuildingLock = new();
	//	public static int cachedCity;
	//	public static int postQueueBuildingCount = -1;
	//	static int postQueueTownHallLevel = -1;
		public  void BuildingsOrQueueChanged()
		{
			postQueueBuildingCache =null;
			buildingCountCache = -1;
		}
		
		public Building[] postQueueBuildings
		{
			get {
				var rv = postQueueBuildingCache;
				if(rv is not null)
					return rv;
				//lock(postQueueBuildingLock)
				{
				//	if(cachedCity == cid)
				//		return postQueueBuildingCache;
					//if (!CityBuild.isPlanner)
					//{
					//var  buildingsCache = buildings;
					//}
					rv = new Building[citySpotCount];
					postQueueBuildingCache = rv;
				
					var buildQueue = this.buildQueue.span;

					//
					// copy current buildings
					//
					for(var i = 0;i < citySpotCount;++i)
					{
						rv[i] = buildings[i];
						rv[i].AssertValid();
					}
					if(!CityBuild.isPlanner)
					{
						//
						// Apply queue
						//
						{
							foreach(var q in buildQueue)
							{
								rv[q.bspot] = q.Apply(rv[q.bspot]);
								rv[q.bspot].AssertValid();

							}

							if(ExtendedQueue.all.TryGetValue(City.build,out var bq))
							{
								var count = bq.queue.count;
								var data = bq.queue.v;

								for(int i = 0;i < count;++i)
								{
									rv[data[i].bspot] = data[i].Apply(rv[data[i].bspot]);
									rv[data[i].bspot].AssertValid();

								}
							}



						}
					}
				//	cachedCity = cid;
				//	postQueuebuildingsCache = rv;
					return rv;
				}
			}
		}

		
		//	public int postQueueTownHallLevel => CityBuild.isPlanner switch { true => 10, _ => postQueueBuildings[bspotTownHall].bl };

		public async Task<int> AnyHub(bool requestHub)
		{
			await NearRes.UpdateTradeStuffIfNeeded();
			
			foreach(var i in requestHub ? tradeInfo.resSource : tradeInfo.resDest)
			{
				if(i !=0)
					return i;
			}
			return 0;

		}
		

		//	public int postQueueBuildingCount => postQueueBuildings.Count(c => c.requiresBuildingSlot);

		public async Task<bool> BuildWallDialogue()
		{
			var result = await App.DispatchOnUIThreadTask(async () =>
			{

				var dialog = new ContentDialog()
				{
					Title = "Want a wall?",
					Content = "Would you like to build a wall first?",
					PrimaryButtonText = "Please",
					SecondaryButtonText = "No"
				};
				return await dialog.ShowAsync2();
			});
			if (result == ContentDialogResult.Primary)
			{
				await Enqueue(0, 1, bidWall, bspotWall);
				await Task.Delay(400);

				return true;
			}
			else
			{
				return false;
			}
		}
		async Task<bool> UpgradeTownHallDialogue(int toLevel)
		{
			toLevel = toLevel.Min(10);
			var currentLevel = postQueueBuildings[bspotTownHall].bl;
			if (currentLevel >= toLevel)
				return true;
			var a = await App.DispatchOnUIThreadTask(async () =>
			{
				var dialog = new ContentDialog()
				{
					Title = "Upgrade TownHall?",
					Content = $"Would you like to upgrade your Town Hall to level {toLevel} first?",
					PrimaryButtonText = "Please",
					SecondaryButtonText = "No"
				};
				return await dialog.ShowAsync2();
			});
			if (a == ContentDialogResult.Primary)
			{

				await EnqueueUpgrade(toLevel, bspotTownHall);
				await Task.Delay(400);
				return true;
			}
			else if (a == ContentDialogResult.Secondary)
			{
				return true;
			}
			// canceled
			return false;
		}

		public async Task Build(int id, int bid, bool dryRun, bool verbose)
		{
			var sel = GetBuildingOrLayout(id);
			if(bid==0)
			{
				StatusIf("Trying to build [nothing]?",dryRun,verbose);
				return;
			}
			//if (bid != bidWall && !sel.isEmpty && !SettingsPage.extendedBuild) // special case, wall upgrade from level is allowed as a synonym for build
			//{
			//	StatusIf("Spot is occupied", dryRun,verbose);
			//}
			//else
			{
				var buildDef = BuildingDef.all[bid];
				if (IsWallSpot(id) && !testFlag)
				{
					StatusIf("Walls go here", dryRun,verbose);
					return;
				}
				if (IsWaterSpot(id) && !testFlag)
				{
					StatusIf("There is water here :(", dryRun,verbose);
					return;
				}
				if (IsTowerSpot(id))
				{
					if (!buildDef.isTower)
					{
						StatusIf("This looks like a nice place for a tower.", dryRun,verbose);
						return;
					}
					else
					{
						if (postQueueBuildings[bspotWall].bl == 0)
						{
							StatusIf("Please build a wall first", dryRun,verbose);
							if (!dryRun)
							{

								var good = await BuildWallDialogue();
								if (!good)
									return;
							}
							else
							{
								return;
							}
						}

					}
				}
				else
				{
					if (buildDef.isTower)
					{
						StatusIf("This does not looks like a nice place for a tower.", dryRun,verbose);
						return;
					}
					if (IsShoreSpot(id))
					{
						if (!buildDef.isShoreBuilding && !CityBuild.testFlag)
						{
							StatusIf("Ports and Shipyards go here", dryRun,verbose);
							return;
						}
					}
					else
					{
						if (buildDef.isShoreBuilding && !CityBuild.testFlag)
						{
							StatusIf("Please put this on the shore", dryRun,verbose);
							return;
						}
					}

				}

				if (dryRun)
				{
					DrawBuilding(IdToXY(id), cityDrawAlpha, bid, AGame.animationT * 0.65f);
				}
				else
				{
					if (isPlanner)
					{

						layoutWritable[id] = BidToLayout(bid).c;
						await PlannerTab.BuildingsChanged(this);
					}
					else
					{
						////var counts = GetTownHallAndBuildingCount(true);
						////var usesSpot = !buildDef.isTower && bid != bidWall;
						////if ((counts.buildingCount == counts.townHallLevel * 10 && counts.townHallLevel < 10 && usesSpot) || buildDef.Thl > counts.townHallLevel)
						////{
						////	if (!await UpgradeTownHallDialogue(((counts.buildingCount) / 10 + 1).Max(buildDef.Thl)))
						////		return;

						////}
						//else if (counts.townHallLevel == 10 && counts.buildingCount >= 100 && usesSpot)
						//{
						//	if (!dryRun && bid != bidCottage && wantUI)
						//	{
						//		int toRemove = await FindBuildingToRemoveUI();
						//		if (toRemove != -1)
						//			await Demolish(toRemove, dryRun);
						//	}
						//}
						await Enqueue(0, 1, bid, id);

					}
				}
			}
		}



		public Task Build((int x, int y) cc, int bid, bool dryRun, bool verbose)
		{
				return Build(XYToId(cc), bid,dryRun: dryRun,verbose:verbose);
		}
		public async Task Downgrade((int x, int y) building, bool dryRun)
		{
			var id = XYToId(building);
			var sel = postQueueBuildings[id];

			if (sel.bl == 1)
			{
				await Demolish(id, dryRun);
			}
			else if (sel.bl == 0)
			{
				Status("Already destroyed", dryRun);
				return;

			}
			else
			{
				Status($"Downgrade {sel.def.Bn}", dryRun);
				if (!dryRun)
				{
					if (isPlanner)
					{
						//	--buildingsCache[id].bl;
						//	PlannerTab.BuildingsChanged();
					}
					else
					{
						await Enqueue(sel.bl, sel.bl - 1, sel.def.bid, id);
					}
				}
			}
			//if(!dryRun)
			//	buildQueue.Add(new BuildQueueItem() { bspot = id, bid = sel.def.bid, slvl = sel.bl, elvl = (byte)(sel.bl-1) });
		}
		public async Task DowngradeTo((int x, int y) building, int level)
		{
			if (!isPlanner)
			{
				var id = XYToId(building);
				var sel = postQueueBuildings[id];
				int count = sel.bl - level;
				for (int i = 0; i < count; ++i)
				{
					await Downgrade(building, false);
				}
			}
		}
		public async Task Demolish(int bspot, bool dryRun)
		{
			var sel = GetBuildingOrLayout(bspot);
			if (sel.isEmpty)
			{
				Status("Already destroyed", dryRun);
			}
			else
			{
				//if (buildQueueFull)
				//{
				//	Status("Build Queue full", dryRun);
				//	return;
				//}

				Status($"Destroy {sel.def.Bn}", dryRun);
				if (!dryRun)
				{
					if (isPlanner)
					{
						layoutWritable[bspot] = BidToLayout(0).c;
						await PlannerTab.BuildingsChanged(this);
					}
					else
					{
						await Enqueue(new BuildQueueItem(sel.bl, 0, sel.bid, (short)bspot));
					}
				}
				else
				{
					DrawSprite(IdToXY(bspot), decalBuildingInvalid, 0.312f);
				}
			}
		}
		public Task Enqueue(int slvl, int elvl, int bid, int bspot) => Enqueue(new BuildQueueItem((byte)slvl, (byte)elvl, (short)bid, (short)bspot));


		public Task Enqueue(BuildQueueItem b)
		{
			//if(slvl == 0 && elvl == 1)
			//{
			//	var tlvl = postQueueBuildings[bspotTownHall].bl;
			//	if(BuildingDef.all[bid].Thl > tlvl)
			//	{
			//		Note.Show($"Please upgrade town hall be level {BuildingDef.all[bid].Thl}");
			//		return;

			//	}	
			//}
			//var maxBuildings = postQueueBuildings[bspotTownHall].bl * 10;
			// preview the change immediately
			postQueueBuildings[b.bspot] = b.Apply(postQueueBuildings[b.bspot]);
			return BuildQueue.Enqueue(cid, b);

		}
		public async Task<int> EnqueueUpgrade(int elvl, int spot)
		{

			int rv = 0;
			var b = postQueueBuildings[spot];
			Assert(elvl >= b.bl);
			var bid = b.bid;
			Assert(bid != 0);
			for (var level = b.bl; level < elvl.Min(10); ++level)
			{
				++rv;
				await Enqueue(new BuildQueueItem(level, (byte)(level + 1), (short)bid, (short)spot));
			}
			return rv;

		}
		public async Task<int> EnqueueMove(short spotFrom, short spotTo)
		{
			var b = postQueueBuildings[spotFrom];
			await Enqueue(new BuildQueueItem(b.bl,b.bl,b.bid,spotTo,0,true));
			await Enqueue(new BuildQueueItem(0,0,0,spotFrom,0,true));
			Player.moveSlots -= 1;


			return 1;

		}
		public async Task<int> EnqueueSwap(short spotFrom,short spotTo)
		{
			var bFrom = postQueueBuildings[spotFrom];
			var bTo = postQueueBuildings[spotTo];
			await Enqueue(new BuildQueueItem(bFrom.bl,bFrom.bl,bFrom.bid,spotTo,0,true));
			await Enqueue(new BuildQueueItem(bTo.bl,bTo.bl,bTo.bid,spotFrom,0,true));
			Player.moveSlots -= 2;

			return 0;

		}
		//private void Upgrade_Click(object sender, RoutedEventArgs e)
		//{

		//	var id = XYToId(selected);
		//	var sel = GetBuildingPostQueue(id);
		//	var lvl = sel.bl;

		//	if(lvl == 0)// special case
		//		Build(id,sel.def.bid,false);
		//	else
		//		Enqueue(lvl,(lvl + 1),sel.def.bid,id);

		//}
		public async Task UpgradeToLevel(int level, (int x, int y) target, bool dryRun = false)
		{
			//var target = hovered;
			if (!target.IsValid())
			{
				Status("Please select a building", dryRun);
				return;
			}
			var id = XYToId(target);
			var sel = GetBuildingPostQueue(id);
			if (sel.isRes)
			{
				Note.Show("Cannot upgrade Res");
				return;
			}
			if (sel.isEmpty)
			{
				Note.Show("Nothing to upgrade");
				return;
			}
			var lvl = sel.bl;
			if (lvl >= 10)
			{
				Note.Show("Already level 10");
				return;
			}

			if (level == 1)
				level = lvl + 1;

			if (lvl == 0)
			{
				await Build(id, sel.def.bid, dryRun, false);
				lvl = 1;
			}
			if (lvl < level)
			{

				if (isPlanner)
				{
					//	buildingsCache[id].bl = (byte)level;
					//	PlannerTab.BuildingsChanged();

				}
				else if (!dryRun)
				{
					await EnqueueUpgrade(level, id);
				}
			}
			else
			{
				Status($"{sel.name} is already level {sel.bl} (or upgrading there)", dryRun);

			}
		}

		public Task Demolish((int x, int y) building, bool dryRun)
		{
			return Demolish(XYToId(building), dryRun);
		}

		//public Building GetBuildingPostQueue(int spot)
		//{
		//	return postQueueBuildings[spot];

		//}
		public Building GetBuildingOrLayout(int spot)
		{
			return isPlanner ? GetLayoutBuilding(spot) : postQueueBuildings[spot];

		}

		public async Task<int> FindBuildingToRemoveUI(bool showUI, bool dryRun)
		{
			var toRemove = -1;
			if (SettingsPage.demoBuildingOnBuildIfFull != false)
			{
				var bd = FindExtraBuilding();
				if (bd != -1)
				{
					if (SettingsPage.demoBuildingOnBuildIfFull == null)
					{
						Status($"Maybe destory {postQueueBuildings[bd].name} to make room...",dryRun);
						if(showUI && !dryRun )
						{
							var xy = await App.DoYesNoBoxSticky($"Demo {postQueueBuildings[bd].name} to make room?");
							SettingsPage.demoBuildingOnBuildIfFull = xy.sticky;
							if(!xy.rv)
								bd = -1;
						}
						else
						{
							bd = -1;
						}
					}
					if (bd != -1)
					{
						toRemove = bd;
					}
				}
			}
			if (toRemove == -1)
			{
				if (SettingsPage.demoCottageOnBuildIfFull != false)
				{
					var bd = FindCabinToDemo();
					if (bd != -1)
					{
						if (SettingsPage.demoCottageOnBuildIfFull == null)
						{
							Status($"Maybe destory {postQueueBuildings[bd].name} to make room...",dryRun);
							if(showUI && !dryRun)
							{
								var xy = await App.DoYesNoBoxSticky($"Demo cabin to make room?");
								SettingsPage.demoCottageOnBuildIfFull = xy.sticky;
								if(!xy.rv)
									bd = -1;
							}
							else
							{
								bd=-1;
							}
						}
						if (bd != -1)
						{
							toRemove = bd;
						}
					}
				}
			}

			return toRemove;
		}

		public async Task<int> SmartBuild((int x, int y) cc, int desBid, bool searchForSpare, bool dryRun = false, 
					
						bool wantDemoUI = false )
		{
			int rv = 0;
			var bspot = XYToId(cc);
			var b = GetBuildingOrLayout(bspot);
			var desB = BuildingDef.all[desBid];
			var desName = desB.Bn;
			if (BuildingDef.IsBidRes(desBid))
			{
				desBid = 0; // if it is a resource, ignore it
			}
			if (!IsBuildingSpot(bspot))
			{
				await Build(cc, desBid, dryRun, false); ;
				return 1;
			}
			var usesSpot = IsBuildingSpot(bspot) && !desB.isTower && !desB.isTownHall && (bspot != bspotWall);
			var curBid = b.def.bid;
			var takeFrom = -1;
			var takeScore = 0;
			var putTo = -1;
			var putScore = 0;
			var postBuildings = postQueueBuildings;

			if (searchForSpare && !isPlanner && isLayoutCustom && usesSpot)
			{
				// See if there are spare buildings that we can take
				for (int xy = 1; xy < City.citySpotCount - 1; ++xy)
				{
					var overlayBid = GetLayoutBid(xy);
					var xyBuilding = postBuildings[xy].def.bid;

					if (overlayBid != xyBuilding)
					{

						// do they have what we need?
						if (xyBuilding == desBid && desBid != 0)
						{
							// two points for ourbuilding is also needed there
							var score = ((overlayBid == curBid) ? 2 : 1);
							
							if (score > takeScore)
							{
								takeScore = score;
								takeFrom = xy;
							}
						}

						//do they want what we have?
						if (overlayBid == curBid && curBid != 0)
						{
							var score = (xyBuilding == desBid) ? 4 : (xyBuilding == 0) ? 3 : postBuildings[xy].isBuilding ? 2 : 1;
							if (score > putScore)
							{
								putScore = score;
								putTo = xy;

							}

						}
					}
				}
			}
			var counts = GetTownHallAndBuildingCount(true);

			// case 1:  nothing here, or res. if res, demo first, then Add building
			if (b.id == 0 || b.isRes)
			{

				// Do we want a building here?
				if (desBid != 0)
				{
					if (b.isRes || isPlanner)
					{

						Status($"Destroying {b.def.Bn} to make way for {desName}", dryRun);


						await Demolish(cc, dryRun);
						++rv;
						if (!dryRun)
							await Task.Delay(400).ConfigureAwait(true);
						if (takeScore > 0 && searchForSpare)
						{
							Status($"Please wait on demo, then spare {desName} can be moved into place", dryRun);
							// wait for demo
							return rv;
						}
					}
					else if (takeScore > 0 && searchForSpare )
					{
						Status($"Found an unneeded {desName}, will move it to the right spot for you", dryRun);

						if (!await MoveBuilding(takeFrom, bspot, dryRun))
							return -1;
						return rv;
					}
					if ((counts.townHallLevel < desB.Thl || (counts.buildingCount == counts.max && counts.townHallLevel < 10)))
					{
						var level = (counts.buildingCount / 10 + 1).Max(b.def.Thl).Min(10);
						if (dryRun)
						{
							Status($"Upgrade town hall to level {level}", dryRun);

						}
						else
						{
							if (!(await UpgradeTownHallDialogue(level)))
								return rv;
							++rv; // not exact
						}
					}
					else if ( counts.buildingCount >= 100 && !desB.isTower && !desB.isWall)
					{
						if (!desB.isCabin && desBid != 0)
						{
							int bestSpot = await FindBuildingToRemoveUI(showUI: wantDemoUI,dryRun:dryRun);

							if (bestSpot != -1)
							{
								Status($"Will Demolish {postBuildings[bestSpot]} to make room", dryRun);

								await Demolish(bestSpot, dryRun);
								//break;
								--counts.buildingCount;
								++rv;

							}
						}
						else
						{
							Status("Don't want to demo a cabin to build a cabin", dryRun);
						}


					}
					//if (counts.buildingCount < 100)
					await Build(cc, desBid == 0 ? bidCottage : desBid, dryRun:dryRun,verbose:true);

				}
				else
				{
					Assert(!isPlanner);
					// Nothing wanted here
					if (b.isRes)
					{
						Status($"What a lovely {b.def.Bn}.", dryRun);

					}
					else
					{
						// nothing here
						if (counts.buildingCount < 100) // can we put a cabin here?
						{
							Status($"No building is wanted here, how about a cottage instead?", dryRun);
							await Build(cc, bidCottage, dryRun,verbose: false);
							++rv;
						}
						else
						{
							Status("Don't want to demo a cabin to build a cabin", dryRun);
						}
					}
				}
			}
			else
			{
				// building is here
				// a building
				// Try to move it to some place where one is needed

				if (desBid != curBid)
				{
					if (putScore > 0)
					{
						var name = b.def.Bn;
						if (putScore < 8)
						{
							Status($"{b.def.Bn} is wanted elsewhere but someone is doing something - please clear the queue or wait", dryRun);
						}
						else
						{
							switch (putScore)
							{
								case 4:
								case 4 + 8:
									{
										Status($"Swaping {b.def.Bn} and {desName} as they are mixed up ({Player.moveSlots} moves left)", dryRun);
										if (!await SwapBuilding(bspot, putTo, dryRun))
											return -1;
										
										// two way swap 
										break;
									}
								case 3:
								case 3 + 8:
									Status($"Move {name} to where it is wanted", dryRun);
									if (!await MoveBuilding(bspot, putTo, dryRun))
										return -1;
									break;
								case 2:
								case 2 + 8:
									Status($"{name} is wanted elsewhere but there is a building in the way", dryRun);
									break;
								case 1:
								case 1 + 8:
									Status($"{name} is wanted elsewhere but there are reources in the way", dryRun);
									break;


							}
						}



					}
					else
					{

						if (takeScore == 0)
						{
							if (desBid == 0)
							{
								Status($"{b.def.Bn} is not wanted, destroying it", dryRun);

								++rv;
								await Demolish(cc, dryRun);

							}
							else
							{
								if (counts.buildingCount >= 100)
								{

									if (wantDemoUI)
									{
										int bestSpot =await FindBuildingToRemoveUI(showUI: wantDemoUI,dryRun:dryRun);
										if (bestSpot != -1)
										{
											await Demolish(bestSpot, dryRun);
											--counts.buildingCount;
											++rv;
										}

									}

									
								}


								var cb = FindAnyFreeSpot(bspot, !dryRun);
								if (!await MoveBuilding(bspot, cb, dryRun))
									return -1;


								// build the correct building
								if (dryRun)
									DrawSprite(hovered, decalBuildingInvalid, .5f);

								await Build(cc, desBid, dryRun, verbose:true);
								++rv;
							}
						}
						else
						{
							if (takeScore < 8)
							{
								Status($"There an extra {desName}, but it cannot be moved because someone is doing something, please clear queue or wait", dryRun);

							}
							else
							{
								if(!dryRun)
								{
									var move = isPlanner ? 1 : await App.DoYesNoBox($"{b.name} in the way :(",$"What do you want to do with {b.name}?","Move it" ," Demo", "Forget it");
									if(move == -1)
										return -1; 
									else if(move == 1)
									{
										if(!await MoveBuilding(bspot,FindAnyFreeSpot(bspot),dryRun))
											return -1;
									}
									else

									{
										await Demolish(bspot,dryRun);
									}
								}
								Status($"Found an unneeded {desName}, will move it to the right spot for you", dryRun);

								if (!await MoveBuilding(takeFrom, bspot, dryRun))
									return -1;
							}
						}
					}

				}
				else
				{
					Status($"{desName} is in the right spot, no changed needed", dryRun);

				}
			}
			return rv;
		}

		private int FindCabinToDemo()
		{
			var bestSpot = -1;
			int bestLevel = int.MaxValue;
			for (int spot = 0; spot < citySpotCount; ++spot)
			{
				var bld = postQueueBuildings[spot];
				if (bld.isCabin)
				{
					if (bld.bl < bestLevel)
					{
						// is it not being modified?
						
						bestLevel = bld.bl;
						bestSpot = spot;
					}
				}
			}

			return bestSpot;
		}

		public int FindAnyFreeSpot(int referenceBid, bool verbose = true)
		{
			return FindFreeSpot(CityBuild.GetSpotType(referenceBid), verbose);
		}
		public ushort findSpotOffset;
		public int FindFreeSpot(SpotType type = SpotType.building, bool verbose = true)
		{
			var build = City.GetBuild();
			var spots = CityBuild.GetSpots(type);
			for (int iter = 0; iter <= City.citySpotCount; ++iter)
			{
				// update and wrap
				++findSpotOffset;
				if (findSpotOffset >= City.citySpotCount - 1)
					findSpotOffset = 1;

				if (!spots.Contains(findSpotOffset))
					continue;
				var bld = postQueueBuildings[findSpotOffset];
				if (!bld.isEmpty)
					continue;
				// not yet removed
				if (!build.buildings[findSpotOffset].isEmpty)
					continue;
				if (build.GetLayoutBid(findSpotOffset) != 0)
					continue;
				return findSpotOffset;
			}
			if (verbose)
				Assert(false);
			return 0; // error
		}
		public int FindSpare(int bid, bool dryRun)
		{
			var rv = 0;
			var build = City.GetBuild();
			for (int xy = 0; xy < City.citySpotCount; ++xy)
			{
				var overlayBid = build.GetLayoutBid(xy);
				var xyBuilding = postQueueBuildings[xy].def.bid;

				if (overlayBid == xyBuilding || xyBuilding != bid)
					continue;
			
				return xy;
			}
			return rv;

		}

		

		public async Task<bool> MoveBuilding(int a, int b, bool dryRun)
		{
			// Todo:  Cannot be moved if queued
			// Todo: Error checking
			if (a == 0 || b == 0)
			{
				Status($"No buildings to move?", dryRun);
				return false;
			}
			if (!isPlanner)
			{
				var bds = buildings;

				
				{
					Status($"Move {bds[a].name} {IdToXY(a).bspotToString()} to {IdToXY(b).bspotToString()} ", dryRun);

					var rv = true;
					if (!dryRun)
					{
						EnqueueMove((short)a,(short)b);
						
						// I hope that these operations are what I expect with references

					}
					return rv;
				}
			}
			else
			{
				//	Status($"Move {bds[a].name} {IdToXY(a).bspotToString()} to {IdToXY(b).bspotToString()} ", dryRun);

				if (!dryRun)
				{
					AUtil.Swap(ref layoutWritable[b], ref layoutWritable[a]);
					await PlannerTab.BuildingsChanged(this);

				}
				return true;
			}
		}

		

		public static async Task<bool> DoMove(int cid,int @from, int to)
		{
			
			var s = await Services.Post.SendForText("includes/mBu.php", $"a={to}&b={@from}&c={cid}", World.CidToPlayerOrMe(cid));
			if(s.Trim().TryParseInt(out var i) && i > 10 )
			{
			
				return true;
			}
			else
			{
				var isBuild = City.IsBuild(cid);
				if (isBuild)
				{
					Trace(
						$"*Invalid Move* Error code {i} (4 means out of move slots, you _might_ have {Player.moveSlots} left) at {cid.AsCity()}: {from}<=>{to}"
						);
				}

				return false;
			}
		}

		public async Task<bool> SwapBuilding(int a, int b, bool dryRun)
		{
			if (!isPlanner)
			{
				var bds = buildings;
				
				{
					// I hope that these operations are what I expect with references
					Status($"Swap {bds[b].name} and {bds[a].name} ({Player.moveSlots} moves left) ", dryRun);
					if (!dryRun)
					{
						EnqueueSwap((short)a, (short) b);

						

					}
					return true;
				}
			}
			else
			{
				//Status($"Swap {bds[b].name} and {bds[a].name} ({Player.moveSlots} moves left) ", dryRun);
				if (!dryRun)
				{
					AUtil.Swap(ref layoutWritable[a], ref layoutWritable[b]);
					await PlannerTab.BuildingsChanged(this);
				}
				return true;

			}

		}
		[Flags]
		public enum IncomingFlags
		{
			incoming = 1<<0,
			sen = 1<<1,
			art = 1<<2,
			sieged = 1<<3, // otherwise incoming such as assuault
			claim = 1<<4,
		}

		public IncomingFlags incomingFlags
		{
			get {
				if(!incoming.Any())
					return default;

				IncomingFlags rv = default;

				foreach(var i in incoming)
				{
					if(i.isAttack)
					{
						rv |= IncomingFlags.incoming;
						if(i.isSiege)
							rv |= IncomingFlags.sieged;
						if(i.hasSenator)
							rv |= IncomingFlags.sen;
						if(i.hasArt)
							rv |= IncomingFlags.art;
					}
				}
				return rv;
			}
		}

		public long reinforcementSortScore =>
			(((long) incomingFlags) << 56) + ((long) pid << 32) + cid.ZCurveEncodeCid();
		public async Task<bool> DoPoll()
		{
			var pollResult = (await Post.SendForJson("/includes/poll2.php",$"cid={cid}&ai=0&ss={JSClient.cotgS}").ConfigureAwait(false) ).RootElement;
			if(pollResult.TryGetProperty("city",out var cjs))
			{
				LoadCityData(cjs);
				return cjs.TryGetProperty("bq", out var _); // did we get a build queue?
			}
			else
			{
				Assert(false);
				return false;
			}
		}
	}

}