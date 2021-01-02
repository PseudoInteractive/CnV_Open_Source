using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended.BitmapFonts;

using System;
using System.Collections.Generic;

using Vector2 = System.Numerics.Vector2;
// Based on default Monogame's Spritebatch by maintainy bois.
// https://github.com/MonoGame/MonoGame/blob/master/MonoGame.Framework/Graphics/SpriteBatch.cs
using static COTG.Debug;

namespace COTG.Draw
{
	// SortKey = (layer << 20) + (Effect._sortingKey << 16) + Texture.SortingKey;
	public class Layer
	{
		public const int background = 1;
		public const int tileBase=16;
		public const int tileShadow = 32;
		public const int tiles = 48;

		
		public const int effects = 65;
		public const int action = effects+8;
		public const int overlay = maxLayer-20;
		public const int text = maxLayer-10;
		public const int maxLayer = (1 << 12) - 1; // 4095
		public int layer;
		List<VertexBatch> allBatches;
		List<VertexBatch> openBatches;

		internal static float GetDepth(int layer)
		{
			return layer / 16384.0f;
		}
		public static int SortKey(int layer, Texture2D texture) => texture.SortingKey + (layer << 24);
	}

	public struct TextureSection
	{
		public Texture2D texture;
		public Vector2 uv0;
		public Vector2 uv1;

		public TextureSection(Texture2D _texture, Vector2 _uv0, Vector2 _uv1)
		{
			texture = _texture;
			uv0 = _uv0;
			uv1 = _uv1;
		}
		public static TextureSection FromTexels(Texture2D tex, Vector2 c0, Vector2 c1)
		{
			var uvGain = new Vector2(1.0f/ tex.Width,1.0f/ tex.Height);
			return new TextureSection(tex, c0*uvGain, c1*uvGain);
		}
		public static TextureSection FromUV(Texture2D tex, Vector2 uv0, Vector2 uv1) => new TextureSection(tex,uv0, uv1);
		public static TextureSection FromAll(Texture2D tex) => new TextureSection(tex, new Vector2(), new Vector2(1, 1));
	}
		public unsafe class VertexBatch
	{

		public bool NeedsFlush => _vertexPoolCount > 0 && _indexPoolCount > 0;

		
		private int _indexPoolCount = 0;
		const int _indexPoolCapacity = _vertexPoolCapacity * 6;

		
		private short _vertexPoolCount = 0;
		public const short _vertexPoolCapacity = 256;

		private EffectPass _effectPass;

		public Effect _effect;
		public void SetTextureAndEffect(Texture2D texture, Effect effect)
		{
			{
				if (effect == null)
					effect = VertexBatcher. _defaultEffect;
				if (effect != _effect || texture != _texture)
				{
					FlushBatch();
					if (_effect != effect)
					{
						_effect = effect;
						_texture = texture;
						_effect.CurrentTechnique = _effect.Techniques["PositionColorTexture"];
						_effectPass = _effect.CurrentTechnique.Passes[0];
						_effectPass.Apply();

						GraphicsDevice.Textures[0] = _texture;
						GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
					}
					else if (_texture != texture)
					{
						_texture = texture;
						GraphicsDevice.Textures[0] = _texture;
						GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
					}

				}

			}
		}
		Texture2D _texture;

	};
		/// <summary>
		/// Helper class for drawing text strings and sprites in one or more optimized batches.
		/// </summary>
		public class VertexBatcher
	{

		#region Fields.



		


	
		#endregion




		const PrimitiveType _primitiveType = PrimitiveType.TriangleList;


		//public Effect DefaultEffect
		//{
		//	get => _defaultEffect;
		//	set
		//	{
		//		// TODO: Revisit.
		//		if (value != _defaultEffect)
		//		{
		//			FlushBatch();

		//			if (value == null)
		//			{
		//				_defaultEffect = _alphaBlendEffect;
		//			}
		//			else
		//			{
		//				_defaultEffect = value;
		//			}

		//			if (_texture != null)
		//			{
		//				// TODO: Move to the separate method.
		//				_defaultEffect.CurrentTechnique = _defaultEffect.Techniques["TexturePremultiplied"];
		//			}
		//			else
		//			{
		//				_defaultEffect.CurrentTechnique = _defaultEffect.Techniques["Basic"];
		//			}

		//			_defaultEffectPass = _defaultEffect.CurrentTechnique.Passes[0];
		//		}
		//	}
		//}
		public static Effect _defaultEffect => AGame.defaultEffect;


		/// <summary>
		/// Default shader with proper alpha blending. 
		/// Replaces BasicEffect. Applied, when CurrentEffect and DefaulrEffect are null.
		/// </summary>
		//public static Effect _alphaBlendEffect;

		public VertexBatcher(
			GraphicsDevice graphicsDevice,
			BlendState blendState = null,
			SamplerState samplerState = null,
			DepthStencilState depthStencilState = null,
			RasterizerState rasterizerState = null
		)
		{
			GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException("graphicsDevice");

			_blendState = blendState ?? BlendState.AlphaBlend;
			_samplerState = samplerState ?? SamplerState.LinearClamp;
			_depthStencilState = depthStencilState ?? DepthStencilState.None;
			_rasterizerState = rasterizerState ?? RasterizerState.CullCounterClockwise;

			_indexPool = new short[_indexPoolCapacity];
			_vertexPool = new VertexPositionColorTexture[_vertexPoolCapacity];


			
		}



		

		private bool FlushIfOverflow(int newVerticesCount, int newIndicesCount)
		{
			if (
				_vertexPoolCount + newVerticesCount < _vertexPoolCapacity
				&& _indexPoolCount + newIndicesCount < _indexPoolCapacity
			)
			{
				return false;
			}

			FlushBatch();
			return true;
		}


		public void FlushBatch()
		{
			if (_vertexPoolCount == 0 || _indexPoolCount == 0)
			{
				return;
			}


			int primitivesCount;
			Debug.Assert(_primitiveType == PrimitiveType.TriangleList);
			primitivesCount = _indexPoolCount / 3;
			{

				

					// Whatever happens in pass.Apply, make sure the texture being drawn
					// ends up in Textures[0].
					GraphicsDevice.Textures[0] = _texture;

					GraphicsDevice.DrawUserIndexedPrimitives(
						_primitiveType,
						_vertexPool,
						0,
						_vertexPoolCount,
						_indexPool,
						0,
						primitivesCount,
						VertexPositionColorTexture.VertexDeclaration
					);

				

			}

			_vertexPoolCount = 0;
			_indexPoolCount = 0;
		}



		#region Quads.


	

		

		public void AddQuad(TextureSection textureSection, Vector2 c0,Vector2 c1, Color color, Effect effect = null)
		{
			
			SetTextureAndEffect(textureSection.texture, effect);

	
			SetQuad(
				c0.X,
				c0.Y,
				c1.X-c0.X,
				c1.Y-c0.Y,
				color,
				textureSection.uv0,
				textureSection.uv1,
				0,
				0,
				0,
				0
			);
		}
		public void AddQuad(Texture2D texture, Vector2 c0, Vector2 c1,Vector2 uv0, Vector2 uv1, Color color, Effect effect = null)
		{
			SetTextureAndEffect(texture, effect);


			//Assert(uv0.X >= 0.0f);
			//Assert(uv0.Y >= 0.0f);
			//Assert(uv1.X >= 0.0f);
			//Assert(uv1.Y >= 0.0f);
			//Assert(uv0.X <= 1.0f);
			//Assert(uv0.Y <= 1.0f);
			//Assert(uv1.X <= 1.0f);
			//Assert(uv1.Y <= 1.0f);
			SetQuad(
				c0.X,
				c0.Y,
				c1.X - c0.X,
				c1.Y - c0.Y,
				color,
				uv0,
				uv1,
				0,
				0,
				0,
				0
			);
		}
		public void AddQuad(Texture2D texture, Vector2 c0, Vector2 c1, Color color, Effect effect = null)
		{
		
			SetTextureAndEffect(texture, effect);


			SetQuad(
				c0.X,
				c0.Y,
				c1.X - c0.X,
				c1.Y - c0.Y,
				color,
				new Vector2(0,0),
				new Vector2(1,1),
				0,
				0,
				0,
				0
			);
		}

		public unsafe void AddLine(Texture2D texture, Vector2 c0, Vector2 c1, float thickness, float u0, float u1, Color color, Effect effect = null)
		{
			SetTextureAndEffect(texture, effect);
		
			var dc0 = c1 - c0;
			var dc1 =new Vector2( dc0.Y,-dc0.X );
			dc1 *= thickness*0.5f / dc1.Length();
				

			SetQuadIndices();


			fixed (VertexPositionColorTexture* vertexPtr = _vertexPool)
			{
				SetVertex(
					vertexPtr,
					c0.X - dc1.X ,
					c0.Y - dc1.Y,
					0,
					color,
					u0,
					0
				);
				SetVertex(
					vertexPtr,
					c0.X + dc1.X,
					c0.Y + dc1.Y,
					0,
					color,
					u0,
					1
				);
				SetVertex(
					vertexPtr,
					c1.X - dc1.X,
					c1.Y - dc1.Y,
					0,
					color,
					u1,
					0
				
				);
				SetVertex(
					vertexPtr,
					c1.X + dc1.X,
					c1.Y + dc1.Y,
					0,
					color,
					u1,
					1

				);

			}
		}


		//public void AddQuad(
		//	Vector2 position,
		//	RectangleF srcRectangle,
		//	Color color,
		//	double rotation,
		//	Vector2 origin,
		//	Vector2 scale,
		//	SpriteFlipFlags flipFlags,
		//	Vector4 zDepth
		//)
		//{

		//	origin = origin * scale;

		//	Vector2 texCoordTL;
		//	Vector2 texCoordBR;


		//	var w = srcRectangle.Width * scale.X;
		//	var h = srcRectangle.Height * scale.Y;
		//	texCoordTL.X = srcRectangle.X / (float)_texture.Width;
		//	texCoordTL.Y = srcRectangle.Y / (float)_texture.Height;
		//	texCoordBR.X = (srcRectangle.X + srcRectangle.Width) / (float)_texture.Width;
		//	texCoordBR.Y = (srcRectangle.Y + srcRectangle.Height) / (float)_texture.Height;

		//	if ((flipFlags & SpriteFlipFlags.FlipVertically) != 0)
		//	{
		//		var temp = texCoordBR.Y;
		//		texCoordBR.Y = texCoordTL.Y;
		//		texCoordTL.Y = temp;
		//	}
		//	if ((flipFlags & SpriteFlipFlags.FlipHorizontally) != 0)
		//	{
		//		var temp = texCoordBR.X;
		//		texCoordBR.X = texCoordTL.X;
		//		texCoordTL.X = temp;
		//	}

		//	if (rotation == 0f)
		//	{
		//		SetQuad(
		//			position.X - origin.X,
		//			position.Y - origin.Y,
		//			w,
		//			h,
		//			color,
		//			texCoordTL,
		//			texCoordBR,
		//			zDepth.X,
		//			zDepth.Y,
		//			zDepth.Z,
		//			zDepth.W
		//		);
		//	}
		//	else
		//	{
		//		SetQuad(
		//			position.X,
		//			position.Y,
		//			-origin.X,
		//			-origin.Y,
		//			w,
		//			h,
		//			(float)Math.Sin(rotation),
		//			(float)Math.Cos(rotation),
		//			color,
		//			texCoordTL,
		//			texCoordBR,
		//			zDepth.X,
		//			zDepth.Y,
		//			zDepth.Z,
		//			zDepth.W
		//		);
		//	}

		//}





		private unsafe void SetQuadIndices()
		{
			fixed (short* poolPtr = _indexPool)
			{
				var indexPtr = poolPtr + _indexPoolCount;

				// 0 - 1
				// | / |
				// 2 - 3

				*(indexPtr + 0) = _vertexPoolCount;
				*(indexPtr + 1) = (short)(_vertexPoolCount + 1);
				*(indexPtr + 2) = (short)(_vertexPoolCount + 2);
				// Second triangle.
				*(indexPtr + 3) = (short)(_vertexPoolCount + 1);
				*(indexPtr + 4) = (short)(_vertexPoolCount + 3);
				*(indexPtr + 5) = (short)(_vertexPoolCount + 2);
			}

			_indexPoolCount += 6;
		}

		private unsafe void SetQuad(
			float x, float y,
			float dx, float dy,
			float w, float h,
			float sin, float cos,
			Color color,
			Vector2 texCoordTL,
			Vector2 texCoordBR,
			float depthTL,
			float depthTR,
			float depthBL,
			float depthBR
		)
		{

			FlushIfOverflow(4, 6);
			SetPrimitiveType(PrimitiveType.TriangleList);

			SetQuadIndices();


			fixed (VertexPositionColorTexture* vertexPtr = _vertexPool)
			{
				SetVertex(
					vertexPtr,
					x + dx * cos - dy * sin,
					y + dx * sin + dy * cos,
					depthTL,
					color,
					texCoordTL.X,
					texCoordTL.Y
				);

				SetVertex(
					vertexPtr,
					x + (dx + w) * cos - dy * sin,
					y + (dx + w) * sin + dy * cos,
					depthTR,
					color,
					texCoordBR.X,
					texCoordTL.Y
				);

				SetVertex(
					vertexPtr,
					x + dx * cos - (dy + h) * sin,
					y + dx * sin + (dy + h) * cos,
					depthBL,
					color,
					texCoordTL.X,
					texCoordBR.Y
				);

				SetVertex(
					vertexPtr,
					x + (dx + w) * cos - (dy + h) * sin,
					y + (dx + w) * sin + (dy + h) * cos,
					depthBR,
					color,
					texCoordBR.X,
					texCoordBR.Y
				);

			}

		}

		private unsafe void SetQuad(
			float x, float y,
			float w, float h,
			Color color,
			Vector2 texCoordTL,
			Vector2 texCoordBR,
			float depthTL,
			float depthTR,
			float depthBL,
			float depthBR
		)
		{
			FlushIfOverflow(4, 6);
			SetPrimitiveType(PrimitiveType.TriangleList);

			SetQuadIndices();

			fixed (VertexPositionColorTexture* vertexPtr = _vertexPool)
			{
				SetVertex(vertexPtr, x, y, depthTL, color, texCoordTL.X, texCoordTL.Y);
				SetVertex(vertexPtr, x + w, y, depthTR, color, texCoordBR.X, texCoordTL.Y);
				SetVertex(vertexPtr, x, y + h, depthBL, color, texCoordTL.X, texCoordBR.Y);
				SetVertex(vertexPtr, x + w, y + h, depthBR, color, texCoordBR.X, texCoordBR.Y);
			}
		}

		private unsafe void SetVertex(
			VertexPositionColorTexture* poolPtr,
			float x, float y, float z,
			Color color,
			float texX, float texY
		)
		{
			var vertexPtr = poolPtr + _vertexPoolCount;

			(*vertexPtr).Position.X = x;
			(*vertexPtr).Position.Y = y;
			(*vertexPtr).Position.Z = z;

			(*vertexPtr).Color = color;
			(*vertexPtr).TextureCoordinate.X = texX;
			(*vertexPtr).TextureCoordinate.Y = texY;

			_vertexPoolCount += 1;
		}

		#endregion



		#region Primitives.

		public unsafe void AddPrimitive(PrimitiveType primitiveType, VertexPositionColorTexture[] vertices, short[] indices)
		{
			FlushIfOverflow(vertices.Length, indices.Length);
			SetPrimitiveType(primitiveType);

			fixed (short* poolPtr = _indexPool, newIndices = indices)
			{
				var newIndicesPtr = newIndices;

				var indicesMax = poolPtr + _indexPoolCount + indices.Length;
				for (
					var indexPtr = poolPtr + _indexPoolCount;
					indexPtr < indicesMax;
					indexPtr += 1, newIndicesPtr += 1
				)
				{
					*indexPtr = (short)(*newIndicesPtr + _vertexPoolCount);
				}
				_indexPoolCount += (short)indices.Length;
			}

			fixed (VertexPositionColorTexture* poolPtr = _vertexPool, newVertices = vertices)
			{
				var newVerticesPtr = newVertices;

				var verticesMax = poolPtr + _vertexPoolCount + vertices.Length;
				for (
					var vertexPtr = poolPtr + _vertexPoolCount;
					vertexPtr < verticesMax;
					vertexPtr += 1, newVerticesPtr += 1
				)
				{
					*vertexPtr = *newVerticesPtr;
				}
				_vertexPoolCount += (short)vertices.Length;
			}
		}

		public unsafe void DrawString(SpriteFont spriteFont, string text, Vector2 position, Vector2 scale,
		 Color color, FlipFlags flags = FlipFlags.None, float depth = 0f)
		{

			var offset = Vector2.Zero;
			var firstGlyphOfLine = true;

			fixed (SpriteFont.Glyph* pGlyphs = spriteFont.Glyphs)
				for (var i = 0; i < text.Length; ++i)
				{
					var c = text[i];

					if (c == '\r')
						continue;

					if (c == '\n')
					{
						offset.X = 0;
						offset.Y += spriteFont.LineSpacing;
						firstGlyphOfLine = true;
						continue;
					}

					var currentGlyphIndex = spriteFont.GetGlyphIndexOrDefault(c);
					var pCurrentGlyph = pGlyphs + currentGlyphIndex;

					// The first character on a line might have a negative left side bearing.
					// In this scenario, SpriteBatch/SpriteFont normally offset the text to the right,
					//  so that text does not hang off the left side of its rectangle.
					if (firstGlyphOfLine)
					{
						offset.X = Math.Max(pCurrentGlyph->LeftSideBearing, 0);
						firstGlyphOfLine = false;
					}
					else
					{
						offset.X += spriteFont.Spacing + pCurrentGlyph->LeftSideBearing;
					}

					var p = offset;
					p.X += pCurrentGlyph->Cropping.X;
					p.Y += pCurrentGlyph->Cropping.Y;
					p *= scale;
					p += position;
					Vector2 _texCoordTL;
					Vector2 _texCoordBR;

					 _texCoordTL.X = pCurrentGlyph->BoundsInTexture.X * spriteFont.Texture.TexelWidth;
					 _texCoordTL.Y = pCurrentGlyph->BoundsInTexture.Y * spriteFont.Texture.TexelHeight;
					 _texCoordBR.X = (pCurrentGlyph->BoundsInTexture.X + pCurrentGlyph->BoundsInTexture.Width) * spriteFont.Texture.TexelWidth;
					 _texCoordBR.Y = (pCurrentGlyph->BoundsInTexture.Y + pCurrentGlyph->BoundsInTexture.Height) * spriteFont.Texture.TexelHeight;

					AddQuad(spriteFont.Texture,p,p+new Vector2(pCurrentGlyph->BoundsInTexture.Width,
							 pCurrentGlyph->BoundsInTexture.Height)*scale,
							
							 _texCoordTL,
							 _texCoordBR,
							  color
							 );

					offset.X += pCurrentGlyph->Width + pCurrentGlyph->RightSideBearing;
				}

		}
		private void SetPrimitiveType(PrimitiveType primitiveType)
		{
			if (_primitiveType != primitiveType)
			{
				FlushBatch();
				_primitiveType = primitiveType;
			}
		}

		#endregion



		#region Matrices. 

		/// <summary>
		/// Sets new transform matrix and pushes old matrix into the stack.
		/// </summary>
		

		#endregion

	}

	public enum FlipFlags
	{
		None
	}
}

