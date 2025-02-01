using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Services;
using TatehamaInterlockingConsole.ViewModels;

namespace TatehamaInterlockingConsole.Models
{
    /// <summary>
    /// サーバー通信クラス
    /// </summary>
    public class ServerCommunication
    {
        private string _token;
        private readonly OpenIddictClientService _openIddictClientService;
        private readonly DataManager _dataManager;
        private static HubConnection _connection;
        private static bool _isUpdateLoopRunning = false;

        /// <summary>
        /// サーバー接続状態変更イベント
        /// </summary>
        public event Action<bool> ConnectionStatusChanged;

        /// <summary>
        /// サーバー接続中の駅名リスト
        /// </summary>
        private List<string> _connectionStationsList;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ServerCommunication(OpenIddictClientService openIddictClientService)
        {
            _openIddictClientService = openIddictClientService;
            _dataManager = DataManager.Instance;

            if (!_isUpdateLoopRunning)
            {
                _isUpdateLoopRunning = true;

                _connectionStationsList = new();
                _dataManager.ActiveStationsListChanged += OnActiveStationsListChanged;

                // ループ処理開始
                Task.Run(() => UpdateLoop());
            }
        }

        /// <summary>
        /// 駅名リスト変更イベント
        /// </summary>
        /// <param name="newActiveStationsList"></param>
        private void OnActiveStationsListChanged(List<string> newActiveStationsList)
        {
            var addedStations = newActiveStationsList.Except(_connectionStationsList).ToList();
            var removedStations = _connectionStationsList.Except(newActiveStationsList).ToList();

            foreach (var station in addedStations)
            {
                // 増加した駅名の処理
                var dataToServer = new DatabaseOperational.DataToServer
                {
                    ActiveStationsList = new List<string> { station },
                };
                _ = SendRequestAsync(dataToServer);
            }

            foreach (var station in removedStations)
            {
                // 減少した駅名の処理
                var dataToServer = new DatabaseOperational.DataToServer
                {
                    ActiveStationsList = new List<string> { station },
                };
                _ = SendRequestAsync(dataToServer);
            }
            _connectionStationsList = newActiveStationsList.ToList();
        }

        /// <summary>
        /// ループ処理
        /// </summary>
        /// <returns></returns>
        private async Task UpdateLoop()
        {
            while (true)
            {
                var timer = Task.Delay(1000);
                await timer;

                // サーバー接続状態変更イベント発火
                ConnectionStatusChanged?.Invoke(_dataManager.ServerConnected);
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
                CustomMessage.Show("認証が拒否されました。\n司令主任に連絡してください。", "認証拒否", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                // その他別な理由で認証失敗
                var result = CustomMessage.Show("認証に失敗しました。\n再認証しますか？", "認証失敗", MessageBoxButton.YesNo, MessageBoxImage.Error);
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

            while (!_dataManager.ServerConnected)
            {
                try
                {
                    await _connection.StartAsync();
                    Console.WriteLine("Connected");
                    _dataManager.ServerConnected = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection Error!! {ex.Message}");
                    _dataManager.ServerConnected = false;
                }
            }

            // 再接続イベントのハンドリング
            _connection.Reconnecting += exception =>
            {
                _dataManager.ServerConnected = false;
                Console.WriteLine("Reconnecting");
                return Task.CompletedTask;
            };

            _connection.Reconnected += exeption =>
            {
                _dataManager.ServerConnected = true;
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
        public async Task SendRequestAsync(DatabaseOperational.DataToServer dataToServer)
        {
            try
            {
                // サーバーメソッドの呼び出し
                var jsonMessage = await _connection.InvokeAsync<string>("SendData_Interlocking", dataToServer);
                try
                {
                    // JSONを一時保存クラスにデシリアライズ
                    var data = JsonConvert.DeserializeObject<DatabaseTemporary.RootObject>(jsonMessage);
                    if (data != null)
                    {
                        // 一時保存クラスから運用クラスに代入
                        _dataManager.DataFromServer = _dataManager.UpdateDataFromServer(data);
                        // 認証情報を保存
                        _dataManager.Authentication ??= _dataManager.DataFromServer.Authentication;
                        // コントロール更新処理
                        DataUpdateViewModel.Instance.UpdateControl(_dataManager.DataFromServer);
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

        /// <summary>
        /// サーバー切断
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAsync()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
            }
        }
    }
}
