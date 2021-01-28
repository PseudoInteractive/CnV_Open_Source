using COTG.Game;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
		public const int sharestringOffset = 444;
		public static Dictionary<byte, byte> sharestringToBuldings;
		
		static BuildingDef()
		{
			all = Json.FromResources<Dictionary<int, BuildingDef>>("buildingDef");
			byte counter = 0;
			idToBid = new BuildingDef[byte.MaxValue];
			prototypes = new Dictionary<int, BuildingDef>();
			foreach (var i in all)
			{
				var b = all[i.Key];
				b.bid = i.Key;
				b.id = counter;
				idToBid[counter] = i.Value;
				// use the first one for the prototype?
				prototypes[b.Proto] = b;
				++counter;
			}
			// all extra buildings are assigned to "None" this may not be needed
			for (; ; )
			{
				idToBid[counter] = idToBid[0]; // The first building is null
				if (++counter == byte.MaxValue)
					break;
			}
			//}
			sharestringToBuldings = new Dictionary<byte, byte>();
			var ix = new byte[] {
				 (byte)'-',(byte)(0),
				 (byte)',',0, //(byte)(452 - sharestringOffset),
				 (byte)'.',0, //(byte)(454 - sharestringOffset),
				 (byte)'1',(byte)(447 - sharestringOffset),
				 (byte)'2',(byte)(448 - sharestringOffset),
				 (byte)'3',(byte)(461 - sharestringOffset),
				 (byte)'4',(byte)(465 - sharestringOffset),
     			 (byte)':',0, // (byte)(451 - sharestringOffset), // stone
				 (byte)';',0, // (byte)(453 - sharestringOffset),
				 (byte)'A',(byte)(462 - sharestringOffset),
				 (byte)'B',(byte)(445 - sharestringOffset),
				 (byte)'C',(byte)(446 - sharestringOffset),
				 (byte)'D',(byte)(477 - sharestringOffset),
				 (byte)'E',(byte)(466 - sharestringOffset),
				 (byte)'G',(byte)(483 - sharestringOffset),
				 (byte)'H',(byte)(479 - sharestringOffset),
				 (byte)'J',(byte)(500 - sharestringOffset),
				 (byte)'K',(byte)(504 - sharestringOffset),
				 (byte)'L',(byte)(460 - sharestringOffset),
				 (byte)'M',(byte)(463 - sharestringOffset),
				 (byte)'P',(byte)(449 - sharestringOffset),
				 (byte)'R',(byte)(490 - sharestringOffset),
				 (byte)'S',(byte)(464 - sharestringOffset),
				 (byte)'T',(byte)(455 - sharestringOffset),
				 (byte)'U',(byte)(481 - sharestringOffset),
				 (byte)'V',(byte)(498 - sharestringOffset),
				 (byte)'X',(byte)(467 - sharestringOffset),
				 (byte)'Y',(byte)(502 - sharestringOffset),
				 (byte)'Z',(byte)(482 - sharestringOffset),
			};
			for(int p=0;p<ix.Length; p+=2)
			{
				sharestringToBuldings.Add(ix[p], ix[p + 1]);
			}


		}
					
		public static BuildingDef FromId(byte id) => idToBid[id];
		public static BuildingDef[] idToBid;
		public static Dictionary<int, BuildingDef> all; // indexed via bid
		public static Dictionary<int, BuildingDef> prototypes; // maps prototype id's to buildings.  Some buildings share a prototype

		[JsonIgnore]
		public bool isRes => Bc.Count == 1;

		public static bool IsRes(int id) => id switch { City.bidStone =>true, City.bidIron =>true,City.bidLake=>true,City.bidForest=>true,_=>false };

		[JsonIgnore]
		public bool isPost => Trcap!=null;
		[JsonIgnore]
		public bool isBarricade => Trrep!=null;
		[JsonIgnore]
		public bool isTower => isPost || isBarricade;
		[JsonIgnore]
		public bool isWall => bid == Game.City.bidWall;
		[JsonIgnore]
		public bool isTownHall => bid == Game.City.bidTownHall;
		// Not persisted
		[JsonIgnore]
		public byte id; // packed id
						// not persisted
		[JsonIgnore]
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
