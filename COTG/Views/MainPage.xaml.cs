using COTG.Game;
using COTG.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Microsoft.UI.Xaml.Controls;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Grid;
using static COTG.Debug;
using Windows.ApplicationModel.Core;
//using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Telerik.Core.Data;
using Telerik.Data.Core;
using Telerik.Data;
using System.Collections.Specialized;
using Windows.Foundation;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Input;
using COTG.Services;
using System.Collections;
//using Windows.UI.Input;
using COTG.Helpers;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;
using System.Threading.Tasks;
using static COTG.Game.Troops;
using Microsoft.UI.Xaml.Controls;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using System.Threading;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using COTG.JSON;

namespace COTG.Views
{



    public sealed partial class MainPage : UserTab, INotifyPropertyChanged
    {
        public static MainPage instance;
     


		//        public static City showingRowDetails;

		//{
		//    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
		//    Assert(rv != null);
		//    return rv;
		//}
		public static RadDataGrid CityGrid => instance.cityGrid;
    
      //  static MenuFlyout cityMenuFlyout;
        public MainPage()
        {
//            var a = Telerik.UI.Xaml.Controls.Grid.Primitives.For
            Assert(instance == null);
            instance = this;
            InitializeComponent();

			
			//dungeonGrid.ProcessTooltips();


		}

	

		
		private void CityGrid_CurrentItemChanged(object sender, EventArgs e)
        {
            Log("Current item " + sender.ToString());
		

		}

        private void ColumnHeaderTap()
        {

        }

		
		//static RadDataGrid GetGrid(PointerRoutedEventArgs e)
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
   

        public static List<int> GetContextCids(object sender)
        {
            
            var cid = (int)((sender as MenuFlyoutItem).DataContext);
            return GetContextCids(cid);
        }
        public static List<int> GetContextCids(int cid)
        {
            var cids = new List<int>();

            if(cid != 0)
             cids.Add(cid);

            if (Spot.TryGetGrid(out var grid))
            {
                foreach (var sel in grid.SelectedItems)
                {
                    if (sel is City city)
                    {
                        if(city.cid != cid)
                            cids.Add(city.cid);
                    }
                }
            }
            NearDefenseTab.GetSelected(cids);
            return cids;
        }
        

        public static int GetContextCidCount(int focusCid)
        {
            return GetContextCids(focusCid).Count;
      
        }
        public static void ReturnSlowClick(object sender, RoutedEventArgs e)
        {
            var cids = GetContextCids(sender);
            if (cids.Count == 1)
                Raiding.ReturnSlow(cids[0], true);
            else
                Raiding.ReturnSlowBatch(cids);
        }

		

		public static void ReturnFastClick(object sender, RoutedEventArgs e)
        {
            var cids = GetContextCids(sender);
            if (cids.Count == 1)
                Raiding.ReturnFast(cids[0], true);
            else
                Raiding.ReturnFastBatch(cids);
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
   
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }
            storage = value;
            OnPropertyChanged(propertyName);
        }

		public static void ToggleInfoBoxes(bool on)
		{
			//if (on)
			//	TabPage.mainTabs.Visibility = Visibility.Visible;
			//else
			//	TabPage.mainTabs.Visibility = Visibility.Collapsed;
			////			var vis = on ? Visibility.Visible : Visibility.Collapsed;
			////			instance.raidInfoBox.Visibility = vis;
			////	instance.raidOptionBox.Visibility = vis;
			////		instance.incomeBox.Visibility = vis;
		}
		override public async Task VisibilityChanged(bool visible, bool longTerm)
		{
            //   Log("Vis change" + visible);

            if (visible)
            {
				if (JSClient.ppdtInitialized)
				{
					await Raiding.UpdateTS(true, false);
					await RaidOverview.Send();
					if (City.build != 0)
						await GetCity.Post(City.build);

					City.gridCitySource.NotifyReset();
				}
             //  if (cityGrid.ItemsSource == App.emptyCityList )
             //     cityGrid.ItemsSource = City.gridCitySource;
            }
            else
            {
        //        cityGrid.ItemsSource = null;
            }
	         await   base.VisibilityChanged(visible, longTerm: longTerm);
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


        public static bool IsVisible() => instance.isFocused;

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

		public static void CheckTipRaiding()
		{
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



		private void ResetBadRaidsFast(object sender, RoutedEventArgs e)
		{
			App.HideFlyout(sender);
			ReturnRaids(true, true);
		}
		private void ResetBadRaidsSlow(object sender, RoutedEventArgs e)
		{
			App.HideFlyout(sender);
			ReturnRaids(false, true);
		}
		private void ResetRaidsFast(object sender, RoutedEventArgs e)
		{
			App.HideFlyout(sender);
			ReturnRaids(true, false);
		}

		private void ResetRaidsSlow(object sender, RoutedEventArgs e)
		{
			App.HideFlyout(sender);
			ReturnRaids(false, false);
		}
		private async void ReturnRaids(bool fast, bool onlyNeeded)
        {
            await RaidOverview.Send();
            await RestAPI.troopsOverview.Post();
			await RaidOverview.Send();

			if (cityGrid.SelectedItems.Count <= 1)
			{
				if (await App.DoYesNoBoxUI("Select All", "Non selected, select all?") != 1)
					return;
				cityGrid.SelectAll();
			}
			var ret = new List<int>();

			foreach (City c in cityGrid.SelectedItems)
            {
				if (!c.testContinentAndTagFilter )
					continue;
                if( !onlyNeeded || (c.raidCarry != 0 && (c.raidCarry <= SettingsPage.resetRaidsCarry || c.tsRaid*100 >= (c.tsRaidTotal * SettingsPage.resetRaidsIdle) )) )
                {
                    ret.Add(c.cid);
                }

            }
			if (await App.DoYesNoBox("Reset Raids?", $"Will return {ret.Count}, best to only reset if you will be around to send returners out again", "Do it", "Maybe Not") == 1)
			{
				if(fast)
					Raiding.ReturnFastBatch(ret);
				else
					Raiding.ReturnSlowBatch(ret);
			}

		}
		public async void AutoRaid(object sender, RoutedEventArgs e)
		{
			using var work = new ShellPage.WorkScope("Auto Raid..");

			var sel = Spot.GetSelectedForContextMenu(0, false, onlyMine: true);
			if(sel.Count <= 1)
			{
				if(await App.DoYesNoBox($"{sel.Count} selected","Select all?",cancel:null) == 1)
				{
					cityGrid.SelectAll();
					await Task.Delay(200);
					sel = Spot.GetSelectedForContextMenu(0,false,onlyMine: true);
				}
			}
			var totalSent1=0;
			for(int iter0=0;iter0<4;++iter0)
			{
				ShellPage.WorkUpdate($"Update Raids {iter0}...");
				int totalSent = 0;
				float minRaidIdle = 0.0625f;
				for (int pass=0;pass<8;++pass)
				{

					await Task.WhenAll(Raiding.UpdateTS(true), RaidOverview.Send() );
					int counter = 0;
					int processed = 0;
					int max = sel.Count;
					foreach (var cid in sel)
					{
						++counter;
						if (counter % 16 == 0)
						{
							Note.ShowTip($"Auto Raid pass {iter0} {pass}: {counter}/{max}..");
						}
						var c = City.Get(cid);
						var city = Spot.GetOrAdd(cid);
						if(city.raidIdle >= minRaidIdle )
						{
							if( await ScanDungeons.Post(cid, city.commandSlots == 0, true) )
							{
								++processed;
								++totalSent;
							}
						}

					}
					if (processed == 0)
						break;
					Note.Show($"Pass {pass} sent {processed} cities to raid");

					// On second and further passes only send if they are all home
					// not ideal but it helps
					if (SettingsPage.raidIntervals != 0)
						minRaidIdle = 15.0f / 16.0f;
				}
				Note.ShowTip($"Auto Raid Pass {iter0} {sel.Count}/{sel.Count}");
				Note.Show($"Sent {totalSent.Min(sel.Count)} Raids (from {sel.Count} selected)");
				totalSent1 += totalSent;
				if(totalSent == 0)
					break;
				for(int i=0;i<5;++i)
				{
					ShellPage.WorkUpdate($"Delaying for pass {iter0}... {5-i}s");
					await Task.Delay(5000);
				}
			}
			Note.Show($"Total Sent {totalSent1.Min(sel.Count)} Raids (from {sel.Count} selected)");

		}

		private void SelectAll(object sender, RoutedEventArgs e)
		{
			cityGrid.SelectAll();
		}

		public void RaidSettings(object sender, RoutedEventArgs e)
		{
			DungeonView.Show(null, null);
		}

		private async void ReturnRaidsForOutgoing(object sender, RoutedEventArgs e)
		{
			using var work = new ShellPage.WorkScope("Return For Outgoing..");
			var counter = OutgoingOverview.outgoingCounter+1;
			OutgoingOverview.OutgoingUpdateDebounce.Go();

			do
			{
				await Task.Delay(500);
			} while (counter > OutgoingOverview.outgoingCounter);
					
			int cities = 0;
			foreach(var city in Spot.allianceCitiesWithOutgoing)
			{
				if (!city.isMine)
					continue;
				await city.ShowReturnAt(false);
				await Task.Delay(200);
				++cities;
			}
			Note.Show($"{cities} returned cities with outgoing");

		}

		





		//      static Dungeon lastTooltip;
		//private void DungeonPointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
		//{
		//          var physicalPoint = e.GetCurrentPoint(sender as RadDataGrid);
		//          var point = new Point { X = physicalPoint.Position.X, Y = physicalPoint.Position.Y };
		//          var row = (sender as RadDataGrid).HitTestService.RowItemFromPoint(point);
		//          var cell = (sender as RadDataGrid).HitTestService.CellInfoFromPoint(point);
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

	public class CustomDataBindingCompleteCommand : DataGridCommand
    {
        public CustomDataBindingCompleteCommand()
        {
            this.Id = CommandId.DataBindingComplete;
        }

		static T GetValue<T>(IDataView view, int id)
		{
			var rv = view.GetAggregateValue(id, null);
			if (rv is null)
				return default;
			return (T)rv;
		}
		public override bool CanExecute(object parameter)
		{
			var context = parameter as DataBindingCompleteEventArgs;
			// put your custom logic here
			return true;
		}
		public override void Execute(object parameter)
        {
		//	App.DispatchOnUIThreadLow(() =>
		   {
			   var context = parameter as DataBindingCompleteEventArgs;
				// put your custom logic here
				var view = context.DataView;

			   MainPage.instance.count.Text = $"Cities: {GetValue<ulong>(view,0)}";
			   MainPage.instance.tsTotal.Text = $"TS Total:  {GetValue<double>(view,1):N0}";
			   MainPage.instance.tsRaid.Text = $"TS Home: {GetValue<double>(view,2):N0}";
			   MainPage.instance.castles.Text = $"Castles: {GetValue<double>(view,3)}";
			   MainPage.instance.water.Text = $"On Water: {GetValue<double>(view,4)}";
		   }//);
        }
		

	}
}
