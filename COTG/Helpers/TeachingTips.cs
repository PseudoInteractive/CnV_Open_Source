using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static COTG.Debug;

namespace COTG.Helpers
{
    public class TipsSeen
    {
        public static TipsSeen instance;
        public bool refresh { get; set; }
        public bool raiding1 { get; set; }
        public bool raiding2 { get; set; }
        public bool raiding3 { get; set; }
        public bool chat0 { get; set; }
        public bool chat1 { get; set; }
        public bool chat2 { get; set; }
    }
    public struct TipInfo
    {
        public bool queued;
        public void Dispatch(Microsoft.UI.Xaml.Controls.TeachingTip tip, Action done)
        {
            if (queued)
                return; // already pending
            queued = true;
            App.QueueIdleTask(() =>
            {
                done();
                App.DispatchOnUIThreadLow(() => tip.IsOpen = true);
            });
        }
    }
}
