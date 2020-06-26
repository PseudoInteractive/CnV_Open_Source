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

namespace COTG
{
    /// <summary>
    /// The j s client.
    /// </summary>
    public class JSClient
    {

          [Flags] public enum ViewMode  
            {
            city = 1,
            region=2,
            world=4
            };

        public static ViewMode viewMode;
        public static JsonDocument ppdt;
        public static int cid; // cityId
        public static JSClient instance = new JSClient();
        public static WebView view;
        static KeyboardAccelerator refreshAccelerator;
        static HttpBaseProtocolFilter httpFilter;
        public static HttpClient httpClient;
        public static HttpClient downloadImageClient;
        public static int world = 19;
        static Regex urlMatch = new Regex(@"^w(\d\d).crownofthegods.com$");
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
            public DateTime launchTime;
            public long gameMSAtSTart;
           
        };
       


        public static JSVars jsVars;

        public static long GameTime()
        {
            return (long)((DateTime.Now - jsVars.launchTime).TotalMilliseconds) + jsVars.gameMSAtSTart;
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
                { HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
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

				refreshAccelerator = new KeyboardAccelerator() { Key = Windows.System.VirtualKey.F5 };
				refreshAccelerator.Invoked += (_, __) => view?.Refresh();

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
            WebViewPage.DefaultUrl = args.Uri;
            await WindowManagerService.Current.TryShowAsStandaloneAsync("overview", typeof(WebViewPage));
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
               
                    
                if( req.RequestUri.LocalPath.Contains("jsfunctions/game.js"))
                {
                    try
                    {
                       
                            view.WebResourceRequested -= View_WebResourceRequested1;
                        string host = args.Request.RequestUri.Host;
                        string uri = args.Request.RequestUri.AbsoluteUri;

                            var reqMsg = args.Request;
                            var respTask = httpClient.SendRequestAsync(reqMsg).AsTask();

                        var asm = typeof(JSClient).Assembly;

                        var js = GetJsString("J0EE") +  GetJsString("game122") + GetJsString("funky");

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

            Log(str);
            ppdt = JsonDocument.Parse(str);
            // extract cities
            {
                var now = DateTime.Now;
                foreach (var jsCity in ppdt.RootElement.GetProperty("c").EnumerateArray())
                {
                    var cid = jsCity.GetProperty("1").GetInt32();

                    var city=City.all.GetOrAdd(cid,City.Factory);
                    
                    city.name = jsCity.GetProperty("2").GetString();
                    city.isCastle = jsCity.GetAsInt("12") != 0;
                    city.points =  (ushort)jsCity.GetAsInt("4");
                    city.lastUpdated = now;
                    city.isOnWater = jsCity.GetAsInt("16") != 0;
                    city.isTemple = jsCity.GetAsInt("15") != 0;
                    city.owner = jsVars.player;
                    city.alliance = jsVars.alliance;
                    

                }

                Log(City.all.ToString());
                Log(City.all.Count());
             }
             Views.MainPage.CityListChange();


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

                        httpsHost = new Uri($"https://{args.Uri.Host}");
                        downloadImageClient = new HttpClient();
                        downloadImageClient.DefaultRequestHeaders.Accept.TryParseAdd("image/png, image/svg+xml, image/*; q=0.8, */*; q=0.5");
                        downloadImageClient.DefaultRequestHeaders.Referer = httpsHost;
                        downloadImageClient.DefaultRequestHeaders.Host = new Windows.Networking.HostName(httpsHost.Host);
                        downloadImageClient.DefaultRequestHeaders.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");

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

                        
                        httpClient = new HttpClient(httpFilter); // reset
                                                                 //   httpClient = new HttpClient(); // reset
                                                                 //                        var headers = httpClient.DefaultRequestHeaders;
                                                                 //     headers.TryAppendWithoutValidation("Content-Type",@"application/x-www-form-urlencoded; charset=UTF-8");
                                                                 // headers.TryAppendWithoutValidation("Accept-Encoding","gzip, deflate, br");
                                                                 //                        headers.TryAppendWithoutValidation("X-Requested-With", "XMLHttpRequest");
                                                                 //    headers.Accept.TryParseAdd(new HttpMediaTypeHeaderValue(@"application/json"));
                                                                 //   headers.Add("Accept", @"*/*");
                        httpClient.DefaultRequestHeaders.AcceptLanguage.TryParseAdd("en-US,en;q=0.5");
                    //    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(@"Mozilla/5.0 (Windows NT 10.0; Win64; x64; WebView/3.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.19631");
                        //    httpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Credentials", "true");
                        httpClient.DefaultRequestHeaders.Accept.TryParseAdd("*/*");
                        // httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("X-Requested-With", "XMLHttpRequest");
                    //    httpClient.DefaultRequestHeaders.Referer = new Uri(httpsHost, "/overview.php?s=0");// new Uri($"https://w{world}.crownofthegods.com");
                          httpClient.DefaultRequestHeaders.Referer = new Uri(httpsHost, "/overview.php?s=0");// new Uri                                                       //             req.Headers.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
                        httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("pp-ss", jsVars.ppss.ToString());

                        httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
                        Log($"Built heades {httpClient.DefaultRequestHeaders.ToString() }");

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

        static private async void View_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            
            Exception($"Internet failed, press any key to retry {e.Uri} {e.WebErrorStatus}");
            Log("Refresh");
            if (view!=null)
                view.Refresh();
        }

        static async private void View_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
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
              //  Log($"Notify: {e.CallingUri} {sender} {e.Value.Truncate(128) }");
                var jsDoc = JsonDocument.Parse(e.Value);
                var jsd = jsDoc.RootElement;
                foreach (var jsp in jsd.EnumerateObject())
                {
                    switch (jsp.Name)
                    {
                        case "jsvars":
                            {
                                var jso = jsp.Value;
                                jsVars.token = jso.GetString("token");
                                var agent = jso.GetString("agent");
                                httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(agent);
                                jsVars.ppss = jso.GetAsInt("ppss");
                                jsVars.player = jso.GetString("player");
                                jsVars.pid = jso.GetAsInt("pid");
                                jsVars.alliance = jso.GetString("alliance");
                                jsVars.s = jso.GetString("s");
                                cid = jso.GetAsInt("cid");
                                jsVars.gameMSAtSTart = jso.GetAsInt64("time");
                                jsVars.launchTime = DateTime.Now;
                                Log(System.Text.Json.JsonSerializer.Serialize(jsVars));
                                ShellPage.clientSpan.X = jso.GetAsFloat("spanX");
                                ShellPage.clientSpan.Y = jso.GetAsFloat("spanY");
                                ShellPage.clientTL.X = jso.GetAsFloat("left");
                                ShellPage.clientTL.Y = jso.GetAsFloat("top");

                                Log($" {ShellPage.clientSpan} {ShellPage.clientTL} ");
                                gotCreds = true;
//                                 Log($"Built heades {httpClient.DefaultRequestHeaders.ToString() }");

                                
                                break;
                            }
                        case "cityclick":
                            {
                                var jso = jsp.Value;
                                var cid = jso.GetAsInt("cid");
                                {
                                    var city=City.all.GetOrAdd(cid,City.Factory);
                                    
                                    city.name = jso.GetString("name");
                                    city.owner = jso.GetString("player"); // todo: this shoule be an int playerId
                                    city.notes = jso.GetString("notes");
                                    city.points = (ushort)jso.GetAsInt("score");
                                    city.alliance = jso.GetString("alliance"); // todo:  this should be an into alliance id
                                    city.lastAccessed = DateTime.Now;
                                    COTG.Views.MainPage.CityChange(city);

                                    Note.Show($"{city.name} {city.cid.ToCoordinateMD()}");
                                }
                                break;

                            }
                        case "citydata":
                            {
                                MainPage.ClearDungeonList();
                                var jse = jsp.Value;
                                cid = jse.GetInt("cid");
                                var city=City.all.GetOrAdd(cid,City.Factory);
                                city.LoadFromJson(jse);
                                break;
                            }
                        case "OGA":
                            {
                                Log(e.Value);
                                break;
                            }
                        case "OGR":
                            {
                                Log(e.Value);
                                break;
                            }
                        case "OGT":
                            {
                                Log(e.Value);
                                break;
                            }
                        case "c":
                            {
                                var jso = jsp.Value;
                                cid = jso.GetInt("c");
                                viewMode = (ViewMode)jso.GetInt("v");
                                ShellPage.cameraC.X = jso.GetFloat("x");
                                ShellPage.cameraC.Y = jso.GetFloat("y");
                                ShellPage.T(ShellPage.cameraC.ToString() + " v:" + viewMode);
                               // if((viewMode & ViewMode.region)!=0)
                                    ShellPage.canvas?.Invalidate();
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
    }
}
