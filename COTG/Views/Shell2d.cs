﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using COTG.Game;
using static COTG.Debug;
using COTG.Helpers;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Core;
using Microsoft.Graphics.Canvas.Svg;
using COTG.Services;
using Microsoft.Graphics.Canvas.Text;
using COTG.JSON;
using Microsoft.Graphics.Canvas.Effects;
using static COTG.Game.Enum;
using Windows.UI.Text;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.Graphics.Display;

namespace COTG.Views
{
	public partial class ShellPage
	{
		//	public static Rectangle canvasHitTest;



		const int bottomMargin = 0;
		const int cotgPopupWidth = 550;
		const int cotgPopupLeft = 438;
		const int cotgPopupRight = cotgPopupLeft + cotgPopupWidth;
		public const int canvasBaseX = 414;
		public const int canvasBaseY = 95;
		public static int cachedXOffset = 0;
		public static int cachedTopOffset = 0;
	
		static public SwapChainPanel canvas;

		public static void NotifyCotgPopup(int cotgPopupOpen)
		{
			//JSClient.CaptureWebPage(canvas);
			//	cotgPopupOpen = 0;
			var hasPopup = (cotgPopupOpen & 127) != 0;
			var hasLongWindow = cotgPopupOpen >= 128;
			var leftOffset = hasPopup ? cotgPopupRight - canvasBaseX : 0;
			var topOffset = hasLongWindow ? webclientSpan.y * 65 / 100 : canvasBaseY;

			// temp
			leftOffset = 0;
			topOffset = canvasBaseY;

			if (leftOffset == cachedXOffset && cachedTopOffset == topOffset)
				return;
			cachedTopOffset = topOffset;
			cachedXOffset = leftOffset;
			var _canvas = canvas;
			//	var _in = canvasHitTest;

			//  App.DispatchOnUIThreadLow(() => _grid.Margin = new Thickness(0, topOffset, 0, bottomMargin));
			App.DispatchOnUIThreadLow(() =>
			{
				_canvas.Margin = new Thickness(leftOffset , topOffset, 0, 0);
				//Canvas.SetLeft(_canvas, leftOffset);
				//Canvas.SetTop(_canvas, topOffset);
				//RemakeRenderTarget();
			});
			//            _grid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
			//            AUtil.Nop( (_grid.ColumnDefinitions[0].Width = new GridLength(leftOffset),
			//          _grid.ColumnDefinitions[1].Width = new GridLength(_grid.ColumnDefinitions[1].Width.Value-delta))));

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
				//	DpiScale = SettingsPage.dpiScale != 0 ? SettingsPage.dpiScale : (dpiLimit / DisplayInformation.GetForCurrentView().LogicalDpi).Min(1.0f),
				Name = "Region",
				IsHitTestVisible = true,
				Visibility = Visibility.Visible,
				//	IsTabStop = true,
				//	UseSharedDevice = true,
				//	TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f),
				Margin = new Thickness(0, canvasBaseY, 0, 0),
			//			IsFixedTimeStep = false
		};
			//canvasHitTest = new Rectangle()
			//{
			//	Name="webDrawer",
			//	IsHitTestVisible = true,
			//	 Opacity = 1,
			//	 Stretch=Stretch.Fill,

			//};

			//	canvas.Draw += Canvas_Draw;
			//	canvas.Update += Canvas_Update;

			canvas.SizeChanged += AGame.Canvas_SizeChanged;
			//canvas.LayoutUpdated += AGame.Canvas_LayoutUpdated;
			//canvas.CreateResources += Canvas_CreateResources;
			//			canvasHitTest.Margin=canvas.Margin = new Thickness(0, 0, 0, bottomMargin);
			//canvasHitTest.Stretch = Stretch.Fill;
			//  SetupCoreInput();
			canvas.CompositionScaleChanged += Canvas_CompositionScaleChanged;
			return (canvas, null);

		}

		public enum ViewMode
		{
			city = 0,
			region = 1,
			world = 2,
			invalid = 3
		};

		public static ViewMode viewMode;
		public static bool IsWorldView() => viewMode == ViewMode.world;
		public static bool IsCityView() => viewMode == ViewMode.city;

		public static bool webviewHasFocus;

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
						if (AGame.cameraZoom > AGame.cityZoomThreshold)
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
					ShellPage.isOverPopup = false;// reset again in case it changed
					ShellPage.canvas.IsHitTestVisible = ShellPage.isHitTestVisible;
					ShellPage.canvas.Visibility = !ShellPage.canvasVisible ? Visibility.Collapsed : Visibility.Visible;
					AGame.UpdateMusic();
					if (!webviewHasFocus && priorWebviewHasFocus)
						Verify(ShellPage.instance.commandBar.Focus(FocusState.Programmatic));

				});
			}
		}
		public static void SetWebViewHasFocus(bool _webviewHasFocus)
		{
			SetViewMode(viewMode, _webviewHasFocus);
		}
		public static void SetViewModeCity()
		{
			SetViewMode(ViewMode.city, webviewHasFocus);
		}
		public static void SetViewModeWorld() => SetViewMode(ViewMode.world, webviewHasFocus);


		private void Canvas_CompositionScaleChanged(SwapChainPanel sender, object args)
		{
			Log(canvas.CompositionScaleX);
		}
	}
}

