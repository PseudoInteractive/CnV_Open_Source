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
using Microsoft.Toolkit.Uwp;
using Windows.UI.Xaml.Input;
using COTG.Services;
using System.Collections;
using COTG.JSON;
using Windows.UI.Input;
using Telerik.UI.Xaml.Controls.Input;
using COTG.Helpers;
using Windows.UI.Xaml.Navigation;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using static COTG.Game.Enum;
using System.Threading.Tasks;
using System.Linq;

namespace COTG.Views
{

    public sealed partial class IncomingTab : UserTab, INotifyPropertyChanged
    {

        public static IncomingTab instance;
        //        public static Report showingRowDetails;

        //public DataTemplate GetTsInfoDataTemplate()
        //{
        //    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
        //    Assert(rv != null);
        //    return rv;
        //}
        public IncomingTab()
        {
            Assert(instance == null);
            instance = this;

            InitializeComponent();

            //            historyGrid.ContextFlyout = cityMenuFlyout;

      //      var data = defenderGrid.GetDataView();
        }

        public class ArmyTypeStyleSelector : StyleSelector
        {
            public Style pendingStyle { get; set; }

            public Style siegingStyle { get; set; }
            public Style siegeStyle { get; set; }

            public Style scoutStyle { get; set; }
            public Style assaultStyle { get; set; }
            public Style plunderStyle { get; set; }


            protected override Style SelectStyleCore(object item, DependencyObject container)
            {
                var cell = (item as DataGridCellInfo);
                var report = cell.Item as Army;
                switch (report.type)
                {
                    case reportAssault: return assaultStyle;
                    case reportSiege: return siegeStyle;
                    case reportSieging: return siegingStyle;
                    case reportPlunder: return plunderStyle;
                    default: return scoutStyle;
                }

            }
        }

        public static async Task<IncomingTab> Show()
        {
            var tab = IncomingTab.instance;
            for (; ; )
            {
                if (tab != null)
                    break;
                await Task.Delay(1000);
                tab = IncomingTab.instance;
            }
            if (!tab.isActive)
            {
                TabPage.mainTabs.AddTab(tab, true);
            }
            else
            {
                if (!tab.isVisible)
                    TabPage.Show(tab);
            //    else
              //      tab.Refresh();
            }

            return tab;
        }
        private void gridPointerPress(object sender, PointerRoutedEventArgs e)
        {
            (var hit,var column,var pointerPoint,_) = Spot.HitTest(sender, e);
            //if (hit != null)
            //    defenderGrid.ShowRowDetailsForItem(hit);

            Spot.ProcessPointerPress(sender, e);
        }
        //private void gridPointerMoved(object sender, PointerRoutedEventArgs e)
        //{
        //    Spot.ProcessPointerMoved(sender, e);
        //}
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


        public void NotifyIncomingUpdated()
        {
            if (IncomingTab.IsVisible())
            {
                try
                {
                    var sel = defenderGrid.SelectedItem as Spot;
                    defenderGrid.ItemsSource = Spot.defendersI;
                    if (sel!=null)
                    {
                       
                        App.DispatchOnUIThreadSneaky(() =>
                        {
                            defenderGrid.SelectItem(sel);
                            defenderGrid.ScrollItemIntoView(sel);
                        });

                    }
                }
                catch( Exception e)
                {
                    Log(e);
                }
            }
        }

        

        private void ArmyTapped(object sender, TappedRoutedEventArgs e)
        {
                (var hit, var column, var pointerPoint) = Army.HitTest(sender, e);
                if (hit != null)
                    hit.ProcessTap(column);

          
        }

        public override void VisibilityChanged(bool visible)
        {
            //  Log("Vis change" + visible);
            App.DispatchOnUIThreadSneaky(() =>
            {
                defenderGrid.ItemsSource = Spot.emptySpotSource;
                armyGrid.ItemsSource = Army.empty;
            });
            if (visible)
                IncomingOverview.Process(false,true);
            base.VisibilityChanged(visible);

        }
        public static bool IsVisible() => instance.isVisible;

        private void defenderGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            var sel = defenderGrid.SelectedItems;
            
            var newSel = new HashSet<int>();
            foreach (object s in sel)
            {
                newSel.Add( (s as Spot).cid );

            }


            Spot.selected = newSel;

            if (sel.Any())
            {
                armyGrid.ItemsSource = (sel.First() as Spot).incoming;
            }
        }
    }

   

}