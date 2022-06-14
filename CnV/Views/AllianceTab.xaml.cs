//using Windows.UI.Core;
using Microsoft.UI.Xaml;
//using Windows.UI.Input;

namespace CnV.Views;

using CnV;


public sealed partial class AllianceTab:UserTab
{
	public static AllianceTab instance;

	NotifyList<Alliance> allianceList = new();

	//        public static City showingRowDetails;

	//{
	//    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
	//    Assert(rv != null);
	//    return rv;
	//}
	//  static MenuFlyout cityMenuFlyout;
	public AllianceTab()
	{
		//            var a = Telerik.UI.Xaml.Controls.Grid.Primitives.For
		Assert(instance == null);
		instance = this;
		InitializeComponent();


		//			cityGrid.SelectionChanged += SpotSelectionChanged;
		//cityGrid.OnKey = Spot.OnKeyDown;

		//cityGrid.CurrentItemChanged += CityGrid_CurrentItemChanged;


		//			cityGrid.ProcessTooltips();
		//			spotGrids.Add(cityGrid);

	}





	override public async Task VisibilityChanged(bool visible,bool longTerm)
	{
		//   Log("Vis change" + visible);


		if(visible)
		{

			//if(CnVServer.ppdtInitialized)
			//{
			//	//	await Raiding.UpdateTS(true, false);
			//	//	await RaidOverview.Send();
			//	if(City.build != 0)
			//		await GetCity.Post(City.build);


			//	//	City.gridCitySource.NotifyReset();

			//}

			//Task.Run(GetBuildInfo).ContinueWith(async (_) =>
			//  {
			//   foreach(var c in City.myCities)
			//   {
			//	   if(c.testContinentAndTagFilter)
			//		   c.OnPropertyChanged();
			//   }
			//   City.gridCitySource.NotifyReset(true,true);
			//  });
			//  if (cityGrid.ItemsSource == App.emptyCityList )
			//     cityGrid.ItemsSource = City.gridCitySource;

			allianceList.Set(Alliance.all);
			foreach(var a in Alliance.all)
			{
				a.OnPropertyChanged();
			}
		}
		else
		{
			//        cityGrid.ItemsSource = null;
		}
		await base.VisibilityChanged(visible,longTerm: longTerm);
		//	if(visible)
		//	{
		//		AppS.DispatchOnUIThreadLow(() => Spot.SyncUISelection(true, City.GetBuild() ));
		//	}
	}


	public static bool IsVisible() => instance.isFocused;
	public static void AlliancesChanged()
	{
		if(IsVisible())
		{
			instance.refresh.Go();
		}
	}

	private void OnLoaded(object sender,RoutedEventArgs e)
	{
		base.SetupDataGrid(allianceGrid);
		base.SetupDataGrid(playersGrid);
	}

	private void CellBeginEdit(object sender,Syncfusion.UI.Xaml.DataGrid.CurrentCellBeginEditEventArgs e) {
		try {

			var grid = sender as xDataGrid;
			var row = e.RowColumnIndex.RowIndex;
			var r = Syncfusion.UI.Xaml.DataGrid.GridIndexResolver.ResolveToRecordIndex(grid,row);

			var rec = r>= 0 ? grid.View.Records[r] : null;
			var alliance = rec.Data as Alliance;

			if(e.Column.MappingName == nameof(Alliance.diplomacyWithPlayer)) {
				//			var v = e.Column.
				var strTitle = "Diplomacy";
				if(alliance.isMine) {
					e.Cancel=true;
					AppS.MessageBox("You are always an ally with youself",title: strTitle);

				}
				else {
					if(!(Player.me.allianceTitle>=AllianceTitle.secondLeader || AppS.isTest)) {
						e.Cancel=true;
						AppS.MessageBox("Must be second leader or above to set diplomacy",title: strTitle);

					}
					else {
						Note.Show("Not yet implemented:  Diplomacy persistance");

					}
				}
			}
		}
		catch(Exception _ex) {
			LogEx(_ex);
			e.Cancel=true;
		}
	}
	private void CellEndEdit(object sender,Syncfusion.UI.Xaml.DataGrid.CurrentCellEndEditEventArgs e) {
		try {

				//Note.Show($"EditChanged" );

		}
		catch(Exception _ex) {
			LogEx(_ex);
		}
		}

	private  async void CellDropdownChanged(object sender,Syncfusion.UI.Xaml.Grids.CurrentCellDropDownSelectionChangedEventArgs e) {
		try {

			var sel = e.SelectedItem as DiplomacyComboBoxSource;
			//Note.Show("DropdownChanged" + sel.n);
			var grid = sender as xDataGrid;
			var r = Syncfusion.UI.Xaml.DataGrid.GridIndexResolver.ResolveToRecordIndex(grid,e.RowColumnIndex.RowIndex);
			var rec = grid.View.Records[r];
			if(sel is not null )
			{
				var otherAlliance = rec.Data as Alliance;
				var my = Alliance.my;
				var newDiplomacy = sel.d;
				if( await AppS.DoYesNoBox("Set Diplomacy",$"Set status with {Alliance.IdToName(otherAlliance.id)} to {newDiplomacy.EnumTitle()}?") == 1)
					new CnVEventAlliance(my.id,default,default,otherAlliance.id,newDiplomacy).EnqueueAsap();
			
			}
		}
		catch(Exception _ex) {
			LogEx(_ex);
		}
	}
}
