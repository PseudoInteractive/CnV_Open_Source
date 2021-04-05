﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO.Compression;
using static COTG.Debug;
using COTG.Game;
using COTG.Views;

namespace COTG.Services
{
    public static class WorldStorage
    {
        static string fileName => $"gwrd{(JSClient.world==20?"":JSClient.world.ToString())}.zip";
        public static StorageFolder folder => ApplicationData.Current.LocalFolder;

        const int deltasPerBatch = 16;
        // Todo:  Delta encode
        public static uint[] ComputeDelta(uint[] d0, uint[] d1)
        {
            var rv = new List<uint>();
            int count = d0.Length;
            Assert(d1.Length == count);
            for (int i = 0; i < count; ++i)
            {
                var c0 = d0[i];
                var c1 = d1[i];
                if (c0 == c1
                    || ((c0 & World.typeMask) == World.typeBoss)
                    || ((c0 & World.typeMask) == World.typeDungeon)
                    || ((c1 & World.typeMask) == World.typeBoss)
                    || ((c1 & World.typeMask) == World.typeDungeon))
                {
                    continue;
                }
                rv.Add((uint)i);
                rv.Add(c0 ^ c1);

            }
            return rv.ToArray();
            /*
            var outCount = rv.Count;
            if (outCount <= 64)
                return null;
            var bytes = new byte[outCount*4];
            for (int i = 0; i < outCount; ++i)
            {
                CopyBytes(rv[i], bytes, i);
            }
            return bytes;*/

        }
        public static void ApplyDelta(uint[] d, uint[] delta)
        {
            int count = delta.Length/2;
            for (int i = 0; i < count; ++i)
            {
                var off = delta[i*2];
                var x = delta[i*2+1];
                d[off] ^= x;

            }
        }

        static uint[][] historyBuffer;
		private static DateTimeOffset date;

		public static string ArchiveName(int entryId) => entryId.ToString("D6");
        public static async void SaveWorldData(uint[] data)
        {
			try
            { 
                var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
				int[] lengths;
				using (var streamForZip = await file.OpenStreamForReadAsync())
				{
				
					using (var zip = new ZipArchive(streamForZip, mode: ZipArchiveMode.Read))
					{
						lengths = new int[zip.Entries.Count];
						var counter = 0;
						foreach (var entry in zip.Entries)
						{
							lengths[counter] = (int)entry.Length;
							++counter;
						}
					}
				}

				using (var streamForZip = await file.OpenStreamForWriteAsync())
                {


					using (var zip = new ZipArchive(streamForZip, mode: ZipArchiveMode.Update))
					{

						int entries = zip.Entries.Count;
						var name = ArchiveName(entries);
						var deltaIsSmall = false;
						if (entries != 0)
						{
							// convert the last entry to a delta record
							var priorEntry = entries - 1;
							var priorName = ArchiveName(entries - 1);
							var prior = zip.GetEntry(priorName);
							var byteBuffer = new byte[data.Length * 4];
							using (var instream = prior.Open())
							{
								instream.Read(byteBuffer, 0, byteBuffer.Length);
							}
							var lastData = byteBuffer.ConvertToUints();
							var delta = ComputeDelta(lastData, data);
							historyBuffer = new uint[entries][];
							historyBuffer[priorEntry] = delta;
							var outCount = delta.Length;
							if (outCount < 32 || JSClient.subId != 0) // don't save file if playing a sub as there might be race conditions
							{
								deltaIsSmall = true;
							}
							else
							{
								prior.Delete(); // we want to replace it
								prior = zip.CreateEntry(priorName);
								prior.LastWriteTime = JSClient.ServerTime();

								using (var outStream = prior.Open())
								{
									var bytes = new byte[outCount * 4];
									for (int i = 0; i < outCount; ++i)
									{
										CopyBytes(delta[i], bytes, i);
									}

									// overwrite with delta values
									outStream.Write(bytes, 0, bytes.Length);
								}
							}
							//then fall through and add the full entry

						}
						// write a whole record.
						if (!deltaIsSmall)
						{
							var entry = zip.CreateEntry(name);
							entry.LastWriteTime = JSClient.ServerTime();
							using (var outStream = entry.Open())
							{
								var byteData = data.ConvertToBytesWithoutDungeons();
								outStream.Write(byteData, 0, byteData.Length);
							}
						}

						// report first time offset
						var count = zip.Entries.Count;
						var snapshots = new string[count];
						int counter = 0;
						foreach (var entry in zip.Entries)
						{
							snapshots[counter] = $"{entry.LastWriteTime.ToUniversalTime().ToString("G")}: {(counter < count - 1 ? $"{lengths[counter] / 4} changes" : " current")} ";
							++counter;
						}
						App.DispatchOnUIThread( ()=> HeatmapDatePicker.SetItems(snapshots) );
					}
					
				}
            }
			//catch(InvalidDataException invalidData)
			//{
			//	var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting) ;
			//	await file.DeleteAsync();


			//	Log(invalidData);

			//}
			catch (Exception e)
			{
				Log(e);
			}

        }

        
        
        public static async void SetHeatmapDates( int[] selectedItems)
        {
            if (World.changeMapInProgress)
                return;
            
			World.ClearHeatmap();
			
			Assert(selectedItems.Length > 0);

			World.changeMapInProgress = true;
            if (historyBuffer == null)
                return;

			var selMin = selectedItems.Min();
			var selMax = selectedItems.Max();
			var data = World.raw.ToArray(); // clone it
		    var data1 = World.raw;
			var changeMask = new bool[World.worldDim * World.worldDim];
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
			
			using (var streamForZip = await file.OpenStreamForReadAsync())
            {
                using (var zip = new ZipArchive(streamForZip, mode: ZipArchiveMode.Read))
                {
                    int entry = historyBuffer.Length;
                    while(--entry >= selMin )
                    {
						if (entry == selMax)
							data1 = data.ToArray();

                        var dName = ArchiveName(entry);
                        var prior = zip.GetEntry(dName);
                        if ( prior.LastWriteTime < date )
                            break;

                        //Log($"Delta {dName} {prior.Length} {prior.LastWriteTime}");
                        if( historyBuffer[entry] == null )
                        {
                            var byteBuffer = new byte[prior.Length];
                            using (var instream = prior.Open())
                            {
                                instream.Read(byteBuffer, 0, byteBuffer.Length);
                            }
                            historyBuffer[entry] = byteBuffer.ConvertToUints();

                        }
						var delta = historyBuffer[entry];

						ApplyDelta(data, delta);
						if(entry >= selMin && entry <= selMax )
						{
							int count = delta.Length / 2;
							for (int i = 0; i < count; ++i)
							{
								var off = delta[i * 2];
								changeMask[off] = true;

							}

						}
                    }
					for(int i=0;i<World.worldDim*World.worldDim;++i)
					{
						if(changeMask[i] == false)
						{
							data[i] = World.raw[i];
						}
					}
                    World.CreateChangePixels(data,data1);
                }
            }
        }

        public static void CopyBytes(uint src, byte[]  rv, int i  )
        {
            rv[i * 4 + 0] = (byte)(src & 0xff);
            rv[i * 4 + 1] = (byte)((src >> 8) & 0xff);
            rv[i * 4 + 2] = (byte)((src >> 16) & 0xff);
            rv[i * 4 + 3] = (byte)((src >> 24) & 0xff);
        }

        public static uint CopyBytes(byte[] src, int i)
        {
            return ((uint)src[i * 4 + 0] << 0)
                | ((uint)src[i * 4 + 1] << 8)
                | ((uint)src[i * 4 + 2] << 16)
                | ((uint)src[i * 4 + 3] << 24);

        }

        public static byte[] ConvertToBytesWithoutDungeons( this uint [] data )
        {
            int size4 = data.Length;
            var rv = new byte[size4 * 4];
            for (var i = 0; i < size4; ++i)
            {
                var src = data[i];
                switch (src & World.typeMask)
                {
                    // removing dungeons reduces storage sigificantly
                    case 0:
                    case World.typeDungeon:
                    case World.typeBoss:
                        {
                            rv[i * 4 + 0] =
                            rv[i * 4 + 1] =
                            rv[i * 4 + 2] =
                            rv[i * 4 + 3] = 0;
                            break;
                        }
                    default:
                        {
                            CopyBytes(src, rv, i);
                            break;
                        }
                }
            }
            // Todo: use array pool
            return rv;
        }
        public static uint[] ConvertToUints(this byte[] src)
        {
            int size =src.Length;
            Assert( (size%4) == 0);
            var size4 = size >> 2;
            var rv = new uint[size4 ];
            for (var i = 0; i < size4; ++i)
            {
                rv[i] = CopyBytes(src, i);
            }
            // Todo: use array pool
            return rv;
        }
    }

}
