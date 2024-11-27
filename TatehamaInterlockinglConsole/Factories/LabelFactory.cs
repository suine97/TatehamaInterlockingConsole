using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using TatehamaInterlockinglConsole.Handlers;
using TatehamaInterlockinglConsole.Models;
using TatehamaInterlockinglConsole.Helpers;

namespace TatehamaInterlockinglConsole.Factories
{
    public static class LabelFactory
    {
        /// <summary>
        /// Labelコントロール作成処理
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static Label CreateLabelControl(UIControlSetting setting, List<UIControlSetting> allSettings, bool clickEvent = true)
        {
            var label = new Label
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
            ControlHelper.SetPosition(label, setting, allSettings);

            // イベントが設定されている場合は、イベントをアタッチ
            if (setting.ClickEventName != string.Empty)
            {
                // クリックイベントが不要なら設定しない
                if (!clickEvent)
                {
                    label.IsHitTestVisible = false;
                }
                new LabelHandler().AttachLabelClick(label, setting);
            }

            RotateTransform rotateTransform = new RotateTransform();
            label.RenderTransform = rotateTransform;
            label.RenderTransformOrigin = new Point(setting.AngleOriginX, setting.AngleOriginY);
            rotateTransform.Angle = setting.Angle;

            return label;
        }
    }
}
