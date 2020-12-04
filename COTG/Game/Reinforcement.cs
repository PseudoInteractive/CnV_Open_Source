using COTG.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace COTG.Game
{

    //"trintr"
    public class Reinforcement
    {
        public int sourceCid;
        public int targetCid;

        public long order;
        public TroopTypeCount[] troops = TroopTypeCount.empty;
        static async void Return(long order)
        {
            await Post.Send("overview/reinreca.php", "a=" + order);
            await Task.Delay(1000);
            await Post.Send("overview/reinreca.php", "a=" + order);
        }
        internal static async void ShowReturnDialog(int cid,UIElement uie)
        {
            

            await Services.ReinforcementsOverview.instance.Post();
            var spot = Spot.GetOrAdd(cid);
            var panel = new StackPanel();

            
            var msg = new ContentDialog()
            {
                Title="Return Reinforcements",
                Content=panel,
                IsPrimaryButtonEnabled=true,
                PrimaryButtonText="Go",
                CloseButtonText="Cancel"

            };
			ElementSoundPlayer.Play(ElementSoundKind.Show);
			msg.CopyXamlRoomFrom(uie);

            var orders = new List<long>();
            panel.Children.Add(new TextBlock() { Text="Reinforcements Here:" });
            foreach(var reIn in spot.reinforcementsIn)
            {
                var other = Spot.GetOrAdd(reIn.sourceCid);
                panel.Children.Add( new CheckBox() { Content = $"{other.xy} {other.nameAndRemarks}{reIn.troops.Format(":",' ',',')}",IsChecked=false } );
                orders.Add(reIn.order);
            }
            panel.Children.Add(new TextBlock() { Text="\nDeployed Reinforcements:" });
            foreach (var reIn in spot.reinforcementsOut)
            {
                var other = Spot.GetOrAdd(reIn.targetCid);
                panel.Children.Add(new CheckBox() { Content = $"{other.xy} {other.nameAndRemarks}{reIn.troops.Format(":", ' ', ',')}", IsChecked=false });
                orders.Add(reIn.order);
            }

            var result = await msg.ShowAsync2();
            if (result == ContentDialogResult.Primary)
            {
                int counter = 0;
                foreach(var check in panel.Children)
                {
                    if (!(check is CheckBox c))
                        continue;
                    if(c.IsChecked.GetValueOrDefault())
                    {
                        Return( orders[counter]);
                        await Task.Delay(400);
                    }

                    ++counter;
                }
            }
        }
    }
    public static class ReinforcementHelper
    {
        public static int TS(this Reinforcement[] that)
        {
            var rv = 0;
            foreach(var t in that)
            {
                rv += t.troops.TS();
            }
            return rv;
        }

    }
}
