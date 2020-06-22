using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.Web.Http.Filters;
using static COTG.Debug;
using System.Web;
using COTG.Game;
using COTG.Helpers;

namespace COTG.Services
{
    public class RestAPI
    {
        public static List<RestAPI> all = new List<RestAPI>();
        public string localPath;
        public string secret;
        public static JsonDocument emptyJson;

        public RestAPI(string _localPath, string _secret)
        {
            localPath = _localPath;
            secret = _secret;
            emptyJson = JsonDocument.Parse("{}");
            all.Add(this);
        }


        public virtual async Task<JsonDocument> Accept(Uri uri, HttpResponseMessage resp)
        {

            try
            {
                var buff = await resp.Content.ReadAsBufferAsync();

                var temp = new byte[buff.Length];

                using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buff))
                {
                    dataReader.ReadBytes(temp);
                }
                Log(uri.ToString() + "\n\n>>>>>>>>>>>>>>\n\n" + Encoding.UTF8.GetString(temp) + "\n\n>>>>>>>>>>>>>>\n\n");
                var json = JsonDocument.Parse(temp);
                ProcessJson(json);
                return json;
            }
            catch (Exception e)
            {
                Log(e);
                return emptyJson;
            }

        }

        public virtual void ProcessJson(JsonDocument json)
        {

            //if (json.RootElement.TryGetProperty("a", out var a))
            //{
            //    var s = a.GetString();
            //    if (secret != null)
            //    {
            //        var raw = COTG.Aes.Decode(s, secret);
            //        Log(raw);
            //    }
            //}
        }
        public virtual string GetPostContent()
        {
            return "a=error";
        }
        async public static Task HandleResponse(Uri uri, HttpResponseMessage resp)
        {
            var localPath = uri.LocalPath;
            foreach (var h in all)
            {
                if (uri.LocalPath.Contains(h.localPath))
                {

                    try
                    {
                        await h.Accept(uri, resp); // continue processing if appropriate
                    }
                    catch (Exception e)
                    {
                        Log(e);
                    }

                }

            }

        }

        async public Task<JsonDocument> Post()
        {
            try
            {


                //            using var req  =anyPost;
                var req = new HttpRequestMessage(HttpMethod.Post, new Uri(JSClient.httpsHost, localPath));
                // req.TransportInformation.ver
                //req.AllowAutoRedirect = true;
                req.Content = new HttpStringContent((GetPostContent()),

                            Windows.Storage.Streams.UnicodeEncoding.Utf8,

                                                        "application/x-www-form-urlencoded");

                req.Content.Headers.TryAppendWithoutValidation("Content-Encoding", JSClient.jsVars.token);

                var resp = await JSClient.httpClient.TrySendRequestAsync(req, HttpCompletionOption.ResponseContentRead);
                Log($"res: {resp.GetType()} {resp.Succeeded} {resp}");
                Log($"req: {resp.RequestMessage.ToString()}");
                if (resp.ExtendedError != null)
                    Log(resp.ExtendedError);
                if (resp.Succeeded)
                {
                    return await Accept(req.RequestUri, resp.ResponseMessage);
                    //var b = await resp.Content.ReadAsInputStreamAsync();

                    //                    jso = await JsonDocument.ParseAsync(b.ToString);

                    // Log(b.ToString());
                }
                else
                {

                };
            }
            catch (Exception e)
            {
                Log(e);
            }
            return emptyJson;


        }

        static RestAPI __0 = new RestAPI("includes/sndRad.php", "Sx23WW99212375Daa2dT123ol");
        static RestAPI __2 = new RestAPI("includes/gRepH2.php", "g3542RR23qP49sHH");
        static RestAPI __3 = new RestAPI("includes/bTrp.php", "X2UsK3KSJJEse2");
        public static RestAPI getCity = new gC();
        public static rMp regionView = new rMp();
        static RestAPI __6 = new RestAPI("includes/gSt.php", "X22x5DdAxxerj3");
        public static gWrd getWorldInfo = new gWrd();
        static RestAPI __8 = new RestAPI("includes/UrOA.php", "Rx3x5DdAxxerx3");
        static RestAPI __9 = new RestAPI("includes/sndTtr.php", "JJx452Tdd2375sRAssa");
        // "fCv.php"  cid:cid (unencrptypted) "Xs4b2261f55dlme55s"
        // public static ScanRaids ScanDungeons = new ScanRaids();
        public static TroopsOverview troopsOverview = new TroopsOverview();
    }

    public class rMp : RestAPI
    {
        public rMp() : base("includes/rMp.php", "X22ssa41aA1522")
        {

        }
        public override string GetPostContent()
        {
            var encoded = Aes.Encode("[249]", secret);
            var args = "a=" + HttpUtility.UrlEncode(encoded, Encoding.UTF8);
            return args;


        }
    }

    public class gWrd : RestAPI
    {
        public gWrd() : base("includes/gWrd.php", "Addxddx5DdAxxer569962wz")
        {
        }

        public override string GetPostContent()
        {
            // this	{"a":"worldButton","b":"block","c":true,"d":1591969039987,"e":"World"}
            //      {"a":"worldButton","b":"block","c":true,"d":1591988862914,"e":"World"}
            var json = $"{{\"a\":\"worldButton\",\"b\":\"block\",\"c\":true,\"d\":{JSClient.GameTime()},\"e\":\"World\"}}";
            var encoded = Aes.Encode(json, secret);
            var args = "a=" + HttpUtility.UrlEncode(encoded, Encoding.UTF8);
            //"a=JwHt8WTz416hj%2FsCxccQzDNR47ebTllFGQq957Pigc%2BEb8EHJKNoVgVKQeNu2a4xi9Tx1vFxsUxw9WxRTuPLsey5mcvlVcftThXU4gA9";
            return args;
        }

        public override void ProcessJson(JsonDocument json)
        {
            World.UpdateCurrent(json);
        }

        //async public void Post2()
        //{
        //    var a = await JSClient.view.InvokeScriptAsync("avapost", new string[] { "includes/gWrd.php",
        //        "a=tgLyUZYF5F6ynQjCp3FXJOZ6ElUHXPUygineE33LuF2eDHwB%2FWH8MWY%2FA2CM%2FIra7fwRRCRKZzB1BMW826w6Cq2jSWL6%2FH64owys4lIv" });
        //    Log(a);
        //}
    }

    public class gC : RestAPI
    {
        public gC() : base("includes/gC.php", "X2U11s33S2375ccJx1e2")
        {

        }

        public override string GetPostContent()
        {
            var encoded = Aes.Encode(JSClient.cid.ToString(), secret);
            var args = "a=" + HttpUtility.UrlEncode(encoded, Encoding.UTF8);
            return args;
        }

        public override void ProcessJson(JsonDocument json)
        {
            var cid = json.RootElement.GetAsInt("cid");
            Log("Got JS " + cid);
            if (!City.all.TryGetValue(cid, out var city))
            {
                city = new City() { cid = cid };
                City.all.TryAdd(cid, city);
            }

            city.LoadFromJson(json.RootElement);

        }

    }


    public class sndRaid : RestAPI
    {
        public string json;
        public int cid;
        public sndRaid(string _json, int _cid) : base("includes/sndRaid.php", "XTR977sW56996sss2x2")
        {
            Log($"sndRaid:{_json}");
            cid = _cid;
            json = _json;

        }
        public override string GetPostContent()
        {
            var encoded = Aes.Encode(json, secret);
            var args = $"cid={cid}&a=" + HttpUtility.UrlEncode(encoded, Encoding.UTF8);
            return args;
        }

        public override void ProcessJson(JsonDocument json)
        {
            Log("Sent raid");
        }

    }

    public class ScanDungeons : RestAPI
    {
        City city;

        public ScanDungeons(City _cid) : base("includes/fCv.php", "Xs4b2261f55dlme55s")
        {

            city = _cid;
        }

        public override string GetPostContent()
        {
            var args = "cid=" + city.cid;
            return args;
        }

        public override async Task<JsonDocument> Accept(Uri uri, HttpResponseMessage resp)
        {
            Log("Got fCv");


            try
            {
                var buff = await resp.Content.ReadAsBufferAsync();

                var temp = new byte[buff.Length - 1];

                using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buff))
                {
                    dataReader.ReadByte(); // for some reason, the first two are '\n'
                    dataReader.ReadBytes(temp);
                }
                var dec2 = Encoding.UTF8.GetString(temp);
                var temps = Aes.Decode(dec2, secret);
                var json = JsonDocument.Parse(temps);
                

                var jse = json.RootElement;
                jse = jse[0];
                var rv = new List<Dungeon>();
                foreach (var dung in jse.EnumerateArray())
                {
                    rv.Add(new Dungeon()
                    {
                        city = city,
                        cid = dung.GetAsInt("c"),
                        type = dung.GetAsByte("t"),
                        level = dung.GetAsByte("l"),
                        completion = dung.GetAsFloat("p"),
                        dist = dung.GetAsFloat("d")

                    });
                }
                rv.Sort((a, b) => a.dist.CompareTo(b.dist));
                // dont wait on this 
                COTG.Views.MainPage.UpdateDungeonList(rv);
                return json;
            }
            catch (Exception e)
            {
                Log(e);
                return emptyJson;
            }

        }
    }

    public class OverviewApi : RestAPI
    {
        public OverviewApi(string addr) : base(addr, null) { }

    }
    public class TroopsOverview : OverviewApi
    {
        public TroopsOverview() : base("overview/trpover.php") { }
        public override void ProcessJson(JsonDocument json)
        {
            jsd = json;
            dict = new Dictionary<int, JsonElement>();
            foreach (var item in jsd.RootElement.EnumerateArray())
            {
                dict.Add(item.GetAsInt("id"), item);
            }
            Log("Got JS for troop overview");
            Log(json.ToString());
        }
        public static JsonDocument jsd;
        public static Dictionary<int, JsonElement> dict = new Dictionary<int, JsonElement>();
        public JsonElement Get(int cid)
        {
            return dict.GetValueOrDefault(cid);

        }

    }
    public class Post : RestAPI
    {
        public Post(string url, string secret = null) : base(url, secret) { }
        public override void ProcessJson(JsonDocument json)
        {
            jsd = json;
            dict = new Dictionary<int, JsonElement>();
            foreach (var item in jsd.RootElement.EnumerateArray())
            {
                dict.Add(item.GetAsInt("id"), item);
            }
            Log("Got JS for troop overview");
            Log(json.ToString());
        }
        public static JsonDocument jsd;
        public static Dictionary<int, JsonElement> dict = new Dictionary<int, JsonElement>();
        public JsonElement Get(int cid)
        {
            return dict.GetValueOrDefault(cid);

        }

        async public static Task Send(string url, string secret = null)
        {
            var p = new Post(url, secret);
            await p.Post();

        }
    }

}



