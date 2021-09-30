using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Helpers;

using COTG.Game;
using COTG.Helpers;
using COTG.Services;
using COTG.Views;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
//using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Web.WebView2.Core;
//using ZLogger;
using CoreCursor = Windows.UI.Core.CoreCursor;
//using Cysharp.Text;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PointerUpdateKind = Windows.UI.Input.PointerUpdateKind;
//using Microsoft.AppCenter;
//using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
using Microsoft.UI.Input.Experimental;
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
using static COTG.Debug;
using DispatcherQueueHandler = Microsoft.UI.Dispatching.DispatcherQueueHandler;
using DispatcherQueuePriority= Microsoft.UI.Dispatching.DispatcherQueuePriority;
using ContentDialog = Microsoft.UI.Xaml.Controls.ContentDialog;
using ContentDialogResult = Microsoft.UI.Xaml.Controls.ContentDialogResult;
using MenuFlyout = Microsoft.UI.Xaml.Controls.MenuFlyout;
using MenuFlyoutItem = Microsoft.UI.Xaml.Controls.MenuFlyoutItem;
using MenuFlyoutSubItem = Microsoft.UI.Xaml.Controls.MenuFlyoutSubItem;
using ToggleMenuFlyoutItem = Microsoft.UI.Xaml.Controls.ToggleMenuFlyoutItem;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Hosting;
using Nito.AsyncEx;
using COTG.JSON;
//using Windows.UI.Core;

namespace COTG
{
	public sealed partial class App : Application
	{
		public enum State
		{
			init,
			active,
			closing,
			closed,
		}
		public static State state;
		private Lazy<ActivationService> _activationService;
		public static bool isForeground;
		public static bool processingTasksStarted;
		private ActivationService ActivationService
		{
			get { return _activationService.Value; }
		}


		public static App instance;
		public static string appLink = "cotg";

		public static async void StartAnalytics()
		{
			if (AppCenter.Configured)
			{
				return;
			}
			AppCenter.SetMaxStorageSizeAsync(16 * 1024 * 1024).ContinueWith((storageTask) => {
				// The storageTask.Result is false when the size cannot be honored.
			});
			AppCenter.LogLevel = System.Diagnostics.Debugger.IsAttached ? Microsoft.AppCenter.LogLevel.Warn : Microsoft.AppCenter.LogLevel.Error;
			AppCenter.Start("0b4c4039-3680-41bf-b7d7-685eb68e21d2",
			   typeof(Analytics)
#if CRASHES
			   , typeof(Crashes)
#endif
			   );
			await Task.WhenAll(
#if CRASHES
					Crashes.SetEnabledAsync(true),
#endif
								Analytics.SetEnabledAsync(true) );
			AAnalytics.initialized=true;

#if CRASHES
			bool didAppCrash = await Crashes.HasCrashedInLastSessionAsync();
			if (didAppCrash)
			{
				ErrorReport crashReport = await Crashes.GetLastSessionCrashReportAsync();
			}
#endif
		}
		public App()
		{



			//try
			//{
			//    {

			//        ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
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
			TaskScheduler.UnobservedTaskException+=TaskScheduler_UnobservedTaskException;
			FocusVisualKind = FocusVisualKind.Reveal;
			
			// TODO WTS: Add your app in the app center and set your secret here. More at https://docs.microsoft.com/appcenter/sdk/getting-started/uwp

			// Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
			_activationService = new Lazy<ActivationService>(CreateActivationService);
		//	UserAgent.SetUserAgent(JSClient.userAgent);  // set webview useragent

		}

		private void TaskScheduler_UnobservedTaskException(object sender,UnobservedTaskExceptionEventArgs e)
		{
			LogEx(e.Exception);
			e.SetObserved();
		}

		//		public static Windows.Foundation.IAsyncOperation<CoreWebView2Environment> createWebEnvironmentTask;

		private static async Task SwitchToBackground()
		{
			if(isForeground == true)
			{
				isForeground = false;

				//TODO: Save application state and stop any background activity
				try
				{

					var t0 = SaveState();

					var t = DateTimeOffset.UtcNow;
					var dt = t - activeStart;
					activeStart = t;

					AAnalytics.Track("Background",new Dictionary<string,string> { { "time",dt.TotalSeconds.RoundToInt().ToString() } });
					SystemInformation.Instance.AddToAppUptime(dt);
					await t0;
				}
				catch(Exception ex)
				{
				}
			}
		}

		private static Task SaveState()
		{
			var t0 = BuildQueue.SaveAll();
			var t1 = AttackTab.SaveAttacksBlock();
			SettingsPage.SaveAll();
			return Task.WhenAll(t0,t1);
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

		// can be called from any thread
		public static bool IsKeyPressedControl()
		{
			return controlPressed;
		//	return IsKeyDown(VirtualKey.Control)| IsKeyDown(VirtualKey.LeftControl)| IsKeyDown(VirtualKey.RightControl);
			//Microsoft.Xna.Framework.Input.Keys.LeftControl.IsKeyPressed() |
				//   Microsoft.Xna.Framework.Input.Keys.RightControl.IsKeyPressed();// shiftPressed;
		}
		public static bool IsEscDown()
		{
			return false;//Microsoft.Xna.Framework.Input.Keys.Escape.IsKeyPressed();
		}
		public static bool IsKeyPressedShift()
		{
			return shiftPressed;
//			IsKeyDown(VirtualKey.Shift) | IsKeyDown(VirtualKey.LeftShift) | IsKeyDown(VirtualKey.RightShift);
//			return Microsoft.Xna.Framework.Input.Keys.LeftShift.IsKeyPressed() |
//				   Microsoft.Xna.Framework.Input.Keys.RightShift.IsKeyPressed();// shiftPressed;
		}
		public static bool IsKeyPressedShiftOrControl()
		{
			return IsKeyPressedShift() | IsKeyPressedControl();
		}
		public static bool IsKeyPressedShiftAndControl()
		{
			return IsKeyPressedShift() && IsKeyPressedControl();
		}
		//static void OnKeyUp(CoreWindow sender, KeyEventArgs args)
		//{
		//	var key = args.VirtualKey;
		//	OnKeyUp(key);
		//}

		public static bool shiftPressed;
		public static bool controlPressed;

		public static void OnKeyUp(VirtualKey key)
		{
			switch (key)
			{
				case VirtualKey.Shift:
				case VirtualKey.LeftShift:
				case VirtualKey.RightShift:
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
		public static void OnKeyDown(VirtualKey key)
		{
			switch (key)
			{
				case VirtualKey.Shift:
				case VirtualKey.LeftShift:
				case VirtualKey.RightShift:
					shiftPressed = true;
					break;
				case VirtualKey.Control:
				case VirtualKey.LeftControl:
				case VirtualKey.RightControl:

					controlPressed = true;
					break;

			}
			InputRecieved();
		}

		private void SwitchToForeground()
		{
			Log("Foreground");
			if(!isForeground)
			{
				isForeground = true;
				var t = DateTimeOffset.UtcNow;
				var dt = t - activeStart;
				activeStart = t;
				AAnalytics.Track("Foreground", new Dictionary<string, string> { { "time", dt.TotalSeconds.RoundToInt().ToString() } });
				JSClient.ResumeWebView();

			}
			//if (ShellPage.canvas != null)
			//    ShellPage.canvas.Paused = false;
		}

		static public int storageFull = 0;
		static ConcurrentDictionary<string, int> exceptions = new();
		public static bool RegisterException(string message)
		{
			if (exceptions.TryGetValue(message, out var i))
			{
				exceptions.TryUpdate(message, i + 1, i);
				return false;
			}
			else
			{
				exceptions.TryAdd(message, 1);
				return true;
			}

		}

		private void OnAppUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
		{
#if TRACE
			System.Diagnostics.Debug.WriteLine($"Unhandled Exception: " + e.Message);
			System.Diagnostics.Debug.WriteLine(e.Exception.StackTrace);
#endif

			e.Handled = true;

			if (RegisterException(e.Message))
			{
				try
				{
#if CRASHES

					Crashes.TrackError(e.Exception);
#endif
					AAnalytics.Track("UnhandledException", new Dictionary<string, string> { { "message", e.Message.Truncate(120) } });
				}
				catch (Exception ex2)
				{

					RegisterException(ex2.Message);


				}
			}
		}

		public static int lastInputTick;
		public static void InputRecieved() => App.lastInputTick = Environment.TickCount;

		private static ConcurrentQueue<Action> idleTasks = new ConcurrentQueue<Action>();
		private static ConcurrentQueue<Func<Task>> throttledTasks = new ConcurrentQueue<Func<Task>>();
		public static DesktopWindow window;
		static DateTimeOffset activeStart = DateTimeOffset.UtcNow;
		protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
		{
			try
			{

			//	Windows.UI.ViewManagement.ApplicationView.PreferredLaunchWindowingMode =Windows.UI.ViewManagement.ApplicationViewWindowingMode.Maximized;// new Size(bounds.Width, bounds.Height);
//				Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterViewModeAsync(Windows.UI.ViewManagement.ApplicationViewMode.CompactOverlay);
				
				window= new();
				//	window.
				
				//var view = DisplayInformation.GetForCurrentView();
				var uwpArgs = AppInstance.GetActivatedEventArgs();//args.UWPLaunchActivatedEventArgs;
				if(uwpArgs.Kind == Windows.ApplicationModel.Activation.ActivationKind.Protocol) 
				{
					var eventArgs = uwpArgs as ProtocolActivatedEventArgs;
					Log("Args!! "+eventArgs.Uri);
					var s = System.Web.HttpUtility.ParseQueryString(eventArgs.Uri.Query);

					Debug.Log(s);
					// format $"cotg:launch?w={world}&s=1&n=1"
					// are / chars inserted?
					//  if (s.Length >= 3)
					{
						if(AMath.TryParseInt(s["s"],out int _s))
							JSClient.subId = _s;
						
						var n = s["p"];
						if(n!=null)
							Player.subOwner = n;
						
						if(AMath.TryParseInt(s["w"],out int _w))
							JSClient.world = _w;

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
				window.Title = "Crown of the Gods (sort of)";
//				window.SetTitleBar
			//ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
	//		window.ExtendsContentIntoTitleBar = true;
			//	window.ExtendsContentIntoTitleBar = true;
				//window.Maximize();
			//App.globalDispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
			globalQueue =  window.DispatcherQueue;
				//CoreApplication.EnablePrelaunch(false);
			if (uwpArgs.Kind == Windows.ApplicationModel.Activation.ActivationKind.Launch)
			{
				// do this asynchronously
				Services.StoreHelper.instance.DownloadAndInstallAllUpdatesAsync();
			}

			try
			{
				var str = CoreWebView2Environment.GetAvailableBrowserVersionString();
				Log(str);
	//			createWebEnvironmentTask =  CoreWebView2Environment.CreateAsync();

			}
			catch(Exception ex)
			{
				Windows.System.Launcher.LaunchUriAsync(new ("https://go.microsoft.com/fwlink/p/?LinkId=2124703",UriKind.Absolute));
				LogEx(ex);
			}

			// if (!args.PrelaunchActivated)

			await OnLaunchedOrActivated(args.UWPLaunchActivatedEventArgs);
			}
			catch(Exception e)
			{
				Log(e);
			}
		}

		private bool Window_Closing() //object sender,WindowClosingEventArgs e)
		{
			Trace("Closing!");
			if(state == State.closed)
				return true;
			if( state == State.closing)
				return false;
			state = State.closing;
			
			SwitchToBackground().ContinueWith( (_)=> 
				{
				state = State.closed;
				App.DispatchOnUIThread(window.Close);
			});
			return false;

		}

		private void Window_Closed(object sender,WindowEventArgs args)
		{
			Trace("Closed!");
			Assert( state == State.closed);
			SwitchToBackground();
		}

		private void Content_PreviewKeyUp(object sender,Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			OnKeyUp(e.Key);
		}

		private void Content_PreviewKeyDown(object sender,Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			OnKeyDown(e.Key);
		}

		

		private async Task OnLaunchedOrActivated(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
		{

			try
			{
				

				this.DebugSettings.FailFastOnErrors = false;
#if TRACE || DEBUG
//			this.DebugSettings.FailFastOnErrors = true;
#endif
			this.DebugSettings.EnableFrameRateCounter = false;
			this.DebugSettings.IsTextPerformanceVisualizationEnabled = false;
#if DEBUG
			//this.DebugSettings.FailFastOnErrors = false;
			this.DebugSettings.IsBindingTracingEnabled = true;
#else
            this.DebugSettings.IsBindingTracingEnabled = false;
#endif
			var wasRunning = args.PreviousExecutionState == ApplicationExecutionState.Running || args.PreviousExecutionState == ApplicationExecutionState.Suspended;

			if(!wasRunning)
			{
			//	var window = Window.Current;
				window.VisibilityChanged += Window_VisibilityChanged;
				window.Closed+=Window_Closed;
				window.WantClose+=Window_Closing;
				window.Activated+=Window_Activated;
			}
				await ActivationService.ActivateAsync(args,wasRunning);
			
			if(wasRunning)
				return;

			window.Content.PreviewKeyUp+=Content_PreviewKeyUp;
			window.Content.PreviewKeyDown+=Content_PreviewKeyDown; ;
			window.Maximize();

				//			CoreApplication.MainView.HostedViewClosing+=MainView_HostedViewClosing; ;
				//	CoreApplication.MainView.CoreWindow.Closed+=CoreWindow_Closed;
				//if(args!=null)
				//	SystemInformation.TrackAppUse(args);
				if(processingTasksStarted == false)
				{
					processingTasksStarted = true;

					Task.Run(ProcessThrottledTasks);
					Task.Run(ProcessIdleTasks);
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
			catch(Exception e)
			{
				Log(e);
			}
		}

		private void Window_Activated(object sender,WindowActivatedEventArgs args)
		{
			Trace("Activated");
		//	SwitchToForeground();
		}

		private async void Window_VisibilityChanged(object sender,WindowVisibilityChangedEventArgs args)
		{
				Trace($"Visibility!!: {args.Visible}");
				if(!args.Visible)
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
		//	JSClient.CloseWebView();
		//	TabPage.CloseAllTabWindows();
		//}

		//private async void MainView_HostedViewClosing(CoreApplicationView sender,HostedViewClosingEventArgs args)
		//{
		//	var defer = args.GetDeferral();
		//	Log("Close");
		//	state = State.closing;
		//	JSClient.CloseWebView();
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
		//						App.DispatchOnUIThreadLow(IncomingTab.instance.Show);

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
					COTG.Debug.LogEx(_exception);
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


		//private static void OnPointerMoved(CoreWindow sender, PointerEventArgs args)
		//{
		//	//	ShellPage.UpdateMousePosition(args);
		//	//			args.KeyModifiers.UpdateKeyModifiers();

		//	// reset timer if active
		//	//	InputRecieved();
		//	ShellPage.UpdateMousePosition(args,ShellPage.instance);
		//	ShellPage.UpdateFocus();
		//}

		private static async void ProcessIdleTasks()
		{
	//		await TaskScheduler.Default;
			for (; ; )
			{
				var tick = Environment.TickCount;
				// must be idle for at least 4 s
				if ((tick - lastInputTick).Abs() < 4 * 1000)
				{
					// not idle
					await Task.Delay(4 * 1000).ConfigureAwait(false);
					continue;
				}
				if (isForeground)
				{
					while (idleTasks.TryDequeue(out Action a))
					{
						try
						{
							a();
						}
						catch (Exception _exception)
						{
							COTG.Debug.LogEx(_exception);
						}
						await Task.Delay(1000).ConfigureAwait(false); // wait one second if idle
					}
				}
				// not idle but no tasks
				await Task.Delay(4 * 1000).ConfigureAwait(false);
			}
		}

		// with a delay
		public static void QueueIdleTask(Action a, int intialDelayInmilisecons)
		{
			foreach (var i in idleTasks)
			{
				if (i.IsEqual(a) )
					return;
			}

			Task.Delay(intialDelayInmilisecons).ContinueWith((_) => QueueIdleTask(a));
		}

		public static void QueueIdleTask(Action a)
		{
			foreach (var i in idleTasks)
			{
				if (i.IsEqual(a) )
					return;
			}

			idleTasks.Enqueue(a);


		}

		//protected override async void OnActivated(IActivatedEventArgs args)
		//{
		//	var activation = args as IActivatedEventArgs;
		//	if (activation != null && activation.PreviousExecutionState == ApplicationExecutionState.Running)
		//	{
		//		Window.Current.Activate();
		//	//	isForeground = true;

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
		//				   {
		//					   while (IncomingTab.instance == null)
		//						   await Task.Delay(500);
		//					   App.DispatchOnUIThreadLow( ()=>IncomingTab.instance.Show() );

		//				   });
		//				}
		//			}
		//			// TODO: Show the corresponding content
		//		}


		//		return;
		//	}
		//	await ActivationService.ActivateAsync(args);
		//	OnLaunchedOrActivated(args);



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
		private ActivationService CreateActivationService()
		{
			return new ActivationService();//this, null, new Lazy<UIElement>(()=> new Views.ShellPage()));
		}

		//public static CoreWebView2Environment webEnvironment;
		//public static CoreWebView2Controller webController;
		//public static CoreWebView2   webCore;

		//private void App_Resuming(object sender, object e)
		//{
		//	Log("Resume");
		////	isForeground = true;
		//	AAnalytics.Track("Resume");

		//	activeStart = DateTimeOffset.UtcNow;
		//	JSClient.ResumeWebView();
		//	//         Singleton<SuspendAndResumeService>.Instance.ResumeApp();
		//}

		//protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
		//{
		//	App.globalDispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

		//	await ActivationService.ActivateAsync(args);
		//}

		//protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
		//{
		//	await ActivationService.ActivateFromShareTargetAsync(args);
		//}
		//public static Task<T> EnqueueAsync<T>( DispatcherQueue dispatcher,Func<Task<T>> function,DispatcherQueuePriority priority = 0)
		//{
		//	//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//	if(dispatcher.HasThreadAccess)
		//	{
		//		try
		//		{
		//			Task<T> task = function();
		//			if(task != null)
		//			{
		//				return task;
		//			}

		//			return Task.FromException<T>(new InvalidOperationException("The Task returned by function cannot be null."));
		//		}
		//		catch(Exception exception)
		//		{
		//			return Task.FromException<T>(exception);
		//		}
		//	}

		//	return TryEnqueueAsync(dispatcher,function,priority);
		//	static Task<T> TryEnqueueAsync(DispatcherQueue dispatcher,Func<Task<T>> function,DispatcherQueuePriority priority)
		//	{
		//		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//		//IL_002b: Expected O, but got Unknown
		//		Func<Task<T>> function2 = function;
		//		TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
		//		if(!dispatcher.TryEnqueue(priority,async () =>
		//		{
					
		//			taskCompletionSource.SetResult(await function2());

		//		}))
		//		{
		//			taskCompletionSource.SetException(new InvalidOperationException("Failed to enqueue the operation"));
		//		}

		//		return taskCompletionSource.Task;
		//	}
		//}
	//	public static Task<T> EnqueueAsync<T>( DispatcherQueue dispatcher,Func<T> function,DispatcherQueuePriority priority = 0)
	//	{
	////		DispatcherQueueExtensions
	//		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
	//		if(dispatcher.HasThreadAccess)
	//		{
	//			try
	//			{
	//				return Task.FromResult(function());
	//			}
	//			catch(Exception exception)
	//			{
	//				return Task.FromException<T>(exception);
	//			}
	//		}

	//		return TryEnqueueAsync(dispatcher,function,priority);
	//		static Task<T> TryEnqueueAsync(DispatcherQueue dispatcher,Func<T> function,DispatcherQueuePriority priority)
	//		{
	//			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
	//			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
	//			//IL_002b: Expected O, but got Unknown
	//			Func<T> function2 = function;
	//			TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
	//			if(!dispatcher.TryEnqueue(priority,(DispatcherQueueHandler)(object)(DispatcherQueueHandler)delegate
	//			{
	//				try
	//				{
	//					taskCompletionSource.SetResult(function2());
	//				}
	//				catch(Exception exception2)
	//				{
	//					taskCompletionSource.SetException(exception2);
	//				}
	//			}))
	//			{
	//				taskCompletionSource.SetException(new InvalidOperationException("Failed to enqueue the operation"));
	//			}

	//			return taskCompletionSource.Task;
	//		}
	//	}

		public static Task<T>
			DispatchOnUIThreadTask<T>(  Func<Task<T>> func,Microsoft.UI.Dispatching.DispatcherQueuePriority priority = Microsoft.UI.Dispatching.DispatcherQueuePriority.Low)
		{

			var d = GlobalDispatcher();
	
			return DispatcherQueueExtensions.EnqueueAsync<T>(d,func, priority);

			//var idle = priority == DispatcherQueuePriority.Idle;
			//if (useCurrentThreadIfPossible&& !idle && d.HasThreadAccess)
			//{
			//	return await func();
			//}
			//else
			//{
			//	var taskCompletionSource = new TaskCompletionSource<T>();
			//	if (!idle)
			//	{
			//		await d.TryRunAsync(priority,async () =>
			//	  {
			//		  try
			//		  {
			//			  taskCompletionSource.SetResult(await func());
			//		  }
			//		  catch (Exception ex)
			//		  {
			//			  LogEx(ex);
			//			  taskCompletionSource.SetResult(default);
			//		  }
			//	  });
			//	}
			//	else
			//	{
			//		await d.TryRunIdleAsync( async (_) =>
			//		{
			//			try
			//			{
			//				taskCompletionSource.SetResult(await func());
			//			}
			//			catch (Exception ex)
			//			{
			//				LogEx(ex);
			//				taskCompletionSource.SetResult(default);
			//			}
			//		});

			//	}
			//	return await taskCompletionSource.Task;
			//}
		}
		//public static Task EnqueueAsync(DispatcherQueue dispatcher,Action function,DispatcherQueuePriority priority = 0)
		//{
		//	//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//	if(dispatcher.HasThreadAccess)
		//	{
		//		try
		//		{
		//			function();
		//			return Task.CompletedTask;
		//		}
		//		catch(Exception exception)
		//		{
		//			return Task.FromException(exception);
		//		}
		//	}

		//	return TryEnqueueAsync(dispatcher,function,priority);
		//	static Task TryEnqueueAsync(DispatcherQueue dispatcher,Action function,DispatcherQueuePriority priority)
		//	{
		//		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//		//IL_002b: Expected O, but got Unknown
		//		Action function2 = function;
		//		TaskCompletionSource<object?> taskCompletionSource = new TaskCompletionSource<object>();
		//		if(!dispatcher.TryEnqueue(priority,(DispatcherQueueHandler)(object)(DispatcherQueueHandler)delegate
		//		{
		//			try
		//			{
		//				function2();
		//				taskCompletionSource.SetResult(null);
		//			}
		//			catch(Exception exception2)
		//			{
		//				taskCompletionSource.SetException(exception2);
		//			}
		//		}))
		//		{
		//			taskCompletionSource.SetException(new InvalidOperationException("Failed to enqueue the operation"));
		//		}

		//		return taskCompletionSource.Task;
		//	}
		//}
		// There is no TaskCompletionSource<void> so we use a bool that we throw away.
		public static Task DispatchOnUIThreadTask(
	  Func<Task> func,DispatcherQueuePriority priority = DispatcherQueuePriority.Low, bool useCurrentThreadIfPossible = true)
		{
			var d = GlobalDispatcher();
			return DispatcherQueueExtensions.EnqueueAsync(d,func, priority);
		}
		public static Task DispatchOnUIThreadTask(
	  Action func,DispatcherQueuePriority priority = DispatcherQueuePriority.Low,bool useCurrentThreadIfPossible = true)
		{
			var d = GlobalDispatcher();
			return DispatcherQueueExtensions.EnqueueAsync(d,func,priority);
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
		public static SemaphoreSlim uiSema = new SemaphoreSlim(1);


		public static bool isUISemaLocked => uiSema.IsLocked();
		public static async Task<T>
			DispatchOnUIThreadExclusive<T>(int cid,Func<Task<T>> func, DispatcherQueuePriority priority = DispatcherQueuePriority.Low)
		{
			if (!await LockUiSema(cid))
				return default;
			try
			{
				return await DispatchOnUIThreadTask(func, priority);
			}
			finally
			{
				ReleaseUISema(cid);

			}

		}
		
		public static async Task
			DispatchOnUIThreadExclusive(int cid, Func<Task> func, DispatcherQueuePriority priority = DispatcherQueuePriority.Low)
		{
			if (!await LockUiSema(cid).ConfigureAwait(false))
				return ;

			try
			{
				await DispatchOnUIThreadTask(func, priority);
			}
			finally
			{
				ReleaseUISema(cid);
			}

		}

		public static void ReleaseUISema(int cid)
		{
			Log($"unlock sema: {uiSema.CurrentCount}");
			Assert(City.lockedBuild == cid);
			City.lockedBuild = 0;
			uiSema.Release();
		}

		public static async Task<bool> LockUiSema(int cid)
		{
			Log($"Lock sema: {uiSema.CurrentCount}");
			Assert(City.CanVisit(cid));
			if (App.isUISemaLocked)
			{
				var i = await App.DoYesNoBox("Busy", "Wait for process to finish?");
				if (i != 1)
					return false;
				await uiSema.WaitAsync();
			}
			else
			{
				await uiSema.WaitAsync();
			}
			try
			{
				if (!await JSClient.CitySwitch(cid, isLocked:true))
					throw new UIException("Sema");
				City.lockedBuild = cid;
			}
			catch(Exception ex)
			{
				LogEx(ex);
				uiSema.Release();
				return false;
			}
			return true;
		}
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
		
	public static void DispatchOnUIThread(DispatcherQueueHandler action, DispatcherQueuePriority priority= DispatcherQueuePriority.Normal, bool alwaysQueue = false)
	{
		var d = GlobalDispatcher();
		// run it immediately if we can
		if (d.HasThreadAccess && !alwaysQueue)
			action();
		else
			d.TryEnqueue( priority, action);
	}

		public static void DispatchOnUIThreadIdle(DispatcherQueueHandler action)
		{
			DispatchOnUIThread(action,DispatcherQueuePriority.Low);
//			var d = GlobalDispatcher();
//			d.TryRunIdleAsync((_)=> action() );
		}

		public static void QueueOnUIThread(DispatcherQueueHandler action)
		{
			DispatchOnUIThread(action,priority: DispatcherQueuePriority.Low,alwaysQueue: true);
			//			var d = GlobalDispatcher();
			//			d.TryRunIdleAsync((_)=> action() );
		}

		public static void DispatchOnUIThreadLow(DispatcherQueueHandler action, bool alwaysQueue = false)
	{
		var d = GlobalDispatcher();
		// run it immediately if we can
		if (d.HasThreadAccess && !alwaysQueue)
			action();
		else
			d.TryEnqueue(action);
	}

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
	public static Microsoft.UI.Dispatching.DispatcherQueue GlobalDispatcher() => globalQueue;
		public static Microsoft.UI.Dispatching.DispatcherQueue globalQueue;
		public static bool IsOnUIThread() => globalQueue.HasThreadAccess;
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
		public static PercentFormatter percentFormatter = new PercentFormatter() { FractionDigits=1,IsGrouped = false, NumberRounder = new SignificantDigitsNumberRounder() { SignificantDigits = 1 } };
		public static DecimalFormatter formatter2Digit = new DecimalFormatter() { FractionDigits = 2, IsGrouped = true, NumberRounder = new SignificantDigitsNumberRounder() { SignificantDigits = 2 } };
		public static DecimalFormatter formatterInt = new DecimalFormatter() { FractionDigits=0,IsGrouped = true, IsDecimalPointAlwaysDisplayed=false };
		public static DecimalFormatter formatterSeconds = new DecimalFormatter() { FractionDigits=0,IsGrouped = false, IntegerDigits = 2, IsDecimalPointAlwaysDisplayed = false };


		public static void CopyTextToClipboard(string s)
		{
			App.DispatchOnUIThread(() =>
		 {
			 try
			 {
				 DataPackage dataPackage = new DataPackage();
				 // copy
				 dataPackage.RequestedOperation = DataPackageOperation.Copy;
				 dataPackage.SetText(s);
				 // if(appLink!=null)
				 //     dataPackage.SetApplicationLink(new Uri() )
				 Clipboard.SetContent(dataPackage);
			 }
			 catch (Exception ex)
			 {
				 LogEx(ex);
			 }
		 });
		}

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

		public static CoreCursor cursorDefault;// = CoreCursor.Create(CoreCursorShape.Arrow);
		public static CoreCursor cursorQuickBuild;// = CoreCursor.Create(CoreCursorShape.Cross);
		public static CoreCursor cursorMoveStart;// = CoreCursor.Create(CoreCursorShape.SizeNortheastSouthwest);
		public static CoreCursor cursorMoveEnd;// = CoreCursor.Create(CoreCursorShape.SizeNorthSouth);
		public static CoreCursor cursorLayout;// = CoreCursor.Create(CoreCursorShape.Pin);
		public static CoreCursor cursorDestroy;// = CoreCursor.Create(CoreCursorShape.UniversalNo);
		internal static VirtualKeyModifiers keyModifiers
		{
			get
			{
				var rv = shiftPressed ? VirtualKeyModifiers.Shift : default;
				if( controlPressed)
					rv |= VirtualKeyModifiers.Control;
				return rv;
			}
		}

		public async static Task<int> DoYesNoBox(string title, string text, string yes="Yes", string no = "No", string cancel ="Cancel" )
		{
			
			return await DispatchOnUIThreadTask(async () =>
		   {
				 return await DoYesNoBoxUI(title, text,yes,no,cancel);
		   });
		}

		public async static Task<int> DoYesNoBoxUI(string title, string text, string yes = "Yes", string no = "No", string cancel = "Cancel")
		{
		//	Assert(App.uiSema.CurrentCount == 0);
			Assert(App.IsOnUIThread());

			var dialog = new ContentDialog()
				{
					Title = title ?? string.Empty,
					Content = text ?? string.Empty,
					PrimaryButtonText = yes ?? string.Empty,
					IsSecondaryButtonEnabled = no is not null,
					IsPrimaryButtonEnabled = yes is not null,
					
					SecondaryButtonText = no??string.Empty,
					CloseButtonText = cancel ?? string.Empty
			};
				return (await dialog.ShowAsync2()) switch { ContentDialogResult.Primary => 1, ContentDialogResult.Secondary => 0, _ => -1 };
		}
		public async static Task<(bool rv, bool? sticky)> DoYesNoBoxSticky(string title, string yes = "Yes", string no = "No", string cancel = "Cancel")
		{
			return await DispatchOnUIThreadTask(async () => await DoYesNoBoxStickyUI(title,yes,no,cancel));
		}
		public async static Task<(bool rv,bool? sticky)> DoYesNoBoxStickyUI(string title,  string yes = "Yes", string no = "No", string cancel = "Cancel")
		{
			//	Assert(App.uiSema.CurrentCount == 0);
				var check = new CheckBox() { Content = "Apply to all", IsChecked = false };

				var dialog = new ContentDialog()
				{
					Title = title,
					Content = check,
					PrimaryButtonText = yes,
					SecondaryButtonText = no,
					CloseButtonText = cancel
				};
				var uc = (await dialog.ShowAsync2());
				if (uc == ContentDialogResult.Primary || uc == ContentDialogResult.Secondary)
				{
					var rv = uc == ContentDialogResult.Primary;
					return (rv, check.IsChecked.GetValueOrDefault() ? rv : null);
				}
				else
				{
					return (false, null);
				}
			
		}
		public static void HideFlyout(object sender)
		{
			//var but = sender as Button;
			//Assert(but != null);
			//if (but != null)
			//{
			//	var fly = but.FindParent<FlyoutPresenter>();
			//	Assert(fly != null);
			//	fly?.ContextFlyout?.Hide();
			//}

		}
	}



	public static class UserAgent
	{
		const int URLMON_OPTION_USERAGENT = 0x10000001;

		[DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
		private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);

		[DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
		private static extern int UrlMkGetSessionOption(int dwOption, StringBuilder pBuffer, int dwBufferLength, ref int pdwBufferLength, int dwReserved);

		public static string GetUserAgent()
		{
			int capacity = 255;
			var buf = new StringBuilder(capacity);
			int length = 0;

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

	public static class AApp
	{


		public static SemaphoreSlim popupSema = new SemaphoreSlim(1);

		public static async Task<ContentDialogResult> ShowAsync2(this ContentDialog dialog,UIElement xamlRootSource = null)
		{
			CopyXamlRoomFrom(dialog,xamlRootSource);

			await popupSema.WaitAsync();//.ConfigureAwait(true);
			try
			{
				var result = await dialog.ShowAsync();
				return result;

			}
			finally
			{
				popupSema.Release();
			}
		}







		//[Conditional("DEBUG")]
		public static void UpdateKeyModifiers(this VirtualKeyModifiers mod)
		{
				App.shiftPressed = mod.IsShift();
				App.controlPressed = mod.IsControl();
		}
		public static string CidToStringMD(this int cid)
		{
			var coord = cid.CidToString();
			return $"[{coord}](/c/{coord})";

		}
		public static string bspotToString(this (int x, int y) cc)
		{
			var coord = $"{(cc.x):000}:{(cc.y):000}"; ;
			return coord;

		}
		public static string CidToCoords(this int cid)
		{
			var coord = cid.CidToString();
			return $"<coords>{coord}</coords>";

		}

		public static string CidToString(this int cid)
		{
			return $"{(cid % 65536):000}:{(cid / 65536):000}";
		}
		public static int FromCoordinate(this string s)
		{

			return AUtil.DecodeCid(0, s);
			//                var links = s.Split( new char[] { ' ','.',':',',',';'},StringSplitOptions.RemoveEmptyEntries);
			//              return int.Parse(links[0]) | int.Parse(links[1]) * 65536;


		}
		// 20 bit mash


		public static Vector2 ToWorldC(this int c)
		{
			var x = c % 65536;
			var y = c >> 16;

			return new Vector2(x, y);
		}
		public static (int X, int Y) ToWorldXY(this int c)
		{
			var x = c % 65536;
			var y = c >> 16;

			return (x, y);
		}
		public static bool IsShift(this VirtualKeyModifiers mod)
		{
			return mod.HasFlag(VirtualKeyModifiers.Shift);
		}
		public static bool IsControl(this VirtualKeyModifiers mod)
		{
			return mod.HasFlag(VirtualKeyModifiers.Control);
		}
		public static bool IsShiftOrControl(this VirtualKeyModifiers mod)
		{
			return mod.HasFlag(VirtualKeyModifiers.Control) | mod.HasFlag(VirtualKeyModifiers.Shift);
		}
		public static bool IsShiftAndControl(this VirtualKeyModifiers mod)
		{
			return mod.HasFlag(VirtualKeyModifiers.Control) & mod.HasFlag(VirtualKeyModifiers.Shift);
		}
		public static void CopyXamlRoomFrom(this FlyoutBase target, UIElement source)
		{
			if(source?.XamlRoot is not null) 
				target.XamlRoot = source.XamlRoot;
		}
		public static void CopyXamlRoomFrom(this UIElement target, UIElement source)
		{
			if (source?.XamlRoot is not null)
				target.XamlRoot = source.XamlRoot;
			else
				target.XamlRoot = App.window.Content.XamlRoot;
		}
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
		public static void Set(this CoreCursor c) 
		{
			// is this thread safe?
			//if(ShellPage.coreInputSource != null)
			//{				
			//	ShellPage.coreInputSource.DispatcherQueue.EnqueueAsync(() =>
				
			//		ShellPage.coreInputSource.PointerCursor = type,DispatcherQueuePriority.Low);

			//}
		//	App.QueueOnUIThread( () =>	CoreWindow.GetForCurrentThread().PointerCursor = type);
		}

		public static bool IsLocked(this SemaphoreSlim sema) => sema.CurrentCount==0;
		public static bool IsLocalPointInBounds(this FrameworkElement e,int x,int y)
		{
			return x >=0 && y >= 0 && x < e.ActualWidth && y < e.ActualHeight;
		}
		public static bool IsLocalPointInBounds(this FrameworkElement e,double x,double y)
		{
			return x >=0 && y >= 0 && x < e.ActualWidth && y < e.ActualHeight;
		}
	}
}
