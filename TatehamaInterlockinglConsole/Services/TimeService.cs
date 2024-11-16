using System;
using System.Windows.Threading;

namespace TatehamaInterlockinglConsole.Services
{
    public class TimeService
    {
        private readonly DispatcherTimer _timer;
        private DateTime _baseTime;
        private TimeSpan _timeOffset;

        public event Action<DateTime> TimeUpdated;

        public TimeService()
        {
            _baseTime = DateTime.Now;
            _timeOffset = TimeSpan.FromHours(-10);
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (sender, e) =>
            {
                _baseTime = DateTime.Now;
                TimeUpdated?.Invoke(CurrentTime);
            };
        }

        public DateTime CurrentTime => _baseTime.Add(_timeOffset);

        public void Start() => _timer.Start();

        public void Stop() => _timer.Stop();

        public void IncreaseTime()
        {
            _timeOffset = _timeOffset.Add(TimeSpan.FromHours(1));
            TimeUpdated?.Invoke(CurrentTime);
        }

        public void DecreaseTime()
        {
            _timeOffset = _timeOffset.Subtract(TimeSpan.FromHours(1));
            TimeUpdated?.Invoke(CurrentTime);
        }
    }
}
