using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// 指定した周期でtrue、falseを繰り返すメソッド
        /// </summary>
        /// <param name="intervalInMilliseconds">true/falseを切り替える周期（ミリ秒）</param>
        /// <returns></returns>
        public static bool GetAlternatingBoolean(int intervalInMilliseconds)
        {
            var currentTime = DateTime.Now;
            var totalMilliseconds = (int)currentTime.TimeOfDay.TotalMilliseconds;
            return (totalMilliseconds / intervalInMilliseconds) % 2 == 0;
        }

        /// <summary>
        /// 指定された数値が偶数であるかを判定
        /// </summary>
        /// <param name="number">判定対象の整数</param>
        /// <returns>偶数の場合はtrue、それ以外はfalse</returns>
        public static bool IsEvenNumber(int number)
        {
            return number % 2 == 0;
        }

        /// <summary>
        /// 文字列の中から抽出された数値が偶数であるかを判定
        /// </summary>
        /// <param name="input">判定対象の文字列</param>
        /// <returns>偶数の場合はtrue、それ以外はfalse</returns>
        public static bool IsEvenNumberInString(string input)
        {
            var digits = new string(input.Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(digits))
            {
                throw new ArgumentException("入力文字列に数字が含まれていません。", nameof(input));
            }
            int number = int.Parse(digits);
            return IsEvenNumber(number);
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
            var stationData = _dataManager.StationSettingList
                .FirstOrDefault(setting => setting.FileName.Contains(fileNameWithoutExtension));

            return stationData != null ? stationData.StationName : fileNameWithoutExtension;
        }

        /// <summary>
        /// 英語駅名を基に駅名対照表から日本語駅名を返す
        /// </summary>
        /// <param name="englishName">対象の英語駅名</param>
        /// <returns>駅名</returns>
        public static string GetStationNameFromEnglishName(string englishName)
        {
            DataManager _dataManager = DataManager.Instance;

            var stationData = _dataManager.StationSettingList
                .FirstOrDefault(setting => setting.FileName.Contains(englishName + "_UIList"));

            return stationData != null ? stationData.StationName : string.Empty;
        }

        /// <summary>
        /// 日本語駅名を基に駅名対照表からリストを返す
        /// </summary>
        /// <param name="stationName"></param>
        /// <returns></returns>
        public static List<string> GetDictionaryValuesFromStationName(string stationName)
        {
            DataManager _dataManager = DataManager.Instance;

            var matchingLists = _dataManager.StationSettingList
                .Where(setting => setting.StationName.Contains(stationName))
                .ToList();

            return matchingLists.Select(setting => setting.StationName).ToList();
        }

        /// <summary>
        /// 日本語駅名を基に駅名対照表から駅番号を返す
        /// </summary>
        /// <param name="stationName"></param>
        /// <returns></returns>
        public static string GetStationNumberFromStationName(string stationName)
        {
            DataManager _dataManager = DataManager.Instance;

            var matchingLists = _dataManager.StationSettingList
                .Where(setting => setting.StationName.Contains(stationName))
                .FirstOrDefault();

            var stationSetting = matchingLists;
            return stationSetting?.StationNumber ?? string.Empty;
        }

        /// <summary>
        /// 駅番号を基に駅名対照表から日本語駅名を返す
        /// </summary>
        /// <param name="stationNumber">対象の駅番号</param>
        /// <returns>日本語駅名</returns>
        public static string GetStationNameFromStationNumber(string stationNumber)
        {
            DataManager _dataManager = DataManager.Instance;

            var stationData = _dataManager.StationSettingList
                .FirstOrDefault(setting => setting.StationNumber == stationNumber);

            return stationData != null ? stationData.StationName : string.Empty;
        }

        /// <summary>
        /// TSVファイルを読み込んで辞書データを返す
        /// </summary>
        /// <param name="folderPath">フォルダ名</param>
        /// <param name="fileName">読み込むファイル名</param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> LoadTSVAsDictionary(string folderPath, string fileName)
        {
            var dictionary = new Dictionary<string, List<string>>();

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
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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
                    var values = new List<string>();
                    for (int i = 1; i < parts.Length; i++)
                    {
                        values.Add(parts[i].Trim());
                    }

                    // 辞書に登録
                    dictionary[key] = values;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"エラー: {ex.Message}");
                throw;
            }
            return dictionary;
        }

        /// <summary>
        /// TSVファイルを読み込んでリストデータを返す
        /// </summary>
        /// <param name="folderPath">フォルダ名</param>
        /// <param name="fileName">読み込むファイル名</param>
        /// <returns>リストデータ</returns>
        public static List<List<string>> LoadTSVAsList(string folderPath, string fileName)
        {
            var list = new List<List<string>>();

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
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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

                    // タブで分割して各列を取得
                    // タブで分割して各列を取得
                    var parts = line.Split('\t');
                    var values = new List<string>();
                    foreach (var part in parts)
                    {
                        values.Add(part.Trim().Replace("\\", string.Empty));
                    }

                    // リストに追加
                    list.Add(values);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"エラー: {ex.Message}");
                throw;
            }
            return list;
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
