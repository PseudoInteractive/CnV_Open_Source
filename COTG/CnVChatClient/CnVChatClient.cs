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
	using COTG;

	class CnVChatClient:ICnVChatClient
	{
		static CnVChatClient instance;

		private static CancellationTokenSource shutdownCancellation = new CancellationTokenSource();
		private static ChannelBase channel;

		public static Task Setup()
		{
			if(instance!=null)
				return Task.CompletedTask; // once is enough
			instance = new();
			return instance.Initialize();
		}
		public async Task Initialize()
		{
			try
			{
				Assert(channel == null);
				MessagePack.Resolvers.CompositeResolver.RegisterAndSetAsDefault(
					MessagePack.Resolvers.GeneratedResolver.Instance,
					MessagePack.Resolvers.BuiltinResolver.Instance,
					// use PrimitiveObjectResolver
					PrimitiveObjectResolver.Instance
				);

				// NOTE: Currently, CompositeResolver doesn't work on Unity IL2CPP build. Use StaticCompositeResolver instead of it.
				var resolver = CompositeResolver.Create(BuiltinResolver.Instance, PrimitiveObjectResolver.Instance,
					
					MessagePack.Resolvers.GeneratedResolver.Instance
					
				);

				MessagePackSerializer.DefaultOptions = MessagePackSerializer.DefaultOptions
					.WithResolver(resolver);

				// Connect to the server using gRPC channel.
				channel = GrpcChannel.ForAddress("https://localhost:5001");

				var stream = await StreamingHubClient.ConnectAsync<ICnVChatClientConnection,ICnVChatClient>(channel,this, cancellationToken: shutdownCancellation.Token);
				var a = await stream.JoinAsync(new(){ playerName=Player.myName,world=JSClient.world,alliance=Alliance.my.name,allianceRole="Newbie"}); // Todo store role somewhere
				Log(a);
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
				throw;
			}
		}

		public void OnReceiveMessages(CnVJsonMessagePack<DiscordMessage>[] message)
		{
			throw new NotImplementedException();
		}

	}
}
