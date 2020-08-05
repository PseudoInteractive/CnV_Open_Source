using COTG.Game;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using static COTG.Debug;
namespace COTG.Views
{

    public partial class ShellPage
    {
        public static CoreIndependentInputSource coreInputSource;
        [Flags] enum MouseButtons
        {
            left = 1,
            right = 2,
            middle = 4,
            x1 = 8,
            x2 = 16,
        };
        static MouseButtons mouseButtons;

        public static Vector2 mousePosition;
        public static Vector2 lastMousePressPosition;
        public static DateTimeOffset lastMousePressTime;


        public static string toolTip;
        public static string contToolTip;
        public static int lastCont;

        private void SetupCoreInput()
        {
            coreInputSource = canvas.CreateCoreIndependentInputSource(CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch);
            coreInputSource.PointerMoved += Canvas_PointerMoved;
            coreInputSource.PointerPressed += Canvas_PointerPressed;
            coreInputSource.PointerReleased += Canvas_PointerReleased;
//            coreInputSource.PointerEntered += Canvas_PointerEntered;
            coreInputSource.PointerExited += Canvas_PointerExited;

            coreInputSource.PointerWheelChanged += Canvas_PointerWheelChanged;
          //  coreInputSource.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessUntilQuit);
            coreInputSource.IsInputEnabled = true;


        }

        static Vector2 GetCanvasPosition(Windows.Foundation.Point screenC)
            {
//            return new Vector2((float)screenC.X - clientCScreen.X, (float)screenC.Y - clientCScreen.Y);
            return new Vector2((float)screenC.X , (float)screenC.Y );
        }
        private void Canvas_PointerReleased(object sender, PointerEventArgs e)
        {
            if (JSClient.IsCityView())
            {
                e.Handled = false;
                return;
            }
            var buttons = mouseButtons;
         //   canvas.ReleasePointerCapture(e.Pointer);
            // ChatTab.L("CRelease " + e.GetCurrentPoint(canvas).Position.ToString());
            mouseButtons = 0;
            mousePosition = GetCanvasPosition(e.CurrentPoint.Position);
            
//            mousePosition = point.Position.ToVector2();
            if ((lastMousePressPosition - mousePosition).Length() < 8.0f)
            {
                var worldC = MousePointToWorld(mousePosition);
                var cid = worldC.WorldToCid();
                //var info = World.CityLookup(worldC);
                //if (info.type == World.typeCity)
                //{
                //    var spot = DefensePage.GetDefender(cid); // cache it
                //    spot.pid = info.data; // Set player id from world data.
                //                          // If this has already been selected it will have no effect
                //}

                // If clicking on our city, change city to that, otherwise show the city info
                // for non cities we show info
                //if (info.type == World.typeCity && info.player == JSClient.jsVars.pid)
                //{
                //    var city = SpotTab.TouchSpot(cid); // this will add it to the list if it isn't present and then toggle selection
                //    JSClient.ChangeCity(cid);
                //}
                //else
                if(buttons.HasFlag( MouseButtons.left ) )
                {
                    Spot.ProcessCoordClick(cid, true);
                    e.Handled = true;
                }
                else if(buttons.HasFlag(MouseButtons.right) )
                {
                   
                    var spot = Spot.GetOrAdd(cid);
                    foreach(var i in CityFlyout.Items)
                        i.DataContext = spot;

//                    CityFlyout.ShowAt(canvas,point.Position);

 //                   e.Handled = true;
                }
                else if(buttons.HasFlag(MouseButtons.x1))
                {
                    NavStack.Back();
                }
                else if (buttons.HasFlag(MouseButtons.x2))
                {
                    NavStack.Forward();
                }
                //   JSClient.ShowCity(MousePointToCid(mousePosition));

            }
            else
            {
                SetJSCamera();
            }
        }

       

        //public static void EnsureOnScreen( int cid,bool lazy)
        //{
        //    var worldC = cid.CidToWorldV();
        //    if( lazy )
        //    {
        //        var cc = worldC.WToC();
        //        if (cc.X > 0 && cc.Y > 0 && cc.X < clientSpan.X && cc.Y < clientSpan.Y)
        //            return;
        //    }

        //    ShellPage.cameraC = (-halfSpan  / ShellPage.pixelScale) +worldC - ShellPage.clientC/ ShellPage.pixelScale;



        //}

        public static void SetJSCamera()
        {
            //var cBase = halfSpan + clientC+halfSpan;
            //var c0 = cBase / cameraZoom;
            //var c1 = cBase / 64.0f;
            //var regionC = (cameraC + c0 - c1) * 64.0f;
        //    JSClient.SetJSCamera(regionC);
        }

        private void Canvas_PointerPressed(object sender, PointerEventArgs e)
        {

//            canvas.CapturePointer(e.Pointer);
            var point = e.CurrentPoint;

            mouseButtons = 0;
            var properties = point.Properties;
            if (properties.IsLeftButtonPressed)
                mouseButtons |= MouseButtons.left;
            if (properties.IsRightButtonPressed)
                mouseButtons |= MouseButtons.right;
            if (properties.IsMiddleButtonPressed)
                mouseButtons |= MouseButtons.middle;
            if (properties.IsXButton1Pressed)
            {
                mouseButtons |= MouseButtons.x1;
            }
            if (properties.IsXButton2Pressed)
            {
                mouseButtons |= MouseButtons.x2;
            }
            //if (JSClient.IsCityView())
            //{
            //    switch (properties.PointerUpdateKind)
            //    {
            //        case Windows.UI.Input.PointerUpdateKind.XButton1Pressed:
            //        case Windows.UI.Input.PointerUpdateKind.XButton1Released:
            //            e.Handled = true;
            //            NavStack.Back();
            //            return;
            //        case Windows.UI.Input.PointerUpdateKind.XButton2Pressed:
            //        case Windows.UI.Input.PointerUpdateKind.XButton2Released:
            //            e.Handled = true;
            //            NavStack.Forward();
            //            return;
            //    }
            //    e.Handled = false;
            //    return;
            //}


            mousePosition = GetCanvasPosition(point.Position);
            var prior = lastMousePressTime;
            lastMousePressTime = DateTimeOffset.UtcNow;
            lastMousePressPosition = mousePosition;
            //          if ((lastMousePressTime - prior).TotalSeconds < 2.5f)
            //          {
            //              // double click

            //              var c = MousePointToWorld(e.GetCurrentPoint(canvas).Position.ToVector2());
            //              var data = World.CityLookup(c);
            //              switch (data.type)
            //              {
            //                  case World.typeCity:
            //                      {
            //                          if (data.player == 0)
            //                          {

            //                              //                               toolTip = $"Lawless\n{c.y / 100}{c.x / 100} ({c.x}:{c.y})";
            //                          }
            //                          else
            //                          {
            //                              var player = Player.all.GetValueOrDefault(data.player, Player._default);
            //                              if (Player.IsMe(data.player))
            //                              {
            //                              //    JSClient.ViewCity(c.WorldToCid());
            //                              }
            //                              else
            //                              {
            ////                                  toolTip = $"{player.name}\n{Alliance.IdToName(player.alliance)}\n{c.y / 100}{c.x / 100} ({c.x}:{c.y})\ncities:{player.cities}\npts:{player.pointsH * 100}";
            //                              }


            //                          }
            //                          break;
            //                      }
            //              }
            //              }
            //    ChatTab.L("CPress " + e.GetCurrentPoint(canvas).Position.ToString());
            ClearHover();

        }
        static void ClearHover()
        {
            contToolTip = null;
            lastCont = -1;
            toolTip = null;
            Spot.viewHover = 0;

        }
        private void Canvas_PointerExited(object sender, PointerEventArgs e)
        {
            //if (JSClient.IsCityView())
            //{
            //    e.Handled = false;
            //    return;
            //}

            if (mouseButtons!=0)
            {
                mouseButtons = 0;
//                canvas.ReleasePointerCapture(e.Pointer);
                SetJSCamera();

            }
            ClearHover();
        }
        //private void Canvas_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        //{
        //    mouseButtons = 0;
        //    Spot.viewHover = 0;
        //}

        private void Canvas_PointerWheelChanged(object sender, PointerEventArgs e)
        {
            //if (JSClient.IsCityView())
            //{
            //    e.Handled = false;
            //    return;
            //}

            var pt = e.CurrentPoint;
            var wheel = pt.Properties.MouseWheelDelta;
            var dZoom = wheel.SignOr0() * 0.0625f + wheel * (1.0f / 1024.0f);
            var newZoom = cameraZoom * MathF.Exp(dZoom);
            var cBase = GetCanvasPosition(pt.Position) - halfSpan;
            var c0 = cBase/cameraZoom;
            var c1 = cBase / newZoom;
            cameraC =  cameraC + c0 - c1;

            cameraZoom = newZoom;
            e.Handled = true;
            ClearHover();
            //    ChatTab.L("CWheel " + wheel);
        }
        static (int x,int y) MousePointToWorld(Vector2 c1)
        {
            var dc1 = (c1 - ShellPage.halfSpan);
            dc1 *= (1.0f / cameraZoomLag);
            var wc = ShellPage.cameraC +  dc1;

            int x = (wc.X).RoundToInt();
            int y = (wc.Y).RoundToInt();
            return (x, y);
        }

        static int MousePointToCid(Vector2 c1)
        {
            return MousePointToWorld(c1).WorldToCid();
        }

        int lastCanvasC;
        private void Canvas_PointerMoved(object sender, PointerEventArgs e)
        {
           //if(JSClient.IsCityView() )
           // {
           //     e.Handled = false;
           //     return;
           // }
            var point = e.CurrentPoint;
            var c1 = GetCanvasPosition(point.Position);
            
            var c = MousePointToWorld(c1);
            if (mouseButtons == 0)
            {
                var cont = Continent.GetPackedIdFromC(c);
                if (cont != lastCont)
                {
                    lastCont = cont;
                    ref var cn = ref Continent.all[cont];
                    contToolTip = $"{cn.id}\nSettled {cn.settled}\nFree {cn.unsettled}\nCites {cn.cities}\nCastles {cn.castles}\nTemples {cn.temples}\nDungeons {cn.dungeons}";

                }
                var cid = c.WorldToCid();
                if (lastCanvasC != cid)
                {
                    Spot.viewHover = 0;
                    toolTip = null;

                    lastCanvasC = cid;
                    var data = World.CityLookup(c);
                    switch (data.type)
                    {
                        case World.typeCity:
                            {
                                Spot.viewHover = cid;

                                if (data.player == 0)
                                {
                                    toolTip = $"Lawless\n{c.y / 100}{c.x / 100} ({c.x}:{c.y})";
                                }
                                else
                                {
                                    var player = Player.all.GetValueOrDefault(data.player, Player._default);
                                    if (Player.IsMe(data.player))
                                    {
                                        if (City.allCities.TryGetValue(c.WorldToCid(), out var city))
                                        {
                                            var notes = city.remarks.IsNullOrEmpty() ? "" : city.remarks.Substring(0, city.remarks.Length.Min(40)) + "\n";
                                            toolTip = $"{player.name}\n{Alliance.IdToName(player.alliance)}\nTSh:{city.tsHome}\nTSt:{city.tsTotal}\n{city.cityName}\n{notes}{c.y / 100}{c.x / 100} ({c.x}:{c.y})";
                                            //     Raiding.UpdateTS();
                                        }

                                    }
                                    else
                                    {
                                        toolTip = $"{player.name}\n{Alliance.IdToName(player.alliance)}\n{c.y / 100}{c.x / 100} ({c.x}:{c.y})\ncities:{player.cities}\npts:{player.pointsH * 100}";
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
                }
            }
             if (mouseButtons != 0)
            {
                // If the mouse drags off the surface we will miss the mouse up
                // TODO:  mouse should be hooked.
                if (point.IsInContact)
                {
                    var dr = c1 - mousePosition;
                    dr *= 1.0f/cameraZoom;
                    cameraC -= dr;
                }
            }

            mousePosition = c1;
        }
        private void EventTimeTravelSliderChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var dt = TimeSpan.FromMinutes(e.NewValue);
            var serverTime = JSClient.ServerTime() + TimeSpan.FromMinutes(e.NewValue);
            eventTimeTravelText.Text = $"Attack Time Travel:\t\t{dt.Hours}:{dt.Minutes},\t\tT:{serverTime.Hour}:{serverTime.Minute:D2}";
        }

        private static bool TryGetSpot(object sender, out Spot spot)
        {
            spot = null;
            var from = sender as MenuFlyoutItem;
            if (from == null)
                return false;

            spot = from.DataContext as Spot;
            return (spot != null);
        }
        private void CityFlyoutView(object sender, RoutedEventArgs e)
        {
            if (TryGetSpot(sender, out var spot))
            {
                JSClient.ViewCity(spot.cid);
            }
        }

        private void CityFlyoutInfo(object sender, RoutedEventArgs e)
        {
            if (TryGetSpot(sender, out var spot))
            {
                JSClient.ChangeCity(spot.cid,true);
            }

        }
    }
}
