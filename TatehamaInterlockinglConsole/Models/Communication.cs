using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TatehamaInterlockinglConsole.Models
{
    internal class Communication
    {
        // WebSocket関連のフィールド
        private ClientWebSocket _webSocket = new ClientWebSocket();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private static readonly Encoding _encoding = Encoding.UTF8;
        private readonly string _connectUri = "https://localhost:7232";

        // キャッシュ用の静的辞書
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<Type, FieldInfo[]> FieldCache = new ConcurrentDictionary<Type, FieldInfo[]>();

        // JSONシリアライザ設定
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        // データ関連フィールド
        private string _command = "DataRequest";
        private string[] _request = { "all" };

        // イベント
        internal event Action<string> CycleTimeUpdated;
        internal event Action<string> ConnectionStatusChanged;

        /// <summary>
        /// TrainCrew側データ要求コマンド
        /// (DataRequest, SetEmergencyLight, SetSignalPhase)
        /// </summary>
        internal string Command
        {
            get => _command;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Command cannot be null.");
                }

                if (IsValidCommand(value))
                {
                    _command = value;
                }
                else
                {
                    throw new ArgumentException("Invalid command values.");
                }
            }
        }

        /// <summary>
        /// TrainCrew側データ要求引数
        /// (all, tc, tconlyontrain, tcall, signal, train)
        /// </summary>
        internal string[] Request
        {
            get => _request;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "Request cannot be null.");
                }

                if (IsValidRequest(_command, value))
                {
                    _request = value;
                }
                else
                {
                    throw new ArgumentException("Invalid request values.");
                }
            }
        }

        /// <summary>
        /// コマンドの検証
        /// </summary>
        /// <param name="requestValues"></param>
        /// <returns></returns>
        private static bool IsValidCommand(string requestValues) =>
            new[] { "DataRequest" }.Contains(requestValues);

        /// <summary>
        /// リクエストの検証
        /// </summary>
        /// <param name="commandValue"></param>
        /// <param name="requestValues"></param>
        /// <returns></returns>
        private static bool IsValidRequest(string commandValue, string[] requestValues)
        {
            switch (commandValue)
            {
                case "DataRequest":
                    return requestValues.Length == 1 && requestValues[0] == "all";
                default:
                    return false;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Communication()
        {
            _webSocket = new ClientWebSocket();
        }

        /// <summary>
        /// 受信データ処理メソッド
        /// </summary>
        private void ProcessingReceiveData()
        {
            // その他処理など…
        }

        /// <summary>
        /// WebSocket接続試行
        /// </summary>
        /// <returns></returns>
        internal async Task TryConnectWebSocket()
        {
            while (true)
            {
                _webSocket = new ClientWebSocket();

                try
                {
                    // 接続処理
                    await ConnectWebSocket();
                    break;
                }
                catch (Exception)
                {
                    CycleTimeUpdated?.Invoke("通信周期：----ms");
                    ConnectionStatusChanged?.Invoke("Status：接続待機中...");
                    await Task.Delay(1000);
                }
            }
        }

        /// <summary>
        /// WebSocket接続処理
        /// </summary>
        /// <returns></returns>
        private async Task ConnectWebSocket()
        {
            // 送信処理
            await SendMessages();
            // 受信処理
            await ReceiveMessages();
        }

        /// <summary>
        /// WebSocket送信処理
        /// </summary>
        private async Task SendMessages()
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                await _webSocket.ConnectAsync(new Uri(_connectUri), CancellationToken.None);
            }

            ServerData.CommandToServer requestCommand = new ServerData.CommandToServer()
            {
                Command = _command,
                Args = _request
            };

            // JSON形式にシリアライズ
            string json = JsonConvert.SerializeObject(requestCommand, JsonSerializerSettings);
            byte[] bytes = _encoding.GetBytes(json);

            // WebSocket送信
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task SendMessages(string command, string[] request)
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                await _webSocket.ConnectAsync(new Uri(_connectUri), CancellationToken.None);
            }

            ServerData.CommandToServer requestCommand = new ServerData.CommandToServer()
            {
                Command = command,
                Args = request
            };

            // JSON形式にシリアライズ
            string json = JsonConvert.SerializeObject(requestCommand, JsonSerializerSettings);
            byte[] bytes = _encoding.GetBytes(json);

            // WebSocket送信
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        internal async Task SendSingleCommand(string command, string[] request)
        {
            // コマンドとリクエストを検証
            if (IsValidCommand(command) && IsValidRequest(command, request))
            {
                await SendMessages(command, request);
            }
            else
            {
                throw new ArgumentException("Invalid command or request values.");
            }
        }

        /// <summary>
        /// WebSocket受信処理
        /// </summary>
        /// <returns></returns>
        private async Task ReceiveMessages()
        {
            var buffer = new byte[2048];
            StringBuilder messageBuilder = null;

            while (_webSocket.State == WebSocketState.Open)
            {
                _stopwatch.Restart();
                ConnectionStatusChanged?.Invoke("Status：接続完了");

                WebSocketReceiveResult result;
                do
                {
                    result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        // サーバーからの切断要求を受けた場合
                        await CloseAsync();
                        CycleTimeUpdated?.Invoke("通信周期：----ms");
                        ConnectionStatusChanged?.Invoke("Status：接続待機中...");
                        await TryConnectWebSocket();
                        return;
                    }
                    else
                    {
                        if (messageBuilder == null)
                        {
                            messageBuilder = new StringBuilder();
                        }
                        string partMessage = _encoding.GetString(buffer, 0, result.Count);
                        messageBuilder.Append(partMessage);
                    }

                } while (!result.EndOfMessage);

                string jsonResponse = messageBuilder.ToString();
                messageBuilder = null;

                //
                //
                // デシリアライズ処理実装
                //
                //

                _stopwatch.Stop();
                string s = (_stopwatch.Elapsed.TotalSeconds * 1000).ToString("F2");
                CycleTimeUpdated?.Invoke($"通信周期：{s}ms");
            }
        }

        /// <summary>
        /// WebSocket終了処理
        /// </summary>
        /// <returns></returns>
        private async Task CloseAsync()
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                // 正常に接続を閉じる
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
            }
            _webSocket.Dispose();
        }

        /// <summary>
        /// フィールド・プロパティ置換メソッド
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private void UpdateFieldsAndProperties<T>(T target, T source) where T : class
        {
            if (target == null || source == null)
            {
                throw new ArgumentNullException("target or source cannot be null");
            }

            // プロパティのキャッシュを取得または設定
            var properties = PropertyCache.GetOrAdd(target.GetType(), t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));
            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    var newValue = property.GetValue(source);
                    property.SetValue(target, newValue);
                }
            }

            // フィールドのキャッシュを取得または設定
            var fields = FieldCache.GetOrAdd(target.GetType(), t => t.GetFields(BindingFlags.Public | BindingFlags.Instance));
            foreach (var field in fields)
            {
                var newValue = field.GetValue(source);
                field.SetValue(target, newValue);
            }
        }
    }
}
