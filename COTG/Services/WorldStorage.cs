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
using Windows.Storage.Pickers;

namespace COTG.Services
{
    public static class WorldStorage
    {
        static string fileName => $"gwrd{(JSClient.world==20?"":JSClient.world.ToString())}.zip";
        public static StorageFolder folder => ApplicationData.Current.LocalFolder;

        //const int deltasPerBatch = 16;
        // Todo:  Delta encode
      
        static uint[][] historyBuffer;
		private static DateTimeOffset date;

		public static string ArchiveName(int entryId) => entryId.ToString("D6");
        public static async void SaveWorldData(uint[] data)
        {
			try
            {
				//App.DispatchOnUIThread( async () =>
				//{
				//	var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
				//	FileSavePicker savePicker = new FileSavePicker();
				//	savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
				//	// Dropdown of file types the user can save the file as
				//	savePicker.FileTypeChoices.Add("Zip", new List<string>() { ".zip" });
				//	// Default file name if the user does not type one in or select a file to replace
				//	savePicker.SuggestedFileName = fileName;
				//	StorageFile outFile = await savePicker.PickSaveFileAsync();
				//	await file.CopyAndReplaceAsync(outFile);
				//});

				//return;
				//{
					var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
			
				

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
							Trace(GetLastWriteUTC(prior).ToString("r"));
								var byteBuffer = new byte[data.Length * 4];
								using (var instream = prior.Open())
								{
									instream.Read(byteBuffer, 0, byteBuffer.Length);
								}
								var lastData = byteBuffer.ConvertToUints();
								var delta = HeatMap.ComputeDelta(lastData, data);
								historyBuffer = new uint[entries][];
								historyBuffer[priorEntry] = delta;
								var outCount = delta.Length;
								if (outCount < 24 || JSClient.subId != 0) // don't save file if playing a sub as there might be race conditions
								{
									deltaIsSmall = true;
								}
								else
								{
									prior.Delete(); // we want to replace it
									prior = zip.CreateEntry(priorName);
									

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
									prior.LastWriteTime = JSClient.ServerTime();
								}
								//then fall through and add the full entry

							}
							// write a whole record.
							if (!deltaIsSmall)
							{
								var entry = zip.CreateEntry(name);
								
								using (var outStream = entry.Open())
								{
									var byteData = data.ConvertToBytesWithoutDungeonsOrBosses();
									outStream.Write(byteData, 0, byteData.Length);
								}
								entry.LastWriteTime = JSClient.ServerTime();
							}

							
						}
					}
				try
				{
					using (var streamForZip = await file.OpenStreamForReadAsync())
					{

						using (var zip = new ZipArchive(streamForZip, mode: ZipArchiveMode.Read))
						{
							

							// report first time offset
							const int dayCount = 1;
							var count = zip.Entries.Count* dayCount;
							var snapshots = new DateTimeOffset[count];
							var lengths = new int[count];
							int counter = 0;
							for (int i = dayCount; --i >=0;)
							{
								foreach (var entry in zip.Entries)
								{
									var t = GetLastWriteUTC(entry).ToServerTime();
									snapshots[counter] = t;// $"{entry.LastWriteTime.ToUniversalTime().ToString("u")}: {(counter < count - 1 ? $"{lengths[counter] / 4} changes" : " current")} ";
									lengths[counter] = (int)entry.Length;
									++counter;
								}
							}
							App.DispatchOnUIThread(() => HeatTab.instance.SetItems(snapshots));
						}
					}
				}
				catch (Exception ex)
				{
					Log(ex);
				}
				//}
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

        public static DateTimeOffset GetLastWriteUTC( ZipArchiveEntry e)
		{
			return new DateTimeOffset(e.LastWriteTime.Ticks, TimeSpan.Zero).FromServerTime();
		}
        
        public static async void SetHeatmapDates( DateTimeOffset t0, DateTimeOffset t1)
        {
            if (World.changeMapInProgress)
                return;
            
		//	World.ClearHeatmap();
			
			

			World.changeMapInProgress = true;
            if (historyBuffer == null)
                return;

			var data = World.raw.ToArray(); // clone it
		    var data1 = World.raw;
			var changeMask = new bool[World.worldDim * World.worldDim];
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
			
			using (var streamForZip = await file.OpenStreamForReadAsync())
            {
                using (var zip = new ZipArchive(streamForZip, mode: ZipArchiveMode.Read))
                {
                    int entry = historyBuffer.Length;
                    while(--entry >= 0 )
                    {
				

                        var dName = ArchiveName(entry);
                        var prior = zip.GetEntry(dName);
                        if (GetLastWriteUTC(prior)  == t1 )
							data1 = data.ToArray();
						if (GetLastWriteUTC(prior) < t0)
							break;;

						//Log($"Delta {dName} {prior.Length} {prior.LastWriteTime}");
						if ( historyBuffer[entry] == null )
                        {
                            var byteBuffer = new byte[prior.Length];
                            using (var instream = prior.Open())
                            {
                                instream.Read(byteBuffer, 0, byteBuffer.Length);
                            }
                            historyBuffer[entry] = byteBuffer.ConvertToUints();

                        }
						var delta = historyBuffer[entry];

						HeatMap.ApplyDelta(data, delta);
						if(GetLastWriteUTC(prior) <= t1 )
						{
							int count = delta.Length / 2;
							for (int i = 0; i < count; ++i)
							{
								var off = delta[i * 2];
								changeMask[off] = true;

							}

						}
                    }
					// "erase" changes not in the valid range
					for(int i=0;i<World.worldDim*World.worldDim;++i)
					{
						if(changeMask[i] == false)
						{
							data[i] = data1[i];
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

        public static byte[] ConvertToBytesWithoutDungeonsOrBosses( this uint [] data )
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
