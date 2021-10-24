using COTG.Services;
using COTG.Views;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using TroopTypeCounts = COTG.Game.TroopTypeCounts;
using TroopTypeCountsRef = COTG.Game.TroopTypeCounts;
using static COTG.Game.TroopTypeCountHelper;
using COTG.Helpers;
//COTG.DArrayRef<COTG.Game.TroopTypeCount>;

namespace COTG.Game
{

    //"trintr"
    public class Reinforcement
    {
		public SmallTime time;  // arrival
		public int sourceCid;
        public int targetCid;
		//static int pid;

        public long order;
		public TroopTypeCountsRef troops = new();
		
		static async Task Return(Reinforcement order)
        {
            await Post.Get("overview/reinreca.php", "a=" + order.order, order.sourceCid.CidToPid() );
            await Task.Delay(1000);
            await Post.Get("overview/reinreca.php", "a=" + order.order, order.sourceCid.CidToPid());
        }
		

		internal static async void ShowReinforcements(int _cid, UIElement uie)
        {
			App.UpdateKeyStates();

			var showAll = _cid == 0;
			
            await Services.ReinforcementsOverview.instance.Post();
            var _spot = _cid == 0 ? null: Spot.GetOrAdd(_cid);
			var scroll = new ScrollViewer();
			
			var panel = new StackPanel();
			scroll.Content = panel;
			//pid = _spot.pid;
            
            
			ElementSoundPlayer.Play(ElementSoundKind.Show);

			var spots = !showAll ? new[] { _spot } : City.myCities.OrderBy(a => a.cid.ZCurveEncodeCid() ).ToArray();
			
            var orders = new List<Reinforcement>();
			
			panel.Children.Add(new TextBlock() { Text= "For info related to other players reinforcements in a city, please visit the city" });
			
			panel.Children.Add(new TextBlock() { Text = showAll ? "All Incoming Reinforcements" : "Reinforcements Here:" });

			foreach (var s in spots)
			{
				foreach (var reIn in s.reinforcementsIn.OrderBy(a=> Player.IdToName(a.sourceCid.CidToPid())) )
				{
					var other = Spot.GetOrAdd(reIn.sourceCid);
					var me = Spot.GetOrAdd(reIn.targetCid);
					var content = showAll ? $"{other.xy} {other.playerName} {other.nameAndRemarks} {other.IncomingInfo() } -> {me.xy} {me.nameAndRemarks} {me.IncomingInfo()} {reIn.troops.Format(":", ' ', ',')}"
						: $"{other.xy} {other.playerName} {other.nameAndRemarks} {other.IncomingInfo() } {reIn.troops.Format(":", ' ', ',')}";
					panel.Children.Add(new CheckBox() { Content = content, IsChecked = false });
					orders.Add(reIn);
				}
			}
            panel.Children.Add(new TextBlock() { Text="\nDeployed Reinforcements:" });
			//			List<>
			var reinOut = spots.SelectMany(s => s.reinforcementsOut).GroupBy(s=>s.targetCid).GroupBy(s=>s.Key.CidToPid());
			foreach (var pid in reinOut.OrderBy(s => Player.IdToName(s.Key) ) )
			{
				foreach(var cid in pid.OrderBy(s => s.Key.ZCurveEncodeCid()))
				{
					foreach(var reIn in cid.OrderBy(a => a.time))
					{
						var other = Spot.GetOrAdd(reIn.targetCid);
						var me = Spot.GetOrAdd(reIn.sourceCid);
						var content = showAll ? $"{other.xy} {other.playerName} {other.nameAndRemarks} {other.IncomingInfo()} <- {me.xy} {me.nameAndRemarks} {me.IncomingInfo()} {reIn.troops.Format(":",' ',',')}"
							: $"{other.xy} {other.playerName} {other.nameAndRemarks} {other.IncomingInfo()} {reIn.troops.Format(":",' ',',')}";

						panel.Children.Add(new CheckBox() { Content = content,IsChecked = false });
						orders.Add(reIn);
					}
				}
			}
			scroll.VerticalScrollMode = ScrollMode.Enabled;
			scroll.HorizontalScrollMode = ScrollMode.Enabled;
			scroll.ZoomMode = ZoomMode.Enabled;
			//scroll.IsTabStop = true;
			var msg = new ContentDialog()
			{
				Title = "Return Reinforcements",
				Content = scroll,
				IsPrimaryButtonEnabled = true,
				PrimaryButtonText = "Go",
				CloseButtonText = "Cancel"

			};
			var result = await msg.ShowAsync2(uie);
			if (result == ContentDialogResult.Primary)
			{
				ShellPage.WorkStart("Return..");
				int counter = 0;
				foreach (var check in panel.Children)
				{
					if (!(check is CheckBox c))
						continue;
					if (c.IsChecked.GetValueOrDefault())
					{
						await Return(orders[counter]);
					}

					++counter;
					ShellPage.WorkUpdate($"Return.. {counter}");
				}
				if (counter > 0)
				{
					await Task.Delay(400);
					await Services.ReinforcementsOverview.instance.Post();
				}

				ShellPage.WorkEnd("Return..");
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
		public static Reinforcement [] WhereNotMine(this Reinforcement[] me)
		{
			if (me == null || me.Length == 0)
				return Array.Empty<Reinforcement>();
			return me.Where(r => !Player.IsMe(r.sourceCid.CidToPid())).ToArray();
		}

	}
}
