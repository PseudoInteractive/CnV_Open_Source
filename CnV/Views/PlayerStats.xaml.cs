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
			Player.activePlayerChanged += (priorPlayer,newPlayer) =>
				{
					if(priorPlayer != null)
						priorPlayer.PropertyChanged-= PlayerChanged;
					newPlayer.PropertyChanged+= PlayerChanged;
					Changed();
				};
		}

		private void PlayerChanged(object? sender,PropertyChangedEventArgs e)
		{
			CityStats.nextTextUpdateTick=0;
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
		public string karmaRegenS => $"+{player.karmaRegenRate*ServerTime.secondsPerHour:N1} karma/h";
		public string goldS => player.SampleGold().Format();
		public string RefineS(int id) => player.data.refines[id].Format();
		//internal static void GoldChanged()	=> Changed(nameof(goldS));
		//internal static void ManaChanged()	=> Changed(nameof(manaS));

		//internal static void ZirconiaChanged() => Changed(nameof(zirconiaS));
		//internal static void RefinesChanged() => Changed(nameof(RefineS));
		public string refinesAndGoldS => $"{CnV.Resources.goldGlyph} {Player.active.gold.Format()}\n    +{Player.active.goldPerHour.FormatJson()}/h\nRefines:\n{Player.active.refinesRef.Format("\n")}";

		public string title => player.Title;

		private void AllTapped(object sender,TappedRoutedEventArgs e) =>
			ResContextRequest(sender as UIElement,null);

		private void WoodTapped(object sender,TappedRoutedEventArgs e)=> CityUI.Show( Artifact.ArtifactType.saw, sender);
		private void StoneTapped(object sender,TappedRoutedEventArgs e)=> CityUI.Show( Artifact.ArtifactType.chisel, sender);
		private void IronTapped(object sender,TappedRoutedEventArgs e)=> CityUI.Show( Artifact.ArtifactType.crucible, sender);
		private void FoodTapped(object sender,TappedRoutedEventArgs e)=> CityUI.Show( Artifact.ArtifactType.Hoe, sender);
		private void GoldTapped(object sender,TappedRoutedEventArgs e)=> CityUI.Show( Artifact.ArtifactType.chest, sender);
		//private void TitleTapped(object sender,TappedRoutedEventArgs e)=> ResearchPurchase.ShowInstance( ResearchItems.GetTA(player.TALevel-1) );
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

		private void OutgoingTapped(object sender,RoutedEventArgs e) {
			OutgoingTab.instance.Show();
		}
		internal void UpdateOutgoingText() {
			var c = OutgoingTab.GetOutgoingCounts();
			if( c.forAlliance==0) {
				outgoing.Visibility = Visibility.Collapsed;
			}
			else {
				outgoing.Text = $"{c.forAlliance}({c.forMe})";
				outgoing.Visibility = Visibility.Visible;
			}
			OnPropertyChanged();
		}
		internal string outgoingToolTip {
			get {
				{
					var c = OutgoingTab.GetOutgoingCounts();
					if( c.forAlliance==0) {
						return "No outgoing";
					}
					else {
						return $"{c.forAlliance} outgoing for alliance\n{c.forMe} outgoing for {Player.myShortName}\nNext: {c.nextArrival}";

					}
				}
			}
		}
		
		internal void UpdateIncomingText() {
			var c = IncomingTab.GetIncomingCounts();
			if(c.forAlliance==0 ) {
				incoming.Visibility = Visibility.Collapsed;
			}
			else {
				incoming.Text = $"{c.forAlliance}({c.forMe})";
				incoming.Visibility = Visibility.Visible;
			}
			OnPropertyChanged();
		}
		internal string incomingToolTip {
			get {
				{
					var c = IncomingTab.GetIncomingCounts();
					if(c.forAlliance==0 ) {
						return "No Incoming";
					}
					else {
						return $"{c.forAlliance} incoming for alliance\n{c.forMe} incoming for {Player.myShortName}\nNext: {c.nextArrival}";

					}
				}
			}
		}

		private void IncomingTapped(object sender,RoutedEventArgs e) {
			IncomingTab.instance.Show();

		}
	}
}
