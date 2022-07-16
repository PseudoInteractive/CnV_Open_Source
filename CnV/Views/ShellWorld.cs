using CnV.Game;
using CnV.Helpers;
using CnV.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
//using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using static CnV.Debug;
using static CnV.AGame;
using Microsoft.Xna.Framework.Input;
using System.Collections.Concurrent;
using Windows.System.Threading;
using CnV.Draw;
using static CnV.Helpers.AString;
using static CnV.GameClient;
using Microsoft.UI.Input;
using Microsoft.UI.Dispatching;
using static CnV.View;
using static CnV.ClientView;
//using PointerEventArgs = Microsoft.UI.Input.Experimental.ExpPointerEventArgs;
//using PointerPoint = Microsoft.UI.Input.Experimental.ExpPointerPoint;
//using PointerUpdateKind = Windows.UI.Input.PointerUpdateKind;
//using Windows.UI.Core;
//using Microsoft.UI.Input.Experimental;
using CnV;
//using InputPointerSource = ;//Microsoft.UI.Input.Experimental.expin;
namespace CnV.Views
{
	using Draw;

	using Game;

	using Helpers;

	using Windows.Foundation;

	public partial class ShellPage
	{
		static bool hasCapture;

		public static Vector2 mousePosition;
		//public static Vector2 mousePositionC; // in camera space
		public static Vector2 mousePositionW; // in world space

		internal static void SetMousePosition(Vector2 c)
		{
			mousePosition = c;
			mousePositionW = c.ScreenToWorld();
			
		}
		//public static Vector2 lastMousePressPosition;
		//public static DateTimeOffset lastMousePressTime;

		public static bool mouseOverCanvas;
	//	static Thread inputWorker;

		public float eventTimeOffset;
	//	public static ref string? contToolTip => ref ToolTips.contToolTip;
		//	public static DispatcherQueueController _queuecontroller;
		///ivate static InputPointerSource _inputPointerSource;

		
		




		//		
		//	




		//private static void FocusManager_LostFocus(object? sender,FocusManagerLostFocusEventArgs e)
		//{
		//	//	Note.Show($"Lost focus: {e.OldFocusedElement} { (e.OldFocusedElement as FrameworkElement)?.Name}");
		//}

		//private static void FocusManager_GotFocus(object? sender,FocusManagerGotFocusEventArgs e)
		//{
		//	//	Note.Show($"New focus: {e.NewFocusedElement} { (e.NewFocusedElement as FrameworkElement)?.Name}");
		//}

		//private static void KeyboardProxy_PointerExited(object sender,PointerRoutedEventArgs e)
		//{
		//	e.KeyModifiers.UpdateKeyModifiers();
		//	//	Note.Show($"Pointer exit {mouseOverCanvas}");
		//	//	Assert(mouseOverCanvas== true);
		//	mouseOverCanvas = false;
		//	//	instance.mouseOverCanvasBox.IsChecked = mouseOverCanvas;

		//	var point = e.GetCurrentPoint(canvas);
		//	Canvas_PointerExited(point.Position,point.PointerId);
		//}

		//private static void KeyboardProxy_PointerEntered(object sender,PointerRoutedEventArgs e)
		//{
		//	e.KeyModifiers.UpdateKeyModifiers();

		//	Canvas_PointerEntered(e.GetCurrentPoint(canvas).Position);
		//}

		//private static void KeyboardProxy_PointerReleased(object sender,PointerRoutedEventArgs e)
		//{
		//	e.KeyModifiers.UpdateKeyModifiers();

		//	var point = e.GetCurrentPoint(canvas);
		//	Canvas_PointerReleased((point.Position, point.PointerId, point.IsInContact, point.Timestamp, point.Properties.PointerUpdateKind),e.KeyModifiers);
		//	e.Handled=true;

		//}

		//private static void KeyboardProxy_PointerPressed(object sender,PointerRoutedEventArgs e)
		//{
		//	var point = e.GetCurrentPoint(canvas);
		//	e.KeyModifiers.UpdateKeyModifiers();
		//	App.InputRecieved();
		//	Canvas_PointerPressed((point.Position, point.PointerId, point.IsInContact, point.Timestamp, point.Properties.PointerUpdateKind));
		//	e.Handled=true;
		//}

		//private static void KeyboardProxy_PointerMoved(object sender,PointerRoutedEventArgs e)
		//{
		//	e.KeyModifiers.UpdateKeyModifiers();

		//	var point = e.GetCurrentPoint(canvas);
		//	Canvas_PointerMoved((point.Position, point.PointerId, point.IsInContact, point.Timestamp, point.Properties.PointerUpdateKind));
		//}

		//public static DispatcherQueueHandler RightClick((int x, int y) cc,int cid)
		//{
		//	return () =>
		//	{
		//		if(IsCityView() && (cid == City.build))
		//		{
		//			CityBuild.Click(cc,true);
		//		}
		//		else
		//		{

		//			var spot = Spot.GetOrAdd(cid);
		//			if(!AppS.IsKeyPressedShiftOrControl())
		//				spot.SetFocus(true,true,false);
		//			spot.ShowContextMenu(canvas,CanvasToDIP(mousePosition));
		//			// }
		//		}
		//	};
		//}

		//private static void CoreInputSource_InputEnabled(object sender, InputEnabledEventArgs args) {
		//	LogJson(args);
		//}
		//[Flags]
		//public enum GestureAction
		//{
		//	none = 0,
		//	leftClick = 1,
		//	rightClick = 2,
		//	zoom = 4,
		//	pan = 8,
		//	middleClick = 16,
		//	hover = 64, // mouse only
		//}
		//public static class Gesture
		//{
		//	public class Point
		//	{
		//		public uint id;
		//		public Vector2 startC;
		//		public Vector2 c;
		//		public ulong startTimestamp;
		//	}
		//	public static Vector2 GetAveragePosition() => (from p in points select p.c).Average();
		//	public static Vector2 GetAverageStartPosition() => (from p in points select p.startC).Average();

		//	public static float GetStretch()
		//	{
		//		if(points.Count != 2)
		//			return 0;
		//		var d0 = (points[0].startC - points[1].startC).Length();
		//		var d1 = (points[0].c - points[1].c).Length();
		//		return d1 - d0;

		//	}
		//	static Vector2 lastDelta;
		//	public static float lastStretch;
		//	public static List<Point> points = new List<Point>();
		//	public static int maxPoints;
		//	public static GestureAction currentGesture;
		//	public static void Reset()
		//	{
		//		points.Clear();
		//		maxPoints = 0;
		//		currentGesture = GestureAction.none;
		//		lastDelta = new Vector2();
		//		lastStretch = 0;

		//	}
		//	public static (Vector2 c, bool process) ProcessPressed((Windows.Foundation.Point Position, uint PointerId,
		//																	bool IsInContact, ulong Timestamp, PointerUpdateKind PointerUpdateKind) point)
		//	{
		//		var c = CanvasPointFromDip(point.Position);
		//		var id = point.PointerId;
		//		// hack!  something here is broken
		//		Reset();
		//		var pointer = (points.Find(p => p.id == id));
		//		Assert(pointer == null);
		//		if(pointer == null)
		//		{
		//			if(points.Any())
		//			{
		//				// cull second touches that occur more than 1s after the initial touch
		//				if((points[0].startTimestamp + 1UL * 1000UL * 1000UL < point.Timestamp) &&
		//					 (points[0].startTimestamp + 10UL * 1000UL * 1000UL > point.Timestamp)) // only if less than 10s
		//				{
		//					return (c, false);
		//				}
		//			}
		//			pointer = new Point() { id = id,startTimestamp = point.Timestamp,startC = c,c = c };
		//			points.Add(pointer);
		//			maxPoints = points.Count.Max(maxPoints);
		//			//  reset
		//			if(points.Count > 1)
		//			{
		//				var cStart = GetAverageStartPosition();
		//				var cCurrent = GetAveragePosition();
		//				lastDelta = cCurrent - cStart;
		//				lastStretch = GetStretch();

		//			}
		//			return (GetAveragePosition(), true);
		//		}
		//		else
		//		{
		//			return (c, false);
		//		}
		//	}
		//	//public static void Tick()
		//	//{
		//	//	if (points.Any() && currentGesture == GestureAction.none)
		//	//	{
		//	//		var tick = (Environment.TickCount - (int)(points[0].startTimestamp/1000) );
		//	//		if(tick > 700)
		//	//		{
		//	//			// auto rick click on hold
		//	//			(var worldC, var cc) = ScreenToWorldAndCityC(mousePositionW);
		//	//			Reset();
		//	//			var cid = worldC.WorldToCid();
		//	//			AppS.DispatchOnUIThreadLow(RightClick(cc, cid));

		//	//		}
		//	//	}

		//	//}

		//	public static void ProcessPointerExited(uint PointerId)
		//	{
		//		var id = PointerId;
		//		var index = (points.FindIndex(p => p.id == id));
		//		if(index == -1)
		//		{
		//			return;
		//		}
		//		Reset();
		//	}

		//	public static (Vector2 c, Vector3 delta, GestureAction action) ProcessMoved((Windows.Foundation.Point Position, uint PointerId,
		//																	bool IsInContact, ulong Timestamp, PointerUpdateKind PointerUpdateKind) point)
		//	{
		//		var pointC = CanvasPointFromDip(point.Position);
		//		var id = point.PointerId;
		//		var index = (points.FindIndex(p => p.id == id));
		//		if(index == -1)
		//		{
		//			if(!points.Any())
		//				return (pointC, new Vector3(), GestureAction.hover); // this is a mouse move without a press
		//			else
		//				return (pointC, new Vector3(), GestureAction.none); // this finger not tracked
		//		}
		//		// should not happen
		//		if(!point.IsInContact)
		//		{
		//			//Assert(false);
		//			Reset();
		//			return (pointC, new Vector3(), GestureAction.hover); // this is a mouse move without a press
		//		}
		//		points[index].c = pointC;

		//		var cStart = GetAverageStartPosition();
		//		var c = GetAveragePosition();
		//		var dc = c - cStart;
		//		var stretch = GetStretch();
		//		if(currentGesture == GestureAction.none)
		//		{
		//			if(!currentGesture.HasFlag(GestureAction.zoom))
		//			{
		//				// stretch trumps pan
		//				if(stretch.Abs() > 8)
		//				{
		//					lastStretch = stretch;
		//					currentGesture |= GestureAction.zoom; // sticky bit
		//				}
		//			}
		//			if(!currentGesture.HasFlag(GestureAction.pan))
		//			{

		//				if(dc.Length() > 16)
		//				{
		//					lastDelta = dc;
		//					currentGesture |= GestureAction.pan; // sticky bit

		//				}
		//			}
		//		}
		//		if(currentGesture == GestureAction.none)
		//		{
		//			//if(index==0)
		//			//{
		//			//	if(point.Timestamp > points[0].startTimestamp * 500L*1000L)
		//			//	{
		//			//		Reset();
		//			//		return (c, new Vector3(), GestureAction.rightClick);
		//			//	}
		//			//}
		//			return (c, new Vector3(), GestureAction.none);
		//		}
		//		Vector3 delta = new Vector3();
		//		var gesture = GestureAction.none;
		//		if(currentGesture.HasFlag(GestureAction.zoom))
		//		{
		//			var dz = stretch - lastStretch;
		//			if(dz.Abs() >= 0.5f)
		//			{
		//				gesture |= GestureAction.zoom;
		//				lastStretch = stretch;
		//				delta.Z = dz;
		//			}
		//		}

		//		if(currentGesture.HasFlag(GestureAction.pan))
		//		{
		//			var deltadelta = lastDelta - dc;

		//			lastDelta = dc;
		//			delta.X = deltadelta.X;
		//			delta.Y = deltadelta.Y;
		//			gesture |= GestureAction.pan;
		//		}
		//		return (c, delta, gesture); // this finger not tracked

		//	}

		//	public static (Vector2 c, GestureAction action) ProcessRelased((Windows.Foundation.Point Position, uint PointerId,
		//																	bool IsInContact, ulong Timestamp, PointerUpdateKind PointerUpdateKind) point)
		//	{
		//		var c = CanvasPointFromDip(point.Position);

		//		var id = point.PointerId;
		//		var index = (points.FindIndex(p => p.id == id));
		//		if(index == -1)
		//		{
		//			Log("Error");
		//			return (c, GestureAction.none);
		//		}
		//		points[index].c = c;
		//		var rv = GetAveragePosition();

		//		{

		//			var result = (rv, point.PointerUpdateKind == PointerUpdateKind.MiddleButtonReleased ? GestureAction.middleClick : GestureAction.leftClick);
		//			if(currentGesture != GestureAction.none)
		//				result=(rv, GestureAction.none);
		//			else if(maxPoints > 1 ||
		//				point.PointerUpdateKind == PointerUpdateKind.RightButtonReleased)
		//				result = (rv, GestureAction.rightClick);

		//			Reset();
		//			return result;
		//			// Actions trump presses

		//		}

		//	}
		//}



		//private static void CoreInputSource_PointerCaptureLost(InputPointerSource sender,PointerEventArgs args)
		//{
		//	Log("pointer lost");

		//}

		private static void Canvas_PointerEntered(Windows.Foundation.Point args)
		{
//			if(!hasCapture)
//				args.cap
			//	Assert(mouseOverCanvas== false);
			//	Note.Show($"Pointer entered {mouseOverCanvas}");
			if(!mouseOverCanvas)
			{
				mouseOverCanvas = true;
				//	instance.mouseOverCanvasBox.IsChecked = mouseOverCanvas;

				//Note.Show("MouseEnterred");
			}
			UpdateMousePosition(args);
		//	Debug.Log($"Mouse pos: {mousePositionW}");
			TakeFocus();

			//			Log($"!Focus11: {hasKeyboardFocus} w{webviewHasFocus} w2{webviewHasFocus2}");
			//			args.KeyModifiers.UpdateKeyModifiers();
			//ShellPage.UpdateFocus(); 
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
		// //   App.(() => FocusManager.TryFocusAsync(canvas,FocusState.Programmatic));
		//}

		//public static Vector2 CanvasPointFromDip(Windows.Foundation.Point screenC)
		//{
		//	return new Vector2((float)(screenC.X),(float)(screenC.Y));
		//}
		//public static Vector2 GetCanvasPosition( int x , int y )
		//{
		//	return new Vector2((float)(x * dipToNative), (float)(y * dipToNative));
		//}
		//public static Windows.Foundation.Point CanvasToScreen(Vector2 point)
		//{
		//	return new Windows.Foundation.Point((point.X*nativeToDip)+ canvasBaseX, (point.Y*nativeToDip)+ canvasBaseY);
		//}
		// to device independant position
		public static Windows.Foundation.Point CanvasToDIP(Vector2 point)
		{
			return new Windows.Foundation.Point((point.X),(point.Y));
		}

		//public static (int x, int y) JSPointToScreen((int x, int y) c) => JSPointToScreen(c.x, c.y);
		//public static (int x, int y) JSPointToScreen(int x, int y)
		//{
		//	var dipToNative = AGame.dipToNative;
		//	return ((dipToNative * (x * ShellPage.webViewScale.X - ShellPage.canvasBaseX)).RoundToInt(),
		//			(dipToNative * (y * ShellPage.webViewScale.Y - ShellPage.canvasBaseY)).RoundToInt());

		//}

		static void PointerInfo(PointerEventArgs args,[System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
		{
			Log($"{memberName} : f:{args.CurrentPoint.FrameId} id:{args.CurrentPoint.PointerId} C:{args.CurrentPoint.IsInContact} l{args.CurrentPoint.Position.X},{args.CurrentPoint.Position.Y}> ");
		}
		private static void Canvas_PointerExited(Windows.Foundation.Point Point,uint PointerId)
		{
			UpdateMousePosition(Point);
			if(mouseOverCanvas) {
				//Note.Show("Mouse exited");
				mouseOverCanvas = false;
			}
			//			e.KeyModifiers.UpdateKeyModifiers();
			//Gesture.ProcessPointerExited(PointerId);
			//	PointerInfo(e);
			//		Log("pointer Exit " + isOverPopup);
			//if (ShellPage.IsCityView())
			//{
			//    e.Handled = false;
			//    return;
			//}
			//	Log($"!FocusExit: {hasKeyboardFocus} w{webviewHasFocus} w2{webviewHasFocus2}");
			//	hasKeyboardFocus=0;
			//UpdateFocus();

			ClearHover();
		}
		//static public (int x, int y) ScreenToWorldC(Vector3 c1)
		//{
		//	return (((c1.X) / viewZoomLag + viewTargetW.X).RoundToInt(), ((c1.Y) / viewZoomLag + viewTargetW.Y).RoundToInt());
		//}
		static public (WorldC wc, BuildC cc) ToWorldAndCityC(Vector2 w)
		{
			//var w = new Vector2(((sc.X)  + viewTargetW.X),((sc.Y) + viewTargetW.Y));
			if(!w.IsInWorld())
				return (WorldC.Nan, BuildC.Nan);

			var wi = new WorldC(w);


			(int x, int y) bi = wi.cid == City.build ?
				(((w.X - wi.x)*City.citySpan).RoundToInt().Clamp(City.span0,City.span1), ((w.Y - wi.y) * City.citySpan/CityView.cityYAspectRatio).RoundToInt().Clamp(City.span0,City.span1)) :
				BuildC.Nan;

			return (wi, bi);
		}

		//static public Vector2 CameraToWorld(Vector2 c1)
		//{
		//	return new Vector2( (c1.X-halfSpan.X)/cameraZoomLag + viewCW.X, (c1.Y - halfSpan.Y) / cameraZoomLag + viewCW.Y) ;
		//}

		static public ref int lastCanvasC => ref View.lastCanvasC;
		//private void EventTimeTravelSliderChanged(object sender, RangeBaseValueChangedEventArgs e)
		//{
		//	var dt = TimeSpan.FromMinutes(e.NewValue);
		//	var serverTime = CnVServer.serverTime + TimeSpan.FromMinutes(e.NewValue);
		//	eventTimeTravelText.Text = $"Attack Time Travel:\t\t{dt.Hours}:{dt.Minutes},\t\tT:{serverTime.Format()}";
		//}

		/// <summary>
		/// <inheritdoc 
		///     cref="Bookmarks" />
		/// />
		/// </summary>
		/// <param name="point"></param>
		/// <param name="keyModifiers"></param>
		//private static void Canvas_PointerReleased((Windows.Foundation.Point Position, uint PointerId,
		//																	bool IsInContact, ulong Timestamp, PointerUpdateKind PointerUpdateKind
		//																	) point,
		//	Windows.System.VirtualKeyModifiers keyModifiers)
		//{

		//	keyModifiers.UpdateKeyModifiers();
		//	UpdateMousePosition(point.Position);

		//	//	if(!isFocused)
		//	//		return;

		//	//if (CnVServer.IsCityView())
		//	//{
		//	//	e.Handled = false;
		//	//	return;
		//	//}

		//	//		var pointerPoint = e.CurrentPoint;
		//	var gestureResult = Gesture.ProcessRelased(point);
		//	if(gestureResult.action == GestureAction.none)
		//		return;
		//	// why do this trigger gestures?
		//	switch(point.PointerUpdateKind)
		//	{
		//		case PointerUpdateKind.XButton1Released:
		//		case PointerUpdateKind.XButton2Released:
		//			return;
		//	}
		//	SetMousePosition( gestureResult.c);


		//	//	var wasOverPopup = isOverPopup;
		//	int jsButton = 0;
		//	//if(isOverPopup)
		//	//{
		//	//	jsButton = pointerPoint.Properties.PointerUpdateKind switch
		//	//	{
		//	//		Windows.UI.Input.PointerUpdateKind.LeftButtonReleased => 0,
		//	//		Windows.UI.Input.PointerUpdateKind.MiddleButtonReleased => 1,
		//	//		Windows.UI.Input.PointerUpdateKind.RightButtonReleased => 2,
		//	//	};

		//	//	PostJSMouseEvent("mouseup", jsButton);
		//	//	isOverPopup = false;
		//	//}


		//	//            mousePosition = point.Position.ToVector2();
		//	if(gestureResult.action == GestureAction.leftClick  || gestureResult.action == GestureAction.rightClick)
		//	{
		//		//if(wasOverPopup)
		//		//{
		//		//	PostJSMouseEvent("click", jsButton);
		//		//	return;
		//		//}

		//		(var worldC, var cc) = ToWorldAndCityC(mousePositionW);
		//		var cid = worldC.cid;

		//		switch(gestureResult.action)
		//		{
		//			case GestureAction.leftClick:
		//				{

						
		//					{
		//						if(IsCityView() && (cid == City.build))
		//						{
		//							AppS.DispatchOnUIThread(priority: DispatcherQueuePriority.High,action: () =>
		//							  {
		//								  CityBuild.Click(cc,false);

		//							  });
		//						}
		//						else
		//						{
		//							// check to see if it needs to go to the webview
		//							Spot.ProcessCoordClick(cid,cid != City.build,keyModifiers,true); ;
		//							//e.Handled = true;
		//						}
		//					}
		//					break;
		//				}

		//			case GestureAction.rightClick:
		//				{
		//					AppS.DispatchOnUIThread(RightClick(cc,cid),priority: DispatcherQueuePriority.High);
		//					break;
		//				}
		//			//case GestureAction.rightClick:
		//			//	{
		//			//		AppS.DispatchOnUIThreadLow(() =>
		//			//		{

		//			//			var spot = Spot.GetOrAdd(cid);

		//			//			var text = spot.ToTsv();
		//			//			Note.Show($"Copied to clipboard: {text}");
		//			//			AppS.CopyTextToClipboard(text);
		//			//			spot.SelectMe(true,AppS.keyModifiers);

		//			//		});
		//			//		break;
		//			//	}
		//			//case GestureAction.back:
		//			//	{
		//			//		NavStack.Back();
		//			//	}
		//			//	break;
		//			//case GestureAction.forward:
		//			//	{
		//			//		NavStack.Forward();

		//			//		break;
		//			//	}
		//			default:
		//				break;
		//		}

		//	}
		//	else
		//	{
		//		//	TakeFocus();
		//		// middle click des nothing
		//	}
		//}

		//private static Windows.System.DispatcherQueueHandler HandleRight( (int x, int y) cc, int cid)
		//{
		//	return () =>
		//	{
		//		if (IsCityView() && (cid == City.build))
		//		{
		//			CityBuild.Click(cc, true);
		//		}
		//		else
		//		{

		//			var spot = Spot.GetOrAdd(cid);
		//			if (!AppS.IsKeyPressedShiftOrControl())
		//				spot.SetFocus(true, true, false);
		//			spot.ShowContextMenu(canvas, CanvasToDIP(mousePosition));
		//			// }
		//		}
		//	};
		//}

		static public bool forceAllowWebFocus;
		public static bool isOverPopup
		{
			get {

				foreach(var popup in GameClient.popups)
				{
					// should this be in DIPS or pixels?
					if(popup.Contains(mousePosition))
					{
						return true;
					}

				}
				return false;

			}
		}


		//private static bool TryPostJSMouseEvent(string eventName, int button)
		//{
		//	if(isOverPopup)
		//	{		CnVServer.PostMouseEventToJS( (int)(AGame.nativeToDip/ShellPage.webViewScale.X * mousePosition.X) + canvasBaseX, (int)(AGame.nativeToDip/ShellPage.webViewScale.Y * mousePosition.Y) + canvasBaseY, eventName, button );
		//			Log("JsMouse");
		//		return true;
		//	}
		//	return false;
		//}




		//public static void Canvas_PointerPressed((Windows.Foundation.Point Position, uint PointerId,
		//																	bool IsInContact, ulong Timestamp, PointerUpdateKind PointerUpdateKind) point)
		//{
		//	AppS.InputRecieved();
		//	UpdateMousePosition(point.Position);
		//	//	ShellPage.TakeFocus();            //	ClearHover();
		//	//  e.Handled = false;
		//	//if (CityBuild.menuOpen)
		//	//{
		//	//	AppS.DispatchOnUIThreadLow(() => ShellPage.instance.buildMenu.IsOpen = false); // light dismiss
		//	//	return;
		//	//}

		//	//	if(!isFocused)
		//	//		return;

		//	Assert(isOverPopup == false);
		//	//            canvas.CapturePointer(e.Pointer);
		//	//var point = e.CurrentPoint;

		//	//			var properties = point.Properties;
		//	var gestureResponse = Gesture.ProcessPressed(point);
		//	if(!gestureResponse.process)
		//		return;
		//	SetMousePosition( gestureResponse.c);
		////	mousePositionW = mousePosition.ScreenToWorld();


		//	//var prior = lastMousePressTime;
		//	//lastMousePressTime = DateTimeOffset.UtcNow;
		//	//lastMousePressPosition = mousePosition;

			
		//	(var c, var cc) = ToWorldAndCityC(mousePositionW);

		//	//  if (CnVServer.IsCityView())
		//	// The app pas priority over back and forward events
		//	{
		//		switch(point.PointerUpdateKind)
		//		{
		//			case PointerUpdateKind.XButton1Pressed:
		//				//				e.Handled = true;
		//				NavStack.Back(true);
		//				ClearHover();
		//				return;
		//			case PointerUpdateKind.XButton2Pressed:
		//				//			e.Handled = true;
		//				NavStack.Forward(true);
		//				ClearHover();
		//				return;


		//		}
		//		//    e.Handled = false;
		//		//    return;
		//	}
		//	//			if (TryPostJSMouseEvent("click",
		//	//				point.PointerUpdateKind switch
		//	//				{
		//	//					PointerUpdateKind.LeftButtonPressed => 0,
		//	//					PointerUpdateKind.MiddleButtonPressed => 1,				
		//	//					PointerUpdateKind.RightButtonPressed => 2,
		//	//					_=>0
		//	//				}))
		//	//			{
		//	//				e.Handled = true;
		//	//				Gesture.Reset();
		//	//				ShellPage.SetWebViewHasFocus(true);
		//	//			}
		//	//			else
		//	{
		//		// Update focus
		//		if(IsCityView())
		//		{
		//			CityBuild.UpdateHovered(cc);

		//		}

		//	}


		//}






		public static bool HandleWheel(Windows.Foundation.Point point,float scroll)
		{
			UpdateMousePosition(point);
			///Note.Show($"Wheel");

		//	ShellPage.TakeFocus();
			//PointerInfo(e);
			//if (ShellPage.IsCityView())
			//{
			//    e.Handled = false;
			//    return;
			//}


			// wheel over javascript
			//if(TryPostJSMouseEvent(null,0))
			//{
			//	//				isOverPopup = true;
			////	e.Handled = true;

			//	//				ShellPage.SetWebViewHasFocus(true);
			//	return true;
			//}
			DoZoom(scroll);
			return false;
		}
		//public static void UpdateMousePosition(PointerEventArgs e)
		//{
		//	UpdateMousePosition(e.CurrentPoint.Position);
		//}
		//public static void UpdateMousePosition(PointerEventArgs e,UIElement source)
		//{
		//	var pt0 = e.CurrentPoint.Position;
		//	var pt = pt0.TransformPoint(source,canvas);

		//	UpdateMousePosition(pt);
		//}



		//public static void UpdateMousePosition(PointerRoutedEventArgs e)
		//{
		//	UpdateMousePosition(e.GetCurrentPoint(canvas).Position);
		//}
		public static void UpdateMousePosition(Windows.Foundation.Point point)
		{
			SetMousePosition( point.ToVector2() );

			//Note.Show($"{pointC.X} {pointC.Y}");

			//			var windowsPosition = e.CurrentPoint.Position;
			//			mousePosition = GetCanvasPosition(windowsPosition);


		}
	

	}
	public static class ShellHelper
	{
		public static Windows.Foundation.Point TransformPoint(this Windows.Foundation.Point pt0,UIElement source,UIElement target)
		{
			var gt = source.TransformToVisual(target);
			return gt.TransformPoint(pt0);
		}
	}
}
