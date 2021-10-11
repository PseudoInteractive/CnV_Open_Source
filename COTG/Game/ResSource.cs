using COTG.Helpers;
using COTG.Views;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

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
		[JsonInclude]
		public int wood;
		[JsonInclude]
		public int stone;
		[JsonInclude]
		public int iron;
		[JsonInclude]
		public int food;
		internal static Resources zero = default; // all zeros

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

		public Resources Scale(double s)
		{
			// round down (truncate as it is positive)
			return new Resources() { wood = (int)(wood * s), stone = (int)(stone * s), iron = (int)(iron * s), food = (int)(food * s) };
		}
		public static Resources operator * (Resources r, double s)
		{
			// round down (truncate as it is positive)
			return r.Scale(s);
		}
		public static Resources operator /(Resources r,double s)
		{
			// round down (truncate as it is positive)
			return r.Scale(1.0/s);
		}
		public Resources Min(Resources b)
		{
			// round down (truncate as it is positive)
			return new Resources(wood.Min(b.wood), stone.Min(b.stone), iron.Min(b.iron), food.Min(b.food));
		}

		public Resources Max(int val)
		{
			// round down (truncate as it is positive)
			return new Resources(wood.Max(val), stone.Max(val), iron.Max(val), food.Max(val));
		}
		public string Format() => $"{wood.Format()} wood, {stone:N0} stone, {iron:N0} iron, {food:N0} food";
	
		public Resources Sub(Resources from)
		{
			return new Resources(wood - from.wood, stone - from.stone, iron - from.iron, food - from.food);
		}
		public Resources SubSat(Resources from)
		{
			return new Resources((wood - from.wood).Max0(),(stone - from.stone).Max0(),(iron - from.iron).Max0(),(food - from.food).Max0());
		}
		public static Resources operator -(Resources a, Resources b)
		{
			return a.Sub(b);
		}
		public static Resources operator +(Resources a,Resources b)
		{
			return a.Add(b);
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
		public bool isNonZero => (wood | stone | iron | food) != 0;
	}

	public struct ResourceFilter
	{
		[JsonInclude]
		public bool? wood;
		[JsonInclude]
		public bool? stone;
		[JsonInclude]
		public bool? iron;
		[JsonInclude]
		public bool? food;
		public bool Any => wood.HasValue | stone.HasValue | iron.HasValue | food.HasValue;
		public static ResourceFilter _true = new ResourceFilter(true, true, true, true);
		public static ResourceFilter _false = new ResourceFilter(false, false, false, false);
		public static ResourceFilter _null = default;
		public ResourceFilter(bool? wood, bool? stone, bool? iron, bool? food)
		{
			this.wood = wood;
			this.stone = stone;
			this.iron = iron;
			this.food = food;
		}

	}
	public struct ResourcesNullable
	{
		[JsonInclude]
		public int? wood;
		[JsonInclude]
		public int? stone;
		[JsonInclude]
		public int? iron;
		[JsonInclude]
		public int? food;
		public bool Any => wood.HasValue | stone.HasValue | iron.HasValue | food.HasValue;

		public ResourcesNullable(int? wood, int? stone, int? iron, int? food)
		{
			this.wood = wood;
			this.stone = stone;
			this.iron = iron;
			this.food = food;
		}

		public static ResourcesNullable zero = new ResourcesNullable(0, 0, 0, 0);
		public static ResourcesNullable _null = default;
		public bool isNull => (wood is null) & (stone is null) & (iron is null) & (food is null);
		public int? this[int index]
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
		public string Format() => $"{wood:N0} wood, {stone:N0} stone, {iron:N0} iron, {food:N0} food";
		internal void Clear()
		{
			wood = stone = iron = food = null;
		}
		public static implicit operator ResourcesNullable( Resources r )
		{
			return new(r.wood,r.stone,r.iron,r.food);
		}
	}
	public class ResSource : IANotifyPropertyChanged
	{
		public virtual event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string member = null)
		{
			if(PropertyChanged is not null) ((IANotifyPropertyChanged)this).IOnPropertyChanged();
		}
		public void CallPropertyChanged(string members = null)
		{
			PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(members));
		}


		public static ResSource dummy=new ResSource();
		public City city;
		public bool initialized;
		public CityTradeInfo info;
		public int cartsHome => city.cartsHome;
		public int cartsTotal => city.carts;

		public int shipsHome => city.shipsHome;
		public int shipsTotal => city.ships;
		public int totalRes => res.sum;

		public string xy => city.xy;
		
		public string Send => "Send"; // make shift button column
		public string name => city.nameAndRemarks;
		public Microsoft.UI.Xaml.Media.Imaging.BitmapImage icon => city.icon;
		public int cid => city.cid;
	
		public TimeSpan travel { get; set; }

		public Resources res;
		public string ResString(int index) => $"{res[index]:N0} ({city.res[index]:N0})";
		public void ResSetMax(int index) => res[index] = ResMax(index);
		public void ResSetZero(int index) => res[index] = 0;

		public void NotifyChange(string member = "")
		{
			
				OnPropertyChanged(member);
		}
		public int ResMax(int type)
		{
			return (city.res[type]- SettingsPage.nearResReserve[type]).Min(NearRes.instance.GetTransport(city)); // TODO
		}

		public DateTimeOffset eta { get => JSClient.ServerTime() + travel; set => _ = value; }

		

	}

}