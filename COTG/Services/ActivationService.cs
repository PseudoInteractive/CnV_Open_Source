using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using COTG.Activation;
using COTG.Services;
using COTG.Views;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using Windows.ApplicationModel.Activation;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls;

namespace COTG.Services
{
    // For more information on understanding and extending activation flow see
    // https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/UWP/activation.md
    internal class ActivationService
    {
		
		private readonly App _app;
        private readonly Type _defaultNavItem;
        private Lazy<UIElement> _shell;

        private object _lastActivationArgs;

//        private IdentityService IdentityService => Singleton<IdentityService>.Instance;

       // private UserDataService UserDataService => Singleton<UserDataService>.Instance;

        public ActivationService(App app, Type defaultNavItem, Lazy<UIElement> shell = null)
        {
            _app = app;
            _shell = shell;
            _defaultNavItem = defaultNavItem;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            if (IsInteractive(activationArgs))
            {
                // Initialize services that you need before app activation
                // take into account that the splash screen is shown while this code runs.
                await InitializeAsync();
            //    UserDataService.Initialize();
  //              await IdentityService.InitializeWithAadAndPersonalMsAccounts();
              
                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (Window.Current.Content == null)
                {
                    // Create a Shell or Frame to act as the navigation context
                    Window.Current.Content = _shell?.Value ?? new Frame();
                }
            }

            // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
            // will navigate to the first page
            await HandleActivationAsync(activationArgs);
            _lastActivationArgs = activationArgs;

			if (IsInteractive(activationArgs))
			{
				var activation = activationArgs as IActivatedEventArgs;
				if (activation.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//        await Singleton<SuspendAndResumeService>.Instance.RestoreSuspendAndResumeData();
				}

				// Ensure the current window is active
				Window.Current.Activate();

				// Tasks after activation
				await StartupAsync();

				//MediaPlayer mediaPlayer = new MediaPlayer();

				//mediaPlayer.Source = MediaSource.CreateFromUri(new Uri(UriString));
				//mediaPlayer.Play();

			}
		}

        private async Task InitializeAsync()
        {
            // TODO restore       await Singleton<LiveTileService>.Instance.EnableQueueAsync().ConfigureAwait(false);
            // TODO restore       await Singleton<BackgroundTaskService>.Instance.RegisterBackgroundTasksAsync().ConfigureAwait(false);
            SettingsPage.Initialize();
            var t0= ThemeSelectorService.InitializeAsync();
            var t1= WindowManagerService.Current.InitializeAsync();
            await Task.WhenAll(t0, t1);
            Window.Current.Closed += async (a,b)=> await TabPage.CloseAllTabWindows();
        }

        private async Task HandleActivationAsync(object activationArgs)
        {
            var activationHandler = GetActivationHandlers()
                                                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (IsInteractive(activationArgs) && _defaultNavItem!=null)
            {
                //var defaultHandler = new DefaultActivationHandler(_defaultNavItem);
                //if (defaultHandler.CanHandle(activationArgs))
                //{
                //    await defaultHandler.HandleAsync(activationArgs);
                //}
            }
        }

        private async Task StartupAsync()
        {
            // TODO WTS: This is a sample to demonstrate how to add a UserActivity. Please adapt and move this method call to where you consider convenient in your app.
            // TODO restore       await UserActivityService.AddSampleUserActivity();
   //         Application.ThreadException += (sender, args) =>
			//{
			//	Crashes.TrackError(args.Exception);
			//};
			if (!AppCenter.Configured)
			{
				AppCenter.SetMaxStorageSizeAsync(16 * 1024 * 1024).ContinueWith((storageTask) => {
					// The storageTask.Result is false when the size cannot be honored.
				});
				AppCenter.LogLevel = System.Diagnostics.Debugger.IsAttached ? Microsoft.AppCenter.LogLevel.Warn : Microsoft.AppCenter.LogLevel.Error;
				AppCenter.Start("0b4c4039-3680-41bf-b7d7-685eb68e21d2",
				   typeof(Analytics), typeof(Crashes));
				await Crashes.SetEnabledAsync(true);
				await Analytics.SetEnabledAsync(true);
				bool didAppCrash = await Crashes.HasCrashedInLastSessionAsync();
				if (didAppCrash)
				{
					ErrorReport crashReport = await Crashes.GetLastSessionCrashReportAsync();
				}
			}
	//		await ThemeSelectorService.SetRequestedThemeAsync();

			// TODO WTS: Configure and enable Azure Notification Hub integration.
			//  1. Go to the HubNotificationsService class, in the InitializeAsync() method, provide the Hub Name and DefaultListenSharedAccessSignature.
			//  2. Uncomment the following line (an exception will be thrown if it is executed and the above information is not provided).
			// await Singleton<HubNotificationsService>.Instance.InitializeAsync().ConfigureAwait(false);
			///  Singleton<LiveTileService>.Instance.SampleUpdate();
			// TODO restore     await FirstRunDisplayService.ShowIfAppropriateAsync();
			// TODO restore        await WhatsNewDisplayService.ShowIfAppropriateAsync();
		}

		private IEnumerable<ActivationHandler> GetActivationHandlers()
        {
            // TODO restore        yield return Singleton<LiveTileService>.Instance;
            // TODO restore     yield return Singleton<HubNotificationsService>.Instance;
            yield return ToastNotificationsService.instance;
          //  yield return Singleton<ShareTargetActivationHandler>.Instance;
            // TODO restore    yield return Singleton<BackgroundTaskService>.Instance;
//            yield return Singleton<SuspendAndResumeService>.Instance;
           // yield return Singleton<WebToAppLinkActivationHandler>.Instance;
          //  yield return Singleton<SchemeActivationHandler>.Instance;
           // yield return Singleton<CommandLineActivationHandler>.Instance;
        }

        private bool IsInteractive(object args)
        {
            return args is IActivatedEventArgs;
        }

        internal async Task ActivateFromShareTargetAsync(ShareTargetActivatedEventArgs activationArgs)
        {
            var shareTargetHandler = GetActivationHandlers().FirstOrDefault(h => h.CanHandle(activationArgs));
            if (shareTargetHandler != null)
            {
                await shareTargetHandler.HandleAsync(activationArgs);
            }
        }
    }
}
