using COTG.Services;

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
		public readonly uint lastSeenSec;// { get; set; } // time last seen
										// todo: last action
		public readonly string token;// { get; set; } // token
		public readonly string cookies;// { get; set; } // cookie
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
				cookies = JSClient.jsBase.cookies;
			}
			else
			{
				name = Game.Player.IdToName(pid);
				token = pp.tk;
				cookies = pp.ck;
			}
			cid = pp.cid;
			lastSeenSec = (uint)pp.t;
		}
		public static PlayerPresence[] all = Array.Empty<PlayerPresence>();
	}
	public struct Friend
	{
		public int pid;
		public string name;
		public int online;
		public int flag1;
		public int flag2;
		public static Friend[] all = Array.Empty<Friend>();

		public static async Task LoadAll()
		{
			//try
			//{
			//	var post = Post.SendForJson("includes/gfrnd.php");
			//	var dat = (await post);
			//	if (dat != null)
			//	{
			//		var jse = dat.RootElement;
			//		var count = jse.GetArrayLength();
			//		all = new Friend[count];
			//		for (int i = 0; i < count; ++i)
			//		{
			//			var jsf = jse[i];
			//			all[i].name = jsf[2].GetString();
			//			all[i].pid = jsf[4].GetInt32();
			//			all[i].online = jsf[0].GetInt32();
			//			all[i].flag1 = jsf[1].GetInt32();
			//			all[i].flag2 = jsf[3].GetInt32();
			//		}
			//	}
			//}
			//catch (Exception e)
			//{
			//	Debug.LogEx(e);
			//}
		}
	}

}
