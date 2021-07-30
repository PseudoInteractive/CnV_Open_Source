
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
namespace COTG
{
	public class Json
	{
		public static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions() 
		{ 
			AllowTrailingCommas = true, 
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, 
			NumberHandling = JsonNumberHandling.AllowReadingFromString|JsonNumberHandling.AllowNamedFloatingPointLiterals, 
			ReadCommentHandling = JsonCommentHandling.Skip,
			IgnoreReadOnlyProperties=true
		};
		public static T FromResources<T>(string asm)
		{
			var str = new System.IO.StreamReader(typeof(JSClient).Assembly.GetManifestResourceStream($"COTG.JSON.{asm}.json")).ReadToEnd();
			return JsonSerializer.Deserialize<T>(str, jsonSerializerOptions);

		}
	}
}
