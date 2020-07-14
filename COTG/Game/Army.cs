using COTG.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace COTG.Game
{
    public class Army
    {
        public TroopTypeCount[] troops;
        public TroopTypeCount[] sumDef;
        public bool isAttack;
        public bool isDefense => !isAttack;
        public string xy => sourceCid.CidToString();
        public int targetCid;
        public int sourceCid;
        public string sourceCN => Spot.Get(sourceCid).name;
        public DateTimeOffset arrival { get; set; }
        public DateTimeOffset spotted { get; set; }
        public int ts => troops.Sum((t) => t.ts);
        public int sumTs => sumDef.Sum((t) => t.ts);
        public string details => TroopTypeCount.Format(troops);
        public int pid;
        public string playerName => Player.IdToName(pid);

        public static (Army spot, string column, Vector2 point) HitTest(object sender, TappedRoutedEventArgs e)
        {
            var grid = sender as RadDataGrid;
            var physicalPoint = e.GetPosition(grid);
            var point = new Point { X = physicalPoint.X, Y = physicalPoint.Y };
            var cell = grid.HitTestService.CellInfoFromPoint(point);
            var army = (cell?.Item as Army);


            return (army, cell?.Column.Header?.ToString() ?? string.Empty, physicalPoint.ToVector2());
        }
        public void ProcessTap(string column)
        {
            switch (column)
            {
                case "city":
                case nameof(xy): JSClient.ShowCity(sourceCid); break;
            }
        }
        public static string cN(TroopTypeCount[] troops,int n) => troops.Length > n ? $" {troops[n].count:N0} " : null;
        public static BitmapImage iN(TroopTypeCount[] troops, int n) => troops.Length > n ? ImageHelper.FromImages($"troops{troops[n].type}.png") : null;

        public string c0        => cN(troops,0);
        public BitmapImage i0   => iN(troops, 0);

        public string c1 => cN(troops, 1);
        public BitmapImage i1 => iN(troops, 1);


        public string c2 => cN(troops, 2);
        public BitmapImage i2 => iN(troops, 2);

        public string c3 => cN(troops, 3);
        public BitmapImage i3 => iN(troops, 3);


        public string sc0 => cN(sumDef, 0);
        public BitmapImage si0 => iN(sumDef, 0);

        public string sc1 => cN(sumDef, 1);
        public BitmapImage si1 => iN(sumDef, 1);


        public string sc2 => cN(sumDef, 2);
        public BitmapImage si2 => iN(sumDef, 2);

        public string sc3 => cN(sumDef, 3);
        public BitmapImage si3 => iN(sumDef, 3);


    }
    public class TroopTypeCount : IComparable<TroopTypeCount>
    {
        public int type;
        public int count;

        public TroopTypeCount() { }
        public TroopTypeCount(TroopTypeCount b)
        {
            type = b.type;
            count = b.count;
        }
        public static string Format( IEnumerable<TroopTypeCount> l )
        {
            string rv = "";
            var wantComma = false;
            foreach (var ttc in l)
            {
                if (wantComma)
                    rv += ",";
                else wantComma = true;
                rv += $"{ttc.count,4:N0} {Enum.ttNameWithCaps[ttc.type]}";
            }
            return rv;
        }
        public int ts => Enum.ttTs[type] * count;
        public static int TS(IEnumerable<TroopTypeCount> l)
        {
            var rv = 0;
            foreach (var ttc in l)
            {
                rv += ttc.ts;
            }
            return rv;
        }
        public static void Sort(TroopTypeCount[] l) => Array.Sort(l);

        // Sort greatest TS to least TS
        int IComparable<TroopTypeCount>.CompareTo(TroopTypeCount other)
        {
            return other.ts.CompareTo(ts);
        }
    }
}
