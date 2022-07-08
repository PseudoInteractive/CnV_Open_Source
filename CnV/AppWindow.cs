using CommunityToolkit.WinUI.Helpers;

using Microsoft.AppCenter.Crashes;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace CnV;
using static AppS;

partial class App
{ 

		private static void AppWindow_Destroying(AppWindow sender,object args) {
			Log("Destroying");
		}


#if DEBUG
		private void DebugSettings_BindingFailed1(object sender,BindingFailedEventArgs e)
		{
			try
			{
				var txt = $"BindingFailed: {e.Message}, from {sender.GetType()},{sender}";

				Log(txt);
				Note.Show(txt);
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
		}
#endif

		internal async static void AppWindow_Closing(AppWindow sender,AppWindowClosingEventArgs args)
		{
			try
			{
				sender.Closing -= AppWindow_Closing;
				Log($"Closing!!: {AppS.state}  {AppS.windowState}");

				if(AppS.state <  AppS.State.closing)
				{
					AppS.SetState(AppS.State.closing);

					Microsoft.Xna.Framework.GamePlatform.isExiting=true;
					
					window.VisibilityChanged -= Window_VisibilityChanged;
					if(args is not null)
						args.Cancel = true;

					if(City.GetBuild().SaveLayout()) {
						Thread.Sleep(500);
					}

					// Cancel sim thread
					Sim.simCancelTokenSource.Cancel();
					SocketClient.ShutDown();
					if(CnVDiscord.CnVChatClient.initialized)
						Note.Show("Shutting down chat");
					var t1 = CnVDiscord.CnVChatClient.ShutDown(true);
					var t0 = BackgroundTask.dispatcherQueueController.ShutdownQueueAsync();

					await SwitchToBackground();
//					window.VisibilityChanged -= Window_VisibilityChanged;
//					window.Closed -= Window_Closed;
					Assert(AppS.state == AppS.State.closing);
					AppS.SetState(AppS.State.closed);
					var t2 = AAnalytics.Flush();
					await t0;
					await t1;
					await t2;
					// Wait for sim thread to save
					Log($"Await Sim");
					while(Sim.isSimRunning)
					{
						await Task.Delay(500);
					}
					if(Sim.simThread is not null)
						Sim.simThread.Join();
					Log($"Destroyed");
					//		args.Cancel=false;
					//				window.Close();
					App.instance.Exit();

				}
			}
			catch(Exception ex)
			{
				// Todo
				LogEx(ex);
			}
			finally
			{
			}
		}

	
		//		public static Windows.Foundation.IAsyncOperation<CoreWebView2Environment> createWebEnvironmentTask;

		private static async Task SwitchToBackground()
		{
			try
			{
				Log($"Switch to Background (was foreground: {isForeground})");
				if(state >= State.closed)
				{
					Assert(false);
					return;
				}
				while(AppS.windowState == AppS.WindowState.switching)
				{
					await Task.Delay(100);
				}
				if(state >= State.closed)
				{
					Assert(false);
					return;
				}
				if(AppS.windowState == AppS.WindowState.foreground)
				{
					AppS.windowState = AppS.WindowState.switching;

					//TODO: Save application state and stop any background activity
					try
					{

						var t0 = SaveState();

						var t = DateTimeOffset.UtcNow;
						var dt = t - activeStart;
						activeStart = t;
						//	Trace("Finished!1");

						try {
							AAnalytics.Track("Background",AAnalytics.defaultProperties.Add( "time", ((dt.TotalMinutes/15).RoundToInt()*15).ToString()  ));
							SystemInformation.Instance.AddToAppUptime(dt);
						}
						catch(Exception ex) { }
						await t0;
						AppS.windowState = AppS.WindowState.background;
						Log("Finished!");
					}
					catch(Exception)
					{
					}
				}
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
		}
	
		private static async void SwitchToForeground()
		{
			Log("Foreground");
			while(AppS.windowState == AppS.WindowState.switching)
			{
				await Task.Delay(100);
			}
			if(AppS.windowState == AppS.WindowState.background)
			{
				AppS.windowState = AppS.WindowState.foreground;
				var t = DateTimeOffset.UtcNow;
				var dt = t - activeStart;
				activeStart = t;
				AAnalytics.Track("Foreground",
							AAnalytics.defaultProperties.Add( "time",((dt.TotalMinutes/15).RoundToInt()*15).ToString()  ) );
				//	CnVServer.ResumeWebView();

			}
			//if (ShellPage.canvas != null)
			//    ShellPage.canvas.Paused = false;
		}

		static public int storageFull = 0;

		private void OnAppUnhandledException(object sender,Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			try
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine($"Unhandled Exception: " + e.Message);
				System.Diagnostics.Debug.WriteLine(e.Exception.StackTrace);
#endif



				if(AppS.RegisterException(e.Message))
				{
#if APPCENTER
					if(AAnalytics.initialized)
						Crashes.TrackError(e.Exception);
#endif
					AAnalytics.Track("UnhandledException",
									new Dictionary<string,string> { { "message",e.Message.Truncate(64) } });
				}

			}
			catch(Exception)
			{

				//LogEx(ex2);
				//					RegisterException(ex2.Message);


			}
		}

		private static Task SaveState()
		{
			var t0 = BuildQueue.SaveAll(true,false);
			var t1 = AttackTab.SaveAttacksBlock();
			var t2 = Settings.SaveAll();
			return Task.WhenAll(t0,t1,t2);
		}


		//private void Window_Activated(object sender,WindowActivatedEventArgs args)
		//{
		//	Trace("Activated");
		////	SwitchToForeground();
		//}

		private static async void Window_VisibilityChanged(object sender,WindowVisibilityChangedEventArgs args)

		{
			if(state >= State.closed)
			{
				Assert(false);
				return;
			}

			Log($"Visibility!!: {args.Visible}  {AppS.windowState}");
			AppS.windowState = AppS.WindowState.background;
			if(!args.Visible)
			{
				await SwitchToBackground();
			}
			else
			{
				SwitchToForeground();
			}


			//			throw new NotImplementedException();
		}
}