using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using OpenIddict.Client;
using TatehamaInterlockingConsole.Models;
using TatehamaInterlockingConsole.ViewModels;

namespace TatehamaInterlockingConsole.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private readonly ServerCommunication _serverCommunication;

        public MainWindow(MainViewModel viewModel, OpenIddictClientService openIddictClientService)
        {
            InitializeComponent();
            DataContext = viewModel;
            _serverCommunication = new ServerCommunication(openIddictClientService);

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
            // ユーザー認証・初期化
            await _serverCommunication.AuthenticateAsync();
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
