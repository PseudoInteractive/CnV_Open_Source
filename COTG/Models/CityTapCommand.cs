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

namespace COTG.Models
{
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
            Log("CanExecute");
            return true;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as DataGridCellInfo;
            var grid = Views.DefensePage.HistoryGrid;
            // put your custom logic here
      //      Assert(MainPage.hoverTarget != null);
            //  var i = MainPage.hoverTarget;

            try
            {


                var i = context.Item as COTG.JSON.Report;
             
                var isSelected = grid.SelectedItem == i;
                if (isSelected)
                    grid.DeselectItem(context);
                else
                    grid.SelectItem(context);

              //  Log(context.Item.GetType());
              //  Log(context.Item.ToString());
              //  Log(context.Value);

              //  Log(context.Column.Name);
              //  Log(base.CanExecute(parameter));
                //   grid.BeginEdit(context);
                if (context.Column.Header != null)
                {
                    Log(context.Column.Header);
                    switch (context.Column.Header.ToString())
                    {
                        case nameof(Report.Type): JSClient.ShowReport(i.reportId); break;
                        case nameof(Report.atkC):
                        case nameof(Report.atkCN): JSClient.ShowCity(i.atkCid); break;
                        case nameof(Report.defC):
                        case nameof(Report.defCN): JSClient.ShowCity(i.defCid); break;
                        case nameof(Report.atkAli): JSClient.ShowAlliance(i.atkAli);break;
                        case nameof(Report.defAli): JSClient.ShowAlliance(i.defAli); break;
                        case nameof(Report.aPlyr): JSClient.ShowPlayer(i.aPlyr);break;
                        case nameof(Report.dPlyr): JSClient.ShowPlayer(i.dPlyr); break;


                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }


            //   grid.CommitEdit();

            //            if (base.CanExecute(parameter))
            base.Execute(parameter);

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
                    case "raidRetyrn":
                    case "raidCarry": RaidOverview.Send();break;
                    case "senny": City.UpdateSenatorInfo(); break;
                }
            }
            context.Column.IsVisible = context.IsColumnVisible;
        }
    }
    public class CityKeyCommand : DataGridCommand
    {
        public CityKeyCommand()
        {
            this.Id = CommandId.KeyDown;

        }

        public override bool CanExecute(object parameter)
        {
            var context = parameter as DataGridCellInfo;
            // put your custom logic here
            Log("CanExecute");
            return true;
        }

        public override void Execute(object parameter)
        {
            var keyEvent = parameter as KeyRoutedEventArgs;
            var grid = Views.MainPage.CityGrid;
            var sel = grid.SelectedItem as DataGridCellInfo;
            // put your custom logic here
            if (sel!=null)
            {
                switch (keyEvent.Key)
                {
                    case Windows.System.VirtualKey.Up:
                        {
//                            sel.

                            break;
                        }
                }
            }
         
          
            //            if (base.CanExecute(parameter))
            base.Execute(parameter);

        }
    }
    //    public class CityInfoCommand : DataGridCommand
    //    {
    //        public CityInfoCommand()
    //        {
    //            this.Id = CommandId.CellFlyoutAction;

    //        }

    //        public override bool CanExecute(object parameter)
    //        {
    //            var context = parameter as CellFlyoutActionContext;
    //            // put your custom logic here
    //            Log("CanExecute");
    //            return true;
    //        }

    //        public override void Execute(object parameter)
    //        {
    //            var context = parameter as CellFlyoutActionContext;
    //            // put your custom logic here
    //            var cellInfo = context.CellInfo;
    //            var i = cellInfo.Item as COTG.Game.City;
    //            var cid = i.cid;
    //            Log(i.GetType());
    //            Log(i.ToString());
    //            Log(cellInfo.Value);

    //            Log(cellInfo.Column.Name);
    //            Log(base.CanExecute(parameter));
    //            if (cellInfo.Column.Header != null)
    //            {
    //                Log(cellInfo.Column.Header);
    //                switch (cellInfo.Column.Header.ToString())
    //                {
    //                    case "ts":
    //                        {
    //                            context.FlyoutTemplate = COTG.Views.MainPage.cache.GetTsInfoDataTemplate();
    //                            Assert(context.FlyoutTemplate != null);

    // //                           var t = new DataGridTextColumn() { PropertyName="ts"} );
    // //                            context.FlyoutTemplate.Ge;
    //                            break;

    //                        }
    //                 //   case "xy": JSClient.ShowCity(cid); break;
    //                 //   case "name": JSClient.ChangeCity(cid); break;

    //                }
    //            }

    //           // if (base.CanExecute(parameter))
    //           //     base.Execute(parameter);
    ////
    //        }
    //    }
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
            Log("CanExecute");
            return true;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as DataGridCellInfo;
            // put your custom logic here
            var i = context.Item as COTG.Game.Dungeon;
            var cid = i.cid;
            var processed = false;
            Log(context.Item.GetType());
            Log(context.Item.ToString());
            Log(context.Value);

            Log(context.Column.Name);
            Log(base.CanExecute(parameter));
            if (context.Column.Header != null)
            {
                Log(context.Column.Header);
                switch (context.Column.Header.ToString())
                {
                    case "xy":
                        processed = true;
                        JSClient.ShowCity(cid); break;
              
                }
            }
            if (!processed)
                Raiding.SendRaids(i);

            if (base.CanExecute(parameter))
                base.Execute(parameter);

        }
    }
}
