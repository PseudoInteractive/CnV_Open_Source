using COTG.Game;
using COTG.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

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
    public sealed partial class AttackTab : UserTab
    {
        public static AttackTab instance;
        public static bool IsVisible() => instance.isVisible;

        public static DumbCollection<Attack> attacks = new DumbCollection<Attack>();
        public static DumbCollection<Spot> targets = new DumbCollection<Spot>();
        public AttackTab()
        {
            Assert(instance == null);
            instance = this;
            this.InitializeComponent();

          //  AttackTab.attacks.Clear();
            AttackTab.attacks.AddRange(App.Settings().Read(nameof(AttackTab.attacks), Array.Empty<Attack>()));

            var lastTargets = App.Settings().Read("targets", Array.Empty<TargetPersist>());
            foreach(var target in lastTargets)
            {
                var t = Spot.GetOrAdd(target.cid);
                t.attackGroup = target.attackGroup;
                targets.Add(t);

            }
        }

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
            var temp = new List<Attack>();
            foreach (var sel in attackGrid.SelectedItems)
            {
                temp.Add(sel as Attack);
            }
            foreach(var sel in temp)
            { 
                var id = attacks.IndexOf(sel as Attack);
                if( id >= 0)
                {
                    attacks.RemoveAt(id);
                }

            }
        }

        private async void AddAttackWithNameFromClipboard(Attack atk)
        {
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
            var spot = Spot.GetOrAdd(cid);
            spot.attackGroup = group;
            targets.Add(spot);
        }
    }
}
