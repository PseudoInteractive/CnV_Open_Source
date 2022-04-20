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

		NotifyList<QuestGroup> questGroups = new(QuestGroup.all.Where(a => a.id != 10 && a.n is not null).Prepend(QuestGroup.all[10]) ); // put tutorial first 

	
		private Quest quest => cvsGroups.View.CurrentItem as Quest; 

		public void InvalidateQuestGroups()
		{
	//		cvsGroups.Source  = questGroups;
			questGroups.OnReset();
			questGroups.OnItemsChanged();
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
			var questId = (ushort)quest.id;
			if (!AUtil.InterlockedSetAdd(ref Quests.unlockData.claimed,questId))
			{
				AppS.MessageBox("Already claimed 🙃");
				return;
			}


			new CnVEventClaimQuest(City.GetBuild().c,questId).EnqueueAsap();
			
			var b = sender as Button;
			if(b is not null)
				b.IsEnabled=false;
			await Task.Delay(1000); // wait 1s for event to execut
			foreach(var i in questGroups)
				i.OnPropertyChangedImmediate();
			PropertyChanged?.Invoke(this,new(String.Empty));
			
		}
	}
}
