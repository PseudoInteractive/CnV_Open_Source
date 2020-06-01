using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Web.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using static COTG.Debug;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using Windows.UI.Xaml;
using System.Windows.Input;

namespace COTG
{
	/// <summary>
	/// The j s client.
	/// </summary>
	public class JSClient : ICommand
    {
        public static JSClient instance = new JSClient();
        public static WebView view;
        static KeyboardAccelerator refreshAccelerator;
        static Regex worldRegex = new Regex(@"^https://w\d\d.crownofthegods.com");
        static HttpClient httpClient = new HttpClient();
        public const int world = 19;
        public static HttpRequestHeaderCollection defaultHeaders;

		/// <summary>
		/// Initializes a new instance of the <see cref="JSClient"/> class.
		/// </summary>
		public JSClient()
		{
		}

		event EventHandler ICommand.CanExecuteChanged
		{
			add
			{
				throw new NotImplementedException();
			}

			remove
			{
				throw new NotImplementedException();
			}
		}

		static void AddDefaultHeaders(HttpRequestHeaderCollection headers)
        {
        }

        internal async static Task Initialize(RelativePanel panel)
        {
            var headers = httpClient.DefaultRequestHeaders;
            headers.TryAppendWithoutValidation("Content-Type","application/x-www-form-urlencoded; charset=UTF-8");
            headers.TryAppendWithoutValidation("pp-ss", "0");
            headers.TryAppendWithoutValidation("Referer", $"https://w{world}.crownofthegods.com/overview/overview.php?s=0");
            defaultHeaders = headers;
              view = new WebView(WebViewExecutionMode.SeparateThread);
            view.UnsafeContentWarningDisplaying += View_UnsafeContentWarningDisplaying;
            view.UnsupportedUriSchemeIdentified += View_UnsupportedUriSchemeIdentified;

            view.UnviewableContentIdentified += View_UnviewableContentIdentified;
            view.ScriptNotify += View_ScriptNotify;
            view.DOMContentLoaded += View_DOMContentLoaded;
            view.NavigationFailed += View_NavigationFailed;
            view.NavigationStarting += View_NavigationStarting;
            view.NavigationCompleted += View_NavigationCompletedAsync;
            view.PermissionRequested += View_PermissionRequested;

            //   view.CacheMode = CacheMode.
            RelativePanel.SetAlignLeftWithPanel(view, true);
            RelativePanel.SetAlignRightWithPanel(view, true);
            RelativePanel.SetAlignTopWithPanel(view, true);
            RelativePanel.SetAlignBottomWithPanel(view, true);
            Canvas.SetZIndex(view, 0);
            panel.Children.Add(view);
            view.Source = new Uri("https://www.crownofthegods.com");

            refreshAccelerator = new KeyboardAccelerator() { Key = Windows.System.VirtualKey.F5 };
            refreshAccelerator.Invoked += (_, __) => view?.Refresh();


        }

        private static async Task AddJSPluginAsync()
        {
            var asm = typeof(JSClient).Assembly;
            using (Stream stream = asm.GetManifestResourceStream("COTG.Javascript.mafunky.js"))
            using (StreamReader reader = new StreamReader(stream))
            {
                await view.InvokeScriptAsync("eval", new string[] { reader.ReadToEnd() });
                await view.InvokeScriptAsync("funkyinit", null);
            }
        }

        static private void View_PermissionRequested(WebView sender, WebViewPermissionRequestedEventArgs args)
        {
            var pr = args.PermissionRequest;
            Log($"Permission {pr.Id} {pr.PermissionType} {pr.State} {pr.ToString()}");
            pr.Allow();
        }

        static private void View_NavigationCompletedAsync(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            Log($"Nav complete {args.Uri}");
            
        }

        static private void View_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            Log($"Nav start {args.Uri}");
        }

        static private async void View_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            
            await Exception($"Internet failed, press any key to retry {e.Uri} {e.WebErrorStatus}");
            if (view!=null)
                view.Refresh();
        }

        static async private void View_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            Log($"Dom loaded {args.Uri}");
            if (worldRegex.IsMatch(args.Uri.ToString()))
            {
                Log("Match Regex!");
                await AddJSPluginAsync();
            }
        }

        static private void View_ScriptNotify(object sender, NotifyEventArgs e)
        {
            Log($"Notify: {e.CallingUri} {e.Value} {sender}");
        }

        static private void View_UnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
        {
            Exception("Unviewable");
        }

        static private void View_UnsupportedUriSchemeIdentified(WebView sender, WebViewUnsupportedUriSchemeIdentifiedEventArgs args)
        {
            Exception("UnsupportedUriScheme");
        }

        static private void View_UnsafeContentWarningDisplaying(WebView sender, object args)
        {
            Exception("Unsafe");
        }

        public static async void TestGet()
        {
            Log("TestGet");
            var req = new HttpRequestMessage(HttpMethod.Post, new Uri( new Uri($"https://w{world}.crownofthegods.com"), "overview/citover.php"));
            AddDefaultHeaders(req.Headers);
            var result = await httpClient.TrySendRequestAsync(req);
            var resp = result.ResponseMessage;
            if(resp.IsSuccessStatusCode )
            {
                var str = await resp.Content.ReadAsStringAsync();

                Log(str);
            }
            else {
                Log($"Error: {result.ExtendedError}");

                    };

        }

		bool ICommand.CanExecute(object parameter)
		{
			return true;
		}

		void ICommand.Execute(object parameter)
		{
			throw new NotImplementedException();
		}
	}
}
