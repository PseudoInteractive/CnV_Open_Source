using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using COTG.Game;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp.UI;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace COTG.Views
{
	public sealed partial class CityControl : UserControl, INotifyPropertyChanged
	{
		#region PropertyChanged
		public void OnPropertyChanged(string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		public City city;
		public BitmapImage icon => city?.icon;
		public string name => city != null ? city.nameAndRemarks : "None";
		public CityControl()
		{
			this.InitializeComponent();
		}

		private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
		{
			var image = sender as FrameworkElement;
			var cc = image.FindParent<CityControl>();
			if (cc?.city != null)
				JSClient.ShowCity(cc.city.cid, false, false, false);

		}

		private void Image_Tapped(object sender, TappedRoutedEventArgs e)
		{
			var image = sender as FrameworkElement;
			var cc = image.FindParent<CityControl>();
			if( cc?.city!=null)
				Spot.ProcessCoordClick(cc.city.cid, false, App.keyModifiers, false);

		}

		private void Image_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			var image = sender as FrameworkElement;
			var cc = image.FindParent<CityControl>();
			if (cc?.city != null)
				cc.city.ShowContextMenu(image, e.GetPosition(image));
		}

		
		private void CityName_SuggestionChosen(AutoSuggestBox sender,AutoSuggestBoxSuggestionChosenEventArgs args)
		{
			if(Spot.TryGet(args.SelectedItem as string,useAll.IsOn,out var _city))
			{
				this.city = _city;
				OnPropertyChanged();
		
			}
		}
	}
}
