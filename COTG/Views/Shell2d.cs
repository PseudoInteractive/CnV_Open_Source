using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using COTG.Game;
using static COTG.Debug;
using COTG.Helpers;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Core;
using Microsoft.Graphics.Canvas.Svg;

namespace COTG.Views
{
    public partial class ShellPage
    {
        public static CanvasBitmap worldBackground;
        public static Vector2 clientSpan;
        public static Vector2 clientTL;
        public static Vector2 cameraC;
        public static Vector2 halfSpan;
        public static Vector2 cameraMid;
        public static float cameraZoom;

        static CanvasSolidColorBrush  raidBrush,shadowBrush;

        static public CanvasAnimatedControl canvas;

        static Vector2 dxy;
        static Vector2 SC(float x, float y)
        {
            return new Vector2(x, y) * dxy;
        }
        CanvasStrokeStyle defaultStrokeStyle = new CanvasStrokeStyle() { CustomDashStyle=new float[] { 2, 4 },
            DashCap=CanvasCapStyle.Triangle,
            EndCap=CanvasCapStyle.Triangle,
            StartCap=CanvasCapStyle.Triangle};
        public CanvasAnimatedControl CreateCanvasControl()
		{
			canvas = new CanvasAnimatedControl()
			{
				IsHitTestVisible = false
				//,TargetElapsedTime=TimeSpan.FromSeconds(1.0f/15.0f)
				,
				IsFixedTimeStep = false
			};
			canvas.Draw += Canvas_Draw;

			canvas.Unloaded += Canvas_Unloaded;
			canvas.SizeChanged += Canvas_SizeChanged;
			canvas.CreateResources += Canvas_CreateResources;
			return canvas;

		}

		async private void Canvas_CreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
		{
            worldBackground = await CanvasBitmap.LoadAsync(canvas, new Uri($"ms-appx:///Assets/world.png"));
		}


		private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            halfSpan.X = (float)e.NewSize.Width * 0.5f;
            halfSpan.Y = (float)e.NewSize.Height * 0.5f;
        }

        //private void rootCanvas_LayoutUpdated()
        //{
        //    if (shellFrame != null)
        //    {
        //        var off = shellFrame.ActualOffset;
        //        var size = shellFrame.ActualSize;

        //        var cc = canvasControl;
        //        var x = Canvas.GetLeft(cc);
        //        var y = Canvas.GetTop(cc);
        //        if (x != off.X || y != off.Y)
        //        {
        //            Canvas.SetLeft(cc, off.X);
        //            Canvas.SetTop(cc, off.Y);
        //            cc.InvalidateArrange();
        //        }
        //        if (size.X != cc.Width || size.Y != cc.Height)
        //        {
        //            cc.Width = size.X;
        //            cc.Height = size.Y;

        //        }
        //    }
        //}


        private void Canvas_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
              // Explicitly remove references to allow the Win2D controls to get garbage collected
             canvas.RemoveFromVisualTree();
          
        }
      
        static void DrawLine(CanvasDrawingSession ds, Vector2 c0,Vector2 c1)
        {

        }
        const float lineThickness = 8.0f;
        const float rectSpanMin = 8.0f;
        const float rectSpanMax = 20.0f;
        const float bSizeGain = 4.22166666666667f;
        public static float pixelScale=1;
        const float dashLength = (1 + 2) * lineThickness;
        static Vector2 shadowOffset = new Vector2(lineThickness*0.375f, lineThickness*0.375f);
        private void Canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
		{


			var serverNow = JSClient.ServerTime();
			float animT = ((uint)Environment.TickCount % 3000) * (1.0f / 3000);
			var animTLoop = animT.Wave();
			var rectSpan = animTLoop.Lerp(rectSpanMin, rectSpanMax);
			//   ShellPage.T("Draw");
			if (shadowBrush == null)
			{
				raidBrush = new CanvasSolidColorBrush(canvas, Colors.BlueViolet);
				shadowBrush = new CanvasSolidColorBrush(canvas, Colors.Black) { Opacity = 0.5f };

			}
			defaultStrokeStyle.DashOffset = (1 - animT) * dashLength;

			var ds = args.DrawingSession;
			if (worldBackground != null && IsWorldView())
				ds.DrawImage(worldBackground,
					new Rect(0, 0, halfSpan.X * 2, halfSpan.Y * 2),
					new Rect(cameraC.X * bSizeGain, cameraC.Y * bSizeGain, halfSpan.X * 2 * bSizeGain / pixelScale, halfSpan.Y * 2* bSizeGain / pixelScale));

			ds.Antialiasing = CanvasAntialiasing.Antialiased;
			var scale = ShellPage.canvas.ConvertPixelsToDips(1);
			pixelScale = scale * (cameraZoom);
			// ds.Transform = new Matrix3x2( _gain, 0, 0, _gain, -_gain * ShellPage.cameraC.X, -_gain * ShellPage.cameraC.Y );

			//           dxy.X = (float)sender.Width;
			//            dxy.Y = (float)sender.ActualHeight;

			//            ds.DrawLine( SC(0.25f,.125f),SC(0.lineThickness,0.9f), raidBrush, lineThickness,defaultStrokeStyle);
			//           ds.DrawLine(SC(0.25f, .125f), SC(0.9f, 0.lineThickness), shadowBrush, lineThickness, defaultStrokeStyle);
			foreach (var city in City.all)
			{
				var c = city.Value.cid.ToWorldC().WToC();

				ds.DrawCircle(c, 28 + 32 * animTLoop, raidBrush);
				foreach (var raid in city.Value.raids)
				{
					var ct = raid.target.ToWorldC().WToC();
					(var c0, var c1) = !raid.isReturning ? (c, ct) : (ct, c);
					var progress = (1.0f - ((float)(raid.arrival - serverNow).TotalHours) * 0.5f).Max(0.125f); // we don't know the duration so we approximate with 2 hours
					var mid = progress.Lerp(c0, c1);
					ds.DrawLine(c0, c1, shadowBrush, lineThickness, defaultStrokeStyle);
					ds.FillRectangle(mid.X - rectSpan * 0.5f, mid.Y - rectSpan * 0.5f, rectSpan, rectSpan, shadowBrush);
					var midS = mid - shadowOffset;
					ds.DrawLine(c0 - shadowOffset, midS, raidBrush, lineThickness, defaultStrokeStyle);
					ds.FillRectangle(midS.X - rectSpan * 0.5f, midS.Y - rectSpan * 0.5f, rectSpan, rectSpan, raidBrush);

				}
			}

		}

		private static bool IsWorldView()
		{
			return JSClient.IsWorldView();
		}
	}
    public static class CanvasHelpers
    {
        public static Vector2 WToC(this Vector2 c)
        {
            return new Vector2(
               WToCx(c.X), WToCy(c.Y));
        }
        public static float WToCx(this float c)
        {
            return 
                (c- ShellPage.cameraC.X -ShellPage.halfSpan.X * 0) * ShellPage.pixelScale  + ShellPage.halfSpan.X*0;
        }
        public static float WToCy(this float c)
        {
            return
                (c - ShellPage.cameraC.Y - ShellPage.halfSpan.Y * 0) * ShellPage.pixelScale  + ShellPage.halfSpan.Y * 0;
        }
    }
}
