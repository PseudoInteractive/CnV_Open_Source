
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Helpers;

using CnV.Helpers;
using CnV.Services;

using WinRT;
#if AppCenter

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;

#endif
#if CRASHES
using Microsoft.AppCenter.Crashes;
#endif

using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using static CnV.AppS;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
//using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
//using Microsoft.Web.WebView2.Core;
//using ZLogger;
//using Cysharp.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Microsoft.AppCenter;
//using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Microsoft.UI.Input.Experimental;
using DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.Globalization.NumberFormatting;
using Windows.System;
//using Windows.UI.Core;
//using Windows.UI.Input;
using Microsoft.UI.Dispatching;
using DispatcherQueueHandler = Microsoft.UI.Dispatching.DispatcherQueueHandler;
using DispatcherQueuePriority = Microsoft.UI.Dispatching.DispatcherQueuePriority;
using ContentDialog = Microsoft.UI.Xaml.Controls.ContentDialog;
using ContentDialogResult = Microsoft.UI.Xaml.Controls.ContentDialogResult;
using MenuFlyout = Microsoft.UI.Xaml.Controls.MenuFlyout;
using MenuFlyoutItem = Microsoft.UI.Xaml.Controls.MenuFlyoutItem;
using MenuFlyoutSubItem = Microsoft.UI.Xaml.Controls.MenuFlyoutSubItem;
using ToggleMenuFlyoutItem = Microsoft.UI.Xaml.Controls.ToggleMenuFlyoutItem;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Hosting;
using Nito.AsyncEx;

using Microsoft.UI.Xaml.Input;
using System;
using Microsoft.Extensions.DependencyInjection;
//using Windows.UI.Core;
using System.Runtime.InteropServices;
using Microsoft.UI;
using WinRT;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

namespace CnV
{
	using CnV;
	using Game;
	using Helpers;

	using Microsoft.UI.Windowing;
	using Microsoft.Xna.Framework.Input;
	//// using PInvoke
	using Services;
	using Views;

	/// <summary>
	/// App
	/// </summary>
	public sealed partial class App : Application
	{
		public enum State
		{
			init,
			active,
			closing,
			closed,
		}

//		static IConfigurationRoot configuration;
		public  static      Window?           window => AppS.window;
		public static State                   state;
		private       Lazy<ActivationService> _activationService;
		public static bool                    processingTasksStarted;

		private ActivationService ActivationService
		{
			get { return _activationService.Value; }
		}


		public static App    instance;
		public static string appLink = "cotg";

		public static async Task EnsureBrowserInstalledAsync()
		{
#if AppCenter

			if(AppCenter.Configured)
			{
				return;
			}
			//AppCenter.SetMaxStorageSizeAsync(16 * 1024 * 1024).ContinueWith((storageTask) => {
			//	// The storageTask.Result is false when the size cannot be honored.
			//});

			AppCenter.Configure("0b4c4039-3680-41bf-b7d7-685eb68e21d2");
		//	AppCenter.LogLevel = System.Diagnostics.Debugger.IsAttached ? Microsoft.AppCenter.LogLevel.Warn : Microsoft.AppCenter.LogLevel.None;
			AppCenter.Start(
			   typeof(Analytics)
#if CRASHES
			   , typeof(Crashes)
#endif
			   );

			AAnalytics.initialized = true;
			await Task.WhenAll(
#if CRASHES
					Crashes.SetEnabledAsync(true),
#endif
								Analytics.SetEnabledAsync(true));

#endif


			//try
			//{
			//	var str = CoreWebView2Environment.GetAvailableBrowserVersionString();
			//	Log(str);
			//	//			createWebEnvironmentTask =  CoreWebView2Environment.CreateAsync();
			//	AAnalytics.Track("WebView",
			//					new Dictionary<string, string>(new []
			//					{
			//							new KeyValuePair<string, string>("Version", str)
			//					} ));
			//}
			//catch (Exception ex)
			//{
			//	await Windows.System.Launcher.LaunchUriAsync(new("https://go.microsoft.com/fwlink/p/?LinkId=2124703",
			//													UriKind.Absolute));
			//	LogEx(ex);
			//}
			//#if CRASHES
			//			bool didAppCrash = await Crashes.HasCrashedInLastSessionAsync();
			//			if (didAppCrash)
			//			{
			//				ErrorReport crashReport = await Crashes.GetLastSessionCrashReportAsync();
			//				Log(crashReport);
			//			}
			//#endif
		}


		public App()
		{
			//			services = ConfigureServices();
			RequestedTheme = ApplicationTheme.Dark;
			InitializeComponent();
			UnhandledException += App_UnhandledException;



			//try
			//{
			//    {

			// ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
			//    }
			//}
			//catch (Exception e)
			//{
			//    Log(e);
			//}

			//	ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => { Log(certificate.ToString()); return true; };
			//	InitializeComponent();
			instance = this;

			UnhandledException += OnAppUnhandledException;
			//Microsoft.Extensions.Hosting.Host.Cre
			TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

			// TODO WTS: Add your app in the app center and set your secret here. More at https://docs.microsoft.com/appcenter/sdk/getting-started/uwp


			// Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
			_activationService = new Lazy<ActivationService>(CreateActivationService);
			//	UserAgent.SetUserAgent(CnVServer.userAgent);  // set webview useragent
			//	Ioc.Default.ConfigureServices(ConfigureServices());


		}
		//private System.IServiceProvider ConfigureServices()
		//{
		// Host.CreateDefaultBuilder().Build();
		//         // TODO WTS: Register your services, viewmodels and pages here
		//         var services = new ServiceCollection();
		//services.AddLogging();

		//         return services.BuildServiceProvider();
		//     }

		private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			System.Diagnostics.Debug.WriteLine($"Unhandled Exception: " + e.Message);
			System.Diagnostics.Debug.WriteLine(e.Exception.StackTrace);

		}

		private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			e.SetObserved();
			LogEx(e.Exception);

		}

		//		public static Windows.Foundation.IAsyncOperation<CoreWebView2Environment> createWebEnvironmentTask;

		private static async Task SwitchToBackground()
		{
			Log($"Background {isForeground}");
			if (isForeground == true)
			{
				isForeground = false;

				//TODO: Save application state and stop any background activity
				try
				{

					var t0 = SaveState();

					var t  = DateTimeOffset.UtcNow;
					var dt = t - activeStart;
					activeStart = t;
					//	Trace("Finished!1");

					AAnalytics.Track("Background",
									new Dictionary<string, string>
											{ { "time", dt.TotalSeconds.RoundToInt().ToString() } });
					SystemInformation.Instance.AddToAppUptime(dt);
					await t0;
					Log("Finished!");
				}
				catch (Exception ex)
				{
				}
			}
		}

		private static Task SaveState()
		{
			var t0 = BuildQueue.SaveAll(true, false);
			var t1 = AttackTab.SaveAttacksBlock();
			Settings.SaveAll();
			return Task.WhenAll(t0, t1);
		}


		// can only be called from UI thread
		//private static CoreVirtualKeyStates GetKeyState(VirtualKey key)
		//{
		//	var window = CoreWindow.GetForCurrentThread();
		//	if (window == null)
		//	{
		//		Assert(false);
		//		return CoreVirtualKeyStates.None;
		//	}

		//	return window.GetAsyncKeyState(key);
		//}
		//public static bool IsKeyDown(VirtualKey key)
		//{
		//	var state = GetKeyState(key);
		//	return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
		//}

	

		//static void OnKeyUp(CoreWindow sender, KeyEventArgs args)
		//{
		//	var key = args.VirtualKey;
		//	OnKeyUp(key);
		//}


		public static void OnKeyUp(VirtualKey key)
		{
			AppS.UpdateKeyStates();
			//Trace("KeyUp" + key);
			switch (key)
			{
				case VirtualKey.Shift:
				case VirtualKey.LeftShift:
				case VirtualKey.RightShift:
					//		Trace("Shift Up");
					shiftPressed = false;
					break;
				case VirtualKey.Control:
				case VirtualKey.LeftControl:
				case VirtualKey.RightControl:

					controlPressed = false;
					break;

			}

			InputRecieved();
		}

		//static bool webViewInFront = false;

		//static void OnKeyDown(CoreWindow sender, KeyEventArgs args)
		//{
		//	Note.Show("Key!");
		//	var key = args.VirtualKey;

		//	OnKeyDown(key);

		//}
		public static bool OnKeyDown(VirtualKey key)
		{
			//Log($"KeyDown {key} mouse:{ShellPage.mouseOverCanvas}");

			AppS.UpdateKeyStates();
			switch (key)
			{
				case VirtualKey.Shift:
				case VirtualKey.LeftShift:
				case VirtualKey.RightShift:
					//Trace("Shift Down");

					shiftPressed = true;
					break;
				case VirtualKey.Control:
				case VirtualKey.LeftControl:
				case VirtualKey.RightControl:

					controlPressed = true;
					break;

			}

			InputRecieved();

			return false; //ShellPage.DoKeyDown(key);

		}

		private void SwitchToForeground()
		{
			Log("Foreground");
			if (!isForeground)
			{
				isForeground = true;
				var t  = DateTimeOffset.UtcNow;
				var dt = t - activeStart;
				activeStart = t;
				AAnalytics.Track("Foreground",
								new Dictionary<string, string> { { "time", dt.TotalSeconds.RoundToInt().ToString() } });
				//	CnVServer.ResumeWebView();

			}
			//if (ShellPage.canvas != null)
			//    ShellPage.canvas.Paused = false;
		}

		static public int storageFull = 0;

		private void OnAppUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			try
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine($"Unhandled Exception: " + e.Message);
				System.Diagnostics.Debug.WriteLine(e.Exception.StackTrace);
#endif



				if (AppS.RegisterException(e.Message))
				{
#if CRASHES

					Crashes.TrackError(e.Exception);
#endif
					AAnalytics.Track("UnhandledException",
									new Dictionary<string, string> { { "message", e.Message.Truncate(64) } });
				}

			}
			catch (Exception ex2)
			{

				//LogEx(ex2);
				//					RegisterException(ex2.Message);


			}
		}




		static DateTimeOffset activeStart = DateTimeOffset.UtcNow;
		//private static Microsoft.Extensions.Configuration.IConfigurationRoot BuildConfig()
		//{
		//	var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

		//	var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) ||
		//						devEnvironmentVariable.ToLower() == "development";

		//	var builder = new ConfigurationBuilder();
		//	// tell the builder to look for the appsettings.json file
		//	builder
		//		.AddJsonFile("appsettings.json",optional: false,reloadOnChange: false);

		//	//only add secrets in development
		//	if(isDevelopment)
		//	{
		//		builder.AddUserSecrets<Program>();
		//	}

		//	return builder.Build();
		//}

		protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
		{
			try
			{



				//	Windows.UI.ViewManagement.ApplicationView.PreferredLaunchWindowingMode =Windows.UI.ViewManagement.ApplicationViewWindowingMode.Maximized;// new Size(bounds.Width, bounds.Height);
				//				Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterViewModeAsync(Windows.UI.ViewManagement.ApplicationViewMode.CompactOverlay);

				FocusVisualKind = FocusVisualKind.Reveal;

				AppS.window = new();
				//	window.



				//var view = DisplayInformation.GetForCurrentView();
				var uwpArgs = AppInstance.GetActivatedEventArgs(); //args.UWPLaunchActivatedEventArgs;
				if (uwpArgs.Kind == Windows.ApplicationModel.Activation.ActivationKind.Protocol)
				{
					var eventArgs = uwpArgs as ProtocolActivatedEventArgs;
					Log("Args!! " + eventArgs.Uri);
					var s = System.Web.HttpUtility.ParseQueryString(eventArgs.Uri.Query);

					Debug.Log(s);
					// format $"cnv:launch?w={world}&s=1&n=1"
					// are / chars inserted?
					//  if (s.Length >= 3)
					{
						//if (AMath.TryParseInt(s["s"], out int _s))
						//	CnVServer.subId = _s;

						////var n = s["p"];
						////if (n != null)
						////	Player.subOwner = n;

						//if (AMath.TryParseInt(s["w"], out int _w))
						//	CnVServer.world = _w;

//						if(AMath.TryParseInt(s["n"],out int _n)) // new instance
//							key = "cotgaMulti" + DateTimeOffset.UtcNow.UtcTicks;

					}
				}

				//// Get the screen resolution (APIs available from 14393 onward).
				//var resolution = new Size(view.ScreenWidthInRawPixels, view.ScreenHeightInRawPixels);
				//// Calculate the screen size in effective pixels. 
				//// Note the height of the Windows Taskbar is ignored here since the app will only be given the maxium available size.
				//var scale = view.ResolutionScale == ResolutionScale.Invalid ? 1 : view.RawPixelsPerViewPixel;
				//var bounds = new Size(resolution.Width / scale, resolution.Height / scale);

				{
					IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
					WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
					AppS.appWindow= AppWindow.GetFromWindowId(myWndId);
				}
				AppS.appWindow.Title = "Conquest and Virtue Alpha";

				//				
				//				window.SetTitleBar
				//ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
				//		window.ExtendsContentIntoTitleBar = true;
				//	window.ExtendsContentIntoTitleBar = true;
				//App.globalDispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
				AppS.globalQueue = window.DispatcherQueue;
				//	keyQueue = globalQueue.CreateTimer();
				//CoreApplication.EnablePrelaunch(false);
				if (uwpArgs.Kind == Windows.ApplicationModel.Activation.ActivationKind.Launch)
				{
					// do this asynchronously
					Services.StoreHelper.instance.DownloadAndInstallAllUpdatesAsync();
				}



				// if (!args.PrelaunchActivated)

				await OnLaunchedOrActivated(args.UWPLaunchActivatedEventArgs);
			}
			catch (Exception e)
			{
				Log(e);
			}
		}

		//private bool Window_Closing() //object sender,WindowClosingEventArgs e)
		//{
		//	Log("Closing!");
		//	if(state == State.closed)
		//		return true;
		//	if( state == State.closing)
		//		return false;
		//	state = State.closing;

		//	SwitchToBackground().ContinueWith( (_)=> 
		//		{
		//		state = State.closed;
		//		AppS.DispatchOnUIThread(window.Close);
		//	});
		//	return false;

		//}

		private void Window_Closed(object sender, WindowEventArgs args)
		{
//			BackgroundTask.dispatcherQueueController.ShutdownQueueAsync();

			Log($"Closed!  {isForeground}");
			//	Assert(state == State.closed);
			SwitchToBackground();
		}

		private void Content_PreviewKeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			OnKeyUp(e.Key);

		}

		private void Content_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			InputRecieved();
			//	Log($"PreviewKeyDown {e.Key} {e.Handled}");
			if (!e.Handled)
				e.Handled = OnKeyDown(e.Key);
		}

		private static void DebugSettings_BindingFailed(object sender, BindingFailedEventArgs e)
		{
			Log(e.Message);
		}


		private async Task OnLaunchedOrActivated(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
		{

			try
			{



#if DEBUG
//				this.DebugSettings.FailFastOnErrors = false;
				this.DebugSettings.FailFastOnErrors                      = true;
				this.DebugSettings.EnableFrameRateCounter                = false;
				this.DebugSettings.IsTextPerformanceVisualizationEnabled = false;
				//this.DebugSettings.FailFastOnErrors = false;
				this.DebugSettings.IsBindingTracingEnabled = true;
#endif
				var wasRunning = args.PreviousExecutionState   == ApplicationExecutionState.Running
								|| args.PreviousExecutionState == ApplicationExecutionState.Suspended;
				if (!wasRunning)
				{
					//	var window = Window.Current;
					window.VisibilityChanged += Window_VisibilityChanged;
					window.Closed            += Window_Closed;
					//		window.WantClose+=Window_Closing;
					//	window.Activated+=Window_Activated;
				}

				await ActivationService.ActivateAsync(args, wasRunning);

				if (wasRunning)
					return;
				window.Content.PreviewKeyUp   += Content_PreviewKeyUp;
				window.Content.PreviewKeyDown += Content_PreviewKeyDown;
				;
//			window.KeyDown+=Window_KeyDown;

				//			CoreApplication.MainView.HostedViewClosing+=MainView_HostedViewClosing; ;
				//	CoreApplication.MainView.CoreWindow.Closed+=CoreWindow_Closed;
				//if(args!=null)
				//	SystemInformation.TrackAppUse(args);
				if (processingTasksStarted == false)
				{
					processingTasksStarted = true;

					Task.Run(ProcessThrottledTasks);
					Task.Run(AppS.ProcessIdleTasks);
				}


				SystemInformation.Instance.TrackAppUse(args);
#if DEBUG
//			var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
//			coreTitleBar.ExtendViewIntoTitleBar = false;
//			var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;


				//		var color = Windows.UI.Color.FromArgb(0xFF, 0x20, 0x0, 0x35);
				//		var colorInactive = Windows.UI.Color.FromArgb(0xFF, 0x00, 0x0, 0x35);
				//		titleBar.BackgroundColor = color;
				//titleBar.ForegroundColor = color;
				//titleBar.ButtonForegroundColor = color;
//			titleBar.ButtonBackgroundColor = color;
//			titleBar.InactiveBackgroundColor = titleBar.ButtonInactiveBackgroundColor = colorInactive;
				//				titleBar.InactiveForegroundColor =  titleBar.ButtonInactiveForegroundColor = colorInactive;
				//titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;
				//  UpdateTitleBarLayout(coreTitleBar);
#endif


				// Set XAML element as a draggable region.
				//          Window.Current.SetTitleBar(ShellPage.instance.AppTitleBar);
			}
			catch (Exception e)
			{
				Log(e);
			}
		}



		//private void Window_Activated(object sender,WindowActivatedEventArgs args)
		//{
		//	Trace("Activated");
		////	SwitchToForeground();
		//}

		private async void Window_VisibilityChanged(object sender, WindowVisibilityChangedEventArgs args)
		{
			Log($"Visibility!!: {args.Visible}  {isForeground}");
			if (!args.Visible)
			{
				await SwitchToBackground();
			}
			else
			{
				SwitchToForeground();
			}


			//			throw new NotImplementedException();
		}

		//private void CoreWindow_Closed(CoreWindow sender,CoreWindowEventArgs args)
		//{
		//	Log("Close");
		//	state = State.closing;
		//	CnVServer.CloseWebView();
		//	TabPage.CloseAllTabWindows();
		//}

		//private async void MainView_HostedViewClosing(CoreApplicationView sender,HostedViewClosingEventArgs args)
		//{
		//	var defer = args.GetDeferral();
		//	Log("Close");
		//	state = State.closing;
		//	CnVServer.CloseWebView();
		//	await TabPage.CloseAllTabWindows();
		//}


		//protected override async void OnActivated(IActivatedEventArgs args)
		//{
		//	var activation = args as IActivatedEventArgs;
		//	globalQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
		//	if (activation != null && activation.PreviousExecutionState == ApplicationExecutionState.Running)
		//	{
		//		Window.Current.Activate();
		//		//	isForeground = true;

		//		// Todo:  Handle arguments and stuff
		//		// Ensure the current window is active
		//		if (args is ToastNotificationActivatedEventArgs toastActivationArgs)
		//		{
		//			// Obtain the arguments from the notification
		//			var toastArgs = System.Web.HttpUtility.ParseQueryString(toastActivationArgs.Argument);
		//			// Obtain any user input (text boxes, menu selections) from the notification
		//			ValueSet userInput = toastActivationArgs.UserInput;
		//			foreach (var op in toastArgs.AllKeys)
		//			{
		//				if (op == "incomingNotification")
		//				{
		//					Task.Delay(3000).ContinueWith(async (_) =>
		//					{
		//						while (IncomingTab.instance == null)
		//							await Task.Delay(500);
		//						AppS.DispatchOnUIThreadLow(IncomingTab.instance.Show);

		//					});
		//				}
		//			}
		//			// TODO: Show the corresponding content
		//		}


		//		return;
		//	}

		//	await OnLaunchedOrActivated(args);



		//	//var configuration = new ConfigurationBuilder()
		//	//                                .AddJsonFile("appsettings.json", false, true)
		//	//                                .Build();




		//	//    CreateDefaultBuilder(args)
		//	//        .ConfigureWebHostDefaults(webBuilder =>
		//	//        {
		//	//            webBuilder.UseStartup<Startup>();
		//	//        }).Build().Run();


		//	//ILogger logger;

		//	//using (var serviceProvider = new ServiceCollection()
		//	//    .AddLogging(cfg =>
		//	//    {
		//	//        cfg.AddConfiguration(configuration.GetSection("Logging"));
		//	//        cfg.AddConsole();
		//	//    })
		//	//    .BuildServiceProvider())
		//	//{
		//	//    logger = serviceProvider.GetService<ILogger<App>>();
		//	//}

		//	//logger.LogInformation("logger information");
		//	//logger.LogWarning("logger warning");


		//	//using (var listener = new LoggerTraceListener(logger))
		//	//{
		//	//    System.Diagnostics.Trace.Listeners.Add(listener);
		//	//    TraceSources.Instance.InitLoggerTraceListener(listener);

		//	//    TraceLover.DoSomething();
		//	//    TraceSourceLover.DoSomething();
		//	//}


		//}

		//public static void SetupCoreWindowInputHooks()
		//{

		//	Assert(CoreApplication.Views.Count==1);

		//	var window = CoreApplication.MainView.CoreWindow;
		//	{
		//		//Log($"{view.TitleBar.ToString()} {view.IsMain} ");

		//	//	window.PointerMoved += OnPointerMoved;
		//		window.PointerPressed += OnPointerPressed; ;
		//	//	window.PointerExited+=Window_PointerExited; ;
		//	//	window.PointerEntered+=Window_PointerEntered; ;
		//		window.KeyDown += OnKeyDown;
		//		window.KeyUp += OnKeyUp;

		//	}
		//}

		//private static void Window_PointerEntered(CoreWindow sender,PointerEventArgs args)
		//{
		//	ShellPage.UpdateMousePosition(args,ShellPage.instance);
		//	ShellPage.UpdateFocus();
		//}

		//private static void Window_PointerExited(CoreWindow sender,PointerEventArgs args)
		//{
		//	ShellPage.UpdateMousePosition(args, ShellPage.instance);
		//	ShellPage.UpdateFocus();
		//}

		//private static void Window_PointerEntered(CoreWindow sender,PointerEventArgs args)
		//{
		//	args.KeyModifiers.UpdateKeyModifiers();
		//	Log("Pointer enter");
		//	ShellPage.UpdateFocus();

		//}

		//private static void Window_PointerExited(CoreWindow sender,PointerEventArgs args)
		//{
		//	args.KeyModifiers.UpdateKeyModifiers();
		//	Log("Pointer exit");
		//	ShellPage.UpdateFocus();
		//}

		//public static void OnPointerPressed(CoreWindow sender, PointerEventArgs e)
		//{
		////	ShellPage.UpdateMousePosition(e);

		//	var prop = e.CurrentPoint.Properties.PointerUpdateKind;
		//	if (OnPointerPressed(prop))
		//		e.Handled = true;
		////	ShellPage.UpdateFocus();
		//}

		// Uses Task Await
		static async void ProcessThrottledTasks()
		{
			for (; ; )
			{


				try
				{
					if (!throttledTasks.IsEmpty)
					{
						if (throttledTasks.TryDequeue(out var t))
							await t().ConfigureAwait(false);

					}
				}
				catch (Exception _exception)
				{
					Debug.LogEx(_exception);
				}


				await Task.Delay(500).ConfigureAwait(false);
			}
		}

		public static void EnqeueTask(Func<Task> a)
		{
			throttledTasks.Enqueue(a);
		}

		public static bool OnPointerPressed(PointerUpdateKind prop)
		{
			var rv = false;
			switch (prop)
			{
				case PointerUpdateKind.XButton1Pressed:
					NavStack.Back(true);

					Log("XButton1");
					rv = true;
					break;
				case PointerUpdateKind.XButton2Pressed:
					NavStack.Forward(true);
					Log("XButton2");
					rv = true;
					break;
			}

			InputRecieved();
			return rv;
		}

		public static void InputRecieved() => AppS.InputRecieved();

		//private static void OnPointerMoved(CoreWindow sender, PointerEventArgs args)
		//{
		//	//	ShellPage.UpdateMousePosition(args);
		//	//			args.KeyModifiers.UpdateKeyModifiers();

		//	// reset timer if active
		//	//	InputRecieved();
		//	ShellPage.UpdateMousePosition(args,ShellPage.instance);
		//	ShellPage.UpdateFocus();
		//}
	

		private ActivationService CreateActivationService()
		{
			return new ActivationService(); //this, null, new Lazy<UIElement>(()=> new Views.ShellPage()));
		}

		//public static Task EnqueueAsync(DispatcherQueue dispatcher,Func<Task> function,DispatcherQueuePriority priority = 0)
		//{
		//	//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//	if(dispatcher.HasThreadAccess)
		//	{
		//		try
		//		{
		//			Task task = function();
		//			if(task != null)
		//			{
		//				return task;
		//			}

		//			return Task.FromException(new InvalidOperationException("The Task returned by function cannot be null."));
		//		}
		//		catch(Exception exception)
		//		{
		//			return Task.FromException(exception);
		//		}
		//	}

		//	return TryEnqueueAsync(dispatcher,function,priority);
		//	static Task TryEnqueueAsync(DispatcherQueue dispatcher,Func<Task> function,DispatcherQueuePriority priority)
		//	{
		//		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//		//IL_002b: Expected O, but got Unknown
		//		Func<Task> function2 = function;
		//		TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
		//		if(!dispatcher.TryEnqueue(priority,async ()=>
		//		{
		//			await function2();
		//			taskCompletionSource.SetResult(null);
		//		}))
		//		{
		//			taskCompletionSource.SetException(new InvalidOperationException("Failed to enqueue the operation"));
		//		}

		//		return taskCompletionSource.Task;
		//	}
		//}

		//public static async Task WaitWhileUiSemaBusy()
		//{
		//	Log($"Lock sema: {uiSema.CurrentCount}");
		//	for (; ; )
		//	{
		//		if (!App.isUISemaLocked)
		//			break;

		//		await uiSema.WaitAsync();
		//		uiSema.Release();
		//		await Task.Delay(500); // if there is another thread waiting on the sema, let them go first
		//	}

		//}



		//public static int pendingDispatch;
		//public static int pendingDispatchMax=10;
		//public static void DispatchStart()
		//{
		//	++pendingDispatch;
		//	if(pendingDispatch > pendingDispatchMax)
		//	{
		//		pendingDispatchMax = pendingDispatch + 5;
		//		Trace("PendingDispatch: " + pendingDispatch);
		//	}
		//}
		//public static void DispatchEnd() => --pendingDispatch;

		//public static void DispatchOnUIThreadSneakyLow(DispatcherQueueHandler action)
		//{
		//	var d = GlobalDispatcher();
		//	// run it immediately if we can
		//	if (d.HasThreadAccess && d.CurrentPriority <= DispatcherQueuePriority.Low)
		//		action();
		//	else
		//		d.RunAsync(DispatcherQueuePriority.Low, action);
		//}
		//public static async Task DispatchOnUIThreadSneakyLowAwait(DispatcherQueueHandler action)
		//{
		//	var d = GlobalDispatcher();
		//	// run it immediately if we can
		//	if (d.HasThreadAccess && d.CurrentPriority <= DispatcherQueuePriority.Low)
		//		action();
		//	else
		//		await d.RunAsync(DispatcherQueuePriority.Low, action);
		//}


		// We only have 1 UI thread here
		//public static bool IsKeyPressedControl()
		//{
		//    var window = CoreWindow.GetForCurrentThread();
		//    if (window == null)
		//    {
		//        return false;
		//    }

		//    return window.GetKeyState(VirtualKey.Control) == CoreVirtualKeyStates.Down;

		//}
		//public static bool IsKeyPressedShift()
		//{
		//    var window = CoreWindow.GetForCurrentThread();
		//    if (window == null)
		//    {
		//        return false;
		//    }

		//    return window.GetKeyState(VirtualKey.Shift) == CoreVirtualKeyStates.Down;
		//}



		///        public static DumbCollection<City> emptyCityList = new DumbCollection<City>();
		public static PercentFormatter percentFormatter = new PercentFormatter()
		{
				FractionDigits = 1, IsGrouped = false,
				NumberRounder  = new SignificantDigitsNumberRounder() { SignificantDigits = 1 }
		};

		public static DecimalFormatter formatter2Digit = new DecimalFormatter()
		{
				FractionDigits = 2, IsGrouped = true,
				NumberRounder  = new SignificantDigitsNumberRounder() { SignificantDigits = 2 }
		};

		public static DecimalFormatter formatterInt = new DecimalFormatter()
				{ FractionDigits = 0, IsGrouped = true, IsDecimalPointAlwaysDisplayed = false };

		public static DecimalFormatter formatterSeconds = new DecimalFormatter()
				{ FractionDigits = 0, IsGrouped = false, IntegerDigits = 2, IsDecimalPointAlwaysDisplayed = false };



		public static async Task<string> GetClipboardText()
		{
			try
			{
				return (await Clipboard.GetContent().GetTextAsync()) ?? string.Empty;
			}
			catch (Exception ex)
			{
				LogEx(ex);
				return string.Empty;
			}
		}


//		public static VirtualKeyModifiers canvasKeyModifiers;
		// HTML control messs wit this
//		public static VirtualKeyModifiers keyModifiers => canvasKeyModifiers;

		public static bool isShuttingDown => state == State.closing;

		//{
		//	get
		//	{
		//		var rv = VirtualKeyModifiers.None;
		//		if (IsKeyPressedShift())
		//			rv |= VirtualKeyModifiers.Shift;
		//		if (IsKeyPressedControl())
		//			rv |= VirtualKeyModifiers.Control;
		//		return rv;
		//	}
		//}

		//public static CoreCursor cursorDefault;// = CoreCursor.Create(CoreCursorShape.Arrow);
		//public static CoreCursor cursorQuickBuild;// = CoreCursor.Create(CoreCursorShape.Cross);
		//public static CoreCursor cursorMoveStart;// = CoreCursor.Create(CoreCursorShape.SizeNortheastSouthwest);
		//public static CoreCursor cursorMoveEnd;// = CoreCursor.Create(CoreCursorShape.SizeNorthSouth);
		//public static CoreCursor cursorLayout;// = CoreCursor.Create(CoreCursorShape.Pin);
		//public static CoreCursor cursorDestroy;// = CoreCursor.Create(CoreCursorShape.UniversalNo);

		public static ApplicationDataContainer ClientSettings()
		{
			var appData = ApplicationData.Current;
			if (appData.RoamingStorageQuota > 4)
				return appData.RoamingSettings;
			else
				return appData.LocalSettings;
		}

		public static async Task<byte[]> GetContent(string filename)
		{
			var uri  = new Uri("ms-appx:///" + filename);
			var file = await StorageFile.GetFileFromApplicationUriAsync(uri);

			var buffer = await FileIO.ReadBufferAsync(file);

			return buffer.ToArray();
		}




		public static class UserAgent
		{
			const int URLMON_OPTION_USERAGENT = 0x10000001;

			[DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
			private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength,
															int dwReserved);

			[DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
			private static extern int UrlMkGetSessionOption(int     dwOption, StringBuilder pBuffer, int dwBufferLength,
															ref int pdwBufferLength, int dwReserved);

			public static string GetUserAgent()
			{
				int capacity = 255;
				var buf      = new StringBuilder(capacity);
				int length   = 0;

				UrlMkGetSessionOption(URLMON_OPTION_USERAGENT, buf, capacity, ref length, 0);

				return buf.ToString();
			}

			public static void SetUserAgent(string agent)
			{
				var hr = UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, agent, agent.Length, 0);
				var ex = Marshal.GetExceptionForHR(hr);
				if (null != ex)
				{
					throw ex;
				}
			}

			public static void AppendUserAgent(string suffix)
			{
				SetUserAgent(GetUserAgent() + suffix);
			}

		}

	}

	public static class AApp
	{
		

		public static MenuFlyoutItem CreateMenuItem(string text, Action command, object context = null)
		{
			var rv = new MenuFlyoutItem() { Text = text };
			rv.DataContext = context;
			if (command != null)
				rv.Click += (_, _) => command();
			return rv;
		}
		public static MenuFlyoutItem CreateMenuItem(string text, ICommand command, object parameter, object context = null)
		{
			return new MenuFlyoutItem() { Text = text, Command = command, CommandParameter = parameter, DataContext = context };
		}
		public static MenuFlyoutItem CreateMenuItem(string text, RoutedEventHandler command, object context = null)
		{
			var rv = new MenuFlyoutItem() { Text = text };
			rv.DataContext = context;
			if (command != null)
				rv.Click += command;
			return rv;
		}
		public static MenuFlyoutItem CreateMenuItem(string text, bool isChecked, Action<bool> command)
		{
			var rv = new ToggleMenuFlyoutItem() { Text = text, IsChecked = isChecked };

			rv.Click += (sender, _) => command((sender as ToggleMenuFlyoutItem).IsChecked);
			return rv;
		}
		public static MenuFlyoutItem AddItem(this MenuFlyout menu, string text, RoutedEventHandler command, object context = null)
		{
			var rv = CreateMenuItem(text, command, context);

			menu.Items.Add(rv);
			return rv;
		}
		public static MenuFlyoutItem AddItem(this MenuFlyoutSubItem menu, string text, RoutedEventHandler command, object context = null)
		{
			var rv = CreateMenuItem(text, command, context);

			menu.Items.Add(rv);
			return rv;
		}
		public static MenuFlyoutItem AddItem(this MenuFlyout menu, string text, bool isChecked, Action<bool> command)
		{
			var rv = CreateMenuItem(text, isChecked, command);

			menu.Items.Add(rv);
			return rv;
		}
		public static MenuFlyoutItem AddItem(this MenuFlyoutSubItem menu, string text, bool isChecked, Action<bool> command)
		{
			var rv = CreateMenuItem(text, isChecked, command);

			menu.Items.Add(rv);
			return rv;
		}
		public static MenuFlyoutSubItem AddSubMenu(this MenuFlyout menu, string text)
		{
			var rv = new MenuFlyoutSubItem() { Text = text };

			menu.Items.Add(rv);
			return rv;
		}
		public static MenuFlyoutSubItem AddSubMenu(this MenuFlyoutSubItem menu,string text)
		{
			var rv = new MenuFlyoutSubItem() { Text = text };

			menu.Items.Add(rv);
			return rv;
		}
		public static void RemoveEmpy(this MenuFlyout menu)
		{
			for (int i = menu.Items.Count; --i >= 0;)
			{
				if (menu.Items[i] is MenuFlyoutSubItem sub)
				{
					if (sub.Items.Count == 0)
						menu.Items.RemoveAt(i);
				}
			}
		}

		public static MenuFlyoutItem AddItem(this MenuFlyout menu, string text, Action command)
		{
			var rv = new MenuFlyoutItem() { Text = text };
			if (command != null)
				rv.Click += (_, _) => command();
			menu.Items.Add(rv);
			return rv;
		}

		public static MenuFlyoutItem AddItem(this MenuFlyoutSubItem menu, string text, Action command)
		{
			var rv = new MenuFlyoutItem() { Text = text };
			if (command != null)
				rv.Click += (_, _) => command();
			menu.Items.Add(rv);
			return rv;
		}

		// must be on the right thread for this
		//public static void Set(this CoreCursor c) 
		//{
		//	// is this thread safe?
		//	//if(ShellPage.coreInputSource != null)
		//	//{				
		//	//	ShellPage.coreInputSource.DispatcherQueue.EnqueueAsync(() =>
				
		//	//		ShellPage.coreInputSource.PointerCursor = type,DispatcherQueuePriority.Low);

		//	//}
		////	AppS.QueueOnUIThread( () =>	CoreWindow.GetForCurrentThread().PointerCursor = type);
		//}

		public static bool IsLocalPointOver(this FrameworkElement e,int x,int y)
		{
			return x >=0 && y >= 0 && x < e.ActualWidth && y < e.ActualHeight;
		}
		public static bool IsParentPointOver(this FrameworkElement e,double x,double y)
		{
			var off = e.ActualOffset;
			return IsLocalPointOver(e,x-off.X,y-off.Y);
		}
		public static bool IsLocalPointOver(this FrameworkElement e,double x,double y)
		{
			return x >=0 && y >= 0 && x < e.ActualWidth && y < e.ActualHeight;
		}


	//	static void LoadConfig()
		//{
		//	var config = BuildConfig();

		//	// Get the Google Spreadsheet Config Values
		//	var serviceAccount = config["GOOGLE_SERVICE_ACCOUNT"];
		//	var documentId = config["GOOGLE_SPREADSHEET_ID"];
		//	var jsonCredsPath = config["GOOGLE_JSON_CREDS_PATH"];

		//	// In this case the json creds file is stored locally, but you can store this however you want to (Azure Key Vault, HSM, etc)
		//	var jsonCredsContent = File.ReadAllText(jsonCredsPath);

		//	// Create a new SheetHelper class
		//	var sheetHelper = new SheetHelper(documentId,serviceAccount,"");
		//	sheetHelper.Init(jsonCredsContent);

		//	// Get all the rows for the first 2 columns in the spreadsheet
		//	var rows = sheetHelper.GetRows(new SheetRange("",1,1,2));

		//	// Write all the values from the result set
		//	foreach(var row in rows)
		//	{
		//		foreach(var col in row)
		//		{
		//			Console.Write($"{col}\t");
		//		}
		//		Console.Write("\n");
		//	}

		//	// export a csv file from the current spreadsheet and tab
		//	var exporter = new SheetExporter(sheetHelper);

		//	var filepath = @"output.csv";

		//	using(var stream = new FileStream(filepath,FileMode.Create))
		//	{
		//		var range = new SheetRange("",1,1,2);
		//		exporter.ExportAsCsv(range,stream);
		//	}
		//}

	}

}
