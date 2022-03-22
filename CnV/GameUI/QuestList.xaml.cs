using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
    public sealed partial class QuestList : DialogG
    {
		public static QuestList? instance;
        public QuestList()
        {
            this.InitializeComponent();
			instance=this;
        }

		internal static void ShowInstance()
		{
			Quests.UpdateUnlockData();
			var art = instance ?? new QuestList();
			art.cvsGroups.Source=QuestGroup.all; 	
			art.questItems.ItemsSource = art.cvsGroups.View; // reset
			art.Show(true);
			

		}

		private void List_GotFocus(object sender,RoutedEventArgs e)
		{
            semanticZoom.StartBringIntoView();
		}
	}
}
