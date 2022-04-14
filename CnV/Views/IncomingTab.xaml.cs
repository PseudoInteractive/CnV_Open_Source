﻿using System.Runtime.CompilerServices;

using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
//using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using static CnV.Troops;
using System.Threading.Tasks;

namespace CnV.Views
{
	using Game;
	using Syncfusion.UI.Xaml.DataGrid;
	using Syncfusion.UI.Xaml.Grids;

	public sealed partial class IncomingTab:UserTab
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
			SetupDataGrid(armyGrid);
			//			spotGrids.Add(defenderGrid);

			//defenderGrid.OnKey = Spot.OnKeyDown;
			//            historyGrid.ContextFlyout = cityMenuFlyout;

			//      var data = defenderGrid.GetDataView();
		}

		public class ArmyTypeStyleSelector:StyleSelector
		{
			public Style? pendingStyle { get; set; }

			public Style? siegingStyle { get; set; }
			public Style? siegeStyle { get; set; }

			public Style? scoutStyle { get; set; }
			public Style? assaultStyle { get; set; }
			public Style? plunderStyle { get; set; }


			protected override Style? SelectStyleCore(object item,DependencyObject container)
			{
				try
				{
					var report = (item as BattleReport);
					if(report is null)
						return null;
					return report.type switch
					{

						ArmyType.assault => assaultStyle,
						ArmyType.siege when report.attackArmy.isSieging => siegingStyle,
						ArmyType.siege => siegeStyle,
						ArmyType.plunder => plunderStyle,
						ArmyType.scout => scoutStyle,
						_=>pendingStyle
					};
				}
				catch(Exception _ex)
				{
					LogEx(_ex);
					return scoutStyle;
				}
			}
		}


		//private void gridPointerPress(object sender, PointerRoutedEventArgs e)
		//{
		//	//  (var hit,var column,var pointerPoint,_) = Spot.HitTest(sender, e);
		//	//if (hit != null)
		//	//    defenderGrid.ShowRowDetailsForItem(hit);

		//	Spot.GridPressed(sender, e);
		//}
		////private void gridPointerMoved(object sender, PointerRoutedEventArgs e)
		////{
		////    Spot.ProcessPointerMoved(sender, e);
		////}
		//private void gridPointerExited(object sender, PointerRoutedEventArgs e)
		//{
		//	Spot.ProcessPointerExited();
		//}












		private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if(Equals(storage, value))
			{
				return;
			}
			storage = value;
			OnPropertyChanged(propertyName);
		}


		// we want current not selected
		public static Spot selected
		{
			get {
				if(!instance.isFocused)
					return null;
				return instance.defenderGrid.CurrentItem as Spot;
			}
		}
		public void NotifyIncomingUpdated()
		{
			if(IncomingTab.IsVisible())
			{

				//					lastSelected = sel;



				AppS.QueueOnUIThreadIdle(() =>
				{
					try
					{

						var newItems = City.allianceCities.Where(w => w.testContinentFilter
														&& (includeInternal||w.hasEnemyIncoming)
														&&(typeFilter == 2 ? w.pid == Player.myId
														: typeFilter == 1 ? Settings.incomingWatch.Contains(w.playerName)|| w.pid == Player.myId
														: true)).OrderBy(w => w.firstIncoming).ToArray();
						var sel = defenderGrid.SelectedItems.ToArray();
						++SpotTab.silenceSelectionChanges;
							
					    // remove selections that are gone
						foreach(var i in sel)
						{
							var c = i as City;
							if(!newItems.Contains(c)  )
							{
								defenderGrid.SelectedItems.Remove(i);
							}
						}

						defenderGrid.ItemsSource = newItems;
						if(defenderGrid.SelectedItems.Count > 0)
						{


							defenderGrid.ScrollItemIntoView(defenderGrid.SelectedItems[0]);
							
						};

					}

			
				catch(Exception e)
				{
					LogEx(e);
				} 
				finally
					{
						--SpotTab.silenceSelectionChanges; 

					}
			});
			}
		}





		public override Task VisibilityChanged(bool visible, bool longTerm)
		{
			//  Log("Vis change" + visible);
			//AppS.DispatchOnUIThreadLow(() =>
			//{
			//    defenderGrid.ItemsSource = null;
			//    armyGrid.ItemsSource = Army.empty;
			//});

			if(visible)
			{
			//	lastSelected = null;
				NotifyIncomingUpdated();
				AppS.QueueOnUIThreadIdle(() =>UpdateArmyGrid(true) );
			}
			return base.VisibilityChanged(visible, longTerm: longTerm);

		}
		public static bool IsVisible() => instance.isFocused;



		private void defenderGrid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
		{
			if(!isFocused)
				return;
			if(SpotTab.silenceSelectionChanges == 0)
			{
				UpdateArmyGrid(false);
				//	SpotSelectionChanged(sender, e);
			}
		}

		private void UpdateArmyGrid(bool force)
		{
			var sel = selected;
			var changed = sel != null && sel != lastSelected;
			if(changed || force)
			{
				lastSelected = sel;
				if(sel != null)
				{
					armyGrid.ItemsSource = sel.incoming;

					if(Settings.fetchFullHistory)
					{
						var tab = HitHistoryTab.instance;
						if(!tab.isFocused)
						{
							tab.ShowOrAdd(true,onlyIfClosed: true);

						}
						else
						{
							tab.refresh.Go();
						}
					}
				}
				else
				{
					armyGrid.ItemsSource = Army.EmptyArray;
				}
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
