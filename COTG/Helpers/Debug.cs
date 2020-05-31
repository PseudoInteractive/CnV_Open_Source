using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.UI.Popups;

namespace COTG
{
    /// <summary>
    /// Outpots debug text
    /// </summary>
    public static class Debug
    {
        public static void Log( string  s)
		{
            Trace.WriteLine(s);
		}
        public async static Task Exception(string s)
        {
            Trace.WriteLine($"Exception\n\n:{s}");
            Trace.Flush();
            Trace.Write(new StackTrace(true));
            await new MessageDialog(s,"Not Good").ShowAsync();
        }
    }
}
