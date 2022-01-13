using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using J = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using System.Text.Json.Serialization;

namespace CnV.GameData
{
    public class gFRep2
    {
        public partial class Root
        {
            [J("acoord")] public object Acoord { get; set; }
            [J("rid")] public string Rid { get; set; }
            [J("issum")] public long Issum { get; set; }
            [J("tn")] public string Tn { get; set; }
            [J("ftime")] public string Ftime { get; set; }
            [J("reptype")] public long? Reptype { get; set; }
            [J("tpn")] public string Tpn { get; set; }
            [J("tcid")] public long? Tcid { get; set; }
            [J("tpan")] public string Tpan { get; set; }
            [J("tcn")] public string Tcn { get; set; }
            [J("tcx")] public Acx Tcx { get; set; }
            [J("tcy")] public Acx Tcy { get; set; }
            [J("npro")] public long Npro { get; set; }
            [J("moon")] public long Moon { get; set; }
            [J("moral")] public long Moral { get; set; }
            [J("apn")] public string Apn { get; set; }
            [J("apan")] public string Apan { get; set; }
            [J("acn")] public string Acn { get; set; }
            [J("acx")] public Acx Acx { get; set; }
            [J("acy")] public Acx Acy { get; set; }
            [J("asco")] public long? Asco { get; set; }
            [J("acits")] public long? Acits { get; set; }
            [J("dsco")] public long? Dsco { get; set; }
            [J("dcits")] public long? Dcits { get; set; }
            [J("ahair")] public string Ahair { get; set; }
            [J("acloths")] public string Acloths { get; set; }
            [J("adress")] public string Adress { get; set; }
            [J("askin")] public string Askin { get; set; }
            [J("abody")] public string Abody { get; set; }
            [J("dhair")] public string Dhair { get; set; }
            [J("dcloths")] public string Dcloths { get; set; }
            [J("ddress")] public string Ddress { get; set; }
            [J("dskin")] public string Dskin { get; set; }
            [J("dbody")] public string Dbody { get; set; }
            [J("assault")] public Assault Assault { get; set; }
            [J("loot")] public Apuri Loot { get; set; }
            [J("rew")] public Rew Rew { get; set; }
            [J("senator")] public long? Senator { get; set; }
            [J("type")] public long? Type { get; set; }
            [J("stype")] public string Stype { get; set; }
            [J("neutr")] public long? Neutr { get; set; }
            [J("held")] public long? Held { get; set; }
            [J("dpuri")] public Apuri Dpuri { get; set; }
            [J("apuri")] public Apuri Apuri { get; set; }
            [J("atsb")] public FgrepAtle Atsb { get; set; }
            [J("ttsb")] public long[] Ttsb { get; set; }
            [J("atlo")] public FgrepAtle Atlo { get; set; }
            [J("atle")] public FgrepAtle Atle { get; set; }
            [J("ats")] public FgrepAtle Ats { get; set; }
            [J("ttlo")] public long[] Ttlo { get; set; }
            [J("ttle")] public long[] Ttle { get; set; }
            [J("tts")] public long[] Tts { get; set; }
            [J("pfame")] public long? Pfame { get; set; }
            [J("puri")] public Apuri Puri { get; set; }
            [J("dunitf")] public long? Dunitf { get; set; }
            [J("dfame")] public long? Dfame { get; set; }
            [J("ofame")] public long? Ofame { get; set; }
            [J("ptype")] public long? Ptype { get; set; }
            [J("bd")] public Dictionary<string, long> Bd { get; set; }
            [J("r")] public Dictionary<string, double> R { get; set; }
            [J("t")] public string T { get; set; }
        }

        public partial class Apuri
        {
            [J("w")] public long W { get; set; }
            [J("s")] public long S { get; set; }
            [J("i")] public long I { get; set; }
            [J("f")] public long F { get; set; }
        }

        public partial class Assault
        {
            [J("cities")] public long Cities { get; set; }
            [J("ts_sent")] public long TsSent { get; set; }
            [J("ts_lost")] public long TsLost { get; set; }
            [J("ts_killed")] public long TsKilled { get; set; }
            [J("reports")] public Report[] Reports { get; set; }
        }

        public partial class Report
        {
            [J("ats")] public ReportAtle Ats { get; set; }
            [J("atsb")] public ReportAtle Atsb { get; set; }
            [J("atle")] public ReportAtle Atle { get; set; }
            [J("atlo")] public ReportAtle Atlo { get; set; }
            [J("tts")] public long[] Tts { get; set; }
            [J("ttsb")] public long[] Ttsb { get; set; }
            [J("ttle")] public long[] Ttle { get; set; }
            [J("ttlo")] public long[] Ttlo { get; set; }
            [J("apn")] public string Apn { get; set; }
            [J("acn")] public string Acn { get; set; }
            [J("acid")] public long Acid { get; set; }
            [J("neutr")] public object[] Neutr { get; set; }
            [J("held")] public Dictionary<string, long> Held { get; set; }
            [J("ts_sent")] public long TsSent { get; set; }
            [J("ts_left")] public long TsLeft { get; set; }
            [J("bdmg")] public long Bdmg { get; set; }
            [J("loot")] public Apuri Loot { get; set; }
            [J("rew")] public Rew Rew { get; set; }
            [J("senator")] public long Senator { get; set; }
            [J("scout")] public long Scout { get; set; }
            [J("temple")] public long temple { get; set; }
        }

        public partial class ReportAtle
        {
            [J("5")] public long The5 { get; set; }
        }
        public partial struct Acx
        {
            public long? Integer;
            public string String;

            public static implicit operator Acx(long Integer) => new Acx { Integer = Integer };
            public static implicit operator Acx(string String) => new Acx { String = String };
        }

        public partial class Rew
        {
            [J("w")] public long W { get; set; }
            [J("s")] public long S { get; set; }
            [J("i")] public long I { get; set; }
            [J("f")] public long F { get; set; }
            [J("of")] public long Of { get; set; }
            [J("df")] public long Df { get; set; }
            [J("ud")] public long Ud { get; set; }
            [J("rp")] public double Rp { get; set; }
        }

        public partial class FgrepAtle
        {
            [J("7")] public long The7 { get; set; }
        }

    }
}
/*
[
{
	"acoord": null,
	"rid": "ffa899c41835219e",
	"issum": 1,
	"tn": "Attack Summary (269:157)",
	"ftime": "22:00:00 30\/06",
	"reptype": 1,
	"tpn": "manticorus",
	"tcid": 10289421,
	"tpan": "Horizon",
	"tcn": "INFECTED 501",
	"tcx": "-",
	"tcy": "-",
	"npro": 0,
	"moon": 0,
	"moral": 0,
	"apn": "Greywolf2020",
	"apan": "Phoenix",
	"acn": "-",
	"acx": "-",
	"acy": "-",
	"asco": 672935,
	"acits": 88,
	"dsco": 193829,
	"dcits": 29,
	"ahair": "avM1h0",
	"acloths": "avM1c0",
	"adress": "avM1d0",
	"askin": "avM1s0",
	"abody": "avM1",
	"dhair": "avM2h4",
	"dcloths": "avM2c1",
	"ddress": "avM2d0",
	"dskin": "avM2s1",
	"dbody": "avM2",
	"assault": {
		"cities": 1,
		"ts_sent": 94594,
		"ts_lost": 23121,
		"ts_killed": 25574,
		"reports": [
			{
				"ats": {
					"5": 94594
				},
				"atsb": {
					"5": 35
				},
				"atle": {
					"5": 71473
				},
				"atlo": {
					"5": 23121
				},
				"tts": [
					0,
					0,
					10000,
					10000,
					10448,
					0,
					1221,
					4091,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					2
				],
				"ttsb": [
					6,
					6,
					15,
					10,
					6,
					10,
					10,
					20,
					0,
					0,
					0,
					6,
					0,
					6,
					6,
					6,
					0,
					90
				],
				"ttle": [
					0,
					0,
					2849,
					2849,
					2976,
					0,
					348,
					1165,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					1
				],
				"ttlo": [
					0,
					0,
					7151,
					7151,
					7472,
					0,
					873,
					2926,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					1
				],
				"apn": "Greywolf2020",
				"acn": "C12 Yes(267:158)",
				"acid": 10354955,
				"neutr": [],
				"held": {
					"2": 10000,
					"3": 10000,
					"7": 250
				},
				"ts_sent": 94594,
				"ts_left": 71473,
				"bdmg": 0,
				"loot": {
					"w": 112316,
					"s": 200940,
					"i": 0,
					"f": 90026
				},
				"rew": {
					"w": 1160,
					"s": 1160,
					"i": 1160,
					"f": 1160,
					"of": 4644,
					"df": 0,
					"ud": 25574,
					"rp": 403282.61273148
				},
				"senator": 0,
				"scout": 0,
				"temple": 0
			}
		]
	},
	"loot": {
		"w": 112316,
		"s": 200940,
		"i": 0,
		"f": 90026
	},
	"rew": {
		"w": 1160,
		"s": 1160,
		"i": 1160,
		"f": 1160,
		"of": 4644,
		"df": 0,
		"ud": 25574,
		"rp": 403282
	},
	"senator": 0
}
,
 {
	"issum": 0,
	"type": 3,
	"stype": "b",
	"rid": "05f181095798bc3d",
	"tpn": "shadow1",
	"tcn": "Destiny 09",
	"tcx": 344,
	"tcy": 151,
	"tpan": "Phoenix",
	"apn": "BKSpecial",
	"acn": "13_006",
	"acx": 340,
	"acy": 149,
	"apan": "Horizon",
	"neutr": 0,
	"held": 0,
	"dpuri": {
		"w": 0,
		"s": 0,
		"i": 0,
		"f": 0
	},
	"apuri": {
		"w": 0,
		"s": 0,
		"i": 0,
		"f": 0
	},
	"npro": 40,
	"moon": 0,
	"moral": 50,
	"atsb": {
		"7": 50
	},
	"ttsb": [
		2,
		2,
		10,
		0,
		0,
		3,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		2,
		2,
		0,
		0
	],
	"atlo": {
		"7": 0
	},
	"atle": {
		"7": 500
	},
	"ats": {
		"7": 500
	},
	"ttlo": [
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0
	],
	"ttle": [
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0
	],
	"tts": [
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0
	],
	"pfame": 0,
	"tn": "Sby BKSpecial",
	"puri": {
		"w": 0,
		"s": 0,
		"i": 0,
		"f": 0
	},
	"dunitf": 0,
	"dfame": 0,
	"ofame": 0,
	"ptype": 0,
	"bd": {
		"809": 1,
		"446": 100,
		"448": 171,
		"461": 180,
		"455": 7
	},
	"r": {
		"1": 6788.615961111,
		"2": 44263.544444444,
		"3": 2000,
		"4": 2000
	},
	"t": "04:10:31 01\/07"
}]

 */
