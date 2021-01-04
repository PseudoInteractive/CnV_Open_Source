// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace COTG.Draw
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
		public const int VertexArraySize = MaxBatchSize*4;
		public const int IndexArraySize = MaxBatchSize * 6;


		/// <summary>
		/// The list of batch items to process.
		/// first index is
		///           layer
		///      then shader
		///      then texture
		///      then batch
		/// </summary>
		private SortedList<int, Dictionary<int,Dictionary<int, SpriteBatchItemList>>> _batchItemList;


		/// <summary>
		/// The target graphics device.
		/// </summary>
		private readonly GraphicsDevice _device;

		/// <summary>
		/// Vertex index array. The values in this array never change.
		/// </summary>
		private short[] _index;

		// staging area, verts are copied here first and then copied into a mapped graphics resource
		// TODO:  remove this and skip a copy
		//
		private Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture[] _vertexArray;

		public SpriteBatcher(GraphicsDevice device)
		{
			_device = device;



			_batchItemList = new SortedList<int, Dictionary<int, Dictionary<int, SpriteBatchItemList>>>();

			EnsureArrayCapacity();
		}

		/// <summary>
		/// Reuse a previously allocated SpriteBatchItem from the item pool. 
		/// if there is none available grow the pool and initialize new items.
		/// </summary>
		/// <returns></returns>
		public SpriteBatchItem CreateBatchItem(int layer, Material material)
		{
			if (!_batchItemList.TryGetValue(layer, out var batch))
			{
				batch = new Dictionary<int,Dictionary<int, SpriteBatchItemList> > ();
				_batchItemList.Add(layer, batch);
			}
			if (!batch.TryGetValue(material.effect._sortingKey, out var perEffect))
			{
				perEffect = new Dictionary<int, SpriteBatchItemList>();
				batch.Add(material.effect._sortingKey, perEffect);
			}
			if (!perEffect.TryGetValue(material.texture._sortingKey, out var list))
			{
				list = new SpriteBatchItemList( material );
				perEffect.Add(material.texture._sortingKey, list);
			}

			var rv = new SpriteBatchItem();
			list.sprites.Add(rv);
			return rv;
		}

		/// <summary>
		/// Resize and recreate the missing indices for the index and vertex position color buffers.
		/// </summary>
		/// <param name="numBatchItems"></param>
		private unsafe void EnsureArrayCapacity()
		{
			const int numBatchItems = MaxBatchSize;
			int neededCapacity = 6 * numBatchItems;
		
			short[] newIndex = new short[6 * numBatchItems];
			int start = 0;
			
			fixed (short* indexFixedPtr = newIndex)
			{
				var indexPtr = indexFixedPtr + (start * 6);
				for (var i = start; i < numBatchItems; i++, indexPtr += 6)
				{
					/*
                     *  TL    TR
                     *   0----1 0,1,2,3 = index offsets for vertex indices
                     *   |   /| TL,TR,BL,BR are vertex references in SpriteBatchItem.
                     *   |  / |
                     *   | /  |
                     *   |/   |
                     *   2----3
                     *  BL    BR
                     */
					// Triangle 1
					*(indexPtr + 0) = (short)(i * 4);
					*(indexPtr + 1) = (short)(i * 4 + 1);
					*(indexPtr + 2) = (short)(i * 4 + 2);
					// Triangle 2
					*(indexPtr + 3) = (short)(i * 4 + 1);
					*(indexPtr + 4) = (short)(i * 4 + 3);
					*(indexPtr + 5) = (short)(i * 4 + 2);
				}
			}
			_index = newIndex;

			_vertexArray = new VertexPositionColorTexture[4 * numBatchItems];
		}

		/// <summary>
		/// Sorts the batch items and then groups batch drawing into maximal allowed batch sets that do not
		/// overflow the 16 bit array indices for vertices.
		/// </summary>
		/// <param name="sortMode">The type of depth sorting desired for the rendering.</param>
		/// <param name="effect">The custom effect to apply to the drawn geometry</param>
		public unsafe void DrawBatch()
		{
			// nothing to do
			if (_batchItemList.Count == 0)
				return;


			// Determine how many iterations through the drawing code we need to make
			// Iterate through the batches, doing short.MaxValue sets of vertices only.
			foreach (var layer in _batchItemList)
			{
				foreach (var _effect in layer.Value)
				{
					bool effectInitialized = false;
					foreach (var _list in _effect.Value)
					{
						var list = _list.Value;
						var material = list.material;
						bool textureInitialized = false;
						// setup the vertexArray array

						int numBatchesToProcess = list.sprites.Count;
						if (numBatchesToProcess > MaxBatchSize)
						{
							numBatchesToProcess = MaxBatchSize;
						}
						// Avoid the array checking overhead by using pointer indexing!
						fixed (VertexPositionColorTexture* vertexArrayFixedPtr = _vertexArray)
						{
							var vertexArrayPtr = vertexArrayFixedPtr;

							// Draw the batches
							for (int i = 0; i < numBatchesToProcess; i++, vertexArrayPtr += 4)
							{
								SpriteBatchItem item = list.sprites[i];
								// if the texture changed, we need to flush and bind the new texture

								// store the SpriteBatchItem data in our vertexArray
								*(vertexArrayPtr + 0) = item.vertexTL;
								*(vertexArrayPtr + 1) = item.vertexTR;
								*(vertexArrayPtr + 2) = item.vertexBL;
								*(vertexArrayPtr + 3) = item.vertexBR;


							}
							list.sprites.Clear();
							if (!effectInitialized)
							{
								effectInitialized = true;
								var effect = material.effect;
								if (effect != null)
								{
									var pass = effect.CurrentTechnique.Passes[0];
									pass.Apply();
								}
							}
							if(!textureInitialized)
							{
								textureInitialized = true;
								_device.Textures[0] = material.texture;
								if (material.texture1 != null)
									_device.Textures[1] = material.texture1;
							}

							_device.DrawUserIndexedPrimitives(
									PrimitiveType.TriangleList,
									_vertexArray,
									0,
									numBatchesToProcess * 4,
									_index,
									0,
									(numBatchesToProcess) * 2,
									VertexPositionColorTexture.VertexDeclaration);


							list.material = null;
							list.sprites.Clear();
						}
					}

					_effect.Value.Clear();
				}
				
				layer.Value.Clear();
				// return items to the pool.  
			}
			_batchItemList.Clear();
		}

	}
}


