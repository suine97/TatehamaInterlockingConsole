using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TatehamaInterlockinglConsole.Models;

namespace TatehamaInterlockinglConsole.Services
{
    public static class UIControlSettingLoader
    {
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
            ImagePath = 16
        }

        public static List<UIControlSetting> LoadSettings(string filePath)
        {
            var settings = new List<UIControlSetting>();
            bool header = false;
            foreach (var line in File.ReadAllLines(filePath, Encoding.GetEncoding("shift_jis")))
            {
                if (!header)
                {
                    header = true;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var columns = line.Split('\t');
                settings.Add(new UIControlSetting
                {
                    ControlType = columns[(int)ColumnIndex.ControlType],
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
                    ImagePaths = columns[(int)ColumnIndex.ImagePath].Split(',').Select(path => AppDomain.CurrentDomain.BaseDirectory + path.Trim('"').Trim()).ToList(),
                    SelectedImagePath = AppDomain.CurrentDomain.BaseDirectory + columns[(int)ColumnIndex.ImagePath].Split(',')[0].Trim('"').Trim(),
                });
            }
            return settings;
        }
    }
}
