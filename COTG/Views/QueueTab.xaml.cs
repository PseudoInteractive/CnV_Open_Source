using COTG.Game;
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

	//	public static void AddOp(BuildQueueItem item, int cid)
	//	{
	//		if (!IsVisible())
	//		{
	//			return;
	//		}

	////		await App.DispatchOnUIThreadTask(() =>
	//		{
	//			BuildItemView view = null;
	//			foreach (var c in instance.cities)
	//			{
	//				if (c.cid == cid)
	//				{
	//					return;
	//				}
	//			}
	//			if (view == null)
	//			{
	//				instance.cities.Add(new BuildItemView(cid),true);
	//			}
				
	//		}//);
	//	}

		
		
		public static void RebuildAll()
		{
			App.DispatchOnUIThread(() =>
		   {
			   var build = City.GetBuild();
				//build.UpdateBuildStage();
				//instance.stage.Text = $"Stage: {build.buildStage.AsString()}";
				instance.zoom.ItemsSource = ExtendedQueue.all.ToArray().Select(a => new BuildItemView(a.Value.cid)).ToArray();
		   });
		}
		override public Task VisibilityChanged(bool visible, bool longTerm)
		{
			//   Log("Vis change" + visible);

			if (visible)
			{
				 RebuildAll ();
				
			}
			else
			{
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
			RebuildAll();
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

		private async void ClearSelected(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
		{
			var sel = zoom.SelectedNodes;
			var removedCitites = new List<BuildItemView>();
			var removedOps = new List<BuildItem>();

			/// collect all cities
			foreach (var i in sel)
			{
				if (i.Content is BuildItemView city )
				{
					removedCitites.Add(city);
				}
			}

			// collect op not part of removed cities
			foreach (var i in sel)
			{
				if (i.Content is BuildItem op )
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
				var _city = city;
				await BuildQueue.ClearQueue(_city.cid);
			}
			foreach (var _op in removedOps)
			{
				var op = _op;
				var cid = op.cid;
				if(ExtendedQueue.all.TryGetValue(cid,out var q))
				{

					using(var _lock = await q.queueLock.LockAsync())
					{
						q.queue.Remove(op.op);
					}
					City.Get(cid).BuildingsOrQueueChanged();
					BuildQueue.SaveNeeded();
				}

			}
			RebuildAll();
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


		
		
		internal static int GetBarrackScore(City city,(int x, int y) c, int bid)
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
		internal static int GetSpotCost(City city,(int x,int y) c)
		{
			var bl = city.postQueueBuildings[City.XYToId(c)];
			if (bl.isRes)
				return 1;
			else if (bl.isBuilding)
			    return 2;
			else
				return 0;

		}
		internal static(int x, int y)[] FindPendingOverlayBuildings(City city)
		{
			List<( (int x, int y) c, int bid)> rv = new();
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
								rv.Add((c, bid));

							}
						}
					}
				}
			}
			return  rv.OrderByDescending( a => BuildingDef.FromBid(a.bid).GetBuildTimeMeasure()*AMath.random.NextSingle() ).ThenBy(a=>GetSpotCost(city,a.c)).Select( a=> a.c ).ToArray();
		}
		internal static (int matches,int missingOverlayBuildings,int extraBuildings, bool isBad) CountBadBuildings(City city)
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

		internal async void MoveStuff(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
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





		internal static (int x, int y, int bl) FindValidBuildingOfType(City city, int bid)
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

		internal static List<(int x, int y)> FindPendingOverlayBuildingsOfType(City city, int count, int bid, bool addDummyIfNoLayout=false)
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
		internal static List<(int x, int y)> FindPendingOverlayBuildingsOfType(City city, int count, params int[] bids)
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
		internal static List<(int x, int y)> FindAllUnfinishedTowers(City city)
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
		internal static Building FindFirstUnfinishedBuilding(City city)
		{
			List<(int x, int y)> rv = new();

			// search from center outwards
			for(var cy = span0;cy <= span1;++cy)
			{
				for(var cx = span0;cx <= span1;++cx)
				{
					var c = (cx, cy);// (int x, int y) c = RandCitySpot();
					var b = city.postQueueBuildings[City.XYToId(c)];
					if(b.isBuilding && !b.isCabin && b.bl < 10 )
					{
						return b;
					}
				}
			}
			return default;
		}
		internal  static (int x, int y) RandomSpotForBuilding(City city) => IdToXY(city.FindFreeSpot());

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
		public BuildItem[] queue => ExtendedQueue.TryGetBuildQueue(cid).Select( a => new BuildItem(a,cid) ).ToArray();
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
		public BuildQueueItem op;
		public int cid;
		public BuildItem(BuildQueueItem item, int _cid)
		{
			cid = _cid;
			op = item;
			brush = CityBuild.BuildingBrush(item.bid,size / 128.0f);
			var desc = item.elvl == 0 ? "Destroy" : item.slvl == 0 ? "Build" : item.slvl > item.elvl ? "Downgrade" : "Upgrade";
			text = desc + BuildingDef.all[item.bid].Bn;
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
		internal List<(int x, int y)> FindOverlayBuildingsOfType(int bid,int max = 100)
		{
			List<(int x, int y)> rv = new();
			for(var cy = span0;cy <= span1;++cy)
			{
				for(var cx = span0;cx <= span1;++cx)
				{
					if(rv.Count > max)
						return rv;

					var c = (cx, cy);
					if(bid == this.GetLayoutBid(c))
					{
						rv.Add(c);

					}
				}
			}
			return rv;
		}

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
			await App.DoYesNoBox("Move Stuff", "Something did not move right.  Maybe a race condition?  Maybe try again to continue", "Okay", null);

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
			foreach(var id in buildingSpotsLand)
			{
			
				var bid = (short)GetLayoutBid(id);
				if (bid != 0 && bid != bidTemple )
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
				Assert(!bl.isTower);
				if (!(bl.isRes || bl.isEmpty || bl.isTemple || bl.isCabin || bl.isTower ))
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
				if (!(bl.isRes || bl.isEmpty || bl.isTemple || bl.isCabin || bl.isTower || bl.isCastle))
				{
					var bid = (short)bl.bid;
					if (bid == oBid)
						continue;
					if (counts[bid] < 0)
					{
						Assert(bid != bidCastle);
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
