using COTG.Game;

using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.Toolkit.HighPerformance.Enumerables;

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static COTG.Debug;
namespace COTG
{
	[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
	public class DArray<T> : IDisposable, IEnumerable<T> where T : struct
	{
		static ArrayPool<T> pool = ArrayPool<T>.Shared;

		public T[] v = Array.Empty<T>();
		public int count;
		public int Length => count;
		public int Count => count;
		public bool arrayLeased;

		public DArray(int maxSize)
		{
			if (maxSize != 0)
			{
				arrayLeased = true;
				v = pool.Rent(maxSize);
			}
		}
		public DArray()
		{
		}
		public static DArray<T> freePool;
		DArray<T> freePoolNext; // linked list
		public static DArray<T> Rent()
		{
			for (; ; )
			{

				var rv = freePool;
				if (rv != null)
				{
					var next = freePool.freePoolNext;
					if (object.ReferenceEquals(Interlocked.CompareExchange(ref freePool, next, rv), rv))
					{
						return rv;
					}
				}
				else
				{
					return new DArray<T>();
				}
			}
		}

		public void Return()
		{
			Clear();
			for (; ; )
			{
				var rv = freePool;
				freePoolNext = rv;
				if (object.ReferenceEquals(Interlocked.CompareExchange(ref freePool, this, rv), rv))
				{
					break;
				}
			}
		}


		//public DArray(T[] _array)
		//{
		//	AddRange(_array);
		//}
		public void AddRange<E>(in E i) where E : IEnumerable<T> 
		{
			foreach (var _i in i)
			{
				if (!CanGrow())
				{
					GrowBuffer((count + 1) * 2);
				}
				v[count++] = _i;
			}
		}
		public void Set<E>(in E i) where E : IEnumerable<T>
		{
			Clear();
			AddRange(i);
		}
		public void Set(in T i)
		{
			Clear();
			Add(i);
		}
		public DArray<T> Clone()
		{
			var rv = Rent();
			Assert(rv.count == 0);
			rv.GrowBuffer(count);
			rv.count = count;
			for (int i = 0; i < count; ++i)
				rv.v[i] = v[i];
			return rv;
		}
		public static explicit operator MemoryOwner<T>(DArray<T> rv)
		{
			var m = MemoryOwner<T>.Allocate(rv.count);
			var sp = m.Span;
			for (int i = 0; i < rv.count; ++i)
				sp[i] = rv[i];
			return m;
		}

		public void Add(in T i)
		{
			if (!CanGrow())
			{
				GrowBuffer((count + 1) * 2);
			}
			v[count++] = i;
		}
		public void GrowBuffer(int size)
		{
			if (size <= v.Length)
				return;
			size = size.Max(16);
			var _v = v;
			var wasLeased = arrayLeased;
			arrayLeased = true;
			v = pool.Rent(size);
			for (int i = 0; i < count; ++i)
			{
				v[i] = v[i];

			}
			if (wasLeased)
				pool.Return(_v);
		}

		public bool CanGrow() => count < v.Length;

		//static public implicit operator DArray<T>(T[] e)
		//{
		//	return new DArray<T>(e);
		//}



		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}
		public Span<T> span => new Span<T>(v, 0, count);


		public static implicit operator Span<T>(DArray<T> me) => new Span<T>(me.v, 0, me.count);

		public SpanEnumerable<T> Enumerate()
		{
			return new SpanEnumerable<T>(span);
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}
		public ref T this[int i] => ref v[i];

		public void ClearKeepBuffer()
		{
			// does not release buffer
			count = 0;
		}
		public void Clear()
		{
			if (arrayLeased)
			{
				arrayLeased = false;
				var _v = v;
				v = Array.Empty<T>();
				pool.Return(_v);
			}
			else
			{
				Assert(count == 0);
			}
			count = 0;
		}


		public void Dispose()
		{
			Clear();
		}

		public Enumerator iterate => new Enumerator(this);
		//
		//  A snapshot of that array memory location and count is taken at iterator start
		//  Changes in size will not effect iteration
		//  Reallocation will not effect the iteration
		//  Modifying existing items will
		// 
		public struct Enumerator : IEnumerator<T>
		{

			T[] array;
			int count;
			public int i;
			public ref T r => ref array[i]; // ref access

			T IEnumerator<T>.Current => r;
			object IEnumerator.Current => r;

			public Enumerator(DArray<T> a) { array = a.v; count = a.count; i = -1; }

			public bool Next()
			{
				return ++i < count;
			}
			bool IEnumerator.MoveNext()
			{
				return ++i < count;
			}

			void IEnumerator.Reset()
			{
				i = -1;
			}

			void IDisposable.Dispose()
			{
				count = 0;
				array = null;
			}
		}

		internal void RemoveAt(int offset)
		{
			if (offset < 0 || offset >= count)
			{
				Assert(false);
				return;
			}

			--count;
			for (int i = offset; i < count; ++i)
			{
				v[i] = v[i + 1];
			}

		}

		private string GetDebuggerDisplay()
		{
			return ToString();
		}

		public override string ToString()
		{
			return $"DArray<{typeof(T)}>{count}";
		}

		//internal void Sort()
		//{
		//	v.SortSmall();
		//}
	}
	public struct DArrayRef<T> : IDisposable where T : struct
	{
		public DArray<T> v;
		public int Count => v != null ? v.Count : 0;
		public int count => Count;
		public int Length => Count;
		
		public DArrayRef(DArray<T> v)
		{
			this.v = v;
		}
		public ref T this[int i] => ref v.v[i];
		public DArrayRef(bool alloc=true)
		{
			this.v = alloc? DArray<T>.Rent() : null;
		}

		//internal void Add(T tt)
		//{
		//	v.Add(tt);
		//}

		public DArray<T> Clone() => v.Clone();

		public void Free()
		{
			if (v != null)
			{
				var _v = v;
				v = null;
				_v.Dispose();
			}

		}
		public void Reset()
		{
			v.ClearKeepBuffer();
		}

		//public void TakeRented(ref DArray<T> _v)
		//{
		//	Free();
		//	v = _v;
		//	_v = null;
		//}
		//public void TakeCopy(DArray<T> _v)
		//{
		//	Free();
		//	v = _v.Clone();
		//}

		//public DArray<T> Take()
		//{
		//	var _v = v;
		//	v = null;
		//	return _v;
		//}

		//public void Set(in IEnumerable<T> i)
		//{
		//	v.Set(i);
		//}

		public void Dispose()
		{
			Free();
		}
		//IEnumerator<T> IEnumerable<T>.GetEnumerator()
		//{
		//	Assert(v != null);
		//	return new DArray<T>.Enumerator(v);
		//}
		//IEnumerator IEnumerable.GetEnumerator()
		//{
		//	Assert(v != null);
		//	return new DArray<T>.Enumerator(v);
		//}

		//internal static DArray<T> Alloc()
		//{
		//	return DArray<T>.Rent();
		//}

		//internal void TakeCopy(DArray<T> sumDef)
		//{
		//	TakeCopy(sumDef.v);
		//}

		internal void Set(T t)
		{
			v.Set(t);
		}

		internal void Take(ref DArrayRef<T> tr)
		{
			Free();
			v = tr.v;
			tr.v=null;


		}
	}
}
