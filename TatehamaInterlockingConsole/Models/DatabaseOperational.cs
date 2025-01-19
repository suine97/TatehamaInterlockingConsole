using System.Collections.Generic;

namespace TatehamaInterlockingConsole.Models
{
    /// <summary>
    /// 運用サーバーデータ格納クラス
    /// </summary>
    public class DatabaseOperational
    {
        private static readonly DatabaseOperational _instance = new();
        public static DatabaseOperational Instance => _instance;

        /// <summary>
        /// 連動装置・送信用コマンド
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
            private InterlockingAuthentication _authentication;

            public InterlockingAuthentication Authentication
            {
                get => _authentication;
                set
                {
                    if (_authentication != null)
                    {
                        throw new System.InvalidOperationException("Authentication is already set. Only one instance is allowed.");
                    }
                    _authentication = value;
                }
            }
            public List<InterlockingTrackCircuit> TrackCircuits { get; set; } = [];
            public List<InterlockingPoint> Points { get; set; } = [];
            public List<InterlockingSignal> Signals { get; set; } = [];
            public List<InterlockingLamp> Lamps { get; set; } = [];
            public List<InterlockingRetsuban> Retsuban { get; set; } = [];
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
