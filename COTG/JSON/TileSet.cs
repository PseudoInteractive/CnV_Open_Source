﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static COTG.Debug;

namespace COTG.JSON
{
	using COTG.Game;
	using COTG.Services;
	using COTG.Views;


	using Microsoft.Xna.Framework.Graphics;
	// <auto-generated />
	//
	// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
	//
	//    using C;
	//
	//    var root = Root.FromJson(jsonString);

	using System;
	using System.Text.Json.Serialization;

	public sealed class TileData
	{
		// inclusive
		public static bool IsSpecialTile(int tile) =>
		
			 tile switch { (>= tileShrineStart and <= tileShrineEnd)
				 or tileShrineUnlit
				 or tilePortalClosed
				 or tilePortalOpen 
				 or (>= tileModifierStart and <= tileModifierEnd) => true, _ => false };

		
		public const int tileShrineStart = 1557;
		public const int tileShrineEnd = 1564;
		public const int tileShrineUnlit = 1001;
		public const int tilePortalClosed = 1002;
		public const int tilePortalOpen = 1003;
		public const int tileModifierStart = 1677;
		public const int tileModifierEnd = tileModifierStart+7;
		public enum State
		{
			preInit,
			lostDevice,
			wantRefresh,
			loadStart,
			loadedData,
			loadingImages,
			loadedImages,
			ready = loadedImages
		}
		public static State state = State.preInit;

		public static TileData instance;

		public static void UpdateTile(string s) 
		{
			
			var ss = s.Split('-', StringSplitOptions.RemoveEmptyEntries);
			if( ss.Length <= 1 )
			{
				Assert(false);
				return;
			}
			if (!ss[1].TryParseInt(out var xy))
				return;
			if (!ss[0].TryParseInt(out var tile))
				return;
			if(ss.Length == 3)
			{
				// ignore dungeons and bosses
				if (ss[2].Length == 2 && (ss[2] == "1" || ss[2] == "2"))
					return;
			}
			var x = (xy)/ 1000;
			var y = (xy - x*1000)-100;
			x -= 100;
			int off = x + y * World.span;
			//	var l = 2;
			if (tile == 999)
				return;
			//for(;l< instance.layers.Length;++l)
			//{ 
			//	var data = instance.layers[l].data[off];
			//	if (data == 0)
			//		break;
			//}

			for (int tileId = 0; tileId < instance.tilesets.Length; ++tileId)
			{
				var ts = instance.tilesets[tileId];
				var tileOff = tile - ts.firstgid;
				//    Assert(ts.tilewidth==64);
				var offX = tileOff % ts.columns;
				if ((tileOff >= 0) && tileOff < ts.tilecount)
				{
					var data = (ushort)(tileOff | (tileId << 13));
					// is it already here?
					bool present = instance.layers.Any(l => l.data[off] == data);
					if(!present)
					{
						if (IsSpecialTile(tile))
							Layer.bonus.data[off] = data;// << (put++ * 16);
						else
							instance.layers[instance.layers.Length - 2].data[off] = data;

					}
					break;
				}
			}


		}

		public static async Task Ctor(bool deviceLost)
		{
			// if we are still loading the last data, wait
			while (state >= State.loadStart && state != State.ready)
			{
				await Task.Delay(1000);
			}
			if (state == State.ready)
			{
				if (deviceLost)
					state = State.lostDevice;
				else
					state = State.wantRefresh;
			}
			var prior = (state == State.wantRefresh) ? instance?.tilesets : null;  // if called previously save the images to reuse
			Assert(state == State.preInit || state == State.lostDevice || state == State.wantRefresh); // reset if necessary

			state = State.loadStart;
			for (; ; )
			{
				instance = await TileMapFetch.Get();
				//            Note.Show("TilesFetched");
				if (instance != null)
					break;
				await Task.Delay(2000);
			}
			state = State.loadedData;

			//);
			Assert(state == State.loadedData);
			// remove numbered things

			state = State.loadingImages;
			var tileCount = instance.tilesets.Length - 1;
			// remove names layer
			instance.tilesets = instance.tilesets.Take(tileCount).ToArray();
			if (Layer.bonus == null)
			{
				Layer.bonus = new() { wantShadow = true, id = Layer.idBonus, isBase = false, data = new ushort[World.spanSquared], width = World.span, height = World.span, name = "Bonus" };
			}
			instance.layers = instance.layers.ArrayAppend(Layer.bonus);
			//  await canvas.RunOnGameLoopThreadAsync( async () =>
			if (prior != null)
			{
	//			Assert(false);
			}
			//	var count = prior.Length;
			//	for (int i = 0; i < count; ++i)
			//	{
			//		instance.tilesets[i].material = prior[i].material;
			//		instance.tilesets[i].shadowMaterial = prior[i].shadowMaterial;
			//	}
			//	prior = null;

			//}
			//else
			{
				foreach (var tileSet in instance.tilesets)
				{
					tileSet.Load();
					
				}
			}
			foreach (var tileSet in instance.tilesets)
			{
				switch (tileSet.name)
				{
					case "land":
						tileSet.z = AGame.zLand;
						tileSet.wantShadow = false;
						tileSet.isBase = true;
						
						break;
					case "water":
						tileSet.z = AGame.zWaterBase;
						tileSet.isBase = true;
						break;
					case "terrainfeatures":
						tileSet.z = AGame.zTerrainBase;
						tileSet.wantShadow = true;
						break;
					case "city":
						tileSet.z = AGame.zCitiesBase;
						tileSet.wantShadow = true;
						tileSet.canHover = false;
						break;
					case "toplevel":
						tileSet.z = AGame.zTopLevelBase;
						tileSet.wantShadow = true;
						tileSet.canHover = true;
						break;

				}
			}
			foreach (var layer in instance.layers)
			{
				switch (layer.name)
				{
					case "land":
						{
							layer.isBase = true;
							layer.id = 1;
							break;
						}
					case "water":
						{
							layer.isBase = true;
							layer.id = 2;
							break;
						}
					case "cities":
						{
							layer.wantShadow = true;
							layer.id = 3;
							break;
						}
					case "numbers":
						{
							layer.wantShadow = true;
							layer.id = 4;
							break;
						}
					case "labels":
						{
							layer.wantShadow = false;
							layer.id = 5;
							break;
						}
				}
			}

			for (var i = 0; i < World.span * World.span; ++i)
			{
				foreach (var layer in instance.layers)
				{
					if ( object.ReferenceEquals(layer,Layer.bonus) )
						continue;
					var tile = layer.data[i];
					if (tile == 0)
						continue;
					layer.data[i] = 0; // 0 by default
					for (int tileId = 0; tileId < tileCount; ++tileId)
					{
						var ts = instance.tilesets[tileId];
						var tileOff = tile - ts.firstgid;
						//    Assert(ts.tilewidth==64);
						var offX = tileOff % ts.columns;
						if ((tileOff >= 0) && tileOff < ts.tilecount)
						{
							var data = (ushort)(tileOff | (tileId << 13));// << (put++ * 16);
							if (IsSpecialTile(tile))
								Layer.bonus.data[i] = data;
							else
								layer.data[i] = data;
							break;
						}
					}
				}

			}
			state = State.ready;

		}



		public int compressionlevel { get; set; }
		public static int Height() => instance.height;
		public int height { get; set; }
		public bool infinite { get; set; }
		public Layer[] layers { get; set; }
		public int nextlayerid { get; set; }
		public int nextobjectid { get; set; }
		public string orientation { get; set; }
		public string renderorder { get; set; }
		public string tiledversion { get; set; }
		public int tileheight { get; set; }
		public Tileset[] tilesets { get; set; }
		public int tilewidth { get; set; }
		public string type { get; set; }
		public float version { get; set; }
		public static int Width() => instance.width;
		public int width { get; set; }

		public void ResourceGain(int x, int y, bool diagonal, ref float woodCount, ref float stoneCount, ref float ironCount, ref float plainsCount)
		{
			foreach (var l in layers)
			{
				int off = l.width * y + x;
				var data = l.data[off];
				var tileSet = data >> 13;
				// tileset 1 is land details
				if (tileSet == 1)
				{
					int xy = data & ((1 << 13) - 1);
					int ty = xy / 10;
					int tx = xy - ty * 10;
					Assert(tx < 9);
					// special mountains
					if ((tx == 2 && ty == 3) || (tx == 8 && ty == 1) || (tx == 5 && ty == 1) || (tx == 4 && ty == 6) || (tx == 3 && ty == 6) ||
						(tx >= 2 && tx <= 4 && ty == 11) || (tx >= 2 && tx <= 3 && ty == 14))
					{
						break;
					}

					if (ty < 7)
					{
						ironCount += diagonal ? 2 : 4;
					}
					else if (ty < 12 || (ty < 15 && tx < 4))
					{
						stoneCount += diagonal ? 2 : 4;
					}
					else if (ty < 20)
					{
						woodCount += diagonal ? 2 : 4;
					}
					else
					{
						break; // fall through to plains
					}

					return;
				}
			}
			plainsCount += diagonal ? 0.5f : 1f;
		}
		public enum SpotType
		{
			plain,
			forest,
			hill,
			mountain,
			water,
			portal,
			city,
			shrine,
			other,
		}
		public (SpotType type, int x, int y) GetSpotType(int x, int y)
		{
			var hasPlains = false;
			foreach (var l in layers)
			{
				int off = l.width * y + x;
				var data = l.data[off];
				if (data == 0)
					continue;
				var tileSet = data >> 13;
				int xy = data & ((1 << 13) - 1);
				(var tx, var ty) = xy.DecomposeXY(tilesets[tileSet].columns);
				// tileset 1 is land details
				if (tileSet == 0)
				{
					//     return (SpotType.water,tx,ty);
				}
				if (tileSet == 2)
				{
					hasPlains = true;
				}
				if (tileSet == 1)
				{


					Assert(tx < 9);
					// special mountains
					if ((tx == 2 && ty == 3) || (tx == 8 && ty == 1) || (tx == 5 && ty == 1) || (tx == 4 && ty == 6) || (tx == 3 && ty == 6) ||
						(tx >= 2 && tx <= 4 && ty == 11) || (tx >= 2 && tx <= 3 && ty == 14))
					{
						continue;
					}

					if (ty < 7)
					{
						return (SpotType.mountain, tx, ty);
					}
					else if (ty < 12 || (ty < 15 && tx < 4))
					{
						return (SpotType.hill, tx, ty); ;
					}
					else if (ty < 20)
					{
						return (SpotType.forest, tx, ty); ;
					}
					else if (tx == 0)
					{
						return (SpotType.shrine, tx, ty); ;
					}
					else
					{
						return (SpotType.portal, tx, ty); ;
					}

				}
				if (tileSet == 3)
				{
					return (SpotType.city, tx, ty);
				}
			}
			return hasPlains ? (SpotType.plain, 0, 0) : (SpotType.other, 0, 0);
		}
	}




	public sealed class Layer
	{
		public static Layer bonus;
		public const int idBonus = 7; // this layer is above all
		[JsonIgnore]
		public float z;
		[JsonIgnore]
		public bool wantShadow;
		[JsonIgnore]
		public bool isBase; // shadows draw onto this, this are drawn first

		public ushort[] data { get; set; }
		public int height { get; set; }
		public int id { get; set; }
		public string name { get; set; }
		public int width { get; set; }
		public ushort GetData(int x, int y)
		{
			return data[x + y * width];
		}
		public int FillCount(int x, int y) // 0 or 1
		{
			return data[x + y * width] == 0 ? 0 : 1;
		}
	}

	public sealed class Tileset
	{
		[JsonIgnore]
		public float z;
		[JsonIgnore]
		public bool wantShadow;
		[JsonIgnore]
		public bool canHover;
		[JsonIgnore]
		public bool isBase; // shadows draw onto this, this are drawn first 
		[JsonIgnore]
		public float scaleXToU;
		[JsonIgnore]
		public float scaleYToV;
		[JsonIgnore]
		public float halfTexelU;
		[JsonIgnore]
		public float halfTexelV;



		[JsonIgnore]
		public Draw.Material material;
		[JsonIgnore]
		public Draw.Material shadowMaterial;
		//public (int u,int v) ScaleUV( (int u, int v) uv)
		//{
		//	return ((int)(uv.u * bitmap.Width + imagewidth / 2) / imagewidth, (int)(uv.v * bitmap.Height + imageheight / 2) / imageheight);

		//}
		public int columns { get; set; }
		public int firstgid { get; set; }
		public string image { get; set; }
		public int imageheight { get; set; }
		public int imagewidth { get; set; }
		public string name { get; set; }
		public int tilecount { get; set; }
		public int tileheight { get; set; }
		public int tilewidth { get; set; }
		public async void Load()
		{
			try
			{
				var resName = image;

				//var uri = new Uri($"ms-appx:///Assets/{ resName.Substring(0, resName.Length-3)}dds");
				//var temp = this;
				//  Debug.Log(uri.ToString());
				string shortName = resName.Substring(0, resName.Length - 4);
				string dir = SettingsPage.IsThemeWinter() ? "winter" : "cotg";
				material= Helper.LoadLitMaterial($"Art/Tiles/{dir}/{shortName}");

				shadowMaterial = new Draw.Material(material.texture, AGame.unlitEffect);
				// etc.
				Assert(material != null);


				tilewidth = (int)material.texture2d.Width / columns;
				tileheight = tilewidth;
				scaleXToU = 1.0f / columns;
				scaleYToV = (float)tileheight / material.texture2d.Height;
				//effect = new PixelShaderEffect(ShellPage.lightEffectBytes)
				//{
				halfTexelU = 0.5f / material.texture2d.Width;
				halfTexelV = 0.5f / material.texture2d.Height;
				//	Source1 = temp.bitmap,
				//	Source1Interpolation = CanvasImageInterpolation.Linear,
				//	Source3 = temp.bitmap,
				//	Source3BorderMode=EffectBorderMode.Soft,
				//	Source3Interpolation=CanvasImageInterpolation.Linear,
				//	Source3Mapping=SamplerCoordinateMapping.Offset,

				//	Source2 = ShellPage.sky,
				//Source2Mapping = SamplerCoordinateMapping.Unknown,
				//Source2Interpolation = CanvasImageInterpolation.Linear,
				//Source2BorderMode = EffectBorderMode.Soft,
				//CacheOutput = false,
				//Source1BorderMode = EffectBorderMode.Soft,
				//Source1Mapping = SamplerCoordinateMapping.Offset,
				//MaxSamplerOffset = 4,
				//Name = "SSLighting"

				////    Source2 = await CanvasBitmap.LoadAsync(sender, "Shaders/SketchTexture.jpg"),
				////   Source2Mapping = SamplerCoordinateMapping.Unknown
				//};

			}
			catch (Exception e)
			{
				LogEx(e);

			}




		}

		
	}
}





