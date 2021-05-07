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
using Windows.UI.Input;
using COTG.Helpers;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using System.Threading.Tasks;
using static COTG.Game.Enum;
using Microsoft.UI.Xaml.Controls;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using System.Threading;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using static COTG.Game.City;

namespace COTG.Views
{



	public sealed partial class BuildTab : UserTab, INotifyPropertyChanged
	{
		public static BuildTab instance;


		//        public static City showingRowDetails;

		//{
		//    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
		//    Assert(rv != null);
		//    return rv;
		//}
		public static RadDataGrid CityGrid => instance.cityGrid;
		//  static MenuFlyout cityMenuFlyout;
		public BuildTab()
		{
			//            var a = Telerik.UI.Xaml.Controls.Grid.Primitives.For
			Assert(instance == null);
			instance = this;
			InitializeComponent();
			cityGrid.SelectionChanged += SelectionChanged;
			cityGrid.OnKey = Spot.OnKeyDown;

			cityGrid.CurrentItemChanged += CityGrid_CurrentItemChanged;


			cityGrid.SelectionChanged += SelectionChanged;
			cityGrid.ProcessTooltips();
			spotGrids.Add(cityGrid);

		}

		private void CityGrid_CurrentItemChanged(object sender, EventArgs e)
		{
			           Log("Current item " + sender.ToString());
		}

		private void ColumnHeaderTap()
		{

		}


		
		private void CityGrid_PointerPress(object sender, PointerRoutedEventArgs e)
		{
			Spot.GridPressed(sender, e);
		}
		private void cityGrid_PointerExited(object sender, PointerRoutedEventArgs e)
		{
			Spot.ProcessPointerExited();
			//if (string.Empty!=lastTip)
			//{
			//    lastTip = string.Empty;
			//    TabPage.mainTabs.tip.Text = string.Empty;
			//}
		}


	





		//public string dungeonInfo { get
		//    {
		//        var ci = dungeonGrid.HitTestService.CellInfoFromPoint(CoreWindow.GetForCurrentThread().PointerPosition);
		//        if(ci == null)
		//            return "Missed";
		//        var i = ci.Item as COTG.Game.Dungeon;
		//        return i.ToString();
		//    }
		//}

		//private void OnCheckBoxClick(object sender, RoutedEventArgs e)
		//{
		//    var cb = (CheckBox)sender;
		//    var _showing = showingRowDetails;
		//    showingRowDetails = null;
		//    if (_showing != null && cities.Contains(_showing) )
		//        this.cityGrid.HideRowDetailsForItem(_showing);

		//    var newCheckedItem = (City)cb.DataContext;

		//    if (cb.IsChecked.HasValue && cb.IsChecked.Value)
		//    {
		//        showingRowDetails = newCheckedItem;
		//        this.cityGrid.ShowRowDetailsForItem(newCheckedItem);
		//    }

		//}




		//public static void CityListUpdateAll()
		//{
		//    if (instance == null)
		//        return;
		//    // Note.L("UpdateAll: ");
		//    instance.Dispatcher.DispatchOnUIThreadLow(() =>
		//    {
		//        City.gridCitySource.NotifyReset();
		//        City.GetBuild()?.SelectInUI(true);
		//    });

		//}

	
	


		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(storage, value))
			{
				return;
			}
			storage = value;
			OnPropertyChanged(propertyName);
		}




		static int getBuildState;
		static public async void GetBuildInfo()
		{
			if(getBuildState == 1)
			{
				getBuildState = 2;
				return;
			}
			if (getBuildState == 2)
				return;
			for (; ; )
			{
				getBuildState = 1;
				var js = await Post.SendForJson("overview/bcounc.php");
				foreach (var ci in js.RootElement.EnumerateArray())
				{

					var cid = ci[0].GetAsInt();
					var city = City.GetOrAddCity(cid);
					city.points = (ushort)ci[2].GetAsInt();
					var isBuilding = (ci[4].GetAsFloat()!=0 || ci[3].GetAsFloat() != 0) || (city.buildStage == BuildStage.complete) || (city.buildStage == BuildStage.leave);
					if(ci[5].GetAsFloat() != 0 )
					{
						isBuilding = true;
					}
					if (isBuilding != city.isBuilding)
					{
						city.isBuilding = isBuilding;
						App.DispatchOnUIThreadSneakyLow( ()=> city.OnPropertyChanged(nameof(city.isBuilding)));
					}

					city.wood = ci[8].GetAsInt();
					city.stone = ci[9].GetAsInt();
					if (!city.buildingsLoaded)
					{
						GetCity.Post(cid);
					}
				}
				if (getBuildState != 2)
					break;
			}
			getBuildState = 0;
					/*
0:		 17236203,  // cid
1:		"22+1001+-+Vanq",
2:		8808, // score
3:		0.00027777777777778,   ??
4:		0.5, // queue length (hours)
		0,
		1,  // has buildings
		0,  // has towers
		460411, // stone
		460549,
		1,
		1,
		1,
		1,
		1,
		0,
		0
				 */
			//	City.AllCityDataDirty();
		}


		override public async void VisibilityChanged(bool visible)
		{
			//   Log("Vis change" + visible);

			if (visible)
			{

				if (JSClient.ppdtInitialized)
				{
				//	await Raiding.UpdateTS(true, false);
				//	await RaidOverview.Send();
					if (City.build != 0)
						await GetCity.Post(City.build);

					App.DispatchOnUIThreadSneaky( () =>
					{
						City.gridCitySource.NotifyReset();
					} );
				}

				GetBuildInfo();
				//  if (cityGrid.ItemsSource == App.emptyCityList )
				//     cityGrid.ItemsSource = City.gridCitySource;
			}
			else
			{
				//        cityGrid.ItemsSource = null;
			}
			base.VisibilityChanged(visible);
			if(visible)
			{
				App.DispatchOnUIThreadSneaky(() => Spot.SyncUISelection(true, City.GetBuild() ));
			}
		}
		//private void BuildCityContextFlyout(TabPage newPage)
		//{
		//    if(newPage!=null)
		//        cityGrid.DataContext = newPage;
		//    cityContextFlyout = new MenuFlyout();
		//    cityContextFlyout.Items.Add(App.CreateMenuItem("Home Whenever", ReturnSlowClick));
		//    cityContextFlyout.Items.Add(App.CreateMenuItem("Home Please", ReturnFastClick));
		//    if (newPage != null)
		//        cityContextFlyout.XamlRoot = newPage.XamlRoot;
		//    cityGrid.ContextFlyout = cityContextFlyout;

		//}
		//public override void XamlTreeChanged(TabPage newPage) {
		//    //       cityGrid.ContextFlyout = null;
		//    if (newPage == null)
		//    {
		//        cityGrid.ContextFlyout = null;
		//        cityGrid.DataContext = null;
		//     //   cityContextFlyout.XamlRoot = null;
		//    }
		//    else
		//    {
		//        //   cityContextFlyout.XamlRoot = null;
		//        cityGrid.DataContext = newPage;
		//        cityContextFlyout = new MenuFlyout();
		//        cityContextFlyout.Items.Add(App.CreateMenuItem("Home Whenever", ReturnSlowClick));
		//        cityContextFlyout.Items.Add(App.CreateMenuItem("Home Please", ReturnFastClick));
		//        cityContextFlyout.XamlRoot = newPage.XamlRoot;
		//        cityGrid.ContextFlyout = cityContextFlyout;

		//    }
		//} // The tab was dragged somewhere else


		public static bool IsVisible() => instance.isVisible;

		private void SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
		{
			if (SpotTab.silenceSelectionChanges == 0)
			{
				try
				{

					var sel = cityGrid.SelectedItems;
					var newSel = new HashSet<int>();
					foreach (Spot s in sel)
					{
						newSel.Add(s.cid);

					}

					//          Spot.selected.EnterWriteLock();

					Spot.selected = newSel;
				}
				catch (Exception ex)
				{
					LogEx(ex);
				}
				finally
				{
					//          Spot.selected.ExitWriteLock();
				}
			}
		}

		private void SelectAll(object sender, RoutedEventArgs e)
		{
			cityGrid.SelectAll();

		}

		//      static Dungeon lastTooltip;
		//private void DungeonPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
		//{
		//          var physicalPoint = e.GetCurrentPoint(sender as RadDataGrid);
		//          var point = new Point { X = physicalPoint.Position.X, Y = physicalPoint.Position.Y };
		//          var row = (sender as RadDataGrid).HitTestService.RowItemFromPoint(point);
		//          var cell = (sender as RadDataGrid).HitTestService.CellInfoFromPoint(point);
		//          var hit = cell?.Item as Dungeon;
		//          if(hit!=lastTooltip)
		//	{
		//              lastTooltip = hit;
		//              if (hit != null)
		//                  ToolTipService.SetToolTip(tip, hit.ToString());
		//              else
		//                  ToolTipService.SetToolTip(tip,"None");
		//	}
		//      }
	}

	public class BuildCustomDataBindingCompleteCommand : DataGridCommand
	{
		public BuildCustomDataBindingCompleteCommand()
		{
			this.Id = CommandId.DataBindingComplete;
		}

		public override bool CanExecute(object parameter)
		{
			var context = parameter as DataBindingCompleteEventArgs;
			// put your custom logic here
			return true;
		}

		static T TryGetValue<T>(object o) => o == null ? default : (T)o;
		public override void Execute(object parameter)
		{
			var context = parameter as DataBindingCompleteEventArgs;

		}


	}
}
