using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Game
{
    //public struct Attack
    //{
    //    public int defCid;
    //    public int atkCid;
    //    public DateTimeOffset time;
    //    public DateTimeOffset spotted;



    //    public static Attack[] attacks;

    //    public override string ToString()
    //    {
    //        return $"{defCid.ToCoordinate()}<={atkCid.ToCoordinate()} eta:{time.ToString()} spot:{spotted.ToString()}";
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        return obj is Attack attack &&
    //               defCid == attack.defCid &&
    //               atkCid == attack.atkCid &&
    //               time == attack.time;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return HashCode.Combine(defCid, atkCid, (int)(time.Ticks>>20) );
    //    }

    //    public static bool operator ==(Attack left, Attack right)
    //    {
    //        return left.Equals(right);
    //    }

    //    public static bool operator !=(Attack left, Attack right)
    //    {
    //        return !(left == right);
    //    }
    //}

}
