using System;
using System.Threading.Tasks;

namespace TatehamaInterlockinglConsole.Models
{
    /// <summary>
    /// 通信クラス
    /// </summary>
    public class Communication
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Communication()
        {

        }

        public event Action<string> DataReceived;

        public async Task StartReceivingAsync()
        {
            // サーバーからデータを受信する（仮実装）
            await Task.Delay(1000);
            string rawData = "{ \"TrackCircuits\": [{ \"Name\": \"TC1\", \"RouteSetting\": true, \"OnTrack\": false }] }";

            // データ受信イベントを発火
            DataReceived?.Invoke(rawData);
        }
    }
}
