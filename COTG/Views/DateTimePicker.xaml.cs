using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace COTG.Views
{
    public sealed partial class DateTimePicker : ContentDialog
    {
        public DateTimeOffset dateTime;
        public int seconds
        {
            get => dateTime.Second;
            set => dateTime = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, value, new TimeSpan());
        }
        public TimeSpan time
        {
            get => dateTime - dateTime.Date; // Does not zero out seconds, hopefully that is okay
            set => dateTime = dateTime.Date + value + TimeSpan.FromSeconds(dateTime.Second);
        }
       

        public DateTimePicker(DateTimeOffset i,string title)
        {
            dateTime = i;
            this.InitializeComponent();
            if(title != null)
                Title = title;
            date.SelectedDates.Clear();
            date.SelectedDates.Add(i);
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
        private void date_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (args.AddedDates.Any())
            {
                var _date = args.AddedDates.First().Date;
                dateTime = new DateTimeOffset(_date.Year, _date.Month, _date.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, new TimeSpan());
            }
           
        }
    }
}
