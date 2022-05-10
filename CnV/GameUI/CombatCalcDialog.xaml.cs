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
	public sealed partial class CombatCalcDialog:DialogG,INotifyPropertyChanged
	{
		protected override string title => "Combat Calc"; 
		internal static CombatCalcDialog? instance;
		internal CombatTroopItem [] attackers;
		internal CombatTroopItem [] defenders;
		internal float winRatio;
		internal string winRatioS => $"Attack Ratio: {winRatio}";
		internal int wallLevel;
		internal float nightProtection;
		internal int attackType = (int)ArmyType.assault;
		internal City city => City.GetBuild();

		public CombatCalcDialog()
		{
			this.InitializeComponent();
			instance = this;
			UpdateTroopItems();
		}

		private void UpdateTroopItems()
		{
			if(attackers is not null)
				return;
			attackers= new CombatTroopItem[Troops.ttCount];
			defenders= new CombatTroopItem[Troops.ttCount];

			for(int i = 0;i<Troops.ttCount;++i)
			{
				attackers[i] = new() { city = city,type=(byte)i,count=0 };
				defenders[i] = new() { city = city,type=(byte)i,count=0 };
				
			}
			defenders[ttTriari].towerVis = defenders[ttRanger].towerVis = defenders[ttPriest].towerVis = defenders[ttBallista].towerVis =
				defenders[ttScout].towerVis = Visibility.Visible;
			OnPropertyChanged();

		}
		public static void ShowInstance()
		{
			var rv = instance ?? new CombatCalcDialog();
			
			rv.Show(true) ;
			
		}

	
		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}
		public static void Changed(string? member = null)
		{
			if(instance is not null)
				instance.OnPropertyChanged(member);
		}

		private void CalcClick(object sender,RoutedEventArgs e)
		{
			var b = new BattleReport();
			b.attacker = new ArmyResult(new Army(null,default,default,default,default,default,(ArmyType)attackType,new(attackers.Where(i => i.count > 0).Select(i => new TroopTypeCount(i.type,i.count))),
				false));
			b.defenders = new[] { new ArmyResult(new TroopTypeCounts(defenders.Where(i => i.count > 0).Select(i => new TroopTypeCount(i.type,i.count)))) };

			var rv = CombatCalc.Go(b,defenders.Select(a => (ushort)a.towerSlots).ToArray(),
			wallLevel:(byte)wallLevel,(uint)City.GetBuild().cid + Sim.simTime.seconds*1111u, nightProtectionAttenuator:1.0f - nightProtection*0.01f);
			for(var  i = ttZero;i<Troops.ttCount;++i)
			{
				attackers[i].surviving=b.attacker.survived.GetCount(i);
				defenders[i].surviving=b.GetTarget().survived.GetCount(i);
				attackers[i].OnPropertyChanged();
				defenders[i].OnPropertyChanged();
			}
			winRatio = rv;
			OnPropertyChanged();
		}
	}
	internal class CombatTroopItem:INotifyPropertyChanged
	{

		internal City city; // convienience
		internal TType type;
		internal uint count;
		internal uint surviving;
		internal int towerSlots;
		internal Visibility towerVis = Visibility.Collapsed;

		internal TroopTypeCount tt => new(type,count.Max(0));
		internal ImageSource image => Troops.Image(type);
		internal TroopInfo info => TroopInfo.all[type];
		internal string survivingS => $"=> {surviving.Format()}";




		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}

	
	}

}
