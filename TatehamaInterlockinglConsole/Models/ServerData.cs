using System.Collections.Generic;

namespace TatehamaInterlockinglConsole.Models
{
    /// <summary>
    /// 連動装置サーバーデータ格納クラス
    /// </summary>
    public class ServerData
    {
        /// <summary>
        /// 連動装置・コマンド
        /// </summary>
        public class CommandToServer
        {
            public string Command { get; set; }
            public string[] Args { get; set; }
        }

        /// <summary>
        /// 連動装置データクラス
        /// </summary>
        public class InterlockingData
        {
            public List<InterlockingTrackCircuit> TrackCircuits { get; set; } = new List<InterlockingTrackCircuit>();
            public List<InterlockingPoint> Points { get; set; } = new List<InterlockingPoint>();
            public List<InterlockingSignal> Signals { get; set; } = new List<InterlockingSignal>();
            public List<InterlockingLamp> Lamps { get; set; } = new List<InterlockingLamp>();
        }

        /// <summary>
        /// 連動装置・軌道回路情報クラス
        /// </summary>
        public class InterlockingTrackCircuit
        {
            public string Name { get; set; }
            public bool RouteSetting { get; set; }
            public bool OnTrack { get; set; }
        }

        /// <summary>
        /// 連動装置・転てつ器情報クラス
        /// </summary>
        public class InterlockingPoint
        {
            public string Name { get; set; }
            public bool NormalPosition { get; set; }
        }

        /// <summary>
        /// 連動装置・信号機情報クラス
        /// </summary>
        public class InterlockingSignal
        {
            public string Name { get; set; }
            public bool StopSignal { get; set; }
        }

        /// <summary>
        /// 連動装置・ランプ情報クラス
        /// </summary>
        public class InterlockingLamp
        {
            public string Name { get; set; }
            public bool Value { get; set; }
        }
    }
}
