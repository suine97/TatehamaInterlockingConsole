using Newtonsoft.Json;
using System;
using System.Diagnostics;
using TatehamaInterlockinglConsole.Models;

namespace TatehamaInterlockinglConsole.ViewModels
{
    public class InterlockingViewModel
    {
        public InterlockingData interlockingData { get; private set; } = new InterlockingData();

        private readonly Communication _communication;

        public InterlockingViewModel()
        {
            _communication = new Communication();
            _communication.DataReceived += OnDataReceived;
        }

        /// <summary>
        /// データ受信時の処理
        /// </summary>
        /// <param name="rawData">サーバーから受信したJSONデータ</param>
        private void OnDataReceived(string rawData)
        {
            try
            {
                // JSONデータをInterlockingDataにデシリアライズ
            }
            catch (JsonException ex)
            {
                // JSONパースエラー時の処理
                Debug.WriteLine($"JSONデシリアライズに失敗しました: {ex.Message}");
            }
            catch (Exception ex)
            {
                // その他のエラー
                Debug.WriteLine($"予期しないエラーが発生しました: {ex.Message}");
            }
        }

        /// <summary>
        /// 通信処理を開始
        /// </summary>
        public void StartCommunication()
        {
            _ = _communication.StartReceivingAsync();
        }
    }
}
