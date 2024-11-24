using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using TatehamaInterlockinglConsole.Handlers;
using TatehamaInterlockinglConsole.Models;
using System.Windows.Controls;
using TatehamaInterlockinglConsole.Utilities;

namespace TatehamaInterlockinglConsole.Factories
{
    public static class ImageFactory
    {
        /// <summary>
        /// Imageコントロール作成処理
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Image CreateImageControl(UIControlSetting setting, List<UIControlSetting> allSettings, int index = 0, bool clickEvent = true)
        {
            var bitmapImage = new BitmapImage(new Uri(setting.ImagePaths[index], UriKind.RelativeOrAbsolute));
            var image = new Image
            {
                Source = bitmapImage,
                Width = setting.Width != 0 ? setting.Width : bitmapImage.PixelWidth,
                Height = setting.Height != 0 ? setting.Height : bitmapImage.PixelHeight,
                RenderTransform = new RotateTransform(setting.Angle)
            };

            // 親コントロールが設定されている場合は、相対座標に変換
            PositionUtilities.SetPosition(image, setting, allSettings);

            // イベントが設定されている場合は、イベントをアタッチ
            if (setting.ClickEventName != string.Empty)
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
    }
}
