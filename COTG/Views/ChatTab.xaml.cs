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

    public sealed partial class ChatTab : UserControl
    {
        public static ChatTab alliance = new ChatTab() { DataContext = nameof(alliance) };
        public static ChatTab world = new ChatTab() { DataContext = nameof(world) };
        public static ChatTab officer = new ChatTab() { DataContext = nameof(officer) };
        public static ChatTab whisper = new ChatTab() { DataContext = nameof(whisper) };
        public static ChatTab debug = new ChatTab() { DataContext = nameof(debug) };

        public static ChatTab[] all = { world, alliance, officer, whisper, debug };

        //        public DumbCollection<ChatEntry> logEntries = new DumbCollection<ChatEntry>(new ChatEntry[] { new ChatEntry("Hello") });
        public  DumbCollection<ChatEntryGroup> Groups { get; set; } = new DumbCollection<ChatEntryGroup>();// new[] { new ChatEntryGroup() {time=AUtil.dateTimeZero} });
        public bool isActive; // true if this is in a tab view somewhere

        public void Post(ChatEntry entry)
        {
            var activeGroup = Groups.Count > 0 ? Groups.Last() : null;
            var lastHour = activeGroup ==null ? -99 : activeGroup.time.Hour;
            var newHour = entry.time.Hour;
            if(lastHour!=newHour)
            {
                activeGroup = new ChatEntryGroup() { time = entry.time };
                Groups.AddAndNotify(activeGroup);
            }
            activeGroup.Items.AddAndNotify(entry);

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

 //           Groups = new[] { new ChatEntryGroup() { time = DateTimeOffset.UtcNow,Items=logEntries }, new ChatEntryGroup() {Items=logEntries2 } }; 
//            Groups.First().Items = logEntries; // = new DumbCollection<ChatEntryGroup>(new[] { new ChatEntryGroup() { time = DateTimeOffset.UtcNow } });// Items = logEntries } });
//            Groups.First().Items = logEntries; // = new DumbCollection<ChatEntryGroup>(new[] { new ChatEntryGroup() { time = DateTimeOffset.UtcNow } });// Items = logEntries } });
                                               //   logEntries[0].group = Groups[0];

            //            cvsGroups.Source =(from t in logEntries
            //    group t by t.@group into g
            //    orderby g.Key
            //    select g);
            ////            groupInfoCVS.Source = result;

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
    }
}
