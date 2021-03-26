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

		public Resources(int wood, int stone, int iron, int food)
		{
			this.wood = wood;
			this.stone = stone;
			this.iron = iron;
			this.food = food;
		}

		public int this[int index]
		{
			get => index switch { 0 => wood, 1 => stone, 2 => iron, _ => food };
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
		public Resources Min(Resources b)
		{
			// round down (truncate as it is positive)
			return new Resources(wood.Min(b.wood), stone.Min(b.stone), iron.Min(b.iron), food.Min(b.food));
		}
	

		public string Format() => $"{wood:N0} wood, {stone:N0} stone, {iron:N0} iron, {food:N0} food";
	
		public Resources Sub(Resources from)
		{
			return new Resources(wood - from.wood, stone - from.stone, iron - from.iron, food - from.food);
		}

		public Resources Add(Resources from) => new Resources(wood + from.wood, stone + from.stone, iron + from.iron, food + from.food);
		
		public void ClampToPositive()
		{
			wood = wood.Max(0);
			stone = stone.Max(0);		
			iron= iron.Max(0);
			food = food.Max(0);

		}
		internal void Clear()
		{
			wood = stone = iron = food = 0;
		}
	}
	public class ResSource : INotifyPropertyChanged
	{
		public static ResSource dummy=new ResSource();
		public City city;
		public CityTradeInfo info;
		public int cartsHome => info.cartsHome;
		public int cartsTotal => info.cartsTotal;

		public int shipsHome => info.shipsHome;
		public int shipsTotal => info.shipsTotal;
		public int totalRes => res.sum;

		public string xy => city.xy;
		
		public string Send => "Send"; // make shift button column
		public string name => city.nameAndRemarks;
		public Windows.UI.Xaml.Media.Imaging.BitmapImage icon => city.icon;
		public int cid => city.cid;
	
		public TimeSpan travel { get; set; }

		public Resources res;
		public int wood
		{
			get => res.wood;
			set => res.wood = value;
		}
		public int stone
		{
			get => res.stone;
			set => res.stone= value;
		}
		public int iron
		{
			get => res.iron;
			set => res.iron = value;
		}
		public int food
		{
			get => res.food;
			set => res.food = value;
		}

		public void NotifyChange(string member = "")
		{
			
			App.DispatchOnUIThreadSneakyLow(() =>
			{
				NearRes.supporters.OnPropertyChanged(this);
				OnPropertyChanged(member);
			});
		}
		public int ResMax(int type)
		{
			return info.res[type].Min(info.GetTransport(NearRes.instance.viaWater)); // TODO
		}

	public DateTimeOffset eta { get => JSClient.ServerTime() + travel; set => _ = value; }

		public virtual event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	}
	
}