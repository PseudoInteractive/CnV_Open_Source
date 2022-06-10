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
	public static async Task SetTradeSettings(int _cid, int? sourceHub = null,  int? targetHub = null,  bool autoFind = false)
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


			var settings = new TradeSettingsControl();
			settings.city = city; //  InitTradeSettings(city, _sourceHub, _targetHub, reqFilter, targetFilter);
			settings.InitializeFromCity();
			if(sourceHub is not null ) {
				var h = sourceHub.Value.AsCity();
				settings.woodSource.SetCity(h,false);
				settings.stoneSource.SetCity(h,false);
				settings.ironSource.SetCity(h,false);
				settings.foodSource.SetCity(h,false);
			}
			if(targetHub is not null && !city.isHubOrStorage ) {
				var h = targetHub.Value.AsCity();
				settings.woodDest.SetCity(h,false);
				settings.stoneDest.SetCity(h,false);
				settings.ironDest.SetCity(h,false);
				settings.foodDest.SetCity(h,false);
			}
			
		//	await AUtil.AwaitChangesComplete();

			var dialog = new ContentDialog()
			{
					Content             = settings,
					PrimaryButtonText   = "Apply",
					SecondaryButtonText = "Skip",
					CloseButtonText     = "Cancel"
			};

			dialog.SetTitle( $"Set Trade settings for {city.nameAndRemarks}" );
			//	await settings.InitTradeSettings(city,, (city.isHubOrStorage&&!targetExplicit) ? 0 : targetHub.GetValueOrDefault() );
			var rv = await dialog.ShowAsync2();
			if(rv == ContentDialogResult.Primary)
			{
				settings.Apply();
			}
			else if(rv == ContentDialogResult.None)
			{
				break;
			}
		}
	}
	public static  void SetTargetHub(int cid, int targetHub)
	{
		SetTradeSettings(cid, sourceHub: null, targetHub: targetHub);

	}
	public static  void SetSourceHub(int cid, int targetHub)
	{
		SetTradeSettings(cid, sourceHub: targetHub, targetHub: targetHub);

	}
	public static Task SetClosestHub(int cid)
	{
		return SetTradeSettings(cid, autoFind: true );
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


}

