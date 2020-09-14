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
using static COTG.Game.Enum;

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

//        public static JsonDocument ppdt;
        public static JSClient instance = new JSClient();
        public static WebView view;
        static HttpBaseProtocolFilter httpFilter;
        const int clientCount = 6;
        public static ConcurrentBag<HttpClient> clientPool;
        static HttpClient _downloadImageClient;
        public static HttpClient downloadImageClient { get
            {
                if (_downloadImageClient == null)
                {
                    _downloadImageClient = new HttpClient();
                    _downloadImageClient.DefaultRequestHeaders.Accept.TryParseAdd("image/png, image/svg+xml, image/*; q=0.8, */*; q=0.5");
                    _downloadImageClient.DefaultRequestHeaders.Referer = httpsHost;
                    _downloadImageClient.DefaultRequestHeaders.Host = new Windows.Networking.HostName(httpsHost.Host);
                    _downloadImageClient.DefaultRequestHeaders.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
                }
                return _downloadImageClient;
            }
        }
        public static HttpClient _translatorClient;
        public static HttpClient translatorClient
        {
            get {
                if(_translatorClient==null)
                {
                    _translatorClient = new HttpClient();
                    
                }
                return _translatorClient;
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
        public struct JSVars
        {
            public string token { get; set; }
            public int ppss { get; set; }
            public string s { get; set; }
            public string cookie { get; set; }
            public DateTimeOffset launchTime;
            public long gameMSAtStart;
            public TimeSpan gameTOffset;

            public override string ToString()
            {
                return $"{{{nameof(token)}={token}, {nameof(ppss)}={ppss.ToString()},   {nameof(s)}={s}, {nameof(cookie)}={cookie}}}";
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
        public static DateTimeOffset ToServerTime(DateTime time)
        {
            return (time.ToUniversalTime() + jsVars.gameTOffset);
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
                    CacheMode=new BitmapCache()
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
                if (subId != 0)
                {
                    httpsHost = new Uri($"https://w{world}.crownofthegods.com");
             //       view.Source = new Uri($"https://w{world}.crownofthegods.com?s={subId}");
                }
               // else
                    view.Source = new Uri("https://www.crownofthegods.com");
                if (subId != 0)
                {
                    Task.Delay(1000).ContinueWith(_ =>
                    {
                        App.DispatchOnUIThread(() => view.Source = new Uri($"https://w{world}.crownofthegods.com?s={subId}"));
                    });
                }

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
            Trace(args.Uri.ToString());
            Trace(args.Uri.Host);
            Trace(httpsHost.Host);
            if (args.Uri.Host == httpsHost.Host)
            {
                WebViewPage.DefaultUrl = args.Uri;
                await WindowManagerService.Current.TryShowAsStandaloneAsync<WebViewPage>("overview");
            }
            else
            {
                Launcher.LaunchUriAsync(args.Uri);
            }
        }

        private static string GetJsString(string asm)
        {
            return new StreamReader((typeof(JSClient).Assembly).GetManifestResourceStream($"COTG.JS.{asm}") ).ReadToEnd();

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
                else if (req.RequestUri.ToString().EndsWith("jquery/1.9.0/jquery.min.js"))
                {
                    var js = GetJsString("jquery");

                    var newContent = new Windows.Web.Http.HttpStringContent(js, Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/json");

                    args.Response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = newContent };

                }
                //else if (req.RequestUri.ToString().EndsWith("index.html"))
                //{
                //    Assert(false);
                //    var js = GetJsString("jquery");

                //    var newContent = new Windows.Web.Http.HttpStringContent(js, Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/json");

                //    args.Response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = newContent };

                //}
                else if (req.RequestUri.ToString().Contains("/jsfunctions/phaser.js"))
                {
                 //   var js = GetJsString("phaser");

                     var newContent = new Windows.Web.Http.HttpStringContent("", Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/json");

                    args.Response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = newContent };

                }
                else if (req.RequestUri.ToString().Contains("/jsfunctions/pack.js"))
                {
                    var js = GetJsString("pack");

                    var newContent = new Windows.Web.Http.HttpStringContent(js, Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/json");

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

                        var js = GetJsString("funky")
                               + GetJsString("J0EE");
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
                const int div = 220;
                for(; ;)
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
                        if(sp == -1)
                        {
                            Assert(false);
                            break;
                        }
                        remainder = message.Substring(0, sp+1) + remainder;
                    }
                   
                    message = remainder;
                }


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
                    SetViewModeCity();
                    var city = City.StBuild(cityId);
                  //  city.SetFocus( false, true, false);

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

        public static void ChangeCity(int cityId, bool lazyMove)
        {
            try
            {
                if (City.IsMine(cityId))
                {
                    var city = City.StBuild(cityId);
                    if(!lazyMove)
                        cityId.BringCidIntoWorldView(lazyMove);
                    App.DispatchOnUIThreadSneaky(() =>
                        view.InvokeScriptAsync("chcity", new string[] { (cityId).ToString() }));
                }
                else
                {
                    ShowCity(cityId, lazyMove);
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
                if (cityView)
                    SetViewModeCity();
                else
                    SetViewModeWorld();
                App.DispatchOnUIThreadSneaky(() => view.InvokeScriptAsync("setviewmode", new string[] { cityView ? "c" : "w" }));
             
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
                App.CopyTextToClipboard(playerName);
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

        public static async void ShowCity(int cityId, bool lazyMove)
        {
			try
			{
                SetViewModeWorld();
               // if (City.IsMine(cityId))
                {
                    City.SetFocus(cityId);
                }

                // if (JSClient.IsWorldView())
                cityId.BringCidIntoWorldView(lazyMove);

                App.DispatchOnUIThreadSneaky(() =>  
                    view.InvokeScriptAsync("shCit", new string[] { (cityId).ToString() }));
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
        //public static async Task GetCitylistOverview()
        //{

        //    var str = await view.InvokeScriptAsync("getppdt", null);
        //    ppdt = JsonDocument.Parse(str);
        //    UpdatePPDT(ppdt.RootElement);
        //}

        public static async Task PollCity(int cid)
        {
            await Task.Delay(50);
            await view.InvokeScriptAsync("pollthis", new[] { cid.ToString() } );
            await Task.Delay(400); // hack:  Todo, handle this property
            await view.InvokeScriptAsync("pollthis", new[] { cid.ToString() });
            await Task.Delay(300); // hack:  Todo, handle this property
        }
        static readonly float[] researchRamp = { 0, 1, 3, 6, 10, 15, 20, 25, 30, 35, 40, 45, 50 };

        private static void BonusesUpdated()
        {
            cartTravel = 10.0f / ( 1+ faith.merius*0.5f/100f + (researchRamp[research[28]]) / 100f );
            shipTravel = 5.0f / (1 + faith.merius * 0.5f / 100f + (researchRamp[research[27]]) / 100f);

            ttSpeedBonus[0] = 1; // no speed reserach for guard
            ttSpeedBonus[1] = 1 + (faith.domdis) * 0.5f / 100 + (researchRamp[research[12]]) / 100;
            ttSpeedBonus[2] = 1 + (faith.ibria) * 0.5f / 100 + (researchRamp[research[8]]) / 100;
            ttSpeedBonus[3] = 1 + (faith.ibria) * 0.5f / 100 + (researchRamp[research[8]]) / 100;
            ttSpeedBonus[4] = 1 + (faith.ibria) * 0.5f / 100 + (researchRamp[research[8]]) / 100;
            ttSpeedBonus[5] = 1 + (faith.ibria) * 0.5f / 100 + (researchRamp[research[8]]) / 100;
            ttSpeedBonus[6] = 1 + (faith.ibria) * 0.5f / 100 + (researchRamp[research[8]]) / 100;
            ttSpeedBonus[7] = 1 + (faith.ibria) * 0.5f / 100 + (researchRamp[research[11]]) / 100;
            ttSpeedBonus[8] = 1 + (faith.ibria) * 0.5f / 100 + (researchRamp[research[9]]) / 100;
            ttSpeedBonus[9] = 1 + (faith.ibria) * 0.5f / 100 + (researchRamp[research[9]]) / 100;
            ttSpeedBonus[10] = 1 + (faith.ibria) * 0.5f / 100 + (researchRamp[research[9]]) / 100;
            ttSpeedBonus[11] = 1 + (faith.ibria) * 0.5f / 100 + (researchRamp[research[9]]) / 100;
            ttSpeedBonus[12] = 1 + (faith.domdis) * 0.5f / 100 + (researchRamp[research[12]]) / 100;
            ttSpeedBonus[13] = 1 + (faith.domdis) * 0.5f / 100 + (researchRamp[research[12]]) / 100;
            ttSpeedBonus[14] = 1 + (faith.domdis) * 0.5f / 100 + (researchRamp[research[13]]) / 100;
            ttSpeedBonus[15] = 1 + (faith.domdis) * 0.5f / 100 + (researchRamp[research[13]]) / 100;
            ttSpeedBonus[16] = 1 + (faith.domdis) * 0.5f / 100 + (researchRamp[research[13]]) / 100;
            ttSpeedBonus[17] = 1 + (faith.domdis) * 0.5f / 100 + (researchRamp[research[14]]) / 100;


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
        public static void UpdatePPDT(JsonElement jse)
        {
            int clChanged = 0;
            // City lists
            try
            {
                bool bonusesUpdated = false;
                // research?
                if(jse.TryGetProperty("rs", out var rss))
                {
                    foreach (var rs in rss.EnumerateObject())
                    {
                        var id = int.Parse(rs.Name);
                        if (id < researchCount)
                        {
                            research[id]= (byte)rs.Value.GetInt("l"); // this will wrap for senator level (research not supported here)
                                                                       // the rest are 0..12
                        }
                    }
                    bonusesUpdated=true;

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
                    lists.Sort((a, b) => a.name.CompareTo(b.name));

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
                            foreach (var cityId in clc.Value.EnumerateArray())
                            {
                                cityList.cities.Add(cityId.GetInt32());

                            }
                        }
                    }

                }
                if (clChanged == 2 )
                {
                    App.DispatchOnUIThreadLow(() =>
                   {
                       var priorIndex = CityList.box.SelectedIndex;
                       CityList.selections = new CityList[lists.Count+1];
                       CityList.selections[0]=(CityList.allCities);
                       CityList hubs = null;
                       for (int i = 0; i < lists.Count; ++i)
                       {
                           CityList.selections[i + 1] = (lists[i]);
                       }
                       CityList.all = lists.ToArray();

                       CityList.box.ItemsSource =  CityList.selections;
                       CityList.box.SelectedIndex = priorIndex; // Hopefully this is close enough
//                       SettingsPage.instance.
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
 //               Note.Show("Pre PPDT");

                var now = DateTimeOffset.UtcNow;
                foreach (var jsCity in cProp.EnumerateArray())
                {
                    var cid = jsCity.GetProperty("1").GetInt32();

                    var city = City.GetOrAddCity(cid);
                    city._cityName = jsCity.GetProperty("2").GetString();
                    int i = city.cityName.IndexOf('-');
                    if(i!= -1)
                    {
                        city.remarks = city.cityName.Substring(i + 2);
                        city._cityName = city.cityName.Substring(0, i - 1);
                    }
                    city.tsTotal = jsCity.GetAsInt("8");
                    city.tsHome = jsCity.GetAsInt("17");
                    city.isCastle = jsCity.GetAsInt("12") > 0;
                    city.points =  (ushort)jsCity.GetAsInt("4");
                    
                    city.isOnWater |= jsCity.GetAsInt("16") > 0;  // Use Or in case the data is imcomplete or missing, in which case we get it from world data, if that is not incomplete or missing ;)
                    city.isTemple = jsCity.GetAsInt("15") > 0;
                    city.pid = Player.myId;
                    

                }

                //    Log(City.all.ToString());
                //   Log(City.all.Count());
                CityList.SelectedChange();

            }
            City.CheckTipRaiding();

            // Log($"PPDT: c:{cUpdated}, clc:{clChanged}");

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

                if (match.Groups.Count == 2 && (args.Uri.LocalPath == "/" || args.Uri.Fragment.Contains('&')))
                {
                    if (httpFilter != null)
                        Debug.Fatal();  // Todo
                    world = int.Parse(match.Groups[1].ToString());
                    try
                    {

                        httpsHostString = $"https://{args.Uri.Host}";
                        httpsHost = new Uri(httpsHostString);
                        

                        httpFilter = new HttpBaseProtocolFilter();// HttpBaseProtocolFilter.CreateForUser( User.GetDefault());
                     //   httpFilter.AllowAutoRedirect = true;
//                        httpFilter.ServerCredential =
                      //  httpFilter.ServerCustomValidationRequested += HttpFilter_ServerCustomValidationRequested;
                        httpFilter.CacheControl.ReadBehavior = HttpCacheReadBehavior.NoCache;
                        httpFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                        if (subId == 0)
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
                       // httpFilter.AutomaticDecompression = true;
                        httpFilter.MaxVersion = HttpVersion.Http20;

                        //                        httpFilter.User.

                        clientPool = new ConcurrentBag<HttpClient>();
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
                            httpClient.DefaultRequestHeaders.Referer = new Uri(httpsHost, $"/overview.php?s={subId}");// new Uri                                                       //             req.Headers.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
                            httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("pp-ss", subId.ToString());

                            httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Origin", $"https://w{world}.crownofthegods.com");
                            //   Log($"Built headers {httpClient.DefaultRequestHeaders.ToString() }");

                            //httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Pragma", "no-cache");

                            //httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Site", "same-origin");
                            //httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Mode", "cors");
                            //httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Sec-Fetch-Dest", "empty");

                                clientPool.Add(httpClient);

                        }
                      //  clientPool.CompleteAdding();
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
        static private void View_ScriptNotify(object sender, NotifyEventArgs __e)
        {
            var eValue = __e.Value;
            var eCallingUri = __e.CallingUri;
            Task.Run(() =>
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
                                    var jso = jsp.Value;
                                    jsVars.s = jso.GetString("s");
                                    jsVars.token = jso.GetString("token");
                                    ScanDungeons.secret = jso.GetString("raid");
                                    var agent = jso.GetString("agent");
                                    jsVars.cookie = jso.GetString("cookie");

                                    {
                                        //    var clients = clientPool.ToArray();
                                        foreach (var httpClient in clientPool)
                                        {
                                            httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(agent);
                                            if(subId == 0)
                                                httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Cookie", "sec_session_id=" + jsVars.s);
                                        }
                                    }
                                    var timeOffset = jso.GetAsInt64("timeoffset");
                                    var timeOffsetRounded = Math.Round(timeOffset / (1000.0 * 60 * 30)) * 30.0f; // round to nearest half hour
                                    jsVars.gameTOffset = TimeSpan.FromMinutes(timeOffsetRounded);
                                    var str = timeOffsetRounded >= 0 ? " +" : " ";
                                    str += $"{jsVars.gameTOffset.Hours:D2}:{jsVars.gameTOffset.Minutes:D2}";
                                    Helpers.JSON.timeZoneString = str;
                                    //   Log(JSONHelper.timeZoneString);
                                    Log($"TOffset {jsVars.gameTOffset}");
                                    Log(ServerTime().ToString());
                                    jsVars.ppss = jso.GetAsInt("ppss");
                                    Player.myName = jso.GetString("player");
                                    Player.myId = jso.GetAsInt("pid");

                                    var cid = jso.GetAsInt("cid");
                                    City.build = City.focus = cid;
                                    NavStack.Push(cid);
                                    App.DispatchOnUIThreadLow(() => ShellPage.instance.coords.Text = cid.CidToString() );
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
                                    //    Log($"Built heades {httpClient.DefaultRequestHeaders.ToString() }");

                                    //   UpdatePPDT(jso.GetProperty("ppdt"));
                                 

                                    break;
                                }
                            case "error":
                                {
                                    var msg = jsp.Value.GetString();
                                    Note.Show(msg);
                                    break;
                                }
                            case "sub":
                                {
                                    App.DispatchOnUIThread( ()=>Launcher.LaunchUriAsync(new Uri($"cotg:launch?w={world}&s=1")));
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
                                                        //Assert(city.pid > 0);
                                        city.points = (ushort)jso.GetAsInt("score");
                                        //   city.alliance = jso.GetString("alliance"); // todo:  this should be an into alliance id
                                        city.lastAccessed = DateTimeOffset.UtcNow;
                                        // city.isCastle = jso.GetAsInt("castle") == 1;
                                        city.isBlessed = city.pid > 0 ? jso.GetAsInt("bless") > 0 : false;
                                        city.isOnWater |= jso.GetAsInt("water") != 0;  // Use Or in case the data is imcomplete or missing, in which case we get it from world data, if that is not incomplete or missing ;)
                                        city.isTemple = jso.GetAsInt("plvl") != 0;


                                        cid.BringCidIntoWorldView(true);
                                        if (city._cityName != name)
                                        {
                                            city._cityName = name;
                                            if (cid == Spot.focus)
                                                App.DispatchOnUIThreadLow(() => ShellPage.instance.focus.Content = city.nameAndRemarks);
                                        }
                                    }
                                    break;

                                }
                            



                            case "citydata":
                                {
                                    var jse = jsp.Value;
                                    // var priorCid = cid;
                                    var cid = jse.GetInt("cid");
                                    if (!IsWorldView())
                                        ShellPage.cameraC = cid.CidToWorldV();
                                    var isFromTs = jse.TryGetProperty("ts", out _);
                                    //Note.L("citydata=" + cid.CidToString());
                                    var city =  City.GetOrAddCity(cid);
                                    city.LoadFromJson(jse);


                                    if (isFromTs && MainPage.IsVisible())
                                    {
                                     //   if (jse.TryGetProperty("ts", out _))
                                      //  {
                                            ScanDungeons.Post(cid, false);
                                      //  }
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


                                    break;
                                }
                            case "gPlA":
                                {
                                    Player.Ctor(jsp.Value);
                                    
                                    GetWorldInfo.Send();
                                    break;
                                }
                            case "ppdt":
                                {
                                    UpdatePPDT(jsp.Value);
                                    break;
                                }
                            case "chat":
                                {
                                    App.DispatchOnUIThreadLow(() => ChatTab.ProcessIncomingChat(jsp) );

                                    break;
                                }
                            case "chatin":
                                App.DispatchOnUIThreadLow(() => ChatTab.PasteToChatInput(jsp.Value.GetString()));
                                break;

                            case "c":
                                {
                                    var jso = jsp.Value;
                                    var cid = jso.GetInt("c");
                                    //City.StBuild(cid);
                                    var popupCount = jso.GetAsInt("p");
                                    //     Note.L("cid=" + cid.CidToString());
                                    SetViewMode((ViewMode)jso.GetInt("v"));

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
                        ///                   await GetCitylistOverview();
                        City.UpdateSenatorInfo();  // no async
                        Raiding.UpdateTS(true,true);
                        TileData.Ctor();
                        if (TipsSeen.instance.refresh == false
                        ||TipsSeen.instance.chat0==false
                        || TipsSeen.instance.chat1 == false
                        || TipsSeen.instance.chat2 == false)
                            App.QueueIdleTask(ShellPage.ShowTipRefresh);
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
            });
        }

        public static void SetViewMode(ViewMode _viewMode)
        {
            var priorView = viewMode;
            viewMode = _viewMode;
            if (priorView != viewMode)
            {
                var isWorld = IsWorldView();
                App.DispatchOnUIThreadLow(() =>
                {
                    ShellPage.canvas.IsHitTestVisible = isWorld;

                });
            }
        }

        public static void SetViewModeCity() => SetViewMode(ViewMode.city);
        public static void SetViewModeWorld() => SetViewMode(ViewMode.world);


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
