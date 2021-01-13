using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.JSON
{
	using J = System.Text.Json.Serialization.JsonPropertyNameAttribute;
	

	public partial struct BuildingDef
	{
		public static BuildingDef[] all = Json.FromResources< BuildingDef[]>("buildingDef");

			[J("bn")] public string Bn { get; set; }
			[J("proto")] public long Proto { get; set; }
			[J("ds")] public string Ds { get; set; }
			[J("bimg")] public string Bimg { get; set; }
			[J("dimg")] public string Dimg { get; set; }
			[J("climg")] public string Climg { get; set; }
			[J("limg")] public string Limg { get; set; }
			[J("simg")] public string Simg { get; set; }
			[J("tooltip")] public string Tooltip { get; set; }
			[J("rt")] public long Rt { get; set; }
			[J("tc")] public Dictionary<string, long> Tc { get; set; }
			[J("ts")] public Dictionary<string, long> Ts { get; set; }
			[J("mt")] public long Mt { get; set; }
			[J("sc")] public Dictionary<string, long> Sc { get; set; }
			[J("bc")] public Dictionary<string, Bc> Bc { get; set; }
			[J("thl")] public long Thl { get; set; }
			[J("cs")] public Dictionary<string, long> Cs { get; set; }
			[J("eff")] public Dictionary<string, long> Eff { get; set; }
			[J("r")] public Dictionary<string, long> R { get; set; }
			[J("trn")] public Dictionary<string, long> Trn { get; set; }
			[J("st")] public Dictionary<string, long> St { get; set; }
			[J("stt")] public long Stt { get; set; }
			[J("efft")] public long Efft { get; set; }
			[J("tt")] public long Tt { get; set; }
			[J("tb")] public Dictionary<string, long> Tb { get; set; }
			[J("trcap")] public Dictionary<string, long> Trcap { get; set; }
			[J("warnt")] public Dictionary<string, long> Warnt { get; set; }
			[J("trrep")] public Dictionary<string, long> Trrep { get; set; }
			[J("defbo")] public Dictionary<string, long> Defbo { get; set; }
			[J("faith")] public Dictionary<string, long> Faith { get; set; }
		}

		public partial struct Bc
		{
			[J("r_s")] public long RS { get; set; }
			[J("r_w")] public long RW { get; set; }
			[J("tu")] public long Tu { get; set; }
			[J("td")] public long Td { get; set; }
		}
	}
