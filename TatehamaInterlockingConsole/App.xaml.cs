using System;
using System.IO;
using System.Windows;
using TatehamaInterlockingConsole.ViewModels;
using TatehamaInterlockingConsole.Views;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Services;

namespace TatehamaInterlockingConsole
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 画像フォルダを指定してキャッシュを初期化
            string imageFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image");
            ImageCacheManager.LoadImages(imageFolderPath);

            var viewModel = new MainViewModel(new TimeService(), DataManager.Instance, DataUpdateViewModel.Instance);
            var mainWindow = new MainWindow(viewModel);
            mainWindow.Show();
        }
    }
}
