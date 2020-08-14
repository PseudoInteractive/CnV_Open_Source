using COTG.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Game
{
    public class Supporter : INotifyPropertyChanged
    {
        public City city;
        public string xy => city.xy;
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
        public int RangerHome { get => city.troopsHome.Count(Enum.ttRanger); set => _ = value; }
        public int RangerTotal { get => city.troopsTotal.Count(Enum.ttRanger); set =>_=value; }
        public int RangerSend {  get => tSend.Count(Enum.ttRanger);      set => tSend=tSend.SetOrAdd(Enum.ttRanger, value);  }

        public int TriariHome { get => city.troopsHome.Count(Enum.ttTriari); set => _ = value; }
        public int TriariTotal { get => city.troopsTotal.Count(Enum.ttTriari); set => _ = value; }
        public int TriariSend { get => tSend.Count(Enum.ttTriari); set => tSend = tSend.SetOrAdd(Enum.ttTriari, value); }

        public DateTimeOffset eta { get => JSClient.ServerTime() + TimeSpan.FromHours(travel); set => _ = value; }
        public DateTimeOffset etaWings { get => JSClient.ServerTime() + 0.5f * TimeSpan.FromHours(travel); set => _ = value; }
        public DateTimeOffset time { get; set; }

        public virtual event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
