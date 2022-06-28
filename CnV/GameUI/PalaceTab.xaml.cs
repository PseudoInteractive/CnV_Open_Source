//using Windows.UI.Core;
using Microsoft.UI.Xaml;
//using Windows.UI.Input;

namespace CnV;

using CnV;


public sealed partial class PalaceTab:UserTab
{
	public static PalaceTab instance;


	//        public static City showingRowDetails;

	//{
	//    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
	//    Assert(rv != null);
	//    return rv;
	//}
	//  static MenuFlyout cityMenuFlyout;
	public PalaceTab()
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

			AppS.QueueOnUIThread( () => palaceGrid.ItemsSource =(City.allianceCities.Where(a=>a.isTemple||a.isBlessed).ToArray() ));
			

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


	public static bool IsVisible() => instance is not null && instance.isFocused;

	private void OnLoaded(object sender,RoutedEventArgs e)
	{
		base.SetupDataGrid(palaceGrid);
		palaceGrid.AllowFiltering=true;
	//	palaceGrid.AllowGrouping=true;
	}

	private void palaceGrid_QueryRowHeight(object sender,Syncfusion.UI.Xaml.DataGrid.QueryRowHeightEventArgs e) {
		if (this.palaceGrid.ColumnSizer.GetAutoRowHeight(e.RowIndex, new(), out var autoHeight))
    {
        if (autoHeight > 32)
        {
            e.Height = autoHeight;
            e.Handled = true;
        }
    }
	}
}
