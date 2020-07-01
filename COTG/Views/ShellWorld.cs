using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;

namespace COTG.Views
{
    public partial class ShellPage
    {
        static bool isMouseDown;
        static Vector2 lastMousePoint;


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
            ShellPage.L("CWheel " + e.GetCurrentPoint(canvas).Position.ToString());
        }

        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isMouseDown)
            {
                var c1 = e.GetCurrentPoint(canvas).Position.ToVector2();
                cameraC += (c1 - lastMousePoint) / cameraZoom;
                lastMousePoint = c1;
            }
        }

    }
}
