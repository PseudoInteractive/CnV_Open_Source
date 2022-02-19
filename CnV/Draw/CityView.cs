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
using static CnV.CanvasHelpers;
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

	using Microsoft.Xna.Framework.Graphics;

	using Views;
	
	public class CityView
	{

		internal const float buildingPlacementZ = (1.0f/64.0f);
		public static bool isDrawing;
		static CityView()
		{
			for (int y = span0; y <= span1; ++y)
				for (int x=span0;x<=span1;++x)
				{
					baseAnimationOffsets[City.XYToId((x,y))] =MathF.Atan2(y, x) / MathF.PI + (((float)y/span0).Squared() + ((float)x / span0).Squared()).Sqrt();

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

		const int atlasTileSize = 128;
		const int atlasRows = 33;
		const int atlasColumns = 4;
		internal const int atlasIconCount = atlasRows*atlasColumns;
		static public (int x, int y) BidToAtlas(BuildingId bid)
		{
			var iconId = BuildingDef.idToAtlasOffset[bid];
			var y = iconId / atlasColumns;
			return (iconId - y*atlasColumns,y );
		}
		
		const float duDt = (1.0f / atlasColumns);
		const float dvDt = (1.0f / atlasRows);
		static Vector2 buildCityOrigin;
		public const float cityYScale = 96.0f / 128.0f; // aspect ratio
		const float cityTileGainX = 1.0f / citySpan;
		const float cityTileGainY= cityYScale * cityTileGainX;
	//	public static Material buildingAtlas;
	//	public static Material buildingShadows;
		public static Material cityWallsLand;
		public static Material cityWallsWater;
		public static Material cityWater;
		// See CityViewS for more
		
		public static Vector2 half2 = new Vector2(0.5f, 0.5f);

		public static float zHover => 4.0f / 64.0f * Settings.parallax;

		public static BuildC selectedPoint = BuildC.Nan;
		public static BuildC hovered = BuildC.Nan;
	//	static Color textColor = new Color(0xf1, 0xd1, 0x1b, 0xff);
		public static Vector2 CityPointToWC( BuildC c)
		{
			return new Vector2(c.x*cityTileGainX+ buildCityOrigin.X, c.y * cityTileGainY + buildCityOrigin.Y);
		}
		public static (Vector2 c0, Vector2 c1) CityPointToQuad(BuildC bc, float additionalXScale = 1.0f,float yScale=1)
		{
			var c = CityPointToWC(bc);
			// There is no aspect ratio scaling for Y
			var baseScale = (0.5f * cityTileGainX );
			var dc = new Vector2(baseScale* additionalXScale, baseScale* yScale);
			return (c - dc, c + dc);
		}
		internal static (Vector2 c0, Vector2 c1) CityPointToQuad(BuildC bc, BuildingMaterials bt, float lerpC0=0,float lerpC1=1)
		{
			var c = CityPointToWC(bc);
			// There is no aspect ratio scaling for Y
			var baseScale = (0.5f * cityTileGainX );
			var xScale = bt.xScale;
			var yScale = bt.yScale;
			var dc = new Vector2(baseScale* xScale, baseScale* yScale);
			var c0 = c-dc;
			var c1 = c+dc;
			return (lerpC0.Lerp(c0,c1),lerpC1.Lerp(c0,c1) );
		}
		public static float[] baseAnimationOffsets = new float[citySpotCount];
		public static float animationRate = 0.25f;
		
	//	public Building GetBuilding( int id) => buildingsCache[id];
		//public static Building GetBuilding((int x, int y) xy) => buildingsCache[XYToId(xy)];
		//static Vector2 waterC =new Vector2( 1.0f - 768.0f* cityTileGainX / 128,
		//								1.0f - 704.0f*cityTileGainY / 128 );
		
		private static TextFormat textformatBuilding = new TextFormat(TextFormat.HorizontalAlignment.center, TextFormat.VerticalAlignment.center);
		public static int cityDrawAlpha;



		public static void Draw(float alpha)
		{
			Assert(isDrawing == false);
			isDrawing = true;
			ToolTips.contToolTip = null;

			var build = City.GetBuild();
			if(build == null)
				return;

			try
			{
				int iAlpha = (int)(alpha * 255f);
				var postBuildings = build.postQueueBuildingsForRender;
				var buildings = build.buildings;
				buildCityOrigin = build.cid.CidToWorldV();
				// draw each building tile
				var city =build;
				cityDrawAlpha = iAlpha;
				//const float zBase = 0f;
				// selected

				// Timeline:
				// 0..1: B0
				// 1..2: Fade Out B0, fade in Op
				// 2..3: Fade Out Op, fade in B1
				// 3..4: B1
				// 4..5: Fade out B1 fade in B0
				var fontScale = (pixelScale / 64.0f) * (2.5f/64.0f) * Settings.fontScale; // perspective attenuation with distance
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
							if(bspot == bspotWall)
							{
								if(cur.bl == 0)
									continue;
							}



							var dt = (float)(animationT - animationOffsets[bspot]);
							float dtF = dt*3f;
							float blendT = (dt*0.25f).PingPong();
							// ZBase is on initial action placement
							var zBase = 0f;
							if(dtF < 1)
							{
								var prior = priorBuildings[bspot];
								if(prior == bidNone)
								{
									// drop in
									zBase = dtF.SCurve(1f,0f)*buildingPlacementZ;
								}
								else
								{
									if(next.id == bidNone)
									{
										// fire out
										zBase = dtF.Bezier(0f,0.0f,1.0f)*buildingPlacementZ;
									}
									else
									{

										// bump
										zBase = dtF.CatmullRom(-1f,0f,1,0.0f,0f)*buildingPlacementZ;
										//Log($"{dtF}:  {dtF.CatmullRom(-1f, 0f, 1, 0f,0f)}");
									}
								}
							}

							if(cur.id==next.id || ((next.bl==cur.bl)&&(!cur.isRes)))
							{
								if(next.bl != cur.bl)
								{
									float blendOp = 0;
									byte bl;
									float fontA;
									if(blendT < 0.25f)
									{
										var t = (blendT) * 4.0f;
										bl = cur.bl; // fade next number
										fontA = 1.0f;//t.SCurve(0,1);
										blendOp = t.SCurve(0,1);
									}
									else if(blendT < 0.5f)
									{
										var t = (blendT - 0.25f) * 4.0f;
										blendOp = 1;
										// fade out number
										bl = cur.bl; // fade next number
										fontA = t.SCurve(1,0);

									}
									else if(blendT < 0.75f) // fade out hammer
									{
										var t = (blendT - 0.5f) * 4.0f; // fade in new number
										blendOp = 1;// t.SCurve(1,0);
													// fade in last number
										bl = next.bl;
										fontA = t.SCurve(0,1);
									}
									else
									{
										// fade in number
										var t = (blendT - 0.75f) * 4.0f; // fade in new number
																		 // fade out number
										blendOp =t.SCurve(1,0);
										bl = next.bl;
										fontA = 1.0f;//t.SCurve(1,0);// prior number out	
									}
									DrawBuilding(next.id,iAlpha,zBase,Layer.tileCity,bspot,fontScale,(int)(alpha*fontA*255f),bl);
									if(blendOp > 0)
									{
										var cs = CityPointToQuad(bspot);
										// cross fade in new level that this is going to
										float z1 = zBase*1.5f;
										draw.AddQuad(Layer.tileCity + 2,decalSelectGloss,cs.c0.WorldToCamera(),cs.c1.WorldToCamera(),new Color(iAlpha,iAlpha,iAlpha,iAlpha / 2).Scale(blendOp),(z1, z1, z1, z1));
									}
								}
								else
								{
									// not changing
									DrawBuilding(cur.id,iAlpha,zBase,layer: Layer.tileCity,buildC: bspot,fontScale: fontScale,fontAlpha: -1,buildingLevel: cur.bl);

								}

							}
							else
							{
								float blendOp;
								Material blendMat;
								Building bd;
								if(next.id == 0)
								{
									blendMat = decalBuildingInvalid;
									bd = cur;
								}
								else
								{
									blendMat = decalBuildBuilding;
									bd = next;
								}
								if(blendT < 0.5f)
								{
									var t = blendT * 2.0f; // demo fades in, half second
									blendOp = t.SCurve(0,1);
								}
								else
								{
									var t = (blendT - 0.5f) *(2.0f); // building fades in, hammer fades out 1 seconds
									blendOp = t.SCurve(1,0);
								}

								// either buildings from new or demoing
								if(blendOp > 0)
								{
									var cs = CityPointToQuad(bspot);
									float z1 = zBase*1.5f;
									draw.AddQuad(Layer.tileCity + 2,blendMat,cs.c0.WorldToCamera(),cs.c1.WorldToCamera(),(new Color(iAlpha,iAlpha,iAlpha,iAlpha)).Scale(blendOp),(z1, z1, z1, z1));
								}

								DrawBuilding(bd.id,iAlpha,zBase: zBase,layer: Layer.tileCity,buildC: bspot,fontScale: fontScale,fontAlpha: -1,buildingLevel: bd.bl);
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
									DrawBuilding(bidFarmField,255,zBase: zBase,layer: Layer.tileCity,buildC: c1);
								}
							}
						}
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
							DrawBuilding(bid,iAlpha,zBase: zCities*cScale,layer: Layer.tileCity+4,buildC:bspot,lerpC0:0.25f,lerpC1:0.75f,wantShadow:true);
							//draw.AddQuad(Layer.tileCity + 3,buildingShadows,_c0+shadowOffset,_c1+shadowOffset,new Vector2(u0,v0),new Vector2(u0 + duDt,v0 + dvDt),new Color(0,0,0,iAlpha*7/8).Scale(cScale),zZero); // shader does the z transform

							//draw.AddQuad(Layer.tileCity+4,buildingAtlas,_c0,_c1,new Vector2(u0,v0),new Vector2(u0 + duDt,v0 + dvDt),iAlpha.AlphaToAll().Scale(cScale),zZero); // shader does the z transform

						}
					}
					// Walls and background
					var citySpan = new Vector2(0.5f,0.5f * cityYScale);
					var city0 = buildCityOrigin - citySpan;
					var city1 = buildCityOrigin + citySpan;
					draw.AddQuad(Layer.tileCityBase,city.isOnWater ? cityWallsWater : cityWallsLand,city0,city1,iAlpha.AlphaToAll(),(0f, 0f, 0f, 0f));

					// draw overlays

					//if(isPlanner && !build.isLayoutValid)
					//{
					//	Status("Please set a layout.",dryRun);
					//}


					// if selected is 

					//var biSelected = postQueuebuildingsCache[XYToId(selected)];
					//var bdSelected = biSelected.def;

					//if (selected.IsValid())
					//{
					//	Material mat = CityBuild.action switch
					//	{
					//		Action.build => null,
					//		Action.destroy => null,
					//		Action.move => decalMoveBuilding,
					//		_ => decalSelectBuilding
					//	};

					//	if (mat != null)
					//	{
					//		var off = (animationT * 0.3256f);
					//		var cScale = new Vector2(off.Wave().Lerp(0.8f, 1.0f), off.WaveC().Lerp(0.8f, 1.0f));
					//		var cs = CityPointToQuad(selected.x, selected.y, 1.2f);
					//		draw.AddQuad(Layer.tileCity + 2, mat, cs.c0, cs.c1, new Color(iAlpha, iAlpha, iAlpha, iAlpha / 2).Scale(cScale), PlanetDepth, zHover);

					//	}
					//}

					// hovered
					//if (hovered.IsValid())
					//{
					//	var biHovered = postQueuebuildingsCache[XYToId(hovered)]; ;
					//	var bdHovered = biHovered.def;

					//	Material mat = CityBuild.action switch
					//	{
					//		Action.layout => decalBuildingValidMulti,
					//		Action.build => (bdHovered.id == 0) ? decalBuildingValid : decalSelectEmpty,
					//		Action.destroy => (bdHovered.id == 0) ? decalSelectEmpty : decalBuildingInvalid,
					//		Action.move => selected.IsValid() ?
					//			(bdHovered.id == 0 ? decalMoveBuilding : biHovered.isBuilding?decalMoveBuilding: decalBuildingInvalid)
					//			: (bdHovered.id == 0 ? decalSelectEmpty : decalMoveBuilding),
					//		// create
					//		_ => (bdHovered.id == 0) ? decalSelectEmpty : decalSelectEmpty
					//	};

					//	if (mat != null)
					//	{
					//		DrawSprite(hovered, mat, 0.312f);

					//	}
					//	// draw the quickbuild placeholder building
					//	if (CityBuild.action == Action.build)
					//	{
					//		//if (bdHovered.id == 0)
					//		//{

					//		//	var sel = CityBuild.quickBuildId;
					//		//	if (sel != 0)
					//		//	{
					//		//		DrawBuilding(hovered, iAlpha, sel, animationT * 0.3247f);
					//		//		ShellPage.contToolTip = $"Click to place { BuildingDef.FromId(sel).Bn }";
					//		//	}
					//		//}
					//		//else
					//		//{
					//		//	ShellPage.contToolTip = $"Spot is occupied";
					//		//}
					//	}
					//	else if (CityBuild.action == Action.move)
					//	{

					//		if (selected.IsValid())
					//		{
					//			if (Player.isAvatarOrTest ? IsTowerSpot(hovered):!IsBuildingSpot(hovered))
					//			{
					//				ShellPage.contToolTip = $"Please do not build buildings on walls";
					//			}
					//			else
					//			{
					//				if (bdHovered.id == 0)
					//				{
					//					ShellPage.contToolTip = $"Click to Move {GetBuilding(selected).name} to ({hovered.x},{hovered.y})";
					//					DrawBuilding(hovered, iAlpha, GetBuilding(selected).def.bid, animationT * 0.3249f);


					//				}
					//				else
					//				{
					//					if (bdHovered.isRes)
					//						ShellPage.contToolTip = $"Target is occupied by {bdHovered.Bn}, click to demo";
					//					else
					//					{
					//						ShellPage.contToolTip = $"Swap {bdHovered.Bn} with {GetBuilding(selected).name}, (takes 3 moves)";
					//						DrawBuilding(hovered, iAlpha, GetBuilding(selected).def.bid, animationT * 0.3249f);
					//						// draw a move icond too
					//					}
					//				}
					//			}

					//		}
					//		else
					//		{
					//			if (biHovered.isBuilding)
					//			{
					//				ShellPage.contToolTip = $"Click to move {bdHovered.Bn}";
					//			}
					//			else
					//			{
					//				ShellPage.contToolTip = $"Please Select a building to move";
					//			}
					//		}
					//	}


					//}
				}
				if(hovered.isInCity)
				{
					switch (CityBuild.action)
					{
						case CityBuild.CityBuildAction.moveStart:
							case CityBuild.CityBuildAction.moveEnd: 
					{
							CityView.DrawSprite(hovered,CityViewS.decalMoveBuilding ,0.312f);
								break;
					}
						default:
							CityView.DrawHoverMarker(hovered);
							break;
					}
				}

				PreviewBuildAction();
					//var processed = new HashSet<int>();
					//foreach (var r in IterateQueue() )
					//{
					//	int bspot = r.bspot;
					//	if (!processed.Add(bspot))
					//		continue;
					//	var cc = IdToXY(bspot);
					//	var cs = CityPointToQuad(cc.x, cc.y, 1.2f);
					//	var off = (bspot.BSpotToRandom() + animationT*0.325f);
					//	var cScale = new Vector2(off.Wave().Lerp(0.3f,1.0f), off.WaveC().Lerp(0.3f, 1.0f));
					//	if (r.elvl == 0)
					//	{
					//		// demo
					//		draw.AddQuad(Layer.tileCity + 2, decalBuildingInvalid, cs.c0, cs.c1, (new Color(iAlpha, iAlpha, iAlpha, iAlpha / 2)).Scale(cScale), PlanetDepth, zHover);
					//	}
					//	else if (r.slvl == 0)
					//	{
					//		// new building
					//		DrawBuilding(cc, iAlpha, r.bid, off);

					//		// now the overlay
					//		draw.AddQuad(Layer.tileCity + 2, decalBuildingValidMulti, cs.c0, cs.c1, new Color(iAlpha, iAlpha , iAlpha , iAlpha/2).Scale(cScale), PlanetDepth, zHover);
					//	}
					//	else if (r.slvl < r.elvl)
					//	{
					//		// upgrade
					//		draw.AddQuad(Layer.tileCity + 2, decalBuildingValid, cs.c0, cs.c1, new Color(iAlpha, iAlpha , iAlpha , iAlpha / 2).Scale(cScale), PlanetDepth, zHover);
					//	}
					//	else
					//	{
					//		// downgrade or other, just highlight it
					//		draw.AddQuad(Layer.tileCity +2, decalSelectEmpty, cs.c0, cs.c1, new Color(iAlpha, iAlpha, iAlpha, iAlpha / 2).Scale(cScale), PlanetDepth, zHover);

					//	}
					//}
				
			}
			catch(Exception __ex)
			{
				LogEx(__ex);
				
			} 
			Assert(isDrawing);
			isDrawing = false;

		}

		private static void DrawBuilding(BuildingId bid,int iAlpha,float zBase,int layer,BuildC buildC,float fontScale = 0,int fontAlpha = -1,int buildingLevel = -1,float lerpC0 = 0,float lerpC1 = 1,bool wantShadow=false )
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
				var _cs = CityPointToQuad(buildC,materials,lerpC0,lerpC1);
				var bdi = BuildingDef.FromId(bid);
				if(materials.frameCount > 1)
				{
					var frames = materials.frameCount;
					var du = 1.0/frames;
					
					var dt = ((animationT-animationOffsets[buildC])/(bdi.animationDuration)).Frac()*(frames);
	//				var duFX = (int)(255*255*du);
					int frame8 = (int)(dt*256);
					int frame = frame8 >> 8;
					int blend = frame8 - (frame<<8);
				
					
					draw.AddQuad(layer,materials.M(iconId).m,_cs.c0,_cs.c1,
						new Vector2( (float)(du*frame),0),
						new Vector2( (float)(du*(frame+1)),1),
						new(blend.AsByte(),materials.frameDeltaG,materials.frameDeltaB, iAlpha.AsByte() ),(zBase, zBase, zBase, zBase)); 
					if(wantShadow)
					{
					draw.AddQuad(layer-1,materials.M(iconId).shadow,_cs.c0,_cs.c1,
						new Vector2( (float)(du*frame),0),
						new Vector2( (float)(du*(frame+1)),1),
						new(blend.AsByte(),materials.frameDeltaG,materials.frameDeltaB, iAlpha.ScaleAndRound(0.75f).AsByte() ),(0, 0, 0, 0)); 

					}
				}
				else
				{

					draw.AddQuad(layer,materials.M(iconId).m,_cs.c0,_cs.c1,new Vector2(0,0),new Vector2(1,1),new((byte)255,(byte)255,(byte)255,iAlpha.AsByte()),(zBase, zBase, zBase, zBase)); // shader does the z transform
					if(wantShadow)
					{
						draw.AddQuad(layer-1,materials.M(iconId).shadow,_cs.c0,_cs.c1,new Vector2(0,0),new Vector2(1,1),new((byte)0,(byte)0,(byte)0,iAlpha.ScaleAndRound(0.75f).AsByte()),(0, 0, 0, 0)); // shader does the z transform

					}
				}
			}
			// building level
			if(buildingLevel > 0) 
			{
					if(fontAlpha == -1)
						fontAlpha = iAlpha;
				var cs = CityPointToQuad(buildC);
					DrawTextBox(buildingLevel.ToString(), 0.825f.Lerp(cs.c0, cs.c1).WorldToCamera(), textformatBuilding,
						color:new Color(0xf1* fontAlpha/256, 0xd1* fontAlpha/256, 0x1b* fontAlpha/256, fontAlpha),
						backgroundAlpha: (byte)iAlpha, layer:((int)layer+16), scale: fontScale, zBias: zBase*0.5f );
			}
			
		}

		
		public static void DrawBuildingOverlay(BuildC cc, int iAlpha, BuildingId bid,  float zBase)
		{
			var off = (AGame.animationT - animationOffsets[cc]) * 0.333f;
			var cScale = off.Wave().Lerp(0.8f, 1.0f);//, off.WaveC().Lerp(0.8f, 1.0f));

			DrawBuilding(bid: bid, iAlpha: iAlpha.ScaleAndRound(cScale), zBase: zBase, layer: Layer.tileCity-1,buildC: cc);


			//var iconId = BidToAtlas(bid);
			//var u0 = iconId.x * duDt;
			//var v0 = iconId.y * dvDt;

			//draw.AddQuad(Layer.tileCity, buildingAtlas, cs.c0, cs.c1, new Vector2(u0, v0), new Vector2(u0 + duDt, v0 + dvDt), iAlpha.AlphaToAll().Scale(cScale), (zBase, zBase, zBase, zBase)); // shader does the z transform
			//																																													//if(overlay!=null)
			//																																													//	draw.AddQuad(Layer.tileCity + 2, overlay, cs.c0, cs.c1, new Color(iAlpha, iAlpha, iAlpha, iAlpha / 2).Scale(cScale), (zHover+zBias, zHover+zBias, zHover+zBias, zHover+zBias) );
		}
		static (float,float,float,float) zZero = (0,0,0,0);
		public static void DrawSprite(BuildC cc, Material mat, float animFreq)
		{
			Assert(isDrawing);

			var off = (animationT-animationOffsets[cc]) * animFreq;
			var cScale = new Vector2(off.Wave().Lerp(0.8f, 1.0f), off.WaveC().Lerp(0.8f, 1.0f));
			var cs = CityPointToQuad(cc, 1.2f);
			draw.AddQuad(Layer.tileCity + 2, mat, cs.c0.WorldToCamera(), cs.c1.WorldToCamera(), new Color( cityDrawAlpha, cityDrawAlpha, cityDrawAlpha, cityDrawAlpha / 2).Scale(cScale),zZero);
		}
		public static void DrawHoverMarker(BuildC cc)
		{
			Assert(isDrawing);
			var cs = CityPointToQuad(cc);
			DrawRectOutlineShadow(Layer.effects,cs.c0.WorldToCamera(),cs.c1.WorldToCamera(),new Color(32, 0, 96, 220), 3.0f);
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

		internal readonly record  struct MaterialShadowPair(Material? m,Material? shadow);
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

			internal float xScale=1;
			internal float yScale = 1;
			internal float animationDuration;
			internal byte frameDeltaG; // only if animatied
			internal byte frameDeltaB;
			internal byte frameCount=1;

			internal int altCount => alt1.m is null ? 0 :
									alt2.m is null ? 1 :
									alt3.m is null ? 2 :
				alt4.m is null ? 3 :
				4;
			//internal Material shadow;
			internal static Dictionary<BuildingId,BuildingMaterials> all = new();
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
					default:
						Assert(false);
						return ref main;
				}
			}
		}
		public static void LoadTheme()
		{
			//var cityBase = Settings.IsThemeWinter() ? "Art/City/Winter/" : "Art/City/";
			var cityBase =  "Art/City/";
			//buildingAtlas = Material.LoadLitMaterial(cityBase + "building_set5");
			//buildingShadows = new Material(buildingAtlas.texture, AGame.unlitEffect);
			cityWallsLand = Material.LoadLitMaterial(cityBase + "baseland");
			cityWallsWater = Material.LoadLitMaterial(cityBase + "basewater");
			foreach(var build in BuildingDef.all)
			{
				if(build==null  || build.dimg == null || build.dimg.Length ==0 )
					continue;
				try
				{ 
					if(GameClient.TryLoadLitMaterialFromDDS($"runtime\\city\\{build.dimg}", out var main, out var shadow,wantShadow:true,animationFrames:build.animationFrames ))
					{ 
						var bm = new BuildingMaterials() { main = new( main,shadow), 
							xScale = main.texture2d.width/128.0f/build.animationFrames.Max(1),
							yScale = main.texture2d.height/128.0f};
						if(build.animationFrames > 1 )
						{
							bm.frameCount = build.animationFrames;
							bm.animationDuration = build.animationDuration;
							(bm.frameDeltaG, bm.frameDeltaB) = SpriteAnim.ComputeFrameDeltaAsColours(build.animationFrames);
						}
						BuildingMaterials.all[build.id] = bm;
							for(int i = -1;i<=BuildingMaterials.maxAltCount;++i)
							{
								if(i==0)
									continue;
								var str = i switch { -1 => build.destroyImg, 
									1 => build.dimg1, 2 => build.dimg2, 
									3 => build.dimg3, 4 => build.dimg4, _ => null };
								if(str ==null)
									continue;
								if(GameClient.TryLoadLitMaterialFromDDS($"runtime\\city\\{str}", out var main1, out var shadow1,wantShadow:true,animationFrames:build.animationFrames ))
								{
									if(i== -1)
										bm.destroyed = new(main1,shadow1);
									else
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
		public static void UpdateLighting()
		{
		//	if(buildingAtlas.effect is not null)
			// reset effects to lit or unlit
		//		buildingAtlas.effect = AGame.GetTileEffect();
			if(cityWallsLand.effect is not null)
				cityWallsLand.effect = AGame.GetTileEffect();
			if(cityWallsWater.effect is not null)
				cityWallsWater.effect = AGame.GetTileEffect();
		}

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
