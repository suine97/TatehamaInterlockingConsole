using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TatehamaInterlockingConsole.Models
{
    /// <summary>
    /// サーバーデータ格納クラス
    /// </summary>
    public class DatabaseOperational
    {
        private static readonly DatabaseOperational _instance = new();
        public static DatabaseOperational Instance => _instance;

        /// <summary>
        /// 常時送信用データクラス
        /// </summary>
        public class ConstantDataToServer
        {
            /// <summary>
            /// 常時送信用駅データリスト
            /// </summary>
            public List<string> ActiveStationsList { get; set; }
        }

        /// <summary>
        /// イベント送信用データクラス(物理てこ)
        /// </summary>
        public class LeverEventDataToServer
        {
            /// <summary>
            /// 物理てこデータ
            /// </summary>
            public LeverData LeverData { get; set; }
        }

        /// <summary>
        /// イベント送信用データクラス(着点ボタン)
        /// </summary>
        public class ButtonEventDataToServer
        {
            /// <summary>
            /// 着点ボタンデータ
            /// </summary>
            public DestinationButtonData DestinationButtonData { get; set; }
        }

        /// <summary>
        /// 受信用データクラス
        /// </summary>
        public class DataFromServer
        {
            /// <summary>
            /// 認証情報リスト
            /// </summary>
            public TraincrewRole Authentications { get; set; }

            /// <summary>
            /// 軌道回路情報リスト
            /// </summary>
            [JsonProperty("trackCircuitList")]
            public List<TrackCircuitData> TrackCircuits { get; set; }

            /// <summary>
            /// 転てつ器情報リスト
            /// </summary>
            public List<SwitchData> Points { get; set; }

            /// <summary>
            /// 信号機情報リスト
            /// </summary>
            [JsonProperty("signalDataList")]
            public List<SignalData> Signals { get; set; }

            /// <summary>
            /// 物理てこ情報リスト
            /// </summary>
            public List<LeverData> PhysicalLeverDataList { get; set; }

            /// <summary>
            /// 着点ボタン情報リスト
            /// </summary>
            public List<DestinationButtonData> PhysicalButtonDataList { get; set; }

            /// <summary>
            /// 方向てこ情報リスト
            /// </summary>
            public List<DirectionData> DirectionLevers { get; set; }

            /// <summary>
            /// 列番情報リスト
            /// </summary>
            public List<RetsubanData> Retsubans { get; set; }

            /// <summary>
            /// 表示灯情報リスト
            /// </summary>
            public List<Dictionary<string, bool>> Lamps { get; set; }
        }

        /// <summary>
        /// 認証データクラス
        /// </summary>
        public class TraincrewRole
        {
            /// <summary>
            /// 運転士
            /// </summary>
            public required bool IsDriver { get; init; } = false;
            /// <summary>
            /// 乗務助役
            /// </summary>
            public required bool IsDriverManager { get; init; } = false;
            /// <summary>
            /// 車掌
            /// </summary>
            public required bool IsConductor { get; init; } = false;
            /// <summary>
            /// 司令員
            /// </summary>
            public required bool IsCommander { get; init; } = false;
            /// <summary>
            /// 信号扱者
            /// </summary>
            public required bool IsSignalman { get; init; } = false;
            /// <summary>
            /// 司令主任
            /// </summary>
            public required bool IsAdministrator { get; init; } = false;
        }

        /// <summary>
        /// 方向てこデータクラス
        /// </summary>
        public class DirectionData
        {
            /// <summary>
            /// 方向てこ名称
            /// </summary>
            public string Name { get; set; } = "";
            /// <summary>
            /// 方向てこの値
            /// </summary>
            public EnumData.LNR State { get; set; } = EnumData.LNR.Left;
        }

        /// <summary>
        /// 軌道回路データクラス
        /// </summary>
        public class TrackCircuitData
        {
            /// <summary>
            /// 在線状態    
            /// </summary>
            public bool On { get; set; } = false;
            /// <summary>
            /// 鎖錠状態
            /// </summary>
            public bool Lock { get; set; } = false;
            /// <summary>
            /// 軌道回路を踏んだ列車の名前
            /// </summary>
            public string Last { get; set; } = null;
            /// <summary>
            /// 軌道回路名称
            /// </summary>
            public string Name { get; set; } = "";

            public override string ToString()
            {
                return $"{Name}";
            }
        }

        /// <summary>
        /// 転てつ器データクラス
        /// </summary>
        public class SwitchData
        {
            /// <summary>
            /// 転てつ器状態
            /// </summary>
            public EnumData.NRC State { get; set; } = EnumData.NRC.Center;
            /// <summary>
            /// 転てつ器名称
            /// </summary>
            public string Name { get; set; } = "";
        }

        /// <summary>
        /// 信号機データクラス
        /// </summary>
        public class SignalData
        {
            /// <summary>
            /// 信号機名称
            /// </summary>
            public string Name { get; init; } = "";
            /// <summary>
            /// 信号機現示
            /// </summary>
            public EnumData.Phase Phase { get; init; } = EnumData.Phase.None;
        }

        /// <summary>
        /// 列番データクラス
        /// </summary>
        public class RetsubanData
        {
            /// <summary>
            /// 列番名称
            /// </summary>
            public string Name { get; set; } = "";
            /// <summary>
            /// 列番
            /// </summary>
            public string Retsuban { get; set; } = "";
        }

        /// <summary>
        /// 物理てこデータクラス
        /// </summary>
        public class LeverData
        {
            /// <summary>
            /// 物理てこ名称
            /// </summary>
            public string Name { get; set; } = "";
            /// <summary>
            /// 物理てこの状態
            /// </summary>
            public EnumData.LCR State { get; set; } = EnumData.LCR.Center;
        }

        /// <summary>
        /// 着点ボタンデータクラス
        /// </summary>
        public class DestinationButtonData
        {
            /// <summary>
            /// 着点ボタン名称
            /// </summary>
            public required string Name { get; init; } = "";
            /// <summary>
            /// 着点ボタンの状態
            /// </summary>
            public EnumData.RaiseDrop IsRaised { get; set; } = EnumData.RaiseDrop.Raise;
            /// <summary>
            /// 着点ボタンの操作時間
            /// </summary>
            public DateTime OperatedAt { get; set; } = DateTime.MinValue;
        }
    }
}
