using System;
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
using Microsoft.Toolkit.HighPerformance.Buffers;

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
        public static async Task SaveWorldData(MemoryOwner<uint> data)
        {
			try
            {
				await HeatMap.AddSnapshot(SmallTime.serverNow,World.FilterForCompression(data.Span),true);
			}
			catch (Exception e)
			{
				LogEx(e);
			}
			//await UploadHistory();
		}

		public static DateTimeOffset GetLastWriteUTC( ZipArchiveEntry e)
		{
			return new DateTimeOffset(e.LastWriteTime.UtcTicks, TimeSpan.Zero).FromServerTime();
		}
		public static DateTimeOffset GetLastWriteServer(ZipArchiveEntry e)
		{
			return new DateTimeOffset(e.LastWriteTime.UtcTicks, TimeSpan.Zero);
		}

		//const string workStr = "Combining Heatmaps";
		//public static async Task UploadHistory()
		//{
		//	try
		//	{
		//		var file = await folder.TryGetItemAsync(fileName);

		//		if (file == null)
		//			return;

		//		ShellPage.WorkStart(workStr);
		//		   try
		//		   {
		//			   using (var streamForZip = await file.OpenStreamForReadAsync() )
		//			   {
		//				   using (var zip = new ZipArchive(streamForZip, mode: ZipArchiveMode.Read))
		//				   {
		//					   var count = zip.Entries.Count;
		//					   int entry = count;

		//					   uint[] data = null;
		//					   HeatMapDay last = null;
		//					   var lastModified = false;
		//					   while (--entry >= 0)
		//					   {
		//						ShellPage.WorkUpdate(workStr + $"... {count - entry}/{count}");
									
		//							var dName = ArchiveName(entry);

		//						   var prior = zip.GetEntry(dName);
		//						   var byteBuffer = new byte[prior.Length];
		//						   var t = GetLastWriteServer(prior);


		//						   using (var instream = prior.Open())
		//						   {
		//							   instream.Read(byteBuffer, 0, byteBuffer.Length);
		//						   }
		//						   var uintBuffer = byteBuffer.ConvertToUints();

		//						   if (data == null)
		//						   {
		//							   data = uintBuffer;
		//							   Assert(data.Length == World.spanSquared);
		//						   }
		//						   else
		//						   {
		//							   HeatMap.ApplyDelta(data, uintBuffer);
		//						   }
		//						   var map = await HeatMap.AddSnapshot(t,World.SwizzleForCompression(data), false);

		//						   if (map.day != last)
		//						   {
		//							   if (last != null && lastModified)
		//								   await HeatMap.Upload(last);


		//							   last = map.day;
		//							   lastModified = map.modified;

		//						   }
		//						   else
		//						   {
		//							   lastModified |= map.modified;
		//						   }
		//					   }
		//					   if (last != null)
		//						   await HeatMap.Upload(last);

		//				   }

		//			   }
		//		   }
		//		   finally
		//		   {
		//			   await file.DeleteAsync(); // donw with this
	 //  				   ShellPage.WorkEnd(workStr);
		//			}

		//	}
		//	catch(FileNotFoundException notFound)
		//	{
		//		return;  // file does not exist
		//	}
		//	catch(Exception ex)
		//	{
		//		LogEx(ex, false, eventName:"UploadHistory");
		//	}
		//}


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

        //public static byte[] ConvertToBytesWithoutDungeonsOrBosses( this uint [] data )
        //{
        //    int size4 = data.Length;
        //    var rv = new byte[size4 * 4];
        //    for (var i = 0; i < size4; ++i)
        //    {
        //        var src = data[i];
        //        switch (src & World.typeMask)
        //        {
        //            // removing dungeons reduces storage sigificantly
        //            case 0:
        //            case World.typeDungeon:
        //            case World.typeBoss:
        //                {
        //                    rv[i * 4 + 0] =
        //                    rv[i * 4 + 1] =
        //                    rv[i * 4 + 2] =
        //                    rv[i * 4 + 3] = 0;
        //                    break;
        //                }
        //            default:
        //                {
        //                    CopyBytes(src, rv, i);
        //                    break;
        //                }
        //        }
        //    }
        //    // Todo: use array pool
        //    return rv;
        //}
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
