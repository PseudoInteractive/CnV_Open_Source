﻿using COTG.Game;
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
using System.Threading.Tasks;

namespace COTG.Views
{

    public sealed partial class NearDefenseTab : UserTab,INotifyPropertyChanged
    {
        public static NearDefenseTab instance;
        public static bool IsVisible() => instance.isVisible;
        public bool waitReturn { get; set; } = true;
		public bool sendViaWater { get; set; }

		public static ResetableCollection<City> defendants = new();

        public static bool includeOffense;
        public float filterTime=6;
        public int filterTSTotal=10000;
        public int filterTSHome;
		public bool portal { get; set; }
		public bool onlyHome { get; set; } = true;

		public float _filterTime { get => filterTime; set { filterTime = value; refresh.Go(); } }  // defenders outside of this window are not included
        public int _filterTSTotal { get => filterTSTotal; set { filterTSTotal = value; refresh.Go(); } }
        public int _filterTSHome { get => filterTSHome; set { filterTSHome = value; refresh.Go(); } } // need at this this many ts at home to be considered for def
        public DateTimeOffset arriveAt { get; set; } = AUtil.dateTimeZero;
        public static DumbCollection<Supporter> supporters = new DumbCollection<Supporter>();
        public static SupportByTroopType [] supportByTroopTypeEmpty = Array.Empty<SupportByTroopType>();
        public static int[] splitArray = { 1, 2, 3, 4, 5 };
        public static bool Include(TroopTypeCount tt) => includeOffense || tt.isDef;


        public static void GetSelected(List<int> rv)
        {
            var i = instance;
            if (!NearDefenseTab.IsVisible())
                return;
            
            foreach(var sel in  i.supportGrid.SelectedItems)
            {
                var s = sel as Supporter;
                Assert(s != null);
                rv.AddIfAbsent(s.cid);
            }
        }

        public async override Task VisibilityChanged(bool visible)
        {
            if (visible)
            {
				if (defendants.Count == 0)
				{
					var focus = Spot.GetFocus();
					if (focus.isCityOrCastle)
						defendants.Add(focus);
					else
						defendants.Add(City.GetBuild());
					defendants.NotifyReset();
				}

				var viaWater = sendViaWater && defendants.Any(d => d.isOnWater);

//				if (defendants != null && defendant.isCityOrCastle)
				{
					var portal = this.portal;

					var onlyHome = this.onlyHome;
					// Dispatch both and then wait for results in parallel
					var task0 = RestAPI.troopsOverview.Post();
					var task1 = RaidOverview.Send();
					await task0;
					await task1;
					List<Supporter> s = new List<Supporter>();
					//                supportGrid.ItemsSource = null;
					{
						using var _ = await City.cityGridLock.LockAsync();
					foreach (var city in City.gridCitySource)
					{
						Assert(city is City);
						if ((includeOffense ? city.tsHome : city.tsDefHome) < filterTSHome |
							 (includeOffense ? city.tsTotal : city.tsDefTotal) < filterTSTotal)
							continue;
						if (viaWater && !city.isOnWater)
							continue;

						var hours = 0.0f;
						var canTravelViaWater=city.isOnWater && defendants.Any(a=>a.isOnWater);


							int validCount = 0;
							if (!portal)
							{
								validCount = FindValidDefendants(viaWater, onlyHome, city, ref hours).Count;
								if (validCount == 0)
									continue;
							}
							else
							{
								validCount = defendants.Count;
							}

						// re-use if possible
						var supporter = supporters.Find((a) => a.city == city);
						if (supporter == null)
						{
							supporter = new Supporter() { city = city };
						}
						var troops = (onlyHome ? city.troopsHome : city.troopsTotal);
						s.Add(supporter);
						supporter.validTargets = validCount;
						if (viaWater)
						{
							var ttGalleys = troops.FirstOrDefault((tt) => tt.type == ttGalley);
							// handle Galleys
							if (ttGalleys.count > 0 )
							{
								var galleys = ttGalleys.count;
								var landTroops = troops.Where((tt) => IsLandRaider(tt.type)).Sum((t) => t.count);
								var requiredGalleys = (landTroops + 499) / 500;
								var sendGain = 1.0;
								if (galleys >= requiredGalleys)
								{
									galleys = requiredGalleys;
								}
								else
								{
									sendGain = galleys * 500.0 / landTroops;
								}
								supporter.tSend.Clear();
								supporter.tSend.Add(new TroopTypeCount(ttGalley, galleys));
								foreach (var tt in troops)
								{
									if (tt.type == ttStinger)
									{
										supporter.tSend.v.Add(tt);
									}
									else
									{
										if (!IsLandRaider(tt.type) || !Include(tt))
											continue;
										supporter.tSend.v.Add(new TroopTypeCount(tt.type, (int)(sendGain * tt.count))); // round down
									}
								}
							}
							else
							{
								supporter.tSend.v.Set(  troops.Where((tt) => tt.type == ttStinger) ); // take stingers
							}
						}
						else
						{
							supporter.tSend.Set( troops.Where(tt => (includeOffense || tt.isDef) && (canTravelViaWater||!tt.isNaval) ) ); // clone array
						}
						supporter.travel = hours + (defendants.Count - validCount);  // penality for targtes that we cannot make it to
					}
				}
                    if (portal)
                    {
                        if(onlyHome)
                            supporters.Set(s.OrderByDescending(a => a.tsHome));
                        else
                            supporters.Set(s.OrderByDescending(a => a.tsTotal));
                    }
                    else
                        supporters.Set(s.OrderBy(a => a.travel - a.validTargets ));
					
					defendants.NotifyReset();
                }


            }
            else
            {
                supporters.Clear();
				App.DispatchOnUIThreadSneaky( ()=>
				
                troopTypeGrid.ItemsSource=supportByTroopTypeEmpty
           );     //              supportGrid.ItemsSource = null;

            }
			await base.VisibilityChanged(visible);
        }

		private List<Spot> FindValidDefendants(bool viaWater, bool onlyHome, City city, ref float hours)
		{
			if (portal)
				return defendants.ToList<Spot>();

			var rv = new List<Spot>();
			foreach (var d in defendants)
			{

				if (city.ComputeTravelTime(d.cid, onlyHome, includeOffense, false, false, viaWater, filterTime, ref hours))
				{
					rv.Add(d);
				}
			}
			return rv;
		}

		public NearDefenseTab()
        {
            Assert(instance == null);
            instance = this;
            this.InitializeComponent();
      
        }

      

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void Coord_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var image = sender as FrameworkElement;
            
            var supporter = image.DataContext as Supporter;
            Spot.ProcessCoordClick(supporter.city.cid, false, App.keyModifiers,false);

        }

        private void Image_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var image = sender as FrameworkElement;
            var supporter = image.DataContext as Supporter;
            supporter.city.ShowContextMenu(supportGrid, e.GetPosition(supportGrid));
        }

        private void supportGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			if(isVisible)
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
                    if(Include(i))
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
            flyout.CopyXamlRoomFrom(text);
            AApp.AddItem(flyout, "Troops Home", (_, _) =>
            {
                var supporter = stt.supporter;
                supporter.tSend.v.SetOrAdd(stt.type, stt.supporter.city.troopsHome.Count(stt.type));
                supporter.NotifyChange();
            });
            AApp.AddItem(flyout, "Total Troops", (_, _) =>
            {
                var supporter = stt.supporter;
                supporter.tSend.v.SetOrAdd(stt.type, stt.supporter.city.troopsTotal.Count(stt.type));
                supporter.NotifyChange();
            });
            AApp.AddItem(flyout, "None", (_, _) =>
            {
                var supporter = stt.supporter;
                supporter.tSend.v.SetOrAdd(stt.type, 0);
                supporter.NotifyChange();
            });

            flyout.ShowAt(text, e.GetPosition(text));

        }

        private void TsSendRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var text = sender as FrameworkElement;
            var supporter = text.DataContext as Supporter;
            var flyout = new MenuFlyout();
            flyout.CopyXamlRoomFrom(text);
            AApp.AddItem(flyout, "Troops Home", (_, _) =>
            {
				supporter.tSend.Set( supporter.city.troopsHome);
                supporter.NotifyChange();
            });
            AApp.AddItem(flyout, "Total Troops", (_, _) =>
            {
				supporter.tSend.Set( supporter.city.troopsTotal);
                supporter.NotifyChange();
            });
            AApp.AddItem(flyout, "None", (_, _) =>
            {
                supporter.tSend.Clear();
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
            var departAt = AUtil.dateTimeZero;
            var _arriveAt = arriveAt;
            if(waitReturn && !supporter.city.troopsHome.IsSuperSetOf(supporter.tSend.v))
            {
				RaidOverview.SendMaybe();

				if (city.MightRaidsRepeat())
                {
                    await Raiding.ReturnFast(city.cid, false);
                }

                departAt = city.GetRaidReturnTime() + TimeSpan.FromSeconds(15);
                var canArriveAt = departAt+ TimeSpan.FromHours(supporter.travel );
                if (_arriveAt > JSClient.ServerTime() && _arriveAt < canArriveAt)
                {
					var result = await App.DispatchOnUIThreadTask(async () =>
					{
						var msg = new ContentDialog()
						{
							Title = "Home Too late to make arrival time",
							Content = "Would you like to schedule as soon as they return?",
							PrimaryButtonText = "Yes",
							CloseButtonText = "Cancel"
						};
						msg.CopyXamlRoomFrom(text);
						ElementSoundPlayer.Play(ElementSoundKind.Show);

						if (await msg.ShowAsync2() != ContentDialogResult.Primary)
						{
							return false;
						}
						_arriveAt = AUtil.dateTimeZero;
						return true;
					});
					if (result == false)
						return;

                }
            }
			var hours = 0.0f;
			var def = FindValidDefendants(sendViaWater && defendants.Any(d => d.isOnWater),  onlyHome, city, ref hours);
			foreach (var d in def)
			{
				using var ts = new DArrayRef<TroopTypeCount>(supporter.tSend.v.DividedBy(def.Count));
				await Post.SendRein(supporter.cid, d.cid, ts.v, departAt, _arriveAt, hours, supporter.split, text);
			}


        }
		private void gridPointerPress(object sender, PointerRoutedEventArgs e)
		{

			Spot.GridPressed(sender, e);

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
                arriveAt = dateTime;
                OnPropertyChanged(nameof(arriveAt));
            }

        }

       
        private void PropChanged(object sender, RoutedEventArgs e)
        {
            refresh.Go();
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
            Owner.CommandService.ExecuteDefaultCommand(Id, parameter);
        }
    }

}
