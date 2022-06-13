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


		

		public static async Task InitializeGame()
	{
			//var p0 = ServerTime.GetNightProtection(new ServerTime(2022,3,1,21,30,0));
			//var p = ServerTime.GetNightProtection(new ServerTime(2022,3,1,22,30,0));
			//var p1 = ServerTime.GetNightProtection(new ServerTime(2022,3,1,23,30,0));
			//var p2 = ServerTime.GetNightProtection(new ServerTime(2022,3,1,0,30,0));
			//var p3 = ServerTime.GetNightProtection(new ServerTime(2022,3,1,8,0,0));
			//var p4 = ServerTime.GetNightProtection(new ServerTime(2022,3,1,11,0,0));
			//	var selector = new RandomSelector<int>();
			//selector.Add(1.0f,0);
			//selector.Add(0.75f,1);
			//selector.Add(0.5f,2);
			//selector.Add(0.25f,3);
			//selector.Add(0.25f,4);
			//selector.Add(0.125f,5);
			//var sels = new int[6];
			//var rnd = new XXRand(11);
			//for(int i =0;i<1000;++i) {
			//	var sel = selector.Select(ref rnd);
			//	++sels[sel];
			//}
			//int q = 0;

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
;				var t3 = TTip.persist.Load();
				//BuildQueue.Initialize();

				// World cannot load until tiles are loaded
				
				await Task.WhenAll( t2,t3,t1).ConfigureAwait(false); ; // city custom can end here
				
				AppS.SetState( AppS.State.active );

				ShellPage.WorkUpdate("Connect to server");
				if(AppS.isSinglePlayer)
				{
					// Kick off sim ourselves
					//if(!(AppS.currentVersion >= AppS.minCompatibleVersion) ) {
					//			var	error = $"To version: {AppS.minCompatibleVersion}\n\nhttps://www.microsoft.com/store/apps/9NC40PRJ3ZG4";
					//			Trace(error);
					//	await AppS.Fatal(error,"Game update needed");
					//}

					await Sim.StartSim(null).ConfigureAwait(false);
				}
				else
				{
					SocketClient.Init();
					// Message from server will kick off sim
					
				}
				//while(Sim.simPhase == Sim.SimPhase.init)
				//	{
				//		await Task.Delay(500).ConfigureAwait(false);

				//	}
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



				
				while(!Sim.isPastWarmup) {
					await Task.Delay(250).ConfigureAwait(false);
				}
				while(!City.myCities.Any()) {
					await Task.Delay(500).ConfigureAwait(false);
				}
				ShellPage.WorkEnd();

				if(Player.me.allianceId == 0) {
					const AllianceId aCQ = 4;
					const AllianceId aUN = 7;
					const AllianceId aPX = 8;
					
					AllianceId allianceId;
					AllianceTitle title;
					switch(Player.me.pid ) {
						//case 1049:  // Workaholic
						//	allianceId = allianceOffset+1;
						//	title = AllianceTitle.leader;
						//	break;
						//case 1054:  // popov
						//	allianceId = allianceOffset+1;
						//	title = AllianceTitle.officer;
						//	break;
						case 1056:  // Vindu
							allianceId = aCQ;
							title = AllianceTitle.secondLeader;
							break;
						case 1034: // coolasice
							allianceId = aCQ;
							title = AllianceTitle.leader;
							break;
						case 1051: // MonkeySuit
							allianceId = aCQ;
							title = AllianceTitle.officer;
							break;
						case 1008: // juan
							allianceId = aPX;
							title = AllianceTitle.leader;
							break;
						case 1053: // facetrolled
							allianceId = aPX;
							title = AllianceTitle.secondLeader;
							break;
						//case 1006: // tlgger
						//	allianceId = allianceOffset;
						//	title = AllianceTitle.officer;
						//	break;
						case 1012: // tingwing
							allianceId = aPX;
							title = AllianceTitle.secondLeader;
							break;
						case 1024: // avatar
							allianceId = aUN;
							title = AllianceTitle.secondLeader;
							break;
						case 1058: // OhDave
							allianceId = aUN;
							title = AllianceTitle.leader;
							break;
						default: {
								var a0 = Alliance.all[aCQ];
								var a1 = Alliance.all[aUN];
								var a2 = Alliance.all[aPX];
								if(a0.playerCount < a1.playerCount.Min(a2.playerCount) ) 
									allianceId = a0.id;
								else if(a1.playerCount <= a2.playerCount ) 
									allianceId=a1.id;
								else
									allianceId = a2.id;
								title = AllianceTitle.member;
								break;


							}



						
					}
					new CnVEventAlliance(allianceId,Player.me.id,title).EnqueueAsap();
					await Task.Delay(1000).ConfigureAwait(false);
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
	
		
	}
}
