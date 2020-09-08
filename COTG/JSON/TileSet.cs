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
    
        public sealed class TileData
        {
            public enum State
            {
                preInit,
                loadedData,
                loadingImages,
                loadedImages,
                ready = loadedImages
            }
            public static State state = State.preInit;

            public static TileData instance;
        public static async void Ctor()
        {
            state = State.preInit; // reset if necessary
            var prior = instance?.tilesets;  // if called previously save the images to reuse
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
                for(int i=0;i<count;++i)
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
                }
            }

            
                for (var i = 0; i < World.worldDim * World.worldDim; ++i)
                {
                    foreach (var layer in instance.layers)
                    {
                        var tile = layer.data[i];
                    if (tile == 0)
                        continue;
                        for (int tileId = 0; tileId < tileCount; ++tileId)
                        {
                        var ts = instance.tilesets[tileId];
                        var off = tile - ts.firstgid;
                        if ((off >= 0) && off < ts.tilecount)
                            {
                            layer.data[i] = (ushort)(off | (tileId << 13));// << (put++ * 16);
                            break;
                        }
                        }
                    }
            
                }
            await Cosmos.GetSpotDB();
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
        }

        public sealed class Layer
        {
            public ushort[] data { get; set; }
            public int height { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public int width { get; set; }
        }

        public sealed class Tileset
        {
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

                    var uri = new Uri($"ms-appx:///Assets/{ resName.Substring(0,resName.Length-3)}dds");
                    var temp = this;
                Debug.Log(uri.ToString());
                    temp.bitmap = await CanvasBitmap.LoadAsync(ShellPage.canvas.Device,  uri);
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


      


