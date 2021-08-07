using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using COTG.Game;

using Microsoft.Toolkit.Uwp.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

using Telerik.UI.Xaml.Controls.Grid;

using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI;
using static COTG.Debug;
using Windows.UI.Xaml.Media;
using SymbolIconSource = Microsoft.UI.Xaml.Controls.SymbolIconSource;
using Microsoft.Xna.Framework.Input;

namespace COTG.Views
{
    public class UserTab : UserControl
    {
		public virtual TabPage defaultPage => TabPage.mainTabs;
		public static List<RadDataGrid> spotGrids= new();

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


		protected void SpotSelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
		{
			var grid = sender as RadDataGrid;
			Assert(grid != null);
			if (!isActive)
				return;

			if (SpotTab.silenceSelectionChanges == 0)
			{
				try
				{

					var sel = grid.SelectedItems;
					var newSel = new HashSet<int>();
					foreach (Spot s in sel)
					{
						newSel.Add(s.cid);

					}


					//          Spot.selected.EnterWriteLock();

					Spot.selected = newSel;
				}
				catch (Exception ex)
				{
					LogEx(ex);
				}
				finally
				{
					//          Spot.selected.ExitWriteLock();
				}
			}

		}
		public virtual Task VisibilityChanged(bool visible)
        {
            Log($"VisibilityChanged: {visible} {this}");
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
		public virtual void XamlTreeChanged(TabPage newPage) { } // The tab was dragged somewhere else
        public bool isVisible;
        public bool isActive;
		//User pressed F5 or refresh button

		static DateTimeOffset nextCityRefresh = DateTimeOffset.UtcNow;
		public Debounce refresh;
		public UserTab()
		{
			if (refresh == null)
				refresh = new(_Refresh);// { throttled = true };
			ScrollViewer.SetZoomMode(this, ZoomMode.Enabled);
			//DependencyObjectExtensions.FindDescendant<ScrollViewer>(this).AllowFocusOnInteraction= false;
		}
		
		protected virtual async Task _Refresh()
        {
            if (isVisible && isActive)
            {
				await VisibilityChanged(false);  // close enough default behaviour

				await VisibilityChanged(true);  // close enough default behaviour
            }
        }

		public void SetPlus(bool set)
        {
            (var tp, var tvi) = GetViewItem();
            if(tvi!=null)
            {
                var h = tvi.Header as string;
				var hasNumber = ((h[0] >= '0') && (h[0] <= '9'));
				if (!set)
				{
					if ( hasNumber )
					{
						tvi.Header = h.Substring(2);
					}
				}
				else
				{
					if (hasNumber)
					{
						tvi.Header=(int.Parse(h.Substring(0,1))+1).Min(9) + h.Substring(1);
					}
					else
					{
						tvi.Header = "1 " + h;
					}
                }
            }
        }

        public (TabPage tabPage, TabViewItem tabViewItem) GetViewItem()
        {
            foreach (var tabPage in TabPage.tabPages)
            {
                foreach (TabViewItem ti in tabPage.Tabs.TabItems)
                {
                    if (ti.Content == this)
                        return (tabPage, ti);
                }
            }
            return (null, null);
        }
		public  void Show()
		{
			if (!isActive)
			{
				ShowOrAdd( true);
			}
			else
			{
				if (!isVisible)
					TabPage.Show(this);
				//    else
				//      tab.Refresh();
			}

		}
		public virtual void Close()
		{
			if (!isActive)
				return;
			TabPage.Close(this);
		}
		public void ShowOrAdd(bool selectMe=true, bool onlyIfClosed=false)

		{
			if (onlyIfClosed && isActive)
				return;

			// already here?
			var existing = GetViewItem();
			if (existing.tabPage != null)
			{
				if (selectMe)
					existing.tabPage.Tabs.SelectedItem = existing.tabViewItem;
				return;
			}

			var vi = new TabViewItem()
			{
				Header = Tag as string,
				IconSource = TabPage.GetIconForTab(this),
				Content = this
			};
			var page = defaultPage;
			page.Add(vi);
			if (selectMe)
				page.Tabs.SelectedItem = vi;
		}

	}
    public sealed partial class TabPage : Page
    {
        public static List<AppWindow> tabWindows = new List<AppWindow>();
        public static TabPage mainTabs;
		public static TabPage secondaryTabs;
		AppWindow RootAppWindow = null;

        private const string DataIdentifier = "ChatTabItem";
        public TabPage()
        {

			this.InitializeComponent();
			IsTabStop = true;
			TabFocusNavigation = KeyboardNavigationMode.Cycle;
			AllowFocusOnInteraction = true; tabPages.Add(this);
			
        }
        static public List<TabPage> tabPages = new List<TabPage>();

       
        public static bool Show(UserTab tab)
        {
            bool rv = false;
            foreach (var tabPage in tabPages)
            {
                foreach (TabViewItem ti in tabPage.Tabs.TabItems)
                {
                    if (ti.Content == tab)
                    {
                        if(tabPage.Tabs.SelectedItem!=ti)
                            tabPage.Tabs.SelectedItem=(ti);
                        rv=true;
                    }
                }
            }
            return rv;
        }
		public static bool Close(UserTab tab)
		{
			bool rv = false;
			if (App.isShuttingDown)
				return false;

			foreach (var tabPage in tabPages)
			{
				foreach (TabViewItem ti in tabPage.Tabs.TabItems)
				{
					if (ti.Content == tab)
					{
						tabPage.RemoveTab(ti);
						return true;

					}
				}
			}
			return rv;
		}
		public static (TabPage page, bool found) Get(UserTab tab)
		{
			bool rv = false;
			foreach (var tabPage in tabPages)
			{
				foreach (TabViewItem ti in tabPage.Tabs.TabItems)
				{
					if (ti.Content == tab)
					{
						return (tabPage,true);
					}
				}
			}
			return (mainTabs,false);
		}
		private void Tabs_TabItemsChanged(TabView sender, Windows.Foundation.Collections.IVectorChangedEventArgs args)
        {
            // If there are no more tabs, close the window.
            if (sender.TabItems.Count == 0)
            {
                if (RootAppWindow != null)
                {
					Tabs.TabItemsChanged -= Tabs_TabItemsChanged;
					RootAppWindow.CloseAsync();
					
                }
            //    else
            //    {
            //        Window.Current.Close();
            //    }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SetupWindow(null);
        }
        void RemoveTabsOnClose()
        {
            Tabs.TabItemsChanged -= Tabs_TabItemsChanged;
			if (App.isShuttingDown)
				return;

			var _tab = Tabs;
            while (_tab.TabItems.Count > 0)
            {
                RemoveTab(_tab.TabItems[0] as TabViewItem);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (RootAppWindow == null)
            {

    //            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
  //              coreTitleBar.LayoutMetricsChanged -= CoreTitleBar_LayoutMetricsChanged;

            }
            else
            {
                // Secondary
                RootAppWindow.Frame.DragRegionVisuals.Remove(CustomDragRegion);
            }
            RemoveTabsOnClose();
           Assert( Tabs.TabItems.Count==0);
        }

        bool AddAnyChatTab(bool selectIt)
        {
            foreach (var tab in ChatTab.all)
            {
                if (tab.isActive)
                    continue;

				tab.ShowOrAdd( selectIt);
                return true;
            }
            return false; 
        }

      

        static Dictionary<string, Symbol> tabSymbolIcons = new Dictionary<string, Symbol> {
            { "Raid", Symbol.ReShare },
            { "Donation", Symbol.Share },
            { "Boss", Symbol.View },
            { "AttackPlanner", Symbol.Audio },
            { "world", Symbol.Microphone },
			{ "Build", Symbol.Repair },
			{ "Planner", Symbol.Map },
			{ "NearRes", Symbol.Favorite },
			{ "officer" ,Symbol.Admin },
		};

        static Dictionary<string, string> tabFontIcons = new Dictionary<string, string> {
            { "Incoming" , "\uF0EF"  },//tab.Tag as string,
            {    "DefenseHistory", "\uEA0D" },
            {    "Recent" ,  "\uF738" },
            {  "NearDefense", "\uEA18" },            
            { "alliance", "\uE902" },
            { "Outgoing","\uE189" },
            { "Hits","\uEA69" },
			{ "Heat", "\uF738" },
			{ "PlayerChange", "\uE822" }
		};
        public static Microsoft.UI.Xaml.Controls.IconSource GetIconForTab(UserTab tab)
        {
            if (tabSymbolIcons.TryGetValue(tab.Tag as string, out var symbol))
                return new SymbolIconSource() { Symbol = symbol };
            if (tabFontIcons.TryGetValue(tab.Tag as string, out var glyph))
                return new Microsoft.UI.Xaml.Controls.FontIconSource() { Glyph = glyph };
            return new SymbolIconSource() { Symbol = Symbol.Comment };
        }
        private static IconElement GetOldIconForTab(UserTab tab)
        {
            if (tabSymbolIcons.TryGetValue(tab.Tag as string, out var symbol))
                return new SymbolIcon() { Symbol = symbol };
            if (tabFontIcons.TryGetValue(tab.Tag as string, out var glyph))
                return new FontIcon() { Glyph = glyph };
            return new SymbolIcon() { Symbol=Symbol.Comment }; // whisper
        }

        void SetupWindow(AppWindow window)
        {
			Tabs.TabItemsChanged -= Tabs_TabItemsChanged;

			if (window == null)
            {
                // Main Window -- add some default items
              

                Tabs.SelectedIndex = 0;

                // Extend into the titlebar
               // var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                //coreTitleBar.ExtendViewIntoTitleBar = true;

//                coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

                //var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                //titleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
                //titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;

                //Window.Current.SetTitleBar(CustomDragRegion);
            }
            else
            {
                // Secondary AppWindows --- keep track of the window
                RootAppWindow = window;
                window.Closed += Window_Closed;
                // Extend into the titlebar
                window.TitleBar.ExtendsContentIntoTitleBar = true;
                window.TitleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
                window.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;

                // Due to a bug in AppWindow, we cannot follow the same pattern as CoreWindow when setting the min width.
                // Instead, set a hardcoded number. 
            //    CustomDragRegion.MinWidth = 188;

                window.Frame.DragRegionVisuals.Add(CustomDragRegion);
				Tabs.TabItemsChanged += Tabs_TabItemsChanged;
			}
			
        }

        public TabPage AddChatTabs()
        {
            var selectIt = true;
            while (AddAnyChatTab(selectIt))
            {
                selectIt = false;
            }
            return this;
        }

        private void Window_Closed(AppWindow sender, AppWindowClosedEventArgs args)
        {
            tabPages.Remove(this);
            RemoveTabsOnClose();
        }

        //private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        //{
        //    // To ensure that the tabs in the titlebar are not occluded by shell
        //    // content, we must ensure that we account for left and right overlays.
        //    // In LTR layouts, the right inset includes the caption buttons and the
        //    // drag region, which is flipped in RTL. 

        //    // The SystemOverlayLeftInset and SystemOverlayRightInset values are
        //    // in terms of physical left and right. Therefore, we need to flip
        //    // then when our flow direction is RTL.
        //    if (FlowDirection == FlowDirection.LeftToRight)
        //    {
        //        CustomDragRegion.MinWidth = sender.SystemOverlayRightInset;
        //        ShellTitlebarInset.MinWidth = sender.SystemOverlayLeftInset;
        //    }
        //    else
        //    {
        //        CustomDragRegion.MinWidth = sender.SystemOverlayLeftInset;
        //        ShellTitlebarInset.MinWidth = sender.SystemOverlayRightInset;
        //    }

        //    // Ensure that the height of the custom regions are the same as the titlebar.
        //    CustomDragRegion.Height = ShellTitlebarInset.Height = sender.Height;
        //}

        public void Add(TabViewItem tab)
        {
            Tabs.TabItems.Add(tab);
            var ut = tab.Content as UserTab;
            if (ut != null)
            {
          //      Assert(!ut.isActive);
                ut.isActive = true;
             //   ut.XamlTreeChanged( (RootAppWindow != null) ? XamlRoot : null);
            }
            else
            {
                Assert(false);
            }

        }

        // Create a new Window once the Tab is dragged outside.
        private async void Tabs_TabDroppedOutside(TabView sender, TabViewTabDroppedOutsideEventArgs args)
        {
            // AppWindow was introduced in Windows 10 version 18362 (ApiContract version 8). 
            // If the app is running on a version earlier than 18362, simply no-op.
            // If your app needs to support multiple windows on earlier versions of Win10, you can use CoreWindow/ApplicationView.
            // More information about showing multiple views can be found here: https://docs.microsoft.com/windows/uwp/design/layout/show-multiple-views
            if (!ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
            {
                return;
            }

            AppWindow newWindow = await AppWindow.TryCreateAsync();
			newWindow.PersistedStateId = $"tabWindow{tabWindows.Count}";
            tabWindows.Add(newWindow);

            newWindow.Closed += (sender,b)=> tabWindows.Remove(sender);

            var newPage = new TabPage();
            newPage.SetupWindow(newWindow);
            Tabs.TabItems.Remove(args.Tab);
            var ut = args.Tab.Content as UserTab;
            ut.XamlTreeChanged(null);
            newWindow.RequestMoveAdjacentToCurrentView();
            ElementCompositionPreview.SetAppWindowContent(newWindow, newPage);

            newPage.Add(args.Tab);

            ut.XamlTreeChanged(newPage);

            await newWindow.TryShowAsync();

        }

        public static async Task CloseAllTabWindows()
        {
            while(tabWindows.Any())
            {
                await tabWindows.First().CloseAsync();
            }
        }

        private void Tabs_TabDragStarting(TabView sender, TabViewTabDragStartingEventArgs args)
        {
            // We can only drag one tab at a time, so grab the first one...
            var firstItem = args.Tab;

            // ... set the drag data to the tab...
            args.Data.Properties.Add(DataIdentifier, firstItem);

            // ... and indicate that we can move it 
            args.Data.RequestedOperation = DataPackageOperation.Move;
        }

        private void Tabs_TabStripDrop(object sender, DragEventArgs e)
        {
            // This event is called when we're dragging between different TabViews
            // It is responsible for handling the drop of the item into the second TabView

            if (e.DataView.Properties.TryGetValue(DataIdentifier, out object obj))
            {
                // Ensure that the obj property is set before continuing. 
                if (obj == null)
                {
                    return;
                }

                var destinationTabView = sender as TabView;
                var destinationItems = destinationTabView.TabItems;

                if (destinationItems != null)
                {
                    // First we need to get the position in the List to drop to
                    var index = -1;

                    // Determine which items in the list our pointer is between.
                    for (int i = 0; i < destinationTabView.TabItems.Count; i++)
                    {
                        var item = destinationTabView.ContainerFromIndex(i) as TabViewItem;

                        if (e.GetPosition(item).X - item.ActualWidth < 0)
                        {
                            index = i;
                            break;
                        }
                    }

                    // The TabView can only be in one tree at a time. Before moving it to the new TabView, remove it from the old.
                    var destinationTabViewListView = ((obj as TabViewItem).Parent as TabViewListView);
                    destinationTabViewListView.Items.Remove(obj);

                    if (index < 0)
                    {
                        // We didn't find a transition point, so we're at the end of the list
                        destinationItems.Add(obj);
                    }
                    else if (index < destinationTabView.TabItems.Count)
                    {
                        // Otherwise, insert at the provided index.
                        destinationItems.Insert(index, obj);
                    }

                    // Select the newly dragged tab
                    destinationTabView.SelectedItem = obj;
                }
            }
        }

        // This method prevents the TabView from handling things that aren't text (ie. files, images, etc.)
        private void Tabs_TabStripDragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Properties.ContainsKey(DataIdentifier))
            {
                e.AcceptedOperation = DataPackageOperation.Move;
            }
            //else
            //{
            //    e.AcceptedOperation = DataPackageOperation.Copy;
            //}
        }

        public MenuFlyoutItem AddTabMenuItem(UserTab tab)
        {
			var title = tab.Tag as string;
			
			
			
            var rv = new MenuFlyoutItem() { DataContext=tab, Text = title, Icon= GetOldIconForTab(tab)  };
			static void Rv_Click(object sender, RoutedEventArgs e)
			{
				((sender as MenuFlyoutItem).DataContext as UserTab).ShowOrAdd(true);
			}
			rv.Click += Rv_Click;
            return rv;
        }

		private void Tabs_AddTabButtonClick(TabView sender, object args)
        {
            var _args = args as RoutedEventArgs;
            var _sender = _args?.OriginalSource as FrameworkElement;
            if (_sender == null)
                _sender = sender;
            var menu = new MenuFlyout();
            foreach(var tab in UserTab.userTabs)
            {
                if(!tab.isActive)
                    menu.Items.Add(AddTabMenuItem(tab));
            }

            foreach (var tab in ChatTab.all)
            {
                if (tab.isActive)
                    continue;
                menu.Items.Add(AddTabMenuItem(tab));
            }
            if (menu.Items.Count == 0)
                menu.Items.Add(new MenuFlyoutItem() { Text = "All the tabs are open" });
            menu.CopyXamlRoomFrom(sender);

            menu.ShowAt(_sender);
            
                //AddChatTab(true);
                //sender.TabItems.Add(new TabViewItem()
                //{ IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Placeholder },
                //    Header = "New Item", Content = new ChatTab() { Tag = "New Item" } });
            }

            static void RemoveTab(TabView view, TabViewItem tab)
        {
			if (App.isShuttingDown)
				return;
			var itab = tab.Content as UserTab;
			tab.Content = null; // remove it
			view.TabItems.Remove(tab);
			if (itab != null)
            {
                itab.isVisible = false;
                itab.isActive = false;
                itab.VisibilityChanged(false);
				itab.Close();
            }
            // var chatTab = tab.Content as ChatTab;
            // Log("FreeTab1 " + chatTab.Name);
            // Assert(chatTab.isActive);
           // chatTab.isActive = false;
        }
        void RemoveTab(TabViewItem tab)
		{
            RemoveTab(Tabs, tab);
		}
        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
		       RemoveTab(sender,args.Tab);
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var tab in e.RemovedItems)
            {

                var userControl = (tab as TabViewItem).Content as UserTab;
                if (userControl != null)
                {
                    userControl.isVisible = false;
                    userControl.VisibilityChanged(false);
                }
            }
            foreach (var tab in e.AddedItems )
            {
                var userControl = (tab as TabViewItem).Content as UserTab;
                if (userControl != null)
                {
                    userControl.isVisible = true;

                    userControl.VisibilityChanged(true);
                }
            }
        }

		private void tip_Tapped(object sender, TappedRoutedEventArgs e)
		{
			if (TabPage.mainTabs.Visibility == Visibility.Collapsed)
				TabPage.mainTabs.Visibility = Visibility.Visible;
		}

		//private void NewTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
		//{
		//    var senderTabView = args.Element as TabView;
		//    Tabs_AddTabButtonClick(senderTabView,args);
		//    args.Handled = true;
		//}

		//private void CloseSelectedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
		//{
		//    var InvokedTabView = (args.Element as TabView);

		//    // Only close the selected tab if it is closeable
		//    if (((TabViewItem)InvokedTabView.SelectedItem).IsClosable)
		//    {
		//        InvokedTabView.TabItems.Remove(InvokedTabView.SelectedItem);
		//    }
		//    args.Handled = true;
		//}

		//private void NavigateToNumberedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
		//{
		//    var InvokedTabView = (args.Element as TabView);

		//    int tabToSelect = 0;

		//    switch (sender.Key)
		//    {
		//        case Windows.System.VirtualKey.Number1:
		//            tabToSelect = 0;
		//            break;
		//        case Windows.System.VirtualKey.Number2:
		//            tabToSelect = 1;
		//            break;
		//        case Windows.System.VirtualKey.Number3:
		//            tabToSelect = 2;
		//            break;
		//        case Windows.System.VirtualKey.Number4:
		//            tabToSelect = 3;
		//            break;
		//        case Windows.System.VirtualKey.Number5:
		//            tabToSelect = 4;
		//            break;
		//        case Windows.System.VirtualKey.Number6:
		//            tabToSelect = 5;
		//            break;
		//        case Windows.System.VirtualKey.Number7:
		//            tabToSelect = 6;
		//            break;
		//        case Windows.System.VirtualKey.Number8:
		//            tabToSelect = 7;
		//            break;
		//        case Windows.System.VirtualKey.Number9:
		//            // Select the last tab
		//            tabToSelect = InvokedTabView.TabItems.Count - 1;
		//            break;
		//    }

		//    // Only select the tab if it is in the list
		//    if (tabToSelect < InvokedTabView.TabItems.Count)
		//    {
		//        InvokedTabView.SelectedIndex = tabToSelect;
		//    }
		//    args.Handled = true;
		//}
	}
}
