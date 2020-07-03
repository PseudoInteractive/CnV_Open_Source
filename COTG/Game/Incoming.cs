using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Game
{
    public struct Attack
    {
        public int targetCid;
        public int sourceCid;
        public DateTimeOffset time;
        public DateTimeOffset spotted;



        public static Attack[] attacks;

        public override string ToString()
        {
            return $"{targetCid.ToCoordinate()}<={sourceCid.ToCoordinate()} eta:{time.ToString()} spot:{spotted.ToString()}";
        }

        public override bool Equals(object obj)
        {
            return obj is Attack attack &&
                   targetCid == attack.targetCid &&
                   sourceCid == attack.sourceCid &&
                   time == attack.time;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(targetCid, sourceCid, (int)(time.Ticks>>20) );
        }

        public static bool operator ==(Attack left, Attack right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Attack left, Attack right)
        {
            return !(left == right);
        }
    }

}
