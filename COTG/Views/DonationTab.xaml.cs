using COTG.Game;
using COTG.Helpers;
using COTG.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

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
        public DonationTab()
        {
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public async override void VisibilityChanged(bool visible)
        {
            if(visible)
            {

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


               donationGrid.ItemsSource = MainPage.instance.gridCitySource;
            }
            else
            {
                donationGrid.ItemsSource = null;
            }
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
    }
}
