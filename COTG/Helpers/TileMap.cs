using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Helpers
{
    public class TileMap
    {
        public Vector2 span;
        public ICanvasImage image;
        public int guidStart;
        public int tileCount;
        public const float tileWidth = 64.0f;
        public const float tileHeight = 64.0f;

        public static TileMap[] tileMaps = Array.Empty<TileMap>();

        public static void Load(string jsonName)
        {

        }
    }
}
