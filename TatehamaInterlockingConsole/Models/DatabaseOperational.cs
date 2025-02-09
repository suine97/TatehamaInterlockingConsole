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
            /// 起動しているウィンドウの駅名
            /// </summary>
            public List<string> ActiveStationsList { get; set; }
        }

        /// <summary>
        /// 物理てこイベント送信用データクラス
        /// </summary>
        public class LeverEventDataToServer
        {
            /// <summary>
            /// 物理てこ名称
            /// </summary>
            public string LeverName { get; set; }
            /// <summary>
            /// 物理てこデータ
            /// </summary>
            public LeverData LeverData { get; set; }
        }

        /// <summary>
        /// 着点ボタンイベント送信用データクラス
        /// </summary>
        public class ButtonEventDataToServer
        {
            /// <summary>
            /// 着点ボタン名称
            /// </summary>
            public string ButtonName { get; set; }
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
            public TraincrewRole Authentications { get; set; }

            [JsonProperty("trackCircuitList")]
            public List<TrackCircuitData> TrackCircuits { get; set; }

            public List<SwitchData> Points { get; set; }

            [JsonProperty("signalDataList")]
            public List<SignalData> Signals { get; set; } 

            public List<PhysicalUIData> PhysicalUIs { get; set; }

            public List<DirectionLeverData> DirectionLevers { get; set; }

            public List<RetsubanData> Retsubans { get; set; }

            public List<Dictionary<string, bool>> Lamps { get; set; }
        }

        /// <summary>
        /// 認証情報クラス
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
        /// 物理てこ・着点ボタン情報クラス
        /// </summary>
        public class PhysicalUIData
        {
            /// <summary>
            /// 物理てこ・ボタン名称
            /// </summary>
            public string Name { get; set; } = "";
            /// <summary>
            /// 物理てこデータ
            /// </summary>
            public LeverData LeverData { get; set; }
            /// <summary>
            /// 着点ボタンデータ
            /// </summary>
            public DestinationButtonData DestinationButtonData { get; set; }
        }

        /// <summary>
        /// 方向てこ情報クラス
        /// </summary>
        public class DirectionLeverData
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
        /// 軌道回路情報クラス
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
        /// 転てつ器情報クラス
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
        /// 信号機情報クラス
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
        /// 列番情報クラス
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
        /// 表示灯情報データ
        /// Key=表示灯名称, Value=表示灯点灯状態
        /// </summary>
        public Dictionary<string, bool> LampDic { get; set; } = new();

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
            public required string Name { get; init; }
            /// <summary>
            /// 着点ボタンの状態
            /// </summary>
            public EnumData.RaiseDrop IsRaised { get; set; }
            /// <summary>
            /// 着点ボタンの操作時間
            /// </summary>
            public DateTime OperatedAt { get; set; }
        }
    }
}
