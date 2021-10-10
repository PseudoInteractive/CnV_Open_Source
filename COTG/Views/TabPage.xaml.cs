using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using COTG.Game;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Metadata;
//using Windows.UI.ViewManagement;
//using Windows.UI.WindowManagement;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using static COTG.Debug;
using SymbolIconSource = Microsoft.UI.Xaml.Controls.SymbolIconSource;
using System.Linq;
//using Microsoft.UI.Windowing;

namespace COTG.Views
{
	public sealed partial class TabPage:Page
	{
		public static List<Window> tabWindows = new();
		public static TabPage mainTabs;
		public static TabPage secondaryTabs;
		const Window RootAppWindow = null;
		static public TabPage[] tabPages;
		static public UserTab[][] hiddenTabs;

		static TabPage CreateTabView(Frame frame)
		{
			frame.Navigate(typeof(TabPage));
			return frame.Content as TabPage;

		}
		public static void Initialize()
		{
		

			try
			{
				ChatTab.Ctor();

				mainTabs = CreateTabView(ShellPage.instance.shellTabs);
				secondaryTabs = CreateTabView(ShellPage.instance.spotTabs);
				ChatTab.tabPage = CreateTabView(ShellPage.instance.chatTabs);
				tabPages = new[] { mainTabs,secondaryTabs,ChatTab.tabPage };

				UserTab.InitUserTabs();
				

			}
			catch(Exception __ex)
			{
				Debug.LogEx(__ex);
			}
		}

		public void Add(UserTab me,bool selectMe)
		{
			me.isOpen = true;
			var vi = new TabViewItem()
			{
				Header = me.Tag as string,
				IconSource = TabPage.GetIconForTab(me),
				Content = me
			};
			Tabs.TabItems.Add(vi );
			if(selectMe)
				this.Tabs.SelectedItem= vi;
		}
		private const string DataIdentifier = "ChatTabItem";
		public TabPage()
		{
			//App.instance.Resources["TabViewButtonBackground"] = new SolidColorBrush();
			//App.instance.Resources["TabViewButtonForeground"] = new SolidColorBrush();
			//App.instance.Resources["OverlayCornerRadius"] = 1.0;
			//		App.instance.Resources["TopCornerRadiusFilterConverter"] = new 
			this.InitializeComponent();
			//	IsTabStop = true;
		//	TabFocusNavigation = KeyboardNavigationMode.Once;
			//	AllowFocusOnInteraction = true;


		}

		// chat tab is never hidden
		public static bool IsBigEnoughToSee( FrameworkElement e) => e.ActualHeight > 10 && e.ActualWidth > 10;
	//	public bool shouldBeVisible=> (ShellPage.rightTabsVisible || object.ReferenceEquals(this,ChatTab.tabPage));
		public static async void LayoutChanged()
		{
			await Task.Delay(200);

			var rightVisible = ShellPage.rightTabsVisible;
			bool[] pagesVisible= {
				rightVisible && IsBigEnoughToSee(mainTabs),
				rightVisible&&IsBigEnoughToSee(secondaryTabs),
				true || IsBigEnoughToSee(ChatTab.tabPage)			// leave chat for now
				};
			var pageCount = tabPages.Length;
			if(hiddenTabs == null)
				hiddenTabs = new UserTab[pageCount][];
			
			Assert(pageCount == pagesVisible.Length);
			for(int pageId=0;pageId<pageCount;++pageId)
			{
	//			ref var hidden = ref hiddenTabs[pageId];

				var hiddenTab = hiddenTabs[pageId];
				var page = tabPages[pageId];
				var visibleNow = pagesVisible[pageId];
				if(visibleNow == page.isVisible )
				{
					// leave it, maybe resize columns
				}
				else
				{
					page.isVisible = visibleNow;
					if(visibleNow)
					{
						page.RemoveAllTabs();
						var first= true;
						if(hiddenTabs[pageId] != null )
						{
							foreach( var i in hiddenTabs[pageId])
							{
								page.Add(i,first);
								first = false;
								i.refresh.Go();
							}
						}
						hiddenTabs[pageId] = null;
					}
					else
					{
						hiddenTabs[pageId] = page.Tabs.TabItems.Select(a => (a as TabViewItem).Content as UserTab).
							OrderByDescending(a=>a.isFocused).ToArray();  //replace
						page.RemoveAllTabs();
					}

				}
			}
		}

		public static bool Show(UserTab tab)
		{
			bool rv = false;
			foreach(var tabPage in tabPages)
			{
				foreach(TabViewItem ti in tabPage.Tabs.TabItems)
				{
					if(ti.Content.AsObject() == tab)
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
			if(App.isShuttingDown)
				return false;
			foreach(var tabPage in tabPages)
			{
				foreach(TabViewItem ti in tabPage.Tabs.TabItems)
				{
					if(ti.Content.AsObject() == tab)
					{
						tabPage.RemoveTab(ti);
						break;
					}
				}
			}

			return rv;
		}

		
		//public static (TabPage page, bool found) Get(UserTab tab)
		//{
		//	bool rv = false;
		//	foreach(var tabPage in tabPages)
		//	{
		//		foreach(TabViewItem ti in tabPage.Tabs.TabItems)
		//		{
		//			if(ti.Content == tab)
		//			{
		//				return (tabPage, true);
		//			}
		//		}
		//	}
		//	return (mainTabs, false);
		//}
		//private void Tabs_TabItemsChanged(TabView sender,Windows.Foundation.Collections.IVectorChangedEventArgs args)
		//{
		//	// If there are no more Tabs, close the window.
		//	if(sender.TabItems.Count == 0)
		//	{
		//		if(RootAppWindow != null)
		//		{
		//			//Tabs.TabItemsChanged -= Tabs_TabItemsChanged;
		//			//					RootAppWindow.CloseAsync();

		//		}
		//		//    else
		//		//    {
		//		//        Window.Current.Close();
		//		//    }
		//	}
		//}

	
		void RemoveAllTabs()
		{
			if(App.isShuttingDown)
				return;

			var _tab = Tabs;
			while(_tab.TabItems.Count > 0)
			{
				
				RemoveTab(_tab.TabItems[0] as TabViewItem);
			}
		}



		bool AddAnyChatTab(bool selectIt)
		{
			foreach(var tab in ChatTab.all)
			{
				if(tab.isOpen)
					continue;

				tab.ShowOrAdd(selectIt);
				return true;
			}
			return false;
		}

		public static void ShowTabs()
		{
			App.DispatchOnUIThreadIdle(()=>
			{
				MainPage.instance.ShowOrAdd(true);
				SpotTab.instance.ShowOrAdd(true);
				ChatTab.tabPage.AddChatTabs();
			});
		}


		static Dictionary<string,Symbol> tabSymbolIcons = new Dictionary<string,Symbol> {
			{ "Raid", Symbol.ReShare },
			{ "Donation", Symbol.Share },
			{ "Boss", Symbol.View },
			{ "AttackPlanner", Symbol.Audio },
			{ "world", Symbol.Microphone },
			{ "Build", Symbol.Repair },
			{ "Planner", Symbol.Map },
			{ "NearRes", Symbol.Favorite },
			{ "officer" ,Symbol.Admin },
			{ "WebView", Symbol.World },
		};

		static Dictionary<string,string> tabFontIcons = new Dictionary<string,string> {
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
		public bool isVisible=true;

		public static Microsoft.UI.Xaml.Controls.IconSource GetIconForTab(UserTab tab)
		{
			if(tabSymbolIcons.TryGetValue(tab.Tag as string,out var symbol))
				return new SymbolIconSource() { Symbol = symbol };
			if(tabFontIcons.TryGetValue(tab.Tag as string,out var glyph))
				return new Microsoft.UI.Xaml.Controls.FontIconSource() { Glyph = glyph };
			return new SymbolIconSource() { Symbol = Symbol.Comment };
		}
		private static IconElement GetOldIconForTab(UserTab tab)
		{
			if(tabSymbolIcons.TryGetValue(tab.Tag as string,out var symbol))
				return new SymbolIcon() { Symbol = symbol };
			if(tabFontIcons.TryGetValue(tab.Tag as string,out var glyph))
				return new FontIcon() { Glyph = glyph };
			return new SymbolIcon() { Symbol=Symbol.Comment }; // whisper
		}

		

		//private void Window_Closing(AppWindow sender,AppWindowClosingEventArgs args)
		//{
		//	tabPages.Remove(this);
		//	RemoveTabsOnClose();
		//}

		public TabPage AddChatTabs()
		{
			var selectIt = true;
			while(AddAnyChatTab(selectIt))
			{
				selectIt = false;
			}
			return this;
		}


		//private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
		//{
		//    // To ensure that the Tabs in the titlebar are not occluded by shell
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

		

		// Create a new Window once the Tab is dragged outside.
		private async void Tabs_TabDroppedOutside(TabView sender,TabViewTabDroppedOutsideEventArgs args)
		{
			// AppWindow was introduced in Windows 10 version 18362 (ApiContract version 8). 
			// If the app is running on a version earlier than 18362, simply no-op.
			// If your app needs to support multiple windows on earlier versions of Win10, you can use CoreWindow/ApplicationView.
			// More information about showing multiple views can be found here: https://docs.microsoft.com/windows/uwp/design/layout/show-multiple-views
			if(!ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract",8))
			{
				return;
			}

			//         AppWindow newWindow = await AppWindow.TryCreateAsync();
			//newWindow.PersistedStateId = $"tabWindow{tabWindows.Count}";
			//         tabWindows.Add(newWindow);

			//         newWindow.Closed += (sender,b)=> tabWindows.Remove(sender);

			//         var newPage = new TabPage();
			//         newPage.SetupWindow(newWindow);
			//         Tabs.TabItems.Remove(args.Tab);
			//         var ut = args.Tab.Content as UserTab;
			//         ut.XamlTreeChanged(null);
			//         newWindow.RequestMoveAdjacentToCurrentView();
			//         ElementCompositionPreview.SetAppWindowContent(newWindow, newPage);

			//         newPage.Add(args.Tab);

			//         ut.XamlTreeChanged(newPage);

			//         await newWindow.TryShowAsync();

		}

		public static async Task CloseAllTabWindows()
		{
			//	await Task.WhenAll(tabWindows.Select( a=>a.CloseAsync().AsTask() ));
		}

		private void Tabs_TabDragStarting(TabView sender,TabViewTabDragStartingEventArgs args)
		{
			// We can only drag one tab at a time, so grab the first one...
			var firstItem = args.Tab;

			// ... set the drag data to the tab...
			args.Data.Properties.Add(DataIdentifier,firstItem);

			// ... and indicate that we can move it 
			args.Data.RequestedOperation = DataPackageOperation.Move;
		}

		private void Tabs_TabStripDrop(object sender,DragEventArgs e)
		{
			// This event is called when we're dragging between different TabViews
			// It is responsible for handling the drop of the item into the second TabView

			if(e.DataView.Properties.TryGetValue(DataIdentifier,out object obj))
			{
				// Ensure that the obj property is set before continuing. 
				if(obj == null)
				{
					return;
				}

				var destinationTabView = sender as TabView;
				var destinationItems = destinationTabView.TabItems;

				if(destinationItems != null)
				{
					// First we need to get the position in the List to drop to
					var index = -1;

					// Determine which items in the list our pointer is between.
					for(int i = 0;i < destinationTabView.TabItems.Count;i++)
					{
						var item = destinationTabView.ContainerFromIndex(i) as TabViewItem;

						if(e.GetPosition(item).X - item.ActualWidth < 0)
						{
							index = i;
							break;
						}
					}

					// The TabView can only be in one tree at a time. Before moving it to the new TabView, remove it from the old.
					var destinationTabViewListView = ((obj as TabViewItem).Parent as TabViewListView);
					destinationTabViewListView.Items.Remove(obj);

					if(index < 0)
					{
						// We didn't find a transition point, so we're at the end of the list
						destinationItems.Add(obj);
					}
					else if(index < destinationTabView.TabItems.Count)
					{
						// Otherwise, insert at the provided index.
						destinationItems.Insert(index,obj);
					}

					// Select the newly dragged tab
					destinationTabView.SelectedItem = obj;
				}
			}
		}

		// This method prevents the TabView from handling things that aren't text (ie. files, images, etc.)
		private void Tabs_TabStripDragOver(object sender,DragEventArgs e)
		{
			if(e.DataView.Properties.ContainsKey(DataIdentifier))
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



			var rv = new MenuFlyoutItem() { DataContext=tab,Text = title,Icon= GetOldIconForTab(tab) };
			static void Rv_Click(object sender,RoutedEventArgs e)
			{
				((sender as MenuFlyoutItem).DataContext as UserTab).ShowOrAdd(true);
			}
			rv.Click += Rv_Click;
			return rv;
		}

		private void Tabs_AddTabButtonClick(TabView sender,object args)
		{
			var _args = args as RoutedEventArgs;
			var _sender = _args?.OriginalSource as FrameworkElement;
			if(_sender == null)
				_sender = sender;
			var menu = new MenuFlyout();
			foreach(var tab in UserTab.userTabs)
			{
				if(!tab.isOpen)
					menu.Items.Add(AddTabMenuItem(tab));
			}

			foreach(var tab in ChatTab.all)
			{
				if(tab.isOpen)
					continue;
				menu.Items.Add(AddTabMenuItem(tab));
			}
			if(menu.Items.Count == 0)
				menu.Items.Add(new MenuFlyoutItem() { Text = "All the Tabs are open" });
			menu.CopyXamlRoomFrom(sender);

			menu.ShowAt(_sender);

			//AddChatTab(true);
			//sender.TabItems.Add(new TabViewItem()
			//{ IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Placeholder },
			//    Header = "New Item", Content = new ChatTab() { Tag = "New Item" } });
		}

		static void RemoveTab(TabView view,TabViewItem tab)
		{
			if(App.isShuttingDown)
				return;
			var itab = tab.Content as UserTab;
			tab.Content = null; // remove it
			view.TabItems.Remove(tab);
			if(itab != null)
			{
				itab.isFocused = false;
				itab.isOpen = false;
				itab.VisibilityChanged(false,longTerm: true);
				itab.Close();
			}
			// var chatTab = tab.Content as ChatTab;
			// Log("FreeTab1 " + chatTab.Name);
			// Assert(chatTab.isActive);
			// chatTab.isActive = false;
		}
		void RemoveTab(TabViewItem tab)
		{
			RemoveTab(Tabs,tab);
		}
		private void Tabs_TabCloseRequested(TabView sender,TabViewTabCloseRequestedEventArgs args)
		{
			RemoveTab(sender,args.Tab);
		}

		private async void Tabs_SelectionChanged(object sender,SelectionChangedEventArgs e)
		{
			foreach(var tab in e.RemovedItems)
			{

				var userControl = (tab as TabViewItem).Content as UserTab;
				if(userControl != null)
				{
					if(userControl.isFocused)
					{
						userControl.isFocused = false;
						await userControl.VisibilityChanged(false,longTerm: false);
					}
				}
			}
			foreach(var tab in e.AddedItems)
			{
				var userControl = (tab as TabViewItem).Content as UserTab;
				if(userControl != null)
				{
					if(!userControl.isFocused)
					{
						{
							userControl.isFocused = true;
							userControl.VisibilityChanged(true,false);
						}
					}
				}
			}
		}



		//private void tip_Tapped(object sender,TappedRoutedEventArgs e)
		//{
		//	if(TabPage.mainTabs.Visibility == Visibility.Collapsed)
		//		TabPage.mainTabs.Visibility = Visibility.Visible;
		//}

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
