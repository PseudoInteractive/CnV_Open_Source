﻿using COTG.JSON;
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
    public sealed partial class HitTab : UserTab
    {
        public static HitTab instance;
        public static bool IsVisible() => instance.isVisible;
        public Report[] history { get; set; } = Array.Empty<Report>();
        public void SetHistory(Report[] _history)
        {
            history = _history;
            historyGrid.ItemsSource = history;
        }
        public HitTab()
        {
            instance = this;
            this.InitializeComponent();
        }
        override public void VisibilityChanged(bool visible)
        {
            historyGrid.ItemsSource = Array.Empty<Report>();
            if (visible)
                OutgoingOverview.Process(SettingsPage.fetchFullHistory); // Todo: throttle
            base.VisibilityChanged(visible);


        }
    }
}