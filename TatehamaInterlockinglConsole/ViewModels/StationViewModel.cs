using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;
using TatehamaInterlockinglConsole.Factories;
using TatehamaInterlockinglConsole.Models;
using TatehamaInterlockinglConsole.Services;
using TatehamaInterlockinglConsole.Manager;
using System.IO;

namespace TatehamaInterlockinglConsole.ViewModels
{
    /// <summary>
    /// StationViewModelクラス
    /// </summary>
    public class StationViewModel : WindowViewModel
    {
        private readonly UIElementLoader _uiElementLoader;
        private readonly DataManager _dataManager;

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
            Initialize(filePath);

            IsFitMode = false;
            ToggleButtonText = "フィット表示に切り替え";
            Title = title;

            ClosingCommand = new RelayCommand(OnClosing);
            ToggleModeCommand = new RelayCommand(ToggleMode);

            StationElements = LoadTSV.LoadUIFromTSV(filePath);
            WindowWidth = 1280;
            WindowHeight = 720;
            DrawingWidth = ControlFactory.BackImageWidth;
            DrawingHeight = ControlFactory.BackImageHeight;
        }

        private void Initialize(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            StationElements = _uiElementLoader.GetElementsFromSettings(_dataManager.AllTsvDictionary, fileName);
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

        public void ChangeImagePathByIdentifiers(string uniqueName, int imageIndex)
        {
            var setting = StationElements
                .OfType<UIControlSetting>()
                .FirstOrDefault(s => s.UniqueName == uniqueName);

            if (setting != null && setting.ImagePaths.Count > imageIndex)
            {
                // 指定インデックスの画像パスに切り替え
                setting.SelectedImagePath = setting.ImagePaths[imageIndex];
                OnPropertyChanged(nameof(StationElements));
            }
        }
    }
}
