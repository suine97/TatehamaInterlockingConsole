using System.Windows;
using System.Collections.Generic;
using TatehamaInterlockingConsole.Models;
using TatehamaInterlockingConsole.Helpers;

namespace TatehamaInterlockingConsole.Factories
{
    /// <summary>
    /// UIコントロール生成クラス
    /// </summary>
    public static class ControlFactory
    {
        /// <summary>
        /// 種類別コントロール作成処理
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="allSettings"></param>
        /// <returns></returns>
        public static UIElement CreateControl(UIControlSetting setting, List<UIControlSetting> allSettings, bool drawing = true)
        {
            UIElement control = null;

            switch (setting.ControlType)
            {
                case "Button":
                    control = ButtonFactory.CreateButtonControl(setting);
                    break;
                case "Label":
                    control = LabelFactory.CreateLabelControl(setting);
                    break;
                case "TextBlock":
                    control = TextBlockFactory.CreateTextBlockControl(setting, allSettings, false);
                    break;
                case "Image":
                    control = ImageFactory.CreateImageControl(setting, true);
                    break;
                case "BackImage":
                    control = BackImageFactory.CreateBackImageControl(setting);
                    break;
                case "ClockImage":
                    control = ClockImageFactory.CreateClockImageControl(setting, drawing);
                    break;
                case "LeverImage":
                    control = LeverImageFactory.CreateLeverImageControl(setting, drawing);
                    break;
                case "KeyImage":
                    control = KeyImageFactory.CreateKeyImageControl(setting, drawing);
                    break;
                case "ButtonImage":
                    control = ButtonImageFactory.CreateButtonImageControl(setting, drawing);
                    break;
                case "Retsuban":
                    control = RetsubanFactory.CreateRetsubanImageControl(setting, drawing);
                    break;
                default:
                    break;
            }

            if (control != null)
            {
                // 親コントロールが設定されている場合は、相対座標に変換
                ControlHelper.SetPosition(control, setting, allSettings);
            }
            return control;
        }
    }
}
