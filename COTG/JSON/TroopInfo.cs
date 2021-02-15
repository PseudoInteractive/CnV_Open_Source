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
		public static TroopInfo[] all;
		static TroopInfo()
		{
			all = Json.FromResources<TroopInfo[]>("Troops");

		}

		[J("tn")] public string Tn { get; set; }
		[J("dsc")] public string Dsc { get; set; }
		[J("dimg")] public string Dimg { get; set; }
		[J("simg")] public string Simg { get; set; }
		[J("tooltip")] public string Tooltip { get; set; }
		[J("fo")] public long Fo { get; set; }
		[J("at")] public long At { get; set; }
		[J("ut")] public long Ut { get; set; }
		[J("gc")] public long Gc { get; set; }
		[J("dm")] public long Dm { get; set; }
		[J("sc")] public long Sc { get; set; }
		[J("ps")] public long Ps { get; set; }
		[J("ic")] public long Ic { get; set; }
		[J("dmg")] public long Dmg { get; set; }
		[J("da")] public long Da { get; set; }
		[J("ts")] public long Ts { get; set; }
		[J("dc")] public long Dc { get; set; }
		[J("c")] public long C { get; set; }
		[J("di")] public long Di { get; set; }
		[J("ws")] public long Ws { get; set; }
		[J("wc")] public long Wc { get; set; }
		[J("admg")] public long Admg { get; set; }
		[J("tt")] public long Tt { get; set; }
		[J("n")] public string N { get; set; }
		[J("dn")] public string Dn { get; set; }
	}

}
