using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using TatehamaInterlockingConsole.CustomControl;
using TatehamaInterlockingConsole.Handlers;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole.Factories
{
    public static class ButtonFactory
    {
        /// <summary>
        /// Buttonコントロール作成処理
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static Button CreateButtonControl(UIControlSetting setting)
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

            // イベントが設定されている場合は、イベントをアタッチ
            if (setting.ClickEventName != string.Empty)
            {
                ButtonHandler.Instance.AttachButtonClick(button, setting.ClickEventName);
            }

            RotateTransform rotateTransform = new RotateTransform();
            button.RenderTransform = rotateTransform;
            button.RenderTransformOrigin = new Point(setting.AngleOriginX, setting.AngleOriginY);
            rotateTransform.Angle = setting.Angle;

            return button;
        }
    }
}
