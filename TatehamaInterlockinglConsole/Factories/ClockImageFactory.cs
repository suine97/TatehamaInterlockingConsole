using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using TatehamaInterlockinglConsole.Models;
using TatehamaInterlockinglConsole.Services;

namespace TatehamaInterlockinglConsole.Factories
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
            var baseImage = ImageFactory.CreateImageControl(setting, allSettings, 0);
            var hourHandImage = ImageFactory.CreateImageControl(setting, allSettings, 1);
            var minuteHandImage = ImageFactory.CreateImageControl(setting, allSettings, 2);
            var secondHandImage = ImageFactory.CreateImageControl(setting, allSettings, 3);
            var coverImage = ImageFactory.CreateImageControl(setting, allSettings, 4);

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
    }
}
