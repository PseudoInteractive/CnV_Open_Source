using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.Web.Http.Filters;
using System.Net.Http.Json;
using static COTG.Debug;
using System.Web;

namespace COTG.Services
{
    public class RestAPI
    {
        public static List<RestAPI> all = new List<RestAPI>();
        public string localPath;
        public string secret;

        public RestAPI(string _localPath, string _secret)
        {
            localPath = _localPath;
            secret = _secret;
            all.Add(this);
        }


        public virtual async Task Accept(Uri uri, HttpResponseMessage resp)
        {
            var str = await resp.Content.ReadAsStringAsync();
            Log(str);
            try
            {
                var json = JsonDocument.Parse(str);
                Log(json.ToString());
                ProcessJsonPhase0(json);

            }
            catch (Exception e)
            {
                Log(e);
            }

        }

        public virtual void ProcessJsonPhase0(JsonDocument json)
        {

            if (json.RootElement.TryGetProperty("a", out var a))
            {
                var s = a.GetString();
                if (secret != null)
                {
                    var raw = COTG.Aes.Decode(s, secret);
                    Log(raw);
                }
            }
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
      
        async public  Task Post()
        {
            try
            {


                //            using var req  =anyPost;
                var req = new HttpRequestMessage(HttpMethod.Post, new Uri(JSClient.httpsHost, localPath));
                // req.TransportInformation.ver
                //req.AllowAutoRedirect = true;
                req.Content = new HttpStringContent(( GetPostContent() ),

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
                    await Accept(req.RequestUri, resp.ResponseMessage);
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



        }
        
        static RestAPI __0 = new RestAPI("includes/sndRad.php", "Sx23WW99212375Daa2dT123ol");
        static RestAPI __2 = new RestAPI("includes/gRepH2.php", "g3542RR23qP49sHH");
        static RestAPI __3 = new RestAPI("includes/bTrp.php", "X2UsK3KSJJEse2");
        public static RestAPI goCity  = new gC();
        public static rMp regionView = new rMp();
        static RestAPI __6 = new RestAPI("includes/gSt.php", "X22x5DdAxxerj3");
        static RestAPI __7 = new RestAPI("includes/gWrd.php", "Addxddx5DdAxxer23752wz");
        static RestAPI __8 = new RestAPI("includes/UrOA.php", "Rx3x5DdAxxerx3");
        static RestAPI __9 = new RestAPI("includes/sndTtr.php", "JJx452Tdd2375sRAssa");
    }

       public class rMp : RestAPI
    {
        public rMp(): base("includes/rMp.php", "X22ssa41aA1522")
        {

        }
        public override string GetPostContent()
        {
            var encoded = Aes.Encode("[249]",secret);
            var args = "a=" + HttpUtility.UrlEncode( encoded,Encoding.UTF8 );
            return args;


        }
        

        }
    public class gC : RestAPI
    {
        int cid = 0;
        public gC() : base("includes/gC.php", "X2U11s33S2375ccJx1e2")
        {

        }
        async public Task PostC(int _cid = 0)
		{
            cid = _cid;
            await Post();
            cid = 0;
		}

        public override string GetPostContent()
        {
            var encoded = Aes.Encode( (cid!=0?cid:JSClient.jsVars.cid).ToString(), secret);
            var args = "a=" + HttpUtility.UrlEncode(encoded, Encoding.UTF8);
            return args;


        }


    }


}

    

