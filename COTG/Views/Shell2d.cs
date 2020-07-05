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
using COTG.Services;
using Microsoft.Graphics.Canvas.Text;

namespace COTG.Views
{
    public partial class ShellPage
    {
        public static CanvasBitmap worldBackground;
        public static CanvasBitmap worldObjects;
        public static Vector2 clientTL;
        public static Vector2 cameraC;
        public static Vector2 clientC;
        public static Vector2 clientSpan;
        public static Vector2 cameraMid;
        public static float cameraZoom;
        public float eventTimeOffset;
        public float eventTimeEnd;
        static CanvasSolidColorBrush raidBrush, shadowBrush;
        static CanvasLinearGradientBrush tipBackgroundBrush,tipTextBrush;
        static CanvasTextFormat tipTextFormat = new CanvasTextFormat() { FontSize=14};// { HorizontalAlignment=CanvasHorizontalAlignment.Center,VerticalAlignment=CanvasVerticalAlignment.Center};

        static public CanvasAnimatedControl canvas;

        CanvasStrokeStyle defaultStrokeStyle = new CanvasStrokeStyle() { CustomDashStyle=new float[] { 2, 6 },
            DashCap=CanvasCapStyle.Triangle,
            EndCap=CanvasCapStyle.Triangle,
            StartCap=CanvasCapStyle.Triangle};
        public CanvasAnimatedControl CreateCanvasControl()
		{
			canvas = new CanvasAnimatedControl()
			{
				IsHitTestVisible = false,
                
				TargetElapsedTime=TimeSpan.FromSeconds(1.0f/15.0f),
				
				IsFixedTimeStep = false
			};
			canvas.Draw += Canvas_Draw;
			canvas.Unloaded += Canvas_Unloaded;
            canvas.LayoutUpdated += Canvas_LayoutUpdated;
			canvas.SizeChanged += Canvas_SizeChanged;
			canvas.CreateResources += Canvas_CreateResources;
			return canvas;

		}

       
        private void Canvas_LayoutUpdated(object sender, object e)
        {
            var c = canvas.ActualOffset;
            clientC = new Vector2(c.X,c.Y); 
            clientSpan = canvas.ActualSize;
        }
        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            clientSpan = e.NewSize.ToVector2();
        }

        async private void Canvas_CreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
		{
            worldBackground = await CanvasBitmap.LoadAsync(canvas, new Uri($"ms-appx:///Assets/world.png"));
            while (JSClient.cid == 0)
                await Task.Delay(5 * 1000);
         //   var ob = World.CreateBitmap();
         //   worldObjects = CanvasBitmap.CreateFromBytes(canvas, ob.pixels, ob.size, ob.size, Windows.Graphics.DirectX.DirectXPixelFormat.BC1UIntNormalized);
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

        const float lineThickness = 4.0f;
        const float rectSpanMin = 4.0f;
        const float rectSpanMax = 6.0f;
        const float bSizeGain = 4.0f;
        const float bSizeGain2 = 4;//4.22166666666667f;
        const float srcImageSpan = 2400;
        const float bSizeGain3 = bSizeGain* bSizeGain / bSizeGain2;
        public static float pixelScale=1;
        const float dashLength = (1 + 3) * lineThickness;
        static Vector2 shadowOffset = new Vector2(lineThickness*0.75f, lineThickness*0.75f);
        private void Canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
		{
            if(World.bitmapPixels!=null)
            {
                if (worldObjects != null)
                {
                    var w = worldObjects;
                    worldObjects = null;
                    w.Dispose();
                }
                var pixels = World.bitmapPixels;
                World.bitmapPixels = null;
                worldObjects = CanvasBitmap.CreateFromBytes(canvas, pixels, World.outSize, World.outSize, Windows.Graphics.DirectX.DirectXPixelFormat.BC1UIntNormalized);


            }
            if (!(IsWorldView() || IsRegionView()))
                return;

            try
            {
                var serverNow = JSClient.ServerTime() + TimeSpan.FromMinutes(eventTimeOffset);
                float animT = ((uint)Environment.TickCount % 3000) * (1.0f / 3000);
                var animTLoop = animT.Wave();
                var rectSpan = animTLoop.Lerp(rectSpanMin, rectSpanMax);
                //   ShellPage.T("Draw");
                if (shadowBrush == null)
                {
                    raidBrush = new CanvasSolidColorBrush(canvas, Colors.BlueViolet);
                    shadowBrush = new CanvasSolidColorBrush(canvas, new Color() { A = 255, G = 64, B = 64, R = 64 }) {Opacity = 0.5f };
                    tipBackgroundBrush = new CanvasLinearGradientBrush(canvas, new CanvasGradientStop[]
                    {
                        new CanvasGradientStop() { Position = 0.0f, Color = Colors.Gray },
                        new CanvasGradientStop() { Position = 1.0f, Color = Colors.Black } })
                    { Opacity = 0.5f };
                    tipTextBrush = new CanvasLinearGradientBrush(canvas, new CanvasGradientStop[]
                    {
                        new CanvasGradientStop() { Position = 0.0f, Color = Colors.White },
                        new CanvasGradientStop() { Position = 1.0f, Color = Colors.Blue }
                    });
                   ;

                }
                defaultStrokeStyle.DashOffset = (1 - animT) * dashLength;

                var ds = args.DrawingSession;

                if (worldBackground != null && IsWorldView())
                {
                    var srcP0 = new Point(cameraC.X * bSizeGain2+ clientC.X * bSizeGain2 / pixelScale, cameraC.Y * bSizeGain2+ clientC.Y * bSizeGain2 / pixelScale);
                    var srcP1 = new Point(srcP0.X + clientSpan.X * bSizeGain2 / pixelScale,
                                           srcP0.Y + clientSpan.Y * bSizeGain2 / pixelScale);
                    var destP0 = new Point();
                    var destP1 = clientSpan.ToPoint();

                    if (srcP0.X < 0)
                    {
                        destP0.X -= srcP0.X * pixelScale / bSizeGain2;
                        srcP0.X = 0;
                    }
                    if (srcP0.Y < 0)
                    {
                        destP0.Y -= srcP0.Y * pixelScale / bSizeGain2;
                        srcP0.Y = 0;
                    }
                    if (srcP1.X > srcImageSpan)
                    {
                        destP1.X += (srcImageSpan - srcP1.X) * pixelScale / bSizeGain2;
                        srcP1.X = srcImageSpan;

                    }
                    if (srcP1.Y > srcImageSpan)
                    {
                        destP1.Y += (srcImageSpan - srcP1.Y) * pixelScale / bSizeGain2;
                        srcP1.Y = srcImageSpan;

                    }

                    ds.DrawImage(worldBackground,
                        new Rect(destP0, destP1),
                        new Rect(srcP0, srcP1));
                    if(worldObjects != null)
                        ds.DrawImage(worldObjects,
                            new Rect(destP0, destP1),
                            new Rect(srcP0, srcP1));
                }
                ds.Antialiasing = CanvasAntialiasing.Antialiased;
                var scale = ShellPage.canvas.ConvertPixelsToDips(1);
                pixelScale = scale * (cameraZoom);
                // ds.Transform = new Matrix3x2( _gain, 0, 0, _gain, -_gain * ShellPage.cameraC.X, -_gain * ShellPage.cameraC.Y );

                //           dxy.X = (float)sender.Width;
                //            dxy.Y = (float)sender.ActualHeight;

                //            ds.DrawLine( SC(0.25f,.125f),SC(0.lineThickness,0.9f), raidBrush, lineThickness,defaultStrokeStyle);
                //           ds.DrawLine(SC(0.25f, .125f), SC(0.9f, 0.lineThickness), shadowBrush, lineThickness, defaultStrokeStyle);
                if (Attack.attacks != null)
                {
                    const float postAttackDisplayTime = 11*60; // 5 min
                    foreach (var attack in Attack.attacks)
                    {
                        if (selectedTarget != 0 && selectedTarget != attack.targetCid)
                            continue;
                        var c1 = attack.targetCid.ToWorldC().WToC();
                        var c0 = attack.sourceCid.ToWorldC().WToC();
                        // cull (should do this pre-transform as that would be more efficient
                        if (c0.X.Min(c1.X) >= clientSpan.X)
                            continue;
                        if (c0.X.Max(c1.X) <= 0.0f)
                            continue;
                        if (c0.Y.Min(c1.Y) >= clientSpan.Y)
                            continue;
                        if (c0.Y.Max(c1.Y) <= 0.0f)
                            continue;
                        var dt0 = (float)(serverNow - attack.spotted).TotalSeconds;

                        // before attack
                        var dt1 = (float)(attack.time - serverNow).TotalSeconds;
                        if (dt0 <= 0 || dt1 < -postAttackDisplayTime)
                            continue;


                        var progress = (dt0 / (dt0+dt1).Max(1)).Saturate(); // we don't know the duration so we approximate with 2 hours
                        var mid = progress.Lerp(c0, c1);
                        var span = rectSpan;
                        if (dt1 < postAttackDisplayTime)
                            span *= 2.5f;
                        ds.DrawLine(c0, c1, shadowBrush, lineThickness, defaultStrokeStyle);
                        ds.FillCircle(mid ,  span, shadowBrush);
                        var midS = mid - shadowOffset;
                        ds.DrawLine(c0 - shadowOffset, midS, raidBrush, lineThickness, defaultStrokeStyle);
                        ds.FillCircle(midS, span, raidBrush);
                    }
                }
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
                        ds.FillCircle(mid, rectSpan,  shadowBrush);
                        var midS = mid - shadowOffset;
                        ds.DrawLine(c0 - shadowOffset, midS, raidBrush, lineThickness, defaultStrokeStyle);
                        ds.FillCircle(midS,rectSpan, raidBrush);

                    }
                }
                var _toolTip =toolTip;
                if(_toolTip != null)
                {
                    var rectD = new Vector2(32*4, 24*5);
                    var target = new Rect((mousePosition + rectD*0.25f).ToPoint(), rectD.ToSize());
                    tipTextBrush.StartPoint = tipBackgroundBrush.StartPoint = mousePosition;
                    tipTextBrush.EndPoint = tipBackgroundBrush.EndPoint = mousePosition + rectD * 1.5f;
                    ds.FillRoundedRectangle(target, 8, 8, tipBackgroundBrush);
                    target.X+= 12;
                    target.Y += 8;

                    ds.DrawText(_toolTip, target, tipTextBrush, tipTextFormat);
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }



        }

		private static bool IsWorldView()
		{
			return JSClient.IsWorldView();
		}
        private static bool IsRegionView()
        {
            return JSClient.IsRegionView();
        }
        private static bool IsCityView()
        {
            return JSClient.IsCityView();
        }
    }
    public static class CanvasHelpers
    {
        public static Vector2 WToC(this Vector2 c)
        {
            return (c - ShellPage.cameraC) * ShellPage.pixelScale - ShellPage.clientC;
        }
        public static float WToCx(this float c)
        {
            return 
                (c- ShellPage.cameraC.X) * ShellPage.pixelScale - ShellPage.clientC.X;
        }
        public static float WToCy(this float c)
        {
            return
                (c - ShellPage.cameraC.Y) * ShellPage.pixelScale - ShellPage.clientC.Y;
        }
      
    }
}

