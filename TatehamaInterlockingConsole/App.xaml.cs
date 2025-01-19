using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TatehamaInterlockingConsole.ViewModels;
using TatehamaInterlockingConsole.Views;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Services;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            // AppDomainのAssemblyResolveイベントを設定
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // "libs"フォルダのパスを取得
            string libsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libs");
            // DLL名を取得
            string assemblyName = new AssemblyName(args.Name).Name + ".dll";
            // 対象DLLのフルパスを取得
            string assemblyPath = Path.Combine(libsPath, assemblyName);
            // DLLが存在する場合に読み込む
            if (File.Exists(assemblyPath))
            {
                return Assembly.LoadFrom(assemblyPath);
            }
            return null;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // 必要なサービスの登録
            services.AddSingleton(TimeService.Instance);
            services.AddSingleton(DataManager.Instance);
            services.AddSingleton(DataUpdateViewModel.Instance);
            // ViewModelの登録
            services.AddTransient<MainViewModel>();
            // ウィンドウの登録
            services.AddTransient<MainWindow>();
            // Workerサービスを登録
            services.AddHostedService<Worker>();

            _serviceProvider = services.BuildServiceProvider();

            // MainWindowとViewModelのインスタンスを取得して表示
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            // サーバー接続
            await ServerCommunication.Instance.StartAsync();
            // ユーザー認証
            await ServerCommunication.Instance.AuthenticateAsync();
        }
    }
}
