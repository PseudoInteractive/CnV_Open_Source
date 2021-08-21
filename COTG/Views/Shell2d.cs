﻿using COTG.Draw;
using COTG.Game;

using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

using static COTG.Debug;
using static COTG.Game.City;
using static COTG.Views.CityBuild;

namespace COTG.Views
{
	public class KeyboardProxy : Control
	{
	}

	public partial class ShellPage
	{
		// public static Rectangle canvasHitTest;

		public const int canvasBaseXUnscaled = 410;
		public const int canvasTitleYOffset = 42;
		public const int canvasHtmlYOffset = 53;
		public static int canvasBaseYUnscaled = 95;

		public static int canvasBaseX = 410;
		public static int canvasBaseY = 95;
		//public static int cachedTopOffset = 0;
		//public static int cachedXOffset = 0;
		static public SwapChainPanel canvas;
		public static bool hasKeyboardFocus;
		public static KeyboardProxy keyboardProxy;
		public static ViewMode viewMode;
		public static bool webviewHasFocus;
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

		public static void NotifyCotgPopup(int cotgPopupOpen)
		{
			//JSClient.CaptureWebPage(canvas);
			//	cotgPopupOpen = 0;
			//var hasPopup = (cotgPopupOpen & 127) != 0;
			//var hasLongWindow = cotgPopupOpen >= 128;
			//var leftOffset = hasPopup ? cotgPopupRight - canvasBaseX : 0;
			//var topOffset = hasLongWindow ? webclientSpan.y * 65 / 100 : canvasBaseY;

			//// temp
			//leftOffset = 0;
			//topOffset = canvasBaseY;

			//if (leftOffset == cachedXOffset && cachedTopOffset == topOffset)
			//	return;
			//cachedTopOffset = topOffset;
			//cachedXOffset = leftOffset;
			////	var _canvas = canvas;
			// var _in = canvasHitTest;

			//  App.DispatchOnUIThreadLow(() => _grid.Margin = new Thickness(0, topOffset, 0, bottomMargin));
			//App.DispatchOnUIThreadLow(() =>
			//{
			//	_canvas.Margin = new Thickness(leftOffset , topOffset, 0, 0);
			//	//Canvas.SetLeft(_canvas, leftOffset);
			//	//Canvas.SetTop(_canvas, topOffset);
			//	//RemakeRenderTarget();
			//});
			//            _grid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
			//            AUtil.Nop( (_grid.ColumnDefinitions[0].Width = new GridLength(leftOffset),
			//          _grid.ColumnDefinitions[1].Width = new GridLength(_grid.ColumnDefinitions[1].Width.Value-delta))));
		}

		public static void SetViewMode(ViewMode _viewMode, bool? pwebviewHasFocus = null, bool leaveZoom = false)
		{
			var _webviewHasFocus = pwebviewHasFocus.HasValue ? pwebviewHasFocus.Value : webviewHasFocus;
			var priorView = viewMode;
			var priorWebviewHasFocus = webviewHasFocus;
			viewMode = _viewMode;
			webviewHasFocus = _webviewHasFocus;

			if (priorView != viewMode || webviewHasFocus != priorWebviewHasFocus)
			{
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

				ShellPage.isOverPopup = false;// reset
											  //var isWorld = IsWorldView();
				ShellPage.isHitTestVisible = !webviewHasFocus;
				
				App.DispatchOnUIThreadLow(() =>
				{
					instance.webFocus.IsChecked = webviewHasFocus;
					Log("WebViewFocus");
					ShellPage.isOverPopup = false;// reset again in case it changed
					ShellPage.canvas.IsHitTestVisible = ShellPage.isHitTestVisible;
					ShellPage.canvas.Visibility = !ShellPage.canvasVisible ? Visibility.Collapsed : Visibility.Visible;
					AGame.UpdateMusic();
					if (!webviewHasFocus && priorWebviewHasFocus)
					{
						TakeKeyboardFocus();
					}
				});
			}
		}

		public static void SetViewModeCity()
		{
			SetViewMode(ViewMode.city, webviewHasFocus);
		}

		public static void SetViewModeRegion() => SetViewMode(ViewMode.region, webviewHasFocus);

		public static void SetViewModeWorld() => SetViewMode(ViewMode.world, webviewHasFocus);

		public static void SetWebViewHasFocus(bool _webviewHasFocus)
		{
			SetViewMode(viewMode, _webviewHasFocus);
		}

		public static void TakeKeyboardFocus()
		{
			//if (hasKeyboardFocus) return; //Trace($"Take focus {hasKeyboardFocus}"); Set this
			// early, it gets set again once the asyn executes
			hasKeyboardFocus = true;
			App.DispatchOnUIThreadLow(() =>
			{
				keyboardProxy.Focus(FocusState.Programmatic);
			});
		}

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

			canvas = new SwapChainPanel()
			{
				// DpiScale = SettingsPage.dpiScale != 0 ? SettingsPage.dpiScale : (dpiLimit / DisplayInformation.GetForCurrentView().LogicalDpi).Min(1.0f),
				Name = "Region",
				IsHitTestVisible = true,
				Visibility = Visibility.Visible,
				// IsTabStop = true, UseSharedDevice = true, TargetElapsedTime =
				// TimeSpan.FromSeconds(1.0f / 60.0f),
				Margin = new Thickness(0, canvasBaseY*ShellPage.webViewScale, 0, 0),
				// IsFixedTimeStep = false
			};
			keyboardProxy = new KeyboardProxy()
			{
				AllowFocusOnInteraction = true,
				TabFocusNavigation = KeyboardNavigationMode.Once,
				Background = null,
				IsTabStop = true
			};
			keyboardProxy.AddHandler(KeyDownEvent, new KeyEventHandler(KeyboardProxy_KeyDown), true);
			keyboardProxy.LostFocus += KeyboardProxy_LostFocus;
			keyboardProxy.GettingFocus += KeyboardProxy_GettingFocus;
//			GettingFocusEvent
	//		keyboardProxy.AddHandler(GettingFocusEvent, KeyboardProxy_GettingFocus, true);
	//		keyboardProxy.AddHandler(LostFocusEvent, KeyboardProxy_LostFocus, true);
			canvas.Children.Add(keyboardProxy);
			//canvasHitTest = new Rectangle()
			//{
			//	Name="webDrawer",
			//	IsHitTestVisible = true,
			//	 Opacity = 1,
			//	 Stretch=Stretch.Fill,

			//};

			// canvas.Draw += Canvas_Draw; canvas.Update += Canvas_Update;

			canvas.SizeChanged += AGame.Canvas_SizeChanged;
			//canvas.LayoutUpdated += AGame.Canvas_LayoutUpdated;
			//canvas.CreateResources += Canvas_CreateResources;
			//			canvasHitTest.Margin=canvas.Margin = new Thickness(0, 0, 0, bottomMargin);
			//canvasHitTest.Stretch = Stretch.Fill;
			//  SetupCoreInput();
			canvas.CompositionScaleChanged += Canvas_CompositionScaleChanged;
			return (canvas, null);
		}

		private void Canvas_CompositionScaleChanged(SwapChainPanel sender, object args)
		{
			Log(canvas.CompositionScaleX);
		}

		private void KeyboardProxy_GettingFocus(UIElement sender, Windows.UI.Xaml.Input.GettingFocusEventArgs args)
		{
			if(args.FocusState == FocusState.Unfocused)
			{
				Assert(false);
				return;
			}
			if(args.NewFocusedElement == keyboardProxy)
			{
				hasKeyboardFocus = true;
			}
			else

			{
				Assert(false);
			}
			//Log("Get focus");
			//	Trace($"Got focus {hasKeyboardFocus}");
		}

		private void KeyboardProxy_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			App.InputRecieved();
			//if (CityBuild.menuOpen)
			//{
			//	// todo:  Handle naviation menu items and selection
			//	App.DispatchOnUIThreadLow(() =>
			//	{
			//		ShellPage.instance.buildMenu.IsOpen = false;
			//	});
			//	return;

			//}
			var key = e.Key;
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
					if (Player.isAvatarOrTest || (App.IsKeyPressedShift() && App.IsKeyPressedControl()))
					{
						CityBuild.testFlag ^= true;
						Note.Show("Test: " + testFlag);
						JSClient.view.InvokeScriptAsync("setTestFlag", new[] { (testFlag ? "1" : "0") });
					}
					break;
			}

			// });
		}

		private void KeyboardProxy_LostFocus(object sender, RoutedEventArgs e)
		{
			// Trace($"Lost focus {hasKeyboardFocus}");
			hasKeyboardFocus = false;
			// CityBuild.ClearAction();
		}
	}
}
