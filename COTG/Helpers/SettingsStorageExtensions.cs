using System;
using System.IO;
using System.Threading.Tasks;

using COTG.Core.Helpers;

using Windows.Storage;
using Windows.Storage.Streams;
using System.Text.Json;
using Windows.Foundation;

namespace COTG.Helpers
{
    // Use these extension methods to store and retrieve local and roaming app data
    // More details regarding storing and retrieving app data at https://docs.microsoft.com/windows/uwp/app-settings/store-and-retrieve-app-data
    public static class SettingsStorageExtensions
    {
        private const string FileExtension = ".json";

        public static bool IsRoamingStorageAvailable(this ApplicationData appData)
        {
            return appData.RoamingStorageQuota == 0;
        }

        public static async Task SaveAsync<T>(this StorageFolder folder, string name, T content)
        {
            var file = await folder.CreateFileAsync(GetFileName(name), CreationCollisionOption.ReplaceExisting);
            var fileContent = JsonSerializer.Serialize(content);

            await FileIO.WriteTextAsync(file, fileContent);
        }

        public static async Task<T> ReadAsync<T>(this StorageFolder folder, string name)
        {
            if (!File.Exists(Path.Combine(folder.Path, GetFileName(name))))
            {
                return default(T);
            }

            var file = await folder.GetFileAsync($"{name}.json");
            var fileContent = await FileIO.ReadTextAsync(file);

            return JsonSerializer.Deserialize<T>(fileContent);
        }




        public static void Save<T>(this ApplicationDataContainer settings, string key, T value)
        {

            settings.Values[key] = value switch
            {
                int a => a,
                byte a => a,
                sbyte a => a,
                uint a => a,
                ulong a => a,
                float a => a,
                double a => a,
                string a => a,
                bool a => a,
                DateTime a => a,
                TimeSpan a => a,
                Guid a => a,
                Point a => a,
                Size a => a,
                Rect a => a,
                ApplicationDataCompositeValue a => a,
                _ => JsonSerializer.Serialize<T>(value)
            };
        }


        public static void SaveString(this ApplicationDataContainer settings, string key, string value)
        {
            settings.Values[key] = value;
        }

        public static T Read<T>(this ApplicationDataContainer settings, string key, T _default = default)
        {

            if (settings.Values.TryGetValue(key, out var obj))
            {
                if (obj is string && typeof(T) != typeof(string))
                    obj = JsonSerializer.Deserialize<T>((string)obj);
                return (T)obj;
            }

            return _default;
        }

        public static async Task<StorageFile> SaveFileAsync(this StorageFolder folder, byte[] content, string fileName, CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("ExceptionSettingsStorageExtensionsFileNameIsNullOrEmpty".GetLocalized(), nameof(fileName));
            }

            var storageFile = await folder.CreateFileAsync(fileName, options);
            await FileIO.WriteBytesAsync(storageFile, content);
            return storageFile;
        }

        public static async Task<byte[]> ReadFileAsync(this StorageFolder folder, string fileName)
        {
            var item = await folder.TryGetItemAsync(fileName).AsTask().ConfigureAwait(false);

            if ((item != null) && item.IsOfType(StorageItemTypes.File))
            {
                var storageFile = await folder.GetFileAsync(fileName);
                byte[] content = await storageFile.ReadBytesAsync();
                return content;
            }

            return null;
        }

        public static async Task<byte[]> ReadBytesAsync(this StorageFile file)
        {
            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    using (var reader = new DataReader(stream.GetInputStreamAt(0)))
                    {
                        await reader.LoadAsync((uint)stream.Size);
                        var bytes = new byte[stream.Size];
                        reader.ReadBytes(bytes);
                        return bytes;
                    }
                }
            }

            return null;
        }

        private static string GetFileName(string name)
        {
            return string.Concat(name, FileExtension);
        }
    }
}
namespace COTG { 
    partial class App
	{
        public static ApplicationDataContainer Settings()
        {
            var appData = ApplicationData.Current;
            if (appData.RoamingStorageQuota > 4)
                return appData.RoamingSettings;
            else
                return appData.LocalSettings;
        }
    }
}
