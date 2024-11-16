using System;
using System.Linq;
using System.Windows;
using TatehamaInterlockinglConsole.Manager;
using TatehamaInterlockinglConsole.Services;
using TatehamaInterlockinglConsole.Views;

namespace TatehamaInterlockinglConsole.ViewModels
{
    public static class WindowAction
    {
        public static void ShowStationWindow(string stationName)
        {
            try
            {
                // ウィンドウタイトルを設定
                int i = stationName.IndexOf('_');
                var station = i > 0 ? stationName.Substring(i + 1) : stationName;
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
                var viewModel = new StationViewModel(titleText, $"TSV/{stationName}_UIList.tsv", new UIElementLoader(), DataManager.Instance);
                var window = new StationWindow(viewModel)
                {
                    DataContext = viewModel,
                    Title = titleText,
                    Topmost = true
                };
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
