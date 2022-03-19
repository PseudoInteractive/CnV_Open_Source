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
	public sealed partial class SendResDialogue:DialogG,INotifyPropertyChanged
	{
		internal static SendResDialogue? instance;
		protected override string title => $"Send from {source} to {destination}"; 
		internal Resources res;
		
		internal City source;
		internal City destination;

		public static void ShowInstance(City city,City target)
		{
			var rv = instance ?? new SendResDialogue();
			rv.source = city;
			rv.destination = target;
			rv.SetDefault();
			rv.Show(false);

		}

		private void SetDefault()
		{
			res = source.SampleResources();
			var gain = bandWidth / (double)res.sum;
			if(gain < 1.0f)
				res *= gain;

			OnPropertyChanged();
		}

		internal void SetMax(int id)
		{
			var extra = bandWidth - res.sum;
			var extra2 = (source.SampleResources()[id] - res[id]).Min(extra);
			res[id] += extra2;
			OnPropertyChanged();
		}


		public SendResDialogue()
		{
			this.InitializeComponent();
			instance = this;
		}
		internal string FreeRes(int i) => source.SampleResources()[i].Format();

		bool viaWater => transport.SelectedIndex == 1;
		bool viaLand => transport.SelectedIndex == 0;
		internal int bandWidth =>  viaLand ? (int)source.cartsHome*1000 : (int)source.shipsHome*10000;
		internal RoutedEventHandler SetMaxDel(int id) => (_,_) => SetMax(id);


		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.DispatchOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}
		public static void Changed(string? member = null)
		{
			if(instance is not null)
				instance.OnPropertyChanged(member);
		}

		internal string shipsStr => $"Ships {source.shipsHome}/{source.ships}";
		internal string cartsStr => $"Carts {source.cartsHome}/{source.carts}";
		private void SetMax0(object sender,RoutedEventArgs e) => SetMax(0);
		private void SetMax1(object sender,RoutedEventArgs e) => SetMax(1);
		private void SetMax2(object sender,RoutedEventArgs e) => SetMax(2);
		private void SetMax3(object sender,RoutedEventArgs e) => SetMax(3);

		private void SendClick(object sender,RoutedEventArgs e)
		{
		
		}

		private void transport_SelectionChange(object sender,SelectionChangedEventArgs e)
		{
			if(source is not null)
				SetDefault();
		}

	}
}
