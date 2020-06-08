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
namespace COTG.Services
{
    public class RestAPI
    {
        public static List<RestAPI> all;
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
            var json = JsonDocument.Parse(str);
            Log(str);
            ProcessJsonPhase0(json);
        }

        public virtual void ProcessJsonPhase0(JsonDocument json)
        {
           
            if(json.RootElement.TryGetProperty("a", out var a))
            {
                var s = a.GetString();
                if (secret != null)
                {
                    var raw = COTG.Aes.Decode(s, secret);
                    Log(raw);
                }
            }
        }
        async public static Task HandleResonse(Uri uri,HttpResponseMessage resp)
        {
            var localPath = uri.LocalPath;
            foreach (var h in all)
            {
                if(uri.LocalPath.Contains(h.localPath))
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
        static RestAPI __0 = new RestAPI("includes/sndRad.php", "Sx23WW99212375Daa2dT123ol");
        static RestAPI __2 = new RestAPI("includes/gRepH2.php", "g3542RR23qP49sHH");
        static RestAPI __3 = new RestAPI("includes/bTrp.php", "X2UsK3KSJJEse2");
        static RestAPI __4 = new RestAPI("includes/gC.php", "X2U11s33S2375ccJx1e2");
        static RestAPI __5 = new RestAPI("includes/rMp.php", "X22ssa41aA1522");
        static RestAPI __6 = new RestAPI("includes/gSt.php", "X22x5DdAxxerj3");
        static RestAPI __7 = new RestAPI("includes/gWrd.php", "Addxddx5DdAxxer23752wz");
        static RestAPI __8 = new RestAPI("includes/UrOA.php", "Rx3x5DdAxxerx3");
        static RestAPI __9 = new RestAPI("includes/sndTtr.php", "JJx452Tdd2375sRAssa");
    }

}

    
}
