using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using TatehamaInterlockinglConsole.Factories;
using TatehamaInterlockinglConsole.Services;
using TatehamaInterlockinglConsole.Manager;

namespace TatehamaInterlockinglConsole.ViewModels
{
    /// <summary>
    /// StationViewModelクラス
    /// </summary>
    public class StationViewModel : WindowViewModel
    {
        private readonly UIElementLoader _uiElementLoader;
        private readonly DataManager _dataManager;
        private readonly Sound _sound;
        private string _stationName;

        public ICommand ToggleModeCommand { get; }
        public ICommand ClosingCommand { get; }
        public ObservableCollection<UIElement> StationElements { get; private set; } = new ObservableCollection<UIElement>();
        public string Title { get; set; }

        private bool _isFitMode = true;
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

        public StationViewModel(string title, string filePath, UIElementLoader uiElementLoader, DataManager dataManager)
        {
            _uiElementLoader = uiElementLoader;
            _dataManager = dataManager;
            _sound = new Sound();
            _stationName = Data.GetStatinNameFromFilePath(filePath);

            IsFitMode = false;
            ToggleButtonText = "フィット表示に切り替え";
            Title = title;

            ClosingCommand = new RelayCommand(OnClosing);
            ToggleModeCommand = new RelayCommand(ToggleMode);

            var stationSettingList = _dataManager.AllControlSettingList.FindAll(list => list.StationName == _stationName);
            StationElements = CreateUIControl.CreateUIControlAsUIElement(stationSettingList);
            WindowWidth = 1280;
            WindowHeight = 720;
            DrawingWidth = BackImageFactory.BackImageWidth;
            DrawingHeight = BackImageFactory.BackImageHeight;
        }

        private void OnClosing()
        {
            StationElements.Clear();
        }

        private void ToggleMode()
        {
            IsFitMode = !IsFitMode;
            ToggleButtonText = IsFitMode ? "原寸大表示に切り替え" : "フィット表示に切り替え";
        }
    }
}
