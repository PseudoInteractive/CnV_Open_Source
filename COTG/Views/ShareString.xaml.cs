using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using static CnV.Game.City;
using System.Text.RegularExpressions;
using System.ComponentModel;
using CommunityToolkit.WinUI.UI.Controls;

namespace CnV.Views
{
	using Game;
	using Helpers;
	using Services;

	//	record ab(string a=null, string b=null);
	[Flags]
	public enum SetupFlags:byte
	{

		name = 1<<0,
		layout = 1<<1,
		trade = 1<<2,
		tags = 1<<3,
		all = 255,


	}

	public sealed partial class ShareString:TeachingTip, IANotifyPropertyChanged
	{

		#region PropertyChanged
		public void CallPropertyChanged(string member = null)
		{
			PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(member));
		}
		public void OnPropertyChanged(string member = null)
		{
			if(PropertyChanged is not null) ((IANotifyPropertyChanged)this).IOnPropertyChanged();
		}
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		public static bool loadedLayouts;
		static public ShareString instance;
		private bool tradeStale;
		private int cid;
		public ShareString() : base()
		{
			InitializeComponent();
		}
		internal static ShareString Touch()
		{
			Log("touch");
			// todo: reentrancy
			if(instance is not null)
				return instance;

			instance = ShellPage.instance.citySetup;


			Reload();
			return instance;
		}

		private static void Reload()
		{
			Task.Run(async () =>
			{
				using var defer = new NotifyCollectionDeferral(ShareStringItem.all);
				ShareStringItem.all.Clear();
				var shares = await Tables.ReadShares(Player.myName).ConfigureAwait(false);
				await AppS.DispatchOnUIThreadTask( () =>
				{
					AddStaticLayouts();
					foreach(var s in shares)
					{
						new ShareStringItem(s.s);
					}

					return Task.CompletedTask;

				});
			});
		}



		//	public static ShareStringItem tempItem = new ShareStringItem("temp~na") { notes = "nothing selected" };
		//[ShareString.1.3]:
		//[/ShareString]
		public static (string root, string mid, string title) DecomposePath(string src)
		{
			if(src.IsNullOrEmpty())
				return (string.Empty, string.Empty, string.Empty);

			var vr = src.Split('~',StringSplitOptions.RemoveEmptyEntries);
			if(vr.Length >= 3)
			{
				var mid = vr.Skip(1).Take(vr.Length - 2);
				var midStr = String.Join('~',mid);
				return (vr[0], midStr, vr[vr.Length - 1]);
			}
			else if(vr.Length >= 2)
			{
				return (vr[0], string.Empty, vr[1]);
			}
			else if(vr.Length >= 1)
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
			if(ss.IsNullOrEmpty())
				return false;
			if(!ss.StartsWith(City.shareStringStart))
				return false;
			if(ss.Length < City.minShareStringLength)
				return false;
			return true;
		}


		static public Task Show(int cid,SetupFlags flags = SetupFlags.all)
		{
			return App.DispatchOnUIThreadExclusive(cid,async () =>
			{
				Log("enter");
				await ShowNoLock(cid,flags);
			}
			);
		}
		//		public static Nito.AsyncEx.AsyncLock showLock = new ();
		public static Task<bool> ShowNoLock(int cid,SetupFlags flags = SetupFlags.all)
		{
			Log("enter2");
			Assert(App.uiSema.CurrentCount == 0);
			Assert(cid == City.build);

			//	var sel = Spot.GetSelectedForContextMenu(cid,false, onlyMine: true);
			var me = Touch(); // loads async
			return me.ShowNoLockInternal(cid,flags);
		}
		async Task<bool> ShowNoLockInternal(int cid,SetupFlags flags = SetupFlags.all)
		{
			try
			{
				this.cid = cid;
				tradeStale=true;
				Log("enter3");

				// could be on any thread
				var city = City.GetOrAdd(cid);

				return await AppS.DispatchOnUIThreadTask(async () =>
			   {

				   try
				   {
					   NameBlade.IsOpen=false;
					   LayoutBlade.IsOpen=false;
					   TagsBlade.IsOpen=false;
					   TradeBlade.IsOpen=false;
					   AutobuildBlade.IsOpen=false;
					   HeroGrid.Width = (ShellPage.instance.grid.ActualWidth-256).Min(1200);
					   HeroGrid.Height = (ShellPage.instance.grid.ActualHeight - 200).Min(1000);
					   //			onComplete.IsOn = CityBuild.isPlanner;
					   shareStrings.SelectedItem = null;
					   // remove them all
					   // //TagsBlade.IsOpen=false;
					   // // TradeBlade.IsOpen=false;
					   // // LayoutBlade.IsOpen=false;
					   // // NameBlade.IsOpen=false;
					   //if(flags.HasFlag(SetupFlags.layout))
					   //   LayoutBlade.IsOpen=true;
					   //  if(flags.HasFlag(SetupFlags.name))
					   //   NameBlade.IsOpen=true;
					   //  if(flags.HasFlag(SetupFlags.tags))
					   //   TagsBlade.IsOpen=true;
					   //  if(flags.HasFlag(SetupFlags.trade))
					   //   TradeBlade.IsOpen=true;
					   // add in order
					   // bladeView.bl

					   SetFromSS(city.shareString,false,false);
					   res.sendFilter = ResourceFilter._true;
					   res.reqFilter = ResourceFilter._true;
					   Title = city.nameAndRemarks;
					   SetCheckboxesFromTags(city.remarks);
					   //Bindings.Update();
					   OnPropertyChanged();

					   var result = await this.ShowAsync2();
					   if(result==true)
					   {
						   var setTags = TagsBlade.IsOpen;
						   var setTrade = TradeBlade.IsOpen;
						   var setName = NameBlade.IsOpen;
						   var autobuild = AutobuildBlade.IsOpen;
						   if(setTags)
						   {
							   await SetCityTags(cid);
							   //await CitySettings.SetCitySettings(City.build, setRecruit: true);
						   }
						   if(NameBlade.IsOpen)
						   {
							   if(useSuggested.IsOn)
							   {
								   city._cityName = suggested.Text;
							   }
							   else
							   {
								   city._cityName = current.Text;
							   }

							   city.OnPropertyChanged();
							   city.BuildStageDirty();
							   await Post.Get("includes/nnch.php",$"a={HttpUtility.UrlEncode(city._cityName,Encoding.UTF8)}&cid={cid}",World.CidToPlayerOrMe(cid));
							   Note.Show($"Set name to {city._cityName}");
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
						   await CitySettings.SetCitySettings(cid,
							   autoBuildOn: autobuild&&SettingsPage.autoBuildOn.GetValueOrDefault() ? true : null,
								autoWalls: autobuild&&(SettingsPage.autoWallLevel == 10) ? true : null,
								autoTowers: autobuild&&(SettingsPage.autoTowerLevel == 10) ? true : null,
								  reqHub: (setTrade ? res.reqHub.city switch { City a => a.cid, _ => 0 } : null),
								  targetHub: (setTrade ? res.sendHub.city switch { null => 0, var a => a.cid } : null),
								   setRecruit: setTags && SettingsPage.setRecruit,
								   cartReserve: setTrade ? res.cartReserve : null,
								  shipReserve: setTrade ? res.shipReserve : null,
								   req: (setTrade ? res.req : ResourcesNullable._null),
								   max: (setTrade ? res.max : ResourcesNullable._null),
								  reqFilter: (setTrade ? reqFilter : ResourceFilter._null),
								  sendFilter: (setTrade ? sendFilter : ResourceFilter._null));

						   if(LayoutBlade.IsOpen)
						   {
							   await city.SetShareString(await GetShareStringWithJson(),true);

							   if(SettingsPage.autoRearrangeShareStrings && city.isLayoutCustom)
							   {
								   await PlannerTab.SmartRearrange(city,true);
							   }
						   }
						   city.BuildStageDirty();

						   city.OnPropertyChanged();
						   if(autobuild)
							   await DoTheStuff.Go(city,false,false);
							if(cid == City.build)
							{
							   JSClient.CityRefresh();

							}
					   }
					   return result;
				   }
				   catch(Exception ex)
				   {
					   LogEx(ex);
					   return false;
				   }

			   }).ConfigureAwait(false);
				// todo:  copy back sharestring
			}
			catch(Exception ex)
			{
				Log(ex);
				return false;
			}
		}


		public async Task<ShareStringMeta> GetMeta()
		{
			return await AppS.DispatchOnUIThreadTask(async () =>
		   {
			   var meta = new ShareStringMeta() { notes = TagHelper.ApplyTags(await TagsFromCheckboxes(),string.Empty),desc = description.Text,path = path.Text };
			   if(SettingsPage.embedTradeInShareStrings)
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
		   });
		}
		async Task<string> GetShareStringWithJson()
		{


			return GetShareString() + JsonSerializer.Serialize(await GetMeta(),JSON.jsonSerializerOptions);

		}


		string GetShareString()
		{
			return shareString.Text.Replace("\n","",StringComparison.Ordinal).Replace("\r","",StringComparison.Ordinal);

		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender,ContentDialogButtonClickEventArgs args)
		{
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender,ContentDialogButtonClickEventArgs args)
		{
		}

		private void UseBuildingsClick(object sender,RoutedEventArgs e)
		{
			var s = City.GetBuild();
			shareString.Text=City.BuildingsToShareString(s.postQueueBuildings,s.isOnWater);
		}
		public void SetCheckboxesFromTags(string remarks)
		{
			var tags = TagHelper.Get(remarks);
			foreach(var tag in TagHelper.tagsWithoutAliases)
			{
				var check = tagsPanel.Children.FirstOrDefault((a) => a is ToggleButton b && b.Content as string == tag.s) as ToggleButton;
				if(check == null)
				{
					check = new ToggleButton() { Content = tag.s };
					//					check.Checked+= (_,_)=>;
					tagsPanel.Children.Add(check);
				}
				check.IsChecked = tags.HasFlag(tag.v);
			}

		}
		static void SetValue(ref int? v,int? source)
		{
			if(source.HasValue)
				v = source.Value;
		}

		public void SetFromSS(string shareString,bool setRes,bool setTags)
		{
			var s = SplitShareString(shareString);
			ShareStringMeta meta = new();

			try
			{
				meta = JsonSerializer.Deserialize<ShareStringMeta>(s.json,JSON.jsonSerializerOptions);
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}

			var path = DecomposePath(meta.path);
			this.shareString.Text = s.ss ?? string.Empty;
			var tags = meta.notes ?? string.Empty;

			description.Text = meta.desc ?? string.Empty;
			shareTitle.Text = path.title;
			this.path.Text = CombinePath(path);

			if(setRes)
			{

				SetValue(ref res.req.wood,meta.reqWood);
				SetValue(ref res.req.stone,meta.reqStone);
				SetValue(ref res.req.iron,meta.reqIron);
				SetValue(ref res.req.food,meta.reqFood);

				SetValue(ref res.max.wood,meta.maxWood);
				SetValue(ref res.max.stone,meta.maxStone);
				SetValue(ref res.max.iron,meta.maxIron);
				SetValue(ref res.max.food,meta.maxFood);
				res.OnPropertyChanged();
			}
			if(setTags)
			{
				SetCheckboxesFromTags(tags);

			}
		}
		public static (string ss, string json) SplitShareString(string shareString)
		{
			if(shareString == null)
				return (string.Empty, AUtil.emptyJson);
			string json;
			string ss;

			var i = shareString.IndexOf('{');
			if(i == -1)
				json=   AUtil.emptyJson;
			else
				json = shareString.Substring(i);
			if(shareString.Length >= shareStringStartOffset + citySpotCount)
				ss = shareString.Substring(0,shareStringStartOffset+  citySpotCount);
			else
				ss = string.Empty;
			return (ss, json);
		}


		private async void FromClipboardClick(object sender,RoutedEventArgs e)
		{
			var text = await App.GetClipboardText();
			if(text == null)
			{
				Note.Show("Clipboard is empty");

			}
			else
			{
				Note.Show($"New Sharestring: {text}");
				shareString.Text = text;
			}
		}

		private void ToClipboardClick(object sender,RoutedEventArgs e)
		{
			App.CopyTextToClipboard(GetShareString());
		}

		private async void ShareItemInvoked(Microsoft.UI.Xaml.Controls.TreeView sender,Microsoft.UI.Xaml.Controls.TreeViewItemInvokedEventArgs args)
		{
			var i = args.InvokedItem as ShareStringItem;
			Assert(i != null);
			if(i!=null)
			{
				if(i.shareString!= null)
				{
					if(i.shareStringWithJson != null)
					{
						TagsBlade.IsOpen=true;
						var setTags = (await AppS.DoYesNoBox("Tags from ShareString","Set tags?") == 1);
						var setRes = (await AppS.DoYesNoBox("Trade Settings from ShareString","Set Trade Settings?") == 1);
						if(setRes)
						{
							await SetupTradeDefaults();
							TradeBlade.IsOpen=true;
						}
						SetFromSS(i.shareStringWithJson,setTags: setTags,setRes: setRes);

						NameBlade.IsOpen=true;
						AutobuildBlade.IsOpen=true;
					}
					else
					{
						shareString.Text = i.shareString.Replace("\n","",StringComparison.Ordinal).Replace("\r","",StringComparison.Ordinal);
						description.Text = i.desc;
						path.Text = i.path;
						shareTitle.Text = i.label;
						SetCheckboxesFromTags(i.tags);
					}
				}
			}
		}

		//private void TogglePlannerClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
		//{
		//	//onComplete.IsOn = true;
		//}

		private void ClearClick(object sender,RoutedEventArgs e)
		{
			shareString.Text = string.Empty;
		}

		private async void ShareClick(object sender,RoutedEventArgs e)
		{
			var title = this.shareTitle.Text;
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
			var json = await GetShareStringWithJson();
			Tables.ShareShareString(Player.myName,title,json);

			new ShareStringItem(json);

			Note.Show("Shared!");
		}

		private void title_TextChanged(object sender,TextChangedEventArgs e)
		{
			var ss = DecomposePath(path.Text);
			ss.title = shareTitle.Text;
			ss.root = Player.myName;

			path.Text = CombinePath(ss);

		}


		public Task<Tags> TagsFromCheckboxes()
		{
			return AppS.DispatchOnUIThreadTask(() =>
		   {
			   Tags tags = default;
			   foreach(var tag in TagHelper.tagsWithoutAliases)
			   {

				   var check = tagsPanel.Children.First((ch) => (ch as ToggleButton)?.Content == tag.s) as ToggleButton;
				   if(check.IsChecked.GetValueOrDefault())
					   tags |= tag.v;

			   }
			   return Task.FromResult(tags);
		   });
		}

		public async Task SetCityTags(int cid)
		{
			City city = City.GetOrAddCity(cid);
			await GetCity.Post(cid).ConfigureAwait(false); // need to fetch notes
			var tags = await TagsFromCheckboxes();
			city.tags = tags;
			city.remarks = TagHelper.ApplyTags(tags,city.remarks);
			//		Post.Send("includes/sNte.php", $"a={HttpUtility.UrlEncode(tags, Encoding.UTF8)}&b=&cid={cid}");
			await Post.Get("includes/sNte.php",$"a={HttpUtility.UrlEncode(city.remarks,Encoding.UTF8)}&b={HttpUtility.UrlEncode(city.notes,Encoding.UTF8)}&cid={cid}",World.CidToPlayerOrMe(cid)).ConfigureAwait(false);
		}

		private void CollapsedDisable(object sender,EventArgs e)
		{
			var exp = sender as Expander;
			Assert(exp!=null);
			exp.Header =(exp.Header as string) + " - No Change";
		}
		private void ExpandedEnable(object sender,EventArgs e)
		{
			var exp = sender as Expander;
			Assert(exp!=null);
			exp.Header = (exp.Header as string).Replace(" - No Change","");
		}


		private void toggle_Toggled(object sender,RoutedEventArgs e)
		{
		}

		async Task SetupTradeDefaults()
		{
			if(tradeStale)
			{
				tradeStale=false;
				var city = City.Get(cid);
				await NearRes.UpdateTradeStuffIfNeeded().ConfigureAwait(false);
				var tags = await TagsFromCheckboxes();
				var isHubOrStorage = tags.HasFlag(Tags.Hub) | tags.HasFlag(Tags.Storage);

				var bestReqHub = await city.AnyHub(true);
				var bestSendHub = await city.AnyHub(false);
				var hasAnyHubs = (bestReqHub!=0)||(bestSendHub!=0);
				if(!hasAnyHubs)
				{
					bestReqHub =   await CitySettings.FindBestHubWithChoice(cid,"Find Request Hub",null,isHubOrStorage).ConfigureAwait(false);
					if( !isHubOrStorage)
						bestSendHub =   await CitySettings.FindBestHub(cid,false).ConfigureAwait(false);
				}
				else
				{

				}

				await res.InitTradeSettings(city,bestReqHub,bestSendHub,ResourceFilter._true,isHubOrStorage ? ResourceFilter._null : ResourceFilter._true);
				OnPropertyChanged();
			}
		}
		private async void toggleTrade_Toggled(object sender,RoutedEventArgs e)
		{
			if(toggleTrade.IsOn )
			{
				await SetupTradeDefaults();
			}
		}
		static Regex regexCityName = new Regex(@"([^\d]*)(\d+)([^\d]+)([1-9]?)(0*)(\d+)(.*)",RegexOptions.CultureInvariant | RegexOptions.Compiled);
		public static bool IsNew(City city) => IsNew(city._cityName);
		public static bool IsNew(string _cityName)
		{
			return _cityName == "*New City";
		}
		public static bool IsNewOrCaptured(City city)
		{
			return IsNew(city._cityName) || city._cityName == "lawless city" || city._cityName == "*Lawless City";
		}

		void ChooseName(object sender,SelectionChangedEventArgs e)
		{
			var city = City.Get(cid);
			string name0 = $"{city.cont:00} 1";
			string name1 = ""; // default
			var format = (int a) => a.ToString("D3");
			var type = cityType.SelectedIndex;
			switch(type)
			{
				case 0:
				case 2:
					{
						var closestScore = float.MaxValue;

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

										//num.TryParseInt(out var numV);

										//								if(num.StartsWith("0"))

										format = type == 0 ? (int a) => a.ToString("D3") : (a) => AUtil.BeyondHex(a).ToString();
										closestScore = score;
										name0 = pre + city.cont.ToString("00") + mid + (type==2 ? "0" : "") + num;
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
					name0 = $"{city.cont:00} 00";
					name1 = ""; // default
					format = (a) => AUtil.BeyondHex(a).ToString();
					break;
			}
			for(int uid = 1;;++uid)
			{
				var name = name0 + format(uid) + name1;
				if(!City.myCities.Any((v) => v._cityName == name && v != city))
				{
					suggested.Text = name;
					break;
				}
			} // u

		}

		private async void toggleName_Toggled(object sender,RoutedEventArgs e)
		{
			if(toggleName.IsOn)
			{
				var city = City.Get(cid);
				var tags = await TagsFromCheckboxes();
				cityType.SelectionChanged-= ChooseName;
				
				if(tags.HasFlag(Tags.Hub) )
					cityType.SelectedIndex = 1;
				else if(tags.HasFlag(Tags.Storage))
					cityType.SelectedIndex = 2;
				else
					cityType.SelectedIndex =0;
				bool isNew = IsNewOrCaptured(city)||city._cityName.IsNullOrEmpty();
				useSuggested.IsOn = isNew;
				current.Text = city._cityName;
				// is this needed?

				ChooseName(null,null);	
				cityType.SelectionChanged+= ChooseName;

			}
		}

		private void NameBlade_VisibilityChanged(object sender,Visibility e)
		{
			//ChooseName(_,_);
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
		public static NotifyCollection<ShareStringItem> all = new();
		public string shareString { get; set; }
		[JsonIgnore]
		public string label
		{
			get {

				var d = ShareString.DecomposePath(path);
				return d.title;

			}
			set {
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
		public NotifyCollection<ShareStringItem> children { get; set; } = new();
		// group items
		public ShareStringItem(string _path,bool _isPath)
		{
			path = _path;
		}
		//public ShareStringItem() 
		//{
		//	path = $"{Player.myName}~tba";
		//	title = "tba";
		//}
		public override string ToString() => JsonSerializer.Serialize(this,JSON.jsonSerializerOptions);

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
				var json = s.json.Replace("&#34;","\"");
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
				var meta = JsonSerializer.Deserialize<ShareStringMeta>(json,JSON.jsonSerializerOptions);
				//	var path = ShareString.DecomposePath(meta.path);
				Ctor(meta.path,meta.notes ?? string.Empty,meta.desc ?? string.Empty,s.ss ?? string.Empty,shareString);
			}
			catch(Exception ex)
			{
				Log(ex);
			}
		}

		public ShareStringItem(string path,string _tags,string _desc,string _share)
		{
			Ctor(path,_tags,_desc,_share,null);
		}

		public void Ctor(string path,string _tags,string _desc,string _share,string _shareStringWithJson)
		{
			this.path = path;
			shareStringWithJson = _shareStringWithJson;
			tags = _tags;
			desc = _desc;
			shareString = _share;
			var dir = path.Split('~',StringSplitOptions.RemoveEmptyEntries);
			var myList = all;
			var pathSoFar = String.Empty;
			for(int i = 0;i < dir.Length - 1;++i)
			{
				pathSoFar = pathSoFar + '~' + dir[i];
				var parent = myList.c.FirstOrDefault((a) => a.label == dir[i]);
				if(parent == null)
				{
					parent = new ShareStringItem(pathSoFar,true);
					myList.Add(parent);
				}
				myList = parent.children;
			}
			myList.Remove(a => a.path==path);
			myList.Add(this);
		}



	}
}
