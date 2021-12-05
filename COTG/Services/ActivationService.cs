using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CnV.Activation;
using CnV.Services;
using CnV.Views;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;

using Windows.ApplicationModel.Activation;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;

using Windows.System;
using Microsoft.Web.WebView2.Core;
using Microsoft.UI.Xaml.Media;

namespace CnV.Services
{
	using System.Runtime.CompilerServices;
	using Activation;
	using PInvoke;
	using Views;

	// For more information on understanding and extending activation flow see
    // https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/UWP/activation.md
    internal class ActivationService
    {
		
//		private readonly App _app;
   //    private readonly Type _defaultNavItem;
   //     private Lazy<UIElement> _shell;

  //      private object _lastActivationArgs;

//        private IdentityService IdentityService => Singleton<IdentityService>.Instance;

       // private UserDataService UserDataService => Singleton<UserDataService>.Instance;

        //public ActivationService(App app, Type defaultNavItem, Lazy<UIElement> shell = null)
        //{
        //    _app = app;
        //    _shell = shell;
        //    _defaultNavItem = defaultNavItem;
        //}

        public async Task ActivateAsync(IActivatedEventArgs activationArgs, bool wasRunning)
        {
			//AppS.globalQueue = DispatcherQueue.GetForCurrentThread();
			Debug.Assert(IsInteractive(activationArgs));
			if(!wasRunning)
				await InitializeAsync();



			AAnalytics.Track("Activate",new Dictionary<string,string> { { "kind",activationArgs.Kind.ToString() },
				{ "prior", activationArgs.Kind.ToString() },
				{"args" , activationArgs switch 
				{ ILaunchActivatedEventArgs a=>a.Arguments,
					IProtocolActivatedEventArgs p=>p.Uri.ToString(),
					_=>"{}"} } }  );
			if(IsInteractive(activationArgs))
            {
                // Initialize services that you need before app activation
                // take into account that the splash screen is shown while this code runs.
            //    UserDataService.Initialize();
  //              await IdentityService.InitializeWithAadAndPersonalMsAccounts();
              
                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (App.window.Content == null)
                {
					// Create a Shell or Frame to act as the navigation context
				//	App.instance.Resources["TabViewBackground"] = new SolidColorBrush();
				//	App.instance.Resources["TabViewButtonBackground"] = new SolidColorBrush();
				//	App.instance.Resources["TabViewButtonForeground"] = new SolidColorBrush();
				//	App.instance.Resources["OverlayCornerRadius"] = 1.0;
				//	App.instance.Resources["TopCornerRadiusFilterConverter"] = new object();
					
					App.window.Content = new ShellPage();

				}
            }

            // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
            // will navigate to the first page
            await HandleActivationAsync(activationArgs);
          //  _lastActivationArgs = activationArgs;

			if (IsInteractive(activationArgs))
			{
				var activation = activationArgs as IActivatedEventArgs;
				if (activation.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//        await Singleton<SuspendAndResumeService>.Instance.RestoreSuspendAndResumeData();
				}
				// Ensure the current window is active
				await Task.Delay(500);
				Log("Activate!");
				App.window.Activate();
				Log("Activate!Done");
				await Task.Delay(500);
				Log("Max");
//				App.window.Maximize();
				_ = PInvoke.User32.ShowWindow(WinRT.Interop.WindowNative.GetWindowHandle(App.window), PInvoke.User32.WindowShowStyle.SW_MAXIMIZE);
				//User32.WINDOWPLACEMENT placement = new()
				//{
				//	length = Unsafe.SizeOf<User32.WINDOWPLACEMENT>(),
				//	flags = User32.WindowPlacementFlags.WPF_RESTORETOMAXIMIZED
				//	        | User32.WindowPlacementFlags.WPF_ASYNCWINDOWPLACEMENT,
				//	showCmd = User32.WindowShowStyle.SW_SHOWMAXIMIZED
				//};
			//	_ = PInvoke.User32.SetWindowPlacement(WinRT.Interop.WindowNative.GetWindowHandle(App.window), placement);
				;

			}
		}

        private async Task InitializeAsync()
        {
			// TODO restore       await Singleton<LiveTileService>.Instance.EnableQueueAsync().ConfigureAwait(false);
			// TODO restore       await Singleton<BackgroundTaskService>.Instance.RegisterBackgroundTasksAsync().ConfigureAwait(false);
			try
			{
				var t2 = BuildingDef.Init();
				var t3 = TroopInfo.Init();
				var t4 = App.StartAnalyticsAsync();
				SettingsPage.Initialize();


				await Task.WhenAll(t2,t3,t4);
			}
			catch(Exception ex)
			{
				Debug.LogEx(ex);
			}

			//			var t1 = WindowManagerService.Current.InitializeAsync();
			//	var t0 = ThemeSelectorService.InitializeAsync();
			//	await Task.WhenAll(t0);
			//         Window.Current.Closed += async (a,b)=>
			//{
			//	Debug.Log("Window!!Closed!");
			//	await TabPage.CloseAllTabWindows(); 
			//};
			// thread??


			//if(AGame.colorKind != Windows.Graphics.Display.AdvancedColorKind.HighDynamicRange)
			//{
			//	if(colorInfo.IsAdvancedColorKindAvailable(Windows.Graphics.Display.AdvancedColorKind.HighDynamicRange))
			//	{
			//		if(!SettingsPage.askedToHdr)
			//		{
			//			SettingsPage.askedToHdr=true;
			//			AppS.DoYesNoBox("HDR Available", "Tip: You can enable HDR for your monitor in Settings", "Thank you", null,null );
			//		}

			//	}	
			//}

		}

        private async Task HandleActivationAsync(IActivatedEventArgs activationArgs)
        {
            var activationHandler = GetActivationHandlers()
                                                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            //if (IsInteractive(activationArgs) && _defaultNavItem!=null)
            //{
            //    //var defaultHandler = new DefaultActivationHandler(_defaultNavItem);
            //    //if (defaultHandler.CanHandle(activationArgs))
            //    //{
            //    //    await defaultHandler.HandleAsync(activationArgs);
            //    //}
            //}
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
