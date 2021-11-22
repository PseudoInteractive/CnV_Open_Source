using COTG.Game;

using DSharpPlus.CommandsNext;

namespace CnVDiscord
{

	public class GreetPlayerEventArgs
	{
	}
	public class PlayerHooks
	{
		public class PlayerCommandEventArgs
		{
			public Player Player;
			public string CommandText;
		}
		public class PlayerLogoutEventArgs { }
		public class PlayerChatEventArgs
		{
			public Player player;

			public string text;
			public CommandContext context;
			public ulong discordChannelId;
		}

		public static System.Action<PlayerCommandEventArgs> PlayerCommand;
		internal static System.Action<PlayerLogoutEventArgs> PlayerLogout;
		internal static System.Action<PlayerChatEventArgs> PlayerChat;
	}


}