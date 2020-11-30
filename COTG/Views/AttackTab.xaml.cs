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
using System.Numerics;
using Microsoft.Toolkit.Diagnostics;
using System.Diagnostics.Contracts;
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
            SortAttacks(null, null);


            var clusterCount = attackClusters.Length;
            var used = new HashSet<int>(readable.attacks.Length);

            var scripts = new string[clusterCount];
            int clusterId = 0;
            foreach (var cluster in attackClusters)
            {
                var atk = new AttackSenderScript() { type = new List<int>(), x = new List<int>(), y = new List<int>() };
                // group all attacks for this player
                foreach (var a1 in cluster.targets.OrderBy((a) => Spot.GetOrAdd(a).attackFake))
                {
                    var t = Spot.GetOrAdd(a1);

                    atk.type.Add(t.attackFake ? 0 : 1);

                    var xy = a1.CidToWorld();
                    atk.x.Add(xy.x);
                    atk.y.Add(xy.y);
                }
                atk.time = new string[] { time.Hour.ToString("00"), time.Minute.ToString("00"), time.Second.ToString("00"), time.ToString("MM/dd/yyyy") };
                scripts[clusterId++] = System.Text.Json.JsonSerializer.Serialize(atk);
            }

            StringBuilder sb = new StringBuilder();
            var players = new List<int>();

            foreach (var a in attacks)
            {
                var player = Spot.GetOrAdd(a.cid).pid;
                if (players.Contains(player))
                    continue;
                players.Add(player);
            }
            foreach (var player in players)
            {
                sb.Append($"\n\n{Player.IdToName(player)}");
                var lastCluster = -1;
                foreach (var a in attacks)
                {
                    if (Spot.GetOrAdd(a.cid).pid != player || a.attackCluster==255)
                        continue;
                    if (lastCluster != a.attackCluster)
                    {
                        lastCluster=a.attackCluster;
                        sb.Append($"\n\n{scripts[lastCluster]}");
                    }
                    sb.Append('\n');
                    sb.Append(a.cid.CidToCoords());
                }


            }
            App.CopyTextToClipboard(sb.ToString());
            Note.Show("Copied Attack sender scripts to clipboard");
        }

        private void AttackTargetCoord_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var i = sender as FrameworkElement;

            var atk = i.DataContext as Attack;

            //if (atk.targets.Any() )
            //{
            //    var id = atk.targets.FindIndex(Spot.focus)+1;
            //    if (id >= atk.targets.Length)
            //        id =0;
            //    var cid = atk.targets[id];
            //    var spot = Spot.GetOrAdd(cid);
            //    Spot.ProcessCoordClick(cid, false,  App.keyModifiers);
            //    foreach (var t in targets)
            //    {
            //        if (t.cid == cid)
            //            targetGrid.ScrollIntoView(t, null);
            //    }
            //}
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
        public static ReadableAttacks readable = new ReadableAttacks() { attacks= Array.Empty<Attack>(), targets  = Array.Empty<TargetPersist>() };

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
            foreach (var a in attacks)
            {
                Spot.GetOrAdd(a.cid).attackCluster = a.attackCluster;
            }
            readable.attacks = attacks.ToArray(); // This is not completely atomic
            readable.targets = targets;
            UpdateStats();
            BuildAttackClusters();
        }
        public class AttackCluster
        {
            public HashSet<int> attacks;// first is SE or siege
            public HashSet<int> targets;// first is real
            public Vector2 topLeft;
            public Vector2 bottomRight;

            public int real => (targets.FirstOrDefault((a) => !Spot.GetOrAdd(a).attackFake));


        }
        public static AttackCluster[] attackClusters;
        public static void BuildAttackClusters()
        {
            var reals = targets.Where((a) => !a.attackFake).ToArray();
            var clusterCount = reals.Length;
            var _attackClusters = new AttackCluster[clusterCount];
            for (int i = 0; i<clusterCount; ++i)
            {
                var ac = new AttackCluster(); ;
                _attackClusters[i] = ac;
                ac.targets = new HashSet<int>(targets.Where((a) => a.attackCluster == (byte)i).Select((a) => a.cid));
                ac.attacks = new HashSet<int>(attacks.Where((a) => a.attackCluster == (byte)i).Select((a) => a.cid));
                if (ac.targets.Any())
                {
                    ac.topLeft.X = ac.targets.Select(a => a.ToWorldC().X).Min() - 0.5f;
                    ac.topLeft.Y = ac.targets.Select(a => a.ToWorldC().Y).Min() - 0.5f;
                    ac.bottomRight.X = ac.targets.Select(a => a.ToWorldC().X).Max() + 0.5f;
                    ac.bottomRight.Y = ac.targets.Select(a => a.ToWorldC().Y).Max() + 0.5f;
                }
            }
            attackClusters = _attackClusters;
        }
        private static void UpdateStats()
        {
            App.DispatchOnUIThreadSneaky(() =>
            {
                instance.attackCount.Text=$"Attacks: {readable.attacks.Length}";
                var seCount = readable.attacks.Count((a) => a.troopType==ttScorpion);
                instance.SE.Text=$"SE: {seCount}";
                instance.vanqs.Text=$"Vanqs: {readable.attacks.Count((a) => a.troopType==ttVanquisher)}";
                instance.sorcs.Text=$"Sorc: {readable.attacks.Count((a) => a.troopType==ttSorcerer)}";
                instance.horses.Text=$"Horse: {readable.attacks.Count((a) => a.troopType==ttHorseman)}";
                var fakes = readable.targets.Count(a => a.fake);
                var reals = (readable.targets.Length-fakes);
                instance.fakeCount.Text=$"Fakes: {fakes/(float)reals.Max(1):0.00}";
                instance.realCount.Text = $"Reals: {reals}";
                instance.sePerTarget.Text = $"SE/Target: {seCount/(float)reals.Max(1):0.00}";
                instance.attacksPerTarget.Text = $"Attacks/Target: {readable.attacks.Length/(float)reals.Max(1):0.00}";
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
                readable = await folder.ReadAsync<ReadableAttacks>("attacks", readable);
                // App.DispatchOnUIThreadSneaky(() =>
                //  {
                attacks.Set(readable.attacks);
                foreach (var att in readable.attacks)
                {
                    var spot = Spot.GetOrAdd(att.cid);
                    spot.attackCluster=att.attackCluster;
                    await spot.ClassifyIfNeeded();

                }
                var spots = new List<Spot>();
                if (readable.targets != null)
                {
                    foreach (var target in readable.targets)
                    {
                        var t = Spot.GetOrAdd(target.cid);
                        t.attackCluster = target.attackCluster;
                        t.classification =  await t.ClassifyIfNeeded();
                        t.attackFake = target.fake;
                        spots.Add(t);

                    }
                }

                targets.Set(spots);
                //});
                UpdateStats();
                BuildAttackClusters();
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
            //foreach (var t in attacks)
            //{
            //    // take any one
            //    if (t.typeT == Attack.Type.se && t.targets.Contains(spot.cid))
            //    {
            //        attackGrid.ScrollIntoView(t, null);
            //        break;
            //    }
            //}

        }


        private async void RemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            var removeAttacks = new CheckBox() { Content = $"{attackGrid.SelectedItems.Count} attacks", IsChecked=true };
            var removeTargets = new CheckBox() { Content = $"{targetGrid.SelectedItems.Count} targets", IsChecked=true };
            var panel = new StackPanel();

            var msg = new ContentDialog()
            {
                Title="Remove Selected",
                Content=panel,
                IsPrimaryButtonEnabled=true,
                PrimaryButtonText="Remove",
                CloseButtonText="Cancel"


            };
            msg.CopyXamlRoomFrom(sender as UIElement);
            panel.Children.Add(removeAttacks);
            panel.Children.Add(removeTargets);

            var result = await msg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (removeAttacks.IsChecked.GetValueOrDefault())
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
                if (removeTargets.IsChecked.GetValueOrDefault())
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
                    {
                        "vanq" => ttVanquisher,
                        "sorc" => ttSorcerer,
                        "horses" => ttHorseman,
                        "siege engines" => ttScorpion,
                        "druids" => ttDruid,
                        _ => throw new ArgumentException()
                    };

                    atk.typeT = atk.troopType == ttScorpion ? Attack.Type.se : strs[i+1]=="Yes" ? Attack.Type.senator : Attack.Type.assault;
                    atk.fake = false;

                    atk.player = Spot.GetOrAdd(cid).player;
                    if (isNew)
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
                var text = await Clipboard.GetContent().GetTextAsync();

                if (text.IsNullOrEmpty())
                {
                    Note.Show("No clipboard text");
                    return;
                }
                AddAttacksFromString(text);

            }
            catch (Exception ex)
            {
                Log(ex);
                Note.Show("Invalid clipboard text");
            }
        }
        public async void AddTargetsFromClipboard(object sender, RoutedEventArgs e)
        {
                using var work = new ShellPage.WorkScope("Adding attacks...");
                await TouchLists();

                int duplicates = 0;
                var reals = 0;
                //     text = text.Replace("\r", "");
                var text = await Clipboard.GetContent().GetTextAsync();

                //    var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (Match m in AUtil.coordsRegex.Matches(text))
                {

                    try
                    {
                        if (m.Value.EndsWith(':')||m.Value.StartsWith(':'))
                            continue;
                        //  var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var cid = m.Value.FromCoordinate();

                        var spot = Spot.GetOrAdd(cid);
                    if(targets.Contains(spot))
                    {
                        ++duplicates;
                        continue;
                    }
                    ++reals;
                    targets.Add(spot);


                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                    }
                }
                Note.Show($"{reals} added, {duplicates} updated");
                WritebackAttacks();
                SaveAttacks();
            
        }


        public async void AddAttacksFromString(string text)
        {
            try
            {
                using var work = new ShellPage.WorkScope("Adding attacks...");
                await TouchLists();

                int duplicates = 0;
                var reals = 0;
                var fakes = 0;
                //     text = text.Replace("\r", "");
                //    var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (Match m in AUtil.coordsRegex.Matches(text))
                {

                    try
                    {
                        if (m.Value.EndsWith(':')||m.Value.StartsWith(':'))
                            continue;
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
                        atk.troopType = cl.classification switch
                        {
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
                        if (isNew)
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
                return b.attackCluster.CompareTo(a.attackCluster);
            }
            );
            targets.Sort((a, b) =>
            {
                var c = a.attackCluster.CompareTo(b.attackCluster);
                if (c!=0)
                    return c;
                return a.attackFake.CompareTo(b.attackFake);
            });

            attacks.NotifyReset();
            targets.NotifyReset();

            WritebackAttacks();
            SaveAttacks();
            attacks.NotifyReset();
        }


        internal static async void AddTarget(int cid, bool fake)
        {
            await instance.TouchLists();
            var spot = Spot.GetOrAdd(cid);
            spot.attackFake = fake;
            await spot.ClassifyIfNeeded();
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
            // var set = new HashSet<int>(targets.Count);
            foreach (var t in targets)
            {

                t.attackCluster=255;
                // set.Add(t.cid);
            }
            foreach (var a in attacks)
            {
                a.attackCluster = 255;
                Spot.GetOrAdd(a.cid).attackCluster=255;
            }

        }

        private void AssignAttacks_Click(object sender, RoutedEventArgs e)
        {
            // first create clusters
            foreach (var f in targets)
            {
                f.attackCluster=255;
            }
            foreach (var a in attacks)
            {
                a.attackCluster=255;
                Spot.GetOrAdd(a.cid).attackCluster=255;
            }
            var reals = targets.Where((a) => !a.attackFake).ToArray();
            if (!reals.Any())
            {
                Note.Show("No reals");
                return;
            }
            var maxTravelHours = (float)this.maxTravelHours.Value;
            var desiredFakes = (int)this.desiredFakes.Value;
            var minAssaults = (int)this.minAssaults.Value;
            var clusterCount = reals.Length;
            var fakes = targets.Where((a) => a.attackFake).ToArray();
           
            var fakeCount = (fakes.Length / clusterCount).Min(desiredFakes); // rounds down

           

            var targetClusters = new int[clusterCount][];
            var fakesPerCluster = new List<Spot>[clusterCount];
            {
                byte cluster = 0;
                // first pass, select N closest fakes for each real
                foreach (var real in reals)
                {
                    real.attackCluster=cluster;
                    var c = real.cid.ToWorldC();
                    Span2 span = new Span2(c);

                    for (int i = 0; i<fakeCount; ++i)
                    {
                        var bestDist = float.MaxValue;
                        Spot bestFake = null;
                        foreach (var f in fakes)
                        {
                            if (f.attackCluster != 255)
                                continue;
                            var d = span.Distance2(f.cid.ToWorldC());
                            if (d < bestDist)
                            {
                                bestDist = d;
                                bestFake = f;
                            }
                        }
                        if (bestFake != null)
                        {
                            bestFake.attackCluster=real.attackCluster;// this marks it as used, it is overwritten later after swaps
                            span.Union(bestFake.cid.ToWorldC());
                            fakesPerCluster[cluster].Add(bestFake);
                        }
                    }
 
                    ++cluster;

                }
            }
            // optimize
            for (int iter = 0; iter<8; ++iter)
            {
                for (int c0 = 0; c0<clusterCount; ++c0)
                {
                    for (int c1 = c0+1; c1<clusterCount; ++c1)
                    {
                        var span0a = new Span2(fakesPerCluster[c0].GetEnumerator());
                        var span1a = new Span2(fakesPerCluster[c1].GetEnumerator());
                        Spot bestSwap0 = null;
                        Spot bestSwap1 = null;
                        float bestScore = 0;

                        foreach (var f0 in fakesPerCluster[c0])
                        {
                            var span0b = Span2.UnionWithout(fakesPerCluster[c0].GetEnumerator(), f0);

                            foreach (var f1 in fakesPerCluster[c1])
                            {
                                var span1b = Span2.UnionWithout(fakesPerCluster[c1].GetEnumerator(), f1);

                                var span0c = span0b + f1.cid.ToWorldC();
                                var span1c = span1b + f0.cid.ToWorldC();
                                // L4 distance
                                var scoreA = span0a.radius2.Squared() + span1a.radius2.Squared();
                                var scoreC = span0c.radius2.Squared() + span1c.radius2.Squared();
                                var delta = scoreC -scoreA;
                                if (delta < bestScore)
                                {
                                    bestSwap0 = f0;
                                    bestSwap1 = f1;
                                    bestScore = delta;
                                }
                                // swap if it improves things
                            }
                        }

                        if (bestSwap0 != null)
                        {
                            var good = fakesPerCluster[c0].Remove(bestSwap0);
                            Assert(good);
                            good = fakesPerCluster[c1].Remove(bestSwap1);
                            Assert(good);
                            fakesPerCluster[c0].Add(bestSwap1);
                            fakesPerCluster[c1].Add(bestSwap0);

                        }

                    }
                }
            }

            var spans = new Span2[clusterCount];
            // clusters are final
            // assign attack clusters and calculate spans
            for(int c0=0;c0<clusterCount;++c0)
            {
                foreach (var f0 in fakesPerCluster[c0])
                {
                    f0.attackCluster = (byte)c0;
                }
                spans[c0] = new Span2(fakesPerCluster[c0].GetEnumerator());
            }

            // Assign attacks to clusters
            var attackCount = attacks.Count;
            var seCount = attacks.Count(a => a.isSE);
            var assaults = attackCount - seCount;
            
            var maxAssaultsPerCluster = (assaults+clusterCount-1) / clusterCount;
            var maxSePerCluster = (seCount+clusterCount-1) / clusterCount;

            
                var assaultsPerCluster = new int[clusterCount];
                var sePerCluster = new int[clusterCount];
                foreach (var attack in attacks)
                {
                    //   var a = Spot.GetOrAdd(attack.cid);
                    var isSE = attack.isSE;
                    int best = -1;
                    float bestDist = float.MaxValue;
                  
                    for(int cluster =0;cluster<clusterCount;++cluster)
                    {
                        if (isSE)
                        {
                            if (sePerCluster[cluster] >= maxSePerCluster)
                                continue;
                        }
                        else
                        {
                            if (assaultsPerCluster[cluster] >= maxAssaultsPerCluster)
                                continue;

                        }

                        var d = spans[cluster].Distance2(attack.cid.ToWorldC());
                        if (d < bestDist)
                        {
                            bestDist = d;
                            best = cluster;
                        }
                    }

                    Assert(best>=0);
                    if (best >= 0)
                    {
                        if (isSE)
                            ++sePerCluster[best];
                        else
                            ++assaultsPerCluster[best];
                        attack.attackCluster = reals[best].attackCluster;
                    }


                }

            

           
            // optimize
            for (int iter = 0; iter<8; ++iter)
            {
                foreach (var f0 in attacks)
                {
                    //                    var s0 = Spot.GetOrAdd(f0.cid);
                    if (f0.attackCluster==255) // should never happen
                        continue;
                    var c0 = f0.attackCluster;
                    var isSE = f0.isSE;
                    foreach (var f1 in attacks)
                    {
                        if (f1.attackCluster==255 || f1.isSE != isSE)
                            continue;
                        //var s1 = Spot.GetOrAdd(f1.cid);
                        var c1 = f1.attackCluster;
                        if (c0==c1)
                            continue;

                        var d0 = spans[c0].Distance2(f0.cid.ToWorldC());
                        var d1 = spans[c1].Distance2(f1.cid.ToWorldC());

                        var dd0 = spans[c0].Distance2(f1.cid.ToWorldC());
                        var dd1 = spans[c1].Distance2(f0.cid.ToWorldC());
                        if (d0+ d1 > dd0+dd1 )
                        {
                            // a change improves things
                            f1.attackCluster= c0;
                            f0.attackCluster=c1;
                        }

                    }

                }
            }
            // cull
            var culledAttacks = 0;
            var reassignedAttacks = 0;
            foreach (var f0 in attacks)
            {
                if (f0.attackCluster==255) // should never happen
                    continue;
                var c0 = f0.attackCluster;
                var tt = f0.troopType;
                var time = tt.ApproxTravelTime(f0.cid, reals[c0].cid);
                foreach(var t in fakesPerCluster[c0])
                {
                    time = time.Max(tt.ApproxTravelTime(f0.cid, t.cid));
                }
                if (time <= maxTravelHours)
                    continue;// all good

                if (f0.isSE)
                    --sePerCluster[c0];
                else
                    --assaultsPerCluster[c0];

                int bestCluster = -1;
                int bestClusterAttackCount = int.MaxValue;
                // Can we Reassign?
                for(int c1=0;c1<clusterCount;++c1)
                {
                    if (c0 == c1)
                        continue;
                    time = tt.ApproxTravelTime(f0.cid, reals[c1].cid);
                    foreach (var t in fakesPerCluster[c1])
                    {
                        time = time.Max(tt.ApproxTravelTime(f0.cid, t.cid));
                    }
                    if (time >= maxTravelHours)
                        continue;// cannot use
                    var current = f0.isSE ? sePerCluster[c1] : assaultsPerCluster[c1];
                    if (current >= bestClusterAttackCount)
                        continue;
                    bestClusterAttackCount = current;
                }
                if(bestCluster >= 0)
                {
                    ++reassignedAttacks;
                    f0.attackCluster = (byte)bestCluster;
                    if (f0.isSE)
                        ++sePerCluster[bestCluster];
                    else
                        ++assaultsPerCluster[bestCluster];
                }
                else
                {
                    ++culledAttacks;
                }
            }
            int culledClusters = 0;
            // cull clusters with too few attacks
            for(int c=0;c<clusterCount;++c)
            {
                if (sePerCluster[c]==0 || assaultsPerCluster[c] < minAssaults)
                {
                    ++culledClusters;
                    foreach (var f0 in attacks)
                    {
                        if(f0.attackCluster == c)
                        {
                            f0.attackCluster = 255;
                        }
                    }
                    reals[c].attackCluster=255;
                    foreach (var t in fakesPerCluster[c])
                    {
                        t.attackCluster=255;
                    }
                }
            }

            WritebackAttacks();

            //         var targetCluster = new List<int>();
            //           targetCluster.Add(real.cid);
            //            targetCluster.Add(bestFake);
            float maxDistanceToSE = attacks.Where((a) => a.typeT==Attack.Type.se && a.attackCluster!=255).Max((a) => spans[a.attackCluster].Distance(a.cid.ToWorldC() ));
            float averageDistanceToSE = attacks.Where((a) => a.typeT==Attack.Type.se && a.attackCluster!=255).Average((a) => spans[a.attackCluster].Distance(a.cid.ToWorldC()));
            float maxDistanceToAssault = attacks.Where((a) => a.typeT!=Attack.Type.se && a.attackCluster!=255).Max((a) => spans[a.attackCluster].Distance(a.cid.ToWorldC()));
            float averageDistanceToAssault = attacks.Where((a) => a.typeT!=Attack.Type.se && a.attackCluster!=255).Average((a) => spans[a.attackCluster].Distance(a.cid.ToWorldC()));

            float maxClusterSize = spans.Max((a) => a.radius2.Sqrt() );
            float averageClusterSize = spans.Average((a) => a.radius2.Sqrt());

            //           bad = $"{bad} Assigned {reals} reals and {fakes} fakes\nUnused: reals {unusedReals}, fakes {unusedFakes}";

            Note.Show($"Attack plan done, {clusterCount} reals, {maxAssaultsPerCluster+maxSePerCluster} attacks per cluster, {maxSePerCluster}{fakeCount} fakes per target,{sePerCluster} SE per cluster,{culledClusters} culled clutsers,{culledAttacks},{reassignedAttacks} reassigned attacks, SE max Distance- max: {maxDistanceToSE} av: {averageDistanceToSE}  Assault distance- max:{maxDistanceToAssault} av:{averageDistanceToAssault} Cluster size max: {maxClusterSize} av: {averageClusterSize}");
            attacks.NotifyReset();
            targets.NotifyReset();
            //StringBuilder sb = new StringBuilder();
            //foreach (var a in attacks)
            //{
            //    if (a.target!=0)
            //    {
            //        sb.Append($"{a.player} <coords>{a.target.CidToString()}</coords> {AttackType.types[a.type].name} {(a.fake ? "Fake" : "Real")} at {time.FormatDefault()}\n");
            //    }
            //}
            //App.CopyTextToClipboard(sb.ToString());
            SaveAttacks();
        }

        //private void AssignAttacks_Click(object sender, RoutedEventArgs e)
        //{
        //    string bad = string.Empty;
        //    var reals = 0;
        //    var fakes = 0;
        //    CleanTargets();

        //    foreach (var a in attacks)
        //    {
        //        if (a.target != 0) // not already assigned
        //        {
        //            if (!a.fake)
        //                ++reals;
        //            else
        //                ++fakes;
        //            continue;
        //        }
        //        Spot best = null;
        //        float bestScore = float.MaxValue;
        //        foreach (var t in targets)
        //        {
        //            if (a.fake == (t.attackCluster == 0)) // real or fake match
        //                continue;
        //            if (t.attackAssignment != null)
        //                continue; // already taken
        //            var score = a.cid.DistanceToCid(t.cid);
        //            if (!a.fake)
        //            {
        //                switch (a.troopType)
        //                {
        //                    case ttSorcerer:
        //                    case ttDruid:
        //                        {
        //                            switch (t.classification)
        //                            {
        //                                case Spot.Classification.unknown:
        //                                    break;
        //                                case Spot.Classification.vanqs:
        //                                case Spot.Classification.rt:
        //                                    score -= 3;
        //                                    break;
        //                                case Spot.Classification.sorcs:
        //                                case Spot.Classification.druids:
        //                                    score += 2;
        //                                    break;
        //                                case Spot.Classification.academy:
        //                                    score += 8;
        //                                    break;
        //                                case Spot.Classification.horses:
        //                                case Spot.Classification.arbs:
        //                                    score -= 4;
        //                                    break;
        //                                case Spot.Classification.se:
        //                                    break;
        //                                case Spot.Classification.hub:
        //                                    break;
        //                                case Spot.Classification.navy:
        //                                    break;
        //                                case Spot.Classification.misc:
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            break;
        //                        }
        //                    case ttVanquisher:
        //                        {
        //                            switch (t.classification)
        //                            {
        //                                case Spot.Classification.unknown:
        //                                    break;
        //                                case Spot.Classification.vanqs:
        //                                case Spot.Classification.rt:

        //                                    score += 4;
        //                                    break;
        //                                case Spot.Classification.sorcs:
        //                                case Spot.Classification.druids:
        //                                    score -= 2;
        //                                    break;
        //                                case Spot.Classification.academy:
        //                                    score -= 4;
        //                                    break;
        //                                case Spot.Classification.horses:
        //                                case Spot.Classification.arbs:

        //                                    score -= 3;
        //                                    break;
        //                                case Spot.Classification.se:
        //                                    break;
        //                                case Spot.Classification.hub:
        //                                    break;
        //                                case Spot.Classification.navy:
        //                                    break;
        //                                case Spot.Classification.misc:
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            break;
        //                        }
        //                    case ttHorseman:
        //                        {
        //                            switch (t.classification)
        //                            {
        //                                case Spot.Classification.unknown:
        //                                    break;
        //                                case Spot.Classification.vanqs:
        //                                case Spot.Classification.rt:

        //                                    score += 2;
        //                                    break;
        //                                case Spot.Classification.sorcs:
        //                                case Spot.Classification.druids:
        //                                    score -= 2;
        //                                    break;
        //                                case Spot.Classification.academy:
        //                                    score -= 6;
        //                                    break;
        //                                case Spot.Classification.horses:
        //                                case Spot.Classification.arbs:

        //                                    score += 7;
        //                                    break;
        //                                case Spot.Classification.se:
        //                                    break;
        //                                case Spot.Classification.hub:
        //                                    break;
        //                                case Spot.Classification.navy:
        //                                    break;
        //                                case Spot.Classification.misc:
        //                                    break;
        //                                default:
        //                                    break;
        //                            }
        //                            break;
        //                        }
        //                }
        //            }
        //            if (score < bestScore)
        //            {
        //                bestScore = score;
        //                best = t;

        //            }

        //        }
        //        if (best == null)
        //        {
        //            bad = $"{bad}No target for {a.xy} isReal: {!a.fake}\n";

        //        }
        //        else
        //        {
        //            a.target = best.cid;
        //            best.attackAssignment = a;
        //            if (!a.fake)
        //                ++reals;
        //            else
        //                ++fakes;

        //        }
        //    }
        //    var unusedReals = 0;
        //    var unusedFakes = 0;
        //    foreach (var a in attacks)
        //    {
        //        if (a.target == 0)
        //        {
        //            if (a.fake)
        //                ++unusedFakes;
        //            else
        //                ++unusedReals;
        //        }
        //    }

        //    bad = $"{bad} Assigned {reals} reals and {fakes} fakes\nUnused: reals {unusedReals}, fakes {unusedFakes}";

        //    Note.Show(bad);
        //    attacks.NotifyReset();
        //    targets.NotifyReset();
        //    StringBuilder sb = new StringBuilder();
        //    foreach (var a in attacks)
        //    {
        //        if (a.target!=0)
        //        {
        //            sb.Append($"{a.player} <coords>{a.target.CidToString()}</coords> {AttackType.types[a.type].name} {(a.fake ? "Fake" : "Real")} at {time.FormatDefault()}\n");
        //        }
        //    }
        //    App.CopyTextToClipboard(sb.ToString());
        //    SaveAttacks();
        //}

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
            CleanTargets();
            attacks.NotifyReset();
        }
    }
}
