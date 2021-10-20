global using TickT = System.Int64; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG
{
	public static class ATime
	{
		public static TickT TickCount => Environment.TickCount64;

	}
}
