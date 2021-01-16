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

namespace COTG.Draw
{
	
	public class CityView
	{
	
		const int atlasTileSize = 128;
		const int atlasColumns = 4;
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
		public static Vector2 half2 = new Vector2(0.5f, 0.5f);

		public static (int x, int y) selected = invalidXY;
		public static (int x, int y) hovered = invalidXY;
	//	static Color textColor = new Color(0xf1, 0xd1, 0x1b, 0xff);
		public static Vector2 CityPointToCC( float x, float y)
		{
			return new Vector2(x*cityTileGainX+ buildCityOrigin.X, y * cityTileGainY + buildCityOrigin.Y).WToC();
		}
		public static (Vector2 c0, Vector2 c1) CityPointToQuad(float x, float y, float additionalYScale = 1.0f)
		{
			var c = CityPointToCC(x,y);
			// There is no aspect ratio scaling for Y
			var baseScale = (0.5f * cityTileGainX * AGame.pixelScale);
			var dc = new Vector2(baseScale, baseScale* additionalYScale);
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

			if (selected.IsValid())
			{
				var bi = GetBuilding(selected);
				var bd = bi.def;
				Material mat = null;
				if (bd.id == 0)
				{
					// empty
					mat = decalSelectBuilding;
				}
				else if (bi.bl > 0)
				{
					// building
					mat = decalSelectBuilding;
				}
				else
				{
					mat = decalBuildingInvalid;
				}

				if (mat != null)
				{
					var cs = CityPointToQuad(selected.x, selected.y, 0.84f);
					draw.AddQuad(Layer.tileCity + 1, mat, cs.c0, cs.c1, iAlpha.AlphaToAll(), PlanetDepth, zHover);

				}
			}

			// hovered
			if (hovered.IsValid() )
			{
				var bi = GetBuilding(hovered);
				var bd = bi.def;
				Material mat = null;
				if (bd.id == 0)
				{
					// empty
					mat = decalBuildingValid;
				}
				else if(bi.bl > 0)
				{
					// building
					mat = decalMoveBuilding;
				}
				else 
				{
					mat = decalBuildingInvalid;
				}

				if ( mat != null)
				{ 
					var cs = CityPointToQuad(hovered.x, hovered.y, 0.84f);
					draw.AddQuad(Layer.tileCity + 1, mat,cs.c0,cs.c1, iAlpha.AlphaToAll(), PlanetDepth, zHover);

				}
			}
			var fontScale = (pixelScale / 64.0f) * (1.25f/64.0f) * SettingsPage.fontScale; // perspective attenuation with distance
			for (var cy = span0; cy <= span1; ++cy)
			{
				for (var cx = span0; cx <= span1; ++cx)
				{
					var bid = GetBuilding((cx, cy));
					if (bid.id == 0)
						continue;
					var bd = BuildingDef.FromId(bid.id);
					
					var iconId = bd.bid - 443;

					var u0 = (iconId%atlasColumns)* duDt;
					var v0 = (int)(iconId / atlasColumns) * dvDt;
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
			draw.AddQuad(Layer.tileCity - 2, city.isOnWater ? cityWallsWater : cityWallsLand, city0.WToC(), city1.WToC(), iAlpha.AlphaToAll(), PlanetDepth, zBase);
			
		}
		public static void LoadContent()
		{
			LoadTheme();
			decalBuildingInvalid = LoadMaterial("Art/City/decal_building_invalid");
			decalBuildingValid = LoadMaterial("Art/City/decal_building_valid");
			decalBuildingValidMulti = LoadMaterial("Art/City/decal_building_valid_multi");
			decalMoveBuilding = LoadMaterial("Art/City/decal_move_building");
			decalSelectBuilding = LoadMaterial("Art/City/decal_select_building");
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
					i.Building.Text = string.Empty;
					i.Description.Text = string.Empty;
					i.Upgrade.IsEnabled = false;
					i.Downgrade.IsEnabled = false;

				});
			}
		}

	}

}
