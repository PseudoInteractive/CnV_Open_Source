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

    using Microsoft.Graphics.Canvas;
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
		public const float zBase = 0;
		public const float zLand = 0;
		public const float zWater = 10;
		public const float zTerrain = 20;
		public const float zTopLevel = 36;
		public const float zCities = 32;
		public const float zLabels = 40;

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
		

		public static async Task Ctor(bool deviceLost)
        {
			// if we are still loading the last data, wait
			while(state >= State.loadStart && state != State.ready)
			{
				await Task.Delay(1000);
			}
			if(state == State.ready)
			{
				if (deviceLost)
					state = State.lostDevice;
				else
					state = State.wantRefresh;
			}
            var prior = (state == State.wantRefresh) ? instance?.tilesets : null;  // if called previously save the images to reuse
			Assert(state == State.preInit || state == State.lostDevice || state == State.wantRefresh); // reset if necessary

			state = State.loadStart; 
			instance = await TileMapFetch.Get();
            //            Note.Show("TilesFetched");
            state = State.loadedData;

            //);
            Assert(state == State.loadedData);
            // remove numbered things

            state = State.loadingImages;
            var tileCount = instance.tilesets.Length - 1;
            // remove names layer
            instance.tilesets = instance.tilesets.Take(tileCount).ToArray();
            //  await canvas.RunOnGameLoopThreadAsync( async () =>
            if (prior != null)
            {
                var count = prior.Length;
                for (int i = 0; i<count; ++i)
                {
                    instance.tilesets[i].bitmap = prior[i].bitmap;
                }
                prior = null;

            }
            else
            {
                foreach (var tileSet in instance.tilesets)
                {
                    tileSet.Load();
					switch(tileSet.name)
					{
						case "land":
							tileSet.z = zLand;
							tileSet.wantShadow = false;
							tileSet.isBase = true;
							break;
						case "water":
							tileSet.z = zWater;
							tileSet.isBase = true;
							break;
						case "terrainfeatures":
							tileSet.z = zTerrain;
						//	tileSet.wantShadow = true;
							break;
						case "city":
							tileSet.z = zCities;
							tileSet.wantShadow = true;
							break;
						case "toplevel":
							tileSet.z = zTopLevel;
							tileSet.wantShadow = true;
							break;

					}
				}
            }
			foreach (var layer in instance.layers)
			{
				switch(layer.name)
				{
					case "land":
						{
							layer.isBase = true;
							break;
						}
					case "water":
						{
							layer.isBase = true;
							break;
						}
					case "cities":
						{
							layer.wantShadow = true;
							break;
						}
					case "numbers":
						{
							layer.wantShadow = true;
							break;
						}
					case "labels":
						{
							layer.wantShadow = false;
							break;
						}
				}
			}

				for (var i = 0; i < World.worldDim * World.worldDim; ++i)
            {
                foreach (var layer in instance.layers)
                {
				
                    var tile = layer.data[i];
                    if (tile == 0)
                        continue;
                    //       layer.data[i] =0; // 0 by default
                    for (int tileId = 0; tileId < tileCount; ++tileId)
                    {
                        var ts = instance.tilesets[tileId];
                        var off = tile - ts.firstgid;
                        Assert(ts.tilewidth==64);
                        var offX = off%ts.columns;
                        if ((off >= 0) && off < ts.tilecount)
                        {
                            layer.data[i] = (ushort)(off | (tileId << 13));// << (put++ * 16);
                            break;
                        }
                    }
                }

            }

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
                int off = l.width*y+x;
                var data = l.data[off];
                var tileSet = data>>13;
                // tileset 1 is land details
                if (tileSet == 1)
                {
                    int xy = data&((1 << 13) - 1);
                    int ty = xy/10;
                    int tx = xy - ty*10;
                    Assert(tx < 9);
                    // special mountains
                    if((tx==2 && ty==3)||(tx==8&&ty==1)||(tx==5&&ty==1)||(tx==4&&ty==6)||(tx==3&&ty==6)||
                        (tx>=2&&tx<=4&&ty==11)||(tx>=2&&tx<=3&&ty==14) )
                    {
                        break;
                    }
                   
                    if (ty < 7)
                    {
                        ironCount += diagonal ? 2 : 4;
                    }
                    else if (ty < 12 || (ty<15 && tx<4))
                    {
                        stoneCount += diagonal ? 2 : 4;
                    }
                    else if (  ty < 20)
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
            plainsCount+=diagonal ? 0.5f : 1f;
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
        public (SpotType type,int x,int y) GetSpotType(int x, int y)
        {
            var hasPlains = false;
            foreach (var l in layers)
            {
                int off = l.width*y+x;
                var data = l.data[off];
                if (data==0)
                    continue;
                var tileSet = data>>13;
                int xy = data&((1 << 13) - 1);
                (var tx, var ty) = xy.DecomposeXY(tilesets[tileSet].columns);
                // tileset 1 is land details
                if (tileSet == 0)
                {
               //     return (SpotType.water,tx,ty);
                }
                if(tileSet == 2)
                {
                    hasPlains=true;
                }
                if (tileSet == 1)
                {
                   
                    
                    Assert(tx < 9);
                    // special mountains
                    if ((tx==2 && ty==3)||(tx==8&&ty==1)||(tx==5&&ty==1)||(tx==4&&ty==6)||(tx==3&&ty==6)||
                        (tx>=2&&tx<=4&&ty==11)||(tx>=2&&tx<=3&&ty==14))
                    {
                        continue;
                    }
                    
                    if (ty < 7)
                    {
                        return (SpotType.mountain,tx,ty);
                    }
                    else if (ty < 12 || (ty<15 && tx<4))
                    {
                        return (SpotType.hill, tx, ty); ;
                    }
                    else if (  ty < 20)
                    {
                        return (SpotType.forest,tx,ty); ;
                    }
                    else if( tx==0)
                    {
                        return (SpotType.shrine,tx,ty); ;
                    }
                    else
                    {
                        return (SpotType.portal,tx,ty); ;
                    }

                }
                if(tileSet==3)
                {
                    return (SpotType.city,tx,ty);
                }
            }
            return hasPlains ? (SpotType.plain,0,0) : (SpotType.other,0,0);
        }
    }




    public sealed class Layer
    {
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
            return data[x+y*width];
        }
        public int FillCount(int x, int y) // 0 or 1
        {
            return data[x+y*width] == 0 ? 0 : 1;
        }
    }

    public sealed class Tileset
    {
		[JsonIgnore]
		public float z;
		[JsonIgnore]
		public bool wantShadow;
		[JsonIgnore]
		public bool isBase; // shadows draw onto this, this are drawn first 


		public CanvasBitmap bitmap;
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

                var uri = new Uri($"ms-appx:///Assets/{ resName.Substring(0, resName.Length-3)}dds");
                var temp = this;
                Debug.Log(uri.ToString());
                temp.bitmap = await CanvasBitmap.LoadAsync(ShellPage.canvas.Device, uri);
                // etc.
                Assert(temp.bitmap != null);
            }
            catch (Exception e)
            {
                Log(e);

            }




        }

    }
}





