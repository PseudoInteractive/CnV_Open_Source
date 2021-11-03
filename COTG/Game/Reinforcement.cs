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

namespace COTG.Game;

//"trintr"
public class Reinforcement
{


	public SmallTime time;  // arrival
	public DateTimeOffset _Time { get => time.dateTime; }
	public int sourceCid;
	public City _Source { get => sourceCid.AsCity(); }
	public int targetCid;
	public City _Target { get => targetCid.AsCity(); }
	//static int pid;

	public long order;

	public TroopTypeCountsRef troops = new();
	public string _Troops { get => troops.Format(":",' ',','); }

	static async Task Return(Reinforcement order)
	{
		await Post.Get("overview/reinreca.php","a=" + order.order,order.sourceCid.CidToPid());
		await Task.Delay(1000);
		await Post.Get("overview/reinreca.php","a=" + order.order,order.sourceCid.CidToPid());
	}


	internal static async void ShowReinforcements(int _cid,UIElement uie)
	{
		try
		{
			App.UpdateKeyStates();

			var showAll = _cid == 0;

			await Services.ReinforcementsOverview.instance.Post();
			var _spot = _cid == 0 ? null : Spot.GetOrAdd(_cid);
			var scroll = new ScrollViewer();

			var tab = ReinforcementsTab.instance;


			var spots = !showAll ? new[] { _spot } : City.myCities.OrderBy(a => a.cid.ZCurveEncodeCid()).ToArray();

			var orders = new List<Reinforcement>();

			if(showAll)
			{
				tab.reinInTitle = "All Incoming Reinforcements";
				tab.reinOutTitle = "All Outgoing Reinforcements";
			}
			else
			{

				tab.reinInTitle = "Incoming Reinforcements";
				tab.reinOutTitle = "Outgoing Reinforcements";
			}

			//tab. panel.Children.Add(new TextBlock() { Text = showAll ? "All Incoming Reinforcements" : "Reinforcements Here:" });
			{
				var toAdd = new List<Reinforcement>();
				foreach(var s in spots.OrderByDescending(s => s.incomingFlags))
				{
					foreach(var reIn in s.reinforcementsIn.OrderBy(a => Player.IdToName(a.sourceCid.CidToPid())))
					{
						//var other = Spot.GetOrAdd(reIn.sourceCid);
						//var me = Spot.GetOrAdd(reIn.targetCid);
						//var content = showAll ? $"{other.xy} {other.playerName} {other.nameAndRemarks} {other.IncomingInfo() } -> {me.xy} {me.nameAndRemarks} {me.IncomingInfo()} {reIn.troops.Format(":",' ',',')}"
						//	: $"{other.xy} {other.playerName} {other.nameAndRemarks} {other.IncomingInfo() } {reIn.troops.Format(":",' ',',')}{reIn.time.FormatIfLaterThanNow()}";
						orders.Add(reIn);

					}
				}
				tab.reinforcementsIn.Set(toAdd);
			}
			//			List<>
			var byFlags = spots.SelectMany(s => s.reinforcementsOut).GroupBy(s => s.targetCid.AsCity().incomingFlags);
			foreach(var flagGroup in byFlags.OrderByDescending(s => (int)s.Key))
			{
				var toAdd = new List<Reinforcement>();
				foreach(var reIn in flagGroup.OrderByDescending(s => s.targetCid.AsCity().incomingFlags).ThenBy(s => Player.IdToName(s.targetCid.CidToPid())).ThenBy(a => a.time))
				{
					//	foreach(var reIn in cid)
					{
						//var other = Spot.GetOrAdd(reIn.targetCid);
						//var me = Spot.GetOrAdd(reIn.sourceCid);
						//var content = showAll ? $"{other.xy} {other.playerName} {other.nameAndRemarks} {other.IncomingInfo()} <- {me.xy} {me.nameAndRemarks} {me.IncomingInfo()} {reIn.troops.Format(":",' ',',')}"
						//	: $"{other.xy} {other.playerName} {other.nameAndRemarks} {other.IncomingInfo()} {reIn.troops.Format(":",' ',',')}{reIn.time.FormatIfLaterThanNow()}";

						toAdd.Add(reIn);
					}
				}
				tab.reinforcementsIn.Set(toAdd);
			}
			//}
			//	var result = await msg.ShowAsync2(uie);
			//	if(result == ContentDialogResult.Primary)
			//	{
			//		ShellPage.WorkStart("Return..");
			//		int counter = 0;
			//		foreach(var check in panel.Children)
			//		{
			//			if(!(check is CheckBox c))
			//				continue;
			//			if(c.IsChecked.GetValueOrDefault())
			//			{
			//				await Return(orders[counter]);
			//			}

			//			++counter;
			//			ShellPage.WorkUpdate($"Return.. {counter}");
			//		}
			//		if(counter > 0)
			//		{
			//			await Task.Delay(400);
			//			await Services.ReinforcementsOverview.instance.Post();
			//		}


			//	}
			tab.OnPropertyChanged();
			ShellPage.WorkEnd("Return..");
			tab.ShowOrAdd(true,false);

		}
		catch(Exception __ex)
		{
			Debug.LogEx(__ex);
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
