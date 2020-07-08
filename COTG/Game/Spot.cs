using COTG.Helpers;
using COTG.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using static COTG.Debug;
namespace COTG.Game
{
    public interface IKeyedItem
    {
      public  int GetKey();
      public  void Ctor(int id);
    }
    public class Spot : IEquatable<Spot>, IKeyedItem
    {
        public static ConcurrentDictionary<int, Spot> allSpots = new ConcurrentDictionary<int, Spot>(); // keyed by cid
        public static ConcurrentHashSet<int> selected = new ConcurrentHashSet<int>();

        public static List<int> GetSelected()
        {
            var rv = new List<int>();
            try
            {
                selected.EnterReadLock();
                rv.AddRange(selected._hashSet);
                if (uiHover != 0 && !selected.Contains(uiHover))
                {
                        rv.Add(uiHover);
                }
                if (viewHover != 0 && viewHover != uiHover &&
                    !selected.Contains(viewHover))
                        rv.Add(viewHover);

            }
            finally
            {
                selected.ExitReadlLock();
            }
            return rv;

        }
        public static List<Spot> GetSelectedSpots()
        {
            var rv = new List<Spot>();
            var selected= GetSelected();
            foreach(var sel in selected)
            {
                if (allSpots.TryGetValue(sel, out var v))
                    rv.Add(v);

            }
            return rv;

        }
        public static bool ToggleSelected(int cid)
        {
            bool rv = false;
            try
            {
                selected._lock.EnterWriteLock();

                if (selected._hashSet.Contains(cid))
                {
                    rv = false;
                    selected._hashSet.Remove(cid);
                }
                else
                {
                    rv = true;
                    selected._hashSet.Add(cid);
                }
            }
            finally
            {
                selected._lock.ExitWriteLock();
            }
            return rv;
        }
        public bool ToggleSelected()
        {
            return Spot.ToggleSelected(cid);
        }
        public static bool AreAnySelected()
        {
            return selected.Count != 0;// || viewHover != 0 || uiHover != 0;
        }
        public static bool IsSelectedOrHovered( int cid)
        {
            return (cid == viewHover || cid == uiHover || selected.Contains(cid));
        }

        public static int viewHover; // in the view menu

        public static int uiHover; // in the DataGrids
        public static string uiHoverColumn = string.Empty;
        public static int uiPress; //  set when pointerPressed is recieved, at this point a contect menu might come up, causing us to lose uiHover
        public static string uiPressColumn = string.Empty;

        readonly static int[] pointSizes = { 500, 1000, 2500, 4000, 5500, 7000, 8000 };

        const int pointSizeCount = 7;

        int GetSize()
        {
            for (int i = 0; i < pointSizeCount; ++i)
                if (points <= pointSizes[i])
                    return i;
            return pointSizeCount;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Spot);
        }

        public bool Equals(Spot other)
        {
            return other != null &&
                   cid == other.cid;
        }

        public override int GetHashCode()
        {
            return cid;
        }

         int IKeyedItem.GetKey()
        {
           return cid;
        }
         void IKeyedItem.Ctor(int i)
        {
            cid = i;
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
        public int continent => cid.CidToContinent();

        public static bool operator ==(Spot left, Spot right)
        {
            return EqualityComparer<Spot>.Default.Equals(left, right);
        }

        public static bool operator !=(Spot left, Spot right)
        {
            return !(left == right);
        }

        

        public static (Spot spot, string column, PointerPoint pt) HitTest(object sender, PointerRoutedEventArgs e)
        {
            var grid = sender as RadDataGrid;
            var physicalPoint = e.GetCurrentPoint(grid);
            var point = new Point { X = physicalPoint.Position.X, Y = physicalPoint.Position.Y };
            var cell = grid.HitTestService.CellInfoFromPoint(point);
            var spot = (cell?.Item as Spot);
            uiHover = spot != null ? spot.cid : 0;
            uiHoverColumn = cell?.Column.Header?.ToString() ?? string.Empty;
            
            return (spot, uiHoverColumn, physicalPoint);
        }
        public static void ProcessPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            HitTest(sender, e);
        }
        public static void ProcessPointerPress(object sender, PointerRoutedEventArgs e)
        {
            var hit= Spot.HitTest(sender, e);
            var spot = hit.spot;
            uiPress = spot != null ? spot.cid : 0;
            uiPressColumn = hit.column;
            if (spot != null)
                spot.ProcessClick(hit.column, hit.pt);
        }
        public static void ProcessPointerExited()
        {
            ClearHover();
        }

        public static void ClearHover()
        {
            uiHover = 0;
            uiHoverColumn = string.Empty;
        }

        public void ProcessClick(string column, PointerPoint pt)
        {
            if (pt.Properties.IsLeftButtonPressed)
            {
                switch (column)
                {
                    case "xy": JSClient.ShowCity(cid); break;
                    case "icon": JSClient.ChangeCity(cid); break;
                    case "ts":
                    case "tsHome":
                        {
                            ScanDungeons.Post(cid); break;
                        }
                }
            }
        }


    }
}
