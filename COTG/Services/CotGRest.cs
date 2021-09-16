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
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using ContentDialog = Microsoft.UI.Xaml.Controls.ContentDialog;
using ContentDialogResult = Microsoft.UI.Xaml.Controls.ContentDialogResult;
using TroopTypeCounts = COTG.Game.TroopTypeCounts;
//COTG.DArray<COTG.Game.TroopTypeCount>;
using TroopTypeCountsRef = COTG.Game.TroopTypeCounts;
using static COTG.Game.TroopTypeCountHelper;
//COTG.DArrayRef<COTG.Game.TroopTypeCount>;

namespace COTG.Services
{
	[Flags]
	public enum RestFlags
	{
		silenceError=1,
		track=1<<1, // only if eventName is not empty
		onlyHeaders=1<<2,
	}
	public class RestAPI
	{
		//   public static List<RestAPI> all = new List<RestAPI>();
		public string localPath;
		public static JsonDocument emptyJson;

		public RestFlags restFlags =  default;
		public string eventName = string.Empty;
		public virtual string extra => string.Empty;
		public RestAPI(string _localPath, int _pid = -1)
		{
			localPath = _localPath;
			emptyJson = JsonDocument.Parse("{}");
			pid = _pid;
			//  all.Add(this);
		}

		public int pid = -1;

		public virtual async Task<bool> AcceptAndProcess(HttpResponseMessage resp, bool except)
		{

			try
			{
				
					var json = JsonDocument.Parse(await AsArray(resp), jsonParseOptions);
					ProcessJson(json);
					return true;
				
			}
			catch (Exception e)
			{
				LogEx(e, report: false, eventName: $"json:{GetType().Name}{eventName}", extra: resp?.RequestMessage?.RequestUri?.ToString().Truncate(128) );
				return false;
			}

		}



		//public static async Task<byte[]> AcceptBytes(HttpResponseMessage resp, bool except)
		//{

		//	try
		//	{
		//		var buff = await AsStream(resp);
		//		var temp = new byte[buff.Length];

		//		using(var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buff))
		//		{
		//			dataReader.ReadBytes(temp);
		//		}
		//		//   Log(resp.RequestMessage.RequestUri.ToString() + "\n\n>>>>>>>>>>>>>>\n\n" + Encoding.UTF8.GetString(temp) + "\n\n>>>>>>>>>>>>>>\n\n");
		//		return temp;


		//	}
		//	catch (Exception e)
		//	{
		//		LogEx(e,report: except, eventName: "HTTPAcceptBytes");
		//	}
		//	return Array.Empty<byte>();

		//}

		//private static async Task<Stream> AsStream(HttpResponseMessage resp)
		//{
		//	var buffer = await resp.Content.ReadAsBufferAsync();

		//	return (buffer).AsStream();
		//}
		private static async Task<byte[]> AsArray(HttpResponseMessage resp)
		{
			var buffer = await resp.Content.ReadAsBufferAsync();

			return (buffer).ToArray();
		}

		public static async Task<string> AcceptText(HttpResponseMessage resp, bool except=false)
		{

			try
			{
				var buff = await resp.Content.ReadAsStringAsync();

				return buff;
			}
			catch (Exception e)
			{
				LogEx(e,report: except, eventName: "HTTPText");
			}
			return string.Empty;

		}
		public static async Task<JsonDocument> AcceptJson(HttpResponseMessage resp, bool except)
		{

			try
			{
				return JsonDocument.Parse(await AsArray(resp));

			}
			catch (Exception e)
			{
				LogEx(e, report: except,eventName: "HTTPJson");
				return null;
			}
		}

		public static async Task<T> AcceptJsonT<T>(HttpResponseMessage resp, bool except=false)
		{
			try
			{
				return JsonSerializer.Deserialize<T>(await AsArray(resp), Json.jsonSerializerOptions);

			}
			catch (Exception e)
			{
				LogEx(e,report: except, eventName: "HTTPJson");
				return default;
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
			return nullPost;
		}

		async public Task<bool> Post(bool except=false)
		{
			while (JSClient.jsVars == null)
			{
				await Task.Delay(400);
			}
			try
			{
				return await AcceptAndProcess(await Send(except),except);

			}
			catch (Exception e)
			{
				LogEx(e,report: except,eventName: "JsonProcess");
				return false;
			}

		}

		public const string nullPost = "a=0";
		private static readonly JsonDocumentOptions jsonParseOptions = new() { AllowTrailingCommas = true };

		public Task<HttpResponseMessage> Send(bool except=false)
		{
			return Send(GetPostContent(), except);
		}
		async public Task<HttpResponseMessage> Send(string postContent, bool except=true)
		{
			HttpClient client = null;
			await JSClient.clientPoolSema.WaitAsync().ConfigureAwait(false);
			try
			{


				for (; ; )
				{
					if (JSClient.clientPool.TryTake(out client))
					{
						break;
					}
					Assert(false);
					await Task.Delay(128).ConfigureAwait(false);
				}
				//				HttpResponseMessage resp;
				var uri = new Uri(JSClient.httpsHost, localPath);
				using (var req = new HttpRequestMessage(HttpMethod.Post, uri))
				{
					req.Content = new HttpStringContent(postContent,
								Windows.Storage.Streams.UnicodeEncoding.Utf8,
								"application/x-www-form-urlencoded");
					//req.TransportInformation.
					req.Content.Headers.TryAppendWithoutValidation("Content-Encoding", JSClient.PlayerToken(pid));
					req.Headers.Cookie.ParseAdd(JSClient.cookies); 

					//                req.Headers.Append("Sec-Fetch-Site", "same-origin");
					//    req.Headers.Append("Sec-Fetch-Mode", "cors");
					//    req.Headers.Append("Sec-Fetch-Dest", "empty");


					var respT = client.SendRequestAsync(req,restFlags.HasFlag(RestFlags.onlyHeaders)? HttpCompletionOption.ResponseHeadersRead: HttpCompletionOption.ResponseContentRead);
					//     Log($"res: {resp.GetType()} {resp.Succeeded} {resp}");
					//     Log($"req: {resp.RequestMessage.ToString()}");
					//   if (resp.ExtendedError != null)
					//      Log(resp.ExtendedError);
					if(restFlags.HasFlag(RestFlags.track))
					{
						AAnalytics.Track(uri.LocalPath);
					}
					var resp = await respT;
				
					if (resp != null)
					{
						return resp;
						//var b = await resp.Content.ReadAsInputStreamAsync();

						//                    jso = await JsonDocument.ParseAsync(b.ToString);

						// Log(b.ToString());
					}
				}


			}
			catch (Exception e)
			{
				if(except)
					LogEx(e, eventName: "HTTPPost");
			}
			finally
			{
				if (client != null)
					JSClient.clientPool.Add(client);
				client = null;
				JSClient.clientPoolSema.Release();
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
	public static class RestHelper
	{

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
			// this should be server time??
			var json = $"{{\"a\":\"worldButton\",\"b\":\"block\",\"c\":true,\"d\":{JSClient.ServerTimeMs()},\"e\":\"World\"}}";
			var encoded = Aes.Encode(json, $"Addxddx5DdAxxer{Player.activeId}2wz");
			var args = "a=" + HttpUtility.UrlEncode(encoded, Encoding.UTF8);
			//"a=JwHt8WTz416hj%2FsCxccQzDNR47ebTllFGQq957Pigc%2BEb8EHJKNoVgVKQeNu2a4xi9Tx1vFxsUxw9WxRTuPLsey5mcvlVcftThXU4gA9";
			return args;
		}

		public override void ProcessJson(JsonDocument json)
		{
			World.UpdateCurrent(json);
		}
		public static Task<bool> Send()
		{
			Assert(World.state == World.State.none || World.state == World.State.completed); ;
			World.state = World.State.started;
			return (new GetWorldInfo()).Post();
		}

		//async public void Post2()
		//{
		//    var a = await JSClient.view.ExecuteScriptAsync("avapost", new string[] { "includes/gWrd.php",
		//        "a=tgLyUZYF5F6ynQjCp3FXJOZ6ElUHXPUygineE33LuF2eDHwB%2FWH8MWY%2FA2CM%2FIra7fwRRCRKZzB1BMW826w6Cq2jSWL6%2FH64owys4lIv" });
		//    Log(a);
		//}
	}

	public class GetCity : RestAPI
	{
		public int cid;
		Action<JsonElement, City> action;
		public GetCity(int _cid, Action<JsonElement, City> _action) : base("includes/gC.php", World.CidToPlayerOrMe(_cid))
		{
			cid = _cid;
			action = _action;

		}
		public override string GetPostContent()
		{
			var encoded = Aes.Encode(cid.ToString(), $"X2U11s33S{pid}ccJx1e2");
			var args = "a=" + HttpUtility.UrlEncode(encoded, Encoding.UTF8);
			return args;
		}

		public override void ProcessJson(JsonDocument json)
		{
			// var cid = json.RootElement.GetAsInt("cid");
			//   Log("Got JS " + cid);
			var city = City.GetOrAddCity(cid);
			var root = json.RootElement;
			city.LoadCityData(root);
			if (action != null)
				action(root, city);
		}
		public static Task<bool> Post(int _cid, Action<JsonElement, City> _action = null)
		{
			Assert(_cid > 1);
			return (new GetCity(_cid, _action)).Post();

		}


	}


	public class sndRaid : RestAPI
	{
		public string json;
		public int cid;
		public sndRaid(string _json, int _cid) : base("includes/sndRad.php", World.CidToPlayerOrMe(_cid))
		{
	//		Log($"sndRaid:{_json}");
			cid = _cid;
			json = _json;
		//	restFlags &= ~RestFlags.track;
		}
		public override string GetPostContent()
		{
			var encoded = Aes.Encode(json, $"Sx23WW9921{World.CidToPlayerOrMe(cid)}Daa2dT123ol");
			//var encoded = Aes.Encode(json, $"XTR977sW{World.CidToPlayer(cid)}sss2x2");
			var args = $"cid={cid}&a=" + HttpUtility.UrlEncode(encoded, Encoding.UTF8);
			return args;
		}

		public override void ProcessJson(JsonDocument json)
		{
			Log("Sent raid");
		}

	}
	public class BuildEx:RestAPI
	{
		public string json;
		public int cid;
		public BuildEx(string _json,int _cid) : base("includes/nBuu.php",World.CidToPlayerOrMe(_cid))
		{
			//		Log($"sndRaid:{_json}");
			cid = _cid;
			json = _json;
			//	restFlags &= ~RestFlags.track;
		}
		public override string GetPostContent()
		{
			var encoded = Aes.Encode(json,$"X2U11s33S{World.CidToPlayerOrMe(cid)}ccJx1e2");
			//var encoded = Aes.Encode(json, $"XTR977sW{World.CidToPlayer(cid)}sss2x2");
			var args = $"cid={cid}&a=" + HttpUtility.UrlEncode(encoded,Encoding.UTF8);
			return args;
		}

		public override void ProcessJson(JsonDocument json)
		{
			var city = City.GetOrAddCity(cid);
			var root = json.RootElement;
			city.LoadCityData(root);
		}

	}

	public static class Recruit
	{

		struct Args
		{
			public int tid { get; set; }
			public int ttype { get; set; }
			public int bt { get; set; }
			public int tc { get; set; }
			public long ds { get; set; }
			public long de { get; set; }
			public int tl { get; set; }
			public int tm { get; set; }
			public int tbt { get; set; }
			public int pa { get; set; }
		}
		const string magic = "X2UsK3KSJJEse2";
		public static async Task<bool> Send(int cid, int tt, int count, bool cancelOrDismiss)
		{
			var city = City.Get(cid);
			try
			{
				if (cancelOrDismiss)
				{
					long tid = 0;
					int maxTs = 0;
					int curTs = 0;
					await GetCity.Post(cid, (jse, _city) =>
					{
						if (jse.TryGetProperty("tq", out var tq))
						{
							if (tq.ValueKind == JsonValueKind.Array && tq.GetArrayLength() > 0)
							{
								tid = tq[0].GetAsInt64("tid");
							}
						}
						maxTs = jse.GetAsInt("tt");
						curTs = jse.GetAsInt("tu");
					});
					if (tid != 0) // if recruiting
					{
						await Post.Send("includes/cTrp.php", $"cid={cid}&a={tid}");
					}

					// dismisss
					if (curTs+count > maxTs)
					{
						await city.SetMinistersOnAsync(false);
						var magic = "X2UsfKKKsse2"; ;
						var encoded = Aes.Encode("{\"5\":" + count + "}", magic);
						var urle = $"cid={cid}&a=" + HttpUtility.UrlEncode(encoded, Encoding.UTF8);

						await Post.Send("includes/dTp.php", urle);

					}
				}
				for (int i = 0; ; ++i)
				{

					var t = JSClient.ServerTimeMs();
					var args = new Args() { tid = AMath.random.Next(), bt = 1, ds = t, de = t + 1, pa = 1, tc = count, tm = 0, ttype = tt, tbt = 4, tl = 1 };

					var encoded = Aes.Encode(JsonSerializer.Serialize(args, Json.jsonSerializerOptions), magic);
					var urle = $"cid={cid}&a=" + HttpUtility.UrlEncode(encoded, Encoding.UTF8);
					var str = (await Post.SendForText("includes/bTrp.php", urle)).Trim();
					if (str == "0")
					{
						Note.Show($"Recruit {count} {ttNameWithCaps[tt]} in {City.Get(cid).nameMarkdown}");
						return true;
					}
					else if (i == 4)
					{
						Note.Show(str);
						return false;
					}
					await Task.Delay(250);
				}
			}
			catch (Exception ex)
			{
				LogEx(ex);
			}
			finally
			{
				city.SetMinistersOn(true);
			}
			return false;
		}


	}

		/*
		  {
	"tid": 1628537154,
	"ttype": 17,
	"bt": 4528000,
	"tc": 1,
	"ds": 1620092155331,
	"de": 1620096683331,
	"tl": 1,
	"tm": 0,
	"tbt": 4,
	"pa": 1
	}
		 *  var n5D = {
						tid: Number(p5D),
						ttype: Number(x5D),
						bt: Number(z5D),
						tc: Number(L5D),
						ds: Number(M5D),
						de: Number(r5D),
						tl: Number(L5D),
						tm: 0,
						tbt: Number(K5D),
						pa: 1
					};
					if (x5D == +17)
						ppdt["bc"] = ppdt[_s(+h6R)] + i5D;
					var Z5D = __s[5491];
					var g5D = a6.ccazzx.encrypt(JSON.stringify(n5D), Z5D, +256);
					N6();
					var P5D = $.post("/includes/" + __s[3174], { cid: cid, a: g5D });
		 */

		public class ScanDungeons : RestAPI
	{
		int cid;
		//                       Xs4b22320360lme55s
		public static string secret => JSClient.jsVars.raidSecret;// = "Xs4b2261f55dlme55s";
		public ScanDungeons(int _cid) : base("includes/fCv.php", World.CidToPlayerOrMe(_cid))
		{
			cid = _cid;
		//	restFlags &= ~RestFlags.track;

		}
		// returns true if raids were sent or sending failed.  True means that we should loop back and try again
		public static async Task<bool> Post(int _cid, bool getCityFirst, bool autoRaid)
		{


			//   Log(_cid.CidToString());
			if (getCityFirst)
			{
				for (int counter = 0; ; ++counter)
				{
					var okay = await GetCity.Post(_cid, null);
					if (okay)
						break;
					if (counter >= 8)
					{
						Note.Show($"Internet failed for {Spot.GetOrAdd(_cid).nameMarkdown}, please run auto raids again in a few minutes");
						return false;
					}
					await Task.Delay(500);
				}

			}            //   await Task.Delay(2000);
						 //   COTG.Views.MainPage.CityListUpdateAll();
			if (secret == null)
				return false;

			try
			{
				var msg = new ScanDungeons(_cid);
				var resp = await msg.Send(false);
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
					City.TryGet(_cid, out var city);
					return await Dungeon.ShowDungeonList(city, jse, autoRaid);
				}
				return false; // no dungeons?
			}
			catch
			{
				return true; // try again please
			}

		}
		public override string GetPostContent()
		{
			var args = "cid=" + cid;
			return args;
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
			var changed = new HashSet<City>();
			if (json.RootElement.ValueKind == JsonValueKind.Array)
			{
				foreach (var item in json.RootElement.EnumerateArray())
				{
					var cid = item.GetAsInt("id");
					if (!City.TryGet(cid, out var v))
						continue;
					var tsh = v.troopsHome.TS();
					v.troopsHome.Clear();
					v.troopsTotal.Clear();
				//	TroopTypeCounts tsHome = new();
			//	TroopTypeCounts tsTotal = new();
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
						var tE = Game.Enum.ttNameWithCapsAndGuard.IndexOf(split[0]);
						if (tE < 0) // Guard
							tE = 0;
						var ttc = new TroopTypeCount(tE, count);

						
						
						if (split[1] == "home")
							Add(ref v.troopsHome, ttc);
						else
							Add(ref v.troopsTotal, ttc);
						hasAny = true;
					}
	//				TroopTypeCount.Replace(ref v.troopsHome, ref tsHome);
		//			TroopTypeCount.Replace(ref v.troopsTotal,ref tsTotal);
					if (hasAny)
					{
						v._tsHome = v.troopsHome.TS();
						v._tsTotal = v.troopsTotal.TS();
						//	Trace($"TS Home {v._tsHome}");

					}
					else
					{
						v._tsHome = 0;
						v._tsTotal = 0;
					}
					if ( (tsh - v.troopsHome.TS()).Abs() > 16 || (tsh==0) ) 
					{
						changed.Add(v);
						//v.OnPropertyChanged(nameof(v.tsTotal));
					}

				}
			}
			if (!changed.IsNullOrEmpty())
			{
				changed.NotifyChange(nameof(Spot.tsHome), nameof(Spot.tsRaid), nameof(Spot.tsTotal));
			}
			//  Log("Got JS for troop overview");
			//  Log(json.ToString());
		}
	}


	public class ReinforcementsOverview : OverviewApi
	{
		public static ReinforcementsOverview instance = new ReinforcementsOverview();
		public ReinforcementsOverview() : base("overview/reinover.php") { }
		public override void ProcessJson(JsonDocument json)
		{
			// This gets rid of all reinforcements except those that from other players 
			foreach (var s in Spot.allSpots)
			{
				s.Value.reinforcementsIn = s.Value.reinforcementsIn.WhereNotMine() ;
				s.Value.reinforcementsOut = s.Value.reinforcementsOut.WhereNotMine();
			}
			var jsd = json;
			var changed = new HashSet<City>();
			int cityCount = 0;
			int reinCount = 0;
			int ts = 0;
			foreach (var item in jsd.RootElement.EnumerateObject())
			{
				var cid = int.Parse(item.Name);
				var spot = Spot.GetOrAdd(cid);
				++cityCount;
				foreach (var rein in item.Value[9].EnumerateArray())
				{
					try
					{
						var re = new Reinforcement();
						re.targetCid = cid;
						var targetCId = rein[1].GetAsInt();

						Assert(targetCId == cid);
						re.sourceCid = rein[15].GetAsInt();
						Assert(re.sourceCid != targetCId);
						// re.time = rein[9].ToString(
						re.order = rein[10].GetAsInt64();
						foreach (var ti in rein[8].EnumerateArray())
						{
							var str = ti.GetAsString();
							int tcEnd = 0;
							while (IsDigitOrCommaOrMinus(str, tcEnd))
								++tcEnd;
							if (str.Substring(0, tcEnd).TryParseIntChecked(out var count))
							{
								//					var count = int.Parse(str.Substring(0, tcEnd), NumberStyles.Any);
								var typeS = str.Substring(tcEnd + 1);
								var tE = Game.Enum.ttNameWithCapsAndBatteringRam.IndexOf(typeS);
								Add(ref re.troops, new TroopTypeCount(tE, count));
							}
							else
							{
								Add(ref re.troops, new TroopTypeCount(0, -1));
								Log("Bad string: " + ti.GetAsString());
							}
						}
						ts += re.troops.TS();
						spot.reinforcementsIn = spot.reinforcementsIn.ArrayAppend(re);
						var source = Spot.GetOrAdd(re.sourceCid);
						source.reinforcementsOut = source.reinforcementsOut.ArrayAppend(re);
						++reinCount;
					}
					catch(Exception ex)
					{
						LogEx(ex);

					}

				}


			}
			Note.Show($"Reinforcements updated {cityCount} cities reinforced with {reinCount} orders and {ts} TS");
		}

		private static bool IsDigitOrComma(string str, int tcEnd)
		{
			return tcEnd < str.Length && (char.IsDigit(str, tcEnd) || str[tcEnd] == ',');
		}
		private static bool IsDigitOrCommaOrMinus(string str, int tcEnd)
		{
			return tcEnd < str.Length && (char.IsDigit(str, tcEnd) || str[tcEnd] == ',' || str[tcEnd] == '-');
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

		public static Task SendMaybe()
		{
			var t = DateTimeOffset.UtcNow;
			if (t - lastFetched <= TimeSpan.FromMinutes(5))
			{
				lastFetched = t;
				return inst.Post();
			}
			return Task.CompletedTask;
		}
		public RaidOverview() : base("overview/graid.php") { }
		public static DateTimeOffset lastFetched = AUtil.dateTimeZero;
		public override void ProcessJson(JsonDocument jsd)
		{
			lastFetched = DateTimeOffset.UtcNow;
			// reset all to start
			foreach (var city in City.myCities)
			{
				city.raids = Array.Empty<Raid>();
				city.raidCarry = 0;
			}
			float rWood = 0, rStone = 0, rIron = 0, rFood = 0, rGold = 0;
			if (jsd.RootElement.ValueKind == JsonValueKind.Object)
			{
				//           string dateExtra = DateTime.Now.Year
				var a = jsd.RootElement.GetProperty("a");
				foreach (var cr in a.EnumerateArray())
				{
					int cid = cr[0].GetInt32();
					var city = City.GetOrAddCity(cid);
					var raids = Array.Empty<Raid>();
					var minCarry = 255;
					float tWood = 0, tStone = 0, tIron = 0, tFood = 0, tGold = 0;
					
					foreach (var r in cr[12].EnumerateArray())
					{
						var target = r[8].GetInt32();
						var dateTime = r[7].GetString().ParseDateTime(false);

						if (raids.FindAndIncrement(target, dateTime))
						{
							rWood += tWood;
							rStone += tStone;
							rIron += tIron;
							rFood += tFood;
							rGold += tGold;
							continue;
						}
						string desc = r[2].GetString();
						//    Mountain Cavern, Level 4(91 %)
						var raid = new Raid();
						raid.repeatCount = 1;
						raid.target = target;
						raid.time = dateTime;
						var r4 = r[4].GetByte(); 
						raid.isReturning = r[3].GetInt32() != 0;
						raid.r4 = r4;
						//=  r4== 2 ||r4==3;
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

						// slowest
						var maxTravel = 0.0;
						foreach (var ttr in r[5].EnumerateArray())
						{
							var tt = ttr.GetAsInt("tt");
							int tv = ttr.GetAsInt("tv");
							cc += ttCarry[tt] * tv;
							//var ts = ttTs[tt] * tv;
							var travel = TTTravel(tt);
							// Todo Navy
							if (travel > maxTravel)
							{
								maxTravel = travel;
								raid.troopType = (byte)tt;
							}
							//   Log($"{tt}:{tv}");
						}
						if (raid.isReturning)
						{
							var resO = r[6];
							var rate = 60.0f * 0.5f / (raid.GetOneWayTripTimeMinutes(city)); // to res per hour
							tWood = resO.GetAsInt("w") * rate;
							tIron = resO.GetAsInt("i") * rate;
							tFood = resO.GetAsInt("f") * rate;
							tStone = resO.GetAsInt("s") * rate;
							tGold = resO.GetAsInt("g") * rate;
							rWood += tWood;
							rStone += tStone;
							rIron += tIron;
							rFood += tFood;
							rGold += tGold;
						}
						else
						{
							tWood = 0; tStone = 0; tIron = 0; tFood = 0; tGold = 0;
						}
						var carry = (cc * 100.0f / res).RoundToInt();
						if (carry < minCarry)
							minCarry = carry;
						// Log($"cc:{cc}, res:{res}, carry:{cc/res} {r[7].GetString()} {r[3].GetInt32()} {r[4].GetInt32()}");

						raids = raids.ArrayAppend(raid);
					}
					city.raidCarry = (byte)minCarry.Min(255);
					city.raids = raids;
					var commands = (byte)cr[12].GetArrayLength();
					city.activeCommands = city.activeCommands.Max(commands);
					// Log($"cid:{cid} carry: {minCarry}");

				}
			}
			App.QueueOnUIThread(()
				=>
			{
				MainPage.instance.rWood.Text = $"Wood: {(rWood * 0.001).RoundToInt():N0} k/h";
				MainPage.instance.rStone.Text = $"Stone: {(rStone * 0.001).RoundToInt():N0} k/h";
				MainPage.instance.rIron.Text = $"Iron: {(rIron * 0.001).RoundToInt():N0} k/h";
				MainPage.instance.rFood.Text = $"Food: {(rFood * 0.001).RoundToInt():N0} k/h";
				MainPage.instance.rGold.Text = $"Gold: {(rGold * 0.001).RoundToInt():N0} k/h";
				//MainPage.rStone = rStone;
				//MainPage.rIron = rIron;
				//MainPage.rFood = rFood;
				//MainPage.rGold = rGold;
				//// MainPage.CityListUpdateAll();
				///
			});
		}
	}

	public class Post : RestAPI
	{
		public Post(string url, int _pid = -1) : base(url, _pid) { }


		// Does not wait for full response and does not parse json
		// postContent is xml uri encoded
		async public static Task Send(string url, string postContent, int _pid = -1, bool except = true)
		{
			var p = new Post(url, _pid);
			await p.Send(postContent, except);

		}
		async public static Task<HttpResponseMessage> SendForResponse(string url, string postContent, int _pid = -1, bool except=false)
		{
			var p = new Post(url, _pid);
			return await p.Send(postContent, except);
		}
		async public static Task<bool> SendForOkay(string url, string postContent, int _pid = -1, bool except = false)
		{
			var p = new Post(url, _pid);

			var result = await p.Send(postContent, except);
			if (result == null)
				return false;
			if (result.StatusCode == HttpStatusCode.Ok)
				return true;
			Log($"HTTP: {result.StatusCode} url:{url} post:{postContent}");
			return false;
		}

		async public static Task<JsonDocument> SendForJson(string url, string postContent = nullPost, int _pid = -1, bool except = false)
		{
			var p = new Post(url, _pid);
			return await AcceptJson(await p.Send(postContent, except), except);
		}
		async public static Task<string> SendForText(string url, string postContent = nullPost, int _pid = -1, bool except=false)
		{
			var p = new Post(url, _pid);
			return await AcceptText(await p.Send(postContent, except), except);
		}

		async public static Task<T> SendForJsonT<T>(string url, string postContent = nullPost, int _pid = -1, bool except=false)
		{
			var p = new Post(url, _pid);
			return await AcceptJsonT<T>(await p.Send(postContent, except));


		}

		async public static Task SendEncrypted(string url, string postContentJson, string secret, int _pid)
		{
			var p = new Post(url, _pid);
			await p.Send("a=" + HttpUtility.UrlEncode(Aes.Encode(postContentJson, secret), Encoding.UTF8));
		}
		async public static Task<JsonDocument> SendEncryptedForJson(string url, string postContentJson, string secret, int _pid, bool except=true)
		{
			var p = new Post(url, _pid);
			return await AcceptJson(await p.Send("a=" + HttpUtility.UrlEncode(Aes.Encode(postContentJson, secret), Encoding.UTF8), except),except);
		}
		async public static Task<string> SendEncryptedForText(string url, string postContentJson, string secret, int _pid, bool except = true)
		{
			var p = new Post(url, _pid);
			return await AcceptText(await p.Send("a=" + HttpUtility.UrlEncode(Aes.Encode(postContentJson, secret), Encoding.UTF8), except), except);
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

		public static async Task SendRein(int cid, int rcid, TroopTypeCounts tsSend, DateTimeOffset departAt, DateTimeOffset arrival, float travelTime, int splits, Microsoft.UI.Xaml.UIElement uie)
		{
			var tttv = new List<tt_tv>();
			tsSend.ForEach((t =>
		   {
			   tttv.Add(new tt_tv() { tt = t.type, tv = t.count / splits });
		   }));

			var pid = World.CidToPlayerOrMe(cid);

			var sr = new SndRein()
			{
				cid = cid,
				rcid = rcid,
				tr = JsonSerializer.Serialize(tttv, Json.jsonSerializerOptions),
				snd = 1, // 1 means send immediately
			};
			if (arrival > JSClient.ServerTime())
			{
				sr.snd = 3;
				sr.ts = arrival.ToString("MM/dd/yyyy HH':'mm':'ss");
			}
			else if (departAt > JSClient.ServerTime())
			{
				sr.snd = 2;
				sr.ts = departAt.ToString("MM/dd/yyyy HH':'mm':'ss");
			}
			var post = JsonSerializer.Serialize(sr, Json.jsonSerializerOptions);
			var secret = $"XTR977sW{pid}sss2x2";
			var city = City.GetOrAddCity(cid);
			for (var i = 0; ;)
			{
				Note.Show("Sending Reinforcements " + (i + 1));
				var jsd = await SendEncryptedForJson("includes/sndRein.php", post, secret, pid);
				if (jsd == null)
				{
					Note.Show("Something went wrong");
					break;
				}

				if (++i >= splits)
				{
					if (sr.snd != 3)
					{
						Log("Sent last");
						if (jsd.RootElement.ValueKind != JsonValueKind.Object)
						{
							if (jsd.RootElement.ValueKind == JsonValueKind.Number)
							{
								Note.Show("Scheduled Reinforcements");
							}
							else
							{
								if (!arrival.IsZero())
								{
									var result = await App.DispatchOnUIThreadTask(async () =>
									{
										var content = new ContentDialog()
										{
											Title = "Not enought Troops home or troops cannot make scheduled time",
											Content = "Send now or when they return from raiding?",
											PrimaryButtonText = "Yes",
											CloseButtonText = "Cancel"
										};
										//ElementSoundPlayer.Play(ElementSoundKind.Show);

										content.CopyXamlRoomFrom(uie);
										return await content.ShowAsync2();
									});
									if (result == ContentDialogResult.Primary)
									{
										await SendRein(cid, rcid, tsSend, departAt, AUtil.dateTimeZero, travelTime, splits, uie);
										return;
									}

								}
								else
								{
									Note.Show("Something went wrong, maybe not enough troops home");
								}
							}
							break;
						}
						city.LoadCityData(jsd.RootElement);
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
				await JSClient.clientPoolSema.WaitAsync();
				for (; ; )
				{
					if (JSClient.clientPool.TryTake(out client))
						break;
					Assert(false);
					await Task.Delay(128);
				}
				var buff = await client.GetBufferAsync(new Uri(JSClient.httpsHost, $"maps/newmap/rmap6.json?a={HttpUtility.UrlEncode(DateTime.Now.ToString("R"))}"));
				/*
				 * GET /maps/newmap/rmap6.json?a=Sat%20Mar%2013%202021%2014:24:01%20GMT-0800%20(Pacific%20Standard%20Time) HTTP/1.1
			  Host: w23.crownofthegods.com
			  Connection: keep-alive
			  Pragma: no-cache
			  Cache-Control: no-cache
			  User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.82 Safari/537.36 Edg/89.0.774.50
			  Accept: 
			  Sec-Fetch-Site: same-origin
			  Sec-Fetch-Mode: cors
			  Sec-Fetch-Dest: empty
			  Referer: https://w23.crownofthegods.com/
			  Accept-Encoding: gzip, deflate, br
			  Accept-Language: en-US,en;q=0.9
			  Cookie: _ga=GA1.2.1055797043.1609264074; _fbp=fb.1.1609264074502.2131400288; __gads=ID=bb534c2c262b5eb1-227ecd5a79c5009f:T=1609264077:RT=1609264077:S=ALNI_MbiRMZeGoatJbQVm5gpAomH5vSxRw; MicrosoftApplicationsTelemetryDeviceId=112453f2-6422-4887-88c6-12e528008f71; MicrosoftApplicationsTelemetryFirstLaunchTime=2021-02-28T01:02:30.235Z; remember_me=c50f0c70cd; _gid=GA1.2.991235873.1615674212; _gat=1; sec_session_id=4fr7soo40b1255mi6f1hjspee3
			  */

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

				LogEx(e, eventName: "TileMapFetch");
			}
			finally
			{
				if (client != null)
					JSClient.clientPool.Add(client);
				JSClient.clientPoolSema.Release();
			}
			return null;


		}



	}

	public class CityOverview
	{
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
			return Post.SendForJsonT<CityOverview[]>("overview/citover.php", RestAPI.nullPost);
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



