using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

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
        public static async Task SendRequestAsync(HubConnection client)
        {
            try
            {
                // サーバーメソッドの呼び出し
                var resource = await client.InvokeAsync<int>("Emit");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Server send failed: {exception.Message}");
            }
        }
    }
}
