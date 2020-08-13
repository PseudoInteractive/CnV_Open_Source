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
        public int tsHome => city.tsHome;
        public int tsTotal => city.tsTotal;
        public int split { get; set; } = 1; // splits def and sends in batches for wings
        public float travel { get; set; }

        public TroopTypeCount[] tSend;
        public int RangerTotal => city.troopsTotal.Count(Enum.ttRanger);
        public int RangerSend {
            get => tSend.Count(Enum.ttRanger);
            set => tSend=tSend.SetOrAdd(Enum.ttRanger, value);
        }

        public DateTimeOffset eta => JSClient.ServerTime() + TimeSpan.FromHours(travel);
        public DateTimeOffset etaWings => JSClient.ServerTime() + 0.5f*TimeSpan.FromHours(travel);
        public DateTimeOffset time { get; set; }

        public virtual event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
