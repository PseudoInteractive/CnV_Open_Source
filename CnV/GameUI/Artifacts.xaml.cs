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
	public sealed partial class Artifacts: DialogG
	{
		public static Artifacts? instance;
		public Artifacts()
		{
			this.InitializeComponent();
			Assert(instance == null);
			instance =this;
			Canvas.SetLeft(this,220);
			titleComboBox.SelectedIndex = ( Player.me.title.rank-1).Max(0);
		}

		ObservableCollection<Artifact> items  = new();

		int selectedTab;
		int SelectedTab
		{
			get => selectedTab;
			set {
				if(value != selectedTab)
				{
					selectedTab=value;
					UpdateItems();
				}

			}
		}

		int selectedTitle;
		int SselectedTitle
		{
			get => selectedTitle;
			set {
				if(value != selectedTitle)
				{
					selectedTitle=value;
					UpdateItems();
				}

			}
		}

		

		internal void UpdateItems()
		{
			var sel = selectedTitle+2;
			//			if(sel >= 1)
			//				++sel;
			// Do we need to call property changes on each artifact?
			items.Clear();
			switch(selectedTab)
			{
				case 0:
					items.AddRange(Artifact.all.Where(a => a.level == sel && a.column == 1));
					break;

				case 1:
					items.AddRange( Artifact.all.Where(a => a.level == sel && (a.column == 2)));
					break;
				case 2:
					items.AddRange(Artifact.all.Where(a => a.level == sel && a.column == 3));

					break;
				case 3:
					items.AddRange( Artifact.all.Where(a => a.owned > 0));
					break;
			}
		}




		private void GetZirconiaClick(object sender,RoutedEventArgs e)
		{
			(new CnVEventPurchaseArtifacts() {  artifact = (ushort)Artifact.ArtifactType.denari, count = 1}).Execute();
		}

		

		internal static void ShowInstance()
		{
			var art = Artifacts.instance ?? new Artifacts();
			art.items.Clear();
			art.Show();
			art.UpdateItems();
			
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
