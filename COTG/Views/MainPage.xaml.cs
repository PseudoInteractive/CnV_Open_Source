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

namespace COTG.Views
{

    public class DumbCollection<T> : List<T>, INotifyCollectionChanged,INotifyPropertyChanged
    {
		public DumbCollection(IEnumerable<T> collection) : base(collection)
		{
		}

		public DumbCollection()
		{
		}

		public void OnPropertyChanged(T city,string propertyName) => PropertyChanged?.Invoke(city, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

       

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void NotifyReset()
        {
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset ));
        }
        public void Reset(IEnumerable<T> src)
        {
            // catch for thread safety
            Clear();
            AddRange(src);
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public void NotifyChange(T item)
        {
         //   OnPropertyChanged(item,null);
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,item,item,IndexOf(item)));
        }
        public void Replace(T newItem, T oldItem,int index)
        {
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,newItem,oldItem, index));
        }
        public void AddAndNotify(T item)
        {
            var id = Count;
            Add(item);
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, id));
        }
        
    }

    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public BoundCollection<City> cities { get; } = new BoundCollection<City>();
        public DumbCollection<Dungeon> dungeons { get; } = new DumbCollection<Dungeon>();
        public static MainPage cache;
        //        public static City showingRowDetails;

        //public DataTemplate GetTsInfoDataTemplate()
        //{
        //    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
        //    Assert(rv != null);
        //    return rv;
        //}
        public static RadDataGrid CityGrid => cache.cityGrid;
        static MenuFlyout cityMenuFlyout;
        public MainPage()
        {
            Assert(cache == null);
            cache = this;
            InitializeComponent();
            cityMenuFlyout = new MenuFlyout();
            var c = new MenuFlyoutItem() { Text = "Return Slow" };
            c.Click += ReturnSlowClick;
            cityMenuFlyout.Items.Add(c);
            c = new MenuFlyoutItem() { Text = "Return Fast" };
            c.Click += ReturnFastClick;
            cityMenuFlyout.Items.Add(c);

            cityGrid.ContextFlyout = cityMenuFlyout;


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
                Note.Show($"{cid.ToCoordinateMD()} recall slow");
                Post.SendEncrypted("includes/UrOA.php",json,"Rx3x5DdAxxerx3") ;
            }
        }
        private void ReturnFastClick(object sender, RoutedEventArgs e)
        {
            var cid = Spot.uiPress;
            if (cid != 0)
            {
                Note.Show($"{cid.ToCoordinateMD()} recall fast");
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

        public async static void CityChange( City city,string memberName=null)
        {
            if (cache == null || cache.cities.Count == 0 )
                return;
            
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var cities = cache.cities;
                if (cities.Contains(city))
                {
                 //   Note.L($"Change: {city.cid.ToCoordinate()} {cities.IndexOf(city) }");
                    //                    cache.cityGrid.BeginEdit(city);
                 //   cache.cityGrid.BeginEdit(cache.cityGrid.SelectedItem ?? city);
                 //   cache.cityGrid.CommitEdit();

                    city.OnPropertyChanged(memberName);
                }
                else
                {
                 //   Note.L($"Add: {city.cid.ToCoordinate()} {cities.IndexOf(city) }");
                    cities.Add(city);

                }
            });
        }
        public async static void CityListChange()
        {
            if (cache == null)
                return;

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Note.L("ListChange: ");

                var cities = cache.cities;
                cities.Reset(City.all.Values);
            });
        }
        public async static void CityListUpdateAll ()
        {
            if (cache == null)
                return;
            Note.L("UpdateAll: ");
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                cache.cities.NotifyReset();
            });
        }

        public static void UpdateDungeonList(List<Dungeon> dungeons)
        {
            if (cache == null)
                return;

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var l = cache.dungeons;
                l.Clear();
                if(dungeons!=null)
                    l.AddRange(dungeons);
                l.NotifyReset();
            });
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
