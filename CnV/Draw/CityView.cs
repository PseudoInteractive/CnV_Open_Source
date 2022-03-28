using CnV.Game;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CnV.City;
using static CnV.AGame;
using UWindows = Windows;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

using XVector2 = Microsoft.Xna.Framework.Vector2;
using XVector3 = Microsoft.Xna.Framework.Vector3;
using XVector4 = Microsoft.Xna.Framework.Vector4;
using Microsoft.Xna.Framework;
using CnV.Views;
using static CnV.Debug;
using static CnV.CityBuild;
using static CnV.GameClient;
using static CnV.CityViewS;
using static CnV.Building;
//using CityBuildAction = CnV.CityBuild.Action;
namespace CnV
{
	using Draw;
	using Game;
	using static View;
	using Microsoft.Xna.Framework.Graphics;

	
	public class CityView
	{

		internal static BuildC[] selectedBuildCs = Array.Empty<BuildC>();
		internal static double selectedBuildCsChangeTime;
		internal static BuildingId[] selectedBuildingIds = Array.Empty<BuildingId>();
		internal static double selectedBuildingIdsChangeTime;

		const float zBuildings = 0;
		const float zCityOverlay = 1.0f/128;
		internal const float buildingPlacementZ = 0;// (1.0f/64.0f);
		public static bool isDrawing;
		static CityView()
		{
			for (int y = span0; y <= span1; ++y)
				for (int x=span0;x<=span1;++x)
				{
					baseAnimationOffsets[City.XYToId((x,y))] =Math.Atan2(y, x)  +(((double)y/span0).Squared() + ((double)x / span0).Squared()).Sqrt()*MathF.PI;

				}
			AUtil.UnsafeCopy(CityView.baseAnimationOffsets, CityViewS.animationOffsets);

			City.buildCityChanged += ()
											=>
									{
										AUtil.UnsafeCopy(CityView.baseAnimationOffsets, CityViewS.animationOffsets);
										//buildQueue.ClearKeepBuffer();
										//buildQInSync = false;
										ClearSelectedBuilding();

									};
		}

		//const int atlasTileSize = 128;
		//const int atlasRows = 33;
		//const int atlasColumns = 4;
		//internal const int atlasIconCount = atlasRows*atlasColumns;
		//static public (int x, int y) BidToAtlas(BuildingId bid)
		//{
		//	var iconId = BuildingDef.idToAtlasOffset[bid];
		//	var y = iconId / atlasColumns;
		//	return (iconId - y*atlasColumns,y );
		//}
		
		//const float duDt = (1.0f / atlasColumns);
		//const float dvDt = (1.0f / atlasRows);
		static Vector2 buildCityOrigin;
		public const float cityYAspectRatio = 80.0f / 128.0f; // aspect ratio
		const float cityTileGainX = 1.0f / citySpan;
		const float cityTileGainY= cityYAspectRatio * cityTileGainX;
	//	public static Material buildingAtlas;
	//	public static Material buildingShadows;
		public static Material cityWallsLand;
		public static Material cityWallsWater;
		public static Material cityNoWallsLand;
		public static Material cityNoWallsWater;
		public static Material cityWater;
		// See CityViewS for more
		
		public static Vector2 half2 = new Vector2(0.5f, 0.5f);

		public static float zHover => 4.0f / 64.0f * Settings.parallax;

		public static BuildC selectedPoint = BuildC.Nan;
		public static BuildC hovered = BuildC.Nan;
		//	static Color textColor = new Color(0xf1, 0xd1, 0x1b, 0xff);
		public static Vector2 CityPointToWC(Vector2 c)
		{
			return new Vector2(c.X*cityTileGainX+ buildCityOrigin.X, c.Y * cityTileGainY + buildCityOrigin.Y);
		}
		public static Vector2 CityPointToWC(BuildC c, (float x, float y) delta)
		{
			return CityPointToWC(new(c.x+delta.x,c.y+delta.y) );
		}

		public static (Vector2 c0, Vector2 c1) CityPointToQuad(BuildC bc, float additionalXScale = 1.0f,float yScale=1)
		{
			var c = CityPointToWC(new(bc.x,bc.y) );
			// There is no aspect ratio scaling for Y
			var baseScale = (0.5f * cityTileGainX );
			var dc = new Vector2(baseScale* additionalXScale, baseScale* yScale);
			return (c - dc, c + dc);
		}
		internal static (Vector2 c0, Vector2 c1) CityPointToQuad(BuildC bc, BuildingMaterials bt,(float x0, float y0, float x1, float y1) lerpC )
		{
			if(lerpC == default)
				lerpC  = (0, 0, 1, 1);

			var c = CityPointToWC(bc,new(lerpC.x0,lerpC.y0) );
			// There is no aspect ratio scaling for Y
			var baseScale = (0.5f * cityTileGainX );
			var xScale = bt.main.xScale * (lerpC.x1-lerpC.x0);
			var yScale = bt.main.yScale * (lerpC.y1-lerpC.y0);
			
			var dc = new Vector2( baseScale*xScale, baseScale* yScale);
			return (c-dc, c+dc);
			

		}
		public static double[] baseAnimationOffsets = new double[citySpotCount];
		public static float animationRate = 0.25f;
		
	//	public Building GetBuilding( int id) => buildingsCache[id];
		//public static Building GetBuilding((int x, int y) xy) => buildingsCache[XYToId(xy)];
		//static Vector2 waterC =new Vector2( 1.0f - 768.0f* cityTileGainX / 128,
		//								1.0f - 704.0f*cityTileGainY / 128 );
		
		private static TextFormat textformatBuilding = new TextFormat(TextFormat.HorizontalAlignment.center, TextFormat.VerticalAlignment.center);
		public static int cityDrawAlpha;



		public static void Draw(float cityAlpha)
		{
			Assert(isDrawing == false);
			isDrawing = true;
		//	ToolTips.contToolTip = null;

			var build = City.GetBuild();
			if(build == null)
				return;

			var buildOp = currentBuildOp.unpack;
			
			//if(build.buildQueue.Any())
			//{
			//	var op = build.buildQueue.FirstOrDefault(); 
			//	if( !op.isMove && op.pa)
			//	{
			//		constructionSpot = op.bspot;
			//	}
			//}

			try
			{
				int iAlpha = (cityAlpha * 255f).RoundToInt();
				var postBuildings = build.postQueueBuildingsForRender;
				var buildings = build.buildings;
				buildCityOrigin = build.cid.CidToWorldV();
				// draw each building tile
				var city =build;
				cityDrawAlpha =  iAlpha;
				//const float zBase = 0f;
				// selected
				double simTime = IServerTime.NowToServerSeconds(); // includes fractional
				// Walls and background
				var cityWallSpan = new Vector2(0.5f*(23.0f/21.0f),0.5f*(23.0f/21.0f) * cityYAspectRatio);
				var cityWallOrigin = new Vector2(buildCityOrigin.X+(2.0f/64f)*cityTileGainX,buildCityOrigin.Y+0.375f*cityTileGainY );
				var cityWall0 = cityWallOrigin - cityWallSpan;
				var cityWall1 = cityWallOrigin + cityWallSpan;
				{
				
					
					{
						var hasWall = buildings[bspotWall].bl > 0;
						draw.AddQuad(iAlpha >= 255 ? Layer.tileCityBaseOpaque:
													Layer.tileCityBaseAlpha,
													city.isOnWater ? !hasWall ? cityNoWallsWater: cityWallsWater :!hasWall ? cityNoWallsLand: cityWallsLand,
													cityWall0,cityWall1,
													color:new(byte.MaxValue,byte.MaxValue,byte.MaxValue,(byte)iAlpha),depth: 0.0f/1024f);
					}
				}

				var cityFontScale = MathF.Sqrt(6.0f / viewW.Z.Clamp(0.5f,6.0f)) *Settings.buildingLabelScale* baseFontScale; // perspective attenuation with distance
				for(var cy = span0;cy <= span1;++cy)
				{
					for(var cx = span0;cx <= span1;++cx)
					{
						var bspot = new BuildC(cx, cy);

						Building cur, next;//,overlay;
						if(!CityBuild.isPlanner)
						{
							cur = buildings[bspot];
							next = postBuildings[bspot];
							//					overlay = city.BuildingFromOverlay(id);
						}
						else
						{
							cur = city.GetLayoutBuilding(bspot);
							next = cur;
							//				overlay = city.postQueueBuildings[id];
						}

						if(cur.id!=0 || next.id!=0)
						{
							// this is to show the demo symbol?
							//if(bspot == bspotWall)
							//{
							//	if(cur.bl == 0)
							//		continue;
							//}


							BuildingId bid;
							byte bl;

							var dt = (animationT - animationOffsets[bspot]);
							float dtF = (float)dt;
							//float blendT = (dt*(1/3.0)).Frac();
							// ZBase is on initial action placement
							var bumpOffset = 1f;

							if(dtF < 0.5f )
							{
								var prior = priorBuildings[bspot];
								if(prior == bidNone)
								{
									// drop in
								//	bumpOffset = dtF.SCurve(0f,1f);
								}
								else
								{
									if(next.id == bidNone)
									{
									}
									else
									{

										// bump
										var t0 = (2*dt).Wave().Lerp(1.0f,0.95f);
										bumpOffset = t0;
										Assert(t0 >=0.5f);
										Assert(t0 <=1.0f);
										//Log($"{dtF}:  {dtF.CatmullRom(-1f, 0f, 1, 0f,0f)}");
									}
								}
							}
							var alpha = cityAlpha*bumpOffset;
							var fontAlpha = alpha;


							// Is building changing
							if(cur.id==next.id || ((next.bl==cur.bl)&&(!cur.isRes)))
							{
								// No change
								bid = next.id;

								// upgrade of downgrade. same building
								if(next.bl != cur.bl)
								{
									float blendOp = 0;
									var blendT = (dt*(1.0f/3.0f)).Frac();
									var blendT4 = blendT*4;
									if(blendT < 0.25f)
									{
										var t = (blendT4);
										bl = cur.bl; // fade next number
										fontAlpha *= t.SCurve(0,1);
										blendOp = 0;//.SCurve(0,1);
									}
									else if(blendT < 0.5f)
									{
										var t = (blendT4 - 1);
										blendOp = 0;
										// fade out number
										bl = cur.bl; // fade next number
										fontAlpha*= t.SCurve(1,0);

									}
									else if(blendT < 0.75f) // fade out hammer
									{
										var t = (blendT4 -2); // fade in new number
										blendOp = t.SCurve(0,1);
													// fade in last number
										bl = next.bl;
										fontAlpha*= t.SCurve(0,1);
									}
									else
									{
										// fade in number
										var t = (blendT4 - 3); // fade in new number
																		 // fade out number
										blendOp =t.SCurve(1,0);
										bl = next.bl;
										fontAlpha *= t.SCurve(1,0);// prior number out	
									}
									////fontAlpha = (cityAlpha*fontA*255f).TruncateToInt();
									//if(blendOp > 0)
									//{
									//	var cs = CityPointToQuad(bspot,1.2f);
									//	// cross fade in new level that this is going to
									//	float z1 = zCityOverlay*bumpOffset;
									//	draw.AddQuad(Layer.cityBuildEffect,decalSelectGloss,cs.c0,cs.c1,new Color(iAlpha,iAlpha,iAlpha,iAlpha / 2).Scale(blendOp*0.5f),depth: z1);
									//}
								}
								else
								{
									bl = next.bl;
								}

							}
							else
							{
								// build or destroy

							//	float blendOp;
							//	Material blendMat;
								// destroy
								if(next.id == 0)
								{
									// Draw an X

									

									bid = cur.bid;
									
									alpha *= (dtF.Saturate().Bezier(1f,0.625f,0.625f));
									
									var cs = CityPointToQuad(bspot,1.2f);
									//float z1 = zCityOverlay*bumpOffset;
									if(bspot != buildOp.c)
									{
										bl = cur.bl;
										var blendMat = decalBuildingInvalid;
										float blendOp = (dt*(1.0f/3.0f)).Wave().Lerp(0.3125f,0.625f);

										draw.AddQuad(bspot.LayerEffect(),blendMat,cs.c0,cs.c1,(new Color(iAlpha,iAlpha,iAlpha,iAlpha)).Scale(blendOp),depth: 0);
									}
									else
									{
										bl=0;
									}
								}
								else
								{
									//blendMat = decalSelectBuilding;
									bid = next.bid;
									if(next.bl == 1)
										bl = 0;
									else
										bl = next.bl;
									alpha *= (dtF.Saturate().Bezier(0f,0.4375f,0.4375f));
								}

								// Don't draw the build overlay is this is a construction as the crane is drawing
								//if(next.id == 0 || bspot != constructionSpot)
								//{
								//	// either buildings from new or demoing
								//	if(blendOp > 0)
								//	{
								//		var cs = CityPointToQuad(bspot,1.2f);
								//		float z1 = (1-bumpOffset)*(1.0f/64.0f);
								//		draw.AddQuad(Layer.cityBuildEffect,blendMat,cs.c0.WorldToCamera(),cs.c1.WorldToCamera(),(new Color(iAlpha,iAlpha,iAlpha,iAlpha)).Scale(blendOp),depth: z1);
								//	}
							//	}
								

							}
							if(bspot == buildOp.c)
								{
									
									var buildEnd = city.buildItemEndsAt.EarliestSeconds; // should this be earliest?
									var startT = currentBuildStartTime;
									var required = buildEnd-startT;
										var gain = ((buildEnd - simTime)/required).SaturateToFloat();
								//	Assert(buildEnd >= simTime - 1.0f);
									Assert(buildEnd - simTime <= required + 1.0f);
									if(buildOp.isBuild|buildOp.isDemo)
									{
										if(buildOp.isBuild)
										{
											alpha = gain.Lerp(1,alpha);
										}
										else
										{
											alpha = gain.Lerp(0,alpha);

										}
										if(bspot == bspotWall)
										{
											var fade = 1-gain;

											draw.AddQuad(Layer.tileCityBaseFade,
												city.isOnWater ?buildOp.isBuild? cityWallsWater : cityNoWallsWater 
												: buildOp.isBuild ? cityWallsLand : cityNoWallsLand,
												cityWall0,cityWall1,new Color(byte.MaxValue,byte.MaxValue,byte.MaxValue,fade.UNormToByte()),depth: 0.0f/1024f);
				
										}

									//	alpha *= (gain);
									}
									if(!buildOp.isMove)
									{
										var dT = ((simTime -currentBuildStartTime)*2.0f).SaturateToFloat();
										var fade = (dT).UNormToByte();

										float v0, v1;
										if(buildOp.isDemo || buildOp.isDowngrade)
										{
											v0 = (1-gain); v1 = 1;
										}
										else
										{
											v0 = 0; v1 = gain;
										}	
										var cs = CityPointToQuad(bspot,1.2f);
										draw.AddQuad(bspot.LayerEffect(),decalSelectGloss,
											new(cs.c0.X, v0.Lerp(cs.c0.Y,cs.c1.Y)),
											new(cs.c1.X, v1.Lerp(cs.c0.Y,cs.c1.Y)),
											new(0f,v0),new(1.0f,v1),
											fade.AlphaToAll(), depth:zCityOverlay );
									}
								}

							DrawBuilding(bid,iAlpha: alpha.UNormToByte(),zBase: 0,layer: bspot.LayerBuilding(),buildC: bspot,fontScale: cityFontScale,fontAlpha: fontAlpha.UNormToByte(),buildingLevel: bl);
							if(selectedBuildingIds.Contains(bid))
							{
								//var t = ((animationT - selectedBuildingIdsChangeTime)*2).Saturate().Wave()*2.0f;
								CityView.DrawHoverMarker(bspot,new Color(255, 64, 0, 255));
							}
							// farm fields
							if(next.bid == bidFarm)
							{

								foreach(var delta in Octant.deltas)
								{
									var c1 = bspot + delta;
									if(!IsBuildingSpot(c1,city))
										continue;

									if(!(isPlanner ? city.GetLayoutBid(c1) == bidNone : (city.postQueueBuildings[c1].bid == bidNone)))
										continue; // spot is full
									DrawBuilding(bidFarmField,alpha.UNormToByte(),zBase: 0,layer: bspot.LayerBuilding(),buildC: c1);
								}
							}
						} // if any building is there

						// draw overlays
						if( (build.isLayoutCustom||isPlanner) && Settings.drawBuildingOverlays && build.IsBuildingSpotOrWater(bspot) )
						{
							BuildingId bid, currentBid;
							//	BuildingDef bd;
							//Building current;
							if(CityBuild.isPlanner)
							{
								var bs = postBuildings[bspot];
								if(bs.isEmpty)
								{
									continue;
								}

								bid = bs.bid;

								currentBid = build.GetLayoutBid(bspot);

							}
							else
							{
								currentBid = postBuildings[bspot].bid;
								bid=build.GetLayoutBid(bspot);
								if(IsBidRes(currentBid) && bid == 0) // don't need to be removed
								{
									continue;
								}
							}
							//							var currentBid = current.refId;

							if(currentBid == bid)
								continue;

							if(bid==0)
							{
								if(currentBid == Building.bidCabin)
									continue; // leave it, it is fine
							}


							//var iconId = BidToAtlas(bid);

							//var u0 = iconId.x * duDt;
							//var v0 = iconId.y * dvDt;
							//var cs = CityPointToQuad(cx,cy);
							//var _c0 = 0.125f.Lerp(cs.c0,cs.c1);
							//var _c1 = 0.625f.Lerp(cs.c0,cs.c1);
							var shadowOffset = new Vector2(5.0f,5.0f);
							var off = (animationT -animationOffsets[bspot]) * 0.3245f;
							var cScale = off.Wave().Lerp(0.875f,1.0f);
							// overlays
							DrawBuilding(bid,iAlpha,zBase: zCities*cScale,layer: Layer.cityLayout,buildC:bspot,lerpC:(0.25f,0.25f,0.75f,0.75f),wantShadow:true);
							//draw.AddQuad(Layer.tileCity + 3,buildingShadows,_c0+shadowOffset,_c1+shadowOffset,new Vector2(u0,v0),new Vector2(u0 + duDt,v0 + dvDt),new Color(0,0,0,iAlpha*7/8).Scale(cScale),zZero); // shader does the z transform

							//draw.AddQuad(Layer.tileCity+4,buildingAtlas,_c0,_c1,new Vector2(u0,v0),new Vector2(u0 + duDt,v0 + dvDt),iAlpha.AlphaToAll().Scale(cScale),zZero); // shader does the z transform

						}
					}
					

					
				}
				if(hovered.isInCityAndNotNan)
				{
					switch (CityBuild.action)
					{
						case CityBuild.CityBuildAction.moveStart:
							case CityBuild.CityBuildAction.moveEnd: 
					{
							CityView.DrawSprite(hovered,CityViewS.decalMoveBuilding);
								break;
					}
						default:
							CityView.DrawHoverMarker(hovered,new Color(48, 0, 128, 255));
							break;
					}
				}
				if(buildOp.isNotNop)
				{
					var spot = buildOp.c;
					var dT = ((simTime -currentBuildStartTime)*1.5f).SaturateToFloat().SCurve();
					var fade = ((dT*255).RoundToInt());

					DrawBuilding(bidConstruction,fade,zBase: zCities,layer: spot.LayerConstruction(),buildC: spot,lerpC:(-0.375f,0.5f,0.625f,1.5f) ); //  bspot,lerpC0: 0.25f,lerpC1: 0.75f,wantShadow: true);
				}
				if(lastBuiltOp.isNotNop )
				{
					var spot = lastBuiltOp.bspot;
					var dT = ((animationT -lastBuildCompleteTime)*0.5f).SaturateToFloat().SCurve();
					if(dT >= 1.0f)
					{
						// over
					}
					else
					{
						{
							var fade = (((1-dT)*255).RoundToInt());
							DrawBuilding(bidConstruction,fade,zBase: zCities,layer: spot.LayerConstruction(),buildC: spot,lerpC: (-0.375f, 0.5f, 0.625f, 1.5f)); //  bspot,lerpC0: 0.25f,lerpC1: 0.75f,wantShadow: true);
						}
					}
				}
				
				foreach(var selBc in selectedBuildCs)
				{
			//		var t = ((animationT - selectedBuildCsChangeTime)*2).Saturate().Wave()*2.0f;

					CityView.DrawHoverMarker(selBc,new Color(255,64,0,255));
				}
				if(quickBuildOverlay != 0)
				{
					CityView.DrawBuildingOverlay(lastToolTipSpot,CityView.cityDrawAlpha,quickBuildOverlay);
				}
				if(quickBuildOverlaySprite!= null)
				{
					CityView.DrawSprite(lastToolTipSpot,quickBuildOverlaySprite); 
				}
				if(quickBuildSelectedSprite!= null)
				{
					CityView.DrawSprite(selectedPoint,quickBuildSelectedSprite); 
				}
			//	PreviewBuildAction();
				
			}
			catch(Exception __ex)
			{
				LogEx(__ex);
				
			} 
			Assert(isDrawing);
			isDrawing = false;

		}


		private static void DrawBuilding(BuildingId bid,int iAlpha,float zBase,int layer,BuildC buildC,float fontScale = 0,int fontAlpha = -1,int buildingLevel = -1,(float x0,float y0, float x1, float y1) lerpC = default,bool wantShadow=false )
		{
			Assert(isDrawing);
			if( bid == 0)
				return;

			if( BuildingMaterials.all.TryGetValue(bid,out var materials) )
			{
				// building
				int iconId = 0;
				var altCount = materials.altCount;
				if(altCount > 0)
				{
					iconId = buildC.ToRandom(altCount+1);
				}
				var _cs = CityPointToQuad(buildC,materials,lerpC);
			//	var bdi = BuildingDef.FromId(bid);
				var m = materials.main;
				var frameCount = materials.frameCount;
				if(frameCount > 1)
				{
					var frames = frameCount;
				//	var du = 1.0/frames;

					var dt = ((animationOffsets[buildC]-animationT)/(materials.animationDuration)).Frac()*(frames);
					//				var duFX = (int)(255*255*du);
					int frame8 = (int)(dt*256);
					int frame = frame8 >> 8;
					var blend =  (frame8 - (frame<<8)).AsByte();
					var f0 = (frame*4).AsByte();
					var f1 = (frame+1) >= frames ? (byte)0 : ((frame+1)*4).AsByte();
					{
						var alpha = iAlpha.AsByte();
					draw.AddQuad(layer,materials.M(iconId).m,_cs.c0,_cs.c1,
						new Vector2(0,0),
						new Vector2(1,1),
						new(blend,f0,f1,alpha),
						

						depth: zBase);
				}
					if(wantShadow)
					{
						var alpha = iAlpha.ScaleAndRound(0.75f).AsByte();
					draw.AddQuad(layer-1,materials.M(iconId).shadow,_cs.c0,_cs.c1,
						new Vector2(0,0),
						new Vector2(1,1),
						new(blend,f0,f1,alpha),
						depth:0f); 

					}
				}
				else
				{

					draw.AddQuad(layer,materials.M(iconId).m,_cs.c0,_cs.c1,new Vector2(0,0),new Vector2(1,1),new(byte.MaxValue,byte.MaxValue,byte.MaxValue,iAlpha.AsByte()),depth:zBase); // shader does the z transform
					if(wantShadow)
					{
						// this should be the overlay
						draw.AddQuad(layer-1,materials.M(iconId).shadow,_cs.c0,_cs.c1,new Vector2(0,0),new Vector2(1,1),new((byte)0,(byte)0,(byte)0,iAlpha.ScaleAndRound(0.75f).AsByte()),depth:0f); // shader does the z transform

					}
				}
			}
			// building level
			if(buildingLevel > 0) 
			{
					if(fontAlpha == -1)
						fontAlpha = iAlpha;
				var cs = CityPointToQuad(buildC);
					DrawTextBox(buildingLevel.ToString(), 0.825f.Lerp(cs.c0, cs.c1), textformatBuilding,
						color:new Color(0xf1* fontAlpha/256, 0xd1* fontAlpha/256, 0x1b* fontAlpha/256, fontAlpha),
						backgroundAlpha: (byte)iAlpha, layer:Layer.tileCityNumbers, scale: fontScale, zBias: zBase*0.5f );
			}
			
		}

		
		public static void DrawBuildingOverlay(BuildC cc, int iAlpha, BuildingId bid)
		{
			var off = (AGame.animationT - animationOffsets[cc]) * 0.333f;
			var cScale = off.Wave().Lerp(0.75f, 0.875f);//, off.WaveC().Lerp(0.8f, 1.0f));

			DrawBuilding(bid: bid, iAlpha: iAlpha.ScaleAndRound(cScale), zBase: zBuildings, layer: cc.LayerBuilding(),buildC: cc);


			//var iconId = BidToAtlas(bid);
			//var u0 = iconId.x * duDt;
			//var v0 = iconId.y * dvDt;

			//draw.AddQuad(Layer.tileCity, buildingAtlas, cs.c0, cs.c1, new Vector2(u0, v0), new Vector2(u0 + duDt, v0 + dvDt), iAlpha.AlphaToAll().Scale(cScale), (zBase, zBase, zBase, zBase)); // shader does the z transform
			//																																													//if(overlay!=null)
			//																																													//	draw.AddQuad(Layer.tileCity + 2, overlay, cs.c0, cs.c1, new Color(iAlpha, iAlpha, iAlpha, iAlpha / 2).Scale(cScale), (zHover+zBias, zHover+zBias, zHover+zBias, zHover+zBias) );
		}
		public static void DrawSprite(BuildC cc, Material mat, float animFreq = (1f/3f) )
		{
			Assert(isDrawing);

			var off = (animationT-animationOffsets[cc]) * animFreq;
			var cScale = new Vector2(off.Wave().Lerp(0.8f, 1.0f), off.WaveC().Lerp(0.8f, 1.0f));
			var cs = CityPointToQuad(cc, 1.2f);
			draw.AddQuad(Layer.cityActionIndicator, mat, cs.c0, cs.c1, new Color( cityDrawAlpha, cityDrawAlpha, cityDrawAlpha, cityDrawAlpha / 2).Scale(cScale),depth:zCityOverlay);
		}
		public static void DrawHoverMarker(BuildC cc,Color color)
		{
			Assert(isDrawing);
			var cs = CityPointToQuad(cc,yScale:0.875f);
			DrawRectOutlineShadow(Layer.effects,cs.c0,cs.c1,color,animationOffset:animationOffsets[cc]);
		}
		

		public static void LoadContent()
		{
			LoadTheme();
			decalBuildingInvalid       = LoadMaterialAdditive("Art/City/decal_building_invalid");
			decalBuildBuilding         = LoadMaterialAdditive("Art/City/decal_building_valid");
			decalBuildBuildingMulti    = LoadMaterialAdditive("Art/City/decal_building_valid_multi");
			decalMoveBuilding          = LoadMaterialAdditive("Art/City/decal_move_building");
			decalSelectBuilding        = LoadMaterialAdditive("Art/City/decal_select_building");
			decalSelectGloss           = LoadMaterialAdditive("Art/City/build_details_gloss_overlay");
		}

		internal  struct MaterialShadowPair
		{
			internal Material? m;
		    internal Material? shadow;
			internal float xScale;
			internal float yScale ;

			internal MaterialShadowPair(Material? m,Material? shadow)
			{
				this.m=m;
				this.shadow=shadow;
				this.xScale = m.texture2d.width/128.0f;
				this.yScale = m.texture2d.height/128.0f;
			

			}
		}
		internal class BuildingMaterials
		{
			internal const int maxAltCount = 4;
			internal MaterialShadowPair main;
			// alternates
			internal MaterialShadowPair alt1;
			internal MaterialShadowPair alt2;
			internal MaterialShadowPair alt3;
			internal MaterialShadowPair alt4;

			internal MaterialShadowPair destroyed;

			internal MaterialShadowPair overlay;

			internal float animationDuration;
			internal byte frameCount;

			internal const int MFirst = -2;
			internal const int MLast = maxAltCount;

			internal int altCount => alt1.m is null ? 0 :
									alt2.m is null ? 1 :
									alt3.m is null ? 2 :
				alt4.m is null ? 3 :
				4;
			//internal Material shadow;
			internal static Dictionary<BuildingId,BuildingMaterials> all = new();

			public BuildingMaterials(MaterialShadowPair main,float animationDuration)
			{
				this.main=main;
				this.animationDuration =animationDuration;
				this.frameCount = (byte)main.m.GetFrameCount();
			}

			internal ref MaterialShadowPair M(int id)
			{
				switch(id)
				{
					case 0: return ref main;
					case 1: return ref alt1;
					case 2: return ref alt2;
					case 3: return ref alt3;
					case 4: return ref alt4;
					case -1: return ref destroyed;
					case -2: return ref overlay;
					default:
						Assert(false);
						return ref main;
				}
			}
		}
		public static void LoadTheme()
		{
			//var cityBase = Settings.IsThemeWinter() ? "Art/City/Winter/" : "Art/City/";
			//var cityBase =  "Art/City/";
			//buildingAtlas = Material.LoadLitMaterial(cityBase + "building_set5");
			//buildingShadows = new Material(buildingAtlas.texture, AGame.unlitEffect);
			if(!GameClient.TryLoadLitMaterialFromDDS($"runtime\\city\\CityWallsLL",out cityWallsLand,out _,wantShadow: false,unlit: false,city:false,opaque:true))
			{
				Assert(false);
			}
			///	cityWallsLand = LoadLitMaterial(cityBase + "baseland");
			if(!GameClient.TryLoadLitMaterialFromDDS($"runtime\\city\\CityWallsWater",out cityWallsWater,out _,wantShadow: false,unlit: false,city:false,opaque:true))
			{
				Assert(false);
			}
			if(!GameClient.TryLoadLitMaterialFromDDS($"runtime\\city\\CityNoWallsLL",out cityNoWallsLand,out _,wantShadow: false,unlit: false,city:false,opaque:true))
			{
				Assert(false);
			}
			///	cityWallsLand = LoadLitMaterial(cityBase + "baseland");
			if(!GameClient.TryLoadLitMaterialFromDDS($"runtime\\city\\CityNoWallsWater",out cityNoWallsWater,out _,wantShadow: false,unlit: false,city:false,opaque:true))
			{
				Assert(false);
			}
			foreach(var build in BuildingDef.all)
			{
				if(build==null  || build.dimg == null || build.dimg.Length ==0 )
					continue;
				try
				{
					
					if(GameClient.TryLoadLitMaterialFromDDS($"runtime\\city\\{build.dimg}", out var main, out var shadow,wantShadow:true,unlit:build.isUnlit, city:true ))
					{
						var frames = main.GetFrameCount();
						var bm = new BuildingMaterials( new( main,shadow) ,build.animationDuration);
						
						BuildingMaterials.all[build.id] = bm;
							for(int i = BuildingMaterials.MFirst;i<=BuildingMaterials.maxAltCount;++i)
							{
								if(i==0)
									continue;
								var str = i switch {
									-2 => build.overlay,
									-1 => build.destroyImg, 
									1 => build.dimg1, 2 => build.dimg2, 
									3 => build.dimg3, 4 => build.dimg4 };
								if(str ==null)
									continue;
								if(GameClient.TryLoadLitMaterialFromDDS($"runtime\\city\\{str}", out var main1, out var shadow1,wantShadow:true,unlit:build.isUnlit, city:true ))
								{
									bm.M(i) = new(main1,shadow1);
								}
							}

						}
				}
				catch( Exception e )
				{
					Log($"InvalidTexture {build} {build.dimg}, {e.Message}");
				}

			}
		}
		//public static void UpdateLighting()
		//{
		////	if(buildingAtlas.effect is not null)
		//	// reset effects to lit or unlit
		////		buildingAtlas.effect = AGame.GetTileEffect();
		//	if(cityWallsLand.effect is not null)
		//		cityWallsLand.effect = AGame.GetTileEffect();
		//	if(cityWallsWater.effect is not null)
		//		cityWallsWater.effect = AGame.GetTileEffect();
		//}

		public static void ClearSelectedBuilding()
		{
			SetSelectedBuilding(BuildC.Nan, isSingleClickAction);
		}

		public static void SetSelectedBuilding( BuildC xy, bool _isSingleClickAction)
		{
			if (xy.isNotNan)
			{
				if (action == CityBuildAction.moveEnd)
				{ 
					// stay here
				}
				else if (action == CityBuildAction.moveStart)
				{
					if (_isSingleClickAction)
					{

						CityBuild.PushSingleAction(CityBuildAction.moveEnd);

					}
					else
					{
						SetAction(CityBuildAction.moveEnd);
					}
				}
			}
			else
			{
				if (action == CityBuildAction.moveEnd)
				{
					if (_isSingleClickAction)
					{
						RevertToLastAction();
					}
					else
					{
						SetAction(CityBuildAction.moveStart);
					}
				}
			}
			selectedPoint = xy;
		}

	}

}
