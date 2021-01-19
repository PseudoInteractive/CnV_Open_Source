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
		public static Building GetBuilding( int id) => buildingsCache[id];
		public static Building GetBuilding((int x, int y) xy) => buildingsCache[XYToId(xy)];
		static Vector2 waterC =new Vector2( 1.0f - 768.0f* cityTileGainX / 128,
										1.0f - 704.0f*cityTileGainY / 128 );
		
		private static TextFormat textformatBuilding = new TextFormat(TextFormat.HorizontalAlignment.center, TextFormat.VerticalAlignment.center);

		public static void Draw(float alpha)
		{
			var build = City.GetBuild();
			if (build == null)
				return;
			int iAlpha = (int)(alpha * 255f);
			buildingsCache = build.buildings;
			
			buildCityOrigin = build.cid.CidToWorldV();
			// draw each building tile
			var city = City.GetBuild();

			var zBase = 0;
			var zHover = 1.0f / 64.0f * Views.SettingsPage.parallax;
			// selected

			
			var fontScale = (pixelScale / 64.0f) * (2.5f/64.0f) * SettingsPage.fontScale; // perspective attenuation with distance
			for (var cy = span0; cy <= span1; ++cy)
			{
				for (var cx = span0; cx <= span1; ++cx)
				{
					var bid = GetBuilding((cx, cy));
					if (bid.id == 0)
						continue;
					var bd = BuildingDef.FromId(bid.id);
					
					var iconId = BidToAtlas(bd.bid);

					var u0 = iconId.x* duDt;
					var v0 = iconId.y* dvDt;
					var cs = CityPointToQuad(cx, cy);
					
					draw.AddQuad(Layer.tileCity, buildingAtlas, cs.c0, cs.c1, new Vector2(u0, v0), new Vector2(u0 + duDt, v0 + dvDt), iAlpha.AlphaToAll(), (zBase, zBase, zBase, zBase) ); // shader does the z transform
					var textColor = new Color(0xf1, 0xd1, 0x1b, iAlpha);
					if (bid.bl!=0)
						DrawTextBox(bid.bl.ToString(), 0.825f.Lerp(cs.c0,cs.c1) , textformatBuilding, textColor,(byte) iAlpha, Layer.tileText, scale: fontScale,zBias:0);

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
							(var bid,var bd) = build.BFromOverlay((cx, cy));
							var current =  GetBuilding((cx, cy));
							var currentBid = current.bl > 0 ? current.def.bid : 0;
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

							draw.AddQuad(Layer.tileCity+1, buildingAtlas, cs.c0,0.5f.Lerp(cs.c0,cs.c1), new Vector2(u0, v0), new Vector2(u0 + duDt, v0 + dvDt), iAlpha.AlphaToAll(), (zBase, zBase, zBase, zBase)); // shader does the z transform
							
						}
					}
				}
				catch (Exception exception)
				{
					Log(exception);
				}

			}

			if (selected.IsValid())
			{
				var bi = GetBuilding(selected);
				var bd = bi.def;
				Material mat = CityBuild.action switch
				{
					CityBuild.Action.build => null,
					CityBuild.Action.destroy => null,
					CityBuild.Action.move => decalMoveBuilding,
					_ => decalSelectBuilding
				};

				if (mat != null)
				{
					var off = (animationT * 0.56f);
					var cScale = new Vector2(off.Wave().Lerp(0.8f, 1.0f), off.WaveC().Lerp(0.8f, 1.0f));
					var cs = CityPointToQuad(selected.x, selected.y, 1.2f);
					draw.AddQuad(Layer.tileCity + 2, mat, cs.c0, cs.c1, new Color(iAlpha, iAlpha, iAlpha, iAlpha / 2).Scale(cScale), PlanetDepth, zHover);

				}
			}

			// hovered
			if (hovered.IsValid())
			{
				var bi = GetBuilding(hovered);
				var bd = bi.def;

				Material mat = CityBuild.action switch
				{
					CityBuild.Action.build => (bd.id == 0) ? decalBuildingValid : decalSelectEmpty,
					CityBuild.Action.destroy => (bd.id == 0) ? decalSelectEmpty : decalBuildingInvalid,
					CityBuild.Action.move => selected.IsValid() ?
						(bd.id == 0 ? decalMoveBuilding : decalBuildingInvalid)
						: (bd.id == 0 ? decalSelectEmpty : decalMoveBuilding),
					// create
					_ => (bd.id == 0) ? decalBuildingValid : decalSelectEmpty
				};

				if (mat != null)
				{
					var off = (animationT * 0.57f);
					var cScale = new Vector2(off.Wave().Lerp(0.8f, 1.0f), off.WaveC().Lerp(0.8f, 1.0f));
					var cs = CityPointToQuad(hovered.x, hovered.y, 1.2f);
					draw.AddQuad(Layer.tileCity + 2, mat, cs.c0, cs.c1,new Color(iAlpha,iAlpha,iAlpha,iAlpha/2).Scale(cScale), PlanetDepth, zHover);

				}
				// draw the quickbuild placeholder building
				if (CityBuild.action == CityBuild.Action.build && bd.id == 0)
				{
					
					var sel = CityBuild.quickBuildId;
					if (sel != 0)
					{
						var iconId = BidToAtlas(sel);
						var u0 = iconId.x* duDt;
						var v0 = iconId.y* dvDt;
						var cs = CityPointToQuad(hovered.x, hovered.y);

						var off = (animationT * 0.61f);
						var cScale = new Vector2(off.Wave().Lerp(0.8f, 1.0f), off.WaveC().Lerp(0.8f, 1.0f));
						draw.AddQuad(Layer.tileCity, buildingAtlas, cs.c0, cs.c1, new Vector2(u0, v0), new Vector2(u0 + duDt, v0 + dvDt), 212.AlphaToAll().Scale(cScale), (zBase, zBase, zBase, zBase)); // shader does the z transform

					}
				}
				
			}
			var processed = new HashSet<int>();
			for (var it = buildQueue.iterate; it.Next();)
			{
				ref var r = ref it.r;
				int bspot = r.bspot;
				if (!processed.Add(bspot))
					continue;
				var cc = IdToXY(bspot);
				var cs = CityPointToQuad(cc.x, cc.y, 1.2f);
				var off = (bspot.BSpotToRandom() + animationT*0.5f);
				var cScale = new Vector2(off.Wave().Lerp(0.8f,1.0f), off.WaveC().Lerp(0.8f, 1.0f));
				if (r.elvl == 0)
				{
					// demo
					draw.AddQuad(Layer.tileCity + 2, decalBuildingInvalid, cs.c0, cs.c1, (new Color(iAlpha, iAlpha, iAlpha, iAlpha / 2)).Scale(cScale), PlanetDepth, zHover);
				}
				else if (r.slvl == 0)
				{
					// new building

					var iconId = BidToAtlas(r.brep);
					var u0 = iconId.x * duDt;
					var v0 = iconId.y * dvDt;
					var csb = CityPointToQuad(cc.x, cc.y);
					draw.AddQuad(Layer.tileCity, buildingAtlas, csb.c0, csb.c1, new Vector2(u0, v0), new Vector2(u0 + duDt, v0 + dvDt), iAlpha.AlphaToAll().Scale(cScale), (zBase, zBase, zBase, zBase)); // shader does the z transform
					var off2 = off * 1.07f;
					var cScale2 = new Vector2(off2.Wave().Lerp(0.8f, 1.1f), off2.WaveC().Lerp(0.8f, 1.1f));
					// now the overlay
					draw.AddQuad(Layer.tileCity + 2, decalBuildingValidMulti, cs.c0, cs.c1, new Color(iAlpha*3/4, iAlpha*3/4, iAlpha*3/4, iAlpha*3 / 8).Scale(cScale2), PlanetDepth, zHover);
				}
				else if (r.slvl < r.elvl)
				{
					// upgrade
					draw.AddQuad(Layer.tileCity + 2, decalBuildingValid, cs.c0, cs.c1, new Color(iAlpha , iAlpha , iAlpha , iAlpha / 2).Scale(cScale), PlanetDepth, zHover);
				}
				else
				{
					// downgrade or other, just highlight it
					draw.AddQuad(Layer.tileCity +2, decalSelectEmpty, cs.c0, cs.c1, new Color(iAlpha, iAlpha, iAlpha, iAlpha / 2).Scale(cScale), PlanetDepth, zHover);

				}
			}
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
				App.DispatchOnUIThreadSneaky(() =>
				{
					var i = Views.CityBuild.instance;
					i.rect.Fill = null;
					i.building.Text = string.Empty;
					i.description.Text = string.Empty;
					i.upgrade.IsEnabled = false;
					i.downgrade.IsEnabled = false;

				});
			}
		}

	}

}
