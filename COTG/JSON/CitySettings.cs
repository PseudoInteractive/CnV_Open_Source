using COTG.Game;
using COTG.Services;
using COTG.Views;

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
            UpdateMinisterOptions(cid, (split) =>
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


                var auto = autoBuildOn ? 1 : 0;
                //        var args = $"[1,{auto},{auto},{auto},{auto},{auto},{auto},{auto},0,0,   0
                //                      0,0,0,0,0,0,0,0,0,0,                                      10
                //                      0,0,0,0,0,0,0,0,0,0,                                      20
                //                      0,0,1,{reqWood},{reqStone},{reqIron},{reqFood},0,0,0,     30
                //                      0,1,{reqHub},{reqHub},0,0,0,{maxWood},{maxStone},{maxIron}, 40
                //                     {maxFood},[1,{cottageLevel}],[1,10],[1,10],[1,10],[1,       50
                //                      10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10]]";
                //                var args = $"[1,{auto},{auto},{auto},{auto},{auto},{auto},{auto},0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,{reqWood},{reqStone},{reqIron},{reqFood},0,0,0,0,1,{reqHub},{reqHub},0,0,0,{maxWood},{maxStone},{maxIron},{maxFood},[1,{cottageLevel}],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10]]";


                split[32] = "1"; // use the same city all requests
                split[33] = reqWood.ToString();
                split[34] = reqStone.ToString();
                split[35] = reqIron.ToString();
                split[36] = reqFood.ToString();

                // send target
                split[37] = reqHub.ToString(); // hub to use for this res
                split[38] = reqHub.ToString(); // hub to use for this res
                split[39] = reqHub.ToString(); // hub to use for this res
                split[40] = reqHub.ToString(); // hub to use for this res
                split[41] = "0"; // use a different city for all sends


                split[42] = reqHub.ToString();
 //               split[43] = reqHub.ToString();
                split[47] = maxWood.ToString();
                split[48] = maxStone.ToString();
                split[49] = maxIron.ToString();
                split[50] = maxFood.ToString();
                split[52] = cottageLevel.ToString() + ']';
            });


        }
        public static async void UpdateMinisterOptions(int cid, Action<string[]> opts)
        {
            var args = await JSClient.view.InvokeScriptAsync("avagetmo", null);

            try
            {
                var split = args.Split(',', StringSplitOptions.RemoveEmptyEntries);

                opts(split);
                var args2 = string.Join(',', split);
                Post.Send("includes/mnio.php", $"a={HttpUtility.UrlEncode(args2, Encoding.UTF8)}&b={cid}");
                // find closest hub

            }
            catch (Exception e)
            {
                COTG.Debug.Log(e);
                Note.Show($"Set hub failed, restarting might fix it");
            }


        }
        public static void SetTargetHub(int cid, int targetHub)
        {
            UpdateMinisterOptions(cid, (split) =>
            {
                split[37] = targetHub.ToString(); // hub to use for this res
                split[38] = targetHub.ToString(); // hub to use for this res
                split[39] = targetHub.ToString(); // hub to use for this res
                split[40] = targetHub.ToString(); // hub to use for this res
                split[41] = "0"; // use a different city for all sends
       //         split[43] = targetHub.ToString();
            });
        }
        public static void SetOtherHubSettings(int cid, int sourceHub)
        {
            UpdateMinisterOptions(sourceHub, (split) =>
            {
                split[33] = reqWood.ToString();
                split[34] = reqStone.ToString();
                split[35] = reqIron.ToString();
                split[36] = reqFood.ToString();
                split[47] = maxWood.ToString();
                split[48] = maxStone.ToString();
                split[49] = maxIron.ToString();
                split[50] = maxFood.ToString();
                split[43] = cid.ToString();
            });
        }
        static string lastName = string.Empty;
        public static async void RenameDialog(int cid)
        {
            try
            {
                var city = City.GetOrAddCity(cid);
                var nameDialog = new CityRename();
                if (city._cityName == "*New City" && !lastName.IsNullOrEmpty())
                {
                    var name = lastName;
                    var lg = name.Length;
                    var numberEnd = lg;
                    while (numberEnd > 0 && char.IsNumber(name[numberEnd]))
                    {
                        --numberEnd;
                    }
                    if (numberEnd < lg)
                    {
                        // increment number if there is one
                        var start = name.Substring(0, numberEnd);
                        var end = (int.Parse(name.Substring(numberEnd, lg - numberEnd)) + 1).ToString();
                        name = start + end;
                    }
                    nameDialog.name.Text = name;
                }
                else
                {
                    nameDialog.name.Text = city._cityName;
                }
                var result = await nameDialog.ShowAsync();
                if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
                {
                    lastName = nameDialog.name.Text;
                    Post.Send("includes/nnch.php", $"a={HttpUtility.UrlEncode(lastName, Encoding.UTF8)}&cid={cid}");


                }
            }
            catch (Exception e)
            {
                Note.Show("Something went wrong");
            }


        }
    }
}
