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
        public static DataManager Instance => _instance;
        public DateTime CurrentTime => _timeService?.CurrentTime ?? DateTime.MinValue;
        public Dictionary<string, List<UIControlSetting>> AllTsvDictionary { get; set; }
        private TimeService _timeService;
        public event Action<DateTime> TimeUpdated;

        private DataManager()
        {
            AllTsvDictionary = new Dictionary<string, List<UIControlSetting>>();
        }

        public void Initialize(TimeService timeService)
        {
            _timeService = timeService;
            _timeService.TimeUpdated += (currentTime) => OnTimeUpdated();
            TimeUpdated += (currentTime) => ControlFactory.CurrentTime = currentTime;
        }

        private void OnTimeUpdated()
        {
            TimeUpdated?.Invoke(CurrentTime);
        }
    }
}
