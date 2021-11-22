using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COTG.Game;
using COTG.Services;
using COTG.Views;
using System.Numerics;
using COTG.JSON;
using Microsoft.AppCenter;
using CommunityToolkit.WinUI.Helpers;
using System.Runtime;

namespace COTG
{
	using CnVChat;
	using CnVDiscord;

	public partial class AGame
	{
	public static async void InitializeForWorld()
	{

		//					   CnVChatClient.instance = new();
		//					   CnVChatClient.instance.Initialize();
		try
		{
			Assert( JSClient.world != 0);
			ShellPage.SetViewModeCity();
			
			APlayfab.Login();

			//CnVChatClient.CnVChatClient.Setup();

			GetWorldInfo.Send();
			ShellPage.canvasVisible = true;
			//   ShellPage.isHitTestVisible = true;
			///                   await GetCitylistOverview();
			Task.Delay(3000).ContinueWith((_) => City.UpdateSenatorInfo());  // no async
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

			JSClient.SetStayAlive(SettingsPage.stayAlive);
			//{
			//    //var now = DateTime.UtcNow;
			//    //if (now.Day <= 28 && now.Month==11)
			//    {
			{
				AppCenter.SetUserId(Player.myName);
				//AppCenter.Analytics.Properties.put("UserId", "your user Id");
				CustomProperties properties = new CustomProperties();
				properties.Set("alliance",Alliance.myId).Set("allianceName",Alliance.my.name).Set("world",JSClient.world).Set("sub",JSClient.isSub).Set("playerId",Player.myId).Set("UserId",Player.myName);
				AppCenter.SetCustomProperties(properties);
				AAnalytics.Track("GotCreds",new Dictionary<string,string>() { { "World",JSClient.world.ToString() },{ "sub",JSClient.isSub.ToString() },{ "UserId",Player.myName } });
				ShellPage.UpdateFocus();
			}

			// 
			// Friend.LoadAll();

			App.state = App.State.active;
			CnVDiscord.Discord.Initialize();

			//  System.GC.Collect(2,GCCollectionMode.Default,true);

			// give some time for initial pressure to come down
			TabPage.ShowTabs();
			CityCustom.Load();
			await Task.Delay(1000);

		//	System.GC.Collect(2,GCCollectionMode.Default,true,true);
		//	GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
		}
		catch(Exception ex)
		{
			Log(ex);
		}
		}
	
	
	}
}
