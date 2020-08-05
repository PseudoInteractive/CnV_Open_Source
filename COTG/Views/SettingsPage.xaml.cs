using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using COTG.Core.Helpers;
using COTG.Core.Services;
using COTG.Game;
using COTG.Helpers;
using COTG.Models;
using COTG.Services;

using Windows.ApplicationModel;
using Windows.Globalization.NumberFormatting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace COTG.Views
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/UWP/pages/settings-codebehind.md
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
  //      private static UserDataService UserDataService => Singleton<UserDataService>.Instance;

 //       private static IdentityService IdentityService => Singleton<IdentityService>.Instance;

        private static ElementTheme _elementTheme = ThemeSelectorService.Theme;
        //private static bool _isLoggedIn;
        //private static bool _isBusy;
//        private static UserData _user;
        public static bool fetchFullHistory;
        public TipsSeen tips => TipsSeen.instance;
        public bool FetchFullHistory { get=>fetchFullHistory; set
            {
                fetchFullHistory = value;
                DefensePage.instance.Refresh();
              }

        }
        float raidCarry { get => Raiding.desiredCarry; set => Raiding.desiredCarry = value;  }

        public static void LoadAll()
        {
            fetchFullHistory = App.Settings().Read<bool>(nameof(fetchFullHistory),true ); // default is true
            Raiding.desiredCarry = App.Settings().Read<float>(nameof(raidCarry), 1.02f);
            TipsSeen.instance = App.Settings().Read<TipsSeen>(nameof(TipsSeen), new TipsSeen());

        }
        public static void SaveAll(object _=null, Windows.UI.Core.CoreWindowEventArgs __ =null)
        {
           App.Settings().Save(nameof(fetchFullHistory), fetchFullHistory);
            App.Settings().Save(nameof(raidCarry), Raiding.desiredCarry);
            App.Settings().Save(nameof(TipsSeen), TipsSeen.instance);

        }
        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
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
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public  static void Initialize()
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

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
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
				   var r = new Random();
				   var addedContinents = 0;
				   List<int> changed = new List<int>();
				   var temp = new List<string>();
				   string sli = null;
				   var cgs = new List<string>();
				   foreach (var city in City.allCities.Values)
				   {
					   var cl = CityList.FindForContinent(city.cont);
					   if (cl == null)
					   {
						   var id = r.Next(65536) + 10000;
						   cl = new CityList() { id = id, name = city.cont.ToString() };
						   CityList.all = CityList.all.ArrayAppend(cl);
						   ++addedContinents;
					   }

					   if (cl.cities.Add(city.cid))
						   changed.Add(city.cid);
				   }
				   if (addedContinents > 0)
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
				   Note.Show($"Added {addedContinents} continent citylists, updating {cgs.Count} cities");
				   foreach (var it in cgs)
					   await Post.Send("includes/cgS.php", it);
				   //   JSClient.GetCitylistOverview();
				   Note.Show($"Successfully added continent citylists :)");
			   });
		}

        private void TipsRestore(object sender, RoutedEventArgs e)
        {
            TipsSeen.instance = new TipsSeen();
            Note.Show("Keener :)");
        }

        //private async void FeedbackLink_Click(object sender, RoutedEventArgs e)
        //{
        //    // This launcher is part of the Store Services SDK https://docs.microsoft.com/windows/uwp/monetize/microsoft-store-services-sdk
        //    var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
        //    await launcher.LaunchAsync();
        //}
    }
}
