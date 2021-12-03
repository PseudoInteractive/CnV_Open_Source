using CnV;

using CnV.Game;
using CnV.Views;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

namespace CnV.Services
{
	using Game;
	using Views;

	public static class ChatStorage
    {
        static string fileName => $"whisper{JSClient.world}{Player.myId}.zip";
        static string ArchiveName(int playerId) => playerId.ToString();
        public static bool whisperInitialized;
        public struct WhisperMessage
        {
            public byte type { get; set; }
            public string text { get; set; }
            public DateTimeOffset time { get; set; }

        }
        public static List<WhisperMessage> all = new List<WhisperMessage>();
        public static StorageFolder folder => ApplicationData.Current.LocalFolder;

        static StorageFile storageFile;
        async static Task<StorageFile> GetFile()
        {
            if(storageFile!= null)
                return storageFile;
            return (storageFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists));
        }
        public static async void ReadWhisperData(int playerId)
        {
            var file = await GetFile();
            using (var streamForZip = await file.OpenStreamForWriteAsync())
            {
                using (var zip = new ZipArchive(streamForZip, mode: ZipArchiveMode.Update))
                {
                    int entries = zip.Entries.Count;
                    var prior = zip.GetEntry(ArchiveName(playerId));
                    if (prior != null)
                    {
                        var byteBuffer = new byte[prior.Length];
                        using (var instream = prior.Open())
                        {
                            instream.Read(byteBuffer, 0, byteBuffer.Length);
                        }

                        var temp = System.Text.Json.JsonSerializer.Deserialize<List<WhisperMessage>>(byteBuffer);
                        AppS.DispatchOnUIThreadLow(() =>
                        {

                            // add in new messages
                            temp.AddRange(all);
                            all = temp;
                            foreach (var message in all)
                            {
                                var ch = new ChatEntry(Player.IdToName(playerId), message.text, message.time.ToServerTime(), message.type);
                                // post!
                            }
                            whisperInitialized = true;
                        });
                    }
                    else
                    {
                        whisperInitialized = true;
                    }
                }
            }
        }

        public static async void SaveWhisperData(int playerId)
        {
            var file = await GetFile();
            
                using (var streamForZip = await file.OpenStreamForWriteAsync())
                {
                    using (var zip = new ZipArchive(streamForZip, mode: ZipArchiveMode.Update))
                    {
                        int entries = zip.Entries.Count;
                    var name = ArchiveName(playerId);
                        var prior = zip.GetEntry(name);
                        if (prior != null)
                            prior.Delete();
                        prior = zip.CreateEntry(name);
                        var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(all);

                        using (var outStream = prior.Open())
                        {
                     
                            outStream.Write(bytes, 0, bytes.Length);
                        }

                    }
                }
        }
    }
}
