using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using COTG.Core.Helpers;
using COTG.Core.Services;
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
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/UWP/pages/settings-codebehind.md
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : ContentDialog, INotifyPropertyChanged
    {
        //      private static UserDataService UserDataService => Singleton<UserDataService>.Instance;

        //       private static IdentityService IdentityService => Singleton<IdentityService>.Instance;



        //private static bool _isLoggedIn;
        //private static bool _isBusy;
        //        private static UserData _user;
        public static bool fetchFullHistory = true;
        public static bool? autoBuildOn = null;
        public static string hubCitylistName = "Hubs";
        public static int reqWood = 160000;
        public static int reqStone = 205000;
        public static int reqIron = 200000;
        public static int reqFood = 200000;
        public static int maxWood = 250000;
        public static int maxStone = 250000;
        public static int maxIron = 300000;
        public static int maxFood = 300000;
        public static int cottageLevel = 8;
        public static bool sendWood = true;
        public static bool sendStone = true;
        public static bool sendIron = true;
        public static bool sendFood = true;
        public static bool cartsAreForRequests = false;
        public static string[] incomingWatch = Array.Empty<string>();
        public static int mruSize = 32;
        public static int[] pinned = Array.Empty<int>();
        public static int showAttacksLimit = 100;
		public static HashSet<int> tipSeen;

        // public TipsSeen tips => TipsSeen.instance;
        public bool FetchFullHistory
        {
            get => fetchFullHistory; set
            {
                fetchFullHistory = value;
                DefenseHistoryTab.instance.Refresh();
            }

        }
        float raidCarry { get => Raiding.desiredCarry; set => Raiding.desiredCarry = value; }

        public static void LoadAll()
        {
            ///  fetchFullHistory = st.Read(nameof(fetchFullHistory),true ); // default is true

            //     TipsSeen.instance = st.Read(nameof(TipsSeen), new TipsSeen());
            //  hubCitylistName = st.Read(nameof(hubCitylistName), "Hubs");
            var props = typeof(SettingsPage).GetFields(System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.DeclaredOnly);
            var st = App.Settings();
            foreach (var p in props)
            {
                try
                {
                    p.SetValue(null, st.ReadT(p.Name, p.FieldType, p.GetValue(null)));
                }
                catch (Exception e)
                {
                    Log(e);
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
                Raiding.desiredCarry = st.Read(nameof(raidCarry), Raiding.desiredCarry);
                //reserveCarts = st.Read(nameof(reserveCarts), reserveCarts);
                DonationTab.reserveCarts = st.Read(nameof(DonationTab.reserveCarts), 800);
                DonationTab.reserveCartsPCT = st.Read(nameof(DonationTab.reserveCartsPCT), 0.0625f);
                DonationTab.reserveShips = st.Read(nameof(DonationTab.reserveShips), 10);
                DonationTab.reserveShipsPCT = st.Read(nameof(DonationTab.reserveShipsPCT), 0f);
                DonationTab.woodStoneRatio = st.Read(nameof(DonationTab.woodStoneRatio), -1f);
                DonationTab.reserveWood = st.Read(nameof(DonationTab.reserveWood), 0);
                DonationTab.reserveStone = st.Read(nameof(DonationTab.reserveStone), 0);
				Tips.ReadSeen();
				

                // incomingWatch = st.Read(nameof(incomingWatch), Array.Empty<string>() );
                //    autoBuildOn = st.Read(nameof(autoBuildOn)+'2', -1) switch {  0 => false, 1 => true, _ => null };
                // AttackTab.time = st.Read("attacktime", DateTime.UtcNow.Date);
            }
            catch (Exception e)
            {
                Log(e);
            }
        }
        public static void SaveAll(object _ = null, Windows.UI.Core.CoreWindowEventArgs __ = null)
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
                st.Save(nameof(raidCarry), Raiding.desiredCarry);

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

                AttackTab.SaveAttacks();
            }
            catch (Exception e)
            {
                Log(e);
            }

        }
        public ElementTheme ElementTheme
        {
            get { return ThemeSelectorService.Theme; }

            set { Set(ref ThemeSelectorService.Theme, value); }
        }
        public bool isLightTheme
        {
            get { return ThemeSelectorService.Theme == ElementTheme.Light; }
            set { if(value ) ThemeSelectorService.Theme = ElementTheme.Light; }
        }
        public bool isDarkTheme
        {
            get { return ThemeSelectorService.Theme == ElementTheme.Dark; }
            set { if (value) ThemeSelectorService.Theme = ElementTheme.Dark; }
        }

        private static string _versionDescription;

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

            var cl = CityList.Find(hubCitylistName);
            if (cl != null)
                hubCityListBox.SelectedItem = cl;
            hubCityListBox.SelectionChanged += HubCityListBox_SelectionChanged;

        }

        private void HubCityListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = hubCityListBox.SelectedItem as CityList;
            if (item != null)
            {
                Debug.Log("CityList Changed to " + item.name);
                hubCitylistName = item.name;
            }
        }



        public static void Initialize()
        {
            _versionDescription = GetVersionDescription();
            //      IdentityService.LoggedIn += OnLoggedIn;
            //       IdentityService.LoggedOut += OnLoggedOut;
            //       UserDataService.UserDataUpdated += OnUserDataUpdated;
            //_isLoggedIn = true;// IdentityService.IsLoggedIn();
            LoadAll();
            Window.Current.Closed -= SaveAll;
            Window.Current.Closed += SaveAll;

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

        private void SetContinentCityLists(object sender, RoutedEventArgs e)
        {
            _ = MainPage.instance.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
               {
                   using var work = new ShellPage.WorkScope("update citylists");
                   var cityListCount = CityList.all.Length;
                   var changed = new HashSet<int>();
                   var temp = new List<string>();
                   string sli = null;
                   var cgs = new List<string>();
                   var newCities = CityList.FindNewCities();
                   if (newCities == null)
                   {
                       newCities = new CityList(CityList.sNewCities);
                       CityList.all = CityList.all.ArrayAppend(newCities);

                   }
                   {
                       var priorNewCities = newCities.cities;
                       newCities.cities= new HashSet<int>();
                       foreach (var city in priorNewCities)
                       {
                           if (!CityList.IsNew(City.GetOrAddCity(city)))
                           {
                               changed.Add(city);
                           }
                           else
                           {
                               newCities.cities.Add(city);
                           }
                       }
                   }
                   foreach (var city in City.allCities.Values)
                   {
                       COTG.Debug.Assert(city is City);

                       var remarks = city.remarks.ToLower();
                       foreach (var t in CityList.perContinentTags)
                       {
                           if (remarks.Contains(t))
                           {
                               var cl = CityList.GetForContinentAndTag(city.cont, t);
                               if (cl.cities.Add(city.cid))
                                   changed.Add(city.cid);
                           }
                       }
                       foreach (var t in CityList.globalTags)
                       {
                           if (remarks.Contains(t))
                           {
                               var cl = CityList.GetOrAdd(t);
                               if (cl.cities.Add(city.cid))
                                   changed.Add(city.cid);
                           }
                       }

                       {
                           var cl = CityList.GetForContinent(city.cont);
                           if (cl.cities.Add(city.cid))
                               changed.Add(city.cid);
                       }
                       if (CityList.IsNew(city))
                       {
                           if (newCities.cities.Add(city.cid))
                               changed.Add(city.cid);
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
                       sli = ("a=" + HttpUtility.UrlEncode(JsonSerializer.Serialize(cityList)));
                       //                await Post.Send("includes/sLi.php",);
                   }
                   foreach (var cid in changed)
                   {
                       // enumerate all city
                       temp.Clear();
                       foreach (var l in CityList.all)
                       {
                           if (l.id == 0)
                               continue;
                           if (l.cities.Contains(cid))
                           {
                               temp.Add(l.id.ToString());
                           }

                           //                  await Post.Send("includes/cgS.php",  );
                       }
                       cgs.Add($"a={HttpUtility.UrlEncode(JsonSerializer.Serialize(temp))}&cid={cid}");
                   }
                   if (sli != null)
                       await Post.Send("includes/sLi.php", sli);
                   Note.Show($"Adding {addedCityLists} citylists, updating {cgs.Count} cities...");
                   foreach (var it in cgs)
                       await Post.Send("includes/cgS.php", it);
                   Note.Show($"Added {addedCityLists} citylists, updated {cgs.Count} cities");
                   //   JSClient.GetCitylistOverview();
                   //			   Note.Show($"Successfully added continent citylists :)");
               });
            this.Hide();
        }

        private void TipsRestore(object sender, RoutedEventArgs e)
        {
            Tips.seen = new HashSet<string>();
            Note.Show("Keener :)");
            this.Hide();
        }

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
                World.DumpCities(x*100, y*100, (x+1)*100, (y+1)*100, CastlesAlliance.Text, onlyCastles.IsChecked.GetValueOrDefault());
            }
            this.Hide();
        }
        private async void ShrineFinder(object sender, RoutedEventArgs e)
        {
            this.Hide();
           
            var cont = (int)CastlesCont.Value;
            var cy = cont / 10;
            var cx = cont - cy * 10;
            int x0 = cx*100, y0 = cy*100, x1 = (cx+1)*100, y1 = (cy+1)*100;
            using (new ShellPage.WorkScope("Shrine Finder"))
            for (int x = x0; x<x1; ++x)
            {
                for (int y = y0; y<y1; ++y)
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
                    if (m.Value.EndsWith(':')||m.Value.StartsWith(':'))
                        continue;

                    var cords = m.Value.FromCoordinate();
                    var s = Spot.GetOrAdd(cords);


                    SpotTab.TouchSpot(s.cid, Windows.System.VirtualKeyModifiers.Shift,true,true);
                }
                Note.Show("Added spots success");

            }
            catch (Exception ex)
            {
                Note.Show("Copy strings and coords to clipboard please");
                COTG.Debug.Log(ex);

            }


        }
        public async void FixupReserve(object sender, RoutedEventArgs e)
        {
            var counter = 0;
            foreach (var a in City.allCities)
            {
                ++counter;
                COTG.Debug.Log(counter);
                await CitySettings.FixupReserve(a.Value.cid);
            }
            Note.Show("Fixup reserve cmoplete");
        }
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

    }
}
