﻿using System;
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
	}
	public struct TagInfo
	{
		public Tags id;
		public string s;
	}
	public static class TagHelper
	{
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
