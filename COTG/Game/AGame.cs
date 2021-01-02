
using COTG.Draw;
using COTG.Game;
using COTG.Helpers;
using COTG.JSON;
using COTG.Views;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using static COTG.Debug;
using static COTG.Game.Enum;
using static COTG.CanvasHelpers;


using UWindows = Windows;
using Vector2 = System.Numerics.Vector2;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Layer = COTG.Draw.Layer;

namespace COTG
{
	public static partial class Helper
	{
		static public CoreWindow CoreWindow => Window.Current.CoreWindow;
		static public UIElement CoreContent => Window.Current.Content;
	}
	public class AGame : Microsoft.Xna.Framework.Game
	{
		public static SwapChainPanel canvas;
		public static AGame instance;
		public static GraphicsDevice device=>instance.GraphicsDevice;
		public static GraphicsDeviceManager _graphics;
		public static Draw.SpriteBatch spriteBatch;
		public static SpriteFont font;
		const float detailsZoomThreshold = 36;
		const float detailsZoomFade = 8;
		public static Material worldBackground;
		//public static Effect imageEffect;
		public static Effect defaultEffect;
		public static Material lineDraw;
		public static Material quadTexture;
		public static Material roundedRect;
		public static Material sky;
		public static Material webMask;
		//    public static TintEffect worldBackgroundDark;
		public static Material worldObjects;
		public static Material worldOwners;
		//     public static TintEffect worldObjectsDark;
		public static Material worldChanges;
	//	public static Vector2 clientTL;
		public static Vector2 cameraC = new Vector2(300, 300);
		public static Vector2 cameraCLag = cameraC; // for smoothing
//		public static Vector2 clientC;
//		public static Vector2 clientCScreen;
		public static Vector2 clientSpan;
		public static Vector2 halfSpan;
		static Vector2 windowSpan = new Vector2(800, 600);
		static Army underMouse;
		static float bestUnderMouseScore;
		//   public static Vector2 cameraMid;
		public static float cameraZoom = 64;
		public static float cameraZoomLag = 64;
		
		public float eventTimeOffsetLag;
		public float eventTimeEnd;
		static public Color nameColor, nameColorHover, myNameColor, nameColorIncoming, nameColorSieged, nameColorIncomingHover, nameColorSiegedHover, myNameColorIncoming, myNameColorSieged, shadowColor;
	//	static CanvasLinearGradientBrush tipBackgroundBrush, tipTextBrush;
	//	static CanvasTextFormat tipTextFormat = new CanvasTextFormat() { FontSize = 14, WordWrapping = CanvasWordWrapping.NoWrap, FontStretch = fontStretch };
	//	static CanvasTextFormat tipTextFormatCentered = new CanvasTextFormat() { FontSize = 12, HorizontalAlignment = CanvasHorizontalAlignment.Center, VerticalAlignment = CanvasVerticalAlignment.Center, WordWrapping = CanvasWordWrapping.NoWrap, FontStretch = fontStretch };
	//	static CanvasTextFormat nameTextFormat = new CanvasTextFormat()
//		{
//			FontSize = 11,
//			HorizontalAlignment = CanvasHorizontalAlignment.Center,
//			VerticalAlignment = CanvasVerticalAlignment.Center,
//			FontStretch = fontStretch,
//			//		FontWeight= FontWeights.Bold,
//
//			WordWrapping = CanvasWordWrapping.NoWrap,
			//	 Options= CanvasDrawTextOptions.NoPixelSnap,

		//};
		//        static readonly Color attackColor = Color.DarkRed;
		static readonly Color attackColor = Color.White;
		static readonly Color defenseColor = new Color(255, 20, 160, 160);
		static readonly Color defenseArrivedColor = new Color(255, 20, 255, 160);
		static readonly Color artColor = Color.DarkOrange;
		static readonly Color senatorColor = Color.OrangeRed;
		static readonly Color defaultAttackColor = Color.Maroon;// (0xFF8B008B);// Color.DarkMagenta;
		static readonly Color raidColor = Color.Yellow;
		//        static readonly Color shadowColor = new Color(128, 0, 0, 0);
		static readonly Color selectColor = new Color(255, 20, 255, 192);
		static readonly Color buildColor = Color.DarkRed;
		static readonly Color hoverColor = Color.Purple;
		static readonly Color focusColor = Color.Magenta;
		static readonly Color pinnedColor = Color.Teal;
		static readonly Color black0Alpha = new Color() { A = 0, R = 0, G = 0, B = 0 };
		public static Material[] troopImages = new Material[Game.Enum.ttCount];
		static Vector2 troopImageOriginOffset;
		const int maxTextLayouts = 1024;
		public static bool initialized => canvas != null;
		static Dictionary<int, TextLayout> nameLayoutCache = new Dictionary<int, TextLayout>();
		static public TextLayout GetTextLayout( string name,TextFormat format)
		{
			var hash = name.GetHashCode(StringComparison.Ordinal);
			if (nameLayoutCache.TryGetValue(name.GetHashCode(StringComparison.Ordinal), out var rv))
				return rv;
			rv = new TextLayout( name, format);

			if (nameLayoutCache.Count >= maxTextLayouts)
				nameLayoutCache.Remove(nameLayoutCache.First().Key);
			nameLayoutCache.Add(hash, rv);

			return rv;

		}
		public AGame()
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferFormat = SurfaceFormat.Color,
				PreferMultiSampling = false,
				PreferredDepthStencilFormat = DepthFormat.None,
			
			};
			IsFixedTimeStep = false;
			_graphics.PreparingDeviceSettings += _graphics_PreparingDeviceSettings;
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		private void _graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			var inf = e.GraphicsDeviceInformation;
			inf.GraphicsProfile = GraphicsProfile.HiDef;
			inf.PresentationParameters.SwapChainPanel = canvas;
			inf.PresentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;
			if (clientSpan.X > 0 && clientSpan.Y > 0)
			{
				inf.PresentationParameters.BackBufferHeight = (int)clientSpan.Y;
				inf.PresentationParameters.BackBufferWidth = (int)clientSpan.X;
			}
		}

		static public void Create(SwapChainPanel swapChainPanel)
		{
			canvas = swapChainPanel;
			instance = MonoGame.Framework.XamlGame<AGame>.Create( ()=> new AGame() { },"", Helper.CoreWindow, swapChainPanel);
		}

		protected override void Initialize()
		{
			instance = this;
			base.Initialize();

		

		}

	

		bool inputSetup;
		public static Material CreateFromBytes(byte[] pixels, int x, int y, SurfaceFormat format)
		{
			var rv = new Texture2D(instance.GraphicsDevice, x,y, false, format);
			rv.SetData(pixels);
			
			return new Material(rv);
		}
		public static Material LoadTexture(string filename)
		{
			var rv = instance.Content.Load<Texture2D>(filename);
			return new Material(rv);
		}
		public static MouseState mouseState;
		public static MouseState priorMouseState;
		public static KeyboardState keyboardState;
		public static KeyboardState priorKeyboardState;
		protected override void Update(GameTime gameTime)
		{
			
			try
			{
				if (clientSpan.X > 0 && clientSpan.Y > 0)
				{
					if (resolutionDirtyCounter > 0)
					{
						if (--resolutionDirtyCounter == 0)
						{
//									_graphics.PreferredBackBufferHeight = (int)clientSpan.Y;
	//								_graphics.PreferredBackBufferWidth = (int)clientSpan.X;
		//					_graphics.ApplyChanges();
							var pre = new PresentationParameters()
							{
								BackBufferFormat = SurfaceFormat.Color,
								DepthStencilFormat = DepthFormat.None,
								SwapChainPanel = canvas,
								RenderTargetUsage = RenderTargetUsage.DiscardContents,
								BackBufferWidth = (int)clientSpan.X,// - ShellPage.cachedXOffset,
							BackBufferHeight = (int)clientSpan.Y, // - ShellPage.cachedTopOffset,
							};
							GraphicsDevice.Reset(pre);
						}
					}
				}



				worldLightC = ShellPage.CameraToWorld(cameraLightC);
				priorMouseState = mouseState;
				priorKeyboardState = keyboardState;
				keyboardState = Keyboard.GetState();
				App.canvasKeyModifiers = UWindows.System.VirtualKeyModifiers.None;
				if ((keyboardState.IsKeyDown(Keys.LeftShift) | keyboardState.IsKeyDown(Keys.RightShift)))
					App.canvasKeyModifiers |= UWindows.System.VirtualKeyModifiers.Shift;
				if ((keyboardState.IsKeyDown(Keys.LeftControl) | keyboardState.IsKeyDown(Keys.RightControl)))
					App.canvasKeyModifiers |= UWindows.System.VirtualKeyModifiers.Control;

				//if (resolutionDirty)
				//{
				//	resolutionDirty = false;
				//	if (clientSpan.X > 0 && clientSpan.Y > 0)
				//	{
				//		_graphics.PreferredBackBufferHeight = (int)clientSpan.Y;
				//		_graphics.PreferredBackBufferWidth = (int)clientSpan.X;
				////		_graphics.ApplyChanges();
				//	}
				//}

				mouseState = Mouse.GetState();
				ShellPage.Canvas_PointerMoved(mouseState, priorMouseState);
				ShellPage.Canvas_PointerWheelChanged(mouseState, priorMouseState);
				ShellPage.Canvas_PointerPressed(mouseState, priorMouseState);

				if (!inputSetup && IsActive)
				{
					inputSetup = true;
					ShellPage.SetupCoreInput();
				}

				if (World.bitmapPixels != null)
				{
					// canvas.Paused = true;
					var pixels = World.bitmapPixels;
					var ownerPixels = World.worldOwnerPixels;
					World.bitmapPixels = null;
					World.worldOwnerPixels = null;
					if (worldObjects != null)
					{
						var w = worldObjects;
						worldObjects = null;
						w.texture.Dispose();
					}
					if (worldOwners != null)
					{
						var w = worldOwners;
						worldOwners = null;
						w.texture.Dispose();
					}
					worldObjects = CreateFromBytes(pixels, World.outSize, World.outSize, SurfaceFormat.Dxt1a);
					worldOwners = CreateFromBytes(ownerPixels, World.outSize, World.outSize, SurfaceFormat.Dxt1a);
					//canvas.Paused = falwirse;
					//if (worldObjectsDark != null)
					//    worldObjectsDark.Dispose();
					//worldObjectsDark = new TintEffect() { BufferPrecision = CanvasBufferPrecision.Precision8UIntNormalizedSrgb, Source = worldObjects, Color = new Color() { A = 255, R = 128, G = 128, B = 128 } };

				}
				if (World.changePixels != null)
				{
					var pixels = World.changePixels;
					ClearHeatmapImage();
					worldChanges = CreateFromBytes(pixels, World.outSize, World.outSize, SurfaceFormat.Dxt1a);

				}
				//if(JSClient.webViewBrush!=null)
				//	App.DispatchOnUIThread(
				//		() =>
				//		{
				//			JSClient.webViewBrush.SourceName = "cotgView";
				//			JSClient.webViewBrush.SetSource(JSClient.view);
				//			JSClient.webViewBrush.Redraw();
				//			ShellPage.canvasHitTest.Fill = JSClient.webViewBrush;
				//		});
			}
			catch (Exception _exception)
			{
				COTG.Debug.Log(_exception);
			}



		}
	




	public static void ClearHeatmapImage()
	{
		World.changePixels = null;
		if (worldChanges != null)
		{
			var w = worldChanges;
			worldChanges = null;
			w.texture.Dispose();
		}
		World.changeMapInProgress = false;// this is used to temporarily block the UI from issuing multiple changes at once

	}
	public static void ClearHeatmap()
	{
		ClearHeatmapImage();
		World.rawPrior = null;
	}
	//public static void SetCanvasVisibility(bool visible)
	//{
	//    if (canvas.Visibility == Visibility.Visible)
	//    {
	//        if (!visible)
	//            canvas.Visibility = Visibility.Collapsed;
	//    }
	//    else
	//    {
	//        if (visible)
	//            canvas.Visibility = Visibility.Visible;
	//    }


	//}

	public static void SetClientSpan(Vector2 span)
	{
		clientSpan.X = span.X - (span.X % 8);
		clientSpan.Y = span.Y - (span.Y % 8);
		halfSpan = clientSpan * 0.5f;
	}
		public static int resolutionDirtyCounter;

	public static void Canvas_LayoutUpdated(object sender, object e)
	{
			// not yet initialized?
			if(!initialized)
			{
				return;
			}
		var c = canvas.ActualOffset;

//		clientC = new Vector2(c.X, c.Y);
		SetClientSpan(canvas.ActualSize);
	//	clientCScreen = canvas.TransformToVisual(Helper.CoreContent)
	//		.TransformPoint(new UWindows.Foundation.Point(0, 0)).ToVector2();
			resolutionDirtyCounter = 60;
	}
	public static void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
	{
			if (!initialized)
			{
				return;
			}
			SetClientSpan(new Vector2((float)e.NewSize.Width, (float)e.NewSize.Height) );
			//clientCScreen = canvas.TransformToVisual(Helper.CoreContent)
			//	.TransformPoint(new UWindows.Foundation.Point(0, 0)).ToVector2();
			//	canvas.RunOnGameLoopThreadAsync(RemakeRenderTarget);

			Log(canvas.CompositionScaleX);

			var bounds = Helper.CoreWindow.Bounds;
			windowSpan = new Vector2((float)bounds.Width,(float)bounds.Height);
			resolutionDirtyCounter = 60;
		}

		//class Disposer
		//{
		//	List<IDisposable> disposables = new List<IDisposable>();
		//	public Disposer Add<T>(ref T d) where T : IDisposable
		//	{
		//		if (d != null)
		//		{
		//			disposables.Add(d);
		//			d = default;
		//		}
		//		return this;
		//	}
		//	public void DoDispose()
		//	{
		//		var l = disposables;
		//		disposables = null;
		//		foreach (var d in l)
		//		{
		//			d.Dispose();
		//		}
		//	}
		//}
		//static void RemakeRenderTarget()
		//{
		//	if (renderTarget != null)
		//		renderTarget.Dispose();
		//	//var margin = new Thickness(cachedXOffset - cotgPanelRight, cachedTopOffset, 0, bottomMargin);
		//	renderTarget = new CanvasRenderTarget(canvas, (float)(clientSpan.X), (float)(clientSpan.Y), canvas.Dpi, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, CanvasAlphaMode.Premultiplied);

		//}
		public static bool readyToLoad;
		public static Song music;
		public static void UpdateMusic()
		{

			if (SettingsPage.musicVolume > 0)
			{
				if (music == null  )
				{
					if (!readyToLoad)
						return;
					music = instance.Content.Load<Song>("Audio/UXOMainTheme");
				}

				MediaPlayer.Volume = SettingsPage.musicVolume;
				MediaPlayer.IsRepeating = true;
				MediaPlayer.Play(music);
			}
			else
			{
				if(MediaPlayer.State!=MediaState.Stopped)
					MediaPlayer.Stop();
			}
		}
		protected override async void LoadContent()
		{
			defaultEffect = Content.Load<Effect>("Effects/DefaultEffect");
			readyToLoad = true;

			SpriteAnim.flagHome.Load();
			SpriteAnim.flagSelected.Load();
	
			draw = new COTG.Draw.SpriteBatch(GraphicsDevice);
			worldBackground = LoadTexture("Art/world");
			font = Content.Load<SpriteFont>("Fonts/perp");
			font.defaultMaterial = new Material(font.Texture);
			// worldBackgroundDark = new TintEffect() { BufferPrecision = CanvasBufferPrecision.Precision8UIntNormalizedSrgb, Source = worldBackground, Color = new Color() { A = 255, R = 128, G = 128, B = 128 } };

			

			lineDraw = LoadTexture("Art/lineDraw");
		//lineDraw2 = new PixelShaderEffect(
		sky = LoadTexture("Art/sky");
			roundedRect = new Material(Content.Load<Texture2D>("Art/Icons/roundRect"));
			quadTexture = new Material(Content.Load<Texture2D>("Art/Icons/roundRect"));
			for (int i = 0; i < COTG.Game.Enum.ttCount; ++i)
		{

			troopImages[i] = LoadTexture($"Art/icons/troops{i}");
			if (i == 0)
			{
				troopImageOriginOffset.X = (float)troopImages[i].Width * 0.5f;
				troopImageOriginOffset.Y = (float)troopImages[i].Height * 0.625f;
			}
		}
		//// create a full screen rendertarget
		//RemakeRenderTarget();

		//tipBackgroundBrush = new CanvasLinearGradientBrush(canvas, new CanvasGradientStop[]
		//	   {
		//				new CanvasGradientStop() { Position = 0.0f, Color = new Color(){A=255,R=128,G=64,B=64 } },
		//				new CanvasGradientStop() { Position = 1.0f, Color = Color.Black } }, CanvasEdgeBehavior.Clamp, CanvasAlphaMode.Premultiplied)
		//{ Opacity = 0.675f };
		//tipTextBrush = new CanvasLinearGradientBrush(canvas, new CanvasGradientStop[]
		//{
		//				new CanvasGradientStop() { Position = 0.0f, Color = Color.White },
		//				new CanvasGradientStop() { Position = 1.0f, Color = Color.Blue }
		//}, CanvasEdgeBehavior.Clamp, CanvasAlphaMode.Premultiplied);
		//;

		//if (args.Reason != Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesReason.FirstTime)
		{
		//	TileData.Ctor(true);
		}


			//var lightEffectBytes = await App.GetContent("shader/light.bin");
			//	var lightEffect = new Effect(instance.GraphicsDevice, lightEffectBytes)
			//{

			//	//Source2 = sky,
			//	//Source2Mapping = SamplerCoordinateMapping.Unknown,
			//	//Source2Interpolation = CanvasImageInterpolation.Linear,
			//	//Source2BorderMode = EffectBorderMode.Soft,
			//	//CacheOutput = false,
			//	//Source1BorderMode = EffectBorderMode.Soft,
			//	//Source1Mapping = SamplerCoordinateMapping.Offset,
			//	//MaxSamplerOffset = 4,
			//	Name = "SSLighting"

			//	//    Source2 = await CanvasBitmap.LoadAsync(sender, "Shaders/SketchTexture.jpg"),
			//	//   Source2Mapping = SamplerCoordinateMapping.Unknown
			//};


			UpdateMusic();
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
	const float bSizeGain3 = bSizeGain * bSizeGain / bSizeGain2;
	public static float pixelScale = 1;
	public static float circleRadiusBase = 1.0f;
	public static float shapeSizeGain = 1.0f;
	public static float pixelScaleInverse = 1;
		//	const float dashLength = (dashD0 + dashD1) * lineThickness;
 public	static Draw.SpriteBatch draw;

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
		return x1 + pad <= 0 | x0 - pad >= clientSpan.X |
				y1 + pad <= 0 | y0 - pad >= clientSpan.Y;
	}




	public static Vector2 shadowOffset = new Vector2(lineThickness * 1.0f, lineThickness * 1.0f);
	public static Vector2 halfShadowOffset = new Vector2(lineThickness * 0.75f, lineThickness * 0.7f);
	public static void SetCameraCNoLag(Vector2 c) => cameraCLag = cameraC = c;
	static DateTimeOffset lastDrawTime;
	public static bool tileSetsPending;
	private const float smallRectSpan = 4;
	public const float lightZ0 = 460f;
	public static Vector2 cameraLightC;
	public static Vector2 worldLightC;
	private float cameraZ = 260;
	public static float animationTWrap; // wraps every 3 seconds
	public static float animationT; // approximate animation time in seconds
		private TextFormat textformatLabel = new TextFormat(TextFormat.HorizontalAlignment.center, TextFormat.VerticalAlignment.center);
		private TextFormat tipTextFormatCentered = new TextFormat(TextFormat.HorizontalAlignment.center);
		private TextFormat tipTextFormat = new TextFormat(TextFormat.HorizontalAlignment.left);
		private TextFormat nameTextFormat = new TextFormat(TextFormat.HorizontalAlignment.center, TextFormat.VerticalAlignment.center);


		const float lineTileGain = 1.0f/32.0f;

		//	static CanvasTextAntialiasing canvasTextAntialiasing = CanvasTextAntialiasing.Grayscale;
		//	static CanvasTextRenderingParameters canvasTextRenderingParameters = new CanvasTextRenderingParameters(CanvasTextRenderingMode.NaturalSymmetric, CanvasTextGridFit.Disable);
		protected override void Draw(GameTime gameTime)
	{
		underMouse = null;
		bestUnderMouseScore = 32 * 32;
		if (!(IsWorldView()) || (TileData.state < TileData.State.loadingImages) || (worldOwners == null))
			return;


		parralaxZ0 = 1024 * 64.0f / cameraZoomLag;


		try
		{
			var _serverNow = JSClient.ServerTime();
			var dt = (float)(_serverNow - lastDrawTime).TotalSeconds;
			lastDrawTime = _serverNow;

			var gain = (1 - MathF.Exp(-4 * dt));
			cameraCLag += (cameraC - cameraCLag) * gain;
			cameraZoomLag += (cameraZoom - cameraZoomLag) * gain;
			eventTimeOffsetLag += (ShellPage.instance.eventTimeOffset - eventTimeOffsetLag) * gain;
			cameraLightC += (ShellPage.mousePosition - cameraLightC) * gain;
			//                cameraZoomLag += (cameraZoom

			var serverNow = _serverNow + TimeSpan.FromMinutes(eventTimeOffsetLag);

			// not too high or we lose float precision
			// not too low or people will see when when wraps
			animationT = ((uint)Environment.TickCount % 0xffffff) * (1.0f / 1000.0f);

			//{
			//	var i = (int)(animationT / 4.0f);
			//	var stretchCount = FontStretch.UltraExpanded - FontStretch.UltraCondensed+1;
			//	fontStretch = FontStretch.UltraCondensed + (i % stretchCount);
			//	tipTextFormat.FontStretch = fontStretch;
			//	tipTextFormatCentered.FontStretch = fontStretch;
			//}
			animationTWrap = ((uint)Environment.TickCount % 3000) * (1.0f / 3000); // wraps every 3 seconds, 0..1
																				//				float accentAngle = animT * MathF.PI * 2;
			int tick = (Environment.TickCount >> 3) & 0xfffff;
			var animTLoop = animationTWrap.Wave();
			int cx0 = 0, cy0 = 0, cx1 = 0, cy1 = 0;
			var rectSpan = animTLoop.Lerp(rectSpanMin, rectSpanMax);
			//   ShellPage.T("Draw");

			var notFaded = true;
		//	defaultStrokeStyle.DashOffset = (1 - animT) * dashLength;


			//                ds.Blend = ( (int)(serverNow.Second / 15) switch { 0 => CanvasBlend.Add, 1 => CanvasBlend.Copy, 2 => CanvasBlend.Add, _ => CanvasBlend.SourceOver } );




			//ds.TextRenderingParameters = new CanvasTextRenderingParameters(!App.IsKeyPressedControl() ? CanvasTextRenderingMode.Outline : CanvasTextRenderingMode.Default, CanvasTextGridFit.Default);

			//              ds.TextRenderingParameters = new CanvasTextRenderingParameters(CanvasTextRenderingMode.Default, CanvasTextGridFit.Disable);
			// var scale = ShellPage.canvas.ConvertPixelsToDips(1);
			pixelScale = (cameraZoomLag);
			pixelScaleInverse = 1.0f / cameraZoomLag;
			shapeSizeGain = MathF.Sqrt(pixelScale * (1.50f / 64.0f));
			var deltaZoom = cameraZoomLag - detailsZoomThreshold;
			var wantDetails = deltaZoom > 0;
			var wantImage = deltaZoom < detailsZoomFade;


			// workd space coords
			var srcP0 = new Vector2((cameraCLag.X + 0.5f) * bSizeGain2 - halfSpan.X * bSizeGain2 * pixelScaleInverse,
									  (cameraCLag.Y + 0.5f) * bSizeGain2 - halfSpan.Y * bSizeGain2 * pixelScaleInverse);
			var srcP1 = new Vector2(srcP0.X + clientSpan.X * bSizeGain2 * pixelScaleInverse,
								   srcP0.Y + clientSpan.Y * bSizeGain2 * pixelScaleInverse);
			var destP0 = new Vector2(0,0);
			var destP1 = clientSpan;

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

			var attacksVisible = DefenseHistoryTab.IsVisible() | OutgoingTab.IsVisible() | IncomingTab.IsVisible() | HitTab.IsVisible() | AttackTab.IsVisible();
			var wantDesaturate = true || attacksVisible;

			var wantLight = SettingsPage.wantLight;
			var wantParallax = SettingsPage.wantParallax;

				//var gr = spriteBatch;// spriteBatch;// wantLight ? renderTarget.CreateDrawingSession() : args.DrawingSession;
			
				//		ds.Blend = CanvasBlend.Copy;

			// funky logic
			//if (wantLight)
				GraphicsDevice.Clear(new Color()); // black transparent
												   //ds.TextAntialiasing = canvasTextAntialiasing;
												   //ds.TextRenderingParameters = canvasTextRenderingParameters;
												   // prevent MSAA gaps
				GraphicsDevice.BlendState = BlendState.NonPremultiplied;
				GraphicsDevice.DepthStencilState = DepthStencilState.None;
				
				GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
				GraphicsDevice.RasterizerState = RasterizerState.CullNone;
				
				{
					var viewport = GraphicsDevice.Viewport;


					var proj = Matrix.CreateOrthographicOffCenter(0f, viewport.Width, viewport.Height,0, 0, -1);
//					var proj = Matrix.CreateOrthographicOffCenter(480, 1680, 1680, 480, 0, -1);
					defaultEffect.Parameters["WorldViewProjection"].SetValue(proj);
					defaultEffect.Parameters["DiffuseColor"].SetValue(new Microsoft.Xna.Framework.Vector4(1, 1, 1, 1));
					
					draw.Begin();
					
				}

				//	ds.Antialiasing = CanvasAntialiasing.Aliased;
				if (worldBackground != null && wantImage)
			{

				if (wantImage)
				{
						const float texelGain = 1.0f / srcImageSpan;
						draw.AddQuad(COTG.Draw.Layer.background, worldBackground,
							destP0, destP1, srcP0* texelGain, srcP1* texelGain,
						 255.AlphaToColor() );

					if (worldObjects != null)
						draw.AddQuad(COTG.Draw.Layer.background+1,worldObjects,
							destP0.CToCp(TileData.zCities), destP1.CToCp(TileData.zCities), srcP0 * texelGain, srcP1 * texelGain, 255.AlphaToColor());


				}


			}
				

				//   ds.Antialiasing = CanvasAntialiasing.Antialiased;
				// ds.Transform = new Matrix3x2( _gain, 0, 0, _gain, -_gain * ShellPage.cameraC.X, -_gain * ShellPage.cameraC.Y );

				//           dxy.X = (float)sender.Width;
				//            dxy.Y = (float)sender.ActualHeight;

				//            ds.DrawLine( SC(0.25f,.125f),SC(0.lineThickness,0.9f), raidBrush, lineThickness,defaultStrokeStyle);
				//           ds.DrawLine(SC(0.25f, .125f), SC(0.9f, 0.lineThickness), shadowBrush, lineThickness, defaultStrokeStyle);
				// if (IsPageDefense())

				if (wantDetails)
			{
				var wantFade = wantImage;
				var alpha = wantFade ? (deltaZoom / detailsZoomFade).Min(1) : 1.0f;
				var intAlpha = (byte)(alpha * 255.0f).RoundToInt();
					
				var rgb = attacksVisible ? 255 : 255;
				var tint = new Color(rgb, rgb, rgb, intAlpha);
				var tintShadow = new Color(0, 0,32, intAlpha/2);
				//	var tintAlpha = (byte)(alpha * 255.0f).RoundToInt();

				if (wantDesaturate)
				{
					nameColor = new Color() { A = intAlpha, G = 0, B = 32, R = 0 };
					nameColorHover = new Color() { A = intAlpha, G = 80, R = 80, B = 160 };
					myNameColor = new Color() { A = intAlpha, G = 255 / 3, B = 190 / 3, R = 210 / 3 };
					nameColorIncoming = new Color() { A = intAlpha, G = 220 / 3, B = 220 / 3, R = 255 / 3 };
					nameColorSieged = new Color() { A = intAlpha, G = 220 / 3, B = 190 / 3, R = 255 / 3 };
					nameColorIncomingHover = new Color() { A = intAlpha, G = 220 / 3, B = 160 / 3, R = 255 / 3 };
					nameColorSiegedHover = new Color() { A = intAlpha, G = 220 / 3, B = 140 / 3, R = 255 / 3 };
					myNameColorIncoming = new Color() { A = intAlpha, G = 240 / 3, B = 150 / 3, R = 255 / 3 };
					myNameColorSieged = new Color() { A = intAlpha, G = 240 / 3, B = 120 / 3, R = 255 / 3 };
				}
				else
				{
					nameColor = new Color() { A = intAlpha, G = 255, B = 255, R = 255 };
					nameColorHover = new Color() { A = intAlpha, G = 255, B = 255, R = 185 };
					myNameColor = new Color() { A = intAlpha, G = 255, B = 190, R = 210 };
					nameColorIncoming = new Color() { A = intAlpha, G = 220, B = 220, R = 255 };
					nameColorSieged = new Color() { A = intAlpha, G = 220, B = 190, R = 255 };
					nameColorIncomingHover = new Color() { A = intAlpha, G = 220, B = 160, R = 255 };
					nameColorSiegedHover = new Color() { A = intAlpha, G = 220, B = 140, R = 255 };
					myNameColorIncoming = new Color() { A = intAlpha, G = 240, B = 150, R = 255 };
					myNameColorSieged = new Color() { A = intAlpha, G = 240, B = 120, R = 255 };
				}
				shadowColor = new Color() { A = 192 };

				var td = TileData.instance;
				var halfTiles = (clientSpan * (0.5f / cameraZoomLag));
				var _c0 =  (cameraCLag - halfTiles);
				var _c1 = cameraCLag + halfTiles;
				cx0 = _c0.X.FloorToInt().Max(0);
				cy0 = (_c0.Y.FloorToInt()).Max(0);
				cx1 = (_c1.X.CeilToInt()+1).Min(World.worldDim);
				cy1 = (_c1.Y.CeilToInt() + 2).Min(World.worldDim);
				const bool isShift = false;// App.IsKeyPressedShift();
				const float tcOff = isShift ? 0.0f : 0.0f;
				const float tzOff = isShift ? 0.0f : 0.0f;
				float parallaxGain = wantParallax ? 1.0f : 0.0f;
				{
					// 0 == land
					// 1 == shadows
					// 2 == features
					for (int pass = 0; pass < 3; ++pass)
					{
						if (pass == 1 && (!wantParallax || !wantLight))
							continue;
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
											if (tile.isBase != (pass == 0))
											{
												continue;
											}
											if ((pass == 1) && (!tile.wantShadow || !layer.wantShadow))
											{
												continue;
											}
											var _tint = (pass == 1) ? tintShadow : (pass == 2) ? World.GetTint(ccid) : tint;
											if((pass == 2))
												_tint.A = intAlpha;;
											var dz = tile.z * parallaxGain; // shadows draw at terrain level 

											var scale = (pass == 1) ? CanvasHelpers.ParalaxScaleShadow(dz) : CanvasHelpers.ParalaxScale(dz);
											var wc = new Vector2(cx - .5f, cy - 0.5f);
											var cc = (pass == 1) ?  wc.WToC(): wc.WToCp(dz);
										

											var sy = off / tile.columns;
											var sx = off - sy * tile.columns;
												var uv0 = new Vector2((sx) * tile.scaleXToU + tile.halfTexelU , (sy ) * tile.scaleYToV+tile.halfTexelV );
												var uv1 = new Vector2((sx + 1) * tile.scaleXToU+tile.halfTexelU , (sy + 1) * tile.scaleYToV  - tile.halfTexelV );
												
												draw.AddQuad(pass switch { 0=>Layer.tileBase+layer.id, 1=> Layer.tileShadow,2=>Layer.tiles+layer.id }, tile.bitmap, cc, cc + new Vector2(pixelScale * scale, pixelScale * scale),
													uv0,
													uv1 , _tint);



										}


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


			// fade out background
			//if (attacksVisible)
			//{
			//    ds.FillRectangle(new Rect(new Point(), clientSpan.ToSize()), desaturateBrush);
			//    notFaded=false;
			//}

			//    ds.Antialiasing = CanvasAntialiasing.Antialiased;
			if (worldChanges != null)
				draw.AddQuad(Layer.effects-1, worldChanges,
					destP0.CToCp(TileData.zCities), destP1.CToCp(TileData.zCities),
					srcP0, srcP1,255.AlphaToColor() );


			circleRadiusBase = circleRadMin * shapeSizeGain * 7.9f;
			var circleRadius = animTLoop.Lerp(circleRadMin, circleRadMax) * shapeSizeGain * 6.5f;
			//    var highlightRectSpan = new Vector2(circleRadius * 2.0f, circleRadius * 2);

			//	ds.FillRectangle(new Rect(0, 0, clientSpan.X, clientSpan.Y), JSClient.webViewBrush);



			{
				var defenderVisible = IncomingTab.IsVisible() || NearDefenseTab.IsVisible();
				var outgoingVisible = OutgoingTab.IsVisible();
				{
					if (DefenseHistoryTab.IsVisible() || HitTab.IsVisible())
					{
						for (var dfof = 0; dfof < 2; ++dfof)
						{
							if (dfof == 0)
							{
								if (!DefenseHistoryTab.IsVisible())
									continue;
							}
							else
							{
								if (!HitTab.IsVisible())
									continue;

							}
							var reports = dfof == 0 ? DefenseHistoryTab.instance.history : HitTab.instance.history;

							if (reports.Length > 0)
							{
								var autoShow = reports.Length <= SettingsPage.showAttacksLimit;

								var counts = new Dictionary<int, IncomingCounts>();

								foreach (var attack in reports)
								{
									if (attack.type == COTG.Game.Enum.reportPending)
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
									var c1 = targetCid.CidToCp(TileData.zLabels);
									var c0 = sourceCid.CidToCp(TileData.zLabels);
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
									if (!Spot.IsSelectedOrHovered(targetCid, sourceCid, autoShow))
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
										DrawAction( dt1, journeyTime, r, c0, c1, c, troopImages[attack.troops[iType].type], true, attack, 28, alpha);
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
									var c = cid.CidToCp(TileData.zLabels);
									DrawTextBox( $"{count.prior}`{count.incoming}", c,textformatLabel, Color.DarkOrange, notFaded);


								}
							}
						}
					}
					if (AttackTab.IsVisible())
					{
						if (AttackTab.attackClusters != null)
						{
							foreach (var cluster in AttackTab.attackClusters)
							{
								var selected = false;
								foreach (var i in cluster.attacks)
								{
									if (Spot.IsSelectedOrHovered(i, true))
									{
										selected = true;
										break;
									}
								}
								foreach (var i in cluster.targets)
								{
									if (Spot.IsSelectedOrHovered(i, true))
									{
										selected = true;
										break;
									}
								}
								{
									var c0 = cluster.topLeft.WToCp(TileData.zLabels);
									var c1 = cluster.bottomRight.WToCp(TileData.zLabels);
									DrawRect(Layer.effects, c0, c1,  selected ? Color.Black : Color.Maroon);
								}

								if (selected)
								{
									var real = cluster.real;
									var c0 = real.CidToCp(TileData.zLabels);
									foreach (var a in cluster.attacks)
									{
										var t = (tick * a.CidToRandom().Lerp(1.5f / 512.0f, 1.75f / 512f)) + 0.25f;
										var r = t.Ramp();
										var c1 = a.CidToCp(TileData.zLabels);
										var spot = Spot.GetOrAdd(a);
										DrawAction( 0.5f, 1.0f, r, c1, c0, Color.Red, troopImages[(int)spot.GetPrimaryTroopType(false)], false, null, 16);
									}
									foreach (var target in cluster.targets)
									{
										var c = target.CidToCp(TileData.zLabels);
										var rnd = target.CidToRandom();

										var t = (tick * rnd.Lerp(1.5f / 512.0f, 1.75f / 512f)) + 0.25f;
										var r = t.Wave().Lerp(circleRadiusBase, circleRadiusBase * 1.325f);
										DrawAccent(target, 0.2f, Color.White);
									}

								}
							}
						}

						//foreach (var t in AttackTab.readable.targets)
						//{
						//    var c1 = t.cid.CidToCC();
						//    DrawTextBox(ds, $"{Spot.GetOrAdd(t.cid).classificationString}", c1, tipTextFormatCentered, t.attackCluster == 0 ? Color.White : Color.Teal);
						//}
						//foreach (var t in AttackTab.readable.attacks)
						//{
						//  //  DrawTextBox(ds, $"{Spot.GetOrAdd(t.cid).classificationString}", c1, tipTextFormatCentered, t.attackCluster == 0 ? Color.White : Color.Teal);

						//    //if (t.target != 0)
						//    //{
						//    //    var _t = (tick * t.cid.CidToRandom().Lerp(1.5f / 512.0f, 2.0f / 512f)) + 0.25f;
						//    //    var r = _t.Ramp();
						//    //    var c = t.fake ? Color.White : Color.Red;
						//    //    var c0 = t.cid.CidToCC();
						//    //    var c1 = t.target.CidToCC();
						//    //    //   DrawTextBox(ds, $"{t.type} {t.fake} {t.player}", c1, tipTextFormatCentered);
						//    //    DrawAction(ds, batch, .5f, 1.0f, r, c0, c1, c, troopImages[t.troopType], false, null, 28, 0.8f);

						//    //}
						//}
					}
					if ((defenderVisible || outgoingVisible))
					{
						var cullSlopSpace = 80 * pixelScale;
						for (int iOrO = 0; iOrO < 2; ++iOrO)
						{
							var defenders = (iOrO == 0);
							if (defenders)
							{
								if (!defenderVisible)
									continue;
							}
							else
							{
								if (!outgoingVisible)
									continue;
							}
							var list = defenders ? Spot.defendersI : Spot.defendersO;
							bool noneIsAll = list.Length <= SettingsPage.showAttacksLimit;
							foreach (var city in list)
							{
								if (city.incoming.Any() || city.isMine)
								{

									var targetCid = city.cid;
									var c1 = targetCid.CidToCp(TileData.zLabels);
									if (IsCulled(c1, cullSlopSpace))  // this is in pixel space - Should be normalized for screen resolution or world space (1 continent?)
										continue;
									var incAttacks = 0;
									var incTs = 0;
									foreach (var i in city.incoming)
									{
										var c0 = i.sourceCid.CidToCp(TileData.zLabels);
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
										if (!Spot.IsSelectedOrHovered(i.sourceCid, targetCid, noneIsAll))
										{
											continue;
											//       c.A = (byte)((int)c.A * 3 / 8); // reduce alpha if not selected
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

											DrawAction( i.TimeToArrival(serverNow), i.journeyTime, r, c0, c1, c, troopImages[i.troops[iType].type], true, i, 28, alpha);
										}
										else
										{
											Assert(false);
										}
									}
									if (wantDetails || Spot.IsSelectedOrHovered(targetCid, noneIsAll))
										DrawTextBox( $"{incAttacks}`{city.claim.ToString("00")}%`{(incTs + 500) / 1000}k\n{ (city.tsDefMax.Max(city.tsHome) + 500) / 1000 }k", c1, tipTextFormatCentered, incAttacks != 0 ? Color.White : Color.Cyan, notFaded);
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
									var c1 = targetCid.CidToCp(TileData.zLabels);
									if (IsCulled(c1, cullSlopSpace))  // this is in pixel space - Should be normalized for screen resolution or world space (1 continent?)
										continue;
									if (wantDetails || Spot.IsSelectedOrHovered(targetCid, true))
										DrawTextBox( $"{city.reinforcementsIn.Length},{(city.tsDefMax.Max(city.tsHome) + 500) / 1000 }k", c1, tipTextFormatCentered, Color.Cyan, notFaded);

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
							var c = city.cid.CidToCp(TileData.zLabels);
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
									var c1 = sen.target.CidToCp(TileData.zLabels);

									var dist = city.cid.DistanceToCid(sen.target) * cartTravel; // todo: ship travel?
									var t = (tick * city.cid.CidToRandom().Lerp(1.5f / 512.0f, 1.75f / 512f)) + 0.25f;
									var r = t.Ramp();
									// Todo: more accurate senator travel times
									DrawAction( (float)(sen.time - serverNow).TotalSeconds, dist * 60.0f, r, c, c1, senatorColor,
										troopImages[ttSenator], false, null, 20);
								}
							}
							DrawTextBox( $"{recruiting}`{idle}`{active}", c, tipTextFormatCentered, Color.White, notFaded);

						}
							{
								DrawFlag(city.cid, SpriteAnim.flagHome);
							}
							if (MainPage.IsVisible())
						{
							var c = city.cid.CidToCp(TileData.zLabels);
							if (IsCulled(c, raidCullSlopSpace))
								continue;
							var t = (tick * city.cid.CidToRandom().Lerp(1.375f / 512.0f, 1.75f / 512f));
							var r = t.Ramp();
							//ds.DrawRoundedSquareWithShadow(c,r, raidBrush);
							foreach (var raid in city.raids)
							{
								var ct = raid.target.CidToCp(TileData.zLabels);
								(var c0, var c1) = !raid.isReturning ? (c, ct) : (ct, c);
								DrawAction( (float)(raid.time - serverNow).TotalSeconds,
									raid.GetOneWayTripTimeMinutes(city) * 60.0f,
									r, c0, c1, raidColor, troopImages[raid.troopType], false, null, 20);

							}
						}
					}
				}

				foreach (var cid in Spot.selected)
				{
						DrawFlag(cid, SpriteAnim.flagSelected);                
				}
				foreach (var cid in SettingsPage.pinned)
				{
					DrawAccent(cid, 1.0625f, pinnedColor);
				}
				if (Spot.focus != 0)
				{
					var cid = Spot.focus;
					DrawAccent(cid, -1.125f, focusColor);
				}
				if (Spot.viewHover != 0)
				{
					var cid = Spot.viewHover;
					DrawAccent(cid, 1.25f, hoverColor);
				}
				if (City.build != 0)
				{
					var cid = City.build;
					DrawAccent(cid, 0.875f, buildColor);
				}
				
			}


			if (wantLight)
			{
				// finished first pass
			
				//						ds.Dispose(); // end the frawing session
				
				
				var ddpx = (float)destP1.X - (float)destP0.X;
				var ddpy = (float)destP1.Y - (float)destP0.Y;
				var dspx = (float)srcP1.X - (float)srcP0.X;
				var dspy = (float)srcP1.Y - (float)srcP0.Y;
				var scaleX = ddpx / dspx;
				var scaleY = ddpy / dspy;
				//	if (webMask != null)
				//if (wantLight && lighteffect != null)
				//{
				//	lighteffect.Source1 = renderTarget;
				//	lighteffect.Properties["cameraPosition"] = new Vector3(halfSpan, cameraZ);
				//	var light3 = new Vector3(cameraLightC, lightZ0);


				//	lighteffect.Properties["lightPosition"] = light3;
				//	{

				//		//							var transform = new Transform2DEffect()
				//		//							{
				//		//								TransformMatrix = new Matrix3x2(scaleX, 0, 0, scaleY,
				//		//(float)destP0.X - ((float)srcP0.X + 0.5f) * scaleX,
				//		//(float)destP0.Y - ((float)srcP0.Y + 0.5f) * scaleY),
				//		//								Source = worldOwners,
				//		//								InterpolationMode = CanvasImageInterpolation.NearestNeighbor
				//		//					};

				//		var r = new Rect(destP0, destP1);
				//		//using (var crop = new CropEffect() { Source = transform, SourceRectangle = r })
				//		{
				//			//using (var blend = new ArithmeticCompositeEffect() { Source1 = light2, Source2 = crop, Source1Amount = 0.0f, MultiplyAmount = 1.25f })
				//			{
				//				//						var blend = new BlendEffect() { Foreground = emboss, Background = crop, Mode=BlendEffectMode.Multiply };

				//				//					var	TransformMatrix2 = new Matrix3x2(4f, 0, 0, 4f, -cachedXOffset , -cachedTopOffset );
				//				//					var transform2 = new Transform2DEffect() { TransformMatrix = TransformMatrix2, Source = webMask };

				//				//							var dis = new AlphaMaskEffect() { Source = blend, AlphaMask = transform2 };
				//				//							var dis = new CompositeEffect() { Sources = { transform2,blend }, Mode=blendMod   };
				//				ds.DrawImage(lighteffect, r, r);
				//			}
				//		}


				//	}

				//}



			}
			{
				if (wantDetails)
				{
					//
					// Text names
				//	using (var batch = ds.CreateSpriteBatch(CanvasSpriteSortMode.Bitmap))
					{
						// Labels last
						for (var cy = cy0; cy < cy1; ++cy)
						{
							for (var cx = cx0; cx < cx1; ++cx)
							{
								(var name, var isMine, var hasIncoming, var hovered, var spot) = World.GetLabel((cx, cy));
								var zScale = CanvasHelpers.ParalaxScale(TileData.zCities);

								if (name != null)
								{
									var layout = GetTextLayout( name, nameTextFormat);
									var span= pixelScale*zScale;
									var drawC = (new Vector2(cx, cy).WToCp(TileData.zCities));
										drawC.Y += span * 8.75f / 16.0f;

									layout.Draw(drawC,
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
								if (spot != null && spot.isClassified)
								{
									var c1 = (cx, cy).WToCp(TileData.zLabels);
									var t = (tick * spot.cid.CidToRandom().Lerp(1.5f / 512.0f, 1.75f / 512f)) + 0.25f;
									var r = t.Ramp();
									var alpha = (t * 1.21f).Wave() * 0.75f + 0.25f;
									var spriteSize = new Vector2( 16 * zScale );

									draw.AddQuad(Layer.effects, troopImages[spot.classificationTT], c1 - spriteSize, c1+ spriteSize, HSLToRGB.ToRGBA(rectSpan, 0.3f, 0.825f, alpha, alpha + 0.125f));
								}
							}
						}
					}
				}
			}
			// show selected
			var _toolTip = ShellPage.toolTip;

			if (underMouse != null)
			{
				//         Spot.viewHover = 0; // clear
				_toolTip = underMouse.GetToopTip(serverNow);
			}
			if (_toolTip != null)
			{
				TextLayout textLayout = GetTextLayout( _toolTip, tipTextFormat);
				var bounds = textLayout.span;
				Vector2 c = ShellPage.mousePosition + new Vector2(16, 16);
				var expand = new Vector2(7);
				
				//  var rectD = new Vector2(32*4, 24*5);
				// var target = new Rect((mousePosition + rectD*0.25f).ToPoint(), rectD.ToSize());
				//tipTextBrush.StartPoint = tipBackgroundBrush.StartPoint = new Vector2((float)bounds.Left, (float)bounds.Top);
				//tipTextBrush.EndPoint = tipBackgroundBrush.EndPoint = new Vector2((float)bounds.Right, (float)bounds.Bottom);
				FillRoundedRectangle(Layer.overlay,c- expand,c+bounds+expand,new Color(255,255,255,255));
					//                    target.X+= 12;
					//                  target.Y += 8;

				textLayout.Draw( c, Color.Black);//.Dra ds.DrawText(_toolTip, c, tipTextBrush, tipTextFormat);
			}
			var _contTip = ShellPage.contToolTip;
			if (_contTip != null)
			{
				TextLayout textLayout = GetTextLayout( _contTip, tipTextFormat);
					var bounds = textLayout.span;
					Vector2 c = new Vector2(16, 16);
					var expand = new Vector2(7);

					//  var rectD = new Vector2(32*4, 24*5);
					// var target = new Rect((mousePosition + rectD*0.25f).ToPoint(), rectD.ToSize());
					//tipTextBrush.StartPoint = tipBackgroundBrush.StartPoint = new Vector2((float)bounds.Left, (float)bounds.Top);
					//tipTextBrush.EndPoint = tipBackgroundBrush.EndPoint = new Vector2((float)bounds.Right, (float)bounds.Bottom);
					FillRoundedRectangle(Layer.overlay, c - expand, c + bounds + expand, new Color(255, 255, 255, 255));
					//                    target.X+= 12;
					//                  target.Y += 8;

					textLayout.Draw(c, Color.Black);//.Dra ds.DrawText(_toolTip, c, tipTextBrush, tipTextFormat);
													//	ds.DrawTextLayout(textLayout, c, tipTextBrush);//.Dra ds.DrawText(_toolTip, c, tipTextBrush, tipTextFormat);
				}

				draw.End();

			}
		catch (Exception ex)
		{
			Log(ex);
				draw._beginCalled = false;
			}
			


		}

		private static void DrawFlag(int cid, SpriteAnim sprite)
		{
			var c = cid.CidToCp(TileData.zLabels);
			var dv = WToCpSpan(TileData.zLabels);

			float frameCount = sprite.frameCount;
			var frame = (int)(((animationT + cid.CidToRandom() * 15) * 15) % frameCount);

			draw.AddQuad(Layer.effects,sprite.material, new Vector2(c.X, c.Y - dv * 0.475f), new Vector2(c.X + dv * 0.5f, c.Y - dv*0.1f), new Vector2(frame / frameCount, 0.0f), new Vector2((frame + 1) / frameCount, 1), 255.AlphaToColor());
		}

		private static void FillRoundedRectangle(int layer,Vector2 c0, Vector2 c1,Color background)
		{
			//	throw new NotImplementedException();
			draw.AddQuad(layer,roundedRect, c0, c1, background);
		}

		private static Color GetAttackColor(Army attack)
	{
		return attack.type switch
		{
			reportAssault => new Color(255 / 2, 0x7e / 2, 0x3e / 2, 0xd4 / 2),
			reportSiege => new Color(255 / 2, 0xcf / 2, 0x50 / 2, 0x07 / 2),
			reportSieging => new Color(192, 0xc5 / 2, 0x7f / 2, 0x4a / 2),
			reportPlunder => new Color(255 / 2, 0x28 / 2, 0x86 / 2, 0xc0 / 2),
			reportScout => new Color(255 / 2, 0xc8 / 2, 0x2d / 2, 0xbf / 2),

			_ => defaultAttackColor
		};
	}

	private static void DrawTextBox( string text, Vector2 at, TextFormat format, Color color, bool drawBackground)
	{
		float xLoc = at.X;
		float yLoc = at.Y;
			
		TextLayout textLayout = GetTextLayout( text, format);
		var bounds = textLayout.span;
		var expand = new Vector2(4);
			if (drawBackground)
			{
				var c0 = at;
				if (format.horizontalAlignment == TextFormat.HorizontalAlignment.center)
					c0.X -= textLayout.span.X * 0.5f;
				if (format.horizontalAlignment == TextFormat.HorizontalAlignment.right)
					c0.X -= textLayout.span.X ;
				if (format.verticalAlignment == TextFormat.VerticalAlignment.center)
					c0.Y -= textLayout.span.Y * 0.5f;
				if (format.verticalAlignment == TextFormat.VerticalAlignment.bottom)
					c0.Y -= textLayout.span.Y;
				FillRoundedRectangle(Layer.overlay, c0 - expand, c0 + textLayout.span + bounds, 255.AlphaToColor());
			}
		textLayout.Draw( at, color);
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

		const float actionStopDistance = 48.0f;
		private void DrawAction(float timeToArrival, float journeyTime, float rectSpan, Vector2 c0, Vector2 c1, Color color,
		Material bitmap, bool applyStopDistance, Army army, float spriteSize, float alpha = 1)
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
				progress = progress.Min(1.0f - shapeSizeGain * actionStopDistance / Vector2.Distance(c0, c1));
				//      var dc01 = c0 - c1;
				//       c1 += dc01 *( actionStopDistance / dc01.Length());
			}

			var gain = 1.0f;
			if (timeToArrival < postAttackDisplayTime)
				gain = 1.0f + (1.0f - timeToArrival / postAttackDisplayTime) * 0.25f;
			var mid = progress.Lerp(c0, c1);
			var shadowColor = color.GetShadowColor();
			var midS = mid - shadowOffset;
			if (army != null)
			{
				var d2 = Vector2.DistanceSquared(mid, ShellPage.mousePosition);
				if (d2 < bestUnderMouseScore)
				{
					bestUnderMouseScore = d2;
					underMouse = army;
				}
			}
			DrawLine(Layer.action, c0, c1, shadowColor);
			if (applyStopDistance)
				DrawSquare(Layer.action+1, c0, shadowColor);
			DrawLine(Layer.action+2, c0 - shadowOffset, midS, color);
			if (applyStopDistance)
				DrawSquare(Layer.action+3,new Vector2(c0.X - shadowOffset.X , c0.Y - shadowOffset.Y), color);
			var dc = new Vector2(spriteSize, spriteSize);
			if (bitmap != null)
				draw.AddQuad(Layer.action+4,  bitmap, new Vector2(mid.X - spriteSize, mid.Y - spriteSize), new Vector2(mid.X + spriteSize, mid.Y + spriteSize), HSLToRGB.ToRGBA(rectSpan, 0.3f, 0.825f, alpha, gain * 1.1875f));
			//            ds.DrawRoundedSquare(midS, rectSpan, color, 2.0f);
		
	
		}

		private static void DrawSquare(int layer,Vector2 c0, Color color)
		{
			draw.AddQuad(layer,quadTexture, new Vector2(c0.X - smallRectSpan, c0.Y - smallRectSpan), new Vector2(c0.X + smallRectSpan, c0.Y + smallRectSpan), new Vector2(0, 0), new Vector2(1,1), color);
		}
		private static void DrawRect(int layer,Vector2 c0,Vector2 c1, Color color)
		{
			draw.AddQuad(layer,quadTexture,c0,c1, new Vector2(), new Vector2(1,1), color);
		}
		private static void DrawLine(int layer,Vector2 c0, Vector2 c1, Color color)
		{
			draw.AddLine(layer,lineDraw, c0, c1, lineThickness, animationTWrap, animationTWrap + (c0 - c1).Length() * lineTileGain, color);
		}

		private static bool IsWorldView()
	{
		return JSClient.IsWorldView();
	}

	private static bool IsCityView()
	{
		return JSClient.IsCityView();
	}


		public static void DrawAccentBaseI(float cX, float cY, float radius, float angle, Color color)
		{
			var dx0 = radius * MathF.Cos(angle);
			var dy0 = radius * MathF.Sin(angle);
			var angle1 = angle + MathF.PI * 0.1875f;
			var dx1 = radius * MathF.Cos(angle1);
			var dy1 = radius * MathF.Sin(angle1);
			DrawLine(Layer.overlay-1,new Vector2(cX + dx0, cY + dy0),new Vector2( cX + dx1, cY + dy1), color);
			// rotated by 180
			DrawLine(Layer.overlay - 1, new Vector2( cX - dx0, cY - dy0),new Vector2( cX - dx1, cY - dy1), color);
		}
		public static void DrawAccentBase(float cX, float cY, float radius, float angle, Color color)
		{
			DrawAccentBaseI(cX, cY, radius, angle, color);
			DrawAccentBaseI(cX, cY, radius * 0.875f, angle + angle.SignOr0() * 0.125f, color);
			//DrawAccentBaseI(ds, cX, cY, radius*0.655f, angle+angle.SignOr0()*0.25f, color);
		}

		public static void DrawAccent(Vector2 c, float radius, float angularSpeed, Color brush)
		{
			var angle = angularSpeed * AGame.animationT;
			DrawAccentBase(c.X + AGame.halfShadowOffset.X, c.Y + AGame.halfShadowOffset.Y, radius, angle, brush.GetShadowColorDark());
			DrawAccentBase(c.X - AGame.halfShadowOffset.X, c.Y - AGame.halfShadowOffset.Y, radius, angle, brush);
		}
		public static void DrawAccent(int cid, float angularSpeedBase, Color brush)
		{
			var c = cid.CidToCp(TileData.zLabels);
			var rnd = cid.CidToRandom();

			var angularSpeed = angularSpeedBase + rnd * 0.5f;
			var t = (AGame.animationT * rnd.Lerp(1.25f / 256.0f, 1.75f / 256f));
			var r = t.Wave().Lerp(AGame.circleRadiusBase, AGame.circleRadiusBase * 1.375f);
			DrawAccent(c, r, angularSpeed, brush);
		}
	}

	
	public struct TextFormat
	{
		public enum VerticalAlignment
		{
			top,
			center, 
			bottom,
		}
		public enum HorizontalAlignment
		{
			left,
			center,
			right,
		}
		public HorizontalAlignment horizontalAlignment;
		public VerticalAlignment verticalAlignment;
		public Vector2 scale;

		public TextFormat(HorizontalAlignment _horizontalAlignment = HorizontalAlignment.left, VerticalAlignment _verticalAlignment = VerticalAlignment.top)
		{
			horizontalAlignment = _horizontalAlignment;
			verticalAlignment = _verticalAlignment;
			scale = new Vector2(1, 1);

		}
	}

	public static class CanvasHelpers
{
	public static Color AlphaToColor(this int alpha) { return new Color(255, 255, 255, alpha); }

	public static Point2 ToPoint(this Vector2 me) => new Point2(me.X,me.Y);
	public static Vector2 ToV2(this Point me) => new Vector2(me.X, me.Y);
	public static Vector2 ToV2(this Microsoft.Xna.Framework.Vector2 me) => new Vector2(me.X, me.Y);

		//public static Vector4 ToFVector4(this Color c) => new Vector4(c.R / 255, c.G / 255, c.B / 255, c.A / 255);
		//public static void DrawLine(this CanvasSpriteBatch b, Vector2 c0, Vector2 c1, Vector4 c, float thickness, CanvasStrokeStyle style)
		//{
		//	var dl = c1 - c0;
		//	var dllg = dl.Length();
		//	var dllgInv = 1 / dllg;
		//	var de = new Vector2(dl.Y * dllgInv * thickness, dl.X * dllgInv * thickness);
		//	float xoffset = ShellPage.animationT * 4 % 256;
		//	b.DrawFromSpriteSheet(ShellPage.lineDraw, new Matrix3x2(dl.X, dl.Y, de.X, de.Y, c0.X - de.X * 0.5f, c0.X - de.X * 0.5f), new Rect(xoffset, 0, dllg * 16 + xoffset, 128), c);
		//}

		//public static void DrawRoundedSquare(this CanvasDrawingSession ds, Vector2 c, float circleRadius, Color color, float thickness = 1.5f)
		//{
		//    ds.DrawRoundedRectangle(c.X - circleRadius, c.Y - circleRadius, circleRadius*2, circleRadius*2, circleRadius*0.25f, circleRadius*0.25f, color, thickness);
		//}


		//public static void DrawRoundedSquareWithShadow(this CanvasDrawingSession ds, Vector2 c, float circleRadius, Color brush, float thickness = 1.5f)
		//      {
		//          DrawRoundedSquareShadow(ds, c, circleRadius, brush.GetShadowColor(), thickness);
		//          DrawRoundedSquareBase(ds, c, circleRadius, brush, thickness);
		//      }
		//public static void DrawRoundedSquareShadow(this CanvasDrawingSession ds, Vector2 c, float circleRadius, Color color, float thickness = 1.5f)
		//      {
		//          DrawRoundedSquare(ds, c, circleRadius, color, thickness);
		//      }
		//      public static void DrawRoundedSquareBase(this CanvasDrawingSession ds, Vector2 c, float circleRadius, Color brush, float thickness = 1.5f)
		//      {
		//          DrawRoundedSquare(ds, c - ShellPage.shadowOffset, circleRadius, brush, thickness);
		//      }
		public static float parralaxZ0 = 1024;
	public static float ParalaxScale(float dz) => SettingsPage.wantParallax? parralaxZ0 / (parralaxZ0 - dz) : 1.0f;
		// for now we just the same bias for shadows as for light, assuming that the camera is as far from the world as the light
		public static float ParalaxScaleShadow(float dz) => 1;// parralaxZ0 / (parralaxZ0 - dz);

	public static Vector2 WToCp(this Vector2 c, float dz)
	{
		var paralaxGain = ParalaxScale(dz);
		
		return (c- AGame.cameraCLag)* paralaxGain*AGame.pixelScale + AGame.halfSpan;
	}
	public static Vector2 WToCpSpan(this Vector2 c, float dz)
	{
		var paralaxGain = ParalaxScale(dz);
		
		return (c)* paralaxGain*AGame.pixelScale ;
	}
		public static float WToCpSpan( float dz)
		{
			var paralaxGain = ParalaxScale(dz);

			return paralaxGain * AGame.pixelScale;
		}
		//public static Vector2 WToCShadow(this Vector2 c, float dz)
		//{
		//	var paralaxGain = ParalaxScaleShadow(dz);
		//	return (c - AGame.worldLightC) * paralaxGain * AGame.pixelScale + AGame.cameraLightC;
		//}
		public static Vector2 CToCShadow(this Vector2 c, float dz)
	{
		//var paralaxGain = ParalaxScaleShadow(dz);
		return c;//(c - AGame.cameraLightC) * paralaxGain + AGame.cameraLightC;
	}
	// camera space to camera space with parallax
	public static Vector2 CToCp(this Vector2 c, float dz)
	{
		var paralaxGain = ParalaxScale(dz);
			return (c - AGame.halfSpan) * paralaxGain + AGame.halfSpan;
	}
	//public static Point CToCp(this Vector2 c, float dz)
	//{
	//	var paralaxGain = ParalaxScale(dz);
	//	return ((c - AGame.halfSpan) * paralaxGain + AGame.halfSpan).ToPoint();

	//}

	public static Vector2 WToC(this Vector2 c)
	      {
	          return (c - AGame.cameraCLag) * AGame.pixelScale + AGame.halfSpan;
	      }
	public static Vector2 WToC(this (int x, int y) c)
	{
	    return new Vector2(c.x, c.y).WToC();
	}
	public static Vector2 WToCp(this (int x, int y) c, float z)
	{
		return new Vector2(c.x, c.y).WToCp(z);
	}
	//public static Vector2 CidToCC(this int c)
	//      {
	//          return c.ToWorldC().WToC();
	//      }
	public static Vector2 CidToCp(this int c, float z)
	{
		return c.ToWorldC().WToCp(z);
	}

	public static bool BringCidIntoWorldView(this int cid, bool lazy)
	{
		var v = cid.CidToWorldV();
		var newC = v;
		var dc = newC - AGame.cameraC;
		// only move if needed
		if (!lazy ||
			dc.X.Abs() * AGame.pixelScale > AGame.halfSpan.X * 0.875f ||
			dc.Y.Abs() * AGame.pixelScale > AGame.halfSpan.Y * 0.875f)
		{
			if (Vector2.DistanceSquared(AGame.cameraC, newC) >= 0.875f)
			{
				AGame.cameraC = newC;
				ShellPage.SetJSCamera();
				return true;
			}

		}
		return false;
	}
	public static Color GetShadowColor(this Color c)
	{
		return new Color( (byte)(c.R * 2 / 4), (byte)(c.G * 2 / 4), (byte)(c.B * 2 / 4), (byte)192);
		//            (0.625f).Lerp(c, new Color(128, 0, 0, 0));
		//            (0.625f).Lerp(c, new Color(128, 0, 0, 0));
	}
	public static Color GetShadowColorDark(this Color c)
	{
		return new Color( (byte)(c.R * 1 / 4), (byte)(c.G * 1 / 4), (byte)(c.B * 1 / 4), (byte)192);
		//            (0.625f).Lerp(c, new Color(128, 0, 0, 0));
		//            (0.625f).Lerp(c, new Color(128, 0, 0, 0));
	}
		public static (Vector2 c0, Vector2 c1) GetBounds(this Rectangle textureRegion)
		{
			return (new Vector2(textureRegion.X, textureRegion.Y), new Vector2(textureRegion.Right, textureRegion.Bottom));
		}
	}
	public class TextLayout
	{
		public TextFormat format;
		internal Vector2 span;
		private string text;

		public TextLayout(string text, TextFormat format)
		{
			this.text = text;
			this.format = format;
			var size = AGame.font.MeasureString(text);
			span.X = size.X;// *0.5f;
			span.Y = size.Y;// *0.5f;
		}

		internal void Draw(Vector2 c,Color color)
		{
			if (format.horizontalAlignment == TextFormat.HorizontalAlignment.center)
				c.X -= span.X * 0.5f;
			else if (format.horizontalAlignment == TextFormat.HorizontalAlignment.right)
				c.X -= span.X;
			else if (format.verticalAlignment == TextFormat.VerticalAlignment.center)
				c.Y -= span.Y * 0.5f;
			else if (format.verticalAlignment == TextFormat.VerticalAlignment.bottom)
				c.Y -= span.Y;


			AGame.draw.DrawString(AGame.font, text, c, color);
		}
	}
}

		
	



