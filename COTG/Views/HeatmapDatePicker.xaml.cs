﻿using System;
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


namespace COTG.Views
{
    public sealed partial class HeatmapDatePicker : UserControl
    {
		public static string [] items;
        public static HeatmapDatePicker Touch()
		{
			if (instance == null)
				instance = new HeatmapDatePicker();
			return instance;
		}
		public static HeatmapDatePicker instance;

        public HeatmapDatePicker()
        {
            this.InitializeComponent();
        }

        private void Now_Click(object sender, RoutedEventArgs e)
        {
            snapshots.SelectedItems.Clear();
			AGame.ClearHeatmap();
        }

		private void snapshots_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var lv = sender as ListView;
			var sel = lv.SelectedItems.Select((a) => Array.IndexOf(items,(string)a)).OrderBy(a=>a).ToArray();


			if (sel.Length > 0)
            {
                Services.WorldStorage.SetHeatmapDates(sel); // Is Timezone Right?
            }
            else
            {
				AGame.ClearHeatmap();
            }
        }

		
		
	}
}
