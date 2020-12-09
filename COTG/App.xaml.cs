using System;
using static COTG.Debug;

using COTG.Core.Helpers;
using COTG.Services;

//using Microsoft.AppCenter;
//using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;

//using ZLogger;

//using Cysharp.Text;
using Microsoft.Toolkit.Uwp.UI.Controls;
using COTG.Views;
using System.Numerics;
using Windows.ApplicationModel.Core;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Windows.System;
using Windows.UI.Xaml.Controls;
using COTG.Helpers;
using COTG.Game;
using System.Diagnostics;
using Windows.Globalization.NumberFormatting;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using System.Collections.Concurrent;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Input;
using Windows.Foundation.Collections;
using System.Threading;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace COTG
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;

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



            InitializeComponent();
            instance = this;
            UnhandledException += OnAppUnhandledException;
            FocusVisualKind = FocusVisualKind.Reveal;



            EnteredBackground += App_EnteredBackground;
            LeavingBackground += App_LeavingBackground;
            Resuming += App_Resuming;

            // TODO WTS: Add your app in the app center and set your secret here. More at https://docs.microsoft.com/appcenter/sdk/getting-started/uwp

            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
            UserAgent.SetUserAgent(JSClient.userAgent);  // set webview useragent

        }
        // these are not reliably set
        // We set then on key up and key down events and on mouse input events
        static public bool shiftPressed, controlPressed;

        public static bool IsKeyPressedControl()
        {
            return controlPressed;
        }
        public static bool IsKeyPressedShift()
        {
            return shiftPressed;
        }
        static void OnKeyUp(CoreWindow sender, KeyEventArgs args)
        {
            var key = args.VirtualKey;
            OnKeyUp(key);
        }

        public static void OnKeyUp(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Shift:
                    shiftPressed = false;
                    break;
                case VirtualKey.Control:
                    controlPressed = false;
                    break;

            }
            App.DispatchOnUIThreadSneaky(ResetIdleTimer);
        }

        static bool webViewInFront = false;

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
                    shiftPressed = true;
                    break;
                case VirtualKey.Control:
                    controlPressed = true;
                    break;
                case VirtualKey.Scroll:
                    if (webViewInFront)
                    {
                        webViewInFront=false;
                        Canvas.SetZIndex(ShellPage.canvas, ShellPage.canvasZDefault);
                    }
                    else
                    {
                        webViewInFront=true;
                        Canvas.SetZIndex(ShellPage.canvas, ShellPage.canvasZBack);
                    }
                    break;
            }
            App.DispatchOnUIThreadSneaky(ResetIdleTimer);
        }

        private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            Trace("LeavingBackground");
            if (ShellPage.canvas != null)
                ShellPage.canvas.Paused = false;
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            Debug.Log(e.Message);
            e.Handled = true;
        }

        static DispatcherTimer idleTimer;
        static bool timerActive;
        private static ConcurrentQueue<Action> idleTasks = new ConcurrentQueue<Action>();
        private static ConcurrentQueue<Func<Task>> throttledTasks = new ConcurrentQueue<Func<Task>>();


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

            CoreApplication.EnablePrelaunch(false);
			await ActivationService.ActivateAsync(args);
			// if (!args.PrelaunchActivated)

			OnLaunchedOrActivated(args);
        }
        private void OnLaunchedOrActivated(LaunchActivatedEventArgs args)
        { 
            this.DebugSettings.FailFastOnErrors = false;
#if TRACE || DEBUG
         //   this.DebugSettings.FailFastOnErrors = true;
#endif
            this.DebugSettings.EnableFrameRateCounter = false;
            this.DebugSettings.IsTextPerformanceVisualizationEnabled = false;
#if DEBUG
            //this.DebugSettings.FailFastOnErrors = false;
            this.DebugSettings.IsBindingTracingEnabled = true;
#else
            this.DebugSettings.IsBindingTracingEnabled = false;
#endif
			if(args!=null)
				SystemInformation.TrackAppUse(args);
			if (idleTimer == null)
			{

				
				idleTimer = new DispatcherTimer();
				idleTimer.Interval = TimeSpan.FromSeconds(10);  // 16s idle delay, at most one event per 16 seconds?  What if too many are queued?
				idleTimer.Tick += IdleTimer_Tick;
				Assert(idleTimer.IsEnabled == false);
				ProcessThrottledTasks();
			}


			var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;
          //  UpdateTitleBarLayout(coreTitleBar);

            // Set XAML element as a draggable region.
  //          Window.Current.SetTitleBar(ShellPage.instance.AppTitleBar);
        }
        public static void SetupCoreWindowInputHooks()
        {
			
			foreach (var view in CoreApplication.Views)
            {
                Log($"{view.TitleBar.ToString()} {view.IsMain} ");
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
                e.Handled=true;

        }
        static async void ProcessThrottledTasks()
        {
            for (; ; )
            {


				try
				{
					if (!throttledTasks.IsEmpty)
					{
						if (throttledTasks.TryDequeue(out var t))
							await t();

					}
				}
				catch (Exception _exception)
				{
					COTG.Debug.Log(_exception);
				}


				await Task.Delay(500);
            }
        }
        public static void EnqeueTask(Func<Task> a)
        {
            throttledTasks.Enqueue(a);
        }
        public static bool OnPointerPressed( PointerUpdateKind prop)
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
            App.DispatchOnUIThreadSneaky( ResetIdleTimer );
            return rv;
        }

        private static void ResetIdleTimer()
        {
            if (timerActive)
            {
                idleTimer.Stop();
                idleTimer.Start();
            }
        }
        private static void OnPointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            // reset timer if active
            ResetIdleTimer();
        }

        private static void IdleTimer_Tick(object sender, object e)
        {
            if (idleTasks.Count < 1)
            {
                Log("no tasks for tick?");
            }
            if (idleTasks.Count <= 1)
            {
                timerActive = false;
                idleTimer.Stop();
            }
            if (idleTasks.TryDequeue(out Action a))
            {
				try
				{
					a();
				}
				catch (Exception _exception)
				{
					COTG.Debug.Log(_exception);
				}


			}
        }

        // with a delay
        public static void QueueIdleTask( Action a , int intialDelayInmilisecons)
        {
        
                Task.Delay(intialDelayInmilisecons).ContinueWith((_) => QueueIdleTask(a));
        }

        public static void QueueIdleTask(Action a)
        {
            foreach(var i in idleTasks)
            {
                if (i == a)
                    return;

            }

            
            {
                idleTasks.Enqueue(a);
                if (!timerActive)
                {
                    timerActive = true;
                    DispatchOnUIThreadSneaky(idleTimer.Start);
                }
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            var activation = args as IActivatedEventArgs;
            if (activation != null && activation.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                // Todo:  Handle arguments and stuff
                // Ensure the current window is active
                if (args is ToastNotificationActivatedEventArgs toastActivationArgs)
                {
                    // Obtain the arguments from the notification
                    var toastArgs = System.Web.HttpUtility.ParseQueryString(toastActivationArgs.Argument);
                    // Obtain any user input (text boxes, menu selections) from the notification
                    ValueSet userInput = toastActivationArgs.UserInput;
                    foreach(var op in toastArgs.AllKeys)
                    {
                        if(op == "incomingNotification")
                        {
                            Task.Delay(3000).ContinueWith( async (_) => 
							{
								while (IncomingTab.instance == null)
									await Task.Delay(500);
								IncomingTab.instance.Show();

							});
                        }
                    }
                    // TODO: Show the corresponding content
                }

               
                return;
            }
            await ActivationService.ActivateAsync(args);
            OnLaunchedOrActivated(args as LaunchActivatedEventArgs);
            //AppCenter.Start("0b4c4039-3680-41bf-b7d7-685eb68e21d2",
            //   typeof(Analytics), typeof(Crashes));



            //var configuration = new ConfigurationBuilder()
            //                                .AddJsonFile("appsettings.json", false, true)
            //                                .Build();




            //    CreateDefaultBuilder(args)
            //        .ConfigureWebHostDefaults(webBuilder =>
            //        {
            //            webBuilder.UseStartup<Startup>();
            //        }).Build().Run();


            //ILogger logger;

            //using (var serviceProvider = new ServiceCollection()
            //    .AddLogging(cfg =>
            //    {
            //        cfg.AddConfiguration(configuration.GetSection("Logging"));
            //        cfg.AddConsole();
            //    })
            //    .BuildServiceProvider())
            //{
            //    logger = serviceProvider.GetService<ILogger<App>>();
            //}

            //logger.LogInformation("logger information");
            //logger.LogWarning("logger warning");


            //using (var listener = new LoggerTraceListener(logger))
            //{
            //    System.Diagnostics.Trace.Listeners.Add(listener);
            //    TraceSources.Instance.InitLoggerTraceListener(listener);

            //    TraceLover.DoSomething();
            //    TraceSourceLover.DoSomething();
            //}


        }
        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, null, new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }

        private void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            Trace("Enter Background");
            if (ShellPage.canvas != null)
                ShellPage.canvas.Paused = true;
            SettingsPage.SaveAll();
            //            var deferral = e.GetDeferral();
            //            await Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();
            //           deferral.Complete();
        }

        private void App_Resuming(object sender, object e)
        {
            Trace("Resume");

            //         Singleton<SuspendAndResumeService>.Instance.ResumeApp();
        }

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            await ActivationService.ActivateFromShareTargetAsync(args);
        }
        public static void DispatchOnUIThread(DispatchedHandler action)
        {
            GlobalDispatcher().RunAsync(CoreDispatcherPriority.Normal, action);
        }
        public static void DispatchOnUIThreadLow(DispatchedHandler action)
        {
            GlobalDispatcher().RunAsync(CoreDispatcherPriority.Low, action);
        }
        public static void DispatchOnUIThreadSneaky(DispatchedHandler action)
        {
            var d = GlobalDispatcher();
            // run it immediately if we can
            if (d.HasThreadAccess)
                action();
            else
                d.RunAsync(CoreDispatcherPriority.Low, action);
        }
        public static async Task DispatchOnUIThreadSneakyTask(DispatchedHandler action)
        {
            var d = GlobalDispatcher();
            // run it immediately if we can
            if (d.HasThreadAccess)
                action();
            else
                await d.RunAsync(CoreDispatcherPriority.Low, action);
        }
        // We only have 1 UI thread here
        public static CoreDispatcher GlobalDispatcher() => ShellPage.instance.Dispatcher;

        public static bool IsOnUIThread() => GlobalDispatcher().HasThreadAccess;
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
        public static PercentFormatter percentFormatter = new PercentFormatter();// { FractionDigits = 1, NumberRounder=new IncrementNumberRounder() { Increment=.001,RoundingAlgorithm=RoundingAlgorithm.RoundHalfToEven} };
        public static DecimalFormatter formatter2Digit = new DecimalFormatter() { FractionDigits = 2, IsGrouped = true };
        public static DecimalFormatter formatterInt = new DecimalFormatter() { FractionDigits = 0, IsGrouped = true };
        public static DecimalFormatter formatterSeconds = new DecimalFormatter() { FractionDigits = 0, IntegerDigits = 2 };

        public static void CopyTextToClipboard(string s)
        {
            App.DispatchOnUIThreadSneaky(() =>
         {
             DataPackage dataPackage = new DataPackage();
            // copy
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(s);
            // if(appLink!=null)
            //     dataPackage.SetApplicationLink(new Uri() )
            Clipboard.SetContent(dataPackage);
        } );
        }

        public static async Task<string> GetClipboardText()
        {
            try
            {
                return (await Clipboard.GetContent().GetTextAsync()) ?? string.Empty;
            }
            catch (Exception ex)
            {
                Log(ex);
                return string.Empty;
            }
        }


        // HTML control messs wit this
        public static VirtualKeyModifiers keyModifiers {
            get
            {
             var rv=  VirtualKeyModifiers.None;
                if (IsKeyPressedShift())
                    rv |= VirtualKeyModifiers.Shift;
                if (IsKeyPressedControl())
                    rv |= VirtualKeyModifiers.Control;
                return rv;
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
		static ContentDialog active;
		public static async Task<ContentDialogResult> ShowAsync2(this ContentDialog dialog )
		{
			var escCounter = 0;
			for (int i=0;i<30;++i)
			{
				if (active == null)
				{
					active = dialog;
					try
					{
						var result = await dialog.ShowAsync();
						Assert(active == dialog);
						if (active == dialog)
							active = null;
						return result;
					}
					catch(Exception e)
					{
						Log(e);
						active = null;
						break;
					}
				}
				else
				{
					Log($"Rentry: {dialog.Title} {active.Title}");
					if (CoreWindow.GetForCurrentThread().GetAsyncKeyState(VirtualKey.Escape).HasFlag(CoreVirtualKeyStates.Down))
					{
						++escCounter;
						if (escCounter > 3)
							break;
					}
					else
					{
						escCounter = 0;
					}
					await Task.Delay(500);
				}
			}
			return ContentDialogResult.None;
		}
		public static void DispatchOnUIThreadLow(this CoreDispatcher d,DispatchedHandler action)
        {
            //if (d.HasThreadAccess && d.CurrentPriority == CoreDispatcherPriority.Low)
            //    action();
            //else
              d.RunAsync(CoreDispatcherPriority.Low, action);
        }
        public static void DispatchOnUIThread(this CoreDispatcher d, DispatchedHandler action)
        {
            //if (d.HasThreadAccess && d.CurrentPriority == CoreDispatcherPriority.Normal)
            //    action();
            //else
                d.RunAsync(CoreDispatcherPriority.Normal, action);
        }


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
            return mod.HasFlag(VirtualKeyModifiers.Control)|mod.HasFlag(VirtualKeyModifiers.Shift);
        }
        public static bool IsShiftAndControl(this VirtualKeyModifiers mod)
        {
            return mod.HasFlag(VirtualKeyModifiers.Control) & mod.HasFlag(VirtualKeyModifiers.Shift);
        }
        public static void CopyXamlRoomFrom(this FlyoutBase target, UIElement source)
        {
            if (source!=null & source.XamlRoot!=null)
                target.XamlRoot=source.XamlRoot;
        }
        public static void CopyXamlRoomFrom(this UIElement target, UIElement source)
        {
            if (source!=null & source.XamlRoot!=null)
                target.XamlRoot=source.XamlRoot;
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

		public static MenuFlyoutSubItem AddSubMenu(this MenuFlyout menu, string text)
		{
			var rv = new MenuFlyoutSubItem() { Text = text };

			menu.Items.Add(rv);
			return rv;
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
	}

	public static class Note
	{
		static Note()
		{
			ShellPage.inAppNote.Closed +=InAppNote_Closed;
		}

		private static void InAppNote_Closed(object sender, InAppNotificationClosedEventArgs e)
		{
			if (e.DismissKind==InAppNotificationDismissKind.User)
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
		static DateTime nextInAppNote = new DateTime(0);
		static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		public static async void Show(string s, bool lowPriority = false, int timeout = 5000)
		{
			const int noteDelay = 2;
			if (ShellPage.instance != null)
			{

				var now = DateTime.UtcNow;
				var next = nextInAppNote;
				if (now >= next)
				{
					// all clear
					nextInAppNote = now + TimeSpan.FromSeconds(noteDelay);
				}
				else
				{
					if (lowPriority)
						return;
					var wait = (next - now);
					if (wait.TotalSeconds >= 20.0f)
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
					ChatTab.L(s);
					var textBlock = new MarkdownTextBlock() { Text = s, Background = null };
					textBlock.LinkClicked += MarkDownLinkClicked;
					ShellPage.inAppNote.Show(textBlock, timeout);
				});


			}
		}

		static Regex regexCoordsTag = new Regex(@"\<coords\>(\d{1,3}:\d{1,3})\<\/coords\>", RegexOptions.CultureInvariant|RegexOptions.Compiled);
		static Regex regexPlayer = new Regex(@"\<player\>(\w+)\<\/player\>", RegexOptions.CultureInvariant|RegexOptions.Compiled);
		static Regex regexAlliance = new Regex(@"\<alliance\>(\w+)\<\/alliance\>", RegexOptions.CultureInvariant|RegexOptions.Compiled);
		static Regex regexReport = new Regex(@"\<report\>(\w+)\<\/report\>", RegexOptions.CultureInvariant|RegexOptions.Compiled);
		public static string TranslateCOTGChatToMarkdown(string s)
		{

			s = regexCoordsTag.Replace(s, @"[$1](/c/$1)");
			s = regexPlayer.Replace(s, @"[$1](/p/$1)");
			s = regexAlliance.Replace(s, @"[$1](/a/$1)");
			s = regexReport.Replace(s, @"[Report:$1](/r/$1)");
			return s;
		}
		public static void MarkDownLinkClicked(object sender, LinkClickedEventArgs e)
		{

			try
			{
				if (e.Link.StartsWith("http", StringComparison.OrdinalIgnoreCase))
				{
					Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link));
				}
				else
				{
					var paths = e.Link.Split('/');
					Assert(paths[0].Length == 0);
					switch (paths[1])
					{
						case "c":
							Spot.ProcessCoordClick(paths[2].FromCoordinate(), false, App.keyModifiers,false);
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
				Log(ex);
			}
		}
		public static void Focus(this Telerik.UI.Xaml.Controls.Grid.RadDataGrid ob)
		{
			if (ob != null)
			{
				Verify(ob.ScrollViewer.Focus(FocusState.Programmatic));
				Verify(ob.Focus(FocusState.Programmatic));
			}
		}
		static string lastTip;
		public static void ProcessTooltipsOnPointerMoved(object sender, PointerRoutedEventArgs e)
		{


			var info = Spot.HitTest(sender, e);
			var str = info.column?.Column?.Tip ?? string.Empty;
			if (str!=lastTip)
			{
				lastTip = str;
				TabPage.mainTabs.tip.Text = str; // Todo:  use the correct tabPage
			}
		}
		public static void ProcessTooltips(this Telerik.UI.Xaml.Controls.Grid.RadDataGrid grid)
		{
			grid.PointerMoved   -=ProcessTooltipsOnPointerMoved;
			grid.PointerMoved   +=ProcessTooltipsOnPointerMoved;
		}
	}

}
