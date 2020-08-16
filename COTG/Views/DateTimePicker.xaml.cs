using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static COTG.Debug;
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace COTG.Views
{
    public sealed partial class DateTimePicker : ContentDialog,INotifyPropertyChanged
    {
        byte pauseChange;
        public static DateTimeOffset dateTime;
        DateTimeOffset saved;
        public static List<string> recentTimes = new List<String>();
        
        public int seconds
        {
            get => dateTime.Second;
            set => dateTime = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, value.Clamp(0,59), TimeSpan.Zero);
        }

        public TimeSpan time
        {
            get => TimeSpan.FromHours(dateTime.Hour) + TimeSpan.FromMinutes(dateTime.Minute); // Does not zero out seconds, hopefully that is okay
            set => dateTime = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, value.Hours, value.Minutes, dateTime.Second, TimeSpan.Zero);
        }

        private void UpdateDate()
        {
            ++pauseChange;
            var localDateTime = dateTime - AUtil.localTimeOffset;
            var localDate = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, AUtil.localTimeOffset);
            //if (date.SelectedDates.Count == 1)
            //{
            //    date.SelectedDates[0] = localDate;
            //}
            //else
            {
                date.SelectedDates.Clear();
                date.SelectedDates.Add(localDate);
            }
            --pauseChange;
        }

        public DateTimePicker(DateTimeOffset i,string title)
        {
            saved = i;
            this.InitializeComponent();
            if(title != null)
                Title = title;
            UpdateDate();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var s = dateTime.ToString(AUtil.fullDateFormat, DateTimeFormatInfo.InvariantInfo);
            Log(s);
            Log(dateTime);
            if (!recentTimes.Contains(s))
            {
                recentTimes.Insert(0, s);
                if (recentTimes.Count > 8)
                    recentTimes.RemoveAt(recentTimes.Count - 1);
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            dateTime = saved;
        }

        private void date_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (pauseChange == 0)
            {
                if (args.AddedDates.Any())
                {
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
            Log(dateTime);
            OnPropertyChanged(string.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            UpdateDate();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Any())
            {
                var sel = e.AddedItems.First() as string;
                if (DateTimeOffset.TryParseExact(sel,AUtil.fullDateFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AssumeUniversal, out dateTime))
                {
                    OnPropertyChanged(string.Empty);
                }
            }
        }
    }
}
