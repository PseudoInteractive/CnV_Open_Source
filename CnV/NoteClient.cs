using CnV.Game;
using CnV.Services;
using CnV.Views;
//using ZLogger;

//using Cysharp.Text;

using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
//using Microsoft.AppCenter;
//using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Notifications;
using CommunityToolkit.WinUI.UI.Controls;

using static CnV.Debug;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using CnV;

namespace CnV
{
	using Game;
	using Services;
	using Views;

	public static partial class Note
	{
	
		public static void Init()
		{
			//markDownText = new();
			//markDownText.TableCellPadding = new Thickness(0,0,0,0); // hack!
			//markDownText.ListMargin = new Thickness(0,0,0,0); // hack!
			//markDownText.Background = null;
			
		}

		//private static void InAppNote_Closed(object sender, InAppNotificationClosedEventArgs e)
		//{
		//	markDownText.Text = string.Empty;
		//	currentPriority = Priority.none;
		//	if (e.DismissKind == InAppNotificationDismissKind.User)
		//	{
		//		cancellationTokenSource.Cancel();
		//		cancellationTokenSource = new CancellationTokenSource();
		//	}
		//}
		// /* TODO: Hello [Avatar, 10/29/2021] */:
		// [Conditional("TRACE")]
		public static void L(string s)
		{
			ChatTab.L(s);
		}
		
		public static void ShowQuiet(string s,Debug.Priority priority = Debug.Priority.medium,bool useInfoBar = false,int timeout = 5000)
		{
			Show(s,priority,useInfoBar,timeout,false);
		}
	//	static Priority currentPriority;
	//	static DateTime nextInAppNote = new DateTime(0);
	//static MarkdownTextBlock markDownText;
	//	static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		internal static async partial void Show(string s, Debug.Priority priority=Debug.Priority.medium, bool useInfoBar = false, int timeout = 5000, bool showDebugOutput=true, bool showNote=true)
		{
			try
			{
				//if(showDebugOutput)
				//{
				//	AppS.DispatchOnUIThreadLow(() =>
				//	{
				//		ChatTab.L(s);
				//	});
				//}

				if(showNote && ShellPage.instance != null)
				{
					int noteDelay = Settings.notificationDuration;
					int noteDelayHigh = noteDelay+3;

					//if (!initialized)
					//{
					//	initialized = true;
					//	AppS.DispatchOnUIThread(() =>
					//	{
					//		ShellPage.inAppNote.Closed += InAppNote_Closed;
					//		//		ShellPage.instance.infoBar.CloseButtonClick += InfoBar_CloseButtonClick;
					//		//		ShellPage.instance.infoMD.LinkClicked += MarkDownLinkClicked;
					//	});
					//}
					//var now = DateTime.UtcNow;
					//var next = nextInAppNote;
					//var _priority = priority;
					//if (now >= next || ((priority >= Priority.high)))
					//{
					//	// all clear
					//	nextInAppNote = now + TimeSpan.FromSeconds(;
					//}
					//else
					//{
					//	if (priority == Priority.low)
					//		return;
					//	var wait = (next - now);
					//	if (wait.TotalSeconds >= 20.0f && priority < Priority.high)
					//		return;

					//	nextInAppNote = next + TimeSpan.FromSeconds(noteDelay);

					//	try
					//	{
					//		await Task.Delay(wait, cancellationTokenSource.Token);

					//	}
					//	catch (Exception _exception)
					//	{
					//		Log(_exception.Message);
					//		return;
					//	}

					//}

					//	AppS.DispatchOnUIThreadLow(() =>
					{
						//ChatTab.L(s);

						//	currentPriority = _priority;
						//if (ShellPage.instance.infoBar.IsOpen)
						//{
						//	wasOpen = true;
						//	ShellPage.instance.infoBar.IsOpen = false;
						//}
						//if (!useInfoBar)
						{
							try
							{
								//	var textBlock = markDownText;
								//if(ShellPage.instance.inAppNotes.Contains(s))
								//{
								//	return;
								//}
								//var textNull = ShellPage.instance.noteText.Length == 0;
								// update on screen
								AppS.DispatchOnUIThread( () =>
								{
									ShellPage.instance.inAppNotes.Add( CnVServer.ServerTime().ToString("HH':'mm':'ss") + "\t" + s );
								});
								//if(noteDelay < 30 )
								//{
								//	await Task.Delay(noteDelay*1000).ConfigureAwait(false);
								//	AppS.DispatchOnUIThread(() =>
								//	{
								//		try
								//		{
								//			ShellPage.instance.inAppNotes.RemoveAt(0);
								//		}
								//		catch(Exception e)
								//		{
								//			LogEx(e);
								//		}
								//	});

								//}

							}
							catch(Exception __ex)
							{
								Debug.LogEx(__ex);
							}

							//ShellPage.instance.infoBar.IsOpen = false;
						}
						//else
						//{
						//	var textBlock = new MarkdownTextBlock() { Text = s, Background = null };
						//	ShellPage.instance.infoMD.Text = s;
						//	if (wasOpen)
						//	{
						//		Task.Delay(500).ContinueWith((_) => ShellPage.instance.infoBar.IsOpen = true, TaskScheduler.FromCurrentSynchronizationContext());
						//	}
						//	else
						//	{
						//		ShellPage.instance.infoBar.IsOpen = true;
						//	}
						//}
					}



				}
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
		}

		//private static void InfoBar_CloseButtonClick(Microsoft.UI.Xaml.Controls.InfoBar sender, object args)
		//{
		//	cancellationTokenSource.Cancel();
		//	cancellationTokenSource = new CancellationTokenSource();
		//}

		static Regex regexCoordsTag = new Regex(@" ?\<coords\>(\d{1,3}:\d{1,3})\<\/coords\>", RegexOptions.CultureInvariant | RegexOptions.Compiled);
		static Regex regexPlayer = new Regex(@" ?\<player\>(\w+)\<\/player\>", RegexOptions.CultureInvariant | RegexOptions.Compiled);
		static Regex regexAlliance = new Regex(@" ?\<alliance\>(\w+)\<\/alliance\>", RegexOptions.CultureInvariant | RegexOptions.Compiled);
		static Regex regexReport = new Regex(@" ?\<report\>(\w+)\<\/report\>", RegexOptions.CultureInvariant | RegexOptions.Compiled);
		public static string TranslateCOTGChatToMarkdown(string s)
		{

			s = regexCoordsTag.Replace(s, @" [$1](/c/$1)");
			s = regexPlayer.Replace(s, @" [$1](/p/$1)");
			s = regexAlliance.Replace(s, @" [$1](/a/$1)");
			s = regexReport.Replace(s, @" [Report:$1](/r/$1)");
			return s;
		}
		public static void MarkDownLinkClicked(object sender, LinkClickedEventArgs e)
		{

			try
			{
				if (e.Link.StartsWith("http", StringComparison.OrdinalIgnoreCase)||e.Link.StartsWith("mailto"))
				{
					Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link,UriKind.Absolute));
				}
				else
				{
					var paths = e.Link.Split('/');
					Assert(paths[0].Length == 0);
					switch (paths[1])
					{
						case "s":
							{
								var cid = paths[2].FromCoordinate();
								DoSetup(cid);
							}
							break;
						case "c":
							Spot.ProcessCoordClick(paths[2].FromCoordinate(), false, AppS.keyModifiers, false);
							break;
						case "p": // player
							CnVServer.ShowPlayer(paths[2]);
							break;
						case "a": // Alliance
							CnVServer.ShowAlliance(paths[2]);
							break;
						case "r": // Report
							CnVServer.ShowReport(paths[2]);
							break;
					}
				}
			}
			catch (Exception ex)
			{
				LogEx(ex);
			}
		}

		private static async Task DoSetup(int cid)
		{
			try
			{
				await ShellPage.RefreshX();
			}
			catch (Exception ex)
			{
				LogEx(ex);
			}
			for (int i = 0; ; ++i)
			{
				await GetCity.Post(cid);
				if (City.CanVisit(cid))
					break;
				if( i > 5)
				{
					Trace("City not here");
				}
				await Task.Delay(1000);
			}
			await ShareString.Show(cid);
		}

		//public static void Focus(this Telerik.UI.Xaml.Controls.Grid.SfDataGrid ob)
		//{
		//	if (ob != null)
		//	{
		//	//	ShellPage.instance.commandBar.Focus(FocusState.Programmatic);

		//		AppS.DispatchOnUIThreadLow(() => ob.Focus(FocusState.Programmatic));
		//	}
		//}
		//public static void Focus(this Microsoft.UI.Xaml.Controls.Control ob)
		//{
		//	if (ob != null)
		//	{
		//	//	ShellPage.keyboardProxy.Focus(FocusState.Programmatic);

		//		AppS.DispatchOnUIThreadIdle(() => ob.Focus(FocusState.Programmatic));
		//	}
		//}

		static string lastTip;
		//public static void ProcessTooltipsOnPointerMoved(object sender, PointerRoutedEventArgs e)
		//{
		//	try
		//	{
		//		var info = Spot.HitTest(sender,e);
		//		var ctrl = info.column?.Column;
		//		var str = string.Empty;
		//		if(ctrl != null)
		//		{
		//			var _str = ToolTipService.GetToolTip(ctrl) as string;
		//			if(_str != null)
		//				str = _str;
		//		}
		//		ShowTip(str);
		//	}
		//	catch(Exception ex)
		//	{
		//		Log(ex);
		//	}
		//}
		public static void ShowTip(string str)
		{
			if(str != lastTip && TabPage.mainTabs?.tip is not null)
			{
				lastTip = str;
				AppS.DispatchOnUIThreadLow(() =>
			   TabPage.mainTabs.tip.Text = str); // Todo:  use the correct tabPage
			}
		}
		//public static void ProcessTooltips(this xDataGrid grid)
		//{
		//	grid.PointerMoved -= ProcessTooltipsOnPointerMoved;
		//	grid.PointerMoved += ProcessTooltipsOnPointerMoved;
		//}

	}
}
