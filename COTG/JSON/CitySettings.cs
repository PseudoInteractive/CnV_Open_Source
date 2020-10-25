using COTG.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static COTG.Views.SettingsPage;
namespace COTG.JSON
{
    public class CitySettings
    {
        public static async void SetCitySettings(int cid)
        {
            var args = await JSClient.view.InvokeScriptAsync("avagetmo", null);

            try
            {
                var cl = Game.CityList.Find(Views.SettingsPage.hubCitylistName);
                int reqHub = 0;
                var bestDist = 4096f;
                foreach (var hub in cl.cities)
                {
                    var d = hub.DistanceToCid(cid);
                    if (d < bestDist)
                    {
                        bestDist = d;
                        reqHub = hub;
                    }

                }

                var split = args.Split(',', StringSplitOptions.RemoveEmptyEntries);

                var auto = autoBuildOn ? 1 : 0;
                //        var args = $"[1,{auto},{auto},{auto},{auto},{auto},{auto},{auto},0,0,   0
                //                      0,0,0,0,0,0,0,0,0,0,                                      10
                //                      0,0,0,0,0,0,0,0,0,0,                                      20
                //                      0,0,1,{reqWood},{reqStone},{reqIron},{reqFood},0,0,0,     30
                //                      0,1,{reqHub},{reqHub},0,0,0,{maxWood},{maxStone},{maxIron}, 40
                //                     {maxFood},[1,{cottageLevel}],[1,10],[1,10],[1,10],[1,       50
                //                      10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10]]";
//                var args = $"[1,{auto},{auto},{auto},{auto},{auto},{auto},{auto},0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,{reqWood},{reqStone},{reqIron},{reqFood},0,0,0,0,1,{reqHub},{reqHub},0,0,0,{maxWood},{maxStone},{maxIron},{maxFood},[1,{cottageLevel}],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10]]";

                split[33] = reqWood.ToString();
                split[34] = reqStone.ToString();
                split[35] = reqIron.ToString();
                split[36] = reqFood.ToString();
                split[42] = reqHub.ToString();
                split[43] = reqHub.ToString();
                split[47] = maxWood.ToString();
                split[48] = maxStone.ToString();
                split[49] = maxIron.ToString();
                split[50] = maxFood.ToString();
                split[52] = cottageLevel.ToString() + ']';
                var args2 = string.Join(',', split);
                Post.Send("includes/mnio.php", $"a={HttpUtility.UrlEncode(args2, Encoding.UTF8)}&b={cid}");
                // find closest hub

                Note.Show($"Set hub and other settings: {Game.City.GetOrAddCity(reqHub).nameAndRemarks }");
            }
            catch (Exception e)
            {
                COTG.Debug.Log(e);
                Note.Show($"Set hub failed, restarting might fix it");
            }


        }
    }
}
