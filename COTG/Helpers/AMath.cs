using COTG.Game;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using static COTG.Debug;

using Vector2 = System.Numerics.Vector2;

namespace COTG
{
    public static class AMath
	{
		public static bool IsOneBitSet(ulong l)
		{
			return ((l & (l - 1ul)) == 0);
		}
		// Returns the bit index if one bit is set
		// returns -1 is 0 or 2 or more bits are set
		// there is a faster way to do this?
		public static int GetSetBit(ulong l)
		{
			if (!IsOneBitSet(l))
				return -1;

			for(int i=0; ;++i)
			{

				if ( (l & (1ul << i)) != 0)
					return i;
			}

		}

		public static Random random = new Random();

		public static float Dot(this Vector3 v0, Vector3 v1)
		{
			return Vector3.Dot(v0, v1);
		}
		public static float Dot(this Vector2 v0, Vector2 v1)
		{
			return Vector2.Dot(v0, v1);
		}

		public static uint RotateLeft12(this uint source)
		{
			return source << 12 | (source >> (32 - 12) );
		}
		public static uint RotateRight12(this uint source)
		{
			return source >> 12 | (source << (32 - 12));
		}
		public static bool TryParseInt(this string s, out int o)
		{
			return int.TryParse(s, System.Globalization.NumberStyles.Any, NumberFormatInfo.InvariantInfo, out o);
        }
		public static bool TryParseIntChecked(this string s, out int o)
		{
			
			checked
			{

				try
				{
					var rv = long.TryParse(s, System.Globalization.NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var _o);
					if (rv == false)
					{
						o = 0;
					}
					else
					{
						if (_o > int.MaxValue)
							o = int.MaxValue;
						else if (_o < int.MinValue)
							o = int.MinValue;
						else
							o = (int)_o;

					}
					return rv;
				}
				catch (Exception ex)
				{
					o = 0;
					return false;
				}			
			}
		}

		public static bool TryParseFloat(this string s, out float o)
		{
			return float.TryParse(s, System.Globalization.NumberStyles.Any, NumberFormatInfo.InvariantInfo, out o);
		}
		public static float SmoothStep(this float f, float v0, float v1, float pow=1)
		{
			var df = (f - v0) / (v1 - v0);
			var dfs = MathF.Pow(df, pow);
			var df2 = dfs * dfs;
			if (df <= 0.0f)
				return 0;
			if (df >=  1.0f)
				return 1.0f;
			
			return 3*df2 - 2*df2*dfs;
		}
		public static float SmoothStepOut(this float f, float v0, float v1, float pow = 1)
		{
			return 1 - SmoothStep(f, v0, v1, pow);
		}

		public static Vector2 ToVector(this (float x, float y) me) => new Vector2(me.x, me.y);
		public static (float x, float y) ToTuple(this Vector2 me) => (me.X, me.Y);
		public static Vector3 ToVector(this (float x, float y, float z) me) => new Vector3(me.x, me.y, me.z);
		public static int ScaleInt(this float f, int i)
		{
			return RoundToInt(f * i);
		}

		public static int ScaleInt(this double f, int i)
		{
			return RoundToInt(f * i);
		}

		

		public static int RoundToInt(this float f)
		{
			return (int)MathF.Round(f);
		}
		public static int FloorToInt(this float f)
		{
			return (int)MathF.Floor(f);
		}
		public static int CeilToInt(this float f)
		{
			return (int)MathF.Ceiling(f);
		}

		/// <summary>
		/// Workaournd for missing Span<uint>
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static ulong SumF(this Span<uint> source)
		{
			ulong sum = 0;
				foreach (var v in source)
				{
					sum += v;
				}
			
			return sum;
		}
		public static int CountF(this Span<uint> source, Func<uint,bool> pred )
		{
			int sum = 0;
		
			foreach (var v in source)
			{
				if (pred(v))
					++sum;
			}
			
			return sum;
		}
		public static int CountNonZero(this Span<uint> source)
		{
			int sum = 0;

			foreach (var v in source)
			{
				if (v!=0)
					++sum;
			}

			return sum;
		}
		public static int CountZero(this Span<uint> source)
		{
			int sum = 0;

			foreach (var v in source)
			{
				if (v == 0)
					++sum;
			}

			return sum;
		}
		public static int CountPositive(this Span<int> source)
		{
			int sum = 0;

			foreach (var v in source)
			{
				if (v > 0)
					++sum;
			}

			return sum;
		}
		//
		// Summary:
		//     Computes the average of a sequence of nullable System.Double values.
		//
		// Parameters:
		//   source:
		//     A sequence of nullable System.Double values to calculate the average of.
		//
		// Returns:
		//     The average of the sequence of values, or null if the source sequence is empty
		//     or contains only values that are null.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     source is null.
		public static Vector2 Average(this IEnumerable<Vector2> source)
		{
			var it = source.GetEnumerator();
			if(!it.MoveNext())
			{
				return new Vector2();  // empty;
			}
			var rv = it.Current;
			if (!it.MoveNext()) // 1
				return rv;
			rv += it.Current;
			if (!it.MoveNext())
				return rv *= 0.5f;

			var count = 2;
			do
			{
				rv += it.Current;
				++count;
			} while (it.MoveNext());

			return rv * (1.0f / count);

		}


		public static (int x, int y) RoundToInt(this Vector2 v)
		{
			return (RoundToInt(v.X), RoundToInt(v.Y));
		}
		public static (int x, int y) CeilToInt(this Vector2 v)
		{
			return (CeilToInt(v.X), CeilToInt(v.Y));
		}
		public static (int x, int y) FloorToInt(this Vector2 v)
		{
			return (FloorToInt(v.X), FloorToInt(v.Y));
		}
		public static (int x, int y) Sum(this (int x, int y) a, (int x, int y) b)
		{
			return (a.x + b.x, a.y + b.y);
		}
		public static (int x, int y) Div(this (int x, int y) a, int b)
		{
			return (a.x / b, a.y / b);
		}
	    public static string RandomDigits(int count)
		{
			var rv = new byte[count];
			for(int i =0;i<count;++i)
				rv[i] = (byte)random.Next('0','9');
			return Encoding.ASCII.GetString(rv);
		}
		public static int DivideRoundUp(this int a, int b)
		{
			return (a+b-1)/b;
		}
		public static int DivideRound(this int a, int b)
		{
			return (a + (b)/2 ) / b;
		}
		public static (int x, int y) Mul(this (int x, int y) a, int b)
		{
			return (a.x * b, a.y * b);
		}
		public static (int x, int y) Sub(this (int x, int y) a, (int x, int y) b)
		{
			return (a.x - b.x, a.y - b.y);
		}
		public static float Distance(this (int x, int y) a, (int x, int y) b)
		{
			var dx = (a.x - b.x);
			var dy = (a.y - b.y);
			Assert(dx.Abs() < 32767);
			Assert(dy.Abs() < 32767);
			return MathF.Sqrt((float)(dx * dx + dy * dy));
		}
		public static double DistanceD(this (int x, int y) a, (int x, int y) b)
		{
			var dx = (a.x - b.x);
			var dy = (a.y - b.y);
			Assert(dx.Abs() < 32767);
			Assert(dy.Abs() < 32767);
			return Math.Sqrt((double)(dx * dx + dy * dy));
		}


		public static (int x, int y) Negate(this (int x, int y) a)
		{
			return (-a.x, -a.y);
		}
		public static float Length(this (int x, int y) a)
		{
			var x = ((float)a.x);
			var y = ((float)a.y);
			return MathF.Sqrt(x * x + y * y);

		}

		public static int RoundToInt(this double f)
		{
			return (int)Math.Round(f);
		}
		public static long RoundToLong(this double f)
		{
			return (long)Math.Round(f); 
		}
		public static long RoundToLong(this float f)
		{
			return (long)MathF.Round(f); 
		}

		public static int? RoundToIntOrNAN(this double f)
		{
			return double.IsNaN(f) ? null : (int)Math.Round(f);
		}

		#region Dynamics

		public static float CritDampingKd( float ks)
		{
			return MathF.Sqrt(ks) * 2.0f;
		}

		public static float CritDampingKs( float kd)
		{
			return (kd).Squared() * 0.25f;
		}

		#endregion

		public static int IDivNearest(this int val,int denometer)
        {
            return (val + (denometer >> 1)) / denometer;
        }
        public static int Random(this (int min, int max) range ) => random.Next(range.min, range.max);
        public static float Random(this (float min, float max) range) => ((float)random.NextDouble()).Lerp(range.min,range.max);

        public static float Clamp(this float f, float min, float max)
        {
            Assert(max >= min);
            return f >= min ? (f <= max ? f : max) : min;
        }
        public static int Clamp(this int f, int min, int max)
        {
            Assert(max >= min);
            return f >= min ? (f <= max ? f : max) : min;
        }
		public static long Clamp(this long f, long min, long max)
		{
			Assert(max >= min);
			return f >= min ? (f <= max ? f : max) : min;
		}
		public static ushort ClampToU16(this long v)
		{
			return v < 0 ? (ushort)0 : v > 65535 ? (ushort)65535u : (ushort)v;
		}

		
		public static float Min(this float f,  float max)
        {
            return (f <= max ? f : max);
		}
		public static byte AsByte(this int v)
		{
			Assert(v >= byte.MinValue);
			Assert(v <=byte.MaxValue);
			return (byte)v;
		}
		public static short AsShort(this int v)
		{
			Assert(v >= short.MinValue);
			Assert(v <= short.MaxValue);
			return (short)v;
		}
		public static ushort AsUShort(this int v)
		{
			Assert(v >= ushort.MinValue);
			Assert(v <= ushort.MaxValue);
			return (ushort)v;
		}

		public static float Frac(this float f) => f - MathF.Floor(f);
		public static float Max(this float f, float m)
        {
            return (f >= m ? f : m);
        }
        public static byte Max(this byte f, byte m)
        {
            return (f >= m ? f : m);
        }
        public static float Max0(this float f)
        {
            return (f >= 0 ? f : 0);
        }
        public static int Max0(this int f)
        {
            return (f >= 0 ? f : 0);
        }
        public static int Max(this int f, int m)
        {
            return (f >= m ? f : m);
        }
		public static float Max(this float f, int m)
		{
			return (f >= m ? f : m);
		}
		public static int Min(this int f, int min)
        {
            return (f <=min ? f : min);
        }
		public static float Min(this float f, int min)
		{
			return (f <= min ? f : min);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Min(this float a, float b, float c)
		{
			return Min(Min(a, b), c);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Min(this float a, float b, float c, float d)
		{
			return Min(Min(a, b), Min(c, d));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Min(this float a, float b, float c, float d, float e)
		{
			return Min(e, Min(Min(a, b), Min(c, d)));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Max(this float a, float b, float c)
		{
			return Max(Max(a, b), c);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Max(this float a, float b, float c, float d)
		{
			return Max(Max(a, b), Max(c, d));
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Max(this float a, float b, float c, float d, float e)
		{
			return Max(e, Max(Max(a, b), Max(c, d)));
		}

		public static int Abs(this int f)
        {
            return (f >= 0 ? f : -f);
        }
        public static float Abs(this float f)
        {
            return (f >= 0f ? f : -f);
        }

        public static float Saturate(this float f)
        {
            return (f >= 0.0f ? (f <= 1.0f ? f : 1.0f) : 0.0f);
        }
        public static float ToFloat(this double f)
        {
            return (float)f;
        }
        public static Vector2 Lerp(this float t, Vector2 c0,Vector2 c1)
        {
            var t0 = 1.0f - t;
            return new Vector2(t0*c0.X+t*c1.X, t0 * c0.Y + t * c1.Y);
        }

        public static float SCurve(this float t)
        {
            return t * t * (3 - 2 * t);
        }
        public static float SCurve(this float t, float c0, float c1)
        {
            return c0 + (c1 - c0) * SCurve(t);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Lerp(this float t, float from, float to)
		{
			return from + t * (to - from);
		}

		// signed from -1 .. 1
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float LerpS(this float t, float from, float to)
		{
			return from + (t*0.5f+0.5f) * (to - from);
		}


		public static float Wave(this float t)
        {
            return 0.5f+ 0.5f*MathF.Sin(t * (2 * MathF.PI));
        }
		public static float WaveC(this float t)
		{
			return 0.5f + 0.5f * MathF.Cos(t * (2 * MathF.PI));
		}
		public static float RampSym(this float t) // -0.5 .. 0.5
        {
            return t - MathF.Round(t);
        }
        public static float Ramp(this float t) // 0.0 .. 1.0
        {
            return t - MathF.Floor(t);
        }
        public static float SignOr0(this float t)
        {
            return t > 0 ? 1.0f : t < 0 ? -1.0f : 0;
        }
        public static float SignOr0(this int t)
        {
            return t > 0 ? 1.0f : t < 0 ? -1.0f : 0;
        }
        public static float Squared(this float f)
        {
            return f * f;
        }
        public static float Squared(this int f)
        {
            return (float)f * (float)f;
        }
        public static float Sqrt(this float f)
        {
            return MathF.Sqrt(f);
        }
        // like gamma
        public static float LerpSquared(this float t,float c0, float c1)
        {
            var a0 = (c0).Squared();
            var a1 = (c1).Squared();
            return t.Lerp(a0, a1).Max(0.0f).Sqrt();
        }
        public static float LerpSqrt(this float t, float c0, float c1)
        {
            var a0 = (c0).Max(0.0f).Sqrt();
            var a1 = (c1).Max(0.0f).Sqrt();
            return t.Lerp(a0, a1).Squared();
        }
        public static float Squared(this byte f)
        {
            return (float)f * (float)f;
        }
        

        public static float DistanceToCid(this int a, int cid) => Distance(a.CidToWorld(), cid.CidToWorld());
		public static double DistanceToCidD(this int a, int cid) => DistanceD(a.CidToWorld(), cid.CidToWorld());
		public static bool CanReachByTrade(this int hub, int cid1)
		{
			if (hub.CidToContinent() == cid1.CidToContinent())
			{
				return (City.Get(hub).carts > 0);
			}
			if (!City.Get(cid1).isOnWater)
				return false;

			return (City.Get(hub).ships > 0);
		}
		public static float DistanceToCid(this (int x, int y) a, int cid) => Distance(a, cid.CidToWorld());
		public static double DistanceToCidD(this (int x, int y) a, int cid) => DistanceD(a, cid.CidToWorld());
		public static uint ToCompactCid(this int c)
        {
            var x = c % 65536;
            var y = c >> 16;
            return ToCompactCid((x, y));

        }
        public static uint ToCompactCid(this (int x,int y) c)
        {
            return (uint)c.x | (uint)c.y * World.span;
        }
        public static (int x,int y) FromCompactCid(this uint c)
        {
			var y = c / World.span;

			return ( (int)(c -y* World.span),(int) y );
        }
        static public bool IsNullOrEmpty<T>(this T[] a)
        {
            return a == null || a.Length == 0;
        }
		public static bool Is00(this (int x, int y) c) => (c.x == 0) & (c.y == 0);

		static public int IndexOfClosest(this int[] a,int b)
		{
			var bestScore = long.MaxValue;
			int bestId = -1;
			int counter = -1;
			foreach(var i in a)
			{
				++counter;
				var err = (long)(i - b);
				err = err * err;

				if(err < bestScore)
				{
					bestScore = err;
					bestId = counter;
				}

			}
			return bestId;
		}

		static public int IndexOfClosest(this IEnumerable<float> a, float b)
		{
			var bestScore = float.MaxValue;
			int bestId = -1;
			int counter = -1;
			foreach (var i in a)
			{
				++counter;
				var err = (i - b);
				err = err * err;

				if (err < bestScore)
				{
					bestScore = err;
					bestId = counter;
				}

			}
			return bestId;
		}


		static public bool IsNullOrEmpty<T>(this IEnumerable<T> a)
        {
            if (a == null)
                return true;
            return !a.Any();
        }
        static public bool IsNullOrEmpty<Tkey,T>(this Dictionary<Tkey,T> a)
        {
            return a == null || a.Count == 0;
        }
        static public bool IsNullOrEmpty(this string a)
        {
            return string.IsNullOrEmpty(a);
        }
		public const uint randomPrime0 = 0xcc9e2d51;
		public const int randomPrime1s = 0x1b873593; // signed
		public const uint randomPrime1 = 0x1b873593;
		// Randomish hash for city ids
		public static float CidToRandom(this int cid)
		{
			const uint c1 = randomPrime0;
			const uint c2 = randomPrime1;
			
			var result = (((int)(cid * c1 + cid/11131 * c2) >> 8) & 0xffff);
			return result * (1.0f / 0x10000);
		}
		// not very random at all, but good enough
		public static float BSpotToRandom(this (int x,int y) c )
		{
			var cx = (float)c.x;
			var cy = (float)c.y;
			var r = (c.x.Squared() + c.y.Squared()).Sqrt();
			return (MathF.Atan2(cy,cx))*AMath.inverse2Pi+r*(1.0f/16.0f);

		}
		public static float RandomFloat(uint offset)
		{
			var dRand = (double)(unchecked(offset)); 

			var dotProduct = Math.Cos(dRand) *(256*RandomSeed.GelfondConst) + Math.Sin(dRand) *(256*RandomSeed.GelfondSchneiderConst);
			var remainder = RandomSeed.Numerator % dotProduct;

			return (float)(remainder - Math.Floor(remainder));
		}

		//public static float CidToRandom(this int cid,int offset)
		//{
		//    var x = cid % 65536;
		//    var y = cid / 65536;
		//    const uint c1 = 0xcc9e2d51;
		//    const uint c2 = 0x1b873593;
		//    var result = ((((int)(x * c1 + y * c2) >> 8)+offset ) & 0xffff);
		//    return result * (1.0f / 0x10000);

		//}
		// adds an offset (i.e. time)


		/// <summary>
		/// The <see cref="RandomSeed"/> is a structure for deterministically acquiring random values.
		/// One <see cref="RandomSeed"/> should be able to reproduce the same pseudo-random value for a fixed offset, but
		/// provide enough random distribution for different offsets or different random seeds
		/// Although other methods exist, the current implementation can easily be replicated in the shaders if required
		/// </summary>
		public struct RandomSeed
		{
			public const double GelfondConst = 23.1406926327792690;            // e to the power of Pi = (-1) to the power of -i
			public const double GelfondSchneiderConst = 2.6651441426902251;    // 2 to the power of sqrt(2)
			public const double Numerator = 123456789;

			// When casting uint to double it works fine, but when casting it to float it might cause underflow errors (loss of precision)
			// We want to limit the maximum settable value to prevent such errors.
			private const uint UnderflowGuard = 0xFFFF;

			private readonly uint seed;

			/// <summary>
			/// Initializes a new instance of the <see cref="RandomSeed"/> struct from a target uint.
			/// </summary>
			/// <param name="seed">The seed value to initialize the deterministic random generator.</param>
			public RandomSeed(uint seed)
			{
				this.seed = (seed & UnderflowGuard);
			}

			/// <summary>
			/// Get a deterministic double value between 0 and 1 based on the seed
			/// </summary>
			/// <returns>Deterministic pseudo-random value between 0 and 1</returns>
			public double GetDouble(uint offset)
			{
				var dRand = (double)(unchecked(seed + offset)); // We want it to overflow

				var dotProduct = Math.Cos(dRand) * GelfondConst + Math.Sin(dRand) * GelfondSchneiderConst;
				var denominator = 1e-7 + 256 * dotProduct;
				var remainder = Numerator % denominator;

				return (remainder - Math.Floor(remainder));
			}

			/// <summary>
			/// Get a deterministic float value between 0 and 1 based on the seed
			/// The calculations are still made as doubles to prevent underflow errors.
			/// </summary>
			/// <returns>Deterministic pseudo-random value between 0 and 1</returns>
			public float GetFloat(uint offset) => (float)GetDouble(offset);
		}
		public class ParticleRandomSeedGenerator
		{
			private uint rngSeed;

			public ParticleRandomSeedGenerator(uint seed)
			{
				rngSeed = seed;
			}

			public double GetNextDouble() => GetNextSeed().GetDouble(0);
			public double GetNextFloat() =>  GetNextSeed().GetFloat(0);

			public RandomSeed GetNextSeed()
			{
				return new RandomSeed(unchecked(rngSeed++)); // We want it to overflow
			}
		}
		public static ParticleRandomSeedGenerator randomF = new ParticleRandomSeedGenerator((uint)Environment.TickCount);
		private const float inverse2Pi = 1.0f/(2.0f*MathF.PI);

		public static Vector3 Normalized(this Vector3 me) => Vector3.Normalize(me);
		public static Vector2 Normalized(this Vector2 me) => Vector2.Normalize(me);
		public static Vector4 Normalized(this Vector4 me) => Vector4.Normalize(me);

		/// <summary>
		///     Encode a signed long into an ZigZag unsigned long.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static uint ZigZagEncode(this int value)
			{
				return (uint)((value << 1) ^ (value >> 63));
			}

			/// <summary>
			///     Encode an array of signed longs into a ZigZag encoded array of unsigned longs.
			/// </summary>
			/// <param name="values"></param>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static uint[] ZigZagEncode(this int[] values)
			{
				var output = new uint[values.Length];
				for (var i = 0; i < values.Length; i++)
				{
					output[i] = ZigZagEncode(values[i]);
				}

				return output;
			}

			/// <summary>
			///     Decode a ZigZag unsigned long back into a signed long.
			/// </summary>
			/// <param name="value"></param>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int ZigZagDecode(this uint value)
			{
				return (int)((value >> 1) ^ (~(value & 1) + 1));
			}

			/// <summary>
			/// Decode an array of unsigned longs into a ZigZag encoded array of signed longs.
			/// </summary>
			/// <param name="values"></param>
			/// <returns></returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int[] ZigZagDecode(this uint[] values)
			{
				var output = new int[values.Length];
				for (var i = 0; i < values.Length; i++)
				{
					output[i] = ZigZagDecode(values[i]);
				}

				return output;
			}

		// return distance to line segment, normalized T of closest point
		// t==0 is p1, t==1 is p2
			public static (float distance, float t) DistanceToSegment(this Vector2 pt, Vector2 p0, Vector2 p1)
			{
				float dx = p1.X - p0.X;
				float dy = p1.Y - p0.Y;
				if ((dx == 0) && (dy == 0))
				{
					// It's a point not a line segment.
					dx = pt.X - p0.X;
					dy = pt.Y - p0.Y;
					return (MathF.Sqrt(dx * dx + dy * dy),0f);
				}

				// Calculate the t that minimizes the distance.
				float t = ((pt.X - p0.X) * dx + (pt.Y - p0.Y) * dy) /
					(dx * dx + dy * dy);

				// See if this represents one of the segment's
				// end points or a point in the middle.
				if (t < 0)
				{
		//			closest = new Vector2(p1.X, p1.Y);
					dx = pt.X - p0.X;
					dy = pt.Y - p0.Y;
				}
				else if (t > 1)
				{
					//closest = new Vector2(p2.X, p2.Y);
					dx = pt.X - p1.X;
					dy = pt.Y - p1.Y;
				}
				else
				{
					var closest = new Vector2(p0.X + t * dx, p0.Y + t * dy);
					dx = pt.X - closest.X;
					dy = pt.Y - closest.Y;
				}

				return (MathF.Sqrt(dx * dx + dy * dy),t);
			}

	}

	
}
