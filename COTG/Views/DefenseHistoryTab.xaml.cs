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

namespace COTG.Views
{
    public class ReportKillStyleSelector : Windows.UI.Xaml.Controls.StyleSelector
	{
        public Style attackerWinStyle { get; set; }

        public Style defenderWinStyle { get; set; }

        public Style tieStyle { get; set; }
        public Style noKillsStyle { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            var cell = (item as DataGridCellInfo);
            var report = cell.Item as Army;
            if (report.type == reportPending)
                return noKillsStyle;
            if(report.type == reportSieging)
                return report.claim > 0 ? attackerWinStyle : tieStyle;

            var dKill = report.dTsKill;
            var aKill = report.aTsKill;
            if (dKill < 1000 && aKill < 1000)
                return report.type == reportScout ? attackerWinStyle : noKillsStyle;
            if (dKill > aKill * 3 / 2)
                return defenderWinStyle;
            if (aKill > dKill * 3 / 2)
                return attackerWinStyle;
            return tieStyle;
        }

      

    }
    public class ReportTypeStyleSelector : Windows.UI.Xaml.Controls.StyleSelector
	{
        public Style pendingStyle { get; set; }

        public Style siegingStyle { get; set; }
        public Style siegeStyle { get; set; }

        public Style scoutStyle { get; set; }
        public Style assaultStyle { get; set; }
        public Style plunderStyle { get; set; }


        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            var cell = (item as DataGridCellInfo);
            var report = cell.Item as Army;
            switch (report.type)
			{
                case reportAssault: return assaultStyle;
                case reportSiege: return siegeStyle;
                case reportSieging: return siegingStyle;
                case reportPlunder: return plunderStyle;
                case reportPending: return pendingStyle;
                default: return scoutStyle;
            }

        }
    }
    public sealed partial class DefenseHistoryTab : UserTab, INotifyPropertyChanged
    {
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
        override public void VisibilityChanged(bool visible)
        {
            historyGrid.ItemsSource = Array.Empty<Army>();
            if (visible)
            {
                historyGrid.IsBusyIndicatorEnabled = true;

                IncomingOverview.Process(SettingsPage.fetchFullHistory, true); // Todo: throttle
            }
            base.VisibilityChanged(visible);


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
