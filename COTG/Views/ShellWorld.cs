﻿using COTG.Game;
using COTG.Helpers;
using COTG.Services;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace COTG.Views
{
    
    public partial class ShellPage
    {
        public static CoreIndependentInputSource coreInputSource;
        static bool isMouseDown;
        public static Vector2 mousePosition;
        public static Vector2 lastMousePressPosition;
        public static string toolTip;
        

        //private void SetupCanvasInput()
        //{
        //    coreInputSource = canvas.CreateCoreIndependentInputSource(CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch);
        //    coreInputSource.PointerMoved += CoreInputSource_PointerMoved;
        //    coreInputSource.PointerPressed += CoreInputSource_PointerPressed;
        //    coreInputSource.PointerReleased += CoreInputSource_PointerReleased;
        //    coreInputSource.PointerWheelChanged += CoreInputSource_PointerWheelChanged;
        //    coreInputSource.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessUntilQuit);
        //    coreInputSource.IsInputEnabled = true;

        //    canvas.Update += Canvas_Update;

        //}
        //private void Canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        //{
        //}


        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            canvas.ReleasePointerCapture(e.Pointer);
            ShellPage.L("CRelease " + e.GetCurrentPoint(canvas).Position.ToString());
            isMouseDown = false;
            mousePosition = e.GetCurrentPoint(canvas).Position.ToVector2();
            if( (lastMousePressPosition-mousePosition).Length() < 8.0f )
            {
                var worldC = MousePointToWorld(mousePosition);
                var cid = worldC.WorldToCid();
                var info = World.CityLookup(worldC);
                //if (info.type == World.typeCity)
                //{
                //    var spot = DefensePage.GetDefender(cid); // cache it
                //    spot.pid = info.data; // Set player id from world data.
                //                          // If this has already been selected it will have no effect
                //}
                
                    // If clicking on our city, change city to that, otherwise show the city info
                    // for non cities we show info
                    if (info.type == World.typeCity && info.data == JSClient.jsVars.pid)
                    {
                        var city = DefensePage.GetDefender(cid); // this will add it to the list if it isn't present and then toggle selection
                        JSClient.ChangeCity(cid);
                    }
                    else
                    {
                        JSClient.ShowCityWithoutViewChange(cid);
                    }
                //   JSClient.ShowCity(MousePointToCid(mousePosition));

            }
        }

        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isMouseDown = true;
            canvas.CapturePointer(e.Pointer);
            mousePosition = e.GetCurrentPoint(canvas).Position.ToVector2();
            lastMousePressPosition = mousePosition;
            ShellPage.L("CPress " + e.GetCurrentPoint(canvas).Position.ToString());
        }
        private void Canvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if(isMouseDown )
            {
                isMouseDown = false;
                canvas.ReleasePointerCapture(e.Pointer);
            }
            Spot.viewHover = 0;
        }
        private void Canvas_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            isMouseDown = false;
            Spot.viewHover = 0;
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
        //    ShellPage.L("CWheel " + wheel);
        }
        static (int x,int y) MousePointToWorld(Vector2 c1)
        {
            var wc = ShellPage.cameraC + (c1 + ShellPage.clientC) * (1.0f / ShellPage.pixelScale);

            int x = (int)(wc.X);
            int y = (int)(wc.Y);
            return (x, y);
        }

        static int MousePointToCid(Vector2 c1)
        {
            return MousePointToWorld(c1).WorldToCid();
        }


        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            var c1 = point.Position.ToVector2();
            var c = MousePointToWorld(c1);
            (var type, var data) = World.CityLookup(c);
            if (type != 0)
            {
                Spot.viewHover = c.WorldToCid();
                var player = Player.all.GetValueOrDefault(data, Player._default);
                toolTip = $"{player.name}\n{Alliance.IdToName(player.alliance)}\n{c.y/100}{c.x/100} ({c.x}:{c.y})\ncities:{player.cities}\npts:{player.pointsH * 100}";

            }
            else
            {
                Spot.viewHover = 0;
                toolTip = null;
            }
            if (isMouseDown)
            {
                // If the mouse drags off the surface we will miss the mouse up
                // TODO:  mouse should be hooked.
                if (point.IsInContact)
                {
                    cameraC -= (c1 - mousePosition) / cameraZoom;
                }
            }

            mousePosition = c1;
        }
        private void EventTimeTravelSliderChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var dt = TimeSpan.FromMinutes(e.NewValue);
            var serverTime = JSClient.ServerTime() + TimeSpan.FromMinutes(e.NewValue);
            eventTimeTravelText.Text = $"Event Time Travel Offset: {dt.Hours}hr:{dt.Minutes}min,  T:{serverTime.Hour}:{serverTime.Minute:D2}";
        }
    }
}
