using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;


namespace COTG.Helpers
{
    public static class JSONHelper
    {
    public static int GetAsInt(this JsonElement js, string prop)
    {
            if (!js.TryGetProperty(prop, out var e))
                return 0;

        switch (e.ValueKind)
        {
            case JsonValueKind.String:
                return int.Parse(e.GetString());
            case JsonValueKind.Number:
                return e.GetInt32();
            case JsonValueKind.True:
                return 1;
            case JsonValueKind.Array:
            case JsonValueKind.False:
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
            case JsonValueKind.Object:
            default:
                return 0;
        }
    }
        public static int GetInt(this JsonElement js, string prop)
        {
            if (!js.TryGetProperty(prop, out var e))
                return 0;
            return e.GetInt32();

        }
        public static string GetString(this JsonElement js, string prop)
        {
            if (!js.TryGetProperty(prop, out var e))
                return null;
            return e.GetString();

        }
        public static float GetFloat(this JsonElement js, string prop)
        {
            if (!js.TryGetProperty(prop, out var e))
                return 0;
            return e.GetSingle();

        }
    }
}
