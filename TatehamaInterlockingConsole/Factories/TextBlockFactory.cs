using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using TatehamaInterlockingConsole.Models;
using TatehamaInterlockingConsole.Handlers;

namespace TatehamaInterlockingConsole.Factories
{
    public static class TextBlockFactory
    {
        /// <summary>
        /// TextBlockコントロール作成処理
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static Grid CreateTextBlockControl(UIControlSetting setting, List<UIControlSetting> allSettings, bool clickEvent = true)
        {
            // Gridの作成
            var grid = new Grid
            {
                Width = setting.Width,
                Height = setting.Height,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(setting.BackgroundColor)),
                UseLayoutRounding = true
            };

            // TextBlockの作成
            var textBlock = new TextBlock
            {
                Text = setting.Text,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = setting.FontSize,
                FontFamily = new FontFamily("BIZ UDゴシック"),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(setting.TextColor)),
                Padding = new Thickness(0.5),
                SnapsToDevicePixels = true
            };

            // イベントが設定されている場合は、イベントをアタッチ
            if (setting.ClickEventName != string.Empty)
            {
                // クリックイベントが不要なら設定しない
                if (!clickEvent)
                {
                    grid.IsHitTestVisible = false;
                }
                TextBlockHandler.Instance.AttachTextBlockClick(grid, setting.ClickEventName);
            }

            // GridにTextBlockを追加
            grid.Children.Add(textBlock);

            // サイズ変更イベントを登録
            grid.SizeChanged += (s, e) =>
            {
                AdjustTextWidth(textBlock, grid.ActualWidth);
            };

            // 親ControlTypeがLeverImageなら角度を設定
            if (!string.IsNullOrWhiteSpace(setting.ParentName))
            {
                var parentsetting = allSettings.FirstOrDefault(all => all.UniqueName == setting.ParentName);

                if (parentsetting.ControlType.Contains("LeverImage"))
                {
                    RotateTransform rotateTransform = new RotateTransform();
                    grid.RenderTransform = rotateTransform;
                    grid.RenderTransformOrigin = new Point(setting.AngleOriginX, setting.AngleOriginY);

                    if (parentsetting.ImageIndex < 0)
                        rotateTransform.Angle = -40;
                    else if (parentsetting.ImageIndex > 0)
                        rotateTransform.Angle = 40;
                    else
                        rotateTransform.Angle = 0;
                }
            }

            return grid;
        }

        /// <summary>
        /// Textに応じてTextBlockの幅を調整
        /// </summary>
        /// <param name="textBlock"></param>
        /// <param name="availableWidth"></param>
        private static void AdjustTextWidth(TextBlock textBlock, double availableWidth)
        {
            var dpi = VisualTreeHelper.GetDpi(textBlock).PixelsPerDip;
            var formattedText = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                textBlock.Foreground,
                dpi);

            double textWidth = formattedText.Width;

            if (textWidth > availableWidth)
            {
                double scale = availableWidth / textWidth;
                textBlock.RenderTransformOrigin = new Point(0.5, 0.5);
                textBlock.LayoutTransform = new ScaleTransform(scale, 1.0);
            }
            else
            {
                textBlock.LayoutTransform = Transform.Identity;
            }
        }
    }
}
