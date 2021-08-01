using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Web.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static COTG.Debug;
using Windows.Web.Http.Filters;
using Windows.UI.Xaml;
using Windows.System;
using System.Text.Json;
using COTG.Game;
using System.Threading;
using COTG.Helpers;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Concurrent;
using Windows.Storage.Streams;
using COTG.Services;
using COTG.Views;
using System.Numerics;
using COTG.JSON;
using static COTG.Game.Enum;
using Windows.UI.Input;
using Microsoft.Graphics.Canvas;
using Windows.Graphics.Imaging;
using System.Text.Json.Serialization;
using COTG.DB;
using Microsoft.AppCenter;
using WebView = Windows.UI.Xaml.Controls.WebView;
using ContentDialog = Windows.UI.Xaml.Controls.ContentDialog;
using ContentDialogResult = Windows.UI.Xaml.Controls.ContentDialogResult;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Text;
using System.Web;
using Windows.Security.Cryptography.Certificates;
using Windows.Foundation;
using Windows.Web.Http.Headers;
using DiscordCnV;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI;

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
		public static string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.114 Safari/537.36 Edg/91.0.864.54";
		//        public static JsonDocument ppdt;
		public static JSClient instance = new JSClient();
		public static WebView view;
		//public static WebViewBrush webViewBrush; 
		public static HttpBaseProtocolFilter httpFilter;
		public static HttpCookieManager cookieManager;
		const int clientCount = 6;
		public static ConcurrentBag<HttpClient> clientPool;
		public static SemaphoreSlim clientPoolSema = new SemaphoreSlim(clientCount);
		static HttpClient _downloadImageClient;
		static bool hasCouncilors;
		static bool councillorsChecked;
		public static int spanX;
		public static int spanY;
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

		public static void JSInvoke(string func, string[] args)
		{
			App.DispatchOnUIThreadSneaky(async () =>
			{

				try
				{
					await JSClient.view.InvokeScriptAsync(func, args);

				}
				catch (Exception ex)
				{
					LogEx(ex);

				} }
);
		}


		public static async Task<string> JSInvokeTask(string func, string[] args)
		{

			// this won't await the actually js call
			return await App.DispatchOnUIThreadTask(async () =>
		  await JSClient.view.InvokeScriptAsync(func, args));


		}
		public static HttpClient _genericClient;
		public static HttpClient genericClient
		{
			get
			{
				if (_genericClient == null)
				{
					_genericClient = new HttpClient();

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

		public static int ppss;
		public static bool isSub => subId != 0;

		public static long GameTimeMs()
		{
			return (long)((DateTimeOffset.UtcNow - launchTime).TotalMilliseconds) + gameMSAtStart;
		}
		public static DateTimeOffset ServerTime()
		{
			return (DateTimeOffset.UtcNow + gameTOffset);
		}
		public static int ServerTimeSeconds()
		{
			return (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + gameTOffsetSeconds);
		}
		// timestamp - ServerTime all in in MS 
		public static int ServerTimeOffsetMs(long t)
		{
			return (int)(t - ServerTimeMs());
		}
		public static long ServerTimeMs()
		{
			return (ServerTime() - SmallTime.t0).TotalMilliseconds.RoundToLong();
		}

		public static DateTimeOffset ServerToLocal(DateTimeOffset t)
		{
			return t.ToUniversalTime() - gameTOffset;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="JSClient"/> class.
		/// </summary>
		public JSClient()
		{
		}

		public static void SetPlayer(int pid, int cid)
		{
			foreach (var p in PlayerPresence.all)
			{
				if (p.pid == pid)
				{

					SetPlayer(pid, p.token, p.cookies, cid, p.name);
					return;
				}
			}
			Debug.Log("Missing player");
		}

		//public static string GetSecSessionId()
		//{
		//	var cookies = cookieManager.GetCookies(new Uri("https://crownofthegods.com"));
		//	foreach (var cookie in cookies)
		//	{
		//		if (cookie.Name == "sec_session_id")
		//			return cookie.Value;


		//	}
		//	return null; // error!
		//}
		static string pendingCookies;
		public static async void SetPlayer(int pid, string token, string cookies, int cid, string name)
		{
			// already set
			if (jsVars.token == token)
				return;
			if (pendingCookies != null)
				return;
			pendingCookies = cookies;

			Note.Show($"Entering {name}'s City");


			Log($"ChangePlayer:{name}");
			{
				var _cookies = cookieManager.GetCookies(new Uri("https://crownofthegods.com"));
				foreach (var c in _cookies)
				{
					Log($"{c.Name} {c.Domain} {c.Path} {c.Value}");
				}
			}

			var secSessionId = CookieDB.Apply(cookies);


			{
				var _cookies = cookieManager.GetCookies(new Uri("https://crownofthegods.com"));
				foreach (var c in _cookies)
				{
					Log($"{c.Name} {c.Domain} {c.Path} {c.Value} {c.Secure} {c.HttpOnly}");
				}
			}
			//	AddPlayer(false, true, pid, Player.all[pid].name, token, "", secSessionId, null);
			//	await GetCity.Post(cid, (jse,city) => Log($"{jse.ToString()} Here!!") );
			App.DispatchOnUIThreadSneaky(() => view.InvokeScriptAsync("setPlayerGlobals", new[] { token, secSessionId, cid.ToString() }));
		}

		public static void SetCookieCollab(string name, string value, bool session, bool httpOnly, bool clearOnly = false)
		{
			var cookie = new HttpCookie(name, ".crownofthegods.com", "/");
			//		var remember = new HttpCookie("remember_me", ".crownofthegods.com", "/");
			if (httpOnly)
			{
				cookie.Secure = true;
				cookie.HttpOnly = true;
			}

			if (!session)
			{
				cookie.Expires = DateTimeOffset.UtcNow + TimeSpan.FromDays(7);
			}
			cookieManager.DeleteCookie(cookie);
			if (!clearOnly)
			{
				cookie.Value = value;
				cookieManager.SetCookie(cookie);
			}
		}

		public static void AddPlayer(bool isMe, bool setCurrent, int pid, string pn, string token, string raid, string cookies, string ppdt)
		{
			var jsv = new JSVars() { token = token, pn = pn, pid = pid, ppdt = ppdt, cookies = cookies, raidSecret = raid }; // todo: need raidSecret
																															 //
																															 // add if necessary
																															 //
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
				jsBase = jsv;
				jsVars = jsv;
				Player.activeId = pid;
			}
			else if (setCurrent)
			{
				//	Player.myName = pn;
				Player.activeId = pid;
				jsVars = jsv;

				App.DispatchOnUIThreadSneakyLow(() => ShellPage.instance.friendListBox.SelectedItem = pn);

			}

		}
		public static void SetSessionCookie()
		{
			if(!SettingsPage.secSessionId.IsNullOrEmpty() && !JSClient.isSub  )
			{
				SetCookie("sec_session_id", SettingsPage.secSessionId);
			}
		}

		public static void SetCookie(string name, string value, string domain = ".crownofthegods.com", string path = "/" )
		{
			var cookie = new HttpCookie(name, domain, path);
			//		var remember = new HttpCookie("remember_me", ".crownofthegods.com", "/");
			//if (httpOnly)
			{
				cookie.Secure = true;
				cookie.HttpOnly = true;
			}

			//if (!session)
			{
				cookie.Expires = DateTimeOffset.UtcNow + TimeSpan.FromDays(200);
			}
			JSClient.cookieManager.DeleteCookie(cookie);
			//if (!clearOnly)
			if(!value.IsNullOrEmpty())
			{
				cookie.Value = value;
				JSClient.cookieManager.SetCookie(cookie);
			}


		}
		//	static string secSessionId;

		internal static WebView Initialize(Windows.UI.Xaml.Controls.Grid panel)
		{
			httpFilter = new HttpBaseProtocolFilter();
			httpFilter.AutomaticDecompression = true;

			httpFilter.AllowAutoRedirect = true;
		//	httpFilter.UseProxy = false;

			httpFilter.MaxVersion = HttpVersion.Http20;
			httpFilter.ServerCustomValidationRequested += ServerCustomValidationRequested;
			//httpFilter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
			//httpFilter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);

			httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.WrongUsage);
			httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);
			httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Untrusted);
			httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.IncompleteChain);
			httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.RevocationInformationMissing);
			httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.RevocationFailure);

			httpFilter.AllowUI = true;
			httpFilter.CookieUsageBehavior = HttpCookieUsageBehavior.Default;


			//	  HttpBaseProtocolFilter.CreateForUser( User.GetDefault());
			//                         httpFilter.ServerCredential =

			httpFilter.MaxConnectionsPerServer = 10;
			//  httpFilter.ServerCustomValidationRequested += HttpFilter_ServerCustomValidationRequested;
			httpFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.NoCache;
			httpFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
			

			cookieManager = httpFilter.CookieManager;
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
			try
			{


				view = new WebView(Windows.UI.Xaml.Controls.WebViewExecutionMode.SeparateThread)
				{
					//HorizontalAlignment = HorizontalAlignment.Stretch,
					//VerticalAlignment = VerticalAlignment.Stretch,
					//CacheMode=new BitmapCache()
					DefaultBackgroundColor = new Windows.UI.Color() { G = 0, B = 0, R = 0, A = 0 },

					Name = "cotgView",
					//Opacity = 0.5,

				};

				view.EffectiveViewportChanged += View_EffectiveViewportChanged;

				//	view.AddHandler(WebView.KeyDownEvent, new KeyEventHandler(webViewKeyDownHandler), true);
				//	view.AddHandler(WebView.PointerPressedEvent, new PointerEventHandler(pointerEventHandler), true);
				view.UnsafeContentWarningDisplaying += View_UnsafeContentWarningDisplaying;
				view.UnsupportedUriSchemeIdentified += View_UnsupportedUriSchemeIdentified;

				view.UnviewableContentIdentified += View_UnviewableContentIdentified;
				view.ScriptNotify += View_ScriptNotify;
				view.DOMContentLoaded += View_DOMContentLoaded;
				view.NavigationFailed += View_NavigationFailed;
				view.NavigationStarting += View_NavigationStarting;
				view.NavigationCompleted += View_NavigationCompletedAsync;
				view.PermissionRequested += View_PermissionRequested;
				view.NewWindowRequested += View_NewWindowRequested;
				//view.WebResourceRequested += View_WebResourceRequested1;
				//	view.WebResourceRequested -= View_WebResourceRequested1;
				//	view.WebResourceRequested += View_WebResourceRequested1;
				//  view.WebResourceRequested += View_WebResourceRequested1;
				//	webViewBrush = new WebViewBrush() { Stretch = Stretch.Fill };

				//   view.CacheMode = CacheMode.
				//Grid.Se SetAlignLeftWithPanel(view, true);
				//RelativePanel.SetAlignRightWithPanel(view, true);
				///	RelativePanel.SetAlignTopWithPanel(view, true);
				//		RelativePanel.SetAlignBottomWithPanel(view, true);
				if (isSub)
				{
					httpsHost = new Uri($"https://w{world}.crownofthegods.com");
					//       view.Source = new Uri($"https://w{world}.crownofthegods.com?s={subId}");
				}
				// else
				view.Source = new Uri("https://www.crownofthegods.com/home");
				if (isSub)
				{
					Task.Delay(5000).ContinueWith(_ =>
					{
						App.DispatchOnUIThread(() => view.Source = new Uri($"https://w{world}.crownofthegods.com?s=1"));
					});
				}
				else
				{
					
				}
				//	App.SetupCoreWindowInputHooks();

			}
			catch (Exception e)
			{
				LogEx(e);
			}

			return view;



		}
		public static void ClearAllCookies(string domain= "https://crownofthegods.com")
		{
			var _cookies = cookieManager.GetCookies(new Uri(domain));
			foreach (var c in _cookies)
			{
				cookieManager.DeleteCookie(c);
			}

		}
		private static void View_EffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
		{
			var scrollView = sender as ScrollViewer;
			if (scrollView != null)
			{
				Log(args);
			}
			Log(sender);
		}

		

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
		static async Task<CanvasBitmap> CreateAplhaMaskFromBitmap(IRandomAccessStream source, ICanvasResourceCreator canvas)
		{

			BitmapDecoder decoder = await BitmapDecoder.CreateAsync(source);
			var transform = new BitmapTransform();
			transform.ScaledHeight = decoder.PixelHeight / 4;
			transform.ScaledWidth = decoder.PixelWidth / 4;
			PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
				BitmapPixelFormat.Bgra8,
				BitmapAlphaMode.Straight,
				transform,
				ExifOrientationMode.IgnoreExifOrientation,
				ColorManagementMode.DoNotColorManage);
			Log(decoder, decoder.BitmapPixelFormat.ToString());
			Log(decoder, decoder.BitmapAlphaMode.ToString());
			// no extract alpha
			var pixels = pixelData.DetachPixelData();
			var size = transform.ScaledWidth * transform.ScaledHeight;
			var alphas = new byte[size];
			int other = 0;
			for (int i = 0; i < size; ++i)
			{
				if (pixels[i * 4 + 3] != 0)
					++other;
				pixels[i * 4 + 3] = (byte)(255 - pixels[i * 4 + 3]);
			}
			Log(other);
			return CanvasBitmap.CreateFromBytes(canvas, pixels, (int)transform.ScaledWidth, (int)transform.ScaledHeight, Windows.Graphics.DirectX.DirectXPixelFormat.R8G8B8A8UIntNormalized);

		}

		//private static void pointerEventHandler(object sender, PointerRoutedEventArgs e)
		//{
		//	Note.Show("Pointer " + e.GetCurrentPoint(sender as UIElement).Properties.PointerUpdateKind + e.KeyModifiers + e.ToString());
		//}

		//private static void webViewKeyDownHandler(object sender, KeyRoutedEventArgs e)
		//{
		//	Note.Show("Key " + e.Key + e.ToString());
		//}

		async private static void View_NewWindowRequested(WebView sender, Windows.UI.Xaml.Controls.WebViewNewWindowRequestedEventArgs args)
		{
			args.Handled = true;
			//if (WebViewPage.instance != null)
			//{
			//    WebViewPage.instance.Focus(FocusState.Programmatic);
			//    return;
			//}

			Trace(args.Uri.ToString());
			Trace(args.Uri.Host);
			//          Trace(httpsHost.Host);
			if ((httpsHost != null && args.Uri.Host == httpsHost.Host))
			{
				Log(args.Uri.ToString());
				if (App.IsKeyPressedShift())
				{

					Launcher.LaunchUriAsync(args.Uri, new LauncherOptions() { DisplayApplicationPicker = true });
				}
				else
				{
					WebViewPage.DefaultUrl = args.Uri;
					await WindowManagerService.Current.TryShowAsStandaloneAsync<WebViewPage>("overview");
				}
			}
			else if (args.Uri.OriginalString.StartsWith("https://accounts.google.com/o/oauth2/auth?"))
			{
				WebViewPage.post = args.Uri;
				await WindowManagerService.Current.TryShowAsStandaloneAsync<WebViewPage>("login");
			}
			//			else if (httpsHost != null && args.)
			else
			{
				Launcher.LaunchUriAsync(args.Uri);
			}
		}

		private static string GetJsString(string asm)
		{
			return new StreamReader((typeof(JSClient).Assembly).GetManifestResourceStream($"COTG.JS.{asm}")).ReadToEnd();

		}
		//static async void GetSessionSoon()
		//{
		//	await Task.Delay(2000);
		//	SettingsPage.secSessionId= GetSecSessionId();
		//	view.Source = new Uri("https://www.crownofthegods.com/home");


		//}
		private static void View_WebResourceRequested1(WebView sender, Windows.UI.Xaml.Controls.WebViewWebResourceRequestedEventArgs args)
		{

			try
			{
				var req = args.Request;
				var str = req.RequestUri.ToString();
				if(str.EndsWith("https://www.crownofthegods.com/nincludes/pro_log.php"))
				{
					//sender.NavigateWithHttpRequestMessage(args.Request);
				//	GetMe(args,args.GetDeferral());
				//	GetSessionSoon();
				}
				//	Log(req.RequestUri.ToString());
				else if (str.EndsWith("jquery/1.9.0/jquery.min.js"))
				{
					////	var js = GetJsString("jquery");
					//var js = GetJsString("jquery3_5_1") + GetJsString("jquerymigrate");// + GetJsString("jquerymigrate3_3_2");
					//var newContent = new Windows.Web.Http.HttpStringContent(js, Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/json");

					//args.Response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = newContent };

				}
				//else if (str == "https://www.crownofthegods.com/index.php")
				//{

				//	//	var js = GetJsString("jquery");
				//	var js = GetJsString("index.htm");// + GetJsString("jquerymigrate3_3_2");
				//	var newContent = new Windows.Web.Http.HttpStringContent(js, Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/json");

				//	args.Response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = newContent };
				//}
				//else if (req.RequestUri.ToString().Contains("alasstylesheet.css"))
				//{
				//	if (SettingsPage.IsThemeWinter())
				//	{
				//		var js = GetJsString("alasstylesheet.css");

				//		var newContent = new Windows.Web.Http.HttpStringContent(js, Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/css");

				//		args.Response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = newContent };
				//	}
				//}
				//else if (req.RequestUri.ToString().Contains("index.html"))
				//{
				//    Assert(false);
				//    var js = GetJsString("jquery");
				//    args.de

				//    var newContent = new Windows.Web.Http.HttpStringContent(js, Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/json");

				//    args.Response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = newContent };

				//}
				else if (str.Contains("/jsfunctions/phaser.js"))
				{
					//   var js = GetJsString("phaser");

					var newContent = new Windows.Web.Http.HttpStringContent("", Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/json");

					args.Response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = newContent };

				}
				else if (str.Contains("/jsfunctions/pack.js"))
				{
					//var js = GetJsString("pack.js");

					//var newContent = new Windows.Web.Http.HttpStringContent(js, Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/json");

					//args.Response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = newContent };

				}
				//else if (req.RequestUri.LocalPath.Contains("building_set5"))
				//{
				//	int q = 0;
				//	var data = TitleContainer.OpenStream("Art/buildingset5");

				//	args.Response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = new HttpBufferContent( data) };

				//}

				else if (str.Contains("jsfunctions/game.js"))
				{
					try
					{

						view.WebResourceRequested -= View_WebResourceRequested1;
						string host = args.Request.RequestUri.Host;
						string uri = args.Request.RequestUri.AbsoluteUri;

						//   var reqMsg = args.Request;
						//   var respTask = httpClient.SendRequestAsync(reqMsg).AsTask();

						var asm = typeof(JSClient).Assembly;
						var js = "const cityAtlas = " +
						  (SettingsPage.IsThemeWinter() ?
						  		"'ms-appx-web:///Content/Art/City/Winter/building_set5.png'\n" :
							   "'/images/city128/building_set5.png'\n") +

								 GetJsString("funky")
							+ GetJsString("DHRUVCC.js")
							   + GetJsString("J0EE");
						var newContent = new Windows.Web.Http.HttpStringContent(js, Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/json");

						args.Response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = newContent };
						// args.Response = resp;
						//var response = await client.SendRequestAsync(reqMsg).AsTask();
						// resp.Content = newContent;
						//    }




					}
					catch (Exception e)
					{
						LogEx(e);
					}

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
			App.DispatchOnUIThreadSneaky(() => view.InvokeScriptAsync("postMouseEvent", new string[] { x.ToString(), y.ToString(), eventName, button.ToString(), dx.ToString(), dy.ToString() }));
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
			App.DispatchOnUIThreadSneakyLow(() => view.InvokeScriptAsync("setStayAlive", new string[] { stayAlive ? "1" : "" }));
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
					view.InvokeScriptAsync("sendchat", new string[] { channel.ToString(), message });
					
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

		//            view.InvokeScriptAsync("viewcity", new string[] { (cityId).ToString() });
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

		public static async Task<bool> CitySwitch(int cityId, bool lazyMove = false, bool select = true, bool scrollIntoUI = true, bool isLocked = false, bool waitOnChange = false)
		{
			// Make sure we don't ignore the exception
			{

				if (City.CanVisit(cityId))
				{
					if (!Spot.CanChangeCity(cityId))
					{
						ShellPage.EnsureNotCityView();
						Note.Show("Please wait for current operation to complete");
						return false;
					}
					var city = City.GetOrAddCity(cityId);

					if (city.pid != Player.activeId)
					{
						Assert(false);
						// need to switch player
						JSClient.SetPlayer(city.pid, cityId);
					}
					else
					{
						var changed = await city.SetBuildInternal(scrollIntoUI, true, isLocked);
						if (changed)
						{
							if (isLocked || waitOnChange)
								await ChangeCityJSWait(cityId);
							else
								ChangeCityJS(cityId);
						}
					}
					if (!lazyMove)
						cityId.BringCidIntoWorldView(lazyMove, false);
				}
				else
				{
					ShowCity(cityId, lazyMove, scrollIntoUI);
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


				  await view.InvokeScriptAsync("addtoattacksender", new string[] { (cityId).ToString() });
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

				var p = JsonSerializer.Deserialize<AttackSenderScript>(cmd);
				await CitySwitch(p.cid, false);

				await App.DispatchOnUIThreadTask(async () =>
				{
					await view.InvokeScriptAsync("openAttackSender", new string[] { cmd });
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
		//		App.DispatchOnUIThreadSneaky(async () =>
		//		{

		//			if (City.StBuild(cityId, false).changed)
		//			{
		//				await view.InvokeScriptAsync("chcity", new string[] { (cityId).ToString() });
		//				await Task.Delay(1000);
		//			}
		//			view.InvokeScriptAsync("clearres", new string[] { (cityId).ToString() });
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
				App.DispatchOnUIThreadSneaky(() => view.InvokeScriptAsync("setviewmode", new string[] { viewMode == ShellPage.ViewMode.city ? "c" : "r" }));

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
			//    view.InvokeScriptAsync("setCameraC", new string[] {
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
				//     view.InvokeScriptAsync("eval", new string[] { $"gspotfunct.infoPlay('{playerName}')" });
				view.InvokeScriptAsync("infoPlay", new string[] { playerName });
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
				view.InvokeScriptAsync("alliancelink", new[] { allianceName });
				//     view.InvokeScriptAsync("eval", new string[] { $"gspotfunct.alliancelink('{allianceName}')" });
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
				view.InvokeScriptAsync("eval", new string[] { $"__c.showreport('{report}')" });
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
		//             view.InvokeScriptAsync("gStphp", new string[] { (cityId%65536).ToString(),(cityId/65536).ToString() });
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
			App.DispatchOnUIThreadSneaky(() =>
			{
				view.InvokeScriptAsync("shCit", new string[] { (cityId).ToString() });
				//int x = cityId%65536;
				//int y = cityId/65536;
				//var spotInfo = TileData.instance.GetSpotType(x, y);
				//Note.Show($"{x}:{y},{spotInfo.x}:{spotInfo.y} {spotInfo.type}");

			});
		}

		
		public static void gStCB(int cityId, Action<JsonElement> cb, int hash)
		{
			gstCBs.TryAdd(hash, cb);
			App.DispatchOnUIThreadSneaky(() =>
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
				
				view.InvokeScriptAsync("rmp",  new string[] { str });
				view.InvokeScriptAsync("gStQueryCB", new string[] { (cityId).ToString(), hash.ToString() });
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
		//                await view.InvokeScriptAsync("eval", new string[] { reader.ReadToEnd() });
		//                Log("funky");
		//                await view.InvokeScriptAsync("avactor", null);
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

		//    var str = await view.InvokeScriptAsync("getppdt", null);
		//    ppdt = JsonDocument.Parse(str);
		//    UpdatePPDT(ppdt.RootElement);
		//}

		//public static async Task PollCity(int cid)
		//{
		//	await Task.Delay(50);
		//	await view.InvokeScriptAsync("pollthis", new[] { cid.ToString() });
		//	await Task.Delay(400); // hack:  Todo, handle this property
		//	await view.InvokeScriptAsync("pollthis", new[] { cid.ToString() });
		//	await Task.Delay(300); // hack:  Todo, handle this property
		//}
		static readonly float[] researchRamp = { 0, 1, 3, 6, 10, 15, 20, 25, 30, 35, 40, 45, 50 };

		static ConcurrentDictionary<int, Action<JsonElement>> gstCBs = new ConcurrentDictionary<int, Action<JsonElement>>();

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
					
					App.DispatchOnUIThreadLow( ()=>
					{
						ShareString.Touch().tradeSettings.ItemsSource = TradeSettings.all;
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
					if(wo.TryGetProperty("p",out var p))
					{
						foreach(var pset in p.EnumerateObject())
						{
							var ps = new WorldViewSettings.PlayerSetting();
							ps.pid = int.Parse(pset.Name);
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
							ps.pid = int.Parse(pset.Name);
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
				if (!councillorsChecked)
				{
					councillorsChecked = true;
					if (jse.TryGetProperty("cob", out var cob))
					{
						var serverTime = GameTimeMs();


						foreach (var c in cob.EnumerateObject())
						{
							var t = c.Value.GetAsInt64() * 1000;
							if (t < serverTime)
							{
								App.DispatchOnUIThreadSneaky(ShowCouncillorsMissingDialog);
								break;
							}

						}
					}
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
					App.DispatchOnUIThreadLow(() =>
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
							App.DispatchOnUIThreadSneaky(() =>
							 view.InvokeScriptAsync("chcity", new string[] { (cid).ToString() }));

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

			// Log($"PPDT: c:{cUpdated}, clc:{clChanged}");

			// Log(ppdt.ToString());
		}

		
		public static async Task CityRefresh()
		{
				if (JSClient.ppdtInitialized)
				{
					// don't wait on this
					await JSClient.JSInvokeTask("cityRefresh", new[] { City.build.ToString() } );
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

		static private void View_PermissionRequested(WebView sender, Windows.UI.Xaml.Controls.WebViewPermissionRequestedEventArgs args)
		{
			var pr = args.PermissionRequest;
			pr.Allow();
			Log($"Permission {pr.Id} {pr.PermissionType} {pr.State} {pr.ToString()}");
			
		}

		static private void View_NavigationCompletedAsync(WebView sender, Windows.UI.Xaml.Controls.WebViewNavigationCompletedEventArgs args)
		{
			Log($"Complete {args.Uri}");
//			if(  )
			{

			}

		}
		static Certificate cotgCert;
		private static void  ServerCustomValidationRequested(HttpBaseProtocolFilter sender, HttpServerCustomValidationRequestedEventArgs customValidationArgs)
		{
		
			cotgCert = customValidationArgs.ServerCertificate;
			// Validate the server certificate as required.
			//            customValidationArgs.Reject();
		}
		static HttpResponseMessage resp;
		private static async void GetMe(Windows.UI.Xaml.Controls.WebViewWebResourceRequestedEventArgs args, Deferral def)
		{
			var client = new HttpClient(httpFilter);
			//	var headers = httpClient.DefaultRequestHeaders;
			//client.DefaultRequestHeaders.Accept.Clear();
			//client.DefaultRequestHeaders.Accept.ParseAdd("application/signed-exchange");
		
			// httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("X-Requested-With", "XMLHttpRequest");
			//    httpClient.DefaultRequestHeaders.Referer = new Uri(httpsHost, "/overview.php?s=0");// new Uri($"https://w{world}.crownofthegods.com");
			client.DefaultRequestHeaders.Referer = new Uri("https://www.crownofthegods.com/"); // new Uri                                                       //             req.Headers.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
																 //httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
			
				client.DefaultRequestHeaders.Host = new Windows.Networking.HostName("www.crownofthegods.com");
			//   Log($"Built headers {httpClient.DefaultRequestHeaders.ToString() }");
			
			
			client.DefaultRequestHeaders.UserAgent.TryParseAdd(userAgent);
			


			client.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Dest", "document");
			client.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Site", "same-origin");
			client.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-User", "?1");
			client.DefaultRequestHeaders.TryAppendWithoutValidation("sec-ch-ua", "\" Not; A Brand\";v=\"99\", \"Microsoft Edge\"; v=\"91\", \"Chromium\"; v=\"91\"");
			client.DefaultRequestHeaders.TryAppendWithoutValidation("sec-ch-ua-mobile", "?0");

	//		SetCookie("lout", "1");
	//		SetCookie("G_AUTHUSER_H", "0", "www.crownofthegods.com", "/");
	//		SetCookie("G_AUTHUSER_H", "0", "www.crownofthegods.com", "/home");


			var req = args.Request;
			args.Request.Headers.Accept.Clear();
			args.Request.Headers.Accept.ParseAdd("application/signed-exchange;v=b3;q=0.9");
			{
				resp = await client.SendRequestAsync(args.Request, HttpCompletionOption.ResponseContentRead);
				foreach (var h in resp.Headers)
				{
					Log(h.Key);
					Log(h.Value);
					if(h.Key == "Set-Cookie")
					{
						var bb = h.Value.Split(';', StringSplitOptions.RemoveEmptyEntries);
						var bb2 = bb[0].Split('=', StringSplitOptions.RemoveEmptyEntries);
						SettingsPage.secSessionId = bb2[1];
					}

				}

				foreach (var h in resp.Content.Headers)
				{
					Log(h.Key);
					Log(h.Value);

				}
				var buff = await resp.Content.ReadAsStringAsync();
				Log(buff);
				args.Response = resp;
				def.Complete();
			}

		}

		static private void View_NavigationStarting(WebView sender, Windows.UI.Xaml.Controls.WebViewNavigationStartingEventArgs args)
		{

			try
			{
				//if (args.Uri.ToString().StartsWith("https://www.crownofthegods.com/?email"))
				//{
				//	GetMe(args.Uri);
				//	return;
				//}
				Log($"Nav start {args.Uri} {args.Uri}");
				

				var match = urlMatch.Match(args.Uri.Host);

				if (match.Groups.Count == 2 && (args.Uri.LocalPath == "/" || args.Uri.Fragment.Contains('&')))
				{
					//if (httpFilter != null)
					//	Debug.Fatal();  // Todo
					world = int.Parse(match.Groups[1].ToString());


					// once we have the world id we can load the background
					AGame.LoadWorldBackground();
					try
					{

						httpsHostString = $"https://{args.Uri.Host}";
						httpsHost = new Uri(httpsHostString);
						//httpFilter = 
						
						//						if (subId == 0)
						//							httpFilter.CookieUsageBehavior = HttpCookieUsageBehavior.NoCookies;// HttpCookieUsageBehavior.Default;
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





						clientPool = new ConcurrentBag<HttpClient>();
						for (int i = 0; i < clientCount; ++i)
						{
							var httpClient = new HttpClient(httpFilter); // reset
																		 //   httpClient = new HttpClient(); // reset
																		 //                        var headers = httpClient.DefaultRequestHeaders;
																		 //     headers.TryAppendWithoutValidation("Content-Type",@"application/x-www-form-urlencoded; charset=UTF-8");
																		 // headers.TryAppendWithoutValidation("Accept-Encoding","gzip, deflate, br");
																		 //                        headers.TryAppendWithoutValidation("X-Requested-With", "XMLHttpRequest");
																		 //    headers.Accept.TryParseAdd(new HttpMediaTypeHeaderValue(@"application/json"));
																		 //   headers.Add("Accept", @"*/*");
																		 //                            httpClient.DefaultRequestHeaders.Clear();
							httpClient.DefaultRequestHeaders.AcceptLanguage.TryParseAdd("en-US,en;q=0.5");
							//    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(@"Mozilla/5.0 (Windows NT 10.0; Win64; x64; WebView/3.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.19631");
							//    httpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Credentials", "true");
							httpClient.DefaultRequestHeaders.Accept.TryParseAdd("*/*");
							// httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("X-Requested-With", "XMLHttpRequest");
							//    httpClient.DefaultRequestHeaders.Referer = new Uri(httpsHost, "/overview.php?s=0");// new Uri($"https://w{world}.crownofthegods.com");
							httpClient.DefaultRequestHeaders.Referer = new Uri(httpsHost, $"/overview.php?s={subId}");// new Uri                                                       //             req.Headers.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
							if (subId != 0)
								httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("pp-ss", subId.ToString());

							//httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
							//   Log($"Built headers {httpClient.DefaultRequestHeaders.ToString() }");
							httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(userAgent);
							//httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Pragma", "no-cache");

							//httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Site", "same-origin");
							//httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Mode", "cors");
							//httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Dest", "empty");

							clientPool.Add(httpClient);


						}
						//cookieManager = new HttpBaseProtocolFilter().CookieManager;
						//  clientPool.CompleteAdding();
						view.WebResourceRequested -= View_WebResourceRequested1;
						view.WebResourceRequested += View_WebResourceRequested1;


					}
					catch (Exception e)
					{

						LogEx(e);
					}

				}
				else if (args.Uri.ToString().StartsWith( "https://www.crownofthegods.com"))
				{
				//	if(!JSClient.isSub)
						SetSessionCookie();
				}
			}
			catch (Exception e)
			{
				LogEx(e);
			}



		}

		static ConcurrentDictionary<string, BitmapImage> imageCache = new ConcurrentDictionary<string, BitmapImage>();
		private static long gameMSAtStart;
		private static DateTimeOffset launchTime;

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

		static private void View_NavigationFailed(object sender, Windows.UI.Xaml.Controls.WebViewNavigationFailedEventArgs e)
		{

			
			Exception($"Internet failed, press any key to retry {e.Uri} {e.WebErrorStatus}");
			Log("Refresh");
			if (view != null)
				view.Refresh();
		}

		static private void View_DOMContentLoaded(WebView sender, Windows.UI.Xaml.Controls.WebViewDOMContentLoadedEventArgs args)
		{
			//if (args.Uri.ToString() == "https://www.crownofthegods.com/home/")
			//{
			//	SetSessionCookie();
			//}

			//Log($"Dom loaded {args.Uri}");
			//if (urlMatch.IsMatch(args.Uri.Host))
			//{
			//    Log("Match Regex!");
			//    await AddJSPluginAsync();
			//}
		}

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
				cid = 0;
				var _t = t;
				t = null;
				_t.SetResult(true);
			}
			public bool isDone => t == null;
		}

		static ConcurrentBag<WaitOnCityDataData> waitingOnCityData = new();
		static void ChangeCityJS(int cityId)
		{
			App.DispatchOnUIThreadSneaky(() =>
							view.InvokeScriptAsync("chcity", new string[] { (cityId).ToString() }));

		}
		static async Task ChangeCityJSWait(int cityId)
		{
			Log($"Wait {City.GetOrAdd(cityId).nameAndRemarks}");
			var i = new WaitOnCityDataData(cityId);
			waitingOnCityData.Add(i);
			ChangeCityJS(cityId);
			await i.t.Task;
			Log($"WaitComplete {City.GetOrAdd(cityId).nameAndRemarks}");
		}
		static private void View_ScriptNotify(object sender, Windows.UI.Xaml.Controls.NotifyEventArgs __e)

		{
			var eValue = __e.Value;
			var eCallingUri = __e.CallingUri;
			Task.Run(async () =>
		   {
		   try
		   {
			   bool gotCreds = false;
			   Log($"Notify: {eValue.Length},{eCallingUri},{sender}:{eValue.Truncate(128) }");
			   var jsDoc = JsonDocument.Parse(eValue);
			   var jsd = jsDoc.RootElement;
			   foreach (var jsp in jsd.EnumerateObject())
			   {
				   switch (jsp.Name)
				   {
					   case "jsvars":
						   {
							   App.DispatchOnUIThreadSneaky(() => ShellPage.instance.cookie.Visibility = Visibility.Collapsed );

								   var jso = jsp.Value;

								   var s = CookieDB.Serialize(cookieManager);// GetSecSessionId();
								   var token = jso.GetString("token");
								   var raidSecret = jso.GetString("raid");
								   var agent = jso.GetString("agent");
//								   jsVars.cookie = jso.GetString("cookie");
								//   Log(jsVars.cookie);
								   Log(token);
								   Log(s);
								   for (int i = 0; i < clientCount; ++i)
								   {
									   await clientPoolSema.WaitAsync();
								   }
								   for (; ; )
								   {
									   try
									   {
										   {
											   //    var clients = clientPool.ToArray();
											   foreach (var httpClient in clientPool)
											   {

												   
								//				   if (subId == 0)
									//				   httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Cookie", "sec_session_id=" + s);
											   }
										   }
									   }
									   catch (Exception _ex)
									   {
										   LogEx(_ex);
										   await Task.Delay(1000);
										   continue;

									   }
									   break;
								   }

								   clientPoolSema.Release(clientCount);
								   
								   var timeOffset = jso.GetAsInt64("timeoffset");
								   var timeOffsetRounded = Math.Round(timeOffset / (1000.0 * 60 * 30)) * 30.0f; // round to nearest half hour
								   gameTOffset = TimeSpan.FromMinutes(timeOffsetRounded);
								   gameTOffsetSeconds = (int)gameTOffset.TotalSeconds;
									gameTOffsetMs = (long)gameTOffset.TotalMilliseconds;
								   var str = timeOffsetRounded >= 0 ? " +" : " ";
								   str += $"{gameTOffset.Hours:D2}:{gameTOffset.Minutes:D2}";
								   Helpers.JSON.timeZoneString = str;
								   //   Log(JSONHelper.timeZoneString);
								   
								   Log($"TOffset {gameTOffset}");
								   Log(ServerTime().ToString());
								   ppss = jso.GetAsInt("ppss");
								   Player.myName = jso.GetString("player");
								   if (Player.subOwner == null)
									   Player.subOwner = Player.myName;
								   Player.activeId =Player.myId = jso.GetAsInt("pid"); ;
								   Player.myIds.Add(Player.myId);
								   var cid = jso.GetAsInt("cid");
								   City.build = City.focus = cid;
								   NavStack.Push(cid);
								   App.DispatchOnUIThreadLow(() => ShellPage.instance.coords.Text = cid.CidToString());
								   AGame.cameraC = cid.CidToWorldV();
								   //Note.L("cid=" + cid.CidToString());
								   gameMSAtStart = jso.GetAsInt64("time");
								   launchTime = DateTimeOffset.UtcNow;
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
								   AddPlayer(true, true, Player.myId, Player.myName, token, raidSecret,s, ppdt.ToString());


								   UpdatePPDT(ppdt,Player.myId,pruneCities:true);
								   if (Player.isAvatarOrTest)
									   Raid.test = true;
								   Task.Delay(2200).ContinueWith((_)=> App.DispatchOnUIThreadSneakyLow(Spot.UpdateFocusText));
								   BuildQueue.Initialize();
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
								   App.DispatchOnUIThread(() => Launcher.LaunchUriAsync(new Uri($"{App.appLink}:launch?w={world}&s={i}&n=1&p={HttpUtility.UrlEncode(Player.myName, Encoding.UTF8)}")));
								   break;
							   }
						   case "shcit":
							   {
								   var jso = jsp.Value;
								   var cid = jso.GetAsInt();
								   Spot.ProcessCoordClick(cid, false, App.keyModifiers, true); // then normal click
								   //App.DispatchOnUIThreadSneaky(async () =>
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

								   App.OnPointerPressed(
								   kind);
								   {
									   var _x = x - ShellPage.canvasBaseX;
									   var _y = y - ShellPage.canvasBaseY;
									   if (_x > 0 && _y > 0)
										   ShellPage.Canvas_PointerPressedJS(x, y, kind);
								   }
								   ShellPage.SetWebViewHasFocus(false);
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
											   await Task.Delay(30000);
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
										   App.DispatchOnUIThreadSneakyLow(() => city.OnPropertyChanged(nameof(City.icon)));
									   }
									   city.isOnWater |= jso.GetAsInt("water") != 0;  // Use Or in case the data is imcomplete or missing, in which case we get it from world data, if that is not incomplete or missing ;)
									   city.isTemple = jso.GetAsInt("plvl") != 0;


									 //  cid.BringCidIntoWorldView(true,false);
									   if (city._cityName != name)
									   {
										   city._cityName = name;
										   if (cid == Spot.focus)
											   App.DispatchOnUIThreadLow(() => ShellPage.instance.focus.Content = city.nameAndRemarks);
									   }
									   //     city.SetFocus(true);
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
								   foreach( var i in waitingOnCityData )
								   {
										   if(i.cid == cid)
										   {
											   i.Done();
										   }
								   }
								   if( !waitingOnCityData.Any(i=>!i.isDone) )
								   {
									   waitingOnCityData.Clear();
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
									   await Task.Delay(500);
								   }
								   if (Player.isAvatarOrTest)
								   {
									   App.DispatchOnUIThreadSneaky(() =>
									   {
									   // create a timer for precense updates
									   presenceTimer = new DispatcherTimer();
										   presenceTimer.Interval = TimeSpan.FromSeconds(16);
										   presenceTimer.Tick += PresenceTimer_Tick; ;
										   presenceTimer.Start();
									   // Seed it off

								   });
									   PresenceTimer_Tick(null, null); // seed it off, but only after our token has time to have been set
								   }
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
								   var jso = jsp.Value;
								   var raidSecret = jso.GetString("secret");
								   var pid = jso.GetInt("pid");

								   pendingCookies = null;
								   var pn = jso.GetString("pn");
								   var ppdt = jso.GetProperty("ppdt");
								   var token = jso.GetString("token");
								   var s = jso.GetString("s");
								   var cid = jso.GetAsInt("cid");
								   AddPlayer(false, true, pid, pn, token, raidSecret, s, ppdt.ToString());

								   var city = City.GetOrAdd(cid);
								   // If they are visiting somene elses city we don't want to be directed there
								   // so we go to the default city
								   UpdatePPDT(ppdt, pid,updateBuildCity:(city.pid != pid) ); 
								   
								   if (city.pid == pid) // we want ot visit a specific city
								   {
									  CitySwitch(cid,true);
								   }
								   City.CitiesChanged();
								   break;
							   }
						   case "restoreglobals":
						   {
								   Note.Show("Cookies failed, maybe they need to log in again to refresh cookies?");
								   // only need to restore cookies
								   CookieDB.Apply(jsVars.cookies);
								   pendingCookies = null;

								   App.DispatchOnUIThreadSneaky(() => ShellPage.instance.friendListBox.SelectedItem = Player.activePlayerName);

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
									   // App.DispatchOnUIThreadSneaky(() => Models.JSPopupNode.Show(popup));
									   Models.JSPopupNode.Show(popup);

								   }
								   ShellPage.NotifyCotgPopup(popupCount);
								   //                                ShellPage.SetCanvasVisibility(noPopup);
								   if (ppdtInitialized && jso.TryGetProperty("c", out var _cid))
								   {
									   var cid = _cid.GetAsInt();
									   if (cid != City.build)
									   {
										   var city = City.GetOrAddCity(cid);
										   city.SetBuildInternal(true);
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
					  

					   ShellPage.SetViewModeCity();

					   APlayfab.Login();

					   GetWorldInfo.Send();
					   ShellPage.canvasVisible = true;
					   ShellPage.isHitTestVisible = true;
					   ///                   await GetCitylistOverview();
					   Task.Delay(3000).ContinueWith( (_)=> City.UpdateSenatorInfo() );  // no async
					   Friend.LoadAll();
					   TileData.Ctor(false);
					   //if (TipsSeen.instance.refresh == false
					   //||TipsSeen.instance.chat0==false
					   //|| TipsSeen.instance.chat1 == false
					   //|| TipsSeen.instance.chat2 == false)
					   //    App.QueueIdleTask(ShellPage.ShowTipRefresh);
					   // await RaidOverview.Send();
					   App.QueueIdleTask(IncomingOverview.ProcessTask, 1000);
					   App.QueueIdleTask(OutgoingOverview.ProcessTask, 1000);

					   SetStayAlive(SettingsPage.stayAlive);
					   //{
					   //    //var now = DateTime.UtcNow;
					   //    //if (now.Day <= 28 && now.Month==11)
					   //    {
					   AppCenter.SetUserId(Player.myName);
					   {
						   CustomProperties properties = new CustomProperties();
						   properties.Set("alliance", Alliance.myId).Set("world", JSClient.world).Set("sub", JSClient.isSub );
						   AppCenter.SetCustomProperties(properties);
					   }
					   if (SystemInformation.Instance.IsAppUpdated)
					   {
						   App.DispatchOnUIThreadSneaky(ShowWhatsNew);
					   }
					
					   // 
					   
					   App.state = App.State.active;
					   CnVDiscord.Discord.Initialize();

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
		
		
		public static async void ShowWhatsNew()
		{

			var dialog = new WhatsNewDialog();
			dialog.DefaultButton = Windows.UI.Xaml.Controls.ContentDialogButton.Primary;
			dialog.fixesText.Text = new StreamReader((typeof(Fixes).Assembly).GetManifestResourceStream($"COTG.Notes.fixes.md")).ReadToEnd();
			
			var result = await dialog.ShowAsync2();
		}
		private static async void PresenceTimer_Tick(object sender, object e)
		{
			/*
			var players = await Cosmos.GetPlayersInfo();
			var changed = false;
			int put = 0;
			int validCount = 0;
			foreach (var _p in players)
			{
				var pid = int.Parse(_p.id);
				if (pid == Player.myId || Friend.all.Any(a =>a.pid==pid) || Player.isAvatarOrTest )
					++validCount;
			}
			var presence = new PlayerPresence[validCount];
			foreach (var _p in players)
			{
				var p = new PlayerPresence(_p);
				int priorCid;
				var pid = p.pid;
				if (!(pid == Player.myId || Friend.all.Any(a => a.pid == pid)||Player.isAvatarOrTest))
					continue;

				var priorIndex = PlayerPresence.all.IndexOf( ( a) => a.pid == pid );
				if (priorIndex == -1)
				{
					changed = true;
					priorCid = 0;
				}
				else
				{
					if (PlayerPresence.all[priorIndex].token != p.token)
						changed = true; // need to refresh token
					priorCid = PlayerPresence.all[priorIndex].cid;
				}
				
			//	Player.myIds.Add(pid);
			// TODO:  restore this functionality when it works again
				if (pid != Player.myId)
				{
					if (p.cid != priorCid)
					{
						if (p.cid == City.build && priorCid != City.build)
							Note.Show($"{p.name } has joined you in {p.cid.CidToStringMD()}");
						if (p.cid != City.build && priorCid == City.build)
							Note.Show($"{p.name } has left {p.cid.CidToStringMD()}");

					}
				}
				presence[put++] = p;
			
			}
			PlayerPresence.all = presence;

			if(changed)
			{
				App.DispatchOnUIThreadLow(() =>
				{
					// Update menu
					ShellPage.instance.friendListBox.SelectedIndex = -1;
					ShellPage.instance.friendListBox.Items.Clear();
					int counter = 0;
					int sel = -1;
					foreach (var p in PlayerPresence.all)
					{
						ShellPage.instance.friendListBox.Items.Add(p.name);
						if (p.pid == Player.activeId)
							sel = counter;
						++counter;
						// reset menu, TOTO:  Keep track of active selection
					}

					ShellPage.instance.friendListBox.SelectedIndex = sel;
					ShellPage.instance.friendListBox.Visibility = PlayerPresence.all.Length > 1 ? Visibility.Visible : Visibility.Collapsed;
				});
			}
			*/
		}

		static private async void View_UnviewableContentIdentified(WebView sender, Windows.UI.Xaml.Controls.WebViewUnviewableContentIdentifiedEventArgs args)
		{
			if (await Windows.System.Launcher.LaunchUriAsync(args.Uri))
			{
				Note.Show($"Launched {args.Uri}");
			}
			else
			{
				Note.Show($"Failed to launch {args.Uri}");
			}
		}

		static private void View_UnsupportedUriSchemeIdentified(WebView sender, Windows.UI.Xaml.Controls.WebViewUnsupportedUriSchemeIdentifiedEventArgs args)
		{
			Exception("UnsupportedUriScheme");
		}

		static private void View_UnsafeContentWarningDisplaying(WebView sender, object args)
		{
			Exception("Unsafe");
		}

		public static TimeSpan gameTOffset;
		public static long gameTOffsetMs;
		public static int gameTOffsetSeconds;

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
