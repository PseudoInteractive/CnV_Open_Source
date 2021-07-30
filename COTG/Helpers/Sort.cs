using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace COTG
{
	public static class Sort
	{

		/// <summary>
		/// Container class for HPCsharp extension methods
		/// </summary>
			/// <summary>
			/// O(n^2) sorting algorithm that's in-place. Fast for small arrays
			/// l: left/starting index within the List where to start sorting
			/// size: number of elements to sort
			/// comparer: optional compare method
			/// </summary>
			public static void SortSmall<T>(this IList<T> a, Comparison<T> comparer)
			{
				const int l = 0;
				int size = a.Count;
				var equalityComparer = comparer;
				int r = l + size;
				for (int i = l + 1; i < r; i++)
				{
					//if (a[i] < a[i - 1])        // no need to do (j > 0) compare for the first iteration
					if (equalityComparer(a[i], a[i - 1]) < 0)
					{
						T currentElement = a[i];
						a[i] = a[i - 1];
						int j = i - 1;
						for (; j > l && equalityComparer(currentElement, a[j - 1]) < 0; j--)
						{
							a[j] = a[j - 1];
						}
						a[j] = currentElement;  // always necessary work/write
					}
					// Perform no work at all if the first comparison fails - i.e. never assign an element to itself!
				}
			}

		public static void SortSmall<T>(this IList<T> a)
		{
			const int l = 0;
			int size = a.Count;
			int r = l + size;
			var comparable = Comparer<T>.Default;
			for (int i = l + 1; i < r; i++)
			{
				//if (a[i] < a[i - 1])        // no need to do (j > 0) compare for the first iteration
				if (comparable.Compare( a[i], a[i - 1]) < 0)
				{
					T currentElement = a[i];
					a[i] = a[i - 1];
					int j = i - 1;
					for (; j > l && comparable.Compare(currentElement, a[j - 1])<0; j--)
					{
						a[j] = a[j - 1];
					}
					a[j] = currentElement;  // always necessary work/write
				}
				// Perform no work at all if the first comparison fails - i.e. never assign an element to itself!
			}
		}
		/// <summary>
		/// O(n^2) sorting algorithm that's in-place. Fast for small arrays
		/// l: left/starting index within the array where to start sorting
		/// size: number of elements to sort
		/// comparer: optional compare method
		/// </summary>
		public static void SortSmall<T1, T2>(this IList<T1> a, IList<T2> b, Comparison<T1> comparer)
			{
				var equalityComparer = comparer;
				var size = a.Count;
				Debug.Assert(size == b.Count);
			const int l = 0;
				int r = l + size;
				for (int i = l + 1; i < r; i++)
				{
					//if (a[i] < a[i - 1])        // no need to do (j > 0) compare for the first iteration
					if (equalityComparer(a[i], a[i - 1]) < 0)
					{
						T1 a_currentElement = a[i];
						T2 b_currentElement = b[i];
						a[i] = a[i - 1];
						b[i] = b[i - 1];
						int j = i - 1;
						for (; j > l && equalityComparer(a_currentElement, a[j - 1]) < 0; j--)
						{
							a[j] = a[j - 1];
							b[j] = b[j - 1];
						}
						a[j] = a_currentElement;  // always necessary work/write
						b[j] = b_currentElement;
					}
					// Perform no work at all if the first comparison fails - i.e. never assign an element to itself!
				}
			}

		// Double specialization - todo: Make this work with generics only 
		public static unsafe void SortSmall<T2>( this IList<T2> b, Func<T2,double> eval)
		{
			// eval is only called once rather than On^2 times
			int size = b.Count;
			var a = stackalloc double[size];
			for(int i=0;i<size;++i)
			{
				a[i] = eval(b[i]);
			}
			
			const int l = 0;
			int r = l + size;
			for (int i = l + 1; i < r; i++)
			{
				//if (a[i] < a[i - 1])        // no need to do (j > 0) compare for the first iteration
				if ( a[i] < a[i - 1])
				{
					var a_currentElement = a[i];
					var b_currentElement = b[i];
					a[i] = a[i - 1];
					b[i] = b[i - 1];
					var j = i - 1;
					for (; j > l && a_currentElement < a[j - 1]; j--)
					{
						a[j] = a[j - 1];
						b[j] = b[j - 1];
					}
					a[j] = a_currentElement;  // always necessary work/write
					b[j] = b_currentElement;
				}
				// Perform no work at all if the first comparison fails - i.e. never assign an element to itself!
			}
		}


	}
}
