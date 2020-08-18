using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.Web.Http.Headers;
using Windows.Web.Http.Filters;
using static COTG.Debug;
using System.Web;
using COTG.Game;
using COTG.Helpers;
using static COTG.Game.Enum;
using COTG.Views;
using System.Globalization;
using COTG.JSON;
using Microsoft.Graphics.Canvas;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;

namespace COTG.Services
{
    public class RestAPI
    {
        //   public static List<RestAPI> all = new List<RestAPI>();
        public string localPath;
        public static JsonDocument emptyJson;

        public RestAPI(string _localPath)
        {
            localPath = _localPath;
            emptyJson = JsonDocument.Parse("{}");
            //  all.Add(this);
        }


        public virtual async Task Accept(HttpResponseMessage resp)
        {

            try
            {
                var buff = await resp.Content.ReadAsBufferAsync();

                var temp = new byte[buff.Length];

                using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buff))
                {
                    dataReader.ReadBytes(temp);
                }
                //   Log(resp.RequestMessage.RequestUri.ToString() + "\n\n>>>>>>>>>>>>>>\n\n" + Encoding.UTF8.GetString(temp) + "\n\n>>>>>>>>>>>>>>\n\n");
                ProcessJsonRaw(temp);
            }
            catch (Exception e)
            {
                Log(e);
            }

        }
        public virtual async Task<byte[]> AcceptAndReturn(HttpResponseMessage resp)
        {

            try
            {
                var buff = await resp.Content.ReadAsBufferAsync();

                var temp = new byte[buff.Length];

                using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buff))
                {
                    dataReader.ReadBytes(temp);
                }
                //   Log(resp.RequestMessage.RequestUri.ToString() + "\n\n>>>>>>>>>>>>>>\n\n" + Encoding.UTF8.GetString(temp) + "\n\n>>>>>>>>>>>>>>\n\n");
                return temp;
            }
            catch (Exception e)
            {
                Log(e);
            }
            return Array.Empty<byte>();

        }
        public virtual async Task<JsonDocument> AcceptJson(HttpResponseMessage resp)
        {
            var data = await AcceptAndReturn(resp);

            try
            {
                return JsonDocument.Parse(data);

            }
            catch (Exception e)
            {
                Log(e);
                return null;
            }


        }
        public virtual async Task<T> AcceptJsonT<T>(HttpResponseMessage resp)
        {
            var data = await AcceptAndReturn(resp);
            try
            {
                return JsonSerializer.Deserialize<T>(data);

            }
            catch (Exception e)
            {
                Log(e);
                return default;
            }


        }

        public virtual void ProcessJsonRaw(byte[] data)
        {
            var json = JsonDocument.Parse(data);
            ProcessJson(json);

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
            return nullPost;
        }

        async public Task Post()
        {
            for (; ;)
            {
                try
                {
                    if (JSClient.jsVars.token != null  )
                    {
                        await Accept(await Send(GetPostContent()));
                        return;
                    }


                }
                catch (Exception e)
                {
                    Log(e);
                    return;
                }
                await Task.Delay(400);

            }

        }

        public const string nullPost = "a=0";
        async public Task<HttpResponseMessage> Send(string postContent= nullPost)
        {
            HttpClient client = null;
            try
            {


                var req = new HttpRequestMessage(HttpMethod.Post, new Uri(JSClient.httpsHost, localPath));
                req.Content = new HttpStringContent(postContent,
                            Windows.Storage.Streams.UnicodeEncoding.Utf8,
                            "application/x-www-form-urlencoded");
                //req.TransportInformation.

                req.Content.Headers.TryAppendWithoutValidation("Content-Encoding", JSClient.jsVars.token);


                //                req.Headers.Append("Sec-Fetch-Site", "same-origin");
                //    req.Headers.Append("Sec-Fetch-Mode", "cors");
                //    req.Headers.Append("Sec-Fetch-Dest", "empty");

                for (; ; )
                {
                    if (JSClient.clientPool.TryTake(out client))
                        break;
                    await Task.Delay(128);
                }
                var resp = await client.SendRequestAsync(req, HttpCompletionOption.ResponseHeadersRead);
                //     Log($"res: {resp.GetType()} {resp.Succeeded} {resp}");
                //     Log($"req: {resp.RequestMessage.ToString()}");
                //   if (resp.ExtendedError != null)
                //      Log(resp.ExtendedError);
                JSClient.clientPool.Add(client);
                // Log("HTTP:" + resp.Version);
                client = null;
                if (resp != null)
                {
                    return resp;
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
                if (client != null)
                    JSClient.clientPool.Add(client);
                client = null;
                Log(e);
            }
            return null;


        }

     //   static RestAPI __0 = new RestAPI("includes/sndRad.php");//, "Sx23WW99212375Daa2dT123ol");
    //    static RestAPI __2 = new RestAPI("includes/gRepH2.php");//, "g3542RR23qP49sHH");
   //     static RestAPI __3 = new RestAPI("includes/bTrp.php");//, "X2UsK3KSJJEse2");
        //  public static GetCity getCity = new GetCity(0);
        //public static rMp regionView = new rMp();
  //      static RestAPI __6 = new RestAPI("includes/gSt.php");//, "X22x5DdAxxerj3");
   //     public static gWrd getWorldInfo = new gWrd();
  //      static RestAPI __8 = new RestAPI("includes/UrOA.php");//, "Rx3x5DdAxxerx3");
  //      static RestAPI __9 = new RestAPI("includes/sndTtr.php");//, "JJx452Tdd2375sRAssa");JJx452Tdd2375sRAssa "JJx452Tdd" + b2() + "sRAssa"
        // "fCv.php"  cid:cid (unencrptypted) "Xs4b2261f55dlme55s"
        // public static ScanRaids ScanDungeons = new ScanRaids();
        public static TroopsOverview troopsOverview = new TroopsOverview();
    }

    //public class rMp : RestAPI
    //{
    //    public rMp() : base("includes/rMp.php", "X22ssa41aA1522")
    //    {

    //    }
    //    public override string GetPostContent()
    //    {
    //        var args = "a=" + HttpUtility.UrlEncode(Aes.Encode("[249]", secret), Encoding.UTF8);
    //        return args;


    //    }
    //}

    public class GetWorldInfo : RestAPI
    {
        public GetWorldInfo() : base("includes/gWrd.php") // "Addxddx5DdAxxer569962wz")
        {
        }

        public override string GetPostContent()
        {
            // this	{"a":"worldButton","b":"block","c":true,"d":1591969039987,"e":"World"}
            //      {"a":"worldButton","b":"block","c":true,"d":1591988862914,"e":"World"}
            var json = $"{{\"a\":\"worldButton\",\"b\":\"block\",\"c\":true,\"d\":{JSClient.GameTimeMs()},\"e\":\"World\"}}";
            var encoded = Aes.Encode(json, $"Addxddx5DdAxxer{Player.myId}2wz" );
            var args = "a=" + HttpUtility.UrlEncode(encoded, Encoding.UTF8);
            //"a=JwHt8WTz416hj%2FsCxccQzDNR47ebTllFGQq957Pigc%2BEb8EHJKNoVgVKQeNu2a4xi9Tx1vFxsUxw9WxRTuPLsey5mcvlVcftThXU4gA9";
            return args;
        }

        public override void ProcessJson(JsonDocument json)
        {
            World.UpdateCurrent(json);
        }
        public static async void Send()
        {
            if (Alliance.diplomacyFetched)
            {
                (new GetWorldInfo()).Post();
            }
            else
            {
                await Task.Delay(200);
                Send();
            }
        }

        //async public void Post2()
        //{
        //    var a = await JSClient.view.InvokeScriptAsync("avapost", new string[] { "includes/gWrd.php",
        //        "a=tgLyUZYF5F6ynQjCp3FXJOZ6ElUHXPUygineE33LuF2eDHwB%2FWH8MWY%2FA2CM%2FIra7fwRRCRKZzB1BMW826w6Cq2jSWL6%2FH64owys4lIv" });
        //    Log(a);
        //}
    }

    public class GetCity : RestAPI
    {
        public int cid;
        Action<JsonElement, City> action;
        public GetCity(int _cid, Action<JsonElement, City> _action = null) : base("includes/gC.php")
        {
            cid = _cid;
            action = _action;
        }
        public override string GetPostContent()
        {
            var encoded = Aes.Encode(cid.ToString(), $"X2U11s33S{Player.myId}ccJx1e2");
            var args = "a=" + HttpUtility.UrlEncode(encoded, Encoding.UTF8);
            return args;
        }

        public override void ProcessJson(JsonDocument json)
        {
            // var cid = json.RootElement.GetAsInt("cid");
            //   Log("Got JS " + cid);
            var city = City.GetOrAddCity(cid);
            var root = json.RootElement;
            city.LoadFromJson(root);
            if (action != null)
                action(root, city);
        }
        public static Task Post(int _cid, Action<JsonElement, City> _action = null)
        {
            Assert(_cid > 65536);
            return (new GetCity(_cid, _action)).Post();

        }

    }


    public class sndRaid : RestAPI
    {
        public string json;
        public int cid;
        public sndRaid(string _json, int _cid) : base("includes/sndRaid.php" )
        {
            Log($"sndRaid:{_json}");
            cid = _cid;
            json = _json;

        }
        public override string GetPostContent()
        {
            var encoded = Aes.Encode(json, $"XTR977sW{Player.myId}sss2x2");
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
        int cid;
        //                       Xs4b22320360lme55s
        public static string secret;// = "Xs4b2261f55dlme55s";
        public ScanDungeons(int _cid) : base("includes/fCv.php")
        {
            cid = _cid;
        }
        public static async void Post(int _cid, bool getCityFirst)
        {
            //   Log(_cid.CidToString());
            if (getCityFirst)
                await GetCity.Post(_cid);
            //   await Task.Delay(2000);
            //   COTG.Views.MainPage.CityListUpdateAll();
            if(secret != null)
                new ScanDungeons(_cid).Post();

        }
        public override string GetPostContent()
        {
            var args = "cid=" + cid;
            return args;
        }

        public override async Task Accept(HttpResponseMessage resp)
        {
            Log("Got fCv");


            try
            {
                var city = City.allCities[cid];
                var buff = await resp.Content.ReadAsBufferAsync();

                var temp = new byte[buff.Length - 1];

                using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buff))
                {
                    dataReader.ReadByte(); // for some reason, the first two are '\n'
                    dataReader.ReadBytes(temp);
                }
                var dec2 = Encoding.UTF8.GetString(temp);
                if (!dec2.IsNullOrEmpty())
                {
                    var temps = Aes.Decode(dec2, secret);
                    var json = JsonDocument.Parse(temps);


                    var jse = json.RootElement;
                    jse = jse[0];
                    Dungeon.ShowDungeonList(city, jse);
                }
            }
            catch (Exception e)
            {
                Log(e);
            }

        }


    }

    public class OverviewApi : RestAPI
    {
        public OverviewApi(string addr) : base(addr) { }

    }
    public class TroopsOverview : OverviewApi
    {
        public TroopsOverview() : base("overview/trpover.php") { }
        public override void ProcessJson(JsonDocument json)
        {
            jsd = json;
            var changed = new HashSet<City>();
            foreach (var item in jsd.RootElement.EnumerateArray())
            {
                var cid = item.GetAsInt("id");
                var v = City.allCities[cid];
                List<TroopTypeCount> tsHome = new List<TroopTypeCount>();
                List<TroopTypeCount> tsTotal = new List<TroopTypeCount>();
                var hasAny = false;
                foreach (var tt in item.EnumerateObject())
                {
                    var type = tt.Name;
                    // Some are lower case
                    if (type[0] >= 'a' && type[0] <= 'z')
                        continue;
                    int count = tt.Value.GetInt32();
                    if (count == 0)
                        continue;

                    var split = type.Split('_', StringSplitOptions.RemoveEmptyEntries);
                    Assert(split.Length == 2);
                    var tE = Game.Enum.ttNameWithCaps.IndexOf(split[0]);
                    var ttc = new TroopTypeCount(tE, count);

                    if (split[1] == "home")
                        tsHome.Add(ttc);
                    else
                        tsTotal.Add(ttc);
                    hasAny = true;
                }
                if (hasAny)
                {
                    v.troopsHome = tsHome.ToArray();
                    v.troopsTotal = tsTotal.ToArray();
                }
                else
                {
                    v.troopsTotal = v.troopsHome = Array.Empty<TroopTypeCount>();
                }
                var tsh = v.troopsHome.TS();
                var tst = v.troopsTotal.TS();
                if ((tsh - v.tsHome).Abs().Max((tst - v.tsTotal).Abs()) > 16)
                {
                    v.tsTotal = tst;
                    v.tsHome = tsh;
                    changed.Add(v);

                    //v.OnPropertyChanged(nameof(v.tsTotal));
                }

            }
            if (!changed.IsNullOrEmpty())
            {
                changed.NotifyChange();
            }
            //  Log("Got JS for troop overview");
            //  Log(json.ToString());
        }
        public static JsonDocument jsd;
        public static Dictionary<int, JsonElement> dict = new Dictionary<int, JsonElement>();
        public JsonElement Get(int cid)
        {
            return dict.GetValueOrDefault(cid);

        }
    }



    /*
     {
	"a": [
		[
			20447699,               // 0
			"City of KittyKat",     // 1
			"C 34 (467:312)",       // 2
			4,                      // 3
			5200,                   // 4
			0,                      // 5
			4,                      // 6
			0,                      // 7
			0, // 8
			0,//9
			0,//10
			0,//11
			[ // 12
				[
					8622089209966, // 0
					1300,          // 1
					"Mountain Cavern, Level 4 (91%)", // 2
					0,   // 3  1=is returning
					2, // 4  repeat or once
					[  // 5
						{
							"tt": "2",
							"tv": 308
						},
						{
							"tt": "3",
							"tv": 185
						},
						{
							"tt": "5",
							"tv": 807
						}
					],
					{  // 6
						"w": 0,
						"s": 0,
						"i": 0,
						"f": 0,
						"g": 0,
						"r": "1",
						"rut": 0,
						"otr": [
							{
								"tt": "2",
								"tv": 308
							},
							{
								"tt": "3",
								"tv": 185
							},
							{
								"tt": "5",
								"tv": 807
							}
						]
					},
					"01:51:31 23/06", // 7
					20316624 //
				],
			]
		]
	],
	"b": [
		1,
		5200,
		0,
		0,
		[
			0,
			0,
			4,
			0
		],
		4
	]
}
     */
    public class RaidOverview : OverviewApi
    {
        public static RaidOverview inst = new RaidOverview();
        public static Task Send() => inst.Post();
        public RaidOverview() : base("overview/graid.php") { }
        public override void ProcessJson(JsonDocument jsd)
        {
            // reset all to start
            foreach (var city in City.allCities.Values)
            {
                city.raids = Array.Empty<Raid>();
                city.raidCarry = 0;
            }
            //           string dateExtra = DateTime.Now.Year
            var a = jsd.RootElement.GetProperty("a");
            foreach (var cr in a.EnumerateArray())
            {
                int cid = cr[0].GetInt32();
                var city = City.GetOrAddCity(cid);
                var raids = Array.Empty<Raid>();
                var minCarry = 255;
                foreach (var r in cr[12].EnumerateArray())
                {
                    var target = r[8].GetInt32();
                    var dateTime = r[7].GetString().ParseDateTime(false);
                    
                    if(raids.FindAndIncrement(target, dateTime))
                    {
                        continue;
                    }
                    string desc = r[2].GetString();
                    //    Mountain Cavern, Level 4(91 %)
                    var raid = new Raid();
                    raid.repeatCount = 1;
                    raid.target = target;
                    raid.time = dateTime;
                    raid.repeatCount = 1;
                    raid.isReturning = r[3].GetInt32() != 0;
                    raid.isRepeating = r[4].GetInt32() == 2;
                    //    Log(raid.ToString());
                    // raid.arrival.Year = DateTime.Now.Year;
                    var ss0 = desc.Split(',');
                    Assert(ss0.Length == 2);
                    var isMountain = ss0[0].Trim()[0] == 'M';
                    var ss = ss0[1].Split(new char[] { ' ', '(', ',', '%' }, StringSplitOptions.RemoveEmptyEntries);
                    Assert(ss.Length == 4);
                    var level = int.Parse(ss[1]);
                    var completion = int.Parse(ss[2]);
                    var res = (isMountain ? mountainLoot[level - 1] : otherLoot[level - 1]) * (2 - completion * 0.01f);
                    int cc = 0;
                    foreach (var ttr in r[5].EnumerateArray())
                    {
                        var tt = ttr.GetAsInt("tt");
                        int tv = ttr.GetAsInt("tv");
                        cc += ttCarry[tt] * tv;
                        //   Log($"{tt}:{tv}");
                    }
                    var carry = (cc * 100.0f / res).RoundToInt();
                    if (carry < minCarry)
                        minCarry = carry;
                    // Log($"cc:{cc}, res:{res}, carry:{cc/res} {r[7].GetString()} {r[3].GetInt32()} {r[4].GetInt32()}");

                    raids = raids.ArrayAppend(raid);
                }
                city.raidCarry = (byte)minCarry;
                city.raids = raids;
                // Log($"cid:{cid} carry: {minCarry}");

            }
           // MainPage.CityListUpdateAll();
        }
    }

    public class Post : RestAPI
    {
        public Post(string url) : base(url) { }


        // Does not wait for full response and does not parse json
        // postContent is xml uri encoded
        async public static Task Send(string url, string postContent)
        {
            var p = new Post(url);
            await p.Send(postContent);

        }
        async public static Task<JsonDocument> SendForJson(string url, string postContent= nullPost)
        {
            var p = new Post(url);
            return await p.AcceptJson(await p.Send(postContent));


        }
        async public static Task<T> SendForJsonT<T>(string url, string postContent=nullPost)
        {
            var p = new Post(url);
            return await p.AcceptJsonT<T>(await p.Send(postContent));


        }

        async public static Task SendEncrypted(string url, string postContentJson, string secret)
        {
            var p = new Post(url);
            await p.Send("a=" + HttpUtility.UrlEncode(Aes.Encode(postContentJson, secret), Encoding.UTF8));
        }
        async public static Task<JsonDocument> SendEncryptedForJson(string url, string postContentJson, string secret)
        {
            var p = new Post(url);
            return await p.AcceptJson(await p.Send("a=" + HttpUtility.UrlEncode(Aes.Encode(postContentJson, secret), Encoding.UTF8)));
        }

        /*
        {
            "rcid": 21627422,
            "tr": "[{\"tt\":\"5\",\"tv\":\"1\"}]",
            "snd": 1,
            "cid": 21692958,
            "ts": 0
        }
        */
        public struct tt_tv
        {
            public int tt { get; set; }
            public int tv { get; set; }
        };
        public struct SndRein
        {
            public int rcid { get; set; }
            public string tr { get; set; }
            public int snd { get; set; }
            public int cid { get; set; }
            public string ts { get; set; }
        };

        public static async void SendRein( int cid,int rcid, TroopTypeCount[] tsSend, DateTimeOffset arrival, float travelTime,int splits )
        {
            var tttv = new List<tt_tv>();
            foreach(var t in tsSend)
            {
                tttv.Add(new tt_tv() { tt = t.type, tv = t.count/splits });
            }
            var sr = new SndRein()
            {
                cid = cid,
                rcid = rcid,
                tr = JsonSerializer.Serialize(tttv),
                snd = 1,
            };
            if (arrival - JSClient.ServerTime() > TimeSpan.FromHours(travelTime+1.0f/64.0))
            {
                sr.snd = 3;
                sr.ts = arrival.ToString("MM/dd/yyyy HH':'mm':'ss");
            }
             var post = JsonSerializer.Serialize(sr);
            var secret = $"XTR977sW{Player.myId}sss2x2";
            var city = City.GetOrAddCity(cid);
            for(var i = 0; ; )
            {
                Note.Show("Sending Reinforcements "+(i+1));
                var jsd = await SendEncryptedForJson("includes/sndRein.php", post, secret);
                if(jsd ==null)
                {
                    Note.Show("Something went wrong");
                    break;
                }

                if (++i >= splits)
                {
                    if (sr.snd != 3)
                    {
                        Log("Sent last");
                        city.LoadFromJson(jsd.RootElement);
                        Note.Show("Sent Reinforcements");
                    }
                    else
                    {
                        Note.Show("Scheduled Reinforcements");
                    }
                    break;
                }
                await Task.Delay(500); 
            }
        }

    }
    public static class TileMapFetch
    {
        async static public Task<TileData> Get()
        {
            HttpClient client = null;
            try
            {
                for (; ; )
                {
                    if (JSClient.clientPool.TryTake(out client))
                        break;
                    await Task.Delay(128);
                }
                var buff = await client.GetBufferAsync(new Uri(JSClient.httpsHost, "maps/newmap/rmap6.json?a=0"));
                JSClient.clientPool.Add(client);
                client = null;
                if (buff != null)
                {
                    var temp = new byte[buff.Length];

                    using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buff))
                    {
                        dataReader.ReadBytes(temp);
                    }
                   // Log("Hello!");
                    return JsonSerializer.Deserialize<TileData>(temp);
                   // Log("Helllo!");
                }
                else
                {
                    Log("Error!");
                };
            }
            catch (Exception e)
            {
                if (client != null)
                    JSClient.clientPool.Add(client);
                client = null;
                Log(e);
            }
            return null;


        }

        async static public Task<CanvasBitmap> Load(Uri uri)
        {

            try
            {
                var buff = await JSClient.downloadImageClient.GetBufferAsync(uri);
                return await CanvasBitmap.LoadAsync(ShellPage.canvas, buff.AsStream().AsRandomAccessStream());

            }
            catch (Exception e)
            {
                Log(e);
            }
            return null;

        }

    }

    public class CityOverview{
//        public string city { get; set; }
 //       public string location { get; set; }
        public int score { get; set; }
        [JsonConverter(typeof(UShortConverter))]
        public ushort carts_total { get; set; }
        [JsonConverter(typeof(UShortConverter))]
        public ushort carts_home { get; set; }
   //     public int wood_per_hour { get; set; }
        public int wood { get; set; }
        public int wood_storage { get; set; }
     //   public int stone_per_hour { get; set; }
        public int stone { get; set; }
        public int stone_storage { get; set; }
      //  public int iron_per_hour { get; set; }
        public int iron { get; set; }
        public int iron_storage { get; set; }
       // public int food_per_hour { get; set; }
        public int food { get; set; }
        public int food_storage { get; set; }
        [JsonConverter(typeof(UShortConverter))]
        public ushort ships_total { get; set; }

        [JsonConverter(typeof(UShortConverter))]
        public ushort ships_home { get; set; }
        public string Academy { get; set; }
        public string Sorc_tower { get; set; }
       // public string reference { get; set; }
        public int id { get; set; }

        public static Task<CityOverview[]> Send()
        {
            return Post.SendForJsonT<CityOverview[]>("overview/citover.php",RestAPI.nullPost);
        }
    }
    public class UShortConverter : JsonConverter<ushort>
    {
        public override ushort Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (stringValue.IsNullOrEmpty())
                    return 0;
                if (ushort.TryParse(stringValue, out var value))
                {
                    return value;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetUInt16(out var rv))
                    return rv;

                return (ushort)reader.GetSingle();
            }
            else if (reader.TokenType == JsonTokenType.Null)
            {
                return 0;
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, ushort value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}



