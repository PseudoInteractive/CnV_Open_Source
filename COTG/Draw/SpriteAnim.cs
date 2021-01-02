using COTG.Draw;

using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG
{
	public class SpriteAnim
	{
		public int frameCount;
		public string asset;
		public Material material;
		public string assetPath => $"Art/Anim/{asset}";
		public void Load() { material =new Material( AGame.instance.Content.Load<Texture2D>(assetPath)); }
		
		public static SpriteAnim flagHome = new SpriteAnim() { frameCount = 12, asset = "flagAnim0" };
		public static SpriteAnim flagSelected = new SpriteAnim() { frameCount = 12, asset = "flagAnim4" };
	}

}
