using System.Collections.Generic;
using TatehamaInterlockinglConsole.Manager;
using TatehamaInterlockinglConsole.Models;

namespace TatehamaInterlockinglConsole.ViewModels
{
    /// <summary>
    /// UIControlSettingList更新クラス
    /// </summary>
    public class DataUpdateViewModel : BaseViewModel
    {
        private readonly DataManager _dataManager = DataManager.Instance;

        /// <summary>
        /// タイマー周期毎に呼び出し
        /// </summary>
        public void UpdateTimerEvent()
        {
            // コントロール更新処理
            var updateList = UpdateControlsetting();
        }

        /// <summary>
        /// サーバー情報を基に全コントロールの状態更新
        /// </summary>
        /// <returns></returns>
        private List<UIControlSetting> UpdateControlsetting()
        {
            var allSettingList = _dataManager.AllControlSettingList;
            var settingList = new List<UIControlSetting>();

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
                            // てこTextBlock角度更新
                        }
                        break;
                    case "KeyImage":
                        {
                            // 鍵挿入状態を基に鍵てこ画像更新
                        }
                        break;
                    case "ButtonImage":
                        {
                            // クリック状態を基にボタン画像更新
                        }
                        break;
                    case "Retsuban":
                        {
                            // 列番文字列を基に列番画像更新
                        }
                        break;
                    case "Button":
                    case "Label":
                    case "BackImage":
                    case "ClockImage":
                    case "TextBlock":
                        break;
                    default:
                        break;
                }
            }
            return settingList;
        }
    }
}
