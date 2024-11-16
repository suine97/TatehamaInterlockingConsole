using System.ComponentModel;
using System.Windows;
using TatehamaInterlockinglConsole.ViewModels;

namespace TatehamaInterlockinglConsole.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // DataContextの初期サイズをウィンドウに設定
            if (DataContext is MainViewModel viewModel)
            {
                Width = viewModel.WindowWidth;
                Height = viewModel.WindowHeight;
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

                // アプリケーション全体を終了
                Application.Current.Shutdown();
            }
        }
    }
}
