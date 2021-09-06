
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using J = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace COTG.JSON
{
	public partial class TroopInfo
	{
		public const int count = 35;
		public static TroopInfo[] all;
		public static async Task Init()
		{
			all = await Json.FromContent<TroopInfo[]>("Troops");
		}
		public int i { get; set; }  // index
			public string tn { get; set; } // name
			public string dsc { get; set; }
			public string dimg { get; set; }
			public string simg { get; set; }
			public string tooltip { get; set; }
			public int fo { get; set; } // food?
			public int at { get; set; } // attack damage type:  &1 damages walls, &2 damages buildings
			public int ut { get; set; } // troop transport
			public int gc { get; set; } // gold cost
			public int dm { get; set; } // defense magic
			public int sc { get; set; } // stone cost
			public int ps { get; set; } // recruit seconds
			public int ic { get; set; } // icon cost
			public int dmg { get; set; } // damage
			public int da { get; set; } // defense art
			public int ts { get; set; } // ts per unit
			public int dc { get; set; } // defence cav
			public int c { get; set; } // carry
			public int di { get; set; } // defense inf
			public int ws { get; set; } // speed
			public int wc { get; set; } // wood cost
			public int admg { get; set; } // building damage
			public int tt { get; set; } // not sure
			public string n { get; set; } // name
			public string dn { get; set; } // display name
		

	}

}
