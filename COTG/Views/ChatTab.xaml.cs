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
using COTG.Helpers;
using Windows.UI.Xaml.Documents;
using COTG.Game;
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

        public static ChatTab[] all = { world, alliance, officer, whisper, debug };

        public static string[] chatToId = { nameof(world), nameof(whisper), nameof(alliance), nameof(officer) };
        //        public DumbCollection<ChatEntry> logEntries = new DumbCollection<ChatEntry>(new ChatEntry[] { new ChatEntry("Hello") });
        public  DumbCollection<ChatEntryGroup> Groups { get; set; } = new DumbCollection<ChatEntryGroup>();// new[] { new ChatEntryGroup() {time=AUtil.dateTimeZero} });

        override public void VisibilityChanged(bool visible)
        {
            if (visible)
            {
                var count = Groups.Count;
                if (count > 0 )
                {
                    listView.ScrollIntoView(Groups[count - 1].Items.Last());
                }
                input.Focus(FocusState.Keyboard);
            }

        }

    public void Post(ChatEntry entry)
        {
            var activeGroup = Groups.Count > 0 ? Groups.Last() : null;
            var lastHour = activeGroup ==null ? -99 : activeGroup.time.Hour;
            var newHour = entry.time.Hour;
            if(lastHour!=newHour)
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
   
                    debug.Post(new ChatEntry(s,JSClient.ServerTime()));
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
            if(chatEntry!=null)
               JSClient.ShowPlayer(chatEntry.Content.ToString());
        }

        private void input_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (DataContext is string s)
            {
                int id = chatToId.IndexOf(s);
                if (id >= 0)
                {
                    if (e.Key == Windows.System.VirtualKey.Enter)
                    {
                     //   Log(input.Text);
                        JSClient.SendChat(id + 1, input.Text);
                        input.Text = "";
                    }
                }
                else
                {
                    Log("Invalid Chat: " + s);
                }
            }
        }
    }
}
