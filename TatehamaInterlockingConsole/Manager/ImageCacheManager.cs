using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace TatehamaInterlockingConsole.Manager
{
    public static class ImageCacheManager
    {
        private static readonly Dictionary<string, ImageSource> _cache = new Dictionary<string, ImageSource>();

        /// <summary>
        /// 指定フォルダ内の画像を全てキャッシュにロードする
        /// </summary>
        public static void LoadImages(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"指定されたフォルダが見つかりません: {folderPath}");
            }

            foreach (var file in Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    var normalizedPath = NormalizePath(file); // 正規化
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(normalizedPath);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    _cache[normalizedPath] = bitmapImage;
                }
            }
        }

        /// <summary>
        /// キャッシュから画像を取得する
        /// </summary>
        public static ImageSource GetImage(string filePath)
        {
            var normalizedPath = NormalizePath(filePath); // 正規化
            if (_cache.TryGetValue(normalizedPath, out var imageSource))
            {
                return imageSource;
            }

            throw new FileNotFoundException($"指定された画像はキャッシュに存在しません: {filePath}");
        }

        /// <summary>
        /// パスを正規化する
        /// </summary>
        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(path).Replace('\\', '/');
        }

        /// <summary>
        /// キャッシュをクリアする
        /// </summary>
        public static void ClearCache()
        {
            _cache.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
