using COTG.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace COTG.Game
{
    public class Spot
    {
        readonly static int[] pointSizes = { 500, 1000, 2500, 4000, 5500, 7000, 8000 };

        const int pointSizeCount = 7;

        int GetSize()
        {
            for (int i = 0; i < pointSizeCount; ++i)
                if (points <= pointSizes[i])
                    return i;
            return pointSizeCount;
        }
        public string name { get; set; }
        public int cid; // x,y combined into 1 number
        public string xy => $"{cid % 65536}:{cid / 65536}";
        public int pid { get; set; }
        public string player => Player.Get(pid).name;
        public string alliance => Player.Get(pid).allianceName; // todo:  this should be an into alliance id
        public DateTimeOffset lastUpdated { get; set; }
        public DateTimeOffset lastAccessed { get; set; } // lass user access
        public bool isCastle { get; set; }
        public bool isOnWater { get; set; }
        public bool isTemple { get; set; }
        public ushort points { get; set; }
        public BitmapImage icon => ImageHelper.FromImages($"{(isCastle ? "castle" : "city")}{GetSize()}.png");

    }
}
