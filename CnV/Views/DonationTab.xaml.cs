using System.Collections.Generic;
using System.Threading.Tasks;

namespace CnV.Views;

using Game;

using Microsoft.UI.Xaml;

using Services;
using Syncfusion.UI.Xaml.DataGrid;
using Syncfusion.UI.Xaml.Grids;

public sealed partial class DonationTab : UserTab
{
	public static DonationTab instance;

	//public ObservableCollection<DonationOrder> donationOrders = new();
	//public static bool wantTempleDonations;

	public static float woodStoneRatio = -1;
	private static bool viaWater;
	//public string[] priorityNames = { "Do Not Send","NA", "Low", "Medium", "High"  };
	NotifyCollection<City> donationGridSource = new();

	public static bool ViaWater { get => viaWater; set {
		if(viaWater == value) return;
			viaWater=value;
			DonationTab.instance.refresh.Go();
		} }

	public DonationTab()
	{
		Assert(instance == null);
		instance = this;
		this.InitializeComponent();
	
		//	spotGrids.Add(donationGrid);
	//	CityUI.cityListChanged += CityListChanged;
	}

	private static void CityListChanged(IEnumerable<City> l)
	{

		if (DonationTab.IsVisible())
		{
			
				instance.donationGridSource.Set(l.Where((city) => (ViaWater?city.shipsHome > Settings.tradeSendReserveShips:
				city.cartsHome > Settings.tradeSendReserveCarts) )
					.OrderBy(a => a.cont).ThenByDescending(a => (!ViaWater ? a.cartsHome * 1000 : a.shipsHome*10_000).Min(a.sampleResources.sum)));

			
		}
	}

	public static bool IsVisible() => instance.isFocused;

	// List<BlessedCity> blessedGridSource = new List<BlessedCity>();
	public async override Task VisibilityChanged(bool visible, bool longTerm)
	{
		if (visible)
		{
			//DonationOrder.Refresh();
			// This just updates details for all cities
			//var details = await  CityOverview.Send().ConfigureAwait(false);
			//foreach (var detail in details)
			//{
			////	var city = City.GetOrAddCity(detail.id);
			////	city.points = (ushort) detail.score;
			////	city.cartsHome = detail.carts_home;
			////	city.carts = detail.carts_total;
			////	city.shipsHome = detail.ships_home;
			////	city.ships = detail.ships_total;
			////	city.wood = detail.wood;
			//////	city.woodStorage = detail.wood_storage;
			////	city.stone = detail.stone;
			//////	city.stoneStorage = detail.stone;
			////	city.iron = detail.iron;
			//////	city.ironStorage = detail.iron_storage;
			////	city.food = detail.food;
			////	//city.foodStorage = detail.food_storage;
			////	city.hasAcademy = detail.Academy == "Y";
			//////	city.sorcTower = detail.Sorc_tower == "Y";
			//}

			CityListChanged(City.gridCitySource);


			//CityList.NotifyChange(true);
			//AppS.QueueOnUIThread(() =>
			//	{
			//	//	blessedGrid.ItemsSource = DonationOrder.GetForCity(null);

			//		///	donationGrid.ItemsSource = City.gridCitySource;
			//	}
			//); // many items changed
		}
		else
		{
			// Not listening
			ClearBlessedCity();
			
		}

		await base.VisibilityChanged(visible, longTerm: longTerm);

	}

	public static void ClearBlessedCity()
	{
		AppS.QueueOnUIThread(() =>
		{
		//	instance.blessedGrid.ItemsSource = Array.Empty<DonationOrder>();
			//    donationGrid.ItemsSource = null;
	//		BlessedCity.senderCity = null;
		});
	}


	private void donationGrid_SelectionChanged(object? sender, GridSelectionChangedEventArgs e)
	{
		if (!isFocused)
			return;
		if(donationGrid.SelectedItems.Any())
		{
			var newSel = donationGrid.SelectedItems.First() as City;
		//	blessedGrid.ItemsSource = DonationOrder.GetForCity(newSel);
		
		
		}
	}

	//private void donationGrid_DetailsViewExpanding(object sender,GridDetailsViewExpandingEventArgs e) {
	//	    e.DetailsViewItemsSource.Clear();
	//	var c = e.Record as City;
	//	e.DetailsViewItemsSource.Add("Targets", c.donationTargets);
	//}
private void OnLoaded(object sender,RoutedEventArgs e)
{
	base.SetupDataGrid(donationGrid);
	base.SetupDataGrid(donationTargetsGrid);
	base.SetupDataGrid(marketTargetsGrid);
}
}


