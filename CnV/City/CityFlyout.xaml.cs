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

public sealed partial class CityFlyout:NavigationView, IANotifyPropertyChanged
{
	internal City city = City.invalid;


	ObservableCollection<ACommandInstance> baseCommands = new();
	ObservableCollection<ACommandInstance> categoryCommands = new();
	ObservableCollection<ACommandInstance> mruCommands = new();
	List<ACommand> mru = new();

	public event PropertyChangedEventHandler? PropertyChanged;

	public CityFlyout() {
		this.InitializeComponent();
	}

	public void SetCity(City city) {
		this.city = city;
		UpdateCommands();
		this.CallPropertyChanged(null);
	}
	void ClearCommands() {
		categoryCommands.Clear();
		baseCommands.Clear();
		mruCommands.Clear();
			

	}

//	public void Reset() {
////		if(!object.ReferenceEquals(SelectedItem , catCity) )
////			SelectedItem = catCity;
////		else
//			UpdateCommands();
//	}

	private void CategoryChanged(NavigationView sender,NavigationViewSelectionChangedEventArgs args) {
		UpdateCommands();
	}
		internal void UpdateCommands() {
		
			ClearCommands();
			if(!city.IsValid() ) {
			
				return;
			}

		var sel = SelectedItem as NavigationViewItem;
		if(sel is null)
			return;
		var cat = sel.Name;

		if(cat.StartsWith("Settings") ){
			cat = "settings";
		}
		else { 
			Assert(cat.StartsWith("cat"));
			cat = cat.Substring(3);
		}
		//var items = new List<ACommandInstance>();
		foreach(var c in ACommand.commands) 
		{
				//if(c.category == "city") {
				//	baseCommands.Add(c.CreateInstance(city));
				//}
				//else
				{
					if(!c.category.EqualsIgnoreCase(cat)) {
						continue;
					}
					//				var b = new Button() { Command = c.CreateInstance(city),Content = c.Label,CommandParameter = city };
					categoryCommands.Add(c.CreateInstance(city));
				}
		}

	//	commands.ItemsSource = items;
	}
	private void CommandClick(object sender,ItemClickEventArgs e) {
		var c = e.ClickedItem as ACommandInstance;
		if(c is not null) {
			try {
				ShellPage.instance.cityFlyoutFlyout.Hide();
				if(c.CanExecute(c.city))
					c.Execute(c.city);
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
				var candidates = new SortedList<int,ACommandInstance>();
				foreach(var c in ACommand.commands) {
					var score = c.GetSearchScore(t);
					if(score <= 0)
						continue;
					candidates.Add(score,c.CreateInstance(city));
				}
				candidates.Values.ToArray().SyncList(categoryCommands);
			}
			
		}
	}

	private void SearchSubmit(AutoSuggestBox sender,AutoSuggestBoxQuerySubmittedEventArgs args) {

	}

	private void TogglePane(object sender,TappedRoutedEventArgs e) {
		IsPaneOpen = !IsPaneOpen;
	}

    private void search_FocusDisengaged(Control sender,FocusDisengagedEventArgs args) {

    }
}

