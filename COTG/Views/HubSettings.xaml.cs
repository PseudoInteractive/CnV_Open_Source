using COTG.Game;
using COTG.JSON;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
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
				var hubName = Spot.GetOrAdd(bestHub).nameAndRemarks;
				instance.bestHub.Text = hubName;
				if (await instance.ShowAsync2() == ContentDialogResult.Primary)
				{

					if (!string.Equals(instance.bestHub.Text, hubName, StringComparison.OrdinalIgnoreCase))
					{
						// Todo
					}
					await CitySettings.SetCitySettings(cid, bestHub);
					return true;
				}
			}
			else
			{
				Note.Show("No hub found");
			}
			return false;
		}
	
	}
}
