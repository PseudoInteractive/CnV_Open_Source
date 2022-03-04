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

partial class ShellPage
{
	static GestureRecognizer gestureRecognizer;

	public static void SetupNonCoreInput()
	{
		canvas.ManipulationMode = ManipulationModes.None;
		//Canvas_PointerWheelChanged(mouseState, priorMouseState);
		SetupCoreInput();

//		canvas.PointerMoved+=KeyboardProxy_PointerMoved;
//		canvas.PointerPressed+=KeyboardProxy_PointerPressed;
//		canvas.PointerReleased+=KeyboardProxy_PointerReleased;
//		canvas.PointerWheelChanged += KeyboardProxy_PointerWheelChanged;

		canvas.PointerEntered+=KeyboardProxy_PointerEntered;
		canvas.PointerExited +=KeyboardProxy_PointerExited;
		canvas.KeyDown += KeyboardProxy_KeyDown;
		//FocusManager.GotFocus +=FocusManager_GotFocus;
		//FocusManager.LostFocus +=FocusManager_LostFocus;
		canvas.IsHitTestVisible = true;
	}

	internal static DispatcherQueueController inputQueueController;
	static GestureRecognizer recognizer;
	public static void SetupCoreInput()
	{
		inputQueueController = DispatcherQueueController.CreateOnDedicatedThread();
		inputQueueController.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.High,
			() =>
			{
				try
				{
					// Set up the pointer input source to receive pen input for the swap chain panel.
					coreInputSource = canvas.CreateCoreIndependentInputSource(InputPointerSourceDeviceKinds.Mouse | InputPointerSourceDeviceKinds.Pen|InputPointerSourceDeviceKinds.Touch);
					recognizer = new() { GestureSettings= GestureSettings.Tap|GestureSettings.RightTap|GestureSettings.ManipulationTranslateX|GestureSettings.ManipulationTranslateY|GestureSettings.ManipulationScale };
					recognizer.AutoProcessInertia = true;
					recognizer.ManipulationCompleted+=Recognizer_ManipulationCompleted;
					recognizer.ManipulationInertiaStarting+=Recognizer_ManipulationInertiaStarting;
					recognizer.ManipulationStarted+=Recognizer_ManipulationStarted;
					recognizer.ManipulationUpdated+=Recognizer_ManipulationUpdated;
					recognizer.Tapped+=Recognizer_Tapped;
					recognizer.RightTapped+=Recognizer_RightTapped;
					//	Log(canvas.ManipulationMode);
					//	canvas.ManipulationMode = ManipulationModes.All;
					coreInputSource.PointerMoved+=CoreInputSource_PointerMoved;
					coreInputSource.PointerPressed+=CoreInputSource_PointerPressed; ;
					coreInputSource.PointerReleased+=CoreInputSource_PointerReleased; ;
					//					coreInputSource.PointerEntered+=CoreInputSource_PointerEntered; ;
					//					coreInputSource.PointerExited+=CoreInputSource_PointerExited; ;
					//coreInputSource.PointerCaptureLost += CoreInputSource_PointerCaptureLost;

					coreInputSource.PointerWheelChanged+=CoreInputSource_PointerWheelChanged;

				}
				catch(Exception __ex)
				{
					Debug.LogEx(__ex);
				}

				//	Thread.Sleep(-1);
			});
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
		if(scale != 1.0f)
			Note.Show($"Scale: {scale}");
		var exp = args.Delta.Expansion;
		if(exp != 0.0f ) // && args.PointerDeviceType != PointerDeviceType.Mouse)
		{
			HandleWheel(args.Position,exp); 
		}
	}

	private static void Recognizer_ManipulationStarted(GestureRecognizer sender,ManipulationStartedEventArgs args) => Note.Show("Manipulation Started");
	private static void Recognizer_ManipulationInertiaStarting(GestureRecognizer sender,ManipulationInertiaStartingEventArgs args) => Note.Show("Inertia");
	private static void Recognizer_ManipulationCompleted(GestureRecognizer sender,ManipulationCompletedEventArgs args) => Note.Show("Manipulation Complete");

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
		recognizer.ProcessUpEvent(args.CurrentPoint);

		args.KeyModifiers.UpdateKeyModifiers();

		var point = args.CurrentPoint;
		//Canvas_PointerReleased((point.Position, point.PointerId, point.IsInContact, point.Timestamp, point.Properties.PointerUpdateKind),args.KeyModifiers);
		args.Handled=true;
	}
	//		public static PointerUpdateKind GetPointerUpdateKind()
	private static void CoreInputSource_PointerPressed(InputPointerSource sender,PointerEventArgs args)
	{

		AppS.InputRecieved();
		recognizer.ProcessDownEvent(args.CurrentPoint);

		var point = args.CurrentPoint;
		args.KeyModifiers.UpdateKeyModifiers();
		UpdateMousePosition(point.Position);
		if(!recognizer.IsActive)
		{
			if(point.Properties.IsXButton1Pressed)
			{

				NavStack.Back(true);
				ClearHover();
				args.Handled=true;
				return;
			}
			if(point.Properties.IsXButton2Pressed)
			{

				NavStack.Forward(true);
				ClearHover();
				args.Handled=true;
				return;
			}
		}
		if(IsCityView())
		{
			(var c, var cc) = ToWorldAndCityC(mousePositionW);
			CityBuild.UpdateHovered(cc);

		}
		//	Canvas_PointerPressed((point.Position, point.PointerId, point.IsInContact, point.Timestamp, point.Properties.PointerUpdateKind));


	}

	public static void CoreInputSource_PointerMoved(InputPointerSource sender,PointerEventArgs e)
	{
		recognizer.ProcessMoveEvents(e.GetIntermediatePoints());
		var point = e.CurrentPoint;
		e.KeyModifiers.UpdateKeyModifiers();
		Canvas_PointerMoved((point.Position, point.PointerId, point.IsInContact, point.Timestamp, point.Properties.PointerUpdateKind));
		e.Handled=true;
	}
	private static void CoreInputSource_PointerWheelChanged(InputPointerSource sender,PointerEventArgs e)
	{
		var pt = e.CurrentPoint;
		var point = pt.Position;
		if( pt.PointerDeviceType == PointerDeviceType.Mouse )
		{
			var scroll = pt.Properties.MouseWheelDelta;
			HandleWheel(point,scroll);
		}
		else
		{
		 recognizer.ProcessMouseWheelEvent(e.CurrentPoint,e.KeyModifiers.IsShift(),e.KeyModifiers.IsControl());
		}
			e.Handled=true;
	}
}

