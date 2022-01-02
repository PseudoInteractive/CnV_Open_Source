using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV
{
	public static partial class ClientUI
	{
		public static bool ArmyProcessTap(Army me,string column)
		{
			switch(column)
			{
				case "city":
				case nameof(Army.sXY): Spot.ProcessCoordClick(me.sourceCid, false, AppS.keyModifiers, false); return true;

				case nameof(Army.sPlayer): CnVClient.ShowPlayer(me.sPlayer); return true;

				case nameof(Army.tPlayer): CnVClient.ShowPlayer(me.tPlayer); return true;

				case "Troops":
				case "Total Def":
				{
					var s = $"{me.targetCid.CidToCoords()}\t{me.sourceCid.CidToCoords()}{ (column=="Troops" ? me.troops : me.sumDef).Format("", '\t', ',')}";
					Note.Show(s);
					App.CopyTextToClipboard(s);
				}
					return true;

			}

			return false;
		}
	}
}
