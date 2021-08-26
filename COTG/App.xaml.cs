using COTG.Game;
using COTG.Helpers;
using COTG.Services;
using COTG.Views;

using Microsoft.AppCenter.Crashes;
//using ZLogger;

//using Cysharp.Text;
using Microsoft.Toolkit.Uwp.UI.Controls;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
//using Microsoft.AppCenter;
//using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.Globalization.NumberFormatting;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Core.Preview;
using Windows.UI.Input;
using Windows.UI.Xaml;
//using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using ContentDialog = Windows.UI.Xaml.Controls.ContentDialog;
using ContentDialogResult = Windows.UI.Xaml.Controls.ContentDialogResult;
using MenuFlyoutItem = Windows.UI.Xaml.Controls.MenuFlyoutItem;
using MenuFlyout = Windows.UI.Xaml.Controls.MenuFlyout;
using ToggleMenuFlyoutItem = Windows.UI.Xaml.Controls.ToggleMenuFlyoutItem;
using MenuFlyoutSubItem = Windows.UI.Xaml.Controls.MenuFlyoutSubItem;
using static COTG.Debug;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.AppCenter.Analytics;
using System.Collections.Generic;
using System.Net;
using Nito.AsyncEx;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp;

namespace COTG
{
	public sealed partial class App : Application
	{
		public enum State
		{
			init,
			active,
			closing,
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
			InitializeComponent();
			instance = this;

			UnhandledException += OnAppUnhandledException;

			FocusVisualKind = FocusVisualKind.Reveal;



			EnteredBackground += App_EnteredBackground;
			LeavingBackground += App_LeavingBackground;
			Resuming += App_Resuming;
			Suspending += App_Suspending;

			//AppCenter.Start("0b4c4039-3680-41bf-b7d7-685eb68e21d2",
			//	   typeof(Analytics), typeof(Crashes));
			// TODO WTS: Add your app in the app center and set your secret here. More at https://docs.microsoft.com/appcenter/sdk/getting-started/uwp

			// Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
			_activationService = new Lazy<ActivationService>(CreateActivationService);
			UserAgent.SetUserAgent(JSClient.userAgent);  // set webview useragent

		}

		public static async void App_CloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
		{
			Log("Close");
			state = State.closing;
			await TabPage.CloseAllTabWindows();
		}

		private async void App_Suspending(object sender, SuspendingEventArgs e)
		{
			if (isForeground == true)
			{
				isForeground = false;

				var deferral = e.SuspendingOperation.GetDeferral();
				//TODO: Save application state and stop any background activity
				try
				{
					await SaveState();

				}
				catch (Exception ex)
				{
				}
				finally
				{
					deferral.Complete();
				}
			}
		}

		private static async Task SaveState()
		{

			await JSON.BuildQueue.SaveIfNeeded();
			SettingsPage.SaveAll();
			await AttackTab.SaveAttacksBlock();
		}


		// can only be called from UI thread
		private static CoreVirtualKeyStates GetKeyState(VirtualKey key)
		{
			var window = CoreWindow.GetForCurrentThread();
			if (window == null)
			{
				Assert(false);
				return CoreVirtualKeyStates.None;
			}

			return window.GetAsyncKeyState(key);
		}
		public static bool IsKeyDown(VirtualKey key)
		{
			var state = GetKeyState(key);
			return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
		}

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
			return Microsoft.Xna.Framework.Input.Keys.Escape.IsKeyPressed();
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
		static void OnKeyUp(CoreWindow sender, KeyEventArgs args)
		{
			var key = args.VirtualKey;
			OnKeyUp(key);
		}

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

		static void OnKeyDown(CoreWindow sender, KeyEventArgs args)
		{
			var key = args.VirtualKey;
	
			OnKeyDown(key);

		}

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

		private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
		{
			Log("LeavingBackground");
			isForeground = true;
			var t = DateTimeOffset.UtcNow;
			var dt = t - activeStart;
			activeStart = t;
			AAnalytics.Track("Foreground", new Dictionary<string, string> { { "time", dt.TotalSeconds.RoundToInt().ToString() } });

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

		private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
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
					Crashes.TrackError(e.Exception);
					AAnalytics.Track("UnhandledException", new Dictionary<string, string> { { "message", e.Message.Truncate(120) } });
				}
				catch (Exception ex2)
				{

					RegisterException(ex2.Message);


				}
			}
		}

		static int lastInputTick;
		public static void InputRecieved() => lastInputTick = Environment.TickCount;

		private static ConcurrentQueue<Action> idleTasks = new ConcurrentQueue<Action>();
		private static ConcurrentQueue<Func<Task>> throttledTasks = new ConcurrentQueue<Func<Task>>();

		static DateTimeOffset activeStart = DateTimeOffset.UtcNow;
		protected override async void OnLaunched(LaunchActivatedEventArgs args)
		{
			{


				//var view = DisplayInformation.GetForCurrentView();

				//// Get the screen resolution (APIs available from 14393 onward).
				//var resolution = new Size(view.ScreenWidthInRawPixels, view.ScreenHeightInRawPixels);

				//// Calculate the screen size in effective pixels. 
				//// Note the height of the Windows Taskbar is ignored here since the app will only be given the maxium available size.
				//var scale = view.ResolutionScale == ResolutionScale.Invalid ? 1 : view.RawPixelsPerViewPixel;
				//var bounds = new Size(resolution.Width / scale, resolution.Height / scale);

				//ApplicationView.PreferredLaunchViewSize = new Size(bounds.Width, bounds.Height);
				//ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
		
			}
			//App.globalDispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
			globalQueue = DispatcherQueue.GetForCurrentThread();
			CoreApplication.EnablePrelaunch(false);

			if (args.Kind == ActivationKind.Launch)
			{
				// do this asynchronously
				Services.StoreHelper.instance.DownloadAndInstallAllUpdatesAsync();
			}

			await ActivationService.ActivateAsync(args);
			// if (!args.PrelaunchActivated)

			OnLaunchedOrActivated(args);
		}
		private void OnLaunchedOrActivated(IActivatedEventArgs args)
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
			//if(args!=null)
			//	SystemInformation.TrackAppUse(args);
			if (processingTasksStarted == false)
			{
				processingTasksStarted = true;

				Task.Run(ProcessThrottledTasks);
				Task.Run(ProcessIdleTasks);
			}

			SystemInformation.Instance.TrackAppUse(args);
#if DEBUG
			var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
			coreTitleBar.ExtendViewIntoTitleBar = false;
			var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;


			var color = Windows.UI.Color.FromArgb(0xFF, 0x20, 0x0, 0x35);
			var colorInactive = Windows.UI.Color.FromArgb(0xFF, 0x00, 0x0, 0x35);
			titleBar.BackgroundColor = color;
			//titleBar.ForegroundColor = color;
			//titleBar.ButtonForegroundColor = color;
			titleBar.ButtonBackgroundColor = color;
			titleBar.InactiveBackgroundColor = titleBar.ButtonInactiveBackgroundColor = colorInactive;
//				titleBar.InactiveForegroundColor =  titleBar.ButtonInactiveForegroundColor = colorInactive;
			//titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;
			//  UpdateTitleBarLayout(coreTitleBar);
#endif

			// Set XAML element as a draggable region.
			//          Window.Current.SetTitleBar(ShellPage.instance.AppTitleBar);
		}
		public static void SetupCoreWindowInputHooks()
		{

			foreach (var view in CoreApplication.Views)
			{
				//Log($"{view.TitleBar.ToString()} {view.IsMain} ");
				var window = view.CoreWindow;
				window.PointerMoved -= OnPointerMoved;
				window.PointerPressed -= OnPointerPressed; ;

				window.KeyDown -= OnKeyDown;
				window.KeyUp -= OnKeyUp;

				window.PointerMoved += OnPointerMoved;
				window.PointerPressed += OnPointerPressed; ;

				window.KeyDown += OnKeyDown;
				window.KeyUp += OnKeyUp;

			}
		}
		public static void OnPointerPressed(CoreWindow sender, PointerEventArgs e)
		{
			var prop = e.CurrentPoint.Properties.PointerUpdateKind;
			if (OnPointerPressed(prop))
				e.Handled = true;

		}

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


		private static void OnPointerMoved(CoreWindow sender, PointerEventArgs args)
		{
			// reset timer if active
			InputRecieved();
		}

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
			return new ActivationService(this, null, new Lazy<UIElement>(CreateShell));
		}

		private UIElement CreateShell()
		{
			return new Views.ShellPage();
		}
		private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
		{
			Log("Enter Background");

			isForeground = false;
			var deferral = e.GetDeferral();
			try
			{
				// only save if we have c

				await SaveState();

				var t = DateTimeOffset.UtcNow;
				var dt = t - activeStart;
				activeStart = t;
			
				AAnalytics.Track("Background", new Dictionary<string, string> { { "time", dt.TotalSeconds.RoundToInt().ToString() } });
				SystemInformation.Instance.AddToAppUptime(dt);

			}
			catch
			{
			}
			finally
			{
				deferral.Complete();
			}

		}

		private void App_Resuming(object sender, object e)
		{
			Log("Resume");
		//	isForeground = true;

			activeStart = DateTimeOffset.UtcNow;

			//         Singleton<SuspendAndResumeService>.Instance.ResumeApp();
		}

		//protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
		//{
		//	App.globalDispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

		//	await ActivationService.ActivateAsync(args);
		//}

		//protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
		//{
		//	await ActivationService.ActivateFromShareTargetAsync(args);
		//}

		public static Task<T>
			DispatchOnUIThreadTask<T>(  Func<Task<T>> func, DispatcherQueuePriority priority = DispatcherQueuePriority.Low)
		{
			var d = GlobalDispatcher();
			
			return d.EnqueueAsync<T>(func, priority);

			//var idle = priority == CoreDispatcherPriority.Idle;
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

		// There is no TaskCompletionSource<void> so we use a bool that we throw away.
		public static async Task DispatchOnUIThreadTask(
	  Func<Task> func, CoreDispatcherPriority priority = CoreDispatcherPriority.Low, bool useCurrentThreadIfPossible = true)
		{
			var d = GlobalDispatcher();
			var idle = priority == CoreDispatcherPriority.Idle;

			if (useCurrentThreadIfPossible&&!idle && d.HasThreadAccess)
			{
				await func();
			}
			else
			{
				var taskCompletionSource = new TaskCompletionSource<bool>();
				if (!idle)
				{
					await d.TryRunAsync(priority, async () =>
						{
							try
							{
								await func();
								taskCompletionSource.SetResult(true);
							}
							catch (Exception ex)
							{
								LogEx(ex);
								taskCompletionSource.SetResult(false);
							}
						});
				}
				else
				{
					await d.RunIdleAsync(async (_) =>
					{
						try
						{
							await func();
							taskCompletionSource.SetResult(true);
						}
						catch (Exception ex)
						{
							LogEx(ex);
							taskCompletionSource.SetResult(false);
						}
					});

				}
				await taskCompletionSource.Task;
			}
		}
		public static SemaphoreSlim uiSema = new SemaphoreSlim(1);


		public static bool isUISemaLocked => uiSema.CurrentCount != 1;
		public static async Task<T>
			DispatchOnUIThreadExclusive<T>(int cid,Func<Task<T>> func, CoreDispatcherPriority priority = CoreDispatcherPriority.Low)
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
			DispatchOnUIThreadExclusive(int cid, Func<Task> func, CoreDispatcherPriority priority = CoreDispatcherPriority.Low)
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
		
	public static void DispatchOnUIThread(DispatchedHandler action, CoreDispatcherPriority priority= CoreDispatcherPriority.Normal, bool alwaysQueue = false)
	{
		var d = GlobalDispatcher();
		// run it immediately if we can
		if (d.HasThreadAccess && !alwaysQueue)
			action();
		else
			d.TryRunAsync(priority, action);
	}

		public static void DispatchOnUIThreadIdle(DispatchedHandler action)
		{
			DispatchOnUIThread(action, CoreDispatcherPriority.Low, false);
//			var d = GlobalDispatcher();
//			d.TryRunIdleAsync((_)=> action() );
		}

		public static void DispatchOnUIThreadLow(DispatchedHandler action, bool alwaysQueue = false) => DispatchOnUIThread(action, CoreDispatcherPriority.Low, alwaysQueue);

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

		//public static void DispatchOnUIThreadSneakyLow(DispatchedHandler action)
		//{
		//	var d = GlobalDispatcher();
		//	// run it immediately if we can
		//	if (d.HasThreadAccess && d.CurrentPriority <= CoreDispatcherPriority.Low)
		//		action();
		//	else
		//		d.RunAsync(CoreDispatcherPriority.Low, action);
		//}
		//public static async Task DispatchOnUIThreadSneakyLowAwait(DispatchedHandler action)
		//{
		//	var d = GlobalDispatcher();
		//	// run it immediately if we can
		//	if (d.HasThreadAccess && d.CurrentPriority <= CoreDispatcherPriority.Low)
		//		action();
		//	else
		//		await d.RunAsync(CoreDispatcherPriority.Low, action);
		//}





		// We only have 1 UI thread here
		public static DispatcherQueue GlobalDispatcher() => globalQueue;
		public static DispatcherQueue globalQueue;
		public static bool IsOnUIThread() => globalQueue.HasThreadAccess };
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
			App.DispatchOnUIThreadLow(() =>
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


		public static VirtualKeyModifiers canvasKeyModifiers;
		// HTML control messs wit this
		public static VirtualKeyModifiers keyModifiers => canvasKeyModifiers;

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

		public static CoreCursor cursorDefault = new(CoreCursorType.Arrow, 0);
		public static CoreCursor cursorQuickBuild = new(CoreCursorType.Cross, 0);
		public static CoreCursor cursorMoveStart = new(CoreCursorType.SizeNortheastSouthwest, 0);
		public static CoreCursor cursorMoveEnd = new(CoreCursorType.SizeNorthwestSoutheast, 0);
		public static CoreCursor cursorLayout = new(CoreCursorType.Pin, 0);
		public static CoreCursor cursorDestroy = new(CoreCursorType.UniversalNo, 0);

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
			var but = sender as Button;
			Assert(but != null);
			if (but != null)
			{
				var fly = but.FindParent<FlyoutPresenter>();
				Assert(fly != null);
				fly?.ContextFlyout?.Hide();
			}

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

		public static async Task<ContentDialogResult> ShowAsync2(this ContentDialog dialog)
		{

			await popupSema.WaitAsync().ConfigureAwait(true);
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
			if (source != null & source.XamlRoot != null)
				target.XamlRoot = source.XamlRoot;
		}
		public static void CopyXamlRoomFrom(this UIElement target, UIElement source)
		{
			if (source != null & source.XamlRoot != null)
				target.XamlRoot = source.XamlRoot;
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

		public static void Set(this CoreCursor type)
		{
			// is this thread safe?
			if (ShellPage.coreInputSource != null)
				ShellPage.coreInputSource.PointerCursor = type;
		}

	}

	public static class Note
	{
		static bool initialized = false;
		static Note()
		{

		}

		private static void InAppNote_Closed(object sender, InAppNotificationClosedEventArgs e)
		{
			currentPriority = Priority.none;
			if (e.DismissKind == InAppNotificationDismissKind.User)
			{
				cancellationTokenSource.Cancel();
				cancellationTokenSource = new CancellationTokenSource();
			}
		}

		// [Conditional("TRACE")]
		public static void L(string s)
		{
			ChatTab.L(s);
		}
		public enum Priority
		{
			none,
			low, // if one is active, drop this
			medium, // if one is active wait
			high // if one is active cancel it
		}
		static Priority currentPriority;
		static DateTime nextInAppNote = new DateTime(0);
		static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		public static async void Show(string s, Priority priority=Priority.medium, bool useInfoBar = false, int timeout = 5000)
		{
			const int noteDelay = 2;
			if (ShellPage.instance != null)
			{
				if (!initialized)
				{
					initialized = true;
					App.DispatchOnUIThreadLow(() =>
					{
						ShellPage.inAppNote.Closed += InAppNote_Closed;
						//		ShellPage.instance.infoBar.CloseButtonClick += InfoBar_CloseButtonClick;
						//		ShellPage.instance.infoMD.LinkClicked += MarkDownLinkClicked;
					});
				}
				App.DispatchOnUIThreadLow(() =>
				{
					ChatTab.L(s);
				});

				var now = DateTime.UtcNow;
				var next = nextInAppNote;
				var _priority = priority;
				if (now >= next || ((priority >= Priority.high)&&(currentPriority< Priority.high)))
				{
					// all clear
					nextInAppNote = now + TimeSpan.FromSeconds((priority >= Priority.high) ? timeout: noteDelay);
				}
				else
				{
					if (priority == Priority.low)
						return;
					var wait = (next - now);
					if (wait.TotalSeconds >= 20.0f && priority < Priority.high)
						return;

					nextInAppNote = next + TimeSpan.FromSeconds(noteDelay);

					try
					{
						await Task.Delay(wait, cancellationTokenSource.Token);

					}
					catch (Exception _exception)
					{
						Log(_exception.Message);
						return;
					}

				}

				App.DispatchOnUIThreadLow(() =>
				{
					//ChatTab.L(s);
					var wasOpen = false;
					currentPriority = _priority;
					//if (ShellPage.instance.infoBar.IsOpen)
					//{
					//	wasOpen = true;
					//	ShellPage.instance.infoBar.IsOpen = false;
					//}
					//if (!useInfoBar)
					{
						var textBlock = new MarkdownTextBlock() { Text = s, Background = null };
						textBlock.LinkClicked += MarkDownLinkClicked;
						ShellPage.inAppNote.Show(textBlock, timeout);

						//ShellPage.instance.infoBar.IsOpen = false;
					}
					//else
					//{
					//	var textBlock = new MarkdownTextBlock() { Text = s, Background = null };
					//	ShellPage.instance.infoMD.Text = s;
					//	if (wasOpen)
					//	{
					//		Task.Delay(500).ContinueWith((_) => ShellPage.instance.infoBar.IsOpen = true, TaskScheduler.FromCurrentSynchronizationContext());
					//	}
					//	else
					//	{
					//		ShellPage.instance.infoBar.IsOpen = true;
					//	}
					//}
				});


			}
		}

		private static void InfoBar_CloseButtonClick(Microsoft.UI.Xaml.Controls.InfoBar sender, object args)
		{
			cancellationTokenSource.Cancel();
			cancellationTokenSource = new CancellationTokenSource();
		}

		static Regex regexCoordsTag = new Regex(@" ?\<coords\>(\d{1,3}:\d{1,3})\<\/coords\>", RegexOptions.CultureInvariant | RegexOptions.Compiled);
		static Regex regexPlayer = new Regex(@" ?\<player\>(\w+)\<\/player\>", RegexOptions.CultureInvariant | RegexOptions.Compiled);
		static Regex regexAlliance = new Regex(@" ?\<alliance\>(\w+)\<\/alliance\>", RegexOptions.CultureInvariant | RegexOptions.Compiled);
		static Regex regexReport = new Regex(@" ?\<report\>(\w+)\<\/report\>", RegexOptions.CultureInvariant | RegexOptions.Compiled);
		public static string TranslateCOTGChatToMarkdown(string s)
		{

			s = regexCoordsTag.Replace(s, @" [$1](/c/$1)");
			s = regexPlayer.Replace(s, @" [$1](/p/$1)");
			s = regexAlliance.Replace(s, @" [$1](/a/$1)");
			s = regexReport.Replace(s, @" [Report:$1](/r/$1)");
			return s;
		}
		public static void MarkDownLinkClicked(object sender, LinkClickedEventArgs e)
		{

			try
			{
				if (e.Link.StartsWith("http", StringComparison.OrdinalIgnoreCase)||e.Link.StartsWith("mailto"))
				{
					Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link));
				}
				else
				{
					var paths = e.Link.Split('/');
					Assert(paths[0].Length == 0);
					switch (paths[1])
					{
						case "s":
							{
								var cid = paths[2].FromCoordinate();
								DoSetup(cid);
							}
							break;
						case "c":
							Spot.ProcessCoordClick(paths[2].FromCoordinate(), false, App.keyModifiers, false);
							break;
						case "p": // player
							JSClient.ShowPlayer(paths[2]);
							break;
						case "a": // Alliance
							JSClient.ShowAlliance(paths[2]);
							break;
						case "r": // Report
							JSClient.ShowReport(paths[2]);
							break;
					}
				}
			}
			catch (Exception ex)
			{
				LogEx(ex);
			}
		}

		private static async Task DoSetup(int cid)
		{
			try
			{
				await ShellPage.instance.RefreshX();
			}
			catch (Exception ex)
			{
				LogEx(ex);
			}
			for (int i = 0; ; ++i)
			{
				await GetCity.Post(cid);
				if (City.CanVisit(cid))
					break;
				if( i > 5)
				{
					Trace("City not here");
				}
				await Task.Delay(1000);
			}

			await 	App.DispatchOnUIThreadExclusive(cid, async () =>
			{
				return await CityRename.RenameDialog(cid, true);

			});
		}

		public static void Focus(this Telerik.UI.Xaml.Controls.Grid.RadDataGrid ob)
		{
			if (ob != null)
			{
			//	ShellPage.instance.commandBar.Focus(FocusState.Programmatic);

				App.DispatchOnUIThreadLow(() => ob.Focus(FocusState.Programmatic));
			}
		}
		//public static void Focus(this Windows.UI.Xaml.Controls.Control ob)
		//{
		//	if (ob != null)
		//	{
		//	//	ShellPage.keyboardProxy.Focus(FocusState.Programmatic);

		//		App.DispatchOnUIThreadIdle(() => ob.Focus(FocusState.Programmatic));
		//	}
		//}

		static string lastTip;
		public static void ProcessTooltipsOnPointerMoved(object sender, PointerRoutedEventArgs e)
		{
			var info = Spot.HitTest(sender, e);
			var str = info.column?.Column?.Tip ?? string.Empty;
			ShowTip(str);
		}
		public static void ShowTip(string str)
		{
			if (str != lastTip)
			{
				lastTip = str;
				App.DispatchOnUIThreadLow(() =>
			   TabPage.mainTabs.tip.Text = str); // Todo:  use the correct tabPage
			}
		}
		public static void ProcessTooltips(this Telerik.UI.Xaml.Controls.Grid.RadDataGrid grid)
		{
			grid.PointerMoved -= ProcessTooltipsOnPointerMoved;
			grid.PointerMoved += ProcessTooltipsOnPointerMoved;
		}
	}


}
