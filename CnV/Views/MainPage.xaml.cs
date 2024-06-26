﻿using System.Runtime.CompilerServices;

using Microsoft.UI.Xaml.Controls;
//using Windows.UI.Core;
using Microsoft.UI.Xaml;
//using Windows.UI.Input;

namespace CnV.Views
{
	using Game;

	using Syncfusion.UI.Xaml.DataGrid;


	public sealed partial class MainPage:UserTab
	{
		public static MainPage instance;



		//        public static City showingRowDetails;

		//{
		//    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
		//    Assert(rv != null);
		//    return rv;
		//}
		public static SfDataGrid CityGrid => instance?.cityGrid;

		//  static MenuFlyout cityMenuFlyout;
		public MainPage() {
			//            var a = Telerik.UI.Xaml.Controls.Grid.Primitives.For
			Assert(instance == null);
			instance = this;
			InitializeComponent();

			//dungeonGrid.ProcessTooltips();


		}
		public override async Task Closed() {
			await base.Closed();
			DungeonView.Destroy();
			instance = null;
		}



		private void CityGrid_CurrentItemChanged(object sender,EventArgs e) {
			Log("Current item " + sender.ToString());


		}

		private void ColumnHeaderTap() {

		}


		//static SfDataGrid GetGrid(PointerRoutedEventArgs e)
		//{
		//	var a = e.OriginalSource as FrameworkElement;
		//	while(a != null)
		//	{
		//		if (a is DataGridCellsPanel panel)
		//			return panel.Owner;
		//		if (a is DataGridRootPanel root)
		//			return root.Owner;
		//		a = a.Parent as FrameworkElement; 
		//	}
		//	return null;
		//}


		public static List<int> GetContextCids(object sender) {
			var cid = (int)((sender as MenuFlyoutItem).DataContext);
			return Spot.GetSelectedForContextMenu(cid);
		}



		public static int GetContextCidCount(int focusCid) {
			return Spot.GetSelectedForContextMenu(focusCid).Count;

		}
		public static void ReturnSlowClick(int cid) {
			var cids = GetContextCids(cid);
			if(cids.Count == 1)
				Raiding.Return(cids[0],true,fast: false);
			else
				Raiding.ReturnBatch(cids,fast: false); ;
		}



		public static void ReturnFastClick(int cid) {
			var cids = GetContextCids(cid);
			if(cids.Count == 1)
				Raiding.Return(cids[0],true,fast: true);
			else
				Raiding.ReturnBatch(cids,fast: true);
		}




		//public string dungeonInfo { get
		//    {
		//        var ci = dungeonGrid.HitTestService.CellInfoFromPoint(CoreWindow.GetForCurrentThread().PointerPosition);
		//        if(ci == null)
		//            return "Missed";
		//        var i = ci.Item as COTG.Game.Dungeon;
		//        return i.ToString();
		//    }
		//}

		//private void OnCheckBoxClick(object sender, RoutedEventArgs e)
		//{
		//    var cb = (CheckBox)sender;
		//    var _showing = showingRowDetails;
		//    showingRowDetails = null;
		//    if (_showing != null && cities.Contains(_showing) )
		//        this.cityGrid.HideRowDetailsForItem(_showing);

		//    var newCheckedItem = (City)cb.DataContext;

		//    if (cb.IsChecked.HasValue && cb.IsChecked.Value)
		//    {
		//        showingRowDetails = newCheckedItem;
		//        this.cityGrid.ShowRowDetailsForItem(newCheckedItem);
		//    }

		//}




		//public static void CityListUpdateAll()
		//{
		//    if (instance == null)
		//        return;
		//    // Note.L("UpdateAll: ");
		//    instance.Dispatcher.(() =>
		//    {
		//        City.gridCitySource.NotifyReset();
		//        City.GetBuild()?.SelectInUI(true);
		//    });

		//}


		private void Set<T>(ref T storage,T value,[CallerMemberName] string propertyName = null) {
			if(Equals(storage,value)) {
				return;
			}
			storage = value;
			OnPropertyChanged(propertyName);
		}

		internal void UpdateRaidIncome() {

			ResourcesAndGold raidIncome = new(); // per hour
			var tsTotal = 0;
			var tsHome = 0;
			var onWater = 0;
			var castles = 0;
			//		if (jsd.RootElement.ValueKind == JsonValueKind.Object)
			//		{
			//			//           string dateExtra = DateTime.Now.Year
			//			var a = jsd.RootElement.GetProperty("a");
			foreach(var city in City.gridCitySource) {
				int cid = city.cid;
				if(city.isOnWater)
					++onWater;
				if(city.isCastle)
					++castles;
				tsTotal += city.tsTotal;
				tsHome += city.tsHome;
				//				var city = City.GetOrAddCity(cid);
				//				var raids = Array.Empty<Raid>();
				//				var minCarry = 255;

				foreach(var r in city.outgoing) {
					if(!r.isRaid)
						continue;
					var carry = r.troops.carry;
					var d = Cavern.Get(r.targetCid);
					//	Assert(d.valid);

					var maxLoot = d.GetLoot(r.splits,r.troops.carry);
					var loot = carry.Min((maxLoot));
					var res = new Resources(Troops.cavernLootRatios[(int)d.resId]*loot);
					var gold = (int)((loot * d.goldPercent)/100);
					var dt = r.currentJourneyTime * 2;
					var multiplier = ServerTime.secondsPerHour / (double)dt;
					raidIncome += new ResourcesAndGold(res*multiplier,(gold*multiplier).RoundToInt());
					//				foreach (var r in cr[12].EnumerateArray())
					//				{
					//					var target = r[8].GetInt32();
					//					var dateTime = r[7].GetString().ParseDateTime(false);

					//					if (raids.FindAndIncrement(target, dateTime))
					//					{
					//						rWood += tWood;
					//						rStone += tStone;
					//						rIron += tIron;
					//						rFood += tFood;
					//						rGold += tGold;
					//						continue;
					//					}
					//					string desc = r[2].GetString();
					//					//    Mountain Cavern, Level 4(91 %)
					//					var raid = new Raid();
					//					raid.repeatCount = 1;
					//					raid.target = target;
					//					raid.time = dateTime;
					//					var r4 = r[4].GetByte(); 
					//					raid.isReturning = r[3].GetInt32() != 0;
					//					raid.r4 = r4;
					//					//=  r4== 2 ||r4==3;
					//					//    Log(raid.ToString());
					//					// raid.arrival.Year = DateTime.Now.Year;
					//					var ss0 = desc.Split(',');
					//					Assert(ss0.Length == 2);
					//					var isMountain = ss0[0].Trim()[0] == 'M';
					//					var ss = ss0[1].Split(new char[] { ' ', '(', ',', '%' }, StringSplitOptions.RemoveEmptyEntries);
					//					Assert(ss.Length == 4);
					//					var level = int.Parse(ss[1]);
					//					var completion = int.Parse(ss[2]);
					//					var res = (isMountain ? mountainLoot[level - 1] : otherLoot[level - 1]) * (2 - completion * 0.01f);
					//					int cc = 0;

					//					// slowest
					//					var maxTravel = 0.0;
					//					foreach (var ttr in r[5].EnumerateArray())
					//					{
					//						var tt = ttr.GetAsInt("tt");
					//						int tv = ttr.GetAsInt("tv");
					//						cc += ttCarry[tt] * tv;
					//						//var ts = ttTs[tt] * tv;
					//						var travel = TTTravel(tt);
					//						// Todo Navy
					//						if (travel > maxTravel)
					//						{
					//							maxTravel = travel;
					//							raid.troopType = (byte)tt;
					//						}
					//						//   Log($"{tt}:{tv}");
					//					}
					//					if (raid.isReturning)
					//					{
					//						var resO = r[6];
					//						var rate = 60.0f * 0.5f / (raid.GetOneWayTripTimeMinutes(city)); // to res per hour
					//						tWood = resO.GetAsInt("w") * rate;
					//						tIron = resO.GetAsInt("i") * rate;
					//						tFood = resO.GetAsInt("f") * rate;
					//						tStone = resO.GetAsInt("s") * rate;
					//						tGold = resO.GetAsInt("g") * rate;
					//						rWood += tWood;
					//						rStone += tStone;
					//						rIron += tIron;
					//						rFood += tFood;
					//						rGold += tGold;
					//					}
					//					else
					//					{
					//						tWood = 0; tStone = 0; tIron = 0; tFood = 0; tGold = 0;
					//					}
					//					var carry = (cc * 100.0f / res).RoundToInt();
					//					if (carry < minCarry)
					//						minCarry = carry;
					//					// Log($"cc:{cc}, res:{res}, carry:{cc/res} {r[7].GetString()} {r[3].GetInt32()} {r[4].GetInt32()}");

					//					raids = raids.ArrayAppend(raid);
					//				}
					//				city.raidCarry = (byte)minCarry.Min(255);
					//				city.raids = raids;
					//				var commands = (byte)cr[12].GetArrayLength();
					//				city.activeCommands = city.activeCommands.Max(commands);
					//				// Log($"cid:{cid} carry: {minCarry}");

				}
			}
			AppS.QueueOnUIThread(()
				=> {
					MainPage.instance.rWood.Text = $"{CnV.Resources.ResGlyphC(0)} {(raidIncome.r.wood * 0.001).RoundToInt():N0} k/h";
					MainPage.instance.rStone.Text = $"{CnV.Resources.ResGlyphC(1)} {(raidIncome.r.stone * 0.001).RoundToInt():N0} k/h";
					MainPage.instance.rIron.Text = $"{CnV.Resources.ResGlyphC(2)} {(raidIncome.r.iron * 0.001).RoundToInt():N0} k/h";
					MainPage.instance.rFood.Text = $"{CnV.Resources.ResGlyphC(3)} {(raidIncome.r.food * 0.001).RoundToInt():N0} k/h";
					MainPage.instance.rGold.Text = $"{CnV.Resources.goldGlyph} {(raidIncome.gold * 0.001).RoundToInt():N0} k/h";

					MainPage.instance.count.Text = $"Cities: {City.gridCitySource.Count}";
					MainPage.instance.tsTotal.Text = $"TS Total:  {tsTotal:N0}";
					MainPage.instance.tsRaid.Text = $"TS Home: {tsHome:N0}";
					MainPage.instance.castles.Text = $"Castles: {castles}";
					MainPage.instance.water.Text = $"On Water: {onWater}";


					// ts stuff
					//MainPage.rStone = rStone;
					//			//MainPage.rIron = rIron;
					//			//MainPage.rFood = rFood;
					//			//MainPage.rGold = rGold;
					//			//// MainPage.CityListUpdateAll();
					//			///
				});
		}

		public static void ToggleInfoBoxes(bool on) {
			//if (on)
			//	TabPage.mainTabs.Visibility = Visibility.Visible;
			//else
			//	TabPage.mainTabs.Visibility = Visibility.Collapsed;
			////			var vis = on ? Visibility.Visible : Visibility.Collapsed;
			////			instance.raidInfoBox.Visibility = vis;
			////	instance.raidOptionBox.Visibility = vis;
			////		instance.incomeBox.Visibility = vis;
		}
		override public async Task VisibilityChanged(bool visible,bool longTerm) {
			//   Log("Vis change" + visible);

			if(visible) {
				if(Sim.ppdtInitialized) {
					//await Raiding.UpdateTS(true, false);
					//await RaidOverview.Send();
					//	if (City.build != 0)
					//		await GetCity.Post(City.build);
					foreach(var c in City.subCities) {
						if(c.testContinentAndTagFilter)
							c.OnPropertyChanged();
					}
					UpdateRaidIncome();
					//		City.gridCitySource.NotifyReset(true,true);
				}
				//  if (cityGrid.ItemsSource == App.emptyCityList )
				//     cityGrid.ItemsSource = City.gridCitySource;
			}
			else {
				//        cityGrid.ItemsSource = null;
			}
			await base.VisibilityChanged(visible,longTerm: longTerm);
			//if (visible)
			//{
			//	Spot.SyncUISelection(true, City.GetBuild() );
			//}
		}
		//private void BuildCityContextFlyout(TabPage newPage)
		//{
		//    if(newPage!=null)
		//        cityGrid.DataContext = newPage;
		//    cityContextFlyout = new MenuFlyout();
		//    cityContextFlyout.Items.Add(App.CreateMenuItem("Home Whenever", ReturnSlowClick));
		//    cityContextFlyout.Items.Add(App.CreateMenuItem("Home Please", ReturnFastClick));
		//    if (newPage != null)
		//        cityContextFlyout.XamlRoot = newPage.XamlRoot;
		//    cityGrid.ContextFlyout = cityContextFlyout;

		//}
		//public override void XamlTreeChanged(TabPage newPage) {
		//    //       cityGrid.ContextFlyout = null;
		//    if (newPage == null)
		//    {
		//        cityGrid.ContextFlyout = null;
		//        cityGrid.DataContext = null;
		//     //   cityContextFlyout.XamlRoot = null;
		//    }
		//    else
		//    {
		//        //   cityContextFlyout.XamlRoot = null;
		//        cityGrid.DataContext = newPage;
		//        cityContextFlyout = new MenuFlyout();
		//        cityContextFlyout.Items.Add(App.CreateMenuItem("Home Whenever", ReturnSlowClick));
		//        cityContextFlyout.Items.Add(App.CreateMenuItem("Home Please", ReturnFastClick));
		//        cityContextFlyout.XamlRoot = newPage.XamlRoot;
		//        cityGrid.ContextFlyout = cityContextFlyout;

		//    }
		//} // The tab was dragged somewhere else


		public static bool IsVisible() => instance is not null && instance.isFocused;
		static string[] raidingTips =
		{
			//nameof(instance.TipRaiding101),
			//nameof(instance.TipRaiding103),
			//nameof(instance.TipRaiding103),
			//nameof(instance.TipRaidReturn101),
			//nameof(instance.TipRaidReturn102),
			//nameof(instance.TipRaidReturn103),
			//nameof(instance.TipRaidReturn104),
			//nameof(instance.TipCarryCapacity101),
		};

		public static void CheckTipRaiding() {
			//foreach(var t in raidingTips)
			//{
			//	if(!Tips.seen.Contains(t))
			//	{
			//		var tt = instance.GetType().GetField(t).GetValue(instance) as TeachingTip;
			//		App.(()=>tt.Show(t,4000) );
			//		return;
			//	}
			//}

		}




		//static public void ShowTip(TeachingTip tip)
		//{
		//    if (seen || Tips.queued)
		//        return;
		//    Tips.Dispatch(tip, () => seen = true);
		//}
		//static public void ShowTipRaiding1()
		//{
		//    if (Tips.instance.raiding1 || Tips.tipQueued)
		//        return;
		//    instance.TipRaiding101.Show();
		//}
		//static public void ShowTipRaiding2()
		//{
		//    if (!Tips.instance.raiding1 || Tips.instance.raiding2 || Tips.tipQueued)
		//        return;
		//    raidingTip2.Dispatch(instance.TipRaiding201, () => Tips.instance.raiding2 = true);
		//}


		//static public void ShowTipRaiding3()
		//{
		//    if (Tips.instance.raiding2|| Tips.instance.raiding3 || Tips.tipQueued)
		//        return;
		//    raidingTip3.Dispatch(instance.TipRaiding301, () => Tips.instance.raiding3 = true);
		//}



		private void ResetBadRaidsFast(object sender,RoutedEventArgs e) {
			AppS.HideFlyout(sender);
			ReturnRaids(true,true);
		}
		private void ResetBadRaidsSlow(object sender,RoutedEventArgs e) {
			AppS.HideFlyout(sender);
			ReturnRaids(false,true);
		}
		private void ResetRaidsFast(object sender,RoutedEventArgs e) {
			AppS.HideFlyout(sender);
			ReturnRaids(true,false);
		}

		private void ResetRaidsSlow(object sender,RoutedEventArgs e) {
			AppS.HideFlyout(sender);
			ReturnRaids(false,false);
		}
		private async void ReturnRaids(bool fast,bool onlyNeeded) {
			//         await RaidOverview.Send();
			//         await RestAPI.troopsOverview.Post();
			//await RaidOverview.Send();
			var sel0 = await GetSelected();
			var ret = new List<int>();
			foreach(var sel in sel0) {
				var c = sel.AsCity();
				if(!onlyNeeded || (c.raidCarry != 0 && (c.raidCarry <= Settings.resetRaidsCarry || c.tsRaid*100 >= (c.tsRaidTotal * Settings.resetRaidsIdle)))) {
					ret.Add(sel);
				}

			}
			if(await AppS.DoYesNoBox("Reset Raids?",$"Will return {ret.Count}, best to only reset if you will be around to send returners out again","Do it","Maybe Not") == 1) {
				Raiding.ReturnBatch(ret,fast);
			}

		}
		async  Task<List<int> > GetSelected() {
			
			var triedAgain = false;

			__tryAgain:
			
			var ret = new List<int>();

			foreach(City c in cityGrid.SelectedItems) {
				if(!c.testContinentAndTagFilter)
					continue;
				ret.Add(c.cid);
			
			}
			if(ret.Count <= 1 && triedAgain == false) {
				if(await AppS.DoYesNoBox($"{ret.Count} selected","Select all?",cancel: null) == 1) {
					triedAgain=true;
					SelectAll();
					await Task.Delay(500);
					goto __tryAgain;
				}

			}
			return ret;
		}
		public async void AutoRaid(object sender,RoutedEventArgs e) {
			try {
				using var work = new WorkScope("Auto Raid..");
				var sel = await GetSelected();
				//	var totalSent1=0;
				//	for(int iter0=0;iter0<4;++iter0)
				{
					ShellPage.WorkUpdate($"Update Raids...");
					int totalSent = 0;
					int extraSent = 0;
					float minRaidIdle = Settings.raidSendMinIdle*0.01f;
					for(int pass = 0;pass<4;++pass) {

						//await Task.WhenAll(Raiding.UpdateTS(true) );
						int counter = 0;
						int max = sel.Count;
						var sentThisPass = 0;

						SocketClient.DeferSendStart();

						try {
							foreach(var cid in sel) {
								++counter;
								if(counter % 16 == 0) {
									Note.ShowTip($"Auto Raid {counter}/{max}..");
								}
								var c = City.Get(cid);
								var city = Spot.GetOrAdd(cid);
								if(city.raidIdle >= minRaidIdle) {
									if(await DungeonView.ShowDungeonList(city,true)) {
										if(pass > 0) {
											Note.Show($"Found a dungeon for leftover troops at {city}");
											++extraSent;
										}
										else {
											totalSent++;
										}
										++sentThisPass;

									}
								}

							}

						}
						catch(Exception _ex) {
							LogEx(_ex);

						}
						finally {
							SocketClient.DeferSendEnd();
						}
						if(sentThisPass == 0 || Settings.raidIntervals!=0)
							break;

						for(int i = 0;i<4;++i) {
							ShellPage.WorkUpdate($"Delaying for pass {pass}... {4-i}s");
							await Task.Delay(1000);
						}


						// On second and further passes only send if a good number are home
						// not ideal but it helps
						//if (Settings.raidIntervals != 0)
						//	minRaidIdle = minRaidIdle.Max(4.0f / 16.0f);
					}
					Note.Show($"Sent {totalSent} cities to raid");

					//	Note.ShowTip($"Auto Raid Pass {iter0} {sel.Count}/{sel.Count}");
					//	Note.Show($"Sent {totalSent.Min(sel.Count)} Raids (from {sel.Count} selected)");
					//				totalSent1 += totalSent;
					//				if(totalSent == 0)
					//					break;
				}
			}
			catch(Exception _ex) {
				LogEx(_ex);

			}
			//Note.Show($"Total Sent {totalSent1.Min(sel.Count)} Raids (from {sel.Count} selected)");

		}

		private void SelectAll(object? sender = null,RoutedEventArgs? e = null) {
			SelectAllWorkAround(cityGrid);
		}

		public void RaidSettings(object sender,RoutedEventArgs e) {
			DungeonView.Show(null,null);
		}

		private void OnLoaded(object sender,RoutedEventArgs e) {
			base.SetupDataGrid(cityGrid,true,typeof(City),City.gridCitySource);
		}


		private async void ReturnRaidsForScheduled(object sender,RoutedEventArgs e) {
			var selected = await GetSelected();
			foreach(var sel in selected) {
				try {
					sel.AsCity().ReturnRaidsForScheduled();
				}
				catch(Exception ex) {
					LogEx(ex);
				}
			}
		}










		//      static Dungeon lastTooltip;
		//private void DungeonPointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
		//{
		//          var physicalPoint = e.GetCurrentPoint(sender as SfDataGrid);
		//          var point = new Point { X = physicalPoint.Position.X, Y = physicalPoint.Position.Y };
		//          var row = (sender as SfDataGrid).HitTestService.RowItemFromPoint(point);
		//          var cell = (sender as SfDataGrid).HitTestService.CellInfoFromPoint(point);
		//          var hit = cell?.Item as Dungeon;
		//          if(hit!=lastTooltip)
		//	{
		//              lastTooltip = hit;
		//              if (hit != null)
		//                  ToolTipService.SetToolTip(tip, hit.ToString());
		//              else
		//                  ToolTipService.SetToolTip(tip,"None");
		//	}
		//      }
	}

	//public class CustomDataBindingCompleteCommand : DataGridCommand
	//   {
	//       public CustomDataBindingCompleteCommand()
	//       {
	//           this.Id = CommandId.DataBindingComplete;
	//       }

	//	static T GetValue<T>(IDataView view, int id)
	//	{
	//		var rv = view.GetAggregateValue(id, null);
	//		if (rv is null)
	//			return default;
	//		return (T)rv;
	//	}
	//	public override bool CanExecute(object parameter)
	//	{
	//		var context = parameter as DataBindingCompleteEventArgs;
	//		// put your custom logic here
	//		return true;
	//	}
	//	public override void Execute(object parameter)
	//       {
	//	//	AppS.DispatchOnUIThreadLow(() =>
	//	   {
	//		   var context = parameter as DataBindingCompleteEventArgs;
	//			// put your custom logic here
	//			var view = context.DataView;

	//		   MainPage.instance.count.Text = $"Cities: {GetValue<ulong>(view,0)}";
	//		   MainPage.instance.tsTotal.Text = $"TS Total:  {GetValue<double>(view,1):N0}";
	//		   MainPage.instance.tsRaid.Text = $"TS Home: {GetValue<double>(view,2):N0}";
	//		   MainPage.instance.castles.Text = $"Castles: {GetValue<double>(view,3)}";
	//		   MainPage.instance.water.Text = $"On Water: {GetValue<double>(view,4)}";
	//	   }//);
	//       }


	//}
}
