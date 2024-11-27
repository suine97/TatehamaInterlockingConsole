using System.Collections.Generic;

namespace TatehamaInterlockinglConsole.Models
{
    /// <summary>
    /// UI設定用データクラス
    /// </summary>
    public class UIControlSetting
    {
        public string StationName { get; set; }
        public string ControlType { get; set; }
        public string UniqueName { get; set; }
        public string ServerName { get; set; }
        public string ParentName { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double RelativeX { get; set; }
        public double RelativeY { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Angle { get; set; }
        public double AngleOriginX { get; set; }
        public double AngleOriginY { get; set; }
        public string Text { get; set; }
        public double FontSize { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
        public string ClickEventName { get; set; }
        public List<string> ImagePattern { get; set; }
        public int DefaultImage { get; set; }
        public int CurrentImage { get; set; }
        public string BaseImagePath { get; set; }
        public Dictionary<int, string> ImagePaths { get; set; }
        public string Remark { get; set; }
    }
}