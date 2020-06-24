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
using System.Drawing.Drawing2D;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using COTG.Helpers;

namespace COTG.Views
{
    public partial class ShellPage
    {
        public static Vector2 clientSpan;
        public static Vector2 clientTL;
        public static Vector2 cameraC;



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
            canvas = new CanvasControl();
            canvas.Draw += Canvas_Draw;
            canvas.IsHitTestVisible = false;
            canvas.Unloaded += Canvas_Unloaded;
         
            return canvas;

        }

        private void Canvas_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
              // Explicitly remove references to allow the Win2D controls to get garbage collected
             canvas.RemoveFromVisualTree();
              canvas = null;
        }

        private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            dxy.X = (float)sender.ActualWidth;
            dxy.Y = (float)sender.ActualHeight;

            ds.DrawLine( SC(0.25f,.125f),SC(0.625f,0.9f), Colors.DarkMagenta, 8,defaultStrokeStyle);
            ds.DrawLine(SC(0.25f, .125f), SC(0.9f, 0.625f), Colors.AliceBlue, 8, defaultStrokeStyle);
            foreach(var city in City.all)
            {
                var c = city.Value.cid.ToWorldC() ;
                c -= cameraC;
                c.X = canvas.ConvertPixelsToDips(c.X.RoundToInt());
                c.Y = canvas.ConvertPixelsToDips(c.Y.RoundToInt());
                ds.DrawCircle(c, 64, Colors.Magenta);

            }

        }
    }
}
