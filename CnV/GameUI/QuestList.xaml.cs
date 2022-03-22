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
	public sealed partial class QuestList:DialogG, INotifyPropertyChanged
	{
		public static QuestList? instance;
		public QuestList()
		{
			this.InitializeComponent();
			instance=this;
			
		}
		private QuestGroup[] questGroups => QuestGroup.all.Where(a=>a.n is not null).ToArray();
		private Quest quest => cvsGroups.View.CurrentItem as Quest; 

		public event PropertyChangedEventHandler? PropertyChanged;

		internal static void ShowInstance()
		{
			Quests.UpdateUnlockData();
			var art = instance ?? new QuestList();
			//art.questItems.ItemsSource = art.cvsGroups.View; // reset
			art.Show(true);
			

		}

		private  void View_CurrentChanged(object? sender,object e)
		{
			PropertyChanged.Invoke(this,new(String.Empty));
		}

		private void List_GotFocus(object sender,RoutedEventArgs e)
		{
            semanticZoom.StartBringIntoView();
		}


		bool hasLoaded;
		private void OnLoaded(object sender,RoutedEventArgs e)
		{
			if(!hasLoaded)
			{
				hasLoaded=true;
				cvsGroups.View.CurrentChanged+=View_CurrentChanged;
			}
		}
	}
}
