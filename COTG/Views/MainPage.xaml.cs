﻿using COTG.Game;
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

namespace COTG.Views
{

    public class DumbCollection<T> : List<T>, INotifyCollectionChanged
    {
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
        public void NotifyChange(T item)
        {
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,item,item,IndexOf(item)));
        }
        public void NotifyAdd(T item)
        {
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,item, IndexOf(item)));
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public DumbCollection<City> cities { get; } = new DumbCollection<City>();
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

        public MainPage()
        {
            Assert(cache == null);
            cache = this;
            InitializeComponent();
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

        public async static void CityChange( City city)
        {
            if (cache == null)
                return;

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                var cities = cache.cities;
                if (cities.Contains(city))
                {
                    cities.NotifyChange(city);
                }
                else
                {
                    cities.Add(city);
                    cities.NotifyAdd(city);

                }
            });
        }
        public async static void CityListChange()
        {
            if (cache == null)
                return;

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                var cities = cache.cities;
                cities.Clear();
                cities.AddRange(City.all.Values); // use the most reset city list

                cache.cities.NotifyReset();
            });
        }
        public static void UpdateDungeonList(List<Dungeon> dungeons)
        {
            if (cache == null)
                return;

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
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
        public void OnCityGridChange()  {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(cities)));
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
