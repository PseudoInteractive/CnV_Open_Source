#if CRASHES
#endif


//using Microsoft.UI.Xaml.Controls;
//using ZLogger;
//using Cysharp.Text;
//using Microsoft.AppCenter;
//using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Windows.UI.Core;
//using Windows.UI.Input;
//using Windows.UI.Core;

namespace COTG
{
	using System.Diagnostics;
	using System.Threading;
	using Microsoft.Extensions.Hosting;
	using Microsoft.UI.Dispatching;
	using Microsoft.UI.Xaml;

	public static class Program
	{
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 0.0.0.0")]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.STAThreadAttribute]
		static void Main(string[] args)
		{
			global::WinRT.ComWrappersSupport.InitializeComWrappers();

			try
			{
				Application.Start((p) => {
					try
					{
						var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
						SynchronizationContext.SetSynchronizationContext(context);
						new App();
					}
					catch (Exception e)
					{
						Log(e);
					}
				});
			}
			catch (Exception e2)
			{
				Console.WriteLine(e2.Message);
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
			}
		}

	
		//public static IHostBuilder CreateHostBuilder(string[] args) =>
		//	Host.CreateDefaultBuilder(args)
		//		.ConfigureServices((hostContext,services) =>
		//		{
		//			services.AddHostedService<Worker>();
		//		});
	}
}
