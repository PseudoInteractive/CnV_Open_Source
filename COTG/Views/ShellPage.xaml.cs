using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using COTG.Core.Helpers;
using COTG.Core.Services;
using COTG.Helpers;
using COTG.Models;
using COTG.Services;

using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Controls;
using static COTG.Debug;
using WinUI = Microsoft.UI.Xaml.Controls;
using COTG.Game;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Foundation;

namespace COTG.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page, INotifyPropertyChanged
    {
        private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
        private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);
        static public ShellPage instance;
        public Frame shellFrame;
        private bool _isBackEnabled;
        private WinUI.NavigationViewItem _selected;
        private bool _isBusy;
        private bool _isLoggedIn;
        private bool _isAuthorized;
        private IdentityService IdentityService => Singleton<IdentityService>.Instance;

        private UserDataService UserDataService => Singleton<UserDataService>.Instance;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }
            set { Set(ref _isBackEnabled, value); }
        }

        public WinUI.NavigationViewItem Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }



        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set(ref _isBusy, value); }
        }

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set { Set(ref _isLoggedIn, value); }
        }

        public bool IsAuthorized
        {
            get { return _isAuthorized; }
            set { Set(ref _isAuthorized, value); }
        }

        public ShellPage()
        {
            instance = this;
            InitializeComponent();
            DataContext = this;
            Initialize();
        }

        private void Initialize()
        {
            NavigationService.NavigationFailed += Frame_NavigationFailed;
            NavigationService.Navigated += Frame_Navigated;
            NavigationService.OnCurrentPageCanGoBackChanged += OnCurrentPageCanGoBackChanged;
            navigationView.BackRequested += OnBackRequested;
            IdentityService.LoggedIn += OnLoggedIn;
            IdentityService.LoggedOut += OnLoggedOut;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {

            var webView = JSClient.Initialize(panel);
            shellFrame = new Frame()
            {
                Background = null
           //  HorizontalAlignment=HorizontalAlignment.Stretch,
           //   VerticalAlignment=VerticalAlignment.Stretch
           ,
                CacheSize = 10
            };
            //   RelativePanel.SetAlignLeftWithPanel(shellFrame, true);
            //      RelativePanel.SetAlignRightWithPanel(shellFrame, true);
            //      RelativePanel.SetAlignTopWithPanel(shellFrame, true);
            //      RelativePanel.SetAlignBottomWithPanel(shellFrame, true);

            panel.Background = null;

            panel.Children.Add(webView);

            panel.Children.Add(shellFrame);
            Grid.SetColumn(shellFrame, 4);
            Grid.SetRow(shellFrame, 0);
            Grid.SetRowSpan(shellFrame, 5);
            Canvas.SetZIndex(shellFrame, 3);


            Grid.SetColumn(webView, 0);
            Grid.SetRow(webView, 0);
            Grid.SetRowSpan(webView, 5);
            Grid.SetColumnSpan(webView, 4);
            Canvas.SetZIndex(webView, 0);

            var splitter = new GridSplitter();
            panel.Children.Add(splitter);
            Grid.SetColumn(splitter, 4);
            Grid.SetRowSpan(splitter, 4);
            splitter.Width = 16;
            splitter.HorizontalAlignment = HorizontalAlignment.Left;
            splitter.ResizeDirection = GridSplitter.GridResizeDirection.Columns;
            Canvas.SetZIndex(splitter, 5);

            NavigationService.Frame = shellFrame;

            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            // More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8
            KeyboardAccelerators.Add(_altLeftKeyboardAccelerator);
            KeyboardAccelerators.Add(_backKeyboardAccelerator);
            IsLoggedIn = true;// IdentityService.IsLoggedIn();
            IsAuthorized = true;// IsLoggedIn && IdentityService.IsAuthorized();
            // panel.hor
            Services.NavigationService.Navigate<Views.MainPage>();
            navigationView.IsPaneOpen = false;

        }


        private void OnLoggedIn(object sender, EventArgs e)
        {
            IsLoggedIn = true;
            IsAuthorized = true;// IsLoggedIn && IdentityService.IsAuthorized();
            IsBusy = false;
        }

        private void OnLoggedOut(object sender, EventArgs e)
        {
            IsLoggedIn = false;
            IsAuthorized = false;
            CleanRestrictedPagesFromNavigationHistory();
            GoBackToLastUnrestrictedPage();
        }

        private void CleanRestrictedPagesFromNavigationHistory()
        {
            NavigationService.Frame.BackStack
.Where(b => Attribute.IsDefined(b.SourcePageType, typeof(Restricted)))
.ToList()
.ForEach(page => NavigationService.Frame.BackStack.Remove(page));
        }

        private void GoBackToLastUnrestrictedPage()
        {
            var currentPage = NavigationService.Frame.Content as Page;
            var isCurrentPageRestricted = Attribute.IsDefined(currentPage.GetType(), typeof(Restricted));
            if (isCurrentPageRestricted)
            {
                NavigationService.GoBack();
            }
        }
        void TestGet(object sender, RoutedEventArgs e)
        {
            JSClient.TestGet();
        }

        private async void OnUserProfile(object sender, RoutedEventArgs e)
        {
            if (IsLoggedIn)
            {
                NavigationService.Navigate<SettingsPage>();
            }
            //else
            //{
            //    IsBusy = true;
            //    var loginResult = await IdentityService.LoginAsync();
            //    if (loginResult != LoginResultType.Success)
            //    {
            //        await AuthenticationHelper.ShowLoginErrorAsync(loginResult);
            //        IsBusy = false;
            //    }
            //}
        }

        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw e.Exception;
        }

        private void OnCurrentPageCanGoBackChanged(object sender, bool currentPageCanGoBack)
            => IsBackEnabled = NavigationService.CanGoBack || currentPageCanGoBack;

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;
            if (e.SourcePageType == typeof(SettingsPage))
            {
                Selected = navigationView.SettingsItem as WinUI.NavigationViewItem;
                return;
            }

            var selectedItem = GetSelectedItem(navigationView.MenuItems, e.SourcePageType);
            if (selectedItem != null)
            {
                Selected = selectedItem;
            }
        }

        private WinUI.NavigationViewItem GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
        {
            foreach (var item in menuItems.OfType<WinUI.NavigationViewItem>())
            {
                if (IsMenuItemForPageType(item, pageType))
                {
                    return item;
                }

                var selectedChild = GetSelectedItem(item.MenuItems, pageType);
                if (selectedChild != null)
                {
                    return selectedChild;
                }
            }

            return null;
        }

        private bool IsMenuItemForPageType(WinUI.NavigationViewItem menuItem, Type sourcePageType)
        {
            var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
            return pageType == sourcePageType;
        }

        private void OnItemInvoked(WinUI.NavigationView sender, WinUI.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavigationService.Navigate<SettingsPage>(null, args.RecommendedNavigationTransitionInfo);
                return;
            }

            if (args.InvokedItemContainer is WinUI.NavigationViewItem selectedItem)
            {
                var pageType = selectedItem.GetValue(NavHelper.NavigateToProperty) as Type;
                NavigationService.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void OnBackRequested(WinUI.NavigationView sender, WinUI.NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.GoBack();
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var result = NavigationService.GoBack();
            args.Handled = result;
        }

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


        public void TestPost(object o, RoutedEventArgs e)
        {
            JSClient.TestGet();
        }

        public void Refresh(object o, RoutedEventArgs e)
        {
            JSClient.Refresh(o, e);

        }



        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async void TestPost2(object sender, RoutedEventArgs e)
        {
            await RestAPI.regionView.Post();

        }
        private async void TestGoCity(object sender, RoutedEventArgs e)
        {
            await RestAPI.goCity.Post();

        }
        private async void GetWorldInfo(object sender, RoutedEventArgs e)
        {
            await RestAPI.getWorldInfo.Post();

        }
        private async void GetPPDT(object sender, RoutedEventArgs e)
        {
            await JSClient.GetPPDT();

        }

        static string[] buildings = { "forester", "cottage", "storehouse", "quarry", "hideaway", "farmhouse", "cityguardhouse", "barracks", "mine", "trainingground", "marketplace", "townhouse", "sawmill", "stable", "stonemason", "mage_tower", "windmill", "temple", "smelter", "blacksmith", "castle", "port", "port", "port", "shipyard", "shipyard", "shipyard", "townhall", "castle" };


        private void ShowBuildings(object sender, PointerRoutedEventArgs e)
        {
            Log("Show Buildings");
            List<BuildingCount> bd = new List<BuildingCount>();
            foreach (var b in buildings)
            {
                var bmp = new BitmapImage(new Uri(JSClient.httpsHost, $"images/city/buildings/icons/{b}.png"));// { Width = 40, height = 40 };
                Log(bmp.UriSource.ToString());
            bd.Add(new BuildingCount() { count = 5, image = bmp });

            }
            var button = sender as Button;
            button.Focus(FocusState.Programmatic);

            buildingList.ItemsSource = bd;
            buildingList.Width = Double.NaN;
            buildingList.Height = Double.NaN;

            //  buildingList.Height = ((bd.Count + 5) / 6) * 60+10;
            //  buildingList.DesiredSize
            button.Flyout.OverlayInputPassThroughElement = button;
            buildingList.UpdateLayout();
            
 //           buildingList.UpdateLayout();
       //     button.Flyout.with
            var mouseC = e.GetCurrentPoint(null).Position;
            const float spawn = 20.0f;
            //            button.Focus(FocusState.Programmatic);

//            var button.Flyout.Update = new Rect(mouseC.X - spawn, mouseC.Y - spawn, mouseC.X + spawn, mouseC.Y + spawn);

            var avoid = new Rect(mouseC.X - spawn, mouseC.Y - spawn, mouseC.X + spawn, mouseC.Y + spawn);
            button.Flyout.ShowAt(button, new FlyoutShowOptions() { Placement=FlyoutPlacementMode.Full, ShowMode=FlyoutShowMode.Transient }); // ,ExclusionRect=avoid });

           
        }

		private void DoNothing(object sender, RoutedEventArgs e)
		{

		}

        private void FlyoutClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            Log("Why is this closing?");
        }

        private void GridLostMouse(object sender, PointerRoutedEventArgs e)
        {
            var 

        }
    }
}
