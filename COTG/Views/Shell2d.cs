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
using COTG.JSON;
using Microsoft.Graphics.Canvas.Effects;
using static COTG.Game.Enum;
namespace COTG.Views
{
    public partial class ShellPage
    {
        const float detailsZoomThreshold = 36;
        const float detailsZoomFade = 8;
        public static CanvasBitmap worldBackground;
        public static TintEffect worldBackgroundDark;
        public static CanvasBitmap worldObjects;
        //     public static TintEffect worldObjectsDark;
        public static CanvasBitmap worldChanges;
        public static Vector2 clientTL;
        public static Vector2 cameraC = new Vector2(300, 300);
        public static Vector2 cameraCLag = cameraC; // for smoothing
        public static Vector2 clientC;
        public static Vector2 clientCScreen;
        public static Vector2 clientSpan;
        public static Vector2 halfSpan;
        static Army underMouse;
        static float bestUnderMouseScore;
        //   public static Vector2 cameraMid;
        public static float cameraZoom = 64;
        public static float cameraZoomLag = 64;
        public float eventTimeOffset;
        public float eventTimeOffsetLag;
        public float eventTimeEnd;
        static public CanvasSolidColorBrush raidBrush, shadowBrush, desaturateBrush;
        static public Color nameColor, nameColorHover, myNameColor, nameColorIncoming, nameColorSieged, nameColorIncomingHover, nameColorSiegedHover, myNameColorIncoming, myNameColorSieged;
        static CanvasLinearGradientBrush tipBackgroundBrush, tipTextBrush;
        static CanvasTextFormat tipTextFormat = new CanvasTextFormat() { FontSize = 14, WordWrapping = CanvasWordWrapping.NoWrap };
        static CanvasTextFormat tipTextFormatCentered = new CanvasTextFormat() { FontSize = 12, HorizontalAlignment = CanvasHorizontalAlignment.Center, VerticalAlignment = CanvasVerticalAlignment.Center, WordWrapping = CanvasWordWrapping.NoWrap };
        static CanvasTextFormat nameTextFormat = new CanvasTextFormat()
        {
            FontSize = 10,
            HorizontalAlignment = CanvasHorizontalAlignment.Center,
            VerticalAlignment = CanvasVerticalAlignment.Center,
            WordWrapping = CanvasWordWrapping.NoWrap,
            Options=CanvasDrawTextOptions.EnableColorFont | CanvasDrawTextOptions.NoPixelSnap
        };
        //        static readonly Color attackColor = Colors.DarkRed;
        static readonly Color attackColor = Colors.White;
        static readonly Color defenseColor = Color.FromArgb(255, 20, 200, 200);
        static readonly Color defenseArrivedColor = Color.FromArgb(255, 20, 255, 192);
        static readonly Color artColor = Colors.DarkOrange;
        static readonly Color senatorColor = Colors.OrangeRed;
        static readonly Color incomingHistoryColor = Colors.White;// (0xFF8B008B);// Colors.DarkMagenta;
        static readonly Color raidColor = Colors.Yellow;
        //        static readonly Color shadowColor = Color.FromArgb(128, 0, 0, 0);
        static readonly Color selectColor = Colors.DarkMagenta;
        static readonly Color buildColor = Colors.DarkRed;
        static readonly Color hoverColor = Colors.Purple;
        static readonly Color focusColor = Colors.Magenta;
        static readonly Color black0Alpha = new Color() { A = 0, R = 0, G = 0, B = 0 };
        static CanvasBitmap[] troopImages = new CanvasBitmap[Game.Enum.ttCount];
        static Vector2 troopImageOriginOffset;
        static Dictionary<string, CanvasTextLayout> nameLayoutCache = new Dictionary<string, CanvasTextLayout>();
        const int bottomMargin = 0;
        const int cotgPopupWidth = 550;
        const int cotgPopupLeft = 438;
        const int cotgPopupRight = cotgPopupLeft+cotgPopupWidth;
        const int cotgPanelRight = 410;
        public static int cachedXOffset = cotgPanelRight;
        public static int cachedTopOffset = 0;
        const int cotgPopupTopDefault = 95;
        const int cotgPopupTopLong = 300+95;
        const float actionStopDistance = 48.0f;
        static public CanvasAnimatedControl canvas;

        public static void NotifyCotgPopup(int cotgPopupOpen)
        {
            var hasPopup = (cotgPopupOpen&127) != 0;
            var hasLongWindow = cotgPopupOpen >= 128;
            var leftOffset = hasPopup ? cotgPopupRight : cotgPanelRight;
            var topOffset = hasLongWindow ? webclientSpan.y*65/100 : cotgPopupTopDefault;
            var delta = leftOffset - cachedXOffset;
            if (delta==0 && cachedTopOffset == topOffset)
                return;
            cachedTopOffset = topOffset;
            cachedXOffset = leftOffset;
            var _grid = canvas;


            App.DispatchOnUIThreadLow(() => _grid.Margin = new Thickness(hasPopup ? cotgPopupWidth+(cotgPopupLeft-cotgPanelRight) : 0, topOffset, 0, bottomMargin));
            //            _grid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            //            AUtil.Nop( (_grid.ColumnDefinitions[0].Width = new GridLength(leftOffset),
            //          _grid.ColumnDefinitions[1].Width = new GridLength(_grid.ColumnDefinitions[1].Width.Value-delta))));

        }

        //CanvasStrokeStyle defaultStrokeStyle = new CanvasStrokeStyle() { CustomDashStyle=new float[] { 2, 6 },
        //    DashCap=CanvasCapStyle.Triangle,
        //    EndCap=CanvasCapStyle.Triangle,
        //    StartCap=CanvasCapStyle.Triangle};
        const float dashD0 = 6.0f;
        const float dashD1 = 6f;
        CanvasStrokeStyle defaultStrokeStyle = new CanvasStrokeStyle()
        {
            CustomDashStyle = new float[] { dashD0, dashD1 },
            EndCap = CanvasCapStyle.Flat,
            DashCap = CanvasCapStyle.Triangle,
            StartCap = CanvasCapStyle.Flat,
            //           TransformBehavior=CanvasStrokeTransformBehavior.Hairline
        };
        public CanvasAnimatedControl CreateCanvasControl()
        {
            //Assert((0.5f).CeilToInt() == 1);
            //Assert((-1.0f).CeilToInt() == -1);
            //Assert((0.0f).CeilToInt() == 0);
            //Assert((-0.5f).CeilToInt() == 0);
            //Assert((0.5f).FloorToInt() == 0);
            //Assert((-1.0f).FloorToInt() == -1);
            //Assert((0.0f).FloorToInt() == 0);
            //Assert((-0.5f).FloorToInt() == -1);

            canvas = new CanvasAnimatedControl()
            {
                IsHitTestVisible = false,
                IsTabStop=true,
                UseSharedDevice = true,
                TargetElapsedTime =TimeSpan.FromSeconds(1.0f/60.0f),

                IsFixedTimeStep = false
            };
            canvas.Draw += Canvas_Draw;
            canvas.Update += Canvas_Update;
            canvas.Unloaded += Canvas_Unloaded;
            canvas.LayoutUpdated += Canvas_LayoutUpdated;
            canvas.SizeChanged += Canvas_SizeChanged;
            canvas.CreateResources += Canvas_CreateResources;
            canvas.Margin = new Thickness(0, 0, 0, bottomMargin);
            //  SetupCoreInput();
            return canvas;

        }

        bool inputSetup;
        private void Canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            if (!inputSetup)
            {
                inputSetup = true;
                SetupCoreInput();
            }

            if (World.bitmapPixels != null)
            {
                // canvas.Paused = true;
                var pixels = World.bitmapPixels;
                World.bitmapPixels = null;
                if (worldObjects != null)
                {
                    var w = worldObjects;
                    worldObjects = null;
                    w.Dispose();
                }
                worldObjects = CanvasBitmap.CreateFromBytes(canvas, pixels, World.outSize, World.outSize, Windows.Graphics.DirectX.DirectXPixelFormat.BC1UIntNormalized);
                //canvas.Paused = false;
                //if (worldObjectsDark != null)
                //    worldObjectsDark.Dispose();
                //worldObjectsDark = new TintEffect() { BufferPrecision = CanvasBufferPrecision.Precision8UIntNormalizedSrgb, Source = worldObjects, Color = new Color() { A = 255, R = 128, G = 128, B = 128 } };

            }
            if (World.changePixels != null)
            {
                var pixels = World.changePixels;
                ClearHeatmapImage();
                worldChanges = CanvasBitmap.CreateFromBytes(canvas, pixels, World.outSize, World.outSize, Windows.Graphics.DirectX.DirectXPixelFormat.BC1UIntNormalized);

            }

        }
        public static void ClearHeatmapImage()
        {
            World.changePixels = null;
            if (worldChanges != null)
            {
                var w = worldChanges;
                worldChanges = null;
                w.Dispose();
            }
            World.changeMapInProgress = false;// this is used to temporarily block the UI from issuing multiple changes at once

        }
        public static void ClearHeatmap()
        {
            ClearHeatmapImage();
            World.rawPrior = null;
        }
        public static void SetCanvasVisibility(bool visible)
        {
            if (canvas.Visibility == Visibility.Visible)
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

        private static void SetClientSpan(Vector2 span)
        {
            clientSpan.X = span.X - (span.X % 8);
            clientSpan.Y = span.Y - (span.Y % 8);
            halfSpan = clientSpan * 0.5f;
        }

        private void Canvas_LayoutUpdated(object sender, object e)
        {
            var c = canvas.ActualOffset;

            clientC = new Vector2(c.X, c.Y);
            SetClientSpan(canvas.ActualSize);

            clientCScreen = canvas.TransformToVisual(Window.Current.Content)
                .TransformPoint(new Point(0, 0)).ToVector2();
        }
        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetClientSpan(e.NewSize.ToVector2());
            clientCScreen = canvas.TransformToVisual(Window.Current.Content)
                .TransformPoint(new Point(0, 0)).ToVector2();
        }

        async private void Canvas_CreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            if (args.Reason != Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesReason.FirstTime)
            {
                Fatal();
                return;
            }

            worldBackground = await CanvasBitmap.LoadAsync(canvas.Device, new Uri("ms-appx:///Assets/world.dds"));
            worldBackgroundDark = new TintEffect() { BufferPrecision = CanvasBufferPrecision.Precision8UIntNormalizedSrgb, Source = worldBackground, Color = new Color() { A = 255, R = 128, G = 128, B = 128 } };

            for (int i = 0; i < ttCount; ++i)
            {
                troopImages[i] = await CanvasBitmap.LoadAsync(canvas.Device, new Uri($"ms-appx:///images/troops{i}.png"));
                if (i == 0)
                {
                    var size = troopImages[i].Size;
                    troopImageOriginOffset.X = (float)size.Width*0.5f;
                    troopImageOriginOffset.Y = (float)size.Height*0.625f;
                }
            }
            //  while (JSClient.cid == 0)
            //     await Task.Delay(1 * 1000);
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

        const float postAttackDisplayTime = 15 * 60; // 11 min

        const float circleRadMin = 3.0f;
        const float circleRadMax = 5.5f;
        const float lineThickness = 2.0f;
        const float rectSpanMin = 4.0f;
        const float rectSpanMax = 8.0f;
        const float bSizeGain = 4.0f;
        const float bSizeGain2 = 4;//4.22166666666667f;
        const float srcImageSpan = 2400;
        const float bSizeGain3 = bSizeGain* bSizeGain / bSizeGain2;
        public static float pixelScale = 1;
        public static float shapeSizeGain = 1.0f;
        public static float pixelScaleInverse = 1;
        const float dashLength = (dashD0 + dashD1) * lineThickness;


        public static bool IsCulled(Vector2 c0, Vector2 c1)
        {
            var x1 = c0.X.Max(c1.X);
            var x0 = c0.X.Min(c1.X);

            var y1 = c0.Y.Max(c1.Y);
            var y0 = c0.Y.Min(c1.Y);
            // todo: cull on diagonals
            return x1 <= 0 | x0 >= clientSpan.X |
                    y1 <= 0 | y0 >= clientSpan.Y;
        }
        public static bool IsCulled(Vector2 c0, float pad)
        {
            var x1 = c0.X;
            var x0 = c0.X;

            var y1 = c0.Y;
            var y0 = c0.Y;
            // todo: cull on diagonals
            return x1+pad <= 0 | x0-pad >= clientSpan.X |
                    y1 + pad <= 0 | y0-pad >= clientSpan.Y;
        }

        public static Vector2 shadowOffset = new Vector2(lineThickness*0.75f, lineThickness*0.75f);
        public static void SetCameraCNoLag(Vector2 c) => cameraCLag = cameraC = c;
        static DateTimeOffset lastDrawTime;
        public static bool tileSetsPending;
        private const float smallRectSpan = 4;

        private void Canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            underMouse = null;
            bestUnderMouseScore = 32 * 32;
            if (!(IsWorldView()))
                return;




            try
            {
                var _serverNow = JSClient.ServerTime();
                var dt = (float)(_serverNow - lastDrawTime).TotalSeconds;
                lastDrawTime = _serverNow;

                var gain = (1 - MathF.Exp(-4 * dt));
                cameraCLag += (cameraC - cameraCLag) * gain;
                cameraZoomLag += (cameraZoom - cameraZoomLag) * gain;
                eventTimeOffsetLag += (eventTimeOffset - eventTimeOffsetLag) * gain;
                //                cameraZoomLag += (cameraZoom

                var serverNow = _serverNow + TimeSpan.FromMinutes(eventTimeOffsetLag);

                float animT = ((uint)Environment.TickCount % 3000) * (1.0f / 3000);
                int tick = (Environment.TickCount >> 3) & 0xfffff;
                var animTLoop = animT.Wave();
                int cx0 = 0, cy0 = 0, cx1 = 0, cy1 = 0;
                var rectSpan = animTLoop.Lerp(rectSpanMin, rectSpanMax);
                //   ShellPage.T("Draw");
                if (shadowBrush == null)
                {

                    desaturateBrush = new CanvasSolidColorBrush(canvas, new Color() { A = 255, G = 80, B = 80, R = 80 }) { Opacity = 0.8f };
                    raidBrush = new CanvasSolidColorBrush(canvas, Colors.BlueViolet);
                    shadowBrush = new CanvasSolidColorBrush(canvas, new Color() { A = 255, G = 64, B = 64, R = 64 }) { Opacity = 0.675f };
                    tipBackgroundBrush = new CanvasLinearGradientBrush(canvas, new CanvasGradientStop[]
                    {
                        new CanvasGradientStop() { Position = 0.0f, Color = new Color(){A=255,R=128,G=64,B=64 } },
                        new CanvasGradientStop() { Position = 1.0f, Color = Colors.Black } })
                    { Opacity = 0.675f };
                    tipTextBrush = new CanvasLinearGradientBrush(canvas, new CanvasGradientStop[]
                    {
                        new CanvasGradientStop() { Position = 0.0f, Color = Colors.White },
                        new CanvasGradientStop() { Position = 1.0f, Color = Colors.Blue }
                    });
                    ;

                }
                defaultStrokeStyle.DashOffset = (1 - animT) * dashLength;

                var ds = args.DrawingSession;
                //                ds.Blend = ( (int)(serverNow.Second / 15) switch { 0 => CanvasBlend.Add, 1 => CanvasBlend.Copy, 2 => CanvasBlend.Add, _ => CanvasBlend.SourceOver } );

                ds.Antialiasing = CanvasAntialiasing.Aliased;
                //ds.TextRenderingParameters = new CanvasTextRenderingParameters(!App.IsKeyPressedControl() ? CanvasTextRenderingMode.Outline : CanvasTextRenderingMode.Default, CanvasTextGridFit.Default);

                //              ds.TextRenderingParameters = new CanvasTextRenderingParameters(CanvasTextRenderingMode.Default, CanvasTextGridFit.Disable);
                // var scale = ShellPage.canvas.ConvertPixelsToDips(1);
                pixelScale = (cameraZoomLag);
                pixelScaleInverse = 1.0f / cameraZoomLag;
                shapeSizeGain = MathF.Sqrt(pixelScale * (1.50f / 64.0f));
                var deltaZoom = cameraZoomLag - detailsZoomThreshold;
                var wantDetails = deltaZoom > 0;
                var wantImage = deltaZoom < detailsZoomFade;

                {
                    var srcP0 = new Point((cameraCLag.X + 0.5f) * bSizeGain2 - halfSpan.X * bSizeGain2 * pixelScaleInverse,
                                              (cameraCLag.Y + 0.5f) * bSizeGain2 - halfSpan.Y * bSizeGain2 * pixelScaleInverse);
                    var srcP1 = new Point(srcP0.X + clientSpan.X * bSizeGain2 * pixelScaleInverse,
                                           srcP0.Y + clientSpan.Y * bSizeGain2 * pixelScaleInverse);
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

                    var attacksVisible = DefensePage.IsVisible() | OutgoingTab.IsVisible() | DefenderPage.IsVisible() | HitTab.IsVisible() | AttackTab.IsVisible();
                    if (worldBackground != null && IsWorldView() && wantImage)
                    {

                        if (wantImage)
                        {

                            ds.DrawImage(attacksVisible ? worldBackground : worldBackground,
                                new Rect(destP0, destP1),
                                new Rect(srcP0, srcP1), 1.0f, CanvasImageInterpolation.Linear);
                            if (worldObjects != null)
                                ds.DrawImage(attacksVisible ? worldObjects : worldObjects,
                                    new Rect(destP0, destP1),
                                    new Rect(srcP0, srcP1), 1.0f, CanvasImageInterpolation.Linear);

                        }


                    }
                    //   ds.Antialiasing = CanvasAntialiasing.Antialiased;
                    // ds.Transform = new Matrix3x2( _gain, 0, 0, _gain, -_gain * ShellPage.cameraC.X, -_gain * ShellPage.cameraC.Y );

                    //           dxy.X = (float)sender.Width;
                    //            dxy.Y = (float)sender.ActualHeight;

                    //            ds.DrawLine( SC(0.25f,.125f),SC(0.lineThickness,0.9f), raidBrush, lineThickness,defaultStrokeStyle);
                    //           ds.DrawLine(SC(0.25f, .125f), SC(0.9f, 0.lineThickness), shadowBrush, lineThickness, defaultStrokeStyle);
                    // if (IsPageDefense())
                    if (!IsCityView() && TileData.state >= TileData.State.loadingImages)
                    {
                        if (wantDetails)
                        {
                            var wantFade = wantImage;
                            var alpha = wantFade ? (deltaZoom / detailsZoomFade).Min(1) : 1.0f;
                            var rgb = attacksVisible ? 1.0f : 1.0f;
                            Vector4 tint = new Vector4(rgb, rgb, rgb, alpha);
                            var intAlpha = (byte)(alpha * 255.0f).RoundToInt();

                            nameColor = new Color() { A = intAlpha, G = 255, B = 255, R = 255 };
                            nameColorHover = new Color() { A = intAlpha, G = 255, B = 255, R = 185 };
                            myNameColor = new Color() { A = intAlpha, G = 255, B = 190, R = 210 };
                            nameColorIncoming = new Color() { A = intAlpha, G = 220, B = 220, R = 255 };
                            nameColorSieged = new Color() { A = intAlpha, G = 220, B = 190, R = 255 };
                            nameColorIncomingHover = new Color() { A = intAlpha, G = 220, B = 160, R = 255 };
                            nameColorSiegedHover = new Color() { A = intAlpha, G = 220, B = 140, R = 255 };
                            myNameColorIncoming = new Color() { A = intAlpha, G = 240, B = 150, R = 255 };
                            myNameColorSieged = new Color() { A = intAlpha, G = 240, B = 120, R = 255 };


                            var td = TileData.instance;
                            var halfTiles = (clientSpan * (0.5f / cameraZoomLag)).CeildToInt();
                            var ccBase = cameraCLag.RoundToInt();
                            cx0 = (-halfTiles.x + ccBase.x).Max(0);
                            cy0 = (-halfTiles.y + ccBase.y).Max(0);
                            cx1 = (halfTiles.x + 1 + ccBase.x).Min(World.worldDim);
                            cy1 = (halfTiles.y + 1 + ccBase.y).Min(World.worldDim);
                            const bool isShift = true;// App.IsKeyPressedShift();
                            const float tcOff = isShift ? 0.0f : 0.5f;
                            const float tzOff = isShift ? 0.0f : 1.0f;

                            using (var batch = ds.CreateSpriteBatch(CanvasSpriteSortMode.None, CanvasImageInterpolation.Linear, isShift ? CanvasSpriteOptions.ClampToSourceRect : CanvasSpriteOptions.None))
                            {
                                foreach (var layer in td.layers)
                                {
                                    var layerDat = layer.data;
                                    for (var cy = cy0; cy < cy1; ++cy)
                                    {
                                        for (var cx = cx0; cx < cx1; ++cx)
                                        {
                                            var ccid = cx + cy * World.worldDim;
                                            var imageId = layerDat[ccid];
                                            if (imageId == 0)
                                                continue;
                                            {
                                                var rect = new Rect(((new Vector2(cx - .5f, cy - 0.5f)).WToC()).ToPoint(), new Size(pixelScale, pixelScale));
                                                //   var layerData = TileData.packedLayers[ccid];
                                                //  while (layerData != 0)
                                                {
                                                    //    var imageId = ((uint)layerData & 0xffffu);
                                                    //     layerData >>= 16;
                                                    var tileId = imageId >> 13;
                                                    var off = imageId & ((1 << 13) - 1);
                                                    var tile = td.tilesets[tileId];

                                                    if (tile.bitmap == null)
                                                        continue;
                                                    var sy = off / tile.columns;
                                                    var sx = off - sy * tile.columns;
                                                    if (wantFade || attacksVisible)
                                                        batch.DrawFromSpriteSheet(tile.bitmap, rect,
                                                            new Rect(new Point(sx * tile.tilewidth + tcOff, sy * tile.tileheight + tcOff), new Size(tile.tilewidth - tzOff, tile.tileheight - tzOff)), tint);
                                                    else
                                                        batch.DrawFromSpriteSheet(tile.bitmap, rect,
                                                        new Rect(new Point(sx * tile.tilewidth + tcOff, sy * tile.tileheight + tcOff), new Size(tile.tilewidth - tzOff, tile.tileheight - tzOff)));



                                                }


                                            }
                                        }
                                    }
                                }
                            }// sprite batch
                             //
                             //   if (attacksVisible)
                             //       ds.FillRectangle(new Rect(new Point(), clientSpan.ToSize()), desaturateBrush);


                        }
                        //else
                        {
                            if (attacksVisible)
                                ds.FillRectangle(new Rect(new Point(), clientSpan.ToSize()), desaturateBrush);
                        }

                        // overlay
                        if (worldChanges != null)
                            ds.DrawImage(worldChanges,
                                new Rect(destP0, destP1),
                                new Rect(srcP0, srcP1), 1.0f,
                                CanvasImageInterpolation.Linear, CanvasComposite.Add);

                    }
                    //    ds.Antialiasing = CanvasAntialiasing.Antialiased;
                }
                var circleRadBase = circleRadMin * shapeSizeGain*6.5f;
                var circleRadius = animTLoop.Lerp(circleRadMin, circleRadMax) * shapeSizeGain * 6.5f;
                //    var highlightRectSpan = new Vector2(circleRadius * 2.0f, circleRadius * 2);
                ds.Antialiasing = CanvasAntialiasing.Antialiased;


                if (!IsCityView())
                {
                    var defenderVisible = DefenderPage.IsVisible() || DefendTab.IsVisible();
                    var outgoingVisible = OutgoingTab.IsVisible();
                    using (var batch = ds.CreateSpriteBatch(CanvasSpriteSortMode.Bitmap))
                    {
                        if (DefensePage.IsVisible() || HitTab.IsVisible())
                        {
                            for (var dfof = 0; dfof < 2; ++dfof)
                            {
                                if (dfof == 0)
                                {
                                    if (!DefensePage.IsVisible())
                                        continue;
                                }
                                else
                                {
                                    if (!HitTab.IsVisible())
                                        continue;

                                }
                                var reports = dfof == 0 ? DefensePage.instance.history : HitTab.instance.history;
                                if (reports.Length > 0)
                                {

                                    var counts = new Dictionary<int, IncomingCounts>();
                                    foreach (var attack in reports)
                                    {
                                        if (attack.type == reportPending)
                                        {
                                            if (dfof == 0)
                                            {
                                                // this will be drawn later, don't repeat
                                                if (defenderVisible)
                                                    continue;

                                            }
                                            else
                                            {
                                                // this will be drawn later, don't repeat
                                                if (outgoingVisible)
                                                    continue;
                                            }

                                        }
                                        var targetCid = attack.targetCid;
                                        var sourceCid = attack.sourceCid;
                                        var c1 = targetCid.CidToCC();
                                        var c0 = sourceCid.CidToCC();
                                        // cull (should do this pre-transform as that would be more efficient
                                        if (c0.X.Min(c1.X) >= clientSpan.X)
                                            continue;
                                        if (c0.X.Max(c1.X) <= 0.0f)
                                            continue;
                                        if (c0.Y.Min(c1.Y) >= clientSpan.Y)
                                            continue;
                                        if (c0.Y.Max(c1.Y) <= 0.0f)
                                            continue;
                                        var dt1 = attack.TimeToArrival(serverNow);

                                        // before attack
                                        var journeyTime = attack.journeyTime;
                                        {
                                            // register attack
                                            if (!counts.TryGetValue(targetCid, out var count))
                                            {
                                                count = new IncomingCounts();
                                                counts.Add(targetCid, count);
                                            }
                                            if (dt1 > 0)
                                                ++count.incoming;
                                            else
                                                ++count.prior;
                                        }

                                        if (dt1 >= journeyTime || dt1 < -postAttackDisplayTime)
                                            continue;
                                        if (!Spot.IsSelectedOrHovered(targetCid, sourceCid))
                                        {
                                            continue;
                                        }
                                        Color c = GetAttackColor(attack);

                                        {
                                            var t = (tick * sourceCid.CidToRandom().Lerp(1.5f / 512.0f, 1.75f / 512f)) + 0.25f;
                                            var r = t.Ramp();
                                            int iType = 0;
                                            float alpha = 1;
                                            var nSprite = attack.troops.Length;

                                            if (nSprite > 1)
                                            {
                                                Assert(t > 0);
                                                var rtype = t % nSprite;
                                                iType = (int)rtype;
                                                var frac = rtype - iType;
                                                iType = iType.Min(nSprite - 1);
                                                if (frac < 0.25f)
                                                    alpha = AMath.STerm(frac * 4.0f);
                                                else if (frac > 0.75f)
                                                    alpha = AMath.STerm((1 - frac) * 4.0f);

                                            }
                                            DrawAction(ds, batch, dt1, journeyTime, r, c0, c1, c, troopImages[attack.troops[iType].type], true, attack, 28, alpha);
                                        }
                                        //var progress = (dt0 / (dt0 + dt1).Max(1)).Saturate(); // we don't know the duration so we approximate with 2 hours
                                        //var mid = progress.Lerp(c0, c1);
                                        //ds.DrawLine(c0, c1, shadowBrush, lineThickness, defaultStrokeStyle);
                                        //ds.FillCircle(mid, span, shadowBrush);
                                        //var midS = mid - shadowOffset;
                                        //ds.DrawLine(c0 - shadowOffset, midS, raidBrush, lineThickness, defaultStrokeStyle);
                                        //ds.FillCircle(midS, span, raidBrush);
                                    }
                                    foreach (var i in counts)
                                    {
                                        var cid = i.Key;
                                        var count = i.Value;
                                        var c = cid.CidToCC();
                                        DrawTextBox(ds, $"{count.prior}`{count.incoming}", c, tipTextFormatCentered, Colors.White);


                                    }
                                }
                            }
                        }
                        if (AttackTab.IsVisible())
                        {
                            foreach (var t in AttackTab.readable.targets)
                            {
                                var c1 = t.cid.CidToCC();
                                DrawTextBox(ds, $"{Spot.GetOrAdd(t.cid).classificationString}", c1, tipTextFormatCentered, t.attackCluster == 0 ? Colors.White : Colors.Teal);
                            }
                            foreach (var t in AttackTab.readable.attacks)
                            {
                                if (t.target != 0)
                                {
                                    var _t = (tick * t.cid.CidToRandom().Lerp(1.5f / 512.0f, 2.0f / 512f)) + 0.25f;
                                    var r = _t.Ramp();
                                    var c = t.fake ? Colors.White : Colors.Red;
                                    var c0 = t.cid.CidToCC();
                                    var c1 = t.target.CidToCC();
                                    //   DrawTextBox(ds, $"{t.type} {t.fake} {t.player}", c1, tipTextFormatCentered);
                                    DrawAction(ds, batch, .5f, 1.0f, r, c0, c1, c, troopImages[t.troopType], false, null, 28, 0.8f);

                                }
                            }
                        }
                        if (defenderVisible || outgoingVisible)
                        {
                            var cullSlopSpace = 80 * pixelScale;
                            for (int iOrO = 0; iOrO < 2; ++iOrO)
                            {
                                foreach (var city in (iOrO ==0) ? Spot.defendersI : Spot.defendersO)
                                {
                                    if (city.incoming.Any() || city.isMine)
                                    {

                                        var targetCid = city.cid;
                                        var c1 = targetCid.CidToCC();
                                        if (IsCulled(c1, cullSlopSpace))  // this is in pixel space - Should be normalized for screen resolution or world space (1 continent?)
                                            continue;
                                        var incAttacks = 0;
                                        var incTs = 0;
                                        foreach (var i in city.incoming)
                                        {
                                            var c0 = i.sourceCid.CidToCC();
                                            if (IsCulled(c0, c1))
                                                continue;
                                            Color c;
                                            if (i.isDefense)
                                            {

                                                if (i.sourceCid == targetCid)
                                                    continue;

                                                c = i.time <= serverNow ? defenseArrivedColor : defenseColor;
                                            }
                                            else
                                            {
                                                ++incAttacks;
                                                incTs += i.ts;
                                                if (i.hasArt)
                                                {
                                                    c = artColor;
                                                }
                                                else if (i.hasSenator)
                                                {
                                                    c = senatorColor; ;
                                                }
                                                else
                                                {
                                                    c = GetAttackColor(i);
                                                }
                                            }
                                            if (!Spot.IsSelectedOrHovered(i.sourceCid, targetCid))
                                            {
                                                continue;
                                                c.A = (byte)((int)c.A * 3 / 8); // reduce alpha if not selected
                                            }
                                            if (i.troops.Any())
                                            {
                                                var t = (tick * i.sourceCid.CidToRandom().Lerp(1.5f / 512.0f, 2.0f / 512f)) + 0.25f;
                                                var r = t.Ramp();
                                                int iType = 0;
                                                float alpha = 1;
                                                var nSprite = i.troops.Length;

                                                if (nSprite > 1)
                                                {
                                                    Assert(t > 0);
                                                    var rtype = t % nSprite;
                                                    iType = (int)rtype;
                                                    var frac = rtype - iType;
                                                    iType = iType.Min(nSprite - 1);
                                                    if (frac < 0.25f)
                                                        alpha = AMath.STerm(frac * 4.0f);
                                                    else if (frac > 0.75f)
                                                        alpha = AMath.STerm((1 - frac) * 4.0f);

                                                }

                                                DrawAction(ds, batch, i.TimeToArrival(serverNow), i.journeyTime, r, c0, c1, c, troopImages[i.troops[iType].type], true, i, 28, alpha);
                                            }
                                            else
                                            {
                                                Assert(false);
                                            }
                                        }
                                        DrawTextBox(ds, $"{incAttacks}`{city.claim.ToString("00")}%`{(incTs + 500) / 1000}k\n{ (city.tsMax.Max(city.tsHome) + 500) / 1000 }k", c1, tipTextFormatCentered, incAttacks != 0 ? Colors.White : Colors.Teal);
                                    }
                                }
                            }
                            if (defenderVisible)
                            {
                                foreach (var _city in City.allCities)
                                {

                                    var city = _city.Value;
                                    Assert(city is City);
                                    if (!city.incoming.Any())
                                    {
                                        var targetCid = city.cid;
                                        var c1 = targetCid.CidToCC();
                                        if (IsCulled(c1, cullSlopSpace))  // this is in pixel space - Should be normalized for screen resolution or world space (1 continent?)
                                            continue;
                                        DrawTextBox(ds, $"{(city.tsMax.Max(city.tsHome) + 500) / 1000 }k", c1, tipTextFormatCentered, Colors.Teal);

                                    }
                                }
                            }
                        }
                        var raidCullSlopSpace = 8 * pixelScale;


                        foreach (var city in City.allCities.Values)
                        {
                            // Todo: clip thi
                            if (city.senatorInfo.Length != 0 && !defenderVisible)
                            {
                                var c = city.cid.CidToCC();
                                var idle = 0;
                                var active = 0;
                                var recruiting = 0;
                                foreach (var sen in city.senatorInfo)
                                {
                                    if (sen.type == SenatorInfo.Type.idle)
                                        idle += sen.count;
                                    else if (sen.type == SenatorInfo.Type.recruit)
                                        recruiting += sen.count;
                                    else
                                        active += sen.count;
                                    if (sen.target != 0)
                                    {
                                        var c1 = sen.target.CidToCC();

                                        var dist = city.cid.DistanceToCid(sen.target) * cartTravel; // todo: ship travel?
                                        var t = (tick * city.cid.CidToRandom().Lerp(1.5f / 512.0f, 1.75f / 512f)) + 0.25f;
                                        var r = t.Ramp();
                                        // Todo: more accurate senator travel times
                                        DrawAction(ds, batch, (float)(sen.time - serverNow).TotalSeconds, dist*60.0f, r, c, c1, senatorColor,
                                            troopImages[ttSenator], false, null, 20);
                                    }
                                }
                                DrawTextBox(ds, $"{recruiting}`{idle}`{active}", c, tipTextFormatCentered, Colors.White);

                            }

                            if (MainPage.IsVisible())
                            {
                                var c = city.cid.CidToCC();
                                if (IsCulled(c, raidCullSlopSpace))
                                    continue;
                                var t = (tick * city.cid.CidToRandom().Lerp(1.375f / 512.0f, 1.75f / 512f));
                                var r = t.Ramp();
                                //ds.DrawRoundedSquareWithShadow(c,r, raidBrush);
                                foreach (var raid in city.raids)
                                {
                                    var ct = raid.target.CidToCC();
                                    (var c0, var c1) = !raid.isReturning ? (c, ct) : (ct, c);
                                    DrawAction(ds, batch, (float)(raid.time - serverNow).TotalSeconds,
                                        raid.GetOneWayTripTimeMinutes(city) * 60.0f,
                                        r, c0, c1, raidColor, troopImages[raid.troopType], false, null, 20);

                                }
                            }
                        }
                    }

                    foreach (var city in Spot.GetSelected())
                    {
                        var c = city.CidToCC();
                        var t = (tick * city.CidToRandom().Lerp(1.5f / 512.0f, 1.75f / 512f)) + 0.25f;
                        var r = t.Wave().Lerp(circleRadBase, circleRadBase * 1.325f);
                        ds.DrawRoundedSquareWithShadow(c, r, City.IsBuild(city) ? buildColor :
                                                            City.IsFocus(city) ? focusColor :
                                                            City.IsHover(city) ? hoverColor :
                                                            selectColor);

                    }
                }

                if (!IsCityView() && TileData.state >= TileData.State.loadingImages)
                {
                    if (wantDetails)
                    {
                        //
                        // Text names
                        {
                            // Labels last
                            for (var cy = cy0; cy < cy1; ++cy)
                            {
                                for (var cx = cx0; cx < cx1; ++cx)
                                {
                                    (var name, var isMine, var hasIncoming, var hovered, var spot) = World.GetLabel((cx, cy));
                                    if (name != null)
                                    {
                                        if (!nameLayoutCache.TryGetValue(name, out var layout))
                                        {
                                            layout = new CanvasTextLayout(ds, name, nameTextFormat, 0.0f, 0.0f);
                                            nameLayoutCache.Add(name, layout);
                                        }

                                        var rect = new Rect(((new Vector2(cx - .5f, cy - 0.5f)).WToC()).ToPoint(), new Size(pixelScale, pixelScale));

                                        ds.DrawTextLayout(layout, new Vector2((float)(rect.Left + rect.Right) * 0.5f,
                                            (float)rect.Top + (float)rect.Height * 7.25f / 8.0f),
                                            isMine ?
                                                (hasIncoming ?
                                                    (spot.underSiege ? myNameColorSieged
                                                                    : myNameColorIncoming)
                                                                        : myNameColor) :
                                            (hasIncoming ?
                                                (hovered ?
                                                    (spot.underSiege ? nameColorSiegedHover : nameColorIncomingHover)
                                                   : (spot.underSiege ? nameColorSieged : nameColorIncoming))
                                                   : hovered ? nameColorHover : nameColor));

                                    }
                                    if (spot != null &&  spot.isClassified)
                                    {
                                        var c1 = (cx, cy).WToC();
                                        DrawTextBox(ds, $"{spot.classificationString}", c1, tipTextFormatCentered, Colors.Cyan);
                                    }
                                }
                            }
                        }
                    }
                }

                // show selected
                var _toolTip = toolTip;

                if (underMouse!=null)
                {
                    //         Spot.viewHover = 0; // clear
                    _toolTip = underMouse.GetToopTip(serverNow);
                }
                if (_toolTip != null && IsWorldView())
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
                    tipTextBrush.EndPoint = tipBackgroundBrush.EndPoint = new Vector2((float)bounds.Right, (float)bounds.Bottom);
                    ds.FillRoundedRectangle(bounds, 8, 8, tipBackgroundBrush);
                    //                    target.X+= 12;
                    //                  target.Y += 8;

                    ds.DrawTextLayout(textLayout, c, tipTextBrush);//.Dra ds.DrawText(_toolTip, c, tipTextBrush, tipTextFormat);
                }
                var _contTip = contToolTip;
                if (_contTip != null && IsWorldView())
                {
                    CanvasTextLayout textLayout = new CanvasTextLayout(ds, _contTip, tipTextFormat, 0.0f, 0.0f);
                    var bounds = textLayout.DrawBounds;
                    Vector2 c = new Vector2(16, 16);
                    const float expand = 7;
                    bounds.X += c.X - expand;
                    bounds.Y += c.Y - expand;
                    bounds.Width += expand * 2;
                    bounds.Height += expand * 2;

                    //  var rectD = new Vector2(32*4, 24*5);
                    // var target = new Rect((mousePosition + rectD*0.25f).ToPoint(), rectD.ToSize());
                    tipTextBrush.StartPoint = tipBackgroundBrush.StartPoint = new Vector2((float)bounds.Left, (float)bounds.Top);
                    tipTextBrush.EndPoint = tipBackgroundBrush.EndPoint = new Vector2((float)bounds.Right, (float)bounds.Bottom);
                    ds.FillRoundedRectangle(bounds, 8, 8, tipBackgroundBrush);
                    //                    target.X+= 12;
                    //                  target.Y += 8;

                    ds.DrawTextLayout(textLayout, c, tipTextBrush);//.Dra ds.DrawText(_toolTip, c, tipTextBrush, tipTextFormat);
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }



        }

        private static Color GetAttackColor(Army attack)
        {
            return attack.type switch
            {
                reportAssault => Color.FromArgb(255, 0x7e, 0x3e, 0xd4),
                reportSiege => Color.FromArgb(255, 0xcf, 0x50, 0x07),
                reportSieging => Color.FromArgb(255, 0xc5, 0x7f, 0x4a),
                reportPlunder => Color.FromArgb(255, 0x28, 0x86, 0xc0),
                reportScout => Color.FromArgb(255, 0xc8, 0x2d, 0xbf),

                _ => incomingHistoryColor
            };
        }

        private static void DrawTextBox(CanvasDrawingSession ds, string text, Vector2 at, CanvasTextFormat format, Color color)
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
            ds.FillRoundedRectangle(bounds, 3, 3, shadowBrush);
            ds.DrawTextLayout(textLayout, at.X, at.Y, color);
        }

        //      private void DrawAction( CanvasDrawingSession ds, float timeToArrival, float journeyTime, float rectSpan, Vector2 c0, Vector2 c1,Color color)
        //{
        //          if (IsCulled(c0, c1))
        //              return;
        //          float progress;
        //          if (timeToArrival <= 0.0f)
        //          {
        //              progress = 1.0f;
        //          }
        //          else 
        //          {
        //              if (timeToArrival >= journeyTime)
        //                  progress = 1.0f / 16.0f; // just starting
        //              else
        //                  progress = 1f - (timeToArrival / journeyTime); // we don't know the duration so we approximate with 2 hours
        //          }
        //          if (timeToArrival < postAttackDisplayTime)
        //              rectSpan *= 2.0f - timeToArrival / postAttackDisplayTime;
        //          var mid = progress.Lerp(c0, c1);
        //          var shadowC = color.GetShadowColor();
        //          var midS = mid - shadowOffset;

        //          ds.DrawLine(c0, c1, shadowC, lineThickness, defaultStrokeStyle);
        //          ds.DrawRoundedSquare(mid, rectSpan, shadowC,2.0f);
        //          ds.DrawLine(c0 - shadowOffset, midS, color, lineThickness, defaultStrokeStyle);
        //          ds.DrawRoundedSquare(midS, rectSpan, color, 2.0f) ;
        //      }
        private void DrawAction(CanvasDrawingSession ds, CanvasSpriteBatch batch, float timeToArrival, float journeyTime, float rectSpan, Vector2 c0, Vector2 c1, Color color,
            CanvasBitmap bitmap, bool applyStopDistance, Army army, float spriteSize, float alpha = 1)
        {
            if (IsCulled(c0, c1))
                return;
            float progress;
            if (timeToArrival <= 0.0f)
            {
                progress = 1.0f;
                timeToArrival = 0;
            }
            else
            {
                if (timeToArrival >= journeyTime)
                    progress = 1.0f / 16.0f; // just starting
                else
                    progress = 1f - (timeToArrival / journeyTime); // we don't know the duration so we approximate with 2 hours
            }
            if (applyStopDistance)
            {
                progress = progress.Min(1.0f - shapeSizeGain*actionStopDistance  / Vector2.Distance(c0, c1));
                //      var dc01 = c0 - c1;
                //       c1 += dc01 *( actionStopDistance / dc01.Length());
            }

            var gain = 1.0f;
            if (timeToArrival < postAttackDisplayTime)
                gain = 1.0f +  (1.0f - timeToArrival / postAttackDisplayTime)*0.25f;
            var mid = progress.Lerp(c0, c1);
            var shadowC = color.GetShadowColor();
            var midS = mid - shadowOffset;
            var d2 = Vector2.DistanceSquared(mid, mousePosition);
            if (d2<bestUnderMouseScore)
            {
                bestUnderMouseScore = d2;
                underMouse = army;
            }
            ds.DrawLine(c0, c1, shadowC, lineThickness, defaultStrokeStyle);
            if (applyStopDistance)
                ds.FillRectangle(new Rect(c0.X - smallRectSpan, c0.Y - smallRectSpan, smallRectSpan*2, smallRectSpan*2.0f), shadowC);
            ds.DrawLine(c0 - shadowOffset, midS, color, lineThickness, defaultStrokeStyle);
            if (applyStopDistance)
                ds.FillRectangle(new Rect(c0.X- shadowOffset.X - smallRectSpan, c0.Y- shadowOffset.Y - smallRectSpan, smallRectSpan * 2, smallRectSpan * 2.0f), color);
            var dc = new Vector2(spriteSize, spriteSize);
            batch.Draw(bitmap, new Rect(mid.X-spriteSize, mid.Y-spriteSize, spriteSize*2, spriteSize*2), HSLToRGB.ToRGBA(rectSpan, 0.3f, 0.825f, alpha, gain*1.1875f));
            //            ds.DrawRoundedSquare(midS, rectSpan, color, 2.0f);
        }

        private static bool IsWorldView()
        {
            return JSClient.IsWorldView();
        }

        private static bool IsCityView()
        {
            return JSClient.IsCityView();
        }
    }
    public static class CanvasHelpers
    {
        public static void DrawRoundedSquare(this CanvasDrawingSession ds, Vector2 c, float circleRadius, Color color, float thickness = 1.5f)
        {
            ds.DrawRoundedRectangle(c.X - circleRadius, c.Y - circleRadius, circleRadius*2, circleRadius*2, circleRadius*0.25f, circleRadius*0.25f, color, thickness);
        }
        public static void DrawRoundedSquareWithShadow(this CanvasDrawingSession ds, Vector2 c, float circleRadius, Color brush, float thickness = 1.5f)
        {
            DrawRoundedSquareShadow(ds, c, circleRadius, brush.GetShadowColor(), thickness);
            DrawRoundedSquareBase(ds, c, circleRadius, brush, thickness);
        }
        public static void DrawRoundedSquareShadow(this CanvasDrawingSession ds, Vector2 c, float circleRadius, Color color, float thickness = 1.5f)
        {
            DrawRoundedSquare(ds, c, circleRadius, color, thickness);
        }
        public static void DrawRoundedSquareBase(this CanvasDrawingSession ds, Vector2 c, float circleRadius, Color brush, float thickness = 1.5f)
        {
            DrawRoundedSquare(ds, c - ShellPage.shadowOffset, circleRadius, brush, thickness);
        }

        public static Vector2 WToC(this Vector2 c)
        {
            return (c - ShellPage.cameraCLag) * ShellPage.pixelScale + ShellPage.halfSpan;
        }
        public static Vector2 WToC(this (int x, int y) c)
        {
            return new Vector2(c.x, c.y).WToC();
        }
        public static Vector2 CidToCC(this int c)
        {
            return c.ToWorldC().WToC();
        }


        public static bool BringCidIntoWorldView(this int cid, bool lazy)
        {
            var v = cid.CidToWorldV();
            var newC = v;
            var dc = newC - ShellPage.cameraC;
            // only move if needed
            if (!lazy||
                dc.X.Abs() * ShellPage.pixelScale > ShellPage.halfSpan.X*0.675f  ||
                dc.Y.Abs() * ShellPage.pixelScale > ShellPage.halfSpan.Y*0.675f)
            {
                if (Vector2.DistanceSquared(ShellPage.cameraC, newC) >= 0.875f)
                {
                    ShellPage.cameraC = newC;
                    ShellPage.SetJSCamera();
                    return true;
                }

            }
            return false;
        }
        public static Color GetShadowColor(this Color c)
        {
            return Color.FromArgb(192, (byte)(c.R*3/4), (byte)(c.G*3/4), (byte)(c.B*3/4));
            //            (0.625f).Lerp(c, Color.FromArgb(128, 0, 0, 0));
            //            (0.625f).Lerp(c, Color.FromArgb(128, 0, 0, 0));
        }
    }
}

