using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG
{
	public class DArray<T> : IEnumerable<T>  where T : struct 
	{
		public T[] v;
		public int count;
	
		public DArray(int maxSize)
		{
			v = new T[maxSize];
		}

		public void Add(in T i)
		{
			v[count++] = i;
		}
		public void Clear() => count = 0;

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
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
	
	}
}
