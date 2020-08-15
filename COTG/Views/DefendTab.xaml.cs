using COTG.Game;
using System;
using System.Collections.Generic;
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
using static COTG.Game.Enum;
using static COTG.Debug;
using COTG.Helpers;
using System.ComponentModel;
using COTG.Services;

namespace COTG.Views
{

    public sealed partial class DefendTab : UserTab,INotifyPropertyChanged
    {
        public static DefendTab instance;
        public static bool IsVisible() => instance.isVisible;
        public static Spot defendant;
        public static float filterTime=8;  // defenders outside of this window are not included
        public static int filterTSTotal=10000;
        public static int filterTSHome; // need at this this many ts at home to be considered for def

        public static DumbCollection<Supporter> supporters = new DumbCollection<Supporter>();
        public static SupportByTroopType [] supportByTroopTypeEmpty = Array.Empty<SupportByTroopType>();
        public static int[] splitArray = { 1, 2, 3, 4, 5 };

        public async override void VisibilityChanged(bool visible)
        {
            if (visible)
            {
                if (defendant == null )
                    defendant = Spot.GetFocus();
                if (defendant != null && defendant.isCityOrCastle)
                {
                    // Dispatch both and then wait for results in parallel
                    var task0 = RestAPI.troopsOverview.Post();
                    var task1 = RaidOverview.Send();
                    await task0;
                    await task1;
                    List<Supporter> s = new List<Supporter>();
                    //                supportGrid.ItemsSource = null;
                    foreach (var city in City.allCities.Values)
                    {
                        if (city.tsHome < filterTSHome | city.tsTotal < filterTSTotal)
                            continue;
                        if (!city.ComputeTravelTime(defendant.cid, out var hours) || hours > filterTime)
                            continue;
                        // re-use if possible
                        var supporter = supporters.Find((a) => a.city == city);
                        if (supporter == null)
                        {
                            supporter = new Supporter() { city = city };
                        }
                        s.Add(supporter);
                        supporter.tSend = city.troopsHome.ToArray(); // clone array
                        supporter.travel = hours;
                        supporter.arrival = supporter.eta;
                    }
                    supporters.Set(s.OrderBy(a=>a.travel));
                }


            }
            else
            {
                supporters.Clear();
                troopTypeGrid.ItemsSource=supportByTroopTypeEmpty;
                //              supportGrid.ItemsSource = null;

            }
        }

        public DefendTab()
        {
            Assert(instance == null);
            instance = this;
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Coord_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var image = sender as FrameworkElement;
            var supporter = image.DataContext as Supporter;
            Spot.ProcessCoordClick(supporter.city.cid, false);

        }

        private void Image_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var image = sender as FrameworkElement;
            var supporter = image.DataContext as Supporter;
            supporter.city.ShowContextMenu(image, e.GetPosition(image));
        }

        private void supportGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshSupportByType();
        }
        public void RefreshSupportByType()
        {
            Log("Refresh");
            var sel = supportGrid.SelectedItem;
            if( sel is Supporter support )
            {
                var stt = new List<SupportByTroopType>();
                foreach (var i in support.city.troopsTotal)
                {
                    stt.Add(new SupportByTroopType() { type = i.type, supporter = support });
                }
                troopTypeGrid.ItemsSource = stt;
            }
            else
            {
                troopTypeGrid.ItemsSource = supportByTroopTypeEmpty; 
            }

        }


        private void TTSendRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var text = sender as FrameworkElement;
            var stt = text.DataContext as SupportByTroopType;
            var flyout = new MenuFlyout();
            App.AddItem(flyout, "Troops Home", (_, _) =>
            {
                var supporter = stt.supporter;
                supporter.tSend = supporter.tSend.SetOrAdd(stt.type, stt.supporter.city.troopsHome.Count(stt.type));
                supporter.NotifyChange();
            });
            App.AddItem(flyout, "Total Troops", (_, _) =>
            {
                var supporter = stt.supporter;
                supporter.tSend = supporter.tSend.SetOrAdd(stt.type, stt.supporter.city.troopsTotal.Count(stt.type));
                supporter.NotifyChange();
            });

            flyout.ShowAt(text, e.GetPosition(text));

        }

        private void TsSendRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var text = sender as FrameworkElement;
            var supporter = text.DataContext as Supporter;
            var flyout = new MenuFlyout();
            App.AddItem(flyout, "Troops Home", (_, _) =>
            {
               
                supporter.tSend = supporter.city.troopsHome.ToArray();
                supporter.NotifyChange();
            });
            App.AddItem(flyout, "Total Troops", (_, _) =>
            {
                supporter.tSend = supporter.city.troopsTotal.ToArray();
                supporter.NotifyChange();
            });

            flyout.ShowAt(text, e.GetPosition(text));

        }

        private void SendClick(object sender, RoutedEventArgs e)
        {
            var text = sender as FrameworkElement;
            var supporter = text.DataContext as Supporter;

            Post.SendRein(supporter.cid, defendant.cid, supporter.tSend, supporter.arrival,supporter.travel,supporter.split);
            
            Note.Show("Sent Reinforcements");

        }
    }

    public class SupporterTapCommand : DataGridCommand
    {
        public SupporterTapCommand()
        {
            this.Id = CommandId.CellTap;
        }

        public override bool CanExecute(object parameter)
        {
            var context = parameter as DataGridCellInfo;
            return true;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as DataGridCellInfo;
            var i = context.Item as Supporter;
            Debug.Assert(i != null);
            var c = context.Column.Header?.ToString() ?? string.Empty;
            if (i != null)
            {
                switch (c)
                {

                    case nameof(i.xy):
                        Spot.ProcessCoordClick(i.cid, false);
                        break;
                }
            }
            if (base.CanExecute(parameter))
                base.Execute(parameter);
        }
    }

}
