﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using COTG;
//
//    var poll2 = Poll2.FromJson(jsonString);

namespace COTG
{
	using System;
	using System.Collections.Generic;

	using System.Globalization;


	using J = System.Text.Json.Serialization.JsonPropertyNameAttribute;

	public partial class Poll2
	{
		[J("lastct")] public long Lastct { get; set; }
		[J("cssrsn")] public long Cssrsn { get; set; }
		[J("cssrs")] public long Cssrs { get; set; }
		[J("player")] public JSPlayer Player { get; set; }
		[J("alliance")] public long Alliance { get; set; }
		[J("server")] public long Server { get; set; }
		[J("mail")] public long Mail { get; set; }
		[J("RI")] public string Ri { get; set; }
		[J("OGT")] public string Ogt { get; set; }
		[J("OGR")] public string Ogr { get; set; }
		[J("OGA")] public string Oga { get; set; }
		[J("ICC")] public string Icc { get; set; }
		[J("iNt")] public Dictionary<string, long> INt { get; set; }
		[J("EL")] public long El { get; set; }
		[J("AL")] public long Al { get; set; }
		[J("OC")] public long Oc { get; set; }
		[J("IC")] public long Ic { get; set; }
		[J("AIC")] public long Aic { get; set; }
		[J("AOC")] public long Aoc { get; set; }
		[J("rep")] public long Rep { get; set; }
		[J("city")] public JSCity City { get; set; }
		[J("cth")] public long Cth { get; set; }
		[J("chh")] public long Chh { get; set; }
		[J("cid")] public long Cid { get; set; }
		[J("resregion")] public long Resregion { get; set; }
	}

	public partial class JSCity
	{
		[J("_id")] public long Id { get; set; }
		[J("cid")] public long Cid { get; set; }
		[J("pn")] public string Pn { get; set; }
		[J("pid")] public long Pid { get; set; }
		[J("pan")] public string Pan { get; set; }
		[J("paid")] public long Paid { get; set; }
		[J("tt")] public long Tt { get; set; }
		[J("tu")] public long Tu { get; set; }
		[J("tc")] public long[] Tc { get; set; }
		[J("th")] public long[] Th { get; set; }
		[J("te")] public long[] Te { get; set; }
		[J("w")]  public long W { get; set; }
		[J("s")] public long S { get; set; }
		[J("x")] public long X { get; set; }
		[J("y")] public long Y { get; set; }
		[J("c")] public long C { get; set; }
		[J("co")] public long Co { get; set; }
		[J("cs")] public long Cs { get; set; }
		[J("tps")] public Tp[] Tps { get; set; }
		[J("r")] public Dictionary<string, CityR> R { get; set; }
		[J("st")] public Dictionary<string, long> St { get; set; }
		[J("rb")] public string Rb { get; set; }
		[J("sb")] public string Sb { get; set; }
		[J("bq")] public Bq[] Bq { get; set; }
		[J("tq")] public object[] Tq { get; set; }
		[J("lup")] public long Lup { get; set; }
		[J("citn")] public string Citn { get; set; }
		[J("lut")] public long Lut { get; set; }
		[J("cn")] public string[] Cn { get; set; }
		[J("prot")] public CityProt Prot { get; set; }
		[J("mBu")] public long MBu { get; set; }
		[J("coof")] public long Coof { get; set; }
		[J("fw")] public long Fw { get; set; }
		[J("fwarn")] public long Fwarn { get; set; }
		[J("trintr")] public object[] Trintr { get; set; }
		[J("trin")] public object[] Trin { get; set; }
		[J("triin")] public object[] Triin { get; set; }
		[J("fg")] public Fg Fg { get; set; }
		[J("gg")] public long Gg { get; set; }
		[J("hs")] public long Hs { get; set; }
		[J("crt")] public long Crt { get; set; }
		[J("shp")] public long Shp { get; set; }
		[J("ww")] public long[] Ww { get; set; }
		[J("cbt")] public long Cbt { get; set; }
		[J("mibt")] public long Mibt { get; set; }
		[J("crth")] public long Crth { get; set; }
		[J("shph")] public long Shph { get; set; }
		[J("crtu")] public long Crtu { get; set; }
		[J("shpu")] public long Shpu { get; set; }
		[J("tr")] public object[] Tr { get; set; }
		[J("oa")] public object[] Oa { get; set; }
		[J("comm")] public long Comm { get; set; }
		[J("bab")] public string Bab { get; set; }
		[J("buldtimeleft")] public long Buldtimeleft { get; set; }
		[J("lud")] public long Lud { get; set; }
		//[J("mo")] public Mo[] Mo { get; set; }
		[J("cartinfo")] public string Cartinfo { get; set; }
		[J("cg")]  public long[] Cg { get; set; }
		[J("lock")] public long Lock { get; set; }
		[J("ble")] public Dictionary<string, long> Ble { get; set; }
		[J("ciupd")] public long Ciupd { get; set; }
		[J("trts")] public long Trts { get; set; }
		[J("trpl")] public long Trpl { get; set; }
		[J("ctt")] public long Ctt { get; set; }
		[J("ncs")] public long Ncs { get; set; }
		[J("rcs")] public long Rcs { get; set; }
		[J("ics")] public long Ics { get; set; }
		[J("fcs")] public long Fcs { get; set; }
		[J("aatroopittest")] public string Aatroopittest { get; set; }
		[J("aispaid")] public string Aispaid { get; set; }
		[J("aaaaaa")] public string Aaaaaa { get; set; }
		[J("ot")] public object[] Ot { get; set; }
		[J("msave")] public long Msave { get; set; }
		[J("musave")] public long Musave { get; set; }
		[J("buto")] public long Buto { get; set; }
		[J("butu")] public long Butu { get; set; }
		[J("thlvlcheck")] public long Thlvlcheck { get; set; }
	}

	public partial class Fg
	{
		[J("t")] public long T { get; set; }
		[J("l")] public long L { get; set; }
		[J("n")] public long N { get; set; }
	}
	public partial class Bq
	{
		public byte slvl { get; set; }
		public byte elvl { get; set; }
		public long ds { get; set; }
		public long de { get; set; }

		public long bid { get; set; }
		public ushort brep { get; set; }
		public ushort bspot { get; set; }

	}
	public partial class CityProt
	{
		[J("s")] public long S { get; set; }
		[J("e")] public long E { get; set; }
	}

	public partial class CityR
	{
		[J("r")] public long R { get; set; }
		[J("g")] public long G { get; set; }
	}

	public partial class Tp
	{
		[J("t")] public long T { get; set; }
		[J("s")] public long S { get; set; }
	}

	public partial class JSPlayer
	{
		[J("_id")] public long Id { get; set; }
		[J("pn")] public string Pn { get; set; }
		[J("pid")] public long Pid { get; set; }
		[J("lcit")]  public long Lcit { get; set; }
		[J("r")] public long R { get; set; }
		[J("t")] public string T { get; set; }
		[J("sc")] public long Sc { get; set; }
		[J("g")] public G G { get; set; }
		[J("m")] public M M { get; set; }
		[J("ms")] public M Ms { get; set; }
		[J("cac")] public long Cac { get; set; }
		[J("cc")] public long Cc { get; set; }
		[J("bc")] public long Bc { get; set; }
		[J("b")] public long B { get; set; }
		[J("bi")] public long Bi { get; set; }
		[J("bq")] public long Bq { get; set; }
		[J("pr")] public Dictionary<string, long> Pr { get; set; }
		[J("cg")] public object[] Cg { get; set; }
		[J("rs")] public Dictionary<string, PlayerR> Rs { get; set; }
		[J("rw")] public Rw[] Rw { get; set; }
		[J("ts")] public long Ts { get; set; }
	//	[J("lc")] public Dictionary<string, Lc> Lc { get; set; }
		[J("cob")] public Dictionary<string, long> Cob { get; set; }
		[J("cobm")] public Dictionary<string, long> Cobm { get; set; }
		[J("npp")] public long Npp { get; set; }
		[J("ft")] public long[] Ft { get; set; }
		//[J("opt")] public Dictionary<string, Lc> Opt { get; set; }
		[J("hlp")] public long[] Hlp { get; set; }
		[J("arc")] public long Arc { get; set; }
		[J("lock")] public long Lock { get; set; }
		[J("nppf")] public string Nppf { get; set; }
		[J("planame")] public string Planame { get; set; }
		[J("cb")] public long Cb { get; set; }
		[J("prot")] public PlayerProt Prot { get; set; }
		[J("fa")] public Dictionary<string, long> Fa { get; set; }
		[J("paid")] public long Paid { get; set; }
		[J("fwc")] public long Fwc { get; set; }
		[J("sco")] public long Sco { get; set; }
		[J("mibt")] public long Mibt { get; set; }
		[J("alatitties")] public long Alatitties { get; set; }
		[J("td")] public Mvb Td { get; set; }
		[J("mvb")] public Mvb Mvb { get; set; }
		[J("mats")] public long Mats { get; set; }
		[J("lrct")] public long Lrct { get; set; }
		[J("repcnt")] public long Repcnt { get; set; }
		[J("crw")] public long Crw { get; set; }
		[J("pccount")] public long Pccount { get; set; }
		[J("gr")] public long Gr { get; set; }
		[J("fec")] public long Fec { get; set; }
		[J("specse")] public long Specse { get; set; }
		[J("gra")] public Gra Gra { get; set; }
		[J("itc")] public Dictionary<string, long> Itc { get; set; }
		[J("suba")] public long Suba { get; set; }
		[J("subb")] public long Subb { get; set; }
		[J("subc")] public long Subc { get; set; }
		[J("blk")] public object[] Blk { get; set; }
	}

	public partial class G
	{
		[J("t")] public long T { get; set; }
		[J("b")] public long B { get; set; }
	}

	public partial class Gra
	{
		[J("t")] public string T { get; set; }
	}

	public partial class M
	{
		[J("t")] public double T { get; set; }
		[J("b")] public long B { get; set; }
		[J("r")] public long R { get; set; }
		[J("f")] public long? F { get; set; }
	}

	public partial class Mvb
	{
		[J("t")] public long T { get; set; }
		[J("l")] public long L { get; set; }
	}

	public partial class PlayerProt
	{
		[J("s")] public long S { get; set; }
		[J("e")] public long E { get; set; }
		[J("sf")] public long Sf { get; set; }
		[J("ef")] public long Ef { get; set; }
	}

	public partial class PlayerR
	{
		[J("n")] public long N { get; set; }
		[J("l")] public long L { get; set; }
	}

	public partial class Rw
	{
		[J("p")] public double P { get; set; }
		[J("l")] public long L { get; set; }
	}

	//public partial struct Mo
	//{
	//	public long? Integer;
	//	public long[] IntegerArray;

	//	public static implicit operator Mo(long Integer) => new Mo { Integer = Integer };
	//	public static implicit operator Mo(long[] IntegerArray) => new Mo { IntegerArray = IntegerArray };
	//}

	//public partial struct Lc
	//{
	//	public long? Integer;
	//	public string String;

	//	public static implicit operator Lc(long Integer) => new Lc { Integer = Integer };
	//	public static implicit operator Lc(string String) => new Lc { String = String };
	//}

}