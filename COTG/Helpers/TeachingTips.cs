using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

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
		static string activeTip;

        public static bool Show(this Microsoft.UI.Xaml.Controls.TeachingTip tip,string name,int delay=0)
        {
			//if (activeTip != null)
			//	return false;

			//if (seen.Contains(name))
			//	return false;
			//activeTip = name;
			//tip.Closed -= Tip_Closed;
			//tip.Closed += Tip_Closed;
			//if (delay == 0)
			//{
			//	seen.Add(name);
			//	tip.IsOpen = true;
			//	return true;
			//}

			//         if (queued!=null)
			//             return true;

			//         queued =tip;
			//         App.QueueIdleTask(ShowTip, delay);
			//         return true;
			return true;
        }

		private static void Tip_Closed(Microsoft.UI.Xaml.Controls.TeachingTip sender, Microsoft.UI.Xaml.Controls.TeachingTipClosedEventArgs args)
		{
			Assert(activeTip != null);
			seen.Add(activeTip);
			activeTip = null;
		}

		private static void ShowTip()
        {
            var _queued = queued;
            queued = null;
            
            App.DispatchOnUIThreadLow(() => _queued.IsOpen = true);
        }

		static StorageFolder folder => ApplicationData.Current.LocalFolder;
		public static async void SaveSeen()
		{
			await folder.SaveAsync("tipsSeen", seen.ToArray());
		}
		public static async void ReadSeen()
		{
			seen = new HashSet<string>( await folder.ReadAsync("tipsSeen", Array.Empty<string>()));
		}
	}

}
