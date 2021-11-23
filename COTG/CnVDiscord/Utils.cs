using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;

//using Terraria;

//using TShockAPI;
//using TShockAPI.DB;

namespace CnVDiscord
{
    class Utils
    {
//        static IDbConnection database;
        public Utils()
        {
        }

   //     public static async void ReloadConfig()
   //     {
   //         await Discord.DiscordBot.DisconnectAsync();

			//Discord.DiscordBot = new DiscordClient(new DiscordConfiguration
			//{
			//	Token = Discord.Config.DiscordBotToken,
			//	TokenType = TokenType.Bot,
			//	MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Warning
			//});

   //         await Discord.DiscordBot.ConnectAsync();
   //     }

   //     public static async void OnPostInitialize(EventArgs args)
   //     {
   //         await Online();
   //         //Bridge.AutoBC();
   //     }

   //     public static async Task Online()
   //     {
   //         var chat = await Discord.DiscordBot.GetChannelAsync(Discord.Config.LogID);
   //         await Discord.DiscordBot.SendMessageAsync(chat, $"✅ Server online!");
   //     }
   //     public async void OnServerCommand(CommandEventArgs args)
   //     {
   //         var chat = await Discord.DiscordBot.GetChannelAsync(Discord.Config.LogID);

   //         await Discord.DiscordBot.SendMessageAsync(chat, "**Server:** " + args.Command);
   //     }
   //     //public async void OnPlayerCommand(TShockAPI.Hooks.PlayerCommandEventArgs args)
   //     //{
   //     //    if ((args.Player == null) || (args.Player == TSPlayer.Server))
   //     //        return;
   //     //    try
   //     //    {
   //     //        var logs = await Discord.DiscordBot.GetChannelAsync(Discord.Config.LogID);
   //     //        await Discord.DiscordBot.SendMessageAsync(logs, "**" + args.Player.Name + ":** " + args.CommandText);
   //     //    }
   //     //    catch (Exception ex)
   //     //    {
   //     //        TShock.Log.Error(ex.ToString());
   //     //    }
   //     //}

    }
}
