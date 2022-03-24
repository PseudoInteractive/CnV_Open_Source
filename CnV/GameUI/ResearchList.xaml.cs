﻿using System;
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
	

	public sealed partial class ResearchList:DialogG, INotifyPropertyChanged
	{
		public static ResearchList? instance;
		public ResearchList()
		{
			this.InitializeComponent();
			instance=this;
			
		}

		NotifyList<TechTreeHeadLine> questGroups = new(ResearchItems.v.TechTreeHeadLines.Where(a => a.n is not null));

	
		private TechTree quest => cvsGroups.View.CurrentItem as TechTree; 

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
			var art = instance ?? new ResearchList();
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

		private void Claim(object sender,RoutedEventArgs e)
		{
			//new CnVEventClaimQuest(City.GetBuild().c,(ushort)quest.id).EnqueueAsap();

			//var b = sender as Button;
			//if(b is not null)
			//	b.IsEnabled=false;
			//await Task.Delay(1000); // wait 1s for event to execut
			//foreach(var i in questGroups)
			//	i.OnPropertyChanged();
			//PropertyChanged?.Invoke(this,new(String.Empty));
			var t = quest;
			var s = t.step;
			Assert(t.canClaim);
			new CnVEventResearch(s.id).EnqueueAsap();
			Hide();
		}

		
	}
}