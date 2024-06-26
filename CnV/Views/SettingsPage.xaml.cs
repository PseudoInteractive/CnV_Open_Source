﻿using System;
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
using static CnV.Settings;

namespace CnV
{
	using Game;
	using Helpers;
	using Services;

	

	// TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/UWP/pages/settings-codebehind.md
	// TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
	public sealed partial class Settings : ContentDialog, INotifyPropertyChanged
	{
		public static double canvasHeight => AGame.clientSpan.Y-16;
		public static double canvasWidth => AGame.clientSpan.X-16;
		private const double largeFontSizeBase = 20.0;
		private const double mediumFontSizeBase = 14.0;
		private const double smallFontSizeBase = 12.0;
		private const double chatFontSizeBase = 14.0;
		private const double largeGridRowHeightBase = largeFontSizeBase*2.0;
		private const double mediumGridRowHeightBase = 32;//mediumFontSizeBase*(2.25);
		private const double smallGridRowHeightBase = 28;//smallFontSizeBase*2.25;
		private const double chatRowHeightBase = (14.0/30.0);

		internal static Settings instance;
		//      private static UserDataService UserDataService => Singleton<UserDataService>.Instance;

		//       private static IdentityService IdentityService => Singleton<IdentityService>.Instance;


		

		public float RenderQuality
		{
			get => renderQuality;
			set
			{
				
				if ( !renderQuality.AlmostEquals(value,1.0f/16.0f)  )
				{
					renderQuality = value;
					GameClient.wantFastRefresh=true;
					GameClient.UpdateRenderQuality(renderQuality);
					Note.Show("Some changes require restart");
				}
			}
		}
	
		public float uiMusic
		{
			get => musicVolume;
			set
			{
				musicVolume = value;
				Audio.UpdateMusic();
			}
		}
		public int uiTheme
		{
			get => (int)theme;
			set
			{
				theme = (Theme)value;
				ShellPage.RefreshAndReloadWorldData();
			//	CityView.LoadTheme();
				Note.Show("City theme will not completly update until you restart the app");
			}
		}
		
		int uiLighting
		{
			get => (int)lighting;
			set
			{
				if( value != (int)lighting)
				{
					lighting = (Lighting)value;
					Note.Show("Please restart to see full changes");
				}
			}
		}
		//bool uiStayAlive
		//{
		//	get => stayAlive;
		//	set { stayAlive = value; CnVServer.SetStayAlive(value); }
		//}
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
		
		//public static int raidCarryMinPercent
		//{
		//	get => (raidCarryMin*100.0).RoundToInt();
		//	set => raidCarryMin = value*(0.01f);
		//}
		//public static int raidCarryTargetPercent
		//{
		//	get => (raidCarryTarget*100.0).RoundToInt();
		//	set => raidCarryTarget = value*(0.01f);
		//}
		//public static int raidCarryMaxPercent
		//{
		//	get => (raidCarryMax*100.0).RoundToInt();
		//	set => raidCarryMax = value*(0.01f);
		//}


		

		public static async Task LoadFromPlayFab()
		{
			var settings = await APlayFab.LoadSettings();
			var props = typeof(Settings).GetFields(BindingFlags.Static  | BindingFlags.Public | BindingFlags.DeclaredOnly);
			var st = App.ClientSettings();
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
		// processing connect 2
		// move bounding
		//random offset
		// show res full
		// random 5


		public static void LoadAll()
		{
			Assert(!Settings.loadedOnce);
			
			///  fetchFullHistory = st.Read(nameof(fetchFullHistory),true ); // default is true

			//     TipsSeen.instance = st.Read(nameof(TipsSeen), new TipsSeen());
			//  hubCitylistName = st.Read(nameof(hubCitylistName), "Hubs");
			var props = typeof(Settings).GetFields(BindingFlags.Static  | BindingFlags.Public | BindingFlags.DeclaredOnly  );
			var st = App.ClientSettings();
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
				//DonationTab.reserveCarts = st.Read(nameof(DonationTab.reserveCarts), 10);
				//DonationTab.reserveCartsPCT = st.Read(nameof(DonationTab.reserveCartsPCT), 0.0625f);
				//DonationTab.reserveShips = st.Read(nameof(DonationTab.reserveShips), 0);
				//DonationTab.reserveShipsPCT = st.Read(nameof(DonationTab.reserveShipsPCT), 0f);
				//DonationTab.woodStoneRatio = st.Read(nameof(DonationTab.woodStoneRatio), -1f);
				//DonationTab.reserveWood = st.Read(nameof(DonationTab.reserveWood), 0);
				//DonationTab.reserveStone = st.Read(nameof(DonationTab.reserveStone), 0);
			//	Tips.ReadSeen();
			//	World.LoadContinentHistory();
				if(!Settings.loadedOnce)
				{
					Settings.loadedOnce = true;
					

				}
				// incomingWatch = st.Read(nameof(incomingWatch), Array.Empty<string>() );
				//    autoBuildOn = st.Read(nameof(autoBuildOn)+'2', -1) switch {  0 => false, 1 => true, _ => null };
				// AttackTab.time = st.Read("attacktime", DateTime.UtcNow.Date);
				AppS.DispatchOnUIThread( ()=>
				{
					SetSoundOn(soundOn);
					ElementSoundPlayer.Volume = volume;
					SetSpatialOn(spatialOn);
					UpdateZoom();
				});

				

				ShellPage.updateHtmlOffsets.SystemUpdated();
				//	DungeonView.Initialize();
				//layoutOffsets = new LayoutOffsets[]
				//{
				//	new(0.5f, 0.12f, 0.75f, 0.17f, 0.2f),
				//	new(0.75f, 0.0f, 0.75f, 0.17f, 0.2f),

				//	new(0.5f, 0.12f, 0.75f, 0.17f, 0.2f),

				//	new(0.5f, 0.5f, 0.75f, 0.17f, 0.2f),
				//	new(0f, 0.5f, 0.375f, 0.17f, 0.2f),

				//	new(0.875f, 0.12f, 0.75f, 0.625f, 0.5f),
				//};
			}
			catch (Exception e)
			{
				LogEx(e);
			}
		}
		public int DynamicRange
		{
			get => Settings.hdrMode;
			set {
				if(Settings.hdrMode != value)
				{
					Settings.hdrMode = value;
					Microsoft.Xna.Framework.SharpDXHelper.SetHDR(Settings.hdrMode,Settings.gammaProfile);
					GameClient.wantDeviceReset =true;
					GameClient.UpdateClientSpan.Go();
				}
			}
		}
		public int GammaProfile
		{
			get => Settings.gammaProfile;
			set {
				if(Settings.gammaProfile != value)
				{
					Settings.gammaProfile = value;
					Microsoft.Xna.Framework.SharpDXHelper.SetHDR(Settings.hdrMode,Settings.gammaProfile);
					GameClient.wantDeviceReset =true;
					GameClient.UpdateClientSpan.Go();
				}
			}
		}
		[NonSerialized]
		public static double mediumFontSize = mediumFontSizeBase;
		[NonSerialized]
		public static double largeFontSize = largeFontSizeBase;
		[NonSerialized]
		public static double smallFontSize = smallFontSizeBase;
		[NonSerialized] 
		public static double largeGridRowHeight = largeGridRowHeightBase;
		[NonSerialized]
		public static double mediumGridRowHeight = mediumGridRowHeightBase;
		[NonSerialized] 
		public static double shortGridRowHeight = smallGridRowHeightBase;

		public int PointerGestureMode
		{
			get => (int)Settings.pointerGestureMode;
			set => Settings.pointerGestureMode = (PointerGestureMode)value;
		}

		public static void UpdateZoom(object sender = null, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e = null)
		{
			UpdateXamlConstants();
			//			<x:Double x:Key="TextControlThemeMinHeight">24</x:Double>
			//		<x:Double x:Key="ListViewItemMinHeight">32</x:Double>
			var _chatZoom = chatZoom.Squared() + 0.75f;
			var _tabZoom = tabZoom.Squared()  + 0.75f;
			Log($"FontZoom {_tabZoom} Chat: {_chatZoom} Med:{mediumFontSize}");
			double AsDouble(object d) => (double)d;
			double RoundDouble(double d) => Math.Round(d);
			var mfont = RoundDouble(_tabZoom * mediumFontSizeBase);
			smallFontSize = AsDouble(App.instance.Resources["SmallFontSize"] = RoundDouble(_tabZoom * smallFontSizeBase));
			largeFontSize = AsDouble(App.instance.Resources["LargeFontSize"] = RoundDouble(_tabZoom * largeFontSizeBase));
			mediumFontSize = AsDouble(App.instance.Resources["MediumFontSize"] = mfont);

			largeGridRowHeight = AsDouble(App.instance.Resources["LargeGridRowHeight"] = RoundDouble(_tabZoom * largeGridRowHeightBase));
			mediumGridRowHeight = AsDouble(App.instance.Resources["MediumGridRowHeight"] = RoundDouble(_tabZoom * mediumGridRowHeightBase));
			shortGridRowHeight = AsDouble(App.instance.Resources["ShortGridRowHeight"] = RoundDouble(_tabZoom * smallGridRowHeightBase));

			App.instance.Resources["ControlContentThemeFontSize"] =mfont;
			App.instance.Resources["ContentControlFontSize"] = mfont;
			App.instance.Resources["ContentControlFontSize"] = mfont;
		}


		static bool updatinghConstants;
		internal static void UpdateXamlConstants()
		{
			if(updatinghConstants)
				return;
			updatinghConstants=true;
			AppS.DispatchOnUIThreadLow(()=>{
			
				App.instance.Resources["canvasHeight"] = Settings.canvasHeight;
				App.instance.Resources["ContentDialogMaxHeight"] = Settings.canvasHeight;
				lock(DialogG.all)
				{
					foreach(var a in DialogG.all)
						a.MaxHeight =  Settings.canvasHeight;
				}
				updatinghConstants=false;

			})
			;
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

		////public static string versionDescription = string.Empty;
		////public static bool incomingAlwaysVisible;
		////public static bool attacksAlwaysVisible;
		////public static float penaltyForWrongDungeonType = 6;
		////public static ushort scoreForSorcTower = 2000;
		////public static int notificationDuration = 3;
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


		public Settings()
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

		//private void UpdateCityLists(object sender, RoutedEventArgs e)
		//{
		//	AppS.QueueOnUIThreadIdle( async () =>
		//	   {
		//		   var pid = Player.myId;
		//		   using var work = new WorkScope("update citylists");
		//		   var cityListCount = CityList.all.Length;
		//		   string sli = null;
		//		   var cgs = new List<string>();
		//		   var newCities = CityList.FindNewCities();
		//		   if (newCities == null)
		//		   {
		//			   newCities = new CityList(CityList.sNewCities);
		//			   CityList.all = CityList.all.ArrayAppend(newCities);

		//		   }
		//		   else
		//		   {
		//			   newCities.cities.Clear();
		//		   }
		//		   var global = new List<CityList.GroupDef>();
		//		   var perContinent = new List<CityList.GroupDef>();
				   
		//		   if(Settings.cityListGalleys)
		//			   global.Add(CityList.gdGalley);
		//		   if (Settings.cityListStingers)
		//			   global.Add(CityList.gdStinger);
		//		   if (Settings.cityListOffense)
		//			   perContinent.Add(CityList.gdOffense);
		//		   if (Settings.cityListDefense)
		//			   perContinent.Add(CityList.gdDefense);
		//		   if (Settings.cityListShippers)
		//			   global.Add(CityList.gdShipper);
		//		   if (Settings.cityListWarship)
		//			   global.Add(CityList.gdWarship);
		//		   global.Add(CityList.gdHubs);
		//		   global.Add(CityList.gdLeaveMe);
		//		   perContinent.Add(CityList.gdStorage);

		//		   Dictionary<int, List<CityList>  > priorCityLists = new();
		//		   HashSet<CityList> processed = new();

		//		   foreach (var city in City.myCities)
		//		   {
		//			   var cid = city.cid;
		//			   Debug.Assert(city is City);
		//			   List<CityList> prior = new();
		//			   foreach (var l in CityList.all)
		//			   {
		//				   if (l.isUnassigned)
		//					   continue;
		//				   if (l.cities.Contains(cid))
		//				   {
		//					   prior.Add(l);
		//				   }

		//			   }
		//			   priorCityLists.Add(cid, prior);
		//		   }


		//		   foreach (var city in City.myCities)
		//		   {

		//			   var remarks = city.remarks.ToLower();
		//			   foreach (var t in perContinent)
		//			   {
		//				   if (remarks.ContainsAny( t.tags))
		//				   {
		//					   var cl = CityList.GetForContinentAndTag(city.cont,t.name, processed);
		//					   cl.cities.Add(city.cid);
		//				   }
		//			   }
		//			   foreach (var t in global )
		//			   {
		//				   if (remarks.ContainsAny(t.tags))
		//				   {
		//					   var cl = CityList.GetOrAdd(t.name, processed);
		//					   cl.cities.Add(city.cid);
		//				   }
		//			   }

		//			   {
		//				   var cl = CityList.GetForContinent(city.cont, processed);
		//				   cl.cities.Add(city.cid);
		//			   }
		//			   if (city.IsNew())
		//			   {
		//				   newCities.cities.Add(city.cid);
		//			   }
		//		   }
		//		   var addedCityLists = CityList.all.Length - cityListCount;

		//		   if (addedCityLists > 0)
		//		   {
		//			   var cityList = new List<string>();
		//			   foreach (var l in CityList.all)
		//			   {
		//				   if (l.id == 0)
		//					   continue;
		//				   cityList.Add(l.id.ToString() + l.name);
		//			   }
		//			   sli = ("a=" + HttpUtility.UrlEncode(JsonSerializer.Serialize(cityList, JSON.jsonSerializerOptions)));
		//			   //                await Post.Send("includes/sLi.php",);
		//		   }
		//		   {
		//			   HashSet<CityList> temp = new ();

		//			   foreach (var city in City.myCities)
		//			   {
		//				   var cid = city.cid;
		//				   // enumerate all city
		//				   temp.Clear();

		//				   foreach (var l in CityList.all)
		//				   {
		//					   if (l.isUnassigned)
		//						   continue;
		//					   if (l.cities.Contains(cid))
		//					   {
		//						   temp.Add(l);
		//					   }

		//					   //                  await Post.Send("includes/cgS.php",  );
		//				   }
		//				   if (temp.SetEquals(priorCityLists[cid]))
		//					   continue;

		//				   var strs = temp.Select(a => a.id.ToString()).ToArray();
		//				   cgs.Add($"a={HttpUtility.UrlEncode(JsonSerializer.Serialize(strs, JSON.jsonSerializerOptions))}&cid={cid}");
		//			   }
		//		   }
		//		   if (sli != null)
		//			   await Post.Get("includes/sLi.php", sli, pid);
		//		   Note.Show($"Adding {addedCityLists} citylists, updating {cgs.Count} cities...");
		//		   foreach (var it in cgs)
		//			   await Post.Get("includes/cgS.php", it, pid);
		//		   Note.Show($"Added {addedCityLists} citylists, updated {cgs.Count} cities");
		//		   //   CnVServer.GetCitylistOverview();
		//		   //			   Note.Show($"Successfully added continent citylists :)");
		//	   });
		//	HideMe();
		//}

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
		//private async void ShrineFinder(object sender, RoutedEventArgs e)
		//{
		//	var cont = (int)CastlesCont.Value;
		//	this.Hide();

		//	var cy = cont / 10;
		//	var cx = cont - cy * 10;
		//	int x0 = cx * 100, y0 = cy * 100, x1 = (cx + 1) * 100, y1 = (cy + 1) * 100;
		//	using (new WorkScope("Shrine Finder"))
		//		for (int x = x0; x < x1; ++x)
		//		{
		//			ShellPage.WorkUpdate($"Shrine Finder {cont}:  {x - x0}%");
		//			for (int y = y0; y < y1; ++y)
		//			{
		//				if (TileData.instance.GetSpotType(x, y).type == TileData.TileType.plain)
		//				{
		//					var cityId = (x, y).WorldToCid();
		//					await AppS.DispatchOnUIThreadTask( async () =>
		//					  await CnVServer.ExecuteScriptAsync("gStQuery", (cityId) )
		//					  );
		//					await Task.Delay(100);
		//				}
		//			}
		//		}
		//}

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
				//IncomingOverview.ProcessTask();
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

					s.SetPinned(true);
					SpotTab.AddToGrid(s, false);
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
			AppS.DispatchOnUIThread(async () =>
			{
				ElementSoundPlayer.Play(ElementSoundKind.Hide);
				if (instance == null)
					instance = new Settings();

				//shown = true;
				// set subbers
		//		instance.subbers.ItemsSource = World.activePlayers;
				instance.subbers.SetPlayers(Player.me.subbers);
			//	instance.subbees.ItemsSource = World.activePlayers;
				instance.subbees.SetPlayers(Player.me.subbees);
			///	instance.stackRoot.MaxWidth = instance.ActualWidth;
			//	instance.UpdateLayout();
				var result = await instance.ShowAsync2();
				//if (!instance.visitToken.IsNullOrEmpty())
				//{
				//	//Cosmos.PublishPlayerInfo(56996, 220 + 226*65536, instance.visitToken, instance.visitCookie);
					
				//}
				if(!Player.me.subbers.SequenceEqual(instance.subbers.players )) 
				{
					Note.Show("Updated Sub privileges");
					Player.me.subbers = instance.subbers.players;
					(new CnVEventPlayerSubs(Player.me.myCities.First().c,Player.myId,Player.me.subbers)).EnqueueAsap();
				}
				if(AppS.isTest) {
					var subbees = Player.me.subbees;
					if(!subbees.SequenceEqual(instance.subbees.players)) {

						Note.Show("Updated Sub privileges");
						foreach(var subbee in instance.subbees.players) {
							if(subbees.Contains(subbee))
								continue;
							var p2 = Player.Get(subbee);
							Assert(!p2.subbers.Contains(Player.myId));
							p2.subbers = p2.subbers.ArrayAppend(Player.myId);
//							Player.me.subbers = instance.subbers.players;
//							(new CnVEventPlayerSubs(Player.me.myCities.First().c,Player.myId,Player.me.subbers)).EnqueueAsap();
						}
					}
				}
					

				Settings.SaveAll();
				//   dialog.auto
			});
		}

		
		
		private void VersionTapped(object sender, RoutedEventArgs e)
		{
			instance.Hide();
			ShowWhatsNew();
		}

		//private void ExportRanks(object sender, RoutedEventArgs e)
		//{
		//	AppS.HideFlyout(sender);
		//	HideMe();
		//	var cont = exportRanksCont.Value.RoundToInt().ContinentToXY().ContinentXYToContinentId();
		//	var t1 = Sim.serverTime;
		//	Blobs.AllianceStats(t1 - TimeSpan.FromDays(exportRanksDays.Value), t1, cont, exportRanksCities.Value.RoundToInt() );
		//}


		//private void ExportTS(object sender, RoutedEventArgs e)
		//{
		//	AppS.HideFlyout(sender);
		//	HideMe();
		//	var cont = Settings.exportContinent.ContinentToXY().ContinentXYToContinentId();
		//	var tsMin = exportTSMinTS.Value.RoundToInt();
		//	var t1 = Sim.serverTime;
		//	Blobs.PlayerStats(t1-TimeSpan.FromDays(exportTSDays.Value), t1,cont,tsMin,
		//		this.exportTSScore.IsChecked.GetValueOrDefault(),
		//		this.exportTSCities.IsChecked.GetValueOrDefault(),
		//		exportTSAlliance.IsChecked.GetValueOrDefault(), 
		//		exportTSPlayers.Value.RoundToInt(),
		//		exportTSTotal.IsChecked.GetValueOrDefault(),
		//		exportTSOff.IsChecked.GetValueOrDefault(),
		//		exportTSDef.IsChecked.GetValueOrDefault()
		//		);


		//}

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
		private void MouseScrollTapped(object sender,RoutedEventArgs e) {
			Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-settings:mousetouchpad"));
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

		private async void SignoutTapped(object sender, RoutedEventArgs e)
		{
			Hide();
			await CnVSignin.SignOut();
			AppS.Exit();
			await Task.Delay(-1);
		}

		private async void Reset(object sender,RoutedEventArgs e) {
			var c = Player.active.cities;
			if(await AppS.DoYesNoBox($"Reset {Player.active.name}","Are you sure?") == 1) {

				for(int i = c.Length;--i>= 0;) {
					new CnVEventAbandon( (WorldC)(c[i]),Player.activeId,i==0).EnqueueAsap();
				}
			}

        }


        //private async void ChangeNameTapped(object sender, RoutedEventArgs e)
        //{
        //	Hide();
        //	await CnVSignin.EditProfile();

        //}
    }

}
