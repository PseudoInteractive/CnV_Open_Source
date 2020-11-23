using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Game
{

    //"trintr"
    public class Reinforcement
    {
        public int sourceCid;
        public int targetCid;

        public long order;
        public TroopTypeCount[] troops = TroopTypeCount.empty;
    }
}
