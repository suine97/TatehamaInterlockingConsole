using System.Collections.Generic;

namespace TatehamaInterlockingConsole.Models
{
    public static class SendStationData
    {
        /// <summary>
        /// サーバー送信用の駅データリストを生成する
        /// </summary>
        /// <param name="activeStationsList">現在アクティブな駅リスト</param>
        /// <returns>送信する駅データリスト</returns>
        public static List<string> GenerateSendStationData(List<string> activeStationsList)
        {
            List<string> sendStationData = new(activeStationsList);

            // 江ノ原検車区(TH66S)を開いている場合、江ノ原(TH66)を追加
            if (sendStationData.Contains("TH66S"))
            {
                sendStationData.Add("TH66");
            }
            return sendStationData;
        }
    }
}
