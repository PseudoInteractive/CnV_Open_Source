//using ZLogger;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
using COTG.Helpers;
using COTG.Views;

using Microsoft.AppCenter.Crashes;

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

using Windows.UI.Popups;

namespace COTG
{
    /// <summary>
    /// Outpots debug text
    /// </summary>
    public static class Debug
    {
		public static string timeStamp => DateTimeOffset.UtcNow.FormatTimePrecise();
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
        const int defaultStackDepth = 4;
        static void DumpStack(StackTrace  __s)
        {
        
          //  var __f = __s.GetFrames();
            System.Diagnostics.Debug.WriteLine(__s.ToString());
            //for (int i = 0; i<defaultStackDepth && i < __s.FrameCount; ++i)
            //{
            //    var __f = __s.GetFrame(i);
            //    if(__f != null)
            //    System.Diagnostics.Debug.WriteLine($"{__f.GetFileName()}({__f.GetFileLineNumber()}): {__f.GetMethod()},{__f.GetFileColumnNumber()}");

            //}
        }
        // public static CoreWindow coreWindow => CoreWindow.GetForCurrentThread();
        [Conditional("DEBUG")]
        public static void Log( string  s,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
		{
            System.Diagnostics.Debug.WriteLine( $"{sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName}\n{s}");
			DumpStack(new StackTrace(1, true));
			//    System.Diagnostics.Debug.WriteLine(new StackTrace());


		}
		[Conditional("TRACE")]
        public static void Trace(string s,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {

			string msg = $"{sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName}\n{s}";
			Note.Show(s);
			System.Diagnostics.Trace.WriteLine(msg);
			DumpStack(new StackTrace(1, true));
			//    System.Diagnostics.Debug.WriteLine(new StackTrace());


		}

		[Conditional("DEBUG")]
        public static void Log<T>(T s,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {

			var str = $"{sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName}\n{s}";

			System.Diagnostics.Debug.WriteLine(str);
			DumpStack(new StackTrace(1, true));
			//  System.Diagnostics.Debug.WriteLine(new StackTrace());
			//Note.Show(str);

		}

		[Conditional("DEBUG")]
		public static void LogJson<T>(T s,
		[System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
		[System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
		[System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
		{

			System.Diagnostics.Debug.WriteLine($"{sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName}");
			System.Diagnostics.Debug.WriteLine(System.Text.Json.JsonSerializer.Serialize<T>(s));
			DumpStack(new StackTrace(1, true));
			//  System.Diagnostics.Debug.WriteLine(new StackTrace());

		}
	//	[Conditional("TRACE")]
        public static void Log(Exception e,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)

        {
			Crashes.TrackError(e);
#if TRACE
			System.Diagnostics.Trace.WriteLine($"{sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName} : Exception: {e.Message} {e.StackTrace}");
			DumpStack(new StackTrace(e, true));
#endif
			Note.Show(e.Message);
		}
      //  [Conditional("TRACE")]
        public  static void Exception(string s,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
			//
#if TRACE
			System.Diagnostics.Trace.WriteLine($"{sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName} : Exception: {s}");
			DumpStack(new StackTrace(1, true));
			//            logger.ZLogError($"{s}\nCaller {memberName}, {sourceFilePath}:{sourceLineNumber}");
#endif
			Note.Show(s);
        }


        public static void Assert(bool v,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
       [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
       [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            if (v)
                return;
			var str = $"{sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName} : Assert";
			Note.Show(str);
#if TRACE

			System.Diagnostics.Trace.WriteLine(str);
			DumpStack(new StackTrace(1, true));
			System.Diagnostics.Trace.Assert(v);
#endif
		}
		public static void Verify(bool v
#if TRACE
			,
			[System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
	   [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
	   [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
#endif

			)
		{
			if (v)
				return;
#if TRACE
			DumpStack(new StackTrace(1, true));
			var str = $"{sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName} : Assert";
			ChatTab.L(str);
			System.Diagnostics.Trace.WriteLine(str);
			System.Diagnostics.Trace.Assert(v);
#endif
		}
    }
}
