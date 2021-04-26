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
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI;
using static COTG.Debug;
using System.Threading.Tasks;
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
					HeatMapDay.days.NotifyReset();
			});
		}
	

		private void Now_Click(object sender, RoutedEventArgs e)
		{
			zoom.SelectedNodes.Clear();
		}

		override public async void VisibilityChanged(bool visible)
		{
			tabVisible = visible;
			if (visible)
			{
				HeatMap.LoadList();
				DaysChanged();
				//	zoom.Focus(FocusState.Programmatic);
				if (!callbackRunning)
					CheckSelectionChanged();
				foreach (var day in HeatMapDay.days)
				{
					day.NotifyChange();
					foreach (var d in day.deltas)
						d.NotifyChange();
				}
				HeatMapDay.days.NotifyReset();
			}
			else
			{
		//		World.ClearHeatmap();
			}
			base.VisibilityChanged(visible);
		}
		static bool tabVisible;
		static bool callbackRunning;
		static int lastSelHash;
		static int ComputeHash(IList<TreeViewNode> nodes)
		{
			int rv = 0;
			foreach (var n in nodes)
			{
				var o = n.Content as HeatMapItem;
				rv += o.GetHashCode();
			}
			return rv;
		}
		public async void CheckSelectionChanged()
		{
			Assert(callbackRunning == false);
			callbackRunning = true;
			try
			{
				for (; ; )
				{
					await App.DispatchOnUIThreadSneakyLowAwait(() =>
				   {
					   var hash = ComputeHash(zoom.SelectedNodes);
					   if (hash != lastSelHash)
					   {
						   lastSelHash = hash;
						   SelectionChanged();
					   }

				   });
					await Task.Delay(500);
					if (!tabVisible)
						break;
				}
			}
			finally
			{
				callbackRunning = false;
			}
		}

		public void SelectionChanged()
		{
			Trace("Sel Changed");
			{
				var t1 = SmallTime.zero;
				var t0 = SmallTime.serverNow;
				var sel = zoom.SelectedNodes;
				if (sel.Count > 0)
				{
					foreach (var i in sel)
					{
						var t = (i.Content as HeatMapItem ).t;
						if (t < t0)
							t0 = t;
						if (t > t1)
							t1 = t;
					}
					World.SetHeatmapDates(t0, t1); // Is Timezone Right?
				}
				else
				{
					World.ClearHeatmap();
				}
			}
		}

		private void snapshots_PointerEntered(object sender, PointerRoutedEventArgs e)
		{
		//	listView.Focus(FocusState.Programmatic);
		}

		private void List_GotFocus(object sender, RoutedEventArgs e)
		{
		//	snapshots_SelectionChanged(null, null);
		}

		static TreeViewNode FindTreeViewNode(HeatMapItem content) => FindTreeViewNode(instance.zoom.RootNodes, content);

		static bool IsSelected(HeatMapItem content) => instance.zoom.SelectedNodes.Any((a) => a.Content == content);

		static TreeViewNode FindTreeViewNode(IList<TreeViewNode> nodes, HeatMapItem content)
		{
			foreach(var n in nodes)
			{
				if (n.Content == content)
					return n;
				var ch = n.Children;
				if(ch!=null)
				{
					var rv = FindTreeViewNode(ch,content);
					if (rv != null)
						return rv;

				}
			}
			return null;
		}
		static int deltaOffset;
		private async void zoom_ItemInvoked(Windows.UI.Xaml.Controls.TreeView sender, Windows.UI.Xaml.Controls.TreeViewItemInvokedEventArgs args)
		{
			args.Handled = true;
			var item = args.InvokedItem as HeatMapItem;
			ItemInvoked(item);
		}

		private async void ItemInvoked(HeatMapItem item)
		{
			if (item is HeatMapDay day)
			{
				if (!day.isInitialized)
				{
					await day.Load();

				}
			}
			else if (item is HeatMapDelta delta)
			{
				if(!IsSelected(delta))
				{
					var node = FindTreeViewNode(delta);
					if (node != null)
					{
						instance.zoom.SelectedNodes.Clear();
						instance.zoom.SelectedNodes.Add(node);
					}

				}

				if(World.rawPrior0 == null)
				{
				}
				else
				{
					for (int counter = 0; counter < 64; ++counter)
					{
						++deltaOffset;
						var id = delta.deltas.Span[deltaOffset * 2 % delta.deltas.Length];
						var change = ChangeInfo.GetChangeDesc(World.rawPrior0.Span[(int)id], World.rawPrior1.Span[(int)id]);
						if (change == null)
							continue;
						Note.Show(change);
						var c = World.PackedIdToCid(id);
						Spot.ProcessCoordClick(c.WorldToCid(), false, App.keyModifiers);
						break;
					}
				}
			}


		}


		private void tree_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			var tv = sender as TreeViewItem;
			ItemInvoked(tv.DataContext as HeatMapItem); 
	//		Trace($"Key:  {sender} {e}");
		}


		

		
		

		private void zoom_Expanding(Windows.UI.Xaml.Controls.TreeView sender, Windows.UI.Xaml.Controls.TreeViewExpandingEventArgs args)
		{
			var i = args.Item as HeatMapItem;
			var day = i as HeatMapDay;
			if(day!=null)
			{
				if (!day.isInitialized)
					day.Load();
			}

		}

		//private void TreeViewItem_Tapped(object sender, TappedRoutedEventArgs e)
		//{
		//	Trace(sender.ToString());
		//}
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
