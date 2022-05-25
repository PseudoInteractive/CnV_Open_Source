using System.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;

using static CnV.TroopTypeCountHelper;

//COTG.DArrayRef<COTG.TroopTypeCount>;

namespace CnV
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
        public uint tsHome => NearDefenseTab.includeOffense ? city.tsHome : city.tsDefCityHome;
        public uint tsTotal => NearDefenseTab.includeOffense ? city.tsTotal : city.tsDefCityTotal;
		public TimeSpanS travel;
		public int validTargets { get; set; }
		public string travelTime => travel.MulRoundUp(NearDefenseTab.instance.useHorns.Switch(1.0f,0.5f)).Format();
		public TroopTypeCounts tSend = new();
		public uint tsSend
        {
            get => tSend.TS();
        }
        

        public ServerTime departure {
			get {
				return city.WhenWillEnoughTroopsReturnToSend(tSend,considerDeparting: true);
				
			} }
       
        public ServerTime eta =>departure +  travel.MulRoundUp(NearDefenseTab.instance.useHorns.Switch(1.0f,0.5f)); 

        public string troopInfo
        {
            get
            {

                string rv = "Troops Home/Total";
                foreach (var ttc in city.troopsOwned)
                {
                    rv += $"\n{Troops.ttNames[ttc.type]}: {city.troopsHome.GetCount(ttc.type),4:N0}/{ttc.count,4:N0}";
                }
                return rv;
            }

        }
		public void OnPropertyChanged(string member = null)
		{
			if(PropertyChanged is not null) ((IANotifyPropertyChanged)this).IOnPropertyChanged(member);
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
        public TType type;

        public BitmapImage icon => ImageHelper.Get($"Icons/troops{type}.png");
        public string troopType => Troops.ttNames[type];
        public uint send { get => supporter.tSend.GetCount(type); set => supporter.tSend.SetInPlace(new (type,value) ) ; }
        public uint home { get => supporter.city.troopsHome.GetCount(type); }
        public uint total { get => supporter.city.troopsOwned.GetCount(type); }
    }
}
