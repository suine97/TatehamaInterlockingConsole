namespace TatehamaInterlockingConsole.Models
{
    public class StationSetting
    {
        /// <summary>
        /// 日本語駅名
        /// </summary>
        public string StationName { get; set; }
        /// <summary>
        /// 駅番号
        /// </summary>
        public string StationNumber { get; set; }
        /// <summary>
        /// 駅設定ファイル名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 表示用駅名
        /// </summary>
        public string ViewName { get; set; }
        /// <summary>
        /// 上り接近警報音声ファイル名
        /// </summary>
        public string UpSideApproachingAlarmName { get; set; }
        /// <summary>
        /// 下り接近警報音声ファイル名
        /// </summary>
        public string DownSideApproachingAlarmName { get; set; }
        /// <summary>
        /// 方向てこ警報音声ファイル名
        /// </summary>
        public string DirectionLeverAlarmName { get; set; }
    }
}
