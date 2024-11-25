using System.Collections.Generic;
using System.Windows;
using TatehamaInterlockinglConsole.Manager;
using TatehamaInterlockinglConsole.Models;

namespace TatehamaInterlockinglConsole.Factories
{
    /// <summary>
    /// UIコントロール生成クラス
    /// </summary>
    public static class ControlFactory
    {
        private static readonly List<UIAllImagePaths> list = DataManager.Instance.AllControlImagePathList;

        /// <summary>
        /// 種類別コントロール作成処理
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="allSettings"></param>
        /// <returns></returns>
        public static UIElement CreateControl(UIControlSetting setting, List<UIControlSetting> allSettings, bool drawing = true)
        {
            switch (setting.ControlType)
            {
                case "Button":
                    return ButtonFactory.CreateButtonControl(setting, allSettings);
                case "Label":
                    return LabelFactory.CreateLabelControl(setting, allSettings);
                case "TextBlock":
                    return TextBlockFactory.CreateTextBlockControl(setting, allSettings, false);
                case "Image":
                    return ImageFactory.CreateImageControl(list, setting, allSettings);
                case "BackImage":
                    return BackImageFactory.CreateBackImageControl(setting, allSettings);
                case "ClockImage":
                    return ClockImageFactory.CreateClockImageControl(setting, allSettings, drawing);
                case "LeverImage":
                    return LeverImageFactory.CreateLeverImageControl(list, setting, allSettings, drawing);
                case "KeyImage":
                    return KeyImageFactory.CreateKeyImageControl(list, setting, allSettings, drawing);
                case "ButtonImage":
                    return ButtonImageFactory.CreateButtonImageControl(list, setting, allSettings, drawing);
                case "Retsuban":
                    return ImageFactory.CreateImageControl(list, setting, allSettings);
                default:
                    return null;
            }
        }
    }
}
