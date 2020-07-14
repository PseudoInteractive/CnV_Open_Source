using COTG.Game;
using COTG.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;
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

namespace COTG.Views
{
    public class ReportKillStyleSelector : StyleSelector
    {
        public Style attackerWinStyle { get; set; }

        public Style defenderWinStyle { get; set; }

        public Style tieStyle { get; set; }
        public Style noKillsStyle { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            var cell = (item as DataGridCellInfo);
            var report = cell.Item as Report;
            if (report.type == Report.typePending)
                return noKillsStyle;
            if(report.type == Report.typeSieging)
                return report.claim > 0 ? attackerWinStyle : tieStyle;

            var dKill = report.dTsKill;
            var aKill = report.aTsKill;
            if (dKill < 1000 && aKill < 1000)
                return report.type == Report.typeScout ? attackerWinStyle : noKillsStyle;
            if (dKill > aKill * 3 / 2)
                return defenderWinStyle;
            if (aKill > dKill * 3 / 2)
                return attackerWinStyle;
            return tieStyle;
        }
    }
    public class ReportTypeStyleSelector : StyleSelector
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
            var report = cell.Item as Report;
            switch (report.type)
			{
                case Report.typeAssault: return assaultStyle;
                case Report.typeSiege: return siegeStyle;
                case Report.typeSieging: return siegingStyle;
                case Report.typePlunder: return plunderStyle;
                default: return scoutStyle;
            }

        }
    }
    public sealed partial class DefensePage : Page, INotifyPropertyChanged
    {
        public DumbCollection<Report> history { get; } = new DumbCollection<Report>();
        public DumbCollection<Spot> defendersMRU { get; } = new DumbCollection<Spot>();

        public static DumbCollection<Spot> Defenders => instance.defendersMRU;


        public static DefensePage instance;
        public static RadDataGrid HistoryGrid => instance.historyGrid;
        //        public static Report showingRowDetails;

        //public DataTemplate GetTsInfoDataTemplate()
        //{
        //    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
        //    Assert(rv != null);
        //    return rv;
        //}
        public DefensePage()
        {
            Assert(instance == null);
            instance = this;

            InitializeComponent();

            //            historyGrid.ContextFlyout = cityMenuFlyout;
            defenderGrid.SelectionChanged += DefenderGrid_SelectionChanged;
        
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(!(e.Parameter is ShellPage))
                IncomingOverview.Process(true); // Todo: throttle
        }

        private void DefenderGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            foreach (var __item in e.RemovedItems)
            {
                var item = __item as Spot;
                 Spot.selected.Remove(item.cid);
                
            }
            foreach (var __item in e.AddedItems)
            {
                var item = __item as Spot;
                Spot.selected.Add(item.cid);
            }
        }

        private void gridPointerPress(object sender, PointerRoutedEventArgs e)
        {
            Spot.ProcessPointerPress(sender, e);
        }
        private void gridPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Spot.ProcessPointerMoved(sender, e);
        }
        private void gridPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Spot.ProcessPointerExited();
        }

        public static Spot TouchSpot(int cid)
        {
            var spot = Spot.GetOrAdd(cid);
            AddToGrid(spot);
            return spot;

        }
        public static void AddToGrid(Spot spot)
        {
            // Toggle Selected
          
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    var cid = spot.cid;
                    var def = Defenders;
                    if (!def.Contains(spot))
                    {
                        if (def.Count == 0 && spot is City)
                        {
                            // workaround:  First add cannot be a derived type.  It seems to assume that all contained types are the same as the first added on
                            var temp = new Spot();
                            def.AddAndNotify(temp);
                            def[0] = spot;
                            def.Replace(spot, temp, 0);

                        }
                        else
                        {
                            def.AddAndNotify(spot);
                        }
                        instance.defenderGrid.SelectItem(spot);
                    }
                    else
                    {
                        ToggleSelected(spot);

                    }
                });

        }
        public static void ToggleSelected(Spot rv)
        {
            var isSelected = rv.ToggleSelected();
            if (isSelected)
                instance.defenderGrid.SelectItem(rv);
            else
                instance.defenderGrid.DeselectItem(rv);

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

      
    }
}
