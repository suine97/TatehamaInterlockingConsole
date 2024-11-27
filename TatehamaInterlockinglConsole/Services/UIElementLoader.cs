using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TatehamaInterlockinglConsole.Models;
using TatehamaInterlockinglConsole.Factories;
using System.Linq;

namespace TatehamaInterlockinglConsole.Helpers
{
    public class UIElementLoader
    {
        /// <summary>
        /// フォルダ内の全てのTSVファイルからUIControlSettingListを読み込み、Listに変換して返す
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public List<UIControlSetting> LoadSettingsFromFolderAsList(string folderPath)
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
        /// 指定されたStationNumberのUIControlSettingListを、ObservableCollection<UIElement>に変換して返す
        /// </summary>
        /// <param name="allSettings"></param>
        /// <param name="stationNumber"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public ObservableCollection<UIElement> GetElementsFromSettings(List<UIControlSetting> allSettings, string stationNumber)
        {
            var elements = new ObservableCollection<UIElement>();

            // 指定されたStationNumberがListに存在するか確認
            if (allSettings.Any(list => list.StationName == stationNumber))
            {
                var stationList = allSettings.Where(list => list.StationName == stationNumber).ToList();
                foreach (var setting in stationList)
                {
                    var control = ControlFactory.CreateControl(setting, stationList, false);
                    if (control != null)
                    {
                        elements.Add(control);
                    }
                }
            }
            else
            {
                throw new ArgumentException($"指定されたファイル名 '{stationNumber}' の設定データが見つかりません。");
            }
            return elements;
        }
    }
}
