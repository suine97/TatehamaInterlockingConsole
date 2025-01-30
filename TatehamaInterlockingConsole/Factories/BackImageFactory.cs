using System.Windows;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole.Factories
{
    public static class BackImageFactory
    {
        public static double BackImageWidth { get; private set; }
        public static double BackImageHeight { get; private set; }

        /// <summary>
        /// BackImageコントロール作成処理
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static UIElement CreateBackImageControl(UIControlSetting setting)
        {
            var backImage = ImageFactory.CreateImageControl(setting, 0);

            BackImageWidth = backImage.Width;
            BackImageHeight = backImage.Height;

            return backImage;
        }
    }
}
