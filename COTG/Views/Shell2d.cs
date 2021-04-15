using System;
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
using Windows.System;
using Windows.UI.Text;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.Graphics.Display;
using COTG.Draw;
using Windows.UI.Xaml.Input;
using static COTG.Views.CityBuild;
using static COTG.Game.City;
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
		public static KeyboardProxy keyboardProxy;
		public static bool hasKeyboardFocus;
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
			keyboardProxy = new KeyboardProxy()
			{
				AllowFocusOnInteraction = true,
				TabFocusNavigation=KeyboardNavigationMode.Cycle,
				Background = null,
				IsTabStop = true
			};
			keyboardProxy.AddHandler(KeyDownEvent, new KeyEventHandler(KeyboardProxy_KeyDown),true);
			keyboardProxy.LostFocus += KeyboardProxy_LostFocus;
			keyboardProxy.GettingFocus += KeyboardProxy_GettingFocus;
			canvas.Children.Add(keyboardProxy);
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


		public static void TakeKeyboardFocus()
		{

		//	if (hasKeyboardFocus)
		//		return;
		//	Trace($"Take focus {hasKeyboardFocus}");
			// Set this early, it gets set again once the asyn executes
			hasKeyboardFocus = true;
			App.DispatchOnUIThreadLow(()=>
			{
				keyboardProxy.Focus(FocusState.Programmatic);
			});
		}
		private void KeyboardProxy_GettingFocus(UIElement sender, Windows.UI.Xaml.Input.GettingFocusEventArgs args)
		{
			//Log("Get focus");
		//	Trace($"Got focus {hasKeyboardFocus}");
			hasKeyboardFocus =  true;

		}
		private void KeyboardProxy_LostFocus(object sender, RoutedEventArgs e)
		{
		//	Trace($"Lost focus {hasKeyboardFocus}");
			hasKeyboardFocus = false;
			//			CityBuild.ClearAction();
		}

		static void UpgradeOrTower(int number)
		{
			var xy = CityView.hovered;
			var spot = City.XYToId(xy);
			if (CityBuild.IsTowerSpot(spot) && CityBuild.postQueueBuildings[spot].bl == 0)
			{
				var bid = number switch
				{
					1 => City.bidSentinelPost,
					2 => bidRangerPost,
					3 => bidTriariPost,
					4 => bidPriestessPost,
					5 => bidBallistaPost,
					6 => bidSnagBarricade,
					7 => bidEquineBarricade,
					8 => bidRuneBarricade,
					_ => bidVeiledBarricade
				};
				 
				ShortBuild(bid);
			}
			else
			{
				CityBuild.UpgradeToLevel(number, CityView.hovered);

			}
		}

		private void KeyboardProxy_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			App.InputRecieved();
			//if (CityBuild.menuOpen)
			//{	
			//	// todo:  Handle naviation menu items and selection
			//	App.DispatchOnUIThreadSneaky(() =>
			//	{
			//		ShellPage.instance.buildMenu.IsOpen = false;
			//	});
			//	return;

			//}
			var key = e.Key;
			if (IsWorldView())
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
				//	App.DispatchOnUIThreadSneaky(() =>
				{
					switch (key)
					{
						case VirtualKey.Space: CityBuild.Click(CityView.hovered, true); return;
						case VirtualKey.Enter: CityBuild.Click(CityView.hovered, false); return;
						case Windows.System.VirtualKey.Left:
							if (CityView.hovered.IsValid())
								CityView.hovered.x = (CityView.hovered.x - 1).Max(City.span0);
							else
								CityView.hovered = (0, 0);

							break;
						case Windows.System.VirtualKey.Up:
							if (CityView.hovered.IsValid())
								CityView.hovered.y = (CityView.hovered.y - 1).Max(City.span0);
							else
								CityView.hovered = (0, 0);
							break;
						case Windows.System.VirtualKey.Right:
							if (CityView.hovered.IsValid())
								CityView.hovered.x = (CityView.hovered.x + 1).Min(City.span1);
							else
								CityView.hovered = (0, 0);

							break;
						case Windows.System.VirtualKey.Down:
							if (CityView.hovered.IsValid())
								CityView.hovered.y = (CityView.hovered.y + 1).Min(City.span1);
							else
								CityView.hovered = (0, 0);
							break;
						
						case Windows.System.VirtualKey.Number1: UpgradeOrTower(1); break;
						case Windows.System.VirtualKey.Number2: UpgradeOrTower(2); break;
						case Windows.System.VirtualKey.Number3: UpgradeOrTower(3); break;
						case Windows.System.VirtualKey.Number4: UpgradeOrTower(4); break;
						case Windows.System.VirtualKey.Number5: UpgradeOrTower(5); break;
						case Windows.System.VirtualKey.Number6: UpgradeOrTower(6); break;
						case Windows.System.VirtualKey.Number7: UpgradeOrTower(7); break;
						case Windows.System.VirtualKey.Number8: UpgradeOrTower(8); break;
						case Windows.System.VirtualKey.Number9: UpgradeOrTower(9); break;
						case Windows.System.VirtualKey.Number0: CityBuild.UpgradeToLevel(10, CityView.hovered); break;
						case Windows.System.VirtualKey.U: CityBuild.UpgradeToLevel(1, CityView.hovered, false); break;
						//			case Windows.System.VirtualKey.Q: CityBuild.ClearQueue(); break;
						case Windows.System.VirtualKey.D: CityBuild.Demolish(CityView.hovered, false); break;
						case Windows.System.VirtualKey.Escape: CityBuild.ClearAction(); break;
						case (VirtualKey)192:
							{
								if (action == CityBuild.Action.moveEnd)
									CityBuild.MoveHovered(true, false, false);
								else
								{

									CityView.ClearSelectedBuilding();
									CityBuild.MoveHovered(true, true, false);
								}
								break; //  (City.XYToId(CityView.selected), City.XYToId(CityView.hovered)); break;
							}
						// short keys
						case Windows.System.VirtualKey.F: CityBuild.ShortBuild(City.bidForester); return; //  448;
						case Windows.System.VirtualKey.C: CityBuild.ShortBuild(City.bidCottage); return; //  446;
						case Windows.System.VirtualKey.R: CityBuild.ShortBuild(City.bidStorehouse); return; //  464;
						case Windows.System.VirtualKey.S: CityBuild.ShortBuild(City.bidQuarry); return; //  461;
																										//		case Windows.System.VirtualKey.Q  :  CityBuild.ShortBuild(City.bidHideaway ); return; //  479;
						case Windows.System.VirtualKey.A: CityBuild.ShortBuild(City.bidFarmhouse); return; //  447;
																										   //	case Windows.System.VirtualKey.U  :  CityBuild.ShortBuild(City.bidCityguardhouse ); return; //  504;
						case Windows.System.VirtualKey.B: CityBuild.ShortBuild(City.bidBarracks); return; //  445;
						case Windows.System.VirtualKey.I: CityBuild.ShortBuild(City.bidMine); return; //  465;
						case Windows.System.VirtualKey.T: CityBuild.ShortBuild(City.bidTrainingGround); return; //  483;
						case Windows.System.VirtualKey.M: CityBuild.ShortBuild(City.bidMarketplace); return; //  449;
						case Windows.System.VirtualKey.V: CityBuild.ShortBuild(City.bidTownhouse); return; //  481;
						case Windows.System.VirtualKey.L: CityBuild.ShortBuild(City.bidSawmill); return; //  460;
						case Windows.System.VirtualKey.E: CityBuild.ShortBuild(City.bidStable); return; //  466;
						case Windows.System.VirtualKey.H: CityBuild.ShortBuild(City.bidStonemason); return; //  462;
						case Windows.System.VirtualKey.W: CityBuild.ShortBuild(City.bidSorcTower); return; //  500;
						case Windows.System.VirtualKey.G: CityBuild.ShortBuild(City.bidWindmill); return; //  463;
						case Windows.System.VirtualKey.Y: CityBuild.ShortBuild(City.bidAcademy); return; //  482;
						case Windows.System.VirtualKey.Z: CityBuild.ShortBuild(City.bidSmelter); return; //  477;
						case Windows.System.VirtualKey.K: CityBuild.ShortBuild(City.bidBlacksmith); return; //  502;
						case Windows.System.VirtualKey.X: CityBuild.ShortBuild(City.bidCastle); return; //  467;
						case Windows.System.VirtualKey.O: CityBuild.ShortBuild(City.bidPort); return; //  488;
						case Windows.System.VirtualKey.P: CityBuild.ShortBuild(City.bidShipyard); return; //  491;

						default:
							break;
					}
				}
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
					if (Player.isAvatarOrTest)
					{
						CityBuild.testFlag ^= true;
						Note.Show("Test: " + testFlag);
						JSClient.view.InvokeScriptAsync("setTestFlag", new[] { (testFlag ? "1" : "0") });
					}
					break;
			}
			
						//	});
					

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
						if (AGame.cameraZoom > AGame.cityZoomWorldThreshold )
							AGame.cameraZoom = AGame.cameraZoomWorldDefault;
					}
					else if (viewMode == ViewMode.region)
					{
						if (AGame.cameraZoom > AGame.cityZoomThreshold || AGame.cameraZoom < AGame.cityZoomWorldThreshold )
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
		public static void SetWebViewHasFocus(bool _webviewHasFocus)
		{
			SetViewMode(viewMode, _webviewHasFocus);
		}
		public static void SetViewModeCity()
		{
			SetViewMode(ViewMode.city, webviewHasFocus);
		}
		public static void SetViewModeWorld() => SetViewMode(ViewMode.world, webviewHasFocus);
		public static void SetViewModeRegion() => SetViewMode(ViewMode.region, webviewHasFocus);


		private void Canvas_CompositionScaleChanged(SwapChainPanel sender, object args)
		{
			Log(canvas.CompositionScaleX);
		}
	}
	public class KeyboardProxy : Control
	{

	}
}

