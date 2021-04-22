using COTG.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;
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
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using static COTG.Debug;
using Windows.UI.Xaml.Documents;
using COTG.Game;
using System.Text.Json;
using Windows.UI;
using COTG.Services;
using System.Threading.Tasks;
using System.Text;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace COTG.Views
{
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
		public const byte typeAnnounce =6;
		public sbyte allignment;
        public HorizontalAlignment MsgAlignment => (AMath.random.Next(3)-1) switch { -1 => HorizontalAlignment.Left, 1 => HorizontalAlignment.Right, _ => HorizontalAlignment.Center };
        public DateTimeOffset time;
        public string arrivedString => time.ToString("HH':'mm':'ss");
        public string text { get; set; } = string.Empty;
		const int maxMessageLength = 32 * 1024;
        public ChatEntry(string _a, DateTimeOffset _time = default) { text = Note.TranslateCOTGChatToMarkdown(_a.Truncate(maxMessageLength) ); time = _time; }
        public ChatEntry(string _player, string _text, DateTimeOffset _time, byte _type) { text = _text.Truncate(maxMessageLength); time = _time; type = _type; player = _player; }
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
        public static ChatTab alliance = new ChatTab() { Tag = nameof(alliance) };
        public static ChatTab world = new ChatTab() { Tag = nameof(world) };
        public static ChatTab officer = new ChatTab() { Tag = nameof(officer) };
        // public static ChatTab whisper = new ChatTab() { Tag = nameof(whisper) };
        public static ChatTab debug = new ChatTab() { Tag = nameof(debug) };

        public static ChatTab[] all = Array.Empty<ChatTab>();
        public static ChatTab[] Ctor()
        {
            all = new ChatTab[] { alliance, world, officer, debug };
            return all;
        }
        public string whisperTarget; // null if no target
        public DateTimeOffset lastRead;
        public static string[] chatToId = { nameof(world), "whisper", nameof(alliance), nameof(officer) };
        //        public DumbCollection<ChatEntry> logEntries = new DumbCollection<ChatEntry>(new ChatEntry[] { new ChatEntry("Hello") });
        // public DumbCollection<ChatEntryGroup> Groups { get; set; } = new DumbCollection<ChatEntryGroup>();// new[] { new ChatEntryGroup() {time=AUtil.dateTimeZero} });
        public DumbCollection<ChatEntry> items { get; set; } = new DumbCollection<ChatEntry>();
        override public void VisibilityChanged(bool visible)
        {
            if (items.Count > 0 && visible)
            {
                listView.ScrollIntoView(items.Last());
                input.Focus(FocusState.Programmatic);
            }

            base.VisibilityChanged(visible);
        }

        public void Post(ChatEntry entry)
        {
            if (!isActive)
            {

                tabPage.AddTab(this, false);
            }
            //var activeGroup = Groups.Count > 0 ? Groups.Last() : null;
            //var lastHour = activeGroup == null ? -99 : activeGroup.time.Hour;
            //var newHour = entry.time.Hour;
            //if (lastHour != newHour)
            //{
            //    activeGroup = new ChatEntryGroup() { time = entry.time };
            //    Groups.Add(activeGroup);
            //}
            int count = items.Count;

            //foreach (var g in Groups)
            //    count += g.Items.Count;
            if (count >= maxItems)
            {
                for(int i=0;i<32;++i)
                    items.RemoveAt(0);
            }
            items.Add(entry);
			var text = entry.text;
			if (this != debug && (text.Contains(Player.myName, StringComparison.OrdinalIgnoreCase) || text.Contains("@", StringComparison.Ordinal)))
            {
                Note.Show(entry.text);

            }
            // Set + if not from me
            if (entry.player != Player.myName && entry.player != null && entry.type != ChatEntry.typeWhisperTo)
                SetPlus(true);

        }
        public void Post(IEnumerable<ChatEntry> entries)
        {
            // Todo: batch these
            foreach (var entry in entries)
                Post(entry);
        }


        public ChatTab()
        {
            this.InitializeComponent();

            //   Task.Delay(2000).ContinueWith((_) => App.DispatchOnUIThreadLow(() =>
            //{ //ChatTip0.IsOpen = true;
            //   /// ChatTip1.IsOpen = true;
            //   // ChatTip2.IsOpen = true;
            //}));

        }
        //     private static readonly SemaphoreSlim _logSemaphore = new SemaphoreSlim(1, 1);
        // [Conditional("TRACE")]
        public static void L(string s)
        {

            if (debug == null || !debug.isActive)
                return;
            //              await _logSemaphore.WaitAsync();
            // try
            ////  {
            App.DispatchOnUIThreadSneakyLow(() =>
            {

                try
                {
                    //  var str = $"{Tick.MSS()}:{s}";
                    //  instance.logEntries

                    debug.Post(new ChatEntry(s, JSClient.ServerTime()));
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

            //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
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
                JSClient.ShowPlayer(chatEntry.Content.ToString());
        }
        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var chatEntry = sender as HyperlinkButton;
            if (chatEntry != null)
                PasteToChatInput($"/w {chatEntry.Content.ToString()} ", false);

        }

        static List<string> messageCache = new List<string>();
        internal static TabPage tabPage;
        const int maxItems = 300;

        private void Paste(string s, bool afterInput)
        {
            if (afterInput)
                input.Text = input.Text + s;
            else
            {
                var text = input.Text;
                if (!text.IsNullOrEmpty() && s[0] == '/' && text[0]=='/') // strip other whispers
                {
                    var index = text.IndexOf(' ', 3);
                    if (index >= 0)
                        text = text.Substring(index);
                }
                input.Text = s + text;
            }
        }
		
        private void input_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (Tag is string s)
            {
                var isWhisperChannel = whisperTarget!=null;
                int id = isWhisperChannel ? 1 : chatToId.IndexOf(s);
                if (id >= 0)
                {
                    var sel = input.Text;
                    if ((e.Key == Windows.System.VirtualKey.Up)||(e.Key == Windows.System.VirtualKey.Down))
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
                            JSClient.SendChat(id + 1, str);

                            {
                                var count = items.Count;
                                if (count > 0)
                                {
                                    listView.ScrollIntoView(items.Last());
                                }
                            }
                        }

                    }
                }
                else
                {
                    Log("Invalid Chat: " + s);
                }
            }
        }
        public static ChatTab GetWhisperTab(string player, bool activate)
        {
            foreach (var w in all)
            {
                if (w.whisperTarget!=null && w.whisperTarget == player)
                {
                    if (activate)
                        TabPage.Show(w);
                    return w;
                }
            }
            var ch = new ChatTab() { Tag=player, whisperTarget = player };
            all = all.ArrayAppend(ch);
            tabPage.AddTab(ch, activate);
            return ch;
        }

        private static ChatEntry GetChatMessage(JsonElement msg)
        {
            if (!msg.TryGetProperty("b", out var info))
            {
                return new ChatEntry("Error");
            }
            var ch = new ChatEntry(System.Net.WebUtility.HtmlDecode(info.GetAsString("d")))
            {
                player = info.GetAsString("b"),
                crown = info.GetAsByte("c"),
                type = info.GetAsByte("a")
            };
            if (msg.TryGetProperty("c", out var c))
            {
                ch.time = c.GetString().ParseDateTime();
            }
            else
            {
                ch.time = JSClient.ServerTime();
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
                App.CopyTextToClipboard(player);
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
                App.CopyTextToClipboard(c);
                //               Note.Show($"[{c}](/c/{c}) posted to chat");
            }

        }

        public static void ProcessIncomingChat(JsonProperty jsp)
        {
            var a = jsp.Value.GetAsInt("a");
            switch (a)
            {
                case 444:
                case 555:
                case 333:
                    {
                        if (!jsp.Value.TryGetProperty("b", out var messages))
                            break;

                        var batch = new List<ChatEntry>();
                        foreach (var msg in messages.EnumerateArray())
                        {
                            batch.Add(GetChatMessage(msg));
                        }
                        (a switch { 444 => ChatTab.alliance, 333 => ChatTab.world, _ => ChatTab.officer }).Post(batch);

                    }
                    break;
                case 4:
                case 5:
                case 3:
                    {
                        var ch = GetChatMessage(jsp.Value);
                        if (ch.type == ChatEntry.typeWhisperFrom || ch.type == ChatEntry.typeWhisperTo) // whisper
                        {
                            // add to all tabs
                            ch.text = $"`{(ch.type == ChatEntry.typeWhisperFrom ? "whispers" : "you whisper")}` {ch.text}";
							var prior = FocusManager.GetFocusedElement();
							Log(prior.GetType());

							ChatTab.GetWhisperTab(ch.player, (prior as TextBox == null) ).Post(ch);
                            // ChatTab.whisper.Post(ch);
                            //       ChatTab.alliance.Post(ch);
                            //       ChatTab.officer.Post(ch);
                            //       ChatTab.world.Post(ch);
                        }
                        else
                        {
                            (ch.type switch { 4 => ChatTab.alliance, 5 => ChatTab.officer, _ => ChatTab.world }).Post(ch);
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
        //            JSClient.ShowPlayer(chatEntry.Content.ToString());
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
        //    App.DispatchOnUIThreadSneaky(() =>
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
        //    App.DispatchOnUIThreadSneaky(() =>
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
        //    App.DispatchOnUIThreadSneaky(() =>
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
        //    App.DispatchOnUIThreadSneaky(() =>
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
        //    App.DispatchOnUIThreadSneaky(() =>
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

        private void input_GotFocus(object sender, RoutedEventArgs e)
        {
            SetPlus(false);
        }

        private void inputPointerOver(object sender, PointerRoutedEventArgs e)
        {
            //           Log("Tapped");
            listView.Focus(FocusState.Programmatic);
			App.DispatchOnUIThread(()=>input.Focus(FocusState.Programmatic) );
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
			App.CopyTextToClipboard(sb.ToString());
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
			brushes[2] = new SolidColorBrush() { Color = Colors.Magenta };
			brushes[3] = new SolidColorBrush() { Color = Colors.MediumVioletRed };
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
            throw new NotImplementedException();
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

