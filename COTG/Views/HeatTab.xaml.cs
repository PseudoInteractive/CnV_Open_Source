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
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using static COTG.Debug;
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
				var i = _i;
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
			listView.SelectedItems.Clear();
			World.ClearHeatmap();
		}

		override public async void VisibilityChanged(bool visible)
		{

			if (visible)
			{
				zoom.Focus(FocusState.Programmatic);
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

			var sel = listView.SelectedItems;
			

			if (sel.Count > 0)
			{
				foreach(var i in sel)
				{
					var t= (i as WorldRecord).t;
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
		//	listView.Focus(FocusState.Programmatic);
		}

		private void List_GotFocus(object sender, RoutedEventArgs e)
		{
			snapshots_SelectionChanged(null, null);
		}

		private void tree_ItemInvoked(Microsoft.UI.Xaml.Controls.TreeView sender, Microsoft.UI.Xaml.Controls.TreeViewItemInvokedEventArgs args)
		{
			snapshots_SelectionChanged(null, null);
		}

		private void tree_KeyDown(object sender, KeyRoutedEventArgs e)
		{

		}

		private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
		{

		}

		private void listView_ItemClick(object sender, ItemClickEventArgs e)
		{
	//		snapshots_SelectionChanged(null, null);
		}
	}
	//public class DayChanges
	//{
	//	public SmallTime t; // server time, only the date format is non zero
	//	public string dateStr => t.ToString("yyyy-MM-dd");
	//	public Azure.ETag eTag;
		
	//	public uint[] state; // newest state recorded

	//	public List<SnapshotChanges> snapshots { get; set; } = new();

	//	public void Save(BinaryWriter o)
	//	{
	//		o.Write(t.Ticks);
	//		Assert(t.Offset.Ticks == 0);
	//		o.Write(snapshots.Count);
	//		foreach(var s in snapshots)
	//		{
	//			s.Save(o);
	//		}			
	//	}
	//}

	//public class SnapshotChanges
	//{
	//	public SmallTime t; // server time
	//	public string timeStr => t.ToString("HH':'mm':'ss");

	//	public uint[] deltas;  // pairs of uints, first is offset, second is changed xor value

	//	public void Save(BinaryWriter o)
	//	{
	//		o.Write(t.seconds);
			
	//		o.Write(deltas.Length/2);
	//	//	o.Write7BitEncodedInt((byte*)deltas);
			
	//	}

	//}

	public class WorldRecord
	{
		public DateTimeOffset t;
		public string dateStr => t.ToString("yyyy-MM-dd");
		public string timeStr => t.ToString("HH':'mm':'ss");
		public List<WorldRecord> children { get; set; } = new();
	}

}
