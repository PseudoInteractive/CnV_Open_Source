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

namespace CnV;

public sealed partial class CityFlyout:UserControl, IANotifyPropertyChanged
{
	internal City city = City.invalid;
	internal static bool isLoaded;
	
	internal static bool hasFocus;
	internal static bool isOpen => hasFocus;

	List<ACommandCategory> categories => City.ACatCity.children;
	ObservableCollection<ACommand> baseCommands = new();
	ObservableCollection<ACommand> categoryCommands = new();
	ObservableCollection<ACommand> mruCommands = new();
	ObservableCollection<ACommand> searchCommands = new();
	List<ACommand> mru = new();

	public event PropertyChangedEventHandler? PropertyChanged;

	public CityFlyout() {
		InitializeComponent();
	}

	public void SetCity(City city) {
		this.city = city;
		UpdateCommands();
		this.CallPropertyChanged(null);
	}
	void ClearCommands() {
		categoryCommands.Clear();
		baseCommands.Clear();
		searchCommands.Clear();
		//mruCommands.Clear();
			

	}

//	public void Reset() {
////		if(!object.ReferenceEquals(SelectedItem , catCity) )
////			SelectedItem = catCity;
////		else
//			UpdateCommands();
//	}

	internal static void Hide() {
		if(isLoaded)
			ShellPage.instance.cityFlyout.Visibility = Visibility.Collapsed;

	}
	private void CategoryChanged(object sender,SelectionChangedEventArgs e) {
		UpdateCommands();
	}
		internal void UpdateCommands() {
		
			ClearCommands();
			if(!city.IsValid() ) {
			
				return;
			}

		var sel = categoryListView.SelectedItem as ACommandCategory;
		if(sel is null)
			return;
		var cat = sel.name;

		if(cat.StartsWith("Settings") ){
			cat = "settings";
		}
		else { 
			Assert(cat.StartsWith("ACat"));
			cat = cat.Substring(4);
		}
		//var items = new List<ACommandI>();
		foreach(var c in ACommand.commands) 
		{
				if(c.category == "city") {
					baseCommands.Add(c);
				}
				else
				{
					if(!c.category.EqualsIgnoreCase(cat)) {
						continue;
					}
					//				var b = new Button() { Command = c.CreateInstance(city),Content = c.Label,CommandParameter = city };
					categoryCommands.Add(c);
				}
		}

	//	commands.ItemsSource = items;
	}
	internal static void RegisterRecentCommand( ACommand c) {
		var mru = ShellPage.instance.cityFlyout.mruCommands;
		var id = mru.IndexOf(c);
		if(id >= 0) {
			if(id != 0)
				mru.Move(0,id);
		}
		else {
			if(mru.Count > 3)
				mru.RemoveAt(AMath.random.Next(mru.Count));
	
			mru.Add(c);
		}

	}
	private async void CommandClick(object sender,ItemClickEventArgs e) {
		var c = e.ClickedItem as ACommand;
		await Execute(c);
	}

	private async Task Execute(ACommand c) {
		if(c is not null) {
			try {

				if(c.CanExecute(city)) {
					RegisterRecentCommand(c);
					if(await c.Go(city) == true) {
						Hide();
					}

				}
				else {

					Note.Show("cannot do that now");
				}
			}
			catch(Exception ex) {
				LogEx(ex);

			}

		}
	}

	public void CallPropertyChanged(string members = null) {
		PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(members)); 
	}

    private void SearchChange(AutoSuggestBox sender,AutoSuggestBoxTextChangedEventArgs args) {
		if(args.Reason == AutoSuggestionBoxTextChangeReason.UserInput) {
			// 
			var t = search.Text;
			if(!t.IsNullOrEmpty()) {
				var candidates = new SortedList<int,ACommand>();
				foreach(var c in ACommand.commands) {
					var score = c.GetSearchScore(t);
					if(score <= 0)
						continue;
					candidates.Add(score,c);
				}
				candidates.Values.ToArray().SyncList(searchCommands);
			}
			
		}
	}

	private async void SearchSubmit(AutoSuggestBox sender,AutoSuggestBoxQuerySubmittedEventArgs args) {
		if( (args.ChosenSuggestion is ACommand c) && c is not null) {
			// Change focus back
			await Execute(c);
		}
	}

	


	private void SearchGotFocus(object sender,RoutedEventArgs e) {
		search.Width = 200;
	}

	private void SearchLostFocus(UIElement sender,LosingFocusEventArgs args) {
		search.Width=40;
		search.Text = string.Empty;
	}

	private void VisitCity(object sender,RoutedEventArgs e) {
		if(city.CanVisit() && !city.isBuild && city.isFocus) {
			CnVClient.CitySwitch(city.cid);
		}
		else {
			city.ProcessClick(AppS.keyModifiers.ClickMods(isRight: true,noFlyout: true));
		}
	}

	private void ViewCity(object sender,RightTappedRoutedEventArgs e) {
		city.ProcessClick(AppS.keyModifiers.ClickMods(isRight: true,noFlyout: true));
	}

	private void OnLoaded(object sender,RoutedEventArgs e) {
		isLoaded = true;
	}

	private void lostFocus(object sender,RoutedEventArgs e) {
		Note.Show($"Lost Focus ");

	}

	private void gotFocus(object sender,RoutedEventArgs e) {
		Note.Show($"Got Focus ");

	}

	//private void TogglePane(object sender,TappedRoutedEventArgs e) {
	//	IsPaneOpen = !IsPaneOpen;
	//}

	//   private void search_FocusDisengaged(Control sender,FocusDisengagedEventArgs args) {

	//   }
}

static partial class CityUI {
	public static void ShowContextMenu(this City me,Windows.Foundation.Point position) {
		if(!me.isValid) {
			Assert(false);
			return;
		}
		if(!CityFlyout.isLoaded)
			return;
		//   SelectMe(false) ;
		AppS.DispatchOnUIThread(() => {
			//	var flyout = ShellPage.instance.cityFlyoutFlyout;// new Flyout() {  AreOpenCloseAnimationsEnabled=false,ShouldConstrainToRootBounds=false,ShowMode=Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowMode.Auto };
			var nav = ShellPage.instance.cityFlyout;
			//		nav.Reset(me);
			var sc = (new Windows.Foundation.Point(ShellPage.mousePosition.X,ShellPage.mousePosition.Y)).TransformPoint(ShellPage.canvas,ShellPage.instance._rootGrid);

			Canvas.SetLeft(nav,sc.X - 32); // Points in relation to window since canvas is at origin
		Canvas.SetTop(nav,sc.Y - 60);
			//	Canvas.SetZIndex(nav,99); // Points in relation to window since canvas is at origin	

			nav.SetCity(me);

			//foreach(var cat in City.ACatCity.children) {
			//	NavigationViewItem i = new();
			//	i.Icon = ImageHelper.GetIcon(cat.icon);
			//	i.Content = cat.label;
			//	if(cat.description is not null)
			//		i.SetToolTip(cat.description);
			//	nav.MenuItems.Add(i);

			//}

			//	flyout.SetXamlRoot(uie);
			//	AddToFlyout(me,(flyout,nav), uie is UserTab );
			//		nav.IsPaneOpen=false;
			//		nav.SelectedItem = nav.catCity;
			nav.UpdateCommands();
			nav.Visibility = Visibility.Visible;
			nav.visitCityButton.Focus(FocusState.Programmatic);
//			nav.visitCityButton.
			//	nav.ApplyTemplate();
			//   flyout.XamlRoot = uie.XamlRoot;
			//		flyout.ShowAt(null,new() { Position= position, ShowMode=FlyoutShowMode.TransientWithDismissOnPointerMoveAway,Placement=FlyoutPlacementMode.Auto});

		});
	}
	public static void ShowContextMenu(this City me) {
		ShowContextMenu(me,ShellPage.mousePosition.AsPoint());
	}
}
