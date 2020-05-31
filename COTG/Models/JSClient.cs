using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using static COTG.Debug;

namespace COTG
{
	public static class JSClient
	{
        public static WebView view;
        static KeyboardAccelerator refreshAccelerator;

        internal async static Task Initialize(RelativePanel panel)
        {
            view = new WebView(WebViewExecutionMode.SeparateThread);
            view.UnsafeContentWarningDisplaying += View_UnsafeContentWarningDisplaying;
            view.UnsupportedUriSchemeIdentified += View_UnsupportedUriSchemeIdentified;

            view.UnviewableContentIdentified += View_UnviewableContentIdentified;
            view.ScriptNotify += View_ScriptNotify;
            view.DOMContentLoaded += View_DOMContentLoaded;
            view.NavigationFailed += View_NavigationFailed;
            view.NavigationStarting += View_NavigationStarting;
            view.NavigationCompleted += View_NavigationCompleted;
            view.PermissionRequested += View_PermissionRequested;

            //   view.CacheMode = CacheMode.
            panel.Children.Insert(0, view);
            RelativePanel.SetAlignLeftWithPanel(view, true);
            RelativePanel.SetAlignRightWithPanel(view, true);
            RelativePanel.SetAlignTopWithPanel(view, true);
            RelativePanel.SetAlignBottomWithPanel(view, true);
            Canvas.SetZIndex(view, 0);
            await Task.Delay(2000);
            view.Source = new Uri("https://www.crownofthegods.com");

            refreshAccelerator = new KeyboardAccelerator() { Key = Windows.System.VirtualKey.F5 };
            refreshAccelerator.Invoked += (_, __) => view?.Refresh();


        }

        private static async Task AddJSPluginAsync()
        {
            var asm = typeof(JSClient).Assembly;
            using (Stream stream = asm.GetManifestResourceStream("COTG.Javascript.mafunky.xjs"))
            using (StreamReader reader = new StreamReader(stream))
            {
                await view.InvokeScriptAsync("eval", new string[] { reader.ReadToEnd() });
            }
        }

        static private void View_PermissionRequested(WebView sender, WebViewPermissionRequestedEventArgs args)
        {
            var pr = args.PermissionRequest;
            Log($"Permission {pr.Id} {pr.PermissionType} {pr.State} {pr.ToString()}");
            pr.Allow();
        }

        static private void View_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            Log($"Nav complete {args}");
        }

        static private void View_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            Log($"Nav start {args}");
        }

        static private async void View_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            
            await Exception("Internet failed, press any key to retry");
            if (view!=null)
                view.Refresh();
        }

        static private void View_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            Log($"Dom loaded {args.Uri}");
            if (!args.Uri.ToString().Contains( "www"))
            {
                _ = AddJSPluginAsync();
            }
        }

        static private void View_ScriptNotify(object sender, NotifyEventArgs e)
        {
            Log("Dom loaded");
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

    }
}
