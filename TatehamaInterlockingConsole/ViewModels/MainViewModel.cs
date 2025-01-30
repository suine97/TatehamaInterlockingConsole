using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using OpenIddict.Client;
using TatehamaInterlockingConsole.Factories;
using TatehamaInterlockingConsole.Helpers;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Models;
using TatehamaInterlockingConsole.Services;

namespace TatehamaInterlockingConsole.ViewModels
{
    /// <summary>
    /// メイン画面を管理するViewModelクラス
    /// </summary>
    public class MainViewModel : WindowViewModel
    {
        private readonly TimeService _timeService;                 // 時間管理サービス
        private readonly DataManager _dataManager;                 // データ管理を担当するクラス
        private readonly DataUpdateViewModel _dataUpdateViewModel; // データ更新処理を管理するViewModel
        private readonly ServerCommunication _serverCommunication; // サーバ通信クラス
        private static bool _isConstructorExecuted = false;        // コンストラクタが一度だけ実行されることを保証するフラグ

        /// <summary>
        /// 時刻を1時間進めるコマンド
        /// </summary>
        public ICommand IncreaseTimeCommand { get; }

        /// <summary>
        /// 時刻を1時間戻すコマンド
        /// </summary>
        public ICommand DecreaseTimeCommand { get; }

        /// <summary>
        /// メイン画面に表示されるUI要素List
        /// </summary>
        public ObservableCollection<UIElement> MainElements { get; set; }

        /// <summary>
        /// 現在時刻を取得
        /// </summary>
        public DateTime CurrentTime => _dataManager.CurrentTime;

        /// <summary>
        /// ウィンドウのタイトル
        /// </summary>
        public string Title { get; set; }

        private bool _connectionStatus;
        /// <summary>
        /// 通信状態ステータス
        /// </summary>
        public bool ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                if (_connectionStatus != value)
                {
                    _connectionStatus = value;
                    OnPropertyChanged(nameof(ConnectionStatus));
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timeService"></param>
        /// <param name="dataManager"></param>
        /// <param name="dataUpdateViewModel"></param>
        /// <param name="openIddictClientService"></param>
        public MainViewModel(TimeService timeService, DataManager dataManager, DataUpdateViewModel dataUpdateViewModel, OpenIddictClientService openIddictClientService)
        {
            try
            {
                // 初回呼び出し時のみコンストラクタ処理を実行
                if (!_isConstructorExecuted)
                {
                    _isConstructorExecuted = true;

                    _timeService = timeService;
                    _dataUpdateViewModel = dataUpdateViewModel;
                    _dataManager = dataManager;
                    _dataManager.Initialize(timeService);
                    _serverCommunication = new ServerCommunication(openIddictClientService);

                    Title = "連動盤選択 | 連動盤 - ダイヤ運転会";
                    IncreaseTimeCommand = new RelayCommand(() => _timeService.IncreaseTime());
                    DecreaseTimeCommand = new RelayCommand(() => _timeService.DecreaseTime());

                    // 時間更新イベントを購読
                    _dataManager.TimeUpdated += (currentTime) => OnPropertyChanged(nameof(CurrentTime));
                    // サーバー接続状態イベントを購読
                    _serverCommunication.ConnectionStatusChanged += (status) => ConnectionStatus = status;
                    // 初期化処理
                    Initialize();
                }
            }
            catch (Exception ex)
            {
                CustomMessage.Show(ex.ToString(), "エラー");
                throw ex;
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Initialize()
        {
            var tsvFolderPath = "TSV";
            var retsubanFolderPath = "Image/Retsuban";

            // 駅名データを辞書に格納
            _dataManager.StationNameDictionary = DataHelper.LoadTSVAsDictionary(tsvFolderPath, "StationList.tsv");
            // 設定データをリストに格納
            _dataManager.AllControlSettingList = UIElementLoader.LoadSettingsFromFolderAsUIControlSetting(tsvFolderPath);
            // 列番表示画像Pathを辞書に格納
            _dataManager.RetsubanImagePathDictionary = RetsubanFactory.GetRetsubanImagePath(retsubanFolderPath);

            // メイン画面用のUI要素を取得
            var mainControlSettingList = _dataManager.AllControlSettingList.FindAll(list => list.StationName == "Main_UIList");
            MainElements = UIElementLoader.CreateUIControlModels(mainControlSettingList);

            // タイマー開始
            _timeService.Start();
        }

        /// <summary>
        /// ウィンドウを閉じる際の確認処理
        /// </summary>
        /// <returns>ウィンドウを閉じて良い場合はtrue、それ以外はfalse</returns>
        public bool ConfirmClose()
        {
            var result = CustomMessage.Show("全ての連動盤を閉じます。よろしいですか？",
                "終了確認", 
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                return false;
            }

            // リソース解放処理
            MainElements.Clear();
            _timeService.Stop();
            ImageCacheManager.ClearCache();
            _dataManager.TimeUpdated -= (currentTime) => OnPropertyChanged(nameof(CurrentTime));
            _serverCommunication.ConnectionStatusChanged -= (status) => OnPropertyChanged(nameof(ConnectionStatus));
            return true;
        }
    }
}
