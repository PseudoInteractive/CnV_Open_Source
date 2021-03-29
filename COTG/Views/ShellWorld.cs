using COTG.Game;
using COTG.Helpers;
using COTG.Services;

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using static COTG.Debug;
using static COTG.AGame;
using Microsoft.Xna.Framework.Input;
using System.Collections.Concurrent;
using Windows.System.Threading;
using COTG.Draw;

namespace COTG.Views
{

	public partial class ShellPage
	{
		public static CoreIndependentInputSource coreInputSource;

		public static Vector2 mousePosition;
		public static Vector2 mousePositionC; // in camera space
		public static Vector2 mousePositionW; // in warped space
		public static Vector2 lastMousePressPosition;
		public static DateTimeOffset lastMousePressTime;

		public float eventTimeOffset;
		public static string toolTip;
		public static string contToolTip;
		public static string debugTip;
		public static int lastCont;
		


		public static void SetupCoreInput()
		{

		//	var workItemHandler = new WorkItemHandler((action) =>
			{
				var inputDevices = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch;
			//	Log(canvas.ManipulationMode);
			//	canvas.ManipulationMode = ManipulationModes.All;
				coreInputSource = canvas.CreateCoreIndependentInputSource(inputDevices);

			//	coreInputSource.InputEnabled += CoreInputSource_InputEnabled;
				coreInputSource.PointerMoved += Canvas_PointerMoved;
				coreInputSource.PointerPressed += Canvas_PointerPressed;
				coreInputSource.PointerReleased += Canvas_PointerReleased;
				coreInputSource.PointerEntered += CoreInputSource_PointerEntered;
				coreInputSource.PointerExited += Canvas_PointerExited;
				coreInputSource.PointerCaptureLost += CoreInputSource_PointerCaptureLost;

				coreInputSource.PointerWheelChanged += Canvas_PointerWheelChanged;
	//			coreInputSource.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessUntilQuit);
//				coreInputSource.IsInputEnabled = true;

			};
		//	var inputWorker = ThreadPool.RunAsync(workItemHandler, WorkItemPriority.High, WorkItemOptions.TimeSliced);


		}

		private static void CoreInputSource_InputEnabled(object sender, InputEnabledEventArgs args) {
			LogJson(args);
		}
		[Flags]
		public enum GestureAction
		{
			none=0,
			leftClick=1,
			rightClick=2,
			zoom=4,
			pan = 8,
			hover = 64, // mouse only
		}
		public static class Gesture
		{
			public class Point
			{
				public uint id;
				public Vector2 startC;
				public Vector2 c;
				public ulong startTimestamp;
			}
			public static Vector2 GetAveragePosition() => (from p in points select p.c).Average();
			public static Vector2 GetAverageStartPosition() => (from p in points select p.startC).Average();

			public static float GetStretch()
			{
				if (points.Count != 2)
					return 0;
				var d0 = (points[0].startC - points[1].startC).Length();
				var d1 = (points[0].c - points[1].c).Length();
				return d1 - d0;

			}
			static Vector2 lastDelta;
			public static float lastStretch;
			public static List<Point> points = new List<Point>();
			public static int maxPoints;
			public static GestureAction currentGesture;
			public static void Reset()
			{ 
				points.Clear();
				maxPoints = 0;
				currentGesture = GestureAction.none;
				lastDelta = new Vector2();
				lastStretch = 0;

			}
			public static (Vector2 c,bool process) ProcessPressed(Windows.UI.Input.PointerPoint point)
			{
				var c = GetCanvasPosition(point.Position);
					var id = point.PointerId;
					var pointer = (points.Find(p => p.id == id));
					Assert(pointer == null);
					if (pointer == null)
					{
						if (points.Any())
						{
							// cull second touces that occur more than 1s after the initial touch
							if (points[0].startTimestamp + 1UL * 1000UL * 1000UL < point.Timestamp)
							{
								return (c,false);
							}
						}
						pointer = new Point() { id = id, startTimestamp = point.Timestamp, startC = c, c = c };
						points.Add(pointer);
						maxPoints = points.Count.Max(maxPoints);
					//  reset
					if(points.Count > 1)
					{
						var cStart = GetAverageStartPosition();
						var cCurrent= GetAveragePosition();
						lastDelta = cCurrent - cStart;
						lastStretch = GetStretch();
						
					}
						return (GetAveragePosition(),true);
					}
					else
					{
						return (c,false);
					}
			}
			public static void Tick()
			{
				if (points.Any() && currentGesture == GestureAction.none)
				{
					var tick = (Environment.TickCount - (int)(points[0].startTimestamp/1000) );
					if(tick > 700)
					{
						// auto rick click on hold
						(var worldC, var cc) = ScreenToWorldAndCityC(mousePositionW);
						Reset();
						var cid = worldC.WorldToCid();
						App.DispatchOnUIThreadSneaky(RightClick(cc, cid));

					}
				}

			}

			public static void ProcessPointerExited(Windows.UI.Input.PointerPoint point)
			{
					var id = point.PointerId;
					var index = (points.FindIndex(p => p.id == id));
					if (index == -1)
					{
						return;
					}
					Reset();
			}

			public static (Vector2 c, Vector3 delta, GestureAction action) ProcessMoved(Windows.UI.Input.PointerPoint point)
			{
				var pointC = GetCanvasPosition(point.Position);
				var id = point.PointerId;
					var index = (points.FindIndex(p => p.id == id));
				if (index == -1)
				{
					if(!points.Any())
						return (pointC, new Vector3(), GestureAction.hover); // this is a mouse move without a press
					else
						return (pointC,new Vector3(), GestureAction.none); // this finger not tracked
				}
				// should not happen
				if (!point.IsInContact)
				{
					Assert(false);
					Reset();
					return (pointC, new Vector3(), GestureAction.hover); // this is a mouse move without a press
				}
				points[index].c = pointC;

					var cStart = GetAverageStartPosition();
					var c = GetAveragePosition();
				var dc = c - cStart;
				var stretch = GetStretch();
			//	if (currentGesture == GestureAction.none)
				{
					if (!currentGesture.HasFlag(GestureAction.zoom))
					{
						// stretch trumps pan
						if (stretch.Abs() > 8)
						{
							lastStretch = stretch;
							currentGesture |= GestureAction.zoom; // sticky bit
						}
					}
					if (!currentGesture.HasFlag(GestureAction.pan))
					{

						if (dc.Length() > 16)
						{
							lastDelta = dc;
							currentGesture |= GestureAction.pan; // sticky bit

						}
					}
				}
				if (currentGesture == GestureAction.none)
				{
					//if(index==0)
					//{
					//	if(point.Timestamp > points[0].startTimestamp * 500L*1000L)
					//	{
					//		Reset();
					//		return (c, new Vector3(), GestureAction.rightClick);
					//	}
					//}
					return (c, new Vector3(), GestureAction.none);
				}
				Vector3 delta = new Vector3();
				var gesture = GestureAction.none;
				if (currentGesture.HasFlag(GestureAction.zoom))
				{
					var dz = stretch - lastStretch;
					if (dz.Abs() >= 0.5f)
					{
						gesture |= GestureAction.zoom;
						lastStretch = stretch;
						delta.Z = dz;
					}
				}

				if (currentGesture.HasFlag(GestureAction.pan ))
				{
					var deltadelta = lastDelta - dc;
					
					lastDelta = dc;
					delta.X = deltadelta.X;
					delta.Y = deltadelta.Y;
					gesture |= GestureAction.pan;
				}
				return (c, delta,gesture); // this finger not tracked

			}
			
			public static (Vector2 c, GestureAction action) ProcessRelased(Windows.UI.Input.PointerPoint point)
			{
				var c = GetCanvasPosition(point.Position);

					var id = point.PointerId;
					var index = (points.FindIndex(p => p.id == id));
					if (index == -1)
					{
						Log("Error");
						return (c,GestureAction.none);
					}
					points[index].c = c;
					var rv = GetAveragePosition();
					
					{
						
					var result = (rv, GestureAction.leftClick);
					if (currentGesture != GestureAction.none)
						result=(rv, GestureAction.none);
					else if (maxPoints > 1 ||
						point.Properties.PointerUpdateKind == Windows.UI.Input.PointerUpdateKind.RightButtonReleased)
						result = (rv, GestureAction.rightClick);
					
					Reset();
					return result;
						// Actions trump presses

				}
					
			}
		}



		private static void CoreInputSource_PointerCaptureLost(object sender, PointerEventArgs args)
		{
			Log("pointer lost");
		
		}

		private static void CoreInputSource_PointerEntered(object sender, PointerEventArgs args)
		{
			isOverPopup = false;
			ShellPage.TakeKeyboardFocus();
		}


		/*
		public static void CanvasCheckKeys()
		{

			if (AGame.WasKeyPressed(Keys.Space))
				Spot.GetFocus().SelectMe(true, Windows.System.VirtualKeyModifiers.Control, true);


			if (AGame.WasKeyPressed(Keys.Left))
				Spot.SetFocus(Spot.focus.Translate((-1, 0)), true, true, true);

			if (AGame.WasKeyPressed(Keys.Up))
				Spot.SetFocus(Spot.focus.Translate((0, -1)), true, true, true);
			if (AGame.WasKeyPressed(Keys.Right))
				Spot.SetFocus(Spot.focus.Translate((1, 0)), true, true, true);
			if (AGame.WasKeyPressed(Keys.Down))
				Spot.SetFocus(Spot.focus.Translate((0, 1)), true, true, true);

		}
		*/
		//private void CoreInputSource_PointerEntered(object sender, PointerEventArgs args)
		//{
		// //   App.DispatchOnUIThreadLow(() => FocusManager.TryFocusAsync(canvas,FocusState.Programmatic));
		//}

		public static Vector2 GetCanvasPosition(Windows.Foundation.Point screenC)
		{
			var point = screenC;
			return new Vector2((float)(point.X*dipToNative), (float)(point.Y*dipToNative) );
		}
		public static Windows.Foundation.Point CanvasToScreen(Vector2 point)
		{
			return new Windows.Foundation.Point((point.X*nativeToDip)+ canvasBaseX, (point.Y*nativeToDip)+ canvasBaseY);
		}
		// to device independant position
		public static Windows.Foundation.Point CanvasToDIP(Vector2 point)
		{
			return new Windows.Foundation.Point((point.X * nativeToDip), (point.Y * nativeToDip));
		}

		public static void SetJSCamera()
		{
			//var cBase = halfSpan + clientC+halfSpan;
			//var c0 = cBase / cameraZoom;
			//var c1 = cBase / 64.0f;
			//var regionC = (cameraC + c0 - c1) * 64.0f;
			//    ShellPage.SetJSCamera(regionC);
		}

		public static void ClearHover()
		{
			if(!IsCityView())
				contToolTip = null;
			lastCanvasC = 0;
			lastCont = -1;
			toolTip = null;
			CityView.hovered = CanvasHelpers.invalidXY;
			Spot.viewHover = 0;
			Player.viewHover = 0;
		}

		static void PointerInfo(PointerEventArgs args, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
		{
			Log($"{memberName} : f:{args.CurrentPoint.FrameId} id:{args.CurrentPoint.PointerId} C:{args.CurrentPoint.IsInContact} t:{args.CurrentPoint.PointerDevice.PointerDeviceType} <{args.CurrentPoint.Position.X},{args.CurrentPoint.Position.Y}> ");
		}
		private static void Canvas_PointerExited(object sender, PointerEventArgs e)
		{
			Gesture.ProcessPointerExited(e.CurrentPoint);
		//	PointerInfo(e);
	//		Log("pointer Exit " + isOverPopup);
			isOverPopup = false;
			//if (ShellPage.IsCityView())
			//{
			//    e.Handled = false;
			//    return;
			//}


			ClearHover();
		}
		static public (int x, int y) ScreenToWorldC(Vector3 c1)
		{
			return (((c1.X) / cameraZoomLag + cameraC.X).RoundToInt(), ((c1.Y) / cameraZoomLag + cameraC.Y).RoundToInt());
		}
		static public ((int x, int y) wc, (int x, int y) cc) ScreenToWorldAndCityC(Vector2 c1)
		{
			var w = new Vector2(((c1.X) / cameraZoomLag + cameraC.X), ((c1.Y) / cameraZoomLag + cameraC.Y));
			(int x, int y) wi = (w.X.RoundToInt(), w.Y.RoundToInt());
			(int x, int y) bi = wi.WorldToCid() == City.build ?  
				(((w.X - wi.x)*City.citySpan).RoundToInt().Clamp(City.span0,City.span1), ((w.Y - wi.y) * City.citySpan/CityView.yScale).RoundToInt().Clamp(City.span0, City.span1)):
				CanvasHelpers.invalidXY;

			return (wi, bi);
		}

		//static public Vector2 CameraToWorld(Vector2 c1)
		//{
		//	return new Vector2( (c1.X-halfSpan.X)/cameraZoomLag + cameraC.X, (c1.Y - halfSpan.Y) / cameraZoomLag + cameraC.Y) ;
		//}

		static public int lastCanvasC;
		private void EventTimeTravelSliderChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			var dt = TimeSpan.FromMinutes(e.NewValue);
			var serverTime = JSClient.ServerTime() + TimeSpan.FromMinutes(e.NewValue);
			eventTimeTravelText.Text = $"Attack Time Travel:\t\t{dt.Hours}:{dt.Minutes},\t\tT:{serverTime.Hour}:{serverTime.Minute:D2}";
		}
		private static void Canvas_PointerReleased(object sender, PointerEventArgs e)
		{
			if (!isHitTestVisible)
				return;

			//if (JSClient.IsCityView())
			//{
			//	e.Handled = false;
			//	return;
			//}
			e.KeyModifiers.UpdateKeyModifiers();

			var pointerPoint = e.CurrentPoint;
			var gestureResult = Gesture.ProcessRelased(pointerPoint);
			if (gestureResult.action == GestureAction.none)
				return;

			mousePosition = gestureResult.c;
			mousePositionC = mousePosition.SToC();
			mousePositionW = mousePositionC.InverseProject();

			var wasOverPopup = isOverPopup;
			int jsButton = 0;
			//if(isOverPopup)
			//{
			//	jsButton = pointerPoint.Properties.PointerUpdateKind switch
			//	{
			//		Windows.UI.Input.PointerUpdateKind.LeftButtonReleased => 0,
			//		Windows.UI.Input.PointerUpdateKind.MiddleButtonReleased => 1,
			//		Windows.UI.Input.PointerUpdateKind.RightButtonReleased => 2,
			//	};

			//	PostJSMouseEvent("mouseup", jsButton);
			//	isOverPopup = false;
			//}
			

			//            mousePosition = point.Position.ToVector2();
			if (gestureResult.action == GestureAction.leftClick  || gestureResult.action == GestureAction.rightClick)
			{
				//if(wasOverPopup)
				//{
				//	PostJSMouseEvent("click", jsButton);
				//	return;
				//}
				
				(var worldC, var cc) = ScreenToWorldAndCityC(mousePositionW);
				var cid = worldC.WorldToCid();

				switch (gestureResult.action)
				{
					case GestureAction.leftClick:
						{
							if (IsCityView() && (cid == City.build))
							{
								App.DispatchOnUIThreadSneaky(() =>
								{
									CityBuild.Click(cc,false);

								});
							}
							else
							{
								// check to see if it needs to go to the webview
								Spot.ProcessCoordClick(cid, cid != City.build, e.KeyModifiers, true); ;
								e.Handled = true;
							}
							break;
						}

					case GestureAction.rightClick:
						{
							App.DispatchOnUIThreadSneaky(RightClick( cc, cid));
							break;
						}
					//case Windows.UI.Input.PointerUpdateKind.MiddleButtonReleased:
					//	{
					//		App.DispatchOnUIThreadSneaky(() =>
					//		{

					//			var spot = Spot.GetOrAdd(cid);

					//			var text = spot.ToTsv();
					//			Note.Show($"Copied to clipboard: {text}");
					//			App.CopyTextToClipboard(text);
					//			spot.SelectMe(true, App.keyModifiers);

					//		});
					//		break;
					//	}
					//case Windows.UI.Input.PointerUpdateKind.XButton1Released:
					//    {
					//        NavStack.Back();
					//    }
					//    break;
					//case Windows.UI.Input.PointerUpdateKind.XButton2Released:
					//    {
					//        NavStack.Forward();

					//        break;
					//    }
					default:
						break;
				}

			}
			else
			{
			}
		}

		private static DispatchedHandler RightClick( (int x, int y) cc, int cid)
		{
			return () =>
			{
				if (IsCityView() && (cid == City.build))
				{
					CityBuild.Click(cc, true);
				}
				else
				{

					var spot = Spot.GetOrAdd(cid);
					if (!App.IsKeyPressedShift())
						spot.SetFocus(true, true, false);
					spot.ShowContextMenu(canvas, CanvasToDIP(mousePosition));
					// }
				}
			};
		}

		static public bool isOverPopup;
		private static bool TryPostJSMouseEvent(string eventName, int button)
		{
			foreach (var popup in AGame.popups)
			{
				// should this be in DIPS or pixels?
				if (popup.Contains(mousePosition))
				{
					if(eventName!=null)
						JSClient.PostMouseEventToJS( (int)(AGame.nativeToDip * mousePosition.X) + canvasBaseX, (int)(AGame.nativeToDip * mousePosition.Y) + canvasBaseY, eventName, button );
					return true;
				}
			}
			return false;
		}

	


		private static void Canvas_PointerPressed(object sender, PointerEventArgs e)
		{
			//if (CityBuild.menuOpen)
			//{
			//	App.DispatchOnUIThreadSneaky(() => ShellPage.instance.buildMenu.IsOpen = false); // light dismiss
			//	return;
			//}

			if (!isHitTestVisible)
				return;
		
			Assert(isOverPopup == false);
			//            canvas.CapturePointer(e.Pointer);
			var point = e.CurrentPoint;

			var properties = point.Properties;
			var gestureResponse = Gesture.ProcessPressed(point);
			if (!gestureResponse.process)
				return;
			mousePosition = gestureResponse.c;
			mousePositionC = mousePosition.SToC();
			mousePositionW = mousePositionC.InverseProject();

			var prior = lastMousePressTime;
			lastMousePressTime = DateTimeOffset.UtcNow;
			lastMousePressPosition = mousePosition;

			mousePositionW = mousePositionC.InverseProject();
			(var c, var cc) = ScreenToWorldAndCityC(mousePositionW);

			//  if (JSClient.IsCityView())
			// The app pas priority over back and forward events
			{
				switch (properties.PointerUpdateKind)
				{
					case Windows.UI.Input.PointerUpdateKind.XButton1Pressed:
						e.Handled = true;
						NavStack.Back(true);
						ClearHover();
						return;
					case Windows.UI.Input.PointerUpdateKind.XButton2Pressed:
						e.Handled = true;
						NavStack.Forward(true);
						ClearHover();
						return;


				}
				//    e.Handled = false;
				//    return;
			}
			if (TryPostJSMouseEvent("click",
				properties.PointerUpdateKind switch
				{
					Windows.UI.Input.PointerUpdateKind.LeftButtonPressed => 0,
					Windows.UI.Input.PointerUpdateKind.MiddleButtonPressed => 1,
					
					Windows.UI.Input.PointerUpdateKind.RightButtonPressed => 2,
					_=>0
				}))
			{
				//				isOverPopup = true;
				e.Handled = true;
				Gesture.Reset();
				ShellPage.SetWebViewHasFocus(true);

			}
			else
			{
				// only needs for pen and touch
				if (IsCityView())
				{
					CityBuild.PointerDown(cc);
				}

				TakeKeyboardFocus();
			}

			//	ClearHover();
			//  e.Handled = false;

		}

	

		public static void Canvas_PointerPressedJS(int x, int y, Windows.UI.Input.PointerUpdateKind kind)
		{
			//e.KeyModifiers.UpdateKeyModifiers();

			Assert(isOverPopup == false);
			//            canvas.CapturePointer(e.Pointer);
			//	var point = e.CurrentPoint;

			//var properties = point.Properties;
			mousePosition = new Vector2(x, y);
			
			var prior = lastMousePressTime;
			lastMousePressTime = DateTimeOffset.UtcNow;
			lastMousePressPosition = mousePosition;


			//  if (ShellPage.IsCityView())
			// The app pas priority over back and forward events
			{
				switch (kind)
				{
					case Windows.UI.Input.PointerUpdateKind.XButton1Pressed:
						ClearHover();
						return;
					case Windows.UI.Input.PointerUpdateKind.XButton2Pressed:
						ClearHover();
						return;


				}
				//    e.Handled = false;
				//    return;
			}
			TakeKeyboardFocus();
			ClearHover();
			//  e.Handled = false;
			Gesture.Reset();
		}
		//private void Canvas_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
		//{
		//    mouseButtons = 0;
		//    Spot.viewHover = 0;
		//}
		private static bool AutoSwitchCityView()
		{
			var wc = cameraC.RoundToInt();
			var target = wc;
			float bestScore = float.MaxValue;
			// Try a different city
			
			
			
				for (int x = 0; x <= 0; ++x)
					for (int y = 0; y <= 0; ++y)
					{
						var dxy = (x, y);
						float lg = dxy.Length();
						if (lg > bestScore )
							continue;

						var probe = wc.Sum(dxy);

						if (City.CanVisit(probe.WorldToCid()))
						{
							target = probe;
							bestScore = dxy.Length();
							
						}
					}
				if (bestScore < float.MaxValue)
				{
					var cid = target.WorldToCid();
					if (cid != City.build)
						JSClient.ChangeCity(target.WorldToCid(), true);
					return true;
				}
				return false;
		}

		private static void Canvas_PointerWheelChanged(object sender, PointerEventArgs e)
		{
			//PointerInfo(e);
			//if (ShellPage.IsCityView())
			//{
			//    e.Handled = false;
			//    return;
			//}

			var pt = e.CurrentPoint;

			// wheel over javascript
			if (TryPostJSMouseEvent(null, 0))
			{
				//				isOverPopup = true;
				e.Handled = true;
				ShellPage.SetWebViewHasFocus(true);
				return;
			}
			DoZoom(pt.Properties.MouseWheelDelta,false);
		}

		static void DoZoom(float delta,bool skipPan)
		{ 

			
			var dZoom = delta.SignOr0() * 0.0f + delta * (1.0f / 256);
			var newZoom = (cameraZoom * MathF.Exp(dZoom)).Clamp(1, maxZoom);
			var cBase = new Vector2(); ////GetCanvasPosition(pt.Position) - halfSpan;

			var skipMove = skipPan;

			if ( IsCityView()  )
			{
				if (AutoSwitchCityView())
				{
					if (!skipPan)
					{
						cBase = (City.build.CidToWorldV() - cameraC) * cameraZoom;
						cameraC += 0.25f * (City.build.CidToWorldV() - cameraC); // nudge towards center
					}
				} 
			}
			else
			{
				skipMove = true;
			}


			if (!skipMove)
			{
				// when zooming in in city mode, constrain to city
				var c0 = cBase / cameraZoom;
				var c1 = cBase / newZoom;
				cameraC += c0 - c1;
			}
			var _viewMode =  newZoom >= cityZoomThreshold ? ViewMode.city: ViewMode.world;
			if(_viewMode !=viewMode)
			{
				ShellPage.SetViewMode(_viewMode);
			}

			cameraZoom = newZoom;
			
			ClearHover();
			//    ChatTab.L("CWheel " + wheel);
		}

		private static void Canvas_PointerMoved(object sender, PointerEventArgs e)
		{
			App.InputRecieved(); // prevent idle timer;
			//	PointerInfo(e);
			e.KeyModifiers.UpdateKeyModifiers();
			if (!isHitTestVisible)
				return;
		//	var priorMouseC = mousePosition;
			var gestureResult = Gesture.ProcessMoved(e.CurrentPoint);
			if (gestureResult.action == GestureAction.none)
				return;
			
			mousePosition = gestureResult.c;

			//			var windowsPosition = e.CurrentPoint.Position;
			//			mousePosition = GetCanvasPosition(windowsPosition);
			mousePositionC = mousePosition.SToC();
			mousePositionW = mousePositionC.InverseProject();
			(var c,var cc) = ScreenToWorldAndCityC(mousePositionW);
			//var point = e.CurrentPoint;
			//var props = point.Properties;
			if (gestureResult.action == GestureAction.hover)
			{
				if (TryPostJSMouseEvent(null, 0))
				{
					// mouse over popup
				}
				else
				{

					var cont = Continent.GetPackedIdFromC(c);
					var cid = c.WorldToCid();
					if (IsCityView())
					{
						var build = City.GetBuild();
						if (build != null)
						{
						//	var b = build.GetBuiding(cc);
							//var d = b.def;
							//	contToolTip = $"({cc.x},{cc.y})\n{d.Bn} {b.bl}";
							Spot.viewHover = 0;
							Player.viewHover = 0;
							toolTip = null;
							CityView.hovered = cc;
						}
					}
					else
					{

						if (cont != lastCont)
						{
							lastCont = cont;
							if (!IsCityView())
							{
								ref var cn = ref Continent.all[cont];
								contToolTip = $"{cn.id}\nSettled {cn.settled}\nFree {cn.unsettled}\nCities {cn.cities}\nCastles {cn.castles}\nTemples {cn.temples}\nDungeons {cn.dungeons}";

							}
						}


						if (lastCanvasC != cid)
						{

							Spot.viewHover = 0;
							Player.viewHover = 0;
							toolTip = null;

							lastCanvasC = cid;
							var packedId = World.GetPackedId(c);
							var data = World.GetInfoFromPackedId(packedId);
							switch (data.type)
							{
								case World.typeCity:
									{
										Spot.viewHover = cid;
										Spot.TryGet(cid, out var spot);

										if (data.player == 0)
										{
											toolTip = $"Lawless\n{c.y / 100}{c.x / 100} ({c.x}:{c.y})";
										}
										else
										{
											Player.viewHover = data.player;

											var player = Player.all.GetValueOrDefault(data.player, Player._default);
											if (Player.IsFriend(data.player))
											{
												if (spot is City city)
												{
													var notes = city.remarks.IsNullOrEmpty() ? "" : city.remarks.Substring(0, city.remarks.Length.Min(40)) + "\n";
													toolTip = $"{player.name}\n{city.cityName}\npts:{city.points:D0}\n{Alliance.IdToName(player.alliance)}\nTSh:{city.tsHome:D0}\nTSt:{city.tsTotal:D0}\n{notes}{c.y / 100}{c.x / 100} ({c.x}:{c.y})";
													//     Raiding.UpdateTS();
													if(city.senatorInfo.Length != 0)
													{
														toolTip = toolTip + city.GetSenatorInfo();
													}
													if(city.reinforcementsIn.Length > 0)
													{
														
														foreach(var i in city.reinforcementsIn)
														{
															toolTip = i.troops.Format($"{toolTip}\nFrom {City.GetOrAddCity(i.sourceCid).nameAndRemarks}:", '\n', '\n');
														}

													}
												}

											}
											else
											{
												var info = spot != null ?
													$"{spot.cityName}\n{spot.points}\n"
												 : "";
												toolTip = $"{player.name}\n{Alliance.IdToName(player.alliance)}\n{info}{c.y / 100}{c.x / 100} ({c.x}:{c.y})\ncities:{player.cities}\npts:{player.pointsH * 100}";
											}
										}
										if (spot != null && spot.incoming != null)
										{
											var inc = spot.incoming;
											foreach (var i in inc)
											{
												if (i.isAttack)
												{
													toolTip = toolTip + '\n' + i.Format("\n");
												}
											}
											var def = Array.Empty<TroopTypeCount>();
											foreach (var i in inc)
											{
												if (!i.isAttack)
												{
													def = def.Sum(i.troops);
												}
											}
											if (!def.IsNullOrEmpty())
											{
												toolTip += def.Format("\nDef:", '\n', '\n');
											}

										}
										break;
									}
								case World.typeShrine:
									toolTip = $"Shrine\n{(data.player == 255 ? "Unlit" : "Lit")}";
									break;
								case World.typeBoss:
									toolTip = $"Boss\nLevel:{data.player & 0xf}"; // \ntype:{data >> 4}";
									break;
								case World.typeDungeon:
									toolTip = $"Dungeon\nLevel:{data.player & 0xf}"; // \ntype:{data >> 4}";
									break;
								case World.typePortal:
									toolTip = $"Portal\n{(data.player == 0 ? "Inactive" : "Active")}";
									break;
							}

							if (World.rawPrior != null)
							{
								var pData = World.GetInfoPrior(packedId);
								if (pData.data == data.data | pData.type == World.typeBoss | pData.type == World.typeDungeon)
								{
									// no change

								}
								else
								{
									switch (data.type)
									{
										case World.typeCity:
											if (pData.type == World.typeCity)
											{
												if (pData.player != data.player)
												{
													if (pData.player == 0)
													{
														toolTip += "\nWas settled";
													}
													else if (data.player == 0)
													{
														var player = Player.all.GetValueOrDefault(pData.player, Player._default);
														toolTip += $"\nWas abandoned by:\n{player.name}\n{player.allianceName}";
													}
													else
													{
														var player = Player.all.GetValueOrDefault(pData.player, Player._default);
														toolTip += $"\nWas owned by:\n{player.name}\n{player.allianceName}";
													}
												}
												else
												{
													if (pData.isTemple != data.isTemple)
													{
														if (data.isTemple)
															toolTip += "\nBecame a Temple";
														else
															toolTip += "\nBecame not a temple";
													}
													else if (pData.isCastle != data.isCastle)
													{
														toolTip += "\nWas castled";
													}
													else
													{
														toolTip += "\nWas rennovated";
													}
												}
											}
											else
											{
												toolTip += "\nWas founded";
											}
											break;
										case World.typeShrine:
											toolTip += "\nWas unlit";
											break;
										case World.typePortal:
											if (data.player == 0)
												toolTip += "\nWas active";
											else
												toolTip += "\nWas inactive";
											break;
										default:
											if (pData.player != 0)
												toolTip += $"\nDecayed (was {Player.IdToName(pData.player)})";
											else
												toolTip += "\nDecayed";
											break;

									}
								}
							}
						}
					}
				}
				e.Handled = false;

			}
			else if(gestureResult.action==GestureAction.rightClick)
			{
				var cid = c.WorldToCid();
				
				App.DispatchOnUIThreadSneaky(RightClick(cc, cid));
			}
			else
			{
				e.Handled = true;
				if (gestureResult.action.HasFlag(GestureAction.zoom))
				{
					DoZoom(gestureResult.delta.Z * 0.75f, gestureResult.action.HasFlag(GestureAction.pan));
					cameraZoomLag = cameraZoom;
				}
				if (gestureResult.action.HasFlag(GestureAction.pan))
				{
					var dr = gestureResult.delta;
					{
						dr *= 1.0f / cameraZoomLag;
						cameraC += dr.ToV2();
						// instant
						cameraCLag = cameraC;
						if (IsCityView())
						{
							AutoSwitchCityView();
						}
					}
					//else
					{

						//		PostJSMouseEvent("mousemove",0, (int)dr.X,(int)dr.Y);
					}
				}

			}
		}

	}
}
