using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static COTG.Debug;
/*
 For new continents I got through all those "New Cities" and select "Rename" then "Clear Center Res" its a big time saver
Also I updated Near defense to make it easier to send portal def. It doesn't compute to travel time explicitly but it will tell you have you can or can't make a given time. There is a "Portal" toggle, when its set it
For new castles, raid carry alone doesn't reset raids that have recruited a lot of new troops, so I wanted to combine the two
 */
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
