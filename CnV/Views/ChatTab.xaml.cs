﻿using CnV.Helpers;
using CommunityToolkit.WinUI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
//using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using CnVDiscord;
using static CnV.Debug;
using Microsoft.UI.Xaml.Documents;
using CnV.Game;
using System.Text.Json;
using Microsoft.UI;
using CnV.Services;
using System.Threading.Tasks;
using System.Text;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.UI.Controls;


namespace CnV.Views
{
	using System.Collections.Immutable;
	using CnV;

	using DSharpPlus.Entities;
	using Game;
	using Services;

	public sealed class ChatEntry
	{
		public string player { get; set; }
		public byte crown { get; set; }
		public byte type { get; internal set; }
		public const byte typeWorld = 1;
		public const byte typeWhisperFrom = 2;
		public const byte typeWhisperTo = 3;
		public const byte typeAlliance = 4;
		public const byte typeOfficer = 5;
		public const byte typeAnnounce = 6;
		public sbyte allignment;
		public HorizontalAlignment MsgAlignment => (AMath.random.Next(3) - 1) switch { -1 => HorizontalAlignment.Left, 1 => HorizontalAlignment.Right, _ => HorizontalAlignment.Center };
		public DateTimeOffset time;
#if DEBUG
		public string arrivedString => time.ToString("dd HH':'mm':'ss");
#else
		public string arrivedString => time.ToString("HH':'mm':'ss");
#endif
		public BitmapImage avatar { get; set; }


		public string text { get; set; } = string.Empty;
		const int maxMessageLength = 32 * 1024;
		static BitmapImage GetAvatar(string player)
		{
			return Player.FromNameOrNull(player)?.avatarBrush;
		}
		public ChatEntry(string _player, string _a, DateTimeOffset _time = default)
		{
			avatar = GetAvatar(_player);
			text = Note.TranslateCOTGChatToMarkdown(_a.Truncate(maxMessageLength));
			time = _time;
			player = _player;
		}
		public ChatEntry(string _player, string _text, DateTimeOffset _time, byte _type) 
		{
			avatar = GetAvatar(_player);
			text = _text.Truncate(maxMessageLength);
			time = _time;
			type = _type; 
			player = _player; 
		}



		public override string ToString()
		{
			return $"Chat:{player}:{arrivedString}:{type}:{text}";
		}
		//  public ChatEntry() { }

	}
	//public  sealed class ChatEntryGroup
	//{
	//    public DateTimeOffset time;
	//    public string Title => time.ToString("yyyy/MM/dd, HH:mm:ss");
	//    public DumbCollection<ChatEntry> Items { get; set; } = new DumbCollection<ChatEntry>();

	//    public override string ToString()
	//    {
	//        return this.Title;
	//    }
	//}

	public sealed partial class ChatTab : UserTab
	{
		public static ChatTab alliance;// = new ChatTab() { Tag = nameof(alliance) };
		public static ChatTab world;// = new ChatTab() { Tag = nameof(world) };
		public static ChatTab officer;// = new ChatTab() { Tag = nameof(officer) };
									  // public static ChatTab whisper = new ChatTab() { Tag = nameof(whisper) };

		public static ChatTab debug;// = new ChatTab() { Tag = nameof(debug) };
		public static ImmutableDictionary<ulong,ChatTab> discordChatTabs =  ImmutableDictionary<ulong,ChatTab>.Empty;
		public static ImmutableArray<ChatTab> all = ImmutableArray<ChatTab>.Empty;
		public DiscordChannel discordChannel; // 0 if not a discord Id
		
		public static void Ctor()
		{
			alliance = new ChatTab() { Tag = nameof(alliance) };
			world = new ChatTab() { Tag = nameof(world) };
			officer = new ChatTab() { Tag = nameof(officer) };
		// public static ChatTab whisper = new ChatTab() { Tag = nameof(whisper) };
			debug = new ChatTab() { Tag = nameof(debug) };

			all = (new ChatTab[] { alliance, world, officer, debug }).ToImmutableArray();
		}
		public string whisperTarget; // null if no target
		public DateTimeOffset lastRead;
		public static string[] chatToId = { nameof(world), "whisper", nameof(alliance), nameof(officer) };
		//        public DumbCollection<ChatEntry> logEntries = new DumbCollection<ChatEntry>(new ChatEntry[] { new ChatEntry("Hello") });
		// public DumbCollection<ChatEntryGroup> Groups { get; set; } = new DumbCollection<ChatEntryGroup>();// new[] { new ChatEntryGroup() {time=AUtil.dateTimeZero} });
		public NotifyCollection<ChatEntry> items { get; set; } = new ();
		override public Task VisibilityChanged(bool visible, bool longTerm)
		{
			if (items.Count > 0 && visible)
			{
				AppS.QueueOnUIThread(() =>
				
				{
					items.NotifyReset(true,true);
					listView.ScrollIntoView(items.Last());
		//			input.Focus(FocusState.Programmatic);
				});
			}

			return base.VisibilityChanged(visible, longTerm: longTerm);
		}

		internal static void CreateChatTab(DiscordChannel channel)
		{
			var tab = new ChatTab() { discordChannel = channel, Tag = channel.Name};
			all = all.Add(tab);
			tab.ShowOrAdd();
		}

	

		public override TabPage defaultPage => ChatTab.tabPage;
		//public ChatEntry lastChat = new ChatEntry(null, string.Empty, DateTimeOffset.MinValue, 0);
		public void Post(ChatEntry entry, bool isNew) // if is new, this message is fresh.  Otherwise loaded from archives
		{
		// this runs on the UI thread?
			// duplicate?
			//if (lastChat.player == entry.player
			//	&& string.Equals(lastChat.text, entry.text, StringComparison.Ordinal)
			//	&& lastChat.type == entry.type)
			//	return;
			//lastChat = entry;
			try
			{

				if (!isOpen)
				{
					ShowOrAdd(true, false);
				}
				var at = entry.time;
				//var activeGroup = Groups.Count > 0 ? Groups.Last() : null;
				//var lastHour = activeGroup == null ? -99 : activeGroup.time.Hour;
				//var newHour = entry.time.Hour;
				//if (lastHour != newHour)
				//{
				//    activeGroup = new ChatEntryGroup() { time = entry.time };
				//    Groups.Add(activeGroup);
				//}
				//{
				//	int count = items.Count;

				//	//foreach (var g in Groups)
				//	//    count += g.Items.Count;
				//	if (count >= maxItems)
				//	{
				//		for (int i = 0; i < 32; ++i)
				//			items.RemoveAt(0,false);
				//		if(notify)
				//			items.NotifyReset();
				//	}
				//}
				var insert = items.Count;
				if (!isNew)
				{
					for (; insert > 0 && items[insert - 1].time > at; --insert) { }
					if (insert > 0)
					{
						var lastChat = items[insert - 1];
						if (lastChat.player == entry.player
							&& string.Equals(lastChat.text, entry.text, StringComparison.Ordinal)
							&& lastChat.type == entry.type)
							return;
					}
				}
				items.Insert(insert, entry);
				var text = entry.text;
				if (isNew)
				{
					if (this != debug && (text.Contains(Player.myName, StringComparison.OrdinalIgnoreCase) || text.Contains("@here", StringComparison.OrdinalIgnoreCase)))
					{
						ToastNotificationsService.instance.ShowNotification(entry.ToString(), "mention");
						Note.Show(entry.ToString());
					}
				}

				// Set + if not from me
				if (entry.player != Player.myName && entry.player != null && entry.type != ChatEntry.typeWhisperTo)
				{
				//	Log(entry);
					SetPlus(true);
				}
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
		}
		public void Post(IEnumerable<ChatEntry> entries)
		{
			using var aaa = items.DeferChanges();
			// Todo: batch these
			foreach (var entry in entries)
				Post(entry,false);
		}


		public ChatTab()
		{
			this.InitializeComponent();

			//   Task.Delay(2000).ContinueWith((_) => App.(() =>
			//{ //ChatTip0.IsOpen = true;
			//   /// ChatTip1.IsOpen = true;
			//   // ChatTip2.IsOpen = true;
			//}));

		}
		//     private static readonly SemaphoreSlim _logSemaphore = new SemaphoreSlim(1, 1);

		public static void L(string s)
		{

			if (debug == null || !debug.isOpen)
				return;
			//              await _logSemaphore.WaitAsync();
			// try
			////  {
			AppS.DispatchOnUIThreadIdle(() =>
			{

				try
				{
					//  var str = $"{Tick.MSS()}:{s}";
					//  instance.logEntries

					debug.Post(new ChatEntry(null, s, CnVServer.ServerTime()), true);
				}
				catch (Exception e)
				{
					LogEx(e);
				}
			});

			//finally
			//{
			//    _logSemaphore.Release();
			//}
			//await Task.Delay(500);

			//await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(DispatcherQueuePriority.Low, () =>
			//{
			//    lock (logLock)
			//    {

			//        var ui = instance.logBox.TryGetElement(entries.Count - 1);
			//        if (ui != null)
			//            ui.StartBringIntoView();
			//      }
			//});
		}
		private void MarkdownTextBlock_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			Note.MarkDownLinkClicked(sender, e);
		}

		private void HyperlinkButton_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			var chatEntry = sender as HyperlinkButton;
			if (chatEntry != null)
				CnVServer.ShowPlayer(chatEntry.Content.ToString());
		}
		private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
		{
			var chatEntry = sender as HyperlinkButton;
			if (chatEntry != null)
				PasteToChatInput($"/w {chatEntry.Content.ToString()} ", false);

		}

		static List<string> messageCache = new List<string>();
		internal static TabPage tabPage;
		const int maxItems = 512;

		private void Paste(string s, bool afterInput)
		{
			try
			{
				if(afterInput)
					input.Text = input.Text + s;
				else
				{
					var text = input.Text;
					if(!text.IsNullOrEmpty() && s[0] == '/' && text[0] == '/') // strip other whispers
					{
						var index = text.IndexOf(' ',3);
						if(index >= 0)
							text = text.Substring(index);
					}
					input.Text = s + text;
				}

			}
			catch(Exception __ex)
			{
				Debug.LogEx(__ex);
			}
		}

		private void input_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			if (Tag is string s)
			{
				var isWhisperChannel = whisperTarget != null;
				//int id = isWhisperChannel ? 1 : chatToId.IndexOf(s);
				//if (id >= 0)
				{
					var sel = input.Text;
					if ((e.Key == Windows.System.VirtualKey.Up) || (e.Key == Windows.System.VirtualKey.Down))
					{
						if (messageCache.Count > 0)
						{
							var index = messageCache.IndexOf(sel);
							if (e.Key == Windows.System.VirtualKey.Up)
							{
								if (index <= 0)
								{
									index = messageCache.Count - 1;
									if (!sel.IsNullOrEmpty())
										messageCache.Add(sel);

								}
								else
								{
									--index;
								}
							}
							else
							{
								if (index == -1)
								{
									index = 0;
									if (!sel.IsNullOrEmpty())
									{
										messageCache.Insert(0, sel);
										++index;
									}


								}
								else
								{
									++index;
									if (index >= messageCache.Count)
										index = 0;
								}
							}
							input.SelectAll();
							input.SelectedText = messageCache[index];
						}
					}
					else if (e.Key == Windows.System.VirtualKey.Enter)
					{
						var str = input.Text;
						if (!str.IsNullOrEmpty())
						{

							//   Log(input.Text);
							messageCache.Remove(str); // remove duplicates
							messageCache.Add(str);
							// remove duplicates
							if (isWhisperChannel)
							{
								if (str[0] != '/')
								{
									str = $"/w {whisperTarget} {str}";
								}
							}
							input.Text = "";

							if (discordChannel is not null)
							{
									CnVChatClient.instance.connection.SendMessageAsync(new(){channelId=discordChannel.Id,memberId=Player.me.discordId,messageText=str});
							}
							else
							{
								int cotgId = isWhisperChannel ? 1 : chatToId.IndexOf(s);
								if ( cotgId >= 0)
								{
									CnVServer.SendChat(cotgId + 1, str);
								}

							}

							//{
							//	var count = items.Count;
							//	if (count > 0)
							//	{
							//		listView.ScrollIntoView(items.Last());
							//	}
							//}
						}

					}
				}
			}
		}

		internal static void Post(ulong channelId,ChatEntry chat,bool isNew,bool notify)
		{
			ChatTab t = null;
			foreach (var tab in all)
			{
				if (tab.discordChannel?.Id == channelId)
				{
					t = tab;
					break;
				}
			}
			if(t ==null)
				t = alliance;
			t.Post(chat, isNew);

		}

		public static ChatTab GetWhisperTab(string player, bool activate)
		{
			foreach (var w in all)
			{
				if (w.whisperTarget != null && w.whisperTarget == player)
				{
					if (activate)
						TabPage.Show(w);
					return w;
				}
			}
			var ch = new ChatTab() { Tag = player, whisperTarget = player };
			all = all.Add(ch);
			ch.ShowOrAdd(activate, false);
			return ch;
		}

		private static ChatEntry GetChatMessage(JsonElement msg)
		{
			if (!msg.TryGetProperty("b", out var info))
			{
				return new ChatEntry("", "Error");
			}
			var ch = new ChatEntry(info.GetAsString("b"), System.Net.WebUtility.HtmlDecode(info.GetAsString("d") ) )
			{
				crown = info.GetAsByte("c"),
				type = info.GetAsByte("a")
			};
			if (msg.TryGetProperty("c", out var c))
			{
				ch.time = c.GetString().ParseDateTime();
			}
			else
			{
				ch.time = CnVServer.ServerTime();
			}

			return ch;
		}
		// also for whisper
		public static void PasteToChatInput(string coords, bool afterInput = true)
		{
			if (coords[0] == '/')
			{
				var parseEnd = coords.IndexOf(' ', 3);
				var player = coords.Substring(3, parseEnd - 3);
				AppS.CopyTextToClipboard(player);
				var tab = ChatTab.GetWhisperTab(player, true);
				tab.Paste(coords, false);
				return;
			}
			//                afterInput = false;
			foreach (var tab in all)
				tab.Paste(coords, afterInput);

			var lg = coords.Length;  //  <coords>000:000</coords>
			if (lg == 24)
			{
				var c = coords.Substring(8, 7);
				AppS.CopyTextToClipboard(c);
				//               Note.Show($"[{c}](/c/{c}) posted to chat");
			}
		}

		public static void ProcessIncomingChat(JsonElement jsp)
		{
			var a = jsp.GetAsInt("a");
			switch (a)
			{
				case 444:
				case 555:
				case 333:
					{
						if (!jsp.TryGetProperty("b", out var messages))
							break;

						var batch = new List<ChatEntry>();
						foreach (var msg in messages.EnumerateArray())
						{
							batch.Add(GetChatMessage(msg));
						}
						int c = batch.Count-1;
						var epsilon = TimeSpan.FromSeconds(10);
						var lastTime = CnVServer.ServerTime() + epsilon;
						for (; c >= 0;--c)
						{
							while( lastTime < batch[c].time )
							{
								batch[c].time -= TimeSpan.FromDays(1); // days are missing damn it
							}
							lastTime = batch[c].time+epsilon;
						}

						if (a == 333)
						{
							ChatTab.world.Post(batch);
						}
						else
						{
							if (a != 444)
							{
								ChatTab.officer.Post(batch);
							}
							else
							{
								ChatTab.alliance.Post(batch);
							}
						}

					}
					break;
				case 4:
				case 5:
				case 3:
					{
						var ch = GetChatMessage(jsp);
						if (ch.type == ChatEntry.typeWhisperFrom || ch.type == ChatEntry.typeWhisperTo) // whisper
						{
							//if (ch.player == "Avatar" && ch.type == ChatEntry.typeWhisperFrom)
							//	PlayerHooks.PlayerChat?.Invoke(new PlayerHooks.PlayerChatEventArgs() { player = Player.FromName(ch.player), text = ch.text });

							// add to all tabs
							ch.text = $"`{(ch.type == ChatEntry.typeWhisperFrom ? "whispers" : "you whisper")}` {ch.text}";
							//		var prior = FocusManager.GetFocusedElement();
							//		Log(prior.GetType());

							ChatTab.GetWhisperTab(ch.player, false).Post(ch, true);
							// ChatTab.whisper.Post(ch);
							ChatTab.alliance.Post(ch,true);
							//       ChatTab.officer.Post(ch);
							//       ChatTab.world.Post(ch);
						}
						else
						{
							if (ch.type == 5)
							{
								ChatTab.officer.Post(ch, true);
							}
							if (ch.type == 5 || ch.type == 4)
							{

								//if (ch.type == 4)
								//	PlayerHooks.PlayerChat?.Invoke(new PlayerHooks.PlayerChatEventArgs() { player = Player.FromName(ch.player), text = ch.text });
								ChatTab.alliance.Post(ch, true);
							}
							else
								ChatTab.world.Post(ch, true);
						}
						break;
					}
			}
		}

		//private void input_PointerEntered(object sender, PointerRoutedEventArgs e)
		//{

		//    var rv=input.Focus(FocusState.Programmatic);
		//    Assert(rv == true);

		//}

		//private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
		//{
		//    e.KeyModifiers.UpdateKeyModifiers();

		//    var rv = input.Focus(FocusState.Programmatic);
		//    Assert(rv == true);

		//}

		//private void sender_PointerPressed(object sender, PointerRoutedEventArgs e)
		//{
		//    var chatEntry = sender as HyperlinkButton;
		//    var pt = e.GetCurrentPoint(chatEntry).Properties;

		//    if (pt.PointerUpdateKind == Windows.UI.Input.PointerUpdateKind.RightButtonPressed)
		//    {
		//        if (chatEntry != null)
		//            CnVServer.ShowPlayer(chatEntry.Content.ToString());
		//    }
		//    else if (pt.PointerUpdateKind == Windows.UI.Input.PointerUpdateKind.LeftButtonPressed)
		//    {
		//        if (chatEntry != null)
		//            PasteToChatInput($"/w {chatEntry.Content.ToString()} ");
		//    }
		//}





		//private async void MarkdownTextBlock_RightTapped(object sender, RightTappedRoutedEventArgs e)
		//{
		//    e.Handled = true;
		//    var msg = sender as MarkdownTextBlock;

		//    var ll = await Avatarslate.TouchAsync();
		//    var langs = await ll.GetLanguagesAsync();
		//    AppS.DispatchOnUIThreadLow(() =>
		//    {
		//        var fly = new MenuFlyout();
		//        var i = fly.Items;
		//        foreach (var l in langs)
		//        {
		//            MenuFlyoutItem item = new MenuFlyoutItem() { Text = l, Command = Avatarslate.instance, Tag = msg };
		//            item.CommandParameter = item;
		//            i.Add(item);
		//        }
		//        fly.CopyXamlRoomFrom(msg);
		//        fly.ShowAt(msg, e.GetPosition(msg));
		//    });
		//}

		//private async void input_Tapped(object sender, TappedRoutedEventArgs e)
		//{

		//}

		//private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
		//{

		//}

		//private async void input_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
		//{
		//    var msg = sender as TextBox;
		//    var fly = new MenuFlyout();
		//    var i = fly.Items;
		//    var ll = await Avatarslate.TouchAsync();
		//    var langs = await Avatarslate.GetLanguagesAsync();
		//    AppS.DispatchOnUIThreadLow(() =>
		//    {
		//        foreach (var l in langs)
		//        {
		//            MenuFlyoutItem item = new MenuFlyoutItem() { Text = l, Command = Avatarslate.instance, Tag = msg };
		//            item.CommandParameter = item;
		//            i.Add(item);
		//        }
		//        fly.ShowAt(msg);
		//    });

		//}

		//private async void input_RightTapped(object sender, RightTappedRoutedEventArgs e)
		//{
		//    e.Handled = true;
		//    var msg = sender as TextBox;
		//    var fly = new MenuFlyout();
		//    var i = fly.Items;
		//    var ll = await Avatarslate.TouchAsync();
		//    var langs = await Avatarslate.GetLanguagesAsync();
		//    AppS.DispatchOnUIThreadLow(() =>
		//    {
		//        foreach (var l in langs)
		//        {
		//            MenuFlyoutItem item = new MenuFlyoutItem() { Text = l, Command = Avatarslate.instance, Tag = msg };
		//            item.CommandParameter = item;
		//            i.Add(item);
		//        }
		//        fly.ShowAt(msg);
		//    });
		//}

		//private async void Button_Click(object sender, RoutedEventArgs e)
		//{
		//    var msg = sender as Button;
		//    var ll = await Avatarslate.TouchAsync();
		//    var langs = await Avatarslate.GetLanguagesAsync();
		//    AppS.DispatchOnUIThreadLow(() =>
		//    {
		//        var fly = new MenuFlyout();
		//        var i = fly.Items;
		//        foreach (var l in langs)
		//        {
		//            MenuFlyoutItem item = new MenuFlyoutItem() { Text = l, Command = Avatarslate.instance, Tag = input };
		//            item.CommandParameter = item;
		//            i.Add(item);
		//        }
		//        fly.CopyXamlRoomFrom(msg);
		//        fly.ShowAt(msg);
		//    });
		//}

		//private async void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
		//{

		//    //            e.Handled = true;
		//    var date = sender as TextBlock;
		//    var msg = date.Tag as MarkdownTextBlock;

		//    var ll = await Avatarslate.TouchAsync();
		//    var langs = await ll.GetLanguagesAsync();
		//    AppS.DispatchOnUIThreadLow(() =>
		//    {
		//        var fly = new MenuFlyout();
		//        var i = fly.Items;
		//        foreach (var l in langs)
		//        {
		//            MenuFlyoutItem item = new MenuFlyoutItem() { Text = l, Command = Avatarslate.instance, Tag = msg };
		//            item.CommandParameter = item;
		//            i.Add(item);
		//        }
		//        fly.CopyXamlRoomFrom(date);
		//        fly.ShowAt(date, e.GetPosition(date));
		//    });
		//}
		//static ChatTab hasFocus;
		private void input_GotFocus(object sender, RoutedEventArgs e)
		{
			SetPlus(false);
		//	hasFocus = this;
		}

		private void inputPointerOver(object sender, PointerRoutedEventArgs e)
		{
			//           Log("Tapped");
			//   listView.Focus(FocusState.Programmatic);
	//		if(input.Focus(FocusState) == focus )
	//			input.Focus(FocusState.Keyboard);
		}



		private void Copy_Click(object sender, RoutedEventArgs e)
		{
			var sb = new StringBuilder();
			var sel = listView.SelectedItems;
			if (sel.Any())
			{
				foreach (var _i in sel)
				{
					var i = _i as ChatEntry;
					sb.Append($"{i.arrivedString}:{i.player}:{i.text}\n");
				}
			}
			else
			{
				foreach (var _i in items)
				{
					var i = _i as ChatEntry;
					sb.Append($"{i.arrivedString}:{i.player}:{i.text}\n");
				}

			}
			AppS.CopyTextToClipboard(sb.ToString());
			Note.Show("Copied to clipboard");
		}
	}

	class HyperlinkColorConverter : IValueConverter
	{
		static SolidColorBrush[] brushes;
		static HyperlinkColorConverter()
		{

			brushes = new SolidColorBrush[ChatEntry.typeAnnounce + 1];
			brushes[1] = brushes[0] = new SolidColorBrush() { Color = Colors.Orange };
			brushes[2] = new SolidColorBrush() { Color = Colors.MediumPurple };
			brushes[3] = new SolidColorBrush() { Color = Colors.BlueViolet };
			brushes[4] = new SolidColorBrush() { Color = Colors.ForestGreen };
			brushes[5] = new SolidColorBrush() { Color = Colors.Cyan };
			brushes[ChatEntry.typeAnnounce] = new SolidColorBrush() { Color = Colors.Red };

		}
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var ce = value as ChatEntry;
			return brushes[ce.type];
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			LogEx(new NotImplementedException());
			return default;
		}
	}

	//object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
	//{
	//    throw new NotImplementedException();
	//}

	//object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
	//{
	//    throw new NotImplementedException();
	//}
}

