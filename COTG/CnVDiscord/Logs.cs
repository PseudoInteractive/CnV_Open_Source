using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text.RegularExpressions;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;

using System.Data;
using DSharpPlus.CommandsNext;

namespace CnVDiscord
{
	
	class Logs
    {


		//public static async void JoinLeave(Player player, bool join = true)
  //      {
  //          if (player == null)
  //              return;

  //          try
  //          {
  //              var ch = await Discord.DiscordBot.GetChannelAsync(Discord.Config.JoinLogID);
  //              string msg;
  //              DiscordEmoji emoji;

  //              if (join == false)
  //              {
  //                  emoji = DiscordEmoji.FromName(Discord.DiscordBot, ":x:");
  //                  msg = "** left the server.";
  //              }
  //              else
  //              {
  //                  emoji = DiscordEmoji.FromName(Discord.DiscordBot, ":arrow_right:");
  //                  msg = "** joined the server.";
  //              }

  //              await Discord.DiscordBot.SendMessageAsync(ch, emoji + " **" + player.Name + msg);
  //          }
  //          catch (Exception ex)
  //          {
  //              Log.logger.Error(ex.ToString());
  //          }

  //          return;
  //      }

        public static async Task Goodbye()
        {
            var chat = await Discord.DiscordBot.GetChannelAsync(Discord.Config.ChatID);
            await Discord.DiscordBot.SendMessageAsync(chat, "🛠 **Server is down!**");
        }

        public static async void OnServerCommand(CommandEventArgs args)
        {
            var chat = await Discord.DiscordBot.GetChannelAsync(Discord.Config.LogID);
            //if (args.Command.Contains("user password"))
            //    return;
            await Discord.DiscordBot.SendMessageAsync(chat, "**Server:** " + args.Command);
        }
        public static async void OnPlayerCommand(PlayerHooks.PlayerCommandEventArgs args)
        {
    //        if ((args.Player == null) || (args.Player == Player.Server) || args.CommandText.Contains("login") || args.CommandText.Contains("register") || args.CommandText.Contains("password"))
    //            return;
    //        try
    //        {
    //            var logs = await Discord.DiscordBot.GetChannelAsync(Discord.Config.LogID);
    //            await Discord.DiscordBot.SendMessageAsync(logs, "**" + args.Player.name + ":** " + args.CommandText);
    //        }
    //        catch (Exception ex)
    //        {
				//COTG.Debug.LogEx(ex);
    //        }
        }
    }
}
