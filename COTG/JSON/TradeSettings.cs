using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using J = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace COTG.JSON
{
	public class TradeSettings
	{
		[J("28")] public int sourceWood { get; set; }
		[J("29")] public int sourceStone { get; set; }
		[J("30")] public int sourceIron { get; set; }
		[J("31")] public int sourceFood { get; set; }
		[J("32")] public int useSingleSource { get; set; }
		[J("33")] public int requestWood { get; set; }
		[J("34")] public int requestStone { get; set; }
		[J("35")] public int requestIron { get; set; }
		[J("36")] public int requestFood { get; set; }
		[J("37")] public int destWood { get; set; }
		[J("38")] public int destStone { get; set; }
		[J("39")] public int destIron { get; set; }
		[J("40")] public int destFood { get; set; }
		//	[J("41")] public long The41 { get; set; }
		[J("42")] public int sourceHub { get; set; }
		//	[J("43")] [JsonConverter(typeof(ParseStringConverter))] public long The43 { get; set; }
		//	[J("44")] public long The44 { get; set; }
		//	[J("45")] [JsonConverter(typeof(ParseStringConverter))] public long The45 { get; set; }
		//	[J("46")] [JsonConverter(typeof(ParseStringConverter))] public long The46 { get; set; }
		[J("47")] public int sendWood { get; set; }
		[J("48")] public int sendStone { get; set; }
		[J("49")] public int sendIron { get; set; }
		[J("50")] public int sendFood { get; set; }
		[J("n")] public string N { get; set; }

		static public TradeSettings[] all;
	}
}
