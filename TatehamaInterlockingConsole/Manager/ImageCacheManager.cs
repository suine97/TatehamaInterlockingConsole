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
        /// 指定された画像をキャッシュに追加
        /// </summary>
        public static void AddImage(string filePath, ImageSource imageSource)
        {
            var normalizedPath = NormalizePath(filePath);

            if (!_cache.ContainsKey(normalizedPath))
            {
                _cache[normalizedPath] = imageSource;
            }
        }

        /// <summary>
        /// キャッシュから画像を取得
        /// </summary>
        public static ImageSource GetImage(string filePath)
        {
            var normalizedPath = NormalizePath(filePath);

            return _cache.TryGetValue(normalizedPath, out var imageSource)
                ? imageSource
                : null;
        }

        /// <summary>
        /// パスを正規化する
        /// </summary>
        private static string NormalizePath(string path)
        {
            if (Uri.TryCreate(path, UriKind.Absolute, out var uri) && uri.IsFile)
            {
                // file:// URI をローカルパスに変換
                return uri.LocalPath.Replace('\\', '/');
            }
            else
            {
                // 通常のローカルパスとして処理
                return Path.GetFullPath(path).Replace('\\', '/');
            }
        }

        /// <summary>
        /// キャッシュから画像を削除
        /// </summary>
        public static void RemoveImage(string filePath)
        {
            var normalizedPath = NormalizePath(filePath);

            if (_cache.TryGetValue(normalizedPath, out var imageSource))
            {
                if (imageSource is BitmapImage bitmapImage)
                {
                    bitmapImage.StreamSource?.Dispose(); // リソース解放
                }
                _cache.Remove(normalizedPath);
            }
        }

        /// <summary>
        /// キャッシュサイズを取得
        /// </summary>
        /// <returns></returns>
        public static int GetCacheSize()
        {
            return _cache.Count;
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
