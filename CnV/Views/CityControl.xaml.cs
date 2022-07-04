using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using CnV.Game;
using System.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Helpers;
using CommunityToolkit.WinUI.UI;
using CnV;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CnV
{
	using Game;

	public sealed partial class CityControl : UserControl,IANotifyPropertyChanged
	{
		#region PropertyChanged
		public void CallPropertyChanged(string member = null)
		{
			PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(member));
		}
		public void OnPropertyChanged(string member = null)
		{
			if(PropertyChanged is not null) ((IANotifyPropertyChanged)this).IOnPropertyChanged(member);
		}
		public event PropertyChangedEventHandler PropertyChanged;
		internal ObservableCollection<City> citySelections = new();
		#endregion
	//	public City _city = City.invalid;
		// Sets city silently
		public bool SetCityI(City value) {

			Assert(value != city);
			Assert(value is not null);
			if(!IsValid(value.cid,true)) {
				return false;
			}
			TouchSelections();
			if(value is not null && !citySelections.Contains(value))
				citySelections.Add(value);
			SetValue(cityProperty,value); // thhis should trigger effecst to update the UPI
			return true;
		}
		public void SetCity(City value, bool triggerCityChanged=true) {
			Assert(value is not null);
			if(city != value) {
			{
				if(!SetCityI(value))
					return;
				CallPropertyChanged(nameof(this.city));
			}

			if(triggerCityChanged)
				cityChanged?.Invoke(this,value);
				
			}
		}
		

		internal bool IsValid( SpotId cid, bool verbose) {
			if(cid == City.cidNone) {
				if(!allowNone) {
					if(verbose)
						AppS.MessageBox("Cannot set none here");
					return false;
				}
				return true;
			}
			var t = World.GetTile( (WorldC)(cid) );
			if(!t.isCityOrCastle) {
				if(!allowNonCities) {
					if(verbose)
						AppS.MessageBox("Must select city or castle");
					return false;
				}
			}
			// Assume city
			else {
				if(!allowOtherPlayers) {
					if(t.player!=Player.activeId) {
						if(verbose)
							AppS.MessageBox("Cannot set other players");
						return false;
					}
				}
				if(!allowOtherAlliances) {
					if(!t.player.AsPlayer().isInPlayerAlliance) {

						if(verbose)
							AppS.MessageBox("Cannot set players outside of alliance");
						return false;
					}

				}

			}
			return true;
		}

		// Not for internal use
//		public City city
//		{
//			 get {
//				var rv = City;
//				return rv?? City.invalid;//
//										 //	if(rv == null)
////					return 
////				Assert(_city != null);
////				return allowNone ? _city : _city.IsValid() ? _city : null;
//			}
//			protected  set {
//				Assert(value != null);
//				if(!object.ReferenceEquals(_city, value) )
//				{
//					SetCityI( value); 
//					cityChanged?.Invoke(this,_city);

//				//	OnPropertyChanged(nameof(this.city));
//				}
//			}
//		}
		public int cid => _city.cid;
		public BitmapImage icon => _city.icon;
		public string name
		{
			get => _city.nameAndRemarks;
			set {
				Note.Show(value);
			}
		}
		public CityControl()
		{
			this.InitializeComponent();
		}

		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
  "Label",
  typeof(string),
  typeof(CityControl),
  new PropertyMetadata(null)
);
		public static readonly DependencyProperty cityProperty = DependencyProperty.Register(
  "city",
  typeof(City),
  typeof(CityControl),
  new PropertyMetadata(City.invalid)
);
		public City  city
		{
			get { return (City)GetValue(cityProperty); }
			set {
				if((City)GetValue(cityProperty) != value) { SetCityI(value); }
							}

		}
		//public static void cityChangedI(DependencyObject d,DependencyPropertyChangedEventArgs e) {
		//	var c = d as CityControl;
		//	var value = e.NewValue as City;
		//	Assert(c is not null);
		//	c.TouchSelections();
		//	if(value is not null && !c.citySelections.Contains(value))
		//		c.citySelections.Add(value);
		//}
		public bool allowNone { get; set; } = true;		
		public bool allowOtherPlayers { get; set; } = false;		
		public bool allowOtherAlliances { get; set; } = false;	
		public bool allowNonCities { get; set; } = false;	

		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty,value); }
		}

				public static readonly DependencyProperty LabelWidthProperty = DependencyProperty.Register(
  "LabelWidth",
  typeof(double),
  typeof(CityControl),
  new PropertyMetadata(double.NaN)
);

		

		public double LabelWidth
		{
			get { return (double)GetValue(LabelWidthProperty); }
			set { SetValue(LabelWidthProperty,value); }
		}


		private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
		{
						e.Handled=true;

			try
			{
				var image = sender as FrameworkElement;
				var cc = image.FindParent<CityControl>();
				if(cc.isValid)
					CityUI.ShowCity(cc.city.cid,AppS.keyModifiers.ClickMods(scrollIntoUi:true) );
			}
			catch(Exception _ex)
			{
				LogEx(_ex);

			}

		}

		public bool isValid => city.IsValid();

		private void Image_Tapped(object sender, TappedRoutedEventArgs e)
		{
						e.Handled=true;

			try
			{
				var image = sender as FrameworkElement;
				var cc = image.FindParent<CityControl>();
				if(cc.isValid)
					Spot.ProcessCoordClick(cc.city.cid ,AppS.keyModifiers.ClickMods(scrollIntoUi:true) );
			}
			catch(Exception _ex)
			{
				LogEx(_ex);

			}

		}

		private void Image_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
						e.Handled=true;

			try
			{
				var image = sender as FrameworkElement;
				var cc = image.FindParent<CityControl>();
				if(cc.isValid)
					cc.city.ShowContextMenu(image,e.GetPosition(image));
			}
			catch(Exception _ex)
			{
				LogEx(_ex);

			}
		}
		private void TextSubmitted(ComboBox sender,ComboBoxTextSubmittedEventArgs args)
		{
			try
			{
				var text = args.Text.ToLower();

				Log($"Summitted {text}");
				var coords = text.FromCoordinate();
				if(coords > 0) {

					var city = City.Get(coords);
					Log($"Coords {city}");
					//				sender.SelectedItem()
					//	if(!sender.Items.Contains(city))
					//		sender.Items.Add(city);
					//	sender.SelectedItem = city;
					SetCity(city,true);
					args.Handled = true;
					return;

				}

				var items = citySelections;// sender.Items;// e as IEnumerable<City>;
				Assert(items is not null);
				foreach(var it in items)
				{
					// its good
					if(it.nameAndRemarks.ToLower() == text)
					{
						Log($"== {it}");
						SetCity(it);
						args.Handled = true;
						return;
					}
				}


				foreach(var it in items)
				{
					if(it.nameAndRemarks.ToLower().StartsWith(text))
					{
						Log($"starts {it}");

						SetCity(it);

						args.Handled = true;
						return;
					}
				}
				// try contains
				foreach(var it in items)
				{

					if(it.nameAndRemarks.ToLower().Contains(text))
					{
						Log($"Contains {it}");
						SetCity(it);

						args.Handled = true;
						return;
					}
				}
//				SetCity(city,City.none);
 // None?

				//SetCity(City.invalid,true);
				args.Handled=true;

			}
			catch(Exception _ex)
			{
				LogEx(_ex);

			}
			// todo!
		}

		//private void CityName_SuggestionChosen(AutoSuggestBox sender,AutoSuggestBoxSuggestionChosenEventArgs args)
		//{
		//	try
		//	{
		//		Log($" chosen {args.SelectedItem}");

		//		if(args.SelectedItem is City _city)
		//		{
		//			this.city = _city;
		//			OnPropertyChanged();

		//		}
		//		else
		//		{
		//			this.city = null;
		//			//				sender.Text = string.Empty;
		//			OnPropertyChanged();

		//		}
		//	}
		//	catch(Exception _ex)
		//	{
		//		LogEx(_ex);

		//	}
		//}

		//private void suggestBox_QuerySubmitted(AutoSuggestBox sender,AutoSuggestBoxQuerySubmittedEventArgs args)
		//{
		//	try
		//	{
		//		Log($" submitted {args.ChosenSuggestion}");
		//		if(args.ChosenSuggestion is City c)
		//		{
		//			//	this.city = c;
		//			//	OnPropertyChanged();
		//		}
		//		else
		//		{
		//			//	this.city = null;
		//			//	sender.Text = string.Empty; 
		//			//	OnPropertyChanged();

		//		}
		//	}
		//	catch(Exception _ex)
		//	{
		//		LogEx(_ex);

		//	}
		//}

		//private void SuggestTextChanged(AutoSuggestBox sender,AutoSuggestBoxTextChangedEventArgs args)
		//{
		//	try
		//	{
		//		Log($"Text Change {args.Reason} {sender.Text}");
		//		if(args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
		//		{

		//			var txt = sender.Text.ToLower();
		//			if(txt.Length == 0)
		//			{
		//				sender.ItemsSource = City.gridCitySource;
		//				return;
		//			}
		//			var items = new List<City>();
		//			var startsWith = txt.Length <= 1;
		//			foreach(var c in City.gridCitySource)
		//			{
		//				var p = c.nameAndRemarks;
		//				if(startsWith ? p.StartsWith(txt,StringComparison.InvariantCultureIgnoreCase) : p.Contains(txt,StringComparison.InvariantCultureIgnoreCase))
		//				{
		//					items.Add(c);
		//				}
		//			}
		//			if(items.Count > 0)
		//				sender.ItemsSource = items.ToArray();
		//			else
		//				sender.ItemsSource = new City[] { City.invalid };
		//		}
		//	}
		//	catch(Exception _ex)
		//	{
		//		LogEx(_ex);

		//	}

		//}
		public event EventHandler<City> cityChanged;
		//private void SelectionChanged(object sender,SelectionChangedEventArgs e)
		//{
		//	try
		//	{
		//		//	var box = sender as ComboBox;
		//		//	this.city = box.SelectedItem as City;
		//		//		OnPropertyChanged(nameof(this.city));
		//	}
		//	catch(Exception _ex)
		//	{
		//		LogEx(_ex);

		//	}
		//}

		private void CityIconTapped(object sender,TappedRoutedEventArgs e)
		{
			e.Handled=true;
			if(!isValid)
				return;
			city?.Focus(true);
		}

		private void RightTappedX(object sender,RightTappedRoutedEventArgs e)
		{
			e.Handled=true;
			if(!isValid)
				return;
			try
			{
				var ui = sender as UIElement;
				city?.ShowContextMenu(ui,e.GetPosition(ui));
			}
			catch(Exception _ex)
			{
				LogEx(_ex);

			}
		
		}

		private void CityControlLoaded(object sender,RoutedEventArgs e) {
			//	citySelections.Clear();
			TouchSelections();
		}
		public City _city => city ?? City.invalid;
		private void TouchSelections() {
			if(citySelections.Count > 0)
				return;
			var l = City.gridCitySource.Where(a=>IsValid(a.cid,false)).ToArray();
			var c = city;
			if(c is not null && !l.Contains(c) && (allowNone|| !c.IsInvalid()) )
				l = l.Prepend(c).ToArray();
			if(allowNone && !l.Contains(City.invalid) )
				l = l.Prepend(City.invalid).ToArray();

			l.SyncList(citySelections);
		}

		private void ComboRightTapped(object sender,RightTappedRoutedEventArgs e) {
			e.Handled=true;
			var flyout = new MenuFlyout();
			var sel = City.GetSelectedForContextMenu(City.focus).AsCities().Concat(SpotTab.spotMRU.Where(a => a.pinned)).Concat(SpotTab.spotMRU.Take(8.Min(SpotTab.spotMRU.Count))).
				Where(a=> IsValid(a.cid,false)); ;
			if(allowNone)
				sel = sel.Prepend(City.invalid);
			sel = sel.Distinct();

			foreach(var c in sel) {
				flyout.AddItem(c.nameAndRemarks,() => SetCity(c));
			}
			flyout.SetXamlRoot(cityBox);

		//   flyout.XamlRoot = uie.XamlRoot;
			flyout.ShowAt(cityBox);
		}

		private void ComboBoxDropdownClosed(object sender,object e) {
				cityChanged?.Invoke(this,_city);

		}

		private void ComboBoxSelectionChnaged(object sender,SelectionChangedEventArgs e) {
			if(e.AddedItems.FirstOrDefault() as City != city) {
				cityChanged?.Invoke(this,_city);
			}
		}


		//private void AutoSuggestBox_CharacterReceived(UIElement sender,CharacterReceivedRoutedEventArgs args)
		//{
		//	if(args.Character==(char)27 )
		//	{
		//		suggestBox.Text = name;
		//		OnPropertyChanged();
		//		args.Handled=true;

		//	}
		//}
	}
}
