using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CnV.Game;
using CnV.Services;
using CnV.Views;
using System.Numerics;
#if AppCenter
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

		public static async void InitializeForWorld()
	{

		//					   CnVChatClient.instance = new();
		//					   CnVChatClient.instance.Initialize();
		try
		{
#if CNV
			var signin = new Signin();
			await signin.ShowAsync2();
#else
			await APlayFab.SigninLegacy(Player.myName);
#endif
			Assert( JSClient.world != 0);
			ShellPage.SetViewModeCity();
			

			//CnVChatClient.CnVChatClient.Setup();

			GetWorldInfo.Send();
			ShellPage.canvasVisible = true;
			//   ShellPage.isHitTestVisible = true;
			///                   await GetCitylistOverview();
			TileData.Ctor(false);
			//if (TipsSeen.instance.refresh == false
			//||TipsSeen.instance.chat0==false
			//|| TipsSeen.instance.chat1 == false
			//|| TipsSeen.instance.chat2 == false)
			//    App.QueueIdleTask(ShellPage.ShowTipRefresh);
			// await RaidOverview.Send();
			App.QueueIdleTask(IncomingOverview.ProcessTask,1000);
			App.QueueIdleTask(OutgoingOverview.ProcessTask,1000);

			Task.Run(async () =>
			{
				await CnVChatClient.Setup();
			});

			JSClient.SetStayAlive(Settings.stayAlive);
			//{
			//    //var now = DateTime.UtcNow;
			//    //if (now.Day <= 28 && now.Month==11)
			//    {
#if AppCenter

			{
				AppCenter.SetUserId(Player.myName);
				//AppCenter.Analytics.Properties.put("UserId", "your user Id");
				CustomProperties properties = new CustomProperties();
				properties.Set("alliance",Alliance.myId).Set("allianceName",Alliance.my.name).Set("world",JSClient.world).Set("sub",JSClient.isSub).Set("playerId",Player.myId).Set("UserId",Player.myName);
				AppCenter.SetCustomProperties(properties);
				AAnalytics.Track("GotCreds",new Dictionary<string,string>() { { "World",JSClient.world.ToString() },{ "sub",JSClient.isSub.ToString() },{ "UserId",Player.myName } });
				ShellPage.UpdateFocus();
			}
#endif
			// 
			// Friend.LoadAll();

			App.state = App.State.active;
		//	CnVDiscord.Discord.Initialize();

			//  System.GC.Collect(2,GCCollectionMode.Default,true);

			// give some time for initial pressure to come down
			TabPage.ShowTabs();
			CityCustom.Load();

			ShellPage.updateHtmlOffsets.Go(true);

				//	await Task.Delay(1000);

				//	System.GC.Collect(2,GCCollectionMode.Default,true,true);

				//	GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
				reinforcementsTask = new(interval:64.0f,()=> ReinforcementsOverview.instance.Post(),initialDelay:4.0f );
				senInfoTask        = new( interval: 68f, City.UpdateSenatorInfo, 3.0f);
				CnVServer.isInitialized      = true;

				AGame.ClientDraw = ClientDraw.Draw;
	
		}
		catch(Exception ex)
		{
			Log(ex);
		}
		}
	
	
	}
}
