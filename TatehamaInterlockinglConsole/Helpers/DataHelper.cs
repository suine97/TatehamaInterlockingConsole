using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TatehamaInterlockingConsole.Manager;

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
        /// ファイルパスを基に駅名対照表から駅名を返す
        /// </summary>
        /// <param name="filePath">対象のファイルパス</param>
        /// <returns>駅名</returns>
        public static string ExtractStationNameFromFilePath(string filePath)
        {
            DataManager _dataManager = DataManager.Instance;

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            _dataManager.StationNameDictionary.TryGetValue(fileNameWithoutExtension, out var stationName);

            if (stationName == null)
            {
                return fileNameWithoutExtension;
            }
            return stationName;
        }

        /// <summary>
        /// TSVファイルを読み込んで駅名対照表の辞書データを返す
        /// </summary>
        /// <param name="folderPath">フォルダ名</param>
        /// <param name="fileName">読み込むファイル名</param>
        /// <returns></returns>
        public static Dictionary<string, string> LoadStationNameFromTSVAsDictionary(string folderPath, string fileName)
        {
            var stationNameDictionary = new Dictionary<string, string>();

            // ファイルパスを組み立てる
            string filePath = Path.Combine(folderPath, fileName);

            try
            {
                // ファイルの存在確認
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("指定されたTSVファイルが見つかりません。", filePath);
                }

                // Shift-JISエンコーディングでファイルを読み込む
                var encoding = Encoding.GetEncoding("Shift_JIS");
                var header = false;
                foreach (var line in File.ReadLines(filePath, encoding))
                {
                    // ヘッダー行はスキップ
                    if (!header)
                    {
                        header = true;
                        continue;
                    }

                    // 空行をスキップ
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // タブで分割して1列目と2列目を取得
                    var parts = line.Split('\t');
                    if (parts.Length < 2)
                    {
                        throw new FormatException($"不正な行フォーマット: {line}");
                    }

                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    // 辞書に追加
                    stationNameDictionary[key] = value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"エラー: {ex.Message}");
                throw;
            }
            return stationNameDictionary;
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
