using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Game
{
    public class Attack
    {
        int player { get; set; }
        int target { get; set; }
        public enum Type
        {
            assult,
            senator,
            se, 
        }
        public Type type { get; set; }
        public bool fake { get; set; }
    }
}
