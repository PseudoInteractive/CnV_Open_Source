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

namespace COTG.Services
{
    public static class WorldStorage
    {
        const string fileName = "gwrd.zip";
        public static StorageFolder folder => ApplicationData.Current.LocalFolder;
     
        const int deltasPerBatch = 16;
        // Todo:  Delta encode
        public static byte[] ComputeDelta(uint[] d0, uint[] d1)
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
            var outCount = rv.Count;
            if (outCount <= 64)
                return null;
            var bytes = new byte[outCount*4];
            for (int i = 0; i < outCount; ++i)
            {
                CopyBytes(rv[i], bytes, i);
            }
            return bytes;
        }


        public static string ArchiveName(int entryId) => entryId.ToString("D6");
        public static async void SaveWorldData(uint[] data)
        {
            { 
                var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);

                using (var streamForZip = await file.OpenStreamForWriteAsync())
                {
                    using (var zip = new ZipArchive(streamForZip, mode: ZipArchiveMode.Update))
                    {
                        int entries = zip.Entries.Count;
                        var name = ArchiveName(entries);
                        if ( (entries % deltasPerBatch)!= 0)
                        {
                            // convert the last entry to a delta record
                            var priorName = ArchiveName(entries - 1);
                            var prior = zip.GetEntry(priorName);
                            var byteBuffer = new byte[data.Length*4];
                            using( var instream = prior.Open())
                            {
                                instream.Read(byteBuffer,0,byteBuffer.Length);
                            }
                            var lastData = byteBuffer.ConvertToUints();
                            var delta = ComputeDelta(lastData,data);
                            if (delta == null)
                                goto end; // no changes

                            prior.Delete(); // we want to replace it
                            prior = zip.CreateEntry(priorName);
                            using (var outStream = prior.Open())
                            {
                                // overwrite with delta values
                                outStream.Write(delta, 0, delta.Length);
                            }
                            //then fall through and add the full entry

                        }
                        // write a whole record.
                        var entry = zip.CreateEntry(name);
                            using (var outStream = entry.Open())
                            {
                                var byteData = data.ConvertToBytesWithoutDungeons();
                                outStream.Write(byteData, 0, byteData.Length);
                            }
                        
                    }
                }
            }
        end:;
#if(TRACE)
            Task.Delay(1000).ContinueWith((_) => LoadWorldData());
#endif

        }

        public static async void LoadWorldData()
        {
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);

            using (var streamForZip = await file.OpenStreamForReadAsync())
            {
                using (var zip = new ZipArchive(streamForZip, mode: ZipArchiveMode.Read))
                {
                    foreach (var entry in zip.Entries)
                    {
                        Note.L($"{entry.Name} size: {entry.CompressedLength} Write: {entry.LastWriteTime}");
                        
                    }
                    int entries = zip.Entries.Count;
                    var priorName = ArchiveName(entries - 1);
                    var prior = zip.GetEntry(priorName);
                    Assert(prior.Length == 600 * 600 * 4);
                    var byteBuffer = new byte[prior.Length];
                    using (var instream = prior.Open())
                    {
                        instream.Read(byteBuffer, 0, byteBuffer.Length);
                    }
                    var lastData = byteBuffer.ConvertToUints();
                    World.CreateChangePixels(lastData);
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
