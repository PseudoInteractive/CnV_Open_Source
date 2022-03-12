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
			
			Canvas.SetLeft(this,260);
			this.ManipulationDelta+=this.OnManipulationDelta;
				


			
		}


		private void OnManipulationDelta(object sender,ManipulationDeltaRoutedEventArgs e)
		{
			if(e.Delta.Translation.X != 0)
				Canvas.SetLeft(this,(Canvas.GetLeft(this) + e.Delta.Translation.X).Max(0) );

			if(e.Delta.Translation.Y != 0)
				Canvas.SetTop(this, (Canvas.GetTop(this) + e.Delta.Translation.Y).Max(0) );
			e.Handled=true;
			
		}
		virtual protected Task Opening() => Task.CompletedTask;
		virtual protected Task Closing() => Task.CompletedTask;
		
		bool isAnimating;
		public async void Show()
		{
			try
			{
				while(isAnimating)
				{
					await Task.Delay(100);
				}

				ShellPage.gameUIFrame.Children.Remove(this);
				ShellPage.gameUIFrame.Children.Add(this);
				isAnimating=true;
				await Opening();
				isAnimating=false;
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}

		}
		public async void Hide()
		{
			try
			{
				while(isAnimating)
				{
					await Task.Delay(100);
				}
				isAnimating=true;
				await Closing();
				isAnimating=false;
				ShellPage.gameUIFrame.Children.Remove(this);
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
		}

		internal void Hide(object sender,RoutedEventArgs e)
		{
			Hide();
		}

    }
}
