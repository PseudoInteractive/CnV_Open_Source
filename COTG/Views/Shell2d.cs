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

using Windows.UI;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using COTG.Helpers;
using Microsoft.Graphics.Canvas.Brushes;

namespace COTG.Views
{
    public partial class ShellPage
    {
        public static Vector2 clientSpan;
        public static Vector2 clientTL;
        public static Vector2 cameraC;

        static CanvasSolidColorBrush  raidBrush,shadowBrush;

        static public CanvasControl canvas;
        static Vector2 dxy;
        static Vector2 SC(float x, float y)
        {
            return new Vector2(x, y) * dxy;
        }
        CanvasStrokeStyle defaultStrokeStyle = new CanvasStrokeStyle() { DashStyle=CanvasDashStyle.Dash,
            DashCap=CanvasCapStyle.Triangle,
            EndCap=CanvasCapStyle.Triangle,
            StartCap=CanvasCapStyle.Triangle};

        public CanvasControl CreateCanvasControl()
        {
            canvas = new CanvasControl() { IsHitTestVisible=false };
            canvas.Draw += Canvas_Draw;
           
            canvas.Unloaded += Canvas_Unloaded;
           
            return canvas;

        }

        private void Canvas_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
              // Explicitly remove references to allow the Win2D controls to get garbage collected
             canvas.RemoveFromVisualTree();
              canvas = null;
        }
      
        static void DrawLine(CanvasDrawingSession ds, Vector2 c0,Vector2 c1)
        {

        }
        private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            ShellPage.T("Draw");
            if(shadowBrush==null)
            {
             raidBrush =new CanvasSolidColorBrush(canvas,Colors.BlueViolet);
            shadowBrush =new CanvasSolidColorBrush(canvas,Colors.Black) { Opacity=0.5f };

            }
            var ds = args.DrawingSession;
            var scale = ShellPage.canvas.ConvertPixelsToDips(1);
            ds.Transform = new Matrix3x2( scale, 0, 0, scale, -scale * ShellPage.cameraC.X, -scale * ShellPage.cameraC.Y );

            dxy.X = (float)sender.ActualWidth;
            dxy.Y = (float)sender.ActualHeight;

            ds.DrawLine( SC(0.25f,.125f),SC(0.625f,0.9f), raidBrush, 8,defaultStrokeStyle);
            ds.DrawLine(SC(0.25f, .125f), SC(0.9f, 0.625f), shadowBrush, 8, defaultStrokeStyle);
            foreach(var city in City.all)
            {
                var c = city.Value.cid.ToWorldC() ;
              
                ds.DrawCircle(c, 32, Colors.Magenta);

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
