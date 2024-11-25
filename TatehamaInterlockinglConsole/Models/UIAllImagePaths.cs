using System.Collections.Generic;

namespace TatehamaInterlockinglConsole.Models
{
    /// <summary>
    /// 全コントロールのImagePathデータ格納クラス
    /// </summary>
    public class UIAllImagePaths
    {
        public string StationNumber { get; set; }
        public string UniqueName { get; set; }
        public int DefaultImage { get; set; }
        public Dictionary<int, string> ImagePaths { get; set; }
    }
}
