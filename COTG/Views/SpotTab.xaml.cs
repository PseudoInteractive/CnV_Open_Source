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

namespace COTG.Views
{
    public sealed partial class SpotTab : UserTab
    {
        public DumbCollection<Spot> spotMRU { get; } = new DumbCollection<Spot>();

        public static DumbCollection<Spot> SpotMRU => instance.spotMRU;

        public static SpotTab instance;
        public SpotTab()
        {
            this.InitializeComponent();
            selectedGrid.SelectionChanged += DefenderGrid_SelectionChanged;

        }
        public static bool IsVisible() => instance.isVisible;


        private void DefenderGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            foreach (var __item in e.RemovedItems)
            {
                var item = __item as Spot;
                Spot.selected.Remove(item.cid);

            }
            foreach (var __item in e.AddedItems)
            {
                var item = __item as Spot;
                Spot.selected.Add(item.cid);
            }
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

        public static Spot TouchSpot(int cid)
        {
            var spot = Spot.GetOrAdd(cid);
            AddToGrid(spot);
            return spot;

        }
        public static void AddToGrid(Spot spot)
        {
            // Toggle Selected

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                var cid = spot.cid;
                var def = SpotMRU;
                if (!def.Contains(spot))
                {
                    def.Add(spot);
                    instance.selectedGrid.SelectItem(spot);
                }
                else
                {
                    ToggleSelected(spot);

                }
            });

        }
        public static void ToggleSelected(Spot rv)
        {
            var isSelected = rv.ToggleSelected();
            if (isSelected)
                instance.selectedGrid.SelectItem(rv);
            else
                instance.selectedGrid.DeselectItem(rv);

        }


    }
}
