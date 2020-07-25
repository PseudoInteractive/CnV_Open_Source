using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.Core;
using System.Text.Json;
//using ZLogger;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
using Windows.Foundation.Diagnostics;
using COTG.Helpers;
using Windows.UI.Xaml;

namespace COTG
{
    /// <summary>
    /// Outpots debug text
    /// </summary>
    public static class Debug
    {
        // {1D7815D0-4DCD-4655-8798-D18196D4DE0F}
        static Guid CotgDebug= new Guid( 0x1d7815d0, 0x4dcd, 0x4655,  0x87, 0x98, 0xd1, 0x81, 0x96, 0xd4, 0xde, 0xf  );
    //    static LoggingChannel channel;
        static Random random = new Random();
     //   public static ILogger logger;
     //   static LoggingSession session;
        static Debug()
		{
          //  var now = new DateTime(2000, 12, 12);
            //   Console.SetOut(Console.Error);

            //session = new LoggingSession("CotgS");
            //channel = new LoggingChannel("cotg", new LoggingChannelOptions(), CotgDebug);
            //session.AddLoggingChannel(channel);
            //channel.LogEvent("Event");
            //var loggerFactory =
            //    LoggerFactory.Create(x =>
            //{
            ////    x.SetMinimumLevel(LogLevel.Debug);
            // //   x.AddZLoggerRollingFile((dt, seq) => $"ms-appdata:///temporary/log{dt.ToUnixTimeMilliseconds()%16384}{seq}.log",      x => now, 5);

            //    x.AddZLoggerConsole(options =>
            //    {
            //        options.EnableStructuredLogging = true;

            //    });
            //    //                c.ena

            //});

            //  logger = loggerFactory.CreateLogger("cotg");


        }
        // public static CoreWindow coreWindow => CoreWindow.GetForCurrentThread();
        [Conditional("DEBUG")]
        public static void Log( string  s,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
		{
            System.Diagnostics.Trace.WriteLine( $"{Tick.MSS()}:{s}\nCaller {memberName}, {sourceFilePath}:{sourceLineNumber}");
        //    System.Diagnostics.Debug.WriteLine(new StackTrace());


        }

        [Conditional("DEBUG")]
        public static void Log<T>(T s,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            System.Diagnostics.Debug.WriteLine($"{Tick.MSS()}:{s.ToString()}\nCaller {memberName}, {sourceFilePath}:{sourceLineNumber}");
          //  System.Diagnostics.Debug.WriteLine(new StackTrace());
        }
        [Conditional("TRACE")]
        public static void Log(Exception e,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)

        {
            System.Diagnostics.Debug.WriteLine($"{Tick.MSS()}:Exception: {e.Message} {e.StackTrace} Caller {memberName}, {sourceFilePath}:{sourceLineNumber}");
            Note.Show(e.Message);

        }
        [Conditional("TRACE")]
        public  static void Exception(string s,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            //
            System.Diagnostics.Debug.WriteLine($"{Tick.MSS()}:Exception: {s} Caller {memberName}, {sourceFilePath}:{sourceLineNumber}");
            //            logger.ZLogError($"{s}\nCaller {memberName}, {sourceFilePath}:{sourceLineNumber}");
            Note.Show(s);
        }

        [Conditional("DEBUG")]
        public static async void Assert(bool v,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
       [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
       [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            if (v)
                return;
            var str = $"{Tick.MSS()}:Assert: Caller {memberName}, {sourceFilePath}:{sourceLineNumber}";
            System.Diagnostics.Debug.WriteLine(str);
            await Task.Delay(0);
            Note.Show(str);
        }

        public static async Task Fatal()
        {
            try
            {
                await new MessageDialog("Something is amiss.", "Please Restart").ShowAsync();
            }
            finally
            {
                Application.Current.Exit();
            }
        }
    }
}
