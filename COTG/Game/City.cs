using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using FluentAssertions;
using FluentAssertions.Common;
using static COTG.Debug;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace COTG.Game
{
	public class City
	{
        public string name { get; set; }
        public int cid; // x,y combined into 1 number
        public string xy => $"{cid / 65536}:{cid % 65536}";
        public string owner { get; set; } // todo: this shoule be an int playerId
        public string alliance { get; set; }// todo:  this should be an into alliance id
        public string notes { get; set; }
        public DateTime lastUpdated { get; set; }
        public DateTime lastAccessed { get; set; } // lass user access
        public bool isCastle { get; set; }
        public bool isOnWater { get; set; }
        public bool isTemple { get; set; }

        public BitmapImage icon { get; set; }
        public bool IsShowingRowDetails {
            get => this == Views.MainPage.showingRowDetails; // this is nicely coupled
            set => Log($"{(this == Views.MainPage.showingRowDetails)}","{value}");
                }

        public static Dictionary<int,City> all = new Dictionary<int, City>(); // keyed by city
	}
    public class BuildingCount
    {
        public BitmapImage image { get; set; }
        public int count { get; set; }

    }

}
