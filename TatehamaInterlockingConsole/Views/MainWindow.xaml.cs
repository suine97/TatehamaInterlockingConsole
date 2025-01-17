using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using TatehamaInterlockingConsole.ViewModels;

namespace TatehamaInterlockingConsole.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer;
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
            // 認証処理の実行
            await AuthenticateAsync();
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

                string token = resultAuth.BackchannelAccessToken;

                Debug.WriteLine($"Authentication successful. Token: {token}");
                MessageBox.Show($"Authentication successful.\n\nToken: {token}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Authentication timed out. The process was aborted.");
                MessageBox.Show("Authentication timed out. The process was aborted.", "Timeout", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (OpenIddictExceptions.ProtocolException exception)
                when (exception.Error is OpenIddictConstants.Errors.AccessDenied)
            {
                Debug.WriteLine("The authorization was denied by the end user.");
                MessageBox.Show("The authorization was denied by the end user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Authentication failed: {ex.Message}");
                MessageBox.Show($"Authentication failed:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
