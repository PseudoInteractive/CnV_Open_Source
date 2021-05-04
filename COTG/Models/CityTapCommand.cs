using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using COTG.Helpers;
using static COTG.Debug;
using COTG.Game;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using COTG.Services;
using COTG.Views;
using COTG.JSON;
using Telerik.Data.Core;

namespace COTG.Models
{
	public class CityTextFiler : IFilter
	{
		private string matchString;

		private string propertyName;

		public CityTextFiler(string match, string propterty)
		{
			this.matchString = match;
			this.propertyName = propterty;
		}

		public bool PassesFilter(object item)
		{
			var model = item as Spot;

			if (propertyName == null)
			{
				return false;
			}

			switch (propertyName)
			{
				case nameof(Spot.nameAndRemarks):
					return model.nameAndRemarks.Contains(this.matchString, StringComparison.OrdinalIgnoreCase);
				case nameof(Spot.xy):
					return model.xy.Contains(this.matchString, StringComparison.OrdinalIgnoreCase);
				case nameof(Spot.cont):
					return model.cont.ToString().Contains(this.matchString, StringComparison.OrdinalIgnoreCase);
			
				default:
					break;
			}

			return false;
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
	//    //                case "xy": JSClient.ShowCity(cid); break;
	//    //                case "icon": JSClient.ChangeCity(cid); break;
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

	public class DefenseTapCommand : DataGridCommand
    {
        public DefenseTapCommand()
        {
            this.Id = CommandId.CellTap;
            

        }

        public override bool CanExecute(object parameter)
        {
            var context = parameter as DataGridCellInfo;
            // put your custom logic here
           // Log("CanExecute");
            return true;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as DataGridCellInfo;
            var grid = Views.DefenseHistoryTab.HistoryGrid;
            // put your custom logic here
      //      Assert(MainPage.hoverTarget != null);
            //  var i = MainPage.hoverTarget;

            try
            {


                var i = context.Item as Army;
             
                //var isSelected =  grid.SelectedItem as Army == i;
                //if (isSelected)
                //    grid.DeselectItem(context);
                //else
                //    grid.SelectItem(context);

              //  Log(context.Item.GetType());
              //  Log(context.Item.ToString());
              //  Log(context.Value);

              //  Log(context.Column.Name);
              //  Log(base.CanExecute(parameter));
                //   grid.BeginEdit(context);
                if (context.Column.Header != null)
                {
                //    Log(context.Column.Header);
                    switch (context.Column.Header.ToString())
                    {
                        case nameof(i.Type): if (i.reportId != null)
                                JSClient.ShowReport(i.reportId);
                            else
                                Note.Show("This attack is in the future, there is no report yet");
                            break;
                        case "dXY": Spot.ProcessCoordClick(i.targetCid, false, App.keyModifiers,false); break;
                        case "aXY": Spot.ProcessCoordClick(i.sourceCid, false, App.keyModifiers,false); break;
                        case "aPlyr": JSClient.ShowPlayer(i.sPlayer); break;
                        case "dPlyr": JSClient.ShowPlayer(i.tPlayer); break;
                            //case nameof(i.atkCN): Spot.ProcessCoordClick(i.atkCid, false); break;
                            //case nameof(i.defC):
                            //case nameof(i.defCN): Spot.ProcessCoordClick(i.defCid,false); break;
                        case "atkAli": JSClient.ShowAlliance(i.sourceAllianceName);break;
                        case "defAli": JSClient.ShowAlliance(i.targetAllianceName); break;
                            //case nameof(i.aPlyr): JSClient.ShowPlayer(i.aPlyr);break;
                            //case nameof(i.dPlyr): JSClient.ShowPlayer(i.dPlyr); break;


                    }
                }
            }
            catch (Exception ex)
            {
                LogEx(ex);
            }


            //   grid.CommitEdit();

            //            if (base.CanExecute(parameter))
            Owner.CommandService.ExecuteDefaultCommand(Id, parameter);

        }

    }
    public class CityGridToggleColumnVisibilityCommand : DataGridCommand
    {
        public CityGridToggleColumnVisibilityCommand()
        {
            this.Id = CommandId.ToggleColumnVisibility;
        }

        public override bool CanExecute(object parameter)
        {

            return true;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as ToggleColumnVisibilityContext;
            if (context.IsColumnVisible)
            {
                switch (context.Column.Header?.ToString() )
                {
                    case nameof(City.raidReturn):
                    case nameof(City.raidCarry): RaidOverview.Send();break;
                    case "senny": City.UpdateSenatorInfo(); break;
                }
            }
            Owner.CommandService.ExecuteDefaultCommand(Id, parameter);
        }
    }
    //public class CityHeaderTapCommand : DataGridCommand
    //{
    //    public CityHeaderTapCommand()
    //    {
    //        this.Id = CommandId.ColumnHeaderTap;
    //    }

    //    public override bool CanExecute(object parameter)
    //    {
    //        var context = parameter as ColumnHeaderTapContext;
    //        // put your custom logic here
    //        return true;
    //    }

    //    public override void Execute(object parameter)
    //    {
    //        base.Execute(parameter);
    //        var context = parameter as ColumnHeaderTapContext;
    //        // put your custom logic here
    //        switch (context.Column.Header?.ToString())
    //        {
    //            case nameof(City.raidReturn):
    //            case nameof(City.raidCarry): RaidOverview.Send(); break;
    //            case "senny": City.UpdateSenatorInfo(); break;
    //            case nameof(City.tsTotal):
    //                if ( MainPage.IsVisible())
    //                {
    //                    Raiding.UpdateTS(true,true);
    //                }
    //                break;
    //            case nameof(City.tsRaid):
    //            case nameof(City.tsHome):
    //                if (  MainPage.IsVisible())
    //                {
    //                    Raiding.UpdateTS(true,true);
    //                }
    //                break;

    //        }

    //    }
    //}
    //    public class CityKeyCommand : DataGridCommand
    //    {
    //        public CityKeyCommand()
    //        {
    //            this.Id = CommandId.KeyDown;

    //        }

    //        public override bool CanExecute(object parameter)
    //        {
    //            var context = parameter as DataGridCellInfo;
    //            // put your custom logic here
    //        //    Log("CanExecute");
    //            return true;
    //        }

    //        public override void Execute(object parameter)
    //        {
    //            var keyEvent = parameter as KeyRoutedEventArgs;
    //            var grid = Views.MainPage.CityGrid;
    //            var sel = grid.SelectedItem as DataGridCellInfo;
    //            // put your custom logic here
    //            if (sel!=null)
    //            {
    //                switch (keyEvent.Key)
    //                {
    //                    case Windows.System.VirtualKey.Up:
    //                        {
    ////                            sel.

    //                            break;
    //                        }
    //                }
    //            }


    //            //            if (base.CanExecute(parameter))
    //            base.Execute(parameter);

    //        }
    //    }
    //public class CityInfoCommand : DataGridCommand
    //{
    //    public CityInfoCommand()
    //    {
    //        this.Id = CommandId.CellFlyoutAction;

    //    }

    //    public override bool CanExecute(object parameter)
    //    {
    //        var context = parameter as CellFlyoutActionContext;
    //        // put your custom logic here
    //        Log("CanExecute");
    //        return true;
    //    }

    //    public override void Execute(object parameter)
    //    {
    //        var context = parameter as CellFlyoutActionContext;
    //        // put your custom logic here
    //        var cellInfo = context.CellInfo;
    //        var i = cellInfo.Item as COTG.Game.City;
    //        var cid = i.cid;
    //        Log(i.GetType());
    //        Log(i.ToString());
    //        Log(cellInfo.Value);

    //        Log(cellInfo.Column.Name);
    //        Log(base.CanExecute(parameter));
    //        if (cellInfo.Column.Header != null)
    //        {
    //            Log(cellInfo.Column.Header);
    //            switch (cellInfo.Column.Header.ToString())
    //            {
    //                case "ts":
    //                    {
    //                        context.FlyoutTemplate = COTG.Views.MainPage.cache.GetTsInfoDataTemplate();
    //                        Assert(context.FlyoutTemplate != null);

    //                        //                           var t = new DataGridTextColumn() { PropertyName="ts"} );
    //                        //                            context.FlyoutTemplate.Ge;
    //                        break;

    //                    }
    //                    //   case "xy": JSClient.ShowCity(cid); break;
    //                    //   case "name": JSClient.ChangeCity(cid); break;

    //            }
    //        }

    //        // if (base.CanExecute(parameter))
    //        //     base.Execute(parameter);
    //        //
    //    }
    //}
    public class DungeonTapCommand : DataGridCommand
    {
        public DungeonTapCommand()
        {
            this.Id = CommandId.CellTap;

        }

        public override bool CanExecute(object parameter)
        {
            var context = parameter as DataGridCellInfo;
            // put your custom logic here
        //    Log("CanExecute");
            return true;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as DataGridCellInfo;
            // put your custom logic here
            var i = context.Item as COTG.Game.Dungeon;
            var cid = i.cid;
            if (context.Column.Header != null)
            {
            //    Log(context.Column.Header);
                switch (context.Column.Header.ToString())
                {
                    case nameof(Dungeon.xy):
                        Spot.ProcessCoordClick(cid,false, App.keyModifiers,false);
                        break;
					case nameof(Dungeon.dispatch):
					case nameof(Dungeon.plan):
                        Raiding.SendRaids(i);
						DungeonView.Close();
						return;
//                        break;

                }
            }

            Owner.CommandService.ExecuteDefaultCommand(Id, parameter);

        }
    }
}
