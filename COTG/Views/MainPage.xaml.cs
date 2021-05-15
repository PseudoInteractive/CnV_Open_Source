using COTG.Game;
using COTG.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Grid;
using static COTG.Debug;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Telerik.Core.Data;
using Telerik.Data.Core;
using Telerik.Data;
using System.Collections.Specialized;
using Windows.Foundation;
using Microsoft.Toolkit.Uwp;
using Windows.UI.Xaml.Input;
using COTG.Services;
using System.Collections;
using Windows.UI.Input;
using COTG.Helpers;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using System.Threading.Tasks;
using static COTG.Game.Enum;
using Microsoft.UI.Xaml.Controls;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using System.Threading;
using Telerik.UI.Xaml.Controls.Grid.Primitives;

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

			spotGrids.Add(cityGrid);

			cityGrid.SelectionChanged += SelectionChanged;
			//        var rand = new Random();

			//cityMenuFlyout = new MenuFlyout();
			//var c = new MenuFlyoutItem() { Text = "Home Whenever" };
			//c.Click += ReturnSlowClick;
			//cityMenuFlyout.Items.Add(c);
			//c = new MenuFlyoutItem() { Text = "Home Please" };
			//c.Click += ReturnFastClick;
			//cityMenuFlyout.Items.Add(c);

			cityGrid.OnKey = Spot.OnKeyDown;

			//cityGrid.ContextFlyout = cityMenuFlyout;

			//     cityGrid.SelectionChanged += CityGrid_SelectionChanged;
			cityGrid.CurrentItemChanged += CityGrid_CurrentItemChanged;
			cityGrid.ProcessTooltips();
			//dungeonGrid.ProcessTooltips();
			

		}

		private void CityGrid_CurrentItemChanged(object sender, EventArgs e)
        {
            Log("Current item " + sender.ToString());
		

		}

        private void ColumnHeaderTap()
        {

        }

      
		static RadDataGrid GetGrid(PointerRoutedEventArgs e)
		{
			var a = e.OriginalSource as FrameworkElement;
			while(a != null)
			{
				if (a is DataGridCellsPanel panel)
					return panel.Owner;
				if (a is DataGridRootPanel root)
					return root.Owner;
				a = a.Parent as FrameworkElement; 
			}
			return null;
		}
   

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
        //    instance.Dispatcher.DispatchOnUIThreadLow(() =>
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
		override public async Task VisibilityChanged(bool visible)
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
	         await   base.VisibilityChanged(visible);
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


        public static bool IsVisible() => instance.isVisible;

		static string[] raidingTips =
		{
			nameof(instance.TipRaiding101),
			nameof(instance.TipRaiding103),
			nameof(instance.TipRaiding103),
			nameof(instance.TipRaidReturn101),
			nameof(instance.TipRaidReturn102),
			nameof(instance.TipRaidReturn103),
			nameof(instance.TipRaidReturn104),
			nameof(instance.TipCarryCapacity101),
		};

		public static void CheckTipRaiding()
		{
			//foreach(var t in raidingTips)
			//{
			//	if(!Tips.seen.Contains(t))
			//	{
			//		var tt = instance.GetType().GetField(t).GetValue(instance) as TeachingTip;
			//		App.DispatchOnUIThreadLow(()=>tt.Show(t,4000) );
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


       

        private async void ResetRaids(object sender, RoutedEventArgs e)
        {
            await RaidOverview.Send();
            await RestAPI.troopsOverview.Post();
			await RaidOverview.Send();
			var ret = new List<int>();
            foreach(var c in City.myCities)
            {
				if (!c.testContinentFilter)
					continue;
                if(c.raidCarry != 0 && (c.raidCarry <= SettingsPage.resetRaidsCarry || c.tsRaid*100 >= (c.tsRaidTotal * SettingsPage.resetRaidsIdle) ) )
                {
                    ret.Add(c.cid);
                }

            }
            Note.Show($"Returning {ret.Count}");
            Raiding.ReturnFastBatch(ret);

        }
		private async void AutoRaid(object sender, RoutedEventArgs e)
		{
			using var work = new ShellPage.WorkScope("Auto Raid..");

			var sel = Spot.GetSelectedForContextMenu(0, false, onlyMine: true);
			int totalSent = 0;
			float minRaidIdle = 0.0625f;
			for (int pass=0;pass<8;++pass)
			{

				await Raiding.UpdateTS(true);
				int counter = 0;
				int processed = 0;
				int max = sel.Count;
				foreach (var cid in sel)
				{
					++counter;
					if (counter % 16 == 0)
					{
						Note.ShowTip($"Auto Raid pass {pass}: {counter}/{max}..");
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
			Note.ShowTip($"Auto Raid: Completed: {sel.Count}/{sel.Count}");
			Note.Show($"Sent {totalSent.Min(sel.Count)} Raids (from {sel.Count} selected)");

		}
		private void SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
		{
			if (!isActive)
				return;

			if (SpotTab.silenceSelectionChanges == 0)
			{
				try
				{

					var sel = cityGrid.SelectedItems;
					var newSel = new HashSet<int>();
					bool raidVisible = false;
					foreach (Spot s in sel)
					{
						newSel.Add(s.cid);
					
					}
	

					//          Spot.selected.EnterWriteLock();

					Spot.selected = newSel;
				}
				catch (Exception ex)
				{
					LogEx(ex);
				}
				finally
				{
					//          Spot.selected.ExitWriteLock();
				}
			}
		}

		private void SelectAll(object sender, RoutedEventArgs e)
		{
			cityGrid.SelectAll();

		}

		//      static Dungeon lastTooltip;
		//private void DungeonPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
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

        public override bool CanExecute(object parameter)
        {
            var context = parameter as DataBindingCompleteEventArgs;
            // put your custom logic here
            return true;
        }

        static T TryGetValue<T>(object o) => o == null ? default : (T)o;
        public override void Execute(object parameter)
        {
            var context = parameter as DataBindingCompleteEventArgs;
            // put your custom logic here
            var view = context.DataView;
            
            MainPage.instance.count.Text =$"Cities: {TryGetValue<ulong>(view.GetAggregateValue(0, null))}";
            MainPage.instance.tsTotal.Text=$"TS Total:  {TryGetValue<double>(view.GetAggregateValue(1, null)):N0}";
            MainPage.instance.tsRaid.Text= $"TS Home: {TryGetValue<double>(view.GetAggregateValue(2, null)):N0}";
            MainPage.instance.castles.Text= $"Castles: {TryGetValue<double>(view.GetAggregateValue(3, null))}";
            MainPage.instance.water.Text= $"On Water: {TryGetValue<double>(view.GetAggregateValue(4, null))}";
        }
		

	}
}
