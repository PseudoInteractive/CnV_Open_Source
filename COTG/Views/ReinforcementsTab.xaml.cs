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
		public NotifyCollection<Reinforcement> reinforcementsOut = new();
		public NotifyCollection<Reinforcement> reinforcementsIn = new();
		public static ReinforcementsTab instance;
		public int targetCid;

		public ReinforcementsTab()
		{
			instance = this;
			this.InitializeComponent();
			
		
		}

		public void ReturnClick(object obj)
		{
			var r = (obj as Reinforcement);
			Assert(r != null);
			Note.Show($"Returning {r.troopsString} from {r.targetCity} back to {r.sourceCity} ");
			r.ReturnAsync();
			reinforcementsIn.Remove(r, true);
			reinforcementsOut.Remove(r, true);
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

			var spots = !showAll ? new[] { _spot } : City.myCities.OrderBy(a => a.cid.ZCurveEncodeCid()).ToArray();

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
				var toAdd = new List<Reinforcement>();
				foreach (var s in spots.OrderByDescending(s => s.incomingFlags))
				{
					foreach (var reIn in s.reinforcementsIn.OrderBy(a => Player.IdToName(a.sourceCid.CidToPid())))
					{
						//var other = Spot.GetOrAdd(reIn.sourceCid);
						//var me = Spot.GetOrAdd(reIn.targetCid);
						//var content = showAll ? $"{other.xy} {other.playerName} {other.nameAndRemarks} {other.IncomingInfo() } -> {me.xy} {me.nameAndRemarks} {me.IncomingInfo()} {reIn.troops.Format(":",' ',',')}"
						//	: $"{other.xy} {other.playerName} {other.nameAndRemarks} {other.IncomingInfo() } {reIn.troops.Format(":",' ',',')}{reIn.time.FormatIfLaterThanNow()}";
						toAdd.Add(reIn);

					}
				}

				tab.reinforcementsIn.Set(toAdd, true, true);

			}
			{
				//			List<>
				var byFlags = spots.SelectMany(s => s.reinforcementsOut)
					.GroupBy(s => s.targetCid.AsCity().incomingFlags);
				var toAdd = new List<Reinforcement>();
				foreach (var flagGroup in byFlags.OrderByDescending(s => (int) s.Key))
				{
					foreach (var reIn in flagGroup.OrderByDescending(s => s.targetCid.AsCity().incomingFlags)
						         .ThenBy(s => Player.IdToName(s.targetCid.CidToPid())).ThenBy(a => a.time))
					{
						//	foreach(var reIn in cid)
						{
							//var other = Spot.GetOrAdd(reIn.targetCid);
							//var me = Spot.GetOrAdd(reIn.sourceCid);
							//var content = showAll ? $"{other.xy} {other.playerName} {other.nameAndRemarks} {other.IncomingInfo()} <- {me.xy} {me.nameAndRemarks} {me.IncomingInfo()} {reIn.troops.Format(":",' ',',')}"
							//	: $"{other.xy} {other.playerName} {other.nameAndRemarks} {other.IncomingInfo()} {reIn.troops.Format(":",' ',',')}{reIn.time.FormatIfLaterThanNow()}";

							toAdd.Add(reIn);
						}
					}
				}

				tab.reinforcementsOut.Set(toAdd, true, true);
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
				Note.Show($"Cell Tap {e.Column.HeaderText??"NA"}  {e.RowColumnIndex} {e.RowColumnIndex} {e.Record.ToString} ");
				switch(e.Column.HeaderText)
				{
					case "Ret":
						{
							var r = e.Record as Reinforcement;

							r.ReturnAsync();
							break;

						}
					case nameof(Reinforcement.sourceCity):
						{

							var r = (Reinforcement)e.Record;
							var s = r.sourceCity;
							s.DoClick();
							//						Spot.ProcessCoordClick(s.cid,true,)
							//					r.ReturnAsync();
							break;

						}
					case nameof(Reinforcement.targetCity):
						{

							var r = (Reinforcement)e.Record;
							var s = r.targetCity;
							s.DoClick();
							//						Spot.ProcessCoordClick(s.cid,true,)
							//					r.ReturnAsync();
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

					//			var args = Uri.Par
					Reinforcement.ReturnAsync(args.GetFirstValueByName("order").ParseLong().GetValueOrDefault(),args.GetFirstValueByName("pid").ParseInt().GetValueOrDefault());

				}


			}
		}

		private void CellToolTipOpening(object sender,Syncfusion.UI.Xaml.DataGrid.GridCellToolTipOpeningEventArgs e)
		{

		}

		private bool hasLoaded;


		private void OnLoaded(object sender,Microsoft.UI.Xaml.RoutedEventArgs e)
		{
			if(!hasLoaded)
			{
				hasLoaded = true;
				reinIn.AddCity(nameof(Reinforcement.sourceCity),"Source");
				reinIn.AddCity(nameof(Reinforcement.targetCity),"Target");
				reinOut.AddCity(nameof(Reinforcement.sourceCity),"Source");
				reinOut.AddCity(nameof(Reinforcement.targetCity),"Target");
			}

		}
	}
}
