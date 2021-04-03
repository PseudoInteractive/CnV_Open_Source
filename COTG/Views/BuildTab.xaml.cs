using COTG.Game;
using COTG.Helpers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static COTG.Game.Enum;
using static COTG.Debug;
using System.Text.RegularExpressions;
using System.Numerics;
using COTG.JSON;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.HighPerformance.Enumerables;
using Windows.UI.Xaml.Media.Imaging;

namespace COTG.Views
{

	
	public sealed partial class BuildTab : UserTab, INotifyPropertyChanged
	{

		public static BuildTab instance;

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public static bool IsVisible() => instance.isVisible;

		
		public BuildTab()
		{
			Assert(instance == null);
			instance = this;
			this.InitializeComponent();
		}
		//private void ZoomedInGotFocus(object sender, RoutedEventArgs e)
		//{
		//	Log("in focus");
		//	zoom.StartBringIntoView();
		//	var groups = cvsGroups;
		//	var cur = groups.View.CurrentItem;
		//	Log(cur?.ToString());
		//}
		//private void ZoomedOutGotFocus(object sender, RoutedEventArgs e)
		//{
		//	Log("out focus");
		//	zoom.StartBringIntoView();
		//	var groups = cvsGroups;
		//	var cur = groups.View.CurrentItem;
		//	Log(cur?.ToString());
		//}
		public ObservableCollection<BuildItemView> cities { get; set; } = new ObservableCollection<BuildItemView>();

		

		public static void AddOp(BuildQueueItem item, int cid)
		{
			if (!IsVisible())
				return;			

			App.DispatchOnUIThreadSneakyLow(() =>
			{
				BuildItemView view = null;
				foreach (var c in instance.cities)
				{
					if (c.cid == cid)
					{
						view = c;
						break;
					}
				}
				if (view == null)
				{
					view = BuildItemView.Rent().Ctor(cid);
					instance.cities.Add(view);
				}	
				view.queue.Add(BuildItemView.Rent().Ctor(item,cid));
			});
		}

		public static void Clear(int cid)
		{
			App.DispatchOnUIThreadSneakyLow(() =>
			{
			foreach (var c in instance.cities)
			{
				if (c.cid == cid)
				{
					
						instance.cities.Remove(c);
						c.queue.Clear();
					break;
				}
			}
			});


		}


		public static void RemoveOp( BuildQueueItem item, int cid)
		{
			if (!IsVisible())
				return;
			

			App.DispatchOnUIThreadSneakyLow(() =>
			{
				BuildItemView view = null;
				foreach (var c in instance.cities)
				{
					if (c.cid == cid)
					{
						view = c;
						break;
					}
				}
				if (view == null)
				{
					return; // missing
				}

				int id = 0;
				foreach (var q in view.queue)
				{
					if ((q.item.bspot == item.bspot) && // is this enough for a match?
							(q.item.slvl == item.slvl))
					{
					    
						view.queue.RemoveAt(id);
						break;
					}
					++id;
				}
				if (!view.queue.Any())
				{
					instance.cities.Remove(view);
				}
			});

		}
		public static void RebuildAll()
		{
			App.DispatchOnUIThreadSneaky(() =>
			{
				instance.cities.Clear();
				foreach (var city in CityBuildQueue.all.Values)
				{
					var view =  BuildItemView.Rent().Ctor(city.cid);
					instance.cities.Add(view);
					foreach (var q in city.queue)
					{
						view.queue.Add(BuildItemView.Rent().Ctor(q, city.cid));
					}

				}
			});
		}
		override public async void VisibilityChanged(bool visible)
		{
			//   Log("Vis change" + visible);

			if (visible)
			{
				RebuildAll();
			}
			else
			{
				App.DispatchOnUIThreadSneaky( cities.Clear );
			}
			base.VisibilityChanged(visible);

		}

		//private void ItemClick(object sender, ItemClickEventArgs e)
		//{
		//	var i = (e.ClickedItem as BuildItemView);
		//	BuildQueue.CancelBuildOp(i.cid, i.item);


		//}

		private void ClearQueue(object sender, RoutedEventArgs e)
		{
			CityBuild.ClearQueue();

		}




		//private void zoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
		//{
		//	var item = e.DestinationItem.Item as BuildItemView;
		//	if(item!=null)
		//	{
		//		JSClient.ChangeCity(item.cid, false);
		//		return;
		//	}
		//	var op = e.DestinationItem.Item as BuildItemView;
		//	if(op!=null)
		//	{
		//		JSClient.ChangeCity(op.cid, false);
		//		return;

		//	}
	
		//	item = e.SourceItem.Item as BuildItemView;
		//	if (item != null)
		//	{
		//		JSClient.ChangeCity(item.cid, false);
		//		return;
		//	}
		//	op = e.SourceItem.Item as BuildItemView;
		//	if (op != null)
		//	{
		//		JSClient.ChangeCity(op.cid, false);
		//		return;

		//	}

		//}

		private void ClearSelected(object sender, RoutedEventArgs e)
		{
			var sel = zoom.SelectedNodes;
			var removedCitites = new List<BuildItemView>();
			var removedOps = new List<BuildItemView>();

			/// collect all cities
			foreach (var i in sel)
			{
				if (i.Content is BuildItemView city && city.isCity)
				{
					removedCitites.Add(city);
				}
			}

			// collect op not part of removed cities
			foreach (var i in sel)
			{
  			 if ( i.Content is BuildItemView op && op.isOp)
				{
					if(!removedCitites.Any( city => city.cid == op.cid))
					{
						removedOps.Add(op);
					}
				}
			}

			Note.Show($"Removed {removedOps.Count} build ops and {removedCitites.Count} city queues");
			// now remove
			foreach (var city in removedCitites)
			{
				BuildQueue.ClearQueue(city.cid);
			}
			foreach (var op in removedOps)
			{
				BuildQueue.CancelBuildOp(op.cid, op.item); ;
			}
		}

		private void zoom_ItemInvoked(Windows.UI.Xaml.Controls.TreeView sender, Windows.UI.Xaml.Controls.TreeViewItemInvokedEventArgs args)
		{
			var ob = args.InvokedItem;
			if (ob is BuildItemView q)
			{
				JSClient.ChangeCity(q.cid,false); // this is always true now
			}
			else if (ob is BuildItemView op)
			{

			}
		}

	

	
	}

	public class BuildItemTemplateSelector : Windows.UI.Xaml.Controls.DataTemplateSelector

	{
		public DataTemplate cityTemplate { get; set; }
		public DataTemplate opTemplate { get; set; }

		protected override DataTemplate SelectTemplateCore(object item)
		{
			if (item is BuildItemView city)
				return cityTemplate;
			else
				return opTemplate;
		}
	}
	
	public class BuildItemView
	{
		static List<BuildItemView> pool = new();
		public const int size = 32;
		public ImageBrush brush { get; set; }
		public int cid; // owner
		public BuildQueueItem item;
		public string building { get; set; }
		public string text { get; set; }
		public ObservableCollection<BuildItemView> queue { get; set; } = new ObservableCollection<BuildItemView>();
		public bool isCity => item.isNop;
		public bool isOp => !isCity;

		public BuildItemView Ctor(BuildQueueItem _item, int _cid)
		{
			cid = _cid;
			item = _item;
			brush = CityBuild.BuildingBrush(item.bid, (float)size / 128.0f);
			var op = item.elvl == 0 ? "Destroy" : item.slvl == 0 ? "Build" : item.slvl > item.elvl ? "Downgrade" : "Upgrade";
			text = op + BuildingDef.all[item.bid].Bn;
			return this;
		}
		public BuildItemView Ctor(int _cid)
		{
			var city = City.GetOrAdd(_cid);
			cid = _cid;
			brush = CityBuild.BrushFromImage(city.icon);
			text = City.GetOrAdd(_cid).nameAndRemarks;
			item = BuildQueueItem.nop;
			return this;
		}
		public void Return()
		{
			text = null;
			brush = null;
			cid = 0;
			queue.Clear();
			pool.Add(this);
		}
		public static BuildItemView Rent()
		{
			if(pool.Any())
			{
				int i = pool.Count - 1;
				var rv = pool[i];
				pool.RemoveAt(i);
					return rv;
			}
			return new BuildItemView();

		}
	}
}
