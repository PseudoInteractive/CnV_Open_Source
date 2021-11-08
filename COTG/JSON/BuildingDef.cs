using COTG.Game;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using static COTG.Debug;
using static COTG.Game.City;
namespace COTG
{
	using J = System.Text.Json.Serialization.JsonPropertyNameAttribute;


	public sealed class BuildingDef
	{
		public static byte idCabin;
		public static byte idTownHall;
		public static byte BidToId(int bid)
		{
			if (bid == 0)
				return 0;
			if (all.TryGetValue(bid, out var def))
				return def.id;
			if (!(bid >= 891 && bid <= 894))
				Log("Bad building! " + bid); // farms an temples have extras
			return 0;
		}
		public static (byte c, bool valid) BidToLayout(int bid)
		{
			if (bid != 0)
			{
				var off = bid - BuildingDef.sharestringBuildingOffset;
				if(off <= 255)
				{
					if(BuildingDef.buildingsToSharestring.TryGetValue((off).AsByte(),out var o))
					{
						return (o, o != (byte)'-');
					}
				}

			}

			return ((byte)'-', false);
		}
		public static short LayoutToBid(byte v) => (short)(BuildingDef.sharestringToBuldings.TryGetValue(v, out var c) && c != 0 ? (c + BuildingDef.sharestringBuildingOffset).AsShort() : (short) 0);

		public const short sharestringBuildingOffset = 444;
		public static Dictionary<byte, byte> sharestringToBuldings;
		public static Dictionary<byte, byte> buildingsToSharestring;

		//	public static Dictionary<ushort, ushort> protToBid = new Dictionary<ushort, ushort>(new KeyValuePair<ushort, ushort>[] {
		//new KeyValuePair<ushort,ushort>(443, 448),new KeyValuePair<ushort,ushort>(456, 445),new KeyValuePair<ushort,ushort>(457, 446),new KeyValuePair<ushort,ushort>(458, 447),new KeyValuePair<ushort,ushort>(459, 449),new KeyValuePair<ushort,ushort>(468, 460),new KeyValuePair<ushort,ushort>(469, 461),new KeyValuePair<ushort,ushort>(470, 462),new KeyValuePair<ushort,ushort>(471, 463),new KeyValuePair<ushort,ushort>(472, 464),new KeyValuePair<ushort,ushort>(473, 465),new KeyValuePair<ushort,ushort>(474, 466),new KeyValuePair<ushort,ushort>(475, 467),new KeyValuePair<ushort,ushort>(478, 477),new KeyValuePair<ushort,ushort>(480, 479),new KeyValuePair<ushort,ushort>(485, 481),new KeyValuePair<ushort,ushort>(486, 482),new KeyValuePair<ushort,ushort>(487, 483),new KeyValuePair<ushort,ushort>(492, 488),new KeyValuePair<ushort,ushort>(493, 489),new KeyValuePair<ushort,ushort>(494, 490),new KeyValuePair<ushort,ushort>(495, 491),new KeyValuePair<ushort,ushort>(497, 496),new KeyValuePair<ushort,ushort>(499, 498),new KeyValuePair<ushort,ushort>(501, 500),new KeyValuePair<ushort,ushort>(503, 502),new KeyValuePair<ushort,ushort>(505, 504),new KeyValuePair<ushort,ushort>(575, 539),new KeyValuePair<ushort,ushort>(576, 540),new KeyValuePair<ushort,ushort>(577, 541),new KeyValuePair<ushort,ushort>(578, 542),new KeyValuePair<ushort,ushort>(579, 543),new KeyValuePair<ushort,ushort>(580, 544),new KeyValuePair<ushort,ushort>(581, 545),new KeyValuePair<ushort,ushort>(582, 546),new KeyValuePair<ushort,ushort>(583, 547),new KeyValuePair<ushort,ushort>(584, 547),new KeyValuePair<ushort,ushort>(586, 549),new KeyValuePair<ushort,ushort>(587, 551),new KeyValuePair<ushort,ushort>(588, 552),new KeyValuePair<ushort,ushort>(589, 553),new KeyValuePair<ushort,ushort>(590, 554),new KeyValuePair<ushort,ushort>(591, 555),new KeyValuePair<ushort,ushort>(592, 556),new KeyValuePair<ushort,ushort>(593, 557),new KeyValuePair<ushort,ushort>(594, 558),new KeyValuePair<ushort,ushort>(595, 559),new KeyValuePair<ushort,ushort>(596, 560),new KeyValuePair<ushort,ushort>(597, 561),new KeyValuePair<ushort,ushort>(598, 562),new KeyValuePair<ushort,ushort>(599, 563),new KeyValuePair<ushort,ushort>(600, 564),new KeyValuePair<ushort,ushort>(601, 565),new KeyValuePair<ushort,ushort>(602, 566),new KeyValuePair<ushort,ushort>(603, 567),new KeyValuePair<ushort,ushort>(604, 568),new KeyValuePair<ushort,ushort>(605, 569),new KeyValuePair<ushort,ushort>(606, 570),new KeyValuePair<ushort,ushort>(607, 571),new KeyValuePair<ushort,ushort>(608, 572),new KeyValuePair<ushort,ushort>(609, 573),new KeyValuePair<ushort,ushort>(610, 574)});



		//public int ProtToBid(int prot)
		//{

		//	if (protToBid.TryGetValue((ushort)prot, out var rv))
		//		return rv;
		//	return prot;

		//}
		public static async Task Init()
		{
			all = await Json.FromContent<Dictionary<int, BuildingDef>>("buildingDef");
			byte counter = 0;
			
			idToBid = new BuildingDef[256];
		//	prototypes = new Dictionary<int, BuildingDef>();
			foreach (var i in all)
			{
				var b = all[i.Key];
				b.bid = (short)i.Key;
				b.id = counter;
				if(b.refId==0)
				{
					b.refId = b.bid;
					idToBid[counter] = i.Value;
				}
				else
				{
					idToBid[counter] = all[b.refId];
				}
				// use the first one for the prototype?
			//	prototypes.TryAdd(b.Proto, b);
				++counter;
			}
			// all extra buildings are assigned to "None" this may not be needed
			for (; ; )
			{
				idToBid[counter] = idToBid[0]; // The first building is null
				if(counter++ >= 255)
					break;
			}
			idCabin = BidToId(City.bidCottage);
			idTownHall = BidToId(City.bidTownHall);
			//}
			var ix = new byte[] {
				 (byte)'-',(byte)(0),
				 (byte)',',(byte)0,//(452 - sharestringOffset),
				 (byte)'.',(byte)0,//(454 - sharestringOffset),
				 (byte)'1',(byte)(447 - sharestringBuildingOffset),
				 (byte)'2',(byte)(448 - sharestringBuildingOffset),
				 (byte)'3',(byte)(461 - sharestringBuildingOffset),
				 (byte)'4',(byte)(465 - sharestringBuildingOffset),
				 (byte)':',(byte)0,//(451 - sharestringOffset), // stone
				 (byte)';',(byte)0,//(453 - sharestringOffset),
				 (byte)'A',(byte)(462 - sharestringBuildingOffset),
				 (byte)'B',(byte)(445 - sharestringBuildingOffset),
				 (byte)'C',(byte)(446 - sharestringBuildingOffset),
				 (byte)'D',(byte)(477 - sharestringBuildingOffset),
				 (byte)'E',(byte)(466 - sharestringBuildingOffset),
				 (byte)'G',(byte)(483 - sharestringBuildingOffset),
				 (byte)'H',(byte)(479 - sharestringBuildingOffset),
				 (byte)'J',(byte)(500 - sharestringBuildingOffset),
				 (byte)'K',(byte)(504 - sharestringBuildingOffset),
				 (byte)'L',(byte)(460 - sharestringBuildingOffset),
				 (byte)'M',(byte)(463 - sharestringBuildingOffset),
				 (byte)'P',(byte)(449 - sharestringBuildingOffset),
				 (byte)'R',(byte)(bidPort - sharestringBuildingOffset),
				 (byte)'R',(byte)(490 - sharestringBuildingOffset),
				 (byte)'R',(byte)(489 - sharestringBuildingOffset),
				 (byte)'S',(byte)(464 - sharestringBuildingOffset),
				 (byte)'T',(byte)(bidTownHall - sharestringBuildingOffset),
				 (byte)'U',(byte)(481 - sharestringBuildingOffset),
				 (byte)'V',(byte)(bidShipyard - sharestringBuildingOffset),
				 (byte)'V',(byte)(496 - sharestringBuildingOffset),
				 (byte)'V',(byte)(498 - sharestringBuildingOffset),
				 (byte)'X',(byte)(467 - sharestringBuildingOffset),
				 (byte)'Y',(byte)(502 - sharestringBuildingOffset),
				 (byte)'Z',(byte)(482 - sharestringBuildingOffset),
			};
			sharestringToBuldings = new Dictionary<byte, byte>(ix.Length/2);
			buildingsToSharestring = new Dictionary<byte, byte>(ix.Length/2);
			for (int p = 0; p < ix.Length; p += 2)
			{
				sharestringToBuldings.TryAdd(ix[p], ix[p + 1]);
				buildingsToSharestring.TryAdd(ix[p + 1], ix[p]);
			}


		}

		public static BuildingDef FromId(byte id) => idToBid[id];
		public static BuildingDef[] idToBid;
		public static Dictionary<int, BuildingDef> all; // indexed via bid
		//public static Dictionary<int, BuildingDef> prototypes; // maps prototype id's to buildings.  Some buildings share a prototype

		[JsonIgnore]
		public bool isRes => IsBidRes(bid);

		public static bool IsBidRes(int bid) => (bid >= City.bidResStart) & (bid <= City.bidResEnd);

		[JsonIgnore]
		public bool isPost => Trcap != null;
		[JsonIgnore]
		public bool isBarricade => Trrep != null;
		[JsonIgnore]
		public bool isTower => isPost | isBarricade;
		[JsonIgnore]
		public bool isWall => bid == Game.City.bidWall;
		[JsonIgnore]
		public bool isTownHall => bid == Game.City.bidTownHall;

		[JsonIgnore]
		public bool requiresBuildingSlot => !isTower & !isWall & !isTownHall & !isRes;


		[JsonIgnore]
		public bool isCabin => bid == Game.City.bidCottage;
		// Not persisted
		[JsonIgnore]
		public byte id; // packed id
						// not persisted
						// 8 bit 
		[JsonInclude]
		public short refId; // reference ID, sometimes this offset 
		[JsonIgnore]
		public short bid; // building id
		[JsonIgnore]
		public bool isMilitary => Ts.Any(); // building id
		[JsonIgnore]
		public bool isBarracks => Tc.Any(); // building id


		[JsonIgnore]
		internal bool isShoreBuilding => bid == City.bidPort || bid == City.bidShipyard;

		[J("bn")] public string Bn { get; set; }
		[J("proto")] public short Proto { get; set; }
		[J("ds")] public string Ds { get; set; }
		[J("bimg")] public string Bimg { get; set; }
		[J("dimg")] public string Dimg { get; set; }
		[J("climg")] public string Climg { get; set; }
		[J("limg")] public string Limg { get; set; }
		[J("simg")] public string Simg { get; set; }
		[J("tooltip")] public string Tooltip { get; set; }
		[J("rt")] public int Rt { get; set; }
		[J("tc")] public int[] Tc { get; set; }
		[J("ts")] public int[] Ts { get; set; }
		[J("mt")] public int Mt { get; set; }
		[J("sc")] public Dictionary<string, int> Sc { get; set; }
		[J("bc")] public Dictionary<string, Bc> Bc { get; set; }
		[J("thl")] public int Thl { get; set; }
		[J("cs")] public int[] Cs { get; set; }
		[J("eff")] public Dictionary<string, int> Eff { get; set; }
		[J("r")] public Dictionary<string, int> R { get; set; }
		[J("trn")] public int[] Trn { get; set; }
		[J("st")] public int[] St { get; set; }
		[J("stt")] public int Stt { get; set; }
		[J("efft")] public int Efft { get; set; }
		[J("tt")] public int Tt { get; set; }
		[J("tb")] public Dictionary<string, int> Tb { get; set; }
		[J("trcap")] public Dictionary<string, int> Trcap { get; set; }
		[J("warnt")] public Dictionary<string, int> Warnt { get; set; }
		[J("trrep")] public Dictionary<string, int> Trrep { get; set; }
		[J("defbo")] public Dictionary<string, int> Defbo { get; set; }
		[J("faith")] public Dictionary<string, int> Faith { get; set; }

		public bool isScoutpost => Warnt != null;
	}

	public struct Bc
	{
		[J("r_s")] public int RS { get; set; }
		[J("r_w")] public int RW { get; set; }
		[J("tu")] public long Tu { get; set; }
		[J("td")] public long Td { get; set; }
	}
}
