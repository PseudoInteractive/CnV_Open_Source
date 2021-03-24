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
using System.Web;
using System.Text;

namespace COTG.Views
{

	public sealed partial class NearRes : UserTab, INotifyPropertyChanged
	{
		public static NearRes instance;
		public static bool IsVisible() => instance.isVisible;
		public bool viaWater { get; set; } = false;
		public static City defendant;
		public static bool includeOffense;
		public float filterTime = 6;
		public float _filterTime { get => filterTime; set { filterTime = value; Refresh(); } }  // defenders outside of this window are not included
		public int filterResHome { get; set; } = 1000;
		public int filterCartsHome { get; set; } = 100; // need at this this many ts at home to be considered for def
		public int filterShipsHome { get; set; } = 10; // need at this this many ts at home to be considered for def
		public DateTimeOffset arriveAt { get; set; } = AUtil.dateTimeZero;
		public static DumbCollection<ResSource> supporters = new DumbCollection<ResSource>();
		public static SupportByTroopType[] supportByTroopTypeEmpty = Array.Empty<SupportByTroopType>();
		public static int[] splitArray = { 1, 2, 3, 4, 5 };
		public static bool Include(TroopTypeCount tt) => includeOffense || tt.isDef;

		public Resources des;



		public static void GetSelected(List<int> rv)
		{
			var i = instance;
			if (!NearRes.IsVisible())
				return;

			foreach (var sel in i.supportGrid.SelectedItems)
			{
				var s = sel as ResSource;
				Assert(s != null);
				rv.AddIfAbsent(s.cid);
			}
		}

		public static async void UpdateTradeStuff()
		{
			var data = await Post.SendForJson("overview/tcounc.php");
			foreach (var city in data.RootElement[0].EnumerateArray())
			{
				var ct = new CityTradeInfo();
				var cid = city.GetAsInt("28");
				var cartStr = city.GetAsString("24").Split(@" \/ ", StringSplitOptions.RemoveEmptyEntries);
				int.TryParse(cartStr[0], out ct.cartsHome);
				int.TryParse(cartStr[1], out ct.cartsTotal);
				var shipStr = city.GetAsString("25").Split(@" \/ ", StringSplitOptions.RemoveEmptyEntries);
				int.TryParse(shipStr[0], out ct.shipsHome);
				int.TryParse(shipStr[1], out ct.shipsTotal);
				ct.res.wood = city.GetAsInt("6");
				ct.res.stone = city.GetAsInt("7");
				ct.res.iron = city.GetAsInt("8");
				ct.res.food = city.GetAsInt("9");
				City.GetOrAddCity(cid).tradeInfo = ct;

			}
			// second pass, bind cities
			foreach (var city in data.RootElement[0].EnumerateArray())
			{
				var cid = city.GetAsInt("28");

				var ct = City.GetOrAdd(cid).tradeInfo;
				if (city.TryGetProperty("22", out var sendTo))
				{
					foreach (var to in sendTo.EnumerateObject())
					{
						var toCid = int.Parse(to.Name);
						var cTo = City.GetOrAddCity(toCid);
						cTo.tradeInfo.resSource.Add(cid);
						ct.resDest.Add(toCid);

					}
				}
				if (city.TryGetProperty("23", out var sendFrom))
				{
					foreach (var to in sendFrom.EnumerateObject())
					{
						var toCid = int.Parse(to.Name);
						var cTo = City.GetOrAddCity(toCid);
						cTo.tradeInfo.resDest.Add(cid);
						ct.resSource.Add(toCid);

					}
				}

			}




			//for(var fromTo=0;fromTo<2;++fromTo)
			//{
			//	for(int rs =0;rs<4;++rs)
			//	{
			//		var cn = city.GetAsInt( (14+fromTo*4+rs).ToString() );


			//	}
			//}



		}
		public async override void VisibilityChanged(bool visible)
		{
			if (visible)
			{
				if (defendant == null)
					defendant = Spot.GetFocus();

				if (defendant != null && defendant.isCityOrCastle)
				{
					while (defendant.tradeInfo == null)
					{
						await Task.Delay(500);
					}

					List<ResSource> s = new List<ResSource>();
					//                supportGrid.ItemsSource = null;
					foreach (var city in City.gridCitySource)
					{
						Assert(city is City);
						TimeSpan dt;
						var ti = city.tradeInfo;
						if (viaWater)
						{
							if (!city.ComputeShipTravelTime(defendant.cid, out dt) || dt.TotalHours > filterTime || ti.shipsHome < filterShipsHome)
								continue;
						}
						else
						{
							if (!city.ComputeCartTravelTime(defendant.cid, out dt) || dt.TotalHours > filterTime || ti.cartsHome < filterCartsHome)
								continue;

						}


						// re-use if possible
						var supporter = supporters.Find((a) => a.city == city);
						if (supporter == null)
						{
							supporter = new ResSource() { city = city };
						}
						supporter.info = city.tradeInfo;
						s.Add(supporter);

						supporter.res.wood = 0;
						supporter.res.stone = 0;

						supporter.travel = dt;
					}
					supporters.Set(s.OrderBy(a => a.travel));
				}


			}
			else
			{
				supporters.Clear();
				troopTypeGrid.ItemsSource = supportByTroopTypeEmpty;
				//              supportGrid.ItemsSource = null;

			}
		}

		public NearRes()
		{
			Assert(instance == null);
			instance = this;
			this.InitializeComponent();

		}



		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName)
		{

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
		}

		private void TsSendRightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			var text = sender as FrameworkElement;
			var supporter = text.DataContext as ResSource;
			var flyout = new MenuFlyout();
			flyout.CopyXamlRoomFrom(text);
			AApp.AddItem(flyout, "Zero", (_, _) =>
			{
				supporter.res.Clear();
				supporter.NotifyChange();
			});
			AApp.AddItem(flyout, "Max", (_, _) =>
			{
				var info = supporter.info;
				var res = info.res.sum;
				var viaWater = NearRes.instance.viaWater;
				var shipping = viaWater ? info.shipsHome * 10000 : info.cartsHome * 1000;
				if (shipping > res)
				{
					supporter.res = info.res;  // we can send all of it
				}
				else
				{
					var ratio = shipping / (float)res;

					supporter.res = info.res.Scale(ratio);

				}

				supporter.NotifyChange();
			});
			

			flyout.ShowAt(text, e.GetPosition(text));
		}

		private async void SendClick(object sender, RoutedEventArgs e)
		{
			var text = sender as FrameworkElement;
			var s = text.DataContext as ResSource;
			var city = s.city;
			var pid = city.pid;

			var secret = $"JJx452Tdd{pid}sRAssa";
			var reqF = $"{{\"a\":{s.res.wood},\"b\":{s.res.stone},\"c\":{s.res.iron},\"d\":{s.res.food},\"cid\":{s.city.cid},\"rcid\":{city.cid},\"t\":\"{(viaWater?2:1)}\"}}"; // t==1 is land, t==2 is water

			Post.Send("includes/sndTr.php", $"cid={s.city.cid}&f=" + HttpUtility.UrlEncode(Aes.Encode(reqF, secret), Encoding.UTF8), pid);
			Note.Show($"Sent {s.res.Format()}");
		
			

		}


		private void supportGrid_Sorting(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e)
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
					supporters.Sort(comparer);
					supporters.NotifyReset();
				}
				else if (e.Column.SortDirection == DataGridSortDirection.Descending)
				{
					e.Column.SortDirection = DataGridSortDirection.Ascending;
					supporters.Sort((b, a) => comparer(a, b)); // swap order of comparison
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
			Refresh();
		}
	}

	public class CityTradeInfo
	{
		public static CityTradeInfo invalid = new CityTradeInfo();
		public List<int> resSource;
		public List<int> resDest;

		public int cartsHome;
		public int cartsTotal;

		public int shipsHome;
		public int shipsTotal;

		public Resources res;

	}


}
