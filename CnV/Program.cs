


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
	using System.Runtime.InteropServices;
	using System.Threading;
	using Microsoft.UI.Dispatching;
	using Microsoft.UI.Xaml;
	using Microsoft.Windows.AppLifecycle;
#if DISABLE_XAML_GENERATED_MAIN
	public static class Program
	{


		[global::System.Runtime.InteropServices.DllImport("Microsoft.ui.xaml.dll")]
		private static extern void XamlCheckProcessRequirements();

		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 1.0.0.0")]
		

		[STAThread]
        static void Main(string[] args)
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();

        
            bool isRedirect = DecideRedirection();
            if (!isRedirect)
            {
                Microsoft.UI.Xaml.Application.Start((p) =>
                {
                    var context = new DispatcherQueueSynchronizationContext(
                        DispatcherQueue.GetForCurrentThread());
                    SynchronizationContext.SetSynchronizationContext(context);
                    new App();
                });
            }
        }
		

		 private static void OnActivated(object sender, AppActivationArguments args)
        {
			 System.Diagnostics.Debug.WriteLine($"Activated: {args.Kind}");
         }


		private static bool DecideRedirection()
        {
            
			
			  AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();

            ExtendedActivationKind kind = args.Kind;
			 System.Diagnostics.Debug.WriteLine($"Launch {kind}");
			bool isRedirect = false;


            // Find out what kind of activation this is.
         
                        AppInstance keyInstance = AppInstance.FindOrRegisterForKey("cnv");
                    

                        // If we successfully registered the file name, we must be the
                        // only instance running that was activated for this file.
                        if (keyInstance.IsCurrent)
                        {
                            // Report successful file name key registration.
                        

                            // Hook up the Activated event, to allow for this instance of the app
                            // getting reactivated as a result of multi-instance redirection.
                            keyInstance.Activated += OnActivated;
                        }
                        else
                        {
                            isRedirect = true;
                            RedirectActivationTo(args, keyInstance);
                        }

            return isRedirect;
        }

		 [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateEvent(
            IntPtr lpEventAttributes, bool bManualReset, 
            bool bInitialState, string lpName);

        [DllImport("kernel32.dll")]
        private static extern bool SetEvent(IntPtr hEvent);

        [DllImport("ole32.dll")]
        private static extern uint CoWaitForMultipleObjects(
            uint dwFlags, uint dwMilliseconds, ulong nHandles, 
            IntPtr[] pHandles, out uint dwIndex);

        private static IntPtr redirectEventHandle = IntPtr.Zero;

        // Do the redirection on another thread, and use a non-blocking
        // wait method to wait for the redirection to complete.
        public static void RedirectActivationTo(
            AppActivationArguments args, AppInstance keyInstance)
        {
            redirectEventHandle = CreateEvent(IntPtr.Zero, true, false, null);
            Task.Run(() =>
            {
                keyInstance.RedirectActivationToAsync(args).AsTask().Wait();
                SetEvent(redirectEventHandle);
            });
            uint CWMO_DEFAULT = 0;
            uint INFINITE = 0xFFFFFFFF;
            _ = CoWaitForMultipleObjects(
               CWMO_DEFAULT, INFINITE, 1,
               new IntPtr[] { redirectEventHandle }, out uint handleIndex);
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
