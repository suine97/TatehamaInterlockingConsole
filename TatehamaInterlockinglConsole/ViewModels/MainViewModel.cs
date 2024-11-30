using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Services;

namespace TatehamaInterlockingConsole.ViewModels
{
    /// <summary>
    /// メイン画面を管理するViewModelクラス
    /// </summary>
    public class MainViewModel : WindowViewModel
    {
        private readonly TimeService _timeService; // 時間管理サービス
        private readonly DataManager _dataManager; // データ管理を担当するクラス
        private DataUpdateViewModel _dataUpdate; // データ更新処理を管理するViewModel
        private static bool _isConstructorExecuted = false; // コンストラクタが一度だけ実行されることを保証するフラグ

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

        /// <summary>
        /// 通信状態ステータス
        /// </summary>
        public bool ConnectionStatus { get; set; } = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timeService">時間管理サービス</param>
        /// <param name="uiElementLoader">UI要素ローダー</param>
        /// <param name="dataManager">データ管理クラス</param>
        public MainViewModel(TimeService timeService, DataManager dataManager, DataUpdateViewModel dataUpdateViewModel)
        {
            try
            {
                // 初回呼び出し時のみコンストラクタ処理を実行
                if (!_isConstructorExecuted)
                {
                    _isConstructorExecuted = true;

                    _timeService = timeService;
                    _dataUpdate = dataUpdateViewModel;
                    _dataManager = dataManager;
                    _dataManager.Initialize(timeService);

                    Title = "連動盤選択 | 連動盤 - ダイヤ運転会";
                    IncreaseTimeCommand = new RelayCommand(() => _timeService.IncreaseTime());
                    DecreaseTimeCommand = new RelayCommand(() => _timeService.DecreaseTime());

                    // 時間更新イベントを購読
                    _dataManager.TimeUpdated += (currentTime) => OnPropertyChanged(nameof(CurrentTime));
                    // 初期化処理
                    Initialize();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Initialize()
        {
            var folderPath = "TSV";
            // 設定データをリストに格納
            _dataManager.AllControlSettingList = UIElementLoader.LoadSettingsFromFolderAsUIControlSetting(folderPath);
            // メイン画面用のUI要素を取得
            var mainControlSettingList = _dataManager.AllControlSettingList.FindAll(list => list.StationName == "Main_UIList");
            MainElements = UIElementLoader.CreateUIControlModels(mainControlSettingList);

            // タイマー開始
            _timeService.Start();
        }

        /// <summary>
        /// タイマー周期ごとの処理
        /// </summary>
        public void OnTimerElapsed()
        {
            _dataUpdate.UpdateTimerEvent();
        }

        /// <summary>
        /// ウィンドウを閉じる際の確認処理
        /// </summary>
        /// <returns>ウィンドウを閉じて良い場合はtrue、それ以外はfalse</returns>
        public bool ConfirmClose()
        {
            var result = MessageBox.Show("全ての連動盤を閉じます。よろしいですか？",
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
            return true;
        }
    }
}
