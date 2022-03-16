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
using Microsoft.UI.Xaml.Data;

namespace CnV
{
	public class DialogG :Microsoft.UI.Xaml.Controls.Expander
	{
		protected virtual string title => "Title";

		protected TextBlock titleText;
		public UIElementCollection TitleGrid
	{
		get { return(UIElementCollection) GetValue(TitleGridProperty);}
		set { SetValue(TitleGridProperty,value); }
	}

		public static readonly DependencyProperty TitleGridProperty = DependencyProperty.Register(
		"TitleGrid",
		typeof(UIElementCollection),
		typeof(DialogG),
		new PropertyMetadata(null));

		protected override void OnKeyDown(KeyRoutedEventArgs e)
		{
			if(e.Key == Windows.System.VirtualKey.Escape || e.Key == Windows.System.VirtualKey.Cancel )
			{
				e.Handled=true;
				Hide();
			}
			else
			{
				base.OnKeyDown(e);
			}
		} 
		internal static List<DialogG> all = new();
		public DialogG()
		{
			ManipulationMode = ManipulationModes.TranslateX|ManipulationModes.TranslateY;
			IsExpanded=true;
			
			var brush = AppS.Brush(0xFF150030u);
			Background = brush;
			
			MaxHeight = Settings.canvasHeight;
			var grid = new Grid() {Padding=new(),Margin=new()  };
			TitleGrid = grid.Children;
			grid.ColumnDefinitions.Add(new() { Width=GridLength.Auto } );
			grid.ColumnDefinitions.Add(new() {  Width=new(1,GridUnitType.Star)});
			grid.ColumnDefinitions.Add(new() { Width=GridLength.Auto});
			titleText = new TextBlock()
			{
				Text=title,
				Style = App.instance.Resources["TextBlockMedium"]  as Style,
				VerticalAlignment=VerticalAlignment.Center,
				Padding = new(),
				Margin = new(8,0,16,0)
			};
			
			grid.Children.Add(titleText);
			var button = new Button() {
				
				Content="X", 
				HorizontalAlignment=HorizontalAlignment.Right,Width=40,
				
				Style = (Style)App.instance.Resources["ButtonMedium"],Margin=new(16,0,8,0),Padding=new(4,0,4,0) };
			button.Click += Hide;
			var headerB = new Button() { HorizontalContentAlignment=HorizontalAlignment.Stretch,
				VerticalContentAlignment=VerticalAlignment.Stretch,CornerRadius=new(2),Margin=new(),Padding=new(),
				Background=AppS.Brush(0xff000000u) };
			headerB.Content = grid;
			Grid.SetColumn(button,2);
			
			grid.Children.Add(button);
			IsTabStop=true;
			TabFocusNavigation = KeyboardNavigationMode.Cycle;
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
		public async void Show(bool toggle)
		{
			try
			{
				while(isAnimating)
				{
					await Task.Delay(100);
				}

				var was = ShellPage.gameUIFrame.Children.Remove(this);
				if(!(was && toggle))
				{
					IsExpanded=true;
					titleText.Text = title;
					ShellPage.gameUIFrame.Children.Add(this);
					var focusItem = FocusManager.FindFirstFocusableElement(Content as DependencyObject);
					
					isAnimating=true;
					await Opening();
					isAnimating=false;
					if(focusItem is not null)
					{
						await Task.Delay(0);
						await FocusManager.TryFocusAsync(focusItem,FocusState.Programmatic);
					}
				}
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
