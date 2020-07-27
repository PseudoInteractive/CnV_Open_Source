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

namespace COTG.Views
{



    public sealed partial class MainPage : UserTab, INotifyPropertyChanged
    {
        public DumbCollection<City> gridCitySource { get; } = new DumbCollection<City>();
      //  public DumbCollection<Dungeon> dungeons { get; } = new DumbCollection<Dungeon>();
        public static MainPage instance;

        //        public static City showingRowDetails;

        
        //public DataTemplate GetTsInfoDataTemplate()
        //{
        //    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
        //    Assert(rv != null);
        //    return rv;
        //}
        public static RadDataGrid CityGrid => instance.cityGrid;
        const int raidStepCount = 9;
        static float[] raidSteps = new float[raidStepCount];
        static MenuFlyout cityMenuFlyout;
        public MainPage()
        {
//            var a = Telerik.UI.Xaml.Controls.Grid.Primitives.For
            Assert(instance == null);
            instance = this;
            InitializeComponent();
            for(int i=0;i<raidStepCount;++i)
			{
                raidSteps[i]=( ( (MathF.Exp((i - 4) * 1.0f / 16.0f) + (-0.032625f,0.032625f).Random())*100.0f).RoundToInt() );
			}
            raidSteps[4] = (Raiding.desiredCarry*100.0f).RoundToInt();
            raidCarryBox.ItemsSource= raidSteps;
            raidCarryBox.SelectedIndex = 4;

            var rand = new Random();

            cityMenuFlyout = new MenuFlyout();
            var c = new MenuFlyoutItem() { Text = "Return Slow" };
            c.Click += ReturnSlowClick;
            cityMenuFlyout.Items.Add(c);
            c = new MenuFlyoutItem() { Text = "Return Fast" };
            c.Click += ReturnFastClick;
            cityMenuFlyout.Items.Add(c);

            cityGrid.ContextFlyout = cityMenuFlyout;

            cityGrid.SelectionChanged += CityGrid_SelectionChanged;
            cityGrid.CurrentItemChanged += CityGrid_CurrentItemChanged;
            cityListBox.SelectedIndex = 0; // reset

			cityListBox.SelectionChanged += CityListBox_SelectionChanged;
        }

        private void CityGrid_CurrentItemChanged(object sender, EventArgs e)
        {
            Log("Current item " + sender.ToString());
        }

        private void CityListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            if (e.AddedItems.Any())
            {
                var newSel = e.AddedItems?.FirstOrDefault();
                var priorSel = e.RemovedItems?.FirstOrDefault();
                if (newSel != priorSel)
                {
               //     Log("City Sel changed");
                    CityListChange();
                }
            }
		}

        private void ColumnHeaderTap()
        {

        }

        private void CityGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            foreach(var i in e.AddedItems  )
            {
                Log("Added: " + (i)); 
            }
            foreach (var i in e.RemovedItems)
            {
                Log("Removed: " + (i));
            }

            var it = e.AddedItems.FirstOrDefault();
            
            foreach( var i in cityGrid.SelectedItems)
            {
                Log("Selected: " + i.ToString());
            }
            var newSel = it as City;
        //    Assert(newSel != null);
            if (it is null)
                return;
            if (newSel == City.focus)
                return;
            newSel.SetFocus(true,false,true);
        }

        

        private void CityGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Spot.ProcessPointerMoved(sender, e);
        }
        private void CityGrid_PointerPress(object sender, PointerRoutedEventArgs e)
        {
            Spot.ProcessPointerPress(sender,e);
        }
        private void cityGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Spot.ProcessPointerExited();
        }


        private void ReturnSlowClick(object sender, RoutedEventArgs e)
        {
            var cid = Spot.uiPress;
            if (cid != 0)
            {
                Raiding.ReturnSlow(cid,true);
            }
        }

       

        private void ReturnFastClick(object sender, RoutedEventArgs e)
        {
            var cid = Spot.uiPress;
            if (cid != 0)
            {
                Raiding.ReturnFast(cid,true);
            }
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



        public static void CityListChange()
        {
            if (instance == null)
                return;

            instance.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
             //   Note.L("ListChange: ");

                var citySource = instance.gridCitySource;
                var selectedCityList = CityListBox.SelectedItem as CityList;
                if (selectedCityList == null || selectedCityList.id==-1) // "all"
                {
                    citySource.Set(City.allCities.Values.OrderBy((a) => a.cityName));
                }
                else
                {
                    var cityList = selectedCityList;// CityList.Find(selectedCityList);
                        var filtered = new List<City>();
                        foreach(var cid in cityList.cities)
                        {
                            if(City.allCities.TryGetValue(cid,out var c))
                            {
                                filtered.Add(c);
                            }
                        }
                        citySource.Set(filtered.OrderBy((a) => a.cityName));
                }
            });
        }

        public static void CityListUpdateAll ()
        {
            if (instance == null)
                return;
            // Note.L("UpdateAll: ");
            AApp.DispatchOnUIThreadLow( instance.gridCitySource.NotifyReset);
            
        }

        public static void UpdateDungeonList(IEnumerable<Dungeon> dungeons)
        {
            if (instance == null)
                return;
            //  Raiding.UpdateTS(); // not sychronous, the results will come in after the dungeon list is synced

            AApp.DispatchOnUIThread( () =>
            {
                instance.dungeonGrid.ItemsSource = dungeons;
            });
        }
        public static void UpdateRaidPlans()
        {
            AApp.DispatchOnUIThread(() =>
            {
                // trick it
                var temp = instance.dungeonGrid.ItemsSource;
                instance.dungeonGrid.ItemsSource = null;
                instance.dungeonGrid.ItemsSource = temp;
            });
            // tell UI that list data has changed
           
        }

        public static void ClearDungeonList()
        {
            UpdateDungeonList(null);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }
            storage = value;
            OnPropertyChanged(propertyName);
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
       

        //private void RecallSlow(object sender, RoutedEventArgs e)
        //{
        //    Log("RecallSlow")
        //}

        //private void RecallFast(object sender, RoutedEventArgs e)
        //{
        //    Log("RecallFast")
        //        ;
        //}

        private void cityGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Log("Tapped");
        }

        public static ComboBox CityListBox => instance.cityListBox;
        private DumbCollection<CityList> cityListSelections =>  CityList.selections;
        
        
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

        

        override public void VisibilityChanged(bool visible)
        {
            //   Log("Vis change" + visible);

            if (visible)
            {
                Raiding.UpdateTS();
                RaidOverview.Send();
                if(City.build!=null)
                    GetCity.Post(City.build.cid);
            }
        }

        public static bool IsVisible() => instance.isVisible;


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
}
