﻿using System;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CnV.Views
{
    public sealed partial class WhatsNewDialog : ContentDialog
    {
        public WhatsNewDialog()
        {
            // TODO WTS: Update the contents of this dialog every time you release a new version of the app
        //    RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            InitializeComponent();
        }

		private void ContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
		{
			scrollView.ChangeView(null, scrollView.ScrollableHeight, null);
		}
	}
}
