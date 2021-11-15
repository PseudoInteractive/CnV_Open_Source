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


	public SmallTime time;  // arrival

	public DateTimeOffset _Time
	{
		get => time.dateTime;
	}

	public int sourceCid;

	public int targetCid;
	//static int pid;


	public long order;
	public string troopsString => troops.Format(":", ' ', ',');
	public City sourceCity => sourceCid.AsCity();
	public City targetCity => targetCid.AsCity();

	public const string retUriS = @"cmd://";

	public Uri retUri => new(new(retUriS),
		$"reinRet?order={order.ToString()}&pid={sourceCity.pid.ToString() }");
	public string  retS => "Return";
	
	public TroopTypeCountsRef troops = new();
	public string _Troops { get => troops.Format(":",' ',','); }

	public  async Task ReturnAsync()
	{
		Log($"Return {this}");
		await ReturnAsync(order,targetCid.CidToPid());
		await ReturnAsync(order, sourceCid.CidToPid());
	}
	public static async Task ReturnAsync(long order, int pid)
	{
		await Post.Get("overview/reinreca.php","a=" + order,pid);
		await Task.Delay(1000);
		await Post.Get("overview/reinreca.php","a=" + order,pid );
	}
	public void ReturnClick(object obj)
	{
		Note.Show($"Returning {troopsString} from {targetCity} back to {sourceCity} ");
		ReturnAsync();
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
