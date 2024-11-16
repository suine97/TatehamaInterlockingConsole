using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TatehamaInterlockinglConsole.CustomControl;
using TatehamaInterlockinglConsole.Handlers;
using TatehamaInterlockinglConsole.Models;
using TatehamaInterlockinglConsole.Services;

namespace TatehamaInterlockinglConsole.Factories
{
    /// <summary>
    /// UIコントロール生成クラス
    /// </summary>
    public static class ControlFactory
    {
        public static double BackImageWidth { get; private set; }
        public static double BackImageHeight { get; private set; }
        public static DateTime CurrentTime { get; set; }

        private static List<UIControlSetting> AllSettings = new List<UIControlSetting>();

        public static UIElement CreateControl(UIControlSetting setting, List<UIControlSetting> allSettings)
        {
            AllSettings = allSettings;

            switch (setting.ControlType)
            {
                case "Button":
                    return CreateButtonControl(setting);
                case "Label":
                    return CreateLabelControl(setting);
                case "Image":
                    return CreateImageControl(setting);
                case "BackImage":
                    return CreateBackImageControl(setting);
                case "ClockImage":
                    return CreateClockImageControl(setting);
                default:
                    return null;
            }
        }

        private static Button CreateButtonControl(UIControlSetting setting)
        {
            var button = new CustomButton
            {
                Content = setting.Text,
                Width = setting.Width,
                Height = setting.Height,
                FontSize = setting.FontSize,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(setting.BackgroundColor)),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(setting.TextColor)),
                RenderTransform = new RotateTransform(setting.Angle)
            };

            // 親コントロールが設定されている場合は、相対座標に変換
            SetPosition(button, setting);

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

        private static Label CreateLabelControl(UIControlSetting setting)
        {
            var label = new Label
            {
                Content = setting.Text,
                Width = setting.Width,
                Height = setting.Height,
                FontSize = setting.FontSize,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(setting.BackgroundColor)),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(setting.TextColor)),
                RenderTransform = new RotateTransform(setting.Angle)
            };

            // 親コントロールが設定されている場合は、相対座標に変換
            SetPosition(label, setting);

            // イベントが設定されている場合は、イベントをアタッチ
            if (setting.ClickEventName != string.Empty)
            {
                new LabelHandler().AttachLabelClick(label, setting.ClickEventName);
            }

            RotateTransform rotateTransform = new RotateTransform();
            label.RenderTransform = rotateTransform;
            label.RenderTransformOrigin = new Point(setting.AngleOriginX, setting.AngleOriginY);
            rotateTransform.Angle = setting.Angle;

            return label;
        }

        private static Image CreateImageControl(UIControlSetting setting, int index = 0)
        {
            var bitmapImage = new BitmapImage(new Uri(setting.ImagePaths[index], UriKind.RelativeOrAbsolute));
            var image = new Image
            {
                Source = bitmapImage,
                Width = setting.Width != 0 ? setting.Width : bitmapImage.PixelWidth,
                Height = setting.Height != 0 ? setting.Height : bitmapImage.PixelHeight,
                RenderTransform = new RotateTransform(setting.Angle)
            };

            // 親コントロールが設定されている場合は、相対座標に変換
            SetPosition(image, setting);

            // イベントが設定されている場合は、イベントをアタッチ
            if (setting.ClickEventName != string.Empty)
            {
                new ImageHandler().AttachImageClick(image, setting.ClickEventName);
            }

            RotateTransform rotateTransform = new RotateTransform();
            image.RenderTransform = rotateTransform;
            image.RenderTransformOrigin = new Point(setting.AngleOriginX, setting.AngleOriginY);
            rotateTransform.Angle = setting.Angle;

            return image;
        }

        private static UIElement CreateBackImageControl(UIControlSetting setting)
        {
            var backImage = CreateImageControl(setting);

            BackImageWidth = backImage.Width;
            BackImageHeight = backImage.Height;

            return backImage;
        }

        private static UIElement CreateClockImageControl(UIControlSetting setting)
        {
            var canvas = new Canvas();
            // 時計の各画像を取得 (Base, 短針, 長針, 秒針, カバー)
            var baseImage = CreateImageControl(setting, 0);
            var hourHandImage = CreateImageControl(setting, 1);
            var minuteHandImage = CreateImageControl(setting, 2);
            var secondHandImage = CreateImageControl(setting, 3);
            var coverImage = CreateImageControl(setting, 4);

            // 針の回転用 Transform
            var hourRotateTransform = new RotateTransform();
            var minuteRotateTransform = new RotateTransform();
            var secondRotateTransform = new RotateTransform();

            hourHandImage.RenderTransform = hourRotateTransform;
            hourHandImage.RenderTransformOrigin = new Point(0.5, 0.5);
            minuteHandImage.RenderTransform = minuteRotateTransform;
            minuteHandImage.RenderTransformOrigin = new Point(0.5, 0.5);
            secondHandImage.RenderTransform = secondRotateTransform;
            secondHandImage.RenderTransformOrigin = new Point(0.5, 0.5);

            // 現在時刻を取得して針を設定
            Clock.SetClockHands(CurrentTime, hourRotateTransform, minuteRotateTransform, secondRotateTransform);

            // 秒針の更新タイマー
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (sender, e) =>
            {
                DateTime now = CurrentTime;
                Clock.SetClockHands(now, hourRotateTransform, minuteRotateTransform, secondRotateTransform);
            };
            timer.Start();

            // Canvasに時計の要素を追加
            canvas.Children.Add(baseImage);
            canvas.Children.Add(hourHandImage);
            canvas.Children.Add(minuteHandImage);
            canvas.Children.Add(secondHandImage);
            canvas.Children.Add(coverImage);

            return canvas;
        }

        private static UIControlSetting FindControlByName(string name)
        {
            return AllSettings.FirstOrDefault(control => control.UniqueName == name);
        }

        private static void SetPosition(UIElement element, UIControlSetting setting)
        {
            if (!string.IsNullOrEmpty(setting.ParentName))
            {
                var parentControl = FindControlByName(setting.ParentName);
                if (parentControl != null)
                {
                    Canvas.SetLeft(element, parentControl.X + setting.RelativeX);
                    Canvas.SetTop(element, parentControl.Y + setting.RelativeY);
                }
                else
                {
                    Canvas.SetLeft(element, setting.X);
                    Canvas.SetTop(element, setting.Y);
                }
            }
            else
            {
                Canvas.SetLeft(element, setting.X);
                Canvas.SetTop(element, setting.Y);
            }
        }
    }
}
