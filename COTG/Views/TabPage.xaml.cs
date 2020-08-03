using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

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

using static COTG.Debug;

using SymbolIconSource = Microsoft.UI.Xaml.Controls.SymbolIconSource;

namespace COTG.Views
{
    public class UserTab : UserControl
    {
        public virtual void VisibilityChanged(bool visible)
        {
            Log($"VisibilityChanged: {visible} {this}");
        }
        // If this is not an xaml island, the newXamlRoot will be null
        public virtual void XamlTreeChanged(TabPage newPage) { } // The tab was dragged somewhere else
        public bool isVisible;
        public bool isActive;
        //User pressed F5 or refresh button
        public virtual void Refresh()
        {
            if (isVisible && isActive)
            {
                VisibilityChanged(false);  // close enough default behaviour
                VisibilityChanged(true);  // close enough default behaviour
            }
        }
    }
    public sealed partial class TabPage : Page
    {
        AppWindow RootAppWindow = null;

        private const string DataIdentifier = "ChatTabItem";
        public TabPage()
        {
            this.InitializeComponent();
            tabPages.Add(this);

        }
        static public List<TabPage> tabPages = new List<TabPage>();

        

        private void Tabs_TabItemsChanged(TabView sender, Windows.Foundation.Collections.IVectorChangedEventArgs args)
        {
            // If there are no more tabs, close the window.
            if (sender.TabItems.Count == 0)
            {
                if (RootAppWindow != null)
                {
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

                var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.LayoutMetricsChanged -= CoreTitleBar_LayoutMetricsChanged;

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

                AddTab(tab, selectIt);
                return true;
            }
            return false; 
        }

        public void AddTab(UserTab tab, bool selectIt)
        {
            var vi = new TabViewItem()
            {
                Header = tab.DataContext as string,
                IconSource = GetIconForTab(tab),
                Content = tab
            };
            Add(vi);
            if (selectIt)
                Tabs.SelectedItem = vi;
        }

        static Dictionary<string, Symbol> tabSymbolIcons = new Dictionary<string, Symbol> {
            { "Raid", Symbol.ReShare },
            { "Donation", Symbol.Share },
            { "world", Symbol.Microphone },
            { "alliance" ,  Symbol.People },
            { "whisper" , Symbol.Comment },
            { "officer" ,Symbol.Admin },
        };

        static Dictionary<string, string> tabFontIcons = new Dictionary<string, string> {
            { "Defender" , "\uEA18" },//tab.DataContext as string,
            {    "Defense", "\uEA0D" },
            {    "Recent" ,  "\uF738" },
        };
        private static Microsoft.UI.Xaml.Controls.IconSource GetIconForTab(UserTab tab)
        {
            if (tabSymbolIcons.TryGetValue(tab.DataContext as string, out var symbol))
                return new SymbolIconSource() { Symbol = symbol };
            if (tabFontIcons.TryGetValue(tab.DataContext as string, out var glyph))
                return new Microsoft.UI.Xaml.Controls.FontIconSource() { Glyph = glyph };
            return null;
        }
        private static IconElement GetOldIconForTab(UserTab tab)
        {
            if (tabSymbolIcons.TryGetValue(tab.DataContext as string, out var symbol))
                return new SymbolIcon() { Symbol = symbol };
            if (tabFontIcons.TryGetValue(tab.DataContext as string, out var glyph))
                return new FontIcon() { Glyph = glyph };
            return null;
        }

        void SetupWindow(AppWindow window)
        {
            if (window == null)
            {
                // Main Window -- add some default items
              

                Tabs.SelectedIndex = 0;

                // Extend into the titlebar
                var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = true;

                coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;

                Window.Current.SetTitleBar(CustomDragRegion);
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
                CustomDragRegion.MinWidth = 188;

                window.Frame.DragRegionVisuals.Add(CustomDragRegion);
            }
            Tabs.TabItemsChanged -= Tabs_TabItemsChanged;
            Tabs.TabItemsChanged += Tabs_TabItemsChanged;

        }

        public void AddChatTabs()
        {
            var selectIt = true;
            while (AddAnyChatTab(selectIt))
            {
                selectIt = false;
            }
        }

        private void Window_Closed(AppWindow sender, AppWindowClosedEventArgs args)
        {
            tabPages.Remove(this);
            RemoveTabsOnClose();
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            // To ensure that the tabs in the titlebar are not occluded by shell
            // content, we must ensure that we account for left and right overlays.
            // In LTR layouts, the right inset includes the caption buttons and the
            // drag region, which is flipped in RTL. 

            // The SystemOverlayLeftInset and SystemOverlayRightInset values are
            // in terms of physical left and right. Therefore, we need to flip
            // then when our flow direction is RTL.
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                CustomDragRegion.MinWidth = sender.SystemOverlayRightInset;
                ShellTitlebarInset.MinWidth = sender.SystemOverlayLeftInset;
            }
            else
            {
                CustomDragRegion.MinWidth = sender.SystemOverlayLeftInset;
                ShellTitlebarInset.MinWidth = sender.SystemOverlayRightInset;
            }

            // Ensure that the height of the custom regions are the same as the titlebar.
            CustomDragRegion.Height = ShellTitlebarInset.Height = sender.Height;
        }

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

            var newPage = new TabPage();
            newPage.SetupWindow(newWindow);
            Tabs.TabItems.Remove(args.Tab);
            var ut = args.Tab.Content as UserTab;
            ut.XamlTreeChanged(null);

            ElementCompositionPreview.SetAppWindowContent(newWindow, newPage);

            newPage.Add(args.Tab);

            ut.XamlTreeChanged(newPage);

            await newWindow.TryShowAsync();

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

        private async void Tabs_TabStripDrop(object sender, DragEventArgs e)
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
        }

        public MenuFlyoutItem AddTabMenuItem(UserTab tab)
        {
            var title = tab.DataContext as string;
            var rv = new MenuFlyoutItem() { Text = title, Icon= GetOldIconForTab(tab)  };
            rv.Click += (_, _) => AddTab(tab,true);
            return rv;
        }
        private void Tabs_AddTabButtonClick(TabView sender, object args)
        {
            var _args = args as RoutedEventArgs;
            var _sender = _args?.OriginalSource as FrameworkElement;
            if (_sender == null)
                _sender = sender;
            var menu = new MenuFlyout();
            if (!MainPage.instance.isActive)
                menu.Items.Add(AddTabMenuItem(MainPage.instance));
            if (!DefenderPage.instance.isActive)
                menu.Items.Add(AddTabMenuItem(DefenderPage.instance));
            if (!DefensePage.instance.isActive)
                menu.Items.Add(AddTabMenuItem(DefensePage.instance));
            if (!SpotTab.instance.isActive)
                menu.Items.Add(AddTabMenuItem(SpotTab.instance));

            foreach (var tab in ChatTab.all)
            {
                if (tab.isActive)
                    continue;
                menu.Items.Add(AddTabMenuItem(tab));
            }
            if (menu.Items.Count == 0)
                menu.Items.Add(new MenuFlyoutItem() { Text = "All the tabs are open" });
            menu.XamlRoot = sender.XamlRoot;

            menu.ShowAt(_sender);
            
                //AddChatTab(true);
                //sender.TabItems.Add(new TabViewItem()
                //{ IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Placeholder },
                //    Header = "New Item", Content = new ChatTab() { DataContext = "New Item" } });
            }
            static void RemoveTab(TabView view, TabViewItem tab)
        {
            var itab = tab.Content as UserTab;
            if (itab != null)
            {
                itab.isVisible = false;
                itab.isActive = false;
                itab.VisibilityChanged(false);
            }
            // var chatTab = tab.Content as ChatTab;
            // Log("FreeTab1 " + chatTab.Name);
            // Assert(chatTab.isActive);
            // chatTab.isActive = false;
            tab.Content = null; // remove it
            view.TabItems.Remove(tab);
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

        private void NewTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var senderTabView = args.Element as TabView;
            Tabs_AddTabButtonClick(senderTabView,args);
            args.Handled = true;
        }

        private void CloseSelectedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var InvokedTabView = (args.Element as TabView);

            // Only close the selected tab if it is closeable
            if (((TabViewItem)InvokedTabView.SelectedItem).IsClosable)
            {
                InvokedTabView.TabItems.Remove(InvokedTabView.SelectedItem);
            }
            args.Handled = true;
        }

        private void NavigateToNumberedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var InvokedTabView = (args.Element as TabView);

            int tabToSelect = 0;

            switch (sender.Key)
            {
                case Windows.System.VirtualKey.Number1:
                    tabToSelect = 0;
                    break;
                case Windows.System.VirtualKey.Number2:
                    tabToSelect = 1;
                    break;
                case Windows.System.VirtualKey.Number3:
                    tabToSelect = 2;
                    break;
                case Windows.System.VirtualKey.Number4:
                    tabToSelect = 3;
                    break;
                case Windows.System.VirtualKey.Number5:
                    tabToSelect = 4;
                    break;
                case Windows.System.VirtualKey.Number6:
                    tabToSelect = 5;
                    break;
                case Windows.System.VirtualKey.Number7:
                    tabToSelect = 6;
                    break;
                case Windows.System.VirtualKey.Number8:
                    tabToSelect = 7;
                    break;
                case Windows.System.VirtualKey.Number9:
                    // Select the last tab
                    tabToSelect = InvokedTabView.TabItems.Count - 1;
                    break;
            }

            // Only select the tab if it is in the list
            if (tabToSelect < InvokedTabView.TabItems.Count)
            {
                InvokedTabView.SelectedIndex = tabToSelect;
            }
            args.Handled = true;
        }
    }
}
