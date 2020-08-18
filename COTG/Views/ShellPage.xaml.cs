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
using System.Collections.Concurrent;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;
using System.Text;
using System.Collections.ObjectModel;
using COTG.JSON;
using System.Threading;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Hosting;

namespace COTG.Views
{
    //public class LogEntryStruct
    //{
    //    public string t { get; set; }
    //    public LogEntryStruct()
    //    {

    //    }
    //    public LogEntryStruct(string _t) { t =_t; } 
    //}
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page, INotifyPropertyChanged
    {

        //private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
        //private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);
        //private readonly KeyboardAccelerator _forwardKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoForward);
        static public ShellPage instance;
        private bool _isBackEnabled;
        private WinUI.NavigationViewItem _selected;
        private bool _isBusy;
        private bool _isLoggedIn;
        private bool _isAuthorized;

        private IdentityService IdentityService => Singleton<IdentityService>.Instance;

        private UserDataService UserDataService => Singleton<UserDataService>.Instance;

        public static InAppNotification inAppNote => instance.InAppNote;

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
            Initialize();
        }

        private void Initialize()
        {
            //        NavigationService.NavigationFailed += Frame_NavigationFailed;
            //        NavigationService.Navigated += Frame_Navigated;
            //        NavigationService.OnCurrentPageCanGoBackChanged += OnCurrentPageCanGoBackChanged;
            //       navigationView.BackRequested += OnBackRequested;
            //  IdentityService.LoggedIn += OnLoggedIn;
            //   IdentityService.LoggedOut += OnLoggedOut;
        }

        public static void SetHeaderText(string text)
        {
            //    if(instance!=null && instance.navigationView!=null)
            //        instance.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => instance.status.Label=text );
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {



            var webView = JSClient.Initialize(grid);
            foreach (var i in webView.KeyboardAccelerators)
                i.IsEnabled = false;
            webView.AllowFocusOnInteraction = false;

            grid.Background = null;

            grid.Children.Add(webView);

            //          grid.Children.Add(shellFrame);
            //           Grid.SetColumn(shellFrame, 2);
            //           Grid.SetRow(shellFrame, 0);
            //           Grid.SetRowSpan(shellFrame, 6);
            //          shellFrame.Margin = new Thickness(13, 0, 0, 0);
            //          Canvas.SetZIndex(shellFrame, 3);


            Grid.SetColumn(webView, 0);
            Grid.SetRow(webView, 1);
            Grid.SetRowSpan(webView, 5);
            Grid.SetColumnSpan(webView, 2);
            Canvas.SetZIndex(webView, 0);

            var canvas = CreateCanvasControl();
          //  canvas.ContextFlyout = CityFlyout;
            grid.Children.Add(canvas);
            Grid.SetColumn(canvas, 1);
            Grid.SetRow(canvas, 1);
            Grid.SetRowSpan(canvas, 4);
            Grid.SetColumnSpan(canvas, 1);
            canvas.BorderThickness = new Thickness(0, 0, 0, 0);
            canvas.Margin = new Thickness(0, 0, 0, 36);
            Canvas.SetZIndex(canvas, 11);
            //           Task.Run(SetupCanvasInput);

            //   var img = new Image() { Opacity=0.5f, Source = new SvgImageSource(new Uri($"ms-appx:///Assets/world20.svg")),IsHitTestVisible=false };

            //   grid.Children.Add(img);

            //   Grid.SetRowSpan(img, 4);
            //   Grid.SetColumnSpan(img, 4);
            //   Canvas.SetZIndex(img, 12);


            //var splitter = new GridSplitter();
            //grid.Children.Add(splitter);
            //Grid.SetColumn(splitter, 2);
            //// Grid.SetRowSpan(splitter, 4);
            ////  splitter.Height = 200;
            //splitter.Width = 8;
            //splitter.Height = 200;
            //Grid.SetRowSpan(splitter, 5);
            //splitter.HorizontalAlignment = HorizontalAlignment.Left;
            //splitter.VerticalAlignment = VerticalAlignment.Stretch;
            //splitter.ResizeDirection = GridSplitter.GridResizeDirection.Columns;
            //Canvas.SetZIndex(splitter, 5);

            //      NavigationService.Frame = shellFrame;

            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            // More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8

            KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, NavStack.BackInvoked, VirtualKeyModifiers.Menu));
            KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack,NavStack.BackInvoked));

            KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Right, NavStack.ForwardInvoked, VirtualKeyModifiers.Menu));
            KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoForward, NavStack.ForwardInvoked));

            KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.F5, Refresh_Invoked));

            IsLoggedIn = true;// IdentityService.IsLoggedIn();
            IsAuthorized = true;// IsLoggedIn && IdentityService.IsAuthorized();
                                // grid.hor
            /// we pass this as an argument to let the page know that it is a programmatic navigation
            // Services.NavigationService.Navigate<Views.DefensePage>(this);
         //   ChatTab.Ctor();
            {
                ChatTab.Ctor();
                CreateTabPage(chatTabFrame).AddChatTabs();

                UserTab.InitUserTabs();
                {
                    var tabPage = CreateTabPage(shellFrame);
                    TabPage.mainTabs = tabPage;
                    tabPage.AddTab(MainPage.instance, true);

                    tabPage.AddTab(DefenderPage.instance, false);

                  //   tabPage.AddTab(DefensePage.instance, false);

                    tabPage.AddTab(DonationTab.instance, false);
                    tabPage.AddTab(BossTab.instance, false);

                }
                {
                    var tabPage = CreateTabPage(spotFrame);
                    //   SpotTab.instance = new SpotTab();
                    tabPage.AddTab(SpotTab.instance, true);

                }
            };

            //			refreshAccelerator.Invoked += (_, __) => view?.Refresh();
            //  testMenu.Items.Add(MenuAction(MainPage.ShowTipRaiding1,"TipRaiding1"));
            //   testMenu.Items.Add(MenuAction(MainPage.ShowTipRaiding2, "TipRaiding2"));
            //   testMenu.Items.Add(MenuAction(MainPage.ShowTipRaiding3, "TipRaiding3"));
            cityListBox.SelectedIndex = 0; // reset
            cityListBox.SelectionChanged += CityListBox_SelectionChanged;
            cityBox.SelectionChanged += CityBox_SelectionChanged;
            heatMapFlyout.Content = new HeatmapDatePicker();
            SystemNavigationManager.GetForCurrentView().BackRequested += ShellPage_BackRequested;
        }

        private void ShellPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Log("Back!!");
            NavStack.Back();
            e.Handled = true;
        }

        private void Refresh_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            Refresh();

            Note.Show("Refresh");
           args.Handled = true;
        }

        //public static MenuFlyoutItem MenuAction( Action a, string text)
        //{
        //    var rv = new MenuFlyoutItem() { Text = text };
        //    rv.Click += (_, _) => a();
        //    return rv;

        //}

        private TabPage CreateTabPage(Frame frame)
        {
            frame.Navigate(typeof(TabPage));
            return frame.Content as TabPage;
        }





        //        private void OnLoggedIn(object sender, EventArgs e)
        //        {
        //            IsLoggedIn = true;
        //            IsAuthorized = true;// IsLoggedIn && IdentityService.IsAuthorized();
        //            IsBusy = false;
        //        }

        //        private void OnLoggedOut(object sender, EventArgs e)
        //        {
        //            IsLoggedIn = false;
        //            IsAuthorized = false;
        //            CleanRestrictedPagesFromNavigationHistory();
        //            GoBackToLastUnrestrictedPage();
        //        }

        //        private void CleanRestrictedPagesFromNavigationHistory()
        //        {
        //            NavigationService.Frame.BackStack
        //.Where(b => Attribute.IsDefined(b.SourcePageType, typeof(Restricted)))
        //.ToList()
        //.ForEach(page => NavigationService.Frame.BackStack.Remove(page));
        //        }

        //        private void GoBackToLastUnrestrictedPage()
        //        {
        //            var currentPage = NavigationService.Frame.Content as Page;
        //            var isCurrentPageRestricted = Attribute.IsDefined(currentPage.GetType(), typeof(Restricted));
        //            if (isCurrentPageRestricted)
        //            {
        //                NavigationService.GoBack();
        //            }
        //        }

        //        private void OnUserProfile(object sender, RoutedEventArgs e)
        //        {
        //            if (IsLoggedIn)
        //            {
        //                OpenSettingsPage(sender,e);
        ////                NavigationService.Navigate<SettingsPage>();
        //            }
        //            //else
        //            //{
        //            //    IsBusy = true;
        //            //    var loginResult = await IdentityService.LoginAsync();
        //            //    if (loginResult != LoginResultType.Success)
        //            //    {
        //            //        await AuthenticationHelper.ShowLoginErrorAsync(loginResult);
        //            //        IsBusy = false;
        //            //    }
        //            //}
        //        }

        //private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        //{
        //    throw e.Exception;
        //}

        //private void OnCurrentPageCanGoBackChanged(object sender, bool currentPageCanGoBack)
        //    => IsBackEnabled = NavigationService.CanGoBack || currentPageCanGoBack;

        //private void Frame_Navigated(object sender, NavigationEventArgs e)
        //{
        //    IsBackEnabled = NavigationService.CanGoBack;
        //    if (e.SourcePageType == typeof(SettingsPage))
        //    {
        //        Selected = navigationView.SettingsItem as WinUI.NavigationViewItem;
        //        return;
        //    }

        //    var selectedItem = GetSelectedItem(navigationView.MenuItems, e.SourcePageType);
        //    if (selectedItem != null)
        //    {
        //        Selected = selectedItem;
        //    }
        //}


        //private WinUI.NavigationViewItem GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
        //{
        //    foreach (var item in menuItems.OfType<WinUI.NavigationViewItem>())
        //    {
        //        if (IsMenuItemForPageType(item, pageType))
        //        {
        //            return item;
        //        }

        //        var selectedChild = GetSelectedItem(item.MenuItems, pageType);
        //        if (selectedChild != null)
        //        {
        //            return selectedChild;
        //        }
        //    }

        //    return null;
        //}

        //private bool IsMenuItemForPageType(WinUI.NavigationViewItem menuItem, Type sourcePageType)
        //{
        //    var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
        //    return pageType == sourcePageType;
        //}

        //        private void OnItemInvoked(WinUI.NavigationView sender, WinUI.NavigationViewItemInvokedEventArgs args)
        //        {
        //            if (args.IsSettingsInvoked)
        //            {
        //                OpenSettingsPage(sender, args);
        ////                NavigationService.Navigate<SettingsPage>(null, args.RecommendedNavigationTransitionInfo);
        //                return;
        //            }

        //            //if (args.InvokedItemContainer is WinUI.NavigationViewItem selectedItem)
        //            //{
        //            //    var pageType = selectedItem.GetValue(NavHelper.NavigateToProperty) as Type;
        //            //    NavigationService.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
        //            //}
        //        }

        //private void OnBackRequested(WinUI.NavigationView sender, WinUI.NavigationViewBackRequestedEventArgs args)
        //{
        //    NavigationService.GoBack();
        //}

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, TypedEventHandler<KeyboardAccelerator, KeyboardAcceleratorInvokedEventArgs> OnKeyboardAcceleratorInvoked, VirtualKeyModifiers modifiers = VirtualKeyModifiers.None)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
            keyboardAccelerator.Modifiers = modifiers;
            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        //private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        //{
        //    var result = NavigationService.GoBack();
        //    args.Handled = result;
        //}



        public void TestPost(object o, RoutedEventArgs e)
        {
            Raiding.UpdateTSHome(true);

        }

        public void Refresh(object o, RoutedEventArgs e)
        {
            Refresh();

        }
        private static void RefreshWorldData()
        {
            Note.Show("Refresh world data");
            GetWorldInfo.Send();
            TileData.Ctor();

        }
        private void RefreshX(object sender, RightTappedRoutedEventArgs e)
        {
            RefreshWorldData();

        }
        private static void Refresh()
        {

            if (App.IsKeyPressedShift())
            {
                RefreshWorldData();
            }
            else
            {
                Note.Show("Refresh UI");
            }
            // fall through from shift-refresh.  Shift refresh does both

                foreach (var tab in UserTab.userTabs)
                {
                    tab.Refresh();
                }
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }


        private async void TestPost2(object sender, RoutedEventArgs e)
        {
        }
        private async void TestGoCity(object sender, RoutedEventArgs e)
        {
            await GetCity.Post(City.focus);

        }
        //private async void GetWorldInfo(object sender, RoutedEventArgs e)
        //{
        //    await RestAPI.getWorldInfo.Post();

        //}
        //private async void GetPPDT(object sender, RoutedEventArgs e)
        //{
        //    await JSClient.GetCitylistOverview();
        //}

        static string[] buildings = { "forester", "cottage", "storehouse", "quarry", "hideaway", "farmhouse", "cityguardhouse", "barracks", "mine", "trainingground", "marketplace", "townhouse", "sawmill", "stable", "stonemason", "mage_tower", "windmill", "temple", "smelter", "blacksmith", "castle", "port", "port", "port", "shipyard", "shipyard", "shipyard", "townhall", "castle" };
        const short bidTownHall = 455;
        static short[] bidMap = new short[] { 448, 446, 464, 461, 479, 447, 504, 445, 465, 483, 449, 481, 460, 466, 462, 500, 463, 482, 477, 502, 467, 488, 489, 490, 491, 496, 498, bidTownHall, 467 };


        static DateTimeOffset flyoutCreatedTime;
        public static (int x, int y) webclientSpan;

        private async void ShowBuildings(object sender, RoutedEventArgs e)
        {
            try
            {
                if (City.build == 0)
                    return;
                // Assert(false);
                await GetCity.Post(City.build, (jse, city) =>
                {

                    List<BuildingCount> bd = new List<BuildingCount>();
                    int bCount = 0;
                    var bdd = new Dictionary<string, int>();

                    if (jse.TryGetProperty("bd", out var eBd))
                    {
                        foreach (var bdi in eBd.EnumerateArray())
                        {
                            var bid = bdi.GetAsInt("bid");
                            if (bid == bidTownHall)
                                continue;
                            var bi = bidMap.IndexOf((short)bid);
                            if (bi == -1)
                                continue;

                            var s = buildings[bi];
                            if (!bdd.TryGetValue(s, out var counter))
                            {
                                bdd.Add(s, 0);
                                counter = 0;
                            }
                            bdd[s] = counter + 1;
                            ++bCount;
                        }
                    }
                    foreach (var i in bdd)
                    {
                        bd.Add(new BuildingCount() { count = i.Value, image = JSClient.GetImage("images/city/buildings/icons/", $"{i.Key}.png") });
                    }
                    bd.Add(new BuildingCount() { count = bCount, image = ImageHelper.FromImages("townhall.png") });

                    //         

                    flyoutCreatedTime = DateTimeOffset.Now;

                    //                    var button = sender as Button;
                    // button.Focus(FocusState.Programmatic);
                    App.DispatchOnUIThread(() =>
                    {
                        buildingList.Width = Double.NaN;
                        buildingList.Height = Double.NaN;
                        buildingList.ItemsSource = bd;
                        buildingList.UpdateLayout();
                    });
                });
                //   var flyout = FlyoutBase.GetAttachedFlyout(button);
                //  flyout.OverlayInputPassThroughElement = shellPage;
                //    flyout.XamlRoot = shellFrame.XamlRoot;
                //    flyout.ShowMode = FlyoutShowMode.TransientWithDismissOnPointerMoveAway;
                //  Log($"{button.Tr.ToString()}");
                //  var c = button.CenterPoint;
                //      flyout.ShowAt(button, new FlyoutShowOptions() { Placement = FlyoutPlacementMode.Full, ShowMode = FlyoutShowMode.TransientWithDismissOnPointerMoveAway });
                //  flyout.ShowAt(button, new FlyoutShowOptions() { Placement = FlyoutPlacementMode.Right, ShowMode = FlyoutShowMode.Auto });

                //           buildingList.Focus(FocusState.Programmatic);
                //            buildingList.CapturePointer(e.Pointer);
                //   buildingList.Focus(FocusState.Programmatic);
                //  buildingList.Height = ((bd.Count + 5) / 6) * 60+10;
                //  buildingList.DesiredSize
                //  FlyoutBase.ShowAttachedFlyout(button);//.OverlayInputPassThroughElement = button;

                //           buildingList.UpdateLayout();
                //     button.Flyout.with
                //var mouseC = e.GetCurrentPoint(null).Position;
                //const float spawn = 20.0f;
                //      button.Focus(FocusState.Programmatic);

                //            var button.Flyout.Update = new Rect(mouseC.X - spawn, mouseC.Y - spawn, mouseC.X + spawn, mouseC.Y + spawn);

                //  var avoid = new Rect(mouseC.X - spawn, mouseC.Y - spawn, mouseC.X + spawn, mouseC.Y + spawn);
                //  button.Flyout.ShowAt(button, new FlyoutShowOptions() { Placement=FlyoutPlacementMode.Full, ShowMode=FlyoutShowMode.Transient }); // ,ExclusionRect=avoid });

            }
            catch (Exception ex)
            {
                Log(ex);
            }



        }
        public static void ShowTipRefresh()
        {
            if (TipsSeen.instance.refresh==false)
            {
                TipsSeen.instance.refresh = true;
                instance.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => instance.RefreshTip.IsOpen = true);
            }
        }
        //private void DoNothing(object sender, RoutedEventArgs e)
        //{

        //}

        private void FlyoutClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            // Log("Why is this closing?");
        }

        private void GridLostMouse(object sender, PointerRoutedEventArgs e)
        {

            try
            {
                //   Log($"grid lost: {GetName(sender)} {GetName(e.OriginalSource)}");
                //  var me = sender as GridView;
                //  var button = buildingsButton;
                //  var flyout = FlyoutBase.GetAttachedFlyout(button);
                //  if((DateTime.Now - flyoutCreatedTime).TotalSeconds > 0.25f )
                //     flyout.Hide();


            }
            catch (Exception)
            {

            }
        }

        //      public static string GetName(object o )
        //{

        //          return o is FrameworkElement e ? $"{ e.Name }{e.GetType() }"  : $"{ o.ToString()}{o.GetType() }";
        //}
        //private void ShellPointerEntered(object sender, PointerRoutedEventArgs e)
        //{
        //          Log($"pointer enter: {GetName(sender)} {GetName(e.OriginalSource)}");
        //      }

        //private void ShellPointerExited(object sender, PointerRoutedEventArgs e)
        //{
        //          Log($"pointer exit: {GetName(sender)} {GetName(e.OriginalSource)}");


        //      }
        //       private void TroopOverview(object sender, RoutedEventArgs e)
        //       {
        //           RestAPI.troopsOverview.Post();
        //       }

        //       private void TestRaid(object sender, RoutedEventArgs e)
        //       {
        ////          ScanDungeons.Post();
        //       }


        //       private void GetIncomingOverview(object sender, RoutedEventArgs e)
        //       {
        //           IncomingOverview.Process(false);
        //       }

        //private void logFlyoutButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!logTip.IsOpen)
        //    {
        //  //      chatTabFrame.Navigate(typeof(TabPage));

        //        logTip.IsOpen = true;
        //    }
        //    else
        //    {
        //    //    chatTabFrame.Navigate(typeof(Page));
        //        logTip.IsOpen = false;
        //    }
        //}


        private void TipTest(object sender, RoutedEventArgs e)
        {
            ShowTipRefresh();
        }

        private async void ShowSettings(object sender, RoutedEventArgs e)
        {
            AppWindow newWindow = await AppWindow.TryCreateAsync();
            newWindow.Title = "Settings";
            var newPage = new SettingsPage();

            ElementCompositionPreview.SetAppWindowContent(newWindow, newPage);
            await newWindow.TryShowAsync();

        }


        public static ComboBox CityListBox => instance.cityListBox;
        //  private DumbCollection<CityList> cityListSelections => CityList.selections;
        private void CityListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Any())
            {
                var newSel = e.AddedItems?.FirstOrDefault();
                var priorSel = e.RemovedItems?.FirstOrDefault();
                if (newSel != priorSel && priorSel != null)
                {
                    //     Log("City Sel changed");
                    CityList.SelectedChange();
                }
            }
        }
        private void CityBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Any())
            {
                var newSel = e.AddedItems?.FirstOrDefault() as City;
                if (newSel.cid != City.build)
                {
                    newSel.SetBuild();
                    JSClient.ChangeCity(newSel.cid,false);
                    NavStack.Push(newSel.cid);

                }
            }
        }

        private void ChangeCityClick(int delta)
        {
            var items = ShellPage.instance.cityBox.ItemsSource as City[];
            City newSel;
            if (items.Length <= 1)
            {
                if (items.Length == 0)
                    return;
                newSel = items.First();
            }
            else
            {
                int id = Array.IndexOf(items,City.GetBuild()) + delta;
                if (id < 0)
                    id += items.Length;
                if (id >= items.Length)
                    id -= items.Length;

                newSel = items[id];
            }
            newSel.SetBuild();
            JSClient.ChangeCity(newSel.cid,false);
            NavStack.Push(newSel.cid);
        }

        private void PriorCityClick(object sender, RoutedEventArgs e)
        {
            ChangeCityClick(-1);
        }

        private void NextCityClick(object sender, RoutedEventArgs e)
        {
            ChangeCityClick(+1);
        }


        private void BackRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var menu = new MenuFlyout();
            bool any = false;
            for(int i=1;i<25;++i)
            {
                var str = NavStack.GetSpotName(-i);
                if (str == null)
                    break;
                any = true;
                menu.Items.Add(App.CreateMenuItem(str, NavStack.instance,-i));
            }
            if(!any)
                menu.Items.Add(App.CreateMenuItem("no more :(", ()=>{ }));

            menu.ShowAt(sender as FrameworkElement);

        }

        private void ForwardRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var menu = new MenuFlyout();
            bool any = false;
            for (int i = 1; i < 25; ++i)
            {
                var str = NavStack.GetSpotName(i);
                if (str == null)
                    break;
                any = true;
                menu.Items.Add(App.CreateMenuItem(str, NavStack.instance, i));
            }
            if (!any)
                menu.Items.Add(App.CreateMenuItem("this is the most recent :(", () => { }));

            menu.ShowAt(sender as FrameworkElement);
        }



        private void coords_KeyDown(object sender, KeyRoutedEventArgs e)
        {
                var str = sender as TextBox;
            Assert(str != null);
            if (str != null)
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                        var cid = str.Text.FromCoordinate();
                        if (cid != 0)
                        {
                            JSClient.ChangeCity(cid, false);
                            NavStack.Push(cid);

                        }

                    
                }

            }
        }

       
    }
}
