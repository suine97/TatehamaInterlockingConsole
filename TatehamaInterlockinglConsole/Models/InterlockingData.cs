using System.Collections.Generic;

namespace TatehamaInterlockinglConsole.Models
{
    /// <summary>
    /// 連動装置データクラス
    /// </summary>
    public class InterlockingData
    {
        public List<InterlockingTrackCircuit> TrackCircuits { get; set; } = new List<InterlockingTrackCircuit>();
        public List<InterlockingTurnout> Turnouts { get; set; } = new List<InterlockingTurnout>();
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
    /// 連動装置・転轍機情報クラス
    /// </summary>
    public class InterlockingTurnout
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
