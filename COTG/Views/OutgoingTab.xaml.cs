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
			SetupDataGrid(attackerGrid);
//			spotGrids.Add(attackerGrid);

		//	attackerGrid.OnKey = Spot.OnKeyDown;
            //            historyGrid.ContextFlyout = cityMenuFlyout;

            //      var data = defenderGrid.GetDataView();
        }



		public static Spot selected => instance.attackerGrid.SelectedItem as Spot;
		//private void gridPointerPress(object sender, PointerRoutedEventArgs e)
  //      {
  //     //     (var hit, var column, var pointerPoint,_) = Spot.HitTest(sender, e);
  //          //if (hit != null)
  //          //    defenderGrid.ShowRowDetailsForItem(hit);

  //          Spot.GridPressed(sender, e);
  //      }
        //private void gridPointerMoved(object sender, PointerRoutedEventArgs e)
        //{
        //    Spot.ProcessPointerMoved(sender, e);
        //}
 


		NotifyCollection<City> attackerSource = new();



		public static void NotifyOutgoingUpdated()
        {
            if (OutgoingTab.IsVisible())
            {
                try
                {
					instance.attackerSource.Set( Spot.defendersO.Where( w => w.testContinentFilter 
																					&& (instance.includeInternal || !w.IsAllyOrNap() ) 
																					&& (!instance.onlyMine ||w.HasIncomingFrom(Player.myId))).OrderBy(w=>w.firstIncoming) );
					instance.attackerSource.ItemContentChanged();
				}
                catch (Exception e)
                {
                    LogEx(e);
                }
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

            if (visible)
                OutgoingOverview.OutgoingUpdateDebounce.Go();
            return base.VisibilityChanged(visible, longTerm: longTerm);

        }


		public static bool IsVisible() => instance.isFocused;

		static Debounce selChanged = new Debounce(SelChanged) { runOnUiThread = true };

		static Task SelChanged()
		{
			var sel = lastSelected;
			if (sel != null)
			{
				instance.armyGrid.ItemsSource = sel.incoming;
				if (SettingsPage.fetchFullHistory)
				{
					var tab = HitTab.instance;
					if (!instance.isFocused)
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

				selChanged.Go(/*throttled: true,runAgainIfStarted: false*/);

			}
		}

		private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
		{
			instance.refresh.Go();
		}
	}
       

}
