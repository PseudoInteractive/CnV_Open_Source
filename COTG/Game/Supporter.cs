using COTG.Helpers;
using COTG.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

using TroopTypeCounts = COTG.Game.TroopTypeCounts;
//COTG.DArray<COTG.Game.TroopTypeCount>;
using TroopTypeCountsRef = COTG.Game.TroopTypeCounts;
using static COTG.Game.TroopTypeCountHelper;
//COTG.DArrayRef<COTG.Game.TroopTypeCount>;

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
        public int tsHome => NearDefenseTab.includeOffense ? city.tsHome : city.tsDefCityHome;
        public int tsTotal => NearDefenseTab.includeOffense ? city.tsTotal : city.tsDefCityTotal;
        public int split { get; set; } = 1; // splits def and sends in batches for wings
		public float travel;
		public int validTargets { get; set; }
		public string travelTime => TimeSpan.FromHours(travel).Format();
		public TroopTypeCountsRef tSend = new();
		public int tsSend
        {
            get => tSend.TS();
        }
        

       
        public DateTimeOffset eta { get => JSClient.ServerTime() + TimeSpan.FromHours(travel);
			set {
				NearDefenseTab.instance.arriveAt = value;
				NearDefenseTab.instance.OnPropertyChanged(string.Empty);
			}
		}
        public DateTimeOffset etaWings { get => JSClient.ServerTime() + 0.5f * TimeSpan.FromHours(travel); 
			set
			{
					NearDefenseTab.instance.arriveAt = value;
					NearDefenseTab.instance.OnPropertyChanged(string.Empty);
			}
		}
        public string troopInfo
        {
            get
            {

                string rv = "Troops Home/Total";
                foreach (var ttc in city.troopsTotal.Enumerate())
                {
                    rv += $"\n{Enum.ttNameWithCaps[ttc.type]}: {city.troopsHome.GetCount(ttc.type),4:N0}/{ttc.count,4:N0}";
                }
                return rv;
            }

        }

        public virtual event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		public void NotifyChange(string member = "")
		{
			App.DispatchOnUIThreadIdle(() =>
			{
				OnPropertyChanged(member);
				
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
        public int send { get => supporter.tSend.GetCount(type); set => Set(ref supporter.tSend,new TroopTypeCount( type,value) ); }
        public int home { get => supporter.city.troopsHome.GetCount(type); }
        public int total { get => supporter.city.troopsTotal.GetCount(type); }
    }
}
