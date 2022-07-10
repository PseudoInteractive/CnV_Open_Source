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
//		internal string labelTime => $"{Label} time";
//		internal string labelDate => $"{Label} date";

		internal static DateTime? TranslateTime(ServerTime t) => t.isZero ? null :  t.dateTimeDT;
		internal void SetDateTime(ServerTime t)
		{
			var tDefault = TranslateTime(t);
			if(date.SelectedDate != tDefault || time.SelectedTime != tDefault)
			{
				date.SelectedDate =   tDefault;

				time.SelectedTime = tDefault;

				OnPropertyChanged();
			}
		}
		private void time_DropDownOpened(object sender,EventArgs e)
		{
			// We pretend that this is local
			// 
			var tDefault = Sim.simTime.dateTimeDT;

			if(time.SelectedTime is null) {
				date.SelectedDate = tDefault;
				time.SelectedTime = tDefault;
			}
		}
		private void date_DropDownOpened(object sender,EventArgs e)
		{
			// We pretend that this is local
			// 
			var tDefault = Sim.simTime.dateTimeDT;
			if(date.SelectedDate is null) {
				date.SelectedDate = tDefault;
				time.SelectedTime = tDefault;
			}
		}

		internal static List<ServerTime> recent = new();
		
		public ServerTime dateTime
		{
			get {
				if(AppS.isShuttingDown)
					return default;
				var rv = ServerTime.CombineDateTime(date.SelectedDate,time.SelectedTime);
				if( rv.isNotZero && !recent.Contains(rv)) {

					recent.Add(rv);
					recent.Sort();
					if(recent.Count > 8)
						recent.RemoveAt(0); // remove oldest
				}

				return rv;
			}
		}
//		internal void ResetRecentTimesComboBox() => recentTimesCombo.SelectedItem = null;

		private void RecentChanged(object sender,SelectionChangedEventArgs e)
		{
			var c = sender as ComboBox;
			if(c.SelectedItem is ServerTime t)
			{
				SetDateTime(t);
			}

		}

		private void ClearClick(object sender,RoutedEventArgs e) {
			Clear();
		}

		internal void Clear() {
			date.SelectedDate=null;
			time.SelectedTime = null;
			OnPropertyChanged();
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.DispatchOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}
		bool isLoaded;
		private void OnLoaded(object sender,RoutedEventArgs e) {
			if(isLoaded)
				return;
			isLoaded = true;
			date.SelectedDateChanged+=DateTime_SelectedTimeChanged;
			time.SelectedTimeChanged+=DateTime_SelectedTimeChanged;
		}

		private void DateTime_SelectedTimeChanged(object? sender,Syncfusion.UI.Xaml.Editors.SelectedDateTimeChangedEventArgs e) {
			OnPropertyChanged();
		}

		private void DateChanged(object sender,Syncfusion.UI.Xaml.Editors.SelectedDateTimeChangedEventArgs e) {
			Note.Show("Date changed");

		}

		private void TimeChanged(object sender,Syncfusion.UI.Xaml.Editors.SelectedDateTimeChangedEventArgs e) {
			Note.Show("Time changed");
		}

        private void ContextRequested(UIElement sender,ContextRequestedEventArgs args) {
			args.Handled    = true;
			var flyout = new MenuFlyout();
			flyout.AddItem("Clear",Symbol.Undo,Clear);
			flyout.AddSeparator();
			foreach(var i in recent) {
				var _i = i;
				flyout.AddItem(_i.Format(),() => SetDateTime(_i));
			}
			flyout.ShowContext(sender,args); 
	    }
    }
}
