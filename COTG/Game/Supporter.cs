﻿using System.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;

//COTG.DArray<COTG.Game.TroopTypeCount>;
using TroopTypeCountsRef = CnV.Game.TroopTypeCounts;
using static CnV.TroopTypeCountHelper;

//COTG.DArrayRef<COTG.Game.TroopTypeCount>;

namespace CnV.Game
{
	public class Supporter : IANotifyPropertyChanged
    {
        public City city;
        public string xy => city.xy;
        public string SendOrLocked => city.underSiege ? "Sieged" : "Send";
//        public string SendOrLocked => (city.cid&1)==0  ? "Sieged" : "Send";
        public string Send => "Send"; // make shift button column
        public string name => city.nameAndRemarks;
        public Microsoft.UI.Xaml.Media.Imaging.BitmapImage icon => city.icon;
        public int cid => city.cid;
        public string raidReturn => city.raidReturn;
        public int tsHome => NearDefenseTab.includeOffense ? city.tsHome : city.tsDefCityHome;
        public int tsTotal => NearDefenseTab.includeOffense ? city.tsTotal : city.tsDefCityTotal;
        public int split { get; set; } = 1; // splits def and sends in batches for wings
		public float travel;
		public int validTargets { get; set; }
		public string travelTime => TimeSpan.FromHours(travel).Format();
		public TroopTypeCounts tSend = new();
		public int tsSend
        {
            get => tSend.TS();
        }
        

       
        public DateTimeOffset eta { get => CnVServer.ServerTime() + TimeSpan.FromHours(travel);
			set {
				NearDefenseTab.instance.arriveAt = value;
				NearDefenseTab.instance.OnPropertyChanged(string.Empty);
			}
		}
        public DateTimeOffset etaWings { get => CnVServer.ServerTime() + 0.5f * TimeSpan.FromHours(travel); 
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
                    rv += $"\n{Troops.ttNameWithCaps[ttc.type]}: {city.troopsHome.GetCount(ttc.type),4:N0}/{ttc.count,4:N0}";
                }
                return rv;
            }

        }
		public void OnPropertyChanged(string member = null)
		{
			if(PropertyChanged is not null) ((IANotifyPropertyChanged)this).IOnPropertyChanged();
		}
		public virtual event PropertyChangedEventHandler PropertyChanged;
		public void CallPropertyChanged(string members = null)
		{
			PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(members));
		}
		public void NotifyChange(string member = "")
		{
			AppS.DispatchOnUIThreadIdle(() =>
			{
				OnPropertyChanged(member);
				
				if (NearDefenseTab.instance.supportGrid.SelectedItem == this)
					NearDefenseTab.instance.RefreshSupportByType();

			});
		}

		public void ProcessedTapped(string columnHeaderText)
		{
			switch(columnHeaderText)
			{

				case nameof(xy):
					Spot.ProcessCoordClick(cid, false, AppS.keyModifiers);
					break;
			}
		}
    }
    // Proxy for a datagrid that displays 1 row per troop type
    public class SupportByTroopType
	{
        public Supporter supporter;
        public int type;

        public BitmapImage icon => ImageHelper.FromImages($"Icons/troops{type}.png");
        public string troopType => Troops.ttNameWithCaps[type];
        public int send { get => supporter.tSend.GetCount(type); set => Set(ref supporter.tSend,new TroopTypeCount( type,value) ); }
        public int home { get => supporter.city.troopsHome.GetCount(type); }
        public int total { get => supporter.city.troopsTotal.GetCount(type); }
    }
}
