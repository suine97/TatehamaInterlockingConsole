using System;
using System.Diagnostics;
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
        private readonly DataUpdateViewModel _dataUpdateViewModel;
        private static HubConnection _connection;
        private static bool _isUpdateLoopRunning = false;

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
            _dataManager = DataManager.Instance;
            _dataUpdateViewModel = DataUpdateViewModel.Instance;

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

                // サーバー接続状態変更イベント発火
                ConnectionStatusChanged?.Invoke(_dataManager.ServerConnected);

                // サーバー接続中ならデータ送信
                if (_dataManager.ServerConnected)
                {
                    await SendConstantDataRequestToServerAsync(new DatabaseOperational.ConstantDataToServer
                    {
                        ActiveStationsList = _dataManager.ActiveStationsList
                    });
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

            // サーバー接続
            while (!_dataManager.ServerConnected)
            {
                try
                {
                    await _connection.StartAsync();
                    Debug.WriteLine("Connected");
                    _dataManager.ServerConnected = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Connection Error!! {ex.Message}");
                    _dataManager.ServerConnected = false;
                }
            }

            // 再接続イベントのハンドリング
            _connection.Reconnecting += exception =>
            {
                _dataManager.ServerConnected = false;
                Debug.WriteLine("Reconnecting");
                return Task.CompletedTask;
            };

            _connection.Reconnected += exeption =>
            {
                _dataManager.ServerConnected = true;
                Debug.WriteLine("Connected");
                return Task.CompletedTask;
            };
            await Task.Delay(Timeout.Infinite);
        }

        /// <summary>
        /// サーバーへ常時送信用データをリクエスト
        /// </summary>
        /// <param name="constantDataToServer"></param>
        /// <returns></returns>
        public async Task SendConstantDataRequestToServerAsync(DatabaseOperational.ConstantDataToServer constantDataToServer)
        {
            try
            {
                // サーバーメソッドの呼び出し
                var jsonMessage = await _connection.InvokeAsync<string>("SendData_Interlocking", constantDataToServer);
                try
                {
                    // 受信したJSONデータをデシリアライズ
                    var data = JsonConvert.DeserializeObject<DatabaseOperational.DataFromServer>(jsonMessage);

                    if (data != null)
                    {
                        // 運用クラスに代入
                        if (_dataManager.DataFromServer == null)
                        {
                            _dataManager.DataFromServer = data;
                        }
                        else
                        {
                            // 変更があれば更新
                            foreach (var property in typeof(DatabaseOperational.DataFromServer).GetProperties())
                            {
                                var newValue = property.GetValue(data);
                                var oldValue = property.GetValue(_dataManager.DataFromServer);
                                if (newValue != null && !newValue.Equals(oldValue))
                                {
                                    property.SetValue(_dataManager.DataFromServer, newValue);
                                }
                            }
                        }
                        // 認証情報を保存
                        _dataManager.Authentication ??= _dataManager.DataFromServer.Authentications;
                        // 方向てこ情報を保存
                        if (data.Directions != null && !data.Directions.SequenceEqual(_dataManager.DataFromServer.Directions))
                        {
                            _dataManager.DirectionStateList = data.Directions.Select(d => new DirectionStateList
                            {
                                Name = d.Name,
                                State = d.State,
                                UpdateTime = DateTime.Now
                            }).ToList();
                        }
                        // コントロール更新処理
                        _dataUpdateViewModel.UpdateControl(_dataManager.DataFromServer);
                    }
                    else
                    {
                        Debug.WriteLine("Failed to deserialize JSON.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error during JSON deserialization: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to send constant data to server: {ex.Message}");
            }
        }

        /// <summary>
        /// サーバーへ物理てこイベント送信用データをリクエスト
        /// </summary>
        /// <param name="eventDataToServer"></param>
        /// <returns></returns>
        public async Task SendLeverEventDataRequestToServerAsync(DatabaseOperational.LeverEventDataToServer leverEventDataToServer)
        {
            try
            {
                // サーバーメソッドの呼び出し
                var jsonMessage = await _connection.InvokeAsync<string>("SetPhysicalLeverData", leverEventDataToServer);
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to send event data to server: {exception.Message}");
            }
        }

        /// <summary>
        /// サーバーへ着点ボタンイベント送信用データをリクエスト
        /// </summary>
        /// <param name="eventDataToServer"></param>
        /// <returns></returns>
        public async Task SendButtonEventDataRequestToServerAsync(DatabaseOperational.ButtonEventDataToServer buttonEventDataToServer)
        {
            try
            {
                // サーバーメソッドの呼び出し
                var jsonMessage = await _connection.InvokeAsync<string>("SetDestinationButtonState", buttonEventDataToServer);
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to send event data to server: {exception.Message}");
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
