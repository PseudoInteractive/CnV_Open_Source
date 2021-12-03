using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV
{
	using System.Globalization;
	using System.Text.Json;

	public static class XNAUtil
	{
		public static Color HalfSaturation(this Color c)
		{
			return new Color((byte) (128 + c.R / 2), (byte) (128 + c.G / 2), (byte) (128 + c.B / 2), c.A);
		}

		public static Color ThreeQuarterSaturation(this Color c)
		{
			return new Color((byte) (64 + c.R * 3 / 4), (byte) (64 + c.G * 3 / 4), (byte) (64 + c.B * 3 / 4), c.A);
		}


		// Lists
		public static Color Lerp(this float t, Color c0, Color c1)
		{
			return new Color(

				(byte) t.Lerp(c0.R, c1.R).RoundToInt(),
				(byte) t.Lerp(c0.G, c1.G).RoundToInt(),
				(byte) t.Lerp(c0.B, c1.B).RoundToInt(),
				(byte) t.Lerp(c0.A, c1.A).RoundToInt());
		}

		public static Color LerpGamma(this float t, Color c0, Color c1)
		{
			return new Color(

				(byte) t.LerpSqrt(c0.R, c1.R).RoundToInt(),
				(byte) t.LerpSqrt(c0.G, c1.G).RoundToInt(),
				(byte) t.LerpSqrt(c0.B, c1.B).RoundToInt(),
				(byte) t.LerpSqrt(c0.A, c1.A).RoundToInt());
		}
		public static unsafe Microsoft.Xna.Framework.Color GetColor(this JsonElement js, string prop)
		{
			if(!js.TryGetProperty(prop, out var e))
			{
				Log("Missing " + prop);
				return new Microsoft.Xna.Framework.Color();
			}

			var s = e.GetString();
			int get = 0;
			if(s[get] == '#')
				++get;
			byte* rgba = stackalloc byte[4];
			rgba[3] = 255;
			for(int i = 0; i<4; ++i)
			{
				if(get >= s.Length)
					break;
				var str = s[get + 0].ToString() + s[get + 1].ToString();
				get += 2;
				rgba[i] = (byte)int.Parse(str, NumberStyles.HexNumber);
			}
			return new Microsoft.Xna.Framework.Color(rgba[0], rgba[1], rgba[2], rgba[3]);
		}
	}
	public static class HSLToRGB
    {

        public static Microsoft.Xna.Framework.Color ToRGBA(float h, float sl, float l, float alpha =1,float gain=1)

        {

            float v;

            float r, g, b;



            r = l;   // default to gray

            g = l;

            b = l;

            v = (l <= 0.5f) ? (l * (1.0f + sl)) : (l + sl - l * sl);

            if (v > 0)

            {

                float m;

                float sv;

                int sextant;

                float fract, vsf, mid1, mid2;



                m = l + l - v;

                sv = (v - m) / v;

                h *= 6.0f;

                sextant = (int)h;

                fract = h - sextant;

                vsf = v * sv * fract;

                mid1 = m + vsf;

                mid2 = v - vsf;

                switch (sextant)

                {

                    case 0:

                        r = v;

                        g = mid1;

                        b = m;

                        break;

                    case 1:

                        r = mid2;

                        g = v;

                        b = m;

                        break;

                    case 2:

                        r = m;

                        g = v;

                        b = mid1;

                        break;

                    case 3:

                        r = m;

                        g = mid2;

                        b = v;

                        break;

                    case 4:

                        r = mid1;

                        g = m;

                        b = v;

                        break;

                    case 5:

                        r = v;

                        g = m;

                        b = mid2;

                        break;

                }

            }

            return new Microsoft.Xna.Framework.Color(r*gain, g*gain,b*gain, alpha );

        }

        //Color ToColor()
        //{
        //    R = (byte)(r * 255.0f).RoundToInt(),
        //        G = (byte)(g * 255.0f).RoundToInt(),
        //        B = (byte)(b * 255.0f).RoundToInt(),
        //        A = 255,


        //}

        // Given a Color (RGB Struct) in range of 0-255

        // Return H,S,L in range of 0-1

        public static void ToHSV(Microsoft.Xna.Framework.Color rgb, out float h, out float s, out float l)

        {

            float r = rgb.R / 255.0f;

            float g = rgb.G / 255.0f;

            float b = rgb.B / 255.0f;

            float v;

            float m;

            float vm;

            float r2, g2, b2;



            h = 0; // default to black

            s = 0;

            l = 0;

            v = Math.Max(r, g);

            v = Math.Max(v, b);

            m = Math.Min(r, g);

            m = Math.Min(m, b);

            l = (m + v) * 0.5f;

            if (l <= 0.0)

            {

                return;

            }

            vm = v - m;

            s = vm;

            if (s > 0.0)

            {

                s /= (l <= 0.5) ? (v + m) : (2.0f - v - m);

            }

            else

            {

                return;

            }

            r2 = (v - r) / vm;

            g2 = (v - g) / vm;

            b2 = (v - b) / vm;

            if (r == v)

            {

                h = (g == m ? 5.0f + b2 : 1.0f - g2);

            }

            else if (g == v)

            {

                h = (b == m ? 1.0f + r2 : 3.0f - b2);

            }

            else

            {

                h = (r == m ? 3.0f + g2 : 5.0f - r2);

            }

            h /= 6.0f;

        }
    }
}
