using System.Runtime.CompilerServices;
//using Windows.UI.Core;

//using Windows.UI.Input;
using System.Threading.Tasks;

namespace CnV.Views
{
	using Game;

	using Syncfusion.UI.Xaml.DataGrid;

	public sealed partial class NPCHistory : UserTab
    {
		public ObservableCollection<BossReport> items = new();
		

		public override  TabPage defaultPage => TabPage.secondaryTabs;
		//public ArmyArray history { get; set; } = ArmyArray.Empty;
		public void UpdateDataGrid()
		{
			var rs = (Alliance.CityFilter)filterCombo.SelectedIndex switch
			{
				Alliance.CityFilter.me => BossReport.reports.Where(a => a.playerId == Player.myId).ToArray(),
				Alliance.CityFilter.subs => BossReport.reports.Where(a => Player.IsSubOrMe(a.playerId)).ToArray(),
				Alliance.CityFilter.allied => BossReport.reports.Where(a => Alliance.PidToAlliance(a.playerId).AsAlliance().isAlliedWithPlayer ).ToArray(),
				Alliance.CityFilter.alliance => BossReport.reports.Where(a => Alliance.PidToAlliance(a.playerId) == Alliance.myId).ToArray(),
				_ => BossReport.reports.ToArray() // todo: filter
			};

			if( rs.SyncList( items )) {
				grid.ResetAutoColumns();

			}
	
			
		}
		public static void BossKillNotify()
		{
			if(!IsVisible())
				return;
			AppS.QueueOnUIThread( instance.UpdateDataGrid );
		}
        public static NPCHistory instance;
      
        //        public static Army showingRowDetails;

        //public DataTemplate GetTsInfoDataTemplate()
        //{
        //    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
        //    Assert(rv != null);
        //    return rv;
        //}
        public NPCHistory()
        {
            Assert(instance == null);
            instance = this;

            InitializeComponent();
			SetupDataGrid(grid,false );
			//            historyGrid.ContextFlyout = cityMenuFlyout;

		}
        override public Task VisibilityChanged(bool visible, bool longTerm)
		{
         //   historyGrid.ItemsSource = Array.Empty<Army>();
            if (visible)
            {
				AppS.QueueOnUIThread(UpdateDataGrid);
			}
			return base.VisibilityChanged(visible, longTerm: longTerm);
        }




      
        public static bool IsVisible() => instance.isFocused;

		private void filterFromChanged(object sender,Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
		{
			UpdateDataGrid();
		}
	}
}
