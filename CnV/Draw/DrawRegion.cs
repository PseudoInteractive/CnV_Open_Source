﻿using static CnV.AGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;
using static CnV.View;
using CnV.Draw;

//using Windows.UI.Core;
using static CnV.Troops;
using static CnV.CanvasHelpers;


using UWindows = Windows;
using XVector2 = Microsoft.Xna.Framework.Vector2;
using XVector3 = Microsoft.Xna.Framework.Vector3;
using XVector4 = Microsoft.Xna.Framework.Vector4;
using Layer = CnV.Draw.Layer;
using static CnV.View;
using static CnV.AGame;
using KeyF = CnV.KeyFrame<float>;

namespace CnV;

internal partial class GameClient
{
	public static TextFormat textformatLabel = new TextFormat(TextFormat.HorizontalAlignment.center, TextFormat.VerticalAlignment.center);
	private TextFormat tipTextFormatCentered = new TextFormat(TextFormat.HorizontalAlignment.center);
	private TextFormat tipTextFormat = new TextFormat(TextFormat.HorizontalAlignment.left);
	private TextFormat tipTextFormatRight = new TextFormat(TextFormat.HorizontalAlignment.right);
	private TextFormat nameTextFormat = new TextFormat(TextFormat.HorizontalAlignment.center, TextFormat.VerticalAlignment.center);
	const byte textBackgroundOpacity = 192;

	public static Material worldBackground;
	//public static Effect imageEffect;
	public static Effect   avaEffect;
	public static Span2i[] popups = Array.Empty<Span2i>();

	//    public static TintEffect worldBackgroundDark;
	public static Material worldObjects;
	public static Material worldOwners;

	public static VertexBuffer tesselatedWorldVB;
	public static IndexBuffer tesselatedWorldIB;
	static KeyF[] bulgeKeys = new[] { new KeyF(0.0f, 0.0f), new KeyF(0.44f, 0.44f), new KeyF(1.5f, 0.44f), new KeyF(2.5f, 0.0f) };

	private static bool TilesReady()
	{
		return (TileData.state >= TileData.State.ready);
	}

	private const int   textBoxCullSlop = 80;
	static        byte  clearCounter    = 10;

	public static bool  tileSetsPending;
	private const float smallRectSpan = 4;
	public const  float lightZ0       = 460f;
	public const  float lightZDay     = 550;
	//public static Vector2 cameraLightC;
	static SamplerState fontFilter = new SamplerState()
	{
			Filter                  = TextureFilter.Linear,
			MipMapLevelOfDetailBias = -1.5f,
			BorderColor             = new Color(0, 0, 0, 0),
			MaxAnisotropy           = 2,
			AddressW                = TextureAddressMode.Border,
			AddressU                = TextureAddressMode.Border,
			AddressV                = TextureAddressMode.Border,
	};
	static        int                   filterCounter;
	public static Material              fontMaterial;
	public static Material              darkFontMaterial;
	public static BitmapFont.BitmapFont bfont;
	const         float                 lineTileGain = 1.5f / 64.0f;

	const float actionAnimationGain = 64.0f;
	const float drawActionLength    = 32;
	const float lineAnimationGain   = 2.0f;

	class IncomingCounts
	{
		public int prior;
		public int incoming;
	};

	const float postAttackDisplayTime = 15 * 60; // 11 min

	const  float   lineThickness        = 3.0f;
	const  float   circleRadMin         = 3.0f;
	const  float   circleRadMax         = 5.5f;
	static Vector2 shadowOffset         = new Vector2(4, 4);
	const  float   detailsZoomThreshold = 28;
	const  float   detailsZoomFade      = 4;

	//	static float LineThickness(bool hovered) => hovered ? lineThickness * 2 : lineThickness;
	const float   rectSpanMin  = 4.0f;
	const         float   rectSpanMax  = 8.0f;
	const         float   bSizeGain    = 4.0f;
	const         float   bSizeGain2   = 4; //4.22166666666667f;
	const         float   srcImageSpan = 2400;
	const         float   bSizeGain3   = bSizeGain * bSizeGain / bSizeGain2;
	public static float   pixelScale   = 1;
	public static Vector2 halfSquareOffset;
	public static float   circleRadiusBase = 1.0f;
	public static float   shapeSizeGain    = 1.0f;
	public static float   bulgeSpan => 1.0f + bulgeNegativeRange;
	public static float   bulgeGain           = 0;
	public static float   pixelScaleInverse   = 1;
	public static float   clampedScaleInverse = 1;
	//	const float dashLength = (dashD0 + dashD1) * lineThickness;
	public static Draw.SpriteBatch draw;

	static        Army             underMouse;
		static    float            bestUnderMouseScore;
		//   public static Vector2 cameraMid;
		public static float eventTimeOffsetLag; // smoothed version of event time offset
		public static float eventTimeEnd;
		static public Color nameColor, nameColorHover, myNameColor, nameColorOutgoing, nameColorIncoming, nameColorSieged, nameColorIncomingHover, nameColorSiegedHover, myNameColorIncoming, myNameColorSieged;

		protected override void Draw(GameTime gameTime)
		{
			if(faulted)
				return;
			if(!AppS.isForeground)
				return;
			++renderFrame;
			underMouse = null;
			bestUnderMouseScore = 8;


			//parallaxZ0 = 1024 * 64.0f / cameraZoomLag;
			var isFocused = CnVServer.isInitialized&&AppS.isForeground;

			try
			{
				//		var _serverNow = CnVServer.ServerTime();
				var dt = (float)gameTime.ElapsedGameTime.TotalSeconds; // max delta is 1s
																	   //	lastDrawTime = _serverNow;

				var gain = 1.0f - MathF.Exp(-4.0f * dt);
				cameraCLag += (View.cameraC - cameraCLag) * gain;
				cameraZoomLag += (cameraZoom - cameraZoomLag) * gain;
				//cameraLightC = (ShellPage.mousePositionC);
				//                cameraZoomLag += (cameraZoom
				// smooth ease towards target
				eventTimeOffsetLag += (ShellPage.instance.eventTimeOffset - eventTimeOffsetLag) * gain;
				var serverNow = CnVServer.ServerTime() + TimeSpan.FromMinutes(eventTimeOffsetLag);

				// not too high or we lose float precision
				// not too low or people will see when when wraps
				animationT = (float)gameTime.TotalGameTime.TotalSeconds;// ((uint)Environment.TickCount % 0xffffff) * (1.0f / 1000.0f);

				//{
				//	var i = (int)(animationT / 4.0f);
				//	var stretchCount = FontStretch.UltraExpanded - FontStretch.Condensed+1;
				//	fontStretch = FontStretch.Condensed + (i % stretchCount);
				//	tipTextFormat.FontStretch = fontStretch;
				//	tipTextFormatCentered.FontStretch = fontStretch;
				//}
				animationTWrap = (animationT*0.333f).Frac(); // wraps every 3 seconds, 0..1

				device.Textures[7] = fontTexture;
				//				float accentAngle = animT * MathF.PI * 2;
				int tick = (Environment.TickCount >> 3) & 0xfffff;
				var animTLoop = animationTWrap.Wave();
				int cx0 = 0, cy0 = 0, cx1 = 0, cy1 = 0;
				var rectSpan = animTLoop.Lerp(rectSpanMin, rectSpanMax);


				//   ShellPage.T("Draw");

				//	defaultStrokeStyle.DashOffset = (1 - animT) * dashLength;


				//                ds.Blend = ( (int)(serverNow.Second / 15) switch { 0 => CanvasBlend.Add, 1 => CanvasBlend.Copy, 2 => CanvasBlend.Add, _ => CanvasBlend.SourceOver } );




				//ds.TextRenderingParameters = new CanvasTextRenderingParameters(!AppS.IsKeyPressedControl() ? CanvasTextRenderingMode.Outline : CanvasTextRenderingMode.Default, CanvasTextGridFit.Default);

				//              ds.TextRenderingParameters = new CanvasTextRenderingParameters(CanvasTextRenderingMode.Default, CanvasTextGridFit.Disable);
				// var scale = ShellPage.canvas.ConvertPixelsToDips(1);
				pixelScale       = (cameraZoomLag);
				halfSquareOffset = new System.Numerics.Vector2(pixelScale * 0.5f, pixelScale * .5f);
				var bonusLayerScale = pixelScale.Max(64 * Settings.iconScale);

				bmFontScale = (MathF.Sqrt(pixelScale / 64.0f) * 0.5f * Settings.fontScale);//.Min(0.5f);
				pixelScaleInverse = 1.0f / cameraZoomLag;
				clampedScaleInverse = (64 * pixelScaleInverse).Min(4.0f);
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


				bulgeGain *= Settings.planet * (1.0f - cityAlpha);
				bulgeInputGain = 4*(0.75f.Squared()) / (virtualSpan.X.Squared() + virtualSpan.Y.Squared());
				// world space coords
				var srcP0 = new System.Numerics.Vector2((cameraCLag.X + 0.5f) * bSizeGain2 - projectionC.X * bSizeGain2 * pixelScaleInverse,
														(cameraCLag.Y + 0.5f) * bSizeGain2 - projectionC.Y * bSizeGain2 * pixelScaleInverse);
				var srcP1 = new System.Numerics.Vector2(srcP0.X + clientSpan.X * bSizeGain2 * pixelScaleInverse,
														srcP0.Y + clientSpan.Y * bSizeGain2 * pixelScaleInverse);
				var destP0 = clip.c0;
				var destP1 = clip.c1;

				if(srcP0.X < 0)
				{
					destP0.X -= srcP0.X * pixelScale / bSizeGain2;
					srcP0.X = 0;
				}
				if(srcP0.Y < 0)
				{
					destP0.Y -= srcP0.Y * pixelScale / bSizeGain2;
					srcP0.Y = 0;
				}
				if(srcP1.X > srcImageSpan)
				{
					destP1.X += (srcImageSpan - srcP1.X) * pixelScale / bSizeGain2;
					srcP1.X = srcImageSpan;

				}
				if(srcP1.Y > srcImageSpan)
				{
					destP1.Y += (srcImageSpan - srcP1.Y) * pixelScale / bSizeGain2;
					srcP1.Y = srcImageSpan;

				}
				var isWinter = Settings.IsThemeWinter();
				//				var attacksVisible = DefenseHistoryTab.IsVisible() | OutgoingTab.IsVisible() | IncomingTab.IsVisible() | HitTab.IsVisible() | AttackTab.IsVisible();
				var attacksVisible = OutgoingTab.IsVisible() | IncomingTab.IsVisible() | AttackTab.IsVisible();


				var wantParallax = Settings.parallax > 0.1f;

				//var gr = spriteBatch;// spriteBatch;// wantLight ? renderTarget.CreateDrawingSession() : args.DrawingSession;

				//		ds.Blend = CanvasBlend.Copy;

				// funky logic
				//if (wantLight)
				if(--clearCounter > 0)
				{
					GraphicsDevice.Clear(new Color(0, 0, 0, 0)); // black transparent
				}
				//								   //ds.TextAntialiasing = canvasTextAntialiasing;
				//								   //ds.TextRenderingParameters = canvasTextRenderingParameters;
				//								   // prevent MSAA gaps
				GraphicsDevice.BlendState = BlendState.AlphaBlend;
				//	GraphicsDevice.DepthStencilState = DepthStencilState.None;

				//			GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
				//			GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
				if(fontFilter != null)
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

					//var projCX = projectionC.X / clientSpan.X.Max(1);
					//var projCY = projectionC.Y / clientSpan.Y.Max(1);
					var proj = Matrix.CreateLookAt(new Microsoft.Xna.Framework.Vector3(0, 0, cameraZ),
						Microsoft.Xna.Framework.Vector3.Zero, Microsoft.Xna.Framework.Vector3.Up) *
							   Matrix.CreatePerspectiveOffCenter(
								   -projectionC.X*0.5f, (viewport.Width-projectionC.X)*0.5f,
								   (viewport.Height-projectionC.Y)*0.5f, -projectionC.Y*0.5f,
									  0.5f, 1.5f);
					//					var proj = Matrix.CreateOrthographicOffCenter(480, 1680, 1680, 480, 0, -1);
					worldMatrixParameter.SetValue(proj);

					if(Settings.lighting == Lighting.night)
					{
						var l = ShellPage.mousePosition;//.InverseProject();
						lightPositionParameter.SetValue(new Microsoft.Xna.Framework.Vector3(l.X, l.Y, lightZ0));
						lightGainsParameter.SetValue(new Microsoft.Xna.Framework.Vector4(0.25f, 1.25f, 0.375f, 1.0625f));
						lightAmbientParameter.SetValue(new Microsoft.Xna.Framework.Vector4(.493f, .576f, .639f, 1f) * 0.25f);
						lightColorParameter.SetValue(new Microsoft.Xna.Framework.Vector4(1.0f, 1.0f, 1.0f, 1.0f) * 1.25f);
						lightSpecularParameter.SetValue(new Microsoft.Xna.Framework.Vector4(1.0f, 1.0f, 1.0f, 1.0f) * 1.25f);
					}
					else
					{
						var cc = lightWC.WorldToCamera().CameraToScreen();

						lightPositionParameter.SetValue(new Microsoft.Xna.Framework.Vector3(cc.X, cc.Y, lightZDay));
						lightGainsParameter.SetValue(new Microsoft.Xna.Framework.Vector4(0.25f, 1.25f, 0.375f, 1.0625f));
						lightAmbientParameter.SetValue(new Microsoft.Xna.Framework.Vector4(.483f, .476f, .549f, 1f) * 0.75f);
						lightColorParameter.SetValue(new Microsoft.Xna.Framework.Vector4(1.1f, 1.1f, 0.9f, 1f) * 1.25f);
						lightSpecularParameter.SetValue(new Microsoft.Xna.Framework.Vector4(1.0f, 1.0f, 1.0f, 1.0f) * 1.25f);
					}
					cameraReferencePositionParameter.SetValue(new Microsoft.Xna.Framework.Vector3(projectionC.X, projectionC.Y, lightZ0));
					//					defaultEffect.Parameters["DiffuseColor"].SetValue(new Microsoft.Xna.Framework.Vector4(1, 1, 1, 1));
					var gain1       = bulgeInputGain * bulgeGain * bulgeSpan;
					var planetGains = new Microsoft.Xna.Framework.Vector4(bulgeGain, -gain1, MathF.Sqrt(gain1) * 2.0f, 0);
					planetGainsParamater.SetValue(planetGains);

					cameraCParameter.SetValue(new System.Numerics.Vector4(cameraCLag.X, cameraCLag.Y, 0, 1.0f));
					pixelScaleParameter.SetValue(new Vector4(pixelScale, pixelScale, pixelScale, pixelScale));



					draw.Begin();

				}

				var focusOnCity = (View.viewMode == ViewMode.city);

				parallaxGain = Settings.parallax * MathF.Sqrt(Math.Min(11.0f, cameraZoomLag / 64.0f)) * regionAlpha * (1 - cityAlpha);

				{
					var wToCGain = (1.0f / cameraZoomLag);
					//				var halfTiles = (clientSpan * (0.5f / cameraZoomLag));
					var _c0 = cameraCLag + clip.c0*wToCGain;
					var _c1 = cameraCLag + clip.c1*wToCGain;
					cx0 = _c0.X.FloorToInt().Max(0);
					cy0 = (_c0.Y.FloorToInt()).Max(0);
					cx1 = (_c1.X.CeilToInt() + 1).Min(World.span);
					cy1 = (_c1.Y.CeilToInt() + 2).Min(World.span);
				}
				cullWC = new Span2i(cx0, cy0, cx1, cy1);

				{
					//	ds.Antialiasing = CanvasAntialiasing.Aliased;
					if(worldBackground != null && wantImage)
					{
						if(wantImage)
						{
							const byte brightness = 128;
							const byte oBrightness = 255;
							const byte alpha = 255;
							const float texelGain = 1.0f / srcImageSpan;
							draw.AddQuad(CnV.Draw.Layer.background, worldBackground,
								destP0, destP1, srcP0 * texelGain, srcP1 * texelGain,
								 new Color(brightness, brightness, brightness, alpha), ConstantDepth, 0); ;

							if(worldObjects != null)
								draw.AddQuad(CnV.Draw.Layer.background + 1, worldObjects,
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
						nameColorOutgoing = new Color() { A = intAlpha, G = 255, B = 182, R = 222 };
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


					if(TilesReady())
					{
						var td = TileData.instance;

						// 0 == land
						// 1 == shadows
						// 2 == features
						foreach(var layer in td.layers)
						{
							var isBonus = Object.ReferenceEquals(layer, JsonLayer.bonus);
							if(!wantDetails && !isBonus)
								continue;

							var layerDat = layer.data;

							for(var cy = cy0; cy < cy1; ++cy)
							{
								for(var cx = cx0; cx < cx1; ++cx)
								{
									var ccid = cx + cy * World.span;
									var imageId = layerDat[ccid];
									if(imageId == 0)
										continue;


									var cid = (cx, cy).WorldToCid();
									//   var layerData = TileData.packedLayers[ccid];
									//  while (layerData != 0)
									{
										//    var imageId = ((uint)layerData & 0xffffu);
										//     layerData >>= 16;
										var tileId = imageId >> 13;
										var off = imageId & ((1 << 13) - 1);
										var tile = td.tilesets[tileId];

										if(tile.material == null)
											continue;

										for(int isShadow = layer.wantShadow&&wantShadow ? 2 : 1; --isShadow >= 0;)
										{
											if(isShadow == 1 && !tile.wantShadow)
											{
												continue;
											}
											var _tint = (isShadow == 1 && wantParallax) ? tintShadow : !tile.isBase ? World.GetTint(ccid) : tint;
											if(!isBonus && isShadow == 0 && !tile.isBase)
												_tint.A = intAlpha; ;
											if(wantCity && cid == City.build && layer.id > 1)
											{
												if(cityAlphaI >= 255)
													continue;
												_tint.A = (byte)((_tint.A * (256 - cityAlphaI)) / 256);
											}

											var dz = tile.z * parallaxGain; // shadows draw at terrain level 



											if(tile.canHover && !tile.isBase && viewHovers.TryGetValue((aa) => aa.cid == cid, out var z))
											{
												dz += z.z * viewHoverZGain;
											}


											var wc = new System.Numerics.Vector2(cx, cy);
											var cc = wc.WorldToCamera();
											if(isShadow == 1)
											{

												// shift shadow
												//	cc = (cc - cameraLightC) * (1 + dz*2) + cameraLightC;
												//		cc = (cc - cameraLightC)*
												dz = 0;
											}
											var shift = new System.Numerics.Vector2((isBonus ? imageId== TileData.tilePortalOpen ? bonusLayerScale*2f : bonusLayerScale : pixelScale) * 0.5f);
											var cc0 = cc - shift;
											var cc1 = cc + shift;
											var sy = off / tile.columns;
											var sx = off - sy * tile.columns;
											var uv0 = new System.Numerics.Vector2((sx) * tile.scaleXToU + tile.halfTexelU, (sy) * tile.scaleYToV + tile.halfTexelV);
											var uv1 = new System.Numerics.Vector2((sx + 1) * tile.scaleXToU + tile.halfTexelU, (sy + 1) * tile.scaleYToV - tile.halfTexelV);

											draw.AddQuad((isShadow == 1) ? Layer.tileShadow : tile.isBase ? Layer.tileBase + layer.id : Layer.tiles + layer.id,
												(isShadow == 1 ? tile.shadowMaterial : tile.material), cc0, cc1,
												uv0,
												uv1, _tint,
												(dz, dz, dz, dz));
											//(cc0, cc1).RectDepth(dz));




										}
									}
								}

							}

						}
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
					if(worldChanges != null && !focusOnCity)
					{
						var t2d = worldChanges.texture2d;
						//var tOffset = new Vector2(0.0f, 0.0f);
						//var t2d = worldChanges.texture2d;
						//var scale = new Vector2(t2d.TexelWidth, t2d.TexelHeight);
						draw.AddMesh(tesselatedWorld, Layer.tileShadow - 1, worldChanges);
					}
					if(worldOwners != null && !focusOnCity)
					{
						var tOffset = new System.Numerics.Vector2(0.0f, 0.0f);
						var t2d     = worldOwners.texture2d;
						var scale   = new System.Numerics.Vector2(t2d.TexelWidth, t2d.TexelHeight);
						draw.AddQuad(Layer.tiles - 1, worldOwners,
							destP0, destP1,
							(srcP0 - tOffset) * scale, (srcP1 - tOffset) * scale, new Color(128, 128, 128, 128), ConstantDepth, zTerrain);
					}
					if(wantCity)
					{
						// this could use refactoring
						CityView.Draw(cityAlpha);
					}

					circleRadiusBase = circleRadMin * shapeSizeGain * 7.9f;
					var circleRadius = animTLoop.Lerp(circleRadMin, circleRadMax) * shapeSizeGain * 6.5f;
					//    var highlightRectSpan = new Vector2(circleRadius * 2.0f, circleRadius * 2);

					//	ds.FillRectangle(new Rect(0, 0, clientSpan.X, clientSpan.Y), CnVServer.webViewBrush);



					if(!focusOnCity)
					{
						var defenderVisible = IncomingTab.IsVisible() || ReinforcementsTab.instance.isFocused || NearDefenseTab.IsVisible() || Settings.incomingAlwaysVisible;
						var outgoingVisible = OutgoingTab.IsVisible() || Settings.attacksAlwaysVisible;
						{
							//if (DefenseHistoryTab.IsVisible() || HitTab.IsVisible())
							//{
							//	for (var dfof = 0; dfof < 2; ++dfof)
							//	{
							//		if (dfof == 0)
							//		{
							//			if (!DefenseHistoryTab.IsVisible())
							//				continue;
							//		}
							//		else
							//		{
							//			if (!HitTab.IsVisible())
							//				continue;

							//		}
							//		var reports = dfof == 0 ? DefenseHistoryTab.instance.history : HitTab.instance.history;

							//		if (reports.Length > 0)
							//		{
							//			var autoShow = reports.Length <= Settings.showAttacksLimit;

							//			var counts = new Dictionary<int, IncomingCounts>();

							//			foreach (var attack in reports)
							//			{
							//				if (attack.type == COTG.Game.Enum.reportPending)
							//				{
							//					if (dfof == 0)
							//					{
							//						// this will be drawn later, don't repeat
							//						if (defenderVisible)
							//							continue;

							//					}
							//					else
							//					{
							//						// this will be drawn later, don't repeat
							//						if (outgoingVisible)
							//							continue;
							//					}

							//				}
							//				var targetCid = attack.targetCid;
							//				var sourceCid = attack.sourceCid;
							//				if (!(targetCid.TestContinentFilter() | sourceCid.TestContinentFilter()))
							//					continue;

							//				var c1 = targetCid.CidToCamera();
							//				var c0 = sourceCid.CidToCamera();
							//				// cull (should do this pre-transform as that would be more efficient
							//				if (IsCulled(c0, c1))
							//					continue;

							//				var dt1 = attack.TimeToArrival(serverNow);

							//				// before attack
							//				var journeyTime = attack.journeyTime;
							//				{
							//					// register attack
							//					if (!counts.TryGetValue(targetCid, out var count))
							//					{
							//						count = new IncomingCounts();
							//						counts.Add(targetCid, count);
							//					}
							//					if (dt1 > 0)
							//						++count.incoming;
							//					else
							//						++count.prior;
							//				}

							//				if (dt1 >= journeyTime || dt1 < -postAttackDisplayTime)
							//					continue;
							//				if (!Spot.IsSelectedOrHovered(targetCid, sourceCid, autoShow))
							//				{
							//					continue;
							//				}
							//				Color c = GetAttackColor(attack);

							//				{
							//					var t = (tick * sourceCid.CidToRandom().Lerp(1.5f / 512.0f, 1.75f / 512f)) + 0.25f;
							//					var r = t.Ramp();
							//					var nSprite = attack.troops.count;
							//					if (nSprite > 0)
							//					{
							//						(int iType, float alpha) = GetTroopBlend(t, nSprite);
							//						DrawAction(dt1, journeyTime, r, c0, c1, c, troopImages[attack.troops.GetIndexType(iType)], true, attack, alpha: alpha, 
							//							lineThickness:lineThickness,highlight:Spot.IsSelectedOrHovered(a)(attack.sourceCid,attack.targetCid)) );
							//					}
							//				}
							//				//var progress = (dt0 / (dt0 + dt1).Max(1)).Saturate(); // we don't know the duration so we approximate with 2 hours
							//				//var mid = progress.Lerp(c0, c1);
							//				//ds.DrawLine(c0, c1, shadowBrush, lineThickness, defaultStrokeStyle);
							//				//ds.FillCircle(mid, span, shadowBrush);
							//				//var midS = mid - shadowOffset;
							//				//ds.DrawLine(c0 - shadowOffset, midS, raidBrush, lineThickness, defaultStrokeStyle);
							//				//ds.FillCircle(midS, span, raidBrush);
							//			}
							//			foreach (var i in counts)
							//			{
							//				var cid = i.Key;
							//				var count = i.Value;
							//				var wc = cid.CidToWorld();
							//				if (!IsCulledWC(wc))
							//					DrawTextBox($"{count.prior}`{count.incoming}", wc.WToCamera(), textformatLabel, Color.DarkOrange, textBackgroundOpacity, Layer.tileText);


							//			}
							//		}
							//	}
							//}

							if(AttackTab.IsVisible())
							{
								List<AttackTab.AttackCluster> hovered = new();
								if(!AttackTab.attackClusters.IsNullOrEmpty())
								{
									foreach(var cluster in AttackTab.attackClusters)
									{
										if(cluster.attacks.Any(a => Spot.IsSelectedOrHovered(a)) |
														cluster.targets.Any(a => Spot.IsSelectedOrHovered(a)))
										{
											hovered.Add(cluster);
										}
									}
								}

								if(!AttackTab.attackClusters.IsNullOrEmpty())
								{
									var showAll = (AttackTab.attackClusters.Length < Settings.showAttacksLimit) &&(!hovered.Any());
									foreach(var cluster in AttackTab.attackClusters)
									{
										var isHover = hovered.Contains(cluster);
										if(!(showAll || isHover))
											continue;
										{
											var c0 = cluster.topLeft.WorldToCamera();
											var c1 = cluster.bottomRight.WorldToCamera();
											DrawRectOutlineShadow(Layer.effects+2, c0, c1, isHover ? new Color(128, 64, 64, 220) : new Color(64, 0, 0, 162), 5, 2);
										}

										{
											var real = cluster.real;
											var c0 = real.CidToCamera();
											foreach(var a in cluster.attacks)
											{
												var t = (tick * a.CidToRandom().Lerp(1.5f / 512.0f, 1.75f / 512f)) + 0.25f;
												var r = t.Ramp();
												var c1 = a.CidToCamera();
												var spot = Spot.GetOrAdd(a);
												DrawAction(0.5f, 1.0f, r, c1, c0, Color.Red, troopImages[(int)spot.TroopType], false, null,
													lineThickness: lineThickness, highlight: Spot.IsSelectedOrHovered(a));
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
									foreach(var t in AttackPlan.plan.targets)
									{
										var col = t.attackType switch
										{
											AttackType.senator => CColor(168, 0, 0, 255),
											AttackType.senatorFake => CColor(128, 34, 33, 212),
											AttackType.se => CColor(0, 0, 255, 255),
											AttackType.seFake => CColor(98, 32, 168, 212),
											_ => CColor(64, 64, 64, 64)
										};
										DrawRectOutlineShadow(Layer.effects, t.cid, col, (t.attackCluster >= 0) ? t.attackCluster.ToString() : null, 3, -2);
									}
									foreach(var t in AttackPlan.plan.attacks)
									{
										var col = t.attackType switch
										{
											AttackType.assault => assaultColor,
											AttackType.senator => CColor(168, 64, 0, 242),
											AttackType.senatorFake => CColor(128, 48, 0, 192), // not really used as attack
											AttackType.se => CColor(148, 0, 148, 242),
											AttackType.seFake => CColor(128, 32, 128, 192), // not really used as attack
											_ => CColor(64, 64, 64, 64)
										};
										DrawDiamondShadow(Layer.effects, t.cid, col, (t.attackCluster >= 0) ? t.attackCluster.ToString() : null);
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
							if((defenderVisible || outgoingVisible))
							{
								var cullSlopSpace = 80 * pixelScale;
								for(int iOrO = 0; iOrO < 2; ++iOrO)
								{
									var defenders = (iOrO == 0);
									if(defenders)
									{
										if(!defenderVisible)
											continue;
									}
									else
									{
										if(!outgoingVisible)
											continue;
									}
									var list = defenders ? Spot.defendersI : Spot.defendersO;
									bool noneIsAll = list.Length <= Settings.showAttacksLimit;
									bool showAll = list.Length <= Settings.showAttacksLimit0 ||(defenders ? Settings.incomingAlwaysVisible : Settings.attacksAlwaysVisible);
									foreach(var city in list)
									{
										if(!city.testContinentFilter)
											continue;

										if(city.incoming.Any() || city.isMine)
										{

											var targetCid = city.cid;
											var c1 = targetCid.CidToCamera();
											if(IsSquareCulled(c1, cullSlopSpace))  // this is in pixel space - Should be normalized for screen resolution or world space (1 continent?)
												continue;
											var incAttacks = 0;
											var incTs = 0;
											var sieged = false;
											var hasSen = false;
											var hasArt = false;
											var hasAssault = false;
											foreach(var i in city.incoming)
											{
												var c0 = i.sourceCid.CidToCamera();
												if(IsSegmentCulled(c0, c1))
													continue;

												Color c;
												if(i.isDefense)
												{

													if(i.sourceCid == targetCid)
														continue;

													c = i.time <= serverNow ? defenseArrivedColor : defenseColor;
												}
												else
												{
													++incAttacks;
													incTs += i.ts;
													if(i.hasArt)
													{
														hasArt = true;
														c = assaultColor;
													}
													else if(i.hasSenator)
													{
														c = senatorColor; ;
													}
													else
													{
														hasAssault = true;
														c = assaultColor;
													}
													sieged |= i.isSiege;
													hasSen |= i.hasSenator;
												}
												if(!(showAll || Spot.IsSelectedOrHovered(i.sourceCid, targetCid, noneIsAll)))
												{
													//													DrawRectOutlineShadow(Layer.effects, targetCid, c, null, 2, -4f);
													continue;
													//       c.A = (byte)((int)c.A * 3 / 8); // reduce alpha if not selected
												}
												if(i.troops.Any())
												{
													var t = (tick * i.sourceCid.CidToRandom().Lerp(1.5f / 512.0f, 2.0f / 512f)) + 0.25f;
													var r = t.Ramp();
													var nSprite = i.troops.Count;

													(int iType, float alpha) = GetTroopBlend(t, nSprite);

													DrawAction(i.TimeToArrival(serverNow), i.journeyTime, r, c0, c1, c, troopImages[i.troops.GetIndexType(iType)], true, i, alpha: alpha, lineThickness: lineThickness, highlight: Spot.IsSelectedOrHovered(i.sourceCid, i.targetCid));
												}
												else
												{
													Assert(false);
												}
											}
											if(hasArt)
												DrawRectOutlineShadow(Layer.effects - 1, targetCid, artColor, null, 4, -2f);
											if(hasSen)
												DrawRectOutlineShadow(Layer.effects - 1, targetCid, senatorColor, null, 4, -6f);
											if(hasAssault)
												DrawRectOutlineShadow(Layer.effects - 1, targetCid, assaultColor, null, 4, -10f);
											if(sieged)
												DrawRectOutlineShadow(Layer.effects-1, targetCid, siegeColor, null, 4, -12f);
											if(city.outGoing!=City.OutGoing.none)
												DrawRectOutlineShadow(Layer.effects - 1, targetCid, attackColor, null, 4, -8f);

											if(!IsCulled(c1) && (wantDetails || showAll || Spot.IsSelectedOrHovered(targetCid, noneIsAll)))
											{
												DrawTextBox($"{incAttacks}{city.IncomingInfo()}\n{ (city.tsDefMax + 999) / 1000 }k",
														c1, tipTextFormatCentered, incAttacks != 0 ? Color.White : Color.Cyan, textBackgroundOpacity, Layer.tileText);
											}
										}
									}
								}
								if(defenderVisible)
								{
									foreach(var city in City.myCities)
									{
										if(!city.testContinentFilter)
											continue;
										Assert(city is City);
										if(!city.incoming.Any())
										{
											var targetCid = city.cid;
											var c1 = targetCid.CidToCamera();
											if(IsCulled(c1))  // this is in pixel space - Should be normalized for screen resolution or world space (1 continent?)
												continue;
											if(wantDetails || Spot.IsSelectedOrHovered(targetCid, true))
											{
												DrawTextBox($"{(city.tsDefMax.Max(city.tsHome) + 999) / 1000 }k Ts (#:{city.reinforcementsIn.CountNullable()})", c1, tipTextFormatCentered, Color.Cyan, textBackgroundOpacity, Layer.tileText);
											}

										}
									}
								}
							}

							if(NearRes.IsVisible())
							{
								var sendOffset = new System.Numerics.Vector2(0.125f *pixelScale, 0.125f *pixelScale);
								var viewHover  = Spot.viewHover;
								var hasHover   = viewHover != 0;
								foreach(var city in City.friendCities)
								{
									var needsHover = (hasHover && !city.isHover);
									//	if(!city.testContinentFilter)
									//		continue;
									var wc = city.cid.CidToWorld();
									var cc = wc.WorldToCamera();
									if(!needsHover)
									{
										if(!IsCulledWC(wc))
										{
											for(int r = 0; r < 4; ++r)
											{
												var xT0 = (r + 0.5f) / 4.0f;
												var xT1 = (r + 1.375f) / 4.0f;
												var yt0 = 0.0f;
												var yt1 = (city.res[r] * (1.0f / (512 * 128))).Min(1.0f);
												var color = r switch
												{
													0 => new Color(150, 75, 0, 255),
													1 => new Color(128, 128, 128, 255),
													2 => new Color(24, 124, 168, 255),
													_ => new Color(192, 192, 0, 255)
												};
												if(yt1 < 0.125f)
												{
													yt1 = 0.25f;
													color = new Color(255, 0, 0, 255);
												}

												var c0 = new System.Numerics.Vector2(cc.X + (xT0 * 0.8f - 0.5f) * pixelScale,
																			cc.Y   + (0.25f - yt1 * 0.5f) * pixelScale);
												var c1 = new System.Numerics.Vector2(cc.X + (xT1 * 0.8f - 0.5f) * pixelScale,
																			cc.Y   + 0.25f               * pixelScale);
												DrawRect(Layer.actionOverlay, c0, c1, color, zLabels);
												DrawRect(Layer.action, c0, c1, CColor(a: 192), 0);

											}
										}
									}

									var ti = city.tradeInfo;
									if(ti == null)
										continue;
									var cityHover = city.isHover;
									try
									{

										foreach(var toCid in ti.resDest)
										{
											if(needsHover && (toCid!=City.viewHover))
												continue;
											var c1 = toCid.CidToWorld();
											//	var t = (tick * city.cid.CidToRandom().Lerp(1.375f / 512.0f, 1.75f / 512f));
											//	var r = t.Ramp();
											var hover = cityHover;// | Spot.IsHover(toCid);
											DrawAction(cc - sendOffset, c1.WorldToCamera()- sendOffset, hover ? tradeColorHover : tradeColor, lineThickness, hover);
										}
										foreach(var toCid in ti.resSource)
										{
											if(needsHover && (toCid!=City.viewHover))
												continue;

											var c1 = toCid.CidToWorld();
											//	var t = (tick * city.cid.CidToRandom().Lerp(1.375f / 512.0f, 1.75f / 512f));
											//	var r = t.Ramp();
											var hover = cityHover;// | Spot.IsHover(toCid);
											DrawAction(c1.WorldToCamera()+ sendOffset, cc + sendOffset, hover ? tradeColorHover1 : tradeColor1, lineThickness, hover);
										}

									}
									catch(Exception ex)
									{
										LogEx(ex);
									}

								}
							}
							//if(!defenderVisible && !attacksVisible)
							{
								if(Spot.settles.Any())
								{
									try
									{
										foreach(var s in Spot.settles)
										{
											var wc0 = s.cid.CidToWorld();
											var wc1 = s.rcid.CidToWorld();
											if(!IsSegmentCulledWC(wc0, wc1))
											{
												var cc1 = wc1.WorldToCamera();
												DrawAction(0.5f, 1.0f, 1.0f, wc0.WorldToCamera(), cc1, senatorColor,
												troopImages[ttSenator], false, null, highlight: Spot.IsSelectedOrHovered(s.cid));
												DrawFlag(s.rcid, SpriteAnim.flagGrey, System.Numerics.Vector2.Zero);

											}



										}
									}
									catch(Exception ex)
									{

									}
								}

							}
							const int raidCullSlopSpace = 4;
							foreach(var city in City.friendCities)
							{
								if(!city.testContinentFilter)
									continue;

								var wc = city.cid.CidToWorld();
								// Todo: clip thi
								if(city.senatorInfo.Length != 0 && !(IncomingTab.IsVisible() || NearDefenseTab.IsVisible()))
								{
									var c = wc.WorldToCamera();
									var idle = 0;
									var active = 0;
									var recruiting = 0;
									foreach(var sen in city.senatorInfo)
									{
										if(sen.type == SenatorInfo.Type.idle)
											idle += sen.count;
										else if(sen.type == SenatorInfo.Type.recruit)
											recruiting += sen.count;
										else
											active += sen.count;
										if(sen.target != 0 && sen.type == SenatorInfo.Type.settle)
										{
											var c1 = sen.target.CidToCamera();

											var dist = (float)(city.cid.DistanceToCidD(sen.target) * cartTravel); // todo: ship travel?
											var t = (tick * city.cid.CidToRandom().Lerp(1.5f / 512.0f, 1.75f / 512f)) + 0.25f;
											var r = t.Ramp();
											// Todo: more accurate senator travel times
											DrawAction((float)(sen.time - serverNow).TotalSeconds, dist * 60.0f, r, c, c1, senatorColor,
												troopImages[ttSenator], false, null, highlight: Spot.IsSelectedOrHovered(city.cid));
											DrawFlag(sen.target, SpriteAnim.flagGrey, System.Numerics.Vector2.Zero);
										}
									}
									if(!IsCulledWC(wc))
										DrawTextBox($"Sen:  {recruiting}`{idle}`{active}", c, tipTextFormatCentered, Color.White, textBackgroundOpacity, Layer.tileText);

								}
								if(city.isMine)
								{
									if(!IsCulledWC(wc))
									{
										if(!city.isSelected || city.cid == City.build)
											DrawFlag(city.cid, city.cid == City.build ? SpriteAnim.flagHome : SpriteAnim.flagRed, new System.Numerics.Vector2(4, 4));
									}
									if((MainPage.IsVisible() && Settings.raidsVisible != 0) || Settings.raidsVisible == 1)
									{
										if(IsSquareCulledWC(wc, raidCullSlopSpace))
											continue;
										var c = wc.WorldToCamera();
										var t = (tick * city.cid.CidToRandom().Lerp(1.375f / 512.0f, 1.75f / 512f));
										var r = t.Ramp();
										//ds.DrawRoundedSquareWithShadow(c,r, raidBrush);
										foreach(var raid in city.raids)
										{
											var ct = raid.target.CidToCamera();
											(var c0, var c1) = !raid.isReturning ? (c, ct) : (ct, c);
											DrawAction((float)(raid.time - serverNow).TotalMinutes,
												raid.GetOneWayTripTimeMinutes(city),
												r, c0, c1, raidColor, troopImages[raid.troopType], false, null, highlight: Spot.IsSelectedOrHovered(city.cid));

										}
									}
								}
							}
						}

						foreach(var cid in Spot.selected)
						{
							DrawRectOutlineShadow(Layer.effects - 1, cid, selectColor, null, 3.0f, 4.0f);
							//DrawFlag(cid, SpriteAnim.flagSelected, Vector2.Zero);
						}
						foreach(var cid in Settings.pinned)
						{
							DrawFlag(cid, SpriteAnim.flagPinned, new System.Numerics.Vector2(4, -4));
						}
						if(Spot.focus != 0)
						{
							var cid = Spot.focus;
							DrawAccent(cid, -1.125f, focusColor);
						}
						if(Spot.viewHover != 0)
						{
							var cid = Spot.viewHover;
							DrawAccent(cid, 1.25f, hoverColor);
						}

						if(Player.viewHover != 0)
						{
							if(Player.all.TryGetValue(Player.viewHover, out var p))
							{
								try
								{
									foreach(var cid in p.cities)
									{
										DrawFlag(cid, SpriteAnim.flagGrey, new System.Numerics.Vector2(-4, 4));
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
						if(wantDetails)
						{
							//
							// Text names
							//	using (var batch = ds.CreateSpriteBatch(CanvasSpriteSortMode.Bitmap))
							{
								// Labels last
								for(var cy = cy0; cy < cy1; ++cy)
								{
									for(var cx = cx0; cx < cx1; ++cx)
									{
										(var name, var isMine, var hasIncoming, var hovered, var spot) = World.GetLabel((cx, cy));
										//var zScale = CanvasHelpers.ParallaxScale(TileData.zCities);

										if(name != null)
										{

											var span = pixelScale;
											var cid = (cx, cy).WorldToCid();

											var drawC = (new System.Numerics.Vector2(cx, cy).WorldToCamera());
											drawC.Y += span * (isWinter ? 8.675f / 16.0f : 7.125f / 16.0f);
											var z = zCities;
											var scale = bmFontScale;


											if(viewHovers.TryGetValue((aa) => aa.cid == cid, out var viewHover))
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
																	: spot.outGoing != 0 ? nameColorOutgoing
																		: myNameColor) :
											(hasIncoming ?
												(hovered ?
													(spot.underSiege ? nameColorSiegedHover : nameColorIncomingHover)
												   : (spot.underSiege ? nameColorSieged : nameColorIncoming))
												   : hovered ? nameColorHover
												   : spot.outGoing != 0 ? nameColorOutgoing
												   : nameColor);

											DrawTextBox(name, drawC, nameTextFormat, wantDarkText ? color.A.AlphaToBlack() : color,

												!isWinter ? new Color() :
													wantDarkText ? new Color(color.R, color.G, color.B, (byte)128) : 128.AlphaToBlack(), Layer.tileText, 2, 0, PlanetDepth, z, scale);
											//										layout.Draw(drawC,
											//									, Layer.tileText, z,PlanetDepth);

										}
										if(spot != null && !focusOnCity && !(Settings.troopsVisible.HasValue && Settings.troopsVisible.Value == false))
										{
											if(!spot.troopsTotal.Any() && spot.isNotClassified && spot.canVisit && Settings.troopsVisible.GetValueOrDefault())
												spot.TouchClassification();
											if(spot.troopsTotal.Any() || spot.isClassified)
											{
												var c1 = (cx, cy).WorldToCamera();
												var rand = spot.cid.CidToRandom();
												var t = (tick * rand.Lerp(1.5f / 512.0f, 1.75f / 512f)) + 0.25f;

												int type;
												float typeBlend;
												float alpha = 1;// (t * rand.Lerp(0.7f, 0.8f)).Wave() * 0.20f + 0.70f;

												if(spot.troopsTotal.Any())
												{
													var ta = GetTroopBlend(t, spot.troopsTotal.Length);
													alpha = ta.alpha;

													type = spot.troopsTotal.GetIndexType(ta.iType);
												}
												else
												{
													type = spot.TroopType;
													typeBlend = 1;
													switch(spot.classification)
													{
														case Spot.Classification.misc:
														case Spot.Classification.unknown:
														case Spot.Classification.pending:
														case Spot.Classification.missing:
															goto dontDraw;
													}

												}
												var r          = t.Ramp();
												var spriteSize = new System.Numerics.Vector2(32 * Settings.iconScale);
												var _c0        = c1 - spriteSize;
												var _c1        = c1 + spriteSize;

												draw.AddQuadWithShadow(Layer.effects, Layer.effectShadow, troopImages[type], _c0, _c1, HSLToRGB.ToRGBA(rectSpan, 0.3f, 0.825f, alpha, alpha + 0.125f), ShadowColor(alpha),
													(_c0, _c1).RectDepth(zCities), (_c0, _c1).RectDepth(zEffectShadow), shadowOffset);
											}
										dontDraw:;
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
					var _toolTip = ShellPage.toolTip;
					if(underMouse != null)
					{
						//         Spot.viewHover = 0; // clear
						_toolTip = underMouse.GetToopTip(serverNow);
					}
					if(_toolTip != null)
					{
						//	TextLayout textLayout = GetTextLayout( _toolTip, tipTextFormat);
						//	var bounds = textLayout.span;
						System.Numerics.Vector2 c = ShellPage.mousePositionC + new System.Numerics.Vector2(16, 16);
						DrawTextBox(_toolTip, c, tipTextFormat, Color.White, 192, Layer.overlay, 11, 11, ConstantDepth, 0, 0.5f);
					}
					var _contTip = ShellPage.contToolTip;
					if(_contTip != null)
					{
						var alpha = pixelScale.SmoothStep(cityZoomThreshold - 128, cityZoomThreshold + 128).
							Max(pixelScale.SmoothStep(cityZoomWorldThreshold + 16, cityZoomWorldThreshold - 16));
						System.Numerics.Vector2 c = new System.Numerics.Vector2(20, 16).ScreenToCamera();
						DrawTextBox(_contTip, c, tipTextFormat, Color.White.Scale(alpha), (byte)(alpha * 192.0f).RoundToInt(), Layer.overlay, 11, 11, ConstantDepth, 0, 0.5f);
					}
					if(View.IsCityView())
					{
						var                     alpha = 255;
						System.Numerics.Vector2 c     = new System.Numerics.Vector2(clientSpan.X - 32, 16).ScreenToCamera();
						var                     city  = City.GetBuild();
						if(city != null)
						{
							var counts = city.GetTownHallAndBuildingCount(false);

							DrawTextBox($"{counts.buildingCount}/{counts.townHallLevel * 10}", c, tipTextFormatRight, Color.White.Scale(alpha), (byte)(alpha * 192.0f).RoundToInt(), Layer.overlay, 11, 11, ConstantDepth, 0, 0.5f);
						}
					}
					var _debugTip = ToolTips.debugTip;
					if(_debugTip != null)
					{
						var                     alpha = 255;
						System.Numerics.Vector2 c     = new Vector2(clientSpan.X - 16, 16).ScreenToCamera();
						DrawTextBox(_debugTip, c, tipTextFormatRight, Color.White.Scale(alpha), (byte)(alpha * 192.0f).RoundToInt(), Layer.overlay, 11, 11, ConstantDepth, 0, 0.5f);
					}


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
			catch(Exception ex)
			{
				LogEx(ex);
				draw._beginCalled = false;
			}

			static (int iType, float alpha) GetTroopBlend(float t, int nSprite)
			{
				int iType = 0;
				float alpha = 1;
				if(nSprite > 1)
				{
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


	static Dictionary<int, TextLayout> nameLayoutCache = new Dictionary<int, TextLayout>();
	static public TextLayout GetTextLayout(string name, TextFormat format)
	{
		var hash = name.GetHashCode(StringComparison.Ordinal);
		if(nameLayoutCache.TryGetValue(name.GetHashCode(StringComparison.Ordinal), out var rv))
			return rv;
		rv = new TextLayout(name, format);

		if(nameLayoutCache.Count >= maxTextLayouts)
			nameLayoutCache.Remove(nameLayoutCache.First().Key);
		nameLayoutCache.Add(hash, rv);

		return rv;

	}







	public static void DrawTextBox(string text, Vector2 at, TextFormat format, Color color, byte backgroundAlpha, int layer = Layer.tileText, float _expandX = 2.0f, float _expandY = 0, DepthFunction depth = null, float zBias = -1, float scale = 0)
	{
		DrawTextBox(text, at, format, color, backgroundAlpha == 0 ? new Color() : color.IsDark() ? new Color((byte)255, (byte)255, (byte)255, backgroundAlpha) : new Color((byte)(byte)0, (byte)0, (byte)0, backgroundAlpha), layer, _expandX, _expandY, depth, zBias, scale);
	}
	private static void DrawTextBox(string text, Vector2 at, TextFormat format, Color color, Color backgroundColor, int layer = Layer.tileText, float _expandX = 0.0f, float _expandY = 0, DepthFunction depth = null, float zBias = -1, float scale = 0)
	{
		if(IsSquareCulled(at, textBoxCullSlop))
		{
			return;
		}
		if(scale == 0)
			scale = bmFontScale;
		if(scale == 0)
			return;

		TextLayout textLayout = GetTextLayout(text, format);
		if(zBias == -1)
			zBias = zLabels;

		var constantDepth = depth == null;

		// shift everything, then ignore Z
		if(constantDepth)
		{
			depth = ConstantDepth;
			var atScale = at.Project(zBias);
			at = atScale.c;
			scale *= atScale.scale;
			zBias = 0;
		}
		var span = textLayout.ScaledSpan(scale);
		var expand = new Vector2(_expandX, _expandY);
		if(backgroundColor.A > 0)
		{
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
			FillRoundedRectangle(Layer.textBackground, c0 - expand, c0 + expand + span, backgroundColor, depth, zBias);
		}
		textLayout.Draw(at, scale, color, layer, zBias, depth);
	}

	private static void FillRoundedRectangle(int layer, Vector2 c0, Vector2 c1, Color background, DepthFunction depth, float z)
	{
		draw.AddQuad(layer, quadTexture, c0, c1, background, depth, z);/// c0.CToDepth(),(c1.X,c0.Y).CToDepth(), (c0.X,c1.Y).CToDepth(), c1.CToDepth() );
	}
	//	static Vector2 _uv0;
	//	static Vector2 _uv1;
	private static void DrawFlag(int cid, SpriteAnim sprite, Vector2 offset)
		{
			var wc = cid.CidToWorld();
			if(IsCulledWC(wc))
				return;

			var c = wc.WorldToCamera() + offset;
			var dv = shapeSizeGain * 48 * 4 * Settings.flagScale;
			float z = zLabels;

			// hover flags
			if(viewHovers.TryGetValue((aa) => aa.cid == cid, out var dz))
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

		const float actionStopDistance = 48.0f;
		private static void DrawAction(float timeToArrival, float journeyTime, float rectSpan, Vector2 c0, Vector2 c1, Color color,
		Material bitmap, bool applyStopDistance, Army? army, float alpha = 1, float lineThickness = GameClient.lineThickness, bool highlight = false)
		{
			if(IsSegmentCulled(c0, c1))
				return;
			float progress;
			if(timeToArrival <= 0.0f)
			{
				progress = 1.0f;
				timeToArrival = 0;
			}
			else
			{
				if(timeToArrival >= journeyTime)
					progress = 1.0f / 16.0f; // just starting
				else
					progress = 1f - (timeToArrival / journeyTime); // we don't know the duration so we approximate with 2 hours
			}
			if(applyStopDistance)
			{
				progress = progress.Min(1.0f - shapeSizeGain * actionStopDistance / Vector2.Distance(c0, c1));
				//      var dc01 = c0 - c1;
				//       c1 += dc01 *( actionStopDistance / dc01.Length());
			}

			var gain = 1.0f;
			if(timeToArrival < postAttackDisplayTime)
				gain = 1.0f + (1.0f - timeToArrival / postAttackDisplayTime) * 0.25f;
			var mid = progress.Lerp(c0, c1);

			float spriteSize = 32 * Settings.iconScale;

			if(army is not null && ShellPage.toolTip is null)
			{
				(var distance, _) = ShellPage.mousePositionW.DistanceToSegment(c0, c1);
				if(distance < bestUnderMouseScore)
				{
					bestUnderMouseScore = distance;
					underMouse = army;
					highlight = true;
					spriteSize *= 1.5f;
				}
			}
			if(highlight)
				lineThickness *= 2;
			var shadowColor = ShadowColor(1.0f, highlight);
			//	if(wantShadow)
			//	DrawLine(Layer.effectShadow,c0,c1,GetLineUs(c0,c1),shadowColor,zEffectShadow,thickness: lineThickness);
			if(applyStopDistance && wantShadow)
				DrawSquare(Layer.effectShadow, c0, shadowColor, zEffectShadow);
			DrawLine(Layer.action + 2, c0, c1, GetLineUs(c0, c1), color, zLabels, thickness: lineThickness);
			if(applyStopDistance)
				DrawSquare(Layer.action + 3, new Vector2(c0.X, c0.Y), color, zLabels);
			//var dc = new Vector2(spriteSize, spriteSize);
			if(bitmap != null)
			{
				var _c0 = new Vector2(mid.X - spriteSize, mid.Y - spriteSize);
				var _c1 = new Vector2(mid.X + spriteSize, mid.Y + spriteSize);
				draw.AddQuadWithShadow(Layer.action + 4, Layer.effectShadow, bitmap, _c0, _c1, HSLToRGB.ToRGBA(rectSpan, 0.3f, 0.825f, alpha, gain * 1.1875f), shadowColor, (_c0, _c1).RectDepth(zLabels), (_c0, _c1).RectDepth(zEffectShadow), new());
			}
			//            ds.DrawRoundedSquare(midS, rectSpan, color, 2.0f);


		}

		private void DrawAction(Vector2 c0, Vector2 c1, Color color, float thickness, bool highlight)
		{
			DrawAction(0, 1.0f, 1, c0, c1, color, null, false, null, alpha: 1, lineThickness: thickness, highlight: highlight);


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
		private static void DrawRect(int layer, Vector2 c0, Vector2 c1, Color color, float Z)
		{
			draw.AddQuad(layer, quadTexture, c0, c1, new Vector2(), new Vector2(1, 1), color, PlanetDepth, zLabels);
		}
		private static void DrawRectOutline(int layer, Vector2 c0, Vector2 c1, Color color, float z, float thickness, float expand = 0f)
		{

			float t = thickness * 0.5f;
			const float waveGain = 0.25f;
			z = z * (1.0f + animationTWrap.Wave() * waveGain);
			c0 = new(c0.X - expand, c0.Y - expand);
			c1 = new(c1.X + expand, c1.Y + expand);

			draw.AddQuad(layer, quadTexture, new(c0.X, c0.Y-t), new(c1.X, c0.Y+t), new Vector2(), new Vector2(1, 1), color, PlanetDepth, z);

			draw.AddQuad(layer, quadTexture, new(c0.X, c1.Y -t), new(c1.X, c1.Y +t), new Vector2(), new Vector2(1, 1), color, PlanetDepth, z);

			draw.AddQuad(layer, quadTexture, new(c0.X- t, c0.Y), new(c0.X +t, c1.Y), new Vector2(), new Vector2(1, 1), color, PlanetDepth, z);

			draw.AddQuad(layer, quadTexture, new(c1.X-t, c0.Y), new(c1.X+t, c1.Y), new Vector2(), new Vector2(1, 1), color, PlanetDepth, z);
		}


		private static void DrawRectOutlineShadow(int layer, Vector2 c0, Vector2 c1, Color color, float thickness = 3, float expand = 0)
		{
			DrawRectOutline(layer, c0, c1, color, zCities, thickness, expand);
			if(parallaxGain > 0 && wantShadow)
				DrawRectOutline(Layer.tileShadow, c0 +shadowOffset*0, c1+shadowOffset*0, color.GetShadowColorDark(), 0.0f, thickness, expand);
		}
		private static void DrawRectOutlineShadow(int layer, int cid, Color col, string label = null, float thickness = 3, float expand = 0)
		{
			var wc = cid.CidToWorld();
			if(IsCulledWC(wc))
				return;
			var cc = wc.WorldToCamera();
			var c0 = cc - halfSquareOffset;
			var c1 = cc + halfSquareOffset;
			DrawRectOutlineShadow(Layer.effects, c0, c1, col, thickness, expand);
			if(label != null)
			{
				DrawTextBox(label, cc, textformatLabel, new Color(col, 255), textBackgroundOpacity, Layer.tileText, scale: (bmFontScale * 2.0f).Min(0.5f));
			}
		}

		private static void DrawDiamond(int layer, Vector2 c0, Vector2 c1, Color color, float z, float thickness, float expand)
		{
			float d = thickness;
			var cm = (c0 + c1) * 0.5f;
			float ext = 0.41f + expand;
			float ext1 = 1.0f + ext;
			var ct = new Vector2(cm.X, c0.Y * ext1 - cm.Y * ext);
			var cb = new Vector2(cm.X, c1.Y * ext1 - cm.Y * ext);
			var cl = new Vector2(c0.X * ext1 - cm.X * ext, cm.Y);
			var cr = new Vector2(c1.X * ext1 - cm.X * ext, cm.Y);
			draw.AddLine(layer, whiteMaterial, cl, ct, d, 0.0f, 1.0f, color, (cl.CToDepth(z), ct.CToDepth(z)));
			draw.AddLine(layer, whiteMaterial, ct, cr, d, 0.0f, 1.0f, color, (ct.CToDepth(z), cr.CToDepth(z)));
			draw.AddLine(layer, whiteMaterial, cr, cb, d, 0.0f, 1.0f, color, (cr.CToDepth(z), cb.CToDepth(z)));
			draw.AddLine(layer, whiteMaterial, cb, cl, d, 0.0f, 1.0f, color, (cb.CToDepth(z), cl.CToDepth(z))); ;
		}
		private static void DrawDiamondShadow(int layer, Vector2 c0, Vector2 c1, Color color, float thickness, float expand)
		{
			DrawDiamond(layer, c0, c1, color, zCities, thickness, expand);
			if(parallaxGain > 0 && wantShadow)
				DrawDiamond(Layer.tileShadow, c0 + shadowOffset*0, c1 + shadowOffset*0, color.GetShadowColorDark(), 0f, thickness, expand);
		}
		private static void DrawDiamondShadow(int layer, int cid, Color col, string label = null, float thickness = 3, float expand = 0)
		{
			var wc = cid.CidToWorld();
			if(IsCulledWC(wc))
				return;
			var cc = wc.WorldToCamera();
			var c0 = cc - halfSquareOffset;
			var c1 = cc + halfSquareOffset;
			DrawDiamondShadow(Layer.effects, c0, c1, col, thickness, expand);
			if(label != null)
			{
				DrawTextBox(label, cc, textformatLabel, new Color(col, 255), textBackgroundOpacity, Layer.tileText, scale: (bmFontScale * 2.0f).Min(0.5f));
			}
		}

		static  (float u, float v) GetLineUs(Vector2 c0, Vector2 c1)
		{
			float offset = (lineAnimationGain * (animationTWrap)) % 1;
			return (offset, offset-(c0 - c1).Length()* clampedScaleInverse * lineTileGain);

		}
		private static void DrawLine(int layer, Vector2 c0, Vector2 c1, (float u, float v) uv, Color color, float zBias, float thickness = lineThickness)
		{
			//	draw.AddLine(layer,lineDraw, c0, c1, lineThickness, , color,(c0.CToDepth()+ zBias, c1.CToDepth()+ zBias) );
			draw.AddLine(layer, lineDraw, c0, c1, thickness, uv.u, uv.v, color, (c0.CToDepth(zBias), c1.CToDepth(zBias)));
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
			if(wantShadow)
				DrawAccentBase(c.X, c.Y, radius, angle, brush.GetShadowColorDark(), Layer.effectShadow, zEffectShadow);
			DrawAccentBase(c.X, c.Y, radius, angle, brush, Layer.overlay, zLabels);
		}
		public static void DrawAccent(int cid, float angularSpeedBase, Color brush, float radiusScale = 1.0f)
		{

			var wc = cid.CidToWorld();
			if(IsCulledWC(wc))
				return;

			var c = wc.WorldToCamera();

			var rnd = cid.CidToRandom();

			var angularSpeed = angularSpeedBase + rnd * 0.5f;
			var t = (AGame.animationT * rnd.Lerp(1.25f / 256.0f, 1.75f / 256f));
			var r = t.Wave().Lerp(GameClient.circleRadiusBase, GameClient.circleRadiusBase * 1.375f)* radiusScale;
			DrawAccent(c, r, angularSpeed, brush);
		}
		

		public static void UpdateRenderQuality(float renderQuality)
		{
			UpdateLighting();
			UpdateResolution();
		}

	const float viewHoverZGain = 1.0f / 64.0f;
	const float viewHoverElevationKt = 24.0f;
	public static List<(int cid, float z, float vz)> viewHovers = new List<(int cid, float z, float vz)>();


	protected override void Update(GameTime gameTime)
	{

		try
		{
			float dt = ((float)gameTime.ElapsedGameTime.TotalSeconds).Min(1.0f / 16.0f);

			var hover = lastCanvasC;
			if(hover != 0 && World.GetInfoFromCid(hover).type != 0)
			{
				if(!viewHovers.Exists(a => a.cid == lastCanvasC))
				{
					viewHovers.Add((lastCanvasC, 1.0f / 32.0f, 0.0f));
				}
			}
			{
				var removeMe = 0;
				int count = viewHovers.Count;
				for(int i = 0; i < count; ++i)
				{
					var cid = viewHovers[i].cid;

					float z = viewHovers[i].z;
					float vz = viewHovers[i].vz;
					var kd = (viewHoverElevationKt);
					var ks = AMath.CritDampingKs(kd);

					vz += (((cid == hover ? 1.0f : 0.0f) - z) * ks - vz * kd) * dt;
					z += vz * (float)dt;
					viewHovers[i] = (cid, z, vz);


					if(z <= 1.0f / 32.0f)
					{
						removeMe = i;
					}
				}
				// Hack:  Just remove one per frame, we'll get the rest next time,
				if(removeMe != 0)
					viewHovers.RemoveAt(removeMe);

			}


			if(clientSpan.X > 0 && clientSpan.Y > 0 && AppS.isForeground)
			{
				if(resolutionDirtyCounter > 0 && !faulted)
				{
					if(--resolutionDirtyCounter == 0)
					{
						wantFastRefresh = false;
						//									_graphics.PreferredBackBufferHeight = (int)clientSpan.Y;
						//								_graphics.PreferredBackBufferWidth = (int)clientSpan.X;
						//					_graphics.ApplyChanges();
						var pre = new PresentationParameters()
						{
							BackBufferFormat = SurfaceFormat.Bgra32,//_graphics.PreferredBackBufferFormat,
							DepthStencilFormat = DepthFormat.None,
							SwapChainPanel = canvas,
							RenderTargetUsage = RenderTargetUsage.DiscardContents,
							BackBufferWidth = (int)(clientSpan.X*resolutionScale),// - ShellPage.cachedXOffset,
							BackBufferHeight = (int)(clientSpan.Y*resolutionScale), // - ShellPage.cachedTopOffset,
						};
						GameClient.instance.GraphicsDevice.Reset(pre);
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
			if(!faulted && AppS.isForeground)
			{
				if(World.bitmapPixels != null)
				{
					// canvas.Paused = true;
					var pixels = World.bitmapPixels;

					World.bitmapPixels = null;
					if(worldObjects != null)
					{
						var w = worldObjects;
						worldObjects = null;
						w.texture.Dispose();
					}
					worldObjects = CreateFromBytes(pixels, World.outSize, World.outSize, SurfaceFormat.Dxt1SRgb);
				}
				if(World.worldOwnerPixels != null)
				{
					var ownerPixels = World.worldOwnerPixels;
					World.worldOwnerPixels = null;
					if(worldOwners != null)
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
				if(World.changePixels != null)
				{
					var pixels = World.changePixels;

					ClearHeatmapImage();
					worldChanges = CreateFromBytes(pixels, World.outSize, World.outSize, SurfaceFormat.Dxt1, worldSpaceEffect);


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
		catch(SharpDX.SharpDXException sex)
		{
			if(!faulted)
			{
				faultCount += 16;
				if(faulted)
				{
					try
					{
						CnV.Debug.LogEx(sex, report: false);
						CnV.Debug
							.Log($"{sex.ResultCode} {sex.Descriptor.ApiCode} {sex.Descriptor.Description} {sex.Descriptor.ToString()} ");
						Faulted();

					}
					catch(Exception ex2)
					{

					}
				}
			}

		}
		catch(Exception _exception)
		{
			CnV.Debug.LogEx(_exception);
			++faultCount;
		}



	}

	private static async Task Faulted()
	{
		var a = await AppS.DispatchOnUIThreadTask(async () =>
		{
			return await AppS.Failed("Video Driver broke", "Please restart, it should recover");

		});
		if(a == 1)
		{
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
	protected override bool BeginDraw()
	{
		if(!AppS.isForeground)
			return false;
		if(!CnVServer.isInitialized)
			return false;
		if(faulted)
			return false;
		return base.BeginDraw();
	}

}

