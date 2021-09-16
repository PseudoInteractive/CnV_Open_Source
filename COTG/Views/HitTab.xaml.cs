using COTG.Game;
using COTG.JSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using Telerik.UI.Xaml.Controls.Grid;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace COTG.Views
{
    public sealed partial class HitTab : UserTab
    {
        public static HitTab instance;
		public override TabPage defaultPage => TabPage.secondaryTabs;

		public static bool IsVisible() => instance.isVisible;
        public Army[] history { get; set; } = Army.empty;
        public void SetHistory(Army[] _history)
        {
			//{
			//	StringBuilder sb = new StringBuilder();
			//	foreach(var i in _history)
			//	{
			//		if (i.claim != 100 || i.sPid != Player.myId)
			//			continue;
			//		var c = City.Get(i.targetCid);
			//		var cl = await c.Classify();
			//		sb.AppendLine($"{c.cid.CidToCoords()}\t{cl}");

			//	}
			//	App.CopyTextToClipboard(sb.ToString());
	
			//}
			history = _history;
			var sel = OutgoingTab.selected;
			var cid = (sel?.cid).GetValueOrDefault();
			historyGrid.ItemsSource = cid==0 ? history : history.Where(a=>a.targetCid==cid).ToArray();
        }
        public HitTab()
        {
            instance = this;
            this.InitializeComponent();
        }
        override public Task VisibilityChanged(bool visible, bool longTerm)
		{
			// historyGrid.ItemsSource = Army.empty;
			if (visible)
			{
				if (OutgoingOverview.updateInProgress == false)
				{
					// avaiting on this would take too long
					OutgoingOverview.OutgoingUpdateDebounce.Go(throttled: true,runAgainIfStarted: false);
				}
			}
            return base.VisibilityChanged(visible, longTerm: longTerm);
	}
    }
}
