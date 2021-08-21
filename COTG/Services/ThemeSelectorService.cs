using System;
using System.Threading.Tasks;

using COTG.Helpers;

using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace COTG.Services
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
            App.Settings().Save(SettingsKey, theme);

            await SetRequestedThemeAsync();
        }

        public static async Task SetRequestedThemeAsync()
        {
            foreach (var view in CoreApplication.Views)
            {
                await view.Dispatcher.TryRunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (Window.Current.Content is FrameworkElement frameworkElement)
                    {
                        frameworkElement.RequestedTheme = Theme;
                    }
                });
            }
        }
		public static Debounce RefreshXaml = new(_RefreshXaml) { debounceDelay = 100, throttleDelay = 100 };
		public static async Task _RefreshXaml()
		{
			var theme = Theme;
			var tempTheme = theme == ElementTheme.Light ? ElementTheme.Dark : ElementTheme.Light;
			await App.DispatchOnUIThreadTask(()=>
			{
				foreach (var view in CoreApplication.Views)
				{
					if (Window.Current.Content is FrameworkElement frameworkElement)
					{
						frameworkElement.RequestedTheme = tempTheme;
					}
				}
				return Task.CompletedTask;
			}	);
			await Task.Delay(200);
			await App.DispatchOnUIThreadTask(() =>
			{
				foreach (var view in CoreApplication.Views)
				{
					if (Window.Current.Content is FrameworkElement frameworkElement)
					{
						frameworkElement.RequestedTheme = Theme;
					}
				}
				return Task.CompletedTask;
			});
		}


		private static async Task<ElementTheme> LoadThemeFromSettingsAsync()
        {
            ElementTheme cacheTheme = ElementTheme.Dark;
            string themeName = App.Settings().Read<string>(SettingsKey);
            if (!string.IsNullOrEmpty(themeName))
            {
                Enum.TryParse(themeName, out cacheTheme);
            }

            return cacheTheme;
        }

       
    }
}
