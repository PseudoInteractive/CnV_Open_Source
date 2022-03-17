using Microsoft.UI.Xaml;

using static CnV.Troops;
namespace CnV.Styles
{
	using Game;

	public class ReportKillStyleSelector : Microsoft.UI.Xaml.Controls.StyleSelector
		{
			public Style attackerWinStyle { get; set; }

			public Style defenderWinStyle { get; set; }

			public Style tieStyle { get; set; }
			public Style noKillsStyle { get; set; }

			protected override Style SelectStyleCore(object item, DependencyObject container)
		{
			//		var cell = (item as DataGridCellInfo);
			var report = item as ArmyReport;
			if(report.isPending)
				return noKillsStyle;
			if(report.isSieging)
				return report.claim > 0 ? attackerWinStyle : tieStyle;

			var dKill = report.dTsKill;
			var aKill = report.aTsKill;
			if(dKill < 1000 && aKill < 1000)
				return report.isScout ? attackerWinStyle : noKillsStyle;
			if(dKill > aKill * 3 / 2)
				return defenderWinStyle;
			if(aKill > dKill * 3 / 2)
				return attackerWinStyle;
			return tieStyle;
		}

		


	}
		public class ReportTypeStyleSelector : Microsoft.UI.Xaml.Controls.StyleSelector
		{
			public Style pendingStyle { get; set; }

			public Style capturingStyle { get; set; }
			public Style capturedStyle { get; set; }

			public Style siegingStyle { get; set; }
			public Style siegeStyle { get; set; }

			public Style scoutStyle { get; set; }
			public Style assaultStyle { get; set; }
			public Style plunderStyle { get; set; }


			protected override Style SelectStyleCore(object item, DependencyObject container)
			{
//				var cell = (item as DataGridCellInfo);
				var report = item as Army;
				if( report.claim >= 100)
					return capturedStyle;
				if (report.claim > 0)
					return capturingStyle;

			switch(report.type)
			{
				case ArmyType.assault: return assaultStyle;
					case ArmyType.plunder: return plunderStyle;
					case ArmyType.scout: return scoutStyle;
					case ArmyType.siege	 when !report.isSieging: return siegeStyle;
					case ArmyType.siege  : return siegingStyle;
				default: Assert(false); return scoutStyle;
					
			}

		}
		
	}

}
