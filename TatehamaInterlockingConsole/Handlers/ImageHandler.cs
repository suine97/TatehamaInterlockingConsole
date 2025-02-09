using System;
using System.Windows.Controls;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole.Handlers
{
    public class ImageHandler
    {
        private readonly DataManager _dataManager = DataManager.Instance;
        private readonly ServerCommunication _serverCommunication;
        public static ImageHandler Instance { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="serverCommunication"></param>
        public ImageHandler(ServerCommunication serverCommunication)
        {
            _serverCommunication = serverCommunication;
            Instance = this;
        }

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
                default:
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
        private void HandleLeverImageMouseDown(UIControlSetting control, bool isLeftClick)
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

            // サーバーへリクエスト送信
            if (control.ServerType != string.Empty)
            {
                var dataToServer = new DatabaseOperational.LeverEventDataToServer
                {
                    LeverData = new DatabaseOperational.LeverData
                    {
                        Name = control.ServerName,
                        State = EnumData.ConvertToLCR(control.ImageIndex)
                    }
                };
                _ = _serverCommunication.SendLeverEventDataRequestToServerAsync(dataToServer);
                //CustomMessage.Show($"Name: {dataToServer.LeverData.Name} State: {dataToServer.LeverData.State}",
                //    "サーバー送信",
                //    System.Windows.MessageBoxButton.OK,
                //    System.Windows.MessageBoxImage.Information
                //    );
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
            // Shiftキーが押されているかを判定
            bool isShiftPressed = (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Shift) == System.Windows.Input.ModifierKeys.Shift;
            // 司令主任権限
            bool isAdministrator = _dataManager.Authentication?.IsAdministrator ?? false;

            // 司令主任権限がある場合のみ処理
            if (isAdministrator)
            {
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
                    }
                    control.KeyInserted = !control.KeyInserted;

                    // サーバーへリクエスト送信
                    if (control.ServerType != string.Empty)
                    {
                        var dataToServer = new DatabaseOperational.LeverEventDataToServer
                        {
                            LeverData = new DatabaseOperational.LeverData
                            {
                                Name = control.ServerName,
                                State = EnumData.ConvertToLCR(control.ImageIndex)
                            }
                        };
                        _ = _serverCommunication.SendLeverEventDataRequestToServerAsync(dataToServer);
                        //CustomMessage.Show($"Name: {dataToServer.LeverData.Name} State: {dataToServer.LeverData.State}",
                        //    "サーバー送信",
                        //    System.Windows.MessageBoxButton.OK,
                        //    System.Windows.MessageBoxImage.Information
                        //    );
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

                    // サーバーへリクエスト送信
                    if (control.ServerType != string.Empty)
                    {
                        var dataToServer = new DatabaseOperational.LeverEventDataToServer
                        {
                            LeverData = new DatabaseOperational.LeverData
                            {
                                Name = control.ServerName,
                                State = EnumData.ConvertToLCR(control.ImageIndex)
                            }
                        };
                        _ = _serverCommunication.SendLeverEventDataRequestToServerAsync(dataToServer);
                        //CustomMessage.Show($"Name: {dataToServer.LeverData.Name} State: {dataToServer.LeverData.State}",
                        //    "サーバー送信",
                        //    System.Windows.MessageBoxButton.OK,
                        //    System.Windows.MessageBoxImage.Information
                        //    );
                    }
                }
            }
            // 司令主任権限がない場合は操作無効
            else
            {

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

                // サーバーへリクエスト送信
                if (control.ServerType != string.Empty)
                {
                    var dataToServer = new DatabaseOperational.ButtonEventDataToServer
                    {
                        DestinationButtonData = new DatabaseOperational.DestinationButtonData
                        {
                            Name = control.ServerName,
                            IsRaised = EnumData.ConvertToRaiseDrop(control.ImageIndex),
                            OperatedAt = DateTime.Now
                        }
                    };
                    _ = _serverCommunication.SendButtonEventDataRequestToServerAsync(dataToServer);
                    //CustomMessage.Show($"Name: {dataToServer.DestinationButtonData.Name} State: {dataToServer.DestinationButtonData.IsRaised}",
                    //    "サーバー送信",
                    //    System.Windows.MessageBoxButton.OK,
                    //    System.Windows.MessageBoxImage.Information
                    //    );
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
            if (control.ImageIndex != 0)
            {
                control.ImageIndex = 0;

                // サーバーへリクエスト送信
                if (control.ServerType != string.Empty)
                {
                    var dataToServer = new DatabaseOperational.ButtonEventDataToServer
                    {
                        DestinationButtonData = new DatabaseOperational.DestinationButtonData
                        {
                            Name = control.ServerName,
                            IsRaised = EnumData.ConvertToRaiseDrop(control.ImageIndex),
                            OperatedAt = DateTime.Now
                        }
                    };
                    _ = _serverCommunication.SendButtonEventDataRequestToServerAsync(dataToServer);
                    //CustomMessage.Show($"Name: {dataToServer.DestinationButtonData.Name} State: {dataToServer.DestinationButtonData.IsRaised}",
                    //    "サーバー送信",
                    //    System.Windows.MessageBoxButton.OK,
                    //    System.Windows.MessageBoxImage.Information
                    //    );
                }
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