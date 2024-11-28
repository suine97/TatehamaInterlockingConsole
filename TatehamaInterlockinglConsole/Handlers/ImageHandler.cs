using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Models;
using TatehamaInterlockingConsole.Services;

namespace TatehamaInterlockingConsole.Handlers
{
    /// <summary>
    /// Imageクリックイベントハンドラー
    /// </summary>
    public class ImageHandler
    {
        private readonly Sound _sound = Sound.Instance;
        private Random _random = new Random();
        private DataManager _dataManager = DataManager.Instance;

        public void AttachImageClick(Image image, UIControlSetting setting)
        {
            image.MouseLeftButtonDown += (s, e) =>
            {
                switch (setting.ControlType)
                {
                    case "LeverImage":
                        {
                            var num = _random.Next(1, 5);
                            _sound.SoundPlay($"switch_0{num}", false);
                        }
                        break;
                    case "ButtonImage":
                        {
                            var num = _random.Next(1, 3);
                            _sound.SoundPlay($"button_0{num}", false);
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
                            var num = _random.Next(1, 5);
                            _sound.SoundPlay($"switch_0{num}", false);
                        }
                        break;
                    case "ButtonImage":
                        {
                            var num = _random.Next(1, 3);
                            _sound.SoundPlay($"button_0{num}", false);
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
