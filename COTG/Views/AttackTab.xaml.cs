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
using System.Text.RegularExpressions;
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
            SortAttacks(null,null);
            var used = new HashSet<int>(readable.attacks.Length);
            StringBuilder sb = new StringBuilder();
            foreach (var a in readable.attacks)
            {
                if (!used.Add(a.cid))
                    continue;
                if (!a.targets.Any())
                    continue;

                var atk = new AttackSenderScript() { type = new List<int>(), x = new List<int>(), y = new List<int>() };
                // group all attacks for this player
                foreach (var a1 in a.targets)
                {
                    var t = Spot.GetOrAdd(a1);
                 
                    atk.type.Add(t.attackFake ? 0 : 1);

                    var xy = a1.CidToWorld();
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

            if (atk.targets.Any() )
            {
                var id = atk.targets.FindIndex(Spot.focus)+1;
                if (id >= atk.targets.Length)
                    id =0;
                var cid = atk.targets[id];
                var spot = Spot.GetOrAdd(cid);
                Spot.ProcessCoordClick(cid, false,  App.keyModifiers);
                foreach (var t in targets)
                {
                    if (t.cid == cid)
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

        public class ReadableAttacks
        {
            public Attack[] attacks { get; set; } 
            public TargetPersist[] targets { get; set; } 
     
        }
        // read only cache to enable threads to read the attacks white another thread is writing
        public static ReadableAttacks readable = new ReadableAttacks() { attacks= Array.Empty<Attack>(),targets  = Array.Empty<TargetPersist>() };

        internal static void WritebackAttacks()
        {
            Assert(loaded);
            int targetCount = AttackTab.targets.Count;
            var targets = new TargetPersist[targetCount];
            for (int i = 0; i < targetCount; ++i)
            {
                var t = AttackTab.targets[i];
                targets[i].attackCluster = t.attackCluster;
                targets[i].cid = t.cid;
                targets[i].fake = t.attackFake;
            }
            readable.attacks = attacks.ToArray(); // This is not completely atomic
            readable.targets = targets;
            UpdateStats();
        }

        private static void UpdateStats()
        {
            App.DispatchOnUIThreadSneaky(() =>
            {
                instance.attackCount.Text=$"Total: {readable.attacks.Length}";
                instance.SE.Text=$"SE: {readable.attacks.Count((a) => a.troopType==ttScorpion)}";
                instance.vanqs.Text=$"Vanqs: {readable.attacks.Count((a) => a.troopType==ttVanquisher)}";
                instance.sorcs.Text=$"Sorc: {readable.attacks.Count((a) => a.troopType==ttSorcerer)}";
                instance.horses.Text=$"Horse: {readable.attacks.Count((a) => a.troopType==ttHorseman)}";
            });
        }

        internal static void SaveAttacks()
        {
            if (loaded)
            {
                folder.SaveAsync("attacks", readable);
            }        
        }

        public async Task TouchLists()
        {
            // First init
            //  AttackTab.attacks.Clear();

            if (!loaded)
            {
                loaded = true;
                using var work = new ShellPage.WorkScope("load attacks");
                readable = await folder.ReadAsync<ReadableAttacks>("attacks");
                // App.DispatchOnUIThreadSneaky(() =>
                //  {
                attacks.Set(readable.attacks);
                foreach(var att in readable.attacks)
                {
                    await Spot.GetOrAdd(att.cid).Classify();
                }
                var spots = new List<Spot>();
                if (readable.targets != null)
                {
                    foreach (var target in readable.targets)
                    {
                        var t = Spot.GetOrAdd(target.cid);
                        t.attackCluster = target.attackCluster;
                        t.classification =  (await t.Classify()).classification;
                        spots.Add(t);

                    }
                }

                targets.Set(spots);
                //});
                UpdateStats();
            }

        }

        public async override void VisibilityChanged(bool visible)
        {
            if (visible)
            {
                await TouchLists();


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
                // take any one
                if (t.typeT == Attack.Type.se && t.targets.Contains(spot.cid))
                {
                    attackGrid.ScrollIntoView(t, null);
                    break;
                }
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
            WritebackAttacks();
            CleanTargets();
        }
        public async void AddAttacksFromSheets(object sender, RoutedEventArgs e)
        {
            try
            {
                await TouchLists();
                int duplicates = 0;
                int prior = attacks.Count;
                var text = await App.GetClipboardText();
                var strs = text.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                Assert((strs.Length %3)==0);
                int count = strs.Length;
                for (int i = 0; i<count; i+=3)
                {
                    var cid = strs[i+2].FromCoordinate();
                    var atk = attacks.Find((a) => a.cid == cid);
                    var isNew = (atk == null);
                    if (isNew)
                    {
                        atk = new Attack() { cid = cid };
                    }
                    else
                    {
                        ++duplicates;
                    }

                    atk.troopType =  strs[i].ToLower() switch
                    { "vanq" => ttVanquisher,
                        "sorc" => ttSorcerer,
                        "horses" => ttHorseman,
                        "siege engines" => ttScorpion,
                        "druids" => ttDruid, _ => throw new ArgumentException() };

                    atk.typeT = atk.troopType == ttScorpion ? Attack.Type.se : strs[i+1]=="Yes" ? Attack.Type.senator : Attack.Type.assault;
                    atk.fake = false;
                    
                    atk.player = Spot.GetOrAdd(cid).player;
                    if(isNew)
                        attacks.Add(atk);
                }
                Note.Show($"Added {attacks.Count-prior}, updated {duplicates}");
                WritebackAttacks();
                SaveAttacks();
            }
            catch (Exception ex)
            {
                Log(ex);
                Note.Show("Invalid format");

            }


        }

        public async void AddAttacksFromClipboard(object sender, RoutedEventArgs e)
        {
            try
            {
                using var work = new ShellPage.WorkScope("Adding attacks...");
                await TouchLists();
                var text = await Clipboard.GetContent().GetTextAsync();
                if (text.IsNullOrEmpty())
                {
                    Note.Show("No clipboard text");
                    return;
                }
                int duplicates=0;
                var reals = 0;
                var fakes = 0;
           //     text = text.Replace("\r", "");
            //    var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (Match m in AUtil.coordsRegex.Matches(text))
                {
                   
                    try
                    {
                      //  var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var cid = m.Value.FromCoordinate();
                        var atk = attacks.Find((a) => a.cid == cid);
                        var isNew = (atk == null);
                        if (isNew)
                        {
                            atk = new Attack() { cid = cid };
                        }
                        else
                        {
                            ++duplicates;
                        }
                        var spot = Spot.GetOrAdd(cid);
                        var cl = await spot.Classify();
                      //  string s = $"{cid.CidToString()} {Player.myName} {cl.} {(cl.academies == 1 ? 2 : 0)} {tsTotal}\n";
                        atk.troopType = cl.classification switch {
                            Spot.Classification.sorcs => ttSorcerer,
                            Spot.Classification.druids => ttDruid,
                            Spot.Classification.academy => ttPraetor,
                            Spot.Classification.horses => ttHorseman,
                            Spot.Classification.arbs => ttHorseman,
                            Spot.Classification.se => ttScorpion,
                            _ => ttVanquisher
                        };
                        if (atk.troopType == ttScorpion)
                            atk.typeT = Attack.Type.se;
                        else if (cl.academies > 0)
                            atk.typeT = Attack.Type.senator;
                        else atk.typeT = Attack.Type.assault;

                        //atk.type = (int)Attack.Type.senator;

                        //atk.fake = parts[3].ToLowerInvariant() == "fake";
                        //var tt = parts[2].ToLowerInvariant();
                        //if (tt.StartsWith("sorc"))
                        //    atk.troopType = ttSorcerer;
                        //else if (tt.StartsWith("horse"))
                        //    atk.troopType = ttHorseman;
                        //else if (tt.StartsWith("van"))
                        //    atk.troopType = ttVanquisher;
                        //else if (tt.StartsWith("druid"))
                        //    atk.troopType = ttDruid;
                        //else atk.troopType = ttGuard;

                        //atk.
                        //if (int.TryParse(parts[2], out var ts))
                        //{
                        //    atk.ts = ts;
                        //}

                        atk.player = spot.player;
                        if (atk.fake)
                            ++fakes;
                        else
                            ++reals;
                        if(isNew)
                            attacks.Add(atk);
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                    }
                }
                Note.Show($"{reals-duplicates} added, {duplicates} updated");
                WritebackAttacks();
                SaveAttacks();
            }
            catch (Exception ex)
            {
                Log(ex);
                Note.Show("Invalid clipboard text");
            }



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
            WritebackAttacks();
            SaveAttacks();
            attacks.NotifyReset();
        }


        internal static async void AddTarget(int cid, bool fake)
        {
            await instance.TouchLists();
            var spot = Spot.GetOrAdd(cid);
            spot.attackFake = fake;
            await spot.Classify();
            if (targets.Contains(spot))
            {
                Note.Show($"Target is already present, set to {(fake ? "fake" : "real")} {spot.attackCluster}");

            }
            else
            {
                targets.Add(spot);
                Note.Show($"Added {spot.nameAndRemarks}, {targets.Count} targets");
            }
            WritebackAttacks();
            SaveAttacks();

        }

        private static void CleanTargets()
        {
            var set = new HashSet<int>(targets.Count);
            foreach (var t in targets)
            {
                t.attackAssignment = null;
                set.Add(t.cid);
            }
            foreach (var a in attacks)
            {
                a.targets = a.targets.Where((cid) => set.Contains(cid)).ToArray();
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
                if (a.target == 0)
                {
                    if (a.fake)
                        ++unusedFakes;
                    else
                        ++unusedReals;
                }
            }

            bad = $"{bad} Assigned {reals} reals and {fakes} fakes\nUnused: reals {unusedReals}, fakes {unusedFakes}";

            Note.Show(bad);
            attacks.NotifyReset();
            targets.NotifyReset();
            StringBuilder sb = new StringBuilder();
            foreach (var a in attacks)
            {
                if (a.target!=0)
                {
                    sb.Append($"{a.player} <coords>{a.target.CidToString()}</coords> {AttackType.types[a.type].name} {(a.fake ? "Fake" : "Real")} at {time.FormatDefault()}\n");
                }
            }
            App.CopyTextToClipboard(sb.ToString());
            SaveAttacks();
        }

        private void TargetRemove_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var i = sender as FrameworkElement;

            var spot = i.DataContext as Spot;
            targets.Remove(spot);
            CleanTargets();
            WritebackAttacks();
            SaveAttacks();

        }
        private void AttackRemove_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var i = sender as FrameworkElement;

            var spot = i.DataContext as Attack;
            attacks.Remove(spot);
            WritebackAttacks();
            SaveAttacks();
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
