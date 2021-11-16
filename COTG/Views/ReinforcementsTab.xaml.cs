using Microsoft.UI.Xaml.Controls;

using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace COTG.Views
{
	using System.IO;
	using System.Text;
	using System.Threading.Tasks;
	using Syncfusion.UI.Xaml.DataGrid;


	public sealed partial class ReinforcementsTab : UserTab
	{
		
		public override TabPage defaultPage => TabPage.secondaryTabs;
		public string reinInTitle;
		public string reinOutTitle;
		public NotifyCollection<City> reinforcementsOut = new();
		public NotifyCollection<City> reinforcementsIn = new();
		public static ReinforcementsTab instance;
		public int targetCid;

		public ReinforcementsTab()
		{
			instance = this;
			this.InitializeComponent();
			
		
		}

		

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

		public async Task Update()
		{
			var _cid = targetCid;
			var showAll = _cid == 0;
			using var work = new WorkScope("Checking reinforcements..");
			await Services.ReinforcementsOverview.instance.Post();
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
				tab.reinforcementsIn.Set(spots.Where(s => s.reinforcementsIn.Any()).OrderByDescending(s => s.reinforcementSortScore)
					.ToArray(), true, true,skipHashCheck:true);
			}
			{
				tab.reinforcementsOut.Set(
					spots.SelectMany(s => s.reinforcementsOut).
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

		private void reinIn_CellTapped(object sender,Syncfusion.UI.Xaml.DataGrid.GridCellTappedEventArgs e)
		{

			try
			{
			//	Note.Show($"Cell Tap {e.Column.HeaderText??"NA"}  {e.RowColumnIndex} {e.RowColumnIndex} {e.Record.ToString} ");
			if (e.Record is City city)
			{

				switch (e.Column.MappingName)
				{
					case nameof(City.cityName):
					case nameof(City.iconUri):
					case nameof(City.remarks):
						city.DoClick();
						break;
				}

				
				return;
			}
				switch(e.Column.MappingName)
				{
					case nameof(Reinforcement.retUri):
						{
							var r = e.Record as Reinforcement;
							Note.Show($"Returning {r.troopsString} from {r.targetCity} back to {r.sourceCity} ");
							r.ReturnAsync();
							AUtil.Remove(ref r.targetCity.reinforcementsIn,r);
							AUtil.Remove(ref r.sourceCity.reinforcementsOut,r);
							// Todo: refresh lists
							break;

						}
					
				}
			}
			catch(Exception exception)
			{
				Log(exception);
				throw;
			}



		}

		private const string returnReinforcement = nameof(returnReinforcement);

		private void CelNavigate(object sender,Syncfusion.UI.Xaml.Grids.CurrentCellRequestNavigateEventArgs e)
		{
			var uri = new Uri(e.NavigateText);


			if(uri.Scheme == ProtocolActivation.scheme && uri.LocalPath.StartsWith(ProtocolActivation.command))
			{
				var subStr = uri.LocalPath.AsSpan().Slice(ProtocolActivation.command.Length);
				if(subStr.StartsWith(returnReinforcement.AsSpan(),StringComparison.Ordinal))
				{
					e.Handled=true;
					var args = new WwwFormUrlDecoder(subStr.Slice(returnReinforcement.Length).ToString());

					Note.Show("ProtoClick");
					//			var args = Uri.Par
					//	Reinforcement.ReturnAsync(args.GetFirstValueByName("order").ParseLong().GetValueOrDefault(),args.GetFirstValueByName("pid").ParseInt().GetValueOrDefault());

				}


			}
		}

		private void CellToolTipOpening(object sender,Syncfusion.UI.Xaml.DataGrid.GridCellToolTipOpeningEventArgs e)
		{
			var tt = e.ToolTip;
			var rec = e.Record;
			int q = 0;
		}

		private bool hasLoaded;


		private void OnLoaded(object sender,Microsoft.UI.Xaml.RoutedEventArgs e)
		{
			if(!hasLoaded)
			{
				hasLoaded = true;
				SetupReinforcementGrid( reinIn,false);
				SetupReinforcementGrid(reinOut,true);
			}

		}

		private ADataGrid.ChangeContextDisposable SetupGrid(SfDataGrid grid)
		{
			var _lock0 = grid.ChangeContext();
			grid.ExpanderColumnWidth = 32;
			grid.GridContextFlyoutOpening += ContextFlyoutOpening;
			grid.RecordContextFlyout = new();
			grid.CurrentCellRequestNavigate += CelNavigate;
			grid.CellTapped += reinIn_CellTapped;
			grid.CellToolTipOpening += CellToolTipOpening;


			return _lock0;
		}

		private void SetupReinforcementGrid(SfDataGrid grid, bool isOutgoing)
		{
			using var _lock = SetupGrid(grid);
			grid.SourceType = typeof(City);
			
			grid.AddCity(isOutgoing ? "Defender" : "Target");


			
			
		
			{
				SfDataGrid child0 = new() { AutoGenerateRelations = false, AutoGenerateColumns = false};
				using var _lock1 = SetupGrid(child0);
				child0.SourceType = typeof(Reinforcement);
				child0.AddHyperLink(nameof(Reinforcement.retUri), "Return");
				child0.AddTime(nameof(Reinforcement.dateTime), "Arrival", nullText: "Arrived");
				child0.AddText(nameof(Reinforcement._Troops), "Troops",
					widthMode: Syncfusion.UI.Xaml.Grids.ColumnWidthMode.Star );
				var details = new GridViewDefinition() { RelationalColumn=isOutgoing?nameof(City.reinforcementsOutSorted) : nameof(City.reinforcementsInSorted) , DataGrid=child0 } ;
				grid.DetailsViewDefinition.Add( details );
				{
					SfDataGrid child1 = new() { AutoGenerateRelations = false, AutoGenerateColumns = false};
					using var _lock2 = SetupGrid(child1);
					child1.SourceType = typeof(City);
					child1.AddCity(!isOutgoing ? "Defender" : "Target");
					child0.DetailsViewDefinition.Add( 
						new GridViewDefinition() { RelationalColumn=isOutgoing?nameof(Reinforcement.targetCities) : nameof(Reinforcement.sourceCities) , DataGrid=child1 } 
					  );

				}

			}
		}

		private void ContextFlyoutOpening(object sender,GridContextFlyoutEventArgs e)
		{
			var flyout = e.ContextFlyout;
			flyout.Items.Clear();

			switch(e.ContextFlyoutType)
			{
				case ContextFlyoutType.RecordCell:
				{
					var info = e.ContextFlyoutInfo as GridRecordContextFlyoutInfo;
					var column = info.DataGrid.Columns[e.RowColumnIndex.ColumnIndex];
					if ( info.Record is City city)
					{
						city.AddToFlyout(flyout);
						
						break;
					}

					if ( info.Record is Reinforcement r)
					{
						flyout.AddItem("Return", () => r.ReturnAsync() );
						break;
					}
					break;
				}
			}
		}
	}
}
