using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Helpers
{
    public static class Tick
    {
       
        public static int MS()
        {
            return  (int)(((Environment.TickCount)%100000));
        }
        public static string MSS()
        {
            return  $"{MS(),6}";
        }

    }
}
