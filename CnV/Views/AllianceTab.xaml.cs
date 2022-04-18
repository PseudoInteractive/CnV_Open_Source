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
}
