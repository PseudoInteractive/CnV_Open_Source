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
		const float cityTileGainX = 1.0f / citySpan;
		const float cityTileGainY= yScale * cityTileGainX;
		public const float yScale = 96.0f / 128.0f; // aspect ratio
		public static Material buildingAtlas;
		public static Material buildingShadows;
		public static Material cityWallsLand;
		public static Material cityWallsWater;
		public static Material cityWater;
		// See CityViewS for more
		
		public static Material decalSelectEmpty;
		public static Vector2 half2 = new Vector2(0.5f, 0.5f);

		public static float zHover => 4.0f / 64.0f * Settings.parallax;

		public static BuildC selectedPoint = BuildC.Nan;
		public static BuildC hovered = BuildC.Nan;
	//	static Color textColor = new Color(0xf1, 0xd1, 0x1b, 0xff);
		public static Vector2 CityPointToCC( float x, float y)
		{
			return new Vector2(x*cityTileGainX+ buildCityOrigin.X, y * cityTileGainY + buildCityOrigin.Y).WorldToCamera();
		}
		public static (Vector2 c0, Vector2 c1) CityPointToQuad(float x, float y, float additionalXScale = 1.0f,float yScale=1)
		{
			var c = CityPointToCC(x,y);
			// There is no aspect ratio scaling for Y
			var baseScale = (0.5f * cityTileGainX * GameClient.pixelScale);
			var dc = new Vector2(baseScale* additionalXScale, baseScale* yScale);
			return (c - dc, c + dc);
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
				var city = City.GetBuild();
				cityDrawAlpha = iAlpha;
				var zBase = 0f;
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

						// this is to show the demo symbol?
						var bidOverride = bidNone;
						if(bspot == bspotWall)
						{
							if(cur.bl == 0)
								continue;
							bidOverride = bidWall;
						}

						var cs = CityPointToQuad(cx,cy);

						var dt = (animationT - animationOffsets[bspot]);
						float blendT = ((dt)*0.333f).Frac();
						var bonus = (dt*0.5f).Abs().Saturate().BSpline(1,0,0);

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
									bl = next.bl; // fade next number
									fontA = t.BSpline(0,1,1);
									blendOp = t.SCurve();
								}
								else if(blendT < 0.5f)
								{
									var t = (blendT - 0.25f) * 4.0f;
									blendOp = 1;
									// fade out number
									bl = next.bl; // fade next number
									fontA = t.BSpline(1,1,0);

								}
								else if(blendT < 0.75f) // fade out hammer
								{
									var t = (blendT - 0.5f) * 4.0f; // fade in new number
									blendOp = t.SCurve(1,0);
									// fade in last number
									bl = cur.bl;
									fontA = t.BSpline(0,1,1);
								}
								else
								{
									// fade in number
									var t = (blendT - 0.75f) * 4.0f; // fade in new number
																	 // fade out number
									bl = cur.bl;
									fontA = t.BSpline(1,1,0);// prior number out	
								}
								var z = bonus*buildingPlacementZ;
								DrawBuilding(iAlpha,z*0.5f,fontScale,cs,next,Layer.tileCity,(int)(alpha*fontA*255f),bl,bidOverride);
								if(blendOp > 0)
								{
									// upgrade
									
									draw.AddQuad(Layer.tileCity + 2,(next.bl > cur.bl) ? decalBuildingValid : decalSelectEmpty,cs.c0,cs.c1,new Color(iAlpha,iAlpha,iAlpha,iAlpha / 2).Scale(blendOp),(z,z,z,z) );
								}
							}
							else
							{
								// not changing
								DrawBuilding(iAlpha,zBase,fontScale,cs,cur,Layer.tileCity,-1,-1,bidOverride);

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
								blendMat = decalBuildingValid;
								bd = next;
							}
							if(blendT < 0.25f)
							{
								var t = blendT * 4.0f; // demo fades in, half second
								blendOp = t.BSpline(0,1,1);
							}
							else
							{
								var t = (blendT - 0.25f) *(1.0f/0.75f); // building fades in, hammer fades out 1 seconds
								blendOp = t.BSpline(1,1,0);
							}
							var z = bonus*(0.5f/64.0f);

							if(blendOp > 0)
							{
								draw.AddQuad(Layer.tileCity + 2,blendMat,cs.c0,cs.c1,(new Color(iAlpha,iAlpha,iAlpha,iAlpha)).Scale(blendOp),(z,z,z,z) );
							}

							DrawBuilding(iAlpha,z*0.5f,fontScale,cs,bd,Layer.tileCity,-1,-1,bidOverride);
						}

						// draw overlays
						if(build.isLayoutCustom && Settings.drawBuildingOverlays && build.IsBuildingSpotOrWater(bspot) )
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
								if(BuildingDef.IsBidRes(currentBid) && bid == 0) // don't need to be removed
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


							var iconId = BidToAtlas(bid);

							var u0 = iconId.x * duDt;
							var v0 = iconId.y * dvDt;
							//var cs = CityPointToQuad(cx,cy);
							var _c0 = 0.125f.Lerp(cs.c0,cs.c1);
							var _c1 = 0.625f.Lerp(cs.c0,cs.c1);
							var shadowOffset = new Vector2(5.0f,5.0f);
							var off = (AMath.BSpotToRandom((cx, cy)) + animationT * 0.3245f);
							var cScale = new Vector2(off.Wave().Lerp(0.8f,1.0f),off.WaveC().Lerp(0.8f,1.0f));

							draw.AddQuad(Layer.tileCity + 3,buildingShadows,_c0+shadowOffset,_c1+shadowOffset,new Vector2(u0,v0),new Vector2(u0 + duDt,v0 + dvDt),new Color(0,0,0,iAlpha*7/8).Scale(cScale),zZero); // shader does the z transform

							draw.AddQuad(Layer.tileCity+4,buildingAtlas,_c0,_c1,new Vector2(u0,v0),new Vector2(u0 + duDt,v0 + dvDt),iAlpha.AlphaToAll().Scale(cScale),zZero); // shader does the z transform

						}
					}
					var citySpan = new Vector2(0.5f,0.5f * yScale);
					var city0 = buildCityOrigin - citySpan;
					var city1 = buildCityOrigin + citySpan;
					draw.AddQuad(Layer.tileCity - 2,city.isOnWater ? cityWallsWater : cityWallsLand,city0.WorldToCamera(),city1.WorldToCamera(),iAlpha.AlphaToAll(),(0f, 0f, 0f, 0f));

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

		private static void DrawBuilding(int iAlpha, float zBase, float fontScale, (Vector2 c0,Vector2 c1) cs,in Building bid,int layer,int fontAlpha=-1, int blOverride=-1, BuildingId bidOverride = 0)
		{
			Assert(isDrawing);
			if (bid.id != 0)
			{
				var bd = BuildingDef.FromId(bid.id);

				var iconId = BidToAtlas(bidOverride==bidNone? bid.id : bidOverride);

				var u0 = iconId.x * duDt;
				var v0 = iconId.y * dvDt;
			

				draw.AddQuad(layer, buildingAtlas, cs.c0, cs.c1, new Vector2(u0, v0), new Vector2(u0 + duDt, v0 + dvDt), iAlpha.AlphaToAll(), (zBase, zBase, zBase, zBase)); // shader does the z transform
				if (fontAlpha == -1)
					fontAlpha = iAlpha;
				if (blOverride == -1)
					blOverride = bid.bl;
				if (blOverride != 0)
					DrawTextBox(blOverride.ToString(), 0.825f.Lerp(cs.c0, cs.c1), textformatBuilding, 
						new Color(0xf1* fontAlpha/256, 0xd1* fontAlpha/256, 0x1b* fontAlpha/256, fontAlpha),
						(byte)iAlpha, ((int)layer+16), scale: fontScale, zBias: 0);
				
			}
		}
		static (float,float,float,float) zZero = (0,0,0,0);
		public static void DrawSprite( (int x, int y) cc, Material mat, float animFreq)
		{
			Assert(isDrawing);

			var off = (animationT * animFreq);
			var cScale = new Vector2(off.Wave().Lerp(0.8f, 1.0f), off.WaveC().Lerp(0.8f, 1.0f));
			var cs = CityPointToQuad(cc.x, cc.y, 1.2f);
			draw.AddQuad(Layer.tileCity + 2, mat, cs.c0, cs.c1, new Color( cityDrawAlpha, cityDrawAlpha, cityDrawAlpha, cityDrawAlpha / 2).Scale(cScale),zZero);
		}

		public static void DrawBuilding((int x, int y) cc,int iAlpha, BuildingId bid, float randomValue, float zBias=0)
		{
			const float zBase = 0.0f;
			var iconId = BidToAtlas(bid);
			var u0 = iconId.x * duDt;
			var v0 = iconId.y * dvDt;
			var cs = CityPointToQuad(cc.x, cc.y);

			var off = randomValue;
			var cScale = new Vector2(off.Wave().Lerp(0.8f, 1.0f), off.WaveC().Lerp(0.8f, 1.0f));
			draw.AddQuad(Layer.tileCity, buildingAtlas, cs.c0, cs.c1, new Vector2(u0, v0), new Vector2(u0 + duDt, v0 + dvDt), iAlpha.AlphaToAll().Scale(cScale),(zBias, zBias, zBias, zBias) ); // shader does the z transform
			//if(overlay!=null)
			//	draw.AddQuad(Layer.tileCity + 2, overlay, cs.c0, cs.c1, new Color(iAlpha, iAlpha, iAlpha, iAlpha / 2).Scale(cScale), (zHover+zBias, zHover+zBias, zHover+zBias, zHover+zBias) );
		}

		public static void LoadContent()
		{
			LoadTheme();
			decalBuildingInvalid       = LoadMaterialAdditive("Art/City/decal_building_invalid");
			decalBuildingValid         = LoadMaterialAdditive("Art/City/decal_building_valid");
			decalBuildingValidMulti    = LoadMaterialAdditive("Art/City/decal_building_valid_multi");
			decalMoveBuilding          = LoadMaterialAdditive("Art/City/decal_move_building");
			decalSelectBuilding        = LoadMaterialAdditive("Art/City/decal_select_building");
			decalSelectEmpty           = LoadMaterialAdditive("Art/City/build_details_gloss_overlay");
			CityViewS.ClientDrawSprite = DrawSprite;
		}
		public static void LoadTheme()
		{
			var cityBase = Settings.IsThemeWinter() ? "Art/City/Winter/" : "Art/City/";
			buildingAtlas = Material.LoadLitMaterial(cityBase + "building_set5");
			buildingShadows = new Material(buildingAtlas.texture, AGame.unlitEffect);
			cityWallsLand = Material.LoadLitMaterial(cityBase + "baseland");
			cityWallsWater = Material.LoadLitMaterial(cityBase + "basewater");
		}
		public static void UpdateLighting()
		{
			if(buildingAtlas.effect is not null)
			// reset effects to lit or unlit
				buildingAtlas.effect = AGame.GetTileEffect();
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
