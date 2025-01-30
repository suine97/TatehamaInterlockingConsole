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

        public class RootObject
        {
            public AuthenticationList AuthenticationList { get; set; }

            [JsonProperty("trackCircuitList")]
            public List<TrackCircuitList> TrackCircuitList { get; set; }

            public List<PointList> PointList { get; set; }

            [JsonProperty("signalDataList")]
            public List<SignalDataList> SignalDataList { get; set; }

            public List<LampList> LampList { get; set; }

            public List<RetsubanList> RetsubanList { get; set; }

            public List<LeverList> LeverList { get; set; }
        }

        public class AuthenticationList
        {

        }

        public class TrackCircuitList
        {
            [JsonProperty("On")]
            public bool On { get; set; }
            [JsonProperty("Last")]
            public string Last { get; set; } // 軌道回路を踏んだ列車の名前
            [JsonProperty("Name")]
            public string Name { get; set; }
        }

        public class PointList
        {
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

        public class LampList
        {
            [JsonProperty("Name")]
            public string Name { get; set; }
        }

        public class RetsubanList
        {
            [JsonProperty("Name")]
            public string Name { get; set; }
        }

        public class LeverList
        {
            [JsonProperty("Name")]
            public string Name { get; set; }
        }
    }
}
