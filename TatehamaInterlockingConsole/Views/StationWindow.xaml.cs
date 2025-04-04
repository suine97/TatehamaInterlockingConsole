using System.Windows;
using TatehamaInterlockingConsole.Handlers;
using TatehamaInterlockingConsole.ViewModels;

namespace TatehamaInterlockingConsole.Views
{
    /// <summary>
    /// StationWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class StationWindow : Window
    {
        private StationViewModel _viewModel;

        public StationWindow(StationViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Loaded += OnLoaded;
            PreviewMouseUp += OnPreviewMouseUp;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // DataContextの初期サイズをウィンドウに設定
            if (DataContext is StationViewModel viewModel)
            {
                Width = viewModel.WindowWidth;
                Height = viewModel.WindowHeight;
            }
        }

        private void OnPreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // ImageHandlerのMouseUpイベントを呼び出し
            ImageHandler.Instance?.OnPreviewMouseUp(sender, e);
        }
    }
}
