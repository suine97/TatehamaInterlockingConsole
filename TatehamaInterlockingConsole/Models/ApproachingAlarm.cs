using System.Collections.Generic;

namespace TatehamaInterlockingConsole.Models
{
    /// <summary>
    /// 接近警報設定クラス
    /// </summary>
    public class ApproachingAlarmSetting
    {
        /// <summary>
        /// 自駅名
        /// </summary>
        public string StationName { get; set; }
        /// <summary>
        /// [ ]で囲まれた駅名
        /// </summary>
        public string OtherStationNameA { get; set; }
        /// <summary>
        /// [[ ]]で囲まれた駅名
        /// </summary>
        public string OtherStationNameB { get; set; }
        /// <summary>
        /// 方向
        /// </summary>
        public bool IsUpSide { get; set; }
        /// <summary>
        /// 軌道回路名称
        /// </summary>
        public ApproachingAlarmType TrackName { get; set; }
        /// <summary>
        /// 転てつ器・各種てこ・軌道回路条件リスト
        /// </summary>
        public List<ApproachingAlarmType> ConditionsList { get; set; }

        /// <summary>
        /// 接近警報の鳴動条件が成立したかどうか
        /// </summary>
        public bool IsAlarmConditionMet { get; set; }
    }

    /// <summary>
    /// 接近警報分類クラス
    /// </summary>
    public class ApproachingAlarmType
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分類種別
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 所属駅名
        /// </summary>
        public string Station { get; set; }
        /// <summary>
        /// 反位判定
        /// </summary>
        public bool IsReversePosition { get; set; }
    }
}
