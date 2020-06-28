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
        public static float Min(this float f,  float max)
        {
            return (f <= max ? f : max);
        }
        public static float Max(this float f, float min)
        {
            return (f >= min ? f : min);
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
    }
}
