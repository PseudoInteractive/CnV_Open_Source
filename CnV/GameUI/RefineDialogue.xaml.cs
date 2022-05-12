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
	public sealed partial class RefineDialogue:DialogG, INotifyPropertyChanged
	{
		public static RefineDialogue? instance;

		protected override string title => "Refine";
		public RefineDialogue()
        {
            this.InitializeComponent();
			instance=this;
        }

		public RefineItem[] items { get; set; }
		
		

		private void ResetItems() => items = new RefineItem[] { new(0),new(1),new(2),new(3) };
		public static void ShowInstance()
		{
			if(!City.GetBuild().canRefine)
			{
				AppS.MessageBox("A level 10 Sorc tower is needed to refine.");
				return;
			}
			var rv = instance ?? new RefineDialogue();
			rv.ResetItems();
			rv.OnPropertyChanged();
			rv.Show(false);
			
		}
		public static void ShowInstanceClick(object sender,RoutedEventArgs e) => ShowInstance();
		public void DoRefine(int id)
		{
			var res = new Resources();
			var city = City.GetBuild();
			if(!items[id].count.IsInRange(1,1_000_000)) {
				
				AppS.MessageBox("Value range is [1 .. 1m]");
				return;
			}
			res[id] = items[id].count;
			Assert(res.allPositive);
			var cost = res * -1000;
			if(!(city.SampleResources() + cost).allPositive)
			{
				AppS.MessageBox("Not enough res for refine");
			}
			else
			{

				new CnVEventRefine(city.c,res).EnqueueAsap();
				items[id].count = 0;
			}

			OnPropertyChanged();
			
		}

		private void WoodClick(object sender,RoutedEventArgs e) => DoRefine(0);
		private void StoneClick(object sender,RoutedEventArgs e) => DoRefine(1);
		private void IronClick(object sender,RoutedEventArgs e) => DoRefine(2);
private void FoodClick(object sender,RoutedEventArgs e) => DoRefine(3);

		
		public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged(string? member = null)
	{
		if (this.PropertyChanged is not null) 
			AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
	}
		public static void Changed(string? member = null)
		{
			if(instance is not null)
				instance.OnPropertyChanged(member);
		}

	}
	public sealed class RefineItem
	{
		public RefineItem(int id)
		{
			this.id = id;
			this.count = res/1000;
		}
		City city => City.GetBuild();
		public int id { get; set; }
		public int res => city.SampleResources()[id];
		public string resS => res.Format();
		public int count { get; set; }
	}

}
