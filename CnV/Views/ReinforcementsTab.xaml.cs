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


	public sealed partial class ReinforcementsTab : UserTab
	{
		
		public override TabPage                defaultPage => TabPage.secondaryTabs;
		public          string                 reinInTitle;
		public          string                 reinOutTitle;
		public          NotifyCollection<City> reinforcementsOut = new();
		public          NotifyCollection<City> reinforcementsIn  = new();
		public static   ReinforcementsTab      instance;
		public          int                    targetCid;

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
				await Update();

				//	BuildingsChanged(City.GetBuild(),false);
			}
			else
			{
				//await CityBuild._IsPlanner( false,false );

			}

			await base.VisibilityChanged(visible, longTerm: longTerm);
		}
		void SetupReinforcementGrid(SfDataGrid grid, bool isOutgoing)
		{
			using var _lock = SetupDataGrid(grid,true);
			grid.SourceType = typeof(City);
			
			grid.AddCity(isOutgoing ? "Defender" : "Target", wantTroops:isOutgoing,wantDefense:!isOutgoing);


			
			{
				SfDataGrid child0 = new() { AutoGenerateRelations = false, AutoGenerateColumns = false};
				using var _lock1 = SetupDataGrid(child0,true);
				child0.SourceType = typeof(Reinforcement);
				child0.AddHyperLink(nameof(Reinforcement.retUri), "Return");
				child0.AddTime(nameof(Reinforcement.dateTime), "Arrival", nullText: "Arrived");
				child0.AddText(nameof(Reinforcement._Troops), "Troops",
					widthMode: ColumnWidthMode.Star );
				{
					var details = new GridViewDefinition() { RelationalColumn=isOutgoing?nameof(City.reinforcementsOutProp) : nameof(City.reinforcementsInProp) , DataGrid=child0 } ;
					grid.DetailsViewDefinition.Add(details);
				}
				{
					SfDataGrid child1 = new() { AutoGenerateRelations = false, AutoGenerateColumns = false};
					using var _lock2 = SetupDataGrid(child1,true);
					child1.SourceType = typeof(City);
					child1.AddCity(!isOutgoing ? "Defender" : "Target",wantTroops:!isOutgoing,wantDefense:isOutgoing);
					var details1 = new GridViewDefinition()
					{
						RelationalColumn = isOutgoing
							? nameof(Reinforcement.targetCities)
							: nameof(Reinforcement.sourceCities) ,
						DataGrid = child1
					};
					child0.DetailsViewDefinition.Add(details1);

				}

			}
		}

		private bool hasLoaded;

		private  void ReinLoaded(object sender,RoutedEventArgs e)
		{
			if(!hasLoaded)
			{
				hasLoaded = true;
				
				SetupReinforcementGrid( reinIn,false);
				SetupReinforcementGrid(reinOut,true);
			}

		}

		public async Task Update()
		{
			reinforcementsIn.Clear();
			reinforcementsOut.Clear();
			var refreshTask = NotifyCollectionBase.ProcessAllCollectionChangesNow();
			var _cid = targetCid;
			var showAll = _cid == 0;
			using var work = new WorkScope("Checking reinforcements..");
			//await ReinforcementsOverview.instance.Post();
			await refreshTask;
			
			var _spot = _cid == 0 ? null : Spot.GetOrAdd(_cid);

			var tab = this;

			var spots = !showAll ? new[] { _spot } : City.myCities;

			//		var orders = new List<Reinforcement>();

			if (showAll)
			{
				tab.reinInTitle = "All Incoming Reinforcements";
				tab.reinOutTitle = "All Outgoing Reinforcements";
			}
			else
			{

				tab.reinInTitle = "Incoming Reinforcements";
				tab.reinOutTitle = "Outgoing Reinforcements";
			}

			//tab. panel.Children.Add(new TextBlock() { Text = showAll ? "All Incoming Reinforcements" : "Reinforcements Here:" });
			{
				tab.reinforcementsIn.Set(spots.Where(s => s.reinforcementsIn.AnyNullable()).OrderByDescending(s => s.reinforcementSortScore)
					.ToArray(), true, true,skipHashCheck:true);
			}
			{
				tab.reinforcementsOut.Set(
					spots.Where(s=>s.reinforcementsOut.AnyNullable()).
						SelectMany(s => s.reinforcementsOut).
						Select(s=>s.sourceCity).Distinct().
						OrderByDescending(s=>s.reinforcementSortScore).ToArray(), true, true,skipHashCheck:true);
			}
			

			tab.OnPropertyChanged();
			
			//var memStream = new MemoryStream();
			//reinIn.Serialize(memStream, new() { });
			//var data = new byte[memStream.Length];

			//memStream.Read(data, 0, (int)memStream.Length);
			//var str = Encoding.Default.GetString(data);
			//Log(str);


			//var file = await ADataGrid.Statics.folder.CreateFileAsync( (Tag as string) + ".xml", Windows.Storage.CreationCollisionOption.ReplaceExisting  );

			//reinIn.Serialize(file);

		}

		public static async void ShowReinforcements(int _cid, UIElement uie)
		{
			try
			{
				var tab = ReinforcementsTab.instance;
				tab.targetCid = _cid;
				await tab.Update();

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

				tab.ShowOrAdd(true, false);

			}
			catch(Exception __ex)
			{
				Debug.LogEx(__ex);
			}


		}
	}
}
