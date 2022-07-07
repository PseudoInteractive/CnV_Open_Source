// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace CnV.Draw
{
	/// <summary>
	/// This class handles the queueing of batch items into the GPU by creating the triangle tesselations
	/// that are used to draw the sprite textures. This class supports int.MaxValue number of sprites to be
	/// batched and will process them into short.MaxValue groups (strided by 6 for the number of vertices
	/// sent to the GPU). 
	/// </summary>
	internal class SpriteBatcher
	{
		/*
         * Note that this class is fundamental to high performance for SpriteBatch games. Please exercise
         * caution when making changes to this class.
         */

		/// <summary>
		/// Initialization size for the batch item list and queue.
		/// </summary>
		/// The maximum number of batch items that can be processed per iteration
		/// </summary>
		public const int MaxBatchSize = short.MaxValue / 6; // 6 = 4 vertices unique and 2 shared, per quad, why is this not maxValue/4?
		/// <summary>
		/// Initialization size for the vertex array, in batch units.
		/// </summary>
		public const int VertexArraySize = MaxBatchSize * 4;
		public const int IndexArraySize = MaxBatchSize * 6;


		/// <summary>
		/// The list of batch items to process.
		/// first index is
		///           layer
		///      then shader
		///      then texture
		///      then batch
		/// </summary>
		public const int layerCount = 255;
		private SortedList<int, SortedList<int, SpriteBatchItemList>>[] _batchItemList;


		/// <summary>
		/// The target graphics device.
		/// </summary>
		private readonly GraphicsDevice _device;

		/// <summary>
		/// Vertex index array. The values in this array never change.
		/// </summary>
		//	private short[] _index;

		// staging area, verts are copied here first and then copied into a mapped graphics resource
		// TODO:  remove this and skip a copy
		//
		//		private Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture[] _vertexArray;

		public SpriteBatcher(GraphicsDevice device)
		{
			_device = device;



			_batchItemList = new SortedList<int,SortedList<int,SpriteBatchItemList>>[layerCount];
			for(int i=0;i<layerCount;++i)
			{
				_batchItemList[i] = new();
			}
			//			EnsureArrayCapacity();
		}

		/// <summary>
		/// Reuse a previously allocated SpriteBatchItem from the item pool. 
		/// if there is none available grow the pool and initialize new items.
		/// </summary>
		/// <returns></returns>
		public void AddBatchItem(int layer, Material material,VertexSprite v)
		{
			Assert(material.effect.technique is not null);
			var list = CreateBatchItemList(layer, material);
	
			list.sprites.Add(v);
		}
		
		public SpriteBatchItemList CreateBatchItemList(int layer, Material material)
		{
			Assert(layer >= 0);
			Assert(layer < layerCount);

			var batch = _batchItemList[layer];
			if (!batch.TryGetValue(material.effect._sortingKey, out var perEffect))
			{
				perEffect = new();
				batch.Add(material.effect._sortingKey, perEffect);
			}

			var textureKey = material.texture != null ? material.texture._sortingKey : 0;
			if (!perEffect.TryGetValue(textureKey, out var list))
			{
				list = SpriteBatchItemList.Alloc(material);
				perEffect.Add(textureKey, list);
			}
			return list;
		}



		/// <summary>
		/// Resize and recreate the missing indices for the index and vertex position color buffers.
		/// </summary>
		/// <param name="numBatchItems"></param>
		//private unsafe void EnsureArrayCapacity()
		//{
		//	const int numBatchItems = MaxBatchSize;
		//	int neededCapacity = 6 * numBatchItems;

		//	short[] newIndex = new short[6 * numBatchItems];
		//	int start = 0;

		//	fixed (short* indexFixedPtr = newIndex)
		//	{
		//		var indexPtr = indexFixedPtr + (start * 6);
		//		for (var i = start; i < numBatchItems; i++, indexPtr += 6)
		//		{
		//			/*
		//                   *  TL    TR
		//                   *   0----1 0,1,2,3 = index offsets for vertex indices
		//                   *   |   /| TL,TR,BL,BR are vertex references in SpriteBatchItem.
		//                   *   |  / |
		//                   *   | /  |
		//                   *   |/   |
		//                   *   2----3
		//                   *  BL    BR
		//                   */
		//			// Triangle 1
		//			*(indexPtr + 0) = (short)(i * 4);
		//			*(indexPtr + 1) = (short)(i * 4 + 1);
		//			*(indexPtr + 2) = (short)(i * 4 + 2);
		//			// Triangle 2
		//			*(indexPtr + 3) = (short)(i * 4 + 1);
		//			*(indexPtr + 4) = (short)(i * 4 + 3);
		//			*(indexPtr + 5) = (short)(i * 4 + 2);
		//		}
		//	}
		//	_index = newIndex;

		//	//_vertexArray = new VertexPositionColorTexture[4 * numBatchItems];
		//}

		/// <summary>
		/// Sorts the batch items and then groups batch drawing into maximal allowed batch sets that do not
		/// overflow the 16 bit array indices for vertices.
		/// </summary>
		/// <param name="sortMode">The type of depth sorting desired for the rendering.</param>
		/// <param name="effect">The custom effect to apply to the drawn geometry</param>
		public unsafe void DrawBatch()
		{


			// Determine how many iterations through the drawing code we need to make
			// Iterate through the batches, doing short.MaxValue sets of vertices only.
			EffectPass lastEffectPass = null;
			//Texture lastTexture0 = null;
			//Texture lastTexture1 = null;
			for(int i=0;i<layerCount;++i)
			{
				var layer = _batchItemList[i];
	//			GameClient.instance.GraphicsDevice.BlendState = (layer.Key == Layer.webView) ?  BlendState.Opaque : BlendState.AlphaBlend;
				foreach (var _effect in layer)
				{
					foreach(var _list in _effect.Value)
					{
						var list = _list.Value;
						var material = list.material;
						// setup the vertexArray array

						int numBatchesToProcess = list.sprites.Count;
						if (numBatchesToProcess > MaxBatchSize)
						{
							numBatchesToProcess = MaxBatchSize;
						}
						// Avoid the array checking overhead by using pointer indexing!
						//						fixed (VertexPositionColorTexture* vertexArrayFixedPtr = _vertexArray)
						{
							//						var vertexArrayPtr = vertexArrayFixedPtr;
							
							// Draw the batches
							

							
							var pass = material.effect;
							if(lastEffectPass != pass)
							{
								lastEffectPass =pass;
								if(pass != null)
								{
									Assert(pass.technique is not null);
									if(pass.technique is not null) {
										pass._effect.CurrentTechnique = pass.technique;

										
									}
									pass.Apply();
								}
								//for(int i1 = 0;i1<4;++i1)
								//	_device.Textures[i1] = null;
							}
							var t0 = material.texture;
							var t1 = material.texture1;
							//if(t0 is not null)
							//{
							//	if(t0.IsAnimated())
							//	{
							//		_device.Textures[2] = t0;
							//		if(t1 is not null)
							//			_device.Textures[3] = t1;
							//	}
							//	else
								{
									if(t0 is not null)
										_device.Textures[0] = t0;
									if(t1 is not null)
										_device.Textures[1] = t1;

								}
							//}
							if (list.sprites.Count>0)
							{
								_device.DrawUserSprites(
										list.sprites,
										VertexSprite.VertexDeclaration);
							}
							foreach (var mesh in list.meshes)
							{
								_device.SetVertexBuffers(null );
								if(mesh.ib is not null)
									_device.SetIndexBuffer(mesh.ib);
								Assert(mesh.vb is not null);
								if(mesh.vb2 is not null)
									_device.SetVertexBuffers( new(mesh.vb), new(mesh.vb2) );
								else
									_device.SetVertexBuffer(mesh.vb);
								
								_device.DrawPrimitives(PrimitiveType.PointList,mesh.baseVertex, mesh.primitiveCount);

								_device.SetVertexBuffers(null );
							}

							list.Release();
						}
					}

					_effect.Value.Clear();
				}

				layer.Clear();
				// return items to the pool.  
			}
		}

	}
}


