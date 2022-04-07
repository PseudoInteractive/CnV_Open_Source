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

using Cysharp.Text;

using Draw;

using EnumsNET;

using Game;

	using Helpers;

	using Microsoft.UI.Xaml.Input;

	partial class ShellPage
	{
		private static void Canvas_PointerMoved((Windows.Foundation.Point Position, uint PointerId,
																			bool IsInContact, ulong Timestamp, PointerUpdateKind PointerUpdateKind) point)
		{

			//			if(!mouseOverCanvas)
			//				Log("Mouse Moved Canvas");
			//	App.cursorDefault.Set();
			// prevent idle timer;
			mouseOverCanvas = true;
			//	instance.mouseOverCanvasBox.IsChecked = mouseOverCanvas;


			//	PointerInfo(e);
			UpdateMousePosition(point.Position);
			//	TakeFocusIfAppropriate();
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
							Player.viewHover = PlayerId.MaxValue;
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

							Spot.viewHover = default;
							Player.viewHover = PlayerId.MaxValue;
							ref string toolTip = ref ToolTips.spotToolTip;
						toolTip = null;

							lastCanvasC = cid;
							var packedId = World.GetWorldId(c);
							var data = World.GetTile(World.rawPrior1!=null ? World.rawPrior1 : World.tileData,packedId);

							switch(data.type)
							{
								case World.TileType.typeCity:
									{
										Spot.viewHover = (WorldC)cid;
										var city = City.GetOrAddCity(cid);
										
										if(city != null)
										{
											if(data.player == 0)
											{
												toolTip = $"Lawless\n{c.y / 100}{c.x / 100} ({c.x}:{c.y})\nPoints {city.points}";
											}
											else
											{
												Player.viewHover = data.player;
												toolTip = city.toolTip;
												}
										}
										break;
									}
								case World.TileType.typeShrine:
									if(WorldViewSettings.shrines.isOn)
										toolTip = $"Shrine\n{(data.data == 255 ? "Unlit" : ((Faith)data.data-1).AsString())}";
									break;
								case World.TileType.typePortal:
									toolTip = $"Portal\n{(data.data == 0 ? "Inactive" : "Active")}";
									break;
							default:
								if(data.isDungeon)
								{
									var cavern = Cavern.Get(cid);
									toolTip = cavern.ToString(); // \ntype:{data >> 4}";
								}
								break;
							}

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
														var player = Player.GetValueOrDefault(pData.player,Player._default);
														toolTip += $"\nWas abandoned by:\n{player.name}\n{player.allianceName}";
													}
													else
													{
														var player = Player.GetValueOrDefault(pData.player,Player._default);
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
										case World.TileType.typePortal:
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

