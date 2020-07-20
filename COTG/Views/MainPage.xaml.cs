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



    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public BoundCollection<City> cities { get; } = new BoundCollection<City>();
      //  public DumbCollection<Dungeon> dungeons { get; } = new DumbCollection<Dungeon>();
        public static MainPage instance;
        public static City raidCity;
        //        public static City showingRowDetails;

        
        //public DataTemplate GetTsInfoDataTemplate()
        //{
        //    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
        //    Assert(rv != null);
        //    return rv;
        //}
        public static RadDataGrid CityGrid => instance.cityGrid;
        const int raidStepCount = 9;
        static DumbCollection<float> raidSteps = new DumbCollection<float>();
        static MenuFlyout cityMenuFlyout;
        public MainPage()
        {
//            var a = Telerik.UI.Xaml.Controls.Grid.Primitives.For
            Assert(instance == null);
            instance = this;
            InitializeComponent();
            for(int i=0;i<raidStepCount;++i)
			{
                raidSteps.Add( ( (MathF.Exp((i - 4) * 1.0f / 16.0f) + (-0.032625f,0.032625f).Random())*100.0f).RoundToInt() );
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
        }

        private void CityGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            var it = e.AddedItems.GetEnumerator();
            if(!it.MoveNext())
            {
                Assert(false);
                return;
            }
            var newSel = it.Current as City;
            Assert(newSel != null);
            if (newSel == raidCity)
                return;
            SetRaidCity(newSel,true,false,true);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!(e.Parameter is ShellPage))
                Raiding.UpdateTS();
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
                var json = "{\"a\":"+cid+",\"c\":0,\"b\":1}";
                Note.Show($"{cid.CidToStringMD()} recall slow");
                Post.SendEncrypted("includes/UrOA.php",json,"Rx3x5DdAxxerx3") ;
            }
        }
        private void ReturnFastClick(object sender, RoutedEventArgs e)
        {
            var cid = Spot.uiPress;
            if (cid != 0)
            {
                Note.Show($"{cid.CidToStringMD()} recall fast");
                Post.Send("overview/rcallall.php", "a="+cid);
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

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
             //   Note.L("ListChange: ");

                var cities = instance.cities;
                if (selectedCityList == null || selectedCityList.id==-1)
                {
                    cities.Reset(City.all.Values.OrderBy((a) => a.cityName));
                }
                else
                {
                    var cityList = selectedCityList;// CityList.Find(selectedCityList);
                    if(cityList!=null)
                    {
                        var filtered = new List<City>();
                        foreach(var cid in cityList.cities)
                        {
                            if(City.all.TryGetValue(cid,out var c))
                            {
                                filtered.Add(c);
                            }
                        }
                        cities.Reset(filtered.OrderBy((a) => a.cityName));

                    }


                }
            });
        }

        internal static void SetRaidCity(int cid, bool fromUI, bool noRaidScan, bool getCityData)
        {
            if (City.all.TryGetValue(cid, out var city))
            {
                SetRaidCity(city, fromUI, noRaidScan, getCityData);
            }
        }
        internal static void SetRaidCity(City city, bool fromUI,bool noRaidScan, bool getCityData)
        {
             var changed = city != raidCity;
             raidCity = city;
            if(!fromUI && changed)
                CityGrid.SelectItem(city);

            if (!noRaidScan)
            {
                if (changed)
                    ScanDungeons.Post(city.cid,getCityData);
            }
        }

        public static void CityListUpdateAll ()
        {
            if (instance == null)
                return;
            // Note.L("UpdateAll: ");
            AApp.DispatchOnUIThreadLow( instance.cities.NotifyReset);
            
        }

        public static void UpdateDungeonList(List<Dungeon> dungeons)
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
            // tell UI that list data has changed
            AApp.DispatchOnUIThread(() =>
            {
                instance.dungeonGrid.ItemsSource = instance.dungeonGrid.ItemsSource;
            });
        }

        //public static void ClearDungeonList()
        //{
        //    UpdateDungeonList(null);
        //}


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
       

        private void RecallSlow(object sender, RoutedEventArgs e)
        {
            Log("RecallSlow")
                ;
        }

        private void RecallFast(object sender, RoutedEventArgs e)
        {
            Log("RecallFast")
                ;
        }

        private void cityGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Log("Tapped");
        }

        public static ComboBox CityListBox => instance.cityListBox;
        private DumbCollection<CityList> cityListSelections =>  CityList.selections;
        public static CityList selectedCityList = null;
        private CityList selectedCityListBind {
            get {
                Log("Read!");
                return selectedCityList;
            }
            set
            {
                selectedCityList = value;
                Log("Write!");
                CityListChange();
            }

        }

		private void RaidCarrySubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
		{
            Log("Submit: " + args.Text);
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
                raidSteps.NotifyChange(_raidCarry, bestId);
                raidCarryBox.SelectedValue = _raidCarry;
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
            if ((newVal - Raiding.desiredCarry).Abs() <= 1.0f / 128.0f)
                return false;
            Raiding.desiredCarry = newVal;
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
