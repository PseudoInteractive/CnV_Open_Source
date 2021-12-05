using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

using static CnV.Debug;
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
using ContentDialog = Microsoft.UI.Xaml.Controls.ContentDialog;
using ContentDialogResult = Microsoft.UI.Xaml.Controls.ContentDialogResult;
using System.Collections.ObjectModel;
using CnV;

namespace CnV.Views
{
	public sealed partial class DateTimePicker : ContentDialog, INotifyPropertyChanged
	{
		public static DateTimePicker instance = new DateTimePicker();
		byte pauseChange;
		byte disableFocusNotification;
		public static DateTimeOffset dateTime;
		public static ObservableCollection<string> recentTimes = new ();

		void TimeToUI()  { time.Text = dateTime.Format(); DateToUI(); } // Does not zero out seconds, hopefully that is okay
		bool TimeFromUI()
		{
			var rv = false;
			var text = time.Text;
			if (
			
				  DateTimeOffset.TryParseExact(text, "HH", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal, out var result)||
				  DateTimeOffset.TryParseExact(text, "HH':'mm", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal, out result) ||
				DateTimeOffset.TryParseExact(text, AUtil.defaultTimeFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces |
				DateTimeStyles.AssumeUniversal, out result) )
				   
			{
				dateTime = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, result.Hour, result.Minute, result.Second, TimeSpan.Zero);
				rv = true;

			}
			else if (
				 DateTimeOffset.TryParseExact(text, AUtil.defaultDateFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal, out result))
			{
				// all except year
				dateTime = new DateTimeOffset(dateTime.Year, result.Month, result.Day, result.Hour, result.Minute, result.Second, TimeSpan.Zero); // strip off timezone 
				rv = true;
			}
			else if(  // anything goes
				DateTimeOffset.TryParse(text, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal, out result)
  )
			{
				// includes date
				dateTime = new DateTimeOffset(result.Year, result.Month, result.Day, result.Hour, result.Minute, result.Second, TimeSpan.Zero); // strip off timezone offset
				rv = true;
			}

			// On parse failure the value will be reverted
			TimeToUI();
			return rv;
		}

		private void DateToUI()
		{
			++pauseChange;
			try
			{
				//			var localDateTime = dateTime - AUtil.localTimeOffset;
				Assert(dateTime.Offset.Ticks == 0);
				var localDate = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, 12, 0, 0, TimeSpan.Zero);
				//if (date.SelectedDates.Count == 1)
				//{
				//    date.SelectedDates[0] = localDate;
				//}
				//else
				{
					date.SelectedDates.Clear();
					date.SelectedDates.Add(localDate);
					date.SetDisplayDate(localDate);
				}

			}
			finally
			{
				--pauseChange;
			}
			OnPropertyChanged(string.Empty);
		}

		public static async Task<(DateTimeOffset t, bool yes)> ShowAsync(string title, DateTimeOffset? _time=null)
		{
			var result = await AppS.DispatchOnUIThreadTask(async () =>
			{

				if (title != null)
					instance.Title = title;
				if (recentTimes.Count == 0)
					AddRecentTime(CnVServer.ServerTime());
				if (_time != null)
				{
					dateTime = _time.Value;
					//instance.DateToUI();
					instance.TimeToUI();
				}
				instance.recentTimesBox.SelectedItem = null;
				instance.OnPropertyChanged(string.Empty);
				var rv = await instance.ShowAsync2();
				return rv;
			});
			
			return (dateTime, result == ContentDialogResult.Primary);
		}

		public DateTimePicker()
		{
			this.InitializeComponent();
			TimeToUI();
		}

		private static void AddRecentTime(DateTimeOffset _dateTime)
		{
			var s = _dateTime.ToString(AUtil.fullDateFormat, DateTimeFormatInfo.InvariantInfo);

			var i = recentTimes.IndexOf(s);
			if (i != -1)
				recentTimes.RemoveAt(i);

			recentTimes.Insert(0, s);
			if (recentTimes.Count > 16)
				recentTimes.RemoveAt(recentTimes.Count - 1);
		}


		private void date_SelectedDatesChanged(Microsoft.UI.Xaml.Controls.CalendarView sender, Microsoft.UI.Xaml.Controls.CalendarViewSelectedDatesChangedEventArgs args)
		{
			if (pauseChange == 0)
			{
				if (args.AddedDates.Any())
				{
					// Replace date
					var _date = args.AddedDates.First();// -AUtil.localTimeOffset;
					Log(_date);
					dateTime = new DateTimeOffset(_date.Year, _date.Month, _date.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, TimeSpan.Zero);
					TimeToUI();// Log(dateTime);
				}
			}
		}

		private void NowClick(object sender, RoutedEventArgs e)
		{
			dateTime = CnVServer.ServerTime();
			TimeToUI();
//			Log(dateTime);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
		//	if(propertyName== nameof() )
		//		DateToUI();
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void ComboBox_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
		{
			if (recentTimesBox.SelectedItem != null )
			{
				var sel = recentTimesBox.SelectedItem.ToString();
				if (DateTimeOffset.TryParseExact(sel, AUtil.fullDateFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal, out dateTime))
				{
					TimeToUI();
					OnPropertyChanged(string.Empty);
				}
			}
		}



		private void Time_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			if (e.Key == Windows.System.VirtualKey.Enter)
			{
				if (TimeFromUI())
				{
					//DefaultButton = ContentDialogButton.Primary;
					try
					{
						++disableFocusNotification;
						//for (int i = 0; i < 3; ++i)
						//{// Go to the "Done" button
						//	var temp = FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
						//	Assert(temp);
						//}
					}
					finally 
					{ 
						--disableFocusNotification;
					}
				}
				else
				{
					e.Handled = true;
				}
			}
		}

		private void time_LostFocus(object sender, RoutedEventArgs e)
		{
			if(disableFocusNotification==0)
				TimeFromUI();
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, Microsoft.UI.Xaml.Controls.ContentDialogButtonClickEventArgs args)
		{
			AddRecentTime(dateTime);
		}
	}
}
