using COTG.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using static COTG.Debug;

namespace COTG.Views
{
    public sealed partial class BossTab : UserTab
    {
        public static BossTab instance;
        public static bool IsVisible() => instance.isVisible;



        public async override void VisibilityChanged(bool visible)
        {
            if (visible)
            {
                bossGrid.ItemsSource = null;
                bossGrid.ItemsSource = Boss.all;

            }
        }

        public BossTab()
        {
            Assert(instance == null);
            instance = this;
            this.InitializeComponent();
        }

        private void gridPointerPress(object sender, PointerRoutedEventArgs e)
        {
            Spot.ProcessPointerPress(sender, e);

        }

        private void gridPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Spot.ProcessPointerExited();

        }
        private void CityGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {

            var it = e.AddedItems.FirstOrDefault();
            var newSel = it as City;
            Boss.distanceReference = newSel;
            bossGrid.ItemsSource = Boss.all; // todo
        }
    }
        public class BossTapCommand : DataGridCommand
        {
            public BossTapCommand()
            {
                this.Id = CommandId.CellTap;
            }

            public override bool CanExecute(object parameter)
            {
                var context = parameter as DataGridCellInfo;
                return true;
            }

            public override void Execute(object parameter)
            {
                var context = parameter as DataGridCellInfo;
                var i = context.Item as Boss;
                Debug.Assert(i != null);
                var c = context.Column.Header?.ToString() ?? string.Empty;
                if (i != null)
                {
                    switch (c)
                    {

                        case nameof(i.xy):
                            JSClient.ShowCity(i.cid, false);
                            break;
                    }
                }
                if (base.CanExecute(parameter))
                    base.Execute(parameter);
            }
        }
    
}
