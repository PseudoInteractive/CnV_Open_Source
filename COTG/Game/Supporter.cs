using COTG.Helpers;
using COTG.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace COTG.Game
{
    public class Supporter : INotifyPropertyChanged
    {
        public City city;
        public string xy => city.xy;
        public string SendOrLocked => city.underSiege ? "Sieged" : "Send";
//        public string SendOrLocked => (city.cid&1)==0  ? "Sieged" : "Send";
        public string Send => "Send"; // make shift button column
        public string name => city.nameAndRemarks;
        public Windows.UI.Xaml.Media.Imaging.BitmapImage icon => city.icon;
        public int cid => city.cid;
        public string raidReturn => city.raidReturn;
        public int tsHome => NearDefenseTab.includeOffense ? city.tsHome : city.tsDefHome;
        public int tsTotal => NearDefenseTab.includeOffense ? city.tsTotal : city.tsDefTotal;
        public int split { get; set; } = 1; // splits def and sends in batches for wings
        public float travel { get; set; }

        public TroopTypeCount[] tSend;
        public int tsSend
        {
            get => tSend.TS();
        }
        

       
        public DateTimeOffset eta { get => JSClient.ServerTime() + TimeSpan.FromHours(travel); set => _ = value; }
        public DateTimeOffset etaWings { get => JSClient.ServerTime() + 0.5f * TimeSpan.FromHours(travel); set => _ = value; }
        public string troopInfo
        {
            get
            {

                string rv = "Troops Home/Total";
                foreach (var ttc in city.troopsTotal)
                {
                    rv += $"\n{Enum.ttNameWithCaps[ttc.type]}: {city.troopsHome.Count(ttc.type),4:N0}/{ttc.count,4:N0}";
                }
                return rv;
            }

        }

        public virtual event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

	}
    // Proxy for a datagrid that displays 1 row per troop type
    public class SupportByTroopType
	{
        public Supporter supporter;
        public int type;

        public BitmapImage icon => ImageHelper.FromImages($"Icons/troops{type}.png");
        public string troopType => Enum.ttNameWithCaps[type];
        public int send { get => supporter.tSend.Count(type); set => supporter.tSend = supporter.tSend.SetOrAdd(type,value); }
        public int home { get => supporter.city.troopsHome.Count(type); }
        public int total { get => supporter.city.troopsTotal.Count(type); }
    }
}
