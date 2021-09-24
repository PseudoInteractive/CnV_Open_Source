using Microsoft.UI.Xaml.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COTG.Views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using static COTG.Debug;
using COTG.Helpers;

namespace COTG.Models
{
	public class JSPopupNode
	{
		//	public string className { get; set; }
		//	public string src { get; set; }
		//	public string data { get; set; }
		//	public string type { get; set; }
		//	public string background { get; set; }
		//	public string backgroundPosition { get; set; }
		//	public string id { get; set; }
		////	public string style { get; set; }
		//	public string text { get; set; }
		//	public string onClick { get; set; }
		public int x0 { get; set; }
		public int y0 { get; set; }
		public int x1 { get; set; }
		public int y1 { get; set; }
		public int width => x1 - x0;
		public int height => y1 - y0;
		//	public JSPopupNode[] children { get; set; }
		//	public static TeachingTip[] existingPopups;

		
		public static void Show(JSPopupNode[] popups)
		{
			//if (existingPopups != null)
			//{
			//	foreach (var i in existingPopups)
			//	{
			//		i.IsOpen = false;
			//		ShellPage.instance.grid.Children.Remove(i);
			//	}
			//}
			//existingPopups = null;
			//existingPopups = new TeachingTip[popups.Length];
			//AGame.popups = new Span2i[popups.Length];
			int put = 0;
			var maxX = 0;
			var maxY = 0;
			foreach (var pop in popups)
			{
				// All controls should be relative to this 
				var x0 = pop.x0;
				var y0 = pop.y0;
				var x1 = pop.x1;
				var y1 = pop.y1;
				// maybe a date?
				//	if(y1-y0 > 180) // random guess
				var c1 = ShellPage.JSPointToScreen(x1,y1);
				maxX = maxX.Max(x0.Max(x1));
				maxY = maxY.Max(y0.Max(y1));


				var scale =  AGame.dipToNative; // ShellPage.webViewScale *
				//AGame.popups[put] = new Span2i(
				//	ShellPage.JSPointToScreen(x0, y0),
				//	ShellPage.JSPointToScreen(x1, y1));


				//	var canvas = new Microsoft.UI.Xaml.Controls.Canvas();
				//	canvas.Width = pop.x1 - x0;
				//	canvas.Height = pop.y1 - y0;





				//	const int tipMarginX = 32;
				//	const int tipMarginY = 32;
				////	canvas.Background = new Microsoft.UI.Xaml.Media.Brush();
				//	var tt = new TeachingTip()
				//	 {
				//		 Title = pop.id,
				//		 Content = canvas,
				//		 Width = pop.x1-x0 + tipMarginX,
				//		 Height=pop.y1-y0+tipMarginY,
				//		 IsOpen = true
				//	 };

				//	Add(canvas,x0,y0,1,pop);
				//	ShellPage.instance.grid.Children.Add(tt);
				//	existingPopups[put] = tt;
				++put;
			}
			ShellPage.UpdateWebViewOffsets(maxX,maxY);
			if (AGame.popups.Length == 0)
			{
				// ensure that the webview does not have focus
	//			ShellPage.SetWebViewHasFocus(false);
//				ShellPage.UpdateFocus();

			}
		}

		static Regex regexURl = new Regex(@"url\(([^\)]+)\)", RegexOptions.CultureInvariant | RegexOptions.Compiled);

		//		-384px -256px
		static Regex regexPX = new Regex(@"(-?\d+)px (-?\d+)px", RegexOptions.CultureInvariant | RegexOptions.Compiled);


		


			//private static void Add(Canvas canvas, int x0, int y0,int z0, JSPopupNode pop)
			//{
			//	//if (!pop.text.IsNullOrEmpty())
			//	{
			//		FrameworkElement control = null;
			//		if (!pop.background.IsNullOrEmpty())
			//		{
			//			var backgrounds = pop.background.Split(',');
			//			var backgroundPositions = pop.backgroundPosition.Split(',');
			//			z0 += backgrounds.Length;

			//			Assert(backgrounds.Length == backgroundPositions.Length);

			//			for(int backId=0;backId<backgrounds.Length;++backId)
			//			{

			//			// url("images/items.png") no-repeat -384px -256px, url("images/items.png") no-repeat -448px -320px

			//				var match = regexURl.Match(backgrounds[backId]);
			//				if (match.Success)
			//				{
			//					var bitmap = new BitmapImage();
			//					bitmap.UriSource = new Uri( match.Groups[1].Value);
			//					var brush = new ImageBrush() { ImageSource = bitmap, Stretch = Stretch.None
			//					};
			//					var image = new Microsoft.UI.Xaml.Shapes.Rectangle() { Fill=brush };

			//					var px = regexPX.Matches(backgroundPositions[backId]);
			//					if (px.Count > 0)
			//					{


			//						int offsetX = 0, offsetY = 0;
			//						try
			//						{
			//							int.TryParse(px[0].Groups[1].Value, out int cx0);
			//							int.TryParse(px[0].Groups[2].Value, out int cy0);
			//							//								int.TryParse(px[1].Groups[1].Value, out int cx1);
			//							//								int.TryParse(px[1].Groups[2].Value, out int cy1);
			//							//	offsetX = -cx0;
			//							//	offsetY = -cy0;
			//							brush.AlignmentX = AlignmentX.Left;
			//							brush.AlignmentY = AlignmentY.Top;

			//							brush.Transform = new TranslateTransform() { X = cx0 , Y = cy0 };

			//							//image.Clip = new RectangleGeometry() { Rect = new Windows.Foundation.Rect() { X = -cx0, Y = -cy0, Width = pop.width, Height = pop.height } };

			//						}
			//						catch (Exception _exception)
			//						{
			//							COTG.Debug.Log(_exception);
			//						}

			//						Canvas.SetLeft(image, pop.x0 - x0 - offsetX);
			//						Canvas.SetTop(image, pop.y0 - y0 - offsetY);

			//						image.Width = pop.width;
			//						image.Height = pop.height;


			//					}
			//					else
			//					{
			//						Canvas.SetLeft(image, pop.x0 - x0 );
			//						Canvas.SetTop(image, pop.y0 - y0 );

			//						image.Width = pop.width;
			//						image.Height = pop.height;
			//					}

			//					canvas.Children.Add(image);
			//					Canvas.SetZIndex(image, z0-1-backId);


			//				}

			//			}
			//			//var backGrounds = pop.background.Split(',');



			//		}
			//		if (pop.type== "BUTTON" )
			//			control = new Button() { Content = pop.text };

			//		if (control == null && !pop.text.IsNullOrEmpty())
			//		{ 
			//			control = new TextBlock() { Text = pop.text };
			//		}
			//		if (control != null)
			//		{
			//			control.Width = pop.width;
			//			control.Height = pop.height;
			//			canvas.Children.Add(control);
			//			Canvas.SetLeft(control, pop.x0 - x0);
			//			Canvas.SetTop(control, pop.y0 - y0);
			//			Canvas.SetZIndex(control, ++z0);

			//		}


			//	}
			//	++z0;
			//	foreach (var child in pop.children)
			//	{
			//		Add(canvas, x0, y0, z0,child);

			//	}
			//}
		}
}
