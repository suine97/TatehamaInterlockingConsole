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
                    .FirstOrDefault(t => t.Name == item.ServerName);
                pointA = dataFromServer.Points
                    .FirstOrDefault(p => p.Name == item.PointNameA);
                pointB = dataFromServer.Points
                    .FirstOrDefault(p => p.Name == item.PointNameB);
                signal = dataFromServer.Signals
                    .FirstOrDefault(s => s.Name == item.ServerName);
                lamp = dataFromServer.Lamps
                    .FirstOrDefault(l => l.Name == item.ServerName);
                retsuban = dataFromServer.Retsubans
                    .FirstOrDefault(r => r.Name == item.ServerName);
                lever = dataFromServer.Levers
                    .FirstOrDefault(l => l.Name == item.ServerName);

                // サーバー情報を基に更新
                switch (item.ServerType)
                {
                    case "信号機":
                        if (signal != null)
                        {
                            // 進行信号
                            if (signal.IsProceedSignal)
                                item.ImageIndex = 1;
                            else
                                item.ImageIndex = 0;
                        }
                        break;
                    case "転てつ器":
                        // A, B転てつ器条件が存在する場合
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
                        break;
                    case "軌道回路":
                        if (trackCircuit != null)
                        {
                            // A, B転てつ器条件が存在する場合
                            if (pointA != null && pointB != null)
                            {
                                // 転てつ器状態
                                if ((pointA.IsReversePosition == !item.PointValueA) && (pointB.IsReversePosition == !item.PointValueB))
                                    item.ImageIndex = trackCircuit.IsRouteSetting ? 1 : trackCircuit.IsOnTrack ? 2 : 0;
                                else
                                    item.ImageIndex = 0;
                            }
                            // B転てつ器条件のみ存在する場合
                            else if (pointB != null)
                            {
                                if (pointB.IsReversePosition == !item.PointValueB)
                                    item.ImageIndex = trackCircuit.IsRouteSetting ? 1 : trackCircuit.IsOnTrack ? 2 : 0;
                                else
                                    item.ImageIndex = 0;
                            }
                            // A転てつ器条件のみ存在する場合
                            else if (pointA != null)
                            {
                                if (pointA.IsReversePosition == !item.PointValueA)
                                    item.ImageIndex = trackCircuit.IsRouteSetting ? 1 : trackCircuit.IsOnTrack ? 2 : 0;
                                else
                                    item.ImageIndex = 0;
                            }
                            // 転てつ器条件なし
                            else
                            {
                                if (trackCircuit.IsOnTrack)
                                    item.ImageIndex = trackCircuit.IsRouteSetting ? 1 : trackCircuit.IsOnTrack ? 2 : 0;
                                else
                                    item.ImageIndex = 0;
                            }
                        }
                        break;
                    case "ランプ":
                        if (lamp != null)
                        {
                            // 点灯
                            if (lamp.IsLighting)
                                item.ImageIndex = 1;
                            else
                                item.ImageIndex = 0;
                        }
                        break;
                    case "列車番号":
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
                        break;
                    case "てこ":
                        if (lever != null)
                        {
                            // てこ操作中でなければ更新
                            if (!item.IsLeverhandling)
                            {
                                item.ImageIndex = lever.LeverValue;
                            }
                            // てこ操作中かつ、てこの値がサーバー側と同じなら操作完了判定
                            else if (item.IsLeverhandling && (item.ImageIndex == lever.LeverValue))
                            {
                                item.IsLeverhandling = false;
                            }
                        }
                        break;
                    case "駅扱切換てこ":
                        if (lever != null)
                        {
                            // ランプ"PY"
                            if (item.UniqueName.Contains("PY") && (lever.LeverValue <= 0))
                            {
                                item.ImageIndex = 1;
                            }
                            // ランプ"PG"
                            else if (item.UniqueName.Contains("PG") && (0 < lever.LeverValue))
                            {
                                item.ImageIndex = 1;
                            }
                            else
                            {
                                item.ImageIndex = 0;
                            }
                        }
                        break;
                    case "解放てこ":
                        if (lever != null)
                        {
                            if (lever.LeverValue <= 0)
                            {
                                item.ImageIndex = 1;
                            }
                            else
                            {
                                item.ImageIndex = 0;
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
