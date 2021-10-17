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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using static COTG.Debug;
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
using ContentDialog = Microsoft.UI.Xaml.Controls.ContentDialog;
using ContentDialogResult = Microsoft.UI.Xaml.Controls.ContentDialogResult;
namespace COTG.Views
{
    public sealed partial class CityRename : ContentDialog
    {
		public CityRename()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, Microsoft.UI.Xaml.Controls.ContentDialogButtonClickEventArgs args)
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

				var result = await App.DispatchOnUIThreadTask(async () =>
				{
					
				bool isNew = IsNewOrCaptured(city)||city._cityName.IsNullOrEmpty() ;
				nameDialog.useSuggested.IsOn = isNew;
				nameDialog.current.Text = city._cityName;
					// is this needed?
					void ChooseName()
					{
						string name0 = $"{city.cont:00} 1";
						string name1 = ""; // default
						var format = (int a) => a.ToString("D3");
						var type = nameDialog.cityType.SelectedIndex;
						switch(type)
						{
							case 0:
							case 2:
								{
									var closestScore = float.MaxValue;
									City closestHub = null;

									// normal or storage
									foreach(var v in City.myCities)
									{
										if(v.cont != city.cont || v._cityName == null)
											continue;
										var match = regexCityName.Match(v._cityName);
										bool hasLeadingZero = match.Groups[4].Value.IsNullOrEmpty() && !match.Groups[5].Value.IsNullOrEmpty();
										if(match.Success)
										{
											if(v.isHub)
											{
												var score = city.cid.DistanceToCid(v.cid);
												if(score < closestScore)
												{
													var pre = match.Groups[1].Value;
													var mid = match.Groups[3].Value;
													//var cluster = match.Groups[4].Value;
													var leadingZeros = match.Groups[5].Value;
													var num = match.Groups[6].Value;
													var post = match.Groups[7].Value;
													//string numStr;

													num.TryParseInt(out var numV);

													//								if(num.StartsWith("0"))

													format = type == 0 ? (int a) => a.ToString("D3") : (a) => "0"+AUtil.BeyondHex(a);
													closestScore = score;
													name0 = pre + city.cont.ToString("00") + mid + (type==2 ? "0" : "")+ numV;
													name1 = post;
												}
											}
										}
										else
										{
										//	Assert(false);
										}
									}
									break; // normal
								}
							default:
								// hub
								name0 = $"{city.cont:00} 001";
								name1 = ""; // default
								format = (a) => AUtil.BeyondHex(a).ToString();
								break;
						}
						for(int uid = 1;;++uid)
						{
							var name = name0 + format(uid) + name1;
							if(!City.myCities.Any((v) => v._cityName == name && v != city))
							{
								nameDialog.suggested.Text = name;
								break;
							}
						} // u

					}
					// does this trigger it?
					nameDialog.cityType.SelectedIndex = city.isHub ? 1 : city.isStorage ? 2 : 0;
					nameDialog.cityType.SelectionChanged+= (_,_)=> ChooseName();


					   var result = await nameDialog.ShowAsync2();
					   if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
					   {
						   if (!nameDialog.useSuggested.IsOn)
							city._cityName = nameDialog.current.Text;
						   else
							city._cityName = nameDialog.suggested.Text;

							city.OnPropertyChanged();
						   city.BuildStageDirty();
						   await Post.Get("includes/nnch.php", $"a={HttpUtility.UrlEncode(lastName, Encoding.UTF8)}&cid={cid}", World.CidToPlayerOrMe(cid));
						   Note.Show($"Set name to {city._cityName}");

					   }
					   return result;

				   });
					if (result == ContentDialogResult.Primary)
					{

						if (SettingsPage.setShareString)
						{
							await ShareString.ShowNoLock(cid,SetupFlags.all);
						}

					//if (SettingsPage.setHub)
					//{
					//	await HubSettings.Show(cid);
					//}
					await CitySettings.SetCitySettings(cid,
						autoBuildOn: SettingsPage.autoBuildOn,
						autoWalls: (SettingsPage.autoWallLevel == 10) ? true : null,
						autoTowers: (SettingsPage.autoTowerLevel == 10) ? true : null);

						var rv = true;
						if (SettingsPage.autoBuildCabins && allowSplat)
						{
							// are there any cabins here already?
							rv = await COTG.DoTheStuff.Go(city, false, false);
						}
						else
						{
							if(SettingsPage.clearRes && !city.leaveMe)
							{
								await city.ClearResUI();
							}

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

		private static void CityType_SelectionChanged(object sender,SelectionChangedEventArgs e)
		{
			throw new NotImplementedException();
		}

		//public static async Task ApplyTags(int cid,  CommunityToolkit.WinUI.WrapPanel tagControls)
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

		private void cityType_SelectionChanged(object sender,SelectionChangedEventArgs e)
		{

		}
	}
}
