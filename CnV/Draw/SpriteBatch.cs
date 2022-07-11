// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
using Vector2 = System.Numerics.Vector2;
using System;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;
using RectangleF = System.Drawing.RectangleF;
using static CnV.Debug;
using System.Collections.Generic;
using BitmapFont;
using Microsoft.Xna;

namespace CnV.Draw
{
	public class SpriteBatch : GraphicsResource
	{
        #region Private Fields
        readonly SpriteBatcher _batcher;

			
	    public bool _beginCalled;

	
	//	RectangleF _tempRect = new RectangleF (0,0,0,0);
//		Vector2 _texCoordTL = new Vector2 (0,0);
//		Vector2 _texCoordBR = new Vector2 (0,0);
        #endregion

        /// <summary>
        /// Constructs a <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="graphicsDevice">The <see cref="GraphicsDevice"/>, which will be used for sprite rendering.</param>        
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graphicsDevice"/> is null.</exception>
 
        /// <summary>
        /// Constructs a <see cref="SpriteBatch"/>.
        /// </summary>
        /// <param name="graphicsDevice">The <see cref="GraphicsDevice"/>, which will be used for sprite rendering.</param>
        /// <param name="capacity">The initial capacity of the internal array holding batch items (the value will be rounded to the next multiple of 64).</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="graphicsDevice"/> is null.</exception>
        public SpriteBatch (GraphicsDevice graphicsDevice)
		{
			if (graphicsDevice == null)
            {
				throw new ArgumentNullException ("graphicsDevice", FrameworkResources.ResourceCreationWhenDeviceIsNull);
			}	

			this.GraphicsDevice = graphicsDevice;


            _batcher = new SpriteBatcher(graphicsDevice);

            _beginCalled = false;
		}

        /// <summary>
        /// Begins a new sprite and text batch with the specified render state.
        /// </summary>
        /// <param name="sortMode">The drawing order for sprite and text drawing. <see cref="SpriteSortMode.Deferred"/> by default.</param>
        /// <param name="blendState">State of the blending. Uses <see cref="BlendState.AlphaBlend"/> if null.</param>
        /// <param name="samplerState">State of the sampler. Uses <see cref="SamplerState.LinearClamp"/> if null.</param>
        /// <param name="depthStencilState">State of the depth-stencil buffer. Uses <see cref="DepthStencilState.None"/> if null.</param>
        /// <param name="rasterizerState">State of the rasterization. Uses <see cref="RasterizerState.CullCounterClockwise"/> if null.</param>
        /// <param name="effect">A custom <see cref="Effect"/> to override the default sprite effect. Uses default sprite effect if null.</param>
        /// <param name="transformMatrix">An optional matrix used to transform the sprite geometry. Uses <see cref="Matrix.Identity"/> if null.</param>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="Begin"/> is called next time without previous <see cref="End"/>.</exception>
        /// <remarks>This method uses optional parameters.</remarks>
        /// <remarks>The <see cref="Begin"/> Begin should be called before drawing commands, and you cannot call it again before subsequent <see cref="End"/>.</remarks>
        public void Begin
        (
          
        )
        {
          

            _beginCalled = true;
        }

        /// <summary>
        /// Flushes all batched text and sprites to the screen.
        /// </summary>
        /// <remarks>This command should be called after <see cref="Begin"/> and drawing commands.</remarks>
		public void End ()
		{
            if (!_beginCalled)
                throw new InvalidOperationException("Begin must be called before calling End.");

			_beginCalled = false;

			Setup();
            
            _batcher.DrawBatch();
		}
		
		void Setup() 
        {
		}
		
        //void CheckValid(Texture2D texture)
        //{
        //    if (texture == null)
        //        throw new ArgumentNullException("texture");
        //    if (!_beginCalled)
        //        throw new InvalidOperationException("Draw was called, but Begin has not yet been called. Begin must be called successfully before you can call Draw.");
        //}

        //void CheckValid(SpriteFont spriteFont, string text)
        //{
        //    if (spriteFont == null)
        //        throw new ArgumentNullException("spriteFont");
        //    if (text == null)
        //        throw new ArgumentNullException("text");
        //    if (!_beginCalled)
        //        throw new InvalidOperationException("DrawString was called, but Begin has not yet been called. Begin must be called successfully before you can call DrawString.");
        //}

        //void CheckValid(SpriteFont spriteFont, StringBuilder text)
        //{
        //    if (spriteFont == null)
        //        throw new ArgumentNullException("spriteFont");
        //    if (text == null)
        //        throw new ArgumentNullException("text");
        //    if (!_beginCalled)
        //        throw new InvalidOperationException("DrawString was called, but Begin has not yet been called. Begin must be called successfully before you can call DrawString.");
        //}

		//      /// <summary>
		//      /// Submit a sprite for drawing in the current batch.
		//      /// </summary>
		//      /// <param name="texture">A texture.</param>
		//      /// <param name="position">The drawing location on screen.</param>
		//      /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
		//      /// <param name="color">A color mask.</param>
		//      /// <param name="rotation">A rotation of this sprite.</param>
		//      /// <param name="origin">Center of the rotation. 0,0 by default.</param>
		//      /// <param name="scale">A scaling of this sprite.</param>
		//      /// <param name="effects">Modificators for drawing. Can be combined.</param>
		//      /// <param name="layerDepth">A depth of the layer of this sprite.</param>
		//public void Draw (Texture2D texture,
		//		Vector2 position,
		//		System.Drawing.RectangleF? sourceRectangle,
		//		Color color,
		//		float rotation,
		//		Vector2 origin,
		//		Vector2 scale,
		//		SpriteEffects effects,
		//              int layerDepth)
		//{
		//          CheckValid(texture);

		//          var item = _batcher.CreateBatchItem();
		//          item.Texture = texture;

		//	// set SortKey based on SpriteSortMode.
		//	item.SortKey = Layer.SortKey(layerDepth, texture);

		//	origin = origin * scale;

		//          float w, h;
		//          if (sourceRectangle.HasValue)
		//          {
		//              var srcRect = sourceRectangle.GetValueOrDefault();
		//              w = srcRect.Width * scale.X;
		//              h = srcRect.Height * scale.Y;
		//              _texCoordTL.X = srcRect.X * texture.TexelWidth;
		//              _texCoordTL.Y = srcRect.Y * texture.TexelHeight;
		//              _texCoordBR.X = (srcRect.X + srcRect.Width) * texture.TexelWidth;
		//              _texCoordBR.Y = (srcRect.Y + srcRect.Height) * texture.TexelHeight;
		//          }
		//          else
		//          {
		//              w = texture.Width * scale.X;
		//              h = texture.Height * scale.Y;
		//              _texCoordTL = Vector2.Zero;
		//              _texCoordBR = Vector2.One;
		//          }

		//          if ((effects & SpriteEffects.FlipVertically) != 0)
		//          {
		//              var temp = _texCoordBR.Y;
		//		_texCoordBR.Y = _texCoordTL.Y;
		//		_texCoordTL.Y = temp;
		//          }
		//          if ((effects & SpriteEffects.FlipHorizontally) != 0)
		//          {
		//              var temp = _texCoordBR.X;
		//		_texCoordBR.X = _texCoordTL.X;
		//		_texCoordTL.X = temp;
		//          }

		//          if (rotation == 0f)
		//          {
		//              item.Set(position.X - origin.X,
		//                      position.Y - origin.Y,
		//                      w,
		//                      h,
		//                      color,
		//                      _texCoordTL,
		//                      _texCoordBR,
		//                      layerDepth);
		//          }
		//          else
		//          {
		//              item.Set(position.X,
		//                      position.Y,
		//                      -origin.X,
		//                      -origin.Y,
		//                      w,
		//                      h,
		//                      (float)Math.Sin(rotation),
		//                      (float)Math.Cos(rotation),
		//                      color,
		//                      _texCoordTL,
		//                      _texCoordBR,
		//                      layerDepth);
		//          }

		//          FlushIfNeeded();
		//}

		//      /// <summary>
		//      /// Submit a sprite for drawing in the current batch.
		//      /// </summary>
		//      /// <param name="texture">A texture.</param>
		//      /// <param name="position">The drawing location on screen.</param>
		//      /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
		//      /// <param name="color">A color mask.</param>
		//      /// <param name="rotation">A rotation of this sprite.</param>
		//      /// <param name="origin">Center of the rotation. 0,0 by default.</param>
		//      /// <param name="scale">A scaling of this sprite.</param>
		//      /// <param name="effects">Modificators for drawing. Can be combined.</param>
		//      /// <param name="layerDepth">A depth of the layer of this sprite.</param>
		//public void Draw (Texture2D texture,
		//		Vector2 position,
		//		RectangleF? sourceRectangle,
		//		Color color,
		//		float rotation,
		//		Vector2 origin,
		//		float scale,
		//		SpriteEffects effects,
		//              int layerDepth)
		//{
		//          var scaleVec = new Vector2(scale, scale);
		//          Draw(texture, position, sourceRectangle, color, rotation, origin, scaleVec, effects, layerDepth);
		//}

		//      /// <summary>
		//      /// Submit a sprite for drawing in the current batch.
		//      /// </summary>
		//      /// <param name="texture">A texture.</param>
		//      /// <param name="destinationRectangle">The drawing bounds on screen.</param>
		//      /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
		//      /// <param name="color">A color mask.</param>
		//      /// <param name="rotation">A rotation of this sprite.</param>
		//      /// <param name="origin">Center of the rotation. 0,0 by default.</param>
		//      /// <param name="effects">Modificators for drawing. Can be combined.</param>
		//      /// <param name="layerDepth">A depth of the layer of this sprite.</param>
		//public void Draw (Texture2D texture,
		//	RectangleF destinationRectangle,
		//	RectangleF? sourceRectangle,
		//	Color color,
		//	float rotation,
		//	Vector2 origin,
		//	SpriteEffects effects,
		//          int layerDepth)
		//{
		//          CheckValid(texture);

		//          var item = _batcher.CreateBatchItem();
		//          item.Texture = texture;

		//	item.SortKey = Layer.SortKey(layerDepth, texture);

		//	if (sourceRectangle.HasValue)
		//          {
		//              var srcRect = sourceRectangle.GetValueOrDefault();
		//              _texCoordTL.X = srcRect.X * texture.TexelWidth;
		//              _texCoordTL.Y = srcRect.Y * texture.TexelHeight;
		//              _texCoordBR.X = (srcRect.X + srcRect.Width) * texture.TexelWidth;
		//              _texCoordBR.Y = (srcRect.Y + srcRect.Height) * texture.TexelHeight;

		//              if(srcRect.Width != 0)
		//                  origin.X = origin.X * (float)destinationRectangle.Width / (float)srcRect.Width;
		//              else
		//                  origin.X = origin.X * (float)destinationRectangle.Width * texture.TexelWidth;
		//              if(srcRect.Height != 0)
		//                  origin.Y = origin.Y * (float)destinationRectangle.Height / (float)srcRect.Height; 
		//              else
		//                  origin.Y = origin.Y * (float)destinationRectangle.Height * texture.TexelHeight;
		//          }
		//          else
		//          {
		//              _texCoordTL = Vector2.Zero;
		//              _texCoordBR = Vector2.One;

		//              origin.X = origin.X * (float)destinationRectangle.Width  * texture.TexelWidth;
		//              origin.Y = origin.Y * (float)destinationRectangle.Height * texture.TexelHeight;
		//          }

		//	if ((effects & SpriteEffects.FlipVertically) != 0)
		//          {
		//              var temp = _texCoordBR.Y;
		//		_texCoordBR.Y = _texCoordTL.Y;
		//		_texCoordTL.Y = temp;
		//	}
		//	if ((effects & SpriteEffects.FlipHorizontally) != 0)
		//          {
		//              var temp = _texCoordBR.X;
		//		_texCoordBR.X = _texCoordTL.X;
		//		_texCoordTL.X = temp;
		//	}

		//    if (rotation == 0f)
		//    {
		//              item.Set(destinationRectangle.X - origin.X,
		//                      destinationRectangle.Y - origin.Y,
		//                      destinationRectangle.Width,
		//                      destinationRectangle.Height,
		//                      color,
		//                      _texCoordTL,
		//                      _texCoordBR,
		//                      layerDepth);
		//          }
		//          else
		//    {
		//              item.Set(destinationRectangle.X,
		//                      destinationRectangle.Y,
		//                      -origin.X,
		//                      -origin.Y,
		//                      destinationRectangle.Width,
		//                      destinationRectangle.Height,
		//                      (float)Math.Sin(rotation),
		//                      (float)Math.Cos(rotation),
		//                      color,
		//                      _texCoordTL,
		//                      _texCoordBR,
		//                      layerDepth);
		//          }

		//	FlushIfNeeded();
		//}

		// Mark the end of a draw operation for Immediate SpriteSortMode.

		/// <summary>
		/// Submit a sprite for drawing in the current batch.
		/// </summary>
		/// <param name="texture">A texture.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
		/// <param name="color">A color mask.</param>
		//public void Draw (Texture2D texture, Vector2 position, RectangleF? sourceRectangle, Color color)
		//{
		//	CheckValid(texture);

		//	var item = _batcher.CreateBatchItem();
		//	item.Texture = texture;

		//	// set SortKey based on SpriteSortMode.
		//	item.SortKey = Layer.SortKey(Layer.effects, texture);

		//	Vector2 size;

		//          if (sourceRectangle.HasValue)
		//          {
		//              var srcRect = sourceRectangle.GetValueOrDefault();
		//              size = new Vector2(srcRect.Width, srcRect.Height);
		//              _texCoordTL.X = srcRect.X * texture.TexelWidth;
		//              _texCoordTL.Y = srcRect.Y * texture.TexelHeight;
		//              _texCoordBR.X = (srcRect.X + srcRect.Width)  * texture.TexelWidth;
		//              _texCoordBR.Y = (srcRect.Y + srcRect.Height) * texture.TexelHeight;
		//          }
		//          else
		//          {
		//              size = new Vector2(texture.width, texture.height);
		//              _texCoordTL = Vector2.Zero;
		//              _texCoordBR = Vector2.One;
		//          }

		//          item.Set(position.X,
		//                   position.Y,
		//                   size.X,
		//                   size.Y,
		//                   color,
		//                   _texCoordTL,
		//                   _texCoordBR,
		//                   0);

		//          FlushIfNeeded();
		//}

		//      /// <summary>
		//      /// Submit a sprite for drawing in the current batch.
		//      /// </summary>
		//      /// <param name="texture">A texture.</param>
		//      /// <param name="destinationRectangle">The drawing bounds on screen.</param>
		//      /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
		//      /// <param name="color">A color mask.</param>
		//public void Draw (Texture2D texture, RectangleF destinationRectangle, RectangleF? sourceRectangle, Color color)
		//{
		//          CheckValid(texture);

		//	var item = _batcher.CreateBatchItem();
		//	item.Texture = texture;

		//	// set SortKey based on SpriteSortMode.
		//	item.SortKey = Layer.SortKey(Layer.effects, texture);

		//	if (sourceRectangle.HasValue)
		//          {
		//              var srcRect = sourceRectangle.GetValueOrDefault();
		//              _texCoordTL.X = srcRect.X * texture.TexelWidth;
		//              _texCoordTL.Y = srcRect.Y * texture.TexelHeight;
		//              _texCoordBR.X = (srcRect.X + srcRect.Width) * texture.TexelWidth;
		//              _texCoordBR.Y = (srcRect.Y + srcRect.Height) * texture.TexelHeight;
		//          }
		//          else
		//          {
		//              _texCoordTL = Vector2.Zero;
		//              _texCoordBR = Vector2.One;
		//          }

		//          item.Set(destinationRectangle.X,
		//                   destinationRectangle.Y,
		//                   destinationRectangle.Width,
		//                   destinationRectangle.Height,
		//                   color,
		//                   _texCoordTL,
		//                   _texCoordBR,
		//                   0);

		//          FlushIfNeeded();
		//}

	
	public void AddMesh(Mesh m, int layer, Material material)
	{
		var list = _batcher.CreateBatchItemList(layer, material);
		list.meshes.Add(m);
	}
	
		
		//public void AddQuad(int layer, Material texture, Vector2 c0, Vector2 c1, Vector2 uv0, Vector2 uv1, Color color, (float depth00, float depth10,float depth01, float depth11) depth  )
		//{

		//	var item = _batcher.CreateBatchItem(layer, texture);


		//	item.Set(c0.X,
		//			 c0.Y,
		//			 c1.X,
		//			c1.Y,
		//			 color,
		//			 uv0,
		//			 uv1,
		//			 depth.depth00, depth.depth10, depth.depth01, depth.depth11);

		//}
		public void AddQuad(int layer, Material texture, Vector2 c0, Vector2 c1, Vector2 uv0, Vector2 uv1, Color color, float depth  )
		{

			 _batcher.AddBatchItem(layer, texture,new VertexSprite(c0,c1,depth,uv0,uv1,color) );
			
		}

		public void AddQuad(int layer, Material texture, Vector2 c0, Vector2 c1, Color color, float depth=0)
		{

			_batcher.AddBatchItem(layer, texture,new(c0,c1,depth,Vector2.Zero,Vector2.One,color));




		}
		public void AddQuadWithShadow(int layer,int shadowLayer, Material texture, Vector2 c0, Vector2 c1, Color color,Color shadowColor, float depth)
		{
			if(AGame.wantShadow)
				AddQuad(shadowLayer, texture, c0+GameClient.shadowOffsetW, c1+GameClient.shadowOffsetW, shadowColor, View.zShadow);
			AddQuad(layer, texture, c0, c1, color,depth: depth);

		}
		
		// Thickness is in screen space
		public  void AddLine(int layer,Material texture, Vector2 c0, Vector2 c1, float thickness, float u0, float u1, Color color,(float v0,float v1) depth)
		{
			
				_batcher.AddBatchItem(layer,texture,new(c0,c1,depth.v0,new(u0,0),new(u1,1),color,thickness.ScreenToWorld()));
			return;

			//var item = _batcher.CreateBatchItem(layer, texture);

			//var dc0 = c1 - c0;
			//var dc1 = new Vector2(dc0.Y, -dc0.X);
			//dc1 *= (thickness * 0.5f / dc1.Length()).ScreenToWorld();


			

   //         item.vertexTL=new VertexPositionColorTexture(
			//		new Vector3( c0.X - dc1.X,
			//		c0.Y - dc1.Y,
			//		depth.v0),
			//		color,
			//		new Microsoft.Xna.Framework.Vector2(u0,
			//		0)
					
			//	);
			//item.vertexTR = new VertexPositionColorTexture(
			//   new Vector3(c0.X + dc1.X,
			//		c0.Y + dc1.Y,
			//		depth.v0),
			//		color,
			//		new Microsoft.Xna.Framework.Vector2(u0,
			//		1)
			//	);
			//item.vertexBL = new VertexPositionColorTexture(
			//	   new Vector3(c1.X - dc1.X,
			//		c1.Y - dc1.Y,
			//		depth.v1),
			//		color,
			//		new Microsoft.Xna.Framework.Vector2(u1,
			//		0)

			//	);
			//item.vertexBR = new VertexPositionColorTexture(
			//	   new Vector3(c1.X + dc1.X,
			//		c1.Y + dc1.Y,
			//		depth.v1),
			//		color,
			//		new Microsoft.Xna.Framework.Vector2(u1,
			//		1)

			//	);

			}
		
		/// <summary>
		/// Submit a sprite for drawing in the current batch.
		/// </summary>
		/// <param name="texture">A texture.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		//public void Draw (Texture2D texture, Vector2 position, Color color)
		//{
		//	CheckValid(texture);
            
		//	var item = _batcher.CreateBatchItem();
		//	item.Texture = texture;
            
  //          // set SortKey based on SpriteSortMode.
  //         item.SortKey = Layer.SortKey(Layer.effects, texture);
            
  //          item.Set(position.X,
  //                   position.Y,
  //                   texture.Width,
  //                   texture.Height,
  //                   color,
  //                   Vector2.Zero,
  //                   Vector2.One,
  //                   0);

  //          FlushIfNeeded();
		//}

  //      /// <summary>
  //      /// Submit a sprite for drawing in the current batch.
  //      /// </summary>
  //      /// <param name="texture">A texture.</param>
  //      /// <param name="destinationRectangle">The drawing bounds on screen.</param>
  //      /// <param name="color">A color mask.</param>
  //      public void Draw(Texture2D texture, RectangleF destinationRectangle, Color color)
		//{
  //          CheckValid(texture);
            
  //          var item = _batcher.CreateBatchItem();
  //          item.Texture = texture;
            
  //          // set SortKey based on SpriteSortMode.
  //         item.SortKey = Layer.SortKey(Layer.effects, texture);
            
  //          item.Set(destinationRectangle.X,
  //                   destinationRectangle.Y,
  //                   destinationRectangle.Width,
  //                   destinationRectangle.Height,
  //                   color,
  //                   Vector2.Zero,
  //                   Vector2.One,
  //                   0);
            
  //          FlushIfNeeded();
		//}

        /// <summary>
        /// Submit a text string of sprites for drawing in the current batch.
        /// </summary>
        /// <param name="spriteFont">A font.</param>
        /// <param name="text">The text which will be drawn.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="color">A color mask.</param>
	

		public unsafe void DrawString(BitmapFont.BitmapFont me, string text, Vector2 position, float scale, Color color, int layer, float z, float maxWidth = -1f)
		{
			//var   isDark      = color.IsDark();
			var   material    =  GameClient.fontMaterial;
			var   texture     = GameClient.fontTexture;
			float texelWidth  = texture.TexelWidth;
			float texelHeight = texture.TexelHeight;
			var   drawY       = 0;
			if (!string.IsNullOrEmpty(text))
			{
				char previousCharacter = ' ';
				int drawX=0;
				int length= text.Length;;

				
				
				for (int i = 0; i < length; i++)
				{

					var character = text[i];

					if (character == '\n' || character == '\r')
					{
						if (character == '\n' || i + 1 == length || text[i + 1] != '\n')
						{
							drawY += me.LineHeight;
							drawX = 0;
						}
					}
					else
					{
						

						var data = me[character];
						var kern = me.GetKerning(previousCharacter, character);
						var width = data.XAdvance + kern;

						if (maxWidth != BitmapFont.BitmapFont.NoMaxWidth && drawX + width >= maxWidth)
						{
							drawY += me.LineHeight;
							drawX = 0;
						}
						var p = new Vector2(drawX,drawY);
						p.X += data.XOffset+kern;
						p.Y += data.YOffset;
						

						Vector2 _texCoordTL, _texCoordBR;

						_texCoordTL.X = data.X * texelWidth;
						_texCoordTL.Y = data.Y*texelHeight;
						_texCoordBR.X = (data.X+data.Width) *texelWidth;
						_texCoordBR.Y = (data.Y+data.Height)*texelHeight;
						var x0 = p.X* scale + position.X;
						var y0 = p.Y* scale + position.Y;
						var item = new VertexSprite(new(x0,y0),new(x0+data.Width*scale,y0+data.Height*scale),z,_texCoordTL,_texCoordBR,color);

						_batcher.AddBatchItem(layer, material,item);
						
						drawX += width;
						previousCharacter = character;
					}
				}

				// finish off the current line if required
				

			}
			else
			{
			}


		}
		//internal void AddQuad(int layer,TextureSection textureSection, Vector2 destP0, Vector2 destP1, Color color)
		//{
		//	AddQuad(layer,textureSection.texture, destP0, destP1, textureSection.uv0, textureSection.uv1, color);
		//}

		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="spriteFont">A font.</param>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="rotation">A rotation of this string.</param>
		/// <param name="origin">Center of the rotation. 0,0 by default.</param>
		/// <param name="scale">A scaling of this string.</param>
		/// <param name="effects">Modificators for drawing. Can be combined.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		//public void DrawString (
		//	SpriteFont spriteFont,  string text, Vector2 position, Color color,
		//          float rotation, Vector2 origin, float scale, SpriteEffects effects, int layerDepth, int layer = Layer.labelText)
		//{
		//	var scaleVec = new Vector2(scale, scale);
		//          DrawString(spriteFont, text, position, color, rotation, origin, scaleVec, effects, layerDepth,layer);
		//}

		//      /// <summary>
		//      /// Submit a text string of sprites for drawing in the current batch.
		//      /// </summary>
		//      /// <param name="spriteFont">A font.</param>
		//      /// <param name="text">The text which will be drawn.</param>
		//      /// <param name="position">The drawing location on screen.</param>
		//      /// <param name="color">A color mask.</param>
		//      /// <param name="rotation">A rotation of this string.</param>
		//      /// <param name="origin">Center of the rotation. 0,0 by default.</param>
		//      /// <param name="scale">A scaling of this string.</param>
		//      /// <param name="effects">Modificators for drawing. Can be combined.</param>
		//      /// <param name="layerDepth">A depth of the layer of this string.</param>
		//public unsafe void DrawString (
		//	SpriteFont spriteFont,  string text, Vector2 position, Color color,
		//          float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, int layerDepth)
		//{
		//          CheckValid(spriteFont, text);


		//	var flipAdjustment = Vector2.Zero;

		//          var flippedVert = (effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically;
		//          var flippedHorz = (effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;

		//          if (flippedVert || flippedHorz)
		//          {
		//              Microsoft.Xna.Framework.Vector2 size;

		//              var source = new SpriteFont.CharacterSource(text);
		//              spriteFont.MeasureString(ref source, out size);

		//              if (flippedHorz)
		//              {
		//                  origin.X *= -1;
		//                  flipAdjustment.X = -size.X;
		//              }

		//              if (flippedVert)
		//              {
		//                  origin.Y *= -1;
		//                  flipAdjustment.Y = spriteFont.LineSpacing - size.Y;
		//              }
		//          }

		//          Matrix transformation = Matrix.Identity;
		//          float cos = 0, sin = 0;
		//          if (rotation == 0)
		//          {
		//              transformation.M11 = (flippedHorz ? -scale.X : scale.X);
		//              transformation.M22 = (flippedVert ? -scale.Y : scale.Y);
		//              transformation.M41 = ((flipAdjustment.X - origin.X) * transformation.M11) + position.X;
		//              transformation.M42 = ((flipAdjustment.Y - origin.Y) * transformation.M22) + position.Y;
		//          }
		//          else
		//          {
		//              cos = (float)Math.Cos(rotation);
		//              sin = (float)Math.Sin(rotation);
		//              transformation.M11 = (flippedHorz ? -scale.X : scale.X) * cos;
		//              transformation.M12 = (flippedHorz ? -scale.X : scale.X) * sin;
		//              transformation.M21 = (flippedVert ? -scale.Y : scale.Y) * (-sin);
		//              transformation.M22 = (flippedVert ? -scale.Y : scale.Y) * cos;
		//              transformation.M41 = (((flipAdjustment.X - origin.X) * transformation.M11) + (flipAdjustment.Y - origin.Y) * transformation.M21) + position.X;
		//              transformation.M42 = (((flipAdjustment.X - origin.X) * transformation.M12) + (flipAdjustment.Y - origin.Y) * transformation.M22) + position.Y; 
		//          }

		//          var offset = Microsoft.Xna.Framework.Vector2.Zero;
		//          var firstGlyphOfLine = true;

		//          fixed (SpriteFont.Glyph* pGlyphs = spriteFont.Glyphs)
		//          for (var i = 0; i < text.Length; ++i)
		//          {
		//              var c = text[i];

		//              if (c == '\r')
		//                  continue;

		//              if (c == '\n')
		//              {
		//                  offset.X = 0;
		//                  offset.Y += spriteFont.LineSpacing;
		//                  firstGlyphOfLine = true;
		//                  continue;
		//              }

		//              var currentGlyphIndex = spriteFont.GetGlyphIndexOrDefault(c);
		//              var pCurrentGlyph = pGlyphs + currentGlyphIndex;

		//              // The first character on a line might have a negative left side bearing.
		//              // In this scenario, SpriteBatch/SpriteFont normally offset the text to the right,
		//              //  so that text does not hang off the left side of its rectangle.
		//              if (firstGlyphOfLine)
		//              {
		//                  offset.X = Math.Max(pCurrentGlyph->LeftSideBearing, 0);
		//                  firstGlyphOfLine = false;
		//              }
		//              else
		//              {
		//                  offset.X += spriteFont.Spacing + pCurrentGlyph->LeftSideBearing;
		//              }

		//              var p = offset;

		//              if (flippedHorz)
		//                  p.X += pCurrentGlyph->BoundsInTexture.Width;
		//              p.X += pCurrentGlyph->Cropping.X;

		//              if (flippedVert)
		//                  p.Y += pCurrentGlyph->BoundsInTexture.Height - spriteFont.LineSpacing;
		//              p.Y += pCurrentGlyph->Cropping.Y;

		//              Microsoft.Xna.Framework.Vector2.Transform(ref p, ref transformation, out p);

		//			var item = _batcher.CreateBatchItem(layer, spriteFont.defaultMaterial as Material) ;    

		//              _texCoordTL.X = pCurrentGlyph->BoundsInTexture.X * spriteFont.Texture.TexelWidth;
		//              _texCoordTL.Y = pCurrentGlyph->BoundsInTexture.Y * spriteFont.Texture.TexelHeight;
		//              _texCoordBR.X = (pCurrentGlyph->BoundsInTexture.X + pCurrentGlyph->BoundsInTexture.Width) * spriteFont.Texture.TexelWidth;
		//              _texCoordBR.Y = (pCurrentGlyph->BoundsInTexture.Y + pCurrentGlyph->BoundsInTexture.Height) * spriteFont.Texture.TexelHeight;

		//              if ((effects & SpriteEffects.FlipVertically) != 0)
		//              {
		//                  var temp = _texCoordBR.Y;
		//		    _texCoordBR.Y = _texCoordTL.Y;
		//		    _texCoordTL.Y = temp;
		//	    }
		//              if ((effects & SpriteEffects.FlipHorizontally) != 0)
		//              {
		//                  var temp = _texCoordBR.X;
		//		    _texCoordBR.X = _texCoordTL.X;
		//		    _texCoordTL.X = temp;
		//	    }

		//              if (rotation == 0f)
		//              {
		//                  item.Set(p.X,
		//                          p.Y,
		//                          pCurrentGlyph->BoundsInTexture.Width * scale.X,
		//                          pCurrentGlyph->BoundsInTexture.Height * scale.Y,
		//                          color,
		//                          _texCoordTL,
		//                          _texCoordBR,
		//                          layerDepth);
		//              }
		//              else
		//              {
		//                  item.Set(p.X,
		//                          p.Y,
		//                          0,
		//                          0,
		//                          pCurrentGlyph->BoundsInTexture.Width * scale.X,
		//                          pCurrentGlyph->BoundsInTexture.Height * scale.Y,
		//                          sin,
		//                          cos,
		//                          color,
		//                          _texCoordTL,
		//                          _texCoordBR,
		//                          layerDepth);
		//              }

		//              offset.X += pCurrentGlyph->Width + pCurrentGlyph->RightSideBearing;
		//          }

		//	// We need to flush if we're using Immediate sort mode.
		//}

		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="spriteFont">A font.</param>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		//public unsafe void DrawString (SpriteFont spriteFont,  StringBuilder text, Vector2 position, Color color, int layer = Layer.labelText)
		//{
		//          CheckValid(spriteFont, text);


		//	var offset = Vector2.Zero;
		//          var firstGlyphOfLine = true;

		//          fixed (SpriteFont.Glyph* pGlyphs = spriteFont.Glyphs)
		//          for (var i = 0; i < text.Length; ++i)
		//          {
		//              var c = text[i];

		//              if (c == '\r')
		//                  continue;

		//              if (c == '\n')
		//              {
		//                  offset.X = 0;
		//                  offset.Y += spriteFont.LineSpacing;
		//                  firstGlyphOfLine = true;
		//                  continue;
		//              }

		//              var currentGlyphIndex = spriteFont.GetGlyphIndexOrDefault(c);
		//              var pCurrentGlyph = pGlyphs + currentGlyphIndex;

		//              // The first character on a line might have a negative left side bearing.
		//              // In this scenario, SpriteBatch/SpriteFont normally offset the text to the right,
		//              //  so that text does not hang off the left side of its rectangle.
		//              if (firstGlyphOfLine)
		//              {
		//                  offset.X = Math.Max(pCurrentGlyph->LeftSideBearing, 0);
		//                  firstGlyphOfLine = false;
		//              }
		//              else
		//              {
		//                  offset.X += spriteFont.Spacing + pCurrentGlyph->LeftSideBearing;
		//              }

		//              var p = offset;                
		//              p.X += pCurrentGlyph->Cropping.X;
		//              p.Y += pCurrentGlyph->Cropping.Y;
		//              p += position;

		//              var item = _batcher.CreateBatchItem(layer, spriteFont.defaultMaterial as Material);

		//              _texCoordTL.X = pCurrentGlyph->BoundsInTexture.X * spriteFont.Texture.TexelWidth;
		//              _texCoordTL.Y = pCurrentGlyph->BoundsInTexture.Y * spriteFont.Texture.TexelHeight;
		//              _texCoordBR.X = (pCurrentGlyph->BoundsInTexture.X + pCurrentGlyph->BoundsInTexture.Width) * spriteFont.Texture.TexelWidth;
		//              _texCoordBR.Y = (pCurrentGlyph->BoundsInTexture.Y + pCurrentGlyph->BoundsInTexture.Height) * spriteFont.Texture.TexelHeight;

		//              item.Set(p.X,
		//                       p.Y,
		//                       pCurrentGlyph->BoundsInTexture.Width,
		//                       pCurrentGlyph->BoundsInTexture.Height,
		//                       color,
		//                       _texCoordTL,
		//                       _texCoordBR,
		//                       0);

		//              offset.X += pCurrentGlyph->Width + pCurrentGlyph->RightSideBearing;
		//          }

		//	// We need to flush if we're using Immediate sort mode.
		//}

		/// <summary>
		/// Submit a text string of sprites for drawing in the current batch.
		/// </summary>
		/// <param name="spriteFont">A font.</param>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="rotation">A rotation of this string.</param>
		/// <param name="origin">Center of the rotation. 0,0 by default.</param>
		/// <param name="scale">A scaling of this string.</param>
		/// <param name="effects">Modificators for drawing. Can be combined.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		//public void DrawString (
		//	SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color,
		//          float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		//{
		//	var scaleVec = new Vector2 (scale, scale);
		//          DrawString(spriteFont, text, position, color, rotation, origin, scaleVec, effects, layerDepth);
		//}

		//      /// <summary>
		//      /// Submit a text string of sprites for drawing in the current batch.
		//      /// </summary>
		//      /// <param name="spriteFont">A font.</param>
		//      /// <param name="text">The text which will be drawn.</param>
		//      /// <param name="position">The drawing location on screen.</param>
		//      /// <param name="color">A color mask.</param>
		//      /// <param name="rotation">A rotation of this string.</param>
		//      /// <param name="origin">Center of the rotation. 0,0 by default.</param>
		//      /// <param name="scale">A scaling of this string.</param>
		//      /// <param name="effects">Modificators for drawing. Can be combined.</param>
		//      /// <param name="layerDepth">A depth of the layer of this string.</param>
		//public unsafe void DrawString (
		//	SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color,
		//          float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		//{
		//          CheckValid(spriteFont, text);

		//	int sortKey = Layer.SortKey(Layer.text, spriteFont.Texture);
		//	var flipAdjustment = Vector2.Zero;

		//          var flippedVert = (effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically;
		//          var flippedHorz = (effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;

		//          if (flippedVert || flippedHorz)
		//          {
		//              var source = new SpriteFont.CharacterSource(text);
		//		Microsoft.Xna.Framework.Vector2 size;
		//              spriteFont.MeasureString(ref source, out size);

		//              if (flippedHorz)
		//              {
		//                  origin.X *= -1;
		//                  flipAdjustment.X = -size.X;
		//              }

		//              if (flippedVert)
		//              {
		//                  origin.Y *= -1;
		//                  flipAdjustment.Y = spriteFont.LineSpacing - size.Y;
		//              }
		//          }

		//          Matrix transformation = Matrix.Identity;
		//          float cos = 0, sin = 0;
		//          if (rotation == 0)
		//          {
		//              transformation.M11 = (flippedHorz ? -scale.X : scale.X);
		//              transformation.M22 = (flippedVert ? -scale.Y : scale.Y);
		//              transformation.M41 = ((flipAdjustment.X - origin.X) * transformation.M11) + position.X;
		//              transformation.M42 = ((flipAdjustment.Y - origin.Y) * transformation.M22) + position.Y;
		//          }
		//          else
		//          {
		//              cos = (float)Math.Cos(rotation);
		//              sin = (float)Math.Sin(rotation);
		//              transformation.M11 = (flippedHorz ? -scale.X : scale.X) * cos;
		//              transformation.M12 = (flippedHorz ? -scale.X : scale.X) * sin;
		//              transformation.M21 = (flippedVert ? -scale.Y : scale.Y) * (-sin);
		//              transformation.M22 = (flippedVert ? -scale.Y : scale.Y) * cos;
		//              transformation.M41 = (((flipAdjustment.X - origin.X) * transformation.M11) + (flipAdjustment.Y - origin.Y) * transformation.M21) + position.X;
		//              transformation.M42 = (((flipAdjustment.X - origin.X) * transformation.M12) + (flipAdjustment.Y - origin.Y) * transformation.M22) + position.Y; 
		//          }

		//          var offset = Microsoft.Xna.Framework.Vector2.Zero;
		//          var firstGlyphOfLine = true;

		//          fixed (SpriteFont.Glyph* pGlyphs = spriteFont.Glyphs)
		//          for (var i = 0; i < text.Length; ++i)
		//          {
		//              var c = text[i];

		//              if (c == '\r')
		//                  continue;

		//              if (c == '\n')
		//              {
		//                  offset.X = 0;
		//                  offset.Y += spriteFont.LineSpacing;
		//                  firstGlyphOfLine = true;
		//                  continue;
		//              }

		//              var currentGlyphIndex = spriteFont.GetGlyphIndexOrDefault(c);
		//              var pCurrentGlyph = pGlyphs + currentGlyphIndex;

		//              // The first character on a line might have a negative left side bearing.
		//              // In this scenario, SpriteBatch/SpriteFont normally offset the text to the right,
		//              //  so that text does not hang off the left side of its rectangle.
		//              if (firstGlyphOfLine)
		//              {
		//                  offset.X = Math.Max(pCurrentGlyph->LeftSideBearing, 0);
		//                  firstGlyphOfLine = false;
		//              }
		//              else
		//              {
		//                  offset.X += spriteFont.Spacing + pCurrentGlyph->LeftSideBearing;
		//              }

		//              var p = offset;

		//              if (flippedHorz)
		//                  p.X += pCurrentGlyph->BoundsInTexture.Width;
		//              p.X += pCurrentGlyph->Cropping.X;

		//              if (flippedVert)
		//                  p.Y += pCurrentGlyph->BoundsInTexture.Height - spriteFont.LineSpacing;
		//              p.Y += pCurrentGlyph->Cropping.Y;

		//              Microsoft.Xna.Framework.Vector2.Transform(ref p, ref transformation, out p);

		//              var item = _batcher.CreateBatchItem(Layer.text, spriteFont.defaultMaterial as Material);               

		//              _texCoordTL.X = pCurrentGlyph->BoundsInTexture.X * (float)spriteFont.Texture.TexelWidth;
		//              _texCoordTL.Y = pCurrentGlyph->BoundsInTexture.Y * (float)spriteFont.Texture.TexelHeight;
		//              _texCoordBR.X = (pCurrentGlyph->BoundsInTexture.X + pCurrentGlyph->BoundsInTexture.Width) * (float)spriteFont.Texture.TexelWidth;
		//              _texCoordBR.Y = (pCurrentGlyph->BoundsInTexture.Y + pCurrentGlyph->BoundsInTexture.Height) * (float)spriteFont.Texture.TexelHeight;

		//              if ((effects & SpriteEffects.FlipVertically) != 0)
		//              {
		//                  var temp = _texCoordBR.Y;
		//		    _texCoordBR.Y = _texCoordTL.Y;
		//		    _texCoordTL.Y = temp;
		//	    }
		//              if ((effects & SpriteEffects.FlipHorizontally) != 0)
		//              {
		//                  var temp = _texCoordBR.X;
		//		    _texCoordBR.X = _texCoordTL.X;
		//		    _texCoordTL.X = temp;
		//	    }

		//              if (rotation == 0f)
		//              {
		//                  item.Set(p.X,
		//                          p.Y,
		//                          pCurrentGlyph->BoundsInTexture.Width * scale.X,
		//                          pCurrentGlyph->BoundsInTexture.Height * scale.Y,
		//                          color,
		//                          _texCoordTL,
		//                          _texCoordBR,
		//                          layerDepth);
		//              }
		//              else
		//              {
		//                  item.Set(p.X,
		//                          p.Y,
		//                          0,
		//                          0,
		//                          pCurrentGlyph->BoundsInTexture.Width * scale.X,
		//                          pCurrentGlyph->BoundsInTexture.Height * scale.Y,
		//                          sin,
		//                          cos,
		//                          color,
		//                          _texCoordTL,
		//                          _texCoordBR,
		//                          layerDepth);
		//              }

		//              offset.X += pCurrentGlyph->Width + pCurrentGlyph->RightSideBearing;
		//	}


		//}

	}
}

