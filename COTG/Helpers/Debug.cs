using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.Core;

namespace COTG
{
    /// <summary>
    /// Outpots debug text
    /// </summary>
    public static class Debug
    {
       
        public static CoreWindow coreWindow => CoreWindow.GetForCurrentThread();
        public static void Log( string  s,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
		{
           System.Diagnostics.Debug.WriteLine($"{s}\nCaller {memberName}, {sourceFilePath}:{sourceLineNumber}");
        //    System.Diagnostics.Debug.WriteLine(new StackTrace());


        }
        public static void Log(object s,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            System.Diagnostics.Debug.WriteLine(s);
            System.Diagnostics.Debug.WriteLine(s.ToString());
            System.Diagnostics.Debug.WriteLine($"{s}\nCaller {memberName}, {sourceFilePath}:{sourceLineNumber}");
          //  System.Diagnostics.Debug.WriteLine(new StackTrace());
        }
        public static void Log(Exception e)
        {
            System.Diagnostics.Debug.WriteLine($"Exception\n{e}\n:{e.Message}\n{e.StackTrace}");
        }
        public async static Task Exception(string s)
        {
            System.Diagnostics.Debug.WriteLine($"Exception\n\n:{s}");
            System.Diagnostics.Debug.Flush();
            System.Diagnostics.Debug.Write(new StackTrace(true));
            await new MessageDialog(s, "Not Good").ShowAsync() ;
        }
    }
}
