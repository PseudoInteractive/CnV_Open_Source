using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

using static COTG.Debug;
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
using ContentDialog = Windows.UI.Xaml.Controls.ContentDialog;
using ContentDialogResult = Windows.UI.Xaml.Controls.ContentDialogResult;
namespace COTG.Views
{
	public sealed partial class DateTimePicker : ContentDialog, INotifyPropertyChanged
	{
		public static DateTimePicker instance = new DateTimePicker();
		byte pauseChange;
		byte disableFocusNotification;
		public static DateTimeOffset dateTime;
		public static List<string> recentTimes = new List<String>();

		void TimeToUI()  { time.Text = dateTime.FormatTimeDefault(); DateToUI(); } // Does not zero out seconds, hopefully that is okay
		bool TimeFromUI()
		{
			var rv = false;
			if (
			
				  DateTimeOffset.TryParseExact(time.Text, "HH", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AssumeUniversal, out var result)||
				  DateTimeOffset.TryParseExact(time.Text, "HH':'mm", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AssumeUniversal, out result) ||
				DateTimeOffset.TryParseExact(time.Text, AUtil.defaultTimeFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowInnerWhite |
				DateTimeStyles.AssumeUniversal, out result) )
				   
			{
				dateTime = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, result.Hour, result.Minute, result.Second, TimeSpan.Zero);
				rv = true;

			}
			else if (
				 DateTimeOffset.TryParseExact(time.Text, AUtil.defaultDateFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AssumeUniversal, out result))
			{
				// all except year
				dateTime = new DateTimeOffset(dateTime.Year, result.Month, result.Day, result.Hour, result.Minute, result.Second, TimeSpan.Zero); // strip off timezone 
				rv = true;
			}
			else if(  // anything goes
				DateTimeOffset.TryParse(time.Text, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AssumeUniversal, out result)
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
		}

		public static async Task<(DateTimeOffset t, bool yes)> ShowAsync(string title, DateTimeOffset? _time=null)
		{
			var result = await App.DispatchOnUIThreadTask(async () =>
			{

				if (title != null)
					instance.Title = title;
				if (_time != null)
					dateTime = _time.Value;
				ElementSoundPlayer.Play(ElementSoundKind.Show);

				return  await instance.ShowAsync2();
			});
			return (dateTime, result == ContentDialogResult.Primary);
		}

		public DateTimePicker()
		{
			this.InitializeComponent();
			TimeToUI();
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, Windows.UI.Xaml.Controls.ContentDialogButtonClickEventArgs args)
		{
			var s = dateTime.ToString(AUtil.fullDateFormat, DateTimeFormatInfo.InvariantInfo);

			var i = recentTimes.IndexOf(s);
			if (i != -1)
				recentTimes.RemoveAt(i);

			recentTimes.Insert(0, s);
			if (recentTimes.Count > 16)
				recentTimes.RemoveAt(recentTimes.Count - 1);
		}


		private void date_SelectedDatesChanged(Windows.UI.Xaml.Controls.CalendarView sender, Windows.UI.Xaml.Controls.CalendarViewSelectedDatesChangedEventArgs args)
		{
			if (pauseChange == 0)
			{
				if (args.AddedDates.Any())
				{
					// Replace date
					var _date = args.AddedDates.First();// -AUtil.localTimeOffset;
					Log(_date);
					dateTime = new DateTimeOffset(_date.Year, _date.Month, _date.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, TimeSpan.Zero);
					Log(dateTime);
				}
			}
		}

		private void NowClick(object sender, RoutedEventArgs e)
		{
			dateTime = JSClient.ServerTime();
			TimeToUI();
			Log(dateTime);
			OnPropertyChanged(string.Empty);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			DateToUI();
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void ComboBox_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Any())
			{
				var sel = e.AddedItems.First() as string;
				if (DateTimeOffset.TryParseExact(sel, AUtil.fullDateFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AssumeUniversal, out dateTime))
				{
					TimeToUI();
					OnPropertyChanged(string.Empty);
				}
			}
		}



		private void Time_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			if (e.Key == Windows.System.VirtualKey.Enter)
			{
				if (TimeFromUI())
				{
					//DefaultButton = ContentDialogButton.Primary;
					try
					{
						++disableFocusNotification;
						for (int i = 0; i < 3; ++i)
						{// Go to the "Done" button
							var temp = FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
							Assert(temp);
			
						}
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
	}
}
