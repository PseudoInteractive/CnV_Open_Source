using COTG.Helpers;
using COTG.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using COTG.Views;

using static COTG.Debug;
using System.ComponentModel;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.System;

namespace COTG.Game
{
    //public interface IKeyedItem
    //{
    //  public  int GetKey();
    //  public  void Ctor(int id);
    //}
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class Spot : IEquatable<Spot>, INotifyPropertyChanged
    {
        public virtual event PropertyChangedEventHandler PropertyChanged;
        public static ConcurrentDictionary<int, Spot> allSpots = new ConcurrentDictionary<int, Spot>(); // keyed by cid
        public static ConcurrentHashSet<int> selected = new ConcurrentHashSet<int>();
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public static Spot invalid = new Spot() { _cityName = "Null" };
        public static Spot GetOrAdd(int cid)
        {
            if (!Spot.allSpots.TryGetValue(cid, out var rv))
            {
                if (City.allCities.TryGetValue(cid, out var city))
                    rv = city; // re-use existing one if it is exists (this occurs for the players own cities)
                else
                {
                    rv = new Spot() { cid = cid };
                    var worldC = cid.CidToWorld();
                    var info = World.CityLookup(worldC);
                    rv.pid = info.player;
                    rv.isTemple = info.isTemple;
                    rv.isOnWater = info.isWater;
                    rv.isCastle = info.isCastle;
                    rv.points = (ushort)(info.isBig ? 8000 : 1500);
                }
                Spot.allSpots.TryAdd(cid, rv);
            }
            return (rv);
        }
        public static Spot Get(int cid)
        {
            if (Spot.allSpots.TryGetValue(cid, out var rv))
                return rv;
            
                if (City.allCities.TryGetValue(cid, out var city))
                    return city;
             
            return invalid;
        }

        public string _cityName;
        public string cityName=>  _cityName ?? xy;

        public int cid; // x,y combined into 1 number
        public string xy => cid.CidToString();//$"({cid % 65536}:{cid / 65536})";
        public int tsHome { get; set; }
        public int tsMax { get; set; }
        public int pid { get; set; }
        public string player => Player.Get(pid).name;
        public string alliance => Player.Get(pid).allianceName; // todo:  this should be an into alliance id
       // public DateTimeOffset lastUpdated { get; set; }
        public DateTimeOffset lastAccessed { get; set; } // lass user access
        public bool isCastle { get; set; }
        public bool isOnWater { get; set; }
        public bool isTemple { get; set; }
        public byte claim; // only if this is under attack
        public string Claim => $"{(int)claim,3:D0}%";
            public bool isBlessed { get; set; }
        public float scoutRange { get; set; }
        public ushort points { get; set; }
        public BitmapImage icon => ImageHelper.FromImages( isBlessed ? "blessed.png" :
             ($"{(isTemple ? "temple" : isCastle ? "castle" : "city")}{GetSize()}{(isOnWater?"w":"")}.png") );
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
            //var row = grid.HitTestService.RowItemFromPoint(point);
            //if(cell?.Item != row )
            //{
            //    Note.Show($"{cell} {row} {cell?.Column}");

            //}

            var spot = point.Y > 34 ? (cell?.Item as Spot) : null; // workaround for clicking on the header
            viewHover = spot != null ? spot.cid : 0;
            var uiHoverColumn = cell?.Column.Header?.ToString() ?? string.Empty;
            
            return (spot, uiHoverColumn, physicalPoint);
        }


        
        //public static void ProcessPointerMoved(object sender, PointerRoutedEventArgs e)
        //{
        //    HitTest(sender, e);
        //}
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
            viewHover = 0;
        //    uiHoverColumn = string.Empty;
        }

        
        public void ProcessClick(string column, PointerPoint pt)
        {
        //    Note.Show($"{this} {column} {pt.Position}");

            if (pt.Properties.IsLeftButtonPressed && !(App.IsKeyPressedControl()||App.IsKeyPressedShift()) ) // ignore selection style clicks
            {


                // If we are already selected and we get clicked, there will be no selection chagne to raids are not scanned automatically
                var wantRaidingFocus = (City.IsMine(cid) && MainPage.IsVisible());
                var wantRaidScan = false;
                //                var needCityData = 

                switch (column)
                {
                    case nameof(xy):
                        ProcessCoordClick(cid,false);
                        break;
                    case nameof(icon):
                        if (City.IsMine(cid))
                        {
                            var wasBuild = City.IsBuild(cid);
                            JSClient.ChangeCity(cid);
                            if( wasBuild )
                            {
                                    JSClient.ChangeView(!JSClient.IsCityView());

                            }

                        }
                        else JSClient.ShowCity(cid, false);
                        break;
                    case nameof(City.tsTotal):
                        if (City.IsMine(cid) && MainPage.IsVisible())
                        {
                            Raiding.UpdateTS(true);
                        }
                        break;
                    case nameof(tsHome):
                        if (City.IsMine(cid) && MainPage.IsVisible())
                        {
                            Raiding.UpdateTSHome(true);
                        }
                        break;
                    case nameof(City.raidReturn):
                        if (City.IsMine(cid) && MainPage.IsVisible())
                        {
                            City.SetFocus(cid, false, true, false); // prevent dungeon scan on select
                            Raiding.ReturnFast(cid, true);
                            return; // prevent the traiing dungeon scan
                        }
                        break;
                    case nameof(City.raidCarry):
                        if (City.IsMine(cid) && MainPage.IsVisible())
                        {
                            City.SetFocus(cid, false, true, false);// prevent dungeon scan on select
                            Raiding.ReturnSlow(cid, true);
                            return;// prevent trailing dungeon scan
                        }
                        break;
                    default:
                        wantRaidScan = true;
                        break;
                }

                if (wantRaidingFocus)
                    City.SetFocus(cid, false, wantRaidScan, false);// prevent dungeon scan on select
                if (MainPage.IsVisible() && City.focus == this)
                {
                    //                MainPage.SetRaidCity(cid,true);
                    ScanDungeons.Post(cid, true);
                }
            }
        }

        public static void ProcessCoordClick(int cid,bool lazyMove)
        {
            if (City.IsMine(cid) && City.IsFocus(cid))
            {
                if (City.IsBuild(cid))
                {
                    JSClient.ChangeView(!JSClient.IsCityView());// toggle between city/region view

                }
                else
                {
                    JSClient.ChangeCity(cid);
                }
            }
            else
            {
                JSClient.ShowCity(cid, lazyMove);
            }
        }


        // Incoming attacks
        public List<Army> incoming { get; set; } = new List<Army>();

        public int attacks
        {
            get
            {
                var rv = 0;
                foreach (var a in incoming)
                {
                    if (a.isAttack)
                        ++rv;
                }
                return rv;
            }
        }
        public int defenders
        {
            get
            {
                var rv = 0;
                foreach (var a in incoming)
                {
                    if (a.isDefense)
                        ++rv;
                }
                return rv;
            }
        }
        public int activeSieges
        {
            get 
                {
                    var rv = 0;
                    foreach (var a in incoming)
                    {
                        if (a.isSiege)
                            ++rv;
                    }
                    return rv;
                }
            }
        public bool iNav => incoming.Any((a) => a.hasNaval);
        public bool iSenny => incoming.Any((a) => a.Senny);
        public bool iArt => incoming.Any((a) => a.hasArt);

        public bool isFocus => City.focus == this;
        public bool isHover => viewHover == cid;
        public static bool IsHover(int cid)
        {
            return viewHover == cid;
        }

        public static List<int> GetSelected()
        {
            var rv = new List<int>();
            try
            {
                selected.EnterReadLock();
                rv.AddRange(selected._hashSet);
                if (viewHover != 0 && !selected.Contains(viewHover))
                {
                    rv.Add(viewHover);
                }
                if (City.focus != null)
                {
                    var cid = City.focus.cid;
                    if (cid != viewHover && !selected.Contains(cid))
                    {
                        rv.Add(cid);
                    }
                }

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
            var selected = GetSelected();
            foreach (var sel in selected)
            {
                if (allSpots.TryGetValue(sel, out var v))
                    rv.Add(v);

            }
            return rv;

        }
        public bool ToggleSelected()
        {
            bool rv = false;
            try
            {
                selected.EnterWriteLock();
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
            catch (Exception e)
            {
                Log(e);
            }
            finally
            {
                selected.ExitWriteLock();
            }
            SpotTab.SelectSilent(this, rv);
            return rv;
        }
        public static bool AreAnySelected()
        {
            return selected.Count != 0;// || viewHover != 0 || uiHover != 0;
        }
        public static bool IsSelectedOrHovered(int cid)
        {
            // if nothing is selected we treat it as if everything is selected
            return selected.Count == 0? true :  (cid == viewHover || selected.Contains(cid) || City.IsFocus(cid));
        }
        public static bool IsSelectedOrHovered(int cid0, int cid1)
        {
            // if nothing is selected we treat it as if everything is selected
            return selected.Count == 0 ? true : (cid0 == viewHover || selected.Contains(cid0) || City.IsFocus(cid0)
                                                || cid1 == viewHover || selected.Contains(cid1) || City.IsFocus(cid1));
        }


        public static int viewHover; // in the view menu

//        public static string uiHoverColumn = string.Empty;
        public static int uiPress; //  set when pointerPressed is recieved, at this point a contect menu might come up, causing us to lose uiHover
        public static string uiPressColumn = string.Empty;

        readonly static int[] pointSizes = { 1000,  6000 };

        const int pointSizeCount = 2;

        int GetSize()
        {
            for (int i = 0; i < pointSizeCount; ++i)
                if (points < pointSizes[i])
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

        public void ShowCity(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            JSClient.ChangeCity(cid);

        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }

        public override string ToString()
        {
            return $"{{{nameof(cityName)}={cityName}, {nameof(xy)}={xy}, {nameof(tsHome)}={tsHome.ToString()}, {nameof(tsMax)}={tsMax.ToString()}}}";
        }
        //int IKeyedItem.GetKey()
        //{
        //    return cid;
        //}
        //void IKeyedItem.Ctor(int i)
        //{
        //    cid = i;
        //}
    }
}
