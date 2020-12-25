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
        static float[] raidSteps;
        public static MainPage instance;
     
        public float troopPercent = 1;
        //        public static City showingRowDetails;

        //{
        //    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
        //    Assert(rv != null);
        //    return rv;
        //}
        public static RadDataGrid CityGrid => instance.cityGrid;
        const int raidStepCount = 9;
      //  static MenuFlyout cityMenuFlyout;
        public MainPage()
        {
//            var a = Telerik.UI.Xaml.Controls.Grid.Primitives.For
            Assert(instance == null);
            instance = this;
            InitializeComponent();
            raidSteps =  new float[raidStepCount];
            for (int i=0;i<raidStepCount;++i)
			{
                raidSteps[i]=( ( (MathF.Exp((i - 4) * 1.0f / 16.0f) + (-0.032625f,0.032625f).Random())*100.0f).RoundToInt() );
			}
            raidSteps[4] = (Raiding.desiredCarry*100.0f).RoundToInt();
            raidCarryBox.ItemsSource= raidSteps;
            raidCarryBox.SelectedIndex = 4;
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
			// cityGrid.CurrentItemChanged += CityGrid_CurrentItemChanged;
			cityGrid.ProcessTooltips();
			//dungeonGrid.ProcessTooltips();
			this.music.AutoPlay = true;


		}

		private void CityGrid_CurrentItemChanged(object sender, EventArgs e)
        {
//            Log("Current item " + sender.ToString());
        }

		public static City expandedCity; // city with dungeon list visible if any 
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
		static bool IsFromDungeonGrid(PointerRoutedEventArgs e)
		{
			var a = GetGrid(e);
			if (a==null)
				return false;
			return a.Tag  as String == "Dungeons";
		}
        private void CityGrid_PointerPress(object sender, PointerRoutedEventArgs e)
        {
				if (IsFromDungeonGrid(e)) 
					return;
            Spot.ProcessPointerPress(this,sender,e);
        }
        private void cityGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
			if (IsFromDungeonGrid(e))
				return;
			Spot.ProcessPointerExited();
            //if (string.Empty!=lastTip)
            //{
            //    lastTip = string.Empty;
            //    TabPage.mainTabs.tip.Text = string.Empty;
            //}
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
            if (MainPage.IsVisible())
            {
                foreach (var sel in instance.cityGrid.SelectedItems)
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

        public static void UpdateDungeonList(IEnumerable<Dungeon> dungeons)
        {
            if (instance == null)
                return;
			if(dungeons==null)
				Dungeon.raidDungeons.Clear();
			//  Raiding.UpdateTS(); // not sychronous, the results will come in after the dungeon list is synced

			Dungeon.raidDungeons.NotifyReset();
			//    instance.dungeonGrid.ItemsSource = dungeons;

		}
        public static void UpdateRaidPlans()
        {
			//// instance.Dispatcher.DispatchOnUIThread(() =>
			// {
			//     // trick it
			//     var temp = instance.dungeonGrid.ItemsSource;
			//     instance.dungeonGrid.ItemsSource = null;
			//     instance.dungeonGrid.ItemsSource = temp;
			// }
			// // tell UI that list data has changed
			Dungeon.raidDungeons.NotifyReset();
		}

   
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

       



        
		private void RaidCarrySubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
		{
       //     Log("Submit: " + args.Text);
            if( float.TryParse(args.Text,System.Globalization.NumberStyles.Number,null, out float _raidCarry) )
			{
                
                float bestError = float.MaxValue;
                var bestId = 0;
                for(int i=0;i<raidStepCount;++i)
				{
                    float d = (_raidCarry - raidSteps[i]).Abs();
                    if(d < bestError)
					{
                        bestError = d;
                        bestId = i;
					}
				}
                raidSteps[bestId] = _raidCarry;
                raidCarryBox.ItemsSource = raidSteps;
  //              raidCarryBox.SelectedValue = _raidCarry;
                raidCarryBox.SelectedIndex = bestId;
                //raidSteps;
                if(SetCarry(_raidCarry))
                {
                    //if(raidCity!=null)
                    //    ScanDungeons.Post(raidCity.cid,false) ;
                    UpdateRaidPlans();
                }

            }
            else
			{
                args.Handled = true;
                Assert(false);
			}



		}
        private static bool SetCarry(float src)
		{
            var newVal = (src) * 0.01f;
            if ((newVal - Raiding.desiredCarry).Abs() <= 1.0f / 256.0f)
                return false;
            Raiding.desiredCarry = newVal;
            SettingsPage.SaveAll();
            return true;
        }
        private void RaidCarrySelChanged(object sender, SelectionChangedEventArgs e)
		{
         //   Log("Sel update");
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                if (SetCarry( (float)e.AddedItems[0] ) )
                {
                    UpdateRaidPlans(); //Log("Sel changed");
                    //if (raidCity != null)
                    //    ScanDungeons.Post(raidCity.cid,false);
                }
            }
        }

        private void IncludeButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Microsoft.UI.Xaml.Controls.DropDownButton;
            var flyout = new MenuFlyout();
            for (int i = 0; i < ttCount; ++i)
            {
                if (IsRaider(i))
                {
                    var but = new ToggleMenuFlyoutItem() { IsChecked = Raid.includeRaiders[i], DataContext = (object)i, Text = ttNameWithCaps[i] };
                    flyout.Items.Add(but);
                }
            }
            flyout.CopyXamlRoomFrom(button);
            flyout.Closing += Flyout_Closing;
            flyout.ShowAt(button);
        }

        private void Flyout_Closing(Windows.UI.Xaml.Controls.Primitives.FlyoutBase sender, Windows.UI.Xaml.Controls.Primitives.FlyoutBaseClosingEventArgs args)
        {
            var menu = (sender as MenuFlyout);
            int counter = 0;
            for (int i = 0; i < ttCount; ++i)
            {
                if (IsRaider(i))
                {
                    var but = menu.Items[counter] as ToggleMenuFlyoutItem;
                    Raid.includeRaiders[i] = but.IsChecked;
                    ++counter;
                }
            }
            Raiding.UpdateTS(true, true);
        }

        override public void VisibilityChanged(bool visible)
        {
            //   Log("Vis change" + visible);

            if (visible)
            {
                Raiding.UpdateTS(true,true);
                RaidOverview.Send();
                if(City.build!=0)
                    GetCity.Post(City.build);
         
             //  if (cityGrid.ItemsSource == App.emptyCityList )
             //     cityGrid.ItemsSource = City.gridCitySource;
            }
            else
            {
        //        cityGrid.ItemsSource = null;
            }
            base.VisibilityChanged(visible);

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
        private void TipRaiding101_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            TipRaiding102.IsOpen = true;
        }
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


        private void RaidFraction_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
		{
            Raiding.UpdateTS(true,true);

		}

		private void TipRaiding202_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            TipRaiding203.IsOpen = true;
        }

        private void TipRaiding201_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            TipRaiding201.IsOpen = true;
        }

        private async void ResetRaids(object sender, RoutedEventArgs e)
        {
            await RaidOverview.Send();
            await RestAPI.troopsOverview.Post();
            var ret = new List<int>();
            foreach(var c in City.allCities.Values)
            {
                if(c.raidCarry != 0 && (c.raidCarry <= 90 || c.tsRaid >= c.tsTotal/4) )
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

			await Raiding.UpdateTS(true);
			var sel = Spot.GetSelectedForContextMenu(0, false);
			foreach(var cid in sel)
			{
				Spot s = Spot.GetOrAdd(cid);
				if (s is City city)
				{
					await ScanDungeons.Post(cid, city.commandSlots==0, true);
				}
	
			}

		}
		private void SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
		{
			if (SpotTab.silenceSelectionChanges == 0)
			{
				try
				{

					var sel = cityGrid.SelectedItems;
					var newSel = new HashSet<int>();
					foreach (Spot s in sel)
					{
						newSel.Add(s.cid);

					}

					//          Spot.selected.EnterWriteLock();

					Spot.selected = newSel;
				}
				catch (Exception ex)
				{
					Log(ex);
				}
				finally
				{
					//          Spot.selected.ExitWriteLock();
				}
			}
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
