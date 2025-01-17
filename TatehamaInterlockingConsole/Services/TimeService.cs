using System;
using System.Windows.Threading;

namespace TatehamaInterlockingConsole.Services
{
    /// <summary>
    /// TimeServiceクラスのインターフェース
    /// </summary>
    public interface ITimeService
    {
        event Action<DateTime> TimeUpdated;
        DateTime CurrentTime { get; }
        void Start();
        void Stop();
        void IncreaseTime();
        void DecreaseTime();
    }

    /// <summary>
    /// 時刻を管理および更新するためのサービスクラス
    /// </summary>
    public class TimeService : ITimeService
    {
        private readonly DispatcherTimer _timer; // 一定間隔で時間を更新するためのタイマー
        private DateTime _baseTime; // 基準となる現在時刻
        private TimeSpan _timeOffset; // 基準時刻からのオフセット値

        /// <summary>
        /// 時刻が更新されたときに発生するイベント
        /// </summary>
        public event Action<DateTime> TimeUpdated;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TimeService()
        {
            _baseTime = DateTime.Now; // 現在時刻を基準時刻として設定
            _timeOffset = TimeSpan.FromHours(-10); // デフォルトのオフセット値を設定
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) }; // タイマーの間隔を設定

            // タイマーのTickイベントに時刻更新処理を登録
            _timer.Tick += (sender, e) =>
            {
                _baseTime = DateTime.Now; // 基準時刻を現在時刻に更新
                TimeUpdated?.Invoke(CurrentTime); // イベントをトリガーして現在時刻を通知
            };
        }

        /// <summary>
        /// 現在の基準時刻にオフセットを加えた時刻を取得
        /// </summary>
        public DateTime CurrentTime => _baseTime.Add(_timeOffset);

        /// <summary>
        /// タイマー開始
        /// </summary>
        public void Start() => _timer.Start();

        /// <summary>
        /// タイマー停止
        /// </summary>
        public void Stop() => _timer.Stop();

        /// <summary>
        /// 時刻のオフセットを1時間進める
        /// </summary>
        public void IncreaseTime()
        {
            _timeOffset = _timeOffset.Add(TimeSpan.FromHours(1)); // オフセットを増加
            TimeUpdated?.Invoke(CurrentTime); // 更新された時刻を通知
        }

        /// <summary>
        /// 時刻のオフセットを1時間戻す
        /// </summary>
        public void DecreaseTime()
        {
            _timeOffset = _timeOffset.Subtract(TimeSpan.FromHours(1)); // オフセットを減少
            TimeUpdated?.Invoke(CurrentTime); // 更新された時刻を通知
        }
    }
}
