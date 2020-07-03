﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using static COTG.Debug;

namespace COTG.Views
{
    public sealed partial class WebViewPage : Page, INotifyPropertyChanged
    {
        // TODO WTS: Set the URI of the page to show by default
        public static Uri DefaultUrl;// = "https://docs.microsoft.com/windows/apps/";
        public static WebViewPage instance;
        public WebView WebView => webView;
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
        async private void OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            IsLoading = false;
            OnPropertyChanged(nameof(IsBackEnabled));
            OnPropertyChanged(nameof(IsForwardEnabled));

//            stringTable = await webView.InvokeScriptAsync("eval", new [] {
// 	"let stringTable = []; "+
//    "for (let i = 0; i < 1000; ++i) { "+
//    "	let x = ''; "+
//    "	try { "+
//    "		x = o0FF.y5u(i) || x; "+
//    "	} "+
//"		catch (e) { "+
//"		} "+
//"		stringTable.push(x); "+
//"	} "+
//"	JSON.stringify(stringTable); "

//            });
//            Log(stringTable);
        }

        private void OnNavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            // Use `e.WebErrorStatus` to vary the displayed message based on the error reason
            IsShowingFailedMessage = true;
        }

        private void OnRetry(object sender, RoutedEventArgs e)
        {
            IsShowingFailedMessage = false;
            IsLoading = true;

            webView.Refresh();
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
            webView.Refresh();
        }

        private async void OnOpenInBrowser(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(webView.Source);
        }

        public WebViewPage()
        {
            Source = (DefaultUrl);
            InitializeComponent();
            IsLoading = true;
            Assert(instance == null);
            instance = this;
			webView.ContentLoading += WebView_ContentLoading;
			webView.ScriptNotify += WebView_ScriptNotify;
			webView.DOMContentLoaded += WebView_DOMContentLoaded;

			webView.UnsafeContentWarningDisplaying += WebView_UnsafeContentWarningDisplaying;
			webView.UnsupportedUriSchemeIdentified += WebView_UnsupportedUriSchemeIdentified;
			webView.UnviewableContentIdentified += WebView_UnviewableContentIdentified;
			webView.NewWindowRequested += WebView_NewWindowRequested;
        }

		private void WebView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
		{
			throw new NotImplementedException();
		}

		private void WebView_UnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
		{
			throw new NotImplementedException();
		}

		private void WebView_UnsupportedUriSchemeIdentified(WebView sender, WebViewUnsupportedUriSchemeIdentifiedEventArgs args)
		{
			throw new NotImplementedException();
		}

		private void WebView_UnsafeContentWarningDisplaying(WebView sender, object args)
		{
			throw new NotImplementedException();
		}

		private void WebView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
		{
//			throw new NotImplementedException();
		}

		private void WebView_ScriptNotify(object sender, NotifyEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void WebView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
		{
            Log("loading");
		}
       

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