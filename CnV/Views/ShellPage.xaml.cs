﻿using System.Runtime.CompilerServices;

using Windows.Foundation;
using Windows.System;
//using Windows.UI.Core;
//using Windows.UI.Core.Preview;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

using WinUI = Microsoft.UI.Xaml.Controls;
using CommunityToolkit.WinUI.Helpers;
namespace CnV.Views
{
	using Microsoft.AppCenter;
	using Microsoft.UI.Input;
	using Microsoft.UI.Xaml.Data;

	using Windows.Storage;
	using Windows.Storage.Pickers;
	// using PInvoke

	//	using System.Diagnostics;
	using static CnV.Debug;

	//public class LogEntryStruct
	//{
	//    public string t { get; set; }
	//    public LogEntryStruct()
	//    {
	//    }
	//    public LogEntryStruct(string _t) { t =_t; }
	//}
	// TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
	public sealed partial class ShellPage:UserControl, INotifyPropertyChanged
	{
		public const int canvasZDefault = 11;
		public const int canvasZBack = 0;
		public static CnVSwapChainPanel canvas => GameClient.canvas;
		public int layout {
			get => Settings.layout;
			set {
				if(Settings.layout != value) {
					Settings.layout = value;
					updateHtmlOffsets.SystemUpdated();
				}
			}
		}

		class LayoutItem
		{
			public string name { get; set; }
			public int id { get; set; }

		}
		const float minTimeScale = 0.25f;
		const float maxTimeScale = 1024.0f;

		static float RoundNicely(float d) {
			var absD = d.Abs();

			if(absD > 100)
				return MathF.Round(d);
			else if(absD > 10)
				return MathF.Round(d*10)/10;
			else if(absD> 1)
				return MathF.Round(d/100)*100;
			else
				return MathF.Round(d/1000)*1000;
		}
		// slider ranges from or 0..12, 2^(x-2) 1/4 .. 1024
		static double SliderToTimeScale(double v) => v<= 0 ? 0 : Math.Pow(2,v-2);
		static double TimeScaleToSlider(double v) => v <= 0 ? 0 : (Math.Log2(v) + 2).Clamp(0,12);

		static internal Grid rootGrid;

		internal float timeScale {
			get {
				return Sim.timeScale;
			}
			set {
				if(value.IsEqualTo(Sim.timeScale))
					return;
				if(Sim.isInitialized)
					Sim.SetTimeScale(value.Clamp(minTimeScale,maxTimeScale));
				OnPropertyChanged();
			}
		}

		internal float viewZ {
			get => (View.targetZ/View.viewZCityDefault).Sqrt();
			set {
				var z = (value.Squared() * View.viewZCityDefault).Clamp(View.viewMinZ,View.viewMaxZ);
				if( !z.AlmostEquals(View.targetZ,0.125f) )
					View.SetTargetZ(z,AppS.IsKeyPressedShift() );
			}
		}
		internal int viewX {
			get => (View.viewTargetW.X/10).RoundToInt();
			set => View.SetViewTarget(View.viewTargetW2 with { X= value*10+5 } );
		}
		internal int viewY {
			get => (View.viewTargetW.Y/10).RoundToInt();
			set => View.SetViewTarget(View.viewTargetW2 with { Y= value*10+5 } );
		}


		class TimeScaleToolTipConverter:IValueConverter
		{


			public object Convert(object value,Type targetType,object parameter,string language) => SliderToTimeScale((double)value).ToString();

			public object ConvertBack(object value,Type targetType,object parameter,string language) {
				{ LogEx(new NotImplementedException("Convert")); return null; }
			}
		}
		TimeScaleToolTipConverter timeScaleToolTipConverter = new TimeScaleToolTipConverter();
		private void TimeScalueSliderValueChanged(object sender,RangeBaseValueChangedEventArgs e) {
			if(!e.OldValue.AlmostEquals(e.NewValue,1.0f/2.0f)) {


				var v = SliderToTimeScale(e.NewValue);
				if(Sim.isInitialized)
					Sim.SetTimeScale((float)v);

			}

		}
		internal void TimeScaleChangeNotify() {
			Debounce.Q(runOnUIThread: true,action: () => {



				var v = Sim.timeScale;
				//if(!timeScaleNumberBox.Value.IsEqualTo(v,1.0f/8.0f))
				//{
				// timeScaleNumberBox.Value = v;
				//}
				var sliderValue = TimeScaleToSlider(v);
				if(!timeScaleSlider.Value.AlmostEquals(sliderValue,0.25f)) {
					timeScaleSlider.Value = sliderValue;
				}
				//var symbol = v == 0 ? Symbol.Play : Symbol.Stop;
				//var icon = timeTogglePlayIcon;
				//if(icon.Symbol != symbol)
				//	icon.Symbol = symbol;
				OnPropertyChanged();
			});
		}
		internal static void HistoricModeUIUpdate() {
			AppS.QueueOnUIThread(() => {
				instance.timePlay.IsEnabled = Sim.isHistoric;
				instance.timeForward.IsEnabled = Sim.isHistoric | AppS.isSinglePlayer;
				instance.timeScaleSlider.IsEnabled = Sim.isHistoric| AppS.isSinglePlayer;
				instance.timeScaleNumberBox.IsEnabled = Sim.isHistoric| AppS.isSinglePlayer;
				
			});
		}
		//internal void TimeScaleValueChanged(NumberBox sender,NumberBoxValueChangedEventArgs e)
		//{
		//	if(e.NewValue.IsNaN())
		//	{
		//		App.FilterNans(sender,e);
		//		return;
		//	}
		//	if(!e.OldValue.IsEqualTo(e.NewValue,1.0f/4.0f))
		//	{

		//		if(Sim.isInitialized)
		//			ServerTime.SetTimeScale((float)e.NewValue);
		//	}
		//}
		//float timeScaleSlider
		//{
		//	get => MathF.Log2(Sim.timeScale) + 2.0f;
		//	set
		//	{
		//		var v = SliderToTimeScale(value);
		//		Log(v);;
		//		ServerTime.SetTimeScale(v);
		//		OnPropertyChanged("timeScale");
		//	}

		//}
		string[] layoutOptions = System.Linq.Enumerable.Range(0,Settings.layoutOffsets.Length)
			.Select(a => $"Layout {a}").ToArray();

		//private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
		//private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);
		//private readonly KeyboardAccelerator _forwardKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoForward);
		static public ShellPage? instance;



		public static float webZoomLast; // for lazy setting of HTML zoom
		public static TextBlock gridTip;

		int GridRowIndex(RowDefinition row,int offset) {
			return grid.RowDefinitions.IndexOf(row)+offset;
		}


		//		public static ScrollViewer webView;

		private static DateTime workStarted;
		private static readonly List<string> workQueue = new List<string>();

		//	protected override void OnKeyDown(KeyRoutedEventArgs e) => Trace($"Key: {e.Key} {e.OriginalKey} {e.OriginalSource.ToString()}");
		//		protected override void OnPreviewKeyDown(KeyRoutedEventArgs e) => Trace($"KeyP: {e.Key} {e.OriginalKey} {e.OriginalSource.ToString()}");
		public static void WorkStart(string desc, bool isDeterminate=false) {
			AppS.DispatchOnUIThreadLow(() => {
				if(!workQueue.Any()) {
					instance.workContainer.Visibility = Visibility.Visible;
					instance.progressContainer.Visibility=Visibility.Visible;
					instance.progress.IsIndeterminate = !isDeterminate;

//					instance.progress.Value=0.25f;
					// FIX
					workStarted              = DateTime.UtcNow;
					instance.work.Text       = desc;
				}

				workQueue.Add(desc);
			});
		}

		// Todo:  Queue updates with tasks
		public static void WorkUpdate(string desc, float? completion=null) {

			AppS.DispatchOnUIThreadLow(() => 
			{
				instance.work.Text = desc;
				if(instance.workContainer.Visibility == Visibility.Collapsed) {
					instance.workContainer.Visibility = Visibility.Visible;
					instance.progressContainer.Visibility=Visibility.Visible;

				}
				if(completion != null) {
					instance.progress.IsIndeterminate=false;
					instance.progress.Value = completion.Value;
				}
				else {
					instance.progress.IsIndeterminate=true;

				}

			}
			);
		}

		public static void WorkEnd(string desc = null) {
			AppS.DispatchOnUIThreadLow(() => {
				if(!workQueue.Any()) {
					Log("End end called too often");
				}
				else {
					if(DateTime.UtcNow - workStarted > TimeSpan.FromMinutes(5)) {
						Log("rogue work item");
						workQueue.Clear();
					}
					else {
						if(desc is null)
							workQueue.RemoveAt(0);
						else
							workQueue.Remove(desc);
					}
				}

				if(!workQueue.Any()) {
					//instance.progress.IsActive = false;
					instance.workContainer.Visibility = Visibility.Collapsed;
					instance.progressContainer.Visibility=Visibility.Collapsed;


				}
				else {
					instance.progress.IsIndeterminate=true;
					instance.progress.Value=0.5f;
				
					workStarted        = DateTime.UtcNow;
					instance.work.Text = workQueue.First();
				}
			});
		}



		// private IdentityService IdentityService => Singleton<IdentityService>.Instance;

		// private UserDataService UserDataService => Singleton<UserDataService>.Instance;
		public NotifyList<string> inAppNotes = new();
		//		public static InAppNotification inAppNote => instance.InAppNote;
		public string noteText = string.Empty;



		public ShellPage() {
			//			instance = this;
			InitializeComponent();
			//		RequestedTheme = ElementTheme.Dark; // default theme
			instance = this;
			try {
				App.SetupTitleBar();
				//AppS.window.ExtendsContentIntoTitleBar = true;
				//AppS.window.SetTitleBar(AppTitleBar);
				App.UpdateAppTitleUI();
			}
			catch(Exception _ex) {
				LogEx(_ex);

			}
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
		internal static Canvas gameUIFrame;
		//public static bool _isHitTestVisible;
		public static bool canvasVisible = true;
		//public static bool isFocused => isHitTestVisible && AppS.isForeground;

		private async void OnLoaded(object sender,RoutedEventArgs e) {
			try {
				


				gameUIFrame = _gameUIFrame;
				rootGrid = _rootGrid;
			//	instance = this;
				View.dipToNative = XamlRoot.RasterizationScale;

				//Windows.Devices.Input.PointerDevice getPointerDeviceFromPointPoint(PointerPoint ptrPt)
				//{
				//	var pointerDeviceList = Windows.Devices.Input.PointerDevice.GetPointerDevices();

				//	foreach(var pointerDevice in pointerDeviceList) {
				//		Log($"PointerDevice: {JSON.ToJson(pointerDevice)}");
				//	}

				//}

				//var ps = new PlayerStats();
				//Header = ps;

				//{
				//	var sw = new Stopwatch();
				//	sw.Start();
				//	HashSet<int> ms0 = new();
				//	HashSet<int> ms1 = new();
				//	var t0 = DateTimeOffset.UtcNow;
				//	var end = t0 + TimeSpan.FromSeconds(1.0f);
				//	var ta0 = sw.ElapsedMilliseconds;
				//	for(;;)
				//	{
				//		var t = DateTimeOffset.UtcNow;
				//		if( t >= end )
				//			break;
				//		ms0.Add( (t-t0).TotalMilliseconds.RoundToInt() );
				//		ms1.Add( (int)(sw.ElapsedMilliseconds - ta0) );
				//	}
				//	Log(ms0);
				//	HashSet<int> ms0a = new();
				//	HashSet<int> ms1a = new();
				//	 t0 = DateTimeOffset.UtcNow;
				//	 end = t0 + TimeSpan.FromSeconds(1.0f);
				//	ta0 = Environment.TickCount64;
				//	for(; ; )
				//	{
				//		var t = DateTimeOffset.UtcNow;
				//		if(t >= end)
				//			break;
				//		ms0a.Add((t-t0).TotalMilliseconds.RoundToInt());
				//		ms1a.Add((int)(sw.ElapsedMilliseconds - ta0));
				//		Thread.Sleep(0);
				//	}
				//	Log(ms0);

				//}

				GameClient.canvas    = _canvas;

				var signinTask = CnVSignin.Go();
				//				App.window.titleImage.Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri($"ms-appx:///Assets/AppIcon24.png"));
				Note.Init();
				CityUI.Init();
				NavStack.Init();
				// hook up delegates
	

				WorkStart("Sign in",true);

				//		SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += App.App_CloseRequested; ;
				//typeof(Telerik.UI.Xaml.Controls.RadDataForm).Assembly.GetType("Telerik.UI.Xaml.Controls.TelerikLicense").GetField("messageDisplayed",BindingFlags.NonPublic|BindingFlags.Static).SetValue(null,true,BindingFlags.Static|BindingFlags.NonPublic,null,null);
				//var signinTask = await CnVSignin.Go();// Task.Run(CnVSignin.Go);
				CityBuild.Initialize();
				// Grid.SetColumn(webView, 0);
				Grid.SetRow(CityBuild.instance,1);
				Grid.SetRowSpan(CityBuild.instance,5);
				Grid.SetColumnSpan(CityBuild.instance,1);
				Canvas.SetZIndex(CityBuild.instance,13);
				var c = CreateCanvasControl();

				//				var cachePlayerTask = Task.Run(PlayerGameEntity.UpdateCache);
				// canvas.ContextFlyout = CityFlyout;

				//	grid.Children.Add(c.canvas);
				// grid.Children.Add(c.hitTest);
				//AppS.DispatchOnUIThreadLow(() =>
				//{
				//	var display = Microsoft.Graphics.Canvas.CanvasDevice.GetSharedDevice();
				//Log( display.IsPixelFormatSupported(Windows.Graphics.DirectX.DirectXPixelFormat.R10G10B10A2UIntNormalized) );
				//Log(display.IsPixelFormatSupported(Windows.Graphics.DirectX.DirectXPixelFormat.R10G10B10XRBiasA2UIntNormalized));

				//Log(display.IsPixelFormatSupported(Windows.Graphics.DirectX.DirectXPixelFormat.R11G11B10Float));
				////	var colorInfo = display.GetAdvancedColorInfo();
				////	AGame.colorKind = colorInfo.CurrentAdvancedColorKind;
				////	display.AdvancedColorInfoChanged+= (a,__) =>
				////	{
				////		AGame.colorKind = a.GetAdvancedColorInfo().CurrentAdvancedColorKind;
				////	};
				//});

				// Canvas.SetZIndex(c.hitTest, 13); Task.Run(SetupCanvasInput);//
				// Task.Run(SetupCanvasInput); Placement.SizeChanged += Placement_SizeChanged; var img =
				// new Image() { Opacity=0.5f, Source = new SvgImageSource(new
				// Uri($"ms-appx:///Assets/world20.svg")),IsHitTestVisible=false };
				// Placement.LayoutUpdated += Placement_LayoutUpdated; grid.Children.Add(img);

				// Grid.SetRowSpan(img, 4); Grid.SetColumnSpan(img, 4); Canvas.SetZIndex(img, 12);
				//nVServer.Initialize(grid);
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




				//			KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.F5, Refresh_Invoked,VirtualKeyModifiers.Control));
				//KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.R, instance.Refresh_Invoked,
				//												VirtualKeyModifiers.Control));
				//foreach (var k in buildKeys)
				//{
				//	KeyboardAccelerators.Add(BuildKeyboardAccelerator(k, KeyboardAccelerator));
				//}


				/// we pass this as an argument to let the page know that it is a programmatic navigation
				// Services.NavigationService.Navigate<Views.DefenseHistoryTab>(this); ChatTab.Ctor();
				TabPage.Initialize();
				// refreshAccelerator.Invoked += (_, __) => view?.Refresh();
				// testMenu.Items.Add(MenuAction(MainPage.ShowTipRaiding1,"TipRaiding1"));
				// testMenu.Items.Add(MenuAction(MainPage.ShowTipRaiding2, "TipRaiding2"));
				// testMenu.Items.Add(MenuAction(MainPage.ShowTipRaiding3, "TipRaiding3"));
				//cityListBox.SelectedIndex    =  0; // reset
				//cityListBox.SelectionChanged += CityListBox_SelectionChanged;
				//cityBox.SelectionChanged     += CityBox_SelectionChanged;

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
				Assert(AppS.state == AppS.State.init);
				AppS.SetState(AppS.State.setup);

				Log("Game Create!");


				try {
					GameClient.Create(_canvas);
				}
				catch(Exception ex) {
					LogEx(ex);
					AppS.Fatal(ex.ToString());
					return;
				}


				// Links will not work until after the game is set up
				try {
					if(SystemInformation.Instance==null || SystemInformation.Instance.IsAppUpdated) {
						AppS.DispatchOnUIThread(Settings.ShowWhatsNew);
					}

				}
				catch(Exception __ex) {
					Debug.Log(__ex.ToString());
				}




				//		TabPage.mainTabs.SizeChanged += ((o,args) => ShellPage.updateHtmlOffsets.SizeChanged());
				//		chatTabs.SizeChanged+=((o,args) => ShellPage.updateHtmlOffsets.SizeChanged());

				var okay = await signinTask;
				if(okay) {
					// don't await
#if APPCENTER

					{
						if(AAnalytics.initialized)
							AppCenter.SetUserId(CnVSignin.name);
						//AppCenter.Analytics.Properties.put("UserId", "your user Id");
						//	CustomProperties properties = new CustomProperties();
						//	properties.Set("alliance",Alliance.myId).Set("allianceName",Alliance.my.name).Set("world",CnVServer.world).Set("sub",CnVServer.isSub).Set("playerId",Player.myId).Set("UserId",Player.myName);
						//	AppCenter.SetCustomProperties(properties);
						AAnalytics.defaultProperties = ImmutableDictionary<string,string>.Empty.Add("UserId",CnVSignin.name).Add("Version",AppS.currentVersion.ToString() ) ;
						AAnalytics.Track("GotCreds");
						//ShellPage.UpdateFocus();
					}
#endif

					await PlayerTables.InitializeAndUpdateCurrentPlayer(azureId: CnVSignin.azureId,discordId: CnVSignin.discordId,discordUserName: CnVSignin.name,avatarUrlHash: CnVSignin.avatarUrlHash);
					//if (okay2)
					try {
						if(Player.me.gender == Gender.na) {
							var rv = await AppS.DoYesNoBox("Gender","Which would you like to be?","Female","Male","Nonbinary");
							Player.me.gender = rv switch { 0 => Gender.male, 1 => Gender.female, _ => Gender.both };
							await (new PlayerGameEntity(Player.me.pid) { sex=(int)Player.me.gender }).UpsertAsync();
						}

						var okay3 = await APlayFab.Init();
						Assert(okay3);



					}
					catch(Exception exception) {
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
				else {
					AppS.Failed("Signin didn't happen.  :( ");
					await CnVSignin.SignOut();

				}
				if(AppS.appAlreadyRunning) {
					var a = await AppS.DoYesNoBox(title: "Conquest and Virtue is already running",text: "Please close all game windows, of if you see none, restart computer",
								yes: "Exit",
								no: "Closed Others"

								);
					if(a != 0) {
						Application.Current.Exit();
						await Task.Delay(-1).ConfigureAwait(false);
					}



				}

				WorkUpdate("Init");
				await CnVClient.InitializeGame();
				//		Player.activePlayerChanged += (p0,p1) => AppS.UpdateAppTitle();

				//AppS.QueueOnUIThread(() =>
				//{
				App.UpdateAppTitle();


				//	//							AppS.appWindow.SetIcon(new IconId(0));
				//	//	AppS.MessageBox($"Welcome {Player.me.shortName}.");
				//});
				FindName(nameof(playerStats));
				FindName(nameof(cityStats));
				FindName(nameof(cityFlyout));

				var KeyboardAccelerators = commandBar.KeyboardAccelerators;
				IncomingTab.UpdateIncomingStatus();
				PlayerStats.instance.UpdateOutgoingText();

				KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.F5,Refresh));
				KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left,NavStack.BackInvoked,
																VirtualKeyModifiers.Menu));
				KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.F1,(_,m) => { m.Handled=true; ErrorReport(); },
																VirtualKeyModifiers.Menu));
				// KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack,NavStack.BackInvoked));

				KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Right,NavStack.ForwardInvoked,
																VirtualKeyModifiers.Menu));
				// KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoForward, NavStack.ForwardInvoked));

				for(var i = 0;i < Settings.layoutOffsets.Length;++i) {
					KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Number0 + (int)i,
																		LayoutAccelerator_Invoked,
																		VirtualKeyModifiers.Control));
				}


				KeyboardAccelerators.Add(BuildKeyboardAccelerator(key: VirtualKey.Enter,modifiers:
																VirtualKeyModifiers.Menu,OnKeyboardAcceleratorInvoked: (_,a) => {
																	a.Handled=true;
																	Settings.fullScreen = !Settings.fullScreen.GetValueOrDefault();
																	AppS.appWindow.SetPresenter(Settings.fullScreen.GetValueOrDefault() ? Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen : Microsoft.UI.Windowing.AppWindowPresenterKind.Overlapped);
																}));
				KeyboardAccelerators.Add(BuildKeyboardAccelerator(key: VirtualKey.F12,modifiers:
																VirtualKeyModifiers.Control|VirtualKeyModifiers.Menu|VirtualKeyModifiers.Shift,OnKeyboardAcceleratorInvoked: (_,a) => {
																	a.Handled=true;
																	AppS.isTest ^= true;
																	Trace(AppS.isTest.ToString());
																}));
				//KeyboardAccelerators.Add(BuildKeyboardAccelerator(key: VirtualKey.F12,modifiers:
				//												VirtualKeyModifiers.Control|VirtualKeyModifiers.Shift,OnKeyboardAcceleratorInvoked: (_,a) => {
				//													a.Handled=true;
				//													if(AppS.isTest)
				//														Sim.AddSubs();
				//												}));
				KeyboardAccelerators.Add(BuildKeyboardAccelerator(key: VirtualKey.F12,modifiers:
																VirtualKeyModifiers.Control|VirtualKeyModifiers.Menu,OnKeyboardAcceleratorInvoked: (_,a) => {
																	a.Handled=true;
																	if(AppS.isTest) {
																		AppS.allIntel ^= true;
																		Trace(AppS.allIntel.ToString());
																	}
																}));
				KeyboardAccelerators.Add(BuildKeyboardAccelerator(key: VirtualKey.F12,modifiers:
																VirtualKeyModifiers.Shift|VirtualKeyModifiers.Menu,OnKeyboardAcceleratorInvoked: (_,a) => {
																	a.Handled=true;
																	if(AppS.isTest)
																		ArtifactDialogue.ShowInstance(Artifact.GetForPlayerRank(Artifact.ArtifactType.zirconia),free:true );
																}));

				//AppS.SetState(AppS.State.active);


				await Task.Delay(500);
				await ShellPage.RefreshX();

				System.GC.Collect(2,GCCollectionMode.Default,true,true);
				Microsoft.UI.Xaml.Media.Animation.Timeline.AllowDependentAnimations=false;
				TabPage.ShowTabs();


				HistoricModeUIUpdate(); // Disable buttons
				//Microsoft.UI.Xaml.Media.Animation.Timeline.AllowDependentAnimations=false;
				AppS.QueueIdleTask(DailyDialog.DailyRewardTask);
				{
					var p = Player.me;
					if(!p.hasAlliance && p.allianceInvites.Any()) {
						AppS.QueueOnUIThread(JoinAlliance.ShowInstance);

					}
				}
				//				if(AppS.processingTasksStarted == false)
				{


					//	Task.Run(AppS.ProcessThrottledTasks);
					Task.Run(AppS.ProcessIdleTasks);
				}
			}
			catch(Exception ex) {
				LogEx(ex);
				AppS.Fatal(ex.ToString());
			}
			//Task.Delay(5000).ContinueWith((_) =>
			//{
			//	DGame.Startup();
			//});
		}


		internal async static void ErrorReport() {



			{
				var dialog = new ErrorReport();
				var a = await dialog.ShowAsync2();
				if(a == ContentDialogResult.Primary) {
					AAnalytics.ErrorReport(dialog.notes.Text);
					AppS.MessageBox("Error Report Sent");
				}
			};
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

		private void Refresh_Invoked(KeyboardAccelerator sender,KeyboardAcceleratorInvokedEventArgs args) {
			WorkStart("Refresh");
			sender.Modifiers.UpdateKeyModifiers();
			Refresh();

			WorkEnd("Refresh");
		//	Note.Show("Refresh");
			// args.Handled = true;
		}

		private void LayoutAccelerator_Invoked(KeyboardAccelerator sender,KeyboardAcceleratorInvokedEventArgs args) {
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
																			VirtualKeyModifiers.None) {
			var keyboardAccelerator = new KeyboardAccelerator() { Key = key,Modifiers = modifiers };
			keyboardAccelerator.Invoked   += OnKeyboardAcceleratorInvoked;
			return keyboardAccelerator;
		}
		private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key,
																	Action OnKeyboardAcceleratorInvoked,
																	VirtualKeyModifiers modifiers =
																			VirtualKeyModifiers.None) {
			var keyboardAccelerator = new KeyboardAccelerator() { Key = key,Modifiers = modifiers };
			keyboardAccelerator.Invoked   += (m,a) => {
				try {
					a.Handled=true;
					OnKeyboardAcceleratorInvoked();
				}
				catch(Exception _ex) {
					LogEx(_ex);

				}
			};
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

		public void Refresh(object o,RoutedEventArgs e) {
			Refresh();
		}

		//private async static Task RefreshWorldData()
		//{
		//	//World.lastUpdatedContinent = -1;
		//	//using var work  = new WorkScope("Refresh..");
		//	//var       task0 = TileData.Ctor(false);
		//	//if (World.completed)
		//	//{
		//	//	await GetWorldInfo.Send();
		//	//}

		//	//await task0;
		//}

		public static void RefreshAndReloadWorldData() {
			//using var work = new WorkScope("Refresh..");
			//World.lastUpdatedContinent = -1;
			//if (World.completed)
			//{
			//	GetWorldInfo.Send();
			//}

			//TileData.Ctor(true);
		}

		public static async Task RefreshX() {
			try {
				using var work = new WorkScope("Refresh All");
				//	var t = RefreshWorldData();
				World.InvalidateCities();
				ShellPage.CityListNotifyChange(true);

				World.RefreshTileDataForCurrentContinent();

				NotifyCollectionBase.ResetAll(true);
				CityUI.Refresh();
				RefreshTabs.Go();
				//	Note.Show($"Refresh all");

				foreach(var city in City.allCities) {
					city.OnPropertyChanged();
				}

			
				System.GC.Collect(2,GCCollectionMode.Default,true,true);

				//await t;
			}
			catch(Exception _ex) {
				LogEx(_ex);

			}

		}

		public void RefreshX(object sender,RightTappedRoutedEventArgs e) {
			e.Handled=true;
			RefreshX();
		}

		public static Debounce RefreshTabs = new(_RefreshTabs) { debounceDelay= 400 };

		public static Task _RefreshTabs() {
			// fall through from shift-refresh. Shift refresh does both
			//	City.UpdateSenatorInfo();
			foreach(var tab in UserTab.userTabs) {
				if(tab.isFocused)
					tab.refresh.Go();
			}

			City.gridCitySource.ClearHash();

			Sim.CityRefresh();
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

		private static void Refresh() {
			try {

				//PlayerTables.LoadPlayersAndAlliances();
				RefreshX();
				//using var s = new WorkScope("Refresh...");
				//foreach(var city in City.myCities)
				//	city.OnPropertyChanged();
				//NotifyCollectionBase.ResetAll(false);
				//RefreshTabs.Go();
			}
			catch(Exception _ex) {
				LogEx(_ex);

			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName = null) =>
				PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));

		private void Set<T>(ref T storage,T value,[CallerMemberName] string propertyName = null) {
			if(Equals(storage,value)) {
				return;
			}

			storage = value;
			OnPropertyChanged(propertyName);
		}

		public static void QueueOnPropertyChanged(string propertyName = null) {
			AppS.QueueOnUIThreadIdle(() => instance.OnPropertyChanged(propertyName));
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



		public static void ShowTipRefresh() {
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

		private void FlyoutClosing(FlyoutBase sender,FlyoutBaseClosingEventArgs args) {
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

		private void TipTest(object sender,RoutedEventArgs e) {
			ShowTipRefresh();
		}

		private void ShowSettings(object sender,RoutedEventArgs e) {
			Settings.Show();
		}

	//	public static ComboBox CityListBox => instance.cityListBox;

		// private DumbCollection<CityList> cityListSelections => CityList.selections;
		private void CityListBox_SelectionChanged(object sender,SelectionChangedEventArgs e) {
			try {
				if(e.AddedItems.Any()) {
					var newSel = e.AddedItems?.FirstOrDefault();
					var priorSel = e.RemovedItems?.FirstOrDefault();
					if(newSel != priorSel && priorSel != null) {
						// Log("City Sel changed");
						CityListNotifyChange(false);
					}
				}
			}
			catch(Exception _ex) {
				LogEx(_ex);

			}
		}



		public void ChangeCityClick(int delta) {
			try {
				var items = City.gridCitySource.c;
				City newSel;
				if(items.Length <= 1) {
					if(items.Length == 0) {
						return;
					}

					newSel = items.First();
				}
				else {
					int id = items.IndexOf(City.GetBuild());
					if(id == -1) {
						id = 0;
					}
					else {
						id += delta;
						if(id < 0) {
							id += items.Length;
						}

						if(id >= items.Length) {
							id -= items.Length;
						}

					}
					newSel = items[id];
				}

				//newSel.SetBuild(true);
				CnVClient.CitySwitch(newSel.cid,false);
				// ElementSoundPlayer.Play(delta > 0 ? ElementSoundKind.MoveNext : ElementSoundKind.MovePrevious);
				NavStack.Push(newSel.cid);
			}
			catch(Exception _ex) {
				LogEx(_ex);

			}
		}

		private void PriorCityClick(object sender,RoutedEventArgs e) {
			ChangeCityClick(-1);
		}

		private void NextCityClick(object sender,RoutedEventArgs e) {
			ChangeCityClick(+1);
		}

		private void BackRightTapped(object sender,RightTappedRoutedEventArgs e) {
			try {
				var menu = new MenuFlyout();
				bool any = false;
				for(int i = 1;i < 25;++i) {
					var str = NavStack.GetSpotName(-i);
					if(str == null) {
						break;
					}

					any = true;
					menu.Items.Add(AApp.CreateMenuItem(str,NavStack.instance,-i));
				}

				if(!any) {
					menu.Items.Add(AApp.CreateMenuItem("no more :(",() => { }));
				}

				menu.ShowAt(sender as FrameworkElement);
			}
			catch(Exception _ex) {
				LogEx(_ex);

			}
		}

		private void ForwardRightTapped(object sender,RightTappedRoutedEventArgs e) {
			try {
				var menu = new MenuFlyout();
				menu.SetXamlRoot();
				bool any = false;
				for(int i = 1;i < 25;++i) {
					var str = NavStack.GetSpotName(i);
					if(str == null) {
						break;
					}

					any = true;
					menu.Items.Add(AApp.CreateMenuItem(str,NavStack.instance,i));
				}

				if(!any) {
					menu.Items.Add(AApp.CreateMenuItem("this is the most recent :(",() => { }));
				}

				menu.ShowAt(sender as FrameworkElement);
			}
			catch(Exception _ex) {
				LogEx(_ex);

			}
		}

		//private void coords_KeyDown(object sender,KeyRoutedEventArgs e) {
		//	try {
		//		var str = sender as TextBox;
		//		Assert(str != null);
		//		if(str != null) {
		//			if(e.Key == Windows.System.VirtualKey.Enter) {
		//				var cid = str.Text.FromCoordinate();
		//				if(cid > 0) {
		//					//NavStack.Push(cid);
		//					//SpotTab.TouchSpot(cid,AppS.keyModifiers);
		//					City.ProcessCoordClick(cid,true,AppS.keyModifiers);
		//				}
		//			}
		//		}
		//	}
		//	catch(Exception _ex) {
		//		LogEx(_ex);

		//	}
		//}

		//private void ContinentFilterTapped(object sender,TappedRoutedEventArgs e)
		//{
		//	ContinentTagFilter.Show();
		//}
		public static void ContinentFilterClick(object sender,RoutedEventArgs e) {
			ContinentTagFilter.Show();
			//	/*

			//	var button = sender as Microsoft.UI.Xaml.Controls.DropDownButton;
			//	var flyout = new MenuFlyout();

			//	var isAll = Spot.isContinentFilterAll;
			//	for (int id = 0; id < World.continentCount; ++id)
			//	{
			//		var xy = World.PackedContinentToXY(id);
			//		var but = new ToggleMenuFlyoutItem() { IsChecked = !isAll && Spot.TestContinentFilterPacked(id), Text = ZString.Format("{0}{1}", xy.y, xy.x) };
			//		but.FontSize = button.FontSize;
			//		but.FontFamily = button.FontFamily;
			//		but.Margin = new Thickness(2.0f);
			//		flyout.Items.Add(but);
		}

		//	flyout.CopyXamlRoomFrom(button);
		//	flyout.Closing += ContinentFilterClosing;
		//	flyout.ShowAt(button);
		//	*/
		//}

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

		//private void FocusClick(object sender,RoutedEventArgs e) {
		//	try {
		//		if(Spot.focus == 0) {
		//			return;
		//		}

		//		if(Spot.focus.BringIntoWorldView(false) && City.IsBuild(Spot.focus)) // first just focus
		//		{
		//			return;
		//		}

		//		Spot.ProcessCoordClick(Spot.focus,AppS.keyModifiers.ClickMods(scrollIntoUi:true)); // then normal click
		//	}
		//	catch(Exception _ex) {
		//		LogEx(_ex);

		//	}
		//}

		internal static void CityRightTapped(object sender,RightTappedRoutedEventArgs e) {
			try {
				var ui = sender as UIElement;
				Spot.GetFocus()?.ShowContextMenu(e.GetPosition(ui));
			}
			catch(Exception _ex) {
				LogEx(_ex);

			}
		}

		internal static void BuildHomeClick(object sender,RoutedEventArgs e) {
			try {
				if(City.build == 0) {
					return;
				}

				Spot.ProcessCoordClick(City.build,AppS.keyModifiers.ClickMods(scrollIntoUi: true)); // then normal click
			}
			catch(Exception _ex) {
				LogEx(_ex);

			}
		}

		internal static void BuildHomeRightTapped(object sender,RightTappedRoutedEventArgs e) {
			try {
				if(City.build == 0) {
					return;
				}

				Spot.ProcessCoordClick(City.build,AppS.keyModifiers.ClickMods(isRight:true,scrollIntoUi: true)); // then normal click
			}
			catch(Exception _ex) {
				LogEx(_ex);

			}
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

		static int popupLeftOffset, popupTopOffset;
		//public static ref int popupLeftMargin => ref View.popupLeftMargin;
		//public static ref int popupTopMargin => ref View.popupTopMargin;

		public static void UpdateWebViewOffsets(int leftOffset,int topOffset) {
			if(popupLeftOffset != leftOffset ||
				popupTopOffset  != topOffset) {
				popupLeftOffset = leftOffset;
				popupTopOffset  = topOffset;

			}

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

		private void chatResizeTapped(object sender,TappedRoutedEventArgs e) {
			//			var height = new( chatGrid.RowDefinitions[0].Height.Value switch { 1=>2,2=>3,3=>4,4=>5,_=>1});
			var flyout = new MenuFlyout();
			flyout.SetXamlRoot();
			flyout.AddItem("Tall",() => {
				chatGrid.RowDefinitions[0].Height = new GridLength(1,GridUnitType.Star);
				chatGrid.RowDefinitions[1].Height = new GridLength(5,GridUnitType.Star);
			});
			flyout.AddItem("Medium",() => {
				chatGrid.RowDefinitions[0].Height = new GridLength(4,GridUnitType.Star);
				chatGrid.RowDefinitions[1].Height = new GridLength(2,GridUnitType.Star);
			});
			flyout.AddItem("Small",() => {
				chatGrid.RowDefinitions[0].Height = new GridLength(5,GridUnitType.Star);
				chatGrid.RowDefinitions[1].Height = new GridLength(1,GridUnitType.Star);
			});
			flyout.ShowAt(chatGrid,e.GetPosition(chatGrid));
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

		private void FilterRightTapped(object sender,RightTappedRoutedEventArgs e) {
			ContinentTagFilter.Show(true);
		}


		private void TimeBackClick(object sender,RoutedEventArgs e) {
			var t = Sim.simTime - TimeSpanS.FromHours(gotoTimeOffset.Value);


			Sim.GotoTime(t);
		}

		private async void TimeResetClick(object sender,RoutedEventArgs e) {
			if(await AppS.DoYesNoBox("Rewind","This takes you back to the start, are you sure?").ConfigureAwait(false) != 1)
				return;
			Sim.GotoTime(Sim.serverStartTime);
		}

		private void TimeForwardClick(object sender,RoutedEventArgs e) {
			var dt = TimeSpanS.FromHours(gotoTimeOffset.Value);

			//if(!AppS.isSinglePlayer) {
			//	if(AppS.isTest) {
			//		(new SocketSimControlArgs() { timeScale = Sim.timeScale,deltaServerTickOffset=dt }).SendToServer();
			//		return;
			//	}
			//}
			Sim.GotoTime(Sim.simTime +dt);
		}

		private void TimeTogglePlay(object sender,RoutedEventArgs e) {
			//if(!AppS.isSinglePlayer) {
			//	AppS.MessageBox("Only works in single player mode");
			//	return;
			//}
			if(Sim.timeScale == 0) {
				Sim.SetTimeScale(1.0f);
			}
			else {
				Sim.SetTimeScale(0.0f);
			}
		}

		// From Historic
		private void TimeResumeInteractive(object sender,RoutedEventArgs e) {
			//if(!AppS.isSinglePlayer) {
			//	AppS.MessageBox("Only works in single player mode");
			//	return;
			//}
			//Sim.ClearFutureEvents();
			if(!Sim.isHistoric) {
				Note.Show("No need to leave, you are already in the present.");
				return;
			}
			Sim.HistoricEndRequest();
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

		public static void CityListNotifyChange(bool itemsChanged) {
			if(!Sim.isInteractiveOrHistoric)
				return;
			AppS.QueueOnUIThread(() => {
				//               Log("CityListChange");

				//	var selectedCityList = null;// ShellPage.instance.cityListBox.SelectedItem as CityList;
				City[] l = City.subCities;
				//if(selectedCityList == null || selectedCityList.id == -1) // "all"
				//{
				//	l = City.subCities;
				//}
				//else {
				//	var cityList = selectedCityList; // CityList.Find(selectedCityList);
				//	var filtered = new List<City>();
				//	foreach(var cid in cityList.cities) {
				//		if(City.TryGet(cid,out var c)) {
				//			filtered.Add(c);
				//		}
				//	}

				//	l = cityList.cities.Select(cid => cid.AsCity()).ToArray();
				//}

				l = l.Where(a => a.testContinentAndTagFilter)
					.OrderBy((a) => a,new CityList.CityComparer()).ToArray();
				//	CityStats.instance.cityBox.ItemsSource = l;
				City.gridCitySource.Set(l,true,itemsChanged);
				City.gridCitySourceWithNone.Set(l.Prepend(City.invalid));
				// todo change this
				Spot.selected = Spot.selected.Where(cid => City.TestContinentAndFlagFilter(cid))
					.ToImmutableHashSet(); // filter selected
								  //	CityUI.cityListChanged?.Invoke(l);
				CityUI.SyncCityBox();
				//   if (MainPage.IsVisible())




				//if (IncomingTab.instance.isVisible)
				//   IncomingTab.instance.refresh.Go();



			});
		}



		private void GridSplitter_ManipulationCompleted(object sender,ManipulationCompletedRoutedEventArgs e) {
			//Note.Show("ManipulationCompleted");
			updateHtmlOffsets.UserUpdated();
			
		}

		private void SendError(object sender,RoutedEventArgs e) {
			ErrorReport();
		}

		private void SaveTimeline(object sender,RoutedEventArgs e) {
			if(AppS.isTest || AppS.isHost) {
				SaveWorld(false);
			}
			else {
				Note.Show("Cannot save in multiplayer");
			}
		}

		internal static async Task<bool> SaveWorld(bool useCurrentSave) {
			try {
				FileSavePicker savePicker = new FileSavePicker();
				savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
				savePicker.CommitButtonText="Export World";
				// Dropdown of file types the user can save the file as
				savePicker.FileTypeChoices.Add("CnV World Timeline",new List<string>() { ".mpk" });
				// Default file name if the user does not type one in or select a file to replace
				savePicker.SuggestedFileName = "timeline";

				WinRT.Interop.InitializeWithWindow.Initialize(savePicker,WinRT.Interop.WindowNative.GetWindowHandle(AppS.window));
				StorageFile file = await savePicker.PickSaveFileAsync();
				if(file != null) {
					// Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
					var path = file.Path;
					if(useCurrentSave) {
						File.Copy(Sim.eventDataFileInfo.fileName,path,true);
					}
					else {
						DataFiles.SaveBinary(path,Sim.PersistEvents());
					}
					Note.Show("Exported timeline");
					return true;
				}
				else {
				}
			}
			catch(Exception _ex) {
				LogEx(_ex);

			}
			return false;
		}
		private void LoadTimeline(object sender,RoutedEventArgs e) {
			LoadWorld();
		}

		internal static async Task<bool> LoadWorld() {
			try {
				if(!AppS.isSinglePlayer) {
					AppS.MessageBox("Only works in single player mode");
					return false;
				}

				FileOpenPicker savePicker = new FileOpenPicker();
				savePicker.CommitButtonText = "Load World";
				savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
				// Dropdown of file types the user can save the file as
				savePicker.FileTypeFilter.Add(".mpk");
				// Default file name if the user does not type one in or select a file to replace

				WinRT.Interop.InitializeWithWindow.Initialize(savePicker,WinRT.Interop.WindowNative.GetWindowHandle(AppS.window));
				StorageFile file = await savePicker.PickSingleFileAsync();
				if(file != null) {
					// Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
					var path = file.Path;

					Note.Show("Load timeline");
					//	Sim.ResetSim(eventStream: DataFiles.LoadBinary(path));
					File.Copy(path,Sim.eventDataFileInfo.fileName,true);
					return true;
				}
				else {
				}
			}
			catch(Exception _ex) {
				LogEx(_ex);

			}
			return false;
		}

		private void FormAlliance(object sender,RoutedEventArgs e) {
			if(Player.active.title.rank < (byte)TitleName.Baronetess) {
				AppS.MessageBox("Must be Dame/Knight to create alliance");
			}
			else {
				CreateAlliance.ShowInstance();
			}
		}

		private void ShowAllianceInvites(object sender,RoutedEventArgs e) {
			CnV.JoinAlliance.ShowInstance();
		}

        private void InfoGotFocus(object sender,PointerRoutedEventArgs e) {
			//		var x = sender as FrameworkElement;
			//			if(x is not null)
			InAppNote.Height = InAppNote.MaxHeight;
        }
		private void InfoLostFocus(object sender,PointerRoutedEventArgs e) {
			InAppNote.Height = InAppNote.MinHeight;
		}





		private void ChatGotFocus(object sender,PointerRoutedEventArgs e) {
			var s = chatTabs;
			s.CapturePointer(e.Pointer);
			s.Height = s.MaxHeight;
			chatGridSpliterX.Visibility  = Visibility.Visible;
			chatGridSpliterY.Visibility  = Visibility.Visible;

		}

		private void ChatLostFocus(object sender,PointerRoutedEventArgs e) {
			var s = chatTabs;
			s.Height = s.MinHeight;
			chatGridSpliterX.Visibility  = Visibility.Collapsed;
			chatGridSpliterY.Visibility  = Visibility.Collapsed;

		}

		//private void GotoTimeOffset(object sender,RoutedEventArgs e)
		//{
		//	var dt = TimeSpanS.FromMinutes( gotoTimeOffset.Value );
		//	gotoTimeFlyout.Hide();
		//	CnVServer.GoToTime(CnVServer.simTime + dt);
		//}

		private void CityListSubmitted(ComboBox sender,ComboBoxTextSubmittedEventArgs args) {
			var text = args.Text.ToLower();
			var items = CityList.selections;
			foreach(var it in items) {
				// its good
				if(it.name == text) {
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
			foreach(var it in items) {
				if(it.name.ToLower().Contains(text)) {
					sender.Text         = it.name;
					sender.SelectedItem = it;
					return;
				}
			}
			// todo!
		}
	}



}
