using System.Collections.Generic;
using TatehamaInterlockinglConsole.Manager;
using TatehamaInterlockinglConsole.Models;

namespace TatehamaInterlockinglConsole.ViewModels
{
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
                            // 軌道回路状態から在線ランプ画像更新
                            // 転てつ器開通方向が指定されていたらその条件を判定
                        }
                        break;
                    case "LeverImage":
                        {
                            // 転てつ器状態からてこ画像更新
                            // てこTextBox角度更新
                        }
                        break;
                    case "KeyImage":
                        {

                        }
                        break;
                    case "ButtonImage":
                        {

                        }
                        break;
                    case "Retsuban":
                        {

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
