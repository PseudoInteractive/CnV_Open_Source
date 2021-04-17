using COTG.Game;
using COTG.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using static COTG.Game.City;
using static COTG.Debug;
using COTG.Helpers;
using COTG.JSON;

namespace COTG.Views
{

	public sealed partial class ShareString : Windows.UI.Xaml.Controls.ContentDialog
	{
		public static bool loadedLayouts;
		static public ShareString instance;
		public ShareString()
		{
			instance = this;
			this.InitializeComponent();

		}
		//	public static ShareStringItem tempItem = new ShareStringItem("temp~na") { notes = "nothing selected" };
		//[ShareString.1.3]:
		//[/ShareString]
		public static (string root, string mid, string title) DecomposePath(string src)
		{
			if (src.IsNullOrEmpty())
				return (string.Empty, string.Empty, string.Empty);

			var vr = src.Split('~', StringSplitOptions.RemoveEmptyEntries);
			if (vr.Length >= 3)
			{
				var mid = vr.Skip(1).Take(vr.Length - 2);
				var midStr = String.Join('~', mid);
				return (vr[0], midStr, vr[vr.Length - 1]);
			}
			else if (vr.Length >= 2)
			{
				return (vr[0], string.Empty, vr[1]);
			}
			else if (vr.Length >= 1)
			{
				return (string.Empty, String.Empty, vr[0]);
			}
			else
			{
				return (string.Empty, string.Empty, string.Empty);
			}
		}

		public static string CombinePath((string root, string mid, string title) path)
		{
			return !path.mid.IsNullOrEmpty() ? path.root + '~' + path.mid + "~" + path.title : path.root + "~" + path.title;
		}
		public static string StripRoot(string path)
		{
			var strip = DecomposePath(path);
			return StripRoot(strip).subPath;
		}
		public static (string root, string subPath) StripRoot((string root, string mid, string title) path)
		{
			return (path.root, path.mid.IsNullOrEmpty() ? path.title : path.mid + '~' + path.title);
		}

		public static bool IsValid(string ss)
		{
			if (ss.IsNullOrEmpty())
				return false;
			if (!ss.StartsWith(City.shareStringStart))
				return false;
			if (ss.Length < City.minShareStringLength)
				return false;
			return true;
		}

		static public string GetTags(string initial)
		{
			foreach (var tag in TagHelper.tags)
			{
				var check = instance.tagsPanel.Children.First((ch) => ((ch is ToggleButton t) && (t.Content as string == tag.s))) as ToggleButton;
				initial = TagHelper.SetTag(initial, tag.s, check.IsChecked);

			}
			return initial;
		}
		static public async Task Show()
		{
			await App.DispatchOnUIThreadExclusive(City.build,async () =>



				 await ShowNoLock()
			
			);
		}
		static public async Task ShowNoLock()
		{

			var rv = await App.DispatchOnUIThreadTask(async () =>
			{
			try
			{
				Assert(App.uiSema.CurrentCount == 0);

				if (instance == null)
				{
					try
					{
						new ShareString();
					}
					catch (Exception ex)
					{
						Log(ex);
					}
					AddLayouts();
					var shares = await Tables.ReadShares(Player.myName);
					foreach (var s in shares)
					{
						new ShareStringItem(s.s);
					}
				}
				instance.onComplete.IsOn = CityBuild.isPlanner;
				instance.shareStrings.SelectedItem = null;
				var city = City.GetBuild();
				//	instance.PlannerTeachingTip.Show();
				// todo: copy text 
				if (CityBuild.isPlanner)
					city.BuildingsCacheToShareString();

					var res = await CitySettings.GetTradeResourcesSettings(city.cid);

					instance.reqWood.Value = res.req.wood > 0 ? res.req.wood : SettingsPage.reqWood;
					instance.reqStone.Value = res.req.stone > 0 ? res.req.stone : SettingsPage.reqStone;
					instance.reqIron.Value = res.req.iron > 0 ? res.req.iron : SettingsPage.reqIron;
					instance.reqFood.Value = res.req.food > 0 ? res.req.food : SettingsPage.reqFood;

					instance.maxWood.Value = res.max.wood > 0 ? res.max.wood : SettingsPage.maxWood;
					instance.maxStone.Value = res.max.stone > 0 ? res.max.stone : SettingsPage.maxStone;
					instance.maxIron.Value = res.max.iron > 0 ? res.max.iron : SettingsPage.maxIron;
					instance.maxFood.Value = res.max.food > 0 ? res.max.food : SettingsPage.maxFood;


					SetFromSS();
					instance.Title = city.nameAndRemarks;
					//SetTags(city.remarks);

					return await instance.ShowAsync2();
				}
				catch (Exception ex)
				{
					Log(ex);
				}
				return ContentDialogResult.None;

			});
			// todo:  copy back sharestring

			try
			{
				if (rv == ContentDialogResult.Primary)
				{

					//if ( instance.applyTags.IsOn)
					//{
					//	city.remarks = GetTags(city.remarks);
					//	//		Post.Send("includes/sNte.php", $"a={HttpUtility.UrlEncode(tags, Encoding.UTF8)}&b=&cid={cid}");
					//	await Post.Send("includes/sNte.php", $"a={HttpUtility.UrlEncode(city.remarks, Encoding.UTF8)}&b={HttpUtility.UrlEncode(city.notes, Encoding.UTF8)}&cid={city.cid}", World.CidToPlayerOrMe(city.cid));
					//}

					if (SettingsPage.shareStringApplyTags)
					{
						await CityRename.ApplyTags(City.build, instance.tagsPanel);
						await CitySettings.SetCitySettings(City.build, setRecruit: true);
					}
					var city = City.GetBuild();
					var req = new Resources((int)instance.reqWood.Value, (int)instance.reqStone.Value, (int)instance.reqIron.Value, (int)instance.reqFood.Value);
					var max = new Resources((int)instance.maxWood.Value, (int)instance.maxStone.Value, (int)instance.maxIron.Value, (int)instance.maxFood.Value);
					SettingsPage.reqWood = req.wood;
					SettingsPage.reqStone = req.stone;
					SettingsPage.reqIron = req.iron;
					SettingsPage.reqFood = req.food;

					SettingsPage.maxWood = max.wood;
					SettingsPage.maxStone = max.stone;
					SettingsPage.maxIron = max.iron;
					SettingsPage.maxFood = max.food;

					await CitySettings.SetTradeResourcesSettings(city.cid,req,max);
		
					
					city.SetShareString(instance.GetShareStringWithJson());
					await city.SaveLayout();
					if (SettingsPage.autoRearrangeShareStrings)
					{
						CityBuild._isPlanner = true;
						PlannerTab.SmartRearrange();
					}
					CityBuild._isPlanner = instance.onComplete.IsOn;
					city.BuildStageDirty();
				}
				else
				{

				}
			}
			catch (Exception ex)
			{

				Log(ex);
			}
		}
			
	
		int ? GetSetting(NumberBox v)
		{
			var rv = (int)v.Value;
			if (rv > 0)
				return rv;
			return null; ;
		}
		public ShareStringMeta GetMeta()
		{
			var meta = new ShareStringMeta() { notes = GetTags(string.Empty), desc = description.Text, path = path.Text };
			if (SettingsPage.embedTradeInShareStrings)
			{
				meta.reqWood = GetSetting(reqWood);
				meta.reqStone = GetSetting(reqStone);
				meta.reqIron = GetSetting(reqIron);
				meta.reqFood = GetSetting(reqFood);
				meta.maxWood = GetSetting(maxWood);
				meta.maxStone = GetSetting(maxStone);
				meta.maxIron = GetSetting(maxIron);
				meta.maxFood = GetSetting(maxFood);
			}
			return meta;
		}
		string GetShareStringWithJson()
		{


			return GetShareString() + JsonSerializer.Serialize(GetMeta(), Json.jsonSerializerOptions);

		}
	
		
		static string GetShareString()
		{
			return instance.shareString.Text.Replace("\n", "", StringComparison.Ordinal).Replace("\r", "", StringComparison.Ordinal);

		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

		private void UseBuildingsClick(object sender, RoutedEventArgs e)
		{
			var s = City.GetBuild();
			instance.shareString.Text=City.BuildingsToShareString(s.buildings,s.isOnWater);
		}
		public static void SetTags(string tags)
		{
			foreach (var tag in TagHelper.tags)
			{
				var check = instance.tagsPanel.Children.FirstOrDefault((a) => a is ToggleButton b && b.Content as string == tag.s) as ToggleButton;
				if (check == null)
				{
					check = new ToggleButton() { Content = tag.s };
//					check.Checked+= (_,_)=>;
					instance.tagsPanel.Children.Add(check);
				}
				check.IsChecked = tag.Has(tags);
			}

		}
		static void SetValue(NumberBox v, int ? source)
		{
			if (source.HasValue)
				v.Value = source.Value;
		}
			
		public static void SetFromSS(string shareString)
		{
			var s = SplitShareString(shareString);
			var meta = JsonSerializer.Deserialize<ShareStringMeta>(s.json, Json.jsonSerializerOptions);
			var path = DecomposePath(meta.path);
			instance.shareString.Text = s.ss ?? string.Empty;
			var tags = meta.notes ?? string.Empty;

			instance.description.Text = meta.desc ?? string.Empty;
			instance.title.Text = path.title;
			instance.path.Text = CombinePath(path);

			SetTags(tags);
			SetValue(instance.reqWood, meta.reqWood);
			SetValue(instance.reqStone, meta.reqStone);
			SetValue(instance.reqIron, meta.reqIron);
			SetValue(instance.reqFood, meta.reqFood);

			SetValue(instance.maxWood, meta.maxWood);
			SetValue(instance.maxStone, meta.maxStone);
			SetValue(instance.maxIron, meta.maxIron);
			SetValue(instance.maxFood, meta.maxFood);

		}
		public static void SetFromSS() => SetFromSS(City.GetBuild().shareString);
		public static (string ss, string json) SplitShareString(string shareString)
		{
				if (shareString == null)
					return (string.Empty, AUtil.emptyJson);
				var i = shareString.IndexOf('{');
				if (i == -1)
					return (shareString, AUtil.emptyJson); ;
				return (shareString.Substring(0, i), shareString.Substring(i));
			
		}

		private async void FromClipboardClick(object sender, RoutedEventArgs e)
		{
			var text = await App.GetClipboardText();
			if (text == null)
			{
				Note.Show("Clipboard is empty");

			}
			else
			{
				Note.Show($"New Sharestring: {text}");
				instance.shareString.Text = text;
			}
		}

		private void ToClipboardClick(object sender, RoutedEventArgs e)
		{
			App.CopyTextToClipboard(GetShareString());
		}

		private async void ShareItemInvoked(Microsoft.UI.Xaml.Controls.TreeView sender, Microsoft.UI.Xaml.Controls.TreeViewItemInvokedEventArgs args)
		{
			var i = args.InvokedItem as ShareStringItem;
			Assert(i != null);
			if(i!=null)
			{
				if(i.shareString!= null)
				{
					if (i.shareStringWithJson != null)
					{
						SetFromSS(i.shareStringWithJson);
					}
					else
					{
						instance.shareString.Text = i.shareString.Replace("\n", "", StringComparison.Ordinal).Replace("\r", "", StringComparison.Ordinal);
						instance.description.Text = i.desc;
						instance.path.Text = i.path;
						instance.title.Text = i.label;
						SetTags(i.tags);
					}
				}
			}
		}

		private void TogglePlannerClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
		{
			onComplete.IsOn = true;
		}

		private void ClearClick(object sender, RoutedEventArgs e)
		{
			shareString.Text = string.Empty;
		}

		private void ShareClick(object sender, RoutedEventArgs e)
		{
			var title = this.title.Text;
			if(title.IsNullOrEmpty())
			{
				Note.Show("Please set title");
				return;
			}
			if(!IsValid(GetShareString()))
			{
				Note.Show("Share string is not valide");
				return;

			}
			var p = DecomposePath(path.Text);
			p.root = Player.myName;
			path.Text = CombinePath(p); // set my name as root
			var json = GetShareStringWithJson();
			Tables.ShareShareString(Player.myName, title, json);
			
			new ShareStringItem(json);

			Note.Show("Shared!");
		}

		private void title_TextChanged(object sender, TextChangedEventArgs e)
		{
			var ss = DecomposePath(instance.path.Text);
			ss.title = title.Text;
			ss.root = Player.myName;

			instance.path.Text = CombinePath(ss);

		}
		
	}
	



	public struct ShareStringMeta
	{
		public string path { get; set; }
		public string desc { get; set; }
		public string notes { get; set; }

		public int? reqWood { get; set; }
		public int? reqStone { get; set; }
		public int? reqIron { get; set; }
		public int? reqFood { get; set; }
		public int? maxWood { get; set; }
		public int? maxStone { get; set; }
		public int? maxIron { get; set; }
		public int? maxFood { get; set; }

	}
	public class ShareStringItem
	{
		public string path { get; set; } = string.Empty;
		public static DumbCollection<ShareStringItem> all = new();
		public string shareString { get; set; }
		[JsonIgnore]
		public string label
		{
			get
			{

				var d = ShareString.DecomposePath(path);
				return d.title;

			}
			set
			{
				var d = ShareString.DecomposePath(path);
				d.root = Player.myName;
				d.title = value;
				path = ShareString.CombinePath(d);

			}
		}
		public string desc { get; set; }
		public string tags { get; set; }

		public string shareStringWithJson;
		[JsonIgnore]
		public DumbCollection<ShareStringItem> children { get; set; } = new();
		// group items
		public ShareStringItem(string _path, bool _isPath)
		{
			path = _path;
		}
		//public ShareStringItem() 
		//{
		//	path = $"{Player.myName}~tba";
		//	title = "tba";
		//}
		public override string ToString() => JsonSerializer.Serialize(this, Json.jsonSerializerOptions);

		public ShareStringItem(string shareString)
		{
			var s = ShareString.SplitShareString(shareString);
			var meta = JsonSerializer.Deserialize<ShareStringMeta>(s.json, Json.jsonSerializerOptions);
		//	var path = ShareString.DecomposePath(meta.path);
			Ctor(meta.path, meta.notes ?? string.Empty, meta.desc ?? string.Empty, s.ss ?? string.Empty,shareString);
		}

		public ShareStringItem(string path, string _tags, string _desc, string _share)
		{
			Ctor(path, _tags, _desc, _share,null);
		}

		public void Ctor(string path, string _tags, string _desc, string _share, string _shareStringWithJson)
		{
			this.path = path;
			shareStringWithJson = _shareStringWithJson;
			tags = _tags;
			desc = _desc;
			shareString = _share;
			var dir = path.Split('~', StringSplitOptions.RemoveEmptyEntries);
			var myList = all;
			var pathSoFar = String.Empty;
			for (int i = 0; i < dir.Length - 1; ++i)
			{
				pathSoFar = pathSoFar + '~' + dir[i];
				var parent = myList.Find((a) => a.label == dir[i]);
				if (parent == null)
				{
					parent = new ShareStringItem(pathSoFar, true);
					myList.Add(parent);
				}
				myList = parent.children;
			}
			var existing = myList.Find(a => a.path == path);
			// replace
			if (existing != null)
				myList.Remove(existing);

			myList.Add(this);
			all.NotifyReset();
		}
	}
}
