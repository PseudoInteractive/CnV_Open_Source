﻿using COTG.Game;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telerik.UI.Xaml.Controls.Grid;

using Windows.UI.Xaml;

using static COTG.Game.Enum;
namespace COTG.Styles
{
		public class ReportKillStyleSelector : Windows.UI.Xaml.Controls.StyleSelector
		{
			public Style attackerWinStyle { get; set; }

			public Style defenderWinStyle { get; set; }

			public Style tieStyle { get; set; }
			public Style noKillsStyle { get; set; }

			protected override Style SelectStyleCore(object item, DependencyObject container)
			{
				var cell = (item as DataGridCellInfo);
				var report = cell.Item as Army;
				if (report.type == reportPending)
					return noKillsStyle;
				if (report.type == reportSieging)
					return report.claim > 0 ? attackerWinStyle : tieStyle;

				var dKill = report.dTsKill;
				var aKill = report.aTsKill;
				if (dKill < 1000 && aKill < 1000)
					return report.type == reportScout ? attackerWinStyle : noKillsStyle;
				if (dKill > aKill * 3 / 2)
					return defenderWinStyle;
				if (aKill > dKill * 3 / 2)
					return attackerWinStyle;
				return tieStyle;
			}



		}
		public class ReportTypeStyleSelector : Windows.UI.Xaml.Controls.StyleSelector
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
				var cell = (item as DataGridCellInfo);
				var report = cell.Item as Army;
				if( report.claim >= 100)
					return capturedStyle;
				if (report.claim > 0)
					return capturingStyle;

				switch (report.type)
				{
					case reportAssault: return assaultStyle;
					case reportSiege: return siegeStyle;
					case reportSieging: return siegingStyle;
					case reportPlunder: return plunderStyle;
					case reportPending: return pendingStyle;
					default: return scoutStyle;
				}

			}
		
	}

}
