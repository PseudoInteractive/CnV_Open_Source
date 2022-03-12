using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.UI.Helpers;
using CommunityToolkit.WinUI.Helpers;
using CommunityToolkit.WinUI.UI.Controls;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;

namespace CnV
{
	public class DialogG : UserControl
    {

		public DialogG()
		{
			ManipulationMode = ManipulationModes.TranslateX|ManipulationModes.TranslateY;
			Loaded+=(_,_) =>
			{
				Canvas.SetLeft(this,260);
				this.ManipulationDelta+=this.OnManipulationDelta;
				


			};
		}


		private void OnManipulationDelta(object sender,ManipulationDeltaRoutedEventArgs e)
		{
			if(e.Delta.Translation.X != 0)
				Canvas.SetLeft(this,(Canvas.GetLeft(this) + e.Delta.Translation.X).Max(0) );

			if(e.Delta.Translation.Y != 0)
				Canvas.SetTop(this, (Canvas.GetTop(this) + e.Delta.Translation.Y).Max(0) );
			e.Handled=true;
			
		}
		public void Show()
		{
			
			ShellPage.gameUIFrame.Children.Add(this);
		}
		public void Hide()
		{
			ShellPage.gameUIFrame.Children.Remove(this);
		}

		internal void Hide(object sender,RoutedEventArgs e)
		{
			Hide();
		}

    }
}
