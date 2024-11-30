using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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

        /// <summary>
        /// UIElementコレクション差分比較メソッド
        /// </summary>
        /// <param name="collection1"></param>
        /// <param name="collection2"></param>
        /// <returns></returns>
        public static bool AreCollectionsEqual(ObservableCollection<UIElement> collection1, ObservableCollection<UIElement> collection2)
        {
            if (collection1.Count != collection2.Count)
                return false;

            for (int i = 0; i < collection1.Count; i++)
            {
                if (!AreUIElementsEqual(collection1[i], collection2[i]))
                {
                    return false;
                }
            }
            return true;
        }
        private static bool AreUIElementsEqual(UIElement element1, UIElement element2)
        {
            // カスタムロジックを使用してUIElementを比較
            if (element1 == null || element2 == null)
                return element1 == element2;

            if (element1 is Canvas canvas1 && element2 is Canvas canvas2)
            {
                return canvas1.Children.Count == canvas2.Children.Count &&
                       CompareCanvasChildren(canvas1.Children, canvas2.Children);
            }

            if (element1 is Grid grid1 && element2 is Grid grid2)
            {
                return grid1.Children.Count == grid2.Children.Count;
            }

            if (element1 is TextBlock text1 && element2 is TextBlock text2)
            {
                return text1.Text.ToString() == text2.Text.ToString();
            }

            if (element1 is Image image1 && element2 is Image imate2)
            {
                return image1.Source.ToString() == imate2.Source.ToString();
            }

            if (element1 is Label label1 && element2 is Label label2)
            {
                return label1.Content.ToString() == label2.Content.ToString();
            }

            return false;
        }
        private static bool CompareCanvasChildren(UIElementCollection children1, UIElementCollection children2)
        {
            if (children1.Count != children2.Count)
                return false;

            for (int i = 0; i < children1.Count; i++)
            {
                if (!AreUIElementsEqual(children1[i], children2[i]))
                {
                    return false;
                }
            }
            return true;
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
