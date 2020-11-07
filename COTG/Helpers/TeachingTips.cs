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
        public static HashSet<string> seen;
        public static bool queued; // tip is queued to be executed on idle

        public static bool Show(this Microsoft.UI.Xaml.Controls.TeachingTip tip)
        {
            if (queued)
                return true;
            if (seen.Contains(tip.Name))
                return false;

            queued = true;
            App.QueueIdleTask(() =>
            {
                queued = false;
                seen.Add(tip.Name);
                App.DispatchOnUIThreadLow(() => tip.IsOpen = true);
            });
            return true;
        }
    }
   
}
