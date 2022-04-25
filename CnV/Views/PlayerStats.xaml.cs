using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	public sealed partial class PlayerStats:UserControl,INotifyPropertyChanged
	{
		public static PlayerStats instance;
		public PlayerStats()
		{
			instance = this;
			this.InitializeComponent();
			Player.active.PropertyChanged += PlayerChanged;
			City.buildCityChanged += (oldCity,newCity) =>
				{
					oldCity.player.PropertyChanged-= PlayerChanged;
					newCity.player.PropertyChanged+= PlayerChanged;
					Changed();
				};
		}

		private void PlayerChanged(object? sender,PropertyChangedEventArgs e)
		{
			Changed();

		}

		public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged(string? member = null)
	{
		if (this.PropertyChanged is not null) 
			AppS.QueueOnUIThread(
				() => 
				PropertyChanged?.Invoke(this,new(member)));
	}
		public static void Changed(string? member = null)
		{
			if(Sim.isPastWarmup)
			{
				if(instance is not null)
					instance.OnPropertyChanged(member);
			}
		}

		public static Player player => Player.active;
		public string zirconiaS => player.zirconia.Format();
		public string manaS => player.sampleMana.Format();
		public string goldS => player.SampleGold().Format();
		public string RefineS(int id) => player.data.refines[id].Format();
		internal static void GoldChanged()	=> Changed(nameof(goldS));
		internal static void ManaChanged()	=> Changed(nameof(manaS));

		internal static void ZirconiaChanged() => Changed(nameof(zirconiaS));
		internal static void RefinesChanged() => Changed(nameof(RefineS));

		public string title => player.Title;

		private void AllTapped(object sender,TappedRoutedEventArgs e) =>
			ResContextRequest(sender as UIElement,null);

		private void WoodTapped(object sender,TappedRoutedEventArgs e)=> CityUI.Show( Artifact.ArtifactType.saw, sender);
		private void StoneTapped(object sender,TappedRoutedEventArgs e)=> CityUI.Show( Artifact.ArtifactType.chisel, sender);
		private void IronTapped(object sender,TappedRoutedEventArgs e)=> CityUI.Show( Artifact.ArtifactType.crucible, sender);
		private void FoodTapped(object sender,TappedRoutedEventArgs e)=> CityUI.Show( Artifact.ArtifactType.Hoe, sender);
		private void GoldTapped(object sender,TappedRoutedEventArgs e)=> CityUI.Show( Artifact.ArtifactType.chest, sender);
		private void TitleTapped(object sender,TappedRoutedEventArgs e)=> ResearchPurchase.ShowInstance( ResearchItems.GetTA(player.TALevel-1) );
		private void ResContextRequest(UIElement sender,ContextRequestedEventArgs? args )
		{
			if(args is  not null)
				args.Handled    = true;
			var flyout = new MenuFlyout();
			flyout.SetXamlRoot(sender);

			flyout.AddItem("Refine..",Symbol.Trim,() =>
			{
				RefineDialogue.ShowInstance();
			});
			flyout.AddItem("Toolkit..",Symbol.OutlineStar,() =>
			{
				CityUI.Show(Artifact.ArtifactType.toolkit,sender);
			});
			flyout.AddItem("Chest..",Symbol.OutlineStar,() =>
			{
				CityUI.Show(Artifact.ArtifactType.chest,sender);
			});
			flyout.AddItem("Saw..",Symbol.OutlineStar,() =>
			{
				CityUI.Show(Artifact.ArtifactType.saw,sender);
			});
			flyout.AddItem("Chisel..",Symbol.OutlineStar,() =>
			{
				CityUI.Show(Artifact.ArtifactType.chisel,sender);
			});
			flyout.AddItem("Crucible..",Symbol.OutlineStar,() =>
			{
				CityUI.Show(Artifact.ArtifactType.crucible,sender);
			}); 
			flyout.AddItem("Hoe..",Symbol.OutlineStar,() =>
			{
				CityUI.Show(Artifact.ArtifactType.Hoe,sender);
			});
			flyout.Show(sender,args);
		}

		private void QuestsTapped(object sender,RoutedEventArgs e)
		{
			QuestList.ShowInstance();
		}

		private void ResearchTapped(object sender,RoutedEventArgs e)
		{
			ResearchList.ShowInstance();
		}

		private void ArtifactsTapped(object sender,RoutedEventArgs e)
		{
			Artifacts.ShowInstance();
		}
	}
}
