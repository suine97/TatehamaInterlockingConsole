﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using TatehamaInterlockingConsole.Handlers;
using TatehamaInterlockingConsole.Models;
using TatehamaInterlockingConsole.Manager;

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
        public static Image CreateImageControl(UIControlSetting setting, List<UIControlSetting> allSettings, int index = 0, bool clickEvent = true)
        {
            if (!setting.ImagePaths.ContainsKey(index))
            {
                throw new KeyNotFoundException($"Index {index} に対応する画像パスが見つかりません。");
            }

            var imagePath = setting.ImagePaths[setting.ImageIndex];
            var imageSource = ImageCacheManager.GetImage(imagePath);

            var image = new Image
            {
                Source = imageSource,
                Width = setting.Width != 0 ? setting.Width : imageSource.Width,
                Height = setting.Height != 0 ? setting.Height : imageSource.Height,
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
        public static Image CreateImageControl(UIControlSetting setting, List<UIControlSetting> allSettings, bool clickEvent = true, double angle = 0.0d)
        {
            if (!setting.ImagePaths.ContainsKey(setting.ImageIndex))
            {
                throw new KeyNotFoundException($"ImageIndex {setting.ImageIndex} に対応する画像パスが見つかりません。");
            }

            var imagePath = setting.ImagePaths[setting.ImageIndex];
            var imageSource = ImageCacheManager.GetImage(imagePath);

            var image = new Image
            {
                Source = imageSource,
                Width = setting.Width != 0 ? setting.Width : imageSource.Width,
                Height = setting.Height != 0 ? setting.Height : imageSource.Height,
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