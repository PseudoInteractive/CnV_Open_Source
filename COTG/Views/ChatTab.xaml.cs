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
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace COTG.Views
{
    public sealed class LogEntry
    {
        public string player { get; set; }
        public byte crown { get; set; }
        public byte whisper { get; internal set; }
        public sbyte allignment;
        public HorizontalAlignment MsgAlignment => (AMath.random.Next(3)-1)  switch { -1 => HorizontalAlignment.Left, 1 => HorizontalAlignment.Right, _ => HorizontalAlignment.Center };
        public DateTimeOffset arrived;
        public string arrivedString => arrived.ToString("HH':'mm':'ss");
        public string text { get; set; }= string.Empty;

        public LogEntry(string _a) { text = _a; }
        public LogEntry() { }
    }

    public sealed partial class ChatTab : UserControl
    {
        public static ChatTab alliance = new ChatTab() { DataContext = nameof(alliance) };
        public static ChatTab world = new ChatTab() { DataContext = nameof(world) };
        public static ChatTab officer = new ChatTab() { DataContext = nameof(officer) };
        public static ChatTab whisper = new ChatTab() { DataContext = nameof(whisper) };
        public static ChatTab debug = new ChatTab() { DataContext = nameof(debug) };

        public static ChatTab[] all = { world, alliance, officer, whisper, debug };

        public DumbCollection<LogEntry> logEntries = new DumbCollection<LogEntry>(new LogEntry[] {new LogEntry("Hello") });

        public bool isActive; // true if this is in a tab view somewhere
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
                    var str = $"{Tick.MSS()}:{s}";
                    //  instance.logEntries
                    var entries = debug.logEntries;

                    entries.AddAndNotify(new LogEntry(str));
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
       

    }
}
