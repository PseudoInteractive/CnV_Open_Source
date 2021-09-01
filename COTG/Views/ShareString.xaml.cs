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
using System.ComponentModel;
using static COTG.Game.City;
namespace COTG.Views
{

//	record ab(string a=null, string b=null);

	public sealed partial class ShareString : Windows.UI.Xaml.Controls.ContentDialog, INotifyPropertyChanged
	{
		#region PropertyChanged
		public void OnPropertyChanged(string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		public static bool loadedLayouts;
		static public ShareString instance;
		
		public ShareString()
		{
			instance = this;
			this.InitializeComponent();
			res.req = SettingsPage.defaultReq;
			res.max = SettingsPage.defaultSend;
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
					   App.DispatchOnUIThreadLow(() =>
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

		
		static public Task Show(int cid)
		{
			return App.DispatchOnUIThreadExclusive(cid,async () =>

				  await Touch().ShowNoLock(cid)

			);
		}
		public async Task ShowNoLock(int cid)
		{

			Assert(cid == City.build);

		//	var sel = Spot.GetSelectedForContextMenu(cid,false, onlyMine: true);
			

			bool setResources = false;
			bool setLayout = false;
			bool setTags = false;
			var bestHub = await CitySettings.FindBestHub(cid);
			var rv = await App.DispatchOnUIThreadTask(async () =>
			{
			try
				{
					Assert(App.uiSema.CurrentCount == 0);

					Touch();
					//			onComplete.IsOn = CityBuild.isPlanner;
					shareStrings.SelectedItem = null;




					//	PlannerTeachingTip.Show();
					// todo: copy text 
					var city = City.GetOrAdd(cid);

					if (CityBuild.isPlanner)
						city.BuildingsCacheToShareString();

					await res.InitTradeSettings(city,bestHub,city.isHubOrStorage ? 0 : bestHub);

			//		res.applyRequested = true;
			//		res.applySend = true;

					SetFromSS(city.shareString,false);
					res.sendFilter = ResourceFilter._true;
					res.reqFilter = ResourceFilter._true;
					Title = city.nameAndRemarks;
					SetCheckboxesFromTags(city.remarks);
					Bindings.Update();

					var result = await this.ShowAsync2();
					setResources = expandResources.IsExpanded;
					setLayout = expandShareString.IsExpanded;
					setTags = expandTags.IsExpanded;
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

					//if ( applyTags.IsOn)
					//{
					//	city.remarks = GetTags(city.remarks);
					//	//		Post.Send("includes/sNte.php", $"a={HttpUtility.UrlEncode(tags, Encoding.UTF8)}&b=&cid={cid}");
					//	await Post.Send("includes/sNte.php", $"a={HttpUtility.UrlEncode(city.remarks, Encoding.UTF8)}&b={HttpUtility.UrlEncode(city.notes, Encoding.UTF8)}&cid={city.cid}", World.CidToPlayerOrMe(city.cid));
					//}
					 SettingsPage.defaultReq = res.req;
					 SettingsPage.defaultSend = res.max;

						//if(sel.Count > 1)
						//{
						//	var x = await App.DoYesNoBox("Set Selected Cities?", $"{sel.Count} cities selected",yes:"All", no:"Just this" );
						//	if (x == -1)
						//		return;
						//	if (x == 0)
						//	{
						//		sel.Clear();
						//		sel.Add(cid);
						//	}
						//}
				//	foreach (var ci in sel)
					{
						var city = City.GetOrAdd(cid);
						if ( setTags)
						{
							await SetCityTags(cid);
							//await CitySettings.SetCitySettings(City.build, setRecruit: true);
						}

						//var req = new Resources((int)reqWood.Value, (int)reqStone.Value, (int)reqIron.Value, (int)reqFood.Value);
						//var max = new Resources((int)maxWood.Value, (int)maxStone.Value, (int)maxIron.Value, (int)maxFood.Value);
						//SettingsPage.reqWood = req.wood;
						//SettingsPage.reqStone = req.stone;
						//SettingsPage.reqIron = req.iron;
						//SettingsPage.reqFood = req.food;

						//SettingsPage.maxWood = max.wood;
						//SettingsPage.maxStone = max.stone;
						//SettingsPage.maxIron = max.iron;
						//SettingsPage.maxFood = max.food;
						var reqFilter = res.reqFilter;
						var sendFilter = (!city.isHubOrStorage) ? res.sendFilter : ResourceFilter._null;
						//			await CitySettings.SetTradeResourcesSettings(city.cid,req,max);

						await CitySettings.SetCitySettings(cid,reqHub: bestHub,targetHub:bestHub,
							 setRecruit:setTags && SettingsPage.setRecruit,
							 cartReserve: res.cartReserve,
						shipReserve: res.shipReserve,
							 req:res.req,max:res.max,
							reqFilter:reqFilter,sendFilter:sendFilter);

						if (setLayout)
						{
							city.SetShareString(GetShareStringWithJson(),true);
						}
					}
					if (SettingsPage.autoRearrangeShareStrings && setLayout && City.Get(cid).isLayoutValid )
					{
						await PlannerTab.SmartRearrange(City.GetBuild(),true);
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

		public ShareStringMeta GetMeta()
		{
			var meta = new ShareStringMeta() { notes = TagHelper.ApplyTags(TagsFromCheckboxes(),string.Empty), desc = description.Text, path = path.Text };
			if (SettingsPage.embedTradeInShareStrings)
			{
				meta.reqWood = res.req.wood;
				meta.reqStone = res.req.stone;
				meta.reqIron = res.req.iron;
				meta.reqFood = res.req.food;
				meta.maxWood = res.max.wood;
				meta.maxStone = res.max.stone;
				meta.maxIron = res.max.iron;
				meta.maxFood = res.max.food;
			}
			return meta;
		}
		string GetShareStringWithJson()
		{


			return GetShareString() + JsonSerializer.Serialize(GetMeta(), Json.jsonSerializerOptions);

		}
	
		
		string GetShareString()
		{
			return shareString.Text.Replace("\n", "", StringComparison.Ordinal).Replace("\r", "", StringComparison.Ordinal);

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
			shareString.Text=City.BuildingsToShareString(s.buildings,s.isOnWater);
		}
		public void SetCheckboxesFromTags(string remarks)
		{
			var tags = TagHelper.Get(remarks);
			foreach (var tag in TagHelper.tagsWithoutAliases)
			{
				var check = tagsPanel.Children.FirstOrDefault((a) => a is ToggleButton b && b.Content as string == tag.s) as ToggleButton;
				if (check == null)
				{
					check = new ToggleButton() { Content = tag.s };
//					check.Checked+= (_,_)=>;
					tagsPanel.Children.Add(check);
				}
				check.IsChecked = tags.HasFlag(tag.v);
			}

		}
		static void SetValue(ref int? v, int ? source)
		{
			if (source.HasValue)
				v = source.Value;
		}
			
		public void SetFromSS(string shareString, bool setResAndTags)
		{
			var s = SplitShareString(shareString);
			ShareStringMeta meta = new();
			try
			{
				meta = JsonSerializer.Deserialize<ShareStringMeta>(s.json,Json.jsonSerializerOptions);
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
			var path = DecomposePath(meta.path);
			this.shareString.Text = s.ss ?? string.Empty;
			var tags = meta.notes ?? string.Empty;

			description.Text = meta.desc ?? string.Empty;
			title.Text = path.title;
			this.path.Text = CombinePath(path);

			if (setResAndTags)
			{
				SetCheckboxesFromTags(tags);
				SetValue(ref res.req.wood, meta.reqWood);
				SetValue(ref res.req.stone, meta.reqStone);
				SetValue(ref res.req.iron, meta.reqIron);
				SetValue(ref res.req.food, meta.reqFood);

				SetValue(ref res.max.wood, meta.maxWood);
				SetValue(ref res.max.stone, meta.maxStone);
				SetValue(ref res.max.iron, meta.maxIron);
				SetValue(ref res.max.food, meta.maxFood);
				res.OnPropertyChanged();
			}
			
		}
		public static (string ss, string json) SplitShareString(string shareString)
		{
				if (shareString == null)
					return (string.Empty, AUtil.emptyJson);
			string json;
			string ss;

			var i = shareString.IndexOf('{');
			if (i == -1)
				json=	AUtil.emptyJson;
			else
				json = shareString.Substring(i);
			if (shareString.Length >= shareStringStartOffset + citySpotCount)
				ss = shareString.Substring(shareStringStartOffset,  citySpotCount);
			else
				ss = string.Empty;
			return (ss, json);
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
				shareString.Text = text;
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
						shareString.Text = i.shareString.Replace("\n", "", StringComparison.Ordinal).Replace("\r", "", StringComparison.Ordinal);
						description.Text = i.desc;
						path.Text = i.path;
						title.Text = i.label;
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
			var ss = DecomposePath(path.Text);
			ss.title = title.Text;
			ss.root = Player.myName;

			path.Text = CombinePath(ss);

		}

		
		public Tags TagsFromCheckboxes()
		{
			Tags tags = default;
			foreach (var tag in TagHelper.tagsWithoutAliases)
			{

				var check = tagsPanel.Children.First((ch) => (ch as ToggleButton)?.Content == tag.s) as ToggleButton;
				if (check.IsChecked.GetValueOrDefault())
					tags |= tag.v;

			}
			return tags;
		}

		public  async Task SetCityTags(int cid)
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
			exp.Header = exp.Header as string + " - No Change";
		}
		private void ExpandedEnable(object sender, EventArgs e)
		{
			var exp = sender as Microsoft.Toolkit.Uwp.UI.Controls.Expander;
			Assert(exp!=null);
			exp.Header = (exp.Header as string).Replace( " - No Change", "");
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
		public int? cabinCount { get; set; }
		public int? cabinLevel { get; set; }
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

//		public static Regex squiggleBracketMatcher = new(@"[^{}]*(?>(?>(?'open'\{)[^()]*)+(?>(?'-open'\})[^{}]*)+)+(?(open)(?!))", RegexOptions.CultureInvariant | RegexOptions.Compiled);

		public ShareStringItem(string shareString)
		{
			try
			{
				var s = ShareString.SplitShareString(shareString);
				//if(s.json.Contains("}{"))
				//{
				//	int q = 0;

				//}
				var json = s.json.Replace("&#34;", "\"");
				// massive hack!
				var id = json.LastIndexOf('{');
				if(id > 0)
				{
					json = json.Substring(id);
				}
				//var match = squiggleBracketMatcher.Match(json);
				//if( match.Captures.Count > 1 )
				//{
				//	int q = 0;
				//}
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
