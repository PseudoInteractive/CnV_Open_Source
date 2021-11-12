using COTG.Game;
using COTG.Helpers;
using COTG.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using static COTG.Debug;

namespace COTG.Views
{

    public sealed partial class DonationTab : UserTab
	{
        public static DonationTab instance;
        public static int reserveCarts=800;
        public static float reserveCartsPCT =0.0625f;
        public static int reserveShips = 10;
        public static float reserveShipsPCT = 0.0f;
        public static float woodStoneRatio = -1;
        public static int reserveWood = 0;
        public static int reserveStone = 0;
		public static bool proportionateToWhatsNeeded = true;

		public string[] priorityNames = { "NA","Low","Medium","High","Do Not Send" };
		NotifyCollection<City> donationGridSource = new();

		public DonationTab()
        {
            Assert(instance == null);
            instance = this;
            this.InitializeComponent();
			SetupDataGrid(donationGrid);
			//	spotGrids.Add(donationGrid);
			City.cityListChanged += CityListChanged;
		}

		private static void CityListChanged(IEnumerable<City> l)
		{
			var reserveCartsFilter = DonationTab.reserveCarts;
			if(DonationTab.IsVisible())
			{
				instance.donationGridSource.Set( l.Where((city) => city.cartsHome >= reserveCartsFilter).OrderBy(a=>a.cont).ThenByDescending(a => a.cartsHome) );
			}
		}
		
		public static bool IsVisible() => instance.isFocused;
      
       // List<BlessedCity> blessedGridSource = new List<BlessedCity>();
        public async override Task VisibilityChanged(bool visible, bool longTerm)
		{
            if(visible)
            {
                BlessedCity.Refresh();
				// This just updates details for all cities
                var details = await  CityOverview.Send().ConfigureAwait(false);
                foreach(var detail in details)
                {
                    var city = City.GetOrAddCity(detail.id);
                    city.points = (ushort)detail.score;
                    city.cartsHome = detail.carts_home;
                    city.carts = detail.carts_total;
                    city.shipsHome = detail.ships_home;
                    city.ships =detail.ships_total;
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
				App.DispatchOnUIThread(()=>
                {
                    blessedGrid.ItemsSource = BlessedCity.GetForCity(null);

				///	donationGrid.ItemsSource = City.gridCitySource;
				}
				); // many items changed
            }
            else
            {
                // Not listening
                App.DispatchOnUIThreadLow(() =>
                {
                    blessedGrid.ItemsSource = null;
                //    donationGrid.ItemsSource = null;
                });
                BlessedCity.senderCity = null;
            }
           await base.VisibilityChanged(visible, longTerm: longTerm);

        }

        
   

     

        private void donationGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
			if (!isOpen)
				return;

			var it = e.AddedItems.FirstOrDefault();
            var newSel = it as City;
            blessedGrid.ItemsSource = BlessedCity.GetForCity(newSel);
        }

		
	}

public class BlessedTapCommand : DataGridCommand
{
    public BlessedTapCommand()
    {
        this.Id = CommandId.CellTap;

    }

    public override bool CanExecute(object parameter)
    {
        var context = parameter as DataGridCellInfo;
        // put your custom logic here
        //    Log("CanExecute");
        return true;
    }

    public override void Execute(object parameter)
    {
        var context = parameter as DataGridCellInfo;
        var i = context.Item as BlessedCity;
        Debug.Assert(i != null);
        var c = context.Column.Header?.ToString() ?? string.Empty;
        if (i != null)
        {
            switch (c)
            {
                case nameof(i.virtue):
                    {
                        var sender = BlessedCity.senderCity;

                        if (sender != null)
                        {
	                            var inst = DonationTab.instance;
                                var wood = (sender.wood - DonationTab.reserveWood.Max(1000)).Max0();
                                var stone = (sender.stone - DonationTab.reserveStone.Max(1000)).Max0();
                                if (wood + stone <= 0)
                                {
                                    Note.Show("Not enough res");
                                    return;

                                }
                                var useShips = sender.cid.CidToContinent() != i.cid.CidToContinent();
                                if (useShips)
                                {

                                    var shipReserve = (DonationTab.reserveShipsPCT * sender.ships).RoundToInt()
                                            .Max(DonationTab.reserveShips);
                                    var ships = sender.shipsHome - shipReserve - 1;
                                    if (ships <= 0)
                                    {
                                        Note.Show("Not enough ships");
                                        return;
                                    }
                                    if (wood + stone > ships * 10000)
                                    {
                                        var desiredC = (wood + stone) / 10000 + 1;
                                        var ratio = (float)ships / desiredC;
                                        wood = (int)(wood * ratio);
                                        stone = (int)(stone * ratio);
                                    }
                                    sender.shipsHome -= (ushort)((wood + stone + 9999) / 10000);

                                }
                                else
                                {
                                    var cartReserve = (DonationTab.reserveCartsPCT * sender.carts).RoundToInt()
                                            .Max(DonationTab.reserveCarts);
                                    var carts = sender.cartsHome - cartReserve - 1;
                                    if (carts <= 0)
                                    {
                                        Note.Show("Not enough carts");
                                        return;
                                    }
                                    if (wood + stone > carts * 1000)
                                    {
                                        var desiredC = (wood + stone) / 1000 + 1;
                                        var ratio = (float)carts / desiredC;
                                        
                                        var maxWood = wood;
                                        var maxStone = stone;
                                        wood = (int)(wood * ratio);
                                        stone = (int)(stone * ratio);
                                        float denom = (wood + stone);
                                        var desRatio = DonationTab.proportionateToWhatsNeeded && (i.wood > 0 || i.stone > 0) ?
												   (float)i.wood / (i.wood + i.stone).Max(1) :
												   DonationTab.woodStoneRatio;
                                        if (desRatio >= 0)
                                        {
                                            while (wood > 1000 && stone < maxStone -1000 &&  (wood-1000)/ denom > desRatio)
                                            {
                                                wood -= 1000;
                                                stone += 1000;
                                            }
                                            while (stone > 1000 && wood < maxWood - 1000 && (wood+1000) / denom < desRatio)
                                            {
                                                wood += 1000;
                                                stone -= 1000;
                                            }
                                        }
                                    }
                                    sender.cartsHome -= (ushort)((wood + stone + 999) / 1000);
                                }
                                sender.wood -= wood;
                                    sender.stone -= stone;
                                    BlessedCity.SendDonation(sender.cid,i.cid,wood, stone, useShips);
                                    DonationTab.instance.blessedGrid.ItemsSource = null;
								i.wood -= wood;
								i.stone -= stone;
                                    BlessedCity.senderCity = null;
                              //      var tempSource = DonationTab.instance.donationGrid.ItemsSource;
                                    //   DonationTab.instance.donationGrid.ItemsSource = tempSource;
                               //     DonationTab.instance.donationGrid.ItemsSource = null;
                               //     DonationTab.instance.donationGrid.ItemsSource = tempSource; // Force a refresh -  We set null in between, (might be needed)
                                

                        }
                    }
                    break;
                case nameof(i.xy):
                    Spot.ProcessCoordClick(i.cid, false, App.keyModifiers);
                    break;
                    //            case nameof(Dungeon.plan):
                    //                Raiding.SendRaids(i);
                    //                break;

                    //        }
            }
        }

            Owner.CommandService.ExecuteDefaultCommand(Id, parameter);

        }
}
}
