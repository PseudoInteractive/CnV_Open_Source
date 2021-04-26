using COTG.Game;
using COTG.JSON;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace COTG.Views
{
	public sealed partial class HubSettings : ContentDialog
	{
		public static HubSettings instance = new HubSettings();
		public HubSettings()
		{
			this.InitializeComponent();
		}
		public static async Task<bool> Show(int cid)
		{
			var bestHub = CitySettings.FindBestHub(cid);
			if (bestHub != 0)
			{
				var result = await App.DispatchOnUIThreadTask(async () =>
				{

					var hubName = Spot.GetOrAdd(bestHub).nameAndRemarks;
					instance.bestHub.Text = hubName;
					instance.Bindings.Update();
					if (await instance.ShowAsync2() == ContentDialogResult.Primary)
					{

						if (!string.Equals(instance.bestHub.Text, hubName, StringComparison.OrdinalIgnoreCase))
						{
							// Todo
						}
						await CitySettings.SetCitySettings(cid, bestHub,CitySettings.FilterTargetHub(cid,bestHub),SettingsPage.setRecruit,setAutoBuild:true,setResources:true,filterSend:true,
								autoWalls:(SettingsPage.autoWallLevel==10)?true:null,
								autoTowers: (SettingsPage.autoTowerLevel == 10) ? true : null
								);
						return true;
					}
					else
					{
						return false;
					}
				});
				return result;
			}
			else
			{
				Note.Show("No hub found");
			}
			return false;
		}

	

		public TradeSettings _TradeSettingsSel;
		public TradeSettings tradeSettingsSel
		{
			get => _TradeSettingsSel;
			set
			{
				_TradeSettingsSel = value;
				SettingsPage.reqWood = value.requestWood;
				SettingsPage.reqStone = value.requestStone;
				SettingsPage.reqIron = value.requestIron;
				SettingsPage.reqFood = value.requestFood; ;
				SettingsPage.sendWood = value.destWood != 0;
				SettingsPage.sendStone = value.destStone != 0;
				SettingsPage.sendIron = value.destIron != 0;
				SettingsPage.sendFood = value.destFood != 0;
				SettingsPage.maxWood = value.sendWood;
				SettingsPage.maxStone = value.sendStone;
				SettingsPage.maxIron = value.sendIron;
				SettingsPage.maxFood = value.sendFood;
				instance.Bindings.Update();

			}
		}

	}
}
