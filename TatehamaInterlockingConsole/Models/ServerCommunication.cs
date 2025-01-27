using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using TatehamaInterlockingConsole.Manager;

namespace TatehamaInterlockingConsole.Models
{
    /// <summary>
    /// サーバー通信クラス
    /// </summary>
    public class ServerCommunication
    {
        private string _token;
        private readonly OpenIddictClientService _openIddictClientService;
        private readonly DatabaseOperational _databaseOperational;
        private static HubConnection _connection;
        private static bool _isUpdateLoopRunning = false;

        /// <summary>
        /// サーバー接続状態
        /// </summary>
        public static bool Connected { get; set; } = false;

        /// <summary>
        /// サーバー接続状態変更イベント
        /// </summary>
        public event Action<bool> ConnectionStatusChanged;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServerCommunication(OpenIddictClientService openIddictClientService)
        {
            _openIddictClientService = openIddictClientService;
            _databaseOperational = DatabaseOperational.Instance;

            if (!_isUpdateLoopRunning)
            {
                _isUpdateLoopRunning = true;

                // ループ処理開始
                Task.Run(() => UpdateLoop());
            }
        }

        /// <summary>
        /// ループ処理
        /// </summary>
        /// <returns></returns>
        private async Task UpdateLoop()
        {
            while (true)
            {
                var timer = Task.Delay(100);
                await timer;

                ConnectionStatusChanged?.Invoke(Connected);

                if (!Connected)
                {
                    continue;
                }
                try
                {
                    //await SendRequestAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during server request: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// ユーザー認証
        /// </summary>
        /// <returns></returns>
        public async Task AuthenticateAsync()
        {
            try
            {
                using var source = new CancellationTokenSource(TimeSpan.FromSeconds(90));

                // ブラウザで認証要求
                var result = await _openIddictClientService.ChallengeInteractivelyAsync(new()
                {
                    CancellationToken = source.Token
                });

                // 認証完了まで待機
                var resultAuth = await _openIddictClientService.AuthenticateInteractivelyAsync(new()
                {
                    CancellationToken = source.Token,
                    Nonce = result.Nonce
                });

                // 認証成功(トークン取得)
                _token = resultAuth.BackchannelAccessToken;

                // サーバー接続初期化
                await InitializeConnection();
            }
            catch (OpenIddictExceptions.ProtocolException exception)
                when (exception.Error is OpenIddictConstants.Errors.AccessDenied)
            {
                // 認証拒否(サーバーに入ってないとか、ロールがついてないetc...)
                MessageBox.Show($"認証が拒否されました。\n司令主任に連絡してください。", "認証拒否 | 連動盤 - ダイヤ運転会");
            }
            catch (Exception ex)
            {
                // その他別な理由で認証失敗
                var result = MessageBox.Show($"認証に失敗しました。\n再認証しますか？\n\n{ex.Message}\n{ex.StackTrace})", "認証失敗 | 連動盤 - ダイヤ運転会", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (result == MessageBoxResult.Yes)
                {
                    _ = AuthenticateAsync();
                }
            }
        }

        /// <summary>
        /// サーバー接続初期化
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task InitializeConnection()
        {
            // HubConnectionの作成
            _connection = new HubConnectionBuilder()
                .WithUrl($"{ServerAddress.SignalAddress}/hub/train?access_token={_token}")
                .WithAutomaticReconnect() // 自動再接続
                .Build();

            while (!Connected)
            {
                try
                {
                    await _connection.StartAsync();
                    Console.WriteLine("Connected");
                    Connected = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection Error!! {ex.Message}");
                    Connected = false;
                }
            }

            // 再接続イベントのハンドリング
            _connection.Reconnecting += exception =>
            {
                Connected = false;
                Console.WriteLine("Reconnecting");
                return Task.CompletedTask;
            };

            _connection.Reconnected += exeption =>
            {
                Connected = true;
                Console.WriteLine("Connected");
                return Task.CompletedTask;
            };
            await Task.Delay(Timeout.Infinite);
        }

        /// <summary>
        /// サーバーへリクエスト送信
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task SendRequestAsync()
        {
            try
            {
                // サーバーメソッドの呼び出し
                var jsonMessage = await _connection.InvokeAsync<string>("SendData_Interlocking", "");

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
