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
	public class DialogG :Microsoft.UI.Xaml.Controls.Expander
	{
		protected virtual string title => "Title";
		protected Grid headerGrid;
		internal static List<DialogG> all = new();
		public DialogG()
		{
			ManipulationMode = ManipulationModes.TranslateX|ManipulationModes.TranslateY;
			IsExpanded=true;
			
			var brush = AppS.Brush(0xFF150030u);
			Background = brush;
			Width=600;
			MaxHeight = Settings.canvasHeight;
			var grid = new Grid() {Padding=new(),Margin=new()  };
			headerGrid = grid;
			
			grid.Children.Add(new TextBlock() { Text=title, 
				Style = App.instance.Resources["TextBlockMedium"]  as Style,
				VerticalAlignment=VerticalAlignment.Center,
				Padding = new(),
				Margin = new(4,0,0,0) });
			var button = new Button() {
				
				Content="X", 
				HorizontalAlignment=HorizontalAlignment.Right,
				Style = (Style)App.instance.Resources["ButtonMedium"],Margin=new(1),Padding=new() };
			button.Click += Hide;
			var headerB = new Button() { HorizontalContentAlignment=HorizontalAlignment.Stretch,
				VerticalContentAlignment=VerticalAlignment.Stretch,CornerRadius=new(3),Margin=new(),Padding=new()};
			headerB.Content = grid;
			grid.Children.Add(button);
			base.Header =headerB;
			//grid.IsTapEnabled=true;
			//grid.Tapped +=Grid_Tapped;
			
			
			Canvas.SetLeft(this,260);
			this.ManipulationDelta+=this.OnManipulationDelta;
			headerB.ManipulationMode= ManipulationModes.TranslateX|ManipulationModes.TranslateY;
			headerB.ManipulationDelta+=this.OnManipulationDelta;
			lock(all)
			{


				all.Add(this);
			}
		}

		private void Grid_Tapped(object sender,TappedRoutedEventArgs e)
		{
			Note.Show("Tapped");
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
