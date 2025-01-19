using Newtonsoft.Json;
using System.Collections.Generic;

namespace TatehamaInterlockingConsole.Models
{
    /// <summary>
    /// 一時保存用サーバーデータ格納クラス
    /// </summary>
    public class DatabaseTemporary
    {
        private static readonly DatabaseTemporary _instance = new();
        public static DatabaseTemporary Instance => _instance;

        public class TrackCircuitList
        {
            [JsonProperty("On")]
            public bool On { get; set; }
            [JsonProperty("Last")]
            public string Last { get; set; } // 軌道回路を踏んだ列車の名前
            [JsonProperty("Name")]
            public string Name { get; set; }
        }

        public class SignalDataList
        {
            [JsonProperty("Name")]
            public string Name { get; set; }
            [JsonProperty("phase")]
            public int Phase { get; set; }
        }

        public class RootObject
        {
            [JsonProperty("trackCircuitList")]
            public List<TrackCircuitList> TrackCircuitList { get; set; }

            [JsonProperty("otherTrainDataList")]
            public List<object> OtherTrainDataList { get; set; }

            [JsonProperty("signalDataList")]
            public List<SignalDataList> SignalDataList { get; set; }
        }
    }
}
