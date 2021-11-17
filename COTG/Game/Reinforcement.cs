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

using System.Net.Http;

//"trintr"
[Serializable]
[System.Diagnostics.DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Reinforcement : IEquatable<Reinforcement>
{
	public long order;

	public SmallTime time;  // arrival

	public string TimeString
	{
		get => isReturning ? (time.FormatIfLaterThanNow( "Home") + "(Returning)") : time.FormatIfLaterThanNow("Here");
	}
	public DateTimeOffset? dateTime
	{
		get => (time > SmallTime.serverNow) ? time.dateTime : null;
	}
	public int sourceCid;

	public int targetCid;
	//static int pid;
	public bool isReturning;

	public string troopsString => troops.Format(":", ' ', ',');
	public City sourceCity => sourceCid.AsCity();
	public City targetCity => targetCid.AsCity();
	public City[] sourceCities => new [] { sourceCid.AsCity() };
	public City[] targetCities => new [] { targetCid.AsCity() };

	public const string retUriS = @"cmd://";

	public Uri retUri => new(new(retUriS),
		$"reinRet?order={order.ToString()}&pid={sourceCity.pid.ToString() }");
	public string  retS => "Return";
	
	public TroopTypeCountsRef troops = new();
	public City[] cities => new []{sourceCity, targetCity};
	public string _Troops { get => troops.Format(":",' ',','); }

	public  async Task ReturnAsync()
	{
		Log($"Return {this}");
		await ReturnAsync(order,sourceCid.CidToPid());
		// should not be needed, but who knows?
		if(targetCid.CidToPid()!=sourceCid.CidToPid())
			await ReturnAsync(order,targetCid.CidToPid());
	}
	public static async Task ReturnAsync(long order, int pid)
	{
		await Post.Get("overview/reinreca.php","a=" + order,pid);
		await Task.Delay(1000);
		await Post.Get("overview/reinreca.php","a=" + order,pid );
	}

	public string sReturn => "Return";

	internal static async void ShowReinforcements(int _cid,UIElement uie)
	{
		try
		{
			var tab = ReinforcementsTab.instance;
			tab.targetCid = _cid;
			await tab.Update();

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
			
			tab.ShowOrAdd(true,false);

		}
		catch(Exception __ex)
		{
			Debug.LogEx(__ex);
		}


	}

	public bool Equals(Reinforcement other)
	{
		return order == other.order;
	}

	private string GetDebuggerDisplay()
	{
		return ToString();
	}

	public override string ToString()
	{
		return $"{{{nameof(time)}={time.ToString()},  {nameof(sourceCity)}={sourceCity}, {nameof(targetCity)}={targetCity}, {nameof(troops)}={troops.ToString()}}}";
	}
}
	
    public static class ReinforcementHelper
    {
        public static int TS(this IEnumerable<Reinforcement>? that)
        {
            var rv = 0;
            if (that is not null)
            {
	            foreach (var t in that)
	            {
		            rv += t.troops.TS();
	            }
            }

            return rv;
        }
		public static Reinforcement [] WhereNotMine(this IEnumerable<Reinforcement>? me)
		{
			if (me == null || !me.Any() )
				return Array.Empty<Reinforcement>();
			return me.Where(r => !Player.IsMe(r.sourceCid.CidToPid())).ToArray();
		}

	}
