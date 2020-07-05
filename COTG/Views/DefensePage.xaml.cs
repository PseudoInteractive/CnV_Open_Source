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

namespace COTG.Views
{

    
    public sealed partial class DefensePage : Page, INotifyPropertyChanged
    {
        public DumbCollection<Report> history { get; } = new DumbCollection<Report>();
        public DumbCollection<Dungeon> dungeons { get; } = new DumbCollection<Dungeon>();
        public static DefensePage cache;
        public static Report hoverTarget;
        public static string hoverTargetColumn;
        //        public static Report showingRowDetails;

        //public DataTemplate GetTsInfoDataTemplate()
        //{
        //    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
        //    Assert(rv != null);
        //    return rv;
        //}
        public DefensePage()
        {
            Assert(cache == null);
            cache = this;

            InitializeComponent();
           
//            historyGrid.ContextFlyout = cityMenuFlyout;


        }



        private void DataGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var grid = sender as RadDataGrid;
            var physicalPoint = e.GetCurrentPoint(grid);
            var point = new Point { X = physicalPoint.Position.X, Y = physicalPoint.Position.Y };
            var cell = grid.HitTestService.CellInfoFromPoint(point);
            var city = cell?.Item as Report;
            var cellName = cell?.Column.Header?.ToString();
            if (city != hoverTarget || hoverTargetColumn != cellName)
            {
                hoverTargetColumn = cellName;
                hoverTarget = city;
                if (city != null)
                {
                    //  Note.L($"{cellName} {city.cid.ToCoordinate()}");
                }

            }
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
