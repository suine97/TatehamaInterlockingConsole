﻿using System.Collections.Generic;

namespace TatehamaInterlockingConsole.Models
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
            public List<InterlockingAuthentication> Authentications { get; set; } = new List<InterlockingAuthentication>();
            public List<InterlockingTrackCircuit> TrackCircuits { get; set; } = new List<InterlockingTrackCircuit>();
            public List<InterlockingPoint> Points { get; set; } = new List<InterlockingPoint>();
            public List<InterlockingSignal> Signals { get; set; } = new List<InterlockingSignal>();
            public List<InterlockingLamp> Lamps { get; set; } = new List<InterlockingLamp>();
            public List<InterlockingRetsuban> Retsuban { get; set; } = new List<InterlockingRetsuban>();
        }

        /// <summary>
        /// 連動装置・認証情報クラス
        /// </summary>
        public class InterlockingAuthentication
        {
            /// <summary>
            /// 操作権限ユーザー判定
            /// </summary>
            public bool IsOperableUser { get; set; }
            /// <summary>
            /// 認証トークン
            /// </summary>
            public string Token { get; set; }
        }

        /// <summary>
        /// 連動装置・軌道回路情報クラス
        /// </summary>
        public class InterlockingTrackCircuit
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 鎖錠判定
            /// </summary>
            public bool IsRouteSetting { get; set; }
            /// <summary>
            /// 在線判定
            /// </summary>
            public bool IsOnTrack { get; set; }
        }

        /// <summary>
        /// 連動装置・転てつ器情報クラス
        /// </summary>
        public class InterlockingPoint
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 反位判定
            /// </summary>
            public bool IsReversePosition { get; set; }
        }

        /// <summary>
        /// 連動装置・信号機情報クラス
        /// </summary>
        public class InterlockingSignal
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 進行信号判定
            /// </summary>
            public bool IsProceedSignal { get; set; }
        }

        /// <summary>
        /// 連動装置・ランプ情報クラス
        /// </summary>
        public class InterlockingLamp
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 点灯判定
            /// </summary>
            public bool IsLighting { get; set; }
        }

        /// <summary>
        /// 連動装置・列番情報クラス
        /// </summary>
        public class InterlockingRetsuban
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 列車番号情報
            /// </summary>
            public string RetsubanText { get; set; }
        }
    }
}
