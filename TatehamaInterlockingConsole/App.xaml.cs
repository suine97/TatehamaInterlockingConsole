using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenIddict.Client;
using TatehamaInterlockingConsole.ViewModels;
using TatehamaInterlockingConsole.Views;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Services;
using System.Reflection;

namespace TatehamaInterlockingConsole
{
    public partial class App : Application
    {
        private IHost _host;

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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // IHostの初期化
            _host = new HostBuilder()
                .ConfigureLogging(options => options.AddDebug())
                .ConfigureServices(services =>
                {
                    // DbContextの設定
                    services.AddDbContext<DbContext>(options =>
                    {
                        options.UseSqlite(
                            $"Filename={Path.Combine(Path.GetTempPath(), "trancrew-multiats-client.sqlite3")}");
                        options.UseOpenIddict();
                    });

                    // OpenIddictの設定
                    services.AddOpenIddict()

                        .AddCore(options =>
                        {
                            options.UseEntityFrameworkCore()
                                .UseDbContext<DbContext>();
                        })

                        .AddClient(options =>
                        {
                            options.AllowAuthorizationCodeFlow()
                                .AllowRefreshTokenFlow();

                            options.AddDevelopmentEncryptionCertificate()
                                .AddDevelopmentSigningCertificate();

                            options.UseSystemIntegration();

                            options.UseSystemNetHttp()
                                .SetProductInformation(typeof(App).Assembly);

                            options.AddRegistration(new OpenIddictClientRegistration
                            {
                                Issuer = new Uri(ServerAddress.SignalAddress + "/hub/train", UriKind.Absolute),
                                ClientId = "MultiATS_Client",
                                RedirectUri = new Uri("/", UriKind.Relative),
                            });
                        }); 

                    // 必要なサービスの登録
                    services.AddSingleton<ITimeService, TimeService>();
                    services.AddSingleton<IDataManager>(DataManager.Instance);
                    services.AddSingleton<IDataUpdateViewModel>(DataUpdateViewModel.Instance);

                    // ViewModelの登録
                    services.AddTransient<MainViewModel>();
                    // ウィンドウの登録
                    services.AddTransient<MainWindow>();

                    // Workerサービスを登録
                    services.AddHostedService<Worker>();
                })
                .Build();

            // MainWindowとViewModelのインスタンスを取得して表示
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host is not null)
                await _host.StopAsync();

            base.OnExit(e);
        }
    }
}
