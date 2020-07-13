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
using Telerik.UI.Xaml.Controls.Input;
using COTG.Helpers;
using Windows.UI.Xaml.Navigation;

namespace COTG.Views
{

    public sealed partial class DefenderPage : Page, INotifyPropertyChanged
    {

        public static DefenderPage instance;
        public DumbCollection<Spot> defenders = new DumbCollection<Spot>();
        //        public static Report showingRowDetails;

        //public DataTemplate GetTsInfoDataTemplate()
        //{
        //    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
        //    Assert(rv != null);
        //    return rv;
        //}
        public DefenderPage()
        {
            Assert(instance == null);
            instance = this;

            InitializeComponent();

            //            historyGrid.ContextFlyout = cityMenuFlyout;

        }

        


        private void gridPointerPress(object sender, PointerRoutedEventArgs e)
        {
            (var hit,var column,var pointerPoint) = Spot.HitTest(sender, e);
            if (hit != null)
                defenderGrid.ShowRowDetailsForItem(hit);

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            defenders.Clear();
            foreach(var spot in Spot.allSpots)
            {
                if (spot.Value.incoming.Count > 0)
                    defenders.Add(spot.Value);
            }
            defenders.NotifyReset();
        }

        private void ArmyTapped(object sender, TappedRoutedEventArgs e)
        {
            Log("army Tapped");
                (var hit, var column, var pointerPoint) = Army.HitTest(sender, e);
                if (hit != null)
                    hit.ProcessTap(column);

          
        }
    }
}
