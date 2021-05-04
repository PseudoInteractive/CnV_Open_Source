using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


using System;
using System.Collections.Generic;


namespace COTG.Draw
{
	// SortKey = (layer << 20) + (Effect._sortingKey << 16) + Texture.SortingKey;
	public static class Layer
	{
		public const int background = 1;
		public const int tileBase=16;
		public const int tileShadow = 32;
		public const int tiles = 48;
		public const int tileCity = tiles+16;
		public const int tileText = action+10;
		public const int textBackground = tiles + 32;
		public const int effectShadow = textBackground-1;

		public const int effects = 128;
		public const int action = effects+8;
		public const int labelText = maxLayer - 30;
		public const int overlay = maxLayer-20;
		public const int overlayText = maxLayer-10;
		public const int webView = maxLayer - 9;
		public const int maxLayer = (1 << 16) - 1; // 4095
	

	}
}

