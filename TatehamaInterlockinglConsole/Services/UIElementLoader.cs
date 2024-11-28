using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TatehamaInterlockingConsole.Models;
using TatehamaInterlockingConsole.Factories;

namespace TatehamaInterlockingConsole.Services
{
    public static class UIElementLoader
    {
        /// <summary>
        /// フォルダ内の全てのTSVファイルを読み込み、UIControlSettingに変換して返す
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static List<UIControlSetting> LoadSettingsFromFolderAsUIControlSetting(string folderPath)
        {
            var allSettings = new List<UIControlSetting>();
            foreach (var filePath in Directory.EnumerateFiles(folderPath, "*.tsv"))
            {
                var settings = UIControlSettingLoader.LoadSettings(filePath);
                allSettings.AddRange(settings);
            }
            return allSettings;
        }

        /// <summary>
        /// UIControlSettingを読み込みUIElementModelを返す
        /// </summary>
        /// <param name="allSettings"></param>
        /// <returns></returns>
        public static ObservableCollection<UIElement> CreateUIControlModels(List<UIControlSetting> allSettings, bool drawing = true)
        {
            var elements = new ObservableCollection<UIElement>();

            foreach (var setting in allSettings)
            {
                var element = ControlFactory.CreateControl(setting, allSettings, drawing);
                if (element != null)
                {
                    elements.Add(element);
                }
            }
            return elements;
        }

        /// <summary>
        /// UIControlSettingを読み込みUIElementModelを返す
        /// </summary>
        /// <param name="allSettings"></param>
        /// <returns></returns>
        public static ObservableCollection<UIElement> CreateUIControlModels(List<UIControlSetting> allSettings, string stationName, bool drawing = true)
        {
            var elements = new ObservableCollection<UIElement>();

            // 指定されたStationNumberがListに存在するか確認
            if (allSettings.Any(list => list.StationName == stationName))
            {
                var stationList = allSettings.Where(list => list.StationName == stationName).ToList();
                foreach (var setting in allSettings)
                {
                    var element = ControlFactory.CreateControl(setting, stationList, drawing);
                    if (element != null)
                    {
                        elements.Add(element);
                    }
                }
            }
            else
            {
                throw new ArgumentException($"指定されたファイル名 '{stationName}' の設定データが見つかりません。");
            }
            return elements;
        }
    }
}
