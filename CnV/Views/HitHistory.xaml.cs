using System.Runtime.CompilerServices;
//using Windows.UI.Core;

//using Windows.UI.Input;
using System.Threading.Tasks;

namespace CnV.Views
{
	using Game;

	public sealed partial class HitHistoryTab : UserTab
    {
		public override  TabPage defaultPage => TabPage.secondaryTabs;
		//public ArmyArray history { get; set; } = ArmyArray.Empty;
        public void UpdateDataGrid()
        {
			var sel = IncomingTab.selected;
			if (sel == null)
				historyGrid.ItemsSource = BattleReport.all;
			else
			{
				var cid = sel.cid;
				historyGrid.ItemsSource = BattleReport.all.Where( s => s.targetCid == cid).ToArray() ;
			}
		}

        public static HitHistoryTab instance;
        public static xDataGrid HistoryGrid => instance.historyGrid;
        //        public static Army showingRowDetails;

        //public DataTemplate GetTsInfoDataTemplate()
        //{
        //    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
        //    Assert(rv != null);
        //    return rv;
        //}
        public HitHistoryTab()
        {
            Assert(instance == null);
            instance = this;

            InitializeComponent();
			SetupDataGrid(historyGrid,false );
			//            historyGrid.ContextFlyout = cityMenuFlyout;

		}
        override public Task VisibilityChanged(bool visible, bool longTerm)
		{
         //   historyGrid.ItemsSource = Array.Empty<Army>();
            if (visible)
            {
				UpdateDataGrid();
			}
			return base.VisibilityChanged(visible, longTerm: longTerm);
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

      
        public static bool IsVisible() => instance.isFocused;

    }
}
