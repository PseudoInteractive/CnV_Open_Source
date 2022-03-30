


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

namespace CnV
{
	using System.Diagnostics;
	using System.Threading;
	using Microsoft.UI.Dispatching;
	using Microsoft.UI.Xaml;
	#if DISABLE_XAML_GENERATED_MAIN
	public static class Program
	{


		[global::System.Runtime.InteropServices.DllImport("Microsoft.ui.xaml.dll")]
		private static extern void XamlCheckProcessRequirements();

		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 1.0.0.0")]
		[STAThread]
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
						System.Diagnostics.Debug.WriteLine(e.ToString()); 
					}
				});
			}
			catch (Exception e2)
			{
				System.Diagnostics.Debug.WriteLine(e2.ToString());
				
			}
		}



		//public static IHostBuilder CreateHostBuilder(string[] args) =>
		//	Host.CreateDefaultBuilder(args)
		//		.ConfigureServices((hostContext,services) =>
		//		{
		//			services.AddHostedService<Worker>();
		//		});
	}
	#endif
}
