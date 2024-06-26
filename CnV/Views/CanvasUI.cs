﻿using Microsoft.UI.Input;

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

using System.Numerics;

partial class ShellPage
	{
	static Vector2 lastMovePosition;

	internal static void ViewChanged() {
		QueueOnPropertyChanged(nameof(viewZ));
		//QueueOnPropertyChanged(nameof(viewX));
		//QueueOnPropertyChanged(nameof(viewY));
	}

	private static void Canvas_PointerMoved((Windows.Foundation.Point Position, uint PointerId,
																			bool IsInContact, ulong Timestamp, PointerUpdateKind PointerUpdateKind, bool isPrimary) point)
		{
		if(!point.isPrimary)
			return;
			//			if(!mouseOverCanvas)
			//				Log("Mouse Moved Canvas");
			//	App.cursorDefault.Set();
			// prevent idle timer;
			mouseOverCanvas = true;
		//	instance.mouseOverCanvasBox.IsChecked = mouseOverCanvas;

			//	PointerInfo(e);
			UpdateMousePosition(point.Position);

		if(point.IsInContact) {
			var dr = mousePosition- lastMovePosition;
			dr *= -1.0f.ScreenToWorld();
			//dr.X *= -1;
			var lg = dr.Length();
			if(lg < 128f) {
				View.SetViewTarget(View.viewW2 + dr,true);
			}
		}
		lastMovePosition = mousePosition;

			//	ShellPage.TakeFocus();
			//UpdateFocus();
			//	if (!isFocused)
			//		return;
			//	var priorMouseC = mousePosition;
			//var gestureResult = Gesture.ProcessMoved(point);
			//if(gestureResult.action == GestureAction.none)
			//		return;

			//SetMousePosition( gestureResult.c);

			//			var windowsPosition = e.CurrentPoint.Position;
			//			mousePosition = GetCanvasPosition(windowsPosition);
			//	mousePositionW = mousePosition.ScreenToWorld();
			//		mousePositionW = mousePositionC.InverseProject();

			(var c, var cc) = ToWorldAndCityC(mousePositionW);
			//var point = e.CurrentPoint;
			//var props = point.Properties;
			if(!recognizer.IsActive  && c.isInWorld)
			{
				//if (TryPostJSMouseEvent(null, 0))
				//{
				//	// mouse over popup
				//}
				//else
				{

					var cont = Continent.GetPackedIdFromC(c);
					var cid = c.cid;
					if(IsCityView())
					{
						var build = City.GetBuild();
						if(build != null)
						{
							//	var b = build.GetBuiding(cc);
							//var d = b.def;
							//	contToolTip = $"({cc.x},{cc.y})\n{d.Bn} {b.bl}";
							Spot.viewHover = default;
							Player.SetViewHover( PlayerId.MaxValue );
							//ToolTips.spotToolTip = null;
							CityView.hovered = cc;
							CityBuild.PreviewBuildAction();

						}
					}
					else
					{

						if(cont != lastCont)
						{
							lastCont = cont;
							if(!IsCityView())
							{
								//ref var cn = ref Continent.all[cont];
								//contToolTip = $"{World.UnpackContinent(cont)}\nSettled {cn.settled}\nFree {cn.unsettled}\nCities {cn.cities}\nCastles {cn.castles}\nTemples {cn.temples}\nDungeons {cn.dungeons}";

							}
						}


						if(lastCanvasC != cid)
						{

							//Spot.viewHover = default;
							var newViewHover = PlayerId.MaxValue;
							ref string toolTip = ref ToolTips.spotToolTip;
							toolTip = null;

							lastCanvasC = cid;
							var packedId = World.GetWorldId(c);
							var data = World.GetTile(World.rawPrior1!=null ? World.rawPrior1 : World.tileData,packedId);
							Spot.viewHover = (WorldC)cid;
							switch(data.type)
							{
								case World.TileType.typeCity:
									{
										
										City.TryGet(cid, out var city);
										
										if(city != null)
										{
											toolTip = city.toolTip;
											if(data.player == 0)
											{
											}
											else
											{
												newViewHover = data.player;
												
											}
										}
										break;
									}
								case World.TileType.typeShrine:
								//	if(WorldViewSettings.shrines.isOn)
										toolTip = Shrine.FromCid(cid).toolTip;
								
									break;
								case World.TileType.typeMoongate:
									toolTip = $"Moongate\n{cid.CidToString()}";
									break;
							default:
								if(data.isDungeonOrBoss)
								{
									var cavern = Cavern.Get(cid);
									toolTip = cavern.Format(); // \ntype:{data >> 4}";
								}
								break;
							}
							Player.SetViewHover(newViewHover);

							if(World.rawPrior0 != null)
							{
								var pData = World.GetTile(World.rawPrior0,packedId);
								if(pData.data == data.data)
								{
									// no change

								}
								else
								{
									switch(data.type)
									{
										case World.TileType.typeCity:
											if(pData.type == World.TileType.typeCity)
											{
												if(pData.player != data.player)
												{
													if(pData.player == 0)
													{
														toolTip += "\nLawless was settled";
													}
													else if(data.player == 0)
													{
														var player = Player.GetValueOrDefault(pData.player,Player.none);
														toolTip += $"\nWas abandoned by:\n{player.name}\n{player.allianceName}";
													}
													else
													{
														var player = Player.GetValueOrDefault(pData.player,Player.none);
														toolTip += $"\nWas captured from:\n{player.name}\n{player.allianceName}";
													}
												}
												else
												{
													if(pData.isTemple != data.isTemple)
													{
														if(data.isTemple)
															toolTip += "\nBecame a Temple";
														else
															toolTip += "\nBecame not a temple";
													}
													else if(pData.isCastle != data.isCastle)
													{
														toolTip += "\nWas castled";
													}
													else if(data.isBig)
													{
														toolTip += "\nWas rennovated";
													}
													else
													{
														toolTip += "\nWas flattened";
													}
												}
											}
											else
											{
												toolTip += "\nWas founded";
											}
											break;
										case World.TileType.typeShrine:
											toolTip += "\nWas unlit";
											break;
										case World.TileType.typeMoongate:
											if(data.player == 0)
												toolTip += "\nWas active";
											else
												toolTip += "\nWas inactive";
											break;
										default:
											if(pData.player != 0)
												toolTip += $"\nDecayed (was {Player.IdToName(pData.player)})";
											else
												toolTip += "\nLawless Decayed";
											break;

									}
								}
							}
							
						if(toolTip != null)
								ToolTips.spot = City.Get(cid);;

							//{
							//	StringBuilder sb = new(toolTip);
							//	var info = TileData.instance.GetSpotinfo(c.x,c.y,sb);
							//	sb.Append($"\nOnWater:{data.isOnWater}\nShoreline:{info.shoreline}\nOcean:{info.isOcean}");
							////	Assert(data.type == info.type);

							//	toolTip = sb.ToString();
							//}
							ToolTipWindow.TipChanged();
							World.RefreshTintDataForCurrentContinent( );
						}
					}
				}
				//				e.Handled = false;

			}

			//else
			{
				////			e.Handled = true;
				//if(gestureResult.action.HasFlag(GestureAction.zoom))
				//{
				//	DoZoom(gestureResult.delta.Z * 0.75f);
				//	//	//cameraZoomLag = cameraZoom;
				//	//	//TakeFocus();

				//}
				//if(gestureResult.action.HasFlag(GestureAction.pan))
				//{
				//	//	TakeFocus();
				//	var dr = gestureResult.delta.ToV2();
				//	{
						
				//		dr *= 1.0f.ScreenToWorld();
				//		View.SetViewTargetInstant(View.viewW2 + dr);
				//	}
				//	//else
				//	{

				//		//		PostJSMouseEvent("mousemove",0, (int)dr.X,(int)dr.Y);
				//	}
				//}

			}
		}



}

