using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Threading.Tasks;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CnV.Views
{
	using Game;

	class HeatmapTemplateSelector : Microsoft.UI.Xaml.Controls.DataTemplateSelector
	{
		public DataTemplate dayTemplate { get; set; }
		public DataTemplate deltaTemplate { get; set; }

		protected override DataTemplate SelectTemplateCore(object item)
		{
			return item is HeatMapDay ? dayTemplate : deltaTemplate;
		}
	}

	public sealed partial class HeatTab : UserTab
	{
		public static HeatTab instance;
		public static bool IsVisible() => instance.isFocused;
		public HeatTab()
		{
			instance = this;
			this.InitializeComponent();
		}

		public static void DaysChanged()
		{

			HeatMapDay.days.NotifyReset();

		}


		private void Now_Click(object sender, RoutedEventArgs e)
		{

			zoom.SelectedNodes?.Clear();
			//SelectionChanged();
		}
		public static async Task ResetAllChangeDescriptions()
		{
			// invalidate
			//lastSelHash = 0;
			using var _ = await HeatMap.mutex.LockAsync();

			foreach (var day in HeatMapDay.days)
			{
				day.UpdateDesc(false, true);
			}

			await AppS.DispatchOnUIThreadTask(() =>
		   {
			   foreach (var day in HeatMapDay.days)
			   {
				   day.NotifyChange();
				   if (day.hasDeltas)
				   {
					   foreach (var d in day.deltas)
						   d.NotifyChange();
				   }
			   }
			   return Task.CompletedTask;
		   });
		}

		static bool listLoaded = false;
		override public async Task VisibilityChanged(bool visible, bool longTerm)
		{
			//tabVisible = visible;

			if (visible)
			{
				if (!listLoaded)
				{
					listLoaded = true;
					await HeatMap.LoadList();
					//	zoom.Focus(FocusState.Programmatic);
				}
				await ResetAllChangeDescriptions();
				DaysChanged();
				//			SelectionChanged();
				lastSelHash = 0;
				if (!callbackRunning)
					CheckSelectionChanged();
			}
			else
			{
				//		World.ClearHeatmap();
			}
			await base.VisibilityChanged(visible, longTerm: longTerm);

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
					await AppS.DispatchOnUIThreadTask(() =>
				   {
					   if (zoom.SelectedNodes != null)
					   {
						   var hash = ComputeHash(zoom.SelectedNodes);
						   if (hash != lastSelHash)
						   {
							   lastSelHash = hash;
							   SelectionChanged();
						   }
					   }
					   return Task.CompletedTask;

				   });
					await Task.Delay(500);
					if (!HeatTab.IsVisible())
						break;
				}
			}
			finally
			{
				callbackRunning = false;
			}
		}

		public async Task _SelectionChanged()
		{

			var t1 = SmallTime.zero;
			var t0 = SmallTime.serverNow;
			await AppS.DispatchOnUIThreadTask(() => {
				var sel = zoom.SelectedNodes;
				if (sel != null && sel.Count > 0)
				{
					
					foreach (var i in sel)
					{
						var t = (i.Content as HeatMapItem).t;
						if (t.seconds == 0)
							continue; // this is as placeholder for pending
						if (t < t0)
							t0 = t;
						if (t > t1)
							t1 = t;
					}
				}
				return Task.CompletedTask;
			});
			if( t0 == t1)
			{
				t0 = SmallTime.serverNow;
			}

			if (t1.seconds != 0)
			{
				World.SetHeatmapDates(t0, t1); // Is Timezone Right?
				return;
			}


			World.ClearHeatmap();
			AppS.DispatchOnUIThreadLow(() =>
		   {
			   if (HeatTab.instance.isFocused)
			   {
				   HeatTab.instance.header.Text = "Please load and select a date range to see changes";

			   }
		   });
		}

		//private void snapshots_PointerEntered(object sender, PointerRoutedEventArgs e)
		//{
		////	listView.Focus(FocusState.Programmatic);
		//}

		//private void List_GotFocus(object sender, RoutedEventArgs e)
		//{
		////	snapshots_SelectionChanged(null, null);
		//}

		static TreeViewNode FindTreeViewNode(HeatMapItem content) => content is HeatMapDay day ?  FindTreeViewNode(day) : FindTreeViewNode((HeatMapDelta)content);

		static bool IsSelected(HeatMapItem content, IList<TreeViewNode> sel) => sel != null && sel.Any((a) => object.ReferenceEquals(a.Content ,content) );

		static TreeViewNode FindTreeViewNode(HeatMapDay content) => FindTreeViewNodeNoRecurse(instance.zoom.RootNodes, content);
		static TreeViewNode FindTreeViewNode( HeatMapDelta  content)
		{
			
		
			foreach (var n in instance.zoom.RootNodes)
			{
					var ch = n.Children;
					if (ch != null)
					{
						var rv = FindTreeViewNodeNoRecurse(ch, content);
						if (rv != null)
							return rv;

					}
				
			}
			return null;
		}
		static TreeViewNode FindTreeViewNodeNoRecurse(IList<TreeViewNode> nodes, HeatMapItem content)
		{

			foreach (var n in nodes)
			{
				if (object.ReferenceEquals(n.Content, content))
					return n;
			}
			return null;
		}

		static int deltaOffset;
		private void zoom_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
		{
			args.Handled = true;
			var item = args.InvokedItem as HeatMapItem;
			ItemInvoked(item);
		}

		private async void ItemInvoked(HeatMapItem item)
		{
			if (item is HeatMapDay day)
			{
				if (day.IsLoadingOrLoaded())
				{

					var sel = zoom.SelectedNodes;
					if(sel!=null)
					{
						var id = sel.FirstOrDefault(a => object.ReferenceEquals(a.Content, day));
						// not selected => add
						if(id == null)
						{
							sel.Add(FindTreeViewNode(day));
						}
						else
						{
							var count = sel.Count;
							sel.Clear();
							// many to only this
							if (count > 1)
							{
								sel.Add(FindTreeViewNode(day));
							}
							else
							{
								// only this to none
							}
						}
					}


				}
				else
				{
					await day.Load();
				}
			}
			else if (item is HeatMapDelta delta)
			{
				if (World.rawPrior0 == null)
				{
				}
				else
				{
					for (int counter = 0; counter < 256; ++counter)
					{
						++deltaOffset;
						if (delta.changes.Length == 0)
							break;
						var id = delta.changes[deltaOffset * 2 % delta.changes.Length];
						if (!Spot.TestContinentFilterPacked(World.PackedIdToPackedContinent(id)))
							continue;

						var change = ChangeInfo.GetChangeDesc(World.rawPrior0[(int)id], World.rawPrior1[(int)id]);
						if (change == null)
							continue;
						Note.Show(change);
						var c = World.PackedIdToCid(id);
						Spot.ProcessCoordClick(c.WorldToCid(), false, AppS.keyModifiers);
						break;
					}
				}
			}
			//			AppS.DispatchOnUIThreadLow(() =>
			//		   {
			////			   instance.zoom.SelectionMode = TreeViewSelectionMode.None;
			////			   instance.zoom.SelectionMode = TreeViewSelectionMode.Multiple;
			//			   var sel = instance.zoom.SelectedNodes;
			//			   var selected = IsSelected(item,sel);
			//			   sel?.Clear();
			//			   if (!selected)
			//			   {
			//				   var node = FindTreeViewNode(item);
			//				   if (node != null)
			//				   {
			//					   if (sel != null)
			//					   {
			//						   if (item is HeatMapDay )
			//						   {
			//							   foreach (var i in item.deltas)
			//							   {
			//								   var n = FindTreeViewNode(node.Children,i);
			//								   if(n!=null)
			//									   sel.Add(n);
			//							   }

			//						   }
			//						   sel.Add(node);
			//						   /// select children?
			//					   }
			//				   }

			//			   }
			//			   else
			//			   {

			//			   }
			//			   item.NotifyChange();
			//		   });
		}



		Debounce _selChanged;
		void SelectionChanged()
		{
			if (_selChanged == null)
				_selChanged = new(_SelectionChanged);
			 _selChanged.Go();
		}

		//private void zoom_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
		//{
		//	SelectionChanged();
		//}


		private void zoom_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			var tv = sender as TreeViewItem;
			//		ItemInvoked(tv.DataContext as HeatMapItem); 
			//Trace($"Key:  {sender} {e}");
			if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space)
				ItemInvoked(tv.DataContext as HeatMapItem);
			//SelectionChanged();

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
