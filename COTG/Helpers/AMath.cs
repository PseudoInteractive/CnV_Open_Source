using COTG.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using static COTG.Debug;
namespace COTG
{
    public static class AMath
    {
        public static Random random = new Random();

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
        public static float Min(this float f,  float max)
        {
            return (f <= max ? f : max);
        }
        public static float Max(this float f, float min)
        {
            return (f >= min ? f : min);
        }
        public static int Max(this int f, int min)
        {
            return (f >= min ? f : min);
        }
        public static int Min(this int f, int max)
        {
            return (f <= max ? f : max);
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
        public static float Lerp(this float t, float c0, float c1)
        {
            return c0 + (c1 - c0) * t;
        }
        public static float Wave(this float t)
        {
            return 0.5f+ 0.5f*MathF.Sin(t * (2 * MathF.PI));
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
        

        public static float Distance(this (int x,int y) a,(int x, int y) b)
        {
            var dx = (a.x - b.x);
            var dy = (a.y - b.y);
            Assert(dx.Abs() < 32767 );
            Assert(dy.Abs() < 32767);
            return MathF.Sqrt( (float)(dx*dx+dy*dy));
        }
        public static float DistanceToCid(this (int x, int y) a, int cid) => Distance(a, cid.CidToWorld());
        public static uint ToCompactCid(this int c)
        {
            var x = c % 65536;
            var y = c >> 16;
            return ToCompactCid((x, y));

        }
        public static uint ToCompactCid(this (int x,int y) c)
        {
            return (uint)c.x | (uint)c.y * 1024u;
        }
        public static (int x,int y) FromCompactCid(this uint c)
        {

            return ( (int)(c&1023u),(int)(c/1024u) );
        }
        static public bool IsNullOrEmpty<T>(this T[] a)
        {
            return a == null || a.Length == 0;
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
        // Randomish hash for city ids
        public static float CidToRandom(this int cid)
        {
            var x = cid % 65536;
            var y = cid / 65536;
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;
            var result = (((int)(x * c1 + y * c2) >> 8)&0xffff);
            return result * (1.0f / 0x10000);

        }
        public static float CidToRandom(this int cid,int offset)
        {
            var x = cid % 65536;
            var y = cid / 65536;
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;
            var result = ((((int)(x * c1 + y * c2) >> 8)+offset ) & 0xffff);
            return result * (1.0f / 0x10000);

        }
        // adds an offset (i.e. time)
        public static float CidToRandom(this int cid, float offset)
        {
            var x = cid % 65536;
            var y = cid / 65536;
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;
            var result = ((((int)(x * c1 + y * c2) >> 8) + (int)(offset*0x10000) ) & 0xffff);
            return result * (1.0f / 0x10000);

        }
    }
}
