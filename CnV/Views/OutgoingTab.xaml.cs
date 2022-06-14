﻿//using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

//using Windows.UI.Input;
using System.Threading.Tasks;

namespace CnV.Views
{
	using Game;
	using Syncfusion.UI.Xaml.Grids;

	public sealed partial class OutgoingTab : UserTab
    {
		bool includeInternal=true;
		int filterFrom = filterFromAllied;
		const int filterFromMe = 0;
		const int filterFromSubs = 1;
		const int filterFromAllied = 2;
		const int filterFromAlliance = 3;
		const int filterFromAll = 4;


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
			SetupDataGrid(attackerGrid);
			SetupDataGrid(armyGrid);
//			spotGrids.Add(attackerGrid);

		//	attackerGrid.OnKey = Spot.OnKeyDown;
            //            historyGrid.ContextFlyout = cityMenuFlyout;

            //      var data = defenderGrid.GetDataView();
        }



		public static Spot selected 
		{
			get {
				if(!instance.isFocused)
					return null;
				return  instance.attackerGrid.CurrentItem as Spot;
			}
		}


		ObservableCollection<City> targets = new();
		ObservableCollection<Army> targetIncoming = new();

		public static (int forAlliance, int forMe, ServerTime nextArrival) GetOutgoingCounts() {
			int allianceOutgoing = 0;
			int myOutgoing = 0;
			ServerTime nextArrival = ServerTime.infinity;
			foreach(var c in City.alliedCities) {
				foreach(var o in c.outgoing)
						{
					if(o.isOutgoingAttack) {
						allianceOutgoing++;
						if(c.isSubOrMine)
							++myOutgoing;
						nextArrival = nextArrival.Min(o.arrival);
					}
				}
			}
			return (allianceOutgoing, myOutgoing,nextArrival);
		}

		public static void NotifyOutgoingUpdated()
        {
			if(!Sim.isPastWarmup)
				return;

				AppS.QueueOnUIThread(() => {
					PlayerStats.instance.UpdateOutgoingText();
					if(OutgoingTab.IsVisible()) {

						try {
							var _targets = (instance.filterFrom switch {
								filterFromMe => City.myCities,
								filterFromSubs => City.subCities,
								filterFromAllied => City.alliedCities,
								_ => City.allianceCities

							}).SelectMany(c => c.outgoing.Where(a => a.isOutgoingAttack).Select(a => a.targetCity)).Distinct().
							Where(w =>
																							  w.testContinentFilter
																							&& (instance.includeInternal || !w.isAllyOrNap)).OrderBy(w => w.firstIncoming).ToArray();
							_targets.SyncList(instance.targets);
							instance.attackerGrid.ResetAutoColumns();
							UpdateTargetIncoming(false);
						}
						catch(Exception e) {
							LogEx(e);
						}
					}
				}
				);
			
            
        }

		

        public override Task VisibilityChanged(bool visible, bool longTerm)
		{
			/// TODO:  Why clear?
			//AppS.DispatchOnUIThreadLow(() =>
			//{
			//    attackerGrid.ItemsSource = null;
			//    armyGrid.ItemsSource = Army.empty;
			//});

			if(visible)
				NotifyOutgoingUpdated();
            return base.VisibilityChanged(visible, longTerm: longTerm);

        }


		public static bool IsVisible() => instance.isFocused;

		//static Debounce selChanged = new Debounce(SelChanged) { runOnUiThread = true };
		static City lastSelected;
		static void UpdateTargetIncoming(bool showHistory)
		{
			var sel = instance.attackerGrid.SelectedItem as Spot;
			lastSelected =sel;
			if (sel != null)
			{

				if(sel.incoming.Where(a => a.shareInfo).OrderBy(a=>a.arrival).ToArray().SyncList(instance.targetIncoming)) {
					instance.armyGrid.ResetAutoColumns();
					if(showHistory) {
						var tab = HitHistoryTab.instance;
						tab.SetFilter(sel);
						
					}
				}
			}
			else {
				instance.targetIncoming.Clear();
			}
		}

	
        private void defenderGrid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
		{
			if (!isOpen)
				return;
			if(instance.attackerGrid.SelectedItem as Spot != lastSelected) {
				AppS.QueueOnUIThread(() => UpdateTargetIncoming(true) );
		
			}

		}

		private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
		{
			instance.refresh.Go();
		}

		private void filterFromChanged(object sender,Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
		{
			refresh.Go();
		}
	}
       

}
