using System.Collections.Generic;

namespace TatehamaInterlockinglConsole.Models
{
    /// <summary>
    /// UI設定用データクラス
    /// </summary>
    public class UIControlSetting
    {
        public string ControlType { get; set; }
        public string UniqueName { get; set; }
        public string ServerName { get; set; }
        public string ParentName { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
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
        public string ImagePattern { get; set; }
        public List<string> ImagePaths { get; set; } = new List<string>();
        public double RelativeX { get; set; }
        public double RelativeY { get; set; }
    }
}