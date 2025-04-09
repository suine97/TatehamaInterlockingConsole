using System;
using System.Linq;
using System.Windows;
using TatehamaInterlockingConsole.Helpers;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Services;
using TatehamaInterlockingConsole.Views;

namespace TatehamaInterlockingConsole.ViewModels
{
    public static class WindowAction
    {
        public static void ShowStationWindow(string stationName)
        {
            try
            {
                // ウィンドウタイトルを設定
                var station = DataHelper.GetStationNameFromEnglishName(stationName);
                var titleText = $"{station} | 連動盤 - ダイヤ運転会";

                // 既に同じタイトルのウィンドウが存在するかチェック
                var existingWindow = Application.Current.Windows
                    .OfType<StationWindow>()
                    .FirstOrDefault(w => w.Title == titleText);
                if (existingWindow != null)
                {
                    existingWindow.Activate();
                    return;
                }

                // ウィンドウが存在しない場合、新しく作成して表示
                var viewModel = new StationViewModel(titleText, $"TSV/{stationName}_UIList.tsv", DataManager.Instance, Sound.Instance, DataUpdateViewModel.Instance);
                var window = new StationWindow(viewModel)
                {
                    DataContext = viewModel,
                    Title = titleText,
                    Topmost = DataManager.Instance.IsTopMost,
                };
                window.Show();
            }
            catch (Exception ex)
            {
                CustomMessage.Show(ex.Message, "エラー");
            }
        }
    }
}
