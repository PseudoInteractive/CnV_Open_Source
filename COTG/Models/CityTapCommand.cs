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

namespace COTG.Models
{
    public class CityTapCommand : DataGridCommand
    {
        public CityTapCommand()
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
            base.Execute(parameter);
            var context = parameter as DataGridCellInfo;
            var grid = Views.MainPage.CityGrid;
            // put your custom logic here
            var i = context.Item as COTG.Game.City;
            var cid = i.cid;
            
            var isSelected = grid.SelectedItem == context;
            if (isSelected)
                grid.DeselectCell(context);
            else
                grid.SelectCell(context);

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
                    case "xy": JSClient.ShowCity(cid); break;
                    case "icon": JSClient.ChangeCity(cid); break;
                    case "tsHome": new ScanDungeons(i).Post(); break;

                }
            }
 
//            if (base.CanExecute(parameter))
		
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
