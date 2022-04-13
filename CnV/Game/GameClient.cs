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

	//	public static Mesh tesselatedWorld;



		public static float baseFontScale = 0.125f;
		public static float regionFontScale = 0.125f;
		public static float fontCullScaleW = 0.125f;
		public static Texture2D fontTexture;

		static readonly Color attackColor = Color.White;
		static Color ShadowColor(float alpha, bool highlight = false) => new Color(highlight ? 16 : 0, highlight ? 0 : 0, highlight ? 0 : 8, (int)(220 * alpha));
		static readonly Color shadowColor =  new Color( 0,0, 8,220);
		static readonly Color returnColor = new Color(60, 30, 200, 255);
		static readonly Color defenseColor = new Color(00, 255, 225, 225);
		static readonly Color defenseArrivedColor = new Color(90, 250, 225, 255);
		static readonly Color artColor = Color.Yellow;
		static readonly Color senatorColor = Color.OrangeRed;
		static readonly Color attackingColor = new Color(r:139,g:0,b:0,alpha:255);
		static readonly Color siegeColor = Color.DarkOrange;
		static readonly Color assaultColor = new Color(r:189,g:20,b:20,alpha:255);
		static readonly Color tradeColor = Color.DarkGreen;
		static readonly Color tradeColorHover = Color.Green;

		static readonly Color tradeColor1      = Color.DarkRed;
		static readonly Color tradeColorHover1 = Color.Red;

		static readonly Color defaultAttackColor = Color.Maroon; // (0xFF8B008B);// Color.DarkMagenta;
		static readonly Color raidColor          = Color.Blue;
		//        static readonly Color shadowColor = new Color(128, 0, 0, 0);
		static readonly Color      selectColor = new Color(20, 255, 255, 160);
		static readonly Color      buildColor  = Color.DarkRed;
		static readonly Color      hoverColor  = Color.Purple;
		static readonly Color      focusColor  = Color.Maroon;
		static readonly Color      pinnedColor = Color.Teal;
		internal static readonly Color      black0Alpha = new Color() { A = 0, R = 0, G = 0, B = 0 };
		public static   Material[] troopImages = new Material[Troops.ttCount];
		public static   Material[] tradeImages = new Material[2]; 
		//internal static          Vector2    troopImageOriginOffset;
		internal const           int        maxTextLayouts = 1024;

		public static      EffectPass      alphaAddEffect;
		public static      EffectPass      fontEffect;
		public static      EffectPass      darkFontEffect;
		public static      EffectPass      animatedSpriteEffect;
		internal static     EffectPass      sdfEffect;
		internal static     EffectPass      noTextureEffect;
		internal static     EffectPass      noTextureShadowEffect;
		internal static     EffectPass      worldSpaceEffect;
//		public static      EffectParameter planetGainsParamater;
		public static      EffectParameter worldMatrixParameter;
		public static EffectParameter viewCWParam;
//		public static      EffectParameter lightPositionParameter;
		public static      EffectParameter lightCCParam;
		public static      EffectParameter lightCWParam;
//		public static      EffectParameter lightGainsParameter;
		public static      EffectParameter lightAmbientParameter;
		public static      EffectParameter lightColorParameter;
		public static      EffectParameter lightSpecularParameter;
	//	public static      EffectParameter cameraReferencePositionParameter;
	//	public static      EffectParameter cameraCParameter;
//		public static      EffectParameter pixelScaleParameter;
		public static      Material        lineDraw;
		public static      Material        quadTexture;
		public static      Material        whiteMaterial;
		public static      Material        shadowMaterial; // no texture
//		public static      Material        sky;
		public static      Material        siege0Material;
		public static      Material        siege1Material;
		public static      Material        attack0Material;
		public static      Material        attack1Material;
		public static      Material        attack2Material;

		public static      Material        defenseMaterial;
		public static      Material        settleMaterial;
		public static      Material        returnMaterial;

		public GameClient()
		{

			Microsoft.Xna.Framework.SharpDXHelper.SetHDR(Settings.hdrMode,Settings.gammaProfile);
			instance = this;
			_graphics = new GraphicsDeviceManager(this)
			{

				PreferredBackBufferFormat = GetBackBufferFormat(),
			//	PreferHalfPixelOffset=false,
				//	PreferredBackBufferFormat   = SurfaceFormat.Rgba1010102,
					PreferMultiSampling         = false,
					PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8,
				

					GraphicsProfile =  GraphicsProfile.HiDef
			};
			
			
			//_graphics.PreparingDeviceSettings += _graphics_PreparingDeviceSettings;
			Content.RootDirectory             =  "gameBin";
		}

		
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
		public static Vector2 canvasSizeDip = new(1,1);
		static void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			//	ShellPage.updateHtmlOffsets.SizeChanged();
			canvasSizeDip = new((float)e.NewSize.Width,(float)e.NewSize.Height);
			UpdateClientSpan();
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
			instance.IsFixedTimeStep                   =  false;

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
		//	Log("Deactivated");
		}

		protected override void Initialize()
		{
			Assert( instance == this);
			base.Initialize();
			timer.Start();

		}


		public static int renderFrame;
		//private void _graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		//{
		//	var inf = e.GraphicsDeviceInformation;
		//	inf.GraphicsProfile                             = GraphicsProfile.HiDef;
		//	inf.PresentationParameters.PresentationInterval = PresentInterval.One;
		//	//	inf.PresentationParameters.IsFullScreen= true;
		//	//	inf.PresentationParameters.BackBufferFormat = SurfaceFormat.Bgra32;
		//	inf.PresentationParameters.SwapChainPanel = canvas;

		//	inf.PresentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;
		//	if(clientSpan.X > 0 && clientSpan.Y > 0)
		//	{
		//		inf.PresentationParameters.BackBufferHeight = (int)(clientSpan.Y*resolutionScale);
		//		inf.PresentationParameters.BackBufferWidth  = (int)(clientSpan.X*resolutionScale);
		//	}
		//}
		public static bool wantFastRefresh;
		public static bool wantDeviceReset = false;

		private static float resolutionScale => Settings.renderQuality switch
												{
														>= 0.625f  => 1.0f,
														>= 0.5f  => 0.875f,
														>= 0.375f => 0.75f,
														>= 0.25f => 0.625f,
														>= 0.125f  => 0.5f,
														_          => 0.25f
												};


		public static void UpdateDevice()
		{
			resolutionDirtyCounter = wantFastRefresh ? 2 : 10;
		}

		// dx, dy are logical pixels
		// dipToNative is 1 for 100% scaling 1.5 for 150% etc
		// For hiDPI devices we should probably render at lower res
		public static void UpdateClientSpan()
		{
			if(instance is not null)
			{

				UpdateDevice();

			}
			
			//clientSpan.X = MathF.Round( (float)((dx* dipToNative+3) / 4))*4.0f;
			//clientSpan.Y = MathF.Round((float)((dy* dipToNative+3) /4))*4.0f;

			// bug:  Not using Dip
			clientSpan.X  = (float)(canvasSizeDip.X );
			clientSpan.Y  = (float)(canvasSizeDip.Y );
			virtualSpan.X = clientSpan.X        + View.popupLeftMargin;
			virtualSpan.Y = clientSpan.Y        + View.popupTopMargin;
			projectionC.X = clientSpan.X * 0.5f - View.popupLeftMargin * 0.5f;
			projectionC.Y = clientSpan.Y * 0.5f - View.popupTopMargin  * 0.5f;

			clip.c0 = default;
			clip.c1 = clientSpan;
			Settings.UpdateXamlConstants();
		}
		public static Material CreateFromBytes<T>(T[] pixels, int x, int y, SurfaceFormat format, EffectPass effect ) where T: struct
		{
			
			var rv = CreateFromBytes(pixels,x,y,format);
			if(effect == null)
				effect = AGame.defaultEffect;
			return new Material(rv, effect);
		}
		public static Texture2D CreateFromBytes<T>(T[] pixels, int x, int y, SurfaceFormat format) where T: struct
		{
			
			var rv = new Texture2D(instance.GraphicsDevice, x, y, false, format);
			rv.SetData(pixels);
			return rv;
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
				case DXGI_FORMAT.A8_UNORM: return SurfaceFormat.Alpha8;
				case DXGI_FORMAT.B8G8R8A8_UNORM: return SurfaceFormat.Bgra32;
				case DXGI_FORMAT.R8G8B8A8_UNORM: return SurfaceFormat.Color;

				default: throw new InvalidDataException($"Unsupported DDS format {format}");
			}

		}
		private static int RoundUpTo4(int v) => (v+3)&(~3);
		private static int RoundDownTo4(int v) => (v)&(~3);
		private static bool IsMultipleOf4(int w,int h) => ((h&3)|(w&3)) ==0;//(v+3)&(~3);
		public static Texture2D CreateFromDDS(string fileName,  bool wantSRGB)
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
											image.RowPitch,iWidth,(image.Height),arraySlice,(int)image.SlicePitch);

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
				//		Log("not multiple of 4");
					}
					var rv = new Texture2D(instance.GraphicsDevice,RoundUpTo4(meta.Width),RoundUpTo4(meta.Height),mips,format,meta.ArraySize);


					for(int arraySlice = 0;arraySlice<meta.ArraySize;++arraySlice)
					{
						for(int m = 0;m<mips;++m)
						{
							var image = scratch.GetImage(m,arraySlice,0);
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

		public static bool TryLoadLitMaterialFromDDS(string nameAndPath, out Material main, out Material shadow,bool wantShadow,bool unlit, bool city, bool opaque=false )
		{
			try
			{ 
				var path = AppS.AppFileName($"{nameAndPath}.dds");
				var pathN = AppS.AppFileName($"{nameAndPath}_n.dds");
			//	var animated = nameAndPath.Contains("animseq");
			//var frames = animated ? animationFrames : 1;
				var texture = CreateFromDDS(path,wantSRGB: true);
				var normalMap = unlit ? null : CreateFromDDS(pathN,wantSRGB:false);
				if(texture is not null )
				{
					var animated = texture.IsAnimated();
					main = new Material(texture, normalMap, AGame.GetTileEffect(animated,unlit,city:city,opaque:opaque));
					
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

		
		public static Texture2D LoadTexture(string filename)
		{
			return instance.Content.Load<Texture2D>(filename);
		}

		public static Material LoadMaterial(string filename)
		{
			var rv = instance.Content.Load<Texture2D>(filename);
			Assert(rv!=null);
			return new Material(rv);
		}
		public static Material LoadMaterialAdditive(string filename)
		{
			var rv = instance.Content.Load<Texture2D>(filename);
			Assert(rv!=null);
			return new Material(rv, alphaAddEffect);
		}

		//		public static MouseState mouseState;
		//		public static MouseState priorMouseState;
		//	public static KeyboardState keyboardState;
		//		public static KeyboardState priorKeyboardState;



		public static void LoadWorldBackground()
		{
			//worldBackground = LoadMaterial($"Art/world{(CnVServer.worldId>=26 ? 26 : ((CnVServer.worldId&1) switch { 1 => "23", _ => "22" }))}");

		}


		protected override void LoadContent()
		{
			View.LoadContent();
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
