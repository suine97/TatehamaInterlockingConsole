using System;
using System.Windows.Media;

namespace TatehamaInterlockingConsole.Services
{
    public static class Clock
    {
        /// <summary>
        /// 時計コントロール設定メソッド
        /// </summary>
        /// <param name="time"></param>
        /// <param name="hourRotate"></param>
        /// <param name="minuteRotate"></param>
        /// <param name="secondRotate"></param>
        public static void SetClockHands(DateTime time, RotateTransform hourRotate, RotateTransform minuteRotate, RotateTransform secondRotate)
        {
            // 短針、長針、秒針の角度を計算
            double hourAngle = (time.Hour % 12) * 30 + time.Minute * 0.5; // 1時間あたり30度 + 分による微調整
            double minuteAngle = time.Minute * 6 + time.Second * 0.1; // 1分あたり6度 + 秒による微調整
            double secondAngle = time.Second * 6; // 1秒あたり6度

            // 角度を設定
            hourRotate.Angle = hourAngle;
            minuteRotate.Angle = minuteAngle;
            secondRotate.Angle = secondAngle;
        }
    }
}
