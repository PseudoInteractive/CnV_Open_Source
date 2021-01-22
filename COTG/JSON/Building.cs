using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace COTG.JSON
{
	public struct Building
	{
		public byte id; // this is pretranslated from the buildingDefs definitions to pack it into a byte
		public byte bl; // building level

		public bool isBuilding => bl > 0;

		public BuildingDef def => BuildingDef.FromId(id);
		//[J("bu")] public long Bu { get; set; }
	//	[J("bd")] public long Bd { get; set; }
	//	[J("rt")] public long Rt { get; set; }
	//	[J("rh")] public long Rh { get; set; }
	//	[J("rbb", NullValueHandling = N.Ignore)] public Dictionary<string, Rbb> Rbb { get; set; }
	//	[J("ruh", NullValueHandling = N.Ignore)] public long? Ruh { get; set; }
	//	[J("rdh", NullValueHandling = N.Ignore)] public long? Rdh { get; set; }
	//	[J("rbt", NullValueHandling = N.Ignore)] public Dictionary<string, Rb> Rbt { get; set; }
	//	[J("rb", NullValueHandling = N.Ignore)] public Rb[] Rb { get; set; }
	//	[J("rtt", NullValueHandling = N.Ignore)] public Rbb[] Rtt { get; set; }
	}

		//public partial class Rb
		//{
		//	[J("b")] public long B { get; set; }
		//	[J("i")] public long I { get; set; }
		//	[J("a")] public long A { get; set; }
		//	[J("bl")] public long Bl { get; set; }
		//	[J("au")] public long Au { get; set; }
		//	[J("ad")] public long Ad { get; set; }
		//	[J("add")] public long Add { get; set; }
		//	[J("bt", NullValueHandling = N.Ignore)] public long? Bt { get; set; }
		//}

		//public partial class Rbb
		//{
		//	[J("rt")] public long Rt { get; set; }
		//	[J("rh")] public long Rh { get; set; }
		//	[J("ruh")] public long Ruh { get; set; }
		//	[J("rdh")] public long Rdh { get; set; }
		//}
	}
