﻿using System.Runtime.CompilerServices;
//using Windows.UI.Core;

//using Windows.UI.Input;

namespace CnV.Views
{
	public sealed partial class HitHistoryTab:UserTab
	{
		NotifyList<City> cityFilter = new(new[] { City.invalid });
		internal void SetFilter(City c) {
			AppS.QueueOnUIThread(() => {
				cityFilter.AddIfAbsent(c);
				cityFilter.OnReset();
				cityFilterCombo.SelectedItem = c;
				if(!isFocused) {
					ShowOrAdd(true,onlyIfClosed: false);

				}

			});
		}
		ObservableCollection<BattleReport> reports = new(); 
		public override TabPage defaultPage => TabPage.secondaryTabs;
		//public ArmyArray history { get; set; } = ArmyArray.Empty;
		public void UpdateDataGrid() {

			var sel = instance.cityFilterCombo.SelectedItem as City;

			BattleReport[] items;
			if(sel == null || sel.IsInvalid())
				items= BattleReport.all.Where(a=> a.sourcePlayer.sharesInfo||a.targetPlayer.sharesInfo).ToArray();
			else {
				var cid = sel.cid;
				items = BattleReport.all.Where(s => s.targetCid == cid || s.sourceCid==cid).ToArray();
			}
			items.SyncList(reports);
			//if(IncomingTab.IsVisible()) {
			//	IncomingTab.NotifyIncomingUpdated();
			//}
			//if(OutgoingTab.IsVisible()) {
			//	OutgoingTab.NotifyOutgoingUpdated();
			//}

		}
		public static void CombatNotify() {
			if(!IsVisible())
				return;
			AppS.QueueOnUIThread(instance.UpdateDataGrid);
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
		public HitHistoryTab() {
			Assert(instance == null);
			instance = this;

			InitializeComponent();
			SetupDataGrid(historyGrid,false);
			//            historyGrid.ContextFlyout = cityMenuFlyout;

		}
		override public Task VisibilityChanged(bool visible,bool longTerm) {
			//   historyGrid.ItemsSource = Array.Empty<Army>();
			if(visible) {
				AppS.QueueOnUIThread(UpdateDataGrid);
			}
			return base.VisibilityChanged(visible,longTerm: longTerm);
		}






		private void Set<T>(ref T storage,T value,[CallerMemberName] string propertyName = null) {
			if(Equals(storage,value)) {
				return;
			}
			storage = value;
			OnPropertyChanged(propertyName);
		}


		public static bool IsVisible() => instance.isFocused;

		private void cityFilterCombo_SelectionChanged(object sender,Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e) {
			UpdateDataGrid();
		}
	}
}
