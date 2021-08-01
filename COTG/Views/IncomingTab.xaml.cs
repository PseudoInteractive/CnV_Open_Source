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
//			spotGrids.Add(defenderGrid);

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

		public static Spot selected
		{
			get
			{
				if (!instance.isVisible)
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
                    var sel = defenderGrid.SelectedItems.ToArray();
					//					lastSelected = sel;

					defenderGrid.ItemsSource = Spot.defendersI.Where(w => w.testContinentFilter && (includeInternal||w.hasEnemyIncoming)&&(typeFilter == 2 ? w.pid == Player.activeId : typeFilter == 1 ? SettingsPage.incomingWatch.Contains(w.playerName) : true)).ToArray(); 
                    if (sel.Length > 0)
                    {
                        App.DispatchOnUIThreadSneaky(() =>
                        {
							++SpotTab.silenceSelectionChanges;
							try
							{
								defenderGrid.SelectItems(sel);
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

        public override Task VisibilityChanged(bool visible)
        {
			//  Log("Vis change" + visible);
			//App.DispatchOnUIThreadSneaky(() =>
			//{
			//    defenderGrid.ItemsSource = null;
			//    armyGrid.ItemsSource = Army.empty;
			//});

			if (visible)
			{
				lastSelected = null;
				IncomingOverview.Process(false);
			}
			return base.VisibilityChanged(visible);

        }
        public static bool IsVisible() => instance.isVisible;



		private void defenderGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
			if (!isActive)
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
							if (!tab.isVisible)
							{
								tab.ShowOrAdd(true, false);

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
