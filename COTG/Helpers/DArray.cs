using Microsoft.Toolkit.HighPerformance.Enumerables;

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static COTG.Debug;
namespace COTG
{
	public class DArray<T> :  IDisposable, IEnumerable<T>  where T : struct 
	{
		static ArrayPool<T> pool = ArrayPool<T>.Shared;

		public T[] v;
		public int count;
	
		public DArray(int maxSize)
		{
			v = pool.Rent(maxSize);
		}

		public void Add(in T i)
		{
			v[count++] = i;
		}
		public bool CanGrow() => count < v.Length;
		public void Clear() => count = 0;

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}
		public Span<T> span => new Span<T>(v, 0,count);


		public static explicit operator Span<T>( DArray<T> me) => new Span<T>(me.v, 0, me.count);

		public SpanEnumerable<T> Enumerate()
		{
			return new SpanEnumerable<T>(span);
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}
		public ref T this[int i] => ref v[i];

		public void Dispose()
		{
			if (v != null)
			{
				pool.Return(v);
				v = null;
			}
		}

		public Enumerator iterate => new Enumerator(this);
		//
		//  A snapshot of that array memory location and count is taken at iterator start
		//  Changes in size will not effect iteration
		//  Reallocation will not effect the iteration
		//  Modifying existing items will
		// 
		public struct Enumerator: IEnumerator<T>
		{
			
			T[] array;
			int count;
			public int i;
			public ref T r => ref array[i]; // ref access

			T IEnumerator<T>.Current => r;
			object IEnumerator.Current => r;

			public Enumerator( DArray<T> a) { array = a.v; count = a.count; i = -1; }

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
	}
}
