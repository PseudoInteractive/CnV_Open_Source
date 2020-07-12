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
        static public CanvasSolidColorBrush raidBrush, shadowBrush;
        static CanvasLinearGradientBrush tipBackgroundBrush,tipTextBrush;
        static CanvasTextFormat tipTextFormat = new CanvasTextFormat() { FontSize=14, WordWrapping = CanvasWordWrapping.NoWrap };
        static CanvasTextFormat tipTextFormatCentered = new CanvasTextFormat() { FontSize = 12, HorizontalAlignment=CanvasHorizontalAlignment.Center,VerticalAlignment=CanvasVerticalAlignment.Center,WordWrapping=CanvasWordWrapping.NoWrap};

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

        public static void SetCanvasVisibility(bool visible)
		{
            if ( canvas.Visibility == Visibility.Visible )
			{
                if (!visible)
                    canvas.Visibility = Visibility.Collapsed;
            }
            else
			{
                if (visible)
                    canvas.Visibility = Visibility.Visible;
            }


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

        class IncomingCounts
        {
            public int prior;
            public int incoming;
        };

        const float postAttackDisplayTime = 11 * 60; // 11 min

        const float circleRadMin = 3.0f;
        const float circleRadMax = 5.5f;
        const float lineThickness = 4.0f;
        const float rectSpanMin = 2.0f;
        const float rectSpanMax = 8.0f;
        const float bSizeGain = 4.0f;
        const float bSizeGain2 = 4;//4.22166666666667f;
        const float srcImageSpan = 2400;
        const float bSizeGain3 = bSizeGain* bSizeGain / bSizeGain2;
        public static float pixelScale=1;
        const float dashLength = (1 + 3) * lineThickness;
        public static Vector2 shadowOffset = new Vector2(lineThickness*0.75f, lineThickness*0.75f);
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
                int tick = (Environment.TickCount>>3) & 0xfffff;
                var animTLoop = animT.Wave();
                var rectSpan = animTLoop.Lerp(rectSpanMin, rectSpanMax);
                //   ShellPage.T("Draw");
                if (shadowBrush == null)
                {
                    raidBrush = new CanvasSolidColorBrush(canvas, Colors.BlueViolet);
                    shadowBrush = new CanvasSolidColorBrush(canvas, new Color() { A = 255, G = 64, B = 64, R = 64 }) {Opacity = 0.675f };
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
           //     ds.Antialiasing = CanvasAntialiasing.Antialiased;
                var scale = ShellPage.canvas.ConvertPixelsToDips(1);
                pixelScale = scale * (cameraZoom);
                var circleRadBase = circleRadMin * MathF.Sqrt(pixelScale);
                var circleRadius = animTLoop.Lerp(circleRadMin, circleRadMax) * MathF.Sqrt(pixelScale);
                var highlightRectSpan = new Vector2(circleRadius * 2.0f, circleRadius * 2);
                // ds.Transform = new Matrix3x2( _gain, 0, 0, _gain, -_gain * ShellPage.cameraC.X, -_gain * ShellPage.cameraC.Y );

                //           dxy.X = (float)sender.Width;
                //            dxy.Y = (float)sender.ActualHeight;

                //            ds.DrawLine( SC(0.25f,.125f),SC(0.lineThickness,0.9f), raidBrush, lineThickness,defaultStrokeStyle);
                //           ds.DrawLine(SC(0.25f, .125f), SC(0.9f, 0.lineThickness), shadowBrush, lineThickness, defaultStrokeStyle);
                if (IsPageDefense())
                {
                    if (Attack.attacks != null)
                    {
                       
                        var counts = new Dictionary<int, IncomingCounts> ();
                        foreach (var attack in Attack.attacks)
                        {
                            var targetCid = attack.defCid;
                            var c1 = targetCid.ToWorldC().WToC();
                            var c0 = attack.atkCid.ToWorldC().WToC();
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
							{
                                // register attack
                                if(!counts.TryGetValue(targetCid, out var count) )
								{
                                    count = new IncomingCounts();
                                    counts.Add(targetCid, count);
								}
                                if (dt1 > 0)
                                    ++count.incoming;
                                else
                                    ++count.prior;
							}

                            if (dt0 <= 0 || dt1 < -postAttackDisplayTime)
                                continue;
                            if (Spot.AreAnySelected() && !Spot.IsSelectedOrHovered(targetCid))
                                continue;
                           
                            DrawAction(serverNow, ds, rectSpan, attack.time, attack.time-attack.spotted, c0, c1);
                            //var progress = (dt0 / (dt0 + dt1).Max(1)).Saturate(); // we don't know the duration so we approximate with 2 hours
                            //var mid = progress.Lerp(c0, c1);
                            //ds.DrawLine(c0, c1, shadowBrush, lineThickness, defaultStrokeStyle);
                            //ds.FillCircle(mid, span, shadowBrush);
                            //var midS = mid - shadowOffset;
                            //ds.DrawLine(c0 - shadowOffset, midS, raidBrush, lineThickness, defaultStrokeStyle);
                            //ds.FillCircle(midS, span, raidBrush);
                        }
                        foreach(var i in counts)
						{
                            var cid = i.Key;
                            var count = i.Value;
                            var c = cid.ToWorldC().WToC();
                            DrawTextBox(ds,$"{count.prior},{count.incoming}", c, tipTextFormatCentered);


                        }
                    }
                }
                if (IsPageRaid())
                {
                    foreach (var city in City.all.Values)
                    {
                        var c = city.cid.ToWorldC().WToC();
                       // var t = (tick * city.cid.CidToRandom().Lerp(1.375f / 512.0f, 1.75f / 512f));
                        //var r = t.Wave().Lerp(circleRadBase, circleRadBase * 1.325f);
                        //ds.DrawRoundedSquareWithShadow(c,r, raidBrush);
                        foreach (var raid in city.raids)
                        {
                            var ct = raid.target.ToWorldC().WToC();
                            (var c0, var c1) = !raid.isReturning ? (c, ct) : (ct, c);
                            DrawAction(serverNow, ds, rectSpan, raid.arrival,TimeSpan.FromHours(2), c0, c1);

                        }
                        if (city.senatorInfo.Length != 0)
                        {
                            var idle = 0;
                            var active = 0;
                            var recruiting=0;
                            foreach (var sen in city.senatorInfo) 
                            {
                                if (sen.type == SenatorInfo.Type.idle)
                                    idle += sen.count;
                                else if (sen.type == SenatorInfo.Type.recruit)
                                    recruiting += sen.count;
                                else
                                    active += sen.count;
                                if(sen.target != 0)
                                {
                                    var c1 = sen.target.ToWorldC().WToC();
                                    DrawAction(serverNow, ds, rectSpan, sen.time, TimeSpan.FromHours(2), c, c1);
                                }
                            }
                            DrawTextBox(ds, $"{recruiting},{idle},{active}", c, tipTextFormatCentered);

                        }
                    }
                }
                if(!IsCityView())
                {
                    foreach(var city in Spot.GetSelected())
                    {
                        var c = city.ToWorldC().WToC();
                        var t = (tick * city.CidToRandom().Lerp(1.5f / 512.0f, 1.75f / 512f))+0.25f;
                        var r = t.Wave().Lerp(circleRadBase, circleRadBase * 1.325f);
                        ds.DrawRoundedSquareWithShadow(c, r, raidBrush);

                    }

                }
                // show selected
                var _toolTip =toolTip;
                if(_toolTip != null)
                {
                    CanvasTextLayout textLayout = new CanvasTextLayout(ds, _toolTip, tipTextFormat, 0.0f, 0.0f);
                    var bounds = textLayout.DrawBounds;
                    Vector2 c = mousePosition + new Vector2(16, 16);
                    const float expand = 7;
                    bounds.X += c.X - expand;
                    bounds.Y += c.Y - expand;
                    bounds.Width += expand * 2;
                    bounds.Height += expand * 2;

                    //  var rectD = new Vector2(32*4, 24*5);
                    // var target = new Rect((mousePosition + rectD*0.25f).ToPoint(), rectD.ToSize());
                    tipTextBrush.StartPoint = tipBackgroundBrush.StartPoint = new Vector2((float)bounds.Left, (float)bounds.Top);
                    tipTextBrush.EndPoint = tipBackgroundBrush.EndPoint = new Vector2((float)bounds.Right,(float)bounds.Bottom);
                    ds.FillRoundedRectangle(bounds, 8, 8, tipBackgroundBrush);
//                    target.X+= 12;
  //                  target.Y += 8;

                    ds.DrawText(_toolTip, c, tipTextBrush, tipTextFormat);
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }



        }
        private static void DrawTextBox(CanvasDrawingSession ds, string text, Vector2 at, CanvasTextFormat format)
        {
            float xLoc = at.X;
            float yLoc = at.Y;
            CanvasTextLayout textLayout = new CanvasTextLayout(ds, text, format, 0.0f, 0.0f);
            var bounds = textLayout.DrawBounds;
            const float expand = 4;
            bounds.X += at.X -expand;
            bounds.Y += at.Y -expand;
            bounds.Width += expand * 2;
            bounds.Height += expand * 2;
            ds.FillRoundedRectangle(bounds,3,3, shadowBrush);
            ds.DrawTextLayout(textLayout, at.X,at.Y, Colors.White );
        }

        private void DrawAction(DateTimeOffset serverNow, CanvasDrawingSession ds, float rectSpan, DateTimeOffset arrival,TimeSpan dt, Vector2 c0, Vector2 c1)
		{
            var dt1 = (float)(arrival - serverNow).TotalSeconds;
            var progress = (1f-(float)(dt1 /dt.TotalSeconds )).Max(0.125f).Min(1.0f); // we don't know the duration so we approximate with 2 hours
            if (dt1 < postAttackDisplayTime)
                rectSpan *= 2.0f;
            var mid = progress.Lerp(c0, c1);
            ds.DrawLine(c0, c1, shadowBrush, lineThickness, defaultStrokeStyle);
            ds.DrawRoundedSquare(mid, rectSpan, shadowBrush,2.0f);
            var midS = mid - shadowOffset;
            ds.DrawLine(c0 - shadowOffset, midS, raidBrush, lineThickness, defaultStrokeStyle);
            ds.DrawRoundedSquare(midS, rectSpan, raidBrush, 2.0f) ;
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
        public static void DrawRoundedSquare(this CanvasDrawingSession ds, Vector2 c, float circleRadius, ICanvasBrush brush,float thickness = 1.5f)
        {
            ds.DrawRoundedRectangle(c.X - circleRadius, c.Y - circleRadius, circleRadius*2,circleRadius*2, circleRadius*0.25f, circleRadius*0.25f, brush, thickness);
        }
        public static void DrawRoundedSquareWithShadow(this CanvasDrawingSession ds, Vector2 c, float circleRadius, ICanvasBrush brush, float thickness = 1.5f)
        {
            DrawRoundedSquareShadow(ds, c, circleRadius, thickness);
            DrawRoundedSquareBase(ds, c, circleRadius, brush, thickness);
        }
        public static void DrawRoundedSquareShadow(this CanvasDrawingSession ds, Vector2 c, float circleRadius,  float thickness = 1.5f)
        {
            DrawRoundedSquare(ds, c, circleRadius, ShellPage.shadowBrush, thickness);
        }
        public static void DrawRoundedSquareBase(this CanvasDrawingSession ds, Vector2 c, float circleRadius, ICanvasBrush brush, float thickness = 1.5f)
        {
            DrawRoundedSquare(ds, c - ShellPage.shadowOffset, circleRadius, brush, thickness);
        }

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
        public static void BringCidIntoWorldView(this int cid)
        {
            var v = cid.CidToWorldV();
            var newC = v - (ShellPage.clientC+ShellPage.clientSpan*0.5f) / ShellPage.pixelScale;
            var dc = newC - ShellPage.cameraC;
            // only move if needed
            if (dc.X.Abs() * ShellPage.pixelScale > ShellPage.clientSpan.X * 0.5f ||
                dc.Y.Abs() * ShellPage.pixelScale > ShellPage.clientSpan.Y * 0.5f)
            {

                ShellPage.cameraC = newC;
                JSClient.SetJSCamera();
            }


        }
      
    }
}

