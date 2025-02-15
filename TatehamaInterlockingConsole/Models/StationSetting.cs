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
        public string UpSideAlarmName { get; set; }
        /// <summary>
        /// 上り接近警報音声タイプ
        /// </summary>
        public string UpSideAlarmType { get; set; }
        /// <summary>
        /// 下り接近警報音声ファイル名
        /// </summary>
        public string DownSideAlarmName { get; set; }
        /// <summary>
        /// 下り接近警報音声タイプ
        /// </summary>
        public string DownSideAlarmType { get; set; }
        /// <summary>
        /// 方向てこ警報音声ファイル名
        /// </summary>
        public string DirectionAlarmName { get; set; }
        /// <summary>
        /// 方向てこ警報音声タイプ
        /// </summary>
        public string DirectionAlarmType { get; set; }
    }
}
