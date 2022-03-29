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

	public sealed partial class CityBox:UserControl, IANotifyPropertyChanged
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
		#endregion

		public City city;
		public BitmapImage icon => city?.icon;
		public string name => city != null ? city.nameAndRemarks : "None";
		public CityBox()
		{
			this.InitializeComponent();
		}
		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
  "Label",
  typeof(string),
  typeof(CityControl),
  new PropertyMetadata(null)
);

		public static readonly DependencyProperty CityProperty = DependencyProperty.Register(
		  "City",
		  typeof(City),
		  typeof(CityBox),
		  new PropertyMetadata(null)
		);

		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty,value); }
		}

		public City City
		{
			get => city;
			set => city = value;
		}


		private void TextBlock_Tapped(object sender,TappedRoutedEventArgs e)
		{
			var image = sender as FrameworkElement;
			var cc = image.FindParent<CityControl>();
			if(cc?.city != null)
				CityUI.ShowCity(cc.city.cid,false,false,false);

		}

		private void Image_Tapped(object sender,TappedRoutedEventArgs e)
		{
			var image = sender as FrameworkElement;
			var cc = image.FindParent<CityControl>();
			if(cc?.city!=null)
				Spot.ProcessCoordClick(cc.city.cid,false,AppS.keyModifiers,false);

		}

		private void Image_RightTapped(object sender,RightTappedRoutedEventArgs e)
		{
			var image = sender as FrameworkElement;
			var cc = image.FindParent<CityControl>();
			if(cc?.city != null)
				cc.city.ShowContextMenu(image,e.GetPosition(image));
		}



	}
}
