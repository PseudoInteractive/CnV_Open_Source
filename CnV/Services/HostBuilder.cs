
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

namespace CnV;

    partial class AppS
    {
			internal static bool isTest;

	public static async Task StartHost( Func<Task> OnReady )
        {
            var host = new HostBuilder()
                .ConfigureLogging((hostContext, config) =>
                {
               //     config.AddConsole();
                 //   config.AddDebug();
                })
                .ConfigureHostConfiguration(config =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                    config.AddCommandLine(Environment.GetCommandLineArgs()); // todo: parse these
					var bb = config.Build();
					if(bb["test"] == "1")
						AppS.isTest = true;
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddHostedService<TimedHostedService>();

                    // Application Insights
                    // Add custom TelemetryInitializer
                 //   services.AddSingleton<ITelemetryInitializer, MyCustomTelemetryInitializer>();

                    // Add custom TelemetryProcessor
                 //   services.AddApplicationInsightsTelemetryProcessor<MyCustomTelemetryProcessor>();

                    // Example on Configuring TelemetryModules.
                    // [SuppressMessage("Microsoft.Security", "CS002:SecretInNextLine", Justification="Not a real api key, this is example code.")]
               //     services.ConfigureTelemetryModule<QuickPulseTelemetryModule>((mod, opt) => mod.AuthenticationApiKey = "e18c4e59-5c78-46cc-8dad-cfd1c8ecb062");

                    // instrumentation key is read automatically from appsettings.json
                    services.AddApplicationInsightsTelemetryWorkerService("e18c4e59-5c78-46cc-8dad-cfd1c8ecb062");
                })
                
                .Build();

            using (host)
            {
				
                // Start the host
                await host.StartAsync();
			     
				await OnReady();
                // Wait for the host to shutdown
                await host.WaitForShutdownAsync();
            }
        }

        internal class MyCustomTelemetryInitializer : ITelemetryInitializer
        {
            public void Initialize(ITelemetry telemetry)
            {
                // Replace with actual properties.
                (telemetry as ISupportProperties).Properties["MyCustomKey"] = "MyCustomValue";
            }
        }

        internal class MyCustomTelemetryProcessor : ITelemetryProcessor
        {
            ITelemetryProcessor next;

            public MyCustomTelemetryProcessor(ITelemetryProcessor next)
            {
                this.next = next;
            }

            public void Process(ITelemetry item)
            {
                // Example processor - not filtering out anything.
                // This should be replaced with actual logic.
                this.next.Process(item);
            }
        }
    }


public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private TelemetryClient tc;
     //   private static HttpClient httpClient = new HttpClient();

        public TimedHostedService(ILogger<TimedHostedService> logger, TelemetryClient tc)
        {
		Assert(tc.IsEnabled() == true);
			
            _logger = logger;
            this.tc = tc;
			AAnalytics.tc = tc;
			AAnalytics.log = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

         //   _timer = new Timer(DoWork, null, TimeSpan.Zero,
          //      TimeSpan.FromSeconds(15));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (tc.StartOperation<RequestTelemetry>("workeroperation"))
            {
                _logger.LogInformation("Timed Background Service is working.");
                //var res = httpClient.GetAsync("https://bing.com").Result.StatusCode;
                //_logger.LogInformation("bing http call completed with status:" + res);
            }            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }