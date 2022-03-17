using Microsoft.UI.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CnV;
using static CnV.View;
using static CnV.ClientView;
using static CnV.GameClient;


namespace CnV.Views;

using Draw;

using Game;

using Helpers;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Input;

using System.Numerics;
	using Microsoft.UI.Composition.Interactions;

partial class ShellPage
{
//	static GestureRecognizer gestureRecognizer;
			public static InputPointerSource coreInputSource;

	public static void SetupNonCoreInput()
	{
		canvas.ManipulationMode = ManipulationModes.None;
		
		//Canvas_PointerWheelChanged(mouseState, priorMouseState);
	//	canvas.ManipulationMode= ManipulationModes.None;
		SetupCoreInput();

		//		canvas.PointerMoved+=KeyboardProxy_PointerMoved;
		//		canvas.PointerPressed+=KeyboardProxy_PointerPressed;
		//		canvas.PointerReleased+=KeyboardProxy_PointerReleased;
		//		canvas.PointerWheelChanged += KeyboardProxy_PointerWheelChanged;

	//	canvas.PointerEntered+=KeyboardProxy_PointerEntered;
	//	canvas.PointerExited +=KeyboardProxy_PointerExited;
		canvas.KeyDown += KeyboardProxy_KeyDown;
		//FocusManager.GotFocus +=FocusManager_GotFocus;
		//FocusManager.LostFocus +=FocusManager_LostFocus;
		canvas.IsHitTestVisible = true;
		
		
	}
	public static int pointerPressedCount;

	internal static DispatcherQueueController inputQueueController;
	static GestureRecognizer recognizer;
	public static void SetupCoreInput()
	{
	//	MouseWheelParameters
//		recognizer.MouseWheelParameters
		inputQueueController = DispatcherQueueController.CreateOnDedicatedThread();
		inputQueueController.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.High,
			() =>
			{
				try
				{
					// Set up the pointer input source to receive pen input for the swap chain panel.
					coreInputSource = canvas.CreateCoreIndependentInputSource(InputPointerSourceDeviceKinds.Mouse | InputPointerSourceDeviceKinds.Pen|InputPointerSourceDeviceKinds.Touch  
						);
					recognizer = new()
					{
						GestureSettings= GestureSettings.Tap|GestureSettings.RightTap|
					GestureSettings.ManipulationTranslateX|GestureSettings.ManipulationTranslateY|GestureSettings.ManipulationScale
				//	|GestureSettings.ManipulationMultipleFingerPanning
				//|GestureSettings.Hold
					};

					//var interactionSource = VisualInteractionSource.CreateFromIVisualElement(canvas);
					//interactionSource.PositionXSourceMode = InteractionSourceMode.EnabledWithoutInertia;
					//interactionSource.PositionYSourceMode = InteractionSourceMode.EnabledWithoutInertia;
					//interactionSource.ScaleSourceMode = InteractionSourceMode.EnabledWithoutInertia;

					//// Modify the pointer wheel configuration to disable X and Y.
					//interactionSource.PointerWheelConfig.PositionXSourceMode =
					//	InteractionSourceRedirectionMode.Disabled;
					//interactionSource.PointerWheelConfig.PositionYSourceMode =
					//	InteractionSourceRedirectionMode.Disabled;
					//interactionSource.PointerWheelConfig.ScaleSourceMode =
					//		InteractionSourceRedirectionMode.Enabled;

					recognizer.AutoProcessInertia = false;
					recognizer.ManipulationCompleted+=Recognizer_ManipulationCompleted;
					recognizer.ManipulationInertiaStarting+=Recognizer_ManipulationInertiaStarting;
					recognizer.ManipulationStarted+=Recognizer_ManipulationStarted;
					recognizer.ManipulationUpdated+=Recognizer_ManipulationUpdated;
					recognizer.Tapped+=Recognizer_Tapped;
					recognizer.RightTapped+=Recognizer_RightTapped;
					recognizer.Holding+=Recognizer_Holding;
					//	Log(canvas.ManipulationMode);
					//	canvas.ManipulationMode = ManipulationModes.All;
					coreInputSource.PointerMoved+=CoreInputSource_PointerMoved;
					coreInputSource.PointerPressed+=CoreInputSource_PointerPressed; ;
					coreInputSource.PointerReleased+=CoreInputSource_PointerReleased; ;
					coreInputSource.PointerEntered+=CoreInputSource_PointerEntered; ;
					coreInputSource.PointerExited+=CoreInputSource_PointerExited; ;
					//coreInputSource.PointerCaptureLost += CoreInputSource_PointerCaptureLost;
					coreInputSource.PointerRoutedTo+=CoreInputSource_PointerRoutedTo;
					coreInputSource.PointerWheelChanged+=CoreInputSource_PointerWheelChanged;

				}
				catch(Exception __ex)
				{
					Debug.LogEx(__ex);
				}

				//	Thread.Sleep(-1);
			});
	}

	private static void CoreInputSource_PointerRoutedTo(InputPointerSource sender,PointerEventArgs args)
	{
		args.KeyModifiers.UpdateKeyModifiers();

		Canvas_PointerEntered(args.CurrentPoint.Position);
	//	Debug.Log($"Mouse pos: {mousePositionW}");
	}

	private static void Recognizer_Holding(GestureRecognizer sender,HoldingEventArgs args) {
	
	}
	
	private static void Recognizer_RightTapped(GestureRecognizer sender,RightTappedEventArgs args)
	{
		//args.KeyModifiers.UpdateKeyModifiers();
		UpdateMousePosition(args.Position);
		(var wc, var bc) = ToWorldAndCityC(mousePositionW);

		if(IsCityView())
		{
			CityBuild.Click(bc,true);
		}
		else
		{
			if(!wc.isNan)
			{
				var spot = Spot.GetOrAdd(wc.cid);
				if(!AppS.IsKeyPressedShiftOrControl())
					spot.SetFocus(true,true,false);
				var position = args.Position;
				AppS.DispatchOnUIThread(priority: DispatcherQueuePriority.High,action: () =>
					 spot.ShowContextMenu(canvas,position));

			}
		}
	}

	private static void Recognizer_Tapped(GestureRecognizer sender,TappedEventArgs args)
	{
		UpdateMousePosition(args.Position);
		(var wc, var bc) = ToWorldAndCityC(mousePositionW);

		var isBuild = wc.cid ==  City.build;
		if(IsCityView() && isBuild)
		{

			CityBuild.Click(bc,false);
		}
		else
		{
			if(!wc.isNan)
			{
				// check to see if it needs to go to the webview
				Spot.ProcessCoordClick(wc.cid,!isBuild,AppS.keyModifiers,true); ;
			}
			//e.Handled = true;
		}
	}

	private static void Recognizer_ManipulationUpdated(GestureRecognizer sender,ManipulationUpdatedEventArgs args)
	{
		var dt = args.Delta.Translation;
		var dr = new Vector2(dt._x,dt._y);
		if(dr.LengthSquared() > 0)
		{
			dr *= 1.0f.ScreenToWorld();
			View.SetViewTargetInstant(View.viewW2 - dr);
		}
		var scale = args.Cumulative.Scale;
		//if(scale != 1.0f)
		//	Note.Show($"Scale: {scale}");
		var exp = args.Delta.Expansion;
		if(exp != 0.0f
			//&&
			//args.PointerDeviceType != PointerDeviceType.Mouse
			)
		{
			HandleWheel(args.Position,exp);
		}
	}

	private static void Recognizer_ManipulationStarted(GestureRecognizer sender,ManipulationStartedEventArgs args)  {}// Note.Show("Manipulation Started");
	private static void Recognizer_ManipulationInertiaStarting(GestureRecognizer sender,ManipulationInertiaStartingEventArgs args)  {}// Note.Show("Inertia");
	private static void Recognizer_ManipulationCompleted(GestureRecognizer sender,ManipulationCompletedEventArgs args) 
	{
		var dv = args.Velocities.Linear;
		var dr = new Vector2(dv._x,dv._y);
		var speed = dr.Length();
		//Note.Show($"Speed: {speed}");
		if(speed  >  0.5f  )
		{
			View.isCoasting =true;
		//	var gain = -View.panV1*View.viewW.Z / speed;
			View.viewVW= new(-1000*dr.ScreenToWorldOffset(),0.0f);
		}
	}

	private static void CoreInputSource_PointerExited(InputPointerSource sender,PointerEventArgs args)
	{
		args.KeyModifiers.UpdateKeyModifiers();
		Canvas_PointerExited(args.CurrentPoint.Position,args.CurrentPoint.PointerId);
	}

	private static void CoreInputSource_PointerEntered(InputPointerSource sender,PointerEventArgs args)
	{
		args.KeyModifiers.UpdateKeyModifiers();

		Canvas_PointerEntered(args.CurrentPoint.Position);
	}

	private static void CoreInputSource_PointerReleased(InputPointerSource sender,PointerEventArgs args)
	{
		--pointerPressedCount;
		recognizer.ProcessUpEvent(args.CurrentPoint);

		args.KeyModifiers.UpdateKeyModifiers();

		var point = args.CurrentPoint;
		//Canvas_PointerReleased((point.Position, point.PointerId, point.IsInContact, point.Timestamp, point.Properties.PointerUpdateKind),args.KeyModifiers);
		args.Handled=true;
	}
	//		public static PointerUpdateKind GetPointerUpdateKind()
	private static void CoreInputSource_PointerPressed(InputPointerSource sender,PointerEventArgs args)
	{
		++pointerPressedCount;
		View.EndCoasting();
		//View.isCoasting = false;
		AppS.InputRecieved();

		var point = args.CurrentPoint;
		args.KeyModifiers.UpdateKeyModifiers();
		UpdateMousePosition(point.Position);
		var prop = point.Properties;
		//if(!prop.IsPrimary && !prop.IsRightButtonPressed )
		{
			if(prop.IsXButton1Pressed)
			{

				NavStack.Back(true);
				ClearHover();
				args.Handled=true;
				return;
			}
			if(prop.IsXButton2Pressed)
			{

				NavStack.Forward(true);
				ClearHover();
				args.Handled=true;
				return;
			}
		}
		recognizer.ProcessDownEvent(args.CurrentPoint);

		if(IsCityView())
		{
			(var c, var cc) = ToWorldAndCityC(mousePositionW);
			CityBuild.UpdateHovered(cc);

		}
		//	Canvas_PointerPressed((point.Position, point.PointerId, point.IsInContact, point.Timestamp, point.Properties.PointerUpdateKind));
		args.Handled=true;

	}

	public static void CoreInputSource_PointerMoved(InputPointerSource sender,PointerEventArgs e)
	{
		var points = e.GetIntermediatePoints();
		var point = points.Last();
		var pointC = point.Position;
		recognizer.ProcessMoveEvents(points);
		

		if(point.PointerDeviceType == PointerDeviceType.Touchpad)
			Note.Show("Touchpad");

		if(point.Properties.ContactRect._width> 1)
			Note.Show(point.Properties.ContactRect.ToString());
		e.KeyModifiers.UpdateKeyModifiers();
		Canvas_PointerMoved((pointC, point.PointerId, point.IsInContact, point.Timestamp, point.Properties.PointerUpdateKind));
		e.Handled=true;
	}
	private static void CoreInputSource_PointerWheelChanged(InputPointerSource sender,PointerEventArgs e)
	{
		View.EndCoasting();
		var pt = e.CurrentPoint;
		var point = pt.Position;
		if(pt.PointerDeviceType == PointerDeviceType.Touchpad)
			Note.Show("Touchpad");

		if(  pt.PointerDeviceType == PointerDeviceType.Mouse && (Settings.pointerGestureMode == PointerGestureMode.wheelIsZoom) )
		{
			var scroll = pt.Properties.MouseWheelDelta;
			HandleWheel(point,scroll*0.5f);
		}
		else
		{
		 recognizer.ProcessMouseWheelEvent(e.CurrentPoint,e.KeyModifiers.IsShift(),e.KeyModifiers.IsControl());
		}
			e.Handled=true;
	}
}

