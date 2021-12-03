using System.Collections.Generic;
using static CnV.Game.Troops;
using TroopTypeCountsRef = CnV.Game.TroopTypeCounts;

using System.Runtime.InteropServices;
namespace CnV.Game
{
	using System;

	//public sealed class ArmySourceTargetComparer : IEqualityComparer<Army>
	//{
	//	public bool Equals(Army x, Army y)
	//	{
	//		return ((x.sourceCid - y.sourceCid) | (x.targetCid - y.targetCid)) == 0;
	//	}

	//	public int GetHashCode(Army x)
	//	{
	//		return (x.sourceCid * AMath.randomPrime1s + x.targetCid);
	//		// symetric?
	//	}
	//}
	//public sealed class OutgoingAttack
	//{
	//	DateTimeOffset departs;
	//	DateTimeOffset arrives;
	//	int id;
	//	string desc;
	//	int targetCid;
	//}
	[StructLayout(LayoutKind.Sequential)]
	public struct TroopTypeCounts : IComparable<TroopTypeCounts>, IEquatable<TroopTypeCounts>
	{
		public unsafe fixed int counts[(ttCount)];

		public int this[int id]
		{
			readonly get
			{
				unsafe
				{
					var rv = counts[id];
					return rv;
				}
			}

			set
			{
				unsafe
				{
					counts[id] = value;
				}
			}
		}
		public readonly int Count
		{
			 get
			{
				int count = 0;
				unsafe
				{
					for (int i = 0; i < ttCount; ++i)
					{
						if (counts[i] != 0)
						{
							++count;
						}
					}
				}
				return count;
			}
		}

		
		public readonly IEnumerable<TroopTypeCount> Enumerate()
		{

			for (int i = 0; i < ttCount;++i)
			{
				var count = this[i];
				if (count == 0)
					continue;
				yield return new TroopTypeCount(i, count);
			}
		}


		internal readonly int count => Count;
		internal readonly int Length => Count;

		//public void Append( TroopTypeCount t)
		//{
		//	unsafe
		//	{
		//		counts[t.type] += t.count;
		//	}
		//}
		//public void Add(TroopTypeCount t)
		//{
		//	unsafe
		//	{
		//		counts[t.type] += t.count;
		//	}
		//}
		public readonly TroopTypeCount GetIndex(int id)
		{
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					if (counts[i] != 0 )
					{
						if (id-- == 0)
							return new TroopTypeCount(i, counts[i]);
					}
				}
			}
			return new();
		}
		public readonly int GetIndexType(int id)
		{
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					if (counts[i] != 0)
					{
						if (id-- == 0)
							return i;
					}
				}
			}
			return new();
		}
		public readonly int GetIndexCount(int id)
		{
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					if (counts[i] != 0)
					{
						if (id-- == 0)
							return counts[i];
					}
				}
			}
			return new();
		}

		//public void Append(in TroopTypeCountsX a)
		//{
		//	unsafe
		//	{
		//		for (int i = 0; i < ttCount; ++i)
		//		{
		//			counts[i] += a.counts[i];
		//		}
		//	}
		//}
		
		//public void Set(in TroopTypeCountsX a)
		//{
		//	unsafe
		//	{
		//		for (int i = 0; i < ttCount; ++i)
		//		{
		//			counts[i] = a.counts[i];
		//		}
		//	}
		//}
		//public void Set(int type,int count )
		//{
		//	unsafe
		//	{
		//			counts[type] = count;
		//	}
		//}
		public void Clear()
		{
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					counts[i] = 0;
				}
			}
		}
		public readonly EnumeratorStruct GetEnumerator()
		{
			var rv = new EnumeratorStruct();
			rv.current = -1;
			int count = 0;
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					if (counts[i] > 0)
					{
						rv.tts[i] = EnumeratorStruct.IntsToU64(i,counts[i]);
						++count;
					}
				}
			}
			rv.count = count;
			return rv;

		}

		public ref struct EnumeratorStruct
		{
			public unsafe fixed ulong tts[(ttCount)];
			public int current;
			public int count;
			public readonly int Count  => count;
			public readonly int Length => Length;
			public static ulong IntsToU64(int type, int count)
			{
				return ((ulong)(uint)type) | (((ulong)(uint)count) << 32);
			}
			public static ulong TTCToU64(TroopTypeCount v)
			{
				return ((ulong)(uint)v.type) | (((ulong)(uint)v.count) << 32);
			}
			public static TroopTypeCount U64ToTTC(ulong v)
			{
				return new TroopTypeCount((int)(v), (int)(v >> 32));
			}
			public unsafe fixed ulong v[(ttCount)];

			//	int count;
			//	public int i;
			public TroopTypeCount this[int i]
			{
				readonly get
				{
					unsafe
					{
						return U64ToTTC(v[i]);
					}
				}
				set
				{
					unsafe
					{
						v[i] = TTCToU64(value);
					}

				}
			}
			public readonly TroopTypeCount Current => this[current];

	//		public void Sort() => Array.Sort<ulong>(v);
			//	object IEnumerator.Current => r;

			//	public Enumerator(DArray<T> a) { array = a.v; count = a.count; i = -1; }

			//	public bool Next()
			//	{
			//		return ++i < count;
			//	}
			public bool MoveNext()
			{
				return ++current < count;
			}

			public void Reset()
			{
				current = -1;
			}
			public EnumeratorStruct GetEnumerator() => this;
		}


		internal readonly  bool Any()
		{
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					var c = counts[i];
					if (c != 0)
						return true;
				}
			}
			return false;
		}

		internal readonly bool Any(Func<TroopTypeCount, bool> p)
		{
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					var c = counts[i];
					if (c != 0 && p( new TroopTypeCount(i,c))  )
						return true;
				}
			}
			return false;
		}

		internal readonly EnumeratorStruct GetListDescending()
		{
			var rv = new EnumeratorStruct();
			rv.current = -1;
			int count = 0;
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					// insert sort
					if (counts[i] > 0)
					{
						var uv = EnumeratorStruct.IntsToU64(i, counts[i]);
						int put = 0;
						for(;put<count;++put)
						{
							if (uv >= rv.tts[put])
								break;
						}
						++count;
						for (int j=count;--j>put;)
						{
							rv[j] = rv[j - 1];
						}
						rv.tts[put] = uv;
					}
				}
			}
			rv.count = count;
			return rv;
		}

		internal readonly TroopTypeCount FirstOrDefault(Func<TroopTypeCount, bool> p)
		{
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					var c = counts[i];
					if (c != 0)
					{
						var tt = new TroopTypeCount(i, c);
						if( p(tt))
							return tt;
					}
				}
			}
			return new();
		}

		internal readonly bool HasTT(int tt)
		{
			unsafe
			{
				return counts[tt]!=0;
			}
		}
		internal readonly void ForEach(Action<TroopTypeCount> p)
		{
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					var c = counts[i];
					if (c != 0)
					{
						p( new TroopTypeCount(i, c));
					}
				}
			}
		}
		internal readonly int Sum(Func<TroopTypeCount,int> p)
		{
			unsafe
			{
				var rv = 0;
				for (int i = 0; i < ttCount; ++i)
				{
					var c = counts[i];
					if (c != 0)
					{
						rv += p(new TroopTypeCount(i, c));
					}
				}
				return rv;
			}
		}
		internal readonly float Sum(Func<TroopTypeCount, float> p)
		{
			unsafe
			{
				var rv = 0f;
				for (int i = 0; i < ttCount; ++i)
				{
					var c = counts[i];
					if (c != 0)
					{
						rv += p(new TroopTypeCount(i, c));
					}
				}
				return rv;
			}
		}
		public readonly int TSOff()
		{
			var rv = 0;
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					if (!ttIsDef[i])
						rv += Troops.ttTs[i] * counts[i];
				}
			}
			return rv;
		}
		public readonly int TS()
		{
			var rv = 0;
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
						rv += Troops.ttTs[i] * counts[i];
				}
			}
			return rv;
		}

		//public static bool  HasTT(this TroopTypeCounts me, int type)
		//{
		//    foreach (var i in me)
		//    {
		//        if (i.type == type)
		//            return true;
		//    }
		//    return false;
		//}
		//      public static bool SetCount(this TroopTypeCounts me, int type, int count)
		//      {
		//	for(int i=0;i<me.count;++i)
		//	{ 

		//              if (me[i].type == type)
		//              {
		//			me[i].count=count;
		//                  return true;
		//              }
		//          }
		//          return false;
		//      }
		//public static void SetOrAdd(this TroopTypeCounts me, int type, int count)
		//{
		//	unsafe
		//	{
		//		me.counts[type] = count;
		//	}
		//}



		public readonly int TS( int type)
		{
			unsafe
			{
				return Troops.TTTs(type, counts[type]);
			}
		}
		public readonly int GetCount(int type)
		{
			unsafe
			{
				return counts[type];
			}
		}
		// combined TS
		public readonly int TSRaid()
		{	
			return Raiding.GetTroops(this, true, true).Sum((tt) => tt.count * Troops.ttTs[tt.type]);
		}
		public readonly int TSDef()
		{
			var rv = 0;
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					if (ttIsDef[i])
						rv += Troops.ttTs[i] * counts[i];
				}
			}
			return rv;
		}
		public readonly int TS( Func<TroopTypeCount, bool> pred)
		{
			var rv = 0;
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					var c = counts[i];
					if (c > 0 && pred(new TroopTypeCount(i, c)))
						rv += Troops.ttTs[i] * c;
				}
			}
			return rv;
		}
		public readonly byte GetPrimaryTroopType()
		{
			unsafe
			{
				var best = 0; // if no troops we return guards 
				var bestTS = int.MinValue;
				for (int i = 0; i < ttCount; ++i)
				{
					var c = counts[i];
					if (c == 0)
						continue;
					var ts = TTTs(i, c);
					if (ts > bestTS)
					{
						bestTS = ts;
						best = i;
					}
				}
				return (byte)best;
			}
		}

		public bool Equals(TroopTypeCounts other) => throw new NotImplementedException();
		public override int GetHashCode()
		{
				
				unsafe
				{
				var rv = 0;

				for(int i = 0;i < ttCount;++i)
					{
						unsafe
						{
								rv += HashCode.Combine(counts[i]);
						}
				
					}
				return rv;
			}
		}
		public int CompareTo(TroopTypeCounts other) => TS().CompareTo(other.TS());

		public override bool Equals(object obj)
		{
			if(!(obj is TroopTypeCounts other))
			{
				return false;
			}

			throw new NotImplementedException();
		}

		public static bool operator ==(TroopTypeCounts left,TroopTypeCounts right) => left.Equals(right);
		public static bool operator !=(TroopTypeCounts left,TroopTypeCounts right) => !(left==right);

		public override string ToString()
		{
			return this.Format(":",' ',',');
		}
	}
}

