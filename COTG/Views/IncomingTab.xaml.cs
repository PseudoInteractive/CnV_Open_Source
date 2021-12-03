using CnV.Game;
using CnV.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Microsoft.UI.Xaml.Controls;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Grid;
using static CnV.Debug;
using Windows.ApplicationModel.Core;
//using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Telerik.Core.Data;
using Telerik.Data.Core;
using Telerik.Data;
using System.Collections.Specialized;
using Windows.Foundation;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Input;
using CnV.Services;
using System.Collections;

using Windows.UI.Input;
using Telerik.UI.Xaml.Controls.Input;
using CnV.Helpers;
using Microsoft.UI.Xaml.Navigation;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using static CnV.Game.Troops;
using System.Threading.Tasks;
using System.Linq;

namespace CnV.Views
{
	using Game;

	public sealed partial class IncomingTab : UserTab
    {
		public int typeFilter { get; set; }
		public static Spot lastSelected;
        public static IncomingTab instance;
		public bool includeInternal { get; set; }
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
			SetupDataGrid(defenderGrid);
//			spotGrids.Add(defenderGrid);

			//defenderGrid.OnKey = Spot.OnKeyDown;
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
          //  (var hit,var column,var pointerPoint,_) = Spot.HitTest(sender, e);
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


       








 
        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }
            storage = value;
            OnPropertyChanged(propertyName);
        }

        
		public static Spot selected
		{
			get
			{
				if (!instance.isFocused)
					return null;
				var items =  instance.defenderGrid.SelectedItems;
				if (items.Count == 1)
					return items[0] as Spot;
				return null;
			}
		}
        public void NotifyIncomingUpdated()
        {
            if (IncomingTab.IsVisible())
            {
                try
                {
					//					lastSelected = sel;

					defenderGrid.ItemsSource = Spot.defendersI.Where(w => w.testContinentFilter 
													&& (includeInternal||w.hasEnemyIncoming)
													&&(typeFilter == 2 ? w.pid == Player.activeId 
													: typeFilter == 1 ? SettingsPage.incomingWatch.Contains(w.playerName)|| w.pid == Player.activeId 
													: true)).OrderBy(w=>w.firstIncoming).ToArray();
					var sel = defenderGrid.SelectedItems.ToArray();
					if (sel.Length > 0)
                    {
                        AppS.DispatchOnUIThreadLow(() =>
                        {
							++SpotTab.silenceSelectionChanges;
							try
							{
								defenderGrid.DeselectAll();
								foreach(var i in sel)
									defenderGrid.SelectItem(i);
									if (sel.Length == 1)
									{
										defenderGrid.ScrollItemIntoView(sel[0]);
									}
							}
							finally
							{
								--SpotTab.silenceSelectionChanges;
							}
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

        public override Task VisibilityChanged(bool visible, bool longTerm)
		{
			//  Log("Vis change" + visible);
			//AppS.DispatchOnUIThreadLow(() =>
			//{
			//    defenderGrid.ItemsSource = null;
			//    armyGrid.ItemsSource = Army.empty;
			//});

			if (visible)
			{
				lastSelected = null;
				IncomingOverview.Process(false);
			}
			return base.VisibilityChanged(visible, longTerm: longTerm);

        }
        public static bool IsVisible() => instance.isFocused;



		private void defenderGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
			if (!isOpen)
				return;
			if (SpotTab.silenceSelectionChanges == 0)
			{
				var sel = selected;
				var changed = sel != null && sel != lastSelected;
				if (changed)
				{
					lastSelected = sel;
					if (sel != null)
					{
						armyGrid.ItemsSource = sel.incoming;

						if (SettingsPage.fetchFullHistory)
						{
							var tab = DefenseHistoryTab.instance;
							if (!tab.isFocused)
							{
								tab.ShowOrAdd(true, onlyIfClosed:true);

							}
							else
							{
								tab.refresh.Go();
							}
						}
					}
				}
				SpotSelectionChanged(sender, e);
			}
		}

		private void filterChanged(object sender, RoutedEventArgs e)
		{
			instance.refresh.Go();
		}

		private void ExportSheetsClick(object sender, RoutedEventArgs e)
		{
			defenderGrid.SelectAll();
			var sel = defenderGrid.SelectedItems;
			List<int> cids = new();
			foreach(var c in sel)
			{
				cids.Add((c as City).cid);
			}
			Spot.ExportToDefenseSheet(cids);
		}
	}

   

}
