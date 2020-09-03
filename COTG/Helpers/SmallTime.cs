using System;

namespace COTG
{
    /// <summary>
    /// 32 bit timestamp
    /// number of seconds since 2018, always in UTC
    /// </summary>
    public struct SmallTime
    {
        internal static readonly DateTimeOffset t0 = new DateTimeOffset(2020, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
        internal int seconds;

        internal DateTimeOffset dateTime => t0.AddSeconds(seconds); // should we convert with localTimeError?

        /// <summary>
        /// Does not correct for invalid system time
        /// </summary>
        /// <param name="source"></param>
        public SmallTime(DateTimeOffset source)
        {
            seconds = (int)source.Subtract(t0).TotalSeconds;
        }
        /*	internal SmallTime(uint _seconds = 0)
            {
                seconds=_seconds;
            }*/
        public SmallTime(int _seconds)
        {
            seconds = _seconds;
        }
        public static int Now() => (((int)JSClient.ServerTime().Subtract(t0).TotalSeconds));

        public static implicit operator int(SmallTime t) => t.seconds;
        //		public  static implicit operator int (SmallTime t) => (int)t.seconds;
    }

}
