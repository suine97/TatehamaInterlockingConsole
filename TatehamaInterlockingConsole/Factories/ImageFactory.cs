using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TatehamaInterlockingConsole.Handlers;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole.Factories
{
    public static class ImageFactory
    {
        /// <summary>
        /// Imageコントロール作成処理
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Image CreateImageControl(UIControlSetting setting, int index = 0, bool clickEvent = true)
        {
            if (!setting.ImagePaths.TryGetValue(index, out string imagePath))
            {
                throw new KeyNotFoundException($"Index {index} に対応する画像パスが見つかりません。");
            }

            var imageSource = ImageCacheManager.GetImage(imagePath);

            if (imageSource == null)
            {
                imageSource = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute))
                {
                    CacheOption = BitmapCacheOption.OnLoad
                };
                ImageCacheManager.AddImage(imagePath, imageSource);
            }

            int pixelWidth = 0;
            int pixelHeight = 0;
            if (imageSource is BitmapSource bitmapSource)
            {
                pixelWidth = bitmapSource.PixelWidth;
                pixelHeight = bitmapSource.PixelHeight;
            }

            var image = new Image
            {
                Source = imageSource,
                Width = setting.Width != 0 ? setting.Width : pixelWidth,
                Height = setting.Height != 0 ? setting.Height : pixelHeight,
                RenderTransform = new RotateTransform(setting.Angle)
            };

            // イベントが設定されている場合は、イベントをアタッチ
            if (!string.IsNullOrEmpty(setting.ClickEventName))
            {
                // クリックイベントが不要なら設定しない
                if (!clickEvent)
                {
                    image.IsHitTestVisible = false;
                }
                new ImageHandler().AttachImageClick(image, setting);
            }

            RotateTransform rotateTransform = new RotateTransform();
            image.RenderTransform = rotateTransform;
            image.RenderTransformOrigin = new Point(setting.AngleOriginX, setting.AngleOriginY);
            rotateTransform.Angle = setting.Angle;

            return image;
        }

        /// <summary>
        /// Imageコントロール作成処理
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Image CreateImageControl(UIControlSetting setting, bool clickEvent = true, double angle = 0.0d)
        {
            if (!setting.ImagePaths.TryGetValue(setting.ImageIndex, out string imagePath))
            {
                throw new KeyNotFoundException($"ImageIndex {setting.ImageIndex} に対応する画像パスが見つかりません。");
            }

            var imageSource = ImageCacheManager.GetImage(imagePath);

            if (imageSource == null)
            {
                imageSource = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute))
                {
                    CacheOption = BitmapCacheOption.OnLoad
                };
                ImageCacheManager.AddImage(imagePath, imageSource); // キャッシュに追加
            }

            int pixelWidth = 0;
            int pixelHeight = 0;
            if (imageSource is BitmapSource bitmapSource)
            {
                pixelWidth = bitmapSource.PixelWidth;
                pixelHeight = bitmapSource.PixelHeight;
            }

            var image = new Image
            {
                Source = imageSource,
                Width = setting.Width != 0 ? setting.Width : pixelWidth,
                Height = setting.Height != 0 ? setting.Height : pixelHeight,
                RenderTransform = new RotateTransform(angle)
            };

            // イベントが設定されている場合は、イベントをアタッチ
            if (!string.IsNullOrEmpty(setting.ClickEventName))
            {
                // クリックイベントが不要なら設定しない
                if (!clickEvent)
                {
                    image.IsHitTestVisible = false;
                }
                new ImageHandler().AttachImageClick(image, setting);
            }

            RotateTransform rotateTransform = new RotateTransform();
            image.RenderTransform = rotateTransform;
            image.RenderTransformOrigin = new Point(setting.AngleOriginX, setting.AngleOriginY);
            rotateTransform.Angle = angle;

            return image;
        }
    }
}
