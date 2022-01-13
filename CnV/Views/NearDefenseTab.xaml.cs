using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using static CnV.Troops;
using System.Threading.Tasks;
using static CnV.TroopTypeCountHelper;
using CommunityToolkit.WinUI.UI.Controls;

namespace CnV.Views
{
	using Game;
	using Services;

	public sealed partial class NearDefenseTab : UserTab
	{

        public static NearDefenseTab instance;
        public static bool IsVisible() => instance.isFocused;
        public bool waitReturn { get; set; }
		public bool sendViaWater { get; set; }

		public static NotifyCollection<City> defendants = new();
		public static NotifyCollection<Supporter> supporters = new();

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

        public async override Task VisibilityChanged(bool visible, bool longTerm)
		{
            if (visible)
            {
	            IncomingOverview.ProcessTask();
				if (defendants.Count == 0)
				{
					var focus = Spot.GetFocus();
					if (focus.isCityOrCastle)
						defendants.Add(focus,true);
					else
						defendants.Add(City.GetBuild(),true);
				}

				var viaWater = sendViaWater;// && defendants.Any(d => d.isOnWater);

//				if (defendants != null && defendant.isCityOrCastle)
				{
					var portal = this.portal;

					var onlyHome = this.onlyHome;
					// Dispatch both and then wait for results in parallel
					var task0 = RestAPI.troopsOverview.Post().ConfigureAwait(false);
					var task1 = RaidOverview.Send().ConfigureAwait(false);
					var result0 = await task0;
					var result1 = await task1;
					// should we abort on failure?
					var s = new List<Supporter>();
					//                supportGrid.ItemsSource = null;
					{
					foreach (var city in City.gridCitySource)
					{
						Assert(city is City);
						if ((includeOffense ? city.tsHome : city.tsDefCityHome) < filterTSHome |
							 (includeOffense ? city.tsTotal : city.tsDefCityTotal) < filterTSTotal)
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
						var supporter = supporters.c.FirstOrDefault((a) => a.city == city);
						if (supporter == null)
						{
							supporter = new Supporter() { city = city };
						}
						var troops = (onlyHome ? city.troopsHome : city.troopsTotal);
						s.Add(supporter);
						supporter.tSend.Clear();

						supporter.validTargets = validCount;
						if (viaWater)
						{
							var ttGalleys = troops.FirstOrDefault((tt) => tt.type == ttGalley);
							// handle Galleys
							if (ttGalleys.count > 0 )
							{
								var galleys = ttGalleys.count;
								var landTroops = troops.TS( (tt) => IsLandRaider(tt.type) );
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
								Set(ref supporter.tSend, new TroopTypeCount(ttGalley, galleys));
								foreach (var tt in troops.Enumerate())
								{
									if (tt.type == ttStinger)
									{
										Add( ref supporter.tSend ,  tt);
									}
									else
									{
										if (!IsLandRaider(tt.type) || !Include(tt))
											continue;
										Add(ref supporter.tSend ,new TroopTypeCount(tt.type, (int)(sendGain * tt.count))); // round down
									}
								}
							}
							else
							{
								Add( ref supporter.tSend, troops, (tt) => tt.type == ttStinger ); // take stingers
							}
						}
						else
						{
							Add(ref supporter.tSend,  troops, tt => (includeOffense || tt.isDef) && (canTravelViaWater||!tt.isNaval) ); // clone array
						}
						supporter.travel = hours + (defendants.Count - validCount);  // penality for targtes that we cannot make it to
					}
				}
                    if (portal)
                    {
                        if(onlyHome)
                            supporters.Set(s.OrderByDescending(a => a.tsHome),true);
                        else
                            supporters.Set(s.OrderByDescending(a => a.tsTotal),true);
                    }
                    else
                        supporters.Set(s.OrderBy(a => a.travel - a.validTargets ),true);
					
					defendants.NotifyReset();
                }


            }
            else
            {
				supporters.Clear(true);
				AppS.DispatchOnUIThreadLow( ()=>
				
                troopTypeGrid.ItemsSource=supportByTroopTypeEmpty
           );     //              supportGrid.ItemsSource = null;

            }
			await base.VisibilityChanged(visible, longTerm: longTerm);
        }

		private List<Spot> FindValidDefendants(bool viaWater, bool onlyHome, City city, ref float hours)
		{
			if (portal)
				return defendants.ToList<Spot>();

			var rv = new List<Spot>();
			foreach (var d in defendants)
			{

				if (city.ComputeTravelTime(d.cid, onlyHome, includeOffense,includeOffense,includeOffense, viaWater, filterTime, ref hours))
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
			SetupDataGrid(defendantGrid);

		}

      

        private void Coord_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var image = sender as FrameworkElement;
            
            var supporter = image.DataContext as Supporter;
            Spot.ProcessCoordClick(supporter.city.cid, false, AppS.keyModifiers,false);

        }

        private void Image_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var image = sender as FrameworkElement;
            var supporter = image.DataContext as Supporter;
            supporter.city.ShowContextMenu(supportGrid, e.GetPosition(supportGrid));
        }

        private void supportGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			if(isFocused)
	            RefreshSupportByType();
        }
        public void RefreshSupportByType()
        {
            Log("Refresh");
            var sel = supportGrid.SelectedItem;
            if( sel is Supporter support )
            {
                var stt = new List<SupportByTroopType>();
                foreach (var i in support.city.troopsTotal.Enumerate())
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
            flyout.CopyXamlRootFrom(text);
            AApp.AddItem(flyout, "Troops Home", (_, _) =>
            {
                var supporter = stt.supporter;
                Set( ref supporter.tSend, stt.type, stt.supporter.city.troopsHome.GetCount(stt.type));
                supporter.NotifyChange();
            });
            AApp.AddItem(flyout, "Total Troops", (_, _) =>
            {
                var supporter = stt.supporter;
                Set( ref supporter.tSend,stt.type, stt.supporter.city.troopsTotal.GetCount(stt.type));
                supporter.NotifyChange();
            });
            AApp.AddItem(flyout, "None", (_, _) =>
            {
                var supporter = stt.supporter;
                Set( ref supporter.tSend, stt.type, 0);
                supporter.NotifyChange();
            });

            flyout.ShowAt(text, e.GetPosition(text));

        }

        private void TsSendRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var text = sender as FrameworkElement;
            var supporter = text.DataContext as Supporter;
            var flyout = new MenuFlyout();
            flyout.CopyXamlRootFrom(text);
            AApp.AddItem(flyout, "Troops Home", (_, _) =>
            {
				supporter.tSend = supporter.city.troopsHome;
                supporter.NotifyChange();
            });
            AApp.AddItem(flyout, "Total Troops", (_, _) =>
            {
				supporter.tSend = supporter.city.troopsTotal;
                supporter.NotifyChange();
            });
            AApp.AddItem(flyout, "None", (_, _) =>
            {
                Clear(ref supporter.tSend);
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
            if(waitReturn && !supporter.city.troopsHome.IsSuperSetOf(supporter.tSend))
            {
				RaidOverview.SendMaybe();

				if (city.MightRaidsRepeat())
                {
                    await Raiding.ReturnFast(city.cid, false);
                }

                departAt = city.GetRaidReturnTime() + TimeSpan.FromSeconds(15);
                var canArriveAt = departAt+ TimeSpan.FromHours(supporter.travel );
                if (_arriveAt > CnVServer.ServerTime() && _arriveAt < canArriveAt)
                {
					var result = await AppS.DispatchOnUIThreadTask(async () =>
					{
						var msg = new ContentDialog()
						{
							Title = "Home Too late to make arrival time",
							Content = "Would you like to schedule as soon as they return?",
							PrimaryButtonText = "Yes",
							CloseButtonText = "Cancel"
						};
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
				var ts = supporter.tSend.DividedBy(def.Count);
				var cid = d.cid;
				await Post.SendRein(supporter.cid, cid, ts, departAt, _arriveAt, hours, supporter.split, text);
				Trace($"Sent {ts} from {supporter.cid.AsCity()} to {cid.AsCity()} @{_arriveAt.ToString()}");
				await Task.Delay(500);
			}
			IncomingOverview.ProcessTask();

			supporter.tSend.Clear();
			supporter.OnPropertyChanged();

		}
		//private void gridPointerPress(object sender, PointerRoutedEventArgs e)
		//{

		//	Spot.GridPressed(sender, e);

		//}


		private void supportGrid_Sorting(object sender,DataGridColumnEventArgs e)
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
                  //  supporters.SortSmall(comparer);
                 //   supporters.NotifyReset();
                }
                else if(e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
               //     supporters.SortSmall((b, a) => comparer(a,b) ); // swap order of comparison
              //      supporters.NotifyReset();
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

		private void troopTypeGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			Log(sender.ToString());
			Log(e.OriginalSource.ToString());
		}
	}

	
    

}
