using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Game
{
	[Flags]
	public  enum Tag 
	{
		none= 0,
		RT= 1 << Enum.ttRanger,
		Vanq= 1 << Enum.ttVanquisher,
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
		Shipping= 1 << 21,
		Hub = 1 << 22,
	}

	public static class TagsHelper
	{

	}
}
