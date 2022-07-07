using System.Runtime.CompilerServices;

using Microsoft.UI.Xaml.Controls;
//using Windows.UI.Core;
using Microsoft.UI.Xaml;

namespace CnV.Views
{
	using Syncfusion.UI.Xaml.Grids;

	public sealed partial class IncomingTab:UserTab
	{
		internal Alliance.CityFilter filterTo = Alliance.CityFilter.allied;
		
		public int _filterTo {
			get => (int)filterTo;
			set => filterTo = (Alliance.CityFilter)value;
		}

		public static Spot lastSelected;
		public static IncomingTab? instance;

		internal ObservableCollection<City> citiesWithIncoming = new();
		internal ObservableCollection<Army> armiesIncoming = new();

		public bool includeInternal { get; set; } = true;
		//        public static Report showingRowDetails;

		//public DataTemplate GetTsInfoDataTemplate()
		//{
		//    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
		//    Assert(rv != null);
		//    return rv;
		//}
		public IncomingTab() {
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


			protected override Style? SelectStyleCore(object item,DependencyObject container) {
				try {
					var report = (item as BattleReport);
					if(report is null)
						return null;
					return report.type switch {

						ArmyType.assault => assaultStyle,
						ArmyType.siege when report.attackArmy.isSieging => siegingStyle,
						ArmyType.siege => siegeStyle,
						ArmyType.plunder => plunderStyle,
						ArmyType.scout => scoutStyle,
						_ => pendingStyle
					};
				}
				catch(Exception _ex) {
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












		//private void Set<T>(ref T storage,T value,[CallerMemberName] string propertyName = null) {
		//	if(Equals(storage,value)) {
		//		return;
		//	}
		//	storage = value;
		//	OnPropertyChanged(propertyName);
		//}


		// we want current not selected
		

		public static (int forAlliance, int forMe, ServerTime nextArrival) GetIncomingCounts() {
			int forAlliance = 0;
			int forMe = 0;

			ServerTime nextArrival = ServerTime.infinity;
			foreach(var c in City.alliedCities) {
				foreach(var i in c.incoming) {
					if(!i.isAttack)
						continue;
					if(i.arrival > Sim.simTime + c.scoutRange || !i.departed)
						continue;

					forAlliance++;
					if(c.isSubOrMine)
						++forMe;
					nextArrival = nextArrival.Min(i.arrival);
				}
			}

			return (forAlliance, forMe, nextArrival);
		}

		static int lastAllianceIncoming=-1;
		internal static void UpdateIncomingStatus() {
			var c = GetIncomingCounts();
			if(c.forAlliance != lastAllianceIncoming) {
				lastAllianceIncoming = c.forAlliance;
				NotifyIncomingUpdated();
			}
			
			AppS.QueueIdleTask( UpdateIncomingStatus, 3*1000*60 ); // every three minutes

		}

		public static void NotifyIncomingUpdated() {


			//					lastSelected = sel;

			if(!Sim.isInteractiveOrHistoric && PlayerStats.instance is not null)
				return;

				AppS.QueueOnUIThread(() => {
					
					PlayerStats.instance?.UpdateIncomingText();
					if(IncomingTab.IsVisible()) {
						try {
							var includeInternal = instance.includeInternal;
							var _targets = instance.filterTo.GetCityList();

							var newItems = _targets.Where(w => w.testContinentFilter
															&& w.HasIncomingAttacks(includeInternal)
															).OrderBy(w => w.firstIncoming).ToArray();
							//var sel = defenderGrid.SelectedItems.ToArray();
							//++SpotTab.silenceSelectionChanges;

							if( newItems.SyncList(instance.citiesWithIncoming) ) {
								instance.defenderGrid.ResetAutoColumns(true);

							}

							var sel = instance.defenderGrid.SelectedItem;
							if(sel is not null && newItems.Contains(sel)) {

								instance.defenderGrid.ScrollItemIntoView(sel,true);

							}
							instance.UpdateArmyGrid(true,false);
						}
						catch(Exception ex) { LogEx(ex); }

					}
					
				});
			
		}





		public override Task VisibilityChanged(bool visible,bool longTerm) {
			//  Log("Vis change" + visible);
			//AppS.DispatchOnUIThreadLow(() =>
			//{
			//    defenderGrid.ItemsSource = null;
			//    armyGrid.ItemsSource = Army.empty;
			//});

			if(visible) {
				//	lastSelected = null;
				NotifyIncomingUpdated();
				//AppS.QueueOnUIThreadIdle(() => UpdateArmyGrid(true,false));
			}
			return base.VisibilityChanged(visible,longTerm: longTerm);

		}
		public static bool IsVisible() =>  instance is not null && instance.isFocused;



		private void defenderGrid_SelectionChanged(object sender,GridSelectionChangedEventArgs e) {
			if(!isFocused)
				return;
			if(SpotTab.silenceSelectionChanges == 0) {
				UpdateArmyGrid(false,true);
				//	SpotSelectionChanged(sender, e);
			}
		}

		internal void UpdateArmyGrid(bool force,bool updatehistoryTab) {
			var sel = defenderGrid.SelectedItem as Spot;;
			var changed = sel != lastSelected;
			if(changed || force) {
				lastSelected = sel;
				if(sel != null) {
					var visibilityTime = Sim.simTime + sel.scoutRange;
					var items=  sel.incoming.Where(a => a.isDefense || (a.arrival <= visibilityTime && a.departed) ).OrderBy(a => a.arrival).ThenBy(a=> a.isDefense ? 0 : 1).ToArray();
					if(items.SyncList(armiesIncoming)) {
						armyGrid.ResetAutoColumns();
					}
					if(updatehistoryTab) {
						HitHistoryTab.SetFilter(sel);
						
						
					}
				}
				else {
					armiesIncoming.Clear();
				}
			}
		}

		private void filterChanged(object sender,RoutedEventArgs e) {
			refresh.Go();
		}
			public override async Task Closed()
		{ 
			await base.Closed();
			instance = null;
		}
		private void ExportSheetsClick(object sender,RoutedEventArgs e) {
			defenderGrid.SelectAll();
			var sel = defenderGrid.SelectedItems;
			List<int> cids = new();
			foreach(var c in sel) {
				cids.Add((c as City).cid);
			}
			Spot.ExportToDefenseSheet(cids);
		}


	}



}
