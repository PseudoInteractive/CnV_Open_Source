using COTG.Draw;

using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static COTG.Debug;
namespace COTG
{
	public class SpriteAnim
	{
		public byte frameCount;
		public byte frameDeltaG; // this is the offset ot the next frame in texture coord units
		public byte frameDeltaB; // This is an extra 8 bit fractional part added to the offset
		public string asset;
		public Material material;
		public string assetPath => $"Art/Anim/{asset}";
		public void Load() { material =new Material( AGame.instance.Content.Load<Texture2D>(assetPath),AGame.animatedSpriteEffect); }
		
		public SpriteAnim(string _asset, int _frameCount)
		{
			asset = _asset;
			frameCount = (byte)_frameCount;
			double dFrameCount = _frameCount;
			double frameDelta = 1.0 / dFrameCount;
			var g = Math.Floor(frameDelta * 255.0);
			var error = frameDelta - (g / 255.0);
			Assert(error >= 0.0);
			Assert(error <= 1.0/255.0);
			var b = (int)(error * 256.0 * 255.0);
			Assert(b >= 0);
			Assert(b <= 255);
			frameDeltaB = (byte)b;
			frameDeltaG = (byte)g;
	//		var resultError = (frameDelta - g / 255.0 - b / 256.0 / 255.0);
	//		COTG.Debug.Log($"{_asset} {resultError}");
		}
		public static SpriteAnim flagHome = new SpriteAnim("flagAnim0",12);
		public static SpriteAnim flagSelected = new SpriteAnim("flagAnim4",12);
		public static SpriteAnim flagPinned= new SpriteAnim("flagAnim1",12);
		public static SpriteAnim flagRed = new SpriteAnim("flagAnim3",12 );
		public static SpriteAnim flagGrey = new SpriteAnim("flagAnimGrey", 12);
	}

}
