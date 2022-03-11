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
	public sealed partial class ArtifactDialogue:TeachingTip
	{
		public Artifact a;
		public ArtifactDialogue(Artifact artifact, FrameworkElement target) 
		{
			this.InitializeComponent();
			a = artifact;
			
			if(target is not null)
				Target=target;
		}
		public static async void Show(Artifact artifact, FrameworkElement target=null)
		{
			var rv = new ArtifactDialogue(artifact,target);
		//	rv.HeroContent.Focus(FocusState.Programmatic);
			rv.count.Value = artifact.owned. Max(1);
			await AppS.ShowAsync2(rv);
			
		}
	
		public string priceStr=> ((count.Value.RoundToInt()-a.owned).Max(0)*a.zirconia).Format();
		
		

		private void TeachingTip_ActionButtonClick(TeachingTip sender,object args)
		{
			
			var wanted = count.Value.RoundToInt();
			int have = a.owned;
			if(wanted > have)
				(new CnVEventPurchaseArtifacts() { artifact = (ushort)a.id,count = (ushort)(wanted-have) }).Execute();
			if( wanted > 0 )
				(new CnVEventUseArtifacts(City.build.CidToWorld()) { artifact = (ushort)a.id,count = (ushort)wanted,aux=0 }).EnqueueAsap();
			IsOpen=false;

		}
	}
}
