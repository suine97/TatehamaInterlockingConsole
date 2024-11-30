using System;
using System.Windows;
using System.Windows.Controls;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Models;
using TatehamaInterlockingConsole.Services;
using TatehamaInterlockingConsole.ViewModels;

namespace TatehamaInterlockingConsole.Handlers
{
    public class ImageHandler
    {
        private readonly Sound _sound = Sound.Instance;
        private readonly Random _random = new Random();
        private readonly DataManager _dataManager = DataManager.Instance;
        private readonly DataUpdateViewModel _dataUpdateViewModel = DataUpdateViewModel.Instance;

        /// <summary>
        /// Imageクリックイベント処理
        /// </summary>
        /// <param name="image"></param>
        /// <param name="setting"></param>
        public void AttachImageClick(Image image, UIControlSetting setting)
        {
            // 左クリックイベント
            image.MouseLeftButtonDown += (s, e) =>
            {
                HandleMouseDown(setting, isLeftClick: true);
            };

            // 右クリックイベント
            image.MouseRightButtonDown += (s, e) =>
            {
                HandleMouseDown(setting, isLeftClick: false);
            };

            // マウスアップイベント
            image.MouseUp += (s, e) =>
            {
                HandleMouseUp(setting);
            };
        }

        /// <summary>
        /// 種類別のマウスダウン処理
        /// </summary>
        /// <param name="control"></param>
        /// <param name="isLeftClick"></param>
        private void HandleMouseDown(UIControlSetting control, bool isLeftClick)
        {
            int randomLeverSoundIndex = _random.Next(1, 5);

            switch (control.ControlType)
            {
                case "LeverImage":
                    HandleLeverImageMouseDown(control, isLeftClick, randomLeverSoundIndex);
                    break;
                case "KeyImage":
                    HandleKeyImageMouseDown(control, isLeftClick, randomLeverSoundIndex);
                    break;
                case "ButtonImage":
                    HandleButtonImageMouseDown(control);
                    break;
                default:
                    MessageBox.Show($"Image {(isLeftClick ? "Left" : "Right")} Click event {control.ClickEventName} | {control.UniqueName}");
                    break;
            }
        }

        /// <summary>
        /// 種類別のマウスアップ処理
        /// </summary>
        /// <param name="control"></param>
        private void HandleMouseUp(UIControlSetting control)
        {
            switch (control.ControlType)
            {
                case "ButtonImage":
                    HandleButtonImageMouseUp(control);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// LeverImageマウスダウン処理
        /// </summary>
        /// <param name="control"></param>
        /// <param name="isLeftClick"></param>
        /// <param name="soundIndex"></param>
        private void HandleLeverImageMouseDown(UIControlSetting control, bool isLeftClick, int soundIndex)
        {
            int newIndex;

            // LRパターンのみ別処理
            if (control.ImagePatternSymbol == "LR")
            {
                if (control.ImageIndex == -1 && !isLeftClick)
                {
                    newIndex = 1;
                }
                else if (control.ImageIndex == 1 && isLeftClick)
                {
                    newIndex = -1;
                }
                else
                {
                    return;
                }
            }
            else
            {
                int change = isLeftClick ? -1 : 1;
                newIndex = control.ImageIndex + change;

                if (!IsValidIndex(control.ImagePatternSymbol, newIndex))
                {
                    return;
                }
            }

            control.ImageIndex = newIndex;
            _dataUpdateViewModel.SetControlsetting(control);
            _sound.SoundPlay($"switch_0{soundIndex}", false);
        }

        /// <summary>
        /// KeyImageマウスダウン処理
        /// </summary>
        /// <param name="control"></param>
        /// <param name="isLeftClick"></param>
        /// <param name="soundIndex"></param>
        private void HandleKeyImageMouseDown(UIControlSetting control, bool isLeftClick, int soundIndex)
        {
            // Shiftキーが押されているかを判定
            bool isShiftPressed = (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Shift) == System.Windows.Input.ModifierKeys.Shift;

            if (isShiftPressed)
            {
                // Shiftキーが押されている場合、KeyInsertedを切り替え
                control.KeyInserted = !control.KeyInserted;
                if (control.KeyInserted)
                {
                    if (control.ImageIndex >= 0)
                    {
                        control.ImageIndex += 10;
                    }
                    else
                    {
                        control.ImageIndex -= 10;
                    }
                }
                else
                {
                    if (control.ImageIndex >= 0)
                    {
                        control.ImageIndex -= 10;
                    }
                    else
                    {
                        control.ImageIndex += 10;
                    }
                }
            }
            else if (control.KeyInserted)
            {
                int newIndex;

                // LRパターンのみ別処理
                if (control.ImagePatternSymbol == "KeyLR")
                {
                    if (control.ImageIndex == -11 && !isLeftClick)
                    {
                        newIndex = 11;
                    }
                    else if (control.ImageIndex == 11 && isLeftClick)
                    {
                        newIndex = -11;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    int change = isLeftClick ? -1 : 1;
                    newIndex = control.ImageIndex + change;

                    if (!IsValidIndex(control.ImagePatternSymbol, newIndex))
                    {
                        return;
                    }
                }

                control.ImageIndex = newIndex;
                _dataUpdateViewModel.SetControlsetting(control);
                _sound.SoundPlay($"switch_0{soundIndex}", false);
            }
        }

        /// <summary>
        /// ButtonImageマウスダウン処理
        /// </summary>
        /// <param name="control"></param>
        /// <param name="isLeftClick"></param>
        /// <param name="soundIndex"></param>
        private void HandleButtonImageMouseDown(UIControlSetting control)
        {
            if (control.ImageIndex != 1)
            {
                control.ImageIndex = 1;
                _dataUpdateViewModel.SetControlsetting(control);
                _sound.SoundPlay("button_01", false);
            }
        }

        /// <summary>
        /// ButtonImageマウスアップ処理
        /// </summary>
        /// <param name="control"></param>
        /// <param name="isLeftClick"></param>
        /// <param name="soundIndex"></param>
        private void HandleButtonImageMouseUp(UIControlSetting control)
        {
            if (control.ImageIndex != 0)
            {
                control.ImageIndex = 0;
                _dataUpdateViewModel.SetControlsetting(control);
                _sound.SoundPlay("button_02", false);
            }
        }

        /// <summary>
        /// Index有効判定
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsValidIndex(string pattern, int index)
        {
            switch (pattern)
            {
                case "NR":
                    return index == 0 || index == 1;
                case "LN":
                    return index == -1 || index == 0;
                case "LR":
                    return index == -1 || index == 1;
                case "LNR":
                    return index == -1 || index == 0 || index == 1;
                case "KeyNR":
                    return index == 0 || index == 1 || index == 10 || index == 11;
                case "KeyLN":
                    return index == -1 || index == 0 || index == -11 || index == 10;
                case "KeyLR":
                    return index == -1 || index == 1 || index == -11 || index == 11;
                case "KeyLNR":
                    return index == -1 || index == 0 || index == 1 || index == -11 || index == 10 || index == 11;
                default:
                    return false;
            }
        }
    }
}