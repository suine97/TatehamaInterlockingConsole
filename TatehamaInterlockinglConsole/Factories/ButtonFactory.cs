using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using TatehamaInterlockinglConsole.CustomControl;
using TatehamaInterlockinglConsole.Handlers;
using TatehamaInterlockinglConsole.Models;
using TatehamaInterlockinglConsole.Helpers;
using System.Collections.Generic;

namespace TatehamaInterlockinglConsole.Factories
{
    public static class ButtonFactory
    {
        /// <summary>
        /// Buttonコントロール作成処理
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static Button CreateButtonControl(UIControlSetting setting, List<UIControlSetting> allSettings)
        {
            var button = new CustomButton
            {
                Content = setting.Text,
                Width = setting.Width,
                Height = setting.Height,
                FontSize = setting.FontSize,
                FontFamily = new FontFamily("BIZ UDゴシック"),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(setting.BackgroundColor)),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(setting.TextColor)),
                RenderTransform = new RotateTransform(setting.Angle)
            };

            // 親コントロールが設定されている場合は、相対座標に変換
            ControlHelper.SetPosition(button, setting, allSettings);

            // イベントが設定されている場合は、イベントをアタッチ
            if (setting.ClickEventName != string.Empty)
            {
                new ButtonHandler().AttachButtonClick(button, setting.ClickEventName);
            }

            RotateTransform rotateTransform = new RotateTransform();
            button.RenderTransform = rotateTransform;
            button.RenderTransformOrigin = new Point(setting.AngleOriginX, setting.AngleOriginY);
            rotateTransform.Angle = setting.Angle;

            return button;
        }
    }
}
