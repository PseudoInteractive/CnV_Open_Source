using Microsoft.UI.Xaml;
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
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Artifacts:DialogG
	{
		public static Artifacts? instance;
		public Artifacts()
		{
			this.InitializeComponent();
			Assert(instance == null);
			instance =this;
			Canvas.SetLeft(this,220);
			
		}

		ObservableCollection<Artifact> items = new();

		int selectedTab=0;
		int SelectedTab
		{
			get => selectedTab;
			set {
				Log($"Sel Tab Changed {selectedTab} => {value}");
				if(value != selectedTab && value != -1)
				{
					selectedTab=value;
					UpdateItems();
				}

			}
		}

		int selectedTitle=-1;
		int SselectedTitle
		{
			get => selectedTitle;
			set {
				Log($"Sel title Changed {selectedTab} => {value}");
				if(value != selectedTitle && value != -1)
				{
					selectedTitle=value;
					UpdateItems();
				}

			}
		}

		async Task AddItemsOverTime<T>(IEnumerable<T> a) where T : Artifact
		{
			int count = a.Count();
			int countPer = count.DivideRoundUp(8);
			var iter = a.GetEnumerator();
			
			for(;;)
			{
				for(int j = 0;j<countPer;j++)
				{
					if(!iter.MoveNext())
					{
						return;
					}
					items.Add(iter.Current);
				}
				await Task.Delay(100);
			}
		}

		internal async Task UpdateItems()
		{
			try
			{
				var sel = selectedTitle+2;
				//			if(sel >= 1)
				//				++sel;
				// Do we need to call property changes on each artifact?
				//items.Clear();
				//await Task.Delay(100);
				Assert(items.IsNullOrEmpty());
				switch ( selectedTab)
				{
					case 0:
					relicsList.ItemsSource= Artifact.all.Where(a => a.level == sel && a.column == 1).ToArray();
						break;
					case 1:
						enhancementsList.ItemsSource = Artifact.all.Where(a => a.level == sel && (a.column == 2)).ToArray();
							break;
					case 2:
						specialList.ItemsSource = Artifact.all.Where(a => a.level == sel && a.column == 3).ToArray();
						break;
					case 3:
						overviewList.ItemsSource = Artifact.all.Where(a => a.owned > 0).ToArray();
						break;
				};
				//items.AddRange(i);
				
			}
			catch(Exception ex)
			{

				LogEx(ex);
			}
			
		}




		private void GetZirconiaClick(object sender,RoutedEventArgs e)
		{
			(new CnVEventPurchaseArtifacts() { artifact = (ushort)Artifact.ArtifactType.denari,count = 1 }).Execute();
		}



		internal static void ShowInstance()
		{
			var art = Artifacts.instance ?? new Artifacts();
			if(art.selectedTitle == -1)
			{
				art.selectedTitle = (Player.me.title.rank-2).Max(0);
				art.UpdateItems();
			}
			art.Show();

		}

		//protected override Task Opening()
		//{
		////	return UpdateItems();
		//}
		//protected override Task Closing()
		//{
		//	//items.Clear();
		//	//return Task.Delay(200);
		//}

		private void tabView_SelectionChanged(object sender,SelectionChangedEventArgs e)
		{
			Log($"Sel Changed added {e.AddedItems?.Format()} removed {e.RemovedItems?.Format()}" );
		}
		//private void OnViewChanging(object sender,ScrollViewerViewChangingEventArgs e)
		//{
		//	var zoom = e.FinalView.ZoomFactor.Min(1);
		//	Debounce.Q(runOnUIThread: true,debounceT: 50,action:() =>
		//	 {
		//		 scroll.MaxWidth = 592/zoom;
		//		 scroll.ChangeView(0,0,null);
		//	 });
		//}
	}
}
