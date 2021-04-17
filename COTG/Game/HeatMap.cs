using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Game
{
	class HeatMapDay
	{
		public SmallTime lastUpdate;
		public uint[] snapshot; // 600*600
		public HeatMapDelta [] deltas;
	}
	struct HeatMapDelta
	{
		public SmallTime lastUpdate;
		public uint[] deltas;
	}
	public static class HeatMap
	{
	  
	}
}
