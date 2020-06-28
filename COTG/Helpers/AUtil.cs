using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG
{
    public static class AUtil
    {
        // Lists
        public static void AddIfAbsent<T>(this List<T> l, T a) where T: IEquatable<T>
        {
            foreach(var i in l)
            {
                if (i.Equals( a) )
                    return;
            }
            l.Add(a);
        }
    }
}
