using System;
using System.Collections.Generic;
using System.Windows;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole.ViewModels
{
    /// <summary>
    /// UIControlSettingList更新クラス
    /// </summary>
    public class DataUpdateViewModel : BaseViewModel
    {
        private static readonly DataUpdateViewModel _instance = new DataUpdateViewModel();
        private readonly DataManager _dataManager; // データ管理を担当するクラス

        public static DataUpdateViewModel Instance => _instance;

        /// <summary>
        /// 変更通知イベント
        /// </summary>
        public event Action<List<UIControlSetting>> NotifyUpdateControlEvent;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataUpdateViewModel()
        {
            _dataManager = DataManager.Instance;
        }

        /// <summary>
        /// タイマー周期毎に呼び出し
        /// </summary>
        public void UpdateTimerEvent()
        {
            // コントロール更新処理
            var updateList = UpdateControlsetting();

            // 変更通知イベント発火
            var handler = NotifyUpdateControlEvent;
            handler?.Invoke(updateList);
        }

        /// <summary>
        /// 指定したUIControlSettingをコントロールに反映
        /// </summary>
        /// <param name="setting"></param>
        public void SetControlsetting(UIControlSetting setting)
        {
            try
            {
                var allSettingList = new List<UIControlSetting>(_dataManager.AllControlSettingList);

                int index = allSettingList.FindIndex(list => list.StationName == setting.StationName && list.UniqueName == setting.UniqueName);
                if (index >= 0)
                {
                    allSettingList[index].ImageIndex = setting.ImageIndex;
                    allSettingList[index].Retsuban = setting.Retsuban;

                    // 変更通知イベント発火
                    var handler = NotifyUpdateControlEvent;
                    handler?.Invoke(allSettingList);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// サーバー情報を基にコントロールの状態更新
        /// </summary>
        /// <returns></returns>
        private List<UIControlSetting> UpdateControlsetting()
        {
            var allSettingList = new List<UIControlSetting>(_dataManager.AllControlSettingList);

            foreach (var item in allSettingList)
            {
                switch (item.ControlType)
                {
                    case "Image":
                        {
                            // 軌道回路状態・転てつ器開通方向を基に在線ランプ画像更新
                        }
                        break;
                    case "LeverImage":
                        {
                            // 転てつ器状態を基にてこ画像更新
                            switch (item.ImagePatternSymbol)
                            {
                                case "NR":
                                    break;
                                case "LN":
                                    break;
                                case "LR":
                                    break;
                                case "LNR":
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case "KeyImage":
                        {
                            // 鍵挿入状態を基に鍵てこ画像更新
                            switch (item.ImagePatternSymbol)
                            {
                                case "KeyNR":
                                    break;
                                case "KeyLN":
                                    break;
                                case "KeyLR":
                                    break;
                                case "KeyLNR":
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case "Retsuban":
                        {
                            // 列番文字列を基に列番画像更新
                        }
                        break;
                    default:
                        break;
                }
            }
            return allSettingList;
        }
    }
}
