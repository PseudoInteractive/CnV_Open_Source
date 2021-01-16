using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace COTG.Helpers
{
    public static class ImageHelper
    {
        public static async Task<BitmapImage> ImageFromStringAsync(string data)
        {
            var byteArray = Convert.FromBase64String(data);
            var image = new BitmapImage();
            using (var stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(byteArray.AsBuffer());
                stream.Seek(0);
                await image.SetSourceAsync(stream);
            }

            return image;
        }
        static Dictionary<string, BitmapImage> assetsCache = new Dictionary<string, BitmapImage>();
        static Dictionary<string, BitmapImage> imagesCache = new Dictionary<string, BitmapImage>();
        
        public static BitmapImage FromImages(string fileName)
        {
            if (imagesCache.TryGetValue(fileName, out var o))
                return o;
            var image = new BitmapImage(new Uri($"ms-appx:///Content/Art/Icons/{fileName}"));
            imagesCache.Add(fileName, image);
            return image;
        }
        
        public static async Task<StorageFile> LoadImageFileAsync()
        {
            var openPicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".bmp");

            var imageFile = await openPicker.PickSingleFileAsync();

            return imageFile;
        }

        public static async Task<BitmapImage> GetBitmapFromImageAsync(StorageFile file)
        {
            if (file == null)
            {
                return null;
            }

            try
            {
                using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    var bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    return bitmapImage;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
