using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TatehamaInterlockinglConsole.Models;
using TatehamaInterlockinglConsole.Factories;
using System.Linq;

namespace TatehamaInterlockinglConsole.Services
{
    public class UIElementLoader
    {
        /// <summary>
        /// フォルダ内の全てのTSVファイルからUIControlSettingリストをロードし、Listとして返す
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public List<UIControlSetting> LoadSettingsFromFolderAsDictionary(string folderPath)
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
        /// 指定されたStationNumberのUIControlSettingリストをObservableCollection<UIElement>に変換して返す
        /// </summary>
        /// <param name="settingsList"></param>
        /// <param name="stationNumber"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public ObservableCollection<UIElement> GetElementsFromSettings(List<UIControlSetting> settingsList, string stationNumber)
        {
            var elements = new ObservableCollection<UIElement>();

            // 指定されたStationNumberがListに存在するか確認
            if (settingsList.Any(list => list.StationNumber == stationNumber))
            {
                var stationList = settingsList.Where(list => list.StationNumber == stationNumber).ToList();
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

        /// <summary>
        /// 全コントロールのImagePathをList<UIAllImagePaths>に格納して返す
        /// </summary>
        /// <param name="settingsList"></param>
        /// <returns></returns>
        public List<UIAllImagePaths> GetAllImagePathsFromSettings(List<UIControlSetting> settingsList)
        {
            var list = new List<UIAllImagePaths>();

            foreach (var imagePaths in settingsList)
            {
                // ImagePatternが設定されていなければスキップ
                if (imagePaths.ImagePattern.Count <= 1)
                    continue;

                var dictionary = new Dictionary<int, string>();
                for (int i = 0; i < imagePaths.ImagePattern.Count; i++)
                {
                    dictionary[int.Parse(imagePaths.ImagePattern[i])] = imagePaths.ImagePaths[i];
                }

                list.Add(new UIAllImagePaths
                {
                    StationNumber = imagePaths.StationNumber,
                    UniqueName = imagePaths.UniqueName,
                    DefaultImage = imagePaths.DefaultImage,
                    ImagePaths = dictionary
                });
            }
            return list;
        }
    }
}
