using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Grpc.Core;
using MagicOnion.Client;
using MagicOnion;
using MessagePack;

using MessagePack.Resolvers;
using Grpc.Net.Client;
using CnVShared;
using System.Threading;
using DSharpPlus.Entities;
namespace CnVDiscord
{
	using System.Net.Http;
	using System.Text.RegularExpressions;

	using CnV;

	using CnVChat;
	using Microsoft.UI.Xaml.Media.Imaging;

	class CnVChatClient:ICnVChatClient
	{
		public static CnVChatClient? instance;

		private static CancellationTokenSource shutdownCancellation = new CancellationTokenSource();
		private static ChannelBase channel;
		public ICnVChatClientConnection connection;
		public static async Task Setup()
		{
			#if CNV
			if(instance!=null)
				return;
		
			instance = new();
			
			// Any Magic Onion?
			if(!await  instance.Initialize() ) 
				return; 
			#endif
		}

		public async Task<bool> Initialize()
		{
			return false;
#if CNV
			try
			{
				Assert(channel == null);

				// NOTE: Currently, CompositeResolver doesn't work on Unity IL2CPP build. Use StaticCompositeResolver instead of it.


				var resolver = CompositeResolver.Create(StandardResolver.Instance
					//		,GeneratedResolver.Instance);
				);
				
			//	MessagePackSerializer.DefaultOptions = MessagePackSerializer.DefaultOptions.WithResolver(resolver);
			var handler = new SocketsHttpHandler
			{
				PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
				KeepAlivePingDelay = TimeSpan.FromSeconds(60),
				KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
				EnableMultipleHttp2Connections = true
			};
				// Connect to the server using gRPC channel.
				channel = GrpcChannel.ForAddress("https://localhost:5001",new GrpcChannelOptions()
				{
					HttpHandler = handler
				});
				
				connection = await StreamingHubClient.ConnectAsync<ICnVChatClientConnection,ICnVChatClient>(channel,this, cancellationToken: shutdownCancellation.Token);
				if(connection == null)
					return false;
				await Alliance.alliancesFetchedTask.WaitAsync(false);
				var channels = await connection.JoinAsync(new(){ playerId=Player.myId,world=CnVServer.worldId,alliance=Alliance.myId,allianceTitle=AllianceTitle.newbie}); // Todo store role somewhere
				Log("Got Channels " + channels.Length);
				AppS.DispatchOnUIThread(async () =>
				{
					foreach (var channel in channels)
					{
						Log( channel );
						var c = CnVJsonMessagePackDiscordChannel.Get(channel);
						// Todo:  Create channel
						ChatTab.CreateChatTab(c);

						await connection.ConnectChannelAsync( new()
							{ channelId = c.Id, lastRecieved = 0 }); // todo:  Lastrecieved
					}
				});
			}
			catch (Exception e)
			{
				Log(e.ToString());
				return false;
			}
#endif
			return true;
		}

		//public async void JoinResponse(string[] channels)
		//{

			
		//}

		public void OnReceiveMessages( ICnVChatClient.OnReceiveMessagesArgs messageArgs)
		{
			Log("Got Messages " + messageArgs.discordMessages.Length);
			AppS.DispatchOnUIThread(async () =>
			{
				for(int i = 0;i<messageArgs.discordMessages.Length;++i)
				{
					var message = CnVJsonMessagePackDiscordMessage.Get(messageArgs.discordMessages[i]);
					var senderOverrides = messageArgs.senderOverrides;
					if(!Player.fromDiscordUserName.TryGetValue(message.Author.Username,out var player))
					{
						player = null;
					}
					else
					{
						Log($"Missing discordName: {message.Author.Username} {message.Author.Id} HasOverride:{senderOverrides != null} ");
					}
					await AddMessage( (senderOverrides!=null? senderOverrides[i] : (player is not null ? player.discordId : message.Author.Id)),message, false, true);
				}
			});
		}
		public static string DisplayName(DiscordUser user)
		{
			return (user is DiscordMember member) ? member.DisplayName : user.Username;
		}
		static readonly Regex regexMention = new Regex(@"\<@(\w+)\>", RegexOptions.CultureInvariant | RegexOptions.Compiled);

		public static async Task AddMessage(ulong senderOverride,DiscordMessage message, bool isNew, bool notify)
		{
			try
			{
				if(!Player.fromDiscordId.TryGetValue(senderOverride,out var p))
					p = Player.me;
				var name = p.name; // todo: use clients
				if (p.avatarBrush is null && p.avatarUrl is not null )
				{
					var url = p.avatarUrl;
					

					var _name = name; 
					
					await AppS.DispatchOnUIThreadTask( () =>
						{
							p.avatarBrush= new BitmapImage(new Uri(url));
							return Task.CompletedTask;
						})
						;
				}
//				var avatarUrl = $"![Helpers Image]({p.avatarUrl})";

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
				AppS.DispatchOnUIThread(() => ChatTab.Post(message.ChannelId, chat, isNew,notify));
			}
			catch (Exception ex)
			{
				LogEx(ex);
			}
			return;
		}

	}
}
