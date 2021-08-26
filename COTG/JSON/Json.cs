
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Windows.Storage;

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
		public static async Task<T> FromContent<T>(string asm)
		{
			var file = await ApplicationData.Current.LocalFolder.GetFileAsync($"ms-appx:///JSON/{asm}.json");
			return JsonSerializer.Deserialize<T>(await FileIO.ReadTextAsync(file), jsonSerializerOptions);

		}
	}
}
