using System.Windows;
using System.Collections.Generic;
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
        public static UIElement CreateBackImageControl(UIControlSetting setting, List<UIControlSetting> allSettings)
        {
            var backImage = ImageFactory.CreateImageControl(setting, allSettings, 0);

            BackImageWidth = backImage.Width;
            BackImageHeight = backImage.Height;

            return backImage;
        }
    }
}
