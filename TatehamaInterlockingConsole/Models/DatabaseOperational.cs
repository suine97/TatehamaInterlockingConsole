using System;
using System.Collections.Generic;
using System.Linq;

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
            public List<TrackCircuitData> TrackCircuits { get; set; }

            /// <summary>
            /// 転てつ器情報リスト
            /// </summary>
            public List<SwitchData> Points { get; set; }

            /// <summary>
            /// 信号機情報リスト
            /// </summary>
            public List<SignalData> Signals { get; set; }

            /// <summary>
            /// 物理てこ情報リスト
            /// </summary>
            public List<LeverData> PhysicalLevers { get; set; }

            /// <summary>
            /// 着点ボタン情報リスト
            /// </summary>
            public List<DestinationButtonData> PhysicalButtons { get; set; }

            /// <summary>
            /// 方向てこ情報リスト
            /// </summary>
            public List<DirectionData> Directions { get; set; }

            /// <summary>
            /// 列番情報リスト
            /// </summary>
            public List<RetsubanData> Retsubans { get; set; }

            /// <summary>
            /// 表示灯情報リスト
            /// </summary>
            public List<Dictionary<string, bool>> Lamps { get; set; }

            /// <summary>
            /// 差分データを取得する
            /// </summary>
            /// <param name="oldData">前回のデータ</param>
            /// <returns>変更があったデータのみを含む新しいDataFromServer</returns>
            public DataFromServer GetDifferences(DataFromServer oldData)
            {
                return new DataFromServer
                {
                    Authentications = Authentications,
                    TrackCircuits = GetListDifferences(TrackCircuits, oldData.TrackCircuits),
                    Points = GetListDifferences(Points, oldData.Points),
                    Signals = GetListDifferences(Signals, oldData.Signals),
                    PhysicalLevers = GetListDifferences(PhysicalLevers, oldData.PhysicalLevers),
                    PhysicalButtons = GetListDifferences(PhysicalButtons, oldData.PhysicalButtons),
                    Directions = GetListDifferences(Directions, oldData.Directions),
                    Retsubans = GetListDifferences(Retsubans, oldData.Retsubans),
                    Lamps = GetListDifferences(Lamps, oldData.Lamps)
                };
            }

            /// <summary>
            /// リストの差分を取得する
            /// </summary>
            /// <typeparam name="T">リストの型</typeparam>
            /// <param name="newList">新しいデータ</param>
            /// <param name="oldList">古いデータ</param>
            /// <returns>変更があったリスト（変更がない場合は空のリスト）</returns>
            private List<T> GetListDifferences<T>(List<T> newList, List<T> oldList)
            {
                if (newList == null) return new List<T>();
                if (oldList == null) return new List<T>(newList);

                // 差分を取得
                var differences = newList.Except(oldList).ToList();

                return differences;
            }
        }

        /// <summary>
        /// 認証データクラス
        /// </summary>
        public class TraincrewRole : IEquatable<TraincrewRole>
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

            public bool Equals(TraincrewRole other)
            {
                if (other == null) return false;
                return IsDriver == other.IsDriver &&
                       IsDriverManager == other.IsDriverManager &&
                       IsConductor == other.IsConductor &&
                       IsCommander == other.IsCommander &&
                       IsSignalman == other.IsSignalman &&
                       IsAdministrator == other.IsAdministrator;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as TraincrewRole);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(IsDriver, IsDriverManager, IsConductor, IsCommander, IsSignalman, IsAdministrator);
            }
        }

        /// <summary>
        /// 方向てこデータクラス
        /// </summary>
        public class DirectionData : IEquatable<DirectionData>
        {
            /// <summary>
            /// 方向てこ名称
            /// </summary>
            public string Name { get; set; } = "";
            /// <summary>
            /// 方向てこの値
            /// </summary>
            public EnumData.LNR State { get; set; } = EnumData.LNR.Left;

            public bool Equals(DirectionData other)
            {
                if (other == null) return false;
                return Name == other.Name && State == other.State;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as DirectionData);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, State);
            }
        }

        /// <summary>
        /// 軌道回路データクラス
        /// </summary>
        public class TrackCircuitData : IEquatable<TrackCircuitData>
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

            public bool Equals(TrackCircuitData other)
            {
                if (other == null) return false;
                return On == other.On && Lock == other.Lock && Last == other.Last && Name == other.Name;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as TrackCircuitData);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(On, Lock, Last, Name);
            }
        }

        /// <summary>
        /// 転てつ器データクラス
        /// </summary>
        public class SwitchData : IEquatable<SwitchData>
        {
            /// <summary>
            /// 転てつ器状態
            /// </summary>
            public EnumData.NRC State { get; set; } = EnumData.NRC.Center;
            /// <summary>
            /// 転てつ器名称
            /// </summary>
            public string Name { get; set; } = "";

            public bool Equals(SwitchData other)
            {
                if (other == null) return false;
                return State == other.State && Name == other.Name;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as SwitchData);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(State, Name);
            }
        }

        /// <summary>
        /// 信号機データクラス
        /// </summary>
        public class SignalData : IEquatable<SignalData>
        {
            /// <summary>
            /// 信号機名称
            /// </summary>
            public string Name { get; init; } = "";
            /// <summary>
            /// 信号機現示
            /// </summary>
            public EnumData.Phase Phase { get; init; } = EnumData.Phase.None;

            public bool Equals(SignalData other)
            {
                if (other == null) return false;
                return Name == other.Name && Phase == other.Phase;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as SignalData);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, Phase);
            }
        }

        /// <summary>
        /// 列番データクラス
        /// </summary>
        public class RetsubanData : IEquatable<RetsubanData>
        {
            /// <summary>
            /// 列番名称
            /// </summary>
            public string Name { get; set; } = "";
            /// <summary>
            /// 列番
            /// </summary>
            public string Retsuban { get; set; } = "";

            public bool Equals(RetsubanData other)
            {
                if (other == null) return false;
                return Name == other.Name && Retsuban == other.Retsuban;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as RetsubanData);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, Retsuban);
            }
        }

        /// <summary>
        /// 物理てこデータクラス
        /// </summary>
        public class LeverData : IEquatable<LeverData>
        {
            /// <summary>
            /// 物理てこ名称
            /// </summary>
            public string Name { get; set; } = "";
            /// <summary>
            /// 物理てこの状態
            /// </summary>
            public EnumData.LCR State { get; set; } = EnumData.LCR.Center;

            public bool Equals(LeverData other)
            {
                if (other == null) return false;
                return Name == other.Name && State == other.State;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as LeverData);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, State);
            }
        }

        /// <summary>
        /// 着点ボタンデータクラス
        /// </summary>
        public class DestinationButtonData : IEquatable<DestinationButtonData>
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

            public bool Equals(DestinationButtonData other)
            {
                if (other == null) return false;
                return Name == other.Name && IsRaised == other.IsRaised && OperatedAt == other.OperatedAt;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as DestinationButtonData);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, IsRaised, OperatedAt);
            }
        }
    }
}
