using System.Windows;
using System.Windows.Controls;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole.Factories
{
    public static class KeyImageFactory
    {
        /// <summary>
        /// KeyImageコントロール作成処理
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static UIElement CreateKeyImageControl(UIControlSetting setting, bool drawing)
        {
            var canvas = new Canvas();

            // Base画像の読み込み
            var baseImage = BaseImageFactory.CreateBaseImageControl(setting);
            // 切り替え用画像の読み込み
            var changeImage = ImageFactory.CreateImageControl(setting, true);

            if (drawing)
            {
                canvas.Children.Add(baseImage);
                canvas.Children.Add(changeImage);
            }

            return canvas;
        }
    }
}
