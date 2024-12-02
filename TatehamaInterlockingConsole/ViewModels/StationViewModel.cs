using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using TatehamaInterlockingConsole.Factories;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Helpers;
using TatehamaInterlockingConsole.Services;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole.ViewModels
{
    /// <summary>
    /// 駅毎の連動盤画面を管理するViewModelクラス
    /// </summary>
    public class StationViewModel : WindowViewModel
    {
        private readonly DataManager _dataManager; // データ管理クラス
        private readonly Sound _sound; // サウンド管理クラスのインスタンス
        private readonly DataUpdateViewModel _dataUpdateViewModel;
        private readonly DispatcherTimer _clockUpdateTimer;
        private string _stationName; // 駅名

        /// <summary>
        /// 表示モードを切り替えるコマンド
        /// </summary>
        public ICommand ToggleModeCommand { get; }

        /// <summary>
        /// ウィンドウを閉じる際のコマンド
        /// </summary>
        public ICommand ClosingCommand { get; }

        /// <summary>
        /// ウィンドウのタイトル
        /// </summary>
        public string Title { get; set; }

        private ObservableCollection<UIElement> _stationElements = new ObservableCollection<UIElement>();
        /// <summary>
        /// 駅毎の連動盤のUI要素のコレクション
        /// </summary>
        public ObservableCollection<UIElement> StationElements
        {
            get => _stationElements;
            set
            {
                if (_stationElements != value)
                {
                    _stationElements = value;
                    OnPropertyChanged(nameof(StationElements));
                }
            }
        }

        private bool _isFitMode = true;
        /// <summary>
        /// 表示モードがFitModeかどうか
        /// </summary>
        public bool IsFitMode
        {
            get => _isFitMode;
            set
            {
                if (_isFitMode != value)
                {
                    _isFitMode = value;
                    OnPropertyChanged(nameof(IsFitMode));
                }
            }
        }

        private string _toggleButtonText;
        /// <summary>
        /// トグルボタンのテキスト
        /// </summary>
        public string ToggleButtonText
        {
            get { return _toggleButtonText; }
            set
            {
                if (_toggleButtonText != value)
                {
                    _toggleButtonText = value;
                    OnPropertyChanged(nameof(ToggleButtonText));
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="title">ウィンドウのタイトル</param>
        /// <param name="filePath">駅データのファイルパス</param>
        /// <param name="uiElementLoader">UI 要素ローダー</param>
        /// <param name="dataManager">データ管理クラス</param>
        public StationViewModel(string title, string filePath, DataManager dataManager, Sound sound, DataUpdateViewModel dataUpdateViewModel)
        {
            try
            {
                _dataManager = dataManager;
                _sound = sound;
                _dataUpdateViewModel = dataUpdateViewModel;
                _dataUpdateViewModel.NotifyUpdateControlEvent += OnNotifyUpdateControlEvent;
                _stationName = DataHelper.ExtractStationNameFromFilePath(filePath); // ファイルパスから駅名を抽出

                _clockUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                _clockUpdateTimer.Tick += OnClockUpdate;
                _clockUpdateTimer.Start();

                IsFitMode = false;
                ToggleButtonText = "フィット表示に切り替え";
                Title = title;

                ClosingCommand = new RelayCommand(OnClosing);
                ToggleModeCommand = new RelayCommand(ToggleMode);

                // 駅毎の連動盤に対応する設定データを取得
                var stationSettingList = _dataManager.AllControlSettingList.FindAll(list => list.StationName == _stationName);
                StationElements = UIElementLoader.CreateUIControlModels(stationSettingList);

                // ウィンドウサイズと描画領域の設定
                WindowWidth = 1280;
                WindowHeight = 720;
                DrawingWidth = BackImageFactory.BackImageWidth;
                DrawingHeight = BackImageFactory.BackImageHeight;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// DataUpdateViewModelでの変更通知受け取り処理
        /// </summary>
        /// <param name="updateList"></param>
        private void OnNotifyUpdateControlEvent(List<UIControlSetting> updateList)
        {
            // 駅毎の連動盤に対応する設定データを取得
            var stationSettingList = updateList.FindAll(list => list.StationName == _stationName);
            var newElements = UIElementLoader.CreateUIControlModels(stationSettingList);
            var newCollection = new ObservableCollection<UIElement>(newElements);

            // 差分比較
            if (!UIElementLoader.AreCollectionsEqual(StationElements, newCollection))
            {
                // 差分がある場合のみ更新
                StationElements = newCollection;
            }
        }

        /// <summary>
        /// 時計更新処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClockUpdate(object sender, EventArgs e)
        {
            // 時計UI要素の更新処理
            foreach (var element in StationElements)
            {
                if (element is Canvas clockCanvas)
                {
                    ClockImageFactory.UpdateClockHands(clockCanvas);
                }
            }
        }

        /// <summary>
        /// ウィンドウを閉じる際の処理
        /// </summary>
        public void OnClosing()
        {
            // UI要素をクリア
            StationElements.Clear();
        }

        /// <summary>
        /// 表示モード切り替え
        /// </summary>
        private void ToggleMode()
        {
            IsFitMode = !IsFitMode;
            ToggleButtonText = IsFitMode ? "原寸大表示に切り替え" : "フィット表示に切り替え";
        }
    }
}
