using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CnV.Services;
using CnV.Views;

using EnumsNET;

using static CnV.Views.QueueTab;
using static CnV.Views.ShellPage;
using static CnV.CityBuild;
using Microsoft.UI.Xaml.Controls;
using CnV;
using static CnV.Debug;
using static CnV.City;
using static CnV.Building;

using static CnV.CityBuild;


namespace CnV;

using Game;
using Services;
using Views;

public static class DoTheStuff
	{
	static readonly int[] cabinCounts = { 0,1,2,3,4,5,6,7,8 };

	public static async Task<bool> Go(City city,bool allowRename,bool allowSetLayout)
		{

			InitNextMoveConfirm();

			var cid = city.cid;
			Assert(city.isBuild);
			if(View.viewMode != ViewMode.city)
				View.SetViewMode(ViewMode.city);
			await CityBuild._IsPlanner(false,true);

			Assert(AppS.uiSema.CurrentCount == 0);
			Assert(AppS.IsOnUIThread());
			await GetCity.Post(cid);
			if(city.leaveMe)
			{
				Note.Show($"Skipping ${city.nameMarkdown}, 'LeaveMe' tag is set");
				return true;
			}
			var bc = city.UpdateBuildStage();

			//if(allowSetLayout && (city.buildStage == City.BuildStage._new|| (city.autobuild==0&&(await AppS.DoYesNoBox("Autobuild Off?","Maybe you want Setup?")==1))))
			//{
			//	await ShareString.ShowNoLock(cid, SetupFlags.suggestAutobuild).ConfigureAwait(false);
			//	return true;
			//}
			if(allowSetLayout && !city.isLayoutCustom &&(await AppS.DoYesNoBox("No layouT","Maybe you want Setup?")==1))
			{
				await ShareString.ShowNoLock(cid, SetupFlags.suggestAutobuild|SetupFlags.layout).ConfigureAwait(false);
				return true;
			}


			Assert(city.isBuild);

			if(city.buildStage == City.BuildStage.noLayout && allowSetLayout)
			{
				//				if (!city.isBuild)
				//				await CnVServer.ChangeCity(city.cid, false);
				await ShareString.ShowNoLock(cid,SetupFlags.all);
				bc=city.UpdateBuildStage();

				if(city.buildStage == City.BuildStage.noLayout)
					return false;
			}

			Assert(city.isBuild);
			if((city.buildStage == City.BuildStage.complete)||(false))
			{
				Note.Show($"Complete: {city}");
				return true;
			}
			var bad = CountBadBuildings(city);
			if(bad.isBad)
			{
				switch(await AppS.DoYesNoBox("Layout is not ideal",$"{bad.matches} matches, {bad.extraBuildings} extra or misplaced, {bad.missingOverlayBuildings} missing or out of place, Yes to Continue, No to cancel and change layout (or 'Use Buildings' in planner)","Continue","Layout..","Cancel"))
				{
					case -1: return false;
					case 0:
						await ShareString.ShowNoLock(cid,SetupFlags.layout);
						return false;
					default: break; // continue along
				}
			}

			city.BuildOpStart();
		try
		{


			Assert(city.isBuild);
			if(city.is7Point)
			{
				if(bc.townHallLevel < 8)
				{
					await city.EnqueueUpgrade(8,bspotTownHall);
					bc = city.UpdateBuildStage();
					Assert(false);
				}

			}
			if(city.buildStage == City.BuildStage.cabins || city.buildStage == City.BuildStage.townHall)
			{
				if(bc.cabins >= Settings.startCabinCount || bc.buildingCount >= bc.buildingLimit - 2)
				{
					Note.Show($"Building {city.buildStage.AsString()} - {city}");
					return true;
				}
			}

			Assert(city.isBuild);
			if(!bc.hasCastle  && ((city.tsTotal > Settings.tsForCastle)||(city.is7Point&&bc.townHallLevel>=8)) && city.hasCastleInLayout)
			{
				//			if (!city.isBuild)
				//				await CnVServer.ChangeCity(city.cid, false); ;
				//				if(!bc.hasWall)
				//					await city.Enqueue(0, 1, bidWall, bspotWall);
				await city.SmartBuild(city.FindOverlayBuildingOfType(bidCastle),bidCastle,searchForSpare: true,wantDemoUI: true);
				bc = city.UpdateBuildStage();
				//bc.wallLevel = 1;
				if(city.is7Point)
					city.SetAutobuild(false,false,false);

				if(city.is7Point && bc.hasCastle)
				{
					await city.DowngradeTo((0, 0),1); // downgrade town hall
				}
			}
			if((bc.sorcTowers == 0 || bc.sorcTowerLevel != 10) && (city.tsTotal > Settings.tsForSorcTower || (!city.isMilitary && city.points> Settings.scoreForSorcTower)) && city.FindOverlayBuildingsOfType(bidSorcTower).Count == 1)
			{
				var c = FindValidBuildingOfType(city,bidSorcTower);

				if(c.bl == 0)
				{
					if(await city.SmartBuild((c.x, c.y),bidSorcTower,searchForSpare: true,wantDemoUI: true) != -1)
						c.bl = 1;



				}
				// raise to level 10
				if(c.bl != 0) // did it work?
					await city.EnqueueUpgrade(10,(c.x, c.y));
			}
			Assert(city.isBuild);
			if(!bc.hasWall && bc.hasCastle && !city.is7Point)
			{
				await city.Enqueue(0,1,bidWall,bspotWall,false);
				bc.wallLevel = 1;
			}
			Assert(city.isBuild);
			if(bc.hasWall && bc.scoutpostCount < Settings.scoutpostCount && !city.is7Point)
			{
				while(bc.scoutpostCount < Settings.scoutpostCount)
				{
					var spot = BuildC.Nan;
					foreach(var _spot in innerTowerSpots)
					{
						if(city.postQueueBuildings[_spot].isEmpty)
						{
							spot = _spot;
							goto added;
						}

					}
					foreach(var _spot in outerTowerSpots)
					{
						if(city.postQueueBuildings[_spot].isEmpty)
						{
							spot = _spot;
							goto added;
						}

					}
					break; // non left;
				added:
					Assert(spot.isNotNan);
					await city.Enqueue(0,1,bidSentinelPost,spot,false);

					++bc.scoutpostCount;
				}
			}

			// todo: teardown
			//	await CnVServer.ChangeCity(city.cid,false);
			bc=city.UpdateBuildStage();


			switch(city.buildStage)
			{
				case City.BuildStage.noLayout:
				case City.BuildStage._new:
				case City.BuildStage.setup:
				case City.BuildStage.townHall:
				case City.BuildStage.cabins:
					{
						if((bc.cabins < Settings.startCabinCount || (bc.storeHouses == 0)) && !city.is7Point)
						{
							switch(await AppS.DoYesNoBox("Add Cabins",$"Would you like to add cabins to {city.nameAndRemarks}?"))
							{
								case -1: return false;
								case 0: return true;
							}

							string message = "Cabins are building, please come back later";

							if(bc.townHallLevel < 4)
							{
								await city.EnqueueUpgrade(4,bspotTownHall);
							}
							var storeHouses = FindPendingOverlayBuildingsOfType(city,Settings.intialStorehouses - bc.storeHouses,bidStorehouse,true);
							foreach(var storage in storeHouses)
							{

								message += $"Adding Storehouse";
								await city.SmartBuild(storage,bidStorehouse,searchForSpare: true,wantDemoUI: true);
								++bc.storeHouses;


							}
							if(bc.cabins < Settings.startCabinCount)
							{
								message = $"Adding {Settings.startCabinCount - bc.cabins}";
								var cabinsInLayout = FindPendingOverlayBuildingsOfType(city,100,bidCottage).ToList();

								// find a good spot for a cabin
								for(var y = span0;y <= span1;++y)
								{
									for(var x = span0;x <= span1;++x)
									{
										bool hasCabinsInLayout = cabinsInLayout.Any();

										var c = hasCabinsInLayout ? cabinsInLayout.First() : new BuildC(x,y);// (int x, int y) c = RandCitySpot();
										if(hasCabinsInLayout)
										{
											cabinsInLayout.RemoveAt(0);
										}

										if(!city.IsBuildingSpot(c))
										{
											continue;
										}


										if(bc.buildingCount >= city.GetBuildingLimit(bc))
											goto done;
										if(city.postQueueBuildings[c].isEmpty && (city.GetLayoutBid(c) == 0))
										{
											await city.Build(c,bidCottage,false,false);
											++bc.cabins;
										}
										if(bc.cabins >= Settings.startCabinCount)
										{
											goto done;
										}
									}
								}
							}
						done:;
							Assert(city.isBuild);


							Note.Show(message);
						}
						else
						{
							Note.Show("Cabins or townhall are building, please come back later");
						}
					}
					break;
				case City.BuildStage.cabinsDone:
					{
						//var c = RandomCitySpot();
						var message = string.Empty;
						var buildingLimit = city.GetBuildingLimit(bc);// !city.hasCastleInLayout ? 100 : bc.hasCastle ? 100 : 99;

						if(bc.buildingCount < buildingLimit)
						{
							switch(await AppS.DoYesNoBox("Building Placement",$"Would you like to place buildings for {city.nameAndRemarks}?"))
							{
								case -1: return false;
								case 0: return true;
							}

							if(bc.storeHouses < Settings.intialStorehouses)
							{
								var storage = FindPendingOverlayBuildingsOfType(city,Settings.intialStorehouses  - bc.storeHouses,bidStorehouse,true);
								foreach(var s in storage)
								{
									message += $"Adding Storehouse";
									await city.SmartBuild(s,bidStorehouse,searchForSpare: true,wantDemoUI: true);
									bc = city.GetBuildingCounts();
								}
							}
							if(bc.forums == 0 && bc.ports==0 && bc.buildingCount < buildingLimit)
							{
								var bid = bidMarketplace;
								var bd = FindPendingOverlayBuildingsOfType(city,Settings.intialMarkets,bid);
								if(!bd.Any())
								{
									bid = bidPort;
									bd = FindPendingOverlayBuildingsOfType(city,Settings.intialMarkets,bid);
								}
								foreach(var b in bd)
								{
									if(bc.buildingCount >= buildingLimit)
										break;
									message += $"Adding Forum or Port";
									await city.SmartBuild(b,bid,searchForSpare: true,wantDemoUI: false);
									bc = city.GetBuildingCounts();
								}
							}
							{
								// why + 4?
								var bd = FindPendingOverlayBuildingsOfType(city,buildingLimit + 4 - bc.buildingCount,bidSorcTower,bidAcademy,bidBlacksmith,bidShipyard,bidTrainingGround,bidStable);
								if(bd.Any())
								{


									message += $"Adding { bd.Count} Military buildings";
									foreach(var i in bd)
									{

										var xx = await CheckMoveSlots();
										if(xx == -1)
										{
											return false;
										}

										if(xx == 0)
											break;


										if(bc.buildingCount >= buildingLimit)
											break;
										var bid = city.GetLayoutBid(i);
										//
										// these ones come last?
										// Assume either blacksmith, sorc tower or academy or stable
										if(bd.Count > 4 && (city.CountBuildingsInOverlay(bid) == 1)&&
												bid switch
												{
													bidAcademy or bidSorcTower or bidBlacksmith => true,
													_ => false
												})
										{
											continue;
										}
										if(await city.SmartBuild(i,bid,searchForSpare: true,wantDemoUI: null) == -1)
										{
											Note.Show("Something unusual happened");
											break;
										}
										bc = city.GetBuildingCounts();
									}
								}
							}
							{
								var bid = bidBarracks;
								var bd = FindPendingOverlayBuildingsOfType(city,buildingLimit,bid);  // find them all
								int milBid = bc.GetMainMilitaryBid();

								bd = bd.OrderByDescending((x) => GetBarrackScore(city,x,milBid)).ToList();
								if(bd.Any())
								{
									message += $"Adding Barracks";
									foreach(var i in bd)
									{
										if(bc.buildingCount >= buildingLimit)
											break;
										var xx = await CheckMoveSlots();
										if(xx == -1)
										{

											return false;
										}

										if(xx == 0)
											break;

										if(await city.SmartBuild(i,bid,searchForSpare: true,wantDemoUI: null) == -1)
										{
											Note.Show("Something unusual happened");
											break;
										}

										bc = city.GetBuildingCounts();
									}


								}
							}
							//	if(bc.buildingCount < buildingLimit)
							{
								// do the rest
								var todo = FindPendingOverlayBuildings(city);
								foreach(var c in todo)
								{
									if(bc.buildingCount >= buildingLimit)
										break;
									var xx = await CheckMoveSlots();
									if(xx == -1)
									{
										return false;
									}

									if(xx == 0)
										break;

									var bid = city.GetLayoutBid(c);
									if(await city.SmartBuild(IdToXY(c),bid,searchForSpare: true,wantDemoUI: null) == -1)
									{
										Note.Show("Something unusual happened");
										break;
									}
									bc = city.GetBuildingCounts();

								}

							}

						}
						else
						{
							Note.Show($"Main buildings still building (i.e. {QueueTab.FindFirstUnfinishedBuilding(city)})");
						}
						Assert(city.isBuild);

						break;
					}
				case City.BuildStage.mainBuildings:
				case BuildStage.kindOfDone:
				case City.BuildStage.preTeardown:
				case City.BuildStage.teardown:
					{
						if(city.buildStage == BuildStage.kindOfDone)
						{
							await city.MoveStuffLocked();
						}

						const int maxCommands = 15;
						if(bc.unfinishedBuildings > 4)
						{
							if(await AppS.DoYesNoBox($"{bc.unfinishedBuildings} unfinished buildings",
									$"continue with teardown anyway in {city.nameAndRemarks}?") != 1)
							{
								return true;
							}
						}

						var result = await AppS.DispatchOnUIThreadTask(async () =>
						{
							var panel = new StackPanel() { };
							panel.Children.Add(new TextBlock() { Text = "Swap cabins for buildings" });

							var combo = new ComboBox() { Header="Cabins to remove:",ItemsSource= cabinCounts };
							combo.SelectedIndex = Settings.cabinsToRemovePerSwap;
							//  var hasExtra = city.hasExtraBuildings;
							//   var removeOthers = hasExtra ? new CheckBox() { Header = "Extra buildings", OnContent = "Remove", OffContent = "Leave",Bind.IsChecked = Settings.demoUnwantedBuildingsWithCabins } : null; 

							panel.Children.Add(combo);
							//  if(hasExtra)
							//	   panel.Children.Add(removeOthers);

							var dialog = new ContentDialog()
							{
								Title = "Swap Cabins",
								Content =panel,


								PrimaryButtonText = "Yes",
								SecondaryButtonText = "No",
								CloseButtonText = "Cancel"
							};
							var res = (await dialog.ShowAsync2());
							if(res == ContentDialogResult.Primary)
							{
								//if(removeOthers!=null)
								// Settings.demoUnwantedBuildingsWithCabins = removeOthers.IsOn;
								var sel = combo.SelectedIndex;
								if(sel >= 0)
									Settings.cabinsToRemovePerSwap = cabinCounts[sel];
								//	Settings.SaveAll();
								return 1;
							}
							if(res == ContentDialogResult.Secondary)
							{
								return -1;
							}

							return 0;
						});
						if(result == -1)
							return false;
						if(result == 1)
						{

							var todo = FindPendingOverlayBuildings(city);
							int milBid = bc.GetMainMilitaryBid();
							var barracks = FindPendingOverlayBuildingsOfType(city,100,bidBarracks).OrderByDescending(a => GetBarrackScore(city,a,milBid)).ToList();
						//	var commandLimit = Settings.cabinsToRemovePerSwap * 2;
							if(!todo.Any())
							{
								// 
								Note.Show("No pending buildings?  That doesn't seem right..");
							}
							var todoGet = 0;
							for(int swap=0;swap<Settings.cabinsToRemovePerSwap;++swap)
							{
							//	var count = (commandLimit - city.GetBuildQueueLength());
								if(todoGet >= todo.Length)
									break;
								//var xx = await CheckMoveSlots();
								//if(xx == -1)
								//	return false;
								//if(xx == 0)
								//	break;

								var c = IdToXY(todo[todoGet++]);

								var bid = city.GetLayoutBid(c);
								if(bid == bidBarracks)
								{
									c = barracks[0];
									barracks.RemoveAt(0);
								}
								var delta = await city.SmartBuild(c,bid,searchForSpare: true,wantDemoUI: city.buildStage == BuildStage.kindOfDone ? true : null);
								if(delta == -1)
								{
									Note.Show("Something unusual happened");
									break;
								}
							}
							bc = city.GetBuildingCounts();
							Assert(city.isBuild);

						}
					}
					break;
				case City.BuildStage.complete:
					Note.Show("Complete :)");
					break;
				default:
					Assert(false);
					break;
			}
			if(bc.unfinishedTowerCount > 0)
			{
				foreach(var t in FindAllUnfinishedTowers(city))
				{
					await city.UpgradeToLevel(Settings.autoTowerLevel,t,false);
				}
			}
			if(bc.hasWall && bc.wallLevel < Settings.autoWallLevel && Settings.autoWallLevel != 10)
			{
				await city.UpgradeToLevel(Settings.autoWallLevel,IdToXY(bspotWall),false);

			}
			city.OnPropertyChanged();
			//	BuildTab.GetBuildInfo();
			if(Settings.clearRes && !city.leaveMe)
			{
				await city.ClearResUI();
			}
		}
		catch(Exception _ex)
		{
			LogEx(_ex);

		}
		finally 
		{
			city.BuildOpEnd();
		}
			return true;

		}

	static int nextMoveConfirm;
	static void InitNextMoveConfirm() => nextMoveConfirm = Player.moveSlots - CityBuild.movesPerConfirm;

	static async Task<int> CheckMoveSlots()
	{
		if(Player.moveSlots > nextMoveConfirm)
			return 1;
		InitNextMoveConfirm();
		return await AppS.DoYesNoBox("Move Slot Check",$"{Player.moveSlots} move slots left, continue?");
	}


}

