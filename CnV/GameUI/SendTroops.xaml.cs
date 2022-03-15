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
	public sealed partial class SendTroops:DialogG
	{
		
		protected override string title => "Send"; 
		internal static SendTroops? instance;
		internal City city;
		internal City target;
		public SendTroops()
		{
			this.InitializeComponent();
			instance = this;
			
		}

		private void UpdateTroopItems()
		{
			troopItems = new SendTroopItem[Troops.ttCount];


			for(int i = 0;i<Troops.ttCount;++i)
				troopItems[i] = new(target:target, city:city,type:(byte)i) ;
		}
		public static void ShowInstance(City city,City target)
		{
			var rv = instance ?? new SendTroops();
			rv.city = city;
			rv.target = target;
			rv.UpdateTroopItems();
			rv.Show(true);
			
		}

		internal SendTroopItem [] troopItems;

		private void SendTroopsClick(object sender,RoutedEventArgs e)
		{
			Hide();
		}
	}


	internal class SendTroopItem: INotifyPropertyChanged
	{
		
		internal City city; // convienience
				internal City target;

		internal TType type;
		internal uint count;

		public SendTroopItem(City city,City target,byte type)
		{
			this.city=city;
			this.target=target;
			this.type=type;
			count = city.troopsHome.GetCount(type);
		}

		internal TroopTypeCount tt => new(type,count);
		internal ImageSource image => Troops.Image(type);
		internal TroopInfo info => TroopInfo.all[type];
		internal void Recruit(object sender,RoutedEventArgs e)
		{
			//AppS.MessageBox("Send",info.ToString());
			//Note.Show("Send");
			//new CnVEventRecruit(city.c,tt).Execute();
			count =0;
			OnPropertyChanged();
		}
		public string troopsHome => city.troopsHome.GetCount(type).Format();
		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.DispatchOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}

	}
}

