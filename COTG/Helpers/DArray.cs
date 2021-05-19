using COTG.Game;

using Microsoft.Toolkit.HighPerformance.Enumerables;

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static COTG.Debug;
namespace COTG
{
	[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
	public class DArray<T> :  IDisposable, IEnumerable<T>  where T : struct 
	{
		static ArrayPool<T> pool = ArrayPool<T>.Shared;

		public T[] v = Array.Empty<T>();
		public int count;
		public int Length => count;
		public int Count => count;

		public DArray(int maxSize)
		{
			if(maxSize != 0)
				v = pool.Rent(maxSize);
		}

		public void AddRange(in IEnumerable<T> i)
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

		public void Add(in T i)
		{
			if(!CanGrow())
			{
				GrowBuffer((count + 1) * 2); 
			}
			v[count++] = i;
		}
		public void GrowBuffer( int size)
		{
			if (size <= v.Length)
				return;
			var _v = v;
			v = pool.Rent(size);
			for(int i=0;i<count;++i)
			{
				v[i] = v[i];
				
			}
			if(_v.Length != 0)
				pool.Return(_v);
		}

		public bool CanGrow() => count < v.Length;
		public void Clear() => count = 0;

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}
		public Span<T> span => new Span<T>(v, 0,count);


		public static implicit operator Span<T>( DArray<T> me) => new Span<T>(me.v, 0, me.count);

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
			if ( v.Length != 0 )
			{
				var _v = v;
				v = Array.Empty<T>();
				count = 0;
				pool.Return(_v);
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

		private string GetDebuggerDisplay()
		{
			return ToString();
		}

		public override string ToString()
		{
			return $"DArray<{typeof(T)}>{count}";
		}
	}
}
