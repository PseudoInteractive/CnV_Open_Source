using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

using COTG.Game;
using COTG.Helpers;
using COTG.JSON;
using COTG.Models;
using COTG.Services;
using static COTG.Debug;

using Windows.ApplicationModel;
using Windows.Globalization.NumberFormatting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Text.RegularExpressions;

namespace COTG.Views
{
	public enum Theme
	{
		cotg,
		louWinter,
		louDefault,
	}

	// TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/UWP/pages/settings-codebehind.md
	// TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
	public sealed partial class SettingsPage : ContentDialog, INotifyPropertyChanged
	{
		internal static SettingsPage instance;
		//      private static UserDataService UserDataService => Singleton<UserDataService>.Instance;

		//       private static IdentityService IdentityService => Singleton<IdentityService>.Instance;



		//private static bool _isLoggedIn;
		//private static bool _isBusy;
		public static float renderScale=1.0f;
		public static Lighting lighting = Lighting.day;
		//        private static UserData _user;
		public static bool fetchFullHistory = true;
		public static bool? autoBuildOn = true;
		public static bool setRecruit = true;
		public static bool extendedBuild=true;
		public static float planet = 0.5f;
		public static float parallax = 0.5f;
		public static string hubCitylistName = "Hubs";
		public static int reqWood = 160000;
		public static int reqStone = 205000;
		public static bool setShareString = true;
		public static bool autoBuildCabins = true;
		public static bool autoRearrangeShareStrings= true;
		public static int reqIron = 200000;
		public static int reqFood = 200000;
		public static int maxWood = 250000;
		public static int maxStone = 250000;
		public static int maxIron = 300000;
		public static int maxFood = 300000;
		public static int cabinsToRemovePerSwap= 6;
		public static int cottageLevel = 7;
		public static bool? troopsVisible;
		public static bool sendWood = true;
		public static bool sendStone = true;
		public static string continentsExport = string.Empty;
		public static bool sendIron = true;
		public static bool sendFood = true;
		public static int tsForCastle = 22000;
		public static int tsForSorcTower = 32000;
		public static int defaultFoodWarning = 12;
		public static bool showDungeonsInRegionView = false;
		public static bool applyTags=true;
		public static bool setHub = true;
		public static bool useSuggested = true;
		public static bool demoUnwantedBuildingsWithCabins = true;
		public static bool wantRaidRepeat=true;
		public static bool clearRes;
		public static bool embedTradeInShareStrings = true;
		public static bool autoDemoCottages=true;
		public static int startCabinCount = 39;
		public static float fontScale = 0.5f;
		public static float musicVolume = 0.5f;
		public static bool? autoBuildWalls=true;
		public static float minDungeonProgress = 5;
		public static int autoWallLevel = 1;
		public static int autoTowerLevel = 1;
		public static int scoutpostCount=2;
		public static bool returnRaidsBeforeSend;
		public static float flagScale=0.25f;
		public static float iconScale = 0.5f;
		public static int[] raidCarrySteps;
		public static bool[] includeRaiders = new[] {
				false, false,true,true,
				true,true,true,false,
				true,true,true,true,
				false,false,true,true,
				true,false};

		public static string VRTRatio = "1:1:1";

		public static (float v, float r,float t) vrtRatio {
			get
			{
				try
				{
					var str = VRTRatio.Split(':');
					return (float.Parse(str[0], NumberStyles.Number), float.Parse(str[1], NumberStyles.Number), float.Parse(str[2], NumberStyles.Number));
				}
				catch(Exception ex)
				{
					Note.Show($"Invalid VRT ratio {VRTRatio}, should be like '1:1:1'");
					return (1, 1, 1);
				}

			}
		}

		// rooms for 16 for now
		public static bool[] includeBuildStages = new[] {true,true,true,true,
			true,true,true,true,
			true,true,true,true,
			true,true,true,true };

		public static int raidIntervals;
		public static Resources nearResReserve = new Resources(100000, 100000, 100000, 100000);
		public static float attackMaxTravelHoursSE=40;
		public static float attackMaxTravelHoursSen=40;

		public float uiMusic
		{
			get => musicVolume;
			set
			{
				musicVolume = value;
				AGame.UpdateMusic();
			}
		}
		public static Theme theme = Theme.louWinter;
		public int uiTheme
		{
			get => (int)theme;
			set
			{
				theme = (Theme)value;
				ShellPage.RefreshAndReloadWorldData();
				Draw.CityView.LoadTheme();
				Note.Show("City theme will not update until you restart the app");
			}
		}

		public static bool cartsAreForRequests = false;
		public static string[] incomingWatch = Array.Empty<string>();
		public static int mruSize = 32;
		public static int[] pinned = Array.Empty<int>();
		public static int showAttacksLimit = 100;
		public static int showAttacksLimit0 = 30;
		public static HashSet<int> tipSeen;
		public static bool soundOn = true;
		public static float volume = 0.5f;
		public static bool spatialOn = false;
		public static bool stayAlive;
		public static bool raidOffDungeons = false;
		public static bool tintCities= true;
		public static bool raidSendExact;
		public static int resetRaidsCarry = 90;
		public static int resetRaidsIdle = 10;
		public static DateTimeOffset attackPlannerTime = AUtil.dateTimeZero;


		public static int raidsVisible = -1;
		public static bool cityListWarship=true;
		public static bool cityListShippers = true;
		public static bool cityListDefense = true;
		public static bool cityListOffense = true;
		public static bool cityListGalleys = true;
		public static bool cityListStingers = true;
		public static bool shareStringApplyTags = true;
		int uiLighting
		{
			get => (int)lighting;
			set
			{
				lighting = (Lighting)value;
				AGame.SetLighting(lighting);
			}
		}
		bool uiStayAlive
		{
			get => stayAlive;
			set { stayAlive = value; JSClient.SetStayAlive(value); }
		}
		bool uiSoundOn
		{
			get => soundOn;
			set { soundOn = value; SetSoundOn(value); }
		}
		bool uiSpatial
		{
			get => spatialOn;
			set { spatialOn = value; ElementSoundPlayer.SpatialAudioMode = SetSpatialOn(value); }
		}

		public static CityList hubCitylistUI
		{
			get => CityList.Find(hubCitylistName);
			set => hubCitylistName = value switch { null => hubCitylistName, _ => value.name };
		}

		public string visitToken;
		public string visitCookie;
		public static async void BoostVolume()
		{
			if (volume == 1 || !soundOn)
				return;
			ElementSoundPlayer.Volume = 1;
			await Task.Delay(4000);
			ElementSoundPlayer.Volume = volume; // restore
		}
		private static ElementSpatialAudioMode SetSpatialOn(bool value)
		{
			return (value) ? ElementSpatialAudioMode.Auto : ElementSpatialAudioMode.Off;
		}

		private static void SetSoundOn(bool value)
		{
			ElementSoundPlayer.State = value ? ElementSoundPlayerState.On : ElementSoundPlayerState.Off;
		}

		float uiVolume
		{
			get => volume * 100;
			set { volume = value / 100.0f; ElementSoundPlayer.Volume = volume; }
		}

		// public TipsSeen tips => TipsSeen.instance;
		public bool FetchFullHistory
		{
			get => fetchFullHistory; set
			{
				fetchFullHistory = value;
				DefenseHistoryTab.instance.Refresh();
			}

		}
		public static float raidCarryMin = 1.15f;
		public static float raidCarryMax = 1.80f;
		public static int intialStorehouses=1;
		public static bool IsThemeWinter()
		{
			return theme == Theme.louWinter;
		}
		public static void LoadAll()
		{
			///  fetchFullHistory = st.Read(nameof(fetchFullHistory),true ); // default is true

			//     TipsSeen.instance = st.Read(nameof(TipsSeen), new TipsSeen());
			//  hubCitylistName = st.Read(nameof(hubCitylistName), "Hubs");
			var props = typeof(SettingsPage).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);
			var st = App.Settings();
			if (!st.Values.ContainsKey("currentVersion") )
			{
				st.SaveString("currentVersion", "0");
			}
			
			foreach (var p in props)
			{
				try
				{
					p.SetValue(null, st.ReadT(p.Name, p.FieldType, p.GetValue(null)));
				}
				catch (Exception e)
				{
					LogEx(e);
				}
			}
			try
			{

				//reqWood = st.Read(nameof(reqWood), 160000);
				//reqStone = st.Read(nameof(reqWood), 205000);
				//reqIron = st.Read(nameof(reqIron), 100000);
				//reqFood = st.Read(nameof(reqFood), 100000);
				//maxWood = st.Read(nameof(maxWood), maxWood);
				//maxStone = st.Read(nameof(maxWood), 250000);
				//maxIron = st.Read(nameof(maxIron), 300000);
				//maxFood = st.Read(nameof(maxFood), 300000);
				//sendWood = st.Read(nameof(sendWood), true);
				//sendStone = st.Read(nameof(sendWood), true);
				//sendIron = st.Read(nameof(sendIron), true);
				//sendFood = st.Read(nameof(sendFood), true);
				//reserveCarts = st.Read(nameof(reserveCarts), reserveCarts);
				DonationTab.reserveCarts = st.Read(nameof(DonationTab.reserveCarts), 800);
				DonationTab.reserveCartsPCT = st.Read(nameof(DonationTab.reserveCartsPCT), 0.0625f);
				DonationTab.reserveShips = st.Read(nameof(DonationTab.reserveShips), 10);
				DonationTab.reserveShipsPCT = st.Read(nameof(DonationTab.reserveShipsPCT), 0f);
				DonationTab.woodStoneRatio = st.Read(nameof(DonationTab.woodStoneRatio), -1f);
				DonationTab.reserveWood = st.Read(nameof(DonationTab.reserveWood), 0);
				DonationTab.reserveStone = st.Read(nameof(DonationTab.reserveStone), 0);
				Tips.ReadSeen();
				World.LoadContinentHistory();

				// incomingWatch = st.Read(nameof(incomingWatch), Array.Empty<string>() );
				//    autoBuildOn = st.Read(nameof(autoBuildOn)+'2', -1) switch {  0 => false, 1 => true, _ => null };
				// AttackTab.time = st.Read("attacktime", DateTime.UtcNow.Date);
				SetSoundOn(soundOn);
				ElementSoundPlayer.Volume = volume;
				SetSpatialOn(spatialOn);
				if (raidCarryMin > 90)
					raidCarryMin = 1.15f; // error!
				if (raidCarryMax > 90)
					raidCarryMax = 1.75f; // error!
				if (raidCarryMax <= raidCarryMin)
					raidCarryMax = raidCarryMin*1.75f; // error!

				DungeonView.Initialize();
			}
			catch (Exception e)
			{
				LogEx(e);
			}
		}
		public static void SaveAll(object __ = null, Windows.ApplicationModel.SuspendingEventArgs _=null)
		{
			try
			{
				var props = typeof(SettingsPage).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);
				var st = App.Settings();
				//if (SpotTab.instance.spotMRU.Count>0)
				//{
				//    List<int> mru = new List<int>();
				//    foreach (var sp in SpotTab.instance.spotMRU)
				//    {
				//        if (sp.pinned)
				//            mru.Add(sp.cid);

				//    }
				//    pinned = mru.ToArray();
				//}
				foreach (var p in props)
				{
					st.SaveT(p.Name, p.FieldType, p.GetValue(null));

				}
				//6st.Save(nameof(raidCarry), Raiding.desiredCarry);

				//  st.Save(nameof(fetchFullHistory), fetchFullHistory);
				////  st.Save(nameof(TipsSeen), TipsSeen.instance);
				//  st.Save(nameof(hubCitylistName), hubCitylistName);

				//  st.Save(nameof(reqWood), reqWood);
				//  st.Save(nameof(reqStone), reqStone);
				//  st.Save(nameof(reqFood), reqFood);
				//  st.Save(nameof(reqIron), reqIron);
				//  st.Save(nameof(maxWood), maxWood);
				//  st.Save(nameof(maxStone), maxStone);
				//  st.Save(nameof(maxFood), maxFood);
				//  st.Save(nameof(maxIron), maxIron);
				//  st.Save(nameof(autoBuildOn) + '2', autoBuildOn==null ? -1 : autoBuildOn==true ? 1 : 0 );

				//  st.Save(nameof(sendWood), sendWood);
				//  st.Save(nameof(sendStone), sendStone);
				//  st.Save(nameof(sendFood), sendFood);
				//  st.Save(nameof(sendIron), sendIron);
				//  st.Save(nameof(reserveCarts), reserveCarts);
				//  st.Save(nameof(incomingWatch), incomingWatch);

				st.Save(nameof(DonationTab.reserveCarts), DonationTab.reserveCarts);
				st.Save(nameof(DonationTab.reserveCartsPCT), DonationTab.reserveCartsPCT);
				st.Save(nameof(DonationTab.reserveShips), DonationTab.reserveShips);
				st.Save(nameof(DonationTab.reserveShipsPCT), DonationTab.reserveShipsPCT);
				st.Save(nameof(DonationTab.woodStoneRatio), DonationTab.woodStoneRatio);
				st.Save(nameof(DonationTab.reserveWood), DonationTab.reserveWood);
				st.Save(nameof(DonationTab.reserveStone), DonationTab.reserveStone);
				Tips.SaveSeen();
				//  st.Save("attacktime", AttackTab.time.DateTime);

				AttackTab.WaitAndSaveAttacks();
			}
			catch (Exception e)
			{
				LogEx(e);
			}

		}

		public bool isLightTheme
		{
			get { return ThemeSelectorService.Theme == ElementTheme.Light; }
			set { if (value) ThemeSelectorService.SetThemeAsync(ElementTheme.Light); }
		}
		public bool isDarkTheme
		{
			get { return ThemeSelectorService.Theme == ElementTheme.Dark; }
			set { if (value) ThemeSelectorService.SetThemeAsync(ElementTheme.Dark); }
		}

		private static string _versionDescription;
		public static bool incomingAlwaysVisible;
		public static bool attacksAlwaysVisible;
		public static float penaltyForWrongDungeonType = 6;

		public string VersionDescription
		{
			get { return _versionDescription; }

			set { Set(ref _versionDescription, value); }
		}

		//public bool IsLoggedIn
		//{
		//    get { return _isLoggedIn; }
		//    set { Set(ref _isLoggedIn, value); }
		//}

		//public bool IsBusy
		//{
		//    get { return _isBusy; }
		//    set { Set(ref _isBusy, value); }
		//}

		//public UserData User
		//{
		//    get { return _user; }
		//    set { Set(ref _user, value); }
		//}


		public SettingsPage()
		{
			InitializeComponent();


			//         var cl = CityList.Find(hubCitylistName);
			//if (cl != null)
			//{
			//	hubCityListBox.SelectedItem = cl;
			//}
			//hubCityListBox.SelectionChanged += HubCityListBox_SelectionChanged;

		}

		//private void HubCityListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		//{
		//    var item = (sender as ComboBox).SelectedItem as CityList;
		//    if (item != null)
		//    {
		//        Debug.Log("CityList Changed to " + item.name);
		//        hubCitylistName = item.name;
		//    }
		//}



		public static void Initialize()
		{
			_versionDescription = GetVersionDescription();
			//      IdentityService.LoggedIn += OnLoggedIn;
			//       IdentityService.LoggedOut += OnLoggedOut;
			//       UserDataService.UserDataUpdated += OnUserDataUpdated;
			//_isLoggedIn = true;// IdentityService.IsLoggedIn();
			LoadAll();
		//	App.instance.Suspending += SaveAll;

			//           _user = await UserDataService.GetDefaultUserData();
			//if (Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported())
			//{
			//    FeedbackLink.Visibility = Visibility.Visible;
			//}
		}

		

		private static string GetVersionDescription()
		{
			var appName = "AppDisplayName".GetLocalized();
			var package = Package.Current;
			var packageId = package.Id;
			var version = packageId.Version;

			return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
		}

		private async void ThemeChanged_CheckedAsync(object sender, RoutedEventArgs e)
		{
			var param = (sender as RadioButton)?.CommandParameter;

			if (param != null)
			{
				await ThemeSelectorService.SetThemeAsync((ElementTheme)param);
			}
		}

		//private void OnUserDataUpdated(object sender, UserData userData)
		//{
		//    User = userData;
		//}

		//private async void OnLogIn(object sender, RoutedEventArgs e)
		//{
		//    //IsBusy = true;
		//    //var loginResult = await IdentityService.LoginAsync();
		//    //if (loginResult != LoginResultType.Success)
		//    //{
		//    //    await AuthenticationHelper.ShowLoginErrorAsync(loginResult);
		//    //    IsBusy = false;
		//    //}
		//}

		//private async void OnLogOut(object sender, RoutedEventArgs e)
		//{
		//  //  IsBusy = true;
		// //   await IdentityService.LogoutAsync();
		//}

		//private void OnLoggedIn(object sender, EventArgs e)
		//{
		//    IsLoggedIn = true;
		//    IsBusy = false;
		//}

		//private void OnLoggedOut(object sender, EventArgs e)
		//{
		//    User = null;
		//    IsLoggedIn = false;
		//    IsBusy = false;
		//}

		//protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		//{
		//    base.OnNavigatingFrom(e);
		//    IdentityService.LoggedIn -= OnLoggedIn;
		//    IdentityService.LoggedOut -= OnLoggedOut;
		//    UserDataService.UserDataUpdated -= OnUserDataUpdated;
		//}

		public event PropertyChangedEventHandler PropertyChanged;

		private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(storage, value))
			{
				return;
			}

			storage = value;
			OnPropertyChanged(propertyName);
		}

		private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private void UpdateCityLists(object sender, RoutedEventArgs e)
		{
			_ = MainPage.instance.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
			   {
				   var pid = Player.activeId;
				   using var work = new ShellPage.WorkScope("update citylists");
				   var cityListCount = CityList.all.Length;
				   string sli = null;
				   var cgs = new List<string>();
				   var newCities = CityList.FindNewCities();
				   if (newCities == null)
				   {
					   newCities = new CityList(CityList.sNewCities);
					   CityList.all = CityList.all.ArrayAppend(newCities);

				   }
				   else
				   {
					   newCities.cities.Clear();
				   }
				   var global = new List<CityList.GroupDef>();
				   var perContinent = new List<CityList.GroupDef>();
				   
				   if(SettingsPage.cityListGalleys)
					   global.Add(CityList.gdGalley);
				   if (SettingsPage.cityListStingers)
					   global.Add(CityList.gdStinger);
				   if (SettingsPage.cityListOffense)
					   perContinent.Add(CityList.gdOffense);
				   if (SettingsPage.cityListDefense)
					   perContinent.Add(CityList.gdDefense);
				   if (SettingsPage.cityListShippers)
					   global.Add(CityList.gdShipper);
				   if (SettingsPage.cityListWarship)
					   global.Add(CityList.gdWarship);
				   global.Add(CityList.gdHubs);
				   global.Add(CityList.gdLeaveMe);
				   perContinent.Add(CityList.gdStorage);

				   Dictionary<int, List<CityList>  > priorCityLists = new();
				   HashSet<CityList> processed = new();

				   foreach (var city in City.myCities)
				   {
					   var cid = city.cid;
					   COTG.Debug.Assert(city is City);
					   List<CityList> prior = new();
					   foreach (var l in CityList.all)
					   {
						   if (l.isUnassigned)
							   continue;
						   if (l.cities.Contains(cid))
						   {
							   prior.Add(l);
						   }

					   }
					   priorCityLists.Add(cid, prior);
				   }


				   foreach (var city in City.myCities)
				   {

					   var remarks = city.remarks.ToLower();
					   foreach (var t in perContinent)
					   {
						   if (remarks.ContainsAny( t.tags))
						   {
							   var cl = CityList.GetForContinentAndTag(city.cont,t.name, processed);
							   cl.cities.Add(city.cid);
						   }
					   }
					   foreach (var t in global )
					   {
						   if (remarks.ContainsAny(t.tags))
						   {
							   var cl = CityList.GetOrAdd(t.name, processed);
							   cl.cities.Add(city.cid);
						   }
					   }

					   {
						   var cl = CityList.GetForContinent(city.cont, processed);
						   cl.cities.Add(city.cid);
					   }
					   if (CityList.IsNew(city))
					   {
						   newCities.cities.Add(city.cid);
					   }
				   }
				   var addedCityLists = CityList.all.Length - cityListCount;

				   if (addedCityLists > 0)
				   {
					   var cityList = new List<string>();
					   foreach (var l in CityList.all)
					   {
						   if (l.id == 0)
							   continue;
						   cityList.Add(l.id.ToString() + l.name);
					   }
					   sli = ("a=" + HttpUtility.UrlEncode(JsonSerializer.Serialize(cityList, Json.jsonSerializerOptions)));
					   //                await Post.Send("includes/sLi.php",);
				   }
				   {
					   HashSet<CityList> temp = new ();

					   foreach (var city in City.myCities)
					   {
						   var cid = city.cid;
						   // enumerate all city
						   temp.Clear();

						   foreach (var l in CityList.all)
						   {
							   if (l.isUnassigned)
								   continue;
							   if (l.cities.Contains(cid))
							   {
								   temp.Add(l);
							   }

							   //                  await Post.Send("includes/cgS.php",  );
						   }
						   if (temp.SetEquals(priorCityLists[cid]))
							   continue;

						   var strs = temp.Select(a => a.id.ToString()).ToArray();
						   cgs.Add($"a={HttpUtility.UrlEncode(JsonSerializer.Serialize(strs, Json.jsonSerializerOptions))}&cid={cid}");
					   }
				   }
				   if (sli != null)
					   await Post.Send("includes/sLi.php", sli, pid);
				   Note.Show($"Adding {addedCityLists} citylists, updating {cgs.Count} cities...");
				   foreach (var it in cgs)
					   await Post.Send("includes/cgS.php", it, pid);
				   Note.Show($"Added {addedCityLists} citylists, updated {cgs.Count} cities");
				   //   JSClient.GetCitylistOverview();
				   //			   Note.Show($"Successfully added continent citylists :)");
			   });
			HideMe();
		}

		private void TipsRestore(object sender, RoutedEventArgs e)
		{
			Tips.seen = new HashSet<string>();
			Note.Show("Keener :)");
			HideMe();
		}
		public static void HideMe() => instance. Hide();

		//private void ExportIntelClick(object sender, RoutedEventArgs e)
		//{
		//    Cosmos.SummarizeNotes();
		//    this.Hide();
		//}

		private void CastlesIntel(object sender, RoutedEventArgs e)
		{

			{
				var cont = (int)CastlesCont.Value;
				var y = cont / 10;
				var x = cont - y * 10;
				World.DumpCities(x * 100, y * 100, (x + 1) * 100, (y + 1) * 100, CastlesAlliance.Text, onlyCastles.IsChecked.GetValueOrDefault(), this.onlyWater.IsChecked.GetValueOrDefault());
			}
			this.Hide();
		}
		private async void ShrineFinder(object sender, RoutedEventArgs e)
		{
			this.Hide();

			var cont = (int)CastlesCont.Value;
			var cy = cont / 10;
			var cx = cont - cy * 10;
			int x0 = cx * 100, y0 = cy * 100, x1 = (cx + 1) * 100, y1 = (cy + 1) * 100;
			using (new ShellPage.WorkScope("Shrine Finder"))
				for (int x = x0; x < x1; ++x)
				{
					for (int y = y0; y < y1; ++y)
					{
						if (TileData.instance.GetSpotType(x, y).type == TileData.SpotType.plain)
						{
							var cityId = (x, y).WorldToCid();
							App.DispatchOnUIThreadSneaky(() =>
							  JSClient.view.InvokeScriptAsync("gStQuery", new string[] { (cityId).ToString() })
							  );
							await Task.Delay(200);
						}
					}
				}

		}
		private async void WatchIncomingForPlayers(object sender, RoutedEventArgs e)
		{
			this.Hide();
			await Task.Delay(100);

			var names = await PlayerGroup.ChooseNames("Players to watch", incomingWatch);
			if (names != null)
			{
				incomingWatch = names;
				SaveAll();
				Note.Show("Updated incoming watch list");
				IncomingOverview.ProcessTask();
			}
		}

		private async void ImportSpots(object sender, RoutedEventArgs e)
		{
			try
			{
				var str = await Windows.ApplicationModel.DataTransfer.Clipboard.GetContent().GetTextAsync();
				foreach (Match m in AUtil.coordsRegex.Matches(str))
				{
					if (m.Value.EndsWith(':') || m.Value.StartsWith(':'))
						continue;

					var cords = m.Value.FromCoordinate();
					var s = Spot.GetOrAdd(cords);


					SpotTab.TouchSpot(s.cid, Windows.System.VirtualKeyModifiers.Shift, true, true);
				}
				Note.Show("Added spots success");

			}
			catch (Exception ex)
			{
				Note.Show("Copy strings and coords to clipboard please");
				COTG.Debug.LogEx(ex);

			}


		}
		//public async void FixupReserve(object sender, RoutedEventArgs e)
		//{
		//	var counter = 0;
		//	foreach (var a in City.myCities)
		//	{
		//		++counter;
		//		COTG.Debug.Log(counter);
		//		await CitySettings.FixupReserve(a.Value.cid);
		//	}
		//	Note.Show("Fixup reserve cmoplete");
		//}
		string appInfo
		{
			get
			{
				var package = Package.Current;
				var packageId = package.Id;
				var version = packageId.Version;

				return $"{package.DisplayName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}\nInstalled {package.InstalledDate}";
			}
		}
		public static void Show()
		{
			App.DispatchOnUIThread(async () =>
			{
				ElementSoundPlayer.Play(ElementSoundKind.Show);
				if (instance == null)
					instance = new SettingsPage();
				//shown = true;
				switch (raidsVisible)
				{
					case 0: instance.raidsVisibleCheckbox.IsChecked = false; break;
					case 1: instance.raidsVisibleCheckbox.IsChecked = true; break;
					case -1: instance.raidsVisibleCheckbox.IsChecked = null; break;

				}

				var result = await instance.ShowAsync2();
				if (!instance.visitToken.IsNullOrEmpty())
				{
					//Cosmos.PublishPlayerInfo(56996, 220 + 226*65536, instance.visitToken, instance.visitCookie);
				}

				SettingsPage.SaveAll();
				//   dialog.auto
			});
		}

		private void raidsVisibleTrue(object sender, RoutedEventArgs e)
		{
			raidsVisible = 1;
		}

		private void raidsVisibleFalse(object sender, RoutedEventArgs e)
		{
			raidsVisible = 0;
		}

		private void raidsVisibleMaybe(object sender, RoutedEventArgs e)
		{
			raidsVisible = -1;
		}

		
		private void VersionTapped(object sender, RoutedEventArgs e)
		{
			instance.Hide();
			JSClient.ShowWhatsNew();
		}
	}
}
