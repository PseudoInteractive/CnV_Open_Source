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
using Windows.UI.Xaml.Controls;
using COTG.JSON;
using System.Text.Json;
using Windows.Web.Http;
using static COTG.Game.Enum;
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
        public static ConcurrentDictionary<int, Spot> allSpots = new ConcurrentDictionary<int, Spot>(); // keyed by cid
        public static ConcurrentHashSet<int> selected = new ConcurrentHashSet<int>();
        public static Spot[] defendersI = Array.Empty<Spot>();
        public static Spot[] defendersO = Array.Empty<Spot>();

        public static bool TryGet(int cid, out Spot spot) => allSpots.TryGetValue(cid, out spot);
        public static int focus; // city that has focus (selected, but not necessarily building.  IF you click a city once, it goes to this state

        public virtual event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        internal static Spot GetFocus()
        {
            return focus == 0 ? null : GetOrAdd(focus);
        }

        public static Spot invalid = new Spot() { _cityName = "?" };
        public static Spot pending = new Spot() { _cityName = "pending" };

        public static Spot[] emptySpotSource = new[] { pending };
        public virtual string nameAndRemarks => cityName;

        public static bool IsFocus(int cid)
        {
            return focus == cid;
        }
        public static Spot GetOrAdd(int cid, string cityName = null)
        {
            if (cid <= 0)
            {
                return invalid;
            }
            if (!Spot.TryGet(cid, out var rv))
            {
                Assert(City.allCities.ContainsKey(cid) == false);
                var worldC = cid.CidToWorld();
                var info = World.GetInfo(worldC);
               
                //    Assert(info.type == World.typeCity);
                rv = new Spot() { cid = cid, pid = info.player };
                //       Assert( info.player != 0);
                rv.type = (byte)(info.type >> 28);
                if (info.type == 0)
                {
                    Log("Uninitialized");
                }
                rv.isTemple = info.isTemple;
                rv.isOnWater = info.isWater;
                rv.isCastle = info.isCastle;
                rv.points = (ushort)(info.isBig ? 8000 : 1500);

                Spot.allSpots.TryAdd(cid, rv);
            }
            if (cityName != null)
                rv._cityName = cityName;
            return (rv);
        }
        public static void UpdateName(int cid, string name) => GetOrAdd(cid, name);
        public bool isMine => pid == Player.myId;

        public string _cityName;
        public string cityName => _cityName ?? xy;

        public int cid; // x,y combined into 1 number
        public string xy => cid.CidToString();//$"({cid % 65536}:{cid / 65536})";
        public int x => cid % 65536;
        public int y => cid >> 16;

        public int tsRaid { get; set; }
        public int tsHome { get; set; }
        public int tsMax { get { var i = incomingDefTS; return (i > 0) ? i : tsHome; } }
        public int pid { get; set; }
        public string player => Player.Get(pid).name;
        public string alliance => Player.Get(pid).allianceName; // todo:  this should be an into alliance id

        public byte type;
        public const byte typeCity = 0x1;
        public const byte typeShrine = 0x2;
        public const byte typePortal = 0x3;
        public const byte typeBoss = 0x4;
        public const byte typeDungeon = 0x5;
        public const byte typeNone = 0x0;
        // Water and obscurred terriain are not defined :(
        public bool isCityOrCastle => type == typeCity;
        public bool isBoss => type == typeBoss;
        public bool isDungeon => type == typeDungeon;
        public bool isEmpty => type == typeNone;

        public DateTimeOffset lastAccessed { get; set; } // lass user access
        public byte attackCluster { get; set; } // For attackTab, 0 is real, 1 is fake cluster 1, 2 is fake cluster 2 etc.
        public Attack attackAssignment;
        public enum Classification : byte
        {
            unknown,
            infO,
            infD,
            inf,
            magic,
            academy,
            stablesO,
            stablesD,
            stables,
            se,
            hub,
            navy,
            misc
        }
        public Classification classification { get; set; }

        private static string[] classifications =
        {
            "unknown",
            "infO",
            "infD",
            "inf",
            "magic",
            "academy",
            "stablesO",
            "stablesD",
            "stables",
            "se",
            "hub",
            "navy",
            "misc"
        };
        public string classificationString => classifications[(int)classification];
        public string attackers { get {
                var rv = new List<string>();

                foreach (var atk in AttackTab.attacks)
                {
                    if (atk.target == cid)
                    {
                        rv.Add(atk.player);
                    }
                }
                if (rv.IsNullOrEmpty())
                    return "None";
                else
                    return string.Join(',', rv);
            } }
        public bool isCastle { get; set; }
        public bool isOnWater { get; set; }
        public bool isTemple { get; set; }
        public byte claim; // only if this is under attack
        public string Claim => $"{(int)claim:00}%";
            public bool isBlessed { get; set; }
        public float scoutRange { get; set; }
        public ushort points { get; set; }
        public BitmapImage icon => ImageHelper.FromImages( isBlessed ? "blessed.png" :
             ($"{(isTemple ? "temple" : isCastle ? "castle" : "city")}{GetSize()}{(isOnWater?"w":"")}.png") );
        public int cont => cid.CidToContinent();

        public static bool operator ==(Spot left, Spot right)
        {
            return EqualityComparer<Spot>.Default.Equals(left, right);
        }

        public static bool operator !=(Spot left, Spot right)
        {
            return !(left == right);
        }

        
        // The UIElement returned will be the RadDataGrid
        public static (Spot spot, string column, PointerPoint pt,UIElement uie) HitTest(object sender, PointerRoutedEventArgs e)
        {
            e.KeyModifiers.UpdateKeyModifiers();
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
            Player.viewHover = spot != null ? spot.pid : 0;
            var uiHoverColumn = cell?.Column.Header?.ToString() ?? string.Empty;
            
            return (spot, uiHoverColumn, physicalPoint,grid);
        }


        
        //public static void ProcessPointerMoved(object sender, PointerRoutedEventArgs e)
        //{
        //    HitTest(sender, e);
        //}
        public static void ProcessPointerPress(object sender, PointerRoutedEventArgs e)
        {
            e.KeyModifiers.UpdateKeyModifiers();

            var hit = Spot.HitTest(sender, e);
            var spot = hit.spot;
            uiPress = spot != null ? spot.cid : 0;
            uiPressColumn = hit.column;
            // The UIElement returned will be the RadDataGrid
            if (spot != null)
                spot.ProcessClick(hit.column, hit.pt,hit.uie,e.KeyModifiers);
        }
        public static void ProcessPointerExited()
        {
            ClearHover();
        }

        public static void ClearHover()
        {
            viewHover = 0;
            Player.viewHover = 0;
        //    uiHoverColumn = string.Empty;
        }


        public void ProcessClick(string column, PointerPoint pt, UIElement uie, VirtualKeyModifiers modifiers)
        {
            modifiers.UpdateKeyModifiers();
            //    Note.Show($"{this} {column} {pt.Position}");

            if (pt.Properties.IsLeftButtonPressed && !(modifiers.IsShiftOrControl())) // ignore selection style clicks
            {


                // If we are already selected and we get clicked, there will be no selection chagne to raids are not scanned automatically
                var wantRaidingScan = (City.IsMine(cid) && MainPage.IsVisible());
                var wantRaidScan = isFocus;
                //                var needCityData = 

                switch (column)
                {
                    case nameof(xy):
                        ProcessCoordClick(cid, false,modifiers);
                        wantRaidScan = false;
                        break;
                    case "I":
                        if (City.IsMine(cid))
                        {
                            var wasBuild = City.IsBuild(cid);
                            JSClient.ChangeCity(cid, false);
                            if (wasBuild)
                            {
                                JSClient.ChangeView(!JSClient.IsCityView());

                            }

                        }
                        else
                        {
                            JSClient.ShowCity(cid, false);
                        }
                        wantRaidScan = false;
                        break;
                    case nameof(City.tsTotal):
                        if (City.IsMine(cid) && MainPage.IsVisible())
                        {
                            Raiding.UpdateTS(true, true);
                        }
                        wantRaidScan = false;
                        break;
                    case nameof(tsHome):
                    case nameof(tsRaid):
                        if (City.IsMine(cid) && MainPage.IsVisible())
                        {
                            Raiding.UpdateTS(true, true);
                        }
                        wantRaidScan = false;
                        break;
                    case nameof(City.raidReturn):
                        if (City.IsMine(cid) && MainPage.IsVisible())
                        {
                            Raiding.ReturnFast(cid, true);
                        }
                        wantRaidScan = false;
                        break;
                    case nameof(City.raidCarry):
                        if (City.IsMine(cid) && MainPage.IsVisible())
                        {
                            Raiding.ReturnSlow(cid, true);
                        }
                        wantRaidScan = false;
                        break;
                    default:
                        wantRaidScan = true;
                        break;
                }


                if (MainPage.IsVisible() && isMine && wantRaidScan)
                {
                    //                MainPage.SetRaidCity(cid,true);
                    ScanDungeons.Post(cid, true);
                }
                SetFocus();
                NavStack.Push(cid);

            }
            else if (pt.Properties.IsRightButtonPressed)
            {
                ShowContextMenu(uie, pt.Position);
                return;
            }
            SpotTab.TouchSpot(cid,modifiers);
        }

        

        public static async void ProcessCoordClick(int cid,bool lazyMove,VirtualKeyModifiers mod)
        {
            mod.UpdateKeyModifiers();
            if (City.IsMine(cid) && !mod.IsShiftOrControl())
            {
                if (City.IsBuild(cid))
                {
                    JSClient.ChangeView(!JSClient.IsCityView());// toggle between city/region view

                    cid.BringCidIntoWorldView(lazyMove);
                }
                else
                {
                    JSClient.ChangeCity(cid, lazyMove); // keep current view, switch to city
                }
                NavStack.Push(cid);

            }
            else
            {
                JSClient.ShowCity(cid, lazyMove);
                NavStack.Push(cid);
              
                
            }
            SpotTab.TouchSpot(cid,mod);

            if (mod.IsShiftAndControl())
            {
                var str = await Post.SendForText("includes/gLay.php", $"cid={cid}");
                Log(str);
                App.DispatchOnUIThreadSneaky(() =>
                {
                    App.CopyTextToClipboard(str);
                    Launcher.LaunchUriAsync(new Uri($"http://cotgopt.com/?map={str}"));
                });
            }
        }
        internal async Task Classify()
        {
            var str = await Post.SendForText("includes/gLay.php", $"cid={cid}");

            try
            {
                const int start = 14;
                const int end = 459;
                var stables = 0;
                var academies = 0;
                var training = 0;
                var sorc = 0;
                var se = 0;
                for(int i=start;i<end;++i)
                {
                    switch(str[i])
                    {
                        case 'E': ++stables;break;
                        case 'Y': ++se;break;
                        case 'J': ++sorc; break;
                        case 'G': ++training; break;
                        case 'Z': ++academies;break;

                    }
                }
                var mx = stables.Max(academies).Max(training.Max(sorc)).Max(academies.Max(training)).Max(se);
                if(mx <= 4)
                {
                    classification = Classification.misc;
                }
                else if(mx==stables)
                {
                    if (se > 0 || academies > 0 || stables == 39 || stables == 24 || stables <= 20)
                        classification = Classification.stablesO;
                    else                     {
                        classification = Classification.stablesD;
                    }
                }
                else if(mx==sorc)
                {
                    classification = Classification.magic;
                }
                else if(mx==training)
                {
                    if (se > 0 || academies > 0 || training == 26 || training <= 18 )
                        classification = Classification.infO;
                    else
                        classification = Classification.infD;

                }
                else if (mx == academies)
                {
                    classification = Classification.academy;
                }
                else if (mx == se)
                {
                    classification = Classification.se;
                }
                else
                {
                    classification = Classification.misc;
                }

            }
            catch (Exception e)
            {
                Log(e);
            }



        }


        // Incoming attacks
        public Army[] incoming { get; set; } = Army.empty;

        public int incomingAttacks
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
        public int incomingDefenders
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

        public int incomingDefTS
        {
            get
            {
                var rv = 0;
                foreach (var a in incoming)
                {
                    if (a.isDefense)
                        rv += a.ts;
                }
                return rv;
            }
        }

        public int incomingAttackTS
        {
            get
            {
                var rv = 0;
                foreach (var a in incoming)
                {
                    if (!a.isDefense)
                        rv += a.ts;
                }
                return rv;
            }
        }

        //public int activeSieges
        //{
        //    get 
        //        {
        //            var rv = 0;
        //            foreach (var a in incoming)
        //            {
        //                if (a.isSiege)
        //                    ++rv;
        //            }
        //            return rv;
        //        }
        //    }
        public bool iNav => incoming.Any((a) => a.hasNaval);
        public bool iSenny => incoming.Any((a) => a.hasSenator);
        public bool iArt => incoming.Any((a) => a.hasArt);

        public bool isFocus => focus == cid;
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
                if (focus !=0)
                {
                    var cid = City.focus;
                    if (cid != viewHover && !selected.Contains(cid))
                    {
                        rv.Add(cid);
                    }
                }
                if (City.build != 0 && City.build != focus)
                {
                    var cid = City.build;
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
        public void SelectMe()
        {
            ProcessSelection(VirtualKeyModifiers.None,true);
            JSClient.ShowCity(cid, true);
        }
     
        public bool ProcessSelection(VirtualKeyModifiers mod,bool forceSelect=false)
        {
            bool rv = false;
            try
            {
                selected.EnterWriteLock();
                if (mod.IsShift() || forceSelect)
                {
                    rv = true; // this should also add "in between spots"
                    selected._hashSet.Add(cid);
                    SpotTab.SelectSilent(this, rv);
                }
                else if (mod.IsControl())
                {
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
                    SpotTab.SelectSilent(this, rv);
                }
                else
                {
                    // clear selection and select this
                    rv = true;
                    selected._hashSet.Clear();
                    selected._hashSet.Add(cid);
                    SpotTab.SelectOne(this);
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
            JSClient.ChangeCity(cid,false);

        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }

        public override string ToString()
        {
            return $"{{{cid},{cityName}, {xy},{player},{tsHome.ToString()}ts}}";
        }
        public void SetFocus( )
        {
            SetFocus(cid);
        }
        public static void SetFocus(int cid)
        {
            var changed = cid != focus;
            if(changed)
            {
                focus = cid;
                var spot = Spot.GetOrAdd(cid);
                App.DispatchOnUIThreadSneaky(() =>
                {
                    ShellPage.instance.focus.Content = spot.nameAndRemarks;
                    ShellPage.instance.coords.Text = cid.CidToString();
                }

                );
            }
            cid.BringCidIntoWorldView(true);
        }
        public void ReturnSlowClick()
        {
                Raiding.ReturnSlow(cid, true);
        }
        public async void ReturnAt(object sender, RoutedEventArgs e)
        {
            (var at,var okay) = await Views.DateTimePicker.ShowAsync("Return By:");
            if (!okay)
                return; // aborted
            
            await Raiding.ReturnAt(cid, at);
        }
        public async void ReturnAtBatch(object sender, RoutedEventArgs e)
        {
            (var at, var okay) = await Views.DateTimePicker.ShowAsync("Return By:");
            if (!okay)
                return; // aborted

            var cids = MainPage.GetContextCids(cid);
            foreach (var _cid in cids)
            {
                var __cid = _cid;
               await Raiding.ReturnAt(__cid, at);
            }
        }


        public async Task SuperRaid()
        {
            await Post.Send("overview/rcallall.php", "a=" + cid);
            await Post.SendEncrypted("includes/UrOA.php", "{\"a\":" + cid + ",\"c\":0,\"b\":2}", "Rx3x5DdAxxerx3");
        }

        public void ReturnFastClick()
        {
            if (App.IsKeyPressedShift())
            {
                SuperRaid();
            }
            else
            {
                Raiding.ReturnFast(cid, true);
            }
        }

        //int IKeyedItem.GetKey()
        //{
        //    return cid;
        //}
        //void IKeyedItem.Ctor(int i)
        //{
        //    cid = i;
        //}
        public static void JSCommand(string func, int cid)
        {
            JSClient.view.InvokeScriptAsync(func, new string[] { (cid % 65536).ToString(), (cid >> 16).ToString() });
        }
        public static void JSAttack(int cid)
        {
              JSCommand("spotAttack", cid);
        }
        public static void JSDefend(int cid)
        {
            JSCommand("spotDefend", cid);
        }
        public static void JSSendRes(int cid)
        {
            JSCommand("spotSendRes", cid);
        }
        public static void JSRaid(int cid)
        {
            JSCommand("spotRaid", cid);
        }
        public void ShowDistanceTo(int _cid)
        {
            var dist = cid.DistanceToCid(_cid);
            StringBuilder sb = new StringBuilder();
            sb.Append(dist.ToString("0.00"));
            for (int i = 1; i <  ttCount;++i)
            {
                var dt = TimeSpan.FromMinutes(dist * TTTravel(i));
                sb.Append($"\n{ttName[i]}: {dt.ToString()}");
            }
            var str = sb.ToString();
            App.CopyTextToClipboard(str);
            Note.Show(str);
        }
        public void ShowContextMenu(UIElement uie,Windows.Foundation.Point position)
        {
            var flyout = new MenuFlyout();

            if (this.isCityOrCastle)
            {
                // Look - its my city!
                if (this.isMine)
                {
                    // This one has multi select
                    int count = 1;
                    if (uie == MainPage.CityGrid || uie == DefendTab.instance.supportGrid)
                    {
                        count = MainPage.GetContextCidCount(cid);
                    }
                    if (count > 1)
                        {
                        App.AddItem(flyout, $"End Raids x{count} selected", MainPage.ReturnSlowClick, cid);
                        App.AddItem(flyout, $"Home Please x{count} selected", MainPage.ReturnFastClick, cid);
                        App.AddItem(flyout, $"Return At...x{count}", this.ReturnAtBatch);

                    }
                    else
                     {

                        App.AddItem(flyout, "End Raids", this.ReturnSlowClick);
                        App.AddItem(flyout, "Home Please", this.ReturnFastClick);
                        App.AddItem(flyout, "Return At...", this.ReturnAt);
                    }

                    App.AddItem(flyout, "Set Hub", (_, _) => CitySettings.SetCitySettings(cid) );
                    App.AddItem(flyout, "Attack to Clipboard", (_, _) =>
                        App.CopyTextToClipboard($"{cid.CidToString()} {Player.myName} vanqs real 200000\n{cid.CidToString()} {Player.myName} vanqs fake 3000\n"));
                    App.AddItem(flyout, "Rename", (_, _) => CitySettings.RenameDialog(cid));



                    }
                else
                {
                    App.AddItem(flyout, "Add as Real", (_, _) => AttackTab.AddTarget(cid,0));
                    App.AddItem(flyout, "Add as Fake (1)", (_, _) => AttackTab.AddTarget(cid, 1));
                    App.AddItem(flyout, "Add as Fake (2)", (_, _) => AttackTab.AddTarget(cid, 2));
                    App.AddItem(flyout, "Add as Fake (3)", (_, _) => AttackTab.AddTarget(cid, 3));
                    App.AddItem(flyout, "Add as Fake (4)", (_, _) => AttackTab.AddTarget(cid, 3));

                }
                if (cid != City.build)
                {
                    App.AddItem(flyout, "Set target hub", (_, _) => CitySettings.SetTargetHub(City.build, cid));
                    //if(Player.myName == "Avatar")
                    //    App.AddItem(flyout, "Set target hub I", (_, _) => CitySettings.SetOtherHubSettings(City.build, cid));
                }
                App.AddItem(flyout, "Attack", (_, _) => Spot.JSAttack(cid));
                App.AddItem(flyout, "Near Defence", DefendMe );
                if(incoming.Any())
                    App.AddItem(flyout, "Incoming", ShowIncoming);


                App.AddItem(flyout, "Send Defence", (_, _) => JSDefend(cid));
                App.AddItem(flyout, "Send Res", (_, _) => Spot.JSSendRes(cid));
                App.AddItem(flyout, "Distance", (_, _) => ShowDistanceTo(Spot.focus));
            }
            else if (this.isDungeon || this.isBoss)
            {
                App.AddItem(flyout, "Raid", (_, _) => Spot.JSRaid(cid));

            }
            else if( this.isEmpty && Discord.isValid)
            {
                App.AddItem(flyout, "Claim", this.DiscordClaim);

            }
            App.AddItem(flyout, "Select",  SelectMe );
            App.AddItem(flyout, "Coords to Chat", () => ChatTab.PasteToChatInput($"<coords>{cid.CidToString()}</coords>",true));


            flyout.XamlRoot = uie.XamlRoot;
            flyout.ShowAt(uie, position);
        }
        public void DefendMe()
        {
            DefendTab.defendant = this;
            var tab = DefendTab.instance;
            if (!tab.isActive)
            {
                TabPage.mainTabs.AddTab(tab, true);
            }
            else 
            {
                if (!tab.isVisible)
                    TabPage.Show(tab);
                else
                    tab.Refresh();
            }
        }
        public async void ShowIncoming()
        {
            var tab = DefenderPage.instance;
            if (!tab.isActive)
            {
                TabPage.mainTabs.AddTab(tab, true);
            }
            else
            {
                if (!tab.isVisible)
                    TabPage.Show(tab);
                else
                    tab.Refresh();
            }
            for (; ; )
            {
                await Task.Delay(1000);
                if (tab.defenderGrid.ItemsSource != null)
                    break;
            }
            App.DispatchOnUIThreadSneaky(() =>
            {
                tab.defenderGrid.SetCurrentItem(this,false);
            });

        }
        public async void DiscordClaim()
        {
            if(!Discord.isValid)
            {
                Log("Invalid");
                return;
            }
            try
            {
                Note.Show($"Registering claim on {xy}");
                var client = JSClient.genericClient;


                var message = new Discord.Message() { username = "Cord Claim", content = $"{xy} claimed by {Player.myName}", avatar_url = "" };

                var content = new HttpStringContent(
                          JsonSerializer.Serialize(message), Windows.Storage.Streams.UnicodeEncoding.Utf8,
                           "application/json");

                var result = await client.PostAsync(Discord.discordHook, content);
                result.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Log(ex);
            }


        }

    }
}
