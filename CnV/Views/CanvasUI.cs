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
							toolTip = null;
							CityView.hovered = cc;
						}
					}
					else
					{

						if(cont != lastCont)
						{
							lastCont = cont;
							if(!IsCityView())
							{
								ref var cn = ref Continent.all[cont];
								contToolTip = $"{World.UnpackContinent(cont)}\nSettled {cn.settled}\nFree {cn.unsettled}\nCities {cn.cities}\nCastles {cn.castles}\nTemples {cn.temples}\nDungeons {cn.dungeons}";

							}
						}


						if(lastCanvasC != cid)
						{

							Spot.viewHover = default;
							Player.viewHover = PlayerId.MaxValue;
							toolTip = null;



							lastCanvasC = cid;
							var packedId = World.GetWorldId(c);
							var data = World.GetInfoFromWorldId(World.rawPrior1!=null ? World.rawPrior1 : World.tileData,packedId);
							switch(data.type)
							{
								case World.TileType.city:
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

												var player = Player.GetValueOrDefault(data.player,Player._default);
												//	if (Player.IsFriend(data.player))
												{
													//if (spot is City city)
													{
														using var sb = ZString.CreateUtf8StringBuilder();
														//	var notes = city.remarks.IsNullOrEmpty() ? "" : city.remarks.Substring(0, city.remarks.Length.Min(40)) + "\n";
														sb.AppendLine(player.name);
														sb.AppendLine(city.cityName);
														sb.AppendFormat("pts:{0:N0}\n",city.points);
														if(player.allianceId!= 0)
															sb.AppendLine(Alliance.IdToName(player.allianceId));
														if(Player.IsSubOrMe(data.player))
															sb.AppendLine(city.GetTroopsString("\n"));

														if(city.senatorInfo.Length != 0)
														{
															sb.AppendLine(city.GetSenatorInfo());
														}
														if(city.incoming.Any())
														{

															var incAttacks = 0;
															var incTs = 0u;
															foreach(var i in city.incoming)
															{
																if(i.isAttack)
																{
																	++incAttacks;
																	incTs += i.ts;
																	if(incAttacks<=3)
																	{
																		sb.AppendLine(i.troopInfo); // only show first two
																	}
																	else if(incAttacks ==4)
																	{
																		sb.AppendLine("..");
																	}
																}
															}
															sb.AppendFormat("{0} incoming attacks",incAttacks);
															if(incTs > 0)
																sb.AppendFormat(" ({0} total TS)\n",incTs);
															else
																sb.Append('\n');

															if(city.claim != 0)
																sb.AppendFormat("{0}% Claim\n",city.claim);

															sb.AppendFormat("{0} total def\n",city.tsDefMax);

															sb.AppendLine(city.GetDefString("\n"));

														}
														if(city.outGoingStatus!=0)
														{
															if(city.outGoingStatus.HasFlag(City.OutgoingStatus.sieging))
																sb.Append("Sieging\n");
															else if(city.outGoingStatus.HasFlag(City.OutgoingStatus.scheduled))
																sb.Append("Attack Scheduled\n");
															else
																sb.Append("Attack Sent\n");
														}
														else if(city.reinforcementsIn.AnyNullable())
														{
															sb.AppendFormat("{0} def\n",city.tsDefMax);
															int counter = 0;
															foreach(var i in city.reinforcementsIn)
															{
																sb.AppendLine(i.troops.Format($"From {City.GetOrAddCity(i.sourceCid).nameAndRemarks}:",'\n','\n'));
																if(++counter >= 4)
																{
																	sb.AppendLine("...");
																	break;
																}
															}

														}
														if(!city.remarks.IsNullOrEmpty())
															sb.AppendLine(city.remarks.AsSpan().Wrap(20));
														if(city.hasAcademy.GetValueOrDefault())
															sb.AppendLine("Has Academy");
														if(NearRes.instance.isFocused)
														{
															sb.AppendLine($"Carts:{AUtil.FormaRatio((city.cartsHome, city.carts))}");
															if(city.ships > 0)
																sb.AppendLine($"Ships:{AUtil.FormatRatio(city.shipsHome,city.ships)}");
															sb.AppendLine($"Wood:{city.resources[0].Format()}, Stone:{ city.resources[1].DivideRound(1000):4,N0}k");
															sb.AppendLine($"Iron:{city.resources[2].Format()}, Food:{ city.resources[3].FormatWithSign()}k");
														}
														sb.Append($"{c.y / 100}{c.x / 100} ({c.x}:{c.y})");

														toolTip = sb.ToString();

													}

												}
												//else
												//{
												//	var info = spot != null ?
												//		$"{spot.cityName}\n{spot.points}\n"
												//	 : "";
												//	toolTip = $"{player.name}\n{Alliance.IdToName(player.allianceId)}\n{info}{c.y / 100}{c.x / 100} ({c.x}:{c.y})\ncities:{player.cities.Count}\npts:{player.pointsH * 100}";
												//}
											}
										}
										break;
									}
								case World.TileType.shrine:
									if(WorldViewSettings.shrines.isOn)
										toolTip = $"Shrine\n{(data.data == 255 ? "Unlit" : ((Faith)data.data-1).AsString())}";
									break;
								case World.TileType.boss:
									if(WorldViewSettings.bosses.isOn)
										toolTip = $"Boss\nLevel:{data.data}"; // \ntype:{data >> 4}";
									break;
								case World.TileType.dungeon:
									if(WorldViewSettings.caverns.isOn)
										toolTip = $"Dungeon\nLevel:{data.data}"; // \ntype:{data >> 4}";
									break;
								case World.TileType.portal:
									toolTip = $"Portal\n{(data.data == 0 ? "Inactive" : "Active")}";
									break;
							}

							if(World.rawPrior0 != null)
							{
								var pData = World.GetInfoFromWorldId(World.rawPrior0,packedId);
								if(pData.all == data.all)
								{
									// no change

								}
								else
								{
									switch(data.type)
									{
										case World.TileType.city:
											if(pData.type == World.TileType.city)
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
										case World.TileType.shrine:
											toolTip += "\nWas unlit";
											break;
										case World.TileType.portal:
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
							{
								StringBuilder sb = new(toolTip);
								var info = TileData.instance.GetSpotinfo(c.x,c.y,sb);
								sb.Append($"\nOnWater:{data.isOnWater}\nShoreline:{info.shoreline}\nOcean:{info.isOcean}");
								Assert(data.type == info.type);

								toolTip = sb.ToString();
							}
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

