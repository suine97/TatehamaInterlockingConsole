using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using TatehamaInterlockingConsole.Models;
using TatehamaInterlockingConsole.Manager;

namespace TatehamaInterlockingConsole.Factories
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
            var imagePath = setting.BaseImagePath;
            var imageSource = ImageCacheManager.GetImage(imagePath);

            var image = new Image
            {
                Source = imageSource,
                Width = setting.Width != 0 ? setting.Width : imageSource.Width,
                Height = setting.Height != 0 ? setting.Height : imageSource.Height,
                RenderTransform = new RotateTransform(setting.Angle)
            };

            RotateTransform rotateTransform = new RotateTransform();
            image.RenderTransform = rotateTransform;
            image.RenderTransformOrigin = new Point(setting.AngleOriginX, setting.AngleOriginY);
            rotateTransform.Angle = setting.Angle;

            return image;
        }
    }
}
