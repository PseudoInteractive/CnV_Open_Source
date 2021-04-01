using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Game
{
	class PlayerStats
	{
		public int pid;
		public int cities;
	}


	class PlayerStatSnapshot
	{
		public DateTime dateTime;
		public PlayerStats[] playerStats;

	}

}
