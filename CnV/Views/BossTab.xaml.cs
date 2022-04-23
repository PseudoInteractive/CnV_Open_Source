using System.Collections.Generic;
using Microsoft.UI.Xaml.Input;
using System.Threading.Tasks;

namespace CnV.Views
{
	using Game;
	using Syncfusion.UI.Xaml.Grids;

	public sealed partial class BossTab : UserTab
    {
        public static BossTab instance;
        public static bool IsVisible() => instance.isFocused;

		public static NotifyCollection<City> cities = new();

        public override Task VisibilityChanged(bool visible, bool longTerm)
		{
			AppS.DispatchOnUIThread(() =>
		   {
			   if (visible)
			   {
				   bossGrid.ItemsSource = null;
				   bossGrid.ItemsSource = Boss.all;
				   cities.Set( City.subCities.Where(c => c.testContinentAndTagFilter && c.homeTroopsAttack > 50 * 1000.0f).
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
	         InitializeComponent();
		}

		//private void gridPointerPress(object sender, PointerRoutedEventArgs e)
  //      {
			
		//	Spot.GridPressed(sender, e);

  //      }

      
        private void CityGrid_SelectionChanged(object sender, 
	        GridSelectionChangedEventArgs e)
        {
			if (!isFocused || !bossGrid.SelectedItems.Any())
				return;

            var it = bossGrid.SelectedItems.First();
            var newSel = it as City;
            Boss.distanceReference = newSel;
            var bosses = new List<Boss>();
            if (newSel != null && newSel.isCityOrCastle)
            {
				if (SpotTab.silenceSelectionChanges == 0)
					newSel.SelectInWorldView(false);
                var waterValid = false;
				var groundValid = false;
				foreach (var i in newSel.troopsHome)
				{
					if (i.GetAttack(Player.active) > 50 * 1000)
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
   
    
}
