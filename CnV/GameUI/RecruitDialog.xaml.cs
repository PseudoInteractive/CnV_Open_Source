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
	public sealed partial class RecruitDialog:DialogG,INotifyPropertyChanged
	{
		protected override string title => "Recruit"; 
		internal static RecruitDialog? instance;
		internal City city;

		public RecruitDialog()
		{
			this.InitializeComponent();
			instance = this;
			
		}

		private void UpdateTroopItems()
		{
			troopItems = new RecruitTroopItem[Troops.ttCount];

			
			for(int i = 0;i<Troops.ttCount;++i)
				troopItems[i] = new() { city = city,type=(byte)i,count=1 };
					
			OnPropertyChanged();

		}
		public static void ShowInstance(City city)
		{
			var rv = instance ?? new RecruitDialog();
			rv.city = city;
			rv.UpdateTroopItems();
			rv.Show(false) ;
			
		}

		internal RecruitTroopItem [] troopItems;
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

		
	}


	internal class RecruitTroopItem:INotifyPropertyChanged
	{

		internal City city; // convienience
		internal TType type;
		internal int count;

		public TType _type { get => type;
			set {
				if(value != type) {
					type = value;
					OnPropertyChanged();
				}
			}
		}
		internal TroopTypeCount tt => new(type,(uint)count.Max(0));
		internal TroopTypeCount tt1 => new(type,(uint)count.Max(1));
		internal ImageSource image => Troops.Image(type);
		internal TroopInfo info => TroopInfo.all[type.Min(Troops.ttLast) ];
		internal bool isEnabled => city.CanRecruit(type);
		internal void Recruit(object sender,RoutedEventArgs e)
		{
			var tt = this.tt;
			var freeSpace = city.availableTsSpace;
			if(freeSpace < tt.ts)
			{
				AppS.MessageBox("Not enough troop space",hero: AppS.heroMissingFunds);
				return;
			}
			var rr = tt.resResquired;
			var p = city.player;
			if(type==Troops.ttMagistra) {
				if( p.freeSenatorCount < count) {
					AppS.MessageBox(title:$"Can recruit at most {p.freeSenatorCount} Magistra",hero: AppS.heroMissingFunds);
					return;
				}

			}
			if(count <=0) {
				AppS.MessageBox(title:$"Please recruit at least 1");
				return;
			}
			if((city.SampleResources() - rr.r).allPositive && p.gold >= rr.gold)
			{
				Note.Show("Recruit");
				new CnVEventRecruit(city.c,tt).EnqueueAsap();
				count =0;
				OnPropertyChanged();
			}
			else
			{
				AppS.MessageBox(title:"Not enough funds for recruit",hero: AppS.heroMissingFunds);
			}
		}
		public string troopsHome => $"{(city.troopsHome).GetCount(type).Format()}/{city.troopsOwned.GetCount(type).Format()}";
		
		internal void SetHome(object sender,RoutedEventArgs e)
		{
			count = (int)city.troopsHome.GetCount(type);
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
				m = m.Min( (city.player.gold / req.gold).ClampToInt() );

			var freeSpace = city.availableTsSpace;
			m = m.Min(freeSpace / Troops.ttTs[type]);
			
			count = m;
			OnPropertyChanged();
		}

		public string recruitTime => city.CanRecruit(type) ? tt1.RecruitTimeRequired(city).Format() : string.Empty;
		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}

		internal void CountChanged(NumberBox sender,NumberBoxValueChangedEventArgs args)
		{
			App.FilterPositive(sender,args);
			OnPropertyChanged();
		}

	}
}
