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

        /// <summary>
        /// 現在時刻
        /// </summary>
        public DateTime CurrentTime => _timeService?.CurrentTime ?? DateTime.MinValue;

        /// <summary>
        /// 音量
        /// </summary>
        public double Volume { get; set; }

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
        /// 接近警報鳴動条件辞書データ
        /// </summary>
        public List<List<ApproachingAlarmSetting>> ApproachingAlarmConditionList { get; set; }

        /// <summary>
        /// サーバー接続状態
        /// </summary>
        public bool ServerConnected { get; set; }

        private DatabaseOperational.DataFromServer _dataFromServer;
        /// <summary>
        /// サーバー受信データ
        /// </summary>
        public DatabaseOperational.DataFromServer DataFromServer
        {
            get => _dataFromServer;
            set
            {
                _dataFromServer = value;
            }
        }

        private DatabaseOperational.InterlockingAuthentication _authentication;
        /// <summary>
        /// サーバー受信データ(認証情報)
        /// </summary>
        public DatabaseOperational.InterlockingAuthentication Authentication
        {
            get => _authentication;
            set
            {
                _authentication = value;
            }
        }

        /// <summary>
        /// ActiveStationsList変更通知イベント
        /// </summary>
        public event Action<List<string>> ActiveStationsListChanged;

        private List<string> _activeStationsList;
        /// <summary>
        /// 起動しているウィンドウの駅名を保持するリスト
        /// </summary>
        public List<string> ActiveStationsList
        {
            get => _activeStationsList;
            set
            {
                if (_activeStationsList != value)
                {
                    _activeStationsList = value;
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private DataManager()
        {
            AllControlSettingList = new();
            RetsubanImagePathDictionary = new();
            StationNameDictionary = new();
            DataFromServer = new();
            ActiveStationsList = new();
            ApproachingAlarmConditionList = new();
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
        /// 駅名を追加するメソッド
        /// </summary>
        /// <param name="stationName"></param>
        public void AddActiveStation(string stationName)
        {
            if (!_activeStationsList.Contains(stationName))
            {
                _activeStationsList.Add(stationName);
                ActiveStationsListChanged?.Invoke(_activeStationsList);
            }
        }

        /// <summary>
        /// 駅名を削除するメソッド
        /// </summary>
        /// <param name="stationName"></param>
        public void RemoveActiveStation(string stationName)
        {
            if (_activeStationsList.Contains(stationName))
            {
                _activeStationsList.Remove(stationName);
                ActiveStationsListChanged?.Invoke(_activeStationsList);
            }
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
