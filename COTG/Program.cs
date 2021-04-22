using COTG.Game;

using System;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;

namespace COTG
{
    public static class Program
    {
        // This project includes DISABLE_XAML_GENERATED_MAIN in the build properties,
        // which prevents the build system from generating the default Main method:
        // static void Main(string[] args)
        // {
        //     global::Windows.UI.Xaml.Application.Start((p) => new App());
        // }
        // TODO WTS: Update the logic in this method if you want to control the launching of multiple instances.
        // You may find the `AppInstance.GetActivatedEventArgs()` useful for your app-defined logic.
        public static void Main(string[] _args)
        {
            var args = AppInstance.GetActivatedEventArgs();
            var key = "cotgadefault";
			TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
			try
			{
				if (args.Kind == ActivationKind.Protocol)
				{
					ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
					var s = System.Web.HttpUtility.ParseQueryString(eventArgs.Uri.Query);

					Debug.Log(s);
					// format $"cotg:launch?w={world}&s=1&n=1"
					// are / chars inserted?
					//  if (s.Length >= 3)
					{
						if (int.TryParse(s["s"], out int _s))
							JSClient.subId = _s;
						var n = s["p"];
						if(n!=null)
							Player.subOwner = n;
						if (int.TryParse(s["w"], out int _w))
							JSClient.world = _w;
						if (int.TryParse(s["n"], out int _n)) // new instance
							key = Guid.NewGuid().ToString();

					}
				}

				// If the platform indicates a recommended instance, use that.
				if (AppInstance.RecommendedInstance != null)
				{
					AppInstance.RecommendedInstance.RedirectActivationTo();
				}
				else
				{
					// Update the logic below as appropriate for your app.
					// Multiple instances of an app are registered using keys.
					// Creating a unique key (as below) allows a new instance to always be created.
					// Always using the same key will mean there's only one ever one instance.
					// Or you can use your own logic to launch a new instance or switch to an existing one.
					//string key;
					//if (xargs is ProtocolActivatedEventArgs p)
					//{
					//    key = p.Uri.ToString();
					//}
					//else
					//{
					//    key = "cotga20";
					//}

					// todo handle url activation
					//if (xargs.Kind == ActivationKind.ToastNotification)
					//{
					//    var toast = xargs as ToastNotificationActivatedEventArgs;
					//    instance= AppInstance.GetInstances().FirstOrDefault();
					//}
					var instance = AppInstance.FindOrRegisterInstanceForKey(key);

					if (instance.IsCurrentInstance)
					{
						// If successfully registered this instance, do normal XAML initialization.
						global::Windows.UI.Xaml.Application.Start((xargs) => new App());
					}
					else
					{
						// Some other instance has registered for this key, redirect activation to that instance.
						instance.RedirectActivationTo();
					}
				}
			}
			catch (AggregateException ae)
			{
				Debug.LogEx(ae);
				foreach (var e in ae.InnerExceptions)
				{
					// Handle the custom exception.
					Debug.LogEx(e);
				}
			}
			catch(Exception ex)
			{
				Debug.LogEx(ex);
			}
		}

		private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			Debug.LogEx(e.Exception);
		}
	}
}
