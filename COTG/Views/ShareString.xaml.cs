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
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Text.RegularExpressions;

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
		internal static ShareString Touch()
		{
			if (instance == null)
			{
					try
					{
						new ShareString();
					}
					catch (Exception ex)
					{
						LogEx(ex);
					}
				   Tables.ReadShares(Player.myName).ContinueWith( (shares) =>

				   { 
				   App.DispatchOnUIThreadSneaky(() =>
				  {
					  AddLayouts();
					  foreach (var s in shares.Result)
					  {
						  new ShareStringItem(s.s);
					  }
				  });
			   });

			}
			return instance;
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

		
		static public async Task Show(int cid)
		{
			await App.DispatchOnUIThreadExclusive(cid,async () =>



				 await ShowNoLock(cid)
			
			);
		}
		static public async Task ShowNoLock(int cid)
		{

			Assert(cid == City.build);

			var sel = Spot.GetSelectedForContextMenu(cid,false, onlyMine: true);
			

			bool setResources = true;
			bool setLayout = false;
			var bestHub = await CitySettings.FindBestHub(cid);
			var rv = await App.DispatchOnUIThreadTask(async () =>
			{
			try
			{
				Assert(App.uiSema.CurrentCount == 0);

				Touch();
	//			instance.onComplete.IsOn = CityBuild.isPlanner;
				instance.shareStrings.SelectedItem = null;
				
				
					var hubName = Spot.GetOrAdd(bestHub).nameAndRemarks;
					instance.bestHub.Text = hubName;
					

					//	instance.PlannerTeachingTip.Show();
					// todo: copy text 
					var city = City.GetOrAdd(cid);

					if (CityBuild.isPlanner)
						city.BuildingsCacheToShareString();

					var res = await CitySettings.GetTradeResourcesSettings(city.cid);

					SettingsPage.reqWood = res.req.wood > 0 ? res.req.wood : SettingsPage.reqWood;
					SettingsPage.reqStone = res.req.stone > 0 ? res.req.stone : SettingsPage.reqStone;
					SettingsPage.reqIron = res.req.iron > 0 ? res.req.iron : SettingsPage.reqIron;
					SettingsPage.reqFood = res.req.food > 0 ? res.req.food : SettingsPage.reqFood;

					SettingsPage.maxWood = res.max.wood > 0 ? res.max.wood : SettingsPage.maxWood;
					SettingsPage.maxStone = res.max.stone > 0 ? res.max.stone : SettingsPage.maxStone;
					SettingsPage.maxIron = res.max.iron > 0 ? res.max.iron : SettingsPage.maxIron;
					SettingsPage.maxFood = res.max.food > 0 ? res.max.food : SettingsPage.maxFood;


					SetFromSS();
					instance.Title = city.nameAndRemarks;
					SetCheckboxesFromTags(city.remarks);
					instance.Bindings.Update();

					var result = await instance.ShowAsync2();
					setResources = instance.expandResources.IsExpanded;
					setLayout = instance.expandShareString.IsExpanded;
					return result;
				}
				catch (Exception ex)
				{
					LogEx(ex);
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
					if (sel.Count > 1)
					{
						var x = await App.DoYesNoBox("Set Selected Cities?", $"{sel.Count} cities selected",yes:"All", no:"Just this" );
						if (x == -1)
							return;
						if (x == 0)
						{
							sel.Clear();
							sel.Add(cid);
						}
					}
					foreach (var ci in sel)
					{
						var city = City.GetOrAdd(ci);
						if ( SettingsPage.shareStringApplyTags)
						{
							await SetCityTags(ci);
							//await CitySettings.SetCitySettings(City.build, setRecruit: true);
						}

						//var req = new Resources((int)instance.reqWood.Value, (int)instance.reqStone.Value, (int)instance.reqIron.Value, (int)instance.reqFood.Value);
						//var max = new Resources((int)instance.maxWood.Value, (int)instance.maxStone.Value, (int)instance.maxIron.Value, (int)instance.maxFood.Value);
						//SettingsPage.reqWood = req.wood;
						//SettingsPage.reqStone = req.stone;
						//SettingsPage.reqIron = req.iron;
						//SettingsPage.reqFood = req.food;

						//SettingsPage.maxWood = max.wood;
						//SettingsPage.maxStone = max.stone;
						//SettingsPage.maxIron = max.iron;
						//SettingsPage.maxFood = max.food;

						//			await CitySettings.SetTradeResourcesSettings(city.cid,req,max);

						await CitySettings.SetCitySettings(ci, setResources&&SettingsPage.setHub ? bestHub : null,
							(setResources&&SettingsPage.setHub ? CitySettings.FilterTargetHub(ci, bestHub) : null),
							SettingsPage.shareStringApplyTags && SettingsPage.setRecruit, setResources: setResources
											);

						if (setLayout)
						{
							city.SetShareString(instance.GetShareStringWithJson());
							await city.SaveLayout();
						}
					}
					if (SettingsPage.autoRearrangeShareStrings && setLayout && City.Get(cid).isLayoutValid )
					{
						await PlannerTab.SmartRearrange(true);
					}
					City.Get(cid).BuildStageDirty();
				}
				else
				{

				}
			}
			catch (Exception ex)
			{

				LogEx(ex);
			}
		}
			
	
		int ? GetSetting(int v)
		{
			var rv = v;
			if (rv > 0)
				return rv;
			return null; ;
		}
		public ShareStringMeta GetMeta()
		{
			var meta = new ShareStringMeta() { notes = TagHelper.ApplyTags(TagsFromCheckboxes(),string.Empty), desc = description.Text, path = path.Text };
			if (SettingsPage.embedTradeInShareStrings)
			{
				meta.reqWood = GetSetting(SettingsPage.reqWood);
				meta.reqStone = GetSetting(SettingsPage.reqStone);
				meta.reqIron = GetSetting(SettingsPage.reqIron);
				meta.reqFood = GetSetting(SettingsPage.reqFood);
				meta.maxWood = GetSetting(SettingsPage.maxWood);
				meta.maxStone = GetSetting(SettingsPage.maxStone);
				meta.maxIron = GetSetting(SettingsPage.maxIron);
				meta.maxFood = GetSetting(SettingsPage.maxFood);
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
		public static void SetCheckboxesFromTags(string remarks)
		{
			var tags = TagHelper.Get(remarks);
			foreach (var tag in TagHelper.tags)
			{
				var check = instance.tagsPanel.Children.FirstOrDefault((a) => a is ToggleButton b && b.Content as string == tag.s) as ToggleButton;
				if (check == null)
				{
					check = new ToggleButton() { Content = tag.s };
//					check.Checked+= (_,_)=>;
					instance.tagsPanel.Children.Add(check);
				}
				check.IsChecked = tags.HasFlag(tag.v);
			}

		}
		static void SetValue(ref int v, int ? source)
		{
			if (source.HasValue)
				v = source.Value;
		}
			
		public static void SetFromSS(string shareString, bool setResAndTags)
		{
			var s = SplitShareString(shareString);
			var meta = JsonSerializer.Deserialize<ShareStringMeta>(s.json, Json.jsonSerializerOptions);
			var path = DecomposePath(meta.path);
			instance.shareString.Text = s.ss ?? string.Empty;
			var tags = meta.notes ?? string.Empty;

			instance.description.Text = meta.desc ?? string.Empty;
			instance.title.Text = path.title;
			instance.path.Text = CombinePath(path);

			if (setResAndTags)
			{
				SetCheckboxesFromTags(tags);
				SetValue(ref SettingsPage.reqWood, meta.reqWood);
				SetValue(ref SettingsPage.reqStone, meta.reqStone);
				SetValue(ref SettingsPage.reqIron, meta.reqIron);
				SetValue(ref SettingsPage.reqFood, meta.reqFood);

				SetValue(ref SettingsPage.maxWood, meta.maxWood);
				SetValue(ref SettingsPage.maxStone, meta.maxStone);
				SetValue(ref SettingsPage.maxIron, meta.maxIron);
				SetValue(ref SettingsPage.maxFood, meta.maxFood);
			}
			
		}
		public static void SetFromSS() => SetFromSS(City.GetBuild().shareString,false);
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
						SetFromSS(i.shareStringWithJson,true);
					}
					else
					{
						instance.shareString.Text = i.shareString.Replace("\n", "", StringComparison.Ordinal).Replace("\r", "", StringComparison.Ordinal);
						instance.description.Text = i.desc;
						instance.path.Text = i.path;
						instance.title.Text = i.label;
						SetCheckboxesFromTags(i.tags);
					}
				}
			}
		}

		//private void TogglePlannerClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
		//{
		//	//onComplete.IsOn = true;
		//}

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

		public TradeSettings _TradeSettingsSel;
		public TradeSettings tradeSettingsSel
		{
			get => _TradeSettingsSel;
			set
			{
				if (_TradeSettingsSel != value)
				{
					_TradeSettingsSel = value;
					SettingsPage.reqWood = value.requestWood;
					SettingsPage.reqStone = value.requestStone;
					SettingsPage.reqIron = value.requestIron;
					SettingsPage.reqFood = value.requestFood; ;
				//	SettingsPage.sendWood = value.destWood != 0;
				//	SettingsPage.sendStone = value.destStone != 0;
			//		SettingsPage.sendIron = value.destIron != 0;
			//		SettingsPage.sendFood = value.destFood != 0;
					SettingsPage.maxWood = value.sendWood;
					SettingsPage.maxStone = value.sendStone;
					SettingsPage.maxIron = value.sendIron;
					SettingsPage.maxFood = value.sendFood;
					instance.Bindings.Update();
				}
			}
		}

		public static Tags TagsFromCheckboxes()
		{
			Tags tags = default;
			foreach (var tag in TagHelper.tags)
			{

				var check = instance.tagsPanel.Children.First((ch) => (ch as ToggleButton)?.Content == tag.s) as ToggleButton;
				if (check.IsChecked.GetValueOrDefault())
					tags |= tag.v;

			}
			return tags;
		}

			public static async Task SetCityTags(int cid)
		{
			City city = City.GetOrAddCity(cid);
			await GetCity.Post(cid); // need to fetch notes
			var tags = TagsFromCheckboxes();
			city.tags = tags;
			city.remarks = TagHelper.ApplyTags(tags, city.remarks);
			//		Post.Send("includes/sNte.php", $"a={HttpUtility.UrlEncode(tags, Encoding.UTF8)}&b=&cid={cid}");
			await Post.Send("includes/sNte.php", $"a={HttpUtility.UrlEncode(city.remarks, Encoding.UTF8)}&b={HttpUtility.UrlEncode(city.notes, Encoding.UTF8)}&cid={cid}", World.CidToPlayerOrMe(cid));
		}

		private void CollapsedDisable(object sender, EventArgs e)
		{
			var exp = sender as Microsoft.Toolkit.Uwp.UI.Controls.Expander;
			Assert(exp!=null);
			exp.Header = exp.Header as string + " No Change";
		}
		private void ExpandedEnable(object sender, EventArgs e)
		{
			var exp = sender as Microsoft.Toolkit.Uwp.UI.Controls.Expander;
			Assert(exp!=null);
			exp.Header = (exp.Header as string).Replace( " No Change", "");
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

		public static Regex squiggleBracketMatcher = new(@"[^()]*(?>(?>(?'open'\{)[^()]*)+(?>(?'-open'\})[^()]*)+)+(?(open)(?!))", RegexOptions.Compiled);



		public ShareStringItem(string shareString)
		{
			try
			{
				var s = ShareString.SplitShareString(shareString);
				var json = s.json.Replace("&#34;", "\"");
				var match = squiggleBracketMatcher.Match(json);
				if( match.Captures.Count > 1 )
				{
					int q = 0;
				}
				var meta = JsonSerializer.Deserialize<ShareStringMeta>(json, Json.jsonSerializerOptions);
				//	var path = ShareString.DecomposePath(meta.path);
				Ctor(meta.path, meta.notes ?? string.Empty, meta.desc ?? string.Empty, s.ss ?? string.Empty, shareString);
			}
			catch (Exception ex)
			{
				Log(ex);
			}
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
