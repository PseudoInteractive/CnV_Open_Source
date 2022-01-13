using System;

using CnV.Models;

using Microsoft.UI.Xaml;

namespace CnV.Services.DragAndDrop
{
	using Models;

	public class ListViewDropConfiguration : DropConfiguration
    {
        public static readonly DependencyProperty DragItemsStartingActionProperty =
            DependencyProperty.Register("DragItemsStartingAction", typeof(Action<DragDropStartingData>), typeof(DropConfiguration), new PropertyMetadata(null));

        public static readonly DependencyProperty DragItemsCompletedActionProperty =
            DependencyProperty.Register("DragItemsCompletedAction", typeof(Action<DragDropCompletedData>), typeof(DropConfiguration), new PropertyMetadata(null));

        public Action<DragDropStartingData> DragItemsStartingAction
        {
            get { return (Action<DragDropStartingData>)GetValue(DragItemsStartingActionProperty); }
            set { SetValue(DragItemsStartingActionProperty, value); }
        }

        public Action<DragDropCompletedData> DragItemsCompletedAction
        {
            get { return (Action<DragDropCompletedData>)GetValue(DragItemsCompletedActionProperty); }
            set { SetValue(DragItemsCompletedActionProperty, value); }
        }
    }
}
