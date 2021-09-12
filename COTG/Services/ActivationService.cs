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
using COTG.JSON;
using Windows.System;
using Microsoft.Web.WebView2.Core;


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

        public async Task ActivateAsync(IActivatedEventArgs activationArgs, bool wasRunning)
        {
			//App.globalQueue = DispatcherQueue.GetForCurrentThread();
			Assert(IsInteractive(activationArgs));
			if(!wasRunning)
				await InitializeAsync();
			AAnalytics.Track("Activate",new[] { { "kind":activationArgs.Kind.ToString() },
				{ "prior": activationArgs.Kind.ToString() },
				{"args" : activationArgs switch 
				{ ILaunchActivatedEventArgs a=>a.Arguments,
					IProtocolActivatedEventArgs p=>p.Uri.ToString(),
					_=>"{}"} }  );
			if(IsInteractive(activationArgs))
            {
                // Initialize services that you need before app activation
                // take into account that the splash screen is shown while this code runs.
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


			}
		}

        private async Task InitializeAsync()
        {
			// TODO restore       await Singleton<LiveTileService>.Instance.EnableQueueAsync().ConfigureAwait(false);
			// TODO restore       await Singleton<BackgroundTaskService>.Instance.RegisterBackgroundTasksAsync().ConfigureAwait(false);
			var t2 =  BuildingDef.Init();
			var t3 =  TroopInfo.Init();
			await Task.WhenAll(t2,t3);
			SettingsPage.Initialize();
			var t1 = WindowManagerService.Current.InitializeAsync();
			var t0 = ThemeSelectorService.InitializeAsync();
			await Task.WhenAll(t0, t1);
            Window.Current.Closed += async (a,b)=>
			{
				Debug.Log("Closed!");
				await TabPage.CloseAllTabWindows(); };
		//	App.webEnvironment = await CoreWebView2Environment.CreateAsync();
		//	App.webController = await App.webEnvironment.CreateCoreWebView2ControllerAsync();
		//	App.webCore = App.webController.CoreWebView2;
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

        private bool IsInteractive(IActivatedEventArgs args)
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
