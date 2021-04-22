using System;
using System.IO;
using System.Threading.Tasks;

using COTG.Core.Helpers;

using Windows.Storage;
using Windows.Storage.Streams;
using System.Text.Json;
using Windows.Foundation;
using Windows.Storage.Compression;
using System.Runtime.InteropServices.WindowsRuntime;

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
            var fileContent = JsonSerializer.Serialize(content, Json.jsonSerializerOptions);

            await FileIO.WriteTextAsync(file, fileContent);
        }

		

		public static async Task<T> ReadAsync<T>(this StorageFolder folder, string name, T _default)
        {
			var fileName = GetFileName(name);

			if (!File.Exists(Path.Combine(folder.Path, fileName)))
            {
                return _default;
            }

            var file = await folder.GetFileAsync(fileName);
            var fileContent = await FileIO.ReadTextAsync(file);
			if (fileContent.IsNullOrEmpty())
				return _default;

            return JsonSerializer.Deserialize<T>(fileContent, Json.jsonSerializerOptions);
        }




        public static void Save<T>(this ApplicationDataContainer settings, string key, T value)
        {
            if (typeof(T) == typeof(Nullable<bool>))
            {
              settings.Values[key] = value switch { null => -1, true => 1,false=> 0, _ => -1 };
            }
            else
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
					DateTimeOffset a => a,
                    TimeSpan a => a,
                    Guid a => a,
                    Point a => a,
                    Size a => a,
                    Rect a => a,
                    ApplicationDataCompositeValue a => a,
                    _ => JsonSerializer.Serialize<T>(value, Json.jsonSerializerOptions)
                };
            }
        }
        public static void SaveT(this ApplicationDataContainer settings, string key,Type t,object value)
        {
            if (t == typeof(Nullable<bool>))
            {
                settings.Values[key] = value switch { null => -1, true => 1, false => 0, _ => -1 };
            }
            else
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
                    DateTimeOffset a => a,
                    TimeSpan a => a,
                    Guid a => a,
                    Point a => a,
                    Size a => a,
                    Rect a => a,
                    ApplicationDataCompositeValue a => a,
                    _ => JsonSerializer.Serialize(value,t)
                };
            }
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
                    obj = JsonSerializer.Deserialize<T>((string)obj, Json.jsonSerializerOptions);
                return (T)obj;
            }

            return _default;
        }

        public static object ReadT(this ApplicationDataContainer settings, string key, Type t, object _default) 
        {

            try
            {
                if (settings.Values.TryGetValue(key, out var o))
                {
                    if(t == typeof(Nullable<bool>) )
                    {
                            bool? rv = (int)o switch { 0 => false, 1 => true, _ => null };
                            return rv;
                    }
                    if (o is string && t != typeof(string))
                        return JsonSerializer.Deserialize((string)o, t);
                    else
                        return o;
                }
            }
            catch (Exception e) // not stored properly
            {
                COTG.Debug.LogEx(e);
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
namespace COTG
{
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

		public static async Task<byte[]> GetContent(string filename)
		{
			var uri = new Uri("ms-appx:///" + filename);
			var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
	
			var buffer = await FileIO.ReadBufferAsync(file);

			return buffer.ToArray();
		}
	}
}
