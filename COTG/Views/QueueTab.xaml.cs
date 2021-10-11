﻿using COTG.Game;
using COTG.JSON;
using COTG.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

//using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;
using COTG.Views;
//using Microsoft.UI.Xaml.Media;

using static COTG.Debug;
using static COTG.Game.City;
using static COTG.Views.CityBuild;
using static COTG.Views.ShellPage;
using static COTG.Views.QueueTab;
using EnumsNET;
using Microsoft.UI.Xaml;

namespace COTG.Views
{

	
	public sealed partial class QueueTab : UserTab
	{
		public const int movesPerConfirm = 8;


		static readonly int[] cabinCounts ={ 0,1, 2, 3, 4, 5, 6, 7, 8 };

		public static QueueTab instance;
		private const int badBuildingThreshold = 10;


		public static bool IsVisible() => instance.isFocused;


		public QueueTab()
		{
			Assert(instance == null);
			instance = this;
			this.InitializeComponent();
		}




		//private void ZoomedInGotFocus(object sender, RoutedEventArgs e)
		//{
		//	Log("in focus");
		//	zoom.StartBringIntoView();
		//	var groups = cvsGroups;
		//	var cur = groups.View.CurrentItem;
		//	Log(cur?.ToString());
		//}
		//private void ZoomedOutGotFocus(object sender, RoutedEventArgs e)
		//{
		//	Log("out focus");
		//	zoom.StartBringIntoView();
		//	var groups = cvsGroups;
		//	var cur = groups.View.CurrentItem;
		//	Log(cur?.ToString());
		//}
		public NotifyCollection<BuildItemView> cities {get;set;} = new();

		public static void AddOp(BuildQueueItem item, int cid)
		{
			if (!IsVisible())
			{
				return;
			}

	//		await App.DispatchOnUIThreadTask(() =>
			{
				BuildItemView view = null;
				foreach (var c in instance.cities)
				{
					if (c.cid == cid)
					{
						return;
					}
				}
				if (view == null)
				{
					instance.cities.Add(new BuildItemView(cid),true);
				}
				
			}//);
		}

		public static void Clear(int cid)
		{
			instance.cities.Remove( c=>c.cid == cid,true);
		}

		
		public static void RemoveOp( int cid)
		{
			if (!IsVisible())
			{
				return;
			}
			if(ExtendedQueue.TryGetBuildQueue(cid).Any() )
				return;
			foreach (var c in instance.cities)
			{
				if(c.cid == cid)
				{
					instance.cities.Remove(c,true);
					break;
				}
			}
		}
		public static void RebuildAll()
		{
			var build = City.GetBuild();
			//build.UpdateBuildStage();
			//instance.stage.Text = $"Stage: {build.buildStage.AsString()}";
			instance.cities.Set(ExtendedQueue.all.Values.Select(a=> new BuildItemView(a.cid)),true );
		}
		override public Task VisibilityChanged(bool visible, bool longTerm)
		{
			//   Log("Vis change" + visible);

			if (visible)
			{
				RebuildAll();
			}
			else
			{
				cities.Clear(true);
			}
			return base.VisibilityChanged(visible, longTerm: longTerm);

		}

		//private void ItemClick(object sender, ItemClickEventArgs e)
		//{
		//	var i = (e.ClickedItem as BuildItemView);
		//	BuildQueue.CancelBuildOp(i.cid, i.item);


		//}

		private void ClearQueue(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
		{
			CityBuild.ClearQueue();

		}




		//private void zoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
		//{
		//	var item = e.DestinationItem.Item as BuildItemView;
		//	if(item!=null)
		//	{
		//		JSClient.ChangeCity(item.cid, false);
		//		return;
		//	}
		//	var op = e.DestinationItem.Item as BuildItemView;
		//	if(op!=null)
		//	{
		//		JSClient.ChangeCity(op.cid, false);
		//		return;

		//	}

		//	item = e.SourceItem.Item as BuildItemView;
		//	if (item != null)
		//	{
		//		JSClient.ChangeCity(item.cid, false);
		//		return;
		//	}
		//	op = e.SourceItem.Item as BuildItemView;
		//	if (op != null)
		//	{
		//		JSClient.ChangeCity(op.cid, false);
		//		return;

		//	}

		//}

		private void ClearSelected(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
		{
			var sel = zoom.SelectedNodes;
			var removedCitites = new List<BuildItemView>();
			var removedOps = new List<BuildItemView>();

			/// collect all cities
			foreach (var i in sel)
			{
				if (i.Content is BuildItemView city && city.isCity)
				{
					removedCitites.Add(city);
				}
			}

			// collect op not part of removed cities
			foreach (var i in sel)
			{
				if (i.Content is BuildItemView op && op.isOp)
				{
					if (!removedCitites.Any(city => city.cid == op.cid))
					{
						removedOps.Add(op);
					}
				}
			}

			Note.Show($"Removed {removedOps.Count} build ops and {removedCitites.Count} city queues");
			// now remove
			foreach (var city in removedCitites)
			{
				BuildQueue.ClearQueue(city.cid);
			}
			foreach (var op in removedOps)
			{
				BuildQueue.CancelBuildOp(op.cid); ;
			}
		}
		private void zoom_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
		{
			var ob = args.InvokedItem;
			if (ob is BuildItemView q)
			{
				if(q.cid != City.build)
					JSClient.CitySwitch(q.cid, false, scrollIntoUI:false); // this is always true now
			}
		
		}

		private void DoTheStuff(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
		{
			City.GetBuild().DoTheStuff();

		}


		static int nextMoveConfirm;
		static void InitNextMoveConfirm() => nextMoveConfirm = Player.moveSlots - movesPerConfirm;

		static async Task<int> CheckMoveSlots()
		{
			if (Player.moveSlots > nextMoveConfirm)
				return 1;
			InitNextMoveConfirm();
			return await App.DoYesNoBox("Move Slot Check", $"{Player.moveSlots} move slots left, continue?");
		}

		public static async Task<bool> DoTheStuff(City city,bool allowRename, bool allowSetLayout)
		{

			InitNextMoveConfirm();

			var cid = city.cid;
			Assert(city.isBuild);
			if (ShellPage.viewMode != ViewMode.city)
				JSClient.ChangeView(ViewMode.city);
			await CityBuild._IsPlanner(false,true);
			
			Assert(App.uiSema.CurrentCount == 0);
			Assert(App.IsOnUIThread());
			await GetCity.Post(cid);
			if (city.leaveMe)
			{
				Note.Show($"Skipping ${city.nameMarkdown}, 'LeaveMe' tag is set");
				return true;
			}
			var bc=city.UpdateBuildStage();
			
			if (allowRename && (city.buildStage == BuildStage._new|| (city.autobuildCabinLevel==0&&(await App.DoYesNoBox("Autobuild Off?","Maybe you want Setup?")==1) )))
			{
				if (!await CityRename.RenameDialog(cid, false))
					return false;

				bc=city.UpdateBuildStage();
			}

			Assert(city.isBuild);

			if ( city.buildStage == BuildStage.noLayout && allowSetLayout )
			{
				//				if (!city.isBuild)
				//				await JSClient.ChangeCity(city.cid, false);
				await ShareString.Touch().ShowNoLock(City.build);
				bc=city.UpdateBuildStage();

				if (city.buildStage == BuildStage.noLayout)
					return false;
			}

			Assert(city.isBuild);
			if (city.buildStage == City.BuildStage.complete)
			{
				Note.Show($"Complete: {city}");
				return true;
			}
			var bad = CountBadBuildings(city);
			if (bad.isBad)
			{
				switch (await App.DoYesNoBox("Layout is not ideal", $"{bad.matches} matches, {bad.extraBuildings} extra or misplaced, {bad.missingOverlayBuildings} missing or out of place, Yes to Continue, No to cancel and change layout (or 'Use Buildings' in planner)", "Continue", "Layout..", "Cancel"))
				{
					case -1: return false;
					case 0:
						await ShareString.Touch().ShowNoLock(City.build);
						return false;
					default: break; // continue along
				}
			}


			Assert(city.isBuild);
			if(city.is7Point)
			{
				if (bc.townHallLevel < 8)
				{
					await city.EnqueueUpgrade(8, bspotTownHall);
					bc = city.UpdateBuildStage();
				}

			}
			if (city.buildStage == City.BuildStage.cabins || city.buildStage == BuildStage.townHall)
			{
				if (bc.cabins >= SettingsPage.startCabinCount || bc.buildingCount >= bc.buildingLimit - 2)
				{
					Note.Show($"Building {city.buildStage.AsString()} - {city}");
					return true;
				}
			}

			Assert(city.isBuild);
			if (!bc.hasCastle  && ((city.tsTotal > SettingsPage.tsForCastle)||(city.is7Point&&bc.townHallLevel>=8)) && city.hasCastleInLayout )
			{
				//			if (!city.isBuild)
				//				await JSClient.ChangeCity(city.cid, false); ;
				//				if(!bc.hasWall)
				//					await city.Enqueue(0, 1, bidWall, bspotWall);
				await city.SmartBuild( city.FindOverlayBuildingOfType(bidCastle), bidCastle, true, false, false);
				bc = city.UpdateBuildStage();
				//bc.wallLevel = 1;
				if (city.is7Point)
					await CitySettings.SetCitySettings(city.cid, autoBuildOn: false);

				if (city.is7Point && bc.hasCastle)
				{
					await city.DowngradeTo((0, 0), 1); // downgrade town hall
				}
			}
			if ((bc.sorcTowers == 0 || bc.sorcTowerLevel != 10) && (city.tsTotal > SettingsPage.tsForSorcTower || (!city.isMilitary && city.points> SettingsPage.scoreForSorcTower) ) && city.HasOverlayBuildingOfType( bidSorcTower) )
			{
				var c = FindValidBuildingOfType(city, bidSorcTower);

				if (c.bl == 0)
				{
					if (await city.SmartBuild( (c.x, c.y), bidSorcTower, true, false, false) != -1)
						c.bl = 1;



				}
				// raise to level 10
				if(c.bl != 0) // did it work?
					await city.EnqueueUpgrade( 10, XYToId((c.x, c.y)));
			}
			Assert(city.isBuild);
			if (!bc.hasWall && bc.hasCastle && !city.is7Point)
			{
				await city.Enqueue(0, 1, bidWall, bspotWall);
				bc.wallLevel = 1;
			}
			Assert(city.isBuild);
			if (bc.hasWall && bc.scoutpostCount < SettingsPage.scoutpostCount && !city.is7Point)
			{
				while (bc.scoutpostCount < SettingsPage.scoutpostCount)
				{
					var spot = 0;
					foreach (var _spot in innerTowerSpots)
					{
						if (city.postQueueBuildings[_spot].isEmpty)
						{
							spot = _spot;
							goto added;
						}

					}
					foreach (var _spot in outerTowerSpots)
					{
						if (city.postQueueBuildings[_spot].isEmpty)
						{
							spot = _spot;
							goto added;
						}

					}

				added:
					if (spot == 0)
						break;
					await city.Enqueue(0, 1, bidSentinelPost, spot);

					++bc.scoutpostCount;
				}
			}

			// todo: teardown
			//	await JSClient.ChangeCity(city.cid,false);
			bc=city.UpdateBuildStage();
			

			switch (city.buildStage)
			{
				case City.BuildStage.noLayout:
				case City.BuildStage._new:
				case City.BuildStage.setup:
				case City.BuildStage.townHall:
				case City.BuildStage.cabins:
					{
						if ( (bc.cabins < SettingsPage.startCabinCount || (bc.storeHouses == 0)) && !city.is7Point )
						{
							switch (await App.DoYesNoBox("Add Cabins", $"Would you like to add cabins to {city.nameAndRemarks}?"))
							{
								case -1: return false;
								case 0: return true;
							}

							string message = "Cabins are building, please come back later";

							if (bc.townHallLevel < 4)
							{
								await city.EnqueueUpgrade( 4, bspotTownHall);
							}
							var storeHouses = FindPendingOverlayBuildingsOfType(city,SettingsPage.intialStorehouses - bc.storeHouses, bidStorehouse, true);
							foreach (var storage in storeHouses)
							{

								message += $"Adding Storehouse";
								await city.SmartBuild( storage, bidStorehouse, true, false);
								++bc.storeHouses;


							}
							if (bc.cabins < SettingsPage.startCabinCount)
							{
								message = $"Adding {SettingsPage.startCabinCount - bc.cabins}";
								var cabinsInLayout = FindPendingOverlayBuildingsOfType(city, 100, bidCottage);

								// find a good spot for a cabin
								for (var y = span0; y <= span1; ++y)
								{
									for (var x = span0; x <= span1; ++x)
									{
										bool hasCabinsInLayout = cabinsInLayout.Any();
										
										var c = hasCabinsInLayout ?  cabinsInLayout.First(): (x, y);// (int x, int y) c = RandCitySpot();
										if(hasCabinsInLayout)
										{
											cabinsInLayout.RemoveAt(0);
										}

										if (!IsBuildingSpot(c))
										{
											continue;
										}
										

										if (bc.buildingCount >= bc.buildingLimit)
											goto done;
										if (city.postQueueBuildings[City.XYToId(c)].isEmpty && (city.GetLayoutBid(c) == 0))
										{
											await city.Build(XYToId(c), bidCottage, false,false);
											++bc.cabins;
										}
										if (bc.cabins >= SettingsPage.startCabinCount)
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
							Note.Show("Cabins are building, please come back later");
						}
					}
					break;
				case BuildStage.cabinsDone:
				case City.BuildStage.mainBuildings:
					{
						//var c = RandomCitySpot();
						var message = string.Empty;
						var buildingLimit = bc.buildingLimit;// !city.hasCastleInLayout ? 100 : bc.hasCastle ? 100 : 99;

						if (bc.buildingCount < buildingLimit)
						{
							switch (await App.DoYesNoBox("Building Placement", $"Would you like to place buildings for {city.nameAndRemarks}?"))
							{
								case -1: return false;
								case 0: return true;
							}

							if (bc.storeHouses < SettingsPage.intialStorehouses )
							{
								var storage = FindPendingOverlayBuildingsOfType(city, SettingsPage.intialStorehouses  - bc.storeHouses, bidStorehouse,true);
								foreach(var s in storage)
								{
									message += $"Adding Storehouse";
									await city.SmartBuild(s, bidStorehouse, true, false);
									bc = city.GetBuildingCounts();
								}
							}
							if (bc.forums == 0 && bc.ports==0 && bc.buildingCount < buildingLimit)
							{
								var bid = bidMarketplace;
								var bd = FindPendingOverlayBuildingsOfType(city, SettingsPage.intialMarkets, bid);
								if (!bd.Any())
								{
									bid = bidPort;
									bd = FindPendingOverlayBuildingsOfType(city, SettingsPage.intialMarkets, bid);
								}
								foreach(var b in bd)
								{
									if (bc.buildingCount >= buildingLimit)
										break;
									message += $"Adding Forum or Port";
									await city.SmartBuild(b, bid, true, false);
									bc = city.GetBuildingCounts();
								}
							}
							{
								// why + 4?
								var bd = FindPendingOverlayBuildingsOfType(city, buildingLimit + 4 - bc.buildingCount, bidSorcTower, bidAcademy, bidBlacksmith, bidShipyard, bidTrainingGround, bidStable);
								if (bd.Any())
								{


									message += $"Adding { bd.Count} Military buildings";
									foreach (var i in bd)
									{

										var xx = await CheckMoveSlots();
										if (xx == -1)
											return false;
										if (xx == 0)
											break;
									

										if (bc.buildingCount >= buildingLimit)
											break;
										var bid = city.GetLayoutBid(i);
										//
										// these ones come last?
										// Assume either blacksmith, sorc tower or academy or stable
										if (bd.Count > 4 && (city.CountBuildingsInOverlay(bid) == 1)&& 
												bid switch{ bidAcademy or bidSorcTower or bidBlacksmith  =>true,
																										_=>false } )
										{
											continue;
										}
										if (await city.SmartBuild( i, bid, true, false) == -1)
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
								var bd = FindPendingOverlayBuildingsOfType(city, buildingLimit, bid);  // find them all
								int milBid = bc.GetMainMilitaryBid();

								bd = bd.OrderByDescending((x) => GetBarrackScore(city,x, milBid)).ToList();
								if (bd.Any())
								{
									message += $"Adding Barracks";
									foreach (var i in bd)
									{
										if (bc.buildingCount >= buildingLimit)
											break;
										var xx = await CheckMoveSlots();
										if (xx == -1)
											return false;
										if (xx == 0)
											break;

										if (await city.SmartBuild( i, bid, true, false) == -1)
										{
											Note.Show("Something unusual happened");
											break;
										}

										bc = city.GetBuildingCounts();
									}


								}
							}
							if (bc.buildingCount < buildingLimit)
							{
								// do the rest
								var todo = FindPendingOverlayBuildings(city);
								foreach(var c in todo)
								{
									if (bc.buildingCount >= buildingLimit)
										break;
									var xx = await CheckMoveSlots();
									if (xx == -1)
										return false;
									if (xx == 0)
										break;

									var bid = city.GetLayoutBid(c);
									if( await city.SmartBuild( c, bid, true, false) == -1)
									{
										Note.Show("Something unusual happened");
										break;
									}
									bc = city.GetBuildingCounts();

								}

							}

						}
						Assert(city.isBuild);

						break;
					}
				case City.BuildStage.preTeardown:
				case City.BuildStage.teardown:
					{
						//if( WantMoveStuff() )
						//{
						//	await MoveStuffLocked();
						//}
		
						const int maxCommands = 15;
						int count = (maxCommands - city.GetBuildQueueLength()).Min(bc.cabins * 2);
						if (count < 2 || bc.unfinishedBuildings > 4)
						{
							Note.Show("Already doing teardown or finishing up buildings");
							break;
						}
						var result = await App.DispatchOnUIThreadTask(async () =>
					   {
						   var panel = new StackPanel() { };
						   panel.Children.Add(new TextBlock() { Text = "Swap cabins for buildings" });
						   
						   var combo = new ComboBox() {Header="Cabins to remove:", ItemsSource= cabinCounts };
						   combo.SelectedIndex = SettingsPage.cabinsToRemovePerSwap;
						   var hasExtra = city.hasExtraBuildings;
						   var removeOthers = hasExtra ? new ToggleSwitch() { Header = "Extra buildings", OnContent = "Remove", OffContent = "Leave", IsOn = SettingsPage.demoUnwantedBuildingsWithCabins } : null; 

						   panel.Children.Add( combo );
						   if(hasExtra)
							   panel.Children.Add(removeOthers);

						   var dialog = new ContentDialog()
						   {
							   Title = "Swap Cabins",
							   Content =panel,

							   
							   PrimaryButtonText = "Yes",
							   SecondaryButtonText = "No",
							   CloseButtonText = "Cancel"
						   };
						   var res = (await dialog.ShowAsync2());
						   if (res == ContentDialogResult.Primary)
						   {
							   if(removeOthers!=null)
								   SettingsPage.demoUnwantedBuildingsWithCabins = removeOthers.IsOn;
							   var sel = combo.SelectedIndex;
							   if (sel >= 0)
								   SettingsPage.cabinsToRemovePerSwap = cabinCounts[sel];
							   SettingsPage.SaveAll();
							   return 1;
						   }
						   if (res == ContentDialogResult.Secondary)
						   {
							   return -1;
						   }
						   
						   return 0;
					   });
						if (result == -1)
							return false;
						if (result == 1)
						{

							var todo = FindPendingOverlayBuildings(city);
							int milBid =  bc.GetMainMilitaryBid();
							var barracks = FindPendingOverlayBuildingsOfType(city, 100,bidBarracks).OrderByDescending(a => GetBarrackScore(city,a, milBid)).ToList();
							var commandLimit = SettingsPage.cabinsToRemovePerSwap * 2;
							var todoGet = 0;
							for (; ; )
							{
								count = (commandLimit - city.GetBuildQueueLength());
								if (count < 2 || todoGet >= todo.Length  || bc.cabins == 0)
									break;
								var xx = await CheckMoveSlots();
								if (xx == -1)
									return false;
								if (xx == 0)
									break;

								var c = todo[todoGet++];

								var bid = city.GetLayoutBid(c);
								if (bid == bidBarracks)
								{
									c = barracks[0];
									barracks.RemoveAt(0);
								}
								var delta= await city.SmartBuild( c, bid, true, false, SettingsPage.demoUnwantedBuildingsWithCabins);
								if(delta == -1)
								{
									Note.Show("Something unusual happened");
									break;
								}
								count -= count;
								bc = city.GetBuildingCounts();
							}
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
			if (bc.unfinishedTowerCount > 0)
			{
				foreach (var t in FindAllUnfinishedTowers(city))
				{
					await city.UpgradeToLevel(SettingsPage.autoTowerLevel, t, false);
				}
			}
			if (bc.hasWall && bc.wallLevel < SettingsPage.autoWallLevel && SettingsPage.autoWallLevel != 10)
			{
				await city.UpgradeToLevel(SettingsPage.autoWallLevel, IdToXY(bspotWall), false);

			}
			city.NotifyChange();
			BuildTab.GetBuildInfo();
			if(SettingsPage.clearRes && !city.leaveMe)
			{
				await city.ClearResUI();
			}
			return true;

		}
		static int GetBarrackScore(City city,(int x, int y) c, int bid)
		{
			int rv = -GetSpotCost(city,c);

			for (int y = -1; y <= 1; ++y)
				for (int x = -1; x <= 1; ++x)
				{
					if (x == 0 && y == 0)
						continue;
					var c1 = (c.x + x, c.y + y);
					if (c1.IsInCity())
					{
						if (city.postQueueBuildings[City.XYToId(c1)].bid == bid)
							rv += 4;
					}
				}

			return rv;
		}
		// starts ate center and searches outwards
		static int GetSpotCost(City city,(int x,int y) c)
		{
			var bl = city.postQueueBuildings[City.XYToId(c)];
			if (bl.isRes)
				return 1;
			else if (bl.isBuilding)
			    return 2;
			else
				return 0;

		}
		static (int x, int y)[] FindPendingOverlayBuildings(City city)
		{
			List<(int x, int y)> rv = new();
			for (int r = 1; r <= City.citySpan; ++r)
			{
				for (var y = -r; y <= r; ++y)
				{
					for (var x = -r; x <= r; ++x)
					{
						if ((x == -r || x == r) || (y == -r || y == r))
						{
							var c = (x, y);
							var id = XYToId(c);
							if (!IsBuildingSpot(id))
								continue;

							var bid = city.GetLayoutBid(id);
							if (bid switch { bidCottage or bidWall or bidTownHall or (>= bidResStart and <= bidResEnd) => true, _ => false })
							{
								continue;
							}
							if ((bid != 0) && (city.postQueueBuildings[id].bid != bid))
							{
								rv.Add(c);

							}
						}
					}
				}
			}
			return rv.OrderBy( (a) => GetSpotCost(city,a) ).ToArray();
		}
		public static (int matches,int missingOverlayBuildings,int extraBuildings, bool isBad) CountBadBuildings(City city)
		{
			int matches = 0;
			int missingOverlayBuildings = 0;
			int extraBuildings = 0;
			for (var y = span0; y <= span1; ++y)
			{
				for (var x = span0; x <= span1; ++x)
				{
					var c = (x, y);
					var id = XYToId(c);
					if (!IsBuildingSpot(id))
						continue;
					var pb = city.postQueueBuildings[id];
					var pbid = (!pb.isBuilding || pb.isCabin) ? 0 : pb.bid;
					var bid = city.GetLayoutBid(c);

					if (pbid == bid)
					{
						if( bid != 0)
							++matches;
						continue;
					}
					if (bid != 0)
						++missingOverlayBuildings;
					if (pbid != 0)
						++extraBuildings;
					
				}
			}
			return (matches,missingOverlayBuildings,extraBuildings, missingOverlayBuildings.Min(extraBuildings) > badBuildingThreshold);
		}
		
		private async void MoveStuff(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
		{
			var cid = City.build;
			if (!await App.LockUiSema(cid))
				return;
			try
			{
				await City.GetBuild().MoveStuffLocked();

			}
			finally
			{
				App.ReleaseUISema(cid);

			}
		}
		

		

		static List<(int x, int y)> FindOverlayBuildingsOfType(City city, int bid, int max= 100)
		{
			List<(int x, int y)> rv = new();
			for (var cy = span0; cy <= span1; ++cy)
			{
				for (var cx = span0; cx <= span1; ++cx)
				{
					if (rv.Count > max)
						return rv;

					var c = (cx, cy);
					if (bid == city.GetLayoutBid(c))
					{
						rv.Add(c);

					}
				}
			}
			return rv;
		}

		static (int x, int y, int bl) FindValidBuildingOfType(City city, int bid)
		{
			(int x, int y, int bl) rv = (0, 0, 0);
			for (var cy = span0; cy <= span1; ++cy)
			{
				for (var cx = span0; cx <= span1; ++cx)
				{
					var c = (cx, cy);
					var b = city.postQueueBuildings[XYToId(c)];
					if (b.bid == bid)
					{
						if (b.bl > rv.bl)
							rv = (cx, cy, b.bl);
					}
					else if (rv.bl == 0 && bid == city.GetLayoutBid(c))
					{
						rv = (cx, cy, 0);

					}
				}
			}
			return rv;
		}

		static List<(int x, int y)> FindPendingOverlayBuildingsOfType(City city, int count, int bid, bool addDummyIfNoLayout=false)
		{
			List<(int x, int y)> rv = new();
			if (count <= 0)
				goto done;
			if(!city.isLayoutCustom && addDummyIfNoLayout)
			{
				rv.Add(RandomSpotForBuilding(city));
				return rv;
			}

			// search from center outwards
			for (int r = 1; r <= City.citySpan; ++r)
			{
				for (var y = -r; y <= r; ++y)
				{
					for (var x = -r; x <= r; ++x)
					{
						if ((x == -r || x == r) || (y == -r || y == r))
						{
							var c = (x, y);// (int x, int y) c = RandCitySpot();
							if ((city.GetLayoutBid(c) == bid) && (city.postQueueBuildings[City.XYToId(c)].bid != bid))
							{
								rv.Add(c);
								if (rv.Count >= count)
									goto done;
							}
						}
					}
				}
			}
		done:
			return rv;
		}
		static List<(int x, int y)> FindPendingOverlayBuildingsOfType(City city, int count, params int[] bids)
		{
			List<(int x, int y)> rv = new();
			if (count <= 0)
				goto done;
			// search from center outwards
			for (int r = 1; r <= City.citySpan; ++r)
			{
				for (var y = -r; y <= r; ++y)
				{
					for (var x = -r; x <= r; ++x)
					{
						if ((x == -r || x == r) || (y == -r || y == r))
						{
							var c = (x, y);// (int x, int y) c = RandCitySpot();
							var bid = city.GetLayoutBid(c);
							if (bid == 0)
								continue;
							if (bids.Contains(bid) && (city.postQueueBuildings[City.XYToId(c)].bid != bid))
							{
								rv.Add(c);
								if (rv.Count >= count)
									goto done;
							}
						}
					}
				}
			}
		done:
			return rv;
		}
		static List<(int x, int y)> FindAllUnfinishedTowers(City city)
		{
			List<(int x, int y)> rv = new();

			// search from center outwards
			for (var cy = span0; cy <= span1; ++cy)
			{
				for (var cx = span0; cx <= span1; ++cx)
				{
					var c = (cx, cy);// (int x, int y) c = RandCitySpot();
					var b = city.postQueueBuildings[City.XYToId(c)];
					if (b.isTower && b.bl < SettingsPage.autoTowerLevel)
					{
						rv.Add(c);
					}
				}
			}
			return rv;
		}
		private static (int x, int y) RandomSpotForBuilding(City city) => IdToXY(city.FindFreeSpot());

		//private async void SplatAll(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
		//{
		//	if (App.uiSema.CurrentCount == 0)
		//	{
		//		Log("Already running");
		//		return;
		//	}

		//	try
		//	{
		//		foreach (var _city in City.friendCities)
		//		{

		//			var city = _city;
		//			var rv = await App.DispatchOnUIThreadExclusive(city.cid, async () =>
		//			 {
		//				 return await DoTheStuff(city);
		//			 });
		//			if (rv == false)
		//				break;
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		Log(ex);
		//		Note.Show("Did not complete all of the places, please try again");
		//	}

		//}




	}
	public class BuildItemView
	{
		public Microsoft.UI.Xaml.Media.ImageBrush brush { get; set; }
		public int cid; // owner
		public string text { get; set; }
		public BuildItem[] queue => ExtendedQueue.TryGetBuildQueue(cid).Select( a => new BuildItem(a) ).ToArray();
		public bool isCity => true;
		public bool isOp => false;

		public BuildItemView(int _cid)
		{
			var city = City.GetOrAdd(_cid);
			cid = _cid;
			brush = CityBuild.BrushFromImage(city.icon);
			text = City.GetOrAdd(_cid).nameAndRemarks;
		}
	}
	public class BuildItem
	{
		public const int size = 32;
		public Microsoft.UI.Xaml.Media.ImageBrush brush { get; set; }
		public string text { get; set; }

		public BuildItem(BuildQueueItem item)
		{
			brush = CityBuild.BuildingBrush(item.bid,size / 128.0f);
			var op = item.elvl == 0 ? "Destroy" : item.slvl == 0 ? "Build" : item.slvl > item.elvl ? "Downgrade" : "Upgrade";
			text = op + BuildingDef.all[item.bid].Bn;
		}
	}
	class BuildItemTemplateSelector:Microsoft.UI.Xaml.Controls.DataTemplateSelector
	{
		public DataTemplate buildItemViewTemplate { get; set; }
		public DataTemplate buildItemTemplate { get; set; }

		protected override DataTemplate SelectTemplateCore(object item)
		{
			return item is BuildItem ? buildItemTemplate : buildItemViewTemplate;
		}
	}

}
namespace COTG.Game
{

	public partial class City
	{
		public  bool leaveMe => HasTag(Tags.LeaveMe);

		//public async Task<BuildInfo> GetBuildStage()
		//{
		//	if(leaveMe)
		//		return new BuildInfo(BuildStage.leave, 100);
		//	if (CityRename.IsNew(this))
		//		return new BuildInfo(BuildStage._new, 100);
		//	//await GetCity.Post(cid);
		//	var cabinLevel = await GetAutobuildCabinLevel();
		//	return GetBuildStage(GetBuildingCounts(cabinLevel));

		//}
		public  bool NeedsSorcTower(BuildingCount bc)
		{
			return ((bc.sorcTowers == 0 || bc.sorcTowerLevel != 10) && tsTotal > SettingsPage.tsForSorcTower && HasOverlayBuildingOfType(bidSorcTower));
		}
		public  bool NeedsCastle(BuildingCount bc)
		{
			return ((!bc.hasCastle) && tsTotal > SettingsPage.tsForCastle && HasOverlayBuildingOfType(bidCastle));
		}
		public async Task MoveStuffLocked()
		{
			Note.Show($"Move slots: {Player.moveSlots}");


			var initialMoveSlots = Player.moveSlots;
			var nextMoveConfirm = initialMoveSlots - movesPerConfirm;

			var result = await App.DoYesNoBox("Move Stuff", "Whould you like to demo resources where buildings should go?", cancel: "Don't Move", no: "Move Stuff", yes: "Move+Demo");
			if (result == -1)
				return;
			var allowDemo = result == 1;

			for (int bad = 0; bad < 16; ++bad)
			{
				var hasChanges = false;
				for (int pass = 0; pass < 2; ++pass)
				{
					for (int r = 1; r <= City.citySpan; ++r)
					{
						for (var y = -r; y <= r; ++y)
						{
							for (var x = -r; x <= r; ++x)
							{
								if ((x == -r || x == r) || (y == -r || y == r))
								{
									var c = (x, y);
									var id = XYToId(c);
									if (!IsBuildingSpot(id))
										continue;
									if (HasBuildOps(id))
										continue;
									var bid = GetLayoutBid(id);
									if (bid == 0)
										continue;
									var bl = postQueueBuildings[id];
									var pbid = bl.bid;
									if (pbid == bid)
										continue;
									if (pbid == 0 || (bl.isRes && allowDemo))
									{
										var spare = FindSpare(bid, false);
										if (spare != 0)
										{
											if (bl.isRes)
											{
												await Demolish(c, false);
												await Task.Delay(500);
												hasChanges = true;
											}
											if (spare > 0)
											{
												if (!await MoveBuilding(spare, id, false))
												{
													goto error;
												}
												hasChanges = true;
												if (Player.moveSlots < nextMoveConfirm)
												{
													nextMoveConfirm = (Player.moveSlots - movesPerConfirm).Max(3);
													if (await App.DoYesNoBox("Move Stuff", $"{initialMoveSlots - Player.moveSlots} moves so far, {Player.moveSlots} moves left, continue?") != 1)
														goto done;
												}
											}
										}

									}
									//	else if (pass == 1)
									//	{
									//		var spare = CityBuild.FindAnyFreeSpot(id);
									//		if (!await MoveBuilding(id, spare, false)) 
									//		{

									//			goto error;
									//		}
									//		hasChanges = true;
									//		break;
									//}
								}


							}

						}
					}
					if (hasChanges)
						break;
				}
				if (!hasChanges)
					break;

			}

		done:

			Note.Show($"Final Move slots: {Player.moveSlots}");
			return;
		error:
			await App.DoYesNoBox("Move Stuff", "Failed, build operations in progress might be blocking?", "Okay", null);

		}
		public  string bStage
		{
			get
			{
				if (IsNew())
					return "New";
				if (buildings == Emptybuildings)
				{
					//GetCity.Post(cid);
					return "pending...";
				}
				if (wantSorcTower)
					return "WantSorc";
				if (wantCastle)
					return "WantCastle";

				return buildStage.AsString();
			}
		}

		//public BuildInfo GetBuildStageNoFetch()
		//{
		//	if (leaveMe)
		//		return new BuildInfo(BuildStage.leave, 100);
		//	if (CityRename.IsNew(this))
		//		return new BuildInfo(BuildStage._new, 100);
		//	if (buildings == Emptybuildings)
		//	{
		//		//GetCity.Post(cid);
		//		return new BuildInfo(BuildStage.pending, 100);
		//	}
		//	return GetBuildStage(GetBuildingCounts(GetAutobuildCabinLevelNoFetch()));
		//}

		//public static async Task<BuildInfo> GetBuildBuildStage(BuildingCount bc)
		//{
		//	var city = GetBuild();
		//	if (city.leaveMe)
		//		return new BuildInfo(BuildStage.leave, 100);
		//	if (CityRename.IsNew(city))
		//		return new BuildInfo(BuildStage._new, 100);
		////	await GetCity.Post(City.build);

		//	return city.GetBuildStage(bc);
		//}

		//public static async Task<BuildInfo> GetBuildBuildStage()
		//{
		//	var city = GetBuild();
		//	if (city.leaveMe)
		//		return new BuildInfo(BuildStage.leave, 100);

		//	if (CityRename.IsNew(city))
		//		return new BuildInfo(BuildStage._new, 100);
		//	await GetCity.Post(City.build);
		//	var cabinLevel = await city.GetAutobuildCabinLevel();
		//	return city.GetBuildStage(GetBuildingCountPostQueue(cabinLevel));

		//}

		public  bool HasOverlayBuildingOfType(int bid)
		{
			if (!isLayoutCustom)
				return false;

			for (var id = 0; id < City.citySpotCount; ++id)
			{
				if (bid == GetLayoutBid(id))
					return true;

			}
			return false;
		}
		public  (int x, int y) FindOverlayBuildingOfType( int bid)
		{
			if (!isLayoutCustom)
				return (0,0);
			
			for (var id = 0;id<City.citySpotCount; ++id)
			{
					if (bid == GetLayoutBid(id))
						return IdToXY(id);

				}
			return (0, 0);
		}
		public  bool hasExtraBuildings => FindExtraBuilding() != -1;

		public  int FindExtraBuilding()
		{
			if (!isLayoutCustom)
				return -1;
			Dictionary<short, short> counts = new();


			// first collect counts
			for (var id = 1; id < City.citySpotCount - 1; ++id)
			{
				if (!IsBuildingSpot(id))
					continue;

				var bid = (short)GetLayoutBid(id);
				if (bid != 0 && bid != bidTemple && bid != bidCastle)
				{
					if (counts.TryGetValue(bid, out var c) == false)
					{
						c = 1;
						counts.TryAdd(bid, c);
					}
					else
					{
						++c;
						counts[bid] = c;
					}
				}

				var bl = postQueueBuildings[id];
				if (!(bl.isRes || bl.isEmpty || bl.isTemple || bl.isCabin || bl.isTower))
				{
					bid = bl.bid;
					if (counts.TryGetValue(bid, out var c) == false)
					{
						c = -1;
						counts.TryAdd(bid, c);
					}
					else
					{
						--c;
						counts[bid] = c;
					}

				}
			}
			int rv = -1;
			int bestLevel = int.MaxValue;
			for (var id = 1; id < City.citySpotCount - 1; ++id)
			{
				if (!IsBuildingSpot(id))
					continue;

				var oBid = (short)GetLayoutBid(id);
				var bl = postQueueBuildings[id];
				if (!(bl.isRes || bl.isEmpty || bl.isTemple || bl.isCabin || bl.isTower))
				{
					var bid = (short)bl.bid;
					if (bid == oBid)
						continue;
					if (counts[bid] < 0)
					{
						if (bl.bl < bestLevel)
						{
							bestLevel = bl.bl;
							rv = id;
						}
					}


				}
			}
			return rv;
		}

		public bool hasCastleInLayout
		{
			get
			{
				if (!isLayoutCustom)
					return false;

				for (int i = 0; i < citySpotCount; ++i)
				{
					if (GetLayoutBid(i) == bidCastle)
						return true;
				}
				return false;
			}
		}
		public  (bool hasCastle, bool hasSorcTower) hasCastleOrSorcTowerInLayout
		{
			get
			{
				bool hasCastle = false;
				bool hasSorcTower = false;
				if (!isLayoutCustom)
					return (false, false);

				for (int i = 0; i < citySpotCount; ++i)
				{
					if (GetLayoutBid(i) == bidCastle)
						hasCastle = true;
					else if (GetLayoutBid(i) == bidSorcTower)
						hasSorcTower = true;
				}
				return (hasCastle, hasSorcTower);
			}
		}


		public  void ClearRes()
		{
			App.DispatchOnUIThreadExclusive(cid,async () =>
		   {
			  await ClearResUI();
		   });
		}
		public  async Task ClearResUI()
		{
			var asked = false;
			if(isLayoutCustom)
			{
				for (int r = 1; r <= City.citySpan; ++r)
				{
					for (var y = -r; y <= r; ++y)
					{
						for (var x = -r; x <= r; ++x)
						{
							if ((x == -r || x == r) || (y == -r || y == r))
							{
								var c = (x, y);// (int x, int y) c = RandCitySpot();
								if (!c.IsXYInCenter() && SettingsPage.clearOnlyCenterRes)
									continue;
								if (GetLayoutBid(c) == 0)
									continue;
								
								if (postQueueBuildings[City.XYToId(c)].isRes )
								{
									if(asked==false)
									{
										asked = true;
										if(await App.DoYesNoBox("Clear Res?","Would you like to clear out the resources?") != 1)
										{
											return;
										}
									}
									await Demolish(City.XYToId(c), false);
								}
							}
						}
					}
				}
			}
			else
			{
				await JSClient.ClearCenter(cid);
			}
		}

		public  bool IsLayoutComplete(City city)
		{
			try
			{
				if (!isLayoutCustom)
					return true;
				var bds = isBuild ? city.postQueueBuildings : buildings;
				for (var id = 1; id < City.citySpotCount; ++id)
				{
					if (!IsBuildingSpot(id))
						continue;

					var bid = GetLayoutBid(id);
					if (bid != 0)
					{
						var bl = bds[id];
						if (bl.bid != bid)
							return false;
					}

				}
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}

			return true;
		}

		internal  bool IsAllyOrNap()
		{
			return Alliance.IsAllyOrNap(allianceId);
		}
	}
	//public class BuildItemTemplateSelector : Microsoft.UI.Xaml.Controls.DataTemplateSelector

	//{
	//	public DataTemplate cityTemplate { get; set; }
	//	public DataTemplate opTemplate { get; set; }

	//	protected override DataTemplate SelectTemplateCore(object item)
	//	{
	//		if (item is BuildItemView city)
	//		{
	//			return cityTemplate;
	//		}
	//		else
	//		{
	//			return opTemplate;
	//		}
	//	}
	//}
	
}
