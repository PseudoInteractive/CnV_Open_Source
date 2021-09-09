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
using Windows.UI.Xaml.Controls;
using static COTG.Debug;
using static COTG.Draw.CityView;
using static COTG.Game.City;
using static COTG.Views.CityBuild;
using static COTG.BuildingDef;
using COTG.Views;

namespace COTG.Game
{


	public partial class City
	{
		private static Building[] postQueueBuildingCache = new Building[citySpotCount];
		public static City cachedCity;
		static int postQueueBuildingCount = -1;
		public static void BuildingsOrQueueChanged()
		{
			cachedCity = null;
			postQueueBuildingCount = -1;
		}
		public Building[] postQueueBuildings
		{
			get
			{
				if (cachedCity == this)
					return postQueueBuildingCache;
				cachedCity = this;
				//if (!CityBuild.isPlanner)
				//{
				//var  buildingsCache = buildings;
				//}
				var rv = postQueueBuildingCache;
				//
				// copy current buildings
				//
				for (var i = 0; i < citySpotCount; ++i)
				{
					rv.DangerousGetReferenceAt(i) = buildings.DangerousGetReferenceAt(i);
				}
				if (!CityBuild.isPlanner)
				{
					//
					// Apply queue
					//
					{
						foreach (var q in buildQueue)
						{
							rv.DangerousGetReferenceAt(q.bspot) = q.Apply(rv.DangerousGetReferenceAt(q.bspot));
						}

						if (CityBuildQueue.all.TryGetValue(City.build, out var bq))
						{
							var count = bq.queue.count;
							var data = bq.queue.v;

							for (int i = 0; i < count; ++i)
							{
								rv.DangerousGetReferenceAt(data[i].bspot) = data[i].Apply(rv.DangerousGetReferenceAt(data[i].bspot));
							}
						}



					}
				}
				//postQueuebuildingsCache = rv;
				return rv;
			}
		}
		public int postQueueTownHallLevel => CityBuild.isPlanner switch { true => 10, _ => postQueueBuildings[bspotTownHall].bl };
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

		public async Task Build(int id, int bid, bool dryRun, bool wantUI)
		{
			var sel = GetBuildingOrLayout(id);
			if (bid != bidWall && !sel.isEmpty && !SettingsPage.extendedBuild) // special case, wall upgrade from level is allowed as a synonym for build
			{
				Status("Spot is occupied", dryRun);
			}
			else
			{
				var buildDef = BuildingDef.all[bid];
				if (IsWallSpot(id) && !testFlag)
				{
					Status("Walls go here", dryRun);
					return;
				}
				if (IsWaterSpot(id) && !testFlag)
				{
					Status("There is water here :(", dryRun);
					return;
				}
				if (IsTowerSpot(id))
				{
					if (!buildDef.isTower)
					{
						Status("This looks like a nice place for a tower.", dryRun);
						return;
					}
					else
					{
						if (postQueueBuildings[bspotWall].bl == 0)
						{
							Status("Please build a wall first", dryRun);
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
						Status("This does not looks like a nice place for a tower.", dryRun);
						return;
					}
					if (IsShoreSpot(id))
					{
						if (!buildDef.isShoreBuilding && !CityBuild.testFlag)
						{
							Status("Ports and Shipyards go here", dryRun);
							return;
						}
					}
					else
					{
						if (buildDef.isShoreBuilding && !CityBuild.testFlag)
						{
							Status("Please put this on the shore", dryRun);
							return;
						}
					}

				}

				if (dryRun)
				{
					DrawBuilding(IdToXY(id), cityDrawAlpha, bid, AGame.animationT * 0.3247f);
				}
				else
				{
					if (isPlanner)
					{

						layoutWritable[id] = BidToLayout(bid).c;
						PlannerTab.BuildingsChanged(this);
					}
					else
					{
						var counts = GetTownHallAndBuildingCount();
						var usesSpot = !buildDef.isTower && bid != bidWall;
						if ((counts.buildingCount == counts.townHallLevel * 10 && counts.townHallLevel < 10 && usesSpot) || buildDef.Thl > counts.townHallLevel)
						{
							if (!await UpgradeTownHallDialogue(((counts.buildingCount) / 10 + 1).Max(buildDef.Thl)))
								return;

						}
						else if (counts.townHallLevel == 10 && counts.buildingCount >= 100 && usesSpot)
						{
							if (!dryRun && bid != bidCottage && wantUI)
							{
								int toRemove = await FindBuildingToRemoveUI();
								if (toRemove != -1)
									await Demolish(toRemove, dryRun);
							}
						}
						await Enqueue(0, 1, bid, id);

					}
				}
			}
		}



		public Task Build((int x, int y) cc, int bid, bool dryRun, bool wantRemoveUI)
		{
			if (bid != 0)
				return Build(XYToId(cc), bid, dryRun, wantRemoveUI);
			else
				return Task.CompletedTask;
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
				Status("Already destoryed", dryRun);
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
		public async Task Demolish(int id, bool dryRun)
		{
			var sel = GetBuildingOrLayout(id);
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
						layoutWritable[id] = BidToLayout(0).c;
						PlannerTab.BuildingsChanged(this);
					}
					else
					{
						await Enqueue(new BuildQueueItem(sel.bl, 0, sel.bid, (short)id));
					}
				}
				else
				{
					DrawSprite(IdToXY(id), decalBuildingInvalid, 0.312f);
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
			var bid = b.bid;
			Assert(bid != 0);
			for (var level = b.bl; level < elvl.Min(10); ++level)
			{
				++rv;
				await Enqueue(new BuildQueueItem(level, (byte)(level + 1), (short)bid, (short)spot));
			}
			return rv;

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
			var sel = postQueueBuildings[id];
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

		public async Task<int> FindBuildingToRemoveUI()
		{
			var toRemove = -1;
			if (SettingsPage.demoBuildingOnBuildIfFull != false)
			{
				var bd = FindExtraBuilding();
				if (bd != -1)
				{
					if (SettingsPage.demoBuildingOnBuildIfFull == null)
					{
						var xy = await App.DoYesNoBoxSticky($"Demo {postQueueBuildings[bd].name} to make room?");
						SettingsPage.demoBuildingOnBuildIfFull = xy.sticky;
						if (!xy.rv)
							bd = -1;
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
							var xy = await App.DoYesNoBoxSticky($"Demo cabin to make room?");
							SettingsPage.demoCottageOnBuildIfFull = xy.sticky;
							if (!xy.rv)
								bd = -1;
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

		public async Task<int> SmartBuild((int x, int y) cc, int desBid, bool searchForSpare, bool dryRun = false, bool demoExtraBuildings = false, bool wantDemoUI = false)
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

			if (searchForSpare && !isPlanner && isLayoutCustom && usesSpot)
			{
				// See if there are spare buildings that we can take
				for (int xy = 1; xy < City.citySpotCount - 1; ++xy)
				{
					var overlayBid = GetLayoutBid(xy);
					var xyBuilding = postQueueBuildings[xy].def.bid;

					if (overlayBid != xyBuilding)
					{

						// do they have what we need?
						if (xyBuilding == desBid && desBid != 0)
						{
							// two points for ourbuilding is also needed there
							var score = ((overlayBid == curBid) ? 2 : 1);
							if (!HasBuildOps(xy))
								score += 8;
							if (score > takeScore)
							{
								takeScore = score;
								takeFrom = xy;
							}
						}

						//do they want what we have?
						if (overlayBid == curBid && curBid != 0)
						{
							var score = (xyBuilding == desBid) ? 4 : (xyBuilding == 0) ? 3 : postQueueBuildings[xy].isBuilding ? 2 : 1;
							if (!HasBuildOps(xy))
								score += 8;
							if (score > putScore)
							{
								putScore = score;
								putTo = xy;

							}

						}
					}
				}
			}
			var counts = GetTownHallAndBuildingCount();

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
					else if (takeScore > 0 && searchForSpare)
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
							int bestSpot = -1;
							if (wantDemoUI && !dryRun)
							{
								bestSpot = await FindBuildingToRemoveUI();

							}
							else
							{
								// Is there a cabin to remove?
								bestSpot = demoExtraBuildings ? FindExtraBuilding() : -1;
								if (bestSpot == -1)
									bestSpot = FindCabinToDemo();
							}
							if (bestSpot != -1)
							{
								Status("Will Demolish something to make room", dryRun);

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
					await Build(cc, desBid == 0 ? bidCottage : desBid, dryRun, false);

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
							await Build(cc, bidCottage, dryRun, false);
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

									if (demoExtraBuildings)
									{
										int bestSpot = FindExtraBuilding();
										if (bestSpot != -1)
										{
											await Demolish(bestSpot, dryRun);
											--counts.buildingCount;
											++rv;
										}

									}

									if (counts.buildingCount >= 100 && desBid != bidCottage)
									{
										var bestSpot = FindCabinToDemo();
										if (bestSpot != -1)
										{
											Status("Will Demolish a Cottage to make room", dryRun);

											await Demolish(bestSpot, dryRun);
											//break;
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
									DrawSprite(hovered, decalBuildingInvalid, .31f);

								await Build(cc, desBid, dryRun, wantDemoUI);
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
								if (!await MoveBuilding(bspot, FindAnyFreeSpot(bspot), dryRun))
									return -1;

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
						if (HasBuildOps(spot))
							continue;

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
				if (HasBuildOps(findSpotOffset))
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
				if (HasBuildOps(xy) && !isPlanner)
				{
					Status($"{postQueueBuildings[xy].name} at {IdToXY(xy).bspotToString()} is desired, but it is being rennovated, please wait or cancel build ops for best resuts", dryRun);
					rv = -1;
					continue;
				}
				return xy;
			}
			return rv;

		}

		public bool HasBuildOps(int bspot)
		{
			return (IterateQueue().Any(a => a.bspot == bspot));

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

				if (HasBuildOps(a) && !isPlanner)
				{
					Status($"Cannot move a building that is being rennovated, please wait or cancel build ops on {bds[a].name} at {IdToXY(a).bspotToString()} or wait", dryRun);
					return false;
				}
				if (HasBuildOps(b) && !isPlanner)
				{
					Status($"Cannot move a building to a spot that is being rennovated, please wait or cancel build ops on {bds[b].name} at {IdToXY(b).bspotToString()} or wait for best results", dryRun);
					return false;
				}
				if (Player.moveSlots <= 0 && !isPlanner)
				{
					Status($"No move spots", dryRun);
					return false;
				}
				else
				{
					Status($"Move {bds[a].name} {IdToXY(a).bspotToString()} to {IdToXY(b).bspotToString()} ", dryRun);

					var rv = true;
					if (!dryRun)
					{
						AUtil.Swap(ref bds[b], ref bds[a]);
						{
							rv = await Move(a, b);

							//	await Task.Delay(200);

							--Player.moveSlots;
							BuildingsOrQueueChanged();
							await Task.Delay(200);
						}
						
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
					layoutWritable[b] = layoutWritable[a];
					PlannerTab.BuildingsChanged(this);

				}
				return true;
			}
		}

		async Task<bool> Move(int from, int to)
		{
			Assert(!isPlanner);
			var s = await Services.Post.SendForText("includes/mBu.php", $"a={from}&b={to}&c={cid}", World.CidToPlayerOrMe(cid));
			if ((int.TryParse(s.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out var i) &&

				(i >= City.bidMin && i <= City.bidMax)))
			{
				return true;
			}
			else
			{
				Trace($"Invalid Move {i}");
				return false;
			}

		}

		public async Task<bool> SwapBuilding(int a, int b, bool dryRun)
		{
			if (!isPlanner)
			{
				var bds = buildings;
				if (HasBuildOps(a) || HasBuildOps(b))
				{
					{
						Status($"Cannot move a building that is being rennovated, please wait or cancel build ops on {bds[a].name} at {IdToXY(a).bspotToString()} for best results", dryRun);
					}
					if (HasBuildOps(b))
					{
						Status($"Cannot move a building that is being rennovated, please wait or cancel build ops on {bds[b].name} at {IdToXY(b).bspotToString()} for best results", dryRun);
					}

					return false;
				}
				if (Player.moveSlots >= 3)
				{
					// I hope that these operations are what I expect with references
					Status($"Swap {bds[b].name} and {bds[a].name} ({Player.moveSlots} moves left) ", dryRun);
					if (!dryRun)
					{

						AUtil.Swap(ref bds[a], ref bds[b]);

						{
							var scratch = FindAnyFreeSpot(a);
							if (scratch == 0)
								return false;

							--Player.moveSlots;
							if (!await Move(a, scratch))
								return false;
							--Player.moveSlots;
							if (!await Move(b, a))
								return false;
							--Player.moveSlots;
							if (!await Move(scratch, b))
							{
								Note.Show("Failed to move for some reason?  Was the city building?");
								return false;
							}
							BuildingsOrQueueChanged();
							await Task.Delay(200);

						}

					}
					return true;
				}
				else
				{
					Status("Note enough move spots (press shift F5 if this is not true)", dryRun);
					return false;
				}
			}
			else
			{
				//Status($"Swap {bds[b].name} and {bds[a].name} ({Player.moveSlots} moves left) ", dryRun);
				if (!dryRun)
				{
					AUtil.Swap(ref layoutWritable[a], ref layoutWritable[b]);
					PlannerTab.BuildingsChanged(this);
				}
				return true;

			}

		}

	}

}