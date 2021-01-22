using COTG.Game;
using COTG.JSON;
using COTG.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Web;

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
    public sealed partial class CityRename : ContentDialog
        {
		public CityRename()
            {
            this.InitializeComponent();
            }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
            {
            }

		static string SetTag(string src, string tag, bool? isOn)
		{
			if (isOn == null)
				return src;
			var exists = src.Contains(tag, StringComparison.OrdinalIgnoreCase);
			if (isOn.GetValueOrDefault())
			{
				if (exists)
					return src;
				else
				{
					// add it
					if (src.Length > 0)
						src += " ";
					return src + tag;
				}
			}
			else
			{
				if (exists)
				{
					src = src.Replace(tag + " ", "", StringComparison.OrdinalIgnoreCase);
					src = src.Replace(" " + tag, "", StringComparison.OrdinalIgnoreCase);
					src = src.Replace(tag, "", StringComparison.OrdinalIgnoreCase);
					return src;
				}
				else
					return src;
			}
		}
		static string lastName = string.Empty;
		public static async void RenameDialog(int cid)
		{
			try
			{
				var city = City.GetOrAddCity(cid);
				var nameDialog = new CityRename();
				var isNew = city._cityName == "*New City" || city._cityName == "*Lawless City";


				var name = isNew ? lastName : city._cityName;
				if (name.IsNullOrEmpty())
					name = $"{city.cont:00} 1001";

				var lg = name.Length;
				var numberEnd = lg;
				while (numberEnd > 0 && char.IsNumber(name[numberEnd - 1]))
				{
					--numberEnd;
				}
				if (numberEnd < lg)
				{
					// increment number if there is one
					var start = name.Substring(0, numberEnd);
					var number = int.Parse(name.Substring(numberEnd, lg - numberEnd)) + (name == lastName ? 1 : 0);
					for (; ; )
					{
						name = start + number.ToString();
						if (!City.myCities.Any((v) => v._cityName == name && v != city))
							break;
						++number;
					}
				}
				nameDialog.name.Text = city._cityName;
				nameDialog.suggested.Text = name;
				//	ElementSoundPlayer.Play(ElementSoundKind.Show);

				var result = await nameDialog.ShowAsync2();
				if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
				{
					if (!SettingsPage.useSuggested)
						lastName = nameDialog.name.Text;
					else
						lastName = nameDialog.suggested.Text;
					city._cityName = lastName;
					Post.Send("includes/nnch.php", $"a={HttpUtility.UrlEncode(lastName, Encoding.UTF8)}&cid={cid}");
					if (SettingsPage.applyTags)
					{
						await GetCity.Post(cid); // need to fetch notes
						string tags = city.remarks;

						tags = SetTag(tags, "Hub", nameDialog.hub.IsChecked);
						tags = SetTag(tags, "Vanq", nameDialog.vanqs.IsChecked);
						tags = SetTag(tags, "RT", nameDialog.rt.IsChecked);
						tags = SetTag(tags, "Priestess", nameDialog.priestess.IsChecked);
						tags = SetTag(tags, "Praetor", nameDialog.prae.IsChecked);
						tags = SetTag(tags, "Horse", nameDialog.horse.IsChecked);
						tags = SetTag(tags, "Arb", nameDialog.arb.IsChecked);
						tags = SetTag(tags, "Sorc", nameDialog.sorc.IsChecked);
						tags = SetTag(tags, "Druid", nameDialog.druid.IsChecked);
						tags = SetTag(tags, "Scorp", nameDialog.scorp.IsChecked);
						tags = SetTag(tags, "Shipping", nameDialog.shipping.IsChecked);
						tags = SetTag(tags, "Stinger", nameDialog.stinger.IsChecked);
						tags = SetTag(tags, "Galley", nameDialog.galley.IsChecked);
						tags = SetTag(tags, "Warship", nameDialog.warship.IsChecked);

						city.remarks = tags;
						//		Post.Send("includes/sNte.php", $"a={HttpUtility.UrlEncode(tags, Encoding.UTF8)}&b=&cid={cid}");
						Post.Send("includes/sNte.php", $"a={HttpUtility.UrlEncode(tags, Encoding.UTF8)}&b={HttpUtility.UrlEncode(city.notes, Encoding.UTF8)}&cid={cid}");
					}
					Note.Show($"Set name to {lastName}");
					if (SettingsPage.setHub)
					{
						HubSettings.Show(cid);
					}
					if (SettingsPage.clearRes)
					{
						JSClient.ClearCenter(cid);
					}
				}
				
			}
			catch (Exception e)
			{
				Note.Show("Something went wrong");
				COTG.Debug.Log(e);
			}


		}
	}
    }
