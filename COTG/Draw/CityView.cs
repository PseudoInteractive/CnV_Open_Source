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

namespace COTG.Draw
{
	
	public class CityView
	{
		public static int XYToId((int x, int y) xy) => (xy.x - span0) + (xy.y - span0) * citySpan;
		const int atlasTileSize = 128;
		const int atlasColumns = 4;
		const int atlasRows = 33;
		const float duDt = (1.0f / atlasColumns);
		const float dvDt = (1.0f / atlasRows);
		public static Material buildingAtlas;
		public static Vector2 CityPointToCC( float x, float y)
		{
			return new Vector2(x*20+300, y*20+300).WToC();
		}

			const int span0 = -(citySpan-1)/2; // inclusive
		const int span1 = (citySpan+1)/2; // non inclusive
		public static JSON.Building[] buildingsCache;
		public static Building GetBuilding((int x, int y) xy) => buildingsCache[XYToId(xy)];
		public static void Draw()
		{
			buildingsCache = buildings;
			if (buildingsCache.IsNullOrEmpty())
				return;
			// draw each building tile

			for (var cy = span0; cy < span1; ++cy)
			{
				for (var cx = span0; cx < span1; ++cx)
				{
					var bid = GetBuilding((cx, cy));
					if (bid.id == 0)
						continue;
					var bd = BuildingDef.FromId(bid.id);
					var big = bd.bid;
					var iconId = big - 443;

					var u0 = (iconId%atlasColumns)* duDt;
					var v0 = (int)(iconId / atlasColumns) * dvDt;
					var c0 = CityPointToCC(cx, cy);
					var c1 = CityPointToCC(cx + 1, cy + 1);
					
					draw.AddQuad(Layer.tileBase, buildingAtlas, c0, c1, new Vector2(u0, v0), new Vector2(u0 + duDt, v0 + dvDt), 255.AlphaToWhite(), ConstantDepth, 0);
				}
			}
		}
		public static void LoadContent()
		{
			buildingAtlas = new Material(LoadTexture("Art/City/building_set5"),defaultEffect);
		}
	}
}
