﻿using System;
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
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Concurrent;
using Windows.Storage.Streams;
using Windows.Storage;
using COTG.Services;
using System.Web;
using COTG.Views;
using System.Numerics;
using Windows.UI.Xaml.Media;
using COTG.JSON;

namespace COTG
{
    /// <summary>
    /// The j s client.
    /// </summary>
    public class JSClient
    {

          public enum ViewMode  
            {
            city = 0,
            region=1,
            world=2
            };

        public static ViewMode viewMode;
        public static bool IsWorldView()	=> viewMode == ViewMode.world;
        public static bool IsCityView() => viewMode == ViewMode.city;
        public static bool IsRegionView() => viewMode == ViewMode.region;

        public static JsonDocument ppdt;
        public static JSClient instance = new JSClient();
        public static WebView view;
        static HttpBaseProtocolFilter httpFilter;
        const int clientCount = 8;
        public static BlockingCollection<HttpClient> clientPool;
        public static HttpClient downloadImageClient;

        public static int world = 0;
        static Regex urlMatch = new Regex(@"^w(\d\d).crownofthegods.com$");
        public static Uri httpsHost;
        public static string httpsHostString;
        static HttpRequestMessage anyPost;
        // IHttpContent content;
        public struct JSVars
        {
            public string token { get; set; }
            public int ppss { get; set; }
            public string player { get; set; }
            public int pid { get; set; }
            public string s { get; set; }
            public string cookie { get; set; }
            public DateTimeOffset launchTime;
            public long gameMSAtStart;
            public TimeSpan gameTOffset;

            public override string ToString()
            {
                return $"{{{nameof(token)}={token}, {nameof(ppss)}={ppss.ToString()}, {nameof(player)}={player}, {nameof(pid)}={pid.ToString()},  {nameof(s)}={s}, {nameof(cookie)}={cookie}}}";
            }
        };


        public static JSVars jsVars;

        public static long GameTimeMs()
        {
            return (long)((DateTimeOffset.UtcNow - jsVars.launchTime).TotalMilliseconds) + jsVars.gameMSAtStart;
        }
        public static DateTimeOffset ServerTime()
        {
            return (DateTimeOffset.UtcNow + jsVars.gameTOffset);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="JSClient"/> class.
        /// </summary>
        public JSClient()
		{
		}

		


        internal static WebView Initialize(Grid panel)
        {

			try
			{
                view = new WebView(WebViewExecutionMode.SeparateThread)
                {
                    //HorizontalAlignment = HorizontalAlignment.Stretch,
                    //VerticalAlignment = VerticalAlignment.Stretch,
                    //CacheMode=new BitmapCache()
                };
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
              //  view.WebResourceRequested += View_WebResourceRequested1;

				//   view.CacheMode = CacheMode.
				//Grid.Se SetAlignLeftWithPanel(view, true);
				//RelativePanel.SetAlignRightWithPanel(view, true);
			///	RelativePanel.SetAlignTopWithPanel(view, true);
		//		RelativePanel.SetAlignBottomWithPanel(view, true);
				
				view.Source = new Uri("https://www.crownofthegods.com");


            }
            catch (Exception e)
			{
				Log(e);
			}
            return view;



		}

        async private static void View_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            //if (WebViewPage.instance != null)
            //{
            //    WebViewPage.instance.Focus(FocusState.Programmatic);
            //    return;
            //}
            WebViewPage.DefaultUrl = args.Uri;
            await WindowManagerService.Current.TryShowAsStandaloneAsync<WebViewPage>("overview");
        }

        private static string GetJsString(string asm)
        {
            return new StreamReader((typeof(JSClient).Assembly).GetManifestResourceStream($"COTG.JS.{asm}.js") ).ReadToEnd();

        }
       
		private static void View_WebResourceRequested1(WebView sender, WebViewWebResourceRequestedEventArgs args)
        {
            try
            {
                var req = args.Request;

                //            Log(req.RequestUri.ToString());
                if (req.RequestUri.ToString().EndsWith("jquery/1.9.0/jquery.min.js"))
                {
                    var js = GetJsString("jquery");

                    var newContent = new Windows.Web.Http.HttpStringContent(js, Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/json");

                    args.Response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = newContent };

                }
                else if (req.RequestUri.ToString().Contains("/jsfunctions/phaser.js"))
                {
                 //   var js = GetJsString("phaser");

                     var newContent = new Windows.Web.Http.HttpStringContent("", Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/json");

                    args.Response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = newContent };

                }

                else if ( req.RequestUri.LocalPath.Contains("jsfunctions/game.js"))
                {
                    try
                    {
                       
                        view.WebResourceRequested -= View_WebResourceRequested1;
                        string host = args.Request.RequestUri.Host;
                        string uri = args.Request.RequestUri.AbsoluteUri;

                         //   var reqMsg = args.Request;
                         //   var respTask = httpClient.SendRequestAsync(reqMsg).AsTask();

                        var asm = typeof(JSClient).Assembly;

                        var js = GetJsString("funky") + GetJsString("J0EE");

                                var newContent = new Windows.Web.Http.HttpStringContent(js,Windows.Storage.Streams.UnicodeEncoding.Utf8,"text/json");

                        args.Response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = newContent };
                               // args.Response = resp;
                                //var response = await client.SendRequestAsync(reqMsg).AsTask();
                               // resp.Content = newContent;
                        //    }
                        



                    }
                    catch (Exception e)
                    {
                        Log(e);
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
                Log(e);
            }

        }

        public static void Refresh(object ob,RoutedEventArgs args)
        {
            if (view == null)
                return;
            Debug.Fatal();  // Todo
//            view.Refresh();
//            Services.NavigationService.Navigate<Views.MainPage>();
        }

        public static void SendChat(int channel,string message )
        {
            try
            {

                view.InvokeScriptAsync("sendchat", new string[] { channel.ToString(),message });


            }
            catch (Exception e)
            {
                Log(e);
            }

        }

        public static void ViewCity(int cityId)
        {
            try
            {
                if (City.IsMine(cityId))
                {
                        City.SetFocus(cityId, false, true, false);

                    view.InvokeScriptAsync("viewcity", new string[] { (cityId).ToString() });
                }
                else
                {
                    ShowCity(cityId, false);
                }

            }
            catch (Exception e)
            {
                Log(e);
            }

        }
        public static void ChangeCity(int cityId)
        {
            try
            {
                if (City.IsMine(cityId))
                {
                    City.build = City.GetOrAddCity(cityId);
                    City.SetFocus(cityId, false, true, false);

                    view.InvokeScriptAsync("chcity", new string[] { (cityId).ToString() });
                }
                else
                {
                    ShowCity(cityId, false);
                }

            }
            catch (Exception e)
            {
                Log(e);
            }

        }
        public static void ChangeView(bool cityView)
        {
            try
            {
                view.InvokeScriptAsync("setviewmode", new string[] { cityView ? "c" : "w" });
             
            }
            catch (Exception e)
            {
                Log(e);
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
                //     view.InvokeScriptAsync("eval", new string[] { $"gspotfunct.infoPlay('{playerName}')" });
                     view.InvokeScriptAsync("infoPlay", new string[] {playerName});
            }
            catch (Exception e)
            {
                Log(e);
            }
        }
        public  static void ShowAlliance(string allianceName)
        {
            try
            {
                view.InvokeScriptAsync("alliancelink",new []{allianceName});
           //     view.InvokeScriptAsync("eval", new string[] { $"gspotfunct.alliancelink('{allianceName}')" });
            }
            catch (Exception e)
            {
                Log(e);
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
                Log(e);
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

        public static void ShowCity(int cityId, bool lazyMove)
        {
			try
			{
                if (City.IsMine(cityId))
                {
                    City.SetFocus(cityId, false, true, false);
                }

                if (JSClient.IsWorldView())
                    cityId.BringCidIntoWorldView(lazyMove);

                    view.InvokeScriptAsync("shCit", new string[] { (cityId).ToString() });
       //             if( City.IsMine(cityId)  )
       //                 Raiding.UpdateTSHome();
                
			}
			catch (Exception e)
			{
				Log(e);
			}


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
        public static async Task GetCitylistOverview()
        {

            var str = await view.InvokeScriptAsync("getppdt", null);
            ppdt = JsonDocument.Parse(str);
            UpdatePPDT(ppdt.RootElement);
        }

        public static void UpdatePPDT(JsonElement jse)
        {
            int clChanged = 0;
            // City lists
            try
            {
                List<CityList> lists = new List<CityList>();
                if (jse.TryGetProperty("cl", out var cityListNames))
                {
                    ++clChanged;
                    //  var clList = new List<string>();
                    foreach (var cn in cityListNames.EnumerateObject())
                    {
                        var l = new CityList() { name = cn.Value.GetString(), id = int.Parse(cn.Name) };
                        lists.Add(l);
                    }

                }
                if (jse.TryGetProperty("clc", out var cityListCities))
                {
                    ++clChanged;
                    foreach (var clc in cityListCities.EnumerateObject())
                    {
                        if (clc.Value.ValueKind == JsonValueKind.Null)
                            continue;
                        var id = int.Parse(clc.Name);
                        var cityList = lists.Find((a) => a.id == id);
                        foreach (var cityId in clc.Value.EnumerateArray())
                        {
                            cityList.cities.Add(cityId.GetInt32());

                        }
                    }

                }
                if (clChanged > 0)
                {
                    Log("Change2");
                    Assert(clChanged == 2);
                    CityList.selections.Clear();
                    CityList.selections.Add(CityList.allCities);
                    for (int i = 0; i < lists.Count; ++i)
                        CityList.selections.Add(lists[i]);

                    CityList.all = lists.ToArray();
                    AApp.DispatchOnUIThreadLow(() =>
                   {
                        // is this valid?
                        //   Log("Reset");

                        CityList.selections.NotifyReset();
                   });
                }
            }
            
        catch(Exception E)
        {
            Log(E);
            Log("City lists invalid, maybe you have none");
        }

            var cUpdated = false;
            // extract cities
            if (jse.TryGetProperty("c", out var cProp))
            {
                cUpdated = true;

                var now = DateTimeOffset.UtcNow;
                foreach (var jsCity in cProp.EnumerateArray())
                {
                    var cid = jsCity.GetProperty("1").GetInt32();

                    var city = City.GetOrAddCity(cid);
                    city.cityName = jsCity.GetProperty("2").GetString();
                    int i = city.cityName.IndexOf('-');
                    if(i!= -1)
                    {
                        city.remarks = city.cityName.Substring(i + 2);
                        city.cityName = city.cityName.Substring(0, i - 1);
                    }
                    city.tsTotal = jsCity.GetAsInt("8");
                    city.tsHome = jsCity.GetAsInt("17");
                    city.isCastle = jsCity.GetAsInt("12") > 0;
                    city.points =  (ushort)jsCity.GetAsInt("4");
                    
                    city.isOnWater = jsCity.GetAsInt("16") > 0;
                    city.isTemple = jsCity.GetAsInt("15") > 0;
                    city.pid = jsVars.pid;
                    

                }

            //    Log(City.all.ToString());
             //   Log(City.all.Count());
                Views.MainPage.CityListChange();
            }

            Log($"PPDT: c:{cUpdated}, clc:{clChanged}");

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
                Log($"Nav start {args.Uri} {args.Uri}");
                var match = urlMatch.Match(args.Uri.Host);

                if (match.Groups.Count == 2 && args.Uri.LocalPath == "/" )
                {
                    if (httpFilter != null)
                        Debug.Fatal();  // Todo
                    world = int.Parse(match.Groups[1].ToString());
                    try
                    {

                        httpsHostString = $"https://{args.Uri.Host}";
                        httpsHost = new Uri(httpsHostString);
                        downloadImageClient = new HttpClient();
                        downloadImageClient.DefaultRequestHeaders.Accept.TryParseAdd("image/png, image/svg+xml, image/*; q=0.8, */*; q=0.5");
                        downloadImageClient.DefaultRequestHeaders.Referer = httpsHost;
                        downloadImageClient.DefaultRequestHeaders.Host = new Windows.Networking.HostName(httpsHost.Host);
                        downloadImageClient.DefaultRequestHeaders.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");

                        httpFilter = new HttpBaseProtocolFilter();// HttpBaseProtocolFilter.CreateForUser( User.GetDefault());
                        httpFilter.AllowAutoRedirect = true;
//                        httpFilter.ServerCredential =
                      //  httpFilter.ServerCustomValidationRequested += HttpFilter_ServerCustomValidationRequested;
                        httpFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.Default;
                        httpFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                        httpFilter.CookieUsageBehavior = HttpCookieUsageBehavior.NoCookies;// HttpCookieUsageBehavior.Default;
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
 //                       httpFilter.AllowUI = true;
                        httpFilter.AutomaticDecompression = true;
                        httpFilter.MaxVersion = HttpVersion.Http20;

                        //                        httpFilter.User.

                        clientPool = new BlockingCollection<HttpClient>(clientCount);
                        for (int i = 0; i < clientCount; ++i)
                        {
                            var httpClient = new HttpClient( httpFilter); // reset
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
                            httpClient.DefaultRequestHeaders.Referer = new Uri(httpsHost, "/overview.php?s=0");// new Uri                                                       //             req.Headers.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
                            httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("pp-ss", jsVars.ppss.ToString());

                            httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
                            //   Log($"Built headers {httpClient.DefaultRequestHeaders.ToString() }");

                            //httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Pragma", "no-cache");

                            //httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Site", "same-origin");
                            //httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Mode", "cors");
                            //httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Dest", "empty");

                                clientPool.Add(httpClient);

                        }
                        clientPool.CompleteAdding();
                        view.WebResourceRequested -= View_WebResourceRequested1;
                        view.WebResourceRequested += View_WebResourceRequested1;


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

        static ConcurrentDictionary<string, BitmapImage> imageCache = new ConcurrentDictionary<string, BitmapImage>();
        public static BitmapImage GetImage(string dir,string name)
        {
            return ImageHelper.FromImages(name);
            //if (imageCache.TryGetValue(name, out var b))
            //    return b;
            //b = new BitmapImage();
            //imageCache.TryAdd(name, b);

            //await LoadImage(b,dir,name);
            //return b;
        }
        async static Task LoadImage(BitmapImage b,string dir,string name)
        {

            try
            {

                var uri = new Uri(httpsHost, dir+name);
                using (var response = await downloadImageClient.GetAsync(uri))
                {
                    response.EnsureSuccessStatusCode();
                    var buff = await response.Content.ReadAsBufferAsync();

                    var temp = new byte[buff.Length];

                    var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buff);
                     dataReader.ReadBytes(temp);

                    // Get the path to the app's Assets folder.
                  //  string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
                

                    // Get the folder object that corresponds to this absolute path in the file system.
                   // StorageFolder folder = await DownloadsFolder.CreateFolderAsync(@"\cotg");
                    var file = await DownloadsFolder.CreateFileAsync(name);
                    await FileIO.WriteBytesAsync(file, temp);


                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }

        }
        private static void HttpFilter_ServerCustomValidationRequested(HttpBaseProtocolFilter sender, HttpServerCustomValidationRequestedEventArgs args)
        {
            Log(args.ToString());
        }

        static private void View_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            
            Exception($"Internet failed, press any key to retry {e.Uri} {e.WebErrorStatus}");
            Log("Refresh");
            if (view!=null)
                view.Refresh();
        }

        static private void View_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            //Log($"Dom loaded {args.Uri}");
            //if (urlMatch.IsMatch(args.Uri.Host))
            //{
            //    Log("Match Regex!");
            //    await AddJSPluginAsync();
            //}
        }
        async static private void View_ScriptNotify(object sender, NotifyEventArgs e)
        {
            try
            {
                bool gotCreds = false;
                Log($"Notify: {e.Value.Length},{e.CallingUri},{sender}:{e.Value.Truncate(128) }");
                var jsDoc = JsonDocument.Parse(e.Value);
                var jsd = jsDoc.RootElement;
                foreach (var jsp in jsd.EnumerateObject())
                {
                    switch (jsp.Name)
                    {
                        case "jsvars":
                            {
                                var jso = jsp.Value;
                                jsVars.s = jso.GetString("s");
                                jsVars.token = jso.GetString("token");
                                var agent = jso.GetString("agent");
                                jsVars.cookie = jso.GetString("cookie");
                                {
                                    var clients = clientPool.ToArray();
                                foreach (var httpClient in clients)
                                {
                                    httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(agent);
                                    httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Cookie", "sec_session_id=" + jsVars.s);
                                }
                            }
                                var timeOffset = jso.GetAsInt64("timeoffset");
                                var timeOffsetRounded = Math.Round(timeOffset / (1000.0 * 60 * 30)  )*30.0f; // round to nearest half hour
                                jsVars.gameTOffset =TimeSpan.FromMinutes(timeOffsetRounded);
                                var str = timeOffsetRounded >= 0 ? " +" : " ";
                                str += $"{jsVars.gameTOffset.Hours:D2}:{jsVars.gameTOffset.Minutes:D2}";
                                JSONHelper.timeZoneString = str;
                             //   Log(JSONHelper.timeZoneString);
                                Log($"TOffset {jsVars.gameTOffset}");
                                Log(ServerTime().ToString());
                                jsVars.ppss = jso.GetAsInt("ppss");
                                jsVars.player = jso.GetString("player");
                                jsVars.pid = jso.GetAsInt("pid");
                               
                                var cid = jso.GetAsInt("cid");
                                City.build = City.focus = City.GetOrAddCity(cid);

                                ShellPage.cameraC = cid.CidToWorldV();
                                //Note.L("cid=" + cid.CidToString());
                                jsVars.gameMSAtStart = jso.GetAsInt64("time");
                                jsVars.launchTime = DateTimeOffset.UtcNow;
                                //    Log(jsVars.ToString());
                                ShellPage.webclientSpan.x = jso.GetAsInt("spanX");
                                ShellPage.webclientSpan.y = jso.GetAsInt("spanY");
                                ShellPage.clientTL.X = jso.GetAsFloat("left");
                                ShellPage.clientTL.Y = jso.GetAsFloat("top");
                                Log($"WebClient:{ShellPage.clientTL} {ShellPage.webclientSpan.y}");
                           //     Note.Show($" {clientSpanX}:{clientSpanY} {ShellPage.clientTL} ");
                                gotCreds = true;
                                //                                 Log($"Built heades {httpClient.DefaultRequestHeaders.ToString() }");

                                UpdatePPDT(jso.GetProperty("ppdt"));
                                break;
                            }
                        case "cityclick":
                            {
                                var jso = jsp.Value;
                                var cid = jso.GetAsInt("cid");
                                {
                                    var city =COTG.Views.SpotTab.TouchSpot(cid);
                                    
                                    city.cityName = jso.GetString("name");
                                    city.pid = Player.NameToId(jso.GetAsString("player")); // todo: this shoule be an int playerId
                                    //Assert(city.pid > 0);
                                    city.points = (ushort)jso.GetAsInt("score");
                                 //   city.alliance = jso.GetString("alliance"); // todo:  this should be an into alliance id
                                    city.lastAccessed = DateTimeOffset.UtcNow;
                                    city.isCastle = jso.GetAsInt("castle")==1;
                                    city.isBlessed = city.pid!=0 ? jso.GetAsInt("bless")!=0 : false;
                                    city.isOnWater = jso.GetAsInt("water") != 0;
                                    city.isTemple = jso.GetAsInt("plvl") != 0;


                                  //  Note.Show($"CityClick {city.cityName} {city.cid.CidToStringMD()}");
                                    if(IsWorldView())
                                    {
                                        // bring city into view
                                        cid.BringCidIntoWorldView(true);

                                    }
                                }
                                break;

                            }
                        case "citydata":
                            {
                                var jse = jsp.Value;
                               // var priorCid = cid;
                                var cid = jse.GetInt("cid");
                                if(!IsWorldView())
                                    ShellPage.cameraC = cid.CidToWorldV();

                                //Note.L("citydata=" + cid.CidToString());
                                var city =City.GetOrAddCity(cid);
                                City.build = city;
                                city.LoadFromJson(jse);

                                if (MainPage.IsVisible())
                                    ScanDungeons.Post(cid, false);

                                if (IsWorldView())
                                {
                                    // bring city into view
                                    cid.BringCidIntoWorldView(true);

                                }
                                break;

                            }
                        case "OGA":
                            {
                             //   Log(e.Value);
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
                                Alliance.Ctor(jsDoc);
                                

                                break;
                            }
                        case "gPlA":
                        {
                                Player.Ctor(jsp.Value);
                                break;
                            }
                        case "ppdt":
                            {
                                UpdatePPDT(jsp.Value);
                                break;
                            }
                        case "chat":
                            {
                                ChatTab.ProcessIncomingChat(jsp);

                                break;
                            }
                        case "c":
                            {
                                var jso = jsp.Value;
                                var cid = jso.GetInt("c");
                                City.build = City.GetOrAddCity(cid);
                                var popupCount = jso.GetAsInt("p");
                           //     Note.L("cid=" + cid.CidToString());
                                var priorView = viewMode;
                                viewMode = (ViewMode)jso.GetInt("v");
                                if(priorView!=viewMode )
                                {
                                    var isWorld = IsWorldView();
                                    AApp.DispatchOnUIThreadLow( () =>
                                    {
                                        ShellPage.canvas.IsHitTestVisible = isWorld;

                                    });
                                }
                                if (priorView != ViewMode.world || viewMode != ViewMode.world)
                                {
//                                    ShellPage.cameraZoom = jso.GetAsFloat("z");
//                                    ShellPage.cameraC = (new Vector2(jso.GetAsFloat("x"), jso.GetAsFloat("y")) - ShellPage.halfSpan - ShellPage.clientC) / ShellPage.cameraZoom;
//                                    ShellPage.cameraC.Y = jso.GetAsFloat("y") / ShellPage.cameraZoom;

                                 //   ChatTab.L(ShellPage.cameraC.ToString() + " s:" + ShellPage.cameraZoom + " v:" + viewMode);
                                    // if((viewMode & ViewMode.region)!=0)
                                 //   ShellPage.canvas?.Invalidate();
                                }
                                ShellPage.NotifyCotgPopup(popupCount);
//                                ShellPage.SetCanvasVisibility(noPopup);
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
                    await GetCitylistOverview();
                    City.UpdateSenatorInfo();  // no async
                    Raiding.UpdateTS(true);
                    TileData.Ctor();

                    // await RaidOverview.Send();
                }
                //var cookie = httpClient.DefaultRequestHeaders.Cookie;
                //cookie.Clear();
                //foreach (var c in jsVars.cookie.Split(";"))
                //{
                //    cookie.ParseAdd(c);
                //}





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
