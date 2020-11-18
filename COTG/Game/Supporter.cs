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
        public int raidReturn => city.raidReturn.RoundToInt();
        public int tsHome => city.tsHome;
        public int tsTotal => city.tsTotal;
        public int split { get; set; } = 1; // splits def and sends in batches for wings
        public float travel { get; set; }

        public TroopTypeCount[] tSend;
        public int tsSend
        {
            get => tSend.TS();
        }
        public void NotifyChange(string member = "")
        {
            App.DispatchOnUIThreadSneaky(() =>
            {
                OnPropertyChanged(member);
                Debug.Log("NotifyChange");

                if (DefendTab.instance.supportGrid.SelectedItem == this)
                    DefendTab.instance.RefreshSupportByType();

            });
        }

        public int TriariHome { get => city.troopsHome.Count(Enum.ttTriari); set => _ = value; }
        public int TriariTotal { get => city.troopsTotal.Count(Enum.ttTriari); set => _ = value; }
        public int TriariSend { get => tSend.Count(Enum.ttTriari); set => tSend = tSend.SetOrAdd(Enum.ttTriari, value); }

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

    }
    // Proxy for a datagrid that displays 1 row per troop type
    public class SupportByTroopType
    {
        public Supporter supporter;
        public int type;

        public BitmapImage icon => ImageHelper.FromImages($"troops{type}.png");
        public string troopType => Enum.ttNameWithCaps[type];
        public int send { get => supporter.tSend.Count(type); set => supporter.tSend = supporter.tSend.SetOrAdd(type,value); }
        public int home { get => supporter.city.troopsHome.Count(type); }
        public int total { get => supporter.city.troopsTotal.Count(type); }
    }
}
