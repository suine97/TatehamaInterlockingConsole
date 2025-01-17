using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using TatehamaInterlockingConsole.Manager;

namespace TatehamaInterlockingConsole.Models
{
    /// <summary>
    /// サーバー通信クラス
    /// </summary>
    public static class ServerCommunication
    {
        /// <summary>
        /// サーバーへリクエスト送信
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task SendRequestAsync(string token, CancellationToken cancellationToken)
        {
            try
            {
                // HubConnectionの作成
                await using var client = new HubConnectionBuilder()
                    .WithUrl($"{ServerAddress.SignalAddress}/hub/train?access_token={token}")
                    .WithAutomaticReconnect() // 自動再接続
                    .Build();

                // 接続開始
                await client.StartAsync(cancellationToken);

                // サーバーメソッドの呼び出し
                var resource = await client.InvokeAsync<int>("Emit", cancellationToken);
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Server send failed: {exception.Message}");
                MessageBox.Show($"Server send failed:\n\n{exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
