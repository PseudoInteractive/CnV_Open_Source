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
            //UserAgent.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4170.0 Safari/537.36 Edg/85.0.552.1");

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
        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Shift:
                    shiftPressed = false;
                    break;
                case VirtualKey.Control:
                    controlPressed = false;
                    break;

            }
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Shift:
                    shiftPressed = true;
                    break;
                case VirtualKey.Control:
                    controlPressed = true;
                    break;
            }
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


        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            CoreApplication.EnablePrelaunch(false);
            if (!args.PrelaunchActivated)
            {

                await ActivationService.ActivateAsync(args);
            }
            OnLaunchedOrActivated();
        }
        private void OnLaunchedOrActivated()
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
            idleTimer = new DispatcherTimer();
            idleTimer.Interval = TimeSpan.FromSeconds(10);  // 10s idle delay
            idleTimer.Tick += IdleTimer_Tick;
            Assert(idleTimer.IsEnabled == false);
            Window.Current.CoreWindow.PointerMoved += CoreWindow_PointerMoved;
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed; ;

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs e)
        {
            var prop = e.CurrentPoint.Properties;
            switch (prop.PointerUpdateKind)
            {
                case PointerUpdateKind.XButton1Pressed:
                    if(!NavStack.Back() )
                        ShellPage.instance.ChangeCityClick(-1);
                    Log("XButton1");
                    e.Handled = true;
                    break;
                case PointerUpdateKind.XButton2Pressed:
                    if (!NavStack.Forward())
                        ShellPage.instance.ChangeCityClick(1);
                    Log("XButton2");
                    e.Handled = true;
                    break;
            }
        }

        private static void CoreWindow_PointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            // reset timer if active
            if (timerActive)
            {
                idleTimer.Stop();
                idleTimer.Start();
            }
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
                a();
            }
        }
       

         public static void QueueIdleTask(Action a, int intialDelayInmilisecons =0)
        {
            foreach(var i in idleTasks)
            {
                if (i == a)
                    return;

            }

            if (intialDelayInmilisecons > 0)
            {
                Task.Delay(intialDelayInmilisecons).ContinueWith((_) => QueueIdleTask(a, 0));
            }
            else
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
                // Todo:  Handle arguments and stuff
                // Ensure the current window is active
                if (args is ToastNotificationActivatedEventArgs toastActivationArgs)
                {
                    // Obtain the arguments from the notification
                    var toastArgs = System.Web.HttpUtility.ParseQueryString(toastActivationArgs.Argument);
                    // Obtain any user input (text boxes, menu selections) from the notification
                    ValueSet userInput = toastActivationArgs.UserInput;

                    // TODO: Show the corresponding content
                }

                Window.Current.Activate();
                return;
            }
            await ActivationService.ActivateAsync(args);
            OnLaunchedOrActivated();
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

        public static MenuFlyoutItem CreateMenuItem(string text, Action command)
        {
            var rv = new MenuFlyoutItem() { Text = text };
            if (command != null)
                rv.Click += (_, _) => command();
            return rv;
        }
        public static MenuFlyoutItem AddItem(MenuFlyout menu, string text, RoutedEventHandler command, object context = null)
        {
            var rv = new MenuFlyoutItem() { Text = text };
            rv.DataContext = context;
            if (command != null)
                rv.Click += command;
            menu.Items.Add(rv);
            return rv;
        }
        public static MenuFlyoutItem AddItem(MenuFlyout menu, string text, Action command)
        {
            var rv = new MenuFlyoutItem() { Text = text };
            if (command != null)
                rv.Click += (_, _) => command();
            menu.Items.Add(rv);
            return rv;
        }
        public static MenuFlyoutItem CreateMenuItem(string text, ICommand command, object parameter)
        {
            return new MenuFlyoutItem() { Text = text, Command = command, CommandParameter = parameter };
        }
        ///        public static DumbCollection<City> emptyCityList = new DumbCollection<City>();
        public static PercentFormatter percentFormatter = new PercentFormatter() { FractionDigits = 1, NumberRounder=new IncrementNumberRounder() { Increment=.001,RoundingAlgorithm=RoundingAlgorithm.RoundHalfToEven} };
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



    //public static class UserAgent
    //{
    //    const int URLMON_OPTION_USERAGENT = 0x10000001;

    //    [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
    //    private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);

    //    [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
    //    private static extern int UrlMkGetSessionOption(int dwOption, StringBuilder pBuffer, int dwBufferLength, ref int pdwBufferLength, int dwReserved);

    //    public static string GetUserAgent()
    //    {
    //        int capacity = 255;
    //        var buf = new StringBuilder(capacity);
    //        int length = 0;

    //        UrlMkGetSessionOption(URLMON_OPTION_USERAGENT, buf, capacity, ref length, 0);

    //        return buf.ToString();
    //    }

    //    public static void SetUserAgent(string agent)
    //    {
    //        var hr = UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, agent, agent.Length, 0);
    //        var ex = Marshal.GetExceptionForHR(hr);
    //        if (null != ex)
    //        {
    //            throw ex;
    //        }
    //    }

    //    public static void AppendUserAgent(string suffix)
    //    {
    //        SetUserAgent(GetUserAgent() + suffix);
    //    }
    //}
    public static class AApp
    {
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


    }

    public static class Note 
    {
       // [Conditional("TRACE")]
        public static void L(string s)
        {
            ChatTab.L(s);
        }
        public static void Show(string s, int timeout = 8000)
        {
            if (ShellPage.instance != null)
            {

                App.DispatchOnUIThreadLow(() =>
                {
                    var textBlock = new MarkdownTextBlock() { Text = s, Background = null };
                    textBlock.LinkClicked += MarkDownLinkClicked;
                    ShellPage.inAppNote.Show(textBlock, timeout);
                });

                ChatTab.L(s);
            }
        }

        static Regex regexCoords = new Regex(@"\<coords\>(\d+:\d+)\<\/coords\>");
        static Regex regexPlayer = new Regex(@"\<player\>(\w+)\<\/player\>");
        static Regex regexAlliance = new Regex(@"\<alliance\>(\w+)\<\/alliance\>");
        static Regex regexReport = new Regex(@"\<report\>(\w+)\<\/report\>");
        public static string TranslateCOTGChatToMarkdown(string s)
        {
            s = regexCoords.Replace(s, @"[$1](/c/$1)");
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
                            Spot.ProcessCoordClick(paths[2].FromCoordinate(), false,VirtualKeyModifiers.None);
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
     
    }


}
