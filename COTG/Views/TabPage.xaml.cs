using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using COTG.Models;

using Windows.UI.Xaml.Controls;

using static COTG.Debug;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace COTG.Views
{
    // For more info about the TabView Control see
    // https://docs.microsoft.com/uwp/api/microsoft.ui.xaml.controls.tabview?view=winui-2.2
    // For other samples, get the XAML Controls Gallery app http://aka.ms/XamlControlsGallery
    public sealed partial class TabPage : Page, INotifyPropertyChanged
    {
        public static ConcurrentQueue<TabPage> instances = new ConcurrentQueue<TabPage>(); // instance 0 is always inbedded in App, other instances are new windows

        public ObservableCollection<TabViewItemData> Tabs { get; } = new ObservableCollection<TabViewItemData>()
        {
            new TabViewItemData()
            {
                Index = 1,
                Header = "Item 1",
                //// In this sample the content shown in the Tab is a string, set the content to the model you want to show
                Content = "This is the content for Item 1."
            },
            new TabViewItemData()
            {
                Index = 2,
                Header = "Item 2",
                Content = "This is the content for Item 2."
            },
            new TabViewItemData()
            {
                Index = 3,
                Header = "Item 3",
                Content = "This is the content for Item 3."
            }
        };

        public TabPage()
        {
            InitializeComponent();
            instances.Enqueue(this);

        }

        private void OnAddTabButtonClick(Microsoft.UI.Xaml.Controls.TabView sender, object args)
        {
            int newIndex = Tabs.Any() ? Tabs.Max(t => t.Index) + 1 : 1;
            Tabs.Add(new TabViewItemData()
            {
                Index = newIndex,
                Header = $"Item {newIndex}",
                Content = $"This is the content for Item {newIndex}"
            });
        }

        private void OnTabCloseRequested(WinUI.TabView sender, WinUI.TabViewTabCloseRequestedEventArgs args)
        {
            if (args.Item is TabViewItemData item)
            {
                Tabs.Remove(item);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
