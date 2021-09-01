using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using COTG.Game;
namespace COTG
{
	public readonly record struct Building
	{
		public readonly byte id; // this is pretranslated from the buildingDefs definitions to pack it into a byte
		public readonly byte bl; // building level

		public readonly short bid => def.bid;
		public readonly bool isBuilding => bl > 0;
		public readonly bool isEmpty => id == 0;
		public readonly bool isRes => def.isRes;
		public readonly BuildingDef def => BuildingDef.FromId(id);
		public readonly string name => def.Bn;
	
		public readonly bool isCabin => id == BuildingDef.idCabin;
		public readonly bool isTemple => id == BuildingDef.BidToId(City.bidTemple);

		public readonly void AssertValid() => COTG.Debug.Assert( isRes || bl!=0 );
		public readonly bool isMilitary => def.isMilitary;
		public readonly bool isBarracks => def.isBarracks;

		public readonly bool isTower => def.isTower;
		public readonly bool isWall => def.isWall;
		public readonly bool isTownHall => id == BuildingDef.idTownHall;
		public readonly bool requiresBuildingSlot => def.requiresBuildingSlot;
		public Building(byte id, byte bl)
		{
			this.id = id;
			this.bl = bl;
		}
	//	public static ArrayPool<Building> pool = ArrayPool<Building>.Create(City.citySpotCount,64);
	//	public static Building[] Rent() => pool.Rent(City.citySpotCount);
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
