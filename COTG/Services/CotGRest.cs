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

namespace COTG.Services
{
    public class RestAPI
    {
        //   public static List<RestAPI> all = new List<RestAPI>();
        public string localPath;
        public string secret;
        public static JsonDocument emptyJson;

        public RestAPI(string _localPath, string _secret)
        {
            localPath = _localPath;
            secret = _secret;
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
        public virtual async Task<JsonDocument> AcceptJSON(HttpResponseMessage resp)
        {
            var data = await AcceptAndReturn(resp);
            return JsonDocument.Parse(data);

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
            return "a=error";
        }

        async public Task Post()
        {
            try
            {
                if(JSClient.jsVars.token == null)
                {
                    Log("Post before we have creds");
                    return;
                }
                await Accept(await Send(GetPostContent()));
            }
            catch (Exception e)
            {
                Log(e);
            }


        }
        async public Task<HttpResponseMessage> Send(string postContent)
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

                client = JSClient.clientPool.Take();
                var resp = await client.SendRequestAsync(req, HttpCompletionOption.ResponseHeadersRead);
                //     Log($"res: {resp.GetType()} {resp.Succeeded} {resp}");
                //     Log($"req: {resp.RequestMessage.ToString()}");
                //   if (resp.ExtendedError != null)
                //      Log(resp.ExtendedError);
                JSClient.clientPool.Add(client);
               // Log("HTTP:" + resp.Version);
                client = null;
                if (resp!=null)
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
                if(client!=null)
                JSClient.clientPool.Add(client);
                client = null;
                Log(e);
            }
            return null;


        }

        static RestAPI __0 = new RestAPI("includes/sndRad.php", "Sx23WW99212375Daa2dT123ol");
        static RestAPI __2 = new RestAPI("includes/gRepH2.php", "g3542RR23qP49sHH");
        static RestAPI __3 = new RestAPI("includes/bTrp.php", "X2UsK3KSJJEse2");
     //  public static GetCity getCity = new GetCity(0);
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
            var args = "a=" + HttpUtility.UrlEncode(Aes.Encode("[249]", secret), Encoding.UTF8);
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
            var json = $"{{\"a\":\"worldButton\",\"b\":\"block\",\"c\":true,\"d\":{JSClient.GameTimeMs()},\"e\":\"World\"}}";
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

    public class GetCity : RestAPI
    {
        public int cid;
        public GetCity(int _cid) : base("includes/gC.php", "X2U11s33S56996ccJx1e2")
        {
            cid = _cid;
        }
        public override string GetPostContent()
        {
            var encoded = Aes.Encode(cid.ToString(), secret);
            var args = "a=" + HttpUtility.UrlEncode(encoded, Encoding.UTF8);
            return args;
        }

        public override void ProcessJson(JsonDocument json)
        {
           // var cid = json.RootElement.GetAsInt("cid");
            Log("Got JS " + cid);
             var city=City.all.GetOrAdd(cid,City.Factory);
            city.LoadFromJson(json.RootElement);

        }
        public static  Task Post(int _cid)
        {
            return (new GetCity(_cid)).Post();

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
        int cid;

        public ScanDungeons(int _cid) : base("includes/fCv.php", "Xs4b2261f55dlme55s")
        {
            cid = _cid;
        }
        public static async void Post(int _cid,bool getCityFirst)
        {
         //   Log(_cid.CidToString());
         if(getCityFirst)
            await GetCity.Post(_cid);
         //   await Task.Delay(2000);
         //   COTG.Views.MainPage.CityListUpdateAll();
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
                var city = City.all[cid];
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
            }
            catch (Exception e)
            {
                Log(e);
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
            var changed = new HashSet<City>();
            foreach (var item in jsd.RootElement.EnumerateArray())
            {
                var cid = item.GetAsInt("id");
                var v = City.all[cid];
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
                var tsh = TroopTypeCount.TS(v.troopsHome);
                var tst = TroopTypeCount.TS(v.troopsTotal);
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
            //           string dateExtra = DateTime.Now.Year
            var a = jsd.RootElement.GetProperty("a");
            foreach (var cr in a.EnumerateArray())
            {
                int cid = cr[0].GetInt32();
                var city=City.all.GetOrAdd(cid,City.Factory);
                List<Raid> raids = new List<Raid>();
                var minCarry = 255;
                foreach (var r in cr[12].EnumerateArray())
                {
                    string desc = r[2].GetString();
                    //    Mountain Cavern, Level 4(91 %)
                    var raid = new Raid();
                    raid.target = r[8].GetInt32();
                    raid.arrival =  r[7].GetString().ParseDateTime(false);
                    raid.isReturning = r[3].GetInt32() != 0;
                    raid.isRepeating = r[4].GetInt32() == 2;
                    Log(raid.ToString());
                   // raid.arrival.Year = DateTime.Now.Year;
                    var ss0 = desc.Split(',');
                    Assert(ss0.Length == 2);
                    var isMountain = ss0[0].Trim()[0] == 'M';
                    var ss = ss0[1].Split(new char[] { ' ', '(',  ',', '%' }, StringSplitOptions.RemoveEmptyEntries);
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
                        Log($"{tt}:{tv}");
                    }
                    var carry = (cc * 100.0f / res).RoundToInt();
                    if (carry < minCarry)
                        minCarry = carry;
                    Log($"cc:{cc}, res:{res}, carry:{cc/res} {r[7].GetString()} {r[3].GetInt32()} {r[4].GetInt32()}");

                    raids.AddIfAbsent(raid);
                }
                 city.raidCarry = (byte)minCarry;
                city.raids = raids.ToArray();
                Log($"cid:{cid} carry: {minCarry}");
                
            }
            MainPage.CityListUpdateAll();
        }
    }

    public class Post : RestAPI
    {
        public Post(string url, string secret = null) : base(url, secret) { }
       

        // Does not wait for full response and does not parse json
        // postContent is xml uri encoded
        async public static Task Send(string url, string postContent, string secret = null)
        {
            var p = new Post(url, secret);
            await p.Send(postContent);

        }
        async public static Task<JsonDocument> SendForJson(string url, string postContent, string secret = null)
        {
            var p = new Post(url, secret);
            return await p.AcceptJSON( await p.Send(postContent) );


        }

        async public static Task SendEncrypted(string url, string postContentJson, string secret )
        {
            var p = new Post(url, secret);
            await p.Send("a=" + HttpUtility.UrlEncode(Aes.Encode(postContentJson, secret), Encoding.UTF8) );


        }
    }

}



