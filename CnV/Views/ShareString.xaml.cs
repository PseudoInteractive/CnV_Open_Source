﻿using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using static CnV.City;
using System.Text.RegularExpressions;

namespace CnV.Views
{
	using Game;

	using Microsoft.UI.Xaml.Controls;

	using ContentDialog = Microsoft.UI.Xaml.Controls.ContentDialog;
	using ContentDialogButtonClickEventArgs = Microsoft.UI.Xaml.Controls.ContentDialogButtonClickEventArgs;
	using TextChangedEventArgs = Microsoft.UI.Xaml.Controls.TextChangedEventArgs;

	//	record ab(string a=null, string b=null);
	[Flags]
	public enum SetupFlags:byte
	{
		none = 0<<1,
		name = 1<<0,
		layout = 1<<1,
		trade = 1<<2,
		tags = 1<<3,
		suggestAutobuild = 1<<4,
		all = 255,


	}

	public sealed partial class ShareString:DialogG, IANotifyPropertyChanged
	{

		#region PropertyChanged
		public void CallPropertyChanged(string member = null) {
			PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(member));
		}
		public void OnPropertyChanged(string member = null) {
			if(PropertyChanged is not null) ((IANotifyPropertyChanged)this).IOnPropertyChanged(member);
		}
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		protected override string title => city.nameAndRemarks;
		private City city => City.Get(cid);
		public static bool loadedLayouts;
		static public ShareString instance;
		private bool tradeStale;
		private int cid;
		public ShareString() : base() {
			InitializeComponent();
		}
		internal static ShareString Touch() {
			Log("touch");
			// todo: reentrancy
			if(instance is not null)
				return instance;

			instance= new();


			Reload();
			return instance;
		}

		private static void Reload() {
			Task.Run(async () => {
				using var defer = new NotifyCollectionDeferral(ShareStringItem.all);
				ShareStringItem.all.Clear();
				var shares = await Tables.ReadShares(Player.myName).ConfigureAwait(false);
				await AppS.DispatchOnUIThreadTask(() => {
					AddStaticLayouts();
					foreach(var s in shares) {
						new ShareStringItem(s.s);
					}

					return Task.CompletedTask;

				});
			});
		}



		//	public static ShareStringItem tempItem = new ShareStringItem("temp~na") { notes = "nothing selected" };
		//[ShareString.1.3]:
		//[/ShareString]
		public static (string root, string mid, string title) DecomposePath(string src) {
			if(src.IsNullOrEmpty())
				return (string.Empty, string.Empty, string.Empty);

			var vr = src.Split('~',StringSplitOptions.RemoveEmptyEntries);
			if(vr.Length >= 3) {
				var mid = vr.Skip(1).Take(vr.Length - 2);
				var midStr = String.Join('~',mid);
				return (vr[0], midStr, vr[vr.Length - 1]);
			}
			else if(vr.Length >= 2) {
				return (vr[0], string.Empty, vr[1]);
			}
			else if(vr.Length >= 1) {
				return (string.Empty, String.Empty, vr[0]);
			}
			else {
				return (string.Empty, string.Empty, string.Empty);
			}
		}

		public static string CombinePath((string root, string mid, string title) path) {
			return !path.mid.IsNullOrEmpty() ? path.root + '~' + path.mid + "~" + path.title : path.root + "~" + path.title;
		}
		public static string StripRoot(string path) {
			var strip = DecomposePath(path);
			return StripRoot(strip).subPath;
		}
		public static (string root, string subPath) StripRoot((string root, string mid, string title) path) {
			return (path.root, path.mid.IsNullOrEmpty() ? path.title : path.mid + '~' + path.title);
		}

		public static bool IsValid(string ss) {
			if(ss.IsNullOrEmpty())
				return false;
			if(!ss.StartsWith(City.shareStringStart))
				return false;
			if(ss.Length < City.minShareStringLength)
				return false;
			return true;
		}


		static public Task Show(int cid,SetupFlags flags) {
			return AppS.DispatchOnUIThreadExclusive(cid,async () => {
				Log("enter");
				await ShowNoLock(cid,flags);
			}
			);
		}
		//		public static Nito.AsyncEx.AsyncLock showLock = new ();
		public static Task ShowNoLock(int cid,SetupFlags flags) {
			Log("enter2");
			Assert(AppS.uiSema.CurrentCount == 0);
			Assert(cid == City.build);

			//	var sel = Spot.GetSelectedForContextMenu(cid,false, onlyMine: true);
			var me = Touch(); // loads async
			return me.ShowNoLockInternal(cid,flags);
		}
		async Task<bool> ShowNoLockInternal(int cid,SetupFlags flags) {
			try {
				this.cid = cid;
				tradeStale=true;
				Log("enter3");

				// could be on any thread
				var city = City.GetOrAdd(cid);

				return await AppS.DispatchOnUIThreadTask(async () => {

					try {
						NameBlade.IsOpen=false;
						LayoutBlade.IsOpen=false;
						TagsBlade.IsOpen=false;
						TradeBlade.IsOpen=false;
						AutobuildBlade.IsOpen=false;
						HeroGrid.Width = (ShellPage.instance.grid.ActualWidth-128).Min(Settings.canvasWidth);
						HeroGrid.Height = (ShellPage.instance.grid.ActualHeight - 200).Min(Settings.canvasHeight);
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
						SetCheckboxesFromTags(city.tags);
						res.city = city;
						res.InitializeFromCity();
						//Bindings.Update();
						OnPropertyChanged();

						if(flags.HasFlag(SetupFlags.layout))
							LayoutBlade.IsOpen=true;
						if(flags.HasFlag(SetupFlags.name))
							NameBlade.IsOpen=true;

						var rv = await base.Show(false);

						
						if(rv) {

							var setTags = TagsBlade.IsOpen;
							var setTrade = TradeBlade.IsOpen;
							var setName = NameBlade.IsOpen;
							var autobuild = AutobuildBlade.IsOpen;
							var wantInfoUpdate = false;

							SocketClient.DeferSendStart();

							try {
								if(setTags) {
									await SetCityTags();
									wantInfoUpdate=true;
									//await CitySettings.SetCitySettings(City.build, setRecruit: true);
									if(Settings.setRecruit) {
										CitySettings.SetRecruitFromTags(city);
									}

								}
								var nameWasNull = city.info.name.IsNullOrEmpty();
								if(NameBlade.IsOpen) {
									city.info.name = cityName.Text;
									if(city.info.name.IsNullOrEmpty())
										city.info.name = null;
									city.info.notes = cityNotes.Text;
									city.info.remarks = cityRemarks.Text;

									city.OnPropertyChanged();
									//city.BuildStageDirty();
									//await Post.Get("includes/nnch.php",$"a={HttpUtility.UrlEncode(city._cityName,Encoding.UTF8)}&cid={cid}",World.CidToPlayerOrMe(cid));
									Note.Show($"Set name to {city._cityName}");
									wantInfoUpdate=true;
								}
								if(wantInfoUpdate) {
									city.SaveInfo();
								}

								//var req = new Resources((int)reqWood.Value, (int)reqStone.Value, (int)reqIron.Value, (int)reqFood.Value);
								//var max = new Resources((int)maxWood.Value, (int)maxStone.Value, (int)maxIron.Value, (int)maxFood.Value);
								//Settings.reqWood = req.wood;
								//Settings.reqStone = req.stone;
								//Settings.reqIron = req.iron;
								//Settings.reqFood = req.food;

								//Settings.maxWood = max.wood;
								//Settings.maxStone = max.stone;
								//Settings.maxIron = max.iron;
								//Settings.maxFood = max.food;
								if(autobuild) {
									city.SetAutobuild(Settings.autobuildOn,autoTowers: Settings.autobuildTowers,
										 autoWalls: Settings.autobuildWalls,cabinLevel: (sbyte)Settings.cottageLevel);
								}

								if(setTrade) {
									// var reqFilter = res.reqFilter;
									// var sendFilter = (!city.isHubOrStorage) ? res.sendFilter : ResourceFilter._null;
									//			await CitySettings.SetTradeResourcesSettings(city.cid,req,max);
									res.Apply();
									//city.SetTradeSettings(
									//  reqHub: (setTrade ? res.reqHub.city switch { City a => a.cid, _ => 0 } : null),
									//  targetHub: (setTrade ? res.sendHub.city switch { null => 0, var a => a.cid } : null),
									//cartReserve: setTrade ? res.cartReserve : null,
									//  shipReserve: setTrade ? res.shipReserve : null,
									//req: (setTrade ? res.req : ResourcesNullable._null),
									//max: (setTrade ? res.max : ResourcesNullable._null),
									//  reqFilter: (setTrade ? reqFilter : ResourceFilter._null),
									//  sendFilter: (setTrade ? sendFilter : ResourceFilter._null));

								}

								if(LayoutBlade.IsOpen) {
									// This wasts a save
									await city.SetShareString(await GetShareStringWithJson(),true);

									if(Settings.autoRearrangeShareStrings && city.isLayoutCustom) {
										await PlannerTab.SmartRearrange(city,true);
									}
								}

							}
							catch(Exception _ex) {
								LogEx(_ex);

							}
							finally {
								SocketClient.DeferSendEnd();
							}
							city.BuildStageDirty();

							city.OnPropertyChanged();
							if(autobuild && LayoutBlade.IsOpen && city.isLayoutCustom && flags.HasFlag(SetupFlags.suggestAutobuild))
								await DoTheStuff.Go(city,false,false);
							//if(cid == City.build)
							//{
							//   Sim.CityRefresh();

							//}
						}
						return false;
					}
					catch(Exception ex) {
						LogEx(ex);
						return false;
					}

				}).ConfigureAwait(false);
				// todo:  copy back sharestring
			}
			catch(Exception ex) {
				Log(ex);
				return false;
			}
		}


		public async Task<ShareStringMeta> GetMeta() {
			return await AppS.DispatchOnUIThreadTask(async () => {
				var meta = new ShareStringMeta() {
					tags = (long)await TagsFromCheckboxes(),
					desc = description.Text,path = path.Text
				};
				if(Settings.embedTradeInShareStrings) {
					meta.reqWood = res.woodReq.IntValue();
					meta.reqStone = res.stoneReq.IntValue();
					meta.reqIron = res.ironReq.IntValue();
					meta.reqFood = res.foodReq.IntValue();
					meta.maxWood = res.woodSend.IntValue();
					meta.maxStone = res.stoneSend.IntValue();
					meta.maxIron = res.ironSend.IntValue();
					meta.maxFood = res.foodSend.IntValue();
				}
				return meta;
			});
		}
		async Task<string> GetShareStringWithJson() {


			return GetShareString() + JsonSerializer.Serialize(await GetMeta(),JSON.jsonSerializerOptions);

		}


		string GetShareString() {
			return shareString.Text.Replace("\n","",StringComparison.Ordinal).Replace("\r","",StringComparison.Ordinal);

		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender,ContentDialogButtonClickEventArgs args) {
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender,ContentDialogButtonClickEventArgs args) {
		}

		private async void UseBuildingsClick(object sender,RoutedEventArgs e) {
			var s = city;
			shareString.Text=City.LayoutToShareString(await City.LayoutFromBuildings(s.postQueueBuildings),s.isOnWater);
		}
		public void SetCheckboxesFromTags(Tags tags) {
			foreach(var tag in TagHelper.tagsWithoutAliases) {
				var check = tagsPanel.Children.FirstOrDefault((a) => a is ToggleButton b && b.Content as string == tag.s) as ToggleButton;
				if(check == null) {
					check = new ToggleButton() { Content = tag.s };
					check.SetToolTip(tag.v.EnumName());
					//					check.Checked+= (_,_)=>;
					tagsPanel.Children.Add(check);
				}
				check.IsChecked = tags.HasFlag(tag.v);
			}

		}
		static void SetValue(ref NumberBox? v,int? source) {
			if(source.HasValue)
				v.Value = source.Value;
		}

		public void SetFromSS(string shareString,bool setRes,bool setTags) {
			var s = SplitShareString(shareString);
			ShareStringMeta meta = new();

			try {
				meta = JsonSerializer.Deserialize<ShareStringMeta>(s.json,JSON.jsonSerializerOptions);
			}
			catch(Exception ex) {
				LogEx(ex);
			}

			var path = DecomposePath(meta.path);
			this.shareString.Text = s.ss ?? string.Empty;
			var tags = (Tags)meta.tags | TagHelper.Get(meta.notes);

			description.Text = meta.desc ?? string.Empty;
			shareTitle.Text = path.title;
			this.path.Text = CombinePath(path);

			if(setRes) {

				SetValue(ref res.woodReq,meta.reqWood);
				SetValue(ref res.stoneReq,meta.reqStone);
				SetValue(ref res.ironReq,meta.reqIron);
				SetValue(ref res.foodReq,meta.reqFood);

				SetValue(ref res.woodSend,meta.maxWood);
				SetValue(ref res.stoneSend,meta.maxStone);
				SetValue(ref res.ironSend,meta.maxIron);
				SetValue(ref res.foodSend,meta.maxFood);
				res.OnPropertyChanged();
			}
			if(setTags) {
				SetCheckboxesFromTags(tags);

			}
		}


		private async void FromClipboardClick(object sender,RoutedEventArgs e) {
			var text = await App.GetClipboardText();
			if(text == null) {
				Note.Show("Clipboard is empty");

			}
			else {
				Note.Show($"New Sharestring: {text}");
				shareString.Text = text;
			}
		}

		private void ToClipboardClick(object sender,RoutedEventArgs e) {
			AppS.CopyTextToClipboard(GetShareString());
		}

		private async void ShareItemInvoked(Microsoft.UI.Xaml.Controls.TreeView sender,Microsoft.UI.Xaml.Controls.TreeViewItemInvokedEventArgs args) {
			var i = args.InvokedItem as ShareStringItem;
			Assert(i != null);
			if(i!=null) {
				if(i.shareString!= null) {
					if(i.shareStringWithJson != null) {

						var setTags = (await AppS.DoYesNoBox("Tags from ShareString","Set tags?") == 1);
						var setRes = (await AppS.DoYesNoBox("Trade Settings from ShareString","Set Trade Settings?") == 1);
						if(setTags) {
							TagsBlade.IsOpen=true;
						}
						if(setRes) {
							await SetupTradeDefaults();
							TradeBlade.IsOpen=true;
						}
						SetFromSS(i.shareStringWithJson,setTags: setTags,setRes: setRes);

						NameBlade.IsOpen=true;
						if(!city.autobuild)
							AutobuildBlade.IsOpen=true;
					}
					else {
						shareString.Text = i.shareString.Replace("\n","",StringComparison.Ordinal).Replace("\r","",StringComparison.Ordinal);
						description.Text = i.desc;
						path.Text = i.path;
						shareTitle.Text = i.label;
						SetCheckboxesFromTags(i.tags);
						TagsBlade.IsOpen=true;
					}
				}
			}
		}

		//private void TogglePlannerClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
		//{
		//	//onComplete.IsOn = true;
		//}

		private void ClearClick(object sender,RoutedEventArgs e) {
			shareString.Text = string.Empty;
		}

		private async void ShareClick(object sender,RoutedEventArgs e) {
			var title = this.shareTitle.Text;
			if(title.IsNullOrEmpty()) {
				Note.Show("Please set title");
				return;
			}
			if(!IsValid(GetShareString())) {
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

		private void title_TextChanged(object sender,TextChangedEventArgs e) {
			var ss = DecomposePath(path.Text);
			ss.title = shareTitle.Text;
			ss.root = Player.myName;

			path.Text = CombinePath(ss);

		}


		public Task<Tags> TagsFromCheckboxes() {
			return AppS.DispatchOnUIThreadTask(() => {
				Tags tags = default;
				foreach(var tag in TagHelper.tagsWithoutAliases) {

					var check = tagsPanel.Children.First((ch) => (ch as ToggleButton)?.Content.ToString() == tag.s) as ToggleButton;
					if(check.IsChecked.GetValueOrDefault())
						tags |= tag.v;

				}
				return Task.FromResult(tags);
			});
		}

		public async Task SetCityTags() {
			var tags = await TagsFromCheckboxes();
			City city = this.city;
			city.info.tags = tags;
			//city.remarks = TagHelper.ApplyTags(tags,city.remarks);
			//		Post.Send("includes/sNte.php", $"a={HttpUtility.UrlEncode(tags, Encoding.UTF8)}&b=&cid={cid}");
			//	await Post.Get("includes/sNte.php",$"a={HttpUtility.UrlEncode(city.remarks,Encoding.UTF8)}&b={HttpUtility.UrlEncode(city.notes,Encoding.UTF8)}&cid={cid}",World.CidToPlayerOrMe(cid)).ConfigureAwait(false);
		}

		private void CollapsedDisable(object sender,EventArgs e) {
			var exp = sender as Expander;
			Assert(exp!=null);
			exp.Header =(exp.Header as string) + " - No Change";
		}
		private void ExpandedEnable(object sender,EventArgs e) {
			var exp = sender as Expander;
			Assert(exp!=null);
			exp.Header = (exp.Header as string).Replace(" - No Change","");
		}


		private void toggle_Toggled(object sender,RoutedEventArgs e) {
		}

		async Task SetupTradeDefaults() {
			if(tradeStale) {
				tradeStale=false;
				var city = this.city;
				//	await TradeOverview.UpdateTradeStuffIfNeeded().ConfigureAwait(false);
				var tags = await TagsFromCheckboxes();
				var isHubOrStorage = tags.HasFlag(Tags.Hub) | tags.HasFlag(Tags.Storage) | city.isHubOrStorage;

				var bestReqHub = city.AnyHub(true);
				var bestSendHub = city.AnyHub(false);
				var hasAnyHubs = (bestReqHub!=0)||(bestSendHub!=0);
				if(!hasAnyHubs) {
					bestReqHub =   await CityUI.FindBestHubWithChoice(cid,"Find Request Hub",null,isHubOrStorage);
					{
						var h = bestReqHub.AsCity();
						res.woodSource.SetCity(h,false);
						res.stoneSource.SetCity(h,false);
						res.ironSource.SetCity(h,false);
						res.foodSource.SetCity(h,false);
					}
					if(!isHubOrStorage) {
						bestSendHub =   CitySettings.FindBestHub(cid,false);
						var h = bestSendHub.AsCity();
						res.woodDest.SetCity(h,false);
						res.stoneDest.SetCity(h,false);
						res.ironDest.SetCity(h,false);
						res.foodDest.SetCity(h,false);
					}
					res.OnPropertyChanged();
				}
				else {

				}

			}
		}
		private async void toggleTrade_Toggled(object sender,RoutedEventArgs e) {
			if(toggleTrade.IsOn) {
				await SetupTradeDefaults();
			}
		}
		static Regex regexCityName = new Regex(@"([^\d]*)(\d+)([^\d]+)([1-9]?)(0*)(\d+)(.*)",RegexOptions.CultureInvariant | RegexOptions.Compiled);

		private void SuggetNameClick(object sender,RoutedEventArgs e) {
			var item = sender as MenuFlyoutItem;

			var city = this.city;
			
			
			var isStorage = item.Text == "Storage";
			var isHub = item.Text == "Hub";
			var isNormal = item.Text == "Normal";
			string name0 = $"{city.cont:00} " + (isStorage ? "01" : "1");
			string name1 = ""; // default

			var  format = (int a) => AUtil.BeyondHex(a).ToString();
			if(isNormal || isStorage)
				format =(a) => a.ToString("D3"); 
			var player = city.player;
			//var type = cityType.SelectedIndex;
			{
				if(isNormal || isStorage) {
					var closestScore = float.MaxValue;

					// normal or storage
					foreach(var v in player.myCities) {
						if(v.cont != city.cont || v._cityName == null)
							continue;
						var match = regexCityName.Match(v._cityName);
						bool hasLeadingZero = match.Groups[4].Value.IsNullOrEmpty() && !match.Groups[5].Value.IsNullOrEmpty();
						if(v.isHub) {
							var score = city.cid.DistanceToCid(v.cid);
							if(score < closestScore) {
								string mid, pre, num, post;
								if(match.Success) {


									pre = match.Groups[1].Value;
									mid = match.Groups[3].Value;
									//var cluster = match.Groups[4].Value;
									//	var leadingZeros = match.Groups[5].Value;
									num = match.Groups[6].Value;
									post = match.Groups[7].Value;
									//string numStr;
								}
								else {
									pre = string.Empty;
									mid = " ";
									num="1";
									post = string.Empty;
								}


								//num.TryParseInt(out var numV);

								//								if(num.StartsWith("0"))

								
								closestScore = score;
								name0 = pre + city.cont.ToString("00") + mid + (isStorage ? "0" : "") + num;
								name1 = post;
							}
						}


					}
				}
				else if(isHub) {
					// hub
					name0 = $"{city.cont:00} 00";
					name1 = ""; // default
					//format = (a) => AUtil.BeyondHex(a).ToString();
				}

			}
			// Hubs start at 2, 1 is for unnumbered hubs
			for(int uid = isHub ? 2 : 1;;++uid) {
				var name = name0 + format(uid) + name1;
				if(!player.myCities.Any((v) => v._cityName == name && v != city)) {
					cityName.Text = name;
					break;
				}
			} // u

		}

		private async void toggleName_Toggled(object sender,RoutedEventArgs e) {
			// Pull name settings from City
			if(toggleName.IsOn) {
				var city = this.city;
				var tags = await TagsFromCheckboxes();

				bool isNew = TagHelper.IsNewOrCaptured(city)||city._cityName.IsNullOrEmpty();

				cityName.Text = city.info.name;
				// is this needed?
				cityNotes.Text = city.info.notes;
				cityRemarks.Text = city.info.remarks;

			}
		}

		private void NameBlade_VisibilityChanged(object sender,Visibility e) {
			//ChooseName(_,_);
		}




	}




	public struct ShareStringMeta
	{
		public string path { get; set; }
		public string desc { get; set; }
		public long tags { get; set; }
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
		public string label {
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
		public Tags tags { get; set; }

		public string shareStringWithJson;
		[JsonIgnore]
		public NotifyCollection<ShareStringItem> children { get; set; } = new();
		// group items
		public ShareStringItem(string _path,bool _isPath) {
			path = _path;
		}
		//public ShareStringItem() 
		//{
		//	path = $"{Player.myName}~tba";
		//	title = "tba";
		//}
		public override string ToString() => JsonSerializer.Serialize(this,JSON.jsonSerializerOptions);

		//		public static Regex squiggleBracketMatcher = new(@"[^{}]*(?>(?>(?'open'\{)[^()]*)+(?>(?'-open'\})[^{}]*)+)+(?(open)(?!))", RegexOptions.CultureInvariant | RegexOptions.Compiled);

		public ShareStringItem(string shareString) {
			try {
				var s = SplitShareString(shareString);
				//if(s.json.Contains("}{"))
				//{
				//	int q = 0;

				//}
				var json = s.json.Replace("&#34;","\"");
				// massive hack!
				var id = json.LastIndexOf('{');
				if(id > 0) {
					json = json.Substring(id);
				}
				//var match = squiggleBracketMatcher.Match(json);
				//if( match.Captures.Count > 1 )
				//{
				//	int q = 0;
				//}
				var meta = JsonSerializer.Deserialize<ShareStringMeta>(json,JSON.jsonSerializerOptions);
				//	var path = ShareString.DecomposePath(meta.path);
				Ctor(meta.path,(Tags)meta.tags,meta.desc ?? string.Empty,s.ss ?? string.Empty,shareString);
			}
			catch(Exception ex) {
				Log(ex);
			}
		}

		public ShareStringItem(string path,string _tags,string _desc,string _share) {
			Ctor(path,TagHelper.Get(_tags),_desc,_share,null);
		}

		public void Ctor(string path,Tags _tags,string _desc,string _share,string _shareStringWithJson) {
			this.path = path;
			shareStringWithJson = _shareStringWithJson;
			tags = _tags;
			desc = _desc;
			shareString = _share;
			var dir = path.Split('~',StringSplitOptions.RemoveEmptyEntries);
			var myList = all;
			var pathSoFar = String.Empty;
			for(int i = 0;i < dir.Length - 1;++i) {
				pathSoFar = pathSoFar + '~' + dir[i];
				var parent = myList.c.FirstOrDefault((a) => a.label == dir[i]);
				if(parent == null) {
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
