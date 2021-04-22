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
using Microsoft.Toolkit.Uwp.UI;
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

		public static void DaysChanged()
		{
			App.DispatchOnUIThreadLow( ()=>
				{
					if (instance.groups.View != null)
					{
						var col = instance.groups.View.CollectionGroups;
						instance.zoomedOut.ItemsSource = col;
						HeatMapDay.days.NotifyReset();
					}
			});
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
				HeatMap.LoadList();
				DaysChanged();
				zoom.Focus(FocusState.Programmatic);
				snapshots_SelectionChanged(null, null);
				zoom.IsZoomedInViewActive = true;
				HeatMapDay.days.NotifyReset();
			}
			else
			{
				World.ClearHeatmap();
			}
			base.VisibilityChanged(visible);

		}

		private void snapshots_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			var t1 = SmallTime.zero;
			var t0 = SmallTime.serverNow;

			var sel = listView.SelectedItems;
			

			if (sel.Count > 0)
			{
				foreach(var i in sel)
				{
					var t= (i as Game.HeatMapDelta).t;
					if (t < t0)
						t0 = t;
					if (t > t1)
						t1 = t;
				}
			//	Services.WorldStorage.SetHeatmapDates(t0.FromServerTime(), t1.FromServerTime()); // Is Timezone Right?
			}
			else
			{
			//	World.ClearHeatmap();
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

		private void ZoomedOutTapped(object sender, TappedRoutedEventArgs e)
		{
			var block = sender as TextBlock;
			var lv = zoomedOut;
			var listView = block.FindAscendant(typeof(ListViewItem));
			lv.Items


			Log(e.OriginalSource);
			Log(sender);
		}

		private void ZoomedInGroupHeaderTapped(object sender, TappedRoutedEventArgs e)
		{
			var block = sender as TextBlock;
			var lv = zoomedOut
			var listView = block.FindAscendant(typeof(ListViewItem));


			Log(e.OriginalSource);
			Log(sender);
		}

		private void listView_ItemClick(object sender, ItemClickEventArgs e)
		{
			Log(e.ClickedItem);
			Log(sender);
			//		snapshots_SelectionChanged(null, null);
		}

		private void ZoomedOutItemClick(object sender, ItemClickEventArgs e)
		{
			var i = e.ClickedItem as ICollectionViewGroup;
			var day = i.Group as HeatMapDay;
			if(!day.isInitialized )
			{
				day.Load();
			}

		}

		private void ToggleZoom(object sender, RoutedEventArgs e)
		{
			zoom.IsZoomedInViewActive = !zoom.IsZoomedInViewActive;
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



}
