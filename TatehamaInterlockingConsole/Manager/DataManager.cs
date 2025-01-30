using System;
using System.Collections.Generic;
using System.Linq;
using TatehamaInterlockingConsole.Factories;
using TatehamaInterlockingConsole.Models;
using TatehamaInterlockingConsole.Services;

namespace TatehamaInterlockingConsole.Manager
{
    /// <summary>
    /// GlobalData管理クラス
    /// </summary>
    public class DataManager
    {
        private static readonly DataManager _instance = new();
        private TimeService _timeService;
        public event Action<DateTime> TimeUpdated;

        public static DataManager Instance => _instance;
        public DateTime CurrentTime => _timeService?.CurrentTime ?? DateTime.MinValue;

        /// <summary>
        /// 全コントロール設定データ
        /// </summary>
        public List<UIControlSetting> AllControlSettingList { get; set; }

        /// <summary>
        /// 列番表示画像Pathを格納した辞書データ
        /// </summary>
        public Dictionary<string, string> RetsubanImagePathDictionary { get; set; }

        /// <summary>
        /// 駅名対照表辞書データ
        /// </summary>
        public Dictionary<string, List<string>> StationNameDictionary { get; set; }

        /// <summary>
        /// サーバー接続状態
        /// </summary>
        public bool ServerConnected { get; set; }

        /// <summary>
        /// 起動しているウィンドウの駅名を保持するリスト
        /// </summary>
        public List<string> ActiveStationsList { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private DataManager()
        {
            AllControlSettingList = new();
            RetsubanImagePathDictionary = new();
            StationNameDictionary  = new();
            ActiveStationsList = new();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="timeService"></param>
        public void Initialize(TimeService timeService)
        {
            _timeService = timeService;
            _timeService.TimeUpdated += (currentTime) => OnTimeUpdated();
            TimeUpdated += (currentTime) => ClockImageFactory.CurrentTime = currentTime;
        }

        /// <summary>
        /// サーバー受信データを運用データに代入
        /// </summary>
        /// <param name="data"></param>
        public DatabaseOperational.DataFromServer UpdateDataFromServer(DatabaseTemporary.RootObject data)
        {
            try
            {
                var updatedDataFromServer = new DatabaseOperational.DataFromServer
                {
                    TrackCircuits = data.TrackCircuitList.Select(temp => new DatabaseOperational.InterlockingTrackCircuit
                    {
                        Name = temp.Name,
                        IsRouteSetting = false,
                        IsOnTrack = false,
                    }).ToList(),

                    Signals = data.SignalDataList.Select(temp => new DatabaseOperational.InterlockingSignal
                    {
                        Name = temp.Name,
                        IsProceedSignal = false,
                    }).ToList(),

                    Points = data.PointList.Select(temp => new DatabaseOperational.InterlockingPoint
                    {
                        Name = temp.Name,
                        IsReversePosition = false,
                    }).ToList(),

                    Lamps = data.LampList.Select(temp => new DatabaseOperational.InterlockingLamp
                    {
                        Name = temp.Name,
                        IsLighting = false,
                    }).ToList(),

                    Retsubans = data.RetsubanList.Select(temp => new DatabaseOperational.InterlockingRetsuban
                    {
                        Name = temp.Name,
                        RetsubanText = "",
                    }).ToList(),

                    Levers = data.LeverList.Select(temp => new DatabaseOperational.InterlockingLever
                    {
                        Name = temp.Name,
                        LeverValue = 0,
                    }).ToList()
                };

                return updatedDataFromServer;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 時刻データ変更通知イベント
        /// </summary>
        private void OnTimeUpdated()
        {
            TimeUpdated?.Invoke(CurrentTime);
        }
    }
}
