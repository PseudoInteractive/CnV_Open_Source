using static CnV.AGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;
using static CnV.View;

//using Windows.UI.Core;
using XVector3 = Microsoft.Xna.Framework.Vector3;
using Layer = CnV.Draw.Layer;
using KeyF = CnV.KeyFrame<float>;

namespace CnV;
using static Troops;
internal partial class GameClient
{
	public static TextFormat textformatLabel = new TextFormat(TextFormat.HorizontalAlignment.center,TextFormat.VerticalAlignment.center);
	private TextFormat tipTextFormatCentered = new TextFormat(TextFormat.HorizontalAlignment.center);
	private TextFormat tipTextFormatCenteredBottom = new TextFormat(TextFormat.HorizontalAlignment.center,TextFormat.VerticalAlignment.bottom);
	private TextFormat tipTextFormat = new TextFormat(TextFormat.HorizontalAlignment.left);
	private TextFormat tipTextFormatRight = new TextFormat(TextFormat.HorizontalAlignment.right);
	private TextFormat nameTextFormat = new TextFormat(TextFormat.HorizontalAlignment.center,TextFormat.VerticalAlignment.center);
	const byte textBackgroundOpacity = 192;
	static Color diffuseTint = new(0.75f,0.75f,.75f,1.0f);
	//	public static Material worldBackground;
	//public static Effect imageEffect;
	public static Effect avaEffect;
	public static Span2i[] popups = Array.Empty<Span2i>();

	//    public static TintEffect worldBackgroundDark;
	public static Material worldObjects;
	public static Material worldOwners;

	//	public static VertexBuffer tesselatedWorldVB;
	//	public static IndexBuffer tesselatedWorldIB;
	static KeyF[] bulgeKeys = new[] { new KeyF(0.0f,0.0f),new KeyF(0.44f,0.44f),new KeyF(1.5f,0.44f),new KeyF(2.5f,0.0f) };

	private static bool TilesReady() {
		return (TileData.state >= TileData.State.ready);
	}

	private const int textBoxCullSlop = 80;
	//	static byte clearCounter = 10;

	//public static bool tileSetsPending;
	private static float smallRectSpan => 4f.ScreenToWorld();
	public const float lightZNight = 200;
	public const float lightZDay = 20f;
	public const float cameraZForLighting = 0.5f;
	//public static Vector2 cameraLightC;
	static SamplerState fontSampler = SamplerState.LinearBorder;
	//{
	//	Name = "Font",
	//	Filter                  = TextureFilter.LinearMipPoint,
	//	MipMapLevelOfDetailBias = 0,
	//	BorderColor             = new Color(0,0,0,0),
	//	AddressW                = TextureAddressMode.Border,
	//	AddressU                = TextureAddressMode.Border,
	//	AddressV                = TextureAddressMode.Border,

	//};
	//static SamplerState borderSampler = new SamplerState()
	//{
	//	Name = "border",

	//	Filter                  = TextureFilter.LinearMipPoint,
	//	MipMapLevelOfDetailBias = 0,
	//	MaxAnisotropy           = 0,
	//	AddressW                = TextureAddressMode.Wrap,
	//	AddressU                = TextureAddressMode.Border,
	//	AddressV                = TextureAddressMode.Border,
	//};
	//static SamplerState clampSampler = new SamplerState()
	//{
	//	Filter                  = TextureFilter.LinearMipPoint,
	//	MipMapLevelOfDetailBias = 0,
	//	MaxAnisotropy           = 0,
	//	AddressW                = TextureAddressMode.Wrap,
	//	AddressU                = TextureAddressMode.Clamp,
	//	AddressV                = TextureAddressMode.Clamp,
	//};
	//static SamplerState wrapSampler = new SamplerState()
	//{
	//	Filter                  = TextureFilter.LinearMipPoint,
	//	MipMapLevelOfDetailBias = 0,
	//	MaxAnisotropy           = 0,
	//	AddressW                = TextureAddressMode.Wrap,
	//	AddressU                = TextureAddressMode.Wrap,
	//	AddressV                = TextureAddressMode.Wrap,
	//};
	//static SamplerState fontSampler = new SamplerState()
	//{
	//	Name = "Font",
	//	Filter                  = TextureFilter.LinearMipPoint,
	//	MipMapLevelOfDetailBias = 0,
	//	BorderColor             = new Color(0,0,0,0),
	//	AddressW                = TextureAddressMode.Border,
	//	AddressU                = TextureAddressMode.Border,
	//	AddressV                = TextureAddressMode.Border,

	//};
	static SamplerState borderSampler = SamplerState.LinearBorder;
	static SamplerState clampSampler = SamplerState.LinearClamp;
	static SamplerState wrapSampler = SamplerState.LinearWrap;

	static SamplerState normalSampler = new SamplerState() {
		Filter                  = TextureFilter.LinearMipPoint,
		MipMapLevelOfDetailBias = 0,
		MaxAnisotropy           = 0,
		BorderColor             = new Color(128,128,148,0),
		AddressU                = TextureAddressMode.Border,
		AddressV                = TextureAddressMode.Border,
		AddressW                = TextureAddressMode.Border,
	};

	public static Material fontMaterial;
	//public static Material darkFontMaterial;
	public static BitmapFont.BitmapFont bfont;


	const float actionAnimationGain = 64.0f;
	const float drawActionLength = 32;
	const float lineAnimationRate = 0.5f;



	const float postAttackDisplayTime = 15 * 60; // 11 min

	const float lineThickness = 4.5f;
	const float circleRadMin = 3.0f;
	const float circleRadMax = 5.5f;
	static Vector2 shadowOffset = new Vector2(4,4);
	const float detailsZoomThreshold = 18;
	const float detailsZoomFade = 8;

	//	static float LineThickness(bool hovered) => hovered ? lineThickness * 2 : lineThickness;
	const float rectSpanMin = 4.0f;
	const float rectSpanMax = 8.0f;
	const float bSizeGain = 1.0f;
	const float bSizeGain2 = 1; //4.22166666666667f;
	const float srcImageSpan = 2400;
	const float bSizeGain3 = bSizeGain * bSizeGain / bSizeGain2;


	public static float circleRadiusBase = 1.0f;
	public static float shapeSizeGain = 1.0f;
	public static float spriteSizeGain = 1;
	public static float bulgeSpan => 1.0f + bulgeNegativeRange;
	public static float bulgeGain = 0;
	public static float lineWToUs = 1;
	public static int debugLayer = -1;
	//	const float dashLength = (dashD0 + dashD1) * lineThickness;
	public static Draw.SpriteBatch draw;

	static IToolTip underMouse; // could also be a trade?
	static float bestUnderMouseScore;
	//   public static Vector2 cameraMid;
	public static float eventTimeOffsetLag; // smoothed version of event time offset
											//public static float eventTimeEnd;
	static public Color nameColor, nameColorHover, myNameColor, nameColorOutgoing, nameColorIncoming, nameColorSieged,
			nameColorIncomingHover, nameColorSiegedHover, myNameColorIncoming, myNameColorSieged,
		nameColorDungeon;
	//static float specularGain = 0.5f;
	static long ticksAtDraw;
	public static Matrix projection;
	internal static double timeSinceLastFrame;
	internal const float targetStepsPerSecond = 128;
	public static int drawCounter;
	public static float fadeCounter = 1;
	internal static HashSet<int> drawTradesHash = new();
	internal static HashSet<int> drawArmyHash = new();
	internal static HashSet<int> targetOpponents = new();
	internal static Dictionary<SpotId,int> cityVisitorCounts = new();

	protected override void Draw(GameTime gameTime) {
		if(faulted)
			return;
		if(!AppS.isForeground)
			return;
		++renderFrame;
		var priorUnderMouse = underMouse;
		underMouse = null;
		bestUnderMouseScore = 0.125f;
		++drawCounter;
		simT = Sim.NowToServerSeconds(); ;
#if DEBUG
		if(gameTime.IsRunningSlowly) {
			if(ToolTips.debugTip is null)
				ToolTips.debugTip = "Slow";
			else
				ToolTips.debugTip += "\nSlow";

		}
		if(ToolTips.debugTip == null)
			ToolTips.debugTip = $"{(timeSinceLastFrame*1000).RoundToInt()}ms";
#endif

		//parallaxZ0 = 1024 * 64.0f / cameraZoomLag;
		var isFocused = Sim.isInitialized&&AppS.isForeground;

		try {
			var elapsed = timer.Elapsed;
			var timerT = elapsed.TotalSeconds;
			var priorTicks = ticksAtDraw;
			ticksAtDraw = elapsed.Ticks;
			timeSinceLastFrame = (ticksAtDraw-priorTicks)/(double)TimeSpan.TicksPerSecond;

			if(timeSinceLastFrame > 0.25) {
#if DEBUG
				///		Note.Show($"\nVery Slow {timeSinceLastFrame}");
#endif
				timeSinceLastFrame  = 0.25;
			}
			View.StepViewToPresent();
			//cameraZoomLag += (cameraZoom - cameraZoomLag) * gain;
			//cameraLightC = (ShellPage.mousePositionC);
			//                cameraZoomLag += (cameraZoom
			// smooth ease towards target

			var serverNow = Sim.simTime + TimeSpanS.FromMinutes(eventTimeOffsetLag);

			// not too high or we lose float precision
			// not too low or people will see when when wraps
			animationT = timerT;// ((uint)Environment.TickCount % 0xffffff) * (1.0f / 1000.0f);
			wantParallax = Settings.parallax > 0.1f;

			//{
			//	var i = (int)(animationT / 4.0f);
			//	var stretchCount = FontStretch.UltraExpanded - FontStretch.Condensed+1;
			//	fontStretch = FontStretch.Condensed + (i % stretchCount);
			//	tipTextFormat.FontStretch = fontStretch;
			//	tipTextFormatCentered.FontStretch = fontStretch;
			//}
			//animationTWrap = ().Frac()); // wraps every 3 seconds, 0..1

			device.Textures[7] = fontTexture;

			device.Textures[8] = TileData.landTileset.material.texture;
			device.Textures[9] = TileData.landTileset.material.texture1;

			device.Textures[10] = TileData.waterTileset.material.texture;
			device.Textures[11] = TileData.waterTileset.material.texture1;

			device.Textures[12] = TileData.terrainTileset.material.texture;
			device.Textures[13] = TileData.terrainTileset.material.texture1;

			device.Textures[2] = TileData.cityTileset.material.texture;
			device.Textures[3] = TileData.cityTileset.material.texture1;

			//		device.Textures[4] = TileData.topLevelTileset.material.texture;
			//		device.Textures[5] = TileData.topLevelTileset.material.texture1;
			//				float accentAngle = animT * MathF.PI * 2;
			var tick = (elapsed.TotalSeconds);
			var animTLoop = (animationT*(1.0/3.0)).Wave();
			int cx0 = 0, cy0 = 0, cx1 = 0, cy1 = 0;
			var rectSpan = animTLoop.Lerp(rectSpanMin,rectSpanMax);


			//   ShellPage.T("Draw");

			//	defaultStrokeStyle.DashOffset = (1 - animT) * dashLength;


			//                ds.Blend = ( (int)(serverNow.Second / 15) switch { 0 => CanvasBlend.Add, 1 => CanvasBlend.Copy, 2 => CanvasBlend.Add, _ => CanvasBlend.SourceOver } );



			float xyScale = 1.0f;
			var xyGain = xyScale;
			float projectionOffsetGainX = xyGain/canvasSizeDip.X;
			float projectionOffsetGainY = xyGain/canvasSizeDip.Y;

			//ds.TextRenderingParameters = new CanvasTextRenderingParameters(!AppS.IsKeyPressedControl() ? CanvasTextRenderingMode.Outline : CanvasTextRenderingMode.Default, CanvasTextGridFit.Default);

			//              ds.TextRenderingParameters = new CanvasTextRenderingParameters(CanvasTextRenderingMode.Default, CanvasTextGridFit.Disable);
			// var scale = ShellPage.canvas.ConvertPixelsToDips(1);
			pixelScaleInverse = 1.0f*projectionOffsetGainY*viewW.Z;
			pixelScale       = 1.0f/pixelScaleInverse;

			dipScaleInverse = (float)(pixelScaleInverse*dipToNative*Settings.dpiAdjust);
			//	dipScale       = 1.0f/dipScaleInverse;

			//halfSquareOffset = new System.Numerics.Vector2(pixelScale * 0.5f, pixelScale * .5f);
			var bonusLayerScale = (2 * Settings.iconScale)*MathF.Sqrt(pixelScale / 64.0f).DipToWorld();

			baseFontScale = (Settings.fontScale*0.75f).DipToWorld();//.Min(0.5f);
			regionFontScale =  MathF.Sqrt(64 / viewW.Z) *Settings.regionLabelScale* baseFontScale;//.Min(0.5f);
			fontCullScaleW = (Settings.fontCullScale*0.5f).DipToWorld();

			lineWToUs = 9.0f;// (64.0f).DipToWorld();//.Min(4.0f);
			shapeSizeGain = MathF.Sqrt(pixelScale * (1.50f / 64.0f)).DipToWorld();
			spriteSizeGain = 32 * Settings.iconScale*shapeSizeGain;
			var deltaZoom = viewZoomLag - detailsZoomThreshold;
			var wantDetails = deltaZoom > 0 && regionFontScale > fontCullScaleW;
			var wantImage = deltaZoom < detailsZoomFade;
			var deltaZoomCity = viewZoomLag - cityZoomThreshold;
			var wantCity = deltaZoomCity >= 0;
			var cityAlpha = (deltaZoomCity / 128.0f).Saturate().Sqrt(); // blend in over 128m.  We use the sqrt to shape it a little
																		// in rangeshould we taket the sqrt()?
			var cityAlphaI = (int)(cityAlpha * 255.0f);

			//var wantFade = wantImage; // world to region fade
			var regionFade = (deltaZoom / detailsZoomFade).Saturate(); // 0 means world view, 1 measns region view
			var regionAlpha = 1;// wantFade ? (deltaZoom / detailsZoomFade).Saturate().Sqrt() : 1.0f;
			var intAlpha = byte.MaxValue;
			var nameAlpha = (byte)162;
			var zoomT = viewZoomLag / detailsZoomThreshold;
			bulgeGain = (((MathF.Log(MathF.Max(1.0f,zoomT)))));
			bulgeGain = bulgeGain.Min(0.4f);// Eval(bulgeGain);


			bulgeGain *= Settings.planet * (1.0f - cityAlpha);
			float bulgeInputSpan2 = (virtualSpan.X.Squared() + virtualSpan.Y.Squared());
			bulgeInputGain = 4*(0.75f.Squared()) / bulgeInputSpan2;


			//var gr = spriteBatch;// spriteBatch;// wantLight ? renderTarget.CreateDrawingSession() : args.DrawingSession;

			//		ds.Blend = CanvasBlend.Copy;

			// funky logic
			//if (wantLight)
			//	if(--clearCounter > 0)
			{
				GraphicsDevice.Clear(new Color(0,0,0,0)); // black transparent
			}

			//								   //ds.TextAntialiasing = canvasTextAntialiasing;
			//								   //ds.TextRenderingParameters = canvasTextRenderingParameters;
			//								   // prevent MSAA gaps
			GraphicsDevice.BlendState = BlendState.AlphaBlend;
			//	GraphicsDevice.DepthStencilState = DepthStencilState.None;

			//			GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
			GraphicsDevice.SamplerStates[0] = wrapSampler;
			GraphicsDevice.SamplerStates[1] = clampSampler;
			//			GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
			GraphicsDevice.SamplerStates[6] = borderSampler;
			GraphicsDevice.SamplerStates[5] = normalSampler;
			GraphicsDevice.SamplerStates[7] = fontSampler;

			GraphicsDevice.RasterizerState = rasterizationState;
			GraphicsDevice.DepthStencilState = depthWrite;
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
			//if(AppS.IsKeyPressedShift())
			//	pitch += (float)timeSinceLastFrame * (AppS.IsKeyPressedControl()  ? 0.25f : -0.25f);

			{
				var viewport = GraphicsDevice.Viewport;

				//var cameraZ = cameraZLag;

				//viewCWParam.SetValue(new XVector3(viewW.X,viewW.Y,viewW.Z)); // Z of camera is always 1
				//	pixelScaleParameter.SetValue(pixelScale);

				//var projCX = projectionC.X / clientSpan.X.Max(1);
				//var projCY = projectionC.Y / clientSpan.Y.Max(1);




				var m0 = Matrix.CreateLookAt(
								new XVector3(viewW.X,viewW.Y,viewW.Z),
								new XVector3(viewW.X,viewW.Y,0),
								Microsoft.Xna.Framework.Vector3.Up);
				//var mR = Matrix.CreateRotationX(pitch);
				const float nearZ = 0.125f;
				const float farZ = nearZ +viewMaxZ;
				var nearGain = nearZ*projectionOffsetGainY;

				var m1 = Matrix.CreatePerspectiveOffCenter(
							  (-projectionC.X)*nearGain,
							  (canvasSizeDip.X-projectionC.X)*nearGain,

							  (canvasSizeDip.Y-projectionC.Y)*nearGain,
							   -projectionC.Y*nearGain,

							   nearZ,farZ);

				projection = m0*m1;
				worldMatrixParameter.SetValue(projection);
				//var lightCC = new XVector3(AGame.projectionC.X,AGame.projectionC.Y,virtualSpan.X*cameraZForLighting);
				//lightCCParam.SetValue(lightCC);
				//lightCWParam.SetValue(lightCC);
				//			if(Settings.lighting == Lighting.night)
				//			{
				////				ToolTips.debugTip = $"{ShellPage.mousePosition} {lightZNight*pixelScale}";
				//				var l = ShellPage.mousePosition;//C.CameraToScreen();//.InverseProject();
				//				var lc = ShellPage.mousePositionC;//C.CameraToScreen();//.InverseProject();
				//				lightPositionParameter.SetValue(new Microsoft.Xna.Framework.Vector3(l.X, l.Y, lightZNight));
				//			//	lightPositionCameraParameter.SetValue(new Microsoft.Xna.Framework.Vector3(lc.X, lc.Y, lightZNight));
				//				//lightGainsParameter.SetValue(new Microsoft.Xna.Framework.Vector4(0.25f, 1.25f, 0.375f, 1.0625f));
				//				lightAmbientParameter.SetValue(new Vector4(new Vector3(.493f, .576f, .639f) * 0.25f*Settings.lightA,Settings.lightM));
				//				lightColorParameter.SetValue(new Vector4( new Vector3(1.0f, 1.0f, 1.0f) * 1.25f*Settings.lightD,Settings.lightS));
				//				//lightSpecularParameter.SetValue(new Microsoft.Xna.Framework.Vector3(1.0f, 1.0f, 1.0f) * 1.25f*specularGain);
				//			}
				//			else
				{
					///				var xc = lightWC.WorldToCamera().CameraToScreen();
					var t = (float)Sim.simDateTime.TimeOfDay.TotalDays;

					float shrink = 1.0f/(64*4)*viewW.Z;

					if((Settings.lighting == Lighting.local))
						t = (float)DateTimeOffset.Now.TimeOfDay.TotalDays; // day t= night
					else if((Settings.lighting == Lighting.strobe))
						t = t*256;
					//					else
					//						t = t*64;

					t -= MathF.Floor(t);
					//	t = t.Bezier(0f,0.3f,0.43f,0.57f,0.70f,1.0f);
					t = t.Bezier(0f,0.375f,0.625f,1.0f);
					//	t = t.Bezier(0f,0.5f,0.5f,1.0f);
					var isDay = (t >= 0.25f) & (t <= 0.75f);
					var t1 = (t-0.25f); // 0..1 is Morning to evening, -1..1 is evening until morning 
					t1 -= t1.Floor();
					//Assert( t1 >= -1.0f);
					//Assert(t1 <= 1.0f);
					const float worldSpan0 = World.span * -0.25f;
					const float worldSpan1 = World.span *  1.25f;
					var csTau = MathF.Cos(t*MathF.Tau);
					Vector2 wc = new Vector2(MathF.Sin(t*MathF.Tau).SNormLerp(worldSpan0,worldSpan1),
						(csTau).SNormLerp(worldSpan0,worldSpan1));

					var Z = (csTau * (isDay ? -1.0f : 0.75f) + 1.0f)*World.span;
					var cc = (wc.WorldToCamera()*shrink);
					//var sc = cc.CameraToScreen();
					Assert(Z> 0);
					var lightZ = Z*shrink;
					var lightC = new Vector3(cc.X,cc.Y,lightZ);
					lightCCParam.SetValue(lightC);
					lightCWParam.SetValue(lightC.CameraToWorld());
					viewCWParam.SetValue(viewW);
					var waterGain = regionFade.Lerp(0.5f,1.0f);
					var terrainGain = regionFade.Lerp(0.25f,1.0f);
					tileGains.SetValue(new Vector4(terrainGain,waterGain,terrainGain,0.5f));
					//	ToolTips.debugTip = $"{XVector3.Normalize(lightCC).ToNumerics().Format()} {AUtil.Format(lightCC.ToNumerics())}";
					var sat = Settings.lightSat;
					// Hue night
					var c0 = new Vector3(5.0f/6.0f,sat*0.75f,0.625f);
					// Morning
					var c1 = new Vector3(4.0f/6.0f,sat,0.75f);
					// noon
					var c2 = new Vector3(2.0f/6.0f,sat*0.375f,0.875f);
					// evening
					var c3 = new Vector3(0.0f,sat,0.75f);
					var cp = new Vector3(c3.X+1.0f,c3.Y,c3.Z);
					var c4 = new Vector3(c0.X-1.0f,c0.Y,c0.Z);
					var c5 = new Vector3(c1.X-1.0f,c1.Y,c1.Z);
					var c_ = t.CatmullRom(cp,c0,c1,c2,c3,c4,c5);
					var d3 = new Vector3(c_.X,c_.Y,c_.Z*Settings.lightD).RGBFromHSL();
					var a3 = new Vector3(c_.X,c_.Y,c_.Z*Settings.lightA).RGBFromHSL();
					var s3 = new Vector3(c_.X,c_.Y*0.5f,c_.Z.Lerp(0.25f,0.75f)*Settings.lightS).RGBFromHSL();

					diffuseTint = HSLToRGB.ToRGBA(c_.X,c_.Y,0.75f,0.75f);
					//var d3 = t.CatmullRomLoop(new Vector3(0.75f,0.5f,1.125f),
					//							new Vector3(0.5f,0.5f,1.5f),
					//							new Vector3(1.0f,1.0f,1.0f),
					//							new Vector3(1.25f,0.65f,0.35f)
					//													)*0.75f * Settings.lightD;
					//var a3 = t.CatmullRomLoop(new Vector3(0.3750f,0.125f,0.375f),
					//							new Vector3(0.5f,0.5f,1.25f),
					//							new Vector3(1.0f,1.0f,1.0f),
					//							new Vector3(1.25f,0.5f,0.25f)
					//													)*0.375f* Settings.lightA;
					// this will do for specular


					//var s3 = 0.375f.Lerp(d3.Normalized(),new Vector3(0.5f,0.5f,0.5f))*(t.CatmullRomLoop(1.125f,1.0f,0.875f,1.0f)* Settings.lightS);

					//var hue = 0.6667f - t1;
					//hue -= hue.Floor();
					//var lumd = t.CatmullRomLoop(0.75f,0.75f,1.0f,0.75f);
					//var luma = t.CatmullRomLoop(0.125f,0.25f,0.5f,0.25f);
					//var saturation = t.CatmullRomLoop(0.25f,0.5f,0.0f,0.55f);
					//var diffuse = HSLToRGB.ToRGBAV(hue,saturation,lumd,alpha:1.25f);
					//var ambient = HSLToRGB.ToRGBAV(hue,saturation,luma,alpha:0.75f);


					//lightGainsParameter.SetValue(new XVector4(0.25f, 1.25f, 0.375f, 1.0625f));
					//lightAmbientParameter.SetValue(new XVector4(.483f, .476f, .549f, 1f) * 0.75f);
					//lightColorParameter.SetValue(new XVector4(1.1f, 1.1f, 0.9f, 1f) * 1.25f);
					//lightSpecularParameter.SetValue(new XVector4(1.0f, 1.0f, 1.0f, 1.0f) * 1.25f);
					//	lightGainsParameter.SetValue(new XVector4(0.25f, 1.25f, 0.375f, 1.0625f));
					lightAmbientParameter.SetValue(a3);
					lightColorParameter.SetValue(d3);
					lightSpecularParameter.SetValue(new Vector4(s3,Settings.lightM));
				}
				//				var s3 = new Vector4(Settings.lightSR,Settings.lightSG,Settings.lightSB,Settings.lightShader);//0.5f.Lerp(d3.Normalized(),new Vector3(0.75f,0.75f,0.75f) );



				//cameraReferencePositionParameter.SetValue(new XVector3(projectionC.X, projectionC.Y,1));
				//					defaultEffect.Parameters["DiffuseColor"].SetValue(new Microsoft.Xna.Framework.Vector4(1, 1, 1, 1));
				var gain1 = bulgeInputGain * bulgeGain * bulgeSpan;
				var planetGains = new XVector3(bulgeGain,-gain1,!AppS.IsKeyPressedShift() ? 0.0f : gain1*bulgeInputSpan2.Sqrt());
				//planetGainsParamater.SetValue(planetGains);

				// Update constant buffer
				Assert(avaEffect.ConstantBuffers.Length == 1);
				{
					var cb = avaEffect.ConstantBuffers[0];
					cb.Update(avaEffect.Parameters);
					var device = GameClient.instance.GraphicsDevice;
					device.SetConstantBuffer(ShaderStage.Pixel,0,cb);
					device.SetConstantBuffer(ShaderStage.Vertex,0,cb);
					device.SetConstantBuffer(ShaderStage.Geometry,0,cb);

				}
			}
			//world space coords
			//  var srcP0 = new System.Numerics.Vector2((viewW.X + 0.5f) * bSizeGain2 - projectionC.X * bSizeGain2 * pixelScaleInverse,
			//									   (viewW.Y + 0.5f) * bSizeGain2 - projectionC.Y * bSizeGain2 * pixelScaleInverse);
			//var srcP1 = new System.Numerics.Vector2(srcP0.X + clientSpan.X * bSizeGain2 * pixelScaleInverse,
			//										srcP0.Y + clientSpan.Y * bSizeGain2 * pixelScaleInverse);
			//var destP0 = clip.c0;
			//var destP1 = clip.c1;

			//if(srcP0.X < 0)
			//{
			//	destP0.X -= srcP0.X * pixelScale / bSizeGain2;
			//	srcP0.X = 0;
			//}
			//if(srcP0.Y < 0)
			//{
			//	destP0.Y -= srcP0.Y * pixelScale / bSizeGain2;
			//	srcP0.Y = 0;
			//}
			//if(srcP1.X > srcImageSpan)
			//{
			//	destP1.X += (srcImageSpan - srcP1.X) * pixelScale / bSizeGain2;
			//	srcP1.X = srcImageSpan;

			//}
			//if(srcP1.Y > srcImageSpan)
			//{
			//	destP1.Y += (srcImageSpan - srcP1.Y) * pixelScale / bSizeGain2;
			//	srcP1.Y = srcImageSpan;

			//}
			var isWinter = Settings.IsThemeWinter();
			//				var attacksVisible = DefenseHistoryTab.IsVisible() | OutgoingTab.IsVisible() | IncomingTab.IsVisible() | HitTab.IsVisible() | AttackTab.IsVisible();
			var attacksVisible = OutgoingTab.IsVisible() | IncomingTab.IsVisible() | AttackTab.IsVisible();







			// sprite draw
			draw.Begin();


			var focusOnCity = (View.viewMode == ViewMode.city);

			parallaxGain = Settings.parallax * (viewW.Z*0.125f).Min(1);// (Math.Min(1,viewZoomLag / 128.0f));// * regionAlpha * (1 - cityAlpha);

			{
				//var wToCGain = pixelScaleInverse;// (1.0f / viewZoomLag);
				//				var halfTiles = (clientSpan * (0.5f / cameraZoomLag));
				var _c0 = clip.c0.ScreenToWorld();
				var _c1 = clip.c1.ScreenToWorld();
				cx0 = _c0.X.FloorToInt().Clamp(0,World.span);
				cy0 = (_c0.Y.FloorToInt()).Clamp(0,World.span);
				cx1 = (_c1.X.CeilToInt() + 1).Clamp(0,World.span);
				cy1 = (_c1.Y.CeilToInt() + 1).Clamp(0,World.span);
			}
			cullWC = new Span2i(cx0,cy0,cx1,cy1);
			cullWCF = cullWC.span2;

			//	ds.Antialiasing = CanvasAntialiasing.Aliased;
			//if(wantImage)
			//{
			//	if(wantImage)
			//	{
			//		const byte brightness = 128;
			//		const byte oBrightness = 255;
			//		const byte alpha = 255;
			//		const float texelGain = 1.0f / srcImageSpan;
			//		//draw.AddQuad(CnV.Draw.Layer.background,worldBackground,
			//		//	new(0,0),new(World.span,World.span),
			//		//	 new Color(brightness,brightness,brightness,alpha),ConstantDepth,0); ;

			//		//if(worldObjects != null)
			//		//	draw.AddQuad(CnV.Draw.Layer.background + 1,worldObjects,
			//		//		new(0,0),new(World.span,World.span),new Color(oBrightness,oBrightness,oBrightness,alpha),ConstantDepth,zCities);
			//	}
			//}

			//   ds.Antialiasing = CanvasAntialiasing.Antialiased;
			// ds.Transform = new Matrix3x2( _gain, 0, 0, _gain, -_gain * ShellPage.viewCW.X, -_gain * ShellPage.viewCW.Y );

			//           dxy.X = (float)sender.Width;
			//            dxy.Y = (float)sender.ActualHeight;

			//            ds.DrawLine( SC(0.25f,.125f),SC(0.lineThickness,0.9f), raidBrush, lineThickness,defaultStrokeStyle);
			//           ds.DrawLine(SC(0.25f, .125f), SC(0.9f, 0.lineThickness), shadowBrush, lineThickness, defaultStrokeStyle);
			// if (IsPageDefense())
			var wantDarkText = false;




			var rgb = attacksVisible ? 255 : 255;
			var tint = new Color(rgb,rgb,rgb,intAlpha);
			var tintShadow = new Color(0,0,16,intAlpha * 3 / 4);
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
				nameColor = new Color() { A = nameAlpha,G = 0,B = 0,R = 0 };
				nameColorOutgoing = new Color() { A = nameAlpha,G = 8,B = 14,R = 22 };
				nameColorHover = new Color() { A = nameAlpha,G = 00,B = 0,R = 40 };
				myNameColor = new Color() { A = nameAlpha,G = 0,B = 30,R = 15 };
				nameColorIncoming = new Color() { A = nameAlpha,G = 0,B = 12,R = 6 };
				nameColorSieged = new Color() { A = nameAlpha,G = 10,B = 16,R = 25 };
				nameColorIncomingHover = new Color() { A = nameAlpha,G = 00,B = 34,R = 50 };
				nameColorSiegedHover = new Color() { A = nameAlpha,G = 20,B = 28,R = 50 };
				myNameColorIncoming = new Color() { A = nameAlpha,G = 12,B = 12,R = 25 };
				myNameColorSieged = new Color() { A = nameAlpha,G = 10,B = 12,R = 25 };
				nameColorDungeon = new Color() { A = nameAlpha,G = 0,B = 15,R = 0 };
			}
			//			shadowColor = new Color() { A = 128 };


			if(hideSceneCounter <= 0) {

				//Assert(City.build != 0);
				//	if(false)
				//	{
				//		var td = TileData.instance;

				// 0 == land
				// 1 == shadows
				// 2 == features
				//foreach(var layer in td.layers)
				//	if(false)
				//	{
				//		var layer = TileLayer.bonus;
				//		var isBonus = Object.ReferenceEquals(layer,TileLayer.bonus);
				//		//if(!wantDetails && !isBonus)
				//		//	continue;
				//		//if(debugLayer != -1)
				//		//{
				//		//	if((int)layer.id!= debugLayer)
				//		//		continue;

				//		//}

				//		var layerDat = layer.data;

				//		for(var cy = cy0;cy < cy1;++cy)
				//		{
				//			for(var cx = cx0;cx < cx1;++cx)
				//			{
				//				var ccid = cx + cy * World.span;
				//				var imageId = layerDat[ccid];
				//				if(imageId == 0)
				//					continue;




				//				//   var layerData = TileData.packedLayers[ccid];
				//				//  while (layerData != 0)
				//				{
				//					//    var imageId = ((uint)layerData & 0xffffu);
				//					//     layerData >>= 16;
				//					var tileId = TileLayer.TilesetId(imageId);
				//					var off = TileLayer.ImageId(imageId);
				//					var tile = td.tilesets[(int)tileId];
				//					Assert(off < td.tilesets[(int)tileId].tilecount);
				//					if(tile.material == null)
				//						continue;


				//					var wc = new System.Numerics.Vector2(cx,cy);
				//					//ar cc = wc.WorldToCamera();
				//					var shift = new System.Numerics.Vector2((isBonus ? imageId== TileData.tilePortalOpen ? bonusLayerScale*2f : bonusLayerScale : 1f) * 0.5f);
				//					var cc0 = wc - shift;
				//					var cc1 = wc + shift;
				//					var sy = off / tile.columns;
				//					var sx = off - sy * tile.columns;
				//					var uv0 = new System.Numerics.Vector2((float)(sx / tile.scaleUToX),(float)(sy /tile.scaleVToY));
				//					var uv1 = new System.Numerics.Vector2((float)((sx + 1) / tile.scaleUToX),(float)((sy + 1) / tile.scaleVToY));
				//					Assert(uv0.X.IsInRange(0,1));
				//					Assert(uv0.Y.IsInRange(0,1));
				//					Assert(uv1.X.IsInRange(0,1));
				//					Assert(uv1.Y.IsInRange(0,1));
				//					if(debugLayer != -1)
				//					{
				//						DrawTextBox($"{(sx, sy)}",cc0,nameTextFormat,Color.White,255);
				//					}

				//					for(int isShadow = layer.wantShadow&&wantShadow ? 2 : 1;--isShadow >= 0;)
				//					{
				//						if(isShadow == 1 && !tile.wantShadow)
				//						{
				//							continue;
				//						}
				//						var _tint = (isShadow == 1) ? tintShadow : !tile.isBase ? World.GetTint(ccid) : tint;
				//						if(!isBonus && isShadow == 0 && !tile.isBase)
				//							_tint.A = intAlpha; ;
				//						if(wantCity && (cx, cy).WorldToCid() == City.build)
				//						{
				//							if(cityAlphaI >= 255)
				//								continue;
				//							_tint.A = (byte)((_tint.A * (256 - cityAlphaI)) / 256);
				//						}

				//						var dz = tile.z * parallaxGain; // shadows draw at terrain level 


				//						if(tile.canHover)
				//						{
				//							if(TryGetViewHover((cx, cy).WorldToCid(),out var hz))
				//							{
				//								dz += hz * viewHoverZGain;
				//							}
				//						}
				//						//if(!AppS.IsKeyPressedShift())
				//						//	continue;

				//						//if(isShadow == 1)
				//						//{

				//						//	// shift shadow
				//						//	//	cc = (cc - cameraLightC) * (1 + dz*2) + cameraLightC;
				//						//	//		cc = (cc - cameraLightC)*
				//						////	dz = 0;
				//						//}


				//						draw.AddQuad((isShadow == 1) ? Layer.tileShadow : (tile.isBase ? Layer.tileBase : Layer.tiles) + ((int)layer.id+1),
				//							(isShadow == 1 ? tile.shadowMaterial : tile.material),cc0,cc1,
				//							uv0,
				//							uv1,_tint,
				//							depth: dz);
				//						//(cc0, cc1).RectDepth(dz));




				//					}
				//				}
				//			}

				//		}

				//	}
				//	//
				//	//   if (attacksVisible)
				//	//       ds.FillRectangle(new Rect(new Point(), clientSpan.ToSize()), desaturateBrush);


				//}


				// fade out background
				//if (attacksVisible)
				//{
				//    ds.FillRectangle(new Rect(new Point(), clientSpan.ToSize()), desaturateBrush);
				//    notFaded=false;
				//}

				//    ds.Antialiasing = CanvasAntialiasing.Antialiased;
				if(worldChanges != null && !focusOnCity) {
					var t2d = worldChanges.texture2d;
					//var tOffset = new Vector2(0.0f, 0.0f);
					//var t2d = worldChanges.texture2d;
					//var scale = new Vector2(t2d.TexelWidth, t2d.TexelHeight);
					Assert(false);
					//draw.AddMesh(tesselatedWorld,Layer.tileShadow - 1,worldChanges);
				}
				if(false && (worldOwners != null && !focusOnCity)) {
					var tOffset = new System.Numerics.Vector2(0.0f,0.0f);
					var t2d = worldOwners.texture2d;
					var scale = new System.Numerics.Vector2(t2d.TexelWidth,t2d.TexelHeight);
					draw.AddQuad(Layer.tiles - 1,worldOwners,
						new(0,0),new(World.span,World.span),new Color(128,128,128,128),zTerrain);
				}
				if(wantCity && City.build != 0) {
					Assert(cityAlpha > 0.0f);
					// this could use refactoring
					CityView.Draw(cityAlpha);
				}
				//else
				{
					if(TilesReady()) {
						//	Assert(cityAlpha <= 0);
						var cc0x = (cx0/100).Clamp(0,World.continentCountX-1);
						var cc0y = (cy0/100).Clamp(0,World.continentCountY-1);
						var cc1x = (cx1/100).Clamp(0,World.continentCountX-1);
						var cc1y = (cy1/100).Clamp(0,World.continentCountY-1);

						//	var c = WorldC.FromCid(viewW).continentClamped;
						for(int i = cc0x;i<=cc1x;++i)
							for(int j = cc0y;j<=cc1y;++j)
								draw.AddMesh(World.renderTileMeshes[i,j],Layer.tileBase,World.renderTileMaterial);
					}
				}

				circleRadiusBase = circleRadMin * shapeSizeGain * 7.9f;
				var circleRadius = animTLoop.Lerp(circleRadMin,circleRadMax) * shapeSizeGain * 6.5f;
				//    var highlightRectSpan = new Vector2(circleRadius * 2.0f, circleRadius * 2);

				//	ds.FillRectangle(new Rect(0, 0, clientSpan.X, clientSpan.Y), CnVServer.webViewBrush);



				if(!focusOnCity) {
					//
					// Draw blessed indicators
					// Note:  These could be baked into the big mesh, except that we need another layer
					//
					foreach(var shrine in Shrine.all) {
						foreach(var l in shrine.maybeLit) {
							var c = l;
							if(IsCulledWC(c))
								continue;
							if(!City.Get(c).isBlessed)
								continue;
							Material sprite = blessedMaterials[(int)shrine.virtue];
							float sizeGain = 0.5f;
							var dimX = sizeGain * (168.0f/100.0f);
							var dimY = sizeGain;
							var y = c.y+0.125f;
							var _c0 = new Vector2(c.x - dimX,y - dimY);
							var _c1 = new Vector2(c.x + dimX,y + dimY);
							draw.AddQuad(Layer.blessedOverlay + 4,sprite,_c0,_c1,diffuseTint with { A=192 },zCities);

						}
					}

					// Draw Rich presence
					foreach(var p in World.activePlayers) {
						if(!p.isOnline)
							continue;
						var c = p.visitingCid;
						if(IsCulledWC(c))
							continue;
						// Todo: Draw avatar
						var id = cityVisitorCounts.AddOrUpdate(c,(key) => 0,(key,prior) => prior+1);
						Material sprite = p.GetAvatarMaterial();
						float sizeGain = Settings.iconScale * 64.0f;
						var dim = spriteSizeGain;// *1.25f; // square
						float x, y;
						switch(id&3) {
							case 0: x = c.x-0.25f; y = c.y-0.25f; break;
							case 1: x = c.x+0.25f; y = c.y-0.25f; break;
							case 2: x = c.x+0.25f; y = c.y+0.25f; break;
							default: x = c.x-0.25f; y = c.y+0.25f; break;

						}
						var _c0 = new Vector2(x - dim,y - dim);
						var _c1 = new Vector2(x + dim,y + dim);
						draw.AddQuad(Layer.avatarOverlay,sprite,_c0,_c1,new Color(255,255,255,255),zCities);

					}

					cityVisitorCounts.Clear();

					var incomingVisible = IncomingTab.IsVisible() || ReinforcementsTab.IsVisible() || NearDefenseTab.IsVisible() || Settings.incomingAlwaysVisible;
					var outgoingVisible = AttackTab.IsVisible() || OutgoingTab.IsVisible() || Settings.attacksAlwaysVisible;
					{

						if(AttackTab.IsVisible()) {
							List<AttackTab.AttackCluster> hovered = new();
							if(!AttackTab.attackClusters.IsNullOrEmpty()) {
								foreach(var cluster in AttackTab.attackClusters) {
									if(cluster.attacks.Any(a => Spot.IsSelectedOrHovered(a)) |
													cluster.targets.Any(a => Spot.IsSelectedOrHovered(a))) {
										hovered.Add(cluster);
									}
								}
							}

							if(!AttackTab.attackClusters.IsNullOrEmpty()) {
								var showAll = (AttackTab.attackClusters.Length < Settings.showAttacksLimit) &&(!hovered.Any());
								foreach(var cluster in AttackTab.attackClusters) {
									var isHover = hovered.Contains(cluster);
									if(!(showAll || isHover))
										continue;
									{
										var c0 = cluster.topLeft;
										var c1 = cluster.bottomRight;
										DrawRectOutlineShadow(Layer.effects+2,c0,c1,isHover ? new Color(128,64,64,220) : new Color(64,0,0,162),2,2);
									}

									{
										var real = cluster.real;
										var c0 = real.CidToWorldV();
										foreach(var a in cluster.attacks) {
											//	var t = (tick + a.CidToRandom()).Wave(1.5f / 512.0f + 0.25f,1.75f / 512f + 0.25f);
											var t = (tick * a.CidToRandom().Lerp(1.0f,1.25f));
											var r = t.Ramp();
											var c1 = a.CidToWorldV();
											var spot = Spot.GetOrAdd(a);
											DrawAction(0.5f,1.0f,r,c1,c0,Color.Red,troopImages[(int)spot.TroopType],false,null,
												lineThickness: lineThickness,highlight: Spot.IsSelectedOrHovered(a));
										}
										//	foreach (var target in cluster.targets)
										//	{
										////		var c = target.CidToCamera();
										//	//	var rnd = target.CidToRandom();

										//	//	var t = (tick * rnd.Lerp(1.5f / 512.0f, 1.75f / 512f)) + 0.25f;
										//		//var r = t.Wave().Lerp(circleRadiusBase, circleRadiusBase * 1.325f);

										//		DrawAccent(target, 0.2f, Color.White );
										//	}
										//	DrawAccent(real, 1.5f, Color.Red,2.0f);


									}
								}
							}
							{
								foreach(var t in AttackPlan.plan.targets) {
									var col = t.attackType switch {
										AttackType.senator => CColor(168,0,0,255),
										AttackType.senatorFake => CColor(128,34,33,212),
										AttackType.se => CColor(0,0,255,255),
										AttackType.seFake => CColor(98,32,168,212),
										_ => CColor(64,64,64,64)
									};
									DrawRectOutlineShadow(Layer.effects,t.cid,col,(t.attackCluster >= 0) ? t.attackCluster.ToString() : null,1,-2);
								}
								foreach(var t in AttackPlan.plan.attacks) {
									var col = t.attackType switch {
										AttackType.assault => assaultColor,
										AttackType.senator => CColor(168,64,0,242),
										AttackType.senatorFake => CColor(128,48,0,192), // not really used as attack
										AttackType.se => CColor(148,0,148,242),
										AttackType.seFake => CColor(128,32,128,192), // not really used as attack
										_ => CColor(64,64,64,64)
									};
									DrawDiamondShadow(Layer.effects,t.cid,col,(t.attackCluster >= 0) ? t.attackCluster.ToString() : null);
								}

							}

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
						if((incomingVisible || outgoingVisible)) {
							var cullSlopSpace = (32 * pixelScale).RoundToInt();
							for(int iOrO = 0;iOrO < 2;++iOrO) {
								var isIncoming = (iOrO == 0);
								if(isIncoming) {
									if(!incomingVisible)
										continue;
								}
								else {
									if(!outgoingVisible)
										continue;
								}
								var raidsVisble = Settings.raidVisible != false;
								var list = City.alliedCities; //defenders ? Spot.defendersI : Spot.defendersO;
								bool noneIsAll = list.Length <= Settings.showAttacksLimit;
								bool showAll = list.Length <= Settings.showAttacksLimit0 ||(isIncoming ? Settings.incomingAlwaysVisible : Settings.attacksAlwaysVisible);
								foreach(var city in list) {
									if(!city.testContinentFilter)
										continue;

									if(isIncoming ? city.incoming.Any() : city.outgoing.Any()) {

										var cityCid = city.cid;
										var c1 = cityCid.CidToWorld();
										if(IsSquareCulledWC(c1,cullSlopSpace))  // this is in pixel space - Should be normalized for screen resolution or world space (1 continent?)
											continue;
										var incAttacks = 0;
										var incDef = 0;
										//								var incTs = 0u;
										//	var hasSettle = false;
										var sieged = false;
										var hasSen = false;
										var visibilityTime = isIncoming ? Sim.simTime + city.scoutRange : ServerTime.infinity;
										//	var hasArt = false;
										//	var hasAssault = false;
										foreach(var i in isIncoming ? city.incoming : city.outgoing) {
											var c0 = isIncoming ? i.sourceCid.CidToWorld() : i.targetCid.CidToWorld();
											if(IsSegmentCulledWC(c0,c1))
												continue;
											// not visible yet
											if(isIncoming) {
												if(i.arrival > visibilityTime)
													continue;
												if(!i.departed)
													continue;
											}

											if(!isIncoming && (i.isAttack ) && !i.isReturn && !IsCulledWC(c0)) {
												Assert(i.shareInfo);
												targetOpponents.Add(i.targetCid);
											}

											Color c;
											c = Color.Magenta;
											if(i.isReturn) {
												c = returnColor;
											}
											else {
												if(i.isSettle) {
													if(i.arrived)
														c = returnColor;  // returning carts
													else
														c = Color.White;
													//	hasSettle=true;
													// outgoing
													// if(hasSettle)
													if(!isIncoming) {
														Material sprite = settleMaterial;
														float spriteSize = spriteSizeGain;

														var mid = c0;
														var _c0 = new Vector2(mid.x - spriteSize,mid.y - spriteSize);
														var _c1 = new Vector2(mid.x + spriteSize,mid.y + spriteSize);
														draw.AddQuadWithShadow(Layer.statusOverlay,Layer.effectShadow,sprite,_c0,_c1,Color.White,shadowColor,zEffects);
													}
													else {
														Assert(false);
													}

												}
												else if(i.isRaid) {
													if(!raidsVisble)
														continue;
													c = Color.DarkSlateBlue;
												}

												else if(i.isDefense) {
													c = i.arrival <= serverNow ? defenseArrivedColor : defenseColor;
													if(isIncoming)
														++incDef;
												}
												else if(i.isAttack &&  (!isIncoming || city.IsVisibleIncomingAttack(i))) {
													c = assaultColor;
													if(isIncoming) {
														++incAttacks;
														sieged |= i.isSieging;
														hasSen |= i.isSieging && i.hasSenator;
														if(i.isSieging)
															c = siegeColor; 
													}
													else {

														Assert(i.shareInfo);
														if(i.hasSenator) {
															c = senatorColor; ;
														}
														else if(i.hasArt) {
															//	hasArt = true;
															c = artColor;
														}
														else if(i.isSiege) // intel maybe available
														{
															c = siegeColor;
														}
													}

												}
											}
											var sel = Spot.IsSelectedOrHovered(i.sourceCid,i.targetCid);
											if(!(showAll || sel)) {
												//													DrawRectOutlineShadow(Layer.effects, targetCid, c, null, 2, -4f);
												continue;
												//       c.A = (byte)((int)c.A * 3 / 8); // reduce alpha if not selected
											}
											// don't double draw
											var hash = i.GetHashCode();
											if(drawArmyHash.Add(hash)) {
												var t = (tick  * ((hash&0xffff)/65536f).Lerp(0.375f,0.5f));
												//	var t = (tick +i.sourceCid.CidToRandom()).Wave(1.5f / 512.0f+0.25f,2.0f / 512f+ 0.25f) ;
												var r = t.Ramp();
												var alpha = 1.0f;
												TType ttype = Troops.ttInvalid; ;
												if(i.troops.Length > 0) {
													if(i.shareInfo) {
														var last = i.lastSeen;
														var nSprite = last.troops.Count;

														(int iType, alpha) = GetTroopBlend((float)t,nSprite);
														ttype = last.troops.GetIndexType(iType);
													}
													else {
														// TODO base it on travel time
														ttype = i.travelType.TravelTypeToTT();
													}
												}
												(int x, int y) _c0, _c1;

												if(isIncoming ^ i.isReturn) {
													_c0 = c0; _c1 = c1;
												}
												else {
													_c0 = c1; _c1 = c0;
												}


												DrawAction(i.TimeToArrival(serverNow),i.journeyTime,r,
													_c0.ToVector(),
												_c1.ToVector(),c,ttype != ttInvalid ?troopImages[ttype] : null,
												true,i,alpha: alpha,lineThickness: lineThickness,highlight: sel);
											}
										}
										if(isIncoming) {
											if(!IsCulledWC(c1)) {
												if(incAttacks > 0) {
													Material sprite = null;
													if(sieged) {
														if(hasSen)
															sprite = siege1Material;
														else
															sprite = siege0Material;
													}
													else {
														if(city.isSubOrMine)
															sprite = attack2Material;
														else if(city.isAllyOrNap)
															sprite = attack0Material;
														else
															sprite = attack1Material;


													}
													float spriteSize = spriteSizeGain;

													var mid = new Vector2(c1.x + 0.25f,c1.y-0.25f);
													var _c0 = new Vector2(mid.X - spriteSize,mid.Y - spriteSize);
													var _c1 = new Vector2(mid.X + spriteSize,mid.Y + spriteSize);
													draw.AddQuadWithShadow(Layer.statusOverlay,Layer.effectShadow,sprite,_c0,_c1,Color.White,shadowColor,zEffects);
												}
												if(incDef > 0) {
													Material sprite = defenseMaterial;
													float spriteSize = spriteSizeGain;

													var mid = new Vector2(c1.x - 0.25f,c1.y-0.25f);
													var _c0 = new Vector2(mid.X - spriteSize,mid.Y - spriteSize);
													var _c1 = new Vector2(mid.X + spriteSize,mid.Y + spriteSize);
													draw.AddQuadWithShadow(Layer.statusOverlay,Layer.effectShadow,sprite,_c0,_c1,Color.White,shadowColor,zEffects);
												}
											}

											//if(!IsCulledWC(c1) && (wantDetails || showAll || Spot.IsSelectedOrHovered(cityCid,noneIsAll)))
											//{
											//	DrawTextBox($"{incAttacks}{city.IncomingInfo()}\n{ (city.tsDefMax + 999) / 1000 }k",
											//			c1.ToVector(),tipTextFormatCentered,incAttacks != 0 ? Color.White : Color.Cyan,textBackgroundOpacity,Layer.tileText);
											//}
										}
										else {

											// outgoing

											//if(city.hasOutgoingAttacks)
											//	DrawRectOutlineShadow(Layer.effects - 1,cityCid,attackColor,null,1,-8f);
										}
									}
								}
							}
							drawArmyHash.Clear();

							//
							// process targets, draw badges
							//
							//
							foreach(var opCid in targetOpponents) {
								// opponents with 
								
								var c = City.Get(opCid);
								
								var hasSiege = false;
								var hasPendingSiege = false;
								var hasAttack = false;
								foreach(var i in c.incoming) {
									if(i.isAttack) {
										hasAttack=true;
										hasSiege |= i.isSieging;
										//	hasPendingSiege |= i.isSiege;

									}
								}
								if(hasAttack) {
									Material sprite = null;
									if(hasSiege) {
										sprite = siege1Material;
									}
									else if(hasPendingSiege) {
										sprite = siege0Material;
									}
									else {
										sprite = attack1Material;
									}
									float spriteSize = spriteSizeGain;
									var c1 = c.c;

									var mid = new Vector2(c1.x + 0.25f,c1.y-0.25f);
									var _c0 = new Vector2(mid.X - spriteSize,mid.Y - spriteSize);
									var _c1 = new Vector2(mid.X + spriteSize,mid.Y + spriteSize);
									draw.AddQuadWithShadow(Layer.statusOverlay,Layer.effectShadow,sprite,_c0,_c1,Color.White,shadowColor,zEffects);

								}
							}
							targetOpponents.Clear();

						}
						//if(incomingVisible)
						//{
						//	foreach(var city in City.myCities)
						//	{
						//		if(!city.testContinentFilter)
						//			continue;
						//		Assert(city is City);
						//		if(!city.incoming.Any())
						//		{
						//			var targetCid = city.cid;
						//			var c1 = targetCid.CidToWorld();
						//			if(IsCulledWC(c1))  // this is in pixel space - Should be normalized for screen resolution or world space (1 continent?)
						//				continue;
						//			if(wantDetails || Spot.IsSelectedOrHovered(targetCid,true))
						//			{
						//				DrawTextBox($"{(city.tsDefMax.Max(city.tsHome) + 999) / 1000 }k Ts (#:{city.reinforcementsIn.CountNullable()})",c1.ToVector(),tipTextFormatCentered,Color.Cyan,textBackgroundOpacity,Layer.tileText);
						//			}

						//		}
						//	}
						//}
						var tradesVisible = Settings.tradesVisible != false;
						var tradePartlyVisible = Settings.tradesVisible == null;
						if(tradesVisible) {
							try {

								var cullSlopSpace = (32 * pixelScale).RoundToInt();
								for(int iOrO = 0;iOrO < 2;++iOrO) {
									var isIncoming = (iOrO == 0);

									var list = City.subCities; //defenders ? Spot.defendersI : Spot.defendersO;
									bool showAll = !tradePartlyVisible;
									foreach(var city in list) {
										if(!city.testContinentFilter) // || !(showAll || Spot.IsSelectedOrHovered(city.cid) ))
											continue;


										{

											var cityCid = city.cid;
											var c1 = cityCid.CidToWorld();
											if(IsSquareCulledWC(c1,cullSlopSpace))  // this is in pixel space - Should be normalized for screen resolution or world space (1 continent?)
												continue;


											foreach(var i in isIncoming ? city.tradesIn : city.tradesOut) {
												var c0 = isIncoming ? i.sourceCid.CidToWorld() : i.targetCid.CidToWorld();
												if(IsSegmentCulledWC(c0,c1))
													continue;

												var c = i.isReturning ? returnColor : Color.Green;
												if(!(showAll || Spot.IsSelectedOrHovered(i.sourceCid,i.targetCid))) {
													continue;
												}
												// don't double up
												if(drawTradesHash.Add(i.GetHashCode())) {
													var t = (tick * i.sourceCid.CidToRandom().Lerp(1.0f,1.375f));
													//var t = (tick + i.sourceCid.CidToRandom()).Wave(1.5f / 512.0f+0.25f,2.0f / 512f+0.25f);
													var r = t.Ramp();
													(int x, int y) _c0, _c1;
													if(isIncoming ^ i.isReturning) {
														_c0 = c0; _c1 = c1;
													}
													else {
														_c0 = c1; _c1 = c0;
													}
													DrawAction((i.isReturning ? i.returnTime : i.arrival) - Sim.simTime,i.travelTime,r,
														_c0.ToVector(),
													_c1.ToVector(),c,tradeImages[i.viaWater.Switch(0,1)],
													true,i,alpha: 255,lineThickness: lineThickness,highlight: false);
												}
											}

										}
									}
								}

							}
							catch(Exception ex) {
							}
							drawTradesHash.Clear();
						}

						if(NearRes.IsVisible()||DonationTab.IsVisible()) {
							var sendOffset = new Vector2(0.125f);//  new System.Numerics.Vector2(0.125f *pixelScale,0.125f *pixelScale);
							var viewHover = Spot.viewHover;
							var hasHover = viewHover != 0;
							foreach(var city in City.subCities) {
								var needsHover = (hasHover && !city.isHover);
								//	if(!city.testContinentFilter)
								//		continue;
								var wc = city.cid.CidToWorld();
								//var cc = wc.WorldToCamera();
								if(!needsHover) {
									if(!IsCulledWC(wc)) {
										var res = city.sampleResources;
										for(int r = 0;r < 4;++r) {
											var xT0 = (r + 0.5f) / 4.0f;
											var xT1 = (r + 1.375f) / 4.0f;
											var yt0 = 0.0f;
											var yt1 = MathF.Sqrt(MathF.Sqrt(res[r]  * (1.0f / (1024 * 1024)))).Min(1.0f); // 0 .. 1M
											var color = r switch {
												0 => new Color(150,75,0,255),
												1 => new Color(128,128,128,255),
												2 => new Color(24,124,168,255),
												_ => new Color(192,192,0,255)
											};
											if(yt1 < 0.125f) {
												yt1 = 0.25f;
												color = new Color(255,0,0,255);
											}

											var c0 = new System.Numerics.Vector2(wc.x + (xT0 * 0.8f - 0.5f),
																		wc.y   + (0.25f - yt1 * 0.5f));
											var c1 = new System.Numerics.Vector2(wc.x + (xT1 * 0.8f - 0.5f),
																		wc.y   + 0.25f);
											DrawRect(Layer.materialOverlay,c0,c1,color,zEffects);
											// shadow TODO
											DrawRect(Layer.overlay,c0,c1,CColor(a: 192),0);

										}
									}
								}

								var mo = city.MO;

								if(mo.isNone)
									continue;
								var cityHover = city.isHover;
								try {

									foreach(var toCid in mo.sendCities) {
										if(needsHover && (toCid!=City.viewHover))
											continue;
										var c1 = toCid.CidToWorldV();
										//	var t = (tick * city.cid.CidToRandom().Lerp(1.375f / 512.0f, 1.75f / 512f));
										//	var r = t.Ramp();
										var hover = cityHover;// | Spot.IsHover(toCid);
										DrawAction(wc.ToVector() - sendOffset,c1- sendOffset,hover ? tradeColorHover : tradeColor,lineThickness,hover);
									}
									foreach(var toCid in mo.requestCities) {
										if(needsHover && (toCid!=City.viewHover))
											continue;

										var c1 = toCid.CidToWorldV();
										//	var t = (tick * city.cid.CidToRandom().Lerp(1.375f / 512.0f, 1.75f / 512f));
										//	var r = t.Ramp();
										var hover = cityHover;// | Spot.IsHover(toCid);
										DrawAction(c1 + sendOffset,wc.ToVector() + sendOffset,hover ? tradeColorHover1 : tradeColor1,lineThickness,hover);
									}

								}
								catch(Exception ex) {
									LogEx(ex);
								}

							}
						}
						//if(!defenderVisible && !attacksVisible)
						//	{
						//		if(Spot.settles.Any())
						//		{
						//			try
						//			{
						//				foreach(var s in Spot.settles)
						//				{
						//					var wc0 = s.cid.CidToWorld();
						//					var wc1 = s.rcid.CidToWorld();
						//					if(!IsSegmentCulledWC(wc0,wc1))
						//					{

						//						DrawAction(0.5f,1.0f,1.0f,wc0.ToVector(),wc1.ToVector(),senatorColor,
						//						troopImages[ttSenator],false,null,highlight: Spot.IsSelectedOrHovered(s.cid));
						//						DrawFlag(s.rcid,SpriteAnim.flagGrey,System.Numerics.Vector2.Zero);

						//					}



						//				}
						//			}
						//			catch(Exception ex)
						//			{

						//			}
						//		}

						//	}

					}

					foreach(var cid in Spot.selected) {
						DrawRectOutlineShadow(Layer.effects - 1,cid,selectColor,null,1.0f,1.0f);
						//DrawFlag(cid, SpriteAnim.flagSelected, Vector2.Zero);
					}
					foreach(var cid in Settings.pinned) {
						DrawFlag(cid,SpriteAnim.flagPinned,new System.Numerics.Vector2(4,-4));
					}
					if(Spot.focus != 0) {
						var cid = Spot.focus;
						DrawAccent(cid.cid,-1.125f,focusColor);
					}
					if(Spot.viewHover != 0) {
						var cid = Spot.viewHover;
						DrawAccent(cid.cid,1.25f,hoverColor);
					}

					if(Player.viewHover != PlayerId.MaxValue) {
						if(Player.TryGetValue(Player.viewHover,out var p)) {
							try {
								foreach(var cid in p.cities) {
									DrawFlag(cid,SpriteAnim.flagGrey,new System.Numerics.Vector2(-4,4));
								}
							}
							catch(Exception ex) {
								//LogEx(ex); // collection might change, if this happens just about this render, its 
							}

						}
					}


				}



				{
					if(wantDetails) {
						//
						// Text names
						//	using (var batch = ds.CreateSpriteBatch(CanvasSpriteSortMode.Bitmap))
						{
							// Labels last
							for(var cy = cy0;cy < cy1;++cy) {
								for(var cx = cx0;cx < cx1;++cx) {
									(var name, var isMine, var hovered, var spot) = World.GetLabel((cx, cy));
									//var zScale = CanvasHelpers.ParallaxScale(TileData.zCities);

									if(name != null) {

										var span = 1.0f;
										var cid = new WorldC(cx,cy);

										var drawC = (Vector2)(cid);

										if(spot is null) {
											drawC.X += 0.30f;
											drawC.Y += 0.30f;
										}
										else {
											drawC.Y += span *  7.5f / 16.0f;
										}
										var z = zCities;
										var scale = regionFontScale;


										if(TryGetViewHover(cid,out var hz)) {
											z += hz * viewHoverZGain;
											scale *= hz.Lerp(1.0f,1.25f);
										}
										//	drawC = drawC.Project(zLabels);
										//var layout = GetTextLayout(name,nameTextFormat);
										var color = World.GetTint(cid).Modulate(32);
										//var color = spot ==null ? nameColorDungeon
										//	:
										//	(isMine ?
										//	(incomingStatus.IsAny() ?
										//		(incomingStatus.IsUnderSiege() ? myNameColorSieged
										//						: myNameColorIncoming)
										//						: spot.hasOutgoingAttacks ? nameColorOutgoing
										//							: myNameColor) :
										//(incomingStatus.IsAny() ?
										//	(hovered ?
										//		(incomingStatus.IsUnderSiege() ? nameColorSiegedHover : nameColorIncomingHover)
										//	   : (incomingStatus.IsUnderSiege() ? nameColorSieged : nameColorIncoming))
										//	   : hovered ? nameColorHover
										//	   : spot.hasOutgoingAttacks ? nameColorOutgoing
										//	   : nameColor));

										// a+b-a*b
										// a*( 1+b/a+b)
										// a*b*(1/b + 1/a - 1)

										DrawTextBox(name,drawC,nameTextFormat,wantDarkText ? color.A.AlphaToBlack() : Color.White,
													color,Layer.
													tileText,3,2,z,scale);
										//										layout.Draw(drawC,
										//									, Layer.tileText, z,PlanetDepth);

									}
									//if(false)
									//{
									//	if(spot != null && !focusOnCity && !(Settings.troopsVisible.HasValue && Settings.troopsVisible.Value == false))
									//	{
									//		if(!spot.troopsTotal.Any() && spot.isNotClassified && spot.canVisit && Settings.troopsVisible.GetValueOrDefault())
									//			spot.TouchClassification();
									//		if(spot.troopsTotal.Any() || spot.isClassified)
									//		{
									//			var c1 = (cx, cy);
									//			var rand = spot.cid.CidToRandom();
									//			var t = (tick * rand.Lerp(1.5f / 512.0f,1.75f / 512f)) + 0.25f;

									//			int type;
									//			float typeBlend;
									//			float alpha = 1;// (t * rand.Lerp(0.7f, 0.8f)).Wave() * 0.20f + 0.70f;

									//			if(spot.troopsTotal.Any())
									//			{
									//				var ta = GetTroopBlend(t,spot.troopsTotal.Length);
									//				alpha = ta.alpha;

									//				type = spot.troopsTotal.GetIndexType(ta.iType);
									//			}
									//			else
									//			{
									//				type = spot.TroopType;
									//				typeBlend = 1;
									//				switch(spot.classification)
									//				{
									//					case Spot.Classification.misc:
									//					case Spot.Classification.unknown:
									//					case Spot.Classification.pending:
									//					case Spot.Classification.missing:
									//						goto dontDraw;
									//				}

									//			}
									//			var r = t.Ramp();
									//			var spriteSize = new Vector2(spriteSizeGain);
									//			var _c0 = c1.ToVector() - spriteSize;
									//			var _c1 = c1.ToVector() + spriteSize;

									//			draw.AddQuadWithShadow(Layer.effects,Layer.effectShadow,troopImages[type],_c0,_c1,HSLToRGB.ToRGBA(rectSpan,0.3f,0.825f,alpha,alpha + 0.125f),ShadowColor(alpha),
									//				zCities);
									//		}
									//		dontDraw:;
									//	}
									//}
								}
							}
						}
					}
				}
			}
			//if (!ShellPage.IsCityView() && Player.isTest)
			//{
			//	var avatars = PlayerPresence.all;
			//	foreach (var a in avatars)
			//	{
			//		var cid = a.cid;
			//		var pid = a.pid;
			//		if (Player.myId == pid) // don't show me
			//			continue;

			//		var wc = cid.CidToWorld();
			//		if (!IsCulledWC(wc))
			//		{
			//			DrawTextBox($"~{Player.IdToName(pid)}~", wc.WToCamera(), tipTextFormatCentered, Color.Red, 255, Layer.tileText, 3, 3, null, -1, 0.75f * Settings.fontScale);
			//		}
			//	}
			//}

			// show selected

			if(underMouse!= priorUnderMouse) {
				ToolTips.actionToolTip = underMouse;
				ToolTipWindow.TipChanged();
			}
			Assert(underMouse is null == ToolTips.actionToolTip is null);

			//var _toolTip = ToolTips.toolTip;
			//if(_toolTip != null)
			//{
			//	//	TextLayout textLayout = GetTextLayout( _toolTip, tipTextFormat);
			//	//	var bounds = textLayout.span;
			//	//System.Numerics.Vector2 c = ShellPage.mousePositionC + new System.Numerics.Vector2(16, 16);
			//	System.Numerics.Vector2 c = new System.Numerics.Vector2(clientSpan.X-2,2).ScreenToWorld();
			//	DrawTextBox(_toolTip,c,tipTextFormatRight,Color.White,192,Layer.overlay,4,4,ConstantDepth,0,scale: baseFontScale);
			//}
			//var _contTip = ToolTips.contToolTip;
			//if(_contTip != null)
			//{
			//	var alpha = pixelScale.SmoothStep(cityZoomThreshold - 128,cityZoomThreshold + 128).
			//		Max(pixelScale.SmoothStep(cityZoomWorldThreshold + 16,cityZoomWorldThreshold - 16));
			//	System.Numerics.Vector2 c = new System.Numerics.Vector2(clientSpan.X/2,2).ScreenToWorld();
			//	DrawTextBox(_contTip,c,tipTextFormatCentered,Color.White.Scale(alpha),(byte)(alpha * 192.0f).RoundToInt(),Layer.overlay,4,4,ConstantDepth,0,scale: baseFontScale);
			//}
			//if(View.IsCityView())
			//{
			//	var                     alpha = 255;
			//	System.Numerics.Vector2 c     = new System.Numerics.Vector2(clientSpan.X - 32, 16).ScreenToCamera();
			//	var                     city  =  City.GetBuild();
			//	if(city != null)
			//	{
			//		var counts = city.GetTownHallAndBuildingCount(false);

			//		DrawTextBox($"{counts.buildingCount}/{counts.townHallLevel * 10}", c, tipTextFormatRight, Color.White.Scale(alpha), (byte)(alpha * 192.0f).RoundToInt(), Layer.overlay, 11, 11, ConstantDepth, 0, 0.5f);
			//	}
			//}
			var _debugTip = ToolTips.debugTip;
			if(_debugTip != null) {
				ToolTips.debugTip=null;
				var alpha = 255;
				System.Numerics.Vector2 c = new Vector2(clientSpan.X/2,clientSpan.Y -16).ScreenToWorld();
				DrawTextBox(_debugTip,c,tipTextFormatCenteredBottom,Color.White.Scale(alpha),(byte)(alpha * 192.0f).RoundToInt(),Layer.overlay2d,4,4,0,scale: baseFontScale);
			}
#if DEBUG
			//	DrawRectOutlineShadow(Layer.effects,new Vector2(16,16).ScreenToWorld(),clientSpan.ScreenToWorld() - new Vector2(16f.ScreenToWorld()),Color.Yellow,4,0);
#endif
			if(fadeCounter > 0) {

				fadeCounter = (float)(fadeCounter - (timeSinceLastFrame*0.125f)).Max(0.0);
				DrawRect(Layer.overlay2d-1,new Vector2(-4,-4).ScreenToWorld(),(clientSpan+new Vector2(4f)).ScreenToWorld(),new Color(byte.MinValue,default,default,fadeCounter.SCurve().UNormToByte()),0);
			}


			//if (popups.Length > 0)
			//{
			//	var color = isFocused ? new Color(135, 235, 255, 255) : new Color(255, 255, 255, 255);
			//	foreach (var pop in popups)
			//	{
			//		Vector2 c0 = new Vector2(pop.c0.X, pop.c0.Y).ScreenToCamera();
			//		Vector2 c1 = new Vector2(pop.c1.X, pop.c1.Y).ScreenToCamera();
			//		draw.AddQuad(Layer.webView, quadTexture, c0, c1, color, ConstantDepth, 0);/// c0.CToDepth(),(c1.X,c0.Y).CToDepth(), (c0.X,c1.Y).CToDepth(), c1.CToDepth() );

			//	}

			//}
			draw.End();

		}
		catch(Exception ex) {
			LogEx(ex);
			draw._beginCalled = false;
		}

		static (int iType, float alpha) GetTroopBlend(float t,int nSprite) {
			int iType = 0;
			float alpha = 1;
			if(nSprite > 1) {
				var fSprite = nSprite;
				Assert(t > 0);
				t -= MathF.Floor(t / fSprite) * fSprite;
				var rType = MathF.Floor(t);
				t -= rType;

				iType = ((int)rType).Min(nSprite - 1);
				if(t < 0.25f)
					alpha = AMath.SCurve(t * 4.0f);
				else if(t > 0.75f)
					alpha = AMath.SCurve((1 - t) * 4.0f);

			}
			return (iType, alpha);
		}
	}

	static Dictionary<int,TextLayout> nameLayoutCache = new Dictionary<int,TextLayout>();
	static public TextLayout GetTextLayout(string name,TextFormat format) {
		var hash = name.GetHashCode(StringComparison.Ordinal);
		if(nameLayoutCache.TryGetValue(name.GetHashCode(StringComparison.Ordinal),out var rv))
			return rv;
		rv = new TextLayout(name,format);

		if(nameLayoutCache.Count >= maxTextLayouts)
			nameLayoutCache.Remove(nameLayoutCache.First().Key);
		nameLayoutCache.Add(hash,rv);

		return rv;

	}







	public static void DrawTextBox(string text,Vector2 at,TextFormat format,Color color,byte backgroundAlpha,int layer = Layer.tileText,float _expandX = 2.0f,float _expandY = 0f,float zBias = -1,float scale = 0) {
		DrawTextBox(text,at,format,color,backgroundAlpha == 0 ? new Color() : color.IsDark() ? new Color(byte.MaxValue,byte.MaxValue,byte.MaxValue,backgroundAlpha) : new Color((byte)0,(byte)0,(byte)0,backgroundAlpha),layer,_expandX,_expandY,zBias,scale);
	}

	private static void DrawTextBox(string text,Vector2 at,TextFormat format,Color color,Color backgroundColor,int layer = Layer.tileText,float _expandX = 0.0f,float _expandY = 0,float zBias = -1,float scale = 0) {
		if(IsSquareCulledWC(at,textBoxCullSlop)) {
			return;
		}
		if(scale == 0)
			scale = regionFontScale;
		if(scale <= fontCullScaleW)
			return;

		TextLayout textLayout = GetTextLayout(text,format);
		if(zBias == -1)
			zBias = zLabels;


		// shift everything, then ignore Z

		//	var atScale = at.ViewToScreen(zBias);
		//	at = atScale.c;
		//	scale *= atScale.scale;
		//	zBias = 0;
		var span = textLayout.ScaledSpan(scale);
		var expand = new Vector2(_expandX.DipToWorld(),_expandY.DipToWorld());
		if(backgroundColor.A > 0) {
			var c0 = at;
			if(format.horizontalAlignment == TextFormat.HorizontalAlignment.center)
				c0.X -= span.X * 0.5f;
			if(format.horizontalAlignment == TextFormat.HorizontalAlignment.right)
				c0.X -= span.X;
			if(format.verticalAlignment == TextFormat.VerticalAlignment.center)
				c0.Y -= span.Y * 0.5f;
			if(format.verticalAlignment == TextFormat.VerticalAlignment.bottom)
				c0.Y -= span.Y;
			backgroundColor.A = (byte)(((int)backgroundColor.A * color.A) / 255);
			FillRoundedRectangle(Layer.textBackground,c0 - expand,c0 + expand + span,backgroundColor,zBias);
			Assert(layer > Layer.textBackground);
		}
		textLayout.Draw(at,scale,color,layer,zBias);
	}

	private static void FillRoundedRectangle(int layer,Vector2 c0,Vector2 c1,Color background,float z) {
		draw.AddQuad(layer,quadTexture,c0,c1,background,z);/// c0.CToDepth(),(c1.X,c0.Y).CToDepth(), (c0.X,c1.Y).CToDepth(), c1.CToDepth() );
	}
	//	static Vector2 _uv0;
	//	static Vector2 _uv1;
	private static void DrawFlag(int cid,SpriteAnim sprite,Vector2 offset) {
		//return;
		//var wc = cid.CidToWorld();
		//if(IsCulledWC(wc))
		//	return;

		//var c = wc.ToVector() + offset.DipToWorldOffset();
		//var dv = (shapeSizeGain * 48 * 4 * Settings.flagScale);
		//float z = zEffects;

		//// hover flags
		//if(TryGetViewHover(cid,out var dz))
		//{
		//	c.Y -= dz * (5.0f * shapeSizeGain); // 8 pixels up regardless of scale
		//	z += dz * viewHoverZGain;
		//}
		//double frameCount = sprite.frameCount;
		//double frameTotal = ((animationT + cid.CidToRandom() * 15.0) * 12.0);
		//var frameWrap = frameTotal % frameCount;

		//var frameI = Math.Floor(frameWrap);
		//var frameMod = (frameWrap - frameI) * 255.0 + 0.325;
		//Assert(frameMod >= 0);
		//Assert(frameMod < 256.0f);
		//var blend = (int)(frameMod);
		////	_blend = blend;
		//var c0 = new Vector2(c.X,c.Y - dv * 0.435f * 0.75f);
		//Vector2 c1 = new Vector2(c.X + dv * 0.5f * 0.75f,c.Y - dv * 0.035f * 0.75f);
		//var du = 1.0/frameCount;

		//var uv0 = new Vector2((float)(frameI / frameCount),0.0f);
		//var uv1 = new Vector2((float)((frameI + 1.0f) / frameCount),1.0f);
		////	_uv0 = uv0;
		////	_uv1 = uv1;
		//var material = sprite.material;
		//byte alpha = 255;
		//draw.AddQuad(Layer.effects,material,c0,c1,
		//				new Vector2((float)(du*frameI),sprite.frameDeltaU),
		//				new Vector2((float)(du*(frameI+1)),sprite.frameDeltaU),
		//				new(blend,byte.MaxValue,(byte)0,alpha),
		//				new(blend,byte.MaxValue,(byte)0,alpha),
		//				new(blend,byte.MaxValue,byte.MaxValue,alpha),
		//				new(blend,byte.MaxValue,byte.MaxValue,alpha),
		//				depth: z);
		////draw.AddQuad(Layer.effects, sprite.material, c0.CameraToWorldPosition(), c1.CameraToWorldPosition(),
		////	uv0,
		////	uv1,
		////	new Color(blend, sprite.frameDeltaG, sprite.frameDeltaB, 255), depth:z );
	}



	//private static Color GetAttackColor(Army attack)
	//{
	//	return attack.type switch
	//	{
	//		reportAssault => new
	//		reportSiege => new Color(255 / 2, 0xcf / 2, 0x50 / 2, 0x07 / 2),
	//		reportSieging => new Color(192, 0xc5 / 2, 0x7f / 2, 0x4a / 2),
	//		reportPlunder => new Color(255 / 2, 0x28 / 2, 0x86 / 2, 0xc0 / 2),
	//		reportScout => new Color(255 / 2, 0xc8 / 2, 0x2d / 2, 0xbf / 2),

	//		_ => defaultAttackColor
	//	};
	//}

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

	const float actionStopDistance = 0.375f;
	private static void DrawAction(float timeToArrival,float journeyTime,float wave,Vector2 c0,Vector2 c1,Color color,
	Material bitmap,bool applyStopDistance,IToolTip? army,float alpha = 1,float lineThickness = GameClient.lineThickness,bool highlight = false) {
		if(IsSegmentCulledWC(c0,c1))
			return;
		var lineRate = lineAnimationRate;
		var animWave = false;
		float progress;
		if(timeToArrival <= 0.0f) {
			progress = 1.0f;
			timeToArrival = 0;
			lineRate = lineAnimationRate*0.25f;// (animationT/16.0f).Wave(-lineAnimationRate,lineAnimationRate)*(1f/128.0f);
			animWave = true;
		}
		else {
			if(timeToArrival >= journeyTime)
				progress = 1.0f / 16.0f; // just starting
			else
				progress = 1f - (timeToArrival / journeyTime); // we don't know the duration so we approximate with 2 hours
		}
		if(applyStopDistance) {
			var dr = c1 - c0;
			var _c0 = c0;
			var clip = (actionStopDistance / dr.Length()).Min(0.25f);
			//progress *= (1.0f -  (actionStopDistance / Vector2.Distance(c0,c1)).Min(0.25f));
			c0 = _c0 + dr*clip;
			c1 = _c0 + dr*(1f-clip);
			//c1  = c0 + (c1 - c0)* stop;
		}

		var gain = 1.0f;
		if(timeToArrival < postAttackDisplayTime && timeToArrival > 0)
			gain = 1.0f + (1.0f - timeToArrival / postAttackDisplayTime) * 0.25f;



		float spriteSize = spriteSizeGain;

		if(army is not null) {
			(var distance, _) = ShellPage.mousePositionW.DistanceToSegment(c0,c1);
			//ToolTips.debugTip = distance.ToString("N0");
			if(distance < bestUnderMouseScore) {
				bestUnderMouseScore = distance;
				underMouse = army;
				highlight = true;
				spriteSize *= 1.25f;
			}
		}
		if(highlight)
			lineThickness *= 1.25f;
		var shadowColor = ShadowColor(alpha,highlight);
		if(wantShadow)
			DrawLine(Layer.effectShadow,c0,c1,GetLineUs(c0,c1,lineThickness,lineRate,animWave),shadowColor,zEffectShadow,thickness: lineThickness);

		DrawLine(Layer.lineOverlay,c0,c1,GetLineUs(c0,c1,lineThickness,lineRate,animWave),color,zEffects,thickness: lineThickness);
		//if(applyStopDistance)
		//{
		//	DrawSquare(Layer.action + 3,c0,color,zEffects);
		//	if( wantShadow)
		//		DrawSquare(Layer.effectShadow,c0,shadowColor,zEffectShadow);
		//}
		//var dc = new Vector2(spriteSize, spriteSize);
		if(bitmap != null) {
			var mid = progress.Lerp(c0,c1);
			var _c0 = new Vector2(mid.X - spriteSize,mid.Y - spriteSize);
			var _c1 = new Vector2(mid.X + spriteSize,mid.Y + spriteSize);
			draw.AddQuadWithShadow(Layer.lineIconOverlay,Layer.effectShadow,bitmap,_c0,_c1,HSLToRGB.ToRGBA(wave,0.3f,gain * 1.1875f,alpha),shadowColor,zEffects);
		}
		//            ds.DrawRoundedSquare(midS, rectSpan, color, 2.0f);


	}




	private void DrawAction(Vector2 c0,Vector2 c1,Color color,float thickness,bool highlight) {
		DrawAction(0,1.0f,1,c0,c1,color,null,false,null,alpha: 1,lineThickness: thickness,highlight: highlight);


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
	private static void DrawSquare(int layer,Vector2 c0,Color color,float zBias) {
		draw.AddQuad(layer,quadTexture,new Vector2(c0.X - smallRectSpan,c0.Y - smallRectSpan),new Vector2(c0.X + smallRectSpan,c0.Y + smallRectSpan),new Vector2(0,0),new Vector2(1,1),color,zBias);
	}
	private static void DrawRect(int layer,Vector2 c0,Vector2 c1,Color color,float Z) {
		draw.AddQuad(layer,quadTexture,c0,c1,new Vector2(),new Vector2(1,1),color,zEffects);
	}
	private static void DrawRectOutline(int layer,Vector2 c0,Vector2 c1,Color color,float z,float thickness,float _expand = 0f,double animationOffset = 0,bool includeTop = true) {

		float t = thickness.ScreenToWorld();
		var expand = _expand.ScreenToWorld();
		const float waveGain = 0.5f;
		z = z * (0.125f+ ((animationT-animationOffset)*(1.0/3)).Wave() * waveGain);
		c0 = new(c0.X - expand,c0.Y - expand);
		c1 = new(c1.X + expand,c1.Y + expand);

		if(includeTop)
			draw.AddQuad(layer,quadTexture,new(c0.X,c0.Y-t),new(c1.X,c0.Y+t),new Vector2(),new Vector2(1,1),color,z);

		draw.AddQuad(layer,quadTexture,new(c0.X,c1.Y -t),new(c1.X,c1.Y +t),new Vector2(),new Vector2(1,1),color,z);

		draw.AddQuad(layer,quadTexture,new(c0.X- t,c0.Y),new(c0.X +t,c1.Y),new Vector2(),new Vector2(1,1),color,z);

		draw.AddQuad(layer,quadTexture,new(c1.X-t,c0.Y),new(c1.X+t,c1.Y),new Vector2(),new Vector2(1,1),color,z);
	}


	internal static void DrawRectOutlineShadow(int layer,Vector2 c0,Vector2 c1,Color color,float thickness = 1,float expand = 0,double animationOffset = 0,float zScale = 1f,bool includeTop = true) {
		DrawRectOutline(layer,c0,c1,color,zUI*zScale,thickness,expand,animationOffset,includeTop);
		DrawRectOutline(Layer.tileShadow,c0,c1,color.GetShadowColorDark(),zShadow,thickness,expand,animationOffset,includeTop);
	}
	private static void DrawRectOutlineShadow(int layer,int cid,Color col,string label = null,float thickness = 1,float expand = 0,double animationOffset = 0,bool includeTop = true) {
		var wc = cid.CidToWorld();
		if(IsCulledWC(wc))
			return;
		var cc = wc.ToVector(); ;
		var c0 = cc - v2Half;
		var c1 = cc + v2Half;
		DrawRectOutlineShadow(Layer.effects,c0,c1,col,thickness,expand,animationOffset,includeTop: includeTop);
		if(label != null) {
			DrawTextBox(label,cc,textformatLabel,new Color(col,255),textBackgroundOpacity,Layer.tileText,scale: (regionFontScale * 2.0f));
		}
	}

	private static void DrawDiamond(Material lineMaterial,int layer,Vector2 c0,Vector2 c1,Color color,float z,float thickness,float expand) {
		float d = thickness;
		var cm = (c0 + c1) * 0.5f;
		float ext = 0.41f + expand.ScreenToWorld();
		float ext1 = 1.0f + ext;
		var ct = new Vector2(cm.X,c0.Y * ext1 - cm.Y * ext);
		var cb = new Vector2(cm.X,c1.Y * ext1 - cm.Y * ext);
		var cl = new Vector2(c0.X * ext1 - cm.X * ext,cm.Y);
		var cr = new Vector2(c1.X * ext1 - cm.X * ext,cm.Y);
		draw.AddLine(layer,lineMaterial,cl,ct,d,0.0f,1.0f,color,(cl.CToDepth(z), ct.CToDepth(z)));
		draw.AddLine(layer,lineMaterial,ct,cr,d,0.0f,1.0f,color,(ct.CToDepth(z), cr.CToDepth(z)));
		draw.AddLine(layer,lineMaterial,cr,cb,d,0.0f,1.0f,color,(cr.CToDepth(z), cb.CToDepth(z)));
		draw.AddLine(layer,lineMaterial,cb,cl,d,0.0f,1.0f,color,(cb.CToDepth(z), cl.CToDepth(z))); ;
	}
	private static void DrawDiamondShadow(int layer,Vector2 c0,Vector2 c1,Color color,float thickness,float expand) {
		DrawDiamond(whiteMaterial,layer,c0,c1,color,zCities,thickness,expand);
		if(wantParallax)
			DrawDiamond(shadowMaterial,Layer.tileShadow,c0,c1,color.GetShadowColorDark(),zCities,thickness,expand);
	}
	private static void DrawDiamondShadow(int layer,int cid,Color col,string label = null,float thickness = 3,float expand = 0) {
		var wc = cid.CidToWorld();
		if(IsCulledWC(wc))
			return;
		var ww = wc.ToVector();
		var c0 = ww - v2Half;
		var c1 = ww + v2Half;
		DrawDiamondShadow(Layer.effects,c0,c1,col,thickness,expand);
		if(label != null) {
			DrawTextBox(label,ww,textformatLabel,new Color(col,255),textBackgroundOpacity,Layer.tileText,scale: (regionFontScale * 2.0f));
		}
	}

	static (float u, float v) GetLineUs(Vector2 c0,Vector2 c1,float thickness,float animationSpeed,bool wave) {
		var offset = wave ? (animationSpeed * animationT).Wave() : (animationSpeed * animationT).Wrap01();
		return (offset, offset-(c0 - c1).Length()* lineWToUs/thickness);

	}
	private static void DrawLine(int layer,Vector2 c0,Vector2 c1,(float u, float v) uv,Color color,float zBias,float thickness = lineThickness) {
		//	draw.AddLine(layer,lineDraw, c0, c1, lineThickness, , color,(c0.CToDepth()+ zBias, c1.CToDepth()+ zBias) );
		draw.AddLine(layer,lineDraw,c0,c1,thickness,uv.u,uv.v,color,(c0.CToDepth(zBias), c1.CToDepth(zBias)));
	}



	public static void DrawAccentBaseI(float cX,float cY,float radius,double angle,Color color,int layer,float zBias) {
		var dx0 = radius * (float)Math.Cos(angle);
		var dy0 = radius * (float)Math.Sin(angle);
		var angle1 = angle + MathF.PI * 0.1875f;
		var dx1 = radius * (float)Math.Cos(angle1);
		var dy1 = radius * (float)Math.Sin(angle1);
		DrawLine(layer,new Vector2(cX + dx0,cY + dy0),new Vector2(cX + dx1,cY + dy1),(angle.SignOr0(), 0),color,zBias);
		// rotated by 180
		DrawLine(layer,new Vector2(cX - dx0,cY - dy0),new Vector2(cX - dx1,cY - dy1),(angle.SignOr0(), 0),color,zBias);
	}
	public static void DrawAccentBase(float cX,float cY,float radius,double angle,Color color,int layer,float zBias) {
		DrawAccentBaseI(cX,cY,radius,angle,color,layer,zBias);
		DrawAccentBaseI(cX,cY,radius * 0.875f,angle + angle.SignOr0() * 0.125f,color,layer,zBias);
		//DrawAccentBaseI(ds, cX, cY, radius*0.655f, angle+angle.SignOr0()*0.25f, color);
	}

	public static void DrawAccent(Vector2 c,float radius,float angularSpeed,Color brush) {
		var angle = angularSpeed * AGame.animationT;
		if(wantShadow)
			DrawAccentBase(c.X,c.Y,radius,angle,brush.GetShadowColorDark(),Layer.effectShadow,zEffectShadow);
		DrawAccentBase(c.X,c.Y,radius,angle,brush,Layer.overlay2d,zEffects);
	}
	public static void DrawAccent(int cid,float angularSpeedBase,Color brush,float radiusScale = 1.0f) {

		var wc = cid.CidToWorld();
		if(IsCulledWC(wc))
			return;

		//	var c = wc.WorldToCamera();

		var rnd = cid.CidToRandom();

		var angularSpeed = angularSpeedBase + rnd * 0.5f;
		var t = (AGame.animationT * rnd.Lerp(1.25f / 256.0f,1.75f / 256f));
		var r = t.Wave().Lerp(GameClient.circleRadiusBase,GameClient.circleRadiusBase * 1.375f)* radiusScale;
		DrawAccent(wc.ToVector(),r,angularSpeed,brush);
	}


	public static void UpdateRenderQuality(float renderQuality) {
		UpdateClientSpan();
	}


	public static List<(int cid, float z, float vz)> viewHovers = new List<(int cid, float z, float vz)>();
	static bool TryGetViewHover(int cid,out float z) {
		foreach(var hover in viewHovers) {
			if(hover.cid == cid) {
				z = hover.z;
				return true;
			}
		}
		z=0;
		return false;
	}

	protected override void Update(GameTime gameTime) {

		try {





			if(clientSpan.X > 0 && clientSpan.Y > 0 && AppS.isForeground) {
				if(resolutionDirtyCounter > 0 && !faulted) {
					if(--resolutionDirtyCounter == 0) {
						wantFastRefresh = false;
						//	AppS.DispatchOnUIThread(() =>
						{
							//									_graphics.PreferredBackBufferHeight = (int)clientSpan.Y;
							//								_graphics.PreferredBackBufferWidth = (int)clientSpan.X;
							//					_graphics.ApplyChanges();
							//_graphics.PreferredBackBufferFormat = GetBackBufferFormat();
							var pre = GameClient.instance.GraphicsDevice.PresentationParameters;

							{
								pre.BackBufferFormat =  GetBackBufferFormat();
								pre.MultiSampleCount = 0;
								pre.DepthStencilFormat = DepthFormat.Depth16;
								pre.BackBufferWidth = (int)(clientSpan.X*dipToNative*resolutionScale);// - ShellPage.cachedXOffset,
								pre.BackBufferHeight = (int)(clientSpan.Y*dipToNative*resolutionScale); // - ShellPage.cachedTopOffset,
							};

							if(!wantDeviceReset)
								GameClient.instance.GraphicsDevice.OnPresentationChanged();
							else
								GameClient.instance.GraphicsDevice.Reset();

							wantDeviceReset = false;
						}
						//);
					}
				}
			}




			//	priorMouseState = mouseState;
			//	priorKeyboardState = keyboardState;
			//	keyboardState = Keyboard._nextKeyboardState;
			//	App.canvasKeyModifiers = UWindows.System.VirtualKeyModifiers.None;
			//if ((keyboardState.IsKeyDown(Keys.LeftShift) | keyboardState.IsKeyDown(Keys.RightShift)))
			//	App.canvasKeyModifiers |= UWindows.System.VirtualKeyModifiers.Shift;
			//if ((keyboardState.IsKeyDown(Keys.LeftControl) | keyboardState.IsKeyDown(Keys.RightControl)))
			//	App.canvasKeyModifiers |= UWindows.System.VirtualKeyModifiers.Control;

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
			//ShellPage.Canvas_PointerPressed(mouseState, priorMouseState);

			//			if(AppS.isForeground)
			//			{
			//				if(renderFrame >= 5 && ShellPage.coreInputSource==null)
			//				{
			////					ShellPage.SetupCoreInput();

			//				}
			//				//else
			//				//{
			//				////	ShellPage.coreInputSource.Dispatcher.CurrentPriority = CoreDispatcherPriority.Low;
			//				//	//ShellPage.coreInputSource.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);
			//				//	//ShellPage.Gesture.Tick();
			//				//}

			//			}
			if(!faulted && AppS.isForeground) {
				if(World.bitmapPixels != null) {
					// canvas.Paused = true;
					var pixels = World.bitmapPixels;

					World.bitmapPixels = null;
					if(worldObjects != null) {
						var w = worldObjects;
						worldObjects = null;
						w.texture.Dispose();
					}
					worldObjects = CreateFromBytes(pixels,World.outSize,World.outSize,SurfaceFormat.Dxt1SRgb,defaultEffect);
				}
				//if(World.renderTileStaging != null)
				//{
				//	var pixels = World.renderTileStaging;
				//	World.renderTileStaging= null;
				//	var prior = World.renderTilesTexture;
				//	World.renderTilesTexture = null;
				//	if(prior is not null)
				//		prior.Dispose();
				//	World.renderTilesTexture = CreateFromBytes(World.renderTileStaging,World.span,World.span,SurfaceFormat.R32G32B32A32_SInt);

				//}
				if(World.worldOwnerPixels != null) {
					var ownerPixels = World.worldOwnerPixels;
					World.worldOwnerPixels = null;
					if(worldOwners != null) {
						var w = worldOwners;
						worldOwners = null;
						w.texture.Dispose();
					}
					worldOwners = CreateFromBytes(ownerPixels,World.outSize,World.outSize,SurfaceFormat.Dxt1SRgb,defaultEffect);
					//canvas.Paused = falwirse;
					//if (worldObjectsDark != null)
					//    worldObjectsDark.Dispose();
					//worldObjectsDark = new TintEffect() { BufferPrecision = CanvasBufferPrecision.Precision8UIntNormalizedSrgb, Source = worldObjects, Color = new Color() { A = 255, R = 128, G = 128, B = 128 } };

				}
				if(World.changePixels != null) {
					var pixels = World.changePixels;

					ClearHeatmapImage();
					//	worldChanges = CreateFromBytes(pixels,World.outSize,World.outSize,SurfaceFormat.Dxt1,worldSpaceEffect);


				}
			}
			//if(CnVServer.webViewBrush!=null)
			//	AppS.DispatchOnUIThread(
			//		() =>
			//		{
			//			CnVServer.webViewBrush.SourceName = "cotgView";
			//			CnVServer.webViewBrush.SetSource(CnVServer.view);
			//			CnVServer.webViewBrush.Redraw();
			//			ShellPage.canvasHitTest.Fill = CnVServer.webViewBrush;
			//		});
		}
		catch(SharpDX.SharpDXException sex) {
			if(!faulted) {
				faultCount += 16;
				if(faulted) {
					try {
						CnV.Debug.LogEx(sex,report: false);
						CnV.Debug.Log($"{sex.ResultCode} {sex.Descriptor.ApiCode} {sex.Descriptor.Description} {sex.Descriptor.ToString()} ");
						Faulted();

					}
					catch(Exception ex2) {

					}
				}
			}

		}
		catch(Exception _exception) {
			CnV.Debug.LogEx(_exception);
			++faultCount;
		}



	}

	private static SurfaceFormat GetBackBufferFormat() => Settings.hdrMode switch { 0 => SurfaceFormat.Bgra32SRgb, 1 => SurfaceFormat.Rgba1010102, _ => SurfaceFormat.HalfVector4 };

	private static async Task Faulted() {
		var a = await AppS.DispatchOnUIThreadTask(async () => {
			return await AppS.Failed("Video Driver broke","Please restart, it should recover");

		});
		if(a == 1) {
			instance.GraphicsDevice.Reset();
			faultCount = 0;
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
	protected override bool BeginDraw() {
		if(!AppS.isForeground)
			return false;
		if(!Sim.isInitialized)
			return false;
		if(faulted)
			return false;
		if(!AGame.contentLoadingComplete)
			return false;
		if(!AppS.isStateActive)
			return false;

		if(Sim.isWarmup)
			return false;
		if(TilesReady())
			World.UpdateTileDatas();
		return base.BeginDraw();
	}

}


