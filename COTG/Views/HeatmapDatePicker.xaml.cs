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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace COTG.Views
{
    public sealed partial class HeatmapDatePicker : UserControl
    {
        public static HeatmapDatePicker instance;
        public HeatmapDatePicker()
        {
            instance = this;
            this.InitializeComponent();
        }

        private void Now_Click(object sender, RoutedEventArgs e)
        {
            date.SelectedDates.Clear();
            ShellPage.ClearHeatmap();
        }

        private void date_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if(args.AddedDates.Any())
            {
                Services.WorldStorage.SetHeatmapStartDate(args.AddedDates.First().Date); // Is Timezone Right?
            }
            else
            {
                ShellPage.ClearHeatmap();
            }
        }

        
        public static void SetFirstRecordedHeatmapDate(DateTimeOffset off)
        {
            App.DispatchOnUIThreadLow(() =>
            {
                instance.date.MinDate = (off);
                instance.date.MaxDate = (JSClient.ServerTime() + TimeSpan.FromDays(1));
            });
        }
    }
}
