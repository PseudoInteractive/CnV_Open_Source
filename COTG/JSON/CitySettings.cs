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
using Microsoft.UI.Xaml.Controls;
using ContentDialog = Windows.UI.Xaml.Controls.ContentDialog;
using ContentDialogResult = Windows.UI.Xaml.Controls.ContentDialogResult;

using static COTG.Views.SettingsPage;
namespace COTG.JSON
{
    public class CitySettings
    {
		static HashSet<int> GetHubs()
		{
			var cl = Game.CityList.Find(Views.SettingsPage.hubCitylistName);
			if (cl != null)
				return cl.cities;
			Note.Show("Warning:  No hub city list, querying all cities with the word 'Hub' in their remarks");
			HashSet<int> result = new HashSet<int>();
			foreach(var i in City.myCities)
			{
				if (i.remarks.Contains("hub", StringComparison.OrdinalIgnoreCase))
				{
					result.Add(i.cid);
				}
			}
			if(result.Count==0)
			{
				Note.Show("Warning:  No hubs found");
			}
			return result;

		}

		public static int FindBestHub(int cid)
		{
			int reqHub = 0;
			var bestDist = 4096f;
			var hubs = GetHubs();
			
				foreach (var hub in hubs)
				{
					if (cid == hub)
						continue;
					if (!hub.CanReach(cid))
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
				var hub = CitySettings.FindBestHub(_cid);
				await CitySettings.SetCitySettings(_cid,hub, FilterTargetHub(cid,hub) );
			}

		}

		public static int? FilterTargetHub( City me, int hub)
		{
			return me.isHubOrStorage ? 0 : hub;
		}
		public static int? FilterTargetHub(int me, int hub)
		{
			return FilterTargetHub(City.Get(me), hub);
		}

		public const int ministerOptionAutobuildWalls = 26;
		public const int ministerOptionAutobuildTowers = 27;
		public const int ministerOptionAutobuildCabins = 52;
		public static async Task SetCitySettings(int cid, int? reqHub = null, int? targetHub = null, bool setRecruit = false, bool setAutoBuild = false, bool setResources = false, int? cartReserve = null, bool filterSend = false, bool? autoTowers=null, bool? autoWalls=null)
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

				if (autoBuildOn.HasValue & setAutoBuild)
				{
					var autoVal = autoBuildOn.GetValueOrDefault();
					var auto = autoVal ? "1" : "0";
					split[0] = '[' + auto;
					for (int i = 1; i < 8; ++i)
						split[i] = auto;
					if (autoVal)
					{
						for (int i = 51; i < 51 + 24 * 2; i += 2)
							split[i] = '[' + auto;

					}

				}
				if (reqHub.HasValue)
				{
					split[32] = "1"; // use the same city all requests
					split[42] = reqHub.ToString();
				}
				if (autoWalls.HasValue)
				{
					split[ministerOptionAutobuildWalls] = autoWalls.Value ? "1" : "0";
				}
				if (autoTowers.HasValue)
				{
					split[ministerOptionAutobuildTowers] = autoTowers.Value? "1" : "0";
				}

				if (setResources)
				{
					split[33] = reqWood.ToString();
					split[34] = reqStone.ToString();
					split[35] = reqIron.ToString();
					split[36] = reqFood.ToString();
				}
				if (targetHub.HasValue)
				{
					var cid = targetHub.ToString();
					split[41] = "0"; // use a different city for all sends
									 // hubs dont send by default
									 // send target
					split[37] = sendWood || !filterSend ? cid : "0"; // hub to use for this res
					split[38] = sendStone || !filterSend ? cid : "0"; // hub to use for this res
					split[39] = sendIron || !filterSend ? cid : "0"; // hub to use for this res
					split[40] = sendFood || !filterSend ? cid : "0"; // hub to use for this res

				}
				int resultSourceHub = 0;
				if (split[32] == "1")
				{
					int.TryParse(split[42], out resultSourceHub);

				}
				else
				{

					for (int i = 28; i <= 31; ++i)
					{
						int.TryParse(split[i], out resultSourceHub);
						if (resultSourceHub != 0)
							break;
					}
				}

				int resultTargetHub = 0;
				if (split[41] == "1")
				{
					int.TryParse(split[43], out resultTargetHub);
				}
				else
				{
					for (int i = 37; i <= 40; ++i)
					{
						int.TryParse(split[i], out resultTargetHub);
						if (resultTargetHub != 0)
							break;
					}

				}

				if (cartReserve.HasValue)
				{
					split[45] = cartReserve.ToString();// 45 is % carts reserved for requests
				}

				//                split[43] = sendHub.ToString();


				if (setResources)
				{
					split[47] = maxWood.ToString();
					split[48] = maxStone.ToString();
					split[49] = maxIron.ToString();
					split[50] = maxFood.ToString();
				}
				if (cottageLevel > 0 && setAutoBuild)
				{
					split[ministerOptionAutobuildCabins] = cottageLevel.ToString() + ']';

				}
				var str = setRecruit ? SetRecruit(split, spot) : "";
                
				Note.Show($"Set {City.Get(cid).nameMarkdown}'s trade settings src:{City.Get(resultSourceHub).nameMarkdown} dest:{City.Get(resultTargetHub).nameMarkdown}");
                return true;
            });


        }
		public static async Task<string[]> GetMinisterOptions(City city, bool force)
		{
				if (city.ministerOptions == null || force)
				{
					for (; ; )
					{
						await GetCity.Post(city.cid);
						if (city.ministerOptions != null)
							break;

						if (!city.isCityOrCastle)
						{
							throw new UIException(city.nameAndRemarks);
						}
						await Task.Delay(500);
					}
				}
				return GetMinisterOptionsNoFetch(city);
			
		}
		public static string[] GetMinisterOptionsNoFetch(City city)
		{
			string[] rv = null;
			try
			{
				if (city.ministerOptions != null)
				{
					rv = city.ministerOptions.Split(',', StringSplitOptions.RemoveEmptyEntries);
					

				}
			}
			catch (Exception ex)
			{
				Debug.LogEx(ex);
			}
			if (rv == null || rv.Length != 99)
			{
				COTG.Debug.Log($"Invalid options");
				const string defaults = "[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10],[1,10]]";
				rv = defaults.Split(',', StringSplitOptions.RemoveEmptyEntries);
			}
			return rv;
		}

		public static async Task UpdateMinisterOptions(int cid, Func<string[],Task<bool>> opts)
        {
			var city = City.GetOrAddCity(cid);
            
			try
			{
				var split = await GetMinisterOptions(city,true);
			    if (await opts(split))
                {
                    var args2 = string.Join(',', split);
					city.SetMinisterOptions(args2);
                    await Post.Send("includes/mnio.php", $"a={HttpUtility.UrlEncode(args2, Encoding.UTF8)}&b={cid}", World.CidToPlayerOrMe(cid));
                    // find closest hub
                    Note.Show($"Set Minister options settings",true);
                }
            }
            catch (Exception e)
            {
                COTG.Debug.LogEx(e);
                Note.Show($"Set hub failed, restarting might fix it");
            }
        }

		public static async Task<(Resources req,Resources max)> GetTradeResourcesSettings(int cid) 
		{
			var city = City.GetOrAddCity(cid);
			var split = await GetMinisterOptions(city, true);
	
			Resources req = new Resources();
			Resources max = new Resources();
			for(int i=33;i<=36;++i)
			{
				if(split[i].TryParseInt( out var v))
				{
					req[i - 33] = v;
				}
			}
			for (int i = 47; i <= 50; ++i)
			{
				if (split[i].TryParseInt(out var v))
				{
					max[i - 47] = v;
				}
			}

			return (req, max);
		}

		public static async Task SetTradeResourcesSettings(int cid, Resources req, Resources max )
		{
			var city = City.GetOrAddCity(cid);
			await UpdateMinisterOptions(cid,async (split) =>
			{


				for (int i = 33; i <= 36; ++i)
				{
					var id = i - 33;
					if(req[id] >= 0)
					{
						split[i] = req[id].ToString();
					}
				}
				for (int i = 47; i <= 50; ++i)
				{
					var id = i - 47;
					if (max[id] >= 0)
					{
						split[i] = max[id].ToString();
					}
				}
				return true;
			});

		}
		//		Name Protocol    Method Result  Content type    Received Time    Initiator
		//https://w21.crownofthegods.com/includes/pSs.php	HTTP/1.0	POST	200	text/html		85.7 ms	XMLHttpRequest



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
			for(int i = 11;i<26;++i)
				split[i] = "0";

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
			if (rem.Contains("vt") )
			{
				// 12 is triari
				// 14 is vanq
				split[12] = "200000";
				split[14] = "200000";
				result = "\nSet recruit VT";
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
                split[23] = "440";
            }
			if (rem.Contains("scorp"))
			{
				result += "\nSet recruit Scorps + Rams";
				split[22] = "20000";
				split[21] = "2000";
			}
			if (rem.Contains("warship"))
			{
				result += "\nSet recruit Warships";
				split[25] = "900";
			}
			if (rem.Contains("stinger"))
			{
				result += "\nSet recruit Stinger";
				split[24] = "3600";
			}
			return result;
        }

        public static async void SetTargetHub(int cid, int targetHub)
        {
			var targets = Spot.GetSelectedForContextMenu(cid, false, targetHub);
			var result = await App.DispatchOnUIThreadTask(async () =>
			{
				var dialog = new ContentDialog()
				{
					Title = $"Set Trade Settings",
					Content = $"Set {Spot.GetOrAdd(cid).nameAndRemarks} to send resources to {Spot.GetOrAdd(targetHub).nameAndRemarks} ({targets.Count} cities selected)",
					PrimaryButtonText = "Yes",
					SecondaryButtonText = "Cancel"
				};
				return await dialog.ShowAsync2();
			});
			if (result != ContentDialogResult.Primary)
			{
				return;
			}
			foreach (var _cid in targets )
			{
				if (targetHub != _cid)
				{
					var city = City.Get(_cid);
					if(city.isHubOrStorage)
					{
						var i = await App.DoYesNoBox("Hub Target?", $"Set {city.nameAndRemarks}'s target to {City.Get(targetHub)}?");
						if(i == 0)
						{
							continue;
						}
						if ( i == -1)
						{
							break;
						}
					}
					await CitySettings.SetCitySettings(_cid, null, targetHub);
				}
			}
        }

		public static async void SetAutoTowersOrWalls(int cid, bool ? autoWalls=null,bool ? autoTowers=null)
		{
			var targets = Spot.GetSelectedForContextMenu(cid);
			foreach (var _cid in targets)
			{
				await CitySettings.SetCitySettings(_cid,autoWalls:autoWalls,autoTowers:autoTowers);
			}
		}
		public static (bool?autoTowers,bool?autoWalls) IsAutoTowersAndWallsSet(City city)
		{
			if (city.ministerOptions == null)
				return (null,null);
			var split = GetMinisterOptionsNoFetch(city);
			return (split[ministerOptionAutobuildTowers] == "1", split[ministerOptionAutobuildWalls] == "1");
		}
		public static async void SetSourceHub(int cid, int targetHub)
		{
			var targets = Spot.GetSelectedForContextMenu(cid, false,targetHub);
			var result = await App.DispatchOnUIThreadTask(async () =>
			{
				var dialog = new ContentDialog()
				{
					Title = $"Set Trade Settings",
					Content = $"Set {Spot.GetOrAdd(cid).nameMarkdown} to request resources from {Spot.GetOrAdd(targetHub).nameMarkdown} ({targets.Count} cities selected)",
					PrimaryButtonText = "Yes",
					SecondaryButtonText = "Cancel"
				};
			return await dialog.ShowAsync2();
		});

			if (result != ContentDialogResult.Primary)
			{
				return;
			}
			foreach (var _cid in targets)
			{
				if(_cid != targetHub)
					await CitySettings.SetCitySettings(_cid, targetHub);
			}
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
