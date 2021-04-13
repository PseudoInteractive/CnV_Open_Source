using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


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
using Windows.UI.Input;
using Windows.Graphics.Display;
using Microsoft.Toolkit.Uwp.UI;
using System.Numerics;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Core.Preview;

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
		public const int canvasZDefault = 11;
		public const int canvasZBack = 0;
		//private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
		//private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);
		//private readonly KeyboardAccelerator _forwardKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoForward);
		static public ShellPage instance;
		private bool _isBackEnabled;
		private WinUI.NavigationViewItem _selected;
		private bool _isBusy;
		private bool _isLoggedIn;
		private bool _isAuthorized;

		public static TextBlock gridTip;

		static DateTime workStarted;
		static List<string> workQueue = new List<string>();
		public static string WorkStart(string desc)
		{
			App.DispatchOnUIThreadSneaky(() =>
		   {
			   if (!workQueue.Any())
			   {
				   instance.progress.IsActive = true;
				   workStarted = DateTime.UtcNow;
				   instance.work.Text = desc;
				   instance.work.Visibility = Visibility.Visible;
			   }
			   workQueue.Add(desc);

		   });
			return desc;
		}
		// Todo:  Queue updates with tasks
		public static void WorkUpdate(string desc)
		{
			App.DispatchOnUIThreadSneaky(() =>
			{
				instance.work.Text = desc;

			});
		}
		public static void WorkEnd(string desc)
		{
			App.DispatchOnUIThreadSneaky(() =>
			{
				if (!workQueue.Any())
				{
					Log("End end called too often");
				}
				else
				{
					if (DateTime.UtcNow - workStarted > TimeSpan.FromMinutes(5))
					{
						Log("rogue work item");
						workQueue.Clear();
					}
					else
					{
						workQueue.Remove(desc);
					}
				}
				if (!workQueue.Any())
				{

					instance.progress.IsActive = false;
					instance.work.Visibility = Visibility.Collapsed;
				}
				else
				{
					workStarted = DateTime.UtcNow;
					instance.work.Text = workQueue.First();
				}
			});
		}
		public class WorkScope : IDisposable
		{
			string task;
			// passing null results in a Scope with no effect
			public WorkScope(string _task)
			{
				if (_task != null)
					this.task = WorkStart(_task);

			}

			public void Dispose()
			{
				if (task != null)
					WorkEnd(task);
			}
		}

		//    private IdentityService IdentityService => Singleton<IdentityService>.Instance;

		//    private UserDataService UserDataService => Singleton<UserDataService>.Instance;

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
		}

		public static void SetHeaderText(string text)
		{
			//    if(instance!=null && instance.navigationView!=null)
			//        instance.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => instance.status.Label=text );
		}
		public static bool isHitTestVisible=true;
		public static bool canvasVisible;
		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += App.App_CloseRequested; ;

			CityBuild.Initialize();
		//	Grid.SetColumn(webView, 0);
			Grid.SetRow(CityBuild.instance, 1);
			Grid.SetRowSpan(CityBuild.instance, 5);
			Grid.SetColumnSpan(CityBuild.instance, 1);
			Canvas.SetZIndex(CityBuild.instance, 13);
			var c = CreateCanvasControl();

			//  canvas.ContextFlyout = CityFlyout;
			grid.Children.Add(c.canvas);
			//		grid.Children.Add(c.hitTest);
			Grid.SetColumn(c.canvas, 1);
			//Grid.SetColumn(c.hitTest, 0);
			Grid.SetRow(c.canvas, 1);
			//Grid.SetRow(c.hitTest, 1);
			Grid.SetRowSpan(c.canvas, 4);
			//Grid.SetRowSpan(c.hitTest, 5);
			Grid.SetColumnSpan(c.canvas, 1);
			//Grid.SetColumnSpan(c.hitTest, 2);
			c.canvas.BorderThickness = new Thickness(0, 0, 0, 0);
			//		c.hitTest.BorderThickness = new Thickness(0, 0, 0, 0);
			Canvas.SetZIndex(c.canvas, 12);
			//	Canvas.SetZIndex(c.hitTest, 13);
			//           Task.Run(SetupCanvasInput);//           Task.Run(SetupCanvasInput);
			//	Placement.SizeChanged += Placement_SizeChanged;
			//   var img = new Image() { Opacity=0.5f, Source = new SvgImageSource(new Uri($"ms-appx:///Assets/world20.svg")),IsHitTestVisible=false };
			//		Placement.LayoutUpdated += Placement_LayoutUpdated;
			//   grid.Children.Add(img);

			//   Grid.SetRowSpan(img, 4);
			//   Grid.SetColumnSpan(img, 4);
			//   Canvas.SetZIndex(img, 12);
			var webView = JSClient.Initialize(grid);
			//           foreach (var i in webView.KeyboardAccelerators)
			//              i.IsEnabled = false;
			//            webView.AllowFocusOnInteraction = false;
			//  c.hitTest.Margin= webView.Margin = new Thickness(0, 0, 11, 0);
			grid.Children.Add(webView);

			//c.hitTest.Fill = JSClient.webViewBrush;
			//				var visual = ElementCompositionPreview.GetElementVisual(c.canvas);
			//			var webVisual = ElementCompositionPreview.GetElementVisual(view);
			//	var sprite = visual.Compositor.CreateSpriteVisual();//	var sprite = visual.Compositor.CreateSpriteVisual();

			//	sprite.Brush = 
			//		ElementCompositionPreview.SetElementChildVisual(visual,sprite);
			//            grid.Background = null;


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
			Canvas.SetZIndex(webView, 10);

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
			//            KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack,NavStack.BackInvoked));

			KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Right, NavStack.ForwardInvoked, VirtualKeyModifiers.Menu));
			//            KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoForward, NavStack.ForwardInvoked));

			KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.F5, Refresh_Invoked));
			KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.F2, LayoutAccelerator_Invoked));
			KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.F3, LayoutAccelerator_Invoked));
			KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.F4, LayoutAccelerator_Invoked));
			IsLoggedIn = true;// IdentityService.IsLoggedIn();
			IsAuthorized = true;// IsLoggedIn && IdentityService.IsAuthorized();
								// grid.hor
			/// we pass this as an argument to let the page know that it is a programmatic navigation
			// Services.NavigationService.Navigate<Views.DefenseHistoryTab>(this);
			//   ChatTab.Ctor();
			{
				ChatTab.Ctor();
				ChatTab.tabPage = CreateTabPage(chatTabFrame).AddChatTabs();


				UserTab.InitUserTabs();
				{
					var tabPage = CreateTabPage(shellFrame);
					TabPage.mainTabs = tabPage;
					tabPage.AddTab(MainPage.instance, true);
					tabPage.AddTab(BuildTab.instance, false);
					tabPage.AddTab(QueueTab.instance, false);
					tabPage.AddTab(HeatTab.instance, false);
		
					// gridTip=new TextBlock();
					// tabPage.Tabs.TabStripFooter = gridTip;
					//    tabPage.AddTab(IncomingTab.instance, false);

					//   tabPage.AddTab(DefenseHistoryTab.instance, false);

					//    tabPage.AddTab(AttackTab.instance, false);
					//    tabPage.AddTab(DonationTab.instance, false);
					//    tabPage.AddTab(BossTab.instance, false);

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
		
			SystemNavigationManager.GetForCurrentView().BackRequested += ShellPage_BackRequested;
			//            PointerPressed+=  PointerPressedCB;
			//   HomeButtonTip.IsOpen = true;

			App.SetupCoreWindowInputHooks();
			var displayInformation = DisplayInformation.GetForCurrentView();
			var screenSize = new Size(displayInformation.ScreenWidthInRawPixels,
									  displayInformation.ScreenHeightInRawPixels);
			ShellPage.webclientSpan.x = (screenSize.Width * .715625f).RoundToInt();
			ShellPage.webclientSpan.y = (screenSize.Height * 0.89236111111111116f).RoundToInt();

			AGame.Create(canvas);
			Task.Delay(500).ContinueWith((_) => App.DispatchOnUIThreadLow(() =>
			{
				var sz = canvas.ActualSize;
				AGame. SetClientSpan(sz.X,sz.Y);
				//ShellPage.canvas.IsHitTestVisible = false;
				//ShellPage.canvas.Visibility = Visibility.Collapsed;
			}));

		}

		//void GetPlacement()
		//{
		//	var g = grid;
		//	var columns = grid.ColumnDefinitions;
		//	var rows = grid.RowDefinitions;
		//	Point offset = new Point();
		//	Size size = new Size();
		//	offset.X = columns[0].ActualWidth;
		//	offset.Y = rows[0].ActualHeight;
		//	for (int i = 1; i < 1 + 4; ++i)
		//		size.Width += columns[i].ActualWidth;
		//	for (int i = 1; i < 1 + 4; ++i)
		//		size.Height += rows[i].ActualHeight;
		//	Canvas.SetLeft(canvas, offset.X);
		//	Canvas.SetTop(canvas, offset.Y);

		//	canvas.Width= size.Width;
		//	canvas.Height = size.Height;

		//}
		//private void Placement_LayoutUpdated(object sender, object e)
		//{

		//	var sz = new Vector2((float)Placement.ActualWidth, (float)Placement.ActualHeight) - new Vector2((float)Canvas.GetLeft(canvas), (float)Canvas.GetTop(canvas));
		//	// measure

		//	Log("layout");
		//	AGame.SetClientSpan(sz);

		//	canvas.Width = sz.X;
		//	canvas.Height = sz.Y;
		//}

		//private void Placement_SizeChanged(object sender, SizeChangedEventArgs e)
		//{
		//	var sz = new Vector2((float)e.NewSize.Width, (float)e.NewSize.Height) -new Vector2((float)Canvas.GetLeft(canvas), (float)Canvas.GetTop(canvas) );

		//	Log("Size");
		//	AGame.SetClientSpan(sz);

		//	canvas.Width = sz.X;
		//	canvas.Height = sz.Y;

		//}

		//private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
		//{
		//    // Get the size of the caption controls area and back button 
		//    // (returned in logical pixels), and move your content around as necessary.
		//    LeftPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayLeftInset);
		//    RightPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayRightInset);
		//   // TitleBarButton.Margin = new Thickness(0, 0, coreTitleBar.SystemOverlayRightInset, 0);

		//    // Update title bar control size as needed to account for system size changes.
		//    AppTitleBar.Height = coreTitleBar.Height;
		//}

		private void PointerPressedCB(object sender, PointerRoutedEventArgs e)
		{
			var prop = e.GetCurrentPoint(this).Properties;
			switch (prop.PointerUpdateKind)
			{
				case PointerUpdateKind.XButton1Pressed:
					NavStack.Back(true);
					Log("XButton1");
					e.Handled = true;
					break;
				case PointerUpdateKind.XButton2Pressed:
					NavStack.Forward(true);
					Log("XButton2");
					e.Handled = true;
					break;
			}

		}
		private void ShellPage_BackRequested(object sender, BackRequestedEventArgs e)
		{
			Log("Back!!");
			NavStack.Back(true);
			//e.Handled = true;
		}

		private void Refresh_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
		{
			Refresh();

			Note.Show("Refresh");
			//          args.Handled = true;
		}
		private void LayoutAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
		{
			switch(args.KeyboardAccelerator.Key)
			{
				case VirtualKey.F2:
					SetLayout(0);break;
				case VirtualKey.F3:
					SetLayout(1); break;
				case VirtualKey.F4:
					SetLayout(2); break;
			}
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



		//public void TestPost(object o, RoutedEventArgs e)
		//{
		//    Raiding.UpdateTS(true);

		//}

		public void Refresh(object o, RoutedEventArgs e)
		{
			Refresh();

		}
		private async static void RefreshWorldData()
		{
			using var work = new WorkScope("Refresh..");
			if (World.completed) 
				GetWorldInfo.Send();
			await TileData.Ctor(false);
		}
		public static void RefreshAndReloadWorldData()
		{
			using var work = new WorkScope("Refresh..");
			if (World.completed) 
				GetWorldInfo.Send();
			TileData.Ctor(true);
		}
		public void RefreshX(object sender, RightTappedRoutedEventArgs e)
		{
			Note.Show("Refresh All");
			RefreshWorldData();
			RefreshTabs();

		}

		private static void RefreshTabs()
		{

			// fall through from shift-refresh.  Shift refresh does both
			City.UpdateSenatorInfo();
			foreach (var tab in UserTab.userTabs)
			{
				tab.Refresh();
			}

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
			RefreshTabs();

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

		//static string[] buildings = { "forester", "cottage", "storehouse", "quarry", "hideaway", "farmhouse", "cityguardhouse", "barracks", "mine", "trainingground", "marketplace", "townhouse", "sawmill", "stable", "stonemason", "mage_tower", "windmill", "temple", "smelter", "blacksmith", "castle", "port", "port", "port", "shipyard", "shipyard", "shipyard", "townhall", "castle" };
		
		//static short[] bidMap = new short[] { 448, 446, 464, 461, 479, 447, 504, 445, 465, 483, 449, 481, 460, 466, 462, 500, 463, 482, 477, 502, 467, 488, 489, 490, 491, 496, 498, bidTownHall, 467 };


		public static (int x, int y) webclientSpan;

		private async void ShowBuildings(object sender, RoutedEventArgs e)
		{
			try
			{
				var build = City.GetBuild();
				int bCount = 0;
				var bdd = new Dictionary<int, int>();
				
				void ProcessBuilding(BuildingDef bd, bool add = true)
				{
					if (bd.bid == City.bidTownHall || bd.bid == City.bidWall)
						return;
					
					var id = bd.Proto;
					if (!bdd.TryGetValue(id, out var counter))
					{
						bdd.Add(id, 0);
						counter = 0;
					}
					if (add)
					{
						bdd[id] = counter + 1;
						if(!bd.isTower)
							++bCount;
					}
					else
					{
						bdd[id] = counter - 1;
						if (!bd.isTower)
							--bCount;
					}
				}

				foreach (var bdi in CityBuild.postQueueBuildings)
				{
					var id = bdi.id;
					if (id == 0 || !bdi.isBuilding )
						continue;
					var bd = bdi.def;
					ProcessBuilding(bd);
				}


				{
					var bd = new List<BuildingCount>();
					foreach (var i in bdd)
					{
						if (i.Value > 0)
						{
							var bdf = BuildingDef.prototypes[i.Key];
							bd.Add(new BuildingCount() { count = i.Value, brush = CityBuild.BuildingBrush(bdf.bid, 0.5f) });
						}
					}
					bd.Add(new BuildingCount() { count = bCount, brush = CityBuild.BuildingBrush(City.bidTownHall, 0.5f) });

					//         
					//                    var button = sender as Button;
					// button.Focus(FocusState.Programmatic);
					App.DispatchOnUIThread(() =>
					{
						buildingList.Width = Double.NaN;
						buildingList.Height = Double.NaN;
						buildingList.ItemsSource = bd;
						buildingList.UpdateLayout();
					});
				}
			}
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

			
			catch (Exception ex)
			{
				Log(ex);
			}



		}
		public static void ShowTipRefresh()
		{
			//if (TipsSeen.instance.refresh==false)
			//{
			//    TipsSeen.instance.refresh = true;
			//    instance.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => instance.RefreshTip.IsOpen = true);
			//}
			//else if(TipsSeen.instance.chat0==false)
			//{
			//    TipsSeen.instance.chat0 = true;
			//    instance.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => instance.ChatTip0.IsOpen = true);

			//}
			//else if (TipsSeen.instance.chat1 == false)
			//{
			//    TipsSeen.instance.chat1 = true;
			//    instance.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => instance.ChatTip1.IsOpen = true);


			//}
			//else if (TipsSeen.instance.chat2 == false)
			//{
			//    TipsSeen.instance.chat2 = true;
			//    instance.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => instance.ChatTip2.IsOpen = true);

			//}
		}

		//private void DoNothing(object sender, RoutedEventArgs e)
		//{

		//}

		private void FlyoutClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
		{
			// Log("Why is this closing?");
		}

		//private void GridLostMouse(object sender, PointerRoutedEventArgs e)
		//{

		//    try
		//    {
		//        //   Log($"grid lost: {GetName(sender)} {GetName(e.OriginalSource)}");
		//        //  var me = sender as GridView;
		//        //  var button = buildingsButton;
		//        //  var flyout = FlyoutBase.GetAttachedFlyout(button);
		//        //  if((DateTime.Now - flyoutCreatedTime).TotalSeconds > 0.25f )
		//        //     flyout.Hide();


		//    }
		//    catch (Exception)
		//    {

		//    }
		//}

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
		

		private void ShowSettings(object sender, RoutedEventArgs e)
		{
			SettingsPage.Show();
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
			var sel = cityBox.SelectedItem as City;
			if (sel != null && sel.cid != City.build)
			{
				JSClient.ChangeCity(sel.cid, false);
				NavStack.Push(sel.cid);
			}
		}

		public void ChangeCityClick(int delta)
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
				int id = Array.IndexOf(items, City.GetBuild()) + delta;
				if (id < 0)
					id += items.Length;
				if (id >= items.Length)
					id -= items.Length;

				newSel = items[id];
			}
			//newSel.SetBuild(true);
			JSClient.ChangeCity(newSel.cid, false);
			//	ElementSoundPlayer.Play(delta > 0 ? ElementSoundKind.MoveNext : ElementSoundKind.MovePrevious);
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
			for (int i = 1; i < 25; ++i)
			{
				var str = NavStack.GetSpotName(-i);
				if (str == null)
					break;
				any = true;
				menu.Items.Add(AApp.CreateMenuItem(str, NavStack.instance, -i));
			}
			if (!any)
				menu.Items.Add(AApp.CreateMenuItem("no more :(", () => { }));

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
				menu.Items.Add(AApp.CreateMenuItem(str, NavStack.instance, i));
			}
			if (!any)
				menu.Items.Add(AApp.CreateMenuItem("this is the most recent :(", () => { }));

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
					if (cid > 0)
					{
						NavStack.Push(cid);
						SpotTab.TouchSpot(cid, App.keyModifiers);
						JSClient.ChangeCity(cid, false);


					}


				}

			}
		}

		private void HomeClick(object sender, RoutedEventArgs e)
		{

			if (Spot.focus == 0)
				return;
			if (Spot.focus.BringCidIntoWorldView(false, false) && City.IsBuild(Spot.focus)) // first just focus
			{
				return;
			}
			Spot.ProcessCoordClick(Spot.focus, false, App.keyModifiers, true); // then normal click

		}

		private void HomeRightTapped(object sender, RightTappedRoutedEventArgs e)
		{

			var ui = sender as UIElement;
			Spot.GetFocus()?.ShowContextMenu(ui, e.GetPosition(ui));

		}

		private void BuildHomeClick(object sender, RoutedEventArgs e)
		{
			if (City.build == 0)
				return;

			Spot.ProcessCoordClick(City.build, false, App.keyModifiers, true); // then normal click
		}

		private void BuildHomeRightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			var ui = sender as UIElement;
			City.GetBuild()?.ShowContextMenu(ui, e.GetPosition(ui));

		}

		private void CityListSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
		{
			var text = args.Text.ToLower();
			var items = CityList.selections;
			foreach (var it in items)
			{
				// its good
				if (it.name == text)
					return;
			}
			args.Handled = true;
			foreach (var it in items)
			{
				if (it.name.ToLower().StartsWith(text))
				{
					sender.Text = it.name;
					sender.SelectedItem = it;
					return;
				}
			}
			// try contains
			foreach (var it in items)
			{
				if (it.name.ToLower().Contains(text))
				{
					sender.Text = it.name;
					sender.SelectedItem = it;
					return;
				}
			}
			// todo!

		}

		private void CitySubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
		{
			var text = args.Text.ToLower();
			var items = City.gridCitySource;
			foreach (var it in items)
			{
				// its good
				if (it.nameAndRemarks == text)
					return;
			}
			args.Handled = true;
			foreach (var it in items)
			{
				if (it.nameAndRemarks.ToLower().StartsWith(text))
				{
					sender.Text = it.nameAndRemarks;
					sender.SelectedItem = it;
					return;
				}
			}
			// try contains
			foreach (var it in items)
			{
				if (it.nameAndRemarks.ToLower().Contains(text))
				{
					sender.Text = it.nameAndRemarks;
					sender.SelectedItem = it;
					return;
				}
			}
			// todo!
		}

		private void windowLayout_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (MainPage.instance != null)
			{
				var viewToggle = windowLayout.SelectedIndex;
				SetLayout(viewToggle);
			}
		}

		private void SetLayout(int viewToggle)
		{
			var raidInfoVisible = true;
			switch (viewToggle)
			{
				case 0:
			//		grid.ColumnDefinitions[0].Width = new GridLength(410, GridUnitType.Pixel);
					grid.ColumnDefinitions[1].Width = new GridLength(2, GridUnitType.Star);
					grid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
				//	JSClient.view.Scale = new Vector3(1.0f, 1.0f, 1.0f);
					break;
				case 1:
				//	grid.ColumnDefinitions[0].Width = new GridLength(410*3/4, GridUnitType.Pixel);
					grid.ColumnDefinitions[1].Width = new GridLength(2, GridUnitType.Star);
					grid.ColumnDefinitions[2].Width = new GridLength(160);
					raidInfoVisible = false;
				//	JSClient.view.Scale = new Vector3(0.75f, 0.75f, 1.0f);
					break;
				case 2:
				//	grid.ColumnDefinitions[0].Width = new GridLength(410 * 3 / 4, GridUnitType.Pixel);
					grid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
					grid.ColumnDefinitions[2].Width = new GridLength(2, GridUnitType.Star);
				//	JSClient.view.Scale = new Vector3(0.75f, 0.75f, 1.0f);
					break;

			}

			MainPage.ToggleInfoBoxes(raidInfoVisible);
			City.gridCitySource.NotifyReset();
		}

		private void webFocus_Click(object sender, RoutedEventArgs e)
		{
			var hasFocus = !webviewHasFocus;

			CityBuild.instance.Visibility = hasFocus ? Visibility.Collapsed : Visibility.Visible;
			canvasVisible = !hasFocus;
			isHitTestVisible = !hasFocus;
			SetWebViewHasFocus(hasFocus);

		}


		private void FriendListSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
		{

		}

		private void friendListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(e.AddedItems.Any())
			{
				var s = e.AddedItems[0] as string;
				var friend = Array.Find(PlayerPresence.all,f => f.name == s);
				JSClient.SetPlayer(friend.pid,friend.token, friend.cookies,friend.cid, friend.name);

			}
		}

		private void chatResizeTapped(object sender, TappedRoutedEventArgs e)
		{
			var flyout = new MenuFlyout();
			flyout.AddItem("Tall", () => chatGrid.RowDefinitions[1].Height = new GridLength(chatGrid.ActualHeight*0.875f) );
			flyout.AddItem("Medium", () => chatGrid.RowDefinitions[1].Height = new GridLength(chatGrid.ActualHeight *(1.0f/3.0f)) );
			flyout.AddItem("Small", () => chatGrid.RowDefinitions[1].Height = new GridLength(chatGrid.ActualHeight*(1.0f/32.0f)));
			flyout.ShowAt(chatGrid, e.GetPosition(chatGrid));
		}

	
	}
}
