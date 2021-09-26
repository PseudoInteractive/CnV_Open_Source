﻿using COTG.Game;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Helpers
{
    public struct Span2
    {
        public Vector2 c0;
        public Vector2 c1;
        public void Clear()
        {
            c0 = new Vector2(float.MaxValue);
            c1 = new Vector2(-float.MaxValue);
        }
        public Span2(Vector2 v)
        {
            c0.X = c1.X = v.X;
            c0.Y = c1.Y = v.Y;
        }
        public Span2(IEnumerable<AttackPlanCity> spots)
        {
            c0 = new Vector2(float.MaxValue);
            c1 = new Vector2(-float.MaxValue);
           foreach(var spot in spots)
            {
                Union(spot.cid.ToWorldC());
            }
        }

        public static Span2 UnionWithout(IEnumerable<AttackPlanCity>  spots, AttackPlanCity exclude )
        {
            Span2 rv;
            rv.c0 = new Vector2(float.MaxValue);
            rv.c1 = new Vector2(-float.MaxValue);
			foreach (var s in spots)
			{
			
                if(s != exclude)
                   rv.Union(s.cid.ToWorldC());
            }
            return rv;
        }
        public bool Contains(Vector2 v)
        {
            return c0.X <= v.X & c0.Y <= v.Y &
                c1.X >= v.X & c1.Y >= v.Y;
        }
        public void Union(Vector2 v)
        {
            c0.X = c0.X.Min(v.X);
            c0.Y = c0.Y.Min(v.Y);
            c1.X = c1.X.Max(v.X);
            c1.Y = c1.Y.Max(v.Y);
        }
        public static Span2 operator + (Span2 a, Vector2 v)
        {
            return new Span2() {
                c0=new Vector2(a.c0.X.Min(v.X), a.c0.Y.Min(v.Y)),
                c1=new Vector2(a.c1.X.Max(v.X), a.c1.Y.Max(v.Y))
            };
        }
        // Approximate signed distance
        // If the point is inside this returns -depth
        // If outside it returns greated of distance in X and distance in Y.
        public float Distance(Vector2 v)
        {
            return Distance2(v).Sqrt();
	    }
        public float Distance2(Vector2 v)
        {
            var dx = ((v.X-c1.X).Max(c0.X-v.X));
            var dy = ((v.Y-c1.Y).Max(c0.Y-v.Y));
            if ((dx<0)|(dy<0))
                return (dx.Max(dy)).Squared();
            else
                return dx.Squared() + dy.Squared();
        }

        //  Bounding circle radius
        public float radius => (c1.X-c0.X).Max(c1.Y-c0.Y);
        // Ellipsoid like squared radius
        public float radius2 => (c1.X-c0.X).Squared()+ (c1.Y-c0.Y).Squared();

		public float area => (c1.X - c0.X) * (c1.Y - c0.Y);
	}
	public struct Span2i
	{
		// [c0..c1)  c1 itself is not included
		public (int X, int Y) c0;
		public (int X, int Y) c1;

		public Span2i( (int x, int y) c0, (int x, int y) c1)
		{
			this.c0 = (c0.x.Min(c1.x), c0.y.Min(c1.y));
			this.c1 = (c0.x.Max(c1.x), c0.y.Max(c1.y));
		}
		public Span2i(int c0x, int c0y, int c1x, int c1y)
		{
			this.c0 = (c0x,c0y);
			this.c1 = (c1x, c1y);
		}
		public void Clear()
		{
			c0 = (int.MaxValue,int.MaxValue);
			c1 = (int.MinValue, int.MinValue);
		}
		public bool Any() => c1.X >= c0.X & c1.Y >= c0.Y;
		public bool Contains( (int X, int Y) v)
		{
			return c0.X <= v.X & c0.Y <= v.Y &
				c1.X > v.X & c1.Y > v.Y;
		}
		public bool Contains(Vector2 v)
		{
			return c0.X <= v.X & c0.Y <= v.Y &
				c1.X > v.X & c1.Y > v.Y;
		}
		public bool Overlaps((int X, int Y) v, int r)
		{
			return c0.X-r <= v.X & c0.Y-r <= v.Y &
				c1.X+r > v.X & c1.Y+r > v.Y;
		}
		public bool Overlaps(Span2i s)
		{
			return c0.X < s.c1.X &
					c1.X > s.c0.X &
				   c0.Y < s.c1.Y &
					c1.Y > s.c0.Y;

		}
		public static Span2i operator + (Span2i a, (int X, int Y) v)
		{
			return new Span2i(
				a.c0.X.Min(v.X),
				a.c0.Y.Min(v.Y),
				a.c1.X.Max(v.X),
				a.c1.Y.Max(v.Y));
		}
		public (int X, int Y) Mid => ((c0.X+c1.X)/2,(c0.Y+c1.Y)/2);
		
		public Vector2 MidV => new((c0.X+c1.X)*0.5f, (c0.Y+ c1.Y)*0.5f);
		

	}
}
