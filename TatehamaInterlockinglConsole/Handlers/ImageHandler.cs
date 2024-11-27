using System;
using System.Windows;
using System.Windows.Controls;
using TatehamaInterlockinglConsole.Models;
using TatehamaInterlockinglConsole.Helpers;

namespace TatehamaInterlockinglConsole.Handlers
{
    /// <summary>
    /// Imageクリックイベントハンドラー
    /// </summary>
    public class ImageHandler
    {
        private readonly Sound _sound = Sound.Instance;
        private Random _random = new Random();

        public void AttachImageClick(Image image, UIControlSetting setting)
        {
            image.MouseLeftButtonDown += (s, e) =>
            {
                switch (setting.ControlType)
                {
                    case "LeverImage":
                        {
                            var num = _random.Next(1, 7);
                            _sound.SoundPlay($"switch_0{num}", false);
                        }
                        break;
                    default:
                        MessageBox.Show($"Image Left Click event {setting.ClickEventName} | {setting.UniqueName}");
                        break;
                }
            };
            image.MouseRightButtonDown += (s, e) =>
            {
                switch (setting.ControlType)
                {
                    case "LeverImage":
                        {
                            var num = _random.Next(1, 7);
                            _sound.SoundPlay($"switch_0{num}", false);
                        }
                        break;
                    default:
                        MessageBox.Show($"Image Right Click event {setting.ClickEventName} | {setting.UniqueName}");
                        break;
                }
            };
        }
    }
}
