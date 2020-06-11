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
       
        public static CoreWindow coreWindow => CoreWindow.GetForCurrentThread();
        public static void Log( string  s)
		{
            System.Diagnostics.Debug.WriteLine(s);
		}
        public static void Log(object s)
        {
            System.Diagnostics.Debug.WriteLine(s);
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
