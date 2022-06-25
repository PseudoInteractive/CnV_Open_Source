using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	using static Troops;
	public sealed partial class BattleReportDialog:DialogG
	{
		protected override string title => $"Battle Report {b.arrival.Format()}";
		internal BattleReport b;
		internal string attackType
		{
			get {
				if(b.attackArmy.type is (ArmyType.siege))
					return $"{ttGlyphSiege} Siege claim {b.claim}% ({(b.claim - b.attackArmy.claim).FormatWithSign()})\nattack ratio: {b.attackRatio:N2}x";
				else
					return 	$"{ttGlyphSword} {b.attackArmy.typeS}, ratio: {b.attackRatio:N2}x";
			}
		}
		internal string wallInfo => $"{ttGlyphShield}  {ttGlyphWall} level {b.wallLevel}";

		internal string scoutedResources => b.scoutRecord is not null ? b.scoutRecord.resources.Format(" ","Scouted resources\n") : null;
		public BattleReportDialog(BattleReport _b)
		{
			this.b = _b;
			this.InitializeComponent();
			if(_b.targetPlayer.sharesInfo || AppS.isTest ) { 
			var dPlayers = _b.defenders.Select(a => a.GetPlayer(_b) ).Distinct().ToArray();
				//if(dPlayers.Length > 0) 
				
					defenders = new BattleReportPlayerInfo[dPlayers.Length];
					for(int i = 0;i<dPlayers.Length;++i) {
						defenders[i] = new BattleReportPlayerInfo(dPlayers[i],_b);
					}
				}
				else {
					defendersInfo.Visibility = Visibility.Collapsed;
				}


			
		}

		
		public static void ShowInstance(BattleReport _b)
		{
			var rv = new BattleReportDialog(_b);


			rv.Show(false) ;
			
		}
		internal BattleReportPlayerInfo[] defenders;

	}
	internal sealed class BattleReportPlayerInfo {
		internal Player p;
		internal BattleReport b;

		internal string refines => "Refines: " + ArmyResult.RefinesToRes((uint)b.defenders.Where(d => d.GetPlayer(b)== p)
			.Sum(a=> a.refines )).Format();
		internal string header => $"{b.defenders.Where(d => d.GetPlayer(b) == p)
			.Sum(a => a.contribution):P0} {p.name}";
		internal string troops => "Troops: "+ b.defenders.Where(d => d.GetPlayer(b) == p)
			.Aggregate(new TroopTypeCounts(),(r,a) => r + a.troops).Format();

		internal string survived => "Survived: "+ b.defenders.Where(d => d.GetPlayer(b) == p)
			.Aggregate(new TroopTypeCounts(),(r,a) => r + a.survived).Format();
		

		public BattleReportPlayerInfo(Player p,BattleReport report) {
			this.p=p;
			this.b=report;
		}
	}
	

}
