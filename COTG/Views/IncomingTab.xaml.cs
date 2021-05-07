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
			spotGrids.Add(defenderGrid);

			defenderGrid.OnKey = Spot.OnKeyDown;
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

        
        private void gridPointerPress(object sender, PointerRoutedEventArgs e)
        {
            (var hit,var column,var pointerPoint,_) = Spot.HitTest(sender, e);
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

		public static Spot selected => instance.defenderGrid.SelectedItem as Spot;

        public void NotifyIncomingUpdated()
        {
            if (IncomingTab.IsVisible())
            {
                try
                {
                    var sel = defenderGrid.SelectedItem as Spot;
                    defenderGrid.ItemsSource = onlyMe.IsChecked.GetValueOrDefault() ? Spot.defendersI.Where(w=>w.pid==Player.activeId).ToArray() : Spot.defendersI;
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

        public override void VisibilityChanged(bool visible)
        {
            //  Log("Vis change" + visible);
            App.DispatchOnUIThreadSneaky(() =>
            {
                defenderGrid.ItemsSource = null;
                armyGrid.ItemsSource = Army.empty;
            });
            if (visible)
                IncomingOverview.Process(false,true);
            base.VisibilityChanged(visible);

        }
        public static bool IsVisible() => instance.isVisible;


		private static Spot SelectionChanged( DataGridSelectionChangedEventArgs e, RadDataGrid grid)
		{
			foreach (Spot s in e.RemovedItems)
			{
				s.isSelected = false;
			}
			foreach (Spot s in e.AddedItems)
			{
				s.isSelected = true;
			}

			return grid.SelectedItem as Spot;
		}

		private void defenderGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
			var sel = SelectionChanged(e, defenderGrid);
			if (sel != null )
            {
				armyGrid.ItemsSource = sel.incoming;
				
				var tab = DefenseHistoryTab.instance;
				if (!tab.isActive)
				{
					tab.ShowOrAdd( true,true,TabPage.secondaryTabs );

				}
				if(tab.isVisible)
				{
					tab.Refresh();
				}

			}
        }

		private void onlyMe_Click(object sender, RoutedEventArgs e)
		{
			instance.Refresh();
		}
	}

   

}
