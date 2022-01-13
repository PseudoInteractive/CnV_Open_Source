using System;
using System.Threading.Tasks;

using CnV.Helpers;

using Windows.ApplicationModel.Core;
using Windows.Storage;
//using Windows.UI.Core;
using Microsoft.UI.Xaml;

namespace CnV.Services
{
    public static class ThemeSelectorService
    {
        private const string SettingsKey = "AppBackgroundRequestedTheme";

        public static ElementTheme Theme = ElementTheme.Dark;

        public static async Task InitializeAsync()
        {
            Theme = await LoadThemeFromSettingsAsync();
        }

        public static async Task SetThemeAsync(ElementTheme theme)
        {
            Theme = theme;
            App.ClientSettings().Save(SettingsKey, theme);

            await SetRequestedThemeAsync();
        }

        public static async Task SetRequestedThemeAsync()
        {
            AppS.QueueOnUIThread( ()=>
				{    if (App.window.Content is FrameworkElement frameworkElement)
                    {
                        frameworkElement.RequestedTheme = Theme;
                    }
                });
        }

		//public static Debounce RefreshXaml = new(_RefreshXaml) { debounceDelay = 100, throttleDelay = 100 };
		//public static async Task _RefreshXaml()
		//{
		//	var theme = Theme;
		//	var tempTheme = theme == ElementTheme.Light ? ElementTheme.Dark : ElementTheme.Light;
		//	await AppS.DispatchOnUIThreadTask(()=>
		//	{
		//		foreach (var view in CoreApplication.Views)
		//		{
		//			if (Window.Current.Content is FrameworkElement frameworkElement)
		//			{
		//				frameworkElement.RequestedTheme = tempTheme;
		//			}
		//		}
		//		return Task.CompletedTask;
		//	}	);
		//	await Task.Delay(200);
		//	await AppS.DispatchOnUIThreadTask(() =>
		//	{
		//		foreach (var view in CoreApplication.Views)
		//		{
		//			if (Window.Current.Content is FrameworkElement frameworkElement)
		//			{
		//				frameworkElement.RequestedTheme = Theme;
		//			}
		//		}
		//		return Task.CompletedTask;
		//	});
		//}


		private static async Task<ElementTheme> LoadThemeFromSettingsAsync()
        {
            ElementTheme cacheTheme = ElementTheme.Dark;
            string themeName = App.ClientSettings().Read<string>(SettingsKey);
            if (!string.IsNullOrEmpty(themeName))
            {
                Enum.TryParse(themeName, out cacheTheme);
            }

            return cacheTheme;
        }

       
    }
}
