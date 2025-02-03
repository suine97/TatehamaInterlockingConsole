namespace TatehamaInterlockingConsole.Models
{
    /// <summary>
    /// 接近警報設定クラス
    /// </summary>
    public class ApproachingAlarmSetting
    {
        /// <summary>
        /// [ ]で囲まれた駅名
        /// </summary>
        public string OtherStationNameA { get; set; }
        /// <summary>
        /// [[ ]]で囲まれた駅名
        /// </summary>
        public string OtherStationNameB { get; set; }
        /// <summary>
        /// 上り鳴動条件
        /// </summary>
        public string UpSideCondition { get; set; }
        /// <summary>
        /// 下り鳴動条件
        /// </summary>
        public string DownSideCondition { get; set; }
        /// <summary>
        /// 上り接近警報音声ファイル名
        /// </summary>
        public string UpSideApproachingAlarmName { get; set; }
        /// <summary>
        /// 下り接近警報音声ファイル名
        /// </summary>
        public string DownSideApproachingAlarmName { get; set; }
        /// <summary>
        /// 方向てこ音声ファイル名
        /// </summary>
        public string DirectionLeverAlarmName { get; set; }
    }
}
