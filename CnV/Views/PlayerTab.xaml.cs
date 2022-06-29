//using Windows.UI.Core;
using Microsoft.UI.Xaml;
//using Windows.UI.Input;

namespace CnV.Views;

using CnV;


public sealed partial class PlayerTab:UserTab
{
	private const string workStr = "Refreshing build states..";
	public static PlayerTab? instance;

	NotifyList<Player> playerList = new();

	//        public static City showingRowDetails;

	//{
	//    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
	//    Assert(rv != null);
	//    return rv;
	//}
	//  static MenuFlyout cityMenuFlyout;
	public PlayerTab()
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


		public override async Task Closed()
		{ 
			await base.Closed();
			instance = null;
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

			playerList.Set(World.activePlayers);
			foreach(var a in World.activePlayers)
			{
				a.OnPropertyChanged();
			}
			playerGrid.ResetAutoColumns();
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


	public static bool IsVisible() =>  instance is not null && instance.isFocused;


	private void OnLoaded(object sender,RoutedEventArgs e)
	{
		base.SetupDataGrid(playerGrid);
		base.SetupDataGrid(citiesGrid);
	}
}
