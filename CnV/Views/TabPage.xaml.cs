using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CnV.Game;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Metadata;
//using Windows.UI.ViewManagement;
//using Windows.UI.WindowManagement;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using static CnV.Debug;
using SymbolIconSource = Microsoft.UI.Xaml.Controls.SymbolIconSource;
using System.Linq;
//using Microsoft.UI.Windowing;

namespace CnV.Views
{
	public sealed partial class TabPage:Grid {
		public static List<Window> tabWindows = new();
		public static TabPage? mainTabs;
		public static TabPage? secondaryTabs;
		public static TabPage[]? tabPages;
		public static UserTab[][]? hiddenTabs;

		
		public static void Initialize() {


			try {
				ChatTab.Ctor();

				mainTabs = (ShellPage.instance.rightTabs);
				secondaryTabs = (ShellPage.instance.spotTabs);
				ChatTab.tabPage = (ShellPage.instance.chatTabs);
				tabPages = new[] { mainTabs,secondaryTabs,ChatTab.tabPage };

				UserTab.InitUserTabs();


			}
			catch(Exception __ex) {
				Debug.LogEx(__ex);
			}
		}

		internal void Add(TabInfo info,bool selectMe,UserTab tab) {
			//me.isOpen = true;
			var vi = new TabViewItem() {
				Header =tab is not null ? tab.Tag as string : info.name,
				IconSource = TabPage.GetIconForTab(info),
				Tag= info,
				Content =  new Frame() {  IsNavigationStackEnabled=false},
				};
				var content = tab ??  (info.persist ? info.t.GetConstructor(Type.EmptyTypes).Invoke(null) as UserTab : null);
			vi.Set( content );

			//if(info.persist  )
			//{
			//	if(tab is not null)
			//		f.Navigate()
			//	f.Navigate(	  ),
		
			//}
			Tabs.TabItems.Add(vi );

//			me.Opened();

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
		public static void LayoutChanged()
		{
	//		await Task.Delay(200);

	//		var rightVisible = ShellPage.rightTabsVisible;
	//		bool[] pagesVisible= {
	//			rightVisible && IsBigEnoughToSee(mainTabs),
	//			rightVisible&&IsBigEnoughToSee(secondaryTabs),
	//			true || IsBigEnoughToSee(ChatTab.tabPage)			// leave chat for now
	//			};
	//		var pageCount = tabPages.Length;
	//		if(hiddenTabs == null)
	//			hiddenTabs = new UserTab[pageCount][];
			
	//		Assert(pageCount == pagesVisible.Length);
	//		for(int pageId=0;pageId<pageCount;++pageId)
	//		{
	////			ref var hidden = ref hiddenTabs[pageId];

	//			var hiddenTab = hiddenTabs[pageId];
	//			var page = tabPages[pageId];
	//			var visibleNow = pagesVisible[pageId];
	//			if(visibleNow == page.isVisible )
	//			{
	//				// leave it, maybe resize columns
	//			}
	//			else
	//			{
	//				page.isVisible = visibleNow;
	//				if(visibleNow)
	//				{
	//					page.RemoveAllTabs();
	//					var first= true;
	//					if(hiddenTabs[pageId] != null )
	//					{
	//						foreach( var i in hiddenTabs[pageId])
	//						{
	//							page.Add(i,first);
	//							first = false;
	//							i.refresh.Go();
	//						}
	//					}
	//					hiddenTabs[pageId] = null;
	//				}
	//				else
	//				{
	//					hiddenTabs[pageId] = page.Tabs.TabItems.Select(a => (a as TabViewItem).Content as UserTab).
	//						OrderByDescending(a=>a.isFocused).ToArray();  //replace
	//					page.RemoveAllTabs();
	//				}

	//			}
	//		}
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
		public static Task Close(UserTab tab)
		{
			bool rv = false;
			if(AppS.isShuttingDown)
				return Task.CompletedTask;
			foreach(var tabPage in tabPages)
			{
				foreach(TabViewItem ti in tabPage.Tabs.TabItems)
				{
					if( object.ReferenceEquals( ti.GetTab(), tab) )
					{
						// This might trigger selection changed?
						return tabPage.RemoveTab(ti);

					}
				}
			}

			return Task.CompletedTask;
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

	
		//void RemoveAllTabs()
		//{
		//	if(AppS.isShuttingDown)
		//		return;

		//	var _tab = Tabs;
		//	while(_tab.TabItems.Count > 0)
		//	{
				
		//		RemoveTab(_tab.TabItems[0] as TabViewItem);
		//	}
		//}



		//bool AddAnyChatTab(bool selectIt)
		//{
		//	foreach(var tab in ChatTab.all)
		//	{
		//		if(tab.isOpen)
		//			continue;

		//		tab.ShowOrAdd(selectIt);
		//		return true;
		//	}
		//	return false;
		//}

		public static void ShowTabs()
		{
			AppS.DispatchOnUIThreadIdle(()=>
			{
				BuildTab.ShowOrAdd<BuildTab>(true);
				SpotTab.ShowOrAdd<SpotTab>(true);
			//	PlayerTab.instance.ShowOrAdd(false);
				MainPage.ShowOrAdd<MainPage>(false);
				
			//	ChatTab.tabPage.AddChatTabs();
			});
		}


		//static Dictionary<string,Symbol> tabSymbolIcons = new Dictionary<string,Symbol> {
		//	{ "Raid", Symbol.ReShare },
		//	{ "RssSender", Symbol.Share },
		//	{ "Boss", Symbol.View },
		//	{ "AttackPlanner", Symbol.Audio },
		//	{ "world", Symbol.Microphone },
		//	{ "Build", Symbol.Repair },
		//	{ "Reinforcements", Symbol.AddFriend },
		//	{ "Planner", Symbol.Map },
		//	{ "NearRes", Symbol.Download },
		//	{ "officer" ,Symbol.Admin },
		//	{ "BossHits", Symbol.Play },
		//	{ "Timeline", Symbol.Calendar },
		//	{ "Palace", Symbol.Like }
		//};

		//static Dictionary<string,string> tabFontIcons = new Dictionary<string,string> {
		//	{ "Incoming" , "\uF0EF"  },//tab.Tag as string,
  //          {    "DefenseHistory", "\uEA0D" },
		//	{    "Recent" ,  "\uF738" },
		//	{  "NearDefense", "\uEA18" },
		//	{ "alliance", "\uE902" },
		//	{ "player", "\uE902" },
		//	{ "Outgoing","\uE189" },
		//	{ "Hits","\uEA69" },
		//	{ "Chart","\uEA69" },
		//	{ "Heat", "\uF738" },
		//	{ "PlayerChange", "\uE822" }
		//};
		public bool isVisible=true;

		internal static Microsoft.UI.Xaml.Controls.IconSource GetIconForTab(TabInfo tab)
		{
			if(tab.fontIcon != default)
				return new Microsoft.UI.Xaml.Controls.FontIconSource() { Glyph = tab.fontIcon.ToString() };
			return new SymbolIconSource() { Symbol = tab.symbol };
		}

		internal static Microsoft.UI.Xaml.Controls.IconElement GetOldIconForTab(TabInfo tab)
		{
			if(tab.fontIcon != default)
				return new Microsoft.UI.Xaml.Controls.FontIcon() { Glyph = tab.fontIcon.ToString() };
			return new SymbolIcon() { Symbol = tab.symbol };
		}
		//private static IconElement GetOldIconForTab(UserTab tab)
		//{
		//	if (tab.Tag is null)
		//		return new SymbolIcon() { Symbol = Symbol.Emoji2 };
		//	if(tabSymbolIcons.TryGetValue(tab.Tag as string,out var symbol))
		//		return new SymbolIcon() { Symbol = symbol };
		//	if(tabFontIcons.TryGetValue(tab.Tag as string,out var glyph))
		//		return new FontIcon() { Glyph = glyph };
		//	return new SymbolIcon() { Symbol=Symbol.Comment }; // whisper
		//}

		

		//private void Window_Closing(AppWindow sender,AppWindowClosingEventArgs args)
		//{
		//	tabPages.Remove(this);
		//	RemoveTabsOnClose();
		//}

		//public TabPage AddChatTabs()
		//{
		//	var selectIt = true;
		//	while(AddAnyChatTab(selectIt))
		//	{
		//		selectIt = false;
		//	}
		//	return this;
		//}


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
			//if(!ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract",8))
			//{
			//	return;
			//}

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

		internal MenuFlyoutItem AddTabMenuItem(TabInfo info)
		{
			var title = info.name;
			Assert(title is not null);


			var rv = new MenuFlyoutItem() { DataContext=info,Text = title,Icon= GetOldIconForTab(info) };

			void Rv_Click(object sender,RoutedEventArgs e)
			{
				UserTab.ShowOrAdd( (sender as MenuFlyoutItem).DataContext as TabInfo,true,page:this);
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
			foreach(var tab in TabInfo.all)
			{
				if(tab.t == typeof(PlannerTab) || tab.t== typeof(ChatTab))
					continue;
				if(!tab.isOpen) 
					menu.Items.Add(AddTabMenuItem(tab));
			}

			//foreach(var tab in ChatTab.all)
			//{
			//	if(tab.isOpen)
			//		continue;
			//	menu.Items.Add(AddTabMenuItem(tab));
			//}
			if(menu.Items.Count == 0)
				menu.Items.Add(new MenuFlyoutItem() { Text = "All the Tabs are open" });
			menu.SetXamlRoot(sender);

			menu.ShowAt(_sender);

			//AddChatTab(true);
			//sender.TabItems.Add(new TabViewItem()
			//{ IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Placeholder },
			//    Header = "New Item", Content = new ChatTab() { Tag = "New Item" } });
		}

		static Task RemoveTab(TabView view,TabViewItem tab)
		{
			if(AppS.isShuttingDown)
				return Task.CompletedTask;
			var itab = tab.GetTab();
			tab.Clear(); // remove it
			view.TabItems.Remove(tab);
			if(itab != null)
			{
			
				return itab.Closed();
			}
			// var chatTab = tab.Content as ChatTab;
			// Log("FreeTab1 " + chatTab.Name);
			// Assert(chatTab.isActive);
			// chatTab.isActive = false;
			return Task.CompletedTask;
		}
		Task RemoveTab(TabViewItem tab)
		{
			return RemoveTab(Tabs,tab);
		}
		private void Tabs_TabCloseRequested(TabView sender,TabViewTabCloseRequestedEventArgs args)
		{
			RemoveTab(sender,args.Tab);
		}

		private async void Tabs_SelectionChanged(object sender,SelectionChangedEventArgs e)
		{
			foreach(var tab in e.RemovedItems)
			{

				var userTab = (tab as TabViewItem).GetTab();
				if(userTab != null) {
					
					var info = TabInfo.Get(userTab.GetType());
					if(!info.persist) {
						
						(tab as TabViewItem).Clear();
						await userTab.Closed();
					}
					else {
						if(userTab.isFocused) {
							userTab.isFocused = false;
							await userTab.VisibilityChanged(false,longTerm: false);
						}
					}
				}
				else {
					//Assert(false);
				}

			}
			foreach(var tab in e.AddedItems)
			{
				var userTab = (tab as TabViewItem).GetTab();
				if(userTab == null) {
					var info = (tab as TabViewItem).Tag as TabInfo;
					userTab = info.t.GetConstructor(Type.EmptyTypes).Invoke(null) as UserTab;
					// opened etc
					userTab.isOpen=true;
					userTab.Opened();

					(tab as TabViewItem).Set( userTab );
				}

				if(!userTab.isFocused)
				{
					userTab.isFocused = true;
					await userTab.VisibilityChanged(true,false);
				}
				
			}

			CityUI.SyncSelectionToUI(true);
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
