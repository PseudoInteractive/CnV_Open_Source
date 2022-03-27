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
	public sealed partial class ArtifactDialogue:DialogG,INotifyPropertyChanged
	{
		static ArtifactDialogue instance;
		protected override string title => "Use/Purchase Artifact";
		public Artifact a;
		public ArtifactDialogue() 
		{
			this.InitializeComponent();
			instance =this;
			
			//if(target is not null)
			//	Target=target;
		}
		public static void ShowInstance(Artifact artifact, FrameworkElement target=null)
		{
			var rv = instance ?? new ArtifactDialogue();
			rv.a = artifact;
		//	rv.HeroContent.Focus(FocusState.Programmatic);
			rv.count.Value = artifact.owned. Max(1);
			rv.OnPropertyChanged();
			rv.Show(false);
			
		}

		public string priceStr => $"{CnV.Resources.zirconiaGlyph} {((count.Value.RoundToInt()-a.owned).Max(0)*a.zirconia).Format()}";
		
		


		private void Button_Click(object sender,RoutedEventArgs e)
		{
			var wanted = count.Value.RoundToInt();
			int have = a.owned;
			if(wanted > have)
				(new CnVEventPurchaseArtifacts() { artifact = (ushort)a.id,count = (ushort)(wanted-have) }).EnqueueAsap();
			if( wanted > 0 )
				(new CnVEventUseArtifacts(City.build) { artifactId = (ushort)a.id,count = (ushort)wanted,aux=0 }).EnqueueAsap();
			Done();
		}

		private void CountChanged(NumberBox sender,NumberBoxValueChangedEventArgs args)
		{
			base.FilterNans(sender,args);
			OnPropertyChanged();
		}
		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}
	}
}
