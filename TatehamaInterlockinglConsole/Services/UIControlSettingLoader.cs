using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using TatehamaInterlockinglConsole.Models;
using TatehamaInterlockingConsole.Helpers;

namespace TatehamaInterlockinglConsole.Helpers
{
    /// <summary>
    /// UIControlSettingList読込クラス
    /// </summary>
    public static class UIControlSettingLoader
    {
        /// <summary>
        /// TSVファイルの列要素番号
        /// </summary>
        public enum ColumnIndex
        {
            ControlType = 0,
            UniqueName = 1,
            ServerName = 2,
            ParentName = 3,
            X = 4,
            Y = 5,
            Width = 6,
            Height = 7,
            Angle = 8,
            AngleOriginX = 9,
            AngleOriginY = 10,
            Text = 11,
            FontSize = 12,
            BackgroundColor = 13,
            TextColor = 14,
            ClickEventName = 15,
            ImagePattern = 16, 
            DefaultImage = 17,
            BaseImagePath = 18,
            ImagePath = 19,
            Remark = 20
        }

        /// <summary>
        /// TSVファイルを読み込みUIControlSettingListを作成する
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<UIControlSetting> LoadSettings(string filePath)
        {
            try
            {
                var settings = new List<UIControlSetting>();
                bool header = false;
                foreach (var line in File.ReadAllLines(filePath, Encoding.GetEncoding("shift_jis")))
                {
                    // ヘッダー行はスキップ
                    if (!header)
                    {
                        header = true;
                        continue;
                    }

                    // 行に何も無ければスキップ
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var columns = line.Split('\t');

                    // ControlTypeが未定義ならスキップ
                    if (string.IsNullOrWhiteSpace(columns[(int)ColumnIndex.ControlType]))
                    {
                        continue;
                    }

                    // 駅名称抽出
                    var stationName = DataHelper.ExtractStationNameFromFilePath(filePath);

                    settings.Add(new UIControlSetting
                    {
                        StationName = stationName,
                        ControlType = columns[(int)ColumnIndex.ControlType] != string.Empty ? columns[(int)ColumnIndex.ControlType] : string.Empty,
                        UniqueName = columns[(int)ColumnIndex.UniqueName],
                        ServerName = columns[(int)ColumnIndex.ServerName],
                        ParentName = columns[(int)ColumnIndex.ParentName],
                        X = double.TryParse(columns[(int)ColumnIndex.X], out var x) ? x : 0,
                        Y = double.TryParse(columns[(int)ColumnIndex.Y], out var y) ? y : 0,
                        RelativeX = double.TryParse(columns[(int)ColumnIndex.X], out var relativeX) ? relativeX : 0,
                        RelativeY = double.TryParse(columns[(int)ColumnIndex.Y], out var relativeY) ? relativeY : 0,
                        Width = double.TryParse(columns[(int)ColumnIndex.Width], out var width) ? width : 0,
                        Height = double.TryParse(columns[(int)ColumnIndex.Height], out var height) ? height : 0,
                        Angle = double.TryParse(columns[(int)ColumnIndex.Angle], out var angle) ? angle : 0,
                        AngleOriginX = double.TryParse(columns[(int)ColumnIndex.AngleOriginX], out var angleX) ? angleX : 0.5,
                        AngleOriginY = double.TryParse(columns[(int)ColumnIndex.AngleOriginY], out var angleY) ? angleY : 0.5,
                        Text = columns[(int)ColumnIndex.Text],
                        FontSize = double.TryParse(columns[(int)ColumnIndex.FontSize], out var fontSize) ? fontSize : 10,
                        BackgroundColor = columns[(int)ColumnIndex.BackgroundColor],
                        TextColor = columns[(int)ColumnIndex.TextColor],
                        ClickEventName = columns[(int)ColumnIndex.ClickEventName],
                        ImagePattern = columns[(int)ColumnIndex.ImagePattern].Split(',').Select(pattern => pattern.Trim('"').Trim()).ToList(),
                        DefaultImage = int.TryParse(columns[(int)ColumnIndex.DefaultImage], out var defaultImage) ? defaultImage : 0,
                        CurrentImage = int.TryParse(columns[(int)ColumnIndex.DefaultImage], out var currentImage) ? currentImage : 0,
                        BaseImagePath = AppDomain.CurrentDomain.BaseDirectory + columns[(int)ColumnIndex.BaseImagePath].Trim('"').Trim(),
                        ImagePaths = CreateImagePaths(columns[(int)ColumnIndex.ImagePattern], columns[(int)ColumnIndex.ImagePath]),
                        Remark = columns[(int)ColumnIndex.Remark],
                    });
                }
                return settings;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// ImagePaths作成処理
        /// </summary>
        /// <param name="imagePatternColumn"></param>
        /// <param name="imagePathColumn"></param>
        /// <returns></returns>
        private static Dictionary<int, string> CreateImagePaths(string imagePatternColumn, string imagePathColumn)
        {
            var imagePaths = new Dictionary<int, string>();

            // ImagePatternとImagePathをそれぞれ分割して処理
            var patterns = imagePatternColumn
                .Split(',')
                .Select(p => p.Trim('"').Trim())
                .Where(p => int.TryParse(p, out _))
                .Select(int.Parse)
                .ToList();

            var paths = imagePathColumn
                .Split(',')
                .Select(p => p.Trim('"').Trim())
                .ToList();

            // パターンとパスの個数が多い方を取得
            var maxCount = Math.Max(patterns.Count, paths.Count);

            for (int i = 0; i < maxCount; i++)
            {
                // patternsとpathsを対応付け、インデックス範囲外の場合はデフォルト値を使用
                var pattern = i < patterns.Count ? patterns[i] : i;
                var path = i < paths.Count ? paths[i] : string.Empty;
                imagePaths[pattern] = AppDomain.CurrentDomain.BaseDirectory + path;
            }
            return imagePaths;
        }
    }
}
