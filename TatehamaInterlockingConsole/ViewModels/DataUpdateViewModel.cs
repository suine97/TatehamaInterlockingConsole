using System;
using System.Collections.Generic;
using System.Linq;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Models;
using TatehamaInterlockingConsole.Services;

namespace TatehamaInterlockingConsole.ViewModels
{
    /// <summary>
    /// UIControlSettingList更新クラス
    /// </summary>
    public class DataUpdateViewModel : BaseViewModel
    {
        private static readonly DataUpdateViewModel _instance = new();
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
        /// サーバー受信毎にコントロール更新
        /// </summary>
        public void UpdateControl(DatabaseOperational.DataFromServer dataFromServer)
        {
            // コントロール更新処理
            var updateList = UpdateControlsetting(dataFromServer);

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
                    // コントロール画像更新
                    allSettingList[index].ImageIndex = setting.ImageIndex;
                    allSettingList[index].Retsuban = setting.Retsuban;

                    // 変更通知イベント発火
                    var handler = NotifyUpdateControlEvent;
                    handler?.Invoke(allSettingList);
                }
            }
            catch (Exception ex)
            {
                CustomMessage.Show(ex.ToString(), "エラー");
                throw ex;
            }
        }

        /// <summary>
        /// サーバー情報を基にコントロールの状態更新
        /// </summary>
        /// <returns></returns>
        private List<UIControlSetting> UpdateControlsetting(DatabaseOperational.DataFromServer dataFromServer)
        {
            var allSettingList = new List<UIControlSetting>(_dataManager.AllControlSettingList);
            var signal = new DatabaseOperational.InterlockingSignal();
            var pointA = new DatabaseOperational.InterlockingPoint();
            var pointB = new DatabaseOperational.InterlockingPoint();
            var trackCircuit = new DatabaseOperational.InterlockingTrackCircuit();
            var lamp = new DatabaseOperational.InterlockingLamp();
            var retsuban = new DatabaseOperational.InterlockingRetsuban();
            var lever = new DatabaseOperational.InterlockingLever();

            foreach (var item in allSettingList)
            {
                trackCircuit = dataFromServer.TrackCircuits
                    .Where(t => t.Name == item.ServerName)
                    .FirstOrDefault();
                pointA = dataFromServer.Points
                    .Where(p => p.Name == item.PointNameA)
                    .FirstOrDefault();
                pointB = dataFromServer.Points
                    .Where(p => p.Name == item.PointNameB)
                    .FirstOrDefault();
                signal = dataFromServer.Signals
                    .Where(s => s.Name == item.ServerName)
                    .FirstOrDefault();
                lamp = dataFromServer.Lamps
                    .Where(l => l.Name == item.ServerName)
                    .FirstOrDefault();
                retsuban = dataFromServer.Retsubans
                    .Where(r => r.Name == item.ServerName)
                    .FirstOrDefault();
                lever = dataFromServer.Levers
                    .Where(l => l.Name == item.ServerName)
                    .FirstOrDefault();

                // サーバー情報を基に更新
                switch (item.ServerType)
                {
                    case "信号機":
                        {
                            if (signal != null)
                            {
                                // 進行信号
                                if (signal.IsProceedSignal)
                                    item.ImageIndex = 1;
                                else
                                    item.ImageIndex = 0;
                            }
                        }
                        break;
                    case "転てつ器":
                        {
                            // A,B転てつ器条件が存在する場合
                            if (pointA != null && pointB != null)
                            {
                                // 転てつ器状態
                                if ((pointA.IsReversePosition == !item.PointValueA) && (pointB.IsReversePosition == !item.PointValueB))
                                    item.ImageIndex = 1;
                                else
                                    item.ImageIndex = 0;
                            }
                            // B転てつ器条件のみ存在する場合
                            else if (pointB != null)
                            {
                                if (pointB.IsReversePosition == !item.PointValueB)
                                    item.ImageIndex = 1;
                                else
                                    item.ImageIndex = 0;
                            }
                            // A転てつ器条件のみ存在する場合
                            else if (pointA != null)
                            {
                                if (pointA.IsReversePosition == !item.PointValueA)
                                    item.ImageIndex = 1;
                                else
                                    item.ImageIndex = 0;
                            }
                        }
                        break;
                    case "軌道回路":
                        {
                            if (trackCircuit != null)
                            {
                                // A,B転てつ器条件が存在する場合
                                if (pointA != null && pointB != null)
                                {
                                    // 転てつ器状態
                                    if ((pointA.IsReversePosition == !item.PointValueA) && (pointB.IsReversePosition == !item.PointValueB))
                                        item.ImageIndex = trackCircuit.IsOnTrack ? 1 : trackCircuit.IsRouteSetting ? 2 : 0;
                                    else
                                        item.ImageIndex = 0;
                                }
                                // B転てつ器条件のみ存在する場合
                                else if (pointB != null)
                                {
                                    if (pointB.IsReversePosition == !item.PointValueB)
                                        item.ImageIndex = trackCircuit.IsOnTrack ? 1 : trackCircuit.IsRouteSetting ? 2 : 0;
                                    else
                                        item.ImageIndex = 0;
                                }
                                // A転てつ器条件のみ存在する場合
                                else if (pointA != null)
                                {
                                    if (pointA.IsReversePosition == !item.PointValueA)
                                        item.ImageIndex = trackCircuit.IsOnTrack ? 1 : trackCircuit.IsRouteSetting ? 2 : 0;
                                    else
                                        item.ImageIndex = 0;
                                }
                                // 転てつ器条件なし
                                else
                                {
                                    if (trackCircuit.IsOnTrack)
                                        item.ImageIndex = trackCircuit.IsOnTrack ? 1 : trackCircuit.IsRouteSetting ? 2 : 0;
                                    else
                                        item.ImageIndex = 0;
                                }
                            }
                        }
                        break;
                    case "ランプ":
                        {
                            if (lamp != null)
                            {
                                // 点灯
                                if (lamp.IsLighting)
                                    item.ImageIndex = 1;
                                else
                                    item.ImageIndex = 0;
                            }
                        }
                        break;
                    case "列車番号":
                        {
                            if (retsuban != null)
                            {
                                // 列車番号
                                item.Retsuban = retsuban.RetsubanText;
                                SetControlsetting(item);
                            }
                            else
                            {
                                item.Retsuban = string.Empty;
                                SetControlsetting(item);
                            }
                        }
                        break;
                    default:
                        break;
                }

                // 信号盤操作を基に更新
                switch (item.ControlType)
                {
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
                    default:
                        break;
                }
            }
            return allSettingList;
        }
    }
}
