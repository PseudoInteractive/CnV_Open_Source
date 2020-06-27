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

namespace COTG.Views
{
    public partial class ShellPage
    {
        public static Vector2 clientSpan;
        public static Vector2 clientTL;
        public static Vector2 cameraC;

        static CanvasSolidColorBrush  raidBrush,shadowBrush;

        static public CanvasAnimatedControl canvas;

        static Vector2 dxy;
        static Vector2 SC(float x, float y)
        {
            return new Vector2(x, y) * dxy;
        }
        CanvasStrokeStyle defaultStrokeStyle = new CanvasStrokeStyle() { DashStyle=CanvasDashStyle.Dash,
            DashCap=CanvasCapStyle.Triangle,
            EndCap=CanvasCapStyle.Triangle,
            StartCap=CanvasCapStyle.Triangle};
        public CanvasAnimatedControl CreateCanvasControl()
        {
            canvas = new CanvasAnimatedControl() { IsHitTestVisible = false };
            canvas.Draw += Canvas_Draw;

            canvas.Unloaded += Canvas_Unloaded;

            return canvas;

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
        const float lineThickness = 6.0f;
        const float dashLength = (2 + 2) * lineThickness;
        static int counter;
        private void Canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
          
            float animT = DateTime.Now.Millisecond*.001f;
         //   ShellPage.T("Draw");
            if(shadowBrush==null)
            {
             raidBrush =new CanvasSolidColorBrush(canvas,Colors.BlueViolet);
            shadowBrush =new CanvasSolidColorBrush(canvas,Colors.Black) { Opacity=0.5f };

            }
            defaultStrokeStyle.DashOffset = animT* dashLength;
            var ds = args.DrawingSession;
            var scale = ShellPage.canvas.ConvertPixelsToDips(1);
            ds.Transform = new Matrix3x2( scale, 0, 0, scale, -scale * ShellPage.cameraC.X, -scale * ShellPage.cameraC.Y );

 //           dxy.X = (float)sender.Width;
//            dxy.Y = (float)sender.ActualHeight;

//            ds.DrawLine( SC(0.25f,.125f),SC(0.lineThickness,0.9f), raidBrush, lineThickness,defaultStrokeStyle);
 //           ds.DrawLine(SC(0.25f, .125f), SC(0.9f, 0.lineThickness), shadowBrush, lineThickness, defaultStrokeStyle);
            foreach(var city in City.all)
            {
                var c = city.Value.cid.ToWorldC() ;
              
                ds.DrawCircle(c, 28+32*animT, Colors.Magenta);
                foreach(var raid in city.Value.raids)
                {
                    ds.DrawLine(c, raid.target.ToWorldC(), raidBrush,lineThickness, defaultStrokeStyle);
                }
            }

        }
    }
    public static class CanvasHelpers
    {
        public static Vector2 WToC(this Vector2 c)
        {
            return new Vector2(ShellPage.canvas.ConvertPixelsToDips((c.X - ShellPage.cameraC.X).RoundToInt()),
                ShellPage.canvas.ConvertPixelsToDips((c.Y - ShellPage.cameraC.Y).RoundToInt()));
        }
    }
}
