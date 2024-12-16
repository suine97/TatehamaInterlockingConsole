using System;
using System.Collections.Generic;
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
        private static readonly DataManager _instance = new DataManager();
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
        /// 信号操作管理者判定
        /// </summary>
        public bool Administrator { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private DataManager()
        {
            AllControlSettingList = new List<UIControlSetting>();
            RetsubanImagePathDictionary = new Dictionary<string, string>();
            StationNameDictionary  = new Dictionary<string, List<string>>();
            Administrator = false;
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
        /// 時刻データ変更通知イベント
        /// </summary>
        private void OnTimeUpdated()
        {
            TimeUpdated?.Invoke(CurrentTime);
        }
    }
}
