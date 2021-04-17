using COTG.Game;
using COTG.JSON;
using COTG.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI.Xaml.Controls;
using COTG.Views;
//using Windows.UI.Xaml.Media;

using static COTG.Debug;
using static COTG.Game.City;
using static COTG.Views.CityBuild;
using static COTG.Views.ShellPage;
using static COTG.Views.QueueTab;

namespace COTG.Views
{


	public sealed partial class QueueTab : UserTab, INotifyPropertyChanged
	{

		public static QueueTab instance;

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public static bool IsVisible() => instance.isVisible;


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
		public ObservableCollection<BuildItemView> cities { get; set; } = new ObservableCollection<BuildItemView>();


		public static void AddOp(BuildQueueItem item, int cid)
		{
			if (!IsVisible())
			{
				return;
			}

			App.DispatchOnUIThreadSneakyLow(() =>
			{
				BuildItemView view = null;
				foreach (var c in instance.cities)
				{
					if (c.cid == cid)
					{
						view = c;
						break;
					}
				}
				if (view == null)
				{
					view = BuildItemView.Rent().Ctor(cid);
					instance.cities.Add(view);
				}
				view.queue.Add(BuildItemView.Rent().Ctor(item, cid));
			});
		}

		public static void Clear(int cid)
		{
			App.DispatchOnUIThreadSneakyLow(() =>
			{
				foreach (var c in instance.cities)
				{
					if (c.cid == cid)
					{

						instance.cities.Remove(c);
						c.queue.Clear();
						break;
					}
				}
			});


		}


		public static void RemoveOp(BuildQueueItem item, int cid)
		{
			if (!IsVisible())
			{
				return;
			}

			App.DispatchOnUIThreadSneakyLow(() =>
			{
				BuildItemView view = null;
				foreach (var c in instance.cities)
				{
					if (c.cid == cid)
					{
						view = c;
						break;
					}
				}
				if (view == null)
				{
					return; // missing
				}

				int id = 0;
				foreach (var q in view.queue)
				{
					if ((q.item.bspot == item.bspot) && // is this enough for a match?
							(q.item.slvl == item.slvl))
					{

						view.queue.RemoveAt(id);
						break;
					}
					++id;
				}
				if (!view.queue.Any())
				{
					instance.cities.Remove(view);
				}
			});

		}
		public static async void RebuildAll()
		{
			var stage = await City.GetBuildBuildStage();
			App.DispatchOnUIThreadSneaky(() =>
			{
				instance.cities.Clear();
				instance.stage.Text = $"Stage: {stage.stage}";
				foreach (var city in CityBuildQueue.all.Values)
				{
					var view = BuildItemView.Rent().Ctor(city.cid);
					instance.cities.Add(view);
					foreach (var q in city.queue)
					{
						view.queue.Add(BuildItemView.Rent().Ctor(q, city.cid));
					}

				}
			});
		}
		override public async void VisibilityChanged(bool visible)
		{
			//   Log("Vis change" + visible);

			if (visible)
			{
				RebuildAll();
			}
			else
			{
				App.DispatchOnUIThreadSneaky(cities.Clear);
			}
			base.VisibilityChanged(visible);

		}

		//private void ItemClick(object sender, ItemClickEventArgs e)
		//{
		//	var i = (e.ClickedItem as BuildItemView);
		//	BuildQueue.CancelBuildOp(i.cid, i.item);


		//}

		private void ClearQueue(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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

		private void ClearSelected(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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
				BuildQueue.CancelBuildOp(op.cid, op.item); ;
			}
		}

		private void zoom_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
		{
			var ob = args.InvokedItem;
			if (ob is BuildItemView q)
			{
				JSClient.ChangeCity(q.cid, false); // this is always true now
			}
			else if (ob is BuildItemView op)
			{

			}
		}

		private void DoTheStuff(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			City.GetBuild().DoTheStuff();

		}


		
		public static async Task<bool> DoTheStuff(City city,bool allowRename, bool allowSetLayout)
		{
			var cid = city.cid;
			Assert(city.isBuild);
			if (ShellPage.viewMode != ViewMode.city)
				JSClient.ChangeView(ViewMode.city);
			CityBuild._isPlanner = false;

			Assert(App.uiSema.CurrentCount == 0);
			Assert(App.IsOnUIThread());
			await GetCity.Post(cid);
			if (city.LeaveMe())
			{
				Note.Show($"Skipping ${city.nameMarkdown}, 'LeaveMe' tag is set");
				return true;
			}
			var cabinLevel = city.GetAutobuildCabinLevelNoFetch();
			var bc = City.GetBuildingCountPostQueue(cabinLevel);
			var stage = city.GetBuildStage(bc);

			if (stage.stage == BuildStage._new && allowRename)
			{

				//				if(!city.isBuild)
				//				//					await JSClient.ChangeCity(city.cid, false);

				stage = city.GetBuildStage(bc);
			}
			Assert(city.isBuild);

			if (stage.stage == BuildStage.noLayout && allowSetLayout )
			{
				//				if (!city.isBuild)
				//				await JSClient.ChangeCity(city.cid, false);
				await ShareString.ShowNoLock();
				stage = city.GetBuildStage(bc);
				if (stage.stage == BuildStage.noLayout)
					return false;
			}
			Assert(city.isBuild);
			if (stage.stage == City.BuildStage.complete)
			{
				Note.Show($"Complete: {city}");
				return true;
			}
			Assert(city.isBuild);
			if (stage.stage == City.BuildStage.cabins || stage.stage == BuildStage.townHall)
			{
				if (bc.cabins >= SettingsPage.startCabinCount || bc.buildings >= stage.buildingLimit - 2)
				{
					Note.Show($"Building {stage.stage.ToString()} - {city}");
					return true;
				}
			}

			Assert(city.isBuild);
			if (!bc.hasCastle  && city.tsTotal > SettingsPage.tsForCastle && city.hasCastleInLayout)
			{
				//			if (!city.isBuild)
				//				await JSClient.ChangeCity(city.cid, false); ;
				//				if(!bc.hasWall)
				//					await CityBuild.Enqueue(0, 1, bidWall, bspotWall);
				await CityBuild.Enqueue(0, 1, bidCastle, XYToId(city.FindOverlayBuildingOfType(bidCastle)));
				//bc.wallLevel = 1;
				bc.hasCastle = true;
			}
			if ((bc.sorcTowers == 0 || bc.sorcTowerLevel != 10) && city.tsTotal > SettingsPage.tsForSorcTower && city.HasOverlayBuildingOfType( bidSorcTower) )
			{
				var c = FindValidBuildingOfType(city, bidSorcTower);

				if (c.bl == 0)
				{
					c.bl = 1;
					await CityBuild.Enqueue(0, 1, bidSorcTower, XYToId((c.x, c.y)));
				}
				// raise to level 10
				await CityBuild.EnqueueUpgrade( 10, XYToId((c.x, c.y)));
			}
			Assert(city.isBuild);
			if (!bc.hasWall && bc.hasCastle)
			{
				await CityBuild.Enqueue(0, 1, bidWall, bspotWall);
				bc.wallLevel = 1;
			}
			Assert(city.isBuild);
			if (bc.hasWall && bc.scoutpostCount < SettingsPage.scoutpostCount)
			{
				while (bc.scoutpostCount < SettingsPage.scoutpostCount)
				{
					var spot = 0;
					foreach (var _spot in innerTowerSpots)
					{
						if (CityBuild.postQueueBuildings[_spot].isEmpty)
						{
							spot = _spot;
							goto added;
						}

					}
					foreach (var _spot in outerTowerSpots)
					{
						if (CityBuild.postQueueBuildings[_spot].isEmpty)
						{
							spot = _spot;
							goto added;
						}

					}

				added:
					if (spot == 0)
						break;
					await CityBuild.Enqueue(0, 1, bidSentinelPost, spot);

					++bc.scoutpostCount;
				}
			}

			// todo: teardown
			//	await JSClient.ChangeCity(city.cid,false);
			bc = City.GetBuildingCountsPostQueue(cabinLevel);
			stage = await City.GetBuildBuildStage(bc);

			switch (stage.stage)
			{
				case City.BuildStage.noLayout:
				case City.BuildStage._new:
				case City.BuildStage.setup:
				case City.BuildStage.townHall:
				case City.BuildStage.cabins:
					{
						if (bc.cabins < SettingsPage.startCabinCount || (bc.storeHouses == 0))
						{
							switch (await App.DoYesNoBox("Add Cabins", $"Would you like to add cabins to {city.nameAndRemarks}?"))
							{
								case -1: return false;
								case 0: return true;
							}

							string message = "Cabins are building, please come back later";

							if (bc.townHallLevel < 4)
							{
								await EnqueueUpgrade(4, bspotTownHall);
							}

							if (bc.cabins < SettingsPage.startCabinCount)
							{
								message = $"Adding {SettingsPage.startCabinCount - bc.cabins}";
								// find a good spot for a cabin
								for (var y = span0; y <= span1; ++y)
								{
									for (var x = span0; x <= span1; ++x)
									{
										var c = (x, y);// (int x, int y) c = RandCitySpot();
										if (!IsBuildingSpot(c))
										{
											continue;
										}
										if (bc.buildings >= stage.buildingLimit)
											goto done;
										if (CityBuild.postQueueBuildings[City.XYToId(c)].isEmpty && (city.BidFromOverlay(c) == 0))
										{
											await CityBuild.Build(XYToId(c), bidCottage, false);
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
							var storeHouses = FindPendingOverlayBuildingsOfType(city, bidStorehouse, SettingsPage.intialStorehouses-bc.storeHouses);
							foreach( var storage in storeHouses)
							{
								
									message += $"Adding Storehouse";
									await CityBuild.SmartBuild(city, storage, bidStorehouse, true, false);
									++bc.storeHouses;
								

							}

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
						var buildingLimit = stage.buildingLimit;// !city.hasCastleInLayout ? 100 : bc.hasCastle ? 100 : 99;

						if (bc.buildings < buildingLimit)
						{
							switch (await App.DoYesNoBox("Building Placement", $"Would you like to place buildings for {city.nameAndRemarks}?"))
							{
								case -1: return false;
								case 0: return true;
							}

							if (bc.storeHouses == 0)
							{
								var storage = FindPendingOverlayBuildingsOfType(city, bidStorehouse, 1);
								if (storage.Any())
								{
									message += $"Adding Storehouse";
									await CityBuild.SmartBuild(city, storage.First(), bidStorehouse, true, false);
									bc = City.GetBuildingCountsPostQueue(cabinLevel);
								}
							}
							if (bc.forums == 0 && bc.buildings < buildingLimit)
							{
								var bd = FindPendingOverlayBuildingsOfType(city, bidMarketplace, 1);
								if (bd.Any())
								{
									message += $"Adding Forum";
									await CityBuild.SmartBuild(city, bd.First(), bidMarketplace, true, false);
									bc = City.GetBuildingCountsPostQueue(cabinLevel);
								}
							}
							{
								var bd = FindPendingOverlayBuildingsOfType(city, buildingLimit + 4 - bc.buildings, bidSorcTower, bidAcademy, bidBlacksmith, bidShipyard, bidTrainingGround, bidStable);
								if (bd.Any())
								{
									message += $"Adding { bd.Count} Military buildings";
									foreach (var i in bd)
									{
										if (bc.buildings >= buildingLimit)
											break;
										var bid = city.BidFromOverlay(i);
										if (city.CountBuildingsInOverlay(bid) == 1)
										{
											continue;
										}
										await CityBuild.SmartBuild(city, i, bid, true, false);
										bc = City.GetBuildingCountsPostQueue(cabinLevel);
									}
								}
							}
							{
								var bid = bidBarracks;
								var bd = FindPendingOverlayBuildingsOfType(city, bid, buildingLimit);  // find them all
								int milBid;
								if (bc.training > bc.academies && bc.training > bc.sorcTowers && bc.training >= bc.stables)
								{
									milBid = bidTrainingGround;
								}
								else if (bc.academies > bc.sorcTowers && bc.academies >= bc.stables)
									milBid = bidAcademy;
								else if (bc.sorcTowers >= bc.stables)
									milBid = bidSorcTower;
								else
									milBid = bc.stables;

								bd = bd.OrderByDescending((x) => CountSurroundingBuildingsOfType(x, milBid)).ToList();
								if (bd.Any())
								{
									message += $"Adding Barracks";
									foreach (var i in bd)
									{
										if (bc.buildings >= buildingLimit)
											break;
										await CityBuild.SmartBuild(city, i, bid, true, false);

										bc = City.GetBuildingCountsPostQueue(cabinLevel);
									}


								}
							}
							if (bc.buildings < buildingLimit)
							{
								// do the rest
								var todo = FindPendingOverlayBuildings(city);
								while (todo.Any() && bc.buildings < buildingLimit)
								{
									var id = AMath.random.Next(todo.Count);
									var c = todo[id];
									todo.RemoveAt(id);

									var bid = city.BidFromOverlay(c);
									await CityBuild.SmartBuild(city, c, bid, true, false);
									bc = City.GetBuildingCountsPostQueue(cabinLevel);

								}

							}

						}
						Assert(city.isBuild);

						break;
					}
				case City.BuildStage.preTeardown:
				case City.BuildStage.teardown:
					{
						const int maxCommands = 14;
						int count = (maxCommands - GetBuildQueueLength()).Min(bc.cabins * 2);
						if (count < 2 || bc.unfinishedBuildings > 0)
						{
							Note.Show("Already doing teardown or finishing up buildings");
							break;
						}
						switch (await App.DoYesNoBox("Teardown", $"Would you like to swap cabins for buildings in {city.nameAndRemarks}?"))
						{
							case -1: return false;
							case 0: return true;
						}


						var todo = FindPendingOverlayBuildings(city);
						int milBid;
						if (bc.training > bc.academies && bc.training > bc.sorcTowers && bc.training >= bc.stables)
						{
							milBid = bidTrainingGround;
						}
						else if (bc.academies > bc.sorcTowers && bc.academies >= bc.stables)
							milBid = bidAcademy;
						else if (bc.sorcTowers >= bc.stables)
							milBid = bidSorcTower;
						else
							milBid = bc.stables;

						var barracks = FindPendingOverlayBuildingsOfType(city, bidBarracks, 100).OrderByDescending(a => CountSurroundingBuildingsOfType(a, milBid)).ToList();

						for (; ; )
						{
							count = (maxCommands - GetBuildQueueLength());
							if (count < 2 || todo.Count == 0 || bc.cabins == 0)
								break;
							var id = AMath.random.Next(todo.Count);
							var c = todo[id];
							todo.RemoveAt(id);

							var bid = city.BidFromOverlay(c);
							if (bid == bidBarracks)
							{
								c = barracks[0];
								barracks.RemoveAt(0);
							}
							count -= await CityBuild.SmartBuild(city, c, bid, true, false);
							bc = City.GetBuildingCountsPostQueue(cabinLevel);
						}
						Assert(city.isBuild);


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
				foreach (var t in FindAllUnfinishedTowers())
				{
					await CityBuild.UpgradeToLevel(SettingsPage.autoTowerLevel, t, false);
				}
			}
			if (bc.hasWall && bc.wallLevel < SettingsPage.autoWallLevel && SettingsPage.autoWallLevel != 10)
			{
				await CityBuild.UpgradeToLevel(SettingsPage.autoWallLevel, IdToXY(bspotWall), false);

			}
			city.NotifyChange();
			BuildTab.GetBuildInfo();
			return true;
		}
		static int CountSurroundingBuildingsOfType((int x, int y) c, int bid)
		{
			int rv = 0;
			for (int y = -1; y <= 1; ++y)
				for (int x = -1; x <= 1; ++x)
				{
					if (x == 0 && y == 0)
						continue;
					var c1 = (c.x + x, c.y + y);
					if (c1.IsInCity())
					{
						if (CityBuild.postQueueBuildings[City.XYToId(c1)].bid == bid)
							++rv;
					}
				}

			return rv;
		}
		// starts ate center and searches outwards
		static List<(int x, int y)> FindPendingOverlayBuildings(City city)
		{
			List<(int x, int y)> rv = new();
			for (var cy = span0; cy <= span1; ++cy)
			{
				for (var cx = span0; cx <= span1; ++cx)
				{
					var c = (cx, cy);
					var id = XYToId(c);
					if (!IsBuildingSpot(id))
						continue;

					var bid = city.BidFromOverlay(id);
					if (bid switch { bidCottage or bidWall or bidTownHall or (>= bidResStart and <= bidResEnd) => true, _ => false })
					{
						continue;
					}
					if ((bid != 0) && (CityBuild.postQueueBuildings[id].bid != bid))
					{
						rv.Add(c);

					}
				}
			}
			return rv;
		}
		private async void MoveStuff(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{

			var cid = City.build;
			var city = GetBuild();
			Note.Show($"Move slots: {Player.moveSlots}");

			await GetCity.Post(city.cid);
			if (!await App.LockUiSema(cid))
				return;

			try
			{
				if (await App.DoYesNoBox("Move Stuff", "This can use a lot of move slots, are you sure?") != 1)
					return;

				for (; ; )
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
										var bid = city.BidFromOverlay(c);
										if (bid == 0)
											continue;
										var id = XYToId(c);
										var pbid = CityBuild.postQueueBuildings[id].bid;
										if (pbid == bid)
											continue;
										if (pbid == 0)
										{
											var spare = FindSpare(bid, false);
											if (spare != 0)
											{
												await MoveBuilding(spare, id, false);
												if (Player.moveSlots <= 10)
													goto done;
												hasChanges = true;
											}
										}
										else if (pass == 1)
										{
											var spare = CityBuild.FindAnyFreeSpot();
											await MoveBuilding(id, spare, false);
											if (Player.moveSlots <= 10)
												goto done;


											hasChanges = true;
											break;

										}

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
			done:;
			}
			finally
			{
				App.ReleaseUISema(cid);

			}
			Note.Show($"Final Move slots: {Player.moveSlots}");

		}
		static List<(int x, int y)> FindOverlayBuildingsOfType(City city, int bid)
		{
			List<(int x, int y)> rv = new();
			for (var cy = span0; cy <= span1; ++cy)
			{
				for (var cx = span0; cx <= span1; ++cx)
				{
					var c = (cx, cy);
					if (bid == city.BidFromOverlay(c))
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
					var b = CityBuild.postQueueBuildings[XYToId(c)];
					if (b.bid == bid)
					{
						if (b.bl > rv.bl)
							rv = (cx, cy, b.bl);
					}
					else if (rv.bl == 0 && bid == city.BidFromOverlay(c))
					{
						rv = (cx, cy, 0);

					}
				}
			}
			return rv;
		}

		static List<(int x, int y)> FindPendingOverlayBuildingsOfType(City city, int bid, int count)
		{
			List<(int x, int y)> rv = new();
			if (count == 0)
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
							if ((city.BidFromOverlay(c) == bid) && (CityBuild.postQueueBuildings[City.XYToId(c)].bid != bid))
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
			if (count == 0)
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
							var bid = city.BidFromOverlay(c);
							if (bid == 0)
								continue;
							if (bids.Contains(bid) && (CityBuild.postQueueBuildings[City.XYToId(c)].bid != bid))
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
		static List<(int x, int y)> FindAllUnfinishedTowers()
		{
			List<(int x, int y)> rv = new();

			// search from center outwards
			for (var cy = span0; cy <= span1; ++cy)
			{
				for (var cx = span0; cx <= span1; ++cx)
				{
					var c = (cx, cy);// (int x, int y) c = RandCitySpot();
					var b = CityBuild.postQueueBuildings[City.XYToId(c)];
					if (b.isTower && b.bl < SettingsPage.autoTowerLevel)
					{
						rv.Add(c);
					}
				}
			}
			return rv;
		}
		private static (int x, int y) RandomCitySpot()
		{
			return (x: AMath.random.Next(City.citySpan), y: AMath.random.Next(City.citySpan));
		}

		//private async void SplatAll(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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
		static readonly List<BuildItemView> pool = new();
		public const int size = 32;
		public Windows.UI.Xaml.Media.ImageBrush brush { get; set; }
		public int cid; // owner
		public BuildQueueItem item;
		public string building { get; set; }
		public string text { get; set; }
		public ObservableCollection<BuildItemView> queue { get; set; } = new ObservableCollection<BuildItemView>();
		public bool isCity => item.isNop;
		public bool isOp => !isCity;

		public BuildItemView Ctor(BuildQueueItem _item, int _cid)
		{
			cid = _cid;
			item = _item;
			brush = CityBuild.BuildingBrush(item.bid, size / 128.0f);
			var op = item.elvl == 0 ? "Destroy" : item.slvl == 0 ? "Build" : item.slvl > item.elvl ? "Downgrade" : "Upgrade";
			text = op + BuildingDef.all[item.bid].Bn;
			return this;
		}
		public BuildItemView Ctor(int _cid)
		{
			var city = City.GetOrAdd(_cid);
			cid = _cid;
			brush = CityBuild.BrushFromImage(city.icon);
			text = City.GetOrAdd(_cid).nameAndRemarks;
			item = BuildQueueItem.nop;
			return this;
		}
		public void Return()
		{
			text = null;
			brush = null;
			cid = 0;
			queue.Clear();
			pool.Add(this);
		}
		public static BuildItemView Rent()
		{
			if (pool.Any())
			{
				int i = pool.Count - 1;
				var rv = pool[i];
				pool.RemoveAt(i);
				return rv;
			}
			return new BuildItemView();

		}
	}

}
namespace COTG.Game
{

	public partial class City
	{
		public async Task<BuildInfo> GetBuildStage()
		{
			if (CityRename.IsNew(this))
				return new BuildInfo(BuildStage._new, 100);
			//await GetCity.Post(cid);
			var cabinLevel = await GetAutobuildCabinLevel();
			return GetBuildStage(GetBuildingCounts(cabinLevel));

		}
		bool NeedsSorcTower(BuildingCount bc)
		{
			return ((bc.sorcTowers == 0 || bc.sorcTowerLevel != 10) && tsTotal > SettingsPage.tsForSorcTower && HasOverlayBuildingOfType(bidSorcTower));
		}
		bool NeedsCastle(BuildingCount bc)
		{
			return ((!bc.hasCastle) && tsTotal > SettingsPage.tsForCastle && HasOverlayBuildingOfType(bidCastle));
		}

		public string buildStage
		{
			get
			{
				if (CityRename.IsNew(this))
					return "New";
				if (buildings == Emptybuildings)
				{
					//GetCity.Post(cid);
					return "pending...";
				}
				var bc = GetBuildingCounts(GetAutobuildCabinLevelNoFetch());
				if (NeedsSorcTower(bc))
					return $"WantSorc ({bc.buildings})";
				if (NeedsCastle(bc))
					return $"WantCastle ({bc.buildings})";

				return $"{GetBuildStage(bc).stage} ({bc.buildings})";
			}
		}

		public BuildInfo GetBuildStageNoFetch()
		{
			if (CityRename.IsNew(this))
				return new BuildInfo(BuildStage._new, 100);
			if (buildings == Emptybuildings)
			{
				//GetCity.Post(cid);
				return new BuildInfo(BuildStage.pending, 100);
			}
			return GetBuildStage(GetBuildingCounts(GetAutobuildCabinLevelNoFetch()));
		}

		public static async Task<BuildInfo> GetBuildBuildStage(BuildingCount bc)
		{
			var city = GetBuild();
			if (CityRename.IsNew(city))
				return new BuildInfo(BuildStage._new, 100);
		//	await GetCity.Post(City.build);

			return city.GetBuildStage(bc);
		}

		public static async Task<BuildInfo> GetBuildBuildStage()
		{
			var city = GetBuild();
			if (CityRename.IsNew(city))
				return new BuildInfo(BuildStage._new, 100);
			await GetCity.Post(City.build);
			var cabinLevel = await city.GetAutobuildCabinLevel();
			return city.GetBuildStage(GetBuildingCountPostQueue(cabinLevel));

		}

		public bool HasOverlayBuildingOfType(int bid)
		{
			if (!isLayoutValid)
				return false;

			for (var id = 0; id < City.citySpotCount; ++id)
			{
				if (bid == BidFromOverlay(id))
					return true;

			}
			return false;
		}
		public  (int x, int y) FindOverlayBuildingOfType( int bid)
		{
			if (!isLayoutValid)
				return (0,0);
			
			for (var id = 0;id<City.citySpotCount; ++id)
			{
					if (bid == BidFromOverlay(id))
						return IdToXY(id);

				}
			return (0, 0);
		}

		public bool hasCastleInLayout
		{
			get
			{
				if (!isLayoutValid)
					return false;

				for (int i = 0; i < citySpotCount; ++i)
				{
					if (BidFromOverlay(i) == bidCastle)
						return true;
				}
				return false;
			}
		}
		public (bool hasCastle, bool hasSorcTower) hasCastleOrSorcTowerInLayout
		{
			get
			{
				bool hasCastle = false;
				bool hasSorcTower = false;
				if (!isLayoutValid)
					return (false, false);

				for (int i = 0; i < citySpotCount; ++i)
				{
					if (BidFromOverlay(i) == bidCastle)
						hasCastle = true;
					else if (BidFromOverlay(i) == bidSorcTower)
						hasSorcTower = true;
				}
				return (hasCastle, hasSorcTower);
			}
		}

		public void ClearRes()
		{
			App.DispatchOnUIThreadExclusive(cid,async () =>
		   {
			  await ClearResUI();
		   });
		}
		public static async Task ClearResUI()
		{
			var city = GetBuild();

			if(city.isLayoutValid)
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
								if (!c.IsXYInCenter())
									continue;
								if (city.BidFromOverlay(c) == 0)
									continue;
								
								if (CityBuild.postQueueBuildings[City.XYToId(c)].isRes )
								{
									await Demolish(City.XYToId(c), false);
								}
							}
						}
					}
				}
			}
			else
			{
				await JSClient.ClearCenter(city.cid);
			}
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
