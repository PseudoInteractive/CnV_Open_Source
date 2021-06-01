using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Text;
using EnumsNET;

using static COTG.StringList;

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
		Scout = 1 << Enum.ttScout,
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

		// meta tags
		// mine, alliance enemy other
	}
	public struct TagInfo
	{
		public Tags v;
		public string s;
		public bool isAlias;
		public static TagInfo invalid = new TagInfo() { s = String.Empty };

		public override bool Equals(object obj)
		{
			return obj is TagInfo info &&
				   s == info.s;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(s);
		}

		public static bool operator ==(TagInfo left, TagInfo right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(TagInfo left, TagInfo right)
		{
			return !(left == right);
		}
	}

	public static class TagHelper
	{
		public static TagInfo Get(this Tags tag) => new TagInfo() { v = tag, s = tag.AsString() };
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
				tagsAndAliases[i].v = members[i].Value;
				var isAlias = members[i].Attributes.Has<IsAliasAttribute>();
				tagsAndAliases[i].isAlias = isAlias;
				if(!isAlias)
				{
					_tags.Add(new TagInfo() { v = members[i].Value, s = members[i].Name, isAlias = false });
				}
		
			}
			tags = _tags.ToArray();
			
		}

		

		// removes non set tags, appends tags not present
		static public string ApplyTags(Tags ts, string initial)
		{
			var sb = ZString.CreateStringBuilder();
			var w = new StringListEnumerator(initial );
			bool first = true;
			foreach(var i in w)
			{
				var t = GetTag(i.word);
				bool keep ;

				if (t == TagInfo.invalid)
				{
					keep = true;
				}
				else
				{
					keep = ts.HasFlag(t.v);
					ts &= ~t.v;
				}
				if(keep)
				{
					if (!first)
					{
						if (i.space.IsEmpty)
							sb.Append(" ");
						else
							sb.Append(i.space);
					}
					else
					{
						first = false;
					}
					sb.Append(i.word);
				}
			}
			// append further tags
			foreach(var t in tags)
			{
				if(ts.HasFlag(t.v))
				{
					if(!first)
						sb.Append(" ");
					first = false;

					sb.Append(t.s);
				}

			}
			return sb.ToString();


		}

		//public static string SetTag(string src,Tags tag, bool isOn)
		//{
		//	foreach(var t in tags)
		//	{
		//		if (t.v != tag)
		//			continue;
		//	var exists = src.Contains(tag, StringComparison.OrdinalIgnoreCase);
		//	if (isOn)
		//	{
		//		if (exists)
		//			return src;
		//		else
		//		{
		//			// add it
		//			if (src.Length > 0)
		//				src += " ";
		//			return src + tag;
		//		}
		//	}
		//	else
		//	{
		//		if (exists)
		//		{
		//			src = src.Replace(tag + " ", "", StringComparison.OrdinalIgnoreCase);
		//			src = src.Replace(" " + tag, "", StringComparison.OrdinalIgnoreCase);
		//			src = src.Replace(tag, "", StringComparison.OrdinalIgnoreCase);
		//			return src;
		//		}
		//		else
		//			return src;
		//	}
		//}
		//public static string SetTag(string src, string tag, bool? isOn)
		//{
		//	if (isOn == null)
		//		return src;
		//	return SetTag(src, tag, isOn.GetValueOrDefault());
		//}

		public static bool Has(this TagInfo tag, string s)
		{
			return s.Contains(tag.s, StringComparison.OrdinalIgnoreCase);
		}
		public static bool IsSet(this Tags tag, string s)
		{
			return s.Contains(tag.AsString(), StringComparison.OrdinalIgnoreCase);
		}
		public static TagInfo GetTag(ReadOnlySpan<char> word)
		{
			if (word.Length >= 2)
			{
				word = word.TrimEnd('s');
			}
			foreach (var t in tagsAndAliases)
			{
				if (word.Equals(t.s.AsSpan(), StringComparison.OrdinalIgnoreCase))
				{
					return t;
				}
			}
			return TagInfo.invalid;
		}

		public static Tags Get(string src)
		{
			Tags result = default;
			foreach(var w in new StringListEnumerator(src) )
			{
				var word = w.word;
				if (word.Length >= 2)
				{
					word= word.TrimEnd('s');
				}
				foreach (var t in tagsAndAliases)
				{
					if ( word.Equals(t.s.AsSpan(),StringComparison.OrdinalIgnoreCase))
					{
						result = result | t.v;
						break;
					}
				}

			}

			return result;
		}
		
		public static void UpdateTags(this Spot city)
		{
			city.tags = TagHelper.Get(city.remarks);
		}
		public static Tags GetTags(this Spot city)
		{
			return city.tags;
		}
	}
}
