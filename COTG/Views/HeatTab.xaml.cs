using COTG.Game;
using COTG.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace COTG.Views
{
	public sealed partial class HeatTab : UserTab
	{
		public static HeatTab instance;
		public static bool IsVisible() => instance.isVisible;
		public HeatTab()
		{
			instance = this;
			this.InitializeComponent();
		}

		public List<WorldRecord> records { get; set; }  = new();
		public void SetItems(DateTimeOffset[] _items)
		{
			records.Clear();
			foreach (var _i in _items)
			{
				var i = _i.ToServerTime();
				var d = i.Date;
				WorldRecord rec = null;
				foreach (var dayRec in records)
				{
					if(dayRec.t == d)
					{
						rec = dayRec;
					}
				}
				if(rec==null)
				{
					rec = new WorldRecord();
					rec.t = d;
					records.Add(rec);
				}
				rec.children.Add(new WorldRecord() {t=i });

			}
			//days.Source = dayRecords;

//			var result =
//				from t in _items
//				group t by t.Substring(0,10) into g
//				orderby g.Key
//				select g;

			//			viewSource.Source = result;
			//			items=(_items);
			////			items.NotifyReset();
		}

		private void Now_Click(object sender, RoutedEventArgs e)
		{
			tree.SelectedNodes.Clear();
			World.ClearHeatmap();
		}

		override public async void VisibilityChanged(bool visible)
		{

			if (visible)
			{
				tree.Focus(FocusState.Programmatic);
				snapshots_SelectionChanged(null, null);
			}
			else
			{
				World.ClearHeatmap();
			}
			base.VisibilityChanged(visible);

		}

		private void snapshots_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			DateTimeOffset t1 = AUtil.dateTimeZero;
			DateTimeOffset t0 = DateTimeOffset.UtcNow;

			var sel = tree.SelectedNodes;
			

			if (sel.Count > 0)
			{
				foreach(var i in sel)
				{
					var t= (i.Content as WorldRecord).t;
					if (t < t0)
						t0 = t;
					if (t > t1)
						t1 = t;
				}
				Services.WorldStorage.SetHeatmapDates(t0.FromServerTime(), t1.FromServerTime()); // Is Timezone Right?
			}
			else
			{
				World.ClearHeatmap();
			}
		}

		private void snapshots_PointerEntered(object sender, PointerRoutedEventArgs e)
		{
//			tree.Focus(FocusState.Programmatic);
		}

		private void List_GotFocus(object sender, RoutedEventArgs e)
		{
			snapshots_SelectionChanged(null, null);
		}

		private void tree_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
		{
			snapshots_SelectionChanged(null, null);
		}

		private void tree_KeyDown(object sender, KeyRoutedEventArgs e)
		{

		}
	}
	public class WorldRecord
	{
		public DateTimeOffset t;
		public string tStr => t.ToString();
		public List<WorldRecord> children { get; set; } = new();
	}

}
