﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COTG.Services
{
//    public delegate void ViewClosedHandler(ViewLifetimeControl viewControl, EventArgs e);

    // For instructions on using this service see https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/features/multiple-views.md
    // More details about showing multiple views at https://docs.microsoft.com/windows/uwp/design/layout/show-multiple-views
    public class WindowManagerService
    {
        private static WindowManagerService _current;

        public static WindowManagerService Current => _current ?? (_current = new WindowManagerService());

		//      // Contains all the opened secondary views.
		//      public ObservableCollection<ViewLifetimeControl> SecondaryViews { get; } = new ObservableCollection<ViewLifetimeControl>();

		//      public int MainViewId { get; private set; }

		//      public CoreDispatcher MainDispatcher { get; private set; }

		//      public async Task InitializeAsync()
		//      {
		//          await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
		//          {
		//              MainViewId = ApplicationView.GetForCurrentView().Id;
		//              MainDispatcher = Window.Current.Dispatcher;
		//          });
		//}

		//Displays a view as a standalone

		// You can use the resulting ViewLifeTileControl to interact with the new window.

		//public async Task<ViewLifetimeControl> TryShowAsStandaloneAsync<PageType>(string windowTitle)
		//{
		//	ViewLifetimeControl viewControl = await CreateViewLifetimeControlAsync(windowTitle,typeof(PageType));
		//	SecondaryViews.Add(viewControl);
		//	viewControl.StartViewInUse();

		//	await ApplicationViewSwitcher.TryShowAsStandaloneAsync(viewControl.Id,ViewSizePreference.UseMore,ApplicationView.GetForCurrentView().Id,ViewSizePreference.UseMore);
		//	viewControl.StopViewInUse();
		//	return viewControl;
		//}

		//public Task TryShowAsStandaloneAsync<PageType>(string windowTitle)
		//{
		//	return Task.CompletedTask;

		//}
		//// Displays a view in the specified view mode
		//public async Task<ViewLifetimeControl> TryShowAsViewModeAsync(string windowTitle, Type pageType, ApplicationViewMode viewMode = ApplicationViewMode.Default)
		//{
		//    ViewLifetimeControl viewControl = await CreateViewLifetimeControlAsync(windowTitle, pageType);
		//    SecondaryViews.Add(viewControl);
		//    viewControl.StartViewInUse();
		//    await ApplicationViewSwitcher.TryShowAsViewModeAsync(viewControl.Id, viewMode);
		//    viewControl.StopViewInUse();
		//    return viewControl;
		//}

		//private async Task<ViewLifetimeControl> CreateViewLifetimeControlAsync(string windowTitle, Type pageType)
		//{
		//    ViewLifetimeControl viewControl = null;

		//    await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
		//    {
		//        viewControl = ViewLifetimeControl.CreateForCurrentView();
		//        viewControl.Title = windowTitle;
		//        viewControl.StartViewInUse();
		//        var frame = new Microsoft.UI.Xaml.Controls.Frame();
		//        frame.IsNavigationStackEnabled = false;
		//        frame.RequestedTheme = ThemeSelectorService.Theme;
		//        frame.Navigate(pageType, viewControl);
		//        Window.Current.Content = frame;
		//        Window.Current.Activate();
		//     //   ApplicationView.GetForCurrentView().Title = viewControl.Title;
		//    });

		//    return viewControl;
		//}

		//public bool IsWindowOpen(string windowTitle) => SecondaryViews.Any(v => v.Title == windowTitle);
	}
}
