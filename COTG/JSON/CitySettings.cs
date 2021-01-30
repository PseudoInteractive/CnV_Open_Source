using COTG.Game;
using COTG.Services;
using COTG.Views;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Windows.UI.Popups;

using static COTG.Views.SettingsPage;
namespace COTG.JSON
{
    public class CitySettings
    {
		public static int FindBestHub(int cid)
		{
			var cl = Game.CityList.Find(Views.SettingsPage.hubCitylistName);
			
			int reqHub = 0;
			var bestDist = 4096f;
			foreach (var hub in cl.cities)
			{
				if (cid == hub)
					continue;

				var d = hub.DistanceToCid(cid);
				if (d < bestDist)
				{
					bestDist = d;
					reqHub = hub;
				}

			}
			return reqHub;
		}
		public static async void SetHub(int cid)
		{
			foreach (var _cid in Spot.GetSelectedForContextMenu(cid, false))
			{
				await CitySettings.SetCitySettings(_cid, CitySettings.FindBestHub(_cid));
			}
		}
			public static async Task SetCitySettings(int cid, int reqHub)
        {
            await UpdateMinisterOptions(cid, async (split) =>
            {

				var spot = Spot.GetOrAdd(cid);
				if (reqHub < 0)
					reqHub = 0;
			//	var cl = Game.CityList.Find(Views.SettingsPage.hubCitylistName);

           //     int reqHub = 0;
                //var bestDist = 4096f;
                //foreach (var hub in cl.cities)
                //{
                //    if (cid == hub)
                //        continue;

                //    var d = hub.DistanceToCid(cid);
                //    if (d < bestDist)
                //    {
                //        bestDist = d;
                //        reqHub = hub;
                //    }

                //}


                //        var args = $"[1,{auto},{auto},{auto},{auto},{auto},{auto},{auto},0,0,   0
                //                      0,0,0,0,0,0,0,0,0,0,                                      10
                //                      0,0,0,0,0,0,0,0,0,0,                                      20
                //                      0,0,1,{reqWood},{reqStone},{reqIron},{reqFood},0,0,0,     30
                //                      0,1,{reqHub},{reqHub},0,0,0,{maxWood},{maxStone},{maxIron}, 40
                //                     {maxFood},[1,{cottageLevel}],[1,10],[1,10],[1,10],[1,       50
                //                      10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10]]";
                //                var args = $"[1,{auto},{auto},{auto},{auto},{auto},{auto},{auto},0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,{reqWood},{reqStone},{reqIron},{reqFood},0,0,0,0,1,{reqHub},{reqHub},0,0,0,{maxWood},{maxStone},{maxIron},{maxFood},[1,{cottageLevel}],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10]]";

                if(autoBuildOn.HasValue)
                {
                    var autoVal = autoBuildOn.GetValueOrDefault();
                    var auto = autoVal ? "1" : "0";
                    split[0] = '[' + auto;
                    for (int i = 1; i < 8; ++i)
                        split[i] = auto;
                    if(autoVal)
                    {
                        for (int i = 51; i < 51+24*2; i+=2)
                            split[i] = '[' + auto;

                    }

                }
                split[32] = "1"; // use the same city all requests
                split[33] = reqWood.ToString();
                split[34] = reqStone.ToString();
                split[35] = reqIron.ToString();
                split[36] = reqFood.ToString();

				// hubs dont send by default
				var isHub = spot.HasTag(Tag.Hub);

				// send target
				split[37] = sendWood&& !isHub ? reqHub.ToString() : "0"; // hub to use for this res
                split[38] = sendStone && !isHub ? reqHub.ToString() : "0"; // hub to use for this res
                split[39] = sendIron && !isHub ? reqHub.ToString() : "0"; // hub to use for this res
                split[40] = sendFood && !isHub ? reqHub.ToString() : "0"; // hub to use for this res
                split[41] = "0"; // use a different city for all sends


                split[42] = reqHub.ToString();
                //                split[43] = sendHub.ToString();

                split[45] = cartsAreForRequests ? "100" : "0"; // 45 is % carts reserved for requests
				

				split[47] = maxWood.ToString();
                split[48] = maxStone.ToString();
                split[49] = maxIron.ToString();
                split[50] = maxFood.ToString();
                if(cottageLevel > 0)
                    split[52] = cottageLevel.ToString() + ']';
                
				var str = SettingsPage.setRecruit ? SetRecruit(split, spot) : "";
                
				Note.Show($"Set hub to {Spot.GetOrAdd(reqHub).cityName}{str}");
                return true;
            });


        }
        public static async Task UpdateMinisterOptions(int cid, Func<string[],Task<bool>> opts)
        {
            string args = null;
            await GetCity.Post(cid, (jse, city) =>
                {
                    if (jse.TryGetProperty("mo", out var p))
                    {
                        args = p.ToString();
                    }
                });

              try
              {
                var split = args.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 99)
                {
                    COTG.Debug.Log($"Invalid options {split.Length}");
                    var defaults = "[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10]]";
                    split = defaults.Split(',', StringSplitOptions.RemoveEmptyEntries);
                }
                if (await opts(split))
                {
                    var args2 = string.Join(',', split);
                    await Post.Send("includes/mnio.php", $"a={HttpUtility.UrlEncode(args2, Encoding.UTF8)}&b={cid}");
                    // find closest hub
                    Note.Show($"Set hub settings",true);
                }
            }
            catch (Exception e)
            {
                COTG.Debug.Log(e);
                Note.Show($"Set hub failed, restarting might fix it");
            }


        }
        public static void SetRecruitFromTag(int cid)
        {
            var spot = Spot.GetOrAdd(cid);

            UpdateMinisterOptions(cid, async (split) =>
            {
                SetRecruit(split, spot);
                return true;
            });
        }

        private static string SetRecruit(string[] split, Spot spot)
        {
            var rem = spot.remarks.ToLower();
            var result = string.Empty;
            if (rem.Contains("priest"))
            {
                split[13] = "343343";
                result = "\nSet recruit priestess";
            }
            if (rem.Contains("rt") || rem.Contains("ranger") || rem.Contains("triari"))
            {
                // 12 is triari
                // 11 is ranger
                split[11] = "200000";
                split[12] = "100000";
                result = "\nSet recruit rt";
            }
            if (rem.Contains("vanq"))
            {
                split[14] = "343343";
               result = "\nSet recruit vanqs";

            }
            if (rem.Contains("arb"))
            {
                result = "\nSet recruit arbs";
                split[17] = "343343";
            }
            if (rem.Contains("horse"))
            {
                result = "\nSet recruit horses";
                split[19] = "343343";
            }
            if (rem.Contains("sorc"))
            {
                result = "\nSet recruit sorcs";
                split[15] = "343343";
            }
            if (rem.Contains("prae"))
            {
                result = "\nSet recruit prae";
                split[18] = "343343";
            }
            if (rem.Contains("galley"))
            {
                result += "\nSet recruit Gallys";
                split[23] = "480";
            }
			if (rem.Contains("scorp"))
			{
				result += "\nSet recruit Scorps + Rams";
				split[22] = "20000";
				split[21] = "2000";
			}
			return result;
        }

        public static void SetTargetHub(int cid, int targetHub)
        {
            UpdateMinisterOptions(cid,async (split) =>
            {
                split[37] = sendWood? targetHub.ToString() : "0"; // hub to use for this res
                split[38] = sendStone?targetHub.ToString() : "0"; // hub to use for this res
                split[39] = sendIron?targetHub.ToString() : "0"; // hub to use for this res
                split[40] = sendFood?targetHub.ToString() : "0"; // hub to use for this res
                split[41] = "0"; // use a different city for all sends
                split[45] = cartsAreForRequests ? "100" : "0"; // 45 is % carts reserved for requetestbmini
                                                                   //         split[43] = targetHub.ToString();
                return true;
            });
        }

        public static async Task FixupReserve(int cid)
        {
            await UpdateMinisterOptions(cid,async (split) =>
            {
                {
                    bool anyTroops = false;
                    for (int i = 11; i < 24; ++i)
                    {
                        if (split[i] != "0")
                            anyTroops = true;
                    }
                    if (!anyTroops)
                    {
                        var s = Spot.GetOrAdd(cid);
                        if (!s.remarks.ToLower().Contains("hub") && !s.cityName.StartsWith("*"))
                        {
                            Note.Show($"No troops {s.nameAndRemarks} {s.cid.CidToStringMD()}");
                            SpotTab.TouchSpot(s.cid, Windows.System.VirtualKeyModifiers.Shift);
                        }
                    }
                }
                if (split[45] == "100")
                {
                    COTG.Debug.Log(Spot.GetOrAdd(cid).nameAndRemarks);
                    var messageDialog = new MessageDialog($"Fix {Spot.GetOrAdd(cid).nameAndRemarks}")
                    { DefaultCommandIndex=0,CancelCommandIndex=1};
                    messageDialog.Commands.Add(new UICommand( "Yes"));
                    messageDialog.Commands.Add(new UICommand("No"));


                    var i = await messageDialog.ShowAsync();
                    if ((i?.Label == "Yes"))
                    {
                        split[45] = "0";
                        return true;
                    }
                    return false;
                }// 45 is % carts reserved for requests
                return false;                                                 //         split[43] = targetHub.ToString();
            });
        }
        //public static void SetOtherHubSettings(int cid, int sourceHub)
        //{
        //    UpdateMinisterOptions(sourceHub, (split) =>
        //    {
        //        split[33] = reqWood.ToString();
        //        split[34] = reqStone.ToString();
        //        split[35] = reqIron.ToString();
        //        split[36] = reqFood.ToString();
        //        split[47] = maxWood.ToString();
        //        split[48] = maxStone.ToString();
        //        split[49] = maxIron.ToString();
        //        split[50] = maxFood.ToString();
        //        split[43] = cid.ToString();
        //    });
        //}
     
    }
}
