using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.ViewModels;

namespace TatehamaInterlockingConsole.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private string _token;
        private readonly OpenIddictClientService _openIddictClientService;

        public MainWindow(MainViewModel viewModel, OpenIddictClientService openIddictClientService)
        {
            InitializeComponent();
            DataContext = viewModel;
            _openIddictClientService = openIddictClientService;

            Loaded += OnLoaded;

            // タイマーの初期化
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // DataContextの初期サイズをウィンドウに設定
            if (DataContext is MainViewModel viewModel)
            {
                Width = viewModel.WindowWidth;
                Height = viewModel.WindowHeight;
                Topmost = true;
            }
            // ユーザー認証
            await AuthenticateAsync();
            // サーバー接続初期化
            await InitializeConnection(_token);
        }

        /// <summary>
        /// ユーザー認証
        /// </summary>
        /// <returns></returns>
        private async Task AuthenticateAsync()
        {
            try
            {
                using var source = new CancellationTokenSource(TimeSpan.FromSeconds(90));

                var result = await _openIddictClientService.ChallengeInteractivelyAsync(new()
                {
                    CancellationToken = source.Token
                });

                var resultAuth = await _openIddictClientService.AuthenticateInteractivelyAsync(new()
                {
                    CancellationToken = source.Token,
                    Nonce = result.Nonce
                });

                _token = resultAuth.BackchannelAccessToken;

                Console.WriteLine($"Authentication successful.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Authentication timed out. The process was aborted.");
            }
            catch (OpenIddictExceptions.ProtocolException exception)
                when (exception.Error is OpenIddictConstants.Errors.AccessDenied)
            {
                Console.WriteLine("The authorization was denied by the end user.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication failed: {ex.Message}");
            }
        }

        /// <summary>
        /// サーバー接続初期化
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task InitializeConnection(string token)
        {
            // HubConnectionの作成
            await using var client = new HubConnectionBuilder()
                .WithUrl($"{ServerAddress.SignalAddress}/hub/train?access_token={_token}")
                .WithAutomaticReconnect() // 自動再接続
                .Build();

            // 再接続イベントのハンドリング
            client.Reconnecting += error =>
            {
                Console.WriteLine($"SignalR 再接続中: {error?.Message}");
                return Task.CompletedTask;
            };

            client.Reconnected += connectionId =>
            {
                Console.WriteLine($"SignalR 再接続成功: {connectionId}");
                return Task.CompletedTask;
            };

            client.Closed += async error =>
            {
                Console.WriteLine($"SignalR 接続が切断されました: {error?.Message}");
                await Task.Delay(5000);
                await client.StartAsync();
            };
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                // タイマーイベントをViewModelに通知
                viewModel.OnTimerElapsed();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                // 閉じる確認ダイアログの処理
                if (!viewModel.ConfirmClose())
                {
                    e.Cancel = true;
                    return;
                }

                // タイマーを停止
                _timer.Stop();

                // アプリケーション全体を終了
                Application.Current.Shutdown();
            }
        }
    }
}
