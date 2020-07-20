using System;
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
using Windows.UI.Xaml.Navigation;

using static COTG.Debug;
namespace COTG.Views
{
    public sealed partial class TabPage : Page
    {
        AppWindow RootAppWindow = null;

        private const string DataIdentifier = "ChatTabItem";
        public TabPage()
        {
            this.InitializeComponent();

        }

        private async void Tabs_TabItemsChanged(TabView sender, Windows.Foundation.Collections.IVectorChangedEventArgs args)
        {
            // If there are no more tabs, close the window.
            //if (sender.TabItems.Count == 0)
            //{
            //    if (RootAppWindow != null)
            //    {
            //        await RootAppWindow.CloseAsync();
            //    }
            //    else
            //    {
            //        Window.Current.Close();
            //    }
            //}
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

        bool AddAnyTab()
        {
            foreach (var tab in ChatTab.all)
            {
                if (tab.isActive)
                    continue;
                tab.isActive = true;
                var vi =new TabViewItem()
                {
                    IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Placeholder },
                    Header = tab.DataContext as string,
                    Content = tab
                };
                Tabs.TabItems.Add(vi);
                Tabs.SelectedItem = vi;
                return true;
            }
            return false; 
        }

        void SetupWindow(AppWindow window)
        {
            if (window == null)
            {
                // Main Window -- add some default items
                while (AddAnyTab()) { }

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

        private void Window_Closed(AppWindow sender, AppWindowClosedEventArgs args)
        {
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

        public void AddTabToTabs(TabViewItem tab)
        {
            Tabs.TabItems.Add(tab);
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

            ElementCompositionPreview.SetAppWindowContent(newWindow, newPage);

            Tabs.TabItems.Remove(args.Tab);
            newPage.AddTabToTabs(args.Tab);

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

        private void Tabs_AddTabButtonClick(TabView sender, object args)
        {
            AddAnyTab();
            //sender.TabItems.Add(new TabViewItem()
            //{ IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Placeholder },
            //    Header = "New Item", Content = new ChatTab() { DataContext = "New Item" } });
        }
        static void RemoveTab(TabView view, TabViewItem tab)
        {
            var chatTab = tab.Content as ChatTab;
            Log("FreeTab1 " + chatTab.Name);
            Assert(chatTab.isActive);
            chatTab.isActive = false;
            tab.Content = null; // remove it
            view.TabItems.Remove(tab);
        }
        void RemoveTab(TabViewItem tab)
		{
            var chatTab = tab.Content as ChatTab;
            Log("Free tab " + chatTab.Name);

            Assert(chatTab.isActive);
            chatTab.isActive = false;
            tab.Content = null; // remove it
            Tabs.TabItems.Remove(tab);
		}
        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            RemoveTab(sender,args.Tab);
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count == 1)
            {
                var chat = (e.AddedItems[0] as TabViewItem).Content as ChatTab;
                var count = chat.Groups.Count;
                if (count > 0)
                {
                    chat.listView.ScrollIntoView(chat.Groups[count-1].Items.Last() );
                }

            }
        }
    }
}
