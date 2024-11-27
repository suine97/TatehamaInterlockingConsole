using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using TatehamaInterlockinglConsole.Models;
using System.Windows.Controls;
using TatehamaInterlockinglConsole.Helpers;

namespace TatehamaInterlockinglConsole.Factories
{
    public static class BaseImageFactory
    {
        /// <summary>
        /// BaseImageコントロール作成処理
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Image CreateBaseImageControl(UIControlSetting setting, List<UIControlSetting> allSettings)
        {
            var bitmapImage = new BitmapImage(new Uri(setting.BaseImagePath, UriKind.RelativeOrAbsolute));
            var image = new Image
            {
                Source = bitmapImage,
                Width = setting.Width != 0 ? setting.Width : bitmapImage.PixelWidth,
                Height = setting.Height != 0 ? setting.Height : bitmapImage.PixelHeight,
                RenderTransform = new RotateTransform(setting.Angle)
            };

            // 親コントロールが設定されている場合は、相対座標に変換
            ControlHelper.SetPosition(image, setting, allSettings);

            RotateTransform rotateTransform = new RotateTransform();
            image.RenderTransform = rotateTransform;
            image.RenderTransformOrigin = new Point(setting.AngleOriginX, setting.AngleOriginY);
            rotateTransform.Angle = setting.Angle;

            return image;
        }
    }
}
