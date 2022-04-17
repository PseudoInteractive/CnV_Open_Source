using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CnV.Game;
using CnV.Services;
using CnV.Views;
using System.Numerics;
#if APPCENTER
using Microsoft.AppCenter;
#endif
using CommunityToolkit.WinUI.Helpers;

using System.Runtime;

namespace CnV
{
	using CnV;

	using CnVChat;

	using CnVDiscord;
	using Game;
	using Services;
	using Views;

	internal static partial class CnVClient
	{
		private static BackgroundTask reinforcementsTask;
		private static BackgroundTask senInfoTask;


		

		public static async Task InitializeGame()
	{

		//					   CnVChatClient.instance = new();
		//					   CnVChatClient.instance.Initialize();
		try
		{
				
				// Todo:  Persist this
			
				//var timeOffset = jso.GetAsInt64("timeoffset");
				//var timeOffsetSecondsRounded = Math.Round(timeOffset / (1000.0 * 60*30)) * 60 * 30.0f; // round to nearest half hour
			//	CnVServer.gameTOffset = TimeSpan.FromHours(World.timeZoneOffsetHours);
			//	CnVServer.gameTOffsetSeconds = CnVServer.gameTOffset.TotalSeconds.RoundToInt();
				

				var t1 = Task.Run(() => TileData.Ctor(false));
				
				var t2 = CityCustom.Load();
				BuildQueue.Initialize();
				await t1.ConfigureAwait(false);

				// World cannot load until tiles are loaded
				
				await t2.ConfigureAwait(false); ; // city custom can end here

				AppS.SetState( AppS.State.active );

				ShellPage.WorkUpdate("Connect to server");
				if(AppS.isSinglePlayer)
				{
					// Kick off sim ourselves
					await Sim.StartSim(null);
				}
				else
				{
					SocketClient.Init();
					// Message from server will kick off sim
					while(Sim.simPhase == Sim.SimPhase.init)
					{
						await Task.Delay(500);

					}
				}
//				await Sim.StartSim();

				//	var str = timeOffsetSecondsRounded >= 0 ? " +" : " ";
				//	str += $"{gameTOffset.Hours:D2}:{gameTOffset.Minutes:D2}";
				//	Helpers.JSON.timeZoneString = str;
				//   Log(JSONHelper.timeZoneString);


				//	ppss = jso.GetAsInt("ppss");
				//	Player.myName = jso.GetString("player");
				//	if(Player.subOwner == null)
				//		Player.subOwner = Player.myName;
				//	Player.myId = Player.myId = jso.GetAsInt("pid"); ;

				//Note.L("cid=" + cid.CidToString());
				//gameMSAtStart = jso.GetAsInt64("time");
				//launchTime = DateTimeOffset.UtcNow;
				//    Log(jsVars.ToString());
				//  Settings.secSessionId = jso.GetAsString("s");
				//		AGame.clientTL.X = jso.GetAsFloat("left");
				//  AGame.clientTL.Y = jso.GetAsFloat("top");
				//   Log($"WebClient:{AGame.clientTL} {ShellPage.webclientSpan.y}");
				//     Note.Show($" {clientSpanX}:{clientSpanY} {ShellPage.clientTL} ");
				//			   spanX = jso.GetAsInt("spanX");
				//			   spanY = jso.GetAsInt("spanY");
				//			   Note.Show($"ClientSpan: {spanX}x{spanY}");
				//    Log($"Built heades {httpClient.DefaultRequestHeaders.ToString() }");

				//   UpdatePPDT(jso.GetProperty("ppdt"));

				// todo: utf
				//		AddPlayer(true, true, Player.myId, Player.myName, token, raidSecret, cookies);//, s, ppdt.ToString());
				ShellPage.WorkEnd();


				while(!City.myCities.Any())
				{
					await Task.Delay(500);
					ShellPage.RefreshX();
				}
				//UpdatePPDT();
				var cid = City.myCities.First().cid;

		//		Assert( CnVServer.worldId != 0);
				
				await CitySwitch(cid);
				View.SetViewTargetInstant(cid.CidToWorldV());
				ShellPage.SetViewModeRegion();
				Assert(Spot.build ==cid && Spot.focus == cid);
			//	Spot.build = Spot.focus = cid;
				//NavStack.Push(cid);
				//AGame.CameraC = cid.CidToWorldV();


				//CnVChatClient.CnVChatClient.Setup();
				ShellPage.CityListNotifyChange(true);
				ShellPage.RefreshTabs.Go();
			//	ShellPage.canvasVisible = true;
			   
			///                   await GetCitylistOverview();
			
			//if (TipsSeen.instance.refresh == false
			//||TipsSeen.instance.chat0==false
			//|| TipsSeen.instance.chat1 == false
			//|| TipsSeen.instance.chat2 == false)
			//    App.QueueIdleTask(ShellPage.ShowTipRefresh);
			// await RaidOverview.Send();
		//	AppS.QueueIdleTask(IncomingOverview.ProcessTask,1000);
		//	AppS.QueueIdleTask(OutgoingOverview.ProcessTask,1000);

			

			Task.Run(  CnVChatClient.Setup );

				//CnVServer.SetStayAlive(Settings.stayAlive);
				//{
				//    //var now = DateTime.UtcNow;
				//    //if (now.Day <= 28 && now.Month==11)
				//    {

				// 
				// Friend.LoadAll();

				//	CnVDiscord.Discord.Initialize();

				//  System.GC.Collect(2,GCCollectionMode.Default,true);

				// give some time for initial pressure to come down




				ShellPage.updateHtmlOffsets.SystemUpdated();

				//	await Task.Delay(1000);

				//	System.GC.Collect(2,GCCollectionMode.Default,true,true);

				//	GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
			//	reinforcementsTask = new(interval:64.0f,()=> ReinforcementsOverview.instance.Post(),initialDelay:4.0f );
			//	senInfoTask        = new( interval: 68f, City.UpdateSenatorInfo, 3.0f);
				Sim.isInitialized      = true;
				
				AppS.DispatchOnUIThread( ShellPage.SetupNonCoreInput );
				AppS.QueueIdleTask(DailyRewardTask);
		
			}
		catch(Exception ex)
		{
			LogEx(ex);
		}
		finally
		{
			Interlocked.Decrement(ref View.hideSceneCounter);
		}
		}
	
		static void DailyRewardTask()
		{
			AppS.QueueOnUIThread( () => DailyDialog.ShowInstance(Artifact.GetForPlayerRank(Artifact.ArtifactType.axe)) );
		}
	}
}
