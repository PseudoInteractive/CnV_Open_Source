using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace COTG.JSON
{
	public class Json
	{
		public static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions() { AllowTrailingCommas = true, IgnoreNullValues = true, NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString };
		public static T FromResources<T>(string asm)
		{
			return JsonSerializer.Deserialize<T>(new System.IO.StreamReader((typeof(JSClient).Assembly).GetManifestResourceStream($"COTG.JSON.{asm}.json")).ReadToEnd(), jsonSerializerOptions);

		}
	}
}
