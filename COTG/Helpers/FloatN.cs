using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;


using static COTG.Debug;
using static COTG.AMath;

namespace COTG
{
	/// <summary>
	/// Helpers for <see cref="Vector3"></see>, Vector2, etc./>
	/// </summary>
	public static class FloatN
	{

		#region Float

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Remap(this float value, float from1, float to1, float from2, float to2)
		{
			return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Remap01(this float value, float from1, float to1)
		{
			return (value - from1) / (to1 - from1);
		}
		// Why do generics not accept operator *?
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Bezier(this float t, float p0, float p1, float p2, float p3)
		{
			float t2 = 1 - t;
			return t2 * t2 * t2 * p0 + 3 * t2 * t2 * t * p1 + 3 * t2 * t * t * p2 + t * t * t * p3;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float BSpline(this float t, float p0, float p1, float p2)
		{
			// t in range 0.0 .. 1.0
			Assert(t >= 0.0f);
			Assert(t <= 1.0f);

			//	float t2 = 1 - t;
			//	return t2*t2 *0.5f*(p0+p1) + 2*t*t2* p1 +  t* t *0.5f*(p1+p2);
			return (0.5f - t + 0.5f * t * t) * (p0) + (t - t * t + 0.5f) * p1 + t * t * 0.5f * p2;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Bezier(this float t, float p0, float p1, float p2)
		{
			// t in range 0.0 .. 1.0
			Assert(t >= 0.0f);
			Assert(t <= 1.0f);

			float t2 = 1 - t;
			return t2 * t2 * p0 + 2 * t * t2 * p1 + t * t * p2;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 BSpline(this float t, Vector2 p0, Vector2 p1, Vector2 p2)
		{
			Assert(t >= 0.0f);
			Assert(t <= 1.0f);

			//	float t2 = 1 - t;
			//	return t2 * t2 * 0.5f * (p0 + p1) + 2 * t * t2 * p1 + t * t * 0.5f * (p1 + p2);
			return (0.5f - t + 0.5f * t * t) * (p0) + (t - t * t + 0.5f) * p1 + t * t * 0.5f * p2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Bezier(this float t, Vector2 p0, Vector2 p1, Vector2 p2)
		{
			Assert(t >= 0.0f);
			Assert(t <= 1.0f);

			float t2 = 1 - t;
			return t2 * t2 * p0 + 2 * t * t2 * p1 + t * t * p2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Bezier(this Vector3 t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			Vector3 t2 = new Vector3(1.0f) - t;
			return t2 * t2 * t2 * p0 + 3 * t2 * t2 * t * p1 + 3 * t2 * t * t * p2 + t * t * t * p3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Bezier(this float t, Vector4 p0, Vector4 p1, Vector4 p2, Vector4 p3)
		{
			Vector4 t2 = new Vector4(1.0f - t);
			return t2 * t2 * t2 * p0 + 3 * t2 * t2 * t * p1 + 3 * t2 * t * t * p2 + t * t * t * p3;
		}

		/// <summary>
		///   Interpolates between p1 and p2, p0 and p3 are used for derivatives/curvature
		///   For ease in set p0==p1
		///   For ease out set p3==p2
		/// </summary>
		/// <param name="t">interpolation, 0..1 not saturated first, if saturation is needed use t.Saturate().CatMullRom(...)</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 CatmullRom(this float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			var a = 2f * p1;
			var b = p2 - p0;
			var c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
			var d = -p0 + 3f * p1 - 3f * p2 + p3;
			//The cubic polynomial: a + b * t + c * t^2 + d * t^3
			return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
		}
		/// <summary>
		///   Interpolates between p1 and p2, p0 and p3 are used for derivatives/curvature
		///   For ease in set p0==p1
		///   For ease out set p3==p2
		/// </summary>
		/// <param name="t">interpolation, 0..1 not saturated first, if saturation is needed use t.Saturate().CatMullRom(...)</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float CatmullRom(this float t, float p0, float p1, float p2, float p3)
		{
			var a = 2f * p1;
			var b = p2 - p0;
			var c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
			var d = -p0 + 3f * p1 - 3f * p2 + p3;
			//The cubic polynomial: a + b * t + c * t^2 + d * t^3
			return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
		}

		public static Vector4 CatmullRom(this float t, Vector4 p0, Vector4 p1, Vector4 p2, Vector4 p3)
		{
			var a = 2f * p1;
			var b = p2 - p0;
			var c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
			var d = -p0 + 3f * p1 - 3f * p2 + p3;
			//The cubic polynomial: a + b * t + c * t^2 + d * t^3
			return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
		}
		/// <summary>
		/// Returns a calculated p0 from p1, p2 such that the first segment is linear
		/// </summary>
		/// <param name="t">interpolation, 0..1 not saturated first, if saturation is needed use t.Saturate().CatMullRom(...)</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float CatmulRomEaseLinear(this (float p1, float p2) p)
		{
			return 2 * p.p1 - p.p2;
		}
		public static Vector4 CatmulRomEaseLinear(this Vector4 p1, Vector4 p2)
		{
			return 2 * p1 - p2;
		}
		public static float CatmullRom(this float t, float p0, float p1, float p2, float p3, float p4)
		{
			if (t <= 0.5f)
				return CatmullRom(t * 2.0f, p0, p1, p2, p3);
			else
				return CatmullRom((t - 0.5f) * 2.0f, p1, p2, p3, p4);

		}

		public static Vector4 CatmullRom(this float t, Vector4 p0, Vector4 p1, Vector4 p2, Vector4 p3, Vector4 p4)
		{
			if (t <= 0.5f)
				return CatmullRom(t * 2.0f, p0, p1, p2, p3);
			else
				return CatmullRom((t - 0.5f) * 2.0f, p1, p2, p3, p4);

		}
		/// <summary>
		///   Interpolates between p1 and p4, hitting p2 at 0.333, p3 at 0.66667.  
		///   p0 and p5 are used for derivatives/curvature
		///   For ease in set p0==p1
		///   For ease out set p5==p4
		/// </summary>
		/// <param name="t">interpolation, 0..1 not saturated first, if saturation is needed use t.Saturate().CatMullRom(...)</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float CatmullRom(this float t, float p0, float p1, float p2, float p3, float p4, float p5)
		{
			if (t <= (1.0f / 3.0f))
				return CatmullRom(t * 3.0f, p0, p1, p2, p3);
			else if (t <= (2.0f / 3.0f))
				return CatmullRom((t - (1.0f / 3.0f)) * 3.0f, p1, p2, p3, p4);
			else
				return CatmullRom((t - (2.0f / 3.0f)) * 3.0f, p2, p3, p4, p5);

		}
		public static float CatmullRomLinear(this float t, float p0, float p1, float p2, float p3, float p4)
		{
			return CatmullRom(t, CatmulRomEaseLinear( (p0, p1) ), p0, p1, p2, p3,p4, (p3,p4).CatmulRomEaseLinear());

		}

		/// <summary>
		///   Interpolates between p1 and p5, hitting p2 at 0.25 etc  
		///   p0 and p6 are used for derivatives/curvature
		///   For ease in set p0==p1
		///   For ease out set p6==p5
		/// </summary>
		/// <param name="t">interpolation, 0..1 not saturated first, if saturation is needed use t.Saturate().CatMullRom(...)</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float CatmullRom(this float t, float p0, float p1, float p2, float p3, float p4, float p5, float p6)
		{
			if (t <= 0.25f)
				return CatmullRom(t * 4.0f, p0, p1, p2, p3);
			else if (t <= 0.5f)
				return CatmullRom((t - 0.25f) * 4.0f, p1, p2, p3, p4);
			else if (t <= 0.75f)
				return CatmullRom((t - 0.5f) * 4.0f, p2, p3, p4, p5);
			else
				return CatmullRom((t - 0.75f) * 4.0f, p3, p4, p5, p6);

		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float RemapSaturate(this float value, float from1, float to1, float from2, float to2)
		{
			float t = AMath.Saturate((value - from1) / (to1 - from1));
			return t * (to2 - from2) + from2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float RemapClamp(this float value, float from1, float to1, float from2, float to2)
		{
			float t = (AMath.Clamp(value, from1, to1) - from1) / (to1 - from1);
			return t * (to2 - from2) + from2;
		}

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static float LerpAngle(this float t, float from, float to)
		//{
		//	float da = uMath.DeltaAngle(from, to);
		//	return from + AMath.Saturate(t) * da;
		//}

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static Vector3 Lerp(this float t, Vector3 from, Vector3 to)
		//{
		//	return from + (t) * (to - from);
		//}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Towards(this float v, float target, float speed)
		{
			return v > target ? AMath.Max(target, v - speed) : AMath.Min(target, v + speed);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Towards(this float v, float target, float negSpeed, float posSpeed)
		{
			return v > target ? AMath.Max(target, v - negSpeed) : AMath.Min(target, v + posSpeed);
		}


		

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static sbyte Max(this sbyte x, sbyte y)
		{
			return (x > y) ? x : y;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static sbyte Min(this sbyte x, sbyte y)
		{
			return (x < y) ? x : y;
		}

	

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Cubed(this float t)
		{
			return t * t * t;
		}
	
		#endregion

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Normalize(ref Vector3 v)// not safe vs divide by 0
		{
			v = v / v.Length();

		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Vector3 ClampMag(this Vector3 a, float mag)
		{
			Assert(mag >= 0.0f);
			var _mag = a.Length();
			if (_mag < mag)
				return a;
			else
				return a * (mag / _mag);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float LerpSat(this float t, float from, float to)
		{
			return from + AMath.Saturate(t) * (to - from);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float InverseLerp(this float t, float from, float to)
		{
			return (t - from) / (to - from);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 SignedLerp(this float t, Vector3 from, Vector3 to)
		{
			return t >= 0 ? from + t * (to - from) : from + t * (to + from);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 LerpSat(this float t, Vector3 from, Vector3 to)
		{
			return from + AMath.Saturate(t) * (to - from);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Lerp(this float t, Vector3 from, Vector3 to)
		{
			return from + t * (to - from);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 NLerp(this float t, Vector3 from, Vector3 to)
		{
			return (from + t * (to - from)).Normalized();
		}

		public static Vector3 _000 = new Vector3(0, 0, 0);
		public static Vector3 _100 = new Vector3(1, 0, 0);
		public static Vector3 _010 = new Vector3(0, 1, 0);
		public static Vector3 _001 = new Vector3(0, 0, 1);
		//public static Vector3 mul(this Matrix3x3 a, Vector3 b)
		//{
		//	return math.mul(a, b);
		//}

		//public static Vector3 TDot(this float3x3 a, Vector3 b)
		//{
		//	return new Vector3(dot(a.c0, b), dot(a.c1, b), dot(a.c2, b));
		//}

		//public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float maxDistanceDelta)
		//{
		//	var dc = target - current;
		//	float dist2 = dc.LengthSquared();
		//	if (dist2 <= maxDistanceDelta * maxDistanceDelta)
		//		return target;
		//	float gain = maxDistanceDelta * math.rsqrt(dist2);
		//	return current + dc * gain;
		//}
	}
}