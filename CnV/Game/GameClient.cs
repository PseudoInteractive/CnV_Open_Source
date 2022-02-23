using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectXTexNet;
using static CnV.AGame;
using static CnV.View;
namespace CnV
{
	using Draw;
	using Microsoft.UI.Xaml;
	using Microsoft.UI.Xaml.Controls;
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Graphics;

	internal partial class GameClient:Microsoft.Xna.Framework.Game
	{
		public static GameClient            instance;
		public static GraphicsDevice        device => instance.GraphicsDevice;
		public static GraphicsDeviceManager _graphics;
		public static CnVSwapChainPanel        canvas;

		public static Mesh tesselatedWorld;



		public static float bmFontScale = 0.125f;
		public static Texture2D fontTexture;

		static readonly Color attackColor = Color.White;
		static Color ShadowColor(float alpha, bool highlight = false) => new Color(highlight ? 16 : 0, highlight ? 0 : 0, highlight ? 0 : 8, (int)(220 * alpha));
		static readonly Color defenseColor = new Color(255, 20, 160, 160);
		static readonly Color defenseArrivedColor = new Color(255, 20, 255, 160);
		static readonly Color artColor = Color.DarkBlue;
		static readonly Color senatorColor = Color.OrangeRed;
		static readonly Color attackingColor = Color.PaleVioletRed;
		static readonly Color siegeColor = Color.DarkOrange;
		static readonly Color assaultColor = CColor(55, 94, 190, 242);
		static readonly Color tradeColor = Color.DarkGreen;
		static readonly Color tradeColorHover = Color.Green;

		static readonly Color tradeColor1      = Color.DarkRed;
		static readonly Color tradeColorHover1 = Color.Red;

		static readonly Color defaultAttackColor = Color.Maroon; // (0xFF8B008B);// Color.DarkMagenta;
		static readonly Color raidColor          = Color.Yellow;
		//        static readonly Color shadowColor = new Color(128, 0, 0, 0);
		static readonly Color      selectColor = new Color(20, 255, 255, 160);
		static readonly Color      buildColor  = Color.DarkRed;
		static readonly Color      hoverColor  = Color.Purple;
		static readonly Color      focusColor  = Color.Maroon;
		static readonly Color      pinnedColor = Color.Teal;
		static readonly Color      black0Alpha = new Color() { A = 0, R = 0, G = 0, B = 0 };
		public static   Material[] troopImages = new Material[Troops.ttCount];
		static          Vector2    troopImageOriginOffset;
		const           int        maxTextLayouts = 1024;

		public static      EffectPass      alphaAddEffect;
		public static      EffectPass      fontEffect;
		public static      EffectPass      darkFontEffect;
		public static      EffectPass      animatedSpriteEffect;
		private static     EffectPass      sdfEffect;
		private static     EffectPass      noTextureEffect;
		private static     EffectPass      worldSpaceEffect;
		public static      EffectParameter planetGainsParamater;
		public static      EffectParameter worldMatrixParameter;
		public static      EffectParameter lightPositionParameter;
		public static      EffectParameter lightPositionCameraParameter;
//		public static      EffectParameter lightGainsParameter;
		public static      EffectParameter lightAmbientParameter;
		public static      EffectParameter lightColorParameter;
		public static      EffectParameter lightSpecularParameter;
		public static      EffectParameter cameraReferencePositionParameter;
		public static      EffectParameter cameraCParameter;
		public static      EffectParameter pixelScaleParameter;
		public static      Material        lineDraw;
		public static      Material        quadTexture;
		public static      Material        whiteMaterial;
		public static      Material        sky;
		public GameClient()
		{
			instance = this;
			_graphics = new GraphicsDeviceManager(this)
			{
					//PreferredBackBufferFormat = SurfaceFormat.Bgra32,
					PreferredBackBufferFormat   = SurfaceFormat.Bgra32,
					PreferMultiSampling         = false,
					PreferredDepthStencilFormat = DepthFormat.None,

					GraphicsProfile =  GraphicsProfile.HiDef
			};
			
			
			_graphics.PreparingDeviceSettings += _graphics_PreparingDeviceSettings;
			Content.RootDirectory             =  "gameBin";
		}

		public static double   dipToNative = 1;
		public static double   nativeToDip = 1;
		//public static Material LoadLitMaterial(string name)
		//{
		//	Texture texture;
		//	Texture normalMap;
		//	using (var scope = new SRGBLoadScope())
		//	{
		//		texture = GameClient.instance.Content.Load<Texture2D>($"{name}");
		//	}
		//	using (var scope = new LinearRGBLoadScope())
		//	{
		//		normalMap = GameClient.instance.Content.Load<Texture2D>($"{ name}_n");
		//	}
		//	return new Material(texture, normalMap, AGame.GetTileEffect());
		//}
		static void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			//	ShellPage.updateHtmlOffsets.SizeChanged();

			SetClientSpan(e.NewSize.Width, e.NewSize.Height);
			//clientCScreen = canvas.TransformToVisual(Helper.CoreContent)
			//	.TransformPoint(new UWindows.Foundation.Point(0, 0)).ToVector2();
			//	canvas.RunOnGameLoopThreadAsync(RemakeRenderTarget);

			//	Log(canvas.CompositionScaleX);



		}

		public static int resolutionDirtyCounter;

		static public void Create(CnVSwapChainPanel swapChainPanel)
		{
			//var sz = swapChainPanel.ActualSize;
			//Assert(sz.X > 0);
			//Assert(sz.Y > 0);
			//AGame.SetClientSpan(sz.X,sz.Y);
			//var display = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
			//var colorInfo = display.GetAdvancedColorInfo();
			//AGame.colorKind = colorInfo.CurrentAdvancedColorKind;
			//display.AdvancedColorInfoChanged+= (a,__) =>
			//{
			//	AGame.colorKind = a.GetAdvancedColorInfo().CurrentAdvancedColorKind;
			//};
			//			canvas.CompositeMode = (Microsoft.UI.Xaml.Media.ElementCompositeMode.SourceOver);
			//canvas.CompositeMode = (Microsoft.UI.Xaml.Media.ElementCompositeMode.MinBlend);
			var _instance = MonoGame.Framework.XamlGame<GameClient>.Create(() => new GameClient() { }, "", AppS.window, swapChainPanel);
			Assert(instance == _instance);
			instance.Deactivated+=Instance_Deactivated;
			instance.GraphicsDevice.DeviceLost+=GraphicsDevice_DeviceLost;
			instance.GraphicsDevice.DeviceReset+=GraphicsDevice_DeviceReset; ;
			instance.GraphicsDevice.DeviceResetting+=GraphicsDevice_DeviceResetting; ;
			canvas.SizeChanged += Canvas_SizeChanged;
			//instance.IsFixedTimeStep                   =  true;

		}

		private static void GraphicsDevice_DeviceResetting(object? sender, EventArgs e)
		{
			Log("GraphicsDevice_DeviceResetting");
		}

		private static void GraphicsDevice_DeviceReset(object? sender, EventArgs e)
		{
			Log("GraphicsDevice_DeviceReset");
		}

		private static void GraphicsDevice_DeviceLost(object? sender, EventArgs e)
		{
			LogEx(new InvalidProgramException("GraphicsDevice_DeviceLost"));
		}

		private static void Instance_Deactivated(object? sender, EventArgs e)
		{
			Log("Deactivated");
		}

		protected override void Initialize()
		{
			Assert( instance == this);
			base.Initialize();
			timer.Start();

		}


		public static int renderFrame;
		private void _graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			var inf = e.GraphicsDeviceInformation;
			inf.GraphicsProfile                             = GraphicsProfile.HiDef;
			inf.PresentationParameters.PresentationInterval = PresentInterval.One;
			//	inf.PresentationParameters.IsFullScreen= true;
			//	inf.PresentationParameters.BackBufferFormat = SurfaceFormat.Bgra32;
			inf.PresentationParameters.SwapChainPanel = canvas;

			inf.PresentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;
			if(clientSpan.X > 0 && clientSpan.Y > 0)
			{
				inf.PresentationParameters.BackBufferHeight = (int)(clientSpan.Y*resolutionScale);
				inf.PresentationParameters.BackBufferWidth  = (int)(clientSpan.X*resolutionScale);
			}
		}
		public static bool wantFastRefresh;

		private static float resolutionScale => Settings.renderQuality switch
												{
														>= 0.625f  => 1.0f,
														>= 0.375f  => 0.875f,
														>= 0.1875f => 0.75f,
														_          => 0.625f
												};


		public static void UpdateResolution()
		{
			resolutionDirtyCounter = wantFastRefresh ? 2 : 60;
		}

		// dx, dy are logical pixels
		// dipToNative is 1 for 100% scaling 1.5 for 150% etc
		// For hiDPI devices we should probably render at lower res
		public static void SetClientSpan(double dx, double dy)
		{
			if(instance is not null)
			{

				dipToNative = instance.GraphicsDevice.PresentationParameters.SwapChainPanel.XamlRoot.RasterizationScale;
				nativeToDip = 1.0 / dipToNative;
				UpdateResolution();

			}
			//clientSpan.X = MathF.Round( (float)((dx* dipToNative+3) / 4))*4.0f;
			//clientSpan.Y = MathF.Round((float)((dy* dipToNative+3) /4))*4.0f;

			// bug:  Not using Dip
			clientSpan.X  = (float)(dx * dipToNative);
			clientSpan.Y  = (float)(dy * dipToNative);
			virtualSpan.X = clientSpan.X        + View.popupLeftMargin;
			virtualSpan.Y = clientSpan.Y        + View.popupTopMargin;
			projectionC.X = clientSpan.X * 0.5f - View.popupLeftMargin * 0.5f;
			projectionC.Y = clientSpan.Y * 0.5f - View.popupTopMargin  * 0.5f;

			clip.c0 = -projectionC;
			clip.c1 = clientSpan -projectionC;

		}
		public static Material CreateFromBytes(byte[] pixels, int x, int y, SurfaceFormat format, EffectPass effect = null)
		{
			if(effect == null)
				effect = AGame.defaultEffect;
			var rv = new Texture2D(instance.GraphicsDevice, x, y, false, format);
			rv.SetData(pixels);

			return new Material(rv, effect);
		}

		static SurfaceFormat GetFormat(DXGI_FORMAT format, bool wantSRGB)
		{
			switch(format) {
				case DXGI_FORMAT.BC1_UNORM: return wantSRGB? SurfaceFormat.Dxt1SRgb : SurfaceFormat.Dxt1;
				case DXGI_FORMAT.BC1_UNORM_SRGB: return SurfaceFormat.Dxt1SRgb;

				case DXGI_FORMAT.BC3_UNORM:return wantSRGB?  SurfaceFormat.Dxt5SRgb :  SurfaceFormat.Dxt5;
				case DXGI_FORMAT.BC3_UNORM_SRGB: return SurfaceFormat.Dxt5SRgb;

				case DXGI_FORMAT.BC4_UNORM: return SurfaceFormat.BC4U;
				case DXGI_FORMAT.BC4_SNORM: return SurfaceFormat.BC4S;

				case DXGI_FORMAT.BC5_UNORM: return SurfaceFormat.BC5U;
				case DXGI_FORMAT.BC5_SNORM: return SurfaceFormat.BC5S;

				case DXGI_FORMAT.BC6H_SF16: return SurfaceFormat.BC6S;
				case DXGI_FORMAT.BC6H_UF16: return SurfaceFormat.BC6U;

				case DXGI_FORMAT.BC7_UNORM: return wantSRGB ?   SurfaceFormat.BC7SRgb  : SurfaceFormat.BC7;
				case DXGI_FORMAT.BC7_UNORM_SRGB: return SurfaceFormat.BC7SRgb;

				default: throw new InvalidDataException($"Unsupported DDS format {format}");
			}

		}
		private static int RoundUpTo4(int v) => (v+3)&(~3);
		private static int RoundDownTo4(int v) => (v+3)&(~3);
		private static bool IsMultipleOf4(int w,int h) => ((h&3)|(w&3)) ==0;//(v+3)&(~3);
		public static Texture CreateFromDDS(string fileName,  bool wantSRGB)
		{
			try
			{

				using var scratch = DirectXTexNet.TexHelper.Instance.LoadFromDDSFile(fileName,DDS_FLAGS.NONE);// DDS_FLAGS.BAD_DXTN_TAILS|DDS_FLAGS.FORCE_DX10_EXT_MISC2|DDS_FLAGS.FORCE_DX10_EXT);
			var meta = scratch.GetMetadata();
//			Log($"{fileName} {meta.Dimension} {meta.Width}x{meta.Height}x{meta.Depth}[{meta.ArraySize}] Mips: {meta.MipLevels} Images: {scratch.GetImageCount()}  Format: {meta.Format} {meta.GetAlphaMode()}");
			SurfaceFormat format = GetFormat(meta.Format,wantSRGB);
			Assert( meta.Depth == 1);
				if(meta.ArraySize > 1 )
				{
					

				
					var arraySize = meta.ArraySize ;
					int width = meta.Width ;
					Assert((width&3) == 0);
					var mips = meta.MipLevels;
					var rv = new Texture2D(instance.GraphicsDevice,RoundUpTo4(meta.Width),RoundUpTo4(meta.Height),mips,format,arraySize);
					var bpp = TexHelper.Instance.BitsPerPixel(meta.Format);

					for(int arraySlice = 0;arraySlice<arraySize;++arraySlice)
					{
						for(int mip	= 0;mip<mips;++mip)
						{
							try
					{
							const int item = 0;
							var image = scratch.GetImage(mip,arraySlice,0);
							
							var iWidth = image.Width;
							rv.SetDataRaw(image.Pixels,
											mip,
											image.RowPitch,iWidth,(image.Height),arraySlice,(int)0);

					}
					catch(Exception ex)
					{
						LogEx(ex);

					}
						}
					}
					return rv;
					


				}
				else
				{
					
					//Not done
					int mips = IsMultipleOf4(meta.Width,meta.Height) ?  meta.MipLevels : 1;
					if(mips==1)
					{
						Log("not multiple of 4");
					}
					var rv = new Texture2D(instance.GraphicsDevice,RoundUpTo4(meta.Width),RoundUpTo4(meta.Height),mips,format,meta.ArraySize);


					for(int arraySlice = 0;arraySlice<meta.ArraySize;++arraySlice)
					{
						for(int m = 0;m<mips;++m)
						{
							const int item = 0;
							var image = scratch.GetImage(m,item,arraySlice);
							rv.SetDataRaw(image.Pixels,m,image.RowPitch,(image.Width),(image.Height),arraySlice,(int)image.SlicePitch);

						}
					}




					return rv;
				}
			}
			catch(Exception ex)
			{
				Log($"Invalid Texture {fileName} {ex}" );
			}
			return null;
		}

		public static bool TryLoadLitMaterialFromDDS(string nameAndPath, out Material main, out Material shadow,bool wantShadow,bool unlit, bool city )
		{
			try
			{ 
				var path = AppS.AppFileName($"{nameAndPath}.dds");
				var pathN = AppS.AppFileName($"{nameAndPath}_n.dds");
			//	var animated = nameAndPath.Contains("animseq");
			//var frames = animated ? animationFrames : 1;
				Texture texture = CreateFromDDS(path,wantSRGB: true);
				Texture normalMap = unlit ? null : CreateFromDDS(pathN,wantSRGB:false);
				if(texture is not null )
				{
					var animated = texture.IsAnimated();
					main = new Material(texture, normalMap, AGame.GetTileEffect(animated,unlit,city:city));
					shadow =wantShadow ?  new Material(texture, GetShadowEffect(animated)) : null;
					return true;
				}
			}
			catch (Exception ex)
			{
				Log($"Missing material: {ex}");
			}
			main = null;
			shadow =null;
			return false;
		}

		public static EffectPass EffectPass(string name)
		{
			return avaEffect.Techniques[name].Passes[0];
		}

		public static Texture2D LoadTexture(string filename)
		{
			return instance.Content.Load<Texture2D>(filename);
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

		//		public static MouseState mouseState;
		//		public static MouseState priorMouseState;
		//	public static KeyboardState keyboardState;
		//		public static KeyboardState priorKeyboardState;



		public static void LoadWorldBackground()
		{
			worldBackground = LoadMaterial($"Art/world{(CnVServer.worldId>=26 ? 26 : ((CnVServer.worldId&1) switch { 1 => "23", _ => "22" }))}");

		}


		protected override async void LoadContent()
		{
			try
			{
				contentStage = ContentStage.loading;
				
			

				avaEffect              = Content.Load<Effect>("Effects/Ava");
				Audio.UpdateMusic();
				defaultEffect = EffectPass("AlphaBlend");
				alphaAddEffect         = EffectPass("AlphaAdd");
				fontEffect             = EffectPass("FontLight");
				darkFontEffect         = EffectPass("FontDark");
				Material.litCityEffect              = EffectPass("LitCity");
				Material.litRegionEffect              = EffectPass("LitRegion");
				Material.litAnimatedEffect              = EffectPass("LitAnimated");
				Material.unlitAnimatedEffect              = EffectPass("UnlitAnimated");
				Material.shadowAnimatedEffect              = EffectPass("ShadowAnimated");
				Material.unlitCityEffect   = EffectPass("UnlitCity");
				Material.unlitRegionEffect   = EffectPass("UnlitRegion");
				Material.shadowEffect   = EffectPass("Shadow");
				animatedSpriteEffect   = EffectPass("SpriteAnim");
				sdfEffect              = EffectPass("SDF");
				noTextureEffect        = EffectPass("NoTexture");
				worldSpaceEffect       = EffectPass("WorldSpace");

				

				using var srgb = new SRGBLoadScope();

				worldMatrixParameter = avaEffect.Parameters["WorldViewProjection"];


				lightPositionParameter = avaEffect.Parameters["lightPosition"];
				lightPositionCameraParameter = avaEffect.Parameters["lightPositionCamera"];
				planetGainsParamater = avaEffect.Parameters["planetGains"];
				//lightGainsParameter = avaEffect.Parameters["lightGains"];
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

				var a = new System.IO.StreamReader((typeof(AGame).Assembly).GetManifestResourceStream("CnV.Content.Fonts.tra.fnt")).ReadToEnd();
				//	using (System.IO.TextReader stream = new System.IO.StreamReader(a))
				{

					bfont.LoadXml(a);
				}

				tesselatedWorldVB = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration, ((World.span + 1) * (World.span + 1)), BufferUsage.None);
				{
					var input = new VertexPositionTexture[(World.span + 1) * (World.span + 1)];
					for(int x = 0; x < World.span + 1; ++x)
					{
						for(int y = 0; y < World.span + 1; ++y)
						{
							var i = new VertexPositionTexture();
							i.Position = new Vector3(x, y, 0.0f);
							i.TextureCoordinate.X = (float)(x+0.5f) / (World.span);
							i.TextureCoordinate.Y = (float)(y + 0.5f) / (World.span);
							input[(World.span + 1) * y + x] = i;
						}
					}
					tesselatedWorldVB.SetData(input);
				}
				tesselatedWorldIB = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits,
					(World.span) * (World.span) * 6, BufferUsage.None);
				{
					var input = new int[(World.span) * (World.span) * 6];
					for(int x = 0; x < World.span; ++x)
					{
						for(int y = 0; y < World.span; ++y)
						{
							input[(x + y * (World.span)) * 6 + 0] = (int)(x + y * (World.span + 1));
							input[(x + y * (World.span)) * 6 + 1] = (int)(x + 1 + y * (World.span + 1));
							input[(x + y * (World.span)) * 6 + 2] = (int)(x + 1 + (y + 1) * (World.span + 1));

							input[(x + y * (World.span)) * 6 + 3] = (int)(x + y * (World.span + 1));
							input[(x + y * (World.span)) * 6 + 4] = (int)(x + 1 + (y + 1) * (World.span + 1));
							input[(x + y * (World.span)) * 6 + 5] = (int)(x + (y + 1) * (World.span + 1));
						}
					}
					tesselatedWorldIB.SetData(input);
				}
				tesselatedWorld = new Mesh();
				tesselatedWorld.vb = tesselatedWorldVB;
				tesselatedWorld.ib = tesselatedWorldIB;
				tesselatedWorld.vertexCount = (World.span + 1) * (World.span + 1);
				tesselatedWorld.triangleCount = (World.span) * (World.span) * 2;



				SpriteAnim.flagHome.Load();
				SpriteAnim.flagSelected.Load();
				SpriteAnim.flagRed.Load();
				SpriteAnim.flagPinned.Load();
				SpriteAnim.flagGrey.Load();

				draw = new CnV.Draw.SpriteBatch(GraphicsDevice);
				// worldBackgroundDark = new TintEffect() { BufferPrecision = CanvasBufferPrecision.Precision8UIntNormalizedSrgb, Source = worldBackground, Color = new Color() { A = 255, R = 128, G = 128, B = 128 } };



				lineDraw = LoadMaterial("Art/linedraw2");
				lineDraw.effect = alphaAddEffect;
				//lineDraw2 = new PixelShaderEffect(
				sky = LoadMaterial("Art/sky");
				//				roundedRect = new Material(Content.Load<Texture2D>("Art/Icons/roundRect"),alphaAddEffect);
				//				quadTexture = new Material(Content.Load<Texture2D>("Art/quad"), sdfEffect);
				quadTexture = new Material(null, sdfEffect);
				whiteMaterial = new Material(null, noTextureEffect);
				for(int i = 0; i < CnV.Troops.ttCount; ++i)
				{

					troopImages[i] = LoadMaterial($"Art/icons/troops{i}");
					if(i == 0)
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
				LoadWorldBackground();
				CityView.LoadContent();

				await TileData.LoadImages();

				contentStage = ContentStage.loaded;


			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
		}

		//public static void UpdateLighting()
		//{
		//	if(TileData.instance is not null && TileData.instance.tilesets is not null)
		//	{
		//		foreach(var tile in TileData.instance.tilesets)
		//		{
		//			tile.material.effect = GetTileEffect();
		//		}
		//	}

		//	CityView.UpdateLighting();
		//}
	}
}
