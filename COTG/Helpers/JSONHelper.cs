using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using static COTG.Debug;
using System.Globalization;

namespace COTG.Helpers
{
    public static class JSONHelper
    {
        public static bool IsValid(this JsonElement j)
        {
            return j.ValueKind != JsonValueKind.Undefined;
        }
        public static int GetAsInt(this JsonElement js, string prop)
        {
            return (int)GetAsInt64(js, prop);
        }
        public static short GetAsShort(this JsonElement js, string prop)
        {
            return (short)(GetAsInt64(js, prop));
        }
        public static ushort GetAsUShort(this JsonElement js, string prop)
        {
            return (ushort)(GetAsInt64(js, prop));
        }
        public static byte GetAsByte(this JsonElement js, string prop)
        {
            return (byte)(GetAsInt64(js, prop));
        }
        public static sbyte GetAsSByte(this JsonElement js, string prop)
        {
            return (sbyte)(GetAsInt64(js, prop));
        }
        public static float GetAsFloat(this JsonElement js, string prop)
        {
            if (!js.IsValid())
                return -1;
            if (!js.TryGetProperty(prop, out var e))
            {
                Log("Missing " + prop);
                return -1;
            }
            switch (e.ValueKind)
            {
                case JsonValueKind.String:
                    return float.TryParse(e.GetString(), out var v) ? v : -1;
                case JsonValueKind.Number:
                    return e.GetSingle();
                case JsonValueKind.True:
                    return 1;
                case JsonValueKind.False:
                    return 0;
                case JsonValueKind.Array:
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                case JsonValueKind.Object:
                default:
                    Log("Invalid Json Type " + e.ValueKind);
                    return -1;
            }
        }
        public static long GetAsInt64(this JsonElement js, string prop)
        {
            if (!js.IsValid())
                return -1;
            if (!js.TryGetProperty(prop, out var e))
            {
                Log("Missing " + prop);
                return -1;
            }

            switch (e.ValueKind)
            {
                case JsonValueKind.String:
                    return long.TryParse(e.GetString(), out var v) ? v : -1;
                case JsonValueKind.Number:
                    return e.GetInt64();
                case JsonValueKind.True:
                    return 1;
                case JsonValueKind.False:
                    return 0;
                case JsonValueKind.Array:
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                case JsonValueKind.Object:
                default:
                    return -1;
            }
        }
        public static int GetInt(this JsonElement js, string prop)
        {
            if (!js.TryGetProperty(prop, out var e))
            {
                Log("Missing " + prop);
                return 0;
            }
            return e.GetInt32();

        }
        public static string GetString(this JsonElement js, string prop)
        {
            if (!js.TryGetProperty(prop, out var e))
            {
                Log("Missing " + prop);
                return null;
            }
            return e.GetString();

        }
        public static string GetAsString(this JsonElement js, string prop)
        {
            if (!js.IsValid())
                return "null";
            if (!js.TryGetProperty(prop, out var e))
            {
                Log("Missing " + prop);
                return "null";
            }
            return e.ToString();

        }
        public static string GetAsString(this JsonElement js)
        {
            return js.ToString();
        }
        public static float GetFloat(this JsonElement js, string prop)
        {
            if (!js.TryGetProperty(prop, out var e))
            {
                Log("Missing " + prop);
                return 0;
            }
            return e.GetSingle();

        }

            public static string Truncate(this string value, int maxLength)
            {
                if (string.IsNullOrEmpty(value)) return value;
                return value.Length <= maxLength ? value : value.Substring(0, maxLength);
            }
       public static int RoundToInt(this float f)
        {
            return f >= 0 ? (int)(f + 0.5f) : -( (int)(-f + 0.5f) );
        }
        public static int RoundToInt(this double f)
        {
            return f >= 0 ? (int)(f + 0.5f) : -((int)(-f + 0.5f));
        }
        public static string timeZoneString=string.Empty;
        public static DateTimeOffset ParseDateTime(this string src)
        {
            var format = "H:mm:ss dd/MM/yyyy";
            var s = src; // src may be missing the date or year
            var serverTime = JSClient.ServerTime();
            var dateMarker = s.IndexOf('/');
            if (dateMarker <= 0)
            {
                s = $"{s} {serverTime.Day:D2}/{serverTime.Month:D2}/{serverTime.Year}";
            }
            else
            {
                if( s.IndexOf('/',dateMarker+1) <= 0)
                {
                    s = $"{s}/{serverTime.Year}";

                }
            }
            return DateTimeOffset.ParseExact(s ,format, CultureInfo.InvariantCulture, DateTimeStyles.AllowInnerWhite|DateTimeStyles.AssumeUniversal);
        }
    }
}
