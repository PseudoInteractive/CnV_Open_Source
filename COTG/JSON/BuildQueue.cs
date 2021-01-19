using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.JSON
{
	public struct BuildQueueItem
	{
		public long ds;
		public long de;
		public long btime;
		public int bidHash; // building id hash, generated for each building in the commnad queue
		public int btype; // This is the "proto" member of buildingDef associated with brep
		public int bspot; // location
		public int brep; // building id type
		public byte slvl;
		public byte elvl;
		public byte pa; // pa=1 is queued normally, pa=0 is not paid
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
