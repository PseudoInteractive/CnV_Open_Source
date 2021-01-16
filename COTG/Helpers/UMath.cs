using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace COTG
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct FloatIntUnion
	{
		[FieldOffset(0)]
		public uint intValue;

		[FieldOffset(0)]
		public float floatValue;

	}

	//// Todo:  Test this
	//[StructLayout(LayoutKind.Explicit)]
	//internal unsafe struct ArrayConverter<T> where T: unmanaged
	//{
	//	[FieldOffset(0)]
	//	public T[] tArray;

	//	[FieldOffset(0)]
	//	public byte[] byteArray;

	//       public ArrayConverter(T[] _v) { byteArray = null;tArray = _v; }
	//       public ArrayConverter(byte[] _v) { tArray = null; byteArray = _v; }
	//}

	static public class uMath
	{

	


		public static Vector3 reflectNormalized(this Vector3 normal, Vector3 vector)
		{
			return vector - 2.0f * normal.Dot(vector) * normal;
		}
		public static Vector3 reflect(this Vector3 normal, Vector3 vector)
		{
			return vector - 2.0f * normal.Dot(vector) / normal.LengthSquared() * normal;
		}
		/// <summary>
		/// Returns Sign, or 1 if 0
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SignOr1(this float f) => f >= 0.0f ? 1f : -1f;


		// 
		// converts to -1 or 1
		// NaN's and Infs get converted as well
		[MethodImpl(MethodImplOptions.AggressiveInlining)]

		public static float SignOr1_2(float f)
		{
			FloatIntUnion i;
			i.intValue = 0;
			i.floatValue = f;
			i.intValue = (i.intValue & 0x80000000u) | 0x3f800000;
			return i.floatValue;
		}

		/// <summary>
		/// Test:  Same as above but with pointers
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe float SignOr1_3(float f)
		{
			float* ptr = &f;
			*(uint*)ptr = (*(uint*)ptr & 0x80000000u) | 0x3f800000;
			return f;

		}

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		/////
		///// Degrees
		///// 
		//public static float WrapAngleD(this float angle)
		//{
		//	return angle - math.round(angle * (1f / 360f)) * 360f;
		//}
		//public static Vector3 WrapAnglesD(this Vector3 angle)
		//{
		//	return new Vector3(WrapAngleD(angle.x), WrapAngleD(angle.y), WrapAngleD(angle.z));
		//}
		//public static Vector3 WrapAngles(this Vector3 angle)
		//{
		//	return new Vector3(WrapAngle(angle.x), WrapAngle(angle.y), WrapAngle(angle.z));
		//}
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		/////
		///// Degrees
		///// 
		//public static float Abs(this float v)
		//{
		//	return v >= 0f ? v : -v;
		//}
		///// <summary>
		///// radians
		///// </summary>
		///// <param name="delta"></param>
		///// <returns></returns>
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static float WrapAngle(this float angle)
		//{
		//	return angle - math.round(angle * (1f / (twoPI))) * (twoPI);
		//}

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static float DeltaAngle(float theta0, float theta1)
		//{
		//	return WrapAngleD(theta1 - theta0);
		//}
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static float Saturate(this float t)
		//{
		//	return math.saturate(t);

		//}

		internal static float Wrap0(this float v) // changes when negatve, but what can you do.
		{
			return v - (int)v;
		}
		//internal static float2 Wrap0(this float2 v) // changes when negatve, but what can you do.
		//{
		//	return new float2(v.x - (int)v.x, v.y - (int)v.y);
		//}
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static float Clamp(this float value, float min, float max)
		//{
		//	Assert.IsTrue(max >= min);
		//	if (value <= min)
		//		return min;
		//	if (value >= max)
		//		return max;
		//	return value;
		//}
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static int Clamp(this int value, int min, int max)
		//{
		//	Assert.IsTrue(max >= min);
		//	if (value <= min)
		//		return min;
		//	if (value >= max)
		//		return max;
		//	return value;
		//}


		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static sbyte Clamp(this sbyte value, sbyte min, sbyte max)
		//{
		//	Assert.IsTrue(max >= min);
		//	if (value <= min)
		//		return min;
		//	if (value >= max)
		//		return max;
		//	return value;
		//}


		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//internal static float ClampMag(this float a, float mag)
		//{
		//	Assert.IsTrue(mag >= 0.0f);
		//	if (a <= -mag)
		//		return -mag;
		//	if (a >= mag)
		//		return mag;
		//	return a;
		//}

	}
}