using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.JSON
{
	public class BuildQueueItem
	{
		public long ds;
		public long de;
		public long btime;
		public int bid;
		public ushort btype;
		public ushort bspot;
		public ushort brep;
		public byte slvl;
		public byte elvl;
		public byte pa;
/*	"bq": [
            {
                "bid": 83072020,
                "btype": 455,
                "bspot": 220,
                "slvl": 8,
                "elvl": 9,
                "ds": 1610764495448,
                "de": 1610770669591,
                "brep": 455,
                "btime": 6174143,
                "pa": 1

}*/
	}
}
