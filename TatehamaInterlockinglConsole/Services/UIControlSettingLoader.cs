using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using TatehamaInterlockingConsole.Helpers;
using TatehamaInterlockingConsole.Models;

namespace TatehamaInterlockingConsole.Services
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
            PointName = 4,
            PointValue = 5,
            X = 6,
            Y = 7,
            Width = 8,
            Height = 9,
            Angle = 10,
            AngleOriginX = 11,
            AngleOriginY = 12,
            Text = 13,
            FontSize = 14,
            BackgroundColor = 15,
            TextColor = 16,
            ClickEventName = 17,
            ImagePattern = 18, 
            ImageIndex = 19,
            BaseImagePath = 20,
            ImagePath = 21,
            Remark = 22
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
                        PointName = columns[(int)ColumnIndex.PointName],
                        PointValue = bool.TryParse(columns[(int)ColumnIndex.PointValue], out var pointvalue) && pointvalue,
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
                        ImageIndex = int.TryParse(columns[(int)ColumnIndex.ImageIndex], out var defaultImage) ? defaultImage : 0,
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
