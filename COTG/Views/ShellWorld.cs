﻿using COTG.Game;
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
		public static Vector3 mousePositionW; // in warped space
		public static Vector2 lastMousePressPosition;
		public static DateTimeOffset lastMousePressTime;

		public float eventTimeOffset;
		public static string toolTip;
		public static string contToolTip;
		public static int lastCont;



		public static void SetupCoreInput()
		{

		//	var workItemHandler = new WorkItemHandler((action) =>
			{
				var inputDevices = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch;
				coreInputSource = canvas.CreateCoreIndependentInputSource(inputDevices);


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

		private static void CoreInputSource_PointerCaptureLost(object sender, PointerEventArgs args)
		{
			Log("pointer lost");
		
		}

		private static void CoreInputSource_PointerEntered(object sender, PointerEventArgs args)
		{
			isOverPopup = false;
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

		static Vector2 GetCanvasPosition(Windows.Foundation.Point screenC)
		{
			var point = screenC;
			return new Vector2((float)(point.X*dipToNative), (float)(point.Y*dipToNative) );
		}
		// private static void Canvas_PointerReleased(object sender, PointerEventArgs e)
		// {

		//     e.KeyModifiers.UpdateKeyModifiers();
		//     var pointerPoint = e.CurrentPoint;
		//     var position = pointerPoint.Position;
		//     mousePosition = GetCanvasPosition(position.ToVector2());
		//     e.Handled = false;

		//     //            mousePosition = point.Position.ToVector2();
		//     if ((lastMousePressPosition - mousePosition).Length() < 8)
		//     {
		//         var worldC = ScreenToWorld(mousePosition);
		//         var cid = worldC.WorldToCid();

		//         switch (pointerPoint.Properties.PointerUpdateKind)
		//         {
		//             case Windows.UI.Input.PointerUpdateKind.LeftButtonReleased:
		//                 {
		//                     Spot.ProcessCoordClick(cid, true,e.KeyModifiers,true);
		//                     e.Handled = true;
		//                     break;
		//                 }

		//             case Windows.UI.Input.PointerUpdateKind.RightButtonReleased:
		//                 {

		//App.DispatchOnUIThreadSneaky(() =>
		//{
		//                      //   var _underMouse = underMouse;
		//                         //if (_underMouse!=null)
		//                         //{
		//                         //    var flyout = new MenuFlyout();

		//                         //    var target = Spot.GetOrAdd(_underMouse.targetCid);
		//                         //    var source = Spot.GetOrAdd(_underMouse.sourceCid);
		//                         //    void ShowContextContext(object sender, RoutedEventArgs args)
		//                         //    {
		//                         //        ((sender as MenuFlyoutItem).DataContext as Spot).ShowContextMenu(canvas, position);
		//                         //    }
		//                         //    AApp.AddItem(flyout, "Source", ShowContextContext, source );
		//                         //    AApp.AddItem(flyout, "Target", ShowContextContext, target);
		//                         //    flyout.ShowAt(canvas, pointerPoint.Position);
		//                         //}
		//                         //else
		//                       //  {
		//                             var spot = Spot.GetOrAdd(cid);
		//		if (!e.KeyModifiers.IsShift())
		//			spot.SetFocus(true, true,false);
		//                             spot.ShowContextMenu(canvas, position);
		//                        // }

		//});
		//                     break;
		//                 }
		//             case Windows.UI.Input.PointerUpdateKind.MiddleButtonReleased:
		//                 {
		//                     App.DispatchOnUIThreadSneaky(() =>
		//                     {

		//                         var spot = Spot.GetOrAdd(cid);

		//                         var text = spot.ToTsv();
		//                         Note.Show($"Copied to clipboard: {text}");
		//                         App.CopyTextToClipboard(text);
		//                         spot.SelectMe(true,App.keyModifiers);

		//                     });
		//                     break;
		//                 }
		//             //case Windows.UI.Input.PointerUpdateKind.XButton1Released:
		//             //    {
		//             //        NavStack.Back();
		//             //    }
		//             //    break;
		//             //case Windows.UI.Input.PointerUpdateKind.XButton2Released:
		//             //    {
		//             //        NavStack.Forward();

		//             //        break;
		//             //    }
		//             default:
		//                 break;
		//         }

		//     }
		//     else
		//     {
		//     }
		// }




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
			//    ShellPage.SetJSCamera(regionC);
		}

		//public static void Canvas_PointerPressed(MouseState current, MouseState priorState)
		//{
		//	var leftChanged = current.LeftButton != priorState.LeftButton;
		//	var rightChanged = current.RightButton != priorState.RightButton;
		//	var x1Changed = current.XButton1 != priorState.XButton1;
		//	var x2Changed = current.XButton2 != priorState.XButton2;
		//	if (!(leftChanged | rightChanged | x1Changed | x2Changed))
		//		return;


		//	//  if (ShellPage.IsCityView())
		//	if (x1Changed && current.XButton1 == ButtonState.Pressed)
		//	{
		//		NavStack.Back(true);
		//		return;
		//	}

		//	if (x2Changed && current.XButton2 == ButtonState.Pressed)
		//	{
		//		NavStack.Forward(true);
		//		return;
		//	}

		//	if (leftChanged || rightChanged)
		//	{
		//		if (current.LeftButton == ButtonState.Pressed || current.RightButton == ButtonState.Pressed)
		//		{
		//			lastMousePressTime = DateTimeOffset.UtcNow;
		//			lastMousePressPosition = mousePosition;

		//		}
		//		else
		//		{

		//			if ((lastMousePressPosition - mousePosition).Length() < 8)
		//			{
		//				var worldC = ScreenToWorld(mousePositionW);
		//				var cid = worldC.WorldToCid();

		//				{
		//					if (priorState.LeftButton == ButtonState.Pressed)
		//					{
		//						Spot.ProcessCoordClick(cid, true, App.canvasKeyModifiers, true);

		//					}
		//					if (priorState.RightButton == ButtonState.Pressed)
		//					{
		//						App.DispatchOnUIThreadSneaky(() =>
		//						{
		//							//   var _underMouse = underMouse;
		//							//if (_underMouse!=null)
		//							//{
		//							//    var flyout = new MenuFlyout();

		//							//    var target = Spot.GetOrAdd(_underMouse.targetCid);
		//							//    var source = Spot.GetOrAdd(_underMouse.sourceCid);
		//							//    void ShowContextContext(object sender, RoutedEventArgs args)
		//							//    {
		//							//        ((sender as MenuFlyoutItem).DataContext as Spot).ShowContextMenu(canvas, position);
		//							//    }
		//							//    AApp.AddItem(flyout, "Source", ShowContextContext, source );
		//							//    AApp.AddItem(flyout, "Target", ShowContextContext, target);
		//							//    flyout.ShowAt(canvas, pointerPoint.Position);
		//							//}
		//							//else
		//							//  {
		//							var spot = Spot.GetOrAdd(cid);
		//							if (!App.canvasKeyModifiers.IsShift())
		//								spot.SetFocus(true, true, false);
		//							spot.ShowContextMenu(canvas, new Windows.Foundation.Point(mouseState.Position.X, mouseState.Position.Y));
		//							// }

		//						});
		//					}
		//					//case Windows.UI.Input.PointerUpdateKind.MiddleButtonReleased:
		//					//	{
		//					//		App.DispatchOnUIThreadSneaky(() =>
		//					//		{

		//					//			var spot = Spot.GetOrAdd(cid);

		//					//			var text = spot.ToTsv();
		//					//			Note.Show($"Copied to clipboard: {text}");
		//					//			App.CopyTextToClipboard(text);
		//					//			spot.SelectMe(true, App.keyModifiers);

		//					//		});
		//					//		break;
		//					//	}
		//					//case Windows.UI.Input.PointerUpdateKind.XButton1Released:
		//					//    {
		//					//        NavStack.Back();
		//					//    }
		//					//    break;
		//					//case Windows.UI.Input.PointerUpdateKind.XButton2Released:
		//					//    {
		//					//        NavStack.Forward();

		//					//        break;
		//					//    }
		//				}
		//			}
		//		}
		//		ClearHover();
		//	}
		//	//          if ((lastMousePressTime - prior).TotalSeconds < 2.5f)
		//	//          {
		//	//              // double click

		//	//              var c = MousePointToWorld(e.GetCurrentPoint(canvas).Position.ToVector2());
		//	//              var data = World.CityLookup(c);
		//	//              switch (data.type)
		//	//              {
		//	//                  case World.typeCity:
		//	//                      {
		//	//                          if (data.player == 0)
		//	//                          {

		//	//                              //                               toolTip = $"Lawless\n{c.y / 100}{c.x / 100} ({c.x}:{c.y})";
		//	//                          }
		//	//                          else
		//	//                          {
		//	//                              var player = Player.all.GetValueOrDefault(data.player, Player._default);
		//	//                              if (Player.IsMe(data.player))
		//	//                              {
		//	//                              //    ShellPage.ViewCity(c.WorldToCid());
		//	//                              }
		//	//                              else
		//	//                              {
		//	////                                  toolTip = $"{player.name}\n{Alliance.IdToName(player.alliance)}\n{c.y / 100}{c.x / 100} ({c.x}:{c.y})\ncities:{player.cities}\npts:{player.pointsH * 100}";
		//	//                              }


		//	//                          }
		//	//                          break;
		//	//                      }
		//	//              }
		//	//              }
		//	//    ChatTab.L("CPress " + e.GetCurrentPoint(canvas).Position.ToString());

		//	//  e.Handled = false;

		//}
		public static void ClearHover()
		{
			contToolTip = null;
			lastCanvasC = 0;
			lastCont = -1;
			toolTip = null;
			CityView.hovered = CanvasHelpers.invalidXY;
			Spot.viewHover = 0;
			Player.viewHover = 0;
		}

		private static void Canvas_PointerExited(object sender, PointerEventArgs e)
		{
			Log("pointer Exit " + isOverPopup);
			isOverPopup = false;
			//if (ShellPage.IsCityView())
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

		//public static void Canvas_PointerWheelChanged(MouseState current, MouseState prior)
		//{
		//	var delta = current.ScrollWheelValue - prior.ScrollWheelValue;
		//	if (delta == 0)
		//		return;
		//	//if (ShellPage.IsCityView())
		//	//{
		//	//    e.Handled = false;
		//	//    return;
		//	//}


		//	var dZoom = delta.SignOr0() * 0.0625f + delta * (1.0f / 1024.0f);
		//	var newZoom = (cameraZoom * MathF.Exp(dZoom)).Clamp(1, 160.0f);
		//	var cBase = mousePosition - halfSpan;
		//	var c0 = cBase / cameraZoom;
		//	var c1 = cBase / newZoom;
		//	cameraC = cameraC + c0 - c1;

		//	cameraZoom = newZoom;

		//	ClearHover();
		//	//    ChatTab.L("CWheel " + wheel);
		//}
		static public (int x, int y) ScreenToWorldC(Vector3 c1)
		{
			return (((c1.X) / cameraZoomLag + cameraC.X).RoundToInt(), ((c1.Y) / cameraZoomLag + cameraC.Y).RoundToInt());
		}
		static public ((int x, int y) wc, (int x, int y) cc) ScreenToWorldAndCityC(Vector3 c1)
		{
			var w = new Vector2(((c1.X) / cameraZoomLag + cameraC.X), ((c1.Y) / cameraZoomLag + cameraC.Y));
			(int x, int y) wi = (w.X.RoundToInt(), w.Y.RoundToInt());
			(int x, int y) bi = (((w.X - wi.x)*City.citySpan).RoundToInt().Clamp(City.span0,City.span1), ((w.Y - wi.y) * City.citySpan/CityView.yScale).RoundToInt().Clamp(City.span0, City.span1));

			return (wi, bi);
		}

		//static public Vector2 CameraToWorld(Vector2 c1)
		//{
		//	return new Vector2( (c1.X-halfSpan.X)/cameraZoomLag + cameraC.X, (c1.Y - halfSpan.Y) / cameraZoomLag + cameraC.Y) ;
		//}

		static public int lastCanvasC;
		//public static void Canvas_PointerMoved(MouseState current, MouseState prior)
		//{
		//	if (current.X == prior.X && current.Y == prior.Y)
		//		return;
		//	var priorMouseC = mousePosition;
		//	mousePosition = GetCanvasPosition(current.Position.ToV2());
		//	mousePositionC = mousePosition.SToC();
		//	//	cameraLightC = new Vector2((float)mousePosition.X,(float)mousePosition.Y);
		//	mousePositionW = mousePositionC.InverseProject();
		//	//if(ShellPage.IsCityView() )
		//	// {
		//	//     e.Handled = false;
		//	//     return;
		//	// }

		//	var c = ScreenToWorld(mousePositionW);
		//	if ((current.LeftButton == ButtonState.Pressed | current.RightButton == ButtonState.Pressed) == false)
		//	{
		//		var cont = Continent.GetPackedIdFromC(c);
		//		if (cont != lastCont)
		//		{
		//			lastCont = cont;
		//			ref var cn = ref Continent.all[cont];
		//			contToolTip = $"{cn.id}\nSettled {cn.settled}\nFree {cn.unsettled}\nCites {cn.cities}\nCastles {cn.castles}\nTemples {cn.temples}\nDungeons {cn.dungeons}";

		//		}
		//		var cid = c.WorldToCid();
		//		if (lastCanvasC != cid)
		//		{
		//			//   Spot.uiPress = cid;
		//			Spot.viewHover = 0;
		//			Player.viewHover = 0;
		//			toolTip = null;

		//			lastCanvasC = cid;
		//			var packedId = World.GetPackedId(c);
		//			var data = World.GetInfoFromPackedId(packedId);
		//			switch (data.type)
		//			{
		//				case World.typeCity:
		//					{
		//						Spot.viewHover = cid;



		//						Spot.TryGet(cid, out var spot);

		//						if (data.player == 0)
		//						{
		//							toolTip = $"Lawless\n{c.y / 100}{c.x / 100} ({c.x}:{c.y})";
		//						}
		//						else
		//						{
		//							Player.viewHover = data.player;

		//							var player = Player.all.GetValueOrDefault(data.player, Player._default);
		//							if (Player.IsMe(data.player))
		//							{
		//								if (spot is City city)
		//								{
		//									var notes = city.remarks.IsNullOrEmpty() ? "" : city.remarks.Substring(0, city.remarks.Length.Min(40)) + "\n";
		//									toolTip = $"{player.name}\n{city.cityName}\npts:{city.points}\n{Alliance.IdToName(player.alliance)}\nTSh:{city.tsHome}\nTSt:{city.tsTotal}\n{notes}{c.y / 100}{c.x / 100} ({c.x}:{c.y})";
		//									//     Raiding.UpdateTS();
		//								}

		//							}
		//							else
		//							{
		//								var info = spot != null ?
		//									$"{spot.cityName}\n{spot.points}\n"
		//								 : "";
		//								toolTip = $"{player.name}\n{Alliance.IdToName(player.alliance)}\n{info}{c.y / 100}{c.x / 100} ({c.x}:{c.y})\ncities:{player.cities}\npts:{player.pointsH * 100}";
		//							}
		//						}
		//						if (spot != null && spot.incoming != null)
		//						{
		//							var inc = spot.incoming;
		//							foreach (var i in inc)
		//							{
		//								if (i.isAttack)
		//								{
		//									toolTip = toolTip + '\n' + i.Format("\n");
		//								}
		//							}
		//							var def = Array.Empty<TroopTypeCount>();
		//							foreach (var i in inc)
		//							{
		//								if (!i.isAttack)
		//								{
		//									def = def.Sum(i.troops);
		//								}
		//							}
		//							if (!def.IsNullOrEmpty())
		//							{
		//								toolTip += def.Format("\nDef:", '\n', '\n');
		//							}

		//						}
		//						break;
		//					}
		//				case World.typeShrine:
		//					toolTip = $"Shrine\n{(data.player == 255 ? "Unlit" : "Lit")}";
		//					break;
		//				case World.typeBoss:
		//					toolTip = $"Boss\nLevel:{data.player & 0xf}"; // \ntype:{data >> 4}";
		//					break;
		//				case World.typeDungeon:
		//					toolTip = $"Dungeon\nLevel:{data.player & 0xf}"; // \ntype:{data >> 4}";
		//					break;
		//				case World.typePortal:
		//					toolTip = $"Portal\n{(data.player == 0 ? "Inactive" : "Active")}";
		//					break;
		//			}
		//			if (World.rawPrior != null)
		//			{
		//				var pData = World.GetInfoPrior(packedId);
		//				if (pData.data == data.data | pData.type == World.typeBoss | pData.type == World.typeDungeon)
		//				{
		//					// no change

		//				}
		//				else
		//				{
		//					switch (data.type)
		//					{
		//						case World.typeCity:
		//							if (pData.type == World.typeCity)
		//							{
		//								if (pData.player != data.player)
		//								{
		//									if (pData.player == 0)
		//									{
		//										toolTip += "\nWas settled";
		//									}
		//									else if (data.player == 0)
		//									{
		//										var player = Player.all.GetValueOrDefault(pData.player, Player._default);
		//										toolTip += $"\nWas abandoned by:\n{player.name}\n{player.allianceName}";
		//									}
		//									else
		//									{
		//										var player = Player.all.GetValueOrDefault(pData.player, Player._default);
		//										toolTip += $"\nWas owned by:\n{player.name}\n{player.allianceName}";
		//									}
		//								}
		//								else
		//								{
		//									if (pData.isTemple != data.isTemple)
		//									{
		//										if (data.isTemple)
		//											toolTip += "\nBecame a Temple";
		//										else
		//											toolTip += "\nBecame not a temple";
		//									}
		//									else if (pData.isCastle != data.isCastle)
		//									{
		//										toolTip += "\nWas castled";
		//									}
		//									else
		//									{
		//										toolTip += "\nWas rennovated";
		//									}
		//								}
		//							}
		//							else
		//							{
		//								toolTip += "\nWas founded";
		//							}
		//							break;
		//						case World.typeShrine:
		//							toolTip += "\nWas unlit";
		//							break;
		//						case World.typePortal:
		//							if (data.player == 0)
		//								toolTip += "\nWas active";
		//							else
		//								toolTip += "\nWas inactive";
		//							break;
		//						default:
		//							if (pData.player != 0)
		//								toolTip += $"\nDecayed (was {Player.IdToName(pData.player)})";
		//							else
		//								toolTip += "\nDecayed";
		//							break;

		//					}
		//				}
		//			}
		//		}


		//	}
		//	else
		//	{
		//		// If the mouse drags off the surface we will miss the mouse up
		//		// TODO:  mouse should be hooked.
		//		// if (point.IsInContact)
		//		{
		//			var dr = mousePosition - priorMouseC;
		//			dr *= 1.0f / cameraZoomLag;
		//			cameraC -= dr;
		//			// instant
		//			cameraCLag = cameraC;


		//		}
		//	}


		//}
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
			var position = pointerPoint.Position;
			mousePosition = GetCanvasPosition(position);

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
			if ((lastMousePressPosition - mousePosition).Length() < 8)
			{
				//if(wasOverPopup)
				//{
				//	PostJSMouseEvent("click", jsButton);
				//	return;
				//}
				
				(var worldC, var cc) = ScreenToWorldAndCityC(mousePositionW);
				var cid = worldC.WorldToCid();

				switch (pointerPoint.Properties.PointerUpdateKind)
				{
					case Windows.UI.Input.PointerUpdateKind.LeftButtonReleased:
						{
							if (IsCityView() && (cid == City.build))
							{
								CityView.selected = cc;
								App.DispatchOnUIThreadSneaky(() =>
								{
									var b = City.GetBuild().GetBuiding(cc);
									var d = b.def;
									var i = CityBuild.instance;
									i.Building.Text = d.Bn;
									i.Description.Text = d.Ds;
									i.Upgrade.IsEnabled = d.Bc.Count() > b.bl && b.bl != 0;
									i.Downgrade.IsEnabled = b.bl > 1;
									i.rect.Fill = CityBuild.BuildingBrush(d.bid);



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

					case Windows.UI.Input.PointerUpdateKind.RightButtonReleased:
						{

							App.DispatchOnUIThreadSneaky(() =>
							{
								//   var _underMouse = underMouse;
								//if (_underMouse!=null)
								//{
								//    var flyout = new MenuFlyout();

								//    var target = Spot.GetOrAdd(_underMouse.targetCid);
								//    var source = Spot.GetOrAdd(_underMouse.sourceCid);
								//    void ShowContextContext(object sender, RoutedEventArgs args)
								//    {
								//        ((sender as MenuFlyoutItem).DataContext as Spot).ShowContextMenu(canvas, position);
								//    }
								//    AApp.AddItem(flyout, "Source", ShowContextContext, source );
								//    AApp.AddItem(flyout, "Target", ShowContextContext, target);
								//    flyout.ShowAt(canvas, pointerPoint.Position);
								//}
								//else
								//  {
								var spot = Spot.GetOrAdd(cid);
								if (!e.KeyModifiers.IsShift())
									spot.SetFocus(true, true, false);
								spot.ShowContextMenu(canvas, position);
								// }

							});
							break;
						}
					case Windows.UI.Input.PointerUpdateKind.MiddleButtonReleased:
						{
							App.DispatchOnUIThreadSneaky(() =>
							{

								var spot = Spot.GetOrAdd(cid);

								var text = spot.ToTsv();
								Note.Show($"Copied to clipboard: {text}");
								App.CopyTextToClipboard(text);
								spot.SelectMe(true, App.keyModifiers);

							});
							break;
						}
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
		static public bool isOverPopup;
		private static bool TryPostJSMouseEvent(string eventName, int button)
		{
			foreach (var popup in AGame.popups)
			{
				// should this be in DIPS or pixels?
				if (popup.Contains(mousePosition))
				{
					if(eventName!=null)
						JSClient.PostMouseEventToJS((int)mousePosition.X + canvasBaseX, (int)mousePosition.Y + canvasBaseY, eventName, button );
					return true;
				}
			}
			return false;
		}

		private static void PostJSMouseEvent(string eventName, int button, int dx=0, int dy=0)
		{
			if (eventName != null)
				JSClient.PostMouseEventToJS((int)mousePosition.X + canvasBaseX, (int)mousePosition.Y + canvasBaseY, eventName, button, dx, dy);
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


		private static void Canvas_PointerPressed(object sender, PointerEventArgs e)
		{
			e.KeyModifiers.UpdateKeyModifiers();
			if (!isHitTestVisible)
				return;

			Assert(isOverPopup == false);
			//            canvas.CapturePointer(e.Pointer);
			var point = e.CurrentPoint;

			var properties = point.Properties;
			mousePosition = GetCanvasPosition(point.Position);
			var prior = lastMousePressTime;
			lastMousePressTime = DateTimeOffset.UtcNow;
			lastMousePressPosition = mousePosition;

			
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
				}))
			{
//				isOverPopup = true;
				e.Handled = true;
				ShellPage.SetWebViewHasFocus(true);

			}
			else
			{
				App.DispatchOnUIThreadLow(() => ShellPage.keyboardProxy.Focus(FocusState.Programmatic));
			}

			ClearHover();
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
			App.DispatchOnUIThreadLow(() => ShellPage.keyboardProxy.Focus(FocusState.Programmatic) );
			ClearHover();
			//  e.Handled = false;

		}
		//private void Canvas_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
		//{
		//    mouseButtons = 0;
		//    Spot.viewHover = 0;
		//}
		

		private static void Canvas_PointerWheelChanged(object sender, PointerEventArgs e)
		{
			//if (ShellPage.IsCityView())
			//{
			//    e.Handled = false;
			//    return;
			//}

			var pt = e.CurrentPoint;
			var wheel = pt.Properties.MouseWheelDelta;
			var dZoom = wheel.SignOr0() * 0.0625f + wheel * (1.0f / 1024.0f);
			var newZoom = (cameraZoom * MathF.Exp(dZoom)).Clamp(1, maxZoom);
			var cBase = GetCanvasPosition(pt.Position) - halfSpan;
			var c0 = cBase / cameraZoom;
			var c1 = cBase / newZoom;
			cameraC = cameraC + c0 - c1;

			var _viewMode =  newZoom >= cityZoomThreshold ? ViewMode.city: ViewMode.world;
			if(_viewMode !=viewMode)
			{
				ShellPage.SetViewMode(_viewMode);
			}

			cameraZoom = newZoom;
			e.Handled = true;
			ClearHover();
			//    ChatTab.L("CWheel " + wheel);
		}

		private static void Canvas_PointerMoved(object sender, PointerEventArgs e)
		{
			e.KeyModifiers.UpdateKeyModifiers();
			if (!isHitTestVisible)
				return;
			var priorMouseC = mousePosition;
			var windowsPosition = e.CurrentPoint.Position;
			mousePosition = GetCanvasPosition(windowsPosition);
			mousePositionC = mousePosition.SToC();
			//	//	cameraLightC = new Vector2((float)mousePosition.X,(float)mousePosition.Y);
			mousePositionW = mousePositionC.InverseProject();

			//if(ShellPage.IsCityView() )
			// {
			//     e.Handled = false;
			//     return;
			// }
			var point = e.CurrentPoint;


			(var c,var cc) = ScreenToWorldAndCityC(mousePositionW);
			var props = point.Properties;
			if ((props.IsLeftButtonPressed | props.IsRightButtonPressed) == false)
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
						var b = City.GetBuild().GetBuiding(cc);
						var d = b.def;
						contToolTip = $"({cc.x},{cc.y})\n{d.Bn} {b.bl}";
						Spot.viewHover = 0;
						Player.viewHover = 0;
						toolTip = null;
						CityView.hovered = cc;
					}
					else
					{ 

						if (cont != lastCont)
						{
							lastCont = cont;
							if (!IsCityView())
							{
								ref var cn = ref Continent.all[cont];
								contToolTip = $"{cn.id}\nSettled {cn.settled}\nFree {cn.unsettled}\nCites {cn.cities}\nCastles {cn.castles}\nTemples {cn.temples}\nDungeons {cn.dungeons}";

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
											if (Player.IsMe(data.player))
											{
												if (spot is City city)
												{
													var notes = city.remarks.IsNullOrEmpty() ? "" : city.remarks.Substring(0, city.remarks.Length.Min(40)) + "\n";
													toolTip = $"{player.name}\n{city.cityName}\npts:{city.points}\n{Alliance.IdToName(player.alliance)}\nTSh:{city.tsHome}\nTSt:{city.tsTotal}\n{notes}{c.y / 100}{c.x / 100} ({c.x}:{c.y})";
													//     Raiding.UpdateTS();
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
			else
			{
				var dr = mousePosition - priorMouseC;
				{
					dr *= 1.0f / cameraZoomLag;
					cameraC -= dr;
					// instant
					cameraCLag = cameraC;
					e.Handled = true;

				}
				//else
				{

			//		PostJSMouseEvent("mousemove",0, (int)dr.X,(int)dr.Y);
				}
			}


		}

	}
}
