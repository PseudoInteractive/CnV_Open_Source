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
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static COTG.Game.Enum;
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
                cityGrid.ItemsSource = City.myCities.Where(c => c.homeTroopsAttack > 50 * 1000.0f).
                    OrderBy((c) => -c.homeTroopsAttack). ToArray();
            }
            else
            {
                bossGrid.ItemsSource = null;
                cityGrid.ItemsSource = City.emptyCitySource;

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
			
			Spot.ProcessPointerPress(this,sender, e);

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
            var bosses = new List<Boss>();
            if (newSel != null)
            {
                newSel.SelectInWorldView(false);
                var waterValid = false;
				var groundValid = false;
				foreach (var i in newSel.troopsHome)
				{
					if (i.attack > 50 * 1000)
					{
						if (i.isWaterRaider)
							waterValid = true;
						else
							groundValid = true;
					}
				}
				if (waterValid)
				{
					bosses.AddRange(Boss.all);
				}
				else
				{
					var cont = newSel.cont;
					foreach (var b in Boss.all)
					{
						if (b.cont == cont)
							bosses.Add(b);
					}
				}
                bosses.Sort((a, b) => a.dist.CompareTo(b.dist));
            }
            bossGrid.ItemsSource = bosses; // todo

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
                            Spot.ProcessCoordClick(i.cid, false, App.keyModifiers);
                            break;
                    }
                }
            Owner.CommandService.ExecuteDefaultCommand(Id, parameter);
        }
        }
    
}
