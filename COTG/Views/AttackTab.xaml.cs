using COTG.Game;
using COTG.Helpers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static COTG.Game.Enum;
using static COTG.Debug;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace COTG.Views
{
    
    public sealed partial class AttackTab : UserTab, INotifyPropertyChanged
    {
        public static DateTimeOffset time { get; set; }
        public static StorageFolder folder => ApplicationData.Current.LocalFolder;
        public static AttackTab instance;
        public static bool IsVisible() => instance.isVisible;

        public static DumbCollection<Attack> attacks = new DumbCollection<Attack>();
        public static DumbCollection<Spot> targets = new DumbCollection<Spot>();
        public AttackTab()
        {
            Assert(instance == null);
            instance = this;
            this.InitializeComponent();

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void AttackSender_Tapped(object sender, RoutedEventArgs e)
        {
            var used = new HashSet<int>(attacks.Count);
            StringBuilder sb = new StringBuilder();
            foreach (var a in attacks)
            {
                if (!used.Add(a.cid))
                    continue;
                var atk = new AttackSenderScript() { type = new List<int>(), x = new List<int>(), y = new List<int>() };
                foreach (var a1 in attacks)
                {
                    if (a1.cid != a.cid)
                        continue;
                    atk.type.Add(a1.fake ? 0 : 1);
                    
                    var xy = a1.target.CidToWorld();
                    atk.x.Add(xy.x);
                    atk.y.Add(xy.y);
                }
                atk.time = new string[] { time.Hour.ToString(), time.Minute.ToString(), time.Second.ToString(), time.ToString("MM/dd/yyyy") };
                sb.Append($"\n{a.player} <coords>{a.cid.CidToString()}</coords>\n");

                sb.Append(System.Text.Json.JsonSerializer.Serialize(atk));
             
            }
            App.CopyTextToClipboard(sb.ToString());
            Note.Show("Copied Attack sender scripts to clipboard");
        }

        private void AttackTargetCoord_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var i = sender as FrameworkElement;

            var atk = i.DataContext as Attack;
            if (atk.target != 0)
            {
              
                Spot.ProcessCoordClick(atk.target, false,App.keyModifiers);
                foreach (var t in targets)
                {
                    if (t.cid == atk.target)
                        targetGrid.ScrollIntoView(t, null);
                }
            }
        }
        private void AttackSourceCoord_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var i = sender as FrameworkElement;

            var spot = i.DataContext as Attack;
            if (spot.cid != 0)
            {

                Spot.ProcessCoordClick(spot.cid, false, App.keyModifiers);
            }
        }
        static bool loaded = false;

        struct AttackPersister
        {
           public List<Attack> attacks { get; set; }
            public TargetPersist[] targets { get; set; }
        }

        internal static void SaveAttacks()
        {
            if (!AttackTab.targets.IsNullOrEmpty() || !AttackTab.attacks.IsNullOrEmpty())
            {
                var persist = new AttackPersister() { attacks = attacks };
                
                    int targetCount = AttackTab.targets.Count;
                    var targets = new TargetPersist[targetCount];
                    for (int i = 0; i < targetCount; ++i)
                    {
                        targets[i].attackCluster = AttackTab.targets[i].attackCluster;
                        targets[i].cid = AttackTab.targets[i].cid;
                        targets[i].classification = (byte)AttackTab.targets[i].classification;
                    }
                  persist.targets = targets;
                 folder.SaveAsync("attacks",persist);
                
            }
        }

        public async Task TouchLists()
        {
            // First init
            //  AttackTab.attacks.Clear();

            if (!loaded)
            {
                loaded = true;
                var persist = await folder.ReadAsync<AttackPersister>("attacks" );
               // App.DispatchOnUIThreadSneaky(() =>
              //  {
                    attacks.Set(persist.attacks);
                  
                    var spots = new List<Spot>();
                    if (persist.targets != null)
                    {
                        foreach (var target in persist.targets)
                        {
                            var t = Spot.GetOrAdd(target.cid);
                            t.attackCluster = target.attackCluster;
                            t.classification = (Spot.Classification)target.classification;
                            spots.Add(t);

                        }
                    }
                    
                    targets.Set(spots);
                //});
            }

        }

        public async override void VisibilityChanged(bool visible)
        {
            if (visible)
            {
                TouchLists();


            }
            else
            {

            }
        }

        private void TargetCoord_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var i = sender as FrameworkElement;

            var spot = i.DataContext as Spot;
            Spot.ProcessCoordClick(spot.cid, false, App.keyModifiers);
            foreach (var t in attacks)
            {
                if (t.target == spot.cid)
                    attackGrid.ScrollIntoView(t, null);
            }

        }


        private void RemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            {
                var temp = new List<Attack>();
                foreach (var sel in attackGrid.SelectedItems)
                {
                    temp.Add(sel as Attack);
                }
                foreach (var sel in temp)
                {
                    var id = attacks.IndexOf(sel as Attack);
                    if (id >= 0)
                    {
                        attacks.RemoveAt(id);
                    }

                }
            }
            {
                var temp = new List<Spot>();
                foreach (var sel in targetGrid.SelectedItems)
                {
                    temp.Add(sel as Spot);
                }
                foreach (var sel in temp)
                {
                    var id = targets.IndexOf(sel);
                    if (id >= 0)
                    {
                        targets.RemoveAt(id);
                    }

                }
            }
            CleanTargets();
        }

        public async void AddAttacksFromClipboard(object sender, RoutedEventArgs e)
        {
            await TouchLists();
            var text = await Clipboard.GetContent().GetTextAsync();
            if(text.IsNullOrEmpty())
            {
                Note.Show("No clipboard text");
                return;
            }
            var reals = 0;
            var fakes = 0;
            text = text.Replace("\r", "");
            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                try
                {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    var atk = new Attack();
                    atk.type = (int)Attack.Type.senator;
                    atk.fake = parts[3].ToLowerInvariant() == "fake";
                    var tt = parts[2].ToLowerInvariant();
                    if (tt.StartsWith("sorc"))
                        atk.troopType = ttSorcerer;
                    else if (tt.StartsWith("horse"))
                        atk.troopType = ttHorseman;
                    else if (tt.StartsWith("van"))
                        atk.troopType = ttVanquisher;
                    else if (tt.StartsWith("druid"))
                        atk.troopType = ttDruid;
                    else atk.troopType = ttGuard;            

                    atk.cid = parts[0].FromCoordinate();
                    if (int.TryParse(parts[2], out var ts))
                    {
                        atk.ts = ts;
                    }
                    
                    atk.player = Spot.GetOrAdd(atk.cid).player;
                    if (atk.fake)
                        ++fakes;
                    else
                        ++reals;
                    attacks.Add(atk);
                }
                catch (Exception ex)
                {
                    Log(ex);
                }
            }
            Note.Show($"{reals} reals, {fakes} fakes added");
            
        }
        private void SortAttacks(object sender, RoutedEventArgs e)
        {

            attacks.Sort((a, b) =>
            {
                var c = a.player.CompareTo(b.player);
                if (c != 0)
                    return c;
                return b.fake.CompareTo(a.fake);
            }
            );
            attacks.NotifyReset();
        }
       

        internal static async void AddTarget(int cid, byte group)
        {
            await instance.TouchLists();
            var spot = Spot.GetOrAdd(cid);
            if (targets.Contains(spot))
            {
                Note.Show($"Target is already present in {spot.attackCluster}");
                return;
            }
            spot.attackCluster = group;
            await spot.Classify();
            targets.Add(spot);
            Note.Show($"Added {spot.nameAndRemarks}, {targets.Count} targets");
     }

        private  static void CleanTargets()
        {
            var set = new HashSet<int>(targets.Count);
            foreach(var t in targets)
            {
                t.attackAssignment = null;
                set.Add(t.cid);
            }
            foreach(var a in attacks)
            {
                if (a.target != 0)
                {
                    if (!set.Contains(a.target))
                    {
                        a.target = 0;
                    }
                    else
                    {
                        Spot.GetOrAdd(a.target).attackAssignment = a;
                    }
                }
            }
        }
        private void AssignAttacks_Click(object sender, RoutedEventArgs e)
        {
            string bad = string.Empty;
            var reals = 0;
            var fakes = 0;
            CleanTargets();

            foreach (var a in attacks)
            {
                if (a.target != 0) // not already assigned
                {
                    if (!a.fake)
                        ++reals;
                    else
                        ++fakes;
                    continue;
                }
                    Spot best = null;
                    float bestScore = float.MaxValue;
                    foreach (var t in targets)
                    {
                        if (a.fake == (t.attackCluster == 0)) // real or fake match
                            continue;
                        if (t.attackAssignment != null)
                            continue; // already taken
                        var score = a.cid.DistanceToCid(t.cid);
                    if (!a.fake)
                    {
                        switch (a.troopType)
                        {
                            case ttSorcerer:
                            case ttDruid:
                                {
                                    switch (t.classification)
                                    {
                                        case Spot.Classification.unknown:
                                            break;
                                        case Spot.Classification.vanqs:
                                        case Spot.Classification.rt:
                                            score -= 3;
                                            break;
                                        case Spot.Classification.sorcs:
                                        case Spot.Classification.druids:
                                            score += 2;
                                            break;
                                        case Spot.Classification.academy:
                                            score += 8;
                                            break;
                                        case Spot.Classification.horses:
                                        case Spot.Classification.arbs:
                                            score -= 4;
                                            break;
                                        case Spot.Classification.se:
                                            break;
                                        case Spot.Classification.hub:
                                            break;
                                        case Spot.Classification.navy:
                                            break;
                                        case Spot.Classification.misc:
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                }
                            case ttVanquisher:
                                {
                                    switch (t.classification)
                                    {
                                        case Spot.Classification.unknown:
                                            break;
                                        case Spot.Classification.vanqs:
                                        case Spot.Classification.rt:

                                            score += 4;
                                            break;
                                        case Spot.Classification.sorcs:
                                        case Spot.Classification.druids:
                                            score -= 2;
                                            break;
                                        case Spot.Classification.academy:
                                            score -= 4;
                                            break;
                                        case Spot.Classification.horses:
                                        case Spot.Classification.arbs:

                                            score -= 3;
                                            break;
                                        case Spot.Classification.se:
                                            break;
                                        case Spot.Classification.hub:
                                            break;
                                        case Spot.Classification.navy:
                                            break;
                                        case Spot.Classification.misc:
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                }
                            case ttHorseman:
                                {
                                    switch (t.classification)
                                    {
                                        case Spot.Classification.unknown:
                                            break;
                                        case Spot.Classification.vanqs:
                                        case Spot.Classification.rt:

                                            score += 2;
                                            break;
                                        case Spot.Classification.sorcs:
                                        case Spot.Classification.druids:
                                            score -= 2;
                                            break;
                                        case Spot.Classification.academy:
                                            score -= 6;
                                            break;
                                        case Spot.Classification.horses:
                                        case Spot.Classification.arbs:

                                            score += 7;
                                            break;
                                        case Spot.Classification.se:
                                            break;
                                        case Spot.Classification.hub:
                                            break;
                                        case Spot.Classification.navy:
                                            break;
                                        case Spot.Classification.misc:
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                }
                        }
                    }
                    if (score < bestScore)
                        {
                            bestScore = score;
                            best = t;

                        }

                    }
                if (best == null)
                {
                    bad = $"{bad}No target for {a.xy} isReal: {!a.fake}\n";

                }
                else
                {
                    a.target = best.cid;
                    best.attackAssignment = a;
                    if (!a.fake)
                        ++reals;
                    else
                        ++fakes;

                }
            }
            var unusedReals = 0;
            var unusedFakes = 0;
            foreach (var a in attacks)
            {
                if(a.target == 0)
                {
                    if (a.fake)
                        ++unusedFakes;
                    else
                        ++unusedReals;
                }
            }

            bad = $"{bad}Assigned {reals} reals and {fakes} fakes\nUnused: reals {unusedReals}, fakes {unusedFakes}";

            Note.Show(bad);
            attacks.NotifyReset();
            targets.NotifyReset();
            StringBuilder sb = new StringBuilder();
            foreach(var a in attacks)
            {
                if(a.target!=0)
                {
                    sb.Append($"{a.player} <coords>{a.target.CidToString()}</coords> {AttackType.types[a.type].name} {(a.fake ? "Fake" : "Real" )} at {time.FormatDefault()}\n");
                }
            }
            App.CopyTextToClipboard(sb.ToString());
        }

        private void TargetRemove_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var i = sender as FrameworkElement;

            var spot = i.DataContext as Spot;
            targets.Remove(spot);
            CleanTargets();

        }
        private void AttackRemove_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var i = sender as FrameworkElement;

            var spot = i.DataContext as Attack;
            attacks.Remove(spot);
        }

        private async void SendAtTapped(object sender, PointerRoutedEventArgs e)
        {
            e.KeyModifiers.UpdateKeyModifiers();
            e.Handled = true;
            (var dateTime, var okay) = await DateTimePicker.ShowAsync("Send At");
            if (okay)
            {
                time = dateTime;
                OnPropertyChanged(nameof(time));
            }
        }

        private void ClearTargets_Click(object sender, RoutedEventArgs e)
        {
            {
                var temp = new List<Attack>();
                foreach (var sel in attackGrid.SelectedItems)
                {
                    (sel as Attack).target=0;
                }
            }
            attacks.NotifyReset();
        }
    }
}
