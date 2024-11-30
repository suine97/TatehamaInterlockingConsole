using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using TatehamaInterlockingConsole.Manager;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole.Factories
{
    public static class RetsubanFactory
    {
        private static readonly DataManager _dataManager = DataManager.Instance;

        /// <summary>
        /// Retsubanコントロール作成処理
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="allSettings"></param>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public static UIElement CreateRetsubanImageControl(UIControlSetting setting, List<UIControlSetting> allSettings, bool drawing)
        {
            var canvas = new Canvas();

            // 列番情報を基に各画像を設定
            var imagePathList = ConvertRetsubanToImagePath(setting.Retsuban);

            // ユーティリティメソッドで設定を生成
            var controls = new (string UniqueNameSuffix, int X, int Y, string ImagePath)[]
            {
                ("_HeadSymbol", 1, 1, imagePathList[0]),
                ("_Number1000", 28, 1, imagePathList[1]),
                ("_Number100", 47, 1, imagePathList[2]),
                ("_Number10", 66, 1, imagePathList[3]),
                ("_Number1", 85, 1, imagePathList[4]),
                ("_TailSymbol", 104, 1, imagePathList[5])
            };

            var images = new List<UIElement>();

            foreach (var control in controls)
            {
                var uiControlSetting = CreateUIControlSetting(setting, control.UniqueNameSuffix, control.X, control.Y, control.ImagePath);
                var image = ImageFactory.CreateImageControl(uiControlSetting, allSettings, 0);

                Canvas.SetLeft(image, control.X);
                Canvas.SetTop(image, control.Y);
                images.Add(image);
            }

            // ベースイメージを作成
            var baseImage = ImageFactory.CreateImageControl(setting, allSettings, 0);
            if (drawing)
            {
                canvas.Children.Add(baseImage);
                foreach (var image in images)
                {
                    canvas.Children.Add(image);
                }
            }
            return canvas;
        }

        /// <summary>
        /// UIControlSettingを生成するヘルパーメソッド
        /// </summary>
        /// <param name="baseSetting"></param>
        /// <param name="uniqueNameSuffix"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        private static UIControlSetting CreateUIControlSetting(UIControlSetting baseSetting, string uniqueNameSuffix, int x, int y, string imagePath)
        {
            return new UIControlSetting
            {
                StationName = baseSetting.StationName,
                ControlType = baseSetting.ControlType,
                UniqueName = baseSetting.UniqueName + uniqueNameSuffix,
                ParentName = baseSetting.UniqueName,
                X = x,
                Y = y,
                ImagePaths = new Dictionary<int, string> { [0] = imagePath }
            };
        }

        /// <summary>
        /// Retsuban文字列を画像Pathに変換
        /// </summary>
        /// <param name="retsuban"></param>
        /// <returns></returns>
        public static string[] ConvertRetsubanToImagePath(string retsuban)
        {
            var imagePaths = new string[6];

            // 正規表現パターンの定義
            var pattern = @"([回試臨]?)([0-9]{0,4})(A|B|C|K|X|AX|BX|CX|KX)?$";
            var match = Regex.Match(retsuban, pattern);

            if (!match.Success) return imagePaths;

            // 先頭文字を取得し対応するパスを検索
            var headSymbolKey = GetHeadSymbolKey(match.Groups[1].Value);
            _dataManager.RetsubanImagePathDictionary.TryGetValue(headSymbolKey, out imagePaths[0]);

            // 数字部分を取得し各桁に対応するパスを検索（空白埋め対応）
            var digits = match.Groups[2].Value.PadLeft(4, ' '); // 空白埋め
            for (int i = 0; i < digits.Length; i++)
            {
                var charKey = digits[i] == ' ' ? "7seg_Null" : $"7seg_{digits[i]}";
                _dataManager.RetsubanImagePathDictionary.TryGetValue(charKey, out imagePaths[i + 1]);
            }

            // 接尾文字を取得し対応するパスを検索
            var tailSymbolKey = GetTailSymbolKey(match.Groups[3].Value);
            _dataManager.RetsubanImagePathDictionary.TryGetValue(tailSymbolKey, out imagePaths[5]);

            return imagePaths;
        }

        /// <summary>
        /// 先頭文字を取得するヘルパーメソッド
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        private static string GetHeadSymbolKey(string head)
        {
            if (head == "回") return "16dot_Kai";
            if (head == "試") return "16dot_Shi";
            if (head == "臨") return "16dot_Rin";
            return "16dot_Null";
        }

        /// <summary>
        /// 接尾文字を取得するヘルパーメソッド
        /// </summary>
        /// <param name="tail"></param>
        /// <returns></returns>
        private static string GetTailSymbolKey(string tail)
        {
            if (tail == "A") return "16dot_A";
            if (tail == "B") return "16dot_B";
            if (tail == "C") return "16dot_C";
            if (tail == "K") return "16dot_K";
            if (tail == "X") return "16dot_X";
            if (tail == "AX") return "16dot_AX";
            if (tail == "BX") return "16dot_BX";
            if (tail == "CX") return "16dot_CX";
            if (tail == "KX") return "16dot_KX";
            return "16dot_Null";
        }

        /// <summary>
        /// 列番表示画像Pathの一覧を取得
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetRetsubanImagePath(string folderPath)
        {
            var retsubanDictionary = new Dictionary<string, string>();

            foreach (var filePath in Directory.EnumerateFiles(folderPath, "*.png"))
            {
                retsubanDictionary.Add(Path.GetFileNameWithoutExtension(filePath), AppDomain.CurrentDomain.BaseDirectory + filePath);
            }
            return retsubanDictionary;
        }
    }
}
