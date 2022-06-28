using Microsoft.UI.Xaml.Controls;

using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV.Views
{
	using System.Collections;
	using System.IO;
	using System.Text;
	using System.Threading.Tasks;
	using Game;
	using Microsoft.UI.Xaml;
	using Services;
	using Syncfusion.UI.Xaml.DataGrid;
	using Syncfusion.UI.Xaml.Grids;
using static CnV.Alliance;

	public sealed partial class ReinforcementsTab : UserTab
	{
		public static bool IsVisible() =>  instance is not null && instance.isFocused;
		
		public override TabPage                defaultPage => TabPage.secondaryTabs;
		public          string                 reinInTitle = string.Empty;
//		public          NotifyCollection<City> citiesOut = new();
		public          ObservableCollection<City> cities  = new();
		public static   ReinforcementsTab?      instance;
		internal bool wantIncoming;
		Alliance.CityFilter filter = Alliance.CityFilter.allied;
		internal int _filter {
			get => (int)filter;
			set { if(filter != (Alliance.CityFilter)value) {
					filter = (Alliance.CityFilter)value;
					AppS.QueueOnUIThread(Update);
				}
			}
		}
		public ReinforcementsTab()
		{
			instance = this;
			this.InitializeComponent();
			
		
		}

		//public override IEnumerable<SfDataGrid> GetDataGrids()
		//{
		//	yield return reinIn;
		//	yield return reinOut;
		//}


		public override async Task VisibilityChanged(bool visible, bool longTerm)
		{
			if (visible)
			{
				//	await CityBuild._IsPlanner(true,false);
				AppS.QueueOnUIThread(Update);

				//	BuildingsChanged(City.GetBuild(),false);
			}
			else
			{
				//await CityBuild._IsPlanner( false,false );

			}

			await base.VisibilityChanged(visible, longTerm: longTerm);
		}
		//void SetupReinforcementGrid(SfDataGrid grid, bool isOutgoing)
		//{
		//	using var _lock = SetupDataGrid(grid,true);
		//	grid.SourceType = typeof(City);
			
		//	grid.AddCity(isOutgoing ? "Defender" : "Target", wantTroops:isOutgoing,wantDefense:!isOutgoing);


			
		//	{
		//		SfDataGrid child0 = new() { AutoGenerateRelations = false, AutoGenerateColumns = false};
		//		using var _lock1 = SetupDataGrid(child0,true);
		//		child0.SourceType = typeof(Reinforcement);
		//		child0.AddHyperLink(nameof(Reinforcement.retUri), "Return");
		//		child0.AddTime(nameof(Reinforcement.dateTime), "Arrival", nullText: "Arrived");
		//		child0.AddText(nameof(Reinforcement._Troops), "Troops",
		//			widthMode: ColumnWidthMode.Star );
		//		{
		//			var details = new GridViewDefinition() { RelationalColumn=isOutgoing?nameof(City.reinforcementsOut) : nameof(City.reinforcementsIn) , DataGrid=child0 } ;
		//			grid.DetailsViewDefinition.Add(details);
		//		}
		//		{
		//			SfDataGrid child1 = new() { AutoGenerateRelations = false, AutoGenerateColumns = false};
		//			using var _lock2 = SetupDataGrid(child1,true);
		//			child1.SourceType = typeof(City);
		//			child1.AddCity(!isOutgoing ? "Defender" : "Target",wantTroops:!isOutgoing,wantDefense:isOutgoing);
		//			var details1 = new GridViewDefinition()
		//			{
		//				RelationalColumn = isOutgoing
		//					? nameof(Reinforcement.targetCities)
		//					: nameof(Reinforcement.sourceCities) ,
		//				DataGrid = child1
		//			};
		//			child0.DetailsViewDefinition.Add(details1);

		//		}

		//	}
		//}

		private bool hasLoaded;

		private  void ReinLoaded(object sender,RoutedEventArgs e)
		{
			if(!hasLoaded)
			{
				hasLoaded = true;
				SetupDataGrid(reinIn);
				SetupDataGrid(armiesIn);
				//SetupReinforcementGrid( reinIn,false);
			//	SetupReinforcementGrid(reinOut,true);
			}

		}

		public void Update()
		{
			bool invalidate = true;
			//			citiesOut.Clear();
			//	var refreshTask = NotifyCollectionBase.ProcessAllCollectionChangesNow();
			var spot = City.invalid;// cityFilter.city;
		
			//await ReinforcementsOverview.instance.Post();
//			await refreshTask;
			
			var tab = this;

			var spots = spot.isValid? new[] {spot} : filter.GetCityList();
;

			//		var orders = new List<Reinforcement>();

				tab.reinInTitle = spot.isValid ? $"{spot} reinforcements" : "Reinforcements";
	
			//tab. panel.Children.Add(new TextBlock() { Text = showAll ? "All Incoming Reinforcements" : "Reinforcements Here:" });
			{
				spots = spots.Where(c => c.reinforcementsMap.Any()).ToArray();
				if(invalidate) {
					tab.cities.Clear();
					foreach(var s in spots) {
						s.OnPropertyChanged();
					}
				}
				//var targets = spot.isValid ? spots : (wantIncoming.IsOn ?
				//	spots.SelectMany(c => c.reinforcementsIn).Select(s => s.sourceCity) : 
				//	spots.SelectMany(c => c.reinforcementsOut).Select(s => s.sourceCity))   .Distinct();
				spots.SyncList(tab.cities); //.SyncList .Set(spots.Where(s => s.reinforcementsIn.AnyNullable()).Concat(targets).Distinct().ToArray().OrderByDescending(s => s.reinforcementSortScore)
				//	.ToArray(), true, true,skipHashCheck:true);
			}
			//{
			//	tab.citiesOut.Set(
			//		spots.Where(s=>s.reinforcementsOut.AnyNullable()).
			//			SelectMany(s => s.reinforcementsOut).
			//			Select(s=>s.sourceCity).Distinct().
			//			OrderByDescending(s=>s.reinforcementSortScore).ToArray(), true, true,skipHashCheck:true);
			//}
			

		//	tab.OnPropertyChanged();
			
			//var memStream = new MemoryStream();
			//reinIn.Serialize(memStream, new() { });
			//var data = new byte[memStream.Length];

			//memStream.Read(data, 0, (int)memStream.Length);
			//var str = Encoding.Default.GetString(data);
			//Log(str);


			//var file = await ADataGrid.Statics.folder.CreateFileAsync( (Tag as string) + ".xml", Windows.Storage.CreationCollisionOption.ReplaceExisting  );

			//reinIn.Serialize(file);

		}

		public static async  void ShowReinforcements(int _cid, UIElement uie)
		{
			try
			{
				await ShowOrAdd<ReinforcementsTab>();
				var tab = ReinforcementsTab.instance;
				//	tab.cityFilter.SetCity(_cid.AsCity() );
				tab.Update();
				
				//}
				//	var result = await msg.ShowAsync2(uie);
				//	if(result == ContentDialogResult.Primary)
				//	{
				//		ShellPage.WorkStart("Return..");
				//		int counter = 0;
				//		foreach(var check in panel.Children)
				//		{
				//			if(!(check is CheckBox c))
				//				continue;
				//			if(c.IsChecked.GetValueOrDefault())
				//			{
				//				await Return(orders[counter]);
				//			}

				//			++counter;
				//			ShellPage.WorkUpdate($"Return.. {counter}");
				//		}
				//		if(counter > 0)
				//		{
				//			await Task.Delay(400);
				//			await Services.ReinforcementsOverview.instance.Post();
				//		}


				//	}


			}
			catch(Exception __ex)
			{
				Debug.LogEx(__ex);
			}


		}

		public override async Task Closed()
		{ 
			await base.Closed();
			instance = null;
		}
   //     private void DetailsViewExpanding(object sender,GridDetailsViewExpandingEventArgs e) {
			//var city = e.Record as City;
			// e.DetailsViewItemsSource.Clear();
			// e.DetailsViewItemsSource.Add( "reinforcementsMap", 	new ObservableCollection<Army>(instance.wantIncoming.IsOn ? city.reinforcementsIn : city.reinforcementsOut ));
		
   //     }


		private void ToggleSwitch_Toggled(object sender,RoutedEventArgs e) {
			// invalidate all cities

			AppS.QueueOnUIThread(Update);
		}

		//private void reinIn_DetailsViewCollapsing(object sender,GridDetailsViewCollapsingEventArgs e) {

		//}
	}
}
