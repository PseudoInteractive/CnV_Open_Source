using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV
{
	using System.ComponentModel;

	public class ResSource:IANotifyPropertyChanged
	{
		public virtual event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string member = null)
		{
			if(PropertyChanged is not null) ((IANotifyPropertyChanged)this).IOnPropertyChanged();
		}
		public void CallPropertyChanged(string members = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(members));
		}


		public static ResSource dummy = new ResSource();
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
		public string ResString(int index) => $"{res[index]:N0} ({city.resources[index]:N0})";
		public void ResSetMax(int index) => res[index] = ResMax(index);
		public void ResSetZero(int index) => res[index] = 0;

		public void NotifyChange(string member = "")
		{

			OnPropertyChanged(member);
		}
		public int ResMax(int type)
		{
			return (city.resources[type]- Settings.nearResReserve[type]).Min(NearRes.instance.GetTransport(city)); // TODO
		}

		public DateTimeOffset eta { get => CnVServer.serverTime + travel; set => _ = value; }



	}
}
