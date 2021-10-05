//using ZLogger;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
using COTG.Views;

using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using COTG.Helpers;
using System.Collections.Immutable;

namespace COTG
{
	/// <summary>
	/// Outpots debug text
	/// </summary>
	public static class Debug
	{
		public static string timeStamp => DateTimeOffset.UtcNow.FormatTimePrecise();

		// {1D7815D0-4DCD-4655-8798-D18196D4DE0F}
	//	private static Guid CotgDebug = new Guid(0x1d7815d0, 0x4dcd, 0x4655, 0x87, 0x98, 0xd1, 0x81, 0x96, 0xd4, 0xde, 0xf);

		// static LoggingChannel channel;
		//private static readonly Random random = new Random();

		// public static ILogger logger; static LoggingSession session;
		static Debug()
		{
			System.Diagnostics.Debug.AutoFlush=false;
			// var now = new DateTime(2000, 12, 12); Console.SetOut(Console.Error);

			//session = new LoggingSession("CotgS");
			//channel = new LoggingChannel("cotg", new LoggingChannelOptions(), CotgDebug);
			//session.AddLoggingChannel(channel);
			//channel.LogEvent("Event");
			//var loggerFactory =
			//    LoggerFactory.Create(x =>
			//{
			////    x.SetMinimumLevel(LogLevel.Debug);
			// // x.AddZLoggerRollingFile((dt, seq) =>
			// $"ms-appdata:///temporary/log{dt.ToUnixTimeMilliseconds()%16384}{seq}.log", x => now, 5);

			// x.AddZLoggerConsole(options => { options.EnableStructuredLogging = true;

			// }); // c.ena

			//});

			// logger = loggerFactory.CreateLogger("cotg");
		}

		private const int defaultStackDepth = 4;
		private static int breakCounter = 8;

		private static void DumpStack(StackTrace __s)
		{
			try
			{
				for (int i = 0; i < defaultStackDepth.Min(__s.FrameCount); ++i)
				{
					var __f = __s.GetFrame(i);
					if (__f != null)
					{
						System.Diagnostics.Debug.Write($"{__f.GetFileName()}({__f.GetFileLineNumber()}): {__f.GetMethod()},{__f.GetFileColumnNumber()}\n");
					}
				}
			}
			catch (Exception)
			{
			}
			//  var __f = __s.GetFrames();
			//	System.Diagnostics.Debug.WriteLine(__s.ToString());
			//for (int i = 0; i<defaultStackDepth && i < __s.FrameCount; ++i)
			//{
			//    var __f = __s.GetFrame(i);
			//    if(__f != null)
			//    System.Diagnostics.Debug.WriteLine($"{__f.GetFileName()}({__f.GetFileLineNumber()}): {__f.GetMethod()},{__f.GetFileColumnNumber()}");

			//}
		}

		// public static CoreWindow coreWindow => CoreWindow.GetForCurrentThread();
		[Conditional("DEBUG")]
		public static void Log(string s,
		[System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
		[System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
		[System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
		{
			System.Diagnostics.Debug.Write($"{sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName}\n{s}\n");
			DumpStack(new StackTrace(1, true));
			// System.Diagnostics.Debug.WriteLine(new StackTrace());
		}
		[Conditional("DEBUG")]
		public static void Trace(string s,
		[System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
		[System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
		[System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
		{
			string msg = $"{App.dispatches0}-{App.dispatches1}! {sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName}\n{s}\n";
			Note.Show(s);
			System.Diagnostics.Debug.Write(msg);
			DumpStack(new StackTrace(1, true));
			// System.Diagnostics.Debug.WriteLine(new StackTrace());
		}

		[Conditional("DEBUG")]
		public static void Trace<T>(T o,
	   [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
	   [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
	   [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
		{
			var s = o.ToString();
			string msg = $"{sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName}\n{s}\n";
			Note.Show(s);
			System.Diagnostics.Debug.Write(msg);
			DumpStack(new StackTrace(1, true));
			// System.Diagnostics.Debug.WriteLine(new StackTrace());
		}

		[Conditional("DEBUG")]
		public static void Log<T>(T s,
		[System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
		[System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
		[System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
		{
			var str = $"{sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName}\n{s}\n";

			System.Diagnostics.Debug.Write(str);
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
			System.Diagnostics.Debug.WriteLine(System.Text.Json.JsonSerializer.Serialize<T>(s, Json.jsonSerializerOptions));
			DumpStack(new StackTrace(1, true));
			// System.Diagnostics.Debug.WriteLine(new StackTrace());
		}

		// [Conditional("DEBUG")]
		public static void LogEx(Exception e, bool report = true, string extra = null,
		string eventName = "HandledException",
		[System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
		[System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
		[System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)

		{
			var dic = new Dictionary<string, string> { { "message", e.Message.Truncate(120) }, { "event", eventName.Truncate(120) } };
			if (extra != null)
			{
				dic[extra] = extra;
			}

			if (report)
			{
				if (App.RegisterException(e.Message))
				{

					try
					{
						AAnalytics.Track(eventName, dic);
						Crashes.TrackError(e, dic);
					}
					catch (Exception ex2)
					{
						App.RegisterException(ex2.Message);
					}
				}
				else
				{
					report = false;
				}
			}

			var msg = $"{eventName} {extra ?? string.Empty} {e.Message}";
#if DEBUG
			System.Diagnostics.Debug.WriteLine($"{sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName} : Exception: {msg} {e.StackTrace}");
			var stackTrace = new StackTrace(e, true);
			DumpStack(stackTrace);
			if(report)
			{
				System.Diagnostics.Debug.Flush();
				BreakDebugger(stackTrace,msg);
			}
#endif


			Note.Show(msg);
		}

		// [Conditional("TRACE")]
		public static void Exception(string s,
		[System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
		[System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
		[System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
		{

#if DEBUG
			System.Diagnostics.Debug.WriteLine($"{sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName} : Exception: {s}");
			DumpStack(new StackTrace(1, true));
			// logger.ZLogError($"{s}\nCaller {memberName}, {sourceFilePath}:{sourceLineNumber}");
#endif
			Note.Show(s);
		}

		public static void Assert(bool v,
			[System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
	   [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
	   [System.Runtime.CompilerServices.CallerArgumentExpression("v")] string exp = "",
	   [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
		{
			if (v)
			{
				return;
			}

			var str = $"{sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName} : Assert({exp})";
			Note.Show(str);
#if DEBUG

			System.Diagnostics.Debug.WriteLine(str);
			var stack = new StackTrace(1, true);
			DumpStack(stack);
			if (App.RegisterException($"{sourceFilePath}({sourceLineNumber})"))
			{
				BreakDebugger(stack,str);
			}
#endif
		}

		[Conditional("DEBUG")]
		private static void BreakDebugger(StackTrace s,string str)
		{
			if (System.Diagnostics.Debugger.IsAttached && breakCounter > 0)
			{
				--breakCounter;
				System.Diagnostics.Debug.Flush();
					System.Diagnostics.Debugger.Break(); ;
			}
		}

//		public static void Verify(bool v
//#if TRACE
//			,
//			[System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
//	   [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
//	   [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
//#endif

//			)
//		{
//			if (v)
//			{
//				return;
//			}
//#if TRACE
//			DumpStack(new StackTrace(1, true));
//			var str = $"{sourceFilePath}({sourceLineNumber}): {timeStamp}: {memberName} : Assert";
//			ChatTab.L(str);
//			BreakDebugger();
//#endif
//		}
	}

	public class UIException : Exception
	{
		public UIException([System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
	   [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
	   [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0) : base($"{sourceFilePath}({sourceLineNumber}): {memberName}) Exception")
		{
		}

		public UIException(string message) : base(message)
		{
		}

		public UIException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
