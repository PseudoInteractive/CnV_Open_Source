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
            coreInputSource.PointerEntered += CoreInputSource_PointerEntered; ;
            coreInputSource.PointerExited += Canvas_PointerExited;

            coreInputSource.PointerWheelChanged += Canvas_PointerWheelChanged;
          //  coreInputSource.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessUntilQuit);
            coreInputSource.IsInputEnabled = true;


        }

        private void CoreInputSource_PointerEntered(object sender, PointerEventArgs args)
        {
            App.DispatchOnUIThreadLow(() => canvas.Focus(FocusState.Programmatic));
        }

        static Vector2 GetCanvasPosition(Windows.UI.Input.PointerPoint screenC)
        {
            var point = screenC.Position;
            return new Vector2((float)point.X , (float)point.Y );
        }
        private void Canvas_PointerReleased(object sender, PointerEventArgs e)
        {
            if (JSClient.IsCityView())
            {
                e.Handled = false;
                return;
            }

            var pointerPoint = e.CurrentPoint;
            mousePosition = GetCanvasPosition(pointerPoint);
            e.Handled = false;

            //            mousePosition = point.Position.ToVector2();
            if ((lastMousePressPosition - mousePosition).Length() < 12.0f)
            {
                var worldC = MousePointToWorld(mousePosition);
                var cid = worldC.WorldToCid();

                switch (pointerPoint.Properties.PointerUpdateKind)
                {
                    case Windows.UI.Input.PointerUpdateKind.LeftButtonReleased:
                        {
                            Spot.ProcessCoordClick(cid, true);
                            e.Handled = true;
                            break;
                        }

                    case Windows.UI.Input.PointerUpdateKind.RightButtonReleased:
                        {
                            var position = pointerPoint.Position;
							App.DispatchOnUIThread(() =>
							{

								var spot = Spot.GetOrAdd(cid);

								spot.ShowContextMenu(canvas, position);

							});
                            break;
                        }
                    case Windows.UI.Input.PointerUpdateKind.XButton1Released:
                        {
                            NavStack.Back();
                        }
                        break;
                    case Windows.UI.Input.PointerUpdateKind.XButton2Released:
                        {
                            NavStack.Forward();

                            break;
                        }
                    default:
                        break;
                }

            }
            else
            {
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
           
            var properties = point.Properties;
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


            mousePosition = GetCanvasPosition(point);
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
            e.Handled = false;

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
            var cBase = GetCanvasPosition(pt) - halfSpan;
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
            var dc1 = (c1 );
            dc1 -= ShellPage.halfSpan;
            dc1 *= (1.0f / cameraZoomLag);
            dc1 += ShellPage.cameraC;

            int x = (dc1.X).RoundToInt();
            int y = (dc1.Y).RoundToInt();
            return (x, y);
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
            var c1 = GetCanvasPosition(point);
            
            var c = MousePointToWorld(c1);
            var props = point.Properties;
            if ((props.IsLeftButtonPressed|props.IsRightButtonPressed) ==false)
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
                    Spot.uiPress = cid;
                    Spot.viewHover = 0;
                    toolTip = null;

                    lastCanvasC = cid;
                    var packedId = World.GetPackedId(c);
                    var data = World.CityLookup(packedId);
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
                    if(World.rawPrior!=null)
                    {
                        var pData = World.CityLookupPrior(packedId);
                        if(pData.data == data.data | pData.type == World.typeBoss | pData.type == World.typeDungeon)
                        {
                            // no change

                        }
                        else
                        {
                            switch(data.type)
                            {
                                case World.typeCity:
                                    if(pData.type == World.typeCity)
                                    {
                                        if (pData.player != data.player)
                                        {
                                            if (pData.player == 0)
                                            {
                                                toolTip += "\nWas settled";
                                            }
                                            else if (data.player == 0)
                                            {
                                                toolTip += "\nWas abandoned";
                                            }
                                            else
                                            {
                                                var player = Player.all.GetValueOrDefault(pData.player, Player._default);
                                                toolTip += $"\nWas owned by:\n{player.name}\n{player.allianceName}";
                                            }
                                        }
                                        else
                                        {
                                            toolTip += "\nWas rennovated";
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
                                    if( data.player == 0)
                                        toolTip += "\nWas active";
                                    else
                                        toolTip += "\nWas inactive";
                                    break;
                                default:
                                    toolTip += "\nDecayed";
                                    break;

                            }
                        }
                    }
                }
                e.Handled = false;

            }
            else
            {
                // If the mouse drags off the surface we will miss the mouse up
                // TODO:  mouse should be hooked.
               // if (point.IsInContact)
                {
                    var dr = c1;
                    dr -= mousePosition;
                    dr *= 1.0f/cameraZoomLag;
                    cameraC -= dr;
                    // instant
                    cameraCLag = cameraC;
                    e.Handled = true;

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

        
    }
}
