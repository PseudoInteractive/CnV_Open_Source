using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Text;
using EnumsNET;

using static COTG.StringList;
using static COTG.Game.Troops;
using COTG.Services;

namespace COTG.Game
{
	public class IsAliasAttribute : Attribute
	{

	}
	[Flags]
	public  enum Tags 
	{
		// Ones with more than 1 flag should come first
		VRT = (1 << Troops.ttRanger) | (1 << Troops.ttTriari) | (1 << Troops.ttVanquisher),
		RT = (1 << Troops.ttRanger)| (1 << Troops.ttTriari),
		VT = (1 << Troops.ttVanquisher) | (1 << Troops.ttTriari),
		[IsAlias]
		ranger=RT,
		[IsAlias]
		triari=RT, // todo: fix this
		Vanq = 1 << Troops.ttVanquisher,
		[IsAlias]
		Vanquisher = Vanq,
		Priest = 1 << Troops.ttPriestess,
		[IsAlias]
		Priestess = Priest,
		Prae = 1 << Troops.ttPraetor,
		[IsAlias]
		Praetor = Prae,
		Sorc = 1 << Troops.ttSorcerer,
		[IsAlias]
		Sorceress = Sorc,
		[IsAlias]
		Mage = Sorc,
		Horse = 1 << Troops.ttHorseman,
		[IsAlias]
		Knight =  Horse,
		Druid = 1 << Troops.ttDruid,
		Arb = 1 << Troops.ttArbalist,
		[IsAlias]
		Arbalist = Arb,
		Scout = 1 << Troops.ttScout,
		Scorp = 1 << Troops.ttScorpion,
		[IsAlias]
		Scorpion = Scorp,
		[IsAlias]
		cat = Scorp,
		[IsAlias]
		catapult = Scorp,
		[IsAlias]
		cataram = Scorp,
		Stinger = 1 << Troops.ttStinger,
		[IsAlias]
		Sloop = Stinger,
		Galley = 1 << Troops.ttGalley,
		Warship = 1 << Troops.ttWarship,
		HLT = 1<<20,
		Shipper= 1 << 21,
		[IsAlias]
		Shipping = Shipper,
		Hub = 1 << 22,
		LeaveMe = 1 << 23,
		Storage = 1<< 24,
		Transport = 1 << 25,
		SevenPoint = 1 << 26,
		Jumper = 1 << 27,
		Portal = 1 << 28,
		// meta tags
		// mine, alliance enemy other
	}
	public struct TagInfo : IEquatable<TagInfo>
	{
		public Tags v;
		public string s;
		public bool isAlias;
		public static TagInfo invalid = new TagInfo() { s = String.Empty };

		public readonly override bool Equals(object obj)
		{
			return obj is TagInfo info &&(this == info);
		}

		public readonly override int GetHashCode()
		{
			return HashCode.Combine(s);
		}

		public bool Equals(TagInfo other)
		{
			return v == other.v && isAlias == other.isAlias && s == other.s;
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
		public static TagInfo Get(this Tags tag) => new TagInfo() { v = tag,s = tag.AsString() };
		public static Tags TroopTypeTag(int tt) => (Tags)(1<<tt);
		public static bool HasTroopTypeTag(this Tags _t,int tt) {return _t.HasFlag(TroopTypeTag(tt));}
		public const Tags TroopMask = (Tags)((1 << ttCount) - 1);
		public const Tags MilitaryTroopMask = TroopMask & (Tags)(~((1 << ttSenator)|(1<<ttGuard)) );
		public static Tags Troops(this Tags tag) => tag & TroopMask;
		public static Tags MilitaryTroops(this Tags tag) => tag & MilitaryTroopMask;

		//public static TagInfo tagLeaveMe = Get(Tags.LeaveMe);
		public static Tags FromTroopType(byte troopType, Tags baseTags)
		{
			baseTags &= ~TroopMask;
			switch(troopType)
			{
				case ttVanquisher: return baseTags|Tags.Vanq;
				case ttTriari: 
				case ttRanger:return baseTags | Tags.RT;
				case ttPriestess:  return baseTags | Tags.Priest;
				case ttSorcerer: return baseTags | Tags.Sorc;
				case ttScout: return baseTags | Tags.Scout;
				case ttHorseman: return baseTags | Tags.Horse;
				case ttArbalist: return baseTags | Tags.Arb;
				case ttPraetor: return baseTags | Tags.Praetor;
				case ttDruid: return baseTags | Tags.Druid;
				case ttScorpion:
				case ttRam: return baseTags | Tags.Scorp;
				case ttGalley: return baseTags | Tags.Galley;
				case ttStinger: return baseTags | Tags.Stinger;
				case ttWarship: return baseTags | Tags.Warship;
				default: return baseTags;
			}
		}

		public static TagInfo[] tagsWithoutAliases;
		public static TagInfo[] tagsAndAliases;
		static TagHelper()
		{

			var members = Enums.GetMembers<Tags>().OrderByDescending(a=>a).ToArray();
			var count = members.Length;
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
			tagsWithoutAliases = _tags.ToArray();
			//Assert(tagsWithoutAliases.IndexOf(Tags.VT) < tagsWithoutAliases.IndexOf(Tags.Vanquisher)); 
			
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
			// append tags not included
			foreach(var t in tagsWithoutAliases)
			{
				if(ts.HasFlag(t.v))
				{
					if(!first)
						sb.Append(" ");
					first = false;
					ts &= ~t.v; // remove it, no longer needed
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
			city.tags |= TagHelper.Get(city.remarks);
		}
		public static async Task<Tags> TouchTags(this Spot city)
		{
			await GetCity.Post(city.cid);
			city.UpdateTags();
			return city.tags;
			
		}
		public static Tags GetTags(this Spot city)
		{
			return city.tags;
		}
	}
}
