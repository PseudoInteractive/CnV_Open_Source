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

namespace COTG.Views
{

    public sealed partial class OutgoingTab : UserTab
    {

        public static OutgoingTab instance;
        //        public static Report showingRowDetails;

        //public DataTemplate GetTsInfoDataTemplate()
        //{
        //    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
        //    Assert(rv != null);
        //    return rv;
        //}
        public OutgoingTab()
        {
            Assert(instance == null);
            instance = this;

            InitializeComponent();

            //            historyGrid.ContextFlyout = cityMenuFlyout;

            //      var data = defenderGrid.GetDataView();
        }

        


        private void gridPointerPress(object sender, PointerRoutedEventArgs e)
        {
            (var hit, var column, var pointerPoint,_) = Spot.HitTest(sender, e);
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






        public static void NotifyOutgoingUpdated()
        {
            if (OutgoingTab.IsVisible())
            {
                try
                {
                    App.DispatchOnUIThreadSneaky(() =>
                    {
                        var defenders = new List<Spot>();
                        foreach (var spot in Spot.allSpots)
                        {
                            if (spot.Value.incoming.Length > 0)
                                defenders.Add(spot.Value);
                        }
                        instance.defenderGrid.ItemsSource = defenders;
                    });
                }
                catch (Exception e)
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
            App.DispatchOnUIThreadSneaky(() =>
            {
                defenderGrid.ItemsSource = Spot.emptySpotSource;
                armyGrid.ItemsSource = Army.empty;
            });

            if (visible)
                OutgoingOverview.Process(false);
            base.VisibilityChanged(visible);

        }
        public static bool IsVisible() => instance.isVisible;

        private void defenderGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            var sel = defenderGrid.SelectedItem as Spot;
            if(sel==null)
            {
                armyGrid.ItemsSource = Army.empty;
            }
            else
            {
                armyGrid.ItemsSource = sel.incoming;
            }
        }
    }
       

}
