using Microsoft.UI.Xaml.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV;
using static CnV.CitySettings;
public partial class CityUI
{
	public static async Task SetTradeSettings(int _cid, int? sourceHub = null, ResourceFilter reqFilter = default, int? targetHub = null, ResourceFilter targetFilter = default, bool autoFind = false)
	{
		//	var targetExplicit = targetHub.HasValue;
		foreach(var __cid in Spot.GetSelectedForContextMenu(_cid, false, onlyMine: true))
		{
			var cid  = __cid;
			var city = City.Get(cid);

			if(autoFind)
			{
				sourceHub = targetHub = await FindBestHubWithChoice(cid, "Find Hub");
			}

			if(cid == sourceHub || cid == targetHub)
				continue;

			var _sourceHub = sourceHub is not null ? sourceHub.Value :  city.AnyHub(true);
			var _targetHub = targetHub is not null ? targetHub.Value :  city.AnyHub(false);


			var settings = new ResSettings();
			settings.InitTradeSettings(city, _sourceHub, _targetHub, reqFilter, targetFilter);
		//	await AUtil.AwaitChangesComplete();

			var dialog = new ContentDialog()
			{
					Content             = settings,
					PrimaryButtonText   = "Apply",
					SecondaryButtonText = "Skip",
					CloseButtonText     = "Cancel"
			};

			dialog.Title = $"Set Trade settings for {city.nameAndRemarks}";
			//	await settings.InitTradeSettings(city,, (city.isHubOrStorage&&!targetExplicit) ? 0 : targetHub.GetValueOrDefault() );
			var rv = await dialog.ShowAsync2();
			if(rv == ContentDialogResult.Primary)
			{
				reqFilter    = settings.reqFilter;
				targetFilter = settings.sendFilter;
				// does this change threads?
				City.Get(cid).SetTradeSettings( reqHub: settings.ReqHub, settings.SendHub, req: settings.req, max: settings.max,
									cartReserve: settings.cartReserve,
									shipReserve: settings.shipReserve,
									reqFilter: settings.reqFilter,
									sendFilter: settings.sendFilter);
			}
			else if(rv == ContentDialogResult.None)
			{
				break;
			}
		}
	}
	public static  void SetTargetHub(int cid, int targetHub)
	{
		SetTradeSettings(cid, sourceHub: null, targetHub: targetHub, targetFilter: ResourceFilter._true);

	}
	public static  void SetSourceHub(int cid, int targetHub)
	{
		SetTradeSettings(cid, sourceHub: targetHub, targetHub: targetHub, reqFilter: ResourceFilter._true);

	}
	public static async Task<int> FindBestHubWithChoice(int cid, string title, bool? offContinent = null, bool? isHubOrStorage = null)
	{
		var city = cid.AsCity();
		isHubOrStorage ??= city.isHubOrStorage;
		offContinent ??= (city.isHubOrStorage && city.isOnWater) ? (await AppS.DoYesNoBox(title, $"Find hub for {city} from another Continent?",
																		yes: "Off Continent", no: "Same Continent", cancel: null)) == 1 : false;
		return CitySettings.FindBestHub(cid,
											offContinent.GetValueOrDefault());
	}
	public static Task SetClosestHub(int cid)
	{
		return SetTradeSettings(cid, autoFind: true, reqFilter: ResourceFilter._true, targetFilter: (City.Get(cid).isHubOrStorage ? ResourceFilter._null : ResourceFilter._true));
	}

}

