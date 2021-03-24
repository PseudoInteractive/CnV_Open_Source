using COTG.Helpers;
using COTG.Views;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;


namespace COTG.Game
{
	public struct Resources
	{
		public static string[] names =
		{
			"wood",
			"stone",
			"iron",
			"food"
		};

		public int wood;
		public int stone;
		public int iron;
		public int food;
		public int this[int index]
		{
			get => index switch { 0 => wood, 1 => stone, 2 => iron, _ => 3 };
			set
			{
				switch (index)
				{
					case 0: wood = value; break;
					case 1: stone = value; break;
					case 2: iron = value; break;
					default: food = value; break;
				}

			}
		}
		public int sum => wood + stone + iron + food;

		public Resources Scale(float s)
		{
			// round down (truncate as it is positive)
			return new Resources() { wood = (int)(wood * s), stone = (int)(stone * s), iron = (int)(iron * s), food = (int)(food * s) };
		}

		internal void Clear()
		{
			wood = stone = iron = food = 0;
		}
	}
	public class ResSource : INotifyPropertyChanged
	{
		public City city;
		public CityTradeInfo info;
		public int cartsHome => info.cartsHome;
		public int cartsTotal => info.cartsTotal;

		public int shipsHome => info.shipsHome;
		public int shipsTotal => info.shipsTotal;


		public string xy => city.xy;
		public string SendOrLocked => city.underSiege ? "Sieged" : "Send";
		//        public string SendOrLocked => (city.cid&1)==0  ? "Sieged" : "Send";
		public string Send => "Send"; // make shift button column
		public string name => city.nameAndRemarks;
		public Windows.UI.Xaml.Media.Imaging.BitmapImage icon => city.icon;
		public int cid => city.cid;
		public int tsHome => NearDefenseTab.includeOffense ? city.tsHome : city.tsDefHome;
		public TimeSpan travel { get; set; }

		public Resources res;
		
		public void NotifyChange(string member = "")
		{
			App.DispatchOnUIThreadSneakyLow(() =>
			{
				OnPropertyChanged(member);
				Debug.Log("NotifyChange");

				if (NearDefenseTab.instance.supportGrid.SelectedItem == this)
					NearDefenseTab.instance.RefreshSupportByType();

			});
		}


		public DateTimeOffset eta { get => JSClient.ServerTime() + travel; set => _ = value; }

		public virtual event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	}
	// Proxy for a datagrid that displays 1 row per troop type
	public class SupportByResType
	{
		public ResSource supporter;

		public int type; // 
		public string name => Resources.names[type];
		public int res
		{
			get => supporter.res[type];
			set => supporter.res[type] = value;
		}


		}
}