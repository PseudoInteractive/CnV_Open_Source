using System;
using static COTG.Debug;

using COTG.Core.Helpers;
using COTG.Services;

//using Microsoft.AppCenter;
//using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;

//using ZLogger;

using Cysharp.Text;
using Microsoft.Toolkit.Uwp.UI.Controls;
using COTG.Views;
using System.Numerics;
using Windows.ApplicationModel.Core;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Runtime.InteropServices;
using System.Text;

namespace COTG
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();
            UnhandledException += OnAppUnhandledException;
            
         
            EnteredBackground += App_EnteredBackground;
            Resuming += App_Resuming;

            // TODO WTS: Add your app in the app center and set your secret here. More at https://docs.microsoft.com/appcenter/sdk/getting-started/uwp
        
            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
            //UserAgent.SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4170.0 Safari/537.36 Edg/85.0.552.1");

        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            Debug.Log(e.Message);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {

                await ActivationService.ActivateAsync(args);
            }
#if DEBUG
 //           this.DebugSettings.EnableFrameRateCounter = true;
 //           this.DebugSettings.FailFastOnErrors = false;
 //           this.DebugSettings.IsBindingTracingEnabled = true;
 //           this.DebugSettings.IsTextPerformanceVisualizationEnabled = true;
#endif

        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
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
            return new ActivationService(this,null, new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            await Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();
            deferral.Complete();
        }

        private void App_Resuming(object sender, object e)
        {
            Singleton<SuspendAndResumeService>.Instance.ResumeApp();
        }

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            await ActivationService.ActivateFromShareTargetAsync(args);
        }
        public static IAsyncAction DispatchOnUIThread(DispatchedHandler action, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            return CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(priority, action);
        }
        public static IAsyncAction DispatchOnUIThreadLow(DispatchedHandler action)
        {
            return CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, action);
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
    public static class Note 
    {
        public static void L(string s)
        {
            ShellPage.L(s);
        }
        public static void Show(string s, int timeout = 8000)
        {

            App.DispatchOnUIThreadLow(() =>
            { 
            var textBlock = new MarkdownTextBlock() { Text = s, Background = null };
            textBlock.LinkClicked += MarkDownLinkClicked;
            ShellPage.inAppNote.Show(textBlock, timeout);
            });

            ShellPage.L(s);
        }
        public static void MarkDownLinkClicked(object sender, LinkClickedEventArgs e)
		{

			try
			{
				var paths = e.Link.Split('/');
				Assert(paths[0].Length == 0);
				switch (paths[1])
				{
					case "c":
                        JSClient.ShowCity(paths[2].FromCoordinate());
                        break;
    			}
			}
			catch (Exception ex)
			{
				Log(ex);
			}
		}
        public static string CidToStringMD(this int cid)
		{
            var coord = cid.CidToString();
            return $"[{coord}](/c/{coord})";

        }

		public static string CidToString(this int cid)
		{
            return $"{cid%65536}.{cid/65536}";
		}
        public static int FromCoordinate(this string s)
        {
			try
			{
                var links = s.Split('.');
                return int.Parse(links[0]) | int.Parse(links[1]) * 65536;


            }
            catch (Exception e)
			{
                Log(e);
                return JSClient.cid; // return current city
			}
        }
        // 20 bit mash
       

        public static Vector2 ToWorldC(this int c)
        {
            var x = c % 65536;
            var y = c >> 16;

            return new Vector2(x +0.5f, y + 0.5f);
        }
    }


}
