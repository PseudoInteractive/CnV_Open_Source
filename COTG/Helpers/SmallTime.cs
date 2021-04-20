using System;
using System.Diagnostics;

namespace COTG
{
	/// <summary>
	/// 32 bit timestamp
	/// number of seconds since 2018, always in UTC
	/// </summary>
	[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
	public struct SmallTime : IEquatable<SmallTime>, IComparable<SmallTime>
	{
		public static DateTimeOffset ToDateTime(int seconds) => DateTimeOffset.FromUnixTimeSeconds((seconds));
		public static int ToSeconds(DateTimeOffset source) => (int)(source.ToUnixTimeSeconds() );


		internal static readonly DateTimeOffset t0 = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
		internal const int t0Seconds = 0;
		internal int seconds;
		internal DateTimeOffset dateTime => ToDateTime(seconds); // should we convert with localTimeError?

		public SmallTime Date()
		{
			return new SmallTime( (int)new DateTimeOffset(dateTime.Date,TimeSpan.Zero).ToUnixTimeMilliseconds());
		}
		/// <summary>
		/// Does not correct for invalid system time
		/// </summary>
		/// <param name="source"></param>
		public SmallTime(DateTimeOffset source)
		{
			seconds = ToSeconds(source);
		}
		public SmallTime(int _seconds)
		{
			seconds = _seconds;
		}
		public static int serverNow => JSClient.ServerTimeSeconds();
		public static implicit operator int(SmallTime t) => t.seconds;

		private string GetDebuggerDisplay()
		{
			return ToString();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return seconds;
		}

		public override string ToString()
		{
			return dateTime.ToString("u");
		}
		public string ToString(string s)
		{
			return dateTime.ToString(s);
		}

		public bool Equals(SmallTime other)
		{
			return seconds == other.seconds;
		}

		int IComparable<SmallTime>.CompareTo(SmallTime other)
		{
			return seconds.CompareTo(other.seconds);
		}


		//		public  static implicit operator int (SmallTime t) => (int)t.seconds;
	}

}
