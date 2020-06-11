using COTG.Game;
using COTG.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using Xunit;
using Telerik.UI.Xaml.Controls.Grid;
using static COTG.Debug;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Telerik.Core.Data;
using System.Collections.Specialized;

namespace COTG.Views
{
    public class DumbCollection<T> : List<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyChange()
        {
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset ));

        }
    }

    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public DumbCollection<City> cities { get; } = new DumbCollection<City>();
        public static MainPage cache;

        public static City showingRowDetails;

        public MainPage()
        {
            Assert.True(cache == null);
            cache = this;
            InitializeComponent();
        }

        private void OnCheckBoxClick(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            if (showingRowDetails != null && cities.Contains(showingRowDetails) )
                this.cityGrid.HideRowDetailsForItem(showingRowDetails);
            showingRowDetails = null;

            var newCheckedItem = (City)cb.DataContext;

            if (cb.IsChecked.HasValue && cb.IsChecked.Value)
            {
                showingRowDetails = newCheckedItem;
                this.cityGrid.ShowRowDetailsForItem(newCheckedItem);
            }

        }

        public async static void UpdateCityList()
        {
            if (cache == null)
                return;

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    var cities = cache.cities;
                    cities.Clear();
                    lock (City.all)
                    {
                        cities.AddRange(City.all.Values);
                    }
                    cities.NotifyChange();
                });

            
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
    }
}
