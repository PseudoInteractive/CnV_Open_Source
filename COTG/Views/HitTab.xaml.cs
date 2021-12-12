using System.Threading.Tasks;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CnV.Views
{
	using Game;

	public sealed partial class HitTab : UserTab
    {
        public static HitTab instance;
		public override TabPage defaultPage => TabPage.secondaryTabs;

		public static bool IsVisible() => instance.isFocused;
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
					OutgoingOverview.OutgoingUpdateDebounce.Go();
				}
			}
            return base.VisibilityChanged(visible, longTerm: longTerm);
	}
    }
}
