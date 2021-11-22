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
									Trace($"Extra DiscordId: {a.playerName} discordId:{a.discordId} discordId1:{p.discordId}");
								}
								p.discordId = a.discordId;
								p.avatarUrl = a.avatarURL;
							}
							else
							{
								Trace($"Missing {a.playerName} discordId:{a.discordId}");
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
				
				// Connect to the server using gRPC channel.
				channel = GrpcChannel.ForAddress("https://localhost:5001");
				
				connection = await StreamingHubClient.ConnectAsync<ICnVChatClientConnection,ICnVChatClient>(channel,this, cancellationToken: shutdownCancellation.Token);
				if(connection == null)
					return;
				while (!Alliance.alliancesFetched)
				{
					await Task.Delay(1000);
				}
				await connection.JoinAsync(new(){ playerName=Player.myName,world=JSClient.world,alliance=Alliance.my.name,allianceRole="Newbie"}); // Todo store role somewhere
			//	await Task.Delay(5000);
			//if (a is null)
			//{
			//	Assert(false);
			//}
			//else
			//{
			//	foreach (var c in a)
			//	{
			//		Log(c.Get());
			//	}
			//}
			// Initialize gRPC channel provider when the application is loaded.
				//GrpcChannelProviderHost.Initialize(new DefaultGrpcChannelProvider(new GrpcCCoreChannelOptions(new[]
				//{
				//             // send keepalive ping every 5 second, default is 2 hours
				//             new ChannelOption("grpc.keepalive_time_ms", 5000),
				//             // keepalive ping time out after 5 seconds, default is 20 seconds
				//             new ChannelOption("grpc.keepalive_timeout_ms", 5 * 1000),
				//})));
			}
			catch (Exception e)
			{
				LogEx(e);
			}
		}

		public async void JoinResponse(string[] channels)
		{

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

		public void OnReceiveMessages(string[] messages)
		{
			Log("Got Messages " + messages.Length);
			App.DispatchOnUIThread(async () =>
			{
				foreach (var message in messages)
				{

					await Discord.AddMessage( CnVJsonMessagePackDiscordMessage.Get(message), false, true);
				}
			});
		}

	}
}
