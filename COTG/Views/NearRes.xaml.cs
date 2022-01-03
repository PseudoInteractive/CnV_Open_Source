using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using Microsoft.UI.Xaml.Media.Imaging;
using CommunityToolkit.WinUI.UI.Controls;

namespace CnV.Views
{
	using Game;
	using Services;

	public sealed partial class NearRes : UserTab
	{
		public static NearRes instance;
		public static bool IsVisible() => instance.isFocused;
		bool _viaWater;
		public bool viaWater { get =>_viaWater;
			set {
				_viaWater = value;
				DoRefresh(true);
			} }
		public bool templeDonation;
		public City target;
		public float filterTime = 6;
		public float _filterTime { get => filterTime; set { filterTime = value; DoRefresh(); } }  // defenders outside of this window are not included
		public int filterResHome { get; set; } = 1000;
		public int filterCartsHome { get; set; } = 100; // need at this this many ts at home to be considered for def
		public int filterShipsHome { get; set; } = 10; // need at this this many ts at home to be considered for def
		public DateTimeOffset arriveAt { get; set; } = AUtil.dateTimeZero;
		public static NotifyCollection<ResSource> supporters = new NotifyCollection<ResSource>();

		public BitmapImage targetIcon => target.icon;
		public string targetName => target.nameAndRemarks;
		public static bool useRatio => SettingsPage.nearResAsRatio;

		public Resources des = new Resources() { wood = 100000, stone = 100000, food = 100000, iron = 100000 };
		public Resources willHave => target.res.Add(target.tradeInfo.inc);
		

		public ResSource selected = ResSource.dummy;

//		public Resources send;
		public Resources reserve => SettingsPage.nearResReserve;

		//	public Resources reserve = new Resources(100000,100000,100000,100000);
		public static int reserveCarts =>SettingsPage.nearResCartReserve;
		public static int reserveShips => SettingsPage.nearResShipReserve;

		public int GetTransport(City city) => viaWater ? (city.shipsHome-SettingsPage.nearResShipReserve).Max0() * 10000 : (city.cartsHome-SettingsPage.nearResCartReserve).Max0() * 1000;


		public void RefreshSupportByRes()
		{
		//	Log("Refresh");
			var sel = supportGrid.SelectedItem;
			if (sel is ResSource support)
			{
				selected = support;
				selectedCommands.Visibility = Visibility.Visible;
			}
			else
			{
				selected = ResSource.dummy;
				selectedCommands.Visibility = Visibility.Collapsed;
			}
			OnPropertyChanged();
			//OnPropertyChanged(nameof(targetIcon));
			//OnPropertyChanged(nameof(sendWood));
			//OnPropertyChanged(nameof(sendStone));
			//OnPropertyChanged(nameof(sendIron));
			//OnPropertyChanged(nameof(sendFood));

		}

		static bool updating = false;
		static bool resetValuesPending;

		


		public async  Task DoRefresh(bool resetValues=false)
		{
			resetValuesPending = resetValues;
		
			if (updating )
				return;
			updating = true;
			try
			{
				Trace($"Update {DateTime.Now}");

				await UpdateTradeStuffDebounce.Go();

				int shipReserve = SettingsPage.nearResShipReserve;
				int cartReserve = SettingsPage.nearResCartReserve;

				if(resetValuesPending)
				{
					resetValuesPending = false;
					supporters.Clear(true);
				}

				//supportGrid.ItemsSource = null;
				if (target != null && target.isCityOrCastle)
				{
				

					var r = SettingsPage.nearResSend;// des.Sub(target.res.Add(target.tradeInfo.inc));
					
					var reserve = SettingsPage.nearResReserve;
					r.ClampToPositive();
					//views: SettingsPage.nearResSend = r;
					if(useRatio)
						r = r*((1<<30)/r.sum.Max(1));

					List<ResSource> s = new List<ResSource>();
					//                supportGrid.ItemsSource = null;
					{

						foreach (var city in City.myCities)
						{
							if (city == target)
								continue;
							TimeSpan dt;
							var ti = city.tradeInfo;
							if (viaWater)
							{
								if (!city.ComputeShipTravelTime(target.cid, out dt) || dt.TotalHours > filterTime || city.shipsHome < filterShipsHome + shipReserve|| city.res.Sub(reserve).sum < filterResHome)
									continue;
							}
							else
							{
								if (!city.ComputeCartTravelTime(target.cid, out dt) || dt.TotalHours > filterTime || city.cartsHome < filterCartsHome + cartReserve  || city.res.Sub(reserve).sum < filterResHome)
									continue;

							}


							// re-use if possible
							var supporter = supporters.c.FirstOrDefault((a) => a.city == city);
							if (supporter == null)
							{
								supporter = new ResSource() { city = city };
							}
							
							supporter.info = city.tradeInfo;
							s.Add(supporter);


							supporter.travel = dt;
						}
					}
					s = s.OrderBy(a => a.travel).ToList();
					foreach (var sup in s)
					{
						if (!sup.initialized)
						{
							var info = sup.info;
							var city = sup.city;
							var shipping = GetTransport( city);//, viaWater ? (city.shipsHome - shipReserve).Max0() * 10000 : (city.cartsHome - cartReserve).Max0() * 1000;
							var send = r;
							if(shipping < r.sum)
							{
								send *= (shipping / (double)r.sum.Max(1));
							}
							send = send.Min(sup.city.res.SubSat(SettingsPage.nearResReserve));
							Assert( send.sum <= shipping);
	

							//if (shipping < sum)
							//{

							//	send *= (shipping / (double)sum);
							//}
							sup.res = send;
							sup.initialized = true;
						}
						if(!useRatio)
							r = r.Sub(sup.res);
						//AppS.DispatchOnUIThreadIdle(() =>
						//{
						//	supporters.OnPropertyChanged(sup);
						//	sup.OnPropertyChanged(string.Empty);
						//});

					}
					supporters.ItemContentChanged();
					supporters.Set(s,true);

					//	supportGrid.ItemsSource = supporters;

				}
				AppS.DispatchOnUIThreadIdle(() =>
				{
					RefreshSupportByRes();
					OnPropertyChanged(nameof(targetIcon));
					OnPropertyChanged(nameof(targetName));
				});
			}
			finally
			{
				updating = false;
			}
		}

		public override Task VisibilityChanged(bool visible, bool longTerm)
		{
			if (target == null)
				target = City.GetBuild();
			if (visible)
			{
				if (target == null)
					target = Spot.GetFocus();

				DoRefresh(true);
				
			}
			else
			{
				supporters.Clear(true);
				selected = ResSource.dummy;
			}
			return base.VisibilityChanged(visible, longTerm: longTerm);
		}

		public NearRes()
		{
			Assert(instance == null);
			instance = this;
			this.InitializeComponent();

		}



		private void Coord_Tapped(object sender, TappedRoutedEventArgs e)
		{
			var image = sender as FrameworkElement;

			var supporter = image.DataContext as ResSource;
			Spot.ProcessCoordClick(supporter.city.cid, false, App.keyModifiers, false);

		}

		private void Image_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			var image = sender as FrameworkElement;
			var supporter = image.DataContext as ResSource;
			supporter.city.ShowContextMenu(supportGrid, e.GetPosition(supportGrid));
		}

		private void supportGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!isOpen)
				return;

			if (isFocused)
				RefreshSupportByRes();
		}

		private void TsSendRightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			var text = sender as FrameworkElement;
			var supporter = text.DataContext as ResSource;
			var flyout = new MenuFlyout();
			flyout.CopyXamlRootFrom(text);
			AApp.AddItem(flyout, "Zero", (_, _) =>
			{
				supporter.res.Clear();
				DoRefresh();
				supporter.NotifyChange();
			});
			AApp.AddItem(flyout, "Recalc", (_, _) =>
			{
				supporter.initialized = false;
				DoRefresh();
			});
			AApp.AddItem(flyout, "Max", (_, _) =>
			{
				MaxClicked(supporter);
			});
			

			flyout.ShowAt(text, e.GetPosition(text));
		}

		private void MaxClicked(ResSource supporter)
		{
			var info = supporter.info;
			var city = supporter.city;
			var res = supporter.city.res.Sub(reserve).Max(0);
		//	var viaWater = NearRes.instance.viaWater;
			var shipping = GetTransport(city);//viaWater ? (city.shipsHome - SettingsPage.nearResShipReserve).Max0() * 10000 : (city.cartsHome - SettingsPage.nearResCartReserve).Max0() * 1000;
			if (shipping > res.sum)
			{
				supporter.res = res;  // we can send all of it
			}
			else
			{
				var ratio = shipping / (float)res.sum.Max(1);

				supporter.res = res.Scale(ratio);

			}


			DoRefresh();
			supporter.NotifyChange();
		}

		private async void SendClick(object sender, RoutedEventArgs e)
		{
			App.UpdateKeyStates();

			var text = sender as FrameworkElement;
			var s = text.DataContext as ResSource;
			var city = s.city;
			var pid = city.pid;
			var cid = city.cid;
			var secret = $"JJx452Tdd{pid}sRAssa";
			var reqF = $"{{\"a\":{s.res.wood},\"b\":{s.res.stone},\"c\":{s.res.iron},\"d\":{s.res.food},\"cid\":{s.city.cid},\"rcid\":{target.cid},\"t\":\"{(viaWater?2:1)}\"}}"; // t==1 is land, t==2 is water
			int count = AppS.IsKeyPressedShiftAndControl() ? 8 : 1;
			Trace(count.ToString());
			string res = string.Empty;
			var asDonation = this.SendAsDontation.IsOn;
			if(asDonation)
			{
				await BlessedCity.SendDonation(s.city.cid, target.cid, s.res.wood, s.res.stone, viaWater );
			}
			else
			{
				for (int j = 0; j < count; ++j)
				{

					res = await Post.SendForText("includes/sndTr.php",
						$"cid={cid}&f=" + HttpUtility.UrlEncode(Aes.Encode(reqF, secret), Encoding.UTF8), pid);
					if (int.TryParse(res.Trim(), out var i) && i == 10)
					{
						Note.Show($"Sent {s.res.Format()}");
					}
					else
					{
						Note.Show($"Something changed, please refresh and try again");
					}

					if (count == 1)
						break;
					await Task.Delay(450);
				}
			}

			s.res.Clear();
			s.OnPropertyChanged();
			AppS.DispatchOnUIThreadIdle(() =>
			{
				s.NotifyChange();
				DoRefresh();
			});
	//		Analytics.TrackEvent("NearResSend");

		}


		private void supportGrid_Sorting(object sender,DataGridColumnEventArgs e)
		{
			var dg = supportGrid;
			var tag = e.Column.Tag?.ToString();
			//Use the Tag property to pass the bound column name for the sorting implementation
			Comparison<ResSource> comparer = null;
			switch (tag)
			{
				case nameof(ResSource.cartsHome): comparer = (a, b) => b.cartsHome.CompareTo(a.cartsHome); break;
				case nameof(ResSource.shipsHome): comparer = (a, b) => b.shipsHome.CompareTo(a.shipsHome); break;
				case nameof(ResSource.totalRes): comparer = (a, b) => b.totalRes.CompareTo(a.totalRes); break;
				case nameof(ResSource.travel): comparer = (b, a) => b.travel.CompareTo(a.travel); break;
			}

			if (comparer != null)
			{
				//Implement sort on the column "Range" using LINQ
				if (e.Column.SortDirection == null)
				{
					e.Column.SortDirection = DataGridSortDirection.Descending;
				//	supporters.SortSmall(comparer);
					supporters.NotifyReset();
				}
				else if (e.Column.SortDirection == DataGridSortDirection.Descending)
				{
					e.Column.SortDirection = DataGridSortDirection.Ascending;
				//	supporters.SortSmall((b, a) => comparer(a, b)); // swap order of comparison
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
				if (dgColumn.Tag != null && dgColumn.Tag.ToString() != tag)
				{
					dgColumn.SortDirection = null;
				}
			}
		}

		private void NumberBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
		{
			supporters.Clear(false);
			DebouceItemChanged.Go();
		
		}
		Debounce DebouceItemChanged = new (async ()=>
		{
			await instance.DoRefresh();
			supporters.NotifyReset();
		});

		private async void ItemValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
		{
			DebouceItemChanged.Go();
		}

		//private async void ToggleSwitch_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		//{
		//	//	supporters.Clear();
		//	await DoRefresh();
		//	supporters.NotifyReset();
		//}



		private void MaxResClick(int id)
		{
			selected.res[id] = selected.ResMax(id);
			//var viaWater = NearRes.instance.viaWater;
			var info = selected.info;
			var transport = GetTransport(selected.city);
			if (viaWater)
				transport -= (selected.res[id] + 9999) / 10000 * 10000;
			else
				transport -= (selected.res[id] + 999) / 1000 * 1000;
			var sumOther = 0;
			for(int i=0;i<4;++i)
			{
				if (i == id)
					continue;
				sumOther += selected.res[i];
			}
			if( sumOther > transport )
			{
				var scale = transport / (double)sumOther.Max(1);
				for (int i = 0; i < 4; ++i)
				{
					if (i == id)
						continue;
					selected.res[i] = (int)( selected.res[i] * scale);
				}

			}
			selected.NotifyChange();
			RefreshSupportByRes();
		}
		int sendWood
		{
			get => selected.res[0];
			set
			{
				selected.res[0] = value;
				selected.NotifyChange();
			}
		}
		int sendStone
		{
			get => selected.res[1];
			set
			{
				selected.res[1] = value;
				selected.NotifyChange();
			}
		}
		int sendIron
		{
			get => selected.res[2];
			set
			{
				selected.res[2] = value;
				selected.NotifyChange();
			}
		}
		int sendFood
		{
			get => selected.res[3];
			set
			{
				selected.res[3] = value;
				selected.NotifyChange();
			}
		}

		private void MaxWoodClick(object sender, RoutedEventArgs e)
		{
			MaxResClick(0);
		}
		private void ZeroWoodClick(object sender, RoutedEventArgs e)
		{
			selected.res[0] = 0;
			selected.NotifyChange();
			RefreshSupportByRes();
		}
		private void MaxStoneClick(object sender, RoutedEventArgs e)
		{
			MaxResClick(1);
		}
		private void ZeroStoneClick(object sender, RoutedEventArgs e)
		{
			selected.res[1] = 0;
			selected.NotifyChange();
			RefreshSupportByRes();
		}

		private void MaxIronClick(object sender, RoutedEventArgs e)
		{
			MaxResClick(2);
		}
		private void ZeroIronClick(object sender, RoutedEventArgs e)
		{
			selected.res[2] = 0;
			selected.NotifyChange();
			RefreshSupportByRes();
		}

		private void MaxFoodClick(object sender, RoutedEventArgs e)
		{
			MaxResClick(3);
		}
		private void ZeroFoodClick(object sender, RoutedEventArgs e)
		{
			selected.res[3] = 0;
			selected.NotifyChange();
			RefreshSupportByRes();
		}



		private void TargetTapped(object sender, TappedRoutedEventArgs e)
		{
			Spot.ProcessCoordClick(target.cid, false, App.keyModifiers, false);

		}

		private void ZeroClick(object sender, RoutedEventArgs e)
		{
			for(int i=0;i<4;++i)
				selected.res[i] = 0;
			selected.NotifyChange();
			RefreshSupportByRes();
		}
		private void RecalcClick(object sender, RoutedEventArgs e)
		{
			selected.initialized = false;
			DoRefresh();
		}

		private void MaxClick(object sender, RoutedEventArgs e)
		{
			MaxClicked(selected);
		}

		private void ResSetMax(object sender, TappedRoutedEventArgs e)
		{
			//Trace(e.OriginalSource.ToString());

		}

		private void ResSetZero(object sender, RightTappedRoutedEventArgs e)
		{
			//Trace(sender.ToString());

		}
	}
}
