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
		}

		public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged(string? member = null)
	{
		if (this.PropertyChanged is not null) 
			AppS.DispatchOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
	}
		public static void Changed(string? member = null)
		{
			if(instance is not null)
				instance.OnPropertyChanged(member);
		}

		public string zirconiaS => Player.me.zirconia.Format();
		public string manaS => Player.me.mana.Format();
		public string goldS => (Player.me?.gold ?? 0).Format();
		public string RefineS(int id) => Player.me.data.refines[id].Format();
		internal static void GoldChanged()	=> Changed(nameof(goldS));
		internal static void ManaChanged()	=> Changed(nameof(manaS));

		internal static void ZirconiaChanged() => Changed(nameof(zirconiaS));
		internal static void RefinesChanged() => Changed(nameof(RefineS));

		

		private void WoodTapped(object sender,TappedRoutedEventArgs e)=> Artifact.Show( Artifact.ArtifactType.saw, sender);
		private void StoneTapped(object sender,TappedRoutedEventArgs e)=> Artifact.Show( Artifact.ArtifactType.chisel, sender);
		private void IronTapped(object sender,TappedRoutedEventArgs e)=> Artifact.Show( Artifact.ArtifactType.crucible, sender);
		private void FoodTapped(object sender,TappedRoutedEventArgs e)=> Artifact.Show( Artifact.ArtifactType.Hoe, sender);
		private void GoldTapped(object sender,TappedRoutedEventArgs e)=> Artifact.Show( Artifact.ArtifactType.chest, sender);
		private void ResContextRequest(UIElement sender,ContextRequestedEventArgs args, Artifact.ArtifactType artifactType )
		{
			args.Handled    = true;
			var flyout = new MenuFlyout();
			flyout.SetXamlRoot(sender);

			flyout.AddItem("Refine..",Symbol.Trim,() =>
			{
				Artifact.Show(artifactType,sender);
			});

			flyout.AddItem("Artifact..",Symbol.OutlineStar,() =>
			{
				Artifact.Show(artifactType,sender);
			});

			flyout.Show(sender,args);
		}

		

		private void WoodContextRequest(UIElement sender,ContextRequestedEventArgs args)
		{

		}
	}
}
