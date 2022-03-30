using static CnV.AGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;
using static CnV.View;
using CnV.Draw;

//using Windows.UI.Core;
using static CnV.Troops;
using UWindows = Windows;
using XVector2 = Microsoft.Xna.Framework.Vector2;
using XVector3 = Microsoft.Xna.Framework.Vector3;
using XVector4 = Microsoft.Xna.Framework.Vector4;
using Layer = CnV.Draw.Layer;
using KeyF = CnV.KeyFrame<float>;
using Emzi0767;
namespace CnV;

using static GameClient;

static partial class View

{
	public static async void LoadContent()
	{ 
	try
			{
				contentStage = ContentStage.loading;
				
				var Content = instance.Content;

				avaEffect              = Content.Load<Effect>("Effects/Ava");
				Audio.UpdateMusic();
				defaultEffect = EffectPass("AlphaBlend");
				alphaAddEffect         = EffectPass("AlphaAdd");
				fontEffect             = EffectPass("FontLight");
				darkFontEffect         = EffectPass("FontDark");
				Material.litCityEffect              = EffectPass("LitCity");
				Material.unlitCityEffect   = EffectPass("UnlitCity");

				Material.litCityOpaqueEffect = EffectPassOpaque(Material.litCityEffect, blendAlphaAdd,true);
				Material.unlitCityOpaqueEffect = EffectPassOpaque(Material.unlitCityEffect, blendAlphaAdd,true);
		
				Material.litRegionEffect              = EffectPassOpaque("LitRegion", blendAlphaAdd,false);
				Material.unlitRegionEffect   = EffectPassOpaque("UnlitRegion", blendAlphaAdd,false);
				Material.litRegionOpaqueEffect = EffectPassOpaque(Material.litRegionEffect,blendAlphaAdd,true);
				Material.unlitRegionOpaqueEffect = EffectPassOpaque(Material.unlitRegionEffect,blendAlphaAdd,true);
				
			Material.litAnimatedEffect              = EffectPass("LitAnimated");
				Material.unlitAnimatedEffect              = EffectPass("UnlitAnimated");
				Material.shadowAnimatedEffect              = EffectPass("ShadowAnimated");
				
				Material.shadowEffect   = EffectPass("Shadow");
				World.tileEffect =EffectPassOpaque("LitTile", blendReplace,false);
				World.unlitTileEffect =EffectPassOpaque("UnLitTile",blendReplace,false);
				animatedSpriteEffect   = EffectPass("SpriteAnim");
				sdfEffect              = EffectPass("SDF");
				noTextureEffect        = EffectPass("NoTexture");
				worldSpaceEffect       = EffectPass("WorldSpace");
				noTextureShadowEffect = EffectPass("NoTextureShadow");
		
				var imageLoad =  TileData.LoadImages(); // start loading here
			

				using var srgb = new SRGBLoadScope();

				worldMatrixParameter = avaEffect.Parameters["WorldViewProjection"];
				//worldMatrixParameter = avaEffect.Parameters["WorldViewProjection"];


			//	lightPositionParameter = avaEffect.Parameters["lightPosition"];
				lightCCParam = avaEffect.Parameters["lightCC"];
				lightCWParam = avaEffect.Parameters["lightCW"];
				//planetGainsParamater = avaEffect.Parameters["planetGains"];
				//lightGainsParameter = avaEffect.Parameters["lightGains"];
				lightColorParameter = avaEffect.Parameters["lightColor"];
				lightSpecularParameter = avaEffect.Parameters["lightSpecular"];
				lightAmbientParameter = avaEffect.Parameters["lightAmbient"];
			//	cameraReferencePositionParameter = avaEffect.Parameters["cameraReferencePosition"];
				viewCWParam = avaEffect.Parameters["viewCW"];
			//pixelScaleParameter = avaEffect.Parameters["pixelScale"];

				fontTexture = CreateFromDDS(AppS.AppFileName("runtime/font.dds"),false);

				fontMaterial = new Material(fontTexture,fontEffect);
			//	darkFontMaterial = new Material(fontTexture, darkFontEffect);


				fontMaterial.effect = fontEffect;

				bfont = new BitmapFont.BitmapFont();

				var a = new System.IO.StreamReader((typeof(AGame).Assembly).GetManifestResourceStream("CnV.Content.Fonts.tra.fnt")).ReadToEnd();
				//	using (System.IO.TextReader stream = new System.IO.StreamReader(a))
				{

					bfont.LoadXml(a);
				}

			//	tesselatedWorldVB = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration, ((World.span + 1) * (World.span + 1)), BufferUsage.None);
			//	{
			//		var input = new VertexPositionTexture[(World.span + 1) * (World.span + 1)];
			//		for(int x = 0; x < World.span + 1; ++x)
			//		{
			//			for(int y = 0; y < World.span + 1; ++y)
			//			{
			//				var i = new VertexPositionTexture();
			//				i.Position = new Vector3(x, y, 0.0f);
			//				i.TextureCoordinate.X = (float)(x+0.5f) / (World.span);
			//				i.TextureCoordinate.Y = (float)(y + 0.5f) / (World.span);
			//				input[(World.span + 1) * y + x] = i;
			//			}
			//		}
			//		tesselatedWorldVB.SetData(input);
			//	}
			//	tesselatedWorldIB = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits,
			//		(World.span) * (World.span) * 6, BufferUsage.None);
			//	{
			//		var input = new int[(World.span) * (World.span) * 6];
			//		for(int x = 0; x < World.span; ++x)
			//		{
			//			for(int y = 0; y < World.span; ++y)
			//			{
			//				input[(x + y * (World.span)) * 6 + 0] = (int)(x + y * (World.span + 1));
			//				input[(x + y * (World.span)) * 6 + 1] = (int)(x + 1 + y * (World.span + 1));
			//				input[(x + y * (World.span)) * 6 + 2] = (int)(x + 1 + (y + 1) * (World.span + 1));

			//				input[(x + y * (World.span)) * 6 + 3] = (int)(x + y * (World.span + 1));
			//				input[(x + y * (World.span)) * 6 + 4] = (int)(x + 1 + (y + 1) * (World.span + 1));
			//				input[(x + y * (World.span)) * 6 + 5] = (int)(x + (y + 1) * (World.span + 1));
			//			}
			//		}
			//		tesselatedWorldIB.SetData(input);
			//	}
			//	tesselatedWorld = new Mesh();
			//	tesselatedWorld.vb = tesselatedWorldVB;
			//	tesselatedWorld.ib = tesselatedWorldIB;
			////	tesselatedWorld.vertexCount = (World.span + 1) * (World.span + 1);
			//	tesselatedWorld.triangleCount = (World.span) * (World.span) * 2;



				SpriteAnim.flagHome.Load();
				SpriteAnim.flagSelected.Load();
				SpriteAnim.flagRed.Load();
				SpriteAnim.flagPinned.Load();
				SpriteAnim.flagGrey.Load();

				draw = new CnV.Draw.SpriteBatch(instance.GraphicsDevice);
				// worldBackgroundDark = new TintEffect() { BufferPrecision = CanvasBufferPrecision.Precision8UIntNormalizedSrgb, Source = worldBackground, Color = new Color() { A = 255, R = 128, G = 128, B = 128 } };



				lineDraw = LoadMaterial("Art/linedraw2");
				lineDraw.effect = alphaAddEffect;
				//lineDraw2 = new PixelShaderEffect(
				//sky = LoadMaterial("Art/sky");
				//				roundedRect = new Material(Content.Load<Texture2D>("Art/Icons/roundRect"),alphaAddEffect);
				//				quadTexture = new Material(Content.Load<Texture2D>("Art/quad"), sdfEffect);
				quadTexture = new Material(null, sdfEffect);
				whiteMaterial = new Material(null, noTextureEffect);
				shadowMaterial =new Material(null,noTextureEffect); // todo
			for(int i = 0; i < CnV.Troops.ttCount; ++i)
				{

					troopImages[i] = LoadMaterial($"Art/icons/troops{i}");
					//if(i == 0)
					//{
					//	troopImageOriginOffset.X = (float)troopImages[i].Width * 0.5f;
					//	troopImageOriginOffset.Y = (float)troopImages[i].Height * 0.5f;
					//}
				}
				for(int i = 0;i < tradeImages.Length;++i)
				{
					tradeImages[i] = LoadMaterial($"Art/icons/trade{i}");
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

				await imageLoad;
				


				contentStage = ContentStage.loaded;


			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
}

	internal static DepthStencilState depthWrite = new() { DepthBufferEnable=false, DepthBufferFunction=CompareFunction.Always };
	internal static DepthStencilState depthRead = new() { DepthBufferEnable=true,DepthBufferFunction=CompareFunction.LessEqual,DepthBufferWriteEnable=false  };
	internal static DepthStencilState depthOpaque = new() { DepthBufferEnable=true,DepthBufferFunction=CompareFunction.LessEqual,DepthBufferWriteEnable=true  };

	internal static RasterizerState rasterizationState = new() { CullMode = CullMode.None,DepthClipEnable=false,MultiSampleAntiAlias=false };

	internal static BlendState blendReplace = BlendState.Opaque;
	internal static BlendState blendAlphaAdd = BlendState.AlphaBlend;
	// If we want to different z modes for an effect we have to clone it
	public static EffectPass EffectPass(string name, BlendState blend,DepthStencilState depth)
		{
			var rv = avaEffect.Techniques[name].Passes[0];
			rv._blendState=blend;
			rv._depthStencilState = depth;
			return rv;
		}

	public static EffectPass EffectPass(EffectPass basedOn, BlendState blend,DepthStencilState depth)
		{
			return  new EffectPass(basedOn._effect,basedOn,blend,depth);
		}
		public static EffectPass EffectPassOpaque(string name,BlendState blendState, bool writeZ) => EffectPass(name,blendState,writeZ ? depthOpaque:depthRead);
		public static EffectPass EffectPassOpaque(EffectPass basedOn, BlendState blendState, bool writeZ) => EffectPass(basedOn,blendState,writeZ ? depthOpaque:depthRead);
		public static EffectPass EffectPassAlpha(string name) => EffectPass(name,blendAlphaAdd,depthRead);
		public static EffectPass EffectPass(string name) =>EffectPassAlpha(name);
	
}
