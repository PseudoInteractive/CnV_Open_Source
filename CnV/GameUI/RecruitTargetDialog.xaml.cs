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

	public sealed partial class RecruitTargetDialog:DialogG, INotifyPropertyChanged
	{
		protected override string title => "Auto Recruit";
		internal static RecruitTargetDialog? instance;
		internal City city;

		public RecruitTargetDialog()
		{
			this.InitializeComponent();
			instance = this;

		}

		private void UpdateTroopItems()
		{
			troopItems = new RecruitTroopItem[Troops.ttCount];


			for(int i = 0;i<Troops.ttCount;++i)
				troopItems[i] = new() { city = city,type=(byte)i };
			Resetcounts();

		}

		private void Resetcounts()
		{
			for(int i = 0;i<Troops.ttCount;++i)
				troopItems[i].count=0;
			if(city.MO is not null)
			{
				foreach(var tt in city.MO.troopTargets)
				{
					troopItems[tt.t].count = tt.count;
				}
			}
			OnPropertyChanged();
		}

		public static void ShowInstance(City city)
		{
			var rv = instance ?? new RecruitTargetDialog();
			rv.city = city;
			rv.UpdateTroopItems();
			rv.Show(false);

		}

		internal RecruitTroopItem[] troopItems;
		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if(this.PropertyChanged is not null)
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}
		public static void Changed(string? member = null)
		{
			if(instance is not null)
				instance.OnPropertyChanged(member);
		}

		private void Apply(object sender,RoutedEventArgs e)
		{
			var ttc = new TroopTypeCounts();
			for(int i = 0;i<ttCount;++i)
			{
				var c = troopItems[i].count.Min(TroopTypeCount.countMax);
				ttc += new TroopTypeCount( (TType)i,(uint)c );
			}
			new CnVEventCityTroopTargets(city.c,ttc).EnqueueAsap();
			Done();
		}

		private void Reset(object sender,RoutedEventArgs e)
		{
			Resetcounts();
		}
	}
}
