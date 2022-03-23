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
using System.Collections.Specialized;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	public class NotifyList<T>:List<T>, INotifyPropertyChanged, INotifyCollectionChanged
	{
		public NotifyList()
		{
		}

		public NotifyList(IEnumerable<T> collection) : base(collection)
		{
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		public event NotifyCollectionChangedEventHandler? CollectionChanged;

		public void NotifyReset()
		{
			PropertyChanged?.Invoke(this,new(String.Empty));
			CollectionChanged?.Invoke(this,new(NotifyCollectionChangedAction.Reset));
		}
	}

	public sealed partial class QuestList:DialogG, INotifyPropertyChanged
	{
		public static QuestList? instance;
		public QuestList()
		{
			this.InitializeComponent();
			instance=this;
			
		}

		NotifyList<QuestGroup> questGroups = new(QuestGroup.all.Where(a => a.n is not null));

	
		private Quest quest => cvsGroups.View.CurrentItem as Quest; 

		public void InvalidateQuestGroups()
		{
	//		cvsGroups.Source  = questGroups;
			questGroups.NotifyReset();
			foreach(var i in questGroups)
				i.OnPropertyChanged();
			PropertyChanged?.Invoke(this,new(String.Empty));
		}
		public event PropertyChangedEventHandler? PropertyChanged;

		internal static void ShowInstance()
		{
			Quests.UpdateUnlockData();
			var art = instance ?? new QuestList();
			art.InvalidateQuestGroups();
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

		private async void Claim(object sender,RoutedEventArgs e)
		{
			new CnVEventClaimQuest(City.GetBuild().c,(ushort)quest.id).EnqueueAsap();
			
			var b = sender as Button;
			if(b is not null)
				b.IsEnabled=false;
			await Task.Delay(1000); // wait 1s for event to execut
			foreach(var i in questGroups)
				i.OnPropertyChanged();
			PropertyChanged?.Invoke(this,new(String.Empty));
			
		}
	}
}
