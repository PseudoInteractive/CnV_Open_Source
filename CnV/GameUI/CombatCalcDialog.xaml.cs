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
	public sealed partial class CombatCalcDialog:DialogG,INotifyPropertyChanged
	{
		protected override string title => "Combat Calc"; 
		internal static CombatCalcDialog? instance;
		internal CombatTroopItem [] attackers;
		internal CombatTroopItem [] defenders;
		internal float winRatio;
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

		}
	}
	internal class CombatTroopItem:INotifyPropertyChanged
	{

		internal City city; // convienience
		internal TType type;
		internal uint count;
		internal uint surviving;
		internal TroopTypeCount tt => new(type,count);
		internal ImageSource image => Troops.Image(type);
		internal TroopInfo info => TroopInfo.all[type];
		internal bool isEnabled => city.CanRecruit(type);
		internal void Recruit(object sender,RoutedEventArgs e)
		{

			Note.Show("Recruit");
			new CnVEventRecruit(city.c,tt).EnqueueAsap();
			count =0;
			OnPropertyChanged();
		}

		internal string ResRequiredS(int id)
		{
			return ((int)(count.Max(1)*info.resResquired[id])).Format();
		}
		internal void SetMax(object sender,RoutedEventArgs e)
		{
			var res = city.SampleResources();
			var req = info.resResquired;
			var m = 1<<24;
			
			for(int i=0;i<4;++i)
			{
				if(req.r[i] > 0)
					m = m.Min(res[i] / req.r[i]);
			}
			if(req.gold > 0)
				m = m.Min(city.player.gold / req.gold);

			count = (uint)m;
			OnPropertyChanged();
		}

		public string recruitTime => city.CanRecruit(type) ? tt.RecruitTimeRequired(city).Format() : string.Empty;
		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}

		internal void CountChanged(NumberBox sender,NumberBoxValueChangedEventArgs args)
		{
			App.FilterNans(sender,args);
			OnPropertyChanged();
		}

	}

}
