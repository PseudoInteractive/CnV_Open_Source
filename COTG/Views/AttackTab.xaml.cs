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
                App.DispatchOnUIThreadSneaky(() =>
                {
                    attacks.Set(persist.attacks);
                  
                    var lastTargets = persist.targets;
                    var spots = new List<Spot>();
                    foreach (var target in lastTargets)
                    {
                        var t = Spot.GetOrAdd(target.cid);
                        t.attackCluster = target.attackCluster;
                        t.classification = (Spot.Classification)target.classification;
                        spots.Add(t);

                    }
                    
                    targets.Set(spots);
                });
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

        private async void AddAttackWithCoordsFromClipboard(Attack atk)
        {
            TouchLists();
            try
            {

                atk.cid  = (await Clipboard.GetContent().GetTextAsync()).FromCoordinate();
                atk.player = Spot.GetOrAdd(atk.cid).player;
            }
            catch (Exception ex)
            {
                Log(ex);
            }
            attacks.Add(atk);
            
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
        private void AddFake_Click(object sender, RoutedEventArgs e)
        {
            var atk = new Attack();
            atk.type = (int)Attack.Type.senator;
            atk.fake = true;
            AddAttackWithCoordsFromClipboard(atk);
        }

        private void AddSenator_Click(object sender, RoutedEventArgs e)
        {
            var atk = new Attack();
            atk.type = (int)Attack.Type.senator;
            atk.fake = false;
            AddAttackWithCoordsFromClipboard(atk);

        }

        internal static async void AddTarget(int cid, byte group)
        {
            instance.TouchLists();
            var spot = Spot.GetOrAdd(cid);
            if (targets.Contains(spot))
            {
                Note.Show($"Target is already present in {spot.attackCluster}");
                return;
            }
            spot.attackCluster = group;
            await spot.Classify();
            targets.Add(spot);
        }

        private  static void CleanTargets()
        {
            var set = new HashSet<int>(targets.Count);
            foreach(var t in targets)
            {
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
                }
            }
        }
        private void AssignAttacks_Click(object sender, RoutedEventArgs e)
        {
            string bad = string.Empty;
            var reals = 0;
            var fakes = 0;
            CleanTargets();

            foreach (var t in targets)
            {
                var isReal = t.attackCluster == 0;
                // find an attack
                Attack best = null;
                int bestScore = 0;
                foreach (var a in attacks)
                {
                    // already set?
                    if(a.target == t.cid)
                    {
                        best = a;
                        
                        break;
                    }
                    if (a.target == 0 && a.fake != isReal)
                    {
                        // todo:  score
                        best = a;
                    }
                }
                if (best == null)
                {
                    bad = $"{bad}No attack for {t.xy} isReal: {isReal}\n";

                }
                else
                {
                    best.target = t.cid;
                    if (isReal)
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
