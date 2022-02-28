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

	using Microsoft.UI.Xaml.Input;

	partial class ShellPage
	{
	static GestureRecognizer gestureRecognizer;

		public static void SetupNonCoreInput()
		{
			//Canvas_PointerWheelChanged(mouseState, priorMouseState);
			//	SetupCoreInput();
			canvas.PointerMoved+=KeyboardProxy_PointerMoved;
			canvas.PointerPressed+=KeyboardProxy_PointerPressed;
			canvas.PointerReleased+=KeyboardProxy_PointerReleased;
			canvas.PointerEntered+=KeyboardProxy_PointerEntered;
			canvas.PointerExited +=KeyboardProxy_PointerExited;
			canvas.PointerWheelChanged += KeyboardProxy_PointerWheelChanged;
			canvas.KeyDown += KeyboardProxy_KeyDown;
			FocusManager.GotFocus +=FocusManager_GotFocus;
			FocusManager.LostFocus +=FocusManager_LostFocus;
			canvas.IsHitTestVisible = true;
		}
	

	static void ThreadFunc()
		{

		 _queuecontroller = DispatcherQueueController.CreateOnDedicatedThread();
        _queuecontroller.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.High,
            () =>
            {
                // Set up the pointer input source to receive pen input for the swap chain panel.
                _inputPointerSource = _panel.CreateCoreIndependentInputSource(InputPointerSourceDeviceKinds.Pen);
                _inputPointerSource.PointerPressed += InputPointerSource_PointerPressed;
                _inputPointerSource.PointerMoved += InputPointerSource_PointerMoved;
                _inputPointerSource.PointerReleased += InputPointerSource_PointerReleased;

                // Create the pointer predictor for the input pointer source. By default it will be configured
                // to predict points 15ms into the future
                _pointerPredictor = PointerPredictor.CreateForInputPointerSource(_inputPointerSource);
            });

			//	canvas.DispatchOnUIThread(  () =>
			{
				try
				{

					// Set up the pointer input source to receive pen input for the swap chain panel.
					coreInputSource = canvas.CreateCoreIndependentInputSource(InputPointerSourceDeviceKinds.Mouse | InputPointerSourceDeviceKinds.Pen|InputPointerSourceDeviceKinds.Touch);


					//	Log(canvas.ManipulationMode);
					//	canvas.ManipulationMode = ManipulationModes.All;
					coreInputSource.PointerMoved+=CoreInputSource_PointerMoved;
					coreInputSource.PointerPressed+=CoreInputSource_PointerPressed; ;
					coreInputSource.PointerReleased+=CoreInputSource_PointerReleased; ;
					coreInputSource.PointerEntered+=CoreInputSource_PointerEntered; ;
					coreInputSource.PointerExited+=CoreInputSource_PointerExited; ;
					coreInputSource.PointerCaptureLost += CoreInputSource_PointerCaptureLost;

					coreInputSource.PointerWheelChanged += Canvas_PointerWheelChanged;

				}
				catch(Exception __ex)
				{
					Debug.LogEx(__ex);
				}

				//	Thread.Sleep(-1);
			}
		}

		public static void SetupCoreInput()
		{



			//	);
			var hr = canvas.DispatcherQueue.TryEnqueue(ThreadFunc);
			Assert(hr==true);

			//			inputWorker = new Thread(ThreadFunc) { IsBackground=true  };

			//			inputWorker.Start();
		}



}

