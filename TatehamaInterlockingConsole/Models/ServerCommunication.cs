using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

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
                var jsonMessage = await client.InvokeAsync<string>("SendData_Interlocking");

                try
                {
                    // JSONをDatabaseTemporary.RootObjectにデシリアライズ
                    var data = JsonConvert.DeserializeObject<DatabaseTemporary.RootObject>(jsonMessage);
                    if (data != null)
                    {
                        Console.WriteLine("Data successfully deserialized:");
                        Console.WriteLine($"Track Circuits: {data.TrackCircuitList.Count}");
                        Console.WriteLine($"Signals: {data.SignalDataList.Count}");
                    }
                    else
                    {
                        Console.WriteLine("Failed to deserialize JSON.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during JSON deserialization: {ex.Message}");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Server send failed: {exception.Message}");
            }
        }
    }
}
