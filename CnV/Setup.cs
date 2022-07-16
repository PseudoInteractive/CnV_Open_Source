
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;
using Microsoft.Windows.AppLifecycle;

using System.Diagnostics;
using System.Runtime.InteropServices;

using WinRT.Interop;

namespace CnV;

using static AppS;


partial class App
{

	public static App instance;
	public static string appLink = "cnv";

	//private static readonly IHost _host = Host
	//       .CreateDefaultBuilder()
	//       .ConfigureServices((context, services) =>
	//       {
	//           // Default Activation Handler
	// //          services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

	//           // Other Activation Handlers

	//           // Services
	//   //        services.AddSingleton<ILocalSettingsService, LocalSettingsServicePackaged>();
	//    //       services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
	//     //      services.AddTransient<INavigationViewService, NavigationViewService>();

	//      //     services.AddSingleton<IActivationService, ActivationService>();
	//       //    services.AddSingleton<IPageService, PageService>();
	//        //   services.AddSingleton<INavigationService, NavigationService>();

	//           // Core Services
	//        //   services.AddSingleton<IFileService, FileService>();

	//           // Views and ViewModels
	//          // services.AddTransient<SettingsViewModel>();
	//          // services.AddTransient<SettingsPage>();
	//           // Configuration
	// //          services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
	//       })
	//       .Build();

	//public static T GetService<T>()
	//    where T : class
	//{
	//    return _host.Services.GetService(typeof(T)) as T;
	//}

	public static void InitAppCenter() {
#if APPCENTER
		if(AAnalytics.initialized)
			return;
		//if(AppCenter.Configured)
		//{
		//	return;
		//}
		//AppCenter.SetMaxStorageSizeAsync(16 * 1024 * 1024).ContinueWith((storageTask) => {
		//	// The storageTask.Result is false when the size cannot be honored.
		//});
#if DEBUG
		//AppCenter.LogLevel = LogLevel.Verbose;
#endif
		//			AppCenter.Configure("windowsdesktop=0b4c4039-3680-41bf-b7d7-685eb68e21d2");
		AppCenter.Start("0b4c4039-3680-41bf-b7d7-685eb68e21d2",typeof(Analytics)
				   ,typeof(Crashes));
		//	AppCenter.LogLevel = System.Diagnostics.Debugger.IsAttached ? Microsoft.AppCenter.LogLevel.Warn : Microsoft.AppCenter.LogLevel.None;


		AAnalytics.initialized = true;
		//	var args = AppInstance.GetCurrent().GetActivatedEventArgs();
		//AAnalytics.Track("Activate",new Dictionary<string,string> { { "kind",args.Kind.ToString() },

		//	{"args" ,appArgs } });
		//await Task.WhenAll(
		//			Analytics.SetEnabledAsync(true);
		//		Crashes.SetEnabledAsync(true);



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


	public App() {


		//			services = ConfigureServices();
		RequestedTheme = ApplicationTheme.Dark;
		InitializeComponent();
		//Microsoft.UI.Xaml.Media.Animation.Timeline.AllowDependentAnimations=false;
		//		UnhandledException += App_UnhandledException;
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
		//	_activationService = new Lazy<ActivationService>(CreateActivationService);
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

	//private void App_UnhandledException(object sender,Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
	//{
	//	e.Handled = true;
	//	System.Diagnostics.Debug.WriteLine($"Unhandled Exception: " + e.Message);
	//	System.Diagnostics.Debug.WriteLine(e.Exception.StackTrace);

	//}

	private void TaskScheduler_UnobservedTaskException(object sender,UnobservedTaskExceptionEventArgs e) {
		e.SetObserved();
		LogEx(e.Exception);
	}
	protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args) {
		base.OnLaunched(args);
		try {
			//await AppS.StartHost(OnReady:async () =>
			{
				try {
					AppInstance keyInstance = AppInstance.FindOrRegisterForKey("cnv");


					// If we successfully registered the file name, we must be the
					// only instance running that was activated for this file.
					if(keyInstance.IsCurrent) {
						// Report successful file name key registration.


						// Hook up the Activated event, to allow for this instance of the app
						// getting reactivated as a result of multi-instance redirection.
						//   keyInstance.Activated += OnActivated;
					}
					else {
						AppS.appAlreadyRunning=true;
						//var mainInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("main");

						// If the instance that's executing the OnLaunched handler right now
						// isn't the "main" instance.
						//if(!mainInstance.IsCurrent) {
						// Redirect the activation (and args) to the "main" instance, and exit.
						var activatedEventArgs =
							Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
						await keyInstance.RedirectActivationToAsync(activatedEventArgs);
						System.Diagnostics.Process.GetCurrentProcess().Kill();
						return;
						//}

						//                        isRedirect = true;
						//                       RedirectActivationTo(args, keyInstance);
					}
				}
				catch(Exception ex) {
					Trace(ex.ToString());
				}


				Assert(AppS.state == AppS.State.loading);
				AppS.SetState(AppS.State.init);

				//	CnVFont = new FontFamily("{StaticResource CnvIcons}");

				//	Windows.UI.ViewManagement.ApplicationView.PreferredLaunchWindowingMode =Windows.UI.ViewManagement.ApplicationViewWindowingMode.Maximized;// new Size(bounds.Width, bounds.Height);
				//				Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterViewModeAsync(Windows.UI.ViewManagement.ApplicationViewMode.CompactOverlay);

				//	FocusVisualKind = FocusVisualKind.Reveal;

				window = new();
				AppS.globalQueue = window.DispatcherQueue;
				//	window.




				//var view = DisplayInformation.GetForCurrentView();
				//var uwpArgs = AppInstance.GetCurrent().GetActivatedEventArgs(); //args.UWPLaunchActivatedEventArgs;
				//if(uwpArgs.Kind == ExtendedActivationKind.Protocol)
				//{
				//	var eventArgs = uwpArgs;
				//	Log("Args!! " + eventArgs.Uri);
				//	var s = System.Web.HttpUtility.ParseQueryString(eventArgs.Uri.Query);

				//	Debug.Log(s);
				//	// format $"cnv:launch?w={world}&s=1&n=1"
				//	// are / chars inserted?
				//	//  if (s.Length >= 3)
				//	{
				//		//if (AMath.TryParseInt(s["s"], out int _s))
				//		//	CnVServer.subId = _s;

				//		////var n = s["p"];
				//		////if (n != null)
				//		////	Player.subOwner = n;

				//		//if (AMath.TryParseInt(s["w"], out int _w))
				//		//	CnVServer.world = _w;

				//		//						if(AMath.TryParseInt(s["n"],out int _n)) // new instance
				//		//							key = "cotgaMulti" + DateTimeOffset.UtcNow.UtcTicks;

				//	}
				//}

				//// Get the screen resolution (APIs available from 14393 onward).
				//var resolution = new Size(view.ScreenWidthInRawPixels, view.ScreenHeightInRawPixels);
				//// Calculate the screen size in effective pixels. 
				//// Note the height of the Windows Taskbar is ignored here since the app will only be given the maxium available size.
				//var scale = view.ResolutionScale == ResolutionScale.Invalid ? 1 : view.RawPixelsPerViewPixel;
				//var bounds = new Size(resolution.Width / scale, resolution.Height / scale);


				IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
				WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);

				AppS.appWindow= AppWindow.GetFromWindowId(myWndId);
				try {







					//					AppS.appWindow.Title = $"Conquest and Virtue Alpha - sign in to Discord (version {AppS.currentVersion})";
					//					AppS.appWindow.SetIcon("assets\\cnv.ico");
				}
				catch(Exception ex) {
					Log(ex);
				}
				//				
				//				window.SetTitleBar
				//ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
				//		window.ExtendsContentIntoTitleBar = true;
				//	window.ExtendsContentIntoTitleBar = true;
				//App.globalDispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

				//	keyQueue = globalQueue.CreateTimer();
				//CoreApplication.EnablePrelaunch(false);
				//if (uwpArgs.Kind == Windows.ApplicationModel.Activation.ActivationKind.Launch)
				//{
				//	// do this asynchronously

				//}



				// if (!args.PrelaunchActivated)

				await OnLaunchedOrActivated(args);
				//	InitAppCenter();
				//					var w2 = new Window();
				//w2.Content = new TextBlock() { Text = "Hello" };
				//w2.Activate();

				//if(uwpArgs.Kind == Windows.ApplicationModel.Activation.ActivationKind.Launch)
				Services.StoreHelper.instance.DownloadAndInstallAllUpdatesAsync(hWnd);
			}
			//);

		}
		catch(Exception e) {
			Log(e);
		}

	}


	private async Task OnLaunchedOrActivated(Microsoft.UI.Xaml.LaunchActivatedEventArgs args) {


		try {
			var status = DeploymentManager.GetStatus();
			//Trace($"{DeploymentManager.GetStatus().Status}");
			if(status.Status != DeploymentStatus.Ok) {
				//	// Initialize does a status check, and if the status is not Ok it will attempt to get
				//	// the WindowsAppRuntime into a good state by deploying packages. Unlike a simple
				//	// status check, Initialize can sometimes take several seconds to deploy the packages.
				//	// These should be run on a separate thread so as not to hang your app while the
				// packages deploy. 
				//	Trace("init start");
				await Task.Run(() => DeploymentManager.Initialize(new DeploymentInitializeOptions() { ForceDeployment=true }));
				//Trace("init end");

			}

		}
		catch(Exception _ex) {
			Log(_ex);

		}

		try {

#if DEBUG
			//				this.DebugSettings.FailFastOnErrors = false;
			//this.DebugSettings.FailFastOnErrors                      = true;
			this.DebugSettings.EnableFrameRateCounter                = false;
			this.DebugSettings.IsTextPerformanceVisualizationEnabled = false;
			//this.DebugSettings.FailFastOnErrors = false;
			this.DebugSettings.IsBindingTracingEnabled = true;
			this.DebugSettings.BindingFailed+=DebugSettings_BindingFailed1;

#endif
			var wasRunning = false;// args.PreviousExecutionState   == ApplicationExecutionState.Running
								   //		|| args.PreviousExecutionState == ApplicationExecutionState.Suspended;
			Assert(!wasRunning);
			if(!wasRunning) {
				//	var window = Window.Current;
				window.VisibilityChanged += Window_VisibilityChanged;

				AppS.appWindow.Closing+=AppWindow_Closing;
				AppS.appWindow.Destroying+=AppWindow_Destroying; ;
				//		window.WantClose+=Window_Closing;
				//window.Activated+=Window_Activated;
			}
			//		SystemInformation.Instance.TrackAppUse(args.UWPLaunchActivatedEventArgs);
			// can this be async?
			//typeof(Telerik.UI.Xaml.Controls.RadDataForm).Assembly.GetType("Telerik.UI.Xaml.Controls.TelerikLicense").GetField("messageDisplayed",BindingFlags.NonPublic|BindingFlags.Static).SetValue(null,true,BindingFlags.Static|BindingFlags.NonPublic,null,null);

			//	typeof(Syncfusion.Licensing.SyncfusionLicenseProvider).Assembly.GetType("SyncfusionLicenseProvider").GetMember("ValidateLicense",BindingFlags.Public|BindingFlags.Static)=(null,true,BindingFlags.Static|BindingFlags.Public,null,null);
			//Log("here");
			Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjY1MzMyQDMyMzAyZTMyMmUzMGc1ZnZYYzZGTWh3emQzVFZQWjZGbi9IRWx5Mk96dDQ3TTFSL1luNUtlaVU9");

			if(!wasRunning) {
				Log("load");
				await Sim.LoadJsons();
			}
			const bool isInteractive = true;

			//if(IsInteractive(activationArgs))
			{
				// Initialize services that you need before app activation
				// take into account that the splash screen is shown while this code runs.
				//    UserDataService.Initialize();
				//              await IdentityService.InitializeWithAadAndPersonalMsAccounts();

				// Do not repeat app initialization when the Window already has content,
				// just ensure that the window is active
				if(AppS.window.Content is null) {
					// Create a Shell or Frame to act as the navigation context
					//	App.instance.Resources["TabViewBackground"] = new SolidColorBrush();
					//	App.instance.Resources["TabViewButtonBackground"] = new SolidColorBrush();
					//	App.instance.Resources["TabViewButtonForeground"] = new SolidColorBrush();
					//	App.instance.Resources["OverlayCornerRadius"] = 1.0;
					//	App.instance.Resources["TopCornerRadiusFilterConverter"] = new object();

					AppS.window.Content = new ShellPage();

				}

			}


			// Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
			// will navigate to the first page
			//	await HandleActivationAsync(activationArgs);
			//  _lastActivationArgs = activationArgs;

			//				("NTUwMDAxQDMxMzkyZTM0MmUzMFJnano4Uk4veXEvQmczQ2M5eWZQQ1JUT0UyVVJwamhxcEZjRWEvL3V4ZkE9;NTUwMDAyQDMxMzkyZTM0MmUzMENITkt6cXZtZ2oxZkFTa09HMmkxRXlFaVRhQjRUN1dUQzc2VHNDeXU4TWc9");
			if(isInteractive) {
				var activation = args;
				//if(activation.PreviousExecutionState == ApplicationExecutionState.Terminated)
				//{
				//	//        await Singleton<SuspendAndResumeService>.Instance.RestoreSuspendAndResumeData();
				//}

				//var title = AppS.appWindow.TitleBar;
				//if(title is not null)
				//{
				//	var c = Windows.UI.Color.FromArgb(0xFF,0x34,0x0B,0x0B);
				//	title.BackgroundColor = c;
				//	title.ButtonBackgroundColor = Windows.UI.Color.FromArgb(0xFF,0x24,0x0B,0x0B); ;
				//}
				// Ensure the current window is active
				//await Task.Delay(500);
				Log("Activate!");
				AppS.appWindow.Show(true);

				AppS.presenter.Maximize();
				//await Task.Delay(500);
				Log("Max");
				//if(Program.appAlreadyRunning) {
				//	await AppS.QueueOnUIThreadTask(async void () => {
				//		try {
				//			var dialog = new ContentDialog() {

				//				Title = "Conquest and Virtue already running",
				//				Content = "Please close all game windows, of if you see none, restart computer",
				//				PrimaryButtonText = "Close",
				//				SecondaryButtonText = "Ignore",
				//				IsSecondaryButtonEnabled = true,
				//				IsPrimaryButtonEnabled = true,



				//			};
				//			//	dialog.XamlRoot = AppS.window.Content.XamlRoot;
				//			//dialog.Hide();


				//			var hr = await dialog.ShowAsync2().ConfigureAwait(false);
				//			if(hr != ContentDialogResult.Secondary) {
				//				Application.Current.Exit();
				//				await Task.Delay(-1).ConfigureAwait(false);
				//			}
				//		}

				//		catch(Exception _ex) {
				//			LogEx(_ex);

				//		}

				//	});

				//}
				//				App.window.Maximize();
				//				_ = PInvoke.User32.ShowWindow(WinRT.Interop.WindowNative.GetWindowHandle(App.window), PInvoke.User32.WindowShowStyle.SW_MAXIMIZE);
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


			if(wasRunning)
				return;

			window.Content.PreviewKeyUp   += Content_PreviewKeyUp;
			window.Content.PreviewKeyDown += Content_PreviewKeyDown;
			;
			//			window.KeyDown+=Window_KeyDown;

			//			CoreApplication.MainView.HostedViewClosing+=MainView_HostedViewClosing; ;
			//	CoreApplication.MainView.CoreWindow.Closed+=CoreWindow_Closed;
			//if(args!=null)
			//	SystemInformation.TrackAppUse(args);



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

			await Task.Delay(5000);
			AppS.QueueOnUIThread(InitAppCenter);
			// Set XAML element as a draggable region.
			//          Window.Current.SetTitleBar(ShellPage.instance.AppTitleBar);
		}
		catch(Exception e) {
			Log(e);
		}
	}

	private const int WAINACTIVE = 0x00;
	private const int WAACTIVE = 0x01;
	private const int WMACTIVATE = 0x0006;

	[DllImport("user32.dll")]
	private static extern IntPtr GetActiveWindow();

	[DllImport("user32.dll",CharSet = CharSet.Auto)]
	private static extern IntPtr SendMessage(IntPtr hWnd,int msg,int wParam,IntPtr lParam);

	[Conditional("CNVCLIENT")]
	internal static async void UpdateAppTitle() {
		await Task.Delay(2000);
		QueueOnUIThreadIdle(UpdateAppTitleUI);
	}

	internal static void UpdateAppTitleUI() {

		var title = (Player.activeId!=0) ?
				(Sim.isPastWarmup ? Player.active.shortName
				: $"Conquest and Virtue Alpha W{World.id} - {Player.active.shortName}\nVersion: {AppS.currentVersion}")
				: $"Conquest and Virtue Alpha W{World.id} Sign in to Discord\nVersion: {AppS.currentVersion}";

		try {
			if(AppWindowTitleBar.IsCustomizationSupported()) {
				ShellPage.instance.AppTitleBarText.Text = title;
				var tb = appWindow.TitleBar;
				 var c = Windows.UI.Color.FromArgb(0xFF,0x34,0x0B,0x0B);
						tb.BackgroundColor = c;
						tb.ButtonBackgroundColor = Windows.UI.Color.FromArgb(0xFF,0x24,0x0B,0x0B); ;

			}
			else {
				appWindow.Title = title;
			}

		}
		catch(Exception ex) {
			LogEx(ex);
		}

		//    Application.Current.Resources["WindowCaptionForeground"] = theme switch
		//    {
		//        ElementTheme.Dark => new SolidColorBrush(Colors.White),
		//        ElementTheme.Light => new SolidColorBrush(Colors.Black),
		//        _ => new SolidColorBrush(Colors.Transparent)
		//    };

		//    Application.Current.Resources["WindowCaptionForegroundDisabled"] = theme switch
		//    {
		//        ElementTheme.Dark => new SolidColorBrush(Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF)),
		//        ElementTheme.Light => new SolidColorBrush(Color.FromArgb(0x66, 0x00, 0x00, 0x00)),
		//        _ => new SolidColorBrush(Colors.Transparent)
		//    };

		//    Application.Current.Resources["WindowCaptionButtonBackgroundPointerOver"] = theme switch
		//    {
		//        ElementTheme.Dark => new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF)),
		//        ElementTheme.Light => new SolidColorBrush(Color.FromArgb(0x33, 0x00, 0x00, 0x00)),
		//        _ => new SolidColorBrush(Colors.Transparent)
		//    };

		//    Application.Current.Resources["WindowCaptionButtonBackgroundPressed"] = theme switch
		//    {
		//        ElementTheme.Dark => new SolidColorBrush(Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF)),
		//        ElementTheme.Light => new SolidColorBrush(Color.FromArgb(0x66, 0x00, 0x00, 0x00)),
		//        _ => new SolidColorBrush(Colors.Transparent)
		//    };

		//    Application.Current.Resources["WindowCaptionButtonStrokePointerOver"] = theme switch
		//    {
		//        ElementTheme.Dark => new SolidColorBrush(Colors.White),
		//        ElementTheme.Light => new SolidColorBrush(Colors.Black),
		//        _ => new SolidColorBrush(Colors.Transparent)
		//    };

		//    Application.Current.Resources["WindowCaptionButtonStrokePressed"] = theme switch
		//    {
		//        ElementTheme.Dark => new SolidColorBrush(Colors.White),
		//        ElementTheme.Light => new SolidColorBrush(Colors.Black),
		//        _ => new SolidColorBrush(Colors.Transparent)
		//    };
		//}
		
		//	appWindow.TitleBar.ButtonBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(0x80,0x40,0x20,0x20); 
		//			appWindow.TitleBar.ButtonForegroundColor = Microsoft.UI.ColorHelper.FromArgb(0xff,0x80,0x40,0x60); 
		//	appWindow.TitleBar.ButtonHoverBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(0x66,0x40,0x20,0x44); 
		//	appWindow.TitleBar.ButtonPressedBackgroundColor = Microsoft.UI.ColorHelper.FromArgb(0xff,0xff,0xff,0xff); 
		//          Application.Current.Resources["WindowCaptionBackground"] = new SolidColorBrush(Colors.Transparent);
		//          Application.Current.Resources["WindowCaptionBackgroundDisabled"] = new SolidColorBrush(Colors.Transparent);
		//Application.Current.Resources["WindowCaptionButtonBackgroundPressed"] =  AppS.Brush(0x66,0x40,0x20,0x20);
		//Application.Current.Resources["WindowCaptionButtonBackgroundPointerOver"] =  AppS.Brush(0x66,0x40,0x40,0x40);
		//Application.Current.Resources["WindowCaptionForeground"] =  AppS.Brush(0x66,0x40,0x40,0x90);
		//Application.Current.Resources["WindowCaptionBackgroundDisabled"] = new SolidColorBrush(Colors.Transparent);
		//Application.Current.Resources["WindowCaptionBackgroundDisabled"] = new SolidColorBrush(Colors.Transparent);

		var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(AppS.window);
		if(hwnd == GetActiveWindow()) {
			SendMessage(hwnd,WMACTIVATE,WAINACTIVE,IntPtr.Zero);
			SendMessage(hwnd,WMACTIVATE,WAACTIVE,IntPtr.Zero);
		}
		else {
			SendMessage(hwnd,WMACTIVATE,WAACTIVE,IntPtr.Zero);
			SendMessage(hwnd,WMACTIVATE,WAINACTIVE,IntPtr.Zero);
		}
	}
	internal static void SetupTitleBar() {

		// Check to see if customization is supported.
		// Currently only supported on Windows 11.
		try {
			if(AppWindowTitleBar.IsCustomizationSupported()) {
				var titleBar = appWindow.TitleBar;
				titleBar.ExtendsContentIntoTitleBar = true;
				ShellPage.instance.AppTitleBar.Loaded += AppTitleBar_Loaded;
				ShellPage.instance.AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
				return;
			}
		}
		catch(Exception ex) {
			LogEx(ex);
		}

		{
			// Title bar customization using these APIs is currently
			// supported only on Windows 11. In other cases, hide
			// the custom title bar element.
			ShellPage.instance.AppTitleBarText.Visibility = Visibility.Collapsed; // don't draw text here
			AppS.appWindow.SetIcon("assets\\cnv.ico");

			// Show alternative UI for any functionality in
			// the title bar, such as search.
		}

	}

	static void AppTitleBar_Loaded(object sender,RoutedEventArgs e) {
		// Check to see if customization is supported.
		// Currently only supported on Windows 11.
		if(AppWindowTitleBar.IsCustomizationSupported()) {
			SetDragRegionForCustomTitleBar(appWindow);
		}
	}

	private static void AppTitleBar_SizeChanged(object sender,SizeChangedEventArgs e) {
		// Check to see if customization is supported.
		// Currently only supported on Windows 11.
		if(AppWindowTitleBar.IsCustomizationSupported()
			&& appWindow.TitleBar.ExtendsContentIntoTitleBar) {
			// Update drag region if the size of the title bar changes.
			SetDragRegionForCustomTitleBar(appWindow);
		}
	}

	private static AppWindow GetAppWindowForCurrentWindow() {
		IntPtr hWnd = WindowNative.GetWindowHandle(window);
		WindowId wndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
		return AppWindow.GetFromWindowId(wndId);
	}

	[DllImport("Shcore.dll",SetLastError = true)]
	internal static extern int GetDpiForMonitor(IntPtr hmonitor,Monitor_DPI_Type dpiType,out uint dpiX,out uint dpiY);

	internal enum Monitor_DPI_Type:int
	{
		MDT_Effective_DPI = 0,
		MDT_Angular_DPI = 1,
		MDT_Raw_DPI = 2,
		MDT_Default = MDT_Effective_DPI
	}

	private static double GetScaleAdjustment() {
		IntPtr hWnd = WindowNative.GetWindowHandle(window);
		WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
		DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId,DisplayAreaFallback.Primary);
		IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

		// Get DPI.
		int result = GetDpiForMonitor(hMonitor,Monitor_DPI_Type.MDT_Default,out uint dpiX,out uint _);
		if(result != 0) {
			throw new Exception("Could not get DPI for monitor.");
		}

		uint scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
		return scaleFactorPercent / 100.0;
	}

	private static void SetDragRegionForCustomTitleBar(AppWindow appWindow) {
		// Check to see if customization is supported.
		// Currently only supported on Windows 11.
		if(AppWindowTitleBar.IsCustomizationSupported()
			&& appWindow.TitleBar.ExtendsContentIntoTitleBar) {
			var i = ShellPage.instance;
			double scaleAdjustment = GetScaleAdjustment();

			i.RightPaddingColumn.Width = new GridLength(appWindow.TitleBar.RightInset / scaleAdjustment);
			i.LeftPaddingColumn.Width = new GridLength(appWindow.TitleBar.LeftInset / scaleAdjustment);

			List<Windows.Graphics.RectInt32> dragRectsList = new();

			Windows.Graphics.RectInt32 dragRectL;
			var x0 = (int)((i.LeftPaddingColumn.ActualWidth) * scaleAdjustment);
			const int leftStuff = 60+60+6;
			dragRectL.X = 0;
			dragRectL.Y = 0;
			dragRectL.Height = (int)(i.AppTitleBar.ActualHeight * scaleAdjustment-8);
			dragRectL.Width = x0+(int)(leftStuff * scaleAdjustment);
			dragRectsList.Add(dragRectL);

			Windows.Graphics.RectInt32 dragRectR;
			dragRectR.X = (int)(dragRectL.Width +dragRectL.X +
								(+i.PlayerButtonsColumn.ActualWidth
									+i.CommandColumn.ActualWidth) * scaleAdjustment);
			dragRectR.Y = 0;
			dragRectR.Height = (int)(i.AppTitleBar.ActualHeight * scaleAdjustment-8);
			dragRectR.Width = (int)(i.RightDragColumn.ActualWidth * scaleAdjustment);
			dragRectsList.Add(dragRectR);

			Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();

			appWindow.TitleBar.SetDragRectangles(dragRects);
		}
	}
}