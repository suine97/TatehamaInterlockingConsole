using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TatehamaInterlockinglConsole.Models;
using TatehamaInterlockinglConsole.Factories;

namespace TatehamaInterlockinglConsole.Services
{
    public class UIElementLoader
    {
        /// <summary>
        /// フォルダ内の全てのTSVファイルからUIControlSettingリストをロードし、ファイル名をキーとしたDictionaryとして返す
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public Dictionary<string, List<UIControlSetting>> LoadSettingsFromFolderAsDictionary(string folderPath)
        {
            var allSettings = new Dictionary<string, List<UIControlSetting>>();
            foreach (var filePath in Directory.EnumerateFiles(folderPath, "*.tsv"))
            {
                var settings = UIControlSettingLoader.LoadSettings(filePath);
                allSettings[Path.GetFileNameWithoutExtension(filePath)] = settings;
            }
            return allSettings;
        }

        /// <summary>
        /// 指定されたファイル名のUIControlSettingリストをObservableCollection<UIElement>に変換して返す
        /// </summary>
        /// <param name="settingsDictionary"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public ObservableCollection<UIElement> GetElementsFromSettings(Dictionary<string, List<UIControlSetting>> settingsDictionary, string fileName)
        {
            var elements = new ObservableCollection<UIElement>();

            // 指定されたファイル名が辞書に存在するか確認
            if (settingsDictionary.TryGetValue(fileName, out var settings))
            {
                foreach (var setting in settings)
                {
                    var control = ControlFactory.CreateControl(setting, settings, false);
                    if (control != null)
                    {
                        elements.Add(control);
                    }
                }
            }
            else
            {
                throw new ArgumentException($"指定されたファイル名 '{fileName}' の設定データが見つかりません。");
            }
            return elements;
        }
    }
}
