using CnV;

using CnV.Game;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCnV
{
	

	public static partial class DGame
	{
		public struct Message
		{
			public string username { get; set; }// "Cord Claim",
			public string avatar_url { get; set; } //: "",
			public string content { get; set; } //: message + " claimed by " + name
		}
		public static Uri discordHook = new Uri("https://discord.com/api/webhooks/766166495411437578/xwt4o5KsEjh7MVe3xLluez9Q6keeCjdYOpfBsgBXJriK8WtNUnYvNtKI9IUIqt0hbJbM");
		public static Uri discordIncomingHook = new Uri("https://discord.com/api/webhooks/871564387691946004/L-MAmP_lFR033KeZL6By0OCZDG01aKfegaj49_F15ZSNRZ-XzD9HmIkaYA8-Q6mg_Pvl");

		public static bool isValidForIncomingNotes 	=>	CnVServer.world == 23 && (Alliance.MyId == 42 ); // XMen
			
			
    }
}
