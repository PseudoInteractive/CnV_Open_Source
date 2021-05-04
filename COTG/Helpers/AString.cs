using Cysharp.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Helpers
{

	public static class AString
	{
		public static string Wrap(this ReadOnlySpan<char> str, int chunkSize)
		{
			using var sb = ZString.CreateUtf8StringBuilder();
			var lg = str.Length;
			int id = 0;
			for (; ; )
			{
				int spanEnd = (id + chunkSize);
				if (spanEnd >= lg)
				{
					sb.Append(str.Slice(id));
					break;
				}
				while (!char.IsWhiteSpace(str[spanEnd]))
				{
					--spanEnd;
					if (spanEnd <= id + 4)
					{
						// error
						spanEnd = (id + chunkSize);
						break;
					}
				}
				sb.AppendLine(str.Slice(id, spanEnd - id));
				id = spanEnd;
			}
			return sb.ToString();
		}
	}
			
}
