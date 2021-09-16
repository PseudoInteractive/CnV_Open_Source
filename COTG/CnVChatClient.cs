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
using System.Net.Http;
using Grpc.Net.Client.Web;

namespace COTG.CnVChat
{
	class CnVChatClient:ICnVChatClient
	{
		internal static CnVChatClient instance;
		private static CancellationTokenSource shutdownCancellation = new CancellationTokenSource();
		private static ChannelBase channel;

		public async Task Initialize()
		{
			return;
			// NOTE: Currently, CompositeResolver doesn't work on Unity IL2CPP build. Use StaticCompositeResolver instead of it.
			//StaticCompositeResolver.Instance.Register(
			//	MagicOnion.Resolvers.MagicOnionResolver.Instance,
			//	MessagePack.Resolvers.GeneratedResolver.Instance,
			//	BuiltinResolver.Instance,
			//	PrimitiveObjectResolver.Instance
			//);

			MessagePackSerializer.DefaultOptions = MessagePackSerializer.DefaultOptions
				.WithResolver(StaticCompositeResolver.Instance);

			// Connect to the server using gRPC channel.
			channel = GrpcChannel.ForAddress("https://localhost:5001",new GrpcChannelOptions()	{
//				HttpHandler = new GrpcWebHandler(new HttpClientHandler())
				HttpHandler = new WinHttpHandler()
			});
		//	channel = GrpcChannel.ForAddress("http://localhost:5000");

			var streamingClient = await StreamingHubClient.ConnectAsync<ICnVChatClientConnection,ICnVChatClient>(channel,this);
			var stuff = await streamingClient.JoinAsync(0);
			Debug.Log(stuff);
			// Initialize gRPC channel provider when the application is loaded.
			//GrpcChannelProviderHost.Initialize(new DefaultGrpcChannelProvider(new GrpcCCoreChannelOptions(new[]
			//{
   //             // send keepalive ping every 5 second, default is 2 hours
   //             new ChannelOption("grpc.keepalive_time_ms", 5000),
   //             // keepalive ping time out after 5 seconds, default is 20 seconds
   //             new ChannelOption("grpc.keepalive_timeout_ms", 5 * 1000),
			//})));
		}

		public void OnReceiveMessages(CnVJsonMessagePack<DiscordMessage>[] message)
		{
		}
	}
}
