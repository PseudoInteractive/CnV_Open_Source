using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Game
{
	[Flags]
	public  enum Tags 
	{
		RT= 1 << Enum.ttRanger,
		VT = 1 << Enum.ttTriari,
		Vanq = 1 << Enum.ttVanquisher,
		Priest= 1 << Enum.ttPriestess,
		Prae= 1 << Enum.ttPraetor,
		Sorc= 1 << Enum.ttSorcerer,
		Horse= 1 << Enum.ttHorseman,
		Druid = 1 << Enum.ttDruid,
		Arb = 1 << Enum.ttArbalist,
		Scorp= 1 << Enum.ttScorpion,
		Stinger = 1 << Enum.ttStinger,
		Galley = 1 << Enum.ttGalley,
		Warship = 1 << Enum.ttWarship,
		Navy = 1 << 20,
		Shipper= 1 << 21,
		Shipping = 1 << 21,
		Hub = 1 << 22,
		LeaveMe = 1 << 23,
	}
	public struct TagInfo
	{
		public Tags id;
		public string s;
	}
	public static class TagHelper
	{
		public static TagInfo Get(this Tags tag) => new TagInfo() { id = tag, s = tag.ToString() };
		//public static TagInfo tagLeaveMe = Get(Tags.LeaveMe);
		public static bool LeaveMe(this Spot spot) => spot.HasTag(Tags.LeaveMe); 

		public static TagInfo[] tags;
		static TagHelper()
		{
			var names = Tags.GetNames(typeof(Tags));
			tags = new TagInfo[names.Length];
			for(int i=0;i<names.Length;++i)
			{
				tags[i].id = Tags.Parse<COTG.Game.Tags>(names[i]);
				tags[i].s = names[i];
	
			}
			
		}



		public static string SetTag(string src, string tag, bool isOn)
		{
			var exists = src.Contains(tag, StringComparison.OrdinalIgnoreCase);
			if (isOn)
			{
				if (exists)
					return src;
				else
				{
					// add it
					if (src.Length > 0)
						src += " ";
					return src + tag;
				}
			}
			else
			{
				if (exists)
				{
					src = src.Replace(tag + " ", "", StringComparison.OrdinalIgnoreCase);
					src = src.Replace(" " + tag, "", StringComparison.OrdinalIgnoreCase);
					src = src.Replace(tag, "", StringComparison.OrdinalIgnoreCase);
					return src;
				}
				else
					return src;
			}
		}
		public static string SetTag(string src, string tag, bool? isOn)
		{
			if (isOn == null)
				return src;
			return SetTag(src, tag, isOn.GetValueOrDefault());
		}

		public static bool Has(this TagInfo tag, string s)
		{
			return s.Contains(tag.s, StringComparison.OrdinalIgnoreCase);
		}
		public static Tags Get(string src)
		{
			Tags result = default;
			foreach (var t in tags)
			{
				if (src.Contains(t.s, StringComparison.OrdinalIgnoreCase))
				{
					result = result | t.id;
				}
			}
			return result;
		}
		public static Tags GetTags(this Spot city)
		{
			return Get(city.remarks);
		}
	}
}
