using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.EventArgs;

using Microsoft.Xna.Framework;
using CnVDiscord;
using System.Reflection;
using static CnVDiscord.PlayerHooks;
using Microsoft.Extensions.Logging;
using static COTG.Debug;
using DSharpPlus.CommandsNext.Converters;
using COTG;
using Microsoft.Extensions.Azure;

namespace CnVDiscord
{
	public static class Discord
	{

		static Dictionary<string, DiscordMember> playerToDiscordId;

		static Color Color;
		public static DiscordGuild guild;
		public static DiscordChannel chatChannel;
		public static DiscordClient DiscordBot { get; set; }
		public static CommandsNextExtension DiscordCommands { get; set; }
		public static ConfigFile Config;

		public async static void Initialize()
		{
			#region Hooks
			//ServerApi.Hooks.ServerCommand.Register(this, Logs.OnServerCommand);
			//ServerApi.Hooks.GamePostInitialize.Register(this, Utils.OnPostInitialize);
			//ServerApi.Hooks.ServerBroadcast.Register(this, Bridge.OnBC);
			//ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreet);
			//ServerApi.Hooks.WorldSave.Register(this, OnSave);
			//GetDataHandlers.KillMe += Bridge.OnKill; 
			PlayerHooks.PlayerCommand += Logs.OnPlayerCommand;
			PlayerHooks.PlayerLogout += OnLogout;
			PlayerHooks.PlayerChat += OnChat;
			#endregion

			LoadConfig();

			Color = new Color(Config.Messagecolor[0], Config.Messagecolor[1], Config.Messagecolor[2]);



			DiscordBot = new DiscordClient(new DiscordConfiguration
			{
				Token = Config.DiscordBotToken,
				TokenType = TokenType.Bot,

				MinimumLogLevel = LogLevel.Warning
			});
			await DiscordBot.ConnectAsync();

			DiscordBot.Ready += async (client, args) =>
			{
				chatChannel = await DiscordBot.GetChannelAsync(Config.ChatID);
				guild = chatChannel.Guild;
				playerToDiscordId = (await guild.GetAllMembersAsync()).ToDictionary((a => a.DisplayName), (a => a));

			};




			if (Config.Commands)
			{
				var ccfg = new CommandsNextConfiguration
				{
					StringPrefixes = new[] { Config.Prefix },

					EnableDms = false,

					EnableMentionPrefix = false
				};

				DiscordCommands = DiscordBot.UseCommandsNext(ccfg);

				DiscordCommands.RegisterCommands<BotCommands>();

				DiscordCommands.SetHelpFormatter<HelpFormatter>();

				DiscordCommands.CommandExecuted += CommandExecuted;
				DiscordCommands.CommandErrored += CommandErrored;
			}

			DiscordBot.ClientErrored += ClientErrored;
			DiscordBot.MessageCreated += OnMessageCreated;
		}

		//protected void Dispose(bool disposing)
		//{
		//    if (disposing)
		//    {
		//        ServerApi.Hooks.ServerCommand.Deregister(this, Logs.OnServerCommand);
		//        ServerApi.Hooks.GamePostInitialize.Deregister(this, Utils.OnPostInitialize);
		//        ServerApi.Hooks.ServerBroadcast.Deregister(this, Bridge.OnBC);
		//        ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreet);
		//        ServerApi.Hooks.WorldSave.Deregister(this, OnSave);

		//        GeneralHooks.ReloadEvent -= OnReload;
		//        //GetDataHandlers.KillMe -= Bridge.OnKill;
		//        PlayerHooks.PlayerCommand -= Logs.OnPlayerCommand;
		//        PlayerHooks.PlayerLogout -= OnLogout;
		//        PlayerHooks.PlayerChat -= OnChat;


		//        await Logs.Goodbye();
		//    }
		//    base.Dispose(disposing);
		//}




		//public ClanManager ClanManager = new ClanManager();

		public static void LoadConfig()
		{
			//           string path = Path.Combine(TShock.SavePath, "NewDiscordBridge.json");
			Config = new ConfigFile();
		}



		private static Task ClientErrored(DiscordClient s, ClientErrorEventArgs e)
		{
			// let's log the details of the error that just 
			// occured in our client
			//   e. DebugLogger.LogMessage(LogLevel.Error, "ExampleBot", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}\n \n{e.Exception.InnerException}", DateTime.Now);

			Trace(e.Exception.Source + ": \n" + e.Exception.Message + "\n \n" + e.Exception.InnerException);

			// since this method is not async, let's return
			// a completed task, so that no additional work
			// is done
			return Task.CompletedTask;
		}

		private static async Task CommandExecuted(CommandsNextExtension s, CommandExecutionEventArgs e)
		{
			// let's log the name of the command and user
			//  e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "ExampleBot", $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

			var logs = await DiscordBot.GetChannelAsync(Config.LogID);
			await DiscordBot.SendMessageAsync(logs, "**" + e.Context.User.Username + ":** " + e.Context.Message.Content);

			// since this method is not async, let's return
			// a completed task, so that no additional work
			// is done
			return;
		}
		private static async Task CommandErrored(CommandsNextExtension s, CommandErrorEventArgs e)
		{
			//e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "ExampleBot", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}\n \n{e.Exception.InnerException}", DateTime.Now);

			Trace(e.Exception.Source + ": \n" + e.Exception.Message + "\n \n" + e.Exception.InnerException);

			if (e.Exception is ChecksFailedException ex)
			{
				var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

				var embed = new DiscordEmbedBuilder
				{
					Title = "Access denied",
					Description = $"{emoji} You do not have the permissions required to execute this command.",
					Color = new DiscordColor(0xFF0000) // red
				};
				await e.Context.RespondAsync("", embed: embed);
			}
			if (e.Exception is ArgumentException)
			{
				var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");
				var embed = new DiscordEmbedBuilder
				{
					Title = "Invalid command syntax",
					Description = $"{emoji} Type `/help {e.Command.Name}`",
					Color = new DiscordColor(0xFF0000) // red
													   // there are also some pre-defined colors available
													   // as static members of the DiscordColor struct
				};
				await e.Context.RespondAsync("", embed: embed);
			}
		}

		#region Discord Hooks
		private static async Task OnMessageCreated(DiscordClient s, MessageCreateEventArgs e)
		{
			if (e.Channel.Id != Config.ChatID)
				return;
			if (e.Author == DiscordBot.CurrentUser || e.Author.IsBot)
				return;
			var name = e.Author.Username;
			e.Message
			TShock.Utils.Broadcast(string.Format(Config.DiscordToTerrariaFormat, , e.Message.Content), Color);
			return;
		}
		#endregion


		//void OnGreet(GreetPlayerEventArgs args)
		//{
		////    if (Config.JoinLogID != 0)
		////    {
		////        Logs.JoinLeave(TShock.Players[args.Who]);
		////    }
		//}
		static void OnLogout(PlayerHooks.PlayerLogoutEventArgs args)
		{
			//    if (args.Player == null)
			//        return;

			//    if (args.Player.ReceivedInfo)
			//        Logs.JoinLeave(args.Player, false);

			//    return;
		}
		//void OnSave(WorldSaveEventArgs args)
		//{
		//    if (Config.Chat)
		//    {
		//        Bridge.OnSaveWorld(args);
		//    }
		//}
		public static async void OnChat(PlayerChatEventArgs args)
		{
			if (Config.Chat)
			{
				try
				{
					var name = args.player.name;

					var user = playerToDiscordId.GetValueOrDefault(name);
					//DiscordMessageBuilder message;
					var users = new List<IMention>();

					StringBuilder sb = new StringBuilder();
					if (user == null)
					{
						sb.Append(args.player.name);
					}
					else
					{
						sb.Append(user.Mention.ToString());
						users.Add(new UserMention(user));
					}
					sb.Append(':');
					var str = args.text;
					for (; ; )
					{
						var strLen = str.Length; 
						if (strLen <= 0)
							break;
						var f = str.IndexOf('@');
						if (f == -1)
						{
							sb.Append(str);
							break;
						}
						if (f > 0)
							sb.Append(str.Substring(0, f));
						++f;
						var fStart = f;
						
						while (f < strLen && (char.IsLetterOrDigit(str[f]) || str[f] == '_')) 
						{
							++f;
						}
						
						var mentionName = str.Substring(fStart, f - fStart);
						var mention = playerToDiscordId.GetValueOrDefault(mentionName);
						if (mention != null)
						{
							sb.Append(mention.Mention);
							users.Add(new UserMention(mention));
						}
						else
						{
							sb.Append(str.Substring(0, f));
						}
						str = str.Substring(f);
					}


					var message = new DiscordMessageBuilder().WithContent(sb.ToString()).WithAllowedMentions(users);
					var channel = await Discord.DiscordBot.GetChannelAsync(Discord.Config.ChatID);
					await Discord.DiscordBot.SendMessageAsync(channel, message);

				}
				catch (Exception a)
				{
					LogEx(a, false, $"Error when sending message to discord: {a.Message}");
				}


			}
		}
	}
}
