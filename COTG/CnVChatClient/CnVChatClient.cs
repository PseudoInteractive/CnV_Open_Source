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
	using CnVChat;
	using COTG;

	class CnVChatClient:ICnVChatClient
	{
		public static CnVChatClient instance;

		private static CancellationTokenSource shutdownCancellation = new CancellationTokenSource();
		private static ChannelBase channel;
		public ICnVChatClientConnection connection;
		public static async Task Setup()
		{
			if(instance!=null)
				return;
			instance = new();
			
			await  instance.Initialize();

			await foreach (var a in PlayerGameEntity.table.QueryAsync())
				{
					if ( a.playerName is not null)
					{
						try
						{
							var p = Player.all.FirstOrDefault(p =>
								string.Compare(p.Value.name, a.playerName, StringComparison.OrdinalIgnoreCase) == 0).Value;
						
							if (p != null)
							{
								if(p.discordId != 0)
								{
									Log($"Extra DiscordId: {a.playerName} discordId:{a.discordId} discordId1:{p.discordId}");
								}
								p.discordId = a.discordId;
								p.avatarUrl = a.avatarURL;
								p.discordUserName = a.discordUserName??p.name;
								Player.playerByDiscordId.TryAdd(a.discordId, p);
								Player.playerByDiscordUserName.TryAdd(p.discordUserName,p);
							}
							else
							{
								Log($"Missing {a.playerName} discordId:{a.discordId}");
							}

						}
						catch (Exception e)
						{
							LogEx(e);
						
						}
					}
					else
					{
						Log(a.ToString());
					}
				}
		}
		public async Task Initialize()
		{
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
					return;
				while (!Alliance.alliancesFetched)
				{
					await Task.Delay(1000);
				}
				var channels = await connection.JoinAsync(new(){ playerName=Player.myName,world=JSClient.world,alliance=Alliance.my.name,allianceRole="Newbie"}); // Todo store role somewhere
				Log("Got Channels " + channels.Length);
				App.DispatchOnUIThread(async () =>
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
			}
		}

		//public async void JoinResponse(string[] channels)
		//{

			
		//}

		public void OnReceiveMessages( ICnVChatClient.OnReceiveMessagesArgs messageArgs)
		{
			Log("Got Messages " + messageArgs.discordMessages.Length);
			App.DispatchOnUIThread(async () =>
			{
				for(int i = 0;i<messageArgs.discordMessages.Length;++i)
				{
					var message = CnVJsonMessagePackDiscordMessage.Get(messageArgs.discordMessages[i]);
					var senderOverrides = messageArgs.senderOverrides;
					if(!Player.playerByDiscordUserName.TryGetValue(message.Author.Username,out var player))
					{
						player = null;
					}
					else
					{
						Console.WriteLine(
							$"Missing discordName: {message.Author.Username} {message.Author.Id} HasOverride:{senderOverrides != null} ");
					}
					await Discord.AddMessage( (senderOverrides!=null? senderOverrides[i] : (player is not null ? player.discordId : message.Author.Id)),message, false, true);
				}
			});
		}

	}
}
