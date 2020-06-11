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
using System.Net.WebSockets;
using Windows.System;
using System.Text.Json;
using COTG.Game;
using System.Threading;
using COTG.Helpers;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;

namespace COTG
{
	/// <summary>
	/// The j s client.
	/// </summary>
	public class JSClient : ICommand
    {

   
        static JsonDocument ppdt;
        public static JSClient instance = new JSClient();
        public static WebView view;
        static KeyboardAccelerator refreshAccelerator;
        static HttpBaseProtocolFilter httpFilter;
        public static HttpClient httpClient;
        public static int world = 19;
        static Regex urlMatch = new Regex(@"w(\d\d).crownofthegods.com");
        public static Uri httpsHost;
        static HttpRequestMessage anyPost;
        // IHttpContent content;
        public struct JSVars
        {
            public string token { get; set; }
            public int ppss { get; set; }
            public string player { get; set; }
            public int pid { get; set; }
            public string alliance { get; set; }
            public string s { get; set; }
            public string cookie { get; set; }
            public int cid { get; set; }
        };
       


        public static JSVars jsVars;


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
           // headers.TryAppendWithoutValidation("Content-Type", @"application/x-www-form-urlencoded; charset=UTF-8");
          //  headers.TryAppendWithoutValidation("pp-ss", "0");
         //   headers.TryAppendWithoutValidation("Referer", $"https://w{world}.crownofthegods.com/overview/overview.php?s=0");
         //   headers.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");

        }

        internal static WebView Initialize(Grid panel)
        {

			try
			{
                view = new WebView(WebViewExecutionMode.SeparateThread)
                { HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    AllowFocusOnInteraction=true };
				view.UnsafeContentWarningDisplaying += View_UnsafeContentWarningDisplaying;
				view.UnsupportedUriSchemeIdentified += View_UnsupportedUriSchemeIdentified;

				view.UnviewableContentIdentified += View_UnviewableContentIdentified;
				view.ScriptNotify += View_ScriptNotify;
				view.DOMContentLoaded += View_DOMContentLoaded;
				view.NavigationFailed += View_NavigationFailed;
				view.NavigationStarting += View_NavigationStarting;
				view.NavigationCompleted += View_NavigationCompletedAsync;
				view.PermissionRequested += View_PermissionRequested;
              //  view.WebResourceRequested += View_WebResourceRequested1;

				//   view.CacheMode = CacheMode.
				//Grid.Se SetAlignLeftWithPanel(view, true);
				//RelativePanel.SetAlignRightWithPanel(view, true);
			///	RelativePanel.SetAlignTopWithPanel(view, true);
		//		RelativePanel.SetAlignBottomWithPanel(view, true);
				
				view.Source = new Uri("https://www.crownofthegods.com");

				refreshAccelerator = new KeyboardAccelerator() { Key = Windows.System.VirtualKey.F5 };
				refreshAccelerator.Invoked += (_, __) => view?.Refresh();

			}
			catch (Exception e)
			{
				Log(e);
			}
            return view;



		}

        async private static void View_WebResourceRequested1(WebView sender, WebViewWebResourceRequestedEventArgs args)
        {
            try
            {
                var req = args.Request;
                if (req.Method.Method == HttpMethod.Post.Method && !req.RequestUri.LocalPath.EndsWith("poll2.php") )
                {
               //     if (req.Content != null)
               //         await req.Content.BufferAllAsync();
                 //   req.Content.BufferAllAsync();
                    Log($"Post: {req.RequestUri.ToString()} {req.Headers.ToString()} {req.TransportInformation?.ToString()} {req.Properties?.ToString()} {req.Content?.ToString()}");
                    if (args.Response != null)
                    {
                        Log(args.Response.Version.ToString());
                        anyPost = req;
                    //    await COTG.Services.RestAPI.HandleResponse(args.Request.RequestUri, args.Response);
                    }
                }

            }
            catch (Exception e)
            {
                Log(e);
            }

        }

        public static void Refresh(object ob,RoutedEventArgs args)
        {
            if (view == null)
                return;
            view.Refresh();
            Services.NavigationService.Navigate<Views.MainPage>();
        }

        public async static Task ChangeCity(int cityId)
		{
			try
			{
				if (view != null)
					await view.InvokeScriptAsync("eval", new string[] { $"gspotfunct.chcity({cityId})" });

			}
			catch (Exception e)
			{
				Log(e);
			}


		}
        public async static Task ShowCity(int cityId)
        {
			try
			{
				if (view != null)
				{
					await view.InvokeScriptAsync("eval", new string[] { $"gspotfunct.shCit({cityId})" });
				}
			}
			catch (Exception e)
			{
				Log(e);
			}


		}

        private static async Task AddJSPluginAsync()
        {
            try
            {
                var asm = typeof(JSClient).Assembly;
                using (Stream stream = asm.GetManifestResourceStream("COTG.Javascript.funky.js"))
                {

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        Log("execute");
                        await view.InvokeScriptAsync("eval", new string[] { reader.ReadToEnd() });
                        Log("funky");
                        await view.InvokeScriptAsync("avactor", null);
			        }
                }

            }
            catch (Exception e)
            {
                Log(e);
            }
        }
        public static async Task GetPPDT()
        {
            var str = await view.InvokeScriptAsync("getppdt", null);

            Log(str);
            ppdt = JsonDocument.Parse(str);
            // extract cities
            lock (City.all)
            {
                var now = DateTime.Now;
                foreach (var jsCity in ppdt.RootElement.GetProperty("c").EnumerateArray())
                {
                    var cid = jsCity.GetProperty("1").GetInt32();

                    if (!City.all.TryGetValue(cid, out var city))
                    {
                        city = new City() { cid = cid };
                        City.all.Add(cid, city);

                    }
                    city.name = jsCity.GetProperty("2").GetString();
                    city.isCastle = jsCity.GetAsInt("12") != 0;
                    city.lastUpdated = now;
                    city.isOnWater = jsCity.GetAsInt("16") != 0;
                    city.isTemple = jsCity.GetAsInt("15") != 0;
                    city.owner = jsVars.player;
                    city.alliance = jsVars.alliance;
                    

                }

                Log(City.all.ToString());
                Log(City.all.Count());
            }
             Views.MainPage.UpdateCityList();


            // Log(ppdt.ToString());
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

            try
            {
                Log($"Nav start {args.Uri} {args.Uri.Host}");
                var match = urlMatch.Match(args.Uri.Host);
                if (match.Groups.Count == 2)
                {
                    world = int.Parse(match.Groups[1].ToString());

                    try
                    {
                        httpFilter = new HttpBaseProtocolFilter();// HttpBaseProtocolFilter.CreateForUser( User.GetDefault());
                        httpFilter.AllowAutoRedirect = true;
//                        httpFilter.ServerCredential =
                      //  httpFilter.ServerCustomValidationRequested += HttpFilter_ServerCustomValidationRequested;
                        httpFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.NoCache;
                        httpFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                        httpFilter.CookieUsageBehavior = HttpCookieUsageBehavior.Default;
                        httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.IncompleteChain);
    //                    httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.InvalidCertificateAuthorityPolicy);
  //                      httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.OtherErrors);
      //                  httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.BasicConstraintsError);
          //              httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.InvalidSignature);
                        httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.RevocationInformationMissing);
                        httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.RevocationFailure);
        //                httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Revoked);
                        httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.WrongUsage);
                        httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);
                        httpFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Untrusted );

//                        "Success", "Revoked", "InvalidSignature", "InvalidCertificateAuthorityPolicy", "BasicConstraintsError", "UnknownCriticalExtension", "OtherErrors""Success", "Revoked", "InvalidSignature", "InvalidCertificateAuthorityPolicy", "BasicConstraintsError", "UnknownCriticalExtension", "OtherErrors"
                        httpFilter.AllowUI = true;
                        httpFilter.AutomaticDecompression = true;
                        httpFilter.MaxVersion = HttpVersion.Http20;
                      
//                        httpFilter.User.

                        httpsHost = new Uri($"https://{args.Uri.Host}");
                        httpClient = new HttpClient(httpFilter); // reset
                                                                 //   httpClient = new HttpClient(); // reset
                        //                        var headers = httpClient.DefaultRequestHeaders;
                        //     headers.TryAppendWithoutValidation("Content-Type",@"application/x-www-form-urlencoded; charset=UTF-8");
                        // headers.TryAppendWithoutValidation("Accept-Encoding","gzip, deflate, br");
                        //                        headers.TryAppendWithoutValidation("X-Requested-With", "XMLHttpRequest");
                        //    headers.Accept.TryParseAdd(new HttpMediaTypeHeaderValue(@"application/json"));
                        //   headers.Add("Accept", @"*/*");
                    }
                    catch (Exception e)
                    {

                        Log(e);
                    }

                }
            }
            catch (Exception e)
            {
                Log(e);
            }



        }

        private static void HttpFilter_ServerCustomValidationRequested(HttpBaseProtocolFilter sender, HttpServerCustomValidationRequestedEventArgs args)
        {
            Log(args.ToString());
        }

        static private async void View_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            
            await Exception($"Internet failed, press any key to retry {e.Uri} {e.WebErrorStatus}");
            Log("Refresh");
            if (view!=null)
                view.Refresh();
        }

        static async private void View_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            Log($"Dom loaded {args.Uri}");
            if (urlMatch.IsMatch(args.Uri.Host))
            {
                Log("Match Regex!");
                await AddJSPluginAsync();
            }
        }
        async static private void View_ScriptNotify(object sender, NotifyEventArgs e)
        {
            try
            {
                bool gotCreds = false;
                Log($"Notify: {e.CallingUri} {e.Value} {sender}");
                var jsd = JsonDocument.Parse(e.Value).RootElement;
                foreach (var jsp in jsd.EnumerateObject())
                {
                    switch (jsp.Name)
                    {
                        case "jsvars":
                            {
                                var jso = jsp.Value;
                                jsVars.token = jso.GetString("token");
                                jsVars.ppss = jso.GetAsInt("ppss");
                                jsVars.player = jso.GetString("player");
                                jsVars.pid = jso.GetAsInt("pid");
                                jsVars.alliance = jso.GetString("alliance");
                                jsVars.s = jso.GetString("s");
                                jsVars.cid = jso.GetAsInt("cid");
                                Log(System.Text.Json.JsonSerializer.Serialize(jsVars));

                                gotCreds = true;
                                break;
                            }
                        case "cityclick":
                            {
                                var jso = jsp.Value;
                                var cid = jso.GetAsInt("cid");
                                lock (City.all)
                                {
                                    if (!City.all.TryGetValue(cid, out var city))
                                    {
                                        city = new City() { cid = cid };
                                        City.all.Add(cid, city);
                                    }
                                    city.name = jso.GetString("name");
                                    city.owner = jso.GetString("player"); // todo: this shoule be an int playerId
                                    city.notes = jso.GetString("notes");
                                    city.alliance = jso.GetString("alliance"); // todo:  this should be an into alliance id
                                    city.lastAccessed = DateTime.Now;
                                }
                                COTG.Views.MainPage.UpdateCityList();

                            }
                            break;
                    }

                }


                //var cookie = httpClient.DefaultRequestHeaders.Cookie;
                //cookie.Clear();
                //foreach (var c in jsVars.cookie.Split(";"))
                //{
                //    cookie.ParseAdd(c);
                //}


                httpClient.DefaultRequestHeaders.AcceptLanguage.TryParseAdd("en-US,en;q=0.5");
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(@"Mozilla/5.0 (Windows NT 10.0; Win64; x64; WebView/3.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.19631");
                //    httpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Credentials", "true");
                httpClient.DefaultRequestHeaders.Accept.TryParseAdd("*/*");
                // httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("X-Requested-With", "XMLHttpRequest");
                httpClient.DefaultRequestHeaders.Referer = httpsHost;// new Uri($"https://w{world}.crownofthegods.com");
                                                                     //             req.Headers.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
                httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("pp-ss", jsVars.ppss.ToString());

                httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
                Log($"Built heades {httpClient.DefaultRequestHeaders.ToString() }");

                if (gotCreds)
                {
                    await Task.Delay(2000);
                    await GetPPDT();
                }


            }
            catch (Exception ex)
            {

                Log(ex);
            }
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
            //     using var req = new HttpRequestMessage(HttpMethod.Post, new Uri( new Uri($"https://w{world}.crownofthegods.com"), "poll2.php"));

            //            AddDefaultHeaders(req.Headers);


            try
            {
                var url =
                       "world=&" +
                       "cid=17367265&" +
                       "ai=0&" +
                       $"ss={jsVars.s}";

                //            using var req  =anyPost;
                var req = new HttpRequestMessage(HttpMethod.Post, new Uri(httpsHost, @"includes/poll2.php"));
               // req.TransportInformation.ver
                //req.AllowAutoRedirect = true;
                req.Content = new HttpStringContent(url,

                                                        Windows.Storage.Streams.UnicodeEncoding.Utf8,

                                                        "application/x-www-form-urlencoded");//CONTENT-TYPE header UrlEncodeToBytes( url,  );
                                                                                             //    req.Headers.TryAppendWithoutValidation("Content-Encoding", jsVars.token);
                                                                                             //       req.Headers.Accept.TryParseAdd(@"application/json");
                                                                                             //            req.Headers.Accept.TryParseAdd(@"*/*");
               
  //              var value = new Windows.Networking.HostName(req.RequestUri.Host);
 //               req.Headers.Host = value;
                //if (anyPost != null)
                //    foreach (var h in anyPost.Headers)
                //    {
                //        req.Headers.TryAdd(h.Key, h.Value);
                //    }


                //            req.Headers.TryAppendWithoutValidation("Content-Encoding", jsVars.token);

                //            req.Content.Headers.ContentType.CharSet = "UTF-8";

                req.Content.Headers.TryAppendWithoutValidation("Content-Encoding", jsVars.token);

                var resp = await httpClient.SendRequestAsync(req, HttpCompletionOption.ResponseContentRead);
                Log($"Error: {resp.StatusCode}");
                Log($"Error: {resp.RequestMessage.ToString()}");
                if (resp.IsSuccessStatusCode)
                {
                    var b = await resp.Content.ReadAsStringAsync();

//                    jso = await JsonDocument.ParseAsync(b.ToString);

                    Log(b.ToString());
                }
                else
                {

                };
            }
            catch (Exception e)
            {
                Log(e);
            }




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
