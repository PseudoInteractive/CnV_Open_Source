using CnV.Game;
using CnV.Helpers;

using CnV.Services;

using Cysharp.Text;

using DiscordCnV;

using CommunityToolkit.WinUI;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.System;
//using Windows.UI.Core;
//using Windows.UI.Core.Preview;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System.Net.Http;
using Microsoft.UI.Xaml.Media;
using static CnV.Debug;

using WinUI = Microsoft.UI.Xaml.Controls;
using CommunityToolkit.WinUI.UI.Controls;
using System.Reflection;
using CommunityToolkit.WinUI.Helpers;
using System.Collections.ObjectModel;
using CnV;
namespace CnV.Views
{
	using Game;

	using Helpers;
	// using PInvoke
	using Services;

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
		public const  int            canvasZDefault = 11;
		public const  int            canvasZBack    = 0;
		public static SwapChainPanel canvas => GameClient.canvas;
		public int layout
		{
			get => Settings.layout;
			set
			{
				if (Settings.layout != value)
				{
					Settings.layout = value;
					updateHtmlOffsets.Go(true);
				}
			}
		}

		class LayoutItem
		{
			public string name { get; set; }
			public int    id   { get; set; }

		}
		string[] layoutOptions =  System.Linq.Enumerable.Range(0, Settings.layoutOffsets.Length)
			.Select(a => $"Layout {a}").ToArray();

		//private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
		//private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);
		//private readonly KeyboardAccelerator _forwardKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoForward);
		static public ShellPage? instance;

		//	internal void AddHandler(object pointerMovedEvent, PointerEventHandler pointerEventHandler, bool v) => throw new NotImplementedException();

		private bool                     _isBackEnabled;
		private WinUI.NavigationViewItem _selected;
		private bool                     _isBusy;
		private bool                     _isLoggedIn;
		private bool                     _isAuthorized;

		public static float     webZoomLast; // for lazy setting of HTML zoom
		public static TextBlock gridTip;

		int GridRowIndex( RowDefinition row, int offset  )
		{
			return grid.RowDefinitions.IndexOf(row)+offset;
		}


		//		public static ScrollViewer webView;

		private static          DateTime     workStarted;
		private static readonly List<string> workQueue = new List<string>();

		//	protected override void OnKeyDown(KeyRoutedEventArgs e) => Trace($"Key: {e.Key} {e.OriginalKey} {e.OriginalSource.ToString()}");
//		protected override void OnPreviewKeyDown(KeyRoutedEventArgs e) => Trace($"KeyP: {e.Key} {e.OriginalKey} {e.OriginalSource.ToString()}");
		public static void WorkStart(string desc)
		{
			AppS.DispatchOnUIThreadLow(() =>
										{
											if (!workQueue.Any())
											{
												instance.progress.IsActive = true;
												// FIX
												workStarted              = DateTime.UtcNow;
												instance.work.Text       = desc;
												instance.work.Visibility = Visibility.Visible;
											}

											workQueue.Add(desc);
										});
		}

		// Todo:  Queue updates with tasks
		public static void WorkUpdate(string desc)
		{

			AppS.DispatchOnUIThreadLow(() => { instance.work.Text = desc; });
		}

		public static void WorkEnd(string desc)
		{
			AppS.DispatchOnUIThreadLow(() =>
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

												// FIX
												instance.work.Visibility = Visibility.Collapsed;
											}
											else
											{
												workStarted        = DateTime.UtcNow;
												instance.work.Text = workQueue.First();
											}
										});
		}



		// private IdentityService IdentityService => Singleton<IdentityService>.Instance;

		// private UserDataService UserDataService => Singleton<UserDataService>.Instance;
		public ObservableCollection<string> inAppNotes= new();
		//		public static InAppNotification inAppNote => instance.InAppNote;
		public string noteText = string.Empty;

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
			RequestedTheme = ElementTheme.Dark; // default theme

		}
//		public static bool rightTabsVisible => Settings.layout>=Layout.c;
//		public static bool htmlVisible => Settings.layout is not (Layout.l1 or  Layout.r2 or Layout.r1);

		//public static void SetHeaderText(string text)
		//{
		//	// if(instance!=null && instance.navigationView!=null)
		//	// instance.Dispatcher.RunAsync(DispatcherQueuePriority.Low, () =>
		//	// instance.status.Label=text );
		//}

		public static bool isHitTestVisible => !ShellPage.isOverPopup && !forceAllowWebFocus && canvasVisible;

		//public static bool _isHitTestVisible;
		public static bool canvasVisible;
		//public static bool isFocused => isHitTestVisible && AppS.isForeground;

		private async void OnLoaded(object sender, RoutedEventArgs e)
		{
			try
			{
				CnVServer.CitySwitch = CnVClient.CitySwitch;
				GameClient.canvas    = _canvas;

				var signinTask = Task.Run(CnVSignin.Go);

				Note.Init();
				CityUI.Init();
				NavStack.Init();
				// hook up delegates
				WorkScope.Start  = WorkStart;
				WorkScope.End    = WorkEnd;
				WorkScope.Update = WorkUpdate;
				//		SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += App.App_CloseRequested; ;
				//typeof(Telerik.UI.Xaml.Controls.RadDataForm).Assembly.GetType("Telerik.UI.Xaml.Controls.TelerikLicense").GetField("messageDisplayed",BindingFlags.NonPublic|BindingFlags.Static).SetValue(null,true,BindingFlags.Static|BindingFlags.NonPublic,null,null);
				//var signinTask = await CnVSignin.Go();// Task.Run(CnVSignin.Go);
				CityBuild.Initialize();
				// Grid.SetColumn(webView, 0);
				Grid.SetRow(CityBuild.instance, 1);
				Grid.SetRowSpan(CityBuild.instance, 5);
				Grid.SetColumnSpan(CityBuild.instance, 1);
				Canvas.SetZIndex(CityBuild.instance, 13);
				var c = CreateCanvasControl();


				// canvas.ContextFlyout = CityFlyout;
				//	grid.Children.Add(c.canvas);
				// grid.Children.Add(c.hitTest);

				// Canvas.SetZIndex(c.hitTest, 13); Task.Run(SetupCanvasInput);//
				// Task.Run(SetupCanvasInput); Placement.SizeChanged += Placement_SizeChanged; var img =
				// new Image() { Opacity=0.5f, Source = new SvgImageSource(new
				// Uri($"ms-appx:///Assets/world20.svg")),IsHitTestVisible=false };
				// Placement.LayoutUpdated += Placement_LayoutUpdated; grid.Children.Add(img);

				// Grid.SetRowSpan(img, 4); Grid.SetColumnSpan(img, 4); Canvas.SetZIndex(img, 12);
				CnVServer.Initialize(grid);
				// foreach (var i in webView.KeyboardAccelerators) i.IsEnabled = false;
				// webView.AllowFocusOnInteraction = false; c.hitTest.Margin= webView.Margin = new
				// Thickness(0, 0, 11, 0);
				//		grid.Children.Add(webView);

				//	FocusManager.GotFocus+=FocusManager_GotFocus;

				//c.hitTest.Fill = CnVServer.webViewBrush;
				//				var visual = ElementCompositionPreview.GetElementVisual(c.canvas);
				//			var webVisual = ElementCompositionPreview.GetElementVisual(view);
				//	var sprite = visual.Compositor.CreateSpriteVisual();//	var sprite = visual.Compositor.CreateSpriteVisual();

				// sprite.Brush = ElementCompositionPreview.SetElementChildVisual(visual,sprite);
				// grid.Background = null;

				// grid.Children.Add(shellFrame); Grid.SetColumn(shellFrame, 2); Grid.SetRow(shellFrame,
				// 0); Grid.SetRowSpan(shellFrame, 6); shellFrame.Margin = new Thickness(13, 0, 0, 0);
				// Canvas.SetZIndex(shellFrame, 3);

				//		Grid.SetColumn(webView, 0);
				//		Grid.SetRow(webView, 1);
				//		Grid.SetRowSpan(webView, 5);
				//		Grid.SetColumnSpan(webView, 2);
				//		Canvas.SetZIndex(webView, 10);

				//		webView.Scale = new Vector3(Settings.htmlZoom.Squared() * 2.0f + 0.5f);


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

				// NavigationService.Frame = shellFrame;

				// Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the
				// page. More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8

				KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, NavStack.BackInvoked,
																VirtualKeyModifiers.Menu));
				// KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack,NavStack.BackInvoked));

				KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Right, NavStack.ForwardInvoked,
																VirtualKeyModifiers.Menu));
				// KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoForward, NavStack.ForwardInvoked));

				for(var i = 0; i < Settings.layoutOffsets.Length; ++i)
				{
					KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Number0 + (int)i,
																		LayoutAccelerator_Invoked,
																		VirtualKeyModifiers.Control));
				}


				//			KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.F5, Refresh_Invoked,VirtualKeyModifiers.Control));
				//KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.R, instance.Refresh_Invoked,
				//												VirtualKeyModifiers.Control));
				//foreach (var k in buildKeys)
				//{
				//	KeyboardAccelerators.Add(BuildKeyboardAccelerator(k, KeyboardAccelerator));
				//}

				//				KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.F4, LayoutAccelerator_Invoked));
				IsLoggedIn   = true; // IdentityService.IsLoggedIn();
				IsAuthorized = true; // IsLoggedIn && IdentityService.IsAuthorized();
									 // grid.hor
				/// we pass this as an argument to let the page know that it is a programmatic navigation
				// Services.NavigationService.Navigate<Views.DefenseHistoryTab>(this); ChatTab.Ctor();
				TabPage.Initialize();
				// refreshAccelerator.Invoked += (_, __) => view?.Refresh();
				// testMenu.Items.Add(MenuAction(MainPage.ShowTipRaiding1,"TipRaiding1"));
				// testMenu.Items.Add(MenuAction(MainPage.ShowTipRaiding2, "TipRaiding2"));
				// testMenu.Items.Add(MenuAction(MainPage.ShowTipRaiding3, "TipRaiding3"));
				cityListBox.SelectedIndex    =  0; // reset
				cityListBox.SelectionChanged += CityListBox_SelectionChanged;
				cityBox.SelectionChanged     += CityBox_SelectionChanged;

				//SystemNavigationManager.GetForCurrentView().BackRequested += ShellPage_BackRequested;
				// PointerPressed+= PointerPressedCB; HomeButtonTip.IsOpen = true;
				//	this.ProcessKeyboardAccelerators+=ShellPage_ProcessKeyboardAccelerators;
				//this.PointerPressed+=ShellPage_PointerPressed;
				//App.SetupCoreWindowInputHooks();
				//var displayInformation = DisplayInformation.GetForCurrentView();
				//var screenSize = new Size(displayInformation.ScreenWidthInRawPixels,
				//						  displayInformation.ScreenHeightInRawPixels);
				//	ShellPage.webclientSpan.x = (screenSize.Width * .715625f* Settings.htmlZoom * 2).RoundToInt();
				//	ShellPage.webclientSpan.y = (screenSize.Height * 0.89236111111111116f * Settings.htmlZoom*2).RoundToInt();
				//	await UpdateWebViewScale();
				Log("Game Create!");
				GameClient.Create(_canvas);




				try
				{
					if(SystemInformation.Instance.IsAppUpdated && !CnVServer.isSub)
					{
						AppS.DispatchOnUIThreadLow(Settings.ShowWhatsNew);
					}

				}
				catch(Exception __ex)
				{
					Debug.Log(__ex.ToString());
				}




				TabPage.mainTabs.SizeChanged += ((o, args) => ShellPage.updateHtmlOffsets.SizeChanged());


				var okay = await signinTask;
				if(okay)
				{
					// don't await


					var changed = await PlayerTables.InitializeAndUpdateCurrentPlayer(azureId: CnVSignin.azureId, discordId: CnVSignin.discordId, discordUserName: CnVSignin.name, avatarUrlHash: CnVSignin.avatarUrlHash);
					//if (okay2)
					try
					{
						var okay3 = await APlayFab.Init();
						Assert(okay3);
						AppS.QueueOnUIThread(() =>
						{
							AppS.MessageBox($"Welcome {Player.me.shortName}.");
							AppS.appWindow.Title = $"Conquest and Virtue Alpha, World {World.id} - {Player.me.shortName}";
						});


					}
					catch(Exception exception)
					{
						LogEx(exception);
						throw;
					}
					{

					}
					//else
					//{
					//	AppS.Failed("Player name is already used.  :( ");
					//	await CnVSignin.SignOut();
					//}
				}
				else
				{
					AppS.Failed("Signin didn't happen.  :( ");
					await CnVSignin.SignOut();

				}

				CnVClient.InitializeGame();
			}
			catch(Exception ex)
			{
				LogEx(ex);
				await AppS.Fatal(ex.Message);
			}
			//Task.Delay(5000).ContinueWith((_) =>
			//{
			//	DGame.Startup();
			//});
		}

		//public static void AdjustLayout(int delta)
		//{
		//	Settings.layout+=delta;
		//	if(Settings.layout >= Settings.layoutOffsets.Length)
		//		Settings.layout = 0;
		//	if(Settings.layout < 0)
		//		Settings.layout = Settings.layoutOffsets.Length-1;
		//	updateHtmlOffsets.Go(true);
		//}
		//private void ShellPage_PointerPressed(object sender,PointerRoutedEventArgs e)
		//{
		//	if(e.GetCurrentPoint(this).Properties.IsMiddleButtonPressed)
		//	{
		//		AdjustLayout(1);
		//	}
		//}

		//private void ShellPage_ProcessKeyboardAccelerators(UIElement sender,ProcessKeyboardAcceleratorEventArgs args)
		//{
		////	Trace($"{sender} {args.Key} {args.Modifiers}");
		//	args.Modifiers.UpdateKeyModifiers();
		//	if( args.Key == VirtualKey.F3)
		//	{
		//		AdjustLayout(-1);
		//		args.Handled=true;
		//	}
		//	if(args.Key == VirtualKey.F4)
		//	{
		//		AdjustLayout(1);
		//		args.Handled=true;
		//	}
		//	if(args.Key == VirtualKey.F5)
		//	{
		//		OnRefresh();
		//		args.Handled=true;
		//	}
		//}


		//private void FocusManager_GotFocus(object sender,FocusManagerGotFocusEventArgs e)
		//{
		//	Note.Show($"Focus!!: {e.NewFocusedElement}");
		//}

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

		// canvas.Width= size.Width; canvas.Height = size.Height;

		//}
		//private void Placement_LayoutUpdated(object sender, object e)
		//{
		//	var sz = new Vector2((float)Placement.ActualWidth, (float)Placement.ActualHeight) - new Vector2((float)Canvas.GetLeft(canvas), (float)Canvas.GetTop(canvas));
		//	// measure

		// Log("layout"); AGame.SetClientSpan(sz);

		//	canvas.Width = sz.X;
		//	canvas.Height = sz.Y;
		//}

		//private void Placement_SizeChanged(object sender, SizeChangedEventArgs e)
		//{
		//	var sz = new Vector2((float)e.NewSize.Width, (float)e.NewSize.Height) -new Vector2((float)Canvas.GetLeft(canvas), (float)Canvas.GetTop(canvas) );

		// Log("Size"); AGame.SetClientSpan(sz);

		// canvas.Width = sz.X; canvas.Height = sz.Y;

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

		//private void PointerPressedCB(object sender, PointerRoutedEventArgs e)
		//{
		//	var prop = e.GetCurrentPoint(this).Properties;
		//	switch (prop.PointerUpdateKind)
		//	{
		//		case PointerUpdateKind.XButton1Pressed:
		//			NavStack.Back(true);
		//			Log("XButton1");
		//			e.Handled = true;
		//			break;
		//		case PointerUpdateKind.XButton2Pressed:
		//			NavStack.Forward(true);
		//			Log("XButton2");
		//			e.Handled = true;
		//			break;
		//	}

		//}
		//private void ShellPage_BackRequested(object sender,Microsoft.UI.Core.BackRequestedEventArgs e)
		//{
		//	Log("Back!!");
		//	NavStack.Back(true);
		//	//e.Handled = true;
		//}

		private void Refresh_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
		{
			WorkStart("Refresh");
			sender.Modifiers.UpdateKeyModifiers();
			Refresh();

			WorkEnd("Refresh");
			Note.Show("Refresh");
			// args.Handled = true;
		}

		private void LayoutAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
		{
			sender.Modifiers.UpdateKeyModifiers();
			Log($"Accel: {sender.Key} {sender.ScopeOwner}");
			var layout = args.KeyboardAccelerator.Key - VirtualKey.Number0;
			this.windowLayout.SelectedIndex = layout;

		}

		//public static MenuFlyoutItem MenuAction( Action a, string text)
		//{
		//    var rv = new MenuFlyoutItem() { Text = text };
		//    rv.Click += (_, _) => a();
		//    return rv;

		//}



		// private void OnLoggedIn(object sender, EventArgs e) { IsLoggedIn = true; IsAuthorized =
		// true;// IsLoggedIn && IdentityService.IsAuthorized(); IsBusy = false; }

		// private void OnLoggedOut(object sender, EventArgs e) { IsLoggedIn = false; IsAuthorized =
		// false; CleanRestrictedPagesFromNavigationHistory(); GoBackToLastUnrestrictedPage(); }

		//        private void CleanRestrictedPagesFromNavigationHistory()
		//        {
		//            NavigationService.Frame.BackStack
		//.Where(b => Attribute.IsDefined(b.SourcePageType, typeof(Restricted)))
		//.ToList()
		//.ForEach(page => NavigationService.Frame.BackStack.Remove(page));
		//        }

		// private void GoBackToLastUnrestrictedPage() { var currentPage =
		// NavigationService.Frame.Content as Page; var isCurrentPageRestricted =
		// Attribute.IsDefined(currentPage.GetType(), typeof(Restricted)); if
		// (isCurrentPageRestricted) { NavigationService.GoBack(); } }

		// private void OnUserProfile(object sender, RoutedEventArgs e) { if (IsLoggedIn) { OpenSettingsPage(sender,e);
		////                NavigationService.Navigate<Settings>();
		// } //else //{ // IsBusy = true; // var loginResult = await IdentityService.LoginAsync();
		// // if (loginResult != LoginResultType.Success) // { // await
		// AuthenticationHelper.ShowLoginErrorAsync(loginResult); // IsBusy = false; // } //} }

		//private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
		//{
		//    throw e.Exception;
		//}

		//private void OnCurrentPageCanGoBackChanged(object sender, bool currentPageCanGoBack)
		//    => IsBackEnabled = NavigationService.CanGoBack || currentPageCanGoBack;

		//private void Frame_Navigated(object sender, NavigationEventArgs e)
		//{
		//    IsBackEnabled = NavigationService.CanGoBack;
		//    if (e.SourcePageType == typeof(Settings))
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

		// var selectedChild = GetSelectedItem(item.MenuItems, pageType); if (selectedChild != null)
		// { return selectedChild; } }

		//    return null;
		//}

		//private bool IsMenuItemForPageType(WinUI.NavigationViewItem menuItem, Type sourcePageType)
		//{
		//    var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
		//    return pageType == sourcePageType;
		//}

		// private void OnItemInvoked(WinUI.NavigationView sender,
		// WinUI.NavigationViewItemInvokedEventArgs args) { if (args.IsSettingsInvoked) {
		// OpenSettingsPage(sender, args);
		////                NavigationService.Navigate<Settings>(null, args.RecommendedNavigationTransitionInfo);
		// return; }

		// //if (args.InvokedItemContainer is WinUI.NavigationViewItem selectedItem) //{ // var
		// pageType = selectedItem.GetValue(NavHelper.NavigateToProperty) as Type; //
		// NavigationService.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo); //} }

		//private void OnBackRequested(WinUI.NavigationView sender, WinUI.NavigationViewBackRequestedEventArgs args)
		//{
		//    NavigationService.GoBack();
		//}

		private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key,
																	TypedEventHandler<KeyboardAccelerator,
																					KeyboardAcceleratorInvokedEventArgs>
																			OnKeyboardAcceleratorInvoked,
																	VirtualKeyModifiers modifiers =
																			VirtualKeyModifiers.None)
		{
			var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
			keyboardAccelerator.Modifiers =  modifiers;
			keyboardAccelerator.Invoked   += OnKeyboardAcceleratorInvoked;
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

		private async static Task RefreshWorldData()
		{
			//World.lastUpdatedContinent = -1;
			//using var work  = new WorkScope("Refresh..");
			//var       task0 = TileData.Ctor(false);
			//if (World.completed)
			//{
			//	await GetWorldInfo.Send();
			//}

			//await task0;
		}

		public static void RefreshAndReloadWorldData()
		{
			//using var work = new WorkScope("Refresh..");
			//World.lastUpdatedContinent = -1;
			//if (World.completed)
			//{
			//	GetWorldInfo.Send();
			//}

			//TileData.Ctor(true);
		}

		public static async Task RefreshX()
		{
			using var work = new WorkScope("Refresh All");
			var       t    = RefreshWorldData();

			foreach (var city in City.allSpots)
				city.Value.OnPropertyChanged();
			NotifyCollectionBase.ResetAll(true);

			RefreshTabs.Go();
			await t;

		}

		public void RefreshX(object sender, RightTappedRoutedEventArgs e)
		{
			RefreshX();
		}

		public static Debounce RefreshTabs = new(_RefreshTabs) { };

		public static Task _RefreshTabs()
		{
			// fall through from shift-refresh. Shift refresh does both
			//	City.UpdateSenatorInfo();
			foreach (var tab in UserTab.userTabs)
			{
				if (tab.isFocused)
					tab.refresh.Go();
			}

			City.gridCitySource.ClearHash();

			CnVServer.CityRefresh();
			//	instance.UpdateWebViewScale();
			return Task.CompletedTask;
		}

		//public static void OnRefresh()
		//{
		//	if (CnVServer.world == 0)
		//	{
		//		CnVServer.view.Source = new Uri("https://www.crownofthegods.com/home/");
		//		return;
		//	}


		//	if (AppS.IsKeyPressedShift())
		//	{
		//		RefreshX();
		//	}
		//	else
		//	{
		//		Refresh();
		//	}
		//}

		private static void Refresh()
		{
			using var s = new WorkScope("Refresh...");
			foreach (var city in City.myCities)
				city.OnPropertyChanged();
			NotifyCollectionBase.ResetAll(false);
			RefreshTabs.Go();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName) =>
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(storage, value))
			{
				return;
			}

			storage = value;
			OnPropertyChanged(propertyName);
		}


		//private async void GetWorldInfo(object sender, RoutedEventArgs e)
		//{
		//    await RestAPI.getWorldInfo.Post();

		//}
		//private async void GetPPDT(object sender, RoutedEventArgs e)
		//{
		//    await CnVServer.GetCitylistOverview();
		//}

		//static string[] buildings = { "forester", "cottage", "storehouse", "quarry", "hideaway", "farmhouse", "cityguardhouse", "barracks", "mine", "trainingground", "marketplace", "townhouse", "sawmill", "stable", "stonemason", "mage_tower", "windmill", "temple", "smelter", "blacksmith", "castle", "port", "port", "port", "shipyard", "shipyard", "shipyard", "townhall", "castle" };

		//static short[] bidMap = new short[] { 448, 446, 464, 461, 479, 447, 504, 445, 465, 483, 449, 481, 460, 466, 462, 500, 463, 482, 477, 502, 467, 488, 489, 490, 491, 496, 498, bidTownHall, 467 };

		//		public static (int x, int y) webclientSpan;
		
		private async void ShowBuildings(object sender, object e)
		{
			try
			{
				var build  = City.GetBuild();
				int bCount = 0;
				var bdd    = new Dictionary<int, int>();

				void ProcessBuilding(BuildingDef bd, bool add = true)
				{
					if (bd.bid == City.bidTownHall || bd.bid == City.bidWall)
					{
						return;
					}

					var id = bd.refId;
					if (!bdd.TryGetValue(id, out var counter))
					{
						bdd.Add(id, 0);
						counter = 0;
					}

					if (add)
					{
						bdd[id] = counter + 1;
						if (!bd.isTower)
						{
							++bCount;
						}
					}
					else
					{
						bdd[id] = counter - 1;
						if (!bd.isTower)
						{
							--bCount;
						}
					}
				}

				foreach (var bdi in CityBuild.isPlanner ? build.GetLayoutBuildings() : build.postQueueBuildings)
				{
					var id = bdi.id;
					if (id == 0 || !bdi.isBuilding)
					{
						continue;
					}

					var bd = bdi.def;
					ProcessBuilding(bd);
				}

				{
					var bd = new List<BuildingCountAndBrush>();
					foreach (var i in bdd)
					{
						if (i.Value > 0)
						{
							var bdf = BuildingDef.all[i.Key];
							bd.Add(new BuildingCountAndBrush()
											{ count = i.Value, brush = CityBuild.BuildingBrush(bdf.bid, 0.5f) });
						}
					}

					bd.Add(new BuildingCountAndBrush()
									{ count = bCount, brush = CityBuild.BuildingBrush(City.bidTownHall, 0.5f) });

					// var button = sender as Button; button.Focus(FocusState.Programmatic);
					AppS.DispatchOnUIThreadLow(() =>
												{
													buildingList.Width       = double.NaN;
													buildingList.Height      = double.NaN;
													buildingList.ItemsSource = bd;
													buildingList.UpdateLayout();
												});
				}
			}
			// var flyout = FlyoutBase.GetAttachedFlyout(button);
			// flyout.OverlayInputPassThroughElement = shellPage; flyout.XamlRoot =
			// shellFrame.XamlRoot; flyout.ShowMode =
			// FlyoutShowMode.TransientWithDismissOnPointerMoveAway; Log($"{button.Tr.ToString()}");
			// var c = button.CenterPoint; flyout.ShowAt(button, new FlyoutShowOptions() { Placement
			// = FlyoutPlacementMode.Full, ShowMode =
			// FlyoutShowMode.TransientWithDismissOnPointerMoveAway }); flyout.ShowAt(button, new
			// FlyoutShowOptions() { Placement = FlyoutPlacementMode.Right, ShowMode =
			// FlyoutShowMode.Auto });

			// buildingList.Focus(FocusState.Programmatic); buildingList.CapturePointer(e.Pointer);
			// buildingList.Focus(FocusState.Programmatic); buildingList.Height = ((bd.Count + 5) /
			// 6) * 60+10; buildingList.DesiredSize
			// FlyoutBase.ShowAttachedFlyout(button);//.OverlayInputPassThroughElement = button;

			//           buildingList.UpdateLayout();
			//     button.Flyout.with
			//var mouseC = e.GetCurrentPoint(null).Position;
			//const float spawn = 20.0f;
			//      button.Focus(FocusState.Programmatic);

			// var button.Flyout.Update = new Rect(mouseC.X - spawn, mouseC.Y - spawn, mouseC.X +
			// spawn, mouseC.Y + spawn);

			// var avoid = new Rect(mouseC.X - spawn, mouseC.Y - spawn, mouseC.X + spawn, mouseC.Y +
			// spawn); button.Flyout.ShowAt(button, new FlyoutShowOptions() {
			// Placement=FlyoutPlacementMode.Full, ShowMode=FlyoutShowMode.Transient }); //
			// ,ExclusionRect=avoid });
			catch (Exception ex)
			{
				LogEx(ex);
			}
		}

		public static void ShowTipRefresh()
		{
			//if (TipsSeen.instance.refresh==false)
			//{
			//    TipsSeen.instance.refresh = true;
			//    instance.Dispatcher.RunAsync(DispatcherQueuePriority.Low, () => instance.RefreshTip.IsOpen = true);
			//}
			//else if(TipsSeen.instance.chat0==false)
			//{
			//    TipsSeen.instance.chat0 = true;
			//    instance.Dispatcher.RunAsync(DispatcherQueuePriority.Low, () => instance.ChatTip0.IsOpen = true);

			//}
			//else if (TipsSeen.instance.chat1 == false)
			//{
			//    TipsSeen.instance.chat1 = true;
			//    instance.Dispatcher.RunAsync(DispatcherQueuePriority.Low, () => instance.ChatTip1.IsOpen = true);

			//}
			//else if (TipsSeen.instance.chat2 == false)
			//{
			//    TipsSeen.instance.chat2 = true;
			//    instance.Dispatcher.RunAsync(DispatcherQueuePriority.Low, () => instance.ChatTip2.IsOpen = true);

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

		// } private void TroopOverview(object sender, RoutedEventArgs e) {
		// RestAPI.troopsOverview.Post(); }

		// private void TestRaid(object sender, RoutedEventArgs e) {
		////          ScanDungeons.Post();
		// }

		// private void GetIncomingOverview(object sender, RoutedEventArgs e) {
		// IncomingOverview.Process(false); }

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
			Settings.Show();
		}

		public static ComboBox CityListBox => instance.cityListBox;

		// private DumbCollection<CityList> cityListSelections => CityList.selections;
		private void CityListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Any())
			{
				var newSel   = e.AddedItems?.FirstOrDefault();
				var priorSel = e.RemovedItems?.FirstOrDefault();
				if (newSel != priorSel && priorSel != null)
				{
					// Log("City Sel changed");
					CityListNotifyChange(false);
				}
			}
		}


		private void CityBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var sel = cityBox.SelectedItem as City;
			if (sel != null && sel.cid != City.build)
			{
				CnVServer.CitySwitch(sel.cid, false);
				NavStack.Push(sel.cid);
			}
		}

		public void ChangeCityClick(int delta)
		{
			var  items = ShellPage.instance.cityBox.ItemsSource as City[];
			City newSel;
			if (items.Length <= 1)
			{
				if (items.Length == 0)
				{
					return;
				}

				newSel = items.First();
			}
			else
			{
				int id = Array.IndexOf(items, City.GetBuild()) + delta;
				if (id < 0)
				{
					id += items.Length;
				}

				if (id >= items.Length)
				{
					id -= items.Length;
				}

				newSel = items[id];
			}

			//newSel.SetBuild(true);
			CnVServer.CitySwitch(newSel.cid, false);
			// ElementSoundPlayer.Play(delta > 0 ? ElementSoundKind.MoveNext : ElementSoundKind.MovePrevious);
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
			var  menu = new MenuFlyout();
			bool any  = false;
			for (int i = 1; i < 25; ++i)
			{
				var str = NavStack.GetSpotName(-i);
				if (str == null)
				{
					break;
				}

				any = true;
				menu.Items.Add(AApp.CreateMenuItem(str, NavStack.instance, -i));
			}

			if (!any)
			{
				menu.Items.Add(AApp.CreateMenuItem("no more :(", () => { }));
			}

			menu.ShowAt(sender as FrameworkElement);
		}

		private void ForwardRightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			var  menu = new MenuFlyout();
			bool any  = false;
			for (int i = 1; i < 25; ++i)
			{
				var str = NavStack.GetSpotName(i);
				if (str == null)
				{
					break;
				}

				any = true;
				menu.Items.Add(AApp.CreateMenuItem(str, NavStack.instance, i));
			}

			if (!any)
			{
				menu.Items.Add(AApp.CreateMenuItem("this is the most recent :(", () => { }));
			}

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
						SpotTab.TouchSpot(cid, AppS.keyModifiers);
						CnVServer.CitySwitch(cid, false);
					}
				}
			}
		}

		public static void ContinentFilterClick(object sender, RoutedEventArgs e)
		{
			ContinentTagFilter.Show();
			/*

			var button = sender as Microsoft.UI.Xaml.Controls.DropDownButton;
			var flyout = new MenuFlyout();

			var isAll = Spot.isContinentFilterAll;
			for (int id = 0; id < World.continentCount; ++id)
			{
				var xy = World.PackedContinentToXY(id);
				var but = new ToggleMenuFlyoutItem() { IsChecked = !isAll && Spot.TestContinentFilterPacked(id), Text = ZString.Format("{0}{1}", xy.y, xy.x) };
				but.FontSize = button.FontSize;
				but.FontFamily = button.FontFamily;
				but.Margin = new Thickness(2.0f);
				flyout.Items.Add(but);
			}

			flyout.CopyXamlRoomFrom(button);
			flyout.Closing += ContinentFilterClosing;
			flyout.ShowAt(button);
			*/
		}

		//private static void ContinentFilterClosing(Microsoft.UI.Xaml.Controls.Primitives.FlyoutBase sender, Microsoft.UI.Xaml.Controls.Primitives.FlyoutBaseClosingEventArgs args)
		//{
		//	var menu = (sender as MenuFlyout);
		//	var any = false;
		//	int first = 0;
		//	Spot.continentFilter = 0;
		//	for (int id = 0; id < World.continentCount; ++id)
		//	{
		//		var but = menu.Items[id] as ToggleMenuFlyoutItem;
		//		var v = but.IsChecked;
		//		if (v)
		//		{
		//			if (!any)
		//			{
		//				first = id;
		//			}

		//			any = true;
		//			Spot.continentFilter |= Spot.ContinentFilterFlag(id);
		//		}
		//	}
		//	string label;
		//	if (!any)
		//	{
		//		Spot.continentFilter = Spot.continentFilterAll;

		//		label = "Cont";
		//	}
		//	else
		//	{
		//		// is just one set?
		//		var xy = World.PackedContinentToXY(first);
		//		if ((Spot.continentFilter & (Spot.continentFilter - 1ul)) == 0)
		//		{
		//		   label = ZString.Format("{0}{1}", xy.y, xy.x);
		//		}
		//		else
		//		{
		//			label = ZString.Format("{0}{1}+", xy.y, xy.x);
		//		}
		//	}
		//	ShellPage.instance.ContinentFilter.Content = label;
		//	ExportCastles.instance.ContinentFilter.Content = label;
		//	CityList.NotifyChange();
		//	if (HeatTab.IsVisible())
		//	{
		//		HeatTab.ResetAllChangeDescriptions();

		//	}
		//}

		private void HomeClick(object sender, RoutedEventArgs e)
		{
			if (Spot.focus == 0)
			{
				return;
			}

			if (Spot.focus.BringCidIntoWorldView(false) && City.IsBuild(Spot.focus)) // first just focus
			{
				return;
			}

			Spot.ProcessCoordClick(Spot.focus, false, AppS.keyModifiers, true); // then normal click
		}

		private void HomeRightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			var ui = sender as UIElement;
			Spot.GetFocus()?.ShowContextMenu(ui, e.GetPosition(ui));
		}

		private void BuildHomeClick(object sender, RoutedEventArgs e)
		{
			if (City.build == 0)
			{
				return;
			}

			Spot.ProcessCoordClick(City.build, false, AppS.keyModifiers, true); // then normal click
		}

		private void BuildHomeRightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			var ui = sender as UIElement;
			City.GetBuild()?.ShowContextMenu(ui, e.GetPosition(ui));
		}

		private void CityListSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
		{
			var text  = args.Text.ToLower();
			var items = CityList.selections;
			foreach (var it in items)
			{
				// its good
				if (it.name == text)
				{
					return;
				}
			}

			args.Handled = true;
			//foreach (var it in items)
			//{
			//	if (it.name.ToLower().StartsWith(text))
			//	{
			//		sender.Text = it.name;
			//		sender.SelectedItem = it;
			//		return;
			//	}
			//}
			// try contains
			foreach (var it in items)
			{
				if (it.name.ToLower().Contains(text))
				{
					sender.Text         = it.name;
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
				{
					return;
				}
			}

			args.Handled = true;
			//foreach (var it in items)
			//{
			//	if (it.nameAndRemarks.ToLower().StartsWith(text))
			//	{
			//		sender.Text = it.nameAndRemarks;
			//		sender.SelectedItem = it;
			//		return;
			//	}
			//}
			// try contains
			foreach (var it in items)
			{
				if (it.nameAndRemarks.ToLower().Contains(text))
				{
					sender.Text         = it.nameAndRemarks;
					sender.SelectedItem = it;
					return;
				}
			}
			// todo!
		}

		//private void windowLayout_SelectionChanged(object sender,SelectionChangedEventArgs e)
		//{
		//	if(MainPage.instance != null)
		//	{
		//		var viewToggle = windowLayout.SelectedIndex;
		//		SetLayout(viewToggle);
		//	}
		//}

		//		public static Vector2 webViewScale = new(1,1);
		//		internal static bool webviewHasFocus2=true;
		//private async void SetLayout(int viewToggle)
		//{
		//	if(viewToggle == Settings.layout)
		//		return;
		//	layout = viewToggle;



		//	//			   UpdateCanvasMarginForWebview(webViewScale);
		//	//scroll.ChangeView(null, null, 0.5f);
		//	//var raidInfoVisible = true;

		//	updateHtmlOffsets.Go(true);

		//	//MainPage.ToggleInfoBoxes(raidInfoVisible);
		//	//   Task.Delay(200).ContinueWith((_) => City.gridCitySource.NotifyReset());
		//	//UpdateWebViewScale();

		//}
		//		static Debounce layoutChanged = new(TabPage.LayoutChanged){ runOnUiThead = true};

		static            int popupLeftOffset, popupTopOffset;
		public static ref int popupLeftMargin => ref View.popupLeftMargin;
		public static ref int popupTopMargin  => ref View.popupTopMargin;

		public static void UpdateWebViewOffsets(int leftOffset, int topOffset)
		{
			if (popupLeftOffset != leftOffset ||
				popupTopOffset  != topOffset)
			{
				popupLeftOffset = leftOffset;
				popupTopOffset  = topOffset;
				updateHtmlOffsets.PopupsChanged();
			}

		}




		private void webFocus_Click(object sender, RoutedEventArgs e)
		{
			forceAllowWebFocus = !forceAllowWebFocus;
			//var hasFocus = !webviewHasFocus;
			webFocus.IsChecked = forceAllowWebFocus;
			//CityBuild.instance.Visibility = hasFocus ? Visibility.Collapsed : Visibility.Visible;
			//canvasVisible = !hasFocus;
			//isHitTestVisible = !hasFocus;
			//SetWebViewHasFocus(hasFocus);
			ShellPage.canvas.Visibility = ShellPage.forceAllowWebFocus ? Visibility.Collapsed : Visibility.Visible;
			ShellPage.hasKeyboardFocus  = false;
			ShellPage.UpdateFocus();

		}

		//private void FriendListSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
		//{
		//}

		//private void friendListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		//{
		//	if (e.AddedItems.Any())
		//	{
		//		var s = e.AddedItems[0] as string;
		//		var friend = Array.Find(PlayerPresence.all, f => f.name == s);
		//		CnVServer.SetPlayer(friend.pid, friend.token, friend.cookies, friend.cid, friend.name);
		//	}
		//}

		private void chatResizeTapped(object sender, TappedRoutedEventArgs e)
		{
			//			var height = new( chatGrid.RowDefinitions[0].Height.Value switch { 1=>2,2=>3,3=>4,4=>5,_=>1});
			var flyout = new MenuFlyout();
			flyout.AddItem("Tall", () =>
									{
										chatGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
										chatGrid.RowDefinitions[1].Height = new GridLength(5, GridUnitType.Star);
									});
			flyout.AddItem("Medium", () =>
									{
										chatGrid.RowDefinitions[0].Height = new GridLength(4, GridUnitType.Star);
										chatGrid.RowDefinitions[1].Height = new GridLength(2, GridUnitType.Star);
									});
			flyout.AddItem("Small", () =>
									{
										chatGrid.RowDefinitions[0].Height = new GridLength(5, GridUnitType.Star);
										chatGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
									});
			flyout.ShowAt(chatGrid, e.GetPosition(chatGrid));
		}








		//		private async void CookieClick(object sender, RoutedEventArgs e)
		//		{
		//			var content = new StackPanel();
		//			var text = new TextBox() { Header = "remember_me", PlaceholderText="01245..." };
		////			var text2 = new TextBox() { Header = "sec_session_id", PlaceholderText = "06..." };
		//			var clear  = new CheckBox() { Content = "Clear Cookie", IsChecked=false };

		//			AAnalytics.Track("CookieClick");
		//			content.Children.Add(text);
		//	//		content.Children.Add(text2);
		//			content.Children.Add(clear);

		//			var dialog = new ContentDialog()
		//			{
		//				Title = "Cookie",
		//				Content = content,
		//				PrimaryButtonText = "Apply",
		//				CloseButtonText = "Cancel"
		//			};
		//			var rv = await dialog.ShowAsync();
		//			if (rv == ContentDialogResult.Primary)
		//			{
		//				if( clear.IsChecked.GetValueOrDefault())
		//				{
		//					CnVServer.httpFilter.ClearAuthenticationCache();
		//					Microsft.UI.Xaml.WebView2.ClearTemporaryWebDataAsync();
		//					CnVServer.ClearAllCookies();
		//				}
		//				else if(!text.Text.IsNullOrEmpty()) 
		//				{
		//					CnVServer.SetCookie("remember_me", text.Text);
		//					//	CnVServer.SetCookie("_ttw", "2ebd127595739638f691d800afb6d9a2cb44f03b");
		//					//	CnVServer.SetCookie("CotG", "a%3A4%3A%7Bi%3A0%3Bs%3A5%3A%2239311%22%3Bi%3A1%3Bs%3A40%3A%220578a77365184184d96859fd54cb78925d962139%22%3Bi%3A2%3Bi%3A1626898500%3Bi%3A3%3Bi%3A0%3B%7D");
		//					//	_ttw = 062667a5a7767056ae099f43a2f6d4e24fd015fb; expires = Mon, 20 - Sep - 2021 19:44:58 GMT; Max - Age = 7776000; path =/; domain =.crownofthegods.com; secure; httponly

		//					//				WebView.ClearTemporaryWebDataAsync();
		//		//			CnVServer.SetCookie("sec_session_id", text2.Text);
		//				}
		//			//	Settings.secSessionId = text2.Text;
		//			//	Settings.SaveAll();


		//				await AppS.DoYesNoBox("Cookie set","Cookies", "Okay", null, "Okay");
		//				if(!clear.IsChecked.GetValueOrDefault())
		//					CnVServer.view.Refresh();
		//			}
		//		}

		private void FilterRightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			ContinentTagFilter.Show(true);
		}

		private void GridSpliterOnPointerReleased(object sender, PointerRoutedEventArgs e)
		{
			//	updateHtmlOffsets.Go(true);
		}

		//	protected override void OnKeyboardAcceleratorInvoked(KeyboardAcceleratorInvokedEventArgs args) => base.OnKeyboardAcceleratorInvoked(args);
		//		protected override void OnProcessKeyboardAccelerators(ProcessKeyboardAcceleratorEventArgs args) => base.OnProcessKeyboardAccelerators(args);
		//	protected override void OnPointerEntered(PointerRoutedEventArgs e) => base.OnPointerEntered(e);
		//	protected override void OnPointerPressed(PointerRoutedEventArgs e) => base.OnPointerPressed(e);
		//	protected override void OnPointerMoved(PointerRoutedEventArgs e) => base.OnPointerMoved(e);

		//private void shellPage_ProcessKeyboardAccelerators(UIElement sender,ProcessKeyboardAcceleratorEventArgs args)
		//{
		//	Trace($"Accel2 {args.Key} {sender.ToString()}");
		//}

		//static void ProcessPointerMoved(object sender,PointerRoutedEventArgs e)
		//{
		//	var c = e.GetCurrentPoint(canvas);
		//	UpdateMousePosition( c.Position );
		//}

		public static void CityListNotifyChange(bool itemsChanged)
		{

			AppS.QueueOnUIThread(async () =>
								{
									//               Log("CityListChange");

									var    selectedCityList = ShellPage.instance.cityListBox.SelectedItem as CityList;
									City[] l;
									if (selectedCityList == null || selectedCityList.id == -1) // "all"
									{
										l = City.myCities;
									}
									else
									{
										var cityList = selectedCityList; // CityList.Find(selectedCityList);
										var filtered = new List<City>();
										foreach (var cid in cityList.cities)
										{
											if (City.TryGet(cid, out var c))
											{
												filtered.Add(c);
											}
										}

										l = cityList.cities.Select(cid => cid.AsCity()).ToArray();
									}

									l = l.Where(a => a.testContinentAndTagFilter)
										.OrderBy((a) => a, new CityList.CityComparer()).ToArray();
									ShellPage.instance.cityBox.ItemsSource = l;
									City.gridCitySource.Set(l, true, itemsChanged);
									// todo change this
									Spot.selected = Spot.selected.Where(cid => City.TestContinentAndFlagFilter(cid))
										.ToHashSet(); // filter selected
									CityUI.SyncCityBox();
									CityUI.cityListChanged?.Invoke(l);
									//   if (MainPage.IsVisible())




									//if (IncomingTab.instance.isVisible)
									//   IncomingTab.instance.refresh.Go();



								});
		}
	}
	public class BuildingCountAndBrush
	{
		public Microsoft.UI.Xaml.Media.ImageBrush brush { get; set; }
		public int                                count { get; set; }

	}


}
