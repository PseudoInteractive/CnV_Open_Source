using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

using CnV.Game;
using CnV.Helpers;

using CnV.Models;
using CnV.Services;
using static CnV.Debug;
using CommunityToolkit.WinUI;
using Windows.ApplicationModel;
using Windows.Globalization.NumberFormatting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using MessagePack;
using Windows.Storage;
using System.Collections.Immutable;
using CnV;

namespace CnV.Views
{
	using Game;
	using Helpers;
	using Services;

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
		private const double largeFontSizeBase = 20.0;
		private const double mediumFontSizeBase = 14.0;
		private const double smallFontSizeBase = 12.0;
		private const double chatFontSizeBase = 14.0;
		internal static SettingsPage instance;
		//      private static UserDataService UserDataService => Singleton<UserDataService>.Instance;

		//       private static IdentityService IdentityService => Singleton<IdentityService>.Instance;


		public static bool nearResAsRatio=true;

		public static bool? syncIncoming = null;
		public static bool? syncOutgoing = null;
		public static float tabZoom = 0.5f;
		public static float chatZoom = 0.5f;
		public static string attackPlanName = "PlanB";
		//private static bool _isLoggedIn;
		//private static bool _isBusy;
		public static float renderScale=1.0f;
		
		public static float webZoom = 0.875f;
		public static float webZoomSmall = 0.5f;


		public static float HtmlZoom
		{
			get => webZoom;
			set {
				if(webZoom != value)
				{
					webZoom = value;
					ShellPage.updateHtmlOffsets.Go(false);
				}
			}
		}
		public static float HtmlZoomSmall
		{
			get => webZoomSmall;
			set {
				if(webZoomSmall != value)
				{
					webZoomSmall = value;
					ShellPage.updateHtmlOffsets.Go(false);
				}
			}
		}
		public static Lighting lighting = Lighting.day;

		public static bool donationsProportionateToWhatsNeeded = true;

		public static string playerName = string.Empty;
		public static string playerEmail = string.Empty;
		public static string playerPassword = string.Empty;
		//        private static UserData _user;
		public static bool fetchFullHistory = false;
		public static bool? autoBuildOn = true;
		public static bool setRecruit = true;
		public static bool extendedBuild=true;
		public static float planet = 0.5f;
		public static float parallax = 0.5f;
		public static string hubCitylistName = "Hubs";
		public static string exportPlayer = string.Empty;
		public static string exportAlliance = string.Empty;
		public static bool drawBuildingOverlays=true;
		public static float raidTroopFraction = 1;
		public static float returnRaidsBias = 1.0f;
		public static bool autoBuildCabins = true;
		public static bool autoRearrangeShareStrings= true;
		public static ResourcesNullable defaultReq = new (200000,220000,200000,250000);
		public static ResourcesNullable defaultSend = new(250000,250000,300000,350000);
		public static int cabinsToRemovePerSwap= 6;
		public static int cottageLevel = 7;
		public static bool? troopsVisible;
//		public static bool sendWood = true;
//		public static bool sendStone = true;
		public static int exportContinent = 22;
//		public static bool sendIron = true;
//		public static bool sendFood = true;
		public static int tsForCastle = 22000;
		public static int tsForSorcTower = 32000;
		public static int defaultFoodWarning = 12;
		public static bool showDungeonsInRegionView = false;
		public static bool applyTags=true;
		public static bool setHub = true;
		

		public static bool wantRaidRepeat=true;
		public static bool clearOnlyCenterRes;
		public static bool clearRes=true;
		public static bool embedTradeInShareStrings = true;
		public static bool? demoCottageOnBuildIfFull;
		public static bool? demoBuildingOnBuildIfFull;
		public static int startCabinCount = 39;
		public static float fontScale = 0.5f;
		public static float musicVolume = 0.5f;
		public static bool? autoBuildWalls=true;
		public static float minDungeonCompletion = 15;
		public static int autoWallLevel = 1;
		public static int autoTowerLevel = 1;
		public static int scoutpostCount=2;
		public static bool returnRaidsBeforeSend;
		public static float flagScale=0.25f;
		public static float iconScale = 0.5f;

		public static bool[] includeRaiders = new[] {
				false, false,true,true,
				true,true,true,false,
				true,true,true,true,
				false,false,true,true,
				true,false
		};

		public static string VRTRatio = "1:1:1";

		public static (float v, float r,float t) vrtRatio {
			get
			{
				try
				{
					var str = VRTRatio.Split(':');
					return (float.Parse(str[0], NumberStyles.Any), float.Parse(str[1], NumberStyles.Any), float.Parse(str[2], NumberStyles.Any));
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
		public static int nearResCartReserve = 100;
		public static int nearResShipReserve = 0;
		public static Resources nearResReserve = new Resources(100000, 100000, 100000, 100000);
		public static Resources nearResSend = new Resources(100000,100000,100000,100000);

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
				Note.Show("City theme will not completly update until you restart the app");
			}
		}

		public static string[] incomingWatch = Array.Empty<string>();
		public static byte exportOffence;
		public static byte exportWater;
		public static byte exportCastles;
		public static bool onlyTemples;
		public static bool? exportHeaders = true;
		public static bool? exportScore;
		public static byte exportWho;

		public static int chooseAttackTypeIndex;
		public static bool chooseAttackTypeUpdate;

		//public static string secSessionId;
		public static int mruSize = 32;
		[NonSerialized]
		public static int[] pinned = Array.Empty<int>();

	//	public static bool isPinnedLoaded => pinned != null; 
		public static int showAttacksLimit = 100;
		public static int showAttacksLimit0 = 30;
		[NonSerialized]
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
		public static int raidSendMinIdle = 10; //


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
			get => fetchFullHistory; 
			set
			{
				fetchFullHistory = value;
				//DefenseHistoryTab.instance.refresh.Go();
			}

		}
		public static int raidReserveCommandSlots = 0;
		public static float raidMaxTriariRatio = 2;
		public static float raidCarryVsDistance = 0.5f;

		public static float raidCarryMin = 0.9f;
		public static float raidCarryTarget = 1.15f;
		public static float raidCarryMax = 2.00f;
		public static int intialStorehouses=1;
		public static int intialMarkets = 1;
		public static int raidCarryMinPercent
		{
			get => (raidCarryMin*100.0).RoundToInt();
			set => raidCarryMin = value*(0.01f);
		}
		public static int raidCarryTargetPercent
		{
			get => (raidCarryTarget*100.0).RoundToInt();
			set => raidCarryTarget = value*(0.01f);
		}
		public static int raidCarryMaxPercent
		{
			get => (raidCarryMax*100.0).RoundToInt();
			set => raidCarryMax = value*(0.01f);
		}


		public static bool IsThemeWinter()
		{
			return theme == Theme.louWinter;
		}
		static bool loadedOnce;

		public static async Task LoadFromPlayFab()
		{
			var settings = await APlayFab.LoadSettings();
			var props = typeof(SettingsPage).GetFields(BindingFlags.Static  | BindingFlags.Public | BindingFlags.DeclaredOnly);
			var st = App.Settings();
			foreach(var p in props)
			{
				if(!p.IsNotSerialized)
				{
					try
					{
						if (settings.TryGetValue(p.Name, out var v))
						{
							p.SetValue(null, v);
							st.Values[p.Name] = v;
						}
					}
					catch(Exception e)
					{
						LogEx(e);
					}
				}
			}
		}


		public static void LoadAll()
		{
			///  fetchFullHistory = st.Read(nameof(fetchFullHistory),true ); // default is true

			//     TipsSeen.instance = st.Read(nameof(TipsSeen), new TipsSeen());
			//  hubCitylistName = st.Read(nameof(hubCitylistName), "Hubs");
			var props = typeof(SettingsPage).GetFields(BindingFlags.Static  | BindingFlags.Public | BindingFlags.DeclaredOnly  );
			var st = App.Settings();
			if (!st.Values.ContainsKey("currentVersion") )
			{
				st.SaveString("currentVersion", "0");
			}
			
			foreach (var p in props)
			{
				if (!p.IsNotSerialized)
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
			}
			try
			{
				// try loading as dictionary

			//	LoadPinned();
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
			//	Tips.ReadSeen();
			//	World.LoadContinentHistory();
				if(!loadedOnce)
				{
					loadedOnce = true;
					

				}
				// incomingWatch = st.Read(nameof(incomingWatch), Array.Empty<string>() );
				//    autoBuildOn = st.Read(nameof(autoBuildOn)+'2', -1) switch {  0 => false, 1 => true, _ => null };
				// AttackTab.time = st.Read("attacktime", DateTime.UtcNow.Date);
				AppS.DispatchOnUIThread( ()=>
				{
					SetSoundOn(soundOn);
					ElementSoundPlayer.Volume = volume;
					SetSpatialOn(spatialOn);
				});
				if (raidCarryMin > 90)
					raidCarryMin = 1.15f; // error!
				if (raidCarryMax > 90)
					raidCarryMax = 1.75f; // error!
				if (raidCarryMax <= raidCarryMin)
					raidCarryMax = raidCarryMin*1.75f; // error!
				AppS.DispatchOnUIThread(()=>	UpdateZoom() );
				//	DungeonView.Initialize();
			
			}
			catch (Exception e)
			{
				LogEx(e);
			}
		}

		

		[NonSerialized]
		public static float mediumFontSize = 14;
		[NonSerialized]
		public static float largeFontSize = 20;
		[NonSerialized]
		public static float smallFontSize = 12;
		[NonSerialized] 
		public static float largeGridRowHeight = 34;
		[NonSerialized]
		public static GridLength largeGridRowHeighL = new(largeGridRowHeight);
		[NonSerialized]
		public static float mediumGridRowHeight = 30;
		[NonSerialized] 
		public static float shortGridRowHeight = 28;

		public static void UpdateZoom(object sender = null, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e = null)
		{
			var chatZoom = SettingsPage.chatZoom.Squared()  + 0.75f;
			var tabZoom = SettingsPage.tabZoom.Squared()  + 0.75f;
			float AsFloat(object d) => (float)(double)d;
			smallFontSize = AsFloat(App.instance.Resources["SmallFontSize"] = tabZoom * smallFontSizeBase);
			largeFontSize = AsFloat(App.instance.Resources["LargeFontSize"] = tabZoom * 20.0);
			mediumFontSize = AsFloat(App.instance.Resources["MediumFontSize"] = tabZoom * mediumFontSizeBase);
			App.instance.Resources["ChatFontSize"] = chatZoom * 12.0;

			largeGridRowHeight = AsFloat(App.instance.Resources["LargeGridRowHeight"] = tabZoom * (largeFontSizeBase*2.25));
		
			mediumGridRowHeight = AsFloat(App.instance.Resources["MediumGridRowHeight"] = tabZoom * (mediumFontSizeBase*2.25));
			shortGridRowHeight = AsFloat(App.instance.Resources["ShortGridRowHeight"] = tabZoom * (smallFontSizeBase*2.25) );

			App.instance.Resources["ChatFontSize"] = chatZoom * chatFontSizeBase;
			App.instance.Resources["ChatFontImageHeight"] = chatZoom * 32.0;
			

		}


		public static void SaveAll(object __ = null, Windows.ApplicationModel.SuspendingEventArgs _=null)
		{
			if (!loadedOnce)
				return;
			try
			{
				CityCustom.Save(); // synchronous
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
				var settings = new Dictionary<string, object>();
				foreach (var p in props)
				{
					if (!p.IsNotSerialized)
					{
						var o = p.GetValue(null);
						st.SaveT(p.Name, p.FieldType,o );
						settings[p.Name] = o;
					}

				}

				APlayFab.SaveSettings(settings);
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
				//Tips.SaveSeen();
				//  st.Save("attacktime", AttackTab.time.DateTime);

//				AttackTab.SaveAttacksBlock();
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

		public static string versionDescription = string.Empty;
		public static bool incomingAlwaysVisible;
		public static bool attacksAlwaysVisible;
		public static float penaltyForWrongDungeonType = 6;
		public static ushort scoreForSorcTower = 2000;
		public static int notificationDuration = 3;
		//public string VersionDescription
		//{
		//	get { return _versionDescription; }

		//	set { Set(ref _versionDescription, value); }
		//}

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
			var package = Package.Current;
			var packageId = package.Id;
			var version = packageId.Version;

			versionDescription =   $"{package.DisplayName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}\nInstalled {package.InstalledDate}";

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
			AppS.DispatchOnUIThreadLow( async () =>
			   {
				   var pid = Player.activeId;
				   using var work = new WorkScope("update citylists");
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
					   Debug.Assert(city is City);
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
					   if (city.IsNew())
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
					   sli = ("a=" + HttpUtility.UrlEncode(JsonSerializer.Serialize(cityList, JSON.jsonSerializerOptions)));
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
						   cgs.Add($"a={HttpUtility.UrlEncode(JsonSerializer.Serialize(strs, JSON.jsonSerializerOptions))}&cid={cid}");
					   }
				   }
				   if (sli != null)
					   await Post.Get("includes/sLi.php", sli, pid);
				   Note.Show($"Adding {addedCityLists} citylists, updating {cgs.Count} cities...");
				   foreach (var it in cgs)
					   await Post.Get("includes/cgS.php", it, pid);
				   Note.Show($"Added {addedCityLists} citylists, updated {cgs.Count} cities");
				   //   JSClient.GetCitylistOverview();
				   //			   Note.Show($"Successfully added continent citylists :)");
			   });
			HideMe();
		}

		private void TipsRestore(object sender, RoutedEventArgs e)
		{
		//	Tips.seen = new HashSet<string>();
			Note.Show("Keener :)");
			HideMe();
		}
		public static void HideMe() => instance. Hide();

		//private void ExportIntelClick(object sender, RoutedEventArgs e)
		//{
		//    Cosmos.SummarizeNotes();
		//    this.Hide();
		//}

		//private void CastlesIntel(object sender, RoutedEventArgs e)
		//{

		//	{
		//		var cont = (int)CastlesCont.Value;
		//		var y = cont / 10;
		//		var x = cont - y * 10;
		//		World.DumpCities(x * 100, y * 100, (x + 1) * 100, (y + 1) * 100, exportAllianceMask, onlyCastles.IsChecked.GetValueOrDefault(), this.onlyWater.IsChecked.GetValueOrDefault());
		//	}
		//	this.Hide();
		//}
		private async void ShrineFinder(object sender, RoutedEventArgs e)
		{
			var cont = (int)CastlesCont.Value;
			this.Hide();

			var cy = cont / 10;
			var cx = cont - cy * 10;
			int x0 = cx * 100, y0 = cy * 100, x1 = (cx + 1) * 100, y1 = (cy + 1) * 100;
			using (new WorkScope("Shrine Finder"))
				for (int x = x0; x < x1; ++x)
				{
					ShellPage.WorkUpdate($"Shrine Finder {cont}:  {x - x0}%");
					for (int y = y0; y < y1; ++y)
					{
						if (TileData.instance.GetSpotType(x, y).type == TileData.SpotType.plain)
						{
							var cityId = (x, y).WorldToCid();
							await AppS.DispatchOnUIThreadTask( async () =>
							  await JSClient.ExecuteScriptAsync("gStQuery", (cityId) )
							  );
							await Task.Delay(100);
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
			HideMe();
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
				Debug.LogEx(ex);

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
		
		public static void Show()
		{
			AppS.DispatchOnUIThreadLow(async () =>
			{
				ElementSoundPlayer.Play(ElementSoundKind.Hide);
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
			},true);
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
			ShowWhatsNew();
		}

		private void ExportRanks(object sender, RoutedEventArgs e)
		{
			AppS.HideFlyout(sender);
			HideMe();
			var cont = exportRanksCont.Value.RoundToInt().ContinentToXY().XYToPackedContinent();
			var t1 = CnVServer.ServerTime();
			Blobs.AllianceStats(t1 - TimeSpan.FromDays(exportRanksDays.Value), t1, cont, exportRanksCities.Value.RoundToInt() );
		}


		private void ExportTS(object sender, RoutedEventArgs e)
		{
			AppS.HideFlyout(sender);
			HideMe();
			var cont = SettingsPage.exportContinent.ContinentToXY().XYToPackedContinent();
			var tsMin = exportTSMinTS.Value.RoundToInt();
			var t1 = CnVServer.ServerTime();
			Blobs.PlayerStats(t1-TimeSpan.FromDays(exportTSDays.Value), t1,cont,tsMin,
				this.exportTSScore.IsChecked.GetValueOrDefault(),
				this.exportTSCities.IsChecked.GetValueOrDefault(),
				exportTSAlliance.IsChecked.GetValueOrDefault(), 
				exportTSPlayers.Value.RoundToInt(),
				exportTSTotal.IsChecked.GetValueOrDefault(),
				exportTSOff.IsChecked.GetValueOrDefault(),
				exportTSDef.IsChecked.GetValueOrDefault()
				);


		}

		private async void LaunchTapped(object sender, RoutedEventArgs e)
		{
			try
			{
				HideMe();
				await Windows.System.Launcher.LaunchUriAsync(new Uri($"{App.appLink}:launch?n=1"));
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
		}

		private async void DisplayTapped(object sender,RoutedEventArgs e)
		{
			try
			{
				await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-settings:display"));
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
		}
		static StorageFolder folder => ApplicationData.Current.LocalFolder;
		public static async void ShowWhatsNew()
		{

			try
			{
				var dialog = new WhatsNewDialog();
				dialog.DefaultButton = Microsoft.UI.Xaml.Controls.ContentDialogButton.Primary;
				dialog.fixesText.Text = new StreamReader((typeof(Fixes).Assembly).GetManifestResourceStream($"CnV.Wiki.fixes.md")).ReadToEnd();

				var result = await dialog.ShowAsync2();

			}
			catch(Exception __ex)
			{
				Debug.LogEx(__ex);
			}
		}
	}
}
