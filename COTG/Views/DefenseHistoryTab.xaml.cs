using COTG.Game;
using COTG.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Microsoft.UI.Xaml.Controls;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Grid;
using static COTG.Debug;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Telerik.Core.Data;
using Telerik.Data.Core;
using Telerik.Data;
using System.Collections.Specialized;
using Windows.Foundation;
using Microsoft.Toolkit.Uwp;
using Windows.UI.Xaml.Input;
using COTG.Services;
using System.Collections;
using COTG.JSON;
using Windows.UI.Input;
using Telerik.UI.Xaml.Controls.Input;
using COTG.Helpers;
using Windows.UI.Xaml.Navigation;
using static COTG.Game.Enum;
using System.Linq;
using System.Threading.Tasks;

namespace COTG.Views
{
 
    public sealed partial class DefenseHistoryTab : UserTab, INotifyPropertyChanged
    {
		public override  TabPage defaultPage => TabPage.secondaryTabs;
		public Army[] history { get; set; } = Army.empty;
        public void SetHistory( Army[] _history)
        {
            history = _history;
			var sel = IncomingTab.selected;
			if (sel == null)
				historyGrid.ItemsSource = history;
			else
			{
				var cid = sel.cid;
				historyGrid.ItemsSource = history.Where( s => s.targetCid == cid).ToArray() ;
			}
		}

        public static DefenseHistoryTab instance;
        public static RadDataGrid HistoryGrid => instance.historyGrid;
        //        public static Army showingRowDetails;

        //public DataTemplate GetTsInfoDataTemplate()
        //{
        //    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
        //    Assert(rv != null);
        //    return rv;
        //}
        public DefenseHistoryTab()
        {
            Assert(instance == null);
            instance = this;

            InitializeComponent();

            //            historyGrid.ContextFlyout = cityMenuFlyout;
        
        }
        override public Task VisibilityChanged(bool visible, bool longTerm)
		{
         //   historyGrid.ItemsSource = Array.Empty<Army>();
            if (visible)
            {
				if (!IncomingOverview.updateInProgress)
				{

					IncomingOverview.Process(SettingsPage.fetchFullHistory); // Todo: throttle
				}
			}
			return base.VisibilityChanged(visible, longTerm: longTerm);
        }





        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }
            storage = value;
            OnPropertyChanged(propertyName);
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public static bool IsVisible() => instance.isVisible;

    }
}
