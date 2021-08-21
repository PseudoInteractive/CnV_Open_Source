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
using System.Threading.Tasks;
using System.Linq;

namespace COTG.Views
{

    public sealed partial class OutgoingTab : UserTab
    {
		public static Spot lastSelected;
		bool includeInternal;
		bool onlyMine;

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
//			spotGrids.Add(attackerGrid);

		//	attackerGrid.OnKey = Spot.OnKeyDown;
            //            historyGrid.ContextFlyout = cityMenuFlyout;

            //      var data = defenderGrid.GetDataView();
        }



		public static Spot selected => instance.attackerGrid.SelectedItem as Spot;
		private void gridPointerPress(object sender, PointerRoutedEventArgs e)
        {
            (var hit, var column, var pointerPoint,_) = Spot.HitTest(sender, e);
            //if (hit != null)
            //    defenderGrid.ShowRowDetailsForItem(hit);

            Spot.GridPressed(sender, e);
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
                    App.DispatchOnUIThreadLow(() =>
                    {
                        instance.attackerGrid.ItemsSource = Spot.defendersO.Where( w => w.testContinentFilter 
																					&& (instance.includeInternal || !w.IsAllyOrNap() ) 
																					&& (!instance.onlyMine ||w.HasIncomingFrom(Player.activeId))).OrderBy(w=>w.firstIncoming).ToArray();
                    });
                }
                catch (Exception e)
                {
                    LogEx(e);
                }
            }
        }



        private void ArmyTapped(object sender, TappedRoutedEventArgs e)
        {
            (var hit, var column, var pointerPoint) = Army.HitTest(sender, e);
            if (hit != null)
                hit.ProcessTap(column);


        }

        public override Task VisibilityChanged(bool visible, bool longTerm)
		{
			/// TODO:  Why clear?
            //App.DispatchOnUIThreadLow(() =>
            //{
            //    attackerGrid.ItemsSource = null;
            //    armyGrid.ItemsSource = Army.empty;
            //});

            if (visible)
                OutgoingOverview.OutgoingUpdateDebounce.Go();
            return base.VisibilityChanged(visible, longTerm: longTerm);

        }


		public static bool IsVisible() => instance.isVisible;

		static Debounce selChanged = new Debounce(SelChanged) { runOnUiThead = true };

		static Task SelChanged()
		{
			var sel = lastSelected;
			if (sel != null)
			{
				instance.armyGrid.ItemsSource = sel.incoming;
				if (SettingsPage.fetchFullHistory)
				{
					var tab = HitTab.instance;
					if (!instance.isVisible)
					{
						tab.ShowOrAdd(true, onlyIfClosed: true);

					}
					else
					{
						tab.refresh.Go();
					}
				}
			}
			return Task.CompletedTask;
		}
		

        private void defenderGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
		{
			if (!isActive)
				return;

			var sel = attackerGrid.SelectedItem as Spot;
           // if(sel==null)
           // {
           ////     armyGrid.ItemsSource = Army.empty;
           // }
           // else
            {
				if (lastSelected == sel)
					return;
				lastSelected = sel;

				selChanged.Go();

			}
		}

		private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
		{
			instance.refresh.Go();
		}
	}
       

}
