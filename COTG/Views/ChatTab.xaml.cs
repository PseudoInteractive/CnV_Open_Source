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
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace COTG.Views
{
    public sealed class ChatEntry
    {
        public string player { get; set; }
        public byte crown { get; set; }
        public byte type { get; internal set; }
        public sbyte allignment;
        public HorizontalAlignment MsgAlignment => (AMath.random.Next(3)-1)  switch { -1 => HorizontalAlignment.Left, 1 => HorizontalAlignment.Right, _ => HorizontalAlignment.Center };
        public DateTimeOffset time;
        public string arrivedString => time.ToString("HH':'mm':'ss");
        public string text { get; set; }= string.Empty;

        public ChatEntry(string _a,DateTimeOffset _time = default) { text = Note.TranslateCOTGChatToMarkdown(_a); time = _time; }
      //  public ChatEntry() { }
    }
    public  class ChatEntryGroup
    {
        public DateTimeOffset time;
        public string Title => time.ToString("HH:mm:ss");
        public DumbCollection<ChatEntry> Items { get; set; } = new DumbCollection<ChatEntry>();

        public override string ToString()
        {
            return this.Title;
        }
    }

    public sealed partial class ChatTab : UserTab
    {
        public static ChatTab alliance = new ChatTab() { DataContext = nameof(alliance) };
        public static ChatTab world = new ChatTab() { DataContext = nameof(world) };
        public static ChatTab officer = new ChatTab() { DataContext = nameof(officer) };
        public static ChatTab whisper = new ChatTab() { DataContext = nameof(whisper) };
        public static ChatTab debug = new ChatTab() { DataContext = nameof(debug) };

        public static ChatTab[] all = Array.Empty<ChatTab>();
        public static ChatTab[] Ctor()
        {
           all = new ChatTab[]{ world, alliance, officer, whisper, debug };
            return all;
        }
        public static string[] chatToId = { nameof(world), nameof(whisper), nameof(alliance), nameof(officer) };
        //        public DumbCollection<ChatEntry> logEntries = new DumbCollection<ChatEntry>(new ChatEntry[] { new ChatEntry("Hello") });
        public DumbCollection<ChatEntryGroup> Groups { get; set; } = new DumbCollection<ChatEntryGroup>();// new[] { new ChatEntryGroup() {time=AUtil.dateTimeZero} });

        override public void VisibilityChanged(bool visible)
        {
            if (visible)
            {
                var count = Groups.Count;
                if (count > 0)
                {
                    listView.ScrollIntoView(Groups[count - 1].Items.Last());
                }
                input.Focus(FocusState.Keyboard);
            }

            base.VisibilityChanged(visible);
        }

        public void Post(ChatEntry entry)
        {
            var activeGroup = Groups.Count > 0 ? Groups.Last() : null;
            var lastHour = activeGroup == null ? -99 : activeGroup.time.Hour;
            var newHour = entry.time.Hour;
            if (lastHour != newHour)
            {
                activeGroup = new ChatEntryGroup() { time = entry.time };
                Groups.Add(activeGroup);
            }
            activeGroup.Items.Add(entry);

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


        }
        //     private static readonly SemaphoreSlim _logSemaphore = new SemaphoreSlim(1, 1);
        [Conditional("TRACE")]
        public static void L(string s)
        {

            if (debug == null)
                return;
            //              await _logSemaphore.WaitAsync();
            // try
            ////  {
            debug.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => {

                try
                {
                    //  var str = $"{Tick.MSS()}:{s}";
                    //  instance.logEntries

                    debug.Post(new ChatEntry(s, JSClient.ServerTime()));
                }
                catch (Exception e)
                {
                    Log(e);
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



        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var chatEntry = sender as HyperlinkButton;
            if (chatEntry != null)
                JSClient.ShowPlayer(chatEntry.Content.ToString());
        }
        static List<string> messageCache = new List<string>();
        private void Paste(string s)
        {
            input.Text = input.Text + s;
        }
        private void input_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (DataContext is string s)
            {
                int id = chatToId.IndexOf(s);
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
                                    if(!sel.IsNullOrEmpty())
                                        messageCache.Add(sel);
                                    
                                }
                                else
                                {
                                    --index;
                                }
                            }
                            else
                            {
                                if(index == -1)
                                {
                                    index = 0;
                                    if(!sel.IsNullOrEmpty() )
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
                        //   Log(input.Text);
                        messageCache.Remove(str); // remove duplicates
                        messageCache.Add(str);
                        // remove duplicates

                        JSClient.SendChat(id + 1, str);
                        input.Text = "";
                    }
                }
                else
                {
                    Log("Invalid Chat: " + s);
                }
            }
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
        public static void PasteCoords(string coords)
        {
            foreach (var tab in all)
                tab.Paste(coords);

            
            var lg = coords.Length;  //  <coords>000:000</coords>
            if (lg == 24)
            {
            //    var c = coords.Substring(8, 7);
           //     Note.Show($"[{c}](/c/{c}) posted to chat");
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
                        if (ch.type == 2 || ch.type == 3) // whisper
                        {
                            // add to all tabs
                            ch.text = $"`{(ch.type==2?"whispers":"you whisper")}` {ch.text}";
                            ChatTab.whisper.Post(ch);
                            ChatTab.alliance.Post(ch);
                            ChatTab.officer.Post(ch);
                            ChatTab.world.Post(ch);
                        }
                        else
                        {
                            (ch.type switch { 4 => ChatTab.alliance, 5 => ChatTab.officer, _ => ChatTab.world }).Post(ch);
                        }
                        break;
                    }
            }
        }
    }
}
