using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using TatehamaInterlockinglConsole.Services;
using TatehamaInterlockinglConsole.Manager;
using TatehamaInterlockinglConsole.Handlers;

namespace TatehamaInterlockinglConsole.ViewModels
{
    public class MainViewModel : WindowViewModel
    {
        private readonly TimeService _timeService;
        private readonly UIElementLoader _uiElementLoader;
        private readonly DataManager _dataManager;
        private TimerHandler _timerHandler;
        private static bool _isConstructorExecuted = false;

        public ICommand IncreaseTimeCommand { get; }
        public ICommand DecreaseTimeCommand { get; }
        public DateTime CurrentTime => _dataManager.CurrentTime;
        public ObservableCollection<UIElement> MainElements { get; private set; }
        public string Title { get; set; }

        public MainViewModel(TimeService timeService, UIElementLoader uiElementLoader, DataManager dataManager)
        {
            // 初回呼び出し時のみ処理
            if (!_isConstructorExecuted)
            {
                _isConstructorExecuted = true;

                _timeService = timeService;
                _uiElementLoader = uiElementLoader;
                _dataManager = dataManager;
                _dataManager.Initialize(timeService);

                Title = "連動盤選択 | 連動盤 - ダイヤ運転会";
                IncreaseTimeCommand = new RelayCommand(() => _timeService.IncreaseTime());
                DecreaseTimeCommand = new RelayCommand(() => _timeService.DecreaseTime());

                _dataManager.TimeUpdated += (currentTime) => OnPropertyChanged(nameof(CurrentTime));
                Initialize();
            }
        }

        private void Initialize()
        {
            var folderPath = "TSV";
            // 全TSVデータをDictionaryに格納
            _dataManager.AllTsvDictionary = _uiElementLoader.LoadSettingsFromFolderAsDictionary(folderPath);
            // Main_UIListのみ取得
            MainElements = _uiElementLoader.GetElementsFromSettings(_dataManager.AllTsvDictionary, "Main_UIList");

            _timeService.Start();
            _timerHandler = new TimerHandler();
        }

        public void OnTimerElapsed()
        {
            _timerHandler.HandleTimerEvent();
        }

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

            MainElements.Clear();
            _timeService.Stop();
            return true;
        }
    }
}
