﻿using COTG.Game;
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
using static COTG.Debug;
using static COTG.Views.SettingsPage;
using Windows.UI.Xaml.Controls;

namespace COTG.JSON
{
    public class CitySettings
    {
		static HashSet<int> GetHubs()
		{
			var cl = Game.CityList.Find(Views.SettingsPage.hubCitylistName);
//			if (cl != null)
//				return cl.cities;
//			Note.Show("Warning:  No hub city list, querying all cities with the word 'Hub' in their remarks");
			HashSet<int> result = cl!=null ? new(cl.cities) : new ();
			foreach(var i in City.myCities)
			{
				if (i.HasTag(Tags.Hub))
				{
					result.Add(i.cid);
				}
			}
			Note.Show($"{result.Count}  No hubs found");
			return result;

		}

		public static async Task<int> FindBestHub(int cid)
		{
			await NearRes.UpdateTradeStuffifNeeded();
			int reqHub = 0;
			var bestDist = 4096f;
			var hubs = GetHubs();
				foreach (var hub in hubs)
				{
					if (cid == hub)
						continue;
					if (!hub.CanReachByTrade(cid))
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
			foreach (var _cid in Spot.GetSelectedForContextMenu(cid, false,onlyMine:true))
			{
				var hub = await CitySettings.FindBestHub(_cid);
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
		public static async Task SetCitySettings(
			int cid, 
			int? reqHub = null, 
			int? targetHub = null, 
			bool setRecruit = false, 
			bool setAutoBuild = false, 
			bool setResources = false, 
			int? cartReserve = null,
			int? shipReserve = null,
			bool _filterSend = false, 
			bool? autoTowers=null, 
			bool? autoWalls=null,
			bool? sendWood=true, 
			bool? sendStone=true,
			bool? sendIron=true,
			bool? sendFood=true,
			bool? reqWood = true,
			bool? reqStone = true,
			bool? reqIron = true,
			bool? reqFood = true)
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
				//                      0,1,{reqHub},{targetHub},0,0,0,{maxWood},{maxStone},{maxIron}, 40
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
					split[32] = "0"; // use the different city all requests
					if(reqWood.HasValue ) 
						split[28] = reqWood.Value ? reqHub.ToString() : "0";
					if (reqStone.HasValue)
						split[29] = reqStone.Value ? reqHub.ToString() : "0";
					if (reqIron.HasValue)
						split[30] = reqIron.Value ? reqHub.ToString() : "0";
					if (reqFood.HasValue)
						split[31] = reqFood.Value ? reqHub.ToString() : "0";

					//split[42] = reqHub.ToString();
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
					if(sendWood.HasValue)
						split[37] = sendWood.Value ? cid : "0"; // hub to use for this res
					if(sendStone.HasValue)
						split[38] = sendStone.Value ? cid : "0"; // hub to use for this res
					if(sendIron.HasValue)
						split[39] = sendIron.Value ? cid : "0"; // hub to use for this res
					if(sendFood.HasValue)
						split[40] = sendFood.Value  ? cid : "0"; // hub to use for this res

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
				if (shipReserve.HasValue)
				{
					split[46] = shipReserve.ToString();// 45 is % carts reserved for requests
				}

				//                split[43] = sendHub.ToString();


				if (setResources)
				{
					split[47] = maxWood.ToString();
					split[48] = maxStone.ToString();
					split[49] = maxIron.ToString();
					split[50] = maxFood.ToString();
				}
				if (cottageLevel > 0 && setAutoBuild && split[ministerOptionAutobuildCabins] == "10]" )
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
		public static Task SetFoodWarning(int cid, int warn)
		{
			return Post.Send("includes/svFW.php", $"a={warn}&cid={cid}");
		}
		public static async void SetFoodWarnings(int cid)
		{
			var targets = Spot.GetSelectedForContextMenu(cid, false, onlyMine: true);
			var content = new NumberBox();
			content.Value = SettingsPage.defaultFoodWarning;
			var dialog = new ContentDialog()
			{
				Title = $"Set Food warning for {targets.Count} cities",
				Content = content,
				PrimaryButtonText = "Apply",
				SecondaryButtonText = "Cancel"
			};
			if ((await dialog.ShowAsync2()) != ContentDialogResult.Primary)
				return;
			
			SettingsPage.defaultFoodWarning = content.Value.RoundToInt();
			foreach(var id in targets)
			{
				await SetFoodWarning(id, defaultFoodWarning);
				Note.Show($"Set food warning for {City.GetOrAddCity(id).nameMarkdown} to {defaultFoodWarning} hours");
			}

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
                    Note.Show($"Set Minister options settings",Note.Priority.low);
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



		public static async void SetRecruitFromTag(int _cid)
        {
			var targets = Spot.GetSelectedForContextMenu(_cid, onlyMine: true);
			foreach (var cid in targets)
			{

				await UpdateMinisterOptions(cid, async (split) =>
				{
					 SetRecruit(split, City.Get(cid) );
					return true;
				});
			}
			Note.Show($"Updated recruit for {targets.Count} cities");
        }

		const int countMany = 3333333;
		const string sCountMany = "333333";
		const string sZero = "0";
        private static string SetRecruit(string[] split, Spot spot)
		{
			var rem = spot.remarks.ToLower();
			var result = string.Empty;
			var tags = spot.GetTags();
			void CheckTag1( Tags tag, int id, string count = sCountMany, bool zeroIfClear=true)
			{
				if (tags.HasFlag(tag))
				{
					result += $"\nSet recruit {tag}";
					if (split[id] == sZero)
						split[id] = count;
				}
				else
				{
					if(zeroIfClear)
						split[id] = sZero;
				}

			}

			CheckTag1(Tags.Priest, 13);

			if (tags.HasFlag(Tags.VRT))
			{
				// 12 is triari
				// 14 is vanq
				var vrt = SettingsPage.vrtRatio;
//				if (split[11] == sZero || split[12] == sZero || split[14] == sZero)
				{
					var gain = countMany / (vrt.r + vrt.t + vrt.v);
					split[11] = (vrt.r * gain).RoundToInt().ToString();
					split[12] = (vrt.t * gain).RoundToInt().ToString();
					split[14] = (vrt.v * gain).RoundToInt().ToString();
				}
				result = "\nSet recruit VRT";
			}
			else if (tags.HasFlag(Tags.RT) )
			{
				// 12 is triari
				// 11 is ranger
				var vrt = SettingsPage.vrtRatio;
				split[14] = sZero;
	//			if (split[11] == sZero || split[12] == sZero)
				{
					var gain = countMany / (vrt.r + vrt.t );
					split[11] = (vrt.r * gain).RoundToInt().ToString();
					split[12] = (vrt.t * gain).RoundToInt().ToString();
				}
				result = "\nSet recruit rt";
			}
			else if (tags.HasFlag(Tags.VT) )
			{
				// 12 is triari
				// 14 is vanq
				var vrt = SettingsPage.vrtRatio;

				split[11] = sZero;
		//		if (split[12] == sZero || split[14] == sZero)
				{
					var gain = countMany / (vrt.t + vrt.v);
					split[12] = (vrt.t * gain).RoundToInt().ToString();
					split[14] = (vrt.v * gain).RoundToInt().ToString();
				}
				result = "\nSet recruit VT";
			}
			else if (tags.HasFlag(Tags.Vanq))
			{
				split[11] = sZero;
				split[12] = sZero;
				if (split[14] == sZero)
					split[14] = sCountMany;

				result = "\nSet recruit vanqs";

			}
			else
			{
				split[11] = sZero;
				split[12] = sZero;
				split[14] = sZero;
			}

			CheckTag1(Tags.Horse, 19);
			CheckTag1(Tags.Scout, 16,"20000", false);
			CheckTag1(Tags.Arb, 17);
			CheckTag1(Tags.Sorc, 15);
			CheckTag1(Tags.Prae, 18);
			CheckTag1(Tags.Galley, 23, (countMany / 600).ToString(),false );
			CheckTag1(Tags.Druid, 20);


			if (tags.HasFlag(Tags.Scorp))
			{
				result += "\nSet recruit Scorps + Rams";
				if (split[22] == sZero && split[21] == sZero)
				{
					split[22] = (countMany/10).ToString();
					split[21] = (countMany/100).ToString();
				}
			}
			else
			{
				split[21] = sZero;
				split[22] = sZero;
			}
			CheckTag1(Tags.Warship, 25,"900");
			CheckTag1(Tags.Stinger, 24, "3600");
			return result;
		}

		

		public static async void SetTargetHub(int cid, int targetHub)
        {
			var targets = Spot.GetSelectedForContextMenu(cid, false, targetHub, onlyMine: true);
			bool? sendWood=null, sendStone = null, sendFood = null, sendIron = null;
			int? reserveCarts = null, reserveShips = null;
			var result = await App.DispatchOnUIThreadTask(async () =>
			{
				var panel = new StackPanel();
				panel.Children.Add(new TextBlock() 
				{ Text= $"Set {Spot.GetOrAdd(cid).nameAndRemarks}{(targets.Count>1?" and "+(targets.Count-1)+" others)" : string.Empty)} to send resources to {Spot.GetOrAdd(targetHub).nameAndRemarks}" });
				var sendW = new CheckBox() { Content = "Send Wood", IsThreeState = true, IsChecked = null };
				var sendTip = "Check:  Send, Empty: Don't send, Square: Leave as is";
				var sendS = new CheckBox() { Content = "Send Stone", IsThreeState = true, IsChecked = null };
				var sendI = new CheckBox() { Content = "Send Iron", IsThreeState = true, IsChecked = null };
				var sendF = new CheckBox() { Content = "Send Food", IsThreeState = true, IsChecked = null };
				ToolTipService.SetToolTip(sendW, sendTip);
				ToolTipService.SetToolTip(sendS, sendTip);
				ToolTipService.SetToolTip(sendI, sendTip);
				ToolTipService.SetToolTip(sendF, sendTip);
				var reserveS = new NumberBox() { Header = "Reserve Ships", PlaceholderText="No Change" };
				var reserveC = new NumberBox() { Header = "Reserve Carts", PlaceholderText = "No Change" };

				var reserveTip = "Reserves ships/carts for requests rather than sending them all on overflow (mainly for hubs)";
				ToolTipService.SetToolTip(reserveS, reserveTip);
				ToolTipService.SetToolTip(reserveC, reserveTip);

				panel.Children.Add(sendW);
				panel.Children.Add(sendS);
				panel.Children.Add(sendI);
				panel.Children.Add(sendF);
				panel.Children.Add(reserveC);
				panel.Children.Add(reserveS);

				var dialog = new ContentDialog()
				{
					Title = $"Set Target Hub",
					Content = panel,
					PrimaryButtonText = "Yes",
					SecondaryButtonText = "Cancel"
				};
				var rv = await dialog.ShowAsync2();
				sendWood = sendW.IsChecked;
				sendStone = sendS.IsChecked;
				sendIron = sendI.IsChecked;
				sendFood = sendF.IsChecked;
				reserveCarts = reserveC.Text.IsNullOrEmpty() ? null : reserveC.Value.RoundToIntOrNAN();
				reserveShips = reserveS.Text.IsNullOrEmpty() ? null : reserveS.Value.RoundToIntOrNAN();
				
				return rv;
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
						var i = await App.DoYesNoBox("Hub Selected", $"Double checkin: Send resources from {city.nameAndRemarks}'s?");
						if(i == 0)
						{
							continue;
						}
						if ( i == -1)
						{
							break;
						}
					}
					await CitySettings.SetCitySettings(_cid, targetHub: targetHub,
							sendWood:sendWood,sendStone:sendStone,sendIron:sendIron,sendFood:sendFood,
							cartReserve:reserveCarts,shipReserve:reserveShips);
				}
			}
        }

		public static async void SetAutoTowersOrWalls(int cid, bool ? autoWalls=null,bool ? autoTowers=null)
		{
			var targets = Spot.GetSelectedForContextMenu(cid, onlyMine: true);
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
		//public static async void SetSourceHub(int cid, int targetHub)
		//{
		//	var targets = Spot.GetSelectedForContextMenu(cid, false,targetHub);
		//	var result = await App.DispatchOnUIThreadTask(async () =>
		//	{
		//		var dialog = new ContentDialog()
		//		{
		//			Title = $"Set Trade Settings",
		//			Content = $"Set {Spot.GetOrAdd(cid).nameMarkdown} to request resources from {Spot.GetOrAdd(targetHub).nameMarkdown} ({targets.Count} cities selected)",
		//			PrimaryButtonText = "Yes",
		//			SecondaryButtonText = "Cancel"
		//		};
		//	return await dialog.ShowAsync2();
		//});

		//	if (result != ContentDialogResult.Primary)
		//	{
		//		return;
		//	}
		//	foreach (var _cid in targets)
		//	{
		//		if(_cid != targetHub)
		//			await CitySettings.SetCitySettings(_cid, targetHub);
		//	}
		//}
		public static async void SetSourceHub(int cid, int targetHub)
		{ 
		var targets = Spot.GetSelectedForContextMenu(cid, false, targetHub, onlyMine: true);
		bool? sendWood = null, sendStone = null, sendFood = null, sendIron = null;
		var result = await App.DispatchOnUIThreadTask(async () =>
		{
			var panel = new StackPanel();
			panel.Children.Add(new TextBlock()
			{ Text = $"Set {Spot.GetOrAdd(cid).nameAndRemarks}{(targets.Count > 1 ? " and " + (targets.Count - 1) + " others)" : string.Empty)} to send resources to {Spot.GetOrAdd(targetHub).nameAndRemarks}" });
			var sendW = new CheckBox() { Content = "Send Wood", IsThreeState = true, IsChecked = null };
			var sendTip = "Check:  Send, Empty: Don't send, Square: Leave as is";
			var sendS = new CheckBox() { Content = "Send Stone", IsThreeState = true, IsChecked = null };
			var sendI = new CheckBox() { Content = "Send Iron", IsThreeState = true, IsChecked = null };
			var sendF = new CheckBox() { Content = "Send Food", IsThreeState = true, IsChecked = null };
			ToolTipService.SetToolTip(sendW, sendTip);
			ToolTipService.SetToolTip(sendS, sendTip);
			ToolTipService.SetToolTip(sendI, sendTip);
			ToolTipService.SetToolTip(sendF, sendTip);


			panel.Children.Add(sendW);
			panel.Children.Add(sendS);
			panel.Children.Add(sendI);
			panel.Children.Add(sendF);

			var dialog = new ContentDialog()
			{
				Title = $"Set Source Hub",
				Content = panel,
				PrimaryButtonText = "Yes",
				SecondaryButtonText = "Cancel"
			};
			var rv = await dialog.ShowAsync2();
			sendWood = sendW.IsChecked;
			sendStone = sendS.IsChecked;
			sendIron = sendI.IsChecked;
			sendFood = sendF.IsChecked;

			return rv;
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
					await CitySettings.SetCitySettings(_cid, reqHub: targetHub,
							reqWood: sendWood, reqStone: sendStone, reqIron: sendIron, reqFood: sendFood
							);
				}
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
