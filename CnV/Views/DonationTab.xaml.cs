using System.Collections.Generic;
using System.Threading.Tasks;

namespace CnV.Views
{
	using Game;
	using Services;
	using Syncfusion.UI.Xaml.DataGrid;
	using Syncfusion.UI.Xaml.Grids;

	public sealed partial class DonationTab : UserTab
	{
		public static DonationTab instance;
		public static int reserveCarts = 800;
		public static float reserveCartsPCT = 0.0625f;
		public static int reserveShips = 10;
		public static float reserveShipsPCT = 0.0f;
		public static float woodStoneRatio = -1;
		public static int reserveWood = 0;
		public static int reserveStone = 0;
		public static bool viaWater;
		public string[] priorityNames = { "NA", "Low", "Medium", "High", "Do Not Send" };
		NotifyCollection<City> donationGridSource = new();

		public DonationTab()
		{
			Assert(instance == null);
			instance = this;
			this.InitializeComponent();
		
			//	spotGrids.Add(donationGrid);
			CityUI.cityListChanged += CityListChanged;
		}

		private static void CityListChanged(IEnumerable<City> l)
		{
			var reserveCartsFilter = DonationTab.reserveCarts;
			if (DonationTab.IsVisible())
			{
				instance.donationGridSource.Set( l.Where((city) => city.cartsHome >= reserveCartsFilter)
					.OrderBy(a => a.cont).ThenByDescending(a => (a.cartsHome * 1000).Min(a.stone + a.wood) ) );
			}
		}

		public static bool IsVisible() => instance.isFocused;

		// List<BlessedCity> blessedGridSource = new List<BlessedCity>();
		public async override Task VisibilityChanged(bool visible, bool longTerm)
		{
			if (visible)
			{
				BlessedCity.Refresh();
				// This just updates details for all cities
				var details = await  CityOverview.Send().ConfigureAwait(false);
				foreach (var detail in details)
				{
					var city = City.GetOrAddCity(detail.id);
					city.points = (ushort) detail.score;
					city.cartsHome = detail.carts_home;
					city.carts = detail.carts_total;
					city.shipsHome = detail.ships_home;
					city.ships = detail.ships_total;
					city.wood = detail.wood;
					city.woodStorage = detail.wood_storage;
					city.stone = detail.stone;
					city.stoneStorage = detail.stone;
					city.iron = detail.iron;
					city.ironStorage = detail.iron_storage;
					city.food = detail.food;
					city.foodStorage = detail.food_storage;
					city.hasAcademy = detail.Academy == "Y";
					city.sorcTower = detail.Sorc_tower == "Y";
				}

				CityListChanged(City.gridCitySource);


				CityList.NotifyChange(true);
				AppS.DispatchOnUIThread(() =>
					{
						blessedGrid.ItemsSource = BlessedCity.GetForCity(null);

						///	donationGrid.ItemsSource = City.gridCitySource;
					}
				); // many items changed
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
				instance.blessedGrid.ItemsSource = Array.Empty<BlessedCity>();
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
				blessedGrid.ItemsSource = BlessedCity.GetForCity(newSel);
			
			
			}
		}



		}
	}

