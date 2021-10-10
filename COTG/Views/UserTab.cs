using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

using Telerik.UI.Xaml.Controls.Grid;
//using Windows.UI.ViewManagement;
//using Windows.UI.WindowManagement;
using Microsoft.UI.Xaml;
//using Microsoft.UI.Windowing;

namespace COTG.Views
{
	public class UserTab:UserControl
	{
		public virtual TabPage defaultPage => TabPage.mainTabs;
		public static List<RadDataGrid> spotGrids = new();


		public static UserTab[] userTabs;
		public static void InitUserTabs()
		{

			userTabs = new UserTab[] {
				new MainPage(),
				new IncomingTab(),
				new DonationTab(),
				new DefenseHistoryTab(),
				new BossTab(),
				new HeatTab(),
				new AttackTab(),
				new PlannerTab(),
				new QueueTab(),
				new BuildTab(),
				new SpotTab(),
				new OutgoingTab(),
				new HitTab(),
				new NearDefenseTab(),
				new NearRes(),
				new PlayerChangeTab(),
		};

		}

		protected void SpotSelectionChanged(object sender,DataGridSelectionChangedEventArgs e)
		{
			var grid = sender as RadDataGrid;
			Assert(grid != null);
			if(!isOpen)
				return;

			if(SpotTab.silenceSelectionChanges == 0)
			{
				try
				{

					var sel = grid.SelectedItems;
					Spot.selected =new HashSet<int>( sel.Select(a=> ((Spot)a).cid ));

				}
				catch(Exception ex)
				{
					LogEx(ex);
				}
				finally
				{
					//          Spot.selected.ExitWriteLock();
				}
			}

		}
		public virtual Task VisibilityChanged(bool visible,bool longTerm)
		{
			if(visible)
			{
		//		Assert(isFocused);
			}
//			Log($"VisibilityChanged: {visible} {this}");
			return Task.CompletedTask;

		}
		//public virtual Task Reset()
		//{
		//	if(visible)
		//	{
		//		await VisibilityChanged(false);  // close enough default behaviour
		//		await VisibilityChanged(true);  // close enough default behaviour
		//	}
		//}
		// If this is not an xaml island, the newXamlRoot will be null
	//	public virtual void XamlTreeChanged(TabPage newPage) { } // The tab was dragged somewhere else
		public bool isFocused; // if set, the tab is open/focused/visible.  There is one focused tab per page
		public bool isOpen; // if this is set, the tab exists on 1 tab page
		public bool hasAnnouncedActive; // true after active has been called at least once
										//User pressed F5 or refresh button

		//		static DateTimeOffset nextCityRefresh = DateTimeOffset.UtcNow;
		public Debounce refresh;
		public UserTab()
		{
			if(refresh == null)
				refresh = new(_Refresh) { debounceDelay = 500};// { throttled = true };
										//	ScrollViewer.SetIsVerticalScrollChainingEnabled(this,false);
//			this.Width = 500;
//			this.Height = 500;
			//	ScrollViewer.SetVerticalScrollMode(this, ScrollMode.Auto); //DependencyObjectExtensions.FindDescendant<ScrollViewer>(this).AllowFocusOnInteraction= false;
			Margin= new(8);
		}
		public Task VisibilityMaybeChanged()
		{
			return VisibilityChanged(isFocused,longTerm: false);

		}
		protected virtual async Task _Refresh()
		{
			if(isFocused && isOpen)
			{
				isFocused=false;
				await VisibilityChanged(false,longTerm: false);  // close enough default behaviour

				isFocused=true;
				await VisibilityChanged(true,longTerm: false);  // close enough default behaviour
			}
		}
		public static  void SetupDataGrid(RadDataGrid grid)
		{
			grid.GridLinesVisibility= Telerik.UI.Xaml.Controls.Primitives.GridLinesVisibility.Both;
		//	grid.ProcessTooltips();
		//	grid.ListenForNestedPropertyChange=false;
			grid.FontSize = SettingsPage.smallFontSize;
			grid.RowHeight = SettingsPage.mediumGridRowHeight;
			
		}

		public void SetupCityDataGrid(RadDataGrid grid)
		{
			// damn, there should be a better way to check for this
			if(spotGrids.Contains(grid))
				return;
			spotGrids.Add(grid);
			grid.SelectionChanged += SpotSelectionChanged;
			grid.ProcessTooltips();
			SetupDataGrid(grid);
		}
		protected void DataGridLoaded(object sender,RoutedEventArgs e)
		{
			SetupCityDataGrid(sender as RadDataGrid);
		}

		public void SetPlus(bool set)
		{
			App.QueueOnUIThread(() =>
			{
				(var tp, var tvi) = GetViewItem();
				if(tvi!=null)
				{
					var h = tvi.Header as string;
					var hasNumber = ((h[0] >= '0') && (h[0] <= '9'));
					if(!set)
					{
						if(hasNumber)
						{
							tvi.Header = h.Substring(2);
						}
					}
					else
					{
						if(hasNumber)
						{
							tvi.Header=(int.Parse(h.Substring(0,1))+1).Min(9) + h.Substring(1);
						}
						else
						{
							tvi.Header = "1 " + h;
						}
					}
				}
			});
		}

		public (TabPage tabPage, TabViewItem tabViewItem) GetViewItem()
		{
			foreach(var tabPage in TabPage.tabPages)
			{
				foreach(TabViewItem ti in tabPage.Tabs.TabItems)
				{
					if(ti.Content == this)
						return (tabPage, ti);
				}
			}
			return (null, null);
		}
		public IEnumerable<TabPage> GetTabPages()
		{
			foreach(var tabPage in TabPage.tabPages)
			{
				foreach(TabViewItem ti in tabPage.Tabs.TabItems)
				{
					if(ti.Content == this)
					{
						yield return tabPage;
						break;
					}
				}
			}
		}

		public void Show()
		{
			if(!isOpen)
			{
				ShowOrAdd(true);
			}
			else
			{
				if(!isFocused)
					TabPage.Show(this);
				//    else
				//      tab.Refresh();
			}

		}
		public virtual void Close()
		{
			if(!isOpen)
				return;
			TabPage.Close(this);
		}
		public void ShowOrAdd(bool selectMe = true,bool onlyIfClosed = false)

		{
			if(onlyIfClosed && isOpen)
				return;

			// already here?
			var existing = GetViewItem();
			if(existing.tabPage != null)
			{
				if(selectMe)
					existing.tabPage.Tabs.SelectedItem = existing.tabViewItem;
				return;
			}

			defaultPage.Add(this,selectMe);
		}

		
	}
}
