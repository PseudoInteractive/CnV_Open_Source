using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CnV.Activation;
using CnV.Services;
using CnV.Views;

#if APPCENTER

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
#endif
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
///using Microsoft.Web.WebView2.Core;
using Microsoft.UI.Xaml.Media;

namespace CnV.Services
{
	using System.Runtime.CompilerServices;
	using Activation;
	using GameData;

	using Microsoft.UI.Windowing;
	// using PInvoke
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

  //      public async Task ActivateAsync(IActivatedEventArgs activationArgs, bool wasRunning)
  //      {
		//	//AppS.globalQueue = DispatcherQueue.GetForCurrentThread();
			
		//}

  //      private async Task InitializeAsync()
  //      {
		//	// TODO restore       await Singleton<LiveTileService>.Instance.EnableQueueAsync().ConfigureAwait(false);
		//	// TODO restore       await Singleton<BackgroundTaskService>.Instance.RegisterBackgroundTasksAsync().ConfigureAwait(false);
		//	try
		//	{
		//		var t2 = BuildingDef.Init();
		//		var t3 = CnV.Data.TroopInfo.Init();
			
		//		//var t4 = App.EnsureBrowserInstalledAsync();
		//		var t5 = CnV.Data.Boss.Init();
		//		Settings.Initialize(); // this is the long one


		//		await Task.WhenAll(t2,t3,t5);
		//	}
		//	catch(Exception ex)
		//	{
		//		Debug.LogEx(ex);
		//	}

		//	//			var t1 = WindowManagerService.Current.InitializeAsync();
		//	//	var t0 = ThemeSelectorService.InitializeAsync();
		//	//	await Task.WhenAll(t0);
		//	//         Window.Current.Closed += async (a,b)=>
		//	//{
		//	//	Debug.Log("Window!!Closed!");
		//	//	await TabPage.CloseAllTabWindows(); 
		//	//};
		//	// thread??


		//	//if(AGame.colorKind != Windows.Graphics.Display.AdvancedColorKind.HighDynamicRange)
		//	//{
		//	//	if(colorInfo.IsAdvancedColorKindAvailable(Windows.Graphics.Display.AdvancedColorKind.HighDynamicRange))
		//	//	{
		//	//		if(!Settings.askedToHdr)
		//	//		{
		//	//			Settings.askedToHdr=true;
		//	//			AppS.DoYesNoBox("HDR Available", "Tip: You can enable HDR for your monitor in Settings", "Thank you", null,null );
		//	//		}

		//	//	}	
		//	//}

		//}

        //private async Task HandleActivationAsync(IActivatedEventArgs activationArgs)
        //{
        //    var activationHandler = GetActivationHandlers()
        //                                        .FirstOrDefault(h => h.CanHandle(activationArgs));

        //    if (activationHandler != null)
        //    {
        //        await activationHandler.HandleAsync(activationArgs);
        //    }

        //    //if (IsInteractive(activationArgs) && _defaultNavItem!=null)
        //    //{
        //    //    //var defaultHandler = new DefaultActivationHandler(_defaultNavItem);
        //    //    //if (defaultHandler.CanHandle(activationArgs))
        //    //    //{
        //    //    //    await defaultHandler.HandleAsync(activationArgs);
        //    //    //}
        //    //}
        //}

  
//		private IEnumerable<ActivationHandler> GetActivationHandlers()
//        {
//            // TODO restore        yield return Singleton<LiveTileService>.Instance;
//            // TODO restore     yield return Singleton<HubNotificationsService>.Instance;
//            yield return ToastNotificationsService.instance;
//          //  yield return Singleton<ShareTargetActivationHandler>.Instance;
//            // TODO restore    yield return Singleton<BackgroundTaskService>.Instance;
////            yield return Singleton<SuspendAndResumeService>.Instance;
//           // yield return Singleton<WebToAppLinkActivationHandler>.Instance;
//          //  yield return Singleton<SchemeActivationHandler>.Instance;
//           // yield return Singleton<CommandLineActivationHandler>.Instance;
//        }

        //private bool IsInteractive(IActivatedEventArgs args)
        //{
        //    return args is IActivatedEventArgs;
        //}

        //internal async Task ActivateFromShareTargetAsync(ShareTargetActivatedEventArgs activationArgs)
        //{
        //    var shareTargetHandler = GetActivationHandlers().FirstOrDefault(h => h.CanHandle(activationArgs));
        //    if (shareTargetHandler != null)
        //    {
        //        await shareTargetHandler.HandleAsync(activationArgs);
        //    }
        //}
    }

	}
