using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using static COTG.Debug;
namespace COTG
{
    public static class AMmath
    {
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
            return c0 + (c1 - c0) * t;
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

        public static float Distance(this (int x,int y) a,(int x, int y) b)
        {
            var dx = a.x - b.x;
            var dy = a.y - b.y;
            return MathF.Sqrt((a.x - b.x).Squared() + (a.y - b.y).Squared());
        }
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
    }
}
