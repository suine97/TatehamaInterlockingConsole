using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Services;
using TatehamaInterlockingConsole.ViewModels;
using TatehamaInterlockingConsole.Views;

namespace TatehamaInterlockingConsole.Models
{
    /// <summary>
    /// サーバー通信クラス
    /// </summary>
    public class ServerCommunication
    {
        private static readonly ServerCommunication _instance = new();
        public static ServerCommunication Instance => _instance;

        private IHost _host;
        private string _token;
        private HubConnection _connection;
        private readonly OpenIddictClientService _openIddictClientService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private ServerCommunication()
        {
            // IHostの初期化処理
            _host = new HostBuilder()
                .ConfigureLogging(options => options.AddDebug())
                .ConfigureServices(services =>
                {
                    services.AddDbContext<DbContext>(options =>
                    {
                        options.UseSqlite($"Filename={Path.Combine(Path.GetTempPath(), "trancrew-multiats-client.sqlite3")}");
                        options.UseOpenIddict();
                    });

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

                    services.AddSingleton(TimeService.Instance);
                    services.AddSingleton(DataManager.Instance);
                    services.AddSingleton(DataUpdateViewModel.Instance);
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<MainWindow>();
                    services.AddHostedService<Worker>();
                })
                .Build();

            _openIddictClientService = _host.Services.GetRequiredService<OpenIddictClientService>();
        }

        /// <summary>
        /// サーバーへ接続
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            await _host.StartAsync();
        }

        /// <summary>
        /// サーバーへの接続を停止
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync()
        {
            await _host.StopAsync();
        }

        /// <summary>
        /// ユーザー認証処理
        /// </summary>
        /// <returns></returns>
        public async Task AuthenticateAsync()
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

                // 接続初期設定
                InitializeConnection(_token);

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
        private void InitializeConnection(string token)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{ServerAddress.SignalAddress}/hub/train?access_token={token}")
                .WithAutomaticReconnect()
                .Build();

            // サーバーからのメッセージを受信
            _connection.On<string>("ReceiveMessage", (jsonMessage) =>
            {
                try
                {
                    // JSONをDatabaseTemporary.RootObjectにデシリアライズ
                    var data = JsonConvert.DeserializeObject<DatabaseTemporary.RootObject>(jsonMessage);
                    if (data != null)
                    {
                        Console.WriteLine("Data successfully deserialized:");
                        Console.WriteLine($"Track Circuits: {data.TrackCircuitList.Count}");
                        Console.WriteLine($"Signals: {data.SignalDataList.Count}");
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
            });
        }

        /// <summary>
        /// サーバーへメッセージ送信
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(DatabaseOperational.CommandToServer command)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                try
                {
                    // CommandToServerオブジェクトをJSON形式にシリアライズ
                    string message = JsonConvert.SerializeObject(command);

                    await _connection.InvokeAsync("SendMessage");
                    Console.WriteLine("Message sent.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send message: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Connection is not established.");
            }
        }
    }
}
