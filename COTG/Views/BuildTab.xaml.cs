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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
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
		public static bool IsVisible() => instance.isVisible;

		public event PropertyChangedEventHandler PropertyChanged;

		public BuildTab()
		{
			Assert(instance == null);
			instance = this;
			this.InitializeComponent();
		}
		private void List_GotFocus(object sender, RoutedEventArgs e)
		{
			zoom.StartBringIntoView();
		}
		public ObservableCollection<BuildQueueView> cities { get; set; } = new ObservableCollection<BuildQueueView>(); 
	
		public static void AddOp(BuildQueueItem item, int cid)
		{
			if (!IsVisible())
				return;
			
			App.DispatchOnUIThreadSneaky(() =>
			{
				BuildQueueView view = null;
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
					view = new BuildQueueView() { cid = cid };
					instance.cities.Add(view);
				}
				view.queue.Add(new BuildOpView(item));
			});
	
		}
		public static void Clear(int cid)
		{
			foreach (var c in instance.cities)
			{
				if (c.cid == cid)
				{
					App.DispatchOnUIThreadSneaky(() =>
					{
						instance.cities.Remove(c);
						c.queue.Clear();
					});
					break;
				}
			}

		}
		public static void RemoveOp( BuildQueueItem item, int cid)
		{
			if (!IsVisible())
				return;
			

			App.DispatchOnUIThreadSneaky(() =>
			{
				BuildQueueView view = null;
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
			instance.cities.Clear();
			foreach(var city in CityBuildQueue.all.Values)
			{
				var view = new BuildQueueView() { cid = city.cid };
				instance.cities.Add(view);
				foreach(var q in city.queue)
				{
					view.queue.Add( new BuildOpView(q));
				}

			}
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
				cities.Clear();
			}
			base.VisibilityChanged(visible);

		}

	}


	public class BuildQueueView
	{
		public int cid;
		public string title => City.GetOrAdd(cid).nameAndRemarks;
		public BitmapImage icon => City.GetOrAdd(cid).icon;

		public ObservableCollection<BuildOpView> queue { get; set; } = new ObservableCollection<BuildOpView>();
	}
	public class BuildOpView
	{
		public const int size = 32;
		public ImageBrush brush { get; set; }
		public readonly BuildQueueItem item;
		public string building => BuildingDef.all[item.bid].Bn;
		public string op => item.elvl == 0 ? "Destroy" : item.slvl == 0 ? "Build" : item.slvl > item.elvl ? "Downgrade" : "Upgrade";

		public BuildOpView(BuildQueueItem _item)
		{
			item = _item;
			brush = CityBuild.BuildingBrush(item.bid, (float)size / 128.0f);
		}
	}
}
