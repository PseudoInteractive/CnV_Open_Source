//using Windows.UI.Core;
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
		bool includeInternal;
		int filterFrom = filterFromAlliance;
		const int filterFromMe = 0;
		const int filterFromSubs = 1;
		const int filterFromAlliance = 2;
		const int filterFromAll = 3;


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


		NotifyCollection<City> attackerSource = new();

		

		public static void NotifyOutgoingUpdated()
        {
			if(OutgoingTab.IsVisible())
			{
				AppS.QueueOnUIThread(() =>
				{ 
				try
				{
					instance.attackerSource.Set((instance.filterFrom switch
					{
						filterFromMe => City.myCities,
						filterFromSubs => City.subCities,
						filterFromAlliance => City.allianceCities,
						_ => City.allCities,

					}).SelectMany(c => c.outgoing.Where(a => a.isOutgoingAttack ).Select(a => a.targetCity).Distinct()).
					Where(w =>
																					  w.testContinentFilter
																					&& (instance.includeInternal || !w.isAllyOrNap)).OrderBy(w => w.firstIncoming)); ; ;
					instance.attackerSource.ItemContentChanged();
					selChanged.Go();
				}
				catch(Exception e)
				{
					LogEx(e);
				}
			}
				);
            }
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

		static Debounce selChanged = new Debounce(SelChanged) { runOnUiThread = true };

		static Task SelChanged()
		{
			var sel = instance.attackerGrid.SelectedItem as Spot;
			if (sel != null)
			{
				instance.armyGrid.ItemsSource = sel.incoming;
				if (Settings.fetchFullHistory)
				{
					var tab = HitHistoryTab.instance;
					tab.SetFilter(sel);
					if (!tab.isFocused)
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
		

        private void defenderGrid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
		{
			if (!isOpen)
				return;
			selChanged.Go();
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
