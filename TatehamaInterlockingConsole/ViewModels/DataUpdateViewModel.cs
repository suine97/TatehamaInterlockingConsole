using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
        private readonly DataManager _dataManager;  // データ管理を担当するクラス
        private readonly Sound _sound;              // 音声再生クラス
        private readonly Random _random = new();    // 乱数生成クラス
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
            _sound = Sound.Instance;
        }

        /// <summary>
        /// サーバー受信毎にコントロール更新
        /// </summary>
        public void UpdateControl(DatabaseOperational.DataFromServer dataFromServer)
        {
            // コントロール更新処理
            var updateList = UpdateControlsetting(dataFromServer);

            // 接近警報更新処理
            UpdateApproachingAlarm(dataFromServer);

            // 変更通知イベント発火
            var handler = NotifyUpdateControlEvent;
            handler?.Invoke(updateList);
        }

        /// <summary>
        /// サーバー情報を基にコントロールの状態更新
        /// </summary>
        /// <returns></returns>
        private List<UIControlSetting> UpdateControlsetting(DatabaseOperational.DataFromServer dataFromServer)
        {
            var allSettingList = new List<UIControlSetting>(_dataManager.AllControlSettingList);
            var trackCircuit = new DatabaseOperational.TrackCircuitData();
            var pointA = new DatabaseOperational.SwitchData();
            var pointB = new DatabaseOperational.SwitchData();
            var signal = new DatabaseOperational.SignalData();
            var physicalLever = new DatabaseOperational.LeverData();
            var physicalButton = new DatabaseOperational.DestinationButtonData { Name = string.Empty };
            var direction = new DatabaseOperational.DirectionData();
            var retsuban = new DatabaseOperational.RetsubanData();
            var lamp = new Dictionary<string, bool>();

            try
            {
                // 全コントロール設定データ更新
                foreach (var item in allSettingList)
                {
                    // コントロールと一致するサーバー情報のみ抽出
                    trackCircuit = dataFromServer.TrackCircuits
                        .FirstOrDefault(t => t.Name == item.ServerName);
                    pointA = dataFromServer.Points
                        .FirstOrDefault(p => p.Name == item.PointNameA);
                    pointB = dataFromServer.Points
                        .FirstOrDefault(p => p.Name == item.PointNameB);
                    signal = dataFromServer.Signals
                        .FirstOrDefault(s => s.Name == item.ServerName);
                    physicalLever = dataFromServer.PhysicalLeverDataList
                        .FirstOrDefault(l => l.Name == item.ServerName);
                    physicalButton = dataFromServer.PhysicalButtonDataList
                        .FirstOrDefault(b => b.Name == item.ServerName);
                    direction = dataFromServer.DirectionLevers
                        .FirstOrDefault(l => l.Name == item.ServerName);
                    retsuban = dataFromServer.Retsubans
                        .FirstOrDefault(r => r.Name == item.ServerName);
                    lamp = dataFromServer.Lamps
                        .FirstOrDefault(l => l.ContainsKey(item.ServerName));

                    // NRC型の転てつ器状態取得
                    EnumData.NRC pointAState = item.PointValueA ? EnumData.NRC.Normal : EnumData.NRC.Reversed;
                    EnumData.NRC pointBState = item.PointValueB ? EnumData.NRC.Normal : EnumData.NRC.Reversed;

                    // 音声再生用の乱数生成
                    string randomKeyInsertSoundIndex = _random.Next(1, 6).ToString("00");
                    string randomKeyChainSoundIndex = _random.Next(1, 10).ToString("00");
                    string randomKeyRemoveSoundIndex = _random.Next(1, 6).ToString("00");
                    string randomKeyRejectSoundIndex = _random.Next(1, 4).ToString("00");
                    string randomSwitchSoundIndex = _random.Next(1, 9).ToString("00");
                    string randomDropSoundIndex = _random.Next(1, 13).ToString("00");
                    string randomRaiseSoundIndex = _random.Next(1, 13).ToString("00");

                    // サーバー分類毎に処理
                    switch (item.ServerType)
                    {
                        case "信号機表示灯":
                            if (signal != null)
                            {
                                // 進行信号
                                if (signal.Phase != EnumData.Phase.R)
                                    item.ImageIndex = 1;
                                else
                                    item.ImageIndex = 0;
                            }
                            break;
                        case "転てつ器表示灯":
                            // A, B転てつ器条件が存在する場合
                            if (pointA != null && pointB != null)
                            {
                                // 転てつ器状態
                                if ((pointA.State == pointAState) && (pointB.State == pointBState))
                                    item.ImageIndex = 1;
                                else
                                    item.ImageIndex = 0;
                            }
                            // B転てつ器条件のみ存在する場合
                            else if (pointB != null)
                            {
                                if (pointB.State == pointBState)
                                    item.ImageIndex = 1;
                                else
                                    item.ImageIndex = 0;
                            }
                            // A転てつ器条件のみ存在する場合
                            else if (pointA != null)
                            {
                                if (pointA.State == pointAState)
                                    item.ImageIndex = 1;
                                else
                                    item.ImageIndex = 0;
                            }
                            break;
                        case "軌道回路表示灯":
                            if (trackCircuit != null)
                            {
                                // A, B転てつ器条件が存在する場合
                                if (pointA != null && pointB != null)
                                {
                                    // 転てつ器状態
                                    if ((pointA.State == pointAState) && (pointB.State == pointBState))
                                        item.ImageIndex = trackCircuit.Lock ? 1 : trackCircuit.On ? 2 : 0;
                                    else
                                        item.ImageIndex = 0;
                                }
                                // B転てつ器条件のみ存在する場合
                                else if (pointB != null)
                                {
                                    if (pointB.State == pointBState)
                                        item.ImageIndex = trackCircuit.Lock ? 1 : trackCircuit.On ? 2 : 0;
                                    else
                                        item.ImageIndex = 0;
                                }
                                // A転てつ器条件のみ存在する場合
                                else if (pointA != null)
                                {
                                    if (pointA.State == pointAState)
                                        item.ImageIndex = trackCircuit.Lock ? 1 : trackCircuit.On ? 2 : 0;
                                    else
                                        item.ImageIndex = 0;
                                }
                                // 転てつ器条件なし
                                else
                                {
                                    if (trackCircuit.On)
                                        item.ImageIndex = trackCircuit.Lock ? 1 : trackCircuit.On ? 2 : 0;
                                    else
                                        item.ImageIndex = 0;
                                }
                            }
                            break;
                        case "状態表示灯":
                            if (lamp != null)
                            {
                                // 点灯
                                if (lamp.TryGetValue(item.ServerName, out bool lampValue))
                                    item.ImageIndex = 1;
                                else
                                    item.ImageIndex = 0;
                            }
                            break;
                        case "駅扱切換表示灯":
                            if (physicalLever != null)
                            {
                                // ランプ"PY"
                                if (item.UniqueName.Contains("PY") && (physicalLever.State == EnumData.LCR.Left))
                                    item.ImageIndex = 1;
                                // ランプ"PG"
                                else if (item.UniqueName.Contains("PG") && (physicalLever.State == EnumData.LCR.Right))
                                    item.ImageIndex = 1;
                                else
                                    item.ImageIndex = 0;
                            }
                            break;
                        case "解放表示灯":
                            if (physicalLever != null)
                            {
                                if (physicalLever.State == EnumData.LCR.Right)
                                    item.ImageIndex = 1;
                                else
                                    item.ImageIndex = 0;
                            }
                            break;
                        case "物理てこ":
                            if (physicalLever != null)
                            {
                                // 物理てこの状態がUIと異なる場合に更新
                                if (physicalLever.State != EnumData.ConvertToLCR(item.ImageIndex))
                                {
                                    item.ImageIndex = EnumData.ConvertFromLCR(physicalLever.State);

                                    // 音声再生
                                    _sound.SoundPlay($"switch_{randomSwitchSoundIndex}", false);
                                }
                            }
                            break;
                        case "物理鍵てこ":
                            if (physicalLever != null)
                            {
                                // 物理鍵てこの状態がUIと異なる場合に更新
                                if (physicalLever.State != EnumData.ConvertToLCR(item.ImageIndex))
                                {
                                    item.ImageIndex = EnumData.ConvertFromLCR(physicalLever.State);

                                    // Todo: 鍵音声処理追加

                                    // 音声再生
                                    _sound.SoundPlay($"switch_{randomSwitchSoundIndex}", false);
                                }
                            }
                            break;
                        case "着点ボタン":
                            if (physicalButton != null)
                            {
                                // 着点ボタンの状態がUIと異なる場合に更新
                                if (physicalButton.IsRaised != EnumData.ConvertToRaiseDrop(item.ImageIndex))
                                {
                                    item.ImageIndex = EnumData.ConvertFromRaiseDrop(physicalButton.IsRaised);

                                    // 音声再生
                                    if (item.ImageIndex == 1)
                                        _sound.SoundPlay($"drop_{randomDropSoundIndex}", false);
                                    else
                                        _sound.SoundPlay($"raise_{randomRaiseSoundIndex}", false);
                                }
                            }
                            break;
                        case "列車番号":
                            if (retsuban != null)
                            {
                                // 列車番号
                                item.Retsuban = retsuban.Retsuban;
                                SetControlsetting(item);
                            }
                            else
                            {
                                item.Retsuban = string.Empty;
                                SetControlsetting(item);
                            }
                            break;
                        default:
                            break;
                    }
                }
                return allSettingList;
            }
            catch (Exception ex)
            {
                CustomMessage.Show(ex.ToString(), "エラー");
                throw ex;
            }
        }

        /// <summary>
        /// 接近警報更新
        /// </summary>
        private void UpdateApproachingAlarm(DatabaseOperational.DataFromServer dataFromServer)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                CustomMessage.Show(ex.ToString(), "エラー");
                throw ex;
            }
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
    }
}
