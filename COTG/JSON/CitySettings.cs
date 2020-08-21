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
        public static void SetCitySettings(int cid)
        {
            var cl = Game.CityList.Find(Views.SettingsPage.hubCitylistName);
            int reqHub = 0;
            var bestDist = 4096f;
            foreach(var hub in cl.cities)
            {
                var d = hub.DistanceToCid(cid);
                if(d < bestDist)
                {
                    bestDist = d;
                    reqHub = hub;
                }

            }


            var auto = autoBuildOn ? 1 : 0;
        var args = $"[1,{auto},{auto},{auto},{auto},{auto},{auto},{auto},0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,{reqWood},{reqStone},{reqIron},{reqFood},0,0,0,0,1,{reqHub},{reqHub},0,0,0,{maxWood},{maxStone},{maxIron},{maxFood},[1,{cottageLevel}],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10]]";

            Post.Send("includes/mnio.php", $"a={HttpUtility.UrlEncode(args, Encoding.UTF8)}&b={cid}");
            // find closest hub

            Note.Show($"Set hub and other settings: {Game.City.GetOrAddCity(reqHub).nameAndRemarks }");
        }
    }
}
