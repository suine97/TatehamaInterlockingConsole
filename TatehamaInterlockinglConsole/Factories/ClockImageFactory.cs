using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using TatehamaInterlockingConsole.Models;
using TatehamaInterlockingConsole.Services;

namespace TatehamaInterlockingConsole.Factories
{
    internal class ClockImageFactory
    {
        public static DateTime CurrentTime { get; set; }

        /// <summary>
        /// ClockImageコントロール作成処理
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static UIElement CreateClockImageControl(UIControlSetting setting, List<UIControlSetting> allSettings, bool drawing)
        {
            var canvas = new Canvas();
            // 時計の各画像を取得 (Base, 短針, 長針, 秒針, カバー)
            var baseImage = BaseImageFactory.CreateBaseImageControl(setting, allSettings);
            var hourHandImage = ImageFactory.CreateImageControl(setting, allSettings, 0);
            var minuteHandImage = ImageFactory.CreateImageControl(setting, allSettings, 1);
            var secondHandImage = ImageFactory.CreateImageControl(setting, allSettings, 2);
            var coverImage = ImageFactory.CreateImageControl(setting, allSettings, 3);

            hourHandImage.Tag = "HourHand";
            minuteHandImage.Tag = "MinuteHand";
            secondHandImage.Tag = "SecondHand";

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

            // Canvasに時計の要素を追加
            if (drawing)
            {
                canvas.Children.Add(baseImage);
                canvas.Children.Add(hourHandImage);
                canvas.Children.Add(minuteHandImage);
                canvas.Children.Add(secondHandImage);
                canvas.Children.Add(coverImage);
            }

            return canvas;
        }

        /// <summary>
        /// 時計の各針更新処理
        /// </summary>
        /// <param name="clockCanvas"></param>
        public static void UpdateClockHands(Canvas clockCanvas)
        {
            // 時計の各針を更新
            foreach (var child in clockCanvas.Children)
            {
                if (child is Image image && image.RenderTransform is RotateTransform rotateTransform)
                {
                    if (image.Tag is string tag)
                    {
                        switch (tag)
                        {
                            case "HourHand":
                                rotateTransform.Angle = (CurrentTime.Hour % 12) * 30 + CurrentTime.Minute * 0.5;
                                break;
                            case "MinuteHand":
                                rotateTransform.Angle = CurrentTime.Minute * 6 + CurrentTime.Second * 0.1;
                                break;
                            case "SecondHand":
                                rotateTransform.Angle = CurrentTime.Second * 6;
                                break;
                        }
                    }
                }
            }
        }
    }
}
