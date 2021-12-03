using CnV.Services;
//using Windows.UI.Core;
using Microsoft.UI.Xaml;

using System.Collections.Generic;
using System.Runtime.CompilerServices;
//using Windows.UI.Input;
using System.Threading.Tasks;

using Telerik.UI.Xaml.Controls.Grid;

using static CnV.Game.City;

namespace CnV.Views
{
	using CnV;
	using Game;
	using Services;
	using Syncfusion.UI.Xaml.DataGrid;


	public sealed partial class BuildTab:UserTab
	{
		private const string workStr = "Refreshing build states..";
		public static BuildTab instance;


		//        public static City showingRowDetails;

		//{
		//    var rv = cityGrid.Resources["tsInfoDT"] as DataTemplate;
		//    Assert(rv != null);
		//    return rv;
		//}
		public static SfDataGrid CityGrid => instance.cityGrid;
		//  static MenuFlyout cityMenuFlyout;
		public BuildTab()
		{
			//            var a = Telerik.UI.Xaml.Controls.Grid.Primitives.For
			Assert(instance == null);
			instance = this;
			InitializeComponent();


			//			cityGrid.SelectionChanged += SpotSelectionChanged;
			//cityGrid.OnKey = Spot.OnKeyDown;

			//cityGrid.CurrentItemChanged += CityGrid_CurrentItemChanged;


			//			cityGrid.ProcessTooltips();
			//			spotGrids.Add(cityGrid);

		}




		//private void CityGrid_PointerPress(object sender, PointerRoutedEventArgs e)
		//{
		//	Spot.GridPressed(sender, e);
		//}
		//private void cityGrid_PointerExited(object sender, PointerRoutedEventArgs e)
		//{
		//	Spot.ProcessPointerExited();
		//	//if (string.Empty!=lastTip)
		//	//{
		//	//    lastTip = string.Empty;
		//	//    TabPage.mainTabs.tip.Text = string.Empty;
		//	//}
		//}








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
		//    instance.Dispatcher.(() =>
		//    {
		//        City.gridCitySource.NotifyReset();
		//        City.GetBuild()?.SelectInUI(true);
		//    });

		//}






		private void Set<T>(ref T storage,T value,[CallerMemberName] string propertyName = null)
		{
			if(Equals(storage,value))
			{
				return;
			}
			storage = value;
			OnPropertyChanged(propertyName);
		}



		// -1, refreshed once, not refreshing
		//  0 never refreshed
		//  1 refreshing
		//  2 is pending refresh
		//  build info requested => [-1,0] => 1
		//  ] => 2
		//  on refresh complete 2 => -1
		static int getBuildState;
		static public async Task GetBuildInfo()
		{
			// Refreshing
			if(getBuildState == 1 || getBuildState == 2)
			{
				getBuildState = 2;
				return;
			}
			var firstTime = false;

			if(getBuildState == 0)
			{
				firstTime = true;
				ShellPage.WorkStart(workStr);

			}
			for(;;)
			{
				getBuildState = 1;
				var js = await Post.SendForJson("overview/bcounc.php").ConfigureAwait(false);
				//var changes = new List<int>();
				//var getCities = new List<Task>();
				foreach(var ci in js.RootElement.EnumerateArray())
				{
					var cid = ci[0].GetAsInt();
					var city = City.GetOrAddCity(cid);
					var filter = city.testContinentAndTagFilter;
					if(!filter)
						continue;

					if(!city.buildingsLoaded)
					{
						await GetCity.Post(cid).ConfigureAwait(false);


						city.OnPropertyChanged();
					}
					city.UpdateBuildStage();

					city.points = (ushort)ci[2].GetAsInt();
					var isBuilding = (ci[4].GetAsFloat() != 0) || (city.buildStage == City.BuildStage.complete)||
					(city.buildStage == City.BuildStage.completeX)
								|| (city.buildStage == City.BuildStage.leave);
					//if (ci[3].GetAsFloat() != 0)
					//{
					//	//	Log($"3!: {city.nameAndRemarks}");
					//}

					//if(ci[5].GetAsFloat() != 0)
					//{
					//	//	Log($"5!: {city.nameAndRemarks}");
					//}
					if(isBuilding != city.isBuilding)
					{
						city.isBuilding = isBuilding;
						city.OnPropertyChanged();
					}

					city.wood = ci[8].GetAsInt();
					city.stone = ci[9].GetAsInt();
					city.bcBuildings = ci[6].GetAsInt() == 0;
					city.bcTowers = ci[7].GetAsInt() == 0;
					//	city.bcConvert = (ci[5].GetAsFloat() > 0);
					//	city.bcFill = ci[12].GetAsInt() > 0 && ci[13].GetAsInt() > 0;

					//city.b12 = ci[12].GetAsInt();
					//city.b13 = ci[13].GetAsInt();
					var _blocked = (ci[15].GetAsInt() == 1) &&
										 ((ci[14].GetAsInt() == 1 && ci[16].GetAsInt() == 1) ||
										  (ci[3].GetAsFloat() == 0 && ci[5].GetAsFloat() == 0));

					if(city.bcBlocked != _blocked)
					{
						city.bcBlocked = _blocked;
						city.OnPropertyChanged();
					}

				}

				if(firstTime==true)
				{
					firstTime = false;
					ShellPage.WorkEnd(workStr);

				}

				if(getBuildState != 2)
					break;
			}
			getBuildState = -1;
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


		override public async Task VisibilityChanged(bool visible,bool longTerm)
		{
			//   Log("Vis change" + visible);

			if(visible)
			{

				if(JSClient.ppdtInitialized)
				{
					//	await Raiding.UpdateTS(true, false);
					//	await RaidOverview.Send();
					if(City.build != 0)
						await GetCity.Post(City.build);


					//	City.gridCitySource.NotifyReset();

				}

				Task.Run(GetBuildInfo).ContinueWith(async (_) =>
			   {
				   foreach(var c in City.myCities)
				   {
					   if(c.testContinentAndTagFilter)
						   c.OnPropertyChanged();
				   }
				   City.gridCitySource.NotifyReset(true,true);
			   });
				//  if (cityGrid.ItemsSource == App.emptyCityList )
				//     cityGrid.ItemsSource = City.gridCitySource;
			}
			else
			{
				//        cityGrid.ItemsSource = null;
			}
			await base.VisibilityChanged(visible,longTerm: longTerm);
			//	if(visible)
			//	{
			//		AppS.DispatchOnUIThreadLow(() => Spot.SyncUISelection(true, City.GetBuild() ));
			//	}
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


		public static bool IsVisible() => instance.isFocused;

		private void SelectionChanged(object sender,DataGridSelectionChangedEventArgs e)
		{
			if(!isOpen)
				return;

			if(SpotTab.silenceSelectionChanges == 0)
			{
				try
				{

					var sel = cityGrid.SelectedItems;
					var newSel = new HashSet<int>();
					foreach(Spot s in sel)
					{
						newSel.Add(s.cid);

					}

					//          Spot.selected.EnterWriteLock();

					Spot.selected = newSel;
				}
				catch(Exception ex)
				{
					LogEx(ex);
				}
				finally
				{
					//          Spot.selected.ExitWriteLock();
				}
			}
		}

		private void SelectAll(object sender,RoutedEventArgs e)
		{
			cityGrid.SelectAll();

		}

		//      static Dungeon lastTooltip;
		//private void DungeonPointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
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

	//public class BuildCustomDataBindingCompleteCommand : DataGridCommand
	//{
	//	public BuildCustomDataBindingCompleteCommand()
	//	{
	//		this.Id = CommandId.DataBindingComplete;
	//	}

	//	public override bool CanExecute(object parameter)
	//	{
	//		var context = parameter as DataBindingCompleteEventArgs;
	//		// put your custom logic here
	//		return true;
	//	}

	//	static T TryGetValue<T>(object o) => o == null ? default : (T)o;
	//	public override void Execute(object parameter)
	//	{
	//		var context = parameter as DataBindingCompleteEventArgs;

	//	}


	//}
}
