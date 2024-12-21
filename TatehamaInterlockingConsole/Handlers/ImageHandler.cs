using System;
using System.Diagnostics;
using System.Linq;
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
            switch (control.ControlType)
            {
                case "LeverImage":
                    HandleLeverImageMouseDown(control, isLeftClick);
                    break;
                case "KeyImage":
                    HandleKeyImageMouseDown(control, isLeftClick);
                    break;
                case "ButtonImage":
                    HandleButtonImageMouseDown(control);
                    break;
                case "Retsuban":
                    HandleRetsubanMouseDown(control, isLeftClick);
                    break;
                default:
                    // MessageBox.Show($"Image {(isLeftClick ? "Left" : "Right")} MouseDown event {control.ClickEventName} | {control.UniqueName}");
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
                    // MessageBox.Show($"Image {(isLeftClick ? "Left" : "Right")} MouseUp event {control.ClickEventName} | {control.UniqueName}");
                    break;
            }
        }

        /// <summary>
        /// LeverImageマウスダウン処理
        /// </summary>
        /// <param name="control"></param>
        /// <param name="isLeftClick"></param>
        /// <param name="soundIndex"></param>
        private void HandleLeverImageMouseDown(UIControlSetting control, bool isLeftClick)
        {
            int newIndex;
            string randomSwitchSoundIndex = _random.Next(1, 9).ToString("00");

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
            _sound.SoundPlay($"switch_{randomSwitchSoundIndex}", false);

            // サーバーリクエスト送信判定
            if ((control.LeverType == "方向てこ") && IsBothReleaseLeversManual(control))
            {
                // 双方の方向てこ解放鍵「駅扱」位置
                // サーバーへリクエスト送信
                Debug.WriteLine($"Server Request : {control.UniqueName} : {control.ImageIndex}");
            }
            else if ((control.LeverType != "方向てこ") && IsKeyLeverManual(control))
            {
                // 鍵「駅扱」位置
                // サーバーへリクエスト送信
                Debug.WriteLine($"Server Request : {control.UniqueName} : {control.ImageIndex}");
            }
        }

        /// <summary>
        /// KeyImageマウスダウン処理
        /// </summary>
        /// <param name="control"></param>
        /// <param name="isLeftClick"></param>
        /// <param name="soundIndex"></param>
        private void HandleKeyImageMouseDown(UIControlSetting control, bool isLeftClick)
        {
            string randomKeyInsertSoundIndex = _random.Next(1, 6).ToString("00");
            string randomKeyChainSoundIndex = _random.Next(1, 10).ToString("00");
            string randomKeyRemoveSoundIndex = _random.Next(1, 6).ToString("00");
            string randomKeyRejectSoundIndex = _random.Next(1, 4).ToString("00");
            string randomSwitchSoundIndex = _random.Next(1, 9).ToString("00");

            // Shiftキーが押されているかを判定
            bool isShiftPressed = (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Shift) == System.Windows.Input.ModifierKeys.Shift;
            // Controlキーが押されているかを判定
            bool isControlPressed = (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control;

            if (isShiftPressed)
            {
                // Shiftキーが押されている場合、KeyInsertedを切り替え
                if (control.KeyInserted)
                {
                    if (control.ImageIndex >= 0)
                    {
                        control.ImageIndex -= 10;
                    }
                    else
                    {
                        control.ImageIndex += 10;
                    }
                    _sound.SoundPlay($"keychain_{randomKeyChainSoundIndex}", false);
                    _sound.SoundPlay($"remove_{randomKeyRemoveSoundIndex}", false);
                }
                else
                {
                    if (control.ImageIndex >= 0)
                    {
                        control.ImageIndex += 10;
                    }
                    else
                    {
                        control.ImageIndex -= 10;
                    }
                    _sound.SoundPlay($"keychain_{randomKeyChainSoundIndex}", false);
                    _sound.SoundPlay($"insert_{randomKeyInsertSoundIndex}", false);
                }
                control.KeyInserted = !control.KeyInserted;
            }
            else if (isControlPressed)
            {
                // (デバッグ) Controlが押されている場合、権限無効判定
                _sound.SoundPlay($"keychain_{randomKeyChainSoundIndex}", false);
                _sound.SoundPlay($"reject_{randomKeyRejectSoundIndex}", false);
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
                _sound.SoundPlay($"switch_{randomSwitchSoundIndex}", false);
            }
            // 鍵位置設定
            control.KeyManual = ((control.LeverType == "駅扱切換") && (control.ImageIndex < 0))
                || ((control.LeverType == "方向てこ解放") && (control.ImageIndex > 0));
        }

        /// <summary>
        /// ButtonImageマウスダウン処理
        /// </summary>
        /// <param name="control"></param>
        /// <param name="isLeftClick"></param>
        /// <param name="soundIndex"></param>
        private void HandleButtonImageMouseDown(UIControlSetting control)
        {
            string randomPushSoundIndex = _random.Next(1, 4).ToString("00");
            
            if (control.ImageIndex != 1)
            {
                control.ImageIndex = 1;
                _dataUpdateViewModel.SetControlsetting(control);
                _sound.SoundPlay($"push_{randomPushSoundIndex}", false);

                // 鍵位置「駅扱」判定
                if (!IsKeyLeverManual(control))
                {
                    // サーバーへリクエスト送信
                }
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
            string randomPullSoundIndex = _random.Next(1, 4).ToString("00");

            if (control.ImageIndex != 0)
            {
                control.ImageIndex = 0;
                _dataUpdateViewModel.SetControlsetting(control);
                _sound.SoundPlay($"pull_{randomPullSoundIndex}", false);
            }

            // 鍵位置「駅扱」判定
            if (!IsKeyLeverManual(control))
            {
                // サーバーへリクエスト送信
            }
        }

        /// <summary>
        /// Retsubanマウスダウン処理
        /// </summary>
        /// <param name="control"></param>
        /// <param name="isLeftClick"></param>
        /// <param name="soundIndex"></param>
        private void HandleRetsubanMouseDown(UIControlSetting control, bool isLeftClick)
        {
            if (string.IsNullOrEmpty(control.Retsuban))
            {
                control.Retsuban = "回1234A";
                _dataUpdateViewModel.SetControlsetting(control);
            }
            else if (!string.IsNullOrEmpty(control.Retsuban))
            {
                control.Retsuban = string.Empty;
                _dataUpdateViewModel.SetControlsetting(control);
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

        /// <summary>
        /// 鍵てこ位置が「駅扱」になっているか判定
        /// </summary>
        /// <returns></returns>
        private bool IsKeyLeverManual(UIControlSetting control)
        {
            try
            {
                // 駅扱切換鍵取得
                var keyControl = _dataManager.AllControlSettingList
                    .Where(key => key.StationName == control.StationName && key.UniqueName == control.KeyName)
                    .FirstOrDefault();

                // 「駅扱」位置判定
                if (keyControl.KeyManual)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// 2つの解放てこがどちらも「駅扱」になっているか判定
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private bool IsBothReleaseLeversManual(UIControlSetting control)
        {
            try
            {
                // 駅扱切換鍵取得
                var mainReleaseLever = _dataManager.AllControlSettingList
                    .Where(key => key.StationName == control.StationName && key.UniqueName == control.KeyName)
                    .FirstOrDefault();
                // 方向てこ解放鍵取得
                var subReleaseLever = _dataManager.AllControlSettingList
                    .Where(key => key.StationName == control.LinkedStationName && key.UniqueName == control.LinkedUniqueName)
                    .FirstOrDefault();

                if ((mainReleaseLever != null) && (subReleaseLever != null))
                {
                    // 「駅扱」位置判定
                    if (mainReleaseLever.KeyManual && subReleaseLever.KeyManual)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}