using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StringSpan = System.ReadOnlySpan<char>;
using static COTG.Debug;

namespace COTG
{
	public static class StringHelper
	{
		public static StringList GetWords(this StringSpan s)
		{
			StringList rv = new StringList() { Count = 0, s = s };
			var i1 = 0;
			for (; ; )
			{
				var ip = i1;
				var i0 = ip;
				for (; ; )
				{
					if (i0 >= s.Length)
						return rv;
					var c = s[i0];
					if (char.IsLetter(c))
						break;
					++i0;
				}
				i1 = i0 + 1;
				for (; ; )
				{
					if (i1 >= s.Length || !char.IsLetter(s[i1]))
					{
						rv.Add(ip, i0);
						rv.Add(i0, i1);
						break;
					}
					++i1;
				}
			}
		}
		public static WordEnumerator EnumerateWords(this string s) => new WordEnumerator(s);
	}
	public unsafe ref struct StringList
	{
		const int maxSize = 128;
		
		public fixed int ranges[maxSize*2];
		public int Count;
		public StringSpan s;
		public StringList(StringSpan s) { this.s = s; Count = 0; }
		public  StringSpan this[int id]
		{
			get
			{
				Assert(id >= 0);
				Assert(id < Count);

				return s.Slice(ranges[id*2],ranges[id*2+1] );
			}
		}
		public void Add( int i0, int i1)
		{
			Assert(Count < maxSize);
			ranges[Count * 2] = i0;
			ranges[Count * 2+1] = i1;
			++Count;

		}
	}

	public ref struct WordEnumerator
	{
		private readonly StringSpan s;
		private int i0;
		private int i1;

		public WordEnumerator( string _s) 
		{
			s = _s.AsSpan();
			(i0, i1) = (0, -1);
		}
		public WordEnumerator GetEnumerator() => this;
		public readonly StringSpan Current => s.Slice(i0, i1-i0);
		
		public bool MoveNext()
		{
		
			
			i0 = i1+1;
			for(; ; )
			{
				if (i0 >= s.Length)
					return false;
				var c = s[i0];
				if ( char.IsLetter(c))
					break;
				++i0;
			}
			i1 = i0+1;
			for (; ; )
			{
				if (i1 >= s.Length || !char.IsLetter(s[i1]) )
					return true;
				++i1;
			}
		}
	}
}
