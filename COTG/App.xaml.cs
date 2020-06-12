using System;
using System.Diagnostics;

using COTG.Core.Helpers;
using COTG.Services;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;

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
            this.DebugSettings.EnableFrameRateCounter = true;
            this.DebugSettings.FailFastOnErrors = false;
            this.DebugSettings.IsBindingTracingEnabled = true;
 //           this.DebugSettings.IsTextPerformanceVisualizationEnabled = true;
#endif

        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
            AppCenter.Start("0b4c4039-3680-41bf-b7d7-685eb68e21d2",
               typeof(Analytics), typeof(Crashes));

            var configuration = new ConfigurationBuilder()
                                            .AddJsonFile("appsettings.json", false, true)
                                            .Build();


            ILogger logger;

            using (var serviceProvider = new ServiceCollection()
                .AddLogging(cfg =>
                {
                    cfg.AddConfiguration(configuration.GetSection("Logging"));
                    cfg.AddConsole();
                })
                .BuildServiceProvider())
            {
                logger = serviceProvider.GetService<ILogger<App>>();
            }

            logger.LogInformation("logger information");
            logger.LogWarning("logger warning");

          
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
    }
}
