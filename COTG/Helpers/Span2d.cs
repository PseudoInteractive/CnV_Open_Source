using COTG.Game;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Helpers
{
    struct Span2
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
        public Span2(IEnumerator<Spot> spots)
        {
            c0 = new Vector2(float.MaxValue);
            c1 = new Vector2(-float.MaxValue);
            while(spots.MoveNext())
            {
                Union(spots.Current.cid.ToWorldC());
            }
        }
        public static Span2 UnionWithout(IEnumerator<Spot>  spots,Spot exclude )
        {
            Span2 rv;
            rv.c0 = new Vector2(float.MaxValue);
            rv.c1 = new Vector2(-float.MaxValue);
            while (spots.MoveNext())
            {
                var s = spots.Current;
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
            return ((v.X-c1.X).Max(v.Y-c1.Y)).Max((c0.X-v.X).Max(c0.Y-v.Y));
        }
        public float Distance2(Vector2 v)
        {
            var dx = ((v.X-c1.X).Max(v.Y-c1.Y));
            var dy = ((c0.X-v.X).Max(c0.Y-v.Y));
            if ((dx<0)|(dy<0))
                return dx.Max(dy);
            else
                return dx.Squared() + dy.Squared();
        }

        //  Bounding circle radius
        public float radius => (c1.X-c0.X).Max(c1.Y-c1.Y);
        // Ellipsoid like squared radius
        public float radius2 => (c1.X-c0.X).Squared()+ (c1.Y-c1.Y).Squared();
    }

}
