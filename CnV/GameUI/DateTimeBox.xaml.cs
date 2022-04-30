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
	public sealed partial class DateTimeBox:UserControl,INotifyPropertyChanged
	{
		public DateTimeBox()
		{
			this.InitializeComponent();
		}

		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
  "Label",
  typeof(string),
  typeof(DateTimeBox),
  new PropertyMetadata("")
);
		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty,value); }
		}
		internal string labelTime => $"{Label} time";
		internal string labelDate => $"{Label} date";

		internal static DateTimeOffset TranslateTime(ServerTime t) => t.dateTime;
		internal void SetDateTime(ServerTime t)
		{
			var tDefault = TranslateTime(t);
			if(date.SelectedDate != tDefault || time.SelectedTime != tDefault)
			{
				date.SelectedDate = tDefault;

				time.SelectedTime = tDefault;

				OnPropertyChanged();
			}
		}
		private void time_DropDownOpened(object sender,EventArgs e)
		{
			// We pretend that this is local
			// 
			var tDefault = TranslateTime(Sim.simTime);
			if(date.SelectedDate is null)
				date.SelectedDate = tDefault;
			if(time.SelectedTime is null)
				time.SelectedTime = tDefault;
		}

		internal static ObservableCollection<ServerTime> recent = new( new[] { ServerTime.zero } );

		public ServerTime dateTime
		{
			get {
				var rv = ServerTime.CombineDateTime(date.SelectedDate,time.SelectedTime);
				if(!recent.Contains(rv))
					recent.Add(rv);
				return rv;
			}
		}

		private void RecentChanged(object sender,SelectionChangedEventArgs e)
		{
			var c = sender as ComboBox;
			if(c.SelectedItem is ServerTime t)
			{
				SetDateTime(t);
			}

		}

		private void NowClick(object sender,RoutedEventArgs e)
		{
			SetDateTime(Sim.simTime);
		}
		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.DispatchOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}
	}
}
