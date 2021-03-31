// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
using Vector2 = System.Numerics.Vector2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna;

namespace COTG.Draw
{
	public class Mesh
	{
		public int vertexCount;
		public int triangleCount;
		public VertexBuffer vb;
		public IndexBuffer ib;
	};
	public class SpriteBatchItemList
	{
		public Material material;
		public List<SpriteVertices> sprites;
		public List<Mesh> meshes;

		public SpriteBatchItemList(Material _material)
		{
			material = _material;
			sprites = new List<SpriteVertices>();
			meshes = new List<Mesh>();
		}
		public void Release()
		{
			freePool.AddRange(sprites);
			sprites.Clear();
			meshes.Clear();
			material = null;
		}
		public static List<SpriteVertices> freePool = new List<SpriteVertices>();
		public static SpriteVertices Alloc()
		{
			int size = freePool.Count;
			if (size > 0)
			{
				var rv = freePool[size - 1];
				freePool.RemoveAt(size - 1);
				return rv;
			}
			return new SpriteVertices();
		}

	}

	public delegate float DepthFunction(float x, float y, float zBase);
	public static class SpriteHelper
	{
		public static void Set(this SpriteVertices me, float x, float y, float dx, float dy, float w, float h, float sin, float cos, Color color, Vector2 texCoordTL, Vector2 texCoordBR, float depth)
		{
			me.vertexTL.Position.X = x + dx * cos - dy * sin;
			me.vertexTL.Position.Y = y + dx * sin + dy * cos;
			me.vertexTL.Position.Z = depth;
			me.vertexTL.Color = color;
			me.vertexTL.TextureCoordinate.X = texCoordTL.X;
			me.vertexTL.TextureCoordinate.Y = texCoordTL.Y;

			me.vertexTR.Position.X = x + (dx + w) * cos - dy * sin;
			me.vertexTR.Position.Y = y + (dx + w) * sin + dy * cos;
			me.vertexTR.Position.Z = depth;
			me.vertexTR.Color = color;
			me.vertexTR.TextureCoordinate.X = texCoordBR.X;
			me.vertexTR.TextureCoordinate.Y = texCoordTL.Y;

			me.vertexBL.Position.X = x + dx * cos - (dy + h) * sin;
			me.vertexBL.Position.Y = y + dx * sin + (dy + h) * cos;
			me.vertexBL.Position.Z = depth;
			me.vertexBL.Color = color;
			me.vertexBL.TextureCoordinate.X = texCoordTL.X;
			me.vertexBL.TextureCoordinate.Y = texCoordBR.Y;

			me.vertexBR.Position.X = x + (dx + w) * cos - (dy + h) * sin;
			me.vertexBR.Position.Y = y + (dx + w) * sin + (dy + h) * cos;
			me.vertexBR.Position.Z = depth;
			me.vertexBR.Color = color;
			me.vertexBR.TextureCoordinate.X = texCoordBR.X;
			me.vertexBR.TextureCoordinate.Y = texCoordBR.Y;
		}

		public static void Set(this SpriteVertices me, float x, float y, float w, float h, Color color, Vector2 texCoordTL, Vector2 texCoordBR, float depth)
		{
			me.vertexTL.Position.X = x;
			me.vertexTL.Position.Y = y;
			me.vertexTL.Position.Z = depth;
			me.vertexTL.Color = color;
			me.vertexTL.TextureCoordinate.X = texCoordTL.X;
			me.vertexTL.TextureCoordinate.Y = texCoordTL.Y;

			me.vertexTR.Position.X = x + w;
			me.vertexTR.Position.Y = y;
			me.vertexTR.Position.Z = depth;
			me.vertexTR.Color = color;
			me.vertexTR.TextureCoordinate.X = texCoordBR.X;
			me.vertexTR.TextureCoordinate.Y = texCoordTL.Y;

			me.vertexBL.Position.X = x;
			me.vertexBL.Position.Y = y + h;
			me.vertexBL.Position.Z = depth;
			me.vertexBL.Color = color;
			me.vertexBL.TextureCoordinate.X = texCoordTL.X;
			me.vertexBL.TextureCoordinate.Y = texCoordBR.Y;

			me.vertexBR.Position.X = x + w;
			me.vertexBR.Position.Y = y + h;
			me.vertexBR.Position.Z = depth;
			me.vertexBR.Color = color;
			me.vertexBR.TextureCoordinate.X = texCoordBR.X;
			me.vertexBR.TextureCoordinate.Y = texCoordBR.Y;
		}
		public static void Set(this SpriteVertices me, float x, float y, float w, float h, Color color, Vector2 texCoordTL, Vector2 texCoordBR, float depthBase, DepthFunction depth)
		{
			me.vertexTL.Position.X = x;
			me.vertexTL.Position.Y = y;
			me.vertexTL.Position.Z = depth(x, y, depthBase);
			me.vertexTL.Color = color;
			me.vertexTL.TextureCoordinate.X = texCoordTL.X;
			me.vertexTL.TextureCoordinate.Y = texCoordTL.Y;

			me.vertexTR.Position.X = x + w;
			me.vertexTR.Position.Y = y;
			me.vertexTR.Position.Z = depth(x + w, y, depthBase);
			me.vertexTR.Color = color;
			me.vertexTR.TextureCoordinate.X = texCoordBR.X;
			me.vertexTR.TextureCoordinate.Y = texCoordTL.Y;

			me.vertexBL.Position.X = x;
			me.vertexBL.Position.Y = y + h;
			me.vertexBL.Position.Z = depth(x, y + h, depthBase);
			me.vertexBL.Color = color;
			me.vertexBL.TextureCoordinate.X = texCoordTL.X;
			me.vertexBL.TextureCoordinate.Y = texCoordBR.Y;

			me.vertexBR.Position.X = x + w;
			me.vertexBR.Position.Y = y + h;
			me.vertexBR.Position.Z = depth(x + w, y + h, depthBase);
			me.vertexBR.Color = color;
			me.vertexBR.TextureCoordinate.X = texCoordBR.X;
			me.vertexBR.TextureCoordinate.Y = texCoordBR.Y;
		}

		public static void Set(this SpriteVertices me, float x, float y, float w, float h, Color color, Vector2 texCoordTL, Vector2 texCoordBR, float depthTL, float depthTR, float depthBL, float depthBR)
		{
			me.vertexTL.Position.X = x;
			me.vertexTL.Position.Y = y;
			me.vertexTL.Position.Z = depthTL;
			me.vertexTL.Color = color;
			me.vertexTL.TextureCoordinate.X = texCoordTL.X;
			me.vertexTL.TextureCoordinate.Y = texCoordTL.Y;

			me.vertexTR.Position.X = x + w;
			me.vertexTR.Position.Y = y;
			me.vertexTR.Position.Z = depthTR;
			me.vertexTR.Color = color;
			me.vertexTR.TextureCoordinate.X = texCoordBR.X;
			me.vertexTR.TextureCoordinate.Y = texCoordTL.Y;

			me.vertexBL.Position.X = x;
			me.vertexBL.Position.Y = y + h;
			me.vertexBL.Position.Z = depthBL;
			me.vertexBL.Color = color;
			me.vertexBL.TextureCoordinate.X = texCoordTL.X;
			me.vertexBL.TextureCoordinate.Y = texCoordBR.Y;

			me.vertexBR.Position.X = x + w;
			me.vertexBR.Position.Y = y + h;
			me.vertexBR.Position.Z = depthBR;
			me.vertexBR.Color = color;
			me.vertexBR.TextureCoordinate.X = texCoordBR.X;
			me.vertexBR.TextureCoordinate.Y = texCoordBR.Y;
		}
	}


}

