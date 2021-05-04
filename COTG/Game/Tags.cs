using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnumsNET;

namespace COTG.Game
{
	public class IsAliasAttribute : Attribute
	{

	}
	[Flags]
	public  enum Tags 
	{
		RT= 1 << Enum.ttRanger,
		[IsAlias]
		ranger=RT,
		[IsAlias]
		triari=RT,
		VT = 1 << Enum.ttTriari,
		Vanq = 1 << Enum.ttVanquisher,
		[IsAlias]
		Vanquisher = Vanq,
		Priest = 1 << Enum.ttPriestess,
		[IsAlias]
		Priestess = Priest,
		Prae = 1 << Enum.ttPraetor,
		[IsAlias]
		Praetor = Prae,
		Sorc = 1 << Enum.ttSorcerer,
		[IsAlias]
		Sorceress = Sorc,
		[IsAlias]
		Mage = Sorc,
		Horse = 1 << Enum.ttHorseman,
		[IsAlias]
		Knight =  Horse,
		Druid = 1 << Enum.ttDruid,
		Arb = 1 << Enum.ttArbalist,
		[IsAlias]
		Arbalist = Arb,
		Scorp = 1 << Enum.ttScorpion,
		[IsAlias]
		Scorpion = Scorp,
		[IsAlias]
		cat = Scorp,
		[IsAlias]
		catapult = Scorp,
		[IsAlias]
		cataram = Scorp,
		Stinger = 1 << Enum.ttStinger,
		[IsAlias]
		Sloop = Stinger,
		Galley = 1 << Enum.ttGalley,
		Warship = 1 << Enum.ttWarship,
		Shipper= 1 << 21,
		[IsAlias]
		Shipping = 1 << 21,
		Hub = 1 << 22,
		LeaveMe = 1 << 23,
		Storage = 1<< 24,
		Transport = 1 << 25,
		VRT = 1 << 26,

	}
	public struct TagInfo
	{
		public Tags id;
		public bool isAlias;
		public string s;
	}
	public static class TagHelper
	{
		public static TagInfo Get(this Tags tag) => new TagInfo() { id = tag, s = tag.AsString() };
		//public static TagInfo tagLeaveMe = Get(Tags.LeaveMe);

		public static TagInfo[] tags;
		public static TagInfo[] tagsAndAliases;
		static TagHelper()
		{

			var members = Enums.GetMembers<Tags>();
			var count = members.Count;
			var _tags = new List<TagInfo>();
			tagsAndAliases = new TagInfo[count];

			for(int i=0;i<count;++i)
			{
				tagsAndAliases[i].s = members[i].Name;
				tagsAndAliases[i].id = members[i].Value;
				var isAlias = members[i].Attributes.Has<IsAliasAttribute>();
				tagsAndAliases[i].isAlias = isAlias;
				if(!isAlias)
				{
					_tags.Add(new TagInfo() { id = members[i].Value, s = members[i].Name, isAlias = false });
				}
		
			}
			tags = _tags.ToArray();
			
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
		public static bool IsSet(this Tags tag, string s)
		{
			return s.Contains(tag.AsString(), StringComparison.OrdinalIgnoreCase);
		}

		public static Tags Get(string src)
		{
			Tags result = default;
			foreach(var _word in src.EnumerateWords() )
			{
				var word = _word;
				if (word.Length >= 2)
				{
					word= word.TrimEnd('s');
				}
				foreach (var t in tagsAndAliases)
				{
					if ( word.Equals(t.s.AsSpan(),StringComparison.OrdinalIgnoreCase))
					{
						result = result | t.id;
						break;
					}
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
