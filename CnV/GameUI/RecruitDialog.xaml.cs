﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	public sealed partial class RecruitDialog:DialogG
	{
		protected override string title => "Recruit"; 
		internal static RecruitDialog? instance;
		internal City city;

		public RecruitDialog()
		{
			this.InitializeComponent();
			instance = this;
			
		}

		private void UpdateTroopItems()
		{
			troopItems = new RecruitTroopItem[Troops.ttCount];

			
			for(int i = 0;i<Troops.ttCount;++i)
				troopItems[i] = new() { city = city,type=(byte)i,count=1 };
		}
		public static void ShowInstance(City city)
		{
			var rv = instance ?? new RecruitDialog();
			rv.city = city;
			rv.UpdateTroopItems();
			rv.Show(true);
			
		}

		internal RecruitTroopItem [] troopItems;
	}


	internal class RecruitTroopItem: INotifyPropertyChanged
	{
		
		internal City city; // convienience
		internal TType type;
		internal uint count;
		internal TroopTypeCount tt => new(type,count);
		internal ImageSource image => Troops.Image(type);
		internal TroopInfo info => TroopInfo.all[type];
		internal bool isEnabled => city.CanRecruit(type);
		internal void Recruit(object sender,RoutedEventArgs e)
		{
			
			AppS.MessageBox("recruit",info.ToString());
			Note.Show("Recruit");
			new CnVEventRecruit(city.c,tt).Execute();
			count =0;
			OnPropertyChanged();
		}
		public string recruitTime => city.CanRecruit(type) ? tt.RecruitTimeRequired(city).Format() : string.Empty;
		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.DispatchOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}

	}
}
