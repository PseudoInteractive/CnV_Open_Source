using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public static CoreWindow coreWindow;
        public static void Log( string  s)
		{
            Trace.WriteLine(s);
		}
        public static void Log(Exception e)
        {
            Trace.WriteLine($"Exception\n{e}\n:{e.Message}\n{e.StackTrace}");
        }
        public async static Task Exception(string s)
        {
            Trace.WriteLine($"Exception\n\n:{s}");
            Trace.Flush();
            Trace.Write(new StackTrace(true));
            await coreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await (new MessageDialog(s, "Not Good").ShowAsync()) );
        }
    }
}
