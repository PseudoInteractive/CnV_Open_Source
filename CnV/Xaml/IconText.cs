using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV;

public sealed class IconText:Control
{
	public IconText()
	{
		this.DefaultStyleKey = typeof(IconText);
	}

	private const string PartImage = "image";


	public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
		"Image",
		typeof(ImageSource),
		typeof(IconText),
		new PropertyMetadata(null));
	//public static readonly DependencyProperty ImageUriProperty = DependencyProperty.Register(
	//           "ImageUri",
	//           typeof(string),
	//           typeof(IconText),
	//           new PropertyMetadata(null,imageSourceChanged));

	//public Image XamlImage => (Image)GetTemplateChild("image");

	//private static void imageSourceChanged(DependencyObject d,DependencyPropertyChangedEventArgs e)
	//{
	//	var c = (IconText)d;
	//	if(e.NewValue is string s)
	//		c.ImageUri = s;
	//	else if(e.NewValue is ImageSource src)
	//		c.Image= src;
	//	else
	//		Assert(false);

	//}



	public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
			"Text",
			typeof(string),
			typeof(IconText),
			new PropertyMetadata(null));


	/// <summary>
	/// Identifies the <see cref="Orientation"/> dependency property.
	/// </summary>
	public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
		nameof(Orientation),
		typeof(Orientation),
		typeof(IconText),
		new PropertyMetadata(Orientation.Horizontal,OnOrientationChanged));

	/// <summary>
	/// Gets or sets the <see cref="Orientation"/> used for the header.
	/// </summary>
	/// <remarks>
	/// If set to <see cref="Orientation.Vertical"/> the header will be above the content.
	/// If set to <see cref="Orientation.Horizontal"/> the header will be to the left of the content.
	/// </remarks>
	public Orientation Orientation
	{
		get { return (Orientation)GetValue(OrientationProperty); }
		set { SetValue(OrientationProperty,value); }
	}
	/// <summary>
	/// Gets or sets the data used for the header of each control.
	/// </summary>
	//public ImageSource Image
	//{

	//    set {  XamlImage.Source = value; }
	//}
	public string Text
	{
		get { return(string) GetValue(TextProperty);}
		set { SetValue(TextProperty,value); }
	}
	public ImageSource Image
        {

	get { return(ImageSource) GetValue(ImageProperty);}
		set { SetValue(ImageProperty,value); }
      }
		//public string ImageUri
  //      {
           
  //          set { XamlImage.Source = ImageHelper.FromImages(value) ; }
  //      }
		//public string Text
  //      {
  //          get { return (string)GetValue(TextProperty); }
  //          set { SetValue(TextProperty, value); }
  //      }
		//public ImageSource imageSource
  //      {
           
  //          get { return GetValue(ImageProperty) as ImageSource ?? ImageHelper.FromImages(GetValue(ImageUriProperty) as string ) ; }
  //      }
        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetOrientation();
        }

        

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (IconText)d;
            control.SetOrientation();
        }

  

        private void SetOrientation()
        {
            var orientation = this.Orientation == Orientation.Vertical
                ? nameof(Orientation.Vertical)
                : nameof(Orientation.Horizontal);

            VisualStateManager.GoToState(this, orientation, true);
        }
}
