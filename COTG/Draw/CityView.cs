using COTG.Game;
using COTG.JSON;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static COTG.Game.City;
using static COTG.AGame;
using UWindows = Windows;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

using XVector2 = Microsoft.Xna.Framework.Vector2;
using XVector3 = Microsoft.Xna.Framework.Vector3;
using XVector4 = Microsoft.Xna.Framework.Vector4;
using static COTG.CanvasHelpers;
using Microsoft.Xna.Framework;
using COTG.Views;
using static COTG.Debug;
using static COTG.Views.CityBuild;
namespace COTG.Draw
{
	
	public class CityView
	{
	
		const int atlasTileSize = 128;
		const int atlasColumns = 4;
		static public (int x, int y) BidToAtlas(int bid)
		{
			var iconId = bid - 443;
			var y = iconId / atlasColumns;
			return (iconId - y*atlasColumns,y );
		}
		const int atlasRows = 33;
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
		public static Material decalBuildingInvalid;
		public static Material decalBuildingValid;
		public static Material decalBuildingValidMulti;
		public static Material decalMoveBuilding;
		public static Material decalSelectBuilding;
		public static Material decalSelectEmpty;
		public static Vector2 half2 = new Vector2(0.5f, 0.5f);

		public static float zHover => 1.0f / 64.0f * Views.SettingsPage.parallax;

		public static (int x, int y) selected = invalidXY;
		public static (int x, int y) hovered = invalidXY;
	//	static Color textColor = new Color(0xf1, 0xd1, 0x1b, 0xff);
		public static Vector2 CityPointToCC( float x, float y)
		{
			return new Vector2(x*cityTileGainX+ buildCityOrigin.X, y * cityTileGainY + buildCityOrigin.Y).WToC();
		}
		public static (Vector2 c0, Vector2 c1) CityPointToQuad(float x, float y, float additionalXScale = 1.0f,float yScale=1)
		{
			var c = CityPointToCC(x,y);
			// There is no aspect ratio scaling for Y
			var baseScale = (0.5f * cityTileGainX * AGame.pixelScale);
			var dc = new Vector2(baseScale* additionalXScale, baseScale* yScale);
			return (c - dc, c + dc);
	
		}
		public static JSON.Building[] buildingsCache;
		public static JSON.Building[] postQueuebuildingsCache = new JSON.Building[citySpotCount];
		static float[] animationOffsets = new float[citySpotCount];
		public static bool postQueueBuildingsDirty = true;
		public static void BuildingsOrQueueChanged() => postQueueBuildingsDirty = true;

		public static Building GetBuilding( int id) => buildingsCache[id];
		public static Building GetBuilding((int x, int y) xy) => buildingsCache[XYToId(xy)];
		static Vector2 waterC =new Vector2( 1.0f - 768.0f* cityTileGainX / 128,
										1.0f - 704.0f*cityTileGainY / 128 );
		
		private static TextFormat textformatBuilding = new TextFormat(TextFormat.HorizontalAlignment.center, TextFormat.VerticalAlignment.center);
		public static int cityDrawAlpha;

		public static void Status(string s, bool dryRun)
		{
			if (dryRun)
			{
				Status(s);
			}
			else
			{
				Note.Show(s);
			}
		}

		// Dry Run can be accessed on conditional and directly
		public static void Status(string s)
		{
				if (ShellPage.contToolTip != null)
					ShellPage.contToolTip = $"{ShellPage.contToolTip}\n{s}";
				else
					ShellPage.contToolTip = s;
		}


		public static void Draw(float alpha)
		{
			ShellPage.contToolTip = null;

			var build = City.GetBuild();
			if (build == null)
				return;
			int iAlpha = (int)(alpha * 255f);
			buildingsCache = build.buildings;
			var postBuildings = CityBuild.postQueueBuildings;
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

			var fontScale = (pixelScale / 64.0f) * (2.5f/64.0f) * SettingsPage.fontScale; // perspective attenuation with distance
			for (var cy = span0; cy <= span1; ++cy)
			{
				for (var cx = span0; cx <= span1; ++cx)
				{
					var id = XYToId((cx, cy));
					var bid = buildingsCache[id];
					var next = postBuildings[id];
					var cs = CityPointToQuad(cx, cy);
					byte iAlpha0 = 0;
					byte iAlpha1 = 0;
					float blendw0 = 0, blendw1 = 0, blendOp = 0;
					if (bid.id==next.id)
					{
						if (next.bl != bid.bl)
						{
							float blendT = ((animationT - animationOffsets[id]) * 2.0f) % 2f;
							if (blendT < 1.0f)
							{
								var t = blendT;
								blendOp = t.SCurve();
							}
							else
							{
								var t = blendT - 1;

								blendOp = t.SCurve(1f, 0f);
							}
							DrawBuilding(iAlpha, zBase, fontScale, cs, bid, Layer.tileCity,iAlpha0,next.bl,iAlpha1);
							if (next.bl > bid.bl)
							{
								// upgrade
								draw.AddQuad(Layer.tileCity + 2, decalBuildingValid, cs.c0, cs.c1, new Color(iAlpha, iAlpha, iAlpha, iAlpha / 2).Scale(blendOp), PlanetDepth, zHover);
							}
							else if (next.bl < bid.bl)
							{
								// downgrade or other, just highlight it
								draw.AddQuad(Layer.tileCity + 2, decalSelectEmpty, cs.c0, cs.c1, new Color(iAlpha, iAlpha, iAlpha, iAlpha / 2).Scale(blendOp), PlanetDepth, zHover);

							}
						}
						else
						{
							DrawBuilding(iAlpha, zBase, fontScale, cs, bid, Layer.tileCity);

						}

					}
					else
					{
						
						if (next.id == 0)
						{
							 iAlpha0 = (byte)(blendw0 * alpha * 255.0f);
							 iAlpha1 = (byte)(blendw1 * alpha * 255.0f);
							float blendT = ((animationT - animationOffsets[id]) * 2.0f) % 4f;
							if (blendT < 1.0f)
							{
								blendw0 = 1;
							}
							else if (blendT < 2.0f)
							{
								var t = blendT - 1.0f;
								blendw0 = t.SCurve(1f, 0f);
								blendOp = t.SCurve();
							}
							else if (blendT < 3.0f)
							{
								var t = blendT - 2.0f;
								blendOp = t.SCurve(1f, 0f);
							}
							else
							{
								var t = blendT - 3.0f;
								blendw0 = t.SCurve();
							}


						}
						else if (bid.id == 0)
						{
							float blendT = ((animationT - animationOffsets[id]) * 2.0f) % 4f;
							if (blendT < 1.0f)
							{
								var t = blendT;
								blendOp = t.SCurve();
							}
							else if (blendT <2.0f)
							{
								var t = blendT - 1.0f;
								blendOp = t.SCurve(1f, 0f);
								blendw1 = t.SCurve();
							}
							else if (blendT < 3.0f)
							{
								blendw1 = 1;
							}
							else
							{
								var t = blendT - 3.0f;
								blendw1 = t.SCurve(1f, 0f);
							}


						}
						else
						{
							float blendT = ((animationT - animationOffsets[id]) * 2.0f) % 5f;
							if (blendT < 1.0f)
							{
								blendw0 = 1;
							}
							else if (blendT < 2.0f)
							{
								var t = blendT - 1.0f;
								blendw0 = t.SCurve(1f, 0f);
								blendOp = t.SCurve();
							}
							else if (blendT < 3.0f)
							{
								var t = blendT - 2.0f;
								blendOp = t.SCurve(1f, 0f);
								blendw1 = t.SCurve();
							}
							else if (blendT < 4.0f)
							{
								blendw1 = 1;
							}
							else
							{
								var t = blendT - 4.0f;
								blendw1 = t.SCurve(1f, 0f);
								blendw0 = t.SCurve();
							}
						}

						 iAlpha0 = (byte)(blendw0 * alpha * 255.0f);
						 iAlpha1 = (byte)(blendw1 * alpha * 255.0f);



						if (next.bl == 0)
						{
							// demo
							draw.AddQuad(Layer.tileCity + 2, decalBuildingInvalid, cs.c0,cs.c1, (new Color(iAlpha, iAlpha, iAlpha, iAlpha / 2)).Scale(blendOp), PlanetDepth, zHover);
						}
						else if (bid.bl == 0)
						{

							// now the overlay
							draw.AddQuad(Layer.tileCity + 2, decalBuildingValidMulti,cs.c0 , cs.c1, new Color(iAlpha, iAlpha, iAlpha, iAlpha / 2).Scale(blendOp), PlanetDepth, zHover);
						}
						
						DrawBuilding(iAlpha0, zBase, fontScale, cs, bid, Layer.tileCity);
						DrawBuilding(iAlpha1, zBase, fontScale, cs, next, Layer.tileCity+1);
					}
				}
			}
			var citySpan = new Vector2(0.5f, 0.5f * yScale);
			var city0 = buildCityOrigin - citySpan;
			var city1 = buildCityOrigin + citySpan;
			draw.AddQuad(Layer.tileCity - 2, city.isOnWater ? cityWallsWater : cityWallsLand, city0.WToC(), city1.WToC(), iAlpha.AlphaToAll(),  (0f,0f,0f,0f) );

			if(build.layout!=null)
			{
				try
				{
					for (var cy = span0; cy <= span1; ++cy)
					{
						for (var cx = span0; cx <= span1; ++cx)
						{
							var bspot = City.XYToId((cx, cy));
							if (!CityBuild.IsBuildingSpot(bspot) || (cx==0&&cy==0) )
								continue; // wall

							(var bid,var bd) = build.BFromOverlay((cx, cy));
							if (bd.isRes)
								bid = 0;
							var current =  buildingsCache[bspot];
							var currentBid = current.isBuilding ? current.def.bid : 0;
							if (currentBid == bid)
								continue;

							if(bid==0)
							{
								bid = 443 + 8 * 4; // X mark
							}
							if (bid == 0)
								continue;

							var iconId = BidToAtlas(bid);

							var u0 = iconId.x * duDt;
							var v0 = iconId.y * dvDt;
							var cs = CityPointToQuad(cx, cy);
							var _c0 = 0.25f.Lerp(cs.c0, cs.c1);
							var _c1 = 0.75f.Lerp(cs.c0, cs.c1);
							var shadowOffset = new Vector2(5.0f, 5.0f);
							var off = (bspot.BSpotToRandom() + animationT * 0.3245f);
							var cScale = new Vector2(off.Wave().Lerp(0.8f, 1.0f), off.WaveC().Lerp(0.8f, 1.0f));

							draw.AddQuad(Layer.tileCity + 1, buildingShadows, _c0+shadowOffset,_c1+shadowOffset, new Vector2(u0, v0), new Vector2(u0 + duDt, v0 + dvDt), new Color(0,0,0,iAlpha*7/8).Scale(cScale), (zBase, zBase, zBase, zBase)); // shader does the z transform

							draw.AddQuad(Layer.tileCity+2, buildingAtlas, _c0, _c1, new Vector2(u0, v0), new Vector2(u0 + duDt, v0 + dvDt), iAlpha.AlphaToAll().Scale(cScale), (zBase, zBase, zBase, zBase)); // shader does the z transform
							
						}
					}
				}
				catch (Exception exception)
				{
					Log(exception);
				}

			}


			// if selected is 
			
			var biSelected = GetBuilding(selected);
			var bdSelected = biSelected.def;

			if (selected.IsValid())
			{
				Material mat = CityBuild.action switch
				{
					CityBuild.Action.build => null,
					CityBuild.Action.destroy => null,
					CityBuild.Action.move => decalMoveBuilding,
					_ => decalSelectBuilding
				};

				if (mat != null)
				{
					var off = (animationT * 0.3256f);
					var cScale = new Vector2(off.Wave().Lerp(0.8f, 1.0f), off.WaveC().Lerp(0.8f, 1.0f));
					var cs = CityPointToQuad(selected.x, selected.y, 1.2f);
					draw.AddQuad(Layer.tileCity + 2, mat, cs.c0, cs.c1, new Color(iAlpha, iAlpha, iAlpha, iAlpha / 2).Scale(cScale), PlanetDepth, zHover);

				}
			}

			// hovered
			if (hovered.IsValid())
			{
				var biHovered = GetBuilding(hovered);
				var bdHovered = biHovered.def;

				Material mat = CityBuild.action switch
				{
					CityBuild.Action.layout => decalBuildingValidMulti,
					CityBuild.Action.build => (bdHovered.id == 0) ? decalBuildingValid : decalSelectEmpty,
					CityBuild.Action.destroy => (bdHovered.id == 0) ? decalSelectEmpty : decalBuildingInvalid,
					CityBuild.Action.move => selected.IsValid() ?
						(bdHovered.id == 0 ? decalMoveBuilding : biHovered.isBuilding?decalMoveBuilding: decalBuildingInvalid)
						: (bdHovered.id == 0 ? decalSelectEmpty : decalMoveBuilding),
					// create
					_ => (bdHovered.id == 0) ? decalSelectEmpty : decalSelectEmpty
				};

				if (mat != null)
				{
					DrawSprite(hovered, mat, 0.312f);

				}
				// draw the quickbuild placeholder building
				if (CityBuild.action == CityBuild.Action.build)
				{
					//if (bdHovered.id == 0)
					//{

					//	var sel = CityBuild.quickBuildId;
					//	if (sel != 0)
					//	{
					//		DrawBuilding(hovered, iAlpha, sel, animationT * 0.3247f);
					//		ShellPage.contToolTip = $"Click to place { BuildingDef.all[sel].Bn }";
					//	}
					//}
					//else
					//{
					//	ShellPage.contToolTip = $"Spot is occupied";
					//}
				}
				else if (CityBuild.action == CityBuild.Action.move)
				{

					if (selected.IsValid())
					{
						if (Player.isAvatarOrTest ? IsTowerSpot(hovered):!IsBuildingSpot(hovered))
						{
							ShellPage.contToolTip = $"Please do not build buildings on walls";
						}
						else
						{
							if (bdHovered.id == 0)
							{
								ShellPage.contToolTip = $"Click to Move {GetBuilding(selected).name} to ({hovered.x},{hovered.y})";
								DrawBuilding(hovered, iAlpha, GetBuilding(selected).def.bid, animationT * 0.3249f);


							}
							else
							{
								if (bdHovered.isRes)
									ShellPage.contToolTip = $"Target is occupied by {bdHovered.Bn}, click to demo";
								else
								{
									ShellPage.contToolTip = $"Swap {bdHovered.Bn} with {GetBuilding(selected).name}, (takes 3 moves)";
									DrawBuilding(hovered, iAlpha, GetBuilding(selected).def.bid, animationT * 0.3249f);
									// draw a move icond too
								}
							}
						}

					}
					else
					{
						if (biHovered.isBuilding)
						{
							ShellPage.contToolTip = $"Click to move {bdHovered.Bn}";
						}
						else
						{
							ShellPage.contToolTip = $"Please Select a building to move";
						}
					}
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

		private static void DrawBuilding(int iAlpha, float zBase, float fontScale, (Vector2 c0,Vector2 c1) cs,in Building bid,int layer,byte fontAlpha=0, int bl1=0, byte fontAlpha1=0)
		{
			if (bid.id != 0)
			{
				var bd = BuildingDef.FromId(bid.id);

				var iconId = BidToAtlas(bd.bid);

				var u0 = iconId.x * duDt;
				var v0 = iconId.y * dvDt;
			

				draw.AddQuad(layer, buildingAtlas, cs.c0, cs.c1, new Vector2(u0, v0), new Vector2(u0 + duDt, v0 + dvDt), iAlpha.AlphaToAll(), (zBase, zBase, zBase, zBase)); // shader does the z transform
				if (fontAlpha == 0)
					fontAlpha = (byte)iAlpha;
				var textColor = new Color(0xf1, 0xd1, 0x1b, (int)fontAlpha);
				if (bid.bl != 0)
					DrawTextBox(bid.bl.ToString(), 0.825f.Lerp(cs.c0, cs.c1), textformatBuilding, textColor, fontAlpha,((int)layer+16), scale: fontScale, zBias: 0);
				if(fontAlpha1 !=0)
					DrawTextBox(bl1.ToString(), 0.825f.Lerp(cs.c0, cs.c1), textformatBuilding, textColor, fontAlpha1, ((int)layer + 17), scale: fontScale, zBias: 0);

			}


		}

		public static void DrawSprite( (int x, int y) cc, Material mat, float animFreq)
		{
			var off = (animationT * animFreq);
			var cScale = new Vector2(off.Wave().Lerp(0.8f, 1.0f), off.WaveC().Lerp(0.8f, 1.0f));
			var cs = CityPointToQuad(cc.x, cc.y, 1.2f);
			draw.AddQuad(Layer.tileCity + 2, mat, cs.c0, cs.c1, new Color( cityDrawAlpha, cityDrawAlpha, cityDrawAlpha, cityDrawAlpha / 2).Scale(cScale), PlanetDepth, zHover);
		}

		public static void DrawBuilding((int x, int y) cc,int iAlpha, int bid, float randomValue, Material overlay=null)
		{
			const float zBase = 0.0f;
			var iconId = BidToAtlas(bid);
			var u0 = iconId.x * duDt;
			var v0 = iconId.y * dvDt;
			var cs = CityPointToQuad(cc.x, cc.y);

			var off = randomValue;
			var cScale = new Vector2(off.Wave().Lerp(0.8f, 1.0f), off.WaveC().Lerp(0.8f, 1.0f));
			draw.AddQuad(Layer.tileCity, buildingAtlas, cs.c0, cs.c1, new Vector2(u0, v0), new Vector2(u0 + duDt, v0 + dvDt), iAlpha.AlphaToAll().Scale(cScale), (zBase, zBase, zBase, zBase)); // shader does the z transform
			if(overlay!=null)
				draw.AddQuad(Layer.tileCity + 2, overlay, cs.c0, cs.c1, new Color(iAlpha, iAlpha, iAlpha, iAlpha / 2).Scale(cScale), PlanetDepth, zHover);
		}

		public static void LoadContent()
		{
			LoadTheme();
			decalBuildingInvalid = LoadMaterialAdditive("Art/City/decal_building_invalid");
			decalBuildingValid = LoadMaterialAdditive("Art/City/decal_building_valid");
			decalBuildingValidMulti = LoadMaterialAdditive("Art/City/decal_building_valid_multi");
			decalMoveBuilding = LoadMaterialAdditive("Art/City/decal_move_building");
			decalSelectBuilding = LoadMaterialAdditive("Art/City/decal_select_building");
			decalSelectEmpty = LoadMaterialAdditive("Art/City/build_details_gloss_overlay");
		}
		public static void LoadTheme()
		{
			var cityBase = SettingsPage.IsThemeWinter() ? "Art/City/Winter/" : "Art/City/";
			buildingAtlas = Helper.LoadLitMaterial(cityBase + "building_set5");
			buildingShadows = new Draw.Material(buildingAtlas.texture, AGame.unlitEffect);
			cityWallsLand = Helper.LoadLitMaterial(cityBase + "baseland");
			cityWallsWater = Helper.LoadLitMaterial(cityBase + "basewater");
		}
		public static void UpdateLighting(Lighting value)
		{
			// reset effects to lit or unlit
			buildingAtlas.effect = AGame.GetTileEffect();
			cityWallsLand.effect = AGame.GetTileEffect();
			cityWallsWater.effect = AGame.GetTileEffect();
		}

		public static void ClearSelectedBuilding()
		{
			if (selected.IsValid())
			{
				selected = invalidXY;
				//App.DispatchOnUIThreadSneaky(() =>
				//{
				//	var i = Views.CityBuild.instance;
				//	i.rect.Fill = null;
				//	i.building.Text = string.Empty;
				//	i.description.Text = string.Empty;
				//	i.upgrade.IsEnabled = false;
				//	i.downgrade.IsEnabled = false;

				//});
			}
		}

	}

}
