﻿using CnV.Draw;

using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CnV.Debug;
namespace CnV
{
	using Draw;

	public class SpriteAnim
	{
		public byte frameCount;
		public byte frameDeltaG; // this is the offset ot the next frame in texture coord units
		public byte frameDeltaB; // This is an extra 8 bit fractional part added to the offset
		public string asset;
		public Material material;
		public string assetPath => $"Art/Anim/{asset}";
		public void Load() { material =new Material( GameClient.instance.Content.Load<Texture2D>(assetPath),GameClient.animatedSpriteEffect); }
		
		public static (byte frameDeltaG, byte frameDeltaB) ComputeFrameDeltaAsColours(int frameCount)
		{
			double dFrameCount = frameCount;
			double frameDelta = 1.0 / dFrameCount;
			var g = Math.Floor(frameDelta * 255.0);
			var error = frameDelta - (g / 255.0);
			Assert(error >= 0.0);
			Assert(error <= 1.0/255.0);
			var b = (int)(error * 255.0 * 255.0);
			Assert(b >= 0);
			Assert(b <= 255);
			return ( ((int)g).AsByte(),b.AsByte());
		}

		public SpriteAnim(string _asset, int _frameCount)
		{
			asset = _asset;
			frameCount = (byte)_frameCount;
			(frameDeltaG, frameDeltaB) = ComputeFrameDeltaAsColours(frameCount);
		}
		public static SpriteAnim flagHome = new SpriteAnim("flagAnim0",12);
		public static SpriteAnim flagSelected = new SpriteAnim("flagAnim4",12);
		public static SpriteAnim flagPinned= new SpriteAnim("flagAnim1",12);
		public static SpriteAnim flagRed = new SpriteAnim("flagAnim3",12 );
		public static SpriteAnim flagGrey = new SpriteAnim("flagAnimGrey", 12);
	}

}
