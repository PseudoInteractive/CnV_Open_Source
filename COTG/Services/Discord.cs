using COTG.Game;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.Services
{
	public static partial class Discord
	{
		public struct Message
		{
			public string username { get; set; }// "Cord Claim",
			public string avatar_url { get; set; } //: "",
			public string content { get; set; } //: message + " claimed by " + name
		}
		public static Uri discordHook = new Uri("https://discord.com/api/webhooks/766166495411437578/xwt4o5KsEjh7MVe3xLluez9Q6keeCjdYOpfBsgBXJriK8WtNUnYvNtKI9IUIqt0hbJbM");
		public static Uri discordIncomingHook = new Uri("https://discord.com/api/webhooks/781739168987611137/IlkuOfXCiewDuR3OaQUQnSaDCAgaMjOAMpyhqNquBC5Qb4UU8P8DhxEhzsiwVfIpfYcq");
		public static bool isValid 	=>	JSClient.world == 21 && (Alliance.MyId == 131 || Alliance.MyId == 132 || Alliance.MyId == 212);
			
			
    }
}
