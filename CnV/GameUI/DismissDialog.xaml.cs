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

	public sealed partial class DismissDialog:DialogG, INotifyPropertyChanged
	{
		protected override string title => "Dismiss";
		internal static DismissDialog? instance;
		internal City city;

		public DismissDialog()
		{
			this.InitializeComponent();
			instance = this;

		}

		private void UpdateTroopItems()
		{
			var tCount = city.troopsHome.v.Length;
			troopItems = new RecruitTroopItem[tCount];
			

			for(int i = 0;i<tCount;++i)
				troopItems[i] = new() { city = city,type=city.troopsHome.v[i].t };
	
		}

	

		public static void ShowInstance(City city)
		{
			var rv = instance ?? new DismissDialog();
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
			foreach(var i in troopItems)
			{
				var c = i.count.Clamp(0,(int)TroopTypeCount.countMax);
				ttc += new TroopTypeCount( i.type,(uint)c );
			}
			if(city.underSiege) {
				Note.Show("Cannot dismiss while under siege");
				return;
			}
			if(!city.troopsHome.IsSuperSetOf(ttc)) {
				Note.Show("Not enough troops home to dismiss that many");
				return;
		
			}

			new CnVEventDismissTroops(city.c,ttc).EnqueueAsap();
			Done();
		}

		
	}
}
