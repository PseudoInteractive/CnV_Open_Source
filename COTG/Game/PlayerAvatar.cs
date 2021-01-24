using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Game
{
	public readonly struct PlayerPresence
	{
		public readonly int pid; // playerId is actually an int
		public readonly int cid; // where they are located
		public readonly int lastSeenSec;// { get; set; } // time last seen
										// todo: last action
		public readonly string token;// { get; set; } // token
		public readonly string cookie;// { get; set; } // cookie
		public readonly string name;// => Game.Player.IdToName(pid);

		public DateTimeOffset lastSeen => SmallTime.ToDateTime(lastSeenSec);

		public PlayerPresence(DB.PlayerPresenceDB pp)
		{
			pid = int.Parse(pp.id);
			// Our local session might not have made it to the database, use local data as it will be newer in this case
			if (pid == Player.myId)
			{
				name = Player.myName;
				token = JSClient.jsBase.token;
				cookie = JSClient.jsBase.s;
			}
			else
			{
				name = Game.Player.IdToName(pid);
				token = pp.tk;
				cookie = pp.ck;
			}
			cid = pp.cid;
			lastSeenSec = pp.t;
		}
		public static PlayerPresence[] all = Array.Empty<PlayerPresence>();
	}

}
