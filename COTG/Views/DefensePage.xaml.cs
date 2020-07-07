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
using COTG.JSON;
using Windows.UI.Input;

namespace COTG.Views
{

    public sealed partial class DefensePage : Page, INotifyPropertyChanged
    {
        public DumbCollection<Report> history { get; } = new DumbCollection<Report>();
        public ObservableCollection<Spot> defenders { get; } = new ObservableCollection<Spot>();

        public static ObservableCollection<Spot> Defenders => instance.defenders;


        public static DefensePage instance;
        public static RadDataGrid HistoryGrid => instance.historyGrid;
        //        public static Report showingRowDetails;

        //public DataTemplate GetTsInfoDataTemplate()
        //{
        //    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
        //    Assert(rv != null);
        //    return rv;
        //}
        public DefensePage()
        {
            Assert(instance == null);
            instance = this;

            InitializeComponent();
           
//            historyGrid.ContextFlyout = cityMenuFlyout;


        }
        private void gridPointerPress(object sender, PointerRoutedEventArgs e)
        {
            Spot.ProcessPointerPress(sender, e);
        }
        private void gridPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Spot.ProcessPointerMoved(sender, e);
        }
        private void gridPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Spot.ProcessPointerExited();
        }

        public static Spot GetDefender(int cid)
        {
            // Toggle Selected
            if (!Spot.allSpots.TryGetValue(cid, out var rv))
            {
                if (City.all.TryGetValue(cid, out var city))
                    rv = city; // re-use existing one if it is exists (this occurs for the players own cities)
                else
                    rv = new Spot() { cid = cid };
                Spot.allSpots.TryAdd(cid, rv);

                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Defenders.Add(rv);
                });

            }
           // Spot.ToggleSelected(cid);
            return rv;

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

      
    }
}
