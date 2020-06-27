﻿using System;
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
            var now = new DateTime(2000, 12, 12);
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
        public static void Log( string  s,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
		{
            System.Diagnostics.Trace.WriteLine( $"{Tick.MSS()}:{s}\nCaller {memberName}, {sourceFilePath}:{sourceLineNumber}");
        //    System.Diagnostics.Debug.WriteLine(new StackTrace());


        }

        public static void LogJS(object s,
       [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
       [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
       [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            System.Diagnostics.Debug.WriteLine( $"{Tick.MSS()}:{s}\nCaller {memberName}, {sourceFilePath}:{sourceLineNumber} {s.GetType().Name}:{JsonSerializer.Serialize(s,(new JsonSerializerOptions() {MaxDepth=2}))} ");
            //    System.Diagnostics.Debug.WriteLine(new StackTrace());


        }
        public static void Log(object s,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            System.Diagnostics.Debug.WriteLine($"{Tick.MSS()}:{s}\nCaller {memberName}, {sourceFilePath}:{sourceLineNumber}");
          //  System.Diagnostics.Debug.WriteLine(new StackTrace());
        }
        public static void Log(Exception e)
        {
            System.Diagnostics.Debug.WriteLine($"{Tick.MSS()}:Exception:{e}");
        }
        public  static void Exception(string s,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            //
            System.Diagnostics.Debug.WriteLine($"{Tick.MSS()}:Exception:Caller {memberName}, {sourceFilePath}:{sourceLineNumber}");
//            logger.ZLogError($"{s}\nCaller {memberName}, {sourceFilePath}:{sourceLineNumber}");
        }

        public static void Assert(bool v,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
       [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
       [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            if (v)
                return;
            System.Diagnostics.Debug.WriteLine($"{Tick.MSS()}:Assert: Caller {memberName}, {sourceFilePath}:{sourceLineNumber}");
        }
    }
}
