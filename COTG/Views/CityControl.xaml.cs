﻿using System;
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
using COTG.Game;
using System.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Helpers;
using CommunityToolkit.WinUI.UI;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace COTG.Views
{
	public sealed partial class CityControl : UserControl,IANotifyPropertyChanged
	{
		#region PropertyChanged
		public void CallPropertyChanged(string member = null)
		{
			PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(member));
		}
		public void OnPropertyChanged(string member = null)
		{
			if(PropertyChanged is not null) ((IANotifyPropertyChanged)this).IOnPropertyChanged();
		}
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		public City city;
		public BitmapImage icon => city?.icon;
		public string name => city != null ? city.nameAndRemarks : "None";
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


		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty,value); }
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
			
			if(Spot.TryGet(args.SelectedItem as string,!onlyMine.IsChecked.GetValueOrDefault(),out var _city))
			{
				this.city = _city;
				OnPropertyChanged();
		
			}
			else
			{
				this.city = null;
				sender.Text = string.Empty;
				OnPropertyChanged();

			}
		}

		private void suggestBox_QuerySubmitted(AutoSuggestBox sender,AutoSuggestBoxQuerySubmittedEventArgs args)
		{
				if(args.ChosenSuggestion != null)
				{
				}
				else
			{
				this.city = null;
				sender.Text = string.Empty; 
				OnPropertyChanged();

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
