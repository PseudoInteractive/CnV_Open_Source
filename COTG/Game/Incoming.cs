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
        public DateTime time;
        public DateTime spotted;



        public static Attack[] attacks;

        public override string ToString()
        {
            return $"{targetCid.ToCoordinate()}<={sourceCid.ToCoordinate()} eta:{time.ToString()} spot:{spotted.ToString()}";
        }
    }

}
