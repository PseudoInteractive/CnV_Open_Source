using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Models
{
    public class CityG
    {
        public string name { get; set; }
        public int cityId;
        public string xy => $"{cityId/256}:{cityId%255}";
    }
}
