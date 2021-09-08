using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using WebView = Microsoft.UI.Xaml.Controls.WebView2;
using static COTG.Debug;
using Windows.System;
using Microsoft.Web.WebView2.Core;

namespace COTG.Views
{
    public sealed partial class WebViewPage :Windows.UI.Xaml.Controls.Page, INotifyPropertyChanged
    {
        // TODO WTS: Set the URI of the page to show by default
        public static Uri DefaultUrl;// = "https://docs.microsoft.com/windows/apps/";
        public static WebViewPage instance;
     //   public WebView WebView => webView;
        private Uri _source;

        public Uri Source
        {
            get { return _source; }
            set { Set(ref _source, value); }
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }

            set
            {
                if (value)
                {
                    IsShowingFailedMessage = false;
                }

                Set(ref _isLoading, value);
                IsLoadingVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _isLoadingVisibility;

        public Visibility IsLoadingVisibility
        {
            get { return _isLoadingVisibility; }
            set { Set(ref _isLoadingVisibility, value); }
        }

        private bool _isShowingFailedMessage;

        public bool IsShowingFailedMessage
        {
            get
            {
                return _isShowingFailedMessage;
            }

            set
            {
                if (value)
                {
                    IsLoading = false;
                }

                Set(ref _isShowingFailedMessage, value);
                FailedMesageVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _failedMesageVisibility;

        public Visibility FailedMesageVisibility
        {
            get { return _failedMesageVisibility; }
            set { Set(ref _failedMesageVisibility, value); }
        }

        public string stringTable;
		internal static CoreWebView2WebResourceRequest post;

	

    

        private void OnRetry(object sender, RoutedEventArgs e)
        {
            IsShowingFailedMessage = false;
            IsLoading = true;

            webView.Reload();
        }

        public bool IsBackEnabled
        {
            get { return webView.CanGoBack; }
        }

        public bool IsForwardEnabled
        {
            get { return webView.CanGoForward; }
        }

        private void OnGoBack(object sender, RoutedEventArgs e)
        {
            webView.GoBack();
        }

        private void OnGoForward(object sender, RoutedEventArgs e)
        {
            webView.GoForward();
        }

        private void OnRefresh(object sender, RoutedEventArgs e)
        {
            webView.Reload();
        }

        private async void OnOpenInBrowser(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(webView.Source);
        }

        public WebViewPage()
        {
            InitializeComponent();
            IsLoading = true;
     //       Assert(instance == null);
            instance = this;
			//webView.CoreWebView2.ContentLoading += WebView_ContentLoading;
			//webView.CoreWebView2.IsWeb += WebView_ScriptNotify;
			//		webView.DOMContentLoaded += WebView_DOMContentLoaded;
			webView.CoreWebView2.PermissionRequested+=CoreWebView2_PermissionRequested;
			webView.NavigationCompleted+=WebView_NavigationCompleted;
			//		webView.UnsafeContentWarningDisplaying += WebView_UnsafeContentWarningDisplaying;
			//	webView.UnsupportedUriSchemeIdentified += WebView_UnsupportedUriSchemeIdentified;
			//webView.UnviewableContentIdentified += WebView_UnviewableContentIdentified;
			webView.CoreWebView2.NewWindowRequested+=CoreWebView2_NewWindowRequested;
			if(post != null)
			{
				var p = post;
				post = null;
				//using (var req = onew CoreWebView2WebResourceRequest(HttpMethod.Post, new Uri(p.OriginalString)))
				{
					

					webView.CoreWebView2.NavigateWithWebResourceRequest(p);

				}
			}
			else
			{
				Assert(DefaultUrl != null);
				Source = (DefaultUrl);
				DefaultUrl = null;
			}
		}

		private void CoreWebView2_NewWindowRequested(Microsoft.Web.WebView2.Core.CoreWebView2 sender,Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs args)
		{
			args.Handled = true;

			//	Log(args.Uri.ToString());
			Launcher.LaunchUriAsync(new Uri(args.Uri));
		}

		private void CoreWebView2_PermissionRequested(Microsoft.Web.WebView2.Core.CoreWebView2 sender,Microsoft.Web.WebView2.Core.CoreWebView2PermissionRequestedEventArgs args)
		{
			Log("Permission " + args.PermissionKind);
//			args.PermissionKindAllow();
		}

		private void WebView_NavigationCompleted(WebView sender,Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
		{
			IsLoading = false;
			OnPropertyChanged(nameof(IsBackEnabled));
			OnPropertyChanged(nameof(IsForwardEnabled));

		}

        

		//private async void WebView_UnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
		//{
  //          if (await Windows.System.Launcher.LaunchUriAsync(args.Uri))
  //          {
  //              Note.Show($"Launched {args.Uri}");
  //          }
  //          else
  //          {
  //              Note.Show($"Failed to launch {args.Uri}");
  //          }
  //      }

	

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
            Assert(instance == this);
            instance = null;
			base.OnNavigatedFrom(e);

		}



	}
}
