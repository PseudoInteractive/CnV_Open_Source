using System;
using System.Diagnostics;
using System.Globalization;

namespace COTG
{
	/// <summary>
	/// 32 bit timestamp
	/// number of seconds since 2018, always in UTC
	/// </summary>
	// Todo: use uint
	[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
	public struct SmallTime : IEquatable<SmallTime>, IComparable<SmallTime>
	{
		public static DateTimeOffset ToDateTime(uint seconds) => DateTimeOffset.FromUnixTimeSeconds((seconds));
		public static uint ToSeconds(DateTimeOffset source) => (uint)(source.ToUnixTimeSeconds() );


		public static readonly DateTimeOffset t0 = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);

		internal const uint t0Seconds = 0;
		// legacy
		internal int secondsI => (int)seconds;
		internal uint seconds;
		internal static SmallTime zero = new SmallTime(0);

		internal DateTimeOffset dateTime => ToDateTime(seconds); // should we convert with localTimeError?

		public SmallTime Date()
		{
			return new SmallTime( (uint)new DateTimeOffset(dateTime.Date,TimeSpan.Zero).ToUnixTimeSeconds());
		}
		public string FormatDefault() => dateTime.FormatDefault();


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
			seconds = (uint)_seconds;
		}
		public SmallTime(uint _seconds)
		{
			seconds = _seconds;
		}
		public static SmallTime serverNow => new SmallTime(JSClient.ServerTimeSeconds());

		public bool isZero => seconds == 0;
		
		public static implicit operator uint(SmallTime t) => t.seconds;
		public static implicit operator  int(SmallTime t) => t.secondsI;
		public static implicit operator SmallTime(DateTimeOffset t) => new SmallTime((uint)t.ToUnixTimeSeconds());
		public static implicit operator SmallTime(uint t) => new SmallTime(t);
		public static implicit operator SmallTime(int t) => new SmallTime(t);

		private string GetDebuggerDisplay()
		{
			return ToString();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is SmallTime t && t.seconds == seconds;
		}

		public override int GetHashCode()
		{
			return (int)seconds;
		}

		public override string ToString()
		{
			return FormatDefault();
		}
		public string ToString(string s)
		{
			return dateTime.ToString(s, CultureInfo.InvariantCulture);
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
