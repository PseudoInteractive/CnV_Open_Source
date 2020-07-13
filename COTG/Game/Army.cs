using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.Foundation;
using Windows.UI.Xaml.Input;

namespace COTG.Game
{
    public class Army
	{
        public TroopTypeCount[] troops;
        public bool isAttack;
        public bool isDefense => !isAttack;
        public string xy => sourceCid.CidToString();
        public int targetCid;
        public int sourceCid;
        public string sourceCN => Spot.Get(sourceCid).name;
        public DateTimeOffset arrival { get; set; }
        public DateTimeOffset spotted { get; set; }
        public int ts => troops.Sum((t) => t.count);
        public string sumDef { get; set; }
        public int sumTs { get; set; }
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
                case nameof(xy) : JSClient.ShowCity(sourceCid); break;
            }
        }

    }
    public class TroopTypeCount
	{
        public int type;
        public int count;
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
	}
}
