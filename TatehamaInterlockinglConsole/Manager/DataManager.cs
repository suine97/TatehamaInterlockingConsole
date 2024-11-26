using System;
using System.Collections.Generic;
using TatehamaInterlockinglConsole.Factories;
using TatehamaInterlockinglConsole.Models;
using TatehamaInterlockinglConsole.Services;

namespace TatehamaInterlockinglConsole.Manager
{
    public class DataManager
    {
        private static DataManager _instance = new DataManager();
        private TimeService _timeService;
        public event Action<DateTime> TimeUpdated;

        public static DataManager Instance => _instance;
        public DateTime CurrentTime => _timeService?.CurrentTime ?? DateTime.MinValue;

        /// <summary>
        /// 全コントロール設定データ
        /// </summary>
        public List<UIControlSetting> AllControlSettingList { get; set; }

        private DataManager()
        {
            AllControlSettingList = new List<UIControlSetting>();
        }

        public void Initialize(TimeService timeService)
        {
            _timeService = timeService;
            _timeService.TimeUpdated += (currentTime) => OnTimeUpdated();
            TimeUpdated += (currentTime) => ClockImageFactory.CurrentTime = currentTime;
        }

        private void OnTimeUpdated()
        {
            TimeUpdated?.Invoke(CurrentTime);
        }
    }
}
