using System;
using System.IO;

namespace TatehamaInterlockingConsole.Helpers
{
    /// <summary>
    /// 汎用データ操作ヘルパークラス
    /// </summary>
    public static class DataHelper
    {
        /// <summary>
        /// 指定された数値が偶数であるかを判定
        /// </summary>
        /// <param name="number">判定対象の整数</param>
        /// <returns>偶数の場合はtrue、それ以外はfalse</returns>
        public static bool IsEven(int number)
        {
            return number % 2 == 0;
        }

        /// <summary>
        /// 実行ファイルのディレクトリパスを取得
        /// </summary>
        /// <returns>実行ファイルのディレクトリパス</returns>
        public static string GetApplicationDirectory()
        {
            return Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        /// ファイルパスから駅名を抽出
        /// </summary>
        /// <param name="filePath">対象のファイルパス</param>
        /// <returns>駅名</returns>
        public static string ExtractStationNameFromFilePath(string filePath)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            var parts = fileNameWithoutExtension.Split('_');
            return parts.Length >= 2 ? $"{parts[0]}_{parts[1]}" : string.Empty;
        }
    }

    /// <summary>
    /// float型の拡張メソッド
    /// </summary>
    public static class FloatExtensionMethods
    {
        /// <summary>
        /// 値がゼロであるかを判定
        /// </summary>
        /// <param name="value">判定対象のfloat値</param>
        /// <returns>ゼロの場合はtrue、それ以外はfalse</returns>
        public static bool IsZero(this float value)
        {
            return value.IsZero(float.Epsilon);
        }

        /// <summary>
        /// 指定した許容誤差範囲で値がゼロであるかを判定
        /// </summary>
        /// <param name="value">判定対象のfloat値</param>
        /// <param name="epsilon">許容誤差</param>
        /// <returns>ゼロと見なせる場合はtrue、それ以外はfalse</returns>
        public static bool IsZero(this float value, float epsilon)
        {
            return Math.Abs(value) < epsilon;
        }
    }
}
