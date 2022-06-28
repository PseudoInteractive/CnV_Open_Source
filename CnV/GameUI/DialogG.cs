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
		internal virtual bool closeOnCitySwitch => true;
		
		protected TextBlock titleText;
		protected Button closeButton;
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
				Cancel();
			}
			else
			{
				base.OnKeyDown(e);
			}
		} 
		internal static List<DialogG> all = new();
		public DialogG()
		{
			IsTabStop=true;
		//	ManipulationMode = ManipulationModes.TranslateX|ManipulationModes.TranslateY;
			IsExpanded=true;
			var brush =   AppS.Brush( (Windows.UI.Color)App.instance.Resources["SystemAccentColorDark3"]);
			HorizontalAlignment= HorizontalAlignment.Stretch;
			Background = brush;
			//var shadow = App.instance.Resources["Shadow8"] as AttachedDropShadow;
			//Effects.SetShadow(this,shadow);
			MaxHeight = Settings.canvasHeight;
			var grid = new Grid() {Padding=new(),Margin=new()  };
			TitleGrid = grid.Children;
			grid.ColumnDefinitions.Add(new() { Width=GridLength.Auto } );
			grid.ColumnDefinitions.Add(new() {  Width=new(1,GridUnitType.Star)});
			grid.ColumnDefinitions.Add(new() { Width=GridLength.Auto});
			titleText = new TextBlock()
			{
				
				Style = App.instance.Resources["TextBlockMedium"]  as Style,
				VerticalAlignment=VerticalAlignment.Center,
				Padding = new(),
				Margin = new(8,0,16,0)
			};
			
			grid.Children.Add(titleText);
			closeButton = new Button() {
				
				Content="X", 
				HorizontalAlignment=HorizontalAlignment.Right,Width=40,
				
				Style = (Style)App.instance.Resources["ButtonMedium"],Margin=new(16,0,8,0),Padding=new(4,0,4,0) };
			closeButton.Click += Cancel;
			var headerB = new Button() { 
				HorizontalAlignment=HorizontalAlignment.Stretch,
				HorizontalContentAlignment=HorizontalAlignment.Stretch,
				VerticalContentAlignment=VerticalAlignment.Stretch,CornerRadius=new(2),Margin=new(),Padding=new(),
				Background=AppS.Brush(0xff000000u) };
			headerB.Content = grid;
			Grid.SetColumn(closeButton,2);
			
			grid.Children.Add(closeButton);
		//	IsTabStop=true;
		//	TabFocusNavigation = KeyboardNavigationMode.Cycle;
			base.Header =headerB;
			//grid.IsTapEnabled=true;
			//grid.Tapped +=Grid_Tapped;
			
			
		//	Canvas.SetZIndex(this,101);
			Canvas.SetLeft(this,260);
		//	this.ManipulationDelta+=this.OnManipulationDelta;
			headerB.ManipulationMode= ManipulationModes.TranslateX|ManipulationModes.TranslateY;
			headerB.ManipulationDelta+=this.OnManipulationDelta;
			lock(all)
			{


				all.Add(this);
			}
		}


		
		protected void FilterNaNs(NumberBox sender,NumberBoxValueChangedEventArgs args)
		{
			App.FilterNans(sender,args);
		}
		protected void FilterPositive(NumberBox sender,NumberBoxValueChangedEventArgs args)
		{
			App.FilterPositive(sender,args);
			
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
		
		TaskCompletionSource<bool> showTask;

		static internal void CitySwitched() {
			AppS.QueueOnUIThread(() => {
				lock(all) {
					foreach(var d in all) {
						if(d.closeOnCitySwitch)
							d.Hide(false);

					}

				}
			});
		}

		public Task<bool> Show(bool toggle)
		{
			try
			{
				

				var was = Hide(false);

				if(!(was && toggle))
				{

					IsExpanded=true;
					titleText.Text = title;
					showTask = new TaskCompletionSource<bool>();
					ShellPage.gameUIFrame.Children.Add(this);
					var focusItem = closeButton; 
					
					if(focusItem is not null)
					{
						FocusManager.TryFocusAsync(focusItem,FocusState.Programmatic);
					}
					return showTask.Task;
				}
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
			return Task.FromResult(false);

		}
		
		public bool Hide(bool result)
		{
			try
			{
				if( ShellPage.gameUIFrame.Children.Contains(this) )
				{
					ShellPage.gameUIFrame.Children.Remove(this);
					showTask.SetResult(result);
					return true;
				}
				
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
			return false;
		}
		public void Cancel() => Hide(false);
		public void Done() => Hide(true);
		internal void Cancel(object sender,RoutedEventArgs e) => Cancel();
		internal void Done(object sender,RoutedEventArgs e) => Done();

    }
}
