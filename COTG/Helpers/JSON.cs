using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using static COTG.Debug;
using System.Globalization;
using System.Numerics;

namespace COTG.Helpers
{
	public static class JSON
    {
        public static string ToJson<T>(T a) => JsonSerializer.Serialize(a, Json.jsonSerializerOptions);
        public static T FromJson<T>(string a) => JsonSerializer.Deserialize<T>(a, Json.jsonSerializerOptions);

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
		public static byte GetAsByte(this JsonElement js)
		{
			return (byte)(GetAsInt64(js));
		}
		public static sbyte GetAsSByte(this JsonElement js, string prop)
        {
            return (sbyte)(GetAsInt64(js, prop));
        }
        public static float GetAsFloat(this JsonElement js)
        {
            switch (js.ValueKind)
            {
                case JsonValueKind.String:
                    {
                        // special case: "-"
                        var str = js.GetString();
                        if (str.Length == 1 && str[0] == '-')
                            return 0;

                        return float.TryParse(str, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out var v) ? v : default;
                    }
                case JsonValueKind.Number:
                    {
                        return js.TryGetSingle(out var v) ? v : default;
                    }
                case JsonValueKind.True:
                    return 1;
                case JsonValueKind.False:
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return 0;
                case JsonValueKind.Array:
                case JsonValueKind.Object:
                default:
                    Log("Invalid Json Type " + js.ValueKind);
                    return -1;
            }
        }

        static public IEnumerable<JsonElement> EnumerateArrayOrObject(this JsonElement e)
        {
            if (e.ValueKind == JsonValueKind.Array)
                return e.EnumerateArray();
            else if(e.ValueKind == JsonValueKind.Object)
            {
                var rv = new List<JsonElement>();
                foreach (var i in e.EnumerateObject())
                {
                    rv.Add(i.Value);
                }
                return rv;

            }
            else
            {
      //          Log($"Not array or object {e.ToString()}");
                return Array.Empty<JsonElement>();
            }

        }

        public static float GetAsFloat(this JsonElement js, string prop)
        {
            if (!js.IsValid())
                return -1;
            if (!js.TryGetProperty(prop, out var e))
            {
                Log("Missing " + prop);
                return 0;
            }
            return GetAsFloat(e);
        }
        public static long GetAsInt64(this JsonElement js, string prop,bool verbose=false)
        {
            if (!js.IsValid())
                return -1;
            if (!js.TryGetProperty(prop, out var e))
            {
                if(verbose)
                    Log("Missing " + prop);
                return 0;
            }
            return GetAsInt64(e);
        }
        public static long GetAsInt64(this JsonElement js)
        {

            switch (js.ValueKind)
            {
                case JsonValueKind.String:
                    {
                        // special case: "-"
                        var str = js.GetString();
                        if (str.Length == 1 && str[0] == '-')
                            return 0;
                        return long.TryParse(str, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out var v) ? v : default;
                    }
                case JsonValueKind.Number:
                    {
                        if (js.TryGetInt64(out var rv))
                            return rv;
                        if (js.TryGetDouble(out var d))
                            return (long)d;
                        return -1;
                    }
                case JsonValueKind.True:
                    return 1;
                case JsonValueKind.False:
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return 0;
                case JsonValueKind.Array:
                case JsonValueKind.Object:
                default:
                    return -1;
            }
        }
        public static int GetAsInt(this JsonElement js)
        {
            return (int)GetAsInt64(js);
        }
        public static int GetInt(this JsonElement js, string prop,bool verbose=false)
        {
            if (!js.TryGetProperty(prop, out var e))
            {
                if(verbose)
                    Log("Missing " + prop);
                return 0;
            }
            return e.GetInt32();

        }
		public static unsafe Microsoft.Xna.Framework.Color GetColor(this JsonElement js, string prop)
		{
			if (!js.TryGetProperty(prop, out var e))
			{
				Log("Missing " + prop);
				return new Microsoft.Xna.Framework.Color();
			}

			var s = e.GetString();
			int get = 0;
			if (s[get] == '#')
				++get;
			byte * rgba = stackalloc byte[4];
			rgba[3] = 255;
			for(int i=0;i<4;++i)
			{
				if (get >= s.Length)
					break;
				var str = s[get + 0].ToString() + s[get + 1].ToString();
				get += 2;
				rgba[i] = (byte)int.Parse(str, NumberStyles.HexNumber);
			}
			return new Microsoft.Xna.Framework.Color(rgba[0], rgba[1], rgba[2], rgba[3]);
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
     
        public static string timeZoneString=string.Empty;
        public static DateTimeOffset ParseDateTime(this string src, bool monthThenDay=true)
        {
            var format = "s";
            var serverTime = JSClient.ServerTime();
            var split = src.Split(' ', StringSplitOptions.RemoveEmptyEntries) ;
            string s;
            if (split.Length == 1)
            {
                s = $"{serverTime.Year}-{serverTime.Month:D2}-{serverTime.Day:D2}T{split[0]}";

            }
            else
            {
                var dateEtc = split[1].Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (dateEtc.Length == 1)
                {
                    // only day
                    s = $"{serverTime.Year}-{serverTime.Month:D2}-{int.Parse(dateEtc[0]):D2}T{split[0]}";
                }
                else if (dateEtc.Length == 2)
                {
                    // month then day
                    s = $"{serverTime.Year}-{int.Parse(dateEtc[monthThenDay ? 0 : 1]):D2}-{int.Parse(dateEtc[monthThenDay ? 1 : 0]):D2}T{split[0]}";
                }
                else
                {
                    // month then day
                    s = $"{dateEtc[2]}-{int.Parse(dateEtc[monthThenDay ? 0 : 1]):D2}-{int.Parse(dateEtc[monthThenDay ? 1 : 0]):D2}T{split[0]}";
                }
            }

            if (DateTimeOffset.TryParseExact(s, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AssumeUniversal, out var rv))
                return rv;
            if (DateTimeOffset.TryParse(s, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AssumeUniversal, out rv))
                return rv;
            Assert(false);
            return AUtil.dateTimeZero;

        }
    }
}
