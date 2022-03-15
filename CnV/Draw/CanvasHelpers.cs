namespace CnV;

using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;
using static View;
using static GameClient;

 static partial class AGame
{
	
		
	public static string IncomingInfo(this City city)
	{
		if(!city.incoming.Any())
			return string.Empty;
		var ts = 0u;
		var sieged = false;
		var hasSen = false;
		var hasArt = false;
		var any = false;
		foreach(var i in city.incoming)
		{
			if(i.isAttack)
			{
				any = true;
				ts += i.ts;
				sieged |= i.isSiege;
				hasSen |= i.hasSenator;
				hasArt |= i.hasArt;
			}
		}
		if(!any)
			return string.Empty;
		return $"({(sieged ? ((hasArt&&hasSen ? "SgAr " : hasArt ? "Ar " : hasSen ? "Sg " : "Si ") + ((hasSen||city.claim>0) ? city.claim.ToString("00") + "% " : "")) : "i ") } { (ts ==0 ? "?" : (ts + 999) / 1000) }kTs)";
	}
	public static bool IsDark(this Color color) => ((int)color.R + color.G + color.B) < (int)color.A * 3 / 2;

	public static Color AlphaToWhite(this int alpha) { return new Color(255,255,255,alpha); }
	public static Color AlphaToAll(this int alpha) { return new Color(alpha,alpha,alpha,alpha); }
	public static Color AlphaToAll(this byte alpha) { return new Color(alpha,alpha,alpha,alpha); }
	public static Color AlphaToBlack(this int alpha) { return new Color(0,0,0,alpha); }

	public static Color Scale(this Color value,Vector4 scale)
	{
		return new Color((int)((float)(int)value.R * scale.X),(int)((float)(int)value.G * scale.Y),(int)((float)(int)value.B * scale.Z),(int)((float)(int)value.A * scale.W));

	}
	public static Color Scale(this Color value,float scale)
	{
		return new Color((int)((float)(int)value.R * scale),(int)((float)(int)value.G * scale),(int)((float)(int)value.B * scale),(int)((float)(int)value.A * scale));

	}
	public static Color Scale(this Color value,Vector2 scale)
	{
		return new Color((int)((float)(int)value.R * scale.X),(int)((float)(int)value.G * scale.X),(int)((float)(int)value.B * scale.X),(int)((float)(int)value.A * scale.Y));

	}

	public static Color AlphaToWhite(this byte alpha) { return new Color(byte.MaxValue,byte.MaxValue,byte.MaxValue,alpha); }
	public static Color AlphaToBlack(this byte alpha) { return new Color((byte)0,(byte)0,(byte)0,alpha); }

	//public static Point2 ToPoint(this Vector2 me) => new Point2(me.X,me.Y);


	//public static Vector4 ToFVector4(this Color c) => new Vector4(c.R / 255, c.G / 255, c.B / 255, c.A / 255);
	//public static void DrawLine(this CanvasSpriteBatch b, Vector2 c0, Vector2 c1, Vector4 c, float thickness, CanvasStrokeStyle style)
	//{
	//	var dl = c1 - c0;
	//	var dllg = dl.Length();
	//	var dllgInv = 1 / dllg;
	//	var de = new Vector2(dl.Y * dllgInv * thickness, dl.X * dllgInv * thickness);
	//	float xoffset = ShellPage.animationT * 4 % 256;
	//	b.DrawFromSpriteSheet(ShellPage.lineDraw, new Matrix3x2(dl.X, dl.Y, de.X, de.Y, c0.X - de.X * 0.5f, c0.X - de.X * 0.5f), new Rect(xoffset, 0, dllg * 16 + xoffset, 128), c);
	//}

	//public static void DrawRoundedSquare(this CanvasDrawingSession ds, Vector2 c, float circleRadius, Color color, float thickness = 1.5f)
	//{
	//    ds.DrawRoundedRectangle(c.X - circleRadius, c.Y - circleRadius, circleRadius*2, circleRadius*2, circleRadius*0.25f, circleRadius*0.25f, color, thickness);
	//}


	//public static void DrawRoundedSquareWithShadow(this CanvasDrawingSession ds, Vector2 c, float circleRadius, Color brush, float thickness = 1.5f)
	//      {
	//          DrawRoundedSquareShadow(ds, c, circleRadius, brush.GetShadowColor(), thickness);
	//          DrawRoundedSquareBase(ds, c, circleRadius, brush, thickness);
	//      }
	//public static void DrawRoundedSquareShadow(this CanvasDrawingSession ds, Vector2 c, float circleRadius, Color color, float thickness = 1.5f)
	//      {
	//          DrawRoundedSquare(ds, c, circleRadius, color, thickness);
	//      }
	//      public static void DrawRoundedSquareBase(this CanvasDrawingSession ds, Vector2 c, float circleRadius, Color brush, float thickness = 1.5f)
	//      {
	//          DrawRoundedSquare(ds, c - ShellPage.shadowOffset, circleRadius, brush, thickness);
	//      }
	//	public static float parallaxZ0 = 1024;
	//public static float ParallaxScale(float dz) => Settings.wantParallax? parallaxZ0 / (parallaxZ0 - dz) : 1.0f;
	// for now we just the same bias for shadows as for light, assuming that the camera is as far from the world as the light
	//	public static float ParallaxScaleShadow(float dz) => 1;// parralaxZ0 / (parralaxZ0 - dz);
	public static float bulgeInputGain = 0;
	public static float bulgeNegativeRange = 0.875f;
	public static float CToDepth(this Vector2 c,float zBias)
	{
		return zBias;
		//float r2 = (c.LengthSquared() * bulgeInputGain).Min(1.0f);
		////			return r.SLerp(AGame.bulgeGain, 0.0f);
		////Assert(r2 <= 2.0f);
		//return (r2).Lerp(1,-bulgeNegativeRange) * bulgeGain + zBias;
		//	return Math.Acos(r.SLerp(AGame.bulgeGain, 0.0f);

	}
	
	public static float PlanetDepth(float x, float y, float zBase)
	{
		return new Vector2(x, y).CToDepth(zBase);
	}
	
	public static float ConstantDepth(float x, float y, float zBase)
	{
		return zBase;

	}

	//public static Vector2 Project(this Vector3 c)
	//{
	//	float scale = 1.0f / (AGame.cameraZ - c.Z);
	//	return new Vector2(c.X * scale,c.Y * scale);
	//}

	//public static (Vector2 c, float scale) ViewToScreen(this Vector2 c,float zBias)
	//{
	//	float scale = 1.0f / (viewW.Z - zBias);
	//	return (new Vector2(c.X * scale,c.Y * scale), scale);

	//}

	//public static Vector2 InverseProject(this Vector2 c,float z)
	//{
	//	float scale = (viewW.Z - z);
	//	return new Vector2(c.X * scale,c.Y * scale);
	//}

	//public static Vector2 InverseProject(this Vector2 c)
	//{
	//	return c;
	//	//float zBias = zCities;
	//	//float z = CToDepth(c,zBias);

	//	////for(int i = 0;i < 8;++i)
	//	////{
	//	////	var test = InverseProject(c,z);
	//	////	z = new Vector2(test.X,test.Y).CToDepth(zBias);
	//	////}
	//	//return InverseProject(c,z);
	//}

	//		public static float CToDepth(this (float x, float y) c) => CToDepth(new Vector2(c.x, c.y));
	//	public static Vector2 WToCp(this Vector2 c, float dz)
	//{
	//	var paralaxGain = ParallaxScale(dz);

	//	return (c- AGame.cameraCLag)* paralaxGain*AGame.pixelScale;
	//}
	//	public static Vector2 WToCp(this (float x, float y) c, float dz) => WToCp(new Vector2(c.x, c.y), dz);
	
	public static float ScreenToWorld(this float s)
	{
		return s*pixelScaleInverse;
	}
	public static Vector2 ScreenToWorld(this Vector2 s)
	{
		return (s - AGame.projectionC)*pixelScaleInverse + View.viewW2;
	}

	public static Vector2 ScreenToWorldOffset(this Vector2 s)
	{
		return (s )*pixelScaleInverse ;
	}
	public static Vector2 DipToWorldOffset(this Vector2 s)
	{
		return (s )*dipScaleInverse ;
	}

	public static float DipToWorld(this float s)
	{
		return (s )*dipScaleInverse ;
	}
	//public static Vector2 CameraToScreen(this Vector2 s)
	//{
	//	return s* pixelScale + AGame.projectionC;
	//}

	//public static Vector2 WorldToScreen(this Vector2 s)
	//{
	//	return CameraToScreen(WorldToCamera(s));
	//}
	//public static Vector3 WorldToCamera(this Vector3 s)
	//{
	//	return s - viewW;
	//}
	//	public static Vector2 WToCpSpan(this Vector2 c, float dz)
	//{
	//	var paralaxGain = ParallaxScale(dz);

	//	return (c)* paralaxGain*AGame.pixelScale ;
	//}
	//public static float WToCpSpan( float dz)
	//{
	//	var paralaxGain = ParallaxScale(dz);

	//	return paralaxGain * AGame.pixelScale;
	//}
	//public static Vector2 WToCShadow(this Vector2 c, float dz)
	//{
	//	var paralaxGain = ParalaxScaleShadow(dz);
	//	return (c - AGame.worldLightC) * paralaxGain * AGame.pixelScale + AGame.cameraLightC;
	//}
	//public static Vector2 CToCShadow(this Vector2 c, float dz)
	//{
	//	//var paralaxGain = ParalaxScaleShadow(dz);
	//	return c;//(c - AGame.cameraLightC) * paralaxGain + AGame.cameraLightC;
	//}
	// camera space to camera space with parallax
	//public static Vector2 CToCp(this Vector2 c, float dz)
	//{
	//	var paralaxGain = ParallaxScale(dz);
	//		return (c) * paralaxGain;
	//}
	//public static Point CToCp(this Vector2 c, float dz)
	//{
	//	var paralaxGain = ParalaxScale(dz);
	//	return ((c - AGame.halfSpan) * paralaxGain + AGame.halfSpan).ToPoint();

	//}
	public static Windows.Foundation.Point AsPoint(this Vector2 c)
	{
		return new(c.X,c.Y);
	}
	public static Vector2 WorldToCamera(this Vector2 c)
	{
		return (c - View.viewW2) ;
	}
	//public static Vector2 CameraToWorldDelta(this Vector2 c)
	//{
	//	return (c) ;
	//}
	//public static Vector2 CameraToWorld(this Vector2 c)
	//{
	//	return c + View.viewW2;
	//}
	public static Vector3 CameraToWorld(this Vector3 c)
	{
		return c + View.viewW;
	}

	//public static Vector2 WorldToCamera(this (int x, int y) c)
	//{
	//	return new Vector2(c.x,c.y).WorldToCamera();
	//}
	//public static Vector2 WorldToCamera(this (float x, float y) c)
	//{
	//	return new Vector2(c.x,c.y).WorldToCamera();
	//}
	//	public static Vector2 WToCp(this (int x, int y) c, float z)
	//{
	//	return new Vector2(c.x, c.y).WToCp(z);
	//}
	//public static Vector2 CidToCamera(this int c)
	//{
	//	return c.ToWorldC().WorldToCamera();
	//}
	//public static Vector2 CidToCp(this int c, float z)
	//{
	//	return c.ToWorldC().WToCp(z);
	//}a

	public static (float v0, float v10, float v01, float v11) RectDepth(this (Vector2 c0, Vector2 c1) c,float dz)
	{
		return (c.c0.CToDepth(dz), new Vector2(c.c1.X,c.c0.Y).CToDepth(dz),
			new Vector2(c.c0.X,c.c1.Y).CToDepth(dz), c.c1.CToDepth(dz));
	}
	//public static bool BringCidIntoWorldView(this int cid,bool lazy)
	//{
	//	var worldC = cid.ToWorldC();
	//	var cc = WorldToCamera(worldC);
	//	//	if (ShellPage.IsCityView())
	//	//		lazy = false;
	//	// only move if needed, heuristic is if any part is off screen
	//	if(!lazy ||
	//	   !AGame.clip.ContainsSquare( cc,AGame.pixelScale*1.5f))
	//	{
	//		var thresh = 32;//lazy ? 0.5f : 0.5f;
	//		// only move if moving more than about 64 pixels (should be fraction of screen?)
	//		if(cc.LengthSquared() >= thresh.Squared() )
	//		{
	//			// try region view
					
				
	//			AGame.CameraC = worldC;
	//			ShellPage.SetJSCamera();
	//			if(cid != City.build && (!City.CanVisit(cid) || !Spot.CanChangeCity(cid)))
	//				ShellPage.EnsureNotCityView();

	//			return true;
	//		}

	//	}
	//	return false;
	//}

	
	public static Color GetShadowColor(this Color c)
	{
		return new Color((byte)(c.R * 0 / 4),(byte)(c.G * 0 / 4),(byte)(c.B * 0 / 4),(byte)240);
		//            (0.625f).Lerp(c, new Color(128, 0, 0, 0));
		//            (0.625f).Lerp(c, new Color(128, 0, 0, 0));
	}
	public static Color GetShadowColorDark(this Color c)
	{
		return new Color((byte)(c.R * 0 / 4),(byte)(c.G * 0 / 4),(byte)(c.B * 0 / 4),(byte)240);
		//            (0.625f).Lerp(c, new Color(128, 0, 0, 0));
		//            (0.625f).Lerp(c, new Color(128, 0, 0, 0));
	}
	public static (Vector2 c0, Vector2 c1) GetBounds(this Rectangle textureRegion)
	{
		return (new Vector2(textureRegion.X,textureRegion.Y), new Vector2(textureRegion.Right,textureRegion.Bottom));
	}

	const float gamma = 2.2f;
	public static (float r, float g, float b, float a) LinearToSRGB(this (float r, float g, float b, float a) c)
	{
		return (MathF.Pow(c.r,1.0f / gamma), MathF.Pow(c.g,1.0f / gamma), MathF.Pow(c.b,1.0f / gamma), c.a);

	}
	public static Microsoft.Xna.Framework.Vector4 LinearToSRGB(this Microsoft.Xna.Framework.Vector4 c)
	{
		return new Microsoft.Xna.Framework.Vector4(MathF.Pow(c.X,1.0f / gamma),MathF.Pow(c.Y,1.0f / gamma),MathF.Pow(c.Z,1.0f / gamma),c.W);

	}
	public static Color LinearToSrgb(this Color c)
	{
		return new Color(c.ToVector4().LinearToSRGB());
	}
	public static Color SRGBToLinear(this Color c)
	{
		return new Color(c.ToVector4().SRGBToLinear());
	}
	public static Microsoft.Xna.Framework.Vector4 SRGBToLinear(this Microsoft.Xna.Framework.Vector4 c)
	{
		return new Microsoft.Xna.Framework.Vector4(MathF.Pow(c.X,gamma),MathF.Pow(c.Y,gamma),MathF.Pow(c.Z,gamma),c.W);

	}
}
