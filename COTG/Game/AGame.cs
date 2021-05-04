
using COTG.Draw;
using COTG.Game;
using COTG.Helpers;
using COTG.JSON;
using COTG.Views;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


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
using Vector3 = System.Numerics.Vector3;

using XVector2 = Microsoft.Xna.Framework.Vector2;
using XVector3 = Microsoft.Xna.Framework.Vector3;
using XVector4 = Microsoft.Xna.Framework.Vector4;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Layer = COTG.Draw.Layer;
using BitmapFont;
using System.Threading.Tasks;

namespace COTG
{
	using KeyF = KeyFrame<float>;
	using Vector4 = System.Numerics.Vector4;

	public static partial class Helper
	{
		public static bool IsKeyPressed(this Keys key) => AGame.keyboardState.IsKeyDown(key);

		public static bool WasKeyPressed(this Keys key) => AGame.keyboardState.IsKeyDown(key) && !AGame.priorKeyboardState.IsKeyDown(key);

		static public CoreWindow CoreWindow => Window.Current.CoreWindow;
		static public UIElement CoreContent => Window.Current.Content;
		public static bool TryGetValue<T>(this List<T> l, Func<T, bool> pred, out T var)
		{
			foreach (var i in l)
			{
				if (pred(i))
				{
					var = i;
					return true;
				}
			}
			var = default;
			return false;
		}

		public static Material LoadLitMaterial(string name)
		{
			Texture texture;
			Texture normalMap;
			using (var scope = new AGame.SRGBLoadScope())
			{
				texture = AGame.instance.Content.Load<Texture2D>($"{name}");
			}
			using (var scope = new AGame.LinearRGBLoadScope())
			{
				normalMap = AGame.instance.Content.Load<Texture2D>($"{ name}_n");
			}
			return new Draw.Material(texture, normalMap, AGame.GetTileEffect());
		}
	}
	public enum Lighting
	{
		day,
		night,
		none,
	}
	public class AGame : Microsoft.Xna.Framework.Game
	{
		const float parallaxBaseGain = 2.0f / 1024.0f;
		public const float zBase = 0;
		public const float zLand = 0;
		public const float zEffectShadowBase = 10 * parallaxBaseGain;
		public const float zWaterBase = 10 * parallaxBaseGain;
		public const float zTerrainBase = 18 * parallaxBaseGain;
		public const float zTopLevelBase = 24 * parallaxBaseGain;
		public const float zCitiesBase = 28 * parallaxBaseGain;
		public const float zLabelsBase = 28 * parallaxBaseGain;
		public static float zWater => zWaterBase * AGame.parallaxGain;
		public static float zTerrain => zTerrainBase * AGame.parallaxGain;
		public static float zTopLevel => zTopLevelBase * AGame.parallaxGain;
		public static float zCities => zCitiesBase * AGame.parallaxGain;
		public static float zLabels => zLabelsBase * AGame.parallaxGain;
		public static float zEffectShadow => zEffectShadowBase * 0.5f;

		static Vector2 shadowOffset = new Vector2(4, 4);
		public const float cameraZ = 1.0f;
		public static SwapChainPanel canvas;
		public static AGame instance;
		public static GraphicsDevice device => instance.GraphicsDevice;
		public static GraphicsDeviceManager _graphics;
		const float detailsZoomThreshold = 36;
		const float detailsZoomFade = 8;
		public static Material worldBackground;
		//public static Effect imageEffect;
		public static Effect avaEffect;
		public static Span2i[] popups = Array.Empty<Span2i>();

		public static EffectPass defaultEffect;
		public static EffectPass alphaAddEffect;
		public static EffectPass fontEffect;
		public static EffectPass darkFontEffect;
		public static EffectPass litEffect;
		public static EffectPass unlitEffect;
		public static EffectPass animatedSpriteEffect;
		private static EffectPass sdfEffect;
		private static EffectPass noTextureEffect;
		private static EffectPass worldSpaceEffect;
		public static EffectParameter planetGainsParamater;
		public static EffectParameter worldMatrixParameter;
		public static EffectParameter lightPositionParameter;
		public static EffectParameter lightGainsParameter;
		public static EffectParameter lightAmbientParameter;
		public static EffectParameter lightColorParameter;
		public static EffectParameter lightSpecularParameter;
		public static EffectParameter cameraReferencePositionParameter;
		public static EffectParameter cameraCParameter;
		public static EffectParameter pixelScaleParameter;
		public static Material lineDraw;
		public static Material quadTexture;
		public static Material whiteMaterial;
		public static Material sky;
		public static Material webMask;
		//    public static TintEffect worldBackgroundDark;
		public static Material worldObjects;
		public static Material worldOwners;

		public static VertexBuffer tesselatedWorldVB;
		public static IndexBuffer tesselatedWorldIB;
		public static Mesh tesselatedWorld;
		public static EffectPass GetTileEffect() => (SettingsPage.lighting != Lighting.none) ? litEffect : unlitEffect;

		internal static void SetLighting(Lighting value)
		{
			foreach (var tile in TileData.instance.tilesets)
			{
				tile.material.effect = GetTileEffect();
			}
			CityView.UpdateLighting(value);
		}

		//     public static TintEffect worldObjectsDark;
		public static Material worldChanges;
		//	public static Vector2 clientTL;
		public static Vector2 cameraC = new Vector2(300, 300);
		public static Vector2 cameraCLag = cameraC; // for smoothing
													//		public static Vector2 clientC;
													//		public static Vector2 clientCScreen;
		public static Vector2 clientSpan;
		public static Vector2 halfSpan;
		static Army underMouse;
		static float bestUnderMouseScore;
		//   public static Vector2 cameraMid;
		public static float cameraZoom = cityZoomDefault;
		public static float cameraZoomLag = cityZoomFocusThreshold;
		public const float maxZoom = 4096;
		public const float cameraZoomWorldDefault = 16;
		public const float cameraZoomRegionDefault = 64;
		public const float cityZoomThreshold = 280f;
		public const float cityZoomWorldThreshold = 32;
		public const float cityZoomFocusThreshold = 512;
		public const float cityZoomDefault = 1280;
		public float eventTimeOffsetLag;
		public float eventTimeEnd;
		static public Color nameColor, nameColorHover, myNameColor, nameColorIncoming, nameColorSieged, nameColorIncomingHover, nameColorSiegedHover, myNameColorIncoming, myNameColorSieged; //shadowColor;
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
		public static float bmFontScale = 0.125f;
		public static Texture2D fontTexture;
		static readonly Color attackColor = Color.White;
		static Color ShadowColor(float alpha) => new Color(0, 0, 32, (int)(200*alpha) );
		static readonly Color defenseColor = new Color(255, 20, 160, 160);
		static readonly Color defenseArrivedColor = new Color(255, 20, 255, 160);
		static readonly Color artColor = Color.DarkOrange;
		static readonly Color senatorColor = Color.OrangeRed;
		static readonly Color tradeColor = Color.DarkGreen;
		static readonly Color tradeColorHover = Color.DarkBlue;
		static readonly Color defaultAttackColor = Color.Maroon;// (0xFF8B008B);// Color.DarkMagenta;
		static readonly Color raidColor = Color.Yellow;
		//        static readonly Color shadowColor = new Color(128, 0, 0, 0);
		static readonly Color selectColor = new Color(255, 20, 255, 192);
		static readonly Color buildColor = Color.DarkRed;
		static readonly Color hoverColor = Color.Purple;
		static readonly Color focusColor = Color.Maroon;
		static readonly Color pinnedColor = Color.Teal;
		static readonly Color black0Alpha = new Color() { A = 0, R = 0, G = 0, B = 0 };
		public static Material[] troopImages = new Material[Game.Enum.ttCount];
		static Vector2 troopImageOriginOffset;
		const int maxTextLayouts = 1024;
		public static bool initialized => canvas != null;



		static Dictionary<int, TextLayout> nameLayoutCache = new Dictionary<int, TextLayout>();
		static public TextLayout GetTextLayout(string name, TextFormat format)
		{
			var hash = name.GetHashCode(StringComparison.Ordinal);
			if (nameLayoutCache.TryGetValue(name.GetHashCode(StringComparison.Ordinal), out var rv))
				return rv;
			rv = new TextLayout(name, format);

			if (nameLayoutCache.Count >= maxTextLayouts)
				nameLayoutCache.Remove(nameLayoutCache.First().Key);
			nameLayoutCache.Add(hash, rv);

			return rv;

		}
		public AGame()
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferFormat = SurfaceFormat.Bgra32,
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
			canvas.CompositeMode = (UWindows.UI.Xaml.Media.ElementCompositeMode.MinBlend);
			instance = MonoGame.Framework.XamlGame<AGame>.Create(() => new AGame() { }, "", Helper.CoreWindow, swapChainPanel);
		}

		protected override void Initialize()
		{
			instance = this;
			base.Initialize();



		}



		public static Material CreateFromBytes(byte[] pixels, int x, int y, SurfaceFormat format, EffectPass effect = null)
		{
			if (effect == null)
				effect = AGame.defaultEffect;
			var rv = new Texture2D(instance.GraphicsDevice, x, y, false, format);
			rv.SetData(pixels);

			return new Material(rv, effect);
		}
		public static Material LoadMaterial(string filename)
		{
			var rv = instance.Content.Load<Texture2D>(filename);
			return new Material(rv);
		}
		public static Material LoadMaterialAdditive(string filename)
		{
			var rv = instance.Content.Load<Texture2D>(filename);
			return new Material(rv, alphaAddEffect);
		}
		public static Texture2D LoadTexture(string filename)
		{
			return instance.Content.Load<Texture2D>(filename);
		}
		//		public static MouseState mouseState;
		//		public static MouseState priorMouseState;
		public static KeyboardState keyboardState;
		public static KeyboardState priorKeyboardState;

		const float viewHoverZGain = 1.0f / 64.0f;
		const float viewHoverElevationKt = 24.0f;
		public static List<(int cid, float z, float vz)> viewHovers = new List<(int cid, float z, float vz)>();


		protected override void Update(GameTime gameTime)
		{

			try
			{
				float dt = ((float)gameTime.ElapsedGameTime.TotalSeconds).Min(1.0f / 16.0f);

				var hover = ShellPage.lastCanvasC;
				if (hover != 0 && World.GetInfoFromCid(hover).type != 0)
				{
					if (!viewHovers.Exists(a => a.cid == ShellPage.lastCanvasC))
					{
						viewHovers.Add((ShellPage.lastCanvasC, 1.0f / 32.0f, 0.0f));
					}
				}
				{
					var removeMe = 0;
					int count = viewHovers.Count;
					for (int i = 0; i < count; ++i)
					{
						var cid = viewHovers[i].cid;

						float z = viewHovers[i].z;
						float vz = viewHovers[i].vz;
						var kd = (viewHoverElevationKt);
						var ks = AMath.CritDampingKs(kd);

						vz += (((cid == hover ? 1.0f : 0.0f) - z) * ks - vz * kd) * dt;
						z += vz * (float)dt;
						viewHovers[i] = (cid, z, vz);


						if (z <= 1.0f / 32.0f)
						{
							removeMe = i;
						}
					}
					// Hack:  Just remove one per frame, we'll get the rest next time,
					if (removeMe != 0)
						viewHovers.RemoveAt(removeMe);

				}


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
								BackBufferFormat = SurfaceFormat.Bgra32,
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




				//	priorMouseState = mouseState;
				priorKeyboardState = keyboardState;
				keyboardState = Keyboard._nextKeyboardState;
				App.canvasKeyModifiers = UWindows.System.VirtualKeyModifiers.None;
				if ((keyboardState.IsKeyDown(Keys.LeftShift) | keyboardState.IsKeyDown(Keys.RightShift)))
					App.canvasKeyModifiers |= UWindows.System.VirtualKeyModifiers.Shift;
				if ((keyboardState.IsKeyDown(Keys.LeftControl) | keyboardState.IsKeyDown(Keys.RightControl)))
					App.canvasKeyModifiers |= UWindows.System.VirtualKeyModifiers.Control;

				////	ShellPage.CanvasCheckKeys();
				//	if ( keyboardState.GetPressedKeyCount() > priorKeyboardState.GetPressedKeyCount() )
				//	{
				//		App.NotIdle();
				//	}

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

				//mouseState = Mouse.GetState();
				//ShellPage.Canvas_PointerMoved(mouseState, priorMouseState);
				//ShellPage.Canvas_PointerWheelChanged(mouseState, priorMouseState);
				//ShellPage.Canvas_PointerPressed(mouseState, priorMouseState);

				if (App.isForeground)
				{
					if (ShellPage.coreInputSource == null)
					{
						ShellPage.SetupCoreInput();

					}
					else
					{
						ShellPage.coreInputSource.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);
						ShellPage.Gesture.Tick();
					}

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
						w.texture.Dispose();
					}
					worldObjects = CreateFromBytes(pixels, World.outSize, World.outSize, SurfaceFormat.Dxt1SRgb);
				}
				if(World.worldOwnerPixels!=null)
				{
					var ownerPixels = World.worldOwnerPixels;
					World.worldOwnerPixels = null;
					if (worldOwners != null)
					{
						var w = worldOwners;
						worldOwners = null;
						w.texture.Dispose();
					}
					worldOwners = CreateFromBytes(ownerPixels, World.outSize, World.outSize, SurfaceFormat.Dxt1SRgb);
					//canvas.Paused = falwirse;
					//if (worldObjectsDark != null)
					//    worldObjectsDark.Dispose();
					//worldObjectsDark = new TintEffect() { BufferPrecision = CanvasBufferPrecision.Precision8UIntNormalizedSrgb, Source = worldObjects, Color = new Color() { A = 255, R = 128, G = 128, B = 128 } };

				}
				if (World.changePixels != null)
				{
					var pixels = World.changePixels;
					ClearHeatmapImage();
					worldChanges = CreateFromBytes(pixels, World.outSize, World.outSize, SurfaceFormat.Dxt1, worldSpaceEffect);
					

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
			catch (SharpDX.SharpDXException sex)
			{
				COTG.Debug.LogEx(sex);
				COTG.Debug.Log($"{sex.ResultCode} {sex.Descriptor.ApiCode} {sex.Descriptor.Description} {sex.Descriptor.ToString()} ");
			}
			catch (Exception _exception)
			{
				COTG.Debug.LogEx(_exception);
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
			if (World.changeMapRequested)
			{
				World.changeMapInProgress = true;
				World.changeMapRequested = false;
				
			}
			else
			{
				World.changeMapInProgress = false;// this is used to temporarily block the UI from issuing multiple changes at once
			}		
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

		public static double dipToNative = 1;
		public static double nativeToDip = 1;
		public static void SetClientSpan(double dx, double dy)
		{
			dipToNative = UWindows.Graphics.Display.DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
			nativeToDip = 1.0 / dipToNative;
			//clientSpan.X = MathF.Round( (float)((dx* dipToNative+3) / 4))*4.0f;
			//clientSpan.Y = MathF.Round((float)((dy* dipToNative+3) /4))*4.0f;
			clientSpan.X = (float)(dx * dipToNative);
			clientSpan.Y = (float)(dy * dipToNative);
			halfSpan = clientSpan * 0.5f;
		}
		public static int resolutionDirtyCounter;

		//public static void Canvas_LayoutUpdated(object sender, object e)
		//{
		//	// not yet initialized?
		//	if (!initialized)
		//	{
		//		return;
		//	}
		//	var c = canvas.ActualOffset;

		//	//		clientC = new Vector2(c.X, c.Y);
		//	SetClientSpan(canvas.ActualSize );
		//	//	clientCScreen = canvas.TransformToVisual(Helper.CoreContent)
		//	//		.TransformPoint(new UWindows.Foundation.Point(0, 0)).ToVector2();
		//	resolutionDirtyCounter = 10;
		//}
		public static void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
		{

			if (!initialized)
			{
				return;
			}

			SetClientSpan(e.NewSize.Width, e.NewSize.Height);
			//clientCScreen = canvas.TransformToVisual(Helper.CoreContent)
			//	.TransformPoint(new UWindows.Foundation.Point(0, 0)).ToVector2();
			//	canvas.RunOnGameLoopThreadAsync(RemakeRenderTarget);

			//	Log(canvas.CompositionScaleX);


			resolutionDirtyCounter = 10;
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
		public static Song[] music;
		const int musicCount = 7;
		static int lastSongPlayed;
		public static void UpdateMusic()
		{
			if (SettingsPage.musicVolume > 0 && !JSClient.isSub)
			{
				if (music == null)
				{
					if (!readyToLoad)
						return;
					music = new Song[musicCount];
					for (int i = 0; i < musicCount; ++i)
						music[i] = instance.Content.Load<Song>($"Audio/music{i}");
				}

				MediaPlayer.Volume = SettingsPage.musicVolume;
				if (MediaPlayer.State == MediaState.Stopped)
				{
					MediaPlayer.Play(music[AMath.random.Next(musicCount)]);
				}
			}
			else
			{
				if (MediaPlayer.State != MediaState.Stopped)
					MediaPlayer.Stop();
			}
		}
		public class SRGBLoadScope : IDisposable
		{
			public SRGBLoadScope()
			{
				++Microsoft.Xna.Framework.Content.ContentManager.wantSRGB;
			}

			void IDisposable.Dispose()
			{
				--Microsoft.Xna.Framework.Content.ContentManager.wantSRGB;
			}
		}
		public class LinearRGBLoadScope : IDisposable
		{
			public LinearRGBLoadScope()
			{
				Microsoft.Xna.Framework.Content.ContentManager.wantSRGB -= 16;
			}

			void IDisposable.Dispose()
			{
				Microsoft.Xna.Framework.Content.ContentManager.wantSRGB += 16;
			}
		}

		public static void LoadWorldBackground()
		{
			worldBackground = LoadMaterial($"Art/world{(JSClient.world switch { 23 => "23", _ => "22" })}");

		}
		protected override async void LoadContent()
		{
			try
			{
				avaEffect = Content.Load<Effect>("Effects/Ava");
				defaultEffect = EffectPass("AlphaBlend");
				alphaAddEffect = EffectPass("AlphaAdd");
				fontEffect = EffectPass("FontLight");
				darkFontEffect = EffectPass("FontDark");
				litEffect = EffectPass("Lit");
				unlitEffect = EffectPass("Unlit");
				animatedSpriteEffect = EffectPass("SpriteAnim");
				sdfEffect = EffectPass("SDF");
				noTextureEffect = EffectPass("NoTexture");
				worldSpaceEffect = EffectPass("WorldSpace");

				readyToLoad = true;

				using var srgb = new SRGBLoadScope();

				worldMatrixParameter = avaEffect.Parameters["WorldViewProjection"];


				lightPositionParameter = avaEffect.Parameters["lightPosition"];
				planetGainsParamater = avaEffect.Parameters["planetGains"];
				lightGainsParameter = avaEffect.Parameters["lightGains"];
				lightColorParameter = avaEffect.Parameters["lightColor"];
				lightSpecularParameter = avaEffect.Parameters["lightSpecular"];
				lightAmbientParameter = avaEffect.Parameters["lightAmbient"];
				cameraReferencePositionParameter = avaEffect.Parameters["cameraReferencePosition"];
				cameraCParameter = avaEffect.Parameters["cameraC"];
				pixelScaleParameter = avaEffect.Parameters["pixelScale"];

				fontMaterial = new Material(null, fontEffect);
				fontTexture = Content.Load<Texture2D>("Fonts/tra_0"); // font is always set to register 7
				darkFontMaterial = new Material(fontMaterial.texture, darkFontEffect);


				fontMaterial.effect = fontEffect;

				bfont = new BitmapFont.BitmapFont();

				var a = new System.IO.StreamReader((typeof(AGame).Assembly).GetManifestResourceStream("COTG.Content.Fonts.tra.fnt")).ReadToEnd();
				//	using (System.IO.TextReader stream = new System.IO.StreamReader(a))
				{

					bfont.LoadXml(a);
				}

				tesselatedWorldVB = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration,( (World.span+1) * (World.span + 1) ),BufferUsage.None );
				{
					var input = new VertexPositionTexture[(World.span + 1) * (World.span + 1)];
					for (int x = 0; x < World.span + 1; ++x)
					{
						for (int y = 0; y < World.span + 1; ++y)
						{
							var i = new VertexPositionTexture();
							i.Position = new Vector3(x, y, 0.0f);
							i.TextureCoordinate.X = (float)(x+1) / (World.span + 1);
							i.TextureCoordinate.Y = (float)(y+1) / (World.span + 1);
							input[(World.span + 1) * y + x] = i;
						}
					}
					tesselatedWorldVB.SetData(input);
				}
				tesselatedWorldIB = new IndexBuffer(device,IndexElementSize.ThirtyTwoBits, 
					(World.span) * (World.span)*6, BufferUsage.None);
				{
					var input = new int[(World.span) * (World.span) * 6];
					for (int x = 0; x < World.span ; ++x)
					{
						for (int y = 0; y < World.span ; ++y)
						{
							input[(x + y * (World.span)) * 6 + 0] = (int)(x   + y * (World.span + 1));
							input[(x + y * (World.span)) * 6 + 1] = (int)(x+1 + y * (World.span + 1));
							input[(x + y * (World.span)) * 6 + 2] = (int)(x+1  + (y+1) * (World.span + 1));

							input[(x + y * (World.span)) * 6 + 3] = (int)(x + y * (World.span + 1));
							input[(x + y * (World.span)) * 6 + 4] = (int)(x + 1 + (y+1) * (World.span + 1));
							input[(x + y * (World.span)) * 6 + 5] = (int)(x  + (y + 1) * (World.span + 1));
						}
					}
					tesselatedWorldIB.SetData(input);
				}
				tesselatedWorld = new Mesh();
				tesselatedWorld.vb = tesselatedWorldVB;
				tesselatedWorld.ib = tesselatedWorldIB;
				tesselatedWorld.vertexCount = (World.span + 1) * (World.span + 1);
				tesselatedWorld.triangleCount = (World.span ) * (World.span )*2;



				SpriteAnim.flagHome.Load();
				SpriteAnim.flagSelected.Load();
				SpriteAnim.flagRed.Load();
				SpriteAnim.flagPinned.Load();
				SpriteAnim.flagGrey.Load();

				draw = new COTG.Draw.SpriteBatch(GraphicsDevice);
				// worldBackgroundDark = new TintEffect() { BufferPrecision = CanvasBufferPrecision.Precision8UIntNormalizedSrgb, Source = worldBackground, Color = new Color() { A = 255, R = 128, G = 128, B = 128 } };



				lineDraw = LoadMaterial("Art/linedraw2");
				lineDraw.effect = alphaAddEffect;
				//lineDraw2 = new PixelShaderEffect(
				sky = LoadMaterial("Art/sky");
				//				roundedRect = new Material(Content.Load<Texture2D>("Art/Icons/roundRect"),alphaAddEffect);
				//				quadTexture = new Material(Content.Load<Texture2D>("Art/quad"), sdfEffect);
				quadTexture = new Material(null, sdfEffect);
				whiteMaterial = new Material(null, noTextureEffect);
				for (int i = 0; i < COTG.Game.Enum.ttCount; ++i)
				{

					troopImages[i] = LoadMaterial($"Art/icons/troops{i}");
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

				CityView.LoadContent();
				UpdateMusic();
			}
			catch (Exception ex)
			{
				LogEx(ex);
			}
		}

		public static EffectPass EffectPass(string name)
		{
			return avaEffect.Techniques[name].Passes[0];
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
		const float lineThickness = 4.0f;
		const float rectSpanMin = 4.0f;
		const float rectSpanMax = 8.0f;
		const float bSizeGain = 4.0f;
		const float bSizeGain2 = 4;//4.22166666666667f;
		const float srcImageSpan = 2400;
		const float bSizeGain3 = bSizeGain * bSizeGain / bSizeGain2;
		public static float pixelScale = 1;
		public static float circleRadiusBase = 1.0f;
		public static float shapeSizeGain = 1.0f;
		public static float bulgeSpan => 1.0f + bulgeNegativeRange;
		public static float bulgeGain = 0;
		public static float pixelScaleInverse = 1;
		//	const float dashLength = (dashD0 + dashD1) * lineThickness;
		public static Draw.SpriteBatch draw;

		public static bool IsCulled(Vector2 c0, Vector2 c1)
		{
			var x1 = c0.X.Max(c1.X);
			var x0 = c0.X.Min(c1.X);

			var y1 = c0.Y.Max(c1.Y);
			var y0 = c0.Y.Min(c1.Y);
			// todo: cull on diagonals
			return x1 <= -halfSpan.X | x0 >= halfSpan.X |
					y1 <= -halfSpan.Y | y0 >= halfSpan.Y;
		}
		public static bool IsCulledWC((int x, int y) c)
		{
			return !cullWC.Contains(c);
		}
		public static bool IsCulledWC((int x, int y) c, int slopSpace)
		{
			return !cullWC.Overlaps(c, slopSpace);
		}
		public static bool IsCulled(Vector2 c0, float pad)
		{
			var x1 = c0.X;
			var x0 = c0.X;

			var y1 = c0.Y;
			var y0 = c0.Y;
			// todo: cull on diagonals
			return x1 + pad <= -halfSpan.X | x0 - pad >= halfSpan.X |
					y1 + pad <= -halfSpan.Y | y0 - pad >= halfSpan.Y;
		}
		public static bool IsCulled(Vector2 c0)
		{
			var x1 = c0.X;
			var x0 = c0.X;

			var y1 = c0.Y;
			var y0 = c0.Y;
			// todo: cull on diagonals
			return x1 <= -halfSpan.X | x0 >= halfSpan.X |
					y1 <= -halfSpan.Y | y0 >= halfSpan.Y;
		}




		//public static Vector2 shadowOffset = new Vector2(6.0f, 6.0f);
		//public static Vector2 halfShadowOffset = new Vector2(3.0f, 3.0f);
		public static void SetCameraCNoLag(Vector2 c) => cameraCLag = cameraC = c;
		static DateTimeOffset lastDrawTime;
		public static bool tileSetsPending;
		private const float smallRectSpan = 4;
		public const float lightZ0 = 460f;
		public const float lightZDay = 1500;
		public static Vector2 cameraLightC;
		static SamplerState fontFilter = new SamplerState()
		{
			Filter = TextureFilter.Linear,
			MipMapLevelOfDetailBias = -1.5f,
			BorderColor = new Color(0, 0, 0, 0),
			MaxAnisotropy = 2,
			AddressW = TextureAddressMode.Border,
			AddressU = TextureAddressMode.Border,
			AddressV = TextureAddressMode.Border,
		};
		static int filterCounter;

		public static float animationTWrap; // wraps every 3 seconds
		public static float animationT; // approximate animation time in seconds
		private TextFormat textformatLabel = new TextFormat(TextFormat.HorizontalAlignment.center, TextFormat.VerticalAlignment.center);
		private TextFormat tipTextFormatCentered = new TextFormat(TextFormat.HorizontalAlignment.center);
		private TextFormat tipTextFormat = new TextFormat(TextFormat.HorizontalAlignment.left);
		private TextFormat tipTextFormatRight = new TextFormat(TextFormat.HorizontalAlignment.right);
		private TextFormat nameTextFormat = new TextFormat(TextFormat.HorizontalAlignment.center, TextFormat.VerticalAlignment.center);
		internal static Material fontMaterial;
		internal static Material darkFontMaterial;
		public static BitmapFont.BitmapFont bfont;
		public static float parallaxGain;
		const float lineTileGain = 1.0f / 32.0f;

		const float actionAnimationGain = 64.0f;
		const float drawActionLength = 32;
		const float lineAnimationGain = 2.0f;
		public static Span2i cullWC; // culling bounds in world space
		protected override bool BeginDraw()
		{
			if (!App.isForeground)
				return false;
			if (!ShellPage.canvasVisible)
				return false;
			return base.BeginDraw();
		}
		static KeyF[] bulgeKeys = new[] { new KeyF(0.0f, 0.0f), new KeyF(0.44f, 0.44f), new KeyF(1.5f, 0.44f), new KeyF(2.5f, 0.0f) };

		private static bool TilesReady()
		{
			return (TileData.state >= TileData.State.ready);
		}

		private const int textBoxCullSlop = 80;

		//	static CanvasTextAntialiasing canvasTextAntialiasing = CanvasTextAntialiasing.Grayscale;
		//	static CanvasTextRenderingParameters canvasTextRenderingParameters = new CanvasTextRenderingParameters(CanvasTextRenderingMode.NaturalSymmetric, CanvasTextGridFit.Disable);
		protected override void Draw(GameTime gameTime)
		{
			underMouse = null;
			bestUnderMouseScore = 32 * 32;


			//parallaxZ0 = 1024 * 64.0f / cameraZoomLag;
			var isFocused = ShellPage.isHitTestVisible;

			try
			{
				var _serverNow = JSClient.ServerTime();
				var dt = ((float)(_serverNow - lastDrawTime).TotalSeconds).Saturate(); // max delta is 1s
				lastDrawTime = _serverNow;

				var gain = (1 - MathF.Exp(-4 * dt));
				cameraCLag += (cameraC - cameraCLag) * gain;
				cameraZoomLag += (cameraZoom - cameraZoomLag) * gain;
				eventTimeOffsetLag += (ShellPage.instance.eventTimeOffset - eventTimeOffsetLag) * gain;
				cameraLightC = (ShellPage.mousePositionW);
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

				device.Textures[7] = fontTexture;
				//				float accentAngle = animT * MathF.PI * 2;
				int tick = (Environment.TickCount >> 3) & 0xfffff;
				var animTLoop = animationTWrap.Wave();
				int cx0 = 0, cy0 = 0, cx1 = 0, cy1 = 0;
				var rectSpan = animTLoop.Lerp(rectSpanMin, rectSpanMax);


				//   ShellPage.T("Draw");

				byte textBackgroundOpacity = 192;
				//	defaultStrokeStyle.DashOffset = (1 - animT) * dashLength;


				//                ds.Blend = ( (int)(serverNow.Second / 15) switch { 0 => CanvasBlend.Add, 1 => CanvasBlend.Copy, 2 => CanvasBlend.Add, _ => CanvasBlend.SourceOver } );




				//ds.TextRenderingParameters = new CanvasTextRenderingParameters(!App.IsKeyPressedControl() ? CanvasTextRenderingMode.Outline : CanvasTextRenderingMode.Default, CanvasTextGridFit.Default);

				//              ds.TextRenderingParameters = new CanvasTextRenderingParameters(CanvasTextRenderingMode.Default, CanvasTextGridFit.Disable);
				// var scale = ShellPage.canvas.ConvertPixelsToDips(1);
				pixelScale = (cameraZoomLag);
				bmFontScale = MathF.Sqrt(pixelScale / 64.0f) * 0.5f * SettingsPage.fontScale;
				pixelScaleInverse = 1.0f / cameraZoomLag;
				shapeSizeGain = MathF.Sqrt(pixelScale * (1.50f / 64.0f));
				var deltaZoom = cameraZoomLag - detailsZoomThreshold;
				var wantDetails = deltaZoom > 0;
				var wantImage = deltaZoom < detailsZoomFade;
				var deltaZoomCity = cameraZoomLag - cityZoomThreshold;
				var wantCity = deltaZoomCity >= 0;
				var cityAlpha = (deltaZoomCity / 128.0f).Saturate().Sqrt(); // blend in over 128m.  We use the sqrt to shape it a little
																			// in rangeshould we taket the sqrt()?
				var cityAlphaI = (int)(cityAlpha * 255.0f);

				var wantFade = wantImage; // world to region fade
				var regionAlpha = wantFade ? (deltaZoom / detailsZoomFade).Saturate().Sqrt() : 1.0f;
				var intAlpha = (byte)(regionAlpha * 255.0f).RoundToInt();

				var zoomT = cameraZoomLag / detailsZoomThreshold;
				bulgeGain = (((MathF.Log(MathF.Max(1.0f, zoomT)))));
				bulgeGain = bulgeGain.Min(0.4f);// Eval(bulgeGain);


				bulgeGain *= SettingsPage.planet * (1.0f - cityAlpha);
				bulgeInputGain = 0.75f.Squared() / (AGame.halfSpan.X.Squared() + AGame.halfSpan.Y.Squared());
				// workd space coords
				var srcP0 = new Vector2((cameraCLag.X + 0.5f) * bSizeGain2 - halfSpan.X * bSizeGain2 * pixelScaleInverse,
									  (cameraCLag.Y + 0.5f) * bSizeGain2 - halfSpan.Y * bSizeGain2 * pixelScaleInverse);
				var srcP1 = new Vector2(srcP0.X + clientSpan.X * bSizeGain2 * pixelScaleInverse,
									   srcP0.Y + clientSpan.Y * bSizeGain2 * pixelScaleInverse);
				var destP0 = -halfSpan;
				var destP1 = halfSpan;

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
				var isWinter = SettingsPage.IsThemeWinter();
				var attacksVisible = DefenseHistoryTab.IsVisible() | OutgoingTab.IsVisible() | IncomingTab.IsVisible() | HitTab.IsVisible() | AttackTab.IsVisible();


				var wantParallax = SettingsPage.parallax > 0.1f;

				//var gr = spriteBatch;// spriteBatch;// wantLight ? renderTarget.CreateDrawingSession() : args.DrawingSession;

				//		ds.Blend = CanvasBlend.Copy;

				// funky logic
				//if (wantLight)
				GraphicsDevice.Clear(new Color()); // black transparent
												   //ds.TextAntialiasing = canvasTextAntialiasing;
												   //ds.TextRenderingParameters = canvasTextRenderingParameters;
												   // prevent MSAA gaps
				GraphicsDevice.BlendState = BlendState.AlphaBlend;
				//	GraphicsDevice.DepthStencilState = DepthStencilState.None;

				//			GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
				//			GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
				if (fontFilter != null)
					GraphicsDevice.SamplerStates[7] = fontFilter;
				GraphicsDevice.RasterizerState = RasterizerState.CullNone;
				//if (WasKeyPressed(Keys.F))
				//{
				//	++filterCounter;
				//	fontFilter = new SamplerState()
				//	{
				//		Filter = ((filterCounter & 1)==0) ? TextureFilter.Linear : TextureFilter.Anisotropic,

				//		MipMapLevelOfDetailBias =-((((filterCounter/2)%4))*0.5f),
				//		MaxMipLevel=8
				//		,BorderColor = new Color(255,0,0,255),
				//		MaxAnisotropy=2,

				//	};
				//	Log($"{fontFilter.Filter} {fontFilter.MipMapLevelOfDetailBias}");
				//}
				//if (fontEffect._pixelShader != null)
				//{
				//	for (int i = 0; i < fontEffect._pixelShader.Samplers.Length; ++i)
				//	{
				//		fontEffect._pixelShader.Samplers[i].state = fontFilter;
				//	}
				//}

				{
					var viewport = GraphicsDevice.Viewport;


					var proj = Matrix.CreateLookAt(new Microsoft.Xna.Framework.Vector3(0, 0, cameraZ), Microsoft.Xna.Framework.Vector3.Zero, Microsoft.Xna.Framework.Vector3.Up) * Matrix.CreatePerspective(viewport.Width * 0.5f, -viewport.Height * 0.5f, 0.5f, 1.5f);
					//					var proj = Matrix.CreateOrthographicOffCenter(480, 1680, 1680, 480, 0, -1);
					worldMatrixParameter.SetValue(proj);

					if (SettingsPage.lighting == Lighting.night)
					{
						var l = ShellPage.mousePosition;//.InverseProject();
						lightPositionParameter.SetValue(new Microsoft.Xna.Framework.Vector3(l.X, l.Y, lightZ0));
						lightGainsParameter.SetValue(new Microsoft.Xna.Framework.Vector4(0.25f, 1.25f, 0.375f, 1.0625f));
						lightAmbientParameter.SetValue(new XVector4(.463f, .576f, .769f, 1f) * 0.25f);
						lightColorParameter.SetValue(new XVector4(1.0f, 1.0f, 1.0f, 1.0f) * 1.25f);
						lightSpecularParameter.SetValue(new XVector4(1.0f, 1.0f, 1.0f, 1.0f) * 1.25f);
					}
					else
					{
						var x = (((int)cameraCLag.X) / 100) * 100 + 50;
						var y = (((int)cameraCLag.Y) / 100) * 100 + 50;
						var cc = new Vector2(x, y).WToCamera().CameraToScreen();

						lightPositionParameter.SetValue(new Microsoft.Xna.Framework.Vector3(cc.X, cc.Y, lightZDay * (pixelScale / 64.0f)));
						lightGainsParameter.SetValue(new Microsoft.Xna.Framework.Vector4(0.25f, 1.20f, 0.4f, 1.1875f));
						lightAmbientParameter.SetValue(new XVector4(.463f, .576f, .769f, 1f) * 0.5f);
						lightColorParameter.SetValue(new XVector4(1f, 1.0f, 1.0f, 1f) * 1.25f);
						lightSpecularParameter.SetValue(new XVector4(1.0f, 1.0f, 1.0f, 1.0f) * 1.0f);
					}
					cameraReferencePositionParameter.SetValue(new Microsoft.Xna.Framework.Vector3(halfSpan.X, halfSpan.Y, lightZ0));
					//					defaultEffect.Parameters["DiffuseColor"].SetValue(new Microsoft.Xna.Framework.Vector4(1, 1, 1, 1));
					var gain1 = bulgeInputGain * bulgeGain * bulgeSpan;
					var planetGains = new XVector4(bulgeGain, -gain1, MathF.Sqrt(gain1) * 2.0f, 0);
					planetGainsParamater.SetValue(planetGains);

					cameraCParameter.SetValue(new Vector4(cameraCLag.X, cameraCLag.Y, 0, 1.0f));
					pixelScaleParameter.SetValue(new Vector4(pixelScale, pixelScale, pixelScale, pixelScale));



					draw.Begin();

				}

				var focusOnCity = (ShellPage.viewMode == ShellPage.ViewMode.city);

				parallaxGain = SettingsPage.parallax * MathF.Sqrt(Math.Min(11.0f, cameraZoomLag / 64.0f)) * regionAlpha * (1 - cityAlpha);

				{
					var halfTiles = (clientSpan * (0.5f / cameraZoomLag));
					var _c0 = (cameraCLag - halfTiles);
					var _c1 = cameraCLag + halfTiles;
					cx0 = _c0.X.FloorToInt().Max(0);
					cy0 = (_c0.Y.FloorToInt()).Max(0);
					cx1 = (_c1.X.CeilToInt() + 1).Min(World.span);
					cy1 = (_c1.Y.CeilToInt() + 2).Min(World.span);
				}
				cullWC = new Span2i((cx0, cy0), (cx1, cy1));

				{
					//	ds.Antialiasing = CanvasAntialiasing.Aliased;
					if (worldBackground != null && wantImage)
					{
						if (wantImage)
						{
							const byte brightness = 128;
							const byte oBrightness = 255;
							const byte alpha = 255;
							const float texelGain = 1.0f / srcImageSpan;
							draw.AddQuad(COTG.Draw.Layer.background, worldBackground,
								destP0, destP1, srcP0 * texelGain, srcP1 * texelGain,
								 new Color(brightness, brightness, brightness, alpha), ConstantDepth, 0); ;

							if (worldObjects != null)
								draw.AddQuad(COTG.Draw.Layer.background + 1, worldObjects,
									destP0, destP1, srcP0 * texelGain, srcP1 * texelGain, new Color(oBrightness, oBrightness, oBrightness, alpha), ConstantDepth, zCities);
						}
					}

					//   ds.Antialiasing = CanvasAntialiasing.Antialiased;
					// ds.Transform = new Matrix3x2( _gain, 0, 0, _gain, -_gain * ShellPage.cameraC.X, -_gain * ShellPage.cameraC.Y );

					//           dxy.X = (float)sender.Width;
					//            dxy.Y = (float)sender.ActualHeight;

					//            ds.DrawLine( SC(0.25f,.125f),SC(0.lineThickness,0.9f), raidBrush, lineThickness,defaultStrokeStyle);
					//           ds.DrawLine(SC(0.25f, .125f), SC(0.9f, 0.lineThickness), shadowBrush, lineThickness, defaultStrokeStyle);
					// if (IsPageDefense())
					var wantDarkText = isWinter;




					var rgb = attacksVisible ? 255 : 255;
					var tint = new Color(rgb, rgb, rgb, intAlpha);
					var tintShadow = new Color(0, 0, 32, isWinter ? intAlpha * 3 / 4 : intAlpha / 2);
					//	var tintAlpha = (byte)(alpha * 255.0f).RoundToInt();

					//if (isWinter)
					//{
					//	nameColor = new Color() { A = intAlpha, G = 0, B = 0, R = 0 };
					//	nameColorHover = new Color() { A = intAlpha, G = 0, R = 0, B = 64 };
					//	myNameColor = new Color() { A = intAlpha, G = 64, B = 64, R = 0 };
					//	nameColorIncoming = new Color() { A = intAlpha, G = 220 / 3, B = 0, R = 255 / 3 };
					//	nameColorSieged = new Color() { A = intAlpha, G = 220 / 3, B = 190 / 3, R = 255 / 3 };
					//	nameColorIncomingHover = new Color() { A = intAlpha, G = 220 / 3, B = 160 / 3, R = 255 / 3 };
					//	nameColorSiegedHover = new Color() { A = intAlpha, G = 220 / 3, B = 140 / 3, R = 255 / 3 };
					//	myNameColorIncoming = new Color() { A = intAlpha, G = 240 / 3, B = 150 / 3, R = 255 / 3 };
					//	myNameColorSieged = new Color() { A = intAlpha, G = 240 / 3, B = 120 / 3, R = 255 / 3 };
					//}
					//else
					{
						nameColor = new Color() { A = intAlpha, G = 255, B = 255, R = 255 };
						nameColorHover = new Color() { A = intAlpha, G = 255, B = 255, R = 140 };
						myNameColor = new Color() { A = intAlpha, G = 255, B = 150, R = 170 };
						nameColorIncoming = new Color() { A = intAlpha, G = 190, B = 190, R = 255 };
						nameColorSieged = new Color() { A = intAlpha, G = 190, B = 160, R = 255 };
						nameColorIncomingHover = new Color() { A = intAlpha, G = 190, B = 170, R = 255 };
						nameColorSiegedHover = new Color() { A = intAlpha, G = 190, B = 140, R = 255 };
						myNameColorIncoming = new Color() { A = intAlpha, G = 220, B = 120, R = 255 };
						myNameColorSieged = new Color() { A = intAlpha, G = 200, B = 120, R = 255 };
					}
					//			shadowColor = new Color() { A = 128 };


					if (wantDetails && TilesReady())
					{
						var td = TileData.instance;
						{
							// 0 == land
							// 1 == shadows
							// 2 == features
							for (int pass = 0; pass < 3; ++pass)
							{
								if (pass == 1 && (!wantParallax))
									continue;
								foreach (var layer in td.layers)
								{
									var layerDat = layer.data;

									for (var cy = cy0; cy < cy1; ++cy)
									{
										for (var cx = cx0; cx < cx1; ++cx)
										{
											var ccid = cx + cy * World.span;
											var imageId = layerDat[ccid];
											if (imageId == 0)
												continue;

											{
												var cid = (cx, cy).WorldToCid();
												//   var layerData = TileData.packedLayers[ccid];
												//  while (layerData != 0)
												{
													//    var imageId = ((uint)layerData & 0xffffu);
													//     layerData >>= 16;
													var tileId = imageId >> 13;
													var off = imageId & ((1 << 13) - 1);
													var tile = td.tilesets[tileId];

													if (tile.material == null)
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
													if ((pass == 2))
														_tint.A = intAlpha; ;
													if (wantCity && cid == City.build && layer.id > 1)
													{
														if (cityAlphaI >= 255)
															continue;
														_tint.A = (byte)((_tint.A * (256 - cityAlphaI)) / 256);
													}

													var dz = tile.z * parallaxGain; // shadows draw at terrain level 



													if (tile.canHover && pass > 0 && viewHovers.TryGetValue((aa) => aa.cid == cid, out var z))
													{
														dz += z.z * viewHoverZGain;
													}


													var wc = new Vector2(cx, cy);
													var cc = wc.WToCamera();
													if (pass == 1)
													{

														// shift shadow
														//	cc = (cc - cameraLightC) * (1 + dz*2) + cameraLightC;
														//		cc = (cc - cameraLightC)*
														dz = 0;
													}
													var shift = new Vector2(pixelScale * 0.5f);
													var cc0 = cc - shift;
													var cc1 = cc + shift;
													var sy = off / tile.columns;
													var sx = off - sy * tile.columns;
													var uv0 = new Vector2((sx) * tile.scaleXToU + tile.halfTexelU, (sy) * tile.scaleYToV + tile.halfTexelV);
													var uv1 = new Vector2((sx + 1) * tile.scaleXToU + tile.halfTexelU, (sy + 1) * tile.scaleYToV - tile.halfTexelV);

													draw.AddQuad(pass switch { 0 => Layer.tileBase + layer.id, 1 => Layer.tileShadow, _ => Layer.tiles + layer.id },
														(pass == 1 ? tile.shadowMaterial : tile.material), cc0, cc1,
														uv0,
														uv1, _tint,
														(dz, dz, dz, dz));
													//(cc0, cc1).RectDepth(dz));



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
					if (worldChanges != null && !focusOnCity)
					{
						var t2d = worldChanges.texture2d;
						//var tOffset = new Vector2(0.0f, 0.0f);
						//var t2d = worldChanges.texture2d;
						//var scale = new Vector2(t2d.TexelWidth, t2d.TexelHeight);
						draw.AddMesh(tesselatedWorld, Layer.tileShadow - 1, worldChanges);
					}
					if (worldOwners != null && !focusOnCity  )
					{
						var tOffset = new Vector2(0.0f, 0.0f);
						var t2d = worldOwners.texture2d;
						var scale = new Vector2(t2d.TexelWidth, t2d.TexelHeight);
						draw.AddQuad(Layer.tiles - 1, worldOwners,
							destP0, destP1,
							(srcP0 - tOffset) * scale, (srcP1 - tOffset) * scale, new Color(128, 128, 128, 128), ConstantDepth, zTerrain);
					}
					if (wantCity)
					{
						// this could use refactoring
						CityView.Draw(cityAlpha);
					}

					circleRadiusBase = circleRadMin * shapeSizeGain * 7.9f;
					var circleRadius = animTLoop.Lerp(circleRadMin, circleRadMax) * shapeSizeGain * 6.5f;
					//    var highlightRectSpan = new Vector2(circleRadius * 2.0f, circleRadius * 2);

					//	ds.FillRectangle(new Rect(0, 0, clientSpan.X, clientSpan.Y), JSClient.webViewBrush);



					if (!focusOnCity)
					{
						var defenderVisible = IncomingTab.IsVisible() || NearDefenseTab.IsVisible() ||SettingsPage.incomingAlwaysVisible;
						var outgoingVisible = OutgoingTab.IsVisible() ||SettingsPage.attacksAlwaysVisible;
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
											if (!(targetCid.TestContinentFilter() | sourceCid.TestContinentFilter()))
												continue;

											var c1 = targetCid.CidToCamera();
											var c0 = sourceCid.CidToCamera();
											// cull (should do this pre-transform as that would be more efficient
											if (IsCulled(c0, c1))
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
												var nSprite = attack.troops.Length;

												(int iType, float alpha) = GetTroopBlend(t, nSprite);
												DrawAction(dt1, journeyTime, r, c0, c1, c, troopImages[attack.troops[iType].type], true, attack,alpha: alpha);
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
											var wc = cid.CidToWorld();
											if (!IsCulledWC(wc))
												DrawTextBox($"{count.prior}`{count.incoming}", wc.WToCamera(), textformatLabel, Color.DarkOrange, textBackgroundOpacity, Layer.tileText);


										}
									}
								}
							}
							if (AttackTab.IsVisible())
							{
								if (AttackTab.attackClusters != null)
								{
									var showAll = AttackTab.attackClusters.Length < 50;
									foreach (var cluster in AttackTab.attackClusters)
									{
										var selected = false;
										foreach (var i in cluster.attacks)
										{
											if (showAll ||Spot.IsSelectedOrHovered(i, true))
											{
												selected = true;
												break;
											}
										}
										foreach (var i in cluster.targets)
										{
											if (showAll || Spot.IsSelectedOrHovered(i, true))
											{
												selected = true;
												break;
											}
										}
										{
											var c0 = cluster.topLeft.WToCamera();
											var c1 = cluster.bottomRight.WToCamera();
											DrawRect(Layer.effects, c0, c1, selected ? new Color(128,0,0,128) : new Color(64, 0, 0, 128) );
										}

										if (selected)
										{
											var real = cluster.real;
											var c0 = real.CidToCamera();
											foreach (var a in cluster.attacks)
											{
												var t = (tick * a.CidToRandom().Lerp(1.5f / 512.0f, 1.75f / 512f)) + 0.25f;
												var r = t.Ramp();
												var c1 = a.CidToCamera();
												var spot = Spot.GetOrAdd(a);
												DrawAction(0.5f, 1.0f, r, c1, c0, Color.Red, troopImages[(int)spot.GetPrimaryTroopType(false)], false, null);
											}
											foreach (var target in cluster.targets)
											{
												var c = target.CidToCamera();
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
									bool showAll = list.Length <= SettingsPage.showAttacksLimit0 || SettingsPage.incomingAlwaysVisible;
									foreach (var city in list)
									{
										if (!city.testContinentFilter)
											continue;

										if (city.incoming.Any() || city.isMine)
										{

											var targetCid = city.cid;
											var c1 = targetCid.CidToCamera();
											if (IsCulled(c1, cullSlopSpace))  // this is in pixel space - Should be normalized for screen resolution or world space (1 continent?)
												continue;
											var incAttacks = 0;
											var incTs = 0;
											foreach (var i in city.incoming)
											{
												var c0 = i.sourceCid.CidToCamera();
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
												if (!(showAll||Spot.IsSelectedOrHovered(i.sourceCid, targetCid, noneIsAll)))
												{
													continue;
													//       c.A = (byte)((int)c.A * 3 / 8); // reduce alpha if not selected
												}
												if (i.troops.Any())
												{
													var t = (tick * i.sourceCid.CidToRandom().Lerp(1.5f / 512.0f, 2.0f / 512f)) + 0.25f;
													var r = t.Ramp();
													var nSprite = i.troops.Length;

													(int iType, float alpha) = GetTroopBlend(t, nSprite);

													DrawAction(i.TimeToArrival(serverNow), i.journeyTime, r, c0, c1, c, troopImages[i.troops[iType].type], true, i, alpha:alpha);
												}
												else
												{
													Assert(false);
												}
											}
											if (wantDetails || showAll|| Spot.IsSelectedOrHovered(targetCid, noneIsAll))
											{
												DrawTextBox($"{incAttacks}`{city.claim.ToString("00")}%`{(incTs + 500) / 1000}k\n{ (city.tsDefMax + 500) / 1000 }k",
														c1, tipTextFormatCentered, incAttacks != 0 ? Color.White : Color.Cyan, textBackgroundOpacity, Layer.tileText);
											}
										}
									}
								}
								if (defenderVisible)
								{
									foreach (var city in City.myCities)
									{
										if (!city.testContinentFilter)
											continue;
										Assert(city is City);
										if (!city.incoming.Any())
										{
											var targetCid = city.cid;
											var c1 = targetCid.CidToCamera();
											if (IsCulled(c1, cullSlopSpace))  // this is in pixel space - Should be normalized for screen resolution or world space (1 continent?)
												continue;
											if (wantDetails || Spot.IsSelectedOrHovered(targetCid, true))
											{
												DrawTextBox($"{city.reinforcementsIn.Length},{(city.tsDefMax.Max(city.tsHome) + 500) / 1000 }k", c1, tipTextFormatCentered, Color.Cyan, textBackgroundOpacity, Layer.tileText);
											}

										}
									}
								}
							}

							if (NearRes.IsVisible())
							{
								foreach (var city in City.friendCities)
								{
									if (!city.testContinentFilter)
										continue;
									var wc = city.cid.CidToWorld();
									if (IsCulledWC(wc))
										continue;

									var ti = city.tradeInfo;
									if (ti == null)
										continue;
									var cityHover = city.isHover;
									try
									{
										foreach (var toCid in ti.resDest)
										{
											var c1 = toCid.CidToWorld();
											//	var t = (tick * city.cid.CidToRandom().Lerp(1.375f / 512.0f, 1.75f / 512f));
											//	var r = t.Ramp();
											var hover = cityHover | Spot.IsHover(toCid);
											DrawAction( c1.WToCamera(), wc.WToCamera(),hover? tradeColorHover: tradeColor, hover? lineThickness*2f : lineThickness  );


										}
									}
									catch (Exception ex)
									{
										LogEx(ex);
									}

								}
							}

							const int raidCullSlopSpace = 4;
							foreach (var city in City.friendCities)
							{
								if (!city.testContinentFilter)
									continue;

								var wc = city.cid.CidToWorld();
								// Todo: clip thi
								if (city.senatorInfo.Length != 0 && !(IncomingTab.IsVisible() || NearDefenseTab.IsVisible()))
								{
									var c = wc.WToCamera();
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
										if (sen.target != 0 && sen.type == SenatorInfo.Type.settle )
										{
											var c1 = sen.target.CidToCamera();

											var dist = city.cid.DistanceToCid(sen.target) * cartTravel; // todo: ship travel?
											var t = (tick * city.cid.CidToRandom().Lerp(1.5f / 512.0f, 1.75f / 512f)) + 0.25f;
											var r = t.Ramp();
											// Todo: more accurate senator travel times
											DrawAction((float)(sen.time - serverNow).TotalSeconds, dist * 60.0f, r, c, c1, senatorColor,
												troopImages[ttSenator], false, null);
											DrawFlag(sen.target, SpriteAnim.flagGrey, Vector2.Zero);
										}
									}
									if (!IsCulledWC(wc))
										DrawTextBox($"Sen:  {recruiting}`{idle}`{active}", c, tipTextFormatCentered, Color.White, textBackgroundOpacity, Layer.tileText);

								}
								if (city.isMine)
								{
									if (!IsCulledWC(wc))
									{
										if (!city.isSelected || city.cid == City.build)
											DrawFlag(city.cid, city.cid == City.build ? SpriteAnim.flagHome : SpriteAnim.flagRed, new Vector2(4,4) );
									}
									if ((MainPage.IsVisible() && SettingsPage.raidsVisible!=0)||SettingsPage.raidsVisible==1 )
									{
										if (IsCulledWC(wc, raidCullSlopSpace))
											continue;
										var c = wc.WToCamera();
										var t = (tick * city.cid.CidToRandom().Lerp(1.375f / 512.0f, 1.75f / 512f));
										var r = t.Ramp();
										//ds.DrawRoundedSquareWithShadow(c,r, raidBrush);
										foreach (var raid in city.raids)
										{
											var ct = raid.target.CidToCamera();
											(var c0, var c1) = !raid.isReturning ? (c, ct) : (ct, c);
											DrawAction((float)(raid.time - serverNow).TotalSeconds,
												raid.GetOneWayTripTimeMinutes(city) * 60.0f,
												r, c0, c1, raidColor, troopImages[raid.troopType], false, null);

										}
									}
								}
							}
						}

						foreach (var cid in Spot.selected)
						{
							DrawFlag(cid, SpriteAnim.flagSelected, Vector2.Zero);
						}
						foreach (var cid in SettingsPage.pinned)
						{
							DrawFlag(cid, SpriteAnim.flagPinned, new Vector2(4, -4));
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

						if(Player.viewHover != 0)
						{
							if (Player.all.TryGetValue(Player.viewHover, out var p))
							{
								try
								{
									foreach (var cid in p.cities)
									{
										DrawFlag(cid, SpriteAnim.flagGrey, new Vector2(-4,4) );
									}
								}
								catch(Exception ex)
								{
									//LogEx(ex); // collection might change, if this happens just about this render, its 
								}
								
							}
						}

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
										//var zScale = CanvasHelpers.ParallaxScale(TileData.zCities);

										if (name != null)
										{

											var span = pixelScale;
											var cid = (cx, cy).WorldToCid();

											var drawC = (new Vector2(cx, cy).WToCamera());
											drawC.Y += span * (isWinter ? 8.675f / 16.0f : 7.125f / 16.0f);
											var z = zCities;
											var scale = bmFontScale;


											if (viewHovers.TryGetValue((aa) => aa.cid == cid, out var viewHover))
											{
												z += viewHover.z * viewHoverZGain;
												scale *= viewHover.z.Lerp(1.0f, 1.25f);
											}
											//	drawC = drawC.Project(zLabels);
											var layout = GetTextLayout(name, nameTextFormat);
											var color = isMine ?
												(hasIncoming ?
													(spot.underSiege ? myNameColorSieged
																	: myNameColorIncoming)
																		: myNameColor) :
											(hasIncoming ?
												(hovered ?
													(spot.underSiege ? nameColorSiegedHover : nameColorIncomingHover)
												   : (spot.underSiege ? nameColorSieged : nameColorIncoming))
												   : hovered ? nameColorHover : nameColor);

											DrawTextBox(name, drawC, nameTextFormat, wantDarkText ? color.A.AlphaToBlack() : color,

												!isWinter ? new Color() :
													wantDarkText ? new Color(color.R, color.G, color.B, (byte)128) : 128.AlphaToBlack(), Layer.tileText, 2, 0, PlanetDepth, z, scale);
											//										layout.Draw(drawC,
											//									, Layer.tileText, z,PlanetDepth);

										}
										if (spot != null && !focusOnCity && !(SettingsPage.troopsVisible.HasValue && SettingsPage.troopsVisible.Value==false) ) 
										{
											if (!spot.troopsTotal.Any() && spot.isNotClassified && spot.isFriend && SettingsPage.troopsVisible.GetValueOrDefault()  )
												spot.Classify();
											if (spot.troopsTotal.Any()|| spot.isClassified)
											{
												var c1 = (cx, cy).WToCamera();
												var rand = spot.cid.CidToRandom();
												var t = (tick * rand.Lerp(1.5f / 512.0f, 1.75f / 512f)) + 0.25f;

												int type;
												float typeBlend;
												float alpha = 1;// (t * rand.Lerp(0.7f, 0.8f)).Wave() * 0.20f + 0.70f;

												if ( spot.troopsTotal.Any())
												{
													var ta  = GetTroopBlend(t, spot.troopsTotal.Length);
													alpha = ta.alpha;
													
													type = spot.troopsTotal[ta.iType].type;
												}
												else
												{
													type = spot.classificationTT;
													typeBlend = 1;
													switch( spot.classification)
													{
														case Spot.Classification.misc:
														case Spot.Classification.unknown:
														case Spot.Classification.pending:
														case Spot.Classification.missing:
															goto dontDraw;
													}

												}
												var r = t.Ramp();
												var spriteSize = new Vector2(32*SettingsPage.iconScale);
												var _c0 = c1 - spriteSize;
												var _c1 = c1 + spriteSize;

												draw.AddQuadWithShadow(Layer.effects,Layer.effectShadow, troopImages[type], _c0, _c1, HSLToRGB.ToRGBA(rectSpan, 0.3f, 0.825f, alpha, alpha + 0.125f), ShadowColor(alpha),
													(_c0, _c1).RectDepth(zCities), (_c0, _c1).RectDepth(zEffectShadow), shadowOffset);
											}
											dontDraw:;
										}
									}
								}
							}
						}
					}
					if (!ShellPage.IsCityView() && Player.isTest)
					{
						var avatars = PlayerPresence.all;
						foreach (var a in avatars)
						{
							var cid = a.cid;
							var pid = a.pid;
							if (Player.myId == pid) // don't show me
								continue;

							var wc = cid.CidToWorld();
							if (!IsCulledWC(wc))
							{
								DrawTextBox($"~{Player.IdToName(pid)}~", wc.WToCamera(), tipTextFormatCentered, Color.Red, 255, Layer.tileText, 3, 3, null, -1, 0.75f * SettingsPage.fontScale);
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
						//	TextLayout textLayout = GetTextLayout( _toolTip, tipTextFormat);
						//	var bounds = textLayout.span;
						Vector2 c = ShellPage.mousePositionC + new Vector2(16, 16);
						DrawTextBox(_toolTip, c, tipTextFormat, Color.White, 192, Layer.overlay, 11, 11, ConstantDepth, 0, 0.5f);
					}
					var _contTip = ShellPage.contToolTip;
					if (_contTip != null)
					{
						var alpha = pixelScale.SmoothStep(cityZoomThreshold - 128, cityZoomThreshold + 128).
							Max(pixelScale.SmoothStep(cityZoomWorldThreshold + 16, cityZoomWorldThreshold - 16));
						Vector2 c = new Vector2(20, 16).ScreenToCamera();
						DrawTextBox(_contTip, c, tipTextFormat, Color.White.Scale(alpha), (byte)(alpha * 192.0f).RoundToInt(), Layer.overlay, 11, 11, ConstantDepth, 0, 0.5f);
					}
					if (ShellPage.IsCityView())
					{
						var alpha = 255;
						Vector2 c = new Vector2(clientSpan.X - 32, 16).ScreenToCamera();
						DrawTextBox($"{CityBuild.postQueueBuildingCount}/{CityBuild.postQueueTownHallLevel*10}", c, tipTextFormatRight, Color.White.Scale(alpha), (byte)(alpha * 192.0f).RoundToInt(), Layer.overlay, 11, 11, ConstantDepth, 0, 0.5f);

					}
					var _debugTip = ShellPage.debugTip;
					if (_debugTip != null)
					{
						var alpha =  255;
						Vector2 c = new Vector2(clientSpan.X-16, 16).ScreenToCamera();
						DrawTextBox(_debugTip, c, tipTextFormatRight, Color.White.Scale(alpha), (byte)(alpha * 192.0f).RoundToInt(), Layer.overlay, 11, 11, ConstantDepth, 0, 0.5f);
					}
					

				}
				if (popups.Length > 0)
				{
					var color = isFocused ? new Color(135, 235, 255, 255) : new Color(255, 255, 255, 255);
					foreach (var pop in popups)
					{
						Vector2 c0 = new Vector2(pop.c0.X, pop.c0.Y).ScreenToCamera();
						Vector2 c1 = new Vector2(pop.c1.X, pop.c1.Y).ScreenToCamera();
						draw.AddQuad(Layer.webView, quadTexture, c0, c1, color, ConstantDepth, 0);/// c0.CToDepth(),(c1.X,c0.Y).CToDepth(), (c0.X,c1.Y).CToDepth(), c1.CToDepth() );

					}

				}
				draw.End();

			}
			catch (Exception ex)
			{
				LogEx(ex);
				draw._beginCalled = false;
			}

			static (int iType, float alpha) GetTroopBlend(float t, int nSprite)
			{
				int iType = 0;
				float alpha = 1;
				if (nSprite > 1)
				{
					var fSprite = nSprite;
					Assert(t > 0);
					t -=  MathF.Floor(t /fSprite)*fSprite;
					var rType = MathF.Floor(t);
					t -= rType;

					iType = ((int)rType).Min(nSprite-1);
					if (t < 0.25f)
						alpha = AMath.SCurve(t * 4.0f);
					else if (t > 0.75f)
						alpha = AMath.SCurve( (1 - t) * 4.0f);

				}
				return (iType, alpha);
			}
		}
		static int _blend;
		static Vector2 _uv0;
		static Vector2 _uv1;
		private static void DrawFlag(int cid, SpriteAnim sprite, Vector2 offset)
		{
			var wc = cid.CidToWorld();
			if (IsCulledWC(wc))
				return;

			var c = wc.WToCamera()+ offset;
			var dv = AGame.shapeSizeGain*48*4*SettingsPage.flagScale;
			float z = zLabels;

			// hover flags
			if (viewHovers.TryGetValue((aa) => aa.cid == cid, out var dz))
			{
				c.Y -= dz.z * (5.0f * shapeSizeGain); // 8 pixels up regardless of scale
				z += dz.z * viewHoverZGain;
			}
			double frameCount = sprite.frameCount;
			double frameTotal = ((animationT + cid.CidToRandom() * 15.0) * 12.0);
			var frameWrap = frameTotal % frameCount;

			var frameI = Math.Floor(frameWrap);
			var frameMod = (frameWrap - frameI) * 255.0 + 0.325;
			Assert(frameMod >= 0);
			Assert(frameMod < 256.0f);
			var blend = (int)(frameMod);
			//	_blend = blend;
			var c0 = new Vector2(c.X, c.Y - dv * 0.435f * 0.75f);
			Vector2 c1 = new Vector2(c.X + dv * 0.5f * 0.75f, c.Y - dv * 0.035f * 0.75f);
			var uv0 = new Vector2((float)(frameI / frameCount), 0.0f);
			var uv1 = new Vector2((float)((frameI + 1.0f) / frameCount), 1.0f);
			//	_uv0 = uv0;
			//	_uv1 = uv1;
			draw.AddQuad(Layer.effects, sprite.material, c0, c1,
				uv0,
				uv1,
				new Color(blend, sprite.frameDeltaG, sprite.frameDeltaB, 255), (c0, c1).RectDepth(z));
		}

		private static void FillRoundedRectangle(int layer, Vector2 c0, Vector2 c1, Color background, DepthFunction depth, float z)
		{
			draw.AddQuad(layer, quadTexture, c0, c1, background, depth, z);/// c0.CToDepth(),(c1.X,c0.Y).CToDepth(), (c0.X,c1.Y).CToDepth(), c1.CToDepth() );
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
		public static void DrawTextBox(string text, Vector2 at, TextFormat format, Color color, byte backgroundAlpha, int layer = Layer.tileText, float _expandX = 2.0f, float _expandY = 0, DepthFunction depth = null, float zBias = -1, float scale = 0)
		{
			DrawTextBox(text, at, format, color, backgroundAlpha == 0 ? new Color() : color.IsDark() ? new Color((byte)255, (byte)255, (byte)255, backgroundAlpha) : new Color((byte)(byte)0, (byte)0, (byte)0, backgroundAlpha), layer, _expandX, _expandY, depth, zBias, scale);
		}

		private static void DrawTextBox(string text, Vector2 at, TextFormat format, Color color, Color backgroundColor, int layer = Layer.tileText, float _expandX = 0.0f, float _expandY = 0, DepthFunction depth = null, float zBias = -1, float scale = 0)
		{
			if (IsCulled(at, textBoxCullSlop))
			{
				return;
			}
			if (scale == 0)
				scale = bmFontScale;
			if (scale == 0)
				return;

			TextLayout textLayout = GetTextLayout(text, format);
			if (zBias == -1)
				zBias = zLabels;

			var constantDepth = depth == null;

			// shift everything, then ignore Z
			if (constantDepth)
			{
				depth = ConstantDepth;
				var atScale = at.Project(zBias);
				at = atScale.c;
				scale *= atScale.scale;
				zBias = 0;
			}
			var span = textLayout.ScaledSpan(scale);
			var expand = new Vector2(_expandX, _expandY);
			if (backgroundColor.A > 0)
			{
				var c0 = at;
				if (format.horizontalAlignment == TextFormat.HorizontalAlignment.center)
					c0.X -= span.X * 0.5f;
				if (format.horizontalAlignment == TextFormat.HorizontalAlignment.right)
					c0.X -= span.X;
				if (format.verticalAlignment == TextFormat.VerticalAlignment.center)
					c0.Y -= span.Y * 0.5f;
				if (format.verticalAlignment == TextFormat.VerticalAlignment.bottom)
					c0.Y -= span.Y;
				backgroundColor.A = (byte)(((int)backgroundColor.A * color.A) / 255);
				FillRoundedRectangle(Layer.textBackground, c0 - expand, c0 + expand + span, backgroundColor, depth, zBias);
			}
			textLayout.Draw(at, scale, color, layer, zBias, depth);
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
		Material bitmap, bool applyStopDistance, Army army, float alpha = 1)
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
			var shadowColor = ShadowColor(alpha);
			if (army != null)
			{
				var d2 = Vector2.DistanceSquared(mid, ShellPage.mousePositionW);
				if (d2 < bestUnderMouseScore)
				{
					bestUnderMouseScore = d2;
					underMouse = army;
				}
			}
			DrawLine(Layer.effectShadow, c0 + shadowOffset, c1 + shadowOffset, GetLineUs(c0, c1), shadowColor, zEffectShadow);
			if (applyStopDistance)
				DrawSquare(Layer.effectShadow, c0+ shadowOffset, shadowColor, zEffectShadow);
			DrawLine(Layer.action + 2, c0, mid, GetLineUs(c0, mid), color, zLabels);
			if (applyStopDistance)
				DrawSquare(Layer.action + 3, new Vector2(c0.X, c0.Y), color, zLabels);
			float spriteSize = 32 * SettingsPage.iconScale;
		    var dc = new Vector2(spriteSize, spriteSize);
			if (bitmap != null)
			{
				var _c0 = new Vector2(mid.X - spriteSize, mid.Y - spriteSize);
				var _c1 = new Vector2(mid.X + spriteSize, mid.Y + spriteSize);
				draw.AddQuadWithShadow(Layer.action + 4,Layer.effectShadow, bitmap, _c0, _c1, HSLToRGB.ToRGBA(rectSpan, 0.3f, 0.825f, alpha, gain * 1.1875f),shadowColor, (_c0, _c1).RectDepth(zLabels), (_c0, _c1).RectDepth(zEffectShadow),shadowOffset);
			}
			//            ds.DrawRoundedSquare(midS, rectSpan, color, 2.0f);


		}

		private void DrawAction( Vector2 c0, Vector2 c1, Color color, float thickness = lineThickness)
		{
			DrawAction(0, 1.0f, 1, c0, c1, color, null, false, null,alpha:1);
			

/*			if (IsCulled(c0, c1))
				return;

			var dl = (c0 - c1);
			var dllg = dl.Length();
			var inverseL = 1.0f / dllg;
			var t0 = (animationT * actionAnimationGain * inverseL) % 1.0f;

			var cm = t0.Lerp(c0, c1);

			DrawLine(Layer.effectShadow, cm, cm + dl*inverseL*drawActionLength, (0.0f,1.0f), color, zEffectShadow);
*/
		}
		private static void DrawSquare(int layer, Vector2 c0, Color color, float zBias)
		{
			draw.AddQuad(layer, quadTexture, new Vector2(c0.X - smallRectSpan, c0.Y - smallRectSpan), new Vector2(c0.X + smallRectSpan, c0.Y + smallRectSpan), new Vector2(0, 0), new Vector2(1, 1), color, PlanetDepth, zBias);
		}
		private static void DrawRect(int layer, Vector2 c0, Vector2 c1, Color color)
		{
			draw.AddQuad(layer, quadTexture, c0, c1, new Vector2(), new Vector2(1, 1), color, PlanetDepth, zLabels);
		}

		(float u, float v) GetLineUs(Vector2 c0, Vector2 c1)
		{
			float offset = (lineAnimationGain * animationTWrap) % 1;
			return (offset + (c0 - c1).Length() * lineTileGain, offset);

		}
		private static void DrawLine(int layer, Vector2 c0, Vector2 c1, (float u, float v) uv, Color color, float zBias, float thickness= lineThickness)
		{
			//	draw.AddLine(layer,lineDraw, c0, c1, lineThickness, , color,(c0.CToDepth()+ zBias, c1.CToDepth()+ zBias) );
			draw.AddLine(layer, lineDraw, c0, c1, thickness, uv.u, uv.v, color, (c0.CToDepth() + zBias, c1.CToDepth() + zBias));
		}



		public static void DrawAccentBaseI(float cX, float cY, float radius, float angle, Color color, int layer, float zBias)
		{
			var dx0 = radius * MathF.Cos(angle);
			var dy0 = radius * MathF.Sin(angle);
			var angle1 = angle + MathF.PI * 0.1875f;
			var dx1 = radius * MathF.Cos(angle1);
			var dy1 = radius * MathF.Sin(angle1);
			DrawLine(layer, new Vector2(cX + dx0, cY + dy0), new Vector2(cX + dx1, cY + dy1), (angle.SignOr0(), 0), color, zBias);
			// rotated by 180
			DrawLine(layer, new Vector2(cX - dx0, cY - dy0), new Vector2(cX - dx1, cY - dy1), (angle.SignOr0(), 0), color, zBias);
		}
		public static void DrawAccentBase(float cX, float cY, float radius, float angle, Color color, int layer, float zBias)
		{
			DrawAccentBaseI(cX, cY, radius, angle, color, layer, zBias);
			DrawAccentBaseI(cX, cY, radius * 0.875f, angle + angle.SignOr0() * 0.125f, color, layer, zBias);
			//DrawAccentBaseI(ds, cX, cY, radius*0.655f, angle+angle.SignOr0()*0.25f, color);
		}

		public static void DrawAccent(Vector2 c, float radius, float angularSpeed, Color brush)
		{
			var angle = angularSpeed * AGame.animationT;
			DrawAccentBase(c.X+ shadowOffset.X, c.Y+shadowOffset.Y, radius, angle, brush.GetShadowColorDark(), Layer.effectShadow, zEffectShadow);
			DrawAccentBase(c.X, c.Y, radius, angle, brush, Layer.overlay, zLabels);
		}
		public static void DrawAccent(int cid, float angularSpeedBase, Color brush)
		{

			var wc = cid.CidToWorld();
			if (IsCulledWC(wc))
				return;

			var c = wc.WToCamera();

			var rnd = cid.CidToRandom();

			var angularSpeed = angularSpeedBase + rnd * 0.5f;
			var t = (AGame.animationT * rnd.Lerp(1.25f / 256.0f, 1.75f / 256f));
			var r = t.Wave().Lerp(AGame.circleRadiusBase, AGame.circleRadiusBase * 1.375f);
			DrawAccent(c, r, angularSpeed, brush);
		}
		public static float PlanetDepth(float x, float y, float zBase)
		{
			return new Vector2(x, y).CToDepth() + zBase;
		}
		public static float ConstantDepth(float x, float y, float zBase)
		{
			return zBase;

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
		public static bool IsValid(this (int x, int y) c) => c.x != int.MinValue || c.y != int.MinValue;
		public static (int x, int y) invalidXY = (int.MinValue, int.MinValue);


		public static bool IsDark(this Color color) => ((int)color.R + color.G + color.B) < (int)color.A * 3 / 2;

		public static Color AlphaToWhite(this int alpha) { return new Color(255, 255, 255, alpha); }
		public static Color AlphaToAll(this int alpha) { return new Color(alpha, alpha, alpha, alpha); }
		public static Color AlphaToBlack(this int alpha) { return new Color(0, 0, 0, alpha); }

		public static Color Scale(this Color value, Vector4 scale)
		{
			return new Color((int)((float)(int)value.R * scale.X), (int)((float)(int)value.G * scale.Y), (int)((float)(int)value.B * scale.Z), (int)((float)(int)value.A * scale.W));

		}
		public static Color Scale(this Color value, float scale)
		{
			return new Color((int)((float)(int)value.R * scale), (int)((float)(int)value.G * scale), (int)((float)(int)value.B * scale), (int)((float)(int)value.A * scale));

		}
		public static Color Scale(this Color value, Vector2 scale)
		{
			return new Color((int)((float)(int)value.R * scale.X), (int)((float)(int)value.G * scale.X), (int)((float)(int)value.B * scale.X), (int)((float)(int)value.A * scale.Y));

		}

		public static Color AlphaToWhite(this byte alpha) { return new Color((byte)255, (byte)255, (byte)255, alpha); }
		public static Color AlphaToBlack(this byte alpha) { return new Color((byte)0, (byte)0, (byte)0, alpha); }

		//public static Point2 ToPoint(this Vector2 me) => new Point2(me.X,me.Y);
		public static Vector2 ToV2(this Point me) => new Vector2(me.X, me.Y);
		public static Vector2 ToV2(this Microsoft.Xna.Framework.Vector2 me) => new Vector2(me.X, me.Y);
		public static Vector2 ToV2(this Vector3 me) => new Vector2(me.X, me.Y);

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
		//	public static float parallaxZ0 = 1024;
		//public static float ParallaxScale(float dz) => SettingsPage.wantParallax? parallaxZ0 / (parallaxZ0 - dz) : 1.0f;
		// for now we just the same bias for shadows as for light, assuming that the camera is as far from the world as the light
		//	public static float ParallaxScaleShadow(float dz) => 1;// parralaxZ0 / (parralaxZ0 - dz);
		public static float bulgeInputGain = 0;
		public static float bulgeNegativeRange = 0.875f;
		public static float CToDepth(this Vector2 c)
		{
			float r2 = (c.LengthSquared() * bulgeInputGain).Min(1.0f);
			//			return r.SLerp(AGame.bulgeGain, 0.0f);
			//Assert(r2 <= 2.0f);
			return (r2).Lerp(1, -bulgeNegativeRange) * AGame.bulgeGain;
			//	return Math.Acos(r.SLerp(AGame.bulgeGain, 0.0f);

		}

		public static Vector2 Project(this Vector3 c)
		{
			float scale = 1.0f / (AGame.cameraZ - c.Z);
			return new Vector2(c.X * scale, c.Y * scale);

		}
		public static (Vector2 c, float scale) Project(this Vector2 c, float zBias)
		{
			float scale = 1.0f / (AGame.cameraZ - (c.CToDepth() + zBias));
			return (new Vector2(c.X * scale, c.Y * scale), scale);

		}

		public static Vector2 InverseProject(this Vector2 c, float z)
		{
			float scale = (AGame.cameraZ - z);
			return new Vector2(c.X * scale, c.Y * scale);
		}

		public static Vector2 InverseProject(this Vector2 c)
		{
			float zBias = AGame.zCities * 0.5f;
			float z = CToDepth(c) + zBias;

			for (int i = 0; i < 4; ++i)
			{
				var test = InverseProject(c, z);
				z = new Vector2(test.X, test.Y).CToDepth() + zBias;
			}
			return InverseProject(c, z);
		}

		public static float CToDepth(this (float x, float y) c) => CToDepth(new Vector2(c.x, c.y));
		//	public static Vector2 WToCp(this Vector2 c, float dz)
		//{
		//	var paralaxGain = ParallaxScale(dz);

		//	return (c- AGame.cameraCLag)* paralaxGain*AGame.pixelScale;
		//}
		//	public static Vector2 WToCp(this (float x, float y) c, float dz) => WToCp(new Vector2(c.x, c.y), dz);
		public static Vector2 ScreenToCamera(this Vector2 s)
		{
			return s - AGame.halfSpan;
		}
		public static Vector2 CameraToScreen(this Vector2 s)
		{
			return s + AGame.halfSpan;
		}
		//	public static Vector2 WToCpSpan(this Vector2 c, float dz)
		//{
		//	var paralaxGain = ParallaxScale(dz);

		//	return (c)* paralaxGain*AGame.pixelScale ;
		//}
		//public static float WToCpSpan( float dz)
		//{
		//	var paralaxGain = ParallaxScale(dz);

		//	return paralaxGain * AGame.pixelScale;
		//}
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
		//public static Vector2 CToCp(this Vector2 c, float dz)
		//{
		//	var paralaxGain = ParallaxScale(dz);
		//		return (c) * paralaxGain;
		//}
		//public static Point CToCp(this Vector2 c, float dz)
		//{
		//	var paralaxGain = ParalaxScale(dz);
		//	return ((c - AGame.halfSpan) * paralaxGain + AGame.halfSpan).ToPoint();

		//}

		public static Vector2 WToCamera(this Vector2 c)
		{
			return (c - AGame.cameraCLag) * AGame.pixelScale;
		}
		public static Vector2 WToCamera(this (int x, int y) c)
		{
			return new Vector2(c.x, c.y).WToCamera();
		}
		public static Vector2 WToCamera(this (float x, float y) c)
		{
			return new Vector2(c.x, c.y).WToCamera();
		}
		//	public static Vector2 WToCp(this (int x, int y) c, float z)
		//{
		//	return new Vector2(c.x, c.y).WToCp(z);
		//}
		public static Vector2 CidToCamera(this int c)
		{
			return c.ToWorldC().WToCamera();
		}
		//public static Vector2 CidToCp(this int c, float z)
		//{
		//	return c.ToWorldC().WToCp(z);
		//}a

		public static (float v0, float v10, float v01, float v11) RectDepth(this (Vector2 c0, Vector2 c1) c, float dz)
		{
			return (c.c0.CToDepth() + dz, new Vector2(c.c1.X, c.c0.Y).CToDepth() + dz,
													new Vector2(c.c0.X, c.c1.Y).CToDepth() + dz, c.c1.CToDepth() + dz);
		}
		public static bool BringCidIntoWorldView(this int cid, bool lazy, bool allowZoomChange)
		{
			var v = cid.CidToWorldV();
			var newC = v;
			var dc = newC - AGame.cameraC;
			//	if (ShellPage.IsCityView())
			//		lazy = false;
			// only move if needed, heuristic is if any part is off screen
			if (!lazy ||
				(dc.X.Abs() + 0.5f) * AGame.pixelScale >= AGame.halfSpan.X ||
				(dc.Y.Abs() + 0.5f) * AGame.pixelScale >= AGame.halfSpan.Y)
			{
				var thresh = lazy ? 0.75f : 0.25f;
				// only move if moving more than about 1 city span
				if (Vector2.Distance(AGame.cameraC, newC) >= thresh)
				{
					// try region view
					if (allowZoomChange)
					{
						if ((dc.X.Abs() + 0.5f) * AGame.cameraZoomRegionDefault <= AGame.halfSpan.X &&
							(dc.Y.Abs() + 0.5f) * AGame.cameraZoomRegionDefault <= AGame.halfSpan.Y)
						{

							ShellPage.SetViewModeRegion();
							goto done;
						}

						ShellPage.SetViewModeWorld();

						if ((dc.X.Abs() + 0.5f) * AGame.cameraZoomWorldDefault <= AGame.halfSpan.X &&
						(dc.Y.Abs() + 0.5f) * AGame.cameraZoomWorldDefault <= AGame.halfSpan.Y)
						{
							goto done;
						}

					}
				done:
					AGame.cameraC = newC;
					ShellPage.SetJSCamera();
					if (cid != City.build && (!City.CanVisit(cid) || !Spot.CanChangeCity(cid))  )
						ShellPage.EnsureNotCityView();

					return true;
				}

			}
			return false;
		}
		public static Color GetShadowColor(this Color c)
		{
			return new Color((byte)(c.R * 0 / 4), (byte)(c.G * 0 / 4), (byte)(c.B * 0 / 4), (byte)192);
			//            (0.625f).Lerp(c, new Color(128, 0, 0, 0));
			//            (0.625f).Lerp(c, new Color(128, 0, 0, 0));
		}
		public static Color GetShadowColorDark(this Color c)
		{
			return new Color((byte)(c.R * 0 / 4), (byte)(c.G * 0 / 4), (byte)(c.B * 0 / 4), (byte)192);
			//            (0.625f).Lerp(c, new Color(128, 0, 0, 0));
			//            (0.625f).Lerp(c, new Color(128, 0, 0, 0));
		}
		public static (Vector2 c0, Vector2 c1) GetBounds(this Rectangle textureRegion)
		{
			return (new Vector2(textureRegion.X, textureRegion.Y), new Vector2(textureRegion.Right, textureRegion.Bottom));
		}

		const float gamma = 2.2f;
		public static (float r, float g, float b, float a) LinearToSRGB(this (float r, float g, float b, float a) c)
		{
			return (MathF.Pow(c.r, 1.0f / gamma), MathF.Pow(c.g, 1.0f / gamma), MathF.Pow(c.b, 1.0f / gamma), c.a);

		}
		public static XVector4 LinearToSRGB(this XVector4 c)
		{
			return new XVector4(MathF.Pow(c.X, 1.0f / gamma), MathF.Pow(c.Y, 1.0f / gamma), MathF.Pow(c.Z, 1.0f / gamma), c.W);

		}
		public static Color LinearToSrgb(this Color c)
		{
			return new Color(c.ToVector4().LinearToSRGB());
		}
		public static Color SRGBToLinear(this Color c)
		{
			return new Color(c.ToVector4().SRGBToLinear());
		}
		public static XVector4 SRGBToLinear(this XVector4 c)
		{
			return new XVector4(MathF.Pow(c.X, gamma), MathF.Pow(c.Y, gamma), MathF.Pow(c.Z, gamma), c.W);

		}

	}

	public class TextLayout
	{
		public TextFormat format;
		internal Vector2 normalizedSpan;
		private string text;
		public Vector2 ScaledSpan(float scale) => normalizedSpan * scale;

		public TextLayout(string text, TextFormat format)
		{
			this.text = text;
			this.format = format;
			var size = AGame.bfont.MeasureFont(text);

			normalizedSpan.X = size.Width;// *0.5f;
			normalizedSpan.Y = size.Height;// *0.5f;
		}

		internal void Draw(Vector2 c, float scale, Color color, int layer, float z, DepthFunction depthFunction)
		{
			var span = ScaledSpan(scale);
			if (format.horizontalAlignment == TextFormat.HorizontalAlignment.center)
				c.X -= span.X * 0.5f;
			else if (format.horizontalAlignment == TextFormat.HorizontalAlignment.right)
				c.X -= span.X;
			if (format.verticalAlignment == TextFormat.VerticalAlignment.center)
				c.Y -= span.Y * 0.5f;
			else if (format.verticalAlignment == TextFormat.VerticalAlignment.bottom)
				c.Y -= span.Y;


			AGame.draw.DrawString(AGame.bfont, text, c, scale, color, layer, z, depthFunction);// (c+span*0.5f).CToDepth() );
		}

		//internal void Draw(Vector2 c, Color color,  int layer = Layer.labelText)
		//{
		//	if (format.horizontalAlignment == TextFormat.HorizontalAlignment.center)
		//		c.X -= span.X * 0.5f;
		//	else if (format.horizontalAlignment == TextFormat.HorizontalAlignment.right)
		//		c.X -= span.X;
		//	else if (format.verticalAlignment == TextFormat.VerticalAlignment.center)
		//		c.Y -= span.Y * 0.5f;
		//	else if (format.verticalAlignment == TextFormat.VerticalAlignment.bottom)
		//		c.Y -= span.Y;


		//	AGame.draw.DrawString(AGame.font, text, c, color, layer,  (c+span*0.5f).CToDepth() );
		//}

	}
}






