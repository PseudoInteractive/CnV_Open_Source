using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static COTG.Debug;

namespace COTG.Helpers
{
    public static class Tips
    {
        public static HashSet<string> seen = new HashSet<string>();
        public static Microsoft.UI.Xaml.Controls.TeachingTip queued; // tip is queued to be executed on idle

        public static bool Show(this Microsoft.UI.Xaml.Controls.TeachingTip tip,int delay=1000)
        {
            if (queued!=null)
                return true;
            if (seen.Contains(tip.Name))
                return false;

            queued =tip;
            App.QueueIdleTask(ShowTip, delay);
            return true;
        }

        private static void ShowTip()
        {
            var _queued = queued;
            queued = null;
            seen.Add(_queued.Name);
            App.DispatchOnUIThreadLow(() => _queued.IsOpen = true);
        }
    }
   
}
