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
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.AppCenter.Analytics;

namespace COTG.Views
{

	public sealed partial class NearRes : UserTab, INotifyPropertyChanged
	{
		public static NearRes instance;
		public static bool IsVisible() => instance.isVisible;
		public bool viaWater { get; set; } = false;
		public City target;
		public float filterTime = 6;
		public float _filterTime { get => filterTime; set { filterTime = value; DoRefresh(); } }  // defenders outside of this window are not included
		public int filterResHome { get; set; } = 1000;
		public int filterCartsHome { get; set; } = 100; // need at this this many ts at home to be considered for def
		public int filterShipsHome { get; set; } = 10; // need at this this many ts at home to be considered for def
		public DateTimeOffset arriveAt { get; set; } = AUtil.dateTimeZero;
		public static DumbCollection<ResSource> supporters = new DumbCollection<ResSource>();

		public BitmapImage targetIcon => target.icon;
		public string targetName => target.nameAndRemarks;


		public Resources des = new Resources() { wood = 1000000, stone = 1000000, food = 1000000, iron = 1000000 };
		public Resources willHave => target.tradeInfo.res.Add(target.tradeInfo.inc);
		

		public ResSource selected = ResSource.dummy;

		public Resources send;



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
				selectedCommands.Visibility = Visibility.Collapsed;
			}
		}

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

		public static async Task UpdateTradeStuff()
		{

			var data = await Post.SendForJson("overview/tcounc.php");
			foreach (var js in data.RootElement[0].EnumerateArray())
			{
				
				var cid = js.GetAsInt("28");
				var city = City.GetOrAddCity(cid);
				CityTradeInfo ct;
				if (city.tradeInfo != CityTradeInfo.invalid)
				{
					ct = city.tradeInfo;
					ct.resDest.Clear();
					ct.resSource.Clear();
				}
				else
				{
					ct = new CityTradeInfo();
					city.tradeInfo = ct;
				}

				var cartStr = js.GetAsString("24").Split(@" / ", StringSplitOptions.RemoveEmptyEntries);
				int.TryParse(cartStr[0], out ct.cartsHome);
				int.TryParse(cartStr[1], out ct.cartsTotal);
				var shipStr = js.GetAsString("25").Split(@" / ", StringSplitOptions.RemoveEmptyEntries);
				int.TryParse(shipStr[0], out ct.shipsHome);
				int.TryParse(shipStr[1], out ct.shipsTotal);
				ct.res.wood = js.GetAsInt("6");
				ct.res.stone = js.GetAsInt("7");
				ct.res.iron = js.GetAsInt("8");
				ct.res.food = js.GetAsInt("9");
				

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
						cTo.tradeInfo.resSource.AddIfAbsent(cid);

						ct.resDest.AddIfAbsent(toCid);

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

//			await GetCity.Post(target.cid, (jse, c) => { });


			//for(var fromTo=0;fromTo<2;++fromTo)
			//{
			//	for(int rs =0;rs<4;++rs)
			//	{
			//		var cn = city.GetAsInt( (14+fromTo*4+rs).ToString() );


			//	}
			//}



		}

		public async  Task DoRefresh()
		{
			await UpdateTradeStuff();

			//supportGrid.ItemsSource = null;
			if (target != null && target.isCityOrCastle)
			{
				while (target.tradeInfo == CityTradeInfo.invalid)
				{
					await Task.Delay(500);
				}

				var r = des.Sub(target.tradeInfo.res.Add(target.tradeInfo.inc));
				r.ClampToPositive();
				List<ResSource> s = new List<ResSource>();
				//                supportGrid.ItemsSource = null;
				foreach (var city in City.gridCitySource)
				{
					if (city == target)
						continue;
					TimeSpan dt;
					var ti = city.tradeInfo;
					if (viaWater)
					{
						if (!city.ComputeShipTravelTime(target.cid, out dt) || dt.TotalHours > filterTime || ti.shipsHome < filterShipsHome)
							continue;
					}
					else
					{
						if (!city.ComputeCartTravelTime(target.cid, out dt) || dt.TotalHours > filterTime || ti.cartsHome < filterCartsHome)
							continue;

					}


					// re-use if possible
					var supporter = supporters.Find((a) => a.city == city);
					if (supporter == null)
					{
						supporter = new ResSource() { city = city };
					}
					if(city.tradeInfo==null)
					{
						Assert(false);
						continue;
					}
					supporter.info = city.tradeInfo;
					s.Add(supporter);


					supporter.travel = dt;
				}
				s = s.OrderBy(a => a.travel).ToList();
				foreach (var sup in s)
				{
					var info = sup.info;
					var shipping = viaWater ? info.shipsHome * 10000 : info.cartsHome * 1000;
					var send = info.res.Min(r);
					var sum = send.sum;
					
					if (shipping < sum)
					{
						var ratio = shipping / (double)sum;

						send = send.Scale(ratio);
					}
					sup.res = send;
					r = r.Sub(send);
					supporters.OnPropertyChanged(sup);
					sup.OnPropertyChanged(string.Empty);

				}
				supporters.Set(s);
				
			//	supportGrid.ItemsSource = supporters;

				RefreshSupportByRes();
			}
			OnPropertyChanged(nameof(targetIcon));
			OnPropertyChanged(nameof(targetName));

		}

		public async override void VisibilityChanged(bool visible)
		{
			if (visible)
			{
				if (target == null)
					target = Spot.GetFocus();

				DoRefresh();
				
			}
			else
			{
				supporters.Clear();
				selected = ResSource.dummy;
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
			RefreshSupportByRes();
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
				RefreshSupportByRes();
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

				RefreshSupportByRes();

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
			var reqF = $"{{\"a\":{s.res.wood},\"b\":{s.res.stone},\"c\":{s.res.iron},\"d\":{s.res.food},\"cid\":{s.city.cid},\"rcid\":{target.cid},\"t\":\"{(viaWater?2:1)}\"}}"; // t==1 is land, t==2 is water

			var res = await Post.SendForText("includes/sndTr.php", $"cid={s.city.cid}&f=" + HttpUtility.UrlEncode(Aes.Encode(reqF, secret), Encoding.UTF8), pid);
			if (int.TryParse(res.Trim(), out var i) && i == 10)
			{
				Note.Show($"Sent {s.res.Format()}");
			}
			else
			{
				Note.Show($"Something changed, please refresh and try again");
			}
			DoRefresh();
			Analytics.TrackEvent("NearResSend");

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

		private void NumberBox_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
		{
			supporters.Clear();

			DoRefresh();
			supporters.NotifyReset();
		}

		private void ToggleSwitch_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			supporters.Clear();
			DoRefresh();
			supporters.NotifyReset();
		}



		private void MaxResClick(int id)
		{
			selected.res[id] = selected.ResMax(id);
			var viaWater = NearRes.instance.viaWater;
			var info = selected.info;
			var transport = info.GetTransport(viaWater);
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
				var scale = transport / (double)sumOther;
				for (int i = 0; i < 4; ++i)
				{
					if (i == id)
						continue;
					selected.res[i] = (int)( selected.res[i] * scale);
				}

			}
			selected.NotifyChange();
		}


		private void MaxWoodClick(object sender, RoutedEventArgs e)
		{
			MaxResClick(0);
		}
		private void ZeroWoodClick(object sender, RoutedEventArgs e)
		{
			selected.res[0] = 0;
			selected.NotifyChange();
		}
		private void MaxStoneClick(object sender, RoutedEventArgs e)
		{
			MaxResClick(1);
		}
		private void ZeroStoneClick(object sender, RoutedEventArgs e)
		{
			selected.res[1] = 0;
			selected.NotifyChange();
		}

		private void MaxIronClick(object sender, RoutedEventArgs e)
		{
			MaxResClick(2);
		}
		private void ZeroIronClick(object sender, RoutedEventArgs e)
		{
			selected.res[2] = 0;
			selected.NotifyChange();
		}

		private void MaxFoodClick(object sender, RoutedEventArgs e)
		{
			MaxResClick(3);
		}
		private void ZeroFoodClick(object sender, RoutedEventArgs e)
		{
			selected.res[3] = 0;
			selected.NotifyChange();
		}



		private void TargetTapped(object sender, TappedRoutedEventArgs e)
		{
			Spot.ProcessCoordClick(target.cid, false, App.keyModifiers, false);

		}
	}

	public class CityTradeInfo
	{
		public static CityTradeInfo invalid = new CityTradeInfo();
		public List<int> resSource = new List<int>();
		public List<int> resDest = new List<int>();

		public int cartsHome;
		public int cartsTotal;

		public int shipsHome;
		public int shipsTotal;

		public int GetTransport( bool viaWater ) => viaWater ? shipsHome*10000 : cartsHome*1000;
		
		public Resources res;
		public Resources inc;
		public int resTotal => res.sum;

	}


}