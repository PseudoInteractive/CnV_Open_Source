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
using Telerik.UI.Xaml.Controls.Grid.Commands;
using System.Threading.Tasks;

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
//			spotGrids.Add(attackerGrid);

			attackerGrid.OnKey = Spot.OnKeyDown;
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
                    App.DispatchOnUIThreadSneaky(() =>
                    {
                        instance.attackerGrid.ItemsSource = Spot.defendersO;
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

        public override Task VisibilityChanged(bool visible)
        {
			/// TODO:  Why clear?
            //App.DispatchOnUIThreadSneaky(() =>
            //{
            //    attackerGrid.ItemsSource = null;
            //    armyGrid.ItemsSource = Army.empty;
            //});

            if (visible)
                OutgoingOverview.Process(false);
            return base.VisibilityChanged(visible);

        }
        public static bool IsVisible() => instance.isVisible;

        private void defenderGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
		{
			if (!isActive)
				return;

			var sel = attackerGrid.SelectedItem as Spot;
            if(sel==null)
            {
           //     armyGrid.ItemsSource = Army.empty;
            }
            else
            {
                armyGrid.ItemsSource = sel.incoming;

				var tab = HitTab.instance;
				if (!tab.isActive)
				{
					tab.ShowOrAdd(true, true);

				}
				if (tab.isVisible)
				{
					tab.Refresh();
				}

			}
		}
    }
       

}
