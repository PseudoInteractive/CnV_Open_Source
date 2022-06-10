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

namespace CnV.Views
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
		public City _city = City.invalid;
		// Sets city silently
		public void SetCityI(City value) {
			_city = value;
			if(_city is not null && !citySelections.Contains(_city))
				citySelections.Add(_city);
		}
		public void SetCity(City value, bool triggerCityChanged=true) {
			if(_city != value) {
				SetCityI(value);
			CallPropertyChanged(nameof(this.city));

			if(triggerCityChanged)
				cityChanged?.Invoke(this,_city);
				
			}
		}

		// Not for internal use
		protected City city
		{
			get => _city;
			set {
				if(!object.ReferenceEquals(_city, value) )
				{
					SetCityI( value); 
					cityChanged?.Invoke(this,_city);

				//	OnPropertyChanged(nameof(this.city));
				}
			}
		}
		public int cid => city is City c ? c.cid : 0;
		public BitmapImage icon => city?.icon;
		public string name
		{
			get => city != null ? city.nameAndRemarks : "None";
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

		public bool allowNone { get; set; } = true;		
		public bool allowOtherPlayers { get; set; } = false;		
		public bool allowOtherAlliances { get; set; } = false;		

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
			try
			{
				var image = sender as FrameworkElement;
				var cc = image.FindParent<CityControl>();
				if(cc?.city != null)
					CityUI.ShowCity(cc.city.cid,false,false,false);
			}
			catch(Exception _ex)
			{
				LogEx(_ex);

			}

		}

		private void Image_Tapped(object sender, TappedRoutedEventArgs e)
		{
			try
			{
				var image = sender as FrameworkElement;
				var cc = image.FindParent<CityControl>();
				if(cc?.city!=null)
					Spot.ProcessCoordClick(cc.city.cid,false,AppS.keyModifiers,false);
			}
			catch(Exception _ex)
			{
				LogEx(_ex);

			}

		}

		private void Image_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			try
			{
				var image = sender as FrameworkElement;
				var cc = image.FindParent<CityControl>();
				if(cc?.city != null)
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
		internal event EventHandler<City> cityChanged;
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
			city?.Focus();
		}

		private void RightTappedX(object sender,RightTappedRoutedEventArgs e)
		{
			
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
			var l = City.gridCitySource.ToArray();
			if(_city is not null && !l.Contains(_city))
				l = l.Prepend(_city).ToArray();
			if(allowNone && _city != City.invalid )
				l = l.Prepend(City.invalid).ToArray();

			l.SyncList(citySelections);
		}

		private void ComboRightTapped(object sender,RightTappedRoutedEventArgs e) {
			var flyout = new MenuFlyout();
			var sel = City.GetSelectedForContextMenu(City.focus);
			foreach(var i in sel) {
				var c = i.AsCity();
				flyout.AddItem(c.nameAndRemarks,() => SetCity(c));
			}
			flyout.SetXamlRoot(cityBox);

		//   flyout.XamlRoot = uie.XamlRoot;
			flyout.ShowAt(cityBox);
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
