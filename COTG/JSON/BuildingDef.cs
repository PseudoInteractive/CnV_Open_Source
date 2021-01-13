using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static COTG.Debug;
namespace COTG.JSON
{
	using J = System.Text.Json.Serialization.JsonPropertyNameAttribute;
	

	public partial class BuildingDef
	{
		public static byte BidToId(int bid)
		{
			if (bid == 0)
				return 0;
			if (all.TryGetValue(bid, out var def))
				return def.id;
			Log("Bad building! " + bid);
			return 0;
		}
		static BuildingDef()
		{
			all= Json.FromResources<Dictionary<int, BuildingDef>>("buildingDef");
			byte counter = 0;
			idToBid = new Dictionary<byte, BuildingDef>();
			foreach (var i in all)
			{
				var b = all[i.Key];
				b.bid = i.Key;
				b.id = counter;
				idToBid.Add(counter, i.Value);
				++counter;
			}
		}
		public static BuildingDef FromId(byte id) => idToBid[id];
		public static Dictionary<byte, BuildingDef> idToBid;
		public static Dictionary<int, BuildingDef> all;

			public byte id; // packed id
			public int bid; // building id
			[J("bn")] public string Bn { get; set; }
			[J("proto")] public int Proto { get; set; }
			[J("ds")] public string Ds { get; set; }
			[J("bimg")] public string Bimg { get; set; }
			[J("dimg")] public string Dimg { get; set; }
			[J("climg")] public string Climg { get; set; }
			[J("limg")] public string Limg { get; set; }
			[J("simg")] public string Simg { get; set; }
			[J("tooltip")] public string Tooltip { get; set; }
			[J("rt")] public int Rt { get; set; }
			[J("tc")] public Dictionary<string, int> Tc { get; set; }
			[J("ts")] public Dictionary<string, int> Ts { get; set; }
			[J("mt")] public int Mt { get; set; }
			[J("sc")] public Dictionary<string, int> Sc { get; set; }
			[J("bc")] public Dictionary<string, Bc> Bc { get; set; }
			[J("thl")] public int Thl { get; set; }
			[J("cs")] public Dictionary<string, int> Cs { get; set; }
			[J("eff")] public Dictionary<string, int> Eff { get; set; }
			[J("r")] public Dictionary<string, int> R { get; set; }
			[J("trn")] public Dictionary<string, int> Trn { get; set; }
			[J("st")] public Dictionary<string, int> St { get; set; }
			[J("stt")] public int Stt { get; set; }
			[J("efft")] public int Efft { get; set; }
			[J("tt")] public int Tt { get; set; }
			[J("tb")] public Dictionary<string, int> Tb { get; set; }
			[J("trcap")] public Dictionary<string, int> Trcap { get; set; }
			[J("warnt")] public Dictionary<string, int> Warnt { get; set; }
			[J("trrep")] public Dictionary<string, int> Trrep { get; set; }
			[J("defbo")] public Dictionary<string, int> Defbo { get; set; }
			[J("faith")] public Dictionary<string, int> Faith { get; set; }
		}

		public partial struct Bc
		{
			[J("r_s")] public int RS { get; set; }
			[J("r_w")] public int RW { get; set; }
			[J("tu")] public long Tu { get; set; }
			[J("td")] public long Td { get; set; }
		}
	}
