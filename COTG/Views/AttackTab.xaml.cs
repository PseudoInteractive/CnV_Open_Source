using COTG.Game;
using COTG.Helpers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
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



        private void AttackCoord_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var i = sender as FrameworkElement;

            var spot = i.DataContext as Attack;
            if (spot.target != 0)
            {
                Spot.ProcessCoordClick(spot.target, false, App.keyModifiers);
                foreach (var t in targets)
                {
                    if (t.cid == spot.target)
                        targetGrid.ScrollIntoView(t, null);
                }
            }
        }

        public void TouchLists()
        {
            // First init
            //  AttackTab.attacks.Clear();

            if (attacks.IsNullOrEmpty() && targets.IsNullOrEmpty())
            {
                App.DispatchOnUIThreadSneaky(() =>
                {
                    attacks.Set(App.Settings().Read(nameof(attacks), Array.Empty<Attack>()));

                    var lastTargets = App.Settings().Read(nameof(targets), Array.Empty<TargetPersist>());
                    var spots = new List<Spot>();
                    foreach (var target in lastTargets)
                    {
                        var t = Spot.GetOrAdd(target.cid);
                        t.attackCluster = target.attackCluster;
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

        }

        private async void AddAttackWithNameFromClipboard(Attack atk)
        {
            TouchLists();
            try
            {

                atk.player = await Clipboard.GetContent().GetTextAsync();
            }
            catch (Exception ex)
            {
                atk.player = string.Empty;
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
            AddAttackWithNameFromClipboard(atk);
        }

        private void AddSenator_Click(object sender, RoutedEventArgs e)
        {
            var atk = new Attack();
            atk.type = (int)Attack.Type.senator;
            atk.fake = false;
            AddAttackWithNameFromClipboard(atk);

        }

        internal static void AddTarget(int cid, byte group)
        {
            instance.TouchLists();
            var spot = Spot.GetOrAdd(cid);
            spot.attackCluster = group;
            if (targets.Contains(spot))
            {
                Note.Show("Target is already present");
                return;
            }
            targets.Add(spot);
        }

        private void AssignAttacks_Click(object sender, RoutedEventArgs e)
        {
            foreach (var a in attacks)
            {
                a.target = 0;

            }
            string bad = string.Empty;
            var reals = 0;
            var fakes = 0;
            foreach (var t in targets)
            {
                var isReal = t.attackCluster == 0;
                // find an attack
                bool found = false;
                foreach (var a in attacks)
                {
                    if (a.target == 0 && a.fake != isReal)
                    {
                        found = true;
                        if (isReal)
                            ++reals;
                        else
                            ++fakes;
                        a.target = t.cid;
                        break;
                    }
                }
                if (!found)
                {
                    bad = $"{bad}No attack for {t.xy} isReal: {isReal}\n";

                }
            }
            bad = $"{bad}Assigned {reals} reals and {fakes} fakes";
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

        
    }
}
