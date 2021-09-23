using COTG.Game;
using COTG.Services;
using COTG.Views;
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

using static COTG.Debug;
using Microsoft.UI.Xaml.Controls;

namespace COTG
{
	public static class Note
	{
		static bool initialized = false;
		static Note()
		{
		}

		private static void InAppNote_Closed(object sender, InAppNotificationClosedEventArgs e)
		{
			currentPriority = Priority.none;
			if (e.DismissKind == InAppNotificationDismissKind.User)
			{
				cancellationTokenSource.Cancel();
				cancellationTokenSource = new CancellationTokenSource();
			}
		}

		// [Conditional("TRACE")]
		public static void L(string s)
		{
			ChatTab.L(s);
		}
		public enum Priority
		{
			none,
			low, // if one is active, drop this
			medium, // if one is active wait
			high // if one is active cancel it
		}
		static Priority currentPriority;
		static DateTime nextInAppNote = new DateTime(0);
		static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		public static async void Show(string s, Priority priority=Priority.medium, bool useInfoBar = false, int timeout = 5000)
		{
			const int noteDelay = 2;
			const int noteDelayHigh = 5;
			if(ShellPage.instance != null)
			{
				if (!initialized)
				{
					initialized = true;
					App.DispatchOnUIThread(() =>
					{
						ShellPage.inAppNote.Closed += InAppNote_Closed;
						//		ShellPage.instance.infoBar.CloseButtonClick += InfoBar_CloseButtonClick;
						//		ShellPage.instance.infoMD.LinkClicked += MarkDownLinkClicked;
					});
				}
				App.DispatchOnUIThreadLow(() =>
				{
					ChatTab.L(s);
				});

				var now = DateTime.UtcNow;
				var next = nextInAppNote;
				var _priority = priority;
				if (now >= next || ((priority >= Priority.high)))
				{
					// all clear
					nextInAppNote = now + TimeSpan.FromSeconds((priority >= Priority.high) ? noteDelayHigh : noteDelay);
				}
				else
				{
					if (priority == Priority.low)
						return;
					var wait = (next - now);
					if (wait.TotalSeconds >= 20.0f && priority < Priority.high)
						return;

					nextInAppNote = next + TimeSpan.FromSeconds(noteDelay);

					try
					{
						await Task.Delay(wait, cancellationTokenSource.Token);

					}
					catch (Exception _exception)
					{
						Log(_exception.Message);
						return;
					}

				}

				App.DispatchOnUIThreadLow(() =>
				{
					//ChatTab.L(s);
					var wasOpen = false;
					currentPriority = _priority;
					//if (ShellPage.instance.infoBar.IsOpen)
					//{
					//	wasOpen = true;
					//	ShellPage.instance.infoBar.IsOpen = false;
					//}
					//if (!useInfoBar)
					{
						var textBlock = new MarkdownTextBlock() { Text = s, Background = null };
						textBlock.LinkClicked += MarkDownLinkClicked;
						ShellPage.inAppNote.Show(textBlock, timeout);

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
				});


			}
		}

		private static void InfoBar_CloseButtonClick(Microsoft.UI.Xaml.Controls.InfoBar sender, object args)
		{
			cancellationTokenSource.Cancel();
			cancellationTokenSource = new CancellationTokenSource();
		}

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
							Spot.ProcessCoordClick(paths[2].FromCoordinate(), false, App.keyModifiers, false);
							break;
						case "p": // player
							JSClient.ShowPlayer(paths[2]);
							break;
						case "a": // Alliance
							JSClient.ShowAlliance(paths[2]);
							break;
						case "r": // Report
							JSClient.ShowReport(paths[2]);
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
				await ShellPage.instance.RefreshX();
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

			await 	App.DispatchOnUIThreadExclusive(cid, async () =>
			{
				return await CityRename.RenameDialog(cid, true);

			});
		}

		//public static void Focus(this Telerik.UI.Xaml.Controls.Grid.RadDataGrid ob)
		//{
		//	if (ob != null)
		//	{
		//	//	ShellPage.instance.commandBar.Focus(FocusState.Programmatic);

		//		App.DispatchOnUIThreadLow(() => ob.Focus(FocusState.Programmatic));
		//	}
		//}
		//public static void Focus(this Microsoft.UI.Xaml.Controls.Control ob)
		//{
		//	if (ob != null)
		//	{
		//	//	ShellPage.keyboardProxy.Focus(FocusState.Programmatic);

		//		App.DispatchOnUIThreadIdle(() => ob.Focus(FocusState.Programmatic));
		//	}
		//}

		static string lastTip;
		public static void ProcessTooltipsOnPointerMoved(object sender, PointerRoutedEventArgs e)
		{
			var info = Spot.HitTest(sender, e);
			var ctrl = info.column?.Column;
			var str =string.Empty;
			if(ctrl != null)
			{
				var _str = ToolTipService.GetToolTip(ctrl) as string;
				if(_str != null)
					str = _str;
			}
			ShowTip(str);
		}
		public static void ShowTip(string str)
		{
			if (str != lastTip)
			{
				lastTip = str;
				App.DispatchOnUIThreadLow(() =>
			   TabPage.mainTabs.tip.Text = str); // Todo:  use the correct tabPage
			}
		}
		public static void ProcessTooltips(this Telerik.UI.Xaml.Controls.Grid.RadDataGrid grid)
		{
			grid.PointerMoved -= ProcessTooltipsOnPointerMoved;
			grid.PointerMoved += ProcessTooltipsOnPointerMoved;
		}
		
	}
}
