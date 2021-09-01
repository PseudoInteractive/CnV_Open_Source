using COTG.Game;
using COTG.JSON;
using COTG.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static COTG.Debug;
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
using ContentDialog = Windows.UI.Xaml.Controls.ContentDialog;
using ContentDialogResult = Windows.UI.Xaml.Controls.ContentDialogResult;
namespace COTG.Views
{
    public sealed partial class CityRename : ContentDialog
    {
		public CityRename()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, Windows.UI.Xaml.Controls.ContentDialogButtonClickEventArgs args)
        {
        }

		

		static Regex regexCityName = new Regex(@"([^\d]*)(\d+)([^\d]+)([1-9]?)(0*)(\d+)(.*)", RegexOptions.CultureInvariant | RegexOptions.Compiled);
		static Regex regexHubName = new Regex(@"([^\d]*)(\d+)([^\d]+)(0{1,3})(\d{1,2})\b(.*)", RegexOptions.CultureInvariant | RegexOptions.Compiled);
		static Regex regexStorageName = new Regex(@"([^\d]*)(\d+)([^\d]+)(0{1,3})(\d{1,2})0(\d{1,2})\b(.*)", RegexOptions.CultureInvariant | RegexOptions.Compiled);


		static string lastName = string.Empty;
		public static async Task<bool> RenameDialog(int cid, bool allowSplat)
		{
			try
			{
				Assert(cid == City.build);
				var city = City.GetOrAddCity(cid);
				var nameDialog = new CityRename();
				
				bool isNew = IsNewOrCaptured(city)||city._cityName.IsNullOrEmpty();
				if(!isNew)
				{
					var match = regexCityName.Match(city._cityName);
					if (!match.Success)
						isNew = true;
				}
				var name = (isNew || city._cityName == null) ? null : city._cityName;
				
					
				
					//bool? isHub;
				//if (await city.TouchTags() == 0)
				//{
				//	isHub = await App.DoYesNoBox("A Hub?", "Is this a hub?", "Hub", "City", "Whatever") switch { 1=> true,0=>false,_=>null };
				//}
				//else
				//{
				//	isHub = city.isHub;
				//}

				// foreach (var tag in TagHelper.tags)
				// {
				//if (tag.isAlias)
				//	continue;
				//  var check = new ToggleButton() { IsChecked = city.HasTag(tag.id), Content = tag.s };
				//  nameDialog.tagsPanel.Children.Add(check);
				// }
				int closestHubCluster = 1;

				if (isNew)
				{
					// find closest hub for cluster #
					var closestScore = float.MaxValue;
					City closestHub = null;
					foreach (var v in City.myCities)
					{
						if (v.cont != city.cont || v._cityName == null)
							continue;
						var match = regexCityName.Match(v._cityName);
						bool hasLeadingZero = match.Groups[4].Value.IsNullOrEmpty() && !match.Groups[5].Value.IsNullOrEmpty();
						if (match.Success && (v.isHub || hasLeadingZero))
						{
							var score = city.cid.DistanceToCid(v.cid);
							if (score < closestScore)
							{
								var pre = match.Groups[1].Value;
								var mid = match.Groups[3].Value;
								//var cluster = match.Groups[4].Value;
								//	var leadingZeros = match.Groups[4].Value;
								var num = match.Groups[6].Value;
								var post = match.Groups[7].Value;
								//string numStr;

								num.TryParseInt(out var numV);

								//								if(num.StartsWith("0"))


								closestScore = score;
								name = pre + city.cont.ToString("00") + mid + ((hasLeadingZero ? numV * 1000 : numV) + 1).ToString() + post;
								closestHubCluster = numV;
							}
						}
					}
				}
			
				 
				if (name.IsNullOrEmpty())
					name = city.isHub ? $"{city.cont:00} 00{closestHubCluster}" : $"{city.cont:00} {closestHubCluster}001";

					{
						var match = regexCityName.Match(name);
						if (match.Success)
						{
							try
							{
								Assert(match.Groups.Count == 8);
								//var cont = match.Groups[2].Value;
								//cont.TryParseInt(out var contV);

								//// new cont?
								//if (contV != city.cont)
								//{
								//	var lastCity = City.myCities.LastOrDefault((v) => v.cont == city.cont);
								//	if (lastCity != null)
								//	{
								//		name = IsNewOrCaptured(lastCity) ? $"{city.cont:00} 0001" : lastCity.cityNameOrNull;
								//		match = regexCityName.Match(name);
								//		cont = match.Groups[2].Value;
								//		cont.TryParseInt(out contV);
								//	}
								//	else
								//	{
								//		name = $"{city.cont:00} 0001";

								//	}

								//}

								var pre = match.Groups[1].Value;

								var mid = match.Groups[3].Value;
								var cluster = match.Groups[4].Value;
								var leadingZeros = match.Groups[5].Value;
								var num = match.Groups[6].Value;
								var post = match.Groups[7].Value;
								num.TryParseInt(out var numV);
								var cont = $"{city.cont:00}";
								for (; ; )
								{

									name = pre + cont + mid + (cluster.IsNullOrEmpty()? string.Empty : closestHubCluster.ToString())+ leadingZeros + numV.ToString() + post;
									if (!City.myCities.Any((v) => v._cityName == name && v != city))
										break;
									++numV;
								}
							}
							catch (Exception ex)
							{
								LogEx(ex);
							}
						}
					}
					var result = await App.DispatchOnUIThreadTask(async () =>
				   {
					   nameDialog.name.Text = city._cityName;
					   nameDialog.suggested.Text = name;
					   //	ElementSoundPlayer.Play(ElementSoundKind.Show);

					   var result = await nameDialog.ShowAsync2();
					   bool wantSplat = false;
					   if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
					   {
						   if (!SettingsPage.useSuggested)
							   lastName = nameDialog.name.Text;
						   else
							   lastName = nameDialog.suggested.Text;
						   city._cityName = lastName;
						   if (IsNew(lastName))
							   lastName = string.Empty;
						   city.BuildStageDirty();
						   await Post.Send("includes/nnch.php", $"a={HttpUtility.UrlEncode(lastName, Encoding.UTF8)}&cid={cid}", World.CidToPlayerOrMe(cid));
						   //if (SettingsPage.applyTags)
						   //{
						   // await ApplyTags(cid, nameDialog.tagsPanel);
						   //}
						   Note.Show($"Set name to {lastName}");

					   }
					   return result;

				   });
					if (result == ContentDialogResult.Primary)
					{

						if (SettingsPage.setShareString)
						{
							await ShareString.Touch().ShowNoLock(City.build);
						}
						
						//if (SettingsPage.setHub)
						//{
						//	await HubSettings.Show(cid);
						//}
						var rv = true;
						if (SettingsPage.autoBuildCabins && allowSplat)
						{
							// are there any cabins here already?
							rv = await QueueTab.DoTheStuff(city, false, false);
						}

						await CitySettings.SetCitySettings(cid,
							autoBuildOn: SettingsPage.autoBuildOn,
							autoWalls: (SettingsPage.autoWallLevel == 10) ? true : null,
							autoTowers: (SettingsPage.autoTowerLevel == 10) ? true : null							);

					if (SettingsPage.clearRes && !city.leaveMe)
					{
						await city.ClearResUI();
					}
					return rv;
					}
					return result != ContentDialogResult.Secondary;
				
			}
			catch (Exception e)
			{
				Note.Show("Something went wrong, make new or lost cities?  Please retart app");
				COTG.Debug.LogEx(e);
			}
			return false;
		}

		//public static async Task ApplyTags(int cid,  Microsoft.Toolkit.Uwp.UI.Controls.WrapPanel tagControls)
		//{
		//	City city = City.GetOrAddCity(cid);
		//	await GetCity.Post(cid); // need to fetch notes
		//	string tags = city.remarks;
		//	foreach (var tag in TagHelper.tags)
		//	{
		//		var check = tagControls.Children.First( (ch) => (ch as ToggleButton)?.Content == tag.s)  as ToggleButton;
		//		tags = TagHelper.SetTag(tags, tag.s, check.IsChecked);
		//	}


		//	city.remarks = tags;
		//	//		Post.Send("includes/sNte.php", $"a={HttpUtility.UrlEncode(tags, Encoding.UTF8)}&b=&cid={cid}");
		//	await Post.Send("includes/sNte.php", $"a={HttpUtility.UrlEncode(tags, Encoding.UTF8)}&b={HttpUtility.UrlEncode(city.notes, Encoding.UTF8)}&cid={cid}", World.CidToPlayerOrMe(cid));
		//}

		public static bool IsNew(City city) => IsNew(city._cityName);
		public static bool IsNew(string _cityName)
		{
			return _cityName == "*New City" ;
		}
		public static bool IsNewOrCaptured(City city)
		{
			return IsNew(city._cityName) || city._cityName == "lawless city" || city._cityName == "*Lawless City";
		}
	}
}
