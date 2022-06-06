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
	public sealed partial class FixDialog:DialogG,INotifyPropertyChanged
	{
		protected override string title => $"Fix for {city.FormatName(true,true,true)}"; 
	//	internal static FixDialog? instance;
		internal City city;
		internal CnVEventFix fix = new();
		public FixDialog()
		{
			this.InitializeComponent();
		//	instance = this;
			
		}

		

		public static async void ShowInstance(City city)
		{
			var rv = new FixDialog();
			rv.city = city;
			

			await rv.Show(false) ;
			int q = 0;
			
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}
		

		private void PostEvent(object sender,RoutedEventArgs e) {
			// Clone
			var data = MessagePack.MessagePackSerializer.Serialize(fix);
			var fix2 = MessagePackSerializer.Deserialize<CnVEventFix>(data);
			fix2.cityEtc = new WorldCPacked(city.c,0);
			fix2.EnqueueAsap();
			Hide(true);
		}
	}


}
