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
            return  (int)(((DateTime.Now.Ticks/10000)%1000000));
        }
        public static string MSS()
        {
            return  $"{MS():06d}";
        }

    }
}
