﻿using System;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COTG.Views
{
    public sealed partial class FirstRunDialog : ContentDialog
    {
        public FirstRunDialog()
        {
            // TODO WTS: Update the contents of this dialog with any important information you want to show when the app is used for the first time.
            RequestedTheme = (App.window.Content as FrameworkElement).RequestedTheme;
            InitializeComponent();
        }
    }
}