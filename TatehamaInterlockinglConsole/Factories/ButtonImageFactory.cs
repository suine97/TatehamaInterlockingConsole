using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using TatehamaInterlockinglConsole.Models;

namespace TatehamaInterlockinglConsole.Factories
{
    public static class ButtonImageFactory
    {
        /// <summary>
        /// ButtonImageコントロール作成処理
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static UIElement CreateButtonImageControl(UIControlSetting setting, List<UIControlSetting> allSettings, bool drawing)
        {
            var canvas = new Canvas();

            // Base画像の読み込み
            var baseImage = BaseImageFactory.CreateBaseImageControl(setting, allSettings);
            // 切り替え用画像の読み込み
            var changeImage = ImageFactory.CreateImageControl(setting, allSettings, true);

            if (drawing)
            {
                canvas.Children.Add(baseImage);
                canvas.Children.Add(changeImage);
            }

            return canvas;
        }
    }
}
