using System.Collections.Generic;
using System.Windows;
using TatehamaInterlockinglConsole.Models;

namespace TatehamaInterlockinglConsole.Factories
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
            var backImage = ImageFactory.CreateImageControl(setting, allSettings);

            BackImageWidth = backImage.Width;
            BackImageHeight = backImage.Height;

            return backImage;
        }
    }
}
