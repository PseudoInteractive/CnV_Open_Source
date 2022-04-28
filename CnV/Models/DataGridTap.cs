using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CnV.Helpers;
using static CnV.Debug;
using CnV.Game;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using CnV.Services;
using CnV.Views;

using CnV;

namespace CnV;

	using Game;
	using Services;
	using Views;

	public static partial class ADataGrid

	{

		internal static void SfCellTapped(object sender, Syncfusion.UI.Xaml.DataGrid.GridCellTappedEventArgs e)
		{

			try
			{
				Assert(e.Column is not null);
				Assert(e.Column?.HeaderText is not null);
				if(e.Record is null)
				{
					Assert(false);
					return;
				}
				//	Note.Show($"Cell Tap {e.Column.HeaderText??"NA"}  {e.RowColumnIndex} {e.RowColumnIndex} {e.Record.ToString} ");
				switch (e.Record)
				{
					case Player player: {
						if(ChartDialog.ShowPlayerMetric(e.Column.MappingName,false))
							return;
						break;

					};
					case Alliance alliance: {
						if(ChartDialog.ShowPlayerMetric(e.Column.MappingName,true))
							return;
						break;

					};
					case City city:
					{
						city.CityRowClick(e);


						return;
					}
					
					case BossReport br:
					{
						switch(e.Column.MappingName)
						{
							case nameof(BossReport.bossC):
						
								Spot.ProcessCoordClick(br.bossC, false, AppS.keyModifiers, false);
								break;
							case nameof(BossReport.city):
								Spot.ProcessCoordClick(br.cityC, false, AppS.keyModifiers, false);
								break;

							case nameof(BossReport.Player):
								br.Player.Click();
								break;
							case nameof(BossReport.icon):
								BossReportDialog.ShowInstance(br);
								break;

						}
						return;
					}
					case  Army i:
					{
						if (i.ProcessTap(e.Column.HeaderText) == false)
						{
							switch (e.Column.HeaderText)
							{
								
								case nameof(Reinforcement.retUri):
							{
								Note.Show($"Returning {i}");
							//	i.ReturnAsync();
								
								//if (r.targetCity.reinforcementsIn is not null)
								//	r.targetCity.reinforcementsIn.Remove(r, true);
								//if (r.sourceCity.reinforcementsOut is not null)
								//	r.sourceCity.reinforcementsOut.Remove(r, true);
								// Todo: refresh lists
								break;

							}
								case "dXY":
									Spot.ProcessCoordClick(i.targetCid, false, AppS.keyModifiers, false);
									break;
								case "aXY":
									Spot.ProcessCoordClick(i.sourceCid, false, AppS.keyModifiers, false);
									break;
								case "aPlyr":
									PlayerHelper.Click(i.sPlayer);
									break;
								case "dPlyr":
									PlayerHelper.Click(i.tPlayer);
									break;
								//case nameof(i.atkCN): Spot.ProcessCoordClick(i.atkCid, false); break;
								//case nameof(i.defC):
								//case nameof(i.defCN): Spot.ProcessCoordClick(i.defCid,false); break;
								case "atkAli":
									Sim.ShowAlliance(i.sourceAllianceName);
									break;
								case "defAli":
									Sim.ShowAlliance(i.targetAllianceName);
									break;
									//case nameof(i.aPlyr): CnVServer.ShowPlayer(i.aPlyr);break;
									//case nameof(i.dPlyr): CnVServer.ShowPlayer(i.dPlyr); break;

									break;

							}
						}

						break;
					}
					case  BattleReport i:
					{
						if (i.ProcessTap(e.Column.MappingName) == false)
						{
							switch (e.Column.MappingName)
							{
								case nameof(i.type):
									
									Sim.ShowReport(i);
									
									break;
							
								case nameof(i.tXY):
								case nameof(i.targetCN):
									Spot.ProcessCoordClick(i.targetCid, false, AppS.keyModifiers, false);
									break;
								case nameof(i.sXY):
								case nameof(i.sourceCN):
									Spot.ProcessCoordClick(i.sourceCid, false, AppS.keyModifiers, false);
									break;
								case nameof(i.tPlayer):
									Sim.ShowPlayer(i.tPlayer);
									break;
								case nameof(i.sPlayer):
									Sim.ShowPlayer(i.sPlayer);
									break;
								//case nameof(i.atkCN): Spot.ProcessCoordClick(i.atkCid, false); break;
								//case nameof(i.defC):
								//case nameof(i.defCN): Spot.ProcessCoordClick(i.defCid,false); break;
								case nameof(i.sourceAllianceName):
									Sim.ShowAlliance(i.sourceAllianceName);
									break;
								case nameof(i.targetAllianceName):
									Sim.ShowAlliance(i.targetAllianceName);
									break;
									//case nameof(i.aPlyr): CnVServer.ShowPlayer(i.aPlyr);break;
									//case nameof(i.dPlyr): CnVServer.ShowPlayer(i.dPlyr); break;

									break;

							}
						}

						break;
					}

					case  Dungeon d:
					{
						var cid = d.target;
						//    Log(context.Column.Header);
						switch (e.Column.MappingName)
						{
							case nameof(Dungeon.xy):
								Spot.ProcessCoordClick(cid, false, AppS.keyModifiers, false);
								break;
							case nameof(Dungeon.dispatch):
							case nameof(Dungeon.plan):
								Raiding.SendRaids(d);
								DungeonView.Close();
								return;
							//                        break;

						}

						break;
					}
					case DonationOrder blessedCity:
					{
						blessedCity.ProcessTap(e.Column.MappingName);
						break;
					}
					case Supporter supporter:
					{
						supporter.ProcessedTapped(e.Column.MappingName);
						break;

					}
					case Boss boss:
					{
						switch(e.Column.HeaderText)
						{

							case nameof(boss.xy):
								Spot.ProcessCoordClick(boss.cid, false, AppS.keyModifiers);
								break;
						}

						break;
					}

				}
			}
			catch (Exception exception)
			{
				LogEx(exception);
			}



		}
	}


//public class CityTapCommand : DataGridCommand
	//{
	//    public CityTapCommand()
	//    {
	//        this.Id = CommandId.CellTap;

	//    }

	//    public override bool CanExecute(object parameter)
	//    {
	//        var context = parameter as DataGridCellInfo;
	//        // put your custom logic here
	//        Log("CanExecute");
	//        return true;
	//    }

	//    //public override void Execute(object parameter)
	//    //{
	//    //    var context = parameter as DataGridCellInfo;
	//    //    var grid = Views.MainPage.CityGrid;
	//    //    // put your custom logic here
	//    //    Assert(MainPage.hoverTarget != null);
	//    //  //  var i = MainPage.hoverTarget;

	//    //    try
	//    //    {


	//    //        var i = context.Item as COTG.Game.Spot;
	//    //        var cid = i.cid;

	//    //        var isSelected = grid.SelectedItem == i;
	//    //        if (isSelected)
	//    //            grid.DeselectCell(context);
	//    //        else
	//    //            grid.SelectCell(context);

	//    //        Log(context.Item.GetType());
	//    //        Log(context.Item.ToString());
	//    //        Log(context.Value);

	//    //        Log(context.Column.Name);
	//    //        Log(base.CanExecute(parameter));
	//    //        //   grid.BeginEdit(context);
	//    //        if (context.Column.Header != null)
	//    //        {
	//    //            Log(context.Column.Header);
	//    //            switch (context.Column.Header.ToString())
	//    //            {
	//    //                case "xy": CityUI.ShowCity(cid); break;
	//    //                case "icon": CnVServer.ChangeCity(cid); break;
	//    //                case "tsHome":
	//    //                    {
	//    //                        ScanDungeons.Post(cid); break;
	//    //                    }
	//    //            }
	//    //        }
	//    //    }
	//    //    catch (Exception ex)
	//    //    {
	//    //        Log(ex);
	//    //    }


	//    //    //   grid.CommitEdit();

	//    //    //            if (base.CanExecute(parameter))
	//    //    base.Execute(parameter);

	//    //}

	//}
	//public class CustomFilterButtonTapCommand : DataGridCommand
	//{
	//    public CustomFilterButtonTapCommand()
	//    {
	//        this.Id = CommandId.FilterButtonTap;
	//    }

	//    public override bool CanExecute(object parameter)
	//    {
	//        return true;
	//    }

	//    public override void Execute(object parameter)
	//    {
	//        var context = parameter as FilterButtonTapContext;
	//        if ( (context.Column.Header?. == "Name" )
	//        {
	//            context.FirstFilterControl = new TextFilterDescriptor() {  IsCaseSensitive=false,Operator=TextOperator.Contains,PropertyName= };
	//            {
	//                PropertyName = "EyeColor",
	//                DataContext = context.AssociatedDescriptor
	//            };

	//            context.SecondFilterControl = null;
	//        }
	//        this.Owner.CommandService.ExecuteDefaultCommand(CommandId.FilterButtonTap, context);
	//    }
	// }
	//



