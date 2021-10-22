using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
namespace COTG
{

	[MessagePackObject]
	public struct CityCustom:IEquatable<CityCustom>
	{
		[Key(0)]
		public int cid;
		[Key(1)]
		public bool pinned;

		public static ImmutableHashSet<CityCustom> all = ImmutableHashSet<CityCustom>.Empty;
		public static bool loaded;

		public bool Equals(CityCustom other)
		{
			return cid == other.cid;
		}

		public override int GetHashCode() => cid;
		public override string ToString() => $"{City.Get(cid)}: pinned={pinned}";
	}
	

}
