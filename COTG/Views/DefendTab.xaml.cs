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
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace COTG.Views
{

    public sealed partial class DefendTab : UserTab,INotifyPropertyChanged
    {
        public static DefendTab instance;
        public static bool IsVisible() => instance.isVisible;
        public bool waitReturn { get; set; } = true;
        public static Spot defendant;
        public float filterTime=6;
        public int filterTSTotal=10000;
        public int filterTSHome;
        public float _filterTime { get => filterTime; set { filterTime = value; Refresh(); } }  // defenders outside of this window are not included
        public int _filterTSTotal { get => filterTSTotal; set { filterTSTotal = value; Refresh(); } }
        public int _filterTSHome { get => filterTSHome; set { filterTSHome = value; Refresh(); } } // need at this this many ts at home to be considered for def
        public DateTimeOffset sendAt { get; set; } = AUtil.dateTimeZero;
        public static DumbCollection<Supporter> supporters = new DumbCollection<Supporter>();
        public static SupportByTroopType [] supportByTroopTypeEmpty = Array.Empty<SupportByTroopType>();
        public static int[] splitArray = { 1, 2, 3, 4, 5 };


        public static void GetSelected(List<int> rv)
        {
            var i = instance;
            if (!DefendTab.IsVisible())
                return;
            
            foreach(var sel in  i.supportGrid.SelectedItems)
            {
                var s = sel as Supporter;
                Assert(s != null);
                rv.AddIfAbsent(s.cid);
            }
        }

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
                        Assert(city is City);
                        if (city.tsHome < filterTSHome | city.tsTotal < filterTSTotal)
                            continue;
                        if (!city.ComputeTravelTime(defendant.cid, out var hours, out var onDifferentContinent) || hours > filterTime)
                            continue;
                        // re-use if possible
                        var supporter = supporters.Find((a) => a.city == city);
                        if (supporter == null)
                        {
                            supporter = new Supporter() { city = city };
                        }
                        s.Add(supporter);
                        if (onDifferentContinent)
                        {
                            var ttGalleys = city.troopsHome.FirstOrDefault((tt) => tt.type == ttGalley);
                            // handle Galleys
                            if (ttGalleys != null)
                            {
                                var galleys = ttGalleys.count;
                                var landTroops = city.troopsHome.Where((tt) => IsLandRaider(tt.type)).Sum((t) => t.count);
                                var requiredGalleys = (landTroops + 499) / 500;
                                var sendGain = 1.0;
                                if(galleys >= requiredGalleys )
                                {
                                    galleys = requiredGalleys;
                                }
                                else
                                {
                                    sendGain = galleys * 500.0 / landTroops;
                                }
                                List<TroopTypeCount> tSend = new List<TroopTypeCount>();
                                tSend.Add(new TroopTypeCount(ttGalley, galleys));
                                foreach(var tt in city.troopsHome)
                                {
                                    if (tt.type == ttStinger)
                                    {
                                        tSend.Add(tt); 
                                    }
                                    else
                                    {
                                        if (!IsLandRaider(tt.type))
                                            continue;
                                        tSend.Add(new TroopTypeCount(tt.type, (int)(sendGain * tt.count))); // round down
                                    }
                                }
                                supporter.tSend = tSend.ToArray();
                            }
                            else
                            {
                                supporter.tSend = city.troopsHome.Where((tt) => tt.type == ttStinger).ToArray(); // take stingers
                            }
                        }
                        else
                        {
                            supporter.tSend = city.troopsHome.ToArray(); // clone array
                        }
                        supporter.travel = hours;
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
            Spot.ProcessCoordClick(supporter.city.cid, false, App.keyModifiers);

        }

        private void Image_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var image = sender as FrameworkElement;
            var supporter = image.DataContext as Supporter;
            supporter.city.ShowContextMenu(supportGrid, e.GetPosition(supportGrid));
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
            App.AddItem(flyout, "None", (_, _) =>
            {
                var supporter = stt.supporter;
                supporter.tSend = supporter.tSend.SetOrAdd(stt.type, 0);
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
            App.AddItem(flyout, "None", (_, _) =>
            {
                supporter.tSend = TroopTypeCount.empty;
                supporter.NotifyChange();
            });

            flyout.ShowAt(text, e.GetPosition(text));

        }

        private async void SendClick(object sender, RoutedEventArgs e)
        {
            var text = sender as FrameworkElement;
            var supporter = text.DataContext as Supporter;
            var city = supporter.city;
            if(city.commandSlots!=0 &&  supporter.split > city.freeCommandSlots)
            {
                Note.Show("To few command slots");
                return;
            }
            var at = sendAt;
            if(waitReturn)
            {
                if(city.AreRaidsRepeating())
                {
                    await Raiding.ReturnFast(city.cid, false);
                }
                var canReturnAt = city.GetRaidReturnTime() + TimeSpan.FromHours(supporter.travel + 1.0f / 256.0f);
                if (canReturnAt > at)
                    at = canReturnAt;

            }

            Post.SendRein(supporter.cid, defendant.cid, supporter.tSend, at,supporter.travel,supporter.split);
            

        }


        private void supportGrid_Sorting(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e)
        {
            var dg = supportGrid;
            var tag = e.Column.Tag?.ToString();
            //Use the Tag property to pass the bound column name for the sorting implementation
            Comparison<Supporter> comparer =null;
            switch (tag)
            {
                case nameof(Supporter.tsTotal): comparer = ( a,  b) => b.tsTotal.CompareTo(a.tsTotal);  break;
                case nameof(Supporter.tsHome): comparer = (a, b) => b.tsHome.CompareTo(a.tsHome); break;
                case nameof(Supporter.tsSend): comparer = (a, b) => b.tsSend.CompareTo(a.tsSend); break;
                case nameof(Supporter.travel): comparer = (b, a) => b.travel.CompareTo(a.travel); break;
                case nameof(Supporter.raidReturn): comparer = (b, a) => b.raidReturn.CompareTo(a.raidReturn); break;
            }

            if (comparer != null)
            {
                //Implement sort on the column "Range" using LINQ
                if (e.Column.SortDirection == null)
                {
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                    supporters.Sort(comparer);
                    supporters.NotifyReset();
                }
                else if(e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                    supporters.Sort((b, a) => comparer(a,b) ); // swap order of comparison
                    supporters.NotifyReset();
                }
                else
                {
                    e.Column.SortDirection = null;

                }
            }
            // add code to handle sorting by other columns as required

            // Remove sorting indicators from other columns
            foreach (var dgColumn in dg.Columns)
            {
                if (dgColumn.Tag!=null && dgColumn.Tag.ToString() != tag)
                {
                    dgColumn.SortDirection = null;
                }
            }
        }

        private async void SendAtTapped(object sender, PointerRoutedEventArgs e)
        {
            e.KeyModifiers.UpdateKeyModifiers();
            e.Handled = true;
            (var dateTime, var okay) = await DateTimePicker.ShowAsync("Send At");
            if (okay)
            {
                sendAt = dateTime;
                OnPropertyChanged(nameof(sendAt));
            }

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
                        Spot.ProcessCoordClick(i.cid, false, App.keyModifiers);
                        break;
                }
            }
            if (base.CanExecute(parameter))
                base.Execute(parameter);
        }
    }

}
