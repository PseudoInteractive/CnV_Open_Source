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

	internal class CnVChatClient:ICnVChatClient,IMagicOnionClientLogger
	{
		public static CnVChatClient? instance;
		internal static bool initialized = false;
		internal static bool isShuttingDown = false;
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
		public static async Task ShutDown(bool permanent)
		{
			//Assert(isShuttingDown == false);
			try
			{
				isShuttingDown=true;
				//if(!initialized)
				//	return;
				initialized=false;
				if(instance?.connection is var c && c is not null)
				{
					instance.connection = null;
					await c.LeaveAsync();

					await c.DisposeAsync();
				}
				if(channel is var ch && ch is not null)
				{
					channel = null;
					await ch.ShutdownAsync();
				}
			}
			catch(Exception _ex)
			{
				Log(_ex.Message);

			}
			if(!permanent)
				isShuttingDown=false;

		}
		public async Task<bool> Initialize()
		{
#if CNV
				Assert(channel == null);
				Assert(initialized==false);
				// NOTE: Currently, CompositeResolver doesn't work on Unity IL2CPP build. Use StaticCompositeResolver instead of it.


				//var resolver = CompositeResolver.Create(StandardResolver.Instance
				//	//		,GeneratedResolver.Instance);
				//);

				//	MessagePackSerializer.DefaultOptions = MessagePackSerializer.DefaultOptions.WithResolver(resolver);
				
				for(;;)
				{
					if(!initialized && !isShuttingDown)
					{
						// Connect to the server using gRPC channel.
						try
						{
						
							channel = GrpcChannel.ForAddress("http://cnv.westus2.cloudapp.azure.com:5000",new GrpcChannelOptions()
							{
								HttpHandler = new SocketsHttpHandler
							{
								PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
								KeepAlivePingDelay = TimeSpan.FromSeconds(60),
								KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
								EnableMultipleHttp2Connections = true
							}
							});

						connection = await StreamingHubClient.ConnectAsync<ICnVChatClientConnection,ICnVChatClient>(channel,this,cancellationToken: shutdownCancellation.Token,logger:this);
						if(connection == null)
							goto tryAgain;
						while(!Sim.isPastWarmup)
						{
							await Task.Delay(300);
						}

						await Alliance.alliancesFetchedTask.WaitAsync(false);
						var me = Player.me;



						var channels = await connection.JoinAsync(new() { playerId=me.pid,world=Sim.worldId,alliance=me.allianceId,allianceTitle=me.allianceTitle }); // Todo store role somewhere
						Log("Got Channels " + channels.Length);
						AppS.DispatchOnUIThread(async () =>
						{
							foreach(var channel in channels)
							{
								if(connection is null)
									break;
								Log(channel);
								var c = CnVJsonMessagePackDiscordChannel.Get(channel);
								ChatTab.CreateChatTab(c);
								await connection.ConnectChannelAsync(new()
								{ channelId = c.Id,lastRecieved = 0 }); // todo:  Lastrecieved


							}
						});
					Note.Show($"Connected to Chat ({channels.Length} channels)");
						initialized=true;
						}
						catch (Exception e)
						{
							Log(e.ToString());
							await ShutDown(false);

						//	return false;
						}
					}
					tryAgain:
					await Task.Delay(TimeSpan.FromMinutes(5.0f));
				}
			
			
#endif
			return true;
		}

		public static Task UpdatePlayerAlliance(Player me)
		{
			if(initialized)
				return instance.UpdatePlayerAsync(new() { playerId=me.pid,world=Sim.worldId,alliance=me.allianceId,allianceTitle=me.allianceTitle });
			else
				return Task.CompletedTask;
		}
			/// <summary>
		/// A property of an 
		/// existing player is changed (alliance, role, etc)
		/// This is called to update the player immediately in Discord
		/// Functionally this is equivalent to to the default implementation
		/// (which is not used, it is for reference only)
		/// However the presentation is different (you don't see "Avatar" signed out ... "Avatar" signed in)
		/// Only changed values should be non null in the changed parameter
		/// </summary>
		/// <param name="memberId"></param>
		/// <param name="world"></param>
		/// <returns></returns>
		async Task UpdatePlayerAsync(ICnVChatClientConnection.JoinOrUpdateAsyncArgs changed)
		{
			// Equivalent functionality
			// Todo: store connected channels
			if (initialized)
			{

				await connection?.LeaveAsync();
				await connection?.JoinAsync(changed);
			}
			// Todo:   ConnectChannelAsync() for each channel previously connected
			// 
		}
		public void OnReceiveMessages( ICnVChatClient.OnReceiveMessagesArgs messageArgs)
		{
			Log("Got Messages " + messageArgs.discordMessages.Length);
			AppS.DispatchOnUIThread(() =>
			{
				for(int i = 0;i<messageArgs.discordMessages.Length;++i)
				{
					var message = CnVJsonMessagePackDiscordMessage.Get(messageArgs.discordMessages[i]);
					var senderOverrides = messageArgs.senderOverrides;
					var authorId = senderOverrides is not null ? senderOverrides[i] : message.Author.Id;
					string name;
					if(Player.fromDiscordId.TryGetValue(authorId,out var player))
					{
						name = player.name;
						
					}
					else
					{
						// Search for a long name from the short name
						name = message.Author.Username;
						var longer = Player.all.FirstOrDefault(a=>a.shortName == name);
						if(longer is not null)
							name = longer.name;
						Log($"Missing discordName: {message.Author.Username}=>{name} {message.Author.Id} {authorId} HasOverride:{senderOverrides != null} ");

					}
					AddMessage( name,message, false, true);
				}
			});
		}
		public static string DisplayName(DiscordUser user)
		{
			return (user is DiscordMember member) ? member.DisplayName : user.Username;
		}
		static readonly Regex regexMention = new Regex(@"\<@(\w+)\>", RegexOptions.CultureInvariant | RegexOptions.Compiled);

		public static void AddMessage(string name,DiscordMessage message, bool isNew, bool notify)
		{
			try
			{
				//if(!Player.fromDiscordId.TryGetValue(senderOverride,out var p))
				//	p = Player.me;
				//var name = p.name; // todo: use clients
				//if (p.avatarBrush is null && p.avatarUrl is not null )
				//{
				//	var url = p.avatarUrl;
					

				//	var _name = name; 
					
				//	await AppS.DispatchOnUIThreadTask( () =>
				//		{
				//			p.avatarBrush= new BitmapImage(new Uri(url));
				//			return Task.CompletedTask;
				//		})
				//		;
				//}
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
				var chat = new ChatEntry(name, content,IServerTime.UtcToServerTime(message.Timestamp), ChatEntry.typeAlliance);
				AppS.DispatchOnUIThread(() => ChatTab.Post(message.ChannelId, chat, isNew,notify));
			}
			catch (Exception ex)
			{
				LogEx(ex);
			}
			return;
		}

		public async void Error(Exception ex,string message) {
			Log(message);
			Note.Show("Disconnected from Chat");
			if(initialized)
			{
				try
				{
					await ShutDown(false);
					
				}
				catch(Exception ex2)
				{
					Log(ex2.Message);
				}
				isShuttingDown=false; // start up again later
			}
			//LogEx(ex);
		}
		public void Information(string message) => Log(message);
		public void Debug(string message) => Log(message);
		public void Trace(string message) => Log(message);
	}
}
