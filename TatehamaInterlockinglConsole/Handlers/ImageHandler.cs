using System;
using System.Windows;
using System.Windows.Controls;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Models;
using TatehamaInterlockingConsole.Services;
using TatehamaInterlockingConsole.ViewModels;

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
        private DataUpdateViewModel _dataUpdateViewModel = DataUpdateViewModel.Instance;

        public void AttachImageClick(Image image, UIControlSetting setting)
        {
            var control = setting;

            // 左クリックイベント
            image.MouseLeftButtonDown += (s, e) =>
            {
                switch (control.ControlType)
                {
                    case "LeverImage":
                        {
                            var num = _random.Next(1, 5);
                            switch (control.ImagePatternSymbol)
                            {
                                case "NR":
                                    if (control.ImageIndex != 0)
                                    {
                                        control.ImageIndex--;
                                        _dataUpdateViewModel.SetControlsetting(control);
                                        _sound.SoundPlay($"switch_0{num}", false);
                                    }
                                    break;
                                case "LN":
                                    if (control.ImageIndex != -1)
                                    {
                                        control.ImageIndex--;
                                        _dataUpdateViewModel.SetControlsetting(control);
                                        _sound.SoundPlay($"switch_0{num}", false);
                                    }
                                    break;
                                case "LR":
                                    if (control.ImageIndex != -1)
                                    {
                                        control.ImageIndex = -1;
                                        _dataUpdateViewModel.SetControlsetting(control);
                                        _sound.SoundPlay($"switch_0{num}", false);
                                    }
                                    break;
                                case "LNR":
                                    if (control.ImageIndex != -1)
                                    {
                                        control.ImageIndex--;
                                        _dataUpdateViewModel.SetControlsetting(control);
                                        _sound.SoundPlay($"switch_0{num}", false);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case "KeyImage":
                        {

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
            // 右クリックイベント
            image.MouseRightButtonDown += (s, e) =>
            {
                switch (setting.ControlType)
                {
                    case "LeverImage":
                        {
                            var num = _random.Next(1, 5);
                            switch (control.ImagePatternSymbol)
                            {
                                case "NR":
                                    if (control.ImageIndex != 1)
                                    {
                                        control.ImageIndex++;
                                        _dataUpdateViewModel.SetControlsetting(control);
                                        _sound.SoundPlay($"switch_0{num}", false);
                                    }
                                    break;
                                case "LN":
                                    if (control.ImageIndex != 0)
                                    {
                                        control.ImageIndex++;
                                        _dataUpdateViewModel.SetControlsetting(control);
                                        _sound.SoundPlay($"switch_0{num}", false);
                                    }
                                    break;
                                case "LR":
                                    if (control.ImageIndex != 1)
                                    {
                                        control.ImageIndex = 1;
                                        _dataUpdateViewModel.SetControlsetting(control);
                                        _sound.SoundPlay($"switch_0{num}", false);
                                    }
                                    break;
                                case "LNR":
                                    if (control.ImageIndex != 1)
                                    {
                                        control.ImageIndex++;
                                        _dataUpdateViewModel.SetControlsetting(control);
                                        _sound.SoundPlay($"switch_0{num}", false);
                                    }
                                    break;
                                default:
                                    break;
                            }
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
