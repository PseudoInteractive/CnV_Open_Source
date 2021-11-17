using COTG.Draw;
using COTG.Game;

using System;
using System.Threading;
using System.Threading.Tasks;

using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

using static COTG.Debug;
using static COTG.Game.City;
using static COTG.Views.CityBuild;
using System.Collections.Generic;
using CommunityToolkit.WinUI.UI;

namespace COTG.Views
{
	//public class KeyboardProxy : Control
	//{
	//	protected override void OnKeyDown(KeyRoutedEventArgs e)
	//	{
	//		Log("keydown");
	//	}
	//	protected override void OnPreviewKeyDown(KeyRoutedEventArgs e)
	//	{
	//		Log("preview keydown");
	//		base.OnPreviewKeyDown(e);
	//	}
	//}

	public partial class ShellPage
	{
		// public static Rectangle canvasHitTest;

		public const int canvasBaseXUnscaled = 420;
		public const int canvasTitleYOffset = 40;
		public const int canvasHtmlYOffset = 55;
		public const int canvasBaseYUnscaled = 95;

//		public static int canvasScaledX = 420;
//		public static int canvasScaledY = 95;

		public static int canvasBaseX = 420;
		public static int canvasBaseY = 95;
	//	public static int htmlShift = 0;
		//public static int cachedTopOffset = 0;
		//public static int cachedXOffset = 0;
		static public SwapChainPanel canvas;
		public static bool hasKeyboardFocus;
	//	public static KeyboardProxy keyboardProxy;
		public static ViewMode viewMode;
//		public static bool webviewHasFocus=>webviewHasFocus2;
		private const int bottomMargin = 0;
		private const int cotgPopupLeft = 438;
		private const int cotgPopupRight = cotgPopupLeft + cotgPopupWidth;
		private const int cotgPopupWidth = 550;
		public enum ViewMode
		{
			city = 0,
			region = 1,
			world = 2,
			invalid = 3
		};

		public static bool IsCityView() => viewMode == ViewMode.city;

		public static bool IsWorldView() => viewMode == ViewMode.world;

		//public static void NotifyCotgPopup(int cotgPopupOpen)
		//{
		//	//JSClient.CaptureWebPage(canvas);
		//	//	cotgPopupOpen = 0;
		//	var hasPopup = (cotgPopupOpen & 127) != 0;
		//	var hasLongWindow = cotgPopupOpen >= 128;
		//	var leftOffset = hasPopup ? cotgPopupRight - canvasBaseX : 0;
		//	var topOffset = hasLongWindow ? webclientSpan.y * 65 / 100 : canvasBaseY;

		//	// temp
		//	leftOffset = 0;
		//	topOffset = canvasBaseY;

		//	if(leftOffset == cachedXOffset && cachedTopOffset == topOffset)
		//		return;
		//	cachedTopOffset = topOffset;
		//	cachedXOffset = leftOffset;
		//	//	var _canvas = canvas;
		//	var _in = canvasHitTest;

		//	App.DispatchOnUIThreadLow(() => _grid.Margin = new Thickness(0,topOffset,0,bottomMargin));
		//	App.DispatchOnUIThreadLow(() =>
		//	{
		//		_canvas.Margin = new Thickness(leftOffset,topOffset,0,0);
		//		//Canvas.SetLeft(_canvas, leftOffset);
		//		//Canvas.SetTop(_canvas, topOffset);
		//		//RemakeRenderTarget();
		//	});
		//	_grid.Dispatcher.RunAsync(Windows.UI.Core.DispatcherQueuePriority.Normal,() =>
		//   AUtil.Nop((_grid.ColumnDefinitions[0].Width = new GridLength(leftOffset),
		// _grid.ColumnDefinitions[1].Width = new GridLength(_grid.ColumnDefinitions[1].Width.Value-delta))));
		//}

		public static void SetViewMode(ViewMode _viewMode,  bool leaveZoom = false)
		{
		//	var _webviewHasFocus = pwebviewHasFocus.HasValue ? pwebviewHasFocus.Value : webviewHasFocus;
			var priorView = viewMode;
		//	var priorWebviewHasFocus = webviewHasFocus;
			viewMode = _viewMode;
		//	webviewHasFocus = _webviewHasFocus;

			if (priorView != viewMode )
			{
				//Log($"!Focus5: {hasKeyboardFocus}");
				//hasKeyboardFocus = 0;
				if (!leaveZoom && priorView != viewMode)
				{
					if (viewMode == ViewMode.world)
					{
						if (AGame.cameraZoom > AGame.cityZoomWorldThreshold)
							AGame.cameraZoom = AGame.cameraZoomWorldDefault;
					}
					else if (viewMode == ViewMode.region)
					{
						if (AGame.cameraZoom > AGame.cityZoomThreshold || AGame.cameraZoom < AGame.cityZoomWorldThreshold)
							AGame.cameraZoom = AGame.cameraZoomRegionDefault;
					}
					else
					{
						if (AGame.cameraZoom < AGame.cityZoomThreshold)
							AGame.cameraZoom = AGame.cityZoomDefault;
					}
				}

				//	ShellPage.isOverPopup = false;// reset
				//var isWorld = IsWorldView();
				//				ShellPage.isHitTestVisible = !webviewHasFocus;
			//	UpdateFocus();
				//App.DispatchOnUIThreadLow(() =>
				//{
				//	//	instance.webFocus.IsChecked = webviewHasFocus;
				//	//	Log($"!FocusWeb: {hasKeyboardFocus} w{webviewHasFocus} w2{webviewHasFocus2}");
				//	//		ShellPage.isOverPopup = false;// reset again in case it changed
				//	ShellPage.canvas.IsHitTestVisible = ShellPage.isOverPopup
				//	ShellPage.canvas.Visibility = !ShellPage.canvasVisible ? Visibility.Collapsed : Visibility.Visible;
				//	AGame.UpdateMusic();
					
				//	if (!webviewHasFocus && priorWebviewHasFocus)
				//	{
				//		TakeKeyboardFocus();
				//	}
				//});
			}
		}
		public static void UpdateFocus()
		{
			
	//		Log($"!Focu92: {ShellPage.isHitTestVisible} o{ShellPage.isMouseOver}");

			updateCanvasVisibility.Go();
		}

		static Debounce updateCanvasVisibility = new(()=>
		{
			var wantVisible = ShellPage.isHitTestVisible;
			if(ShellPage.canvas.IsHitTestVisible!= wantVisible)
			{
			//	_isHitTestVisible=wantVisible;
				ShellPage.canvas.IsHitTestVisible = wantVisible;
				//if(!wantVisible)
				//	JSClient.view.Focus(FocusState.Programmatic);
			//	TakeFocus();
			//	note|=1;
			}

			
			//if(note!=0)
			//	Note.Show($"!Focu{note}: {wantVisible} f{canvas.IsHitTestVisible} o{isOverCanvas}");


			return Task.CompletedTask;
		}) {
			runOnUiThread=true,
			debounceDelay=100,
			throttleDelay=100 };

		static bool forceFocus;
		static void TakeFocusIfAppropriate()
		{
			takeFocusIfAppropriate.Go();
		}
		static void TakeFocus()
		{
			forceFocus=true;;
			takeFocusIfAppropriate.Go();
		}
		static  Debounce takeFocusIfAppropriate = new( () =>
		{
			var isOverCanvas = isMouseOverCanvas;
			var note = 0;
			if(isOverCanvas)
			{
				if((canvas.FocusState is FocusState.Unfocused) || forceFocus)
				{
					forceFocus=false;
					//if(JSClient.view is not null && App.IsKeyPressedShift())
					{
						//		var f = JSClient.view.Focus(FocusState.Programmatic);
						//		Assert(f);
					}
					//else if( App.IsKeyPressedControl())
					//{
					//	var f = ChatTab.tabPage.Focus(FocusState.Programmatic);
					//	Assert(f);
					//}
					//App.QueueOnUIThread( ()=>
					{
						var f = canvas.Focus(FocusState.Programmatic);
						if(!forceFocus)
						{
							if(!f)
							{
								Assert(false);
							}
						}
					} //);
					note|=2;
				}
			}
#if DEBUG
		//	if(note!=0)
		//		Note.Show($"!Focu{note}: f{canvas.IsHitTestVisible} o{isOverCanvas}");
#endif
			return Task.CompletedTask;

		})
		{
			runOnUiThread=true,
			debounceDelay=50,
			throttleDelay=100
		};

		
		private static bool IsMouseInCanvas()
		{
			return mouseOverCanvas;
			
		//	return canvas.IsLocalPointOver(mousePosition.X,mousePosition.Y) && !IsMouseOverChat();
		}
		private static bool IsMouseOverChat()
		{
			var xf = canvas.TransformToVisual(ChatTab.tabPage );
			var pt = xf.TransformPoint(new(mousePosition.X,mousePosition.Y));
			return ChatTab.tabPage.IsLocalPointOver(pt.X,pt.Y);
		}
		private static bool isMouseOverCanvas => IsMouseInCanvas() && isHitTestVisible;

		public static void SetViewModeCity()
		{
			SetViewMode(ViewMode.city);
		}

		public static void SetViewModeRegion() => SetViewMode(ViewMode.region);

		public static void SetViewModeWorld() => SetViewMode(ViewMode.world);

		//public static void SetWebViewHasFocus(bool _webviewHasFocus)
		//{
		//	Log($"!Focu92: {hasKeyboardFocus} w{webviewHasFocus} w2{webviewHasFocus2}");
		//	SetViewMode(viewMode, _webviewHasFocus);
		//}

		//public static void TakeKeyboardFocus()
		//{
		//	//if (hasKeyboardFocus) return; //Trace($"Take focus {hasKeyboardFocus}"); Set this
		//	// early, it gets set again once the asyn executes
		//	//	Assert(webviewHasFocus == webviewHasFocus2);
			
		//	Log($"!Focus0: {hasKeyboardFocus} w{webviewHasFocus} w2{webviewHasFocus2}");
		//	if (webviewHasFocus)
		//		return;
		//	if ( Interlocked.CompareExchange(ref hasKeyboardFocus, 1, 0) == 0)
		//	{
		//		// set to 0
		//		//	Log($"!Focus1: {hasKeyboardFocus} w{webviewHasFocus} w2{webviewHasFocus2}");
		//		App.QueueOnUIThreadIdle( () =>
		//		{
		//		//	if(hasKeyboardFocus==2)
		//			{
		//				Log($"!Focus8: {hasKeyboardFocus} w{webviewHasFocus} w2{webviewHasFocus2}");
		//				// back to 0 temporarily 
		//				hasKeyboardFocus = 0;
		//		//	Log("Focus: " + webviewHasFocus + webviewHasFocus2);
		//			//	keyboardProxy.Focus(FocusState.Programmatic);
		//			//	App.cursorDefault.Set();
		//			}
		//			//	Log($"!Focus2: {hasKeyboardFocus} w{webviewHasFocus} w2{webviewHasFocus2}");
		//		});
		//	}
		//}

		//CanvasStrokeStyle defaultStrokeStyle = new CanvasStrokeStyle() { CustomDashStyle=new float[] { 2, 6 },
		//    DashCap=CanvasCapStyle.Triangle,
		//    EndCap=CanvasCapStyle.Triangle,
		//    StartCap=CanvasCapStyle.Triangle};
		//const float dashD0 = 6.0f;
		//const float dashD1 = 6f;
		//CanvasStrokeStyle defaultStrokeStyle = new CanvasStrokeStyle()
		//{
		//	CustomDashStyle = new float[] { dashD0, dashD1 },
		//	EndCap = CanvasCapStyle.Flat,
		//	DashCap = CanvasCapStyle.Triangle,
		//	StartCap = CanvasCapStyle.Flat,
		//	//           TransformBehavior=CanvasStrokeTransformBehavior.Hairline
		//};
		public (SwapChainPanel canvas, Rectangle hitTest) CreateCanvasControl()
		{
			//Assert((0.5f).CeilToInt() == 1);
			//Assert((-1.0f).CeilToInt() == -1);
			//Assert((0.0f).CeilToInt() == 0);
			//Assert((-0.5f).CeilToInt() == 0);
			//Assert((0.5f).FloorToInt() == 0);
			//Assert((-1.0f).FloorToInt() == -1);
			//Assert((0.0f).FloorToInt() == 0);
			//Assert((-0.5f).FloorToInt() == -1);
			const float dpiLimit = 96.0f;

			canvas = _canvas;
			
			//{
			//	// DpiScale = SettingsPage.dpiScale != 0 ? SettingsPage.dpiScale : (dpiLimit / DisplayInformation.GetForCurrentView().LogicalDpi).Min(1.0f),
			//	Name = "DX",
			//	IsHitTestVisible = false,
			//	Visibility = Visibility.Visible,
			////	Background=null,
			//	IsTabStop=true,
			//	AllowFocusOnInteraction=true,
			//	// IsTabStop = true, UseSharedDevice = true, TargetElapsedTime =
			//	// TimeSpan.FromSeconds(1.0f / 60.0f),
				
			//	Margin = new Thickness(0, canvasBaseYUnscaled, 0, 0),
			//	// IsFixedTimeStep = false
			//};
			//keyboardProxy = new KeyboardProxy()
			//{
			//	AllowFocusOnInteraction = true,
			//	TabFocusNavigation = KeyboardNavigationMode.Once,
			//	Background = null,
			//	IsTabStop = true
			//};

			//canvas.Children.Add(keyboardProxy);
			//			canvas.PreviewKeyDown+=KeyboardProxy_KeyDown;
		//	canvas.KeyDown += KeyboardProxy_KeyDown;
			App.window.Content.PreviewKeyDown+=ShellPage.KeyboardProxy_KeyDown; ;
			canvas.PreviewKeyDown+=ShellPage.KeyboardProxy_KeyDown2; ;
			//			App.window.Content.PreviewKeyUp+=ShellPage.KeyboardProxy_KeyUp;
			App.window.Content.AddHandler(PointerWheelChangedEvent,new PointerEventHandler( KeyboardProxy_PointerWheelChanged),true);
			//			canvas.AddHandler(KeyDownEvent,new KeyEventHandler(KeyboardProxy_KeyDown2),true);
			//keyboardProxy.LostFocus += KeyboardProxy_LostFocus;
			//keyboardProxy.GotFocus += KeyboardProxy_GotFocus;
			//	keyboardProxy.PointerWheelChanged+=KeyboardProxy_PointerWheelChanged;
			canvas.PointerWheelChanged += KeyboardProxy_PointerWheelChanged;

			//canvas.AddHandler(canvas.Children.Add(keyboardProxy);)
			//			GettingFocusEvent
			//		keyboardProxy.AddHandler(GettingFocusEvent, KeyboardProxy_GettingFocus, true);
			//		keyboardProxy.AddHandler(LostFocusEvent, KeyboardProxy_LostFocus, true);
		//	canvas.ProcessKeyboardAccelerators+=Canvas_ProcessKeyboardAccelerators;
			//canvasHitTest = new Rectangle()
			//{
			//	Name="webDrawer",
			//	IsHitTestVisible = true,
			//	 Opacity = 1,
			//	 Stretch=Stretch.Fill,

			//};

			// canvas.Draw += Canvas_Draw; canvas.Update += Canvas_Update;

			//canvas.LayoutUpdated += AGame.Canvas_LayoutUpdated;
			//canvas.CreateResources += Canvas_CreateResources;
			//			canvasHitTest.Margin=canvas.Margin = new Thickness(0, 0, 0, bottomMargin);
			//canvasHitTest.Stretch = Stretch.Fill;
			//  SetupCoreInput();
			canvas.CompositionScaleChanged += Canvas_CompositionScaleChanged;
			SetupNonCoreInput();
			
		///	SetupCoreInput();
			return (canvas, null);
		}

		//private void KeyboardProxy_KeyDown2(object sender,KeyRoutedEventArgs e)
		//{
		//	Log($"Key!!Canvas {e.Key} {e}");

		//}

		//private void Canvas_ProcessKeyboardAccelerators(UIElement sender,ProcessKeyboardAcceleratorEventArgs args)
		//{
		//	Log($"Key!!Canvas {args.Key} {args}");
		//}

		private void KeyboardProxy_PointerWheelChanged(object sender,PointerRoutedEventArgs e)
		{
			if(mouseOverCanvas)
			{
				var pt = e.GetCurrentPoint(canvas);
				HandleWheel(pt.Position,pt.Properties.MouseWheelDelta);
			}
			else
			{
				Log("Mouse not over");
			}
		}

		private void Canvas_CompositionScaleChanged(SwapChainPanel sender, object args)
		{
			Log(canvas.CompositionScaleX);
		}


		public static void KeyboardProxy_KeyDown2(object sender,Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			Log($"KeyDown2 {e.Key} handled:{e.Handled} mouse:{mouseOverCanvas}");
			if(!e.Handled)
			{
				e.Handled=	DoKeyDown(e.Key);
			}
		}

		public static void KeyboardAccelerator(KeyboardAccelerator acc,KeyboardAcceleratorInvokedEventArgs args)
		{
			Log($"Accelerator {acc.Key} handled:{args.Handled} mouse:{mouseOverCanvas}");
			if(args.Handled)
			{

				return;
			}
			args.Handled=DoKeyDown(acc.Key);
		}
		public static void KeyboardProxy_KeyDown(object sender,Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			Log($"KeyDown {e.Key} handled:{e.Handled} mouse:{mouseOverCanvas}"); 
			//KeyDown(e.Key);
			if(!e.Handled)
				e.Handled= DoKeyDown(e.Key);
		}
		static HashSet<VirtualKey> GetBuildKeys()
		{
			var rv = new HashSet<VirtualKey>();
			rv.UnionWith( new[] { VirtualKey.Up,VirtualKey.Down,VirtualKey.Left,VirtualKey.Right,VirtualKey.Space, VirtualKey.Enter, VirtualKey.F11, VirtualKey.F10,
			VirtualKey.U,VirtualKey.D,VirtualKey.Escape,(VirtualKey)192, VirtualKey.F,VirtualKey.C,VirtualKey.R,VirtualKey.S,VirtualKey.A,VirtualKey.B,VirtualKey.I,VirtualKey.T,
			VirtualKey.M,VirtualKey.V,VirtualKey.L, VirtualKey.E,VirtualKey.H,VirtualKey.W,VirtualKey.G,VirtualKey.Y,VirtualKey.Z,VirtualKey.K,
			VirtualKey.X,VirtualKey.O,VirtualKey.P,VirtualKey.Q});
			rv.UnionWith(Enumerable.Range(0,9).Select(x => (VirtualKey)(VirtualKey.Number0 + x)));
			return rv;
			
		}
		public static HashSet<VirtualKey> buildKeys = GetBuildKeys();

		public static Debounce1<VirtualKey> hotKeyDown = new(_KeyDown) { debounceDelay=0,throttleDelay=50, runOnUiThread=true,throttled=true };
		public static bool DoKeyDown(VirtualKey key)
		{
			Log($"DoKeyDown {key}");
			App.UpdateKeyStates();
			if(App.IsKeyPressedShiftOrControl())
				return false;
			if(!buildKeys.Contains(key))
			{
				Log("Not a build Key " + key);
				return false;
			}
			if(!mouseOverCanvas)
			{
				Log("Not over canvas");
				return false;
			}
			// don't process if chat has focus
			foreach(var tab in ChatTab.all)
			{
				if(tab.input.FocusState != FocusState.Unfocused)
				{
					Log($"{tab.Name} {tab.input.Focus}");
					return false;
				}
			}
			App.UpdateKeyStates();

			hotKeyDown.Go(key);
			return true;
		}

		public static Task _KeyDown(VirtualKey key)
		{
			Log("SomeKeyDown " + key);

			App.UpdateKeyStates();



			if(!mouseOverCanvas)
			{
				return Task.CompletedTask;
				Log("Mouse not over");
			}
			App.InputRecieved();

			if(!isHitTestVisible)
				return Task.CompletedTask;

			//if (CityBuild.menuOpen)
			//{
			//	// todo:  Handle naviation menu items and selection
			//	App.DispatchOnUIThreadLow(() =>
			//	{
			//		ShellPage.instance.buildMenu.IsOpen = false;
			//	});
			//	return;

			//}
			if (!IsCityView())
			{
				switch (key)
				{
					// todo: handle differently for city view

					case Windows.System.VirtualKey.Space:
						Spot.ProcessCoordClick(Spot.focus, false, App.keyModifiers, true);
						break;

					case Windows.System.VirtualKey.Left:
						Spot.SetFocus(Spot.focus.Translate((-1, 0)), true, true, true);
						break;

					case Windows.System.VirtualKey.Up:
						Spot.SetFocus(Spot.focus.Translate((0, -1)), true, true, true);
						break;

					case Windows.System.VirtualKey.Right:
						Spot.SetFocus(Spot.focus.Translate((1, 0)), true, true, true);
						break;

					case Windows.System.VirtualKey.Down:
						Spot.SetFocus(Spot.focus.Translate((0, 1)), true, true, true);
						break;
				}
			}
			else
			{
				// App.DispatchOnUIThreadLow(() =>
				
				ProcessKey(key);
				
			}
			switch (key)
			{
				case Windows.System.VirtualKey.F11:
					if (Player.isAvatarOrTest)
					{
						Raid.test ^= true;
						Note.Show("Test: " + Raid.test);
					}
					break;

				case Windows.System.VirtualKey.F10:
					if (Player.isAvatarOrTest  || (App.IsKeyPressedShift() && App.IsKeyPressedControl()))
					{
						CityBuild.testFlag ^= true;
						Note.Show("Test: " + testFlag);
						JSClient.view.ExecuteScriptAsync($"setTestFlag({(testFlag ? "1" : "0")})");
					}
					break;
			}
			return Task.CompletedTask;
			// });
		}

		//private void KeyboardProxy_LostFocus(object sender, RoutedEventArgs e)
		//{
		//	//	Log($"!FocusLost: {hasKeyboardFocus} w{webviewHasFocus} w2{webviewHasFocus2}");
		//	Note.Show($"!Focus10: {hasKeyboardFocus} w{webviewHasFocus} w2{webviewHasFocus2}");
		//	// Trace($"Lost focus {hasKeyboardFocus}");
		//	hasKeyboardFocus = false;
		//	// CityBuild.ClearAction();
		//}
		//private void KeyboardProxy_GotFocus(object sender,RoutedEventArgs e)
		//{
		//	//	Log($"!FocusGot: {hasKeyboardFocus} w{webviewHasFocus} w2{webviewHasFocus2}");
		//	Note.Show($"!Focus9: {hasKeyboardFocus} w{webviewHasFocus} w2{webviewHasFocus2}");
		//	//			hasKeyboardFocus = 1;
		//	ShellPage.hasKeyboardFocus = true;
		//	//Log("Get focus");
		//	//	Trace($"Got focus {hasKeyboardFocus}");
		//}

	}
}
