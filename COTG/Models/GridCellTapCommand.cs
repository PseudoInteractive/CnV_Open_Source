using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using COTG.Helpers;
using static COTG.Debug;

namespace COTG.Models
{
    public class GridCellTapCommand : DataGridCommand
    {
        public GridCellTapCommand()
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
            var i = context.Item as COTG.Game.City;
            var cid = i.cid;
            Log(context.Item.GetType());
            Log(context.Item.ToString());
            Log(context.Value);

            Log(context.Column.Name);
            Log(context.Column.Header);
            Log(base.CanExecute(parameter));
            if(context.Column.Header != null )
            switch (context.Column.Header.ToString())
			{
                case "xy": JSClient.ShowCity(cid); break;
                case "name": JSClient.ChangeCity(cid); break;
          
			};
 
            if (base.CanExecute(parameter))
                base.Execute(parameter);
		
        }
    }
}
