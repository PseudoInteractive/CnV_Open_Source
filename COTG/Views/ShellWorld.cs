using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;

namespace COTG.Views
{
    
    public partial class ShellPage
    {
        public static CoreIndependentInputSource coreInputSource;
        static bool isMouseDown;
        static Vector2 lastMousePoint;

        private void SetupCanvasInput()
        {
            coreInputSource = canvas.CreateCoreIndependentInputSource(CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch);
            coreInputSource.PointerMoved += CoreInputSource_PointerMoved;
            coreInputSource.PointerPressed += CoreInputSource_PointerPressed;
            coreInputSource.PointerReleased += CoreInputSource_PointerReleased;
            coreInputSource.PointerWheelChanged += CoreInputSource_PointerWheelChanged;
            coreInputSource.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessUntilQuit);
            coreInputSource.IsInputEnabled = true;

            canvas.Update += Canvas_Update;

        }
        private void Canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
        }


        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ShellPage.L("CRelease " + e.GetCurrentPoint(canvas).Position.ToString());
            isMouseDown = false;
        }

        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isMouseDown = true;
            lastMousePoint = e.GetCurrentPoint(canvas).Position.ToVector2();
            ShellPage.L("CPress " + e.GetCurrentPoint(canvas).Position.ToString());
        }

        private void Canvas_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            var pt = e.GetCurrentPoint(canvas);
            var wheel = pt.Properties.MouseWheelDelta;
            var dZoom = wheel.SignOr0() * 0.0625f + wheel * (1.0f / 1024.0f);
            var newZoom = cameraZoom * MathF.Exp(dZoom);
            var cBase = pt.Position.ToVector2() + clientC;
            var c0 = cBase/cameraZoom;
            var c1 = cBase / newZoom;
            cameraC += c0 - c1;

            cameraZoom = newZoom;
            ShellPage.L("CWheel " + wheel);
        }

        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isMouseDown)
            {
                var c1 = e.GetCurrentPoint(canvas).Position.ToVector2();
                cameraC -= (c1 - lastMousePoint) / cameraZoom;
                lastMousePoint = c1;
            }
        }
        private void CoreInputSource_PointerWheelChanged(object sender, PointerEventArgs args)
        {
            L("wheel");
       //     throw new NotImplementedException();
        }

        private void CoreInputSource_PointerReleased(object sender, PointerEventArgs args)
        {
            L("rel");
        }

        private void CoreInputSource_PointerPressed(object sender, PointerEventArgs args)
        {
            L("press");
        }

        private void CoreInputSource_PointerMoved(object sender, PointerEventArgs args)
        {
            L("moved");

//            throw new NotImplementedException();
        }
    }
}
