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
using COTG.Views;
using COTG.Services;
using SharpDX.Direct2D1;
using COTG.Helpers;
using Windows.UI.Xaml.Media.Imaging;

namespace CnVDiscord
{
	public static class Discord
	{
		public static Dictionary<string, string> avatarUrls = new();
		public static Dictionary<string, BitmapImage> avatarBrushes = new();
		const int messageFetchCount = 200;
		static Dictionary<string, DiscordMember> playerToMember = new();
		//		static Dictionary<ulong, DiscordMember> UserIdToMemeber;

	//	static Color Color;
	//	public static DiscordGuild guild;
		public static DiscordChannel chatChannel;
		public static DiscordClient DiscordBot { get; set; }
		public static CommandsNextExtension DiscordCommands { get; set; }
		public static ConfigFile Config = new();
		public static bool IsAllianceConnected => Config.ChatID != 0;
		public async static void Initialize()
		{


			Config.ChatID = await Tables.GetDiscordChatId().ConfigureAwait(false);
			if (Config.ChatID == 0)
				return;

		//	Color = new Color(Config.Messagecolor[0], Config.Messagecolor[1], Config.Messagecolor[2]);



			DiscordBot = new DiscordClient(new DiscordConfiguration
			{
				Token = Config.DiscordBotToken,
				TokenType = TokenType.Bot,
				Intents = DiscordIntents.AllUnprivileged,

				MinimumLogLevel = LogLevel.Warning
			});

			await DiscordBot.ConnectAsync().ConfigureAwait(false);
			
			//DiscordBot.GuildDownloadCompleted += (_, args) =>
			//{
			//	return App.DispatchOnUIThreadTask(() =>
			//	{
			//		foreach (var g in args.Guilds)
			//		{
			//			foreach (var author in g.Value.Members.Values)
			//			{
			//				var nameLower = author.DisplayName.ToLower();
			//				if (!avatarUrls.TryGetValue(nameLower, out var _))
			//				{
			//					var url = author.GetAvatarUrl(ImageFormat.Auto, 32);
			//					avatarUrls.TryAdd(nameLower, $"![Helpers Image]({url})");
			//					avatarBrushes.TryAdd(nameLower, new BitmapImage(new Uri(url)));
			//				}
			//			}
			//		}
			//		return Task.CompletedTask;

			//	});
				

			//};


			DiscordBot.Ready += async (client, args) =>
			{
				chatChannel = await DiscordBot.GetChannelAsync(Config.ChatID).ConfigureAwait(false);
				var members = await chatChannel.Guild.GetAllMembersAsync().ConfigureAwait(false);
				foreach (var i in members)
				{
					var nameLower = i.DisplayName.ToLowerInvariant();
					if (!avatarUrls.TryGetValue(nameLower, out var _))
					{
						var url = i.GetAvatarUrl(ImageFormat.Auto, 32);
						avatarUrls.TryAdd(nameLower, $"![Helpers Image]({url})");
						avatarBrushes.TryAdd(nameLower, new BitmapImage(new Uri(url)));
					}

					playerToMember.TryAdd(nameLower, i);
				}
				//			UserIdToMemeber = members.ToDictionary((a => a.Id), (a => a));




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


				//if (Config.Commands )
				//{
				//	var ccfg = new CommandsNextConfiguration
				//	{
				//		StringPrefixes = new[] { Config.Prefix },

				//		EnableDms = false,

				//		EnableMentionPrefix = false
				//	};

				//	DiscordCommands = DiscordBot.UseCommandsNext(ccfg);

				//	DiscordCommands.RegisterCommands<BotCommands>();

				//	DiscordCommands.SetHelpFormatter<HelpFormatter>();

				//	DiscordCommands.CommandExecuted += CommandExecuted;
				//	DiscordCommands.CommandErrored += CommandErrored;
				//}
							;
		
				DiscordBot.ClientErrored += ClientErrored;
				DiscordBot.MessageCreated += OnMessageCreated;
				var fetch = await chatChannel.GetMessagesAsync(messageFetchCount).ConfigureAwait(false);
				foreach(var message in fetch)
				{
					await AddMessage(message,false).ConfigureAwait(false);
				}
			};
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
			try
			{
				// let's log the name of the command and user
				//  e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "ExampleBot", $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

				var logs = await DiscordBot.GetChannelAsync(Config.LogID).ConfigureAwait(false);
				await DiscordBot.SendMessageAsync(logs, "**" + e.Context.User.Username + ":** " + e.Context.Message.Content).ConfigureAwait(false);

				// since this method is not async, let's return
				// a completed task, so that no additional work
				// is done
			}
			catch (Exception ex)
			{

				LogEx(ex);
			}
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
				await e.Context.RespondAsync("", embed: embed).ConfigureAwait(false);
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
				await e.Context.RespondAsync("", embed: embed).ConfigureAwait(false);
			}
		}

		public static string DisplayName(DiscordUser user)
		{
			return (user is DiscordMember member) ? member.DisplayName : user.Username;
		}
		static Regex regexMention = new Regex(@"\<@(\w+)\>", RegexOptions.CultureInvariant | RegexOptions.Compiled);

		private static async Task AddMessage(DiscordMessage message, bool isNew)
		{
			try
			{
				var author = message.Author;
				if (author.IsBot)
					return;

				var name = DisplayName(author); // todo: use clients
				var nameLower = name.ToLowerInvariant();
				if (!avatarUrls.TryGetValue(nameLower,out var avatarUrl) )
				{
					var url = author.GetAvatarUrl(ImageFormat.Auto, 32);
					avatarUrl = $"![Helpers Image]({url})";
					avatarUrls.TryAdd(nameLower, avatarUrl);
				

					var _name = name; 
					
					await App.DispatchOnUIThreadTask( () =>
					{
						avatarBrushes.TryAdd(nameLower, new BitmapImage(new Uri(url)));
						return Task.CompletedTask;
					})
						;
				}
				var content = message.Content;

				foreach (var i in message.MentionedUsers)
				{
					var mention = i.Mention;
					var displayName = DisplayName(i);
					var mentionGame = $"[{displayName}](/p/{displayName})";
					content = content.Replace(mention ,mentionGame );
					if(content.Contains('!'))
					{
						int q = 0;
					}
					if (mention.Contains('!'))
						mention = mention.Replace("!", "");
					else
						mention = regexMention.Replace(mention,"<@!$1>" );

					content = content.Replace(mention, mentionGame);
				}
				var chat = new ChatEntry(name, content, message.Timestamp.ToServerTime(), ChatEntry.typeAlliance);
				App.DispatchOnUIThreadLow(() => ChatTab.alliance.Post(chat, isNew));
			}
			catch (Exception ex)
			{
				LogEx(ex);
			}
			return;
		}


		#region Discord Hooks
		private static async  Task OnMessageCreated(DiscordClient s, MessageCreateEventArgs e)
		{
			try
			{
				if (e.Channel.Id != Config.ChatID)
					return;
				await AddMessage(e.Message,true).ConfigureAwait(false); // is this correct
				e.Handled = true;
			}
			catch (Exception ex)
			{
				LogEx(ex);
			}
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
					if (!await Tables.TryAddChatMessage(name + args.text))
						return;
					var user = playerToMember.GetValueOrDefault(name.ToLowerInvariant());
					//DiscordMessageBuilder message;
					var users = new List<IMention>();

					StringBuilder sb = new StringBuilder();
					//var displayName;
					//if (user == null)
					//{
					//	displayName = (args.player.name);
					//}
					//else
					//{
					//	displayName = (user.Mention.ToString());
					//	users.Add(new UserMention(user));
					//}
					//	sb.Append(':');
					var embed = new DiscordEmbedBuilder();
					var displayName = user != null ? user.DisplayName : name;

					if (user != null)
					{
						embed.WithAuthor(displayName,user.AvatarUrl);//,null,user.GetAvatarUrl(ImageFormat.Auto, 64));
						embed.WithThumbnail(user.GetAvatarUrl(ImageFormat.Auto,64));
	//					sb.Append($"<img src=\"{user.AvatarUrl}\" alt=\"{user.DisplayName}\") width=\"32\" height=\"32\" > ");
					}
					else
					{
						embed.WithAuthor(displayName);//,null,user.GetAvatarUrl(ImageFormat.Auto, 64));
					}


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
						var mention = playerToMember.GetValueOrDefault(mentionName.ToLowerInvariant());
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
					embed.WithDescription(sb.ToString());

					var message = new DiscordMessageBuilder().WithAllowedMentions(users).WithEmbed(embed);
					
					var channel = chatChannel;
					await DiscordBot.SendMessageAsync(channel, message).ConfigureAwait(false);

				}
				catch (Exception a)
				{
					LogEx(a, false, $"Error when sending message to discord: {a.Message}");
				}


			}
		}
	}
}
