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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using static COTG.Game.Troops;
using static COTG.Debug;
using System.Threading.Tasks;

namespace COTG.Views
{
    public sealed partial class BossTab : UserTab
    {
        public static BossTab instance;
        public static bool IsVisible() => instance.isFocused;

		public static NotifyCollection<City> cities = new();

        public override Task VisibilityChanged(bool visible, bool longTerm)
		{
			App.DispatchOnUIThreadLow(() =>
		   {
			   if (visible)
			   {
				   bossGrid.ItemsSource = null;
				   bossGrid.ItemsSource = Boss.all;
				   cities.Set( City.myCities.Where(c => c.testContinentAndTagFilter && c.homeTroopsAttack > 50 * 1000.0f).
					   OrderBy((c) => -c.homeTroopsAttack) );
				   cities.NotifyReset();
			   }
			   else
			   {
				   bossGrid.ItemsSource = null;

			   }
		   });
			return base.VisibilityChanged(visible, longTerm: longTerm);
        }

        public BossTab()
        {
            Assert(instance == null);
            instance = this;
            this.InitializeComponent();

			spotGrids.Add(cityGrid);

		}

		private void gridPointerPress(object sender, PointerRoutedEventArgs e)
        {
			
			Spot.GridPressed(sender, e);

        }

        private void gridPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Spot.ProcessPointerExited();

        }
        private void CityGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
			if (!isOpen)
				return;

            var it = e.AddedItems.FirstOrDefault();
            var newSel = it as City;
            Boss.distanceReference = newSel;
            var bosses = new List<Boss>();
            if (newSel != null && newSel.isCityOrCastle)
            {
				if (SpotTab.silenceSelectionChanges == 0)
					newSel.SelectInWorldView(false);
                var waterValid = false;
				var groundValid = false;
				foreach (var i in newSel.troopsHome.Enumerate())
				{
					if (i.attack > 50 * 1000)
					{
						if (i.isNaval)
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
                bosses.SortSmall((a, b) => a.dist.CompareTo(b.dist));
				bossGrid.ItemsSource = bosses; // todo
			}

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
