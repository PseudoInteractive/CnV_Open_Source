using COTG.Game;
using COTG.Helpers;
using COTG.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace COTG.Views
{

    public sealed partial class DonationTab : UserTab, INotifyPropertyChanged
    {
        public static DonationTab instance;
        public int reserveCarts=800;
        public float reserveCartsPCT=0.0625f;
        public int reserveShips = 10;
        public float reserveShipsPCT = 0.0f;
        public int reserveWood = 0;
        public int reserveStone = 0;
        public DonationTab()
        {
            this.InitializeComponent();
        }

        public static bool IsVisible() => instance.isVisible;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        List<BlessedCity> blessedGridSource = new List<BlessedCity>();
        public async override void VisibilityChanged(bool visible)
        {
            if(visible)
            {
                BlessedCity.Refresh();
                var details = await  CityOverview.Send();
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
                    city.academy = detail.Academy == "Y";
                    city.sorcTower = detail.Sorc_tower == "Y";
                }

                App.DispatchOnUIThread(()=>
                {
                    CityList.SelectedChange();
                    blessedGrid.ItemsSource = BlessedCity.GetForCity(null);
                }
                ); // many items changed
          //     donationGrid.ItemsSource = City.gridCitySource;
            }
            else
            {
                // Not listening
                donationGrid.ItemsSource = null;
                BlessedCity.senderCity = null;
            }
            base.VisibilityChanged(visible);

        }

        public override void XamlTreeChanged(TabPage newPage)
        {
            base.XamlTreeChanged(newPage);
        }

        public override void Refresh()
        {
            base.Refresh();
        }

        private void gridPointerPress(object sender, PointerRoutedEventArgs e)
        {
            Spot.ProcessPointerPress(sender, e);

        }

        private void gridPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Spot.ProcessPointerExited();

        }

        private void donationGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            
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
                                var cartReserve = (inst.reserveCartsPCT * sender.carts).RoundToInt()
                                        .Max(inst.reserveCarts);
                                var carts = sender.cartsHome - cartReserve;
                                if (carts <= 0)
                                {
                                    Note.Show("Not enough carts");
                                    return;
                                }
                                var wood = 0.Max(sender.wood - inst.reserveWood);
                                var stone = 0.Max(sender.stone - inst.reserveStone);
                                if (wood + stone > carts * 1000)
                                {
                                    var desiredC = (wood + stone) / 1000 + 1;
                                    var ratio = (float)carts / desiredC;
                                    wood = (int)(wood * ratio);
                                    stone = (int)(stone * ratio);
                                }
                                else if (wood + stone <= 0)
                                {
                                    Note.Show("Not enough res");
                                    return;

                                }

                                i.SendDonation(wood, stone);
                                DonationTab.instance.blessedGrid.ItemsSource = null;
                                BlessedCity.senderCity = null;
                            }
                        }
                        break;
                      case nameof(i.xy):
                            JSClient.ShowCity(i.cid, false);
                            break;
            //            case nameof(Dungeon.plan):
            //                Raiding.SendRaids(i);
            //                break;

            //        }
                }
            }

            if (base.CanExecute(parameter))
                base.Execute(parameter);

        }
    }

}
