using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HuyaGPT;

public static class ImageUtil
{
    private static readonly HttpClient Client = new();
    public static async Task<BitmapImage> LoadImageFromUrl(string url)
    {
        var imageData = await Client.GetByteArrayAsync(url);
        var bitmapImage = new BitmapImage();
        using var memoryStream = new MemoryStream(imageData);
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.StreamSource = memoryStream;
        bitmapImage.EndInit();
        bitmapImage.Freeze(); // 提高图像的性能和线程安全性

        // 在UI线程上更新图像
        return bitmapImage;
    }
}