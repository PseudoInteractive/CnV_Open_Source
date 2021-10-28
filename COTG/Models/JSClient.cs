using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static COTG.Debug;
using Microsoft.UI.Xaml;
//using Windows.System;
using System.Text.Json;
using COTG.Game;
using System.Threading;
using COTG.Helpers;
using Microsoft.UI.Input.Experimental;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Concurrent;
using Windows.Storage.Streams;
using COTG.Services;
using COTG.Views;
using System.Numerics;
using COTG.JSON;
using static COTG.Game.Troops;
//using Windows.UI.Input;
using Windows.Graphics.Imaging;
using System.Text.Json.Serialization;
using COTG.DB;
using Microsoft.AppCenter;
using VirtualKey = Windows.System.VirtualKey;
using VirtualKeyModifiers = Windows.System.VirtualKeyModifiers;
using ContentDialog = Microsoft.UI.Xaml.Controls.ContentDialog;
using ContentDialogResult = Microsoft.UI.Xaml.Controls.ContentDialogResult;
using System.Text;
using System.Web;
using Windows.Security.Cryptography.Certificates;
//using Windows.Foundation;
using System.Net.Http.Headers;
using DiscordCnV;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.WinUI.Notifications;
using static COTG.Game.City;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web;
using CoreWebView = Microsoft.Web.WebView2.Core.CoreWebView2;
//using COTG.CnVChat;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Dispatching;
//using Windows.UI.ViewManagement;
using System.Runtime;
using PointerUpdateKind = Windows.UI.Input.PointerUpdateKind;
using System.Net;

namespace COTG
{
	/// <summary>
	/// The j s client.
	/// </summary>
	public class JSClient
	{

		//static public string secSessionId; // hack!



		//	public static string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36";
		//public static string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.114 Safari/537.36";

		//public static string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.114 Safari/537.36";
		//public static string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.114 Safari/537.36 Edg/91.0.864.54";
//		public static string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36 Edg/93.0.961.52";
		public static string userAgent;// = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36 Edg/94.0.992.31";
		//        public static JsonDocument ppdt;
		public static JSClient instance = new JSClient();
		public static WebView2 view;
		public static CoreWebView coreWebView;
		//public static WebViewBrush webViewBrush; 
		public static HttpClientHandler  httpFilter;
	//	public static HttpCookieManager cookieManager;
//		const int clientCount = 6;
	//	public static ConcurrentBag<HttpClient> clientPool;
	//	public static SemaphoreSlim clientPoolSema = new SemaphoreSlim(clientCount);
		static HttpClient _downloadImageClient;
		static bool councillorsChecked;
		public static int spanX;
		public static int spanY;
		public static string cookies;
		// hack:  resources for web load
		//		static string jsFunkyEtc;

		//public static HttpClient downloadImageClient
		//{
		//	get
		//	{
		//		if (_downloadImageClient == null)
		//		{
		//			_downloadImageClient = new HttpClient();
		//			_downloadImageClient.DefaultRequestHeaders.Accept.TryParseAdd("image/png, image/svg+xml, image/*; q=0.8, */*; q=0.5");
		//			_downloadImageClient.DefaultRequestHeaders.Referer = httpsHost;
		//			_downloadImageClient.DefaultRequestHeaders.Host = new Windows.Networking.HostName(httpsHost.Host);
		//			_downloadImageClient.DefaultRequestHeaders.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
		//		}
		//		return _downloadImageClient;
		//	}
		//}

		public static void JSInvoke(string func)
		{
			App.DispatchOnUIThreadLow(async () =>
			{

				try
				{
					await JSClient.view.ExecuteScriptAsync(func);

				}
				catch (Exception ex)
				{
					LogEx(ex);

				} }
);
		}


		public static async Task<string> JSInvokeTask(string func)
		{

			// this won't await the actually js call
			return await App.DispatchOnUIThreadTask(async () =>
		  await JSClient.view.ExecuteScriptAsync(func));


		}
		public static HttpClient _genericClient;
		public static HttpClient genericClient
		{
			get
			{
				if (_genericClient == null)
				{
					_genericClient = new HttpClient();
					_genericClient.DefaultRequestVersion = HttpVersion.Version20;
				}
				return _genericClient;
			}
		}


		public struct Faith
		{
			public byte evara;   // 1
			public byte vexemis; // 2
			public byte domdis; // 3
			public byte cyndros; // 4
			public byte merius; // 5
			public byte ylanna; // 6
			public byte ibria; // 7
			public byte naera; // 8
		};
		public static Faith faith;

		const int researchCount = 64; // I think this is really 49
		public static byte[] research = new byte[researchCount];
		public static int world = 0;
		public static int subId = 0;
		static Regex urlMatch = new Regex(@"^w(\d\d).crownofthegods.com$");
		public static Uri httpsHost;
		public static string httpsHostString;

		// IHttpContent content;
		public class JSVars
		{
			[JsonInclude]
			public int pid;
			[JsonInclude]
			public string pn; // redundant player name
			[JsonInclude]
			public string token;
			//	[JsonInclude]
			//	public  string s ;
			[JsonInclude]
			public string raidSecret;
			//		[JsonInclude]
			//			public  string cookie; // not used

			[JsonInclude]
			public string ppdt;

			public int[] allowedAlliances;
			public int[] allowedPlayers;
			public int[] deniedPlayers;

			public string cookies;

		}
		public static JSVars[] jsVarsByPlayer = Array.Empty<JSVars>();
		public static JSVars jsVars;
		public static JSVars jsBase;
		public static JSVars PlayerVars(int pid)
		{
			if (pid == -1)
				return jsVars;
			for (int i = 0; i < jsVarsByPlayer.Length; ++i)
			{
				var p = jsVarsByPlayer[i];
				if (p.pid == pid)
					return p;
			}
			return jsVars;
		}
		public static string PlayerToken(int pid) => PlayerVars(pid).token;
	//	public static string PlayerToken(int pid) => PlayerVars(pid).token;

		public static int ppss;
		public static bool isSub => subId != 0;

		//public static long GameTimeMs()
		//{
		//	return (long)((DateTimeOffset.UtcNow - launchTime).TotalMilliseconds) + gameMSAtStart;
		//}
		public static DateTimeOffset ServerTime()
		{
			return (DateTimeOffset.UtcNow + gameTOffset);
		}
		public static uint ServerTimeSeconds()
		{
			return (uint)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + gameTOffsetSeconds);
		}
		public static SmallTime ServerTimeSmall()
		{
			return (uint)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + gameTOffsetSeconds);
		}
		// timestamp - ServerTime all in in MS 
		public static int ServerTimeOffsetSeonds(uint t) // t is COTG server time in MS
		{
			return (int)(t - ServerTimeSeconds());
		}
		//public static int ServerTimeOffsetSeconds(long t)
		//{
		//	return (int)(t - ServerTimeSeconds());
		//}
		public static long ServerTimeMs() => (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + gameTOffsetSeconds*1000L);

		//public static DateTimeOffset ServerToLocal(DateTimeOffset t)
		//{
		//	return t.ToUniversalTime() - gameTOffset;
		//}
		/// <summary>
		/// Initializes a new instance of the <see cref="JSClient"/> class.
		/// </summary>
		public JSClient()
		{
		}

		//public static void SetPlayer(int pid, int cid)
		//{
		//	foreach (var p in PlayerPresence.all)
		//	{
		//		if (p.pid == pid)
		//		{

		//			SetPlayer(pid, p.token, p.cookies, cid, p.name);
		//			return;
		//		}
		//	}
		//	Debug.Log("Missing player");
		//}

		//public static string GetSecSessionId()
		//{
		//	var cookies = coreWebView.CookieManager.Get.GetCookies(new Uri("https://crownofthegods.com"));
		//	foreach(var cookie in cookies)
		//	{
		//		if(cookie.Name == "sec_session_id")
		//			return cookie.Value;


		//	}
		//	return null; // error!
		//}
		//static string pendingCookies;
		//public static async void SetPlayer(int pid, string token, string cookies, int cid, string name)
		//{
		//	// already set
		//	if (jsVars.token == token)
		//		return;
		//	if (pendingCookies != null)
		//		return;
		//	pendingCookies = cookies;

		//	Note.Show($"Entering {name}'s City");


		//	Log($"ChangePlayer:{name}");
		//	{
		//		var _cookies = cookieManager.GetCookies(new Uri("https://crownofthegods.com"));
		//		foreach (var c in _cookies)
		//		{
		//			Log($"{c.Name} {c.Domain} {c.Path} {c.Value}");
		//		}
		//	}

		//	var secSessionId = CookieDB.Apply(cookies);


		//	{
		//		var _cookies = cookieManager.GetCookies(new Uri("https://crownofthegods.com"));
		//		foreach (var c in _cookies)
		//		{
		//			Log($"{c.Name} {c.Domain} {c.Path} {c.Value} {c.Secure} {c.HttpOnly}");
		//		}
		//	}
		//	//	AddPlayer(false, true, pid, Player.all[pid].name, token, "", secSessionId, null);
		//	//	await GetCity.Post(cid, (jse,city) => Log($"{jse.ToString()} Here!!") );
		//	App.DispatchOnUIThreadLow(() => view.ExecuteScriptAsync($"setPlayerGlobals({token},{secSessionId},{cid})"));
		//}

		//public static void SetCookieCollab(string name, string value, bool session, bool httpOnly, bool clearOnly = false)
		//{
		//	var cookie = new HttpCookie(name, ".crownofthegods.com", "/");
		//	//		var remember = new HttpCookie("remember_me", ".crownofthegods.com", "/");
		//	if (httpOnly)
		//	{
		//		cookie.Secure = true;
		//		cookie.HttpOnly = true;
		//	}

		//	if (!session)
		//	{
		//		cookie.Expires = DateTimeOffset.UtcNow + TimeSpan.FromDays(7);
		//	}
		//	cookieManager.DeleteCookie(cookie);
		//	if (!clearOnly)
		//	{
		//		cookie.Value = value;
		//		cookieManager.SetCookie(cookie);
		//	}
		//}

		public static void AddPlayer(bool isMe, bool setCurrent, int pid , string pn, string token, string raid = null, string cookies=null, string ppdt=null)
		{
			var jsv = new JSVars() { token = token, pn = pn, pid = pid, ppdt = ppdt, cookies = cookies, raidSecret = raid }; // todo: need raidSecret
																															 //
																															 //			Player.myIds																								 // add if necessary

			bool present = false;
			for (int i = 0; i < jsVarsByPlayer.Length; ++i)
			{
				if (jsVarsByPlayer[i].pid == pid)
				{
					jsVarsByPlayer[i] = jsv;
					present = true;
				}
			}
			if (!present)
			{
				jsVarsByPlayer = jsVarsByPlayer.ArrayAppend(jsv);
				Player.myIds.Add(pid);
			}

			if (isMe)
			{
				jsVars = jsv;
				Player.activeId = pid;
			}
			else if (setCurrent)
			{
				//	Player.myName = pn;
				Player.activeId = pid;
				jsVars = jsv;

//				App.DispatchOnUIThreadSneakyLow(() => ShellPage.instance.friendListBox.SelectedItem = pn);

			}

		}
		//public static void SetSessionCookie()
		//{
		//	//if(!SettingsPage.secSessionId.IsNullOrEmpty() && !JSClient.isSub  )
		//	//{
		//	//	SetCookie("sec_session_id", SettingsPage.secSessionId);
		//	//}
		//}

		//public static void SetCookie(string name,string value,string domain = "crownofthegods.com",string path = "/")
		//{
		//	var cookie = new HttpCookie(name,domain,path);
		//	//		var remember = new HttpCookie("remember_me", ".crownofthegods.com", "/");
		//	//if (httpOnly)
		//	{
		//		cookie.Secure = true;
		//		cookie.HttpOnly = true;
		//	}

		//	//if (!session)
		//	{
		//		cookie.Expires = DateTimeOffset.UtcNow + TimeSpan.FromDays(64);
		//	}
		//	var cookieManager = httpFilter.CookieManager;
		//	cookieManager.DeleteCookie(cookie);
		//	//if (!clearOnly)
		//	if(!value.IsNullOrEmpty())
		//	{
		//		cookie.Value = value;
		//		cookieManager.SetCookie(cookie);
		//	}


		//}
		//	static string secSessionId;
		static string jsFunkyEtc;

		static async Task LoadJsStrings()
		{
			var t0 = App.GetAppString("JS/funky");
			var t1 = App.GetAppString("JS/DHRUVCC.js");
			var t2 = App.GetAppString("JS/J0EE");

			var jsf = await t0+ await t1+ await t2;

			//var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
			//// Create the data writer object backed by the in-memory stream.
			//using(var dataWriter = new Windows.Storage.Streams.DataWriter(stream))
			//{
				jsFunkyEtc = "const cityAtlas = " +
				  (SettingsPage.IsThemeWinter() ?
						  "'ms-appx-web:///Content/Art/City/Winter/building_set5.png'\n" :
					   "'/images/city128/building_set5.png'\n") +
					   jsf;

		//		dataWriter.WriteString(js);
		//		await dataWriter.StoreAsync();
		//		await dataWriter.FlushAsync();
		//		dataWriter.DetachStream();
		//	}

		//	stream.Seek(0);
			
		//	var Response = coreWebView.Environment.CreateWebResourceResponse(stream,200,"Ok","");
		//	var headers = Response.Headers;
		//	headers.AppendHeader("Content-Type","text/javascript");
		////	headers.AppendHeader("Content-Encoding","text/json");
		////	headers.("Content-Encoding","text/json");
		//	jsFunkyEtc = js;

		}

		private const string jsFunctionMask = "*jsfunctions/*";
		public static async Task SuspendWebView()
		{
			if(view is not null && (view.CoreWebView2 is not null))
			{

				if(!view.CoreWebView2.IsSuspended)
				{
					view.Visibility = Visibility.Collapsed;
					await view.CoreWebView2.TrySuspendAsync();

				}
				//	grid.Children.Remove(_w);
				//			_w.Close();
				//webView = null;
			}
		}
		public static void ResumeWebView()
		{
			if(view is not null && (view.CoreWebView2 is not null))
			{
				view.Visibility = Visibility.Visible;
				if(view.CoreWebView2.IsSuspended)
				{
					view.CoreWebView2.Resume();

				}
				//	grid.Children.Remove(_w);
				//	_w.Close();
				//view = null;
			}
		}

		public static void CloseWebView()
		{
			if(view is not null )
			{

				var v =view;
				coreWebView = null;
				view = null;
				v.Visibility = Visibility.Collapsed;
				//ShellPage.instance.grid.Children.Remove(ShellPage.instance.webView);
				//ShellPage.instance.webView = null;
				v.Close();
			}
		}
		private static async void WebViewException(Exception ex)
		{
			Log(ex);
			AAnalytics.Track("WebViewEx",new Dictionary<string,string>
					   {{"Ex",ex.Message } });

			if( await App.DoYesNoBox("Some is wrong with Webview","Download the webview?", cancel:null) == 1)
			{
				await Windows.System.Launcher.LaunchUriAsync(new Uri("https://go.microsoft.com/fwlink/p/?LinkId=2124703",UriKind.Absolute));
			}
			else
			{
				await App.DoYesNoBox("Some is wrong with Webview","Maybe Restart",yes:"Close",no:null,cancel:null);
			}

		}
		private static async void CoreWebView2Initialized(WebView2 sender,CoreWebView2InitializedEventArgs _args)
		{

			try
			{
				if(_args.Exception is not null)
				{
					WebViewException(_args.Exception);
					return;
				}

				coreWebView = view.CoreWebView2;
				//			view.CharacterReceived +=View_CharacterReceived;
				coreWebView.Settings.AreDevToolsEnabled=System.Diagnostics.Debugger.IsAttached;
				//	coreWebView.Settings.UserAgent = userAgent;

				coreWebView.Settings.IsWebMessageEnabled=true;
				//	coreWebView.Settings.IsPasswordAutosaveEnabled=true;
				coreWebView.Settings.IsScriptEnabled=true;
				//			coreWebView.Settings.IsPinchZoomEnabled =false;
				coreWebView.Settings.IsZoomControlEnabled=false;
				coreWebView.Settings.AreDefaultScriptDialogsEnabled=true;
				coreWebView.Settings.IsStatusBarEnabled=false;
				coreWebView.Settings.IsBuiltInErrorPageEnabled=true;
				coreWebView.Settings.AreHostObjectsAllowed=false;
				//	coreWebView.Settings.AreBrowserAcceleratorKeysEnabled=false;
				coreWebView.Settings.AreDefaultContextMenusEnabled=true;
				coreWebView.Environment.NewBrowserVersionAvailable+=Environment_NewBrowserVersionAvailable;
				//				coreWebView.Settings.AreBrowserAcceleratorKeysEnabled=false;
				//coreWebView.AddWebResourceRequestedFilter("*jsfunctions/game.js",ResourceContext:CoreWebView2WebResourceContext.Script);
				coreWebView.AddWebResourceRequestedFilter(jsFunctionMask,ResourceContext: CoreWebView2WebResourceContext.Script);
				coreWebView.WebResourceRequested += View_WebResourceRequested;
				coreWebView.WebMessageReceived +=CoreWebView_WebMessageReceived;
				//	view.EffectiveViewportChanged += View_EffectiveViewportChanged;
				//	view.AddHandler(WebView2.KeyDownEvent, new KeyEventHandler(webViewKeyDownHandler), true);
				//	view.AddHandler(WebView2.PointerPressedEvent, new PointerEventHandler(pointerEventHandler), true);
				//	view.UnsafeContentWarningDisplaying += View_UnsafeContentWarningDisplaying;
				//	view.UnsupportedUriSchemeIdentified += View_UnsupportedUriSchemeIdentified;

				//	view.UnviewableContentIdentified += View_UnviewableContentIdentified;
				//	view.ScriptNotify += View_ScriptNotify;
				//	view.DOMContentLoaded += View_DOMContentLoaded;
				//	view.NavigationFailed += View_NavigationFailed;
				//view.PointerEntered+=View_PointerEntered;
				//view.PointerExited+=View_PointerExited;
				//view.PointerMoved+=View_PointerMoved;
				//view.PointerPressed+=View_PointerPressed;
				//	view.KeyDown+=View_KeyDown;
				//	view.PreviewKeyDown+=View_PreviewKeyDown;
				view.NavigationStarting+=View_NavigationStarting;
				view.NavigationCompleted+=View_NavigationCompleted; ;
				//	view.NavigationCompleted+=View_NavigationCompleted;
				coreWebView.PermissionRequested+=View_PermissionRequested; ;
				coreWebView.NewWindowRequested+=CoreWebView_NewWindowRequested; ;
				//	webViewBrush = new WebViewBrush() { Stretch = Stretch.Fill };
				view.GotFocus += View_GotFocus;
				view.LostFocus += View_LostFocus; ;

				view.CoreWebView2.ProcessFailed+=CoreWebView2_ProcessFailed;

				//   view.CacheMode = CacheMode.
				//Grid.Se SetAlignLeftWithPanel(view, true);
				//RelativePanel.SetAlignRightWithPanel(view, true);
				///	RelativePanel.SetAlignTopWithPanel(view, true);
				//		RelativePanel.SetAlignBottomWithPanel(view, true);
				//if(isSub)
				//{
				//	// this is not really needed?  It gets updated later
				//	httpsHost = new Uri($"https://w{world}.crownofthegods.com",UriKind.Absolute);
				//	//       view.Source = new Uri($"https://w{world}.crownofthegods.com?s={subId}");
				//}

				// else
				if(isSub)
				{
					Note.ShowTip("Loading Sub, please wait.");
					ShellPage.WorkStart("Finding Sub.");
					AAnalytics.Track("LaunchSub");

					while(!AGame.contentLoadingStarted)
					{
						await Task.Delay(500);
					}

					view.Source = new Uri($"https://w{world}.crownofthegods.com?s=1",UriKind.Absolute);
					ShellPage.WorkEnd("Finding Sub.");

				}
				else
				{
					view.Source = new Uri("https://www.crownofthegods.com/home.php",UriKind.Absolute);
				}
			}
			catch(Exception ex)
			{
				WebViewException(ex);
			}
		}
		internal static void Initialize(Microsoft.UI.Xaml.Controls.Grid panel,WebView2 _view)
		{
			try
			{

				LoadJsStrings(); // let it run async



				httpFilter = new();
			//	httpFilter.AutomaticDecompression = true;
			//	httpFilter.AllowAutoRedirect = true;
			//	httpFilter.UseProxy = false;

			//httpFilter.http MaxVersion = HttpVersion.Http20;
			//	httpFilter.ServerCustomValidationRequested += ServerCustomValidationRequested;
			//httpFilter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
			//httpFilter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);

			//httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.WrongUsage);
			//httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);
			//httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Untrusted);
			//httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.IncompleteChain);
			//httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.RevocationInformationMissing);
			//httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.RevocationFailure);

			//httpFilter.AllowUI = true;
			//	httpFilter.CookieUsageBehavior = HttpCookieUsageBehavior.NoCookies;

			httpFilter.AutomaticDecompression=DecompressionMethods.All;


			//	  HttpBaseProtocolFilter.CreateForUser( User.GetDefault());
			//                         httpFilter.ServerCredential =

			//httpFilter.MaxConnectionsPerServer = 10;
			//  httpFilter.ServerCustomValidationRequested += HttpFilter_ServerCustomValidationRequested;
			//		httpFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.NoCache;
			//		httpFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
			//httpClient = new HttpClient(httpFilter);
			//httpClient.DefaultRequestVersion = HttpVersion.Version20;


			//cookieManager = httpFilter.CookieManager;
			if(!JSClient.isSub)
			{
				//var _cookies = cookieManager.GetCookies(new Uri("https://crownofthegods.com"));
				//foreach (var c in _cookies)
				//{
				//	if(c.Name == "sec_session_id")
				//	{
				//		SettingsPage.secSessionId = c.Value;
				//	}
				//}
			}
				view = _view;
				view.CoreWebView2Initialized+=CoreWebView2Initialized;
				view.EnsureCoreWebView2Async();

			}
			catch (Exception e)
			{
				LogEx(e);
			}

			



		}

		

		//private static void View_KeyDown(object sender,Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		//{
		//	Log("KeyDown");

		//}

		//private static void View_PreviewKeyDown(object sender,Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		//{
		//	Log("PreviewPointerKeyDown");
		//}

		//private static void View_PointerPressed(object sender,Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
		//{
		//	Log("PointerPressed");
		//	ShellPage.UpdateMousePosition(e);
		//	ShellPage.UpdateFocus();
		//}

		//private static void View_PointerMoved(object sender,Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
		//{
		//	Log("PointerMoved");
		//	//			ShellPage.UpdateMousePosition(e);
		//	//			ShellPage.UpdateFocus();
		//	ShellPage.UpdateMousePosition(e);
		//	ShellPage.UpdateFocus();
		//}

		//private static void View_PointerExited(object sender,Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
		//{
		//	Log("PointerExited");
		//	//		ShellPage.UpdateMousePosition(e);
		//	//		ShellPage.UpdateFocus();
		//	ShellPage.UpdateMousePosition(e);
		//	ShellPage.UpdateFocus();
		//}

		//private static void View_PointerEntered(object sender,Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
		//{
		//	//			Log("PointerEntered");
		//	//		ShellPage.UpdateMousePosition(e);
		//	//		ShellPage.UpdateFocus();
		//	ShellPage.UpdateMousePosition(e);
		//	ShellPage.UpdateFocus();
		//}

		//private static void View_CharacterReceived(UIElement sender,Microsoft.UI.Xaml.Input.CharacterReceivedRoutedEventArgs args)
		//{
		//	Log("Character recieved");
		//}

		private static void Environment_NewBrowserVersionAvailable(CoreWebView2Environment sender,object args)
		{
			AAnalytics.Track("NewBrowser");
			Log(args.ToString());
		}



		private static void CoreWebView2_ProcessFailed(CoreWebView sender,CoreWebView2ProcessFailedEventArgs args)
		{
			AAnalytics.Track("WebViewFail",new Dictionary<string,string> {
				{"Kind:",args.ProcessFailedKind.ToString() },
			//	{"Reason:",args.Reason.ToString() },
			//	{"Desc:",args.ProcessDescription },
			});
			App.DoYesNoBox("The internet is broken", "Please restart");

		}

		private async static void CoreWebView_NewWindowRequested(CoreWebView sender,CoreWebView2NewWindowRequestedEventArgs args)
		{
			var defer = args.GetDeferral();
			try
			{
				
				var webView = new WebViewPage();
				webView.ShowOrAdd(true);
	//			f.Content = webView;
				
				//w.

				////WebViewPage.DefaultUrl = new Uri(args.Uri);
				//WebViewPage.instance = null;
				//await WindowManagerService.Current.TryShowAsStandaloneAsync<WebViewPage>("overview");
				//while(WebViewPage.instance == null || WebViewPage.instance.webView==null)
				//{
				//	await Task.Delay(1000);
				//}
				var view = webView.webView;
				view.DispatcherQueue.TryEnqueue( DispatcherQueuePriority.Normal,  async ()=>
				{
					await view.EnsureCoreWebView2Async();

					args.NewWindow = view.CoreWebView2;
			//		view.CoreWebView2.Settings.UserAgent = userAgent;
					view.Source = new Uri(args.Uri);
					args.Handled = true;
					defer.Complete();
				});
				
				//	Log(args.Uri.ToString());
				//	Launcher.LaunchUriAsync(new Uri(args.Uri));
				//	}

			}
			catch(Exception __ex)
			{
				Debug.LogEx(__ex);
			}		}

		//private static void CoreWebView_NewWindowRequested(CoreWebView sender,CoreWebView2NewWindowRequestedEventArgs args)
		//{
		//	args.NewWindow.Settings.UserAgent = userAgent;
		//}

		private static void View_NavigationCompleted(WebView2 sender,CoreWebView2NavigationCompletedEventArgs args)
		{
			Log("Nav complete: " + args);
			if(hasMainPageLoaded)
			{
				coreWebView.WebResourceRequested-=View_WebResourceRequested; //-= View_WebResourceRequested1;
				coreWebView.RemoveWebResourceRequestedFilter(jsFunctionMask,ResourceContext: CoreWebView2WebResourceContext.Script);
			
			}
		}


		private static void View_PermissionRequested(CoreWebView sender,CoreWebView2PermissionRequestedEventArgs args)
		{
			args.State = CoreWebView2PermissionState.Allow;
			switch(args.PermissionKind)
			{
				case CoreWebView2PermissionKind.ClipboardRead:
				case CoreWebView2PermissionKind.Microphone:
				case CoreWebView2PermissionKind.Notifications:
					args.State = CoreWebView2PermissionState.Allow;
					break;

			}
			
		}


		

		
		
		
		private static void View_LostFocus(object sender, RoutedEventArgs e)
		{
		//	Log($"!Focus2: {ShellPage.hasKeyboardFocus} w{ShellPage.webviewHasFocus} w2{ShellPage.webviewHasFocus2}");
	//		ShellPage.webviewHasFocus2 = false;
			ShellPage.hasKeyboardFocus = false;
		}

		private static void View_GotFocus(object sender, RoutedEventArgs e)
		{
//			Log($"!Focus3: {ShellPage.hasKeyboardFocus} w{ShellPage.webviewHasFocus} w2{ShellPage.webviewHasFocus2}");
			//ShellPage.webviewHasFocus2 = true;
			ShellPage.hasKeyboardFocus = false;

		}

		//public static void ClearAllCookies(string domain= "https://crownofthegods.com")
		//{
		//	var _cookies = cookieManager.GetCookies(new Uri(domain));
		//	foreach (var c in _cookies)
		//	{
		//		cookieManager.DeleteCookie(c);
		//	}

		//}
		//private static void View_EffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
		//{
		//	var scrollView = sender as Microsoft.UI.Xaml.Controls.ScrollViewer;
		//	if (scrollView != null)
		//	{
		//		Log(args);
		//	}
		//	Log(sender);
		//}



		//		public static async void CaptureWebPage(ICanvasResourceCreator canvas)
		//		{
		//			App.DispatchOnUIThread( async () =>
		//		 {
		//			 var stream = new InMemoryRandomAccessStream();
		//			 await view.CapturePreviewToStreamAsync(stream);
		//			 ShellPage.webMask = await CanvasBitmap.LoadAsync(canvas,stream,96, CanvasAlphaMode.Premultiplied);
		////			 ShellPage.webMask= await CreateAplhaMaskFromBitmap(stream, canvas);

		//		 });
		//		}
		

		//private static void pointerEventHandler(object sender, PointerRoutedEventArgs e)
		//{
		//	Note.Show("Pointer " + e.GetCurrentPoint(sender as UIElement).Properties.PointerUpdateKind + e.KeyModifiers + e.ToString());
		//}

		//private static void webViewKeyDownHandler(object sender, KeyRoutedEventArgs e)
		//{
		//	Note.Show("Key " + e.Key + e.ToString());
		//}

//		private async static void View_NewWindowRequested(CoreWebView sender,CoreWebView2NewWindowRequestedEventArgs args)
//		{
////			args.Handled = true;
//			var uri = new Uri(args.Uri);
//			//if (WebViewPage.instance != null)
//			//{
//			//    WebViewPage.instance.Focus(FocusState.Programmatic);
//			//    return;
//			//}

//			Trace(args.Uri);
//			//          Trace(httpsHost.Host);
//			if ((httpsHost != null && uri.Host == httpsHost.Host))
//			{
//				Log(uri.ToString());
//				if (App.IsKeyPressedShift())
//				{

//					Launcher.LaunchUriAsync(uri, new LauncherOptions() { DisplayApplicationPicker = true });
//				}
//				else
//				{
//					WebViewPage.DefaultUrl = uri;
//					await WindowManagerService.Current.TryShowAsStandaloneAsync<WebViewPage>("overview");
//				}
//			}
//			else if (args.Uri.OriginalString.StartsWith("https://accounts.google.com/o/oauth2/auth?"))
//			{
//				WebViewPage.post =uri;
//				await WindowManagerService.Current.TryShowAsStandaloneAsync<WebViewPage>("login");
//			}
//			//			else if (httpsHost != null && args.)
//			else
//			{
//				Launcher.LaunchUriAsync(uri);
//			}
//		}

		//private static string GetJsString(string asm)
		//{
		//	return new StreamReader((typeof(JSClient).Assembly).GetManifestResourceStream($"COTG.JS.{asm}")).ReadToEnd();

		//}

		//static async void GetSessionSoon()
		//{
		//	await Task.Delay(2000);
		//	SettingsPage.secSessionId= GetSecSessionId();
		//	view.Source = new Uri("https://www.crownofthegods.com/home");


//		const string webResourceFilter = "webResourceFilter";
		//}

//		HTTP/1.1 200 OK
//static readonly string headers=
//			@"Content-Type: application/x-javascript Content-Encoding: gzip




		private static async void View_WebResourceRequested(CoreWebView sender,CoreWebView2WebResourceRequestedEventArgs args)
		{


			try
			{
				var req = args.Request;
				var str = req.Uri.ToString();
				if(userAgent.IsNullOrEmpty())
				{
					userAgent = args.Request.Headers.GetHeader("User-Agent" );
					if(!userAgent.IsNullOrEmpty())
					{
						App.QueueOnUIThread( () => UserAgent.SetUserAgent(JSClient.userAgent) );
					}
				}
				if (str.Contains("/jsfunctions/phaser.js"))
				{
					//var a = args.GetDeferral();
					//var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
					//	// Create the data writer object backed by the in-memory stream.
					//	using(var dataWriter = new Windows.Storage.Streams.DataWriter(stream))
					//	{
							
					//		dataWriter.WriteString(" ");
					//		await dataWriter.StoreAsync();
					//		await dataWriter.FlushAsync();
					//		dataWriter.DetachStream();
					//	}

					//	stream.Seek(0);
					args.Response = coreWebView.Environment.CreateWebResourceResponse(null,200,"Ok","Content-Type: text/javascript" );

	//				a.Complete();
				}
				else if (str.Contains("jsfunctions/game.js"))
				{
						//var a= args.GetDeferral();

						//var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
						//// Create the data writer object backed by the in-memory stream.
						//using(var dataWriter = new Windows.Storage.Streams.DataWriter(stream))
						//{
						//var js = "const cityAtlas = " +
						//  (SettingsPage.IsThemeWinter() ?
						//  		"'ms-appx-web:///Content/Art/City/Winter/building_set5.png'\n" :
						//	   "'/images/city128/building_set5.png'\n") +
						//	   jsFunkyEtc;

						//	dataWriter.WriteString(js);
						//	await dataWriter.StoreAsync();
						//	await dataWriter.FlushAsync();
						//	dataWriter.DetachStream();
						//}

						//stream.Seek(0);
						//var Response = coreWebView.Environment.CreateWebResourceResponse(stream,200,"Ok","Content-Type: application/x-javascript\r\nContent-Encoding: text/json\r\n");
						//var headers = Response.Headers;
						//headers.AppendHeader("Content-Type","application/x-javascript");
						//headers.AppendHeader("Content-Encoding","text/json");
						cookies = args.Request.Headers.GetHeader("Cookie");
					    App.QueueOnUIThread(()=>
						{
							SetupHttpFilter();
						});
					//						httpFilter.CookieContainer.SetCookies(new Uri("https://crownofthegods.com"),cookies));
					using(var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
					{
					// Create the data writer object backed by the in-memory stream.
					using(var dataWriter = new Windows.Storage.Streams.DataWriter(stream))
					{
						dataWriter.WriteString(jsFunkyEtc);
						await dataWriter.StoreAsync();
						await dataWriter.FlushAsync();
						dataWriter.DetachStream();
					}

					stream.Seek(0);

					var Response = coreWebView.Environment.CreateWebResourceResponse(stream,200,"Ok","");
					var headers = Response.Headers;
					headers.AppendHeader("Content-Type","text/javascript");
						args.Response = Response;
					}
				
					//	headers.AppendHeader("Content-Encoding","text/json");
					//	headers.("Content-Encoding","text/json");
				

						hasMainPageLoaded=true;
						//	coreWebView.RemoveWebResourceRequestedFilter(,)
						//					string host = args.Request.RequestUri.Host;
						//						string uri = args.Request.RequestUri.AbsoluteUri;

						//   var reqMsg = args.Request;
						//   var respTask = httpClient.SendRequestAsync(reqMsg).AsTask();

						//	var asm = typeof(JSClient).Assembly;
						//var newContent = new System.Net.Http.HttpStringContent(js, Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/json");
					//	args.Response = coreWebView.Environment.CreateWebResourceResponse(stream,(int)HttpStatusCode.Ok,"OK",null);//"text/json");


						//	args.Response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = newContent };
						// args.Response = resp;
						//var response = await client.SendRequestAsync(reqMsg).AsTask();
						// resp.Content = newContent;
						//    }
				//		args.Response = jsFunkyEtc;
						//a.Complete();


				}

				// if (req.Method.Method == HttpMethod.Post.Method && !req.RequestUri.LocalPath.EndsWith("poll2.php") )
				// {
				////     if (req.Content != null)
				////         await req.Content.BufferAllAsync();
				//  //   req.Content.BufferAllAsync();
				//     Log($"Post: {req.RequestUri.ToString()} {req.Headers.ToString()} {req.TransportInformation?.ToString()} {req.Properties?.ToString()} {req.Content?.ToString()}");
				//     if (args.Response != null)
				//     {
				//         Log(args.Response.Version.ToString());
				//         anyPost = req;
				//     //    await COTG.Services.RestAPI.HandleResponse(args.Request.RequestUri, args.Response);
				//     }
				// }

			}
			catch (Exception e)
			{
				LogEx(e);
			}

		}

		

		public static void PostMouseEventToJS(int x, int y, string eventName, int button, int dx = 0, int dy = 0)
		{
			App.DispatchOnUIThreadLow(() => coreWebView.PostWebMessageAsString($"{{\"postMouseEvent\":{{\"x\":{x},\"y\":{y},\"eventName\":\"{eventName}\",\"button\":{button},\"dx\":{dx},\"dy\":{dy}}}}}") );
		}

		//        public static void Refresh(object ob,RoutedEventArgs args)
		//        {
		//            if (view == null)
		//                return;
		//            Debug.Fatal();  // Todo
		////            view.Refresh();
		////            Services.NavigationService.Navigate<Views.MainPage>();
		//        }

		public static void SetStayAlive(bool stayAlive)
		{
			App.DispatchOnUIThreadLow(() => ExecuteScriptAsync("setStayAlive",( stayAlive ? 1: 0) ) );
		}
		public static void SendChat(int channel, string message)
		{
			try
			{
				const int div = 220;
				for (; ; )
				{
					string remainder = null;
					if (message.Length > div + 8)
					{
						remainder = message.Substring(div);
						message = message.Substring(0, div);
					}
					ExecuteScriptAsync("sendchat", channel, message );
					
					if (remainder == null)
						break;
					
					// Copy Whisper to the next segment
					if (message[0] == '/')
					{
						var sp = message.IndexOf(' ', 3); // break at second space
						if (sp == -1)
						{
							Assert(false);
							break;
						}
						remainder = message.Substring(0, sp + 1) + remainder;
					}
					
					Task.Delay(1000).ContinueWith( (_)=> App.DispatchOnUIThreadLow(()=> SendChat(channel, remainder) ));
					break;
				}
			}
			catch (Exception e)
			{
				LogEx(e);
			}

		}


		//public static void ViewCity(int cityId)
		//{
		//    try
		//    {
		//        if (City.IsMine(cityId))
		//        {
		//            SetViewModeCity();
		//            var city = City.StBuild(cityId,true);
		//          //  city.SetFocus( false, true, false);

		//            view.ExecuteScriptAsync("viewcity", new string[] { (cityId).ToString() });
		//        }
		//        else
		//        {
		//            ShowCity(cityId, false);
		//        }


		//    }
		//    catch (Exception e)
		//    {
		//        Log(e);
		//    }
		//}

		public static async Task<bool> CitySwitch(int cid, bool lazyMove = false, bool select = true, bool scrollIntoUI = true, bool isLocked = false, bool waitOnChange = false)
		{
			// Make sure we don't ignore the exception
			{
				// is it my city?
				if (City.CanVisit(cid))
				{
			//		Assert(cid != City.build);
					// Is it locked?
					if (!Spot.CanChangeCity(cid))
					{
						ShellPage.EnsureNotCityView();
						Note.Show("Please wait for current operation to complete");
						return false;
					}
					var city = City.GetOrAddCity(cid);

					if (city.pid != Player.activeId)
					{
						// no longer happens
						Assert(false);
						// need to switch player
						//JSClient.SetPlayer(city.pid, cityId);
					}
					else
					{
						var changed = cid != City.build;
						if (changed)
						{
							if (Spot.lockedBuild != 0 && cid != Spot.lockedBuild)
							{
								Note.Show("Please wait for current operation to complete");
								if (await App.DoYesNoBox("Busy", "Please wait for current operation to complete") != 1)
								{
									throw new System.Exception("SetBuildOverlap");
								}
							}
							bool wantUnblock = false;
							// this blocks if we can't change the city
							if (!isLocked)
								await App.uiSema.WaitAsync();
							try
							{

								//	var wasPlanner = CityBuild.isPlanner;

								//if (CityBuild.isPlanner)
								//{
								//	//	var b = City.GetBuild();
								//	//	b.BuildingsCacheToShareString();
								//	//		await b.SaveLayout();
								//	//					CityBuild.isPlanner = false;
								//	await CityBuild._IsPlanner(false, true);
								//}
							
							//	Assert(pid == Player.activeId);
								//Cosmos.PublishPlayerInfo(JSClient.jsBase.pid, City.build, JSClient.jsBase.token, JSClient.jsBase.cookies); // broadcast change

								//foreach (var p in PlayerPresence.all)
								//{
								//	if (p.pid != Player.myId && p.cid == cid)
								//	{
								//		Note.Show($"You have joined {p.name } in {City.Get(p.cid).nameMarkdown}");
								//	}
								//}

								city.SetAsBuildCity();
								//if (wasPlanner)
								//{
								//	await GetCity.Post(cid);
								//	await CityBuild._IsPlanner(true, false);
								//}
								// async
								wantUnblock = true;
							}
							finally
							{
								if (!isLocked)
									App.uiSema.Release();
							}

							if (wantUnblock)
								ExtendedQueue.UnblockQueue(cid);

						}
						city.SetFocus(scrollIntoUI, select);
						City.SyncCityBox();

						if (changed)
						{
							if (isLocked || waitOnChange)
							{
								if( await ChangeCityJSWait(cid) == false)
								{
									Note.Show("Somethings wrong, please try again");
									return false;
								}
							}
							else
								ChangeCityJS(cid);
						}
					}
					if (!lazyMove)
						cid.BringCidIntoWorldView(lazyMove, false);
				}
				else
				{
					ShowCity(cid, lazyMove, scrollIntoUI);
				}

			}
			return true;

		}

		public static async Task AddToAttackSender(int cityId)
		{
			try
			{
				await App.DispatchOnUIThreadTask(async () =>
			  {


				  await ExecuteScriptAsync("addtoattacksender", cityId );
			  });

			}
			catch (Exception e)
			{
				LogEx(e);
			}

		}

		public static async Task OpenAttackSender(string cmd)
		{
			try
			{

				var p = JsonSerializer.Deserialize<AttackSenderScript>(cmd,Json.jsonSerializerOptions);
				await CitySwitch(p.cid, false);

				await App.DispatchOnUIThreadTask(async () =>
				{
					await ExecuteScriptAsync("openAttackSender", cmd );
				});
				await Task.Delay(500);
			}
			catch (Exception e)
			{
				LogEx(e);
			}

		}
		//public static async void ShowClearMenu(int cityId)
		//{
		//	try
		//	{
		//		App.DispatchOnUIThreadLow(async () =>
		//		{

		//			if (City.StBuild(cityId, false).changed)
		//			{
		//				await view.ExecuteScriptAsync("chcity", new string[] { (cityId).ToString() });
		//				await Task.Delay(1000);
		//			}
		//			view.ExecuteScriptAsync("clearres", new string[] { (cityId).ToString() });
		//		});

		//	}
		//	catch (Exception e)
		//	{
		//		Log(e);
		//	}

		//}
		public static async Task ClearCenter(int cid)
		{
			Note.Show($"{City.Get(cid).nameMarkdown} Clear Center Res");

			if (cid != 0)
			{
				await Post.SendEncrypted("includes/clearresque.php", "{\"a\":" + cid + ",\"b\":5}", "BBIdl1a11AEkem24c2", World.CidToPlayerOrMe(cid));

			}
		}
		public static void ChangeView(ShellPage.ViewMode viewMode)
		{
			try
			{
				ShellPage.SetViewMode(viewMode);
			//	App.DispatchOnUIThreadLow(() => ExecuteScriptAsync("setviewmode", ( viewMode == ShellPage.ViewMode.city ? "c" : "r" )));

			}
			catch (Exception e)
			{
				LogEx(e);
			}
		}

		public static void SetJSCamera(Vector2 cameraC)
		{
			// Thiis is not working as it should
			//try
			//{ 
			//    view.ExecuteScriptAsync("setCameraC", new string[] {
			//        (cameraC.X).RoundToInt().ToString(),
			//        (cameraC.Y).RoundToInt().ToString()
			//    });

			//}
			//catch (Exception e)
			//{
			//    Log(e);
			//}

		}

		public static void ShowPlayer(string playerName)
		{
			try
			{
				App.CopyTextToClipboard(playerName);
				//     view.ExecuteScriptAsync("eval", new string[] { $"gspotfunct.infoPlay('{playerName}')" });
				ExecuteScriptAsync("infoPlay", playerName );
			}
			catch (Exception e)
			{
				LogEx(e);
			}
		}
		public static void ShowAlliance(string allianceName)
		{
			try
			{
				ExecuteScriptAsync("alliancelink", allianceName );
				//     view.ExecuteScriptAsync("eval", new string[] { $"gspotfunct.alliancelink('{allianceName}')" });
			}
			catch (Exception e)
			{
				LogEx(e);
			}
		}
		public static void ShowReport(string report)
		{
			if (report.IsNullOrEmpty())
				return;
			try
			{
				view.ExecuteScriptAsync( $"__c.showreport('{report}')");
			}
			catch (Exception e)
			{
				LogEx(e);
			}
		}

		//internal static void ShowCityWithoutViewChange(int cityId,bool lazy)
		//{
		//    try
		//    {

		//        ShellPage.EnsureOnScreen(cityId,lazy);
		//             view.ExecuteScriptAsync("gStphp", new string[] { (cityId%65536).ToString(),(cityId/65536).ToString() });
		//    }
		//    catch (Exception e)
		//    {
		//        Log(e);
		//    }
		//}



		public static async void ShowCity(int cityId, bool lazyMove, bool select = true, bool scrollToInUI = true)
		{
			try
			{
				//				ShellPage.SetViewModeWorld();

				// if (City.IsMine(cityId))
				{
					Spot.SetFocus(cityId, scrollToInUI, select, true, lazyMove);
				}

				// if (JSClient.IsWorldView())
				//	cityId.BringCidIntoWorldView(lazyMove, false);

				FetchCity(cityId);
				//             if( City.IsMine(cityId)  )
				//                 Raiding.UpdateTSHome();



			}
			catch (Exception e)
			{
				LogEx(e);
			}


		}

		public static void FetchCity(int cityId)
		{
			App.DispatchOnUIThreadLow( () =>
			{
				coreWebView.PostWebMessageAsString($"{{\"shCit\":{cityId}}}" );
				//int x = cityId%65536;
				//int y = cityId/65536;
				//var spotInfo = TileData.instance.GetSpotType(x, y);
				//Note.Show($"{x}:{y},{spotInfo.x}:{spotInfo.y} {spotInfo.type}");

			});
		}
		public static Windows.Foundation.IAsyncOperation<string> ExecuteScriptAsync(string func,  string arg0) => view?.ExecuteScriptAsync($"{func}(\"{arg0}\")");
		public static Windows.Foundation.IAsyncOperation<string> ExecuteScriptAsync(string func,int arg0) => view.ExecuteScriptAsync($"{func}({arg0})");

		static string FormatJSArg<T>(T a) => a switch 
		{
			int i => i.ToString() , 
			// simple string escaping
			string s => '\"' + s.Replace("\"", "\\\"") + '\"' ,
			_ => '\"'+a.ToString()+'\"' };

		public static Windows.Foundation.IAsyncOperation<string> ExecuteScriptAsync<T0>(string func,T0 arg0) => view.ExecuteScriptAsync($"{func}({FormatJSArg(arg0)})");
		public static Windows.Foundation.IAsyncOperation<string> ExecuteScriptAsync<T0,T1>(string func,T0 arg0,T1 arg1) => view.ExecuteScriptAsync($"{func}({FormatJSArg(arg0)},{FormatJSArg(arg1)})");
		public static Windows.Foundation.IAsyncOperation<string> ExecuteScriptAsync<T0, T1, T2>(string func,T0 arg0,T1 arg1, T2 arg2) =>
			view.ExecuteScriptAsync($"{func}({FormatJSArg(arg0)},{FormatJSArg(arg1)},{FormatJSArg(arg2)})");



		public static void gStCB(int cityId, Action<JsonElement> cb, int hash)
		{
			gstCBs.TryAdd(hash, cb);
			App.DispatchOnUIThreadLow(() =>
			{
				var cc = cityId.CidToContinentXY();
				var str = "[";
				var sep = "";
				for(int i=0;i<4;++i)
				{
					for (int j = 0; j < 4; ++j)
					{
						str = $"{str}{sep}{cc.x * 4 + i + (cc.y * 4 + j) * 24}";
						sep = ",";
					}
				}
				str += "]";
				
				ExecuteScriptAsync("rmp",str);
				ExecuteScriptAsync("gStQueryCB",  (cityId), hash.ToString() );
			});

		}

		//private static async Task AddJSPluginAsync()
		//{
		//    try
		//    {
		//        var asm = typeof(JSClient).Assembly;
		//        using (Stream stream = asm.GetManifestResourceStream("COTG.Javascript.funky.js"))
		//        {

		//            using (StreamReader reader = new StreamReader(stream))
		//            {
		//                Log("execute");
		//                await view.ExecuteScriptAsync("eval", new string[] { reader.ReadToEnd() });
		//                Log("funky");
		//                await view.ExecuteScriptAsync("avactor", null);
		//   }
		//        }

		//    }
		//    catch (Exception e)
		//    {
		//        Log(e);
		//    }
		//}

		// Gets an overview of all cities
		//public static async Task GetCitylistOverview()
		//{

		//    var str = await view.ExecuteScriptAsync("getppdt", null);
		//    ppdt = JsonDocument.Parse(str);
		//    UpdatePPDT(ppdt.RootElement);
		//}

		//public static async Task PollCity(int cid)
		//{
		//	await Task.Delay(50);
		//	await view.ExecuteScriptAsync("pollthis", new[] { cid.ToString() });
		//	await Task.Delay(400); // hack:  Todo, handle this property
		//	await view.ExecuteScriptAsync("pollthis", new[] { cid.ToString() });
		//	await Task.Delay(300); // hack:  Todo, handle this property
		//}
		static readonly float[] researchRamp = { 0, 1, 3, 6, 10, 15, 20, 25, 30, 35, 40, 45, 50 };

		static ConcurrentDictionary<int, Action<JsonElement>> gstCBs = new ConcurrentDictionary<int, Action<JsonElement>>();
		public static bool hasMainPageLoaded;
		private static void BonusesUpdated()
		{
			cartTravel = 10.0f / (1.0 + faith.merius * 0.5 / 100 + (researchRamp[research[28]]) / 100);
			shipTravel = 5.0f / (1.0 + faith.merius * 0.5 / 100 + (researchRamp[research[27]]) / 100);

			// these are all scaled by 100 to reduce rounding errors
			ttSpeedBonus[0] = 100; // no speed reserach for guard
			ttSpeedBonus[1] = 100 + (faith.domdis) * 0.5f  + (researchRamp[research[12]]) ;
			ttSpeedBonus[2] = 100 + (faith.ibria) * 0.5f  + (researchRamp[research[8]]) ;
			ttSpeedBonus[3] = 100 + (faith.ibria) * 0.5f  + (researchRamp[research[8]]) ;
			ttSpeedBonus[4] = 100 + (faith.ibria) * 0.5f  + (researchRamp[research[8]]) ;
			ttSpeedBonus[5] = 100 + (faith.ibria) * 0.5f  + (researchRamp[research[8]]) ;
			ttSpeedBonus[6] = 100 + (faith.ibria) * 0.5f  + (researchRamp[research[8]]) ;
			ttSpeedBonus[7] = 100 + (faith.ibria) * 0.5f  + (researchRamp[research[11]]) ;
			ttSpeedBonus[8] = 100 + (faith.ibria) * 0.5f + (researchRamp[research[9]]) ;
			ttSpeedBonus[9] = 100 + (faith.ibria) * 0.5f  + (researchRamp[research[9]]) ;
			ttSpeedBonus[10] = 100 + (faith.ibria) * 0.5f  + (researchRamp[research[9]]) ;
			ttSpeedBonus[11] = 100 + (faith.ibria) * 0.5f  + (researchRamp[research[9]]) ;
			ttSpeedBonus[12] = 100 + (faith.domdis) * 0.5f + (researchRamp[research[12]]) ;
			ttSpeedBonus[13] = 100 + (faith.domdis) * 0.5f + (researchRamp[research[12]]) ;
			ttSpeedBonus[14] = 100 + (faith.domdis) * 0.5f + (researchRamp[research[13]]) ;
			ttSpeedBonus[15] = 100 + (faith.domdis) * 0.5f + (researchRamp[research[13]]) ;
			ttSpeedBonus[16] = 100 + (faith.domdis) * 0.5f + (researchRamp[research[13]]) ;
			ttSpeedBonus[17] = 100 + (faith.evara) * 0.5f + (researchRamp[research[14]]) ;


			ttCombatBonus[0] = 1 + (faith.naera) * 0.5f / 100 + (researchRamp[research[29]]) / 100;
			ttCombatBonus[1] = 1 + (faith.naera) * 0.5f / 100 + (researchRamp[research[42]]) / 100;
			ttCombatBonus[2] = 1 + (faith.naera) * 0.5f / 100 + (researchRamp[research[30]]) / 100;
			ttCombatBonus[3] = 1 + (faith.naera) * 0.5f / 100 + (researchRamp[research[31]]) / 100;
			ttCombatBonus[4] = 1 + (faith.naera) * 0.5f / 100 + (researchRamp[research[32]]) / 100;
			ttCombatBonus[5] = 1 + (faith.vexemis) * 0.5f / 100 + (researchRamp[research[33]]) / 100;
			ttCombatBonus[6] = 1 + (faith.vexemis) * 0.5f / 100 + (researchRamp[research[34]]) / 100;
			ttCombatBonus[7] = 1 + (faith.vexemis) * 0.5f / 100 + (researchRamp[research[46]]) / 100;
			ttCombatBonus[8] = 1 + (faith.naera) * 0.5f / 100 + (researchRamp[research[35]]) / 100;
			ttCombatBonus[9] = 1 + (faith.naera) * 0.5f / 100 + (researchRamp[research[36]]) / 100;
			ttCombatBonus[10] = 1 + (faith.vexemis) * 0.5f / 100 + (researchRamp[research[37]]) / 100;
			ttCombatBonus[11] = 1 + (faith.vexemis) * 0.5f / 100 + (researchRamp[research[38]]) / 100;
			ttCombatBonus[14] = 1 + (faith.ylanna) * 0.5f / 100 + (researchRamp[research[44]]) / 100;
			ttCombatBonus[15] = 1 + (faith.ylanna) * 0.5f / 100 + (researchRamp[research[43]]) / 100;
			ttCombatBonus[16] = 1 + (faith.cyndros) * 0.5f / 100 + (researchRamp[research[45]]) / 100;
			ttCombatBonus[17] = 1; // no combat research for senator
		}
		public static JsonDocument extCityHack;
		public static bool ppdtInitialized;
		static private int[] lastCln = null;
		public static async void UpdatePPDT(JsonElement jse, int thisPid, bool pruneCities=false, bool updateBuildCity=false)
		{
			// Todo:  should we update our local PPDT to the server?
		
			int clChanged = 0;
			// City lists
			try
			{
				if (!councillorsChecked)
				{
					if (jse.TryGetProperty("cob", out var cob))
					{
						councillorsChecked = true;

						foreach (var c in cob.EnumerateObject())
						{
							var t = ServerTimeOffsetSeonds((uint)c.Value.GetAsInt64());
							if (t <= 0)
							{
								App.DispatchOnUIThreadLow(ShowCouncillorsMissingDialog);
								break;
							}

						}
					}
				}

				bool bonusesUpdated = false;
				// research?
				if (jse.TryGetProperty("rs", out var rss))
				{
					foreach (var rs in rss.EnumerateObject())
					{
						var id = int.Parse(rs.Name);
						if (id < researchCount)
						{
							research[id] = (byte)rs.Value.GetInt("l"); // this will wrap for senator level (research not supported here)
																	   // the rest are 0..12
						}
					}
					bonusesUpdated = true;

				}
				if(jse.TryGetProperty("tcps", out var tcps))
				{
					TradeSettings.all = JsonSerializer.Deserialize<TradeSettings[]>(tcps.ToString(), Json.jsonSerializerOptions);
					
					App.DispatchOnUIThreadIdle( ()=>
					{
						ResSettings.tradeSettingsItemsSource = TradeSettings.all;
					});
				}

				if(jse.TryGetProperty("wmo", out var wo))
				{
					WorldViewSettings.ownCities.isOn = wo.GetAsInt("0")==1;
					WorldViewSettings.ownCities.color = wo.GetColor("16");

					WorldViewSettings.ownAlliance.isOn = wo.GetAsInt("1") == 1;
					WorldViewSettings.ownAlliance.color = wo.GetColor("17");

					WorldViewSettings.alliedAlliance.isOn = wo.GetAsInt("2") == 1;
					WorldViewSettings.alliedAlliance.color = wo.GetColor("18");

					WorldViewSettings.napAlliance.isOn = wo.GetAsInt("3") == 1;
					WorldViewSettings.napAlliance.color = wo.GetColor("19");

					WorldViewSettings.enemyAlliance.isOn = wo.GetAsInt("4") == 1;
					WorldViewSettings.enemyAlliance.color = wo.GetColor("20");

					WorldViewSettings.otherPlayers.isOn = wo.GetAsInt("15") == 1;
					WorldViewSettings.otherPlayers.color = wo.GetColor("28");

					WorldViewSettings.lawless.isOn = wo.GetAsInt("5") == 1;
					WorldViewSettings.lawless.color = wo.GetColor("21");

					WorldViewSettings.friends.isOn = wo.GetAsInt("6") == 1;
					WorldViewSettings.friends.color = wo.GetColor("22");

					WorldViewSettings.citiesWithoutCastles = wo.GetAsInt("7") == 1;
					WorldViewSettings.citiesWithoutWater = wo.GetAsInt("8") == 1;
					WorldViewSettings.citiesWithoutTemples = wo.GetAsInt("9") == 1;
					
					WorldViewSettings.caverns.isOn = wo.GetAsInt("10") == 1;
					WorldViewSettings.caverns.color = wo.GetColor("23");

					WorldViewSettings.bosses.isOn = wo.GetAsInt("11") == 1;
					WorldViewSettings.bosses.color = wo.GetColor("24");

					WorldViewSettings.shrines.isOn = wo.GetAsInt("12") == 1;
					WorldViewSettings.shrines.color = wo.GetColor("25");

					WorldViewSettings.inactivePortals.isOn = wo.GetAsInt("13") == 1;
					WorldViewSettings.inactivePortals.color = wo.GetColor("26");

					WorldViewSettings.activePortals.isOn = wo.GetAsInt("14") == 1;
					WorldViewSettings.activePortals.color = wo.GetColor("27");
				
					WorldViewSettings.cavernMinLevel = wo.GetAsInt("29");
					WorldViewSettings.cavernMaxLevel = wo.GetAsInt("30");

					WorldViewSettings.bossMinLevel = wo.GetAsInt("31");
					WorldViewSettings.bossMaxLevel = wo.GetAsInt("32");


					WorldViewSettings.playerSettings.Clear();
					if(wo.TryGetProperty("p",out var p) && p.ValueKind == JsonValueKind.Object)
					{
						foreach(var pset in p.EnumerateObject())
						{
							var ps = new WorldViewSettings.PlayerSetting();
							ps.pid = pset.Value.GetAsInt("a");
							ps.color = pset.Value.GetColor("c");
							ps.isOn = pset.Value.GetAsInt("d") == 1;

							WorldViewSettings.playerSettings.Add(ps.pid, ps);
						}
					}
					WorldViewSettings.allianceSettings.Clear();
					if (wo.TryGetProperty("a", out var a))
					{
						
						foreach (var pset in a.EnumerateObject())
						{
							var ps = new WorldViewSettings.AllianceSetting();
							ps.pid = pset.Value.GetAsInt("a");
							ps.color = pset.Value.GetColor("c");
							ps.isOn = pset.Value.GetAsInt("d") == 1;

							WorldViewSettings.allianceSettings.Add(ps.pid, ps);
						}
					}

				//	if (World.completed)
				//		GetWorldInfo.Send();
				}

				if (jse.TryGetProperty("mvb", out var mvb))
				{
					Log("MVB: " + mvb.ToString());
					Player.moveSlots = mvb.ValueKind == JsonValueKind.Number ? mvb.GetAsInt() : mvb.GetAsInt("l");

				}
				
				if (jse.TryGetProperty("fa", out var fa))
				{
					faith.evara = fa.GetAsByte("1");
					faith.vexemis = fa.GetAsByte("2"); // 2
					faith.domdis = fa.GetAsByte("3");
					faith.cyndros = fa.GetAsByte("4");
					faith.merius = fa.GetAsByte("5");
					faith.ylanna = fa.GetAsByte("6");
					faith.ibria = fa.GetAsByte("7");
					faith.naera = fa.GetAsByte("8");

					bonusesUpdated = true;

				}
				if (bonusesUpdated)
					BonusesUpdated();

				List<CityList> lists = new List<CityList>();
				if (jse.TryGetProperty("cl", out var cityListNames))
				{
					++clChanged;
					//  var clList = new List<string>();
					if (cityListNames.ValueKind == JsonValueKind.Object)
					{
						foreach (var cn in cityListNames.EnumerateObject())
						{
							var l = new CityList() { name = cn.Value.GetString(), id = int.Parse(cn.Name) };
							lists.Add(l);
						}
					}
					//  lists.Sort((a, b) => a.name.CompareTo(b.name));


					if (jse.TryGetProperty("cln", out var cln))
					{
						//  ++clChanged;

						//  var clList = new List<string>();
						lastCln = GetIntArray(cln).Values.ToArray();

					}
					if (lastCln != null)
					{

						var prior = lists;
						lists = new List<CityList>();
						foreach (var id in lastCln)
						{
							var ins = prior.Find((a) => a.id == id);
							if (ins != null)
								lists.Add(ins);

						}
					}
					//  lists.Sort((a, b) => a.name.CompareTo(b.name));

				}

				if (jse.TryGetProperty("r", out var r))
				{
					Player.myTitle = r.GetAsInt();
				}

				if (jse.TryGetProperty("clc", out var cityListCities))
				{
					++clChanged;
					if (cityListCities.ValueKind == JsonValueKind.Object)
					{
						foreach (var clc in cityListCities.EnumerateObject())
						{
							if (clc.Value.ValueKind == JsonValueKind.Null)
								continue;
							var id = int.Parse(clc.Name);
							var cityList = lists.Find((a) => a.id == id);
							foreach (var cityId in GetIntArray(clc.Value))
							{
								cityList.cities.Add(cityId.Value);

							}

						}
					}
				}

				if (clChanged >= 2)
				{
					App.DispatchOnUIThreadIdle(() =>
				   {
					   var priorIndex = CityList.box.SelectedIndex;
					   CityList.selections = new CityList[lists.Count + 1];
					   CityList.selections[0] = (CityList.allCities);

					   for (int i = 0; i < lists.Count; ++i)
					   {
						   CityList.selections[i + 1] = (lists[i]);
					   }
					   CityList.all = lists.ToArray();
					   if (SettingsPage.instance!=null)
					   {
						   SettingsPage.instance.hubCityListBox.ItemsSource = null;
						   SettingsPage.instance.hubCityListBox.ItemsSource = CityList.all;
					   }
					   CityList.box.ItemsSource = CityList.selections;
					   CityList.box.SelectedIndex = priorIndex; // Hopefully this is close enough
																//                       SettingsPage.instance.
				   });
				}
			}

			catch (Exception E)
			{
			//	Log(E);
				Log("City lists invalid, maybe you have none");
			}


			// extract cities
			if (jse.TryGetProperty("c", out var cProp))
			{
				while (!World.initialized)
				{
					await Task.Delay(1000);

				}

				var now = DateTimeOffset.UtcNow;
				foreach (var jsCity in cProp.EnumerateArray())
				{
					//                    Log(jsCity.ToString());
					var cid = jsCity.GetProperty("1").GetInt32();
					Assert(thisPid != 0);
					if (pruneCities)
					{
						if (World.GetInfoFromCid(cid).player != thisPid)
						{
							Note.Show($"Invalid City, was it lost? {cid.CidToString()}");
							ChangeCityJS(cid);
							
							await Task.Delay(2000);
							continue;

						}
					}


					var city = City.GetOrAddCity(cid);
					city.type = City.typeCity;
					if (thisPid != 0)
						city.pid = thisPid;
					var name = jsCity.GetProperty("2").GetString();
					int i = name.LastIndexOf('-');
					if (i != -1 && i+2 < name.Length )
					{
						city.remarks = name.Substring(i + 2);
						city._cityName = name.Substring(0, i - 1);
						city.UpdateTags();
					}
					else
					{
						city._cityName = name;
					}
					city.type = City.typeCity;
					city._tsTotal = jsCity.GetAsInt("8");
					//city._tsHome = jsCity.GetAsInt("17");
		//			city.troopsTotal = TroopTypeCount.empty;
	//				city.troopsHome = TroopTypeCount.empty;

		//			Trace($"TS Home {city._tsHome}");

					//   city.tsRaid = city.tsHome;
					city.isCastle = jsCity.GetAsInt("12") > 0;
					city.points = (ushort)jsCity.GetAsInt("4");

					city.isOnWater |= jsCity.GetAsInt("16") > 0;  // Use Or in case the data is imcomplete or missing, in which case we get it from world data, if that is not incomplete or missing ;)
					city.isTemple = jsCity.GetAsInt("15") > 0;
				//	city.pid = Player.activeId;
					//  Log($"Temple:{jsCity.GetAsInt("15")}:{jsCity.ToString()}");


				}
				if(updateBuildCity)
				{
					if (jse.TryGetProperty("lcit", out var lcit))
					{
						var cid = lcit.GetAsInt();
						CitySwitch(cid, true);
					}
				}
				CityList.NotifyChange();
				
				if (!ppdtInitialized)
				{

					ppdtInitialized = true;

					//Task.Delay(500).ContinueWith( _ => App.DispatchOnUIThreadSneakyLow( MainPage.instance.Refresh));
					ShellPage.RefreshTabs.Go();
				}
				
				//    Log(City.all.ToString());
				//   Log(City.all.Count());

			}
			MainPage.CheckTipRaiding();
			City.CitiesChanged();
			// Log($"PPDT: c:{cUpdated}, clc:{clChanged}");

			// Log(ppdt.ToString());
		}

		
		public static async Task CityRefresh()
		{
				if (JSClient.ppdtInitialized)
				{
					// don't wait on this
					await JSClient.JSInvokeTask($"cityRefresh({City.build})" );
					Game.City.CitiesChanged();
				}
		}



private static async void ShowCouncillorsMissingDialog()
		{
			 await App.DispatchOnUIThreadTask(async () =>
			{

				var msg = new ContentDialog()
				{
					IsPrimaryButtonEnabled = true,
					Title = "Councillors Expired",
					Content = "Unfortunately, this app requires councillors",
					PrimaryButtonText = "Okay",
					CloseButtonText = "Quit"
				};
				return  await msg.ShowAsync2();
			});
		}

		private static SortedList<int, int> GetIntArray(JsonElement cln)
		{
			var rv = new SortedList<int, int>();
			if (cln.ValueKind == JsonValueKind.Array)
			{
				foreach (var cn in cln.EnumerateArray())
				{
					rv.Add(rv.Count, cn.GetAsInt());
				}
			}
			else if (cln.ValueKind == JsonValueKind.Object)
			{
				foreach (var cn in cln.EnumerateObject())
				{
					rv.Add(int.Parse(cn.Name), cn.Value.GetAsInt());
				}
			}
			return rv;
		}

		

		
		
		//static Certificate cotgCert;
		//private static void  ServerCustomValidationRequested(HttpBaseProtocolFilter sender, HttpServerCustomValidationRequestedEventArgs customValidationArgs)
		//{
		
		//	cotgCert = customValidationArgs.ServerCertificate;
		//	// Validate the server certificate as required.
		//	//            customValidationArgs.Reject();
		//}
		////	static HttpResponseMessage resp;
		////	private static async void GetMe(Microsoft.UI.Xaml.Controls.WebViewWebResourceRequestedEventArgs args, Deferral def)
		////	{
		////		var client = new HttpClient(httpFilter);
		////		//	var headers = httpClient.DefaultRequestHeaders;
		////		//client.DefaultRequestHeaders.Accept.Clear();
		////		//client.DefaultRequestHeaders.Accept.ParseAdd("application/signed-exchange");

		////		// httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("X-Requested-With", "XMLHttpRequest");
		////		//    httpClient.DefaultRequestHeaders.Referer = new Uri(httpsHost, "/overview.php?s=0");// new Uri($"https://w{world}.crownofthegods.com");
		////		client.DefaultRequestHeaders.Referer = new Uri("https://www.crownofthegods.com/"); // new Uri                                                       //             req.Headers.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
		////															 //httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");

		////			client.DefaultRequestHeaders.Host = new Windows.Networking.HostName("www.crownofthegods.com");
		////		//   Log($"Built headers {httpClient.DefaultRequestHeaders.ToString() }");


		////		client.DefaultRequestHeaders.UserAgent.TryParseAdd(userAgent);



		////		client.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Dest", "document");
		////		client.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Site", "same-origin");
		////		client.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-User", "?1");
		////		client.DefaultRequestHeaders.TryAppendWithoutValidation("sec-ch-ua", "\" Not; A Brand\";v=\"99\", \"Microsoft Edge\"; v=\"91\", \"Chromium\"; v=\"91\"");
		////		client.DefaultRequestHeaders.TryAppendWithoutValidation("sec-ch-ua-mobile", "?0");

		//////		SetCookie("lout", "1");
		//////		SetCookie("G_AUTHUSER_H", "0", "www.crownofthegods.com", "/");
		//////		SetCookie("G_AUTHUSER_H", "0", "www.crownofthegods.com", "/home");


		////		var req = args.Request;
		////		args.Request.Headers.Accept.Clear();
		////		args.Request.Headers.Accept.ParseAdd("application/signed-exchange;v=b3;q=0.9");
		////		{
		////			resp = await client.SendRequestAsync(args.Request, HttpCompletionOption.ResponseContentRead);
		////			foreach (var h in resp.Headers)
		////			{
		////				Log(h.Key);
		////				Log(h.Value);
		////				if(h.Key == "Set-Cookie")
		////				{
		////					var bb = h.Value.Split(';', StringSplitOptions.RemoveEmptyEntries);
		////					var bb2 = bb[0].Split('=', StringSplitOptions.RemoveEmptyEntries);
		////					SettingsPage.secSessionId = bb2[1];
		////				}

		////			}

		////			foreach (var h in resp.Content.Headers)
		////			{
		////				Log(h.Key);
		////				Log(h.Value);

		////			}
		////			var buff = await resp.Content.ReadAsStringAsync();
		////			Log(buff);
		////			args.Response = resp;
		////			def.Complete();
		////		}

		////	}

		private static void View_NavigationStarting(WebView2 sender,CoreWebView2NavigationStartingEventArgs args)
		{

			try
			{
				// You can check out any time you like..
				if(hasMainPageLoaded)
				{
					args.Cancel = true;
					return;
				}

			var uri = new Uri(args.Uri);
			//if (args.Uri.ToString().StartsWith("https://www.crownofthegods.com/?email"))
			//{
			//	GetMe(args.Uri);
			//	return;
			//}
			Log($"Nav start {uri}");


			var match = urlMatch.Match(uri.Host);

			if(match.Groups.Count == 2 && (uri.LocalPath == "/" || uri.Fragment.Contains('&')))
			{
				//if (httpFilter != null)
				//	Debug.Fatal();  // Todo
				world = int.Parse(match.Groups[1].ToString());


				// once we have the world id we can load the background


					httpsHostString = $"https://{uri.Host}";
					httpsHost = new Uri(httpsHostString);



				
					}
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
		}
		static void SetupHttpFilter()
		{
			try
			{
			{
				//if (httpFilter != null)
				//	Debug.Fatal();  // Todo
				

				// once we have the world id we can load the background

				try
				{

					
					//httpFilter = 

					//						if (subId == 0)
					//	httpFilter.CookieUsageBehavior = HttpCookieUsageBehavior.NoCookies;// HttpCookieUsageBehavior.Default;
					//						httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.IncompleteChain);
					//						                    httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.InvalidCertificateAuthorityPolicy);
					//          httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.OtherErrors);
					//      httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.BasicConstraintsError);
					//     httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.InvalidSignature);
					//					httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.RevocationInformationMissing);
					//					httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.RevocationFailure);
					//						                httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Revoked);
					//		httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.WrongUsage);
					//		httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);
					//		httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Untrusted);

					//                        "Success", "Revoked", "InvalidSignature", "InvalidCertificateAuthorityPolicy", "BasicConstraintsError", "UnknownCriticalExtension", "OtherErrors""Success", "Revoked", "InvalidSignature", "InvalidCertificateAuthorityPolicy", "BasicConstraintsError", "UnknownCriticalExtension", "OtherErrors"
					//                       httpFilter.AllowUI = true;

					//                        httpFilter.User.





					//	clientPool = new ConcurrentBag<HttpClient>();
					//	for (int i = 0; i < clientCount; ++i)
					{
						//	httpClient = new HttpClient(httpFilter); // reset
						//   httpClient = new HttpClient(); // reset
						//                        var headers = httpClient.DefaultRequestHeaders;
						//     headers.TryAppendWithoutValidation("Content-Type",@"application/x-www-form-urlencoded; charset=UTF-8");
						// headers.TryAppendWithoutValidation("Accept-Encoding","gzip, deflate, br");
						//                        headers.TryAppendWithoutValidation("X-Requested-With", "XMLHttpRequest");
						//    headers.Accept.TryParseAdd(new HttpMediaTypeHeaderValue(@"application/json"));
						//   headers.Add("Accept", @"*/*");
						//                            httpClient.DefaultRequestHeaders.Clear();
						//	httpClient.DefaultRequestHeaders.AcceptLanguage.TryParseAdd("en-US,en;q=0.5");
						//    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(@"Mozilla/5.0 (Windows NT 10.0; Win64; x64; WebView2/3.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.19631");
						//    httpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Credentials", "true");
						//	httpClient.DefaultRequestHeaders.Accept.TryParseAdd("*/*");
						// httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("X-Requested-With", "XMLHttpRequest");
						//    httpClient.DefaultRequestHeaders.Referer = new Uri(httpsHost, "/overview.php?s=0");// new Uri($"https://w{world}.crownofthegods.com");
					
							//httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Pragma", "no-cache");
							var cookieContainer = new CookieContainer();
							//cookieContainer.SetCookies(new Uri("https://crownofthegods.com"),cookies.Replace(';',','));

							foreach(var s in cookies.Split(';',StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries))
							{
								var vEq = s.IndexOf('=');
								if(vEq== -1)
									continue;
								cookieContainer.Add(new System.Net.Cookie( s.Substring(0,vEq).Trim(), s.Substring(vEq+1).Trim(),"/","crownofthegods.com"));
							}

							httpFilter.CookieContainer = cookieContainer;//.SetCookies(new Uri("https://crownofthegods.com",UriKind.Absolute),cookies);

							//httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Site", "same-origin");
							//httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Mode", "cors");
							//httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Dest", "empty");

							//	clientPool.Add(httpClient);


						}
						httpClient = new HttpClient(httpFilter);
					httpClient.DefaultRequestVersion = HttpVersion.Version20;
						httpClient.DefaultRequestHeaders.Referrer = new Uri(httpsHost,$"/overview.php?s={subId}");// new Uri                                                       //             req.Headers.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
						if(subId != 0)
							httpClient.DefaultRequestHeaders.TryAddWithoutValidation("pp-ss",subId.ToString());

						//httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
						//   Log($"Built headers {httpClient.DefaultRequestHeaders.ToString() }");
						//	httpFilter.CookieContainer.SetCookies(uri,cookies);// = coreWebView.CookieManager;

						httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(userAgent);
						//cookieManager = new HttpBaseProtocolFilter().CookieManager;
						//  clientPool.CompleteAdding();

						AGame.LoadWorldBackground();

				}
				catch(Exception e)
				{

					LogEx(e);
				}

			}
			}
			catch (Exception e)
			{
				LogEx(e);
	
	}
}

		static ConcurrentDictionary<string, BitmapImage> imageCache = new ConcurrentDictionary<string, BitmapImage>();
	//	private static long gameMSAtStart;
//		private static DateTimeOffset launchTime;

		public static BitmapImage GetImage(string dir, string name)
		{
			return ImageHelper.FromImages(name);
			//if (imageCache.TryGetValue(name, out var b))
			//    return b;
			//b = new BitmapImage();
			//imageCache.TryAdd(name, b);

			//await LoadImage(b,dir,name);
			//return b;
		}
		//async static Task LoadImage(BitmapImage b, string dir, string name)
		//{

		//	try
		//	{

		//		var uri = new Uri(httpsHost, dir + name);
		//		using (var response = await downloadImageClient.GetAsync(uri))
		//		{
		//			response.EnsureSuccessStatusCode();
		//			var buff = await response.Content.ReadAsBufferAsync();

		//			var temp = new byte[buff.Length];

		//			var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buff);
		//			dataReader.ReadBytes(temp);

		//			// Get the path to the app's Assets folder.
		//			//  string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;


		//			// Get the folder object that corresponds to this absolute path in the file system.
		//			// StorageFolder folder = await DownloadsFolder.CreateFolderAsync(@"\cotg");
		//			var file = await DownloadsFolder.CreateFileAsync(name);
		//			await FileIO.WriteBytesAsync(file, temp);


		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		Log(ex);
		//	}

		//}
		//private static void HttpFilter_ServerCustomValidationRequested(HttpBaseProtocolFilter sender, HttpServerCustomValidationRequestedEventArgs args)
		//{
		//	Log(args.ToString());
		//}

		



		static DispatcherTimer presenceTimer;
		class WaitOnCityDataData
		{
			public int cid;
			public TaskCompletionSource<bool> t;

			public WaitOnCityDataData(int cid)
			{
				this.cid = cid;
				t=new TaskCompletionSource<bool>();
			}

			public void Done()
			{
				try
				{
					cid = 0;
					var _t = t;
					t = null;
					if (_t != null)
						_t.SetResult(true);

				}
				catch (Exception ex)
				{

					LogEx(ex);
				}
			}
			public void Abort()
			{
				try
				{
					cid = 0;
					var _t = t;
					t = null;
					if(_t != null)
						_t.SetResult(false);

				}
				catch (Exception ex)
				{

					LogEx(ex);
				}
			}
			public bool isDone => t == null;
		}

		static WaitOnCityDataData[] waitingOnCityData = Array.Empty<WaitOnCityDataData>();
		static void ChangeCityJS(int cityId)
		{
			App.DispatchOnUIThreadLow(() =>
							coreWebView.PostWebMessageAsString($"{{\"chcity\":{cityId}}}") );

		}
		static async Task<bool> ChangeCityJSWait(int cityId)
		{
			Log($"Wait {City.GetOrAdd(cityId).nameAndRemarks}");
			var i = new WaitOnCityDataData(cityId);
			waitingOnCityData =  waitingOnCityData.ArrayAppend(i);
			ChangeCityJS(cityId);
			var t = i.t.Task;
			var xx = (await Task.WhenAny( t , Task.Delay(10000) )) == t;
			Assert(xx);
			Log($"WaitComplete {xx} {City.GetOrAdd(cityId).nameAndRemarks}");
			if(!xx)
			{
				i.Abort();
			}
			return xx;
		}
		private static void CoreWebView_WebMessageReceived(CoreWebView sender,CoreWebView2WebMessageReceivedEventArgs args)
		{
			var eValue = args.WebMessageAsJson;
			Task.Run(async () =>
		   {
		   try
		   {
			   bool gotCreds = false;
			   Log($"Notify: {eValue.Length},{sender}:{eValue.Truncate(128) }");
			   var jsDoc = JsonDocument.Parse(eValue);
			   var jsd = jsDoc.RootElement;
			   foreach (var jsp in jsd.EnumerateObject())
			   {
				   switch (jsp.Name)
				   {
					   case "jsvars":
						   {
						//	   App.DispatchOnUIThreadLow(() => ShellPage.instance.cookie.Visibility = Visibility.Collapsed);

							   var jso = jsp.Value;

							//   var s = CookieDB.Serialize(cookieManager);// GetSecSessionId();
							   var token = jso.GetString("token");
							   var raidSecret = jso.GetString("raid");
							   var agent = jso.GetString("agent");
							 //  var cookie = jso.GetString("cookie");
							   //   Log(jsVars.cookie);
							   Log(token);
							Log(cookies);
							  // Log(s);
							 //  for (int i = 0; i < clientCount; ++i)
							 //  {
								//   await clientPoolSema.WaitAsync();//.ConfigureAwait(false);
								////   httpFilter.CookieManager.SetCookie(new HttpCookie)

							 //  }
								   //HTTPCook
								   // {

								   //  var cooki
								   // }

								   for (; ; )
							   {
								   try
								   {
									   {
										   //    var clients = clientPool.ToArray();
//										   foreach (var httpClient in clientPool)
//										   {
//												  // httpClient.DefaultRequestHeaders.Cookie = "sec_session_id="+s;

////											   		if (subId == 0)
//											   		//  httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Cookie",cookies);//"sec_session_id=" + s);
//										   }
									   }
								   }
								   catch (Exception _ex)
								   {
									   LogEx(_ex);
									   await Task.Delay(1000);//.ConfigureAwait(false);
									   continue;

								   }
								   break;
							   }

							//   clientPoolSema.Release(clientCount);

							   var timeOffset = jso.GetAsInt64("timeoffset");
							   var timeOffsetSecondsRounded = Math.Round(timeOffset / (1000.0 * 60*30)) * 60 * 30.0f; // round to nearest half hour
							   gameTOffset = TimeSpan.FromSeconds(timeOffsetSecondsRounded);
							   gameTOffsetSeconds = (int)timeOffsetSecondsRounded;
							//   gameTOffsetMs = (long)timeOffsetSecondsRounded*1000;
							   var str = timeOffsetSecondsRounded >= 0 ? " +" : " ";
							   str += $"{gameTOffset.Hours:D2}:{gameTOffset.Minutes:D2}";
							   Helpers.JSON.timeZoneString = str;
							   //   Log(JSONHelper.timeZoneString);

							   Log($"TOffset {gameTOffset}");
							   Log(ServerTime().ToString("u"));
							   ppss = jso.GetAsInt("ppss");
							   Player.myName = jso.GetString("player");
							   if (Player.subOwner == null)
								   Player.subOwner = Player.myName;
							   Player.activeId = Player.myId = jso.GetAsInt("pid"); ;
							   Player.myIds.Add(Player.myId);
							   var cid = jso.GetAsInt("cid");
							   City.build = City.focus = cid;
							   NavStack.Push(cid);
							   AGame.CameraC = cid.CidToWorldV();
							   //Note.L("cid=" + cid.CidToString());
							   //gameMSAtStart = jso.GetAsInt64("time");
							   //launchTime = DateTimeOffset.UtcNow;
							   //    Log(jsVars.ToString());
							   //  SettingsPage.secSessionId = jso.GetAsString("s");
							   //		AGame.clientTL.X = jso.GetAsFloat("left");
							   //  AGame.clientTL.Y = jso.GetAsFloat("top");
							   //   Log($"WebClient:{AGame.clientTL} {ShellPage.webclientSpan.y}");
							   //     Note.Show($" {clientSpanX}:{clientSpanY} {ShellPage.clientTL} ");
							   gotCreds = true;
							   //			   spanX = jso.GetAsInt("spanX");
							   //			   spanY = jso.GetAsInt("spanY");
							   //			   Note.Show($"ClientSpan: {spanX}x{spanY}");
							   //    Log($"Built heades {httpClient.DefaultRequestHeaders.ToString() }");

							   //   UpdatePPDT(jso.GetProperty("ppdt"));
							   var ppdt = jso.GetProperty("ppdt");
									// todo: utf
									AddPlayer(true, true, Player.myId, Player.myName, token, raidSecret,cookies);//, s, ppdt.ToString());


							   UpdatePPDT(ppdt, Player.myId, pruneCities: true);
							   if (Player.isAvatarOrTest)
								   Raid.test = true;
							   World.RunWhenLoaded(() => App.DispatchOnUIThreadIdle(Spot.UpdateFocusText));


							   BuildQueue.Initialize();
							   App.DispatchOnUIThreadLow(() =>
							   {
							   ShellPage.instance.coords.Text = cid.CidToString();
						//		   ShellPage.instance.cookie.Visibility = Visibility.Collapsed;
						   });

								   break;
							   }
						   case "aexp":
							   {
								   var msg = jsp.Value.ToString();
								   Note.Show($"Exported Order to clipboard: {msg}");
								   App.CopyTextToClipboard(msg);
								   break;

								   ;
							   }
						   case "buildFail":
							   {
									var city =City.GetOrAddCity( jsp.Value.GetAsInt("cid") );
									var e = jsp.Value.GetAsInt("e");
									Trace($"Build Command: {city.nameMarkdown} {e}, {jsp.Value.ToString()}");
									

									break;
							   }
						   case "error":
							   {
								   var msg = jsp.Value.GetString();
								   Trace(msg);
								 
								   break;
							   }
						   case "sub":
							   {
								   var i = jsp.Value.GetAsInt();
								   App.DispatchOnUIThread(() => Windows.System.Launcher.LaunchUriAsync(new Uri($"{App.appLink}:launch?w={world}&s={i}&n=1&p={HttpUtility.UrlEncode(Player.myName, Encoding.UTF8)}",UriKind.Absolute)));
								   break;
							   }
						   case "shcit":
							   {
								   var jso = jsp.Value;
								   var cid = jso.GetAsInt();
								   Spot.ProcessCoordClick(cid, false, App.keyModifiers, true); // then normal click
								   //App.DispatchOnUIThreadLow(async () =>
								   //{
									  // try
									  // {
										 //  var t = await App.GetClipboardText();
										 //  if (t.StartsWith("{") && t.EndsWith("}"))
										 //  {
											//   // is it json?
											//   var p = JsonSerializer.Deserialize<AttackSenderScript>(t);
											//   OpenAttackSender(t);
										 //  }
									  // }
									  // catch (Exception ex)
									  // {

									  // }
								   //});
								   break;
							   }
						   case "keyDown":
							   {
								   Log("Key");
								   //   Log($"Keydown: {jsp.Value.ToString()}");
								   VirtualKey key = default;
								   switch (jsp.Value.GetString("key"))
								   {
									   case "Control": key = VirtualKey.Control; break;
									   case "Shift": key = VirtualKey.Shift; break;
									   case "ScrollLock": key = VirtualKey.Scroll; break;
								   }
								   if (key != default)
								   {

									   App.OnKeyDown(key);
								   }
								   break;
							   }
						   case "keyUp":
							   {
								   Log("Key");
								   VirtualKey key = default;
								   switch (jsp.Value.GetString("key"))
								   {
									   case "Control": key = VirtualKey.Control; break;
									   case "Shift": key = VirtualKey.Shift; break;
									   case "ScrollLock": key = VirtualKey.Scroll; break;
								   }
								   if (key != default)
								   {
									   //   Note.Show($"{key} Up");
									   App.OnKeyUp(key);
								   }
								   break;
							   }
						   case "mouseDown":
							   {
								   Log($"mouseDown: {jsp.Value.ToString()}");
								   var but = jsp.Value.GetInt("button");
								   var x = jsp.Value.GetInt("x");
								   var y = jsp.Value.GetInt("y");
									// 2 is context button
									//if(but==2)
									//    Spot.GetFocus().ShowContextMenu(this,App.Current.m.GetPointer)
									//else
									var kind = but switch
									{
										0 => PointerUpdateKind.LeftButtonPressed,
										1 => PointerUpdateKind.MiddleButtonPressed,
										2 => PointerUpdateKind.RightButtonPressed,
										3 => PointerUpdateKind.XButton1Pressed,
										4 => PointerUpdateKind.XButton2Pressed,
										_ => PointerUpdateKind.Other };

								  // App.OnPointerPressed(kind);
								   //{
									  // var c = ShellPage.JSPointToScreen(x, y);
									  // if (c.x > 0 && c.y > 0)
									  // {
										 //  ShellPage.Canvas_PointerPressedJS(c.x, c.y, kind);
									
									  // }
								   //}
								   
								   break;
							   }
						   //case "cityinfo":
						   //    {
						   //        var jso = jsp.Value;
						   //        var cid = jso.GetAsInt("cid");
						   //        var pid = Player.NameToId(jso.GetAsString("player"));
						   //        var city = Spot.GetOrAdd(cid);
						   //        var name = jso.GetString("name");
						   //        city.pid = pid; // todo: this shoule be an int playerId
						   //                        //Assert(city.pid > 0);
						   //        city.points = (ushort)jso.GetAsInt("score");
						   //        //   city.alliance = jso.GetString("alliance"); // todo:  this should be an into alliance id
						   //        city.lastAccessed = DateTimeOffset.UtcNow;
						   //        // city.isCastle = jso.GetAsInt("castle") == 1;
						   //        city.isBlessed = city.pid > 0 ? jso.GetAsInt("bless") > 0 : false;
						   //        city.isOnWater |= jso.GetAsInt("water") != 0;  // Use Or in case the data is imcomplete or missing, in which case we get it from world data, if that is not incomplete or missing ;)
						   //        city.isTemple = jso.GetAsInt("plvl") != 0;


						   //        break;
						   //    }
						   case "notify":
							   {
								   foreach(var note in jsp.Value.EnumerateArray())
								   {
									   var str = note.GetString();
									   Log(str);
									   var ss = str.Split(',', StringSplitOptions.RemoveEmptyEntries);
									   if(int.TryParse(ss[0], out var id))
									   {
										   if( id == 99)
										   {
											   // online notify
											   var friend = ss[1];
											   var online = ss[2] == "1";
											   var msg = new ChatEntry(friend, online ? " has come online" : " has gone offline", ServerTime(), ChatEntry.typeAnnounce);
											   App.DispatchOnUIThreadLow(() =>
											  {
												  ChatTab.alliance.Post(msg,true);
												  ChatTab.world.Post(msg, true);
											  }); // post on both
										   }
										   else if( id == 9)
										   {
											   var cid = int.Parse(ss[1]);
											   // founded new city
											   await Task.Delay(30000);//.ConfigureAwait(false);
											   Note.Show($"You have founded a new city!  Would you like to run [Setup](/s/{cid.CidToString()})");

										   }
									   }

								   }
								   break;
							   }
						   case "incoming":
							   {
								   App.QueueIdleTask(IncomingOverview.ProcessTask, 1000);
								   break;
							   }
						   case "outgoing":
							  {
								   App.QueueIdleTask(OutgoingOverview.ProcessTask, 1000);
								   break;
							  }
						   case "gstcb":
							   {
								   Note.Show(jsp.ToString());
								   var jso = jsp.Value;
								   var tag = jso.GetAsInt("tag");
								   if (gstCBs.TryGetValue(tag, out var cb))
								   {
									   cb(jso);
								   }

								   break;
							   }
						   case "rmp":
							   {
								   var str = jsp.ToString();
								   App.CopyTextToClipboard(str);
									Note.Show(str);
								   foreach(var o in jsp.Value.EnumerateObject())
								   {
									   foreach(var st in o.Value.EnumerateArray())
									   {
										   TileData.UpdateTile(st.GetAsString());
									   }
								   }
								   break;
							   }

						   case "gstempty":
							   {
								   var jso = jsp.Value;
								   var water = jso.GetAsInt("water") == 1;
								   var res = jso.GetAsString("res").Split('^', StringSplitOptions.RemoveEmptyEntries);
								   var cid = jso.GetAsInt("cid");

								   var food = float.Parse(res[3]);
								   var wood = float.Parse(res[0]);
								   var stone = float.Parse(res[1]);
								   var iron = float.Parse(res[2]);
								   var sum = (wood + stone + iron + food);
								   (var x, var y) = cid.CidToWorld();
								   float woodCount = 10, stoneCount = 10, ironCount = 10, plainsCount = 2;
								   TileData.instance.ResourceGain(x, y + 1, false, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
								   TileData.instance.ResourceGain(x - 1, y, false, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
								   TileData.instance.ResourceGain(x, y - 1, false, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
								   TileData.instance.ResourceGain(x + 1, y, false, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
								   TileData.instance.ResourceGain(x + 1, y + 1, true, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
								   TileData.instance.ResourceGain(x - 1, y + 1, true, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
								   TileData.instance.ResourceGain(x - 1, y - 1, true, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
								   TileData.instance.ResourceGain(x + 1, y - 1, true, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
								   //if (!App.IsKeyPressedShift())
								   //{
								   //    woodCount = woodCount.Min(30);
								   //    stoneCount = stoneCount.Min(30);
								   //    ironCount = ironCount.Min(30);
								   //}

								   var totalRes = woodCount + stoneCount + ironCount + plainsCount;
								   var iWood = ((int)(woodCount * 80.0 / totalRes)).Min(30);
								   var iStone = ((int)(stoneCount * 80.0 / totalRes)).Min(30);
								   var iIron = ((int)(ironCount * 80.0 / totalRes)).Min(30);
								   var maxDelta = (iWood - wood).Abs().Max((iStone - stone).Abs()).Max((iIron - iIron).Abs()).Max((food - plainsCount).Abs());
								   var predicted = iWood + iStone + iIron + plainsCount;
								   if (iWood - wood >= 1.0f)
								   {
									   Note.Show($"{predicted} predicted, {sum} actual, {iWood}:{wood} {iStone}:{stone} {iIron}:{iron} {plainsCount}:{food}");
									   SpotTab.TouchSpot(cid, VirtualKeyModifiers.None, false, true);
								   }
								   //  var nodes = (nodeCount+30)/(nodeCount+30+2+ plainsCount)*80 + 2+ plainsCount;
								   break;
							   }
						   case "cityclick":
							   {
								   var jso = jsp.Value;
								   var cid = jso.GetAsInt("cid");
								   {
									   var pid = Player.NameToId(jso.GetAsString("player"));
									   var city = Spot.GetOrAdd(cid);
									   var name = jso.GetString("name");
									   city.pid = pid; // todo: this shoule be an int playerId
									   city.type = jso.GetAsByte("type");
									   city.remarks = jso.GetAsString("notes");                //Assert(city.pid > 0);
									   city.UpdateTags();
									   city.points = (ushort)jso.GetAsInt("score");
									   //   city.alliance = jso.GetString("alliance"); // todo:  this should be an into alliance id
									   //       city.lastAccessed = DateTimeOffset.UtcNow;
									   // city.isCastle = jso.GetAsInt("castle") == 1;
									   var blessed = city.pid > 0 ? jso.GetAsInt("bless") > 0 : false;
									   if (blessed != city.isBlessed)
									   {
										   city.isBlessed = blessed;
										   city.OnPropertyChanged(nameof(City.icon));
									   }
									   city.isOnWater |= jso.GetAsInt("water") != 0;  // Use Or in case the data is imcomplete or missing, in which case we get it from world data, if that is not incomplete or missing ;)
									   city.isTemple = jso.GetAsInt("plvl") != 0;

									   //if(City.focus != cid)
										  // cid.BringCidIntoWorldView(true,false);
									   if (city._cityName != name)
									   {
										   city._cityName = name;
										   if (cid == Spot.focus)
											   App.DispatchOnUIThreadLow(() => ShellPage.instance.focus.Content = city.nameAndRemarks);
									   }
									   if(City.focus != cid)
										   city.SetFocus(true);
									   if (city.isNotClassified )
									   {
										   if (App.IsKeyPressedControl() && (Alliance.wantsIntel || Player.isAvatarOrTest) )
										   {
											   city.Classify();
										   }
									   }
								   }
							
								   break;

							   }


						   case "ext":
							   {
								   extCityHack = jsDoc;
								   break;
							   }


						   case "citydata":
							   {
								   try
								   {
									   var jse = jsp.Value;
									   // var priorCid = cid;
									   var cid = jse.GetInt("cid");
									   //if (!ShellPage.IsWorldView())
									   // AGame.cameraC = cid.CidToWorldV();
									   var isFromTs = jse.TryGetProperty("ts", out _);
									   //Note.L("citydata=" + cid.CidToString());
									   var city = City.GetOrAddCity(cid);
									   city.LoadCityData(jse);

									   // If it does not include TS it is from a call to chcity
									   // Otherwise is is from a change in TS

									   if (!isFromTs)
									   {
										   //		if (cid != City.build)
										   //		   city.SetBuild(false);
									   }
									   if (isFromTs && cid == DungeonView.openCity && DungeonView.IsVisible())
									   {
										   //   if (jse.TryGetProperty("ts", out _))
										   //  {
										   ScanDungeons.Post(cid, city.commandSlots == 0, false);  // if command slots is 0, something was not send correctly
																								   //  }
									   }
									   NavStack.Push(cid);
									   if (waitingOnCityData.Length > 0)
									   {
										   bool allDone = true;
										   foreach (var i in waitingOnCityData)
										   {
											   if (i.cid == cid)
											   {
												   i.Done();
											   }
											   allDone &= i.isDone;
										   }
										   if (allDone)
										   {
											   waitingOnCityData = Array.Empty<WaitOnCityDataData>();
										   }
									   }
								   }catch(Exception ex)
								   {
									   LogEx(ex);
								   }
								   finally
								   {

								   }
								   
								   
								   break;

							   }
						   case "OGA":
							   {
								   Log("OGA" + eValue.ToString());
								   break;
							   }
						   case "OGR":
							   {
								   //  Log(e.Value);
								   break;
							   }
						   case "snd":
							   {
								   City.UpdateSenatorInfo();
								   break;
							   }
						   case "OGT":
							   {
								   // Log(e.Value);
								   break;
							   }
						   case "aldt":
							   {
								   Log("Aldt");
								   Alliance.Ctor(jsDoc);
								   
								   // now we can update player info
								   //Cosmos.PublishPlayerInfo(jsBase.pid, City.build, jsBase.token, jsBase.cookies);

								  
								   break;
							   }
						   case "gPlA":
							   {
								   Player.Ctor(jsp.Value);
								   while( !ppdtInitialized || !Alliance.diplomacyFetched)
								   {
									   await Task.Delay(500).ConfigureAwait(false);
								   }
								   //if (Player.isAvatarOrTest)
								   //{
									  // App.DispatchOnUIThreadLow(() =>
									  // {
									  // // create a timer for precense updates
									  // presenceTimer = new DispatcherTimer();
										 //  presenceTimer.Interval = TimeSpan.FromSeconds(16);
										 //  presenceTimer.Tick += PresenceTimer_Tick; ;
										 //  presenceTimer.Start();
									  // // Seed it off

								   //});
									  // PresenceTimer_Tick(null, null); // seed it off, but only after our token has time to have been set
								   //}
								   break;
							   }
						   // city lists
						   case "ppdt":
							   {
									var jse = jsp.Value;
								   UpdatePPDT(jse,jse.TryGetProperty("pid", out var _pid) ? _pid.GetAsInt() : Player.activeId);
								   break;
							   }
						   case "chat":
							   {
								   App.DispatchOnUIThreadLow(() => ChatTab.ProcessIncomingChat(jsp));

								   break;
							   }
						   case "chatin":
							   App.DispatchOnUIThreadLow(() => ChatTab.PasteToChatInput(jsp.Value.GetString()));
							   break;
						   case "copyclip":
							   {
								   App.CopyTextToClipboard(jsp.Value.GetAsString());
								   break;
							   }
						   case "cmd":
							   {
								   var str = jsp.Value.GetAsString();
								   OpenAttackSender(str);
								   break;
							   }
						   case "setglobals":
							   {
								   Assert(false);
								   //var jso = jsp.Value;
								   //var raidSecret = jso.GetString("secret");
								   //var pid = jso.GetInt("pid");

								   //pendingCookies = null;
								   //var pn = jso.GetString("pn");
								   //var ppdt = jso.GetProperty("ppdt");
								   //var token = jso.GetString("token");
								   //var s = jso.GetString("s");
								   //var cid = jso.GetAsInt("cid");
								   //AddPlayer(false, true, pid, pn, token, raidSecret, s, ppdt.ToString());

								   //var city = City.GetOrAdd(cid);
								   //// If they are visiting somene elses city we don't want to be directed there
								   //// so we go to the default city
								   //UpdatePPDT(ppdt, pid,updateBuildCity:(city.pid != pid) ); 
								   
								   //if (city.pid == pid) // we want ot visit a specific city
								   //{
									  //CitySwitch(cid,true);
								   //}
								  
								   break;
							   }
						   case "restoreglobals":
						   {
								   Assert(false);
								   //Note.Show("Cookies failed, maybe they need to log in again to refresh cookies?");
								   //// only need to restore cookies
								   //CookieDB.Apply(jsVars.cookies);
								   //pendingCookies = null;

								   //App.DispatchOnUIThreadLow(() => ShellPage.instance.friendListBox.SelectedItem = Player.activePlayerName);

								   break;
						   }
						   case "c":
							   {

								   var jso = jsp.Value;
								   var popupCount = jso.GetAsInt("p");
								   //     Note.L("cid=" + cid.CidToString());
								   if(ppdtInitialized && jso.TryGetProperty("v", out var v))
								   {
									   var vm = (ShellPage.ViewMode)v.GetAsInt();
									   switch (vm)
									   {
										   case ShellPage.ViewMode.city:
											   AGame.cameraZoom = AGame.cityZoomDefault;
											   break;
										   case ShellPage.ViewMode.region:
											   AGame.cameraZoom = AGame.cameraZoomRegionDefault;
											   break;
										   case ShellPage.ViewMode.world:
											   AGame.cameraZoom = AGame.cameraZoomWorldDefault;

											   break;
									   }
									   City.build.BringCidIntoWorldView(false,false);
									   ShellPage.AutoSwitchViewMode();
								   }

								   //   ShellPage.SetViewMode((ShellPage.ViewMode)jso.GetInt("v"));
								   if(jso.TryGetProperty("pop", out var pop))
								   {
									   var str = pop.ToString();

									   var popup = System.Text.Json.JsonSerializer.Deserialize<Models.JSPopupNode[]>(str, Json.jsonSerializerOptions);
									   Log(popup.Length.ToString() );
									   // App.DispatchOnUIThreadLow(() => Models.JSPopupNode.Show(popup));
									   Models.JSPopupNode.Show(popup);

								   }
								 //  ShellPage.NotifyCotgPopup(popupCount);
								   //                                ShellPage.SetCanvasVisibility(noPopup);
								   if (ppdtInitialized && jso.TryGetProperty("c", out var _cid))
								   {
									   // this should be rare, sometimes the JS city is out of sync with the registered city
									  // Assert(false);
									   var cid = _cid.GetAsInt();
									   if (cid != City.build)
									   {
										   CitySwitch(cid, true, false, false, false, false);
									   }
								   }
								   break;
							   }

							   //case "stable":
							   //    {
							   //        var jse = jsp.Value;
							   //        int counter = 0;
							   //        StringBuilder sb = new StringBuilder();
							   //        foreach (var i in jse.EnumerateArray())
							   //        {
							   //            sb.Append('"');

							   //            sb.Append(HttpUtility.JavaScriptStringEncode(i.GetString()));
							   //            sb.Append("\" /* " + counter++ + " */,"); 

							   //        }
							   //        var s = sb.ToString();
							   //        Log(s);
							   //        break;

							   //    }
							   //    break;
					   }

				   }

				   if (gotCreds)
				   {
					   AGame.InitializeForWorld();

				   }
			   }
			   //}


			   // }
			   //var cookie = httpClient.DefaultRequestHeaders.Cookie;
			   //cookie.Clear();
			   //foreach (var c in jsVars.cookie.Split(";"))
			   //{
			   //    cookie.ParseAdd(c);
			   //}






			   catch (Exception ex)
			   {

				   LogEx(ex);
			   }
		   });
		}

		

		
		//private static async void PresenceTimer_Tick(object sender, object e)
		//{
		//	/*
		//	var players = await Cosmos.GetPlayersInfo();
		//	var changed = false;
		//	int put = 0;
		//	int validCount = 0;
		//	foreach (var _p in players)
		//	{
		//		var pid = int.Parse(_p.id);
		//		if (pid == Player.myId || Friend.all.Any(a =>a.pid==pid) || Player.isAvatarOrTest )
		//			++validCount;
		//	}
		//	var presence = new PlayerPresence[validCount];
		//	foreach (var _p in players)
		//	{
		//		var p = new PlayerPresence(_p);
		//		int priorCid;
		//		var pid = p.pid;
		//		if (!(pid == Player.myId || Friend.all.Any(a => a.pid == pid)||Player.isAvatarOrTest))
		//			continue;

		//		var priorIndex = PlayerPresence.all.IndexOf( ( a) => a.pid == pid );
		//		if (priorIndex == -1)
		//		{
		//			changed = true;
		//			priorCid = 0;
		//		}
		//		else
		//		{
		//			if (PlayerPresence.all[priorIndex].token != p.token)
		//				changed = true; // need to refresh token
		//			priorCid = PlayerPresence.all[priorIndex].cid;
		//		}
				
		//	//	Player.myIds.Add(pid);
		//	// TODO:  restore this functionality when it works again
		//		if (pid != Player.myId)
		//		{
		//			if (p.cid != priorCid)
		//			{
		//				if (p.cid == City.build && priorCid != City.build)
		//					Note.Show($"{p.name } has joined you in {p.cid.CidToStringMD()}");
		//				if (p.cid != City.build && priorCid == City.build)
		//					Note.Show($"{p.name } has left {p.cid.CidToStringMD()}");

		//			}
		//		}
		//		presence[put++] = p;
			
		//	}
		//	PlayerPresence.all = presence;

		//	if(changed)
		//	{
		//		App.(() =>
		//		{
		//			// Update menu
		//			ShellPage.instance.friendListBox.SelectedIndex = -1;
		//			ShellPage.instance.friendListBox.Items.Clear();
		//			int counter = 0;
		//			int sel = -1;
		//			foreach (var p in PlayerPresence.all)
		//			{
		//				ShellPage.instance.friendListBox.Items.Add(p.name);
		//				if (p.pid == Player.activeId)
		//					sel = counter;
		//				++counter;
		//				// reset menu, TOTO:  Keep track of active selection
		//			}

		//			ShellPage.instance.friendListBox.SelectedIndex = sel;
		//			ShellPage.instance.friendListBox.Visibility = PlayerPresence.all.Length > 1 ? Visibility.Visible : Visibility.Collapsed;
		//		});
		//	}
		//	*/
		//}

		
		static private void View_UnsafeContentWarningDisplaying(WebView2 sender, object args)
		{
			Exception("Unsafe");
		}

		public static TimeSpan gameTOffset;
	//	public static long gameTOffsetMs;
		public static int gameTOffsetSeconds;
		internal static HttpClient httpClient;

		//        public static async void TestGet()
		//        {
		//            Log("TestGet");
		//            //     using var req = new HttpRequestMessage(HttpMethod.Post, new Uri( new Uri($"https://w{world}.crownofthegods.com"), "poll2.php"));

		//            //            AddDefaultHeaders(req.Headers);


		//            try
		//            {
		//                var url =
		//                       "world=&" +
		//                       "cid=17367265&" +
		//                       "ai=0&" +
		//                       $"ss={jsVars.s}";

		//                //            using var req  =anyPost;
		//                var req = new HttpRequestMessage(HttpMethod.Post, new Uri(httpsHost, @"includes/poll2.php"));
		//               // req.TransportInformation.ver
		//                //req.AllowAutoRedirect = true;
		//                req.Content = new HttpStringContent(url,

		//                                                        Windows.Storage.Streams.UnicodeEncoding.Utf8,

		//                                                        "application/x-www-form-urlencoded");//CONTENT-TYPE header UrlEncodeToBytes( url,  );
		//                                                                                             //    req.Headers.TryAppendWithoutValidation("Content-Encoding", jsVars.token);
		//                                                                                             //       req.Headers.Accept.TryParseAdd(@"application/json");
		//                                                                                             //            req.Headers.Accept.TryParseAdd(@"*/*");

		//  //              var value = new Windows.Networking.HostName(req.RequestUri.Host);
		// //               req.Headers.Host = value;
		//                //if (anyPost != null)
		//                //    foreach (var h in anyPost.Headers)
		//                //    {
		//                //        req.Headers.TryAdd(h.Key, h.Value);
		//                //    }


		//                //            req.Headers.TryAppendWithoutValidation("Content-Encoding", jsVars.token);

		//                //            req.Content.Headers.ContentType.CharSet = "UTF-8";

		//                req.Content.Headers.TryAppendWithoutValidation("Content-Encoding", jsVars.token);

		//                var resp = await httpClient.SendRequestAsync(req, HttpCompletionOption.ResponseContentRead);
		//                Log($"Error: {resp.StatusCode}");
		//                Log($"Error: {resp.RequestMessage.ToString()}");
		//                if (resp.IsSuccessStatusCode)
		//                {
		//                    var b = await resp.Content.ReadAsStringAsync();

		////                    jso = await JsonDocument.ParseAsync(b.ToString);

		//                    Log(b.ToString());
		//                }
		//                else
		//                {

		//                };
		//            }
		//            catch (Exception e)
		//            {
		//                Log(e);
		//            }




		//        }

	}
}
